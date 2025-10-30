using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamDinhHinh_Color
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamDinhHinh_Color";
        private readonly string qrDelete = @"DELETE [dbo].[MaThanhPhamDinhHinh_Color] where [ColorCode] = @ColorCode
      and [Ngay] = @Ngay
      and [MaLo] = @MaLo
      and [MaXuong] = @MaXuong";
        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPhamDinhHinh_Color]
           ([MaThanhPham]
           ,[ColorCode]
           ,[Ngay]
           ,[MaLo]
           ,[MaXuong])
     VALUES
           (@MaThanhPham 
           ,@ColorCode 
           ,@Ngay 
           ,@MaLo 
           ,@MaXuong)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPhamDinhHinh_Color]
   SET [MaThanhPham] = @MaThanhPham
      
 WHERE [ColorCode] = @ColorCode
      and [Ngay] = @Ngay
      and [MaLo] = @MaLo
      and [MaXuong] = @MaXuong";
        private readonly string qrGetAll = "Select * from MaThanhPhamDinhHinh_Color";

        public MaThanhPhamDinhHinh_Color()
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
        public List<T> GetsLastDay<T>()
        {
            try
            {
                var query = @"Select
    *
from
    (
        Select
            d.*,
            ROW_NUMBER() OVER (
                PARTITION BY ColorCode
                ORDER BY
                    Ngay DESC
            ) AS [ROW NUMBER]
        from
            MaThanhPhamDinhHinh_Color d
    ) as c
where
   c.[ROW NUMBER] = 1";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query).ToList();
                return items;
            }
            catch (Exception)
            {
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
