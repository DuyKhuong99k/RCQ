using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;


namespace Dao.Repos.HQ
{
    public partial class HQ_PhieuCan
    {
        private readonly string connectionString;
        private string tableName = @"HQ_PhieuCan";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_PhieuCan]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[HQ_PhieuCan]
           ([Id]
           ,[STT]
           ,[Ngay]
           ,[NgayGio]
           ,[MayCan]
           ,[MaLo]
           ,[MaSize]
           ,[MaThanhPham]
           ,[MaLoaiNguyenLieu]
           ,[MaNhanVien]
           ,[MaNhanVienPhucVu]
           ,[MaNhanVienBanKiem]
           ,[TrongLuong]
           ,[TrongLuongTare]
           ,[ChiSanLuong]
           ,[Status]
           ,[TheId]
           ,[TheIdNhanVien]
           ,[GhiChu]
           ,[MaXuong])
     VALUES
           (@Id
           ,@STT
           ,@Ngay
           ,@NgayGio
           ,@MayCan
           ,@MaLo
           ,@MaSize
           ,@MaThanhPham
           ,@MaLoaiNguyenLieu
           ,@MaNhanVien
           ,@MaNhanVienPhucVu
           ,@MaNhanVienBanKiem
           ,@TrongLuong
           ,@TrongLuongTare
           ,@ChiSanLuong
           ,@Status
           ,@TheId
           ,@TheIdNhanVien
           ,@GhiChu
           ,@MaXuong)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[HQ_PhieuCan]
   SET
      [STT] = @STT
      ,[Ngay] = @Ngay
      ,[NgayGio] = @NgayGio
      ,[MayCan] = @MayCan
      ,[MaLo] = @MaLo
      ,[MaSize] = @MaSize
      ,[MaThanhPham] = @MaThanhPham
      ,[MaLoaiNguyenLieu] = @MaLoaiNguyenLieu
      ,[MaNhanVien] = @MaNhanVien
      ,[MaNhanVienPhucVu] = @MaNhanVienPhucVu
      ,[MaNhanVienBanKiem] = @MaNhanVienBanKiem
      ,[TrongLuong] = @TrongLuong
      ,[TrongLuongTare] = @TrongLuongTare
      ,[ChiSanLuong] = @ChiSanLuong
      ,[Status] = @Status
      ,[TheId] = @TheId
      ,[TheIdNhanVien] = @TheIdNhanVien
      ,[GhiChu] = @GhiChu
      ,[MaXuong] = @MaXuong
 WHERE [Id] =@Id
";
        private readonly string qrGetsLastByNumAndMayCan = @"WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MayCan ORDER BY NgayGio DESC) AS RowNum
    FROM HQ_PhieuCan where Ngay =@ngay
)

SELECT *
FROM RankedPhieu
WHERE RowNum <= @num";
        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var query = qrGetsLastByNumAndMayCan;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, num })
                    .Result
                    .ToList();
                return items;
            }
        }
        public HQ_PhieuCan(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;
        }
        public T? Get<T>(string id)
        {
            try
            {
                var query = $"SELECT * FROM {tableName} WHERE Id = @id";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.QuerySingleOrDefault<T>(query, new { id });
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate,string xuongId)
        {
            try
            {
//                var query = @"select
//p.Id,
//p.STT,
//p.Ngay,
//p.NgayGio,
//p.MayCan,
//p.MaLo,
//p.MaSize,
//s.Ten as SizeName,
//p.MaThanhPham,
//tp.Ten as ThanhPhamName,
//p.MaLoaiNguyenLieu,
//lnl.Ten as LoaiNguyenLieuName,
//p.MaNhanVien,
//nv.MaHoSo,
//nv.Name as NhanVienName,
//p.MaNhanVienPhucVu,
//nvpv.MaHoSo as MaHoSoPV,
//nvpv.Name as NhanVienPVName,
//p.MaNhanVienBanKiem,
//nvbk.MaHoSo as MaHoSoBK,
//nvbk.Name as NhanVienBKName,
//p.TrongLuong,
//p.TrongLuongTare,
//p.ChiSanLuong,
//p.Status,
//p.TheId,
//p.TheIdNhanVien
//from HQ_PhieuCan p,
//	HQ_Size s,
//	HQ_ThanhPham tp,
//	HQ_LoaiNguyenLieu lnl,
//	NhanVienDaiThanh nv,
//	NhanVienDaiThanh nvpv,
//	NhanVienDaiThanh nvbk

//where p.Ngay >= @fromDate and p.Ngay <= @toDate  and p.MaXuong = @xuongId
//	and p.MaSize = s.Id
//	and p.MaThanhPham = tp.Id
//	and p.MaLoaiNguyenLieu = lnl.Id
//	and p.MaNhanVien = nv.MaNhanVien
//	and p.MaNhanVienPhucVu = nvpv.MaNhanVien
//	and p.MaNhanVienBanKiem = nvbk.MaNhanVien
//	order by
//	p.STT desc
	
//";
//Bỏ thông tin NVPV VÀ NVBK
var query = @"select
p.Id,
p.STT,
p.Ngay,
p.NgayGio,
p.MayCan,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLoaiNguyenLieu,
lnl.Ten as LoaiNguyenLieuName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
--p.MaNhanVienPhucVu,
--nvpv.MaHoSo as MaHoSoPV,
--nvpv.Name as NhanVienPVName,
--p.MaNhanVienBanKiem,
--nvbk.MaHoSo as MaHoSoBK,
--nvbk.Name as NhanVienBKName,
p.TrongLuong,
p.TrongLuongTare,
p.ChiSanLuong,
p.Status,
p.TheId,
p.TheIdNhanVien
from HQ_PhieuCan p,
	HQ_Size s,
	HQ_ThanhPham tp,
	HQ_LoaiNguyenLieu lnl,
	NhanVienDaiThanh nv
	--NhanVienDaiThanh nvpv,
	--NhanVienDaiThanh nvbk

where p.NgayGio >= @fromDate and p.NgayGio <= @toDate  and p.MaXuong = @xuongId
	and p.MaSize = s.Id
	and p.MaThanhPham = tp.Id
	and p.MaLoaiNguyenLieu = lnl.Id
	and p.MaNhanVien = nv.MaNhanVien
	--and p.MaNhanVienPhucVu = nvpv.MaNhanVien
	--and p.MaNhanVienBanKiem = nvbk.MaNhanVien
	order by
	p.STT desc
	
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate,  toDate ,xuongId}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetChiTietXLPCs<T>( DateTime dateTime,string xuongId)
        {
            try
            {
                var query = @"Select
    p.*,
    nvpv.Name as NhanVienPVName,
    nvbk.Name as NhanVienBKName,
    nvbk.MaHoSo as MaHoSoBK,
    nvpv.MaHoSo as MaHoSoPV
from
    (
        select
            p.Id,
            p.STT,
            p.Ngay,
            p.NgayGio,
            p.MayCan,
            p.MaLo,
            p.MaSize,
            s.Ten as SizeName,
            p.MaThanhPham,
            tp.Ten as ThanhPhamName,
            p.MaLoaiNguyenLieu,
            lnl.Ten as LoaiNguyenLieuName,
            p.MaNhanVien,
            nv.MaHoSo,
            nv.Name as NhanVienName,
            p.MaNhanVienPhucVu,
            -- nvpv.MaHoSo as MaHoSoPV,
            -- nvpv.Name as NhanVienPVName,
            p.MaNhanVienBanKiem,
            -- nvbk.MaHoSo as MaHoSoBK,
            -- nvbk.Name as NhanVienBKName,
            p.TrongLuong,
            p.TrongLuongTare,
            p.ChiSanLuong,
            p.Status,
            p.TheId,
            p.TheIdNhanVien,
            p.GhiChu
        from
            HQ_PhieuCan p,
            HQ_Size s,
            HQ_ThanhPham tp,
            HQ_LoaiNguyenLieu lnl,
            NhanVienDaiThanh nv
        where
            p.Ngay = @dateTime and p.MaXuong = @xuongId
            and p.MaSize = s.Id
            and p.MaThanhPham = tp.Id
            and p.MaLoaiNguyenLieu = lnl.Id
            and p.MaNhanVien = nv.MaNhanVien
    ) p
    LEFT JOIN NhanVienDaiThanh nvpv ON p.MaNhanVienPhucVu = nvpv.MaNhanVien
    LEFT JOIN NhanVienDaiThanh nvbk ON p.MaNhanVienBanKiem = nvbk.MaNhanVien
order by
    p.STT desc
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { dateTime = dateTime.Date,xuongId}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        
        public List<T> GetTongHopNhanViens<T>(DateTime fromDate, DateTime toDate,string xuongId)
        {
            try
            {
                var query = @"select
p.Ngay,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLoaiNguyenLieu,
lnl.Ten as LoaiNguyenLieuName,
COUNT(*) as SoRo,
Sum(p.TrongLuong) as TrongLuong
from 
HQ_PhieuCan p,
NhanVienDaiThanh nv,
HQ_Size s,
HQ_ThanhPham tp,
HQ_LoaiNguyenLieu lnl
where
p.NgayGio BETWEEN @fromDate AND @toDate and p.MaXuong = @xuongId and p.TrongLuong > 0
and p.MaNhanVien = nv.MaNhanVien
and p.MaSize = s.Id
and p.MaThanhPham = tp.Id
and p.MaLoaiNguyenLieu =  lnl.Id
group by
p.Ngay,
p.MaNhanVien,
nv.MaHoSo,
nv.Name,
p.MaLo,
p.MaSize,
s.Ten,
p.MaThanhPham,
tp.Ten,
p.MaLoaiNguyenLieu,
lnl.Ten
	
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


        public List<T> GetTongHopNhanVienTheoCas<T>(DateTime fromDate, DateTime toDate,string xuongId)
        {
            try
            {
//                var query = @"SELECT 
//    p.Ngay,
//    p.MaNhanVien,
//    nv.MaHoSo,
//    nv.Name AS NhanVienName,
//    c.Id AS CaId,
//    c.Ten AS CaName,
//    c.GioBatDau AS GioBatDauCa,
//    c.GioKetThuc AS GioKetThucCa,
//    p.MaThanhPham,
//    tp.Ten AS ThanhPhamName,
//    p.MaLoaiNguyenLieu,
//    lnl.Ten AS LoaiNguyenLieuName,
//    COUNT(*) AS SoRo,
//    SUM(p.TrongLuong) AS TrongLuong,
//    MIN(p.NgayGio) AS GioBatDauCan, -- Giờ bắt đầu cân
//    MAX(p.NgayGio) AS GioKetThucCan, -- Giờ kết thúc cân
//    DATEDIFF(MINUTE, MIN(p.NgayGio), MAX(p.NgayGio)) / 60.0 AS TongGioLamViec, -- Tổng giờ làm việc

//    -- Số rổ tăng ca: Chỉ đếm số lần cân nằm ngoài khung giờ của ca
//SUM(CASE 
//    WHEN CAST(p.NgayGio AS TIME) < CAST(c.GioBatDau AS TIME) 
//         OR CAST(p.NgayGio AS TIME) > CAST(c.GioKetThuc AS TIME) THEN 1
//    ELSE 0 
//END) AS SoRoTangCa,

//-- Giờ bắt đầu tăng ca
//CASE 
//    WHEN SUM(CASE WHEN CAST(p.NgayGio AS TIME) < CAST(c.GioBatDau AS TIME) OR CAST(p.NgayGio AS TIME) > CAST(c.GioKetThuc AS TIME) THEN 1 ELSE 0 END) = 0 THEN NULL
//    ELSE MIN(CASE 
//        WHEN CAST(p.NgayGio AS TIME) < CAST(c.GioBatDau AS TIME) 
//             OR CAST(p.NgayGio AS TIME) > CAST(c.GioKetThuc AS TIME) THEN p.NgayGio
//        ELSE NULL 
//    END)
//END AS GioBatDauTangCa,

//-- Giờ kết thúc tăng ca
//CASE 
//    WHEN SUM(CASE WHEN CAST(p.NgayGio AS TIME) < CAST(c.GioBatDau AS TIME) OR CAST(p.NgayGio AS TIME) > CAST(c.GioKetThuc AS TIME) THEN 1 ELSE 0 END) = 0 THEN NULL
//    ELSE MAX(CASE 
//        WHEN CAST(p.NgayGio AS TIME) < CAST(c.GioBatDau AS TIME) 
//             OR CAST(p.NgayGio AS TIME) > CAST(c.GioKetThuc AS TIME) THEN p.NgayGio
//        ELSE NULL 
//    END)
//END AS GioKetThucTangCa,

//-- Tổng số giờ tăng ca
//CASE 
//    WHEN SUM(CASE WHEN CAST(p.NgayGio AS TIME) < CAST(c.GioBatDau AS TIME) OR CAST(p.NgayGio AS TIME) > CAST(c.GioKetThuc AS TIME) THEN 1 ELSE 0 END) = 0 THEN 0
//    ELSE DATEDIFF(MINUTE,
//        MIN(CASE 
//            WHEN CAST(p.NgayGio AS TIME) < CAST(c.GioBatDau AS TIME) 
//                 OR CAST(p.NgayGio AS TIME) > CAST(c.GioKetThuc AS TIME) THEN p.NgayGio
//            ELSE NULL 
//        END),
//        MAX(CASE 
//            WHEN CAST(p.NgayGio AS TIME) < CAST(c.GioBatDau AS TIME) 
//                 OR CAST(p.NgayGio AS TIME) > CAST(c.GioKetThuc AS TIME) THEN p.NgayGio
//            ELSE NULL 
//        END)
//    ) / 60.0
//END AS TongGioTangCa,

//-- Trọng lượng tăng ca: Chỉ tính trọng lượng các lần cân ngoài khung giờ của ca
//CASE 
//    WHEN SUM(CASE WHEN CAST(p.NgayGio AS TIME) < CAST(c.GioBatDau AS TIME) OR CAST(p.NgayGio AS TIME) > CAST(c.GioKetThuc AS TIME) THEN 1 ELSE 0 END) = 0 THEN 0
//    ELSE SUM(CASE 
//        WHEN CAST(p.NgayGio AS TIME) < CAST(c.GioBatDau AS TIME) 
//             OR CAST(p.NgayGio AS TIME) > CAST(c.GioKetThuc AS TIME) THEN p.TrongLuong
//        ELSE 0 
//    END)
//END AS TrongLuongTangCa


//FROM 
//    HQ_PhieuCan p
//    LEFT JOIN NhanVienDaiThanh nv ON p.MaNhanVien = nv.MaNhanVien
//    LEFT JOIN HQ_NhanVienTheoCa nvc ON nv.MaNhanVien = nvc.NhanVienId
//    LEFT JOIN HQ_Ca c ON nvc.CaId = c.Id
//    LEFT JOIN HQ_ThanhPham tp ON p.MaThanhPham = tp.Id
//    LEFT JOIN HQ_LoaiNguyenLieu lnl ON p.MaLoaiNguyenLieu = lnl.Id
//WHERE 
//    p.Ngay BETWEEN @fromDate AND @toDate
//    AND p.MaXuong = @xuongId and p.TrongLuong > 0
//GROUP BY
//    p.Ngay,
//    p.MaNhanVien,
//    nv.MaHoSo,
//    nv.Name,
//    c.Id, 
//    c.Ten,
//    c.GioBatDau,
//    c.GioKetThuc,
//    p.MaThanhPham,
//    tp.Ten,
//    p.MaLoaiNguyenLieu,
//    lnl.Ten
	
//";
var query = @";WITH AdjustedCa AS (
    SELECT 
        c.Id AS CaId,
        c.Ten AS CaName,
        c.GioBatDau,
        -- Nếu giờ kết thúc < giờ bắt đầu, cộng thêm 1 ngày vào giờ kết thúc
        CASE 
            WHEN CAST(c.GioKetThuc AS TIME) < CAST(c.GioBatDau AS TIME) 
            THEN DATEADD(DAY, 1, c.GioKetThuc)
            ELSE c.GioKetThuc
        END AS GioKetThucCa
    FROM HQ_Ca c
)
SELECT 
    p.Ngay,
    p.MaNhanVien,
    nv.MaHoSo,
    nv.Name AS NhanVienName,
    ac.CaId,
    ac.CaName,
    ac.GioBatDau AS GioBatDauCa,
    ac.GioKetThucCa,
    p.MaThanhPham,
    tp.Ten AS ThanhPhamName,
    p.MaLoaiNguyenLieu,
    lnl.Ten AS LoaiNguyenLieuName,
    COUNT(*) AS SoRo,
    SUM(p.TrongLuong) AS TrongLuong,
    MIN(p.NgayGio) AS GioBatDauCan,
    MAX(p.NgayGio) AS GioKetThucCan,
    DATEDIFF(MINUTE, MIN(p.NgayGio), MAX(p.NgayGio)) / 60.0 AS TongGioLamViec,

    -- Số rổ tăng ca
    SUM(CASE 
        WHEN CAST(p.NgayGio AS TIME) < CAST(ac.GioBatDau AS TIME) 
             OR CAST(p.NgayGio AS TIME) > CAST(ac.GioKetThucCa AS TIME) THEN 1
        ELSE 0 
    END) AS SoRoTangCa,

    -- Giờ bắt đầu tăng ca
    CASE 
        WHEN SUM(CASE WHEN CAST(p.NgayGio AS TIME) < CAST(ac.GioBatDau AS TIME) OR CAST(p.NgayGio AS TIME) > CAST(ac.GioKetThucCa AS TIME) THEN 1 ELSE 0 END) = 0 THEN NULL
        ELSE MIN(CASE 
            WHEN CAST(p.NgayGio AS TIME) < CAST(ac.GioBatDau AS TIME) 
                 OR CAST(p.NgayGio AS TIME) > CAST(ac.GioKetThucCa AS TIME) THEN p.NgayGio
            ELSE NULL 
        END)
    END AS GioBatDauTangCa,

    -- Giờ kết thúc tăng ca
    CASE 
        WHEN SUM(CASE WHEN CAST(p.NgayGio AS TIME) < CAST(ac.GioBatDau AS TIME) OR CAST(p.NgayGio AS TIME) > CAST(ac.GioKetThucCa AS TIME) THEN 1 ELSE 0 END) = 0 THEN NULL
        ELSE MAX(CASE 
            WHEN CAST(p.NgayGio AS TIME) < CAST(ac.GioBatDau AS TIME) 
                 OR CAST(p.NgayGio AS TIME) > CAST(ac.GioKetThucCa AS TIME) THEN p.NgayGio
            ELSE NULL 
        END)
    END AS GioKetThucTangCa,

    -- Tổng số giờ tăng ca
    CASE 
        WHEN SUM(CASE WHEN CAST(p.NgayGio AS TIME) < CAST(ac.GioBatDau AS TIME) OR CAST(p.NgayGio AS TIME) > CAST(ac.GioKetThucCa AS TIME) THEN 1 ELSE 0 END) = 0 THEN 0
        ELSE DATEDIFF(MINUTE,
            MIN(CASE 
                WHEN CAST(p.NgayGio AS TIME) < CAST(ac.GioBatDau AS TIME) 
                     OR CAST(p.NgayGio AS TIME) > CAST(ac.GioKetThucCa AS TIME) THEN p.NgayGio
                ELSE NULL 
            END),
            MAX(CASE 
                WHEN CAST(p.NgayGio AS TIME) < CAST(ac.GioBatDau AS TIME) 
                     OR CAST(p.NgayGio AS TIME) > CAST(ac.GioKetThucCa AS TIME) THEN p.NgayGio
                ELSE NULL 
            END)
        ) / 60.0
    END AS TongGioTangCa,

    -- Trọng lượng tăng ca
    CASE 
        WHEN SUM(CASE WHEN CAST(p.NgayGio AS TIME) < CAST(ac.GioBatDau AS TIME) OR CAST(p.NgayGio AS TIME) > CAST(ac.GioKetThucCa AS TIME) THEN 1 ELSE 0 END) = 0 THEN 0
        ELSE SUM(CASE 
            WHEN CAST(p.NgayGio AS TIME) < CAST(ac.GioBatDau AS TIME) 
                 OR CAST(p.NgayGio AS TIME) > CAST(ac.GioKetThucCa AS TIME) THEN p.TrongLuong
            ELSE 0 
        END)
    END AS TrongLuongTangCa

FROM 
    HQ_PhieuCan p
    LEFT JOIN NhanVienDaiThanh nv ON p.MaNhanVien = nv.MaNhanVien
    LEFT JOIN HQ_NhanVienTheoCa nvc ON nv.MaNhanVien = nvc.NhanVienId
    LEFT JOIN AdjustedCa ac ON nvc.CaId = ac.CaId
    LEFT JOIN HQ_ThanhPham tp ON p.MaThanhPham = tp.Id
    LEFT JOIN HQ_LoaiNguyenLieu lnl ON p.MaLoaiNguyenLieu = lnl.Id
WHERE 
    p.NgayGio BETWEEN DATEADD(DAY, -1, @fromDate) AND DATEADD(DAY, 1, @toDate)
    AND p.MaXuong = @xuongId AND p.TrongLuong > 0
GROUP BY
    p.Ngay,
    p.MaNhanVien,
    nv.MaHoSo,
    nv.Name,
    ac.CaId, 
    ac.CaName,
    ac.GioBatDau,
    ac.GioKetThucCa,
    p.MaThanhPham,
    tp.Ten,
    p.MaLoaiNguyenLieu,
    lnl.Ten;
	
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate, toDate,xuongId}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> GetTongHopSanLuong<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = $@"Declare @fromDate datetime = '{fromDate:yyyy-MM-dd HH:mm:ss}',
    @toDate datetime = '{toDate:yyyy-MM-dd HH:mm:ss}' IF OBJECT_ID('tempdb..#TempPhieuCan') IS NOT NULL DROP TABLE #TempPhieuCan;
    IF OBJECT_ID('tempdb..#TempNhanVienNhom') IS NOT NULL DROP TABLE #TempNhanVienNhom;
    IF OBJECT_ID('tempdb..#TempNhanVienNhom2') IS NOT NULL DROP TABLE #TempNhanVienNhom2;
    IF OBJECT_ID('tempdb..#TempNhanVienNhom3') IS NOT NULL DROP TABLE #TempNhanVienNhom3;
    if OBJECT_ID('tempdb..#TempMaNhomNhanVienDaiDien') IS NOT NULL DROP TABLE #TempMaNhomNhanVienDaiDien;
    IF OBJECT_ID('tempdb..#TempThoiGianLamViec') IS NOT NULL DROP TABLE #TempThoiGianLamViec;
    IF OBJECT_ID('tempdb..#TempNhanVienCa') IS NOT NULL DROP TABLE #TempNhanVienCa;
    IF OBJECT_ID('tempdb..#LichSuThayDoiNhom') IS NOT NULL DROP TABLE #LichSuThayDoiNhom;
    IF OBJECT_ID('tempdb..#RankedDataNhom') IS NOT NULL DROP TABLE #RankedDataNhom;
    IF OBJECT_ID('tempdb..#TempThoiGianLamViecNhom') IS NOT NULL DROP TABLE #TempThoiGianLamViecNhom;
    IF OBJECT_ID('tempdb..#TempNhanVienTheoNhom') IS NOT NULL DROP TABLE #TempNhanVienTheoNhom;
    IF OBJECT_ID('tempdb..#MapSanPhamTinhLuong') IS NOT NULL DROP TABLE #MapSanPhamTinhLuong;
    WITH MapSanPhamTinhLuong AS (
        SELECT p.Id,
            p.MaSanPham,
            dg.Ten as SanPhamName,
            p.MaThanhPham,
            p.MaSize,
            p.MaLoaiNguyenLieu,
            p.NgayGio,
            ROW_NUMBER() OVER (
                PARTITION BY p.MaThanhPham,
                p.MaSanPham,
                p.MaSize,
                p.MaLoaiNguyenLieu
                ORDER BY p.NgayGio DESC
            ) AS RowNum
        FROM HQ_MapSanPhamTinhLuong p,
            DG_SanPhamTinhLuong dg
        WHERE p.NgayGio <= @toDate
            and p.MaSanPham = dg.Ma
    )
SELECT Id,
    MaSanPham,
    SanPhamName,
    MaThanhPham,
    MaSize,
    MaLoaiNguyenLieu,
    NgayGio
    into #MapSanPhamTinhLuong
FROM MapSanPhamTinhLuong
WHERE RowNum = 1 -- 1️⃣ Tạo bảng tạm chứa dữ liệu phiếu cân với phân loại ca làm việc
SELECT p.*,
    CASE
        WHEN DATEPART(HOUR, p.NgayGio) >= 5
        AND DATEPART(HOUR, p.NgayGio) < 17 THEN 'N'
        ELSE 'D'
    END AS CaLamViec,
    CONVERT(
        DATE,
        CASE
            WHEN DATEPART(HOUR, p.NgayGio) >= 5 THEN p.NgayGio -- Ca ngày giữ nguyên
            ELSE DATEADD(DAY, -1, p.NgayGio) -- Ca đêm tính vào ngày trước đó
        END
    ) AS NgayLamViec INTO #TempPhieuCan
FROM HQ_PhieuCan p
WHERE p.NgayGio >= @fromDate
    AND p.NgayGio <= @toDate and MaXuong ='{xuongId}'
order by p.NgayGio;
-- Select *
-- from #TempPhieuCan;
WITH LichSuThayDoi AS (
    SELECT NgayGioBatDau,
        LAG(NgayGioBatDau) OVER (
            ORDER BY NgayGioBatDau
        ) AS NgayGioTruoc
    FROM HQ_NhanVienTheoNhom
    WHERE NgayGioBatDau BETWEEN @fromDate AND @toDate
),
DanhSachChinh AS (
    SELECT NgayGioBatDau,
        DATEDIFF(MINUTE, NgayGioTruoc, NgayGioBatDau) AS ThoiGianTruocSau
    FROM LichSuThayDoi
)
SELECT @fromDate as NgayGioBatDau into #LichSuThayDoiNhom
UNION ALL
SELECT NgayGioBatDau
FROM DanhSachChinh
WHERE ThoiGianTruocSau IS NULL
    or ThoiGianTruocSau > 5 -- 2️⃣ Xác định ca làm việc đầu tiên của mỗi nhân viên trong ngày
SELECT p.MaNhanVien,
    p.NgayLamViec,
    MIN(p.NgayGio) AS GioBatDau,
    (
        SELECT TOP 1 CaLamViec
        FROM #TempPhieuCan pc 
        WHERE pc.MaNhanVien = p.MaNhanVien
            AND pc.NgayLamViec = p.NgayLamViec
        ORDER BY pc.NgayGio ASC
    ) AS CaDinhDanh INTO #TempNhanVienCa
FROM #TempPhieuCan p
GROUP BY p.MaNhanVien,
    p.NgayLamViec;
-- 3️⃣ Lấy nhóm nhân viên theo thời gian mới nhất
SELECT p.NgayLamViec,
    p.NgayGio,
    p.MaNhanVien,
    nv.Name as TenNhanVien,
    nv.IsNhom,
    p.MaLoaiNguyenLieu,
    p.MaSize,
    p.MaThanhPham,
    COALESCE(nvt.MaNhom, '000') as MA_DAI_DIEN,
    nc.CaDinhDanh AS CaLamViec,
    nvt.HeSo,
    p.TrongLuong INTO #TempNhanVienNhom
FROM #TempPhieuCan p
    LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
    JOIN #TempNhanVienCa nc ON p.MaNhanVien = nc.MaNhanVien AND p.NgayLamViec = nc.NgayLamViec
    OUTER APPLY (
        SELECT TOP 1 nvt.MaNhom,
            nvt.HeSo
        FROM HQ_NhanVienTheoNhom nvt
        WHERE nvt.MaNhanVien = p.MaNhanVien
            AND nvt.NgayGioBatDau <= p.NgayGio
        ORDER BY nvt.NgayGioBatDau DESC
    ) nvt;
Select nn.MaNhanVien,
    dbo.non_unicode_convert(upper(nn.TenNhanVien)) as TenNhanVien,
    dbo.non_unicode_convert(upper(nhom.Ten)) As TenNhom,
    nhom.Id as MaNhom INTO #TempMaNhomNhanVienDaiDien
from (
        Select DISTINCT MaNhanVien,
            TenNhanVien
        FROM #TempNhanVienNhom p where p.IsNhom =1 ) nn, HQ_Nhom nhom where dbo.non_unicode_convert(upper(nn.TenNhanVien))= dbo.non_unicode_convert(upper(nhom.Ten)) ;
        Select p.NgayLamViec,
            p.NgayGio,
            p.MaNhanVien,
            p.TenNhanVien,
            p.IsNhom,
            p.MaLoaiNguyenLieu,
            p.MaSize,
            p.MaThanhPham,
            COALESCE(nvt.MaNhom, p.MA_DAI_DIEN) as MA_DAI_DIEN,
            p.CaLamViec,
            COALESCE(p.HeSo, 1) as HeSo,
            p.TrongLuong into #TempNhanVienNhom2
        from #TempNhanVienNhom p 
            LEFT JOIN #TempMaNhomNhanVienDaiDien nvt on p.MaNhanVien = nvt.MaNhanVien;
            -- 4️⃣ Tính thời gian làm việc của nhân viên theo tổ hợp (Nhân viên/Nhóm, Nguyên Liệu,,Thành Phẩm, Size)
        Select l.NgayGioBatDau,
            nhommoinhat.MaNhom,
            nhommoinhat.MaNhanVien,
            nhommoinhat.HeSo into #TempNhanVienTheoNhom
        from #LichSuThayDoiNhom l
            OUTER APPLY (
                SELECT MaNhanVien,
                    MaNhom,
                    NgayGioBatDau,
                    HeSo
                FROM (
                        SELECT MaNhanVien,
                            MaNhom,
                            NgayGioBatDau,
                            HeSo,
                            ROW_NUMBER() OVER (
                                PARTITION BY MaNhanVien
                                ORDER BY NgayGioBatDau DESC
                            ) AS RowNum
                        FROM HQ_NhanVienTheoNhom
                        where NgayGioBatDau <= l.NgayGioBatDau
                    ) AS LatestGroups
                WHERE RowNum = 1
            ) nhommoinhat;
WITH TimeRanges AS (
    SELECT NgayGioBatDau AS StartTime,
        LEAD(NgayGioBatDau) OVER (
            ORDER BY NgayGioBatDau
        ) AS EndTime
    FROM #LichSuThayDoiNhom
)
Select t.StartTime,
    p.* into #TempNhanVienNhom3
from (
        Select *
        from #TempNhanVienNhom2 p where MA_DAI_DIEN!='000') p
            JOIN TimeRanges t ON p.NgayGio >= t.StartTime
            AND (
                p.NgayGio < t.EndTime
                OR t.EndTime IS NULL
            )
        ORDER BY p.MA_DAI_DIEN,
            p.NgayGio;
-- Select *
-- from #TempNhanVienNhom3 ; 
SELECT StartTime,
    NgayGio,
    NgayLamViec,
    MaLoaiNguyenLieu,
    MaSize,
    MaThanhPham,
    MA_DAI_DIEN,
    CaLamViec,
    TrongLuong,
    LAG(MA_DAI_DIEN) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_MaDaiDien,
    LAG(MaLoaiNguyenLieu) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_MaLoaiNguyenLieu,
    LAG(MaSize) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_MaSize,
    LAG(MaThanhPham) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_MaThanhPham,
    LAG(StartTime) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_StartTime into #RankedDataNhom
FROM #TempNhanVienNhom3 
ORDER BY NgayLamViec,
    MA_DAI_DIEN,
    NgayGio;
with GroupedData AS (
    SELECT *,
        SUM(
            CASE
                WHEN MA_DAI_DIEN != Prev_MaDaiDien
                or MaLoaiNguyenLieu != Prev_MaLoaiNguyenLieu
                OR MaSize != Prev_MaSize
                OR MaThanhPham != Prev_MaThanhPham
                OR StartTime != Prev_StartTime THEN 1
                ELSE 0
            END
        ) OVER (
            PARTITION BY NgayLamViec,
            MA_DAI_DIEN
            ORDER BY NgayGio
        ) AS GroupID
    FROM #RankedDataNhom
)
SELECT StartTime,
    MIN(NgayGio) AS GioBatDau,
    MAX(NgayGio) AS GioKetThuc,
    MaLoaiNguyenLieu,
    MaSize,
    MaThanhPham,
    MA_DAI_DIEN,
    CaLamViec,
    NgayLamViec,
    SUM(TrongLuong) AS TongTrongLuong into #TempThoiGianLamViecNhom
FROM GroupedData
GROUP BY NgayLamViec,
    StartTime,
    GroupID,
    MaLoaiNguyenLieu,
    MaSize,
    MaThanhPham,
    MA_DAI_DIEN,
    CaLamViec
ORDER BY NgayLamViec,
    StartTime,
    ma_dai_dien,
    GioBatDau;
WITH RankedData AS (
    SELECT NgayGio,
        NgayLamViec,
        MaNhanVien,
        TenNhanVien,
        IsNhom,
        MaLoaiNguyenLieu,
        MaSize,
        MaThanhPham,
        MA_DAI_DIEN,
        CaLamViec,
        HeSo,
        TrongLuong,
        LAG(MaNhanVien) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS Prev_MaNhanVien,
        LAG(MaLoaiNguyenLieu) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS Prev_MaLoaiNguyenLieu,
        LAG(MaSize) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS Prev_MaSize,
        LAG(MaThanhPham) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS Prev_MaThanhPham
    FROM #TempNhanVienNhom2 where MA_DAI_DIEN ='000'
),
GroupedData AS (
    SELECT *,
        SUM(
            CASE
                WHEN MaNhanVien != Prev_MaNhanVien
                OR MaLoaiNguyenLieu != Prev_MaLoaiNguyenLieu
                OR MaSize != Prev_MaSize
                OR MaThanhPham != Prev_MaThanhPham THEN 1
                ELSE 0
            END
        ) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS GroupID
    FROM RankedData
)
Select p.*,
    nv.Name as TenNhanVien,
    nl.Ten as TenLoaiNguyenLieu,
    s.Ten as TenSize,
    tp.Ten as TenThanhPham,
    map.MaSanPham,
    map.SanPhamName
from (
        SELECT @fromDate as StartTime,
            MIN(NgayGio) AS GioBatDau,
            MAX(NgayGio) AS GioKetThuc,
            MaNhanVien,
            MaLoaiNguyenLieu,
            MaSize,
            MaThanhPham,
            MA_DAI_DIEN,
            CaLamViec,
            NgayLamViec,
            cast(SUM(TrongLuong * HeSo) as decimal(18, 3)) AS TongTrongLuong
        FROM GroupedData
        GROUP BY NgayLamViec,
            GroupID,
            MaNhanVien,
            MaLoaiNguyenLieu,
            MaSize,
            MaThanhPham,
            MA_DAI_DIEN,
            CaLamViec
        UNION ALL
        SELECT p.StartTime,
            p.GioBatDau,
            p.GioKetThuc,
            nvn.MaNhanVien,
            p.MaLoaiNguyenLieu,
            p.MaSize,
            p.MaThanhPham,
            p.MA_DAI_DIEN,
            p.CaLamViec,
            p.NgayLamViec,
            cast(
                (p.TongTrongLuong / nvn.TongHeSo) * nvn.HeSo as decimal(18, 3)
            ) as TongTrongLuong -- nvn.HeSo,
            -- nvn.MaNhanVien,
            -- nvn.MaNhom,
            -- nvn.TongHeSo
        from #TempThoiGianLamViecNhom p OUTER APPLY (Select nvn.MaNhanVien,nvn.HeSo,nvn.MaNhom, SUM(nvn.HeSo) over (partition by NgayGioBatDau, MaNhom ORDER BY NgayGioBatDau) as TongHeSo from #TempNhanVienTheoNhom nvn where nvn.MaNhom = p.MA_DAI_DIEN and nvn.NgayGioBatDau = p.StartTime ) nvn) p
            LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
            LEFT JOIN HQ_LoaiNguyenLieu nl on p.MaLoaiNguyenLieu = nl.Id
            LEFT JOIN HQ_Size s on p.MaSize = s.Id
            LEFT JOIN HQ_ThanhPham tp on p.MaThanhPham = tp.Id
            LEFT JOIN #MapSanPhamTinhLuong map on p.MaLoaiNguyenLieu = map.MaLoaiNguyenLieu
            and p.MaSize = map.MaSize
            and p.MaThanhPham = map.MaThanhPham;";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query).ToList();
                return items;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        /// <summary>
        /// Map lại tên và mã sản phẩm theo MaSanPham của HQ_Nhom nếu không thể lấy chính xác được tên sản phẩm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="xuongId"></param>
        /// <returns></returns>
        public List<T> GetTongHopSanLuong2<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = $@"Declare @fromDate datetime = '{fromDate:yyyy-MM-dd HH:mm:ss}',
    @toDate datetime = '{toDate:yyyy-MM-dd HH:mm:ss}' IF OBJECT_ID('tempdb..#TempPhieuCan') IS NOT NULL DROP TABLE #TempPhieuCan;
    IF OBJECT_ID('tempdb..#TempNhanVienNhom') IS NOT NULL DROP TABLE #TempNhanVienNhom;
    IF OBJECT_ID('tempdb..#TempNhanVienNhom2') IS NOT NULL DROP TABLE #TempNhanVienNhom2;
    IF OBJECT_ID('tempdb..#TempNhanVienNhom3') IS NOT NULL DROP TABLE #TempNhanVienNhom3;
    if OBJECT_ID('tempdb..#TempMaNhomNhanVienDaiDien') IS NOT NULL DROP TABLE #TempMaNhomNhanVienDaiDien;
    IF OBJECT_ID('tempdb..#TempThoiGianLamViec') IS NOT NULL DROP TABLE #TempThoiGianLamViec;
    IF OBJECT_ID('tempdb..#TempNhanVienCa') IS NOT NULL DROP TABLE #TempNhanVienCa;
    IF OBJECT_ID('tempdb..#LichSuThayDoiNhom') IS NOT NULL DROP TABLE #LichSuThayDoiNhom;
    IF OBJECT_ID('tempdb..#RankedDataNhom') IS NOT NULL DROP TABLE #RankedDataNhom;
    IF OBJECT_ID('tempdb..#TempThoiGianLamViecNhom') IS NOT NULL DROP TABLE #TempThoiGianLamViecNhom;
    IF OBJECT_ID('tempdb..#TempNhanVienTheoNhom') IS NOT NULL DROP TABLE #TempNhanVienTheoNhom;
    IF OBJECT_ID('tempdb..#MapSanPhamTinhLuong') IS NOT NULL DROP TABLE #MapSanPhamTinhLuong;
    WITH MapSanPhamTinhLuong AS (
        SELECT p.Id,
            p.MaSanPham,
            dg.Ten as SanPhamName,
            p.MaThanhPham,
            p.MaSize,
            p.MaLoaiNguyenLieu,
            p.NgayGio,
            ROW_NUMBER() OVER (
                PARTITION BY p.MaThanhPham,
                p.MaSanPham,
                p.MaSize,
                p.MaLoaiNguyenLieu
                ORDER BY p.NgayGio DESC
            ) AS RowNum
        FROM HQ_MapSanPhamTinhLuong p,
            DG_SanPhamTinhLuong dg
        WHERE p.NgayGio <= @toDate
            and p.MaSanPham = dg.Ma
    )
SELECT Id,
    MaSanPham,
    SanPhamName,
    MaThanhPham,
    MaSize,
    MaLoaiNguyenLieu,
    NgayGio
    into #MapSanPhamTinhLuong
FROM MapSanPhamTinhLuong
WHERE RowNum = 1 -- 1️⃣ Tạo bảng tạm chứa dữ liệu phiếu cân với phân loại ca làm việc
SELECT p.*,
    CASE
        WHEN DATEPART(HOUR, p.NgayGio) >= 5
        AND DATEPART(HOUR, p.NgayGio) < 17 THEN 'N'
        ELSE 'D'
    END AS CaLamViec,
    CONVERT(
        DATE,
        CASE
            WHEN DATEPART(HOUR, p.NgayGio) >= 5 THEN p.NgayGio -- Ca ngày giữ nguyên
            ELSE DATEADD(DAY, -1, p.NgayGio) -- Ca đêm tính vào ngày trước đó
        END
    ) AS NgayLamViec INTO #TempPhieuCan
FROM HQ_PhieuCan p
WHERE p.NgayGio >= @fromDate
    AND p.NgayGio <= @toDate and MaXuong ='{xuongId}'
order by p.NgayGio;
-- Select *
-- from #TempPhieuCan;
WITH LichSuThayDoi AS (
    SELECT NgayGioBatDau,
        LAG(NgayGioBatDau) OVER (
            ORDER BY NgayGioBatDau
        ) AS NgayGioTruoc
    FROM HQ_NhanVienTheoNhom
    WHERE NgayGioBatDau BETWEEN @fromDate AND @toDate
),
DanhSachChinh AS (
    SELECT NgayGioBatDau,
        DATEDIFF(MINUTE, NgayGioTruoc, NgayGioBatDau) AS ThoiGianTruocSau
    FROM LichSuThayDoi
)
SELECT @fromDate as NgayGioBatDau into #LichSuThayDoiNhom
UNION ALL
SELECT NgayGioBatDau
FROM DanhSachChinh
WHERE ThoiGianTruocSau IS NULL
    or ThoiGianTruocSau > 5 -- 2️⃣ Xác định ca làm việc đầu tiên của mỗi nhân viên trong ngày
SELECT p.MaNhanVien,
    p.NgayLamViec,
    MIN(p.NgayGio) AS GioBatDau,
    (
        SELECT TOP 1 CaLamViec
        FROM #TempPhieuCan pc 
        WHERE pc.MaNhanVien = p.MaNhanVien
            AND pc.NgayLamViec = p.NgayLamViec
        ORDER BY pc.NgayGio ASC
    ) AS CaDinhDanh INTO #TempNhanVienCa
FROM #TempPhieuCan p
GROUP BY p.MaNhanVien,
    p.NgayLamViec;
-- 3️⃣ Lấy nhóm nhân viên theo thời gian mới nhất
SELECT p.NgayLamViec,
    p.NgayGio,
    p.MaNhanVien,
    nv.Name as TenNhanVien,
    nv.IsNhom,
    p.MaLoaiNguyenLieu,
    p.MaSize,
    p.MaThanhPham,
    COALESCE(nvt.MaNhom, '000') as MA_DAI_DIEN,
    nc.CaDinhDanh AS CaLamViec,
    nvt.HeSo,
    p.TrongLuong INTO #TempNhanVienNhom
FROM #TempPhieuCan p
    LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
    JOIN #TempNhanVienCa nc ON p.MaNhanVien = nc.MaNhanVien AND p.NgayLamViec = nc.NgayLamViec
    OUTER APPLY (
        SELECT TOP 1 nvt.MaNhom,
            nvt.HeSo
        FROM HQ_NhanVienTheoNhom nvt
        WHERE nvt.MaNhanVien = p.MaNhanVien
            AND nvt.NgayGioBatDau <= p.NgayGio
        ORDER BY nvt.NgayGioBatDau DESC
    ) nvt;
Select nn.MaNhanVien,
    dbo.non_unicode_convert(upper(nn.TenNhanVien)) as TenNhanVien,
    dbo.non_unicode_convert(upper(nhom.Ten)) As TenNhom,
    nhom.Id as MaNhom INTO #TempMaNhomNhanVienDaiDien
from (
        Select DISTINCT MaNhanVien,
            TenNhanVien
        FROM #TempNhanVienNhom p where p.IsNhom =1 ) nn, HQ_Nhom nhom where dbo.non_unicode_convert(upper(nn.TenNhanVien))= dbo.non_unicode_convert(upper(nhom.Ten)) ;
        Select p.NgayLamViec,
            p.NgayGio,
            p.MaNhanVien,
            p.TenNhanVien,
            p.IsNhom,
            p.MaLoaiNguyenLieu,
            p.MaSize,
            p.MaThanhPham,
            COALESCE(nvt.MaNhom, p.MA_DAI_DIEN) as MA_DAI_DIEN,
            p.CaLamViec,
            COALESCE(p.HeSo, 1) as HeSo,
            p.TrongLuong into #TempNhanVienNhom2
        from #TempNhanVienNhom p 
            LEFT JOIN #TempMaNhomNhanVienDaiDien nvt on p.MaNhanVien = nvt.MaNhanVien;
            -- 4️⃣ Tính thời gian làm việc của nhân viên theo tổ hợp (Nhân viên/Nhóm, Nguyên Liệu,,Thành Phẩm, Size)
        Select l.NgayGioBatDau,
            nhommoinhat.MaNhom,
            nhommoinhat.MaNhanVien,
            nhommoinhat.HeSo into #TempNhanVienTheoNhom
        from #LichSuThayDoiNhom l
            OUTER APPLY (
                SELECT MaNhanVien,
                    MaNhom,
                    NgayGioBatDau,
                    HeSo
                FROM (
                        SELECT MaNhanVien,
                            MaNhom,
                            NgayGioBatDau,
                            HeSo,
                            ROW_NUMBER() OVER (
                                PARTITION BY MaNhanVien
                                ORDER BY NgayGioBatDau DESC
                            ) AS RowNum
                        FROM HQ_NhanVienTheoNhom
                        where NgayGioBatDau <= l.NgayGioBatDau
                    ) AS LatestGroups
                WHERE RowNum = 1
            ) nhommoinhat;
WITH TimeRanges AS (
    SELECT NgayGioBatDau AS StartTime,
        LEAD(NgayGioBatDau) OVER (
            ORDER BY NgayGioBatDau
        ) AS EndTime
    FROM #LichSuThayDoiNhom
)
Select t.StartTime,
    p.* into #TempNhanVienNhom3
from (
        Select *
        from #TempNhanVienNhom2 p where MA_DAI_DIEN!='000') p
            JOIN TimeRanges t ON p.NgayGio >= t.StartTime
            AND (
                p.NgayGio < t.EndTime
                OR t.EndTime IS NULL
            )
        ORDER BY p.MA_DAI_DIEN,
            p.NgayGio;
-- Select *
-- from #TempNhanVienNhom3 ; 
SELECT StartTime,
    NgayGio,
    NgayLamViec,
    MaLoaiNguyenLieu,
    MaSize,
    MaThanhPham,
    MA_DAI_DIEN,
    CaLamViec,
    TrongLuong,
    LAG(MA_DAI_DIEN) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_MaDaiDien,
    LAG(MaLoaiNguyenLieu) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_MaLoaiNguyenLieu,
    LAG(MaSize) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_MaSize,
    LAG(MaThanhPham) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_MaThanhPham,
    LAG(StartTime) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_StartTime into #RankedDataNhom
FROM #TempNhanVienNhom3 
ORDER BY NgayLamViec,
    MA_DAI_DIEN,
    NgayGio;
with GroupedData AS (
    SELECT *,
        SUM(
            CASE
                WHEN MA_DAI_DIEN != Prev_MaDaiDien
                or MaLoaiNguyenLieu != Prev_MaLoaiNguyenLieu
                OR MaSize != Prev_MaSize
                OR MaThanhPham != Prev_MaThanhPham
                OR StartTime != Prev_StartTime THEN 1
                ELSE 0
            END
        ) OVER (
            PARTITION BY NgayLamViec,
            MA_DAI_DIEN
            ORDER BY NgayGio
        ) AS GroupID
    FROM #RankedDataNhom
)
SELECT StartTime,
    MIN(NgayGio) AS GioBatDau,
    MAX(NgayGio) AS GioKetThuc,
    MaLoaiNguyenLieu,
    MaSize,
    MaThanhPham,
    MA_DAI_DIEN,
    CaLamViec,
    NgayLamViec,
    SUM(TrongLuong) AS TongTrongLuong into #TempThoiGianLamViecNhom
FROM GroupedData
GROUP BY NgayLamViec,
    StartTime,
    GroupID,
    MaLoaiNguyenLieu,
    MaSize,
    MaThanhPham,
    MA_DAI_DIEN,
    CaLamViec
ORDER BY NgayLamViec,
    StartTime,
    ma_dai_dien,
    GioBatDau;
WITH RankedData AS (
    SELECT NgayGio,
        NgayLamViec,
        MaNhanVien,
        TenNhanVien,
        IsNhom,
        MaLoaiNguyenLieu,
        MaSize,
        MaThanhPham,
        MA_DAI_DIEN,
        CaLamViec,
        HeSo,
        TrongLuong,
        LAG(MaNhanVien) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS Prev_MaNhanVien,
        LAG(MaLoaiNguyenLieu) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS Prev_MaLoaiNguyenLieu,
        LAG(MaSize) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS Prev_MaSize,
        LAG(MaThanhPham) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS Prev_MaThanhPham
    FROM #TempNhanVienNhom2 where MA_DAI_DIEN ='000'
),
GroupedData AS (
    SELECT *,
        SUM(
            CASE
                WHEN MaNhanVien != Prev_MaNhanVien
                OR MaLoaiNguyenLieu != Prev_MaLoaiNguyenLieu
                OR MaSize != Prev_MaSize
                OR MaThanhPham != Prev_MaThanhPham THEN 1
                ELSE 0
            END
        ) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS GroupID
    FROM RankedData
)
Select p.*,
    nv.Name as TenNhanVien,
    nl.Ten as TenLoaiNguyenLieu,
    s.Ten as TenSize,
    tp.Ten as TenThanhPham,
    COALESCE(
        map.MaSanPham, 
        nhom.MaSanPham,  -- Nếu không có trong map, thì lấy từ nhóm
        'N/A'  -- Giá trị mặc định nếu không tìm thấy
    ) as MaSanPham,
    COALESCE(
        map.SanPhamName, 
        nhomsptl.Ten,  -- Nếu không có trong map, lấy tên từ leftjoin sptl
        N'Không xác định' 
    ) as SanPhamName
from (
    SELECT @fromDate as StartTime,
        MIN(NgayGio) AS GioBatDau,
        MAX(NgayGio) AS GioKetThuc,
        MaNhanVien,
        MaLoaiNguyenLieu,
        MaSize,
        MaThanhPham,
        MA_DAI_DIEN,
        CaLamViec,
        NgayLamViec,
        cast(SUM(TrongLuong * HeSo) as decimal(18, 3)) AS TongTrongLuong
    FROM GroupedData
    GROUP BY NgayLamViec,
        GroupID,
        MaNhanVien,
        MaLoaiNguyenLieu,
        MaSize,
        MaThanhPham,
        MA_DAI_DIEN,
        CaLamViec
    UNION ALL
    SELECT p.StartTime,
        p.GioBatDau,
        p.GioKetThuc,
        nvn.MaNhanVien,
        p.MaLoaiNguyenLieu,
        p.MaSize,
        p.MaThanhPham,
        p.MA_DAI_DIEN,
        p.CaLamViec,
        p.NgayLamViec,
        cast(
            (p.TongTrongLuong / nvn.TongHeSo) * nvn.HeSo as decimal(18, 3)
        ) as TongTrongLuong
    from #TempThoiGianLamViecNhom p 
    OUTER APPLY (
        Select 
            nvn.MaNhanVien,
            nvn.HeSo,
            nvn.MaNhom, 
            SUM(nvn.HeSo) over (partition by NgayGioBatDau, MaNhom ORDER BY NgayGioBatDau) as TongHeSo 
        from #TempNhanVienTheoNhom nvn 
        where nvn.MaNhom = p.MA_DAI_DIEN and nvn.NgayGioBatDau = p.StartTime 
    ) nvn
) p
LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
LEFT JOIN HQ_LoaiNguyenLieu nl on p.MaLoaiNguyenLieu = nl.Id
LEFT JOIN HQ_Size s on p.MaSize = s.Id
LEFT JOIN HQ_ThanhPham tp on p.MaThanhPham = tp.Id
LEFT JOIN #MapSanPhamTinhLuong map on 
    p.MaLoaiNguyenLieu = map.MaLoaiNguyenLieu
    and p.MaSize = map.MaSize
    and p.MaThanhPham = map.MaThanhPham
LEFT JOIN HQ_Nhom nhom on p.MA_DAI_DIEN = nhom.Id
left join DG_SanPhamTinhLuong nhomsptl on nhom.MaSanPham = nhomsptl.Ma";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query).ToList();
                return items;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        /// <summary>
        /// Nếu là Nhóm != '000' thì lấy tên sản phẩm tính lương từ HQ_Nhom
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="xuongId"></param>
        /// <returns></returns>
        public List<T> GetTongHopSanLuong3<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = $@"Declare @fromDate datetime = '{fromDate:yyyy-MM-dd HH:mm:ss}',
    @toDate datetime = '{toDate:yyyy-MM-dd HH:mm:ss}' IF OBJECT_ID('tempdb..#TempPhieuCan') IS NOT NULL DROP TABLE #TempPhieuCan;
    IF OBJECT_ID('tempdb..#TempNhanVienNhom') IS NOT NULL DROP TABLE #TempNhanVienNhom;
    IF OBJECT_ID('tempdb..#TempNhanVienNhom2') IS NOT NULL DROP TABLE #TempNhanVienNhom2;
    IF OBJECT_ID('tempdb..#TempNhanVienNhom3') IS NOT NULL DROP TABLE #TempNhanVienNhom3;
    if OBJECT_ID('tempdb..#TempMaNhomNhanVienDaiDien') IS NOT NULL DROP TABLE #TempMaNhomNhanVienDaiDien;
    IF OBJECT_ID('tempdb..#TempThoiGianLamViec') IS NOT NULL DROP TABLE #TempThoiGianLamViec;
    IF OBJECT_ID('tempdb..#TempNhanVienCa') IS NOT NULL DROP TABLE #TempNhanVienCa;
    IF OBJECT_ID('tempdb..#LichSuThayDoiNhom') IS NOT NULL DROP TABLE #LichSuThayDoiNhom;
    IF OBJECT_ID('tempdb..#RankedDataNhom') IS NOT NULL DROP TABLE #RankedDataNhom;
    IF OBJECT_ID('tempdb..#TempThoiGianLamViecNhom') IS NOT NULL DROP TABLE #TempThoiGianLamViecNhom;
    IF OBJECT_ID('tempdb..#TempNhanVienTheoNhom') IS NOT NULL DROP TABLE #TempNhanVienTheoNhom;
    IF OBJECT_ID('tempdb..#MapSanPhamTinhLuong') IS NOT NULL DROP TABLE #MapSanPhamTinhLuong;
    WITH MapSanPhamTinhLuong AS (
        SELECT p.Id,
            p.MaSanPham,
            dg.Ten as SanPhamName,
            p.MaThanhPham,
            p.MaSize,
            p.MaLoaiNguyenLieu,
            p.NgayGio,
            ROW_NUMBER() OVER (
                PARTITION BY p.MaThanhPham,
                p.MaSanPham,
                p.MaSize,
                p.MaLoaiNguyenLieu
                ORDER BY p.NgayGio DESC
            ) AS RowNum
        FROM HQ_MapSanPhamTinhLuong p,
            DG_SanPhamTinhLuong dg
        WHERE p.NgayGio <= @toDate
            and p.MaSanPham = dg.Ma
    )
SELECT Id,
    MaSanPham,
    SanPhamName,
    MaThanhPham,
    MaSize,
    MaLoaiNguyenLieu,
    NgayGio
    into #MapSanPhamTinhLuong
FROM MapSanPhamTinhLuong
WHERE RowNum = 1 -- 1️⃣ Tạo bảng tạm chứa dữ liệu phiếu cân với phân loại ca làm việc
SELECT p.*,
    CASE
        WHEN DATEPART(HOUR, p.NgayGio) >= 5
        AND DATEPART(HOUR, p.NgayGio) < 17 THEN 'N'
        ELSE 'D'
    END AS CaLamViec,
    CONVERT(
        DATE,
        CASE
            WHEN DATEPART(HOUR, p.NgayGio) >= 5 THEN p.NgayGio -- Ca ngày giữ nguyên
            ELSE DATEADD(DAY, -1, p.NgayGio) -- Ca đêm tính vào ngày trước đó
        END
    ) AS NgayLamViec INTO #TempPhieuCan
FROM HQ_PhieuCan p
WHERE p.NgayGio >= @fromDate
    AND p.NgayGio <= @toDate and MaXuong ='{xuongId}'
order by p.NgayGio;
-- Select *
-- from #TempPhieuCan;
WITH LichSuThayDoi AS (
    SELECT NgayGioBatDau,
        LAG(NgayGioBatDau) OVER (
            ORDER BY NgayGioBatDau
        ) AS NgayGioTruoc
    FROM HQ_NhanVienTheoNhom
    WHERE NgayGioBatDau BETWEEN @fromDate AND @toDate
),
DanhSachChinh AS (
    SELECT NgayGioBatDau,
        DATEDIFF(MINUTE, NgayGioTruoc, NgayGioBatDau) AS ThoiGianTruocSau
    FROM LichSuThayDoi
)
SELECT @fromDate as NgayGioBatDau into #LichSuThayDoiNhom
UNION ALL
SELECT NgayGioBatDau
FROM DanhSachChinh
WHERE ThoiGianTruocSau IS NULL
    or ThoiGianTruocSau > 0 -- 2️⃣ Xác định ca làm việc đầu tiên của mỗi nhân viên trong ngày
SELECT p.MaNhanVien,
    p.NgayLamViec,
    MIN(p.NgayGio) AS GioBatDau,
    (
        SELECT TOP 1 CaLamViec
        FROM #TempPhieuCan pc 
        WHERE pc.MaNhanVien = p.MaNhanVien
            AND pc.NgayLamViec = p.NgayLamViec
        ORDER BY pc.NgayGio ASC
    ) AS CaDinhDanh INTO #TempNhanVienCa
FROM #TempPhieuCan p
GROUP BY p.MaNhanVien,
    p.NgayLamViec;
-- 3️⃣ Lấy nhóm nhân viên theo thời gian mới nhất
SELECT p.NgayLamViec,
    p.NgayGio,
    p.MaNhanVien,
    nv.Name as TenNhanVien,
    nv.IsNhom,
    p.MaLoaiNguyenLieu,
    p.MaSize,
    p.MaThanhPham,
    COALESCE(nvt.MaNhom, '000') as MA_DAI_DIEN,
    nc.CaDinhDanh AS CaLamViec,
    nvt.HeSo,
    p.TrongLuong INTO #TempNhanVienNhom
FROM #TempPhieuCan p
    LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
    JOIN #TempNhanVienCa nc ON p.MaNhanVien = nc.MaNhanVien AND p.NgayLamViec = nc.NgayLamViec
    OUTER APPLY (
        SELECT TOP 1 nvt.MaNhom,
            nvt.HeSo
        FROM HQ_NhanVienTheoNhom nvt
        WHERE nvt.MaNhanVien = p.MaNhanVien
            AND nvt.NgayGioBatDau <= p.NgayGio 
        ORDER BY nvt.NgayGioBatDau DESC
    ) nvt;
Select nn.MaNhanVien,
    dbo.non_unicode_convert(upper(nn.TenNhanVien)) as TenNhanVien,
    dbo.non_unicode_convert(upper(nhom.Ten)) As TenNhom,
    nhom.Id as MaNhom INTO #TempMaNhomNhanVienDaiDien
from (
        Select DISTINCT MaNhanVien,
            TenNhanVien
        FROM #TempNhanVienNhom p where p.IsNhom =1 ) nn, HQ_Nhom nhom where dbo.non_unicode_convert(upper(nn.TenNhanVien))= dbo.non_unicode_convert(upper(nhom.Ten)) ;
        Select p.NgayLamViec,
            p.NgayGio,
            p.MaNhanVien,
            p.TenNhanVien,
            p.IsNhom,
            p.MaLoaiNguyenLieu,
            p.MaSize,
            p.MaThanhPham,
            COALESCE(nvt.MaNhom, p.MA_DAI_DIEN) as MA_DAI_DIEN,
            p.CaLamViec,
            COALESCE(p.HeSo, 1) as HeSo,
            p.TrongLuong into #TempNhanVienNhom2
        from #TempNhanVienNhom p 
            LEFT JOIN #TempMaNhomNhanVienDaiDien nvt on p.MaNhanVien = nvt.MaNhanVien;
            -- 4️⃣ Tính thời gian làm việc của nhân viên theo tổ hợp (Nhân viên/Nhóm, Nguyên Liệu,,Thành Phẩm, Size)
        Select l.NgayGioBatDau,
            nhommoinhat.MaNhom,
            nhommoinhat.MaNhanVien,
            nhommoinhat.HeSo into #TempNhanVienTheoNhom
        from #LichSuThayDoiNhom l
            OUTER APPLY (
                SELECT MaNhanVien,
                    MaNhom,
                    NgayGioBatDau,
                    HeSo
                FROM (
                        SELECT MaNhanVien,
                            MaNhom,
                            NgayGioBatDau,
                            HeSo,
                            ROW_NUMBER() OVER (
                                PARTITION BY MaNhanVien
                                ORDER BY NgayGioBatDau DESC
                            ) AS RowNum
                        FROM HQ_NhanVienTheoNhom
                        where NgayGioBatDau <= l.NgayGioBatDau
                    ) AS LatestGroups
                WHERE RowNum = 1
            ) nhommoinhat;
WITH TimeRanges AS (
    SELECT NgayGioBatDau AS StartTime,
        LEAD(NgayGioBatDau) OVER (
            ORDER BY NgayGioBatDau
        ) AS EndTime
    FROM #LichSuThayDoiNhom
)
Select t.StartTime,
    p.* into #TempNhanVienNhom3
from (
        Select *
        from #TempNhanVienNhom2 p where MA_DAI_DIEN!='000') p
            JOIN TimeRanges t ON p.NgayGio >= t.StartTime
            AND (
                p.NgayGio < t.EndTime
                OR t.EndTime IS NULL
            )
        ORDER BY p.MA_DAI_DIEN,
            p.NgayGio;
-- Select *
-- from #TempNhanVienNhom3 ; 
SELECT StartTime,
    NgayGio,
    NgayLamViec,
    MaLoaiNguyenLieu,
    MaSize,
    MaThanhPham,
    MA_DAI_DIEN,
    CaLamViec,
    TrongLuong,
    LAG(MA_DAI_DIEN) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_MaDaiDien,
    LAG(MaLoaiNguyenLieu) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_MaLoaiNguyenLieu,
    LAG(MaSize) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_MaSize,
    LAG(MaThanhPham) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_MaThanhPham,
    LAG(StartTime) OVER (
        PARTITION BY NgayLamViec
        ORDER BY MA_DAI_DIEN,
            NgayGio
    ) AS Prev_StartTime into #RankedDataNhom
FROM #TempNhanVienNhom3 
ORDER BY NgayLamViec,
    MA_DAI_DIEN,
    NgayGio;
with GroupedData AS (
    SELECT *,
        SUM(
            CASE
                WHEN MA_DAI_DIEN != Prev_MaDaiDien
                or MaLoaiNguyenLieu != Prev_MaLoaiNguyenLieu
                OR MaSize != Prev_MaSize
                OR MaThanhPham != Prev_MaThanhPham
                OR StartTime != Prev_StartTime THEN 1
                ELSE 0
            END
        ) OVER (
            PARTITION BY NgayLamViec,
            MA_DAI_DIEN
            ORDER BY NgayGio
        ) AS GroupID
    FROM #RankedDataNhom
)
SELECT StartTime,
    MIN(NgayGio) AS GioBatDau,
    MAX(NgayGio) AS GioKetThuc,
    MaLoaiNguyenLieu,
    MaSize,
    MaThanhPham,
    MA_DAI_DIEN,
    CaLamViec,
    NgayLamViec,
    SUM(TrongLuong) AS TongTrongLuong into #TempThoiGianLamViecNhom
FROM GroupedData
GROUP BY NgayLamViec,
    StartTime,
    GroupID,
    MaLoaiNguyenLieu,
    MaSize,
    MaThanhPham,
    MA_DAI_DIEN,
    CaLamViec
ORDER BY NgayLamViec,
    StartTime,
    ma_dai_dien,
    GioBatDau;
WITH RankedData AS (
    SELECT NgayGio,
        NgayLamViec,
        MaNhanVien,
        TenNhanVien,
        IsNhom,
        MaLoaiNguyenLieu,
        MaSize,
        MaThanhPham,
        MA_DAI_DIEN,
        CaLamViec,
        HeSo,
        TrongLuong,
        LAG(MaNhanVien) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS Prev_MaNhanVien,
        LAG(MaLoaiNguyenLieu) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS Prev_MaLoaiNguyenLieu,
        LAG(MaSize) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS Prev_MaSize,
        LAG(MaThanhPham) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS Prev_MaThanhPham
    FROM #TempNhanVienNhom2 where MA_DAI_DIEN ='000'
),
GroupedData AS (
    SELECT *,
        SUM(
            CASE
                WHEN MaNhanVien != Prev_MaNhanVien
                OR MaLoaiNguyenLieu != Prev_MaLoaiNguyenLieu
                OR MaSize != Prev_MaSize
                OR MaThanhPham != Prev_MaThanhPham THEN 1
                ELSE 0
            END
        ) OVER (
            PARTITION BY NgayLamViec
            ORDER BY NgayGio
        ) AS GroupID
    FROM RankedData
)
Select p.*,
    nv.Name as TenNhanVien,
    nl.Ten as TenLoaiNguyenLieu,
    s.Ten as TenSize,
    tp.Ten as TenThanhPham,
    CASE
    WHEN p.MA_DAI_DIEN != '000' AND (nhom.MaSanPham IS NULL OR LTRIM(RTRIM(nhom.MaSanPham)) = '''') THEN ISNULL(map.MaSanPham, 'N/A')
    WHEN p.MA_DAI_DIEN != '000' THEN ISNULL(nhom.MaSanPham, 'N/A')
    ELSE ISNULL(map.MaSanPham, 'N/A')
END AS MaSanPham,

CASE
    WHEN p.MA_DAI_DIEN != '000' AND (nhom.MaSanPham IS NULL OR LTRIM(RTRIM(nhom.MaSanPham)) = '''') THEN ISNULL(map.SanPhamName, N'Không xác định')
    WHEN p.MA_DAI_DIEN != '000' THEN ISNULL(sp.Ten, N'Không xác định')
    ELSE ISNULL(map.SanPhamName, N'Không xác định')
END AS SanPhamName
--viết thử
--CASE
--    WHEN p.MA_DAI_DIEN != '000' THEN ISNULL(NULLIF(nhom.MaSanPham, ''''), map.MaSanPham)
--    ELSE map.MaSanPham
--END AS MaSanPham,

--CASE
--    WHEN p.MA_DAI_DIEN != '000' THEN ISNULL(NULLIF(sp.Ten, ''''), map.SanPhamName)
--    ELSE map.SanPhamName
--END AS SanPhamName
from (
        SELECT @fromDate as StartTime,
            MIN(NgayGio) AS GioBatDau,
            MAX(NgayGio) AS GioKetThuc,
            MaNhanVien,
            MaLoaiNguyenLieu,
            MaSize,
            MaThanhPham,
            MA_DAI_DIEN,
            CaLamViec,
            NgayLamViec,
            cast(SUM(TrongLuong * HeSo) as decimal(18, 3)) AS TongTrongLuong
        FROM GroupedData
        GROUP BY NgayLamViec,
            GroupID,
            MaNhanVien,
            MaLoaiNguyenLieu,
            MaSize,
            MaThanhPham,
            MA_DAI_DIEN,
            CaLamViec
        UNION ALL
        Select * from (SELECT p.StartTime,
            p.GioBatDau,
            p.GioKetThuc,
            nvn.MaNhanVien,
            p.MaLoaiNguyenLieu,
            p.MaSize,
            p.MaThanhPham,
            p.MA_DAI_DIEN,
            p.CaLamViec,
            p.NgayLamViec,
            cast(
                (p.TongTrongLuong / nvn.TongHeSo) * nvn.HeSo as decimal(18, 3)
            ) as TongTrongLuong
        from #TempThoiGianLamViecNhom p OUTER APPLY (
            Select nvn.MaNhanVien,
                nvn.HeSo,
                nvn.MaNhom, 
                SUM(nvn.HeSo) over (partition by NgayGioBatDau, MaNhom ORDER BY NgayGioBatDau) as TongHeSo 
            from #TempNhanVienTheoNhom nvn 
            where nvn.MaNhom = p.MA_DAI_DIEN 
            and nvn.NgayGioBatDau = p.StartTime 
        ) nvn) nvn where ISNULL( nvn.MaNhanVien ,'') != '' 
    ) p
    LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
    LEFT JOIN HQ_LoaiNguyenLieu nl on p.MaLoaiNguyenLieu = nl.Id
    LEFT JOIN HQ_Size s on p.MaSize = s.Id
    LEFT JOIN HQ_ThanhPham tp on p.MaThanhPham = tp.Id
    LEFT JOIN #MapSanPhamTinhLuong map on p.MaLoaiNguyenLieu = map.MaLoaiNguyenLieu
        and p.MaSize = map.MaSize
        and p.MaThanhPham = map.MaThanhPham
   
    LEFT JOIN HQ_Nhom nhom ON p.MA_DAI_DIEN = nhom.Id AND p.MA_DAI_DIEN != '000'
   
    LEFT JOIN DG_SanPhamTinhLuong sp ON nhom.MaSanPham = sp.Ma 
        AND p.MA_DAI_DIEN != '000';";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query).ToList();
                return items;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public List<T> GetTongHopSanLuong4<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = $@"Declare @fromDate datetime = '{fromDate:yyyy-MM-dd HH:mm:ss}',
    @toDate datetime = '{toDate:yyyy-MM-dd HH:mm:ss}'
	
	IF OBJECT_ID('tempdb..#TempPhieuCan') IS NOT NULL DROP TABLE #TempPhieuCan;
    IF OBJECT_ID('tempdb..#TempNhanVienNhom') IS NOT NULL DROP TABLE #TempNhanVienNhom;
    IF OBJECT_ID('tempdb..#TempNhanVienNhom2') IS NOT NULL DROP TABLE #TempNhanVienNhom2;
    IF OBJECT_ID('tempdb..#TempNhanVienNhom3') IS NOT NULL DROP TABLE #TempNhanVienNhom3;
    if OBJECT_ID('tempdb..#TempMaNhomNhanVienDaiDien') IS NOT NULL DROP TABLE #TempMaNhomNhanVienDaiDien;
    IF OBJECT_ID('tempdb..#TempThoiGianLamViec') IS NOT NULL DROP TABLE #TempThoiGianLamViec;
    IF OBJECT_ID('tempdb..#TempNhanVienCa') IS NOT NULL DROP TABLE #TempNhanVienCa;
    IF OBJECT_ID('tempdb..#LichSuThayDoiNhom') IS NOT NULL DROP TABLE #LichSuThayDoiNhom;
    IF OBJECT_ID('tempdb..#RankedDataNhom') IS NOT NULL DROP TABLE #RankedDataNhom;
    IF OBJECT_ID('tempdb..#TempThoiGianLamViecNhom') IS NOT NULL DROP TABLE #TempThoiGianLamViecNhom;
    IF OBJECT_ID('tempdb..#TempNhanVienTheoNhom') IS NOT NULL DROP TABLE #TempNhanVienTheoNhom;
    IF OBJECT_ID('tempdb..#MapSanPhamTinhLuong') IS NOT NULL DROP TABLE #MapSanPhamTinhLuong;
    WITH MapSanPhamTinhLuong AS (
        SELECT p.Id,
            p.MaSanPham,
            dg.Ten as SanPhamName,
            p.MaThanhPham,
            p.MaSize,
            p.MaLoaiNguyenLieu,
            p.NgayGio,
            ROW_NUMBER() OVER (
                PARTITION BY p.MaThanhPham,
                p.MaSanPham,
                p.MaSize,
                p.MaLoaiNguyenLieu
                ORDER BY p.NgayGio DESC
            ) AS RowNum
        FROM HQ_MapSanPhamTinhLuong p,
            DG_SanPhamTinhLuong dg
        WHERE p.NgayGio <= @toDate
            and p.MaSanPham = dg.Ma
    )
SELECT Id,
    MaSanPham,
    SanPhamName,
    MaThanhPham,
    MaSize,
    MaLoaiNguyenLieu,
    NgayGio
    into #MapSanPhamTinhLuong
FROM MapSanPhamTinhLuong
WHERE RowNum = 1 -- 1️⃣ Tạo bảng tạm chứa dữ liệu phiếu cân với phân loại ca làm việc
SELECT p.*,
    CASE
        WHEN DATEPART(HOUR, p.NgayGio) >= 5
        AND DATEPART(HOUR, p.NgayGio) < 17 THEN 'N'
        ELSE 'D'
    END AS CaLamViec,
    CONVERT(
        DATE,
        CASE
            WHEN DATEPART(HOUR, p.NgayGio) >= 5 THEN p.NgayGio -- Ca ngày giữ nguyên
            ELSE DATEADD(DAY, -1, p.NgayGio) -- Ca đêm tính vào ngày trước đó
        END
    ) AS NgayLamViec INTO #TempPhieuCan
FROM HQ_PhieuCan p
WHERE p.NgayGio >= @fromDate
    AND p.NgayGio <= @toDate and MaXuong ='1'
order by p.NgayGio;
-- Select *
-- from #TempPhieuCan;
WITH LichSuThayDoi AS (
    SELECT NgayGioBatDau,
        LAG(NgayGioBatDau) OVER (
            ORDER BY NgayGioBatDau
        ) AS NgayGioTruoc
    FROM HQ_NhanVienTheoNhom
    WHERE NgayGioBatDau BETWEEN @fromDate AND @toDate
),
DanhSachChinh AS (
    SELECT NgayGioBatDau,
        DATEDIFF(MINUTE, NgayGioTruoc, NgayGioBatDau) AS ThoiGianTruocSau
    FROM LichSuThayDoi
)
SELECT @fromDate as NgayGioBatDau into #LichSuThayDoiNhom
UNION ALL
SELECT NgayGioBatDau
FROM DanhSachChinh
WHERE ThoiGianTruocSau IS NULL
    or ThoiGianTruocSau > 0 -- 2️⃣ Xác định ca làm việc đầu tiên của mỗi nhân viên trong ngày
SELECT p.MaNhanVien,
    p.NgayLamViec,
    MIN(p.NgayGio) AS GioBatDau,
    (
        SELECT TOP 1 CaLamViec
        FROM #TempPhieuCan pc 
        WHERE pc.MaNhanVien = p.MaNhanVien
            AND pc.NgayLamViec = p.NgayLamViec
        ORDER BY pc.NgayGio ASC
    ) AS CaDinhDanh INTO #TempNhanVienCa
FROM #TempPhieuCan p
GROUP BY p.MaNhanVien,
    p.NgayLamViec;
-- 3️⃣ Lấy nhóm nhân viên theo thời gian mới nhất
SELECT p.NgayLamViec,
    p.NgayGio,
    p.MaNhanVien,
    nv.Name as TenNhanVien,
    nv.IsNhom,
    p.MaLoaiNguyenLieu,
    p.MaSize,
    p.MaThanhPham,
    COALESCE(nvt.MaNhom, '000') as MA_DAI_DIEN,
    nc.CaDinhDanh AS CaLamViec,
    nvt.HeSo,
    p.TrongLuong INTO #TempNhanVienNhom
FROM #TempPhieuCan p
    LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
    JOIN #TempNhanVienCa nc ON p.MaNhanVien = nc.MaNhanVien AND p.NgayLamViec = nc.NgayLamViec
    OUTER APPLY (
        SELECT TOP 1 nvt.MaNhom,
            nvt.HeSo
        FROM HQ_NhanVienTheoNhom nvt
        WHERE nvt.MaNhanVien = p.MaNhanVien
            AND nvt.NgayGioBatDau <= p.NgayGio
        ORDER BY nvt.NgayGioBatDau DESC
    ) nvt;
Select nn.MaNhanVien,
    dbo.non_unicode_convert(upper(nn.TenNhanVien)) as TenNhanVien,
    dbo.non_unicode_convert(upper(nhom.Ten)) As TenNhom,
    nhom.Id as MaNhom INTO #TempMaNhomNhanVienDaiDien
from (
        Select DISTINCT MaNhanVien,
            TenNhanVien
        FROM #TempNhanVienNhom p where p.IsNhom =1 ) nn, HQ_Nhom nhom where dbo.non_unicode_convert(upper(nn.TenNhanVien))= dbo.non_unicode_convert(upper(nhom.Ten)) ;
        Select p.NgayLamViec,
            p.NgayGio,
            p.MaNhanVien,
            p.TenNhanVien,
            p.IsNhom,
            p.MaLoaiNguyenLieu,
            p.MaSize,
            p.MaThanhPham,
            COALESCE(nvt.MaNhom, p.MA_DAI_DIEN) as MA_DAI_DIEN,
            p.CaLamViec,
            COALESCE(p.HeSo, 1) as HeSo,
            p.TrongLuong into #TempNhanVienNhom2
        from #TempNhanVienNhom p 
            LEFT JOIN #TempMaNhomNhanVienDaiDien nvt on p.MaNhanVien = nvt.MaNhanVien;
            -- 4️⃣ Tính thời gian làm việc của nhân viên theo tổ hợp (Nhân viên/Nhóm, Nguyên Liệu,,Thành Phẩm, Size)
        Select l.NgayGioBatDau,
            nhommoinhat.MaNhom,
            nhommoinhat.MaNhanVien,
            nhommoinhat.HeSo into #TempNhanVienTheoNhom
        from #LichSuThayDoiNhom l
            OUTER APPLY (
                SELECT MaNhanVien,
                    MaNhom,
                    NgayGioBatDau,
                    HeSo
                FROM (
                        SELECT MaNhanVien,
                            MaNhom,
                            NgayGioBatDau,
                            HeSo,
                            ROW_NUMBER() OVER (
                                PARTITION BY MaNhanVien
                                ORDER BY NgayGioBatDau DESC
                            ) AS RowNum
                        FROM HQ_NhanVienTheoNhom
                        where NgayGioBatDau <= l.NgayGioBatDau
                    ) AS LatestGroups
                WHERE RowNum = 1
            ) nhommoinhat;
WITH TimeRanges AS (
    SELECT NgayGioBatDau AS StartTime,
        LEAD(NgayGioBatDau) OVER (
            ORDER BY NgayGioBatDau
        ) AS EndTime
    FROM #LichSuThayDoiNhom
)
Select t.StartTime,
    p.* into #TempNhanVienNhom3
from (
        Select *
        from #TempNhanVienNhom2 p where MA_DAI_DIEN!='000') p
            JOIN TimeRanges t ON p.NgayGio >= t.StartTime
            AND (
                p.NgayGio < t.EndTime
                OR t.EndTime IS NULL
            )
        ORDER BY p.MA_DAI_DIEN,
            p.NgayGio;
-- Select *
-- from #TempNhanVienNhom3 ; 
SELECT StartTime,
    NgayGio,
    NgayLamViec,
    MaLoaiNguyenLieu,
    MaSize,
    MaThanhPham,
    MA_DAI_DIEN,
    CaLamViec,
    TrongLuong,
    LAG(MA_DAI_DIEN) OVER (
        PARTITION BY NgayLamViec,CaLamViec, MA_DAI_DIEN
        ORDER BY 
            NgayGio
    ) AS Prev_MaDaiDien,
    LAG(MaLoaiNguyenLieu) OVER (
        PARTITION BY NgayLamViec,CaLamViec, MA_DAI_DIEN
        ORDER BY 
            NgayGio
    ) AS Prev_MaLoaiNguyenLieu,
    LAG(MaSize) OVER (
        PARTITION BY NgayLamViec,CaLamViec, MA_DAI_DIEN
        ORDER BY 
            NgayGio
    ) AS Prev_MaSize,
    LAG(MaThanhPham) OVER (
        PARTITION BY NgayLamViec ,CaLamViec, MA_DAI_DIEN
        ORDER BY 
            NgayGio
    ) AS Prev_MaThanhPham,
    LAG(StartTime) OVER (
        PARTITION BY NgayLamViec ,CaLamViec, MA_DAI_DIEN
        ORDER BY
            NgayGio
    ) AS Prev_StartTime into #RankedDataNhom
FROM #TempNhanVienNhom3 
ORDER BY NgayLamViec,
    MA_DAI_DIEN,
    NgayGio;
with GroupedData AS (
    SELECT *,
        SUM(
            CASE
                WHEN MA_DAI_DIEN != Prev_MaDaiDien
                or MaLoaiNguyenLieu != Prev_MaLoaiNguyenLieu
                OR MaSize != Prev_MaSize
                OR MaThanhPham != Prev_MaThanhPham
                OR StartTime != Prev_StartTime THEN 1
                ELSE 0
            END
        ) OVER (
            PARTITION BY NgayLamViec,CaLamViec,MA_DAI_DIEN
            ORDER BY NgayGio
        ) AS GroupID
    FROM #RankedDataNhom
)
SELECT StartTime,
    MIN(NgayGio) AS GioBatDau,
    MAX(NgayGio) AS GioKetThuc,
    MaLoaiNguyenLieu,
    MaSize,
    MaThanhPham,
    MA_DAI_DIEN,
    CaLamViec,
    NgayLamViec,
    SUM(TrongLuong) AS TongTrongLuong into #TempThoiGianLamViecNhom
FROM GroupedData
GROUP BY NgayLamViec,
    StartTime,
    GroupID,
    MaLoaiNguyenLieu,
    MaSize,
    MaThanhPham,
    MA_DAI_DIEN,
    CaLamViec
ORDER BY NgayLamViec,
    StartTime,
    MA_DAI_DIEN,
    GioBatDau;
WITH RankedData AS (
    SELECT NgayGio,
        NgayLamViec,
        MaNhanVien,
        TenNhanVien,
        IsNhom,
        MaLoaiNguyenLieu,
        MaSize,
        MaThanhPham,
        MA_DAI_DIEN,
        CaLamViec,
        HeSo,
        TrongLuong,
        LAG(MaNhanVien) OVER (
            PARTITION BY NgayLamViec ,CaLamViec, MaNhanVien
            ORDER BY NgayGio
        ) AS Prev_MaNhanVien,
        LAG(MaLoaiNguyenLieu) OVER (
            PARTITION BY NgayLamViec ,CaLamViec, MaNhanVien
            ORDER BY NgayGio
        ) AS Prev_MaLoaiNguyenLieu,
        LAG(MaSize) OVER (
            PARTITION BY NgayLamViec,CaLamViec, MaNhanVien
            ORDER BY NgayGio
        ) AS Prev_MaSize,
        LAG(MaThanhPham) OVER (
            PARTITION BY NgayLamViec,CaLamViec, MaNhanVien
            ORDER BY NgayGio
        ) AS Prev_MaThanhPham
    FROM #TempNhanVienNhom2 where MA_DAI_DIEN ='000'
),
GroupedData AS (
    SELECT *,
        SUM(
            CASE
                WHEN MaNhanVien != Prev_MaNhanVien
                OR MaLoaiNguyenLieu != Prev_MaLoaiNguyenLieu
                OR MaSize != Prev_MaSize
                OR MaThanhPham != Prev_MaThanhPham THEN 1
                ELSE 0
            END
        ) OVER (
            PARTITION BY NgayLamViec,MaNhanVien,CaLamViec
            ORDER BY NgayGio
        ) AS GroupID
    FROM RankedData
)
Select p.*,
    nv.Name as TenNhanVien,
    nl.Ten as TenLoaiNguyenLieu,
    s.Ten as TenSize,
    tp.Ten as TenThanhPham,
    CASE
    WHEN p.MA_DAI_DIEN != '000' AND (nhom.MaSanPham IS NULL OR LTRIM(RTRIM(nhom.MaSanPham)) = '''') THEN ISNULL(map.MaSanPham, 'N/A')
    WHEN p.MA_DAI_DIEN != '000' THEN ISNULL(nhom.MaSanPham, 'N/A')
    ELSE ISNULL(map.MaSanPham, 'N/A')
END AS MaSanPham,

CASE
    WHEN p.MA_DAI_DIEN != '000' AND (nhom.MaSanPham IS NULL OR LTRIM(RTRIM(nhom.MaSanPham)) = '''') THEN ISNULL(map.SanPhamName, N'Không xác định')
    WHEN p.MA_DAI_DIEN != '000' THEN ISNULL(sp.Ten, N'Không xác định')
    ELSE ISNULL(map.SanPhamName, N'Không xác định')
END AS SanPhamName
--viết thử
--CASE
--    WHEN p.MA_DAI_DIEN != '000' THEN ISNULL(NULLIF(nhom.MaSanPham, ''''), map.MaSanPham)
--    ELSE map.MaSanPham
--END AS MaSanPham,

--CASE
--    WHEN p.MA_DAI_DIEN != '000' THEN ISNULL(NULLIF(sp.Ten, ''''), map.SanPhamName)
--    ELSE map.SanPhamName
--END AS SanPhamName
from (
        SELECT @fromDate as StartTime,
            MIN(NgayGio) AS GioBatDau,
            MAX(NgayGio) AS GioKetThuc,
            MaNhanVien,
            MaLoaiNguyenLieu,
            MaSize,
            MaThanhPham,
            MA_DAI_DIEN,
            CaLamViec,
            NgayLamViec,
            cast(SUM(TrongLuong * HeSo) as decimal(18, 3)) AS TongTrongLuong
        FROM GroupedData
        GROUP BY NgayLamViec,
            GroupID,
            MaNhanVien,
            MaLoaiNguyenLieu,
            MaSize,
            MaThanhPham,
            MA_DAI_DIEN,
            CaLamViec
        UNION ALL
        Select * from (SELECT p.StartTime,
            p.GioBatDau,
            p.GioKetThuc,
            nvn.MaNhanVien,
            p.MaLoaiNguyenLieu,
            p.MaSize,
            p.MaThanhPham,
            p.MA_DAI_DIEN,
            p.CaLamViec,
            p.NgayLamViec,
            cast(
                (p.TongTrongLuong / nvn.TongHeSo) * nvn.HeSo as decimal(18, 3)
            ) as TongTrongLuong
        from #TempThoiGianLamViecNhom p OUTER APPLY (
            Select nvn.MaNhanVien,
                nvn.HeSo,
                nvn.MaNhom, 
                SUM(nvn.HeSo) over (partition by NgayGioBatDau, MaNhom ORDER BY NgayGioBatDau) as TongHeSo 
            from #TempNhanVienTheoNhom nvn 
            where nvn.MaNhom = p.MA_DAI_DIEN 
            and nvn.NgayGioBatDau = p.StartTime 
        ) nvn) nvn where ISNULL( nvn.MaNhanVien ,'') != '' 
    ) p
    LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
    LEFT JOIN HQ_LoaiNguyenLieu nl on p.MaLoaiNguyenLieu = nl.Id
    LEFT JOIN HQ_Size s on p.MaSize = s.Id
    LEFT JOIN HQ_ThanhPham tp on p.MaThanhPham = tp.Id
    LEFT JOIN #MapSanPhamTinhLuong map on p.MaLoaiNguyenLieu = map.MaLoaiNguyenLieu
        and p.MaSize = map.MaSize
        and p.MaThanhPham = map.MaThanhPham
   
    LEFT JOIN HQ_Nhom nhom ON p.MA_DAI_DIEN = nhom.Id AND p.MA_DAI_DIEN != '000'
   
    LEFT JOIN DG_SanPhamTinhLuong sp ON nhom.MaSanPham = sp.Ma 
        AND p.MA_DAI_DIEN != '000'";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query).ToList();
                return items;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public List<T> GetTongHopTinhLuongs<T>(DateTime fromDate, DateTime toDate,string xuongId, float gioCa)
        {
            try
            {
//                var query = @"	;WITH NhomTrongLuong AS (
//    SELECT 
//        n.Id AS NhomId,
//        p.MaThanhPham,
//        p.MaLoaiNguyenLieu,
//        COUNT(DISTINCT p.MaNhanVien) AS SoNhanVien, -- Chỉ đếm nhân viên có phiếu cân
//        SUM(p.TrongLuong) AS TongTrongLuong
//    FROM 
//        HQ_PhieuCan p
//        INNER JOIN HQ_NhanVienTheoNhom nvn ON p.MaNhanVien = nvn.MaNhanVien
//        INNER JOIN HQ_Nhom n ON n.Id = nvn.MaNhom
//    WHERE 
//        p.Ngay BETWEEN @fromDate AND @toDate
//        AND p.MaXuong = @xuongId
//        AND p.TrongLuong > 0
//    GROUP BY 
//        n.Id, p.MaThanhPham, p.MaLoaiNguyenLieu
//)
//SELECT 
//    p.MaNhanVien,
//    nv.MaHoSo,
//    nv.Name AS NhanVienName,
//    n.Id AS NhomId,
//    n.Ten AS NhomName,
//    nvc.CaId,
//    c.Ten AS CaName,
//    p.MaLoaiNguyenLieu,
//    lnl.Ten AS LoaiNguyenLieuName,
//    p.MaThanhPham,
//    tp.Ten AS ThanhPhamName,
//    sptl.Ten AS SanPhamTinhLuongName,
//    CASE 
//        WHEN n.Id = 000 THEN nt.TongTrongLuong -- Trọng lượng không chia cho nhân viên
//        ELSE 
//            CASE 
//                WHEN nt.SoNhanVien IS NULL OR nt.SoNhanVien = 0 THEN 0 -- Không có nhân viên nào có phiếu cân
//                ELSE nt.TongTrongLuong / nt.SoNhanVien -- Chia đều cho nhân viên có phiếu cân
//            END
//    END AS TrongLuong,
//    COUNT(p.Id) AS SoRo,
//    CASE 
//        WHEN CAST(MAX(p.NgayGio) AS TIME) > CAST(c.GioKetThuc AS TIME) THEN 1 
//        ELSE 0 
//    END AS TangCa,
//    dg.DonGia,
//    CASE 
//        WHEN n.Id = 000 THEN nt.TongTrongLuong
//        ELSE 
//            CASE 
//                WHEN nt.SoNhanVien IS NULL OR nt.SoNhanVien = 0 THEN 0
//                ELSE nt.TongTrongLuong / nt.SoNhanVien
//            END
//    END * dg.DonGia AS ThanhTien
//FROM 
//    HQ_PhieuCan p
//    INNER JOIN NhanVienDaiThanh nv ON nv.MaNhanVien = p.MaNhanVien
//    LEFT JOIN HQ_NhanVienTheoNhom nvn ON p.MaNhanVien = nvn.MaNhanVien
//    LEFT JOIN HQ_Nhom n ON n.Id = nvn.MaNhom
//    LEFT JOIN HQ_NhanVienTheoCa nvc ON nv.MaNhanVien = nvc.NhanVienId
//    LEFT JOIN HQ_Ca c ON nvc.CaId = c.Id
//    LEFT JOIN HQ_ThanhPham tp ON p.MaThanhPham = tp.Id
//    LEFT JOIN HQ_LoaiNguyenLieu lnl ON p.MaLoaiNguyenLieu = lnl.Id
//    LEFT JOIN HQ_MapSanPhamTinhLuong msptl ON msptl.MaThanhPham = tp.Id
//    LEFT JOIN DG_SanPhamTinhLuong sptl ON sptl.Ma = msptl.MaSanPham
//    LEFT JOIN DG_DonGia dg ON dg.MaSanPham = sptl.Ma
//    LEFT JOIN NhomTrongLuong nt ON n.Id = nt.NhomId 
//                                 AND p.MaThanhPham = nt.MaThanhPham 
//                                 AND p.MaLoaiNguyenLieu = nt.MaLoaiNguyenLieu
//WHERE 
//    p.Ngay BETWEEN @fromDate AND @toDate
//    AND p.MaXuong = @xuongId
//    AND p.TrongLuong > 0
//GROUP BY 
//    p.MaNhanVien, nv.MaHoSo, nv.Name, n.Id, n.Ten, nvc.CaId, c.Ten, p.MaLoaiNguyenLieu, 
//    lnl.Ten, p.MaThanhPham, tp.Ten, sptl.Ten, dg.DonGia, nt.TongTrongLuong, nt.SoNhanVien, p.TrongLuong, c.GioKetThuc
//ORDER BY 
//    n.Ten, nv.Name;

//";
//var query = @"	;WITH LatestNhanVienTheoCa AS (
//    SELECT 
//        nvc.NhanVienId, 
//        nvc.CaId,
//        ROW_NUMBER() OVER (PARTITION BY nvc.NhanVienId ORDER BY nvc.NgayGio DESC) AS RowNum
//    FROM HQ_NhanVienTheoCa nvc
//),
//LatestNhanVienTheoNhom AS (
//    SELECT 
//        nvn.Id, 
//        nvn.MaNhanVien,
//		nvn.MaNhom, 
//		nvn.NgayGioBatDau, 
//		nvn.HeSo, 
//        ROW_NUMBER() OVER (PARTITION BY nvn.MaNhanVien, nvn.MaNhom  ORDER BY nvn.NgayGioBatDau DESC) AS RowNum
//    FROM HQ_NhanVienTheoNhom nvn
//),
//LatestMapSanPhamTinhLuong AS (
//    SELECT 
//    p.Id,
//    p.MaSanPham,
//	dg.Ten as SanPhamName,
//    p.MaThanhPham,
//	tp.Ten as ThanhPhamName,
//    p.MaSize,
//    s.Ten as SizeName,
//    p.NgayGio,
//    ROW_NUMBER() OVER (
//        PARTITION BY p.MaThanhPham, p.MaSanPham , p.MaSize
//        ORDER BY p.NgayGio DESC
//    ) AS RowNum
//FROM 
//    HQ_MapSanPhamTinhLuong p,
//	DG_SanPhamTinhLuong dg,
//	HQ_ThanhPham tp,
//    HQ_Size s
//WHERE 
//    p.NgayGio <= @toDate
//	and p.MaSanPham = dg.Ma
//	and p.MaThanhPham = tp.Id
//    and p.MaSize = s.Id
//)
//--NhomTrongLuong AS (
//----declare @fromDate datetime, @toDate datetime,@xuongId varchar(5), @gioCa float
//----set @fromDate = '2025-01-16'
//----set @toDate = '2025-01-16'
//----set @xuongId = '1'
//----set @gioCa = 4.5
//--    SELECT 
//--        n.Id AS NhomId,
//--        p.MaThanhPham,
//--        p.MaLoaiNguyenLieu,
//--		p.MaSize,
//--        COUNT(DISTINCT p.MaNhanVien) AS SoNhanVien, -- Chỉ đếm nhân viên có phiếu cân
//--        SUM(p.TrongLuong) AS TongTrongLuong
//--    FROM 
//--        HQ_PhieuCan p
//--        INNER JOIN HQ_NhanVienTheoNhom nvn ON p.MaNhanVien = nvn.MaNhanVien
//--        INNER JOIN HQ_Nhom n ON n.Id = nvn.MaNhom
//--	WHERE 
//--(
//--    (DATEPART(HOUR, p.NgayGio) < @gioCa AND p.Ngay BETWEEN DATEADD(DAY, 1, @fromDate) AND DATEADD(DAY, 1, @toDate))
//--    OR
//--    (DATEPART(HOUR, p.NgayGio) >= @gioCa AND p.Ngay BETWEEN @fromDate AND @toDate)
//--)
//--AND p.MaXuong = @xuongId 
//--AND p.TrongLuong > 0
//--    GROUP BY 
//--        n.Id, p.MaThanhPham, p.MaLoaiNguyenLieu, p.MaSize
//--)
//select
//p.MaNhanVien,
//nv.Name as NhanVienName,
//nv.MaHoSo,
//nvn.MaNhom,
//n.Ten as NhomName,
//nvc.CaId,
//p.MaLoaiNguyenLieu,
//lnl.Ten as LoaiNguyenLieuName,
//p.MaThanhPham,
//tp.Ten as ThanhPhamName,
//sptl.Ma as MaSanPhamTinhLuong,
//sptl.Ten as TenSanPhamTinhLuong,
//--CASE 
//--     WHEN n.Id = 000 THEN ntl.TongTrongLuong
//--     ELSE 
//--         CASE 
//--                WHEN ntl.SoNhanVien IS NULL OR ntl.SoNhanVien = 0 THEN 0
//--                ELSE ntl.TongTrongLuong / ntl.SoNhanVien
//--         END
//--    END AS TrongLuong
//Sum(p.TrongLuong) as TrongLuong
//from HQ_PhieuCan p
//left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
//left join LatestNhanVienTheoNhom nvn on nvn.MaNhanVien = p.MaNhanVien
//LEFT join HQ_Nhom n on n.Id = nvn.MaNhom
//LEFT JOIN LatestNhanVienTheoCa nvc ON p.MaNhanVien = nvc.NhanVienId AND nvc.RowNum = 1 
//left join HQ_LoaiNguyenLieu lnl on p.MaLoaiNguyenLieu = lnl.Id
//left join HQ_ThanhPham tp on p.MaThanhPham = tp.Id
//left join LatestMapSanPhamTinhLuong msptl on msptl.MaThanhPham = p.MaThanhPham and msptl.RowNum = 1
//left join DG_SanPhamTinhLuong sptl on sptl.Ma = msptl.MaSanPham
//--left join NhomTrongLuong ntl on ntl.NhomId = n.Id and ntl.MaThanhPham = p.MaThanhPham and ntl.MaLoaiNguyenLieu = p.MaLoaiNguyenLieu and ntl.MaSize = p.MaSize
//WHERE 
//(
//    (DATEPART(HOUR, p.NgayGio) < @gioCa AND p.Ngay BETWEEN DATEADD(DAY, 1, @fromDate) AND DATEADD(DAY, 1, @toDate))
//    OR
//    (DATEPART(HOUR, p.NgayGio) >= @gioCa AND p.Ngay BETWEEN @fromDate AND @toDate)
//)
//AND p.MaXuong = @xuongId 
//AND p.TrongLuong > 0

//GROUP BY
//    p.MaNhanVien,
//    nv.Name,
//    nv.MaHoSo,
//    nvn.MaNhom,
//    n.Ten,
//    nvc.CaId,
//    p.MaLoaiNguyenLieu,
//    lnl.Ten,
//    p.MaThanhPham,
//    tp.Ten,
//    sptl.Ma,
//    sptl.Ten

//";

                //////////////////////////////////////////////////////////////ĐÂY LÀ CHO TÍNH THẺ NHÓM//////////////////////////////////////////
//var query = @"	;WITH LatestNhanVienTheoCa AS (
//    SELECT 
//        nvc.NhanVienId, 
//        nvc.CaId,
//        ROW_NUMBER() OVER (PARTITION BY nvc.NhanVienId ORDER BY nvc.NgayGio DESC) AS RowNum
//    FROM HQ_NhanVienTheoCa nvc
//),
//LatestNhanVienTheoNhom AS (
//    SELECT 
//        nvn.Id, 
//        nvn.MaNhanVien,
//		nvn.MaNhom, 
//		nvn.NgayGioBatDau, 
//		nvn.HeSo, 
//        ROW_NUMBER() OVER (PARTITION BY nvn.MaNhanVien, nvn.MaNhom  ORDER BY nvn.NgayGioBatDau DESC) AS RowNum
//    FROM HQ_NhanVienTheoNhom nvn
//),
//LatestMapSanPhamTinhLuong AS (
//    SELECT 
//    p.Id,
//    p.MaSanPham,
//	dg.Ten as SanPhamName,
//    p.MaThanhPham,
//	tp.Ten as ThanhPhamName,
//    p.MaSize,
//    s.Ten as SizeName,
//    p.MaLoaiNguyenLieu,
//    lnl.Ten as LoaiNguyenLieuName,
//    p.NgayGio,
//    ROW_NUMBER() OVER (
//        PARTITION BY p.MaThanhPham, p.MaSanPham , p.MaSize
//        ORDER BY p.NgayGio DESC
//    ) AS RowNum
//FROM 
//    HQ_MapSanPhamTinhLuong p,
//	DG_SanPhamTinhLuong dg,
//	HQ_ThanhPham tp,
//    HQ_Size s,
//    HQ_LoaiNguyenLieu lnl
//WHERE 
//    CAST(p.NgayGio as DATE) <= @toDate
//	and p.MaSanPham = dg.Ma
//	and p.MaThanhPham = tp.Id
//    and p.MaSize = s.Id
//    and p.MaLoaiNguyenLieu = lnl.Id
//),
//NhomTrongLuong AS (
//    SELECT 
//        n.Id AS NhomId,
//        p.MaThanhPham,
//        p.MaLoaiNguyenLieu,
//		p.MaSize,
//        COUNT(DISTINCT p.MaNhanVien) AS SoNhanVien, -- Chỉ đếm nhân viên có phiếu cân
//        SUM(p.TrongLuong) AS TongTrongLuong
//    FROM 
//        HQ_PhieuCan p
//        INNER JOIN HQ_NhanVienTheoNhom nvn ON p.MaNhanVien = nvn.MaNhanVien
//        INNER JOIN HQ_Nhom n ON n.Id = nvn.MaNhom
//	WHERE 
//(
//    (DATEPART(HOUR, p.NgayGio) < @gioCa AND p.Ngay BETWEEN DATEADD(DAY, 1, @fromDate) AND DATEADD(DAY, 1, @toDate))
//    OR
//    (DATEPART(HOUR, p.NgayGio) >= @gioCa AND p.Ngay BETWEEN @fromDate AND @toDate)
//)
//AND p.MaXuong = @xuongId 
//AND p.TrongLuong > 0
//    GROUP BY 
//        n.Id, p.MaThanhPham, p.MaLoaiNguyenLieu, p.MaSize
//)
//select
//p.MaNhanVien,
//nv.Name as NhanVienName,
//nv.MaHoSo,
//nvn.MaNhom,
//n.Ten as NhomName,
//nvc.CaId,
//c.Ten as CaName ,
//CASE 
//        WHEN CAST(MAX(p.NgayGio) AS TIME) > CAST(c.GioKetThuc AS TIME) THEN 1 
//        ELSE 0 
//END AS TangCa,
//--COUNT(p.Id) AS SoRo,
//p.MaLoaiNguyenLieu,
//lnl.Ten as LoaiNguyenLieuName,
//p.MaThanhPham,
//tp.Ten as ThanhPhamName,
//p.MaSize,
//s.Ten as SizeName,
//sptl.Ma as MaSanPhamTinhLuong,
//sptl.Ten as TenSanPhamTinhLuong,
//Sum(p.TrongLuong) as TrongLuong,
//dg.DonGia,
//Sum(p.TrongLuong) * dg.DonGia AS ThanhTien
//from HQ_PhieuCan p
//left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
//left join LatestNhanVienTheoNhom nvn on nvn.MaNhanVien = p.MaNhanVien
//LEFT join HQ_Nhom n on n.Id = nvn.MaNhom
//LEFT JOIN LatestNhanVienTheoCa nvc ON p.MaNhanVien = nvc.NhanVienId AND nvc.RowNum = 1 
//LEFT join HQ_Ca c on nvc.CaId = c.Id
//left join HQ_LoaiNguyenLieu lnl on p.MaLoaiNguyenLieu = lnl.Id
//left join HQ_ThanhPham tp on p.MaThanhPham = tp.Id
//left join LatestMapSanPhamTinhLuong msptl on msptl.MaThanhPham = p.MaThanhPham and msptl.MaSize = p.MaSize and msptl.MaLoaiNguyenLieu = p.MaLoaiNguyenLieu and msptl.RowNum = 1
//left join DG_SanPhamTinhLuong sptl on sptl.Ma = msptl.MaSanPham
//LEFT JOIN DG_DonGia dg ON sptl.Ma = dg.MaSanPham
//left join NhomTrongLuong ntl on ntl.NhomId = n.Id and ntl.MaThanhPham = p.MaThanhPham and ntl.MaLoaiNguyenLieu = p.MaLoaiNguyenLieu and ntl.MaSize = p.MaSize
//left join HQ_Size s on s.Id = p.MaSize
//WHERE 
//(
//    (DATEPART(HOUR, p.NgayGio) < @gioCa AND p.Ngay BETWEEN DATEADD(DAY, 1, @fromDate) AND DATEADD(DAY, 1, @toDate))
//    OR
//    (DATEPART(HOUR, p.NgayGio) >= @gioCa AND p.Ngay BETWEEN @fromDate AND @toDate)
//)
//AND p.MaXuong = @xuongId 
//AND p.TrongLuong > 0

//GROUP BY
//    p.MaNhanVien,
//    nv.Name,
//    nv.MaHoSo,
//    nvn.MaNhom,
//    n.Ten,
//    nvc.CaId,
//    p.MaLoaiNguyenLieu,
//    lnl.Ten,
//    p.MaThanhPham,
//    tp.Ten,
//    p.MaSize,
//    s.Ten,
//    sptl.Ma,
//    sptl.Ten,
//	c.GioKetThuc,
//	c.Ten,
//	dg.DonGia,
//	n.Id

//";
/////////////////////////////////////ĐÂY LÀ KHÔNG SỬ DỤNG THẺ NHÓM NHƯNG VẪN CHIA ĐƯỢC SL CHO TỪNG NHÂN VIÊN TRONG NHÓM/////////////////////////
var query = @"	;WITH LatestNhanVienTheoCa AS (
    SELECT 
        nvc.NhanVienId, 
        nvc.CaId,
        ROW_NUMBER() OVER (PARTITION BY nvc.NhanVienId ORDER BY nvc.NgayGio DESC) AS RowNum
    FROM HQ_NhanVienTheoCa nvc
),
LatestNhanVienTheoNhom AS (
    SELECT 
        nvn.Id, 
        nvn.MaNhanVien,
		nvn.MaNhom, 
		nvn.NgayGioBatDau, 
		nvn.HeSo, 
        ROW_NUMBER() OVER (PARTITION BY nvn.MaNhanVien, nvn.MaNhom  ORDER BY nvn.NgayGioBatDau DESC) AS RowNum
    FROM HQ_NhanVienTheoNhom nvn
	where cast( nvn.NgayGioBatDau as date) <= @toDate
),
LatestMapSanPhamTinhLuong AS (
    SELECT 
    p.Id,
    p.MaSanPham,
	dg.Ten as SanPhamName,
    p.MaThanhPham,
	tp.Ten as ThanhPhamName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaLoaiNguyenLieu,
    lnl.Ten as LoaiNguyenLieuName,
    p.NgayGio,
    ROW_NUMBER() OVER (
        PARTITION BY p.MaThanhPham, p.MaSanPham , p.MaSize
        ORDER BY p.NgayGio DESC
    ) AS RowNum
FROM 
    HQ_MapSanPhamTinhLuong p,
	DG_SanPhamTinhLuong dg,
	HQ_ThanhPham tp,
    HQ_Size s,
    HQ_LoaiNguyenLieu lnl
WHERE 
    CAST(p.NgayGio as DATE) <= @toDate
	and p.MaSanPham = dg.Ma
	and p.MaThanhPham = tp.Id
    and p.MaSize = s.Id
    and p.MaLoaiNguyenLieu = lnl.Id
),
NhomTrongLuong AS (
Select nvn.MaNhom,
    p.MaThanhPham,
    p.MaLoaiNguyenLieu,
    p.MaSize,SUM(p.TongTrongLuong) AS TongTrongLuong,sum(nvn.HeSo) as TongHeSo from LatestNhanVienTheoNhom nvn 
left join ( SELECT
p.MaNhanVien,
    p.MaThanhPham,
    p.MaLoaiNguyenLieu,
    p.MaSize,
	SUM(p.TrongLuong) AS TongTrongLuong
FROM 
    HQ_PhieuCan p
WHERE 
    (
        (DATEPART(HOUR, p.NgayGio) < @gioCa AND p.Ngay BETWEEN DATEADD(DAY, 1, @fromDate) AND DATEADD(DAY, 1, @toDate))
        OR
        (DATEPART(HOUR, p.NgayGio) >= @gioCa AND p.Ngay BETWEEN @fromDate AND @toDate)
    )
    AND p.MaXuong = @xuongId 
    AND p.TrongLuong > 0 group by p.MaNhanVien,
    p.MaThanhPham,
    p.MaLoaiNguyenLieu,
    p.MaSize) p
	on nvn.MaNhanVien = p.MaNhanVien
	group by
	 nvn.MaNhom,
    p.MaThanhPham,
    p.MaLoaiNguyenLieu,
    p.MaSize
)
select
p.MaNhanVien,
nv.Name as NhanVienName,
nv.MaHoSo,
nvn.MaNhom,
n.Ten as NhomName,
nvc.CaId,
c.Ten as CaName ,
CASE 
        WHEN CAST(MAX(p.NgayGio) AS TIME) > CAST(c.GioKetThuc AS TIME) THEN 1 
        ELSE 0 
END AS TangCa,
--COUNT(p.Id) AS SoRo,
p.MaLoaiNguyenLieu,
lnl.Ten as LoaiNguyenLieuName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
sptl.Ma as MaSanPhamTinhLuong,
sptl.Ten as TenSanPhamTinhLuong,

CASE 
    WHEN nvn.MaNhom is null or nvn.MaNhom = '000' THEN ntl.TongTrongLuong
    ELSE (ntl.TongTrongLuong / ntl.TongHeSo) * nvn.HeSo  
END AS TrongLuong,

dg.DonGia,
(
    CASE 
        WHEN nvn.MaNhom IS NULL OR nvn.MaNhom = '000' THEN ntl.TongTrongLuong
        ELSE (ntl.TongTrongLuong / ntl.TongHeSo) * nvn.HeSo  
    END
) * dg.DonGia AS ThanhTien
from HQ_PhieuCan p
left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
left join LatestNhanVienTheoNhom nvn on nvn.MaNhanVien = p.MaNhanVien and nvn.RowNum =1
LEFT join HQ_Nhom n on n.Id = nvn.MaNhom
LEFT JOIN LatestNhanVienTheoCa nvc ON p.MaNhanVien = nvc.NhanVienId AND nvc.RowNum = 1 
LEFT join HQ_Ca c on nvc.CaId = c.Id
left join HQ_LoaiNguyenLieu lnl on p.MaLoaiNguyenLieu = lnl.Id
left join HQ_ThanhPham tp on p.MaThanhPham = tp.Id
left join LatestMapSanPhamTinhLuong msptl on msptl.MaThanhPham = p.MaThanhPham and msptl.MaSize = p.MaSize and msptl.MaLoaiNguyenLieu = p.MaLoaiNguyenLieu and msptl.RowNum = 1
left join DG_SanPhamTinhLuong sptl on sptl.Ma = msptl.MaSanPham
LEFT JOIN DG_DonGia dg ON sptl.Ma = dg.MaSanPham
left join NhomTrongLuong ntl on ntl.MaNhom = n.Id and ntl.MaThanhPham = p.MaThanhPham and ntl.MaLoaiNguyenLieu = p.MaLoaiNguyenLieu and ntl.MaSize = p.MaSize
--left join HQ_Nhom n on n.Id = ntl.NhomId
left join HQ_Size s on s.Id = p.MaSize
WHERE 
(
    (DATEPART(HOUR, p.NgayGio) < @gioCa AND p.Ngay BETWEEN DATEADD(DAY, 1, @fromDate) AND DATEADD(DAY, 1, @toDate))
    OR
    (DATEPART(HOUR, p.NgayGio) >= @gioCa AND p.Ngay BETWEEN @fromDate AND @toDate)
)
AND p.MaXuong = @xuongId 
AND p.TrongLuong > 0

GROUP BY
    p.MaNhanVien,
    nv.Name,
    nv.MaHoSo,
    nvn.MaNhom,
    n.Ten,
    nvc.CaId,
    p.MaLoaiNguyenLieu,
    lnl.Ten,
    p.MaThanhPham,
    tp.Ten,
    p.MaSize,
    s.Ten,
    sptl.Ma,
    sptl.Ten,
	c.GioKetThuc,
	c.Ten,
	dg.DonGia,
	n.Id,
	ntl.TongTrongLuong,
	ntl.TongHeSo,
	--ntl.HeSo,
	nvn.HeSo
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date ,xuongId, gioCa= gioCa}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetTongHopThanhPhams<T>(DateTime fromDate, DateTime toDate,string xuongId)
        {
            try
            {
                
                var query = @"
    	select
p.Ngay,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLoaiNguyenLieu,
lnl.Ten as LoaiNguyenLieuName,
COUNT(*) as SoRo,
Sum(p.TrongLuong) as TrongLuong
from 
HQ_PhieuCan p
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join HQ_Size s on p.MaSize = s.Id
left join HQ_ThanhPham tp on p.MaThanhPham = tp.Id
left join HQ_LoaiNguyenLieu lnl on p.MaLoaiNguyenLieu = lnl.Id
where
p.Ngay BETWEEN @fromDate AND @toDate and p.MaXuong = @xuongId and p.TrongLuong > 0
group by
p.Ngay,
p.MaLo,
p.MaSize,
s.Ten,
p.MaThanhPham,
tp.Ten,
p.MaLoaiNguyenLieu,
lnl.Ten

";

                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date , toDate = fromDate.Date,xuongId}).Result.ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamDashboards<T>(DateTime fromDate, DateTime toDate,string xuongId)
        {
            try
            {
               
                var query = @"
      select
p.Ngay,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
COUNT(*) as SoRo,
Sum(p.TrongLuong) as TrongLuong
from 
HQ_PhieuCan p
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join HQ_ThanhPham tp on p.MaThanhPham = tp.Id
where
p.Ngay BETWEEN @fromDate AND @toDate and p.MaXuong = @xuongId and p.TrongLuong > 0
group by
p.Ngay,
p.MaThanhPham,
tp.Ten

";

                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date,xuongId}).Result.ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThongKeSanXuats<T>(DateTime date,string xuongId, float gioCa)
        {
            try
            {
//                var query = @"	;WITH CTE AS (
//    SELECT *,
//		CASE 
//			WHEN DATEPART(HOUR, p.NgayGio) < 5 THEN DATEADD(DAY, -1, p.Ngay)
//			ELSE p.Ngay
//		END AS AdjustedNgay,
//        CASE
//            WHEN LAG(CONVERT(VARCHAR(8), p.NgayGio, 108)) OVER (PARTITION BY p.MaNhanVien ORDER BY p.NgayGio) IS NULL THEN CONVERT(VARCHAR(8), p.NgayGio, 108)
//            ELSE LAG(CONVERT(VARCHAR(8), p.NgayGio, 108)) OVER (PARTITION BY p.MaNhanVien ORDER BY CONVERT(VARCHAR(8), p.NgayGio, 108))
//        END AS PreviousGioLam,

//        ROW_NUMBER() OVER (PARTITION BY p.MaNhanVien ORDER BY CONVERT(VARCHAR(8), p.NgayGio, 108)) - ROW_NUMBER() OVER (PARTITION BY p.MaNhanVien, p.MaThanhPham ORDER BY CONVERT(VARCHAR(8), p.NgayGio, 108)) AS GroupKey
//    FROM HQ_PhieuCan p
	
//     WHERE (DATEPART(HOUR, p.NgayGio) < @gioCa and p.Ngay =  DATEADD(DAY, 1, @date ))  or (DATEPART(HOUR, p.NgayGio) >= @gioCa and p.Ngay =  DATEADD(DAY, 0, @date )) -- Mở rộng khoảng thời gian để tính đúng
//   AND p.MaXuong = @xuongId 
//   AND p.TrongLuong > 0
//)
//SELECT 
//    p.Ngay,
//    c.Id AS CaId,
//    c.Ten AS CaName,
//    p.MaNhanVien,
//    nv.MaHoSo,
//    nv.Name AS NhanVienName,
//    n.Id AS MaNhom,
//    n.Ten AS NhomName,
//    p.MaThanhPham,
//    tp.Ten AS ThanhPhamName,
//	Sum(p.TrongLuong) as TrongLuong,
//    MIN(PreviousGioLam) AS GioBatDau,
//    MAX(CONVERT(VARCHAR(8), p.NgayGio, 108)) AS GioKetThuc
//FROM CTE p
//LEFT JOIN NhanVienDaiThanh nv ON nv.MaNhanVien = p.MaNhanVien
//LEFT JOIN HQ_NhanVienTheoCa nvc ON nv.MaNhanVien = nvc.NhanVienId
//LEFT JOIN HQ_Ca c ON nvc.CaId = c.Id
//LEFT JOIN HQ_NhanVienTheoNhom nvn ON p.MaNhanVien = nvn.MaNhanVien
//LEFT JOIN HQ_Nhom n ON n.Id = nvn.MaNhom
//LEFT JOIN HQ_ThanhPham tp ON p.MaThanhPham = tp.Id
//GROUP BY 
//    p.Ngay,
//    c.Id,
//    c.Ten,
//    p.MaNhanVien,
//    nv.MaHoSo,
//    nv.Name,
//    n.Id,
//    n.Ten,
//    p.MaThanhPham,
//    tp.Ten,
//    GroupKey
//ORDER BY 
//    p.MaNhanVien,
//    p.Ngay,
//    GioBatDau,
//    p.MaThanhPham;

//";
 var query = @"
    	;WITH CTE AS (
    SELECT *,
        CASE 
            WHEN DATEPART(HOUR, p.NgayGio) < 5 THEN DATEADD(DAY, -1, p.Ngay)
            ELSE p.Ngay
        END AS AdjustedNgay,
        CASE
            WHEN LAG(CONVERT(VARCHAR(8), p.NgayGio, 108)) OVER (PARTITION BY p.MaNhanVien ORDER BY p.NgayGio) IS NULL THEN CONVERT(VARCHAR(8), p.NgayGio, 108)
            ELSE LAG(CONVERT(VARCHAR(8), p.NgayGio, 108)) OVER (PARTITION BY p.MaNhanVien ORDER BY CONVERT(VARCHAR(8), p.NgayGio, 108))
        END AS PreviousGioLam
    FROM HQ_PhieuCan p
    WHERE (DATEPART(HOUR, p.NgayGio) < @gioCa AND p.Ngay =  DATEADD(DAY, 1, @date))  
          OR (DATEPART(HOUR, p.NgayGio) >= @gioCa AND p.Ngay =  DATEADD(DAY, 0, @date)) 
          AND p.MaXuong = @xuongId 
          AND p.TrongLuong > 0
),
LatestNhanVienTheoCa AS (
    SELECT 
        nvc.NhanVienId, 
        nvc.CaId,
        ROW_NUMBER() OVER (PARTITION BY nvc.NhanVienId ORDER BY nvc.NgayGio DESC) AS RowNum
    FROM HQ_NhanVienTheoCa nvc
),
LatestMapSanPhamTinhLuong AS (
    SELECT 
    p.Id,
    p.MaSanPham,
	dg.Ten as SanPhamName,
    p.MaThanhPham,
	tp.Ten as ThanhPhamName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaLoaiNguyenLieu,
    lnl.Ten as LoaiNguyenLieuName,
    p.NgayGio,
    ROW_NUMBER() OVER (
        PARTITION BY p.MaThanhPham, p.MaSanPham , p.MaSize , p.MaLoaiNguyenLieu
        ORDER BY p.NgayGio DESC
    ) AS RowNum
	FROM 
    HQ_MapSanPhamTinhLuong p,
	DG_SanPhamTinhLuong dg,
	HQ_ThanhPham tp,
    HQ_Size s,
    HQ_LoaiNguyenLieu lnl
WHERE 
    CAST(p.NgayGio as DATE) <= @date
	and p.MaSanPham = dg.Ma
	and p.MaThanhPham = tp.Id
    and p.MaSize = s.Id
    and p.MaLoaiNguyenLieu = lnl.Id
)
SELECT 
    p.Ngay,
    c.Id AS CaId,
    c.Ten AS CaName,
    p.MaNhanVien,
    nv.MaHoSo,
    nv.Name AS NhanVienName,
    n.Id AS MaNhom,
    n.Ten AS NhomName,
    p.MaThanhPham,
    tp.Ten AS ThanhPhamName,
	sptl.Ma as MaSanPhamTinhLuong,
	sptl.Ten as SanPhamTinhLuongName,
    SUM(p.TrongLuong) AS TrongLuong,
    MIN(PreviousGioLam) AS GioBatDau,
    MAX(CONVERT(VARCHAR(8), p.NgayGio, 108)) AS GioKetThuc
FROM CTE p
OUTER APPLY (
    SELECT TOP 1 nvn.*
    FROM HQ_NhanVienTheoNhom nvn
    WHERE nvn.MaNhanVien = p.MaNhanVien
      AND nvn.NgayGioBatDau <= p.NgayGio
    ORDER BY nvn.NgayGioBatDau DESC
) nvn
LEFT JOIN HQ_Nhom n ON n.Id = nvn.MaNhom
LEFT JOIN HQ_ThanhPham tp ON p.MaThanhPham = tp.Id
LEFT JOIN NhanVienDaiThanh nv ON p.MaNhanVien = nv.MaNhanVien
LEFT JOIN LatestNhanVienTheoCa nvc ON p.MaNhanVien = nvc.NhanVienId AND nvc.RowNum = 1 
LEFT JOIN HQ_Ca c ON nvc.CaId = c.Id
left join LatestMapSanPhamTinhLuong msptl on msptl.MaThanhPham = p.MaThanhPham and msptl.MaSize = p.MaSize and msptl.MaLoaiNguyenLieu = p.MaLoaiNguyenLieu and msptl.RowNum = 1
left join DG_SanPhamTinhLuong sptl on sptl.Ma = msptl.MaSanPham
GROUP BY 
    p.Ngay,
    c.Id,
    c.Ten,
    p.MaNhanVien,
    nv.MaHoSo,
    nv.Name,
    n.Id,
    n.Ten,
    p.MaThanhPham,
    tp.Ten,
	sptl.Ma,
	sptl.Ten
ORDER BY 
    p.MaNhanVien,
    p.Ngay,
    GioBatDau,
    p.MaThanhPham;
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { date = date.Date ,xuongId, gioCa = gioCa}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanDeleteXLPCs<T>( DateTime dateTime,string xuongId)
        {
            try
            {
                var query = @"
Select p.Id,
p.STT,
p.Ngay,
p.NgayGio,
p.MayCan,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLoaiNguyenLieu,
lnl.Ten as LoaiNguyenLieuName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.TrongLuong,
p.TrongLuongTare,
p.ChiSanLuong,
p.Status,
p.TheId,
p.TheIdNhanVien,
p.GhiChu
from HQ_PhieuCan_D p,
	HQ_Size s,
	HQ_ThanhPham tp,
	HQ_LoaiNguyenLieu lnl,
	NhanVienDaiThanh nv

where p.Ngay = @dateTime and p.MaXuong = @xuongId
	and p.MaSize = s.Id
	and p.MaThanhPham = tp.Id
	and p.MaLoaiNguyenLieu = lnl.Id
	and p.MaNhanVien = nv.MaNhanVien
	order by
	p.STT desc;
	
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { dateTime = dateTime.Date,xuongId}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanUpdateXLPCs<T>( DateTime dateTime,string xuongId)
        {
            try
            {
                var query = @"select
p.Id,
p.STT,
p.Ngay,
p.NgayGio,
p.MayCan,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLoaiNguyenLieu,
lnl.Ten as LoaiNguyenLieuName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.TrongLuong,
p.TrongLuongTare,
p.ChiSanLuong,
p.Status,
p.TheId,
p.TheIdNhanVien,
p.GhiChu
from HQ_PhieuCan_U p,
	HQ_Size s,
	HQ_ThanhPham tp,
	HQ_LoaiNguyenLieu lnl,
	NhanVienDaiThanh nv
where p.Ngay = @dateTime and p.MaXuong = @xuongId
	and p.MaSize = s.Id
	and p.MaThanhPham = tp.Id
	and p.MaLoaiNguyenLieu = lnl.Id
	and p.MaNhanVien = nv.MaNhanVien
	order by
	p.STT desc
	
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { dateTime = dateTime.Date,xuongId}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public Tuple<int, decimal> GetTongSoRoTongTrongLuong(DateTime dateTime, string nhanVienId)
        {
            try
            {
                var query =
                    "Select Count(*) as Item1,ISNULL(Sum(TrongLuong) ,0) As Item2 from HQ_PhieuCan where ChiSanLuong = 0 and MaNhanVien = @nhanVienId and Ngay =@ngay and ISNULL(GhiChu,'') <> 'HUY'";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.Query<Tuple<int, decimal>>(query, new { ngay = dateTime.Date, nhanVienId })
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Tuple<int, decimal, int, decimal> GetTongSoRoTongTrongLuong(
            DateTime dateTime,
            string nhanVienId,
            string thanhPhamId)
        {
            try
            {
                var query = @"Select
    Count(*) as Item1,
    ISNULL(Sum(TrongLuong), 0) As Item2,
    ISNULL(
        Sum(
            case
                when MaThanhPham = @thanhPhamId then 1
                else 0
            end
        ),
        0
    )  as Item3,
    ISNULL(
        Sum(
            case
                when MaThanhPham = @thanhPhamId then TrongLuong
                else 0
            end
        ),
        0
    ) as Item4
from
    HQ_PhieuCan WITH(READPAST)
where
    ChiSanLuong = 0
    and MaNhanVien = @nhanVienId
    and Ngay = @ngay
    and ISNULL(GhiChu, '') <> 'HUY'";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.Query<Tuple<int, decimal, int, decimal>>(
                            query,
                            new { ngay = dateTime.Date, nhanVienId, thanhPhamId })
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Tuple<int, decimal, int, decimal> GetTongSoRoTongTrongLuong_laychiSanLuong(
            DateTime dateTime,
            string nhanVienId,
            string thanhPhamId)
        {
            try
            {
                var query = @"Select
    Count(*) as Item1,
    ISNULL(Sum(TrongLuong), 0) As Item2,
    ISNULL(
        Sum(
            case
                when MaThanhPham = @thanhPhamId then 1
                else 0
            end
        ),
        0
    )  as Item3,
    ISNULL(
        Sum(
            case
                when MaThanhPham = @thanhPhamId then TrongLuong
                else 0
            end
        ),
        0
    ) as Item4
from
    HQ_PhieuCan WITH(READPAST)
where MaNhanVien = @nhanVienId
    and Ngay = @ngay
    and ISNULL(GhiChu, '') <> 'HUY'";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.Query<Tuple<int, decimal, int, decimal>>(
                            query,
                            new { ngay = dateTime.Date, nhanVienId, thanhPhamId })
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
	
        public List<T> GetChiTiets_TG<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"SELECT
    p.Ngay,
    p.STT,
    p.Gio,
    p.TrongLuong + p.TrongLuongTare as TrongLuongCan,
    p.TrongLuongTare,
    p.TrongLuong as TrongLuong,
    tp.Ten as MaSanPham,
    p.MaNhanVien,
    n.MaHoSo as MaHoSoPhucVu,
    p.MaLo,
    p.MaXuong,
    p.MaMayCan,
    0 as DonGia,
    0 * p.TrongLuong as ThanhTien,
    p.MaHoSo
from
    (
        Select
            p.*,
            nv.MaHoSo
        from
            HQ_PhieuCan p,
            NhanVienDaiThanh nv
        where
            p.Ngay <= @ngay and p.Ngay>=@fromDate
            and p.MaNhanVien = nv.MaNhanVien
    ) p
    left join MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
    left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
order by
    p.STT";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public int Insert<T>(T item)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(qrInsert, item);  
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Update<T>(T item)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(qrUpdate, item);
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Delete<T>(T item) {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(qrDelete, item);
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
