using Dapper;
using Microsoft.Data.SqlClient;
namespace Dao.Repos.HQ
{
    public partial class DG_LoaiDonGia
    {
        private readonly string connectionString;
        private string tableName = @"DG_LoaiDonGia";
        private readonly string qrDelete = @"DELETE FROM [dbo].[DG_LoaiDonGia]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[DG_LoaiDonGia]
           ([Ma]
           ,[Ten]
           ,[DienGiai])
     VALUES
           (@Ma 
           ,@Ten 
           ,@DienGiai)";

        private readonly string qrUpdate = @"UPDATE [dbo].[DG_LoaiDonGia]
   SET [Ten] = @Ten 
      ,[DienGiai] = @DienGiai 
 WHERE [Ma] = @Ma ";

        private readonly string qrGetAll = "Select * from DG_LoaiDonGia";

        public DG_LoaiDonGia()
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
