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
    pnl.MaLo,
    pn.NgayGio,
    pn.SoPhieuCanNhap,
    pn.MaSanPham,
    pn.MaNhaCC,
    pn.MaPhuongTien,
    ncc.Ten as TenNhaCC,
    ncc.CCCD as CCCDNCC,
    pn.TaiXe,
    pn.CCCD,
    pn.SDT,
    ctn.MaQuyCach,
    qc.MaNguyenLieu,
    qc.DonViTinh,
    qc.Ten AS TenQuyCach,
    ctn.TyLe,
    qc.NhomQuyCach,
    pn.NoiDungGiaoNhan,
    qc.[Index] AS ThuTu,
    CAST(
        pn.TrongLuongHang * ISNULL(ctn.TyLe, 0) / 100.0
        AS DECIMAL(18,3)
    ) AS KhoiLuongNhap
FROM HQ_PhieuCanNhapNguyenLieu pn
JOIN (
    SELECT DISTINCT SoPhieuCanNhap, MaQuyCach, TyLe
    FROM HQ_ChiTietPhanBoTyLeNguyenLieuNhap
) ctn ON ctn.SoPhieuCanNhap = pn.SoPhieuCanNhap
JOIN HQ_PhieuCanNguyenLieu pnl
    ON pnl.Id = pn.IdPhieuCanNguyenLieu
JOIN (
    SELECT DISTINCT Id, Ten, [Index], MaNguyenLieu, DonViTinh, NhomQuyCach
    FROM HQ_QuyCachNguyenLieu
) qc ON qc.Id = ctn.MaQuyCach
JOIN NhaCungCapNguyenLieu ncc 
    ON pn.MaNhaCC = ncc.Ma 
WHERE pn.NgayGio >= @fromDate
  AND pn.NgayGio <= DATEADD(DAY, 1, @toDate)
	
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
    pnl.MaLo,
    pn.NgayGio,
    pn.SoPhieuCanNhap,
    pn.MaSanPham,
    pn.MaNhaCC,
    ncc.Ten as TenNhaCC,
    ncc.CCCD as CCCDNCC,
    pn.TaiXe,
    pn.CCCD,
    pn.SDT,
    pn.MaPhuongTien,
    pn.LoaiGiaoDich,
    ctn.MaQuyCach,
    qc.MaNguyenLieu,
    qc.DonViTinh,
    qc.Ten AS TenQuyCach,
    ctn.TyLe,
    qc.NhomQuyCach,
    pn.NoiDungGiaoNhan,
    qc.[Index] AS ThuTu,
    CAST(
        pn.TrongLuongHang * ISNULL(ctn.TyLe, 0) / 100.0
        AS DECIMAL(18,1)
    ) AS KhoiLuongNhap
FROM HQ_PhieuCanNhapNguyenLieu pn
JOIN (
    SELECT DISTINCT SoPhieuCanNhap, MaQuyCach, TyLe
    FROM HQ_ChiTietPhanBoTyLeNguyenLieuNhap
) ctn ON ctn.SoPhieuCanNhap = pn.SoPhieuCanNhap
JOIN HQ_PhieuCanNguyenLieu pnl
    ON pnl.Id = pn.IdPhieuCanNguyenLieu
JOIN (
    SELECT DISTINCT Id, Ten, [Index], MaNguyenLieu, DonViTinh, NhomQuyCach
    FROM HQ_QuyCachNguyenLieu
) qc ON qc.Id = ctn.MaQuyCach
LEFT JOIN NhaCungCapNguyenLieu ncc 
    ON pn.MaNhaCC = ncc.Ma 
WHERE pn.NgayGio >= @fromDate
  AND pn.NgayGio <= DATEADD(DAY, 1, @toDate)
	
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
