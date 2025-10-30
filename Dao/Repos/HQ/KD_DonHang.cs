using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class KD_DonHang
    {
        private readonly string connectionString;
        private string tableName = @"KD_DonHang";
        private readonly string qrDelete = @"DELETE FROM [dbo].[KD_DonHang]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[KD_DonHang]
           ([Ma]
           ,[Ten]
           ,[CreateDate]
           ,[Ngay]
           ,[IsCompleted]
           ,[SuDung])
     VALUES
           (@Ma
           ,@Ten
           ,@CreateDate
           ,@Ngay
           ,@IsCompleted
           ,@SuDung)";

        private readonly string qrUpdate = @"UPDATE [dbo].[KD_DonHang]
   SET [Ten] = @Ten
      ,[CreateDate] = @CreateDate
      ,[Ngay] = @Ngay
      ,[IsCompleted] = @IsCompleted
      ,[SuDung] = @SuDung
 WHERE [Ma] = @Ma";

        private readonly string qrGetAll = "Select * from KD_DonHang";

        public KD_DonHang()
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
