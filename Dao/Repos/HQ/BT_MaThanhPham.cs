using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class BT_MaThanhPham
    {
        private readonly string connectionString;
        private string tableName = @"BT_MaThanhPham";
        private readonly string qrDelete = @"DELETE FROM [dbo].[BT_MaThanhPham]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[BT_MaThanhPham]
           ([Ma]
           ,[Ten]
           ,[X]
           ,[Y]
           ,[SuDung]
           ,[BravoId])
     VALUES
           (@Ma
           ,@Ten
           ,@X
           ,@Y
           ,@SuDung
           ,@BravoId)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[BT_MaThanhPham]
   SET [Ten] = @Ten
      ,[X] = @X
      ,[Y] = @Y
      ,[SuDung] = @SuDung
      ,[BravoId] = @BravoId
 WHERE [Ma] = @Ma 
";

        private readonly string qrGetAll = "Select * from BT_MaThanhPham";

        public BT_MaThanhPham()
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
