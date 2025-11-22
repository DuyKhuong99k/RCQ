using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Repos.HQ
{
    public class HQ_PhieuCanXuatNguyenLieu
    {
        private readonly string connectionString;
        public HQ_PhieuCanXuatNguyenLieu(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;
        }
        public List<T> GetChiTietPhieuCaXuatNguyenLieus<T>(DateTime fromDate, DateTime toDate,string xuongId)
        {
            try
            {
                var query = @"
SELECT
    px.STT,
    pc.NgayGio,
    pc.MaLo AS LoNguyenLieu,
    px.MaThuKho,
	nv.Name as TenThuKho,
    sp.Ten AS LoaiSanPham,
	px.SoPhieuNhap,
    px.SoPhieuXuat,
    px.TrongLuongTong,
    px.TrongLuongXe,
    px.TrongLuongHang,
    dvt.Ten AS DonViTinh,
    x.Ten AS XuongXuatDen,
    kho.Ten AS Kho
FROM HQ_PhieuCanXuatNguyenLieu px
JOIN HQ_PhieuCanNguyenLieu pc
        ON px.IdPhieuCanNguyenLieu = pc.Id
LEFT JOIN HQ_SanPhamNguyenLieu sp
        ON px.MaSanPham = sp.Id
LEFT JOIN HQ_DonViTinh dvt
        ON px.MaDonVi = dvt.Id
LEFT JOIN HQ_KhoNguyenLieu kho
        ON px.MaKho = kho.Id
LEFT JOIN XiNghiep x
        ON px.MaXuongXuatDen = x.Ma
Left join NhanVienDaiThanh nv on px.MaThuKho = nv.MaNhanVien and nv.IsThuKho =1 
Where pc.MaXuong = @xuongId
and pc.NgayGio >= @fromDate and pc.NgayGio <= @toDate
ORDER BY pc.NgayGio, px.STT;
	
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
        public List<T> GetTongHopSanPhamPhieuCanXuatNguyenLieus<T>(DateTime fromDate, DateTime toDate,string xuongId)
        {
            try
            {
                var query = @"
SELECT
    pc.Ngay AS Ngay,
    pc.MaLo AS LoNguyenLieu,
    sp.Ten AS SanPham,
    SUM(px.TrongLuongHang) AS TrongLuong,
    COUNT(DISTINCT px.SoPhieuXuat) AS SoPhieuCan

FROM HQ_PhieuCanXuatNguyenLieu px
JOIN HQ_PhieuCanNguyenLieu pc
        ON px.IdPhieuCanNguyenLieu = pc.Id
LEFT JOIN HQ_SanPhamNguyenLieu sp
        ON px.MaSanPham = sp.Id
Where pc.MaXuong = @xuongId
and pc.NgayGio >= @fromDate and pc.NgayGio <= @toDate
GROUP BY
    pc.Ngay,
    pc.MaLo,
    sp.Ten

ORDER BY
    pc.Ngay,
    pc.MaLo,
    sp.Ten;
	
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
        public List<T> GetTonKhoSanPhamPhieuCanXuatNguyenLieus<T>(DateTime fromDate, DateTime toDate,string xuongId)
        {
            try
            {
                var query = @"
;WITH Nhap AS (
    SELECT
        pc.MaLo,
        pcn.MaSanPham,
        dvt.Ten AS DonViTinh,
        SUM(ISNULL(pcn.TrongLuongHang, 0)) AS TongNhap
    FROM HQ_PhieuCanNhapNguyenLieu pcn
    JOIN HQ_PhieuCanNguyenLieu pc
        ON pcn.IdPhieuCanNguyenLieu = pc.Id
    LEFT JOIN HQ_DonViTinh dvt
        ON pcn.MaDonVi = dvt.Id
    WHERE pc.MaXuong = @xuongId
      AND pc.NgayGio >= @fromDate AND pc.NgayGio <= @toDate
    GROUP BY pc.MaLo, pcn.MaSanPham, dvt.Ten
),

Xuat AS (
    SELECT
        pc.MaLo,
        pcx.MaSanPham,
        dvt.Ten AS DonViTinh,
        SUM(ISNULL(pcx.TrongLuongHang,0)) AS TongXuat
    FROM HQ_PhieuCanXuatNguyenLieu pcx
    JOIN HQ_PhieuCanNguyenLieu pc
        ON pcx.IdPhieuCanNguyenLieu = pc.Id
    LEFT JOIN HQ_DonViTinh dvt
        ON pcx.MaDonVi = dvt.Id
    WHERE pc.MaXuong = @xuongId
      AND pc.NgayGio >= @fromDate AND pc.NgayGio <= @toDate
    GROUP BY pc.MaLo, pcx.MaSanPham, dvt.Ten
)

SELECT
    COALESCE(n.MaLo, x.MaLo) AS LoNguyenLieu,
    COALESCE(n.MaSanPham, x.MaSanPham) AS MaSanPham,
    sp.Ten AS TenSanPham,
    COALESCE(n.DonViTinh, x.DonViTinh) AS DonViTinh,

    ISNULL(n.TongNhap, 0) AS TongNhap,
    ISNULL(x.TongXuat, 0) AS TongXuat,
    ISNULL(n.TongNhap, 0) - ISNULL(x.TongXuat, 0) AS TonKho

FROM Nhap n
FULL OUTER JOIN Xuat x
    ON n.MaLo = x.MaLo AND n.MaSanPham = x.MaSanPham
LEFT JOIN HQ_SanPhamNguyenLieu sp
    ON sp.Id = COALESCE(n.MaSanPham, x.MaSanPham)

ORDER BY LoNguyenLieu, TenSanPham;
	
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
    }
}
