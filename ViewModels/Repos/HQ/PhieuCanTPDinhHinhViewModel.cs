using System.Collections.ObjectModel;
using System.Windows.Input;
using AppViewModels;
using BravoModelV1.EF;
using BravoModelV1.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Repos.Models;
using MvvmHelpers;
using Vars.Hubs;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using PhieuCanDinhHinh = Models.Repos.AppModel.PhieuCanDinhHinh;
using PhieuCanKiemDinhHinh = BravoModelV1.Model.PhieuCanKiemDinhHinh;

namespace ViewModels.Repos.HQ;

public partial class PhieuCanTPDinhHinhViewModel : ObservableObject
{
    private static PhieuCanTPDinhHinhViewModel instance;
    private readonly SynchronizationContext synchronizationContext;
    [ObservableProperty] private ICommand _closeItemWindowCommand;
    [ObservableProperty] private bool _isWindowItemShown;
    [ObservableProperty] private ObservableRangeCollection<PhieuCanTongHopTinhLuong> _phieuCanTongHopTinhLuongs = new();
    [ObservableProperty] private ObservableCollection<SanLuongNhanVienKiem> _sanLuongNhanVienKiems = new();
    [ObservableProperty] private bool idItemIsReadOnly = true;
    [ObservableProperty] private bool isAdd;
    [ObservableProperty] private bool isEdit;
    [ObservableProperty] private PhieuCanTPDinhHinh? item;
    [ObservableProperty] private ObservableRangeCollection<PhieuCanTPDinhHinh> items = new();

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsVailSelectedItem))]
    private PhieuCanTPDinhHinh? selectedItem;

    [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();

    private PhieuCanTPDinhHinhViewModel()
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

    public static PhieuCanTPDinhHinhViewModel Instance => instance ??= new PhieuCanTPDinhHinhViewModel();
    //[RelayCommand(CanExecute = nameof(IsItemPass))]
    //private void Insert_(PhieuCanTPDinhHinh item)
    //{
    //    try
    //    {
    //        if (Insert(item) > 0)
    //        {
    //            var items = new List<PhieuCanTPDinhHinh>();
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

    private AppViewModel VmApp => AppViewModel.Instance;
    private MessageViewModel VmMessage => MessageViewModel.Instance;

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

    public int ChuyenXuong(List<PhieuCanTPDinhHinh> items, string xuongId, string? connStr = null)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh(connStr);
        return dao.ChuyenXuong(items, xuongId);
    }

    //[RelayCommand]
    //private void Reload_(ObservableRangeCollection<PhieuCanTPDinhHinh> obj)
    //{
    //    try
    //    {
    //        Reload();
    //    }
    //    catch (Exception e)
    //    {
    //        Console.WriteLine(e);
    //        //throw;
    //        VmMessage.SetExceptionCommand.Execute(e);
    //    }
    //}
    public DataCoummunication Convert(PhieuCanTPDinhHinh item)
    {
        return new DataCoummunication
        {
            TheId = item.MaThe,
            IdIn = item.IdIn,
            MaLo = item.MaLo,
            MaNhanVien = item.MaNhanVien,
            MaSize = item.MaSize,
            MaThanhPham = item.MaThanhPham,
            MayCan = item.MaMayCan,
            NgayGio = $"{item.Ngay:yyyyMMdd}{item.Gio:HHmmss}",
            TrongLuong = item.TrongLuongTra,
            MaNhanVienPhucVu = item.MaNhanVienPhucVu,
            TrongLuongTare = item.TrongLuongTare,
            IdOut = item.Id,
            DinhMuc = item.DinhMucThucTe
        };
    }

    public PhieuCanTPDinhHinh CopyItem(PhieuCanTPDinhHinh item)
    {
        return new PhieuCanTPDinhHinh
        {
            //SuDung = item.SuDung,
            //Ma = item?.Ma,
            //Ten = item?.Ten
        };
    }

    public PhieuCanTPDinhHinh CopySelectedItem()
    {
        return CopyItem(SelectedItem);
    }

    public PhieuCanTPDinhHinh CreateDefaultNew()
    {
        //var maxId = Items.Where(x => int.TryParse(x.Ma, out var rl))
        //       .Select(x => int.Parse(x.Ma))
        //       .DefaultIfEmpty(0)
        //       .Max();
        //var id = $"{(maxId + 1).ToString()}";
        return new PhieuCanTPDinhHinh
        {
            //SuDung = true,
            //Ma = id
        };
    }

    public PhieuCanTPDinhHinh CreateDefaultNew(string mayCan, PhieuCanBTPDinhHinh phieuCan, DataCoummunication data)
    {
        var stt = 0;
        if (Items.Any())
            stt = Items.Where(x => x.MaMayCan == mayCan).Select(x => Math.Abs(x.STT)).DefaultIfEmpty(0).Max() + 1;
        else
            stt = 1;
        return new PhieuCanTPDinhHinh
        {
            Id = data.IdOut,
            IdIn = data.IdIn,
            MaMayCan = mayCan,
            MaNhanVien = data.MaNhanVien ?? "",
            MaSize = data.MaSize ?? "",
            MaThanhPham = data.MaThanhPham,
            MaLo = data.MaLo,
            MaThe = data.TheId ?? "",
            Ngay = DateTime.Now.Date,
            Gio = DateTime.Now.TimeOfDay,
            CaTra = false,
            ChiSanLuong = false,
            IsOffline = false,
            DinhMucThucTe = data.DinhMuc,
            DinhMucYeuCau = 0,
            GhiChu = "",
            MaLoaiCa = LoaiCaDinhHinhViewModel.Instance.Items.FirstOrDefault()?.Ma ?? "",
            MaMau = MauDinhHinhViewModel.Instance.Items.FirstOrDefault()?.Ma ?? "",
            STT = stt,
            SuDung = true,
            TrongLuongTare = data.TrongLuongTare,
            TrongLuongTra = data.TrongLuong,
            MaUserCan = VmApp.UserName,
            MaMayCanBTP = phieuCan.MaMayCan,
            MaXuong = phieuCan.MaXuong,
            STTBTP = phieuCan.STT,
            TrongLuongNhan = phieuCan.TrongLuong,
            MaNhanVienBanKiem = data.MaNhanVienKiem,
            MaNhanVienPhucVu = data.MaNhanVienPhucVu,
            TrongLuongBu = 0
        };
    }

    private int Delete<T>(T item)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.Delete(item);
    }

    public List<T> GetChiTietByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetChiTietByMaHoSos<T>(fromDate, toDate, maHoSo, xuongId);
    }

    public List<T> GetChiTietByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetChiTietByMaNhanViens<T>(fromDate, toDate, maNhanVien, xuongId);
    }

    public List<T> GetChiTietByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetChiTietByMaThes<T>(fromDate, toDate, maThe, xuongId);
    }

    public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetChiTiets<T>(fromDate, toDate, xuongId);
    }

    public List<T> GetChiTiets<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetChiTiets<T>(fromDate, toDate, xuongId, isDinhMucBinhThuong);
    }

    public List<T> GetNhanVienKhongLamViecs<T>(DateTime dateTime)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetNhanVienKhongLamViecs<T>(dateTime);
    }

    //[RelayCommand(CanExecute = nameof(IsItemPass))]
    //private void Update_(PhieuCanTPDinhHinh item)
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

    public List<T> GetPhieuCanDinhHinh_XLPC<T>(DateTime dateTime, string xuongId, bool isDinhMucBinhThuong = true,
        string? connStr = null)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh(connStr);
        return dao.GetPhieuCanDinhHinh_XLPC<T>(dateTime, xuongId, isDinhMucBinhThuong);
    }

    public List<PhieuCanDinhHinh> GetPhieuCanDinhHinhs(DateTime dateTime,
        List<PhieuCanTongHopTinhLuong> phieuCanTongHopTinhLuongs)
    {
        try
        {
            var items = new List<PhieuCanDinhHinh>();
            foreach (var phieuCanTongHopTinhLuong in phieuCanTongHopTinhLuongs)
            {
                var sanPhamBravo =
                    SanPhamBravoViewModel.Instance.SanPhamBravoes.SingleOrDefault(x =>
                        x.DaiThanhId == phieuCanTongHopTinhLuong.MaThanhPham);
                if (sanPhamBravo != null)
                {
                    var item = new PhieuCanDinhHinh
                    {
                        Ngay = dateTime,
                        MaNhanVien = phieuCanTongHopTinhLuong.MaNhanVien,
                        DinhMucYeuCau = phieuCanTongHopTinhLuong.DinhMucYeuCau,
                        DanhGia = phieuCanTongHopTinhLuong.DanhGia,
                        DinhMucThucTe = phieuCanTongHopTinhLuong.DinhMucThucTe,
                        SoRo = phieuCanTongHopTinhLuong.SoRo,
                        SuDung = true,
                        TrongLuongNhan = phieuCanTongHopTinhLuong.TrongLuongNhan,
                        TrongLuongTra = phieuCanTongHopTinhLuong.TrongLuongTra,
                        _Status = 0,
                        MaSanPham = sanPhamBravo.Id,
                        TenSanPham = sanPhamBravo.Name,
                        CaLamViec = "CT04"
                    };
                    items.Add(item);
                }
            }

            return items;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public List<PhieuCanKiemDinhHinh> GetPhieuCanKiemDinhHinhs(List<SanLuongNhanVienKiem> sanLuongNhanVienKiems,
        SanPhamBravo sanPhamBravo, int khuVucId, DateTime dateTime)
    {
        try
        {
            var items = new List<PhieuCanKiemDinhHinh>();
            foreach (var sanLuongNhanVienKiem in sanLuongNhanVienKiems)
                if (sanPhamBravo != null)
                {
                    var item = new PhieuCanKiemDinhHinh
                    {
                        MaNhanVien = sanLuongNhanVienKiem.NhanVienKiem.NhanVienDaiThanh.MaNhanVien,
                        TrongLuong = sanLuongNhanVienKiem.TrongLuongHuong,
                        Ngay = dateTime,
                        _Status = 0,
                        CaLamViec = "CT04",
                        TenSanPham = sanPhamBravo.Name,
                        MaSanPham = sanPhamBravo.Id,
                        KhuVuc = khuVucId.ToString("00")
                    };
                    items.Add(item);
                }

            return items;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public List<PhieuCanKiemDinhHinh> GetPhieuCanKiemDinhHinhs(DateTime dateTime, string bravoId,
        string? connStr = null)
    {
        try
        {
            var phieuCanDao = new Dao.Repos.HQ.PhieuCanKiemDinhHinh(connStr);
            var items = phieuCanDao.GetByDate<PhieuCanKiemDinhHinh>(dateTime, bravoId);
            return items;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public List<PhieuCanKiemDinhHinh> GetPhieuCanTinhLuongs(
        List<SanLuongTinhLuongXepKhuon> sanLuongs,
        int khuVucId, DateTime dateTime)
    {
        var phieuCans = new List<PhieuCanKiemDinhHinh>();
        foreach (var item in sanLuongs)
        {
            var Cas = CaViewModel.Instance.Gets();
            var congViecs = CongViecTinhLuongXepKhuonViewModel.Instance.Gets();
            var caId = Cas.SingleOrDefault(x => x.Ma == item.CaId).BravoId;
            var congViecName = congViecs.SingleOrDefault(x => x.Ma == item.MaCongViec)
                .Ten;

            phieuCans.Add(
                new PhieuCanKiemDinhHinh
                {
                    CaLamViec = caId,
                    MaNhanVien = item.MaNhanVien,
                    MaSanPham = item.BravoId,
                    Ngay = dateTime,
                    TenSanPham = congViecName,
                    TrongLuong = Math.Round(item.SanLuongHuong, 2),
                    _Status = 0,
                    KhuVuc = khuVucId.ToString("00")
                });
        }

        return phieuCans;
    }

    /// <summary>
    ///     Áp dụng tỷ lệ đậu quá mức = false
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dateTime"></param>
    /// <param name="xuongId"></param>
    /// <param name="khuVucId"></param>
    /// <param name="connStr"></param>
    /// <returns></returns>
    public List<T> GetPhieuCanTongHopTinhLuong<T>(DateTime dateTime, string xuongId, string khuVucId,
        string? connStr = null)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh(connStr);
        return dao.GetPhieuCanTongHopTinhLuong<T>(dateTime, xuongId, khuVucId);
    }

    /// <summary>
    ///     Áp dụng tỷ lệ đậu quá mức = true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dateTime"></param>
    /// <param name="xuongId"></param>
    /// <param name="khuVucId"></param>
    /// <param name="connStr"></param>
    /// <returns></returns>
    public List<T> GetPhieuCanTongHopTinhLuong2<T>(DateTime dateTime, string xuongId, string khuVucId,
        string? connStr = null)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh(connStr);
        return dao.GetPhieuCanTongHopTinhLuong2<T>(dateTime, xuongId, khuVucId);
    }
    //public List<T> GetTongHopLoaiThanhPhams<T>(
    //    DateTime fromDate,
    //    DateTime toDate,
    //    string xuongId)
    //{
    //    var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
    //    return dao.GetTongHopLoaiThanhPhams<T>(
    //        fromDate,
    //        toDate,
    //        xuongId);
    //} 
    //}

    public List<T> Gets<T>()
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.Gets<T>();
    }

    public List<T> Gets<T>(DateTime dateTime)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.Gets<T>(dateTime);
    }

    public List<T> GetSanLuongTBTPDinhHinh<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetSanLuongTBTPDinhHinh<T>(fromDate, toDate, xuongId);
    }

    public List<T> GetsLast<T>(DateTime dateTime, int num)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetsLast<T>(dateTime, num);
    }

    public List<T> GetsLastMinutes<T>(int minu)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetsLastMinutes<T>(minu);
    }

    public Tuple<int, decimal> GetSoRoTongTrongLuongByNhanVienId(DateTime dateTime, string nhanVienId)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetSoRoTongTrongLuongByNhanVienId(dateTime, nhanVienId);
    }

    // public List<T> GetsSanPhamTinhLuong<T>()
    //{
    //    var dao = new Dao.Repos.HQ.MaThanhPhamDinhHinh();
    //    return dao.GetsSanPhamTinhLuong<T>();
    //}
    public List<PMS_DataRecord> GetsTPSoft(DateTime dateTime, string xuongId, string? connStr = null)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh(connStr);
        return dao.GetsTPSoft<PMS_DataRecord>(dateTime, xuongId);
    }

    public List<T> GetTongHopDanhGiaDinhMucs<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = true,
        bool isFloor = true)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopDanhGiaDinhMucs<T>(
            fromDate,
            toDate,
            xuongId,
            isDinhMucBinhThuong,
            isCaTraChuyenDoi,
            isFloor);
    }

    public List<T> GetTongHopDinhMucSanLuongTheoNhom<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopDinhMucSanLuongTheoNhom<T>(fromDate, toDate, xuongId);
    }


    public List<T> GetTongHopDinhMucSanLuongTheoThanhPham<T>(
        DateTime dateTime,
        string xuongId)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopDinhMucSanLuongTheoThanhPham<T>(dateTime, xuongId);
    }

    public List<T> GetTongHopDinhMucTheoSanPham<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopDinhMucTheoSanPham<T>(fromDate, toDate, xuongId);
    }

    public List<T> GetTongHopHaoHut<T>(DateTime fromDate, DateTime toDate)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopHaoHut<T>(fromDate, toDate);
    }

    public List<T> GetTongHopLoaiThanhPhamByMaHoSos<T>(
        DateTime fromDate,
        DateTime toDate,
        string maHoSo,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = true,
        bool isFloor = true)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopLoaiThanhPhamByMaHoSos<T>(
            fromDate,
            toDate,
            maHoSo,
            xuongId,
            isDinhMucBinhThuong,
            isCaTraChuyenDoi,
            isFloor);
    }

    public List<T> GetTongHopLoaiThanhPhamByMaNhanViens<T>(
        DateTime fromDate,
        DateTime toDate,
        string maNhanVien,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = true,
        bool isFloor = true)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopLoaiThanhPhamByMaNhanViens<T>(
            fromDate,
            toDate,
            maNhanVien,
            xuongId,
            isDinhMucBinhThuong,
            isCaTraChuyenDoi,
            isFloor);
    }

    public List<T> GetTongHopLoaiThanhPhamByMaThes<T>(
        DateTime fromDate,
        DateTime toDate,
        string maThe,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = true,
        bool isFloor = true)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopLoaiThanhPhamByMaThes<T>(
            fromDate,
            toDate,
            maThe,
            xuongId,
            isDinhMucBinhThuong,
            isCaTraChuyenDoi,
            isFloor);
    }

    public List<T> GetTongHopLoaiThanhPhams<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = true,
        bool isFloor = true)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopLoaiThanhPhams<T>(
            fromDate,
            toDate,
            xuongId,
            isDinhMucBinhThuong,
            isCaTraChuyenDoi,
            isFloor);
    }

    public List<T> GetTongHopLos<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = true,
        bool isFloor = true)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopLos<T>(
            fromDate,
            toDate,
            xuongId,
            isDinhMucBinhThuong,
            isCaTraChuyenDoi,
            isFloor);
    }

    public List<T> GetTongHopNangSuatNhom<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopNangSuatNhom<T>(fromDate, toDate, xuongId);
    }

    public List<T> GetTongHopNhanVienBanKiems<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopNhanVienBanKiems<T>(fromDate, toDate, xuongId);
    }

    public List<T> GetTongHopNhanVienPhucVus<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopNhanVienPhucVus<T>(fromDate, toDate, xuongId);
    }

    public List<T> GetTongHopNhanViens<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isFloor = true)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopNhanViens<T>(fromDate, toDate, xuongId, isDinhMucBinhThuong, isFloor);
    }

    public List<T> GetTongHopSanLuongAndDinhMucThanhPhamTheoChuyen<T>(DateTime fromDate, DateTime toDate,
        string xuongId)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopSanLuongAndDinhMucThanhPhamTheoChuyen<T>(fromDate, toDate, xuongId);
    }

    public List<T> GetTongHopTyLeThoiGianVaDinhMuc<T>(DateTime fromDate, DateTime toDate, string xuongId,
        int MocThoiGian)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopTyLeThoiGianVaDinhMuc<T>(fromDate, toDate, xuongId, MocThoiGian);
    }

    public List<T> GetTongHopTyLeThoiGianVaDinhMucForGrid<T>(DateTime fromDate, DateTime toDate, string xuongId,
        int MocThoiGian)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopTyLeThoiGianVaDinhMucForGrid<T>(fromDate, toDate, xuongId, MocThoiGian);
    }

    public List<T> GetTongHopTyLeThoiGianVaDinhMucTheoNhom<T>(DateTime fromDate, DateTime toDate, string xuongId,
        int MocThoiGian1, int MocThoiGian2)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.GetTongHopTyLeThoiGianVaDinhMucTheoNhom<T>(fromDate, toDate, xuongId, MocThoiGian1, MocThoiGian2);
    }

    public int Insert<T>(T item)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.Insert(item);
    }

    public int Insert(List<PhieuCanKiemDinhHinh> phieuCans, string? connStr = null)
    {
        try
        {
            var phieuCanDao = new Dao.Repos.HQ.PhieuCanKiemDinhHinh(connStr);
            var rows = phieuCanDao.Insert(phieuCans);
            return rows;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public int Insert2(List<PhieuCanKiemDinhHinh> phieuCans, string? connStr = null)
    {
        try
        {
            var phieuCanDao = new Dao.Repos.HQ.PhieuCanKiemDinhHinh(connStr);
            var rows = phieuCanDao.Insert(phieuCans);
            return rows;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public int InsertTL(List<PhieuCanDinhHinh> phieuCanDinhHinhs, string? connStr = null)
    {
        try
        {
            var dao = new Dao.Repos.HQ.PhieuCanDinhHinh(connStr);
            var rows = dao.Insert(phieuCanDinhHinhs);
            return rows;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public List<PhieuCanTongHopTinhLuong> LoadPhieuCanTongHopTinhLuong(DateTime dateTime, string xuongId,
        string khuVucId)
    {
        try
        {
            PhieuCanTongHopTinhLuongs.Clear();
            BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime);
            var items = new List<PhieuCanTongHopTinhLuong>();
            if (AppViewModel.Instance.ApDungTyLeDauRotQuaMuc)
                items = GetPhieuCanTongHopTinhLuong2<PhieuCanTongHopTinhLuong>(dateTime, xuongId, khuVucId);
            else
                items = GetPhieuCanTongHopTinhLuong<PhieuCanTongHopTinhLuong>(dateTime, xuongId, khuVucId);


            //Kiem Dinh Hinh

            #region Kiem Dinh Hinh

            var danhSachKiems = NhanVienViewModel.Instance.GetNhanViensKiems(xuongId, dateTime, 0);
            var toKiems = NhanVienViewModel.Instance.GetToKiemsByXuongId(xuongId);
            var nhanVienKiems = (from d in danhSachKiems
                from t in toKiems
                where d.ToId == t.Ma
                select new { d, t.MaHoSo }).ToList();
            var maHoSoIds = toKiems.Select(x => x.MaHoSo).Distinct();
            var sanLuongToKiems = items.Where(x => maHoSoIds.Contains(x.MaHoSo)).ToList();
            foreach (var item in maHoSoIds) items.RemoveAll(x => x.MaHoSo == item);

            foreach (var maHoSoKiem in maHoSoIds)
            {
                var nhanVienInToKiems = nhanVienKiems.Where(x => x.MaHoSo == maHoSoKiem).ToList();
                //var maHoSoInToKiemIds = nhanVienInToKiems.Select(x => x.MaHoSo).ToList();
                var sanLuongInToKiems = sanLuongToKiems.Where(x => x.MaHoSo == maHoSoKiem);
                if (sanLuongInToKiems != null && sanLuongInToKiems.Any())
                {
                    var sanPhams = sanLuongInToKiems.GroupBy(x => new
                        {
                            x.MaThanhPham,
                            x.ThanhPhamName,
                            x.DinhMucYeuCau,
                            x.LoaiCaName,
                            x.SizeName,
                            x.CaTra,
                            x.SoRo
                        })
                        .Select(x => new
                        {
                            x.Key.MaThanhPham,
                            x.Key.ThanhPhamName,
                            x.Key.DinhMucYeuCau,
                            x.Key.LoaiCaName,
                            x.Key.SizeName,
                            x.Key.CaTra,
                            x.Key.SoRo
                        })
                        .Distinct()
                        .ToList();
                    foreach (var sanPham in sanPhams)
                    {
                        var tongSoGioTyLe = nhanVienInToKiems.Select(x => x.d.SoGio * x.d.TyLe)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var tongSanLuongNhan = sanLuongInToKiems.Where(x => x.MaThanhPham == sanPham.MaThanhPham &&
                                                                            x.ThanhPhamName == sanPham.ThanhPhamName &&
                                                                            x.DinhMucYeuCau == sanPham.DinhMucYeuCau &&
                                                                            x.LoaiCaName == sanPham.LoaiCaName &&
                                                                            x.SizeName == sanPham.SizeName &&
                                                                            x.CaTra == sanPham.CaTra &&
                                                                            x.SoRo == sanPham.SoRo)
                            .Select(x => x.TrongLuongNhan)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var tongSanLuongTra = sanLuongInToKiems.Where(x => x.MaThanhPham == sanPham.MaThanhPham &&
                                                                           x.ThanhPhamName == sanPham.ThanhPhamName &&
                                                                           x.DinhMucYeuCau == sanPham.DinhMucYeuCau &&
                                                                           x.LoaiCaName == sanPham.LoaiCaName &&
                                                                           x.SizeName == sanPham.SizeName &&
                                                                           x.CaTra == sanPham.CaTra &&
                                                                           x.SoRo == sanPham.SoRo)
                            .Select(x => x.TrongLuongTra)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var trongLuongNhanTrenGio = tongSoGioTyLe == 0
                            ? 0
                            : tongSanLuongNhan / (decimal)tongSoGioTyLe;
                        var trongLuongTraTrenGio = tongSoGioTyLe == 0
                            ? 0
                            : tongSanLuongTra / (decimal)tongSoGioTyLe;
                        foreach (var nhanVienInToKiem in nhanVienInToKiems)
                        {
                            var phieuCanTongHopTinhLuong = new PhieuCanTongHopTinhLuong
                            {
                                CaTra = sanPham.CaTra,
                                DanhGia = false,
                                DinhMucYeuCau = sanPham.DinhMucYeuCau,
                                LoaiCaName = sanPham.LoaiCaName,
                                ThanhPhamName = sanPham.ThanhPhamName,
                                MaThanhPham = sanPham.MaThanhPham,
                                SizeName = sanPham.SizeName,
                                SoRo = 0,
                                TenNhanVien = nhanVienInToKiem.d.NhanVienDaiThanh.Name,
                                MaHoSo = nhanVienInToKiem.MaHoSo,
                                MaNhanVien = nhanVienInToKiem.d.NhanVienDaiThanh.MaNhanVien,
                                TrongLuongNhan =
                                    trongLuongNhanTrenGio *
                                    (decimal)(nhanVienInToKiem.d.SoGio * nhanVienInToKiem.d.TyLe),
                                TrongLuongTra =
                                    trongLuongTraTrenGio *
                                    (decimal)(nhanVienInToKiem.d.SoGio * nhanVienInToKiem.d.TyLe)
                            };
                            phieuCanTongHopTinhLuong.DinhMucThucTe = Math.Floor(
                                                                         phieuCanTongHopTinhLuong.TrongLuongTra == 0
                                                                             ? 0
                                                                             : phieuCanTongHopTinhLuong.TrongLuongNhan /
                                                                               phieuCanTongHopTinhLuong.TrongLuongTra *
                                                                               100) /
                                                                     100;
                            items.Add(phieuCanTongHopTinhLuong);
                        }
                    }
                }
            }

            #endregion Kiem Dinh Hinh

            #region Kiem So Che

            var danhSachKiemSoChes = BoTriNhomKiemSoCheViewModel.Instance.Gets(dateTime, xuongId);
            if (danhSachKiemSoChes == null || !danhSachKiemSoChes.Any())
                danhSachKiemSoChes = BoTriNhomKiemSoCheViewModel.Instance.Gets(new DateTime(2020, 03, 21), xuongId);

            var toKiemSoChes = NhomKiemSoCheViewModel.Instance.Gets(xuongId);
            var nhanVienKiemSoChes = (from d in danhSachKiemSoChes
                from t in toKiemSoChes
                where d.MaNhomKiem == t.Ma
                select new { d, t.MaHoSo }).ToList();
            var maHoSoSoCheIds = toKiemSoChes.Select(x => x.MaHoSo).Distinct();
            var sanLuongToKiemSoChes = items.Where(x => maHoSoSoCheIds.Contains(x.MaHoSo)).ToList();
            foreach (var item in maHoSoSoCheIds) items.RemoveAll(x => x.MaHoSo == item);

            foreach (var maHoSoKiem in maHoSoSoCheIds)
            {
                var nhanVienInToKiemSoChes = (from n in NhanVienViewModel.Instance.NhanVienDaiThanhs
                    from nk in nhanVienKiemSoChes
                    where n.MaNhanVien == nk.d.MaNhanVien && nk.MaHoSo == maHoSoKiem
                    select new { nk.d, nk.MaHoSo, TenNhanVien = n.Name }).ToList();
                //var nhanVienInToKiemSoChes = nhanVienKiemSoChes.Where(x => x.MaHoSo == maHoSoKiem).ToList();
                //var maHoSoInToKiemIds = nhanVienInToKiems.Select(x => x.MaHoSo).ToList();
                var sanLuongInToKiemSoChes =
                    sanLuongToKiemSoChes.Where(x => x.MaHoSo == maHoSoKiem);
                if (sanLuongInToKiemSoChes != null && sanLuongInToKiemSoChes.Any())
                {
                    var sanPhams = sanLuongInToKiemSoChes.GroupBy(x => new
                        {
                            x.MaThanhPham,
                            x.ThanhPhamName,
                            x.DinhMucYeuCau,
                            x.LoaiCaName,
                            x.SizeName,
                            x.CaTra,
                            x.SoRo
                        })
                        .Select(x => new
                        {
                            x.Key.MaThanhPham,
                            x.Key.ThanhPhamName,
                            x.Key.DinhMucYeuCau,
                            x.Key.LoaiCaName,
                            x.Key.SizeName,
                            x.Key.CaTra,
                            x.Key.SoRo
                        })
                        .Distinct()
                        .ToList();
                    foreach (var sanPham in sanPhams)
                    {
                        var tongSoGioTyLe = nhanVienInToKiemSoChes.Select(x => x.d.SoGio * x.d.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var tongSanLuongNhan = sanLuongInToKiemSoChes.Where(x => x.MaThanhPham == sanPham.MaThanhPham &&
                                x.ThanhPhamName == sanPham.ThanhPhamName &&
                                x.DinhMucYeuCau == sanPham.DinhMucYeuCau &&
                                x.LoaiCaName == sanPham.LoaiCaName &&
                                x.SizeName == sanPham.SizeName &&
                                x.CaTra == sanPham.CaTra &&
                                x.SoRo == sanPham.SoRo)
                            .Select(x => x.TrongLuongNhan)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var tongSanLuongTra = sanLuongInToKiemSoChes.Where(x => x.MaThanhPham == sanPham.MaThanhPham &&
                                x.ThanhPhamName == sanPham.ThanhPhamName &&
                                x.DinhMucYeuCau == sanPham.DinhMucYeuCau &&
                                x.LoaiCaName == sanPham.LoaiCaName &&
                                x.SizeName == sanPham.SizeName &&
                                x.CaTra == sanPham.CaTra &&
                                x.SoRo == sanPham.SoRo)
                            .Select(x => x.TrongLuongTra)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var trongLuongNhanTrenGio = tongSoGioTyLe == 0
                            ? 0
                            : tongSanLuongNhan / (decimal)tongSoGioTyLe;
                        var trongLuongTraTrenGio = tongSoGioTyLe == 0
                            ? 0
                            : tongSanLuongTra / (decimal)tongSoGioTyLe;
                        foreach (var nhanVienInToKiem in nhanVienInToKiemSoChes)
                        {
                            var phieuCanTongHopTinhLuong = new PhieuCanTongHopTinhLuong
                            {
                                CaTra = sanPham.CaTra,
                                DanhGia = false,
                                DinhMucYeuCau = sanPham.DinhMucYeuCau,
                                LoaiCaName = sanPham.LoaiCaName,
                                ThanhPhamName = sanPham.ThanhPhamName,
                                MaThanhPham = sanPham.MaThanhPham,
                                SizeName = sanPham.SizeName,
                                SoRo = 0,
                                TenNhanVien = nhanVienInToKiem.TenNhanVien,
                                MaHoSo = nhanVienInToKiem.MaHoSo,
                                MaNhanVien = nhanVienInToKiem.d.MaNhanVien,
                                TrongLuongNhan =
                                    trongLuongNhanTrenGio *
                                    (decimal)(nhanVienInToKiem.d.SoGio *
                                              nhanVienInToKiem.d.TyLeHuong),
                                TrongLuongTra =
                                    trongLuongTraTrenGio *
                                    (decimal)(nhanVienInToKiem.d.SoGio *
                                              nhanVienInToKiem.d.TyLeHuong)
                            };
                            phieuCanTongHopTinhLuong.DinhMucThucTe = Math.Floor(
                                                                         phieuCanTongHopTinhLuong.TrongLuongTra == 0
                                                                             ? 0
                                                                             : phieuCanTongHopTinhLuong.TrongLuongNhan /
                                                                               phieuCanTongHopTinhLuong.TrongLuongTra *
                                                                               100) /
                                                                     100;
                            items.Add(phieuCanTongHopTinhLuong);
                        }
                    }
                }
            }

            #endregion Kiem So Che

            #region Nhom So Che

            var danhSachNhomSoChes = BoTriNhomSoCheViewModel.Instance.Gets(dateTime, xuongId);
            //var id = danhSachNhomSoChes.Where(x => x.MaNhanVien == "000003603").ToList();
            if (danhSachNhomSoChes == null || !danhSachNhomSoChes.Any())
                danhSachNhomSoChes = BoTriNhomSoCheViewModel.Instance
                    .Gets(new DateTime(2020, 03, 21), xuongId);

            var nhomSoChes = NhomSoCheDinhHinhViewModel.Instance.Gets(xuongId, true);
            var nhanVienNhomSoChes = (from d in danhSachNhomSoChes
                from t in nhomSoChes
                where d.MaNhomSoChe == t.Ma
                select new { d, t.MaHoSo }).ToList();
            var maHoSoNhomSoCheIds = nhomSoChes.Select(x => x.MaHoSo).Distinct();
            var sanLuongNhomSoChes =
                items.Where(x => maHoSoNhomSoCheIds.Contains(x.MaHoSo)).ToList();
            foreach (var item in maHoSoNhomSoCheIds) items.RemoveAll(x => x.MaHoSo == item);

            foreach (var maHoSoNhomSoChe in maHoSoNhomSoCheIds)
            {
                var nhanVienInNhomSoChes = (from n in NhanVienViewModel.Instance.NhanVienDaiThanhs
                    from nk in nhanVienNhomSoChes
                    where n.MaNhanVien == nk.d.MaNhanVien && nk.MaHoSo == maHoSoNhomSoChe
                    select new { nk.d, nk.MaHoSo, TenNhanVien = n.Name }).ToList();
                //var nhanVienInToKiemSoChes = nhanVienKiemSoChes.Where(x => x.MaHoSo == maHoSoKiem).ToList();
                //var maHoSoInToKiemIds = nhanVienInToKiems.Select(x => x.MaHoSo).ToList();
                var sanLuongInNhomSoChes =
                    sanLuongNhomSoChes.Where(x => x.MaHoSo == maHoSoNhomSoChe);
                if (sanLuongInNhomSoChes != null && sanLuongInNhomSoChes.Any())
                {
                    var sanPhams = sanLuongInNhomSoChes.GroupBy(x => new
                        {
                            x.MaThanhPham,
                            x.ThanhPhamName,
                            x.DinhMucYeuCau,
                            x.LoaiCaName,
                            x.SizeName,
                            x.CaTra,
                            x.SoRo
                        })
                        .Select(x => new
                        {
                            x.Key.MaThanhPham,
                            x.Key.ThanhPhamName,
                            x.Key.DinhMucYeuCau,
                            x.Key.LoaiCaName,
                            x.Key.SizeName,
                            x.Key.CaTra,
                            x.Key.SoRo
                        })
                        .Distinct()
                        .ToList();
                    foreach (var sanPham in sanPhams)
                    {
                        var tongSoGioTyLe = nhanVienInNhomSoChes
                            .Select(x => x.d.SoGio * x.d.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var tongSanLuongNhan = sanLuongInNhomSoChes.Where(x => x.MaThanhPham == sanPham.MaThanhPham &&
                                                                               x.ThanhPhamName ==
                                                                               sanPham.ThanhPhamName &&
                                                                               x.DinhMucYeuCau ==
                                                                               sanPham.DinhMucYeuCau &&
                                                                               x.LoaiCaName == sanPham.LoaiCaName &&
                                                                               x.SizeName == sanPham.SizeName &&
                                                                               x.CaTra == sanPham.CaTra &&
                                                                               x.SoRo == sanPham.SoRo)
                            .Select(x => x.TrongLuongNhan)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var tongSanLuongTra = sanLuongInNhomSoChes.Where(x => x.MaThanhPham == sanPham.MaThanhPham &&
                                                                              x.ThanhPhamName ==
                                                                              sanPham.ThanhPhamName &&
                                                                              x.DinhMucYeuCau ==
                                                                              sanPham.DinhMucYeuCau &&
                                                                              x.LoaiCaName == sanPham.LoaiCaName &&
                                                                              x.SizeName == sanPham.SizeName &&
                                                                              x.CaTra == sanPham.CaTra &&
                                                                              x.SoRo == sanPham.SoRo)
                            .Select(x => x.TrongLuongTra)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var trongLuongNhanTrenGio = tongSoGioTyLe == 0
                            ? 0
                            : tongSanLuongNhan / (decimal)tongSoGioTyLe;
                        var trongLuongTraTrenGio = tongSoGioTyLe == 0
                            ? 0
                            : tongSanLuongTra / (decimal)tongSoGioTyLe;
                        foreach (var nhanVienInNhomSoChe in nhanVienInNhomSoChes)
                        {
                            var phieuCanTongHopTinhLuong = new PhieuCanTongHopTinhLuong
                            {
                                CaTra = sanPham.CaTra,
                                DanhGia = false,
                                DinhMucYeuCau = sanPham.DinhMucYeuCau,
                                LoaiCaName = sanPham.LoaiCaName,
                                ThanhPhamName = sanPham.ThanhPhamName,
                                MaThanhPham = sanPham.MaThanhPham,
                                SizeName = sanPham.SizeName,
                                SoRo = 0,
                                TenNhanVien = nhanVienInNhomSoChe.TenNhanVien,
                                MaHoSo = nhanVienInNhomSoChe.MaHoSo,
                                MaNhanVien = nhanVienInNhomSoChe.d.MaNhanVien,
                                TrongLuongNhan =
                                    trongLuongNhanTrenGio *
                                    (decimal)(nhanVienInNhomSoChe.d.SoGio *
                                              nhanVienInNhomSoChe.d.TyLeHuong),
                                TrongLuongTra =
                                    trongLuongTraTrenGio *
                                    (decimal)(nhanVienInNhomSoChe.d.SoGio *
                                              nhanVienInNhomSoChe.d.TyLeHuong)
                            };
                            phieuCanTongHopTinhLuong.DinhMucThucTe = Math.Floor(
                                                                         phieuCanTongHopTinhLuong.TrongLuongTra == 0
                                                                             ? 0
                                                                             : phieuCanTongHopTinhLuong.TrongLuongNhan /
                                                                               phieuCanTongHopTinhLuong.TrongLuongTra *
                                                                               100) /
                                                                     100;
                            //var id = danhSachNhomSoChes.Where(x => x.MaNhanVien == "000003603").ToList();

                            items.Add(phieuCanTongHopTinhLuong);
                        }
                    }
                }
            }

            #endregion Nhom So Che

            var danhSachNhomsXepKhuon = NhomXepKhuonViewModel.Instance.GetNhanVienNhomXepKhuons(dateTime);
            if (danhSachNhomsXepKhuon == null || !danhSachNhomsXepKhuon.Any())
                danhSachNhomsXepKhuon =
                    NhomXepKhuonViewModel.Instance.GetNhanVienNhomXepKhuons(new DateTime(2020, 05, 29));

            var nhomIds = danhSachNhomsXepKhuon.Select(x => x.MaNhom).Distinct().ToList();
            var sanLuongNhomXepKhuons = items.Where(x => nhomIds.Contains(x.MaHoSo)).ToList();
            foreach (var item in nhomIds)
            {
                items.RemoveAll(x => x.MaHoSo == item);
                var nhanVienInNhomXepKhuons = danhSachNhomsXepKhuon.Where(x => x.MaNhom == item)
                    .ToList();
                var nhanVienInNhomXepKhuonsFullInfo =
                    (from n in NhanVienViewModel.Instance.NhanVienDaiThanhs
                        from nvh in nhanVienInNhomXepKhuons
                        where nvh.MaNhanVien == n.MaNhanVien && !nhomIds.Contains(n.MaHoSo)
                        select n).ToList();
                //var nhanVienInToKiemSoChes = nhanVienKiemSoChes.Where(x => x.MaHoSo == maHoSoKiem).ToList();
                //var maHoSoInToKiemIds = nhanVienInToKiems.Select(x => x.MaHoSo).ToList();
                var sanLuongInNhomSoChes =
                    sanLuongNhomXepKhuons.Where(x => x.MaHoSo == item).ToList();
                if (sanLuongInNhomSoChes != null && sanLuongInNhomSoChes.Any())
                {
                    var sanPhams = sanLuongInNhomSoChes.GroupBy(x => new
                        {
                            x.MaThanhPham,
                            x.ThanhPhamName,
                            x.DinhMucYeuCau,
                            x.LoaiCaName,
                            x.SizeName,
                            x.CaTra,
                            x.SoRo
                        })
                        .Select(x => new
                        {
                            x.Key.MaThanhPham,
                            x.Key.ThanhPhamName,
                            x.Key.DinhMucYeuCau,
                            x.Key.LoaiCaName,
                            x.Key.SizeName,
                            x.Key.CaTra,
                            x.Key.SoRo
                        })
                        .Distinct()
                        .ToList();
                    foreach (var sanPham in sanPhams)
                    {
                        var tongSoGioTyLe = nhanVienInNhomXepKhuons
                            .Select(x => x.SoGio * x.TyLeHuong)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var tongSanLuongNhan = sanLuongInNhomSoChes.Where(x => x.MaThanhPham == sanPham.MaThanhPham &&
                                                                               x.ThanhPhamName ==
                                                                               sanPham.ThanhPhamName &&
                                                                               x.DinhMucYeuCau ==
                                                                               sanPham.DinhMucYeuCau &&
                                                                               x.LoaiCaName == sanPham.LoaiCaName &&
                                                                               x.SizeName == sanPham.SizeName &&
                                                                               x.CaTra == sanPham.CaTra &&
                                                                               x.SoRo == sanPham.SoRo)
                            .Select(x => x.TrongLuongNhan)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var tongSanLuongTra = sanLuongInNhomSoChes.Where(x => x.MaThanhPham == sanPham.MaThanhPham &&
                                                                              x.ThanhPhamName ==
                                                                              sanPham.ThanhPhamName &&
                                                                              x.DinhMucYeuCau ==
                                                                              sanPham.DinhMucYeuCau &&
                                                                              x.LoaiCaName == sanPham.LoaiCaName &&
                                                                              x.SizeName == sanPham.SizeName &&
                                                                              x.CaTra == sanPham.CaTra &&
                                                                              x.SoRo == sanPham.SoRo)
                            .Select(x => x.TrongLuongTra)
                            .DefaultIfEmpty(0)
                            .Sum();
                        var trongLuongNhanTrenGio = tongSoGioTyLe == 0
                            ? 0
                            : tongSanLuongNhan / tongSoGioTyLe;
                        var trongLuongTraTrenGio = tongSoGioTyLe == 0
                            ? 0
                            : tongSanLuongTra / tongSoGioTyLe;
                        foreach (var nhanVienInNhomSoChe in nhanVienInNhomXepKhuons)
                        {
                            var nhanVien =
                                nhanVienInNhomXepKhuonsFullInfo.SingleOrDefault(x =>
                                    x.MaNhanVien == nhanVienInNhomSoChe.MaNhanVien);

                            if (nhanVien != null)
                            {
                                var phieuCanTongHopTinhLuong = new PhieuCanTongHopTinhLuong
                                {
                                    CaTra = sanPham.CaTra,
                                    DanhGia = false,
                                    DinhMucYeuCau = sanPham.DinhMucYeuCau,
                                    LoaiCaName = sanPham.LoaiCaName,
                                    ThanhPhamName = sanPham.ThanhPhamName,
                                    MaThanhPham = sanPham.MaThanhPham,
                                    SizeName = sanPham.SizeName,
                                    SoRo = 0,
                                    TenNhanVien = nhanVien.Name,
                                    MaHoSo = nhanVien.MaHoSo,
                                    MaNhanVien = nhanVienInNhomSoChe.MaNhanVien,
                                    TrongLuongNhan =
                                        trongLuongNhanTrenGio *
                                        nhanVienInNhomSoChe.SoGio * nhanVienInNhomSoChe.TyLeHuong,
                                    TrongLuongTra =
                                        trongLuongTraTrenGio *
                                        nhanVienInNhomSoChe.SoGio * nhanVienInNhomSoChe.TyLeHuong
                                };
                                phieuCanTongHopTinhLuong.DinhMucThucTe = Math.Floor(
                                                                             phieuCanTongHopTinhLuong.TrongLuongTra == 0
                                                                                 ? 0
                                                                                 : phieuCanTongHopTinhLuong
                                                                                     .TrongLuongNhan /
                                                                                 phieuCanTongHopTinhLuong
                                                                                     .TrongLuongTra *
                                                                                 100) /
                                                                         100;
                                //var id = danhSachNhomSoChes.Where(x => x.MaNhanVien == "000003603").ToList();

                                if (phieuCanTongHopTinhLuong.TrongLuongTra > 0)

                                    items.Add(phieuCanTongHopTinhLuong);
                            }
                        }
                    }
                }
            }


            var all = items.Where(x => x.DinhMucThucTe <= x.DinhMucYeuCau).All
            (x =>
            {
                x.DanhGia = true;
                return true;
            });
            var donGias = DG_DonGiaViewModel.Instance.Gets<DG_DonGia>(dateTime, @"DG", true);
            var itemtonghoptinhluongs = (from i in from item in items
                    from tp in ThanhPhamDinhHinhViewModel.Instance.Items
                    where item.MaThanhPham == tp.Ma
                    select new { item, tp.BravoId }
                join dg in donGias on new { i.BravoId, i.item.DanhGia } equals new
                {
                    BravoId = dg.MaSanPham, dg.DanhGia
                } into gj
                from jItem in gj.DefaultIfEmpty()
                select new PhieuCanTongHopTinhLuong
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
                    TrongLuongTra = i.item.TrongLuongTra,
                    TrongLuongTraOrg = i.item.TrongLuongTraOrg,
                    TyLe = i.item.TyLe,
                    DonGia = jItem?.DonGia ?? 0,
                    ThanhTien = i.item.TrongLuongTra * (jItem?.DonGia ?? 0) * (jItem?.HeSo ?? 1),
                    IsChamCong =
                        BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime)
                            .FirstOrDefault(x => x == i.item.MaNhanVien) == null
                            ? false
                            : true
                }).ToList();
            //var itemtonghoptinhluongs = (from i in (from item in items
            //                                        from tp in ThanhPhamDinhHinhViewModel.Instance.Items
            //                                        where item.MaThanhPham == tp.Ma
            //                                        select new { item, tp.BravoId })
            //                             join dg in donGias on new { i.BravoId, i.item.DanhGia } equals new
            //                             {
            //                                 BravoId = dg.MaSanPham,
            //                                 dg.DanhGia
            //                             } into gj
            //                             from jItem in gj.DefaultIfEmpty()
            //                             select new BravoModelV1.Model.PhieuCanTongHopTinhLuong
            //                             {
            //                                 CaTra = i.item.DanhGia,
            //                                 DanhGia = i.item.DanhGia,
            //                                 DinhMucThucTe = i.item.DinhMucThucTe,
            //                                 DinhMucThucTeOrg = i.item.DinhMucThucTeOrg,
            //                                 DinhMucYeuCau = i.item.DinhMucYeuCau,
            //                                 LoaiCaName = i.item.LoaiCaName,
            //                                 MaHoSo = i.item.MaHoSo,
            //                                 MaNhanVien = i.item.MaNhanVien,
            //                                 MaThanhPham = i.item.MaThanhPham,
            //                                 MaThanhPhamOrg = i.item.MaThanhPhamOrg,
            //                                 SizeName = i.item.SizeName,
            //                                 SoRo = i.item.SoRo,
            //                                 SoRoOrg = i.item.SoRoOrg,
            //                                 TenNhanVien = i.item.TenNhanVien,
            //                                 ThanhPhamName = i.item.ThanhPhamName,
            //                                 ThanhPhamNameOrg = i.item.ThanhPhamNameOrg,
            //                                 TrongLuongNhan = i.item.TrongLuongNhan,
            //                                 TrongLuongNhanOrg = i.item.TrongLuongNhanOrg,
            //                                 TrongLuongTra = i.item.TrongLuongTra,
            //                                 TrongLuongTraOrg = i.item.TrongLuongTraOrg,
            //                                 TyLe = i.item.TyLe,
            //                                 DonGia = jItem?.DonGia ?? 0,
            //                                 ThanhTien = i.item.TrongLuongTra * (jItem?.DonGia ?? 0) * (jItem?.HeSo ?? 1),
            //                                 IsChamCong = BV_BravoCheckInOutViewModel.Instance.EmployeeCodes.FirstOrDefault(x => x == i.item.MaNhanVien) == null ? false : true
            //                             }).ToList();


            PhieuCanTongHopTinhLuongs.Clear();
            PhieuCanTongHopTinhLuongs = new ObservableRangeCollection<PhieuCanTongHopTinhLuong>(itemtonghoptinhluongs);
            return items;
        }
        catch (Exception ex)
        {
            return new List<PhieuCanTongHopTinhLuong>();
            //Console.WriteLine(ex);
            //// throw;
            //MessageBox.Show(ex.Message);
        }
    }

    //private bool IsItemPass(PhieuCanTPDinhHinh item)
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

        var items = Gets<PhieuCanTPDinhHinh>();
        if (items.Any())
            lock (Items)
            {
                try
                {
                    //Items.AddRange(items);
                    foreach (var item in items) Items.Add(item);
                }
                catch (NotSupportedException e)
                {
                }
            }
    }

    public List<SanLuongNhanVienKiem> SanLuongNhanVienKiem(DateTime dateTime, string xuongId)
    {
        try
        {
            BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime);
            var nhanVienKiems = NhanVienViewModel.Instance.GetNhanViensKiems(xuongId, dateTime, 0);
            SanLuongNhanVienKiems.Clear();
            SanLuongNhanVienKiems =
                new ObservableCollection<SanLuongNhanVienKiem>(
                    NhanVienViewModel.Instance.GetSanLuongNhanVienKiems(nhanVienKiems));
            var nhanVienKiemsSangLuong = NhanVienViewModel.Instance.GetNhanViensKiems(xuongId, dateTime, 1);
            var sanLuongKiemNhoms =
                NhanVienViewModel.Instance.GetSanLuongKiemNhoms(nhanVienKiems, nhanVienKiemsSangLuong, dateTime,
                    xuongId);
            var sanPhamBravo = SanPhamBravoViewModel.Instance.GetSanPhamKiem();
            var donGia = DG_DonGiaViewModel.Instance.Gets2<DG_DonGia>(dateTime, sanPhamBravo?.Id);
            foreach (var sanLuongKiemNhom in sanLuongKiemNhoms)
            {
                var rl = SanLuongNhanVienKiems.Where(x => x.NhanVienKiem.ToId == sanLuongKiemNhom.ToKiem.Ma)
                    .All(x =>
                    {
                        x.TrongLuongPhanChia = sanLuongKiemNhom.TrongLuongTrenGio * x.SoGio;
                        x.TrongLuongNhom = sanLuongKiemNhom.TongSanLuong;
                        x.TrongLuongTrenGio = sanLuongKiemNhom.TrongLuongTrenGio;
                        x.TrongLuongHuong =
                            sanLuongKiemNhom.TrongLuongTrenGio *
                            x.SoGio *
                            x.PhanTramHuong -
                            sanLuongKiemNhom.TrongLuongTrenGio *
                            x.SoGio *
                            x.PhanTramHuong *
                            x.TyLeTru;
                        x.GioTyLe = x.SoGio * x.PhanTramHuong;
                        x.MaSanPham = sanPhamBravo?.Id;
                        x.DonGia = donGia?.DonGia ?? 0;
                        x.ThanhTien = (donGia?.DonGia ?? 0) *
                                      x.TrongLuongHuong *
                                      (donGia?.HeSo ?? 1);
                        x.IsChamCong =
                            BV_BravoCheckInOutViewModel.Instance.EmployeeCodes(dateTime)
                                .FirstOrDefault(e => e == x.NhanVienKiem.NhanVienDaiThanh.MaNhanVien) == null
                                ? false
                                : true;
                        return true;
                    });
            }

            //List<BravoModelV1.Model.SanLuongNhanVienKiem> items = new List<BravoModelV1.Model.SanLuongNhanVienKiem>(SanLuongNhanVienKiems);
            return SanLuongNhanVienKiems.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new List<SanLuongNhanVienKiem>();
            // throw;
            // MessageBox.Show(ex.Message);
        }
    }

    private int Update<T>(T item)
    {
        var dao = new Dao.Repos.HQ.PhieuCanTPDinhHinh();
        return dao.Update(item);
    }
}