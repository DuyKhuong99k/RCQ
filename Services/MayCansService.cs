using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Timers;
using System.Threading;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Models.Repos.Models;
using Newtonsoft.Json;
using ToolsEx;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using Timer = System.Timers.Timer;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Services;

public partial class MayCansService : ObservableObject, IMayCansService
{
    private const int MaxSize = 150;
    private const int MinSize = 100;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly HighThroughputQueue<PhieuCanBTPDinhHinh> _btpDinhHinhQueue;
    private readonly int _debounceDelay = 100;
    private readonly object _lock = new();
    private readonly ConcurrentDictionary<string, MayCan> _pendingUpdates = new();
    private System.Threading.Timer? _notifyDebounceTimer;
    private readonly object _notifyTimerLock = new();
    private readonly HighThroughputQueue<PhieuCanTPDinhHinh> _tpDinhHinhQueue;
    private readonly IHubContext<ChatHub> hubContext;
    private readonly IMainService vmMain;
    // private CancellationTokenSource _debounceCancellationTokenSource;
    private Timer? _timer;
    private readonly HttpClient Http;
   
    [ObservableProperty] private ConcurrentDictionary<string, MayCan> items = new();

    [ObservableProperty] private List<MayCan> selectedItems = new();

    public MayCansService(IMainService vmMain, IHubContext<ChatHub> hubContext,
        HighThroughputQueue<PhieuCanBTPDinhHinh> btpDinhHinhQueue,
        HighThroughputQueue<PhieuCanTPDinhHinh> tpDinhHinhQueue, IHostApplicationLifetime appLifetime,IHttpClientFactory httpClientFactory)
    {
        _appLifetime = appLifetime;
        this.vmMain = vmMain;
        this.hubContext = hubContext;
        _btpDinhHinhQueue = btpDinhHinhQueue;
        Http = httpClientFactory.CreateClient();
        _tpDinhHinhQueue = tpDinhHinhQueue;
        try
        {
            //StartTimer();
            Load();
            // Task.Run(() => StartCleanQueue(_appLifetime.ApplicationStopping));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // Console.WriteLine(e);
            //throw;
            VmMessage.SetExceptionCommand.Execute(e);
        }
    }

    

    private AppViewModel VmApp => AppViewModel.Instance;
    private MessageViewModel VmMessage => MessageViewModel.Instance;

    #region IMayCansService Members

    public MayCan? Find(string id)
    {
        try
        {
            Items.TryGetValue(id, out var mayCan);
            return mayCan;
            //return Items.FirstOrDefault(x => x.Id == id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // throw;
            return null;
        }
    }

    public MayCan? Find(string id, bool isConnected)
    {
        var mayCan = Find(id);
        if (mayCan == null) return null;
        return mayCan.IsConnected ? mayCan : null;
    }

    public void AddItem(MayCan mayCan, object item)
    {
        if (mayCan.Items.Count > 100) mayCan.Items.Remove(mayCan.Items.Last());

        mayCan.Items.Insert(0, item);
        ItemUpdated?.Invoke(mayCan);
    }

    public void AddorUpdate(MayCan? mayCan)
    {
        try
        {
            if (mayCan == null) return;
            Items.AddOrUpdate(mayCan.Id, item =>
            {
                ItemAdded?.Invoke(mayCan);
                return mayCan;
            }, (id, oldMayCan) =>
            {
                oldMayCan.CopyFrom(mayCan);
                ItemUpdated?.Invoke(mayCan);
                return oldMayCan;
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void Remove(string id)
    {
        try
        {
            // var item = Items.FirstOrDefault(x => x.Id == id);
            // if (item != null) Items.Replace(item);
            if (Items.TryRemove(id, out _)) ItemRemoved?.Invoke(id);
            //Console.WriteLine($"Item with id {id} not found.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void Dis(string id)
    {
        try
        {
            var item = Find(id);
            if (item != null)
            {
                item.IsActive = false;
                //if (item.AType != AppType._type5)
                item.IsConnected = false;
                item.ConnectionId = "";
                ItemUpdated?.Invoke(item);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void DisConnectionId(string id)
    {
        try
        {
            var item = Items.Values.FirstOrDefault(x => x.ConnectionId == id);
            if (item != null)
            {
                item.IsActive = false;
                //if (item.AType != AppType._type5)
                item.IsConnected = false;
                item.ConnectionId = "";
                ItemUpdated?.Invoke(item);
                // Update(item);
            }
            //OnItemChanged();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task ExpandHandler(MayCan mayCan)
    {
        mayCan.IsExpand = !mayCan.IsExpand;
        ItemUpdated?.Invoke(mayCan);
    }

    public void SetAllExpand(bool isExpanded)
    {
        foreach (var mayCan in Items.Values.ToList())
        {
            mayCan.IsExpand = isExpanded;
            ItemUpdated?.Invoke(mayCan);
        }
    }

    public void SetIsConnected(bool isConnected, string id)
    {
        // for (var i = 0; i < Items.Count; i++)
        // {
        //     var item = Items[i];
        //     //if (item.Id == id && item.AType != AppType._type5) item.IsConnected = isConnected;
        //     if (item.Id == id) item.IsConnected = isConnected;
        // }
        var mayCan = Find(id);
        if (mayCan != null)
        {
            mayCan.IsConnected = isConnected;
            ItemUpdated?.Invoke(mayCan);
            //if (mayCan.AType != AppType._type5)
            //    mayCan.IsConnected = isConnected;
            //else
            //    mayCan.IsConnected = true;
        }
        // OnItemChanged();
    }

    // public void Update(MayCan mayCan)
    // {
    //     // for (var i = 0; i < Items.Count; i++)
    //     // {
    //     //     var item = Items[i];
    //     //     if (item.Id == mayCan.Id)
    //     //     {
    //     //         item.MType = mayCan.MType;
    //     //         item.ConnectionId = mayCan.ConnectionId;
    //     //         item.DateTimeConnected = mayCan.DateTimeConnected;
    //     //         item.IPAddr = mayCan.IPAddr;
    //     //         item.IsActive = mayCan.IsActive;
    //     //         //if (item.AType != AppType._type5)
    //     //         item.IsConnected = mayCan.IsConnected;
    //     //         //else
    //     //         //    item.IsConnected = true;
    //     //         item.WKv = mayCan.WKv;
    //     //         item.AType = mayCan.AType;
    //     //     }
    //     // }
    //
    //     OnItemChanged();
    // }

    // public event EventHandler? ItemChanged;
    public event Action<MayCan>? ItemAdded;
    public event Action<MayCan>? ItemUpdated;
    public event Action<string>? ItemRemoved;

    public void NotifyItemChanged(MayCan mayCan)
    {
        try
        {
            // Coalesce: keep only the latest update per MayCan within the debounce window
            _pendingUpdates[mayCan.Id] = mayCan;
            StartOrResetNotifyDebounceTimer();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Queue notify failed: {ex.Message}");
        }
    }

    private void StartOrResetNotifyDebounceTimer()
    {
        var due = _debounceDelay > 0 ? _debounceDelay : 50;
        lock (_notifyTimerLock)
        {
            if (_notifyDebounceTimer == null)
            {
                // Single-shot timer; we recreate/replace it on subsequent calls
                _notifyDebounceTimer = new System.Threading.Timer(_ =>
                {
                    try
                    {
                        // Take snapshot and clear to accept new updates while flushing
                        var snapshot = _pendingUpdates.ToArray();
                        _pendingUpdates.Clear();

                        // Push out at most once per item per window
                        foreach (var kv in snapshot)
                        {
                            try
                            {
                                ItemUpdated?.Invoke(kv.Value);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Debounced update failed: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Flush pending updates failed: {ex.Message}");
                    }
                    finally
                    {
                        // Dispose timer after the single run
                        lock (_notifyTimerLock)
                        {
                            _notifyDebounceTimer?.Dispose();
                            _notifyDebounceTimer = null;
                        }
                    }
                }, null, due, Timeout.Infinite);
            }
            else
            {
                try
                {
                    // Reset the timer window
                    _notifyDebounceTimer.Change(due, Timeout.Infinite);
                }
                catch (ObjectDisposedException)
                {
                    // In rare races, timer was disposed between check and change; recreate it
                    _notifyDebounceTimer = new System.Threading.Timer(_ =>
                    {
                        try
                        {
                            var snapshot = _pendingUpdates.ToArray();
                            _pendingUpdates.Clear();
                            foreach (var kv in snapshot)
                            {
                                try { ItemUpdated?.Invoke(kv.Value); }
                                catch (Exception ex) { Console.WriteLine($"Debounced update failed: {ex.Message}"); }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Flush pending updates failed: {ex.Message}");
                        }
                        finally
                        {
                            lock (_notifyTimerLock)
                            {
                                _notifyDebounceTimer?.Dispose();
                                _notifyDebounceTimer = null;
                            }
                        }
                    }, null, due, Timeout.Infinite);
                }
            }
        }
    }

    public void NotifyItemsChanged(IEnumerable<MayCan> mayCans)
    {
        try
        {
            foreach (var mc in mayCans)
            {
                _pendingUpdates[mc.Id] = mc;
            }
            StartOrResetNotifyDebounceTimer();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Queue bulk notify failed: {ex.Message}");
        }
    }
    public async Task<bool> CommandDoiKhuVuc(string id, AppKV khuVuc)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                var _appKV = "HQ";
                var amode = "1D0";
                switch (khuVuc)
                {
                    case AppKV.Main:
                    {
                        
                        break;
                    }
                    case AppKV.DauAo:
                        break;
                    case AppKV.NguyenLieu:
                    {
                        _appKV = "NL";
                        amode = "1DM";
                        break;
                    }
                    case AppKV.BTPFillet:
                    {
                        _appKV = "BTP";
                        amode = "1D0";
                        break;
                    }
                    case AppKV.TPFillet:
                    {
                        _appKV = "TP";
                        amode = "1D0";
                        break;
                    }
                    case AppKV.BTPFilletv2:
                    {
                        _appKV = "BTP";
                        amode = "1D0";
                        break;
                    }
                    case AppKV.TPFilletv2:
                    {
                        _appKV = "TP";
                        amode = "1D0";
                        break;
                    }
                    case AppKV.PhuPham:
                        break;
                    case AppKV.BTPDinhHinh:
                    {
                        _appKV = "BTP";
                        amode = "1D0";
                        break;
                    }
                    case AppKV.TPDinhHinh:
                    {
                        _appKV = "TP";
                        amode = "1D0";
                        break;
                    }
                    case AppKV.XepKhuon:
                    {
                        _appKV = "XK";
                        amode = "1DM";
                        break;
                    }
                    case AppKV.BaoTu:
                        break;
                    case AppKV.CaoThit:
                        break;
                    case AppKV.XepKhuonRaCoi:
                    {
                        _appKV = "XK";
                        amode = "1DM";
                        break;
                    }
                    case AppKV.XepKhuonPhu:
                    {
                        _appKV = "XKPHU";
                        amode = "1D0";
                        break;
                    }
                    case AppKV.XepKhuonKXL:
                        break;
                    case AppKV.PhuPhamv2:
                        break;
                    case AppKV.Hq:
                    {
                        _appKV = "HQ";
                        amode = "1D0";
                        break;
                    }
                    case AppKV.XepKhuonBlock:
                    {
                        _appKV = "XKBLOCK";
                        amode = "1D0";
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(khuVuc), khuVuc, null);
                }
                var url = item.IPAddr;
                var _url = $"http://{url}/api/w/posts";
                var data = new
                {
                    
                    Path = "appkv",
                    akv = _appKV,
                    amode = amode,
                    KVName  = khuVuc.ToString()
                };
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, _url)
                {
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
                };

                var response = await Http.SendAsync(requestMessage);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
                

                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }
    public async Task<bool> CommandZero(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                item.TrongLuongTare = 0;
                //if (item.AType != AppType._type5)
                //{
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("ZERO", "0");
                //}
                //else
                //{
                //    var url = $"http://{item.IPAddr}/api/w/posts";
                //    var data = new { CMD = "ZERO",Path="zero"};
                //    var dataJson = JsonConvert.SerializeObject(data);
                //    var dJson1 = HttpPost($"{url}",dataJson);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetNapThe(string id, bool isNapThe)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                item.IsNapThe = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandTare(string id, decimal? trongLuongTare)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                //if (item.AType != AppType._type5)
                //{
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("TARE", $"{trongLuongTare ?? 0}");
                //}
                //else
                //{
                //    var url = $"http://{item.IPAddr}/api/w/posts";
                //    var _trongLuongTare = trongLuongTare ?? 0;
                //    var data = new { trongluongtare = _trongLuongTare,Path="tare"};
                //    var dataJson = JsonConvert.SerializeObject(data);
                //    var dJson1 = HttpPost($"{url}",dataJson);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandGetLo(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                //if (item.AType != AppType._type5)
                //{
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETLO", "");
                //}
                //else
                //{
                //    var url = $"http://{item.IPAddr}/api/lo/gets?Path=getselecteditem";
                //    var dataJson = HttpGet(url);
                //    var json = JObject.Parse(dataJson);
                //    var datacount = 0;
                //    int.TryParse(json["datacount"]?.ToString(), out datacount);
                //    if (datacount > 0)
                //    {

                //        var maLo = (json.SelectToken("data[0].Id"))?.ToString();
                //        item.MaLo = maLo ?? string.Empty;
                //    }
                //}
            }
            catch (Exception e)
            {
                vmMain.VmMessage.MessageBoxShow(e.ToString(), "", 0);
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }


    public async Task<bool> CommandSetLo(string id, string maLo)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                //if (item.AType != AppType._type5)
                //{
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETLO", $"{maLo}");
                //    }
                //    else
                //    {
                //        var dataInsert = new { Id = maLo,NgayNguyenLieu = DateTime.Now.ToString("yyyyMMddhhmmss"),SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="insert"};
                //        var dataSelecteditem = new { Id = maLo,NgayNguyenLieu = DateTime.Now.ToString("yyyyMMddhhmmss"),SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="selecteditem"};
                //        if (item.WKv == AppKV.Hq)
                //        {
                //            var lo = vmMain.VmLo.GetHqLo(maLo);
                //            if (lo != null)
                //            {
                //                dataInsert = new
                //                {
                //                    Id = maLo, NgayNguyenLieu = $"{lo.NgayNguyenLieu.ToString("yyyyMMdd")}000000",
                //                    SuDung = lo.SuDung, MNgay = lo.MNgay.ToString("yyyyMMdd"), Path = "insert"
                //                };
                //                dataSelecteditem = new
                //                {
                //                    Id = maLo, NgayNguyenLieu = $"{lo.NgayNguyenLieu.ToString("yyyyMMdd")}000000",
                //                    SuDung = lo.SuDung, MNgay = lo.MNgay.ToString("yyyyMMdd"), Path = "selecteditem"
                //                };
                //            }
                //            else
                //            {
                //               var rlIn =  vmMain.VmLoHq.Insert(new HQ_Lo()
                //                {
                //                    Id = maLo, NgayNguyenLieu = DateOnly.FromDateTime(DateTime.Now), MNgay = DateTime.Now,
                //                    SuDung = true
                //                });
                //                if (rlIn >0)
                //                {
                //                    vmMain.VmLoHq.Items.Insert(0,maLo);
                //                    vmMain.VmLo.Items.Insert(0,maLo);
                //                }
                //            }
                //        }

                //        var url = $"http://{item.IPAddr}/api/lo/posts";

                //        var dataJsonInsert = JsonConvert.SerializeObject(dataInsert);
                //        var dataJsonSelecteditem = JsonConvert.SerializeObject(dataSelecteditem);
                //        var dJson1 = HttpPost($"{url}",dataJsonInsert);
                //        var dJson2 = HttpPost($"{url}", dataJsonSelecteditem);

                //        //var json = JObject.Parse(dataJson);
                //        //var datacount = 0;
                //        //int.TryParse(json["datacount"]?.ToString(), out datacount);
                //        //if (datacount > 0)
                //        //{

                //        //    //var maLo = (json.SelectToken("data[0].Id"))?.ToString();
                //        item.MaLo = maLo;
                //        //}
                //    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetPhuongTien(string id, string phuongTienId, string phuongTienName)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "DESKTOP")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETPHUONGTIEN", $"{phuongTienId}");
                else if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETPHUONGTIEN", $"{phuongTienId}", $"{phuongTienName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetKhachHang(string id, string khachHangId, string khachHangName)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "DESKTOP")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETKHACHHANG", $"{khachHangId}");
                else if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETKHACHHANG", $"{khachHangId}", $"{khachHangName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetNet(string id, string netId, string netName)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "DESKTOP")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETNET", $"{netId}");
                else if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETNET", $"{netId}", $"{netName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetMau(string id, string mauId, string mauName)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "DESKTOP")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETMAU", $"{mauId}");
                else if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETMAU", $"{mauId}", $"{mauName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetCongViec(string id, string congViecId, string congViecName)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "DESKTOP")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETCONGVIEC", $"{congViecId}");
                else if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETCONGVIEC", $"{congViecId}", $"{congViecName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }


    public async Task<bool> CommandSetInfoCoi(string id, string dataCoiJson)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETINFOCOI", dataCoiJson);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandGetThanhPham(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                //if (item.AType != AppType._type5)
                //{
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETTHANHPHAM", "");
                //}
                //else
                //{
                //    var url = $"http://{item.IPAddr}/api/thanhpham/gets?Path=getselecteditem";
                //    var dataJson = HttpGet(url);
                //    var json = JObject.Parse(dataJson);
                //    var datacount = 0;
                //    int.TryParse(json["datacount"]?.ToString(), out datacount);
                //    if (datacount > 0)
                //    {

                //        var maThanhPham = (json.SelectToken("data[0].Id"))?.ToString();
                //        item.MaThanhPham = maThanhPham ?? string.Empty;
                //    }
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandGetChieuXa(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETCHIEUXA", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandGetCoi(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETCOI", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetCoi(string id, string coiId, string coiName)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "DESKTOP")
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETCOI", $"{coiId}");
                else if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETCOI", $"{coiId}", $"{coiName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandGetAo(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETAO", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetAo(string id, string aoId, string aoName)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "DESKTOP")
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETAO", $"{aoId}");
                else if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETAO", $"{aoId}", $"{aoName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandGetPhuongTien(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETPHUONGTIEN", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandGetChatLuong(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETCHATLUONG", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetThanhPham(string id, string thanhPhamId, string thanhPhamName)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                //if (item.AType != AppType._type5)
                //{
                if (item.WKv == AppKV.XepKhuonRaCoi)
                {
                    try
                    {
                        var thanhPham = vmMain.VmThanhPhamChinhXepKhuon.Items.FirstOrDefault(x=>x.Ma == thanhPhamId);
                        if (thanhPham != null)
                        {
                            item.ThamSoTangTrong = thanhPham.ThamSoTangTrong;
                            NotifyItemChanged(item);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        // throw;
                    }
                   
                }
                if (item.MType == "DESKTOP")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETTHANHPHAM", $"{thanhPhamId}");
                else if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETTHANHPHAM", $"{thanhPhamId}", $"{thanhPhamName}");
                
                //}
                //else
                //{

                //    var url = $"http://{item.IPAddr}/api/thanhpham/posts";
                //    var dataIsnert = new { Id = thanhPhamId,Ten = thanhPhamName,SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="insert"};
                //    var dataSelecteditem = new { Id = thanhPhamId,Ten = thanhPhamName,SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="selecteditem"};
                //    var dataJsonInsert = JsonConvert.SerializeObject(dataIsnert);
                //    var dataJsonSelecteditem = JsonConvert.SerializeObject(dataSelecteditem);
                //    var dJson1 = HttpPost($"{url}",dataJsonInsert);
                //    var dJson2 = HttpPost($"{url}", dataJsonSelecteditem);
                //    item.MaThanhPham = thanhPhamId;
                //    //}
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandGetNguyenLieu(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                //if (item.AType != AppType._type5)
                //{
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETLOAINGUYENLIEU", "");
                //}
                //else
                //{
                //    var url = $"http://{item.IPAddr}/api/loainguyenlieu/gets?Path=getselecteditem";
                //    var dataJson = HttpGet(url);
                //    var json = JObject.Parse(dataJson);
                //    var datacount = 0;
                //    int.TryParse(json["datacount"]?.ToString(), out datacount);
                //    if (datacount > 0)
                //    {

                //        var maNguyenLieu = (json.SelectToken("data[0].Id"))?.ToString();
                //        item.MaLoaiNguyenLieu = maNguyenLieu ?? string.Empty;
                //    }
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetNguyenLieu(string id, string nguyenLieuId, string nguyenLieuName)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                //if (item.AType != AppType._type5)
                //{
                if (item.MType == "DESKTOP")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETLOAINGUYENLIEU", $"{nguyenLieuId}");
                else if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETLOAINGUYENLIEU", $"{nguyenLieuId}", $"{nguyenLieuName}");
                //}
                //else
                //{

                //    var url = $"http://{item.IPAddr}/api/loainguyenlieu/posts";
                //    var dataInsert = new { Id = nguyenLieuId,Ten = nguyenLieuName,SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="insert"};
                //    var dataSelectedItem = new { Id = nguyenLieuId,Ten = nguyenLieuName,SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="selecteditem"};
                //    var dataJsonInsert = JsonConvert.SerializeObject(dataInsert);
                //    var dataJsonSelectedItem = JsonConvert.SerializeObject(dataSelectedItem);
                //    var dJson1 = HttpPost($"{url}",dataJsonInsert);
                //    var dJson2 = HttpPost($"{url}", dataJsonSelectedItem);
                //    item.MaLoaiNguyenLieu = nguyenLieuId;
                //    //}
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
                //return false;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandGetSize(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                //if (item.AType != AppType._type5)
                //{
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETSIZE", "");
                //}
                //else
                //{
                //    var url = $"http://{item.IPAddr}/api/size/gets?Path=getselecteditem";
                //    var dataJson = HttpGet(url);
                //    var json = JObject.Parse(dataJson);
                //    var datacount = 0;
                //    int.TryParse(json["datacount"]?.ToString(), out datacount);
                //    if (datacount > 0)
                //    {

                //        var masize = (json.SelectToken("data[0].Id"))?.ToString();
                //        item.MaSize = masize ?? string.Empty;
                //    }
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetSize(string id, string sizeId, string sizeName)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                //if (item.AType != AppType._type5)
                //{
                if (item.MType == "DESKTOP")
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETSIZE", $"{sizeId}");
                else if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETSIZE", $"{sizeId}", $"{sizeName}");
                //}
                //else
                //{

                //    var url = $"http://{item.IPAddr}/api/size/posts";
                //    var dataInsert = new { Id = sizeId,Ten = sizeName,SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="insert"};
                //    var dataSelectedItem = new { Id = sizeId,Ten = sizeName,SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="selecteditem"};
                //    var dataJsonInsert = JsonConvert.SerializeObject(dataInsert);
                //    var dataJsonSelecteditem = JsonConvert.SerializeObject(dataSelectedItem);
                //    var dJson1 = HttpPost($"{url}",dataJsonInsert);
                //    var dJson2 = HttpPost($"{url}", dataJsonSelecteditem);
                //    item.MaSize = sizeId;
                //    //}
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetChieuXa(string id, string chieuXaId, string chieuXaName)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "DESKTOP")
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETCHIEUXA", $"{chieuXaId}");
                else if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETCHIEUXA", $"{chieuXaId}", $"{chieuXaName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetChatLuong(string id, string chatLuongId, string chatLuongName)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "DESKTOP")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETCHATLUONG", $"{chatLuongId}");
                else if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "")
                        .SendAsync("SETCHATLUONG", $"{chatLuongId}", $"{chatLuongName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetNhanVien(string id, string nhanVienId, string maHoSo, string nhanVienName)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETNHANVIEN", $"{nhanVienId}",
                        $"{maHoSo}", $"{nhanVienName}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }
    public async Task<bool> CommandSetNhanVienPhucVu(string id,string nhanVienId, string maHoSo)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETNHANVIENPV",$"{nhanVienId}",
                        $"{maHoSo}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }
    /// <summary>
    /// Màu xanh lá
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> CommandSUCCESS(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SUCCESS", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }
    /// <summary>
    /// Màu Cam
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> CommandSUCCESS2(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SUCCESS2", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }
    /// <summary>
    /// Mau Xanh Dương
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> CommandSUCCESS3(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SUCCESS3", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }
    public async Task<bool> CommandSUCCESS4(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.MType == "BOARD")
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SUCCESS4", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }
    public async Task<bool> CommandGetNhanVien(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETNHANVIEN", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetWaiting(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETWAITINGSTABLE", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSetUpdateTime(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "")
                    .SendAsync("SETTIME", DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandBtnpTare(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "")
                    .SendAsync("btnptare", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandBtnpZero(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "")
                    .SendAsync("btnpzero", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandBtnpMode(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "")
                    .SendAsync("btnpmode", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandBtnpEnter(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "")
                    .SendAsync("btnpenter", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandBtnpc(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "")
                    .SendAsync("btnpc", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandBtnpRestart(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "")
                    .SendAsync("btnprestart", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandBtnpRelease(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "")
                    .SendAsync("btnr", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> Commandrestartsystem(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "")
                    .SendAsync("restartsystem", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandGetDeviceInfos(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "")
                    .SendAsync("GETINFOS", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandPhieuCanSync(string id, DateTime dateTime)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "")
                    .SendAsync("phieucansync", dateTime.ToString("yyyyMMdd"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandGetPhieuCanCountStatus0(string id, DateTime dateTime)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                await hubContext.Clients.Client(item.ConnectionId ?? "")
                    .SendAsync("GETPHIEUCANCOUNTSTATUS0", dateTime.ToString("yyyyMMdd"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //return false;
                throw e;
            }

            rl = true;
        }

        return rl;
    }

    public async Task<bool> CommandSendMessageByConnectionId(string id, string MessStr)
    {
        var rl = false;

        try
        {
            await hubContext.Clients.Client(id ?? "")
                .SendAsync("Message", MessStr);
            rl = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
            //throw e;
        }


        return rl;
    }

    public void Load()
    {
        try
        {
            var context = vmMain._dbContext;
            var constr = context.Database.GetConnectionString();
            var items = context.MayCans.AsNoTracking().OrderBy(x => x.Idx).ToList();
            var WKvs = items.Select(x => x.WKv).Distinct().ToList();
            var itemsTPDinhHinh = new List<PhieuCanTPDinhHinh>();
            var itemsBTPDinhHinh = new List<PhieuCanBTPDinhHinh>();
            var itemsTPFillet = new List<PhieuCanTPFillet>();
            var itemsBTPFilletv2 = new List<PhieuCanBTPFilletv2>();
            var itemsTPFilletv2 = new List<PhieuCanTPFilletv2>();
            var itemsXepKhuon = new List<PhieuCanChinhXepKhuon>();
            var itemsNguyenLieu = new List<PhieuCanNguyenLieu>();
            var itemsXepKhuonPhu = new List<PhieuCanPhuXepKhuon>();
            var itemsXepKhuonRaCoi = new List<PhieuCanRaCoi>();
            var itemsXepKhuonKXL = new List<PhieuCanXepKhuonKHC>();
            var itemsXepKhuonBlock = new List<PhieuCanXepKhuonBlock>();
            var itemsPhuPham = new List<PhieuCanPhuPham>();
            var itemsPhuPhamv2 = new List<PhieuCanPhuPhamv2>();
            var itemsHq = new List<HQ_PhieuCan>();
            foreach (var appKv in WKvs)
                switch (appKv)
                {
                    case AppKV.Main:
                        break;
                    case AppKV.DauAo:
                        break;
                    case AppKV.NguyenLieu:
                    {
                        var _items = vmMain.VmPhieuCanNguyenLieu.GetsLast<PhieuCanNguyenLieu>(VmApp.DateTimeNow,
                            VmApp.MaxRowsView);
                        if (_items.Count > 0) itemsNguyenLieu.AddRange(_items);
                        break;
                    }
                    case AppKV.BTPFillet:
                        break;
                    case AppKV.TPFillet:
                    {
                        var _items = vmMain.VmPhieuCanTPFillet.GetsLast<PhieuCanTPFillet>(VmApp.DateTimeNow,
                            VmApp.MaxRowsView);
                        if (_items.Count > 0) itemsTPFillet.AddRange(_items);
                        break;
                    }
                    case AppKV.BTPFilletv2:
                    {
                        var _items = vmMain.VmPhieuCanBtpFilletv2.GetsLast<PhieuCanBTPFilletv2>(VmApp.DateTimeNow,
                            VmApp.MaxRowsView);
                        if (_items.Count > 0) itemsBTPFilletv2.AddRange(_items);
                        break;
                    }
                    case AppKV.TPFilletv2:
                    {
                        var _items = vmMain.VmPhieuCanTpFilletv2.GetsLast<PhieuCanTPFilletv2>(VmApp.DateTimeNow,
                            VmApp.MaxRowsView);
                        if (_items.Count > 0) itemsTPFilletv2.AddRange(_items);
                        break;
                    }
                    case AppKV.PhuPham:
                    {
                        var _items = vmMain.VmPhieuCanPhuPham.GetsLast<PhieuCanPhuPham>(VmApp.DateTimeNow,
                            VmApp.MaxRowsView);
                        if (_items.Count > 0) itemsPhuPham.AddRange(_items);
                        break;
                    }
                    case AppKV.BTPDinhHinh:
                    {
                        var _items = vmMain.VmPhieuCanBTPDinhHinh.GetsLast<PhieuCanBTPDinhHinh>(VmApp.DateTimeNow,
                            VmApp.MaxRowsView);
                        if (_items.Count > 0) itemsBTPDinhHinh.AddRange(_items);
                        break;
                    }
                    case AppKV.TPDinhHinh:
                    {
                        var _items = vmMain.VmPhieuCanTPDinhHinh.GetsLast<PhieuCanTPDinhHinh>(VmApp.DateTimeNow,
                            VmApp.MaxRowsView);
                        if (_items.Count > 0) itemsTPDinhHinh.AddRange(_items);
                        break;
                    }
                    case AppKV.XepKhuon:
                    {
                        var _items = vmMain.VmPhieuCanChinhXepKhuon.GetsLast<PhieuCanChinhXepKhuon>(VmApp.DateTimeNow,
                            VmApp.MaxRowsView);
                        if (_items.Count > 0) itemsXepKhuon.AddRange(_items);
                        break;
                    }
                    case AppKV.BaoTu:
                        break;
                    case AppKV.CaoThit:
                        break;
                    case AppKV.XepKhuonRaCoi:
                    {
                        var _items = vmMain.VmPhieuCanRaCoi.GetsLast<PhieuCanRaCoi>(VmApp.DateTimeNow,
                            VmApp.MaxRowsView);
                        if (_items.Count > 0) itemsXepKhuonRaCoi.AddRange(_items);

                        break;
                    }
                    case AppKV.XepKhuonPhu:
                    {
                        var _items = vmMain.VmPhieuCanPhuXepKhuon.GetsLast<PhieuCanPhuXepKhuon>(VmApp.DateTimeNow,
                            VmApp.MaxRowsView);
                        if (_items.Count > 0) itemsXepKhuonPhu.AddRange(_items);
                        break;
                    }
                    case AppKV.XepKhuonKXL:
                    {
                        var _items = vmMain.VmPhieuCanXepKhuonKXL.GetsLast<PhieuCanXepKhuonKHC>(VmApp.DateTimeNow,
                            VmApp.MaxRowsView);
                        if (_items.Count > 0) itemsXepKhuonKXL.AddRange(_items);
                        break;
                    }

                    case AppKV.PhuPhamv2:
                    {
                        var _items = vmMain.VmPhieuCanPhuPhamv2.GetsLast<PhieuCanPhuPhamv2>(VmApp.DateTimeNow,
                            VmApp.MaxRowsView);
                        if (_items.Count > 0) itemsPhuPhamv2.AddRange(_items);
                        break;
                    }
                    case AppKV.Hq:
                    {
                        var _items = vmMain.VmPhieuCanHq.GetsLast<HQ_PhieuCan>(VmApp.DateTimeNow,
                            VmApp.MaxRowsView);
                        if (_items.Count > 0) itemsHq.AddRange(_items);
                        break;
                    }
                    case AppKV.XepKhuonBlock:
                    {
                        var _items = vmMain.VmPhieuCanXepKhuonBlock.GetsLast<PhieuCanXepKhuonBlock>(VmApp.DateTimeNow,
                            VmApp.MaxRowsView);
                        if (_items.Count > 0) itemsXepKhuonBlock.AddRange(_items);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            foreach (var mayCan in items)
                switch (mayCan.WKv)
                {
                    case AppKV.Main:
                        break;
                    case AppKV.DauAo:
                        break;
                    case AppKV.NguyenLieu:
                    {
                        var _items = itemsNguyenLieu.Where(x => x.MaMayTinhCan == mayCan.Id).ToList();
                        if (_items.Count > 0)
                        {
                            mayCan.Items.AddRange(_items);
                            mayCan.TongSoRo = itemsNguyenLieu.Count;
                            mayCan.TongTrongLuong = itemsNguyenLieu.Sum(x => x.TrongLuong ?? 0);
                        }

                        break;
                    }
                    case AppKV.BTPFillet:
                        break;
                    case AppKV.TPFillet:
                    {
                        var _items = itemsTPFillet.Where(x => x.MaMayTinhCan == mayCan.Id).ToList();
                        if (_items.Count > 0)
                        {
                            mayCan.Items.AddRange(_items);
                            mayCan.TongSoRo = itemsTPFillet.Count;
                            mayCan.TongTrongLuong = itemsTPFillet.Sum(x => x.TrongLuong ?? 0);
                        }

                        break;
                    }
                    case AppKV.BTPFilletv2:
                    {
                        var _items = itemsBTPFilletv2.Where(x => x.MaMayCan == mayCan.Id).ToList();
                        if (_items.Count > 0)
                        {
                            mayCan.Items.AddRange(_items);
                            mayCan.TongSoRo = itemsBTPFilletv2.Count;
                            mayCan.TongTrongLuong = itemsBTPFilletv2.Sum(x => x.TrongLuong);
                        }

                        break;
                    }
                    case AppKV.TPFilletv2:
                    {
                        var _items = itemsTPFilletv2.Where(x => x.MaMayCan == mayCan.Id).ToList();
                        if (_items.Count > 0)
                        {
                            mayCan.Items.AddRange(_items);
                            mayCan.TongSoRo = itemsTPFilletv2.Count;
                            mayCan.TongTrongLuong = itemsTPFilletv2.Sum(x => x.TrongLuongTra);
                        }

                        break;
                    }
                    case AppKV.PhuPham:
                    {
                        var _items = itemsPhuPham.Where(x => x.MaMayTinhCan == mayCan.Id).ToList();
                        if (_items.Count > 0)
                        {
                            mayCan.Items.AddRange(_items);
                            mayCan.TongSoRo = itemsPhuPham.Count;
                            mayCan.TongTrongLuong = itemsPhuPham.Sum(x => x.TrongLuong ?? 0);
                        }

                        break;
                    }
                    case AppKV.BTPDinhHinh:
                    {
                        var _items = itemsBTPDinhHinh.Where(x => x.MaMayCan == mayCan.Id).ToList();
                        if (_items.Count > 0)
                        {
                            mayCan.Items.AddRange(_items);
                            mayCan.TongSoRo = itemsBTPDinhHinh.Count;
                            mayCan.TongTrongLuong = itemsBTPDinhHinh.Sum(x => x.TrongLuong);
                        }

                        break;
                    }
                    case AppKV.TPDinhHinh:
                    {
                        var _items = itemsTPDinhHinh.Where(x => x.MaMayCan == mayCan.Id).ToList();
                        if (_items.Count > 0)
                        {
                            mayCan.Items.AddRange(_items);
                            mayCan.TongSoRo = itemsTPDinhHinh.Count;
                            mayCan.TongTrongLuong = itemsTPDinhHinh.Sum(x => x.TrongLuongTra);
                        }

                        break;
                    }
                    case AppKV.XepKhuon:
                    {
                        var _items = itemsXepKhuon.Where(x => x.MaMayCan == mayCan.Id).ToList();
                        if (_items.Count > 0)
                        {
                            mayCan.Items.AddRange(_items);
                            mayCan.TongSoRo = itemsXepKhuon.Count;
                            mayCan.TongTrongLuong = itemsXepKhuon.Sum(x => x.TrongLuong);
                        }

                        break;
                    }

                    case AppKV.BaoTu:
                        break;
                    case AppKV.CaoThit:
                        break;
                    case AppKV.XepKhuonRaCoi:
                    {
                        var _items = itemsXepKhuonRaCoi.Where(x => x.MayCan == mayCan.Id).ToList();
                        if (_items.Count > 0)
                        {
                            mayCan.Items.AddRange(_items);
                            mayCan.TongSoRo = itemsXepKhuonRaCoi.Count;
                            mayCan.TongTrongLuong = itemsXepKhuonRaCoi.Sum(x => x.TrongLuong);
                        }

                        break;
                    }
                    case AppKV.XepKhuonPhu:
                    {
                        var _items = itemsXepKhuonPhu.Where(x => x.MaMayCan == mayCan.Id).ToList();
                        if (_items.Count > 0)
                        {
                            mayCan.Items.AddRange(_items);
                            mayCan.TongSoRo = itemsXepKhuonPhu.Count;
                            mayCan.TongTrongLuong = itemsXepKhuonPhu.Sum(x => x.TrongLuong);
                        }

                        break;
                    }
                    case AppKV.XepKhuonKXL:
                    {
                        var _items = itemsXepKhuonKXL.Where(x => x.MaMayCan == mayCan.Id).ToList();
                        if (_items.Count > 0)
                        {
                            mayCan.Items.AddRange(_items);
                            mayCan.TongSoRo = itemsXepKhuonKXL.Count;
                            mayCan.TongTrongLuong = itemsXepKhuonKXL.Sum(x => x.TrongLuong);
                        }

                        break;
                    }
                    case AppKV.XepKhuonBlock:
                    {
                        var _items = itemsXepKhuonBlock.Where(x => x.MaMayCan == mayCan.Id).ToList();
                        if (_items.Count > 0)
                        {
                            mayCan.Items.AddRange(_items);
                            mayCan.TongSoRo = itemsXepKhuonBlock.Count;
                            mayCan.TongTrongLuong = itemsXepKhuonBlock.Sum(x => x.TrongLuong);
                        }

                        break;
                    }
                    case AppKV.PhuPhamv2:
                    {
                        var _items = itemsPhuPhamv2.Where(x => x.MaMayCan == mayCan.Id).ToList();
                        if (_items.Count > 0)
                        {
                            mayCan.Items.AddRange(_items);
                            mayCan.TongSoRo = itemsPhuPhamv2.Count;
                            mayCan.TongTrongLuong = itemsPhuPhamv2.Sum(x => x.TrongLuong);
                        }

                        break;
                    }
                    case AppKV.Hq:
                    {
                        var _items = itemsHq.Where(x => x.MayCan == mayCan.Id).ToList();
                        if (_items.Count > 0)
                        {
                            mayCan.Items.AddRange(_items);
                            mayCan.TongSoRo = itemsHq.Count;
                            mayCan.TongTrongLuong = itemsHq.Sum(x => x.TrongLuong);
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            lock (_lock)
            {
                //Items
                // Create a new collection to hold the items to be added
                var newItems = new List<MayCan>();
                newItems.AddRange(items); // Use AddRange to add multiple items to the new collection
                var itemRemoves = new List<MayCan>();
                //Items.Clear();
                // for (var i = 0; i < Items.Count; i++)
                // {
                //     var itemNotRemove = newItems.FirstOrDefault(x => x.Id == Items[i].Id);
                //     if (itemNotRemove == null)
                //         itemRemoves.Add(Items[i]);
                // }
                var newItemIds = new HashSet<string>(newItems.Select(x => x.Id));
                foreach (var kvp in Items)
                    if (!newItemIds.Contains(kvp.Key))
                        itemRemoves.Add(kvp.Value);
                foreach (var key in itemRemoves) Remove(key.Id);
                foreach (var newItem in newItems)
                {
                    if (newItem.AType == AppType._type5) newItem.IsConnected = true;
                    var _newItem = Find(newItem.Id); //Items.FirstOrDefault(x => x.Id == newItem.Id);
                    if (_newItem != null)
                        Update2(newItem);
                    else
                        AddorUpdate(newItem);
                }

                foreach (var item in itemRemoves) Remove(item.Id);
                //Items.RemoveRange(itemRemoves);
                //Items.Clear();
                // Replace the existing collection with the new collection
                //Items.ReplaceRange(newItems);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void Update(string id,AppKV wKv, AppType aType, string xuongId)
    {
        try
        {
            var context = vmMain._dbContext;
            var constr = context.Database.GetConnectionString();
            // var items = context.MayCans.AsNoTracking().OrderBy(x => x.Idx).ToList();
            var item = context.MayCans.FirstOrDefault(x => x.Id == id );
            if (item != null)
            {
                item.WKv = wKv;
                item.AType = aType;
                item.MaXuong = xuongId;
                context.MayCans.Update(item);
                context.SaveChanges();
            }
        }
        
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public async Task<bool> CommandSendMess(string id, string mess)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                var data = new DataLiteRes
                {
                    IsDataAction = 0,
                    MessStr = mess
                };
                var dataJson = JsonConvert.SerializeObject(data);
                var dataTranfer = new DataTranfer
                {
                    Data = dataJson,
                    IsError = false
                };
                var dataToSend = JsonConvert.SerializeObject(dataTranfer);
                await hubContext.Clients.Client(item.ConnectionId ?? "")
                    .SendAsync("ReceiveData", "LITE", dataToSend);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            rl = true;
        }

        return rl;
    }

    #endregion

    public void CommandSetThanhPham(string id, string thanhPhamId)
    {
    }

    public List<T> GetItems<T>(AppKV akv)
    {
        var result = new List<T>();

        foreach (var mayCan in Items.Values)
        {
            if (mayCan.WKv != akv) continue;

            foreach (var item in mayCan.Items)
                if (item is T typedItem)
                    result.Add(typedItem);
        }

        return result;
    }

    private string HttpGet(string url)
    {
        using (var client = new HttpClient())
        {
            using (var response = client.GetAsync(url).Result)
            {
                using (var content = response.Content)
                {
                    return content.ReadAsStringAsync().Result;
                }
            }
        }
    }

    private string HttpPost(string url, string data)
    {
        using (var client = new HttpClient())
        {
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            using (var response = client.PostAsync(url, content).Result)
            {
                using (var content2 = response.Content)
                {
                    return content2.ReadAsStringAsync().Result;
                }
            }
        }
    }

    // protected virtual void OnItemChanged()
    // {
    //     ItemChanged?.Invoke(this, EventArgs.Empty);
    // }

    private async void OnTimedEvent(object? sender, ElapsedEventArgs e)
    {
        _timer.Stop();
        foreach (var item in Items)
            if (item.Value.AType != AppType._type5)
            {
            }
            else
            {
                DisConnectionId(item.Key);
            }
        // for (var i = 0; i < Items.Count; i++)
        // {
        //     var mayCan = Items[i];
        //     if (mayCan.AType != AppType._type5)
        //     {
        //         //mayCan.IsConnected = !mayCan.IsConnected;
        //     }
        //     else
        //         mayCan.IsConnected = true;
        // }

        _timer.Start();
        // OnItemChanged();
    }

    private void StartTimer()
    {
        if (_timer != null)
        {
            _timer.Elapsed -= OnTimedEvent;
            _timer.Stop();
            _timer.Dispose();
        }

        _timer = new Timer(5000);
        _timer.Elapsed += OnTimedEvent;
        _timer.AutoReset = true;
        _timer.Start();
    }

    public void Update2(MayCan mayCan)
    {
        // for (var i = 0; i < Items.Count; i++)
        // {
        //     var item = Items[i];
        //     if (item.Id == mayCan.Id)
        //     {
        //         item.MType = mayCan.MType;
        //         item.WKv = mayCan.WKv;
        //         item.AType = mayCan.AType;
        //         item.Items = mayCan.Items;
        //         item.DisplayName = mayCan.DisplayName;
        //         item.SCode = mayCan.SCode;
        //         item.Par1 = mayCan.Par1;
        //         item.Par2 = mayCan.Par2;
        //         item.Par3 = mayCan.Par3;
        //         item.Par4 = mayCan.Par4;
        //         item.Idx = mayCan.Idx;
        //         item.MaXuong = mayCan.MaXuong;
        //         item.IsSuDungMauThanhPham = mayCan.IsSuDungMauThanhPham;
        //     }
        // }
        AddorUpdate(mayCan);

        // OnItemChanged();
    }
}
// public partial class MayCansService : ObservableObject, IMayCansService
// {
//     private readonly object _lock = new();
//     private readonly IHubContext<ChatHub> hubContext;
//     private readonly IMainService vmMain;
//     private Timer? _timer;
//     private CancellationTokenSource _debounceCancellationTokenSource;
//     private readonly int _debounceDelay = 10;
//     [ObservableProperty] private ObservableRangeCollection<MayCan> items = new();
//
//     [ObservableProperty] private List<MayCan> selectedItems = new();
//
//     public MayCansService(IMainService vmMain, IHubContext<ChatHub> hubContext)
//     {
//         this.vmMain = vmMain;
//         this.hubContext = hubContext;
//         try
//         {
//             //StartTimer();
//             Load();
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             Console.WriteLine(e);
//             //throw;
//             VmMessage.SetExceptionCommand.Execute(e);
//         }
//     }
//
//     private AppViewModel VmApp => AppViewModel.Instance;
//     private MessageViewModel VmMessage => MessageViewModel.Instance;
//
//     public MayCan? Find(string id)
//     {
//         try
//         {
//             return Items.FirstOrDefault(x => x.Id == id);
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             throw;
//         }
//     }
//
//     public MayCan? Find(string id, bool isConnected)
//     {
//         var mayCan = Find(id);
//         if (mayCan == null) return null;
//         return mayCan.IsConnected ? mayCan : null;
//     }
//
//     public void AddItem(MayCan mayCan, object item)
//     {
//         if (mayCan.Items.Count > 100)
//         {
//             mayCan.Items.Remove(mayCan.Items.Last());
//         }
//
//         mayCan.Items.Insert(0, item);
//     }
//
//     public void Remove(string id)
//     {
//         try
//         {
//             var item = Items.FirstOrDefault(x => x.Id == id);
//             if (item != null) Items.Replace(item);
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             throw;
//         }
//     }
//
//     public void Dis(string id)
//     {
//         try
//         {
//             var item = Items.FirstOrDefault(x => x.Id == id);
//             if (item != null)
//             {
//                 var index = Items.IndexOf(item);
//                 if (index >= 0)
//                 {
//                     item.IsActive = false;
//                     //if (item.AType != AppType._type5)
//                     item.IsConnected = false;
//                     item.ConnectionId = "";
//                     Items.RemoveAt(index);
//                     Items.Insert(index, item);
//                 }
//             }
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             throw;
//         }
//     }
//
//     public void DisConnectionId(string id)
//     {
//         try
//         {
//             var item = Items.FirstOrDefault(x => x.ConnectionId == id);
//             if (item != null)
//             {
//                 var index = Items.IndexOf(item);
//                 if (index >= 0)
//                 {
//                     item.IsActive = false;
//                     //if (item.AType != AppType._type5)
//                     item.IsConnected = false;
//                     item.ConnectionId = "";
//                     Update(item);
//                 }
//             }
//             //OnItemChanged();
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             throw;
//         }
//     }
//
//     public async Task ExpandHandler(MayCan mayCan)
//     {
//         mayCan.IsExpand = !mayCan.IsExpand;
//     }
//
//     public void SetAllExpand(bool isExpanded)
//     {
//         foreach (var mayCan in Items) mayCan.IsExpand = isExpanded;
//     }
//
//     public void SetIsConnected(bool isConnected, string id)
//     {
//         for (var i = 0; i < Items.Count; i++)
//         {
//             var item = Items[i];
//             //if (item.Id == id && item.AType != AppType._type5) item.IsConnected = isConnected;
//             if (item.Id == id) item.IsConnected = isConnected;
//         }
//
//         OnItemChanged();
//     }
//
//     public void Update(MayCan mayCan)
//     {
//         for (var i = 0; i < Items.Count; i++)
//         {
//             var item = Items[i];
//             if (item.Id == mayCan.Id)
//             {
//                 item.MType = mayCan.MType;
//                 item.ConnectionId = mayCan.ConnectionId;
//                 item.DateTimeConnected = mayCan.DateTimeConnected;
//                 item.IPAddr = mayCan.IPAddr;
//                 item.IsActive = mayCan.IsActive;
//                 //if (item.AType != AppType._type5)
//                 item.IsConnected = mayCan.IsConnected;
//                 //else
//                 //    item.IsConnected = true;
//                 item.WKv = mayCan.WKv;
//                 item.AType = mayCan.AType;
//             }
//         }
//
//         OnItemChanged();
//     }
//
//     public event EventHandler? ItemChanged;
//
//     public void NotifyItemChanged()
//     {
//         _debounceCancellationTokenSource?.Cancel();
//         _debounceCancellationTokenSource?.Dispose();
//         _debounceCancellationTokenSource = new CancellationTokenSource();
//         var token = _debounceCancellationTokenSource.Token;
//         Task.Delay(_debounceDelay, token).ContinueWith(t =>
//         {
//             if (!token.IsCancellationRequested)
//             {
//                 OnItemChanged();
//             }
//         }, token);
//         //OnItemChanged();
//     }
//
//     public async Task<bool> CommandZero(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 item.TrongLuongTare = 0;
//
//                 //if (item.AType != AppType._type5)
//                 //{
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("ZERO", "0");
//                 //}
//                 //else
//                 //{
//                 //    var url = $"http://{item.IPAddr}/api/w/posts";
//                 //    var data = new { CMD = "ZERO",Path="zero"};
//                 //    var dataJson = JsonConvert.SerializeObject(data);
//                 //    var dJson1 = HttpPost($"{url}",dataJson);
//                 //}
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandSetNapThe(string id, bool isNapThe)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 item.IsNapThe = true;
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandTare(string id, decimal? trongLuongTare)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 //if (item.AType != AppType._type5)
//                 //{
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("TARE", $"{trongLuongTare ?? 0}");
//                 //}
//                 //else
//                 //{
//                 //    var url = $"http://{item.IPAddr}/api/w/posts";
//                 //    var _trongLuongTare = trongLuongTare ?? 0;
//                 //    var data = new { trongluongtare = _trongLuongTare,Path="tare"};
//                 //    var dataJson = JsonConvert.SerializeObject(data);
//                 //    var dJson1 = HttpPost($"{url}",dataJson);
//                 //}
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandGetLo(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 //if (item.AType != AppType._type5)
//                 //{
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETLO", "");
//                 //}
//                 //else
//                 //{
//                 //    var url = $"http://{item.IPAddr}/api/lo/gets?Path=getselecteditem";
//                 //    var dataJson = HttpGet(url);
//                 //    var json = JObject.Parse(dataJson);
//                 //    var datacount = 0;
//                 //    int.TryParse(json["datacount"]?.ToString(), out datacount);
//                 //    if (datacount > 0)
//                 //    {
//
//                 //        var maLo = (json.SelectToken("data[0].Id"))?.ToString();
//                 //        item.MaLo = maLo ?? string.Empty;
//                 //    }
//                 //}
//             }
//             catch (Exception e)
//             {
//                 vmMain.VmMessage.MessageBoxShow(e.ToString(), "", 0);
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//
//     public async Task<bool> CommandSetLo(string id, string maLo)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 //if (item.AType != AppType._type5)
//                 //{
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETLO", $"{maLo}");
//                 //    }
//                 //    else
//                 //    {
//                 //        var dataInsert = new { Id = maLo,NgayNguyenLieu = DateTime.Now.ToString("yyyyMMddhhmmss"),SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="insert"};
//                 //        var dataSelecteditem = new { Id = maLo,NgayNguyenLieu = DateTime.Now.ToString("yyyyMMddhhmmss"),SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="selecteditem"};
//                 //        if (item.WKv == AppKV.Hq)
//                 //        {
//                 //            var lo = vmMain.VmLo.GetHqLo(maLo);
//                 //            if (lo != null)
//                 //            {
//                 //                dataInsert = new
//                 //                {
//                 //                    Id = maLo, NgayNguyenLieu = $"{lo.NgayNguyenLieu.ToString("yyyyMMdd")}000000",
//                 //                    SuDung = lo.SuDung, MNgay = lo.MNgay.ToString("yyyyMMdd"), Path = "insert"
//                 //                };
//                 //                dataSelecteditem = new
//                 //                {
//                 //                    Id = maLo, NgayNguyenLieu = $"{lo.NgayNguyenLieu.ToString("yyyyMMdd")}000000",
//                 //                    SuDung = lo.SuDung, MNgay = lo.MNgay.ToString("yyyyMMdd"), Path = "selecteditem"
//                 //                };
//                 //            }
//                 //            else
//                 //            {
//                 //               var rlIn =  vmMain.VmLoHq.Insert(new HQ_Lo()
//                 //                {
//                 //                    Id = maLo, NgayNguyenLieu = DateOnly.FromDateTime(DateTime.Now), MNgay = DateTime.Now,
//                 //                    SuDung = true
//                 //                });
//                 //                if (rlIn >0)
//                 //                {
//                 //                    vmMain.VmLoHq.Items.Insert(0,maLo);
//                 //                    vmMain.VmLo.Items.Insert(0,maLo);
//                 //                }
//                 //            }
//                 //        }
//
//                 //        var url = $"http://{item.IPAddr}/api/lo/posts";
//
//                 //        var dataJsonInsert = JsonConvert.SerializeObject(dataInsert);
//                 //        var dataJsonSelecteditem = JsonConvert.SerializeObject(dataSelecteditem);
//                 //        var dJson1 = HttpPost($"{url}",dataJsonInsert);
//                 //        var dJson2 = HttpPost($"{url}", dataJsonSelecteditem);
//
//                 //        //var json = JObject.Parse(dataJson);
//                 //        //var datacount = 0;
//                 //        //int.TryParse(json["datacount"]?.ToString(), out datacount);
//                 //        //if (datacount > 0)
//                 //        //{
//
//                 //        //    //var maLo = (json.SelectToken("data[0].Id"))?.ToString();
//                 //        item.MaLo = maLo;
//                 //        //}
//                 //    }
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public Task<bool> CommandSetPhuongTien(string id, string phuongTienId, string phuongTienName)
//     {
//         throw new NotImplementedException();
//     }
//
//     public async Task<bool> CommandSetInfoCoi(string id, string dataCoiJson)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETINFOCOI", dataCoiJson);
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 return false;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandGetThanhPham(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 //if (item.AType != AppType._type5)
//                 //{
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETTHANHPHAM", "");
//                 //}
//                 //else
//                 //{
//                 //    var url = $"http://{item.IPAddr}/api/thanhpham/gets?Path=getselecteditem";
//                 //    var dataJson = HttpGet(url);
//                 //    var json = JObject.Parse(dataJson);
//                 //    var datacount = 0;
//                 //    int.TryParse(json["datacount"]?.ToString(), out datacount);
//                 //    if (datacount > 0)
//                 //    {
//
//                 //        var maThanhPham = (json.SelectToken("data[0].Id"))?.ToString();
//                 //        item.MaThanhPham = maThanhPham ?? string.Empty;
//                 //    }
//                 //}
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandGetChieuXa(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETCHIEUXA", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandGetCoi(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETCOI", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandSetCoi(string id, string coiId, string coiName)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 if (item.MType == "DESKTOP")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETCOI", $"{coiId}");
//                 else if (item.MType == "BOARD")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "")
//                         .SendAsync("SETCOI", $"{coiId}", $"{coiName}");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandGetAo(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETAO", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandSetAo(string id, string aoId, string aoName)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 if (item.MType == "DESKTOP")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETAO", $"{aoId}");
//                 else if (item.MType == "BOARD")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "")
//                         .SendAsync("SETAO", $"{aoId}", $"{aoName}");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandGetPhuongTien(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETPHUONGTIEN", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandGetChatLuong(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETCHATLUONG", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandSetThanhPham(string id, string thanhPhamId, string thanhPhamName)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 //if (item.AType != AppType._type5)
//                 //{
//                 if (item.MType == "DESKTOP")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "")
//                         .SendAsync("SETTHANHPHAM", $"{thanhPhamId}");
//                 else if (item.MType == "BOARD")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "")
//                         .SendAsync("SETTHANHPHAM", $"{thanhPhamId}", $"{thanhPhamName}");
//                 //}
//                 //else
//                 //{
//
//                 //    var url = $"http://{item.IPAddr}/api/thanhpham/posts";
//                 //    var dataIsnert = new { Id = thanhPhamId,Ten = thanhPhamName,SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="insert"};
//                 //    var dataSelecteditem = new { Id = thanhPhamId,Ten = thanhPhamName,SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="selecteditem"};
//                 //    var dataJsonInsert = JsonConvert.SerializeObject(dataIsnert);
//                 //    var dataJsonSelecteditem = JsonConvert.SerializeObject(dataSelecteditem);
//                 //    var dJson1 = HttpPost($"{url}",dataJsonInsert);
//                 //    var dJson2 = HttpPost($"{url}", dataJsonSelecteditem);
//                 //    item.MaThanhPham = thanhPhamId;
//                 //    //}
//                 //}
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public void CommandSetThanhPham(string id, string thanhPhamId)
//     {
//     }
//
//     public async Task<bool> CommandGetNguyenLieu(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 //if (item.AType != AppType._type5)
//                 //{
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETLOAINGUYENLIEU", "");
//                 //}
//                 //else
//                 //{
//                 //    var url = $"http://{item.IPAddr}/api/loainguyenlieu/gets?Path=getselecteditem";
//                 //    var dataJson = HttpGet(url);
//                 //    var json = JObject.Parse(dataJson);
//                 //    var datacount = 0;
//                 //    int.TryParse(json["datacount"]?.ToString(), out datacount);
//                 //    if (datacount > 0)
//                 //    {
//
//                 //        var maNguyenLieu = (json.SelectToken("data[0].Id"))?.ToString();
//                 //        item.MaLoaiNguyenLieu = maNguyenLieu ?? string.Empty;
//                 //    }
//                 //}
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandSetNguyenLieu(string id, string nguyenLieuId, string nguyenLieuName)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 //if (item.AType != AppType._type5)
//                 //{
//                 if (item.MType == "DESKTOP")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "")
//                         .SendAsync("SETLOAINGUYENLIEU", $"{nguyenLieuId}");
//                 else if (item.MType == "BOARD")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "")
//                         .SendAsync("SETLOAINGUYENLIEU", $"{nguyenLieuId}", $"{nguyenLieuName}");
//                 //}
//                 //else
//                 //{
//
//                 //    var url = $"http://{item.IPAddr}/api/loainguyenlieu/posts";
//                 //    var dataInsert = new { Id = nguyenLieuId,Ten = nguyenLieuName,SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="insert"};
//                 //    var dataSelectedItem = new { Id = nguyenLieuId,Ten = nguyenLieuName,SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="selecteditem"};
//                 //    var dataJsonInsert = JsonConvert.SerializeObject(dataInsert);
//                 //    var dataJsonSelectedItem = JsonConvert.SerializeObject(dataSelectedItem);
//                 //    var dJson1 = HttpPost($"{url}",dataJsonInsert);
//                 //    var dJson2 = HttpPost($"{url}", dataJsonSelectedItem);
//                 //    item.MaLoaiNguyenLieu = nguyenLieuId;
//                 //    //}
//                 //}
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 throw e;
//                 //return false;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandGetSize(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 //if (item.AType != AppType._type5)
//                 //{
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETSIZE", "");
//                 //}
//                 //else
//                 //{
//                 //    var url = $"http://{item.IPAddr}/api/size/gets?Path=getselecteditem";
//                 //    var dataJson = HttpGet(url);
//                 //    var json = JObject.Parse(dataJson);
//                 //    var datacount = 0;
//                 //    int.TryParse(json["datacount"]?.ToString(), out datacount);
//                 //    if (datacount > 0)
//                 //    {
//
//                 //        var masize = (json.SelectToken("data[0].Id"))?.ToString();
//                 //        item.MaSize = masize ?? string.Empty;
//                 //    }
//                 //}
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandSetSize(string id, string sizeId, string sizeName)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 //if (item.AType != AppType._type5)
//                 //{
//                 if (item.MType == "DESKTOP")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETSIZE", $"{sizeId}");
//                 else if (item.MType == "BOARD")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "")
//                         .SendAsync("SETSIZE", $"{sizeId}", $"{sizeName}");
//                 //}
//                 //else
//                 //{
//
//                 //    var url = $"http://{item.IPAddr}/api/size/posts";
//                 //    var dataInsert = new { Id = sizeId,Ten = sizeName,SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="insert"};
//                 //    var dataSelectedItem = new { Id = sizeId,Ten = sizeName,SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd"),Path="selecteditem"};
//                 //    var dataJsonInsert = JsonConvert.SerializeObject(dataInsert);
//                 //    var dataJsonSelecteditem = JsonConvert.SerializeObject(dataSelectedItem);
//                 //    var dJson1 = HttpPost($"{url}",dataJsonInsert);
//                 //    var dJson2 = HttpPost($"{url}", dataJsonSelecteditem);
//                 //    item.MaSize = sizeId;
//                 //    //}
//                 //}
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandSetChieuXa(string id, string chieuXaId, string chieuXaName)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 if (item.MType == "DESKTOP")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETCHIEUXA", $"{chieuXaId}");
//                 else if (item.MType == "BOARD")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "")
//                         .SendAsync("SETCHIEUXA", $"{chieuXaId}", $"{chieuXaName}");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandSetChatLuong(string id, string chatLuongId, string chatLuongName)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 if (item.MType == "DESKTOP")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "")
//                         .SendAsync("SETCHATLUONG", $"{chatLuongId}");
//                 else if (item.MType == "BOARD")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "")
//                         .SendAsync("SETCHATLUONG", $"{chatLuongId}", $"{chatLuongName}");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandSetNhanVien(string id, string nhanVienId, string maHoSo, string nhanVienName)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 if (item.MType == "BOARD")
//                     await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETNHANVIEN", $"{nhanVienId}",
//                         $"{maHoSo}", $"{nhanVienName}");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandGetNhanVien(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETNHANVIEN", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandSetWaiting(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETWAITINGSTABLE", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandSetUpdateTime(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "")
//                     .SendAsync("SETTIME", DateTime.Now.ToString("yyyyMMddHHmmss"));
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandBtnpTare(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "")
//                     .SendAsync("btnptare", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandBtnpZero(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "")
//                     .SendAsync("btnpzero", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandBtnpMode(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "")
//                     .SendAsync("btnpmode", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandBtnpEnter(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "")
//                     .SendAsync("btnpenter", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandBtnpc(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "")
//                     .SendAsync("btnpc", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandBtnpRestart(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "")
//                     .SendAsync("btnprestart", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandBtnpRelease(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "")
//                     .SendAsync("btnr", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> Commandrestartsystem(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "")
//                     .SendAsync("restartsystem", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandGetDeviceInfos(string id)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "")
//                     .SendAsync("GETINFOS", "");
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//         return rl;
//     }
//
//     public async Task<bool> CommandPhieuCanSync(string id, DateTime dateTime)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "")
//                     .SendAsync("phieucansync", dateTime.ToString("yyyyMMdd"));
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandGetPhieuCanCountStatus0(string id, DateTime dateTime)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 await hubContext.Clients.Client(item.ConnectionId ?? "")
//                     .SendAsync("GETPHIEUCANCOUNTSTATUS0", dateTime.ToString("yyyyMMdd"));
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 //return false;
//                 throw e;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     public async Task<bool> CommandSendMessageByConnectionId(string id, string MessStr)
//     {
//         var rl = false;
//
//         try
//         {
//             await hubContext.Clients.Client(id ?? "")
//                 .SendAsync("Message", MessStr);
//             rl = true;
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             return false;
//             //throw e;
//         }
//
//
//         return rl;
//     }
//
//     public void Load()
//     {
//         try
//         {
//             var context = vmMain._dbContext;
//             var constr = context.Database.GetConnectionString();
//             var items = context.MayCans.AsNoTracking().OrderBy(x => x.Idx).ToList();
//             var WKvs = items.Select(x => x.WKv).Distinct().ToList();
//             var itemsTPDinhHinh = new List<PhieuCanTPDinhHinh>();
//             var itemsBTPDinhHinh = new List<PhieuCanBTPDinhHinh>();
//             var itemsTPFillet = new List<PhieuCanTPFillet>();
//             var itemsBTPFilletv2 = new List<PhieuCanBTPFilletv2>();
//             var itemsTPFilletv2 = new List<PhieuCanTPFilletv2>();
//             var itemsXepKhuon = new List<PhieuCanChinhXepKhuon>();
//             var itemsNguyenLieu = new List<PhieuCanNguyenLieu>();
//             var itemsXepKhuonPhu = new List<PhieuCanPhuXepKhuon>();
//             var itemsXepKhuonRaCoi = new List<PhieuCanRaCoi>();
//             var itemsXepKhuonKXL = new List<PhieuCanXepKhuonKHC>();
//             var itemsPhuPham = new List<PhieuCanPhuPham>();
//             var itemsPhuPhamv2 = new List<PhieuCanPhuPhamv2>();
//             var itemsHq = new List<HQ_PhieuCan>();
//             foreach (var appKv in WKvs)
//                 switch (appKv)
//                 {
//                     case AppKV.Main:
//                         break;
//                     case AppKV.DauAo:
//                         break;
//                     case AppKV.NguyenLieu:
//                     {
//                         var _items = vmMain.VmPhieuCanNguyenLieu.GetsLast<PhieuCanNguyenLieu>(VmApp.DateTimeNow,
//                             VmApp.MaxRowsView);
//                         if (_items.Count > 0) itemsNguyenLieu.AddRange(_items);
//                         break;
//                     }
//                     case AppKV.BTPFillet:
//                         break;
//                     case AppKV.TPFillet:
//                     {
//                         var _items = vmMain.VmPhieuCanTPFillet.GetsLast<PhieuCanTPFillet>(VmApp.DateTimeNow,
//                             VmApp.MaxRowsView);
//                         if (_items.Count > 0) itemsTPFillet.AddRange(_items);
//                         break;
//                     }
//                     case AppKV.BTPFilletv2:
//                     {
//                         var _items = vmMain.VmPhieuCanBtpFilletv2.GetsLast<PhieuCanBTPFilletv2>(VmApp.DateTimeNow,
//                             VmApp.MaxRowsView);
//                         if (_items.Count > 0) itemsBTPFilletv2.AddRange(_items);
//                         break;
//                     }
//                     case AppKV.TPFilletv2:
//                     {
//                         var _items = vmMain.VmPhieuCanTpFilletv2.GetsLast<PhieuCanTPFilletv2>(VmApp.DateTimeNow,
//                             VmApp.MaxRowsView);
//                         if (_items.Count > 0) itemsTPFilletv2.AddRange(_items);
//                         break;
//                     }
//                     case AppKV.PhuPham:
//                     {
//                         var _items = vmMain.VmPhieuCanPhuPham.GetsLast<PhieuCanPhuPham>(VmApp.DateTimeNow,
//                             VmApp.MaxRowsView);
//                         if (_items.Count > 0) itemsPhuPham.AddRange(_items);
//                         break;
//                     }
//                     case AppKV.BTPDinhHinh:
//                     {
//                         var _items = vmMain.VmPhieuCanBTPDinhHinh.GetsLast<PhieuCanBTPDinhHinh>(VmApp.DateTimeNow,
//                             VmApp.MaxRowsView);
//                         if (_items.Count > 0) itemsBTPDinhHinh.AddRange(_items);
//                         break;
//                     }
//                     case AppKV.TPDinhHinh:
//                     {
//                         var _items = vmMain.VmPhieuCanTPDinhHinh.GetsLast<PhieuCanTPDinhHinh>(VmApp.DateTimeNow,
//                             VmApp.MaxRowsView);
//                         if (_items.Count > 0) itemsTPDinhHinh.AddRange(_items);
//                         break;
//                     }
//                     case AppKV.XepKhuon:
//                     {
//                         var _items = vmMain.VmPhieuCanChinhXepKhuon.GetsLast<PhieuCanChinhXepKhuon>(VmApp.DateTimeNow,
//                             VmApp.MaxRowsView);
//                         if (_items.Count > 0) itemsXepKhuon.AddRange(_items);
//                         break;
//                     }
//                     case AppKV.BaoTu:
//                         break;
//                     case AppKV.CaoThit:
//                         break;
//                     case AppKV.XepKhuonRaCoi:
//                     {
//                         var _items = vmMain.VmPhieuCanRaCoi.GetsLast<PhieuCanRaCoi>(VmApp.DateTimeNow,
//                             VmApp.MaxRowsView);
//                         if (_items.Count > 0) itemsXepKhuonRaCoi.AddRange(_items);
//
//                         break;
//                     }
//                     case AppKV.XepKhuonPhu:
//                     {
//                         var _items = vmMain.VmPhieuCanPhuXepKhuon.GetsLast<PhieuCanPhuXepKhuon>(VmApp.DateTimeNow,
//                             VmApp.MaxRowsView);
//                         if (_items.Count > 0) itemsXepKhuonPhu.AddRange(_items);
//                         break;
//                     }
//                     case AppKV.XepKhuonKXL:
//                     {
//                         var _items = vmMain.VmPhieuCanXepKhuonKXL.GetsLast<PhieuCanXepKhuonKHC>(VmApp.DateTimeNow,
//                             VmApp.MaxRowsView);
//                         if (_items.Count > 0) itemsXepKhuonKXL.AddRange(_items);
//                         break;
//                     }
//
//                     case AppKV.PhuPhamv2:
//                     {
//                         var _items = vmMain.VmPhieuCanPhuPhamv2.GetsLast<PhieuCanPhuPhamv2>(VmApp.DateTimeNow,
//                             VmApp.MaxRowsView);
//                         if (_items.Count > 0) itemsPhuPhamv2.AddRange(_items);
//                         break;
//                     }
//                     case AppKV.Hq:
//                     {
//                         var _items = vmMain.VmPhieuCanHq.GetsLast<HQ_PhieuCan>(VmApp.DateTimeNow,
//                             VmApp.MaxRowsView);
//                         if (_items.Count > 0) itemsHq.AddRange(_items);
//                         break;
//                     }
//                     default:
//                         throw new ArgumentOutOfRangeException();
//                 }
//
//             foreach (var mayCan in items)
//                 switch (mayCan.WKv)
//                 {
//                     case AppKV.Main:
//                         break;
//                     case AppKV.DauAo:
//                         break;
//                     case AppKV.NguyenLieu:
//                     {
//                         var _items = itemsNguyenLieu.Where(x => x.MaMayTinhCan == mayCan.Id).ToList();
//                         if (_items.Count > 0)
//                         {
//                             mayCan.Items.AddRange(_items);
//                             mayCan.TongSoRo = itemsNguyenLieu.Count;
//                             mayCan.TongTrongLuong = itemsNguyenLieu.Sum(x => x.TrongLuong ?? 0);
//                         }
//
//                         break;
//                     }
//                     case AppKV.BTPFillet:
//                         break;
//                     case AppKV.TPFillet:
//                     {
//                         var _items = itemsTPFillet.Where(x => x.MaMayTinhCan == mayCan.Id).ToList();
//                         if (_items.Count > 0)
//                         {
//                             mayCan.Items.AddRange(_items);
//                             mayCan.TongSoRo = itemsTPFillet.Count;
//                             mayCan.TongTrongLuong = itemsTPFillet.Sum(x => x.TrongLuong ?? 0);
//                         }
//
//                         break;
//                     }
//                     case AppKV.BTPFilletv2:
//                     {
//                         var _items = itemsBTPFilletv2.Where(x => x.MaMayCan == mayCan.Id).ToList();
//                         if (_items.Count > 0)
//                         {
//                             mayCan.Items.AddRange(_items);
//                             mayCan.TongSoRo = itemsBTPFilletv2.Count;
//                             mayCan.TongTrongLuong = itemsBTPFilletv2.Sum(x => x.TrongLuong);
//                         }
//
//                         break;
//                     }
//                     case AppKV.TPFilletv2:
//                     {
//                         var _items = itemsTPFilletv2.Where(x => x.MaMayCan == mayCan.Id).ToList();
//                         if (_items.Count > 0)
//                         {
//                             mayCan.Items.AddRange(_items);
//                             mayCan.TongSoRo = itemsTPFilletv2.Count;
//                             mayCan.TongTrongLuong = itemsTPFilletv2.Sum(x => x.TrongLuongTra);
//                         }
//
//                         break;
//                     }
//                     case AppKV.PhuPham:
//                     {
//                         var _items = itemsPhuPham.Where(x => x.MaMayTinhCan == mayCan.Id).ToList();
//                         if (_items.Count > 0)
//                         {
//                             mayCan.Items.AddRange(_items);
//                             mayCan.TongSoRo = itemsPhuPham.Count;
//                             mayCan.TongTrongLuong = itemsPhuPham.Sum(x => x.TrongLuong ?? 0);
//                         }
//
//                         break;
//                     }
//                     case AppKV.BTPDinhHinh:
//                     {
//                         var _items = itemsBTPDinhHinh.Where(x => x.MaMayCan == mayCan.Id).ToList();
//                         if (_items.Count > 0)
//                         {
//                             mayCan.Items.AddRange(_items);
//                             mayCan.TongSoRo = itemsBTPDinhHinh.Count;
//                             mayCan.TongTrongLuong = itemsBTPDinhHinh.Sum(x => x.TrongLuong);
//                         }
//
//                         break;
//                     }
//                     case AppKV.TPDinhHinh:
//                     {
//                         var _items = itemsTPDinhHinh.Where(x => x.MaMayCan == mayCan.Id).ToList();
//                         if (_items.Count > 0)
//                         {
//                             mayCan.Items.AddRange(_items);
//                             mayCan.TongSoRo = itemsTPDinhHinh.Count;
//                             mayCan.TongTrongLuong = itemsTPDinhHinh.Sum(x => x.TrongLuongTra);
//                         }
//
//                         break;
//                     }
//                     case AppKV.XepKhuon:
//                     {
//                         var _items = itemsXepKhuon.Where(x => x.MaMayCan == mayCan.Id).ToList();
//                         if (_items.Count > 0)
//                         {
//                             mayCan.Items.AddRange(_items);
//                             mayCan.TongSoRo = itemsXepKhuon.Count;
//                             mayCan.TongTrongLuong = itemsXepKhuon.Sum(x => x.TrongLuong);
//                         }
//
//                         break;
//                     }
//
//                     case AppKV.BaoTu:
//                         break;
//                     case AppKV.CaoThit:
//                         break;
//                     case AppKV.XepKhuonRaCoi:
//                     {
//                         var _items = itemsXepKhuonRaCoi.Where(x => x.MayCan == mayCan.Id).ToList();
//                         if (_items.Count > 0)
//                         {
//                             mayCan.Items.AddRange(_items);
//                             mayCan.TongSoRo = itemsXepKhuonRaCoi.Count;
//                             mayCan.TongTrongLuong = itemsXepKhuonRaCoi.Sum(x => x.TrongLuong);
//                         }
//
//                         break;
//                     }
//                     case AppKV.XepKhuonPhu:
//                     {
//                         var _items = itemsXepKhuonPhu.Where(x => x.MaMayCan == mayCan.Id).ToList();
//                         if (_items.Count > 0)
//                         {
//                             mayCan.Items.AddRange(_items);
//                             mayCan.TongSoRo = itemsXepKhuonPhu.Count;
//                             mayCan.TongTrongLuong = itemsXepKhuonPhu.Sum(x => x.TrongLuong);
//                         }
//
//                         break;
//                     }
//                     case AppKV.XepKhuonKXL:
//                     {
//                         var _items = itemsXepKhuonKXL.Where(x => x.MaMayCan == mayCan.Id).ToList();
//                         if (_items.Count > 0)
//                         {
//                             mayCan.Items.AddRange(_items);
//                             mayCan.TongSoRo = itemsXepKhuonKXL.Count;
//                             mayCan.TongTrongLuong = itemsXepKhuonKXL.Sum(x => x.TrongLuong);
//                         }
//
//                         break;
//                     }
//                     case AppKV.PhuPhamv2:
//                     {
//                         var _items = itemsPhuPhamv2.Where(x => x.MaMayCan == mayCan.Id).ToList();
//                         if (_items.Count > 0)
//                         {
//                             mayCan.Items.AddRange(_items);
//                             mayCan.TongSoRo = itemsPhuPhamv2.Count;
//                             mayCan.TongTrongLuong = itemsPhuPhamv2.Sum(x => x.TrongLuong);
//                         }
//
//                         break;
//                     }
//                     case AppKV.Hq:
//                     {
//                         var _items = itemsHq.Where(x => x.MayCan == mayCan.Id).ToList();
//                         if (_items.Count > 0)
//                         {
//                             mayCan.Items.AddRange(_items);
//                             mayCan.TongSoRo = itemsHq.Count;
//                             mayCan.TongTrongLuong = itemsHq.Sum(x => x.TrongLuong);
//                         }
//
//                         break;
//                     }
//                     default:
//                         throw new ArgumentOutOfRangeException();
//                 }
//
//             lock (_lock)
//             {
//                 //Items
//                 // Create a new collection to hold the items to be added
//                 var newItems = new List<MayCan>();
//                 newItems.AddRange(items); // Use AddRange to add multiple items to the new collection
//                 var itemRemoves = new List<MayCan>();
//                 //Items.Clear();
//                 for (var i = 0; i < Items.Count; i++)
//                 {
//                     var itemNotRemove = newItems.FirstOrDefault(x => x.Id == Items[i].Id);
//                     if (itemNotRemove == null)
//                         itemRemoves.Add(Items[i]);
//                 }
//
//                 foreach (var newItem in newItems)
//                 {
//                     if (newItem.AType == AppType._type5) newItem.IsConnected = true;
//                     var _newItem = Items.FirstOrDefault(x => x.Id == newItem.Id);
//                     if (_newItem != null)
//                         Update2(newItem);
//                     else
//                         Items.Add(newItem);
//                 }
//
//                 Items.RemoveRange(itemRemoves);
//                 //Items.Clear();
//                 // Replace the existing collection with the new collection
//                 //Items.ReplaceRange(newItems);
//             }
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             throw;
//         }
//     }
//
//     public async Task<bool> CommandSendMess(string id, string mess)
//     {
//         var rl = false;
//         var item = Find(id);
//         if (item != null)
//         {
//             try
//             {
//                 var data = new DataLiteRes
//                 {
//                     IsDataAction = 0,
//                     MessStr = mess
//                 };
//                 var dataJson = JsonConvert.SerializeObject(data);
//                 var dataTranfer = new DataTranfer
//                 {
//                     Data = dataJson,
//                     IsError = false
//                 };
//                 var dataToSend = JsonConvert.SerializeObject(dataTranfer);
//                 await hubContext.Clients.Client(item.ConnectionId ?? "")
//                     .SendAsync("ReceiveData", "LITE", dataToSend);
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 return false;
//             }
//
//             rl = true;
//         }
//
//         return rl;
//     }
//
//     private string HttpGet(string url)
//     {
//         using (var client = new HttpClient())
//         {
//             using (var response = client.GetAsync(url).Result)
//             {
//                 using (var content = response.Content)
//                 {
//                     return content.ReadAsStringAsync().Result;
//                 }
//             }
//         }
//     }
//
//     private string HttpPost(string url, string data)
//     {
//         using (var client = new HttpClient())
//         {
//             var content = new StringContent(data, Encoding.UTF8, "application/json");
//             using (var response = client.PostAsync(url, content).Result)
//             {
//                 using (var content2 = response.Content)
//                 {
//                     return content2.ReadAsStringAsync().Result;
//                 }
//             }
//         }
//     }
//
//     protected virtual void OnItemChanged()
//     {
//         ItemChanged?.Invoke(this, EventArgs.Empty);
//     }
//
//     private async void OnTimedEvent(object? sender, ElapsedEventArgs e)
//     {
//         _timer.Stop();
//         for (var i = 0; i < Items.Count; i++)
//         {
//             var mayCan = Items[i];
//             if (mayCan.AType != AppType._type5)
//             {
//                 //mayCan.IsConnected = !mayCan.IsConnected;
//             }
//             else
//                 mayCan.IsConnected = true;
//         }
//
//         _timer.Start();
//         OnItemChanged();
//     }
//
//     private void StartTimer()
//     {
//         if (_timer != null)
//         {
//             _timer.Elapsed -= OnTimedEvent;
//             _timer.Stop();
//             _timer.Dispose();
//         }
//
//         _timer = new Timer(5000);
//         _timer.Elapsed += OnTimedEvent;
//         _timer.AutoReset = true;
//         _timer.Start();
//     }
//
//     public void Update2(MayCan mayCan)
//     {
//         for (var i = 0; i < Items.Count; i++)
//         {
//             var item = Items[i];
//             if (item.Id == mayCan.Id)
//             {
//                 item.MType = mayCan.MType;
//                 item.WKv = mayCan.WKv;
//                 item.AType = mayCan.AType;
//                 item.Items = mayCan.Items;
//                 item.DisplayName = mayCan.DisplayName;
//                 item.SCode = mayCan.SCode;
//                 item.Par1 = mayCan.Par1;
//                 item.Par2 = mayCan.Par2;
//                 item.Par3 = mayCan.Par3;
//                 item.Par4 = mayCan.Par4;
//                 item.Idx = mayCan.Idx;
//                 item.MaXuong = mayCan.MaXuong;
//                 item.IsSuDungMauThanhPham = mayCan.IsSuDungMauThanhPham;
//             }
//         }
//
//         OnItemChanged();
//     }
//
//     public List<T> GetItems<T>(AppKV akv)
//     {
//         var _mayCans = Items.Where(x => x.WKv == akv).ToList();
//         var _items = new List<T>();
//         foreach (var _mayCan in _mayCans)
//         {
//             foreach (var _item in _mayCan.Items)
//             {
//                 if (_item is T typedItem)
//                 {
//                     _items.Add(typedItem);
//                 }
//             }
//         }
//         return _items;
//     }
// }