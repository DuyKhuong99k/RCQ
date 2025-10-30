using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class CoiLeXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"CoiLeXepKhuon";
        private readonly string qrDelete = @"DELETE FROM [dbo].[CoiLeXepKhuon]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[CoiLeXepKhuon]
           ([MaCoi]
           ,[MaSanPham]
           ,[TrongLuong]
           ,[NgayGio],[NgayNguyenLieu])
     VALUES
           (@MaCoi
           ,@MaSanPham
           ,@TrongLuong
           ,@NgayGio,@NgayNguyenLieu)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[CoiLeXepKhuon]
   SET [MaCoi] = @MaCoi
      ,[MaSanPham] = @MaSanPham
      ,[TrongLuong] = @TrongLuong
      ,[NgayGio] = @NgayGio, [NgayNguyenLieu] = @NgayNguyenLieu
 WHERE [Id] = @Id
";

        private readonly string qrGetAll = "Select * from CoiLeXepKhuon";

        public CoiLeXepKhuon()
        {
            connectionString = AppViewModels.Base.Ins.ConnectionString;

        }
        public List<T> GetAllsFullField<T>(DateTime dateTime)
        {
            var query = @"select 
p.Id,
p.MaCoi,
c.Ten as CoiName,
p.MaSanPham,
tp.Ten as ThanhPhamName,
p.TrongLuong,
p.NgayNguyenLieu,
p.NgayGio
from CoiLeXepKhuon p
left join MaCoiXepKhuon c on c.Ma = p.MaCoi
left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaSanPham
where CAST(p.NgayGio AS DATE) = @dateTime";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { dateTime })
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
