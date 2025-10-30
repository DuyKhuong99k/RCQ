using System.Windows.Input;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Repos.Models;
using MvvmHelpers;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace ViewModels.Repos.HQ;

public partial class PhieuCanChinhXepKhuonViewModel : ObservableObject
{
    private static PhieuCanChinhXepKhuonViewModel instance;
    private readonly SynchronizationContext synchronizationContext;
    [ObservableProperty] private ICommand _closeItemWindowCommand;
    [ObservableProperty] private bool _isWindowItemShown;
    [ObservableProperty] private bool idItemIsReadOnly = true;
    [ObservableProperty] private bool isAdd;
    [ObservableProperty] private bool isEdit;
    [ObservableProperty] private PhieuCanChinhXepKhuon? item;
    [ObservableProperty] private ObservableRangeCollection<PhieuCanChinhXepKhuon> items = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsVailSelectedItem))]
    private PhieuCanChinhXepKhuon? selectedItem;

    [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();

    private PhieuCanChinhXepKhuonViewModel()
    {
        try
        {
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
            VmMessage.SetExceptionCommand.Execute(e);
        }
    }

    public static PhieuCanChinhXepKhuonViewModel Instance => instance ??= new PhieuCanChinhXepKhuonViewModel();

    //[RelayCommand(CanExecute = nameof(IsItemPass))]
    //private void Insert_(PhieuCanChinhXepKhuon item)
    //{
    //    try
    //    {
    //        if (Insert(item) > 0)
    //        {
    //            var items = new List<PhieuCanChinhXepKhuon>();
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

    public PhieuCanChinhXepKhuon CopyItem(PhieuCanChinhXepKhuon item)
    {
        return new PhieuCanChinhXepKhuon
        {
            MaNhanVien = item.MaNhanVien,
            TrongLuongTare = item.TrongLuongTare,
            MaXuong = item.MaXuong,
            ThoiGianQuay = item.ThoiGianQuay,
            ChuyenXuong = item.ChuyenXuong,
            DaQuay = item.DaQuay,
            Forced = item.Forced,
            GhiChu = item.GhiChu,
            Gio = item.Gio,
            IdMonitor = item.IdMonitor,
            LuotQuay = item.LuotQuay,
            MaChatLuong = item.MaChatLuong,
            MaChieuXa = item.MaChieuXa,
            MaCoiChinh = item.MaCoiChinh,
            MaCoiTam = item.MaCoiTam,
            MaKhuVuc = item.MaKhuVuc,
            MaLo = item.MaLo,
            MaLoaiCa = item.MaLoaiCa,
            MaMau = item.MaMau,
            MaMayCan = item.MaMayCan,
            MaNhanVienPvPhanCo = item.MaNhanVienPvPhanCo,
            MaNhom = item.MaNhom,
            MaSizeChinh = item.MaSizeChinh,
            MaThanhPhamChinh = item.MaThanhPhamChinh,
            MaUserCan = item.MaUserCan,
            Ngay = item.Ngay,
            NgayBatDauQuay = item.NgayBatDauQuay,
            NgayNguyenLieu = item.NgayNguyenLieu,
            NgayRaCoi = item.NgayRaCoi,
            STT = item.STT,
            TaiChe = item.TaiChe,
            ThoiGianBatDauQuay = item.ThoiGianBatDauQuay,
            ThoiGianRaCoi = item.ThoiGianRaCoi,
            TrongLuong = item.TrongLuong,
            MayQuay = item.MayQuay
        };
    }

    public PhieuCanChinhXepKhuon CopySelectedItem()
    {
        return CopyItem(SelectedItem);
    }

    public PhieuCanChinhXepKhuon CreateDefaultNew()
    {
        //var maxId = Items.Where(x => int.TryParse(x.Ma, out var rl))
        //       .Select(x => int.Parse(x.Ma))
        //       .DefaultIfEmpty(0)
        //       .Max();
        //var id = $"{(maxId + 1).ToString()}";
        return new PhieuCanChinhXepKhuon
        {
            //SuDung = true,
            //Ma = id
        };
    }

    private int Delete<T>(T item)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.Delete(item);
    }

    public List<T> GetLiteReport<T>(string idMonitor)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.GetLiteReport<T>(idMonitor);
    }

    //[RelayCommand(CanExecute = nameof(IsItemPass))]
    //private void Update_(PhieuCanChinhXepKhuon item)
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

    public List<T> GetPhieuCan_XLPC<T>(DateTime dateTime, string xuongId, string? connStr = null)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon(connStr);
        return dao.GetPhieuCan_XLPC<T>(dateTime, xuongId);
    }

    #endregion

    public List<T> GetPhieuCanTongHopChinhXepKhuons<T>(DateTime fromDate, DateTime dateTime, string xuongId)
    {
        try
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetPhieuCanTongHopsXepKhuon<T>(fromDate, dateTime, xuongId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    //[RelayCommand(CanExecute = nameof(IsItemPass))]
    //private void Delete_(PhieuCanChinhXepKhuon item)
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
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.Gets<T>();
    }
    public List<T> Gets<T>(DateTime dateTime)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.Gets<T>(dateTime);
    }
    public List<T> GetsByIdMonitor<T>(string idMonitor)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.GetsByIdMonitor<T>(idMonitor);
    }

    private int Insert<T>(T item)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.Insert(item);
    }

    //private bool IsItemPass(PhieuCanChinhXepKhuon item)
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

        var items = Gets<PhieuCanChinhXepKhuon>();
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

    public List<T> GetsLast<T>(DateTime dateTime, int num)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.GetsLast<T>(dateTime, num);
    }
    public List<T> GetsTongHopCoiTam<T>(DateTime dataTime)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.GetsTongHopCoiTam<T>(dataTime);
    }

    public List<T> GetPhieuCanOnCoiTams<T>(DateTime dateTime)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.GetPhieuCanOnCoiTams<T>(dateTime);
    }
    public int SetRaCoiStateByIdMonitor(string idMonitor, TimeSpan thoiGianRaCoi, DateTime ngayRaCoi, bool forced)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.SetRaCoiStateByIdMonitor(idMonitor, thoiGianRaCoi, ngayRaCoi, forced);
    }
    public int SetQuayStateByIdMonitor(string idMonitor, bool daQuay, TimeSpan thoiGianBatDauQuay,
        DateTime ngayBatDauQuay, string mayQuay)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.SetQuayStateByIdMonitor(idMonitor, daQuay, thoiGianBatDauQuay, ngayBatDauQuay, mayQuay);
    }

    //[RelayCommand]
    //private void Reload_(ObservableRangeCollection<PhieuCanChinhXepKhuon> obj)
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

    public int Update<T>(T item)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.Update(item);
    }

    public int Update<T>(List<T> items)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.Update(items);
    }

    public int UpdateChatLuongByIdMonitor(string maChatLuong, string idMonitor)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.UpdateChatLuongByIdMonitor(maChatLuong, idMonitor);
    }

    #region Báo Cáo
    /// <summary>
    /// Báo Cáo Chi Tiết Chính Xếp Khuôn
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="xuongId"></param>
    /// <returns></returns>
    public List<T> GetPhieuCanChinhXepKhuonsByFromDateToDate<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        try
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetPhieuCanChinhXepKhuonsByFromDateToDate<T>(fromDate, toDate, xuongId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    public List<T> GetPhieuCanChinhXepKhuonsByMaNhanVien<T>(DateTime fromDate, DateTime toDate,string maNhanVien, string xuongId)
    {
        try
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetPhieuCanChinhXepKhuonsByMaNhanVien<T>(fromDate, toDate,maNhanVien, xuongId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    public List<T> GetPhieuCanChinhXepKhuonsByMaHoSo<T>(DateTime fromDate, DateTime toDate,string maHoSo, string xuongId)
    {
        try
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetPhieuCanChinhXepKhuonsByMaHoSo<T>(fromDate, toDate,maHoSo, xuongId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    public List<T> GetPhieuCanChinhXepKhuonsByMaThe<T>(DateTime fromDate, DateTime toDate,string maThe, string xuongId)
    {
        try
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetPhieuCanChinhXepKhuonsByMaThe<T>(fromDate, toDate,maThe, xuongId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    /// <summary>
    /// Báo Cáo Chi Tiết Nhân Viên Chính Xếp Khuôn
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="xuongId"></param>
    /// <returns></returns>
    public List<Models.Repos.AppModel.PhieuCanTongHopPhu> GetPhieuCanTongHopNhanVienFromdateToDates(DateTime fromDate, DateTime toDate, string xuongId)
    {
        try
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetPhieuCanTongHopNhanVienFromdateToDates<Models.Repos.AppModel.PhieuCanTongHopPhu>(fromDate, toDate, xuongId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    /// <summary>
    /// Báo Cáo Tổng hợp nhân viên chính xếp khuôn
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="xuongId"></param>
    /// <returns></returns>
    public List<Models.Repos.AppModel.PhieuCanTongHopPhu> GetPhieuCanTongHopNhanViens2FromDateToDate(DateTime fromDate, DateTime toDate, string xuongId)
    {
        try
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetPhieuCanTongHopNhanViens2FromDateToDate<Models.Repos.AppModel.PhieuCanTongHopPhu>(fromDate, toDate, xuongId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    /// <summary>
    /// Báo Cáo Tổng Hợp 2 CHính Xếp Khuôn (tổng hợp thành phẩm)
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="xuongId"></param>
    /// <returns></returns>
    public List<Models.Repos.AppModel.PhieuCanTongHop> GetPhieuCanTongHops2FromDateToDate(DateTime fromDate, DateTime toDate, string xuongId)
    {
        try
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetPhieuCanTongHops2FromDateToDate<Models.Repos.AppModel.PhieuCanTongHop>(fromDate, toDate, xuongId);
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
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
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
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
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
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetTongHopThanhPhamByMaThes<T>(fromDate, toDate, maThe, xuongId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }


    /// <summary>
    /// Báo Cáo Chi Tiết Cối
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="xuongId"></param>
    /// <returns></returns>
    public List<Models.Repos.AppModel.PhieuCanTongHopCoi> GetPhieuCanTongHopCoisFromDateToDate(DateTime fromDate, DateTime toDate, string xuongId)
    {
        try
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetPhieuCanTongHopCoisFromDateToDate<Models.Repos.AppModel.PhieuCanTongHopCoi>(fromDate, toDate, xuongId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    /// <summary>
    /// Báo Cáo Chi tiết cối chưa quay
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="xuongId"></param>
    /// <returns></returns>
    public List<Models.Repos.AppModel.PhieuCanTongHopCoi> GetPhieuCanTongHopCoisChuaQuayFromDateToDate(DateTime fromDate, DateTime toDate, string xuongId)
    {
        try
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetPhieuCanTongHopCoisChuaQuayFromDateToDate<Models.Repos.AppModel.PhieuCanTongHopCoi>(fromDate, toDate, xuongId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    /// <summary>
    /// Báo Cáo tổng hợp cối
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="xuongId"></param>
    /// <returns></returns>
    public List<Models.Repos.AppModel.PhieuCanTongHopCoi> GetPhieuCanTongHopCois2FromDateToDate(DateTime fromDate, DateTime toDate, string xuongId)
    {
        try
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetPhieuCanTongHopCois2FromDateToDate<Models.Repos.AppModel.PhieuCanTongHopCoi>(fromDate, toDate, xuongId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    /// <summary>
    /// Báo Cáo Tổng Hợp CHi Tiết Cối
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="xuongId"></param>
    /// <returns></returns>
    public List<Models.Repos.AppModel.PhieuCanTongHopChiTietCoi> GetPhieuCanTongHopChiTietCoisFromDateToDate(DateTime fromDate, DateTime toDate, string xuongId)
    {
        try
        {
            var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
            return dao.GetPhieuCanTongHopChiTietCoisFromDateToDate<Models.Repos.AppModel.PhieuCanTongHopChiTietCoi>(fromDate, toDate, xuongId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    /// <summary>
    /// Báo cáo tổng hợp cối xưởng
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    public List<T> GetTongHopCoisXuongFromDateToDate<T>(DateTime fromDate, DateTime toDate)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.GetTongHopCoisXuongFromDateToDate<T>(fromDate, toDate);
    }

    /// <summary>
    /// Báo Cáo thời gian sắp ra cối
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    public List<T> GetTongHopCoiGanRaFromDateToDate<T>(DateTime fromDate, DateTime toDate)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.GetTongHopCoiGanRaFromDateToDate<T>(fromDate, toDate);
    }

    /// <summary>
    /// Báo Cáo Tổng hợp thời gian lượt ra cối xưởng
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    public List<T> GetTongHopThoiGianLuotRaCoiFromDate<T>(DateTime fromDate, DateTime toDate)
    {
        var dao = new Dao.Repos.HQ.PhieuCanChinhXepKhuon();
        return dao.GetTongHopThoiGianLuotRaCoiFromDate<T>(fromDate, toDate);
    }

    #endregion
}