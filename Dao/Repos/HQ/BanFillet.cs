using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class BanFillet
    {
        private readonly string connectionString;
        private string tableName = @"BanFillet";
        private readonly string qrDelete = @"DELETE FROM [dbo].[BanFillet]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[BanFillet]
           ([Ma]
           ,[Ten]
           ,[SuDung]
           ,[MaXuong])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung
           ,@MaXuong)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[BanFillet]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
      ,[MaXuong] = @MaXuong
 WHERE [Ma] = @Ma
";

        private readonly string qrGetAll = "Select * from BanFillet";

        public BanFillet()
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
