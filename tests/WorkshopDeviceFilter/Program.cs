using System.Collections.Concurrent;
using System.Reflection;
using Models.Repos.Models;
using MvvmHelpers;
using Services;
using Vars;
using ViewModels.Repos.Hubs.IServices;

var checks = 0;
void Check(bool pass, string message)
{
    if (!pass) throw new Exception(message);
    checks++;
    Console.WriteLine("PASS: " + message);
}
var machines = DispatchProxy.Create<IMayCansService, MemoryProxy>();
var machineProxy = (MemoryProxy)(object)machines;
machines.Items = new ConcurrentDictionary<string, MayCan>();
var users = DispatchProxy.Create<IUserService, MemoryProxy>();
users.UserAreas = new ObservableRangeCollection<UserArea> {
    new() {UserId = 1, WKv = 17}, new() {UserId = 1, WKv = 17}, new() {UserId = 2, WKv = 18}
};
string[] workshop1 = ["10244140", "10244144", "10244148", "14148760", "14149048", "14285244", "7779520"];
string[] workshop2 = ["10244216", "14148764", "14149064", "14149084", "14285216", "14285264"];
foreach (var (ids, code) in new[] {(workshop1, "1"), (workshop2, "2")})
    foreach (var id in ids) machines.Items[id] = new MayCan {Id = id, MaXuong = code, WKv = (AppKV)17};
machines.Items["forbidden"] = new MayCan {Id = "forbidden", MaXuong = "1", WKv = (AppKV)18};
machines.Items["unknown"] = new MayCan {Id = "unknown", MaXuong = "HQ", WKv = (AppKV)17};
using var first = new DeviceViewService(machines, users) {XiNghiep = new XiNghiep {Ma = "1", Ten = "Xưởng Mới"}};
using var second = new DeviceViewService(machines, users) {XiNghiep = new XiNghiep {Ma = "2", Ten = "Xưởng Cũ"}};
first.Load(1);
second.Load(1);
Check(first.Items.Select(x => x.Id).Order().SequenceEqual(workshop1.Order()), "Xưởng Mới chỉ có đúng 7 cân theo bảng mẫu");
Check(second.Items.Select(x => x.Id).Order().SequenceEqual(workshop2.Order()), "Xưởng Cũ chỉ có đúng 6 cân theo bảng mẫu");
Check(!first.Items.Any(x => x.Id == "forbidden" || x.Id == "unknown"), "Giữ quyền khu vực và không ghép HQ vào xưởng số");
first.Load(1);
Check(first.Items.Count == 7 && machineProxy.HandlerCount("ItemAdded") == 2, "Tải lại không trùng cân hoặc đăng ký sự kiện");
var added = new MayCan {Id = "new", MaXuong = "2", WKv = (AppKV)17};
machines.Items[added.Id] = added;
machineProxy.Raise("ItemAdded", added);
Check(!first.Items.Contains(added) && second.Items.Contains(added), "Cân mới chỉ xuất hiện ở đúng xưởng");
first.DeviceCard = machines.Items["7779520"];
first.DeviceSelectedId = "7779520";
first.SelectedItems = [first.DeviceCard];
var moved = machines.Items["7779520"];
moved.MaXuong = "2";
machineProxy.Raise("ItemUpdated", moved);
Check(!first.Items.Contains(moved) && second.Items.Contains(moved), "Đổi xưởng cân tự rời danh sách cũ và vào danh sách mới");
Check(first.DeviceCard == null && first.DeviceSelectedId == "" && first.SelectedItems.Count == 0, "Xóa lựa chọn cân đã chuyển ra khỏi xưởng");
moved.WKv = (AppKV)18;
machineProxy.Raise("ItemUpdated", moved);
Check(!second.Items.Contains(moved), "Đổi khu vực không vượt quyền xem hiện có");
machineProxy.Raise("ItemRemoved", "new");
Check(!second.Items.Any(x => x.Id == "new"), "Xóa cân cập nhật danh sách");
first.XiNghiep = new XiNghiep {Ma = "3", Ten = "Xưởng Mới Đông Lạnh"};
first.Load(1);
Check(first.Items.Count == 0 && second.Items.Count == 6, "Xưởng chưa có cân để trống, không ảnh hưởng phiên khác");
first.XiNghiep = null;
first.Load(1);
Check(first.Items.Count == 0, "Không chọn xưởng không tự hiển thị toàn bộ cân");
first.XiNghiep = new XiNghiep {Ma = "1"};
first.Load(99);
Check(first.Items.Count == 0, "Tài khoản không có khu vực không xem được cân");
first.Dispose();
Check(machineProxy.HandlerCount("ItemUpdated") == 1, "Hủy phiên gỡ đăng ký sự kiện của riêng phiên đó");
var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
var login = File.ReadAllText(Path.Combine(root, "PMSHub/Components/Pages/LoginLocal.razor"));
Check(!login.Contains("@bind-Value=\"@MainService.VmXiNghiep.SelectedItem\"") &&
    login.Contains("SessionService.Set(\"XuongId\", workshop.Ma)"), "Lựa chọn đăng nhập riêng từng trang và lưu vào phiên");
Check(login.Contains("x.SuDung == true") && login.Contains("if (workshop == null)"), "Bắt buộc chọn xưởng hợp lệ đang sử dụng");
await UiRegressionChecks.Run(Check);
LoginAccountChecks.Run(Check);
Console.WriteLine($"PASS: {checks} checks. No SQL connection or firmware commands.");

public class MemoryProxy : DispatchProxy
{
    private readonly Dictionary<string, object?> _values = new();
    private readonly Dictionary<string, Delegate?> _events = new();
    public int HandlerCount(string name) => _events.GetValueOrDefault(name)?.GetInvocationList().Length ?? 0;
    public void Raise(string name, object value) => _events.GetValueOrDefault(name)?.DynamicInvoke(value);
    protected override object? Invoke(MethodInfo? method, object?[]? args)
    {
        var name = method!.Name;
        if (name.StartsWith("get_")) return _values.GetValueOrDefault(name[4..]);
        if (name.StartsWith("set_")) { _values[name[4..]] = args![0]; return null; }
        if (name.StartsWith("add_")) { _events[name[4..]] = Delegate.Combine(_events.GetValueOrDefault(name[4..]), (Delegate?)args![0]); return null; }
        if (name.StartsWith("remove_")) { _events[name[7..]] = Delegate.Remove(_events.GetValueOrDefault(name[7..]), (Delegate?)args![0]); return null; }
        throw new NotSupportedException(name);
    }
}
