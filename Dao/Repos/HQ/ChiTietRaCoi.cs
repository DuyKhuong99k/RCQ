using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class ChiTietRaCoi
    {
        private readonly string connectionString;
        private string tableName = @"ChiTietRaCoi";
        private readonly string qrDelete = @"DELETE FROM [dbo].[ChiTietRaCoi]
      WHERE  [Id] = @Id";

        private readonly string qrInsert = @"INSERT INTO [dbo].[ChiTietRaCoi]
           ([Ngay]
           ,[Gio]
           ,[MaCoi]
           ,[Luot]
           ,[IsDone])
     VALUES 
           (@Ngay
           ,@Gio
           ,@MaCoi
           ,@Luot
           ,@IsDone)";

        private readonly string qrUpdate = @"UPDATE [dbo].[ChiTietRaCoi]
     SET    [Ngay] = @Ngay
           ,[Gio] = @Gio
           ,[MaCoi] = @MaCoi
           ,[Luot] = @Luot
           ,[IsDone] = @IsDone
     WHERE [Id] = @Id";

        private readonly string qrGetAll = "Select * from ChiTietRaCoi";

        public ChiTietRaCoi(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

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
        public List<T> GetsFullField<T>()
        {
            try
            {
                var query = @"select
ct.Id,
ct.Ngay,
ct.Gio,
ct.MaCoi,
c.Ten as CoiName,
ct.Luot,
ct.IsDone,
ct.NgayNguyenLieu
from ChiTietRaCoi ct
left join MaCoiXepKhuon c on ct.MaCoi = c.Ma

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
    }
}
