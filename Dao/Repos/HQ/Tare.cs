using Dapper;
using Microsoft.Data.SqlClient;
namespace Dao.Repos.HQ
{
    public partial class Tare
    {
        private readonly string connectionString;
        private string tableName = @"Tare";
        private readonly string qrDelete = @"DELETE FROM [dbo].[Tare]
      WHERE TrongLuong=@TrongLuong";

        private readonly string qrInsert = @"INSERT INTO [dbo].[Tare]
           ([TrongLuong],[TyLeBu]
           )
     VALUES
           (@TrongLuong ,@TyLeBu
          )";

        private readonly string qrUpdate = @"UPDATE [dbo].[Tare]
   SET [TyLeBu] = @TyLeBu
 WHERE [TrongLuong] = @TrongLuong
      
";

        private readonly string qrGetAll = "Select * from Tare";

        public Tare()
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
