using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Models.Repos;
using Models.Repos.Models;
using Services;
using ViewModels.Repos.Hubs.IServices;

internal static class HubEmployeeManagerChecks
{
    public static async Task Run(DbContext schema, string connection, Action<bool, string> check)
    {
        var target = new SqlConnectionStringBuilder(connection);
        if (target.DataSource != @"(localdb)\MSSQLLocalDB" || !target.InitialCatalog.StartsWith("RCQ_AttendanceDiag_", StringComparison.Ordinal))
            throw new Exception("Employee CRUD tests require the owned LocalDB fixture.");
        var now = ScaleWorkshopHistory.BusinessNow();
        // The earlier compatibility test removed optional AvatarImg. Auth must not require that column.
        await schema.Database.ExecuteSqlRawAsync("""
            INSERT INTO dbo.NguoiDung (Id, UserName, Password, DefaultKey, IsActive, IsAdmin)
            VALUES (90001, 'EMP_OPERATOR_TEST', 'UNUSED', 'TEST', 1, 0),
                   (90002, 'EMP_LOCKED_TEST', 'UNUSED', 'TEST', 0, 0);
            """);
        schema.AddRange(new XiNghiep { Ma = "EMP_SCOPE_1", Ten = "Xưởng test 1", CodeId = "TEST", SuDung = true },
            new XiNghiep { Ma = "EMP_SCOPE_2", Ten = "Xưởng test 2", CodeId = "TEST", SuDung = true },
            new XiNghiep { Ma = "EMP_SCOPE_OFF", Ten = "Xưởng đã ngừng", CodeId = "TEST", SuDung = false });
        schema.AddRange(new NhanVienDaiThanh { MaNhanVien = "EMP_OTHER", MaHoSo = "OTHER", Name = "Ngoài xưởng", Xuong = "EMP_SCOPE_2", MNgay = now },
            new NhanVienDaiThanh { MaNhanVien = "EMP_UNASSIGNED", MaHoSo = "NONE", Name = "Chưa gán xưởng", MNgay = now });
        await schema.SaveChangesAsync();
        var session = new TestSession();
        session.Values["UserId"] = "90001";
        session.Values["XuongId"] = "EMP_SCOPE_1";
        var catalog = new TestCatalog();
        var log = new TestLog();
        var manager = new TestManager(session, catalog, log, connection, schema.Model);

        check(HubEmployeeEdit.SuggestCode(new[] { "000001", "ABC", "001009", "MÁY 1", "1.5", "-1" }) == "001010",
            "Nhân viên: gợi ý mã số từ mã số lớn nhất, không đổi mã chữ/nhóm");
        check(HubEmployeeEdit.SuggestCode(new[] { "99999999999999999999999999999999999999999999999999" }) == "",
            "Nhân viên: mã quá 50 ký tự không tràn số hoặc sinh mã không hợp lệ");
        var defaults = new HubEmployeeEdit();
        check(defaults.IsHuman && defaults.IsContracting && defaults.IsShowDinhMuc && defaults.LoaiSanLuong == 1 && !defaults.IsNhom,
            "Nhân viên: mặc định cùng form Báo Cáo Dev_Khuong");
        check(typeof(HubEmployeeEdit).GetProperty("Xuong") == null, "Form nhân viên không cho người dùng gửi xưởng khác để vượt phạm vi");

        var group = Form("EMP_NEW_GROUP");
        group.IsNhom = group.IsHuman = true;
        group.Name = "  MÁY CẮT thử nghiệm  ";
        group.MaHoSo = "  MAY CAT TEST  ";
        group.MaChamCong = " ";
        group.DeptName0 = " Nhóm thử ";
        var inserted = await manager.SaveAsync(group, true);
        Expect(inserted, "Tài khoản thường đang hoạt động được thêm hồ sơ trong xưởng đang chọn");
        var saved = await Employee(group.MaNhanVien);
        check(saved.Xuong == "EMP_SCOPE_1" && saved.IsNhom && saved.IsHuman && saved.Name == "MÁY CẮT thử nghiệm" &&
              saved.MaHoSo == "MAY CAT TEST" && saved.DeptName0 == "Nhóm thử" && saved.MaChamCong == null,
            "Nhân viên: lưu Unicode, giữ đồng thời IsHuman/IsNhom và chuẩn hóa khoảng trắng");
        check(catalog.Calls == 1 && saved.MNgay >= now && saved.MNgay <= ScaleWorkshopHistory.BusinessNow(),
            "Nhân viên: giờ cập nhật lấy phía server và làm mới catalog sau commit");

        foreach (var code in new[] { group.MaNhanVien, "EMP_OTHER" })
            check(!(await manager.SaveAsync(Form(code), true)).Success, "Nhân viên: chặn mã trùng trên toàn DB, kể cả khác xưởng: " + code);
        var callsBeforeValidation = catalog.Calls;
        foreach (var mode in new[] { "EMPTY_CODE", "EMPTY_NAME", "EMPTY_FILE", "LONG_CODE", "LONG_NAME", "LONG_CLOCK", "BAD_TYPE" })
        {
            var form = Form("EMP_INVALID");
            switch (mode)
            {
                case "EMPTY_CODE": form.MaNhanVien = " "; break;
                case "EMPTY_NAME": form.Name = " "; break;
                case "EMPTY_FILE": form.MaHoSo = " "; break;
                case "LONG_CODE": form.MaNhanVien = new string('X', 51); break;
                case "LONG_NAME": form.Name = new string('X', 101); break;
                case "LONG_CLOCK": form.MaChamCong = new string('X', 51); break;
                case "BAD_TYPE": form.LoaiSanLuong = 2; break;
            }
            check(!(await manager.SaveAsync(form, true)).Success, "Nhân viên: server chặn dữ liệu không hợp lệ: " + mode);
        }
        check(catalog.Calls == callsBeforeValidation && !await schema.Set<NhanVienDaiThanh>().AnyAsync(x => x.MaNhanVien == "EMP_INVALID"),
            "Nhân viên: lỗi nhập liệu không ghi SQL hoặc phát sinh tải catalog");
        var list = await manager.LoadAsync();
        var listingSql = schema.Set<NhanVienDaiThanh>().AsNoTracking()
            .Where(x => x.Xuong == "EMP_SCOPE_1").Select(HubEmployeeEdit.Projection).ToQueryString();
        check(listingSql.Contains("[MaNhanVien]") && listingSql.Contains("[IsNhom]") &&
              !listingSql.Contains("[Tel]") && !listingSql.Contains("[Address]") && !listingSql.Contains("[BirthDate]"),
            "Nhân viên: SQL chỉ tải trường hiển thị/form, không kéo metadata không cần thiết");
        check(list.WorkshopCode == "EMP_SCOPE_1" && list.Employees.Any(x => x.MaNhanVien == group.MaNhanVien) &&
              list.Employees.All(x => x.MaNhanVien is not "EMP_OTHER" and not "EMP_UNASSIGNED"),
            "Nhân viên: bảng chỉ hiện xưởng đang đăng nhập, không kéo hồ sơ khác xưởng/chưa gán xưởng");
        var other = await Employee("EMP_OTHER");
        check(!(await manager.SaveAsync(HubEmployeeEdit.From(other), false, other.MaNhanVien)).Success &&
              !(await manager.DeleteAsync(other.MaNhanVien, other.MNgay)).Success,
            "Nhân viên: kiểm tra xưởng trên server cho cả Sửa và Xóa");

        foreach (var (user, workshop) in new[] { ("-1", "EMP_SCOPE_1"), ("missing", "EMP_SCOPE_1"),
            ("90002", "EMP_SCOPE_1"), ("90001", "EMP_SCOPE_OFF"), ("90001", "MISSING"), ("90001", "") })
        {
            session.Values["UserId"] = user;
            session.Values["XuongId"] = workshop;
            check(!(await manager.SaveAsync(Form("EMP_DENIED"), true)).Success &&
                  !(await manager.DeleteAsync(saved.MaNhanVien, saved.MNgay)).Success,
                $"Nhân viên: chặn phiên/tài khoản/xưởng không hợp lệ: {user}/{workshop}");
            try { await manager.LoadAsync(); throw new Exception("Unauthorized load was allowed"); }
            catch (UnauthorizedAccessException) { check(true, "Nhân viên: đọc danh sách cũng cần phiên và xưởng hợp lệ"); }
        }
        session.Values["UserId"] = "90001";
        session.Values["XuongId"] = "EMP_SCOPE_1";

        // Unmapped factory columns and fields absent from the form must survive updates.
        await schema.Database.ExecuteSqlRawAsync("ALTER TABLE dbo.NhanVienDaiThanh ADD IsThuKho bit NOT NULL CONSTRAINT DF_TEST_EMP_ThuKho DEFAULT(0);");
        await schema.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.NhanVienDaiThanh SET IsThuKho=1, Tel=N'090_TEST', Address=N'Giữ địa chỉ', BirthDate=N'2000-01-01' WHERE MaNhanVien={saved.MaNhanVien}");
        var edit = HubEmployeeEdit.From(await Employee(saved.MaNhanVien));
        edit.Name = "Tên đã sửa trên Hub";
        edit.IsPhucVu = edit.IsBanKiem = edit.IsGiaCong = edit.IsChucNang = edit.IsNhanVienCat = true;
        edit.IsContracting = edit.IsShowDinhMuc = false;
        edit.LoaiSanLuong = -1;
        Expect(await manager.SaveAsync(edit, false, edit.MaNhanVien), "Sửa hồ sơ và ghi nhật ký đồng bộ cùng transaction");
        var updated = await Employee(edit.MaNhanVien);
        var updateJournal = await schema.Set<HQ_NhanVien_U>().AsNoTracking().SingleAsync(x => x.MaNhanVien == edit.MaNhanVien);
        check(updated.Name == edit.Name && updated.Tel == "090_TEST" && updated.Address == "Giữ địa chỉ" && updated.BirthDate == "2000-01-01" &&
              updated.IsNhom && updated.IsHuman && updated.IsPhucVu && updated.IsBanKiem && updated.IsGiaCong && updated.IsChucNang &&
              updated.IsNhanVienCat && !updated.IsContracting && !updated.IsShowDinhMuc && updated.LoaiSanLuong == -1,
            "Nhân viên: sửa đúng các cờ, không làm trắng metadata ngoài form");
        check(updateJournal.Name == updated.Name && updateJournal.Tel == updated.Tel && updateJournal.IsNhom &&
              updateJournal.MNgay == updated.MNgay && updateJournal.Ngay >= now,
            "Nhân viên: HQ_NhanVien_U đủ metadata và mốc ngày để firmware tải thay đổi");
        await using (var sql = new SqlConnection(connection))
        {
            await sql.OpenAsync();
            using var command = sql.CreateCommand();
            command.CommandText = "SELECT CAST(IsThuKho AS int) FROM dbo.NhanVienDaiThanh WHERE MaNhanVien=@id";
            command.Parameters.AddWithValue("@id", edit.MaNhanVien);
            check((int)(await command.ExecuteScalarAsync())! == 1, "Nhân viên: cột IsThuKho không có trong model Hub vẫn giữ nguyên");
        }
        check(!(await manager.SaveAsync(edit, false, edit.MaNhanVien)).Success && !(await manager.DeleteAsync(edit.MaNhanVien, edit.ModifiedAt)).Success,
            "Nhân viên: chặn ghi đè/xóa bằng phiên bản hồ sơ cũ");
        var changedKey = HubEmployeeEdit.From(updated);
        changedKey.MaNhanVien = "EMP_CHANGED_KEY";
        check(!(await manager.SaveAsync(changedKey, false, updated.MaNhanVien)).Success,
            "Nhân viên: server không cho đổi khóa mã nhân viên dù sửa payload");

        await schema.Database.ExecuteSqlRawAsync("ALTER TABLE dbo.HQ_NhanVien_U ADD CONSTRAINT CK_TEST_EMP_ROLLBACK CHECK (Name <> N'ROLLBACK_CHECK');");
        var rollback = HubEmployeeEdit.From(updated);
        rollback.Name = "ROLLBACK_CHECK";
        var journalsBefore = await schema.Set<HQ_NhanVien_U>().CountAsync();
        check(!(await manager.SaveAsync(rollback, false, rollback.MaNhanVien)).Success &&
              (await Employee(updated.MaNhanVien)).Name == updated.Name && await schema.Set<HQ_NhanVien_U>().CountAsync() == journalsBefore,
            "Nhân viên: lỗi ghi nhật ký phải rollback cả hồ sơ, không lưu nửa transaction");

        var raceA = HubEmployeeEdit.From(await Employee(updated.MaNhanVien));
        var raceB = raceA.Copy();
        raceA.Name = "Người sửa A";
        raceB.Name = "Người sửa B";
        var racing = await Task.WhenAll(manager.SaveAsync(raceA, false, raceA.MaNhanVien), manager.SaveAsync(raceB, false, raceB.MaNhanVien));
        check(racing.Count(x => x.Success) == 1 && await schema.Set<HQ_NhanVien_U>().CountAsync() == journalsBefore + 1,
            "Nhân viên: hai người sửa cùng phiên bản chỉ một lần được lưu, lần còn lại phải tải lại");
        var duplicateRace = await Task.WhenAll(manager.SaveAsync(Form("EMP_RACE_INSERT"), true), manager.SaveAsync(Form("EMP_RACE_INSERT"), true));
        check(duplicateRace.Count(x => x.Success) == 1 && await schema.Set<NhanVienDaiThanh>().CountAsync(x => x.MaNhanVien == "EMP_RACE_INSERT") == 1,
            "Nhân viên: hai lần Thêm đồng thời cùng mã chỉ tạo một hồ sơ");

        foreach (var reference in new[] { "ACCOUNT", "CARD", "OLD_CARD", "WEIGHT", "SERVICE_WEIGHT", "INSPECT_WEIGHT", "WORKSHOP", "SCALE", "CLOCK_CODE" })
        {
            var code = "EMP_REF_" + reference;
            var form = Form(code);
            form.MaChamCong = "CLOCK_" + reference;
            Expect(await manager.SaveAsync(form, true), "Tạo fixture tham chiếu " + reference);
            switch (reference)
            {
                case "ACCOUNT":
                    await schema.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO dbo.NguoiDung (Id, UserName, Password, DefaultKey, IsActive, IsAdmin, MaNhanVien) VALUES (90003, 'EMP_LINKED_TEST', 'UNUSED', 'TEST', 1, 0, {code})");
                    break;
                case "CARD": schema.Add(new TheTu { MaTheTu = "CARD_EMP_REF", MaNhanVien = code, PCName = "TEST", NgayGio = now }); break;
                case "OLD_CARD": schema.Add(new HQ_TheTu_D { MaTheTu = "OLD_CARD_EMP_REF", MaNhanVien = code, PCName = "TEST", Ngay = now, NgayGio = now }); break;
                case "WEIGHT": case "SERVICE_WEIGHT": case "INSPECT_WEIGHT":
                    schema.Add(new HQ_PhieuCan { Id = code, MayCan = "TEST", MaLo = "TEST", MaSize = "TEST", MaThanhPham = "TEST",
                        MaLoaiNguyenLieu = "TEST", TheId = "TEST", TheIdNhanVien = "TEST", MaXuong = "EMP_SCOPE_1",
                        MaNhanVien = reference == "WEIGHT" ? code : "OTHER",
                        MaNhanVienPhucVu = reference == "SERVICE_WEIGHT" ? code : "",
                        MaNhanVienBanKiem = reference == "INSPECT_WEIGHT" ? code : "" }); break;
                case "WORKSHOP": schema.Add(new WorkshopAttendance { Id = code, MaNhanVien = code, MaTheTu = "TEST", ThietBi = "TEST", ThoiGian = now }); break;
                case "SCALE": case "CLOCK_CODE": schema.Add(new CheckInOut { Id = code,
                    MaChamCong = reference == "SCALE" ? code : form.MaChamCong!, TenMay = "TEST", ThoiGian = now }); break;
            }
            await schema.SaveChangesAsync();
            var current = await Employee(code);
            var linkedBefore = await LinkedDataAsync(reference);
            var refreshBefore = catalog.Calls;
            Expect(await manager.DeleteAsync(code, current.MNgay), "Xóa hồ sơ có tham chiếu như Báo Cáo: " + reference);
            check(!await schema.Set<NhanVienDaiThanh>().AnyAsync(x => x.MaNhanVien == code) &&
                  await schema.Set<HQ_NhanVien_D>().CountAsync(x => x.MaNhanVien == code && x.Name == current.Name) == 1 &&
                  await LinkedDataAsync(reference) == linkedBefore && catalog.Calls == refreshBefore + 1,
                "Nhân viên: chỉ xóa hồ sơ, ghi journal và tải catalog; giữ nguyên dữ liệu liên quan: " + reference);
        }
        var unusedForm = Form("EMP_UNUSED_DELETE");
        Expect(await manager.SaveAsync(unusedForm, true), "Tạo hồ sơ chưa dùng để kiểm tra Xóa");
        var unused = await Employee(unusedForm.MaNhanVien);
        Expect(await manager.DeleteAsync(unused.MaNhanVien, unused.MNgay), "Xóa hồ sơ chưa dùng sau xác nhận");
        check(!await schema.Set<NhanVienDaiThanh>().AnyAsync(x => x.MaNhanVien == unused.MaNhanVien) &&
              await schema.Set<HQ_NhanVien_D>().AnyAsync(x => x.MaNhanVien == unused.MaNhanVien && x.Name == unused.Name),
            "Nhân viên: xóa thành công phải ghi HQ_NhanVien_D để cân tải danh sách xóa");

        var deleteRollback = Form("EMP_DELETE_ROLLBACK");
        deleteRollback.Name = "DELETE_ROLLBACK_CHECK";
        Expect(await manager.SaveAsync(deleteRollback, true), "Tạo fixture lỗi journal Xóa");
        await schema.Database.ExecuteSqlRawAsync("ALTER TABLE dbo.HQ_NhanVien_D ADD CONSTRAINT CK_TEST_EMP_DELETE_ROLLBACK CHECK (Name <> N'DELETE_ROLLBACK_CHECK');");
        var rollbackEmployee = await Employee(deleteRollback.MaNhanVien);
        var deletesBefore = await schema.Set<HQ_NhanVien_D>().CountAsync();
        var refreshBeforeFailure = catalog.Calls;
        check(!(await manager.DeleteAsync(rollbackEmployee.MaNhanVien, rollbackEmployee.MNgay)).Success &&
              await schema.Set<NhanVienDaiThanh>().AnyAsync(x => x.MaNhanVien == rollbackEmployee.MaNhanVien) &&
              await schema.Set<HQ_NhanVien_D>().CountAsync() == deletesBefore && catalog.Calls == refreshBeforeFailure,
            "Nhân viên: journal Xóa lỗi phải rollback hồ sơ và không làm mới catalog");

        Expect(await manager.SaveAsync(Form("EMP_DELETE_FK"), true), "Tạo fixture ràng buộc FK");
        await schema.Database.ExecuteSqlRawAsync("""
            CREATE TABLE dbo.EmployeeDeleteReference (
                MaNhanVien varchar(50) NOT NULL PRIMARY KEY,
                CONSTRAINT FK_TEST_EMP_DELETE FOREIGN KEY (MaNhanVien) REFERENCES dbo.NhanVienDaiThanh(MaNhanVien));
            INSERT INTO dbo.EmployeeDeleteReference (MaNhanVien) VALUES ('EMP_DELETE_FK');
            """);
        var constrained = await Employee("EMP_DELETE_FK");
        var fkResult = await manager.DeleteAsync(constrained.MaNhanVien, constrained.MNgay);
        check(!fkResult.Success && fkResult.Message.StartsWith("SQL không cho xóa") &&
              await schema.Set<NhanVienDaiThanh>().AnyAsync(x => x.MaNhanVien == constrained.MaNhanVien) &&
              !await schema.Set<HQ_NhanVien_D>().AnyAsync(x => x.MaNhanVien == constrained.MaNhanVien),
            "Nhân viên: tôn trọng FK thật của SQL, thông báo rõ và rollback journal, không tự gỡ ràng buộc");

        Expect(await manager.SaveAsync(Form("EMP_DELETE_RACE"), true), "Tạo fixture hai yêu cầu Xóa đồng thời");
        var deleteRace = await Employee("EMP_DELETE_RACE");
        var deletes = await Task.WhenAll(manager.DeleteAsync(deleteRace.MaNhanVien, deleteRace.MNgay),
            manager.DeleteAsync(deleteRace.MaNhanVien, deleteRace.MNgay));
        check(deletes.Count(x => x.Success) == 1 &&
              await schema.Set<HQ_NhanVien_D>().CountAsync(x => x.MaNhanVien == deleteRace.MaNhanVien) == 1,
            "Nhân viên: hai yêu cầu Xóa cùng hồ sơ chỉ commit một lần và một journal");

        catalog.Fail = true;
        var committed = await manager.SaveAsync(Form("EMP_REFRESH_FAILED"), true);
        check(committed.Success && committed.CatalogRefreshFailed && committed.Message.StartsWith("Đã thêm") &&
              await schema.Set<NhanVienDaiThanh>().AnyAsync(x => x.MaNhanVien == "EMP_REFRESH_FAILED"),
            "Nhân viên: SQL đã commit nhưng tải cache lỗi vẫn báo Đã thêm, không lưu lại lần nữa");
        var refreshFailedEmployee = await Employee("EMP_REFRESH_FAILED");
        var deletedWithRefreshFailure = await manager.DeleteAsync(refreshFailedEmployee.MaNhanVien, refreshFailedEmployee.MNgay);
        check(deletedWithRefreshFailure.Success && deletedWithRefreshFailure.CatalogRefreshFailed &&
              deletedWithRefreshFailure.Message.StartsWith("Đã xóa") &&
              !await schema.Set<NhanVienDaiThanh>().AnyAsync(x => x.MaNhanVien == refreshFailedEmployee.MaNhanVien) &&
              await schema.Set<HQ_NhanVien_D>().CountAsync(x => x.MaNhanVien == refreshFailedEmployee.MaNhanVien) == 1,
            "Nhân viên: Xóa đã commit nhưng cache lỗi vẫn báo đúng Đã xóa kèm cảnh báo");
        catalog.Fail = false;

        await HubEmployeeImportChecks.Run(schema, manager, session.Values, () => catalog.Calls, value => catalog.Fail = value, check);

        async Task<string> LinkedDataAsync(string reference)
        {
            // Only fixed fixture identifiers enter SQL; compare every stored field before/after deletion.
            var (table, key) = reference switch
            {
                "ACCOUNT" => ("NguoiDung", "Id"),
                "CARD" => ("TheTu", "MaTheTu"),
                "OLD_CARD" => ("HQ_TheTu_D", "Id"),
                "WEIGHT" or "SERVICE_WEIGHT" or "INSPECT_WEIGHT" => ("HQ_PhieuCan", "Id"),
                "WORKSHOP" => ("ChamCongTaiXuong", "Id"),
                "SCALE" or "CLOCK_CODE" => ("CheckInOut", "Id"),
                _ => throw new ArgumentOutOfRangeException(nameof(reference))
            };
            await using var sql = new SqlConnection(connection);
            await sql.OpenAsync();
            using var command = sql.CreateCommand();
            command.CommandText = $"SELECT * FROM dbo.[{table}] ORDER BY [{key}] FOR JSON PATH, INCLUDE_NULL_VALUES";
            using var reader = await command.ExecuteReaderAsync();
            var json = new System.Text.StringBuilder();
            while (await reader.ReadAsync()) json.Append(reader.GetString(0));
            return json.ToString();
        }

        Task<NhanVienDaiThanh> Employee(string code) => schema.Set<NhanVienDaiThanh>().AsNoTracking().SingleAsync(x => x.MaNhanVien == code);
        void Expect(HubEmployeeResult result, string label) => check(result.Success,
            label + (result.Success ? "" : ": " + result.Message + " / " + log.LastError?.GetBaseException().Message));
    }

    private static HubEmployeeEdit Form(string code) => new() { MaNhanVien = code, MaHoSo = code, Name = "Nhân viên thử nghiệm" };
    private sealed class TestManager(ISessionService session, IHubEmployeeCardCatalog catalog, ILogger<HubEmployeeManager> logger,
        string connection, IModel model) : HubEmployeeManager(session, catalog, logger)
    { protected override dbPMScontext CreateDb() => new TestDb(connection, model); }
    private sealed class TestSession : ISessionService
    {
        public Dictionary<string, string> Values { get; } = new();
        public string? Get(string key) => Values.GetValueOrDefault(key);
        public void Set(string key, string value) => Values[key] = value;
    }
    private sealed class TestCatalog : IHubEmployeeCardCatalog
    {
        private int calls;
        public int Calls => calls;
        public bool Fail { get; set; }
        public Task RefreshAsync(CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref calls);
            return Fail ? Task.FromException(new IOException("Fixture catalog failure")) : Task.CompletedTask;
        }
    }
    private sealed class TestLog : ILogger<HubEmployeeManager>
    {
        public Exception? LastError { get; private set; }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) => LastError = exception;
    }
}
