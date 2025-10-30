using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaSizeXepKhuonBlock
    {
        private readonly string connectionString;
        private string tableName = @"MaSizeXepKhuonBlock";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaSizeXepKhuonBlock]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[MaSizeXepKhuonBlock]
           ([Ma]
           ,[Ten]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[MaSizeXepKhuonBlock]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from MaSizeXepKhuonBlock";

        public MaSizeXepKhuonBlock()
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
