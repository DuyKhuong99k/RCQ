using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaLoaiCaLangDa
    {
        private readonly string connectionString;
        private string tableName = @"MaLoaiCaLangDa";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaLoaiCaLangDa]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[MaLoaiCaLangDa]
           ([Ma]
           ,[Ten]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[MaLoaiCaLangDa]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from MaLoaiCaLangDa";

        public MaLoaiCaLangDa()
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
