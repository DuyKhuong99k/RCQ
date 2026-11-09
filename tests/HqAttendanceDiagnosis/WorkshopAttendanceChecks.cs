using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Repos.Models;
using Newtonsoft.Json.Linq;
using PMSHub.Controllers;
using Services;

internal static class WorkshopAttendanceChecks
{
    // Runs only inside the caller's GUID LocalDB fixture. Both public transports use Save.
    public static async Task<int> RunAsync(DbContext schema)
    {
        var checks = 0;
        void Check(bool value, string label)
        {
            if (!value) throw new Exception("FAIL: " + label);
            checks++;
            Console.WriteLine("PASS: Workshop 15m: " + label);
        }
        var card = new TheTu { MaTheTu = "0009150001", MaNhanVien = "WORKSHOP_15M" };
        schema.AddRange(card, new NhanVienDaiThanh
            { MaNhanVien = card.MaNhanVien, MaHoSo = card.MaNhanVien, Name = "Nhân viên thử nghiệm", IsHuman = true });
        foreach (var entry in schema.ChangeTracker.Entries().Where(x => x.State == EntityState.Added))
            foreach (var property in entry.Properties.Where(x => x.Metadata.ClrType == typeof(string) &&
                !x.Metadata.IsNullable && x.CurrentValue == null)) property.CurrentValue = "";
        await schema.SaveChangesAsync();

        var start = ScaleWorkshopHistory.BusinessNow().Date.AddHours(7);
        JObject Save(string device, DateTime time, string? status = null, string? scannedCard = null) =>
            JObject.FromObject(AttendanceProcessor.Save(device, scannedCard ?? card.MaTheTu, status,
                time.ToString("yyyyMMddHHmmss"), "1"));
        static bool Ok(JObject result) => result.Value<bool?>("success") == true;
        bool IsRepeatedEntry(JObject result) => result.Value<bool?>("success") == true &&
            result.Value<bool?>("duplicated") == true &&
            result.Value<string>("message") == "đã quẹt vào" && result.Value<string>("trangThai") == "đã quẹt vào" &&
            result.Value<string>("maTheTu") == card.MaTheTu && result.Value<string>("maNhanVien") == card.MaNhanVien &&
            result.Value<string>("tenNhanVien") == "Nhân viên thử nghiệm";
        int Rows() => schema.Set<WorkshopAttendance>().Count(x => x.MaTheTu == card.MaTheTu);

        Check(!Ok(Save("WS_A", start.AddSeconds(-1), "Ra")) && Rows() == 0,
            "Ra without Vao remains rejected");
        var entered = Save("WS_A", start);
        Check(Ok(entered) && entered.Value<string>("trangThai") == "Vào" && Rows() == 1,
            "first auto swipe writes one Vao");
        Check(IsRepeatedEntry(Save("WS_A", start.AddSeconds(1))) && Rows() == 1,
            "auto swipe one second after Vao acknowledges identity/message without INSERT");
        Check(IsRepeatedEntry(Save("WS_B", start.AddMinutes(14).AddSeconds(59), "Ra")) && Rows() == 1,
            "explicit Ra at 14m59s on another workshop device is rejected");
        Check(IsRepeatedEntry(Save("WS_B", start.AddMinutes(14).AddSeconds(59), "Vào")) && Rows() == 1,
            "explicit Vao inside the window reports already checked in without another root");
        var replay = Save("WS_A", start);
        Check(Ok(replay) && replay.Value<bool?>("duplicated") == true &&
            replay.Value<string>("trangThai") == "Vào" && Rows() == 1,
            "same-Id retry still confirms original Vao without toggling");
        var repeatedAfterWindow = Save("WS_REPEAT", start.AddMinutes(15), "Vào");
        Check(IsRepeatedEntry(repeatedAfterWindow) && Rows() == 1,
            "explicit repeated Vao after 15m uses the same identity acknowledgment without another row");
        var leave = Save("WS_B", start.AddMinutes(15));
        Check(Ok(leave) && leave.Value<string>("trangThai") == "Ra" && Rows() == 2,
            "auto swipe at exactly 15m writes Ra; rejected swipes never reset the timer");
        Check(Ok(Save("WS_A", start.AddMinutes(15).AddSeconds(1), "Vào")) && Rows() == 3,
            "next Vao starts a new interval");
        Check(IsRepeatedEntry(Save("WS_B", start.AddMinutes(30), "Ra")) && Rows() == 3,
            "new Vao has its own 15m interval");
        Check(Ok(Save("WS_B", start.AddMinutes(30).AddSeconds(1), "Ra")) && Rows() == 4,
            "explicit Ra at exactly 15m succeeds");
        Check(!Ok(Save("WS_A", start.AddSeconds(-2))) && Rows() == 4,
            "backdated event is still rejected instead of becoming a fresh Vao");
        Check(!Ok(Save("WS_A", start.AddMinutes(31), "BAD")) && Rows() == 4,
            "invalid explicit status still rejects without a row");
        var unknown = Save("WS_A", start.AddMinutes(31), scannedCard: "UNREGISTERED_15M");
        Check(unknown.Value<string>("message") == "Thẻ chưa đăng ký" && Rows() == 4,
            "unregistered card remains rejected");

        // Public HTTP controller: repeat acknowledgments carry identity, success=true and duplicated=true.
        var controller = new ChamCongTaiXuongController();
        var httpStart = start.AddHours(1);
        JObject Post(DateTime time, string? status = null)
        {
            var response = controller.Post(new ChamCongTaiXuongController.Request
            {
                Device = 985001, MaTheTu = card.MaTheTu, Time = time.ToString("yyyy-MM-dd HH:mm:ss"),
                Xuong = "1", TrangThai = status
            });
            if (response is not OkObjectResult ok) throw new Exception("Unexpected HTTP envelope");
            return JObject.FromObject(ok.Value!);
        }
        Check(Ok(Post(httpStart)) && Rows() == 5, "HTTP starts a Vao");
        Check(IsRepeatedEntry(Post(httpStart.AddMinutes(5))) && Rows() == 5,
            "HTTP auto swipe inside 15m returns the message without SQL write");
        Check(IsRepeatedEntry(Post(httpStart.AddMinutes(14).AddSeconds(59), "Ra")) && Rows() == 5,
            "HTTP explicit Ra also obeys the same guard");

        var messages = new List<(string Target, JObject Body)>();
        var caller = Mock<ISingleClientProxy>((method, values) =>
        {
            if (method.Name != "SendCoreAsync") throw new Exception(method.Name);
            messages.Add(((string)values![0]!, JObject.FromObject(((object?[])values[1]!)[0]!)));
            return Task.CompletedTask;
        });
        using var hub = new WorkshopHub
            { Clients = Mock<IHubCallerClients>((method, _) => method.Name == "get_Caller" ? caller : throw new Exception(method.Name)) };
        await hub.CheckIn("WS_SIGNALR", card.MaTheTu, "Ra", httpStart.AddMinutes(14).ToString("yyyyMMddHHmmss"), "1");
        Check(messages.Count == 1 && messages[0].Target == "CHAMCONG_RESULT" && IsRepeatedEntry(messages[0].Body) && Rows() == 5,
            "SignalR sends CHAMCONG_RESULT already-entered message, even on a different device");
        await hub.CheckIn("WS_SIGNALR", card.MaTheTu, "Ra", httpStart.AddMinutes(15).ToString("yyyyMMddHHmmss"), "1");
        Check(messages.Count == 2 && Ok(messages[1].Body) && messages[1].Body.Value<string>("trangThai") == "Ra" && Rows() == 6,
            "SignalR can check out at exactly 15m after HTTP check-in");

        // Different days keep the existing workshop reset rule (no night-shift linking here).
        var dayEnd = start.Date.AddHours(23).AddMinutes(59);
        Check(Ok(Save("WS_A", dayEnd)), "late-day Vao accepted");
        var nextDay = Save("WS_A", dayEnd.AddMinutes(2));
        Check(Ok(nextDay) && nextDay.Value<string>("trangThai") == "Vào" && Rows() == 8,
            "next day starts Vao, not Ra of yesterday even if less than 15m apart");
        Check(IsRepeatedEntry(Save("WS_B", dayEnd.AddMinutes(3), "Ra")) && Rows() == 8,
            "new day's Vao has its own 15m protection");

        // Concurrent swipes use the same SQL card lock as normal traffic; no process-only timer.
        var concurrentStart = start.Date.AddDays(2).AddHours(7);
        Check(Ok(Save("WS_LOCK", concurrentStart)), "concurrency fixture starts one Vao");
        var attempts = await Task.WhenAll(Enumerable.Range(1, 10).Select(seconds => Task.Run(() =>
            Save("WS_LOCK", concurrentStart.AddSeconds(seconds)))));
        Check(attempts.All(IsRepeatedEntry) && Rows() == 9,
            "ten concurrent auto swipes inside the window produce zero extra rows");
        var exits = await Task.WhenAll(new[] { "WS_LOCK_A", "WS_LOCK_B" }.Select(device => Task.Run(() =>
            Save(device, concurrentStart.AddMinutes(15), "Ra"))));
        Check(exits.Count(Ok) == 1 && Rows() == 10,
            "two concurrent explicit Ra after 15m commit exactly one checkout");
        return checks;
    }

    private static T Mock<T>(Func<MethodInfo, object?[]?, object?> handler) where T : class
    {
        var proxy = DispatchProxy.Create<T, WorkshopProxy>();
        ((WorkshopProxy)(object)proxy).Handler = handler;
        return proxy;
    }
}

public class WorkshopProxy : DispatchProxy
{
    public Func<MethodInfo, object?[]?, object?> Handler = null!;
    protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args);
}
