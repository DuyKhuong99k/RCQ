using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace Dao.Repos.HQ
{
    public partial class CongViecTinhLuongXepKhuonSanLuong
    {
        private readonly string connectionString;
        private string tableName = @"CongViecTinhLuongXepKhuonSanLuong";
        private readonly string qrDelete = @"DELETE FROM [dbo].[CongViecTinhLuongXepKhuonSanLuong] WHERE [MaCongViec] = @MaCongViec and [Ngay]= @Ngay and [MaCa] = @MaCa and [MaXuong] = @MaXuong
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[CongViecTinhLuongXepKhuonSanLuong]([MaCongViec],[Ngay] ,[MaCa] ,[SanLuong],[MaXuong]) VALUES( @MaCongViec, @Ngay, @MaCa,@SanLuong,@MaXuong)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[CongViecTinhLuongXepKhuonSanLuong]
   SET [MaCongViec] = @MaCongViec,[Ngay] =@Ngay ,[MaCa]=@MaCa ,[SanLuong] = @SanLuong,[MaXuong] =@MaXuong
 WHERE [MaCongViec] = @MaCongViec and [Ngay]= @Ngay and [MaCa] = @MaCa and [MaXuong] = @MaXuong
";

        private readonly string qrGetAll = "Select * from CongViecTinhLuongXepKhuonSanLuong";


        public CongViecTinhLuongXepKhuonSanLuong(string? _connectionString = null)
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
        public List<Models.Repos.Models.CongViecTinhLuongXepKhuonSanLuong> Gets(DateTime dateTime, string xuongId, string congViecId)
        {
            try
            {
                var query =
                    "Select * from CongViecTinhLuongXepKhuonSanLuong Where Ngay =@ngay and MaXuong = @xuongId and MaCongViec = @congViecId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.CongViecTinhLuongXepKhuonSanLuong>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId,
                                congViecId = congViecId
                            })
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
        public List<T> GetsFullField<T>(DateTime dateTime, string maXuong)
        {
            try
            {
                var query = @"select
p.MaCongViec,
p.Ngay,
p.MaCa,
lc.Ten as LoaiCaName,
p.MaXuong,
x.Ten as XuongName,
p.SanLuong
from CongViecTinhLuongXepKhuonSanLuong p
left join CongViecTinhLuongXepKhuon cv on cv.Ma = p.MaCongViec
left join LoaiCaXepKhuon lc on lc.Ma = p.MaCa
left join XiNghiep x on x.Ma = p.MaXuong
where p.Ngay =@dateTime and p.MaXuong = @maXuong";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { dateTime = dateTime.Date, maXuong = maXuong }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
