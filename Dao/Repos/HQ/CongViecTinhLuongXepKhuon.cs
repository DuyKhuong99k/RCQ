using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class CongViecTinhLuongXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"CongViecTinhLuongXepKhuon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[CongViecTinhLuongXepKhuon]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = "INSERT INTO [dbo].[CongViecTinhLuongXepKhuon] ([Ma] ,[Ten] ,[BravoId] ,[SuDung] ,[LoaiDuLieu]) VALUES (@Ma, @Ten, @BravoId, @SuDung, @LoaiDuLieu)";

        private readonly string qrUpdate = @"UPDATE[dbo].[CongViecTinhLuongXepKhuon]
                SET [Ten] = @Ten
      ,[BravoId] = @BravoId
      ,[BravoIdDem] = @BravoIdDem
      ,[SuDung] = @SuDung
      ,[LoaiDuLieu] = @LoaiDuLieu
  WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from CongViecTinhLuongXepKhuon";

        public CongViecTinhLuongXepKhuon(string? _connectionString = null)
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
        public List<Models.Repos.Models.CongViecTinhLuongXepKhuon> Gets()
        {
            try
            {
                var query = "Select * from CongViecTinhLuongXepKhuon";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.CongViecTinhLuongXepKhuon>(query).Result.ToList();
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
