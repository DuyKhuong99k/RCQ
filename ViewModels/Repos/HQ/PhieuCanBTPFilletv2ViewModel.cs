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
    public partial class PhieuCanBTPFilletv2ViewModel : ObservableObject
    {
        private static PhieuCanBTPFilletv2ViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanBTPFilletv2? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanBTPFilletv2> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanBTPFilletv2? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanBTPFilletv2ViewModel()
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

        public static PhieuCanBTPFilletv2ViewModel Instance => instance ??= new PhieuCanBTPFilletv2ViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public List<T> Gets<T>(DateTime dateTime, string theId,bool isEnabled = false)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
                return dao.Gets<T>(dateTime, theId, isEnabled);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetsLastMinutes<T>(int minu)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
            return dao.GetsLastMinutes<T>(minu);
        }
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
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
                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
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
                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
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
                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
                return dao.GetChiTietByMaThes<T>(fromDate, toDate,maThe, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopNhanViens<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
                return dao.GetTongHopNhanViens<T>(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetChiTietPhieuCanChuaSuas<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
            return dao.GetChiTietPhieuCanChuaSuas<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopPhucVus<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
                return dao.GetTongHopPhucVus<T>(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
            return dao.GetsLast<T>(dateTime, num);
        }
        public List<T> GetTongHopThanhPhams<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
                return dao.GetTongHopThanhPhams<T>(fromDate, toDate, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopLos<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
                return dao.GetTongHopLo<T>(fromDate, toDate, xuongId);
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
                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
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
                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
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
                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
                return dao.GetTongHopThanhPhamsByMaThe<T>(fromDate, toDate, maThe, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public PhieuCanBTPFilletv2 CopyItem(PhieuCanBTPFilletv2 item)
        {
            return new PhieuCanBTPFilletv2
            {
                TrongLuong = item.TrongLuong,
                TrongLuongTare = item.TrongLuongTare,
                CaTra = item.CaTra,
                GhiChu = item.GhiChu,
                Gio = item.Gio,
                IsEnabled = item.IsEnabled,
                MaLo = item.MaLo,
                MaLoaiCa = item.MaLoaiCa,
                MaMau = item.MaMau,
                MaMayCan = item.MaMayCan,
                MaMayLangDa = item.MaMayLangDa,
                MaNhanVien = item.MaNhanVien,
                MaNhanVienPhucVu = item.MaNhanVienPhucVu,
                MaSize = item.MaSize,
                MaThanhPham = item.MaThanhPham,
                MaThe = item.MaThe,
                MaUserCan = item.MaUserCan,
                MaXuong = item.MaXuong,
                Ngay = item.Ngay,
                STT = item.STT,
            };
        }
        public PhieuCanBTPFilletv2 CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public PhieuCanBTPFilletv2 CreateDefaultNew()
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

            return new PhieuCanBTPFilletv2()
            {
                STT = stt,
                CaTra = false,
                Gio = DateTime.Now.TimeOfDay,
                IsEnabled = true,
                //MaLo = PMSMSharedv1.ViewModel.LoViewModel.Ins.ItemsWithSize?.FirstOrDefault()?.Item2,
                MaLoaiCa = LoaiCaFilletViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaMau = MauFilletViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaMayCan = AppViewModel.Instance.PCName,
                MaMayLangDa = "M1",
                MaSize = SizeFilletViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaThanhPham = ThanhPhamFilletViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaThe = "0",
                //MaUserCan = AppViewModel.Instance.UserName,
                MaXuong = XiNghiepViewModel.Instance.SelectedItem?.Ma,
                Ngay = AppViewModel.Instance.DateTimeNow,
                TrongLuongTare = 0,
                TrongLuong = 0

            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(PhieuCanBTPFilletv2 item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.STT == item.STT);
                        if (_item != null)
                        {
                            var index = Items.IndexOf(_item);
                            Items.RemoveAt(index);
                            //Items.Insert(index,item);
                        }
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        private List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
            return dao.Gets<T>();
        }

        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(PhieuCanBTPFilletv2 item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<PhieuCanBTPFilletv2>();
                    lock (Items)
                    {
                        Items.Add(item);
                    }
                    VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
                    IsWindowItemShown = false;
                }
                else
                {
                    VmMessage.MessageBoxShow("Không thể thêm", "Thông Báo", 0);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        [RelayCommand()]
        private void Insert2_()
        {
            try
            {
                if (Insert(Item) > 0)
                {
                    Items.Insert(0, Item);
                    VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
                }
            }
            catch (Exception e)
            {
                VmMessage.SetExceptionCommand.Execute(e);
                //throw;
            }
        }
        public bool IsVailSelectedItem => SelectedItem != null;
        private bool IsItemPass(PhieuCanBTPFilletv2 item)
        {
            return item != null &&
                 item.Gio != null &&
                 item.MaLoaiCa != null &&
                 item.MaLoaiCa.Trim() != string.Empty &&
                 item.Ngay != null &&
                 item.MaMayCan != null &&
                 item.MaMayCan.Trim() != string.Empty &&
                 item.STT > 0 &&
                 item.MaXuong != null &&
                 item.MaXuong.Trim() != string.Empty &&
                 item.MaUserCan != null &&
                 item.MaUserCan.Trim() != string.Empty &&
                 item.MaLoaiCa != null &&
                 item.MaLoaiCa.Trim() != string.Empty &&
                 item.MaMau != null &&
                 item.MaMau.Trim() != string.Empty &&
                 item.MaSize != null &&
                 item.MaSize.Trim() != string.Empty &&
                 item.MaThanhPham != null &&
                 item.MaThanhPham.Trim() != string.Empty &&
                 item.MaLo != null &&
                 item.MaLo.Trim() != string.Empty &&
                 item.MaThe != null &&
                 item.MaThe.Trim() != string.Empty &&
                 item.TrongLuong > 0 &&
                 item.MaNhanVien != null &&
                 item.MaNhanVien.Trim() != string.Empty;
        }
        //[RelayCommand]
        //private void ForceRaseCanExcute()
        //{
        //    try
        //    {
        //        Insert_Command.NotifyCanExecuteChanged();
        //        Delete_Command.NotifyCanExecuteChanged();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //    }
        //}
        public void Reload()
        {
            lock (Items)
            {
                Items.Clear();
            }

            var items = Gets<PhieuCanBTPFilletv2>();
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

        [RelayCommand]
        private void Reload_(ObservableRangeCollection<PhieuCanBTPFilletv2> obj)
        {
            try
            {
                Reload();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        public int Update<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
            return dao.Update(item);
        }

        public int Update<T>(List<T> items)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
            return dao.Update(items);
        }
        public int Update(string id, bool isEnabled)
        {
            try
            {

                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();

                return dao.Update(id, isEnabled);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Set(int stt, DateTime ngay, string mayCan, string maXuong, bool isEnabled, string nhanVienId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
            return dao.Set(stt, ngay, mayCan, maXuong, isEnabled, nhanVienId);
        }
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(PhieuCanBTPFilletv2 item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.STT == item.STT);
                        if (_item != null)
                        {
                            var index = Items.IndexOf(_item);
                            Items.RemoveAt(index);
                            Items.Insert(index, item);
                        }
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        [RelayCommand()]
        private void Update2_()
        {
            try
            {
                if (Update(Item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.STT == Item.STT);
                        if (_item != null)
                        {
                            var index = Items.IndexOf(_item);
                            Items.RemoveAt(index);
                            Items.Insert(index, Item);
                            SelectedItem = Item;
                        }
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        public PhieuCanBTPFilletv2? Find(int ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.STT == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(int ma)
        {
            try
            {

                return Find(ma) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public T? GetLastByTheId<T>(DateTime dateTime, string theId, bool isEnabled )
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
                return dao.GetLastByThe<T>(dateTime, theId, isEnabled);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public T? GetLastByTheId<T>(DateTime dateTime, string theId )
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
                return dao.GetLastByThe<T>(dateTime, theId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<T> GetTongHopThanhPhamDatBTPFillets<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
            return dao.GetTongHopThanhPhamDatBTPFillets<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetSanLuongDatNguyenConBTPFillets<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
            return dao.GetSanLuongDatNguyenConBTPFillets<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopThanhPhamDatRjNguyenConBTPFillets<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2();
            return dao.GetTongHopThanhPhamDatRjNguyenConBTPFillets<T>(fromDate, toDate, xuongId);
        }
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCanBTPFillet_XLPC<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanBTPFilletv2(connStr);
            return dao.GetPhieuCanBTPFillet_XLPC<T>(dateTime, xuongId);
        }
        public int ChuyenXuong(List<PhieuCanTPDinhHinh> items, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh(connStr);
            return dao.ChuyenXuong(items, xuongId);
        }
        public int ChuyenSize(List<PhieuCanTPDinhHinh> items, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh(connStr);
            return dao.ChuyenSize(items, xuongId);
        }
        public int ChuyenThanhPham(List<PhieuCanTPDinhHinh> items, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh(connStr);
            return dao.ChuyenThanhPham(items, xuongId);
        }
        #endregion
    }
}
