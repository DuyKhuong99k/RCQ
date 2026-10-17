using Models.Repos.Models;
using Services;
using Vars.Hubs;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ViewModels.Repos.Hubs.IServices;

var checks=0;
void Assert(bool ok,string name) { if(!ok) throw new Exception(name); checks++; }
void Reject(Action action,string name) { try {action();} catch(ArgumentException){checks++;return;} throw new Exception(name); }
Assert(!ScaleProductionAttendancePolicy.IsProductionCard(null),"Unregistered card rejected");
Assert(ScaleProductionAttendancePolicy.RequiresAttendance(null),"Missing identity cannot bypass attendance");
for(var flags=0;flags<16;flags++) {
    var identity=new NhanVienDaiThanh {MaNhanVien="GROUP_TEST",IsHuman=(flags&1)!=0,
        IsNhom=(flags&2)!=0,IsChucNang=(flags&4)!=0,IsPhucVu=(flags&8)!=0};
    var eligible=(identity.IsHuman || identity.IsNhom) && !identity.IsChucNang && !identity.IsPhucVu;
    Assert(ScaleProductionAttendancePolicy.IsProductionCard(identity)==eligible,$"Production identity flags {flags}");
    Assert(ScaleProductionAttendancePolicy.RequiresAttendance(identity)==(!eligible || !identity.IsNhom),
        $"Only valid SQL group bypasses attendance, flags {flags}");
}
var groupCard=new NhanVienDaiThanh {MaNhanVien="MAYCAT2",IsNhom=true,IsHuman=false};
Assert(ScaleProductionAttendancePolicy.IsProductionCard(groupCard)
    && !ScaleProductionAttendancePolicy.RequiresAttendance(groupCard),"MAYCAT2 group weighs without Vao/Ra");
groupCard.IsHuman=true;
Assert(!ScaleProductionAttendancePolicy.RequiresAttendance(groupCard),"Group with IsHuman=true also needs no Vao/Ra");
groupCard.IsNhom=false;
Assert(ScaleProductionAttendancePolicy.RequiresAttendance(groupCard),"Normal human still requires Vao/Ra");
var now=new DateTime(2026,10,12,14,0,0);
DataCoummunication Record(int seconds=0)=>new() {Id="7779520."+new DateTime(2026,10,12,11,0,0).AddSeconds(seconds).ToString("yyyyMMddHHmmss"),
 NgayGio=new DateTime(2026,10,12,11,0,0).AddSeconds(seconds).ToString("yyyyMMddHHmmss"),TheId="0000000001",MayCan="7779520",
 MaLo="L01",MaSize="20x20",MaThanhPham="CD0011",MaNhanVien="TEST_NV",TrongLuong=7.3m,TrongLuongTare=0,Status=0};
var normalized=HqWeightBackupProcessor.Normalize(Record(),"7779520",now);
Assert(normalized.Id=="7779520.20261012110000" && normalized.TheIdNhanVien=="0000000001","Identity/card");
Assert(normalized.Ngay==new DateOnly(2026,10,12) && normalized.NgayGio.Hour==11 && normalized.Status==1,"Original local time/status");
var alias=Record();alias.IdIn=alias.Id;alias.Id=null;
Assert(HqWeightBackupProcessor.Normalize(alias,"7779520",now).Id==normalized.Id,"IdIn alias");
alias=Record();alias.IdOut=alias.Id;alias.Id=null;
Assert(HqWeightBackupProcessor.Normalize(alias,"7779520",now).Id==normalized.Id,"IdOut alias");
var conflict=Record();conflict.IdOut="OTHER";Reject(()=>HqWeightBackupProcessor.Normalize(conflict,"7779520",now),"Conflicting ids");
var wrongMachine=Record();wrongMachine.MayCan="7779521";Reject(()=>HqWeightBackupProcessor.Normalize(wrongMachine,"7779520",now),"Wrong machine");
var wrongTime=Record();wrongTime.NgayGio="20261012110001";Reject(()=>HqWeightBackupProcessor.Normalize(wrongTime,"7779520",now),"Wrong time/id");
var missingCode=Record();missingCode.MaThanhPham="";Reject(()=>HqWeightBackupProcessor.Normalize(missingCode,"7779520",now),"No current config fallback");
var negative=Record();negative.TrongLuong=-1;Reject(()=>HqWeightBackupProcessor.Normalize(negative,"7779520",now),"Negative weight");
var cancelled=Record();cancelled.Status=-1;Reject(()=>HqWeightBackupProcessor.Normalize(cancelled,"7779520",now),"Cancelled ticket");
var notProduction=Record();notProduction.ChiSanLuong=0;Reject(()=>HqWeightBackupProcessor.Normalize(notProduction,"7779520",now),"Never convert non-production ticket");
var saved=HqWeightBackupProcessor.Normalize(Record(),"7779520",now);saved.MaSize="20X20";saved.MaLoaiNguyenLieu="CD001";saved.MaXuong="1";
Assert(HqWeightBackupProcessor.MatchesExisting(saved,normalized),"Legacy replay with missing metadata and case difference");
normalized.TrongLuong=9;Assert(!HqWeightBackupProcessor.MatchesExisting(saved,normalized),"Never overwrite changed weight");
normalized=HqWeightBackupProcessor.Normalize(Record(),"7779520",now);
HqWeightBackupProcessor.ResolveHistoricalMetadata(normalized,new[]{new HqWeightBackupProcessor.HistoricalMetadata("CD001","1")});
Assert(normalized.MaLoaiNguyenLieu=="CD001" && normalized.MaXuong=="1","Unique historical metadata");
Reject(()=>HqWeightBackupProcessor.ResolveHistoricalMetadata(normalized,Array.Empty<HqWeightBackupProcessor.HistoricalMetadata>()),"Missing reference");
Reject(()=>HqWeightBackupProcessor.ResolveHistoricalMetadata(normalized,new[]{new HqWeightBackupProcessor.HistoricalMetadata("CD001","1"),new HqWeightBackupProcessor.HistoricalMetadata("OTHER","2")}),"Ambiguous reference");
var scan=new CheckInOut {Id="7779520.20261012100000",MaChamCong="0000000001",MaSoMay=7779520,ThoiGian=new DateTime(2026,10,12,10,0,0),TrangThai="Vào"};
Assert(HqWeightBackupProcessor.ValidateHistoricalScan(scan,7779520,saved.NgayGio,16)==null,"Historical open at weighing time, irrespective of now/later checkout");
scan.TrangThai="Ra";Assert(HqWeightBackupProcessor.ValidateHistoricalScan(scan,7779520,saved.NgayGio,16)!=null,"Already checked out at weighing");
scan.TrangThai="Vào";scan.Isforget=1;Assert(HqWeightBackupProcessor.ValidateHistoricalScan(scan,7779520,saved.NgayGio,16)!=null,"Error shift");
scan.Isforget=0;Assert(HqWeightBackupProcessor.ValidateHistoricalScan(scan,7779521,saved.NgayGio,16)!=null,"Other machine shift");
Assert(HqWeightBackupProcessor.ValidateHistoricalScan(null,7779520,saved.NgayGio,16)!=null,"No check-in");
var all=Enumerable.Range(0,44).Select(i=>HqWeightBackupProcessor.Normalize(Record(i),"7779520",now)).ToArray();
var store=all.Take(10).ToDictionary(x=>x.Id);
int Replay() {var inserted=0;foreach(var item in all) {if(store.TryGetValue(item.Id,out var old)) Assert(HqWeightBackupProcessor.MatchesExisting(old,item),"Replay match");else {store.Add(item.Id,item);inserted++;}}return inserted;}
Assert(Replay()==34 && store.Count==44,"44 received/10 existed/34 inserted simulation");
Assert(Replay()==0 && store.Count==44,"Second resend is idempotent simulation");
var fakeCommit=DispatchProxy.Create<ICommitService,MockCommit>();
var router=new CommunicationService(null!,fakeCommit,Options.Create(new ScaleAttendanceOptions {MaxShiftHours=16}));
var client=new ClientInfo {Id="7779520",WKv=Vars.AppKV.Hq,IsActive=true,IsMayCan=true};
var json="[{\"Id\":\"7779520.20261012110000\",\"MaLo\":\"Lô thử\"}]";
var routed=JObject.Parse(await router.Route(client,"PHIEUCANADDS",Encoding.UTF8.GetByteCount(json),json));
Assert(routed["IsError"]!.Value<bool>()==false,"Weight backup UTF-8 byte length accepted");
routed=JObject.Parse(await router.Route(client,"PHIEUCANADDS",1,json));
Assert(routed["IsError"]!.Value<bool>() && !string.IsNullOrEmpty(routed["DataError"]?.Value<string>()),"Bad length returns explicit failure");
routed=JObject.Parse(await router.Route(client,"PHIEUCANADDS",1,"["));
Assert(routed["IsError"]!.Value<bool>() && routed["DataError"]!.Value<string>()!.Contains("JSON"),"Malformed backup array returns explicit failure");
var response = new DataLiteRes {
    Id="7779520.20261013103946", NgayGio="20261013103946", Ngay="20261013",
    MayCan="7779520", MaNhanVien="0022", TheId="0005610611", MaNhanVienPhucVu="PV", MaNhanVienBanKiem="BK",
    ChiSanLuong=1, Status=1, TongTLTP=3m, DinhMucTP=4m, DanhGia=true,
    IsDataAction=1, MessStr="ĐÃ HOÀN THÀNH", TrongLuongNhan=0.85m, TrongLuongTare=0m,
    TongTrongLuongCan=702.250m, TongSoRoCan=101, TongTrongLuong=94.700m, TongSoRo=15
};
foreach(var serialized in new[] {JsonConvert.SerializeObject(response), System.Text.Json.JsonSerializer.Serialize(response)}) {
    var payload=JObject.Parse(serialized);
    foreach(var removed in new[]{"MayCan","MaNhanVienPhucVu","MaNhanVienBanKiem","ChiSanLuong","Status","TongTLTP","DinhMucTP","DanhGia"})
        Assert(payload.Property(removed)==null,"Firmware payload omits "+removed+" even with non-default value");
    Assert(payload["Id"]!.Value<string>()==response.Id && payload["IsDataAction"]!.Value<int>()==1,"ACK fields preserved");
    Assert(payload["TrongLuongNhan"]!.Value<decimal>()==0.85m && payload["TongTrongLuongCan"]!.Value<decimal>()==702.250m,"Weight precision preserved");
    Assert(payload["MaNhanVien"]!.Value<string>()=="0022" && payload["TheId"]!.Value<string>()=="0005610611","Employee/card fields preserved");
    Assert(payload["TongSoRoCan"]!.Value<int>()==101 && payload["TongSoRo"]!.Value<int>()==15 && payload["TongTrongLuong"]!.Value<decimal>()==94.700m,"Machine/employee totals preserved");
    Assert(payload["MessStr"]!.Value<string>()=="ĐÃ HOÀN THÀNH","Unicode notification preserved");
}
Assert(response.MayCan=="7779520" && response.Status==1 && response.ChiSanLuong==1,"Serialization does not mutate response values");
var defaultPayload=JObject.Parse(JsonConvert.SerializeObject(new DataLiteRes()));
foreach(var removed in new[]{"MayCan","MaNhanVienPhucVu","MaNhanVienBanKiem","ChiSanLuong","Status","TongTLTP","DinhMucTP","DanhGia"})
    Assert(defaultPayload.Property(removed)==null,"Default firmware payload omits "+removed);
var baseline=new DateTime(2026,10,16,6,0,0);
var workshopHistory=new[] {
    new Models.Repos.Models.LichSuXuongMayCan {MayCan="7779520",MaXuong="2",TuThoiGian=baseline},
    new Models.Repos.Models.LichSuXuongMayCan {MayCan="7779520",MaXuong="1",TuThoiGian=baseline.AddHours(4)},
    new Models.Repos.Models.LichSuXuongMayCan {MayCan="7779520",MaXuong="2",TuThoiGian=baseline.AddDays(1)},
    new Models.Repos.Models.LichSuXuongMayCan {MayCan="999",MaXuong="9",TuThoiGian=baseline}
};
Assert(ScaleWorkshopHistory.Resolve(workshopHistory,"7779520",baseline.AddSeconds(-1))==null,"Before baseline: do not guess current workshop for old attendance");
Assert(ScaleWorkshopHistory.Resolve(workshopHistory,"7779520",baseline)=="2","Exact baseline captures original workshop");
Assert(ScaleWorkshopHistory.Resolve(workshopHistory,"7779520",baseline.AddHours(4).AddSeconds(-1))=="2","Offline event before move remains in old workshop");
Assert(ScaleWorkshopHistory.Resolve(workshopHistory,"7779520",baseline.AddHours(4))=="1","Exact move boundary uses new workshop");
Assert(ScaleWorkshopHistory.Resolve(workshopHistory.Reverse(),"7779520",baseline.AddHours(6))=="1","Unordered history still resolves by event time");
Assert(ScaleWorkshopHistory.Resolve(workshopHistory,"7779520",baseline.AddDays(2))=="2","Moving back creates separate interval");
Assert(ScaleWorkshopHistory.Resolve(workshopHistory,"missing",baseline.AddDays(2))==null,"Missing scale has unknown historical workshop");
Assert(ScaleWorkshopHistory.Resolve(workshopHistory,"999",baseline.AddHours(6))=="9","Do not mix history of different scales");
var unknownPeriod=workshopHistory.Append(new Models.Repos.Models.LichSuXuongMayCan {MayCan="7779520",MaXuong=null,TuThoiGian=baseline.AddDays(3)});
Assert(ScaleWorkshopHistory.Resolve(unknownPeriod,"7779520",baseline.AddDays(4))==null,"Unassigned interval does not fall through to previous workshop");
Assert(typeof(Models.Repos.Models.CheckInOut).GetProperty("MaXuong")!=null,"CheckInOut can persist workshop snapshot");
var transferMethod=typeof(ScaleAttendanceTransferService).GetMethod("Transfer")!;
Assert(!transferMethod.GetParameters().Any(p=>p.ParameterType==typeof(DateTime)||p.ParameterType==typeof(DateTimeOffset)),"Transfer API has no client-controlled timestamp parameter");
var clockMethod=typeof(ScaleAttendanceTransferService).GetMethod("ServerTransferTime",BindingFlags.Static|BindingFlags.NonPublic)!;
var clockResult=(DateTime)clockMethod.Invoke(null,new object[]{new DateTimeOffset(2026,10,16,18,30,15,987,TimeSpan.Zero)})!;
Assert(clockResult==new DateTime(2026,10,17,1,30,15),"Transfer clock uses UTC+7 and floors to firmware-second precision across midnight");
foreach (var pair in new[] {
    ("192.168.0.234","http://192.168.0.234/api/w/posts"),
    ("http://192.168.0.234","http://192.168.0.234/api/w/posts"),
    (" https://scale.local:5153/ ","https://scale.local:5153/api/w/posts")
}) Assert(DeviceSettingsAddress.Parse(pair.Item1).AbsoluteUri==pair.Item2,"Device settings URI handles IP, scheme and port: "+pair.Item1);
foreach(var invalid in new string?[]{null,""," ","http://","http://http://192.168.0.234","ftp://scale.local","http://user:pass@scale.local","http://scale.local/path"}) {
    var rejected=false;try{DeviceSettingsAddress.Parse(invalid);}catch(ArgumentException){rejected=true;}
    Assert(rejected,"Invalid device address gets clear validation, not raw URI exception");
}
Console.WriteLine($"PASS: {checks} HQ backup, workshop-history, device-address and firmware payload checks. In-memory only; no SQL connection or firmware commands.");

public class MockCommit : DispatchProxy {
    protected override object? Invoke(MethodInfo? method,object?[]? args) => method?.Name=="Commit"
        ? Tuple.Create(true,"[]") : throw new NotSupportedException(method?.Name);
}
