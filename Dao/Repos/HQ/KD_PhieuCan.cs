using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class KD_PhieuCan
    {
        private readonly string connectionString;
        private string tableName = @"KD_PhieuCan";
        private readonly string qrDelete = @"DELETE FROM [dbo].[KD_PhieuCan]
      WHERE [STT] = @STT
      and  [Ngay] = @Ngay
      and [MayCan] = @MayCan
and [MaXuong] =@MaXuong";

        private readonly string qrInsert = @"INSERT INTO [dbo].[KD_PhieuCan]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MayCan]
           ,[MaTui]
           ,[SoLuongPhanTu]
           ,[TrongLuong]
           ,[MaNhanVien]
           ,[MaThe]
           ,[SuDung]
           ,[GhiChu],[MaXuong])
     VALUES
           (@STT
           ,@Ngay
           ,@Gio
           ,@MayCan
           ,@MaTui
           ,@SoLuongPhanTu
           ,@TrongLuong
           ,@MaNhanVien
           ,@MaThe
           ,@SuDung
           ,@GhiChu,@MaXuong)";

        private readonly string qrUpdate = @"UPDATE [dbo].[KD_PhieuCan]
   SET [Gio] = @Gio
      ,[MaTui] = @MaTui
      ,[SoLuongPhanTu] = @SoLuongPhanTu
      ,[TrongLuong] = @TrongLuong
      ,[MaNhanVien] = @MaNhanVien
      ,[MaThe] = @MaThe
      ,[SuDung] = @SuDung
      ,[GhiChu] = @GhiChu
, [MaXuong]
 WHERE [STT] = @STT
      and  [Ngay] = @Ngay
      and [MayCan] = @MayCan
and [MaXuong] =@MaXuong";

        private readonly string qrGetAll = "Select * from KD_PhieuCan";

        public KD_PhieuCan()
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
