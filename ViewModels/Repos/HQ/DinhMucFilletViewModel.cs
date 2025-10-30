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
    public partial class DinhMucFilletViewModel : ObservableObject
    {
        private static DinhMucFilletViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private DinhMucFillet? item;
        [ObservableProperty] private ObservableRangeCollection<DinhMucFillet> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private DinhMucFillet? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private DinhMucFilletViewModel()
        {
            try
            {
                ReLoadLast(DateTime.Now);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        public static DinhMucFilletViewModel Instance => instance ??= new DinhMucFilletViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public DinhMucFillet CopyItem(DinhMucFillet item)
        {
            return new DinhMucFillet
            {
                STT = item.STT,
                CaTra = item.CaTra,
                DinhMuc = item.DinhMuc,
                Gio = item.Gio,
                MaLo = item.MaLo,
                MaLoaiCa = item.MaLoaiCa,
                MaMau = item.MaMau,
                MaSize = item.MaSize,
                MaThanhPham = item.MaThanhPham,
                MaXuong = item.MaXuong,
                Ngay = item.Ngay,
                SuDung = item.SuDung,
            };
        }
        public DinhMucFillet CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public DinhMucFillet CreateDefaultNew(DateTime dateTime, string xuongId)
        {
            var maxId = Items.Where(x => x.Ngay.Date == dateTime.Date && x.MaXuong == xuongId)
                    .Select(x => x.STT)
                    .DefaultIfEmpty(0)
                    .Max();
            var id = maxId + 1;
            return new DinhMucFillet()
            {
                STT = id,
                SuDung = true,
                CaTra = false,
                Ngay = dateTime.Date,
                MaXuong = xuongId,
                //MaLoaiCa =
                //    PMSMSharedv1.ViewModel.LoaiCaDinhHinhViewModel.Ins.Items
                //        .Where(x => x.SuDung == true)
                //        .FirstOrDefault()?.Ma,
                //MaMau =
                //    PMSMSharedv1.ViewModel.MauDinhHinhViewModel.Ins.Items
                //        .Where(x => x.SuDung == true)
                //        .FirstOrDefault()?.Ma,
                //MaSize =
                //    PMSMSharedv1.ViewModel.SizeDinhHinhViewModel.Ins.Items
                //        .Where(x => x.SuDung == true)
                //        .FirstOrDefault()?.Ma,
                //MaThanhPham =
                //    PMSMSharedv1.ViewModel.ThanhPhamDinhHinhViewModel.Ins.Items
                //        .Where(x => x.SuDung == true)
                //        .FirstOrDefault()?.Ma,
                DinhMuc = 0,
                Gio = DateTime.Now.TimeOfDay
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.DinhMucFillet();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(DinhMucFillet item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.STT == item.STT);
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

        private List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.DinhMucFillet();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.DinhMucFillet();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(DinhMucFillet item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<DinhMucFillet>();
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
        public bool IsVailSelectedItem => SelectedItem != null;
        private bool IsItemPass(DinhMucFillet item)
        {
            return item != null && item.MaXuong != null && item.MaXuong.Trim() != "" &&
                   item.SuDung != null;
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
        public List<T> GetsLast<T>(DateTime dateTime)
        {
            var dao = new Dao.Repos.HQ.DinhMucFillet();
            return dao.GetsLast<T>(dateTime);
        }
        public void ReLoadLast(DateTime dateTime)
        {
            lock (Items)
            {
                Items.Clear();
            }

            var items = GetsLast<DinhMucFillet>(dateTime);
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
        public void Reload()
        {
            lock (Items)
            {
                Items.Clear();
            }

            var items = Gets<DinhMucFillet>();
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
        private void Reload_(ObservableRangeCollection<DinhMucFillet> obj)
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
            var dao = new Dao.Repos.HQ.DinhMucFillet();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(DinhMucFillet item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.STT == item.STT);
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
                        var _item = Items.SingleOrDefault(x => x.STT == Item.STT);
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
        public DinhMucFillet? Find(int stt)
        {
            try
            {
                return Items.FirstOrDefault(x => x.STT == stt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(DinhMucFillet item)
        {
            try
            {

                return Find(item.STT) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public List<T> GetsFullField<T>(string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.DinhMucFillet(connStr);
            return dao.GetsFullField<T>();
        }
    }
}
