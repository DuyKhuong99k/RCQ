using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanXepKhuonKHC
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanXepKhuonKHC";
        private readonly string qrDelete = "Delete [dbo].[PhieuCanXepKhuonKHC] WHERE [STT] = @STT and [Ngay] =@Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanXepKhuonKHC] ([STT] ,[Ngay] ,[Gio] ,[MaUserCan] ,[MaXuong] ,[MaMayCan] ,[GhiChu] ,[MaLo] ,[MaLoaiCa] ,[MaThanhPham] ,[MaSize] ,[MaKhachHang] ,[MaCoiTam] ,[TrongLuong] ,[DaXacNhan] ,[Block],[MaNhanVien]) VALUES (@STT,@Ngay,@Gio,@MaUserCan,@MaXuong,@MaMayCan,@GhiChu,@MaLo,@MaLoaiCa,@MaThanhPham,@MaSize,@MaKhachHang,@MaCoiTam,@TrongLuong,@DaXacNhan,@Block,@MaNhanVien)";

        private readonly string qrUpdate = @"
UPDATE [dbo].[PhieuCanXepKhuonKHC] SET  [Gio] = @Gio, [MaUserCan] = @MaUserCan,  [GhiChu] =@GhiChu, [MaLo] = @MaLo, [MaLoaiCa] =@MaLoaiCa, [MaThanhPham] = @MaThanhPham, [MaSize] = @MaSize, [MaKhachHang] =@MaKhachHang, [MaCoiTam] = @MaCoiTam, [TrongLuong] = @TrongLuong, [DaXacNhan] = @DaXacNhan ,[Block] = @Block, [MaNhanVien] = @MaNhanVien WHERE [STT] = @STT and [Ngay] =@Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";

        private readonly string qrGetAll = "Select * from PhieuCanXepKhuonKHC";
        private readonly string qrGetsLastByNumAndMayCan = @"WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MaMayCan ORDER BY Ngay DESC, Gio DESC) AS RowNum
    FROM PhieuCanXepKhuonKHC where Ngay =@ngay
)

SELECT *
FROM RankedPhieu
WHERE RowNum <= @num";
        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var query = qrGetsLastByNumAndMayCan;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, num })
                    .Result
                    .ToList();
                return items;
            }
        }
        public PhieuCanXepKhuonKHC(string? _connectionString = null)
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
        public List<T> GetPhieuCanKXLXepKhuonsByDateToDate<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
p.Ngay,
p.MaXuong,
p.MaMayCan,
p.Gio,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaCoiTam,
c.Ten as CoiTamName,
p.TrongLuong,
p.DaXacNhan,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
p.Block
from PhieuCanXepKhuonKHC p
left join MaLoaiCaXepKhuon lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamXepKhuonKHC tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuonKHC s on p.MaSize = s.Ma
left join MaKhachHangXepKhuon kh on p.MaKhachHang = kh.Ma
left join MaCoiXepKhuon c on p.MaCoiTam = p.MaCoiTam
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
where 
p.Ngay <= @toDate 
and p.Ngay >= @fromDate 
and MaXuong = @xuongId";
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
        public List<T> GetPhieuCanKXLXepKhuonsByMaNhanVien<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
p.Ngay,
p.MaXuong,
p.MaMayCan,
p.Gio,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaCoiTam,
c.Ten as CoiTamName,
p.TrongLuong,
p.DaXacNhan,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
p.Block
from PhieuCanXepKhuonKHC p
left join MaLoaiCaXepKhuon lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamXepKhuonKHC tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuonKHC s on p.MaSize = s.Ma
left join MaKhachHangXepKhuon kh on p.MaKhachHang = kh.Ma
left join MaCoiXepKhuon c on p.MaCoiTam = p.MaCoiTam
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
where 
p.Ngay <= @toDate 
and p.Ngay >= @fromDate 
and MaXuong = @xuongId
and p.MaNhanVien = @maNhanVien";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId ,maNhanVien})
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
        public List<T> GetPhieuCanKXLXepKhuonsByMaHoSo<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
p.Ngay,
p.MaXuong,
p.MaMayCan,
p.Gio,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaCoiTam,
c.Ten as CoiTamName,
p.TrongLuong,
p.DaXacNhan,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
p.Block
from PhieuCanXepKhuonKHC p
left join MaLoaiCaXepKhuon lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamXepKhuonKHC tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuonKHC s on p.MaSize = s.Ma
left join MaKhachHangXepKhuon kh on p.MaKhachHang = kh.Ma
left join MaCoiXepKhuon c on p.MaCoiTam = p.MaCoiTam
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
where 
p.Ngay <= @toDate 
and p.Ngay >= @fromDate 
and MaXuong = @xuongId
and nv.MaHoSo = @maHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId ,maHoSo})
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
        public List<T> GetPhieuCanKXLXepKhuonsByMaThe<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
p.Ngay,
p.MaXuong,
p.MaMayCan,
p.Gio,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaCoiTam,
c.Ten as CoiTamName,
p.TrongLuong,
p.DaXacNhan,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
p.Block
from PhieuCanXepKhuonKHC p
left join MaLoaiCaXepKhuon lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamXepKhuonKHC tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuonKHC s on p.MaSize = s.Ma
left join MaKhachHangXepKhuon kh on p.MaKhachHang = kh.Ma
left join MaCoiXepKhuon c on p.MaCoiTam = p.MaCoiTam
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join TheTu t on p.MaNhanVien = t.MaNhanVien
where 
p.Ngay <= @toDate 
and p.Ngay >= @fromDate 
and MaXuong = @xuongId
and t.MaTheTu = @maThe";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId ,maThe})
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



        public List<T> GetPhieuCanTongHopKXLXepKhuonsByDateToDate<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.MaNhanVien,
n.MaHoSo,
n.[Name] as NhanVienName,
p.MaLo,
tp.Ten as ThanhPhamName,
s.Ten as SizeName,
kh.Ten as KhachHangName,
Sum( p.TrongLuong) as TrongLuong,
COUNT(*) As SoRo
from PhieuCanXepKhuonKHC p, 
NhanVienDaiThanh n,
MaThanhPhamXepKhuonKHC tp,
MaSizeXepKhuonKHC s,
MaKhachHangXepKhuon kh
where
p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong =@xuongId 
and p.MaNhanVien = n.MaNhanVien 
and p.MaThanhPham = tp.Ma 
and p.MaSize = s.Ma 
and p.MaKhachHang = kh.Ma 
group by 
p.MaNhanVien,
n.MaHoSo,
n.[Name] ,
p.MaLo,
tp.Ten ,
s.Ten ,
kh.Ten 
order by 
n.MaHoSo,
p.MaLo,
kh.Ten,
tp.Ten,
s.Ten";
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
        public int Delete(PhieuCanXepKhuonKHC phieuCan)
        {
            try
            {
                var query =
                    "Delete [dbo].[PhieuCanXepKhuonKHC] WHERE [STT] = @STT and [Ngay] =@Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.ExecuteAsync(query, phieuCan).Result;
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Delete<T>(List<T> phieuCans)
        {
            try
            {
                var query =
                    "Delete [dbo].[PhieuCanXepKhuonKHC] WHERE [STT] = @STT and [Ngay] =@Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.ExecuteAsync(query, phieuCans).Result;
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PhieuCanXepKhuonKHC> Gets()
        {
            try
            {
                var query = "Select * from PhieuCanXepKhuonKHC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<PhieuCanXepKhuonKHC>(query).Result.ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<PhieuCanXepKhuonKHC> Gets(DateTime dateTime)
        {
            try
            {
                var query = "Select * from PhieuCanXepKhuonKHC where Ngay = @ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<PhieuCanXepKhuonKHC>(query, new { ngay = dateTime.Date })
                        .Result
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

        public List<PhieuCanXepKhuonKHC> Gets(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = "Select * from PhieuCanXepKhuonKHC where Ngay = @ngay and MaXuong = @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<PhieuCanXepKhuonKHC>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId
                            })
                        .Result
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

        public List<T> Gets<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select p.MaNhanVien,n.MaHoSo,n.[Name] as TenNhanVien, p.MaLo,tp.Ten as ThanhPhamName,s.Ten as SizeName,kh.Ten as KhachHangName,Sum( p.TrongLuong) as TrongLuong, COUNT(*) As SoRo from PhieuCanXepKhuonKHC p, NhanVienDaiThanh n,MaThanhPhamXepKhuonKHC tp,MaSizeXepKhuonKHC s, MaKhachHangXepKhuon kh where p.Ngay = @ngay and p.MaXuong =@xuongId and p.MaNhanVien = n.MaNhanVien and p.MaThanhPham = tp.Ma and p.MaSize = s.Ma and p.MaKhachHang = kh.Ma group by p.MaNhanVien,n.MaHoSo,n.[Name] , p.MaLo,tp.Ten ,s.Ten ,kh.Ten order by n.MaHoSo,p.MaLo,kh.Ten,tp.Ten,s.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId = xuongId }).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PhieuCanXepKhuonKHC> Gets(DateTime dateTime, string xuongId, string mayCan)
        {
            try
            {
                var query =
                    "Select * from PhieuCanXepKhuonKHC where Ngay = @ngay and MaXuong = @xuongId and MaMayCan = @mayCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<PhieuCanXepKhuonKHC>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId,
                                mayCan = mayCan
                            })
                        .Result
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

        public decimal GetSanLuong(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> thanhPhamIds)
        {
            try
            {
                var listOfIdsJoined = $@"('{string.Join("','", thanhPhamIds.ToArray())}')";
                var query =
                    $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanXepKhuonKHC p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaThanhPham in {listOfIdsJoined} and p.Gio >= @fromTime and p.Gio < @toTime";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = (decimal)connection.ExecuteScalar(
                        query,
                        new
                        {
                            ngay = dateTime.Date,
                            fromTime = fromTime,
                            toTime = toTime,
                            xuongId = xuongId
                        });
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public decimal GetSanLuong(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> thanhPhamIds,
            string nhanVienId)
        {
            try
            {
                var listOfIdsJoined = $@"('{string.Join("','", thanhPhamIds.ToArray())}')";
                var query =
                    $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanXepKhuonKHC p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaThanhPham in {listOfIdsJoined} and p.Gio >= @fromTime and p.Gio < @toTime and MaNhanVien = @nhanVienId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = (decimal)connection.ExecuteScalar(
                        query,
                        new
                        {
                            ngay = dateTime.Date,
                            fromTime = fromTime,
                            toTime = toTime,
                            xuongId = xuongId,
                            nhanVienId = nhanVienId
                        });
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetSanLuongTinhLuong<T>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> nhanVienIdsEx,
            TimeSpan mocThoiGian)
        {
            try
            {
                var listOfIdsJoined = $@"('{string.Join("','", nhanVienIdsEx.ToArray())}')";
                var query =
                    $@"Select p.Ngay, CaId as CaLamViec,p.MaNhanVien,tp.BravoId as MaSanPham,tp.Ten as TenSanPham, Sum(p.TrongLuong) as TrongLuong,0 as _Status from 
(Select p.*, (case when p.Gio < @mocThoiGian then 'CT04' else 'CT05' end) as CaId   from PhieuCanXepKhuonKHC p where p.Ngay=@ngay and p.MaXuong = @xuongId ) p,MaThanhPhamXepKhuonKHC tp
where p.MaThanhPham = tp.Ma and tp.BravoId is not null and p.MaNhanVien not in {listOfIdsJoined}
group by p.CaId,p.Ngay,p.MaNhanVien,tp.BravoId,tp.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId,
                                mocThoiGian = mocThoiGian
                            })
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetSanLuongTinhLuongInIds<T>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> nhanVienIdsEx,
            TimeSpan mocThoiGian)
        {
            try
            {
                var listOfIdsJoined = $@"('{string.Join("','", nhanVienIdsEx.ToArray())}')";
                var query =
                    $@"Select p.Ngay, CaId as CaLamViec,p.MaNhanVien,tp.BravoId as MaSanPham,tp.Ten as TenSanPham, Sum(p.TrongLuong) as TrongLuong,0 as _Status from 
(Select p.*, (case when p.Gio < @mocThoiGian then 'CT04' else 'CT05' end) as CaId   from PhieuCanXepKhuonKHC p where p.Ngay=@ngay and p.MaXuong = @xuongId ) p,MaThanhPhamXepKhuonKHC tp
where p.MaThanhPham = tp.Ma and tp.BravoId is not null and p.MaNhanVien in {listOfIdsJoined}
group by p.CaId,p.Ngay,p.MaNhanVien,tp.BravoId,tp.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId,
                                mocThoiGian = mocThoiGian
                            })
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetsTongHopTheoNhanVien<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select p.MaNhanVien,n.MaHoSo,n.[Name] as TenNhanVien,tp.Ten as ThanhPhamName,s.Ten as SizeName,kh.Ten as KhachHangName,Sum( p.TrongLuong) as TrongLuong, COUNT(*) As SoRo from PhieuCanXepKhuonKHC p, NhanVienDaiThanh n,MaThanhPhamXepKhuonKHC tp,MaSizeXepKhuonKHC s, MaKhachHangXepKhuon kh where p.Ngay = @ngay and p.MaXuong =@xuongId and p.MaNhanVien = n.MaNhanVien and p.MaThanhPham = tp.Ma and p.MaSize = s.Ma and p.MaKhachHang = kh.Ma group by p.MaNhanVien,n.MaHoSo,n.[Name] ,tp.Ten ,s.Ten ,kh.Ten order by n.MaHoSo,tp.Ten,s.Ten,kh.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId = xuongId }).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetsTongHopTheoNhanVien<T>(DateTime fromDate,DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.MaNhanVien,
n.MaHoSo,
n.[Name] as NhanVienName,
tp.Ten as ThanhPhamName,
s.Ten as SizeName,
kh.Ten as KhachHangName,
Sum( p.TrongLuong) as TrongLuong, 
COUNT(*) As SoRo 
from PhieuCanXepKhuonKHC p,
NhanVienDaiThanh n,
MaThanhPhamXepKhuonKHC tp,
MaSizeXepKhuonKHC s, 
MaKhachHangXepKhuon kh
where 
p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong =@xuongId 
and p.MaNhanVien = n.MaNhanVien 
and p.MaThanhPham = tp.Ma 
and p.MaSize = s.Ma 
and p.MaKhachHang = kh.Ma 
group by 
p.MaNhanVien,
n.MaHoSo,
n.[Name] ,
tp.Ten ,
s.Ten ,
kh.Ten 
order by 
n.MaHoSo,
tp.Ten,
s.Ten,
kh.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date,toDate=toDate.Date, xuongId = xuongId }).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetsTongHopThanhPhamByMaNhanViens<T>(DateTime fromDate,DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.MaNhanVien,
n.MaHoSo,
n.[Name] as NhanVienName,
tp.Ten as ThanhPhamName,
s.Ten as SizeName,
kh.Ten as KhachHangName,
Sum( p.TrongLuong) as TrongLuong, 
COUNT(*) As SoRo 
from PhieuCanXepKhuonKHC p,
NhanVienDaiThanh n,
MaThanhPhamXepKhuonKHC tp,
MaSizeXepKhuonKHC s, 
MaKhachHangXepKhuon kh
where 
p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong =@xuongId 
and p.MaNhanVien = n.MaNhanVien 
and p.MaThanhPham = tp.Ma 
and p.MaSize = s.Ma 
and p.MaKhachHang = kh.Ma 
and p.MaNhanVien = @maNhanVien
group by 
p.MaNhanVien,
n.MaHoSo,
n.[Name] ,
tp.Ten ,
s.Ten ,
kh.Ten 
order by 
n.MaHoSo,
tp.Ten,
s.Ten,
kh.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date,toDate=toDate.Date, maNhanVien = maNhanVien , xuongId = xuongId}).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetsTongHopThanhPhamByMaHoSos<T>(DateTime fromDate,DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.MaNhanVien,
n.MaHoSo,
n.[Name] as NhanVienName,
tp.Ten as ThanhPhamName,
s.Ten as SizeName,
kh.Ten as KhachHangName,
Sum( p.TrongLuong) as TrongLuong, 
COUNT(*) As SoRo 
from PhieuCanXepKhuonKHC p,
NhanVienDaiThanh n,
MaThanhPhamXepKhuonKHC tp,
MaSizeXepKhuonKHC s, 
MaKhachHangXepKhuon kh
where 
p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong =@xuongId 
and p.MaNhanVien = n.MaNhanVien 
and p.MaThanhPham = tp.Ma 
and p.MaSize = s.Ma 
and p.MaKhachHang = kh.Ma 
and n.MaHoSo = @maHoSo
group by 
p.MaNhanVien,
n.MaHoSo,
n.[Name] ,
tp.Ten ,
s.Ten ,
kh.Ten 
order by 
n.MaHoSo,
tp.Ten,
s.Ten,
kh.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date,toDate=toDate.Date, maHoSo = maHoSo , xuongId = xuongId}).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetsTongHopThanhPhamByMaThes<T>(DateTime fromDate,DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.MaNhanVien,
n.MaHoSo,
n.[Name] as NhanVienName,
tp.Ten as ThanhPhamName,
s.Ten as SizeName,
kh.Ten as KhachHangName,
Sum( p.TrongLuong) as TrongLuong, 
COUNT(*) As SoRo 
from PhieuCanXepKhuonKHC p,
NhanVienDaiThanh n,
MaThanhPhamXepKhuonKHC tp,
MaSizeXepKhuonKHC s, 
MaKhachHangXepKhuon kh,TheTu t
where 
p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong =@xuongId 
and p.MaNhanVien = n.MaNhanVien 
and p.MaThanhPham = tp.Ma 
and p.MaSize = s.Ma 
and p.MaKhachHang = kh.Ma 
and p.MaNhanVien = t.MaNhanVien
and t.MaTheTu = @maThe
group by 
p.MaNhanVien,
n.MaHoSo,
n.[Name] ,
tp.Ten ,
s.Ten ,
kh.Ten 
order by 
n.MaHoSo,
tp.Ten,
s.Ten,
kh.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date,toDate=toDate.Date, maThe = maThe , xuongId = xuongId}).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Insert(PhieuCanXepKhuonKHC phieuCan)
        {
            try
            {
                var query =
                    "INSERT INTO [dbo].[PhieuCanXepKhuonKHC] ([STT] ,[Ngay] ,[Gio] ,[MaUserCan] ,[MaXuong] ,[MaMayCan] ,[GhiChu] ,[MaLo] ,[MaLoaiCa] ,[MaThanhPham] ,[MaSize] ,[MaKhachHang] ,[MaCoiTam] ,[TrongLuong] ,[DaXacNhan] ,[Block],[MaNhanVien]) VALUES (@STT,@Ngay,@Gio,@MaUserCan,@MaXuong,@MaMayCan,@GhiChu,@MaLo,@MaLoaiCa,@MaThanhPham,@MaSize,@MaKhachHang,@MaCoiTam,@TrongLuong,@DaXacNhan,@Block,@MaNhanVien)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.ExecuteAsync(query, phieuCan).Result;
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Insert<T>(List<T> phieuCans)
        {
            try
            {
                var query =
                    "INSERT INTO [dbo].[PhieuCanXepKhuonKHC] ([STT] ,[Ngay] ,[Gio] ,[MaUserCan] ,[MaXuong] ,[MaMayCan] ,[GhiChu] ,[MaLo] ,[MaLoaiCa] ,[MaThanhPham] ,[MaSize] ,[MaKhachHang] ,[MaCoiTam] ,[TrongLuong] ,[DaXacNhan] ,[Block],[MaNhanVien]) VALUES (@STT,@Ngay,@Gio,@MaUserCan,@MaXuong,@MaMayCan,@GhiChu,@MaLo,@MaLoaiCa,@MaThanhPham,@MaSize,@MaKhachHang,@MaCoiTam,@TrongLuong,@DaXacNhan,@Block,@MaNhanVien)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.ExecuteAsync(query, phieuCans).Result;
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Update(PhieuCanXepKhuonKHC phieuCan)
        {
            try
            {
                var query =
                    "UPDATE [dbo].[PhieuCanXepKhuonKHC] SET  [Gio] = @Gio, [MaUserCan] = @MaUserCan,  [GhiChu] =@GhiChu, [MaLo] = @MaLo, [MaLoaiCa] =@MaLoaiCa, [MaThanhPham] = @MaThanhPham, [MaSize] = @MaSize, [MaKhachHang] =@MaKhachHang, [MaCoiTam] = @MaCoiTam, [TrongLuong] = @TrongLuong, [DaXacNhan] = @DaXacNhan ,[Block] = @Block, [MaNhanVien] = @MaNhanVien WHERE [STT] = @STT and [Ngay] =@Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.ExecuteAsync(query, phieuCan).Result;
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Update(List<PhieuCanXepKhuonKHC> phieuCans)
        {
            try
            {
                var query =
                    "UPDATE [dbo].[PhieuCanXepKhuonKHC] SET  [Gio] = @Gio, [MaUserCan] = @MaUserCan,  [GhiChu] =@GhiChu, [MaLo] = @MaLo, [MaLoaiCa] =@MaLoaiCa, [MaThanhPham] = @MaThanhPham, [MaSize] = @MaSize, [MaKhachHang] =@MaKhachHang, [MaCoiTam] = @MaCoiTam, [TrongLuong] = @TrongLuong, [DaXacNhan] = @DaXacNhan ,[Block] = @Block, [MaNhanVien] = @MaNhanVien WHERE [STT] = @STT and [Ngay] =@Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.ExecuteAsync(query, phieuCans).Result;
                    return rows;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetPhieuCanTongHopThanhPhams<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.MaThanhPham,
tp.Ten as ThanhPhamName,
SUM(p.TrongLuong) as TrongLuong,
p.MaXuong
from 
PhieuCanXepKhuonKHC p,
MaThanhPhamXepKhuonBlock tp
where 
Ngay <=@ngay and
Ngay >= @fromDate 
and MaXuong= @xuongId 
and p.MaThanhPham = tp.Ma 
group by 
tp.Ten ,
p.MaThanhPham,
p.MaXuong
order by 
tp.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<T>(query, new { fromDate = fromDate.Date, ngay = dateTime.Date, xuongId = xuongId })
                        .Result
                        .ToList();
                    return item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan_XLPC<T>(DateTime dateTime, string xuongId)
        {
            var query = @"select
p.STT,
p.Ngay,
p.Gio,
p.MaUserCan,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.GhiChu,
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaCoiTam,
c.Ten as CoiTamName,
p.TrongLuong,
p.DaXacNhan,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
nv.DeptName0 as Nhom,
p.Block
from PhieuCanXepKhuonKHC p
left join MaLoaiCaXepKhuon lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamXepKhuonKHC tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuonKHC s on p.MaSize = s.Ma
left join MaKhachHangXepKhuon kh on p.MaKhachHang = kh.Ma
left join MaCoiXepKhuon c on p.MaCoiTam = c.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien =nv.MaNhanVien
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
    }
}
