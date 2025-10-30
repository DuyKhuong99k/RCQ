using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class DinhMucXepHang
    {
        private readonly string connectionString;
        private string tableName = @"DinhMucXepHang";
        private readonly string qrDelete = @"DELETE FROM [dbo].[DinhMucXepHang]
      WHERE [MaLo] = @MaLo and [MaSanPham] = @MaSanPham and [MaXepHang] = @MaXepHang and [Year] = @Year";

        private readonly string qrInsert = @"INSERT INTO [dbo].[DinhMucXepHang]
           ([MaLo]
           ,[MaSanPham]
           ,[MaXepHang]
            ,[DinhMucUp],[DinhMucDown],[Year],[DinhMuc])
     VALUES
           (@MaLo
           ,@MaSanPham
           ,@MaXepHang
           ,@DinhMucUp,@DinhMucDown,@Year,@DinhMuc)";

        private readonly string qrUpdate = @"UPDATE [dbo].[DinhMucXepHang]
   SET [DinhMucUp] = @DinhMucUp,[DinhMucDown] = @DinhMucDown,[DinhMuc] = @DinhMuc
 WHERE [MaLo] = @MaLo
      and [MaSanPham] = @MaSanPham
      and [MaXepHang] = @MaXepHang and [Year] =@Year
";

        private readonly string qrGetAll = "Select * from DinhMucXepHang";
        public DinhMucXepHang(string? _connectionString = null)
        {
            connectionString =_connectionString??AppViewModels.Base.Ins.ConnectionString;

        }

        //public DinhMucXepHang()
        //{
        //    connectionString = AppViewModels.Base.Ins.ConnectionString;

        //}

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

        public List<T> GetsFullFieldByYear<T>(int year)
        {
            try
            {
                var query = @"select 
dm.MaLo,
dm.MaSanPham,
sp.Ten as SanPhamName,
dm.MaXepHang,
xh.Ten as XepHangName,
dm.DinhMucDown,
dm.DinhMucUp,
dm.DinhMuc
from DinhMucXepHang dm
left join DG_SanPhamTinhLuong sp on sp.Ma = dm.MaSanPham 
left join MaXepHang xh on xh.Ma = dm.MaXepHang
where dm.Year = @year
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new{year}).ToList();
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
