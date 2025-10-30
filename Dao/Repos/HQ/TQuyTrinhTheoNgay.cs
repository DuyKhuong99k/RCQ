using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TQuyTrinhTheoNgay
    {
        private readonly string connectionString;
        private string tableName = @"T_QuyTrinhTheoNgay";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_QuyTrinhTheoNgay]
      WHERE  [Id] = @Id";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_QuyTrinhTheoNgay]
           ([Id]
           ,[Ngay]
           ,[MaQuyTrinh]
           ,[TrongLuong]
           ,[TheId])
     VALUES
           (@Id
           ,@Ngay
           ,@MaQuyTrinh
           ,@TrongLuong
           ,@TheId)
";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_QuyTrinhTheoNgay]
   SET [Ngay] = @Ngay
      ,[MaQuyTrinh] = @MaQuyTrinh
      ,[TrongLuong] = @TrongLuong
      ,[TheId] = @TheId
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from T_QuyTrinhTheoNgay";

        public TQuyTrinhTheoNgay()
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
