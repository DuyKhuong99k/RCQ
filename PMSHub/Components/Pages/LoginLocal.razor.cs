using Microsoft.AspNetCore.Components.Web;
using Models.Repos.Models;
using Vars.Hubs;
using PMSHub.Navigation;

namespace PMSHub.Components.Pages;

public partial class LoginLocal
{
    private static readonly TimeSpan LoginMessageDuration = TimeSpan.FromSeconds(2);
    private string userName { get; set; } = "";
    private string passWord { get; set; } = "";
    private string selectedWorkshopCode = "";
    private bool isLoggingIn;
    private List<XiNghiep> AvailableWorkshops => MainService.VmXiNghiep.Items
        .Where(x => x.SuDung == true).ToList();

    private async Task<bool> RefreshCatalogAfterLoginAsync()
    {
        try
        {
            // Only after authentication succeeds. Finish before navigating into the Hub.
            await EmployeeCardCatalog.RefreshAsync();
            return true;
        }
        catch (Exception error)
        {
            // A refresh failure must not erase the previous catalog or misreport the password.
            Logger.LogWarning(error, "Đăng nhập thành công nhưng chưa tải lại được nhân viên/thẻ.");
            return false;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (loginService.IsUserAuthenticated())
        {
            Navigation.NavigateTo(HubRoutes.Default, forceLoad: true);
        }

        await base.OnInitializedAsync();
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var xuongId = await LocalStorage.GetItemAsync<string>("XuongId");
            if (!string.IsNullOrEmpty(xuongId))
            {
                selectedWorkshopCode = AvailableWorkshops.FirstOrDefault(x => x.Ma == xuongId)?.Ma ?? "";
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private async Task DoLogin()
    {
        if (isLoggingIn) return;
        isLoggingIn = true;
        try
        {
            var workshop = AvailableWorkshops.FirstOrDefault(x => x.Ma == selectedWorkshopCode);
            if (workshop == null)
            {
                ToastService.ShowToast("Vui lòng chọn xưởng trước khi đăng nhập.", ToastType.Error);
                return;
            }
            var loginResult = await Task.Run(() => loginService.LoginLocal(userName, passWord));
            if (loginResult)
            {
                SessionService.Set("XuongId", workshop.Ma);
                var catalogUpdated = await RefreshCatalogAfterLoginAsync();
                await LocalStorage.SetItemAsync("XuongId", workshop.Ma);
                await LocalStorage.SetItemAsync("PageReloadKey", "false");
                var userId = loginService.GetSessionVal("Username");
                var o = new ToastOption
                {
                    Title = "Thông Báo",
                    Content = catalogUpdated
                        ? $"User: {userId} - Đăng Nhập Thành Công"
                        : "Đăng nhập thành công. Chưa tải lại được nhân viên/thẻ; đang giữ danh sách cũ.",
                };
                o.SetType(catalogUpdated ? ToastType.Success : ToastType.Warning);

                ToastService.ShowToast(o);
                await Task.Delay(LoginMessageDuration);
                Navigation.NavigateTo(HubRoutes.Default, forceLoad: true);
            }
            else
            {
                var o = new ToastOption
                    {
                        Title = "Thông Báo",
                        Content = $"Tên đăng nhập hoặc mật khẩu không chính xác."
                    };
                o.SetType(ToastType.Error);

                ToastService.ShowToast(o);
            }
        }
        catch (Exception error)
        {
            Logger.LogError(error, "Không xử lý được đăng nhập Hub.");
        }
        finally
        {
            isLoggingIn = false;
        }
    }

    private async Task keyUpEvent(KeyboardEventArgs obj)
    {
        if (obj.Code == "Enter" || obj.Code == "NumpadEnter")
        {
            await InvokeAsync(StateHasChanged);
            await DoLogin();
        }
    }
}
