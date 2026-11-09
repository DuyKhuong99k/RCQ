using Microsoft.AspNetCore.Http.Features;
using System.Diagnostics;
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
    IMayCansService mayCansService, IConverterService converterService,
    ILogger<ChatHub> logger)
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
        var isLite = string.Equals(command, "LITE", StringComparison.OrdinalIgnoreCase);
        var isAttendance = string.Equals(command, "CHAMCONG", StringComparison.OrdinalIgnoreCase);
        var traceId = isAttendance ? Guid.NewGuid().ToString("N") : null;
        string? machineId = null;
        HqFirmwareSend.Scope? delivery = null;
        try
        {
            var client = userService.GetByConnectedId(Context.ConnectionId);
            machineId = client?.Id;
            if (client?.WKv == AppKV.Hq && (isLite || isAttendance))
                delivery = HqFirmwareSend.Begin(Context.ConnectionId, Context.ConnectionAborted);
            if (traceId != null)
            {
                string? scanId = null;
                try
                {
                    var token = JObject.Parse(dataJson)["ChamCongId"];
                    if (token?.Type == JTokenType.String) scanId = token.Value<string>();
                }
                catch (JsonException) { /* Router sẽ trả lỗi JSON. */ }
                logger.LogInformation(
                    "[CHAMCONG RX] UTC={Utc:O} Host={Host} PID={Pid} Trace={Trace} Connection={Connection} Machine={Machine} ChamCongId={ScanId} Len={Length} Hash={Hash}",
                    DateTimeOffset.UtcNow, Environment.MachineName, Environment.ProcessId,
                    traceId, Context.ConnectionId, client?.Id, scanId, len, TraceHash(dataJson));
            }
            if (client == null || client.IsMayCan == false)
            {
                if (traceId != null)
                    logger.LogWarning("[CHAMCONG REJECT] Trace={Trace} Connection={Connection}: kết nối chưa xác định là máy cân.",
                        traceId, Context.ConnectionId);
                var rejectError = CreateSendDataError(command, dataJson, machineId,
                    "Kết nối chưa xác định là máy cân.");
                if (delivery != null || isLite)
                {
                    delivery ??= HqFirmwareSend.Begin(Context.ConnectionId, Context.ConnectionAborted);
                    await DeliverHqReply(command, dataJson, rejectError, delivery, machineId, traceId, 0);
                }
                else await Clients.Caller.SendAsync("ReceiveData", command, rejectError);
                return;
            }
            var routeStarted = Stopwatch.GetTimestamp();
            var dataToSend = await communicationService.Route(client, command, len, dataJson);
            var routeMs = Stopwatch.GetElapsedTime(routeStarted).TotalMilliseconds;
            if (delivery != null)
            {
                await DeliverHqReply(command, dataJson, dataToSend, delivery, machineId, traceId, routeMs);
                return;
            }
            // HQ online has already returned through its bounded delivery path.
            await Clients.Caller.SendAsync("ReceiveData", command, dataToSend);
            if (traceId != null)
                logger.LogInformation(
                    "[CHAMCONG TX] UTC={Utc:O} PID={Pid} Trace={Trace} Connection={Connection} Hash={Hash}",
                    DateTimeOffset.UtcNow, Environment.ProcessId, traceId,
                    Context.ConnectionId, TraceHash(dataToSend));
        }
        catch (Exception e)
        {
            if (delivery != null)
            {
                HqFirmwareSend.Log($"[HQ ROUTE ERROR] Machine={machineId}; Error={e.GetType().Name}; Outcome=UNKNOWN");
                var fallback = CreateSendDataError(command, dataJson, machineId,
                    ScaleOnlineMessages.ProcessingError, HqOnlineResponse.Unknown);
                await DeliverHqReply(command, dataJson, fallback, delivery, machineId, traceId, 0);
                return;
            }
            if (Context.ConnectionAborted.IsCancellationRequested) return;
            if (traceId != null)
                logger.LogError(e, "[CHAMCONG ERROR] Trace={Trace} Connection={Connection}",
                    traceId, Context.ConnectionId);
            else Console.WriteLine(e);

            try
            {
                var errorResponse = CreateSendDataError(command, dataJson, machineId,
                    isLite ? ScaleOnlineMessages.ProcessingError : "Máy chủ bận hoặc lỗi xử lý: " + e.Message,
                    isLite ? HqOnlineResponse.Unknown : null);
                await Clients.Caller.SendAsync("ReceiveData", command, errorResponse);
            }
            catch { /* tránh ngoại lệ lồng */ }
        }
        finally
        {
            delivery?.Dispose();
        }
    }

    private static string CreateSendDataError(string command, string request, string? machine,
        string message, string? saveStatus = null)
    {
        if (!string.Equals(command, "LITE", StringComparison.OrdinalIgnoreCase))
            return JsonConvert.SerializeObject(new DataError { Cmd = command, ErrorString = message, SaveStatus = saveStatus });

        var error = HqOnlineResponse.Error(command, message, HqOnlineResponse.Correlation(request),
            machine, saveStatus ?? HqOnlineResponse.NotSaved);
        return JsonConvert.SerializeObject(new DataTranfer
        {
            IsError = true,
            DataError = JsonConvert.SerializeObject(error)
        });
    }

    private async Task DeliverHqReply(string command, string request, string reply, HqFirmwareSend.Scope scope,
        string? machine, string? trace, double routeMs)
    {
        if (!scope.TryBeginReply())
        {
            HqFirmwareSend.Log($"[HQ REPLY SKIPPED] Machine={machine}; Connection={scope.ConnectionId}; Original reply already attempted; SQL unchanged");
            AbortCaller();
            return;
        }
        var failure = scope.SendFailure;
        if (failure == null)
        {
            var result = await HqFirmwareSend.RunAsync(token => Clients.Caller.SendAsync("ReceiveData", command, reply, token),
                HqFirmwareSend.ReplyBudget, scope.ConnectionAborted);
            if (result.Sent)
            {
                // Send completion is not a firmware application ACK. Logging must not resend a completed frame.
                try
                {
                    if (routeMs + result.Milliseconds >= 2000)
                        logger.LogWarning("[HQ WSS SLOW] Machine={Machine} Connection={Connection} RouteMs={RouteMs:F0} SendMs={SendMs:F0}",
                            machine, scope.ConnectionId, routeMs, result.Milliseconds);
                    if (trace != null)
                        logger.LogInformation("[CHAMCONG TX] UTC={Utc:O} PID={Pid} Trace={Trace} Connection={Connection} Hash={Hash}",
                            DateTimeOffset.UtcNow, Environment.ProcessId, trace, scope.ConnectionId, TraceHash(reply));
                }
                catch { }
                return;
            }
            failure = new("ReceiveData", result.Reason ?? "UNCONFIRMED");
            scope.Fail(failure.Target, failure.Reason);
        }
        // Parse audit-only metadata only on failed delivery, not on every successful ticket.
        var status = HqFirmwareSend.SaveStatus(reply);
        var id = ReplyCorrelationId(command, request);
        HqFirmwareSend.Log($"[HQ TRANSPORT UNCONFIRMED] Machine={machine}; Id={id}; Connection={scope.ConnectionId}; Cmd={command}; SaveStatus={status}; Target={failure.Target}; Reason={failure.Reason}; No retry, SQL unchanged");
        // A timed-out write may still be pending below SignalR. Abort the ORIGINAL connection
        // rather than sending a contradictory fallback or letting late frames reach the next request.
        AbortCaller();
    }

    private static string? ReplyCorrelationId(string command, string request)
    {
        var id = HqOnlineResponse.Correlation(request)?.Id;
        if (id != null || !string.Equals(command, "CHAMCONG", StringComparison.OrdinalIgnoreCase)) return id;

        try
        {
            var scanId = JObject.Parse(request).Value<string>("ChamCongId");
            return scanId?.Length <= 100 ? scanId : null;
        }
        catch { return null; }
    }

    private void AbortCaller()
    {
        try { Context.Abort(); }
        catch { /* The original connection may already have closed. */ }
    }

    private static string TraceHash(string value) =>
        Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(value)));

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
            if (mayCan?.WKv == AppKV.Hq)
            {
                if (!await ApplyHqSelection(mayCan, datas, echo: false)) return;
                datas = datas?.Where(x => !HqScaleSelection.IsSelector(x.PropetyName)).ToList();
                if (datas?.Count == 0) return;
            }
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
            if (mayCan?.WKv == AppKV.Hq)
            {
                if (!await ApplyHqSelection(mayCan, datas, echo: true)) return;
                datas = datas?.Where(x => !HqScaleSelection.IsSelector(x.PropetyName)).ToList();
                if (datas?.Count == 0) return;
            }
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
    private async Task<bool> ApplyHqSelection(Models.Repos.Models.MayCan machine, List<DataCMD>? data, bool echo)
    {
        if (data == null) return false;
        var client = userService.GetByConnectedId(Context.ConnectionId);
        if (client?.IsMayCan != true || !client.IsActive || client.Id != machine.Id ||
            machine.ConnectionId != Context.ConnectionId) return false;
        return await HqOnlineWeightWriter.ForMachine(machine.Id, async () =>
        {
            var accepted = HqScaleSelection.ApplyFirmware(machine, Context.ConnectionId, data);
            // Echo is NOT another web Set: do not create a second pending change.
            // Hold the device gate through the reply so a newer web Set cannot be overtaken by this echo.
            foreach (var item in accepted)
            {
                if (!echo) continue;
                var target = item.PropetyName switch
                {
                    nameof(machine.MaThanhPham) => "SETTHANHPHAM",
                    nameof(machine.MaLoaiNguyenLieu) => "SETLOAINGUYENLIEU",
                    nameof(machine.MaSize) => "SETSIZE",
                    _ => "SETLO"
                };
                if (machine.MType == "DESKTOP" || item.PropetyName == nameof(machine.MaLo))
                    await Clients.Caller.SendAsync(target, item.PropetyValue);
                else if (machine.MType == "BOARD")
                {
                    var name = item.PropetyName switch
                    {
                        nameof(machine.MaThanhPham) => converterService.ThanhPhamToName(item.PropetyValue, machine.WKv),
                        nameof(machine.MaLoaiNguyenLieu) => converterService.LoaiNguyenLieuToName(item.PropetyValue),
                        _ => converterService.SizeToName(item.PropetyValue, machine.WKv)
                    };
                    await Clients.Caller.SendAsync(target, item.PropetyValue, name);
                }
            }
            if (accepted.Count > 0) mayCansService.NotifyItemChanged(machine);
            return true;
        });
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
