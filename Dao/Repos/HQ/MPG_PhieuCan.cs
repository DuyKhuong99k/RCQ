using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class MPG_PhieuCan
    {
        private readonly string connectionString;
        private string tableName = @"MPG_PhieuCan";
        private readonly string qrDelete = @"DELETE FROM [dbo].[MPG_PhieuCan]
      WHERE [STT] = @STT
      and [Ngay] = @Ngay
      and [MayCan] = @MayCan and [MaXuong] = @MaXuong";

        private readonly string qrInsert = @"INSERT INTO [dbo].[MPG_PhieuCan]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MayCan]
           ,[MaCongThucChiTiet]
           ,[PhuTroi]
           ,[TrongLuong]
           ,[Block]
           ,[TrongLuongCongThuc]
           ,[MaThe]
           ,[MaNhanVien]
           ,[SuDung]
           ,[GhiChu]
           ,[MaXuong])
     VALUES
           (@STT
           ,@Ngay
           ,@Gio
           ,@MayCan
           ,@MaCongThucChiTiet
           ,@PhuTroi
           ,@TrongLuong
           ,@Block
           ,@TrongLuongCongThuc
           ,@MaThe
           ,@MaNhanVien
           ,@SuDung
           ,@GhiChu
           ,@MaXuong)";

        private readonly string qrUpdate = @"UPDATE [dbo].[MPG_PhieuCan]
   SET [Gio] = @Gio
      ,[MaCongThucChiTiet] = @MaCongThucChiTiet
      ,[PhuTroi] = @PhuTroi
      ,[TrongLuong] = @TrongLuong
      ,[Block] = @Block
      ,[TrongLuongCongThuc] = @TrongLuongCongThuc
      ,[MaThe] = @MaThe
      ,[MaNhanVien] = @MaNhanVien
      ,[SuDung] = @SuDung
      ,[GhiChu] = @GhiChu
      
 WHERE [STT] = @STT
      and [Ngay] = @Ngay
      and [MayCan] = @MayCan and [MaXuong] = @MaXuong";

        private readonly string qrGetAll = "Select * from MPG_PhieuCan";

        public MPG_PhieuCan()
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
