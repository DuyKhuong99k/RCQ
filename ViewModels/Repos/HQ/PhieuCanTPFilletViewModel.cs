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
        public T? Get<T>(string id)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.Get<T>(id);
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
        public Tuple<int, decimal> GetSoRoTongTrongLuongByNhanVienId(
            DateTime dateTime,
            string nhanVienId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetSoRoTongTrongLuongByNhanVienId(dateTime, nhanVienId);
        }

        public Tuple<int, decimal> GetSoRoTongTrongLuongByNhanVienIdandThanhPhamId(DateTime dateTime, string nhanVienId,
            string thanhPhamId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetSoRoTongTrongLuongByNhanVienIdandThanhPhamId(dateTime, nhanVienId, thanhPhamId);
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

        #region Xẻ bướm
        public List<T> GetChiTietBTPXeBuoms<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetChiTietBTPXeBuoms<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopLTPSBTPXeBuoms<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTongHopLTPSBTPXeBuoms<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopNhanVienBTPXeBuoms<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTongHopNhanVienBTPXeBuoms<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopThanhPhamBTPXeBuoms<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTongHopThanhPhamBTPXeBuoms<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetChiTietTPXeBuoms<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetChiTietTPXeBuoms<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopLTPSTPXeBuoms<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTongHopLTPSTPXeBuoms<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopThanhPhamTPXeBuoms<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTongHopThanhPhamTPXeBuoms<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopThanhPhamTPXeBuoms2<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTongHopThanhPhamTPXeBuoms2<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopNhanVienTPXeBuoms<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTongHopNhanVienTPXeBuoms<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopDinhMucTPXeBuoms<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
            return dao.GetTongHopDinhMucTPXeBuoms<T>(fromDate, toDate, xuongId);
        }
        #endregion

        #region XLPC
        #region BTP XẺ BƯỚM
        public List<T> GetPhieuCanBTPXeBuom_XLPC<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet(connStr);
            return dao.GetPhieuCanBTPXeBuom_XLPC<T>(dateTime, xuongId);
        }
        #endregion

        #region TP XẺ BƯỚM
        public List<T> GetPhieuCanTPXeBuom_XLPC<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFillet(connStr);
            return dao.GetPhieuCanTPXeBuom_XLPC<T>(dateTime, xuongId);
        }
        #endregion
        #endregion
    }
}
