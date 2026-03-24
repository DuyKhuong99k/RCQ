using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
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
    IMayCansService mayCansService, IConverterService converterService)
    : Hub
{
    public async Task Auth(string id, int wType, string dataJson)
    {
        try
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
            else
            {
                await  mayCansService.CommandSetUpdateTime(id);
                //await mayCansService.CommandGetChiSanLuong(id);
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
       
    }

    public async Task GetConnectionId()
    {
        try
        {
            await Clients.Caller.SendAsync("CONNECTIONID", Context.ConnectionId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
        
    }

    public async Task GetChiSanLuong(string id)
    {
        try
        {
            await mayCansService.CommandGetChiSanLuong(id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
        
    }

    public async Task GetXacDinh(string id)
    {
        try
        {
            await mayCansService.CommandGetXacDinh(id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
        
    }

    public override async Task<Task> OnConnectedAsync()
    {
        try
        {
            var feature = Context.Features.Get<IHttpConnectionFeature>();
            // await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId}", $"{feature.RemoteIpAddress}",
            //     $"{feature.RemotePort}", $"{feature.LocalIpAddress}", $"{feature.LocalPort}");
#if DEBUG
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {Context.ConnectionId} - {feature.RemoteIpAddress} Connected.");
#endif
            userService.Clients.AddOrUpdate(Context.ConnectionId,new ClientInfo
            {
                ConnectedId = Context.ConnectionId,
                DateTimeConnected = DateTime.Now,
                IPAddr = feature.RemoteIpAddress?.ToString(),
                WKv = AppKV.Main
            }, (key, oldValue) =>
            {
                oldValue.DateTimeConnected = DateTime.Now;
                oldValue.IPAddr = feature.RemoteIpAddress?.ToString();
                //oldValue.WKv = AppKV.Main;
                return oldValue;
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
        
        return
            base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            userService.RemoveByConnectedId(Context.ConnectionId);
            mayCansService.DisConnectionId(Context.ConnectionId);
            //mayCansService.NotifyItemChanged(mayCan);
#if DEBUG
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {Context.ConnectionId}  DisConnected.");
#endif
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
       
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendData(string command, int len, string dataJson)
    {
        try
        {
            var client = userService.GetByConnectedId(Context.ConnectionId);
            if (client == null || client.IsMayCan == false) return;
            var dataToSend = await communicationService.Route(client, command, len, dataJson);
            //await Task.Delay(300);
            //dataToSend=   $"\"{ dataToSend.Replace("\\", "")}\"";
            await Clients.Caller.SendAsync("ReceiveData",command, dataToSend);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
            //throw;
        }
       
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
        try
        {
            var client = userService.GetByConnectedId(Context.ConnectionId);
            //if(client == null || client.IsMayCan == true) return;
            if (client == null || client.WKv != AppKV.XepKhuon) return;
            var dataToSend = await communicationService.Route(client, command, len, dataJson);
            //await Task.Delay(300);
            await Clients.Caller.SendAsync("ReceiveDataCoi", command, dataToSend);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
       
    }

    public async Task SendId(string message)
    {
        try
        {
            await Clients.Caller.SendAsync("ReceiveId", message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
       
    }

    public async Task SendMessage(string user, string message)
    {
        try
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
        
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

                    mayCansService.NotifyItemChanged(mayCan);
                }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
    }
    public async Task SendSelectedData2(string id, AppKV kv, string dataJson)
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
                        if (property != null)
                        {
                            property.SetValue(mayCan, dataCmd.PropetyValue);
                            switch (dataCmd.PropetyName)
                            {
                                case nameof(mayCan.MaThanhPham):
                                    var thanhPham = converterService.ThanhPhamToName(dataCmd.PropetyValue,mayCan.WKv);
                                  await  mayCansService.CommandSetThanhPham(id, dataCmd.PropetyValue, thanhPham);
                                    break;
                                case nameof(mayCan.MaSize):
                                    var size = converterService.SizeToName(dataCmd.PropetyValue,mayCan.WKv);
                                    await mayCansService.CommandSetSize(id, dataCmd.PropetyValue, size);
                                    break;
                                case nameof(mayCan.MaLoaiNguyenLieu):
                                    var nguyenlieu = converterService.LoaiNguyenLieuToName(dataCmd.PropetyValue);
                                    await mayCansService.CommandSetNguyenLieu(id, dataCmd.PropetyValue, nguyenlieu);
                                    break;
                                case nameof(mayCan.MaLo):
                                   await mayCansService.CommandSetLo(id, dataCmd.PropetyValue);
                                    break;
                                case nameof(mayCan.MaAo):
                                    var ao = converterService.AoToName(dataCmd.PropetyValue);
                                    await mayCansService.CommandSetAo(id, dataCmd.PropetyValue, ao);
                                    break;
                                case nameof(mayCan.MaChatLuong):
                                    var chatluong = converterService.ChatLuongXepKhuonToName(dataCmd.PropetyValue);
                                    await mayCansService.CommandSetChatLuong(id, dataCmd.PropetyValue, chatluong);
                                    break;
                                case nameof(mayCan.MaChieuXa):
                                    var chieuxa = converterService.ChieuXaToName(dataCmd.PropetyValue);
                                    await mayCansService.CommandSetChieuXa(id, dataCmd.PropetyValue, chieuxa);
                                    break;
                                case nameof(mayCan.MaCoi):
                                    var coi = converterService.CoiToName(dataCmd.PropetyValue);
                                    await mayCansService.CommandSetCoi(id, dataCmd.PropetyValue, coi);
                                    break;
                                case nameof(mayCan.MaPhuongTien):
                                    var phuongtien = converterService.PhuongTienToName(dataCmd.PropetyValue);
                                    await mayCansService.CommandSetPhuongTien(id, dataCmd.PropetyValue, phuongtien);
                                    break;
                                case nameof(mayCan.MaKhachHang):
                                    var khachHang = converterService.KhachHangXepKhuonToName(dataCmd.PropetyValue);
                                    await mayCansService.CommandSetKhachHang(id, dataCmd.PropetyValue, khachHang);
                                    break;
                                case nameof(mayCan.MaNet):
                                    var net = converterService.NetXepKhuonToName(dataCmd.PropetyValue);
                                    await mayCansService.CommandSetNet(id, dataCmd.PropetyValue, net);
                                    break;
                                case nameof(mayCan.MaMau):
                                    var mau = converterService.MauToName(dataCmd.PropetyValue,mayCan.WKv);
                                    await mayCansService.CommandSetMau(id, dataCmd.PropetyValue, mau);
                                    break;
                                case nameof(mayCan.MaCongViec):
                                    var congviec = converterService.CongViecToName(dataCmd.PropetyValue,mayCan.WKv);
                                    await mayCansService.CommandSetCongViec(id, dataCmd.PropetyValue, congviec);
                                    break;
                            }
                        }

                    }

                    mayCansService.NotifyItemChanged(mayCan);
                }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
    }
    public async Task KeepAlive()
    {
        #if DEBUG
        Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {Context.ConnectionId}] KeepAlive received.");
        #endif
        await Clients.Caller.SendAsync("KeepAlive","KeepAlive", "OK");
        await Task.CompletedTask;
    }
}