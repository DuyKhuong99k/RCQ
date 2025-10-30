using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamFillet_ThanhPhamMacDinh
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamFillet_ThanhPhamMacDinh";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MaThanhPhamFillet_ThanhPhamMacDinh]
      WHERE [STT] = @STT 
      and [Ngay] = @Ngay and MaXuong = @MaXuong";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPhamFillet_ThanhPhamMacDinh]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MaLo]
           ,[MaThanhPham]
           ,[MaXuong])
     VALUES
           (@STT
           ,@Ngay
           ,@Gio
           ,@MaLo
           ,@MaThanhPham
           ,@MaXuong)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPhamFillet_ThanhPhamMacDinh]
   SET [Gio] = @Gio
      ,[MaLo] = @MaLo
      ,[MaThanhPham] = @MaThanhPham
      
 WHERE [STT] = @STT
      and [Ngay] = @Ngay
      and [MaXuong] = @MaXuong
";

        private readonly string qrGetAll = "Select * from MaThanhPhamFillet_ThanhPhamMacDinh";

        public MaThanhPhamFillet_ThanhPhamMacDinh(string? _connectionString = null)
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
        public List<T> GetsFullField<T>(DateTime ngay, string xuong)
        {
            try
            {
                var query = @"select
md.STT,
md.Ngay,
md.Gio,
md.MaLo,
md.MaThanhPham,
tp.Ten as ThanhPhamName,
md.MaXuong,
x.Ten as XuongName
from MaThanhPhamFillet_ThanhPhamMacDinh md
left join MaThanhPhamFillet tp on tp.Ma = md.MaThanhPham
left join XiNghiep x on x.Ma = md.MaXuong
where md.Ngay = @ngay and md.MaXuong = @xuong
order by STT DESC
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new {ngay, xuong }).ToList();
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
