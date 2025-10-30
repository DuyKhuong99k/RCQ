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
    public partial class BT_PhieuCanViewModel : ObservableObject
    {
        private static BT_PhieuCanViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private BT_PhieuCan? item;
        [ObservableProperty] private ObservableRangeCollection<BT_PhieuCan> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private BT_PhieuCan? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private BT_PhieuCanViewModel()
        {
            try
            {
                //Reload();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        public static BT_PhieuCanViewModel Instance => instance ??= new BT_PhieuCanViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;
        public bool IsVailSelectedItem => SelectedItem != null;
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan_XLPC<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.BT_PhieuCan(connStr);
            return dao.GetPhieuCan_XLPC<T>(dateTime, xuongId);
        }
        #endregion
        #region Tính Lương
        public List<T> GetTongHopsTinhLuong<T>(DateTime dateTime, string xuongId)
        {
            var dao = new Dao.Repos.HQ.BT_PhieuCan();
            return dao.GetsTongHopsTinhLuong<T>(dateTime, xuongId);
        }

        public List<T> Gets<T>(DateTime dateTime)
        {
            var dao = new Dao.Repos.HQ.BT_PhieuCan();

            return dao.Gets<T>(dateTime);
        }
        #endregion
        #region Báo Cáo
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.BT_PhieuCan();
            return dao.GetChiTiets<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetChiTietByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            var dao = new Dao.Repos.HQ.BT_PhieuCan();
            return dao.GetChiTietByMaNhanViens<T>(fromDate, toDate,maNhanVien, xuongId);
        }
        public List<T> GetChiTietByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            var dao = new Dao.Repos.HQ.BT_PhieuCan();
            return dao.GetChiTietByMaHoSos<T>(fromDate, toDate,maHoSo, xuongId);
        }
        public List<T> GetChiTietByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            var dao = new Dao.Repos.HQ.BT_PhieuCan();
            return dao.GetChiTietByMaThes<T>(fromDate, toDate,maThe, xuongId);
        }
        public List<T> GetTongHops<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.BT_PhieuCan();
            return dao.GetTongHops<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopThanhPhamByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            var dao = new Dao.Repos.HQ.BT_PhieuCan();
            return dao.GetTongHopThanhPhamByMaNhanViens<T>(fromDate, toDate, maNhanVien, xuongId);
        }
        public List<T> GetTongHopThanhPhamByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            var dao = new Dao.Repos.HQ.BT_PhieuCan();
            return dao.GetTongHopThanhPhamByMaHoSos<T>(fromDate, toDate, maHoSo, xuongId);
        }
        public List<T> GetTongHopThanhPhamByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            var dao = new Dao.Repos.HQ.BT_PhieuCan();
            return dao.GetTongHopThanhPhamByMaThes<T>(fromDate, toDate, maThe, xuongId);
        }
        #endregion
    }
}
