using Dapper;
using Microsoft.Data.SqlClient;
using ToolsEx;

namespace Dao.Repos.HQ
{
    public partial class BoTriNhanVienTheoSize
    {
        private readonly string connectionString;
        private string tableName = @"BoTriNhanVienTheoSize";
        private readonly string qrDelete = @"DELETE FROM [dbo].[BoTriNhanVienTheoSize]
      WHERE [Ngay] = @Ngay
";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[BoTriNhanVienTheoSize]
          ( [Ngay]
           ,[MaNhanVien]
           ,[MaLo]
           ,[MaSize]
           ,[ThoiGianBatDau]
           ,[MaXuong]
           ,[SuDung])
     VALUES
          ( @Ngay
           ,@MaNhanVien
           ,@MaLo
           ,@MaSize
           ,@ThoiGianBatDau
           ,@MaXuong
           ,@SuDung)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[BoTriNhanVienTheoSize]
   SET [MaNhanVien] =@MaNhanVien
           ,[MaLo]=@MaLo
           ,[MaSize]=@MaSize
           ,[ThoiGianBatDau]=@ThoiGianBatDau
           ,[MaXuong]=@MaXuong
           ,[SuDung]=@SuDung
 WHERE [Ngay] = @Ngay
";

        private readonly string qrGetAll = "Select * from BoTriNhanVienTheoSize";

        public BoTriNhanVienTheoSize(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

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
         public List<T> GetNhanVienTheoSizeFullFileds<T>(DateTime ngay, string maLo,string xuongId)
        {
            try
            {
                var query = @"select
bt.Ngay,
bt.MaNhanVien,
nv.MaHoSo,
nv.Name,
nv.DeptName0,
bt.MaLo,
bt.MaSize,
s.Ten as SizeName,
bt.ThoiGianBatDau,
bt.MaXuong,
x.Ten as XuongName,
bt.SuDung
from BoTriNhanVienTheoSize bt
left join NhanVienDaiThanh nv on bt.MaNhanVien = nv.MaNhanVien
left join MaSizeFillet s on bt.MaSize = s.Ma
left join XiNghiep x on bt.MaXuong = x.Ma
where bt.Ngay = @ngay and bt.MaLo = @maLo and bt.MaXuong = @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { ngay = ngay.Date,maLo = maLo, xuongId = xuongId }).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
