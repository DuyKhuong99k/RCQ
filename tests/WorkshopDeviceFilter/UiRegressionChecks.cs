using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models.Repos.Models;
using PMSHub.Components.Pages;
using PMSHub.Controllers;
using System.Reflection;

internal static class UiRegressionChecks
{
    public static async Task Run(Action<bool, string> check)
    {
        var session = new MemorySession();
        session.Set("UserId", [1]);
        session.Set("Username", [2]);
        session.Set("XuongId", [3]);
        var context = new DefaultHttpContext();
        context.Features.Set<ISessionFeature>(new MemorySessionFeature {Session = session});
        var controller = new SessionController {ControllerContext = new ControllerContext {HttpContext = context}};
        var result = await controller.Logout();
        check(!session.Keys.Any() && session.Committed, "Đăng xuất xóa và commit phiên HTTP gồm UserId, Username, XuongId");
        check(result is LocalRedirectResult {Url: "~/api/loginlocal/"}, "Đăng xuất về thẳng trang đăng nhập, không vòng qua trang gốc");
        check(await controller.Logout() is LocalRedirectResult, "Phiên đã hết vẫn đăng xuất và về trang đăng nhập được");
        var method = typeof(SessionController).GetMethod(nameof(SessionController.Logout))!;
        check(method.IsDefined(typeof(HttpPostAttribute), true) && method.IsDefined(typeof(ValidateAntiForgeryTokenAttribute), true),
            "Đăng xuất chỉ dùng POST có kiểm tra chống giả mạo yêu cầu");

        await using var services = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        await renderer.Dispatcher.InvokeAsync(async () => {
            DeviceCardState? first = null, second = null;
            ParameterView Parameters(Action<DeviceCardState> capture) => ParameterView.FromDictionary(new Dictionary<string, object?> {
                [nameof(DeviceCardState.ChildContent)] = (RenderFragment<DeviceCardState>)(state => builder => {
                    capture(state);
                    builder.AddContent(0, state.Expanded ? "DETAILS" : "CLOSED");
                })
            });
            var a = await renderer.RenderComponentAsync<DeviceCardState>(Parameters(state => first = state));
            var b = await renderer.RenderComponentAsync<DeviceCardState>(Parameters(state => second = state));
            check(a.ToHtmlString() == "CLOSED" && b.ToHtmlString() == "CLOSED", "Các cân mặc định thu gọn");
            first!.Toggle();
            check(a.ToHtmlString() == "DETAILS", "Bấm Chi tiết thực sự render nội dung mở rộng");
            check(b.ToHtmlString() == "CLOSED" && !second!.Expanded, "Mở cân này không mở cân khác hoặc dùng trạng thái toàn cục");
            first.Toggle();
            check(a.ToHtmlString() == "CLOSED", "Bấm Thu gọn đóng lại nội dung");
        });
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        var top = File.ReadAllText(Path.Combine(root, "PMSHub/Components/Layout/TopLayout.razor"));
        check(top.Contains("action=\"api/session/logout\"") && top.Contains("<AntiforgeryToken") && !top.Contains("await loginService.Logout()"),
            "Thanh công cụ dùng form HTTP thay vì sửa session trong circuit");
        var devices = File.ReadAllText(Path.Combine(root, "PMSHub/Components/Pages/Devices.razor"));
        var transferPage=File.ReadAllText(Path.Combine(root,"PMSHub/Components/Pages/ScaleAttendanceTransfers.razor"));
        var transferComponent = new ScaleAttendanceTransfers();
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var transferType = typeof(ScaleAttendanceTransfers);
        void SetTransferField(string name, object value) => transferType.GetField(name, flags)!.SetValue(transferComponent, value);
        void SelectTransfer(CheckInOut row, bool selected) => transferType.GetMethod("Select", flags)!.Invoke(transferComponent, [row, selected]);
        void SelectTransferPage(bool selected) => transferType.GetMethod("SelectPage", flags)!.Invoke(transferComponent,
            [new ChangeEventArgs { Value = selected }]);
        var selectedTransfers = (Dictionary<string, CheckInOut>)transferType.GetField("selectedRows", flags)!.GetValue(transferComponent)!;
        var firstShift = new CheckInOut { Id = "shift1", MaChamCong = "card1" };
        var secondShift = new CheckInOut { Id = "shift2", MaChamCong = "card2" };
        var thirdShift = new CheckInOut { Id = "shift3", MaChamCong = "card3" };
        SetTransferField("rows", new List<CheckInOut> { firstShift, secondShift });
        SelectTransferPage(true);
        SelectTransfer(firstShift, true);
        check(selectedTransfers.Count == 2, "Chọn cả trang không trùng ca khi chọn lại từng dòng");
        SetTransferField("rows", new List<CheckInOut> { thirdShift });
        SelectTransferPage(true);
        check(selectedTransfers.Count == 3, "Giữ lựa chọn trang trước khi chọn thêm ở trang sau");
        SelectTransferPage(false);
        check(selectedTransfers.Count == 2 && !selectedTransfers.ContainsKey(thirdShift.Id), "Bỏ chọn trang hiện tại không mất lựa chọn trang khác");
        SelectTransfer(firstShift, false);
        check(selectedTransfers.Count == 1 && selectedTransfers.ContainsKey(secondShift.Id), "Bỏ chọn từng nhân viên độc lập");
        SetTransferField("busy", true);
        SelectTransfer(thirdShift, true);
        transferType.GetMethod("ClearSelection", flags)!.Invoke(transferComponent, null);
        check(selectedTransfers.Count == 1, "Khóa thay đổi lựa chọn khi đang xử lý chuyển máy");
        SetTransferField("busy", false);
        SetTransferField("targetMachine", 7779521);
        SetTransferField("reason", "Chuyển tổ");
        transferType.GetMethod("ClearSelection", flags)!.Invoke(transferComponent, null);
        check(selectedTransfers.Count == 0 && (int)transferType.GetField("targetMachine", flags)!.GetValue(transferComponent)! == 0
            && (string)transferType.GetField("reason", flags)!.GetValue(transferComponent)! == "", "Bỏ chọn tất cả xóa máy đích và lý do cũ");
        SetTransferField("totalRows", 41);
        check((int)transferType.GetProperty("TotalPages", flags)!.GetValue(transferComponent)! == 3, "41 ca chia thành 3 trang với 20 dòng mỗi trang");
        SetTransferField("totalRows", 0);
        check((int)transferType.GetProperty("TotalPages", flags)!.GetValue(transferComponent)! == 1, "Danh sách rỗng vẫn hiển thị trang 1 hợp lệ");
        check(!transferPage.Contains("datetime-local") && !transferPage.Contains("transferTime") && transferPage.Contains("giờ máy chủ"),
            "Không có ô nhập giờ chuyển hoặc biến giờ do người dùng kiểm soát");
        var transferSource=File.ReadAllText(Path.Combine(root,"Services/ScaleAttendanceTransferService.cs"));
        check(transferSource.Contains("ServerTransferTime(DateTimeOffset.UtcNow)") && transferSource.Contains("var actor = user.UserName;")
            && transferSource.Contains("NguoiXacNhan = actor") && transferSource.Contains("user?.IsAdmin != true"),
            "Server tự ghi giờ và tài khoản quản trị đang đăng nhập, không lấy người xác nhận từ client");
        var move = devices[devices.IndexOf("private async Task CloseDialogDoiKhuVuc()", StringComparison.Ordinal)..];
        move = move[..move.IndexOf("private async void CloseDialogNhanKyCanLoi", StringComparison.Ordinal)];
        check(move.Contains("var doiKhuVuc = mayCan.WKv != SelectedKhuVuc;") && move.IndexOf("if (doiKhuVuc)",StringComparison.Ordinal)<move.IndexOf("await MayCanService.CommandDoiKhuVuc",StringComparison.Ordinal),
            "Đổi xưởng đơn thuần không gửi HTTP cấu hình khu vực xuống cân");
        check(move.IndexOf("MayCanService.Update(",StringComparison.Ordinal)<move.IndexOf("mayCan.MaXuong =",StringComparison.Ordinal)
            && move.Contains("Đã chuyển cân thành công.",StringComparison.Ordinal) && move.Contains("ToastType.Success"),
            "Chỉ cập nhật giao diện và báo chuyển thành công sau khi lưu SQL");
        check(move.Contains("ToastType.Warning") && move.Contains("Chưa chuyển được cân:") && !move.Contains("Navigation.NavigateTo"),
            "Phân biệt lỗi lưu xưởng với cảnh báo gửi cấu hình, không reload che thông báo");
        check(devices.Contains("<DeviceCardState @key=\"mayCan.Id\"") && devices.Contains("@onclick:stopPropagation=\"true\"") && !devices.Contains("buttonRefs"),
            "Template cân mới có trạng thái riêng, nút không kích hoạt chọn dòng ListBox");
        await LogoutHttpChecks.Run(root, check);
    }

    private sealed class MemorySessionFeature : ISessionFeature
    {
        public ISession Session { get; set; } = default!;
    }

    private sealed class MemorySession : ISession
    {
        private readonly Dictionary<string, byte[]> _data = new();
        public bool Committed { get; private set; }
        public bool IsAvailable => true;
        public string Id => "test";
        public IEnumerable<string> Keys => _data.Keys;
        public void Clear() => _data.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) {Committed = true; return Task.CompletedTask;}
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _data.Remove(key);
        public void Set(string key, byte[] value) => _data[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _data.TryGetValue(key, out value!);
    }
}
