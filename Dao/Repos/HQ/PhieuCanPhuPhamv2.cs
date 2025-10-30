using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanPhuPhamv2
    {
        private readonly string connectionString;
        private string tableName = @"BoPhan";
        private readonly string qrDelete = @"Delete [dbo].[PhieuCanPhuPhamv2]  WHERE [STT] = @STT 
      and [Ngay] = @Ngay 
      and [MaMayCan] = @MaMayCan 
      and [MaXuong] = @MaXuong ";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanPhuPhamv2]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MaThanhPham]
           ,[MaLo]
           ,[MaXuong]
           ,[MaMayCan]
           ,[MaNhanVien]
           ,[MaThe]
           ,[SuDung]
           ,[GhiChu]
           ,[TrongLuong])
     VALUES
           (@STT
           ,@Ngay
           ,@Gio
           ,@MaThanhPham
           ,@MaLo
           ,@MaXuong
           ,@MaMayCan
           ,@MaNhanVien
           ,@MaThe
           ,@SuDung
           ,@GhiChu
           ,@TrongLuong)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanPhuPhamv2]
   SET [Gio] = @Gio 
      ,[MaThanhPham] = @MaThanhPham 
      ,[MaLo] = @MaLo   
      ,[MaNhanVien] = @MaNhanVien 
      ,[MaThe] =@MaThe 
      ,[SuDung] = @SuDung 
      ,[GhiChu] = @GhiChu 
      ,[TrongLuong] = @TrongLuong
 WHERE [STT] = @STT 
      and [Ngay] = @Ngay 
      and [MaMayCan] = @MaMayCan 
      and [MaXuong] = @MaXuong ";

        private readonly string qrGetAll = "Select * from PhieuCanPhuPhamv2";
        private readonly string qrGetsLastByNumAndMayCan = @"WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MaMayCan ORDER BY Ngay DESC, Gio DESC) AS RowNum
    FROM PhieuCanPhuPhamv2 where Ngay =@ngay
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
        public PhieuCanPhuPhamv2(string? _connectionString = null)
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
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan_XLPC<T>(DateTime dateTime, string xuongId)
        {
            var query = @"select
p.STT,
p.Ngay,
p.Gio,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLo,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
nv.DeptName0 as Nhom,
p.MaThe,
p.SuDung,
p.GhiChu,
p.TrongLuong
from PhieuCanPhuPhamv2 p
left join MaThanhPhamPhuPham tp on p.MaThanhPham = tp.Ma
left join XiNghiep x on p.MaXuong = x.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
where p.Ngay = @dateTime and p.MaXuong = @xuongId
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
        public List<T> GetSanLuongTinhLuong<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    p.Ngay,
    'CT04' as CaLamViec,
    p.MaNhanVien,
n.Name as TenNhanVien,
n.MaHoSo,
    tp.BarvoId as MaSanPham,
    tp.Ten as TenSanPham,
    SUM(p.TrongLuong) as TrongLuong,
    '11' as KhuVuc,
    0 as _Status,
    cast(0 as decimal(18, 2)) as DonGia,
    cast(0 as decimal(18, 2)) as ThanhTien,
    cast(0 as bit) as IsChamCong,
    COUNT(*) as SoRo
from
    PhieuCanPhuPhamv2 p,
    NhanVienDaiThanh n,
    MaThanhPhamPhuPham tp
where
    p.Ngay = @ngay
    and p.MaNhanVien = n.MaNhanVien
    and p.MaThanhPham = tp.Ma
    and p.MaXuong = @xuongId GROUP BY
    p.Ngay,
    p.MaNhanVien,
    tp.BarvoId,
    tp.Ten,
n.Name,
n.MaHoSo";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
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
    p.MaMayCan,
    p.MaXuong
from
    PhieuCanPhuPhamv2 p,
    MaThanhPhamPhuPham tp,
    NhanVienDaiThanh n
WHERE
    p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
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
        public List<T> GetChiTietByMaNhanViens<T>(DateTime fromDate, DateTime toDate,string maNhanVien, string xuongId )
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
    p.MaMayCan,
    p.MaXuong
from
    PhieuCanPhuPhamv2 p,
    MaThanhPhamPhuPham tp,
    NhanVienDaiThanh n
WHERE
    p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
    and p.MaNhanVien = @maNhanVien
ORDER BY
    p.STT DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId =xuongId ,maNhanVien = maNhanVien }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTietByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo,string xuongId)
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
    p.MaMayCan,
    p.MaXuong
from
    PhieuCanPhuPhamv2 p,
    MaThanhPhamPhuPham tp,
    NhanVienDaiThanh n
WHERE
    p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
 and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
    and n.MaHoSo = @maHoSo
ORDER BY
    p.STT DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId =xuongId , maHoSo = maHoSo }).ToList();
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
    p.MaMayCan,
    p.MaXuong
from
    PhieuCanPhuPhamv2 p,
    MaThanhPhamPhuPham tp,
    NhanVienDaiThanh n,
    TheTu t
WHERE
    p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
 and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
    and p.MaNhanVien = t.MaNhanVien
	and t.MaTheTu = @maThe
ORDER BY
    p.STT DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId =xuongId , maThe = maThe }).ToList();
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
    p.MaXuong
from
    PhieuCanPhuPhamv2 p,
    MaThanhPhamPhuPham tp,
    NhanVienDaiThanh n
WHERE
    p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
GROUP BY
    p.Ngay,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name,
    p.MaLo,
    tp.Ten,
    p.MaXuong
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

        public List<T> GetTongHopThanhPhamTheoNhanVienByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var query = @"
SELECT
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,
    p.MaLo,
    tp.Ten as ThanhPhamName,
    SUM(p.TrongLuong) as TrongLuong,
    COUNT(p.STT) as SoRo,
    p.MaXuong
from
    PhieuCanPhuPhamv2 p,
    MaThanhPhamPhuPham tp,
    NhanVienDaiThanh n
WHERE
    p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
	and p.MaNhanVien = @maNhanVien
GROUP BY
    p.Ngay,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name,
    p.MaLo,
    tp.Ten,
    p.MaXuong
ORDER BY
    n.MaHoSo DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, maNhanVien, xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamTheoNhanVienByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var query = @"
SELECT
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,
    p.MaLo,
    tp.Ten as ThanhPhamName,
    SUM(p.TrongLuong) as TrongLuong,
    COUNT(p.STT) as SoRo,
    p.MaXuong
from
    PhieuCanPhuPhamv2 p,
    MaThanhPhamPhuPham tp,
    NhanVienDaiThanh n,
    TheTu t
WHERE
    p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
	and p.MaNhanVien = t.MaNhanVien
	and t.MaTheTu = @maThe
GROUP BY
    p.Ngay,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name,
    p.MaLo,
    tp.Ten,
    p.MaXuong
ORDER BY
    n.MaHoSo DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, maThe, xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamTheoNhanVienByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var query = @"
SELECT
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,
    p.MaLo,
    tp.Ten as ThanhPhamName,
    SUM(p.TrongLuong) as TrongLuong,
    COUNT(p.STT) as SoRo,
    p.MaXuong
from
    PhieuCanPhuPhamv2 p,
    MaThanhPhamPhuPham tp,
    NhanVienDaiThanh n
WHERE
    p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.MaNhanVien = n.MaNhanVien
	and n.MaHoSo = @maHoSo
GROUP BY
    p.Ngay,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name,
    p.MaLo,
    tp.Ten,
    p.MaXuong
ORDER BY
    n.MaHoSo DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, maHoSo, xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
