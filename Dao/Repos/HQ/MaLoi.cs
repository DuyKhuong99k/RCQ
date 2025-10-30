using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaLoi
    {
        private readonly string connectionString;
        private string tableName = @"MaLoi";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaLoi]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaLoi]
           ([Ma]
           ,[Ten]
           ,[MoTa]
           ,[LyDo]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@MoTa
           ,@LyDo
           ,@SuDung)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaLoi]
   SET [Ten] = @Ten
      ,[MoTa] = @MoTa
      ,[LyDo] = @LyDo
      ,[SuDung] = @SuDung
 WHERE [Ma] = @Ma
      ";

        private readonly string qrGetAll = "Select * from MaLoi";

        public MaLoi()
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
