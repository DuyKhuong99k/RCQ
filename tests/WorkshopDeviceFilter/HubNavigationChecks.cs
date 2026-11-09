using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using PMSHub.Navigation;

internal static class HubNavigationChecks
{
    public static async Task Run(Action<bool, string> check)
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        string Read(string path) => File.ReadAllText(Path.Combine(root, path));
        var menu = Read("PMSHub/Components/Layout/NavMenu.razor");
        var links = Regex.Matches(menu, "<NavLink\\s+href=\"([^\"]+)\"").Select(match => match.Groups[1].Value).ToArray();
        check(links.First() == "devices" && links.Count(link => link == "devices") == 1 && !links.Contains("Home"),
            "Máy cân đứng đầu và chỉ xuất hiện một lần, bỏ menu Trang chủ");
        check(links[1] == "WorkshopDevices" && links[2] == "NhanVien" && links.Count(link => link == "WorkshopDevices") == 1,
            "Thứ tự menu Máy cân → Máy chấm công tại xưởng → Nhân viên, không trùng mục");
        check(menu.Contains(">Máy cân</div>") && menu.Contains(">Máy chấm công tại xưởng</div>") &&
              menu.Contains("href=\"WorkshopDevices\""), "Đổi đúng nhãn nhưng giữ URL Máy cân/Máy chấm công tại xưởng");
        check(HubRoutes.Default == "/Devices", "Điểm vào Hub dùng chung URL Máy cân");
        var login = Read("PMSHub/Components/Pages/LoginLocal.razor.cs");
        check(login.Split("Navigation.NavigateTo(HubRoutes.Default, forceLoad: true)").Length == 3 &&
              !login.Contains("returnUrl"), "Đã có phiên và đăng nhập mới đều về Máy cân, không bị returnUrl cũ ghi đè");
        var index = new PMSHub.Components.Pages.Index();
        var navigation = new NavigationStub();
        typeof(PMSHub.Components.Pages.Index).GetProperty("Navigation", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(index, navigation);
        await (Task)typeof(PMSHub.Components.Pages.Index).GetMethod("OnAuthenticatedAsync", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(index, null)!;
        check(navigation.Destination == HubRoutes.Default && navigation.ForceLoad,
            "Trang gốc/Home chuyển thật sang Máy cân sau bước xác thực");
        var routes = typeof(PMSHub.Components.Pages.Index).Assembly.GetTypes()
            .SelectMany(type => type.GetCustomAttributes<RouteAttribute>().Select(route => route.Template)).ToArray();
        check(routes.Count(route => route == "/") == 1 && routes.Count(route => route == "/Home") == 1 &&
              routes.Contains("/Devices") && routes.Contains("/WorkshopDevices"),
            "Bookmark / và /Home có duy nhất một redirect, không trùng route và không mất trang chấm công");
        check(Read("PMSHub/Components/Pages/Devices.razor").Contains("<PageTitle>Máy cân</PageTitle>") &&
              Read("PMSHub/Components/Pages/WorkshopDevices.razor").Contains("<PageTitle>Máy chấm công tại xưởng</PageTitle>"),
            "Tiêu đề tab và trang khớp tên menu mới");
    }

    private sealed class NavigationStub : NavigationManager
    {
        public string? Destination { get; private set; }
        public bool ForceLoad { get; private set; }
        public NavigationStub() => Initialize("http://localhost/", "http://localhost/Home");
        protected override void NavigateToCore(string uri, bool forceLoad) { Destination = uri; ForceLoad = forceLoad; }
    }
}
