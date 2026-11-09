using Microsoft.EntityFrameworkCore;
using Models.Repos.Models;
using Services;
using ViewModels.Repos.Staff;

internal static class HubEmployeeImportChecks
{
    public static async Task Run(DbContext schema, HubEmployeeManager manager,
        Dictionary<string, string> session, Func<int> refreshCalls, Action<bool> failRefresh,
        Action<bool, string> check)
    {
        const string scope = "EMP_SCOPE_1";
        NhanVienImportRow Row(string code, string? file = null, string name = "Nhân viên Excel") =>
            new() { RowNumber = 2, MaNhanVien = code, MaHoSo = file ?? code, Name = name, Status = "Ready" };
        Task<NhanVienImportResult> Import(NhanVienImportRow[] rows, bool preview = false) => manager.ImportAsync(rows, scope, preview);
        Task<int> Count() => schema.Set<NhanVienDaiThanh>().CountAsync();
        var before = await Count();
        var refreshBefore = refreshCalls();
        var rows = new[] { Row("IMP_001", " 000123 ", " Nguyễn Văn A "), Row("IMP_002") };
        var preview = await Import(rows, true);
        check(preview.IsSuccess && preview.ReadyCount == 2 && await Count() == before && refreshCalls() == refreshBefore,
            "Import Hub: xem trước không ghi SQL hoặc làm mới cache");
        var saved = await Import(rows);
        check(saved.IsSuccess && saved.ImportedCount == 2 && saved.ReadyCount == 0 && await Count() == before + 2 &&
              saved.Rows.All(row => row.Status == "Imported") && refreshCalls() == refreshBefore + 1,
            "Import Hub: lưu cả lô và làm mới catalog đúng một lần sau commit");
        var employee = await schema.Set<NhanVienDaiThanh>().AsNoTracking().SingleAsync(x => x.MaNhanVien == "IMP_001");
        check(employee.Xuong == scope && employee.MaHoSo == "000123" && employee.Name == "Nguyễn Văn A" &&
              employee.IsHuman && employee.IsContracting && employee.IsShowDinhMuc && !employee.IsNhom && employee.LoaiSanLuong == 1 &&
              employee.FirstWorkingDate == "Đang Làm" && employee.MNgay <= ScaleWorkshopHistory.BusinessNow(),
            "Import Hub: giữ số 0, Unicode, xưởng phía server và mặc định như Báo Cáo");
        rows[0].Name = "Không được ghi đè";
        var repeated = await Import(rows);
        check(repeated.IsSuccess && repeated.SkippedCount == 2 && repeated.ImportedCount == 0 &&
              refreshCalls() == refreshBefore + 1 && (await schema.Set<NhanVienDaiThanh>().AsNoTracking().SingleAsync(x => x.MaNhanVien == "IMP_001")).Name == employee.Name,
            "Import Hub: nhập lại bỏ qua hồ sơ cũ, không ghi đè hoặc tăng tổng");
        check(!(await Import(new[] { Row("IMP_VALID"), Row("IMP_BAD", name: "") })).IsSuccess &&
              !await schema.Set<NhanVienDaiThanh>().AnyAsync(x => x.MaNhanVien == "IMP_VALID"),
            "Import Hub: dù giả trạng thái Ready, một dòng sai ngăn toàn lô");
        check(!(await Import(new[] { Row("IMP_FILE_CONFLICT", "000123") })).IsSuccess,
            "Import Hub: chặn mã hồ sơ đã dùng");
        check(!(await Import(new[] { Row("IMP_DUP"), Row("imp_dup") })).IsSuccess &&
              !(await Import(new[] { Row("IMP_D1", "SAME"), Row("IMP_D2", "same") })).IsSuccess,
            "Import Hub: chặn mã nhân viên/hồ sơ trùng trong file không phân biệt hoa thường");
        check((await Import(new[] { Row("EMP_OTHER") })).SkippedCount == 1,
            "Import Hub: mã đã tồn tại khác xưởng cũng bỏ qua, không di chuyển nhân viên");
        check((await Import(new[] { Row("IMP_LATE") }, true)).ReadyCount == 1, "Import Hub: fixture xem trước trước thay đổi đồng thời");
        await manager.SaveAsync(new() { MaNhanVien = "IMP_LATE", MaHoSo = "IMP_LATE", Name = "Đã tạo từ màn khác" }, true);
        check((await Import(new[] { Row("IMP_LATE") })).SkippedCount == 1,
            "Import Hub: lúc lưu kiểm tra lại hồ sơ phát sinh sau xem trước");

        var baseline = await Count();
        session["XuongId"] = "EMP_SCOPE_2";
        check(!(await Import(new[] { Row("IMP_SCOPE") })).IsSuccess && await Count() == baseline,
            "Import Hub: đổi xưởng sau xem trước không lưu sang xưởng mới");
        session["XuongId"] = scope;
        session["UserId"] = "90002";
        check(!(await Import(new[] { Row("IMP_LOCKED") })).IsSuccess && await Count() == baseline,
            "Import Hub: tài khoản bị khóa không nhập được");
        session["UserId"] = "";
        check(!(await Import(new[] { Row("IMP_NO_SESSION") })).IsSuccess, "Import Hub: thiếu phiên đăng nhập bị từ chối");
        session["UserId"] = "90001";
        check(!(await Import(Array.Empty<NhanVienImportRow>())).IsSuccess &&
              !(await Import(Enumerable.Range(0, 1001).Select(i => Row("IMP_MAX_" + i)).ToArray())).IsSuccess,
            "Import Hub: giới hạn tối đa 1000 dòng và không nhận danh sách rỗng");

        await schema.Database.ExecuteSqlRawAsync("ALTER TABLE dbo.NhanVienDaiThanh ADD CONSTRAINT CK_TEST_EMP_IMPORT CHECK (Name <> N'IMPORT_ROLLBACK');");
        var calls = refreshCalls();
        check(!(await Import(new[] { Row("IMP_AOK"), Row("IMP_ZFAIL", name: "IMPORT_ROLLBACK") })).IsSuccess &&
              !await schema.Set<NhanVienDaiThanh>().AnyAsync(x => x.MaNhanVien == "IMP_AOK" || x.MaNhanVien == "IMP_ZFAIL") && refreshCalls() == calls,
            "Import Hub: lỗi SQL rollback toàn bộ lô và không phát sinh refresh");
        var concurrent = await Task.WhenAll(Import(new[] { Row("IMP_RACE") }), Import(new[] { Row("IMP_RACE") }));
        check(concurrent.Any(result => result.ImportedCount == 1) &&
              await schema.Set<NhanVienDaiThanh>().CountAsync(x => x.MaNhanVien == "IMP_RACE") == 1,
            "Import Hub: hai lượt nhập đồng thời chỉ có một hồ sơ");
        failRefresh(true);
        var cacheFailure = await Import(new[] { Row("IMP_CACHE") });
        check(cacheFailure.IsSuccess && cacheFailure.ImportedCount == 1 && cacheFailure.Messages.StartsWith("Đã thêm") &&
              cacheFailure.Messages.Contains("Chưa cập nhật") && await schema.Set<NhanVienDaiThanh>().AnyAsync(x => x.MaNhanVien == "IMP_CACHE"),
            "Import Hub: cache lỗi sau commit vẫn xác nhận đúng đã lưu");
        failRefresh(false);
        var thousand = await Import(Enumerable.Range(0, 1000).Select(i => Row("IMP_BULK_" + i)).ToArray());
        check(thousand.IsSuccess && thousand.ImportedCount == 1000 &&
              await schema.Set<NhanVienDaiThanh>().CountAsync(x => x.MaNhanVien.StartsWith("IMP_BULK_")) == 1000,
            "Import Hub: lưu đủ lô tối đa 1000 nhân viên");
    }
}
