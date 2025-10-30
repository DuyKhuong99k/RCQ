using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TSize
    {
        private readonly string connectionString;
        private string tableName = @"T_Size";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_Size]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_Size]
           ([Ma]
           ,[Ten]
           ,[MaKhuVuc]
           ,[SuDung],[Inx])
     VALUES
           (@Ma
           ,@Ten
           ,@MaKhuVuc
           ,@SuDung,@Inx)";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_Size]
   SET [Ten] = @Ten
      ,[MaKhuVuc] = @MaKhuVuc
      ,[SuDung] = @SuDung, [Inx] =@Inx
 WHERE [Ma] = @Ma
      ";

        private readonly string qrGetAll = "Select * from T_Size";

        public TSize()
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
