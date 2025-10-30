using Dapper;
using Microsoft.Data.SqlClient;


namespace Dao.Repos.HQ
{
    public partial class TrongLuongCoiTheoSanPham
    {
        private readonly string connectionString;
        private string tableName = @"TrongLuongCoiTheoSanPham";
        private readonly string qrDelete = @"DELETE FROM [dbo].[TrongLuongCoiTheoSanPham]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[TrongLuongCoiTheoSanPham]
           ([MaCoi]
           ,[MaSanPham]
           ,[TrongLuong])
     VALUES
           (@MaCoi
           ,@MaSanPham
           ,@TrongLuong)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[TrongLuongCoiTheoSanPham]
   SET [MaCoi] = @MaCoi
      ,[MaSanPham] = @MaSanPham
      ,[TrongLuong] = @TrongLuong
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from TrongLuongCoiTheoSanPham";

        public TrongLuongCoiTheoSanPham()
        {
            connectionString = AppViewModels.Base.Ins.ConnectionString;

        }
        public List<T> GetAllsFullField<T>()
        {
            var query = @"select 
p.Id,
p.MaCoi,
c.Ten as CoiName,
p.MaSanPham,
tp.Ten as ThanhPhamName,
p.TrongLuong
from TrongLuongCoiTheoSanPham p
left join MaCoiXepKhuon c on c.Ma = p.MaCoi
left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaSanPham";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query)
                .ToList();
            return items;
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
