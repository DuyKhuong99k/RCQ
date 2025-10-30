using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Repos.Models;
using MvvmHelpers;
using Vars;
using ViewModels.Repos.Hubs.IServices;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using Timer = System.Timers.Timer;

namespace Services;

public partial class DeviceViewService(IMayCansService mayCansService,IUserService userService) : ObservableObject, IDeviceViewService
{
    private Timer _timer;

    [ObservableProperty] private ObservableRangeCollection<MayCan> items = new();
    [ObservableProperty] private string deviceSelectedId = "";
    [ObservableProperty] private MayCan? deviceCard;
    public List<MayCan> SelectedItems { get; set; }

    public void Load(int userId)
    {
        //_timer = new System.Timers.Timer(1000);
        //_timer.Elapsed += OnTimedEvent;
        //_timer.AutoReset = true;
        //_timer.Start();
        //Items.Clear();
//#if DEBUG
        Items.Clear();
        var userAreas = userService.UserAreas.Where(x=>x.UserId == userId).Select(x=>x.WKv).Distinct().ToList();
        foreach (var userArea in userAreas)
        {
            var items = mayCansService.Items.Where(x=>x.WKv == (AppKV)userArea);
            foreach (var mayCan in items) Items.Add(mayCan);
        }
       
//#else 
//            var items = mayCansService.Items.Where(x => x.MaXuong == xuongId && x.WKv == kv);
//            foreach (var mayCan in items)
//            {
//               Items.Add(mayCan);
//            }
//#endif
    }

    public void Clear()
    {
        Items.Clear();
    }

    public event EventHandler? DevicesChanged;

    public void Dispose()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }

    protected virtual void OnDevicesChanged()
    {
        DevicesChanged?.Invoke(this, EventArgs.Empty);
    }

    private async void OnTimedEvent(object? sender, ElapsedEventArgs e)
    {
        _timer.Stop();
        for (var i = 0; i < Items.Count; i++)
        {
            var mayCan = Items[i];
            mayCansService.SetIsConnected(!mayCan.IsConnected, mayCan.Id);
            //mayCan.IsConnected =  _maycan?.IsConnected??false;
            //Items.Insert(i,mayCan);
        }

        _timer.Start();
        OnDevicesChanged();
    }
}