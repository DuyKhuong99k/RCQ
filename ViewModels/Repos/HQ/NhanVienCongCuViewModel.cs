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
    public partial class NhanVienCongCuViewModel : ObservableObject
    {
        private static NhanVienCongCuViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private NhanVienCongCu? item;
        [ObservableProperty] private ObservableRangeCollection<NhanVienCongCu> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private NhanVienCongCu? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private NhanVienCongCuViewModel()
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

        public static NhanVienCongCuViewModel Instance => instance ??= new NhanVienCongCuViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public NhanVienCongCu CopyItem(NhanVienCongCu item)
        {
            return new NhanVienCongCu
            {
                Gio = item.Gio,
                MaChieuXaXepKhuonChinh = item.MaChieuXaXepKhuonChinh,
                MaKhachHangXepKhuonKXL = item.MaKhachHangXepKhuonKXL,
                MaLo = item.MaLo,
                MaNhanVien = item.MaNhanVien,
                MaSizeXepKhuonChinh = item.MaSizeXepKhuonChinh,
                MaSizeXepKhuonKXL = item.MaSizeXepKhuonKXL,
                MaThanhPhamXepKhuonChinh = item.MaThanhPhamXepKhuonChinh,
                MaThanhPhamXepKhuonKXL = item.MaThanhPhamXepKhuonKXL,
                Ngay = item.Ngay,
                TabName = item.TabName,
                MaXuong = item.MaXuong,
                
            };
        }
        public NhanVienCongCu CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public NhanVienCongCu CreateDefaultNew()
        {
           
            return new NhanVienCongCu
            {
                Gio = DateTime.Now.TimeOfDay,
                Ngay = AppViewModel.Instance.DateTimeNow.Date,
                MaXuong = XiNghiepViewModel.Instance.SelectedItem?.Ma??""
            };
        }

        public int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.NhanVienCongCu();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(NhanVienCongCu item)
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

        private List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.NhanVienCongCu();
            return dao.Gets<T>();
        }
        private List<T> Gets<T>(DateTime dateTime)
        {
            var dao = new Dao.Repos.HQ.NhanVienCongCu();
            return dao.Gets<T>(dateTime);
        }
        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.NhanVienCongCu();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(NhanVienCongCu item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<NhanVienCongCu>();
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
        private bool IsItemPass(NhanVienCongCu item)
        {
            return item.MaNhanVien != null &&
                 item.MaNhanVien.Trim() != "" &&
                 item.Ngay != null &&
                 item.MaLo != null &&
                 item.MaLo.Trim() != "" &&
                 item.TabName != null &&
                 item.TabName.Trim() != "" && Item.MaXuong != null && Item.MaXuong.Trim() != "";
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

            var items = Gets<NhanVienCongCu>(DateTime.Now);
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
        private void Reload_(ObservableRangeCollection<NhanVienCongCu> obj)
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
            var dao = new Dao.Repos.HQ.NhanVienCongCu();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(NhanVienCongCu item)
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
        public NhanVienCongCu? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.MaNhanVien == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(NhanVienCongCu item)
        {
            try
            {

                return Find(item.MaNhanVien) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
