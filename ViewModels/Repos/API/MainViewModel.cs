using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Dao.Repos.HQ;
using Models.Repos.Models;
using ViewModels.Repos.HQ;

namespace ViewModels.Repos.API;

public class MainViewModel : ObservableObject
{
    private static MainViewModel instance;

    private MainViewModel()
    {
        var dateTime = VmApp.DateTimeNow;
        try
        {
            VmApp.ComName = VmXiNghiep.SelectedItem?.CodeId;
            DbUpdateDatabase();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            //throw;
        }
       
        VmMessage.MessageBoxShow = MessageBoxShow;
        VmApp.CheckServerOnline = CheckServerOnline ;
        
    }

    public static MainViewModel Instance => instance ??= new MainViewModel();
    public MessageViewModel VmMessage => MessageViewModel.Instance;
    public AppViewModel VmApp => AppViewModel.Instance;
    public BanCatTietViewModel VmBanCatTiet => BanCatTietViewModel.Instance;
    public BanFilletViewModel VmBanFillet => BanFilletViewModel.Instance;
    public BoTriLoSizeThanhPhamViewModel VmBoTriLoSizeThanhPham => BoTriLoSizeThanhPhamViewModel.Instance;
    public BoTriNhanVienTheoSizeViewModel VmBoTriNhanVienTheoSize => BoTriNhanVienTheoSizeViewModel.Instance;
    public BT_KhachHangViewModel VmBT_KhachHang => BT_KhachHangViewModel.Instance;
    public BT_NhanVienTheoNhomViewModel VmBT_NhanVienTheoNhom => BT_NhanVienTheoNhomViewModel.Instance;
    public BT_NhomTinhLuongViewModel VmBT_NhomTinhLuong => BT_NhomTinhLuongViewModel.Instance;
    //public BT_PhieuCanViewModel VmBT_PhieuCan => BT_PhieuCanViewModel.Instance;
    public BT_ThanhPhamViewModel VmBT_ThanhPham => BT_ThanhPhamViewModel.Instance;
    public CaViewModel VmCa => CaViewModel.Instance;
    public ChatLuongTaiCheViewModel VmChatLuongTaiChe => ChatLuongTaiCheViewModel.Instance;
    public ChatLuongXepKhuonBlockViewModel VmChatLuongXepKhuonBlock => ChatLuongXepKhuonBlockViewModel.Instance;
    public ChiTietRaCoiViewModel VmChiTietRaCoi => ChiTietRaCoiViewModel.Instance;
    public CoiViewModel VmCoi => CoiViewModel.Instance;
    public ColorCodeViewModel VmColorCode => ColorCodeViewModel.Instance;
    public CongDoanXepKhuonViewModel VmCongDoanXepKhuon => CongDoanXepKhuonViewModel.Instance;
    public CongViecTaiCheViewModel VmCongViecTaiChe => CongViecTaiCheViewModel.Instance;
    public DG_DonGiaViewModel VmDG_DonGia => DG_DonGiaViewModel.Instance;
    public DG_LoaiDonGiaViewModel VmDG_LoaiDonGia => DG_LoaiDonGiaViewModel.Instance;
    public DG_SanPhamTinhLuongViewModel VmDG_SanPhamTinhLuong => DG_SanPhamTinhLuongViewModel.Instance;
    public DinhMucDinhHinhViewModel VmDinhMucDinhHinh => DinhMucDinhHinhViewModel.Instance;
    public DinhMucFilletViewModel VmDinhMucFillet => DinhMucFilletViewModel.Instance;
    public DinhMucXepHangViewModel VmDinhMucXepHang => DinhMucXepHangViewModel.Instance;
    public KD_DonHangViewModel VmKD_DonHang => KD_DonHangViewModel.Instance;
    public KD_PhieuCanViewModel VmKD_PhieuCan => KD_PhieuCanViewModel.Instance;
    public KD_QuyCachViewModel VmKD_QuyCach => KD_QuyCachViewModel.Instance;
    public KD_SizeViewModel VmKD_Size => KD_SizeViewModel.Instance;
    public KD_ThanhPhamViewModel VmKD_ThanhPham => KD_ThanhPhamViewModel.Instance;
    public KD_TuiViewModel VmKD_Tui => KD_TuiViewModel.Instance;
    public KhachHangCaChetViewModel VmKhachHangCaChet => KhachHangCaChetViewModel.Instance;
    public KhachHangXepKhuonViewModel VmKhachHangXepKhuon => KhachHangXepKhuonViewModel.Instance;
    //public KhuVucKetChuyenViewModel VmKhuVucKetChuyen => KhuVucKetChuyenViewModel.Ins
    //public KhuVucViewModel VmKhuVuc => KhuVucViewModel.in
    public KhuVucXepKhuonViewModel VmKhuVucXepKhuon => KhuVucXepKhuonViewModel.Instance;
    public KNH_PhieuCanViewModel VmKNH_PhieuCan => KNH_PhieuCanViewModel.Instance;
    public KNH_QuyCachViewModel VmKNH_QuyCach => KNH_QuyCachViewModel.Instance;
    public KNH_SizeViewModel VmKNH_Size => KNH_SizeViewModel.Instance;
    public KNH_ThanhPhamViewModel VmKNH_ThanhPham => KNH_ThanhPhamViewModel.Instance;
    public KNH_ThongTinSanPhamViewModel VmKNH_ThongTinSanPham => KNH_ThongTinSanPhamViewModel.Instance;
    public LineFilletv2ViewModel VmLineFilletv2 => LineFilletv2ViewModel.Instance;
    public LoaiCaCaChetViewModel VmLoaiCaCaChet => LoaiCaCaChetViewModel.Instance;
    public LoaiCaCaoThitViewModel VmLoaiCaCaoThit => LoaiCaCaoThitViewModel.Instance;
    public LoaiCaDinhHinhViewModel VmLoaiCaDinhHinh => LoaiCaDinhHinhViewModel.Instance;
    public LoaiCaFilletViewModel VmLoaiCaFillet => LoaiCaFilletViewModel.Instance;
    public LoaiCaLangDaViewModel VmLoaiCaLangDa => LoaiCaLangDaViewModel.Instance;
    public LoaiCaNguyenLieuViewModel VmLoaiCaNguyenLieu => LoaiCaNguyenLieuViewModel.Instance;
    public LoaiCaPhuPhamViewModel VmLoaiCaPhuPham => LoaiCaPhuPhamViewModel.Instance;
    public LoaiCaSoCheDinhHinhViewModel VmLoaiCaSoCheDinhHinh => LoaiCaSoCheDinhHinhViewModel.Instance;
    public LoaiCaTaiCheViewModel VmLoaiCaTaiChe => LoaiCaTaiCheViewModel.Instance;
    public LoaiCaXepKhuonViewModel VmLoaiCaXepKhuon => LoaiCaXepKhuonViewModel.Instance;
    //public LoCaXepKhuonViewModel VmLoCaXepKhuon => LoCaXepKhuonViewModel.In
    public LogKetChuyenV2ViewModel VmLogKetChuyenV2 => LogKetChuyenV2ViewModel.Instance;
    public LogKetChuyenViewModel VmLogKetChuyen => LogKetChuyenViewModel.Instance;
    public LoTheoLineViewModel VmLoTheoLine => LoTheoLineViewModel.Instance;
    public AoVungNuoiViewModel VmAoVungNuoi => AoVungNuoiViewModel.Instance;
    public LoViewModel VmLo => LoViewModel.Instance;
    public MaChatLuongXepKhuonViewModel VmChatLuongXepKhuon => MaChatLuongXepKhuonViewModel.Instance;
    public MaChieuXaXepKhuonViewModel VmChieuXaXepKhuon => MaChieuXaXepKhuonViewModel.Instance;
    public MaCoiXepKhuonViewModel VmCoiXepKhuon => MaCoiXepKhuonViewModel.Instance;
    public MaLoiViewModel VmLoi => MaLoiViewModel.Instance;
    public MaSizeChinhXepKhuonViewModel VmMaSizeChinhXepKhuon => MaSizeChinhXepKhuonViewModel.Instance;
    public MauBlockXepKhuonViewModel VmMauBlockXepKhuon => MauBlockXepKhuonViewModel.Instance;
    public MauCaGiongVungNuoiViewModel VmMauCaGiongVungNuoi => MauCaGiongVungNuoiViewModel.Instance;
    public MauDinhHinhViewModel VmMauDinhHinh => MauDinhHinhViewModel.Instance;
    public MauFilletViewModel VmMauFillet => MauFilletViewModel.Instance;
    public MauLangDaViewModel VmmMauLangDa => MauLangDaViewModel.Instance;
    public MauNguyenLieuViewModel VmMauNguyenLieu => MauNguyenLieuViewModel.Instance;
    public MauPhuPhamViewModel VmMauPhuPham => MauPhuPhamViewModel.Instance;
    public MauTaiCheViewModel VmMauTaiChe => MauTaiCheViewModel.Instance;
    public MayLangDaViewModel VmMayLangDa => MayLangDaViewModel.Instance;
    public MocThoiGianPhanCaViewModel VmMocThoiGianPhanCa => MocThoiGianPhanCaViewModel.Instance;
    public MPG_CongThucChiTietViewModel VmMPG_CongThucChiTiet => MPG_CongThucChiTietViewModel.Instance;
    public MPG_CongThucViewModel VmMPG_CongThuc => MPG_CongThucViewModel.Instance;
    public MPG_CongThucXacNhanViewModel VmMPG_CongThucXacNhan => MPG_CongThucXacNhanViewModel.Instance;
    public MPG_PhieuCanViewModel VmMPG_PhieuCan => MPG_PhieuCanViewModel.Instance;
    public MPG_SanPhamViewModel VmMPG_SanPham => MPG_SanPhamViewModel.Instance;
    public NETXepKhuonViewModel VmNETXepKhuon => NETXepKhuonViewModel.Instance;
    public NguoiDung_QuyenViewModel VmNguoiDung_Quyen => NguoiDung_QuyenViewModel.Instance;
    public NguoiDungNhomQuyenViewModel VmNguoiDungNhomQuyen => NguoiDungNhomQuyenViewModel.Instance;
    public NguoiDungThongTinViewModel VmNguoiDungThongTin => NguoiDungThongTinViewModel.Instance;
    public NguyenLieu_TyLeNuocViewModel VmNguyenLieu_TyLeNuoc => NguyenLieu_TyLeNuocViewModel.Instance;
    public NhaCungCapNguyenLieuDonGiaVanChuyenViewModel VmNhaCungCapNguyenLieuDonGiaVanChuyen => NhaCungCapNguyenLieuDonGiaVanChuyenViewModel.Instance;
    public NhaCungCapNguyenLieuViewModel VmNhaCungCapNguyenLieu => NhaCungCapNguyenLieuViewModel.Instance;
    public NhaMuaHangPhuPhamViewModel VmNhaMuaHangPhuPham => NhaMuaHangPhuPhamViewModel.Instance;
    public NhanVienCongCuViewModel VmNhanVienCongCu => NhanVienCongCuViewModel.Instance;
    public NhanVienTheoBanViewModel VmNhanVienTheoban => NhanVienTheoBanViewModel.Instance;
    public NhanVienTheoLineViewModel VmNhanVienTheoLine => NhanVienTheoLineViewModel.Instance;
    public NhanVienViewModel VmNhanVien => NhanVienViewModel.Instance;
    public PD_CongThucViewModel VmPD_CongThuc => PD_CongThucViewModel.Instance;
    public PD_LyDoViewModel VmPD_LyDo => PD_LyDoViewModel.Instance;
    public PD_SanPhamViewModel VmPD_SanPham => PD_SanPhamViewModel.Instance;
    public PheuExViewModel VmPheuEx => PheuExViewModel.Instance;
    public PhieuCan_DH_XL_ViewModel VmPhieuCan_DH_XL => PhieuCan_DH_XL_ViewModel.Instance;
    public PhieuCan_Fillet_XL_ViewModel VmPhieuCan_Fillet_XL => PhieuCan_Fillet_XL_ViewModel.Instance;
    public PhieuCanBTPDinhHinhViewModel VmPhieuCanBTPDinhHinh => PhieuCanBTPDinhHinhViewModel.Instance;
    public PhieuCanBTPFilletv2ViewModel VmPhieuCanBTPFilletv2 => PhieuCanBTPFilletv2ViewModel.Instance;
    public PhieuCanCaChetDaiThanhSideViewModel VmPhieuCanCaChetDaiThanhSide => PhieuCanCaChetDaiThanhSideViewModel.Instance;
    public PhieuCanCaChetViewModel VmPhieuCanCaChet => PhieuCanCaChetViewModel.Instance;
    public PhieuCanCaGiongViewModel VmPhieuCanCaGiong => PhieuCanCaGiongViewModel.Instance;
    public PhieuCanCaoThitViewModel VmPhieuCanCaoThit => PhieuCanCaoThitViewModel.Instance;
    public PhieuCanChinhXepKhuonViewModel VmPhieuCanChinhXepKhuon => PhieuCanChinhXepKhuonViewModel.Instance;
    public PhieuCanDauAoViewModel VmPhieuCanDauAo => PhieuCanDauAoViewModel.Instance;
    public PhieuCanLangDaViewModel VmPhieuCanLangDa => PhieuCanLangDaViewModel.Instance;
    public PhieuCanNguyenLieuViewModel VmPhieuCanNguyenLieu => PhieuCanNguyenLieuViewModel.Instance;
    public PhieuCanPhaLocViewModel VmPhieuCanPhaLoc => PhieuCanPhaLocViewModel.Instance;
    public PhieuCanPhuPhamv2ViewModel VmPhieuCanPhuPhamv2 => PhieuCanPhuPhamv2ViewModel.Instance;
    public PhieuCanPhuPhamViewModel VmPhieuCanPhuPham => PhieuCanPhuPhamViewModel.Instance;
    //public PhieuCanRaDongViewModel
    public PhieuCanSauXepKhuonViewModel VmPhieuCanSauXepKhuon => PhieuCanSauXepKhuonViewModel.Instance;
    public PhieuCanTPDinhHinhViewModel VmPhieuCanTPDinhHinh => PhieuCanTPDinhHinhViewModel.Instance;
    public PhieuCanTPFilletv2ViewModel VmPhieuCanTPFilletv2 => PhieuCanTPFilletv2ViewModel.Instance;
    public PhieuCanTPFilletViewModel VmPhieuCanTPFillet => PhieuCanTPFilletViewModel.Instance;
    public PhuongTien_TrongLuongDauAoViewModel VmPhuongTien_TrongLuongDauAo => PhuongTien_TrongLuongDauAoViewModel.Instance;
    public PhuongTienTaiTrongViewModel VmPhuongTienTaiTrong => PhuongTienTaiTrongViewModel.Instance;
    public PhuongTienVanChuyenNguyenLieuViewModel VmPhuongTienVanChuyenNguyenLieu => PhuongTienVanChuyenNguyenLieuViewModel.Instance;
    public PhuongTienVanChuyenPhuPhamViewModel VmPhuongTienVanChuyenPhuPham => PhuongTienVanChuyenPhuPhamViewModel.Instance;

    public PhieuCanVungNuoiDaiThanhSideViewModel VmPhieuCanVungNuoiDaiThanhSide => PhieuCanVungNuoiDaiThanhSideViewModel.Instance;
    public PMS_LViewModel VmPMS_L => PMS_LViewModel.Instance;
    public QuyCachCaoThitViewModel VmQuyCachCaoThit => QuyCachCaoThitViewModel.Instance;
    //public RP_NangXuatDinhMucViewModel VmRP_NangXuatDinhMuc => RP_NangXuatDinhMucViewModel.
    public SizeBlockXepKhuonViewModel VmSizeBlockXepKhuon => SizeBlockXepKhuonViewModel.Instance;
    public SizeCaoThitViewModel VmSizeCaoThit => SizeCaoThitViewModel.Instance;
    public SizeChinhXepKhuonViewModel VmSizeChinhXepKhuon => SizeChinhXepKhuonViewModel.Instance;
    public SizeDinhHinhViewModel VmSizeDinhHinh => SizeDinhHinhViewModel.Instance;
    public SizeFilletViewModel VmSizeFillet => SizeFilletViewModel.Instance;
    public SizeKHCViewModel VmSizeKHC => SizeKHCViewModel.Instance;
    public SizeLangDaViewModel VmSizeLangDa => SizeLangDaViewModel.Instance;
    public SizeNguyenLieuViewModel VmSizeNguyenLieu => SizeNguyenLieuViewModel.Instance;
    public SizePhuPhamViewModel VmSizePhuPham => SizePhuPhamViewModel.Instance;
    public SizePhuXepKhuonViewModel VmSizePhuXepKhuon => SizePhuXepKhuonViewModel.Instance;
    public SizeTaiCheViewModel VmSizeTaiChe => SizeTaiCheViewModel.Instance;
    //public SyncViewModel VmSync => SyncViewModel.ReferenceEquals
    //public TBaoCaoViewModel VmT_BaoCao => TBaoCaoViewModel.
    public TBonViewModel VmT_Bon => TBonViewModel.Instance;
    public TChiTietBonViewModel VmT_ChiTietBon => TChiTietBonViewModel.Instance;
    public TCongDoanTheoThanhPhamViewModel VmT_CongDoanTheoThanhPham => TCongDoanTheoThanhPhamViewModel.Instance;
    public TCongDoanViewModel VmT_CongDoan => TCongDoanViewModel.Instance;
    public TDinhMucViewModel VmT_DinhMuc => TDinhMucViewModel.Instance;
    public TDonHangViewModel VmT_DonHang => TDonHangViewModel.Instance;
    public TGoupViewModel VmT_Goup => TGoupViewModel.Instance;
    public THoaChatViewModel VmT_HoaChat => THoaChatViewModel.Instance;
    public TKhachHangViewModel VmT_KhachHang => TKhachHangViewModel.Instance;
    public TKhangSinhViewModel VmT_KhangSinh => TKhangSinhViewModel.Instance;
    public TKhuVucViewModel VmT_KhuVuc => TKhuVucViewModel.Instance;
    public TLenhSanXuatViewModel VmT_LenhSanXuat => TLenhSanXuatViewModel.Instance;
    public TLoaiCanViewModel VmT_LoaiCan => TLoaiCanViewModel.Instance;
    public TLoaiKhuonViewModel VmT_LoaiKhuon => TLoaiKhuonViewModel.Instance;
    public TLoaiNguyenLieuViewModel VmT_LoaiNguyenLieu => TLoaiNguyenLieuViewModel.Instance;
    public TLoNguyenlieuViewModel VmT_LoNguyenLieu => TLoNguyenlieuViewModel.Instance;
    public TNhaCungCapViewModel VmT_NhaCungCap => TNhaCungCapViewModel.Instance;
    public TNhomHoaChatViewModel VmT_NhomHoaChat => TNhomHoaChatViewModel.Instance;
    public TNhomLoViewModel VmT_NhomLo => TNhomLoViewModel.Instance;
    public TPhieuCanViewModel VmT_PhieuCan => TPhieuCanViewModel.Instance;
    public TPhieuCanThuMuaViewModel VmT_PhieuCanThuMua => TPhieuCanThuMuaViewModel.Instance;
    public TPhieuPhanCoChiTietViewModel VmT_PhieuPhanCoChiTiet => TPhieuPhanCoChiTietViewModel.Instance;
    public TPhieuPhanCoViewModel VmT_PhieuPhanCo => TPhieuPhanCoViewModel.Instance;
    public TPhuGiaViewModel VmT_PhuGia => TPhuGiaViewModel.Instance;
    public TPhuongTienViewModel VmT_PhuongTien => TPhuongTienViewModel.Instance;
    public TQuyCachViewModel VmT_QuyCach => TQuyCachViewModel.Instance;
    public TQuyTrinhViewModel VmT_QuyTrinh => TQuyTrinhViewModel.Instance;
    public TQuyTrinhTheoNgayViewModel VmT_QuyTrinhTheoNgay => TQuyTrinhTheoNgayViewModel.Instance;
    public TSizeViewModel VmT_Size => TSizeViewModel.Instance;
    public TSanPhamViewModel VmT_SanPham => TSanPhamViewModel.Instance;
    public TThanhPhamViewModel VmT_ThanhPham => TThanhPhamViewModel.Instance;
    public TThongTinPhuViewModel VmT_ThongTinPhu => TThongTinPhuViewModel.Instance;
    public TTrangThaiNguyenLieuViewModel VmT_TrangThaiNguyenLieu => TTrangThaiNguyenLieuViewModel.Instance;
    public TTyLeHoaChatTheoNhomViewModel VmT_TyLeHoaChatTheoNhom => TTyLeHoaChatTheoNhomViewModel.Instance;
    public TareViewModel VmTare => TareViewModel.Instance;
    public ThanhPhamBlockViewModel VmThanhPhamBlock => ThanhPhamBlockViewModel.Instance;
    public ThanhPhamCaoThitViewModel VmThanhPhamCaoThit => ThanhPhamCaoThitViewModel.Instance;
    public ThanhPhamChinhXepKhuonViewModel VmThanhPhamChinhXepKhuon => ThanhPhamChinhXepKhuonViewModel.Instance;
    public ThanhPhamDinhHinh_ColorViewModel VmThanhPhamDinhHinh_Color => ThanhPhamDinhHinh_ColorViewModel.Instance;
    public ThanhPhamDinhHinh_TyLeViewModel VmThanhPhamDinhHinh_TyLe => ThanhPhamDinhHinh_TyLeViewModel.Instance;
    public ThanhPhamDinhHinhViewModel VmThanhPhamDinhHinh => ThanhPhamDinhHinhViewModel.Instance;
    public ThanhPhamFillet_ColorViewModel VmThanhPhamFillet_Color => ThanhPhamFillet_ColorViewModel.Instance;
    public ThanhPhamFillet_HanMucTrongLuongViewModel VmThanhPhamFillet_HanMucTrongLuong => ThanhPhamFillet_HanMucTrongLuongViewModel.Instance;
    public ThanhPhamFillet_ThanhPhamMacDinhViewModel VmThanhPhamFillet_ThanhPhamMacDinh => ThanhPhamFillet_ThanhPhamMacDinhViewModel.Instance;
    public ThanhPhamFilletMapViewModel VmThanhPhamFilletMap => ThanhPhamFilletMapViewModel.Instance;
    public ThanhPhamFilletViewModel VmThanhPhamFillet => ThanhPhamFilletViewModel.Instance;
    public ThanhPhamKHCViewModel VmThanhPhamKHC => ThanhPhamKHCViewModel.Instance;
    public ThanhPhamLangDa_ColorViewModel VmThanhPhamLangDa_Color => ThanhPhamLangDa_ColorViewModel.Instance;
    public ThanhPhamLangDaViewModel VmThanhPhamLangDa => ThanhPhamLangDaViewModel.Instance;
    public ThanhPhamNguyenLieuViewModel VmThanhPhamNguyenLieu => ThanhPhamNguyenLieuViewModel.Instance;
    public ThanhPhamPhoiTronViewModel VmThanhPhamPhoiTron => ThanhPhamPhoiTronViewModel.Instance;
    public ThanhPhamPhuPham_ColorViewModel VmThanhPhamPhuPham_Color => ThanhPhamPhuPham_ColorViewModel.Instance;
    public ThanhPhamPhuPhamViewModel VmThanhPhamPhuPham => ThanhPhamPhuPhamViewModel.Instance;
    public ThanhPhamPhuXepKhuonViewModel VmThanhPhamPhuXepKhuon => ThanhPhamPhuXepKhuonViewModel.Instance;
    public ThanhPhamSoCheDinhHinhViewModel VmThanhPhamSoCheDinhHinh => ThanhPhamSoCheDinhHinhViewModel.Instance;
    public ThanhPhamSoCheDinhHinh_ColorViewModel VmThanhPhamSoCheDinhHinh_Color => ThanhPhamSoCheDinhHinh_ColorViewModel.Instance;
    public ThanhPhamTaiCheViewModel VmThanhPhamTaiChe => ThanhPhamTaiCheViewModel.Instance;
    public ThePhieuSanLuongFilletViewModel VmThePhieuSanLuongFillet => ThePhieuSanLuongFilletViewModel.Instance;
    public TheRoViewModel VmTheRo => TheRoViewModel.Instance;
    public TheThanhPhamViewModel VmTheThanhPham => TheThanhPhamViewModel.Instance;
    public TheViewModel VmThe => TheViewModel.Instance;
    public Thit_LoaiNguyenLieuPhaLocViewModel VmThit_LoaiNguyenLieuPhaLoc => Thit_LoaiNguyenLieuPhaLocViewModel.Instance;
    public Thit_MaThanhPhamPhaLocViewModel VmThit_MaThanhPham => Thit_MaThanhPhamPhaLocViewModel.Instance;
    public Thit_PhieuCanPhaLocViewModel VmThit_PhieuCanPhaLoc => Thit_PhieuCanPhaLocViewModel.Instance;
    public Thit_SizePhaLocViewModel VmThit_SizePhaLoc => Thit_SizePhaLocViewModel.Instance;
    public ThongKeCaChetViewModel VmThongKeCaChet => ThongKeCaChetViewModel.Instance;
    public TrangThaiNhanVienTamThoiViewModel VmTrangThaiNhanVienTamThoi => TrangThaiNhanVienTamThoiViewModel.Instance;
    public TrongLuongBinhQuanCaChetViewModel VmTrongLuongBinhQuanCaChet => TrongLuongBinhQuanCaChetViewModel.Instance;
    public ViTriFilletViewModel VmViTriFillet => ViTriFilletViewModel.Instance;
  
    //public XepHangViewModel VmXepHang => XepHangViewModel.
    public XiNghiepViewModel VmXiNghiep => XiNghiepViewModel.Instance;
    public RolePermistionViewModel VmRolePermistion => RolePermistionViewModel.Instance;
    public UserRoleViewModel VmUserRole => UserRoleViewModel.Instance;
    public TableInfoViewModel VmTableInfo => TableInfoViewModel.Instance;
    public PhieuCanSoCheDinhHinhViewModel VmPhieuCanSoCheDinhHinh => PhieuCanSoCheDinhHinhViewModel.Instance;
    public PhieuCanPhuXepKhuonViewModel VmPhieuCanPhuXepKhuon => PhieuCanPhuXepKhuonViewModel.Instance;
    public PhieuCanXepKhuonBlockViewModel VmPhieuCanXepKhuonBlock => PhieuCanXepKhuonBlockViewModel.Instance;
    public PhieuCanXepKhuonKHCViewModel VmPhieuCanXepKhuonKHC => PhieuCanXepKhuonKHCViewModel.Instance;
    public PhieuCanTaiCheViewModel VmPhieuCanTaiChe => PhieuCanTaiCheViewModel.Instance;
    public PD_PhieuCanViewModel VmPD_PhieuCan => PD_PhieuCanViewModel.Instance;
    public BT_PhieuCanViewModel VmBT_PhieuCan => BT_PhieuCanViewModel.Instance;
    public BV_BravoCheckInOutViewModel VmBV_BravoCheckInOut => BV_BravoCheckInOutViewModel.Instance;
    public KhuVucViewModel VmKhuVuc => KhuVucViewModel.Instance;
    public PMS_DataRecordViewModel VmPMS_Record => PMS_DataRecordViewModel.Instance;
    public UserAreaViewModel VmUserArea => UserAreaViewModel.Instance;
    public SanPhamBravoViewModel VmSanPhamBravo => SanPhamBravoViewModel.Instance;
    public BoTriNhomSoCheViewModel VmBoTriNhomSoChe => BoTriNhomSoCheViewModel.Instance;
    public NhomSoCheDinhHinhViewModel VmNhomSoCheDinhHinh => NhomSoCheDinhHinhViewModel.Instance;
    public BoTriNhomKiemSoCheViewModel VmBoTriNhomKiemSoChe => BoTriNhomKiemSoCheViewModel.Instance;
    public NhomKiemSoCheViewModel VmNhomKiemSoChe => NhomKiemSoCheViewModel.Instance;
    public ThanhPhamPhuFilletViewModel VmThanhPhamPhuFillet => ThanhPhamPhuFilletViewModel.Instance;
    public GioVaoRaViewModel VmGioRaVaoFillet => GioVaoRaViewModel.Instance;
    public BV_PhieuCanDinhHinhViewModel VmBV_PhieuCanDinhHinh => BV_PhieuCanDinhHinhViewModel.Instance;
    public BV_PhieuCanKiemDinhHinhViewModel VmBV_PhieuCanKiemDinhHinh => BV_PhieuCanKiemDinhHinhViewModel.Instance;
    public BV_PhieuCanFilletViewModel VmBV_PhieuCanFillet=> BV_PhieuCanFilletViewModel.Instance;
    public CongViecTinhLuongTheoLoaiThanhPhamViewModel VmCongViecTinhLuongTheoLoaiThanhPham => CongViecTinhLuongTheoLoaiThanhPhamViewModel.Instance;
    public CongViecTinhLuongSanLuongViewModel VmCongViecTinhLuongSanLuong => CongViecTinhLuongSanLuongViewModel.Instance;
    public LoaiDuLieuViewModel VmLoaiDuLieu => LoaiDuLieuViewModel.Instance;
    public PhieuSanLuongViewModel VmPhieuSanLuong => PhieuSanLuongViewModel.Instance;
    public KhuVucKetChuyenViewModel VmKhuVucKetChuyen => KhuVucKetChuyenViewModel.Instance;
    public BV_PhieuCanSanXuatViewModel VmBV_PhieuCanSanXuat => BV_PhieuCanSanXuatViewModel.Instance;
    public BV_PhieuCanCaGiongViewModel VmBV_PhieuCanCaGiong => BV_PhieuCanCaGiongViewModel.Instance;
    public BV_PhieuCanCaThitViewModel VmBV_PhieuCanCaThit => BV_PhieuCanCaThitViewModel.Instance;
    public BV_PhieuCanCaNgopViewModel VmBV_PhieuCanCaNgop => BV_PhieuCanCaNgopViewModel.Instance;
    public BV_PhieuCanBotCaViewModel VmBV_PhieuCanBotCa => BV_PhieuCanBotCaViewModel.Instance;
    public PhieuCanTPDinhHinh_TyLeViewModel VmPhieuCanTPDinhHinh_TyLe => PhieuCanTPDinhHinh_TyLeViewModel.Instance;
    public PhieuCanKeToanViewModel VmPhieuCanKeToan => PhieuCanKeToanViewModel.Instance;
    public DashboardViewModel VmDashBoard => DashboardViewModel.Instance;
    public HQ_PhieuCanViewModel VmHQ_PhieuCan => HQ_PhieuCanViewModel.Instance;
    public HQ_NhomViewModel VmHQ_Nhom => HQ_NhomViewModel.Instance;
    public HQ_NhanVienTheoNhomViewModel VmHQ_NhanVienTheoNhom => HQ_NhanVienTheoNhomViewModel.Instance;
    private int MessageBoxShow(string message, string title, int button)
    {
       return 0;

    }
    private  bool CheckServerOnline()
    {
        var dao = new Dao.Repos.Database();
        return dao.CheckServerOnline();
    }
    
    private void DbUpdateDatabase()
    {
        var database = new Dao.Repos.Database();
        database.DbContextUpdateDatabase();
    }
}