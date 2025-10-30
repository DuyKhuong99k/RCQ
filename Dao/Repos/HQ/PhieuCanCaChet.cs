using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanCaChet
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanCaChet";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhieuCanCaChet]
      WHERE [STT] = @STT
      and [Ngay] = @Ngay
      and [MaMayCan] = @MaMayCan";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanCaChet]
           ([STT]
           ,[Ngay]
           ,[MaMayCan]
           ,[Gio]
           ,[MaLoaiCa]
           ,[MaKhachHang]
           ,[MaThongKe]
           ,[MaAo]
           ,[TrongLuong]
           ,[TrongLuongBinhQuan]
           ,[TrongLuongTare]
           ,[GhiChu])
     VALUES
           (@STT
           ,@Ngay
           ,@MaMayCan
           ,@Gio
           ,@MaLoaiCa
           ,@MaKhachHang
           ,@MaThongKe
           ,@MaAo
           ,@TrongLuong
           ,@TrongLuongBinhQuan
           ,@TrongLuongTare
           ,@GhiChu)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanCaChet]
   SET 
      [Gio] = @Gio
      ,[MaLoaiCa] = @MaLoaiCa
      ,[MaKhachHang] = @MaKhachHang
      ,[MaThongKe] = @MaThongKe
      ,[MaAo] = @MaAo
      ,[TrongLuong] = @TrongLuong
      ,[TrongLuongBinhQuan] = @TrongLuongBinhQuan
      ,[TrongLuongTare] = @TrongLuongTare
      ,[GhiChu] = @GhiChu
 WHERE [STT] = @STT
      and [Ngay] = @Ngay
      and [MaMayCan] = @MaMayCan";

        private readonly string qrGetAll = "Select * from PhieuCanCaChet";

        public PhieuCanCaChet()
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
