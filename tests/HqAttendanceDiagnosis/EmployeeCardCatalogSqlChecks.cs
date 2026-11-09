using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models.Repos.Models;
using MvvmHelpers;
using Services;
using ViewModels.Repos.Hubs.IServices;

internal static class EmployeeCardCatalogSqlChecks
{
    public static async Task Run(DbContext schema, string connection, Action<bool, string> check)
    {
        var target = new SqlConnectionStringBuilder(connection);
        if (target.DataSource != @"(localdb)\MSSQLLocalDB" || !target.InitialCatalog.StartsWith("RCQ_AttendanceDiag_", StringComparison.Ordinal))
            throw new InvalidOperationException("Only the owned GUID LocalDB fixture may be used.");
        var oldDaoConnection = AppViewModels.Base.Ins.ConnectionString;
        {
            var employeesVm = Cache("VmNhanVien", new ObservableRangeCollection<NhanVienDaiThanh>());
            var cardsVm = Cache("VmThe", new ObservableRangeCollection<TheTu>());
            var main = DispatchProxy.Create<IMainService, ServiceProxy>();
            ((ServiceProxy)(object)main).Handler = (method, _) => method.Name switch
            {
                "get_VmNhanVien" => employeesVm, "get_VmThe" => cardsVm,
                _ => throw new Exception("No real caches, firmware commands or factory queries: " + method.Name)
            };
            var catalog = new SqlFixtureCatalog(main, connection);
            var employee = new NhanVienDaiThanh
            {
                MaNhanVien = "CATALOG_SQL_NV", MaHoSo = "CATALOG_SQL_HS", Name = "Nhân viên mới – kiểm thử SQL",
                Xuong = "1", IsHuman = true, IsNhom = true, MNgay = new DateTime(2026, 10, 27, 10, 0, 0)
            };
            var card = new TheTu
            {
                MaTheTu = "CATALOG_SQL_CARD", MaNhanVien = employee.MaNhanVien,
                NgayGio = employee.MNgay, PCName = "TEST_ONLY"
            };
            schema.Add(employee);
            schema.Add(card);
            await schema.SaveChangesAsync();
            await catalog.RefreshAsync();
            check(main.VmNhanVien.Items.Any(x => x.MaNhanVien == employee.MaNhanVien && x.Name == employee.Name && x.IsNhom && x.IsHuman),
                "Catalog SQL thật: đọc nhân viên mới, tên Unicode và cờ nhóm/nhân viên chính xác");
            check(main.VmThe.Items.Any(x => x.MaTheTu == card.MaTheTu && x.MaNhanVien == employee.MaNhanVien &&
                x.NhanVienName == employee.Name && x.MaSo == employee.MaHoSo),
                "Catalog SQL thật: JOIN nhân viên và mã thẻ đúng, đủ tên/mã hồ sơ cho Hub");
            var prior = main.VmThe.Items;
            await catalog.RefreshAsync();
            check(main.VmThe.Items.Count(x => x.MaTheTu == card.MaTheTu) == 1 && !ReferenceEquals(main.VmThe.Items, prior),
                "Catalog SQL thật: tải lại không nhân đôi thẻ và công bố collection hoàn chỉnh mới");
            employee.Name = "Tên đã sửa";
            employee.MaHoSo = "HS_DA_SUA";
            await schema.SaveChangesAsync();
            await catalog.RefreshAsync();
            check(main.VmThe.Items.Single(x => x.MaTheTu == card.MaTheTu).NhanVienName == "Tên đã sửa" &&
                main.VmThe.Items.Single(x => x.MaTheTu == card.MaTheTu).MaSo == "HS_DA_SUA",
                "Catalog SQL thật: sửa hồ sơ rồi tải lại, cache thẻ nhận đúng tên/mã hồ sơ mới");
            schema.Remove(card);
            await schema.SaveChangesAsync();
            await catalog.RefreshAsync();
            check(!main.VmThe.Items.Any(x => x.MaTheTu == card.MaTheTu) && main.VmNhanVien.Items.Any(x => x.MaNhanVien == employee.MaNhanVien),
                "Catalog SQL thật: xóa thẻ được cập nhật, không xóa hồ sơ nhân viên");
        }
        check(AppViewModels.Base.Ins.ConnectionString == oldDaoConnection,
            "Catalog SQL test không đổi cấu hình bộ nhớ, file cấu hình hoặc SQL xưởng");
    }

    private static object Cache<T>(string property, T rows)
    {
        var type = typeof(IMainService).GetProperty(property)!.PropertyType;
        var vm = RuntimeHelpers.GetUninitializedObject(type);
        type.GetProperty("Items")!.SetValue(vm, rows);
        return vm;
    }

    private sealed class SqlFixtureCatalog(IMainService main, string connection) : HubEmployeeCardCatalog(main)
    {
        protected override SqlConnection CreateConnection() => new(connection);
    }
}
