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
    public partial class PhieuCanTPFilletViewModel : ObservableObject
    {
        private static PhieuCanTPFilletViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanTPFillet? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanTPFillet> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanTPFillet? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanTPFilletViewModel()
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

        public static PhieuCanTPFilletViewModel Instance => instance ??= new PhieuCanTPFilletViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public PhieuCanTPFillet CopyItem(PhieuCanTPFillet item)
        {
            return new PhieuCanTPFillet
            {

            };
        }
        public PhieuCanTPFillet CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }
        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetsLast<T>(dateTime, num);
        }
        public PhieuCanTPFillet CreateDefaultNew()
        {

            return new PhieuCanTPFillet
            {
                //SuDung = true,
                //Ma = id
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.Delete(item);
        }

        private List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.Gets<T>();
        }
        public List<T> Gets<T>(DateTime dateTime)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.Gets<T>(dateTime);
        }
        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.Insert(item);
        }


        public bool IsVailSelectedItem => SelectedItem != null;

        public void Reload()
        {
            lock (Items)
            {
                Items.Clear();
            }

            var items = Gets<PhieuCanTPFillet>();
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
        private int Update<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.Update(item);
        }
        public List<T> GetPhieuCanChiTiets<T>(DateTime fromDate, DateTime dateTime, string xuongId,
            bool isServer = false)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetChiTiets<T>(fromDate, dateTime, xuongId);
        }
        public List<T> GetPhieuCanChiTietByMaNhanViens<T>(DateTime fromDate, DateTime dateTime,string maNhanVien, string xuongId,
            bool isServer = false)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetChiTietByMaNhanViens<T>(fromDate, dateTime,maNhanVien, xuongId);
        }
        public List<T> GetPhieuCanChiTietByMaHoSos<T>(DateTime fromDate, DateTime dateTime,string maHoSo, string xuongId,
            bool isServer = false)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetChiTietByMaHoSos<T>(fromDate, dateTime,maHoSo, xuongId);
        }
        public List<T> GetPhieuCanChiTietByMaThes<T>(DateTime fromDate, DateTime dateTime,string maThe, string xuongId,
            bool isServer = false)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetChiTietByMaThes<T>(fromDate, dateTime,maThe, xuongId);
        }





        public List<T> GetPhieuCanTongHopNhanViens<T>(DateTime fromDate, DateTime dateTime, string xuongId,
            bool isServer = false)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTongHopNhanViens<T>(fromDate, dateTime, xuongId);
        }
        public List<T> GetPhieuCanTonghopThanhPhams<T>(DateTime fromDate, DateTime dateTime, string xuongId,
            bool isServer = false)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTongHopThanhPhams<T>(fromDate, dateTime, xuongId);
        }
        public List<T> GetTongHopThanhPhamByMaNhanViens<T>(DateTime fromDate, DateTime dateTime, string maNhanVien, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTongHopThanhPhamByMaNhanViens<T>(fromDate, dateTime, maNhanVien, xuongId);
        }
        public List<T> GetTongHopThanhPhamByMaHoSos<T>(DateTime fromDate, DateTime dateTime, string maHoSo, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTongHopThanhPhamByMaHoSos<T>(fromDate, dateTime, maHoSo, xuongId);
        }
        public List<T> GetPhieuCanTonghopThanhPhamByMaThes<T>(DateTime fromDate, DateTime dateTime, string maThe, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTongHopThanhPhamByMaThes<T>(fromDate, dateTime, maThe, xuongId);
        }
        public List<T> GetPhieuCanTongHopNhanVienPhucVus<T>(DateTime dateTime, string sanPhamId, string xuongId,
            bool isServer = false)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTongHopNhanVienPhucVus<T>(dateTime, sanPhamId, xuongId);
        }
        #region Tính Lương Fillet
        public List<T> GetPhieuTinhLuongs<T>(DateTime dateTime, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTinhLuongs<T>(dateTime, xuongId);
        }
        public void LoadDataTinhLuongs(DateTime dateTime, string xuongId)
        {
            //PhieuTinhLuongs.Clear();

            BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime);
            var phieuTinhLuongs = new List<BravoModelV1.Model.PhieuLuongFillet>(GetPhieuTinhLuongs<BravoModelV1.Model.PhieuLuongFillet>(dateTime, xuongId));
            for (var i = 0; i < phieuTinhLuongs.Count; i++)
                phieuTinhLuongs[i].IsChamCong =
                    BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime).FirstOrDefault(x =>
                        x == phieuTinhLuongs[i].MaNhanVien) == null
                        ? false
                        : true;
        }
        public List<BravoModelV1.Model.PhieuLuongFillet> ReloadTinhLuong(DateTime dateTime, string xuongId)
        {
            try
            {
                BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime);
                var phieuTinhLuongs = new List<BravoModelV1.Model.PhieuLuongFillet>(GetPhieuTinhLuongs<BravoModelV1.Model.PhieuLuongFillet>(dateTime, xuongId));
                for (var i = 0; i < phieuTinhLuongs.Count; i++)
                    phieuTinhLuongs[i].IsChamCong =
                        BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime).FirstOrDefault(x =>
                            x == phieuTinhLuongs[i].MaNhanVien) == null
                            ? false
                            : true;
                return phieuTinhLuongs.ToList();
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                return new List<BravoModelV1.Model.PhieuLuongFillet>();
                //throw;
            }
        }
        public List<T> GetsTPSoft<T>(DateTime dateTime, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetsTPSoft<T>(dateTime, xuongId);
        }
        #endregion
    }
}
