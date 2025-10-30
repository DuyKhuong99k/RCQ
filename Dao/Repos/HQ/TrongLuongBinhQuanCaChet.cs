using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TrongLuongBinhQuanCaChet
    {
        private readonly string connectionString;
        private string tableName = @"TrongLuongBinhQuanCaChet";
        private readonly string qrDelete = @"DELETE FROM [dbo].[TrongLuongBinhQuanCaChet]
      WHERE [Ngay] = @Ngay
      and [MaAo] = @MaAo";

        private readonly string qrInsert = @"INSERT INTO [dbo].[TrongLuongBinhQuanCaChet]
           ([Ngay]
           ,[MaAo]
           ,[TrongLuongBinhQuan]
           ,[CreateBy])
     VALUES
           (@Ngay
           ,@MaAo
           ,@TrongLuongBinhQuan
           ,@CreateBy)";

        private readonly string qrUpdate = @"UPDATE [dbo].[TrongLuongBinhQuanCaChet]
   SET [TrongLuongBinhQuan] = @TrongLuongBinhQuan
      ,[CreateBy] = @CreateBy
 WHERE [Ngay] = @Ngay
      and [MaAo] = @MaAo";

        private readonly string qrGetAll = "Select * from TrongLuongBinhQuanCaChet";

        public TrongLuongBinhQuanCaChet()
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
