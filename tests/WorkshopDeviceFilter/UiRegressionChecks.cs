using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PMSHub.Components.Pages;
using PMSHub.Controllers;

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
