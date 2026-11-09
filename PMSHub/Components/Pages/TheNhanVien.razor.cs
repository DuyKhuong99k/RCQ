using Models.Repos.Models;
using Vars.Hubs;

namespace PMSHub.Components.Pages;

public partial class TheNhanVien
{
    private MayCan? _enrollmentDevice;
    private bool _enrollmentBusy;
    private bool _disposed;
    private string _lastReadCardId = "";

    // Offline is a weighing type, not a WSS connection state. Never bypass user/workshop scope.
    private IReadOnlyList<MayCan> filteredItems
    {
        get
        {
            lock (DeviceViewService.Items)
                return DeviceViewService.Items.Where(x => x.IsConnected && x.IsActive &&
                    !string.IsNullOrWhiteSpace(x.ConnectionId)).ToArray();
        }
    }

    protected override async Task OnAuthenticatedAsync()
    {
        DeviceViewService.XiNghiep = MainService.VmXiNghiep.Items.FirstOrDefault(x =>
            x.Ma == SessionService.Get("XuongId") && x.SuDung == true);
        DeviceViewService.Load(int.TryParse(SessionService.Get("UserId"), out var userId) ? userId : -1);
        DeviceViewService.DevicesChanged -= MayCanChangedHandler;
        DeviceViewService.DevicesChanged += MayCanChangedHandler;
        await RefreshEmployeeCardsAsync();
        StateHasChanged();
    }

    private async Task<bool> RefreshEmployeeCardsAsync(bool saved = false)
    {
        try
        {
            await EmployeeCardCatalog.RefreshAsync();
            if (!string.IsNullOrWhiteSpace(cardService.ItemNhanVien?.MaNhanVien))
                cardService.LoadByNhanVien(cardService.ItemNhanVien.MaNhanVien);
            return true;
        }
        catch (Exception error)
        {
            Logger.LogWarning(error, "Không tải lại được danh sách nhân viên/thẻ; giữ dữ liệu đã có.");
            var message = saved
                ? "Đã lưu. Chưa tải lại được danh sách, bấm Tải lại."
                : "Chưa tải lại được nhân viên/thẻ. Bấm Tải lại để thử lại.";
            if (saved) ThongBao = message;
            ToastService.ShowToast(message, ToastType.Warning);
            return false;
        }
    }

    private void MayCanChangedHandler(object? sender, EventArgs args)
    {
        if (_disposed) return;
        _ = InvokeAsync(() =>
        {
            if (_disposed) return;
            RefreshEnrollmentCard();
            StateHasChanged();
        });
    }

    private void RefreshEnrollmentCard()
    {
        if (_enrollmentDevice == null) return;
        if (!filteredItems.Any(x => ReferenceEquals(x, _enrollmentDevice)))
        {
            ReleaseEnrollment();
            return;
        }
        var cardId = _enrollmentDevice.NapTheId;
        if (IsVisibleAdd && _enrollmentDevice.IsNapThe && !string.IsNullOrWhiteSpace(cardId) && cardId != _lastReadCardId)
        {
            _lastReadCardId = cardId;
            TheId = cardId;
        }
    }

    private async Task MayCanStartStop(string id)
    {
        if (_enrollmentBusy) return;
        _enrollmentBusy = true;
        try
        {
            if (_enrollmentDevice != null)
            {
                var previous = _enrollmentDevice;
                ReleaseEnrollment();
                if (previous.IsConnected && !string.IsNullOrWhiteSpace(previous.ConnectionId))
                    await MayCansService.CommandSendMess(previous.Id, "");
                return;
            }
            if (!isXoa && string.IsNullOrWhiteSpace(cardService.ItemNhanVien.MaNhanVien))
            {
                ToastService.ShowToast("Chưa chọn nhân viên!", ToastType.Warning);
                return;
            }
            // Recheck at Start, not just when opening the dropdown: the scale can disconnect/move.
            var device = filteredItems.FirstOrDefault(x => x.Id == id);
            if (device == null)
            {
                ToastService.ShowToast("Chọn cân đang kết nối trong xưởng.", ToastType.Warning);
                return;
            }
            lock (device)
            {
                if (!device.IsNapThe)
                {
                    device.NapTheId = "";
                    device.IsNapThe = true;
                    _enrollmentDevice = device;
                }
            }
            if (_enrollmentDevice == null)
            {
                ToastService.ShowToast("Cân đang được dùng nạp thẻ.", ToastType.Warning);
                return;
            }
            TheId = "";
            _lastReadCardId = "";
            DeviceViewService.DeviceCard = device;
            DeviceViewService.DeviceSelectedId = device.Id;
            if (!await MayCansService.CommandSendMess(device.Id, "Nạp Thẻ"))
            {
                ReleaseEnrollment();
                ToastService.ShowToast("Không gửi được lệnh nạp thẻ.", ToastType.Warning);
                return;
            }
            MayCansService.NotifyItemChanged(device);
        }
        finally
        {
            _enrollmentBusy = false;
        }
    }

    private void ReleaseEnrollment()
    {
        var device = _enrollmentDevice;
        _enrollmentDevice = null;
        _lastReadCardId = "";
        TheId = "";
        if (device == null) return;
        lock (device)
        {
            device.IsNapThe = false;
            device.NapTheId = "";
        }
        MayCansService.NotifyItemChanged(device);
    }

    public void Dispose()
    {
        _disposed = true;
        DeviceViewService.DevicesChanged -= MayCanChangedHandler;
        ReleaseEnrollment();
    }
}
