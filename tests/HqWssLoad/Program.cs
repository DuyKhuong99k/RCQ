using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Models.Repos;
using Models.Repos.Models;
using MvvmHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;

if (!args.Contains("--sql-localdb"))
    throw new InvalidOperationException("Explicit --sql-localdb required; never read production configuration.");
const int machinesCount = 30;
const int seedRows = 100000;
ThreadPool.SetMinThreads(100, 100); // match current PMSHub Program, not a new tuning setting
var database = "RCQ_WssLoadTest_" + Guid.NewGuid().ToString("N");
var connection = @"Server=(localdb)\MSSQLLocalDB;Database=" + database + ";Integrated Security=True;TrustServerCertificate=True";
using var schema = new SchemaDb(connection);
WebApplication? app = null;
var peers = new List<Peer>();
var summaries = new List<Burst>();
var now = ScaleWorkshopHistory.BusinessNow();
var start = now.AddMinutes(-10);
var ticketBase = now.AddMinutes(-3);
var usersMap = new ConcurrentDictionary<string, ClientInfo>();
var machinesMap = new ConcurrentDictionary<string, MayCan>();
var cards = new ObservableRangeCollection<TheTu>();
var people = new ObservableRangeCollection<NhanVienDaiThanh>();
var ownCounts = new int[machinesCount];
var expectedContext = 0;
var roundIndex = 0;

try
{
    await schema.Database.EnsureCreatedAsync();
    for (var i=0;i<machinesCount;i++)
    {
        var machine=(90000000+i).ToString(CultureInfo.InvariantCulture);
        var card="100000"+i.ToString("D4");var employee="BENCH_NV_"+i.ToString("D2");
        machinesMap[machine]=new MayCan {Id=machine,DisplayName="BENCH",WKv=AppKV.Hq,MType="BOARD",
            MaXuong="1",MaLo="BENCH_LO",MaSize="20X20",MaThanhPham="LM",MaLoaiNguyenLieu="XOAIDONG"};
        cards.Add(new TheTu{MaTheTu=card,MaNhanVien=employee});
        people.Add(new NhanVienDaiThanh{MaNhanVien=employee,MaHoSo=employee,Name="BENCH "+i,IsHuman=true,IsNhom=false});
        var rootId=machine+"."+start.ToString("yyyyMMddHHmmss");
        schema.Add(new CheckInOut {Id=rootId,MaChamCong=card,ThoiGian=start,MaSoMay=int.Parse(machine),
            TenMay="BENCH",TrangThai="Vào",CaDem=true,Isforget=0,MaXuong="1"});
        schema.Add(new ScaleAttendanceScan {Id=rootId,VaoId=rootId,MaSoMay=int.Parse(machine),ThoiGian=start,
            TrangThai="Vào",MaThanhPham="LM",MaLoaiNguyenLieu="XOAIDONG",MaXuong="1"});
        schema.Add(new LichSuXuongMayCan {MayCan=machine,MaXuong="1",TuThoiGian=start.AddDays(-1)});
    }
    await schema.SaveChangesAsync();
    // Only run the already-tested migration inside this GUID fixture; never against PMS_HQ.
    var rootPath=Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,"../../../../../"));
    var migration=File.ReadAllText(Path.Combine(rootPath,"docs/sql/20261018-HqOnlineWorkContext.sql"));
    if (!database.StartsWith("RCQ_WssLoadTest_") || !migration.Contains("USE [PMS_HQ];")) throw new Exception("Fixture guard");
    migration=migration.Replace("USE [PMS_HQ];",$"USE [{database}];");
    foreach(var batch in Regex.Split(migration,@"(?im)^\s*GO\s*$"))
        if(!string.IsNullOrWhiteSpace(batch)) await schema.Database.ExecuteSqlRawAsync(batch);
    // Match the factory's RCSI. Clear only this process's pool for this exact test database.
    using(var poolKey=new SqlConnection(connection)) SqlConnection.ClearPool(poolKey);
    await using(var master=new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=master;Integrated Security=True;TrustServerCertificate=True"))
    {
        await master.OpenAsync();
        await using var command=master.CreateCommand();
        command.CommandText=$"ALTER DATABASE [{database}] SET READ_COMMITTED_SNAPSHOT ON";
        command.CommandTimeout=15;
        await command.ExecuteNonQueryAsync();
    }
    await using(var info=new SqlConnection(connection))
    {
        await info.OpenAsync();
        await using var command=info.CreateCommand();
        command.CommandText="SELECT CONVERT(nvarchar(128),SERVERPROPERTY('ProductVersion')), CONVERT(nvarchar(128),SERVERPROPERTY('Edition')), is_read_committed_snapshot_on FROM sys.databases WHERE database_id=DB_ID()";
        await using var reader=await command.ExecuteReaderAsync();await reader.ReadAsync();
        Console.WriteLine($"ENV SQL={reader.GetString(0)}; Edition={reader.GetString(1)}; RCSI={reader.GetBoolean(2)}; LogicalCPUs={Environment.ProcessorCount}; Framework={System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
    }

    object Cache(string property,object items)
    {
        var type=typeof(IMainService).GetProperty(property)!.PropertyType;
        var vm=RuntimeHelpers.GetUninitializedObject(type);
        type.GetField("items",BindingFlags.NonPublic|BindingFlags.Instance)!.SetValue(vm,items);
        return vm;
    }
    var cardsCache=Cache("VmThe",cards);var peopleCache=Cache("VmNhanVien",people);
    var main=Mock<IMainService>((m,_)=>m.Name switch
    {"get_VmThe"=>cardsCache,"get_VmNhanVien"=>peopleCache,_=>throw new Exception("Unexpected main/DAO access: "+m.Name)});
    var users=Mock<IUserService>((m,a)=>m.Name switch
    {
        "get_Clients"=>usersMap,
        "GetByConnectedId"=>usersMap.TryGetValue((string)a![0]!,out var c)?c:null,
        "RemoveByConnectedId"=>RemoveUser((string)a![0]!),
        _=>throw new Exception("Unexpected user method: "+m.Name)
    });
    object? RemoveUser(string id) {usersMap.TryRemove(id,out _);return null;}
    var machines=Mock<IMayCansService>((m,a)=>m.Name switch
    {
        "Find"=>machinesMap.TryGetValue((string)a![0]!,out var machine)?machine:null,
        "AddItem"=>AddUi((MayCan)a![0]!,a[1]!),
        "NotifyItemChanged"=>null,
        "DisConnectionId"=>null,
        "CommandSetNhanVien"=>SetIdentity(a!),
        _=>throw new Exception("Unexpected machine method: "+m.Name)
    });
    object? AddUi(MayCan machine,object item)
    { lock(machine.Items){if(machine.Items.Count>=100)machine.Items.RemoveAt(machine.Items.Count-1);machine.Items.Insert(0,item);}return null; }
    async Task<bool> SetIdentity(object?[] a)
    {
        var machine=machinesMap[(string)a[0]!];
        await app!.Services.GetRequiredService<IHubContext<ChatHub>>().Clients.Client(machine.ConnectionId!)
            .SendAsync("SETNHANVIEN",(string)a[1]!, (string)a[2]!, (string)a[3]!);
        return true;
    }
    var mode=new AttendanceModeStore(Path.Combine(Path.GetTempPath(),"RCQ_WssMode_"+Guid.NewGuid().ToString("N")+".json"));
    var commit=new LoadCommit(machines,main,connection,schema.Model,mode);
    var router=new CommunicationService(users,commit,Options.Create(new ScaleAttendanceOptions()),machines,attendanceMode:mode);
    var builder=WebApplication.CreateBuilder(new WebApplicationOptions {Args=Array.Empty<string>(),EnvironmentName="Production"});
    builder.Logging.ClearProviders();
    builder.WebHost.UseUrls("http://127.0.0.1:0"); // loopback only; no factory/public listening address
    builder.Services.AddSignalR(o=>{o.KeepAliveInterval=TimeSpan.FromSeconds(15);o.ClientTimeoutInterval=TimeSpan.FromSeconds(30);});
    builder.Services.AddSingleton(users);builder.Services.AddSingleton(machines);
    builder.Services.AddSingleton<ICommitService>(commit);builder.Services.AddSingleton<ICommunicationService>(router);
    builder.Services.AddSingleton(Mock<IConverterService>((m,_)=>throw new Exception("Unexpected converter "+m.Name)));
    app=builder.Build();app.MapHub<ChatHub>("/bench");await app.StartAsync();
    var url=app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single()+"/bench";
    for(var i=0;i<machinesCount;i++)
    {
        var peer=new Peer(i,(90000000+i).ToString(),"100000"+i.ToString("D4"),
            new HubConnectionBuilder().WithUrl(url,o=>o.Transports=HttpTransportType.WebSockets).Build());
        peer.Connection.On<string,string>("ReceiveData",(cmd,json)=>peer.Receive(cmd,json));
        peer.Connection.On<string,string,string>("SETNHANVIEN",(_,_,_)=>Interlocked.Increment(ref peer.Identities));
        peers.Add(peer);
    }
    await Task.WhenAll(peers.Select(async peer=>
    {
        await peer.Connection.StartAsync();
        var connectionId=peer.Connection.ConnectionId??throw new Exception("Negotiated connection Id missing");
        machinesMap[peer.Machine].ConnectionId=connectionId;
        usersMap[connectionId]=new ClientInfo{Id=peer.Machine,IsMayCan=true,IsActive=true,WKv=AppKV.Hq};
    }));
    Console.WriteLine("READY: 30 real SignalR WebSocket connections; authentication pre-registered; GUI/background jobs/TLS/LAN not included.");

    async Task RunBurst(string label)
    {
        var time=ticketBase.AddSeconds(2*roundIndex++);
        var before=expectedContext;
        var release=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        long releasedAt=0;
        var jobs=peers.Select(async peer=>
        {
            await release.Task;
            var sentAt=Stopwatch.GetTimestamp();
            var dispatchMs=Stopwatch.GetElapsedTime(releasedAt,sentAt).TotalMilliseconds;
            var id=peer.Machine+"."+time.ToString("yyyyMMddHHmmss");
            var pending=new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            peer.RequiredIdentities=Volatile.Read(ref peer.Identities)+1;
            peer.Pending=pending;
            try
            {
                var payload=JsonConvert.SerializeObject(new DataLite {Id=id,NgayGio=time.ToString("yyyyMMddHHmmss"),TheId=peer.Card,
                    Stated="ST",TrongLuong=1.4m,TrongLuongTare=.71m});
                await peer.Connection.SendAsync("SendData","LITE",payload.Length,payload);
                var json=await pending.Task.WaitAsync(TimeSpan.FromSeconds(60));
                var elapsed=Stopwatch.GetElapsedTime(sentAt).TotalMilliseconds;
                var outer=JObject.Parse(json);
                if((bool?)outer["IsError"]!=false) return new Sample(peer.Index,id,elapsed,dispatchMs,(string?)outer["DataError"]??"Missing success flag");
                var data=JsonConvert.DeserializeObject<DataLiteRes>((string)outer["Data"]!)!;
                var expectedOwn=ownCounts[peer.Index]+1;
                var valid=data.Id==id && data.IsDataAction==1 && data.SaveStatus==HqOnlineResponse.Saved &&
                    data.TongSoRo==expectedOwn && data.TongTrongLuong==1.4m*expectedOwn &&
                    data.TongSoRoCan>=before+1 && data.TongSoRoCan<=before+machinesCount &&
                    data.TongTrongLuongCan==data.TongSoRoCan*1.4m;
                return new Sample(peer.Index,id,elapsed,dispatchMs,valid?null:"Invalid Id/full totals");
            }
            catch(Exception ex) {return new Sample(peer.Index,id,Stopwatch.GetElapsedTime(sentAt).TotalMilliseconds,dispatchMs,ex.Message);}
            finally {peer.Pending=null;}
        }).ToArray();
        releasedAt=Stopwatch.GetTimestamp();release.SetResult();
        var samples=await Task.WhenAll(jobs);
        var batchMs=Stopwatch.GetElapsedTime(releasedAt).TotalMilliseconds;
        var ids=samples.Select(x=>x.Id).ToArray();
        var inserted=await schema.Set<HQ_PhieuCan>().AsNoTracking().CountAsync(x=>ids.Contains(x.Id));
        var sorted=samples.Select(x=>x.Milliseconds).Order().ToArray();
        double P(double p)=>sorted[Math.Clamp((int)Math.Ceiling(p*sorted.Length)-1,0,sorted.Length-1)];
        var result=new Burst(label,samples.Count(x=>x.Error==null),inserted,Math.Round(batchMs,2),Math.Round(sorted.Average(),2),
            Math.Round(P(.5),2),Math.Round(P(.95),2),Math.Round(P(.99),2),Math.Round(sorted[0],2),Math.Round(sorted[^1],2),
            Math.Round(samples.Max(x=>x.DispatchMs),2));
        summaries.Add(result);Console.WriteLine("BURST "+JsonConvert.SerializeObject(result));
        foreach(var failure in samples.Where(x=>x.Error!=null))Console.WriteLine("FAILURE "+JsonConvert.SerializeObject(failure));
        if(result.Success!=machinesCount || inserted!=machinesCount)throw new Exception("Burst failed; do not hide failures behind an average.");
        for(var i=0;i<machinesCount;i++)ownCounts[i]++;
        expectedContext+=machinesCount;
    }

    await RunBurst("first_path_30");
    for(var round=1;round<=5;round++)await RunBurst("warm_small_"+round);
    // Synthetic current-context rows: test aggregate cost, not production data or throughput claims.
    await schema.Database.ExecuteSqlInterpolatedAsync($@"
        WITH N AS (SELECT TOP ({seedRows}) ROW_NUMBER() OVER(ORDER BY (SELECT NULL))-1 AS n
                   FROM sys.all_objects a CROSS JOIN sys.all_objects b)
        INSERT dbo.HQ_PhieuCan
        (Id,STT,Ngay,NgayGio,MayCan,MaLo,MaSize,MaThanhPham,MaLoaiNguyenLieu,MaNhanVien,
         MaNhanVienPhucVu,MaNhanVienBanKiem,TrongLuong,TrongLuongTare,ChiSanLuong,Status,TheId,TheIdNhanVien,GhiChu,MaXuong)
        SELECT 'BENCH_SEED.'+CONVERT(varchar(20),n),0,CONVERT(date,{ticketBase}),{ticketBase.AddSeconds(-30)},
            CONVERT(varchar(50),90000000+n%30),'BENCH_LO','20X20','LM','XOAIDONG',
            'BENCH_NV_'+RIGHT('00'+CONVERT(varchar(2),n%30),2),'','',1.400,0.710,1,1,
            '100000'+RIGHT('0000'+CONVERT(varchar(4),n%30),4),
            '100000'+RIGHT('0000'+CONVERT(varchar(4),n%30),4),'','1' FROM N;");
    for(var i=0;i<machinesCount;i++)ownCounts[i]+=seedRows/machinesCount+(i<seedRows%machinesCount?1:0);
    expectedContext+=seedRows;
    Console.WriteLine($"SEEDED: {seedRows} synthetic rows in same day/work context; all indexes from migration enabled.");
    for(var round=1;round<=5;round++)await RunBurst("warm_100k_"+round);
    var actual=await schema.Set<HQ_PhieuCan>().CountAsync();
    await Task.WhenAll(machinesMap.Values.Select(HqOnlineUi.WhenIdleAsync)).WaitAsync(TimeSpan.FromSeconds(5));
    if(actual!=expectedContext)throw new Exception($"Expected {expectedContext} SQL rows; got {actual}");
    Console.WriteLine($"PASS: {summaries.Sum(x=>x.Success)} acknowledgements, {actual-seedRows} new SQL tickets, {peers.Sum(x=>x.Identities)} SETNHANVIEN messages; original seed={seedRows}.");
}
finally
{
    foreach(var peer in peers)await peer.Connection.DisposeAsync();
    if(app!=null)await app.DisposeAsync();
    await schema.Database.EnsureDeletedAsync();
    Console.WriteLine("CLEANUP: dedicated GUID LocalDB fixture deleted; factory database untouched.");
}

static T Mock<T>(Func<MethodInfo,object?[]?,object?> handler) where T:class
{var p=DispatchProxy.Create<T,ServiceProxy>();((ServiceProxy)(object)p).Handler=handler;return p;}
public class ServiceProxy:DispatchProxy
{public Func<MethodInfo,object?[]?,object?> Handler=null!;protected override object? Invoke(MethodInfo? m,object?[]? a)=>Handler(m!,a);}
sealed class SchemaDb(string connection):DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder b)=>b.UseSqlServer(connection);
    protected override void OnModelCreating(ModelBuilder m)
    {m.Entity<HQ_PhieuCan>();m.Entity<CheckInOut>();m.Entity<ScaleAttendanceScan>();m.Entity<LichSuXuongMayCan>();}
}
sealed class TestDb(string connection,IModel model):dbPMScontext(new DbContextOptionsBuilder<dbPMScontext>()
    .UseSqlServer(connection).UseModel(model).Options)
{protected override void OnConfiguring(DbContextOptionsBuilder b) { }}
sealed class LoadCommit(IMayCansService machines,IMainService main,string connection,IModel model,IAttendanceMode mode)
    :CommitService(machines,null!,main,Options.Create(new ScaleAttendanceOptions()),mode)
{protected override dbPMScontext CreateHqOnlineDb()=>new TestDb(connection,model);}
sealed class Peer(int index,string machine,string card,HubConnection connection)
{
    public int Index=index;public string Machine=machine,Card=card;public HubConnection Connection=connection;
    public TaskCompletionSource<string>? Pending;public int Identities,RequiredIdentities;
    public void Receive(string cmd,string json)
    {
        if(cmd!="LITE")return;
        if(Volatile.Read(ref Identities)!=RequiredIdentities)
            Pending?.TrySetException(new Exception("ReceiveData arrived without exactly one preceding SETNHANVIEN"));
        else Pending?.TrySetResult(json);
    }
}
record Sample(int Index,string Id,double Milliseconds,double DispatchMs,string? Error);
record Burst(string Label,int Success,int Inserted,double BatchMs,double AverageMs,double P50Ms,double P95Ms,double P99Ms,double MinMs,double MaxMs,double DispatchMaxMs);
