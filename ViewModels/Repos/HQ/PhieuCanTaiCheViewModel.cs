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
    public partial class PhieuCanTaiCheViewModel : ObservableObject
    {
        private static PhieuCanTaiCheViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanTaiChe? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanTaiChe> items = new();

        //[ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanBTPFilletv2? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanTaiCheViewModel()
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
        public static PhieuCanTaiCheViewModel Instance => instance ??= new PhieuCanTaiCheViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public List<T> GetTongHopThanhPhams<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanTaiChe();
                return dao.GetPhieuCanTongHopThanhPhams<T>(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan_XLPC<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTaiChe(connStr);
            return dao.GetPhieuCan_XLPC<T>(dateTime, xuongId);
        }
        #endregion
        #region báo cáo
        public List<T> GetPhieuCanRaDongXepKhuonsByDateToDate<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanRaDong();
                return dao.GetPhieuCanRaDongXepKhuonsByDateToDate<T>(fromDate, toDate, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanTaiChes<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanTaiChe();
                return dao.GetPhieuCanTaiCheXepKhuonsByDateToDate<T>(fromDate, toDate, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanTaiCheByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanTaiChe();
                return dao.GetPhieuCanTaiCheXepKhuonsByMaNhanVien<T>(fromDate, toDate, maNhanVien, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanTaiCheByMaHoSo<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanTaiChe();
                return dao.GetPhieuCanTaiCheXepKhuonsByMaHoSo<T>(fromDate, toDate, maHoSo, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanTaiCheByMaThe<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanTaiChe();
                return dao.GetPhieuCanTaiCheXepKhuonsByMaThe<T>(fromDate, toDate, maThe, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanTongHopTaiChe<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanTaiChe();
                return dao.GetPhieuCanTongHopTaiCheXepKhuonsByDateToDate<T>(fromDate, toDate, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanTaiChe();
                return dao.GetTongHopThanhPhamByMaNhanViens<T>(fromDate, toDate, maNhanVien, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanTaiChe();
                return dao.GetTongHopThanhPhamByMaHoSos<T>(fromDate, toDate, maHoSo, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanTaiChe();
                return dao.GetTongHopThanhPhamByMaThes<T>(fromDate, toDate, maThe, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        #endregion
    }
}
