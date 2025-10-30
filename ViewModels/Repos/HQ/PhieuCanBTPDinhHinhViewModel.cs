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
using System.Data;
using Vars.Hubs;

namespace ViewModels.Repos.HQ
{
    public partial class PhieuCanBTPDinhHinhViewModel : ObservableObject
    {
        private static PhieuCanBTPDinhHinhViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanBTPDinhHinh? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanBTPDinhHinh> items = new();
        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanBTPDinhHinh? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanBTPDinhHinh> itemUnlockModes = new();
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanBTPDinhHinhViewModel()
        {
            try
            {
                //UpdateDatabase();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        public static PhieuCanBTPDinhHinhViewModel Instance => instance ??= new PhieuCanBTPDinhHinhViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;


        public bool IsVailSelectedItem => SelectedItem != null;

        public List<T> GetsTongHopMayCan<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetsTongHopMayCan<T>(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void AddItemUnlocks(DateTime dateTime, string theId, bool isEnabled = false)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                var _item = ItemUnlockModes.SingleOrDefault(x => x.MaThe == theId);
                if (_item == null)
                {
                    var item = dao.Get<PhieuCanBTPDinhHinh>(dateTime, theId, isEnabled);
                    if (item != null)
                    {
                        item.GhiChu = "HUY";
                        ItemUnlockModes.Insert(0, item);
                    }
                    else
                    {
                        throw new Exception("Thẻ Chưa Cân hoặc Đã Mở Khóa");
                        //MessageBox.Show("Thẻ Chưa Cân hoặc Đã Mở Khóa");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Delete<T>(T item)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.Delete(item);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public int Delete(DateTime dateTime)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.Delete(dateTime);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public int Delete<T>(List<T> items)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.Delete(items);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public T Get<T>(DateTime dateTime, string theId, bool isEnabled = false)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.Get<T>(dateTime, theId, isEnabled);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTiets<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetChiTiets<T>(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetChiTiets<T>(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetChiTietByMaNhanViens<T>(DateTime fromDate, DateTime toDate,string maNhanVien, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetChiTietByMaNhanViens<T>(fromDate, toDate,maNhanVien, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetChiTietByMaHoSos<T>(DateTime fromDate, DateTime toDate,string maHoSo, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetChiTietByMaHoSos<T>(fromDate, toDate,maHoSo, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTietByMaThes<T>(DateTime fromDate, DateTime toDate,string maThe, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetChiTietByMaThes<T>(fromDate, toDate,maThe, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public T? GetLastByTheId<T>(DateTime dateTime, string theId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetLastByThe<T>(dateTime, theId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public T? GetLastByTheId<T>(DateTime dateTime, string theId, bool isEnabled)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetLastByThe<T>(dateTime, theId, isEnabled);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public T GetLastByNhanVienId<T>(DateTime dateTime, string nhanVienId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetLastByNhanVien<T>(dateTime, nhanVienId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetMaxSTT(DateTime dateTime, string mayCanId, bool isServer = false)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetMaxSTT<int>(dateTime, mayCanId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetMayCans(DateTime dateTime, bool isServer = false)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetMayCans(dateTime);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetNumNhanVienDaChiaCa(DateTime dateTime, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetNumNhanVienDaChiaCa(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetNumTheDaSuDung(DateTime dateTime, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetNumTheDaSuDung(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PhieuCanBTPDinhHinh> Gets(DateTime dateTime)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.Gets<PhieuCanBTPDinhHinh>(dateTime);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.Gets<T>(dateTime);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PhieuCanBTPDinhHinh> Gets(DateTime dateTime, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.Gets<PhieuCanBTPDinhHinh>(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PhieuCanBTPDinhHinh> GetOfflines(DateTime dateTime, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetOfflines<PhieuCanBTPDinhHinh>(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime, string mayCanId, int stt)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.Gets<T>(dateTime, mayCanId, stt);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PhieuCanBTPDinhHinh> Gets(DateTime dateTime, string xuongId, string mayCanId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.Gets<PhieuCanBTPDinhHinh>(dateTime, xuongId, mayCanId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PhieuCanBTPDinhHinh> Gets_mayCan(DateTime dateTime, string maycan)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.Gets_maycan<PhieuCanBTPDinhHinh>(dateTime, maycan);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets_STT_IsEnabled<T>(DateTime dateTime, string mayCanId, bool isServer = false)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.Gets_STT_IsEnabled<T>(dateTime, mayCanId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets_STT_IsEnabled<T>(
            DateTime dateTime,
            string mayCanId,
            List<int> sttsEx,
            bool isServer = false)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.Gets_STT_IsEnabled<T>(dateTime, mayCanId, sttsEx);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets_STT_IsEnabled<T>(DateTime dateTime, string mayCanId, bool isEnabled, bool isServer = false)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.Gets_STT_IsEnabled<T>(dateTime, mayCanId, isEnabled);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopNhanhs<T>(DateTime dateTime, string xuongId, string maHoSo)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongHopNhanh<T>(dateTime, xuongId, maHoSo);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopNhanhs_MaNhanVien<T>(DateTime dateTime, string xuongId, string maHoSo)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongHopNhanh_MaNhanVien<T>(dateTime, xuongId, maHoSo);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetTongHops(DateTime dateTime, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongHops(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetTongHops(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongHops(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHops<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongHops<T>(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHops<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongHops<T>(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopNhanVienPhucVus<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongHopNhanVienPhucVus<T>(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetTongHopThanhPhams(DateTime dateTime, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongHopThanhPhams(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetTongHopThanhPhams(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongHopThanhPhams(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhams<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongHopThanhPhams<T>(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien,string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongHopThanhPhamByMaNhanViens<T>(fromDate, toDate, maNhanVien,xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe,string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongHopThanhPhamByMaThes<T>(fromDate, toDate, maThe,xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo,string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongHopThanhPhamByMaHoSos<T>(fromDate, toDate, maHoSo,xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopTheoMayLangDas<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongHopTheoMayLangDa<T>(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Tuple<int, decimal> GetTongSoRoTongTrongLuong(DateTime dateTime, string nhanVienId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetTongSoRoTongTrongLuong(dateTime, nhanVienId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public Tuple<int, decimal, int, decimal> GetTongSoRoTongTrongLuong(
        //    DateTime dateTime,
        //    string nhanVienId,
        //    string thanhPhamId)
        //{
        //    try
        //    {
        //        //var connecttionString = PMSSTv1.ViewModel.SettingViewModel.Ins.ConnectionString;
        //        //#if DEBUG
        //        //                connecttionString = "";
        //        //#endif
        //        var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
        //        if (VmApp.AppType == Modelv1.Vars.AppType._type3)
        //        {
        //            return dao.GetTongSoRoTongTrongLuong_laychiSanLuong(dateTime, nhanVienId, thanhPhamId);
        //        }
        //        else
        //        {
        //            return dao.GetTongSoRoTongTrongLuong(dateTime, nhanVienId, thanhPhamId);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public List<T> GetPhieuCanChiTiets_TG<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetChiTiets_TG<T>(fromDate, toDate);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetPhieuCanChiTiets_TG(DateTime dateTime, DateTime toDate)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.GetChiTiets_TG(dateTime, toDate);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert<T>(T item)//, bool isServer = false)
        {
            try
            {

                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();

                return dao.Insert(item);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Update<T>(T item)
        {
            try
            {

                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();

                return dao.Update(item);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
            return dao.GetsLast<T>(dateTime, num);
        }
        public int Update(string id, bool isEnabled)
        {
            try
            {

                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();

                return dao.Update(id, isEnabled);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert<T>(List<T> items)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh();
                return dao.Insert(items);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public PhieuCanBTPDinhHinh CopyItem(PhieuCanBTPDinhHinh item)
        {
            return new PhieuCanBTPDinhHinh()
            {
                STT = item.STT,
                Ngay = item.Ngay,
                Gio = item.Gio,
                MaUserCan = item.MaUserCan,
                MaMayCan = item.MaMayCan,
                MaLoaiCa = item.MaLoaiCa,
                MaMau = item.MaMau,
                MaSize = item.MaSize,
                MaThanhPham = item.MaThanhPham,
                MaLo = item.MaLo,
                MaThe = item.MaThe,
                MaNhanVien = item.MaNhanVien,
                MaMayLangDa = item.MaMayLangDa,
                TrongLuong = item.TrongLuong,
                IsEnabled = item.IsEnabled,
                MaXuong = item.MaXuong,
                CaTra = item.CaTra,
                GhiChu = item.GhiChu,
                ChiSanLuong = item.ChiSanLuong,
                TrongLuongBu = item.TrongLuongBu,
                TrongLuongTare = item.TrongLuongTare,
                IsOffline = item.IsOffline
            };
        }
        public PhieuCanBTPDinhHinh CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }
        public PhieuCanBTPDinhHinh CreateDefaultNew()
        {
            var stt = 0;
            if (Items.Any())
            {
                stt = Items.Select(x => Math.Abs(x.STT)).DefaultIfEmpty(0).Max() + 1;
            }
            else
            {
                stt = 1;
            }
            return new PhieuCanBTPDinhHinh
            {
                STT = stt,
                Ngay = DateTime.Now,
                Gio = DateTime.Now.TimeOfDay,
                MaUserCan = AppViewModel.Instance.UserName,
                MaMayCan = AppViewModel.Instance.PCName,
                MaLoaiCa = LoaiCaDinhHinhViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaMau = MauDinhHinhViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaSize = SizeDinhHinhViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaThanhPham = ThanhPhamDinhHinhViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaLo = LoViewModel.Instance.ItemsWithSize?.FirstOrDefault()?.Item2,
                MaThe = "0",
                //MaNhanVien = item.MaNhanVien,
                //MaMayLangDa = ,
                TrongLuong = 0,
                //IsEnabled = item.IsEnabled,
                MaXuong = AppViewModel.Instance.XuongId,
                //CaTra = item.CaTra,
                GhiChu = "",
                //ChiSanLuong = item.ChiSanLuong,
                TrongLuongBu = 0,
                TrongLuongTare = 0,
                //IsOffline = AppViewModel.Ins.of

            };
        }
        public PhieuCanBTPDinhHinh CreateDefaultNew(string mayCan, DataCoummunication data)
        {
            var stt = 0;
            if (Items.Any())
            {
                stt = Items.Where(x => x.MaMayCan == mayCan).Select(x => Math.Abs(x.STT)).DefaultIfEmpty(0).Max() + 1;
            }
            else
            {
                stt = 1;
            }
            return new PhieuCanBTPDinhHinh
            {
                STT = stt,
                Ngay = DateTime.Now,
                Gio = DateTime.Now.TimeOfDay,
                MaUserCan = AppViewModel.Instance.UserName,
                MaMayCan = mayCan,
                MaLoaiCa = LoaiCaDinhHinhViewModel.Instance.Items.FirstOrDefault()?.Ma ?? "",
                MaMau = MauDinhHinhViewModel.Instance.Items.FirstOrDefault()?.Ma ?? "",
                MaSize = data.MaSize ?? "",
                MaThanhPham = data.MaThanhPham,
                MaLo = data.MaLo,
                MaThe = data.TheId ?? "",
                TrongLuong = data.TrongLuong,
                MaXuong = AppViewModel.Instance.XuongId,
                GhiChu = "",
                TrongLuongBu = 0,
                TrongLuongTare = data.TrongLuongTare,
                Id = data.IdIn,
                CaTra = false,
                ChiSanLuong = false,
                IsEnabled = false,
                IsOffline = false,
                MaMayLangDa = "",
                MaNhanVien = data.MaNhanVien
            };
        }


        public DataCoummunication Convert(PhieuCanBTPDinhHinh item)
        {
            return new DataCoummunication()
            {
                TheId = item.MaThe,
                IdIn = item.Id,
                MaLo = item.MaLo,
                MaNhanVien = item.MaNhanVien,
                MaSize = item.MaSize,
                MaThanhPham = item.MaThanhPham,
                MayCan = item.MaMayCan,
                NgayGio = $"{item.Ngay:yyyyMMdd}{item.Gio:HHmmss}",
                TrongLuong = item.TrongLuong,
                TrongLuongTare = item.TrongLuongTare,


            };
        }
        public void LoadDatas(DateTime dateTime, string xuongId, string mayCanId)
        {
            try
            {
                Items.Clear();
                Items = new ObservableRangeCollection<PhieuCanBTPDinhHinh>(Gets(dateTime, xuongId,
                    mayCanId));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int ChuyenXuong(List<PhieuCanBTPDinhHinh> items, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh(connStr);
            return dao.ChuyenXuong(items, xuongId);
        }
        public int ChuyenSize(List<PhieuCanBTPDinhHinh> items, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh(connStr);
            return dao.ChuyenSize(items, xuongId);
        }
        public int ChuyenThanhPham(List<PhieuCanBTPDinhHinh> items, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh(connStr);
            return dao.ChuyenThanhPham(items, xuongId);
        }
        #region Tính Lương Phụ Fillet
        public double GetSanLuongBTPDinhHinh(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string mayLangDa,
            string exThanhPhamId,
            string sizeId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh(connStr);
                return dao.GetSanLuong(fromTime, toTime, dateTime, xuongId, mayLangDa, exThanhPhamId, sizeId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuongBTPDinhHinh(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string mayLangDa,
            string exThanhPhamId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPDinhHinh(connStr);
                return dao.GetSanLuong(fromTime, toTime, dateTime, xuongId, mayLangDa, exThanhPhamId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
