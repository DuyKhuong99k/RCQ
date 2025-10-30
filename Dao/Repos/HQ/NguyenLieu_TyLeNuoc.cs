using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class NguyenLieu_TyLeNuoc
    {
        private readonly string connectionString;
        private string tableName = @"NguyenLieu_TyLeNuoc";
        private readonly string qrDelete = @"DELETE FROM [dbo].[NguyenLieu_TyLeNuoc]
      WHERE STT] = @STT 
      and [Ngay] = @Ngay 
      and [MaXuong] = @MaXuong";

        private readonly string qrInsert = @"INSERT INTO [dbo].[NguyenLieu_TyLeNuoc]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MaLo]
           ,[MaPhuongTien]
           ,[MaThanhPham]
           ,[TyLeNuoc]
           ,[GhiChu],[MaXuong])
     VALUES
           (@STT 
           ,@Ngay 
           ,@Gio 
           ,@MaLo 
           ,@MaPhuongTien 
           ,@MaThanhPham 
           ,@TyLeNuoc 
           ,@GhiChu,@MaXuong)";

        private readonly string qrUpdate = @"UPDATE [dbo].[NguyenLieu_TyLeNuoc]
   SET [Gio] = @Gio  
      ,[TyLeNuoc] = @TyLeNuoc 
      ,[GhiChu] = @GhiChu
      ,[MaLo] = @MaLo 
      ,[MaPhuongTien] = @MaPhuongTien 
      ,[MaThanhPham] = @MaThanhPham
 WHERE [STT] = @STT 
      and [Ngay] = @Ngay 
       and [MaXuong] = @MaXuong";

        private readonly string qrGetAll = "Select * from NguyenLieu_TyLeNuoc";

        public NguyenLieu_TyLeNuoc()
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
