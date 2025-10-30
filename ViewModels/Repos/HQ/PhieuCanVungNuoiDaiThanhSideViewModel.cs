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
    public partial class PhieuCanVungNuoiDaiThanhSideViewModel : ObservableObject
    {
        private static PhieuCanVungNuoiDaiThanhSideViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanVungNuoiDaiThanhSide? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanVungNuoiDaiThanhSide> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanVungNuoiDaiThanhSide? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanVungNuoiDaiThanhSideViewModel()
        {
            try
            {
                //Reload();

            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                ////throw;
                //VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        public static PhieuCanVungNuoiDaiThanhSideViewModel Instance => instance ??= new PhieuCanVungNuoiDaiThanhSideViewModel();
        public bool IsVailSelectedItem => SelectedItem != null;
        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;
        public List<T> GetTongHopGhe<T>(DateTime fromDate, DateTime toDate)
        {
            var dao = new Dao.Repos.HQ.PhieuCanVungNuoiDaiThanhSide();
            return dao.GetTongHopGhe<T>(fromDate.Date, toDate.Date);
        }
        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanVungNuoiDaiThanhSide();
            return dao.Delete(item);
        }
        private List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.PhieuCanVungNuoiDaiThanhSide();
            return dao.Gets<T>();
        }

        public List<T> Gets<T>(DateTime date, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanVungNuoiDaiThanhSide(connStr);
            return dao.Gets<T>(date.Date);
        }
        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanVungNuoiDaiThanhSide();
            return dao.Insert(item);
        }
        private int Update<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanVungNuoiDaiThanhSide();
            return dao.Update(item);
        }
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan<T>(DateTime dateTime, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanVungNuoiDaiThanhSide(connStr);
            return dao.GetPhieuCan<T>(dateTime);
        }
        #endregion
        #region Báo Cáo
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate)
        {
            var dao = new Dao.Repos.HQ.PhieuCanVungNuoiDaiThanhSide();
            return dao.GetChiTiets<T>(fromDate, toDate);
        }
        public List<T> GetTongHops<T>(DateTime fromDate, DateTime toDate)
        {
            var dao = new Dao.Repos.HQ.PhieuCanVungNuoiDaiThanhSide();
            return dao.GetTongHops<T>(fromDate, toDate);
        }
        public List<T> GetTongHops_NgayBatCa<T>(DateTime fromDate, DateTime toDate)
        {
            var dao = new Dao.Repos.HQ.PhieuCanVungNuoiDaiThanhSide();
            return dao.GetTongHops_NgayBatCa<T>(fromDate, toDate);
        }
        public List<T> GetTongHops_NgayNhapXuong<T>(DateTime fromDate, DateTime toDate)
        {
            var dao = new Dao.Repos.HQ.PhieuCanVungNuoiDaiThanhSide();
            return dao.GetTongHops_NgayNhapXuong<T>(fromDate, toDate);
        }
        #endregion
    }
}
