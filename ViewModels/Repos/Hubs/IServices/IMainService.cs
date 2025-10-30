using AppViewModels;
using Models.Repos;
using ViewModels.Repos.HQ;

namespace ViewModels.Repos.Hubs.IServices;

public interface IMainService
{
    public dbPMScontext _dbContext { get; set; }

    public AoViewModel VmAo { get; }

    public AppViewModel VmApp { get; }

    public BanCatTietViewModel VmBanCatTiet { get; }

    public ChatLuongXepKhuonViewModel VmChatLuong { get; }

    public MaChieuXaXepKhuonViewModel VmChieuXaXepKhuon { get; }

    public CoiLogsViewModel VmCoiLogs { get; }

    public CoiMonitorViewModel VmCoiMonitor { get; }

    public ColorCodeViewModel VmColorCode { get; }

    public DinhMucDinhHinhViewModel VmDinhMucDinhHinh { get; }

    public DinhMucFilletViewModel VmDinhMucFillet { get; }

    public KhachHangXepKhuonViewModel VmKhachHangXepKhuon { get; }

    public LoViewModel VmLo { get; }

    public LoaiCaDinhHinhViewModel VmLoaiCaDinhHinh { get; }

    public LoaiCaFilletViewModel VmLoaiCaFillet { get; }

    public LoaiCaNguyenLieuViewModel VmLoaiCaNguyenLieu { get; }

    public LoaiCaPhuPhamViewModel VmLoaiCaPhuPham { get; }

    public MauDinhHinhViewModel VmMauDinhHinh { get; }

    public MauFilletViewModel VmMauFillet { get; }

    public MauNguyenLieuViewModel VmMauNguyenLieu { get; }

    public MauPhuPhamViewModel VmMauPhuPham { get; }

    public MessageViewModel VmMessage { get; }

    public NguoiDungViewModel VmNguoiDung { get; }

    public NhaCungCapNguyenLieuViewModel VmNhaCungCapNguyenLieu { get; }

    public NhaMuaHangPhuPhamViewModel VmNhaMuaHangPhuPham { get; }

    public NhanVienViewModel VmNhanVien { get; }

    public NhanVienCongCuViewModel VmNhanVienCongCu { get; }

    public NhanVienTheoBanViewModel VmNhanVienTheoBan { get; }

    public PhieuCanBTPDinhHinhViewModel VmPhieuCanBTPDinhHinh { get; }

    public PhieuCanBTPFilletv2ViewModel VmPhieuCanBtpFilletv2 { get; }

    public PhieuCanChinhXepKhuonViewModel VmPhieuCanChinhXepKhuon { get; }

    public PhieuCanNguyenLieuViewModel VmPhieuCanNguyenLieu { get; }

    public PhieuCanPhuPhamViewModel VmPhieuCanPhuPham { get; }

    public PhieuCanPhuPhamv2ViewModel VmPhieuCanPhuPhamv2 { get; }

    public PhieuCanPhuXepKhuonViewModel VmPhieuCanPhuXepKhuon { get; }

    public PhieuCanRaCoiViewModel VmPhieuCanRaCoi { get; }

    public PhieuCanTPDinhHinhViewModel VmPhieuCanTPDinhHinh { get; }

    public PhieuCanTPFilletViewModel VmPhieuCanTPFillet { get; }

    public PhieuCanTPFilletv2ViewModel VmPhieuCanTpFilletv2 { get; }

    public PhieuCanXepKhuonKHCViewModel VmPhieuCanXepKhuonKXL { get; }

    public PhuongTienVanChuyenNguyenLieuViewModel VmPhuongTienNguyenLieu { get; }

    public PhuongTienVanChuyenPhuPhamViewModel VmPhuongTienPhuPham { get; }

    public PLCViewModel VmPLC { get; }

    public PLCChiTietViewModel VmPLCChiTiet { get; }

    public MaSizeChinhXepKhuonViewModel VmSizeChinhXepKhuon { get; }

    public SizeDinhHinhViewModel VmSizeDinhHinh { get; }

    public SizeFilletViewModel VmSizeFillet { get; }

    public SizeKHCViewModel VmSizeKHCXepKhuon { get; }

    public SizeNguyenLieuViewModel VmSizeNguyenLieu { get; }

    public SizePhuPhamViewModel VmSizePhuPham { get; }

    public SizePhuXepKhuonViewModel VmSizePhuXepKhuon { get; }

    public MaThanhPhamChinhXepKhuonViewModel VmThanhPhamChinhXepKhuon { get; }

    public ThanhPhamDinhHinhViewModel VmThanhPhamDinhHinh { get; }

    public ThanhPhamDinhHinh_ColorViewModel VmThanhPhamDinhHinhColor { get; }

    public ThanhPhamFilletViewModel VmThanhPhamFillet { get; }

    public ThanhPhamFillet_ColorViewModel VmThanhPhamFilletColor { get; }

    public ThanhPhamKHCViewModel VmThanhPhamKHCXepKhuon { get; }

    public ThanhPhamNguyenLieuViewModel VmThanhPhamNguyenLieu { get; }

    public ThanhPhamPhuPhamViewModel VmThanhPhamPhuPham { get; }

    public ThanhPhamPhuXepKhuonViewModel VmThanhPhamPhuXepKhuon { get; }

    public ThanhPhamPhuXepKhuon_ColorViewModel VmThanhPhamPhuXepKhuonColor { get; }

    public ThanhPhamSoCheDinhHinhViewModel VmThanhPhamSoCheDinhHinh { get; }

    public TheViewModel VmThe { get; }

    public ThePhieuSanLuongFilletViewModel VmThePhieuSanLuongFillet { get; }

    public TheRoViewModel VmTheRo { get; }

    public TheThanhPhamViewModel VmTheThanhPham { get; }

    public TQuyTrinhViewModel VmTQuyTrinh { get; }

    public UserAreaViewModel VmUserArea { get; }

    public XiNghiepViewModel VmXiNghiep { get; }
    public HQ_SizeViewModel VmSizeHq { get; }
    public HQ_ThanhPhamViewModel VmThanhPhamHq { get; }
    public HQ_LoaiNguyenLieuViewModel VmNguyenLieuHq { get; }
    public HQ_LoViewModel VmLoHq { get; }
    public HQ_NhanVienViewModel VmNhanVienHq { get; }
    public HQ_PhieuCanViewModel VmPhieuCanHq {get; }
    public event Action<string> AlertRequested;
    public CoiViewModel VmCoi {get;}
}