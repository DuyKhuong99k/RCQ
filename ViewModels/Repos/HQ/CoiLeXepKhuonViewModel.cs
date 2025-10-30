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


public partial class CoiLeXepKhuonViewModel : ObservableObject
{
    private static CoiLeXepKhuonViewModel instance;
    [ObservableProperty] private bool idItemIsReadOnly = true;
    [ObservableProperty] private bool isAdd;
    [ObservableProperty] private bool isEdit;
    [ObservableProperty] private CoiLeXepKhuon? item;
    [ObservableProperty] private ObservableRangeCollection<CoiLeXepKhuon> items = new();
    [ObservableProperty] private ObservableRangeCollection<CoiLeXepKhuon> usedItems = new();
    [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private CoiLeXepKhuon? selectedItem;
    [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
    [ObservableProperty] private ICommand _closeItemWindowCommand;
    [ObservableProperty] private bool _isWindowItemShown = false;
    private readonly SynchronizationContext synchronizationContext;
    private CoiLeXepKhuonViewModel()
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

    public static CoiLeXepKhuonViewModel Instance => instance ??= new CoiLeXepKhuonViewModel();

    private AppViewModel VmApp => AppViewModel.Instance;
    private MessageViewModel VmMessage => MessageViewModel.Instance;

    public CoiLeXepKhuon CopyItem(CoiLeXepKhuon item)
    {
        return new CoiLeXepKhuon
        {
            Id = item.Id,
            MaCoi = item.MaCoi,
            MaSanPham = item.MaSanPham,
            TrongLuong = item.TrongLuong,
            NgayGio = item.NgayGio, NgayNguyenLieu = item.NgayNguyenLieu
        };
    }
    public CoiLeXepKhuon CopySelectedItem()
    {
        return CopyItem(SelectedItem);
    }

    public CoiLeXepKhuon CreateDefaultNew()
    {
        var maxId = Items
               .Select(x => x.Id)
               .DefaultIfEmpty(0)
               .Max();
        var id = maxId + 1;
        return new CoiLeXepKhuon
        {
            Id = id,
        };
    }

    private int Delete<T>(T item)
    {
        var dao = new Dao.Repos.HQ.CoiLeXepKhuon();
        return dao.Delete(item);
    }

    [RelayCommand(CanExecute = nameof(IsItemPass))]
    private void Delete_(CoiLeXepKhuon item)
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
    public async Task DeleteAsync(CoiLeXepKhuon item)
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
        var dao = new Dao.Repos.HQ.CoiLeXepKhuon();
        return dao.Gets<T>();
    }
    public List<T> GetsFullField<T>(DateTime dateTime, string? connStr = null)
    {
        var dao = new Dao.Repos.HQ.CoiLeXepKhuon();
        return dao.GetAllsFullField<T>(dateTime);
    }
    public bool IsDuplicateInDay(string maCoi, string maSanPham, DateTime ngayGio, DateTime ngayNguyenLieu)
    {
        using (var db = new dbPMScontext())
        {
            return db.CoiLeXepKhuons.Any(x =>
                x.MaCoi == maCoi &&
                x.MaSanPham == maSanPham &&
                x.NgayGio.Date == ngayGio.Date && x.NgayNguyenLieu.Date == ngayNguyenLieu.Date
            );
        }
    }

    private int Insert<T>(T item)
    {
        var dao = new Dao.Repos.HQ.CoiLeXepKhuon();

        //if (item is CoiLeXepKhuon model)
        //{
        //    bool exists = IsDuplicateInDay(model.MaCoi, model.MaSanPham, model.NgayGio, model.NgayNguyenLieu);
        //    if (exists)
        //    {
        //        return -1;
        //    }
        //}

        return dao.Insert(item);
    }

    [RelayCommand(CanExecute = nameof(IsItemPass))]
    private void Insert_(CoiLeXepKhuon item)
    {
        try
        {
            if (Insert(item) > 0)
            {
                var items = new List<CoiLeXepKhuon>();
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
    public async Task InsertAsync(CoiLeXepKhuon item)
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
    private bool IsItemPass(CoiLeXepKhuon item)
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

        var items = Gets<CoiLeXepKhuon>();
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
    private void Reload_(ObservableRangeCollection<CoiLeXepKhuon> obj)
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

    public bool IsDuplicateInDayExceptId(string maCoi, string maSanPham, DateTime ngayGio, int id, DateTime ngayNguyenLieu)
    {
        using (var db = new dbPMScontext())
        {
            return db.CoiLeXepKhuons.Any(x =>
                x.Id != id && // loại trừ bản ghi đang sửa
                x.MaCoi == maCoi &&
                x.MaSanPham == maSanPham &&
                x.NgayGio.Date == ngayGio.Date && x.NgayNguyenLieu.Date == ngayNguyenLieu.Date
            );
        }
    }
    private int Update<T>(T item)
    {
        var dao = new Dao.Repos.HQ.CoiLeXepKhuon();

        if (item is CoiLeXepKhuon model)
        {
            // Kiểm tra trùng nhưng bỏ qua chính bản ghi đang cập nhật
            bool exists = IsDuplicateInDayExceptId(model.MaCoi, model.MaSanPham, model.NgayGio, model.Id, model.NgayNguyenLieu);
            if (exists)
            {
                return -1; // Hoặc throw exception, hoặc return mã lỗi cụ thể
            }
        }

        return dao.Update(item);
    }

    //private int Update<T>(T item)
    //{
    //    var dao = new Dao.Repos.HQ.CoiLeXepKhuon();
    //    return dao.Update(item);
    //}

    [RelayCommand(CanExecute = nameof(IsItemPass))]
    private void Update_(CoiLeXepKhuon item)
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
    public async Task UpdateAsync(CoiLeXepKhuon item)
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
    public CoiLeXepKhuon? Find(long id)
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
    public async Task<CoiLeXepKhuon?> FindAsync(long id)
    {
        Reload();
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
    public bool Exists(CoiLeXepKhuon item)
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


