using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoModelV1.Dao
{
    public partial class PhieuCanFillet
    {
        private readonly string connectionString;
        public PhieuCanFillet(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionStringBravo;
        }
        public List<EF.PhieuCanFillet> Gets()
        {
            try
            {
                var query = "Select * from PhieuCanFillet";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<EF.PhieuCanFillet>(query).Result.ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public int GetsCount(DateTime dateTime)
        {
            try
            {
                var query = "Select IsNull(Count(*),0) from PhieuCanFillet Where Ngay=@ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<int>(query, new { ngay = dateTime.Date });
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<EF.PhieuCanFillet> Gets(DateTime dateTime)
        {
            try
            {
                var query = "Select * from PhieuCanFillet Where Ngay=@ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<EF.PhieuCanFillet>(query, new { ngay = dateTime.Date }).Result
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

        public List<EF.PhieuCanFillet> Gets(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = "Select * from PhieuCanFillet where Ngay >= @fromDate and Ngay<= @toDate";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<EF.PhieuCanFillet>(
                            query,
                            new { fromDate = fromDate, toDate = toDate })
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

        public DataTable GetTongHopSanPhamsPV(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = $@"declare @DynamicPivotQuery as nvarchar(Max);
declare @ColumnName as nvarchar(Max) ;
declare @PivotSelectColumnNames AS NVARCHAR(MAX);
select @ColumnName = ISNULL(@ColumnName + ',','') +QUOTENAME(Ngay) from (select distinct Ngay from PhieuCanFillet where Ngay >= @fromDate and Ngay<= @toDate) as MS
SELECT @PivotSelectColumnNames 
    = ISNULL(@PivotSelectColumnNames + ',','')
    + 'ISNULL(' + QUOTENAME(Ngay) + ', 0) AS '
    + QUOTENAME(Ngay)
FROM (SELECT DISTINCT Ngay FROM PhieuCanFillet where Ngay >= @fromDate and Ngay<= @toDate) AS Courses order by Ngay
set @DynamicPivotQuery = N'Select re.MaSanPham as [Mã Sản Phẩm],TenSanPham as [Tên Sản Phẩm],CaLamViec as [Ca],total.TrongLuong as [Tổng Trọng Lượng],'+ @PivotSelectColumnNames  +'  from (Select p.Ngay,ca.Name as CaLamViec,p.MaSanPham,pro.Name as TenSanPham ,Sum(p.TrongLuong) as TrongLuong from PhieuCanFillet p,B20HrmShift ca,B20SalaryProduct pro where p.CaLamViec = ca.Code and p.Ngay >=''{fromDate.ToString("yyyy-MM-dd")}'' and p.Ngay<= ''{toDate.ToString("yyyy-MM-dd")}'' and p.MaSanPham = pro.Code group by p.Ngay,ca.Name ,p.MaSanPham,pro.Name) as tb pivot(sum ([TrongLuong]) for Ngay in (' + @ColumnName + ')) as re,(SELECT MaSanPham,SUM(TrongLuong) as TrongLuong FROM PhieuCanFillet where Ngay >= ''{fromDate.ToString("yyyy-MM-dd")}'' and Ngay<= ''{toDate.ToString("yyyy-MM-dd")}'' group by MaSanPham) total Where re.MaSanPham = total.MaSanPham order by re.MaSanPham' ;
EXEC sp_executesql @DynamicPivotQuery";
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@fromDate", fromDate);
                        cmd.Parameters.AddWithValue("@toDate", toDate);

                        connection.Open();
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            var dataTable = new DataTable();
                            da.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Delete(DateTime dateTime)
        {
            try
            {
                var query = "Delete [dbo].[PhieuCanFillet] where  [Ngay] =@Ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.Execute(query, new { ngay = dateTime.Date });
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Insert<T>(List<T> items)
        {
            try
            {
                var query = @"INSERT INTO [dbo].[PhieuCanFillet]
           ([Ngay]
           ,[CaLamViec]
           ,[MaNhanVien]
           ,[SuDung]
           ,[MaSanPham]
           ,[TenSanPham]
           ,[TrongLuong]
           ,[SoRo]
           ,[_Status]
           ,[CreatedAt])
     VALUES
           (@Ngay 
           ,@CaLamViec 
           ,@MaNhanVien 
           ,@SuDung 
           ,@MaSanPham 
           ,@TenSanPham 
           ,@TrongLuong 
           ,@SoRo 
           ,@Status 
           ,@CreatedAt )";
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

    }
}
