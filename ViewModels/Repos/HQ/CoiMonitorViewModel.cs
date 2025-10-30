using System.Windows.Input;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Repos.Models;
using MvvmHelpers;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace ViewModels.Repos.HQ;

public partial class CoiMonitorViewModel : ObservableObject
{
    private static CoiMonitorViewModel instance;
    private readonly SynchronizationContext synchronizationContext;
    [ObservableProperty] private ICommand _closeItemWindowCommand;
    [ObservableProperty] private bool _isWindowItemShown;
    [ObservableProperty] private bool idItemIsReadOnly = true;
    [ObservableProperty] private bool isAdd;
    [ObservableProperty] private bool isEdit;
    [ObservableProperty] private CoiMonitor? item;
    [ObservableProperty] private ObservableRangeCollection<CoiMonitor> items = new();

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsVailSelectedItem))]
    private CoiMonitor? selectedItem;

    [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();

    private CoiMonitorViewModel()
    {
        try
        {
            Reload();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
            VmMessage.SetExceptionCommand.Execute(e);
        }
    }

    public static CoiMonitorViewModel Instance => instance ??= new CoiMonitorViewModel();
    public bool IsVailSelectedItem => SelectedItem != null;

    private AppViewModel VmApp => AppViewModel.Instance;
    private MessageViewModel VmMessage => MessageViewModel.Instance;

    public CoiMonitor CopyItem(CoiMonitor item)
    {
        return new CoiMonitor
        {
            Id = item.Id,
            MaCoi = item.MaCoi,
            MaXuong = item.MaXuong,
            NgayGio = item.NgayGio,
            InLocked = item.InLocked,
            OutLocked = item.OutLocked,
            MaThe = item.MaThe,
            NgayNguyenLieu = item.NgayNguyenLieu,
            TimeROut = item.TimeROut
            
        };
    }

    public CoiMonitor CopySelectedItem()
    {
        return CopyItem(SelectedItem);
    }

    public CoiMonitor CreateDefaultNew()
    {
        var id = $"{VmApp.DateTimeNow.ToString("yyyyMMddhhmmss")}.{VmApp.XuongId}.{VmApp.PCName}";
        return new CoiMonitor
        {
            Id = id,
            NgayGio = DateTime.Now,
            InLocked = false,
            OutLocked = true
        };
    }

    private int Delete<T>(T item)
    {
        var dao = new Dao.Repos.HQ.CoiMonitor();
        return dao.Delete(item);
    }

    [RelayCommand(CanExecute = nameof(IsItemPass))]
    private void Delete_(CoiMonitor item)
    {
        try
        {
            if (Delete(item) > 0)
                lock (Items)
                {
                    var _item = Items.SingleOrDefault(x => x.Id == item.Id);
                    if (_item != null)
                    {
                        var index = Items.IndexOf(_item);
                        Items.RemoveAt(index);
                        //Items.Insert(index,item);
                    }
                }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
            VmMessage.SetExceptionCommand.Execute(e);
        }
    }

    public bool Exists(CoiMonitor item)
    {
        try
        {
            return Find(item.Id) != null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public CoiMonitor? Find(string ma)
    {
        try
        {
            return Items.FirstOrDefault(x => x.Id == ma);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private List<T> Gets<T>()
    {
        var dao = new Dao.Repos.HQ.CoiMonitor();
        return dao.Gets<T>();
    }
    public List<T> GetsLast3Day<T>()
    {
        var dao = new Dao.Repos.HQ.CoiMonitor();
        return dao.GetsLast3Day<T>();
    }
    public bool Add(CoiMonitor item)
    {
        if (Exists(item)) return false;
        Items.Insert(0,item);
        if (Items.Count > 500)
        {
            Items.RemoveAt(Items.Count - 1);
        }
        return true;
    }
    public int Insert<T>(T item)
    {
        var dao = new Dao.Repos.HQ.CoiMonitor();
        return dao.Insert(item);
    }

    [RelayCommand(CanExecute = nameof(IsItemPass))]
    private void Insert_(CoiMonitor item)
    {
        try
        {
            if (Insert(item) > 0)
            {
                var items = new List<CoiMonitor>();
                lock (Items)
                {
                    Items.Add(item);
                }

                VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
                IsWindowItemShown = false;
            }
            else
            {
                VmMessage.MessageBoxShow("Không thể thêm", "Thông Báo", 0);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
            VmMessage.SetExceptionCommand.Execute(e);
        }
    }

    [RelayCommand]
    private void Insert2_()
    {
        try
        {
            if (Insert(Item) > 0)
            {
                Items.Insert(0, Item);
                VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
            }
        }
        catch (Exception e)
        {
            VmMessage.SetExceptionCommand.Execute(e);
            //throw;
        }
    }

    private bool IsItemPass(CoiMonitor item)
    {
        return item != null && item.MaCoi != null && item.MaCoi.Trim() != "" && item.MaXuong != null &&
               item.MaXuong.Trim() != "" && item.MaThe != null && item.MaThe.Trim() != "" && item.NgayGio != null;
    }

    //[RelayCommand]
    //private void ForceRaseCanExcute()
    //{
    //    try
    //    {
    //        Insert_Command.NotifyCanExecuteChanged();
    //        Delete_Command.NotifyCanExecuteChanged();
    //    }
    //    catch (Exception e)
    //    {
    //        Console.WriteLine(e);
    //        //throw;
    //    }
    //}
    public void Reload()
    {
        lock (Items)
        {
            Items.Clear();
        }

        var items = GetsLast3Day<CoiMonitor>();
        if (items.Any())
            lock (Items)
            {
                try
                {
                    //Items.AddRange(items);
                    foreach (var item in items) Items.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    [RelayCommand]
    private void Reload_(ObservableRangeCollection<CoiMonitor> obj)
    {
        try
        {
            Reload();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
            VmMessage.SetExceptionCommand.Execute(e);
        }
    }

    public int Update<T>(T item)
    {
        var dao = new Dao.Repos.HQ.CoiMonitor();
        return dao.Update(item);
    }
    /// <summary>
    /// Get Last 5 days data
    /// CoiMonitorId,
    /// MaCoi,
    /// MaXuong,
    /// NgayGio,
    /// NgayNguyenLieu,
    /// TrongLuong,
    /// MaChatLuong,
    /// MaThanhPham,
    /// MaSize,
    /// MaLo,
    /// MaChieuXa,
    /// MayQuay,
    /// ThoiGianQuay,
    /// ThoiGianBatDauQuay,
    /// NgayBatDauQuay,
    /// </summary>
    /// <typeparam name="T">
   
    /// </typeparam>
    /// <returns></returns>
    public List<T> GetInfos<T>()
    {
        var dao = new Dao.Repos.HQ.CoiMonitor();
        return dao.GetInfos<T>();
    }

    [RelayCommand(CanExecute = nameof(IsItemPass))]
    private void Update_(CoiMonitor item)
    {
        try
        {
            if (Update(item) > 0)
                lock (Items)
                {
                    var _item = Items.SingleOrDefault(x => x.Id == item.Id);
                    if (_item != null)
                    {
                        var index = Items.IndexOf(_item);
                        Items.RemoveAt(index);
                        Items.Insert(index, item);
                    }
                }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
            VmMessage.SetExceptionCommand.Execute(e);
        }
    }

    [RelayCommand]
    private void Update2_()
    {
        try
        {
            if (Update(Item) > 0)
                lock (Items)
                {
                    var _item = Items.SingleOrDefault(x => x.Id == Item.Id);
                    if (_item != null)
                    {
                        var index = Items.IndexOf(_item);
                        Items.RemoveAt(index);
                        Items.Insert(index, Item);
                        SelectedItem = Item;
                    }
                }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
            VmMessage.SetExceptionCommand.Execute(e);
        }
    }
}