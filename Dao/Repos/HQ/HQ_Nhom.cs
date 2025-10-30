using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class HQ_Nhom
    {
        private readonly string connectionString;
        private string tableName = @"HQ_Nhom";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_Nhom]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[HQ_Nhom]
           ([Id]
           ,[Ten]
           ,[SuDung],[GhiChu])
     VALUES
           (@Id
           ,@Ten
           ,@SuDung,@GhiChu)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[HQ_Nhom]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung,[GhiChu] = @GhiChu
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from HQ_Nhom";

        public HQ_Nhom()
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
