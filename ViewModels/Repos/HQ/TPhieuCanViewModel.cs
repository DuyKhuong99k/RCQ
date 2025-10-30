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
    public partial class TPhieuCanViewModel : ObservableObject
    {
        private static TPhieuCanViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private T_PhieuCan? item;
        [ObservableProperty] private ObservableRangeCollection<T_PhieuCan> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private T_PhieuCan? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private TPhieuCanViewModel()
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

        public static TPhieuCanViewModel Instance => instance ??= new TPhieuCanViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public T_PhieuCan CopyItem(T_PhieuCan item)
        {
            return new T_PhieuCan
            {
                Ma = item.Ma,
                SuDung = item.SuDung,
                GhiChu = item.GhiChu,
                MaLoaiNguyenLieu = item.MaLoaiNguyenLieu,
                DinhMuc = item.DinhMuc,
                Gio = item.Gio,
                MaGroup = item.MaGroup,
                MaKhuVuc = item.MaKhuVuc,
                MaLenhSanXuat = item.MaLenhSanXuat,
                MaLo = item.MaLo,
                MaLoaiCan = item.MaLoaiCan,
                MaLoaiKhuon = item.MaLoaiKhuon,
                MaNhaCungCap = item.MaNhaCungCap,
                MaNhanVien = item.MaNhanVien,
                MaNhomLo = item.MaNhomLo,
                MaPhuongTien = item.MaPhuongTien,
                MaQuyCach = item.MaQuyCach,
                MaSize = item.MaSize,
                MaThanhPham = item.MaThanhPham,
                MaThe = item.MaThe,
                MaXuong = item.MaXuong,
                MayCan = item.MayCan,
                Ngay = item.Ngay,
                NgayNguyenLieu = item.NgayNguyenLieu,
                STT = item.STT,
                TrongLuong = item.TrongLuong,
                TrongLuongNhan = item.TrongLuongNhan,
                TrongLuongTare = item.TrongLuongTare,
                MaNhanVien2 = item.MaNhanVien2,
                MaLoaiCongViec = item.MaLoaiCongViec,
                MaNhanVienPhucVu = item.MaNhanVienPhucVu,
                MaCongDoan = item.MaCongDoan,
                IsEnabled = item.IsEnabled,
                MaSanPham = item.MaSanPham,
                MaBon = item.MaBon,
                Luot = item.Luot,
                MaDonHang = item.MaDonHang,
                MaKhachHang = item.MaKhachHang,
                MaQuyTrinh = item.MaQuyTrinh,
                IsDatKhangSinh = item.IsDatKhangSinh,
                MaKhangSinh = item.MaKhangSinh,
                VoXo = item.VoXo,
                MaPhuGia = item.MaPhuGia,
                MaThongTinPhu = item.MaThongTinPhu,
                MaTrangThaiNguyenLieu = item.MaTrangThaiNguyenLieu,
                MaQuyTrinhOrg = item.MaQuyTrinhOrg,
                IsNgam = item.IsNgam,
                IsCanTay = item.IsCanTay,
                T = item.T,
                MaTyLeNhomHoaChat = item.MaTyLeNhomHoaChat,
                SoLuong = item.SoLuong,
                TrongLuongDonVi = item.TrongLuongDonVi,
                MaKhachHangOrg = item.MaKhachHangOrg,
                MaSizeOrg = item.MaSizeOrg,
                MaPhuGiaOrg = item.MaPhuGiaOrg,
                MaNhomHoaChat = item.MaNhomHoaChat,
                MaPhieuPhanCo = item.MaPhieuPhanCo,
                MaSizeTP = item.MaSizeTP,
            };
        }
        public T_PhieuCan CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public T_PhieuCan CreateDefaultNew()
        {
            var VmApp = AppViewModel.Instance;
            var VmXiNghiep = XiNghiepViewModel.Instance;
            var VmKhuVuc = TKhuVucViewModel.Instance;
            var VmLenhSanXuat = TLenhSanXuatViewModel.Instance;
            var VmSize = TSizeViewModel.Instance;
            var VmNhaCungCap = TNhaCungCapViewModel.Instance;
            var VmPhuongTien = TPhuongTienViewModel.Instance;
            var VmGroup = TGoupViewModel.Instance;
            var VmLoaiNguyenLieu = TLoaiNguyenLieuViewModel.Instance;
            var VmThanhPham = TThanhPhamViewModel.Instance;
            var VmLo = TLoNguyenlieuViewModel.Instance;
            var VmNhomLo = TNhomLoViewModel.Instance;
            var VmLoaiKhuon = TLoaiKhuonViewModel.Instance;
            var VmQuyCach = TQuyCachViewModel.Instance;
            var vmSanPham = TSanPhamViewModel.Instance;
            var vmCongDoan = TCongDoanViewModel.Instance;
            var vmBon = TBonViewModel.Instance;
            var vmQuyTrinh = TQuyTrinhViewModel.Instance;
            var vmKhachHang = TKhachHangViewModel.Instance;
            var vmDonHang = TDonHangViewModel.Instance;
            var vmKhangSinh = TKhangSinhViewModel.Instance;
            var vmPhuGia = TPhuGiaViewModel.Instance;
            var vmThongTinPhu = TThongTinPhuViewModel.Instance;
            var vmTrangThaiNguyenLieu = TTrangThaiNguyenLieuViewModel.Instance;
            var vmTyLeNhomHoaChat = TTyLeHoaChatTheoNhomViewModel.Instance;
            var vmNhomHoaChat = TNhomHoaChatViewModel.Instance;
            var vmPhieuPhanCo = TPhieuPhanCoViewModel.Instance;
            //var maxId = Items.Where(
            //        x => int.TryParse(x.Ma.Substring(x.Ma.Length - 6 - VmApp.T_KhuVucId.Length - 1, 6), out var rl))
            //    .Select(x => x.Ma.Substring(x.Ma.Length - 6 - VmApp.T_KhuVucId.Length - 1, 6))
            //    .DefaultIfEmpty("000000")
            //    .Max();
            var maxId = Items.Where(
                   x => int.TryParse(x.Ma.Substring(x.Ma.Length - 6), out var rl))
               .Select(x => x.Ma.Substring(x.Ma.Length - 6))
               .DefaultIfEmpty("000000")
               .Max();
            var stt = int.Parse(maxId) + 1;
            var id =
                $"{VmApp.DateTimeNow.ToString("yyyyMMdd")}.{VmXiNghiep.SelectedItem?.Ma}.{VmApp.PCName}.{stt.ToString("000000")}";/*.{ VmApp.T_KhuVucId}*/

            return new T_PhieuCan
            {
                SuDung = true,
                Ma = id,
                Ngay = VmApp.DateTimeNow,
                MayCan = VmApp.PCName,
                NgayNguyenLieu = VmApp.DateTimeNow,
                Gio = DateTime.Now.TimeOfDay,
                MaThe = "0",
                MaXuong = VmXiNghiep.SelectedItem?.Ma,
                TrongLuong = 0,
                TrongLuongNhan = 0,
                TrongLuongTare = 0,
                DinhMuc = 0,
                //MaLoaiCan = VmApp.LoaiCan,
                MaKhuVuc = VmKhuVuc.SelectedItem?.Ma,
                MaLenhSanXuat = VmLenhSanXuat.SelectedItem?.Ma ?? "0",
                MaSize = VmSize.SelectedItem?.Ma,
                MaNhaCungCap = VmNhaCungCap.SelectedItem?.Ma,
                MaPhuongTien = VmPhuongTien.SelectedItem?.Ma,
                MaGroup = VmGroup.SelectedItem?.Ma,
                MaLoaiNguyenLieu = VmLoaiNguyenLieu.SelectedItem?.Ma,
                MaThanhPham = VmThanhPham.SelectedItem?.Ma,
                MaLo = VmLo.SelectedItem?.Ma,
                MaNhomLo = VmNhomLo.SelectedItem?.Ma,
                STT = stt,
                MaLoaiKhuon = VmLoaiKhuon.SelectedItem?.Ma,
                MaQuyCach = VmQuyCach.SelectedItem?.Ma,
                MaLoaiCongViec = "TP",
                MaSanPham = vmSanPham.SelectedItem?.Ma,
                IsEnabled = true,
                MaCongDoan = vmCongDoan.SelectedItem?.Ma,
                MaBon = vmBon.SelectedItem?.Ma,
                Luot = 0,
                MaQuyTrinh = vmQuyTrinh.SelectedItem?.Ma,
                MaQuyTrinhOrg = vmQuyTrinh.SelectedItem?.Ma,
                MaDonHang = vmDonHang.SelectedItem?.Ma,
                MaKhachHang = vmKhachHang.SelectedItem?.Ma,
                //IsDatKhangSinh = VmApp.IsDatKhangSinh,
                VoXo = 0,
                MaKhangSinh = vmKhangSinh.SelectedItem?.Ma,
                MaThongTinPhu = vmThongTinPhu.SelectedItem?.Ma,
                MaPhuGia = vmPhuGia.SelectedItem?.Ma,
                MaTrangThaiNguyenLieu = vmTrangThaiNguyenLieu.SelectedItem?.Ma,
                //IsNgam = VmApp.IsNgam,
                //IsCanTay = VmApp.IsCanTay,
                //T = VmApp.Temperature,
                //GhiChu = VmApp.GhiChu,
                MaTyLeNhomHoaChat = vmTyLeNhomHoaChat.SelectedItem?.Ma,
                //SoLuong = VmApp.TSoLuong,
                TrongLuongDonVi = 1M,
                MaKhachHangOrg = vmKhachHang.SelectedItem?.Ma,
                MaSizeOrg = VmSize.SelectedItem?.Ma,
                MaPhuGiaOrg = vmPhuGia.SelectedItem?.Ma,
                MaNhomHoaChat = vmNhomHoaChat.SelectedItem?.Ma,
                MaSizeTP = VmSize.SelectedItem.Ma,
                MaPhieuPhanCo = vmPhieuPhanCo.SelectedItem?.Ma,
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(T_PhieuCan item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
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
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(T_PhieuCan item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<T_PhieuCan>();
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
        private bool IsItemPass(T_PhieuCan item)
        {
            return item != null;
        }

        public void Reload()
        {
            lock (Items)
            {
                Items.Clear();
            }

            var items = Gets<T_PhieuCan>();
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
        private void Reload_(ObservableRangeCollection<T_PhieuCan> obj)
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
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(T_PhieuCan item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
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
                        var _item = Items.SingleOrDefault(x => x.Ma == Item.Ma);
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
        public T_PhieuCan? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.Ma == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(string ma)
        {
            try
            {

                return Find(ma) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public List<T> GetPhieuCanChiTietsToNgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId)
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetChiTietsToNgayNguyenLieu<T>(fromDate, dateTime, xuongId);
        }
        public List<T> GetTongHopNhanViensToNgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId)
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetTongHopNhanViensToNgayNguyenLieu<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopNhanViens3ToNgayNguyenLieuNSRC<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isNhom,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetTongHopNhanViens3ToNgayNguyenLieuNSRC<T>(fromDate, toDate, xuongId, khuVuc, isNhom, isDinhMucBinhThuong, isFloor);
        }
        public List<T> GetTongHopNhanViens3DateTimeToDateTimeNSRC<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            bool isNhom
            )
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetTongHopNhanViens3DateTimeToDateTimeNSRC<T>(fromDate, toDate, xuongId, isNhom);
        }
        public List<T> GetAllWithFullFields<T>(DateTime toDate)
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetAllWithFullFields<T>(toDate);
        }

        public List<T> GetTongHopBaoCaoHoaChatNgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId
            )
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetTongHopBaoCaoHoaChatNgayNguyenLieu<T>(fromDate, dateTime, xuongId);
        }
        public List<T> GetTongHopBaoCaoHoaChat<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId
            )
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetTongHopBaoCaoHoaChat<T>(fromDate, dateTime, xuongId);
        }
        public List<T> GetNgayAndNguyenLieuHoaChat<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId
        )
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetNgayAndNguyenLieuHoaChat<T>(fromDate, dateTime, xuongId);
        }
        public List<T> GetNgayAndNguyenLieuHoaChat_NgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId
        )
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetNgayAndNguyenLieuHoaChat_NgayNguyenLieu<T>(fromDate, dateTime, xuongId);
        }
        public List<T> GetTongHopBaoCaoHangNgay<T>(
           DateTime fromDate,
           DateTime dateTime,
           string xuongId
           )
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetBaoCaoTongHopHangNgay<T>(fromDate, dateTime, xuongId);
        }
        //public List<T> GetPhieuCanChiTietsToNgayNguyenLieu<T>(
        //    DateTime fromDate,
        //    DateTime dateTime,
        //    string xuongId)
        //{
        //    var dao = new Dao.Repos.HQ.TPhieuCan();
        //    return dao.GetChiTietsToNgayNguyenLieu<T>(fromDate, dateTime, xuongId);
        //}
        public List<T> GetPhieuCanChiTietsDateTimeToDateTime<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true)
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetChiTietsDateTimeToDateTime<T>(fromDate, dateTime, xuongId, khuVuc, isDinhMucBinhThuong);
        }

        public List<T> GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId
            )
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieu<T>(fromDate, dateTime, xuongId);
        }
        public List<T> GetTongHopBaoCaoSauRaiMayPhanCo<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId
            )
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetTongHopBaoCaoSauRaiMayPhanCo<T>(fromDate, dateTime, xuongId);
        }
        public List<T> GetNgayAndNguyenLieuPhanCo<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId
        )
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetNgayAndNguyenLieuPhanCo<T>(fromDate, dateTime, xuongId);
        }
        public List<T> GetNgayAndNguyenLieuPhanCo_NgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId
        )
        {
            var dao = new Dao.Repos.HQ.TPhieuCan();
            return dao.GetNgayAndNguyenLieuPhanCo_NgayNguyenLieu<T>(fromDate, dateTime, xuongId);
        }
    }
}
