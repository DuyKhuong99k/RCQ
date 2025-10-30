using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class HQ_NhatKyDangKyMonAn
    {
        private readonly string connectionString;
        private string tableName = @"HQ_NhatKyDangKyMonAn";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_NhatKyDangKyMonAn]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[HQ_NhatKyDangKyMonAn]
           ([Id]
           ,[NgayGio]
           ,[NhanVienId]
           ,[ThietBi]
           ,[MonAnId]
           ,[GhiChu])
     VALUES
           (@Id
           ,@NgayGio
           ,@NhanVienId
           ,@ThietBi
           ,@MonAnId
           ,@GhiChu)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[HQ_NhatKyDangKyMonAn]
   SET [NgayGio] =@NgayGio
           ,[NhanVienId] = @NhanVienId
           ,[ThietBi] = @ThietBi
           ,[MonAnId] = @MonAnId
           ,[GhiChu] = @GhiChu
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from HQ_NhatKyDangKyMonAn";

        public HQ_NhatKyDangKyMonAn()
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
