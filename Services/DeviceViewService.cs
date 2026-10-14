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
    [ObservableProperty] private XiNghiep? xiNghiep;
    public List<MayCan> SelectedItems { get; set; }
    private HashSet<AppKV> _allowedAreas = new();
    private string? _workshopCode;
    

    public void Load(int userId)
    {
        //_timer = new System.Timers.Timer(1000);
        //_timer.Elapsed += OnTimedEvent;
        //_timer.AutoReset = true;
        //_timer.Start();
        //Items.Clear();
//#if DEBUG
        lock (Items)
        {
            Items.Clear();
        }
        _allowedAreas = userService.UserAreas.Where(x => x.UserId == userId)
            .Select(x => (AppKV)x.WKv).ToHashSet();
        _workshopCode = XiNghiep?.Ma?.Trim();
        // Load có thể được gọi nhiều lần. Không đăng ký sự kiện trùng.
        mayCansService.ItemUpdated -= MayCansServiceOnItemUpdated;
        mayCansService.ItemAdded -= MayCansServiceOnItemAdded;
        mayCansService.ItemRemoved -= MayCansServiceOnItemRemoved;
       mayCansService.ItemUpdated += MayCansServiceOnItemUpdated;
       mayCansService.ItemAdded += MayCansServiceOnItemAdded; 
       mayCansService.ItemRemoved+= MayCansServiceOnItemRemoved;
        lock (Items)
        {
            foreach (var item in mayCansService.Items.Values.Where(IsVisible))
                if (!Items.Any(x => x.Id == item.Id)) Items.Add(item);
            ClearHiddenSelection();
        }
        OnDevicesChanged();
//#else 
//            var items = mayCansService.Items.Where(x => x.MaXuong == xuongId && x.WKv == kv);
//            foreach (var mayCan in items)
//            {
//               Items.Add(mayCan);
//            }
//#endif
    }

    private void MayCansServiceOnItemRemoved(string id)
    {
        lock (Items)
        {
            var toRemove = Items.FirstOrDefault(x => x.Id == id);
            if (toRemove == null) return;
            Items.Remove(toRemove);
            ClearHiddenSelection();
        }
        OnDevicesChanged();
        
    }

    private void MayCansServiceOnItemAdded(MayCan obj)
    {
        MayCansServiceOnItemUpdated(obj);
    }

    private bool IsVisible(MayCan item) =>
        !string.IsNullOrEmpty(_workshopCode) &&
        string.Equals(item.MaXuong?.Trim(), _workshopCode, StringComparison.OrdinalIgnoreCase) &&
        _allowedAreas.Contains(item.WKv);

    private void ClearHiddenSelection()
    {
        SelectedItems?.RemoveAll(x => !IsVisible(x) || !Items.Any(item => item.Id == x.Id));
        if (DeviceCard != null && (!IsVisible(DeviceCard) || !Items.Any(x => x.Id == DeviceCard.Id))) DeviceCard = null;
        if (!Items.Any(x => x.Id == DeviceSelectedId)) DeviceSelectedId = "";
    }

    private void MayCansServiceOnItemUpdated(MayCan? obj)
    {
        if (obj != null && Items != null)
        {
            lock (Items)
            {
                var existing = Items.FirstOrDefault(x => x.Id == obj.Id);
                if (!IsVisible(obj))
                {
                    if (existing == null) return;
                    Items.Remove(existing);
                }
                else if (existing == null) Items.Add(obj);
                else if (!ReferenceEquals(existing, obj)) existing.CopyFrom(obj);
                ClearHiddenSelection();
            }
            OnDevicesChanged();
        }
    }

    public void Clear()
    {
        lock (Items)
        {
            Items.Clear();
        }
    }

    public event EventHandler? DevicesChanged;

    public void Dispose()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Dispose();
        }
        mayCansService.ItemUpdated -= MayCansServiceOnItemUpdated;
        mayCansService.ItemAdded -= MayCansServiceOnItemAdded; 
        mayCansService.ItemRemoved-= MayCansServiceOnItemRemoved;
        Items.Clear();
        // Items = null!;
        if (SelectedItems != null)
        {
            SelectedItems?.Clear();
            // SelectedItems = null!;
        }
    }

    protected virtual void OnDevicesChanged()
    {
        DevicesChanged?.Invoke(this, EventArgs.Empty);
    }

    // private async void OnTimedEvent(object? sender, ElapsedEventArgs e)
    // {
    //     _timer.Stop();
    //     for (var i = 0; i < Items.Count; i++)
    //     {
    //         var mayCan = Items[i];
    //         mayCansService.SetIsConnected(!mayCan.IsConnected, mayCan.Id);
    //         //mayCan.IsConnected =  _maycan?.IsConnected??false;
    //         //Items.Insert(i,mayCan);
    //     }
    //
    //     _timer.Start();
    //     OnDevicesChanged();
    // }
}
