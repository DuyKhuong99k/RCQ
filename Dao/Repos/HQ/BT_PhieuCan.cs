using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class BT_PhieuCan
    {
        private readonly string connectionString;
        private string tableName = @"BT_PhieuCan";
        private readonly string qrDelete = @"Delete [dbo].[BT_PhieuCan]  WHERE [STT] = @STT 
      and [Ngay] = @Ngay 
      and [MaMayCan] = @MaMayCan 
      and [MaXuong] = @MaXuong ";

        private readonly string qrInsert = @"INSERT INTO [dbo].[BT_PhieuCan]
           ([STT]
           ,[Ngay]
           ,[MaMayCan]
           ,[MaXuong]
           ,[Gio]
           ,[MaLoaiCa]
           ,[MaThanhPham]
           ,[MaKhachHang]
           ,[MaNhanVien]
           ,[TrongLuong]
           ,[SuDung]
           ,[CreateDateTime]
           ,[CreateBy]
           ,[ModifiedDateTime]
           ,[ModifiedBy],[GhiChu])
     VALUES
           (@STT 
           ,@Ngay 
           ,@MaMayCan 
           ,@MaXuong 
           ,@Gio 
           ,@MaLoaiCa 
           ,@MaThanhPham 
           ,@MaKhachHang 
           ,@MaNhanVien 
           ,@TrongLuong 
           ,@SuDung 
           ,@CreateDateTime 
           ,@CreateBy 
           ,@ModifiedDateTime 
           ,@ModifiedBy,@GhiChu)";

        private readonly string qrUpdate = @"UPDATE [dbo].[BT_PhieuCan]
   SET [Gio] = @Gio 
      ,[MaLoaiCa] = @MaLoaiCa 
      ,[MaThanhPham] = @MaThanhPham 
      ,[MaKhachHang] = @MaKhachHang 
      ,[MaNhanVien] = @MaNhanVien 
      ,[TrongLuong] = @TrongLuong 
      ,[SuDung] = @SuDung 
      ,[CreateDateTime] = @CreateDateTime 
      ,[CreateBy] = @CreateBy 
      ,[ModifiedDateTime] = @ModifiedDateTime 
      ,[ModifiedBy] = @ModifiedBy,[GhiChu]=@GhiChu
 WHERE [STT] = @STT 
      and [Ngay] = @Ngay 
      and [MaMayCan] = @MaMayCan 
      and [MaXuong] = @MaXuong ";

        private readonly string qrGetAll = "Select * from BT_PhieuCan";

        public BT_PhieuCan(string? _connectionString = null)
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
        public List<T> Gets<T>(DateTime dateTime)
        {
            var query = @"Select
    p.*,
    ISNULL(tp.BravoId, 'NONE') as BravoId,
    ISNULL(kh.Ten, 'NONE') as KhachHangName,
    ISNULL(tp.Ten,'') as ThanhPhamName
from
    (
        Select
            *
        from
            BT_PhieuCan p
        where
            p.Ngay = @ngay
    ) p
    left JOIN BT_MaThanhPham tp on p.MaThanhPham = tp.Ma
    LEFT JOIN BT_KhachHang kh on p.MaKhachHang = kh.Ma";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { ngay = dateTime.Date })
                .ToList();
            return items;
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
P.MaXuong,
x.Ten as XuongName,
P.Gio,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
nv.DeptName0 as Nhom,
p.TrongLuong,
p.SuDung,
p.CreateDateTime,
p.CreateBy,
p.ModifiedDateTime,
p.ModifiedBy,
p.GhiChu
from BT_PhieuCan p
left join XiNghiep x on p.MaXuong = x.Ma
left join BT_MaLoaiCa lc on p.MaLoaiCa = lc.Ma
left join BT_MaThanhPham tp on p.MaThanhPham = tp.Ma
left join BT_KhachHang kh on p.MaKhachHang = kh.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
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
        #region Tính Lương
        public List<T> GetsTongHopsTinhLuong<T>(DateTime dateTime, string xuongId)
        {
            var query = @"Select p.* from
(Select p.MaThanhPham as MaCongViec,p.Ngay,tp.Ten as TenCongViec,p.MaNhanVien,n.MaHoSo,tp.BravoId,'CT04' as CaId,SUM(p.TrongLuong) as SanLuongHuong from BT_PhieuCan p,BT_MaThanhPham tp,NhanVienDaiThanh n where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaThanhPham = tp.Ma and STT>0 and p.MaNhanVien = n.MaNhanVien group by p.MaThanhPham,p.MaNhanVien,tp.BravoId,n.MaHoSo,tp.Ten,p.Ngay) p";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result
                    .ToList();
                return items;
            }
        }
        #endregion
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var query =
                @"Select
p.STT,
p.Ngay,
p.MaMayCan,
p.MaXuong,
p.Gio,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
p.TrongLuong,
p.SuDung,
p.CreateDateTime,
p.CreateBy,
p.ModifiedDateTime,
p.ModifiedBy,
p.GhiChu
from BT_PhieuCan p
left join BT_MaLoaiCa lc on p.MaLoaiCa = lc.Ma
left join BT_MaThanhPham tp on p.MaThanhPham = tp.Ma
left join BT_KhachHang kh on p.MaKhachHang = kh.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
where 
Ngay >= @fromDate 
and  Ngay <= @toDate 
and MaXuong = @xuongId";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new
                        {
                            fromDate = fromDate.Date,
                            toDate = toDate.Date,
                            xuongId
                        })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetChiTietByMaNhanViens<T>(DateTime fromDate, DateTime toDate,string maNhanVien, string xuongId)
        {
            var query =
                @"Select
p.STT,
p.Ngay,
p.MaMayCan,
p.MaXuong,
p.Gio,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
p.TrongLuong,
p.SuDung,
p.CreateDateTime,
p.CreateBy,
p.ModifiedDateTime,
p.ModifiedBy,
p.GhiChu
from BT_PhieuCan p
left join BT_MaLoaiCa lc on p.MaLoaiCa = lc.Ma
left join BT_MaThanhPham tp on p.MaThanhPham = tp.Ma
left join BT_KhachHang kh on p.MaKhachHang = kh.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
where 
Ngay >= @fromDate 
and  Ngay <= @toDate 
and MaXuong = @xuongId
and p.MaNhanVien = @maNhanVien";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new
                        {
                            fromDate = fromDate.Date,
                            toDate = toDate.Date,
                            xuongId,
                            maNhanVien
                        })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetChiTietByMaHoSos<T>(DateTime fromDate, DateTime toDate,string maHoSo, string xuongId)
        {
            var query =
                @"Select
p.STT,
p.Ngay,
p.MaMayCan,
p.MaXuong,
p.Gio,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
p.TrongLuong,
p.SuDung,
p.CreateDateTime,
p.CreateBy,
p.ModifiedDateTime,
p.ModifiedBy,
p.GhiChu
from BT_PhieuCan p
left join BT_MaLoaiCa lc on p.MaLoaiCa = lc.Ma
left join BT_MaThanhPham tp on p.MaThanhPham = tp.Ma
left join BT_KhachHang kh on p.MaKhachHang = kh.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
where 
Ngay >= @fromDate 
and  Ngay <= @toDate 
and MaXuong = @xuongId
and nv.MaHoSo = @maHoSo";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new
                        {
                            fromDate = fromDate.Date,
                            toDate = toDate.Date,
                            xuongId,
                            maHoSo
                        })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetChiTietByMaThes<T>(DateTime fromDate, DateTime toDate,string maThe, string xuongId)
        {
            var query =
                @"Select
p.STT,
p.Ngay,
p.MaMayCan,
p.MaXuong,
p.Gio,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
p.TrongLuong,
p.SuDung,
p.CreateDateTime,
p.CreateBy,
p.ModifiedDateTime,
p.ModifiedBy,
p.GhiChu
from BT_PhieuCan p
left join BT_MaLoaiCa lc on p.MaLoaiCa = lc.Ma
left join BT_MaThanhPham tp on p.MaThanhPham = tp.Ma
left join BT_KhachHang kh on p.MaKhachHang = kh.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join TheTu t on p.MaNhanVien = t.MaNhanVien
where 
Ngay >= @fromDate 
and  Ngay <= @toDate 
and MaXuong = @xuongId
and nv.MaHoSo = @maHoSo
and t.MaTheTu = @maThe";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new
                        {
                            fromDate = fromDate.Date,
                            toDate = toDate.Date,
                            xuongId,
                            maThe
                        })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHops<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
//            var query =
//                @"Select 
//n.MaHoSo,
//p.Ngay,
//p.MaXuong,
//x.Ten as XuongName,
//p.MaMayCan,
//p.MaLoaiCa,
//la.Ten as LoaiCaName,
//p.MaThanhPham,
//tp.Ten as ThanhPhamName,
//p.MaKhachHang,
//kh.Ten as KhachHangName,
//p.MaNhanVien,
//n.Name as NhanVienName,
//Count(*) as SoRo,
//Sum(p.TrongLuong) as TrongLuong 
//from BT_PhieuCan p,
//BT_MaLoaica la,
//BT_MaThanhPham tp,
//BT_KhachHang kh,
//NhanVienDaiThanh n,
//XiNghiep x
//where 
//Ngay >= @fromDate 
//and Ngay <= @toDate 
//and MaXuong = @xuongId 
//and p.MaKhachHang=kh.Ma 
//and p.MaLoaiCa= la.Ma 
//and p.MaThanhPham = tp.Ma 
//and p.MaNhanVien = n.MaNhanVien 
//and p.MaXuong = x.Ma
//group by 
//p.Ngay,
//p.MaXuong,
//p.MaMayCan,
//p.MaLoaiCa,
//la.Ten ,
//p.MaThanhPham,
//tp.Ten ,
//p.MaKhachHang,
//kh.Ten ,
//p.MaNhanVien,
//n.Name ,
//n.MaHoSo,
//x.Ten ";
var query =
                @"Select 
n.MaHoSo,
p.Ngay,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.MaLoaiCa,
la.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaNhanVien,
n.Name as NhanVienName,
Count(*) as SoRo,
Sum(p.TrongLuong) as TrongLuong ,
MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianVao,
MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianRa,
DATEDIFF(hour, MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay), MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay)) as TongThoiGian
from BT_PhieuCan p,
BT_MaLoaica la,
BT_MaThanhPham tp,
BT_KhachHang kh,
NhanVienDaiThanh n,
XiNghiep x,
CheckInOut c
where 
Ngay >= @fromDate 
and Ngay <= @toDate 
and MaXuong = @xuongId 
and p.MaKhachHang=kh.Ma 
and p.MaLoaiCa= la.Ma 
and p.MaThanhPham = tp.Ma 
and p.MaNhanVien = n.MaNhanVien 
and p.MaXuong = x.Ma
AND n.MaChamCong = c.MaChamCong AND c.ThoiGian = p.Ngay AND c.ThoiGian >= @fromDate AND c.ThoiGian <= @toDate 
group by 
p.Ngay,
p.MaXuong,
p.MaMayCan,
p.MaLoaiCa,
la.Ten ,
p.MaThanhPham,
tp.Ten ,
p.MaKhachHang,
kh.Ten ,
p.MaNhanVien,
n.Name ,
n.MaHoSo,
x.Ten ,
p.Ngay,
n.MaChamCong,
c.ThoiGian";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new
                        {
                            fromDate = fromDate.Date,
                            toDate = toDate.Date,
                            xuongId
                        })
                    .Result
                    .ToList();
                return items;
            }
        }
         public List<T> GetTongHopThanhPhamByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            var query =
                @"Select 
n.MaHoSo,
p.Ngay,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.MaLoaiCa,
la.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaNhanVien,
n.Name as NhanVienName,
Count(*) as SoRo,
Sum(p.TrongLuong) as TrongLuong 
from BT_PhieuCan p,
BT_MaLoaica la,
BT_MaThanhPham tp,
BT_KhachHang kh,
NhanVienDaiThanh n,
XiNghiep x
where 
p.Ngay >= @fromDate 
and p.Ngay <= @toDate 
and p.MaXuong = @xuongId
and p.MaNhanVien = @maNhanVien 
and p.MaKhachHang=kh.Ma 
and p.MaLoaiCa= la.Ma 
and p.MaThanhPham = tp.Ma 
and p.MaNhanVien = n.MaNhanVien 
and p.MaXuong = x.Ma
group by 
p.Ngay,
p.MaXuong,
p.MaMayCan,
p.MaLoaiCa,
la.Ten ,
p.MaThanhPham,
tp.Ten ,
p.MaKhachHang,
kh.Ten ,
p.MaNhanVien,
n.Name ,
n.MaHoSo,
x.Ten ";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new
                        {
                            fromDate = fromDate.Date,
                            toDate = toDate.Date,
                            maNhanVien,
                            xuongId
                        })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopThanhPhamByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            var query =
                @"Select 
n.MaHoSo,
p.Ngay,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.MaLoaiCa,
la.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaNhanVien,
n.Name as NhanVienName,
Count(*) as SoRo,
Sum(p.TrongLuong) as TrongLuong 
from BT_PhieuCan p,
BT_MaLoaica la,
BT_MaThanhPham tp,
BT_KhachHang kh,
NhanVienDaiThanh n,
XiNghiep x
where 
p.Ngay >= @fromDate 
and p.Ngay <= @toDate 
and p.MaXuong = @xuongId
and n.MaHoSo = @maHoSo 
and p.MaKhachHang=kh.Ma 
and p.MaLoaiCa= la.Ma 
and p.MaThanhPham = tp.Ma 
and p.MaNhanVien = n.MaNhanVien 
and p.MaXuong = x.Ma
group by 
p.Ngay,
p.MaXuong,
p.MaMayCan,
p.MaLoaiCa,
la.Ten ,
p.MaThanhPham,
tp.Ten ,
p.MaKhachHang,
kh.Ten ,
p.MaNhanVien,
n.Name ,
n.MaHoSo,
x.Ten ";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new
                        {
                            fromDate = fromDate.Date,
                            toDate = toDate.Date,
                            maHoSo,
                            xuongId
                        })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopThanhPhamByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            var query =
                @"Select 
n.MaHoSo,
p.Ngay,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.MaLoaiCa,
la.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaNhanVien,
n.Name as NhanVienName,
Count(*) as SoRo,
Sum(p.TrongLuong) as TrongLuong 
from BT_PhieuCan p,
BT_MaLoaica la,
BT_MaThanhPham tp,
BT_KhachHang kh,
NhanVienDaiThanh n,
XiNghiep x,
TheTu t
where 
p.Ngay >= @fromDate 
and p.Ngay <= @toDate 
and p.MaXuong = @xuongId
and p.MaNhanVien = t.MaNhanVien
and t.MaTheTu = @maThe 
and p.MaKhachHang=kh.Ma 
and p.MaLoaiCa= la.Ma 
and p.MaThanhPham = tp.Ma 
and p.MaNhanVien = n.MaNhanVien 
and p.MaXuong = x.Ma
group by 
p.Ngay,
p.MaXuong,
p.MaMayCan,
p.MaLoaiCa,
la.Ten ,
p.MaThanhPham,
tp.Ten ,
p.MaKhachHang,
kh.Ten ,
p.MaNhanVien,
n.Name ,
n.MaHoSo,
x.Ten ";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new
                        {
                            fromDate = fromDate.Date,
                            toDate = toDate.Date,
                            maThe,
                            xuongId
                        })
                    .Result
                    .ToList();
                return items;
            }
        }
    }
}
