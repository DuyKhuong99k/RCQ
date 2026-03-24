using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Models.Repos;
using ViewModels.Repos.HQ;
using ViewModels.Repos.Hubs.IServices;

namespace Services;

/// <summary>
///     Cầu nối đến các viewmodel cũ đã viết, có thể kết nối đến các interface mới
/// </summary>
public class MainViewModel : ObservableObject, IMainService
{
    private IUserService userService;

    /// <summary>
    ///     Cầu nối đến các viewmodel cũ đã viết, có thể kết nối đến các interface mới
    /// </summary>
    /// <param name="userService"></param>
    public MainViewModel()
    {
        _dbContext = new dbPMScontext();
        var datetime = VmApp.DateTimeNow;
        _dbContext.Database.SetConnectionString(Base.Ins.ConnectionString2);
        VmMessage.MessageBoxShow = MessageBoxShow;
        VmMessage.IsShow = true;
    }

    [Inject] public IJSRuntime JsRuntime { get; set; }

    public ThanhPhamPhuXepKhuonViewModel VmThanhPhamPhuXepKhuon => ThanhPhamPhuXepKhuonViewModel.Instance;

    public ThanhPhamPhuXepKhuon_ColorViewModel VmThanhPhamPhuXepKhuonColor =>
        ThanhPhamPhuXepKhuon_ColorViewModel.Instance;

    public AppViewModel VmApp => AppViewModel.Instance;

    public NhanVienViewModel VmNhanVien => NhanVienViewModel.Instance;

    public NhanVienTheoBanViewModel VmNhanVienTheoBan => NhanVienTheoBanViewModel.Instance;
    public PhieuCanChinhXepKhuonViewModel VmPhieuCanChinhXepKhuon => PhieuCanChinhXepKhuonViewModel.Instance;

    public PhieuCanBTPDinhHinhViewModel VmPhieuCanBTPDinhHinh => PhieuCanBTPDinhHinhViewModel.Instance;
    public PhieuCanBTPFilletv2ViewModel VmPhieuCanBtpFilletv2 => PhieuCanBTPFilletv2ViewModel.Instance;
    public PhieuCanTPFilletv2ViewModel VmPhieuCanTpFilletv2 => PhieuCanTPFilletv2ViewModel.Instance;
    public PhieuCanTPFilletViewModel VmPhieuCanTPFillet => PhieuCanTPFilletViewModel.Instance;
    public ThePhieuSanLuongFilletViewModel VmThePhieuSanLuongFillet => ThePhieuSanLuongFilletViewModel.Instance;
    public PhieuCanTPDinhHinhViewModel VmPhieuCanTPDinhHinh => PhieuCanTPDinhHinhViewModel.Instance;
    public TheViewModel VmThe => TheViewModel.Instance;
    public TheRoViewModel VmTheRo => TheRoViewModel.Instance;
    public ThanhPhamDinhHinh_ColorViewModel VmThanhPhamDinhHinhColor => ThanhPhamDinhHinh_ColorViewModel.Instance;
    public ThanhPhamDinhHinh_TyLeViewModel VmThanhPhamDinhHinhTyLe { get; }
    public ThanhPhamDinhHinh_TyLeViewModel VmThanhPhamTyLe => ThanhPhamDinhHinh_TyLeViewModel.Instance;
    public ThanhPhamFillet_ColorViewModel VmThanhPhamFilletColor => ThanhPhamFillet_ColorViewModel.Instance;
    public TheThanhPhamViewModel VmTheThanhPham => TheThanhPhamViewModel.Instance;

    public dbPMScontext _dbContext { get; set; }
    public ThanhPhamFilletViewModel VmThanhPhamFillet => ThanhPhamFilletViewModel.Instance;
    public SizeFilletViewModel VmSizeFillet => SizeFilletViewModel.Instance;

    public ThanhPhamDinhHinhViewModel VmThanhPhamDinhHinh => ThanhPhamDinhHinhViewModel.Instance;
    public SizeDinhHinhViewModel VmSizeDinhHinh => SizeDinhHinhViewModel.Instance;

    public ThanhPhamPhuPhamViewModel VmThanhPhamPhuPham => ThanhPhamPhuPhamViewModel.Instance;
    public SizePhuPhamViewModel VmSizePhuPham => SizePhuPhamViewModel.Instance;
    public PhuongTienVanChuyenPhuPhamViewModel VmPhuongTienPhuPham => PhuongTienVanChuyenPhuPhamViewModel.Instance;
    public NhaMuaHangPhuPhamViewModel VmNhaMuaHangPhuPham => NhaMuaHangPhuPhamViewModel.Instance;
    public ThanhPhamSoCheDinhHinhViewModel VmThanhPhamSoCheDinhHinh => ThanhPhamSoCheDinhHinhViewModel.Instance;

    public LoViewModel VmLo => LoViewModel.Instance;
    public XiNghiepViewModel VmXiNghiep => XiNghiepViewModel.Instance;

    public LoaiCaFilletViewModel VmLoaiCaFillet => LoaiCaFilletViewModel.Instance;
    public LoaiCaDinhHinhViewModel VmLoaiCaDinhHinh => LoaiCaDinhHinhViewModel.Instance;
    public MauFilletViewModel VmMauFillet => MauFilletViewModel.Instance;
    public LoaiCaPhuPhamViewModel VmLoaiCaPhuPham => LoaiCaPhuPhamViewModel.Instance;
    public MauDinhHinhViewModel VmMauDinhHinh => MauDinhHinhViewModel.Instance;

    public ColorCodeViewModel VmColorCode => ColorCodeViewModel.Instance;
    public MauPhuPhamViewModel VmMauPhuPham => MauPhuPhamViewModel.Instance;
    public MessageViewModel VmMessage => MessageViewModel.Instance;
    public NhanVienCongCuViewModel VmNhanVienCongCu => NhanVienCongCuViewModel.Instance;
    public MaThanhPhamChinhXepKhuonViewModel VmThanhPhamChinhXepKhuon => MaThanhPhamChinhXepKhuonViewModel.Instance;
    public ThanhPhamKHCViewModel VmThanhPhamKHCXepKhuon => ThanhPhamKHCViewModel.Instance;
    public MaSizeChinhXepKhuonViewModel VmSizeChinhXepKhuon => MaSizeChinhXepKhuonViewModel.Instance;
    public SizeKHCViewModel VmSizeKHCXepKhuon => SizeKHCViewModel.Instance;
   
    public MaChieuXaXepKhuonViewModel VmChieuXaXepKhuon => MaChieuXaXepKhuonViewModel.Instance;


    public SizePhuXepKhuonViewModel VmSizePhuXepKhuon => SizePhuXepKhuonViewModel.Instance;

    public DinhMucFilletViewModel VmDinhMucFillet => DinhMucFilletViewModel.Instance;
    public KhachHangXepKhuonViewModel VmKhachHangXepKhuon => KhachHangXepKhuonViewModel.Instance;
    public DinhMucDinhHinhViewModel VmDinhMucDinhHinh => DinhMucDinhHinhViewModel.Instance;
    public TQuyTrinhViewModel VmTQuyTrinh => TQuyTrinhViewModel.Instance;
    public PLCViewModel VmPLC => PLCViewModel.Instance;
    public PLCChiTietViewModel VmPLCChiTiet => PLCChiTietViewModel.Instance;
    public CoiLogsViewModel VmCoiLogs => CoiLogsViewModel.Instance;
    public CoiMonitorViewModel VmCoiMonitor => CoiMonitorViewModel.Instance;
    public ChatLuongXepKhuonViewModel VmChatLuong => ChatLuongXepKhuonViewModel.Instance;
    public NETXepKhuonViewModel VmNETXepKhuon => NETXepKhuonViewModel.Instance;
    public CongDoanXepKhuonViewModel VmCongDoanXepKhuon => CongDoanXepKhuonViewModel.Instance;
    public PhieuCanRaCoiViewModel VmPhieuCanRaCoi => PhieuCanRaCoiViewModel.Instance;
    public UserAreaViewModel VmUserArea => UserAreaViewModel.Instance;
    public NguoiDungViewModel VmNguoiDung => NguoiDungViewModel.Instance;
    public ThanhPhamNguyenLieuViewModel VmThanhPhamNguyenLieu => ThanhPhamNguyenLieuViewModel.Instance;
    public AoViewModel VmAo => AoViewModel.Instance;
    public NhaCungCapNguyenLieuViewModel VmNhaCungCapNguyenLieu => NhaCungCapNguyenLieuViewModel.Instance;

    public PhuongTienVanChuyenNguyenLieuViewModel VmPhuongTienNguyenLieu =>
        PhuongTienVanChuyenNguyenLieuViewModel.Instance;

    public LoaiCaNguyenLieuViewModel VmLoaiCaNguyenLieu => LoaiCaNguyenLieuViewModel.Instance;
    public SizeNguyenLieuViewModel VmSizeNguyenLieu => SizeNguyenLieuViewModel.Instance;
    public BanCatTietViewModel VmBanCatTiet => BanCatTietViewModel.Instance;
    public MauNguyenLieuViewModel VmMauNguyenLieu => MauNguyenLieuViewModel.Instance;
    public MauBlockXepKhuonViewModel VmMauBlockXepKhuon => MauBlockXepKhuonViewModel.Instance;
    public PhieuCanNguyenLieuViewModel VmPhieuCanNguyenLieu => PhieuCanNguyenLieuViewModel.Instance;
    public PhieuCanPhuXepKhuonViewModel VmPhieuCanPhuXepKhuon => PhieuCanPhuXepKhuonViewModel.Instance;
    public PhieuCanXepKhuonKHCViewModel VmPhieuCanXepKhuonKXL => PhieuCanXepKhuonKHCViewModel.Instance;
    public PhieuCanXepKhuonBlockViewModel VmPhieuCanXepKhuonBlock => PhieuCanXepKhuonBlockViewModel.Instance;
    public PhieuCanPhuPhamViewModel VmPhieuCanPhuPham => PhieuCanPhuPhamViewModel.Instance;
    public PhieuCanPhuPhamv2ViewModel VmPhieuCanPhuPhamv2 => PhieuCanPhuPhamv2ViewModel.Instance;
    public HQ_SizeViewModel VmSizeHq => HQ_SizeViewModel.Instance;
    public HQ_ThanhPhamViewModel VmThanhPhamHq => HQ_ThanhPhamViewModel.Instance;
    public HQ_LoaiNguyenLieuViewModel VmNguyenLieuHq => HQ_LoaiNguyenLieuViewModel.Instance;
    public HQ_LoViewModel VmLoHq => HQ_LoViewModel.Instance;
    public HQ_NhanVienViewModel VmNhanVienHq => HQ_NhanVienViewModel.Instance;
    public HQ_PhieuCanViewModel VmPhieuCanHq => HQ_PhieuCanViewModel.Instance;
    CoiViewModel IMainService.VmCoi => CoiViewModel.Instance;
    public AoViewModel VmAoNguyenLieu => AoViewModel.Instance;
    public TrongLuongCoiTheoThanhPhamViewModel VmTrongLuongCoiTheoThanhPham => TrongLuongCoiTheoThanhPhamViewModel.Instance;
    public LoaiCaXepKhuonViewModel VmLoaiCaXepKhuon => LoaiCaXepKhuonViewModel.Instance;

    public LogGhiNhanLoiCanViewModel VmLogGhiNhanLoiCan => LogGhiNhanLoiCanViewModel.Instance;
    public event Action<string> AlertRequested;

    private int MessageBoxShow(string message, string title, int button)
    {
        //var task = new Task(async () =>
        //{
        //    await JsRuntime.InvokeVoidAsync("alert", message);
        //});
        //task.RunSynchronously();
        //_ = Task.Run(async () =>
        //{
        //    await JsRuntime.InvokeVoidAsync("alert", message);
        //});
        try
        {
            AlertRequested(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //throw;
        }
       
        return 0;
    }
}