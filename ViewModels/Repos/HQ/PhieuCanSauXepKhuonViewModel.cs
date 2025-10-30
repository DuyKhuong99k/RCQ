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
    public partial class PhieuCanSauXepKhuonViewModel : ObservableObject
    {
        private static PhieuCanSauXepKhuonViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanSauXepKhuon? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanSauXepKhuon> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanSauXepKhuon? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanSauXepKhuonViewModel()
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

        public static PhieuCanSauXepKhuonViewModel Instance => instance ??= new PhieuCanSauXepKhuonViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public PhieuCanSauXepKhuon CopyItem(PhieuCanSauXepKhuon item)
        {
            return new PhieuCanSauXepKhuon
            {
                STT = item.STT,
                Ngay = item.Ngay,
                NgayNguyenLieu = item.NgayNguyenLieu,
                Gio = item.Gio,
                MaXuong = item.MaXuong,
                MaMayCan = item.MaMayCan,
                MaLo = item.MaLo,
                MaLoaiCa = item.MaLoaiCa,
                MaThanhPham = item.MaThanhPham,
                MaSize = item.MaSize,
                MaChieuXa = item.MaChieuXa,
                MaCoi = item.MaCoi,
                TrongLuong = item.TrongLuong,
                TrongLuongTare = item.TrongLuongTare,
                ChiTietLuotRaCoiId = item.ChiTietLuotRaCoiId,
                MaNhanVien = item.MaNhanVien,
                MaUserCan = item.MaUserCan,
                GhiChu = item.GhiChu,
                MaThe = item.MaThe
            };
        }
        public PhieuCanSauXepKhuon CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public PhieuCanSauXepKhuon CreateDefaultNew()
        {
            var stt = 0;
            if (Items.Any())
                stt = Items.Select(x => Math.Abs(x.STT)).DefaultIfEmpty(0).Max() + 1;
            else
                stt = 1;

            return new PhieuCanSauXepKhuon
            {
                STT = stt,
                Ngay = DateTime.Now,
                NgayNguyenLieu = DateTime.Now,
                Gio = DateTime.Now.TimeOfDay,
                //MaXuong = AppViewModel.Instance.XuongId,
                MaMayCan = AppViewModel.Instance.PCName,
                // = LoViewModel.Instance.ItemsWithSize?.FirstOrDefault()?.Item2,
                MaLoaiCa = LoaiCaXepKhuonViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaThanhPham = MaThanhPhamChinhXepKhuonViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaSize = MaSizeChinhXepKhuonViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaChieuXa = MaSizeChinhXepKhuonViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaCoi = MaChieuXaXepKhuonViewModel.Instance.Items.FirstOrDefault()?.Ma,
                TrongLuong = 0,
                TrongLuongTare = 0,
                //khác kiểu dữ liệu
               // ChiTietLuotRaCoiId = ChiTietRaCoiViewModel.Instance.Items.FirstOrDefault().Id,
                //MaNhanVien = SelectedItem.MaNhanVien,
                //MaUserCan = AppViewModel.Instance.UserName,
                GhiChu = "",
                MaThe = "0"
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanSauXepKhuon();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(PhieuCanSauXepKhuon item)
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
            var dao = new Dao.Repos.HQ.PhieuCanSauXepKhuon();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanSauXepKhuon();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(PhieuCanSauXepKhuon item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<PhieuCanSauXepKhuon>();
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
        private bool IsItemPass(PhieuCanSauXepKhuon item)
        {
            return item != null &&
                item.STT > 0 &&
                item.Ngay != null &&
                item.NgayNguyenLieu != null &&
                item.Gio != null &&
                item.MaXuong != null &&
                item.MaXuong.Trim() != string.Empty &&
                item.MaMayCan != null &&
                item.MaMayCan.Trim() != string.Empty &&
                item.MaLo != null &&
                item.MaLo.Trim() != string.Empty &&
                item.MaLoaiCa != null &&
                item.MaLoaiCa.Trim() != string.Empty &&
                item.MaThanhPham != null &&
                item.MaThanhPham.Trim() != string.Empty &&
                item.MaSize != null &&
                item.MaSize.Trim() != string.Empty &&
                item.MaChieuXa != null &&
                item.MaChieuXa.Trim() != string.Empty &&
                item.MaCoi != null &&
                item.MaCoi.Trim() != string.Empty &&
                item.TrongLuong > 0 &&
                item.ChiTietLuotRaCoiId > 0 &&
                item.MaNhanVien != null &&
                item.MaNhanVien.Trim() != string.Empty;
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

            var items = Gets<PhieuCanSauXepKhuon>();
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
        private void Reload_(ObservableRangeCollection<PhieuCanSauXepKhuon> obj)
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
            var dao = new Dao.Repos.HQ.PhieuCanSauXepKhuon();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(PhieuCanSauXepKhuon item)
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
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan_XLPC<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanSauXepKhuon(connStr);
            return dao.GetPhieuCan_XLPC<T>(dateTime, xuongId);
        }
        #endregion
    }
}
