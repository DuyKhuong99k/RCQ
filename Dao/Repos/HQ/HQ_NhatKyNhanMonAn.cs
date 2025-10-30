using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class HQ_NhatKyNhanMonAn
    {
        private readonly string connectionString;
        private string tableName = @"HQ_NhatKyNhanMonAn";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_NhatKyNhanMonAn]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[HQ_NhatKyNhanMonAn]
           ([Id]
           ,[NgayGio]
           ,[NhanVienId]
           ,[ThietBi]
           ,[CodeId]
           ,[GhiChu])
     VALUES
           (@Id
           ,@NgayGio
           ,@NhanVienId
           ,@ThietBi
           ,@CodeId
           ,@GhiChu)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[HQ_NhatKyNhanMonAn]
   SET [NgayGio] =@NgayGio
           ,[NhanVienId] = @NhanVienId
           ,[ThietBi] = @ThietBi
           ,[CodeId] = @CodeId
           ,[GhiChu] = @GhiChu
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from HQ_NhatKyNhanMonAn";

        public HQ_NhatKyNhanMonAn()
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
