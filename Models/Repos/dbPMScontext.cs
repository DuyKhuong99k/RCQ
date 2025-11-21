using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Models.Repos.Models;

namespace Models.Repos;

public partial class dbPMScontext : DbContext
{
    public dbPMScontext()
    {
    }

    public dbPMScontext(DbContextOptions<dbPMScontext> options)
        : base(options)
    {
    }

    public virtual DbSet<API_User> API_User { get; set; }

    public virtual DbSet<BT_KhachHang> BT_KhachHang { get; set; }

    public virtual DbSet<BT_MaLoaiCa> BT_MaLoaiCa { get; set; }

    public virtual DbSet<BT_MaThanhPham> BT_MaThanhPham { get; set; }

    public virtual DbSet<BT_NhanVienTheoNhom> BT_NhanVienTheoNhom { get; set; }

    public virtual DbSet<BT_NhomTinhLuong> BT_NhomTinhLuong { get; set; }

    public virtual DbSet<BT_PhieuCan> BT_PhieuCan { get; set; }

    public virtual DbSet<BanCatTiet> BanCatTiet { get; set; }

    public virtual DbSet<BanCatTietTheoLine> BanCatTietTheoLine { get; set; }

    public virtual DbSet<BanCatTiet_temp> BanCatTiet_temp { get; set; }

    public virtual DbSet<BanFillet> BanFillet { get; set; }

    public virtual DbSet<BanQuyenPhanMem> BanQuyenPhanMem { get; set; }

    public virtual DbSet<BieuMau_Navico> BieuMau_Navico { get; set; }

    public virtual DbSet<BoPhan> BoPhan { get; set; }

    public virtual DbSet<BoTriLoSizeThanhPham> BoTriLoSizeThanhPham { get; set; }

    public virtual DbSet<BoTriNhanVienTheoSize> BoTriNhanVienTheoSize { get; set; }

    public virtual DbSet<BoTriNhomKiemSoChe> BoTriNhomKiemSoChe { get; set; }

    public virtual DbSet<BoTriNhomSoChe> BoTriNhomSoChe { get; set; }

    public virtual DbSet<BoTriTinhLuonXepKhuon> BoTriTinhLuonXepKhuon { get; set; }

    public virtual DbSet<BravoSoChe> BravoSoChe { get; set; }

    public virtual DbSet<ChiTietRaCoi> ChiTietRaCoi { get; set; }

    public virtual DbSet<ChucVu> ChucVu { get; set; }

    public virtual DbSet<CoiLogs> CoiLogs { get; set; }

    public virtual DbSet<CoiMonitor> CoiMonitor { get; set; }

    public virtual DbSet<ColorCode> ColorCode { get; set; }

    public virtual DbSet<CongViecPhuFillet> CongViecPhuFillet { get; set; }

    public virtual DbSet<CongViecTinhLuongXepKhuon> CongViecTinhLuongXepKhuon { get; set; }

    public virtual DbSet<CongViecTinhLuongXepKhuonSanLuong> CongViecTinhLuongXepKhuonSanLuong { get; set; }

    public virtual DbSet<CongViecTinhLuongXepKhuonTheoLoaiThanhPham> CongViecTinhLuongXepKhuonTheoLoaiThanhPham { get; set; }

    public virtual DbSet<DG_DonGia> DG_DonGia { get; set; }

    public virtual DbSet<DG_DonGiaT> DG_DonGiaT { get; set; }

    public virtual DbSet<DG_LoaiDonGia> DG_LoaiDonGia { get; set; }

    public virtual DbSet<DG_SanPhamTinhLuong> DG_SanPhamTinhLuong { get; set; }

    public virtual DbSet<DanhMucFormMoTrucTiep> DanhMucFormMoTrucTiep { get; set; }

    public virtual DbSet<DanhSachMayTinh> DanhSachMayTinh { get; set; }

    public virtual DbSet<DanhSachToKiem> DanhSachToKiem { get; set; }

    public virtual DbSet<DataFilletv2> DataFilletv2 { get; set; }

    public virtual DbSet<DinhMucDinhHinh> DinhMucDinhHinh { get; set; }

    public virtual DbSet<DinhMucFillet> DinhMucFillet { get; set; }

    public virtual DbSet<DinhMucXepHang> DinhMucXepHang { get; set; }

    public virtual DbSet<GioVaoRaFillet> GioVaoRaFillet { get; set; }

    public virtual DbSet<GioVaoRaTinhLuongXepKhuon> GioVaoRaTinhLuongXepKhuon { get; set; }

    public virtual DbSet<KD_DonHang> KD_DonHang { get; set; }

    public virtual DbSet<KD_PhieuCan> KD_PhieuCan { get; set; }

    public virtual DbSet<KD_QuyCach> KD_QuyCach { get; set; }

    public virtual DbSet<KD_Size> KD_Size { get; set; }

    public virtual DbSet<KD_ThanhPham> KD_ThanhPham { get; set; }

    public virtual DbSet<KD_Tui> KD_Tui { get; set; }

    public virtual DbSet<KNH_PhieuCan> KNH_PhieuCan { get; set; }

    public virtual DbSet<KNH_QuyCach> KNH_QuyCach { get; set; }

    public virtual DbSet<KNH_Size> KNH_Size { get; set; }

    public virtual DbSet<KNH_ThanhPham> KNH_ThanhPham { get; set; }

    public virtual DbSet<KNH_ThongTinSanPham> KNH_ThongTinSanPham { get; set; }

    public virtual DbSet<LineFillet> LineFillet { get; set; }

    public virtual DbSet<LineFilletv2> LineFilletv2 { get; set; }

    public virtual DbSet<LoNguyenLieu> LoNguyenLieu { get; set; }

    public virtual DbSet<LoTheoLine> LoTheoLine { get; set; }

    public virtual DbSet<LoaiCaXepKhuon> LoaiCaXepKhuon { get; set; }

    public virtual DbSet<LogKetChuyen> LogKetChuyen { get; set; }

    public virtual DbSet<LogKetChuyenBravo> LogKetChuyenBravo { get; set; }

    public virtual DbSet<LogKiemSoatKetChuyen> LogKiemSoatKetChuyen { get; set; }

    public virtual DbSet<MPG_CongThuc> MPG_CongThuc { get; set; }

    public virtual DbSet<MPG_CongThucChiTiet> MPG_CongThucChiTiet { get; set; }

    public virtual DbSet<MPG_CongThucXacNhan> MPG_CongThucXacNhan { get; set; }

    public virtual DbSet<MPG_PhieuCan> MPG_PhieuCan { get; set; }

    public virtual DbSet<MPG_SanPham> MPG_SanPham { get; set; }

    public virtual DbSet<MaAoVungNuoi> MaAoVungNuoi { get; set; }

    public virtual DbSet<MaCa> MaCa { get; set; }

    public virtual DbSet<MaChatLuongTaiChe> MaChatLuongTaiChe { get; set; }

    public virtual DbSet<MaChatLuongXepKhuon> MaChatLuongXepKhuon { get; set; }

    public virtual DbSet<MaChatLuongXepKhuonBlock> MaChatLuongXepKhuonBlock { get; set; }

    public virtual DbSet<MaChieuXaXepKhuon> MaChieuXaXepKhuon { get; set; }

    public virtual DbSet<MaChuAoVungNuoi> MaChuAoVungNuoi { get; set; }

    public virtual DbSet<MaCoiXepKhuon> MaCoiXepKhuon { get; set; }

    public virtual DbSet<MaCongDoanVungNuoi> MaCongDoanVungNuoi { get; set; }

    public virtual DbSet<MaCongDoanXepKhuon> MaCongDoanXepKhuon { get; set; }

    public virtual DbSet<MaCongViecTaiChe> MaCongViecTaiChe { get; set; }

    public virtual DbSet<MaGheVungNuoi> MaGheVungNuoi { get; set; }

    public virtual DbSet<MaKhachHangCaChet> MaKhachHangCaChet { get; set; }

    public virtual DbSet<MaKhachHangXepKhuon> MaKhachHangXepKhuon { get; set; }

    public virtual DbSet<MaKhuVucXepKhuon> MaKhuVucXepKhuon { get; set; }

    public virtual DbSet<MaLo> MaLo { get; set; }

    public virtual DbSet<MaLoaiCaCaChet> MaLoaiCaCaChet { get; set; }

    public virtual DbSet<MaLoaiCaCaoThit> MaLoaiCaCaoThit { get; set; }

    public virtual DbSet<MaLoaiCaCatTiet> MaLoaiCaCatTiet { get; set; }

    public virtual DbSet<MaLoaiCaDinhHinh> MaLoaiCaDinhHinh { get; set; }

    public virtual DbSet<MaLoaiCaFillet> MaLoaiCaFillet { get; set; }

    public virtual DbSet<MaLoaiCaGiongVungNuoi> MaLoaiCaGiongVungNuoi { get; set; }

    public virtual DbSet<MaLoaiCaLangDa> MaLoaiCaLangDa { get; set; }

    public virtual DbSet<MaLoaiCaNguyenLieu> MaLoaiCaNguyenLieu { get; set; }

    public virtual DbSet<MaLoaiCaNguyenLieu_temp> MaLoaiCaNguyenLieu_temp { get; set; }

    public virtual DbSet<MaLoaiCaPhuPham> MaLoaiCaPhuPham { get; set; }

    public virtual DbSet<MaLoaiCaSoCheDinhHinh> MaLoaiCaSoCheDinhHinh { get; set; }

    public virtual DbSet<MaLoaiCaTaiChe> MaLoaiCaTaiChe { get; set; }

    public virtual DbSet<MaLoaiCaVungNuoi> MaLoaiCaVungNuoi { get; set; }

    public virtual DbSet<MaLoaiCaXepKhuon> MaLoaiCaXepKhuon { get; set; }

    public virtual DbSet<MaLoi> MaLoi { get; set; }

    public virtual DbSet<MaMauCaGiongVungNuoi> MaMauCaGiongVungNuoi { get; set; }

    public virtual DbSet<MaMauCatTiet> MaMauCatTiet { get; set; }

    public virtual DbSet<MaMauDinhHinh> MaMauDinhHinh { get; set; }

    public virtual DbSet<MaMauFillet> MaMauFillet { get; set; }

    public virtual DbSet<MaMauLangDa> MaMauLangDa { get; set; }

    public virtual DbSet<MaMauNguyenLieu> MaMauNguyenLieu { get; set; }

    public virtual DbSet<MaMauNguyenLieu_temp> MaMauNguyenLieu_temp { get; set; }

    public virtual DbSet<MaMauPhuPham> MaMauPhuPham { get; set; }

    public virtual DbSet<MaMauTaiChe> MaMauTaiChe { get; set; }

    public virtual DbSet<MaMauXepKhuon> MaMauXepKhuon { get; set; }

    public virtual DbSet<MaMauXepKhuonBlock> MaMauXepKhuonBlock { get; set; }

    public virtual DbSet<MaNetXepKhuon> MaNetXepKhuon { get; set; }

    public virtual DbSet<MaNhanVienTheoNhomXepKhuon> MaNhanVienTheoNhomXepKhuon { get; set; }

    public virtual DbSet<MaNhomXepKhuon> MaNhomXepKhuon { get; set; }

    public virtual DbSet<MaQuyCachCaoThit> MaQuyCachCaoThit { get; set; }

    public virtual DbSet<MaQuyCachXepKhuon> MaQuyCachXepKhuon { get; set; }

    public virtual DbSet<MaSanPhamDinhHinhBravo> MaSanPhamDinhHinhBravo { get; set; }

    public virtual DbSet<MaSizeCaoThit> MaSizeCaoThit { get; set; }

    public virtual DbSet<MaSizeCatTiet> MaSizeCatTiet { get; set; }

    public virtual DbSet<MaSizeChinhXepKhuon> MaSizeChinhXepKhuon { get; set; }

    public virtual DbSet<MaSizeDinhHinh> MaSizeDinhHinh { get; set; }

    public virtual DbSet<MaSizeFillet> MaSizeFillet { get; set; }

    public virtual DbSet<MaSizeLangDa> MaSizeLangDa { get; set; }

    public virtual DbSet<MaSizeNguyenLieu> MaSizeNguyenLieu { get; set; }

    public virtual DbSet<MaSizeNguyenLieu_temp> MaSizeNguyenLieu_temp { get; set; }

    public virtual DbSet<MaSizePhuPham> MaSizePhuPham { get; set; }

    public virtual DbSet<MaSizeTaiChe> MaSizeTaiChe { get; set; }

    public virtual DbSet<MaSizeXepKhuon> MaSizeXepKhuon { get; set; }

    public virtual DbSet<MaSizeXepKhuonBlock> MaSizeXepKhuonBlock { get; set; }

    public virtual DbSet<MaSizeXepKhuonKHC> MaSizeXepKhuonKHC { get; set; }

    public virtual DbSet<MaThanhPhamCaoThit> MaThanhPhamCaoThit { get; set; }

    public virtual DbSet<MaThanhPhamCatTiet> MaThanhPhamCatTiet { get; set; }

    public virtual DbSet<MaThanhPhamChinhXepKhuon> MaThanhPhamChinhXepKhuon { get; set; }

    public virtual DbSet<MaThanhPhamDinhHinh> MaThanhPhamDinhHinh { get; set; }

    public virtual DbSet<MaThanhPhamDinhHinh_Color> MaThanhPhamDinhHinh_Color { get; set; }

    public virtual DbSet<MaThanhPhamDinhHinh_TyLe> MaThanhPhamDinhHinh_TyLe { get; set; }

    public virtual DbSet<MaThanhPhamExDinhHinh> MaThanhPhamExDinhHinh { get; set; }

    public virtual DbSet<MaThanhPhamFillet> MaThanhPhamFillet { get; set; }

    public virtual DbSet<MaThanhPhamFillet_Color> MaThanhPhamFillet_Color { get; set; }

    public virtual DbSet<MaThanhPhamFillet_HanMucTrongLuong> MaThanhPhamFillet_HanMucTrongLuong { get; set; }

    public virtual DbSet<MaThanhPhamFillet_ThanhPhamMacDinh> MaThanhPhamFillet_ThanhPhamMacDinh { get; set; }

    public virtual DbSet<MaThanhPhamLangDa> MaThanhPhamLangDa { get; set; }

    public virtual DbSet<MaThanhPhamLangDa_Color> MaThanhPhamLangDa_Color { get; set; }

    public virtual DbSet<MaThanhPhamNguyenLieu> MaThanhPhamNguyenLieu { get; set; }

    public virtual DbSet<MaThanhPhamNguyenLieu_temp> MaThanhPhamNguyenLieu_temp { get; set; }

    public virtual DbSet<MaThanhPhamPhuPham> MaThanhPhamPhuPham { get; set; }

    public virtual DbSet<MaThanhPhamPhuPham_Color> MaThanhPhamPhuPham_Color { get; set; }

    public virtual DbSet<MaThanhPhamSoCheDinhHinh> MaThanhPhamSoCheDinhHinh { get; set; }

    public virtual DbSet<MaThanhPhamSoCheDinhHinh_Color> MaThanhPhamSoCheDinhHinh_Color { get; set; }

    public virtual DbSet<MaThanhPhamTaiChe> MaThanhPhamTaiChe { get; set; }

    public virtual DbSet<MaThanhPhamXepKhuon> MaThanhPhamXepKhuon { get; set; }

    public virtual DbSet<MaThanhPhamXepKhuonBlock> MaThanhPhamXepKhuonBlock { get; set; }

    public virtual DbSet<MaThanhPhamXepKhuonBlock_Color> MaThanhPhamXepKhuonBlock_Color { get; set; }

    public virtual DbSet<MaThanhPhamXepKhuonKHC> MaThanhPhamXepKhuonKHC { get; set; }

    public virtual DbSet<MaThanhPhamXepKhuon_Color> MaThanhPhamXepKhuon_Color { get; set; }

    public virtual DbSet<MaThanhPham_PhoiTron> MaThanhPham_PhoiTron { get; set; }

    public virtual DbSet<MaThongKeCaChet> MaThongKeCaChet { get; set; }

    public virtual DbSet<MaThongKeDauAo> MaThongKeDauAo { get; set; }

    public virtual DbSet<MaXepHang> MaXepHang { get; set; }

    public virtual DbSet<MapThanhPhamFillet> MapThanhPhamFillet { get; set; }

    public virtual DbSet<MayCan> MayCans { get; set; }

    public virtual DbSet<MayLangDa> MayLangDa { get; set; }

    public virtual DbSet<MayPhanCo> MayPhanCo { get; set; }

    public virtual DbSet<MocThoiGianPhanCa> MocThoiGianPhanCa { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NguoiDung_FormMoTrucTiep> NguoiDung_FormMoTrucTiep { get; set; }

    public virtual DbSet<NguoiDung_NhomQuyen> NguoiDung_NhomQuyen { get; set; }

    public virtual DbSet<NguoiDung_Quyen> NguoiDung_Quyen { get; set; }

    public virtual DbSet<NguoiDung_ThongTin> NguoiDung_ThongTin { get; set; }

    public virtual DbSet<NguyenLieu_PhuongTien> NguyenLieu_PhuongTien { get; set; }

    public virtual DbSet<NguyenLieu_TyLeNuoc> NguyenLieu_TyLeNuoc { get; set; }

    public virtual DbSet<NhaCungCapNguyenLieu> NhaCungCapNguyenLieu { get; set; }

    public virtual DbSet<NhaCungCapNguyenLieu_DonGiaVanChuyen> NhaCungCapNguyenLieu_DonGiaVanChuyen { get; set; }

    public virtual DbSet<NhaCungCapNguyenLieu_temp> NhaCungCapNguyenLieu_temp { get; set; }

    public virtual DbSet<NhaMuaPhuPham> NhaMuaPhuPham { get; set; }

    public virtual DbSet<NhanVien> NhanVien { get; set; }

    public virtual DbSet<NhanVienCongCu> NhanVienCongCu { get; set; }

    public virtual DbSet<NhanVienDaiThanh> NhanVienDaiThanh { get; set; }

    public virtual DbSet<NhanVienDaiThanhCu> NhanVienDaiThanhCu { get; set; }

    public virtual DbSet<NhanVienPhuTheoBanFillet> NhanVienPhuTheoBanFillet { get; set; }

    public virtual DbSet<NhanVienPhuTheoLine> NhanVienPhuTheoLine { get; set; }

    public virtual DbSet<NhanVienPhucVuTheoBan> NhanVienPhucVuTheoBan { get; set; }

    public virtual DbSet<NhanVienSanLuongTheoLineFillet> NhanVienSanLuongTheoLineFillet { get; set; }

    public virtual DbSet<NhanVienTheoBan> NhanVienTheoBan { get; set; }

    public virtual DbSet<NhanVienTheoLine> NhanVienTheoLine { get; set; }

    public virtual DbSet<NhanVienTrongBanCatTiet> NhanVienTrongBanCatTiet { get; set; }

    public virtual DbSet<NhomKiemSoChe> NhomKiemSoChe { get; set; }

    public virtual DbSet<NhomLo> NhomLo { get; set; }

    public virtual DbSet<NhomSoCheDinhHinh> NhomSoCheDinhHinh { get; set; }

    public virtual DbSet<PD_CongThuc> PD_CongThuc { get; set; }

    public virtual DbSet<PD_CongThucChiTiet> PD_CongThucChiTiet { get; set; }

    public virtual DbSet<PD_DonViTinh> PD_DonViTinh { get; set; }

    public virtual DbSet<PD_LoaiSanPham> PD_LoaiSanPham { get; set; }

    public virtual DbSet<PD_LyDo> PD_LyDo { get; set; }

    public virtual DbSet<PD_PhieuCan> PD_PhieuCan { get; set; }

    public virtual DbSet<PD_PhieuNhap> PD_PhieuNhap { get; set; }

    public virtual DbSet<PD_PhieuNhapChiTiet> PD_PhieuNhapChiTiet { get; set; }

    public virtual DbSet<PD_PhieuXuat> PD_PhieuXuat { get; set; }

    public virtual DbSet<PD_PhieuXuatChiTiet> PD_PhieuXuatChiTiet { get; set; }

    public virtual DbSet<PD_SanPham> PD_SanPham { get; set; }

    public virtual DbSet<PLC> PLC { get; set; }

    public virtual DbSet<PLCChiTiet> PLCChiTiet { get; set; }

    public virtual DbSet<PMS_L> PMS_L { get; set; }

    public virtual DbSet<PhanBoNhanVienTheoCongViecPhuFillet> PhanBoNhanVienTheoCongViecPhuFillet { get; set; }

    public virtual DbSet<PhieuCanBTPDinhHinh> PhieuCanBTPDinhHinh { get; set; }

    public virtual DbSet<PhieuCanBTPDinhHinh2> PhieuCanBTPDinhHinh2 { get; set; }

    public virtual DbSet<PhieuCanBTPFillet> PhieuCanBTPFillet { get; set; }

    public virtual DbSet<PhieuCanBTPFilletv2> PhieuCanBTPFilletv2 { get; set; }

    public virtual DbSet<PhieuCanCaChet> PhieuCanCaChet { get; set; }

    public virtual DbSet<PhieuCanCaChetDaiThanhSide> PhieuCanCaChetDaiThanhSide { get; set; }

    public virtual DbSet<PhieuCanCaGiongVungNuoi> PhieuCanCaGiongVungNuoi { get; set; }

    public virtual DbSet<PhieuCanCaGiongVungNuoiDaiThanhSide> PhieuCanCaGiongVungNuoiDaiThanhSide { get; set; }

    public virtual DbSet<PhieuCanCaoThit> PhieuCanCaoThit { get; set; }

    public virtual DbSet<PhieuCanChinhXepKhuon> PhieuCanChinhXepKhuon { get; set; }

    public virtual DbSet<PhieuCanChinhXepKhuon2> PhieuCanChinhXepKhuon2 { get; set; }

    public virtual DbSet<PhieuCanDinhHinh> PhieuCanDinhHinh { get; set; }

    public virtual DbSet<PhieuCanLangDa> PhieuCanLangDa { get; set; }

    public virtual DbSet<PhieuCanNguyenLieu> PhieuCanNguyenLieu { get; set; }

    public virtual DbSet<PhieuCanPhaLoc> PhieuCanPhaLoc { get; set; }

    public virtual DbSet<PhieuCanPhuPham> PhieuCanPhuPham { get; set; }

    public virtual DbSet<PhieuCanPhuPhamv2> PhieuCanPhuPhamv2 { get; set; }

    public virtual DbSet<PhieuCanPhuXepKhuon> PhieuCanPhuXepKhuon { get; set; }

    public virtual DbSet<PhieuCanPhuXepKhuon2> PhieuCanPhuXepKhuon2 { get; set; }

    public virtual DbSet<PhieuCanRaDong> PhieuCanRaDong { get; set; }

    public virtual DbSet<PhieuCanRaDong2> PhieuCanRaDong2 { get; set; }

    public virtual DbSet<PhieuCanSauXepKhuon> PhieuCanSauXepKhuon { get; set; }

    public virtual DbSet<PhieuCanSoCheDinhHinh> PhieuCanSoCheDinhHinh { get; set; }

    public virtual DbSet<PhieuCanSoCheDinhHinh2> PhieuCanSoCheDinhHinh2 { get; set; }

    public virtual DbSet<PhieuCanTPDinhHinh> PhieuCanTPDinhHinh { get; set; }

    public virtual DbSet<PhieuCanTPDinhHinh2> PhieuCanTPDinhHinh2 { get; set; }

    public virtual DbSet<PhieuCanTPFillet> PhieuCanTPFillet { get; set; }

    public virtual DbSet<PhieuCanTPFilletv2> PhieuCanTPFilletv2 { get; set; }

    public virtual DbSet<PhieuCanTaiChe> PhieuCanTaiChe { get; set; }

    public virtual DbSet<PhieuCanTaiChe2> PhieuCanTaiChe2 { get; set; }

    public virtual DbSet<PhieuCanVungNuoi> PhieuCanVungNuoi { get; set; }

    public virtual DbSet<PhieuCanVungNuoiDaiThanhSide> PhieuCanVungNuoiDaiThanhSide { get; set; }

    public virtual DbSet<PhieuCanXepKhuonBlock> PhieuCanXepKhuonBlock { get; set; }

    public virtual DbSet<PhieuCanXepKhuonBlock2> PhieuCanXepKhuonBlock2 { get; set; }

    public virtual DbSet<PhieuCanXepKhuonKHC> PhieuCanXepKhuonKHC { get; set; }

    public virtual DbSet<PhieuCanXepKhuonKHC2> PhieuCanXepKhuonKHC2 { get; set; }

    public virtual DbSet<PhieuSanLuongRaCoiTinhLuongXepKhuon> PhieuSanLuongRaCoiTinhLuongXepKhuon { get; set; }

    public virtual DbSet<PhuongTienChoNguyenLieu> PhuongTienChoNguyenLieu { get; set; }

    public virtual DbSet<PhuongTienChoNguyenLieu_temp> PhuongTienChoNguyenLieu_temp { get; set; }

    public virtual DbSet<PhuongTienChoPhuPham> PhuongTienChoPhuPham { get; set; }

    public virtual DbSet<PhuongTienKhongSuDung> PhuongTienKhongSuDung { get; set; }

    public virtual DbSet<PhuongTien_TaiTrong> PhuongTien_TaiTrong { get; set; }

    public virtual DbSet<PhuongTien_TrongLuongDauAo> PhuongTien_TrongLuongDauAo { get; set; }

    public virtual DbSet<QuickSearchMenu> QuickSearchMenu { get; set; }

    public virtual DbSet<RP_MaThanhPhamDinhHinh> RP_MaThanhPhamDinhHinh { get; set; }

    public virtual DbSet<RP_MaThanhPhamFillet> RP_MaThanhPhamFillet { get; set; }

    public virtual DbSet<RP_MaThanhPhamNL> RP_MaThanhPhamNL { get; set; }

    public virtual DbSet<RP_MaThanhPhamNL_Fillet> RP_MaThanhPhamNL_Fillet { get; set; }

    public virtual DbSet<RP_MaThanhPhamNL_SoChe> RP_MaThanhPhamNL_SoChe { get; set; }

    public virtual DbSet<RP_NangXuatDinhMuc> RP_NangXuatDinhMuc { get; set; }

    public virtual DbSet<RP_NhomThanhPham_DinhHinh_Fillet> RP_NhomThanhPham_DinhHinh_Fillet { get; set; }

    public virtual DbSet<RP_SanLuongDenThoiDiem> RP_SanLuongDenThoiDiem { get; set; }

    public virtual DbSet<RP_SanLuongNguyenLieuTrenBanNgay> RP_SanLuongNguyenLieuTrenBanNgay { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermistion> RolePermistion { get; set; }

    public virtual DbSet<SettingDashboard> SettingDashboard { get; set; }

    public virtual DbSet<Sheet1_> Sheet1_ { get; set; }

    public virtual DbSet<T_BaoBi> T_BaoBi { get; set; }

    public virtual DbSet<T_BieuMauGiao_Import> T_BieuMauGiao_Import { get; set; }

    public virtual DbSet<T_Bon> T_Bon { get; set; }

    public virtual DbSet<T_ChiTietBon> T_ChiTietBon { get; set; }

    public virtual DbSet<T_ChiTietVoXo> T_ChiTietVoXo { get; set; }

    public virtual DbSet<T_CongDoan> T_CongDoan { get; set; }

    public virtual DbSet<T_CongDoanTheoThanhPham> T_CongDoanTheoThanhPham { get; set; }

    public virtual DbSet<T_DinhMuc> T_DinhMuc { get; set; }

    public virtual DbSet<T_DonHang> T_DonHang { get; set; }

    public virtual DbSet<T_Goup> T_Goup { get; set; }

    public virtual DbSet<T_HoaChat> T_HoaChat { get; set; }

    public virtual DbSet<T_KhachHang> T_KhachHang { get; set; }

    public virtual DbSet<T_KhangSinh> T_KhangSinh { get; set; }

    public virtual DbSet<T_KhuVuc> T_KhuVuc { get; set; }

    public virtual DbSet<T_LenhSanXuat> T_LenhSanXuat { get; set; }

    public virtual DbSet<T_LoNguyenLieu> T_LoNguyenLieu { get; set; }

    public virtual DbSet<T_LoaiCan> T_LoaiCan { get; set; }

    public virtual DbSet<T_LoaiKhuon> T_LoaiKhuon { get; set; }

    public virtual DbSet<T_LoaiNguyenLieu> T_LoaiNguyenLieu { get; set; }

    public virtual DbSet<T_MuaNguyenLieu_Import> T_MuaNguyenLieu_Import { get; set; }

    public virtual DbSet<T_NhaCungCap> T_NhaCungCap { get; set; }

    public virtual DbSet<T_NhomHoaChat> T_NhomHoaChat { get; set; }

    public virtual DbSet<T_NhomLo> T_NhomLo { get; set; }

    public virtual DbSet<T_Phieu> T_Phieu { get; set; }

    public virtual DbSet<T_PhieuCan> T_PhieuCan { get; set; }

    public virtual DbSet<T_PhieuCanThuMua> T_PhieuCanThuMua { get; set; }

    public virtual DbSet<T_PhieuCanThuMua_Server> T_PhieuCanThuMua_Server { get; set; }

    public virtual DbSet<T_PhieuCan_Server> T_PhieuCan_Server { get; set; }

    public virtual DbSet<T_PhieuNhomYeuCau> T_PhieuNhomYeuCau { get; set; }

    public virtual DbSet<T_PhieuPhanCo> T_PhieuPhanCo { get; set; }

    public virtual DbSet<T_PhieuPhanCoChiTiet> T_PhieuPhanCoChiTiet { get; set; }

    public virtual DbSet<T_PhieuYeuCau> T_PhieuYeuCau { get; set; }

    public virtual DbSet<T_PhuGia> T_PhuGia { get; set; }

    public virtual DbSet<T_PhuongTien> T_PhuongTien { get; set; }

    public virtual DbSet<T_QuyCach> T_QuyCach { get; set; }

    public virtual DbSet<T_QuyTrinh> T_QuyTrinh { get; set; }

    public virtual DbSet<T_QuyTrinhTheoNgay> T_QuyTrinhTheoNgay { get; set; }

    public virtual DbSet<T_SanPham> T_SanPham { get; set; }

    public virtual DbSet<T_Size> T_Size { get; set; }

    public virtual DbSet<T_ThanhPham> T_ThanhPham { get; set; }

    public virtual DbSet<T_ThongTinPhu> T_ThongTinPhu { get; set; }

    public virtual DbSet<T_TieuChuan> T_TieuChuan { get; set; }

    public virtual DbSet<T_TrangThaiNguyenLieu> T_TrangThaiNguyenLieu { get; set; }

    public virtual DbSet<T_TyLeHoaChatTheoNhom> T_TyLeHoaChatTheoNhom { get; set; }

    public virtual DbSet<Tare> Tare { get; set; }

    public virtual DbSet<ThePhieuSanLuongFillet> ThePhieuSanLuongFillet { get; set; }

    public virtual DbSet<TheRo> TheRo { get; set; }

    public virtual DbSet<TheThanhPham> TheThanhPham { get; set; }

    public virtual DbSet<TheTu> TheTu { get; set; }

    public virtual DbSet<TheTuDaiThanh> TheTuDaiThanh { get; set; }

    public virtual DbSet<TheTuNhomXepKhuon> TheTuNhomXepKhuon { get; set; }

    public virtual DbSet<Thit_LoaiNguyenLieuPhaLoc> Thit_LoaiNguyenLieuPhaLoc { get; set; }

    public virtual DbSet<Thit_MaThanhPhamPhaLoc> Thit_MaThanhPhamPhaLoc { get; set; }

    public virtual DbSet<Thit_PhieuCanPhaLoc> Thit_PhieuCanPhaLoc { get; set; }

    public virtual DbSet<Thit_SizePhaLoc> Thit_SizePhaLoc { get; set; }

    public virtual DbSet<ToKiem> ToKiem { get; set; }

    public virtual DbSet<TrangThaiNhanVienTamThoi> TrangThaiNhanVienTamThoi { get; set; }

    public virtual DbSet<TrongLuongBinhQuanCaChet> TrongLuongBinhQuanCaChet { get; set; }

    public virtual DbSet<TrongLuongGioiHanDinhHinh> TrongLuongGioiHanDinhHinh { get; set; }

    public virtual DbSet<Update> Update { get; set; }

    public virtual DbSet<UpdateSuaCa> UpdateSuaCa { get; set; }

    public virtual DbSet<UserArea> UserAreas { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<ViTriFillet> ViTriFillet { get; set; }

    public virtual DbSet<ViewBaoCaoKeToanTheoNhaCungCap> ViewBaoCaoKeToanTheoNhaCungCap { get; set; }

    public virtual DbSet<ViewBaoCaoKeToanTheoPhuongTien> ViewBaoCaoKeToanTheoPhuongTien { get; set; }

    public virtual DbSet<ViewChiPhiVanChuyen> ViewChiPhiVanChuyen { get; set; }

    public virtual DbSet<ViewTongTungAoTrongNgay> ViewTongTungAoTrongNgay { get; set; }

    public virtual DbSet<XiNghiep> XiNghiep { get; set; }
    public virtual DbSet<TableInfo> TableInfos { get; set; }
    public virtual DbSet<PhieuCanRaCoi> PhieuCanRaCois { get; set; }
    public virtual DbSet<HQ_ColorCode_D> HqColorCodeDs { get; set; }
    public virtual DbSet<HQ_ColorCode_U> HqColorCodeUs { get; set; }
    public virtual DbSet<HQ_Lo> HqLos { get; set; }
    public virtual DbSet<HQ_Lo_D> HqLoDs { get; set; }
    public virtual DbSet<HQ_Lo_U> HqLoUs { get; set; }
    public virtual DbSet<HQ_LoaiNguyenLieu> HqLoaiNguyenLieus { get; set; }
    public virtual DbSet<HQ_LoaiNguyenLieu_D> HqLoaiNguyenLieuDs{ get; set; }
    public virtual DbSet<HQ_LoaiNguyenLieu_U> HqLoaiNguyenLieuUs { get; set; }
    public virtual DbSet<HQ_LoChiTiet> HqLoChiTiets { get; set; }
    public virtual DbSet<HQ_LoChiTiet_D> HqLoChiTietDs { get; set; }
    public virtual DbSet<HQ_LoChiTiet_U> HqLoChiTietUs { get; set; }
    public virtual DbSet<HQ_NhanVien_D> HqNhanVienDs { get; set; }
    public virtual DbSet<HQ_NhanVien_U> HqNhanVienUs { get; set; }
    public virtual DbSet<HQ_PhieuCan> HqPhieuCans { get; set; }
    public virtual DbSet<HQ_PhieuCan_D> HqPhieuCanDs { get; set; }
    public virtual DbSet<HQ_PhieuCan_U> HqPhieuCanUs { get; set; }
    public virtual DbSet<HQ_Size> HqSizes { get; set; }
    public virtual DbSet<HQ_Size_D> HqSizeDs { get; set; }
    public virtual DbSet<HQ_Size_U> HqSizeUs { get; set; }
    public virtual DbSet<HQ_ThanhPham> HqThanhPhams { get; set; }
    public virtual DbSet<HQ_ThanhPham_D> HqThanhPhamDs { get; set; }
    public virtual DbSet<HQ_ThanhPham_U> HqThanhPhamUs { get; set; }
    public virtual DbSet<HQ_TheRo_D> HqTheRoDs { get; set; }
    public virtual DbSet<HQ_TheRo_U> HqTheRoUs { get; set; }
    public virtual DbSet<HQ_TheThanhPham_D> HqTheThanhPhamDs { get; set; }
    public virtual DbSet<HQ_TheThanhPham_U> HqTheThanhPhamUs { get; set; }
    public virtual DbSet<HQ_TheTu_D> HqTheTuDs { get; set; }
    public virtual DbSet<HQ_TheTu_U> HqTheTuUs { get; set; }
    public virtual DbSet<HQ_Nhom> HqNhoms { get; set; }
    public virtual DbSet<HQ_NhanVienTheoNhom> HqNhanVienTheoNhoms { get; set; }
    public virtual DbSet<HQ_Ca> HqCas { get; set; }
    public virtual DbSet<HQ_NhanVienTheoCa> HqNhanVienTheoCas { get; set; }
    public virtual DbSet<HQ_MapSanPhamTinhLuong> HqMapSanPhamTinhLuongs { get; set; }
    public virtual DbSet<HQ_PhieuThongKeSanXuat> HqPhieuThongKeSanXuats { get; set; }
    public virtual DbSet<CheckInOut> CheckInOuts { get; set; }
    public virtual DbSet<Ao> Aos { get; set; }
    public virtual DbSet<Ao_D> AoDs { get; set; }
    public virtual DbSet<Ao_U> AoUs { get; set; }
    public virtual DbSet<MaChatLuongXepKhuon_D> MaChatLuongXepKhuonDs { get; set; }
    public virtual DbSet<MaChatLuongXepKhuon_U> MaChatLuongXepKhuonUs { get; set; }
    public virtual DbSet<MaChieuXaXepKhuon_D> MaChieuXaXepKhuonDs { get; set; }
    public virtual DbSet<MaChieuXaXepKhuon_U> MaChieuXaXepKhuonUs { get; set; }
    public virtual DbSet<MaCoiXepKhuon_D> MaCoiXepKhuonDs { get; set; }
    public virtual DbSet<MaCoiXepKhuon_U> MaCoiXepKhuonUs { get; set; }
    public virtual DbSet<MaSizeChinhXepKhuon_D> MaSizeChinhXepKhuonDs { get; set; }
    public virtual DbSet<MaSizeChinhXepKhuon_U> MaSizeChinhXepKhuonUs { get; set; }
    public virtual DbSet<MaSizeFillet_D> MaSizeFilletDs { get; set; }
    public virtual DbSet<MaSizeFillet_U> MaSizeFilletUs { get; set; }
    public virtual DbSet<MaSizeNguyenLieu_D> MaSizeNguyenLieuDs { get; set; }
    public virtual DbSet<MaSizeNguyenLieu_U> MaSizeNguyenLieuUs { get; set; }
    public virtual DbSet<MaSizeXepKhuon_D> MaSizeXepKhuonDs { get; set; }
    public virtual DbSet<MaSizeXepKhuon_U> MaSizeXepKhuonUs { get; set; }
    public virtual DbSet<MaThanhPhamChinhXepKhuon_D> MaThanhPhamChinhXepKhuonDs { get; set; }
    public virtual DbSet<MaThanhPhamChinhXepKhuon_U> MaThanhPhamChinhXepKhuonUs { get; set; }
    public virtual DbSet<MaThanhPhamFillet_D> MaThanhPhamFilletDs { get; set; }
    public virtual DbSet<MaThanhPhamFillet_U> MaThanhPhamFilletUs { get; set; }
    public virtual DbSet<MaThanhPhamNguyenLieu_D> MaThanhPhamNguyenLieuDs { get; set; }
    public virtual DbSet<MaThanhPhamNguyenLieu_U> MaThanhPhamNguyenLieuUs { get; set; }
    public virtual DbSet<MaThanhPhamXepKhuon_D> MaThanhPhamXepKhuonDs { get; set; }
    public virtual DbSet<MaThanhPhamXepKhuon_U> MaThanhPhamXepKhuonUs { get; set; }
    public virtual DbSet<PhuongTienChoNguyenLieu_D> PhuongTienChoNguyenLieuDs { get; set; }
    public virtual DbSet<PhuongTienChoNguyenLieu_U> PhuongTienChoNguyenLieuUs { get; set; }
    public virtual DbSet<TrongLuongCoiTheoThanhPham> TrongLuongCoiTheoThanhPhams { get; set; }
    public virtual DbSet<HQ_CodeGen> HQCodeGens { get; set; }
    public virtual DbSet<HQ_DuyetThucDon> HQDuyetThucDons { get; set; }
    public virtual DbSet<HQ_HuyThucDon> HQHuyThucDons { get; set; }
    public virtual DbSet<HQ_LoaiMonAn> HQLoaiMonAns { get; set; }
    public virtual DbSet<HQ_MonAn> HQMonAns { get; set; }
    public virtual DbSet<HQ_NhatKyDangKyMonAn> HQNhatKyDangKyMonAns { get; set; }
    public virtual DbSet<HQ_NhatKyNhanMonAn> HQNhatKyNhanMonAns { get; set; }
    public virtual DbSet<HQ_ThucDon> HQThucDons { get; set; }
    public virtual DbSet<HQ_ThucDonChiTiet> HQThucDonChiTiets { get; set; }
    public virtual DbSet<CoiLeXepKhuon> CoiLeXepKhuons { get; set; }
    public virtual DbSet<TrongLuongCoiTheoSanPham> TrongLuongCoiTheoSanPhams { get; set; }
    public virtual DbSet<HQ_PhieuCanNguyenLieu> HQ_PhieuCanNguyenLieus { get; set; }
    public virtual DbSet<HQ_PhieuCanNhapNguyenLieu> HQ_PhieuCanNhapNguyenLieus { get; set; }
    public virtual DbSet<HQ_PhieuCanXuatNguyenLieu> HQ_PhieuCanXuatNguyenLieus { get; set; }
    public virtual DbSet<HQ_ChatLuongNguyenLieu> HQ_ChatLuongNguyenLieus { get; set; }
    public virtual DbSet<HQ_DonViTinh> HQ_DonViTinhs { get; set; }
    public virtual DbSet<HQ_KhoNguyenLieu> HQ_KhoNguyenLieus { get; set; }
    public virtual DbSet<HQ_SanPhamNguyenLieu> HQ_SanPhamNguyenLieus { get; set; }
    public virtual DbSet<HQ_QuyCachNguyenLieu> HQ_QuyCachNguyenLieus { get; set; }

#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        =>
    optionsBuilder.UseSqlServer(
        AppViewModels.Base.Ins.ConnectionString2, o => o.UseCompatibilityLevel(120));
    //optionsBuilder.UseSqlServer(
    //     "server=data.pms-vn.com,4751;database=PMS_HQ;uid=pmsvn;pwd=Sql@123456789;TrustServerCertificate=True");//AppViewModels.Base.Ins.ConnectionString2, o => o.UseCompatibilityLevel(120));
#else
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        =>
            optionsBuilder.UseSqlServer(AppViewModels.Base.Ins.ConnectionString2, o => o.UseCompatibilityLevel(120));
#endif


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<API_User>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<BT_NhanVienTheoNhom>(entity =>
        {
            entity.Property(e => e.MaXuong).HasDefaultValueSql("((1))");
            entity.Property(e => e.SoGio).HasDefaultValue(8m);
            entity.Property(e => e.TyLeHuong).HasDefaultValue(1m);
        });

        modelBuilder.Entity<BT_NhomTinhLuong>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<BT_PhieuCan>(entity =>
        {
            entity.Property(e => e.CreateBy).HasDefaultValue("default");
            entity.Property(e => e.CreateDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue("default");
            entity.Property(e => e.ModifiedDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<BanCatTiet>(entity =>
        {
            entity.Property(e => e.ColSpanX1).HasDefaultValue(1);
            entity.Property(e => e.ColSpanX2).HasDefaultValue(1);
            entity.Property(e => e.PlcAdr).HasDefaultValue("00");
            entity.Property(e => e.PlcOffAdr).HasDefaultValue("00");
            entity.Property(e => e.PlcYadr).HasDefaultValue("00");
            entity.Property(e => e.SoLanChiaCaX1).HasDefaultValue(1);
            entity.Property(e => e.SoLanChiaCaX2).HasDefaultValue(1);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
            entity.Property(e => e.ThoiGianNhanCaX1).HasDefaultValue(1000);
            entity.Property(e => e.ThoiGianNhanCaX2).HasDefaultValue(1000);
            entity.Property(e => e.ThoiGianQuangDuongPheu1X1).HasDefaultValue(1000);
            entity.Property(e => e.ThoiGianQuangDuongPheu1X2).HasDefaultValue(1000);
            entity.Property(e => e.ThoiGianQuangDuongPheu2X1).HasDefaultValue(1000);
            entity.Property(e => e.ThoiGianQuangDuongPheu2X2).HasDefaultValue(1000);
            entity.Property(e => e.TimeOpen).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.VongChiaCa).HasDefaultValue(1);
        });

        modelBuilder.Entity<BanCatTiet_temp>(entity =>
        {
            entity.Property(e => e.ColSpanX1).HasDefaultValue(1);
            entity.Property(e => e.ColSpanX2).HasDefaultValue(1);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<BanFillet>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_Demo_BanFillet");

            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<BieuMau_Navico>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ThoiDiemGhiNhanSanLuong).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<BoPhan>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<BoTriLoSizeThanhPham>(entity =>
        {
            entity.Property(e => e.CodeId).HasDefaultValue("");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<BoTriNhanVienTheoSize>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<BoTriTinhLuonXepKhuon>(entity =>
        {
            entity.HasKey(e => new { e.MaNhanVien, e.Ngay, e.MaCongViec, e.MaXuong }).HasName("PK_BiTriTinhLuonXepKhuon");

            entity.Property(e => e.SoGio).HasDefaultValue(8m);
            entity.Property(e => e.TyLeHuong).HasDefaultValue(1m);
        });

        modelBuilder.Entity<ChiTietRaCoi>(entity =>
        {
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayNguyenLieu).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<ChucVu>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<CoiLogs>(entity =>
        {
            entity.Property(e => e.NgayGio).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<CoiMonitor>(entity =>
        {
            entity.Property(e => e.NgayGio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OutLocked).HasDefaultValue(true);
        });

        modelBuilder.Entity<ColorCode>(entity =>
        {
            entity.Property(e => e.Code).HasDefaultValue("none");
            entity.Property(e => e.Code2).HasDefaultValue("none");
        });

        modelBuilder.Entity<CongViecPhuFillet>(entity =>
        {
            entity.Property(e => e.NguonSanLuong)
                .HasDefaultValue(1)
                .HasComment("1: Xuong, 2: Ban Cat Tiet, 3: Line");
        });

        modelBuilder.Entity<CongViecTinhLuongXepKhuon>(entity =>
        {
            entity.Property(e => e.LoaiDuLieu)
                .HasDefaultValue(1)
                .HasComment("1: Lay Tu Bang,2:Nhap Tay,3:Ca 2,4:Lay tu san luong ra coi theo phan bo");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<CongViecTinhLuongXepKhuonSanLuong>(entity =>
        {
            entity.HasKey(e => new { e.MaCongViec, e.Ngay, e.MaCa, e.MaXuong }).HasName("PK_CongViecXepKhuonSanLuong");
        });

        modelBuilder.Entity<CongViecTinhLuongXepKhuonTheoLoaiThanhPham>(entity =>
        {
            entity.HasKey(e => new { e.MaCongViec, e.MaThanhPhamDinhHinh, e.MaThanhPhamPhuXepKhuon, e.MaThanhPhamChinhXepKhuon, e.MaThanhPhamBlockXepKhuon, e.MaThanhPhamKHCXepKhuon, e.MaThanhPhamTaiChe, e.MaThanhPhamSoChe, e.MaCongViecTaiChe, e.MaCongDoan, e.MaChieuXaChinhXepKhuon }).HasName("PK_CongViecXepKhuonTheoLoaiThanhPham");

            entity.Property(e => e.MaCongViecTaiChe).HasDefaultValue(" ");
            entity.Property(e => e.MaCongDoan).HasDefaultValue(" ");
            entity.Property(e => e.MaChieuXaChinhXepKhuon).HasDefaultValue(" ");
        });

        modelBuilder.Entity<DG_DonGia>(entity =>
        {
            entity.Property(e => e.CreateBy).HasDefaultValue("default");
            entity.Property(e => e.CreateDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DanhGia).HasDefaultValue(true);
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.HeSo).HasDefaultValue(1m);
            entity.Property(e => e.HeSoRot).HasDefaultValue(1m);
            entity.Property(e => e.LoaiCan).HasDefaultValue("");
            entity.Property(e => e.MaSizeFillet).HasDefaultValue("");
            entity.Property(e => e.MaThanhPham).HasDefaultValue("");
            entity.Property(e => e.MaXepHang).HasDefaultValue("F");
            entity.Property(e => e.ModifiedBy).HasDefaultValue("default");
            entity.Property(e => e.ModifiedDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<DG_DonGiaT>(entity =>
        {
            entity.Property(e => e.NgayGio).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<DanhSachToKiem>(entity =>
        {
            entity.HasKey(e => new { e.MaToKiem, e.MaXuong, e.MaNhanVien, e.Ngay, e.LoaiBoTri }).HasName("PK_DanhSachToKiem_1");

            entity.Property(e => e.SoGio).HasDefaultValue(8.0);
            entity.Property(e => e.TyLe).HasDefaultValue(1.0);
        });

        modelBuilder.Entity<DinhMucDinhHinh>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<DinhMucFillet>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<GioVaoRaFillet>(entity =>
        {
            entity.Property(e => e.MaXuong).HasDefaultValueSql("((0))");
        });

        modelBuilder.Entity<GioVaoRaTinhLuongXepKhuon>(entity =>
        {
            entity.Property(e => e.CreateBy).HasDefaultValue("default");
            entity.Property(e => e.CreateDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MaCa).HasDefaultValueSql("((1))");
            entity.Property(e => e.MaMayCan).HasDefaultValue("default");
            entity.Property(e => e.ModifyBy).HasDefaultValue("default");
            entity.Property(e => e.ModifyDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<KD_DonHang>(entity =>
        {
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<KD_PhieuCan>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<KD_QuyCach>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<KD_Size>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<KD_ThanhPham>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<KD_Tui>(entity =>
        {
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PhuTroi).HasDefaultValue(0.01m);
            entity.Property(e => e.PhuTroiMax).HasDefaultValue(0.02m);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<KNH_PhieuCan>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<KNH_QuyCach>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<KNH_Size>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<KNH_ThanhPham>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<KNH_ThongTinSanPham>(entity =>
        {
            entity.Property(e => e.CreateDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.JsonValue).HasComment("Thuoc Tinh, Gia Tri");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<LineFilletv2>(entity =>
        {
            entity.Property(e => e.CodeId).HasDefaultValue("");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<LoNguyenLieu>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<LoTheoLine>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValue(1);
            entity.Property(e => e.CodeId).HasDefaultValue("");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<LogKetChuyen>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<LogKiemSoatKetChuyen>(entity =>
        {
            entity.Property(e => e.Action).HasDefaultValue("XOA");
            entity.Property(e => e.CreateDayTime).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<MPG_CongThuc>(entity =>
        {
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MPG_CongThucChiTiet>(entity =>
        {
            entity.Property(e => e.PhuTroi).HasDefaultValue(0.01m);
            entity.Property(e => e.PhuTroiMax).HasDefaultValue(0.02m);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MPG_CongThucXacNhan>(entity =>
        {
            entity.HasKey(e => new { e.MaCongThuc, e.Block, e.Ngay }).HasName("PK_MPD_CongThucXacNhan");

            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MPG_PhieuCan>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PhuTroi).HasDefaultValue(0.01m);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MPG_SanPham>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaAoVungNuoi>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaCa>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaChatLuongTaiChe>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaChatLuongXepKhuon>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaChatLuongXepKhuonBlock>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaChieuXaXepKhuon>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaChuAoVungNuoi>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaCoiXepKhuon>(entity =>
        {
            entity.Property(e => e.Tam).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaCongDoanVungNuoi>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaCongDoanXepKhuon>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaCongViecTaiChe>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaGheVungNuoi>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaKhachHangCaChet>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaKhachHangXepKhuon>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaKhuVucXepKhuon>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLo>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoaiCaCaChet>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoaiCaCaoThit>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoaiCaCatTiet>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoaiCaDinhHinh>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoaiCaFillet>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoaiCaGiongVungNuoi>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoaiCaLangDa>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoaiCaNguyenLieu>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoaiCaNguyenLieu_temp>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoaiCaPhuPham>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoaiCaSoCheDinhHinh>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoaiCaTaiChe>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoaiCaVungNuoi>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoaiCaXepKhuon>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaLoi>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaMauCaGiongVungNuoi>(entity =>
        {
            entity.Property(e => e.TrongLuongDonVi).HasDefaultValue(1m);
        });

        modelBuilder.Entity<MaMauCatTiet>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaMauDinhHinh>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaMauFillet>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaMauLangDa>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaMauNguyenLieu>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaMauNguyenLieu_temp>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaMauPhuPham>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaMauTaiChe>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaMauXepKhuon>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaMauXepKhuonBlock>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaNetXepKhuon>(entity =>
        {
            entity.Property(e => e.BienDo).HasDefaultValue(0.02);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
            entity.Property(e => e.TrongLuong).HasDefaultValue(5.0);
        });

        modelBuilder.Entity<MaNhanVienTheoNhomXepKhuon>(entity =>
        {
            entity.HasKey(e => new { e.MaNhanVien, e.MaNhom, e.Ngay }).HasName("PK_MaNhanVienTheoNhomXepKhuon_1");

            entity.Property(e => e.SoGio).HasDefaultValue(8m);
            entity.Property(e => e.TyLeHuong).HasDefaultValue(1m);
        });

        modelBuilder.Entity<MaNhomXepKhuon>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_MaNhomXepKhuon_1");
        });

        modelBuilder.Entity<MaQuyCachCaoThit>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaQuyCachXepKhuon>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaSizeCaoThit>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaSizeCatTiet>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaSizeChinhXepKhuon>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaSizeDinhHinh>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaSizeFillet>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaSizeLangDa>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaSizeNguyenLieu>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaSizeNguyenLieu_temp>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaSizePhuPham>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaSizeTaiChe>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaSizeXepKhuon>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaSizeXepKhuonBlock>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_SizeXepKhuonBlock");

            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaSizeXepKhuonKHC>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaThanhPhamCaoThit>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaThanhPhamCatTiet>(entity =>
        {
            entity.Property(e => e.Max).HasDefaultValue(999.0);
            entity.Property(e => e.Min).HasDefaultValue(0.0);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaThanhPhamChinhXepKhuon>(entity =>
        {
            entity.Property(e => e.Max).HasDefaultValue(500.0);
        });

        modelBuilder.Entity<MaThanhPhamDinhHinh>(entity =>
        {
            entity.Property(e => e.CodeId).HasDefaultValue("");
            entity.Property(e => e.DinhMuc).HasDefaultValue(1.0);
            entity.Property(e => e.DinhMucCaTra).HasDefaultValue(1m);
            entity.Property(e => e.DinhMucKhongDauVao).HasDefaultValue(1m);
            entity.Property(e => e.IsDisplay).HasDefaultValue(true);
            entity.Property(e => e.Max).HasDefaultValue(999.0);
            entity.Property(e => e.MaxOut).HasDefaultValue(999.0);
            entity.Property(e => e.Min).HasDefaultValue(0.0);
            entity.Property(e => e.MinOut).HasDefaultValue(0.0);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
            entity.Property(e => e.TyLeDinhMucDau).HasDefaultValue(0.5m);
            entity.Property(e => e.TyLeDinhMucRot).HasDefaultValue(0.5m);
        });

        modelBuilder.Entity<MaThanhPhamDinhHinh_Color>(entity =>
        {
            entity.HasKey(e => new { e.ColorCode, e.Ngay, e.MaLo, e.MaXuong }).HasName("PK_MaThanhPhamDinhHinh_Color_1");
        });

        modelBuilder.Entity<MaThanhPhamDinhHinh_TyLe>(entity =>
        {
            entity.Property(e => e.MaXuong).HasDefaultValue("");
            entity.Property(e => e.NgayGio).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<MaThanhPhamExDinhHinh>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_MaLoaiCaExDinhHinh");
        });

        modelBuilder.Entity<MaThanhPhamFillet>(entity =>
        {
            entity.Property(e => e.CodeId).HasDefaultValue("");
            entity.Property(e => e.DinhMuc).HasDefaultValue(1m);
            entity.Property(e => e.DinhMucHaoHut).HasDefaultValue(1m);
            entity.Property(e => e.Max).HasDefaultValue(999.0);
            entity.Property(e => e.Min).HasDefaultValue(0.0);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
            entity.Property(e => e.ThoiGianTren1kgSeconds).HasComment("Don Vi Giay");
            entity.Property(e => e.TrangThaiThanhPham).HasDefaultValue("NORMAL");
        });

        modelBuilder.Entity<MaThanhPhamLangDa>(entity =>
        {
            entity.Property(e => e.Max).HasDefaultValue(999.0);
            entity.Property(e => e.Min).HasDefaultValue(0.0);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaThanhPhamLangDa_Color>(entity =>
        {
            entity.HasKey(e => new { e.ColorCode, e.Ngay, e.MaLo, e.MaXuong }).HasName("PK_MaThanhPhamLangDa_Color_1");
        });

        modelBuilder.Entity<MaThanhPhamNguyenLieu>(entity =>
        {
            entity.Property(e => e.IsSNL).HasDefaultValue(true);
            entity.Property(e => e.Max).HasDefaultValue(999.0);
            entity.Property(e => e.Min).HasDefaultValue(0.0);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaThanhPhamNguyenLieu_temp>(entity =>
        {
            entity.Property(e => e.IsSNL).HasDefaultValue(true);
            entity.Property(e => e.Max).HasDefaultValue(999.0);
            entity.Property(e => e.Min).HasDefaultValue(0.0);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaThanhPhamPhuPham>(entity =>
        {
            entity.Property(e => e.Max).HasDefaultValue(999.0);
            entity.Property(e => e.Min).HasDefaultValue(0.0);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaThanhPhamPhuPham_Color>(entity =>
        {
            entity.HasKey(e => new { e.ColorCode, e.Ngay, e.MaLo, e.MaXuong }).HasName("PK_MaThanhPhamPhuPham_Color_1");
        });

        modelBuilder.Entity<MaThanhPhamSoCheDinhHinh>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_MaThanhPhamSoBoDinhHinh");

            entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Createdby).HasDefaultValue("default");
            entity.Property(e => e.Max).HasDefaultValue(999.0);
            entity.Property(e => e.Nhan).HasDefaultValue(true);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
            entity.Property(e => e.TinhGio).HasDefaultValue(true);
            entity.Property(e => e.X).HasDefaultValue(-1);
            entity.Property(e => e.Y).HasDefaultValue(-1);
        });

        modelBuilder.Entity<MaThanhPhamSoCheDinhHinh_Color>(entity =>
        {
            entity.HasKey(e => new { e.ColorCode, e.Ngay, e.MaLo, e.MaXuong }).HasName("PK_MaThanhPhamSoCheDinhHinh_Color_1");
        });

        modelBuilder.Entity<MaThanhPhamTaiChe>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaThanhPhamXepKhuon>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_MaThanhPhamXepKhuon_1");

            entity.Property(e => e.Max).HasDefaultValue(999.0);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaThanhPhamXepKhuonBlock>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaThanhPhamXepKhuonBlock_Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_MaThanhPhamXepKhuonKHC_Color");

            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<MaThanhPhamXepKhuonKHC>(entity =>
        {
            entity.Property(e => e.Max).HasDefaultValue(99.0);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaThanhPham_PhoiTron>(entity =>
        {
            entity.Property(e => e.CreateDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifyDateTime).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<MaThongKeCaChet>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaThongKeDauAo>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MaXepHang>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MayCan>(entity =>
        {
            entity.Property(e => e.MType).HasDefaultValue("NONE");
            entity.Property(e => e.MaXuong).HasDefaultValue("");
        });

        modelBuilder.Entity<MayLangDa>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MayPhanCo>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<MocThoiGianPhanCa>(entity =>
        {
            entity.Property(e => e.CreateDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifyDateTime).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.Property(e => e.DefaultKey).HasDefaultValue("");
            entity.Property(e => e.Status).HasComment("// 1: ĐANG LÀM, 2: TẠM NGHĨ, 3: THÔI VIỆC, 4: THỬ VIỆC");
        });

        modelBuilder.Entity<NguoiDung_Quyen>(entity =>
        {
            entity.Property(e => e.MaXuong).HasDefaultValueSql("((0))");
        });

        modelBuilder.Entity<NguoiDung_ThongTin>(entity =>
        {
            entity.Property(e => e.FileName).HasDefaultValue("PMS.exe");
            entity.Property(e => e.FolderName).HasDefaultValue("PMS");
            entity.Property(e => e.MoFormTrucTiep).HasDefaultValue("frmMain");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<NguyenLieu_PhuongTien>(entity =>
        {
            entity.Property(e => e.HienThi).HasDefaultValue(false);
            entity.Property(e => e.SuDung).HasDefaultValue(false);
        });

        modelBuilder.Entity<NguyenLieu_TyLeNuoc>(entity =>
        {
            entity.HasKey(e => new { e.STT, e.Ngay, e.MaXuong }).HasName("PK_NguyenLieu_TyLeNuoc_1");
        });

        modelBuilder.Entity<NhaCungCapNguyenLieu>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<NhaCungCapNguyenLieu_temp>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<NhaMuaPhuPham>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<NhanVienCongCu>(entity =>
        {
            entity.HasKey(e => new { e.Ngay, e.MaLo, e.TabName, e.MaNhanVien, e.MaXuong }).HasName("PK_NhanVienChucNang");

            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<NhanVienDaiThanh>(entity =>
        {
            entity.Property(e => e.IsChucNang).HasDefaultValue(true);
            entity.Property(e => e.IsContracting).HasDefaultValue(true);
            entity.Property(e => e.IsHuman).HasDefaultValue(true);
            entity.Property(e => e.IsShowDinhMuc).HasDefaultValue(true);
            entity.Property(e => e.LoaiSanLuong)
                .HasDefaultValue(1)
                .HasComment("-1 : Trừ ra khỏi tổng lượng| 0: Không cộng không trừ chỉ ghi nhận dữ liệu|1: Cộng vào tổng sản lượng");
        });

        modelBuilder.Entity<NhanVienPhuTheoLine>(entity =>
        {
            entity.Property(e => e.MaCongViec).HasDefaultValue(" ");
        });

        modelBuilder.Entity<NhanVienPhucVuTheoBan>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.KhuVuc).HasComment("FL or SC");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<NhanVienSanLuongTheoLineFillet>(entity =>
        {
            entity.HasKey(e => new { e.MaNhanVien, e.MaLine, e.MaXuong, e.Ngay }).HasName("PK_NhanVienTheoLineFillet");
        });

        modelBuilder.Entity<NhanVienTheoBan>(entity =>
        {
            entity.Property(e => e.NgayGio).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<NhanVienTheoLine>(entity =>
        {
            entity.Property(e => e.CodeId).HasDefaultValue("");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<NhomLo>(entity =>
        {
            entity.Property(e => e.GioTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<PD_CongThuc>(entity =>
        {
            entity.Property(e => e.CreateDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MaDonViTinh).HasDefaultValue("KG");
            entity.Property(e => e.ModifiedDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
            entity.Property(e => e.TrongLuongNguyenLieu).HasDefaultValue(1m);
        });

        modelBuilder.Entity<PD_CongThucChiTiet>(entity =>
        {
            entity.HasKey(e => new { e.STT, e.Ngay, e.MaCongThuc }).HasName("PK_PC_CongThucChiTiet");

            entity.Property(e => e.BienDo).HasDefaultValue(0.050m);
            entity.Property(e => e.TyLe).HasDefaultValue(1m);
        });

        modelBuilder.Entity<PD_DonViTinh>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PD_LoaiSanPham>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PD_PhieuCan>(entity =>
        {
            entity.Property(e => e.CreateDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PD_PhieuNhap>(entity =>
        {
            entity.Property(e => e.CreateBy).HasDefaultValue("default");
            entity.Property(e => e.CreateDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue("default");
            entity.Property(e => e.ModifiedDateTime).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<PD_PhieuNhapChiTiet>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PD_PhieuXuat>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreateDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedDateTime).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<PD_PhieuXuatChiTiet>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PD_SanPham>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PLCChiTiet>(entity =>
        {
            entity.Property(e => e.HzQuayDef).HasDefaultValue((ushort)50);
            entity.Property(e => e.HzRaDef).HasDefaultValue((ushort)20);
            entity.Property(e => e.TimeQuayDef).HasDefaultValue((ushort)300);
        });

        modelBuilder.Entity<PhanBoNhanVienTheoCongViecPhuFillet>(entity =>
        {
            entity.Property(e => e.TyLeHuong).HasDefaultValue(1.0);
        });

        modelBuilder.Entity<PhieuCanBTPDinhHinh>(entity =>
        {
            entity.HasKey(e => new { e.STT, e.Ngay, e.MaMayCan, e.MaXuong }).HasName("PK_Demo_PhieuCanBTPDinhHinh_1");

            entity.Property(e => e.MaXuong).HasDefaultValue("DT");
            entity.Property(e => e.MaUserCan).HasDefaultValue("admin");
        });

        modelBuilder.Entity<PhieuCanBTPDinhHinh2>(entity =>
        {
            entity.HasKey(e => new { e.STT, e.Ngay, e.MaMayCan, e.MaXuong }).HasName("PK_Demo_PhieuCanBTPDinhHinh_3");
        });

        modelBuilder.Entity<PhieuCanBTPFillet>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PhieuCanBTPFilletv2>(entity =>
        {
            entity.HasKey(e => new { e.STT, e.Ngay, e.MaMayCan, e.MaXuong }).HasName("PK_Demo_PhieuCanBTPFilletv2_1");

            entity.Property(e => e.MaXuong).HasDefaultValue("DT");
            entity.Property(e => e.MaUserCan).HasDefaultValue("admin");
        });

        modelBuilder.Entity<PhieuCanCaChetDaiThanhSide>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.GioTai).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTai).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<PhieuCanCaGiongVungNuoiDaiThanhSide>(entity =>
        {
            entity.Property(e => e.TenLoaiCa).HasDefaultValue(".");
        });

        modelBuilder.Entity<PhieuCanCaoThit>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<PhieuCanChinhXepKhuon>(entity =>
        {
            entity.Property(e => e.NgayNguyenLieu).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MaChieuXa).HasDefaultValue("000");
            entity.Property(e => e.MaNhanVien).HasDefaultValueSql("((0))");
        });

        modelBuilder.Entity<PhieuCanChinhXepKhuon2>(entity =>
        {
            entity.Property(e => e.MaChieuXa).HasDefaultValue("000");
        });

        modelBuilder.Entity<PhieuCanDinhHinh>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Demo_PhieuCanDinhHinh");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.MaSanPham).HasDefaultValue("");
            entity.Property(e => e.TenSanPham).HasDefaultValue("");
        });

        modelBuilder.Entity<PhieuCanLangDa>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MayCan).HasDefaultValue("default");
        });

        modelBuilder.Entity<PhieuCanNguyenLieu>(entity =>
        {
            entity.Property(e => e.Chuyen).HasDefaultValue(1);
            entity.Property(e => e.MaBanCatTiet).HasDefaultValueSql("((0))");
            entity.Property(e => e.MaMau).HasDefaultValueSql("((0))");
            entity.Property(e => e.MaSize).HasDefaultValueSql("((0))");
            entity.Property(e => e.Pheu).HasDefaultValueSql("((0))");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PhieuCanPhaLoc>(entity =>
        {
            entity.HasKey(e => new { e.MaMayTinhCan, e.MaUserCan, e.ThoiGianCan, e.Ngay }).HasName("PK_Demo_PhieuCanPhaLoc");

            entity.Property(e => e.GhiChu).HasDefaultValue("");
            entity.Property(e => e.InOut).HasComment("0 nhap, 1 xuat");
            entity.Property(e => e.LoaiCan)
                .HasDefaultValue("TP")
                .HasComment("TP & BTP");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PhieuCanPhuPham>(entity =>
        {
            entity.HasKey(e => new { e.MaMayTinhCan, e.MaUserCan, e.ThoiGianCan, e.NgayCan }).HasName("PK_PhieuCanPhuPham_1");

            entity.Property(e => e.NgayCan).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MaMau).HasDefaultValueSql("((0))");
            entity.Property(e => e.MaSize).HasDefaultValueSql("((0))");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PhieuCanPhuPhamv2>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PhieuCanSauXepKhuon>(entity =>
        {
            entity.Property(e => e.NgayNguyenLieu).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<PhieuCanSoCheDinhHinh>(entity =>
        {
            entity.Property(e => e.GhiChu).HasDefaultValue("test");
            entity.Property(e => e.MaUserCan).HasDefaultValue("test");
        });

        modelBuilder.Entity<PhieuCanSoCheDinhHinh2>(entity =>
        {
            entity.Property(e => e.MaUserCan).HasDefaultValue("test");
        });

        modelBuilder.Entity<PhieuCanTPDinhHinh>(entity =>
        {
            entity.HasKey(e => new { e.STT, e.Ngay, e.MaMayCan, e.MaXuong }).HasName("PK_Demo_PhieuCanTPDinhHinh");

            entity.Property(e => e.MaXuong).HasDefaultValue("DT");
            entity.Property(e => e.MaUserCan).HasDefaultValue("admin");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PhieuCanTPDinhHinh2>(entity =>
        {
            entity.HasKey(e => new { e.STT, e.Ngay, e.MaMayCan, e.MaXuong }).HasName("PK_Demo_PhieuCanTPDinhHinh2");
        });

        modelBuilder.Entity<PhieuCanTPFillet>(entity =>
        {
            entity.HasKey(e => new { e.MaMayTinhCan, e.MaUserCan, e.ThoiGianCan, e.Ngay }).HasName("PK_Demo_PhieuCanTPFillet");

            entity.Property(e => e.GhiChu).HasDefaultValue("");
            entity.Property(e => e.LoaiCan)
                .HasDefaultValue("TP")
                .HasComment("TP & BTP");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PhieuCanTPFilletv2>(entity =>
        {
            entity.HasKey(e => new { e.STT, e.Ngay, e.MaMayCan, e.MaXuong }).HasName("PK_Demo_PhieuCanTPFilletv2");

            entity.Property(e => e.MaXuong).HasDefaultValue("DT");
            entity.Property(e => e.MaUserCan).HasDefaultValue("admin");
        });

        modelBuilder.Entity<PhieuCanTaiChe>(entity =>
        {
            entity.HasKey(e => new { e.STT, e.Ngay, e.MaMayCan, e.MaXuong }).HasName("PK_PhieuCanSoChe");
        });

        modelBuilder.Entity<PhieuCanTaiChe2>(entity =>
        {
            entity.HasKey(e => new { e.STT, e.Ngay, e.MaMayCan, e.MaXuong }).HasName("PK_PhieuCanSoChe2");
        });

        modelBuilder.Entity<PhieuCanVungNuoiDaiThanhSide>(entity =>
        {
            entity.Property(e => e.MaLoaiCaDaiThanhId).HasDefaultValue("N\".\"");
            entity.Property(e => e.TenGhe).HasDefaultValue("N\".\"");
            entity.Property(e => e.TenLoaiCa).HasDefaultValue("N\".\"");
        });

        modelBuilder.Entity<PhuongTienChoNguyenLieu>(entity =>
        {
            entity.Property(e => e.IsGhe).HasDefaultValue(true);
            entity.Property(e => e.IsHD).HasDefaultValue(true);
            entity.Property(e => e.SoGhe).HasDefaultValue("");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
            entity.Property(e => e.VungNuoiId).HasDefaultValue(".");
        });

        modelBuilder.Entity<PhuongTienChoNguyenLieu_temp>(entity =>
        {
            entity.Property(e => e.IsGhe).HasDefaultValue(true);
            entity.Property(e => e.IsHD).HasDefaultValue(true);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PhuongTienChoPhuPham>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<PhuongTien_TrongLuongDauAo>(entity =>
        {
            entity.Property(e => e.MaNhaCungCap).HasDefaultValue(".");
            entity.Property(e => e.Chuyen).HasDefaultValue(1);
            entity.Property(e => e.ApTai).HasDefaultValue("");
            entity.Property(e => e.CreateBy).HasDefaultValue("default");
            entity.Property(e => e.CreateDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.GioXuatPhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue("default");
            entity.Property(e => e.ModifiedDateTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayBatCa).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayXuatPhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.STTChuyen).HasDefaultValueSql("(N'1')");
            entity.Property(e => e.ThuKy).HasDefaultValue("");
        });

        modelBuilder.Entity<QuickSearchMenu>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<RP_MaThanhPhamDinhHinh>(entity =>
        {
            entity.Property(e => e.DinhMucLangDa).HasDefaultValue(1.075m);
            entity.Property(e => e.LangDa).HasDefaultValue(true);
        });

        modelBuilder.Entity<RP_MaThanhPhamFillet>(entity =>
        {
            entity.Property(e => e.DinhMuc).HasDefaultValue(1.96m);
        });

        modelBuilder.Entity<RP_MaThanhPhamNL_Fillet>(entity =>
        {
            entity.Property(e => e.DinhMuc).HasDefaultValue(1m);
            entity.Property(e => e.DinhMucCatTiet).HasDefaultValue(1m);
        });

        modelBuilder.Entity<RP_MaThanhPhamNL_SoChe>(entity =>
        {
            entity.HasKey(e => e.MaThanhPhamSoChe).HasName("PK_RP_MaThanhPhamNL_TaiChe");

            entity.Property(e => e.DinhMucCatTiet).HasDefaultValue(1m);
        });

        modelBuilder.Entity<RP_NhomThanhPham_DinhHinh_Fillet>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_RP_NhomThanhPham_DinhHinh_Fillet");

            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithMany(p => p.RefreshToken)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefreshTo__UserI__3C15C135");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC074553D291");
        });

        modelBuilder.Entity<RolePermistion>(entity =>
        {
            entity.Property(e => e.Status).HasComment("-1 không sử dụng, 1 sử dụng");
        });

        modelBuilder.Entity<SettingDashboard>(entity =>
        {
            entity.HasKey(e => e.NumberDate).HasName("PK_SettingDashboard_1");

            entity.Property(e => e.NumberDate).ValueGeneratedNever();
        });

        modelBuilder.Entity<T_BaoBi>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_BieuMauGiao_Import>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_T_BieuMauGiao");
        });

        modelBuilder.Entity<T_Bon>(entity =>
        {
            entity.Property(e => e.MaXuong).HasDefaultValue("");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
            entity.Property(e => e.TrongLuong).HasDefaultValue(100m);
            entity.Property(e => e.TrongLuongThongBao).HasDefaultValue(10m);
        });

        modelBuilder.Entity<T_ChiTietBon>(entity =>
        {
            entity.Property(e => e.GioBatDau).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayBatDau).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<T_ChiTietVoXo>(entity =>
        {
            entity.Property(e => e.MaCongDoan).HasComment("Công việc");
            entity.Property(e => e.MaThanhPham).HasComment("Nhóm Conong Việc");
        });

        modelBuilder.Entity<T_CongDoan>(entity =>
        {
            entity.Property(e => e.Idx).HasDefaultValue(1);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_CongDoanTheoThanhPham>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_DinhMuc>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_Goup>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_HoaChat>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_KhangSinh>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_KhuVuc>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_LenhSanXuat>(entity =>
        {
            entity.Property(e => e.NgayBatDau).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_LoNguyenLieu>(entity =>
        {
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_LoaiCan>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_LoaiKhuon>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_LoaiNguyenLieu>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_NhaCungCap>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_NhomHoaChat>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_NhomLo>(entity =>
        {
            entity.Property(e => e.NgayBatDau).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayNguyenLieu).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_Phieu>(entity =>
        {
            entity.Property(e => e.DenNgayGio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayGio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PCName).HasDefaultValue("default");
            entity.Property(e => e.TuNgayGio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserName).HasDefaultValue("default");
        });

        modelBuilder.Entity<T_PhieuCan>(entity =>
        {
            entity.Property(e => e.Ma).HasComment("=yyyyMMdd.xinghiepId.PCName(no space and unicode).STT");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDatKhangSinh).HasDefaultValue(true);
            entity.Property(e => e.MaLoaiCan).HasDefaultValue("TP");
            entity.Property(e => e.MaLoaiCongViec)
                .HasDefaultValue("T")
                .HasComment("T: Tay, M: Máy");
            entity.Property(e => e.MaThe).HasDefaultValueSql("((0))");
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayNguyenLieu).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SoLuong).HasDefaultValue(1m);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
            entity.Property(e => e.TrongLuongDonVi).HasDefaultValue(1m);
        });

        modelBuilder.Entity<T_PhieuCanThuMua>(entity =>
        {
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MaThe).HasDefaultValueSql("((0))");
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayGioTao).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<T_PhieuNhomYeuCau>(entity =>
        {
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_PhieuPhanCo>(entity =>
        {
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_PhieuPhanCoChiTiet>(entity =>
        {
            entity.Property(e => e.VoXo).HasDefaultValue(0m);
        });

        modelBuilder.Entity<T_PhieuYeuCau>(entity =>
        {
            entity.Property(e => e.NgayGioTao).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<T_PhuGia>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_PhuGia");

            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_PhuongTien>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_QuyCach>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_QuyTrinh>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_QuyTrinh");

            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_QuyTrinhTheoNgay>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<T_SanPham>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_Size>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_ThanhPham>(entity =>
        {
            entity.Property(e => e.DinhMuc).HasDefaultValue(1m);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_ThongTinPhu>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_ThongTinPhu");

            entity.Property(e => e.IsPhanCo).HasDefaultValue(true);
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_TieuChuan>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_TrangThaiNguyenLieu>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<T_TyLeHoaChatTheoNhom>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
            entity.Property(e => e.Ten).HasDefaultValue("Chua Ðăt Tên");
        });

        modelBuilder.Entity<ThePhieuSanLuongFillet>(entity =>
        {
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<TheRo>(entity =>
        {
            entity.Property(e => e.ColorCode).HasDefaultValue("#0000FF");
            entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<TheTu>(entity =>
        {
            entity.Property(e => e.NgayGio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PCName).HasDefaultValue(".");
        });

        modelBuilder.Entity<Thit_LoaiNguyenLieuPhaLoc>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_Thit_LoaiNguyenLieu");

            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<Thit_MaThanhPhamPhaLoc>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_Thit_MaThanhPham");

            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<Thit_PhieuCanPhaLoc>(entity =>
        {
            entity.Property(e => e.InOut).HasComment("0 nhap, 1 xuat");
        });

        modelBuilder.Entity<Thit_SizePhaLoc>(entity =>
        {
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<ToKiem>(entity =>
        {
            entity.HasKey(e => new { e.Ma, e.MaXuong }).HasName("PK_ToKiem_1");
        });

        modelBuilder.Entity<TrangThaiNhanVienTamThoi>(entity =>
        {
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.KhuVuc).HasDefaultValue("FL");
            entity.Property(e => e.Gio).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<TrongLuongBinhQuanCaChet>(entity =>
        {
            entity.Property(e => e.CreateBy).HasDefaultValue("default");
        });

        modelBuilder.Entity<UpdateSuaCa>(entity =>
        {
            entity.HasKey(e => new { e.TenFile, e.TenFolder, e.ComputerName }).HasName("PK_UpdateSuaCa_1");
        });

        modelBuilder.Entity<UserArea>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserId");

            entity.Property(e => e.NgayGio).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC0761A77614");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRole).HasConstraintName("FK__UserRole__RoleId__459F2B6F");

            entity.HasOne(d => d.User).WithMany(p => p.UserRole).HasConstraintName("FK__UserRole__UserId__44AB0736");
        });

        modelBuilder.Entity<ViTriFillet>(entity =>
        {
            entity.Property(e => e.CodeId).HasDefaultValue("");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        modelBuilder.Entity<ViewBaoCaoKeToanTheoNhaCungCap>(entity =>
        {
            entity.ToView("ViewBaoCaoKeToanTheoNhaCungCap");
        });

        modelBuilder.Entity<ViewBaoCaoKeToanTheoPhuongTien>(entity =>
        {
            entity.ToView("ViewBaoCaoKeToanTheoPhuongTien");
        });

        modelBuilder.Entity<ViewChiPhiVanChuyen>(entity =>
        {
            entity.ToView("ViewChiPhiVanChuyen");
        });

        modelBuilder.Entity<ViewTongTungAoTrongNgay>(entity =>
        {
            entity.ToView("ViewTongTungAoTrongNgay");
        });

        modelBuilder.Entity<XiNghiep>(entity =>
        {
            entity.Property(e => e.CodeId).HasDefaultValue("");
            entity.Property(e => e.SuDung).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
