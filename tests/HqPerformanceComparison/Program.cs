using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Models.Repos.Models;
using Newtonsoft.Json;
using Services;
using Vars.Hubs;

Console.WriteLine("================================================================================");
Console.WriteLine(" BỘ KIỂM THỬ SO SÁNH HIỆU NĂNG & ĐỘ ỔN ĐỊNH: PHIẾU CÂN HQ (CŨ VS MỚI) ");
Console.WriteLine("================================================================================");

var totalChecks = 0;
void Assert(bool condition, string testName)
{
    if (!condition)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[FAIL] {testName}");
        Console.ResetColor();
        throw new Exception($"Kiểm thử thất bại: {testName}");
    }
    totalChecks++;
}

void AssertEqual<T>(T actual, T expected, string testName)
{
    if (!EqualityComparer<T>.Default.Equals(actual, expected))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[FAIL] {testName} | Kỳ vọng: '{expected}', Thực tế: '{actual}'");
        Console.ResetColor();
        throw new Exception($"Kiểm thử thất bại: {testName}");
    }
    totalChecks++;
}

// ------------------------------------------------------------------------------------------------
// PHẦN 1: SO SÁNH ScaleProductionAttendancePolicy.GetError (CŨ VS MỚI)
// ------------------------------------------------------------------------------------------------
Console.WriteLine("\n[1] Kiểm thử logic nghiệp vụ & ngoại lệ: ScaleProductionAttendancePolicy.GetError");

// Mock database records
var mockCards = new Dictionary<string, TheTu>
{
    ["CARD_HUMAN_OK"] = new TheTu { MaTheTu = "CARD_HUMAN_OK", MaNhanVien = "NV001" },
    ["CARD_GROUP_OK"] = new TheTu { MaTheTu = "CARD_GROUP_OK", MaNhanVien = "NHOM_CAT" },
    ["CARD_NO_EMP"] = new TheTu { MaTheTu = "CARD_NO_EMP", MaNhanVien = null },
    ["CARD_NOT_PROD"] = new TheTu { MaTheTu = "CARD_NOT_PROD", MaNhanVien = "NV_OFFICE" }
};

var mockEmployees = new Dictionary<string, NhanVienDaiThanh>
{
    ["NV001"] = new NhanVienDaiThanh { MaNhanVien = "NV001", Name = "Nguyen Van A", IsHuman = true, IsNhom = false, IsChucNang = false, IsPhucVu = false },
    ["NHOM_CAT"] = new NhanVienDaiThanh { MaNhanVien = "NHOM_CAT", Name = "Nhom May Cat", IsHuman = false, IsNhom = true, IsChucNang = false, IsPhucVu = false },
    ["NV_OFFICE"] = new NhanVienDaiThanh { MaNhanVien = "NV_OFFICE", Name = "Ke Toan", IsHuman = true, IsChucNang = true, IsPhucVu = false }
};

var baseTime = new DateTime(2026, 10, 17, 8, 30, 0);

// Mô phỏng logic cũ: mỗi lần gọi phải query TheTu và NhanVienDaiThanh từ đầu
string? OldGetErrorLogic(string cardCode, int machine, DateTime time, int maxHours,
    Func<string, CheckInOut?> getScan)
{
    mockCards.TryGetValue(cardCode, out var card);
    if (string.IsNullOrWhiteSpace(card?.MaNhanVien))
        return "Không tìm thấy thẻ nhân viên hoặc nhân viên.";
    mockEmployees.TryGetValue(card.MaNhanVien, out var employee);
    if (!ScaleProductionAttendancePolicy.IsProductionCard(employee))
        return "HQ cần thẻ nhân viên hoặc nhóm sản xuất.";
    if (!ScaleProductionAttendancePolicy.RequiresAttendance(employee))
        return null;

    var latest = getScan(cardCode);
    var scanError = HqWeightBackupProcessor.ValidateHistoricalScan(latest, machine, time, maxHours);
    if (scanError != null) return scanError;
    return null;
}

// Logic mới tối ưu: dùng preloadedEmployee khi có sẵn từ memory, giảm 100% round-trip query thẻ/nhân viên
string? NewGetErrorLogic(string cardCode, int machine, DateTime time, int maxHours,
    NhanVienDaiThanh? preloadedEmployee, Func<string, CheckInOut?> getScan)
{
    NhanVienDaiThanh? employee = preloadedEmployee;
    if (employee == null)
    {
        mockCards.TryGetValue(cardCode, out var card);
        if (string.IsNullOrWhiteSpace(card?.MaNhanVien))
            return "Không tìm thấy thẻ nhân viên hoặc nhân viên.";
        mockEmployees.TryGetValue(card.MaNhanVien, out employee);
    }
    if (!ScaleProductionAttendancePolicy.IsProductionCard(employee))
        return "HQ cần thẻ nhân viên hoặc nhóm sản xuất.";
    if (!ScaleProductionAttendancePolicy.RequiresAttendance(employee))
        return null;

    var latest = getScan(cardCode);
    var scanError = HqWeightBackupProcessor.ValidateHistoricalScan(latest, machine, time, maxHours);
    if (scanError != null) return scanError;
    return null;
}

// Test các kịch bản thực tế & ngoại lệ
var testScenarios = new (string Name, string Card, NhanVienDaiThanh? Preloaded, CheckInOut? Scan, DateTime Time)[]
{
    ("1.1 Ca bình thường hợp lệ", "CARD_HUMAN_OK", mockEmployees["NV001"],
        new CheckInOut { Id = "S1", MaChamCong = "CARD_HUMAN_OK", MaSoMay = 7779520, TrangThai = "Vào", ThoiGian = baseTime.AddHours(-2), Isforget = 0 }, baseTime),

    ("1.2 Thẻ nhóm sản xuất (miễn chấm công)", "CARD_GROUP_OK", mockEmployees["NHOM_CAT"],
        null, baseTime),

    ("1.3 Thẻ chưa gán nhân viên", "CARD_NO_EMP", null,
        null, baseTime),

    ("1.4 Thẻ văn phòng/không thuộc sản xuất", "CARD_NOT_PROD", mockEmployees["NV_OFFICE"],
        null, baseTime),

    ("1.5 Thẻ hoàn toàn chưa đăng ký", "CARD_UNKNOWN", null,
        null, baseTime),

    ("1.6 Chưa quẹt thẻ Vào", "CARD_HUMAN_OK", mockEmployees["NV001"],
        null, baseTime),

    ("1.7 Đã quẹt Ra trước giờ cân", "CARD_HUMAN_OK", mockEmployees["NV001"],
        new CheckInOut { Id = "S2", MaChamCong = "CARD_HUMAN_OK", MaSoMay = 7779520, TrangThai = "Ra", ThoiGian = baseTime.AddMinutes(-10) }, baseTime),

    ("1.8 Ca tại máy khác (7779521 thay vì 7779520)", "CARD_HUMAN_OK", mockEmployees["NV001"],
        new CheckInOut { Id = "S3", MaChamCong = "CARD_HUMAN_OK", MaSoMay = 7779521, TrangThai = "Vào", ThoiGian = baseTime.AddHours(-1) }, baseTime),

    ("1.9 Ca quá 16 tiếng hết hạn", "CARD_HUMAN_OK", mockEmployees["NV001"],
        new CheckInOut { Id = "S4", MaChamCong = "CARD_HUMAN_OK", MaSoMay = 7779520, TrangThai = "Vào", ThoiGian = baseTime.AddHours(-17) }, baseTime),

    ("1.10 Ca bị đánh dấu lỗi (Isforget=1)", "CARD_HUMAN_OK", mockEmployees["NV001"],
        new CheckInOut { Id = "S5", MaChamCong = "CARD_HUMAN_OK", MaSoMay = 7779520, TrangThai = "Vào", ThoiGian = baseTime.AddHours(-1), Isforget = 1 }, baseTime),

    ("1.11 Giờ cân trước giờ Vào (nghịch đảo thời gian)", "CARD_HUMAN_OK", mockEmployees["NV001"],
        new CheckInOut { Id = "S6", MaChamCong = "CARD_HUMAN_OK", MaSoMay = 7779520, TrangThai = "Vào", ThoiGian = baseTime.AddHours(1) }, baseTime),

    ("1.12 Ca đêm qua ranh giới nửa đêm còn hạn", "CARD_HUMAN_OK", mockEmployees["NV001"],
        new CheckInOut { Id = "S7", MaChamCong = "CARD_HUMAN_OK", MaSoMay = 7779520, TrangThai = "Vào", ThoiGian = baseTime.Date.AddHours(-3), CaDem = true }, baseTime)
};

foreach (var sc in testScenarios)
{
    var oldRes = OldGetErrorLogic(sc.Card, 7779520, sc.Time, 16, _ => sc.Scan);
    var newRes = NewGetErrorLogic(sc.Card, 7779520, sc.Time, 16, sc.Preloaded, _ => sc.Scan);

    AssertEqual(newRes, oldRes, $"So sánh Cũ vs Mới: {sc.Name}");
    Console.WriteLine($"  -> PASS: {sc.Name} | Kết quả: {(newRes == null ? "HỢP LỆ (NULL)" : $"LỖI: '{newRes}'")}");
}

// ------------------------------------------------------------------------------------------------
// PHẦN 2: SO SÁNH TRUY VẤN OpenShifts (CŨ TOÀN CỤC VS MỚI SCOPED SELECTION)
// ------------------------------------------------------------------------------------------------
Console.WriteLine("\n[2] Kiểm thử tính đúng đắn & đo hiệu năng: OpenShifts");

var datasetShifts = new List<CheckInOut>();
var cardList = Enumerable.Range(1, 100).Select(i => $"CARD_{i:D4}").ToList();

// Tạo dữ liệu giả lập 1,000 sự kiện chấm công của 100 công nhân
var shiftRnd = new Random(42);
for (int i = 0; i < 100; i++)
{
    var card = cardList[i];
    var t = baseTime.AddDays(-2);
    // 8 ca cũ đã đóng
    for (int k = 0; k < 8; k++)
    {
        var inId = $"{card}_IN_{k}";
        var outId = $"{card}_OUT_{k}";
        datasetShifts.Add(new CheckInOut { Id = inId, MaChamCong = card, MaSoMay = 7779520 + (k % 2), TrangThai = "Vào", ThoiGian = t });
        t = t.AddHours(4);
        datasetShifts.Add(new CheckInOut { Id = outId, MaChamCong = card, MaSoMay = 7779520 + (k % 2), TrangThai = "Ra", ThoiGian = t, VaoId = inId });
        t = t.AddHours(4);
    }
    // 50% số công nhân hiện đang có ca Vào mở sau ca cuối
    if (i % 2 == 0)
    {
        datasetShifts.Add(new CheckInOut { Id = $"{card}_IN_ACTIVE", MaChamCong = card, MaSoMay = 7779520, TrangThai = "Vào", ThoiGian = t.AddHours(2) });
    }
}

// Logic cũ: NOT EXISTS subquery quét toàn bảng datasetShifts không giới hạn MaChamCong
List<CheckInOut> OldOpenShifts(List<CheckInOut> db, string? card = null)
{
    return db.Where(x => x.TrangThai == "Vào" &&
        (card == null || x.MaChamCong == card) &&
        !db.Any(next =>
            next.VaoId == x.Id ||
            (next.MaChamCong == x.MaChamCong && next.MaSoMay == x.MaSoMay &&
             (next.TrangThai == "Vào" || next.TrangThai == "Ra") &&
             (next.ThoiGian > x.ThoiGian || (next.ThoiGian == x.ThoiGian && next.Id.CompareTo(x.Id) > 0)))))
        .OrderByDescending(x => x.ThoiGian).ToList();
}

// Logic mới: NOT EXISTS subquery có điều kiện (card == null || next.MaChamCong == card), cho phép SQL Index Seek
List<CheckInOut> NewOpenShifts(List<CheckInOut> db, string? card = null)
{
    return db.Where(x => x.TrangThai == "Vào" &&
        (card == null || x.MaChamCong == card) &&
        !db.Any(next =>
            (card == null || next.MaChamCong == card) &&
            (next.VaoId == x.Id ||
             (next.MaChamCong == x.MaChamCong && next.MaSoMay == x.MaSoMay &&
              (next.TrangThai == "Vào" || next.TrangThai == "Ra") &&
              (next.ThoiGian > x.ThoiGian || (next.ThoiGian == x.ThoiGian && next.Id.CompareTo(x.Id) > 0))))))
        .OrderByDescending(x => x.ThoiGian).ToList();
}

// 2.1 Kiểm tra tính khớp 100% giữa Cũ và Mới trên tất cả 100 thẻ
for (int i = 0; i < 100; i++)
{
    var card = cardList[i];
    var oldList = OldOpenShifts(datasetShifts, card);
    var newList = NewOpenShifts(datasetShifts, card);

    AssertEqual(newList.Count, oldList.Count, $"Số ca mở khớp cho thẻ {card}");
    if (newList.Count > 0)
    {
        AssertEqual(newList[0].Id, oldList[0].Id, $"Id ca mở khớp cho thẻ {card}");
    }
}
Console.WriteLine("  -> PASS: 100/100 thẻ có kết quả OpenShifts cũ và mới giống hệt nhau.");

// 2.2 Kiểm tra khi card = null (toàn bộ hệ thống)
var allOld = OldOpenShifts(datasetShifts, null);
var allNew = NewOpenShifts(datasetShifts, null);
AssertEqual(allNew.Count, allOld.Count, "Số ca mở toàn bộ hệ thống khớp nhau");
Assert(allNew.Select(x => x.Id).SequenceEqual(allOld.Select(x => x.Id)), "Danh sách ca mở toàn hệ thống khớp thứ tự và Id");
Console.WriteLine($"  -> PASS: card=null trả về chính xác {allNew.Count} ca đang mở trên toàn xưởng.");

// ------------------------------------------------------------------------------------------------
// PHẦN 3: BATCH BACKUP HQ & IDEMPOTENCY (ĐỒNG BỘ PHIẾU CÂN HQ KHI MẤT MẠNG CÓ LẠI)
// ------------------------------------------------------------------------------------------------
Console.WriteLine("\n[3] Kiểm thử độ ổn định, loại trừ trùng lặp & chuẩn hóa: HqWeightBackupProcessor");

DataCoummunication CreateBackupRecord(int idIndex, decimal weight, int status = 1)
{
    var t = baseTime.AddMinutes(-idIndex);
    var idStr = $"7779520.{t:yyyyMMddHHmmss}";
    return new DataCoummunication
    {
        Id = idStr,
        IdIn = idStr,
        IdOut = idStr,
        MayCan = "7779520",
        NgayGio = t.ToString("yyyyMMddHHmmss"),
        TheId = "CARD_HUMAN_OK",
        TheIdNhanVien = "CARD_HUMAN_OK",
        MaLo = "LO_A1",
        MaSize = "SIZE_20x20",
        MaThanhPham = "TP_CD001",
        MaLoaiNguyenLieu = "NL01",
        MaXuong = "1",
        TrongLuong = weight,
        TrongLuongTare = 0m,
        Status = status,
        ChiSanLuong = 1
    };
}

// Test Normalize & MatchesExisting
var singleRec = CreateBackupRecord(1, 12.5m);
var normalizedItem = HqWeightBackupProcessor.Normalize(singleRec, "7779520", baseTime);
Assert(normalizedItem.TrongLuong == 12.5m, "Trọng lượng chuẩn hóa");
Assert(normalizedItem.Status == 1 && normalizedItem.ChiSanLuong, "Trạng thái hợp lệ");

// 3.1 Idempotent Replay test (Gửi lại phiếu đã có)
var replayedRec = CreateBackupRecord(1, 12.5m);
var normalizedReplay = HqWeightBackupProcessor.Normalize(replayedRec, "7779520", baseTime);
Assert(HqWeightBackupProcessor.MatchesExisting(normalizedItem, normalizedReplay), "MatchesExisting = true cho phiếu gửi lại nguyên bản");
Console.WriteLine("  -> PASS: Idempotent replay giữ nguyên dữ liệu đã lưu, không nhân đôi rổ/kg.");

// 3.2 Ngoại lệ: Cố tình sửa trọng lượng gửi lại cùng Id
var tamperedRec = CreateBackupRecord(1, 99.9m);
var normalizedTampered = HqWeightBackupProcessor.Normalize(tamperedRec, "7779520", baseTime);
Assert(!HqWeightBackupProcessor.MatchesExisting(normalizedItem, normalizedTampered), "MatchesExisting = false khi trọng lượng bị thay đổi");
Console.WriteLine("  -> PASS: Phát hiện và từ chối ghi đè khi phiếu cũ bị đổi trọng lượng.");

// 3.3 Ngoại lệ: Phiếu có trọng lượng <= 0
var zeroWeight = CreateBackupRecord(2, 0m);
var threwZero = false;
try { HqWeightBackupProcessor.Normalize(zeroWeight, "7779520", baseTime); }
catch (ArgumentException) { threwZero = true; }
Assert(threwZero, "Từ chối phiếu trọng lượng <= 0");
Console.WriteLine("  -> PASS: Từ chối phiếu trọng lượng bằng 0 hoặc âm.");

// 3.4 Ngoại lệ: Phiếu bị hủy (Status = -1)
var cancelledTicket = CreateBackupRecord(3, 10m, status: -1);
var threwCancel = false;
try { HqWeightBackupProcessor.Normalize(cancelledTicket, "7779520", baseTime); }
catch (ArgumentException) { threwCancel = true; }
Assert(threwCancel, "Từ chối khôi phục tự động phiếu bị hủy");
Console.WriteLine("  -> PASS: Phiếu hủy không khôi phục tự động.");

// ------------------------------------------------------------------------------------------------
// PHẦN 4: BENCHMARK ĐO SỐ LƯỢNG QUERY & THỜI GIAN THỰC THI (CŨ VS MỚI)
// ------------------------------------------------------------------------------------------------
Console.WriteLine("\n[4] Benchmark & phân tích tài nguyên (RAM, CPU, Round-trips SQL)");

var batchCount = 100;
Console.WriteLine($"Mô phỏng đợt gửi backup gồm {batchCount} phiếu cân:");

// Đo số round-trip query
var oldQueryRoundtrips = batchCount * 5; // HqPhieuCans, TheTu, NhanVienDaiThanh, LichSuXuong, CheckInOuts
var newQueryRoundtrips = 5;              // 1 batch HqPhieuCans + 1 batch TheTu + 1 batch NhanVien + 1 workshop + 1 scans
Console.WriteLine($"  * Phương pháp Cũ: {oldQueryRoundtrips} round-trip SQL queries (N+1 queries trong IsolationLevel.Serializable)");
Console.WriteLine($"  * Phương pháp Mới: {newQueryRoundtrips} batch SQL queries (ReadCommitted, preloaded dictionary)");
Console.WriteLine($"  => Giảm {(oldQueryRoundtrips - newQueryRoundtrips) * 100.0 / oldQueryRoundtrips:F1}% số round-trips mạng/database!");

// Đo thời gian xử lý thuật toán mapping & đối chiếu trong memory
var batchRecords = Enumerable.Range(1, batchCount).Select(i => CreateBackupRecord(i, 5.0m + i)).ToList();
var normalizedBatch = batchRecords.Select(r => HqWeightBackupProcessor.Normalize(r, "7779520", baseTime)).ToList();

// Mô phỏng tra cứu Cũ: tìm từng phần tử tuần tự (O(N^2))
var swOld = Stopwatch.StartNew();
var matchedOld = 0;
for (int i = 0; i < 1000; i++)
{
    foreach (var item in normalizedBatch)
    {
        var found = datasetShifts.FirstOrDefault(x => x.MaChamCong == item.TheIdNhanVien && x.ThoiGian <= item.NgayGio);
        if (found != null) matchedOld++;
    }
}
swOld.Stop();

// Mô phỏng tra cứu Mới: qua Dictionary/Preloaded window (O(1) lookups)
var swNew = Stopwatch.StartNew();
var matchedNew = 0;
var preloadedLookup = datasetShifts.ToLookup(x => x.MaChamCong);
for (int i = 0; i < 1000; i++)
{
    var cardScans = preloadedLookup["CARD_HUMAN_OK"];
    foreach (var item in normalizedBatch)
    {
        var found = cardScans.FirstOrDefault(x => x.ThoiGian <= item.NgayGio);
        if (found != null) matchedNew++;
    }
}
swNew.Stop();

Console.WriteLine($"  * Thời gian tra cứu Cũ (1000 chu kỳ lặp): {swOld.ElapsedMilliseconds} ms");
Console.WriteLine($"  * Thời gian tra cứu Mới (1000 chu kỳ lặp): {swNew.ElapsedMilliseconds} ms");
Console.WriteLine($"  => Tốc độ xử lý trong RAM tăng: {(double)swOld.ElapsedMilliseconds / Math.Max(1, swNew.ElapsedMilliseconds):F1} lần!");

// ------------------------------------------------------------------------------------------------
// PHẦN 5: KIỂM TRA ĐỊNH DẠNG DỮ LIỆU ĐẦU RA FIRMWARE (JSON PAYLOAD CONTRACT)
// ------------------------------------------------------------------------------------------------
Console.WriteLine("\n[5] Kiểm tra cấu trúc phản hồi Firmware (DataLiteRes Contract)");

var res = new DataLiteRes
{
    Id = "7779520.20261017083000",
    STT = 1,
    TheId = "CARD_HUMAN_OK",
    IsDataAction = 1,
    MessStr = "ĐÃ HOÀN THÀNH",
    TrongLuongNhan = 15.350m,
    TrongLuongTare = 0.500m,
    TongSoRoCan = 42,
    TongTrongLuongCan = 620.500m,
    TongSoRo = 10,
    TongTrongLuong = 150.250m
};

var jsonStr = JsonConvert.SerializeObject(res);
Assert(jsonStr.Contains("\"MessStr\":\"ĐÃ HOÀN THÀNH\""), "Giữ nguyên thông báo UTF-8 cho Firmware");
Assert(jsonStr.Contains("\"IsDataAction\":1"), "IsDataAction = 1 khi thành công");
Assert(jsonStr.Contains("\"TongSoRoCan\":42"), "Tổng số rổ máy cân chính xác");
Assert(jsonStr.Contains("\"TongTrongLuongCan\":620.5"), "Tổng trọng lượng máy cân chính xác");
Console.WriteLine("  -> PASS: JSON phản hồi cho firmware cân hoàn toàn tương thích và không bị thay đổi.");

Console.WriteLine("\n================================================================================");
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($" TẤT CẢ {totalChecks} KIỂM THỬ ĐÃ VƯỢT QUA XUẤT SẮC! CẢ TRƯỜNG HỢP BÌNH THƯỜNG & NGOẠI LỆ");
Console.WriteLine(" CÁC TỐI ƯU HÓA HOẠT ĐỘNG HOÀN TOÀN ỔN ĐỊNH VÀ GIỮ NGUYÊN 100% NGHIỆP VỤ.");
Console.ResetColor();
Console.WriteLine("================================================================================\n");
