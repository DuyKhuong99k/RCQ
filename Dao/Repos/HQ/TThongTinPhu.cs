using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TThongTinPhu
    {
        private readonly string connectionString;
        private string tableName = @"T_ThongTinPhu";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_ThongTinPhu]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_ThongTinPhu]
           ([Ma]
           ,[Ten]
           ,[SuDung],[IsPhanCo],[IsBatMau])
     VALUES
           (@Ma 
           ,@Ten 
           ,@SuDung,@IsPhanCo,@IsBatMau )";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_ThongTinPhu]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung, [IsPhanCo] = @IsPhanCo, [IsBatMau] = @IsBatMau
 WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from T_ThongTinPhu";

        public TThongTinPhu()
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
