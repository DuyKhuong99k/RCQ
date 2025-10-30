using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamExDinhHinh
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamExDinhHinh";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaThanhPhamExDinhHinh]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[MaThanhPhamExDinhHinh]
           ([Ma]
           ,[Ten]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[MaThanhPhamExDinhHinh]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from MaThanhPhamExDinhHinh";

        public MaThanhPhamExDinhHinh(string? _connectionString = null)
        {
             connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

        }
        public List<Models.Repos.Models.MaThanhPhamExDinhHinh> GetMaThanhPhamExDinhHinhs()
        {
            try
            {
                var query = "Select * from MaThanhPhamExDinhHinh";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.MaThanhPhamExDinhHinh>(query).Result.ToList();
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
    }
}
