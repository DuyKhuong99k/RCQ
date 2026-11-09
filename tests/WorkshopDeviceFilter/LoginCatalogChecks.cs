using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Repos.Models;
using MvvmHelpers;
using PMSHub.Components.Pages;
using Services;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;

internal static class LoginCatalogChecks
{
    public static async Task Run(Action<bool, string> check)
    {
        foreach (var mode in new[] { "OK", "BAD_PASSWORD", "NO_WORKSHOP", "REFRESH_ERROR", "WAIT_REFRESH" })
        {
            var operations = new List<string>();
            var notifications = new List<ToastOption>();
            var loginCalls = 0;
            var refreshCalls = 0;
            var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var finish = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var catalog = new CatalogStub(async () =>
            {
                refreshCalls++;
                operations.Add("refresh-start");
                if (mode == "REFRESH_ERROR") throw new IOException("Fixture catalog read failed");
                if (mode == "WAIT_REFRESH")
                {
                    started.SetResult();
                    await finish.Task;
                }
                operations.Add("refresh-end");
            });
            var workshopType = typeof(IMainService).GetProperty("VmXiNghiep")!.PropertyType;
            var workshopCache = RuntimeHelpers.GetUninitializedObject(workshopType);
            workshopType.GetProperty("Items")!.SetValue(workshopCache, new ObservableRangeCollection<XiNghiep>
                { new() { Ma = "1", Ten = "Fixture workshop", SuDung = true } });
            var main = DispatchProxy.Create<IMainService, EnrollmentProxy>();
            ((EnrollmentProxy)(object)main).Call = (name, _) => name == "get_VmXiNghiep" ? workshopCache : throw new Exception(name);
            var login = DispatchProxy.Create<ILoginService, EnrollmentProxy>();
            ((EnrollmentProxy)(object)login).Call = (name, _) => name switch
            {
                "LoginLocal" => Authenticate(), "GetSessionVal" => "fixture-user", _ => throw new Exception(name)
            };
            bool Authenticate() { loginCalls++; operations.Add("authenticate"); return mode != "BAD_PASSWORD"; }
            var session = DispatchProxy.Create<ISessionService, EnrollmentProxy>();
            ((EnrollmentProxy)(object)session).Call = (name, args) =>
            {
                if (name != "Set" || (string)args![0]! != "XuongId" || (string)args[1]! != "1") throw new Exception(name);
                operations.Add("workshop-session");
                return null;
            };
            var toast = DispatchProxy.Create<IToastService, EnrollmentProxy>();
            ((EnrollmentProxy)(object)toast).Call = (name, args) =>
            {
                if (name != "ShowToast") throw new Exception(name);
                if (args![0] is ToastOption option) notifications.Add(option);
                return null;
            };
            var navigation = new TestNavigation(operations);
            var page = new LoginLocal();
            Set("MainService", main);
            Set("loginService", login);
            Set("SessionService", session);
            Set("ToastService", toast);
            Set("EmployeeCardCatalog", catalog);
            Set("Logger", NullLogger<LoginLocal>.Instance);
            Set("Navigation", navigation);
            Set("userName", "fixture-user");
            Set("passWord", "fixture-password");
            SetField("selectedWorkshopCode", mode == "NO_WORKSHOP" ? "unknown" : "1");
            var storageType = typeof(LoginLocal).GetProperty("LocalStorage", BindingFlags.Instance | BindingFlags.NonPublic)!.PropertyType;
            var storage = (LoginStorageProxy)DispatchProxy.Create(storageType, typeof(LoginStorageProxy));
            Set("LocalStorage", storage);
            var run = DoLogin();
            if (mode == "WAIT_REFRESH")
            {
                await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
                check(navigation.Destination == null && notifications.Count == 0 && !run.IsCompleted,
                    "Đăng nhập chờ tải đủ nhân viên/thẻ trước khi báo thành công và chuyển vào Hub");
                await DoLogin();
                check(loginCalls == 1 && refreshCalls == 1, "Bấm đăng nhập nhiều lần khi đang tải không tạo lượt SQL trùng");
                finish.SetResult();
            }
            await run;
            if (mode == "BAD_PASSWORD" || mode == "NO_WORKSHOP")
            {
                check(refreshCalls == 0 && navigation.Destination == null,
                    "Không tự tải catalog khi đăng nhập thất bại hoặc xưởng không hợp lệ: " + mode);
                if (mode == "NO_WORKSHOP") check(loginCalls == 0, "Không có xưởng hợp lệ thì không chạy xác thực hoặc tải SQL");
            }
            else
            {
                check(refreshCalls == 1 && navigation.Destination == PMSHub.Navigation.HubRoutes.Default && storage.Writes == 2,
                    "Đăng nhập hợp lệ tự tải danh sách rồi vào Máy cân: " + mode);
                check(operations.IndexOf("authenticate") < operations.IndexOf("refresh-start") &&
                    operations.IndexOf("workshop-session") < operations.IndexOf("refresh-start"),
                    "Chỉ làm mới sau khi xác thực và chọn xưởng thành công: " + mode);
                if (mode == "REFRESH_ERROR")
                    check(notifications.Single().CssClass == "e-toast-warning" && notifications.Single().Content.Contains("Đăng nhập thành công"),
                        "SQL tải lại lỗi: vẫn đăng nhập với cache cũ và cảnh báo, không báo sai mật khẩu");
                else
                    check(operations.IndexOf("refresh-end") < operations.IndexOf("navigate") &&
                        notifications.Single().CssClass == "e-toast-success",
                        "Tải dữ liệu xong mới chuyển trang và báo đăng nhập thành công: " + mode);
            }
            check(!(bool)typeof(LoginLocal).GetField("isLoggingIn", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(page)!,
                "Kết thúc luồng luôn nhả khóa nút đăng nhập: " + mode);

            void Set(string name, object value) => typeof(LoginLocal).GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(page, value);
            void SetField(string name, object value) => typeof(LoginLocal).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(page, value);
            Task DoLogin() => (Task)typeof(LoginLocal).GetMethod("DoLogin", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(page, null)!;
        }
    }

    private sealed class CatalogStub(Func<Task> refresh) : IHubEmployeeCardCatalog
    {
        public Task RefreshAsync(CancellationToken cancellationToken = default) => refresh();
    }
    private sealed class TestNavigation : NavigationManager
    {
        private readonly List<string> _operations;
        public string? Destination { get; private set; }
        public TestNavigation(List<string> operations)
        {
            _operations = operations;
            Initialize("http://localhost/", "http://localhost/api/loginlocal/");
        }
        protected override void NavigateToCore(string uri, bool forceLoad) { Destination = uri; _operations.Add("navigate"); }
    }
}

public class LoginStorageProxy : DispatchProxy
{
    public int Writes { get; private set; }
    protected override object? Invoke(MethodInfo? method, object?[]? args)
    {
        if (method!.Name != "SetItemAsync" || method.ReturnType != typeof(ValueTask)) throw new Exception(method.Name);
        Writes++;
        return ValueTask.CompletedTask;
    }
}
