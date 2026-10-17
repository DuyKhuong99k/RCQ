using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models.Repos.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMSHub.Controllers;
using Services;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;

var checks = 0;
void Assert(bool ok, string name)
{
    if (!ok) throw new Exception(name);
    checks++;
}

const string unstableError = "CÂN CHƯA CÂN BẰNG. CHỜ CÂN BẰNG RỒI QUẸT LẠI THẺ";
var cases = new (string Name, string? Stated, decimal Weight, string? Error)[]
{
    ("Cân chưa ổn định", "US", 7.3m, unstableError),
    ("Thiếu trạng thái", null, 7.3m, unstableError),
    ("Trạng thái rỗng", "", 7.3m, unstableError),
    ("Trạng thái không hợp lệ", "UNKNOWN", 7.3m, unstableError),
    ("ST nhưng cân rỗng", "ST", 0m, unstableError),
    ("ST nhưng trọng lượng âm", "ST", -0.1m, unstableError),
    ("Cân báo ST", "ST", 7.3m, null),
    ("Cân báo ST ở trọng lượng khác", "ST", 7.4m, null)
};

var httpGuard = typeof(DataController).GetMethod("GetHqWeightError", BindingFlags.NonPublic | BindingFlags.Static)!;
var liteMethod = typeof(CommitService).GetMethod("CommitHqLite", BindingFlags.NonPublic | BindingFlags.Instance)!;
foreach (var test in cases)
{
    var machine = new MayCan { Id = "101", WKv = AppKV.Hq, IsStated = true };
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
    var main = DispatchProxy.Create<IMainService, ServiceProxy>();
    ((ServiceProxy)(object)main).Handler = (method, _) =>
    {
        mainReads++;
        // Dừng ở ranh giới truy vấn nghiệp vụ, không mở kết nối database thật.
        if (method.Name == "get_VmPhieuCanHq") throw new BusinessValidationReached();
        throw new Exception("Truy cập nghiệp vụ ngoài dự kiến: " + method.Name);
    };
    var service = new CommitService(machines, null!, main, Options.Create(new ScaleAttendanceOptions()));
    var data = new DataLite
    {
        Id = "101.20261017120000", TheId = "CARD01", NgayGio = "20261017120000",
        TrongLuong = test.Weight, Stated = test.Stated!
    };

    if (test.Error != null)
    {
        var result = await service.CommitLite(JsonConvert.SerializeObject(data), new ClientInfo { Id = "101" }, "LITE");
        Assert(!result.Item1, "LITE từ chối: " + test.Name);
        Assert(JsonConvert.DeserializeObject<DataError>(result.Item2)?.ErrorString == test.Error,
            "LITE trả đúng lý do: " + test.Name);
        Assert(machine.ThongBao == test.Error && machine.ColorString == "red", "Hiển thị yêu cầu quẹt lại");
        Assert(mainReads == 0 && machine.Items.Count == 0, "Từ chối trước khi đọc/lưu phiếu");
        Assert(machine.MaNhanVien == null && machine.TheNhanVien == null, "Không giữ nhân viên để nhận phiếu sau");
    }
    else
    {
        // Xác nhận lần quẹt ST mới đi tiếp đến kiểm tra phiếu/ca, không giả lập đã INSERT thành công.
        var reached = false;
        try
        {
            await (Task<Tuple<bool, string>>)liteMethod.Invoke(service, new object[] { data, machine, "LITE" })!;
        }
        catch (BusinessValidationReached) { reached = true; }
        Assert(reached && mainReads == 1, "Lần quẹt ST mới đi tiếp đến kiểm tra nghiệp vụ: " + test.Name);
    }
    Assert(!machineCalls.Contains("CommandSetWaiting") && !machineCalls.Contains("CommandSetNhanVien"),
        "Kiểm tra trạng thái trước lệnh cân và lưu nhân viên");

    var payload = new JObject
    {
        ["Id"] = data.Id, ["NgayGio"] = data.NgayGio, ["MayCan"] = machine.Id,
        ["TheId"] = data.TheId, ["TrongLuongNhan"] = test.Weight, ["TrongLuongTare"] = 0,
    };
    if (test.Stated != null) payload["Stated"] = test.Stated;
    Assert((string?)httpGuard.Invoke(null, new object[] { payload, test.Weight }) == test.Error,
        "Điều kiện HTTP thống nhất với LITE: " + test.Name);
    if (test.Error != null)
    {
        var controller = new DataController(machines, main);
        foreach (var result in new[] { controller.Post(payload), controller.PostSilent(payload) })
        {
            Assert(result is BadRequestObjectResult, "HTTP từ chối trước khi mở database: " + test.Name);
            var error = JObject.FromObject(((BadRequestObjectResult)result).Value!);
            Assert((string?)(error["message"] ?? error["Status"]) == test.Error, "HTTP trả thông báo quẹt lại");
        }
        Assert(mainReads == 0, "HTTP không truy cập nghiệp vụ khi yêu cầu không hợp lệ");
    }
    Console.WriteLine("PASS: " + test.Name);
}

// ST mới được đi đến bước nghiệp vụ; trường này không xác minh được đó là quẹt mới.
Assert(httpGuard.Invoke(null, new object[] { JObject.Parse("{\"Stated\":\"ST\"}"), 7.3m }) == null,
    "IsWaiting vắng mặt tương đương false, giống DataLite");
Console.WriteLine($"PASS: {checks} kiểm tra quẹt thẻ HQ sau cân bằng. Không kết nối SQL hoặc gửi lệnh tới cân.");

public class ServiceProxy : DispatchProxy
{
    public Func<MethodInfo, object?[]?, object?> Handler { get; set; } = null!;
    protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args);
}

sealed class BusinessValidationReached : Exception { }
