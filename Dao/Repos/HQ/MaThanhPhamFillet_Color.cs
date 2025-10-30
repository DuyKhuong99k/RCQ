using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamFillet_Color
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamFillet_Color";
        private readonly string qrDelete = @"DELETE [dbo].[MaThanhPhamFillet_Color] where [Ngay] =@Ngay and MaXuong = @MaXuong";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPhamFillet_Color]
           ([MaThanhPham]
           ,[ColorCode]
           ,[Ngay]
           ,[MaLo]
           ,[MaXuong]
,[MaSize])
     VALUES
           (@MaThanhPham 
           ,@ColorCode 
           ,@Ngay 
           ,@MaLo 
           ,@MaXuong
,@MaSize)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPhamFillet_Color]
   SET [MaThanhPham] = @MaThanhPham 
,[MaSize] = @MaSize
 WHERE [ColorCode] = @ColorCode 
      and [Ngay] = @Ngay 
      and [MaLo] = @MaLo 
      and [MaXuong] = @MaXuong ";

        private readonly string qrGetAll = "Select * from MaThanhPhamFillet_Color";

        public MaThanhPhamFillet_Color()
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

        public int Insert<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrInsert, item);
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
            MaThanhPhamFillet_Color d
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
        public int Update<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdate, item);
            return rows;
        }
    }
}
