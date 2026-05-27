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
                        ROUND(px.TrongLuongTong, 1) AS TrongLuongTong,
                        ROUND(px.TrongLuongXe, 1) AS TrongLuongXe,
                        ROUND(px.TrongLuongHang, 1) AS TrongLuongHang,
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
                    Left join NhanVienDaiThanh nv on px.MaThuKho = nv.MaNhanVien 
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
    ROUND(SUM(px.TrongLuongHang),1) AS TrongLuong,
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

        ROUND(SUM(ISNULL(pcn.TrongLuongHang, 0)), 1) AS TongNhap

    FROM HQ_PhieuCanNhapNguyenLieu pcn

    JOIN HQ_PhieuCanNguyenLieu pc
        ON pcn.IdPhieuCanNguyenLieu = pc.Id

    LEFT JOIN HQ_DonViTinh dvt
        ON pcn.MaDonVi = dvt.Id

    WHERE pc.MaXuong = @xuongId
        AND pc.NgayGio >= @fromDate
        AND pc.NgayGio <= @toDate

    GROUP BY
        pc.MaLo,
        pcn.MaSanPham,
        dvt.Ten
),

Xuat AS (
    SELECT
        pc.MaLo,
        pcx.MaSanPham,
        dvt.Ten AS DonViTinh,

        ROUND(SUM(ISNULL(pcx.TrongLuongHang, 0)), 1) AS TongXuat

    FROM HQ_PhieuCanXuatNguyenLieu pcx

    JOIN HQ_PhieuCanNguyenLieu pc
        ON pcx.IdPhieuCanNguyenLieu = pc.Id

    LEFT JOIN HQ_DonViTinh dvt
        ON pcx.MaDonVi = dvt.Id

    WHERE pc.MaXuong = @xuongId
        AND pc.NgayGio >= @fromDate
        AND pc.NgayGio <= @toDate

    GROUP BY
        pc.MaLo,
        pcx.MaSanPham,
        dvt.Ten
)

SELECT
    COALESCE(n.MaLo, x.MaLo) AS LoNguyenLieu,
    COALESCE(n.MaSanPham, x.MaSanPham) AS MaSanPham,
    sp.Ten AS TenSanPham,
    COALESCE(n.DonViTinh, x.DonViTinh) AS DonViTinh,

    ROUND(ISNULL(n.TongNhap, 0), 1) AS TongNhap,
    ROUND(ISNULL(x.TongXuat, 0), 1) AS TongXuat,

    ROUND(
        ISNULL(n.TongNhap, 0) - ISNULL(x.TongXuat, 0),
        1
    ) AS TonKho

FROM Nhap n

FULL OUTER JOIN Xuat x
    ON n.MaLo = x.MaLo
    AND n.MaSanPham = x.MaSanPham

LEFT JOIN HQ_SanPhamNguyenLieu sp
    ON sp.Id = COALESCE(n.MaSanPham, x.MaSanPham)

ORDER BY
    LoNguyenLieu,
    TenSanPham;
	
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



        public List<T> GetTyLeNguyenLieuHaoHut<T>(DateTime ngay)
        {
            try
            {
                //                var query = @"
                //                ------------------------------------------------
                //                --LÔ CÓ XUẤT HÔM NAY
                //------------------------------------------------
                //WITH LoXuatHomNay AS(
                //    SELECT DISTINCT pnl.MaLo
                //    FROM HQ_PhieuCanXuatNguyenLieu px
                //    JOIN HQ_PhieuCanNguyenLieu pnl
                //        ON pnl.Id = px.IdPhieuCanNguyenLieu
                //    WHERE px.NgayGio >= @ngay
                //      AND px.NgayGio < DATEADD(DAY, 1, @ngay)
                //),

                //------------------------------------------------
                //--NHẬP THEO QUY CÁCH
                //------------------------------------------------
                //NhapTheoQuyCach AS(
                //    SELECT
                //        pnl.MaLo,
                //        pn.NgayGio,
                //        pn.SoPhieuCanNhap,
                //        pn.MaSanPham,
                //        ctn.MaQuyCach,
                //        qc.Ten AS TenQuyCach,
                //        ctn.TyLe,
                //        qc.[Index] AS ThuTu,
                //        CAST(
                //            pn.TrongLuongHang* ISNULL(ctn.TyLe, 0) / 100.0
                //            AS DECIMAL(18,3)
                //        ) AS KhoiLuongNhap
                //    FROM HQ_PhieuCanNhapNguyenLieu pn
                //    JOIN(
                //        SELECT DISTINCT SoPhieuCanNhap, MaQuyCach, TyLe
                //        FROM HQ_ChiTietPhanBoTyLeNguyenLieuNhap
                //    ) ctn ON ctn.SoPhieuCanNhap = pn.SoPhieuCanNhap
                //    JOIN HQ_PhieuCanNguyenLieu pnl
                //        ON pnl.Id = pn.IdPhieuCanNguyenLieu
                //    JOIN(
                //        SELECT DISTINCT Id, Ten, [Index]
                //        FROM HQ_QuyCachNguyenLieu
                //    ) qc ON qc.Id = ctn.MaQuyCach
                //    WHERE pnl.MaLo IN(SELECT MaLo FROM LoXuatHomNay)
                //),

                //------------------------------------------------
                //--XUẤT TRƯỚC NGÀY
                //------------------------------------------------
                //XuatTruoc AS(
                //    SELECT
                //        pnl.MaLo,
                //        SUM(CASE WHEN ISNULL(px.IsCanHu, 0) = 0 THEN px.TrongLuongHang ELSE 0 END) AS XuatThuongTruoc,
                //        SUM(CASE WHEN px.IsCanHu = 1 THEN px.TrongLuongHang ELSE 0 END) AS XuatHuTruoc
                //    FROM HQ_PhieuCanXuatNguyenLieu px
                //    JOIN HQ_PhieuCanNguyenLieu pnl
                //        ON pnl.Id = px.IdPhieuCanNguyenLieu
                //    WHERE px.NgayGio < @ngay
                //    GROUP BY pnl.MaLo
                //),

                //------------------------------------------------
                //--XUẤT HÔM NAY
                //------------------------------------------------
                //XuatHomNay AS(
                //    SELECT
                //        pnl.MaLo,
                //        SUM(CASE WHEN ISNULL(px.IsCanHu, 0) = 0 THEN px.TrongLuongHang ELSE 0 END) AS XuatThuongHomNay,
                //        SUM(CASE WHEN px.IsCanHu = 1 THEN px.TrongLuongHang ELSE 0 END) AS XuatHuHomNay,
                //        SUM(px.TrongLuongHang) AS TongDaXuat
                //    FROM HQ_PhieuCanXuatNguyenLieu px
                //    JOIN HQ_PhieuCanNguyenLieu pnl
                //        ON pnl.Id = px.IdPhieuCanNguyenLieu
                //    WHERE px.NgayGio >= @ngay
                //      AND px.NgayGio < DATEADD(DAY, 1, @ngay)
                //      AND px.IsHuy = 0
                //    GROUP BY pnl.MaLo
                //),

                //------------------------------------------------
                //--GỘP XUẤT
                //------------------------------------------------
                //TongHop AS(
                //    SELECT
                //        n.*,
                //		ISNULL(t2.XuatThuongHomNay, 0) AS XuatThuongHomNay,
                //        ISNULL(t2.XuatHuHomNay, 0) AS XuatHuHomNay,
                //        ISNULL(t1.XuatThuongTruoc, 0) +ISNULL(t2.XuatThuongHomNay, 0) AS TongXuatThuong,
                //        ISNULL(t1.XuatHuTruoc, 0) +ISNULL(t2.XuatHuHomNay, 0) AS TongXuatHu
                //    FROM NhapTheoQuyCach n
                //    LEFT JOIN XuatTruoc t1 ON t1.MaLo = n.MaLo
                //    LEFT JOIN XuatHomNay t2 ON t2.MaLo = n.MaLo
                //),

                //------------------------------------------------
                //--LŨY KẾ FIFO
                //------------------------------------------------
                //TinhLuyKe AS(
                //    SELECT
                //        t.*,
                //        SUM(
                //            CASE WHEN t.ThuTu >= 1 THEN t.KhoiLuongNhap ELSE 0 END
                //        ) OVER(
                //            PARTITION BY t.MaLo
                //            ORDER BY t.ThuTu
                //        ) AS LuyKeThuong
                //    FROM TongHop t
                //)

                //------------------------------------------------
                //-- FINAL
                //------------------------------------------------
                //SELECT
                //    TenQuyCach,
                //    MaLo,
                //    KhoiLuongNhap,

                //    ------------------------------------------------
                //    --TỒN KHO CŨ
                //    ------------------------------------------------
                //    --CAST(
                //    --KhoiLuongNhap -
                //    --(
                //    --CASE
                //    --            WHEN ThuTu = 0 THEN
                //    --                CASE
                //    --                    WHEN TongXuatHu <= 0 THEN 0
                //    --                    WHEN TongXuatHu >= KhoiLuongNhap THEN KhoiLuongNhap
                //    --                    ELSE TongXuatHu
                //    --                END
                //    --            ELSE
                //    --                CASE
                //    --                    WHEN TongXuatThuong <= 0 THEN 0
                //    --                    WHEN TongXuatThuong >= LuyKeThuong THEN KhoiLuongNhap
                //    --                    ELSE TongXuatThuong - (LuyKeThuong - KhoiLuongNhap)
                //    --                END
                //    --        END
                //    --)
                //    --    AS DECIMAL(18,3)
                //    --) AS TonKhoCu,
                //    CAST(
                //		KhoiLuongNhap -
                //        (
                //            CASE

                //                WHEN ThuTu = 0 THEN

                //                    CASE

                //                        WHEN(TongXuatHu - ISNULL(XuatHuHomNay, 0)) <= 0 THEN 0

                //                        WHEN(TongXuatHu - ISNULL(XuatHuHomNay, 0)) >= KhoiLuongNhap THEN KhoiLuongNhap

                //                        ELSE(TongXuatHu - ISNULL(XuatHuHomNay, 0))

                //                    END

                //                ELSE

                //                    CASE

                //                        WHEN(TongXuatThuong - ISNULL(XuatThuongHomNay, 0)) <= 0 THEN 0

                //                        WHEN(TongXuatThuong - ISNULL(XuatThuongHomNay, 0)) >= LuyKeThuong THEN KhoiLuongNhap

                //                        ELSE(TongXuatThuong - ISNULL(XuatThuongHomNay, 0))
                //                             - (LuyKeThuong - KhoiLuongNhap)

                //                    END

                //            END
                //		)

                //    AS DECIMAL(18,3)
                //	) AS TonKhoCu,
                //    CASE WHEN ThuTu = 0 THEN TyLe ELSE NULL END AS TyLeKhongDat,

                //    SUM(CASE WHEN ThuTu<> 0 THEN TyLe ELSE 0 END)
                //        OVER(PARTITION BY MaLo) AS TyLeTinhTien,

                //    ------------------------------------------------
                //    --TỔNG LƯỢNG XUẤT
                //    ------------------------------------------------
                //    CAST(
                //        CASE
                //            WHEN ThuTu = 0 THEN
                //                CASE
                //                    WHEN TongXuatHu <= 0 THEN 0
                //                    WHEN TongXuatHu >= KhoiLuongNhap THEN KhoiLuongNhap
                //                    ELSE TongXuatHu
                //                END
                //            ELSE
                //                CASE
                //                    WHEN TongXuatThuong <= 0 THEN 0
                //                    WHEN TongXuatThuong >= LuyKeThuong THEN KhoiLuongNhap
                //                    ELSE TongXuatThuong - (LuyKeThuong - KhoiLuongNhap)
                //                END
                //        END
                //        AS DECIMAL(18, 3)
                //    ) AS TongLuongXuat,

                //    ------------------------------------------------
                //    --TỒN CÒN LẠI
                //    ------------------------------------------------
                //    CAST(
                //        KhoiLuongNhap -
                //        CASE
                //            WHEN ThuTu = 0 THEN
                //                CASE
                //                    WHEN TongXuatHu <= 0 THEN 0
                //                    WHEN TongXuatHu >= KhoiLuongNhap THEN KhoiLuongNhap
                //                    ELSE TongXuatHu
                //                END
                //            ELSE
                //                CASE
                //                    WHEN TongXuatThuong <= 0 THEN 0
                //                    WHEN TongXuatThuong >= LuyKeThuong THEN KhoiLuongNhap
                //                    ELSE TongXuatThuong - (LuyKeThuong - KhoiLuongNhap)
                //                END
                //        END
                //        AS DECIMAL(18, 3)
                //    ) AS TonConLai,

                //    ------------------------------------------------
                //    --KHỐI LƯỢNG HƯ
                //    ------------------------------------------------
                //    CAST(
                //        CASE WHEN ThuTu = 0 THEN
                //            CASE
                //                WHEN TongXuatHu <= 0 THEN 0
                //                WHEN TongXuatHu >= KhoiLuongNhap THEN KhoiLuongNhap
                //                ELSE TongXuatHu
                //            END
                //        ELSE 0 END
                //        AS DECIMAL(18, 3)
                //    ) AS KhoiLuongIsCanHu,

                //    ------------------------------------------------
                //    --HAO HỤT
                //    ------------------------------------------------
                //    CAST(
                //        CASE WHEN ThuTu = 0 THEN
                //            CASE
                //                WHEN TongXuatHu <= 0 THEN 0
                //                WHEN TongXuatHu >= KhoiLuongNhap THEN KhoiLuongNhap
                //                ELSE TongXuatHu
                //            END
                //        ELSE 0 END
                //        AS DECIMAL(18, 3)
                //    ) AS HaoHut,

                //    CAST(
                //    (
                //        KhoiLuongNhap -
                //        (
                //            CASE

                //                WHEN ThuTu = 0 THEN

                //                    CASE

                //                        WHEN(TongXuatHu - ISNULL(XuatHuHomNay, 0)) <= 0 THEN 0

                //                        WHEN(TongXuatHu - ISNULL(XuatHuHomNay, 0)) >= KhoiLuongNhap THEN KhoiLuongNhap

                //                        ELSE(TongXuatHu - ISNULL(XuatHuHomNay, 0))

                //                    END

                //                ELSE

                //                    CASE

                //                        WHEN(TongXuatThuong - ISNULL(XuatThuongHomNay, 0)) <= 0 THEN 0

                //                        WHEN(TongXuatThuong - ISNULL(XuatThuongHomNay, 0)) >= LuyKeThuong THEN KhoiLuongNhap

                //                        ELSE(TongXuatThuong - ISNULL(XuatThuongHomNay, 0))
                //                             - (LuyKeThuong - KhoiLuongNhap)

                //                    END

                //            END
                //		)
                //    )
                //    -
                //    (
                //        KhoiLuongNhap -
                //        CASE
                //            WHEN ThuTu = 0 THEN
                //                CASE
                //                    WHEN TongXuatHu <= 0 THEN 0
                //                    WHEN TongXuatHu >= KhoiLuongNhap THEN KhoiLuongNhap
                //                    ELSE TongXuatHu
                //                END
                //            ELSE
                //                CASE
                //                    WHEN TongXuatThuong <= 0 THEN 0
                //                    WHEN TongXuatThuong >= LuyKeThuong THEN KhoiLuongNhap
                //                    ELSE TongXuatThuong -(LuyKeThuong - KhoiLuongNhap)
                //                END
                //        END
                //    )
                //AS DECIMAL(18,3)
                //) AS TongDaXuat

                //FROM TinhLuyKe
                //ORDER BY MaLo, ThuTu;

                //                ";
                var query = @"
                ------------------------------------------------
                --LÔ CÓ XUẤT HÔM NAY(KHÔNG BỊ HUỶ)
------------------------------------------------
WITH LoXuatHomNay AS(
    SELECT DISTINCT pnl.MaLo
    FROM HQ_PhieuCanXuatNguyenLieu px
    JOIN HQ_PhieuCanNguyenLieu pnl
        ON pnl.Id = px.IdPhieuCanNguyenLieu
    WHERE px.NgayGio >= @ngay
      AND px.NgayGio < DATEADD(DAY, 1, @ngay)
      AND px.IsHuy = 0
),

------------------------------------------------
--NHẬP THEO QUY CÁCH
------------------------------------------------
NhapTheoQuyCach AS(
    SELECT
        pnl.MaLo,
        pn.NgayGio,
        pn.SoPhieuCanNhap,
        pn.MaSanPham,
        ctn.MaQuyCach,
        qc.Ten AS TenQuyCach,
        ctn.TyLe,
        qc.[Index] AS ThuTu,
        CAST(
            pn.TrongLuongHang* ISNULL(ctn.TyLe,0) / 100.0
            AS DECIMAL(18,1)
        ) AS KhoiLuongNhap
    FROM HQ_PhieuCanNhapNguyenLieu pn
    JOIN(
        SELECT DISTINCT SoPhieuCanNhap, MaQuyCach, TyLe
        FROM HQ_ChiTietPhanBoTyLeNguyenLieuNhap
    ) ctn
        ON ctn.SoPhieuCanNhap = pn.SoPhieuCanNhap
    JOIN HQ_PhieuCanNguyenLieu pnl
        ON pnl.Id = pn.IdPhieuCanNguyenLieu
    JOIN(
        SELECT DISTINCT Id, Ten, [Index]
        FROM HQ_QuyCachNguyenLieu
    ) qc
        ON qc.Id = ctn.MaQuyCach
    WHERE pnl.MaLo IN(SELECT MaLo FROM LoXuatHomNay) and pn.IsXoa = 0
),

------------------------------------------------
--XUẤT TRƯỚC NGÀY(KHÔNG TÍNH PHIẾU HUỶ)
------------------------------------------------
XuatTruoc AS(
    SELECT
        pnl.MaLo,
        SUM(CASE WHEN ISNULL(px.IsCanHu, 0) = 0 THEN px.TrongLuongHang ELSE 0 END) AS XuatThuongTruoc,
        SUM(CASE WHEN px.IsCanHu = 1 THEN px.TrongLuongHang ELSE 0 END) AS XuatHuTruoc
    FROM HQ_PhieuCanXuatNguyenLieu px
    JOIN HQ_PhieuCanNguyenLieu pnl
        ON pnl.Id = px.IdPhieuCanNguyenLieu
    WHERE px.NgayGio < @ngay
      AND px.IsHuy = 0
    GROUP BY pnl.MaLo
),

------------------------------------------------
--XUẤT HÔM NAY(KHÔNG TÍNH PHIẾU HUỶ)
------------------------------------------------
XuatHomNay AS(
    SELECT
        pnl.MaLo,
        SUM(CASE WHEN ISNULL(px.IsCanHu, 0) = 0 THEN px.TrongLuongHang ELSE 0 END) AS XuatThuongHomNay,
        SUM(CASE WHEN px.IsCanHu = 1 THEN px.TrongLuongHang ELSE 0 END) AS XuatHuHomNay
    FROM HQ_PhieuCanXuatNguyenLieu px
    JOIN HQ_PhieuCanNguyenLieu pnl
        ON pnl.Id = px.IdPhieuCanNguyenLieu
    WHERE px.NgayGio >= @ngay
      AND px.NgayGio < DATEADD(DAY, 1, @ngay)
      AND px.IsHuy = 0
    GROUP BY pnl.MaLo
),

------------------------------------------------
--GỘP XUẤT
------------------------------------------------
TongHop AS(
    SELECT
        n.*,
        ISNULL(t2.XuatThuongHomNay, 0) AS XuatThuongHomNay,
        ISNULL(t2.XuatHuHomNay, 0) AS XuatHuHomNay,
        ISNULL(t1.XuatThuongTruoc, 0) +ISNULL(t2.XuatThuongHomNay, 0) AS TongXuatThuong,
        ISNULL(t1.XuatHuTruoc, 0) +ISNULL(t2.XuatHuHomNay, 0) AS TongXuatHu
    FROM NhapTheoQuyCach n
    LEFT JOIN XuatTruoc t1
        ON t1.MaLo = n.MaLo
    LEFT JOIN XuatHomNay t2
        ON t2.MaLo = n.MaLo
),

------------------------------------------------
--LŨY KẾ FIFO
------------------------------------------------
TinhLuyKe AS(
    SELECT
        t.*,
        SUM(
            CASE WHEN t.ThuTu >= 1 THEN t.KhoiLuongNhap ELSE 0 END
        ) OVER(
            PARTITION BY t.MaLo
            ORDER BY t.ThuTu
        ) AS LuyKeThuong
    FROM TongHop t
)

------------------------------------------------
-- FINAL
------------------------------------------------
SELECT
    TenQuyCach,
    MaLo,
    KhoiLuongNhap,

------------------------------------------------
--TỒN KHO CŨ
------------------------------------------------
CAST(
    KhoiLuongNhap -
    (
        CASE
            WHEN ThuTu = 0 THEN
                CASE
                    WHEN(TongXuatHu - ISNULL(XuatHuHomNay, 0)) <= 0 THEN 0
                    WHEN(TongXuatHu - ISNULL(XuatHuHomNay, 0)) >= KhoiLuongNhap THEN KhoiLuongNhap
                    ELSE(TongXuatHu - ISNULL(XuatHuHomNay, 0))
                END
            ELSE
                CASE
                    WHEN(TongXuatThuong - ISNULL(XuatThuongHomNay, 0)) <= 0 THEN 0
                    WHEN(TongXuatThuong - ISNULL(XuatThuongHomNay, 0)) >= LuyKeThuong THEN KhoiLuongNhap
                    ELSE(TongXuatThuong - ISNULL(XuatThuongHomNay, 0))
                         - (LuyKeThuong - KhoiLuongNhap)
                END
        END
    )
AS DECIMAL(18,1)
) AS TonKhoCu,

CASE WHEN ThuTu = 0 THEN TyLe ELSE NULL END AS TyLeKhongDat,

SUM(CASE WHEN ThuTu<> 0 THEN TyLe ELSE 0 END)
    OVER(PARTITION BY MaLo) AS TyLeTinhTien,

------------------------------------------------
--TỔNG LƯỢNG XUẤT
------------------------------------------------
CAST(
    CASE
        WHEN ThuTu = 0 THEN
            CASE
                WHEN TongXuatHu <= 0 THEN 0
                WHEN TongXuatHu >= KhoiLuongNhap THEN KhoiLuongNhap
                ELSE TongXuatHu
            END
        ELSE
            CASE
                WHEN TongXuatThuong <= 0 THEN 0
                WHEN TongXuatThuong >= LuyKeThuong THEN KhoiLuongNhap
                ELSE TongXuatThuong - (LuyKeThuong - KhoiLuongNhap)
            END
    END
AS DECIMAL(18, 1)
) AS TongLuongXuat,

------------------------------------------------
--TỒN CÒN LẠI
------------------------------------------------
CAST(
    KhoiLuongNhap -
    CASE
        WHEN ThuTu = 0 THEN
            CASE
                WHEN TongXuatHu <= 0 THEN 0
                WHEN TongXuatHu >= KhoiLuongNhap THEN KhoiLuongNhap
                ELSE TongXuatHu
            END
        ELSE
            CASE
                WHEN TongXuatThuong <= 0 THEN 0
                WHEN TongXuatThuong >= LuyKeThuong THEN KhoiLuongNhap
                ELSE TongXuatThuong - (LuyKeThuong - KhoiLuongNhap)
            END
    END
AS DECIMAL(18, 1)
) AS TonConLai,

------------------------------------------------
--KHỐI LƯỢNG HƯ
------------------------------------------------
CAST(
    CASE
        WHEN ThuTu = 0 THEN
            CASE
                WHEN TongXuatHu <= 0 THEN 0
                WHEN TongXuatHu >= KhoiLuongNhap THEN KhoiLuongNhap
                ELSE TongXuatHu
            END
        ELSE 0
    END
AS DECIMAL(18, 1)
) AS KhoiLuongIsCanHu,

------------------------------------------------
--HAO HỤT
------------------------------------------------
CAST(
    CASE
        WHEN ThuTu = 0 THEN
            CASE
                WHEN TongXuatHu <= 0 THEN 0
                WHEN TongXuatHu >= KhoiLuongNhap THEN KhoiLuongNhap
                ELSE TongXuatHu
            END
        ELSE 0
    END
AS DECIMAL(18, 1)
) AS HaoHut

FROM TinhLuyKe
ORDER BY MaLo, ThuTu;
                ";
                //                var query = @"
                //------------------------------------------------
                //-- LÔ CÓ XUẤT HÔM NAY (KHÔNG HUỶ)
                //------------------------------------------------
                //;WITH LoXuatHomNay AS
                //(
                //    SELECT DISTINCT pnl.MaLo
                //    FROM HQ_PhieuCanXuatNguyenLieu px
                //    JOIN HQ_PhieuCanNguyenLieu pnl
                //        ON pnl.Id = px.IdPhieuCanNguyenLieu
                //    WHERE px.NgayGio >= @ngay
                //      AND px.NgayGio < DATEADD(DAY,1,@ngay)
                //      AND px.IsHuy = 0
                //),

                //------------------------------------------------
                //-- NHẬP THEO QUY CÁCH
                //------------------------------------------------
                //NhapTheoQuyCach AS
                //(
                //    SELECT
                //        pnl.MaLo,
                //        pn.NgayGio,
                //        pn.SoPhieuCanNhap,
                //        pn.MaSanPham,
                //        ctn.MaQuyCach,
                //        qc.Ten AS TenQuyCach,
                //        ctn.TyLe,
                //        qc.[Index] AS ThuTu,

                //        CAST(
                //            pn.TrongLuongHang * ISNULL(ctn.TyLe,0) / 100.0
                //            AS DECIMAL(18,3)
                //        ) AS KhoiLuongNhap

                //    FROM HQ_PhieuCanNhapNguyenLieu pn

                //    JOIN
                //    (
                //        SELECT DISTINCT SoPhieuCanNhap, MaQuyCach, TyLe
                //        FROM HQ_ChiTietPhanBoTyLeNguyenLieuNhap
                //    ) ctn
                //        ON ctn.SoPhieuCanNhap = pn.SoPhieuCanNhap

                //    JOIN HQ_PhieuCanNguyenLieu pnl
                //        ON pnl.Id = pn.IdPhieuCanNguyenLieu

                //    JOIN
                //    (
                //        SELECT DISTINCT Id, Ten, [Index]
                //        FROM HQ_QuyCachNguyenLieu
                //    ) qc
                //        ON qc.Id = ctn.MaQuyCach

                //    WHERE pnl.MaLo IN (SELECT MaLo FROM LoXuatHomNay)
                //),

                //------------------------------------------------
                //-- XUẤT TRƯỚC NGÀY
                //------------------------------------------------
                //XuatTruoc AS
                //(
                //    SELECT
                //        pnl.MaLo,

                //        SUM(CASE WHEN ISNULL(px.IsCanHu,0)=0
                //            THEN px.TrongLuongHang ELSE 0 END) AS XuatThuongTruoc,

                //        SUM(CASE WHEN px.IsCanHu=1
                //            THEN px.TrongLuongHang ELSE 0 END) AS XuatHuTruoc

                //    FROM HQ_PhieuCanXuatNguyenLieu px

                //    JOIN HQ_PhieuCanNguyenLieu pnl
                //        ON pnl.Id = px.IdPhieuCanNguyenLieu

                //    WHERE px.NgayGio < @ngay
                //      AND px.IsHuy = 0

                //    GROUP BY pnl.MaLo
                //),

                //------------------------------------------------
                //-- XUẤT HÔM NAY
                //------------------------------------------------
                //XuatHomNay AS
                //(
                //    SELECT
                //        pnl.MaLo,

                //        SUM(CASE WHEN ISNULL(px.IsCanHu,0)=0
                //            THEN px.TrongLuongHang ELSE 0 END) AS XuatThuongHomNay,

                //        SUM(CASE WHEN px.IsCanHu=1
                //            THEN px.TrongLuongHang ELSE 0 END) AS XuatHuHomNay

                //    FROM HQ_PhieuCanXuatNguyenLieu px

                //    JOIN HQ_PhieuCanNguyenLieu pnl
                //        ON pnl.Id = px.IdPhieuCanNguyenLieu

                //    WHERE px.NgayGio >= @ngay
                //      AND px.NgayGio < DATEADD(DAY,1,@ngay)
                //      AND px.IsHuy = 0

                //    GROUP BY pnl.MaLo
                //),

                //------------------------------------------------
                //-- GỘP XUẤT
                //------------------------------------------------
                //TongHop AS
                //(
                //    SELECT
                //        n.*,

                //        ISNULL(t1.XuatThuongTruoc,0)
                //        + ISNULL(t2.XuatThuongHomNay,0) AS TongXuatThuong,

                //        ISNULL(t1.XuatHuTruoc,0)
                //        + ISNULL(t2.XuatHuHomNay,0) AS TongXuatHu

                //    FROM NhapTheoQuyCach n

                //    LEFT JOIN XuatTruoc t1
                //        ON t1.MaLo = n.MaLo

                //    LEFT JOIN XuatHomNay t2
                //        ON t2.MaLo = n.MaLo
                //),

                //------------------------------------------------
                //-- LŨY KẾ FIFO
                //------------------------------------------------
                //TinhLuyKe AS
                //(
                //    SELECT
                //        t.*,

                //        SUM(
                //            CASE WHEN t.ThuTu >= 1
                //                THEN t.KhoiLuongNhap
                //                ELSE 0
                //            END
                //        ) OVER
                //        (
                //            PARTITION BY t.MaLo
                //            ORDER BY t.ThuTu
                //        ) AS LuyKeThuong,

                //        SUM(
                //            CASE WHEN t.ThuTu >= 1
                //                THEN t.KhoiLuongNhap
                //                ELSE 0
                //            END
                //        ) OVER
                //        (
                //            PARTITION BY t.MaLo
                //        ) AS TongNhapThuong

                //    FROM TongHop t
                //)

                //------------------------------------------------
                //-- FINAL
                //------------------------------------------------
                //SELECT

                //    TenQuyCach,
                //    MaLo,
                //    KhoiLuongNhap,

                //------------------------------------------------
                //-- PHẦN XUẤT THƯỜNG VƯỢT
                //------------------------------------------------
                //DuXuatThuong =
                //CASE
                //    WHEN TongXuatThuong > TongNhapThuong
                //    THEN TongXuatThuong - TongNhapThuong
                //    ELSE 0
                //END,

                //------------------------------------------------
                //-- TỔNG LƯỢNG XUẤT
                //------------------------------------------------
                //TongLuongXuat =
                //CAST
                //(
                //    CASE

                //        ------------------------------------------------
                //        -- THU TU 0
                //        ------------------------------------------------
                //        WHEN ThuTu = 0
                //        THEN

                //            CASE

                //                WHEN
                //                    (TongXuatHu
                //                     + CASE
                //                        WHEN TongXuatThuong > TongNhapThuong
                //                        THEN TongXuatThuong - TongNhapThuong
                //                        ELSE 0
                //                      END) <= 0
                //                THEN 0

                //                WHEN
                //                    (TongXuatHu
                //                     + CASE
                //                        WHEN TongXuatThuong > TongNhapThuong
                //                        THEN TongXuatThuong - TongNhapThuong
                //                        ELSE 0
                //                      END) >= KhoiLuongNhap
                //                THEN KhoiLuongNhap

                //                ELSE
                //                    (TongXuatHu
                //                     + CASE
                //                        WHEN TongXuatThuong > TongNhapThuong
                //                        THEN TongXuatThuong - TongNhapThuong
                //                        ELSE 0
                //                      END)

                //            END

                //        ------------------------------------------------
                //        -- THU TU 1..N FIFO
                //        ------------------------------------------------
                //        ELSE

                //            CASE

                //                WHEN TongXuatThuong <= 0
                //                THEN 0

                //                WHEN TongXuatThuong >= LuyKeThuong
                //                THEN KhoiLuongNhap

                //                ELSE
                //                    TongXuatThuong
                //                    - (LuyKeThuong - KhoiLuongNhap)

                //            END

                //    END

                //AS DECIMAL(18,3)
                //),

                //------------------------------------------------
                //-- TỒN CÒN LẠI
                //------------------------------------------------
                //TonConLai =
                //CAST
                //(
                //    KhoiLuongNhap -
                //    (
                //        CASE

                //            WHEN ThuTu = 0
                //            THEN

                //                CASE

                //                    WHEN
                //                        (TongXuatHu
                //                         + CASE
                //                            WHEN TongXuatThuong > TongNhapThuong
                //                            THEN TongXuatThuong - TongNhapThuong
                //                            ELSE 0
                //                          END) <= 0
                //                    THEN 0

                //                    WHEN
                //                        (TongXuatHu
                //                         + CASE
                //                            WHEN TongXuatThuong > TongNhapThuong
                //                            THEN TongXuatThuong - TongNhapThuong
                //                            ELSE 0
                //                          END) >= KhoiLuongNhap
                //                    THEN KhoiLuongNhap

                //                    ELSE
                //                        (TongXuatHu
                //                         + CASE
                //                            WHEN TongXuatThuong > TongNhapThuong
                //                            THEN TongXuatThuong - TongNhapThuong
                //                            ELSE 0
                //                          END)

                //                END

                //            ELSE

                //                CASE

                //                    WHEN TongXuatThuong <= 0
                //                    THEN 0

                //                    WHEN TongXuatThuong >= LuyKeThuong
                //                    THEN KhoiLuongNhap

                //                    ELSE
                //                        TongXuatThuong
                //                        - (LuyKeThuong - KhoiLuongNhap)

                //                END

                //        END
                //    )

                //AS DECIMAL(18,3)
                //)

                //FROM TinhLuyKe
                //ORDER BY MaLo, ThuTu
                //";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = ngay.Date}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> GetKhoiLuongXuatLoTheoXuonget<T>(string maLo)
        {
            var MaLo = string.IsNullOrWhiteSpace(maLo) || maLo == "'" ? null : maLo;
            try
            {
                var query = @"
WITH NhapTheoQuyCach AS (
    SELECT
        pnl.MaLo,
        pn.NgayGio,
        pn.SoPhieuCanNhap,
        pn.MaSanPham,

        ISNULL(bl.MaQuyCach, ctn.MaQuyCach) AS MaQuyCach,

        qc.Ten AS TenQuyCach,

        ctn.TyLe,

        qc.[Index] AS ThuTu,

        CAST(
    CASE

        -- Có bao lụa
        WHEN bl.TrongLuongBaoLua IS NOT NULL
            THEN bl.TrongLuongBaoLua

        -- Có phân bổ tỷ lệ
        WHEN ctn.MaQuyCach IS NOT NULL
            THEN pn.TrongLuongHang * ISNULL(ctn.TyLe,0) / 100.0

        -- Không có phân bổ gì hết
        ELSE pn.TrongLuongHang

         END
       AS DECIMAL(18,1)
   ) AS KhoiLuongNhap

    FROM HQ_PhieuCanNhapNguyenLieu pn

    LEFT JOIN (
        SELECT DISTINCT
            SoPhieuCanNhap,
            MaQuyCach,
            TyLe
        FROM HQ_ChiTietPhanBoTyLeNguyenLieuNhap
    ) ctn
        ON ctn.SoPhieuCanNhap = pn.SoPhieuCanNhap

    LEFT JOIN (
        SELECT DISTINCT
            SoPhieuCanNhap,
            MaQuyCach,
            TrongLuongBaoLua
        FROM HQ_ChiTietPhanBoBaoLuaNguyenLieuNhap
    ) bl
        ON bl.SoPhieuCanNhap = pn.SoPhieuCanNhap
        AND (
            bl.MaQuyCach = ctn.MaQuyCach
            OR ctn.MaQuyCach IS NULL
        )

    JOIN HQ_PhieuCanNguyenLieu pnl
        ON pnl.Id = pn.IdPhieuCanNguyenLieu

    LEFT JOIN (
        SELECT DISTINCT
            Id,
            Ten,
            [Index]
        FROM HQ_QuyCachNguyenLieu
    ) qc
        ON qc.Id = ISNULL(bl.MaQuyCach, ctn.MaQuyCach)
),

------------------------------------------------
-- TỔNG XUẤT THƯỜNG
------------------------------------------------
TongXuatThuongTheoLo AS (
    SELECT
        pnl.MaLo,
        SUM(px.TrongLuongHang) AS TongXuatThuong
    FROM HQ_PhieuCanXuatNguyenLieu px
    JOIN HQ_PhieuCanNguyenLieu pnl
        ON pnl.Id = px.IdPhieuCanNguyenLieu
    WHERE
        ISNULL(px.IsCanHu,0) = 0
        AND ISNULL(px.IsHuy,0) = 0
    GROUP BY pnl.MaLo
),

------------------------------------------------
-- TỔNG XUẤT HƯ
------------------------------------------------
TongXuatHuTheoLo AS (
    SELECT
        pnl.MaLo,
        SUM(px.TrongLuongHang) AS TongXuatHu
    FROM HQ_PhieuCanXuatNguyenLieu px
    JOIN HQ_PhieuCanNguyenLieu pnl
        ON pnl.Id = px.IdPhieuCanNguyenLieu
    WHERE
        px.IsCanHu = 1
        AND ISNULL(px.IsHuy,0) = 0
    GROUP BY pnl.MaLo
),

------------------------------------------------
-- PHÂN BỔ
------------------------------------------------
PhanBoXuat AS (
    SELECT
        n.*,
        ISNULL(tx.TongXuatThuong,0) AS TongXuatThuong,
        ISNULL(th.TongXuatHu,0) AS TongXuatHu,
        -- TongNhapLuyKeThuong: lũy kế Khối lượng nhập của các dòng ThuTu >= 1 trong cùng MaLo.
        SUM(
            CASE
                WHEN n.ThuTu >= 1
                THEN n.KhoiLuongNhap
                ELSE 0
            END
        ) OVER (
            PARTITION BY n.MaLo
            ORDER BY n.ThuTu
        ) AS TongNhapLuyKeThuong,

        SUM(
            CASE
                WHEN n.ThuTu >= 1
                THEN n.KhoiLuongNhap
                ELSE 0
            END
        ) OVER (
            PARTITION BY n.MaLo
        ) AS TongNhapThuong

    FROM NhapTheoQuyCach n
    LEFT JOIN TongXuatThuongTheoLo tx
        ON tx.MaLo = n.MaLo
    LEFT JOIN TongXuatHuTheoLo th
        ON th.MaLo = n.MaLo
)

SELECT
    p.NgayGio,
    p.MaLo,
    p.SoPhieuCanNhap,
    p.MaSanPham,
    sp.Ten AS TenSanPham,
    p.MaQuyCach,
    p.TenQuyCach,
    p.ThuTu AS [Index],
    -- TongXuatThuong/TongXuatHu: tổng trọng lượng đã xuất của lô, tách theo xuất thường và xuất hư.
    -- TongNhapLuyKeThuong/TongNhapThuong: tổng nhập thường theo thứ tự FIFO để tính phần phân bổ còn lại.
    p.TongXuatThuong,
    p.TongXuatHu,
    p.TongNhapLuyKeThuong,
    p.TongNhapThuong,
    p.KhoiLuongNhap,
	    CASE 
    WHEN p.ThuTu = 0 THEN ISNULL(p.TongXuatHu,0)
    ELSE 0
    END AS TongHuKho,

------------------------------------------------
-- KHỐI LƯỢNG XUẤT
------------------------------------------------
-- ThuTu = 0: Khối lượng xuất = XuatHư + phần normal vượt tổng nhập thường, chặn trong [0, Khối lượng nhập].
-- ThuTu >= 1: Khối lượng xuất = TongXuatThuong - (TongNhapLuyKeThuong - Khối lượng nhập), chặn trong [0, Khối lượng nhập].
CAST(
CASE
    ------------------------------------------------
    -- INDEX 0
    ------------------------------------------------
    WHEN p.ThuTu = 0 THEN

        CASE

            WHEN
                (p.TongXuatHu +
                 CASE
                    WHEN p.TongXuatThuong > p.TongNhapThuong
                    THEN p.TongXuatThuong - p.TongNhapThuong
                    ELSE 0
                 END
                ) <= 0
            THEN 0

            WHEN
                (p.TongXuatHu +
                 CASE
                    WHEN p.TongXuatThuong > p.TongNhapThuong
                    THEN p.TongXuatThuong - p.TongNhapThuong
                    ELSE 0
                 END
                ) >= p.KhoiLuongNhap
            THEN p.KhoiLuongNhap

            ELSE
                (p.TongXuatHu +
                 CASE
                    WHEN p.TongXuatThuong > p.TongNhapThuong
                    THEN p.TongXuatThuong - p.TongNhapThuong
                    ELSE 0
                 END
                )

        END

    ------------------------------------------------
    -- FIFO INDEX >=1
    ------------------------------------------------
    ELSE

        CASE

            WHEN p.TongXuatThuong
                 <= (p.TongNhapLuyKeThuong - p.KhoiLuongNhap)
            THEN 0

            WHEN p.TongXuatThuong
                 >= p.TongNhapLuyKeThuong
            THEN p.KhoiLuongNhap

            ELSE
                p.TongXuatThuong -
                (p.TongNhapLuyKeThuong - p.KhoiLuongNhap)

        END

END
AS DECIMAL(18,1)) AS KhoiLuongXuat,

------------------------------------------------
-- HAO HỤT
------------------------------------------------
-- Hao hụt = Khối lượng nhập - Khối lượng xuất.
CAST(
    p.KhoiLuongNhap -
    (
        CASE

            WHEN p.ThuTu = 0 THEN
                CASE
                    WHEN
                        (p.TongXuatHu +
                         CASE
                            WHEN p.TongXuatThuong > p.TongNhapThuong
                            THEN p.TongXuatThuong - p.TongNhapThuong
                            ELSE 0
                         END
                        ) >= p.KhoiLuongNhap
                    THEN p.KhoiLuongNhap
                    ELSE
                        (p.TongXuatHu +
                         CASE
                            WHEN p.TongXuatThuong > p.TongNhapThuong
                            THEN p.TongXuatThuong - p.TongNhapThuong
                            ELSE 0
                         END
                        )
                END

            ELSE
                CASE
                    WHEN p.TongXuatThuong <= 0 THEN 0
                    WHEN p.TongXuatThuong >= p.TongNhapLuyKeThuong
                    THEN p.KhoiLuongNhap
                    ELSE
                        p.TongXuatThuong -
                        (p.TongNhapLuyKeThuong - p.KhoiLuongNhap)
                END

        END
    )
AS DECIMAL(18,1)) AS KhoiLuongHaoHut

FROM PhanBoXuat p
LEFT JOIN HQ_SanPhamNguyenLieu sp
    ON sp.Id = p.MaSanPham

WHERE p.MaLo = @MaLo

ORDER BY
    p.MaLo,
    p.ThuTu ASC;
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { MaLo}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        public List<T> GetListLo<T>()
        {
            try
            {
            var query = @"
                select DISTINCT 
                pnl.MaLo
                from HQ_PhieuCanXuatNguyenLieu p
                left join HQ_PhieuCanNguyenLieu pnl on pnl.Id = p.IdPhieuCanNguyenLieu
                order by pnl.MaLo desc
                ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query).ToList();
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
