using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanVungNuoi
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanVungNuoi";
        private readonly string qrDelete = @"
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanVungNuoi] ([STT] ,[Ngay] ,[MaMayCan] ,[NgayNhapXuong] ,[Gio] ,[MaLo] ,[MaUserCan] ,[GhiChu] ,[MaLoaiCa] ,[MaGhe] ,[MaAo] ,[MaCongDoan] ,[TrongLuong],[MaThongKeDauAo],[TrongLuongTare]) VALUES (@STT,@Ngay,@MaMayCan,@NgayNhapXuong,@Gio,@MaLo,@MaUserCan,@GhiChu,@MaLoaiCa,@MaGhe,@MaAo,@MaCongDoan,@TrongLuong,@MaThongKeDauAo,@TrongLuongTare)";

        private readonly string qrUpdate = @"
UPDATE [dbo].[PhieuCanVungNuoi] SET [NgayNhapXuong] = @NgayNhapXuong, [Gio] = @Gio, [MaLo] = @MaLo, [MaUserCan] = @MaUserCan,[GhiChu] = @GhiChu, [MaLoaiCa] = @MaLoaiCa, [MaGhe] = @MaGhe, [MaAo] = @MaAo,[MaCongDoan] = @MaCongDoan, [TrongLuong] = @TrongLuong,[MaThongKeDauAo]=@MaThongKeDauAo,[TrongLuongTare]=@TrongLuongTare WHERE [STT] =@STT and [Ngay] =@Ngay and MaMayCan] = @MaMayCan";

        private readonly string qrGetAll = "Select * from PhieuCanVungNuoi";

        public PhieuCanVungNuoi()
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
