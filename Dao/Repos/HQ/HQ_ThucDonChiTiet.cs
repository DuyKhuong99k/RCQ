using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class HQ_ThucDonChiTiet
    {
        private readonly string connectionString;
        private string tableName = @"HQ_ThucDonChiTiet";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_ThucDonChiTiet]
      WHERE [Id] = @Id
";
        private readonly string qrDeleteByThucDonId = @"DELETE FROM [dbo].[HQ_ThucDonChiTiet]
      WHERE [ThucDonId] = @ThucDonId
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[HQ_ThucDonChiTiet]
           (
           [ThucDonId]
           ,[MonAnId]
           ,[GhiChu])
     VALUES
           (
           @ThucDonId
           ,@MonAnId
           ,@GhiChu)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[HQ_ThucDonChiTiet]
   SET [ThucDonId] =@ThucDonId
           ,[MonAnId] = @MonAnId
           ,[GhiChu] = @GhiChu
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from HQ_ThucDonChiTiet";

        public HQ_ThucDonChiTiet()
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

        //public List<T> DeleteByThucDonId<T>(int thuDonId)
        //{
            
        //    using var connection = new SqlConnection(connectionString);
        //    connection.Open();
        //    var items = connection.Query<T>(query, new { thuDonId });
        //    return items;
        //}
        public int DeleteByThucDonId<T>(List<T> items)
        {
            try
            {
                var query = @"DELETE FROM [dbo].[HQ_ThucDonChiTiet] WHERE [ThucDonId] = @ThucDonId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(query, items);
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int DeleteByThucDonId<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrDeleteByThucDonId, item);
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
