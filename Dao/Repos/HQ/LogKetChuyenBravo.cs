using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class LogKetChuyenBravo
    {
        private readonly string connectionString;
        private string tableName = @"LogKetChuyenBravo";
        private readonly string qrDelete = @"Delete [dbo].[LogKetChuyenBravo] where [MaSanPham] =@MaSanPham and [Ngay] =@Ngay and [MaXuong] = @MaXuong and [tab] =@tab";

        private readonly string qrInsert = @"INSERT INTO [dbo].[LogKetChuyenBravo] ([MaSanPham] ,[Ngay] ,[Gio] ,[MaXuong],[NgayChuyen],[tab]) VALUES (@MaSanPham, @Ngay,@Gio,@MaXuong,@NgayChuyen,@tab)";

        private readonly string qrUpdate = @"";

        private readonly string qrGetAll = "Select * from LogKetChuyenBravo";

        public LogKetChuyenBravo(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;
        }
        public Models.Repos.Models.LogKetChuyenBravo Get(DateTime dateTime, string xuongId, string sanPhamId, int tab)
        {
            try
            {
                var query =
                    "Select * from LogKetChuyenBravo where Ngay = @ngay and MaSanPham=@sanPhamId and MaXuong=@xuongId and tab =@tab";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.QueryAsync<Models.Repos.Models.LogKetChuyenBravo>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                sanPhamId = sanPhamId,
                                xuongId = xuongId,
                                tab = tab
                            })
                        .Result
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Models.Repos.Models.LogKetChuyenBravo> Gets(DateTime dateTime)
        {
            try
            {
                var query = "Select * from LogKetChuyenBravo where Ngay = @ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.LogKetChuyenBravo>(query, new { ngay = dateTime.Date }).Result
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
        public int Delete(DateTime dateTime, int tab)
        {
            try
            {
                var query = "Delete [dbo].[LogKetChuyenBravo] where  [Ngay] =@Ngay and [tab] =@tab";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.Execute(query, new { ngay = dateTime.Date, tab = tab });
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
