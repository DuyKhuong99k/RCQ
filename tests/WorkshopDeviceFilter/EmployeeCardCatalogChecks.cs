using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Repos.Models;
using MvvmHelpers;
using PMSHub.Components.Pages;
using PMSHub.Controllers;
using Services;
using ViewModels.Repos.Hubs.IServices;

internal static class EmployeeCardCatalogChecks
{
    public static async Task Run(Action<bool, string> check)
    {
        // Avoid singleton VM constructors: no real configuration, SQL or firmware is accessed.
        var employeesVm = Cache("VmNhanVien", new ObservableRangeCollection<NhanVienDaiThanh>
            { new() { MaNhanVien = "OLD" } });
        var cardsVm = Cache("VmThe", new ObservableRangeCollection<TheTu>
            { new() { MaTheTu = "OLD_CARD", MaNhanVien = "OLD" } });
        var main = DispatchProxy.Create<IMainService, EnrollmentProxy>();
        ((EnrollmentProxy)(object)main).Call = (name, _) => name switch
        {
            "get_VmNhanVien" => employeesVm, "get_VmThe" => cardsVm,
            _ => throw new Exception("Unexpected business call: " + name)
        };
        var people = main.VmNhanVien;
        var cards = main.VmThe;
        var oldEmployees = people.Items;
        var oldCards = cards.Items;
        var when = new DateTime(2026, 10, 27, 10, 0, 0);
        var freshEmployees = new List<NhanVienDaiThanh>
        {
            new() { MaNhanVien = "NEW", MaHoSo = "HS_NEW", Name = "Nhân viên mới", MNgay = when }
        };
        var freshCards = new List<TheTu>
        {
            new() { MaTheTu = "0000000002", MaNhanVien = "NEW", NhanVienName = "Nhân viên mới", MaSo = "HS_NEW", NgayGio = when },
            new() { MaTheTu = "0000000001", MaNhanVien = "NEW", NhanVienName = "Nhân viên mới", MaSo = "HS_NEW", NgayGio = when }
        };
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var finish = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var catalog = new FixtureCatalog(main)
        {
            Read = async token =>
            {
                started.TrySetResult();
                await finish.Task.WaitAsync(token);
                return (freshEmployees, freshCards);
            }
        };
        var refresh = catalog.RefreshAsync();
        await started.Task;
        check(ReferenceEquals(people.Items, oldEmployees) && ReferenceEquals(cards.Items, oldCards) &&
              oldEmployees.Count == 1 && oldCards.Count == 1,
            "Đang đọc SQL vẫn giữ nguyên danh sách nhân viên/thẻ cũ, không có khoảng trống Clear");
        finish.SetResult();
        await refresh;
        check(people.Items.Single().MaNhanVien == "NEW" && cards.Items.Count == 2,
            "Chỉ sau khi đọc đủ hai bảng mới công bố danh sách nhân viên và thẻ mới");
        check(oldEmployees.Single().MaNhanVien == "OLD" && oldCards.Single().MaTheTu == "OLD_CARD",
            "Request cân đang dùng danh sách cũ không bị xóa/đổi collection giữa chừng");
        var machines = DispatchProxy.Create<IMayCansService, EnrollmentProxy>();
        ((EnrollmentProxy)(object)machines).Call = (name, _) => throw new Exception("Must not command firmware: " + name);
        var employeeEndpoint = new NhanVienController(main, machines);
        var cardEndpoint = new TheController(main);
        using (var data = Body(employeeEndpoint.Gets("20261027000000", 1, 10)))
        {
            var row = data.RootElement.GetProperty("data")[0];
            check(row.GetProperty("Id").GetString() == "NEW" && row.GetProperty("MaSo").GetString() == "HS_NEW" &&
                  row.GetProperty("Ten").GetString() == "Nhân viên mới" && row.GetProperty("NgayGioTao").GetString() == "20261027100000",
                "API Nạp nhân viên của firmware thấy ngay mã/tên mới và giữ định dạng JSON hiện có");
        }
        using (var first = Body(cardEndpoint.GetTheNhanViens("20261027000000", 1, 1)))
        using (var second = Body(cardEndpoint.GetTheNhanViens("20261027000000", 2, 1)))
        {
            var row = first.RootElement.GetProperty("data")[0];
            check(row.GetProperty("Id").GetString() == "0000000001" && row.GetProperty("MaNhanVien").GetString() == "NEW",
                "API Nạp thẻ trả ngay đúng UID gắn với nhân viên mới");
            check(second.RootElement.GetProperty("data")[0].GetProperty("Id").GetString() == "0000000002",
                "Thẻ cùng nhân viên được sắp thêm theo UID để phân trang ổn định");
        }
        var publishedEmployees = people.Items;
        var publishedCards = cards.Items;
        catalog.Read = _ => throw new IOException("Fixture read failed after employee query");
        try { await catalog.RefreshAsync(); throw new Exception("Must fail"); }
        catch (IOException) { }
        check(ReferenceEquals(people.Items, publishedEmployees) && ReferenceEquals(cards.Items, publishedCards),
            "Đọc một trong hai bảng lỗi không xóa hay công bố nửa bộ dữ liệu");
        using (var cancelled = new CancellationTokenSource())
        {
            cancelled.Cancel();
            try { await catalog.RefreshAsync(cancelled.Token); throw new Exception("Must cancel"); }
            catch (OperationCanceledException) { }
            check(ReferenceEquals(cards.Items, publishedCards), "Hủy tải không ảnh hưởng dữ liệu đang phục vụ cân");
        }
        var active = 0;
        var maximum = 0;
        catalog.Read = async token =>
        {
            maximum = Math.Max(maximum, Interlocked.Increment(ref active));
            try { await Task.Delay(10, token); return (freshEmployees, freshCards); }
            finally { Interlocked.Decrement(ref active); }
        };
        await Task.WhenAll(Enumerable.Range(0, 4).Select(_ => catalog.RefreshAsync()));
        check(maximum == 1 && cards.Items.Count == 2, "Nhiều trang cùng tải: tuần tự cập nhật, không chồng bản hoặc nhân đôi dữ liệu");

        var selectedCards = new CardService(main) { ItemNhanVien = new TheTu { MaNhanVien = "NEW" } };
        var notices = new List<string>();
        var toast = DispatchProxy.Create<IToastService, EnrollmentProxy>();
        ((EnrollmentProxy)(object)toast).Call = (name, args) =>
        {
            if (name != "ShowToast") throw new Exception(name);
            notices.Add((string)args![0]!);
            return null;
        };
        using var page = new TheNhanVien();
        Set("EmployeeCardCatalog", catalog);
        Set("cardService", selectedCards);
        Set("ToastService", toast);
        Set("Logger", NullLogger<TheNhanVien>.Instance);
        // Dispose also releases the enrollment state, but this page has not started enrollment.
        Set("DeviceViewService", new DeviceViewService(machines, DispatchProxy.Create<IUserService, MemoryProxy>()));
        check(await RefreshPage(false) && selectedCards.ItemsByNhanVien.Count == 2,
            "Tải lại trên trang Hub cập nhật luôn bảng thẻ của nhân viên đang chọn");
        catalog.Read = _ => throw new IOException("Fixture read error after a successful SAVE");
        check(!await RefreshPage(true) && notices.Last().StartsWith("Đã lưu.") && cards.Items.Count == 2,
            "Đã lưu nhưng tải lại lỗi: báo đúng Đã lưu, không báo phải thêm lại thẻ hoặc xóa cache");
        catalog.Read = _ => Task.FromResult((new List<NhanVienDaiThanh>(), new List<TheTu>()));
        await catalog.RefreshAsync();
        check(people.Items.Count == 0 && cards.Items.Count == 0,
            "SQL trả danh sách rỗng thành công phải bỏ dữ liệu cũ, không giữ thẻ đã xóa");

        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        var markup = File.ReadAllText(Path.Combine(root, "PMSHub/Components/Pages/TheNhanVien.razor"));
        var code = File.ReadAllText(Path.Combine(root, "PMSHub/Components/Pages/TheNhanVien.razor.cs"));
        var cardActions = File.ReadAllText(Path.Combine(root, "PMSHub/Components/Pages/TheNhanVien.Cards.cs"));
        check(!cardActions.Contains("VmThe.Reload()") && code.Contains("await RefreshEmployeeCardsAsync();") &&
              cardActions.Split("await RefreshEmployeeCardsAsync(saved: true)").Length == 5,
            "Trang Hub nối tải lại khi mở trang và sau thêm/sửa/xóa thẻ, không gọi Reload kiểu xóa-trước-đọc-sau");
        var loginMarkup = File.ReadAllText(Path.Combine(root, "PMSHub/Components/Pages/LoginLocal.razor"));
        var loginActions = File.ReadAllText(Path.Combine(root, "PMSHub/Components/Pages/LoginLocal.razor.cs"));
        check(!markup.Contains("@code") && !loginMarkup.Contains("@code") &&
              !cardActions.Contains("async void") && !loginActions.Contains("async void") &&
              !cardActions.Contains("Console.WriteLine") && !loginActions.Contains("Console.WriteLine"),
            "Giao diện tách khỏi xử lý; sự kiện không async void và lỗi dùng logging có ngữ cảnh");

        void Set(string name, object value) => typeof(TheNhanVien).GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(page, value);
        Task<bool> RefreshPage(bool saved) => (Task<bool>)typeof(TheNhanVien)
            .GetMethod("RefreshEmployeeCardsAsync", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(page, new object[] { saved })!;
    }

    private static object Cache<T>(string property, T items)
    {
        var type = typeof(IMainService).GetProperty(property)!.PropertyType;
        var vm = RuntimeHelpers.GetUninitializedObject(type);
        type.GetProperty("Items")!.SetValue(vm, items);
        return vm;
    }
    private static JsonDocument Body(IActionResult result) => JsonDocument.Parse(JsonSerializer.Serialize(((OkObjectResult)result).Value));
    private sealed class FixtureCatalog(IMainService main) : HubEmployeeCardCatalog(main)
    {
        public Func<CancellationToken, Task<(List<NhanVienDaiThanh>, List<TheTu>)>> Read { get; set; } = null!;
        protected override Task<(List<NhanVienDaiThanh> Employees, List<TheTu> Cards)> ReadAsync(CancellationToken cancellation) => Read(cancellation);
    }
}
