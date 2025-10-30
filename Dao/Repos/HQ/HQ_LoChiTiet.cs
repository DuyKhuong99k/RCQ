using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class HQ_LoChiTiet
    {
        private readonly string connectionString;
        private string tableName = @"HQ_LoChiTiet";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_LoChiTiet]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[HQ_LoChiTiet]
           ([Ma]
           ,[Ten]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[HQ_LoChiTiet]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from HQ_LoChiTiet";

        public HQ_LoChiTiet()
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
