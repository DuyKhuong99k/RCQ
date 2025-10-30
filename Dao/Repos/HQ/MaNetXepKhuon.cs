using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaNetXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"MaNetXepKhuon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaNetXepKhuon]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaNetXepKhuon]
           ([Ma]
           ,[Ten]
           ,[TrongLuong]
           ,[BienDo]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@TrongLuong
           ,@BienDo
           ,@SuDung";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaNetXepKhuon]
   SET [Ten] = @Ten
      ,[TrongLuong] = @TrongLuong
      ,[BienDo] = @BienDo
      ,[SuDung] = @SuDung
 WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from MaNetXepKhuon";

        public MaNetXepKhuon()
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
