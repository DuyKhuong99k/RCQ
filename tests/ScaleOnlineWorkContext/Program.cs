using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Models.Repos;
using Models.Repos.Models;
using Services;
using System.Text.Json;

var count = 0;
void Assert(bool condition, string label)
{
    if (!condition) throw new Exception("FAIL: " + label);
    count++;
    Console.WriteLine("PASS: " + label);
}
var start = new DateTime(2026, 10, 18, 7, 0, 0);
var context = ScaleWorkContext.Create("LM", "XOAIDONG", "1");
var shift = new CheckInOut { Id = "101.20261018070000", MaChamCong = "CARD", MaSoMay = 101,
    TenMay = "A", ThoiGian = start, TrangThai = "Vào", MaXuong = "1" };
ScaleAttendanceScan Initial(CheckInOut root, ScaleWorkContext c) => new()
{
    Id = root.Id, VaoId = root.Id, MaSoMay = root.MaSoMay, ThoiGian = root.ThoiGian,
    TrangThai = "Vào", MaThanhPham = c.Product, MaLoaiNguyenLieu = c.Material, MaXuong = c.Workshop
};
var initial = Initial(shift, context);
string? Can(ScaleWorkContext c, DateTime? time = null, CheckInOut? root = null,
    ScaleAttendanceScan? snapshot = null) => ScaleProductionAttendancePolicy.ValidateOnline(
        new[] { root ?? shift }, new[] { snapshot ?? initial }, c, time ?? start.AddHours(1), 16);
Assert(Can(context) == null, "Đã Vào A được ghi ở B/C cùng bộ mã, không cần tham gia từng máy");
Assert(Can(ScaleWorkContext.Create("BM", "XOAIDONG", "1")) != null, "Đổi thành phẩm phải Vào ca mới");
Assert(Can(ScaleWorkContext.Create("LM", "XOAITUOI", "1")) != null, "Chỉ đổi nguyên liệu cũng phải Vào ca mới");
Assert(Can(ScaleWorkContext.Create("LM", "XOAIDONG", "2")) != null, "Khác xưởng phải Vào ca mới");
Assert(Can(ScaleWorkContext.Create("BM", "XOAIDONG", "1")) == ScaleOnlineMessages.DifferentProduct,
    "Màn cân báo ngắn đúng lỗi khác thành phẩm");
Assert(Can(ScaleWorkContext.Create("LM", "XOAITUOI", "1")) == ScaleOnlineMessages.DifferentMaterial,
    "Màn cân báo ngắn đúng lỗi khác nguyên liệu");
Assert(Can(ScaleWorkContext.Create("LM", "XOAIDONG", "2")) == ScaleOnlineMessages.DifferentWorkshop,
    "Màn cân báo ngắn đúng lỗi khác xưởng");
Assert(Can(ScaleWorkContext.Create("", "XOAIDONG", "1")) == ScaleOnlineMessages.MissingProduct,
    "Màn cân báo đúng trường thiếu thành phẩm");
Assert(Can(ScaleWorkContext.Create("LM", "", "1")) == ScaleOnlineMessages.MissingMaterial,
    "Màn cân báo đúng trường thiếu nguyên liệu");
Assert(Can(ScaleWorkContext.Create("LM", "XOAIDONG", "")) == ScaleOnlineMessages.MissingWorkshop,
    "Màn cân báo đúng trường thiếu xưởng");
Assert(Can(ScaleWorkContext.Create(" lm ", " xoaidong ", " 1 ")) == null, "Chuẩn hóa khoảng trắng/hoa thường, không dựa tên");
Assert(!ScaleWorkContext.Create("001", "XOAIDONG", "1").Matches(ScaleWorkContext.Create("002", "XOAIDONG", "1")),
    "Hai mã 001/002 cùng tên vẫn khác ca");
Assert(!ScaleWorkContext.Create("001", "XOAIDONG", "1").Matches(ScaleWorkContext.Create("1", "XOAIDONG", "1")), "Giữ số 0 đầu mã");
Assert(ScaleSharedAttendancePolicy.SelectContextAction(context, context) == ScaleSharedAttendancePolicy.CheckInAction.Repeat,
    "Vào lại cùng bộ mã không đổi giờ bắt đầu");
foreach (var c in new[] { ScaleWorkContext.Create("BM", "XOAIDONG", "1"),
    ScaleWorkContext.Create("LM", "XOAITUOI", "1"), ScaleWorkContext.Create("LM", "XOAIDONG", "2") })
    Assert(ScaleSharedAttendancePolicy.SelectContextAction(context, c) == ScaleSharedAttendancePolicy.CheckInAction.Switch,
        "Thay một mã đóng/mở ca: " + c);
Assert(Can(ScaleWorkContext.Create("", "XOAIDONG", "1")) != null, "Thiếu mã không được cân");
Assert(Can(context, start.AddTicks(-1)) != null, "Không cân trước giờ Vào");
Assert(Can(context, start.AddDays(1)) != null, "Ca thường hôm qua không cho cân hôm nay");
var night = new CheckInOut { Id = "night", MaChamCong = "CARD", MaSoMay = 101, TenMay = "A",
    TrangThai = "Vào", ThoiGian = start.Date.AddHours(22), CaDem = true };
Assert(Can(context, night.ThoiGian.AddHours(8), night, Initial(night, context)) == null, "Ca đêm 22h -> 6h vẫn được cân");
Assert(Can(context, night.ThoiGian.AddHours(16), night, Initial(night, context)) == null, "Ca đêm đúng 16h còn hạn");
Assert(Can(context, night.ThoiGian.AddHours(16).AddSeconds(1), night, Initial(night, context)) != null, "Không kéo dài ca đêm quá hạn");
var legacy = new ScaleAttendanceScan { Id = shift.Id, VaoId = shift.Id, MaThanhPham = "LM" };
Assert(Can(context, snapshot: legacy) == ScaleOnlineAttendanceService.MissingSnapshot, "Không đoán nguyên liệu/xưởng của ca cũ");
Assert(ScaleOnlineAttendanceService.MissingSnapshot == "Ca cũ thiếu mã. Quẹt Ra rồi Vào", "Ca cũ báo ngắn và chỉ rõ Ra rồi Vào");
shift.Isforget = 1;
Assert(Can(context) != null, "Không bỏ qua ca bị đánh dấu lỗi");
shift.Isforget = 0;
Assert(ScaleProductionAttendancePolicy.ValidateOnline(Array.Empty<CheckInOut>(), new[] { initial }, context, start, 16) != null,
    "Không có Vào không được ghi sản lượng nhân viên");
Assert(ScaleProductionAttendancePolicy.ValidateOnline(new[] { shift, shift }, new[] { initial }, context, start, 16) != null,
    "Không tự gộp nhiều ca mở");

HQ_PhieuCan Weight(string id, string machine, DateTime time, ScaleWorkContext? c = null) => new()
{
    Id = id, STT = 0, MayCan = machine, NgayGio = time, Ngay = DateOnly.FromDateTime(time),
    MaLo = "lo1", MaSize = "00X00", MaThanhPham = (c ?? context).Product,
    MaLoaiNguyenLieu = (c ?? context).Material, MaXuong = (c ?? context).Workshop,
    TheId = "CARD", TheIdNhanVien = "CARD", MaNhanVien = "0169", TrongLuong = 7.760m,
    TrongLuongTare = .710m, ChiSanLuong = true, Status = 1
};
var good = new[] { Weight("a", "101", start.AddMinutes(1)), Weight("b", "102", start.AddMinutes(2)),
    Weight("c", "103", start.AddMinutes(3)) };
var wrongCard = Weight("other-card", "102", start.AddMinutes(5)); wrongCard.TheIdNhanVien = "OTHER"; wrongCard.MaNhanVien = "OTHER_NV";
var canceled = Weight("canceled", "102", start.AddMinutes(5)); canceled.Status = 0;
var weights = good.Concat(new[] { Weight("wrong-product", "101", start.AddMinutes(5), ScaleWorkContext.Create("BM", "XOAIDONG", "1")),
    Weight("wrong-material", "101", start.AddMinutes(5), ScaleWorkContext.Create("LM", "XOAITUOI", "1")),
    Weight("wrong-workshop", "102", start.AddMinutes(5), ScaleWorkContext.Create("LM", "XOAIDONG", "2")),
    wrongCard, canceled, Weight("before", "101", start.AddTicks(-1)) }).AsQueryable();
var rootEvents = new[] { shift }.AsQueryable();
var selected = ScaleSharedAttendancePolicy.ShiftWeights(weights, new[] { initial }.AsQueryable(), shift, start.AddHours(1), rootEvents).ToArray();
Assert(selected.Length == 3 && selected.Sum(x => x.TrongLuong) == 23.280m,
    "Tổng ca lấy đủ A/B/C, loại khác cặp mã/xưởng/thẻ, hủy và trước Vào");

foreach (var changed in new[] { ScaleWorkContext.Create("BM", "XOAIDONG", "1"),
    ScaleWorkContext.Create("LM", "XOAITUOI", "1"), ScaleWorkContext.Create("LM", "XOAIDONG", "2"), context })
{
    var next = new CheckInOut { Id = "102.next", MaChamCong = "CARD", MaSoMay = 102, TenMay = "B",
        ThoiGian = start.AddHours(2), TrangThai = "Vào", MaXuong = changed.Workshop };
    var checkout = new CheckInOut { Id = "auto", MaChamCong = "CARD", MaSoMay = 101, TenMay = "A",
        ThoiGian = next.ThoiGian, TrangThai = "Ra", VaoId = shift.Id };
    var scans = new[] { initial, Initial(next, changed) }.AsQueryable();
    var events = new[] { shift, checkout, next }.AsQueryable();
    var boundary = new[] { Weight("old", "103", next.ThoiGian), Weight("new", "104", next.ThoiGian, changed) }.AsQueryable();
    var oldPart = ScaleSharedAttendancePolicy.ShiftWeights(boundary, scans, shift, next.ThoiGian, events).ToArray();
    var newPart = ScaleSharedAttendancePolicy.ShiftWeights(boundary, scans, next, next.ThoiGian.AddMinutes(1), events).ToArray();
    if (changed.Matches(context))
        Assert(oldPart.Length == 0 && newPart.Length == 2, "Đóng/mở cùng mã cùng giây chỉ tính vào ca mới");
    else
        Assert(oldPart.Single().Id == "old" && newPart.Single().Id == "new", "Giây chuyển khác một mã, mỗi phiếu chỉ thuộc đúng một ca");
}

using var translation = new SchemaDb("Server=(local);Database=TranslationOnly;Integrated Security=True;TrustServerCertificate=True");
var sql = ScaleSharedAttendancePolicy.ShiftWeights(translation.Set<HQ_PhieuCan>(), translation.Set<ScaleAttendanceScan>(),
    shift, start.AddHours(2), translation.Set<CheckInOut>()).ToQueryString();
Assert(sql.Contains("MaLoaiNguyenLieu") && sql.Contains("MaXuong") && sql.Contains("UNION ALL"),
    "Tổng ca mới/cũ dịch thành SQL, không tải toàn bộ phiếu về RAM");

if (args.Contains("--sql-localdb"))
{
    // Database riêng có tên ngẫu nhiên; không đọc cấu hình PMS và không kết nối PMS_HQ.
    var database = "RCQ_OnlineContextTest_" + Guid.NewGuid().ToString("N");
    var connection = @"Server=(localdb)\MSSQLLocalDB;Database=" + database + ";Integrated Security=True;TrustServerCertificate=True";
    using var schema = new SchemaDb(connection);
    try
    {
        schema.Database.EnsureCreated();
        schema.AddRange(new HQ_ThanhPham { Id = "LM", Ten = "LẠNG MÁ", SuDung = true },
            new HQ_ThanhPham { Id = "BM", Ten = "BÀO MÁY", SuDung = true });
        schema.AddRange(new HQ_LoaiNguyenLieu { Id = "XOAIDONG", Ten = "XOÀI ĐÔNG", SuDung = true },
            new HQ_LoaiNguyenLieu { Id = "XOAITUOI", Ten = "XOÀI TƯƠI", SuDung = true });
        schema.SaveChanges();
        var employee = new NhanVienDaiThanh { MaNhanVien = "0169", Name = "TEST", IsHuman = true };
        // Không dùng giờ hệ thống để đổi các mốc kiểm thử bên dưới.
        (bool Success, string Json) Run(int machine, DateTime at, string status, ScaleWorkContext c,
            bool commit = true, string card = "CARD", bool nightShift = false)
        {
            using var db = new TestDb(connection, schema.Model);
            using var tx = db.Database.BeginTransaction();
            ScaleAttendanceTransferService.LockCard(db, card);
            ScaleSharedAttendanceService.LockMachines(db, new[] { machine, 101, 102, 103 });
            var result = ScaleOnlineAttendanceService.Process(db, new ScaleAttendanceProcessor.Request
            {
                TheId = card, ChamCongId = AttendanceRecordId.Create(machine.ToString(), at),
                NgayGio = at.ToString("yyyyMMddHHmmss"), TrangThai = status, Cadem = nightShift
            }, machine, "Machine " + machine, at, c, employee, c.Product == "BM" ? "BÀO MÁY" : "LẠNG MÁ", 16);
            if (result.Success && commit) tx.Commit();
            return result;
        }
        string? Code((bool Success, string Json) r) => JsonDocument.Parse(r.Json).RootElement.TryGetProperty("Code", out var value) ? value.GetString() : null;
        Assert(Run(101, start, "Vào", context).Success, "SQL: mở ca đầu lưu snapshot đủ bộ mã");
        Assert(Code(Run(102, start.AddMinutes(1), "Vào", context)) == "ALREADY_CHECKED_IN", "SQL: Vào B cùng mã giữ ca A");
        Assert(schema.Set<CheckInOut>().Count() == 1, "SQL: không tạo ca phụ khi Vào lặp ở máy khác");
        var replay = Run(101, start, "Vào", ScaleWorkContext.Create("BM", "XOAITUOI", "2"));
        Assert(replay.Success,
            "SQL: gửi lại cùng Id xác nhận ca gốc dù đã đổi cấu hình máy");
        Assert(JsonDocument.Parse(replay.Json).RootElement.GetProperty("TenCongViec").GetString() == "LẠNG MÁ",
            "SQL: xác nhận lại Id giữ tên công việc theo snapshot ca gốc, không theo cấu hình mới");
        Assert(Code(Run(101, start, "Vào", context, card: "OTHER")) == "ID_CONFLICT", "SQL: cùng Id khác thẻ không được ghi đè");
        Assert(Code(Run(102, start.AddTicks(-TimeSpan.TicksPerSecond), "Vào", context)) == "OUT_OF_ORDER", "SQL: từ chối lượt quẹt mới đảo thời gian");
        schema.AddRange(weights.ToArray());
        var alternateCard = Weight("alternate-card", "105", start.AddMinutes(4));
        alternateCard.TheId = alternateCard.TheIdNhanVien = "ALTERNATE";
        schema.Add(alternateCard);
        schema.SaveChanges();
        using (var db = new TestDb(connection, schema.Model))
        {
            var totals = ScaleOnlineAttendanceService.Totals(db, good[2], 16);
            var asyncTotals = await ScaleOnlineAttendanceService.TotalsAsync(db, good[2], 16);
            Assert(asyncTotals.MachineCount == totals.MachineCount && asyncTotals.MachineWeight == totals.MachineWeight &&
                asyncTotals.EmployeeCount == totals.EmployeeCount && asyncTotals.EmployeeWeight == totals.EmployeeWeight,
                "SQL: tổng async/cancellation giữ đúng số ngày/bộ mã như luồng cũ");
            Assert(totals.EmployeeCount == 5 && totals.EmployeeWeight == 38.800m,
                "SQL: tổng ngày theo cùng bộ mã + mã nhân viên, gom mọi máy/thẻ, không trừ bì lần hai");
            var actual = ScaleSharedAttendancePolicy.ShiftWeights(db.HqPhieuCans.AsNoTracking(), db.ScaleAttendanceScans.AsNoTracking(),
                shift, start.AddHours(1), db.CheckInOuts.AsNoTracking()).ToArray();
            Assert(actual.Length == 3 && actual.Sum(x => x.TrongLuong) == 23.280m,
                "SQL: tổng ca đúng ba phiếu của thẻ đã Vào ở ba máy, không lẫn bộ mã khác");
        }
        var material2 = ScaleWorkContext.Create("LM", "XOAITUOI", "1");
        Assert(Run(101, start.AddHours(1), "Vào", material2).Success, "SQL: cùng máy chỉ đổi nguyên liệu tự đóng/mở ca");
        var oldOut = schema.Set<CheckInOut>().AsNoTracking().Single(x => x.TrangThai == "Ra");
        Assert(oldOut.VaoId == shift.Id && oldOut.ThoiGian == start.AddHours(1) && oldOut.Id.Contains(".AUTO."),
            "SQL: Ra tự động đúng thời điểm, liên kết ca cũ, không chiếm Id lượt thật");
        Assert(Run(103, start.AddHours(2), "Vào", material2 with { Workshop = "2" }, commit: false).Success,
            "SQL: chuẩn bị đổi xưởng trong transaction");
        Assert(schema.Set<CheckInOut>().Count() == 3 && schema.Set<ScaleAttendanceScan>().Count() == 3,
            "SQL: rollback không để ca đóng/mở dở dang");
        Assert(Run(103, start.AddHours(2), "Vào", material2 with { Workshop = "2" }).Success,
            "SQL: khác xưởng dù cùng thành phẩm/nguyên liệu vẫn mở ca mới");
        schema.Add(Weight("workshop2", "106", start.AddHours(2).AddMinutes(1), material2 with { Workshop = "2" }));
        schema.SaveChanges();
        var outResult = Run(104, start.AddHours(3), "Ra", ScaleWorkContext.Create(null, null, null));
        Assert(outResult.Success,
            "SQL: Ra tại cân chưa từng Vào, không cần cấu hình thành phẩm tại cân Ra");
        var outJson = JsonDocument.Parse(outResult.Json).RootElement;
        Assert(outJson.GetProperty("TongSoRo").GetInt32() == 1 && outJson.GetProperty("TongTrongLuong").GetDecimal() == 7.760m,
            "SQL: phản hồi Ra tính sản lượng từ máy khác, số ký không bị trừ bì lần hai");
        var finalOut = schema.Set<CheckInOut>().AsNoTracking().Single(x => x.Id == AttendanceRecordId.Create("104", start.AddHours(3)));
        var actualOut = schema.Set<ScaleAttendanceScan>().AsNoTracking().Single(x => x.Id == finalOut.Id);
        Assert(finalOut.MaSoMay == 103 && actualOut.MaSoMay == 104 && finalOut.VaoId == "103.20261018090000",
            "SQL: giữ ghép báo cáo theo ca gốc và lưu máy Ra thực tế riêng");
        Assert(Code(Run(105, start.AddHours(3).AddSeconds(1), "Ra", context)) == "ALREADY_CHECKED_OUT",
            "SQL: Ra lặp ở bất kỳ cân không tạo thêm phiếu Ra");
        Assert(schema.Set<CheckInOut>().Count(x => x.TrangThai == "Vào") == 3 &&
            schema.Set<CheckInOut>().Count(x => x.TrangThai == "Ra") == 3,
            "SQL: ba ca, ba Ra; không có ca dư hoặc mất ca");
        using (var db = new TestDb(connection, schema.Model))
        {
            Assert(ScaleOnlineAttendanceService.OpenShifts(db, "CARD", start.AddHours(3).AddSeconds(1), 16).Count == 0,
                "SQL: Ra cân khác đóng đúng ca đang mở");
            Assert(ScaleSharedAttendancePolicy.ShiftWeights(db.HqPhieuCans.AsNoTracking(), db.ScaleAttendanceScans.AsNoTracking(),
                shift, start.AddHours(1), db.CheckInOuts.AsNoTracking()).Count() == 3, "SQL: đóng ca không làm thay đổi sản lượng lịch sử");
        }
        var parallel = await Task.WhenAll(Enumerable.Range(0, 4).Select(index => Task.Run(() =>
            Run(201 + index, start.AddHours(4), "Vào", context, card: "PARALLEL"))));
        Assert(parallel.Count(x => x.Success) == 1 && parallel.Count(x => Code(x) == "ALREADY_CHECKED_IN") == 3,
            "SQL: bốn lượt Vào đồng thời cùng nhân viên/bộ mã chỉ mở một ca");
        Assert(schema.Set<CheckInOut>().Count(x => x.MaChamCong == "PARALLEL" && x.TrangThai == "Vào") == 1,
            "SQL: đối chiếu sau đua yêu cầu không có ca trùng");
        Assert(Run(301, start, "Vào", context, card: "DAY").Success &&
            Run(302, start.AddDays(1), "Vào", context, card: "DAY").Success,
            "SQL: ngày mới được mở ca mà không lấy ca thường hôm qua");
        Assert(!schema.Set<CheckInOut>().Any(x => x.MaChamCong == "DAY" && x.TrangThai == "Ra"),
            "SQL: không tự bịa giờ Ra cho ca thường thiếu Ra hôm trước");
        var nightStart = start.Date.AddHours(22);
        Assert(Run(401, nightStart, "Vào", context, card: "NIGHT", nightShift: true).Success &&
            Code(Run(402, nightStart.AddHours(8), "Vào", context, card: "NIGHT")) == "ALREADY_CHECKED_IN",
            "SQL: Vào lại cân khác sau nửa đêm vẫn giữ ca đêm");
        var nightBefore = Weight("night-before", "401", nightStart.AddMinutes(30));
        var nightAfter = Weight("night-after", "402", nightStart.AddHours(7).AddMinutes(30));
        foreach (var w in new[] { nightBefore, nightAfter }) w.TheId = w.TheIdNhanVien = "NIGHT";
        schema.AddRange(nightBefore, nightAfter); schema.SaveChanges();
        using (var db = new TestDb(connection, schema.Model))
        {
            var nightTotals = ScaleOnlineAttendanceService.Totals(db, nightAfter, 16);
            var asyncNightTotals = await ScaleOnlineAttendanceService.TotalsAsync(db, nightAfter, 16);
            Assert(asyncNightTotals.MachineCount == nightTotals.MachineCount && asyncNightTotals.MachineWeight == nightTotals.MachineWeight &&
                asyncNightTotals.EmployeeCount == nightTotals.EmployeeCount && asyncNightTotals.EmployeeWeight == nightTotals.EmployeeWeight,
                "SQL: tổng async giữ đủ ca đêm trước/sau nửa đêm");
            Assert(nightTotals.EmployeeCount == 2 && nightTotals.EmployeeWeight == 15.520m,
                "SQL: tổng nhân viên ca đêm không mất phiếu trước nửa đêm");
        }
        Assert(Run(403, nightStart.AddHours(8), "Ra", ScaleWorkContext.Create(null,null,null), card: "NIGHT").Success,
            "SQL: ca đêm Ra ở cân bất kỳ ngày hôm sau");

        // Async policy must retain sync decisions without blocking worker threads.
        var asyncStart=ScaleWorkshopHistory.BusinessNow().AddSeconds(-10);
        var asyncRoot=new CheckInOut {Id="ASYNC_POLICY",MaChamCong="ASYNC_CARD",MaSoMay=101,TenMay="A",
            ThoiGian=asyncStart,TrangThai="Vào",CaDem=true,MaXuong="1"};
        schema.Add(asyncRoot);schema.Add(Initial(asyncRoot,context));schema.SaveChanges();
        var asyncEmployee=new NhanVienDaiThanh {MaNhanVien="ASYNC_NV",IsHuman=true};
        var asyncTime=asyncStart.AddSeconds(5);
        foreach(var asyncSelected in new[]{context,ScaleWorkContext.Create("BM","XOAIDONG","1"),
            ScaleWorkContext.Create("LM","XOAITUOI","1"),ScaleWorkContext.Create("LM","XOAIDONG","2")})
        {
            using var db=new TestDb(connection,schema.Model);
            var oldError=ScaleProductionAttendancePolicy.GetError(db,"ASYNC_CARD",101,asyncTime,16,asyncEmployee,
                asyncSelected.Product,asyncSelected.Material,asyncSelected.Workshop);
            var newError=await ScaleProductionAttendancePolicy.GetOnlineErrorAsync(db,"ASYNC_CARD",asyncTime,16,asyncEmployee,asyncSelected,CancellationToken.None);
            Assert(oldError==newError && (asyncSelected==context ? newError==null : newError!=null),
                "SQL: async/sync policy cùng quyết định bộ mã "+asyncSelected);
        }
        schema.Add(new ScaleAttendanceScan {Id="ASYNC_FUTURE_SCAN",VaoId=asyncRoot.Id,MaSoMay=102,ThoiGian=asyncTime.AddSeconds(1),TrangThai="Vào"});
        schema.SaveChanges();
        using(var db=new TestDb(connection,schema.Model))
        {
            var oldError=ScaleProductionAttendancePolicy.GetError(db,"ASYNC_CARD",101,asyncTime,16,asyncEmployee,context.Product,context.Material,context.Workshop);
            var newError=await ScaleProductionAttendancePolicy.GetOnlineErrorAsync(db,"ASYNC_CARD",asyncTime,16,asyncEmployee,context,CancellationToken.None);
            Assert(oldError==newError && newError!=null,"SQL: MAX gộp vẫn chặn giờ cân trước lượt scan cân khác");
        }

        var rootPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        var script = File.ReadAllText(Path.Combine(rootPath, "docs/sql/20261018-HqOnlineWorkContext.sql"));
        Assert(database.StartsWith("RCQ_OnlineContextTest_") && script.Contains("USE [PMS_HQ];"), "Chỉ chạy thử migration trong database fixture riêng");
        script = script.Replace("USE [PMS_HQ];", $"USE [{database}];");
        foreach (var pass in new[] { 1, 2 })
            foreach (var batch in System.Text.RegularExpressions.Regex.Split(script, @"(?im)^\s*GO\s*$"))
                if (!string.IsNullOrWhiteSpace(batch)) schema.Database.ExecuteSqlRaw(batch);
        Assert(schema.Set<ScaleAttendanceScan>().Any(x => x.MaLoaiNguyenLieu == "XOAITUOI" && x.MaXuong == "2"),
            "SQL: script nâng cấp chạy hai lần không mất snapshot hoặc dữ liệu");
    }
    finally { schema.Database.EnsureDeleted(); }
}
Console.WriteLine($"PASS: {count} kiểm tra ca theo bộ mã" + (args.Contains("--sql-localdb") ? ", gồm SQL LocalDB." : ". Không kết nối database."));

sealed class SchemaDb(string connection) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder) => builder.UseSqlServer(connection);
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<CheckInOut>(); model.Entity<ScaleAttendanceScan>(); model.Entity<HQ_PhieuCan>();
        model.Entity<HQ_ThanhPham>(); model.Entity<HQ_LoaiNguyenLieu>();
    }
}
sealed class TestDb(string connection, IModel model) : dbPMScontext(
    new DbContextOptionsBuilder<dbPMScontext>().UseSqlServer(connection).UseModel(model).Options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder options) { }
}
