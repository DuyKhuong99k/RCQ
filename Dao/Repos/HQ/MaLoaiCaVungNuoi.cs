using Dapper;
using Microsoft.Data.SqlClient;
namespace Dao.Repos.HQ
{
    public partial class MaLoaiCaVungNuoi
    {
        private readonly string connectionString;
        private string tableName = @"MaLoaiCaVungNuoi";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaLoaiCaVungNuoi]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[MaLoaiCaVungNuoi] ([Ma] ,[Ten] ,[SuDung],[CanLai],[IsTap]) VALUES (@Ma, @Ten,@SuDung,@CanLai,@IsTap)";

        private readonly string qrUpdate = @"UPDATE[dbo].[MaLoaiCaVungNuoi]
                SET [Ten] = @Ten
      ,[CanLai] = @CanLai
      ,[SuDung] = @SuDung
      ,[DaiThanhId] = @DaiThanhId
      ,[IsTap] = @IsTap
  WHERE [Ma] =@Ma";

        private readonly string qrGetAll = "Select * from MaLoaiCaVungNuoi";

        public MaLoaiCaVungNuoi()
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
