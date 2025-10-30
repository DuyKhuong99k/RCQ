using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class HQ_DuyetThucDon
    {
        private readonly string connectionString;
        private string tableName = @"HQ_DuyetThucDon";
        //xóa theo thực đơn Id
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_DuyetThucDon]
      WHERE [ThucDonId] = @ThucDonId
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[HQ_DuyetThucDon]
           (
           [NgayTao]
           ,[NguoiTao]
           ,[ThucDonId]
           ,[GhiChu])
     VALUES
           (
           @NgayTao
           ,@NguoiTao
           ,@ThucDonId
           ,@GhiChu)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[HQ_DuyetThucDon]
   SET [NgayTao] = @NgayTao
           ,[NguoiTao] = @NguoiTao
           ,[ThucDonId] = @ThucDonId
           ,[GhiChu] = @GhiChu
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from HQ_DuyetThucDon";

        public HQ_DuyetThucDon()
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
