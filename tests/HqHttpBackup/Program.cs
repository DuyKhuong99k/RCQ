using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using MvvmHelpers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Models.Repos;
using Models.Repos.Models;
using Newtonsoft.Json.Linq;
using PMSHub.Controllers;
using Services;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;

var checks = 0;
void Check(bool ok, string name)
{
    if (!ok) throw new Exception("FAIL: " + name);
    checks++;
    Console.WriteLine("PASS: " + name);
}
JObject Payload(string time = "20261022154059") => JObject.Parse($$"""
    {"Id":"7779520.{{time}}","STT":0,"NgayGio":"{{time}}","MayCan":"7779520","MaLo":"L01",
     "MaSize":"20X20","MaThanhPham":"LVT2","MaNhanVien":"0030","TrongLuongNhan":"1.400",
     "TrongLuongTra":"1.400","TrongLuongTare":"0.000","TheId":"0003162457","TheChucNang":"",
     "Ngay":"20261022","MaLoaiNguyenLieu":"DuaLUOI","MaNhaCungCap":"","MaKhachHangTP2":"",
     "LoaiGiaoDich":"","BienSo":"","TTXe":0}
    """);
HQ_PhieuCan Parse(JObject p) => HqHttpBackupWriter.Parse(p, "7779520");
var sample = Parse(Payload());
Check(sample.Id == "7779520.20261022154059" && sample.MaNhanVien == "0030" && sample.TheId == "0003162457" &&
    sample.NgayGio == new DateTime(2026,10,22,15,40,59) && sample.TrongLuong == 1.4m && sample.MaThanhPham == "LVT2",
    "Đúng payload firmware thiếu Stated: giữ nguyên Id/giờ/thẻ/mã/ký");
Check(sample.Ngay == new DateOnly(2026,10,22) && sample.MaXuong == "", "Không gán xưởng hiện tại lúc parse");
var legacyId = Payload(); legacyId["Id"] = "7779520.20000101000100";
Check(Parse(legacyId).Id == (string)legacyId["Id"]!, "Không đổi Id lịch sử do firmware từng sai đồng hồ");
var numeric = Payload(); numeric["TrongLuongNhan"] = 1.4m; numeric["TrongLuongTare"] = .71m;
Check(Parse(numeric).TrongLuong == 1.4m && Parse(numeric).TrongLuongTare == .71m, "Nhận số hoặc chuỗi số bất biến văn hóa");
numeric["TrongLuongNhan"] = .5m;
Check(Parse(numeric).TrongLuong == .5m, "Kg đã là net: không trừ bì hay yêu cầu net >= bì");
var originalCulture=System.Globalization.CultureInfo.CurrentCulture;
try
{
    System.Globalization.CultureInfo.CurrentCulture=System.Globalization.CultureInfo.GetCultureInfo("vi-VN");
    Check(Parse(numeric).TrongLuong==.5m && Parse(Payload()).TrongLuong==1.4m,
        "Số/chuỗi kg không phụ thuộc server dùng dấu phẩy vi-VN");
}
finally { System.Globalization.CultureInfo.CurrentCulture=originalCulture; }
var explicitSt = Payload(); explicitSt["Stated"] = "ST";
Check(Parse(explicitSt).TrongLuong == 1.4m, "HTTP vẫn nhận ST nếu được gửi rõ");
var onlineMissing = new DataLite { Id=sample.Id,NgayGio="20261022154059",TheId=sample.TheId,TrongLuong=1.4m };
Check(HqOnlineWeightWriter.RequestError(onlineMissing) != null, "Online vẫn từ chối thiếu ST");

var invalid = new List<JObject>();
foreach (var state in new JToken[] { "US", "", "st", "UNKNOWN", JValue.CreateNull(), new JArray("ST") })
{
    var p = Payload(); p["Stated"] = state; invalid.Add(p);
}
foreach (var (field, value) in new (string,JToken)[]
{
    ("TrongLuongNhan",0m),("TrongLuongNhan",-1m),("TrongLuongNhan","abc"),("TrongLuongNhan","1,400"),
    ("TrongLuongNhan",1.4001m),("TrongLuongNhan",1000000000000000m),("TrongLuongNhan",new JArray(1)),
    ("TrongLuongTare",-.1m),("TrongLuongTare","bad"),("TrongLuongTare",.0001m),
    ("NgayGio","bad"),("NgayGio","20991022154059"),("MayCan","OTHER"),("Id",new string('X',101)),
    ("Id","7779460.20261022154059"),
    ("STT","0.1"),("MaNhanVien",""),("MaThanhPham",new JObject()),("MaLo",new string('L',51))
}) { var p=Payload(); p[field]=value; invalid.Add(p); }
foreach (var field in new[] { "Id","NgayGio","TheId","MaLo","MaSize","MaThanhPham","MaLoaiNguyenLieu","MaNhanVien","TrongLuongNhan" })
{ var p=Payload(); p.Remove(field); invalid.Add(p); }
foreach (var p in invalid)
{
    var rejected=false;
    try { Parse(p); } catch (ArgumentException) { rejected=true; }
    Check(rejected,"Từ chối payload không hợp lệ trước SQL: " + invalid.IndexOf(p));
}

if (args.Contains("--sql-localdb"))
{
    var database="RCQ_HttpBackupTest_"+Guid.NewGuid().ToString("N");
    var connection=@"Server=(localdb)\MSSQLLocalDB;Database="+database+";Integrated Security=True;TrustServerCertificate=True";
    using var schema=new SchemaDb(connection);
    try
    {
        schema.Database.EnsureCreated();
        schema.Add(new LichSuXuongMayCan {MayCan="7779520",MaXuong="1",TuThoiGian=new DateTime(2026,10,1)});
        schema.SaveChanges();
        var machine=new MayCan {Id="7779520",DisplayName="TEST",WKv=AppKV.Hq,MaXuong="99"};
        var uiAdds=0; var uiFails=false; var registered=true; var opened=0;
        var machines=Proxy<IMayCansService>((m,a)=>m.Name switch
        {
            "Find" => registered && (string?)a![0]==machine.Id ? machine : null,
            "AddItem" => Add(), "NotifyItemChanged" => Notify(), "CommandSetNhanVien" => Task.FromResult(true),
            _ => throw new Exception("Unexpected service: "+m.Name)
        });
        object? Add() { Interlocked.Increment(ref uiAdds); if(uiFails) throw new Exception("TEST_UI_AFTER_COMMIT"); return null; }
        object? Notify() { if(uiFails) throw new Exception("TEST_UI_AFTER_COMMIT"); return null; }
        var main=Proxy<IMainService>((_,_)=>throw new Exception("Không được DAO/query trên kết nối riêng"));
        TestController Controller()=>new(machines,main,()=> {Interlocked.Increment(ref opened);return new TestDb(connection,schema.Model);});
        IActionResult Send(JObject p,bool silent=false)=>silent ? Controller().PostSilent(p) : Controller().Post(p);
        int Status(IActionResult r)=>((ObjectResult)r).StatusCode??200;
        JObject Body(IActionResult r)=>JObject.FromObject(((ObjectResult)r).Value!);
        foreach(var p in invalid)
        {
            var before=opened;
            Check(Status(Send(p))==400 && Status(Send(p,true))==400 && opened==before,
                "Hai action trả 400 trước SQL cho dữ liệu sai: "+invalid.IndexOf(p));
        }
        registered=false;
        Check(Status(Send(Payload()))==400 && Status(Send(Payload(),true))==400 && opened==0,
            "Máy chưa đăng ký không được HTTP 200 giả");
        registered=true;
        Check(Status(Controller().Post("{"))==400 && Status(Controller().PostSilent("{"))==400,
            "JSON hỏng không được HTTP 200");

        var first=Send(Payload());
        Check(Status(first)==200 && (bool?)Body(first)["duplicated"]==false,"POST HTTP 200 sau COMMIT phiếu thiếu ST");
        var saved=schema.Set<HQ_PhieuCan>().AsNoTracking().Single();
        Check(saved.Id==sample.Id && saved.MaXuong=="1" && saved.MaNhanVien=="0030" && saved.TrongLuong==1.4m,
            "Kết nối khác thấy phiếu đúng sau HTTP 200; xưởng lịch sử 1, không lấy hiện tại 99");
        Check(uiAdds==1,"UI chỉ thêm một lần cho phiếu mới");
        foreach(var silent in new[]{false,true})
        {
            var replay=Send(Payload(),silent);
            Check(Status(replay)==200 && (bool?)Body(replay)["duplicated"]==true && schema.Set<HQ_PhieuCan>().Count()==1 && uiAdds==1,
                "Gửi lại cùng Id xác nhận, không INSERT/UI cộng lại: "+silent);
        }
        foreach(var (field,value) in new (string,JToken)[]
        {
            ("STT",1),("TheId","OTHER"),("MaNhanVien","OTHER"),("TrongLuongNhan",1.41m),
            ("TrongLuongTare",.1m),("NgayGio","20261022154100"),("MaLo","OTHER"),
            ("MaSize","OTHER"),("MaThanhPham","OTHER"),("MaLoaiNguyenLieu","OTHER"),("MaXuong","2")
        })
        {
            var p=Payload();p[field]=value;
            Check(Status(Send(p))==400 && schema.Set<HQ_PhieuCan>().Count()==1,"Cùng Id khác nội dung không được ACK/ghi đè: "+field);
        }
        var second=Send(Payload("20261022154146"));
        Check(Status(second)==200 && schema.Set<HQ_PhieuCan>().Count()==2,"Hai phiếu trong ảnh cách 47s đều lưu được");
        Check(Status(Send(Payload("20261022154150")))==400 && schema.Set<HQ_PhieuCan>().Count()==2,
            "Giữ chính sách offline: khác Id nhưng cùng payload dưới 30s bị chặn");

        var races=await Task.WhenAll(Enumerable.Range(0,8).Select(_=>Task.Run(()=>Send(Payload("20261022154300")))));
        Check(races.All(x=>Status(x)==200) && races.Count(x=>(bool?)Body(x)["duplicated"]==false)==1,
            "Tám HTTP request cùng Id đồng thời: một INSERT, mọi retry hợp lệ HTTP 200");
        Check(schema.Set<HQ_PhieuCan>().Count(x=>x.Id=="7779520.20261022154300")==1,"SQL chỉ một dòng sau đua HTTP");
        var closeIds=await Task.WhenAll(new[]{"20261022154400","20261022154401"}.Select(t=>Task.Run(()=>Send(Payload(t)))));
        Check(closeIds.Count(x=>Status(x)==200)==1 && closeIds.Count(x=>Status(x)==400)==1,
            "Khóa thẻ giữ luật offline chống trùng xuyên hai Id đồng thời");

        uiFails=true;
        Check(Status(Send(Payload("20261022154500")))==200 && schema.Set<HQ_PhieuCan>().Any(x=>x.Id=="7779520.20261022154500"),
            "Lỗi AddItem sau COMMIT không biến phiếu đã lưu thành HTTP 400");
        Check(Status(Send(Payload("20261022154600"),true))==200,"Lỗi Notify sau COMMIT không làm PostSilent báo thất bại");
        uiFails=false;
        schema.Database.ExecuteSqlRaw("ALTER TABLE dbo.HQ_PhieuCan ADD CONSTRAINT CK_HttpBackupTest CHECK (STT <> -999)");
        var sqlFail=Payload("20261022154700");sqlFail["STT"]=-999;
        var failure=Send(sqlFail);
        Check(Status(failure)==503 && (string?)Body(failure)["SaveStatus"]==HqOnlineResponse.Unknown &&
            !schema.Set<HQ_PhieuCan>().Any(x=>x.Id==(string)sqlFail["Id"]!),"SQL lỗi rollback, không ACK hoặc giả đã lưu");
        Check(!Body(failure).ToString().Contains("CK_HttpBackupTest"),"Không lộ chi tiết lỗi SQL trong response");
        sqlFail["STT"]=0;
        Check(Status(Send(sqlFail))==200,"Retry Id gốc sau lỗi ghi không bị kẹt khóa");
        var noHistory=Payload("20260930100000");
        Check(Status(Send(noHistory))==400 && !schema.Set<HQ_PhieuCan>().Any(x=>x.Id==(string)noHistory["Id"]!),
            "Không đoán xưởng hiện tại cho phiếu trước mốc lịch sử");

        var cross=Payload("20261022154800");
        var live=new DataLite {Id=(string)cross["Id"]!,NgayGio=(string)cross["NgayGio"]!,TheId=sample.TheId,
            TrongLuong=1.4m,TrongLuongTare=0,Stated="ST"};
        var onlineTask=Task.Run(async()=>
        {
            using var db=new TestDb(connection,schema.Model);
            return await HqOnlineWeightWriter.SaveAsync(db,live,"7779520",_=>
                {var item=Parse(cross);item.MaXuong="1";return Task.FromResult(new HqOnlineWeightWriter.Result(item,false,null));},(_,_)=>Task.FromResult<string?>(null));
        });
        var httpTask=Task.Run(()=>Send((JObject)cross.DeepClone()));
        var online=await onlineTask;var http=await httpTask;
        Check(online.Error==null && Status(http)==200 && schema.Set<HQ_PhieuCan>().Count(x=>x.Id==live.Id)==1,
            "HTTP và online cùng Id dùng chung khóa SQL, chỉ một phiếu");

        var cardRaces=await Task.WhenAll(new[]{sample.TheId,"OTHER_CARD"}.Select(card=>Task.Run(()=>
        {
            var p=Payload("20261022154900");p["TheId"]=card;return Send(p);
        })));
        Check(cardRaces.Count(x=>Status(x)==200)==1 && cardRaces.Count(x=>Status(x)==400)==1 &&
            schema.Set<HQ_PhieuCan>().Count(x=>x.Id=="7779520.20261022154900")==1,
            "Hai thẻ tranh cùng Id: một phiếu, bên xung đột không được HTTP 200");

        object Cache(string property,object items)
        {
            var type=typeof(IMainService).GetProperty(property)!.PropertyType;
            var vm=RuntimeHelpers.GetUninitializedObject(type);
            type.GetField("items",BindingFlags.NonPublic|BindingFlags.Instance)!.SetValue(vm,items);
            return vm;
        }
        var cardsCache=Cache("VmThe",new ObservableRangeCollection<TheTu>{new(){MaTheTu="ROUTE_CARD",MaNhanVien="ROUTE_GROUP"}});
        var peopleCache=Cache("VmNhanVien",new ObservableRangeCollection<NhanVienDaiThanh>{new(){MaNhanVien="ROUTE_GROUP",IsNhom=true,IsHuman=true,Name="Nhóm test",MaHoSo="ROUTE_GROUP"}});
        var allowCache=true;
        var cachedMain=Proxy<IMainService>((method,_)=>allowCache ? method.Name switch
        {
            "get_VmThe"=>cardsCache,"get_VmNhanVien"=>peopleCache,
            _=>throw new Exception("Không được lookup DAO hoặc tạo VM: "+method.Name)
        }:throw new Exception("Replay không được cần cache"));
        var routeDbOpens=0;
        var route=new TestCommit(machines,cachedMain,()=>{routeDbOpens++;return new TestDb(connection,schema.Model);});
        machine.MaLo="ROUTE_LO";machine.MaSize="20X20";machine.MaThanhPham="PERF";machine.MaLoaiNguyenLieu="DuaLUOI";
        // Online groups now need the same current-day shift/snapshot as employees.
        // Historical HTTP samples above stay unchanged and do not acquire this requirement.
        var routeTime=ScaleWorkshopHistory.BusinessNow().AddMinutes(-2);
        routeTime=routeTime.AddTicks(-(routeTime.Ticks % TimeSpan.TicksPerSecond));
        var checkInTime=routeTime.AddSeconds(-1);
        var rootId=machine.Id+"."+checkInTime.ToString("yyyyMMddHHmmss");
        schema.Add(new CheckInOut {Id=rootId,MaChamCong="ROUTE_CARD",ThoiGian=checkInTime,
            MaSoMay=int.Parse(machine.Id),TenMay="TEST",TrangThai="Vào",MaXuong="1"});
        schema.Add(new ScaleAttendanceScan {Id=rootId,VaoId=rootId,MaSoMay=int.Parse(machine.Id),ThoiGian=checkInTime,
            TrangThai="Vào",MaThanhPham="PERF",MaLoaiNguyenLieu="DuaLUOI",MaXuong="1"});
        schema.SaveChanges();
        var routeRequest=new DataLite {Id=machine.Id+"."+routeTime.ToString("yyyyMMddHHmmss"),
            NgayGio=routeTime.ToString("yyyyMMddHHmmss"),TheId="ROUTE_CARD",TrongLuong=1.4m,Stated="ST"};
        var routeReply=await route.CommitLite(JsonConvert.SerializeObject(routeRequest),new ClientInfo{Id=machine.Id},"LITE");
        var routeData=JsonConvert.DeserializeObject<DataLiteRes>(routeReply.Item2)!;
        Check(routeReply.Item1 && routeData.TongSoRoCan==1 && routeData.TongTrongLuongCan==1.4m && routeDbOpens==2,
            "CommitLite thật: async INSERT + tổng đúng, hai context ghi/đọc, không DAO lookup thừa");
        await HqOnlineUi.WhenIdleAsync(machine).WaitAsync(TimeSpan.FromSeconds(3));
        var addsBeforeReplay=uiAdds;
        allowCache=false;machine.MaLo=null;machine.MaSize=null;machine.MaThanhPham=null;machine.MaLoaiNguyenLieu=null;
        var replayReply=await route.CommitLite(JsonConvert.SerializeObject(routeRequest),new ClientInfo{Id=machine.Id},"LITE");
        await HqOnlineUi.WhenIdleAsync(machine).WaitAsync(TimeSpan.FromSeconds(3));
        Check(replayReply.Item1 && JsonConvert.DeserializeObject<DataLiteRes>(replayReply.Item2)!.TongSoRoCan==1 && uiAdds==addsBeforeReplay,
            "CommitLite retry sau mất cấu hình/cache vẫn xác nhận tổng đúng, không thêm UI/phiếu");

        schema.Database.ExecuteSqlRaw("DELETE dbo.LichSuXuongMayCan");
        Check(Status(Send(Payload()))==200,"Retry đã lưu không phụ thuộc cấu hình/lịch sử hiện tại bị thay đổi");
        Check(Status(Send(Payload("20261022155000")))==400,"Phiếu mới vẫn cần xưởng lịch sử, không bỏ kiểm tra theo retry");
    }
    finally { schema.Database.EnsureDeleted(); }
}
Console.WriteLine($"PASS: {checks} HTTP backup checks; "+(args.Contains("--sql-localdb")?"SQL LocalDB riêng, không SQL xưởng.":"parse-only, không SQL."));

static T Proxy<T>(Func<MethodInfo,object?[]?,object?> handler) where T:class
{
    var p=DispatchProxy.Create<T,MockService>();((MockService)(object)p).Handler=handler;return p;
}
public class MockService:DispatchProxy
{
    public Func<MethodInfo,object?[]?,object?> Handler=null!;
    protected override object? Invoke(MethodInfo? m,object?[]? a)=>Handler(m!,a);
}
sealed class TestController(IMayCansService machines,IMainService main,Func<dbPMScontext> factory):DataController(machines,main)
{
    protected override dbPMScontext CreateHqBackupDb()=>factory();
}
sealed class SchemaDb(string connection):DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder b)=>b.UseSqlServer(connection);
    protected override void OnModelCreating(ModelBuilder m) {m.Entity<HQ_PhieuCan>();m.Entity<LichSuXuongMayCan>();m.Entity<CheckInOut>();m.Entity<ScaleAttendanceScan>();}
}
sealed class TestCommit(IMayCansService machines,IMainService main,Func<dbPMScontext> factory)
    :CommitService(machines,null!,main,Options.Create(new ScaleAttendanceOptions()))
{
    protected override dbPMScontext CreateHqOnlineDb()=>factory();
}
sealed class TestDb(string connection,IModel model):dbPMScontext(
    new DbContextOptionsBuilder<dbPMScontext>().UseSqlServer(connection).UseModel(model).Options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder b) { }
}
