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
    public partial class TPhieuCanThuMuaViewModel : ObservableObject
    {
        private static TPhieuCanThuMuaViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private T_PhieuCanThuMua? item;
        [ObservableProperty] private ObservableRangeCollection<T_PhieuCanThuMua> items = new();

        private readonly SynchronizationContext synchronizationContext;
        private TPhieuCanThuMuaViewModel()
        {
            try
            {
                //Reload();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                //VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        public static TPhieuCanThuMuaViewModel Instance => instance ??= new TPhieuCanThuMuaViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;
        public List<T> GetChiTietsDateTimeToDateTimeNgayNguyenLieu<T>(DateTime fromDate, DateTime toDate,
          string xuongId)
        {
            var dao = new Dao.Repos.HQ.T_PhieuCanThuMua();
            return dao.GetChiTietsDateTimeToDateTimeNgayNguyenLieu<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetChiTietsDateTimeToDateTime<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.T_PhieuCanThuMua();
            return dao.GetChiTietsDateTimeToDateTime<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId
        )
        {
            var dao = new Dao.Repos.HQ.T_PhieuCanThuMua();
            return dao.GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieu<T>(fromDate, dateTime, xuongId);
        }
        public List<T> GetTongHopBaoCaoSauRaiMayPhanCo<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId
        )
        {
            var dao = new Dao.Repos.HQ.T_PhieuCanThuMua();
            return dao.GetTongHopBaoCaoSauRaiMayPhanCo<T>(fromDate, dateTime, xuongId);
        }
         public List<T> GetNgayAndNguyenLieuPhanCo<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId
        )
        {
            var dao = new Dao.Repos.HQ.T_PhieuCanThuMua();
            return dao.GetNgayAndNguyenLieuPhanCo<T>(fromDate, dateTime, xuongId);
        }
        public List<T> GetNgayAndNguyenLieuPhanCo_NgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId
        )
        {
             var dao = new Dao.Repos.HQ.T_PhieuCanThuMua();
            return dao.GetNgayAndNguyenLieuPhanCo_NgayNguyenLieu<T>(fromDate, dateTime, xuongId);
        }
    }
}
