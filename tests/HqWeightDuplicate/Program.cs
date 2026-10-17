using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Models.Repos;
using Models.Repos.Models;
using Services;

Dapper.SqlMapper.AddTypeHandler(new Models.Repos.Handler.DateOnlyTypeHandler());
var checks = 0;
void Assert(bool ok, string message)
{
    if (!ok) throw new Exception(message);
    checks++;
}

var time = new DateTime(2026, 10, 17, 23, 59, 50);
HQ_PhieuCan Ticket(string id, double seconds = 0) => new()
{
    Id = id, STT = 0, NgayGio = time.AddSeconds(seconds), Ngay = DateOnly.FromDateTime(time.AddSeconds(seconds)),
    MayCan = "101", MaLo = "L01", MaSize = "S01", MaThanhPham = "TP01", MaLoaiNguyenLieu = "NL01",
    MaNhanVien = "NV01", MaNhanVienPhucVu = "", MaNhanVienBanKiem = "", MaXuong = "X01",
    TrongLuong = 7.3m, TrongLuongTare = 0.5m, TheId = "CARD01", TheIdNhanVien = "CARD01",
    ChiSanLuong = true, Status = 1, GhiChu = ""
};
var saved = Ticket("saved");
bool Duplicate(HQ_PhieuCan candidate) => HqWeightDuplicatePolicy.FindDuplicates(new[] { saved }.AsQueryable(), candidate).Any();
// Tái hiện dao động nhỏ của cùng máy/thẻ; ghi nhận hành vi hiện tại, chưa đổi nghiệp vụ.
var shortIntervalCases = (from seconds in new[] { 1, 2 }
    from delta in new[] { 0m, -0.2m, -0.1m, 0.1m, 0.2m }
    select (Seconds: seconds, Delta: delta)).ToArray();
foreach (var test in shortIntervalCases)
{
    var candidate = Ticket("short-interval", test.Seconds);
    candidate.TrongLuong += test.Delta;
    Assert(Duplicate(candidate) == (test.Delta == 0),
        $"Tái hiện hiện trạng: {test.Seconds}s, lệch {test.Delta} kg");
    Console.WriteLine($"MEMORY: {test.Seconds}s; {saved.TrongLuong} -> {candidate.TrongLuong} kg; "
        + (Duplicate(candidate) ? "BLOCKED" : "NOT BLOCKED"));
}
foreach (var seconds in new[] { -29.999, -1, 0, 1, 29.999 })
    Assert(Duplicate(Ticket("new", seconds)), $"Chặn trong 30 giây, kể cả gửi ngược thứ tự: {seconds}");
foreach (var seconds in new[] { -31.0, -30, 30, 31 })
    Assert(!Duplicate(Ticket("new", seconds)), $"Cho phép cách ít nhất 30 giây: {seconds}");
Assert(!Duplicate(Ticket("saved", 1)), "Gửi lại cùng Id đi qua luồng xác nhận phiếu đã có");
foreach (var change in new (string Name, Action<HQ_PhieuCan> Apply)[] {
    ("STT", x => x.STT++), ("MayCan", x => x.MayCan = "102"),
    ("MaLo", x => x.MaLo = "L02"), ("MaSize", x => x.MaSize = "S02"),
    ("MaThanhPham", x => x.MaThanhPham = "TP02"), ("MaLoaiNguyenLieu", x => x.MaLoaiNguyenLieu = "NL02"),
    ("MaNhanVien", x => x.MaNhanVien = "NV02"), ("MaNhanVienPhucVu", x => x.MaNhanVienPhucVu = "PV01"),
    ("MaNhanVienBanKiem", x => x.MaNhanVienBanKiem = "BK01"), ("MaXuong", x => x.MaXuong = "X02"),
    ("TrongLuong", x => x.TrongLuong += 0.001m), ("TrongLuongTare", x => x.TrongLuongTare += 0.001m),
    ("TheId", x => x.TheId = "CARD02"), ("TheIdNhanVien", x => x.TheIdNhanVien = "CARD02"),
    ("ChiSanLuong", x => x.ChiSanLuong = false), ("Status", x => x.Status = 0),
    ("GhiChu", x => x.GhiChu = "Ghi chú nghiệp vụ khác") })
{
    var candidate = Ticket("changed", 1);
    change.Apply(candidate);
    Assert(!Duplicate(candidate), "Cho phép dữ liệu khác: " + change.Name);
}
var backup = Ticket("backup", 1);
backup.GhiChu = "[HQ_BACKUP] 2026-10-18 07:00:00";
Assert(Duplicate(backup), "Thời gian khôi phục không làm phiếu backup khác phiếu online");
saved.GhiChu = "[HQ_BACKUP] 2026-10-18 06:00:00";
Assert(Duplicate(backup), "Hai đợt backup khác giờ vẫn kiểm tra trùng");
Assert(Duplicate(Ticket("online", 1)), "Online cũng phát hiện phiếu backup đã lưu");
saved.GhiChu = null;
var empty = Ticket("empty", 1);
empty.MaNhanVienPhucVu = null;
empty.MaNhanVienBanKiem = null;
Assert(Duplicate(empty), "NULL và chuỗi rỗng ở trường tùy chọn cùng nghĩa");

using var translation = new WeightDb("Server=(local);Database=TranslationOnly;Integrated Security=True;TrustServerCertificate=True");
var sql = HqWeightDuplicatePolicy.FindDuplicates(translation.Set<HQ_PhieuCan>(), backup).ToQueryString();
Assert(sql.Contains("WHERE") && sql.Contains("[NgayGio] >") && sql.Contains("[NgayGio] <"),
    "Điều kiện trùng và khoảng thời gian dịch thành SQL");

if (args.Contains("--sql-localdb"))
{
    // Chỉ tạo và dọn database kiểm thử có tên ngẫu nhiên; không đọc cấu hình PMS.
    var connection = @"Server=(localdb)\MSSQLLocalDB;Database=RCQ_HqDuplicateTest_" + Guid.NewGuid().ToString("N")
        + ";Integrated Security=True;TrustServerCertificate=True;Connect Timeout=30";
    using var schema = new WeightDb(connection);
    try
    {
        schema.Database.EnsureCreated();
        schema.Add(saved);
        schema.SaveChanges();
        foreach (var test in shortIntervalCases)
        {
            using var db = new TestDb(connection, schema.Model);
            using var transaction = db.Database.BeginTransaction();
            var candidate = Ticket("sql-short-interval", test.Seconds);
            candidate.TrongLuong += test.Delta;
            ScaleAttendanceTransferService.LockCard(db, candidate.TheIdNhanVien);
            var blocked = HqWeightDuplicatePolicy.HasDuplicate(db, candidate);
            Assert(blocked == (test.Delta == 0),
                $"SQL: {test.Seconds}s, lệch {test.Delta} kg");
            if (!blocked)
            {
                db.HqPhieuCans.Add(candidate);
                db.SaveChanges();
            }
            var tickets = db.HqPhieuCans.Where(x => x.Id == saved.Id || x.Id == candidate.Id);
            Assert(tickets.Count() == (blocked ? 1 : 2), "Đối chiếu số phiếu đã lưu trong SQL thử");
            Assert(tickets.Sum(x => x.TrongLuong) == (blocked ? saved.TrongLuong : saved.TrongLuong + candidate.TrongLuong),
                "Đối chiếu tổng trọng lượng của hai phiếu trong SQL thử");
            Console.WriteLine($"SQL: {test.Seconds}s; {saved.TrongLuong} -> {candidate.TrongLuong} kg; "
                + $"{(blocked ? "BLOCKED" : "INSERTED")}; Count={tickets.Count()}; Total={tickets.Sum(x => x.TrongLuong)} kg");
            // Hoàn tác từng tình huống để không ảnh hưởng tình huống tiếp theo.
            transaction.Rollback();
        }
        using (var db = new TestDb(connection, schema.Model))
        {
            Assert(HqWeightDuplicatePolicy.HasDuplicate(db, backup), "SQL thực phát hiện phiếu trùng qua ngày");
            Assert(!HqWeightDuplicatePolicy.HasDuplicate(db, Ticket("boundary", 30)), "SQL thực cho phép đúng 30 giây");
            var pending = Ticket("pending", 3600);
            db.HqPhieuCans.Add(pending);
            Assert(HqWeightDuplicatePolicy.HasDuplicate(db, Ticket("pending-duplicate", 3601)),
                "Chặn trùng trong cùng lô trước SaveChanges");
        }
        using (var db = new TestDb(connection, schema.Model))
        {
            using var transaction = db.Database.BeginTransaction();
            ScaleAttendanceTransferService.LockCard(db, "CARD01");
            db.HqPhieuCans.Add(Ticket("batch", 3600));
            Assert(HqWeightDuplicatePolicy.HasDuplicate(db, Ticket("batch-duplicate", 3601)), "Lô trùng bị từ chối");
            // Dispose transaction không commit, giống lô backup bị từ chối.
        }
        Assert(!schema.Set<HQ_PhieuCan>().Any(x => x.Id == "batch"), "Lô lỗi không lưu một phần");

        bool InsertConcurrent(int index)
        {
            using var db = new TestDb(connection, schema.Model);
            using var transaction = db.Database.BeginTransaction();
            var candidate = Ticket("concurrent-" + index, 7200 + index);
            ScaleAttendanceTransferService.LockCard(db, candidate.TheIdNhanVien);
            if (HqWeightDuplicatePolicy.HasDuplicate(db, candidate)) return false;
            if (index % 2 == 0)
                new Dao.Repos.HQ.HQ_PhieuCan(connection).Insert(candidate); // Online: kết nối INSERT riêng.
            else
            {
                db.HqPhieuCans.Add(candidate); // Backup: INSERT trong transaction hiện tại.
                db.SaveChanges();
            }
            transaction.Commit();
            return true;
        }
        var outcomes = await Task.WhenAll(Enumerable.Range(0, 6).Select(i => Task.Run(() => InsertConcurrent(i))));
        Assert(outcomes.Count(x => x) == 1, "Sáu yêu cầu online/backup đồng thời chỉ thêm một phiếu");
        Assert(schema.Set<HQ_PhieuCan>().Count(x => x.Id.StartsWith("concurrent-")) == 1,
            "Đối chiếu SQL chỉ có một phiếu sau các yêu cầu đồng thời");
    }
    finally
    {
        schema.Database.EnsureDeleted();
    }
}
Console.WriteLine($"PASS: {checks} kiểm tra chống trùng HQ" + (args.Contains("--sql-localdb") ? ", gồm SQL LocalDB và yêu cầu đồng thời." : "."));

sealed class WeightDb(string connection) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer(connection);
    protected override void OnModelCreating(ModelBuilder model) => model.Entity<HQ_PhieuCan>();
}

sealed class TestDb(string connection, IModel model) : dbPMScontext(
    new DbContextOptionsBuilder<dbPMScontext>().UseSqlServer(connection).UseModel(model).Options)
{
    // Không gọi cấu hình database thật trong dbPMScontext.
    protected override void OnConfiguring(DbContextOptionsBuilder options) { }
}
