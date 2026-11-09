using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;

internal static class TransportChecks
{
    public static async Task Run(Action<bool, string> check)
    {
        await CheckProtocolCompatibility(check);
        const string payload = "{\"Id\":\"101.20261017120000\",\"TheId\":\"CARD01\",\"NgayGio\":\"20261017120000\"}";
        foreach (var mode in new[] { "NO_CLIENT", "NOT_MACHINE", "ROUTE_ERROR", "OK" })
        {
            var calls = 0;
            var sent = new List<object?[]>();
            var client = new ClientInfo { Id = "101", IsMayCan = mode != "NOT_MACHINE", IsActive = true, WKv = AppKV.Hq };
            var users = Mock<IUserService>((method, _) => method.Name == "GetByConnectedId"
                ? mode == "NO_CLIENT" ? null : client : throw new Exception(method.Name));
            var route = Mock<ICommunicationService>((method, _) =>
            {
                if (method.Name != "Route") throw new Exception(method.Name);
                calls++;
                if (mode == "ROUTE_ERROR") throw new InvalidOperationException("SECRET_TRANSPORT_EXCEPTION");
                return Task.FromResult("ORIGINAL_ENVELOPE");
            });
            var caller = Mock<ISingleClientProxy>((method, args) =>
            {
                if (method.Name != "SendCoreAsync") throw new Exception(method.Name);
                check((string?)args![0] == "ReceiveData", "Transport dùng target ReceiveData");
                sent.Add((object?[])args[1]!);
                return Task.CompletedTask;
            });
            var clients = Mock<IHubCallerClients>((method, _) => method.Name == "get_Caller" ? caller : throw new Exception(method.Name));
            using var hub = new ChatHub(users, route, null!, null!, null!, NullLogger<ChatHub>.Instance)
            { Context = new TestContext(), Clients = clients };
            await hub.SendData("LITE", payload.Length, payload);
            check(sent.Count == 1 && (string?)sent[0][0] == "LITE", "Transport luôn trả đúng một phản hồi cuối: " + mode);
            check(calls == (mode is "NO_CLIENT" or "NOT_MACHINE" ? 0 : 1), "Không xử lý nghiệp vụ kết nối chưa hợp lệ");
            if (mode == "OK")
            {
                check((string?)sent[0][1] == "ORIGINAL_ENVELOPE", "Transport giữ nguyên envelope đã xử lý");
                continue;
            }
            var envelope = JObject.Parse((string)sent[0][1]!);
            check((bool?)envelope["IsError"] == true && envelope["DataError"]?.Type == JTokenType.String,
                "Lỗi transport giữ envelope DataTranfer, không gửi DataError trần");
            var detail = JsonConvert.DeserializeObject<DataError>((string)envelope["DataError"]!)!;
            check(detail.Id == "101.20261017120000" && detail.TheId == "CARD01" && !string.IsNullOrWhiteSpace(detail.ErrorString),
                "Lỗi transport có metadata và thông báo");
            check(detail.SaveStatus == (mode == "ROUTE_ERROR" ? HqOnlineResponse.Unknown : HqOnlineResponse.NotSaved),
                "Transport không đoán phiếu chưa lưu khi router lỗi");
            check(!envelope.ToString().Contains("SECRET_TRANSPORT"), "Không đẩy exception transport xuống HMI");
        }

        var processed = false; var sendAttempts = 0;
        var aborted = new CancellationToken(true);
        var disconnectedUser = Mock<IUserService>((_, _) => new ClientInfo { Id="101",WKv=AppKV.Hq,IsMayCan=true,IsActive=true });
        var completedRoute = Mock<ICommunicationService>((_, _) => { processed=true; return Task.FromResult("COMMITTED_RESPONSE"); });
        var deadCaller = Mock<ISingleClientProxy>((_, args) =>
        {
            sendAttempts++;
            var token=(CancellationToken)args![2]!;
            check(token.IsCancellationRequested, "WSS gửi phản hồi dùng token kết nối đã mất");
            throw new OperationCanceledException(token);
        });
        var deadClients=Mock<IHubCallerClients>((_,_)=>deadCaller);
        using var disconnectedHub=new ChatHub(disconnectedUser,completedRoute,null!,null!,null!,NullLogger<ChatHub>.Instance)
        {Context=new TestContext(aborted),Clients=deadClients};
        await disconnectedHub.SendData("LITE",payload.Length,payload);
        check(processed && sendAttempts==0,"Kết nối đã mất vẫn hoàn tất xử lý SQL, nhưng không bắt đầu gửi vào kết nối chết");
    }

    // Characterization tests: exact wire JSON, including legacy errors without SaveStatus.
    private static async Task CheckProtocolCompatibility(Action<bool, string> check)
    {
        const string payload = "{\"Id\":\"101.20261017120000\",\"ChamCongId\":\"101.20261017120000\",\"TheId\":\"CARD01\",\"NgayGio\":\"20261017120000\"}";
        const string routeError = "CHARACTERIZATION_ROUTE_ERROR";
        foreach (var area in new[] { AppKV.Hq, AppKV.Main })
        foreach (var command in new[] { "LITE", "lite", "CHAMCONG", "chamcong", "OTHER" })
        foreach (var mode in new[] { "NO_CLIENT", "NOT_MACHINE", "ROUTE_ERROR", "OK" })
        {
            var routeCalls = 0;
            var replies = new List<object?[]>();
            var registered = new ClientInfo { Id = "101", WKv = area, IsActive = true, IsMayCan = mode != "NOT_MACHINE" };
            var users = Mock<IUserService>((_, _) => mode == "NO_CLIENT" ? null : registered);
            var router = Mock<ICommunicationService>((_, _) =>
            {
                routeCalls++;
                if (mode == "ROUTE_ERROR") throw new InvalidOperationException(routeError);
                return Task.FromResult("EXACT_ORIGINAL_REPLY");
            });
            var caller = Mock<ISingleClientProxy>((_, args) =>
            {
                check((string?)args![0] == "ReceiveData", "Characterization preserves response target");
                replies.Add((object?[])args[1]!);
                return Task.CompletedTask;
            });
            using var hub = new ChatHub(users, router, null!, null!, null!, NullLogger<ChatHub>.Instance)
            { Context = new TestContext(), Clients = Mock<IHubCallerClients>((_, _) => caller) };
            await hub.SendData(command, payload.Length, payload);

            var isLite = command.Equals("LITE", StringComparison.OrdinalIgnoreCase);
            var isHqAttendance = area == AppKV.Hq && command.Equals("CHAMCONG", StringComparison.OrdinalIgnoreCase);
            var rejected = mode is "NO_CLIENT" or "NOT_MACHINE";
            var message = rejected ? "Kết nối chưa xác định là máy cân." :
                isLite || isHqAttendance ? ScaleOnlineMessages.ProcessingError : "Máy chủ bận hoặc lỗi xử lý: " + routeError;
            var savedStatus = mode == "ROUTE_ERROR" ? HqOnlineResponse.Unknown : HqOnlineResponse.NotSaved;
            var expected = mode == "OK" ? "EXACT_ORIGINAL_REPLY" : isLite
                ? JsonConvert.SerializeObject(new DataTranfer { IsError = true,
                    DataError = JsonConvert.SerializeObject(HqOnlineResponse.Error(command, message,
                        HqOnlineResponse.Correlation(payload), mode == "NO_CLIENT" ? null : "101", savedStatus)) })
                : JsonConvert.SerializeObject(new DataError { Cmd = command, ErrorString = message,
                    SaveStatus = mode == "ROUTE_ERROR" && isHqAttendance ? HqOnlineResponse.Unknown : null });
            check(routeCalls == (rejected ? 0 : 1), "Characterization preserves routing count");
            check(replies.Count == 1 && (string?)replies[0][0] == command && (string?)replies[0][1] == expected,
                $"Exact protocol unchanged: {area}/{command}/{mode}");
        }
    }

    private static T Mock<T>(Func<MethodInfo, object?[]?, object?> handler) where T : class
    {
        var result = DispatchProxy.Create<T, ServiceProxy>();
        ((ServiceProxy)(object)result).Handler = handler;
        return result;
    }

    private sealed class TestContext(CancellationToken aborted = default) : HubCallerContext
    {
        public override string ConnectionId => "TEST_ONLY";
        public override string? UserIdentifier => null;
        public override ClaimsPrincipal? User => null;
        public override IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();
        public override IFeatureCollection Features { get; } = new FeatureCollection();
        public override CancellationToken ConnectionAborted => aborted;
        public override void Abort() { }
    }
}
