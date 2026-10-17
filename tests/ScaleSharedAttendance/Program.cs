using Microsoft.EntityFrameworkCore;
using Models.Repos.Models;
using Services;
using System.Reflection;

var checks = 0;
void Assert(bool condition, string label)
{
    if (!condition) throw new Exception("FAIL: " + label);
    checks++;
}

var start = new DateTime(2026, 10, 17, 8, 0, 0);
var shift = new CheckInOut { Id = "101.20261017080000", MaChamCong = "card1",
    MaSoMay = 101, ThoiGian = start, TrangThai = "Vào", TenMay = "A" };
var joined = new ScaleAttendanceScan { Id = "102.20261017090000", VaoId = shift.Id,
    MaSoMay = 102, ThoiGian = start.AddHours(1), TrangThai = "Vào", MaThanhPham = "001" };
var scans = new[] { joined };
Assert(ScaleSharedAttendancePolicy.SameProduct(" 001 ", "001"), "Bỏ khoảng trắng đầu cuối");
Assert(ScaleSharedAttendancePolicy.SameProduct("tp01", "TP01"), "So mã không phân biệt hoa thường");
Assert(!ScaleSharedAttendancePolicy.SameProduct("001", "1"), "Giữ số 0 đầu mã");
Assert(!ScaleSharedAttendancePolicy.SameProduct(null, null), "Thiếu mã không được xem là cùng thành phẩm");
Assert(!ScaleSharedAttendancePolicy.SameProduct("001", "002"), "Khác mã phải chuyển ca");
Assert(ScaleSharedAttendancePolicy.CanUseMachine(shift, scans, 101, start, "OTHER"), "Giữ hành vi cân gốc");
Assert(!ScaleSharedAttendancePolicy.CanUseMachine(shift, scans, 102, start.AddMinutes(59), "001"), "Chưa Vào cân B chưa được cân");
Assert(ScaleSharedAttendancePolicy.CanUseMachine(shift, scans, 102, joined.ThoiGian, "001"), "Đúng mốc Vào cân B được cân");
Assert(!ScaleSharedAttendancePolicy.CanUseMachine(shift, scans, 102, start.AddHours(2), "002"), "Cân tham gia không tự đổi mã ca");
Assert(!ScaleSharedAttendancePolicy.CanUseMachine(shift, scans, 103, start.AddHours(2), "001"), "Cân C chưa tham gia không được cân");
Assert(!ScaleSharedAttendancePolicy.CanUseMachine(shift, new[] { new ScaleAttendanceScan {
    VaoId = "other", MaSoMay = 102, ThoiGian = start, TrangThai = "Vào", MaThanhPham = "001" } },
    102, start.AddHours(2), "001"), "Không dùng lượt Vào của ca khác");
Assert(ScaleSharedAttendancePolicy.ValidateContinuation(shift, "001", "001", joined.ThoiGian, 16) == null,
    "Cùng mã tiếp tục ca hợp lệ");
Assert(ScaleSharedAttendancePolicy.ValidateContinuation(shift, "001", "002", joined.ThoiGian, 16) == null,
    "Khác mã đủ điều kiện đóng/mở ca");
Assert(ScaleSharedAttendancePolicy.ValidateContinuation(shift, null, "001", joined.ThoiGian, 16) != null,
    "Không đoán mã thành phẩm lịch sử");
Assert(ScaleSharedAttendancePolicy.ValidateContinuation(shift, "001", "", joined.ThoiGian, 16) != null,
    "Chưa chọn thành phẩm không tự đóng ca");
Assert(ScaleSharedAttendancePolicy.ValidateContinuation(shift, "001", "002", start, 16) != null,
    "Không chuyển tại hoặc trước giờ Vào");
Assert(ScaleSharedAttendancePolicy.ValidateContinuation(shift, "001", "002", start.AddHours(17), 16) != null,
    "Không tự đóng ca quá hạn");
shift.Isforget = 1;
Assert(ScaleSharedAttendancePolicy.ValidateContinuation(shift, "001", "002", joined.ThoiGian, 16) != null,
    "Không tự đóng ca đang lỗi");
shift.Isforget = 0;
HQ_PhieuCan Weight(string id, string machine, DateTime time, string product = "001", string card = "card1") =>
    new() { Id = id, MayCan = machine, NgayGio = time, MaThanhPham = product,
        TheIdNhanVien = card, TheId = card, MaLo = "L", MaSize = "S", MaLoaiNguyenLieu = "NL",
        TrongLuong = 3m, TrongLuongTare = 0.5m, ChiSanLuong = true, Status = 1 };
var weights = new[] {
    Weight("a", "101", start.AddMinutes(30)), Weight("b", "102", start.AddMinutes(90)),
    Weight("before", "102", start.AddMinutes(59)), Weight("wrong-product", "102", start.AddMinutes(90), "002"),
    Weight("wrong-card", "102", start.AddMinutes(90), card: "card2"),
    Weight("other-machine", "103", start.AddMinutes(90)), Weight("after", "102", start.AddHours(3)),
    Weight("previous", "101", start.AddSeconds(-1)), Weight("cancelled", "101", start.AddMinutes(30)),
    Weight("not-production", "101", start.AddMinutes(30)) };
weights[8].Status = 0;
weights[9].ChiSanLuong = false;
var selected = ScaleSharedAttendancePolicy.ShiftWeights(weights.AsQueryable(), scans.AsQueryable(), shift,
    start.AddHours(2)).ToList();
Assert(selected.Select(x => x.Id).SequenceEqual(new[] { "a", "b" }), "Chỉ cộng đúng máy, thẻ, mã, thời điểm và phiếu hợp lệ");
Assert(selected.Sum(x => x.TrongLuong - x.TrongLuongTare) == 5m, "Cộng sản lượng A+B, trừ tare");
Assert(shift.ThoiGian == start && shift.TrangThai == "Vào", "Tiếp tục không đổi giờ Vào hoặc đóng ca gốc");
using var translationDb = new TranslationDb();
var sql = ScaleSharedAttendancePolicy.ShiftWeights(translationDb.Set<HQ_PhieuCan>(),
    translationDb.Set<ScaleAttendanceScan>(), shift, start.AddHours(2)).ToQueryString();
Assert(sql.Contains("EXISTS") && sql.Contains("ScaleAttendanceScan"), "Tổng ca được dịch thành truy vấn SQL tại server");
var validate = typeof(ScaleProductionAttendancePolicy).GetMethod("ValidateShared", BindingFlags.Static | BindingFlags.NonPublic)!;
string? Production(CheckInOut[] roots, ScaleAttendanceScan[] members, int machine = 102,
    string product = "001", DateTime? time = null) => (string?)validate.Invoke(null,
        new object?[] { roots, members, machine, time ?? start.AddHours(2), 16, null, product });
Assert(Production(new[] { shift }, scans) == null, "Ca chung cho phép cân B ghi phiếu");
Assert(Production(new[] { shift }, scans, product: "002") != null, "Cân B đổi mã phải quẹt Vào chuyển ca");
Assert(Production(new[] { shift }, scans, machine: 103) != null, "Không mở quyền cho mọi cân cùng mã");
Assert(Production(Array.Empty<CheckInOut>(), scans) != null, "Lượt tham gia cũ không mở lại ca đã đóng");
Assert(Production(new[] { shift }, scans, time: start.AddMinutes(59)) != null, "Không dùng lượt Vào tương lai để cân");
shift.Isforget = 1;
Assert(Production(new[] { shift }, scans) == "Ca đang lỗi. Nhờ quản lý kiểm tra.", "Cân B không bỏ qua cờ lỗi ca gốc");
shift.Isforget = 0;
Assert(Production(new[] { shift }, scans, time: start.AddHours(17)) != null, "Đổi cân không kéo dài ca quá hạn");
var secondRoot = new CheckInOut { Id = "103.20261017090000", MaSoMay = 103, ThoiGian = start.AddHours(1), TrangThai = "Vào" };
Assert(Production(new[] { shift, secondRoot }, scans) == "Trùng ca nhiều cân. Báo quản lý.", "Không tự gộp dữ liệu trùng thật");
var service = typeof(ScaleSharedAttendancePolicy).Assembly.GetType("Services.ScaleSharedAttendanceService")!;
var checkout = (CheckInOut)service.GetMethod("Checkout", BindingFlags.Static | BindingFlags.NonPublic)!
    .Invoke(null, new object[] { shift, "102.20261017100000", start.AddHours(2) })!;
Assert(checkout.VaoId == shift.Id && checkout.MaSoMay == shift.MaSoMay && checkout.TrangThai == "Ra",
    "Ra từ cân B đóng ca gốc, giữ cách ghép giờ theo máy cũ");
Assert(checkout.ThoiGian - shift.ThoiGian == TimeSpan.FromHours(2), "Giờ công chỉ tính một lần từ Vào A");
Assert(shift.TrangThai == "Vào" && shift.ThoiGian == start, "Đóng ca bằng phiếu Ra mới, không sửa phiếu Vào cũ");
Assert(ScaleSharedAttendancePolicy.SelectCheckInAction(shift, 102, "001", "001") ==
    ScaleSharedAttendancePolicy.CheckInAction.Continue, "Vào B cùng mã chỉ tiếp tục ca");
Assert(ScaleSharedAttendancePolicy.SelectCheckInAction(shift, 102, "001", "002") ==
    ScaleSharedAttendancePolicy.CheckInAction.Switch, "Vào B khác mã đóng/mở ca");
Assert(ScaleSharedAttendancePolicy.SelectCheckInAction(shift, 101, "001", "001") ==
    ScaleSharedAttendancePolicy.CheckInAction.Repeat, "Giữ cách xử lý Vào lặp tại máy gốc");
Assert(ScaleSharedAttendancePolicy.SelectCheckInAction(shift, 101, "001", "002") ==
    ScaleSharedAttendancePolicy.CheckInAction.Switch, "Trở lại máy gốc khác mã vẫn chuyển ca");
var switched = new CheckInOut { Id = "102.20261017100000", MaChamCong = shift.MaChamCong,
    MaSoMay = 102, ThoiGian = start.AddHours(2), TrangThai = "Vào", TenMay = "B" };
var automaticOut = new CheckInOut { Id = shift.Id + ".AUTO.20261017100000", MaChamCong = shift.MaChamCong,
    MaSoMay = shift.MaSoMay, ThoiGian = switched.ThoiGian, TrangThai = "Ra", VaoId = shift.Id };
var ended = new CheckInOut { Id = "102.20261017110000", MaChamCong = shift.MaChamCong,
    MaSoMay = switched.MaSoMay, ThoiGian = start.AddHours(3), TrangThai = "Ra", VaoId = switched.Id };
var events = new[] { shift, automaticOut, switched, ended }.AsQueryable();
CheckInOut[] At(DateTime time) => ScaleSharedAttendancePolicy.ShiftsAtTime(events, "card1", time).ToArray();
Assert(At(start.AddHours(1)).Single().Id == shift.Id, "Backup trước chuyển vẫn thuộc ca A");
Assert(At(switched.ThoiGian).Single().Id == switched.Id, "Đúng giây chuyển chỉ thuộc ca B, không phụ thuộc thứ tự Id");
Assert(At(start.AddMinutes(150)).Single().Id == switched.Id, "Backup sau chuyển thuộc ca mới");
Assert(At(ended.ThoiGian).Length == 0, "Không ghi backup sau thời điểm Ra");
Assert(ScaleSharedAttendancePolicy.ShiftsAtTime(events, "other-card", start.AddHours(1)).Count() == 0,
    "Không lấy ca lịch sử của thẻ khác");
var duplicateRoots = new[] { shift, new CheckInOut { Id = "103.old", MaChamCong = "card1", MaSoMay = 103,
    ThoiGian = start.AddMinutes(10), TrangThai = "Vào" } }.AsQueryable();
Assert(ScaleSharedAttendancePolicy.ShiftsAtTime(duplicateRoots, "card1", start.AddHours(1)).Count() == 2,
    "Phát hiện hai ca lịch sử chưa đóng, không tự chọn một ca");
var boundary = Weight("boundary", "102", switched.ThoiGian);
Assert(ScaleSharedAttendancePolicy.ShiftWeights(new[] { boundary }.AsQueryable(), scans.AsQueryable(),
    shift, switched.ThoiGian, events).Count() == 0, "Giây chuyển không cộng trùng phiếu vào ca cũ");
Assert(ScaleSharedAttendancePolicy.ShiftWeights(new[] { boundary }.AsQueryable(), scans.AsQueryable(),
    switched, ended.ThoiGian, events).Count() == 1, "Giây chuyển tính phiếu cho ca mới");
var legacyOut = new CheckInOut { Id = "legacy-out", MaChamCong = "card1", MaSoMay = 101,
    ThoiGian = start.AddHours(2), TrangThai = "Ra" };
Assert(!ScaleSharedAttendancePolicy.ShiftsAtTime(new[] { shift, legacyOut }.AsQueryable(), "card1", start.AddHours(3)).Any(),
    "Giữ ghép Ra lịch sử chưa có VaoId");
var lateOldOut = new CheckInOut { Id = "late-old-out", MaChamCong = "card1", MaSoMay = 101,
    ThoiGian = start.AddHours(3), TrangThai = "Ra", VaoId = shift.Id };
var newAtOrigin = new CheckInOut { Id = "101.new", MaChamCong = "card1", MaSoMay = 101,
    ThoiGian = start.AddHours(2), TrangThai = "Vào" };
Assert(ScaleSharedAttendancePolicy.ShiftsAtTime(new[] { shift, newAtOrigin, lateOldOut }.AsQueryable(),
    "card1", start.AddHours(4)).Single().Id == newAtOrigin.Id, "Ra bổ sung ca trước không đóng nhầm ca sau");
Assert(ScaleSharedAttendancePolicy.ShiftsAtTime(translationDb.Set<CheckInOut>(), "card1", start.AddHours(2))
    .ToQueryString().Contains("NOT EXISTS"), "Tra ca lịch sử được lọc tại SQL");
Assert(ScaleSharedAttendancePolicy.ShiftWeights(translationDb.Set<HQ_PhieuCan>(), translationDb.Set<ScaleAttendanceScan>(),
    shift, start.AddHours(2), translationDb.Set<CheckInOut>()).ToQueryString().Contains("NOT EXISTS"),
    "SQL tổng ca loại ranh giới chuyển ca để tránh cộng trùng");
var initialOld = new ScaleAttendanceScan { Id = shift.Id, VaoId = shift.Id, MaSoMay = 101,
    ThoiGian = start, TrangThai = "Vào", MaThanhPham = "001" };
var initialNew = new ScaleAttendanceScan { Id = switched.Id, VaoId = switched.Id, MaSoMay = 102,
    ThoiGian = switched.ThoiGian, TrangThai = "Vào", MaThanhPham = "002" };
var boundaryScans = new[] { initialOld, joined, initialNew }.AsQueryable();
var boundaryWeights = new[] {
    Weight("old-at-A", "101", switched.ThoiGian),
    Weight("old-at-B", "102", switched.ThoiGian),
    Weight("new-at-B", "102", switched.ThoiGian, "002") }.AsQueryable();
var oldPart = ScaleSharedAttendancePolicy.ShiftWeights(boundaryWeights, boundaryScans, shift, switched.ThoiGian, events).ToArray();
var newPart = ScaleSharedAttendancePolicy.ShiftWeights(boundaryWeights, boundaryScans, switched, ended.ThoiGian, events).ToArray();
Assert(oldPart.Select(x => x.Id).SequenceEqual(new[] { "old-at-A", "old-at-B" }),
    "Giây chuyển không bỏ mất phiếu mã cũ tại A/B");
Assert(newPart.Single().Id == "new-at-B", "Giây chuyển mã mới chỉ thuộc ca mới");
Assert(oldPart.Length + newPart.Length == boundaryWeights.Count(), "Mỗi phiếu ranh giới được cộng đúng một lần");
Assert(!ScaleSharedAttendancePolicy.CanUseMachine(switched, boundaryScans, 102, switched.ThoiGian, "001"),
    "Đúng giây Vào mới không dùng mã cũ làm sản lượng ca mới");
Assert(ScaleSharedAttendancePolicy.CanUseMachine(switched, boundaryScans, 102, switched.ThoiGian, "002"),
    "Đúng giây Vào mới chấp nhận đúng mã mới");
Assert(!ScaleSharedAttendancePolicy.CanUseMachine(switched, boundaryScans, 102, switched.ThoiGian.AddTicks(-1), "002"),
    "Không nhận phiếu trước giờ Vào máy gốc");
var originScan = new ScaleAttendanceScan { Id = newAtOrigin.Id, VaoId = newAtOrigin.Id, MaSoMay = 101,
    ThoiGian = newAtOrigin.ThoiGian, TrangThai = "Vào", MaThanhPham = "002" };
var sameMachineEvents = new[] { shift, automaticOut, newAtOrigin }.AsQueryable();
var sameMachineScans = new[] { initialOld, originScan }.AsQueryable();
var sameMachineWeights = new[] { Weight("old", "101", newAtOrigin.ThoiGian),
    Weight("new", "101", newAtOrigin.ThoiGian, "002") }.AsQueryable();
Assert(ScaleSharedAttendancePolicy.ShiftWeights(sameMachineWeights, sameMachineScans, shift,
    newAtOrigin.ThoiGian, sameMachineEvents).Single().Id == "old", "Đổi mã tại máy gốc giữ phiếu mã cũ cùng giây");
Assert(ScaleSharedAttendancePolicy.ShiftWeights(sameMachineWeights, sameMachineScans, newAtOrigin,
    newAtOrigin.ThoiGian.AddHours(1), sameMachineEvents).Single().Id == "new", "Đổi mã tại máy gốc không cộng trùng mã mới");
Console.WriteLine($"PASS: {checks} kiểm tra ca chung; không kết nối SQL hoặc gửi lệnh cân.");

sealed class TranslationDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseSqlServer("Server=(local);Database=TranslationOnly;Integrated Security=True;TrustServerCertificate=True");
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<HQ_PhieuCan>();
        model.Entity<ScaleAttendanceScan>();
        model.Entity<CheckInOut>();
    }
}
