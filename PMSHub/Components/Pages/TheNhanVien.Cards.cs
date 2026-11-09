using Handlers;
using Microsoft.JSInterop;
using Models.Repos.Models;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using Vars.Hubs;
using FilteringEventArgs = Syncfusion.Blazor.DropDowns.FilteringEventArgs;

namespace PMSHub.Components.Pages;

public partial class TheNhanVien
{
    private SfGrid<TheTu> Grid = null!;
    private bool IsVisibleAdd;
    private bool IsVisibleEdit;
    private bool isXoa;
    private SfDropDownList<string, NhanVienDaiThanh> DropNhanVien = null!;
    private readonly Query LocalDataQuery = new Query().Take(10);
    private string TheId = "";
    private string ThongBao = "";
    private async Task Edit()
    {
        var result = await ErrorHandler.HandleAsync(UpdateSelectedCardsAsync);
        await Grid.ClearSelectionAsync();
        if (!result.IsSuccess) ShowCardError(result.Error);
    }

    private async Task UpdateSelectedCardsAsync()
    {
        var selectedCards = await Grid.GetSelectedRecordsAsync();
        if (selectedCards == null || selectedCards.Count == 0)
        {
            ToastService.ShowToast("Không có dữ liệu được chọn!", ToastType.Warning);
            return;
        }
        var employeeCode = cardService.ItemNhanVien.MaNhanVien;
        if (string.IsNullOrWhiteSpace(employeeCode))
        {
            ToastService.ShowToast("Chưa chọn nhân viên!", ToastType.Warning);
            return;
        }
        var employee = MainService.VmNhanVien.Items.FirstOrDefault(x => x.MaNhanVien == employeeCode);
        if (employee == null)
        {
            ToastService.ShowToast("Nhân Viên Không Tồn Tại!", ToastType.Warning);
            return;
        }

        // Work on copies: a failed SQL write must not change the currently displayed mappings.
        var updates = selectedCards.Select(MainService.VmThe.CopyItem).ToList();
        foreach (var card in updates)
        {
            card.MaNhanVien = employee.MaNhanVien;
            card.MaSo = employee.MaHoSo;
            card.NhanVienName = employee.Name;
            card.NgayGio = DateTime.Now;
            card.PCName = MainService.VmApp.PCName;
        }
        if (MainService.VmThe.Update(updates) <= 0)
        {
            ToastService.ShowToast("Không Thể Cập Nhật!", ToastType.Warning);
            return;
        }
        foreach (var card in updates)
        {
            var existing = MainService.VmThe.Items.FirstOrDefault(x => x.MaTheTu == card.MaTheTu);
            if (existing == null) continue;
            var index = MainService.VmThe.Items.IndexOf(existing);
            MainService.VmThe.Items.RemoveAt(index);
            MainService.VmThe.Items.Insert(index, card);
        }
        IsVisibleEdit = false;
        await RefreshEmployeeCardsAsync(saved: true);
        await Grid.Refresh();
    }

    private void ShowCardError(Exception? error)
    {
        Logger.LogError(error, "Không xử lý được thao tác thẻ nhân viên.");
        ToastService.ShowToast(error?.ToString(), ToastType.Error);
    }

    private void CloseDialogAdd()
    {
        IsVisibleAdd = false;
        IsVisibleEdit = false;
    }

    private bool _savingCard;
    private async Task ValueChangeHandler(ChangedEventArgs args)
    {
        if (_savingCard || args.Value?.Length is not >= 10) return;
        _savingCard = true;
        try
        {
            ThongBao = "";
            var nv = MainService.VmNhanVien.Items.FirstOrDefault(x => x.MaNhanVien == cardService.ItemNhanVien.MaNhanVien);
            if (!isXoa && nv == null)
            {
                ThongBao = "Chọn nhân viên hợp lệ trước khi thêm thẻ.";
                return;
            }
            var the = new TheTu
            {
                MaTheTu = args.Value,
                NgayGio = DateTime.Now,
                PCName = MainService.VmApp.PCName,
                MaNhanVien = cardService.ItemNhanVien.MaNhanVien,
                MaSo = nv?.MaHoSo,
                NhanVienName = nv?.Name
            };
            var mes = cardService.Find(args.Value);
            if (mes == "")
            {
                if (isXoa == false)
                {
                    if (MainService.VmThe.Insert(the) > 0)
                    {
                        cardService.ItemsByNhanVien.Insert(0, the);
                        MainService.VmThe.Items.Add(the);
                        ThongBao = $"Đã Thêm Thẻ! {the.MaTheTu}";
                        await RefreshEmployeeCardsAsync(saved: true);
                    }
                    else
                    {
                        ThongBao = $"Không Thể Thêm Thẻ! {the.MaTheTu}";
                    }
                }
                else
                {
                    ThongBao = $"Không Tìm Thấy Thẻ! {the.MaTheTu}";
                }
            }
            else
            {
                if (isXoa == false)
                {
                    ThongBao = mes;
                }
                else
                {
                    if (MainService.VmThe.Delete(the) > 0)
                    {
                        var selectedCard = cardService.ItemsByNhanVien.FirstOrDefault(x => x.MaTheTu == the.MaTheTu);
                        if (selectedCard != null) cardService.ItemsByNhanVien.Remove(selectedCard);
                        var item = MainService.VmThe.Items.FirstOrDefault(x => x.MaTheTu == the.MaTheTu);
                        if (item != null)
                        {
                            MainService.VmThe.Items.Remove(item);
                        }

                        ThongBao = $"Đã Xóa Thẻ! {the.MaTheTu}";
                        await RefreshEmployeeCardsAsync(saved: true);
                    }
                    else
                    {
                        ThongBao = $"Không Thể Xóa Thẻ! {the.MaTheTu}";
                    }
                }
            }

        }
        catch (Exception error)
        {
            Logger.LogError(error, "Không lưu được thay đổi thẻ nhân viên.");
            ThongBao = "Không lưu được thẻ. Bấm Tải lại để kiểm tra.";
        }
        finally
        {
            TheId = "";
            _savingCard = false;
        }
    }

    private async Task ToolbarClickHandler(ClickEventArgs args)
    {
        if (args.Item.Id == "refresh")
        {
            if (!await RefreshEmployeeCardsAsync()) return;
            await Grid.Refresh();
            ToastService.ShowToast("Thực hiện xong!", ToastType.Success);
        }

        if (args.Item.Id == "add")
        {
            if (!await RefreshEmployeeCardsAsync()) return;
            ThongBao = "";
            isXoa = false;
            cardService.ItemNhanVien = new TheTu();
            IsVisibleAdd = true;
        }

        if (args.Item.Id == "edit")
        {
            if (!await RefreshEmployeeCardsAsync()) return;
            cardService.ItemNhanVien = new TheTu();
            var value = await Grid.GetSelectedRecordsAsync();
            if (value != null && value.Count >= 1)
            {
                IsVisibleEdit = true;
            }
            else
            {
                ToastService.ShowToast("Không có dữ liệu được chọn!", ToastType.Warning);
            }
        }

        if (args.Item.Id == "delete")
        {
            var value = await Grid.GetSelectedRecordsAsync();
            if (value != null && value.Count >= 1)
            {
                var result = await JsRuntime.InvokeAsync<bool>("confirm", $"Xóa {value.Count} thẻ");
                if (result)
                {
                    if (MainService.VmThe.Delete(value) > 0)
                    {
                        foreach (var val in value)
                        {
                            var item = MainService.VmThe.Items.FirstOrDefault(x => x.MaTheTu == val.MaTheTu);
                            if (item != null)
                            {
                                var index = MainService.VmThe.Items.IndexOf(item);
                                MainService.VmThe.Items.RemoveAt(index);
                            }
                        }
                        await Grid.ClearSelectionAsync();
                        await RefreshEmployeeCardsAsync(saved: true);

                    }
                    else
                    {
                        ThongBao = "Không Thể Xóa Thẻ!";
                    }
                }
            }
            else
            {
                ThongBao = "";
                isXoa = true;
                cardService.ItemNhanVien = new TheTu();
                if (DeviceViewService.DeviceCard != null && DeviceViewService.DeviceSelectedId != "")
                {
                    var mayCan = MayCansService.Find(DeviceViewService.DeviceCard.Id);
                    DeviceViewService.DeviceCard.IsNapThe = mayCan.IsNapThe;
                }

                cardService.ItemsByNhanVien.Clear();
                IsVisibleAdd = true;
            }
        }

        if (args.Item.Id == "ExcelExport")
        {
            var exportProperties = new ExcelExportProperties
            {
                ExportType = ExportType.AllPages,
                IncludeTemplateColumn = true
            };
            await Grid.ExportToExcelAsync(exportProperties);
        }
    }

    private void OverlayClick(OverlayModalClickEventArgs args) => CloseEnrollmentDialog();

    private async Task OnFiltering_NhanVien(FilteringEventArgs args)
    {
        args.PreventDefaultAction = true;
        var predicates = new[] { nameof(NhanVienDaiThanh.Name), nameof(NhanVienDaiThanh.MaHoSo), nameof(NhanVienDaiThanh.MaNhanVien) }
            .Select(field => new WhereFilter
            {
                Condition = "or", Field = field, value = args.Text,
                Operator = "startswith", IgnoreAccent = true, IgnoreCase = true
            }).ToList();
        await DropNhanVien.FilterAsync(MainService.VmNhanVien.Items, new Query().Where(WhereFilter.Or(predicates)));
    }

    private void ChangeNhanVien(ChangeEventArgs<string, NhanVienDaiThanh> args)
    {
        var result = ErrorHandler.Handle(() => cardService.LoadByNhanVien(args.Value));
        if (!result.IsSuccess) ShowCardError(result.Error);
    }

    private void Closed(CloseEventArgs args) => CloseEnrollmentDialog();

    private void CloseEnrollmentDialog()
    {
        CloseDialogAdd();
        ReleaseEnrollment();
    }
}
