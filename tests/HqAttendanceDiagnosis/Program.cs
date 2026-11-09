using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Models.Repos;
using Models.Repos.Models;
using Services;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;
using MvvmHelpers;

if (!args.Contains("--sql-localdb")) throw new Exception("Explicit --sql-localdb required. No factory config is read.");
var checks = 0;
void Check(bool value, string label)
{
    if (!value) throw new Exception("FAIL: " + label);
    checks++;
    Console.WriteLine("PASS: " + label);
}
var database = "RCQ_AttendanceDiag_" + Guid.NewGuid().ToString("N");
var connection = @"Server=(localdb)\MSSQLLocalDB;Database=" + database + ";Integrated Security=True;TrustServerCertificate=True";
using var schema = new SchemaDb(connection);
var previousEfConnection = AppViewModels.Base.Ins.ConnectionString2;
var machine = new MayCan { Id = "9876543", WKv = AppKV.Hq, MType = "BOARD", ConnectionId = "BENCH_ONLY",
    MaThanhPham = "BV", MaLoaiNguyenLieu = "XOAIDONG", MaXuong = "1", MaLo="LO", MaSize="20X20", IsConnected=true };
var sends = new List<(string Target, object?[] Arguments)>();
var client = Mock<ISingleClientProxy>((method, args) =>
{
    if (method.Name != "SendCoreAsync") throw new Exception(method.Name);
    sends.Add(((string)args![0]!, (object?[])args[1]!));
    return Task.CompletedTask;
});
var clients = Mock<IHubClients>((method, _) => method.Name == "Client" ? client : throw new Exception(method.Name));
var hub = Mock<IHubContext<ChatHub>>((method, _) => method.Name == "get_Clients" ? clients : throw new Exception(method.Name));
// Skip constructors that load plant caches/background jobs; call the real command methods.
var service = (MayCansService)RuntimeHelpers.GetUninitializedObject(typeof(MayCansService));
typeof(MayCansService).GetField("items", BindingFlags.NonPublic | BindingFlags.Instance)!
    .SetValue(service, new ConcurrentDictionary<string, MayCan>(new[] { new KeyValuePair<string, MayCan>(machine.Id, machine) }));
typeof(MayCansService).GetField("hubContext", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(service, hub);

try
{
    await schema.Database.EnsureCreatedAsync();
    schema.AddRange(new HQ_ThanhPham { Id = "BV", Ten = "OLD", SuDung = true },
        new HQ_ThanhPham { Id = "BVT1", Ten = "NEW", SuDung = true },
        new HQ_LoaiNguyenLieu { Id = "XOAIDONG", Ten = "MATERIAL", SuDung = true },
        new HQ_LoaiNguyenLieu { Id = "XOAITUOI", Ten = "NEW MATERIAL", SuDung = true });
    await schema.SaveChangesAsync();
    var at = ScaleWorkshopHistory.BusinessNow().AddMinutes(-10);
    at = at.AddTicks(-(at.Ticks % TimeSpan.TicksPerSecond));
    var employee = new NhanVienDaiThanh { MaNhanVien = "BENCH_NV", IsHuman = true };
    ScaleWorkContext Current() => ScaleWorkContext.Create(machine.MaThanhPham, machine.MaLoaiNguyenLieu, "1");
    (bool Success, string Json) Enter(string card, DateTime time)
    {
        var selection = HqScaleSelection.Capture(machine);
        if (selection.Error != null) return (false, selection.Error);
        using var db = new TestDb(connection, schema.Model);
        using var tx = db.Database.BeginTransaction();
        ScaleAttendanceTransferService.LockCard(db, card);
        var result = ScaleOnlineAttendanceService.Process(db, new ScaleAttendanceProcessor.Request
        { TheId = card, ChamCongId = machine.Id + "." + time.ToString("yyyyMMddHHmmss"),
            NgayGio = time.ToString("yyyyMMddHHmmss"), TrangThai = "Vào" },
            int.Parse(machine.Id), "BENCH", time, Current(), employee, "BENCH", 16);
        if (result.Success) tx.Commit();
        return result;
    }

    await service.CommandSetThanhPham(machine.Id, "BVT1", "NEW");
    Check(sends.Last().Target == "SETTHANHPHAM" && (string?)sends.Last().Arguments[0] == "BVT1",
        "Real web Set sends NEW BVT1 to firmware");
    Check(machine.MaThanhPham == "BV" && HqScaleSelection.Capture(machine).Error == HqScaleSelection.Waiting,
        "Old selector remains confirmed, but new work is explicitly blocked until echo");
    Check(!Enter("BENCH_PRODUCT", at).Success && !schema.Set<CheckInOut>().Any(),
        "No check-in using old product during Set/echo gap");
    var identity = new ClientInfo {Id=machine.Id,WKv=AppKV.Hq,IsActive=true,IsMayCan=true};
    var runtimeMachines=new Dictionary<string,MayCan>{{machine.Id,machine}};
    Action? blockDisplay=null;
    Task<bool>? blockIdentity=null;
    object? MachineCall(MethodInfo method,object?[]? values)
    {
        switch(method.Name)
        {
            case "Find": return runtimeMachines.GetValueOrDefault((string)values![0]!);
            case "NotifyItemChanged": return null;
            case "AddItem": blockDisplay?.Invoke(); return null;
            case "CommandSetNhanVien": return blockIdentity ?? Task.FromResult(true);
            default: throw new Exception(method.Name);
        }
    }
    var machines = Mock<IMayCansService>(MachineCall);
    var users = Mock<IUserService>((_, _) => identity);
    var noMain = Mock<IMainService>((m, _) => throw new Exception("Unexpected cache/production access: "+m.Name));
    var commit = new TestCommit(machines,noMain,connection,schema.Model);
    var router = new CommunicationService(users,commit,Options.Create(new ScaleAttendanceOptions()),machines);
    var request = new ScaleAttendanceProcessor.Request {TheId="BENCH_PRODUCT",ChamCongId=machine.Id+"."+at.ToString("yyyyMMddHHmmss"),NgayGio=at.ToString("yyyyMMddHHmmss"),TrangThai="Vào"};
    var payload=JsonConvert.SerializeObject(request);
    var blocked=JObject.Parse(await router.Route(identity,"CHAMCONG",payload.Length,payload));
    Check((bool?)blocked["IsError"]==true && JObject.Parse((string)blocked["DataError"]!).Value<string>("Code")=="WORK_CONTEXT_PENDING",
        "Actual CHAMCONG router rejects pending Set before opening production SQL");
    var weight=new DataLite {Id=machine.Id+"."+at.ToString("yyyyMMddHHmmss"),NgayGio=at.ToString("yyyyMMddHHmmss"),TheId="BENCH_PRODUCT",Stated="ST",TrongLuong=1.4m};
    var blockedWeight=await commit.CommitLite(JsonConvert.SerializeObject(weight),identity,"LITE");
    Check(!blockedWeight.Item1 && JsonConvert.DeserializeObject<DataError>(blockedWeight.Item2)!.SaveStatus==HqOnlineResponse.NotSaved &&
        !schema.Set<HQ_PhieuCan>().Any(),"Actual CommitLite does not INSERT during pending Set");
    var replayAt=at.AddSeconds(-1);
    var replay=new DataLite {Id=machine.Id+"."+replayAt.ToString("yyyyMMddHHmmss"),NgayGio=replayAt.ToString("yyyyMMddHHmmss"),TheId="BENCH_PRODUCT",Stated="ST",TrongLuong=1.4m};
    schema.Add(new HQ_PhieuCan {Id=replay.Id,Ngay=DateOnly.FromDateTime(replayAt),NgayGio=replayAt,MayCan=machine.Id,
        MaLo="LO",MaSize="20X20",MaThanhPham="BV",MaLoaiNguyenLieu="XOAIDONG",MaXuong="1",MaNhanVien="BENCH_NV",
        TheId=replay.TheId,TheIdNhanVien=replay.TheId,TrongLuong=1.4m,ChiSanLuong=true,Status=1});
    await schema.SaveChangesAsync();
    var replayResponse=await commit.CommitLite(JsonConvert.SerializeObject(replay),identity,"LITE");
    Check(replayResponse.Item1 && JsonConvert.DeserializeObject<DataLiteRes>(replayResponse.Item2)!.SaveStatus==HqOnlineResponse.Saved &&
        schema.Set<HQ_PhieuCan>().Count()==1,"Verified saved-Id replay still returns complete totals while a new Set is pending");
    Check(HqScaleSelection.CheckAttendance(machine,HqScaleSelection.Capture(machine),"Ra")==null,
        "Ra remains permitted while a product Set is pending");
    DataCMD Item(string property,string value)=>new(){PropetyName=property,PropetyValue=value,CommandName="TEST"};
    var confirmed=HqScaleSelection.ApplyFirmware(machine,"OLD_CONNECTION",new[]{Item(nameof(machine.MaThanhPham),"BVT1")});
    Check(confirmed.Count==0 && HqScaleSelection.Capture(machine).Error!=null,"Old connection cannot confirm pending Set");
    HqScaleSelection.ApplyFirmware(machine,machine.ConnectionId,new[]{Item(nameof(machine.MaThanhPham),"BV")});
    Check(machine.MaThanhPham=="BV" && HqScaleSelection.Capture(machine).Error!=null,"Old product echo cannot clear pending new product");
    var converter=Mock<IConverterService>((_,_)=>"NEW NAME");
    var callerClients=Mock<IHubCallerClients>((m,_)=>m.Name=="get_Caller"?client:throw new Exception(m.Name));
    using var actualHub=new ChatHub(users,null!,null!,machines,converter,NullLogger<ChatHub>.Instance)
    { Context=new TestContext(machine.ConnectionId),Clients=callerClients };
    await actualHub.SendSelectedData2(machine.Id,AppKV.Hq,JsonConvert.SerializeObject(new[]{Item(nameof(machine.MaThanhPham),"BVT1")}));
    Check(machine.MaThanhPham=="BVT1" && HqScaleSelection.Capture(machine).Error==null,
        "Actual SendSelectedData2 confirms new product without creating another pending Set");
    Check(sends.Last().Target=="SETTHANHPHAM" && sends.Last().Arguments.Length==2,"BOARD echo retains original two-argument firmware command");
    Check(Enter("BENCH_PRODUCT", at).Success,"Check-in uses confirmed new product");
    using (var db = new TestDb(connection, schema.Model))
    {
        var error = await ScaleProductionAttendancePolicy.GetOnlineErrorAsync(db, "BENCH_PRODUCT", at.AddSeconds(1),
            16, employee, Current(), CancellationToken.None);
        Check(error == null, "First weight after confirmed product check-in is valid, no reswipe needed");
        Check(ScaleOnlineAttendanceService.Snapshot(db, machine.Id + "." + at.ToString("yyyyMMddHHmmss"))!.MaThanhPham == "BVT1",
            "Recorded snapshot contains the confirmed new product");
    }
    await service.CommandSetNguyenLieu(machine.Id, "XOAITUOI", "NEW MATERIAL");
    Check(sends.Last().Target == "SETLOAINGUYENLIEU" && machine.MaLoaiNguyenLieu == "XOAIDONG",
        "Material remains old while pending confirmation");
    Check(HqScaleSelection.Capture(machine).Error!=null,"Material Set also blocks opening a mismatched shift");
    await actualHub.SendSelectedData(machine.Id,AppKV.Hq,JsonConvert.SerializeObject(new[]{Item(nameof(machine.MaLoaiNguyenLieu),"XOAITUOI")}));
    Check(machine.MaLoaiNguyenLieu=="XOAITUOI" && HqScaleSelection.Capture(machine).Error==null,"SendSelectedData (without echo) also confirms material");

    var captured=HqScaleSelection.Capture(machine);
    HqScaleSelection.ApplyFirmware(machine,machine.ConnectionId,new[]{Item(nameof(machine.MaSize),"10X10")});
    Check(HqScaleSelection.Check(machine,captured)==HqScaleSelection.Changed,"Queued request cannot silently use a selector revision that changed");
    await service.CommandSetThanhPham(machine.Id,"BV","OLD");
    var blockedOpposite=false;
    try {await service.CommandSetThanhPham(machine.Id,"BVT1","NEW");} catch(InvalidOperationException) {blockedOpposite=true;}
    Check(blockedOpposite,"A second different web Set cannot overtake an unconfirmed first Set");
    HqScaleSelection.ApplyFirmware(machine,machine.ConnectionId,new[]{Item(nameof(machine.MaThanhPham),"BV")});
    var failed=new MayCan {Id="9876555",ConnectionId="FAILED",MaThanhPham="BV"};
    try {await HqScaleSelection.SendChangeAsync(failed,nameof(machine.MaThanhPham),"BVT1",()=>throw new IOException("test send failure"));} catch(IOException) { }
    Check(HqScaleSelection.Capture(failed).Error!=null,"Send failure never silently returns old configuration to ready");
    Check(HqScaleSelection.ApplyFirmware(machine,machine.ConnectionId,new[]{Item(nameof(machine.MaThanhPham),"BV"),Item(nameof(machine.MaThanhPham),"BVT1")}).Count==0,
        "Conflicting duplicate properties in one firmware batch are rejected atomically");
    var independent=new MayCan {Id="9876556",MaThanhPham="BV"};
    Check(HqScaleSelection.Capture(independent).Error==null,"Pending state on one scale does not block another scale");
    await service.CommandSetLo(machine.Id,"NEW_LO");
    await service.CommandSetSize(machine.Id,"30X30","NEW SIZE");
    HqScaleSelection.ApplyFirmware(machine,machine.ConnectionId,new[]{Item(nameof(machine.MaLo),"NEW_LO")});
    Check(HqScaleSelection.Capture(machine).Error!=null,"Confirming lot alone does not clear a still-pending size change");
    HqScaleSelection.ApplyFirmware(machine,machine.ConnectionId,new[]{Item(nameof(machine.MaSize),"30X30")});
    Check(HqScaleSelection.Capture(machine).Error==null && machine.MaLo=="NEW_LO" && machine.MaSize=="30X30",
        "All pending lot/size fields must be confirmed before accepting a new ticket");
    // Expiry changes the message, NEVER returns stale configuration to Ready.
    var delayed=new MayCan {Id="9876557",ConnectionId="DELAYED",MaThanhPham="BV"};
    await HqScaleSelection.SendChangeAsync(delayed,nameof(machine.MaThanhPham),"BVT1",()=>Task.CompletedTask);
    var states=typeof(HqScaleSelection).GetField("States",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!;
    var state=states.GetType().GetMethod("GetOrCreateValue")!.Invoke(states,new object[]{delayed})!;
    var pending=(System.Collections.IDictionary)state.GetType().GetField("Pending")!.GetValue(state)!;
    var pendingType=pending[nameof(machine.MaThanhPham)]!.GetType();
    pending[nameof(machine.MaThanhPham)]=Activator.CreateInstance(pendingType,new object[]{"BVT1",System.Diagnostics.Stopwatch.GetTimestamp()-11L*System.Diagnostics.Stopwatch.Frequency})!;
    Check(HqScaleSelection.Capture(delayed).Error==HqScaleSelection.Unconfirmed,"Lost echo stays blocked after timeout, with a short Set-again instruction");
    await HqScaleSelection.SendChangeAsync(delayed,nameof(machine.MaThanhPham),"BV",()=>Task.CompletedTask);
    Check(HqScaleSelection.ApplyFirmware(delayed,"DELAYED",new[]{Item(nameof(machine.MaThanhPham),"BVT1")}).Count==0,
        "Old delayed echo cannot confirm a newer operator retry value");
    HqScaleSelection.ApplyFirmware(delayed,"DELAYED",new[]{Item(nameof(machine.MaThanhPham),"BV")});
    Check(HqScaleSelection.Capture(delayed).Error==null,"Operator retry recovers only after matching confirmation");
    foreach(var invalid in new[]{(string?)null,"", "Server=.;Database=PMS_HQ;User Id=;Password=;"})
    {
        var rejected=false;try {HqDatabaseStartup.Validate(invalid,invalid);}catch(InvalidOperationException) {rejected=true;}
        Check(rejected,"Startup refuses empty/incomplete SQL settings without exposing credentials");
    }
    HqDatabaseStartup.Validate(connection,connection);
    await HqDatabaseStartup.ProbeAsync(connection);
    Check(true,"Startup accepts matching initialized connections and probes private SQL with SELECT 1 only");
    var mismatch=false;
    try {HqDatabaseStartup.Validate("Server=.;Database=A;User Id=TEST;Password=TEST_SECRET;","Server=.;Database=B;User Id=TEST;Password=TEST_SECRET;");}
    catch(InvalidOperationException ex) {mismatch=!ex.Message.Contains("TEST_SECRET");}
    Check(mismatch,"DAO/EF pointing at different databases is rejected without printing credentials");
    var missing=false;
    try {await HqDatabaseStartup.InitializeAsync(Path.Combine(Path.GetTempPath(),"RCQ_NoConfig_"+Guid.NewGuid().ToString("N")));}
    catch(InvalidOperationException) {missing=true;}
    Check(missing,"Missing config fails before lazy AppViewModel/background services are started");
    var cancelled=false;
    try {await HqDatabaseStartup.ProbeAsync(connection,new CancellationToken(true));}
    catch(InvalidOperationException ex) {cancelled=ex.InnerException==null;}
    Check(cancelled,"Cancelled SQL startup probe fails closed with a sanitized exception");

    // Full public CHAMCONG router -> real processor -> SQL -> public CommitLite.
    // Only this test process's EF setting is pointed to the guarded GUID database.
    AppViewModels.Base.Ins.ConnectionString2=connection;
    var routeCard=new TheTu {MaTheTu="0001234567",MaNhanVien="BENCH_NV"};
    var routeEmployee=new NhanVienDaiThanh {MaNhanVien="BENCH_NV",MaHoSo="BENCH_NV",Name="BENCH",IsHuman=true};
    schema.Add(routeCard);schema.Add(routeEmployee);
    void AddRegisteredMachine(string id)
    {
        var row=new MayCan {Id=id,DisplayName="BENCH",WKv=AppKV.Hq,MType="BOARD",MaXuong="1"};
        // Old MayCan model has required ancillary strings (Par1/Par2/Par3/SCode).
        foreach(var property in schema.Model.FindEntityType(typeof(MayCan))!.GetProperties()
            .Where(x=>x.ClrType==typeof(string) && !x.IsNullable && x.PropertyInfo?.GetValue(row)==null))
            property.PropertyInfo!.SetValue(row,"");
        schema.Add(row);
    }
    AddRegisteredMachine(machine.Id);
    schema.Add(new LichSuXuongMayCan {MayCan=machine.Id,MaXuong="1",TuThoiGian=at.AddDays(-1)});
    foreach(var entry in schema.ChangeTracker.Entries().Where(x=>x.State==EntityState.Added))
        foreach(var property in entry.Properties.Where(x=>x.Metadata.ClrType==typeof(string) &&
            !x.Metadata.IsNullable && x.CurrentValue==null)) property.CurrentValue="";
    await schema.SaveChangesAsync();
    object Cache(string property,object values)
    {
        var type=typeof(IMainService).GetProperty(property)!.PropertyType;
        var cache=RuntimeHelpers.GetUninitializedObject(type);
        type.GetField("items",BindingFlags.Instance|BindingFlags.NonPublic)!.SetValue(cache,values);
        return cache;
    }
    var cachedCards=new ObservableRangeCollection<TheTu>{routeCard};
    var cachedPeople=new ObservableRangeCollection<NhanVienDaiThanh>{routeEmployee};
    var cardsCache=Cache("VmThe",cachedCards);
    var peopleCache=Cache("VmNhanVien",cachedPeople);
    var cachedMain=Mock<IMainService>((m,_)=>m.Name switch
    {"get_VmThe"=>cardsCache,"get_VmNhanVien"=>peopleCache,_=>throw new Exception("Unexpected main access "+m.Name)});
    var publicCommit=new TestCommit(machines,cachedMain,connection,schema.Model);
    var publicRouter=new CommunicationService(users,publicCommit,Options.Create(new ScaleAttendanceOptions()),machines);
    async Task<JObject> Scan(ClientInfo connected,DateTime time,string direction)
    {
        var json=JsonConvert.SerializeObject(new ScaleAttendanceProcessor.Request {TheId=routeCard.MaTheTu,
            ChamCongId=connected.Id+"."+time.ToString("yyyyMMddHHmmss"),NgayGio=time.ToString("yyyyMMddHHmmss"),TrangThai=direction});
        return JObject.Parse(await publicRouter.Route(connected,"CHAMCONG",json.Length,json));
    }
    async Task<bool> Weigh(ClientInfo connected,DateTime time)
    {
        var json=JsonConvert.SerializeObject(new DataLite {Id=connected.Id+"."+time.ToString("yyyyMMddHHmmss"),
            NgayGio=time.ToString("yyyyMMddHHmmss"),TheId=routeCard.MaTheTu,Stated="ST",TrongLuong=1.4m});
        var reply=JObject.Parse(await publicRouter.Route(connected,"LITE",json.Length,json));
        return (bool?)reply["IsError"]==false;
    }
    Check((bool?)(await Scan(identity,at.AddSeconds(40),"Vào"))["IsError"]==false && await Weigh(identity,at.AddSeconds(41)),
        "Full public route: CHAMCONG Vao commits then first LITE succeeds with a real normal employee");
    await service.CommandSetThanhPham(machine.Id,"BVT1","NEW");
    Check((bool?)(await Scan(identity,at.AddSeconds(42),"Vào"))["IsError"]==true && !await Weigh(identity,at.AddSeconds(43)),
        "Full public route: Set in flight does not create an old-code shift or ticket");
    await actualHub.SendSelectedData(machine.Id,AppKV.Hq,JsonConvert.SerializeObject(new[]{Item(nameof(machine.MaThanhPham),"BVT1")}));
    Check((bool?)(await Scan(identity,at.AddSeconds(45),"Vào"))["IsError"]==false && await Weigh(identity,at.AddSeconds(46)),
        "Full public route: changed product closes old shift, opens new one, next LITE succeeds first time");
    var repeated=await Scan(identity,at.AddSeconds(47),"Vào");
    Check((bool?)repeated["IsError"]==true && JObject.Parse((string)repeated["DataError"]!).Value<string>("Code")=="ALREADY_CHECKED_IN",
        "Full public route: repeated Vao same context keeps the root instead of creating a new shift");
    var second=new MayCan {Id="9876544",WKv=AppKV.Hq,MType="BOARD",ConnectionId="BENCH_SECOND",MaLo=machine.MaLo,MaSize=machine.MaSize,
        MaThanhPham=machine.MaThanhPham,MaLoaiNguyenLieu=machine.MaLoaiNguyenLieu,MaXuong="1"};
    runtimeMachines[second.Id]=second;
    AddRegisteredMachine(second.Id);
    schema.Add(new LichSuXuongMayCan {MayCan=second.Id,MaXuong="1",TuThoiGian=at.AddDays(-1)});
    await schema.SaveChangesAsync();
    var secondIdentity=new ClientInfo {Id=second.Id,WKv=AppKV.Hq,IsMayCan=true,IsActive=true};
    Check(await Weigh(secondIdentity,at.AddSeconds(48)),"Full public route: another scale same context weighs without extra Vao");
    await service.CommandSetNguyenLieu(machine.Id,"XOAIDONG","OLD MATERIAL");
    await actualHub.SendSelectedData2(machine.Id,AppKV.Hq,JsonConvert.SerializeObject(new[]{Item(nameof(machine.MaLoaiNguyenLieu),"XOAIDONG")}));
    Check((bool?)(await Scan(identity,at.AddSeconds(50),"Vào"))["IsError"]==false && await Weigh(identity,at.AddSeconds(51)),
        "Full public route: material-only change also switches shift and accepts the next weight");
    Check((bool?)(await Scan(secondIdentity,at.AddSeconds(52),"Ra"))["IsError"]==false && !await Weigh(secondIdentity,at.AddSeconds(53)),
        "Full public route: Ra at another scale closes root; following weight correctly requires Vao");
    Check(schema.Set<HQ_PhieuCan>().Count(x=>x.TheIdNhanVien==routeCard.MaTheTu)==4,
        "Full public route: exactly four accepted weights, zero writes from blocked/repeated requests");
    var throwingLogger=Mock<Microsoft.Extensions.Logging.ILogger<CommunicationService>>((_,_)=>throw new IOException("test logging failure"));
    var logFailureRouter=new CommunicationService(users,publicCommit,Options.Create(new ScaleAttendanceOptions()),machines,throwingLogger);
    var logRequest=JsonConvert.SerializeObject(new ScaleAttendanceProcessor.Request {TheId=routeCard.MaTheTu,
        ChamCongId=machine.Id+"."+at.AddSeconds(54).ToString("yyyyMMddHHmmss"),NgayGio=at.AddSeconds(54).ToString("yyyyMMddHHmmss"),TrangThai="Vào"});
    var logReply=JObject.Parse(await logFailureRouter.Route(identity,"CHAMCONG",logRequest.Length,logRequest));
    Check((bool?)logReply["IsError"]==false,"Logging provider failure after COMMIT does not turn a successful Vao into an error");

    // Real public WSS -> router -> COMMIT/totals, then deliberately bad display/network.
    await HqOnlineUi.WhenIdleAsync(machine).WaitAsync(TimeSpan.FromSeconds(3));
    async Task SendWeight(ChatHub target,int seconds)
    {
        var time=at.AddSeconds(seconds);
        var json=JsonConvert.SerializeObject(new DataLite {Id=machine.Id+"."+time.ToString("yyyyMMddHHmmss"),
            NgayGio=time.ToString("yyyyMMddHHmmss"),TheId=routeCard.MaTheTu,Stated="ST",TrongLuong=1.4m});
        await target.SendData("LITE",json.Length,json);
    }
    using(var releaseDisplay=new ManualResetEventSlim(false))
    {
        var uiEntered=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        blockDisplay=()=>{uiEntered.TrySetResult();releaseDisplay.Wait();};
        var outgoing=new List<string>();
        var output=Mock<ISingleClientProxy>((_,a)=>{outgoing.Add((string)((object?[])a![1]!)[1]!);return Task.CompletedTask;});
        using var target=new ChatHub(users,publicRouter,null!,machines,null!,NullLogger<ChatHub>.Instance)
        {Context=new TestContext(machine.ConnectionId),Clients=Mock<IHubCallerClients>((_,_)=>output)};
        try
        {
            await SendWeight(target,70).WaitAsync(TimeSpan.FromSeconds(3));
            await uiEntered.Task.WaitAsync(TimeSpan.FromSeconds(3));
            Check(outgoing.Count==1 && HqFirmwareSend.SaveStatus(outgoing[0])=="SAVED" && !releaseDisplay.IsSet,
                "Real SQL weight returns complete SAVED response while display callback remains blocked");
        }
        finally {releaseDisplay.Set();blockDisplay=null;}
        await HqOnlineUi.WhenIdleAsync(machine).WaitAsync(TimeSpan.FromSeconds(3));
    }
    var pendingSend=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var attemptedReplies=new List<string>();
    var stuckOutput=Mock<ISingleClientProxy>((_,a)=>
    {attemptedReplies.Add((string)((object?[])a![1]!)[1]!);return pendingSend.Task;});
    var failedContext=new TestContext(machine.ConnectionId);
    using(var target=new ChatHub(users,publicRouter,null!,machines,null!,NullLogger<ChatHub>.Instance)
    {Context=failedContext,Clients=Mock<IHubCallerClients>((_,_)=>stuckOutput)})
    {
        var before=schema.Set<HQ_PhieuCan>().Count();
        var timer=System.Diagnostics.Stopwatch.StartNew();
        await SendWeight(target,72).WaitAsync(TimeSpan.FromSeconds(7));
        Check(timer.ElapsedMilliseconds>=2800 && timer.ElapsedMilliseconds<6500 && failedContext.AbortCalled,
            "Hung ReceiveData send is bounded at three seconds and original connection is aborted");
        Check(attemptedReplies.Count==1 && HqFirmwareSend.SaveStatus(attemptedReplies[0])=="SAVED" &&
            schema.Set<HQ_PhieuCan>().Count()==before+1,
            "Send timeout after COMMIT neither retries INSERT nor sends contradictory fallback");
        pendingSend.TrySetResult();
        Check(attemptedReplies.Count==1,"Late completion never causes another final packet");
    }
    var pendingIdentity=new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    blockIdentity=pendingIdentity.Task;
    var identityContext=new TestContext(machine.ConnectionId); var finalAttempts=0;
    var unexpectedFinal=Mock<ISingleClientProxy>((_,_)=>{finalAttempts++;return Task.CompletedTask;});
    try
    {
        using var target=new ChatHub(users,publicRouter,null!,machines,null!,NullLogger<ChatHub>.Instance)
        {Context=identityContext,Clients=Mock<IHubCallerClients>((_,_)=>unexpectedFinal)};
        var before=schema.Set<HQ_PhieuCan>().Count();
        await SendWeight(target,74).WaitAsync(TimeSpan.FromSeconds(6));
        Check(identityContext.AbortCalled && finalAttempts==0 && schema.Set<HQ_PhieuCan>().Count()==before+1,
            "Identity send timeout preserves committed ticket and does not queue final frame behind stuck identity");
    }
    finally {blockIdentity=null;pendingIdentity.TrySetResult(true);}
    var successPackets=0;
    var slowLogAttempts=0;
    var throwingHubLogger=Mock<Microsoft.Extensions.Logging.ILogger<ChatHub>>((_,_)=>
    {slowLogAttempts++;throw new IOException("test log after send");});
    // Test-only slow send triggers HQ WSS SLOW logging AFTER successful final send.
    var successfulOutput=Mock<ISingleClientProxy>((_,_)=>{successPackets++;return Task.Delay(2050);});
    using(var target=new ChatHub(users,publicRouter,null!,machines,null!,throwingHubLogger)
    {Context=new TestContext(machine.ConnectionId),Clients=Mock<IHubCallerClients>((_,_)=>successfulOutput)})
        await SendWeight(target,76).WaitAsync(TimeSpan.FromSeconds(5));
    Check(successPackets==1 && slowLogAttempts==1,"Logger failure cannot resend an already completed final response");
    await HqOnlineUi.WhenIdleAsync(machine).WaitAsync(TimeSpan.FromSeconds(3));

    checks += await GroupAttendanceChecks.RunAsync(schema, at.AddSeconds(200), publicRouter,
        machine, second, cachedCards, cachedPeople);
    checks += await WorkshopAttendanceChecks.RunAsync(schema);
    checks += await AttendanceModeChecks.RunAsync(schema, machines, cachedMain, users, machine, cachedCards, cachedPeople);

    var healthyAt = at.AddSeconds(10);
    Check(Enter("BENCH_HEALTHY", healthyAt).Success, "Check-in after selector echo succeeds");
    using (var db = new TestDb(connection, schema.Model))
    {
        Check(await ScaleProductionAttendancePolicy.GetOnlineErrorAsync(db, "BENCH_HEALTHY", healthyAt.AddSeconds(1),
            16, employee, Current(), CancellationToken.None) == null,
            "Immediate weight after committed check-in with same context is accepted on first attempt");
    }
    // Hold an uncommitted Vao. Weight validation must wait for the SAME card lock, then see the commit.
    var racingAt = at.AddSeconds(20);
    using (var checkInDb = new TestDb(connection, schema.Model))
    using (var tx = checkInDb.Database.BeginTransaction())
    {
        ScaleAttendanceTransferService.LockCard(checkInDb, "BENCH_RACE");
        var rootId = machine.Id + "." + racingAt.ToString("yyyyMMddHHmmss");
        checkInDb.Add(new CheckInOut { Id = rootId, MaChamCong = "BENCH_RACE", MaSoMay = int.Parse(machine.Id),
            TenMay = "BENCH", ThoiGian = racingAt, TrangThai = "Vào", MaXuong = "1" });
        checkInDb.Add(new ScaleAttendanceScan { Id = rootId, VaoId = rootId, MaSoMay = int.Parse(machine.Id),
            ThoiGian = racingAt, TrangThai = "Vào", MaThanhPham = Current().Product,
            MaLoaiNguyenLieu = Current().Material, MaXuong = "1" });
        checkInDb.SaveChanges();
        var begun = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var validating = Task.Run(async () =>
        {
            using var weightDb = new TestDb(connection, schema.Model);
            await using var weightTx = await weightDb.Database.BeginTransactionAsync();
            begun.SetResult();
            await ScaleAttendanceTransferService.LockCardAsync(weightDb, "BENCH_RACE", CancellationToken.None);
            return await ScaleProductionAttendancePolicy.GetOnlineErrorAsync(weightDb, "BENCH_RACE", racingAt.AddSeconds(1),
                16, employee, Current(), CancellationToken.None);
        });
        await begun.Task;
        await Task.Delay(150); // test-only delay to prove lock waiting; never production
        Check(!validating.IsCompleted, "Weight waits while check-in owns card transaction lock");
        tx.Commit();
        Check(await validating == null, "After Vao commit, first waiting weight sees the new shift; no retry needed");
    }
    await EmployeeCardCatalogSqlChecks.Run(schema, connection, Check);
    await HubEmployeeManagerChecks.Run(schema, connection, Check);
    Console.WriteLine($"PASS: {checks} selector/attendance/startup regression assertions.");
}
finally
{
    AppViewModels.Base.Ins.ConnectionString2 = previousEfConnection;
    await schema.Database.EnsureDeletedAsync();
    Console.WriteLine("CLEANUP: GUID LocalDB fixture deleted; factory database unchanged.");
}

static T Mock<T>(Func<MethodInfo, object?[]?, object?> handler) where T : class
{ var p = DispatchProxy.Create<T, ServiceProxy>(); ((ServiceProxy)(object)p).Handler = handler; return p; }
public class ServiceProxy : DispatchProxy
{ public Func<MethodInfo, object?[]?, object?> Handler = null!; protected override object? Invoke(MethodInfo? m, object?[]? a) => Handler(m!, a); }
sealed class SchemaDb(string connection) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder b) => b.UseSqlServer(connection);
    protected override void OnModelCreating(ModelBuilder m)
    { m.Entity<CheckInOut>(); m.Entity<ScaleAttendanceScan>(); m.Entity<HQ_PhieuCan>(); m.Entity<HQ_ThanhPham>(); m.Entity<HQ_LoaiNguyenLieu>();
      m.Entity<MayCan>();m.Entity<TheTu>();m.Entity<NhanVienDaiThanh>();m.Entity<LichSuXuongMayCan>();
      m.Entity<WorkshopAttendance>();
      m.Entity<XiNghiep>(); m.Entity<HQ_NhanVien_U>(); m.Entity<HQ_NhanVien_D>(); m.Entity<HQ_TheTu_D>();
      m.Entity<NguoiDung>().Ignore(x=>x.RefreshToken).Ignore(x=>x.UserRole).Property(x=>x.Id).ValueGeneratedNever(); }
}
sealed class TestDb(string connection, IModel model) : dbPMScontext(new DbContextOptionsBuilder<dbPMScontext>()
    .UseSqlServer(connection).UseModel(model).Options)
{ protected override void OnConfiguring(DbContextOptionsBuilder b) { } }
sealed class TestCommit(IMayCansService machines,IMainService main,string connection,IModel model)
    :CommitService(machines,null!,main,Options.Create(new ScaleAttendanceOptions()))
{protected override dbPMScontext CreateHqOnlineDb()=>new TestDb(connection,model);}
sealed class TestContext(string connection):HubCallerContext
{
    private readonly CancellationTokenSource aborted=new();
    public bool AbortCalled {get;private set;}
    public override string ConnectionId=>connection;
    public override string? UserIdentifier=>null;
    public override ClaimsPrincipal? User=>null;
    public override IDictionary<object,object?> Items {get;}=new Dictionary<object,object?>();
    public override IFeatureCollection Features {get;}=new FeatureCollection();
    public override CancellationToken ConnectionAborted=>aborted.Token;
    public override void Abort() {AbortCalled=true;aborted.Cancel();}
}
