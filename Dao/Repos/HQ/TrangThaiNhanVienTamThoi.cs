using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TrangThaiNhanVienTamThoi
    {
        private readonly string connectionString;
        private string tableName = @"TrangThaiNhanVienTamThoi";
        private readonly string qrDelete = @"DELETE FROM [dbo].[TrangThaiNhanVienTamThoi]
      WHERE [STT] = @STT
      and [Ngay] = @Ngay
      and [MaXuong] = @MaXuong and [KhuVuc] =@KhuVuc";

        private readonly string qrInsert = @"INSERT INTO [dbo].[TrangThaiNhanVienTamThoi]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MaNhanVien]
           ,[MaXuong]
           ,[IsPhucVu],[KhuVuc])
     VALUES
           (@STT
           ,@Ngay
           ,@Gio
           ,@MaNhanVien
           ,@MaXuong
           ,@IsPhucVu,@KhuVuc)";

        private readonly string qrUpdate = @"UPDATE [dbo].[TrangThaiNhanVienTamThoi]
   SET [Gio] = @Gio
      ,[MaNhanVien] = @MaNhanVien
      ,[IsPhucVu] = @IsPhucVu 
 WHERE [STT] = @STT
      and [Ngay] = @Ngay
      and [MaXuong] = @MaXuong and [KhuVuc] =@KhuVuc";

        private readonly string qrGetAll = "Select * from TrangThaiNhanVienTamThoi";

        public TrangThaiNhanVienTamThoi()
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

        public List<T> Gets<T>(DateTime dateTime)
        {
            try
            {
                var query = @"Select
p.STT,
p.Ngay,
p.MaXuong,
x.Ten as XuongName,
p.KhuVuc,
p.Gio,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as NhomName,
nv.Name as NhanVienName,
p.IsPhucVu
from TrangThaiNhanVienTamThoi p
left join XiNghiep x on p.MaXuong = x.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
where Ngay = @ngay order by STT DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date }).Result.ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
