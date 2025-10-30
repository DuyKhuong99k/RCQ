using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanCaChetDaiThanhSide
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanCaChetDaiThanhSide";
        private readonly string qrDelete = @"
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanCaChetDaiThanhSide]
           ([STT]
           ,[Ngay]
           ,[MaMayCan]
           ,[Gio]
           ,[MaUserCan]
           ,[GhiChu]
           ,[TenLoaiCa]
           ,[TenKhachHang]
           ,[TenThongKe]
           ,[TenAo]
           ,[TrongLuong]
           ,[TrongLuongBinhQuan]
           ,[TrongLuongTare]
           ,[BiosId]
           ,[GioTai]
           ,[NgayTai])
     VALUES
           (@STT
           ,@Ngay
           ,@MaMayCan
           ,@Gio
           ,@MaUserCan
           ,@GhiChu
           ,@TenLoaiCa
           ,@TenKhachHang
           ,@TenThongKe
           ,@TenAo
           ,@TrongLuong
           ,@TrongLuongBinhQuan
           ,@TrongLuongTare
           ,@BiosId
           ,@GioTai
           ,@NgayTai )";

        private readonly string qrUpdate = @"

";

        private readonly string qrGetAll = "Select * from PhieuCanCaChetDaiThanhSide";

        public PhieuCanCaChetDaiThanhSide()
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
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate, string timeString)
        {
            try
            {
                var query = @"Select
    p.*,
case
        when p.Gio >= @time then 'Chieu'
        else 'Sang'
    end as Buoi
from
    PhieuCanCaChetDaiThanhSide p
where
    Ngay >= @fromDate
    and Ngay <= @toDate
order by
    STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, time = timeString })
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
        public List<T> GetTongHops<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"Select
    p.Ngay,
    p.TenThongKe,
    p.TenKhachHang,
    p.TenLoaiCa,
    p.TenAo,
    Sum(p.TrongLuong) as TrongLuong,
    COUNT(*) as SoRo
from
    PhieuCanCaChetDaiThanhSide p
WHERE
    p.Ngay >= @fromDate
    and p.Ngay <= @toDate
GROUP BY
    p.Ngay,
    p.TenThongKe,
    p.TenKhachHang,
    p.TenLoaiCa,
    p.TenAo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date })
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
    }
}
