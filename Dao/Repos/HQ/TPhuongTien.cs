using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TPhuongTien
    {
        private readonly string connectionString;
        private string tableName = @"T_PhuongTien";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_PhuongTien]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_PhuongTien]
           ([Ma]
           ,[Ten]
           ,[MaSo]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@MaSo
           ,@SuDung)";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_PhuongTien]
   SET [Ten] = @Ten
      ,[MaSo] = @MaSo
      ,[SuDung] = @SuDung
 WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from T_PhuongTien";

        public TPhuongTien()
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
