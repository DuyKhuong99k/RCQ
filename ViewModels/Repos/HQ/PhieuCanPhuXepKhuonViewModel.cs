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
    public partial class PhieuCanPhuXepKhuonViewModel : ObservableObject
    {
        private static PhieuCanPhuXepKhuonViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanPhuXepKhuon? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanPhuXepKhuon> items = new();

        //[ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanBTPFilletv2? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanPhuXepKhuonViewModel()
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
        public static PhieuCanPhuXepKhuonViewModel Instance => instance ??= new PhieuCanPhuXepKhuonViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public List<T> GetTongHopThanhPhams<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
                return dao.GetPhieuCanTongHops2<T>(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
            return dao.GetsLast<T>(dateTime, num);
        }

        public T? Get<T>(string id)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
            return dao.Get<T>(id);
        }

        public int Insert<T>(T item)
        {

            var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
            return dao.Insert(item);
        }
        public PhieuCanPhuXepKhuon CreateDefaultNew()
        {
            return new PhieuCanPhuXepKhuon();
        }
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan_XLPC<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon(connStr);
            return dao.GetPhieuCanPhuXepKhuon_XLPC<T>(dateTime, xuongId);
        }
        #endregion
        #region Báo Cáo

        public List<T> GetPhieuCanPhuXepKhuons<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
                return dao.GetPhieuCanPhuXepKhuonsByDateToDate<T>(fromDate, toDate, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanPhuXepKhuonByMaNhanViens<T>(DateTime fromDate, DateTime toDate,string maNhanVien, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
                return dao.GetPhieuCanPhuXepKhuonsByMaNhanVien<T>(fromDate, toDate,maNhanVien, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanPhuXepKhuonByMaHoSos<T>(DateTime fromDate, DateTime toDate,string maHoSo, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
                return dao.GetPhieuCanPhuXepKhuonsByMaHoSo<T>(fromDate, toDate,maHoSo, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanPhuXepKhuonByMaThes<T>(DateTime fromDate, DateTime toDate,string maThe, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
                return dao.GetPhieuCanPhuXepKhuonsByMaThe<T>(fromDate, toDate,maThe, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanTongHopPhus<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
                return dao.GetPhieuCanTongHopsByDateToDate<T>(fromDate, toDate, xuongId);
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
                var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
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
                var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
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
                var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
                return dao.GetTongHopThanhPhamByMaThes<T>(fromDate, toDate, maThe, xuongId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanTongHopPhus2<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanPhuXepKhuon();
                return dao.GetPhieuCanTongHops2<T>(fromDate, toDate, xuongId);
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
