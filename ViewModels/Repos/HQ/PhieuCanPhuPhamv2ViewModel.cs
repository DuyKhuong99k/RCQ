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
    public partial class PhieuCanPhuPhamv2ViewModel : ObservableObject
    {
        private static PhieuCanPhuPhamv2ViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanPhuPhamv2? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanPhuPhamv2> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanPhuPhamv2? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanPhuPhamv2ViewModel()
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

        public static PhieuCanPhuPhamv2ViewModel Instance => instance ??= new PhieuCanPhuPhamv2ViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public PhieuCanPhuPhamv2 CopyItem(PhieuCanPhuPhamv2 item)
        {
            return new PhieuCanPhuPhamv2
            {
                GhiChu = item.GhiChu,
                Gio = item.Gio,
                MaNhanVien = item.MaNhanVien,
                MaThe = item.MaThe,
                MaXuong = item.MaXuong,
                Ngay = item.Ngay,
                STT = item.STT,
                SuDung = item.SuDung,
                TrongLuong = item.TrongLuong,
                MaLo = item.MaLo,
                MaMayCan = item.MaMayCan,
                MaThanhPham = item.MaThanhPham,
            };
        }
        public PhieuCanPhuPhamv2 CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public PhieuCanPhuPhamv2 CreateDefaultNew()
        {
            var stt = 0;
            if (Items.Any())
            {
                stt = Items.Select(x => Math.Abs(x.STT)).DefaultIfEmpty(0).Max() + 1;
            }

            return new PhieuCanPhuPhamv2()
            {
                STT = stt,
                SuDung = true,
                Ngay = AppViewModel.Instance.DateTimeNow,
                Gio = DateTime.Now.TimeOfDay,
                MaXuong = XiNghiepViewModel.Instance.SelectedItem?.Ma,
                MaThe = ".",
                MaLo = "001",
                MaMayCan = AppViewModel.Instance.PCName,
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(PhieuCanPhuPhamv2 item)
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
            var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(PhieuCanPhuPhamv2 item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<PhieuCanPhuPhamv2>();
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
        private bool IsItemPass(PhieuCanPhuPhamv2 item)
        {
            return item != null &&
                 item.STT > 0 &&
                 item.Ngay != null &&
                 item.Gio != null &&
                 item.MaThanhPham != null &&
                 item.MaThanhPham.Trim() != "" &&
                 item.MaLo != null &&
                 item.MaLo.Trim() != "" &&
                 item.MaMayCan != null &&
                 item.MaMayCan.Trim() != "" &&
                 item.TrongLuong >= 0 &&
                 item.MaNhanVien != null &&
                 item.MaNhanVien.Trim() != string.Empty &&
                 item.MaThe != null &&
                 item.MaXuong != null &&
                 item.MaXuong.Trim() != string.Empty;
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

            var items = Gets<PhieuCanPhuPhamv2>();
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
        private void Reload_(ObservableRangeCollection<PhieuCanPhuPhamv2> obj)
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
            var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2();
            return dao.Update(item);
        }

        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2();
            return dao.GetsLast<T>(dateTime, num);
        }
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(PhieuCanPhuPhamv2 item)
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
            var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2(connStr);
            return dao.GetPhieuCan_XLPC<T>(dateTime, xuongId);
        }
        #endregion
        #region Tính Lương Phụ Phẩm v2
        public List<T> GetSanLuongTinhLuong<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2(connStr);
                return dao.GetSanLuongTinhLuong<T>(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<object> CommandReloadTinhLuong(DateTime dateTime, string xuongId)
        {
            try
            {
                var phieuCanTinhLuongs = new List<object>();
                var items = GetSanLuongTinhLuong<object>(dateTime, xuongId);
                var _items = items.Cast<dynamic>().ToList();
                var donGias = DG_DonGiaViewModel.Instance.Gets<DG_DonGia>(dateTime, @"SP", true);
                var itemtonghoptinhluongs = (from item in _items
                                             join dg in donGias on new { MaSanPham = (string)item.MaSanPham } equals new
                                             {
                                                 dg.MaSanPham
                                             } into gj
                                             from jItem in gj.DefaultIfEmpty()
                                             select new
                                             {
                                                 CaLamViec = item.CaLamViec,
                                                 KhuVuc = item.KhuVuc,
                                                 MaNhanVien = item.MaNhanVien,
                                                 MaSanPham = item.MaSanPham,
                                                 Ngay = item.Ngay,
                                                 TenSanPham = item.TenSanPham,
                                                 TrongLuong = item.TrongLuong,
                                                 _Status = item._Status,
                                                 SoRo = item.SoRo,
                                                 TenNhanVien = item.TenNhanVien,
                                                 MaHoSo = item.MaHoSo,
                                                 DonGia = jItem?.DonGia ?? 0,
                                                 ThanhTien = item.TrongLuong * (jItem?.DonGia ?? 0) * (jItem?.HeSo ?? 1),
                                                 IsChamCong =
                                                     BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime)
                                                         .FirstOrDefault(x => x == item.MaNhanVien) ==
                                                     null
                                                         ? false
                                                         : true
                                             }).ToList();
                phieuCanTinhLuongs.AddRange(itemtonghoptinhluongs);
                return phieuCanTinhLuongs.ToList();
            }
            catch (Exception ex)
            {
                return new List<object>();
                //MessageBox.Show(ex.Message);
                //throw;
            }
        }

        #endregion
        #region Báo Cáo
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2(connStr);
            return dao.GetChiTiets<T>(fromDate, toDate);
        }

        public List<T> GetChiTietByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2(connStr);
            return dao.GetChiTietByMaNhanViens<T>(fromDate, toDate, maNhanVien, xuongId);
        }
        public List<T> GetChiTietByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2(connStr);
            return dao.GetChiTietByMaHoSos<T>(fromDate, toDate, maHoSo, xuongId);
        }

        public List<T> GetChiTietByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2(connStr);
            return dao.GetChiTietByMaThes<T>(fromDate, toDate, maThe, xuongId);
        }



        public List<T> GetTongHops<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2(connStr);
            return dao.GetTongHops<T>(fromDate, toDate);
        }
        public List<T> GetTongHopThanhPhamByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2(connStr);
            return dao.GetTongHopThanhPhamTheoNhanVienByMaNhanViens<T>(fromDate, toDate, maNhanVien, xuongId);
        }
        public List<T> GetTongHopThanhPhamByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2(connStr);
            return dao.GetTongHopThanhPhamTheoNhanVienByMaThes<T>(fromDate, toDate, maThe, xuongId);
        }
        public List<T> GetTongHopThanhPhamTheoByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuPhamv2(connStr);
            return dao.GetTongHopThanhPhamTheoNhanVienByMaHoSos<T>(fromDate, toDate, maHoSo, xuongId);
        }
        #endregion
    }
}
