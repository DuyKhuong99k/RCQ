using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class Thit_PhieuCanPhaLoc
    {
        private readonly string connectionString;
        private string tableName = @"Thit_PhieuCanPhaLoc";
        private readonly string qrDelete = @"DELETE FROM [dbo].[Thit_PhieuCanPhaLoc]
      WHERE [STT] = @STT 
      And [MaMayCan] = @MaMayCan 
      And [MaXuong] = @MaXuong 
      And [Ngay] = @Ngay";

        private readonly string qrInsert = @"INSERT INTO [dbo].[Thit_PhieuCanPhaLoc]
           ([STT]
           ,[MaMayCan]
           ,[Ngay]
           ,[MaXuong]
           ,[Gio]
           ,[MaLoaiNguyenLieu]
           ,[MaLoaiThanhPham]
           ,[MaSize]
           ,[MaNhanVien]
           ,[MaThe]
           ,[TrongLuong]
           ,[SuDung]
           ,[MaNhanVienPhucVu]
           ,[InOut]
           ,[ItemCode]
           ,[MaLoi]
           ,[IsLoi]
           ,[MaLo],[GhiChu],[IsNL],[IsSX])
     VALUES
           (@STT
           ,@MaMayCan
           ,@Ngay
           ,@MaXuong
           ,@Gio
           ,@MaLoaiNguyenLieu
           ,@MaLoaiThanhPham
           ,@MaSize
           ,@MaNhanVien
           ,@MaThe
           ,@TrongLuong
           ,@SuDung
           ,@MaNhanVienPhucVu
           ,@InOut
           ,@ItemCode
           ,@MaLoi
           ,@IsLoi
           ,@MaLo,@GhiChu,@IsNL,@IsSX)";

        private readonly string qrUpdate = @"UPDATE [dbo].[Thit_PhieuCanPhaLoc]
   SET [Gio] = @Gio
      ,[MaLoaiNguyenLieu] = @MaLoaiNguyenLieu
      ,[MaLoaiThanhPham] = @MaLoaiThanhPham
      ,[MaSize] = @MaSize
      ,[MaNhanVien] = @MaNhanVien
      ,[MaThe] = @MaThe
      ,[TrongLuong] = @TrongLuong
      ,[SuDung] = @SuDung
      ,[MaNhanVienPhucVu] = @MaNhanVienPhucVu
      ,[InOut] = @InOut
      ,[ItemCode] = @ItemCode
      ,[MaLoi] = @MaLoi
      ,[IsLoi] = @IsLoi
      ,[MaLo] = @MaLo,GhiChu=@GhiChu,[IsNL] = @IsNL,[IsSX] = @IsSX
 WHERE [STT] = @STT
      and [MaMayCan] = @MaMayCan
      and [Ngay] = @Ngay
      and [MaXuong] = @MaXuong";

        private readonly string qrGetAll = "Select * from Thit_PhieuCanPhaLoc";

        public Thit_PhieuCanPhaLoc()
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
