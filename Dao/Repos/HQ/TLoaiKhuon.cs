using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TLoaiKhuon
    {
        private readonly string connectionString;
        private string tableName = @"T_LoaiKhuon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_LoaiKhuon]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_LoaiKhuon]
           ([Ma]
           ,[Ten]
           ,[MaKhuVuc]
           ,[TrongLuong]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@MaKhuVuc
           ,@TrongLuong
           ,@SuDung)";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_LoaiKhuon]
   SET [Ten] = @Ten
      ,[MaKhuVuc] = @MaKhuVuc
      ,[TrongLuong] = @TrongLuong
      ,[SuDung] = @SuDung
 WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from T_LoaiKhuon";

        public TLoaiKhuon()
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
