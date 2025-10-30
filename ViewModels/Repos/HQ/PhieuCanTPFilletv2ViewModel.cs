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
    public partial class PhieuCanTPFilletv2ViewModel : ObservableObject
    {
        private static PhieuCanTPFilletv2ViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanTPFilletv2? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanTPFilletv2> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanTPFilletv2? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanTPFilletv2ViewModel()
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

        public static PhieuCanTPFilletv2ViewModel Instance => instance ??= new PhieuCanTPFilletv2ViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetsLast<T>(dateTime, num);
        }

        public List<T> GetsLastMinutes<T>(int minu)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetsLastMinutes<T>(minu);
        }

        public Tuple<int, decimal> GetSoRoTongTrongLuongByNhanVienId(DateTime dateTime, string nhanVienId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetSoRoTongTrongLuongByNhanVienId(dateTime, nhanVienId);
        }
        public List<T> GetChiTiets2HN<T>(DateTime fromDate, DateTime toDate, string xuongId, bool isDinhMucBinhThuong = true)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetChiTiets2HN<T>(fromDate, toDate, xuongId, isDinhMucBinhThuong);
        }
        public List<T> GetChiTiets2HNByMaNhanVien<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId, bool isDinhMucBinhThuong = true)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetChiTiets2HNByMaNhanVien<T>(fromDate, toDate, maNhanVien, xuongId, isDinhMucBinhThuong);
        }
        public List<T> GetChiTiets2HNByMaHoSo<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId, bool isDinhMucBinhThuong = true)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetChiTiets2HNByMaHoSo<T>(fromDate, toDate, maHoSo, xuongId, isDinhMucBinhThuong);
        }
        public List<T> GetChiTiets2HNByMeThe<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId, bool isDinhMucBinhThuong = true)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetChiTiets2HNByMaThe<T>(fromDate, toDate, maThe, xuongId, isDinhMucBinhThuong);
        }
        public List<T> GetTongHopLoaiThanhPhamsHN<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            bool isDinhMucBinhThuong = true,
            bool isCaTraChuyenDoi = true,
            bool isFloor = true)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopLoaiThanhPhamsHN<T>(
                fromDate,
                toDate,
                xuongId,
                isDinhMucBinhThuong,
                isCaTraChuyenDoi,
                isFloor);
        }
        public List<T> GetTongHopThanhPhamByMaNhanViens<T>(
            DateTime fromDate,
            DateTime toDate,
            string maNhanVien,
            string xuongId,
            bool isDinhMucBinhThuong = true,
            bool isCaTraChuyenDoi = true,
            bool isFloor = true)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopThanhPhamByMaNhanViens<T>(
                fromDate,
                toDate,
                maNhanVien,
                xuongId,
                isDinhMucBinhThuong,
                isCaTraChuyenDoi,
                isFloor);
        }
        public List<T> GetTongHopThanhPhamByMaThes<T>(
            DateTime fromDate,
            DateTime toDate,
            string maThe,
            string xuongId,
            bool isDinhMucBinhThuong = true,
            bool isCaTraChuyenDoi = true,
            bool isFloor = true)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopThanhPhamByMaThes<T>(
                fromDate,
                toDate,
                maThe,
                xuongId,
                isDinhMucBinhThuong,
                isCaTraChuyenDoi,
                isFloor);
        }
        public List<T> GetTongHopThanhPhamByMaHoSos<T>(
            DateTime fromDate,
            DateTime toDate,
            string maHoSo,
            string xuongId,
            bool isDinhMucBinhThuong = true,
            bool isCaTraChuyenDoi = true,
            bool isFloor = true)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopThanhPhamByMaHoSos<T>(
                fromDate,
                toDate,
                maHoSo,
                xuongId,
                isDinhMucBinhThuong,
                isCaTraChuyenDoi,
                isFloor);
        }
        public List<T> GetTongHopNhanViensHN<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopNhanViensHN<T>(fromDate, toDate, xuongId);
        }

        public List<T> GetTongHopNhanViensHN2<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopNhanViensHN2<T>(fromDate, toDate, xuongId, isDinhMucBinhThuong, isFloor);
        }
        public List<T> GetTongHopNhanVien2HoangLong<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopNhanVien2HoangLong<T>(fromDate, toDate, xuongId, isDinhMucBinhThuong, isFloor);
        }

       public List<T> GetDataGopTheoMaThanhPham<T>(
    DateTime fromDate,
    DateTime toDate,
    string xuongId,
    bool isDinhMucBinhThuong = true,
    bool isFloor = true)
{
    var rawData = GetTongHopNhanVien2HoangLong<object>(fromDate, toDate, xuongId, isDinhMucBinhThuong, isFloor);
    var data = rawData.Cast<dynamic>().ToList();

    var grouped = data
        .GroupBy(x => new { x.MaThanhPham, x.MaNhanVien,x.NhanVienName})
        .Select(g =>
        {
            var first = g.First();
            var dict = new Dictionary<string, object>();

            dict["MaThanhPham"] = g.Key.MaThanhPham;
            dict["MaNhanVien"] = g.Key.MaNhanVien;
            dict["NhanVienName"] = g.Key.NhanVienName;
            dict["ThanhPhamName"] = first.ThanhPhamName;
            dict["XuongId"] = first.XuongId;
            dict["Nhom"] = first.Nhom;
            dict["LoaiCaName"] = first.LoaiCaName;
            dict["TongDauNhan"] = g.Sum(x => (double?)x.TongDauNhan ?? 0);
            dict["TongDauTra"] = g.Sum(x => (double?)x.TongDauTra ?? 0);
            dict["TongRotNhan"] = g.Sum(x => (double?)x.TongRotNhan ?? 0);
            dict["TongRotTra"] = g.Sum(x => (double?)x.TongRotTra ?? 0);
            dict["TongSoDauTra"] = g.Sum(x => (double?)x.TongSoDauTra ?? 0);
            dict["TongSoRotTra"] = g.Sum(x => (double?)x.TongSoRotTra ?? 0);
            dict["TongNhan"] = g.Sum(x => (double?)x.TongNhan ?? 0);
            dict["TongTra"] = g.Sum(x => (double?)x.TongTra ?? 0);
            dict["TongSoTra"] = g.Sum(x => (double?)x.TongSoTra ?? 0);
            dict["DinhMucChuan"] = g.Sum(x => (double?)x.DinhMucChuan ?? 0);
            dict["DinhMucDau"] = g.Sum(x => (double?)x.DinhMucDau ?? 0);
            dict["DinhMucRot"] = g.Sum(x => (double?)x.DinhMucRot ?? 0);
            dict["DinhMucThucTe"] = g.Sum(x => (double?)x.DinhMucThucTe ?? 0);
            dict["TongThoiGian"] = g.Sum(x => (double?)x.TongThoiGian ?? 0);
            dict["ThoiGianVao"] = first.ThoiGianVao;
            dict["ThoiGianRa"] = first.ThoiGianRa;

            return dict;
        })
        .Cast<T>()
        .ToList();

    return grouped;
}

        public List<T> GetTongHopTyLeThoiGianVaDinhMuc<T>(DateTime fromDate, DateTime toDate, string xuongId, int MocThoiGian)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopTyLeThoiGianVaDinhMuc<T>(fromDate, toDate, xuongId, MocThoiGian);
        }
        public List<T> GetTongHopTyLeThoiGianVaDinhMucForGrid<T>(DateTime fromDate, DateTime toDate, string xuongId, int MocThoiGian)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopTyLeThoiGianVaDinhMucForGrid<T>(fromDate, toDate, xuongId, MocThoiGian);
        }
        public List<T> GetTongHopThanhPhamFillet<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopThanhPham2<T>(fromDate, toDate, xuongId);
        }

        public List<T> GetTongHopDinhMucTheoSanPham<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopDinhMucTheoSanPham<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopNangSuatNhom<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopNangSuatNhom<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopLo<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopLo<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopNhanVienPhucVu<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopNhanVienPhucVu<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopGioLamViec<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopGioLamViec<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopDinhMucSanLuongTheoNhom<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopDinhMucSanLuongTheoNhom<T>(fromDate, toDate, xuongId);
        }

        public List<T> GetTongHopDinhMucSanLuongTheoThanhPham<T>(
            DateTime dateTime,
            string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopDinhMucSanLuongTheoThanhPham<T>(dateTime, xuongId);
        }
        public List<T> GetTongSanLuongTP<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongSanLuongTP<T>(fromDate, toDate, xuongId);
        }

        public List<T> GetTongHopSanLuongAndDinhMucThanhPhamTheoChuyen<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopSanLuongAndDinhMucThanhPhamTheoChuyen<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetSanLuongTBTPFillet<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetSanLuongTBTPFillet<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetDinhMucLangDa<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetDinhMucLangDa<T>(fromDate, toDate, xuongId);
        }
        public PhieuCanTPFilletv2 CopyItem(PhieuCanTPFilletv2 item)
        {
            return new PhieuCanTPFilletv2
            {
                TrongLuongNhan = item.TrongLuongNhan,
                TrongLuongTare = item.TrongLuongTare,
                CaTra = item.CaTra,
                GhiChu = item.GhiChu,
                Gio = item.Gio,
                MaLo = item.MaLo,
                MaLoaiCa = item.MaLoaiCa,
                MaMau = item.MaMau,
                MaMayCan = item.MaMayCan,
                MaNhanVien = item.MaNhanVien,
                MaNhanVienPhucVu = item.MaNhanVienPhucVu,
                MaSize = item.MaSize,
                MaThanhPham = item.MaThanhPham,
                MaThe = item.MaThe,
                MaUserCan = item.MaUserCan,
                MaXuong = item.MaXuong,
                Ngay = item.Ngay,
                STT = item.STT,
                DinhMucThucTe = item.DinhMucThucTe,
                DinhMucYeuCau = item.DinhMucYeuCau,
                MaMayCanBTP = item.MaMayCanBTP,
                STTBTP = item.STTBTP,
                TrongLuongTra = item.TrongLuongTra,
                MaBan = item.MaBan,
                ThePhieuSanLuongId = item.ThePhieuSanLuongId
            };
        }
        public PhieuCanTPFilletv2 CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public PhieuCanTPFilletv2 CreateDefaultNew()
        {
            var stt = 0;
            if (Items.Any())
                stt = Items.Select(x => Math.Abs(x.STT)).DefaultIfEmpty(0).Max() + 1;
            else
                stt = 1;

            return new PhieuCanTPFilletv2
            {
                STT = stt,
                CaTra = false,
                Gio = DateTime.Now.TimeOfDay,
                //MaLo = LoViewModel.Instance.ItemsWithSize?.FirstOrDefault()?.Item2,
                MaLoaiCa = LoaiCaFilletViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaMau = MauFilletViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaMayCan = AppViewModel.Instance.PCName,
                MaSize = SizeFilletViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaThanhPham = ThanhPhamFilletViewModel.Instance.Items.FirstOrDefault()?.Ma,
                MaThe = "0",
                //MaUserCan = AppViewModel.Instance.UserName,
                MaXuong = XiNghiepViewModel.Instance.SelectedItem?.Ma,
                Ngay = AppViewModel.Instance.DateTimeNow,
                TrongLuongTare = 0,
                TrongLuongTra = 0,
                STTBTP = 0,
                DinhMucThucTe = 0,
                DinhMucYeuCau = 0,
                MaMayCanBTP = AppViewModel.Instance.PCName,
                TrongLuongNhan = 0
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(PhieuCanTPFilletv2 item)
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
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.Gets<T>();
        }

        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(PhieuCanTPFilletv2 item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<PhieuCanTPFilletv2>();
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
        private bool IsItemPass(PhieuCanTPFilletv2 item)
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
                 item.TrongLuongTra > 0 &&
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

            var items = Gets<PhieuCanTPFilletv2>();
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
        private void Reload_(ObservableRangeCollection<PhieuCanTPFilletv2> obj)
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
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(PhieuCanTPFilletv2 item)
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
        #region XLPC FILLETV2
        public List<T> GetPhieuCanTPFilletv2_XLPC<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2(connStr);
            return dao.GetPhieuCanTPFilletv2_XLPC<T>(dateTime, xuongId);
        }
        #endregion
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCanTPFillet_XLPC<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2(connStr);
            return dao.GetPhieuCanTPFillet_XLPC<T>(dateTime, xuongId);
        }
        public List<T> GetDanhSachNhanVienBanTrongLuongByThePhieuSLFillet<T>(string maBan, string idThePhieuSLFillet, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2(connStr);
            return dao.GetDanhSachNhanVienBanTrongLuongByThePhieuSLFillet<T>(maBan, idThePhieuSLFillet);
        }
        #endregion
        #region Tính Luong Phụ Fillet
        public double GetSanLuongTPFilletv2(TimeSpan fromTime, TimeSpan toTime, DateTime dateTime, string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2(connStr);
                return dao.GetSanLuongv2(fromTime, toTime, dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuongTPFilletTruCaMuoi(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2(connStr);
                return dao.GetSanLuongTruCaMuoi(fromTime, toTime, dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetPhieuCanTongHopNhanVienPhucVusv2<T>(DateTime dateTime, string sanPhamId, string xuongId,
            bool isServer = false, string? connStr = null)
        {
            var connectionString = connStr;
            if (isServer) connectionString = connStr;

            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2(connectionString);
            return dao.GetTongHopNhanVienPhucVusv2<T>(dateTime, sanPhamId, xuongId);
        }
        public double GetSanLuongTPFillet(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> ids, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2(connStr);
                return dao.GetSanLuong(fromTime, toTime, dateTime, xuongId, ids);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Tính Lương Filletv2
        public List<T> GetPhieuCanTongHopTinhLuongsType1_2<T>(DateTime dateTime, string xuongId, string khuVucId, string? connStr = null)
        {
            var phieuCanDao = new Dao.Repos.HQ.PhieuCanTPFilletv2(connStr);
            return phieuCanDao.GetPhieuCanTongHopTinhLuongType1_2<T>(dateTime, xuongId, khuVucId);
        }
        public List<T> GetPhieuCanTongHopTinhLuongs<T>(DateTime dateTime, string xuongId, string khuVucId, string? connStr = null)
        {
            var phieuCanDao = new Dao.Repos.HQ.PhieuCanTPFilletv2(connStr);
            return phieuCanDao.GetPhieuCanTongHopTinhLuong<T>(dateTime, xuongId, khuVucId);
        }
        public List<BravoModelV1.Model.FilletV2_PhieuCanTongHopTinhLuong> LoadTongHopTinhLuong(DateTime dateTime, string xuongId)
        {
            try
            {
                var phieuCanTongHopTinhLuongs = new List<BravoModelV1.Model.FilletV2_PhieuCanTongHopTinhLuong>();

                try
                {
                    var items = new List<BravoModelV1.Model.FilletV2_PhieuCanTongHopTinhLuong>();
                    if (1 == 1) //VmApp.Future.Filletv212 kiểm tra quyền Filletv212
                        items = GetPhieuCanTongHopTinhLuongsType1_2<BravoModelV1.Model.FilletV2_PhieuCanTongHopTinhLuong>(dateTime, xuongId, "09");
                    else
                        items = GetPhieuCanTongHopTinhLuongs<BravoModelV1.Model.FilletV2_PhieuCanTongHopTinhLuong>(dateTime, xuongId, "09");

                    //var all = items.Where(x => x.DinhMucThucTe <= x.DinhMucYeuCau)
                    //    .All(
                    //        x =>
                    //        {
                    //            x.DanhGia = true;
                    //            return true;
                    //        });
                    var isHeSoRotTrongLuong = true;
                    var donGias = DG_DonGiaViewModel.Instance.Gets<DG_DonGia>(dateTime, @"DG", true);
                    var itemtonghoptinhluongs = (from i in from item in items
                                                           from tp in ThanhPhamFilletViewModel.Instance.Items
                                                           where item.MaThanhPham == tp.Ma
                                                           select new { item, tp.BravoId }
                                                 join dg in donGias on new { i.BravoId, i.item.DanhGia } equals new
                                                 {
                                                     BravoId = dg.MaSanPham,
                                                     DanhGia = (bool)dg.DanhGia
                                                 } into gj
                                                 from jItem in gj.DefaultIfEmpty()
                                                 select new BravoModelV1.Model.FilletV2_PhieuCanTongHopTinhLuong
                                                 {
                                                     CaTra = i.item.DanhGia,
                                                     DanhGia = i.item.DanhGia,
                                                     DinhMucThucTe = i.item.DinhMucThucTe,
                                                     DinhMucThucTeOrg = i.item.DinhMucThucTeOrg,
                                                     DinhMucYeuCau = i.item.DinhMucYeuCau,
                                                     LoaiCaName = i.item.LoaiCaName,
                                                     MaHoSo = i.item.MaHoSo,
                                                     MaNhanVien = i.item.MaNhanVien,
                                                     MaThanhPham = i.item.MaThanhPham,
                                                     MaThanhPhamOrg = i.item.MaThanhPhamOrg,
                                                     SizeName = i.item.SizeName,
                                                     SoRo = i.item.SoRo,
                                                     SoRoOrg = i.item.SoRoOrg,
                                                     TenNhanVien = i.item.TenNhanVien,
                                                     ThanhPhamName = i.item.ThanhPhamName,
                                                     ThanhPhamNameOrg = i.item.ThanhPhamNameOrg,
                                                     TrongLuongNhan = i.item.TrongLuongNhan,
                                                     TrongLuongNhanOrg = i.item.TrongLuongNhanOrg,
                                                     TrongLuongTra =
                                                          (isHeSoRotTrongLuong
                                                              ? jItem?.IsUsedHeSoRot == true
                                                                  ? i.item.DanhGia
                                                                      ? i.item.TrongLuongTra * jItem?.HeSo
                                                                      : i.item.TrongLuongTra * jItem?.HeSoRot
                                                                  : i.item.TrongLuongTra
                                                              : i.item.TrongLuongTra).Value,
                                                     TrongLuongTraOrg = i.item.TrongLuongTraOrg,
                                                     TyLe = i.item.TyLe,
                                                     DonGia = jItem?.DonGia ?? 0,
                                                     ThanhTien =
                                                          (isHeSoRotTrongLuong
                                                              ? jItem?.IsUsedHeSoRot == true
                                                                  ? i.item.DanhGia
                                                                      ? i.item.TrongLuongTra * jItem?.HeSo
                                                                      : i.item.TrongLuongTra * jItem?.HeSoRot
                                                                  : i.item.TrongLuongTra
                                                              : i.item.TrongLuongTra).Value *
                                                          (jItem?.DonGia ?? 0) *
                                                          (jItem?.HeSo ?? 1),
                                                     IsChamCong =
                                                          BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime)
                                                              .FirstOrDefault(x => x == i.item.MaNhanVien) ==
                                                          null
                                                              ? false
                                                              : true,
                                                     BarvoId = i.BravoId,
                                                     MaXuong = i.item.MaXuong,
                                                     CaLamViec = i.item.CaLamViec,
                                                     MaSanPham = i.BravoId,
                                                     Status = i.item.Status,
                                                     BanName = i.item.BanName,
                                                     SoBanDuocSap = i.item.SoBanDuocSap
                                                 }).ToList();
                    //PhieuCanTongHopTinhLuongs.Clear();
                    phieuCanTongHopTinhLuongs = new List<BravoModelV1.Model.FilletV2_PhieuCanTongHopTinhLuong>(items);


                }
                catch (Exception ex)
                {
                    //Application.Current.Dispatcher?.Invoke(
                    //    () => { exStr = ex.Message; });
                    ////throw;
                    return new List<BravoModelV1.Model.FilletV2_PhieuCanTongHopTinhLuong>();
                }

                return phieuCanTongHopTinhLuongs.ToList();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                //throw;
                return new List<BravoModelV1.Model.FilletV2_PhieuCanTongHopTinhLuong>();
            }
        }
        public List<T> GetsTPSoft<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2(connStr);
            return dao.GetsTPSoft<T>(dateTime, xuongId);
        }

        #endregion
        public List<dynamic> GetsChiTietThangDinhMuc(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var items = new List<object>();
            var phieuCans = GetChiTiets2HN<dynamic>(fromDate, toDate, xuongId, true);

            if (phieuCans.Any())
            {
                var _listOfDate = phieuCans.Select(x => x.Ngay).Distinct().ToList();
                if (_listOfDate.Any())
                {
                    var listOfDate = _listOfDate.Cast<DateTime>().ToList();
                    var donGias = DG_DonGiaViewModel.Instance.Gets<dynamic>(listOfDate).Where(x => x.MaLoaiDonGia == "DM")
                        .ToList();

                    //foreach (var p in phieuCans)
                    //{
                    //    foreach (var dg in donGias)
                    //    {
                    //        var rl = decimal.Compare((decimal)p.DinhMuc, (decimal)dg.DinhMucDown);
                    //        var rl2 = decimal.Compare((decimal)p.DinhMuc, (decimal)dg.DinhMucUp);
                    //        var rl3 =( (DateTime)p.Ngay).Date == dg.NgayDonGia.Date;
                    //        var rl4 = dg.MaLoaiDonGia.ToString() == "DM";
                    //        var rl5 = p.MaThanhPham.ToString() == dg.MaThanhPham.ToString();
                    //        var rl6 = p.MaSize.ToString() == dg.MaSizeFillet.ToString();
                    //    }
                    //}
                    var _items = (from p in phieuCans
                                  from dg in donGias
                                  where ((DateTime)p.Ngay).Date == dg.NgayDonGia.Date && (string)dg.MaLoaiDonGia == "DM" &&
                                        (string)p.MaThanhPham == (string)dg.MaThanhPham &&
                                        (string)p.MaSize == (string)dg.MaSizeFillet &&
                                        decimal.Compare((decimal)p.DinhMuc, (decimal)dg.DinhMucDown) >= 0 &&
                                        decimal.Compare((decimal)p.DinhMuc, (decimal)dg.DinhMucUp) <= 0
                                  select new
                                  {
                                      MaNhanVien = (string)p.MaNhanVien,
                                      MaHoSo = (string)p.MaHoSo,
                                      NhanVienName = (string)p.TenNhanVien,
                                      Ngay = (DateTime)p.Ngay,
                                      TrongLuong = (decimal)p.TrongLuongTra,
                                      SanPhamName = $@"{(decimal)dg.DinhMucDown}|{(string)dg.SanPhamName}",
                                      DinhMuc = (decimal)p.DinhMuc,
                                      SizeName = (string)p.SizeName,
                                      ThanhPhamName = (string)p.ThanhPhamName
                                  }).Cast<object>().ToList();
                    return _items;
                }
            }

            return items;
        }

        public List<dynamic> GetsTongHopThangDinhMuc(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var listOfItems = GetsChiTietThangDinhMuc(fromDate, toDate, xuongId);
                if (listOfItems.Any())
                {
                    var items = listOfItems
                        .GroupBy(x => new
                        {
                            x.MaNhanVien,
                            x.MaHoSo,
                            x.NhanVienName,
                            x.Ngay,
                            x.SizeName,
                            x.ThanhPhamName,
                            x.SanPhamName
                        })
                        .Select(x => new
                        {
                            x.Key.MaNhanVien,
                            x.Key.MaHoSo,
                            x.Key.NhanVienName,
                            x.Key.SanPhamName,
                            x.Key.Ngay,
                            x.Key.SizeName,
                            x.Key.ThanhPhamName,
                            TrongLuong = x.Sum(g => (decimal)g.TrongLuong)
                        }).Cast<dynamic>().ToList();
                    return items;
                }

                return new List<dynamic>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public List<T> GetTongHopNhanViens<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
        )
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopNhanViens<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopNhanViens2<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
        )
        {
            var dao = new Dao.Repos.HQ.PhieuCanTPFilletv2();
            return dao.GetTongHopNhanViens2<T>(fromDate, toDate, xuongId);
        }
    }
}
