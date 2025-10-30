using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
namespace Dao.Repos.HQ
{
    public partial class TrongLuongCoiTheoThanhPham
    {
        private readonly string connectionString;
        private string tableName = @"TrongLuongCoiTheoThanhPham";
        private readonly string qrDelete = @"DELETE FROM [dbo].[TrongLuongCoiTheoThanhPham]
      WHERE [MaCoi] = @MaCoi and [MaThanhPham] = @MaThanhPham and [MaXuong] = @MaXuong
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[TrongLuongCoiTheoThanhPham]
           ([MaCoi]
           ,[MaThanhPham]
           ,[MaXuong],[TrongLuongMax])
     VALUES
           (@MaCoi 
           ,@MaThanhPham 
           ,@MaXuong,@TrongLuongMax)";

        private readonly string qrUpdate = @"UPDATE [dbo].[TrongLuongCoiTheoThanhPham]
   SET [TrongLuongMax] = @TrongLuongMax 
 WHERE [MaCoi] = @MaCoi and [MaThanhPham] = @MaThanhPham and [MaXuong] = @MaXuong";

        private readonly string qrGetAll = "Select * from TrongLuongCoiTheoThanhPham";

        public TrongLuongCoiTheoThanhPham()
        {
            connectionString = AppViewModels.Base.Ins.ConnectionString;

        }
        public List<T> GetAllsFullField<T>()
        {
            var query = @"select
p.MaCoi,
c.Ten as CoiName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaXuong,
x.Ten as XuongName,
p.TrongLuongMax
from 
TrongLuongCoiTheoThanhPham p
left join MaCoiXepKhuon c on c.Ma = p.MaCoi
left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPham
left join XiNghiep x on x.Ma = p.MaXuong";
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
