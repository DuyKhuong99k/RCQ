using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaCongViecTaiChe
    {
        private readonly string connectionString;
        private string tableName = @"MaCongViecTaiChe";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaCongViecTaiChe]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaCongViecTaiChe]
           ([Ma]
           ,[Ten]
           ,[SuDung],[BravoId]
           )
     VALUES
           (@Ma 
           ,@Ten 
           ,@SuDung ,@BravoId 
           )";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaCongViecTaiChe]
   SET [Ten] = @Ten 
      ,[SuDung] = @SuDung,[BravoId] = @BravoId 
 WHERE [Ma] = @Ma ";

        private readonly string qrGetAll = "Select * from MaCongViecTaiChe";

        public MaCongViecTaiChe()
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
