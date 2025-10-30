using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;

namespace Services;

public sealed class ChatHub(
    IUserService userService,
    ICommunicationService communicationService,
    ICommitService commitService,
    IMayCansService mayCansService)
    : Hub
{
    public async Task Auth(string id, int wType, string dataJson)
    {
        var o = new JObject();
        var active = communicationService.AuthProc(Context.ConnectionId, id, wType, dataJson);
        o["active"] = active;
        var jsonStr = o.ToString();
        await Clients.Caller.SendAsync("ReceiveMessage", jsonStr);
        if (!active)
        {
            userService.RemoveByConnectedId(Context.ConnectionId);
            mayCansService.Dis(id);
            Context.Abort();
        }
    }

    public async Task GetConnectionId()
    {
        await Clients.Caller.SendAsync("CONNECTIONID", Context.ConnectionId);
    }

    public override async Task<Task> OnConnectedAsync()
    {
        var feature = Context.Features.Get<IHttpConnectionFeature>();
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId}", $"{feature.RemoteIpAddress}",
            $"{feature.RemotePort}", $"{feature.LocalIpAddress}", $"{feature.LocalPort}");
        userService.Clients.Add(new ClientInfo
        {
            ConnectedId = Context.ConnectionId,
            DateTimeConnected = DateTime.Now,
            IPAddr = feature.RemoteIpAddress?.ToString(),
            WKv = AppKV.Main
        });
        return
            base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        userService.RemoveByConnectedId(Context.ConnectionId);
        mayCansService.DisConnectionId(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendData(string command, int len, string dataJson)
    {
        var client = userService.GetByConnectedId(Context.ConnectionId);
        if (client == null || client.IsMayCan == false) return;
        var dataToSend = await communicationService.Route(client, command, len, dataJson);
        //await Task.Delay(300);
        await Clients.Caller.SendAsync("ReceiveData", command, dataToSend);
    }

    /// <summary>
    ///     Gửi dữ liệu khu vực cối xếp khuôn
    /// </summary>
    /// <param name="command"></param>
    /// <param name="len"></param>
    /// <param name="dataJson"></param>
    /// <returns></returns>
    public async Task SendData2(string command, int len, string dataJson)
    {
        var client = userService.GetByConnectedId(Context.ConnectionId);
        //if(client == null || client.IsMayCan == true) return;
        if (client == null || client.WKv != AppKV.XepKhuon) return;
        var dataToSend = await communicationService.Route(client, command, len, dataJson);
        //await Task.Delay(300);
        await Clients.Caller.SendAsync("ReceiveDataCoi", command, dataToSend);
    }

    public async Task SendId(string message)
    {
        await Clients.Caller.SendAsync("ReceiveId", message);
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendSelectedData(string id, AppKV kv, string dataJson)
    {
        try
        {
            var mayCan = mayCansService.Find(id, true);
            var datas = JsonConvert.DeserializeObject<List<DataCMD>>(dataJson);
            if (datas != null)
                if (mayCan != null)
                {
                    foreach (var dataCmd in datas)
                    {
                        var property = mayCan.GetType().GetProperty(dataCmd.PropetyName);
                        if (property != null) property.SetValue(mayCan, dataCmd.PropetyValue);
                    }

                    mayCansService.NotifyItemChanged();
                }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
    }
}