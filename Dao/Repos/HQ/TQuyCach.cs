using Microsoft.Data.SqlClient;
using System;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TQuyCach
    {
        private readonly string connectionString;
        private string tableName = @"T_QuyCach";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_QuyCach]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_QuyCach]
           ([Ma]
           ,[Ten]
           ,[MaKhuVuc]
           ,[SuDung],[TrongLuong])
     VALUES
           (@Ma
           ,@Ten
           ,@MaKhuVuc
           ,@SuDung,@TrongLuong)";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_QuyCach]
   SET [Ten] = @Ten
      ,[MaKhuVuc] = @MaKhuVuc
      ,[SuDung] = @SuDung, [TrongLuong] = @TrongLuong
 WHERE  [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from T_QuyCach";

        public TQuyCach()
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
