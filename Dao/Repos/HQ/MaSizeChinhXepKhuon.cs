using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaSizeChinhXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"MaSizeChinhXepKhuon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaSizeChinhXepKhuon]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaSizeChinhXepKhuon]
           ([Ma]
           ,[Ten]
           ,[SuDung]
           ,[_type],[Idx])
     VALUES
           (@Ma 
           ,@Ten 
           ,@SuDung 
           ,@C_type,@Idx)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaSizeChinhXepKhuon]
   SET [Ten] = @Ten  
      ,[SuDung] = @SuDung 
      ,[_type] = @C_type,[Idx] =@Idx
 WHERE  [Ma] = @Ma";
        private readonly string qrGetAll = "Select * from MaSizeChinhXepKhuon";

        public MaSizeChinhXepKhuon()
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
