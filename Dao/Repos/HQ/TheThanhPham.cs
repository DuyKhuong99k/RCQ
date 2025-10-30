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
           ,[MaSize]
           ,[LoLevel]
           ,[MaChatLuongChinhXepKhuon]
           ,[MaChatLuongPhuXepKhuon]
           ,[MaSizeChinhXepKhuon]
           ,[MaSizePhuXepKhuon]
           ,[MaThanhPhamChinhXepKhuon]
           ,[MaThanhPhamPhuXepKhuon],[MaChieuXa])
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
           ,@MaSize
           ,@LoLevel
           ,@MaChatLuongChinhXepKhuon
           ,@MaChatLuongPhuXepKhuon
           ,@MaSizeChinhXepKhuon
           ,@MaSizePhuXepKhuon
           ,@MaThanhPhamChinhXepKhuon
           ,@MaThanhPhamPhuXepKhuon,@MaChieuXa)
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
      ,[LoLevel] = @LoLevel
      ,[MaChatLuongChinhXepKhuon] = @MaChatLuongChinhXepKhuon
      ,[MaChatLuongPhuXepKhuon] = @MaChatLuongPhuXepKhuon
      ,[MaSizeChinhXepKhuon] = @MaSizeChinhXepKhuon
      ,[MaSizePhuXepKhuon] = @MaSizePhuXepKhuon
      ,[MaThanhPhamChinhXepKhuon] = @MaThanhPhamChinhXepKhuon
      ,[MaThanhPhamPhuXepKhuon] = @MaThanhPhamPhuXepKhuon
        ,[MaChieuXa] = @MaChieuXa
 WHERE [MaThe] = @MaThe

";

        private readonly string qrGetAll = @"Select 
p.*,
tpdh.Ten as ThanhPhamDHName,
sdh.Ten as SizeDHName,
tpfl.Ten as ThanhPhamFilletName,
sfl.Ten as SizeFilletName,
tppp.Ten as ThanhPhamPhuPhamName,
tqt.Ten as QuyTrinhName,
cxk.Ten as CoiXepKhuonName,
tpcxk.Ten as ThanhPhamChinhXepKhuonName,
scxk.Ten as SizeChinhXepKhuonName,
tppxk.Ten as ThanhPhamPhuXepKhuonName,
spxk.Ten as SizePhuXepKhuonName,
clxk.Ten as ChatLuongChinhXepKhuonName,
cxxk.Ten as ChieuXaXepKhuonName
from TheThanhPham p
LEFT JOIN MaThanhPhamDinhHinh tpdh on p.MaThanhPham = tpdh.Ma
LEFT JOIN MaSizeDinhHinh sdh on p.MaSizeDinhHinh = sdh.Ma
LEFT JOIN MaThanhPhamFillet tpfl on p.MaThanhPhamFillet = tpfl.Ma
LEFT JOIN MaSizeFillet sfl on p.MaSizeFillet = sfl.Ma
LEFT JOIN MaThanhPhamPhuPham tppp on p.MaThanhPhamPhuPham = tppp.Ma
LEFT JOIN T_QuyTrinh tqt on p.MaQuyTrinhT = tqt.Ma
LEFT JOIN MaCoiXepKhuon cxk on p.MaCoiXepKhuon = cxk.Ma
left join MaThanhPhamChinhXepKhuon tpcxk on p.MaThanhPhamChinhXepKhuon = tpcxk.Ma
left join MaSizeChinhXepKhuon scxk on p.MaSizeChinhXepKhuon = scxk.Ma
left join MaThanhPhamXepKhuon tppxk on p.MaThanhPhamPhuXepKhuon = tppxk.Ma
left join MaSizeXepKhuon spxk on p.MaSizePhuXepKhuon = spxk.Ma
left join MaChatLuongXepKhuon clxk on p.MaChatLuongChinhXepKhuon = clxk.Ma
left join MaChieuXaXepKhuon cxxk on p.MaChieuXa = cxxk.Ma
";
        private readonly string qrGetHqAll = @"Select 
p.*,
tp.Ten as ThanhPhamName,
s.Ten as SizeName,
lnl.Ten as LoaiNguyenLieuName

from TheThanhPham p
LEFT JOIN HQ_ThanhPham tp on p.MaThanhPham = tp.Id
LEFT JOIN HQ_Size s on p.MaSize = s.Id
LEFT JOIN HQ_LoaiNguyenLieu lnl on p.MaLoaiNguyenLieu = lnl.Id
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

        public List<T> GetHqs<T>()
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(qrGetHqAll).ToList();
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
