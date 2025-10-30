using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace Dao.Repos.HQ
{
    public partial class BoTriNhomSoChe
    {
        private readonly string connectionString;
        private string tableName = @"BoTriNhomSoChe";
        private readonly string qrDelete = @"DELETE FROM [dbo].[BoTriNhomSoChe]
      WHERE [MaNhanVien] = @MaNhanVien
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[BoTriNhomSoChe] ([MaNhanVien] ,[Ngay] ,[MaNhomSoChe] ,[MaXuong] ,[SoGio] ,[TyLeHuong] ,[TyLeTru]) VALUES (@MaNhanVien,@Ngay,@MaNhomSoChe,@MaXuong,@SoGio,@TyLeHuong,@TyLeTru)
";

        private readonly string qrUpdate = @"UPDATE [dbo].[BoTriNhomSoChe] SET [SoGio] = @SoGio,[TyLeHuong] = @TyLeHuong,[TyLeTru] = @TyLeTru WHERE [MaNhanVien] = @MaNhanVien and [Ngay] = @Ngay and [MaNhomSoChe] = @MaNhomSoChe and [MaXuong] = @MaXuong
";

        private readonly string qrGetAll = "Select * from BoTriNhomSoChe";

        public BoTriNhomSoChe(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;
        }
        public List<Models.Repos.Models.BoTriNhomSoChe> Gets(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = "Select * from BoTriNhomSoChe where Ngay = @ngay and MaXuong = @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.BoTriNhomSoChe>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId
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
        public List<Models.Repos.Models.BoTriNhomSoChe> Gets(DateTime dateTime, string xuongId, string nhomId)
        {
            try
            {
                var query =
                    "Select * from BoTriNhomSoChe where Ngay = @ngay and MaXuong = @xuongId and MaNhomSoChe = @nhomId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<Models.Repos.Models.BoTriNhomSoChe>(
                        query,
                        new
                        {
                            ngay = dateTime.Date,
                            xuongId = xuongId,
                            nhomId = nhomId
                        })
                    .Result
                    .ToList();
                return items;
            }
            catch (Exception)
            {
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
