using AppModels;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Repos.Models;
using MvvmHelpers;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System;
using System.Diagnostics;
using System.Windows;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using System.Windows.Input;
using Azure.Identity;
using System.Collections.Specialized;
using Models.Repos;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

public partial class TrongLuongCoiTheoSanPhamViewModel : ObservableObject
{
    private static TrongLuongCoiTheoSanPhamViewModel instance;
    [ObservableProperty] private bool idItemIsReadOnly = true;
    [ObservableProperty] private bool isAdd;
    [ObservableProperty] private bool isEdit;
    [ObservableProperty] private TrongLuongCoiTheoSanPham? item;
    [ObservableProperty] private ObservableRangeCollection<TrongLuongCoiTheoSanPham> items = new();
    [ObservableProperty] private ObservableRangeCollection<TrongLuongCoiTheoSanPham> usedItems = new();
    [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private TrongLuongCoiTheoSanPham? selectedItem;
    [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
    [ObservableProperty] private ICommand _closeItemWindowCommand;
    [ObservableProperty] private bool _isWindowItemShown = false;
    private readonly SynchronizationContext synchronizationContext;
    private TrongLuongCoiTheoSanPhamViewModel()
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

    public static TrongLuongCoiTheoSanPhamViewModel Instance => instance ??= new TrongLuongCoiTheoSanPhamViewModel();

    private AppViewModel VmApp => AppViewModel.Instance;
    private MessageViewModel VmMessage => MessageViewModel.Instance;

    public TrongLuongCoiTheoSanPham CopyItem(TrongLuongCoiTheoSanPham item)
    {
        return new TrongLuongCoiTheoSanPham
        {
            Id = item.Id,
            MaCoi = item.MaCoi,
            MaSanPham = item.MaSanPham,
            TrongLuong = item.TrongLuong
        };
    }
    public TrongLuongCoiTheoSanPham CopySelectedItem()
    {
        return CopyItem(SelectedItem);
    }

    public TrongLuongCoiTheoSanPham CreateDefaultNew()
    {
        var maxId = Items
               .Select(x => x.Id)
               .DefaultIfEmpty(0)
               .Max();
        var id = maxId + 1;
        return new TrongLuongCoiTheoSanPham
        {
            Id = id,
        };
    }

    private int Delete<T>(T item)
    {
        var dao = new Dao.Repos.HQ.TrongLuongCoiTheoSanPham();
        return dao.Delete(item);
    }

    [RelayCommand(CanExecute = nameof(IsItemPass))]
    private void Delete_(TrongLuongCoiTheoSanPham item)
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
    [RelayCommand(CanExecute = nameof(IsItemPass))]
    public async Task DeleteAsync(TrongLuongCoiTheoSanPham item)
    {
        try
        {
            var result = await Task.Run(() => Delete(item));

            if (result > 0)
            {
                lock (Items)
                {
                    var _item = Items.SingleOrDefault(x => x.Id == item.Id);
                    if (_item != null)
                    {
                        Items.Remove(_item);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            VmMessage.SetExceptionCommand.Execute(e);
        }
    }
    private List<T> Gets<T>()
    {
        var dao = new Dao.Repos.HQ.TrongLuongCoiTheoSanPham();
        return dao.Gets<T>();
    }
    public List<T> GetsFullField<T>(string? connStr = null)
    {
        var dao = new Dao.Repos.HQ.TrongLuongCoiTheoSanPham();
        return dao.GetAllsFullField<T>();
    }
    

    private int Insert<T>(T item)
    {
        var dao = new Dao.Repos.HQ.TrongLuongCoiTheoSanPham();
        return dao.Insert(item);
    }

    [RelayCommand(CanExecute = nameof(IsItemPass))]
    private void Insert_(TrongLuongCoiTheoSanPham item)
    {
        try
        {
            if (Insert(item) > 0)
            {
                var items = new List<TrongLuongCoiTheoSanPham>();
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

    [RelayCommand()]
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
    [RelayCommand(CanExecute = nameof(IsItemPass))]
    public async Task InsertAsync(TrongLuongCoiTheoSanPham item)
    {
        try
        {
            var result = await Task.Run(() => Insert(item));

            if (result > 0)
            {

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
            VmMessage.SetExceptionCommand.Execute(e);
        }
    }
    public bool IsVailSelectedItem => SelectedItem != null;
    private bool IsItemPass(TrongLuongCoiTheoSanPham item)
    {
        return item != null && item.MaCoi != null && item.MaSanPham != null;
    }
    [RelayCommand]
    private void ForceRaseCanExcute()
    {
        try
        {
            Insert_Command.NotifyCanExecuteChanged();
            Delete_Command.NotifyCanExecuteChanged();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
    }
    public void Reload()
    {
        lock (Items)
        {
            Items.Clear();
            UsedItems.Clear();
        }

        var items = Gets<TrongLuongCoiTheoSanPham>();
        if (items.Any())
            lock (Items)
            {
                try
                {
                    //Items.AddRange(items);
                    foreach (var item in items)
                    {
                        Items.Add(item);
                    }
                }
                catch (NotSupportedException e)
                {

                }

            }


    }


    [RelayCommand]
    private void Reload_(ObservableRangeCollection<TrongLuongCoiTheoSanPham> obj)
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

    //public bool IsDuplicateInDayExceptId(string maCoi, string maSanPham, DateTime ngayGio, int id)
    //{
    //    using (var db = new dbPMScontext())
    //    {
    //        return db.TrongLuongCoiTheoSanPhams.Any(x =>
    //            x.Id != id && // loại trừ bản ghi đang sửa
    //            x.MaCoi == maCoi &&
    //            x.MaSanPham == maSanPham &&
    //            x.NgayGio.Date == ngayGio.Date
    //        );
    //    }
    //}
    private int Update<T>(T item)
    {
        var dao = new Dao.Repos.HQ.TrongLuongCoiTheoSanPham();

        //if (item is TrongLuongCoiTheoSanPham model)
        //{
        //    // Kiểm tra trùng nhưng bỏ qua chính bản ghi đang cập nhật
        //    bool exists = IsDuplicateInDayExceptId(model.MaCoi, model.MaSanPham, model.NgayGio, model.Id);
        //    if (exists)
        //    {
        //        return -1; // Hoặc throw exception, hoặc return mã lỗi cụ thể
        //    }
        //}

        return dao.Update(item);
    }

    //private int Update<T>(T item)
    //{
    //    var dao = new Dao.Repos.HQ.TrongLuongCoiTheoSanPham();
    //    return dao.Update(item);
    //}

    [RelayCommand(CanExecute = nameof(IsItemPass))]
    private void Update_(TrongLuongCoiTheoSanPham item)
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
    [RelayCommand()]
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
    [RelayCommand(CanExecute = nameof(IsItemPass))]
    public async Task UpdateAsync(TrongLuongCoiTheoSanPham item)
    {
        try
        {
            var result = await Task.Run(() => Update(item));

            if (result > 0)
            {
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
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            VmMessage.SetExceptionCommand.Execute(e);
        }
    }
    public TrongLuongCoiTheoSanPham? Find(long id)
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
    public async Task<TrongLuongCoiTheoSanPham?> FindAsync(long id)
    {
        try
        {
            return await Task.Run(() => Items.FirstOrDefault(x => x.Id == id));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public bool Exists(TrongLuongCoiTheoSanPham item)
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
}