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
Assert(Transition(null, "Ra") is not null, "Ra khi chưa Vào bị từ chối");
Assert(Transition("Vào", "Vào") is not null, "Vào liên tiếp bị từ chối");
Assert(Transition("Vào", "Ra") is null, "Vào -> Ra đúng thứ tự trong ngày");
Assert(Transition("Ra", "Vào") is null, "Ra -> Vào hợp lệ");
Assert(Transition("Ra", "Ra") is not null, "Ra liên tiếp bị từ chối");
var timingMethod = typeof(ScaleAttendanceProcessor).GetMethod("BuildTiming",
    BindingFlags.NonPublic | BindingFlags.Static)!;
JsonElement Timing(DateTime start, DateTime? end) =>
    JsonSerializer.SerializeToElement(timingMethod.Invoke(null, new object?[] { start, end }));
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
    (bool)expirationMethod.Invoke(null, new object[] { start, end, limit })!;
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
