using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using Models.Repos;
using Models.Repos.Models;
using Services;
using Vars.Hubs;
using Newtonsoft.Json;
using OnlineTotals = Dao.Repos.HQ.HQ_PhieuCan.OnlineTotals;

var count = 0;
void Check(bool value, string name)
{
    if (!value) throw new Exception("FAIL: " + name);
    count++;
    Console.WriteLine("PASS: " + name);
}
foreach (var field in typeof(ScaleOnlineMessages).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
    .Where(x => x.IsLiteral && x.FieldType == typeof(string)))
{
    var message = (string)field.GetRawConstantValue()!;
    Check(message.Length <= 40 && System.Text.Encoding.UTF8.GetByteCount(message) <= 64 &&
        !message.Contains('\n') && !message.Contains('"'), "Thông báo HMI ngắn, không ký tự phá lệnh: " + field.Name);
}
Check(ScaleOnlineMessages.InvalidTime == "Thời gian phiếu cân không hợp lệ." &&
    ScaleOnlineMessages.InvalidId == "Thiếu hoặc sai Id phiếu cân." &&
    ScaleOnlineMessages.InvalidTare == "Trọng lượng tare không được âm.", "Giữ nguyên thông báo kiểm tra phiếu đã có trước");
DataLite Request(string id = "101.20261021070000", string? state = "ST", string card = "CARD",
    decimal weight = 8m, decimal tare = .710m, string time = "20261021070000") => new()
    { Id = id, Stated = state!, TheId = card, TrongLuong = weight, TrongLuongTare = tare, NgayGio = time };
HQ_PhieuCan Ticket(DataLite r, string machine = "101") => new()
{
    Id = r.Id, MayCan = machine, TheId = r.TheId, TheIdNhanVien = r.TheId, MaNhanVien = "0169",
    NgayGio = DateTime.ParseExact(r.NgayGio, "yyyyMMddHHmmss", null), Ngay = new DateOnly(2026,10,21),
    MaLo = "lo1", MaSize = "00X00", MaThanhPham = "LM", MaLoaiNguyenLieu = "XOAIDONG",
    MaXuong = "1", TrongLuong = r.TrongLuong, TrongLuongTare = r.TrongLuongTare, Status = 1, ChiSanLuong = true
};
foreach (var state in new string?[] { "US", null, "", "UNKNOWN", "st" })
    Check(HqOnlineWeightWriter.RequestError(Request(state: state)) == HqOnlineWeightWriter.UnstableError,
        "Không nhận trạng thái " + (state ?? "null"));
foreach (var w in new[] { 0m, -.01m })
    Check(HqOnlineWeightWriter.RequestError(Request(weight: w)) != null, "Không nhận số ký " + w);
Check(HqOnlineWeightWriter.RequestError(Request(weight: -.8m, tare: .8m)) == ScaleOnlineMessages.NegativeWeight,
    "Writer từ chối số kg âm bằng lý do chính xác, không gọi là US");
Check(HqOnlineWeightWriter.RequestError(Request(weight: 0m)) == ScaleOnlineMessages.ZeroWeight,
    "Writer từ chối số kg bằng 0 bằng lý do chính xác");
Check(HqOnlineWeightWriter.IdentityRequestError(Request(weight: -.8m, tare: .8m)) == null,
    "ST âm do tare hợp lệ cho nhận diện, không phải cho INSERT");
Check(HqOnlineWeightWriter.IdentityRequestError(Request(weight: -.8m, state: "US")) == ScaleOnlineMessages.Unstable,
    "US vẫn không được dùng để nhận diện kể cả có số kg âm");
Check(HqOnlineWeightWriter.RequestError(Request()) == null, "ST hợp lệ được đi tiếp");
Check(HqOnlineWeightWriter.RequestError(Request(weight:.4m,tare:.8m)) == null,
    "Online nhận kg net nhỏ hơn bì, không tự trừ bì lần hai");
foreach (var invalidWeight in new[] {1000000000000000m, decimal.MaxValue})
{
    Check(HqOnlineWeightWriter.RequestError(Request(weight:invalidWeight)) == ScaleOnlineMessages.InvalidWeightRange,
        "Chặn kg vượt decimal(18,3) trước khi mở SQL");
    Check(HqOnlineWeightWriter.RequestError(Request(tare:invalidWeight)) == ScaleOnlineMessages.InvalidWeightRange,
        "Chặn bì vượt decimal(18,3) trước khi mở SQL");
}
Check(HqOnlineWeightWriter.IdentityRequestError(Request(weight:decimal.MinValue)) == ScaleOnlineMessages.InvalidWeightRange,
    "Nhận diện kg âm cực lớn không gây tràn số");
Check(HqOnlineWeightWriter.RequestError(Request(weight:999999999999999.999m,tare:999999999999999.999m)) == null,
    "Giữ chính xác biên tối đa decimal(18,3), không làm tròn dữ liệu");
Check(HqOnlineWeightWriter.RequestError(Request(tare: -.01m)) != null, "Không nhận tare âm");
Check(HqOnlineWeightWriter.RequestError(Request(weight: 8.0001m)) != null, "Không âm thầm làm tròn ký");
Check(HqOnlineWeightWriter.RequestError(Request(time: "BAD")) != null, "Từ chối giờ sai");
Check(HqOnlineWeightWriter.RequestError(Request(card: "")) != null, "Từ chối thẻ trống");
Check(HqOnlineWeightWriter.RequestError(Request(id: "")) != null, "Từ chối Id trống");
var original = Ticket(Request());
Check(HqOnlineWeightWriter.MatchesRequest(original, Request(), "101"), "Cùng Id/nội dung được xác nhận lại");
foreach (var changed in new[] { Request(card: "OTHER"), Request(weight: 8.01m), Request(tare: .72m),
    Request(time: "20261021070001"), Request(id: "other") })
    Check(!HqOnlineWeightWriter.MatchesRequest(original, changed, "101"), "Phát hiện xung đột request " + changed.TheId + "/" + changed.NgayGio + "/" + changed.TrongLuong + "/" + changed.TrongLuongTare + "/" + changed.Id);
Check(!HqOnlineWeightWriter.MatchesRequest(original, Request(), "102"), "Cùng Id khác máy bị chặn");
original.Status = 0;
Check(!HqOnlineWeightWriter.MatchesRequest(original, Request(), "101"), "Không xác nhận nhầm phiếu đã hủy");
original.Status = 1;
original.MaLo = "lô đã lưu"; original.MaThanhPham = "BM";
Check(HqOnlineWeightWriter.MatchesRequest(original, Request(), "101"), "Replay không suy đoán lại mã chọn từ cấu hình máy hiện tại");

// A failed diagnostic sink must not interrupt totals retry or turn COMMIT into UNKNOWN.
var consoleOutput = Console.Out;
var diagnosticAttempts = 0;
Tuple<bool, string> diagnosticRecovered;
Tuple<bool, string> diagnosticExhausted;
try
{
    Console.SetOut(new UnavailableConsole());
    diagnosticRecovered = await HqOnlineResponse.ForSavedAsync(original, "LITE", _ =>
    {
        if (++diagnosticAttempts == 1) throw new IOException("Test-only totals failure");
        return Task.FromResult(new OnlineTotals { MachineCount = 1, MachineWeight = 8m, EmployeeCount = 1, EmployeeWeight = 8m });
    }, _ => throw new IOException("Test-only display failure"), retryDelay: TimeSpan.Zero);
    diagnosticExhausted = await HqOnlineResponse.ForSavedAsync(original, "LITE",
        _ => throw new IOException("Test-only persistent totals failure"), retryDelay: TimeSpan.Zero);
}
finally { Console.SetOut(consoleOutput); }
var diagnosticData = JsonConvert.DeserializeObject<DataLiteRes>(diagnosticRecovered.Item2)!;
Check(diagnosticRecovered.Item1 && diagnosticAttempts == 2 && diagnosticData.SaveStatus == HqOnlineResponse.Saved,
    "Lỗi đầu ra log/UI không ngắt retry tổng hoặc làm mất xác nhận SAVED");
Check(diagnosticData.TongSoRo == 1 && diagnosticData.TongTrongLuong == 8m,
    "Sau lỗi log vẫn trả đủ tổng đúng, không dùng số giả");
Check(!diagnosticExhausted.Item1 && JsonConvert.DeserializeObject<DataError>(diagnosticExhausted.Item2)!.SaveStatus == HqOnlineResponse.Saved,
    "Tổng lỗi dai dẳng cùng lỗi log vẫn giữ trạng thái đã lưu, không đổi thành UNKNOWN");

// Keep two calls pending to test the exact gate used by CommitHqLite.
var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var secondEntered = false;
var first = HqOnlineWeightWriter.ForMachine("GATE", async () => { entered.SetResult(); await release.Task; return 1; });
await entered.Task;
var second = HqOnlineWeightWriter.ForMachine("GATE", () => { secondEntered = true; return Task.FromResult(2); });
try
{
    Check(!secondEntered, "Cùng máy phải chờ lượt đang xử lý");
    Check(await HqOnlineWeightWriter.ForMachine("OTHER_GATE", () => Task.FromResult(3)) == 3,
        "Máy khác không bị khóa chéo");
}
finally { release.TrySetResult(); }
await Task.WhenAll(first, second);
Check(secondEntered, "Máy tiếp tục xử lý sau khi lượt trước hoàn tất");
try { await HqOnlineWeightWriter.ForMachine<int>("GATE", () => throw new InvalidOperationException("TEST")); }
catch (InvalidOperationException) { }
Check(await HqOnlineWeightWriter.ForMachine("GATE", () => Task.FromResult(true)), "Lỗi không làm kẹt khóa máy");

var queueEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var queueRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var queueOwner = HqOnlineWeightWriter.ForMachine("BOUNDED", async () => { queueEntered.SetResult(); await queueRelease.Task; return 1; });
await queueEntered.Task;
var timedOut = false; var queuedActionRan = false;
try
{
    try { await HqOnlineWeightWriter.ForMachine("BOUNDED", () => { queuedActionRan = true; return Task.FromResult(2); }, TimeSpan.FromMilliseconds(30)); }
    catch (HqOnlineWeightWriter.QueueTimeoutException) { timedOut = true; }
    Check(timedOut && !queuedActionRan && !queueOwner.IsCompleted, "Queue hết hạn không chạy INSERT hoặc giải phóng khóa của owner");
}
finally { queueRelease.TrySetResult(); }
await queueOwner;
Check(await HqOnlineWeightWriter.ForMachine("BOUNDED", () => Task.FromResult(true)), "Queue hết hạn không làm kẹt lượt sau");

if (args.Contains("--sql-localdb"))
{
    var database = "RCQ_OnlineWeightTest_" + Guid.NewGuid().ToString("N");
    var connection = @"Server=(localdb)\MSSQLLocalDB;Database=" + database + ";Integrated Security=True;TrustServerCertificate=True";
    using var schema = new SchemaDb(connection);
    try
    {
        schema.Database.EnsureCreated();
        foreach (var weight in new[] { -.80m, -.001m, 0m })
        {
            var counter = new CommandCounter();
            using var db = new MeasuredDb(connection, schema.Model, counter);
            var prepared = 0;
            var rejected = await HqOnlineWeightWriter.SaveAsync(db, Request(weight: weight, tare: .80m), "101",
                _ => { prepared++; throw new Exception("Non-positive weight must not prepare INSERT"); },
                (_, _) => throw new Exception("Non-positive weight must not validate/open attendance"));
            Check(rejected.Item == null && !rejected.Inserted && rejected.Error ==
                (weight < 0 ? ScaleOnlineMessages.NegativeWeight : ScaleOnlineMessages.ZeroWeight),
                "SQL writer blocks non-positive ticket with precise reason: " + weight);
            Check(prepared == 0 && counter.SyncCalls == 0 && counter.AsyncCalls == 0 &&
                !schema.Set<HQ_PhieuCan>().Any(), "No SQL command, totals, attendance or INSERT for non-positive weight: " + weight);
        }
        HqOnlineWeightWriter.Result Save(DataLite r, string machine = "101", Func<HQ_PhieuCan,string?>? validate = null)
        {
            using var db = new TestDb(connection, schema.Model);
            return HqOnlineWeightWriter.Save(db, r, machine, () => Ticket(r, machine), validate ?? (_ => null));
        }
        var inserted = Save(Request());
        Check(inserted.Error == null && inserted.Inserted, "SQL: INSERT đã COMMIT mới trả kết quả thành công");
        Check(schema.Set<HQ_PhieuCan>().AsNoTracking().Count() == 1, "SQL: kết nối khác thấy phiếu sau xác nhận");
        var autoReadAttempts = 0;
        var readContexts = new HashSet<Guid>();
        var serverRecovered = await HqOnlineResponse.ForSavedAsync(inserted.Item!, "LITE", async cancellation =>
        {
            using var db = new TestDb(connection, schema.Model);
            readContexts.Add(db.ContextId.InstanceId);
            autoReadAttempts++;
            if (autoReadAttempts < 3)
                await db.Database.ExecuteSqlRawAsync("THROW 51002,N'TRANSIENT_TOTALS_AFTER_COMMIT',1;", cancellation);
            var count = await db.HqPhieuCans.CountAsync(cancellation);
            var weight = await db.HqPhieuCans.SumAsync(x => x.TrongLuong, cancellation);
            return new OnlineTotals { MachineCount = count, MachineWeight = weight, EmployeeCount = count, EmployeeWeight = weight };
        }, retryDelay: TimeSpan.Zero);
        var serverData = JsonConvert.DeserializeObject<DataLiteRes>(serverRecovered.Item2)!;
        Check(serverRecovered.Item1 && autoReadAttempts == 3 && readContexts.Count == 3,
            "SQL: WSS tự đọc tổng lại bằng context mới sau hai lỗi SQL thật");
        Check(serverData.Id == inserted.Item!.Id && serverData.TongSoRoCan == 1 && serverData.TongTrongLuongCan == 8m &&
            schema.Set<HQ_PhieuCan>().AsNoTracking().Count() == 1,
            "SQL: không cần firmware gửi lại, tổng phục hồi đúng một rổ, phiếu gốc không đổi");
        var failedTotalsAttempts = 0;
        var totalsFailed = await HqOnlineResponse.ForSavedAsync(inserted.Item!, "LITE", cancellation =>
        {
            failedTotalsAttempts++;
            // A real SQL exception AFTER the ticket transaction was committed.
            schema.Database.ExecuteSqlRaw("THROW 51002,N'TEST_TOTALS_AFTER_COMMIT',1;");
            return Task.FromResult(new OnlineTotals());
        }, retryDelay: TimeSpan.Zero);
        var totalsError = JsonConvert.DeserializeObject<DataError>(totalsFailed.Item2)!;
        Check(!totalsFailed.Item1 && totalsError.SaveStatus == HqOnlineResponse.Saved && totalsError.Id == inserted.Item!.Id &&
            totalsError.ErrorString == ScaleOnlineMessages.SavedTotalsError, "SQL: lỗi tổng sau COMMIT báo SAVED, không báo lỗi ghi");
        Check(schema.Set<HQ_PhieuCan>().AsNoTracking().Count() == 1, "SQL: lỗi phản hồi tổng không rollback hoặc xóa phiếu đã COMMIT");
        Check(failedTotalsAttempts == 3 && !totalsError.ErrorString.Contains("gửi lại", StringComparison.OrdinalIgnoreCase),
            "SQL: WSS tự thử query tổng 3 lần, không yêu cầu firmware đổi Id/gửi phiếu");
        var retryAfterTotalsError = Save(Request());
        Check(!retryAfterTotalsError.Inserted && retryAfterTotalsError.Item != null, "SQL: retry sau lỗi tổng chỉ xác nhận cùng Id");
        var totalsRecovered = await HqOnlineResponse.ForSavedAsync(retryAfterTotalsError.Item!, "LITE", _ => Task.FromResult(new OnlineTotals
        {
            MachineCount = schema.Set<HQ_PhieuCan>().Count(), MachineWeight = schema.Set<HQ_PhieuCan>().Sum(x => x.TrongLuong),
            EmployeeCount = schema.Set<HQ_PhieuCan>().Count(x => x.MaNhanVien == "0169"),
            EmployeeWeight = schema.Set<HQ_PhieuCan>().Where(x => x.MaNhanVien == "0169").Sum(x => x.TrongLuong)
        }));
        var recoveredData = JsonConvert.DeserializeObject<DataLiteRes>(totalsRecovered.Item2)!;
        Check(totalsRecovered.Item1 && recoveredData.Id == inserted.Item!.Id && recoveredData.SaveStatus == HqOnlineResponse.Saved &&
            recoveredData.IsDataAction == 1 && recoveredData.TongSoRoCan == 1 && recoveredData.TongTrongLuongCan == 8m &&
            recoveredData.TongSoRo == 1 && recoveredData.TongTrongLuong == 8m, "SQL: tổng phục hồi trả đầy đủ ACK với số rổ/kg chính xác");
        Check(schema.Set<HQ_PhieuCan>().AsNoTracking().Count() == 1, "SQL: không INSERT/cộng thêm vì retry phản hồi");
        var lateWaiting = Request(id: "LATE_WAITING"); lateWaiting.IsWaiting = true;
        Check(Save(lateWaiting).Error == HqOnlineWeightWriter.WaitingError &&
            !schema.Set<HQ_PhieuCan>().Any(x => x.Id == "LATE_WAITING"), "SQL: ST chờ tự động cũ không được tạo phiếu mới");
        var waitingReplay = Request(); waitingReplay.IsWaiting = true;
        Check(Save(waitingReplay).Error == null && !Save(waitingReplay).Inserted,
            "SQL: lượt chờ gửi lại đúng Id đã lưu chỉ xác nhận, không cộng lại");
        using (var db = new TestDb(connection, schema.Model))
        {
            var replay = HqOnlineWeightWriter.Save(db, Request(), "101",
                () => throw new Exception("Replay không được tạo lại phiếu"), _ => throw new Exception("Replay không được kiểm lại ca hiện tại"));
            Check(replay.Error == null && !replay.Inserted && replay.Item!.Id == inserted.Item!.Id,
                "SQL: retry Id xác nhận phiếu gốc, không INSERT/cộng lại");
        }
        foreach (var changed in new[] { Request(card: "OTHER"), Request(weight: 8.01m), Request(tare: .72m), Request(time: "20261021070001") })
            Check(Save(changed).Error == HqOnlineWeightWriter.ConflictError, "SQL: không ghi đè Id có nội dung khác");
        Check(Save(Request(), "102").Error == HqOnlineWeightWriter.ConflictError, "SQL: xung đột Id xuyên máy");
        Check(Save(Request(id: "REJECT"), validate: _ => "Chưa Vào ca").Error == "Chưa Vào ca"
            && !schema.Set<HQ_PhieuCan>().Any(x => x.Id == "REJECT"), "SQL: ca không hợp lệ không có phiếu");
        using (var db = new TestDb(connection, schema.Model))
        {
            var result = HqOnlineWeightWriter.Save(db, Request(id: "US", state: "US"), "101",
                () => throw new Exception("US không được tạo phiếu"), _ => throw new Exception("US không được kiểm tra ca"));
            Check(result.Error == HqOnlineWeightWriter.UnstableError && !result.Inserted && result.Item == null,
                "SQL: US không tạo phiếu hoặc xác nhận thành công");
        }
        using (var db = new TestDb(connection, schema.Model))
        {
            var zero = HqOnlineWeightWriter.Save(db, Request(id: "ZERO", weight: 0), "101",
                () => throw new Exception("Nhận diện 0kg không được tạo phiếu"), _ => throw new Exception("Nhận diện không đổi ca"));
            Check(zero.Error != null && !zero.Inserted && zero.Item == null && !schema.Set<HQ_PhieuCan>().Any(x => x.Id == "ZERO"),
                "SQL: nhận diện ST/0 không làm writer cho phép ghi phiếu 0kg");
        }
        var usReplay = Save(Request(state: "US"));
        Check(usReplay.Error == HqOnlineWeightWriter.UnstableError && !usReplay.Inserted && usReplay.Item == null,
            "SQL: US cùng Id phiếu ST đã lưu vẫn trả từ chối, không xác nhận thành công");
        var usWaiting = Request(id: "US_WAITING", state: "US"); usWaiting.IsWaiting = true;
        Check(Save(usWaiting).Error == HqOnlineWeightWriter.UnstableError,
            "SQL: US lượt chờ cũ không được tự lưu");
        Check(schema.Set<HQ_PhieuCan>().AsNoTracking().Count() == 1 &&
            !schema.Set<HQ_PhieuCan>().Any(x => x.Id == "US" || x.Id == "US_WAITING") &&
            schema.Set<HQ_PhieuCan>().AsNoTracking().Single().TrongLuong == inserted.Item!.TrongLuong,
            "SQL: các lượt US không thêm phiếu hoặc thay đổi phiếu đã lưu");
        var concurrent = await Task.WhenAll(Enumerable.Range(0,8).Select(_ => Task.Run(() => Save(Request(id: "RETRY_RACE")))));
        Check(concurrent.All(x => x.Error == null) && concurrent.Count(x => x.Inserted) == 1,
            "SQL: tám request cùng Id đồng thời chỉ ghi một phiếu");
        Check(schema.Set<HQ_PhieuCan>().Count(x => x.Id == "RETRY_RACE") == 1, "SQL: một dòng sau đua request");
        var conflicting = await Task.WhenAll(new[] { Task.Run(() => Save(Request(id: "CROSS", card: "A"), "201")),
            Task.Run(() => Save(Request(id: "CROSS", card: "B"), "202")) });
        Check(conflicting.Count(x => x.Inserted) == 1 && conflicting.Count(x => x.Error == HqOnlineWeightWriter.ConflictError) == 1,
            "SQL: cùng Id khác máy/thẻ đồng thời chỉ một bên thành công");
        Check(Save(Request(id: "NEW_BASKET_1", time: "20261021070001")).Inserted &&
            Save(Request(id: "NEW_BASKET_2", time: "20261021070002")).Inserted &&
            Save(Request(id: "NEW_BASKET_3", time: "20261021070003", weight: 8.01m)).Inserted,
            "SQL: Id mới ký bằng/lệch 0,01 trong vài giây không bị cooldown 30 giây");

        using var held = new ManualResetEventSlim(false);
        using var unlock = new ManualResetEventSlim(false);
        var overlaps = 0;
        var a = Task.Run(() => Save(Request(id: "SERIAL_A", card: "A"), "303", _ =>
        { held.Set(); if (!unlock.Wait(TimeSpan.FromSeconds(10))) throw new TimeoutException(); return null; }));
        Check(held.Wait(TimeSpan.FromSeconds(10)), "SQL: lượt đầu đã giữ khóa máy");
        var b = Task.Run(() => Save(Request(id: "SERIAL_B", card: "B"), "303", _ =>
        { Interlocked.Increment(ref overlaps); return null; }));
        try
        {
            await Task.WhenAny(b, Task.Delay(150));
            Check(Volatile.Read(ref overlaps) == 0, "SQL: khác thẻ cùng máy không chen vào khi chưa COMMIT");
        }
        finally { unlock.Set(); }
        Check((await a).Inserted && (await b).Inserted, "SQL: hai lượt ST hợp lệ được ghi tuần tự");

        schema.Database.ExecuteSqlRaw("ALTER TABLE dbo.HQ_PhieuCan ADD CONSTRAINT CK_OnlineWeightTest CHECK (Id <> 'SQL_FAIL');");
        var failed = false;
        try { Save(Request(id: "SQL_FAIL")); } catch (DbUpdateException) { failed = true; }
        Check(failed && !schema.Set<HQ_PhieuCan>().Any(x => x.Id == "SQL_FAIL"),
            "SQL: lỗi ghi rollback, không trả kết quả thành công");
        Check(Save(Request(id: "AFTER_FAIL")).Inserted, "SQL: lỗi trước không làm kẹt khóa/card/transaction");
        var originalId = Request().Id;
        Check(schema.Set<HQ_PhieuCan>().AsNoTracking().Single(x => x.Id == originalId).TrongLuong == 8m,
            "SQL: mọi xung đột/retry không đổi phiếu gốc");

        var syncCounter = new CommandCounter();
        using (var db = new MeasuredDb(connection, schema.Model, syncCounter))
            HqOnlineWeightWriter.Save(db, Request(id:"PERF_SYNC"), "101", () => Ticket(Request(id:"PERF_SYNC")), _=>null);
        var asyncCounter = new CommandCounter();
        using (var db = new MeasuredDb(connection, schema.Model, asyncCounter))
        {
            var r=Request(id:"PERF_ASYNC");
            var result=await HqOnlineWeightWriter.SaveAsync(db,r,"101",
                _=>Task.FromResult(new HqOnlineWeightWriter.Result(Ticket(r),false,null)),(_,_)=>Task.FromResult<string?>(null));
            Check(result.Inserted && result.Error==null && schema.Set<HQ_PhieuCan>().Any(x=>x.Id==r.Id), "Async chỉ trả thành công sau COMMIT nhìn được ở kết nối khác");
        }
        Check(asyncCounter.SyncCalls==0 && asyncCounter.AsyncCalls>0, "Writer async không dùng command SQL đồng bộ");
        Check(asyncCounter.IdLookups==1 && asyncCounter.ConnectionIds.Count==1, "Chỉ một lookup Id trên một kết nối trong writer");
        Check(asyncCounter.AsyncCalls < syncCounter.SyncCalls, "Gộp khóa máy/Id giảm một round trip SQL so với writer sync");
        Console.WriteLine($"MEASURED_COMMANDS: Sync={syncCounter.SyncCalls}, Async={asyncCounter.AsyncCalls}; excludes transaction begin/commit, not plant latency benchmark.");
        using (var db = new TestDb(connection,schema.Model))
        {
            var replay=await HqOnlineWeightWriter.SaveAsync(db,Request(id:"PERF_ASYNC"),"101",
                _=>throw new Exception("Không được đọc cache/cấu hình cho replay"),(_,_)=>throw new Exception("Không kiểm tra ca lại cho replay"));
            Check(!replay.Inserted && replay.Error==null, "Async replay được xác nhận dù cache/cấu hình không còn");
        }
        using (var db = new TestDb(connection,schema.Model))
        {
            var conflict=await HqOnlineWeightWriter.SaveAsync(db,Request(id:"PERF_ASYNC",weight:9m),"101",
                _=>throw new Exception("Id conflict không được tạo phiếu"),(_,_)=>throw new Exception());
            Check(conflict.Error==HqOnlineWeightWriter.ConflictError, "Async giữ kiểm tra Id xung đột");
        }
        var asyncRaces=await Task.WhenAll(Enumerable.Range(0,8).Select(_=>Task.Run(async()=>
        {
            using var db=new TestDb(connection,schema.Model); var r=Request(id:"ASYNC_RACE");
            return await HqOnlineWeightWriter.SaveAsync(db,r,"101",
                _=>Task.FromResult(new HqOnlineWeightWriter.Result(Ticket(r),false,null)),(_,_)=>Task.FromResult<string?>(null));
        })));
        Check(asyncRaces.All(x=>x.Error==null) && asyncRaces.Count(x=>x.Inserted)==1 && schema.Set<HQ_PhieuCan>().Count(x=>x.Id=="ASYNC_RACE")==1,
            "Tám writer async đồng thời chỉ INSERT một phiếu");
        using (var db=new TestDb(connection,schema.Model))
        {
            var cancelled=false;
            try
            {
                await HqOnlineWeightWriter.SaveAsync(db,Request(id:"ASYNC_DEADLINE"),"101",async token=>
                {
                    await Task.Delay(Timeout.InfiniteTimeSpan,token); throw new Exception("Deadline phải hủy");
                },(_,_)=>Task.FromResult<string?>(null),TimeSpan.FromMilliseconds(100));
            }
            catch(OperationCanceledException) {cancelled=true;}
            Check(cancelled && !schema.Set<HQ_PhieuCan>().Any(x=>x.Id=="ASYNC_DEADLINE"), "Write deadline hủy trước INSERT và rollback");
        }
        using (var db=new TestDb(connection,schema.Model))
        {
            var r=Request(id:"ASYNC_AFTER_DEADLINE");
            var next=await HqOnlineWeightWriter.SaveAsync(db,r,"101",
                _=>Task.FromResult(new HqOnlineWeightWriter.Result(Ticket(r),false,null)),(_,_)=>Task.FromResult<string?>(null));
            Check(next.Inserted,"Async timeout không giữ kẹt khóa thẻ/máy/Id");
        }
        using(var db=new MeasuredDb(connection,schema.Model,new CommandCounter()))
        {
            var r=Request(id:"ASYNC_REJECT");
            var rejected=await HqOnlineWeightWriter.SaveAsync(db,r,"101",
                _=>Task.FromResult(new HqOnlineWeightWriter.Result(Ticket(r),false,null)),(_,_)=>Task.FromResult<string?>("Chưa Vào ca"));
            Check(rejected.Error=="Chưa Vào ca" && !schema.Set<HQ_PhieuCan>().Any(x=>x.Id==r.Id),"Async ca không hợp lệ không INSERT");
            var us=await HqOnlineWeightWriter.SaveAsync(db,Request(state:"US"),"101",
                _=>throw new Exception("US không được chuẩn bị phiếu"),(_,_)=>throw new Exception());
            Check(us.Error==HqOnlineWeightWriter.UnstableError,"Async vẫn chặn US trước SQL/prepare");
        }
        using(var db=new TestDb(connection,schema.Model))
        {
            var asyncSqlFailed=false;var r=Request(id:"SQL_FAIL");
            try {await HqOnlineWeightWriter.SaveAsync(db,r,"101",
                _=>Task.FromResult(new HqOnlineWeightWriter.Result(Ticket(r),false,null)),(_,_)=>Task.FromResult<string?>(null));}
            catch(DbUpdateException) {asyncSqlFailed=true;}
            Check(asyncSqlFailed && !schema.Set<HQ_PhieuCan>().Any(x=>x.Id==r.Id),"Async SQL từ chối INSERT rollback, không xác nhận thành công");
        }
    }
    finally { schema.Database.EnsureDeleted(); }
}
Console.WriteLine($"PASS: {count} kiểm tra WSS online" + (args.Contains("--sql-localdb") ? ", gồm SQL LocalDB riêng." : ". Không kết nối SQL."));

sealed class UnavailableConsole : StringWriter
{
    public override void WriteLine(string? value) => throw new IOException("Test-only console failure");
}

sealed class SchemaDb(string connection) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder b) => b.UseSqlServer(connection);
    protected override void OnModelCreating(ModelBuilder m) => m.Entity<HQ_PhieuCan>();
}
sealed class TestDb(string connection, IModel model) : dbPMScontext(
    new DbContextOptionsBuilder<dbPMScontext>().UseSqlServer(connection).UseModel(model).Options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder b) { }
}

sealed class MeasuredDb(string connection,IModel model,CommandCounter counter):dbPMScontext(
    new DbContextOptionsBuilder<dbPMScontext>().UseSqlServer(connection).UseModel(model).AddInterceptors(counter).Options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder b) { }
}
sealed class CommandCounter:DbCommandInterceptor
{
    public int SyncCalls,AsyncCalls,IdLookups;
    public HashSet<Guid> ConnectionIds=new();
    void Record(DbCommand c,CommandEventData e,bool async)
    {
        if(async) AsyncCalls++; else SyncCalls++;
        ConnectionIds.Add(e.ConnectionId);
        if(c.CommandText.TrimStart().StartsWith("SELECT") && c.CommandText.Contains("FROM [HQ_PhieuCan]") && c.CommandText.Contains("[Id] =")) IdLookups++;
    }
    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand c,CommandEventData e,InterceptionResult<DbDataReader> r) {Record(c,e,false);return r;}
    public override InterceptionResult<int> NonQueryExecuting(DbCommand c,CommandEventData e,InterceptionResult<int> r) {Record(c,e,false);return r;}
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand c,CommandEventData e,InterceptionResult<DbDataReader> r,CancellationToken token=default) {Record(c,e,true);return ValueTask.FromResult(r);}
    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand c,CommandEventData e,InterceptionResult<int> r,CancellationToken token=default) {Record(c,e,true);return ValueTask.FromResult(r);}
}
