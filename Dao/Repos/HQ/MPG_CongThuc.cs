using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MPG_CongThuc
    {
        private readonly string connectionString;
        private string tableName = @"MPG_CongThuc";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MPG_CongThuc]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MPG_CongThuc]
           ([Ma]
           ,[Ten]
           ,[TrongLuong]
           ,[CreateDate]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@TrongLuong
           ,@CreateDate
           ,@SuDung)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MPG_CongThuc]
   SET [Ten] = @Ten
      ,[TrongLuong] = @TrongLuong
      ,[CreateDate] = @CreateDate
      ,[SuDung] = @SuDung
 WHERE [Ma] = @Ma
      ";

        private readonly string qrGetAll = "Select * from MPG_CongThuc";

        public MPG_CongThuc()
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
