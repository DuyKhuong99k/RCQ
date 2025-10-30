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
using BravoModelV1.Model;

namespace ViewModels.Repos.HQ
{
    public partial class PhieuCanSoCheDinhHinhViewModel : ObservableObject
    {
        private static PhieuCanSoCheDinhHinhViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanSoCheDinhHinh? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanSoCheDinhHinh> items = new();

        //[ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanBTPFilletv2? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanSoCheDinhHinhViewModel()
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
        public static PhieuCanSoCheDinhHinhViewModel Instance => instance ??= new PhieuCanSoCheDinhHinhViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public List<T> GetTongHopThanhPhams<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh();
                return dao.GetPhieuCanTongHopsLoaiTP<T>(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan_XLPC<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh(connStr);
            return dao.GetPhieuCan_XLPC<T>(dateTime, xuongId);
        }
        #endregion
        #region Tính Lương
        public List<BravoModelV1.Model.PhieuCanKiemDinhHinh> GetPhieuCanSoCheTinhLuongs(DateTime dateTime,
            List<BravoModelV1.Model.SanLuongNhanVienSoChe> sanLuongNhanVienKiemSoChes,
            int khuVucId)
        {
            try
            {
                var phieuCans = new List<BravoModelV1.Model.PhieuCanKiemDinhHinh>();
                foreach (var item in sanLuongNhanVienKiemSoChes)
                {
                    phieuCans.Add(
                        new BravoModelV1.Model.PhieuCanKiemDinhHinh()
                        {
                            CaLamViec = "CT04",
                            MaNhanVien = item.MaNhanVien,
                            MaSanPham = item.MaSanPham,
                            TenSanPham = item.TenSanPham,
                            Ngay = dateTime,
                            TrongLuong = Math.Round(item.TrongLuongHuong, 2),
                            _Status = 0,
                            KhuVuc = khuVucId.ToString("00")
                        });
                }

                return phieuCans;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PhieuCanKiemDinhHinh> GetPhieuCanSoCheTinhLuongs(DateTime dateTime,
            List<SanLuongNhanVienKiemSoChe> sanLuongNhanVienKiemSoChes,
            int khuVucId)
        {
            try
            {
                var phieuCans = new List<PhieuCanKiemDinhHinh>();
                foreach (var item in sanLuongNhanVienKiemSoChes)
                {
                    phieuCans.Add(
                        new PhieuCanKiemDinhHinh()
                        {
                            CaLamViec = "CT04",
                            MaNhanVien = item.MaNhanVien,
                            MaSanPham = "SP-216",
                            TenSanPham = "Kiểm cá muối",
                            Ngay = dateTime,
                            TrongLuong = Math.Round(item.TrongLuongHuong, 2),
                            _Status = 0,
                            KhuVuc = khuVucId.ToString("00")
                        });
                }

                return phieuCans;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Insert(List<BravoModelV1.Model.PhieuCanKiemDinhHinh> phieuCans)
        {
            try
            {
                var phieuCanDao = new Dao.Repos.HQ.PhieuCanKiemDinhHinh();
                var rows = phieuCanDao.Insert(phieuCans);
                return rows;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public double GetSanLuongSoCheBatCo(
            DateTime dateTime,
            string xuongId,
            bool isBatCO, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh(connStr);
                return dao.GetSanLuongsBatCO(dateTime, xuongId, isBatCO);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuongSoChe(
            DateTime dateTime,
            string xuongId,
            bool isNguyenLieu, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh(connStr);
                return dao.GetSanLuongs(dateTime, xuongId, isNguyenLieu);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuongSoChe(
            DateTime dateTime,
            string xuongId,
            bool isNguyenLieu,
            bool isBan09, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh(connStr);
                return dao.GetSanLuongs(dateTime, xuongId, isNguyenLieu, isBan09);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuongSoChe(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            bool isNguyenLieu,
            bool isBan09,
            bool loaiGui,
            bool truocLangDa,
            bool sauLangDa, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh(connStr);
                return dao.GetSanLuongs(
                    fromTime,
                    toTime,
                    dateTime,
                    xuongId,
                    isNguyenLieu,
                    isBan09,
                    loaiGui,
                    truocLangDa,
                    sauLangDa);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuongSoChe(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            List<string> los,
            bool loaiGui,
            bool truocLangDa,
            bool sauLangDa,
            bool nhan, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh(connStr);
                return dao.GetSanLuong(fromTime, toTime, dateTime, xuongId, los, loaiGui, truocLangDa, sauLangDa, nhan);
                //return dao.GetSanLuong(fromTime, toTime, dateTime, xuongId, los, loaiGui);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Báo Cáo
        public List<T> GetPhieuCanSoCheDinhHinhs<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh();
                return dao.GetPhieuCanSoCheDinhHinhs<T>(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetPhieuCanSoCheDinhHinhByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh();
                return dao.GetPhieuCanSoCheDinhHinhByMaNhanViens<T>(fromDate, toDate, maNhanVien, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetPhieuCanSoCheDinhHinhByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh();
                return dao.GetPhieuCanSoCheDinhHinhByMaHoSos<T>(fromDate, toDate, maHoSo, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetPhieuCanSoCheDinhHinhByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh();
                return dao.GetPhieuCanSoCheDinhHinhByMaThes<T>(fromDate, toDate, maThe, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetPhieuCanTongHopsNhanVien<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh();
                return dao.GetPhieuCanTongHopsNhanVien<T>(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetPhieuCanTongHopsLoaiTP<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh();
                return dao.GetPhieuCanTongHopsLoaiTP<T>(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamsByMaNhanVien<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh();
                return dao.GetTongHopThanhPhamsByMaNhanVien<T>(fromDate, toDate, maNhanVien, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamsByMaHoSo<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh();
                return dao.GetTongHopThanhPhamsByMaHoSo<T>(fromDate, toDate, maHoSo, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamsByMaThe<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanSoCheDinhHinh();
                return dao.GetTongHopThanhPhamsByMaThe<T>(fromDate, toDate, maThe, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
