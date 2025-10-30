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
    public partial class PhieuCanKiemDinhHinh
    {
        private readonly string connectionString;
        public PhieuCanKiemDinhHinh(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionStringBravo;
        }
        public List<EF.PhieuCanKiemDinhHinh> Gets()
        {
            try
            {
                var query = "Select * from PhieuCanKiemDinhHinh";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<EF.PhieuCanKiemDinhHinh>(query).Result.ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<EF.PhieuCanKiemDinhHinh> Gets(DateTime dateTime)
        {
            try
            {
                var query = "Select * from PhieuCanKiemDinhHinh Where Ngay=@ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<EF.PhieuCanKiemDinhHinh>(query, new { ngay = dateTime.Date })
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

        public List<EF.PhieuCanKiemDinhHinh> Gets(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = "Select * from PhieuCanKiemDinhHinh where Ngay >= @fromDate and Ngay<= @toDate";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<EF.PhieuCanKiemDinhHinh>(
                            query,
                            new
                            {
                                fromDate = fromDate,
                                toDate = toDate
                            })
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

        public List<EF.PhieuCanKiemDinhHinh> Gets(DateTime fromDate, DateTime toDate, IEnumerable<string> sanPhamIds)
        {
            try
            {
                var listOfIdsJoined = $@"('{string.Join("','", sanPhamIds.ToArray())}')";
                var query =
                    $@"Select * from PhieuCanKiemDinhHinh where Ngay >= @fromDate and Ngay<= @toDate and MaSanPham In {listOfIdsJoined}";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<EF.PhieuCanKiemDinhHinh>(
                            query,
                            new
                            {
                                fromDate = fromDate,
                                toDate = toDate
                            })
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

        public DataTable GetTongHopSanPhamsPV(DateTime fromDate, DateTime toDate, int khuVucId)
        {
            try
            {
                var query = $@"declare @DynamicPivotQuery as nvarchar(Max);
declare @ColumnName as nvarchar(Max) ;
declare @PivotSelectColumnNames AS NVARCHAR(MAX);
select @ColumnName = ISNULL(@ColumnName + ',','') +QUOTENAME(Ngay) from (select distinct Ngay from PhieuCanKiemDinhHinh where Ngay >= @fromDate and Ngay<= @toDate and KhuVuc =@khuVuc) as MS
SELECT @PivotSelectColumnNames 
    = ISNULL(@PivotSelectColumnNames + ',','')
    + 'ISNULL(' + QUOTENAME(Ngay) + ', 0) AS '
    + QUOTENAME(Ngay)
FROM (SELECT DISTINCT Ngay FROM PhieuCanKiemDinhHinh where Ngay >= @fromDate and Ngay<= @toDate and KhuVuc =@khuVuc) AS Courses order by Ngay
set @DynamicPivotQuery = N'Select re.MaSanPham as [Mã Sản Phẩm],re.TenSanPham as [Tên Sản Phẩm],CaLamViec as [Ca],total.TrongLuong as [Tổng Trọng Lượng],'+ @PivotSelectColumnNames  +'  from (Select p.Ngay,ca.Name as CaLamViec,p.MaSanPham,p.TenSanPham,Sum(p.TrongLuong) as TrongLuong from PhieuCanKiemDinhHinh p,B20HrmShift ca where p.CaLamViec = ca.Code and p.Ngay >=''{fromDate.ToString("yyyy-MM-dd")}'' and p.Ngay<= ''{toDate.ToString("yyyy-MM-dd")}''and KhuVuc=''{khuVucId.ToString("00")}'' group by p.Ngay,ca.Name ,p.MaSanPham,p.TenSanPham) as tb pivot(sum ([TrongLuong]) for Ngay in (' + @ColumnName + ')) as re,(SELECT MaSanPham,TenSanPham,SUM(TrongLuong) as TrongLuong FROM PhieuCanKiemDinhHinh where Ngay >=''{fromDate.ToString("yyyy-MM-dd")}'' and Ngay<= ''{toDate.ToString("yyyy-MM-dd")}'' and KhuVuc=''{khuVucId.ToString("00")}'' group by MaSanPham,TenSanPham) total where re.MaSanPham = total.MaSanPham and re.TenSanPham = total.TenSanPham order by re.MaSanPham' ;
EXEC sp_executesql @DynamicPivotQuery";
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@fromDate", fromDate);
                        cmd.Parameters.AddWithValue("@toDate", toDate);
                        cmd.Parameters.AddWithValue("@khuVuc", khuVucId);

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

        public DataTable GetTongHopSanPhamsPV(DateTime fromDate, DateTime toDate, IEnumerable<string> sanPhamIds)
        {
            try
            {
                var listOfIdsJoined = $@"(''{string.Join("'',''", sanPhamIds.ToArray())}'')";
                var listOfIdsJoined2 = $@"('{string.Join("','", sanPhamIds.ToArray())}')";
                var query = $@"declare @DynamicPivotQuery as nvarchar(Max);
declare @ColumnName as nvarchar(Max) ;
declare @PivotSelectColumnNames AS NVARCHAR(MAX);
select @ColumnName = ISNULL(@ColumnName + ',','') +QUOTENAME(Ngay) from (select distinct Ngay from PhieuCanKiemDinhHinh where Ngay >= @fromDate and Ngay<= @toDate and MaSanPham in {listOfIdsJoined2}) as MS
SELECT @PivotSelectColumnNames 
    = ISNULL(@PivotSelectColumnNames + ',','')
    + 'ISNULL(' + QUOTENAME(Ngay) + ', 0) AS '
    + QUOTENAME(Ngay)
FROM (SELECT DISTINCT Ngay FROM PhieuCanKiemDinhHinh where Ngay >= @fromDate and Ngay<= @toDate and MaSanPham in {listOfIdsJoined2}) AS Courses order by Ngay
set @DynamicPivotQuery = N'Select re.MaSanPham as [Mã Sản Phẩm],TenSanPham as [Tên Sản Phẩm],CaLamViec as [Ca],total.TrongLuong as [Tổng Trọng Lượng],'+ @PivotSelectColumnNames  +'  from (Select p.Ngay,ca.Name as CaLamViec,p.MaSanPham,p.TenSanPham,Sum(p.TrongLuong) as TrongLuong from PhieuCanKiemDinhHinh p,B20HrmShift ca where p.CaLamViec = ca.Code and p.Ngay >=''{fromDate.ToString("yyyy-MM-dd")}'' and p.Ngay<= ''{toDate.ToString("yyyy-MM-dd")}''and MaSanPham in {listOfIdsJoined} group by p.Ngay,ca.Name ,p.MaSanPham,p.TenSanPham) as tb pivot(sum ([TrongLuong]) for Ngay in (' + @ColumnName + ')) as re,(SELECT MaSanPham,SUM(TrongLuong) as TrongLuong FROM PhieuCanKiemDinhHinh where Ngay >= ''{fromDate.ToString("yyyy-MM-dd")}'' and Ngay<= ''{toDate.ToString("yyyy-MM-dd")}'' and MaSanPham in {listOfIdsJoined} group by MaSanPham) total where re.MaSanPham = total.MaSanPham order by re.MaSanPham' ;
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

        public int Delete(DateTime dateTime, string khuVucId)
        {
            try
            {
                var query = "Delete [dbo].[PhieuCanKiemDinhHinh] where  [Ngay] =@Ngay and [KhuVuc] =@khuVucId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.Execute(query, new { ngay = dateTime.Date, khuVucId = khuVucId });
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
