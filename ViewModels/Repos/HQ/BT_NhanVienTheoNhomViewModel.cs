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
    public partial class BT_NhanVienTheoNhomViewModel : ObservableObject
    {
        private static BT_NhanVienTheoNhomViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private BT_NhanVienTheoNhom? item;
        [ObservableProperty] private ObservableRangeCollection<BT_NhanVienTheoNhom> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private BT_NhanVienTheoNhom? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private BT_NhanVienTheoNhomViewModel()
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

        public static BT_NhanVienTheoNhomViewModel Instance => instance ??= new BT_NhanVienTheoNhomViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public BT_NhanVienTheoNhom CopyItem(BT_NhanVienTheoNhom item)
        {
            return new BT_NhanVienTheoNhom
            {
                MaNhanVien = item?.MaNhanVien,
                MaNhom = item?.MaNhom,
                Ngay = item.Ngay,
                TyLeTru = item.TyLeTru,
                TyLeHuong = item.TyLeHuong,
                SoGio = item.SoGio,
                MaXuong = item.MaXuong,
                MaNhomCongViec = item.MaNhomCongViec,
                MaCongViec = item.MaCongViec,
            };
        }
        public BT_NhanVienTheoNhom CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public BT_NhanVienTheoNhom CreateDefaultNew()
        {
            //var maxId = Items.Where(x => int.TryParse(x.Ma, out var rl))
            //       .Select(x => int.Parse(x.Ma))
            //       .DefaultIfEmpty(0)
            //       .Max();
            //var id = $"{(maxId + 1).ToString()}";
            return new BT_NhanVienTheoNhom
            {
                //MaNhanVien = nhanVien.MaNhanVien,
                //MaNhom = NhanVienViewModel.Ins.NhomSelectItem?.MaNhanVien,
                //Ngay = VmApp.DateTimeNow,
                //TyLeHuong = 1,
                //TyLeTru = 0,
                //SoGio = 8,
                //MaXuong = XiNghiepViewModel.Ins.XiNghiepSelectedItem?.Ma
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.BT_NhanVienTheoNhom();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(BT_NhanVienTheoNhom item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
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

        public BT_NhanVienTheoNhom? Find(BT_NhanVienTheoNhom item)
        {
            return Items.FirstOrDefault(x =>
                x.MaNhanVien ==item.MaNhanVien && x.MaNhom == item.MaNhom && x.Ngay.Date == item.Ngay.Date && x.MaXuong == item.MaXuong);
        }

        public bool Exists(BT_NhanVienTheoNhom item)
        {
            return Find(item) != null;
        }
        private List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.BT_NhanVienTheoNhom();
            return dao.Gets<T>();
        }
       
        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.BT_NhanVienTheoNhom();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(BT_NhanVienTheoNhom item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<BT_NhanVienTheoNhom>();
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
        private bool IsItemPass(BT_NhanVienTheoNhom item)
        {
            return item != null && item.MaNhanVien != null && item.MaNhanVien.Trim() != "" &&
                   item.Ngay != null;
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
            }

            var items = Gets<BT_NhanVienTheoNhom>();
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
        private void Reload_(ObservableRangeCollection<BT_NhanVienTheoNhom> obj)
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
            var dao = new Dao.Repos.HQ.BT_NhanVienTheoNhom();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(BT_NhanVienTheoNhom item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaNhanVien == item.MaNhanVien);
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
                        var _item = Items.SingleOrDefault(x => x.MaNhanVien == Item.MaNhanVien);
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

        private List<T> Gets<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BT_NhanVienTheoNhom();
                return dao.Gets<T>(dateTime,xuongId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public void Reload(DateTime dateTime,string xuongId)
        {
            lock (Items)
            {
                Items.Clear();
            }

            var items = Gets<BT_NhanVienTheoNhom>(dateTime,xuongId);
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
        
        public BT_NhanVienTheoNhom? Get(DateTime dateTime, string nhanVienId, string nhomId, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BT_NhanVienTheoNhom();
                return dao.Get<BT_NhanVienTheoNhom>(dateTime, nhanVienId, nhomId, xuongId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<BT_NhanVienTheoNhom> Gets(DateTime dateTime)
        {
            try
            {

                var dao = new Dao.Repos.HQ.BT_NhanVienTheoNhom();
                return dao.Gets<BT_NhanVienTheoNhom>(dateTime);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
