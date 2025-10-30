using System.Text;
using System.Timers;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Repos.Models;
using MvvmHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vars;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using Timer = System.Timers.Timer;

namespace Services;

public partial class MayCansService : ObservableObject, IMayCansService
{
    private readonly object _lock = new();
    private readonly IHubContext<ChatHub> hubContext;
    private readonly IMainService vmMain;
    private Timer? _timer;
    [ObservableProperty] private ObservableRangeCollection<MayCan> items = new();

    [ObservableProperty] private List<MayCan> selectedItems = new();

    public MayCansService(IMainService vmMain, IHubContext<ChatHub> hubContext)
    {
        this.vmMain = vmMain;
        this.hubContext = hubContext;
        try
        {
            //StartTimer();
            Load();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine(e);
            //throw;
            VmMessage.SetExceptionCommand.Execute(e);
        }
    }

    private AppViewModel VmApp => AppViewModel.Instance;
    private MessageViewModel VmMessage => MessageViewModel.Instance;

    public MayCan? Find(string id)
    {
        try
        {
            return Items.FirstOrDefault(x => x.Id == id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public MayCan? Find(string id, bool isConnected)
    {
        var mayCan = Find(id);
        if (mayCan == null) return null;
        return mayCan.IsConnected ? mayCan : null;
    }

    public void AddItem(MayCan mayCan,object item)
    {
        mayCan.Items.Insert(0,item);
    }

    public void Remove(string id)
    {
        try
        {
            var item = Items.FirstOrDefault(x => x.Id == id);
            if (item != null) Items.Replace(item);
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
            var item = Items.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                var index = Items.IndexOf(item);
                if (index >= 0)
                {
                    item.IsActive = false;
                    if (item.AType != AppType._type5)
                        item.IsConnected = false;
                    item.ConnectionId = "";
                    Items.RemoveAt(index);
                    Items.Insert(index, item);
                }
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
            var item = Items.FirstOrDefault(x => x.ConnectionId == id);
            if (item != null)
            {
                var index = Items.IndexOf(item);
                if (index >= 0)
                {
                    item.IsActive = false;
                    if (item.AType != AppType._type5)
                        item.IsConnected = false;
                    item.ConnectionId = "";
                    Update(item);
                }
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
    }

    public void SetAllExpand(bool isExpanded)
    {
        foreach (var mayCan in Items) mayCan.IsExpand = isExpanded;
    }

    public void SetIsConnected(bool isConnected, string id)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            if (item.Id == id && item.AType != AppType._type5) item.IsConnected = isConnected;
        }

        OnItemChanged();
    }

    public void Update(MayCan mayCan)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            if (item.Id == mayCan.Id)
            {
                item.MType = mayCan.MType;
                item.ConnectionId = mayCan.ConnectionId;
                item.DateTimeConnected = mayCan.DateTimeConnected;
                item.IPAddr = mayCan.IPAddr;
                item.IsActive = mayCan.IsActive;
                if (item.AType != AppType._type5)
                    item.IsConnected = true;
                else
                    item.IsConnected = mayCan.IsConnected;
                item.WKv = mayCan.WKv;
                item.AType = mayCan.AType;
            }
        }

        OnItemChanged();
    }

    public event EventHandler? ItemChanged;

    public void NotifyItemChanged()
    {
        OnItemChanged();
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
                
                if (item.AType != AppType._type5)
                {
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("ZERO", "0");
                }
                else
                {
                    var url = $"http://{item.IPAddr}/api/w/zero";
                    var data = new { CMD = "ZERO"};
                    var dataJson = JsonConvert.SerializeObject(data);
                    var dJson1 = HttpPost($"{url}",dataJson);
                }
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
                return false;
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
               
                if (item.AType != AppType._type5)
                {
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("TARE", $"{trongLuongTare ?? 0}");
                }
                else
                {
                    var url = $"http://{item.IPAddr}/api/w/tare";
                    var _trongLuongTare = trongLuongTare ?? 0;
                    var data = new { trongluongtare = _trongLuongTare};
                    var dataJson = JsonConvert.SerializeObject(data);
                    var dJson1 = HttpPost($"{url}",dataJson);
                }
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

    public async Task<bool> CommandGetLo(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                if (item.AType != AppType._type5)
                {
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETLO", "");
                }
                else
                {
                    var url = $"http://{item.IPAddr}/api/lo/getselecteditem";
                    var dataJson = HttpGet(url);
                    var json = JObject.Parse(dataJson);
                    var datacount = 0;
                    int.TryParse(json["datacount"]?.ToString(), out datacount);
                    if (datacount > 0)
                    {

                        var maLo = (json.SelectToken("data[0].Id"))?.ToString();
                        item.MaLo = maLo ?? string.Empty;
                    }
                }
            }
            catch (Exception e)
            {
                vmMain.VmMessage.MessageBoxShow(e.ToString(), "", 0);
                Console.WriteLine(e);
                return false;
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
                
                if (item.AType != AppType._type5)
                {
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETLO", $"{maLo}");
                }
                else
                {
                    var url = $"http://{item.IPAddr}/api/lo/";
                    var data = new { Id = maLo,NgayNguyenLieu = DateTime.Now.ToString("yyyyMMddhhmmss"),SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd")};
                    var dataJson = JsonConvert.SerializeObject(data);
                    var dJson1 = HttpPost($"{url}insert",dataJson);
                    var dJson2 = HttpPost($"{url}selecteditem", dataJson);

                    //var json = JObject.Parse(dataJson);
                    //var datacount = 0;
                    //int.TryParse(json["datacount"]?.ToString(), out datacount);
                    //if (datacount > 0)
                    //{

                    //    //var maLo = (json.SelectToken("data[0].Id"))?.ToString();
                    item.MaLo = maLo;
                    //}
                }
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
                
                if (item.AType != AppType._type5)
                {
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETTHANHPHAM", "");
                }
                else
                {
                    var url = $"http://{item.IPAddr}/api/thanhpham/getselecteditem";
                    var dataJson = HttpGet(url);
                    var json = JObject.Parse(dataJson);
                    var datacount = 0;
                    int.TryParse(json["datacount"]?.ToString(), out datacount);
                    if (datacount > 0)
                    {

                        var maThanhPham = (json.SelectToken("data[0].Id"))?.ToString();
                        item.MaThanhPham = maThanhPham ?? string.Empty;
                    }
                }
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
                return false;
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
                return false;
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
                if (item.AType != AppType._type5)
                {
                    if (item.MType == "DESKTOP")
                        await hubContext.Clients.Client(item.ConnectionId ?? "")
                            .SendAsync("SETTHANHPHAM", $"{thanhPhamId}");
                    else if (item.MType == "BOARD")
                        await hubContext.Clients.Client(item.ConnectionId ?? "")
                            .SendAsync("SETTHANHPHAM", $"{thanhPhamId}", $"{thanhPhamName}");
                }
                else
                {
                    
                    var url = $"http://{item.IPAddr}/api/thanhpham/";
                    var data = new { Id = thanhPhamId,Ten = thanhPhamName,SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd")};
                    var dataJson = JsonConvert.SerializeObject(data);
                    var dJson1 = HttpPost($"{url}insert",dataJson);
                    var dJson2 = HttpPost($"{url}selecteditem", dataJson);
                    item.MaThanhPham = thanhPhamId;
                    //}
                }
               
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

    public async Task<bool> CommandGetSize(string id)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                
                if (item.AType != AppType._type5)
                {
                    await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("GETSIZE", "");
                }
                else
                {
                    var url = $"http://{item.IPAddr}/api/size/getselecteditem";
                    var dataJson = HttpGet(url);
                    var json = JObject.Parse(dataJson);
                    var datacount = 0;
                    int.TryParse(json["datacount"]?.ToString(), out datacount);
                    if (datacount > 0)
                    {

                        var masize = (json.SelectToken("data[0].Id"))?.ToString();
                        item.MaSize = masize ?? string.Empty;
                    }
                }
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

    public async Task<bool> CommandSetSize(string id, string sizeId, string sizeName)
    {
        var rl = false;
        var item = Find(id);
        if (item != null)
        {
            try
            {
                
                if (item.AType != AppType._type5)
                {
                    if (item.MType == "DESKTOP")
                        await hubContext.Clients.Client(item.ConnectionId ?? "").SendAsync("SETSIZE", $"{sizeId}");
                    else if (item.MType == "BOARD")
                        await hubContext.Clients.Client(item.ConnectionId ?? "")
                            .SendAsync("SETSIZE", $"{sizeId}", $"{sizeName}");
                }
                else
                {
                    
                    var url = $"http://{item.IPAddr}/api/thanhpham/";
                    var data = new { Id = sizeId,Ten = sizeName,SuDung=true,MNgay = DateTime.Now.ToString("yyyyMMdd")};
                    var dataJson = JsonConvert.SerializeObject(data);
                    var dJson1 = HttpPost($"{url}insert",dataJson);
                    var dJson2 = HttpPost($"{url}selecteditem", dataJson);
                    item.MaSize = sizeId;
                    //}
                }
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
                return false;
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
                return false;
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
                return false;
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
                return false;
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
                return false;
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
                return false;
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
            var itemsPhuPham = new List<PhieuCanPhuPham>();
            var itemsPhuPhamv2 = new List<PhieuCanPhuPhamv2>();
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
                for (var i = 0; i < Items.Count; i++)
                {
                    var itemNotRemove = newItems.FirstOrDefault(x => x.Id == Items[i].Id);
                    if (itemNotRemove == null)
                        itemRemoves.Add(Items[i]);
                }

                foreach (var newItem in newItems)
                {
                    if (newItem.AType == AppType._type5) newItem.IsConnected = true;
                    var _newItem = Items.FirstOrDefault(x => x.Id == newItem.Id);
                    if (_newItem != null)
                        Update2(newItem);
                    else
                        Items.Add(newItem);
                }

                Items.RemoveRange(itemRemoves);
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

    protected virtual void OnItemChanged()
    {
        ItemChanged?.Invoke(this, EventArgs.Empty);
    }

    private async void OnTimedEvent(object? sender, ElapsedEventArgs e)
    {
        _timer.Stop();
        for (var i = 0; i < Items.Count; i++)
        {
            var mayCan = Items[i];
            if (mayCan.AType != AppType._type5)
                mayCan.IsConnected = !mayCan.IsConnected;
            else
                mayCan.IsConnected = true;
        }

        _timer.Start();
        OnItemChanged();
    }

    private void StartTimer()
    {
        if (_timer != null)
        {
            _timer.Elapsed -= OnTimedEvent;
            _timer.Stop();
            _timer.Dispose();
        }

        _timer = new Timer(1000);
        _timer.Elapsed += OnTimedEvent;
        _timer.AutoReset = true;
        _timer.Start();
    }

    public void Update2(MayCan mayCan)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            if (item.Id == mayCan.Id)
            {
                item.MType = mayCan.MType;
                item.WKv = mayCan.WKv;
                item.AType = mayCan.AType;
                item.Items = mayCan.Items;
                item.DisplayName = mayCan.DisplayName;
                item.SCode = mayCan.SCode;
                item.Par1 = mayCan.Par1;
                item.Par2 = mayCan.Par2;
                item.Par3 = mayCan.Par3;
                item.Par4 = mayCan.Par4;
                item.Idx = mayCan.Idx;
                item.MaXuong = mayCan.MaXuong;
                item.IsSuDungMauThanhPham = mayCan.IsSuDungMauThanhPham;
            }
        }

        OnItemChanged();
    }
}