using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Dao.Repos.HQ
{
    public class HQ_PhieuCanNhapNguyenLieu
    {
        private readonly string connectionString;
        public HQ_PhieuCanNhapNguyenLieu(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;
        }

        public List<T> GetChiTietPhieuCanNhapNguyenLieus<T>(DateTime dateTime,string xuongId)
        {
            try
            {
                var query = @"
SELECT 
	-- tt chung
    pc.MaLo AS LoNguyenLieu,
    pc.Ngay AS NgayNguyenLieu,
    pcnn.SoPhieuCanNhap AS SoPhieu,
    pcnn.SoLanSua AS SoLanSuaDoi,
    kho.Ten AS DiaDiem,
    pt.Ten AS NguoiGiao,
    pcnn.TaiXe,
    pt.Ten AS SoXe,
    pcnn.CCCD,
    pc.NgayGio AS ThoiGian,
    pcnn.SDT,
    pcnn.NoiDungGiaoNhan,
	-- tt sản phẩm
    pcnn.STT,
    sp.Ten AS TenHang,
    qc.Ten AS QuyCachSanPham,
    dvt.Ten AS DonViTinh,
    cl.Ten AS ChatLuong,
    CASE pcnn.CanHang 
        WHEN 1 THEN N'Hợp Lý'
        WHEN 0 THEN N'Không Hợp Lý'
        ELSE NULL END AS CanHang,
    pcnn.TruBi, 
    CASE 
        WHEN pcnn.MaChatLuong IS NOT NULL THEN N'Đúng'
        ELSE N'Không Đúng'
    END AS PhanLoaiNguyenLieu,
    pc.GhiChu AS NhanXet,
    pcnn.TrongLuongHang,
    pcnn.TrongLuongXe,
cast(
    pcnn.TrongLuongHang * ctsp.TyLe / 100.0
    as decimal(18, 3)
) as TrongLuongPhanBo

FROM HQ_PhieuCanNhapNguyenLieu pcnn
JOIN HQ_PhieuCanNguyenLieu pc 
    ON pcnn.IdPhieuCanNguyenLieu = pc.Id
LEFT JOIN HQ_KhoNguyenLieu kho
    ON pcnn.MaKho = kho.Id
LEFT JOIN HQ_SanPhamNguyenLieu sp
    ON pcnn.MaSanPham = sp.Id
left join HQ_ChiTietPhanBoTyLeNguyenLieuNhap ctsp 
                    on ctsp.SoPhieuCanNhap = pcnn.SoPhieuCanNhap
                left join HQ_QuyCachNguyenLieu qc 
                    on qc.Id = ctsp.MaQuyCach
LEFT JOIN HQ_DonViTinh dvt
    ON pcnn.MaDonVi = dvt.Id
LEFT JOIN HQ_ChatLuongNguyenLieu cl
    ON pcnn.MaChatLuong = cl.Id
LEFT JOIN PhuongTienChoNguyenLieu pt
    ON pcnn.MaPhuongTien = pt.Ma
Where pc.MaXuong = @xuongId
and pc.Ngay >= @dateTime
	
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { dateTime ,xuongId}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        public List<T> GetChiTietPhieuCanNhapNguyenLieus<T>(DateTime fromDate, DateTime toDate,string xuongId)
        {
            try
            {
                var query = @"SELECT 
	-- tt chung
    pc.MaLo AS LoNguyenLieu,
    pc.Ngay AS NgayNguyenLieu,
    pcnn.SoPhieuCanNhap AS SoPhieu,
    pcnn.SoLanSua AS SoLanSuaDoi,
    kho.Ten AS DiaDiem,
    pt.Ten AS NguoiGiao,
    pcnn.TaiXe,
    pt.Ten AS SoXe,
    pcnn.CCCD,
    pc.NgayGio AS ThoiGian,
    pcnn.SDT,
    pcnn.NoiDungGiaoNhan,
	-- tt sản phẩm
    pcnn.STT,
    sp.Ten AS TenHang,
    qc.Ten AS QuyCachSanPham,
    dvt.Ten AS DonViTinh,
    cl.Ten AS ChatLuong,
    CASE pcnn.CanHang 
        WHEN 1 THEN N'Hợp Lý'
        WHEN 0 THEN N'Không Hợp Lý'
        ELSE NULL END AS CanHang,
    pcnn.TruBi, 
    CASE 
        WHEN pcnn.MaChatLuong IS NOT NULL THEN N'Đúng'
        ELSE N'Không Đúng'
    END AS PhanLoaiNguyenLieu,
    pc.GhiChu AS NhanXet,
    pcnn.TrongLuongHang,
    pcnn.TrongLuongXe,
cast(
    pcnn.TrongLuongHang * ctsp.TyLe / 100.0
    as decimal(18, 3)
) as TrongLuongPhanBo

FROM HQ_PhieuCanNhapNguyenLieu pcnn
JOIN HQ_PhieuCanNguyenLieu pc 
    ON pcnn.IdPhieuCanNguyenLieu = pc.Id
LEFT JOIN HQ_KhoNguyenLieu kho
    ON pcnn.MaKho = kho.Id
LEFT JOIN HQ_SanPhamNguyenLieu sp
    ON pcnn.MaSanPham = sp.Id
left join HQ_ChiTietPhanBoTyLeNguyenLieuNhap ctsp 
                    on ctsp.SoPhieuCanNhap = p.SoPhieuCanNhap
                left join HQ_QuyCachNguyenLieu qc 
                    on qc.Id = ctsp.MaQuyCach
LEFT JOIN HQ_DonViTinh dvt
    ON pcnn.MaDonVi = dvt.Id
LEFT JOIN HQ_ChatLuongNguyenLieu cl
    ON pcnn.MaChatLuong = cl.Id
LEFT JOIN PhuongTienChoNguyenLieu pt
    ON pcnn.MaPhuongTien = pt.Ma
Where pc.MaXuong = @xuongId
and pc.NgayGio >= @fromDate and pc.NgayGio <= @toDate
	
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate, toDate ,xuongId}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetTongHopSanPhamPhieuCanNguyenLieus<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"SELECT 
    pc.Ngay,
    pc.MaLo as LoNguyenLieu,
    sp.Ten as TenHang,
    dvt.Ten as DonViTinh,
    SUM(ISNULL(PCN.TrongLuongHang, 0)) as SoLuong
FROM HQ_PhieuCanNhapNguyenLieu pcn
INNER JOIN HQ_PhieuCanNguyenLieu pc
        ON PCN.IdPhieuCanNguyenLieu = pc.Id
LEFT JOIN HQ_SanPhamNguyenLieu sp
        ON PCN.MaSanPham = sp.Id
LEFT JOIN HQ_DonViTinh dvt
        ON PCN.MaDonVi = dvt.Id
Where pc.MaXuong = @xuongId
and pc.NgayGio >= @fromDate and pc.NgayGio <= @toDate
GROUP BY 
    PC.Ngay,
    PC.MaLo,
    SP.Ten,
    DVT.Ten
ORDER BY 
    PC.Ngay,
    PC.MaLo,
    SP.Ten;
	
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate, toDate, xuongId }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
