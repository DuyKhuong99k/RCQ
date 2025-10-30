using System.Windows.Input;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Migrations;
using Models.Repos;
using Models.Repos.Models;
using MvvmHelpers;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace ViewModels.Repos.HQ;

public partial class TrongLuongCoiTheoThanhPhamViewModel: ObservableObject
{
    private static TrongLuongCoiTheoThanhPhamViewModel instance;
    private AppViewModel VmApp => AppViewModel.Instance;
    private MessageViewModel VmMessage => MessageViewModel.Instance;
    [ObservableProperty] private bool idItemIsReadOnly = true;
    [ObservableProperty] private bool isAdd;
    [ObservableProperty] private bool isEdit;
    [ObservableProperty] private TrongLuongCoiTheoThanhPham? item;
    [ObservableProperty] private ObservableRangeCollection<TrongLuongCoiTheoThanhPham> items = new();
    [ObservableProperty] private TrongLuongCoiTheoThanhPham selectedItem;
    [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
    [ObservableProperty] private bool isServer = true;
    [ObservableProperty] private ICommand _closeItemWindowCommand;
    [ObservableProperty] private bool _isWindowItemShown = false;
    public static TrongLuongCoiTheoThanhPhamViewModel Instance => instance ??= new();

    private readonly SynchronizationContext synchronizationContext;
    private TrongLuongCoiTheoThanhPhamViewModel()
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
    public void Reload()
    {
        // Reload();
        Items.Clear();
        using var db = new dbPMScontext();
        var items = Gets();
        if (items != null && items.Any())
        {
            foreach (var item in items)
            {
                Items.Add(item);
         
            }
        }
    }

    public List<TrongLuongCoiTheoThanhPham> Gets()
    {
        using var db = new dbPMScontext();
        return db.TrongLuongCoiTheoThanhPhams.ToList();
    }
    public TrongLuongCoiTheoThanhPham? Get(string maCoi,string maThanhPham,string xuongId)
    {
        using var db = new dbPMScontext();
        return db.TrongLuongCoiTheoThanhPhams.FirstOrDefault(x => x.MaCoi == maCoi && x.MaThanhPham == maThanhPham && x.MaXuong == xuongId);
    }
    public bool Exists(string maCoi,string maThanhPham,string xuongId)
    {
        try
        {

            return Find(maCoi,maThanhPham,xuongId) != null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
     public List<T> GetAllsFullField<T>()
        {
            var dao = new Dao.Repos.HQ.TrongLuongCoiTheoThanhPham();
             return dao.GetAllsFullField<T>();
        }
    public TrongLuongCoiTheoThanhPham? Find(string maCoi,string maThanhPham,string xuongId)
    {
        using var db = new dbPMScontext();
        return Items.FirstOrDefault(x => x.MaCoi == maCoi && x.MaThanhPham == maThanhPham && x.MaXuong == xuongId);
    }

    public TrongLuongCoiTheoThanhPham CopyItem(TrongLuongCoiTheoThanhPham item)
    {
        return new TrongLuongCoiTheoThanhPham()
        {
            MaCoi = item.MaCoi,
            MaThanhPham = item.MaThanhPham,
            MaXuong = item.MaXuong,
            TrongLuongMax = item.TrongLuongMax
        };
    }

    public int Insert(TrongLuongCoiTheoThanhPham item)
    {
        using var db = new dbPMScontext();
        var itemDb = db.TrongLuongCoiTheoThanhPhams.FirstOrDefault(x => x.MaCoi == item.MaCoi && x.MaThanhPham == item.MaThanhPham && x.MaXuong == item.MaXuong);
        if (itemDb != null)
        {
            return 0;
        }
        else
        {
            db.TrongLuongCoiTheoThanhPhams.Add(item);
            db.SaveChanges();
            return 1;
        }
    }

    public int Insert(List<TrongLuongCoiTheoThanhPham> items)
    {
        using var db = new dbPMScontext();
        int result = 0;
        foreach (var item in items)
        {
            var itemDb = db.TrongLuongCoiTheoThanhPhams.FirstOrDefault(x => x.MaCoi == item.MaCoi && x.MaThanhPham == item.MaThanhPham && x.MaXuong == item.MaXuong);
            if (itemDb != null)
            {
                
            }
            else
            {
                db.TrongLuongCoiTheoThanhPhams.Add(item);
                result++;
            }
        }
        db.SaveChanges();
        return result;
    }

    public int Update(TrongLuongCoiTheoThanhPham item)
    {
        using var db = new dbPMScontext();
        var itemDb = db.TrongLuongCoiTheoThanhPhams.FirstOrDefault(x => x.MaCoi == item.MaCoi && x.MaThanhPham == item.MaThanhPham && x.MaXuong == item.MaXuong);
        if (itemDb != null)
        {
            itemDb.TrongLuongMax = item.TrongLuongMax;
            db.SaveChanges();
            return 1;
        }
        else
        {
            return 0;
        }
        
    }

    public int Update(List<TrongLuongCoiTheoThanhPham> items)
    {
        using var db = new dbPMScontext();
        int result = 0;
        foreach (var item in items)
        {
            var itemDb = db.TrongLuongCoiTheoThanhPhams.FirstOrDefault(x => x.MaCoi == item.MaCoi && x.MaThanhPham == item.MaThanhPham && x.MaXuong == item.MaXuong);
            if (itemDb != null)
            {
                itemDb.TrongLuongMax = item.TrongLuongMax;
                result++;
            }
        }
        db.SaveChanges();
        return result;
    }
    public int Delete(TrongLuongCoiTheoThanhPham item)
    {
        using var db = new dbPMScontext();
        var itemDb = db.TrongLuongCoiTheoThanhPhams.FirstOrDefault(x => x.MaCoi == item.MaCoi && x.MaThanhPham == item.MaThanhPham && x.MaXuong == item.MaXuong);
        if (itemDb != null)
        {
            db.TrongLuongCoiTheoThanhPhams.Remove(itemDb);
            db.SaveChanges();
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public int Delete(List<TrongLuongCoiTheoThanhPham> items)
    {
        using var db = new dbPMScontext();
        int result = 0;
        foreach (var item in items)
        {
            var itemDb = db.TrongLuongCoiTheoThanhPhams.FirstOrDefault(x => x.MaCoi == item.MaCoi && x.MaThanhPham == item.MaThanhPham && x.MaXuong == item.MaXuong);
            if (itemDb != null)
            {
                db.TrongLuongCoiTheoThanhPhams.Remove(itemDb);
                result++;
            }
        }
        db.SaveChanges();
        return result;
    }
    
}