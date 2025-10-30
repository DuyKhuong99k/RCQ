using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MaThanhPhamLangDa_Color
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamLangDa_Color";
        private readonly string qrDelete = @"DELETE [dbo].[MaThanhPhamLangDa_Color] where [ColorCode] = @ColorCode
      and [Ngay] = @Ngay
      and [MaLo] = @MaLo
      and [MaXuong] = @MaXuong";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MaThanhPhamLangDa_Color]
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

        private readonly string qrUpdate = @"UPDATE [dbo].[MaThanhPhamLangDa_Color]
   SET [MaThanhPham] = @MaThanhPham
      
 WHERE [ColorCode] = @ColorCode
      and [Ngay] = @Ngay
      and [MaLo] = @MaLo
      and [MaXuong] = @MaXuong";

        private readonly string qrGetAll = "Select * from MaThanhPhamLangDa_Color";

        public MaThanhPhamLangDa_Color()
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

        public int Update<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdate, item);
            return rows;
        }
    }
}
