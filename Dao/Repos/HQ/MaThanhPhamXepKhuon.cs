using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamXepKhuon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaThanhPhamXepKhuon]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPhamXepKhuon]
           ([Ma]
           ,[Ten]
           ,[SuDung]
           ,[Min]
           ,[Max]
           ,[BravoId])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung
           ,@Min
           ,@Max
           ,@BravoId)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPhamXepKhuon]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
      ,[Min] = @Min
      ,[Max] = @Max
      ,[BravoId] = @BravoId
 WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from MaThanhPhamXepKhuon";

        public MaThanhPhamXepKhuon()
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
