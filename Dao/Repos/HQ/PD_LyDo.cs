using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PD_LyDo
    {
        private readonly string connectionString;
        private string tableName = @"PD_LyDo";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PD_LyDo]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PD_LyDo]
           ([Ma]
           ,[Ten]
           ,[DienGiai]
           ,[GhiChu])
     VALUES
           (@Ma 
           ,@Ten 
           ,@DienGiai 
           ,@GhiChu)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PD_LyDo]
   SET [Ten] = @Ten 
      ,[DienGiai] = @DienGiai 
      ,[GhiChu] = @GhiChu
 WHERE [Ma] = @Ma ";

        private readonly string qrGetAll = "Select * from PD_LyDo";

        public PD_LyDo()
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
