using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class HQ_Lo
    {
        private readonly string connectionString;
        private string tableName = @"HQ_Lo";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_Lo]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[HQ_Lo]
           ([Id]
           ,[NgayNguyenLieu]
           ,[SuDung]
           ,[MNgay])
     VALUES
           (@Id
           ,@NgayNguyenLieu
           ,@SuDung,@MNgay)";

        private readonly string qrUpdate = @"UPDATE [dbo].[HQ_Lo]
   SET [NgayNguyenLieu] = @NgayNguyenLieu
      ,[SuDung] = @SuDung
      ,[MNgay] = @MNgay
 WHERE [Id] = @Id";

        private readonly string qrGetAll = "Select * from HQ_Lo";

        public HQ_Lo()
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

        public T? Get<T>(string id)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.QueryFirstOrDefault<T>($"Select * from {tableName} where Id = @id", new { id });
            return item;
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
        public List<Tuple<DateTime?, string, string>> GetsMSLWithSize(string xuongId)
        {
            try
            {
                var query =
                    @"SELECT 
Top 10 
Cast( Max(ThoiGianCan)  as Date) as ThoiGianCanGanNhat,
MSL as HQ_Lo,
MaSize as MaSize
FROM PhieuCanNguyenLieu
Where 
MaXuongSanXuat = @xuongId 
GROUP BY MSL,MaSize 
ORDER BY Max(ThoiGianCan) 
DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                connection.Open();

                var items = connection.Query<Tuple<DateTime?, string, string>>(query, new { xuongId })
                    .ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
