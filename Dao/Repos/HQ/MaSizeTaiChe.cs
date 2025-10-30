using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaSizeTaiChe
    {
        private readonly string connectionString;
        private string tableName = @"MaSizeTaiChe";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaSizeTaiChe]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaSizeTaiChe]
           ([Ma]
           ,[Ten]
           ,[SuDung],[_type]
           )
     VALUES
           (@Ma 
           ,@Ten 
           ,@SuDung , @_type
           )";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaSizeTaiChe]
   SET [Ten] = @Ten 
      ,[SuDung] = @SuDung ,[_type]=@_type
 WHERE [Ma] = @Ma ";

        private readonly string qrGetAll = "Select * from MaSizeTaiChe";

        public MaSizeTaiChe()
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
