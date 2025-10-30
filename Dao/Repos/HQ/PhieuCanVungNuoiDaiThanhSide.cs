using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanVungNuoiDaiThanhSide
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanVungNuoiDaiThanhSide";
        private readonly string qrDelete = @"
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanVungNuoiDaiThanhSide]
           ([STT]
           ,[Ngay]
           ,[MaMayCan]
           ,[BiosId]
           ,[NgayTai]
           ,[GioTai]
           ,[Gio]
           ,[MaLo]
           ,[MaUserCan]
           ,[GhiChu]
           ,[MaLoaiCaDaiThanhId]
           ,[TenLoaiCa]
           ,[CanLai]
           ,[MaGhe]
           ,[TenGhe]
           ,[TenAo]
           ,[TenCongDoan]
           ,[TenThongKeDauAo]
           ,[TrongLuongTare]
           ,[TrongLuong],[IsTap])
     VALUES
           (@STT
           ,@Ngay
           ,@MaMayCan
           ,@BiosId
           ,@NgayTai
           ,@GioTai
           ,@Gio
           ,@MaLo
           ,@MaUserCan
           ,@GhiChu
           ,@MaLoaiCaDaiThanhId
           ,@TenLoaiCa
           ,@CanLai
           ,@MaGhe
           ,@TenGhe
           ,@TenAo
           ,@TenCongDoan
           ,@TenThongKeDauAo
           ,@TrongLuongTare
           ,@TrongLuong,@IsTap)";

        private readonly string qrUpdate = @"
";

        private readonly string qrGetAll = "Select * from PhieuCanVungNuoiDaiThanhSide";

        public PhieuCanVungNuoiDaiThanhSide(string? _connectionString = null)
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
        public int Insert(List<Models.Repos.Models.PhieuCanCaGiongVungNuoiDaiThanhSide> phieuCans)
        {
            var query = qrInsert;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.Execute(query, phieuCans);
                return items;
            }
        }
        public int Update<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdate, item);
            return rows;
        }
        public int Update<T>(List<T> items)
        {
            var query = qrUpdate;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, items);
            return rows;
        }
        public DataTable GetChiTiets(DateTime dateTime)
        {
            var query = @"Select
    STT,
    Ngay as [Ngày],
    MaMayCan as [Máy Cân],
     Cast(
        convert(
            varchar(19),
            cast(@ngay as datetime) + Cast(Gio as datetime),
            120
        ) as datetime
    ) as [Giờ],
    TenLoaiCa as [Loại Cá],
    MaGhe as [Mã Ghe],
    TenGhe as [Tên Ghe],
    TenAo As [Ao],
    TenCongDoan as [Công Đoàn],
    TenThongKeDauAo as [Thống Kê],
    TrongLuongTare as [Trọng Lượng Tare],
    TrongLuong as [Trọng Lượng Cân]
from
    PhieuCanVungNuoiDaiThanhSide
where
    Ngay = @ngay
    order by 
    STT DESC";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
            //cmd.Parameters.AddWithValue("@xuongId", xuongId);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }
        public List<T> GetChiTiets<T>(DateTime dateTime)
        {
            try
            {
                var query = @$"Select
    STT,
    Ngay,
    MaMayCan,
     Cast(
        convert(
            varchar(19),
            cast(@ngay as datetime) + Cast(Gio as datetime),
            120
        ) as datetime
    ) as Gio,
    MaLoaiCa,
    TenLoaiCa,
    MaGhe,
    TenGhe,
    TenAo,
    TenCongDoan,
    TenThongKeDauAo,
    TrongLuongTare,
    TrongLuong
from
    PhieuCanVungNuoiDaiThanhSide
where
    Ngay = @ngay
    order by 
    STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new
                    {
                        ngay = dateTime.Date
                    }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetChiTiets(DateTime fromDate, DateTime toDate)
        {
            var query = @"Select
    STT,
    Ngay as [Ngày],
    MaMayCan as [Máy Cân],
     Cast(
        convert(
            varchar(19),
            cast(Ngay as datetime) + Cast(Gio as datetime),
            120
        ) as datetime
    ) as [Giờ],
    TenLoaiCa as [Loại Cá],
    MaGhe as [Mã Ghe],
    TenGhe as [Tên Ghe],
    TenAo As [Ao],
    TenCongDoan as [Công Đoàn],
    TenThongKeDauAo as [Thống Kê],
    TrongLuongTare as [Trọng Lượng Tare],
    TrongLuong as [Trọng Lượng Cân]
from
    PhieuCanVungNuoiDaiThanhSide
where
    Ngay >= @fromDate
and Ngay <= @toDate
    order by 
    STT DESC";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
            cmd.Parameters.AddWithValue("@toDate", toDate.Date);
            //cmd.Parameters.AddWithValue("@xuongId", xuongId);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public int GetMaxSTT(DateTime dateTime, string biosId)
        {
            var query = @"Select
    Max(STT)
from
    PhieuCanVungNuoiDaiThanhSide
where
    Ngay = @ngay
    and BiosId =@biosId";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.ExecuteScalar<int>(query, new { ngay = dateTime.Date, biosId });
            return item;
        }

        public List<T> GetPhieuCanChiTiets<T>(DateTime dateTime)
        {
            var query =
                "select p.STT,p.Gio,p.TenAo as Ao,la.Ten as LoaiCaName,p.TenThongKeDauAo as ThongKeDauAo,p.TrongLuongTare,p.MaGhe as GheName,p.TenCongDoan as CongDoanName,p.TrongLuong from PhieuCanVungNuoiDaiThanhSide p,MaLoaiCaVungNuoi la where Ngay = @ngay and p.MaLoaiCaDaiThanhId = la.DaiThanhId order by STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date }).Result.ToList();
                return items;
            }
        }

        public List<T> Gets<T>(DateTime dateTime)
        {
            var query = @"Select * from PhieuCanVungNuoiDaiThanhSide
where
    Ngay = @ngay";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
            return rows;
        }

        public List<Models.Repos.Models.PhieuCanCaGiongVungNuoiDaiThanhSide> Gets(DateTime dateTime)
        {
            try
            {
                var query = "Select * from PhieuCanVungNuoiDaiThanhSide where Ngay =@ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.PhieuCanCaGiongVungNuoiDaiThanhSide>(
                            query,
                            new { ngay = dateTime.Date })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<Models.Repos.Models.PhieuCanCaGiongVungNuoiDaiThanhSide> Gets(DateTime dateTime, string mayCanId)
        {
            try
            {
                var query = "Select * from PhieuCanVungNuoiDaiThanhSide where Ngay =@ngay and MaMayCan = @mayCanId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<Models.Repos.Models.PhieuCanCaGiongVungNuoiDaiThanhSide>(
                        query,
                        new { ngay = dateTime.Date, mayCanId })
                    .Result
                    .ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        //public List<T> Gets<T>(DateTime dateTime,string biosId, int stt)
        //{
        //    try
        //    {
        //        var query = @"";
        //        using var connection = new SqlConnection(connectionString);
        //        connection.Open();
        //        var items = connection.Query<T>(query, new { ngay = dateTime.Date, biosId, stt }).ToList();
        //        return items;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        public List<Models.Repos.Models.PhieuCanCaGiongVungNuoiDaiThanhSide> GetsByBiosId(DateTime dateTime, string biosId)
        {
            try
            {
                var query = "Select * from PhieuCanVungNuoiDaiThanhSide where Ngay =@ngay and BiosId = @biosId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.PhieuCanCaGiongVungNuoiDaiThanhSide>(
                            query,
                            new { ngay = dateTime.Date, biosId })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        //   public IList<string> GetSqlsInBatches(
        //       IList<Models.Repos.Models.PhieuCanCaGiongVungNuoiDaiThanhSide> phieuCans,
        //       int batchSize = 500)
        //   {
        //       var insertSql = @"INSERT INTO [dbo].[PhieuCanVungNuoiDaiThanhSide]
        //      ([STT]
        //      ,[Ngay]
        //      ,[MaMayCan]
        //      ,[BiosId]
        //      ,[NgayTai]
        //      ,[GioTai]
        //      ,[Gio]
        //      ,[MaLo]
        //      ,[MaUserCan]
        //      ,[GhiChu]
        //      ,[MaLoaiCaDaiThanhId]
        //      ,[TenLoaiCa]
        //      ,[CanLai]
        //      ,[MaGhe]
        //      ,[TenGhe]
        //      ,[TenAo]
        //      ,[TenCongDoan]
        //      ,[TenThongKeDauAo]
        //      ,[TrongLuongTare]
        //      ,[TrongLuong],[IsTap])
        //VALUES";
        //       var valuesSql =
        //           @"({0},'{1}','{2}', '{3}', '{4}', '{5}', '{6}', '{7}','{8}','{9}','{10}',N'{11}', {12}, N'{13}', N'{14}', N'{15}',N'{16}',N'{17}',{18},{19},{20})";

        //       var sqlsToExecute = new List<string>();
        //       var numberOfBatches = (int)Math.Ceiling((double)phieuCans.Count / batchSize);

        //       for (var i = 0; i < numberOfBatches; i++)
        //       {
        //           var phieuCanToInsert = phieuCans.Skip(i * batchSize).Take(batchSize);
        //           var valuesToInsert = phieuCanToInsert.Select(
        //               x => string.Format(
        //                   valuesSql,
        //                   x.STT,
        //                   x.Ngay.ToString(@"yyyy-MM-dd"),
        //                   x.MaMayCan,
        //                   x.BiosId,
        //                   x.NgayTai.ToString(@"yyyy-MM-dd"),
        //                   x.GioTai.ToString(@"hh\:mm\:ss"),
        //                   x.Gio.ToString(@"hh\:mm\:ss"),
        //                   x.MaLo,
        //                   x.MaUserCan,
        //                   x.GhiChu,
        //                   x.MaLoaiCaDaiThanhId,
        //                   x.TenLoaiCa,
        //                   x.CanLai ? 1 : 0,
        //                   x.MaGhe,
        //                   x.TenGhe,
        //                   x.TenAo,
        //                   x.TenCongDoan,
        //                   x.TenThongKeDauAo,
        //                   x.TrongLuongTare,
        //                   x.TrongLuong,
        //                   x.IsTap ? 1 : 0));
        //           sqlsToExecute.Add(insertSql + string.Join(",", valuesToInsert));
        //       }

        //       return sqlsToExecute;
        //   }

        public DataTable GetTongHops(DateTime dateTime)
        {
            var query = @"Select
            p.Ngay,
            p.CanLai,
            p.IsTap,
            p.TenThongKeDauAo,
            p.MaLo,
            p.TenLoaiCa,
            p.MaGhe,
            p.TenGhe,
            p.TenAo,
            p.TenCongDoan,
            Min(p.STT) as MinSTT,
            Sum(p.TrongLuong) as TrongLuong,
            COUNT(*) as SoRo
        from
            PhieuCanVungNuoiDaiThanhSide p
        where
            p.Ngay = @ngay
        Group by
            p.Ngay,
            p.CanLai,
            p.IsTap,
            p.TenThongKeDauAo,
            p.MaLo,
            p.TenLoaiCa,
            p.MaGhe,
            p.TenGhe,
            p.TenAo,
            p.TenCongDoan";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
            //cmd.Parameters.AddWithValue("@xuongId", xuongId);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetTongHops(DateTime fromDate, DateTime toDate)
        {
            var query = @"Select
            p.Ngay,
            p.CanLai,
            p.IsTap,
            p.TenThongKeDauAo,
            p.MaLo,
            p.TenLoaiCa,
            p.MaGhe,
            p.TenGhe,
            p.TenAo,
            p.TenCongDoan,
            Min(p.STT) as MinSTT,
            Sum(p.TrongLuong) as TrongLuong,
            COUNT(*) as SoRo
        from
            PhieuCanVungNuoiDaiThanhSide p
        where
            p.Ngay >= @fromDate and p.Ngay <= @toDate
        Group by
            p.Ngay,
            p.CanLai,
            p.IsTap,
            p.TenThongKeDauAo,
            p.MaLo,
            p.TenLoaiCa,
            p.MaGhe,
            p.TenGhe,
            p.TenAo,
            p.TenCongDoan";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
            cmd.Parameters.AddWithValue("@toDate", toDate.Date);
            //cmd.Parameters.AddWithValue("@xuongId", xuongId);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public List<T> GetTongHops<T>(DateTime dateTime)
        {
            var query = @"Select
    p.Ngay,
    la.CanLai,
    la.IsTap,
    p.TenThongKeDauAo as ThongKeDauAo,
    p.MaLo,
    la.Ma as MaLoaiCa,
    la.Ten as LoaiCaName,
    p.MaGhe,
    p.MaGhe as GheName,
    p.MaAo,
    p.MaAo as AoName,
    p.TenCongDoan as MaCongDoan,
    p.TenCongDoan as CongDoanName,
    Min(p.STT) as MinSTT,
    Sum(p.TrongLuong) as TrongLuong,
    COUNT(*) as SoRo
from
    PhieuCanVungNuoiDaiThanhSide p,
    MaLoaiCaVungNuoi la
where
    p.Ngay = @ngay
    and p.MaLoaiCa = la.Ma
Group by
    p.Ngay,
    la.CanLai,
    la.IsTap,
    p.TenThongKeDauAo,
    p.MaLo,
    la.Ma,
    la.Ten,
    p.MaGhe,
    p.MaGhe,
    p.MaAo,
    p.MaAo,
    p.TenCongDoan,
    p.TenCongDoan
order by
    MinSTT";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date }).Result.ToList();
                return items;
            }
        }

        public DataTable GetTongHops_NgayBatCa(DateTime dateTime)
        {
            var query = @"DECLARE @columnName as NVARCHAR(max),
@PivotSelectColumnNames AS NVARCHAR(MAX),
@DynamicPivotQuery as nvarchar(Max);

SELECT
    @columnName = ISNULL(@columnName + ',', '') + QUOTENAME(ms.TenLoaiCa)
FROM
    (
        Select
            distinct p.TenLoaiCa
        from
            PhieuCanVungNuoiDaiThanhSide p
    ) as ms;

SELECT
    @PivotSelectColumnNames = ISNULL(@PivotSelectColumnNames + ',', '') + 'ISNULL(' + QUOTENAME(ms.TenLoaiCa) + ', 0) AS ' + QUOTENAME(ms.TenLoaiCa)
FROM
    (
        Select
            distinct p.TenLoaiCa
        from
            PhieuCanVungNuoiDaiThanhSide p
    ) as ms;
set @DynamicPivotQuery  =N'Select Ngay,TenAo,MaGhe,TenGhe, ApTai,TenThongKeDauAo,TenCongDoan,GioBatDau,NgayXuatPhat,GioXuatPhat, '+@PivotSelectColumnNames+' from (Select
    p.*,
    dAo.ApTai,
    Cast(
        convert(
            varchar(19),
            dAo.GioXuatPhat,
            120
        ) as datetime
    ) as GioXuatPhat,
    dAo.NgayXuatPhat
from
    (
        Select
            p.Ngay,
            p.MaGhe,
            p.TenGhe,
            p.TenAo,
            p.TenThongKeDauAo,
            p.TenCongDoan,
            p.TenLoaiCa,
            Cast(
                convert(
                    varchar(19),
                    MIN(p.Gio),
                    120
                ) as datetime
            ) as GioBatDau,
            Sum(p.TrongLuong) as TrongLuong
        from
            PhieuCanVungNuoiDaiThanhSide p
        where
            p.Ngay = @ngay
        Group by
            p.Ngay,
            p.TenThongKeDauAo,
            p.TenLoaiCa,
            p.MaGhe,
            p.TenGhe,
            p.TenAo,
            p.TenCongDoan
    ) p
    LEFT JOIN (
        Select
            dbo.non_unicode_convert(MaPhuongTien) as MaGhe,
            p.*
        from
            PhuongTien_TrongLuongDauAo p
        where
            p.NgayBatCa = @ngay
    ) dAo on p.MaGhe = dAo.MaGhe
    and p.Ngay = dAo.NgayBatCa
    and p.TenAo = dAo.MaAo) as p pivot (sum(p.TrongLuong) for P.TenLoaiCa in ('+@columnName+')) as re order by TenAo,NgayXuatPhat,GioXuatPhat,GioBatDau';
EXEC sp_executesql @DynamicPivotQuery,N'@ngay as DATE',@ngay;";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
            //cmd.Parameters.AddWithValue("@xuongId", xuongId);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetTongHops_NgayBatCa(DateTime fromDate, DateTime toDate)
        {
            var query = @"DECLARE @columnName as NVARCHAR(max),
@PivotSelectColumnNames AS NVARCHAR(MAX),
@DynamicPivotQuery as nvarchar(Max);

SELECT
    @columnName = ISNULL(@columnName + ',', '') + QUOTENAME(ms.TenLoaiCa)
FROM
    (
        Select
            distinct p.TenLoaiCa
        from
            PhieuCanVungNuoiDaiThanhSide p
    ) as ms;

SELECT
    @PivotSelectColumnNames = ISNULL(@PivotSelectColumnNames + ',', '') + 'ISNULL(' + QUOTENAME(ms.TenLoaiCa) + ', 0) AS ' + QUOTENAME(ms.TenLoaiCa)
FROM
    (
        Select
            distinct p.TenLoaiCa
        from
            PhieuCanVungNuoiDaiThanhSide p
    ) as ms;
set @DynamicPivotQuery  =N'Select Ngay,TenAo,MaGhe,TenGhe, ApTai,TenThongKeDauAo,TenCongDoan,GioBatDau,NgayXuatPhat,GioXuatPhat, '+@PivotSelectColumnNames+' from (Select
    p.*,
    dAo.ApTai,
    Cast(
        convert(
            varchar(19),
            dAo.GioXuatPhat,
            120
        ) as datetime
    ) as GioXuatPhat,
    dAo.NgayXuatPhat
from
    (
        Select
            p.Ngay,
            p.MaGhe,
            p.TenGhe,
            p.TenAo,
            p.TenThongKeDauAo,
            p.TenCongDoan,
            p.TenLoaiCa,
            Cast(
                convert(
                    varchar(19),
                    MIN(p.Gio),
                    120
                ) as datetime
            ) as GioBatDau,
            Sum(p.TrongLuong) as TrongLuong
        from
            PhieuCanVungNuoiDaiThanhSide p
        where
            p.Ngay >= @fromDate and p.Ngay <= @toDate
        Group by
            p.Ngay,
            p.TenThongKeDauAo,
            p.TenLoaiCa,
            p.MaGhe,
            p.TenGhe,
            p.TenAo,
            p.TenCongDoan
    ) p
    LEFT JOIN (
        Select
            dbo.non_unicode_convert(MaPhuongTien) as MaGhe,
            p.*
        from
            PhuongTien_TrongLuongDauAo p
        where
            p.NgayBatCa >= @fromDate and p.NgayBatCa <= @toDate
    ) dAo on p.MaGhe = dAo.MaGhe
    and p.Ngay = dAo.NgayBatCa
    and p.TenAo = dAo.MaAo) as p pivot (sum(p.TrongLuong) for P.TenLoaiCa in ('+@columnName+')) as re order by Ngay, TenAo,NgayXuatPhat,GioXuatPhat,GioBatDau';
EXEC sp_executesql @DynamicPivotQuery,N'@fromDate as DATE , @toDate as Date',@fromDate,@toDate;";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
            cmd.Parameters.AddWithValue("@toDate", toDate.Date);
            //cmd.Parameters.AddWithValue("@xuongId", xuongId);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetTongHops_NgayNhapXuong(DateTime dateTime)
        {
            var query = @"DECLARE @columnName as NVARCHAR(max),
@PivotSelectColumnNames AS NVARCHAR(MAX),
@DynamicPivotQuery as nvarchar(Max);

SELECT
    @columnName = ISNULL(@columnName + ',', '') + QUOTENAME(ms.TenLoaiCa)
FROM
    (
        Select
            distinct p.TenLoaiCa
        from
            PhieuCanVungNuoiDaiThanhSide p
    ) as ms;

SELECT
    @PivotSelectColumnNames = ISNULL(@PivotSelectColumnNames + ',', '') + 'ISNULL(' + QUOTENAME(ms.TenLoaiCa) + ', 0) AS ' + QUOTENAME(ms.TenLoaiCa)
FROM
    (
        Select
            distinct p.TenLoaiCa
        from
            PhieuCanVungNuoiDaiThanhSide p
    ) as ms;
set @DynamicPivotQuery  =N'Select
    Ngay,
    TenAo,
    MaGhe,
    TenGhe,
    ApTai,
    TenThongKeDauAo,
    TenCongDoan,
    GioBatDau,
    NgayXuatPhat,
    GioXuatPhat,
    '+@PivotSelectColumnNames+'
from
    (
        Select
            p.*,
            dAo.ApTai,
            Cast(
                convert(
                    varchar(19),
                    dAo.GioXuatPhat,
                    120
                ) as datetime
            ) as GioXuatPhat,
            dAo.NgayXuatPhat
        from
            (
                Select
                    p.Ngay,
                    p.MaGhe,
                    p.TenGhe,
                    p.TenAo,
                    p.TenThongKeDauAo,
                    p.TenCongDoan,
                    p.TenLoaiCa,
                    Cast(
                        convert(
                            varchar(19),
                            MIN(p.Gio),
                            120
                        ) as datetime
                    ) as GioBatDau,
                    Sum(p.TrongLuong) as TrongLuong
                from
                    PhieuCanVungNuoiDaiThanhSide p
                where
                    p.Ngay in (
                        Select
                            distinct NgayBatCa
                        from
                            PhuongTien_TrongLuongDauAo p
                        where
                            p.Ngay = @ngay
                    )
                Group by
                    p.Ngay,
                    p.TenThongKeDauAo,
                    p.TenLoaiCa,
                    p.MaGhe,
                    p.TenGhe,
                    p.TenAo,
                    p.TenCongDoan
            ) p
            LEFT JOIN (
                Select
                    dbo.non_unicode_convert(MaPhuongTien) as MaGhe,
                    p.*
                from
                    PhuongTien_TrongLuongDauAo p
                where
                    p.Ngay = @ngay
            ) dAo on p.MaGhe = dAo.MaGhe
            and p.Ngay = dAo.NgayBatCa
            and p.TenAo = dAo.MaAo
    ) as p pivot (
        sum(p.TrongLuong) for P.TenLoaiCa in ('+@columnName+')
    ) as re
order by
    TenAo,
    NgayXuatPhat,
    GioXuatPhat,
    GioBatDau';
EXEC sp_executesql @DynamicPivotQuery,N'@ngay as DATE',@ngay;";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
            //cmd.Parameters.AddWithValue("@xuongId", xuongId);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetTongHops_NgayNhapXuong(DateTime fromDate, DateTime toDate)
        {
            var query = @"DECLARE @columnName as NVARCHAR(max),
@PivotSelectColumnNames AS NVARCHAR(MAX),
@DynamicPivotQuery as nvarchar(Max);

SELECT
    @columnName = ISNULL(@columnName + ',', '') + QUOTENAME(ms.TenLoaiCa)
FROM
    (
        Select
            distinct p.TenLoaiCa
        from
            PhieuCanVungNuoiDaiThanhSide p
    ) as ms;

SELECT
    @PivotSelectColumnNames = ISNULL(@PivotSelectColumnNames + ',', '') + 'ISNULL(' + QUOTENAME(ms.TenLoaiCa) + ', 0) AS ' + QUOTENAME(ms.TenLoaiCa)
FROM
    (
        Select
            distinct p.TenLoaiCa
        from
            PhieuCanVungNuoiDaiThanhSide p
    ) as ms;
set @DynamicPivotQuery  =N'Select
    Ngay,
    TenAo,
    MaGhe,
    TenGhe,
    ApTai,
    TenThongKeDauAo,
    TenCongDoan,
    GioBatDau,
    NgayXuatPhat,
    GioXuatPhat,
    '+@PivotSelectColumnNames+'
from
    (
        Select
            p.*,
            dAo.ApTai,
            Cast(
                convert(
                    varchar(19),
                    dAo.GioXuatPhat,
                    120
                ) as datetime
            ) as GioXuatPhat,
            dAo.NgayXuatPhat
        from
            (
                Select
                    p.Ngay,
                    p.MaGhe,
                    p.TenGhe,
                    p.TenAo,
                    p.TenThongKeDauAo,
                    p.TenCongDoan,
                    p.TenLoaiCa,
                    Cast(
                        convert(
                            varchar(19),
                            MIN(p.Gio),
                            120
                        ) as datetime
                    ) as GioBatDau,
                    Sum(p.TrongLuong) as TrongLuong
                from
                    PhieuCanVungNuoiDaiThanhSide p
                where
                    p.Ngay in (
                        Select
                            distinct NgayBatCa
                        from
                            PhuongTien_TrongLuongDauAo p
                        where
                            p.Ngay >= @fromDate and p.Ngay <= @toDate
                    )
                Group by
                    p.Ngay,
                    p.TenThongKeDauAo,
                    p.TenLoaiCa,
                    p.MaGhe,
                    p.TenGhe,
                    p.TenAo,
                    p.TenCongDoan
            ) p
            LEFT JOIN (
                Select
                    dbo.non_unicode_convert(MaPhuongTien) as MaGhe,
                    p.*
                from
                    PhuongTien_TrongLuongDauAo p
                where
                    p.Ngay >= @fromDate and p.Ngay <= @toDate
            ) dAo on p.MaGhe = dAo.MaGhe
            and p.Ngay = dAo.NgayBatCa
            and p.TenAo = dAo.MaAo
    ) as p pivot (
        sum(p.TrongLuong) for P.TenLoaiCa in ('+@columnName+')
    ) as re
order by
    Ngay,
    TenAo,
    NgayXuatPhat,
    GioXuatPhat,
    GioBatDau';
EXEC sp_executesql @DynamicPivotQuery,N'@fromDate as DATE,@toDate as Date',@fromDate,@toDate;";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
            cmd.Parameters.AddWithValue("@toDate", toDate.Date);
            //cmd.Parameters.AddWithValue("@xuongId", xuongId);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public List<T> GetTongHopGhe<T>(
            DateTime fromDate,
            DateTime toDate)
        {
            try
            {
                var query = @"select p.MaGhe,
    p.TenGhe,
    sum(p.TrongLuong) as TrongLuong
from PhieuCanVungNuoiDaiThanhSide p,
    (
        Select Distinct 
            ptao.NgayBatCa as Ngay
        from PhuongTien_TrongLuongDauAo ptao
        where ptao.Ngay >= @fromDate
            and ptao.Ngay <= @toDate
    ) ptAo
where p.Ngay = ptAo.Ngay
group by p.MaGhe,
    p.TenGhe";
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
        #region Xử lý phiếu CÂN
        public List<T> GetPhieuCan<T>(DateTime dateTime)
        {
            try
            {
                var query = @"select
p.STT,
P.Ngay,
p.Gio,
p.NgayTai,
p.GioTai,
p.MaLo,
p.MaUserCan,
p.GhiChu,
p.MaLoaiCaDaiThanhId,
lc.Ten as LoaiCaVungNuoiName,
p.CanLai,
p.MaGhe,
pt.Ten as PhuongTienName,
p.TenAo,
p.TenCongDoan,
p.TenThongKeDauAo,
p.TrongLuong,
p.IsTap,
p.MaMayCan,
p.BiosId
from PhieuCanVungNuoiDaiThanhSide p
left join MaLoaiCaVungNuoi lc on p.MaLoaiCaDaiThanhId = lc.Ma
left join PhuongTienChoNguyenLieu pt on p.MaGhe = pt.Ma
where p.Ngay =@dateTime";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { dateTime }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"Select
    STT,
    Ngay,
    MaMayCan,
     Cast(
        convert(
            varchar(19),
            cast(Ngay as datetime) + Cast(Gio as datetime),
            120
        ) as datetime
    ) as Gio,
    TenLoaiCa,
    MaGhe,
    TenGhe,
    TenAo,
    TenCongDoan,
    TenThongKeDauAo,
    TrongLuongTare,
    TrongLuong
from
    PhieuCanVungNuoiDaiThanhSide
where
    Ngay >= @fromDate
and Ngay <= @toDate
    order by 
    STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date }).Result.ToList();
                    return items;
                }
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
                var query = @"    Select
            p.Ngay,
            p.CanLai,
            p.IsTap,
            p.TenThongKeDauAo,
            p.MaLo,
            p.TenLoaiCa,
            p.MaGhe,
            p.TenGhe,
            p.TenAo,
            p.TenCongDoan,
            Min(p.STT) as MinSTT,
            Sum(p.TrongLuong) as TrongLuong,
            COUNT(*) as SoRo
        from
            PhieuCanVungNuoiDaiThanhSide p
        where
            p.Ngay >= @fromDate and p.Ngay <= @toDate
        Group by
            p.Ngay,
            p.CanLai,
            p.IsTap,
            p.TenThongKeDauAo,
            p.MaLo,
            p.TenLoaiCa,
            p.MaGhe,
            p.TenGhe,
            p.TenAo,
            p.TenCongDoan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHops_NgayBatCa<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"DECLARE @columnName as NVARCHAR(max),
@PivotSelectColumnNames AS NVARCHAR(MAX),
@DynamicPivotQuery as nvarchar(Max);

SELECT
    @columnName = ISNULL(@columnName + ',', '') + QUOTENAME(ms.TenLoaiCa)
FROM
    (
        Select
            distinct p.TenLoaiCa
        from
            PhieuCanVungNuoiDaiThanhSide p
    ) as ms;

SELECT
    @PivotSelectColumnNames = ISNULL(@PivotSelectColumnNames + ',', '') + 'ISNULL(' + QUOTENAME(ms.TenLoaiCa) + ', 0) AS ' + QUOTENAME(ms.TenLoaiCa)
FROM
    (
        Select
            distinct p.TenLoaiCa
        from
            PhieuCanVungNuoiDaiThanhSide p
    ) as ms;
set @DynamicPivotQuery  =N'Select Ngay,TenAo,MaGhe,TenGhe, ApTai,TenThongKeDauAo,TenCongDoan,GioBatDau,NgayXuatPhat,GioXuatPhat, '+@PivotSelectColumnNames+' from (Select
    p.*,
    dAo.ApTai,
    Cast(
        convert(
            varchar(19),
            dAo.GioXuatPhat,
            120
        ) as datetime
    ) as GioXuatPhat,
    dAo.NgayXuatPhat
from
    (
        Select
            p.Ngay,
            p.MaGhe,
            p.TenGhe,
            p.TenAo,
            p.TenThongKeDauAo,
            p.TenCongDoan,
            p.TenLoaiCa,
            Cast(
                convert(
                    varchar(19),
                    MIN(p.Gio),
                    120
                ) as datetime
            ) as GioBatDau,
            Sum(p.TrongLuong) as TrongLuong
        from
            PhieuCanVungNuoiDaiThanhSide p
        where
            p.Ngay >= @fromDate and p.Ngay <= @toDate
        Group by
            p.Ngay,
            p.TenThongKeDauAo,
            p.TenLoaiCa,
            p.MaGhe,
            p.TenGhe,
            p.TenAo,
            p.TenCongDoan
    ) p
    LEFT JOIN (
        Select
            dbo.non_unicode_convert(MaPhuongTien) as MaGhe,
            p.*
        from
            PhuongTien_TrongLuongDauAo p
        where
            p.NgayBatCa >= @fromDate and p.NgayBatCa <= @toDate
    ) dAo on p.MaGhe = dAo.MaGhe
    and p.Ngay = dAo.NgayBatCa
    and p.TenAo = dAo.MaAo) as p pivot (sum(p.TrongLuong) for P.TenLoaiCa in ('+@columnName+')) as re order by Ngay, TenAo,NgayXuatPhat,GioXuatPhat,GioBatDau';
EXEC sp_executesql @DynamicPivotQuery,N'@fromDate as DATE , @toDate as Date',@fromDate,@toDate;";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHops_NgayNhapXuong<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"DECLARE @columnName as NVARCHAR(max),
@PivotSelectColumnNames AS NVARCHAR(MAX),
@DynamicPivotQuery as nvarchar(Max);

SELECT
    @columnName = ISNULL(@columnName + ',', '') + QUOTENAME(ms.TenLoaiCa)
FROM
    (
        Select
            distinct p.TenLoaiCa
        from
            PhieuCanVungNuoiDaiThanhSide p
    ) as ms;

SELECT
    @PivotSelectColumnNames = ISNULL(@PivotSelectColumnNames + ',', '') + 'ISNULL(' + QUOTENAME(ms.TenLoaiCa) + ', 0) AS ' + QUOTENAME(ms.TenLoaiCa)
FROM
    (
        Select
            distinct p.TenLoaiCa
        from
            PhieuCanVungNuoiDaiThanhSide p
    ) as ms;
set @DynamicPivotQuery  =N'Select
    Ngay,
    TenAo,
    MaGhe,
    TenGhe,
    ApTai,
    TenThongKeDauAo,
    TenCongDoan,
    GioBatDau,
    NgayXuatPhat,
    GioXuatPhat,
    '+@PivotSelectColumnNames+'
from
    (
        Select
            p.*,
            dAo.ApTai,
            Cast(
                convert(
                    varchar(19),
                    dAo.GioXuatPhat,
                    120
                ) as datetime
            ) as GioXuatPhat,
            dAo.NgayXuatPhat
        from
            (
                Select
                    p.Ngay,
                    p.MaGhe,
                    p.TenGhe,
                    p.TenAo,
                    p.TenThongKeDauAo,
                    p.TenCongDoan,
                    p.TenLoaiCa,
                    Cast(
                        convert(
                            varchar(19),
                            MIN(p.Gio),
                            120
                        ) as datetime
                    ) as GioBatDau,
                    Sum(p.TrongLuong) as TrongLuong
                from
                    PhieuCanVungNuoiDaiThanhSide p
                where
                    p.Ngay in (
                        Select
                            distinct NgayBatCa
                        from
                            PhuongTien_TrongLuongDauAo p
                        where
                            p.Ngay >= @fromDate and p.Ngay <= @toDate
                    )
                Group by
                    p.Ngay,
                    p.TenThongKeDauAo,
                    p.TenLoaiCa,
                    p.MaGhe,
                    p.TenGhe,
                    p.TenAo,
                    p.TenCongDoan
            ) p
            LEFT JOIN (
                Select
                    dbo.non_unicode_convert(MaPhuongTien) as MaGhe,
                    p.*
                from
                    PhuongTien_TrongLuongDauAo p
                where
                    p.Ngay >= @fromDate and p.Ngay <= @toDate
            ) dAo on p.MaGhe = dAo.MaGhe
            and p.Ngay = dAo.NgayBatCa
            and p.TenAo = dAo.MaAo
    ) as p pivot (
        sum(p.TrongLuong) for P.TenLoaiCa in ('+@columnName+')
    ) as re
order by
    Ngay,
    TenAo,
    NgayXuatPhat,
    GioXuatPhat,
    GioBatDau';
EXEC sp_executesql @DynamicPivotQuery,N'@fromDate as DATE,@toDate as Date',@fromDate,@toDate;";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
