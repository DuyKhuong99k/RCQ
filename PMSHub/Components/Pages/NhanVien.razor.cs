using Services;
using Syncfusion.Blazor.Grids;

namespace PMSHub.Components.Pages;

public partial class NhanVien
{
    private HubEmployeeList? _state;
    private HubEmployeeEdit _form = new();
    private HubEmployeeEdit? _deleting;
    private HubEmployeeEdit? _selectedEmployee;
    private SfGrid<HubEmployeeEdit>? _grid;
    private IReadOnlyList<HubEmployeeEdit>? _visibleSource;
    private IReadOnlyList<HubEmployeeEdit> _visibleEmployees = Array.Empty<HubEmployeeEdit>();
    private string _visibleSearch = "";
    private bool _busy, _editing, _creating, _failed;
    private string _search = "", _message = "", _formMessage = "", _originalCode = "";

    private IReadOnlyList<HubEmployeeEdit> VisibleEmployees
    {
        get
        {
            var employees = _state?.Employees ?? Array.Empty<HubEmployeeEdit>();
            var search = _search.Trim();
            // Keep DataSource stable during row selection and dialog renders.
            if (!ReferenceEquals(_visibleSource, employees) || _visibleSearch != search)
            {
                _visibleSource = employees;
                _visibleSearch = search;
                _visibleEmployees = search.Length == 0 ? employees : employees.Where(x =>
                    Contains(x.MaNhanVien, search) || Contains(x.MaHoSo, search) || Contains(x.Name, search) ||
                    Contains(x.DeptName0, search) || Contains(x.MaChamCong, search)).ToArray();
            }
            return _visibleEmployees;
        }
    }

    private static bool Contains(string? value, string search) =>
        value?.Contains(search, StringComparison.OrdinalIgnoreCase) == true;
    private IEnumerable<string> EmployeeGroups => (_state?.Employees ?? Array.Empty<HubEmployeeEdit>())
        .Select(x => x.DeptName0).OfType<string>().Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Order();

    private void SelectEmployee(RowSelectEventArgs<HubEmployeeEdit> args)
    {
        if (_busy || args.Data == null) return;
        _selectedEmployee = VisibleEmployees.FirstOrDefault(employee => employee.MaNhanVien == args.Data.MaNhanVien &&
            employee.ModifiedAt == args.Data.ModifiedAt);
    }

    private void DeselectEmployee(RowDeselectEventArgs<HubEmployeeEdit> args)
    {
        if (_selectedEmployee?.MaNhanVien == args.Data?.MaNhanVien) _selectedEmployee = null;
    }

    private async Task ClearSelectionOnActionAsync(ActionEventArgs<HubEmployeeEdit> args)
    {
        if (_busy) { args.Cancel = true; return; }
        await ClearSelectionAsync();
    }

    private async Task ClearSelectionAsync()
    {
        var hadSelection = _selectedEmployee != null;
        _selectedEmployee = null;
        // Initial load has no selection and the grid's JS may not be ready yet.
        if (hadSelection && _grid != null) await _grid.ClearSelectionAsync();
    }

    private void EditSelected()
    {
        if (_selectedEmployee != null) OpenEdit(_selectedEmployee);
    }

    private void DeleteSelected()
    {
        if (_selectedEmployee != null) AskDelete(_selectedEmployee);
    }

    protected override async Task OnAuthenticatedAsync()
    {
        await ReloadAsync();
        StateHasChanged();
    }

    private async Task ReloadAsync()
    {
        if (_busy) return;
        _busy = true;
        try
        {
            await ClearSelectionAsync();
            var next = await Manager.LoadAsync();
            await Catalog.RefreshAsync();
            _state = next;
            _message = "Đã tải danh sách nhân viên của xưởng.";
            _failed = false;
        }
        catch (Exception error)
        {
            Logger.LogWarning(error, "Không tải được danh mục nhân viên Hub.");
            _message = error is UnauthorizedAccessException ? error.Message : "Chưa tải lại được dữ liệu. Danh sách cũ được giữ nguyên.";
            _failed = true;
        }
        finally { _busy = false; }
    }

    private void OpenCreate()
    {
        if (_busy || _state == null) return;
        _form = new() { MaNhanVien = _state.SuggestedCode };
        _originalCode = _formMessage = "";
        _creating = _editing = true;
        _deleting = null;
    }

    private void OpenEdit(HubEmployeeEdit employee)
    {
        if (_busy) return;
        _form = employee.Copy();
        _originalCode = employee.MaNhanVien;
        _formMessage = "";
        _creating = false;
        _editing = true;
        _deleting = null;
    }

    private void AskDelete(HubEmployeeEdit employee)
    {
        if (_busy) return;
        _deleting = employee.Copy();
        _editing = false;
        _formMessage = "";
    }

    private void Cancel()
    {
        if (_busy) return;
        _editing = false;
        _deleting = null;
        _formMessage = "";
    }

    private async Task SaveAsync()
    {
        if (_busy || !_editing) return;
        _busy = true;
        try
        {
            var result = await Manager.SaveAsync(_form, _creating, _originalCode);
            await ShowResultAsync(result);
        }
        finally { _busy = false; }
    }

    private async Task DeleteAsync()
    {
        if (_busy || _deleting == null) return;
        _busy = true;
        try
        {
            var result = await Manager.DeleteAsync(_deleting.MaNhanVien, _deleting.ModifiedAt);
            await ShowResultAsync(result);
        }
        finally { _busy = false; }
    }

    private async Task ShowResultAsync(HubEmployeeResult result)
    {
        if (!result.Success) { _formMessage = result.Message; return; }
        _editing = false;
        _deleting = null;
        _message = result.Message;
        _failed = result.CatalogRefreshFailed;
        try
        {
            await ClearSelectionAsync();
            _state = await Manager.LoadAsync();
        }
        catch (Exception error)
        {
            Logger.LogWarning(error, "Đã lưu nhân viên nhưng chưa tải lại được bảng.");
            _message += " Chưa tải lại được bảng, bấm Tải lại.";
            _failed = true;
        }
    }
}
