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

namespace ViewModels.Repos.HQ
{
    public partial class LogKetChuyenViewModel : ObservableObject
    {
        private static LogKetChuyenViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private LogKetChuyenBravo? item;
        [ObservableProperty] private ObservableRangeCollection<LogKetChuyenBravo> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private LogKetChuyenBravo? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private LogKetChuyenViewModel()
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

        public static LogKetChuyenViewModel Instance => instance ??= new LogKetChuyenViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public LogKetChuyenBravo CopyItem(LogKetChuyenBravo item)
        {
            return new LogKetChuyenBravo
            {
                Ngay = item.Ngay,
                MaXuong = item.MaXuong,
                Gio = item.Gio,
                MaSanPham = item.MaSanPham,
                NgayChuyen = item.NgayChuyen,
                tab = item.tab
            };
        }
        public LogKetChuyenBravo CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public LogKetChuyenBravo CreateDefaultNew()
        {
            //var maxId = Items.Where(x => int.TryParse(x.Ma, out var rl))
            //       .Select(x => int.Parse(x.Ma))
            //       .DefaultIfEmpty(0)
            //       .Max();
            //var id = $"{(maxId + 1).ToString()}";
            return new LogKetChuyenBravo
            {
                //SuDung = true,
                //Ma = id
            };
        }

        
        public int Delete(DateTime dateTime, int tab)
        {
            try
            {
                var dao = new Dao.Repos.HQ.LogKetChuyenBravo();
                return dao.Delete(dateTime, tab);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Delete_(LogKetChuyenBravo item)
        //{
        //    try
        //    {
        //        if (Delete(item) > 0)
        //            lock (Items)
        //            {
        //                var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
        //                if (_item != null)
        //                {
        //                    var index = Items.IndexOf(_item);
        //                    Items.RemoveAt(index);
        //                    //Items.Insert(index,item);
        //                }
        //            }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}

        private List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.TKhachHang();
            return dao.Gets<T>();
        }

       

        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Insert_(LogKetChuyenBravo item)
        //{
        //    try
        //    {
        //        if (Insert(item) > 0)
        //        {
        //            var items = new List<LogKetChuyenBravo>();
        //            lock (Items)
        //            {
        //                Items.Add(item);
        //            }
        //            VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
        //            IsWindowItemShown = false;
        //        }
        //        else
        //        {
        //            VmMessage.MessageBoxShow("Không thể thêm", "Thông Báo", 0);
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}

        //[RelayCommand()]
        //private void Insert2_()
        //{
        //    try
        //    {
        //        if (Insert(Item) > 0)
        //        {
        //            Items.Insert(0, Item);
        //            VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        VmMessage.SetExceptionCommand.Execute(e);
        //        //throw;
        //    }
        //}
        public bool IsVailSelectedItem => SelectedItem != null;
        //private bool IsItemPass(LogKetChuyenBravo item)
        //{
        //    return item != null && item.Ten != null && item.Ten.Trim() != "" &&
        //           item.SuDung != null;
        //}
        //[RelayCommand]
        //private void ForceRaseCanExcute()
        //{
        //    try
        //    {
        //        //Insert_Command.NotifyCanExecuteChanged();
        //        //Delete_Command.NotifyCanExecuteChanged();
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

            var items = Gets<LogKetChuyenBravo>();
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
        private void Reload_(ObservableRangeCollection<LogKetChuyenBravo> obj)
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

        private int Update<T>(T item)
        {
            var dao = new Dao.Repos.HQ.TKhachHang();
            return dao.Update(item);
        }
        public LogKetChuyenBravo Get(DateTime dateTime, string xuongId, string sanPhamId, int tab, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.LogKetChuyenBravo(connStr);
                return dao.Get(dateTime, xuongId, sanPhamId, tab);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert(LogKetChuyenBravo logKetChuyenBravo, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.LogKetChuyenBravo(connStr);
                return dao.Insert(logKetChuyenBravo);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert<T>(T item)
        {
            try
            {
                var dao = new Dao.Repos.HQ.LogKiemSoatKetChuyen();
                return dao.Insert(item);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Update_(LogKetChuyenBravo item)
        //{
        //    try
        //    {
        //        if (Update(item) > 0)
        //            lock (Items)
        //            {
        //                var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
        //                if (_item != null)
        //                {
        //                    var index = Items.IndexOf(_item);
        //                    Items.RemoveAt(index);
        //                    Items.Insert(index, item);
        //                }
        //            }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}
        //[RelayCommand()]
        //private void Update2_()
        //{
        //    try
        //    {
        //        if (Update(Item) > 0)
        //            lock (Items)
        //            {
        //                var _item = Items.SingleOrDefault(x => x.Ma == Item.Ma);
        //                if (_item != null)
        //                {
        //                    var index = Items.IndexOf(_item);
        //                    Items.RemoveAt(index);
        //                    Items.Insert(index, Item);
        //                    SelectedItem = Item;
        //                }
        //            }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}
    }
}
