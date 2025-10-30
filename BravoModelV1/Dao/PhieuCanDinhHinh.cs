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
    public partial class PhieuCanDinhHinh
    {
        private readonly string connectionString;
        public PhieuCanDinhHinh(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionStringBravo;
        }
         public List<EF.PhieuCanDinhHinh> Gets()
        {
            try
            {
                var query = "Select * from PhieuCanDinhHinh";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<EF.PhieuCanDinhHinh>(query).Result.ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<EF.PhieuCanDinhHinh> Gets(DateTime dateTime)
        {
            try
            {
                var query = "Select * from PhieuCanDinhHinh where Ngay = @ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<EF.PhieuCanDinhHinh>(query, new { ngay = dateTime }).Result
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

        public List<EF.PhieuCanDinhHinh> Gets(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = "Select * from PhieuCanDinhHinh where Ngay >= @fromDate and Ngay<= @toDate";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<EF.PhieuCanDinhHinh>(
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

        public List<EF.PhieuCanDinhHinh> GetTongHopSanPhams(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query =
                    "Select p.Ngay,ca.Name as CaLamViec,p.MaSanPham,p.TenSanPham,p.DanhGia,Sum(p.TrongLuongNhan) as TrongLuongNhan,Sum(p.TrongLuongTra) as TrongLuongTra  from PhieuCanDinhHinh p,B20HrmShift ca where p.CaLamViec = ca.Code and p.Ngay >= @fromDate and p.Ngay<= @toDate group by p.Ngay,ca.Name ,p.MaSanPham,p.TenSanPham,p.DanhGia";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<EF.PhieuCanDinhHinh>(
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
select @ColumnName = ISNULL(@ColumnName + ',','') +QUOTENAME(Ngay) from (select distinct Ngay from PhieuCanDinhHinh where Ngay >= @fromDate and Ngay<= @toDate) as MS
SELECT @PivotSelectColumnNames 
    = ISNULL(@PivotSelectColumnNames + ',','')
    + 'ISNULL(' + QUOTENAME(Ngay) + ', 0) AS '
    + QUOTENAME(Ngay)
FROM (SELECT DISTINCT Ngay FROM PhieuCanDinhHinh where Ngay >= @fromDate and Ngay<= @toDate) AS Courses order by Ngay
set @DynamicPivotQuery = N'Select re.MaSanPham as [Mã Sản Phẩm],re.TenSanPham as [Tên Sản Phẩm],re.CaLamViec as [Ca],Case When re.DanhGia = 1 then N''Đạt'' else N''Rớt'' end as [Đánh Giá],total.TongTrongLuong as [Tổng Trọng Lượng],'+ @PivotSelectColumnNames  +'  from (Select p.Ngay,ca.Name as CaLamViec,p.MaSanPham,p.TenSanPham,p.DanhGia,Sum(p.TrongLuongTra) as TrongLuong from PhieuCanDinhHinh p,B20HrmShift ca where p.CaLamViec = ca.Code and p.Ngay >=''{fromDate.ToString("yyyy-MM-dd")}'' and p.Ngay<= ''{toDate.ToString("yyyy-MM-dd")}'' group by p.Ngay,ca.Name ,p.MaSanPham,p.TenSanPham,p.DanhGia) as tb pivot(sum ([TrongLuong]) for Ngay in (' + @ColumnName + ')) as re,(select MaSanPham,DanhGia,SUM(TrongLuongTra) AS TongTrongLuong from PhieuCanDinhHinh where Ngay >= ''{fromDate.ToString("yyyy-MM-dd")}'' and Ngay<= ''{toDate.ToString("yyyy-MM-dd")}'' group by MaSanPham,DanhGia) total where re.MaSanPham = total.MaSanPham and re.DanhGia = total.DanhGia order by re.MaSanPham' ;
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
                var query = "Delete [dbo].[PhieuCanDinhHinh] where  [Ngay] =@Ngay";
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

        //public List<EF.PhieuCanDinhHinh> GetTongHopSanPhams(DateTime fromDate, DateTime toDate)
        //{
        //    try
        //    {
        //        var query = "Select p.Ngay,ca.Name as CaLamViec,p.MaSanPham,p.TenSanPham,p.DanhGia,Sum(p.TrongLuongNhan) as TrongLuongNhan,Sum(p.TrongLuongTra) as TrongLuongTra  from PhieuCanDinhHinh p,B20HrmShift ca where p.CaLamViec = ca.Code and p.Ngay >= @fromDate and p.Ngay<= @toDate group by p.Ngay,ca.Name ,p.MaSanPham,p.TenSanPham,p.DanhGia";
        //        using (var connection = new SqlConnection(ConnectionString))
        //        {
        //            connection.Open();
        //            var items = connection.QueryAsync<EF.PhieuCanDinhHinh>(query,
        //                                                                   new { fromDate = fromDate, toDate = toDate })
        //                .Result
        //                .ToList();
        //            return items;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}
