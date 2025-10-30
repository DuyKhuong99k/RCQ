using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanLangDa
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanLangDa";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhieuCanLangDa]
      WHERE [MayCan] = @MayCan 
      And [MaXuong] = @MaXuong 
      And [STT] = @STT 
      And [Ngay] = @Ngay";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanLangDa]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MayCan]
           ,[MaXuong]
           ,[MaUserCan]
           ,[MaLoaiCa]
           ,[MaMau]
           ,[MaSize]
           ,[MaThanhPham]
           ,[MaLo]
           ,[MaThe]
           ,[TrongLuong]
           ,[MaNhanVien]
           ,[GhiChu])
     VALUES
           (@STT
           ,@Ngay
           ,@Gio
           ,@MayCan
           ,@MaXuong
           ,@MaUserCan
           ,@MaLoaiCa
           ,@MaMau
           ,@MaSize
           ,@MaThanhPham
           ,@MaLo
           ,@MaThe
           ,@TrongLuong
           ,@MaNhanVien
           ,@GhiChu)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanLangDa]
   SET [Gio] = @Gio 
       ,[MaUserCan] = @MaUserCan 
      ,[MaLoaiCa] = @MaLoaiCa 
      ,[MaMau] = @MaMau 
      ,[MaSize] = @MaSize 
      ,[MaThanhPham] = @MaThanhPham 
      ,[MaLo] = @MaLo 
      ,[MaThe] = @MaThe 
      ,[TrongLuong] = @TrongLuong 
      ,[MaNhanVien] = @MaNhanVien 
      ,[GhiChu] = @GhiChu
 WHERE [MayCan] = @MayCan 
      And [STT] = @STT 
      And [MaXuong] = @MaXuong 
      And [Ngay] = @Ngay ";

        private readonly string qrGetAll = "Select * from PhieuCanLangDa";

        public PhieuCanLangDa()
        {
            connectionString = AppViewModels.Base.Ins.ConnectionString;

        }

        public int Delete<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrDelete, item);
            return rows;
        }

        public List<T> Gets<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetAll).ToList();
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

        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"SELECT
    p.STT,
    p.Ngay,
    p.Gio,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,
    p.MaLo,
    tp.Ten as ThanhPhamName,
    p.TrongLuong,
    p.MayCan as MaMayCan,
    p.MaXuong
from
    PhieuCanLangDa p,
    MaThanhPhamLangDa tp,
    NhanVienDaiThanh n
WHERE
    p.Ngay >= @fromDate and p.Ngay <= @toDate
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
ORDER BY
    p.STT DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTietByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var query = @"SELECT
    p.STT,
    p.Ngay,
    p.Gio,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,
    p.MaLo,
    tp.Ten as ThanhPhamName,
    p.TrongLuong,
    p.MayCan as MaMayCan,
    p.MaXuong
from
    PhieuCanLangDa p,
    MaThanhPhamLangDa tp,
    NhanVienDaiThanh n
WHERE
    p.Ngay >= @fromDate 
    and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
	and p.MaNhanVien = @maNhanVien
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
ORDER BY
    p.STT DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId, maNhanVien = maNhanVien }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTietByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var query = @"SELECT
    p.STT,
    p.Ngay,
    p.Gio,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,
    p.MaLo,
    tp.Ten as ThanhPhamName,
    p.TrongLuong,
    p.MayCan as MaMayCan,
    p.MaXuong
from
    PhieuCanLangDa p,
    MaThanhPhamLangDa tp,
    NhanVienDaiThanh n
WHERE
    p.Ngay >= @fromDate 
    and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
    and n.MaHoSo = @maHoSo
ORDER BY
    p.STT DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId, maHoSo = maHoSo }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTietByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var query = @"SELECT
    p.STT,
    p.Ngay,
    p.Gio,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,
    p.MaLo,
    tp.Ten as ThanhPhamName,
    p.TrongLuong,
    p.MayCan as MaMayCan,
    p.MaXuong
from
    PhieuCanLangDa p,
    MaThanhPhamLangDa tp,
    NhanVienDaiThanh n,
    TheTu t
WHERE
    p.Ngay >= @fromDate 
    and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
    and p.MaNhanVien = t.MaNhanVien
	and t.MaTheTu = @maThe
ORDER BY
    p.STT DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId, maThe = maThe }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHops<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
//                var query = @"
//SELECT
//    p.Ngay,
//    p.MaNhanVien,
//    n.MaHoSo,
//    n.Name as TenNhanVien,
//    p.MaLo,
//    tp.Ten as ThanhPhamName,
//    SUM(p.TrongLuong) as TrongLuong,
//    COUNT(p.STT) as SoRo,
//    p.MaXuong
//from
//    PhieuCanLangDa p,
//    MaThanhPhamLangDa tp,
//    NhanVienDaiThanh n
//WHERE
//    p.Ngay >= @fromDate and p.Ngay <= @toDate
//    and p.MaThanhPham = tp.Ma
//    and p.MaNhanVien = n.MaNhanVien
//GROUP BY
//    p.Ngay,
//    p.MaNhanVien,
//    n.MaHoSo,
//    n.Name,
//    p.MaLo,
//    tp.Ten,
//    p.MaXuong
//ORDER BY
//    n.MaHoSo DESC";
var query = @"
SELECT
    p.Ngay,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,
    p.MaLo,
    tp.Ten as ThanhPhamName,
    SUM(p.TrongLuong) as TrongLuong,
    COUNT(p.STT) as SoRo,
    p.MaXuong,
	MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianVao,
	MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianRa,
	DATEDIFF(hour, MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay), MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay)) as TongThoiGian
from
    PhieuCanLangDa p,
    MaThanhPhamLangDa tp,
    NhanVienDaiThanh n,
	CheckInOut c
WHERE
    p.Ngay >= @fromDate and p.Ngay <= @toDate
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
	AND n.MaChamCong = c.MaChamCong AND c.ThoiGian = p.Ngay AND c.ThoiGian >= @fromDate AND c.ThoiGian <= @toDate 
GROUP BY
    p.Ngay,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name,
    p.MaLo,
    tp.Ten,
    p.MaXuong,
	p.Ngay,
	n.MaChamCong,
	c.ThoiGian
ORDER BY
    n.MaHoSo DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var query = @"
SELECT
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as NhanVienName,
    tp.Ten as ThanhPhamName,
    SUM(p.TrongLuong) as TrongLuong,
    COUNT(p.STT) as SoRo,
    p.MaXuong
from
    PhieuCanLangDa p,
    MaThanhPhamLangDa tp,
    NhanVienDaiThanh n
WHERE
    p.Ngay >= @fromDate and p.Ngay <= @toDate and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
	and p.MaNhanVien = @maNhanVien
GROUP BY
    p.MaNhanVien,
    n.MaHoSo,
    n.Name,
    p.MaLo,
    tp.Ten
ORDER BY
    n.MaHoSo DESCC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, maNhanVien = maNhanVien, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var query = @"
SELECT
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as NhanVienName,
    tp.Ten as ThanhPhamName,
    SUM(p.TrongLuong) as TrongLuong,
    COUNT(p.STT) as SoRo,
    p.MaXuong
from
    PhieuCanLangDa p,
    MaThanhPhamLangDa tp,
    NhanVienDaiThanh n
WHERE
    p.Ngay >= @fromDate and p.Ngay <= @toDate and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
	and n.MaHoSo = @maHoSo
GROUP BY
    p.MaNhanVien,
    n.MaHoSo,
    n.Name,
    p.MaLo,
    tp.Ten
ORDER BY
    n.MaHoSo DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, maHoSo = maHoSo, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var query = @"
SELECT
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as NhanVienName,
    tp.Ten as ThanhPhamName,
    SUM(p.TrongLuong) as TrongLuong,
    COUNT(p.STT) as SoRo,
    p.MaXuong
from
    PhieuCanLangDa p,
    MaThanhPhamLangDa tp,
    NhanVienDaiThanh n,
    TheTu t
WHERE
    p.Ngay >= @fromDate and p.Ngay <= @toDate and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
	and p.MaNhanVien = t.MaNhanVien
	and t.MaTheTu = @maThe
GROUP BY
    p.MaNhanVien,
    n.MaHoSo,
    n.Name,
    p.MaLo,
    tp.Ten
ORDER BY
    n.MaHoSo DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, maThe = maThe, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
