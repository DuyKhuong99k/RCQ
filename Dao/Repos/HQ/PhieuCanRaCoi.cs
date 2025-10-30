using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public class PhieuCanRaCoi
    {
        private readonly string connectionString;
        private string tableName = "PhieuCanRaCoi";
        private readonly string qrInsert = @"
INSERT INTO [dbo].[PhieuCanRaCoi]
           ([Id]
           ,[IdMonitor]
           ,[Ngay]
           ,[Gio]
           ,[MaXuong]
           ,[MayCan]
           ,[NgayNguyenLieu]
           ,[TrongLuong]
           ,[TrongLuongTare]
           ,[MaLo]
           ,[MaThanhPham]
           ,[MaSize]
           ,[MaChieuXa]
           ,[MaChatLuong]
           ,[MaNhanVien]
           ,[MaCoi]
           ,[MaThe]
           ,[GhiChu],[STT],[ThamSoTangTrong])
     VALUES
           (@Id
           ,@IdMonitor
           ,@Ngay
           ,@Gio
           ,@MaXuong
           ,@MayCan
           ,@NgayNguyenLieu
           ,@TrongLuong
           ,@TrongLuongTare
           ,@MaLo
           ,@MaThanhPham
           ,@MaSize
           ,@MaChieuXa
           ,@MaChatLuong
           ,@MaNhanVien
           ,@MaCoi
           ,@MaThe
           ,@GhiChu,@STT,@ThamSoTangTrong)
";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanRaCoi]
   SET [IdMonitor] = @IdMonitor
      ,[Ngay] = @Ngay
      ,[Gio] = @Gio
      ,[MaXuong] =@MaXuong
      ,[MayCan] = @MayCan
      ,[NgayNguyenLieu] = @NgayNguyenLieu
      ,[TrongLuong] = @TrongLuong
      ,[TrongLuongTare] = @TrongLuongTare
      ,[MaLo] = @MaLo
      ,[MaThanhPham] = @MaThanhPham
      ,[MaSize] = @MaSize
      ,[MaChieuXa] = @MaChieuXa
      ,[MaChatLuong] = @MaChatLuong
      ,[MaNhanVien] = @MaNhanVien
      ,[MaCoi] = @MaCoi
      ,[MaThe] = @MaThe
      ,[GhiChu] = @GhiChu,[STT] = @STT,[ThamSoTangTrong] = @ThamSoTangTrong
 WHERE [Id] = @Id

";

        private string qrDelete = @"
DELETE FROM [dbo].[PhieuCanRaCoi]
      WHERE Id =@Id
";
        private readonly string qrGetAll = "Select * from PhieuCanRaCoi";
        private readonly string qrGetByDate = "Select * from PhieuCanRaCoi where Ngay = @Ngay";
        private readonly string qrGetByDateRange = "Select * from PhieuCanRaCoi where Ngay >= @NgayStart and Ngay <= @NgayEnd";
        private readonly string qrGetByNgayXuong = "Select * from PhieuCanRaCoi where Ngay = @Ngay and MaXuong =@MaXuong";
        private readonly string qrGetsLastByNumAndMayCan = @"WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MayCan ORDER BY Ngay DESC, Gio DESC) AS RowNum
    FROM PhieuCanRaCoi where Ngay =@ngay
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
        public T? Get<T>(string id)
        {
            try
            {
                var query = "Select * from PhieuCanRaCoi Where Id=@id";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.QueryFirstOrDefault<T>(query, new { id });
                    return item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public PhieuCanRaCoi(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

        }
        public int Delete<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrDelete, item);
            return rows;
        }
        public int Insert<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrInsert, item);
            return rows;
        }
        public int Update<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdate, item);
            return rows;
        }
        public List<T> Gets<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetAll).ToList();
            return rows;
        }
        public List<T> Gets<T>(DateTime date)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetByDate, new { Ngay = date.Date }).ToList();
            return rows;
        }
        public List<T> Gets<T>(DateTime dateStart, DateTime dateEnd)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetByDateRange, new { NgayStart = dateStart.Date, NgayEnd = dateEnd.Date }).ToList();
            return rows;
        }
        public List<T> Gets<T>(DateTime date, string maXuong)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetByNgayXuong, new { Ngay = date.Date, MaXuong = maXuong }).ToList();
            return rows;
        }
        public List<T> GetChiTietRaCois<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"select
p.STT,
p.Ngay,
p.NgayNguyenLieu,
p.Gio,
p.MaNhanVien,
nv.Name as NhanVienName,
nv.MaHoSo,
p.MaLo,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
cx.Ten as ChieuXaName,
p.MaCoi,
c.Ten as CoiName,
p.MaSize,
s.Ten as SizeName,
p.TrongLuong,
p.TrongLuongTare
from PhieuCanRaCoi p
left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPham
left join MaChatLuongXepKhuon cl on cl.Ma = p.MaChatLuong
left join MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
left join MaCoiXepKhuon c on c.Ma = p.MaCoi
left join MaSizeChinhXepKhuon s on s.Ma = p.MaSize
left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
where
p.NgayNguyenLieu <= @dateTime
and p.NgayNguyenLieu >= @fromDate
and p.MaXuong = @xuongId
and p.TrongLuong > 0 
and p.MaNhanVien is not null
order by
p.STT desc
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { dateTime = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopCoiRaCois<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"select
p.NgayNguyenLieu,
p.MaCoi,
c.Ten as CoiName,
p.MaLo,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
cx.Ten as ChieuXaName,
p.MaSize,
s.Ten as SizeName,
Sum(p.TrongLuong) as TrongLuong
from PhieuCanRaCoi p
left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPham
left join MaChatLuongXepKhuon cl on cl.Ma = p.MaChatLuong
left join MaCoiXepKhuon c on c.Ma = p.MaCoi
left join MaSizeChinhXepKhuon s on s.Ma = p.MaSize
left join MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
where
p.NgayNguyenLieu <= @dateTime
and p.NgayNguyenLieu >= @fromDate
and p.MaXuong = @xuongId
and p.TrongLuong > 0 
and p.MaNhanVien is not null
group by
p.macoi,
c.Ten,
p.MaLo,
p.MaThanhPham,
tp.Ten,
p.MaChatLuong,
cl.Ten,
p.MaSize,
s.Ten,
cx.Ten,
p.NgayNguyenLieu
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { dateTime = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopNhanVienRaCois<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @";WITH CheckInOutData AS (
    SELECT 
        c.MaChamCong,
        MIN(c.ThoiGian) AS ThoiGianVao,
        MAX(c.ThoiGian) AS ThoiGianRa
    FROM CheckInOut c 
    WHERE c.ThoiGian >= @fromDate AND c.ThoiGian <= @dateTime
    GROUP BY c.MaChamCong
)
select
p.NgayNguyenLieu,
p.MaNhanVien,
nv.Name as NhanVienName,
nv.MaHoSo,
p.MaLo,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.MaCoi,
c.Ten as CoiName,
p.MaSize,
s.Ten as SizeName,
Sum(p.TrongLuong) as TrongLuong,
ISNULL(ch.ThoiGianVao, '1900-01-01') AS ThoiGianVao,
ISNULL(ch.ThoiGianRa, '1900-01-01') AS ThoiGianRa,
DATEDIFF(hour, ISNULL(ch.ThoiGianVao, '1900-01-01'), ISNULL(ch.ThoiGianRa, '1900-01-01')) AS TongThoiGian
from PhieuCanRaCoi p
left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPham
left join MaChatLuongXepKhuon cl on cl.Ma = p.MaChatLuong
left join MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
left join MaCoiXepKhuon c on c.Ma = p.MaCoi
left join MaSizeChinhXepKhuon s on s.Ma = p.MaSize
left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
left join CheckInOutData ch on nv.MaChamCong = ch.MaChamCong
where
p.NgayNguyenLieu <= @dateTime
and p.NgayNguyenLieu >= @fromDate
and p.MaXuong = @xuongId
and p.TrongLuong > 0 
and p.MaNhanVien is not null
group by
p.MaNhanVien,
nv.Name,
nv.MaHoSo,
p.macoi,
c.Ten,
p.MaLo,
p.MaThanhPham,
tp.Ten,
p.MaChatLuong,
cl.Ten,
p.MaSize,
s.Ten,
ch.ThoiGianRa,
ch.ThoiGianVao,
p.MaChieuXa,
cx.Ten,
p.NgayNguyenLieu
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { dateTime = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamRaCois<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"select
p.NgayNguyenLieu,
p.MaXuong,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.MaSize,
s.Ten as SizeName,
Sum(p.TrongLuong) as TrongLuong
from PhieuCanRaCoi p
left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPham
left join MaChatLuongXepKhuon cl on cl.Ma = p.MaChatLuong
left join MaSizeChinhXepKhuon s on s.Ma = p.MaSize
left join MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
where
p.NgayNguyenLieu <= @dateTime
and p.NgayNguyenLieu >= @fromDate
and p.MaXuong = @xuongId
and p.TrongLuong > 0 
and p.MaNhanVien is not null
group by
p.MaLo,
p.MaThanhPham,
tp.Ten,
p.MaChatLuong,
cl.Ten,
p.MaSize,
s.Ten,
p.MaXuong,
p.MaChieuXa,
cx.Ten,
p.NgayNguyenLieu
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { dateTime = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopTyLeTangTrongCoiRaCois<T>(DateTime ngayNguyenLieu)
        {
            try
            {
                //                var query = @";WITH Sorted AS (
                //    SELECT *,
                //           LAG(gio) OVER (PARTITION BY MaCoi ORDER BY gio) AS gio_truoc
                //    FROM PhieuCanRaCoi where NgayNguyenLieu = @ngayNguyenLieu
                //)

                //,
                //TimeDiff AS (
                //    SELECT *,
                //           CASE 
                //               WHEN gio_truoc IS NULL THEN 0
                //               WHEN DATEDIFF(MINUTE, gio_truoc, gio) > 60 THEN 1
                //               ELSE 0
                //           END AS is_new_group
                //    FROM Sorted
                //)
                //,
                //GroupNumbered AS (
                //    SELECT *,
                //           SUM(is_new_group) OVER (PARTITION BY MaCoi ORDER BY gio ROWS UNBOUNDED PRECEDING) AS luot
                //    FROM TimeDiff
                //)
                //,
                //Grouped as (
                //SELECT 
                //    p.MaCoi,
                //	p.NgayNguyenLieu,
                //	p.MaThanhPham,
                //	tp.Ten as ThanhPhamName,
                //	p.MaSize,
                //	s.Ten as SizeName,
                //    p.luot +1 AS luot,
                //    MIN(p.gio) AS gio_bat_dau,
                //    MAX(p.gio) AS gio_ket_thuc,
                //    COUNT(*) AS so_phieu,
                //	tlc.TrongLuong as trong_luong_vao_coi,
                //    SUM(p.trongluong) AS tong_trong_luong_ra_coi
                //FROM GroupNumbered p
                //left join MaSizeChinhXepKhuon s on p.MaSize = s.Ma
                //left join MaThanhPhamChinhXepKhuon tp on MaThanhPham = tp.Ma
                //left join TrongLuongCoiTheoSanPham tlc on p.MaCoi = tlc.MaCoi and p.MaThanhPham = tlc.MaSanPham
                //GROUP BY p.MaCoi, p.luot, p.NgayNguyenLieu, p.MaThanhPham , tp.Ten , tlc.TrongLuong, p.MaSize, s.Ten
                //)

                //SELECT 
                //    g.MaCoi,
                //    g.NgayNguyenLieu,
                //    g.MaThanhPham,
                //    g.ThanhPhamName,
                //	g.MaSize,
                //	g.SizeName,
                //    g.luot,
                //    g.gio_bat_dau,
                //    g.gio_ket_thuc,
                //    g.so_phieu,
                //    ISNULL(ck.TrongLuong, g.trong_luong_vao_coi) AS trong_luong_vao_coi,
                //    g.tong_trong_luong_ra_coi,
                //    (g.tong_trong_luong_ra_coi / ISNULL(ck.TrongLuong, g.trong_luong_vao_coi)) / @chiSo AS TyLeTangTrong
                //FROM Grouped g
                //OUTER APPLY (
                //    SELECT TOP 1 c.TrongLuong
                //    FROM CoiLeXepKhuon c
                //    WHERE c.MaCoi = g.MaCoi
                //      AND c.MaSanPham = g.MaThanhPham
                //      AND c.NgayGio BETWEEN 
                //            CAST(g.NgayNguyenLieu AS datetime) + CAST(g.gio_bat_dau AS datetime) AND 
                //            CAST(g.NgayNguyenLieu AS datetime) + CAST(g.gio_ket_thuc AS datetime)
                //    ORDER BY c.NgayGio
                //) ck
                //ORDER BY g.MaCoi, g.luot;
                //";

                //bỏ chỉ số mặc định
//                var query = @"
//;WITH SortedRa AS (
//    SELECT *, LAG(gio) OVER (PARTITION BY MaCoi ORDER BY gio) AS gio_truoc
//    FROM PhieuCanRaCoi
//    WHERE NgayNguyenLieu = @ngayNguyenLieu
//),
//TimeDiffRa AS (
//    SELECT *,
//        CASE
//            WHEN gio_truoc IS NULL THEN 0
//            WHEN DATEDIFF(MINUTE, gio_truoc, gio) > 60 THEN 1
//            ELSE 0
//        END AS is_new_group
//    FROM SortedRa
//),
//GroupNumberedRa AS (
//    SELECT *,
//        SUM(is_new_group) OVER (PARTITION BY MaCoi ORDER BY gio ROWS UNBOUNDED PRECEDING) + 1 AS luot
//    FROM TimeDiffRa
//),
//GroupedRa AS (
//    SELECT
//        p.MaCoi,
//        p.Ngay,
//        p.NgayNguyenLieu,
//        p.MaThanhPham,
//        tp.Ten AS ThanhPhamName,
//        tp.ThamSoTangTrong,
//        tp.DinhMucTangTrong,
//        p.MaSize,
//        s.Ten AS SizeName,
//        p.luot,
//        MIN(p.gio) AS gio_bat_dau,
//        MAX(p.gio) AS gio_ket_thuc,
//        COUNT(*) AS so_phieu,
//        SUM(p.trongluong) AS tong_trong_luong_ra_coi
//		--tp.DinhMucTangTrong
//    FROM GroupNumberedRa p
//    LEFT JOIN MaSizeChinhXepKhuon s ON p.MaSize = s.Ma
//    LEFT JOIN MaThanhPhamChinhXepKhuon tp ON p.MaThanhPham = tp.Ma
//    GROUP BY
//        p.MaCoi, p.luot, p.NgayNguyenLieu, p.MaThanhPham, tp.Ten,
//        tp.ThamSoTangTrong, p.MaSize, s.Ten, tp.DinhMucTangTrong, p.Ngay-- tp.DinhMucTangTrong
//),

//SortedVao AS (
//    SELECT *, LAG(Gio) OVER (PARTITION BY MaCoiChinh ORDER BY Gio) AS gio_truoc
//    FROM PhieuCanChinhXepKhuon
//    WHERE NgayNguyenLieu = @ngayNguyenLieu
//),
//TimeDiffVao AS (
//    SELECT *,
//        CASE
//            WHEN gio_truoc IS NULL THEN 0
//            WHEN DATEDIFF(MINUTE, gio_truoc, Gio) > 60 THEN 1
//            ELSE 0
//        END AS is_new_group
//    FROM SortedVao
//),
//GroupNumberedVao AS (
//    SELECT *,
//        SUM(is_new_group) OVER (PARTITION BY MaCoiChinh ORDER BY Gio ROWS UNBOUNDED PRECEDING) + 1 AS luot
//    FROM TimeDiffVao
//),
//GroupedVao AS (
//    SELECT
//        p.MaCoiChinh AS MaCoi,
//        p.Ngay,
//        p.NgayNguyenLieu,
//        p.MaThanhPhamChinh,
//        tp.Ten AS ThanhPhamNameVao,
//        p.MaSizeChinh,
//        s.Ten AS SizeNameVao,
//        p.luot,
//        MIN(CAST(p.Gio AS TIME)) AS gio_bat_dau_vao,
//        MAX(CAST(p.Gio AS TIME)) AS gio_ket_thuc_vao,
//        COUNT(*) AS so_phieu_vao,
//        SUM(p.TrongLuong) AS trong_luong_vao_coi
//    FROM GroupNumberedVao p
//    LEFT JOIN MaThanhPhamChinhXepKhuon tp ON p.MaThanhPhamChinh = tp.Ma
//    LEFT JOIN MaSizeChinhXepKhuon s ON p.MaSizeChinh = s.Ma
//    GROUP BY
//        p.MaCoiChinh, p.NgayNguyenLieu, p.MaThanhPhamChinh, tp.Ten,
//        p.MaSizeChinh, s.Ten, p.Ngay, p.luot
//)

//SELECT
//    ra.MaCoi,
//    ra.NgayNguyenLieu,
//    ra.luot,

//    -- Thành phẩm và size ra
//    ra.MaThanhPham AS MaThanhPham,
//    ra.ThanhPhamName,
//    ra.MaSize AS MaSize,
//    ra.SizeName,

//	-- thông tin thành phẩm vào
//    --vao.MaThanhPhamChinh AS MaThanhPhamVao,
//    --vao.ThanhPhamNameVao,
//    --vao.MaSizeChinh AS MaSizeVao,
//    --vao.SizeNameVao,

//    --vao.gio_bat_dau_vao,
//    --vao.gio_ket_thuc_vao,
//    ra.gio_bat_dau,
//    ra.gio_ket_thuc,
//	ra.DinhMucTangTrong,
//    --vao.so_phieu_vao,
//    ISNULL(vao.trong_luong_vao_coi, ISNULL(vc.TrongLuong, 0)) AS trong_luong_vao_coi,
//    ra.so_phieu,
//    ra.tong_trong_luong_ra_coi,
//	ra.ThamSoTangTrong,
//    ISNULL((
//        (ra.tong_trong_luong_ra_coi / NULLIF(ISNULL(vao.trong_luong_vao_coi, vc.TrongLuong), 0))
//        / ISNULL(NULLIF(ra.ThamSoTangTrong, 0), 1) * 100
//    ) - 100, 0) AS TyLeTangTrong

//FROM GroupedRa ra
//LEFT JOIN GroupedVao vao ON ra.MaCoi = vao.MaCoi AND ra.luot = vao.luot

//OUTER APPLY (
//    SELECT TOP 1 tlvc.TrongLuong
//    FROM (
//        SELECT tlc.TrongLuong
//        FROM TrongLuongCoiTheoSanPham tlc
//        WHERE tlc.MaCoi = ra.MaCoi AND tlc.MaSanPham = ra.MaThanhPham

//        UNION ALL

//        SELECT cl.TrongLuong
//        FROM CoiLeXepKhuon cl
//        WHERE cl.MaCoi = ra.MaCoi
//          AND cl.MaSanPham = ra.MaThanhPham
//          AND cl.NgayNguyenLieu = ra.NgayNguyenLieu
//          AND cl.NgayGio BETWEEN
//              CAST(ra.Ngay AS DATETIME) + CAST(ra.gio_bat_dau AS DATETIME) AND
//              CAST(ra.Ngay AS DATETIME) + CAST(ra.gio_ket_thuc AS DATETIME)
//    ) tlvc
//) vc

//ORDER BY ra.MaCoi, ra.luot;
//";
// bỏ qua phần size
//                var query = @"
//;WITH SortedRa AS (
//    SELECT *, LAG(gio) OVER (PARTITION BY MaCoi ORDER BY gio) AS gio_truoc
//    FROM PhieuCanRaCoi
//    WHERE NgayNguyenLieu = @ngayNguyenLieu
//),
//TimeDiffRa AS (
//    SELECT *,
//        CASE
//            WHEN gio_truoc IS NULL THEN 0
//            WHEN DATEDIFF(MINUTE, gio_truoc, gio) > 60 THEN 1
//            ELSE 0
//        END AS is_new_group
//    FROM SortedRa
//),
//GroupNumberedRa AS (
//    SELECT *,
//        SUM(is_new_group) OVER (PARTITION BY MaCoi ORDER BY gio ROWS UNBOUNDED PRECEDING) + 1 AS luot
//    FROM TimeDiffRa
//),
//GroupedRa AS (
//    SELECT
//        p.MaCoi,
//        p.Ngay,
//        p.NgayNguyenLieu,
//        p.MaThanhPham,
//        tp.Ten AS ThanhPhamName,
//        tp.ThamSoTangTrong,
//        tp.DinhMucTangTrong,
//        p.luot,
//        MIN(p.gio) AS gio_bat_dau,
//        MAX(p.gio) AS gio_ket_thuc,
//        COUNT(*) AS so_phieu,
//        SUM(p.trongluong) AS tong_trong_luong_ra_coi
//    FROM GroupNumberedRa p
//    LEFT JOIN MaThanhPhamChinhXepKhuon tp ON p.MaThanhPham = tp.Ma
//    WHERE tp.Ma != 'KTCAN'
//    GROUP BY
//        p.MaCoi, p.luot, p.NgayNguyenLieu, p.MaThanhPham, tp.Ten,
//        tp.ThamSoTangTrong, tp.DinhMucTangTrong, p.Ngay
//),

//-- Sắp xếp và đánh lượt dữ liệu vào cối
//SortedVao AS (
//    SELECT *, LAG(Gio) OVER (PARTITION BY MaCoiChinh ORDER BY Gio) AS gio_truoc
//    FROM PhieuCanChinhXepKhuon
//    WHERE NgayNguyenLieu = @ngayNguyenLieu
//),
//TimeDiffVao AS (
//    SELECT *,
//        CASE
//            WHEN gio_truoc IS NULL THEN 0
//            WHEN DATEDIFF(MINUTE, gio_truoc, Gio) > 60 THEN 1
//            ELSE 0
//        END AS is_new_group
//    FROM SortedVao
//),
//GroupNumberedVao AS (
//    SELECT *,
//        SUM(is_new_group) OVER (PARTITION BY MaCoiChinh ORDER BY Gio ROWS UNBOUNDED PRECEDING) + 1 AS luot
//    FROM TimeDiffVao
//),
//GroupedVao AS (
//    SELECT
//        p.MaCoiChinh AS MaCoi,
//        p.Ngay,
//        p.NgayNguyenLieu,
//        p.MaThanhPhamChinh,
//        tp.Ten AS ThanhPhamNameVao,
//        p.luot,
//        MIN(CAST(p.Gio AS TIME)) AS gio_bat_dau_vao,
//        MAX(CAST(p.Gio AS TIME)) AS gio_ket_thuc_vao,
//        COUNT(*) AS so_phieu_vao,
//        SUM(p.TrongLuong) AS trong_luong_vao_coi
//    FROM GroupNumberedVao p
//    LEFT JOIN MaThanhPhamChinhXepKhuon tp ON p.MaThanhPhamChinh = tp.Ma
//    WHERE tp.IsKhongThuc = 0 AND tp.Ma != 'KTCAN'
//    GROUP BY
//        p.MaCoiChinh, p.NgayNguyenLieu, p.MaThanhPhamChinh,
//        tp.Ten, p.Ngay, p.luot
//)

//-- Ghép dữ liệu vào-ra theo Cối + Thành Phẩm + Lượt
//SELECT
//    ra.MaCoi,
//    ra.NgayNguyenLieu,
//    ra.luot,

//    -- Thành phẩm ra
//    ra.MaThanhPham,
//    ra.ThanhPhamName,

//    -- Thành phẩm vào
//    vao.MaThanhPhamChinh AS MaThanhPhamVao,
//    vao.ThanhPhamNameVao,

//    vao.gio_bat_dau_vao,
//    vao.gio_ket_thuc_vao,
//    ra.gio_bat_dau,
//    ra.gio_ket_thuc,

//    ra.DinhMucTangTrong,
//    ISNULL(vao.trong_luong_vao_coi, ISNULL(vc.TrongLuong, 0)) AS trong_luong_vao_coi,
//    ra.so_phieu,
//    ra.tong_trong_luong_ra_coi,
//    ra.ThamSoTangTrong,

//    ISNULL((
//        (ra.tong_trong_luong_ra_coi / NULLIF(ISNULL(vao.trong_luong_vao_coi, vc.TrongLuong), 0))
//        / ISNULL(NULLIF(ra.ThamSoTangTrong, 0), 1) * 100
//    ) - 100, 0) AS TyLeTangTrong

//FROM GroupedRa ra
//LEFT JOIN GroupedVao vao ON ra.MaCoi = vao.MaCoi AND ra.luot = vao.luot AND ra.MaThanhPham = vao.MaThanhPhamChinh

//-- Fallback trọng lượng vào nếu không có dữ liệu
//OUTER APPLY (
//    SELECT TOP 1 tlvc.TrongLuong
//    FROM (
//        SELECT tlc.TrongLuong
//        FROM TrongLuongCoiTheoSanPham tlc
//        WHERE tlc.MaCoi = ra.MaCoi AND tlc.MaSanPham = ra.MaThanhPham

//        UNION ALL

//        SELECT cl.TrongLuong
//        FROM CoiLeXepKhuon cl
//        WHERE cl.MaCoi = ra.MaCoi
//          AND cl.MaSanPham = ra.MaThanhPham
//          AND cl.NgayNguyenLieu = ra.NgayNguyenLieu
//          AND cl.NgayGio BETWEEN
//              CAST(ra.Ngay AS DATETIME) + CAST(ra.gio_bat_dau AS DATETIME) AND
//              CAST(ra.Ngay AS DATETIME) + CAST(ra.gio_ket_thuc AS DATETIME)
//    ) tlvc
//) vc

//ORDER BY ra.MaCoi, ra.luot;
//";
                var query = @"
;WITH SortedRa AS (
    SELECT *, LAG(gio) OVER (PARTITION BY MaCoi ORDER BY gio) AS gio_truoc
    FROM PhieuCanRaCoi
    WHERE NgayNguyenLieu = @ngayNguyenLieu
),
TimeDiffRa AS (
    SELECT *,
        CASE
            WHEN gio_truoc IS NULL THEN 0
            WHEN DATEDIFF(MINUTE, gio_truoc, gio) > 60 THEN 1
            ELSE 0
        END AS is_new_group
    FROM SortedRa
),
GroupNumberedRa AS (
    SELECT *,
        SUM(is_new_group) OVER (PARTITION BY MaCoi ORDER BY gio ROWS UNBOUNDED PRECEDING) + 1 AS luot
    FROM TimeDiffRa
),
GroupedRa AS (
    SELECT
        p.MaCoi,
        p.Ngay,
        p.NgayNguyenLieu,
        p.MaThanhPham,
        tp.Ten AS ThanhPhamName,
		--p.MaSize,
		--s.Ten as SizeName,
        tp.ThamSoTangTrong,
        tp.DinhMucTangTrong,
        p.luot,
        MIN(p.gio) AS gio_bat_dau,
        MAX(p.gio) AS gio_ket_thuc,
        COUNT(*) AS so_phieu,
        SUM(p.trongluong) AS tong_trong_luong_ra_coi
    FROM GroupNumberedRa p
    LEFT JOIN MaThanhPhamChinhXepKhuon tp ON p.MaThanhPham = tp.Ma
	left join MaSizeChinhXepKhuon s on p.MaSize = s.Ma
    WHERE tp.Ma != 'KTCAN' and tp.IsKhongThuc = 0
    GROUP BY
        p.MaCoi, p.luot, p.NgayNguyenLieu, p.MaThanhPham, tp.Ten,
        tp.ThamSoTangTrong, tp.DinhMucTangTrong,
		--p.MaSize, s.Ten,
		p.Ngay
),

-- Sắp xếp và đánh lượt dữ liệu vào cối
SortedVao AS (
    SELECT *, LAG(Gio) OVER (PARTITION BY MaCoiChinh ORDER BY Gio) AS gio_truoc
    FROM PhieuCanChinhXepKhuon
    WHERE NgayNguyenLieu = @ngayNguyenLieu
),
TimeDiffVao AS (
    SELECT *,
        CASE
            WHEN gio_truoc IS NULL THEN 0
            WHEN DATEDIFF(MINUTE, gio_truoc, Gio) > 60 THEN 1
            ELSE 0
        END AS is_new_group
    FROM SortedVao
),
GroupNumberedVao AS (
    SELECT *,
        SUM(is_new_group) OVER (PARTITION BY MaCoiChinh ORDER BY Gio ROWS UNBOUNDED PRECEDING) + 1 AS luot
    FROM TimeDiffVao
),
GroupedVao AS (
    SELECT
        p.MaCoiChinh AS MaCoi,
        --p.Ngay,
        p.NgayNguyenLieu,
        p.MaThanhPhamChinh,
        tp.Ten AS ThanhPhamNameVao,
		--p.MaSizeChinh,
		--s.Ten as SizeNameVao,
        p.luot,
        MIN(CAST(p.Gio AS TIME)) AS gio_bat_dau_vao,
        MAX(CAST(p.Gio AS TIME)) AS gio_ket_thuc_vao,
        COUNT(*) AS so_phieu_vao,
        SUM(p.TrongLuong) AS trong_luong_vao_coi
    FROM GroupNumberedVao p
    LEFT JOIN MaThanhPhamChinhXepKhuon tp ON p.MaThanhPhamChinh = tp.Ma
	left join MaSizeChinhXepKhuon s on p.MaSizeChinh = s.Ma
    WHERE tp.IsKhongThuc = 0 AND tp.Ma != 'KTCAN'
    GROUP BY
        p.MaCoiChinh, p.NgayNguyenLieu, p.MaThanhPhamChinh,
        tp.Ten, --p.Ngay,
		--p.MaSizeChinh,s.Ten,
		p.luot
)

-- Ghép dữ liệu vào-ra theo Cối + Thành Phẩm + Lượt
SELECT
    ra.MaCoi,
    ra.NgayNguyenLieu,
    ra.luot,
	vao.luot as luotvao,
    -- Thành phẩm ra
    ra.MaThanhPham,
    ra.ThanhPhamName,

    -- Thành phẩm vào
    vao.MaThanhPhamChinh AS MaThanhPhamVao,
    vao.ThanhPhamNameVao,
	--ra.MaSize,
	--ra.SizeName,
	--vao.MaSizeChinh,
	--vao.SizeNameVao,
    vao.gio_bat_dau_vao,
    vao.gio_ket_thuc_vao,
    ra.gio_bat_dau,
    ra.gio_ket_thuc,

    ra.DinhMucTangTrong,
	ISNULL(vao.trong_luong_vao_coi, ISNULL(vc1.TrongLuong, vc2.TrongLuong)) AS trong_luong_vao_coi,
    ra.so_phieu,
	vao.so_phieu_vao,
    ra.tong_trong_luong_ra_coi,
    ra.ThamSoTangTrong,

    ISNULL((
    (ra.tong_trong_luong_ra_coi / NULLIF(ISNULL(vao.trong_luong_vao_coi, ISNULL(vc1.TrongLuong, vc2.TrongLuong)), 0))
    / ISNULL(NULLIF(ra.ThamSoTangTrong, 0), 1) * 100
) - 100, 0) AS TyLeTangTrong

FROM GroupedRa ra
LEFT JOIN GroupedVao vao ON ra. MaCoi= vao.MaCoi 
AND ra.luot = vao.luot
--AND ra.MaThanhPham = vao.MaThanhPhamChinh
OUTER APPLY (
    SELECT TOP 1 tlc.TrongLuong
    FROM TrongLuongCoiTheoSanPham tlc
    WHERE tlc.MaCoi = ra.MaCoi AND tlc.MaSanPham = ra.MaThanhPham
) vc1

OUTER APPLY (
    SELECT TOP 1 cl.TrongLuong
    FROM CoiLeXepKhuon cl
    WHERE cl.MaCoi = ra.MaCoi
      AND cl.MaSanPham = ra.MaThanhPham
      AND cl.NgayNguyenLieu = ra.NgayNguyenLieu
      AND cl.NgayGio <= CAST(ra.Ngay AS DATETIME) + CAST(ra.gio_bat_dau AS DATETIME)
    ORDER BY cl.NgayGio DESC
) vc2

ORDER BY ra.MaCoi, ra.luot;
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { ngayNguyenLieu = ngayNguyenLieu.Date}).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan_XLPC<T>(DateTime dateTime, string xuongId)
        {
            var query = @"select
p.STT,
p.Id,
p.IdMonitor,
p.Ngay,
p.Gio,
p.MaXuong,
x.Ten as XuongName,
p.MayCan,
p.NgayNguyenLieu,
p.TrongLuong,
p.TrongLuongTare,
p.MaLo,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
nv.DeptName0 as NhomName,
p.MaCoi,
c.Ten as CoiName,
p.MaThe,
p.GhiChu
from PhieuCanRaCoi p
left join XiNghiep x on x.Ma  = p.MaXuong
left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPham
left join MaSizeChinhXepKhuon s on s.Ma = p.MaSize
left join MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
left join MaChatLuongXepKhuon cl on cl.Ma = p.MaChatLuong
left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
left join MaCoiXepKhuon c on c.Ma = p.MaCoi
where p.NgayNguyenLieu = @dateTime and p.MaXuong = @xuongId
order by
p.STT desc";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { dateTime = dateTime.Date, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }
        #endregion
    }
}
