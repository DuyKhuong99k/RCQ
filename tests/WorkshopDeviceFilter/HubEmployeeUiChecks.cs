using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Services;
using HubEmployeePage = PMSHub.Components.Pages.NhanVien;

internal static class HubEmployeeUiChecks
{
    public static async Task Run(Action<bool, string> check)
    {
        var page = new HubEmployeePage();
        var employee = new HubEmployeeEdit { MaNhanVien = "UI_TEST", MaHoSo = "HS", Name = "Tên gốc", IsNhom = true, IsHuman = true };
        Invoke("OpenEdit", employee);
        var editing = (HubEmployeeEdit)Get("_form")!;
        editing.Name = "Tên chưa lưu";
        check(employee.Name == "Tên gốc" && !ReferenceEquals(employee, editing), "Form nhân viên sửa bản sao, không thay đổi bảng trước khi lưu");
        check(editing.IsHuman && editing.IsNhom && !(bool)Get("_creating")!, "Form Sửa giữ đồng thời cờ nhóm/nhân viên, không chuyển thành Thêm");
        Invoke("Cancel");
        check(!(bool)Get("_editing")! && employee.Name == "Tên gốc", "Hủy Sửa không ghi vào danh sách hoặc SQL");
        Invoke("AskDelete", employee);
        check(Get("_deleting") is HubEmployeeEdit pending && pending.MaNhanVien == employee.MaNhanVien &&
              !ReferenceEquals(pending, employee), "Bấm Xóa chỉ mở xác nhận cho đúng một bản sao hồ sơ");
        Invoke("Cancel");
        check(Get("_deleting") == null, "Hủy xác nhận xóa giữ nguyên hồ sơ");
        Set("_busy", true);
        Invoke("OpenEdit", employee);
        Invoke("AskDelete", employee);
        check(!(bool)Get("_editing")! && Get("_deleting") == null, "Không mở thao tác mới khi đang lưu/xóa");
        Set("_busy", false);
        Set("_state", new HubEmployeeList("1", "Xưởng test", "000010", new[] { employee }));
        Invoke("OpenCreate");
        check((bool)Get("_creating")! && ((HubEmployeeEdit)Get("_form")!).MaNhanVien == "000010",
            "Form Thêm nhận mã gợi ý nhưng vẫn phải qua kiểm tra trùng tại SQL");
        Set("_search", "tên gốc");
        var rows = (IEnumerable<HubEmployeeEdit>)typeof(HubEmployeePage).GetProperty("VisibleEmployees", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(page)!;
        check(rows.Single() == employee, "Tìm kiếm tên trên dữ liệu đúng xưởng, không thêm truy vấn SQL theo phím gõ");
        check(ReferenceEquals(rows, typeof(HubEmployeePage).GetProperty("VisibleEmployees", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(page)),
            "DataSource tìm kiếm ổn định qua các lần render để không mất dòng đang chọn");
        Invoke("Cancel");
        Invoke("EditSelected");
        Invoke("DeleteSelected");
        check(!(bool)Get("_editing")! && Get("_deleting") == null, "Chưa chọn nhân viên thì Sửa/Xóa trên thanh công cụ không làm gì");
        Invoke("SelectEmployee", RowEvent<Syncfusion.Blazor.Grids.RowSelectEventArgs<HubEmployeeEdit>>(employee));
        Invoke("EditSelected");
        check(Get("_selectedEmployee") == employee && (bool)Get("_editing")! &&
              ((HubEmployeeEdit)Get("_form")!).MaNhanVien == employee.MaNhanVien && !ReferenceEquals(Get("_form"), employee),
            "Chọn một dòng rồi Sửa trên thanh công cụ mở đúng bản sao hồ sơ đó");
        Invoke("Cancel");
        Invoke("DeleteSelected");
        check(Get("_deleting") is HubEmployeeEdit deleting && deleting.MaNhanVien == employee.MaNhanVien && !ReferenceEquals(deleting, employee),
            "Xóa trên thanh công cụ chỉ mở xác nhận đúng người, không ghi SQL ngay");
        Invoke("Cancel");
        Set("_busy", true);
        Invoke("EditSelected");
        Invoke("DeleteSelected");
        var blockedAction = new Syncfusion.Blazor.Grids.ActionEventArgs<HubEmployeeEdit>();
        await (Task)Invoke("ClearSelectionOnActionAsync", blockedAction)!;
        check(!(bool)Get("_editing")! && Get("_deleting") == null && blockedAction.Cancel,
            "Đang xử lý thì không sửa/xóa hoặc đổi trang/sắp xếp làm lệch lựa chọn");
        Set("_busy", false);
        Invoke("DeselectEmployee", RowEvent<Syncfusion.Blazor.Grids.RowDeselectEventArgs<HubEmployeeEdit>>(new HubEmployeeEdit { MaNhanVien = "OTHER" }));
        check(Get("_selectedEmployee") == employee, "Bỏ chọn sự kiện dòng khác không làm mất lựa chọn hiện tại");
        await (Task)Invoke("ClearSelectionOnActionAsync", new Syncfusion.Blazor.Grids.ActionEventArgs<HubEmployeeEdit>())!;
        check(Get("_selectedEmployee") == null, "Đổi trang/sắp xếp bỏ dòng đã chọn để không sửa nhầm");
        Invoke("SelectEmployee", RowEvent<Syncfusion.Blazor.Grids.RowSelectEventArgs<HubEmployeeEdit>>(employee));
        Set("_search", "không có dòng này");
        await (Task)Invoke("ClearSelectionAsync")!;
        Invoke("SelectEmployee", RowEvent<Syncfusion.Blazor.Grids.RowSelectEventArgs<HubEmployeeEdit>>(employee));
        check(Get("_selectedEmployee") == null, "Đổi từ khóa bỏ lựa chọn; không nhận lại dòng không thuộc kết quả đang xem");
        Set("_grid", System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(Syncfusion.Blazor.Grids.SfGrid<HubEmployeeEdit>)));
        await (Task)Invoke("ClearSelectionAsync")!;
        check(Get("_selectedEmployee") == null, "Lần tải đầu không gọi JavaScript khi grid chưa sẵn sàng và chưa chọn dòng");
        Set("_grid", null!);
        Set("_search", "");
        Invoke("OpenImport");
        check((bool)Get("_importing")! && (string)Get("_importWorkshop")! == "1" &&
              !(bool)typeof(HubEmployeePage).GetProperty("CanImport", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(page)!,
            "Nhập Excel giữ xưởng đã mở và chưa cho lưu khi chưa kiểm tra");
        Set("_importResult", new ViewModels.Repos.Staff.NhanVienImportResult { IsSuccess = true, ReadyCount = 2 });
        check((bool)typeof(HubEmployeePage).GetProperty("CanImport", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(page)!,
            "Excel chỉ cho nhập sau kết quả xem trước hợp lệ có dòng mới");
        Set("_busy", true);
        Invoke("CloseImport");
        check((bool)Get("_importing")! && !(bool)typeof(HubEmployeePage).GetProperty("CanImport", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(page)!,
            "Đang nhập Excel không đóng cửa sổ hoặc bấm lưu lần hai");
        Set("_busy", false);
        Invoke("CloseImport");
        check(!(bool)Get("_importing")! && Get("_importResult") == null &&
              ((IReadOnlyList<ViewModels.Repos.Staff.NhanVienImportRow>)Get("_importRows")!).Count == 0,
            "Đóng nhập Excel xóa file tạm và kết quả cũ, không giữ quyền nhập lại");
        var empty = new HubEmployeeEdit();
        var errors = new List<ValidationResult>();
        check(!Validator.TryValidateObject(empty, new ValidationContext(empty), errors, true) && errors.Count >= 3,
            "Form nhân viên yêu cầu mã nhân viên, mã hồ sơ và họ tên");

        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        var markup = File.ReadAllText(Path.Combine(root, "PMSHub/Components/Pages/NhanVien.razor"));
        var importGridStart = markup.IndexOf("<SfGrid TValue=\"NhanVienImportRow\"", StringComparison.Ordinal);
        var importGrid = markup[importGridStart..markup.IndexOf("</SfGrid>", importGridStart, StringComparison.Ordinal)];
        check(importGrid.Contains("AllowFiltering=\"true\"") && importGrid.Contains("Type=\"FilterType.Excel\"") &&
              importGrid.Contains("AllowSorting=\"true\""),
            "Bảng xem trước Excel cho lọc từng cột kiểu Excel và sắp xếp");
        check(importGrid.Contains("PageSize=\"20\"") && importGrid.Contains("PageSizes=\"@(new[] { \"20\", \"50\", \"100\" })\""),
            "Bảng xem trước mặc định 20 dòng và chọn được 20/50/100 dòng mỗi trang");
        var importCode = File.ReadAllText(Path.Combine(root, "PMSHub/Components/Pages/NhanVien.Import.cs"));
        check(importGrid.Contains("DataSource=\"@_importResult.Rows\"") &&
              importCode.Contains("Manager.ImportAsync(_importRows, _importWorkshop, previewOnly: false)") &&
              markup.Contains("khi nhập vẫn xử lý toàn bộ danh sách đã kiểm tra"),
            "Lọc/phân trang chỉ đổi hiển thị, không cắt bớt danh sách nhập vào SQL");
        var navigation = File.ReadAllText(Path.Combine(root, "PMSHub/Components/Layout/NavMenu.razor"));
        var cards = File.ReadAllText(Path.Combine(root, "PMSHub/Components/Pages/TheNhanVien.razor"));
        check(markup.Contains("@onclick=\"EditSelected\"") && markup.Contains("@onclick=\"DeleteSelected\"") &&
              markup.Contains("Type=\"SelectionType.Single\"") && markup.Contains("@bind:after=\"ClearSelectionAsync\"") &&
              !markup.Contains("HeaderText=\"Thao tác\""),
            "Thanh công cụ có Thêm/Sửa/Xóa/Tải lại theo một dòng chọn, không cần kéo ngang tới cột thao tác");
        check(typeof(MayCansService).GetMethod("StartTimer", BindingFlags.NonPublic | BindingFlags.Instance) == null &&
              typeof(MayCansService).GetMethod("HttpGet", BindingFlags.NonPublic | BindingFlags.Instance) == null &&
              typeof(MayCansService).GetMethod("HttpPost", BindingFlags.NonPublic | BindingFlags.Instance) == null,
            "Không còn timer ngắt cân Offline hoặc HTTP đồng bộ đã bỏ dùng");
        foreach (var handler in new[] { "CloseDialogLo", "CloseDialogThanhPham", "CloseDialogSize", "CloseDialogNguyenLieu",
                     "CloseDialogChieuXa", "CloseDialogCoi", "CloseDialogAo", "CloseDialogPhuongTien", "CloseDialogChatLuong" })
            check(typeof(PMSHub.Components.Pages.Devices).GetMethod(handler, BindingFlags.NonPublic | BindingFlags.Instance)!.ReturnType == typeof(Task),
                "Blazor chờ hoàn tất callback Thiết bị, không async void: " + handler);
        foreach (var handler in new[] { "Edit", "MayCanStartStop" })
            check(typeof(PMSHub.Components.Pages.TheChucNang).GetMethod(handler, BindingFlags.NonPublic | BindingFlags.Instance)!.ReturnType == typeof(Task),
                "Blazor chờ callback Thẻ chức năng: " + handler);
        var gridColumns = System.Text.RegularExpressions.Regex.Matches(markup, @"<GridColumn\s[^>]*>")
            .Select(match => match.Value).ToArray();
        foreach (var flag in typeof(HubEmployeeEdit).GetProperties().Where(property => property.PropertyType == typeof(bool)))
            check(gridColumns.Count(column => column.Contains($"Field=\"@nameof(HubEmployeeEdit.{flag.Name})\"") &&
                      column.Contains("DisplayAsCheckBox=\"true\"") && column.Contains("TextAlign=\"TextAlign.Center\"")) == 1,
                "Bảng nhân viên có đúng một cột tích căn giữa theo dữ liệu form: " + flag.Name);
        check(markup.Contains("Width=\"100%\" Height=\"500\"") && markup.Contains("HeaderText=\"Tên nhóm\""),
            "Bảng giới hạn trong khung để cuộn ngang các cột, phân biệt tên nhóm với cờ thẻ nhóm");
        check(navigation.Contains("href=\"NhanVien\"") && cards.Contains("href=\"NhanVien\"") && markup.Contains("@page \"/NhanVien\""),
            "Có mục Nhân Viên trên menu và đường dẫn từ trang Thẻ Nhân Viên");
        check(markup.Contains("DataAnnotationsValidator") && markup.Contains("readonly=\"@(!_creating)\"") &&
              markup.Contains("Xác nhận xóa") && !markup.Contains("@code"),
            "Giao diện tách code-behind, khóa mã khi Sửa và yêu cầu xác nhận khi Xóa");
        check(markup.Contains("không xóa thẻ, phiếu cân hoặc lịch sử chấm công") &&
              markup.Contains("báo cáo cũ có thể không tra được tên") && !markup.Contains("Không xóa hồ sơ đã có thẻ"),
            "Xác nhận Xóa giải thích đúng chính sách như Báo Cáo và cảnh báo ảnh hưởng tra cứu/thẻ");
        var styles = File.ReadAllText(Path.Combine(root, "PMSHub/Components/Pages/NhanVien.razor.css"));
        check(System.Text.RegularExpressions.Regex.Matches(markup, "class=\"employee-field-caption\"").Count == 6 &&
              markup.Contains("Tên nhân viên <span aria-hidden=\"true\">*</span></span>") &&
              styles.Contains(".employee-field-caption { display: inline-flex;") &&
              styles.Contains("grid-template-columns: repeat(2, minmax(0, 1fr))"),
            "Sáu nhãn cùng bố cục; dấu bắt buộc nằm cạnh tên để ô Tên nhân viên/Mã chấm công thẳng hàng");

        // Grid sets Data through its internal setter; reflection is only fixture setup.
        static T RowEvent<T>(HubEmployeeEdit employee) where T : new()
        {
            var args = new T();
            typeof(T).GetProperty("Data")!.SetValue(args, employee);
            return args;
        }

        object? Invoke(string name, params object[] args) => typeof(HubEmployeePage).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(page, args);
        object? Get(string name) => typeof(HubEmployeePage).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(page);
        void Set(string name, object value) => typeof(HubEmployeePage).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(page, value);
    }
}
