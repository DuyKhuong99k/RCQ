using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models.Repos.Models;
using Models.Repos;
using MvvmHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMSHub.Controllers;
using Services;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;
using OnlineTotals = Dao.Repos.HQ.HQ_PhieuCan.OnlineTotals;

var checks = 0;
void Assert(bool ok, string name)
{
    if (!ok) throw new Exception(name);
    checks++;
}

const string unstableError = HqOnlineWeightWriter.UnstableError;
Assert(unstableError == "Cân chưa ổn định. Không lưu phiếu", "US báo rõ không lưu phiếu trên màn cân");
// Offline omits Stated; online must still reject its absence.
var cases = new (string Name, string? Stated, decimal Weight, string? Error)[]
{
    ("Cân chưa ổn định", "US", 7.3m, unstableError),
    ("Thiếu trạng thái", null, 7.3m, unstableError),
    ("Trạng thái rỗng", "", 7.3m, unstableError),
    ("Trạng thái không hợp lệ", "UNKNOWN", 7.3m, unstableError),
    ("ST cân rỗng ở lượt chờ cũ", "ST", 0m, ScaleOnlineMessages.ZeroWeight),
    ("ST nhưng trọng lượng âm ở lượt chờ cũ", "ST", -0.1m, ScaleOnlineMessages.NegativeWeight),
    ("Cân báo ST", "ST", 7.3m, null),
    ("Cân báo ST ở trọng lượng khác", "ST", 7.4m, null)
};

var liteMethod = typeof(CommitService).GetMethod("CommitHqLite", BindingFlags.NonPublic | BindingFlags.Instance)!;
var coreMethod = typeof(CommitService).GetMethod("CommitHqLiteCore", BindingFlags.NonPublic | BindingFlags.Instance)!;
// Kiểm tra cấu trúc bản DLL thật: không giả lập INSERT hoặc nhận phiếu thành công.
// Bảo vệ ranh giới online Id-only để tránh vô tình đưa cooldown offline trở lại.
var asyncBody = coreMethod.GetCustomAttribute<AsyncStateMachineAttribute>()!.StateMachineType
    .GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance)!;
var onlineCalls = CalledMethods(asyncBody).ToArray();
Assert(!onlineCalls.Any(x => x.DeclaringType == typeof(HqWeightDuplicatePolicy)),
    "Online không gọi chính sách trùng payload 30 giây của offline");
Assert(!onlineCalls.Any(x => x.Name == "Get" && x.IsGenericMethod) &&
    onlineCalls.Any(x => x.DeclaringType == typeof(HqOnlineWeightWriter) && x.Name == "SaveAsync"),
    "Online bỏ lookup DAO thừa, chỉ writer async tra Id trong transaction SQL");
Assert(!onlineCalls.Any(x => x.Name == "Insert" || x.Name == "AddItem") &&
    onlineCalls.Count(x => x.DeclaringType == typeof(HqOnlineUi) && x.Name == "Add") == 1,
    "Không DAO INSERT hoặc callback UI đồng bộ; chỉ xếp cập nhật UI cho phiếu mới");
foreach (var test in cases)
{
    var machine = new MayCan { Id = "101", WKv = AppKV.Hq, IsStated = true,
        The = "OLD", TheAddDateTime = DateTime.Now, TheNhanVien = "OLD",
        MaNhanVien = "OLD", TheNhanVienAddDateTime = DateTime.Now };
    var machineCalls = new List<string>();
    var machines = DispatchProxy.Create<IMayCansService, ServiceProxy>();
    ((ServiceProxy)(object)machines).Handler = (method, _) =>
    {
        machineCalls.Add(method.Name);
        return method.Name switch
        {
            "Find" => machine,
            "NotifyItemChanged" => null,
            _ => throw new Exception("Không được gửi lệnh/lưu phiếu trong kiểm thử: " + method.Name)
        };
    };
    var mainReads = 0;
    var databaseOpens = 0;
    var simulateSqlFailure = false;
    var main = DispatchProxy.Create<IMainService, ServiceProxy>();
    ((ServiceProxy)(object)main).Handler = (method, _) =>
    {
        mainReads++;
        // Dừng ở ranh giới truy vấn nghiệp vụ, không mở kết nối database thật.
        if (method.Name == "get_VmPhieuCanHq")
        {
            if (simulateSqlFailure) throw new DbUpdateException("DETAILED_SQL_TEST_ERROR");
            throw new BusinessValidationReached();
        }
        throw new Exception("Truy cập nghiệp vụ ngoài dự kiến: " + method.Name);
    };
    var service = new GuardedCommitService(machines, main, () =>
    {
        databaseOpens++;
        if (simulateSqlFailure) throw new DbUpdateException("DETAILED_SQL_TEST_ERROR");
        throw new BusinessValidationReached();
    });
    var data = new DataLite
    {
        Id = "101.20261017120000", TheId = "CARD01", NgayGio = "20261017120000",
        TrongLuong = test.Weight, Stated = test.Stated!
    };
    // Fresh ST/non-positive swipes identify employees; old waiting callbacks must not.
    data.IsWaiting = test.Stated == "ST" && test.Weight <= 0;

    if (test.Error != null)
    {
        var result = await service.CommitLite(JsonConvert.SerializeObject(data), new ClientInfo { Id = "101" }, "LITE");
        Assert(!result.Item1, "LITE từ chối: " + test.Name);
        Assert(JsonConvert.DeserializeObject<DataError>(result.Item2)?.ErrorString == test.Error,
            "LITE trả đúng lý do: " + test.Name);
        var correlated = JsonConvert.DeserializeObject<DataError>(result.Item2)!;
        Assert(correlated.Id == data.Id && correlated.TheId == data.TheId && correlated.NgayGio == data.NgayGio &&
            correlated.MayCan == "101" && correlated.SaveStatus == HqOnlineResponse.NotSaved,
            "Lỗi trước lưu có Id/thẻ/giờ/máy để đối chiếu");
        Assert(machine.ThongBao == test.Error && machine.ColorString == "red", "Hiển thị yêu cầu quẹt lại");
        Assert(mainReads == 0 && machine.Items.Count == 0, "Từ chối trước khi đọc/lưu phiếu");
        Assert(machine.MaNhanVien == null && machine.TheNhanVien == null, "Không giữ nhân viên để nhận phiếu sau");
        Assert(machine.The == null && machine.TheAddDateTime == null && machine.TheNhanVienAddDateTime == null,
            "Xóa thẻ chờ cũ, không tự nhận lần ổn định kế tiếp");
    }
    else
    {
        // Xác nhận lần quẹt ST mới đi tiếp đến kiểm tra phiếu/ca, không giả lập đã INSERT thành công.
        var reached = await (Task<Tuple<bool, string>>)liteMethod.Invoke(service, new object[] { data, machine, "LITE" })!;
        var unexpected = JsonConvert.DeserializeObject<DataError>(reached.Item2)!;
        Assert(!reached.Item1 && databaseOpens == 1 && mainReads == 0 && unexpected.ErrorString == ScaleOnlineMessages.ProcessingError &&
            unexpected.SaveStatus == HqOnlineResponse.Unknown && !reached.Item2.Contains("BusinessValidationReached"),
            "ST đi đến nghiệp vụ; lỗi ngoài dự kiến không lộ exception hoặc khẳng định chưa lưu");
        simulateSqlFailure = true;
        var sqlFailure = await service.CommitLite(JsonConvert.SerializeObject(data), new ClientInfo { Id = "101" }, "LITE");
        Assert(!sqlFailure.Item1 && JsonConvert.DeserializeObject<DataError>(sqlFailure.Item2)?.ErrorString == ScaleOnlineMessages.DatabaseError,
            "Lỗi SQL online trả câu ngắn, không báo thành công");
        Assert(!sqlFailure.Item2.Contains("DETAILED_SQL_TEST_ERROR") && !sqlFailure.Item2.Contains("StackTrace"),
            "Không đẩy chi tiết exception SQL lên HMI");
        var sqlError = JsonConvert.DeserializeObject<DataError>(sqlFailure.Item2)!;
        Assert(sqlError.SaveStatus == HqOnlineResponse.Unknown && sqlError.Id == data.Id,
            "Lỗi SQL khi chưa xác minh COMMIT phải ghi UNKNOWN và giữ Id retry");
    }
    Assert(!machineCalls.Contains("CommandSetWaiting") && !machineCalls.Contains("CommandSetNhanVien"),
        "Kiểm tra trạng thái trước lệnh cân và lưu nhân viên");

    var payload = new JObject
    {
        ["Id"] = data.Id, ["NgayGio"] = data.NgayGio, ["MayCan"] = machine.Id,
        ["TheId"] = data.TheId, ["TrongLuongNhan"] = test.Weight, ["TrongLuongTare"] = 0,
        ["MaLo"] = "L01", ["MaSize"] = "20X20", ["MaThanhPham"] = "LVT2",
        ["MaLoaiNguyenLieu"] = "DuaLUOI", ["MaNhanVien"] = "0030"
    };
    if (test.Stated != null) payload["Stated"] = test.Stated;
    var rejectHttp = test.Weight <= 0 || (test.Stated != null && test.Stated != "ST");
    var parsedHttp = false;
    try { HqHttpBackupWriter.Parse(payload, machine.Id); parsedHttp = true; }
    catch (ArgumentException) { }
    Assert(parsedHttp != rejectHttp, "HTTP lịch sử được thiếu ST nhưng không nhận US/kg lỗi: " + test.Name);
    if (rejectHttp)
    {
        var controller = new DataController(machines, main);
        foreach (var result in new[] { controller.Post(payload), controller.PostSilent(payload) })
        {
            Assert(result is ObjectResult { StatusCode: 400 }, "HTTP từ chối trước khi mở database: " + test.Name);
            var error = JObject.FromObject(((ObjectResult)result).Value!);
            Assert(!string.IsNullOrWhiteSpace((string?)(error["message"] ?? error["Status"])), "HTTP có lý do từ chối rõ ràng");
        }
        Assert(mainReads == 0, "HTTP không truy cập nghiệp vụ khi yêu cầu không hợp lệ");
    }
    Console.WriteLine("PASS: " + test.Name);
}

// Exercise the real CommitLite route with cache-only employee/card lists, no VM constructors/SQL.
object Cache(string property, object items)
{
    var type = typeof(IMainService).GetProperty(property)!.PropertyType;
    var vm = RuntimeHelpers.GetUninitializedObject(type);
    type.GetField("items", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(vm, items);
    return vm;
}
DataLite Zero(string card = "CARD01", string state = "ST", decimal weight = 0,
    bool waiting = false, string time = "20261017120000", string id = "101.20261017120000", decimal tare = 0) =>
    new() { Id = id, TheId = card, Stated = state, TrongLuong = weight, IsWaiting = waiting, NgayGio = time, TrongLuongTare = tare };
var cards = new ObservableRangeCollection<TheTu>
{
    new() { MaTheTu = "CARD01", MaNhanVien = "0170" },
    new() { MaTheTu = "GROUP", MaNhanVien = "MAYCAT2" },
    new() { MaTheTu = "BOTH", MaNhanVien = "000567" },
    new() { MaTheTu = "EMPTYNAME", MaNhanVien = "EMPTY" },
    new() { MaTheTu = "FUNCTION", MaNhanVien = "FUNC" },
    new() { MaTheTu = "ORPHAN", MaNhanVien = "NOT_FOUND" }
};
var cardsVm = Cache("VmThe", cards);
var people = new ObservableRangeCollection<NhanVienDaiThanh>
{
    new() { MaNhanVien = "0170", MaHoSo = "HS0170", Name = "Lê Thị Muốn", IsHuman = true },
    new() { MaNhanVien = "MAYCAT2", MaHoSo = "MAYCAT2", Name = "MÁY CẮT 2", IsNhom = true },
    new() { MaNhanVien = "000567", MaHoSo = "MAY CAT 6", Name = "MÁY CẮT 6", IsNhom = true, IsHuman = true },
    new() { MaNhanVien = "EMPTY", IsHuman = true },
    new() { MaNhanVien = "FUNC", IsHuman = true, IsChucNang = true }
};
var peopleVm = Cache("VmNhanVien", people);
var identityCases = new (string Name, DataLite Data, bool Send, string Error, string SendResult)[]
{
    ("Nhận diện nhân viên ST/0", Zero(), true, ScaleOnlineMessages.ZeroWeight, "OK"),
    ("Nhận diện ST âm do tare -0.80/0.80", Zero(weight: -0.80m, tare: 0.80m), true, ScaleOnlineMessages.NegativeWeight, "OK"),
    ("ST âm 1 gram vẫn nhận diện", Zero(weight: -0.001m), true, ScaleOnlineMessages.NegativeWeight, "OK"),
    ("ST âm nhận diện nhóm", Zero("GROUP", weight: -0.80m, tare: 0.80m), true, ScaleOnlineMessages.NegativeWeight, "OK"),
    ("ST âm nhóm đồng thời IsHuman", Zero("BOTH", weight: -0.80m, tare: 0.80m), true, ScaleOnlineMessages.NegativeWeight, "OK"),
    ("Nhận diện nhóm", Zero("GROUP"), true, ScaleOnlineMessages.ZeroWeight, "OK"),
    ("Nhóm đồng thời IsHuman", Zero("BOTH"), true, ScaleOnlineMessages.ZeroWeight, "OK"),
    ("Tên/hồ sơ null an toàn", Zero("EMPTYNAME"), true, ScaleOnlineMessages.ZeroWeight, "OK"),
    ("Thẻ không có trong cache", Zero("UNKNOWN"), false, ScaleOnlineMessages.UnknownEmployee, "OK"),
    ("Thẻ không có nhân viên tương ứng", Zero("ORPHAN"), false, ScaleOnlineMessages.UnknownEmployee, "OK"),
    ("Thẻ chức năng không thành thẻ sản xuất", Zero("FUNCTION"), false, "HQ cần thẻ nhân viên hoặc nhóm sản xuất.", "OK"),
    ("US/0 không gửi tên", Zero(state: "US"), false, unstableError, "OK"),
    ("US âm không gửi tên", Zero(state: "US", weight: -0.80m, tare: 0.80m), false, unstableError, "OK"),
    ("Lượt chờ 0 cũ không nhận diện", Zero(waiting: true), false, ScaleOnlineMessages.ZeroWeight, "OK"),
    ("Lượt chờ âm cũ không nhận diện", Zero(weight: -0.80m, waiting: true), false, ScaleOnlineMessages.NegativeWeight, "OK"),
    ("ST âm thẻ lạ", Zero("UNKNOWN", weight: -0.80m), false, ScaleOnlineMessages.UnknownEmployee, "OK"),
    ("ST âm thiếu thẻ", Zero("", weight: -0.80m), false, ScaleOnlineMessages.InvalidCard, "OK"),
    ("ST âm sai giờ", Zero(weight: -0.80m, time: "BAD"), false, ScaleOnlineMessages.InvalidTime, "OK"),
    ("ST âm sai Id", Zero(weight: -0.80m, id: ""), false, ScaleOnlineMessages.InvalidId, "OK"),
    ("ST âm nhưng tare âm", Zero(weight: -0.80m, tare: -0.80m), false, ScaleOnlineMessages.InvalidTare, "OK"),
    ("ST âm quá 3 số lẻ", Zero(weight: -0.0001m), false, ScaleOnlineMessages.InvalidPrecision, "OK"),
    ("ST âm gửi tên thất bại", Zero(weight: -0.80m), true, ScaleOnlineMessages.IdentitySendFailed, "FALSE"),
    ("Thiếu thẻ không dùng lại thẻ cũ", Zero(""), false, ScaleOnlineMessages.InvalidCard, "OK"),
    ("Sai giờ không gửi tên", Zero(time: "BAD"), false, ScaleOnlineMessages.InvalidTime, "OK"),
    ("Thiếu Id không gửi tên", Zero(id: ""), false, ScaleOnlineMessages.InvalidId, "OK"),
    ("Tare âm không gửi tên", Zero(tare: -0.01m), false, ScaleOnlineMessages.InvalidTare, "OK"),
    ("Tare quá chính xác không gửi tên", Zero(tare: 0.0001m), false, ScaleOnlineMessages.InvalidPrecision, "OK"),
    ("Gửi tên trả false không báo nhận diện", Zero(), true, ScaleOnlineMessages.IdentitySendFailed, "FALSE"),
    ("Gửi tên lỗi không báo lưu thành công", Zero(), true, ScaleOnlineMessages.IdentitySendFailed, "THROW")
};
foreach (var test in identityCases)
{
    var machine = new MayCan { Id = "101", WKv = AppKV.Hq, TongSoRo = 42, TongTrongLuong = 123,
        The = "OLD", TheAddDateTime = DateTime.Now, TheNhanVien = "OLD", TheNhanVienAddDateTime = DateTime.Now,
        MaNhanVien = "OLD", NhanVienName = "OLD" };
    var reads = new List<string>();
    var databaseOpens = 0;
    var sent = new List<object?[]>();
    var main = DispatchProxy.Create<IMainService, ServiceProxy>();
    ((ServiceProxy)(object)main).Handler = (method, _) =>
    {
        reads.Add(method.Name);
        return method.Name switch
        {
            "get_VmThe" => cardsVm, "get_VmNhanVien" => peopleVm,
            _ => throw new Exception("Không được đọc phiếu/ca/SQL khi nhận diện: " + method.Name)
        };
    };
    var machines = DispatchProxy.Create<IMayCansService, ServiceProxy>();
    ((ServiceProxy)(object)machines).Handler = (method, args) =>
    {
        if (method.Name == "Find") return machine;
        if (method.Name == "NotifyItemChanged") return null;
        if (method.Name == "CommandSetNhanVien")
        {
            sent.Add(args!);
            if (test.SendResult == "THROW") throw new InvalidOperationException("TEST_DISPLAY_FAILURE");
            return Task.FromResult(test.SendResult != "FALSE");
        }
        throw new Exception("Không được ghi phiếu/gửi lệnh chờ/đổi tổng: " + method.Name);
    };
    var service = new GuardedCommitService(machines, main, () =>
    { databaseOpens++; throw new Exception("Nhận diện không được mở SQL"); });
    var result = await service.CommitLite(JsonConvert.SerializeObject(test.Data), new ClientInfo { Id = "101" }, "LITE");
    Assert(!result.Item1 && JsonConvert.DeserializeObject<DataError>(result.Item2)?.ErrorString == test.Error,
        "Không báo thành công cân, trả đúng lỗi: " + test.Name);
    Assert(databaseOpens == 0 && JsonConvert.DeserializeObject<DataError>(result.Item2)?.SaveStatus == HqOnlineResponse.NotSaved,
        "ST 0/âm chỉ nhận diện, không mở SQL và luôn NOT_SAVED: " + test.Name);
    Assert(machine.ThongBao == test.Error && machine.Items.Count == 0 && machine.TongSoRo == 42 && machine.TongTrongLuong == 123,
        "Chỉ nhận diện, không ghi/cộng/reset tổng: " + test.Name);
    Assert(sent.Count == (test.Send ? 1 : 0), "Chỉ gửi SETNHANVIEN cho nhận diện hợp lệ: " + test.Name);
    Assert(machine.The == null && machine.TheAddDateTime == null && machine.TheNhanVien == null && machine.TheNhanVienAddDateTime == null,
        "Không giữ thẻ chờ hoặc tự mở ca: " + test.Name);
    if (test.Send && test.SendResult == "OK")
    {
        var card = cards.Single(x => x.MaTheTu == test.Data.TheId);
        var person = people.Single(x => x.MaNhanVien == card.MaNhanVien);
        Assert(sent[0].SequenceEqual(new object?[] { "101", person.MaNhanVien, person.MaHoSo ?? "", person.Name ?? "" }),
            "Gửi đúng mã nhân viên, hồ sơ và tên theo lệnh cũ: " + test.Name);
        Assert(machine.MaNhanVien == person.MaNhanVien && machine.NhanVienName == person.Name, "Hub hiển thị đúng nhân viên");
        var next = Zero(card: "", weight: 8);
        var nextResult = await service.CommitLite(JsonConvert.SerializeObject(next), new ClientInfo { Id = "101" }, "LITE");
        Assert(!nextResult.Item1 && sent.Count == 1 && machine.MaNhanVien == null,
            "ST có ký tiếp theo không được dùng lại danh tính của lượt 0/âm kg");
    }
    if (test.Data.Stated != "ST" || test.Data.IsWaiting ||
        test.Error is ScaleOnlineMessages.InvalidTime or ScaleOnlineMessages.InvalidId or ScaleOnlineMessages.InvalidCard or ScaleOnlineMessages.InvalidTare or ScaleOnlineMessages.InvalidPrecision)
        Assert(reads.Count == 0, "Request không hợp lệ bị chặn trước cache: " + test.Name);
    Console.WriteLine("PASS: " + test.Name);
}

// ST mới được đi đến bước nghiệp vụ; trường này không xác minh được đó là quẹt mới.
Assert(HqOnlineWeightWriter.RequestError(new DataLite { Id="101.20261017120000",NgayGio="20261017120000",
        TheId="CARD01",Stated="ST",TrongLuong=7.3m }) == null,
    "IsWaiting vắng mặt tương đương false, giống DataLite");

var savedTicket = new HQ_PhieuCan { Id = "101.20261017120000", MayCan = "101", TheId = "CARD01", TheIdNhanVien = "CARD01",
    MaNhanVien = "0170", MaLo = "lo1", MaSize = "00X00", MaThanhPham = "LM", MaLoaiNguyenLieu = "XOAIDONG", MaXuong = "1",
    NgayGio = new DateTime(2026,10,17,12,0,0), Ngay = new DateOnly(2026,10,17), TrongLuong = 8, TrongLuongTare = .71m,
    ChiSanLuong = true, Status = 1 };
var expectedTotals = new OnlineTotals { MachineCount = 5, MachineWeight = 40, EmployeeCount = 2, EmployeeWeight = 16 };
var totalsReads = 0;
var successReply = await HqOnlineResponse.ForSavedAsync(savedTicket, "LITE", _ => { totalsReads++; return Task.FromResult(expectedTotals); },
    employee: people.First());
var successData = JObject.Parse(successReply.Item2);
Assert(successReply.Item1 && totalsReads == 1 && (int?)successData["IsDataAction"] == 1 &&
    (string?)successData["SaveStatus"] == HqOnlineResponse.Saved, "Chỉ ACK thành công khi đọc đủ tổng");
Assert((string?)successData["Id"] == savedTicket.Id && (string?)successData["TheId"] == "CARD01" &&
    (string?)successData["MaNhanVien"] == "0170" && (string?)successData["NhanVienName"] == "Lê Thị Muốn",
    "Phản hồi thành công có đủ Id/thẻ/mã/tên để đối chiếu");
Assert((int?)successData["TongSoRoCan"] == 5 && (decimal?)successData["TongTrongLuongCan"] == 40 &&
    (int?)successData["TongSoRo"] == 2 && (decimal?)successData["TongTrongLuong"] == 16,
    "Giữ nguyên tên trường và số tổng trong giao thức cũ");
Assert(successData["MayCan"] == null, "Không đưa lại trường MayCan đã bỏ khỏi ACK cũ");
foreach (var failure in new Exception[] { new TimeoutException("SECRET_TOTALS_FAILURE"), new InvalidOperationException("SECRET_TOTALS_FAILURE") })
{
    var failedReads = 0;
    var failedReply = await HqOnlineResponse.ForSavedAsync(savedTicket, "LITE", _ =>
        { failedReads++; return Task.FromException<OnlineTotals>(failure); },
        _ => throw new Exception("Không cập nhật UI bằng tổng giả"), retryDelay: TimeSpan.Zero);
    var detail = JsonConvert.DeserializeObject<DataError>(failedReply.Item2)!;
    Assert(!failedReply.Item1 && detail.SaveStatus == HqOnlineResponse.Saved && detail.Id == savedTicket.Id &&
        detail.TheId == savedTicket.TheIdNhanVien && detail.ErrorString == ScaleOnlineMessages.SavedTotalsError,
        "Lỗi sau lưu nói rõ SAVED và Id gốc, không nói lỗi INSERT");
    Assert(failedReads == HqOnlineResponse.MaxTotalsAttempts && !detail.ErrorString.Contains("gửi lại", StringComparison.OrdinalIgnoreCase) &&
        !detail.ErrorString.Contains("quẹt lại", StringComparison.OrdinalIgnoreCase),
        "WSS chỉ thử lại tổng có giới hạn, không yêu cầu firmware gửi phiếu mới");
    Assert(!failedReply.Item2.Contains("SECRET_TOTALS_FAILURE") && !failedReply.Item2.Contains("TongSoRo") &&
        !failedReply.Item2.Contains("IsDataAction"), "Không gửi tổng 0/tổng cũ hoặc ACK hoàn thành khi tổng lỗi");
}
Assert((await HqOnlineResponse.ForSavedAsync(savedTicket, "LITE", _ => Task.FromResult(expectedTotals), _ => throw new Exception("UI_ONLY"))).Item1,
    "Lỗi cập nhật UI không đổi phản hồi lưu/tổng thành công");
var recoveredReads = 0;
var uiUpdates = 0;
var automaticallyRecovered = await HqOnlineResponse.ForSavedAsync(savedTicket, "LITE", _ =>
{
    recoveredReads++;
    return recoveredReads < 3 ? Task.FromException<OnlineTotals>(new TimeoutException("TRANSIENT_TOTALS"))
        : Task.FromResult(expectedTotals);
}, _ => uiUpdates++, retryDelay: TimeSpan.Zero);
var recovered = JsonConvert.DeserializeObject<DataLiteRes>(automaticallyRecovered.Item2)!;
Assert(automaticallyRecovered.Item1 && recoveredReads == 3 && uiUpdates == 1,
    "WSS tự phục hồi query tổng ở lần ba, chỉ cập nhật UI một lần");
Assert(recovered.Id == savedTicket.Id && recovered.TheId == savedTicket.TheIdNhanVien && recovered.TongSoRo == 2 &&
    recovered.TongTrongLuong == 16 && recovered.IsDataAction == 1,
    "Tự retry tổng giữ đúng Id/thẻ/phiếu gốc, không yêu cầu gửi phiếu khác");
var neverCompletes = new TaskCompletionSource<OnlineTotals>(TaskCreationOptions.RunContinuationsAsynchronously);
var timeoutReads = 0;
var timeoutUi = 0;
CancellationToken queryCancellation = default;
var budgetExpired = await HqOnlineResponse.ForSavedAsync(savedTicket, "LITE", cancellation =>
{
    timeoutReads++; queryCancellation = cancellation; return neverCompletes.Task;
}, _ => timeoutUi++, retryDelay: TimeSpan.Zero, budget: TimeSpan.FromMilliseconds(60));
var timeoutDetail = JsonConvert.DeserializeObject<DataError>(budgetExpired.Item2)!;
Assert(!budgetExpired.Item1 && queryCancellation.IsCancellationRequested && timeoutReads == 1 && timeoutUi == 0 &&
    timeoutDetail.SaveStatus == HqOnlineResponse.Saved && timeoutDetail.Id == savedTicket.Id,
    "Hết ngân sách query thì hủy chờ, không treo cân vô hạn hoặc mất trạng thái đã lưu");
neverCompletes.SetResult(expectedTotals);
await Task.Delay(20);
Assert(timeoutUi == 0 && !budgetExpired.Item1,
    "Kết quả query cũ về sau khi hết hạn không phát thêm ACK/cập nhật lượt mới");
Assert(!ScaleOnlineMessages.SavedTotalsError.Contains("gửi lại", StringComparison.OrdinalIgnoreCase) &&
    !ScaleOnlineMessages.ProcessingError.Contains("gửi lại", StringComparison.OrdinalIgnoreCase),
    "Không yêu cầu firmware không giữ Id tạo một phiếu mới để retry lỗi hệ thống");
using (var totalsEntered = new ManualResetEventSlim())
using (var totalsRelease = new ManualResetEventSlim())
{
    var delayedReply = Task.Run(() => HqOnlineResponse.ForSavedAsync(savedTicket, "LITE", _ =>
    {
        totalsEntered.Set();
        if (!totalsRelease.Wait(TimeSpan.FromSeconds(10))) throw new TimeoutException();
        return Task.FromResult(expectedTotals);
    }));
    try
    {
        Assert(totalsEntered.Wait(TimeSpan.FromSeconds(10)) && !delayedReply.IsCompleted,
            "Không ACK sớm khi query tổng còn đang chạy");
    }
    finally { totalsRelease.Set(); }
    Assert((await delayedReply).Item1, "Tổng xong mới trả ACK đầy đủ");
}
var legacyErrorJson = JObject.Parse(JsonConvert.SerializeObject(new DataError { Cmd = "LEGACY", ErrorString = "OLD" }));
Assert(legacyErrorJson.Properties().Count() == 3 && legacyErrorJson["SaveStatus"] == null,
    "DataError nhánh cũ không phát sinh trường null hoặc đổi cấu trúc");
var legacySuccessJson = JObject.Parse(JsonConvert.SerializeObject(new DataLiteRes { Id = "OLD" }));
Assert(legacySuccessJson["SaveStatus"] == null && legacySuccessJson["MaHoSo"] == null && legacySuccessJson["NhanVienName"] == null,
    "ACK nhánh cũ không phát sinh các trường HQ mới");
var oversized = HqOnlineResponse.Error("LITE", "ERR", new DataLite { Id = new string('x', 101), TheId = new string('x', 51), NgayGio = new string('x', 15) }, "101");
Assert(oversized.Id == null && oversized.TheId == null && oversized.NgayGio == null, "Không echo metadata quá dài lên firmware");

foreach (var mode in new[] { "JSON", "OTHER", "EMPTY", "REJECTED", "SAVED", "OK" })
{
    var commit = DispatchProxy.Create<ICommitService, ServiceProxy>();
    ((ServiceProxy)(object)commit).Handler = (method, _) =>
    {
        if (method.Name != "CommitLite") throw new Exception(method.Name);
        if (mode == "JSON") throw new JsonReaderException("RAW_SECRET_JSON");
        if (mode == "OTHER") throw new InvalidOperationException("RAW_SECRET_EXCEPTION");
        return Task.FromResult(mode switch
        {
            "EMPTY" => Tuple.Create(false, ""),
            "REJECTED" => Tuple.Create(false, JsonConvert.SerializeObject(HqOnlineResponse.Error("LITE", unstableError, Zero(), "101"))),
            "SAVED" => Tuple.Create(false, JsonConvert.SerializeObject(HqOnlineResponse.Error("LITE", ScaleOnlineMessages.SavedTotalsError, Zero(), "101", HqOnlineResponse.Saved))),
            _ => successReply
        });
    };
    var route = new CommunicationService(null!, commit, Options.Create(new ScaleAttendanceOptions()));
    var payload = JsonConvert.SerializeObject(Zero());
    var outer = JObject.Parse(await route.Route(new ClientInfo { Id = "101", WKv = AppKV.Hq, IsActive = true }, "LITE", payload.Length, payload));
    Assert((bool?)outer["IsError"] == (mode != "OK"), "Router giữ envelope LITE đúng trạng thái: " + mode);
    if (mode == "OK")
    {
        Assert((string?)outer["Data"] == successReply.Item2, "Router không sửa ACK đầy đủ tổng");
        continue;
    }
    var detail = JsonConvert.DeserializeObject<DataError>((string)outer["DataError"]!)!;
    Assert(!string.IsNullOrWhiteSpace(detail.ErrorString) && detail.Id == "101.20261017120000" && detail.TheId == "CARD01" &&
        detail.NgayGio == "20261017120000" && detail.MayCan == "101", "LITE lỗi không rỗng và có metadata: " + mode);
    Assert(detail.SaveStatus == (mode is "JSON" or "REJECTED" ? HqOnlineResponse.NotSaved :
        mode == "SAVED" ? HqOnlineResponse.Saved : HqOnlineResponse.Unknown), "Không đoán sai trạng thái lưu: " + mode);
    Assert(!outer.ToString().Contains("RAW_SECRET"), "Router không lộ nội dung exception");
}

// Real JSON deserialization errors, not only a mocked CommitLite exception.
foreach (var payload in new[] { "{", "[]", "{\"Id\":\"101.20261017120000\",\"TrongLuong\":[]}" })
{
    var machines = DispatchProxy.Create<IMayCansService, ServiceProxy>();
    ((ServiceProxy)(object)machines).Handler = (_, _) => throw new Exception("Không được tìm cân/SQL trước khi JSON hợp lệ");
    var commit = new CommitService(machines, null!, null!, Options.Create(new ScaleAttendanceOptions()));
    var route = new CommunicationService(null!, commit, Options.Create(new ScaleAttendanceOptions()));
    var envelope = JObject.Parse(await route.Route(new ClientInfo { Id = "101", WKv = AppKV.Hq, IsActive = true }, "LITE", payload.Length, payload));
    var detail = JsonConvert.DeserializeObject<DataError>((string)envelope["DataError"]!)!;
    Assert((bool?)envelope["IsError"] == true && detail.ErrorString == ScaleOnlineMessages.InvalidPayload &&
        detail.SaveStatus == HqOnlineResponse.NotSaved, "JSON hỏng thật luôn có lỗi cấu trúc, không truy cập SQL");
}
await TransportChecks.Run(Assert);
await DeliveryChecks.Run(Assert);
await NonPositiveWeightChecks.Run(Assert);
Console.WriteLine($"PASS: {checks} kiểm tra quẹt thẻ HQ sau cân bằng. Không kết nối SQL hoặc gửi lệnh tới cân.");

static IEnumerable<MethodBase> CalledMethods(MethodInfo method)
{
    var opcodes = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(x => x.FieldType == typeof(OpCode)).Select(x => (OpCode)x.GetValue(null)!)
        .ToDictionary(x => unchecked((ushort)x.Value));
    var bytes = method.GetMethodBody()!.GetILAsByteArray()!;
    for (var i = 0; i < bytes.Length;)
    {
        ushort code = bytes[i++];
        if (code == 0xfe) code = (ushort)(0xfe00 | bytes[i++]);
        var operand = opcodes[code].OperandType;
        if (operand == OperandType.InlineMethod)
            yield return method.Module.ResolveMethod(BitConverter.ToInt32(bytes, i),
                method.DeclaringType?.GetGenericArguments(), method.GetGenericArguments())!;
        i += operand switch
        {
            OperandType.InlineNone => 0,
            OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or OperandType.ShortInlineVar => 1,
            OperandType.InlineVar => 2,
            OperandType.InlineI8 or OperandType.InlineR => 8,
            OperandType.InlineSwitch => 4 + 4 * BitConverter.ToInt32(bytes, i),
            _ => 4
        };
    }
}

public class ServiceProxy : DispatchProxy
{
    public Func<MethodInfo, object?[]?, object?> Handler { get; set; } = null!;
    protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args);
}

sealed class BusinessValidationReached : Exception { }

sealed class GuardedCommitService(IMayCansService machines, IMainService main, Func<dbPMScontext> database)
    : CommitService(machines, null!, main, Options.Create(new ScaleAttendanceOptions()))
{
    protected override dbPMScontext CreateHqOnlineDb() => database();
}
