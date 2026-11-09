using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using MvvmHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services;
using Vars;
using Vars.Hubs;

internal static class GroupAttendanceChecks
{
    // Exercise the public command router, real SQL writer and attendance processor together.
    // The caller owns a GUID LocalDB fixture; this never reads factory configuration.
    public static async Task<int> RunAsync(DbContext schema, DateTime at, CommunicationService router,
        MayCan first, MayCan second, ObservableRangeCollection<TheTu> cards,
        ObservableRangeCollection<NhanVienDaiThanh> employees)
    {
        var checks = 0;
        void Check(bool value, string label)
        {
            if (!value) throw new Exception("FAIL: " + label);
            checks++;
            Console.WriteLine("PASS: " + label);
        }
        var firstProduct = first.MaThanhPham;
        var firstMaterial = first.MaLoaiNguyenLieu;
        var secondProduct = second.MaThanhPham;
        var secondMaterial = second.MaLoaiNguyenLieu;
        ClientInfo Connection(MayCan scale) => new()
            { Id = scale.Id, WKv = AppKV.Hq, IsMayCan = true, IsActive = true };
        void Select(MayCan scale, string product, string material)
        {
            HqScaleSelection.ApplyFirmware(scale, scale.ConnectionId, new[]
            {
                new DataCMD { PropetyName = nameof(scale.MaThanhPham), PropetyValue = product },
                new DataCMD { PropetyName = nameof(scale.MaLoaiNguyenLieu), PropetyValue = material }
            });
        }
        async Task<JObject> Scan(MayCan scale, string card, DateTime time, string direction)
        {
            var json = JsonConvert.SerializeObject(new ScaleAttendanceProcessor.Request
            {
                TheId = card, ChamCongId = scale.Id + "." + time.ToString("yyyyMMddHHmmss"),
                NgayGio = time.ToString("yyyyMMddHHmmss"), TrangThai = direction
            });
            return JObject.Parse(await router.Route(Connection(scale), "CHAMCONG", json.Length, json));
        }
        async Task<JObject> Weigh(MayCan scale, string card, DateTime time, decimal kg = 1.4m)
        {
            var json = JsonConvert.SerializeObject(new DataLite
            {
                Id = scale.Id + "." + time.ToString("yyyyMMddHHmmss"),
                NgayGio = time.ToString("yyyyMMddHHmmss"), TheId = card, Stated = "ST", TrongLuong = kg
            });
            return JObject.Parse(await router.Route(Connection(scale), "LITE", json.Length, json));
        }
        static bool Ok(JObject reply) => (bool?)reply["IsError"] == false;
        static JObject Error(JObject reply) => JObject.Parse((string)reply["DataError"]!);

        try
        {
            for (var variant = 0; variant < 2; variant++)
            {
                var card = new TheTu { MaTheTu = "000900000" + variant, MaNhanVien = "GROUP_" + variant };
                var employee = new NhanVienDaiThanh
                {
                    MaNhanVien = card.MaNhanVien, MaHoSo = card.MaNhanVien, Name = "TEST GROUP",
                    IsNhom = true, IsHuman = variant == 1
                };
                schema.AddRange(card, employee);
                foreach (var entry in schema.ChangeTracker.Entries().Where(x => x.State == EntityState.Added))
                    foreach (var property in entry.Properties.Where(x => x.Metadata.ClrType == typeof(string) &&
                        !x.Metadata.IsNullable && x.CurrentValue == null)) property.CurrentValue = "";
                await schema.SaveChangesAsync();
                cards.Add(card);
                employees.Add(employee);
                var label = variant == 0 ? "Group IsHuman=0" : "Group IsHuman=1";
                var start = at.AddSeconds(variant * 80);
                DateTime Time(int seconds) => start.AddSeconds(seconds);
                async Task CheckPolicyParity(MayCan scale, int seconds, string? expected, string reason)
                {
                    using var db = new TestDbForGroups(schema);
                    var time = Time(seconds);
                    var workshop = ScaleWorkshopHistory.Resolve(db, scale.Id, time);
                    var context = ScaleWorkContext.Create(scale.MaThanhPham, scale.MaLoaiNguyenLieu, workshop);
                    var synchronous = ScaleProductionAttendancePolicy.GetError(db, card.MaTheTu,
                        int.Parse(scale.Id), time, 16, employee, context.Product, context.Material, context.Workshop);
                    var asynchronous = await ScaleProductionAttendancePolicy.GetOnlineErrorAsync(db,
                        card.MaTheTu, time, 16, employee, context, CancellationToken.None);
                    Check(synchronous == expected && asynchronous == expected, label + ": sync/async agree " + reason);
                }
                Select(first, firstProduct, firstMaterial);
                Select(second, firstProduct, firstMaterial);

                await CheckPolicyParity(first, 1, ScaleOnlineMessages.MissingCheckIn, "before Vao");
                JObject missing;
                var output = Console.Out;
                try
                {
                    Console.SetOut(new UnavailableConsole());
                    missing = await Weigh(first, card.MaTheTu, Time(1));
                }
                finally { Console.SetOut(output); }
                Check(!Ok(missing) && Error(missing).Value<string>("ErrorString") == ScaleOnlineMessages.MissingCheckIn &&
                    Error(missing).Value<string>("SaveStatus") == HqOnlineResponse.NotSaved,
                    label + ": no Vao stays NOT_SAVED even when diagnostic output fails");
                Check(!schema.Set<HQ_PhieuCan>().Any(x => x.TheIdNhanVien == card.MaTheTu),
                    label + ": no SQL ticket was created without a shift");
                Check(Ok(await Scan(first, card.MaTheTu, Time(4), "Vào")) && Ok(await Weigh(first, card.MaTheTu, Time(5))),
                    label + ": Vao then first positive weight succeeds immediately");
                await CheckPolicyParity(second, 6, null, "across scales in the same shift");
                Check(Ok(await Weigh(second, card.MaTheTu, Time(6))),
                    label + ": same product/material/workshop works across scales");
                var repeated = await Scan(second, card.MaTheTu, Time(7), "Vào");
                Check(!Ok(repeated) && Error(repeated).Value<string>("Code") == "ALREADY_CHECKED_IN" &&
                    schema.Set<CheckInOut>().Count(x => x.MaChamCong == card.MaTheTu && x.TrangThai == "Vào") == 1,
                    label + ": repeated Vao keeps one root, including at another scale");

                var changedProduct = firstProduct == "BV" ? "BVT1" : "BV";
                Select(second, changedProduct, firstMaterial);
                await CheckPolicyParity(second, 8, ScaleOnlineMessages.DifferentProduct, "on a product mismatch");
                var productMismatch = await Weigh(second, card.MaTheTu, Time(8));
                Check(!Ok(productMismatch) && Error(productMismatch).Value<string>("ErrorString") == ScaleOnlineMessages.DifferentProduct,
                    label + ": different product requires new Vao");
                Check(Ok(await Scan(second, card.MaTheTu, Time(9), "Vào")) && Ok(await Weigh(second, card.MaTheTu, Time(10))),
                    label + ": product switch closes old root and permits next weight");

                var changedMaterial = firstMaterial == "XOAIDONG" ? "XOAITUOI" : "XOAIDONG";
                Select(second, changedProduct, changedMaterial);
                await CheckPolicyParity(second, 11, ScaleOnlineMessages.DifferentMaterial, "on a material mismatch");
                var materialMismatch = await Weigh(second, card.MaTheTu, Time(11));
                Check(!Ok(materialMismatch) && Error(materialMismatch).Value<string>("ErrorString") == ScaleOnlineMessages.DifferentMaterial,
                    label + ": different material requires new Vao");
                Check(Ok(await Scan(second, card.MaTheTu, Time(12), "Vào")) && Ok(await Weigh(second, card.MaTheTu, Time(13))),
                    label + ": material switch permits the next weight");

                // At the test scale's recorded move to another workshop, same codes are not the same shift.
                var move = new LichSuXuongMayCan { MayCan = second.Id, MaXuong = "2", TuThoiGian = Time(14) };
                schema.Add(move);
                await schema.SaveChangesAsync();
                await CheckPolicyParity(second, 14, ScaleOnlineMessages.DifferentWorkshop, "on a workshop mismatch");
                var workshopMismatch = await Weigh(second, card.MaTheTu, Time(14));
                Check(!Ok(workshopMismatch) && Error(workshopMismatch).Value<string>("ErrorString") == ScaleOnlineMessages.DifferentWorkshop,
                    label + ": different workshop requires new Vao even with matching codes");
                Check(Ok(await Scan(second, card.MaTheTu, Time(15), "Vào")) && Ok(await Weigh(second, card.MaTheTu, Time(16))),
                    label + ": workshop switch opens the proper new root");
                using (var db = new TestDbForGroups(schema))
                    Check(ScaleOnlineAttendanceService.OpenShifts(db, card.MaTheTu, Time(16), 16).Count == 1 &&
                        schema.Set<CheckInOut>().Count(x => x.MaChamCong == card.MaTheTu && x.TrangThai == "Ra") == 3,
                        label + ": each switch atomically closes the previous shift, only one remains open");

                Check(Ok(await Scan(first, card.MaTheTu, Time(17), "Ra")),
                    label + ": Ra succeeds at another scale despite different current codes/workshop");
                await CheckPolicyParity(second, 18, ScaleOnlineMessages.MissingCheckIn, "after Ra");
                var afterRa = await Weigh(second, card.MaTheTu, Time(18));
                Check(!Ok(afterRa) && Error(afterRa).Value<string>("ErrorString") == ScaleOnlineMessages.MissingCheckIn &&
                    !Ok(await Weigh(first, card.MaTheTu, Time(19))),
                    label + ": after Ra both scales reject NEW tickets (BAOMAY regression)");
                var savedReplay = await Weigh(second, card.MaTheTu, Time(16));
                Check(Ok(savedReplay) && JObject.Parse((string)savedReplay["Data"]!).Value<string>("SaveStatus") == HqOnlineResponse.Saved &&
                    schema.Set<HQ_PhieuCan>().Count(x => x.TheIdNhanVien == card.MaTheTu) == 5,
                    label + ": same saved Id after Ra confirms original save, never inserts again");

                var zero = await Weigh(first, card.MaTheTu, Time(20), 0m);
                var negative = await Weigh(first, card.MaTheTu, Time(21), -0.8m);
                Check(!Ok(zero) && Error(zero).Value<string>("ErrorString") == ScaleOnlineMessages.ZeroWeight &&
                    !Ok(negative) && Error(negative).Value<string>("ErrorString") == ScaleOnlineMessages.NegativeWeight,
                    label + ": ST zero/negative remain identity-only after Ra");
                Check(!Ok(await Weigh(first, card.MaTheTu, Time(22))) &&
                    schema.Set<HQ_PhieuCan>().Count(x => x.TheIdNhanVien == card.MaTheTu) == 5 &&
                    schema.Set<CheckInOut>().Count(x => x.MaChamCong == card.MaTheTu) == 8,
                    label + ": identity-only swipes neither reopen shift nor insert tickets");
                Check(Ok(await Scan(first, card.MaTheTu, Time(23), "Vào")) && Ok(await Weigh(first, card.MaTheTu, Time(24))),
                    label + ": new Vao after Ra permits production again");
                Check(schema.Set<HQ_PhieuCan>().Count(x => x.TheIdNhanVien == card.MaTheTu) == 6,
                    label + ": exactly six genuine tickets, none from any rejected swipe");

                schema.Remove(move);
                await schema.SaveChangesAsync();
            }
        }
        finally
        {
            Select(first, firstProduct, firstMaterial);
            Select(second, secondProduct, secondMaterial);
        }
        return checks;
    }

    private sealed class UnavailableConsole : StringWriter
    {
        public override void WriteLine(string? value) => throw new IOException("Test-only console failure");
    }

    private sealed class TestDbForGroups(DbContext schema) : dbPMScontext(
        new DbContextOptionsBuilder<dbPMScontext>().UseSqlServer(schema.Database.GetConnectionString())
            .UseModel(schema.Model).Options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder builder) { }
    }
}
