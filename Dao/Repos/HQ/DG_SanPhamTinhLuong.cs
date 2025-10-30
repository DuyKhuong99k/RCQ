using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class DG_SanPhamTinhLuong
    {
        private readonly string connectionString;
        private string tableName = @"DG_SanPhamTinhLuong";
        private readonly string qrDelete = @"DELETE FROM [dbo].[DG_SanPhamTinhLuong]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[DG_SanPhamTinhLuong]
           ([Ma]
           ,[Ten]
           ,[GhiChu])
     VALUES
           (@Ma 
           ,@Ten 
           ,@GhiChu)";

        private readonly string qrUpdate = @"UPDATE [dbo].[DG_SanPhamTinhLuong]
   SET [Ten] = @Ten 
      ,[GhiChu] = @GhiChu 
 WHERE [Ma] = @Ma ";

        private readonly string qrGetAll = "Select * from DG_SanPhamTinhLuong";

        public DG_SanPhamTinhLuong()
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
