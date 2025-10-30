using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TDinhMuc
    {
        private readonly string connectionString;
        private string tableName = @"T_DinhMuc";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_DinhMuc]
      WHERE [STT] = @STT
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[T_DinhMuc]
            ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MaLo]
           ,[MaLoaiNguyenLieu]
           ,[MaSize]
           ,[MaThanhPham]
           ,[MaXuong]
           ,[CaTra]
           ,[DinhMuc]
           ,[SuDung]
           ,[MaKhuVuc])
     VALUES
            (@STT
           ,@Ngay
           ,@Gio
           ,@MaLo
           ,@MaLoaiNguyenLieu
           ,@MaSize
           ,@MaThanhPham
           ,@MaXuong
           ,@CaTra
           ,@DinhMuc
           ,@SuDung
           ,@MaKhuVuc)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[T_DinhMuc]
   SET [Ngay] =@Ngay
           ,[Gio] = @Gio
           ,[MaLo] = @MaLo
           ,[MaLoaiNguyenLieu] = @MaLoaiNguyen
           ,[MaSize] = @MaSize
           ,[MaThanhPham] = @MaThanhPham
           ,[MaXuong] = @MaXuong
           ,[CaTra] = @CaTra
           ,[DinhMuc] = @DinhMuc
           ,[SuDung] = @SuDung
           ,[MaKhuVuc] = @MaKhuVuc
 WHERE [STT] = @STT
";

        private readonly string qrGetAll = "Select * from T_DinhMuc";

        public TDinhMuc()
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
