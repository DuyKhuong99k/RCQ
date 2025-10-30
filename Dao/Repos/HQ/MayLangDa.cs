using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MayLangDa
    {
        private readonly string connectionString;
        private string tableName = @"MayLangDa";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MayLangDa]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[MayLangDa]
           ([Ma]
           ,[Ten]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[MayLangDa]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from MayLangDa";

        public MayLangDa()
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
