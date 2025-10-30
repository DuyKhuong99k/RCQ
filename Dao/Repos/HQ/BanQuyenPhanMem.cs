using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class BanQuyenPhanMem
    {
        private readonly string connectionString;
        private string tableName = @"BanQuyenPhanMem";
        private readonly string qrDelete = @"DELETE FROM [dbo].[BanQuyenPhanMem]
      WHERE [MaMayTinh] = @MaMayTinh
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[BanQuyenPhanMem]
           ([MaMayTinh]
           ,[TenMayTinh])
     VALUES
           (@MaMayTinh
           ,@TenMayTinh)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[BanQuyenPhanMem]
   SET [TenMayTinh] = @TenMayTinh
 WHERE [MaMayTinh] = @MaMayTinh
";

        private readonly string qrGetAll = "Select * from BanQuyenPhanMem";

        public BanQuyenPhanMem()
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
