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
    public partial class PhieuCanXepKhuonBlockViewModel : ObservableObject
    {
        private static PhieuCanXepKhuonBlockViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanXepKhuonBlock? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanXepKhuonBlock> items = new();

        //[ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanBTPFilletv2? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanXepKhuonBlockViewModel()
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
        public static PhieuCanXepKhuonBlockViewModel Instance => instance ??= new PhieuCanXepKhuonBlockViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public List<T> GetTongHopThanhPhams<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanXepKhuonBlock();
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
            var dao = new Dao.Repos.HQ.PhieuCanXepKhuonBlock(connStr);
            return dao.GetPhieuCan_XLPC<T>(dateTime, xuongId);
        }
        #endregion
        #region Báo Cáo
        public List<T> GetPhieuCanBlocks<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanXepKhuonBlock();
                return dao.GetPhieuCanBlockXepKhuonsByDateToDate<T>(fromDate, toDate, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanBlockByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanXepKhuonBlock();
                return dao.GetPhieuCanBlockXepKhuonsByMaNhanVien<T>(fromDate, toDate, maNhanVien, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanBlockByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanXepKhuonBlock();
                return dao.GetPhieuCanBlockXepKhuonsByMaHoSo<T>(fromDate, toDate, maHoSo, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanBlockByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanXepKhuonBlock();
                return dao.GetPhieuCanBlockXepKhuonsByMaThe<T>(fromDate, toDate, maThe, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanTongHopBlock<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanXepKhuonBlock();
                return dao.GetTongHopPhieuCanBlockXepKhuonsByDateToDate<T>(fromDate, toDate, xuongId);
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
                var dao = new Dao.Repos.HQ.PhieuCanXepKhuonBlock();
                return dao.GetTongHopThanhPhamByMaNhanVien<T>(fromDate, toDate, maNhanVien, xuongId);
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
                var dao = new Dao.Repos.HQ.PhieuCanXepKhuonBlock();
                return dao.GetTongHopThanhPhamByMaHoSo<T>(fromDate, toDate, maHoSo, xuongId);
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
                var dao = new Dao.Repos.HQ.PhieuCanXepKhuonBlock();
                return dao.GetTongHopThanhPhamByMaThe<T>(fromDate, toDate, maThe, xuongId);
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
