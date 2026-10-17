using System.Reflection;
using System.Text;
using System.Text.Json;
using Services;
using Microsoft.Extensions.Options;
using Vars.Hubs;

// Kiểm thử luồng thật, nhưng chỉ các nhánh không truy cập database.
static void Assert(bool condition, string label)
{
    if (!condition) throw new Exception("FAIL: " + label);
    Console.WriteLine("PASS: " + label);
}

const string payload = "{\"ChamCongId\":\"7779520.20261005090106\",\"TheId\":\"0002344026\",\"NgayGio\":\"20261005090106\",\"TrangThai\":\"Vào\"}";
var route = new CommunicationService(null!, null!, Options.Create(new ScaleAttendanceOptions()));
var inactive = new ClientInfo { Id = "7779520", IsActive = false, IsMayCan = true };
var result = JsonDocument.Parse(await route.Route(inactive, "CHAMCONG", Encoding.UTF8.GetByteCount(payload), payload));
var error = JsonDocument.Parse(result.RootElement.GetProperty("DataError").GetString()!);
Assert(error.RootElement.GetProperty("ErrorString").GetString() == "Client chưa active",
    "Độ dài UTF-8 của Vào được chấp nhận; lỗi chưa active có nội dung");
result = JsonDocument.Parse(await route.Route(inactive, "CHAMCONG", 1, payload));
error = JsonDocument.Parse(result.RootElement.GetProperty("DataError").GetString()!);
Assert(error.RootElement.GetProperty("ErrorString").GetString() == "Dữ Liệu Không Toàn Vẹn",
    "Độ dài sai trả DataError thay vì null");

var active = new ClientInfo { Id = "7779520", IsActive = true, IsMayCan = true };
result = JsonDocument.Parse(await route.Route(active, "CHAMCONG", 1, "{"));
error = JsonDocument.Parse(result.RootElement.GetProperty("DataError").GetString()!);
Assert(error.RootElement.GetProperty("Code").GetString() == "INVALID_JSON",
    "CHAMCONG đi vào nhánh xử lý mới và trả lỗi JSON");
var forged = ScaleAttendanceProcessor.Process(active, payload.Replace("7779520.", "7779521."));
error = JsonDocument.Parse(forged.Json);
Assert(!forged.Success && error.RootElement.GetProperty("Code").GetString() == "INVALID_ID",
    "Không cho dùng ChamCongId của máy khác");

var method = typeof(ScaleAttendanceProcessor).GetMethod("ValidateTransition",
    BindingFlags.NonPublic | BindingFlags.Static)!;
string? Transition(string? previous, string current) =>
    (string?)method.Invoke(null, new object?[] { previous, current });
Assert(Transition(null, "Vào") is null, "Lượt Vào đầu tiên hợp lệ");
Assert(Transition(null, "Ra") == "Chưa Vào. Quẹt Vào trước.", "Ra khi chưa Vào trả thông báo ngắn");
Assert((string?)typeof(ScaleAttendanceProcessor).GetMethod("OtherMachineMessage", BindingFlags.NonPublic | BindingFlags.Static)!
    .Invoke(null, new object[] {7779520}) == "Đang ở cân 7779520. Ra cân cũ.", "Thông báo ngắn giữ mã cân cần quay lại");
Assert(Transition("Vào", "Vào") is not null, "Vào liên tiếp bị từ chối");
Assert(Transition("Vào", "Ra") is null, "Vào -> Ra đúng thứ tự trong ngày");
Assert(Transition("Ra", "Vào") is null, "Ra -> Vào hợp lệ");
Assert(Transition("Ra", "Ra") is not null, "Ra liên tiếp bị từ chối");
var timingMethod = typeof(ScaleAttendanceProcessor).GetMethod("BuildTiming",
    BindingFlags.NonPublic | BindingFlags.Static)!;
JsonElement Timing(DateTime start, DateTime? end) =>
    JsonSerializer.SerializeToElement(timingMethod.Invoke(null, new object?[] { start, end, false }));
var shiftStart = new DateTime(2026, 10, 5, 8, 0, 0);
var checkInTiming = Timing(shiftStart, null);
Assert(checkInTiming.GetProperty("ThoiGianVao").GetString() == "2026-10-05 08:00:00" &&
    checkInTiming.GetProperty("ThoiGianRa").ValueKind == JsonValueKind.Null &&
    checkInTiming.GetProperty("TongThoiGianGiay").GetInt64() == 0 &&
    checkInTiming.GetProperty("TongThoiGian").GetString() == "00:00:00",
    "Vào trả mốc Vào, Ra=null, tổng thời gian bằng 0");
var checkOutTiming = Timing(shiftStart, new DateTime(2026, 10, 5, 16, 0, 0));
Assert(checkOutTiming.GetProperty("ThoiGianRa").GetString() == "2026-10-05 16:00:00" &&
    checkOutTiming.GetProperty("TongThoiGianGiay").GetInt64() == 28800 &&
    checkOutTiming.GetProperty("TongThoiGian").GetString() == "08:00:00",
    "Ca trong ngày trả đúng mốc Ra và tổng 8 giờ");
try
{
    Timing(new DateTime(2026, 10, 5, 23, 59, 59), new DateTime(2026, 10, 6, 0, 0, 0));
    throw new Exception("FAIL: Đã cộng thời gian qua ngày.");
}
catch (TargetInvocationException ex) when (ex.InnerException is ArgumentException)
{
    Console.WriteLine("PASS: 23:59:59 -> 00:00:00 bị chặn, không cộng qua ngày");
}
var repeatMethod = typeof(ScaleAttendanceProcessor).GetMethod("MarkRepeated",
    BindingFlags.NonPublic | BindingFlags.Static)!;
JsonElement Repeated(JsonElement timing, string status) =>
    JsonDocument.Parse((string)repeatMethod.Invoke(null, new object?[]
        { timing.GetRawText(), status })!).RootElement.Clone();
var repeatedIn = Repeated(checkInTiming, "Vào");
Assert(repeatedIn.GetProperty("MessStr").GetString() == "Đã quẹt Vào rồi." &&
    repeatedIn.GetProperty("Code").GetString() == "ALREADY_CHECKED_IN" &&
    !repeatedIn.GetProperty("IsValid").GetBoolean() &&
    repeatedIn.GetProperty("ThoiGianVao").GetString() == "2026-10-05 08:00:00" &&
    repeatedIn.GetProperty("ThoiGianRa").ValueKind == JsonValueKind.Null,
    "Quẹt Vào lặp trả thông báo và giữ mốc Vào đã lưu");
var repeatedOut = Repeated(checkOutTiming, "Ra");
Assert(repeatedOut.GetProperty("MessStr").GetString() == "Đã quẹt Ra rồi." &&
    repeatedOut.GetProperty("Code").GetString() == "ALREADY_CHECKED_OUT" &&
    repeatedOut.GetProperty("AlreadyRecorded").GetBoolean() &&
    repeatedOut.GetProperty("ThoiGianVao").GetString() == "2026-10-05 08:00:00" &&
    repeatedOut.GetProperty("ThoiGianRa").GetString() == "2026-10-05 16:00:00" &&
    repeatedOut.GetProperty("TongThoiGianGiay").GetInt64() == 28800 &&
    repeatedOut.GetProperty("TongThoiGian").GetString() == "08:00:00",
    "Quẹt Ra lặp giữ nguyên hai mốc và tổng ca 8 giờ");
var expirationMethod = typeof(ScaleAttendanceProcessor).GetMethod("IsShiftExpired",
    BindingFlags.NonPublic | BindingFlags.Static)!;
bool Expired(DateTime start, DateTime end, int limit) =>
    (bool)expirationMethod.Invoke(null, new object[] { start, end, limit, false })!;
Assert(!Expired(shiftStart, shiftStart.AddHours(8), 16), "Ca 8 giờ cùng ngày còn hợp lệ");
Assert(!Expired(shiftStart.Date, shiftStart.Date.AddHours(16), 16), "Đúng 16 giờ vẫn hợp lệ");
Assert(Expired(shiftStart.Date, shiftStart.Date.AddHours(16).AddSeconds(1), 16), "Quá giới hạn một giây là ca quá hạn");
Assert(Expired(new DateTime(2026, 10, 1, 8, 0, 0), new DateTime(2026, 10, 5, 8, 0, 0), 16),
    "Ca ngày 1/10 đến 5/10 không được tính là ca đang làm");
Assert(!Expired(shiftStart.Date, shiftStart.Date.AddHours(18), 20), "Giới hạn ca có thể cấu hình");
var expiredResponseMethod = typeof(ScaleAttendanceProcessor).GetMethod("ExpiredShift",
    BindingFlags.NonPublic | BindingFlags.Static)!;
var expiredResponse = ((bool Success, string Json))expiredResponseMethod.Invoke(null, new object[]
{
    new ScaleAttendanceProcessor.Request { ChamCongId = "7779520.20261005080000", TheId = "0005610480", TrangThai = "Ra" },
    new DateTime(2026, 10, 1, 8, 0, 0), new DateTime(2026, 10, 5, 8, 0, 0), 16
})!;
var expiredJson = JsonDocument.Parse(expiredResponse.Json).RootElement;
Assert(!expiredResponse.Success && expiredJson.GetProperty("Code").GetString() == "SHIFT_EXPIRED" &&
    expiredJson.GetProperty("TongThoiGianGiay").ValueKind == JsonValueKind.Null &&
    expiredJson.GetProperty("ThoiGianRa").ValueKind == JsonValueKind.Null,
    "Ca quá hạn trả lỗi, không bịa giờ Ra hoặc tổng thời gian làm");
var restartMethod = typeof(ScaleAttendanceProcessor).GetMethod("MarkRestarted",
    BindingFlags.NonPublic | BindingFlags.Static)!;
var restart = JsonDocument.Parse((string)restartMethod.Invoke(null, new object[]
    { checkInTiming.GetRawText(), new DateTime(2026, 10, 1, 8, 0, 0) })!).RootElement;
Assert(restart.GetProperty("ThoiGianVao").GetString() == "2026-10-05 08:00:00" &&
    restart.GetProperty("TongThoiGianGiay").GetInt64() == 0 &&
    restart.GetProperty("WarningCode").GetString() == "PREVIOUS_SHIFT_MISSING_OUT",
    "Ca Vào mới giữ mốc mới và cảnh báo ca trước thiếu Ra");
Assert(Expired(new DateTime(2026, 10, 5, 23, 59, 59), new DateTime(2026, 10, 6), 24),
    "Qua 0 giờ reset dù mới một giây và chưa hết giới hạn ca");
var dayMethod = typeof(ScaleAttendanceDayCloseService).GetMethod("BusinessDate",
    BindingFlags.NonPublic | BindingFlags.Static)!;
DateTime BusinessDay(DateTimeOffset utc) => (DateTime)dayMethod.Invoke(null, new object[] { utc })!;
Assert(BusinessDay(new DateTimeOffset(2026, 10, 5, 16, 59, 59, TimeSpan.Zero)) == new DateTime(2026, 10, 5) &&
    BusinessDay(new DateTimeOffset(2026, 10, 5, 17, 0, 0, TimeSpan.Zero)) == new DateTime(2026, 10, 6),
    "Chốt ngày theo UTC+7, đúng ranh giới 0 giờ");
var closeSql = (string)typeof(ScaleAttendanceDayCloseService).GetField("CloseSql",
    BindingFlags.NonPublic | BindingFlags.Static)!.GetRawConstantValue()!;
Assert(!closeSql.Contains("GhiChu") && !closeSql.Contains("MaLoi") && closeSql.Contains("SET Isforget = 1") && closeSql.Contains("TrangThai AS Direction"),
    "Chốt ngày chỉ đánh dấu Isforget, không đọc hoặc sửa ghi chú người dùng");
Console.WriteLine("Không thực hiện ghi hoặc truy vấn SQL trong các test này.");
var jobResolver = typeof(ScaleAttendanceProcessor).GetMethod("ResolveJobName",BindingFlags.NonPublic | BindingFlags.Static)!;
var productNames = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase) {
    ["001"]="Lột vỏ", ["LVT2"]="LỘT VỎ TẢI 2", ["CKTL 20x20"]="Cắt khuôn thanh long 20x20",
    ["CHICHMAT"]="Chích mắt khóm ", ["XAY"]="XAY "
};
var lookedUp = new List<string>();
Func<string,string?> findJob = code => { lookedUp.Add(code); return productNames.TryGetValue(code,out var value)?value:null; };
string ResolveJob(string? code) => (string)jobResolver.Invoke(null,new object?[]{code,findJob})!;
Assert(ResolveJob("LVT2")=="LỘT VỎ TẢI 2","Công việc lấy tên thành phẩm thay tên máy");
Assert(ResolveJob("001")=="Lột vỏ" && lookedUp.Last()=="001","Mã thành phẩm giữ số 0 đầu");
Assert(ResolveJob(" CKTL 20x20 ")=="Cắt khuôn thanh long 20x20","Giữ khoảng trắng bên trong mã thành phẩm");
Assert(ResolveJob("CHICHMAT")=="Chích mắt khóm" && ResolveJob("XAY")=="XAY","Tên thành phẩm bỏ khoảng trắng thừa ở đầu cuối");
Assert(ResolveJob("UNKNOWN")=="","Không tìm thấy thành phẩm không trả tên cân");
var lookupCount=lookedUp.Count;
Assert(ResolveJob(null)=="" && ResolveJob("")=="" && ResolveJob("  ")=="" && lookedUp.Count==lookupCount,"Chưa chọn thành phẩm không query SQL hoặc gán tên máy");
var protocolSource=File.ReadAllText(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,"../../../../../Services/ScaleAttendanceProcessor.cs")));
Assert(protocolSource.Contains("TenCongViec = job"),"Giữ khóa JSON TenCongViec để firmware không phải đổi parser");
Assert(protocolSource.Contains("TenMay = name, CongViec = name"),"Không thay đổi tên máy và lịch sử SQL trong yêu cầu đổi nhãn WSS");
Assert(protocolSource.Split("employee, jobName,").Length-1==4,"Tên thành phẩm dùng cả Vào, Ra, quẹt lặp và gửi lại Id");
var shiftJobResolver = typeof(ScaleAttendanceProcessor).GetMethod("ResolveShiftJobNames", BindingFlags.NonPublic | BindingFlags.Static)!;
productNames["LV"] = "LỘT VỎ"; productNames["BV"] = "BÀO VỎ"; productNames["002"] = "LỘT VỎ ";
string ShiftJobs(params string?[] codes) => (string)shiftJobResolver.Invoke(null, new object[] {codes, findJob})!;
Assert(ShiftJobs("LV", "BV") == "LỘT VỎ, BÀO VỎ", "Ra tổng kết cả Lột vỏ rồi Bào vỏ");
Assert(ShiftJobs("LV", "BV", "LV", "002") == "LỘT VỎ, BÀO VỎ", "Không lặp mã hoặc lặp tên thành phẩm trong tổng kết");
Assert(ShiftJobs(" BV ", "lv") == "BÀO VỎ, LỘT VỎ", "Giữ thứ tự thành phẩm phát sinh, không dùng thứ tự từ điển");
Assert(ShiftJobs("001", "CKTL 20x20") == "Lột vỏ, Cắt khuôn thanh long 20x20", "Tổng kết giữ mã có số 0 đầu và khoảng trắng nội bộ");
Assert(ShiftJobs(null, "", "UNKNOWN") == "", "Không có sản lượng có tên thì không lấy thành phẩm hiện tại hoặc tên máy thay thế");
Assert(protocolSource.Contains(".GroupBy(x => x.MaThanhPham)") && protocolSource.Contains(".OrderBy(x => x.FirstTime)")
    && protocolSource.Contains("job = ResolveShiftJobNames(products.Select"), "Nhánh Ra và gửi lại Ra gom sản phẩm lịch sử theo thời gian");
Assert(protocolSource.Contains("x.MayCan == device && x.TheIdNhanVien == request.TheId")
    && protocolSource.Contains("x.NgayGio >= start && x.NgayGio <= time && x.ChiSanLuong && x.Status == 1"), "Tổng kết giới hạn đúng máy/thẻ/ca và phiếu hợp lệ");
var productionValidate = typeof(ScaleProductionAttendancePolicy).GetMethod("Validate",
    BindingFlags.NonPublic | BindingFlags.Static)!;
var productionNow = new DateTime(2026, 10, 13, 11, 0, 0);
var oldOtherMachine = new Models.Repos.Models.CheckInOut {
    Id="14149084.20261003070009", MaChamCong="0003132217", MaSoMay=14149084,
    ThoiGian=new DateTime(2026,10,3,7,0,9), TrangThai="Vào", Isforget=1, CaDem=false
};
var closedAtCurrent = new Models.Repos.Models.CheckInOut {
    Id="7779520.20261013104154", MaChamCong="0003132217", MaSoMay=7779520,
    ThoiGian=new DateTime(2026,10,13,10,41,54), TrangThai="Ra", VaoId="7779520.20261013102837"
};
string? Production(Models.Repos.Models.CheckInOut[] open, Models.Repos.Models.CheckInOut? latest = null,
    DateTime? at = null, int hours = 16) => (string?)productionValidate.Invoke(null,
        new object?[] {open,7779520,at ?? productionNow,hours,latest});
const string closedMessage = "Quẹt Vào lại để tiếp tục cân.";
Assert(Production(new[]{oldOtherMachine},closedAtCurrent)==closedMessage,"Ca test hết hạn máy khác không che thông báo đã Ra máy hiện tại");
Assert(Production(Array.Empty<Models.Repos.Models.CheckInOut>(),closedAtCurrent)==closedMessage,"Thẻ không có ca test cũng nhận cùng thông báo đã Ra");
Assert(Production(new[]{oldOtherMachine})=="Chưa quẹt Vào tại cân.","Ca cũ máy khác không làm máy chưa Vào báo hết hạn");
Assert(Production(Array.Empty<Models.Repos.Models.CheckInOut>())=="Chưa quẹt Vào tại cân.","Thẻ chưa có lịch sử nhận thông báo chưa Vào");
var expiredHere = new Models.Repos.Models.CheckInOut {MaSoMay=7779520,ThoiGian=new DateTime(2026,10,12,7,0,0),TrangThai="Vào"};
Assert(Production(new[]{expiredHere},expiredHere)=="Chưa quẹt Vào tại cân.","Không xét ca thiếu Ra hôm qua tại máy hiện tại");
var expiredToday = new Models.Repos.Models.CheckInOut {MaSoMay=7779520,ThoiGian=productionNow.Date.AddHours(6),TrangThai="Vào"};
Assert(Production(new[]{expiredToday},expiredToday,productionNow.Date.AddHours(23))=="Ca đã hết hạn. Quẹt Vào lại tại cân.","Ca hết hạn trong ngày vẫn bị chặn");
var yesterdayOut = new Models.Repos.Models.CheckInOut {MaSoMay=7779520,ThoiGian=productionNow.Date.AddDays(-1).AddHours(17),TrangThai="Ra"};
Assert(Production(Array.Empty<Models.Repos.Models.CheckInOut>(),yesterdayOut)=="Chưa quẹt Vào tại cân.","Giờ Ra hôm qua không thay trạng thái hôm nay");
Assert(!ScaleAttendanceTransferService.IsInOperationalDay(oldOtherMachine,productionNow,16)
    && !ScaleAttendanceTransferService.IsInOperationalDay(expiredHere,productionNow,16),"Bỏ lịch sử ngày 3/10 và hôm qua khỏi quyết định Vào mới");
var overnight = new Models.Repos.Models.CheckInOut {MaSoMay=7779520,ThoiGian=productionNow.Date.AddDays(-1).AddHours(22),TrangThai="Vào",CaDem=true};
Assert(ScaleAttendanceTransferService.IsInOperationalDay(overnight,productionNow,16)
    && Production(new[]{overnight},overnight)==null,"Giữ ca đêm 22 giờ đến 11 giờ còn hạn");
Assert(!ScaleAttendanceTransferService.IsInOperationalDay(overnight,productionNow.Date.AddHours(15),16),"Không xét ca đêm ngày cũ khi đã hết hạn");
var activeHere = new Models.Repos.Models.CheckInOut {MaSoMay=7779520,ThoiGian=productionNow.AddHours(-1),TrangThai="Vào"};
Assert(Production(new[]{oldOtherMachine,activeHere},activeHere)==null,"Ca hợp lệ hiện tại không bị ca test cũ chặn");
activeHere.Isforget=1;
Assert(Production(new[]{activeHere},activeHere)=="Ca đang lỗi. Nhờ quản lý kiểm tra.","Không bỏ kiểm tra cờ lỗi ca hiện tại");
activeHere.Isforget=0;
var activeElsewhere = new Models.Repos.Models.CheckInOut {MaSoMay=14149084,ThoiGian=productionNow.AddHours(-1),TrangThai="Vào"};
var otherMachineMessage = (string)typeof(ScaleAttendanceProcessor).GetMethod("OtherMachineMessage",
    BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null,new object[]{14149084})!;
Assert(Production(new[]{activeElsewhere},closedAtCurrent)==otherMachineMessage,"Ca máy khác còn hạn vẫn chặn cân sai máy");
Assert(Production(new[]{activeHere,activeElsewhere},activeHere)=="Trùng ca nhiều cân. Báo quản lý.","Nhiều ca đang mở vẫn cần kiểm tra");
Assert(Production(new[]{activeHere},activeHere,productionNow,hours:0)=="Cấu hình ca không hợp lệ. Nhờ quản lý kiểm tra.","Giữ kiểm tra cấu hình ca");
Assert(Production(new[]{activeHere},activeHere,activeHere.ThoiGian.AddSeconds(-1))=="Giờ cân trước lượt Vào. Kiểm tra giờ máy.","Giữ kiểm tra đồng hồ trước giờ Vào");
Assert(oldOtherMachine.Isforget==1 && oldOtherMachine.TrangThai=="Vào" && closedAtCurrent.VaoId=="7779520.20261013102837","Không sửa ca nguồn khi chọn thông báo");
