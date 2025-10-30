using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanPhaLoc
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanNguyenLieu";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhieuCanPhaLoc]
      WHERE [Ngay]= @Ngay";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanPhaLoc]
           ([MaMayTinhCan]
           ,[MaUserCan]
           ,[ThoiGianCan]
           ,[Ngay]
           ,[MaXuongSanXuat]
           ,[MSL]
           ,[MaLoaiCa]
           ,[MaLoaiThanhPham]
           ,[MaSize]
           ,[MaMau]
           ,[MaNhanVien]
           ,[HoVaTen]
           ,[MaTheTu]
           ,[TrongLuong]
           ,[SuDung]
           ,[GhiChu],[MaNhanVienPhucVu],[LoaiCan],[InOut],[ItemCode] )
     VALUES
           (@MaMayTinhCan 
           ,@MaUserCan 
           ,@ThoiGianCan 
           ,@Ngay 
           ,@MaXuongSanXuat 
           ,@MSL 
           ,@MaLoaiCa 
           ,@MaLoaiThanhPham 
           ,@MaSize 
           ,@MaMau 
           ,@MaNhanVien 
           ,@HoVaTen 
           ,@MaTheTu 
           ,@TrongLuong 
           ,@SuDung 
           ,@GhiChu,@MaNhanVienPhucVu,@LoaiCan,@InOut, @ItemCode)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanPhaLoc]
   SET [MaXuongSanXuat] = @MaXuongSanXuat 
      ,[MSL] = @MSL 
      ,[MaLoaiCa] = @MaLoaiCa 
      ,[MaLoaiThanhPham] = @MaLoaiThanhPham 
      ,[MaSize] = @MaSize 
      ,[MaMau] = @MaMau 
      ,[MaNhanVien] = @MaNhanVien 
      ,[HoVaTen] = @HoVaTen 
      ,[MaTheTu] = @MaTheTu 
      ,[TrongLuong] = @TrongLuong 
      ,[SuDung] = @SuDung 
      ,[GhiChu] = @GhiChu ,[MaNhanVienPhucVu] = @MaNhanVienPhucVu,[LoaiCan] =@LoaiCan,[InOut]=@InOut,[ItemCode] =@ItemCode
 WHERE [MaMayTinhCan] = @MaMayTinhCan 
      And [MaUserCan] = @MaUserCan 
      And [ThoiGianCan] = @ThoiGianCan 
      And [Ngay] = @Ngay ";

        private readonly string qrGetAll = "Select * from PhieuCanPhaLoc";

        public PhieuCanPhaLoc()
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
