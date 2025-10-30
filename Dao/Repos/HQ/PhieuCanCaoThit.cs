using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanCaoThit
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanCaoThit";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhieuCanCaoThit]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[PhieuCanCaoThit]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MaUserCan]
           ,[MaMayCan]
           ,[MaLoaiCa]
           ,[MaQuyCach]
           ,[MaSize]
           ,[MaThanhPham]
           ,[MaLo]
           ,[MaThe]
           ,[TrongLuong]
           ,[TrongLuongSec]
           ,[DinhMucThucTe]
           ,[DinhMucYeuCau]
           ,[MaXuong]
           ,[MaNhanVien]
           ,[GhiChu]
           ,[TrongLuongTare])
     VALUES
           (@STT
           ,@Ngay
           ,@Gio
           ,@MaUserCan
           ,@MaMayCan
           ,@MaLoaiCa
           ,@MaQuyCach
           ,@MaSize
           ,@MaThanhPham
           ,@MaLo
           ,@MaThe
           ,@TrongLuong
           ,@TrongLuongSec
           ,@DinhMucThucTe
           ,@DinhMucYeuCau
           ,@MaXuong
           ,@MaNhanVien
           ,@GhiChu
           ,@TrongLuongTare)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanCaoThit]
   SET [Gio] = @Gio
      ,[MaUserCan] = @MaUserCan
      ,[MaLoaiCa] = @MaLoaiCa
      ,[MaQuyCach] = @MaQuyCach
      ,[MaSize] = @MaSize
      ,[MaThanhPham] = @MaThanhPham
      ,[MaLo] = @MaLo
      ,[MaThe] = @MaThe
      ,[TrongLuong] = @TrongLuong
      ,[TrongLuongSec] = @TrongLuongSec
      ,[DinhMucThucTe] = @DinhMucThucTe
      ,[DinhMucYeuCau] = @DinhMucYeuCau
      ,[MaNhanVien] = @MaNhanVien
      ,[GhiChu] = @GhiChu
      ,[TrongLuongTare] = @TrongLuongTare
 WHERE [STT] = @STT
      and [Ngay] = @Ngay
      and [MaMayCan] = @MaMayCan
      and [MaXuong] = @MaXuong";

        private readonly string qrGetAll = "Select * from PhieuCanCaoThit";

        public PhieuCanCaoThit()
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
