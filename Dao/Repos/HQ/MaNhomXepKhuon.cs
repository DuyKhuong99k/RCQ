using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class MaNhomXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"MaNhomXepKhuon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaNhomXepKhuon]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO[dbo].[MaNhomXepKhuon] ([Ma] ,[Ten] ,[MaXuong] ,[GhiChu]) VALUES (@Ma,@Ten,@MaXuong, @GhiChu )";

        private readonly string qrUpdate = @"
UPDATE [dbo].[MaNhomXepKhuon]
   SET [Ten] = @Ten
      ,[MaXuong] = @MaXuong,[GhiChu] = @GhiChu
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from MaNhomXepKhuon";


        public MaNhomXepKhuon(string? _connectionString = null)
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
        public List<Models.Repos.Models.MaNhomXepKhuon> Gets(string xuongId)
        {
            try
            {
                var query = "Select * from MaNhomXepKhuon where MaXuong =@xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.MaNhomXepKhuon>(query, new { xuongId = xuongId }).Result
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
    }
}
