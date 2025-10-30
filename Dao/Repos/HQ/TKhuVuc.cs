using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TKhuVuc
    {
        private readonly string connectionString;
        private string tableName = @"TKhuVuc";
        private readonly string qrDelete = @"DELETE FROM [dbo].[TKhuVuc]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_KhuVuc]
           ([Ma]
           ,[Ten]
           ,[SuDung]
           ,[GhiChu])
     VALUES
           (@Ma 
           ,@Ten 
           ,@SuDung 
           ,@GhiChu)";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_KhuVuc]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
      ,[GhiChu] = @GhiChu
 WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from TKhuVuc";

        public TKhuVuc()
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
