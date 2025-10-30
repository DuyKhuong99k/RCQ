using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class TheThanhPham
    {
        private readonly string connectionString;
        private string tableName = @"TheThanhPham";
        private readonly string qrDelete = @"DELETE FROM [dbo].[TheThanhPham]
      WHERE [MaThe] = @MaThe ";

        private readonly string qrInsert = @"INSERT INTO [dbo].[TheThanhPham]
           ([MaThe]
           ,[MaThanhPham]
           ,[MaSizeDinhHinh]
           ,[TrongLuongTare]
           ,[MaMayLangDa]
           ,[MaThanhPhamFillet]
           ,[MaThanhPhamPhuPham]
           ,[MaSizeFillet]
           ,[IsZero]
           ,[MaLoaiNguyenLieu]
           ,[IsRestart]
           ,[MaQuyTrinhT]
           ,[STTPhieuPhanCoChiTietT]
           ,[MaCoiXepKhuon]
           ,[NgayGio]
           ,[MaSize])
     VALUES
           (@MaThe
           ,@MaThanhPham
           ,@MaSizeDinhHinh
           ,@TrongLuongTare
           ,@MaMayLangDa
           ,@MaThanhPhamFillet
           ,@MaThanhPhamPhuPham
           ,@MaSizeFillet
           ,@IsZero
           ,@MaLoaiNguyenLieu
           ,@IsRestart
           ,@MaQuyTrinhT
           ,@STTPhieuPhanCoChiTietT
           ,@MaCoiXepKhuon
           ,@NgayGio
           ,@MaSize)
";

        private readonly string qrUpdate = @"UPDATE [dbo].[TheThanhPham]
   SET 
      [MaThanhPham] = @MaThanhPham
      ,[MaSizeDinhHinh] = @MaSizeDinhHinh
      ,[TrongLuongTare] = @TrongLuongTare
      ,[MaMayLangDa] = @MaMayLangDa
      ,[MaThanhPhamFillet] = @MaThanhPhamFillet
      ,[MaThanhPhamPhuPham] = @MaThanhPhamPhuPham
      ,[MaSizeFillet] = @MaSizeFillet
      ,[IsZero] = @IsZero
      ,[MaLoaiNguyenLieu] = @MaLoaiNguyenLieu
      ,[IsRestart] = @IsRestart
      ,[MaQuyTrinhT] = @MaQuyTrinhT
      ,[STTPhieuPhanCoChiTietT] = @STTPhieuPhanCoChiTietT
      ,[MaCoiXepKhuon] = @MaCoiXepKhuon
      ,[NgayGio] = @NgayGio
      ,[MaSize] = @MaSize
 WHERE [MaThe] = @MaThe

";

        private readonly string qrGetAll = @"Select p.*,tpdh.Ten as ThanhPhamDHName,sdh.Ten as SizeDHName,tpfl.Ten as ThanhPhamFilletName,sfl.Ten as SizeFilletName,tppp.Ten as ThanhPhamPhuPhamName,tqt.Ten as QuyTrinhName,cxk.Ten as CoiXepKhuonName
from TheThanhPham p
LEFT JOIN MaThanhPhamDinhHinh tpdh on p.MaThanhPham = tpdh.Ma
LEFT JOIN MaSizeDinhHinh sdh on p.MaSizeDinhHinh = sdh.Ma
LEFT JOIN MaThanhPhamFillet tpfl on p.MaThanhPhamFillet = tpfl.Ma
LEFT JOIN MaSizeFillet sfl on p.MaSizeFillet = sfl.Ma
LEFT JOIN MaThanhPhamPhuPham tppp on p.MaThanhPhamPhuPham = tppp.Ma
LEFT JOIN T_QuyTrinh tqt on p.MaQuyTrinhT = tqt.Ma
LEFT JOIN MaCoiXepKhuon cxk on p.MaCoiXepKhuon = cxk.Ma
";

        public TheThanhPham()
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
        public int Delete<T>(List<T> items)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrDelete, items);
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
        public int Update<T>(List<T> items)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdate, items);
            return rows;
        }
    }
}
