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
    public partial class PhieuCanNguyenLieuViewModel : ObservableObject
    {
        private static PhieuCanNguyenLieuViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanNguyenLieu? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanNguyenLieu> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanNguyenLieu? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanNguyenLieuViewModel()
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

        public static PhieuCanNguyenLieuViewModel Instance => instance ??= new PhieuCanNguyenLieuViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public List<T> GetTongHopThanhPhamDashBoard<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.GetTongHopThanhPhamDashBoard<T>(fromDate.Date, toDate.Date, xuongId);
        }

        //public PhieuCanNguyenLieu CopyItem(PhieuCanNguyenLieu item)
        //{
        //    return new PhieuCanNguyenLieu
        //    {
        //        SuDung = item.SuDung,
        //        Ma = item?.Ma,
        //        Ten = item?.Ten
        //    };
        //}
        //public PhieuCanNguyenLieu CopySelectedItem()
        //{
        //    return CopyItem(SelectedItem);
        //}
        public PhieuCanNguyenLieu CreateDefaultNew()
        {
            return new PhieuCanNguyenLieu
            {
                Chuyen = 0,
                GhiChu = "",
                MaBanCatTiet = "",
                MSL = "",
                MaAo = "",
                MaLoaiCa = "",
                MaLoaiThanhPham = "",
                MaMau = "",
                MaMayTinhCan = VmApp.PCName,
                MaPhuongTien = "",
                MaSize = "",
                MaUserCan = VmApp.UserName,
                MaXuongSanXuat = VmApp.XuongId,
                Ngay = DateTime.Now,
                NhaCC = "",
                Pheu = "0",
                SuDung = true,
                ThoiGianCan = DateTime.Now,
                TrongLuong = 0,
                TrongLuongOrg = 0,
                TrongLuongTare = 0,
                TyLeNuoc = 0,
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.Delete(item);
        }

        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Delete_(PhieuCanNguyenLieu item)
        //{
        //    try
        //    {
        //        if (Delete(item) > 0)
        //            lock (Items)
        //            {
        //                var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
        //                if (_item != null)
        //                {
        //                    var index = Items.IndexOf(_item);
        //                    Items.RemoveAt(index);
        //                    //Items.Insert(index,item);
        //                }
        //            }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}

        private List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.Gets<T>();
        }

        public T? Get<T>(string id)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.Get<T>(id);
        }
        public List<T> Gets<T>(DateTime dateTime)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.Gets<T>(dateTime);
        }

        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.Insert(item);
        }

        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Insert_(PhieuCanNguyenLieu item)
        //{
        //    try
        //    {
        //        if (Insert(item) > 0)
        //        {
        //            var items = new List<PhieuCanNguyenLieu>();
        //            lock (Items)
        //            {
        //                Items.Add(item);
        //            }
        //            VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
        //            IsWindowItemShown = false;
        //        }
        //        else
        //        {
        //            VmMessage.MessageBoxShow("Không thể thêm", "Thông Báo", 0);
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}

        //[RelayCommand()]
        //private void Insert2_()
        //{
        //    try
        //    {
        //        if (Insert(Item) > 0)
        //        {
        //            Items.Insert(0, Item);
        //            VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        VmMessage.SetExceptionCommand.Execute(e);
        //        //throw;
        //    }
        //}
        public bool IsVailSelectedItem => SelectedItem != null;
        //private bool IsItemPass(PhieuCanNguyenLieu item)
        //{
        //    return item != null && item.Ten != null && item.Ten.Trim() != "" &&
        //           item.SuDung != null;
        //}
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

            var items = Gets<PhieuCanNguyenLieu>();
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
        private void Reload_(ObservableRangeCollection<PhieuCanNguyenLieu> obj)
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

        private int Update<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.Update(item);
        }

        public List<T> GetTongQuans<T>(DateTime fromDate, DateTime toDate)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.GetTongQuans<T>(fromDate, toDate);
        }
        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.GetsLast<T>(dateTime, num);
        }
        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Update_(PhieuCanNguyenLieu item)
        //{
        //    try
        //    {
        //        if (Update(item) > 0)
        //            lock (Items)
        //            {
        //                var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
        //                if (_item != null)
        //                {
        //                    var index = Items.IndexOf(_item);
        //                    Items.RemoveAt(index);
        //                    Items.Insert(index, item);
        //                }
        //            }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}
        //[RelayCommand()]
        //private void Update2_()
        //{
        //    try
        //    {
        //        if (Update(Item) > 0)
        //            lock (Items)
        //            {
        //                var _item = Items.SingleOrDefault(x => x.Ma == Item.Ma);
        //                if (_item != null)
        //                {
        //                    var index = Items.IndexOf(_item);
        //                    Items.RemoveAt(index);
        //                    Items.Insert(index, Item);
        //                    SelectedItem = Item;
        //                }
        //            }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetPhieuCan<T>(dateTime, xuongId);
        }
        public List<Tuple<string, DateTime>> GetAos(string xuongId, int topVal = 4, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
                return dao.GetsAo(xuongId, topVal);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Tính Lương Phụ Fillet
        public double GetSanLuongNguyenLieu(TimeSpan fromTime, TimeSpan toTime, DateTime dateTime, string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
                return dao.GetSanLuong(fromTime, toTime, dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuongNguyenLieu(DateTime dateTime, string xuongId, string banId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
                return dao.GetSanLuong(dateTime, xuongId, banId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuongNguyenLieuTruNgop(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string banId,
            string sizeId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
                return dao.GetSanLuongTruNgop(fromTime, toTime, dateTime, xuongId, banId, sizeId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuongCaNgopTruBan(
            DateTime dateTime,
            string xuongId,
            string sizeId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
                return dao.GetSanLuongCaNgopTruBan(dateTime, xuongId, sizeId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetMSLs(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string sizeId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
                return dao.GetMSLs(fromTime, toTime, dateTime, xuongId, sizeId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuongCaNgop(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string banId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
                return dao.GetSanLuongCaNgop(fromTime, toTime, dateTime, xuongId, banId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Báo Cáo
        public List<T> GetPhieuCanChiTiets<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetChiTiets<T>(fromDate, toDate);
        }
        public List<T> GetPhieuCanTongHopNCCs<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetTongHopNCC<T>(fromDate, toDate);
        }
        //BÀN CẮT TIẾT
        public List<T> GetPhieuCanTongHopBCTs<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetTongHopBCT<T>(fromDate, toDate);
        }
        public List<T> GetPhieuCanTongHopPhuongTiens<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetTongHopPhuongTien<T>(fromDate, toDate);
        }
        public List<T> GetPhieuCanTongHopLos<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetTongHopLo<T>(fromDate, toDate);
        }
        #endregion
        public List<T> GetsCaTra<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetsCaTra<T>(dateTime, xuongId);
        }
        public List<T> GetChiTietPhieuCanKhongTheGhiNhanDuLieu<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetChiTietPhieuCanKhongTheGhiNhanDuLieu<T>(fromDate, toDate);
        }

        #region Báo Cao Thành Phẩm 2
        public List<T> GetChiTietThanhPham2s<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetChiTietThanhPham2s<T>(fromDate, toDate);
        }


        public class TyLeThuHoiThanhPham2s<T>
        {
            public string? MaLoaiThanhPham { get; set; }
            public string? ThanhPhamName { get; set; }
            public string? CTTYLE { get; set; }
            public double? SanLuong { get; set; }
            public double? SanLuongNL { get; set; }
            public double TyLeThuHoi { get; set; }
        }
        public List<T> GetTongHopSLNLThanhPham2<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetTongHopSLNLThanhPham2<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopSanPhamThanhPham2<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetTongHopSanPhamThanhPham2<T>(fromDate, toDate);
        }
        public List<TyLeThuHoiThanhPham2s<T>> GetTyLeThuHoiThanhPham2<T>(
    DateTime fromDate,
    DateTime toDate,
    string xuongId,
    string? connStr = null)
        {
            // Lấy dữ liệu thành phẩm và nguyên liệu
            var listTP = GetTongHopSanPhamThanhPham2<T>(fromDate, toDate, connStr).Cast<dynamic>().ToList();
            var listNL = GetTongHopSLNLThanhPham2<T>(fromDate, toDate, xuongId, connStr).Cast<dynamic>().ToList();

            // Gom nhóm nguyên liệu theo MaLoaiThanhPham + NL (NL = NLFILLET, NLXEBUOM, TONGNL)
            var tongNLTheoLoaiVaNL = listNL
                .GroupBy(x => $"{x.NL ?? ""}")
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => (double?)(x.TrongLuong ?? 0)) ?? 0
                );

            var result = new List<TyLeThuHoiThanhPham2s<T>>();

            foreach (dynamic item in listTP)
            {
                string maLoai = item.MaLoaiThanhPham;
                string tenLoai = item.TenThanhPham;
                string cttyle = item.CTTYLE;
                double tongSanLuong = (double)(item.TrongLuong ?? 0);

                double nguyenLieu = 0;
                double tyLe = 0;

                string? loaiNL = null;
                if (cttyle == "SL/NLFILLET") loaiNL = "NLFILLET";
                else if (cttyle == "SL/NLXEBUOM") loaiNL = "NLXEBUOM";
                else if (cttyle == "SL/TONGNL") loaiNL = "TONGNL";

                if (loaiNL != null && tongNLTheoLoaiVaNL.TryGetValue(loaiNL, out nguyenLieu) && nguyenLieu > 0)
                {
                    tyLe = Math.Round(tongSanLuong / nguyenLieu, 4);
                }

                result.Add(new TyLeThuHoiThanhPham2s<T>
                {
                    MaLoaiThanhPham = maLoai,
                    ThanhPhamName = tenLoai,
                    CTTYLE = cttyle,
                    SanLuong = tongSanLuong,
                    SanLuongNL = nguyenLieu,
                    TyLeThuHoi = tyLe
                });
            }

            return result;
        }
        #endregion
    }
}
