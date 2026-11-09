using Microsoft.AspNetCore.Components.Forms;
using PMSHub.EmployeeImport;
using ViewModels.Repos.Staff;

namespace PMSHub.Components.Pages;

public partial class NhanVien
{
    private const long MaxImportBytes = 5 * 1024 * 1024;
    private bool _importing;
    private string _importWorkshop = "", _importFileName = "", _importMessage = "";
    private IReadOnlyList<NhanVienImportRow> _importRows = Array.Empty<NhanVienImportRow>();
    private NhanVienImportResult? _importResult;
    private bool CanImport => !_busy && _importResult is { IsSuccess: true, ReadyCount: > 0 };

    private void OpenImport()
    {
        if (_busy || _state == null) return;
        ResetImport();
        _importWorkshop = _state.WorkshopCode;
        _importing = true;
        _editing = false;
        _deleting = null;
    }

    private void CloseImport()
    {
        if (_busy) return;
        _importing = false;
        ResetImport();
    }

    private void ResetImport()
    {
        _importRows = Array.Empty<NhanVienImportRow>();
        _importResult = null;
        _importFileName = _importMessage = "";
    }

    private async Task ReadImportFileAsync(InputFileChangeEventArgs args)
    {
        if (_busy || !_importing) return;
        ResetImport();
        _busy = true;
        try
        {
            var file = args.File;
            if (file.Size is <= 0 or > MaxImportBytes ||
                !string.Equals(Path.GetExtension(file.Name), ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                _importMessage = "Chọn file .xlsx có dung lượng tối đa 5 MB.";
                return;
            }
            _importFileName = file.Name;
            using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            await using var source = file.OpenReadStream(MaxImportBytes, deadline.Token);
            using var content = new MemoryStream();
            await source.CopyToAsync(content, deadline.Token);
            content.Position = 0;
            _importRows = await Task.Run(() => HubEmployeeExcelReader.Read(content));
            _importMessage = $"Đã đọc {_importRows.Count} dòng. Bấm Kiểm tra danh sách để xem trước.";
        }
        catch (InvalidDataException error) { _importMessage = error.Message; }
        catch (Exception error)
        {
            Logger.LogWarning(error, "Không đọc được file nhập nhân viên Hub.");
            _importMessage = "Không đọc được file. Lưu lại dạng .xlsx rồi chọn lại.";
        }
        finally { _busy = false; }
    }

    private async Task CheckImportAsync()
    {
        if (_busy || !_importing || _importRows.Count == 0) return;
        _busy = true;
        _importResult = null;
        try
        {
            _importResult = await Manager.ImportAsync(_importRows, _importWorkshop);
            _importMessage = _importResult.Messages;
        }
        finally { _busy = false; }
    }

    private async Task SaveImportAsync()
    {
        if (!CanImport || !_importing) return;
        _busy = true;
        // Consume preview before saving; an uncertain response always requires another check.
        _importResult = null;
        try
        {
            _importResult = await Manager.ImportAsync(_importRows, _importWorkshop, previewOnly: false);
            _importMessage = _importResult.Messages;
            if (_importResult.ImportedCount > 0)
            {
                _message = _importMessage;
                _failed = false;
                try
                {
                    await ClearSelectionAsync();
                    _state = await Manager.LoadAsync();
                }
                catch (Exception error)
                {
                    Logger.LogWarning(error, "Đã nhập nhân viên nhưng chưa tải lại được bảng.");
                    _importMessage += " Chưa tải lại được bảng, bấm Tải lại sau khi đóng cửa sổ.";
                    _failed = true;
                }
            }
        }
        finally { _busy = false; }
    }
}
