using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class KD_Size
    {
        private readonly string connectionString;
        private string tableName = @"KD_Size";
        private readonly string qrDelete = @"DELETE FROM [dbo].[KD_Size]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[KD_Size]
           ([Ma]
           ,[Ten]
           ,[TrongLuong]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@TrongLuong
           ,@SuDung)";

        private readonly string qrUpdate = @"UPDATE [dbo].[KD_Size]
   SET [Ten] = @Ten
      ,[TrongLuong] = @TrongLuong
      ,[SuDung] = @SuDung
 WHERE [Ma] = @Ma
      ";

        private readonly string qrGetAll = "Select * from KD_Size";

        public KD_Size()
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
    }
}
