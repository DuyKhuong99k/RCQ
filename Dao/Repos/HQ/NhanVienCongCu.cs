using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class NhanVienCongCu
    {
        private readonly string connectionString;
        private string tableName = @"NhanVienCongCu";
        private readonly string qrDelete = @"DELETE FROM [dbo].[NhanVienCongCu]
      WHERE [Ngay] = @Ngay 
      and [MaLo] = @MaLo 
      and [TabName] = @TabName 
      and [MaNhanVien] = @MaNhanVien and [MaXuong] = @MaXuong";

        private readonly string qrInsert = @"INSERT INTO [dbo].[NhanVienCongCu]
           ([Ngay]
           ,[Gio]
           ,[MaLo]
           ,[TabName]
           ,[MaNhanVien]
           ,[MaSizeXepKhuonChinh]
           ,[MaSizeXepKhuonKXL]
           ,[MaThanhPhamXepKhuonChinh]
           ,[MaThanhPhamXepKhuonKXL]
           ,[MaChieuXaXepKhuonChinh]
           ,[MaKhachHangXepKhuonKXL],[MaXuong])
     VALUES
           (@Ngay
           ,@Gio
           ,@MaLo
           ,@TabName
           ,@MaNhanVien
           ,@MaSizeXepKhuonChinh
           ,@MaSizeXepKhuonKXL
           ,@MaThanhPhamXepKhuonChinh
           ,@MaThanhPhamXepKhuonKXL
           ,@MaChieuXaXepKhuonChinh
           ,@MaKhachHangXepKhuonKXL,@MaXuong)";
        private readonly string qrUpdate = @"UPDATE [dbo].[NhanVienCongCu]
   SET [Gio] = @Gio 
      ,[MaSizeXepKhuonChinh] = @MaSizeXepKhuonChinh 
      ,[MaSizeXepKhuonKXL] = @MaSizeXepKhuonKXL 
      ,[MaThanhPhamXepKhuonChinh] = @MaThanhPhamXepKhuonChinh 
      ,[MaThanhPhamXepKhuonKXL] = @MaThanhPhamXepKhuonKXL 
      ,[MaChieuXaXepKhuonChinh] = @MaChieuXaXepKhuonChinh 
      ,[MaKhachHangXepKhuonKXL] = @MaKhachHangXepKhuonKXL 
 WHERE [Ngay] = @Ngay 
    and [MaXuong] = @MaXuong
      and [MaLo] = @MaLo 
      and [TabName] = @TabName 
      and [MaNhanVien] = @MaNhanVien ";

        private readonly string qrGetAll = "Select * from NhanVienCongCu";
        private readonly string qrGetByDate = "Select * from NhanVienCongCu where Ngay =@ngay";

        public NhanVienCongCu()
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
        public List<T> Gets<T>(DateTime date)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetByDate,new {ngay = date.Date}).ToList();
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
