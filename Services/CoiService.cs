using System.Text.Json;
using System.Timers;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.SignalR;
using Models.Repos.Models;
using MvvmHelpers;
using Vars.Hubs;
using ViewModels.Repos.Hubs.IServices;
using MaCoiXepKhuon = Dao.Repos.HQ.MaCoiXepKhuon;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using Timer = System.Timers.Timer;

namespace Services;

public partial class CoiService : ObservableObject, ICoiService
{
    private readonly IHubContext<ChatHub> hubContext;
    private readonly IMainService vmMain;
    private Timer? _timer;
    [ObservableProperty] private ObservableRangeCollection<PLCChiTiet> items = new();

    [ObservableProperty] private ObservableRangeCollection<object> liteReports = new();

    [ObservableProperty] private ObservableRangeCollection<CoiLogs> logs = new();

    [ObservableProperty] private List<PLCChiTiet> selectedItems;


    public CoiService(IMainService vmMain, IHubContext<ChatHub> hubContext)
    {
        this.vmMain = vmMain;
        this.hubContext = hubContext;
        try
        {
            Load();
            StartTimer();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            //throw;
            VmMessage.SetExceptionCommand.Execute(e);
        }
    }

    private AppViewModel VmApp => AppViewModel.Instance;
    private MessageViewModel VmMessage => MessageViewModel.Instance;
    public event EventHandler? ItemChanged;

    public PLCChiTiet? Find(string id)
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

    public PLCChiTiet? Find(string coiId, string xuongId)
    {
        try
        {
            return Items.FirstOrDefault(x => x.MaCoi == coiId && x.MaXuong == xuongId);
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
            var item = Items.FirstOrDefault(x => x.Id == id);
            if (item != null) Items.Replace(item);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

  
    public void Load()
    {
        try
        {
            var items = vmMain.VmPLCChiTiet.Items.ToList();
            lock (Items)
            {
                Items.Clear();
                Items.AddRange(items);
                LoadData();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void Update(PLCChiTiet item)
    {
        //throw new NotImplementedException();
    }

    public void NotifyItemChanged()
    {
        OnItemChanged();
    }

    public void Dispose()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }

    public bool Command_PAUSE(PLCChiTiet item)
    {
        try
        {
            var plcService = new PLCService(vmMain);
            plcService.SetPause(item);
            item.State = 2;
            Replace(item);
            NotifyItemChanged();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public bool Command_RESUME(PLCChiTiet item)
    {
        try
        {
            var plcService = new PLCService(vmMain);
            plcService.SetResume(item);
            item.State = 1;
            Replace(item);
            NotifyItemChanged();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public bool Command_RUNOUT(PLCChiTiet item)
    {
        try
        {
            var plcService = new PLCService(vmMain);
            plcService.SetRUNOUT(item);
            item.State = 3;
            Replace(item);
            NotifyItemChanged();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> Command_RUN(PLCChiTiet item)
    {
        try
        {
            var plcService = new PLCService(vmMain);
            await plcService.SetRun(item);
            item.State = 1;
            Replace(item);
            NotifyItemChanged();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public bool Command_SETPARAMETER(PLCChiTiet item)
    {
        try
        {
            var plcService = new PLCService(vmMain);
            plcService.SetParameter(item);
            Replace(item);
            NotifyItemChanged();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public bool Command_SETPARAMATERANDDATA(PLCChiTiet item)
    {
        try
        {
            var plcService = new PLCService(vmMain);
            plcService.SetParameter(item);
            // Do data
            if (item.IdMonitor != null && item.IdMonitor.Trim() != "" && item.MaChatLuong != null &&
                item.MaChatLuong.Trim() != "")
                vmMain.VmPhieuCanChinhXepKhuon.UpdateChatLuongByIdMonitor(item.MaChatLuong ?? "", item.IdMonitor);
            Replace(item);
            NotifyItemChanged();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public bool Command_SETDATA(PLCChiTiet item)
    {
        try
        {
            // Do data

            Replace(item);
            NotifyItemChanged();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public bool Command_STARTTIMER()
    {
        try
        {
            StartTimer();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public DataCoi GetDataCoi(PLCChiTiet item)
    {
        try
        {
            var dataCoi = new DataCoi
            {
                CoiTamInfos = item.CoiTamInfos,
                XuongId = item.MaXuong,
                MayQuay = item.PCQuay ?? "",
                MaChatLuong = item.MaChatLuong,
                CoiId = item.MaCoi,
                Forced = false,
                IsRun = item.IsPowerOn,
                IsUpdateUI = true,
                ThoiGianQuay = item.ThoiGianQuay,
                ThoiGianBatDauQuay = item.TimeRun,
                ThoiGianRaCoi = item.TimeRaCoi
            };
            return dataCoi;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> Command_STOP(PLCChiTiet item)
    {
        try
        {
            var plcService = new PLCService(vmMain);
            await plcService.SetStop(item);
            item.State = -1;
            Replace(item);
            NotifyItemChanged();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public bool IsReadyToRun(PLCChiTiet item)
    {
        try
        {
            //if(item.MaChatLuong != null && item.MaChatLuong.Trim() != "" && item.)
            //    return true;
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void LoadData()
    {
        try
        {
            var datas = vmMain.VmCoiMonitor.GetInfos<dynamic>();
            var logs = vmMain.VmCoiLogs.GetLasts5Day<CoiLogs>();
            var items = datas.GroupBy(g => new { g.Id, g.MaCoi, g.MaXuong, g.NgayGio, g.NgayNguyenLieu, g.MaThe })
                .Select(x =>
                    new
                    {
                        Id = (string)x.Key.Id,
                        MaCoi = (string)x.Key.MaCoi,
                        MaXuong = (string)x.Key.MaXuong,
                        NgayGio = (DateTime?)x.Key.NgayGio,
                        NgayNguyenLieu = (DateTime?)x.Key.NgayNguyenLieu,
                        MaThe = (string?)x.Key.MaThe,
                        MaChatLuong = x.Select(x => (string)x.MaChatLuong).ToList(),
                        MaThanhPham = x.Select(x => (string)x.MaThanhPham).ToList(),
                        MaSize = x.Select(x => (string)x.MaSize).ToList(),
                        MaLo = x.Select(x => (string)x.MaLo).ToList(),
                        MaChieuXa = x.Select(x => (string)x.MaChieuXa).ToList(),
                        MayQuay = x.Select(x => (string)x.MayQuay).ToList(),
                        ThoiGianQuay = x.Select(y => (int?)y.ThoiGianQuay).OrderByDescending(y => y).FirstOrDefault(),
                        ThoiGianBatDauQuay = x.Select(y => (DateTime?)y.ThoiGianBatDauQuay).OrderBy(y => y)
                            .FirstOrDefault(),
                        NgayBatDauQuay = x.Select(y => (DateTime?)y.NgayBatDauQuay).OrderBy(y => y).FirstOrDefault(),
                        TrongLuong = x.Sum(x => (decimal)x.TrongLuong),
                        PhieuCans = x.Select(x => new
                        {
                            MaThanhPham = (string?)x.MaThanhPham, MaSize = (string?)x.MaSize,
                            MaChatLuong = (string?)x.MaChatLuong, MaChieuXa = (string?)x.MaChieuXa,
                            TrongLuong = (decimal?)x.TrongLuong
                        }).ToList(),
                        Infos = x.Select(x => new Tuple<string, string>((string)x.MaCoiTam, (string)x.MaMayCan))
                            .Distinct().ToList()
                    }).ToList();
            foreach (var plcChiTiet in Items)
            {
                var item = items.FirstOrDefault(x => x.MaCoi == plcChiTiet.MaCoi && x.MaXuong == plcChiTiet.MaXuong);
                if (item != null)
                {
                    var _logs = logs.Where(x => x.MaCoi == item.MaCoi && x.MaXuong == item.MaXuong).ToList();
                    if(_logs.Count > 0)
                    {
                        plcChiTiet.Logs.AddRange(_logs);
                    }
                    plcChiTiet.TrongLuong = item.TrongLuong;
                    plcChiTiet.MaChatLuong = item.MaChatLuong.FirstOrDefault();
                    plcChiTiet.MaXuong = item.MaXuong;
                    plcChiTiet.MaCoi = item.MaCoi;
                    // plcChiTiet. = item.MaLo.FirstOrDefault();
                    plcChiTiet.IdMonitor = item.Id;
                    plcChiTiet.ThoiGianQuay = (ushort)(item.ThoiGianQuay ?? 0);
                    plcChiTiet.TimeRun = item.ThoiGianBatDauQuay;
                    plcChiTiet.LiteReports.AddRange(item.PhieuCans);
                    plcChiTiet.CoiTamInfos.AddRange(item.Infos);
                    plcChiTiet.TheId = item.MaThe;
                    plcChiTiet.PCQuay = item.MayQuay.FirstOrDefault();
                    
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    protected virtual void OnItemChanged()
    {
        ItemChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnTimedEvent(object? sender, ElapsedEventArgs e)
    {
        try
        {
            _timer?.Stop();
            var plcService = new PLCService(vmMain);
            var count = Items.Count;
            for (var i = 0; i < count; i++)
                try
                {
                    var item = Items[i];

                    if (item.TimeRun != null && item.TimeRaCoi == null)
                    {
                        var timeOut = item.TimeRun.Value.AddMinutes(item.ThoiGianQuay);
                        if (timeOut > DateTime.Now)
                        {
                            var data = GetDataCoi(item);
                            data.IsRun = false;
                            data.ThoiGianRaCoi = DateTime.Now;
                            data.IsUpdateUI = false;

                            var datajson = JsonSerializer.Serialize(data);
                            if (vmMain.VmPhieuCanChinhXepKhuon.SetRaCoiStateByIdMonitor(item.IdMonitor ?? "",
                                    data.ThoiGianRaCoi.Value.TimeOfDay, data.ThoiGianRaCoi.Value.Date, data.Forced) > 0)
                            {
                                item.IsPowerOn = false;
                                item.State = 4;
                            }
                        }
                    }

                    plcService.GetParameter(ref item);
                }
                catch (Exception exception)
                {
                    //Console.WriteLine(exception);
                    //VmMessage.SetExceptionCommand.Execute(e);
                }

            //NotifyItemChanged();
            _timer?.Start();
            OnItemChanged();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            //VmMessage.SetExceptionCommand.Execute(e);
            //throw;
        }
        finally
        {
            if (_timer != null && !_timer.Enabled)
                _timer.Start();
        }
    }

    private bool Replace(PLCChiTiet item)
    {
        try
        {
            var _item = Items.FirstOrDefault(x => x.Id == item.Id);
            if (_item != null)
            {
                var index = Items.IndexOf(_item);
                if (index >= 0)
                {
                    //Items[index] = item;
                    //Items[index].IsPowerOn = item.IsPowerOn;
                    //Items[index].State = item.State;
                    //Items[index].Id
                    Items[index].MaCoi = item.MaCoi;
                    Items[index].MaXuong = item.MaXuong;
                    Items[index].PLCId = item.PLCId;
                    Items[index].RUN = item.RUN;
                    Items[index].STOP = item.STOP;
                    Items[index].TimeQuay = item.TimeQuay;
                    Items[index].HzQuay = item.HzQuay;
                    Items[index].HzRa = item.HzRa;
                    Items[index].HzQuayDef = item.HzQuayDef;
                    Items[index].HzRaDef = item.HzRaDef;
                    Items[index].State = item.State;
                    Items[index].TanSoQuay = item.TanSoQuay;
                    Items[index].TanSoRa = item.TanSoRa;
                    Items[index].ThoiGianQuay = item.ThoiGianQuay;
                    Items[index].TimeQuayDef = item.TimeQuayDef;
                    Items[index].TimeRaCoi = item.TimeRaCoi;
                    Items[index].TimeRun = item.TimeRun;
                    Items[index].TimeStop = item.TimeStop;
                    Items[index].INVERTER = item.INVERTER;
                    Items[index].RUNSTATUS = item.RUNSTATUS;
                    Items[index].IsConnected = item.IsConnected;
                    Items[index].PAUSE = item.PAUSE;
                    Items[index].RUNOUT = item.RUNOUT;
                    Items[index].NhanVienId = item.NhanVienId;
                    Items[index].TrongLuong = item.TrongLuong;
                    Items[index].IdMonitor = item.IdMonitor;
                    Items[index].IsPause = item.IsPause;
                    Items[index].IsPowerOn = item.IsPowerOn;
                    Items[index].IsRunOut = item.IsRunOut;
                    Items[index].LiteReports = item.LiteReports;
                    Items[index].Logs = item.Logs;
                    Items[index].MaChatLuong = item.MaChatLuong;
                    Items[index].CoiTamInfos = item.CoiTamInfos;
                    Items[index].TheId = item.TheId;
                    Items[index].PCQuay = item.PCQuay;

                    return true;
                }
            }

            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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
}