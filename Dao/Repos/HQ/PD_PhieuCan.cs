using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PD_PhieuCan
    {
        private readonly string connectionString;
        private string tableName = @"PD_PhieuCan";
        private readonly string qrDelete = @"DELETE [dbo].[PD_PhieuCan] WHERE [STT] = @STT and [Ngay] = @ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";

        private readonly string qrInsert = @"
INSERT INTO [dbo].[PD_PhieuCan]
           ([STT]
           ,[Ngay]
           ,[MaMayCan]
           ,[MaXuong]
           ,[Gio]
           ,[MaSanPham]
           ,[TrongLuong]
           ,[MaCoi]
           ,[MaCongThuc]
           ,[SuDung]
           ,[MaLyDo]
           ,[GhiChu]
           ,[CreateDateTime]
           ,[CreateBy]
           ,[ModifiedDateTime]
           ,[ModifiedBy]
           ,[MaNhanVien],[TrongLuongNguyenLieu],[Khoa],[Block])
     VALUES
           (@STT
           ,@Ngay 
           ,@MaMayCan 
           ,@MaXuong 
           ,@Gio 
           ,@MaSanPham 
           ,@TrongLuong 
           ,@MaCoi 
           ,@MaCongThuc 
           ,@SuDung 
           ,@MaLyDo 
           ,@GhiChu 
           ,@CreateDateTime 
           ,@CreateBy 
           ,@ModifiedDateTime 
           ,@ModifiedBy 
           ,@MaNhanVien,@TrongLuongNguyenLieu,@Khoa ,@Block)
";

        private readonly string qrUpdate = @"UPDATE [dbo].[PD_PhieuCan]
   SET [Gio] = @Gio 
      ,[MaSanPham] = @MaSanPham 
      ,[TrongLuong] = @TrongLuong 
      ,[MaCoi] = @MaCoi 
      ,[MaCongThuc] = @MaCongThuc 
      ,[SuDung] = @SuDung 
      ,[MaLyDo] = @MaLyDo 
      ,[GhiChu] = @GhiChu 
      ,[CreateDateTime] = @CreateDateTime 
      ,[CreateBy] = @CreateBy 
      ,[ModifiedDateTime] = @ModifiedDateTime 
      ,[ModifiedBy] = @ModifiedBy 
      ,[MaNhanVien] = @MaNhanVien,[TrongLuongNguyenLieu] = @TrongLuongNguyenLieu,[Khoa] =@Khoa , [Block] = @Block
 WHERE [STT] = @STT and [Ngay] = @ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";

        private readonly string qrGetAll = "Select * from PD_PhieuCan";

        public PD_PhieuCan(string? _connectionString = null)
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
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan_XLPC<T>(DateTime dateTime, string xuongId)
        {
            var query = @"select
p.STT,
p.Ngay,
p.MaMayCan,
p.MaXuong,
x.Ten as XuongName,
p.Gio,
p.MaSanPham,
sp.Ten as SanPhamName,
p.TrongLuongNguyenLieu,
p.TrongLuong,
p.MaCoi,
c.Ten as CoiName,
p.MaCongThuc,
ct.Ten as CongThucName,
p.SuDung,
p.MaLyDo,
ld.Ten as LyDoName,
p.GhiChu,
p.CreateDateTime,
p.CreateBy,
p.ModifiedDateTime,
p.ModifiedBy,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
nv.DeptName0 as Nhom,
p.Khoa,
p.Block
from PD_PhieuCan p
left join PD_SanPham sp on p.MaSanPham = sp.Ma
left join MaCoiXepKhuon c on p.MaCoi = c.Ma
left join PD_CongThuc ct on p.MaCongThuc = ct.Ma
left join PD_LyDo ld on p.MaLyDo = ld.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join XiNghiep x on p.MaXuong = x.Ma
where p.Ngay = @dateTime and p.MaXuong = @xuongId
order by
p.STT desc";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { dateTime = dateTime.Date, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }
        #endregion
        public List<T> GetsChiTiet<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.*,
sp.Ten as SanPhamName,
ct.Ten as CongThucName,
ld.Ten as LyDoName,
n.MaHoSo,
n.Name as NhanVienName ,
p.Block ,
c.Ten as CoiChinhName
from PD_PhieuCan p,
PD_SanPham sp,
PD_CongThuc ct,
PD_LyDo ld,
NhanVienDaiThanh n,
MaCoiXepKhuon c
Where 
Ngay <= @toDate
and Ngay >= @fromDate
and MaXuong = @xuongId 
and p.MaSanPham = sp.Ma 
and p.MaCongThuc = ct.Ma 
and p.MaLyDo = ld.Ma 
and p.MaNhanVien = n.MaNhanVien 
and p.MaCoi = c.Ma
order by p.STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId })
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
        public List<T> GetsTongHop<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.MaCoi,
sp.Ten as SanPhamName,
ct.Ten as CongThucName ,
Sum(p.TrongLuong) as TongSanLuong,
Count(*) as SoRo ,
c.Ten as CoiChinhName
from 
PD_PhieuCan p, 
PD_SanPham sp,
PD_CongThuc ct ,
MaCoiXepKhuon c
Where 
Ngay <= @toDate
and Ngay >= @fromDate
and MaXuong = @xuongId 
and p.MaSanPham = sp.Ma 
and p.MaCongThuc = ct.Ma 
and p.MaCoi = c.Ma
Group By p.MaCoi, sp.Ten ,ct.Ten,c.Ten order by p.MaCoi,sp.Ten,ct.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId })
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
