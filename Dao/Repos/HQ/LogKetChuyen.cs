using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models.Repos.Models;

namespace Dao.Repos.HQ
{
    public partial class LogKetChuyen
    {
        private readonly string connectionString;
        private string tableName = @"LogKetChuyen";
        private readonly string qrDelete = @"DELETE FROM [dbo].[LogKetChuyen] WHERE STT = @STT and Ngay =@Ngay and MaKhuVuc =@MaKhuVuc and [MaXuong] = @MaXuong";

        private readonly string qrInsert = @"INSERT INTO [dbo].[LogKetChuyen]
           ([STT]
           ,[MaKhuVuc]
           ,[Ngay]
           ,[PCName],[MaXuong])
     VALUES
           (@STT
           ,@MaKhuVuc
           ,@Ngay
           ,@PCName,@MaXuong)";

        private readonly string qrUpdate = @"UPDATE [dbo].[LogKetChuyen]
   SET [PCName] = @PCName
 WHERE [STT] = @STT
      and [MaKhuVuc] = @MaKhuVuc
      and [Ngay] = @Ngay and [MaXuong] = @MaXuong
      ";

        private readonly string qrGetAll = "Select * from LogKetChuyen";

        public LogKetChuyen(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

        }
        public LogKetChuyen Get(DateTime dateTime, string xuongId, string sanPhamId, int tab)
        {
            try
            {
                var query =
                    "Select * from LogKetChuyenBravo where Ngay = @ngay and MaSanPham=@sanPhamId and MaXuong=@xuongId and tab =@tab";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.QueryAsync<LogKetChuyen>(
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
    }
}
