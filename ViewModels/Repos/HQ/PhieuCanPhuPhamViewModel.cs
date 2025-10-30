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
    public partial class PhieuCanPhuPhamViewModel : ObservableObject
    {
        private static PhieuCanPhuPhamViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanPhuPham? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanPhuPham> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanPhuPham? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanPhuPhamViewModel()
        {
            try
            {
                // Reload();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        public static PhieuCanPhuPhamViewModel Instance => instance ??= new PhieuCanPhuPhamViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public PhieuCanPhuPham CopyItem(PhieuCanPhuPham item)
        {
            return new PhieuCanPhuPham
            {
                GhiChu = item.GhiChu,
                MaLoaiCa = item.MaLoaiCa,
                MaLoaiThanhPham = item.MaLoaiThanhPham,
                MaMau = item.MaMau,
                MaMayTinhCan = item.MaMayTinhCan,
                MaPhuongTien = item.MaPhuongTien,
                MaSize = item.MaSize,
                MaUserCan = item.MaUserCan,
                MaXuongSanXuat = item.MaXuongSanXuat,
                MSL = item.MSL,
                Ngay = item.Ngay,
                NhaMuaHang = item.NhaMuaHang,
                SuDung = item.SuDung,
                ThoiGianCan = item.ThoiGianCan,
                TrongLuong = item.TrongLuong,
                NgayCan = item.NgayCan
            };
        }
        public PhieuCanPhuPham CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public PhieuCanPhuPham CreateDefaultNew()
        {
            return new PhieuCanPhuPham
            {
                MaMayTinhCan = AppViewModel.Instance.PCName,
                //MaUserCan = AppViewModel.Instance.UserName,
                MaXuongSanXuat = XiNghiepViewModel.Instance.SelectedItem?.Ma,
                SuDung = true,
                ThoiGianCan =
                    new DateTime(
                        AppViewModel.Instance.DateTimeNow.Year,
                        AppViewModel.Instance.DateTimeNow.Month,
                        AppViewModel.Instance.DateTimeNow.Day,
                        DateTime.Now.TimeOfDay.Hours,
                        DateTime.Now.TimeOfDay.Minutes,
                        DateTime.Now.TimeOfDay.Seconds),
                Ngay = AppViewModel.Instance.DateTimeNow,
                NgayCan =
                    new DateTime(
                        AppViewModel.Instance.DateTimeNow.Year,
                        AppViewModel.Instance.DateTimeNow.Month,
                        AppViewModel.Instance.DateTimeNow.Day,
                        DateTime.Now.TimeOfDay.Hours,
                        DateTime.Now.TimeOfDay.Minutes,
                        DateTime.Now.TimeOfDay.Seconds)
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPham();
            return dao.Delete(item);
        }

        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Delete_(PhieuCanPhuPham item)
        //{
        //    try
        //    {
        //        if (Delete(item) > 0)
        //            lock (Items)
        //            {
        //                var _item = Items.SingleOrDefault(x => x.S == item.Ma);
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
            var dao = new Dao.Repos.HQ.PhieuCanPhuPham();
            return dao.Gets<T>();
        }
        public List<T> Gets<T>(DateTime dateTime)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPham();
            return dao.Gets<T>(dateTime);
        }
        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPham();
            return dao.Insert(item);
        }

        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPham();
            return dao.GetsLast<T>(dateTime, num);
        }
        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Insert_(PhieuCanPhuPham item)
        //{
        //    try
        //    {
        //        if (Insert(item) > 0)
        //        {
        //            var items = new List<PhieuCanPhuPham>();
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
        //private bool IsItemPass(PhieuCanPhuPham item)
        //{
        //    return item != null && item.Ten != null && item.Ten.Trim() != "" &&
        //           item.SuDung != null;
        //}
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

            var items = Gets<PhieuCanPhuPham>();
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
        private void Reload_(ObservableRangeCollection<PhieuCanPhuPham> obj)
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
            var dao = new Dao.Repos.HQ.PhieuCanPhuPham();
            return dao.Update(item);
        }

        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Update_(PhieuCanPhuPham item)
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
        public List<T> GetTongHopThanhPhams<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanPhuPham();
                return dao.GetTongHopThanhPham<T>(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan_XLPC<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPham(connStr);
            return dao.GetPhieuCan_XLPC<T>(dateTime, xuongId);
        }
        #endregion
        #region Tính Lương Phụ Fillet
        public double GetSanLuongPhuPham(DateTime dateTime, string xuongId, string thanhPhamId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanPhuPham(connStr);
                return dao.GetSanLuong(dateTime, xuongId, thanhPhamId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Báo Cáo
        public List<T> GetPhieuCanChiTiets<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPham(connStr);
            return dao.GetChiTiets<T>(fromDate, toDate);
        }
        public List<T> GetPhieuCanTongHopNhaMuaHangs<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPham(connStr);
            return dao.GetTongHopNhaMuaHang<T>(fromDate, toDate);
        }
        public List<T> GetPhieuCanTongHopThanhPhams<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPham(connStr);
            return dao.GetTongHopThanhPham<T>(fromDate, toDate);
        }
        #endregion
    }
}
