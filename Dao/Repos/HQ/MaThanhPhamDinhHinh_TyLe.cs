using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamDinhHinh_TyLe
    {
        private readonly string connectionString;
        private string tableName = @"BoPhan";
        private readonly string qrDelete = @" DELETE FROM [dbo].[MaThanhPhamDinhHinh_TyLe]
      WHERE [Id] = @Id";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPhamDinhHinh_TyLe]
           ([Id]
           ,[NgayGio]
           ,[MaThanhPham]
           ,[TyLeDau]
           ,[TyLeRot],[MaXuong])
     VALUES
           (@Id
           ,@NgayGio
           ,@MaThanhPham
           ,@TyLeDau
           ,@TyLeRot,@MaXuong)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPhamDinhHinh_TyLe]
   SET [NgayGio] = @NgayGio
      ,[MaThanhPham] = @MaThanhPham
      ,[TyLeDau] = @TyLeDau
      ,[TyLeRot] = @TyLeRot, [MaXuong] = @MaXuong";

        private readonly string qrGetAll = "Select * from MaThanhPhamDinhHinh_TyLe";

        public MaThanhPhamDinhHinh_TyLe(string? _connectionString = null)
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
tl.Id,
tl.NgayGio,
tl.MaThanhPham,
tp.Ten as ThanhPhamName,
tl.TyLeDau,
tl.TyLeRot,
tl.MaXuong,
x.Ten as XuongName
from MaThanhPhamDinhHinh_TyLe tl
left join MaThanhPhamDinhHinh tp on tp.Ma = tl.MaThanhPham
left join XiNghiep x on x.Ma = tl.MaXuong
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
        public List<T> GetsLast<T>(DateTime dateTime,string xuongId)
        {
            try
            {
                var query = @"Select
    *
from
    (
        Select
            dm.*,
            ROW_NUMBER() OVER (
                PARTITION BY MaThanhPham
                ORDER BY
                    NgayGio DESC
            ) AS [ROW NUMBER]
        from
            MaThanhPhamDinhHinh_TyLe dm
        where
           cast(NgayGio as Date) <= @ngay
            and MaXuong = @xuongId
    ) dm where dm.[ROW NUMBER] =1
order by
  MaThanhPham";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(
                        query,
                        new
                        {
                            ngay = dateTime.Date, xuongId
                        })
                    .ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
