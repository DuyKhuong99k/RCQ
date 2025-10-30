using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class KNH_PhieuCan
    {
        private readonly string connectionString;
        private string tableName = @"KNH_PhieuCan";
        private readonly string qrDelete = @"DELETE FROM [dbo].[KNH_PhieuCan]
      WHERE [STT] = @STT
      and  [Ngay] = @Ngay
      and [MayCan] = @MayCan
and [MaXuong] =@MaXuong";
        private readonly string qrInsert = @"INSERT INTO [dbo].[KNH_PhieuCan]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MayCan]
           ,[MaThongTinSanPham]
           ,[TrongLuong]
           ,[TrongLuongKiemLai]
           ,[SoLuongPhanTu]
           ,[MaThe]
           ,[MaNhanVien]
           ,[PhuTroi]
           ,[SuDung]
           ,[GhiChu]
           ,[MaXuong])
     VALUES
           (@STT
           ,@Ngay
           ,@Gio
           ,@MayCan
           ,@MaThongTinSanPham
           ,@TrongLuong
           ,@TrongLuongKiemLai
           ,@SoLuongPhanTu
           ,@MaThe
           ,@MaNhanVien
           ,@PhuTroi
           ,@SuDung
           ,@GhiChu
           ,@MaXuong)";
        private readonly string qrUpdate = @"UPDATE [dbo].[KNH_PhieuCan]
   SET [Gio] = @Gio
      ,[MaThongTinSanPham] = @MaThongTinSanPham
      ,[TrongLuong] = @TrongLuong
      ,[TrongLuongKiemLai] = @TrongLuongKiemLai
      ,[SoLuongPhanTu] = @SoLuongPhanTu
      ,[MaThe] = @MaThe
      ,[MaNhanVien] = @MaNhanVien
      ,[PhuTroi] = @PhuTroi
      ,[SuDung] = @SuDung
      ,[GhiChu] = @GhiChu
WHERE [STT] = @STT
      and  [Ngay] = @Ngay
      and [MayCan] = @MayCan
and [MaXuong] =@MaXuong";
        private readonly string qrGetAll = "Select * from KNH_PhieuCan";

        public KNH_PhieuCan()
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
