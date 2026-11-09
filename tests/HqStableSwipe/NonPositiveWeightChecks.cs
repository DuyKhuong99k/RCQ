using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Models.Repos.Models;
using MvvmHelpers;
using Newtonsoft.Json.Linq;
using Services;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;

internal static class NonPositiveWeightChecks
{
    public static async Task Run(Action<bool, string> check)
    {
        const string sample = "{\"Id\":\"7779520.20261026091610\",\"NgayGio\":\"20261026091610\",\"TrongLuong\":\"-0.80\",\"TrongLuongTare\":\"0.80\",\"TheId\":\"0005610611\",\"Stated\":\"ST\"}";
        // Fixture identities only; never read or guess the real person's name for this card.
        var cards = Cache("VmThe", new ObservableRangeCollection<TheTu>
            { new() { MaTheTu = "0005610611", MaNhanVien = "TEST_NV" } });
        var people = Cache("VmNhanVien", new ObservableRangeCollection<NhanVienDaiThanh>
            { new() { MaNhanVien = "TEST_NV", MaHoSo = "TEST_HS", Name = "Nhân viên thử nghiệm", IsHuman = true } });
        var main = Mock<IMainService>((method, _) => method.Name switch
        { "get_VmThe" => cards, "get_VmNhanVien" => people, _ => throw new Exception("Unexpected SQL/totals/cache access: " + method.Name) });

        foreach (var mode in new[] { "NEGATIVE_ST", "ZERO_ST", "NEGATIVE_US", "UNKNOWN_CARD", "NEGATIVE_TARE" })
        {
            var payload = JObject.Parse(sample);
            if (mode == "ZERO_ST") payload["TrongLuong"] = "0.00";
            if (mode == "NEGATIVE_US") payload["Stated"] = "US";
            if (mode == "UNKNOWN_CARD") payload["TheId"] = "UNKNOWN";
            if (mode == "NEGATIVE_TARE") payload["TrongLuongTare"] = "-0.80";
            var machine = new MayCan { Id = "7779520", WKv = AppKV.Hq, MType = "BOARD", ConnectionId = "TEST_CONNECTION",
                TongSoRo = 42, TongTrongLuong = 123m };
            var frames = new List<(string Target, object?[] Arguments)>();
            var caller = Mock<ISingleClientProxy>((_, args) =>
            {
                frames.Add(((string)args![0]!, (object?[])args[1]!));
                return Task.CompletedTask;
            });
            async Task<bool> SendIdentity(object?[] args)
            {
                await caller.SendAsync("SETNHANVIEN", (string)args[1]!, (string)args[2]!, (string)args[3]!);
                return true;
            }
            var machines = Mock<IMayCansService>((method, args) => method.Name switch
            {
                "Find" => machine, "NotifyItemChanged" => null,
                "CommandSetNhanVien" => SendIdentity(args!),
                _ => throw new Exception("No AddItem or other device command allowed: " + method.Name)
            });
            var sqlOpens = 0;
            var commit = new GuardedCommitService(machines, main, () =>
            { sqlOpens++; throw new Exception("Identification must never open SQL"); });
            var identity = new ClientInfo { Id = machine.Id, WKv = AppKV.Hq, IsMayCan = true, IsActive = true };
            var users = Mock<IUserService>((_, _) => identity);
            var router = new CommunicationService(users, commit, Options.Create(new ScaleAttendanceOptions()), machines);
            using var hub = new ChatHub(users, router, commit, machines, null!, NullLogger<ChatHub>.Instance)
            { Context = new TestContext(), Clients = Mock<IHubCallerClients>((_, _) => caller) };
            var json = payload.ToString(Newtonsoft.Json.Formatting.None);
            await hub.SendData("LITE", json.Length, json);
            await HqOnlineUi.WhenIdleAsync(machine).WaitAsync(TimeSpan.FromSeconds(3));

            var identified = mode is "NEGATIVE_ST" or "ZERO_ST";
            var expectedMessage = mode switch
            {
                "NEGATIVE_ST" => ScaleOnlineMessages.NegativeWeight,
                "ZERO_ST" => ScaleOnlineMessages.ZeroWeight,
                "NEGATIVE_US" => ScaleOnlineMessages.Unstable,
                "UNKNOWN_CARD" => ScaleOnlineMessages.UnknownEmployee,
                _ => ScaleOnlineMessages.InvalidTare
            };
            check(sqlOpens == 0 && machine.Items.Count == 0 && machine.TongSoRo == 42 && machine.TongTrongLuong == 123m,
                "Public WSS identifies only: no SQL/INSERT/totals/change shift: " + mode);
            check(frames.Count == (identified ? 2 : 1) && frames[^1].Target == "ReceiveData",
                "Exactly one final response, after optional identity frame: " + mode);
            if (identified)
                check(frames[0].Target == "SETNHANVIEN" && frames[0].Arguments.SequenceEqual(
                    new object?[] { "TEST_NV", "TEST_HS", "Nhân viên thử nghiệm" }),
                    "ST/non-positive returns the card's employee/name before ReceiveData: " + mode);
            var envelope = JObject.Parse((string)frames[^1].Arguments[1]!);
            var detail = JObject.Parse((string)envelope["DataError"]!);
            check((bool?)envelope["IsError"] == true && envelope["Data"]!.Type == JTokenType.Null &&
                detail.Value<string>("ErrorString") == expectedMessage && detail.Value<string>("SaveStatus") == "NOT_SAVED" &&
                detail.Value<string>("Id") == (string?)payload["Id"] && detail.Value<string>("TheId") == (string?)payload["TheId"],
                "Response has exact reason and original Id/card/NOT_SAVED, not a successful weight ACK: " + mode);
        }
    }

    private static object Cache(string property, object items)
    {
        var type = typeof(IMainService).GetProperty(property)!.PropertyType;
        var cache = RuntimeHelpers.GetUninitializedObject(type);
        type.GetField("items", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(cache, items);
        return cache;
    }

    private static T Mock<T>(Func<MethodInfo, object?[]?, object?> handler) where T : class
    { var result = DispatchProxy.Create<T, ServiceProxy>(); ((ServiceProxy)(object)result).Handler = handler; return result; }

    private sealed class TestContext : HubCallerContext
    {
        public override string ConnectionId => "TEST_CONNECTION";
        public override string? UserIdentifier => null;
        public override ClaimsPrincipal? User => null;
        public override IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();
        public override IFeatureCollection Features { get; } = new FeatureCollection();
        public override CancellationToken ConnectionAborted => CancellationToken.None;
        public override void Abort() { }
    }
}
