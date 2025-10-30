using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TheRo
    {
        private readonly string connectionString;
        private string tableName = @"TheRo";
        private readonly string qrDelete = @"DELETE FROM [dbo].[TheRo]
      WHERE [MaThe] = @MaThe ";

        private readonly string qrInsert = @"INSERT INTO [dbo].[TheRo]
           ([MaThe]
           ,[ColorCode],[CreatedDateTime],[IsRach],[MaThanhPhamDinhHinh])
     VALUES
           (@MaThe
           ,@ColorCode,@CreatedDateTime,@IsRach,@MaThanhPhamDinhHinh)";
        private readonly string qrUpdate = @"UPDATE [dbo].[TheRo]
   SET [ColorCode] =@ColorCode,[IsRach] =@IsRach, [MaThanhPhamDinhHinh]=@MaThanhPhamDinhHinh
 WHERE  [MaThe] = @MaThe ";

        private readonly string qrGetAll = "Select * from TheRo";

        public TheRo()
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
        public int Update<T>(List<T> items)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdate, items);
            return rows;
        }
    }
}
