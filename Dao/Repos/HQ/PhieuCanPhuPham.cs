using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanPhuPham
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanPhuPham";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhieuCanPhuPham]
      WHERE [MaMayTinhCan] = @MaMayTinhCan
       and [MaUserCan] = @MaUserCan
      and [NgayCan] = @NgayCan and [ThoiGianCan] = @ThoiGianCan
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanPhuPham]
           ([MaMayTinhCan]
           ,[MaUserCan]
           ,[ThoiGianCan]
           ,[Ngay]
           ,[NhaMuaHang]
           ,[MSL]
           ,[MaPhuongTien]
           ,[MaLoaiCa]
           ,[MaLoaiThanhPham]
           ,[MaSize]
           ,[MaMau]
           ,[TrongLuong]
           ,[SuDung]
           ,[GhiChu]
           ,[MaXuongSanXuat],[NgayCan],[TrongLuongTare])
     VALUES
           (@MaMayTinhCan
           ,@MaUserCan
           ,@ThoiGianCan
           ,@Ngay
           ,@NhaMuaHang
           ,@MSL
           ,@MaPhuongTien
           ,@MaLoaiCa
           ,@MaLoaiThanhPham
           ,@MaSize
           ,@MaMau
           ,@TrongLuong
           ,@SuDung
           ,@GhiChu
           ,@MaXuongSanXuat,@NgayCan,@TrongLuongTare)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanPhuPham]
   SET [Ngay] = @Ngay
      ,[NhaMuaHang] = @NhaMuaHang
      ,[MSL] = @MSL
      ,[MaPhuongTien] = @MaPhuongTien
      ,[MaLoaiCa] = @MaLoaiCa
      ,[MaLoaiThanhPham] = @MaLoaiThanhPham
      ,[MaSize] = @MaSize
      ,[MaMau] = @MaMau
      ,[TrongLuong] = @TrongLuong
      ,[SuDung] = @SuDung
      ,[GhiChu] = @GhiChu
      ,[MaXuongSanXuat] = @MaXuongSanXuat,[TrongLuongTare] =@TrongLuongTare
 WHERE [MaMayTinhCan] = @MaMayTinhCan
       and [MaUserCan] = @MaUserCan
      and [NgayCan] = @NgayCan
      and [ThoiGianCan] = @ThoiGianCan
";

        private readonly string qrGetAll = "Select * from PhieuCanPhuPham";
        private readonly string qrGetsLastByNumAndMayCan = @"WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MaMayTinhCan ORDER BY NgayCan DESC, ThoiGianCan DESC) AS RowNum
    FROM PhieuCanPhuPham where Ngay =@ngay
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
        public PhieuCanPhuPham(string? _connectionString = null)
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
        public T? Get<T>(string id)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>("select * from PhieuCanPhuPham where Id = @id", new {id}).FirstOrDefault();
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
       

        public T GetMaxThoiGianCan<T>(DateTime dateTime, string mayCanId)
        {
            try
            {
                var query =
                    @"Select Max(NgayCan) from PhieuCanPhuPham where Convert(date, NgayCan)=@ngay and MaMayTinhCan = @mayCanId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.ExecuteScalar<T>(query, new { ngay = dateTime.Date, mayCanId });
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetChiTiets(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"SELECT
    p.[Ngay] as [Ngày],
    cast(p.ThoiGianCan as datetime) as [Thời Gian],
	p.NgayCan as [Ngày Cân],
    p.[MaPhuongTien] as [Mã Phương Tiện],
    pt.[Ten] as [Tên Phương Tiện],
    kh.[Ten] as [Nhà Mua Hàng],
    la.[Ten] as [Loại Cá],
    tp.[Ten] as [Thành Phẩm],
    s.[Ten] as [Size],
    mau.Ten as [Màu],
    p.[TrongLuong] as [Trọng Lượng],
    p.[TrongLuongTare] as [Trọng Lượng Tare], 
    p.[MaXuongSanXuat] as [Xưởng]
FROM
    PhieuCanPhuPham p,
    NhaMuaPhuPham kh,
	MaLoaiCaPhuPham la,
	MaThanhPhamPhuPham tp,
	MaSizePhuPham s,
	MaMauPhuPham mau,
	PhuongTienChoPhuPham pt
WHERE
    p.[SuDung] = 1
    and p.[TrongLuong] > 0
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.NhaMuaHang = kh.Ma
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaPhuongTien = pt.Ma
order by
    ThoiGianCan";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@toDate", toDate.Date);
                // cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetTongHopNhaMuaHang<T>(DateTime dateTime, string maPhuongTien)
        {
            try
            {
                var query = @"SELECT
    p.[Ngay] as [Ngày],
    p.[MaXuongSanXuat] as [Xưởng],
    kh.[Ten] as [Nhà Mua Hàng],
    p.[MaPhuongTien] as [Mã Phương Tiện],
pt.[Ten] as [Tên Phương Tiện],
    la.[Ten] as [Loại Cá],
    tp.[Ten] as [Thành Phẩm],
    s.[Ten] as [Size],
    mau.Ten as [Màu],
    Cast(SUM(p.TrongLuong) as decimal(18, 2)) as [Trọng Lượng]
FROM
    PhieuCanPhuPham p,
    NhaMuaPhuPham kh,
    MaLoaiCaPhuPham la,
    MaThanhPhamPhuPham tp,
    MaSizePhuPham s,
    MaMauPhuPham mau,
PhuongTienChoPhuPham pt
WHERE
    p.[SuDung] = 1
    and p.[TrongLuong] > 0
    and p.Ngay = @ngay
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.NhaMuaHang = kh.Ma
    and p.MaPhuongTien = pt.Ma
    and p.MaPhuongTien = @maPhuongTien
GROUP BY
    p.[Ngay],
    p.[MSL],
    p.[MaPhuongTien],
    la.[Ten],
    tp.[Ten],
    s.[Ten],
    mau.Ten,
    p.[MaXuongSanXuat],
	kh.Ten,
pt.Ten
order by
    p.Ngay,
    kh.Ten";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, maPhuongTien }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopNhaMuaHang<T>(
            DateTime dateTime,
            string nhaMuaHang,
            string maPhuongTien
            )

        {
            try
            {
                var query = $@"SELECT
    ROW_NUMBER() OVER(
        ORDER BY
            p.ThanhPhamName DESC
    ) as STT,
    p.*,
    cast(
        case
            when SUM(p.TrongLuong) over() = 0 then 0
            else p.TrongLuong / SUM(p.TrongLuong) over()
        end as decimal(18, 4)
    ) as TyLe,
'kg' as dvt
from
    (
        SELECT
            p.[Ngay],
            kh.[Ten] as [KhachHangName],
            p.[MaPhuongTien],
            pt.[Ten] as [PhuongTienName],
            la.[Ten] as [LoaiCaName],
            tp.[Ten] as [ThanhPhamName],
            s.[Ten] as [Size],
            mau.Ten as [MauName],
            Cast(SUM(p.TrongLuong) as decimal(18, 2)) as [TrongLuong]
        FROM
            PhieuCanPhuPham p,
            NhaMuaPhuPham kh,
            MaLoaiCaPhuPham la,
            MaThanhPhamPhuPham tp,
            MaSizePhuPham s,
            MaMauPhuPham mau,
            PhuongTienChoPhuPham pt
        WHERE
            p.[SuDung] = 1
            and p.[TrongLuong] > 0
            and p.Ngay = @ngay
            and p.MaLoaiCa = la.Ma
            and p.MaLoaiThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.MaMau = mau.Ma
            and p.NhaMuaHang = kh.Ma
            and p.MaPhuongTien = pt.Ma
            and p.MaPhuongTien = @maPhuongTien
            and p.NhaMuaHang = @nhaMuaHang
        GROUP BY
            p.[Ngay],
            p.[MSL],
            p.[MaPhuongTien],
            p.NhaMuaHang,
            la.[Ten],
            tp.[Ten],
            s.[Ten],
            mau.Ten,
            kh.Ten,
            pt.Ten
    ) p";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, maPhuongTien, nhaMuaHang })
                    .ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopNhaMuaHang<T>(
            DateTime dateTime,
            string nhaMuaHang,
            string maPhuongTien,
            string xuongId,int decNum =4)

        {
            try
            {
                var query = $@"SELECT
    ROW_NUMBER() OVER(
        ORDER BY
            p.ThanhPhamName DESC
    ) as STT,
    p.*,
    cast(
        case
            when SUM(p.TrongLuong) over() = 0 then 0
            else p.TrongLuong / SUM(p.TrongLuong) over()
        end as decimal(18, {decNum})
    ) as TyLe,
'kg' as dvt
from
    (
        SELECT
            p.[Ngay],
            p.[MaXuongSanXuat] as [MaXuong],
            kh.[Ten] as [KhachHangName],
            p.[MaPhuongTien],
            pt.[Ten] as [PhuongTienName],
            la.[Ten] as [LoaiCaName],
            tp.[Ten] as [ThanhPhamName],
            s.[Ten] as [Size],
            mau.Ten as [MauName],
            Cast(SUM(p.TrongLuong) as decimal(18, {decNum})) as [TrongLuong]
        FROM
            PhieuCanPhuPham p,
            NhaMuaPhuPham kh,
            MaLoaiCaPhuPham la,
            MaThanhPhamPhuPham tp,
            MaSizePhuPham s,
            MaMauPhuPham mau,
            PhuongTienChoPhuPham pt
        WHERE
            p.[SuDung] = 1
            and p.[TrongLuong] > 0
            and p.Ngay = @ngay
            and p.MaLoaiCa = la.Ma
            and p.MaLoaiThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.MaMau = mau.Ma
            and p.NhaMuaHang = kh.Ma
            and p.MaPhuongTien = pt.Ma
            and p.MaPhuongTien = @maPhuongTien
            and p.MaXuongSanXuat = @xuongId
            and p.NhaMuaHang = @nhaMuaHang
        GROUP BY
            p.[Ngay],
            p.[MSL],
            p.[MaPhuongTien],
            p.NhaMuaHang,
            la.[Ten],
            tp.[Ten],
            s.[Ten],
            mau.Ten,
            p.[MaXuongSanXuat],
            kh.Ten,
            pt.Ten
    ) p";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, maPhuongTien, nhaMuaHang, xuongId })
                    .ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //chắt thêm
        public List<T> GetTongHopNhaMuaHang<T>(
            DateTime dateTime,
            string nhaMuaHang,
            string xuongId,int decNum )

        {
            try
            {
                var query = $@"SELECT
    ROW_NUMBER() OVER(
        ORDER BY
            p.ThanhPhamName DESC
    ) as STT,
    p.*,
    cast(
        case
            when SUM(p.TrongLuong) over() = 0 then 0
            else p.TrongLuong / SUM(p.TrongLuong) over()
        end as decimal(18, {decNum})
    ) as TyLe,
'kg' as dvt
from
    (
        SELECT
            p.[Ngay],
            p.[MaXuongSanXuat] as [MaXuong],
            kh.[Ten] as [KhachHangName],
            --p.[MaPhuongTien],
            --pt.[Ten] as [PhuongTienName],
            la.[Ten] as [LoaiCaName],
            tp.[Ten] as [ThanhPhamName],
            s.[Ten] as [Size],
            mau.Ten as [MauName],
            Cast(SUM(p.TrongLuong) as decimal(18, {decNum})) as [TrongLuong]
        FROM
            PhieuCanPhuPham p,
            NhaMuaPhuPham kh,
            MaLoaiCaPhuPham la,
            MaThanhPhamPhuPham tp, 
            MaSizePhuPham s, 
            MaMauPhuPham mau
            --PhuongTienChoPhuPham pt
        WHERE
            p.[SuDung] = 1
            and p.[TrongLuong] > 0
            and p.Ngay = @ngay
            and p.MaLoaiCa = la.Ma
            and p.MaLoaiThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.MaMau = mau.Ma
            and p.NhaMuaHang = kh.Ma
            --and p.MaPhuongTien = pt.Ma
            --and p.MaPhuongTien = @maPhuongTien
            and p.MaXuongSanXuat = @xuongId
            and p.NhaMuaHang = @nhaMuaHang
        GROUP BY
            p.[Ngay],
            p.[MSL],
            --p.[MaPhuongTien],
            p.NhaMuaHang,
            la.[Ten],
            tp.[Ten],
            s.[Ten],
            mau.Ten,
            p.[MaXuongSanXuat],
            kh.Ten
            --pt.Ten
    ) p";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, nhaMuaHang, xuongId })
                    .ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetTongHopNhaMuaHang(DateTime fromDate, DateTime toDate,int decNum =2)
        {
            try
            {
                var query = $@"SELECT
    p.[Ngay] as [Ngày],
    p.[MaXuongSanXuat] as [Xưởng],
    kh.[Ten] as [Nhà Mua Hàng],
    p.[MaPhuongTien] as [Mã Phương Tiện],
pt.[Ten] as [Tên Phương Tiện],
    la.[Ten] as [Loại Cá],
    tp.[Ten] as [Thành Phẩm],
    s.[Ten] as [Size],
    mau.Ten as [Màu],
    Cast(SUM(p.TrongLuong) as decimal(18, {decNum})) as [Trọng Lượng]
FROM
    PhieuCanPhuPham p,
    NhaMuaPhuPham kh,
    MaLoaiCaPhuPham la,
    MaThanhPhamPhuPham tp,
    MaSizePhuPham s,
    MaMauPhuPham mau,
PhuongTienChoPhuPham pt
WHERE
    p.[SuDung] = 1
    and p.[TrongLuong] > 0
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.NhaMuaHang = kh.Ma
    and p.MaPhuongTien = pt.Ma
GROUP BY
    p.[Ngay],
    p.[MSL],
    p.[MaPhuongTien],
    la.[Ten],
    tp.[Ten],
    s.[Ten],
    mau.Ten,
    p.[MaXuongSanXuat],
	kh.Ten,
pt.Ten
order by
    p.Ngay,
    kh.Ten";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@toDate", toDate.Date);
                // cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetTongHopThanhPham(DateTime fromDate, DateTime toDate,int decNum =2)
        {
            try
            {
                var query = $@"SELECT
    p.[Ngay] as [Ngày],
    p.[MaXuongSanXuat] as [Xưởng],
    la.[Ten] as [Loại Cá],
    tp.[Ten] as [Thành Phẩm],
    s.[Ten] as [Size],
    mau.Ten as [Màu],
    Cast(SUM(p.TrongLuong) as decimal(18, {decNum})) as [Trọng Lượng]
FROM
    PhieuCanPhuPham p,
    MaLoaiCaPhuPham la,
    MaThanhPhamPhuPham tp,
    MaSizePhuPham s,
    MaMauPhuPham mau
	
WHERE
    p.[SuDung] = 1
    and p.[TrongLuong] > 0
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
GROUP BY
    p.[Ngay],
    la.[Ten],
    tp.[Ten],
    s.[Ten],
    mau.Ten,
    p.[MaXuongSanXuat]
order by
    p.Ngay";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@toDate", toDate.Date);
                // cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #region Chắt làm báo cáo phụ phẩm PT
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"SELECT
    p.Ngay,
    cast(p.ThoiGianCan as datetime) as [ThoiGian],
	p.NgayCan,
    p.MaPhuongTien,
    pt.[Ten] as [TenPhuongTien],
    kh.[Ten] as [NhaMuaHang],
    la.[Ten] as [LoaiCa],
    tp.[Ten] as [ThanhPham],
    s.[Ten] as [Size],
    mau.Ten as [Mau],
    p.[TrongLuong] as [TrongLuong],
    p.[MaXuongSanXuat] as [Xuong]
FROM
    PhieuCanPhuPham p,
    NhaMuaPhuPham kh,
	MaLoaiCaPhuPham la,
	MaThanhPhamPhuPham tp,
	MaSizePhuPham s,
	MaMauPhuPham mau,
	PhuongTienChoPhuPham pt
WHERE
    p.[SuDung] = 1
    and p.[TrongLuong] > 0
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.NhaMuaHang = kh.Ma
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaPhuongTien = pt.Ma
order by
    ThoiGianCan";
                //using var connection = new SqlConnection(connectionString);
                //using var cmd = new SqlCommand(query, connection);
                //cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                //cmd.Parameters.AddWithValue("@toDate", toDate.Date);
                //// cmd.Parameters.AddWithValue("@xuongId", xuongId);
                //connection.Open();
                //using var da = new SqlDataAdapter(cmd);
                //var dataTable = new DataTable();
                //da.Fill(dataTable);
                //return dataTable;

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate, toDate})
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopNhaMuaHang<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"SELECT
    p.Ngay,
    p.[MaXuongSanXuat] as Xuong,
    kh.[Ten] as NhaMuaHang,
    p.[MaPhuongTien] as MaPhuongTien,
pt.[Ten] as TenPhuongTien,
    la.[Ten] as LoaiCa,
    tp.[Ten] as ThanhPham,
    s.[Ten] as [Size],
    mau.Ten as Mau,
    Cast(SUM(p.TrongLuong) as decimal(18, 2)) as TrongLuong
FROM
    PhieuCanPhuPham p,
    NhaMuaPhuPham kh,
    MaLoaiCaPhuPham la,
    MaThanhPhamPhuPham tp,
    MaSizePhuPham s,
    MaMauPhuPham mau,
PhuongTienChoPhuPham pt
WHERE
    p.[SuDung] = 1
    and p.[TrongLuong] > 0
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.NhaMuaHang = kh.Ma
    and p.MaPhuongTien = pt.Ma
GROUP BY
    p.[Ngay],
    p.[MSL],
    p.[MaPhuongTien],
    la.[Ten],
    tp.[Ten],
    s.[Ten],
    mau.Ten,
    p.[MaXuongSanXuat],
	kh.Ten,
pt.Ten
order by
    p.Ngay,
    kh.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate, toDate })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPham<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"SELECT
    p.[Ngay],
    p.[MaXuongSanXuat] as Xuong,
    la.[Ten] as LoaiCa,
    tp.[Ten] as ThanhPham,
    s.[Ten] as [Size],
    mau.Ten as Mau,
    Cast(SUM(p.TrongLuong) as decimal(18, 2)) as TrongLuong,
    p.GhiChu
FROM
    PhieuCanPhuPham p,
    MaLoaiCaPhuPham la,
    MaThanhPhamPhuPham tp,
    MaSizePhuPham s,
    MaMauPhuPham mau
	
WHERE
    p.[SuDung] = 1
    and p.[TrongLuong] > 0
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
GROUP BY
    p.[Ngay],
    la.[Ten],
    tp.[Ten],
    s.[Ten],
    mau.Ten,
    p.[MaXuongSanXuat],
    p.GhiChu
order by
    p.Ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate, toDate })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        

        public List<T> Gets<T>(DateTime dateTime)
        {
            try
            {
                var query = @"SELECT
    p.*,
    ISNULL(tp.Ten, 'NONE') as ThanhPhamName,
	ISNULL(nm.Ten,'NONE') as NhaMuaHangName
from
    (
        Select
            *
        from
            PhieuCanPhuPham p
        Where
            p.Ngay = @ngay
    ) p
    left join MaThanhPhamPhuPham tp on p.MaLoaiThanhPham = tp.Ma
	left join NhaMuaPhuPham nm on p.NhaMuaHang = nm.Ma";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        //public List<T> Gets<T>(DateTime dateTime, string xuongId)

        //{
        //    try
        //    {
        //        var query = @"Select * from PhieuCanPhuPham where Ngay= @ngay and MaXuongSanXuat =@xuongId";
        //        using var connection = new SqlConnection(connectionString);
        //        connection.Open();
        //        var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId }).ToList();
        //        return items;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public List<T> Gets<T>(DateTime dateTime, string mayCanId)
        {
            try
            {
                var query =
                    @"Select * from PhieuCanPhuPham where Convert(date, NgayCan)= @ngay and MaMayTinhCan = @mayCanId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, mayCanId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> Gets<T>(DateTime datTime, string mayCanId, DateTime fromDateTime)
        {
            try
            {
                var query =
                    @"Select * from PhieuCanPhuPham where Convert(date, NgayCan)= @ngay and MaMayTinhCan = @mayCanId and NgayCan > @fromDateTime";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = datTime.Date, mayCanId, fromDateTime }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public double GetSanLuong(DateTime dateTime, string xuongId, string thanhPhamId)
        {
            try
            {
                var query =
                    "Select ISNULL(SUM(TrongLuong),0) from PhieuCanPhuPham where Ngay = @ngay  and SuDung = 1 and MaXuongSanXuat = @xuongId and MaLoaiThanhPham = @thanhPhamId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<double>(
                        query,
                        new { ngay = dateTime.Date, xuongId = xuongId, thanhPhamId = thanhPhamId });
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public double GetSanLuong(TimeSpan fromTime, TimeSpan toTime, DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select ISNULL(SUM(TrongLuong),0) from PhieuCanPhuPham where Ngay = @ngay and CONVERT(time,ThoiGianCan) between @fromTime and @toTime and SuDung = 1 and MaXuongSanXuat = @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<double>(
                        query,
                        new { ngay = dateTime.Date, fromTime = fromTime, toTime = toTime, xuongId = xuongId });
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public double GetSanLuong(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string thanhPhamId)
        {
            try
            {
                var query =
                    "Select ISNULL(SUM(TrongLuong),0) from PhieuCanPhuPham where Ngay = @ngay and CONVERT(time,ThoiGianCan) >= @fromTime and CONVERT(time,ThoiGianCan)< @toTime and SuDung = 1 and MaXuongSanXuat = @xuongId and MaLoaiThanhPham = @thanhPhamId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<double>(
                        query,
                        new

                        {
                            ngay = dateTime.Date,
                            fromTime = fromTime,
                            toTime = toTime,
                            xuongId = xuongId,
                            thanhPhamId = thanhPhamId
                        });
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

//        public IList<string> GetSqlsInBatches(IList<PhieuCanPhuPham> phieuCans)
//        {
//            var insertSql = @"INSERT INTO [dbo].[PhieuCanPhuPham]
//           ([MaMayTinhCan]
//           ,[MaUserCan]
//           ,[ThoiGianCan]
//           ,[Ngay]
//           ,[NhaMuaHang]
//           ,[MSL]
//           ,[MaPhuongTien]
//           ,[MaLoaiCa]
//           ,[MaLoaiThanhPham]
//           ,[MaSize]
//           ,[MaMau]
//           ,[TrongLuong]
//           ,[SuDung]
//           ,[GhiChu]
//           ,[MaXuongSanXuat]
//           ,[NgayCan],[TrongLuongTare])
//VALUES";
//            var valuesSql =
//                @"('{0}','{1}','{2}', '{3}', '{4}', '{5}', '{6}', N'{7}','{8}','{9}','{10}',{11}, {12}, '{13}', '{14}', '{15}','{16}')";
//            var batchSize = 1000;

//            var sqlsToExecute = new List<string>();
//            var numberOfBatches = (int)Math.Ceiling((double)phieuCans.Count / batchSize);

//            for (int i = 0; i < numberOfBatches; i++)
//            {
//                var phieuCanToInsert = phieuCans.Skip(i * batchSize).Take(batchSize);
//                var valuesToInsert = phieuCanToInsert.Select(
//                    x => string.Format(
//                        valuesSql,
//                        x.MaMayTinhCan,
//                        x.MaUserCan,
//                        x.ThoiGianCan.ToString(@"yyyy-MM-dd HH\:mm\:ss"),
//                        x.Ngay?.ToString("yyyy-MM-dd"),
//                        x.NhaMuaHang,
//                        x.MSL,
//                        x.MaPhuongTien,
//                        x.MaLoaiCa,
//                        x.MaLoaiThanhPham,
//                        x.MaSize,
//                        x.MaMau,
//                        x.TrongLuong,
//                        x.SuDung == true ? 1 : 0,
//                        x.GhiChu,
//                        x.MaXuongSanXuat,
//                        x.NgayCan.ToString(@"yyyy-MM-dd HH\:mm\:ss.fff"),x.TrongLuongTare));
//                sqlsToExecute.Add(insertSql + string.Join(",", valuesToInsert));
//            }

//            return sqlsToExecute;
//        }

     //   public int Insert<T>(T item)
     //   {
     //       try
     //       {
     //           var query = @"INSERT INTO [dbo].[PhieuCanPhuPham]
     //      ([MaMayTinhCan]
     //      ,[MaUserCan]
     //      ,[ThoiGianCan]
     //      ,[Ngay]
     //      ,[NhaMuaHang]
     //      ,[MSL]
     //      ,[MaPhuongTien]
     //      ,[MaLoaiCa]
     //      ,[MaLoaiThanhPham]
     //      ,[MaSize]
     //      ,[MaMau]
     //      ,[TrongLuong]
     //      ,[SuDung]
     //      ,[GhiChu]
     //      ,[MaXuongSanXuat],[NgayCan],[TrongLuongTare])
     //VALUES
     //      (@MaMayTinhCan
     //      ,@MaUserCan
     //      ,@ThoiGianCan
     //      ,@Ngay
     //      ,@NhaMuaHang
     //      ,@MSL
     //      ,@MaPhuongTien
     //      ,@MaLoaiCa
     //      ,@MaLoaiThanhPham
     //      ,@MaSize
     //      ,@MaMau
     //      ,@TrongLuong
     //      ,@SuDung
     //      ,@GhiChu
     //      ,@MaXuongSanXuat,@NgayCan,@TrongLuongTare)";
     //           using var connection = new SqlConnection(connectionString);
     //           connection.Open();
     //           var rows = connection.Execute(query, item);
     //           return rows;
     //       }
     //       catch (Exception)
     //       {
     //           throw;
     //       }
     //   }

        public int Insert<T>(List<T> items)
        {
            try
            {
                var query = @"INSERT INTO [dbo].[PhieuCanPhuPham]
           ([MaMayTinhCan]
           ,[MaUserCan]
           ,[ThoiGianCan]
           ,[Ngay]
           ,[NhaMuaHang]
           ,[MSL]
           ,[MaPhuongTien]
           ,[MaLoaiCa]
           ,[MaLoaiThanhPham]
           ,[MaSize]
           ,[MaMau]
           ,[TrongLuong]
           ,[SuDung]
           ,[GhiChu]
           ,[MaXuongSanXuat],[NgayCan],[TrongLuongTare])
     VALUES
           (@MaMayTinhCan
           ,@MaUserCan
           ,@ThoiGianCan
           ,@Ngay
           ,@NhaMuaHang
           ,@MSL
           ,@MaPhuongTien
           ,@MaLoaiCa
           ,@MaLoaiThanhPham
           ,@MaSize
           ,@MaMau
           ,@TrongLuong
           ,@SuDung
           ,@GhiChu
           ,@MaXuongSanXuat,@NgayCan,@TrongLuongTare)";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(query, items);
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public int InsertBatch(List<PhieuCanPhuPham> phieuCans)
        //{
        //    try
        //    {
        //        //var batches = ToolEx.DbExtensions.GetSqlsInBatches(phieuCans);
        //        var batches = GetSqlsInBatches(phieuCans);

        //        var row = 0;
        //        var database = new Modelv1.Dao.Database(connectionString);
        //        foreach (var batche in batches)
        //        {
        //            row += database.ExecuteNonQuery(batche);
        //        }

        //        return row;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        
//        public int UpdateDatabase()
//        {
//            try
//            {
//                //Thêm Cột Id Vào bảng MaLoaiCa trên máy cân đầu Ao
//                var query =
//                    @"DECLARE @tb varchar(30) = 'PhieuCanPhuPham' IF COL_LENGTH(@tb, 'TrongLuongTare') IS NULL BEGIN
//ALTER TABLE
//    PhieuCanPhuPham
//ADD
//    [TrongLuongTare] [decimal](18, 3) NOT NULL DEFAULT ((0))
//END

//";
//                var dao = new Modelv1.Dao.Database(connectionString);
//                return dao.ExecuteNonQuery(query);
//            }
//            catch (Exception exception)
//            {
//                throw new Exception(
//                    $@"Không thể cập nhật Cơ Sở Dữ Liệu vui lòng liên hệ PMS để được hổ trợ [PhieuCanPhuPham]. {Environment.NewLine}{exception.Message}");
//                //throw;
//            }
//        }
        public int Update<T>(List<T> items)
        {
            try
            {
                var query = @"UPDATE [dbo].[PhieuCanPhuPham]
   SET  [Ngay] = @Ngay
      ,[NhaMuaHang] = @NhaMuaHang
      ,[MSL] = @MSL
      ,[MaPhuongTien] = @MaPhuongTien
      ,[MaLoaiCa] = @MaLoaiCa
      ,[MaLoaiThanhPham] = @MaLoaiThanhPham
      ,[MaSize] = @MaSize
      ,[MaMau] = @MaMau
      ,[TrongLuong] = @TrongLuong
      ,[SuDung] = @SuDung
      ,[GhiChu] = @GhiChu
      ,[MaXuongSanXuat] = @MaXuongSanXuat,[TrongLuongTare] =@TrongLuongTare
 WHERE [MaMayTinhCan] = @MaMayTinhCan
       and [MaUserCan] = @MaUserCan
      and [NgayCan] = @NgayCan
      and [ThoiGianCan] = @ThoiGianCan
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(query, items);
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPham<T>(DateTime fromDate, DateTime ngay, string xuongId)
        {
            try
            {
                var query = @"SELECT
    p.[Ngay],
    p.[MaXuongSanXuat] as Xuong,
    la.[Ten] as LoaiCa,
	p.MaLoaiThanhPham as MaThanhPham,
    tp.[Ten] as ThanhPhamName,
    s.[Ten] as [Size],
    mau.Ten as Mau,
    Cast(SUM(p.TrongLuong) as decimal(18, 2)) as TrongLuong,
    p.GhiChu,
    p.MaXuongSanXuat as MaXuong
FROM
    PhieuCanPhuPham p,
    MaLoaiCaPhuPham la,
    MaThanhPhamPhuPham tp,
    MaSizePhuPham s,
    MaMauPhuPham mau
	
WHERE
    p.[SuDung] = 1
    and p.[TrongLuong] > 0
    and p.[Ngay] <= @ngay
    and p.Ngay >= @fromDate
	and p.MaXuongSanXuat = @xuongId
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
GROUP BY
    p.[Ngay],
    la.[Ten],
    tp.[Ten],
    s.[Ten],
    mau.Ten,
    p.[MaXuongSanXuat],
    p.GhiChu,
	p.MaLoaiThanhPham
order by
    p.Ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate = fromDate.Date, ngay = ngay.Date, xuongId })
                        .Result
                        .ToList();
                    return items;
                }
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
p.MaMayTinhCan,
p.MaUserCan,
p.ThoiGianCan,
p.Ngay,
p.NhaMuaHang,
nmh.Ten as NhaMuaHangName,
p.MSL,
p.MaPhuongTien,
pt.Ten as PhuongTienName,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaLoaiThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
p.TrongLuong,
p.SuDung,
p.GhiChu,
p.MaXuongSanXuat,
x.Ten as XuongName,
p.NgayCan,
p.TrongLuongTare
from PhieuCanPhuPham p
left join NhaMuaPhuPham nmh on p.NhaMuaHang = nmh.Ma
left join PhuongTienChoPhuPham pt on p.MaPhuongTien = pt.Ma
left join MaLoaiCaPhuPham lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamPhuPham tp on p.MaLoaiThanhPham = tp.Ma
left join MaSizePhuPham s on p.MaSize = s.Ma
left join MaMauPhuPham m on p.MaMau = m.Ma
left join XiNghiep x on p.MaXuongSanXuat = x.Ma
where p.NgayCan = @dateTime and p.MaXuong = @xuongId
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
