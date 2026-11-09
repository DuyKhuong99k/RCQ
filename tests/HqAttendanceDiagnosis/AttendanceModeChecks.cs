using System.Collections.Concurrent;
using System.Data.Common;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Models.Repos;
using Models.Repos.Models;
using MvvmHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMSHub.Components.Shared;
using PMSHub.Controllers;
using Security.Crypt;
using Services;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;

internal static class AttendanceModeChecks
{
    public static async Task<int> RunAsync(DbContext schema, IMayCansService machines, IMainService main,
        IUserService users, MayCan machine, ObservableRangeCollection<TheTu> cards,
        ObservableRangeCollection<NhanVienDaiThanh> employees)
    {
        var checks = 0;
        void Check(bool value, string label)
        {
            if (!value) throw new Exception("FAIL: Attendance mode: " + label);
            checks++;
            Console.WriteLine("PASS: Attendance mode: " + label);
        }
        var folder = Path.Combine(Path.GetTempPath(), "RCQ_AttendanceModeTest_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, "mode.json");
        var corruptPath = Path.Combine(folder, "corrupt.json");
        var blockedPath = Path.Combine(folder, "blocked-parent");
        var racingPath = Path.Combine(folder, "racing.json");
        try
        {
            var store = new AttendanceModeStore(path);
            Check(store.Current.Enabled && store.Current.Revision == 0 && !File.Exists(path),
                "missing settings default to ON without silently creating operator state");

            const string password = "test-only-mode-password";
            var key = (string)typeof(LoginService).GetField("_defaultKey", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
            var admin = new NguoiDung { Id = 91001, UserName = "MODE_ADMIN", IsActive = true, IsAdmin = true,
                Password = ED.EncryptString(password, key), DefaultKey = "test-only" };
            schema.Add(admin);
            await schema.SaveChangesAsync();
            schema.Database.ExecuteSqlRaw("ALTER TABLE dbo.NguoiDung DROP COLUMN AvatarImg");
            var verifier = new SqlAttendanceAdministratorVerifier();
            Check(await verifier.VerifyAsync(admin.UserName, password, default) is { Id: 91001 },
                "SQL verifier accepts active admin using only needed columns, even without legacy optional AvatarImg");
            Check(await verifier.VerifyAsync(admin.UserName, "wrong", default) == null,
                "wrong password rejected");
            Check(await verifier.VerifyAsync("UNKNOWN_MODE_USER", password, default) == null,
                "unknown account rejected");
            foreach (var active in new bool?[] { false, null })
            {
                admin.IsActive = active;
                await schema.SaveChangesAsync();
                Check(await verifier.VerifyAsync(admin.UserName, password, default) == null,
                    "locked/unconfirmed account rejected using fresh SQL, no login cache");
            }
            admin.IsActive = true;
            admin.IsAdmin = false;
            await schema.SaveChangesAsync();
            Check(await verifier.VerifyAsync(admin.UserName, password, default) == null,
                "normal active user cannot change a global mode");
            admin.IsAdmin = null;
            await schema.SaveChangesAsync();
            Check(await verifier.VerifyAsync(admin.UserName, password, default) == null,
                "NULL admin permission is not treated as an administrator");
            admin.IsAdmin = true;
            await schema.SaveChangesAsync();

            var badManager = new AttendanceModeManager(store, verifier);
            var bad = await badManager.ChangeAsync(false, 0, admin.UserName, "wrong");
            Check(!bad.Success && store.Current.Enabled && !File.Exists(path),
                "failed confirmation never changes memory or disk");
            Check(!(await badManager.ChangeAsync(false, 0, admin.UserName, password)).Success && store.Current.Enabled,
                "failed credentials trigger a short confirmation cooldown");
            var manager = new AttendanceModeManager(store, verifier);
            var changed = 0;
            store.Changed += () => changed++;
            store.Changed += () => throw new IOException("Test-only disconnected page");
            Check((await manager.ChangeAsync(false, 0, admin.UserName, password)).Success && !store.Current.Enabled &&
                store.Current.Revision == 1 && changed == 1,
                "verified admin disables mode and a faulty page does not undo committed state");
            Check(store.Current.ChangedById == admin.Id && store.Current.ChangedBy == admin.UserName &&
                store.Current.ChangedAtUtc != null,
                "last change has persisted admin/time attribution");
            var disk = File.ReadAllText(path);
            Check(!disk.Contains(password) && !disk.Contains(admin.Password) && !disk.Contains("Password"),
                "private state contains no password or encrypted credential");
            Check(!new AttendanceModeStore(path).Current.Enabled && new AttendanceModeStore(path).Current.Revision == 1,
                "server restart reloads OFF, not the default ON");
            await using (var services = new ServiceCollection().AddLogging()
                .AddSingleton<IAttendanceMode>(store).AddSingleton(manager)
                .AddSingleton(Proxy<ILoginService>((_, _) => "" )).BuildServiceProvider())
            await using (var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>()))
            {
                var html = await renderer.Dispatcher.InvokeAsync(async () =>
                    (await renderer.RenderComponentAsync<AttendanceModePanel>()).ToHtmlString());
                html = System.Net.WebUtility.HtmlDecode(html);
                Check(html.Contains("ĐANG TẮT") && html.Contains("Bật chấm công") && !html.Contains(password),
                    "real rendered control shows shared OFF state and no password");
            }
            Check(!(await manager.ChangeAsync(true, 0, admin.UserName, password)).Success && !store.Current.Enabled,
                "stale confirmation cannot overwrite another operator change");
            Check(!(await manager.ChangeAsync(true, 1, admin.UserName, "wrong")).Success && !store.Current.Enabled,
                "turning ON also requires valid re-authentication");
            manager = new AttendanceModeManager(store, verifier); // independent fixture after cooldown assertion
            Check((await manager.ChangeAsync(true, 1, admin.UserName, password)).Success &&
                new AttendanceModeStore(path).Current.Enabled,
                "correctly re-authenticated ON is persisted");
            Check(!(await manager.ChangeAsync(false, store.Current.Revision, admin.UserName, password,
                new CancellationToken(true))).Success && store.Current.Enabled,
                "cancelled confirmation leaves mode unchanged");

            File.WriteAllText(corruptPath, "{broken}");
            var corruptRejected = false;
            try { _ = new AttendanceModeStore(corruptPath); }
            catch (InvalidOperationException) { corruptRejected = true; }
            Check(corruptRejected, "corrupt persisted settings fail closed instead of silently disabling attendance");
            File.WriteAllText(corruptPath, "{\"Revision\":1,\"ChangedById\":1,\"ChangedBy\":\"admin\",\"ChangedAtUtc\":\"2026-10-27T00:00:00Z\"}");
            corruptRejected = false;
            try { _ = new AttendanceModeStore(corruptPath); }
            catch (InvalidOperationException) { corruptRejected = true; }
            Check(corruptRejected, "missing Enabled field cannot become an implicit OFF");
            File.WriteAllText(blockedPath, "test-only-file-not-directory");
            var blockedStore = new AttendanceModeStore(Path.Combine(blockedPath, "mode.json"));
            var blockedManager = new AttendanceModeManager(blockedStore, verifier);
            Check(!(await blockedManager.ChangeAsync(false, 0, admin.UserName, password)).Success && blockedStore.Current.Enabled,
                "disk write failure never changes the live mode");
            var racingStore = new AttendanceModeStore(racingPath);
            var racingManagers = new[] { new AttendanceModeManager(racingStore, verifier), new AttendanceModeManager(racingStore, verifier) };
            var race = await Task.WhenAll(racingManagers.Select(control => control.ChangeAsync(false, 0, admin.UserName, password)));
            Check(race.Count(result => result.Success) == 1 && !racingStore.Current.Enabled && racingStore.Current.Revision == 1,
                "concurrent confirmations use revision CAS and persist exactly one change");
            Check(!new AttendanceModeStore(racingPath).Current.Enabled && new AttendanceModeStore(racingPath).Current.Revision == 1,
                "concurrent change reloads as one complete valid document");

            // Actual Blazor handler: it must clear the password, and may only call the authenticated manager.
            var panel = new AttendanceModePanel();
            Set(panel, "Manager", manager); Set(panel, "Mode", store);
            Set(panel, "editing", true); Set(panel, "desired", false);
            Set(panel, "expectedRevision", store.Current.Revision);
            Set(panel, "userName", admin.UserName); Set(panel, "password", password);
            await (Task)typeof(AttendanceModePanel).GetMethod("Confirm", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(panel, null)!;
            Check(!store.Current.Enabled && Get<string>(panel, "password") == "" && !Get<bool>(panel, "editing"),
                "UI confirmation authenticates and clears password after success");
            Set(panel, "editing", true); Set(panel, "password", password);
            typeof(AttendanceModePanel).GetMethod("Cancel", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(panel, null);
            Check(Get<string>(panel, "password") == "" && !Get<bool>(panel, "editing"),
                "cancelling the UI also clears password");
            panel.Dispose();
            var badPanel = new AttendanceModePanel();
            Set(badPanel, "Manager", new AttendanceModeManager(store, verifier)); Set(badPanel, "Mode", store);
            Set(badPanel, "editing", true); Set(badPanel, "desired", true);
            Set(badPanel, "expectedRevision", store.Current.Revision);
            Set(badPanel, "userName", admin.UserName); Set(badPanel, "password", "wrong");
            await (Task)typeof(AttendanceModePanel).GetMethod("Confirm", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(badPanel, null)!;
            Check(!store.Current.Enabled && Get<bool>(badPanel, "editing") && Get<string>(badPanel, "password") == "",
                "UI wrong password cannot change mode and is cleared before retry");
            badPanel.Dispose();

            var card = new TheTu { MaTheTu = "0009160001", MaNhanVien = "MODE_PRODUCTION" };
            var employee = new NhanVienDaiThanh { MaNhanVien = card.MaNhanVien, MaHoSo = card.MaNhanVien,
                Name = "Mode production test", IsHuman = true };
            schema.AddRange(card, employee);
            foreach (var entry in schema.ChangeTracker.Entries().Where(x => x.State == EntityState.Added))
                foreach (var property in entry.Properties.Where(x => x.Metadata.ClrType == typeof(string) &&
                    !x.Metadata.IsNullable && x.CurrentValue == null)) property.CurrentValue = "";
            await schema.SaveChangesAsync();
            cards.Add(card); employees.Add(employee);
            var spy = new AttendanceSqlSpy();
            var commit = new ModeCommit(machines, main, schema, store, spy);
            var router = new CommunicationService(users, commit, Options.Create(new ScaleAttendanceOptions()), machines,
                attendanceMode: store);
            var client = new ClientInfo { Id = machine.Id, WKv = AppKV.Hq, IsActive = true, IsMayCan = true };
            var start = ScaleWorkshopHistory.BusinessNow().AddMinutes(-1);
            start = start.AddTicks(-(start.Ticks % TimeSpan.TicksPerSecond));
            async Task<JObject> Weigh(int seconds, string state = "ST", decimal kg = 1.4m)
            {
                var time = start.AddSeconds(seconds);
                var json = JsonConvert.SerializeObject(new DataLite { Id = machine.Id + "." + time.ToString("yyyyMMddHHmmss"),
                    NgayGio = time.ToString("yyyyMMddHHmmss"), TheId = card.MaTheTu, Stated = state, TrongLuong = kg });
                return JObject.Parse(await router.Route(client, "LITE", json.Length, json));
            }
            async Task<JObject> Scan(int seconds, string direction)
            {
                var time = start.AddSeconds(seconds);
                var json = JsonConvert.SerializeObject(new ScaleAttendanceProcessor.Request { TheId = card.MaTheTu,
                    ChamCongId = machine.Id + "." + time.ToString("yyyyMMddHHmmss"), NgayGio = time.ToString("yyyyMMddHHmmss"), TrangThai = direction });
                return JObject.Parse(await router.Route(client, "CHAMCONG", json.Length, json));
            }
            static bool Ok(JObject response) => response.Value<bool?>("IsError") == false;
            int ProductionRows() => schema.Set<HQ_PhieuCan>().Count(x => x.TheIdNhanVien == card.MaTheTu);
            int Roots() => schema.Set<CheckInOut>().Count(x => x.MaChamCong == card.MaTheTu);

            var first = await Weigh(0);
            Check(Ok(first) && ProductionRows() == 1 && Roots() == 0,
                "OFF accepts positive production without any check-in");
            Check(!spy.Commands.Any(sql => sql.Contains("CheckInOut") || sql.Contains("ScaleAttendanceScan")),
                "OFF writer AND totals never query attendance tables");
            var totals = JObject.Parse(first.Value<string>("Data")!);
            Check(totals.Value<int>("TongSoRo") == 1 && totals.Value<decimal>("TongTrongLuong") == 1.4m,
                "OFF still responds with correct complete production totals");
            Check(Ok(await Weigh(0)) && ProductionRows() == 1, "OFF same-Id replay never duplicates SQL");
            Check(!Ok(await Weigh(1, "US")) && ProductionRows() == 1, "OFF still rejects unstable US");
            Check(!Ok(await Weigh(2, kg: 0m)) && !Ok(await Weigh(3, kg: -0.8m)) && ProductionRows() == 1,
                "OFF zero/negative weight stays identity-only");
            Check(!Ok(await Weigh(0, kg: 1.5m)) && ProductionRows() == 1,
                "OFF conflicting same Id still rejects without overwriting");
            foreach (var direction in new[] { "Vào", "Ra" })
            {
                var response = await Scan(4, direction);
                Check(!Ok(response) && JObject.Parse(response.Value<string>("DataError")!).Value<string>("Code") == "ATTENDANCE_DISABLED" && Roots() == 0,
                    "OFF scale " + direction + " replies disabled without changing attendance");
            }
            var workshop = new ChamCongTaiXuongController(store);
            var beforeWorkshop = schema.Set<WorkshopAttendance>().Count();
            var responseHttp = (OkObjectResult)workshop.Post(new() { Device = 986001, MaTheTu = card.MaTheTu,
                Time = start.ToString("yyyyMMddHHmmss"), Xuong = "1" });
            Check(JObject.FromObject(responseHttp.Value!).Value<string>("message") == "Chấm công đang tắt" &&
                schema.Set<WorkshopAttendance>().Count() == beforeWorkshop,
                "OFF workshop HTTP acknowledges disabled without a Vao/Ra insert");
            JObject? workshopFrame = null;
            var workshopCaller = Proxy<ISingleClientProxy>((_, values) =>
            { workshopFrame = JObject.FromObject(((object?[])values![1]!)[0]!); return Task.CompletedTask; });
            using (var workshopHub = new WorkshopHub(store)
                { Clients = Proxy<IHubCallerClients>((_, _) => workshopCaller) })
                await workshopHub.CheckIn("MODE_WORKSHOP", card.MaTheTu, "Vào", start.ToString("yyyyMMddHHmmss"), "1");
            Check(workshopFrame?.Value<string>("message") == "Chấm công đang tắt" &&
                schema.Set<WorkshopAttendance>().Count() == beforeWorkshop,
                "OFF workshop SignalR also replies without writing attendance");
            Check(!ScaleAttendanceProcessor.Process(client, "{}", attendanceEnabled: false).Success,
                "direct scale processor also has a no-SQL disabled guard");
            var scaleHttp = new ChamCongController(machines, NullLogger<ChamCongController>.Instance,
                Options.Create(new ScaleAttendanceOptions()), store);
            var scaleHttpResult = scaleHttp.Post(new());
            Check(scaleHttpResult is ConflictObjectResult conflict &&
                JObject.FromObject(conflict.Value!).Value<string>("code") == "ATTENDANCE_DISABLED" && Roots() == 0,
                "OFF scale HTTP sync does not insert or falsely acknowledge a saved attendance row");
            var transferBlocked = false;
            try { ScaleAttendanceTransferService.Transfer(0, "TEST", 1, "TEST", 16, attendanceEnabled: false); }
            catch (InvalidOperationException) { transferBlocked = true; }
            Check(transferBlocked, "OFF cannot transfer shifts even before opening SQL");

            var backupCard = new TheTu { MaTheTu = "0009160002", MaNhanVien = "MODE_BACKUP" };
            schema.AddRange(backupCard, new NhanVienDaiThanh { MaNhanVien = backupCard.MaNhanVien,
                MaHoSo = backupCard.MaNhanVien, Name = "Backup without Vao", IsHuman = true });
            foreach (var entry in schema.ChangeTracker.Entries().Where(x => x.State == EntityState.Added))
                foreach (var property in entry.Properties.Where(x => x.Metadata.ClrType == typeof(string) &&
                    !x.Metadata.IsNullable && x.CurrentValue == null)) property.CurrentValue = "";
            await schema.SaveChangesAsync();
            async Task<JObject> Backup(int seconds, decimal kg = 9.9m)
            {
                var time = start.AddSeconds(seconds);
                var json = JsonConvert.SerializeObject(new DataCoummunication { Id = machine.Id + "." + time.ToString("yyyyMMddHHmmss"),
                    NgayGio = time.ToString("yyyyMMddHHmmss"), MayCan = machine.Id, TheId = backupCard.MaTheTu,
                    MaNhanVien = backupCard.MaNhanVien, MaLo = machine.MaLo, MaSize = machine.MaSize,
                    MaThanhPham = machine.MaThanhPham, MaLoaiNguyenLieu = machine.MaLoaiNguyenLieu,
                    TrongLuong = kg, TrongLuongTare = 0m, MaXuong = "1" });
                return JObject.Parse(await router.Route(client, "PHIEUCANADD", json.Length, json));
            }
            Check(Ok(await Backup(5)) && schema.Set<HQ_PhieuCan>().Count(x => x.TheIdNhanVien == backupCard.MaTheTu) == 1,
                "OFF WSS production backup also accepts a registered employee without historical Vao");

            Check((await manager.ChangeAsync(true, store.Current.Revision, admin.UserName, password)).Success,
                "admin re-enables attendance after OFF production");
            Check(!Ok(await Backup(6, 8.8m)) && schema.Set<HQ_PhieuCan>().Count(x => x.TheIdNhanVien == backupCard.MaTheTu) == 1,
                "ON WSS backup restores historical attendance checks for normal employees");
            Check(!Ok(await Weigh(10)) && ProductionRows() == 1 && Roots() == 0,
                "ON immediately requires a valid shift for new tickets again");
            Check(Ok(await Scan(11, "Vào")) && Ok(await Weigh(12)) && Roots() == 1 && ProductionRows() == 2,
                "ON check-in then production works normally");
            var workshopEnteredAt = start.AddMinutes(-20);
            JObject WorkshopPost(DateTime time, string direction) => JObject.FromObject(((OkObjectResult)workshop.Post(new()
            {
                Device = 986001, MaTheTu = card.MaTheTu, Xuong = "1", Time = time.ToString("yyyyMMddHHmmss"), TrangThai = direction
            })).Value!);
            Check(WorkshopPost(workshopEnteredAt, "Vào").Value<bool?>("success") == true &&
                schema.Set<WorkshopAttendance>().Count(x => x.MaTheTu == card.MaTheTu) == 1,
                "ON workshop check-in writes its original Vao normally");
            var rootsBeforeSwitch = Roots();
            Check((await manager.ChangeAsync(false, store.Current.Revision, admin.UserName, password)).Success &&
                Ok(await Weigh(13)) && Roots() == rootsBeforeSwitch,
                "disabling while a shift exists preserves that exact shift and production continues");
            Check(WorkshopPost(workshopEnteredAt.AddMinutes(15), "Ra").Value<string>("message") == "Chấm công đang tắt" &&
                schema.Set<WorkshopAttendance>().Single(x => x.MaTheTu == card.MaTheTu).TrangThai == "Vào",
                "OFF preserves an existing workshop Vao and never inserts a synthetic Ra");
            Check((await manager.ChangeAsync(true, store.Current.Revision, admin.UserName, password)).Success &&
                Ok(await Weigh(14)) && Roots() == rootsBeforeSwitch && ProductionRows() == 4,
                "re-enabling reuses the old still-valid shift, no synthetic Vao/Ra");
            Check(WorkshopPost(workshopEnteredAt.AddMinutes(15), "Ra").Value<bool?>("success") == true &&
                schema.Set<WorkshopAttendance>().Count(x => x.MaTheTu == card.MaTheTu) == 2,
                "re-enabled workshop Ra uses the original Vao and its original 15m boundary");
        }
        finally
        {
            // Only known files in the GUID test directory; never recursively remove a broader path.
            foreach (var file in new[] { path, corruptPath, blockedPath, racingPath }) File.Delete(file);
            Directory.Delete(folder);
        }
        return checks;
    }

    private static void Set(object target, string name, object? value)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var property = target.GetType().GetProperty(name, flags);
        if (property != null) property.SetValue(target, value);
        else target.GetType().GetField(name, flags)!.SetValue(target, value);
    }
    private static T Get<T>(object target, string name) =>
        (T)target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(target)!;
    private static T Proxy<T>(Func<MethodInfo, object?[]?, object?> handler) where T : class
    {
        var proxy = DispatchProxy.Create<T, ServiceProxy>();
        ((ServiceProxy)(object)proxy).Handler = handler;
        return proxy;
    }

    private sealed class ModeCommit(IMayCansService machines, IMainService main, DbContext schema,
        IAttendanceMode mode, AttendanceSqlSpy spy) : CommitService(machines, null!, main,
            Options.Create(new ScaleAttendanceOptions()), mode)
    {
        protected override dbPMScontext CreateHqOnlineDb() => new ModeDb(schema, spy);
    }

    private sealed class ModeDb(DbContext schema, AttendanceSqlSpy spy) : dbPMScontext(
        new DbContextOptionsBuilder<dbPMScontext>().UseSqlServer(schema.Database.GetConnectionString())
            .UseModel(schema.Model).AddInterceptors(spy).Options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder builder) { }
    }

    private sealed class AttendanceSqlSpy : DbCommandInterceptor
    {
        public ConcurrentBag<string> Commands { get; } = new();
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
            CommandEventData data, InterceptionResult<DbDataReader> result, CancellationToken cancellation = default)
        { Commands.Add(command.CommandText); return ValueTask.FromResult(result); }
        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command,
            CommandEventData data, InterceptionResult<int> result, CancellationToken cancellation = default)
        { Commands.Add(command.CommandText); return ValueTask.FromResult(result); }
    }
}
