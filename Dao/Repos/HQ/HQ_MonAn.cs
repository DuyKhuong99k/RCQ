using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class HQ_MonAn
    {
        private readonly string connectionString;
        private string tableName = @"HQ_MonAn";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_MonAn]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[HQ_MonAn]
           (
           [Ten]
           ,[GhiChu],[LoaiMonAnId])
     VALUES
           (
           @Ten
           ,@GhiChu,@LoaiMonAnId)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[HQ_MonAn]
   SET [Ten] = @Ten
      ,[GhiChu] = @GhiChu, [LoaiMonAnId] = @LoaiMonAnId
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from HQ_MonAn";

        public HQ_MonAn()
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
        public List<T> GetsFullField<T>()
        {
            try
            {
                var query = @"select 
p.Id,
p.Ten,
p.GhiChu,
p.LoaiMonAnId,
l.Ten as LoaiMonAnName
from HQ_MonAn p
left join HQ_LoaiMonAn l on p.LoaiMonAnId = l.Id 
order by
p.Id desc

";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
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
