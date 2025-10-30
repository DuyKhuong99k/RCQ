using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PMS_L
    {
        private readonly string connectionString;
        private string tableName = @"PMS_L";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PMS_L]
      WHERE STT=@STT";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PMS_L]
           ([STT]
           ,[SCode]
           ,[Par1]
           ,[Par2]
           ,[Par3]
           ,[Par4])
     VALUES
           (@STT
           ,@SCode
           ,@Par1
           ,@Par2
           ,@Par3
           ,@Par4)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PMS_L]
   SET 
      [SCode] = @SCode 
      ,[Par1] = @Par1 
      ,[Par2] = @Par2 
      ,[Par3] = @Par3 
      ,[Par4] = @Par4 
 WHERE [STT] = @STT";

        private readonly string qrGetAll = "Select * from PMS_L";

        public PMS_L()
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
