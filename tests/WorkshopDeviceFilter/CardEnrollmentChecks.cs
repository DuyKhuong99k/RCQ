using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Models.Repos.Models;
using MvvmHelpers;
using PMSHub.Components.Pages;
using Services;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;

internal static class CardEnrollmentChecks
{
    public static async Task Run(Action<bool, string> check)
    {
        var machines = DispatchProxy.Create<IMayCansService, EnrollmentProxy>();
        var runtime = new ConcurrentDictionary<string, MayCan>();
        var sent = new List<string>();
        var notified = 0;
        var sendSucceeds = true;
        ((EnrollmentProxy)(object)machines).Call = (name, args) => name switch
        {
            "get_Items" => runtime,
            "Find" => runtime.GetValueOrDefault((string)args![0]!),
            "NotifyItemChanged" => Notify(),
            "CommandSendMess" => Send(args!),
            _ when name.StartsWith("add_") || name.StartsWith("remove_") => null,
            _ => throw new Exception("Unexpected machine call: " + name)
        };
        object? Notify() { notified++; return null; }
        Task<bool> Send(object?[] args) { sent.Add((string)args[1]!); return Task.FromResult(sendSucceeds); }

        var users = DispatchProxy.Create<IUserService, MemoryProxy>();
        users.UserAreas = new ObservableRangeCollection<UserArea> { new() { UserId = 1, WKv = (int)AppKV.Hq } };
        foreach (var type in Enum.GetValues<AppType>())
        {
            var device = Connected(type.ToString(), type);
            runtime[device.Id] = device;
        }
        var offline = runtime[AppType._type5.ToString()];
        runtime["other-workshop"] = Connected("other-workshop", AppType._type5, workshop: "2");
        runtime["forbidden"] = Connected("forbidden", AppType._type5, area: (AppKV)18);
        runtime["disconnected"] = Connected("disconnected", AppType._type5);
        runtime["disconnected"].IsConnected = false;
        runtime["not-active"] = Connected("not-active", AppType._type5);
        runtime["not-active"].IsActive = false;
        runtime["fake-connected"] = Connected("fake-connected", AppType._type5);
        runtime["fake-connected"].ConnectionId = " ";
        using var view = new DeviceViewService(machines, users) { XiNghiep = new XiNghiep { Ma = "1" } };
        view.Load(1);
        var cards = DispatchProxy.Create<ICardService, MemoryProxy>();
        cards.ItemNhanVien = new TheTu { MaNhanVien = "EMP" };
        var warnings = new List<string>();
        var toast = DispatchProxy.Create<IToastService, EnrollmentProxy>();
        ((EnrollmentProxy)(object)toast).Call = (name, args) =>
        {
            if (name != "ShowToast") throw new Exception(name);
            warnings.Add((string)args![0]!);
            return null;
        };
        using var page = new TheNhanVien();
        Set("DeviceViewService", view);
        Set("MayCansService", machines);
        Set("cardService", cards);
        Set("ToastService", toast);
        foreach (var type in Enum.GetValues<AppType>())
            check(Choices().Any(x => x.AType == type), "Nạp thẻ cho phép loại cân " + type + " khi WSS kết nối thật");
        check(Choices().Count == Enum.GetValues<AppType>().Length, "Ẩn cân mất mạng, chưa active, thiếu ConnectionId, sai xưởng hoặc quyền");

        offline.NapTheId = "STALE";
        await Toggle(offline.Id);
        check(offline.IsNapThe && offline.NapTheId == "" && view.DeviceCard == offline && sent.SequenceEqual(new[] { "Nạp Thẻ" }),
            "Bắt đầu nạp ở cân Offline: xóa UID cũ, chỉ gửi thông báo, không đổi chế độ");
        SetField("IsVisibleAdd", true);
        offline.NapTheId = "0005610611";
        RefreshCard();
        check(GetField("TheId") as string == "0005610611", "UID từ LITE được đưa vào ô Thẻ Id đúng cân đang nạp");
        SetField("TheId", "");
        RefreshCard();
        check(GetField("TheId") as string == "", "Cập nhật cân khác không điền lại UID cũ vừa xử lý");
        cards.ItemNhanVien = new TheTu();
        await Toggle(offline.Id);
        check(!offline.IsNapThe && sent.Last() == "", "Dừng được dù nhân viên đã bị bỏ chọn");
        cards.ItemNhanVien = new TheTu { MaNhanVien = "EMP" };
        foreach (var id in new[] { "other-workshop", "forbidden", "disconnected", "not-active", "fake-connected", "unknown" })
        {
            var count = sent.Count;
            await Toggle(id);
            check(sent.Count == count, "Bắt đầu kiểm tra lại và từ chối cân không hợp lệ: " + id);
        }
        offline.IsConnected = false;
        await Toggle(offline.Id);
        check(!offline.IsNapThe, "Cân vừa mất kết nối sau khi mở danh sách không được bắt đầu");
        offline.IsConnected = true;
        await Toggle(offline.Id);
        offline.IsConnected = false;
        RefreshCard();
        check(!offline.IsNapThe && GetField("TheId") as string == "", "Mất kết nối khi đang nạp phải dừng và xóa mã thẻ cũ");
        offline.IsConnected = true;
        await Toggle(offline.Id);
        offline.MaXuong = "2";
        view.Load(1);
        RefreshCard();
        check(!offline.IsNapThe, "Cân đổi ra xưởng khác khi đang nạp phải được trả về trạng thái thường");
        offline.MaXuong = "1";
        view.Load(1);
        offline.IsNapThe = true;
        var before = sent.Count;
        await Toggle(offline.Id);
        check(offline.IsNapThe && sent.Count == before && warnings.Last() == "Cân đang được dùng nạp thẻ.",
            "Không chiếm hoặc tắt nạp thẻ của phiên khác");
        offline.IsNapThe = false;
        sendSucceeds = false;
        await Toggle(offline.Id);
        check(!offline.IsNapThe, "Gửi lệnh thất bại phải trả cân về trạng thái thường");
        sendSucceeds = true;
        await Toggle(offline.Id);
        page.Dispose();
        check(!offline.IsNapThe && notified > 0, "Rời trang dừng nạp thẻ của chính trang, không giữ cân mắc chế độ nạp");

        // Exercise the actual LITE path: any weighing/business/SQL access fails this test.
        var main = DispatchProxy.Create<IMainService, EnrollmentProxy>();
        ((EnrollmentProxy)(object)main).Call = (name, _) => throw new Exception("Must not access business/SQL: " + name);
        var commit = new CommitService(machines, null!, main, Options.Create(new ScaleAttendanceOptions()));
        offline.IsNapThe = true;
        foreach (var weight in new[] { -0.8m, 0m, 5m })
        {
            var result = await commit.CommitLite(JsonSerializer.Serialize(new DataLite
            {
                Id = offline.Id + ".20261027100000", NgayGio = "20261027100000", TheId = "0005610611",
                TrongLuong = weight, Stated = "ST"
            }), new ClientInfo { Id = offline.Id }, "LITE");
            using var reply = JsonDocument.Parse(result.Item2);
            check(result.Item1 && offline.NapTheId == "0005610611" &&
                reply.RootElement.GetProperty("IsDataAction").GetInt32() == 0,
                $"LITE nạp thẻ cân Offline, {weight} kg: trả UID, không chạm nghiệp vụ/SQL phiếu cân");
        }
        offline.IsNapThe = false;

        void Set(string name, object value) => typeof(TheNhanVien).GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(page, value);
        IReadOnlyList<MayCan> Choices() => (IReadOnlyList<MayCan>)typeof(TheNhanVien)
            .GetProperty("filteredItems", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(page)!;
        Task Toggle(string id) => (Task)typeof(TheNhanVien)
            .GetMethod("MayCanStartStop", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(page, new object[] { id })!;
        void RefreshCard() => typeof(TheNhanVien).GetMethod("RefreshEnrollmentCard", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(page, null);
        void SetField(string name, object value) => typeof(TheNhanVien).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(page, value);
        object? GetField(string name) => typeof(TheNhanVien).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(page);
    }

    private static MayCan Connected(string id, AppType type, string workshop = "1", AppKV area = AppKV.Hq) => new()
    {
        Id = id, AType = type, MaXuong = workshop, WKv = area,
        IsConnected = true, IsActive = true, ConnectionId = "test-connection-" + id
    };
}

public class EnrollmentProxy : DispatchProxy
{
    public Func<string, object?[]?, object?> Call { get; set; } = null!;
    protected override object? Invoke(MethodInfo? method, object?[]? args) => Call(method!.Name, args);
}
