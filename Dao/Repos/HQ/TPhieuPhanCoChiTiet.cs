using Dapper;
using Microsoft.Data.SqlClient;
namespace Dao.Repos.HQ
{
    public partial class TPhieuPhanCoChiTiet
    {
        private readonly string connectionString;
        private string tableName = @"T_PhieuPhanCoChiTiet";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_PhieuPhanCoChiTiet]
      WHERE  [STT] = @STT and [MaPhieuPhanCo] = @MaPhieuPhanCo";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_PhieuPhanCoChiTiet]
           ([STT]
           ,[MaPhieuPhanCo]
           ,[MaSize]
           ,[MaQuyTrinh]
           ,[MaPhuGia]
           ,[MaKhachHang]
           ,[MaKhangSinh]
           ,[VoXo]
           ,[MaThongTinPhu]
           ,[MaTrangThaiNguyenLieu]
           ,[TyLe]
           ,[GramCuoi]
           ,[GramDau]
           ,[GhiChu],[MaCongDoan])
     VALUES
           (@STT
           ,@MaPhieuPhanCo
           ,@MaSize
           ,@MaQuyTrinh
           ,@MaPhuGia
           ,@MaKhachHang
           ,@MaKhangSinh
           ,@VoXo
           ,@MaThongTinPhu
           ,@MaTrangThaiNguyenLieu
           ,@TyLe
           ,@GramCuoi
           ,@GramDau
           ,@GhiChu,@MaCongDoan)";

        private readonly string qrUpdate = @"UPDATE [dbo].[T_PhieuPhanCoChiTiet]
   SET 
      [MaSize] = @MaSize
      ,[MaQuyTrinh] = @MaQuyTrinh
      ,[MaPhuGia] = @MaPhuGia
      ,[MaKhachHang] = @MaKhachHang
      ,[MaKhangSinh] = @MaKhangSinh
      ,[VoXo] = @VoXo
      ,[MaThongTinPhu] = @MaThongTinPhu
      ,[MaTrangThaiNguyenLieu] = @MaTrangThaiNguyenLieu
      ,[TyLe] = @TyLe
      ,[GramCuoi] = @GramCuoi
      ,[GramDau] = @GramDau
      ,[GhiChu] = @GhiChu , [MaCongDoan] = @MaCongDoan
 WHERE [STT] = @STT
      and [MaPhieuPhanCo] = @MaPhieuPhanCo";

        private readonly string qrGetAll = "Select * from T_PhieuPhanCoChiTiet";

        public TPhieuPhanCoChiTiet()
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
