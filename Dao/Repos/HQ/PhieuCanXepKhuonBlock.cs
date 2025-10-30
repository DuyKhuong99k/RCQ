using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanXepKhuonBlock
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanXepKhuonBlock";
        private readonly string qrDelete = "Delete [dbo].[PhieuCanXepKhuonBlock] WHERE [STT] = @STT and [Ngay] =@Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";

        private readonly string qrInsert = @"
INSERT INTO[dbo].[PhieuCanXepKhuonBlock] ([STT] ,[Ngay] ,[MaXuong] ,[MaMayCan] ,[Gio] ,[MaUserCan] ,[GhiChu] ,[MaLo] ,[MaThanhPham] ,[MaSize] ,[MaChatLuong] ,[MaNet] ,[MaChieuXa] ,[MaKhachHang] ,[MaMau] ,[MaCongDoan] ,[MaNhanVien] ,[TrongLuong]) VALUES (@STT,@Ngay,@MaXuong,@MaMayCan,@Gio,@MaUserCan,@GhiChu,@MaLo,@MaThanhPham, @MaSize,@MaChatLuong,@MaNet,@MaChieuXa,@MaKhachHang,@MaMau,@MaCongDoan,@MaNhanVien,@TrongLuong)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanXepKhuonBlock] SET  [Gio] = @Gio, [MaUserCan] = @MaUserCan, [GhiChu] = @GhiChu, [MaLo] = @MaLo,  [MaThanhPham] = @MaThanhPham, [MaSize] = @MaSize, [MaChatLuong] =@MaChatLuong, [MaNet] = @MaNet, [MaChieuXa] = @MaChieuXa, [MaKhachHang] = @MaKhachHang, [MaMau] = @MaMau, [MaCongDoan] = @MaCongDoan, [MaNhanVien] = @MaNhanVien, [TrongLuong] = @TrongLuong WHERE [STT] = @STT and [Ngay] =@Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";

        private readonly string qrGetAll = "Select * from PhieuCanXepKhuonBlock";

        public PhieuCanXepKhuonBlock(string? _connectionString = null)
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
        public List<PhieuCanXepKhuonBlock> Gets()
        {
            try
            {
                var query = "Select * from PhieuCanXepKhuonBlock";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<PhieuCanXepKhuonBlock>(query).Result.ToList();
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
                    $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanXepKhuonBlock p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaThanhPham in {listOfIdsJoined} and p.Gio >= @fromTime and p.Gio < @toTime";
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
            IEnumerable<Tuple<string, string>> thanhPhamCongDoanIds)
        {
            try
            {
                var query =
                    $@"Select p.MaThanhPham as Item1,p.MaCongDoan as Item2,Sum(TrongLuong) as Item3 from PhieuCanXepKhuonBlock p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.Gio >= @fromTime and p.Gio < @toTime group by p.MaThanhPham,p.MaCongDoan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<Tuple<string, string, decimal>>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                fromTime = fromTime,
                                toTime = toTime,
                                xuongId = xuongId
                            })
                        .ToList();
                    if (items.Any())
                    {
                        var num = (from i in items
                                   from td in thanhPhamCongDoanIds
                                   where i.Item1 == td.Item1 && i.Item2 == td.Item2
                                   select i.Item3).DefaultIfEmpty(0)
                            .Sum();
                        return num;
                    }
                    else
                    {
                        return 0;
                    }
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
                    $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanXepKhuonBlock p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaThanhPham in {listOfIdsJoined} and p.Gio >= @fromTime and p.Gio < @toTime and MaNhanVien = @nhanVienId";
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

        public decimal GetSanLuong(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<Tuple<string, string>> thanhPhamCongDoanIds,
            string nhanVienId)
        {
            try
            {
                var query =
                    $@"Select p.MaThanhPham as Item1,p.MaCongDoan as Item2,Sum(TrongLuong) as Item3 from PhieuCanXepKhuonBlock p where p.Ngay=@ngay and p.MaXuong = @xuongId and MaNhanVien = @nhanVienId and p.Gio >= @fromTime and p.Gio < @toTime group by p.MaThanhPham,p.MaCongDoan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<Tuple<string, string, decimal>>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                fromTime = fromTime,
                                toTime = toTime,
                                xuongId = xuongId,
                                nhanVienId = nhanVienId
                            })
                        .ToList();
                    if (items.Any())
                    {
                        var num = (from i in items
                                   from td in thanhPhamCongDoanIds
                                   where i.Item1 == td.Item1 && i.Item2 == td.Item2
                                   select i.Item3).DefaultIfEmpty(0)
                            .Sum();
                        return num;
                    }
                    else
                    {
                        return 0;
                    }
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
                    $@"Select p.Ngay, CaId as CaLamViec,p.MaNhanVien,cd.BravoId as MaSanPham,cd.Ten as TenSanPham, Sum(p.TrongLuong) as TrongLuong,0 as _Status from 
(Select p.*, (case when p.Gio < @mocThoiGian then 'CT04' else 'CT05' end) as CaId   from PhieuCanXepKhuonBlock p where p.Ngay=@ngay and p.MaXuong = @xuongId ) p,MaCongDoanXepKhuon cd
where p.MaCongDoan = cd.Ma and cd.BravoId is not null and p.MaNhanVien not in {listOfIdsJoined}
group by p.CaId,p.Ngay,p.MaNhanVien,cd.BravoId,cd.Ten";
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
                    $@"Select p.Ngay, CaId as CaLamViec,p.MaNhanVien,cd.BravoId as MaSanPham,cd.Ten as TenSanPham, Sum(p.TrongLuong) as TrongLuong,0 as _Status from 
(Select p.*, (case when p.Gio < @mocThoiGian then 'CT04' else 'CT05' end) as CaId   from PhieuCanXepKhuonBlock p where p.Ngay=@ngay and p.MaXuong = @xuongId ) p,MaCongDoanXepKhuon cd
where p.MaCongDoan = cd.Ma and cd.BravoId is not null and p.MaNhanVien in {listOfIdsJoined}
group by p.CaId,p.Ngay,p.MaNhanVien,cd.BravoId,cd.Ten";
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

        public List<PhieuCanXepKhuonBlock> Gets(DateTime dateTime)
        {
            try
            {
                var query = "Select * from PhieuCanXepKhuonBlock where Ngay = @ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<PhieuCanXepKhuonBlock>(query, new { ngay = dateTime.Date })
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

        public List<PhieuCanXepKhuonBlock> Gets(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = "Select * from PhieuCanXepKhuonBlock where Ngay = @ngay and MaXuong = @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<PhieuCanXepKhuonBlock>(
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
        public List<T> GetPhieuCanBlockXepKhuonsByDateToDate<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
p.Ngay,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.Gio,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaNet,
n.Ten as NetName,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaMau,
m.Ten as MauName,
p.MaCongDoan,
cd.Ten as CongDoanName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as Nhom,
nv.Name as NhanVienName,
p.TrongLuong
from PhieuCanXepKhuonBlock p
left join XiNghiep x on p.MaXuong = x.Ma
left join MaThanhPhamXepKhuonBlock tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuonBlock s on p.MaSize = s.Ma
left join MaChatLuongXepKhuonBlock cl on p.MaChatLuong = cl.Ma
left join MaNetXepKhuon n on p.MaNet = n.Ma
left join MaChieuXaXepKhuon cx on p.MaChatLuong = cx.Ma
left join MaKhachHangXepKhuon kh on p.MaKhachHang = kh.Ma
left join MaMauXepKhuonBlock m on p.MaMau = m.Ma
left join MaCongDoanXepKhuon cd on p.MaCongDoan = cd.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
where 
p.Ngay <= @toDate 
and p.Ngay >= @fromDate 
and p.MaXuong = @xuongId";
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
        public List<T> GetPhieuCanBlockXepKhuonsByMaNhanVien<T>(DateTime fromDate, DateTime toDate,string maNhanVien, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
p.Ngay,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.Gio,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaNet,
n.Ten as NetName,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaMau,
m.Ten as MauName,
p.MaCongDoan,
cd.Ten as CongDoanName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as Nhom,
nv.Name as NhanVienName,
p.TrongLuong
from PhieuCanXepKhuonBlock p
left join XiNghiep x on p.MaXuong = x.Ma
left join MaThanhPhamXepKhuonBlock tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuonBlock s on p.MaSize = s.Ma
left join MaChatLuongXepKhuonBlock cl on p.MaChatLuong = cl.Ma
left join MaNetXepKhuon n on p.MaNet = n.Ma
left join MaChieuXaXepKhuon cx on p.MaChatLuong = cx.Ma
left join MaKhachHangXepKhuon kh on p.MaKhachHang = kh.Ma
left join MaMauXepKhuonBlock m on p.MaMau = m.Ma
left join MaCongDoanXepKhuon cd on p.MaCongDoan = cd.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
where 
p.Ngay <= @toDate 
and p.Ngay >= @fromDate 
and p.MaXuong = @xuongId
and p.MaNhanVien = @maNhanVien";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId , maNhanVien})
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
        public List<T> GetPhieuCanBlockXepKhuonsByMaHoSo<T>(DateTime fromDate, DateTime toDate,string maHoSo, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
p.Ngay,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.Gio,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaNet,
n.Ten as NetName,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaMau,
m.Ten as MauName,
p.MaCongDoan,
cd.Ten as CongDoanName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as Nhom,
nv.Name as NhanVienName,
p.TrongLuong
from PhieuCanXepKhuonBlock p
left join XiNghiep x on p.MaXuong = x.Ma
left join MaThanhPhamXepKhuonBlock tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuonBlock s on p.MaSize = s.Ma
left join MaChatLuongXepKhuonBlock cl on p.MaChatLuong = cl.Ma
left join MaNetXepKhuon n on p.MaNet = n.Ma
left join MaChieuXaXepKhuon cx on p.MaChatLuong = cx.Ma
left join MaKhachHangXepKhuon kh on p.MaKhachHang = kh.Ma
left join MaMauXepKhuonBlock m on p.MaMau = m.Ma
left join MaCongDoanXepKhuon cd on p.MaCongDoan = cd.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
where 
p.Ngay <= @toDate 
and p.Ngay >= @fromDate 
and p.MaXuong = @xuongId
and nv.MaHoSo = @maHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId , maHoSo})
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
        public List<T> GetPhieuCanBlockXepKhuonsByMaThe<T>(DateTime fromDate, DateTime toDate,string maThe, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
p.Ngay,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.Gio,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaNet,
n.Ten as NetName,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaMau,
m.Ten as MauName,
p.MaCongDoan,
cd.Ten as CongDoanName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as Nhom,
nv.Name as NhanVienName,
p.TrongLuong
from PhieuCanXepKhuonBlock p
left join XiNghiep x on p.MaXuong = x.Ma
left join MaThanhPhamXepKhuonBlock tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuonBlock s on p.MaSize = s.Ma
left join MaChatLuongXepKhuonBlock cl on p.MaChatLuong = cl.Ma
left join MaNetXepKhuon n on p.MaNet = n.Ma
left join MaChieuXaXepKhuon cx on p.MaChatLuong = cx.Ma
left join MaKhachHangXepKhuon kh on p.MaKhachHang = kh.Ma
left join MaMauXepKhuonBlock m on p.MaMau = m.Ma
left join MaCongDoanXepKhuon cd on p.MaCongDoan = cd.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join TheTu t on p.MaNhanVien = t.MaNhanVien
where 
p.Ngay <= @toDate 
and p.Ngay >= @fromDate 
and p.MaXuong = @xuongId
and t.MaTheTu = @maThe";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId , maThe})
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
        public List<T> GetTongHopPhieuCanBlockXepKhuonsByDateToDate<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.MaLo,
p.MaNhanVien,
n.[Name] as NhanVienName,
n.MaHoSo,
tp.Ten as ThanhPhamName, 
s.Ten as SizeName,
cl.Ten as ChatLuongName,
net.Ten as NetName,
cx.Ten as ChieuXaName,
kh.Ten as KhachHangName,
ma.Ten as MauName,
cd.Ten as CongDoanName,
SUM(p.TrongLuong) as TrongLuong,
COUNT(*) as SoRo 
from PhieuCanXepKhuonBlock p,
NhanVienDaiThanh n,MaThanhPhamXepKhuonBlock tp,
MaSizeXepKhuonBlock s,
MaChatLuongXepKhuonBlock cl,
MaNetXepKhuon net,
MaChieuXaXepKhuon cx,
MaKhachHangXepKhuon kh,
MaMauXepKhuonBlock ma,
MaCongDoanXepKhuon cd 
where p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId 
and p.MaNhanVien = n.MaNhanVien
and p.MaThanhPham = tp.Ma 
and p.MaSize = s.Ma 
and p.MaChatLuong = cl.Ma 
and p.MaNet = net.Ma 
and p.MaChieuXa = cx.Ma 
and p.MaKhachHang = kh.Ma 
and p.MaMau = ma.Ma 
and p.MaCongDoan = cd.Ma 
group by 
p.MaLo,
p.MaNhanVien,
n.[Name],
n.MaHoSo,
tp.Ten, 
s.Ten,
cl.Ten,
net.Ten, 
cx.Ten,
kh.Ten,
ma.Ten,
cd.Ten 
order by MaHoSo";
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
        public List<T> GetTongHopThanhPhamByMaNhanVien<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.MaLo,
p.MaNhanVien,
n.[Name] as NhanVienName,
n.MaHoSo,
tp.Ten as ThanhPhamName, 
s.Ten as SizeName,
cl.Ten as ChatLuongName,
net.Ten as NetName,
cx.Ten as ChieuXaName,
kh.Ten as KhachHangName,
ma.Ten as MauName,
cd.Ten as CongDoanName,
SUM(p.TrongLuong) as TrongLuong,
COUNT(*) as SoRo 
from PhieuCanXepKhuonBlock p,
NhanVienDaiThanh n,MaThanhPhamXepKhuonBlock tp,
MaSizeXepKhuonBlock s,
MaChatLuongXepKhuonBlock cl,
MaNetXepKhuon net,
MaChieuXaXepKhuon cx,
MaKhachHangXepKhuon kh,
MaMauXepKhuonBlock ma,
MaCongDoanXepKhuon cd 
where p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId 
and p.MaNhanVien = n.MaNhanVien
and p.MaThanhPham = tp.Ma 
and p.MaSize = s.Ma 
and p.MaChatLuong = cl.Ma 
and p.MaNet = net.Ma 
and p.MaChieuXa = cx.Ma 
and p.MaKhachHang = kh.Ma 
and p.MaMau = ma.Ma 
and p.MaCongDoan = cd.Ma 
and p.MaNhanVien = @maNhanVien
group by 
p.MaLo,
p.MaNhanVien,
n.[Name],
n.MaHoSo,
tp.Ten, 
s.Ten,
cl.Ten,
net.Ten, 
cx.Ten,
kh.Ten,
ma.Ten,
cd.Ten 
order by MaHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, maNhanVien, xuongId })
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
        public List<T> GetTongHopThanhPhamByMaHoSo<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.MaLo,
p.MaNhanVien,
n.[Name] as NhanVienName,
n.MaHoSo,
tp.Ten as ThanhPhamName, 
s.Ten as SizeName,
cl.Ten as ChatLuongName,
net.Ten as NetName,
cx.Ten as ChieuXaName,
kh.Ten as KhachHangName,
ma.Ten as MauName,
cd.Ten as CongDoanName,
SUM(p.TrongLuong) as TrongLuong,
COUNT(*) as SoRo 
from PhieuCanXepKhuonBlock p,
NhanVienDaiThanh n,MaThanhPhamXepKhuonBlock tp,
MaSizeXepKhuonBlock s,
MaChatLuongXepKhuonBlock cl,
MaNetXepKhuon net,
MaChieuXaXepKhuon cx,
MaKhachHangXepKhuon kh,
MaMauXepKhuonBlock ma,
MaCongDoanXepKhuon cd 
where p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId 
and p.MaNhanVien = n.MaNhanVien
and p.MaThanhPham = tp.Ma 
and p.MaSize = s.Ma 
and p.MaChatLuong = cl.Ma 
and p.MaNet = net.Ma 
and p.MaChieuXa = cx.Ma 
and p.MaKhachHang = kh.Ma 
and p.MaMau = ma.Ma 
and p.MaCongDoan = cd.Ma 
and n.MaHoSo = @maHoSo
group by 
p.MaLo,
p.MaNhanVien,
n.[Name],
n.MaHoSo,
tp.Ten, 
s.Ten,
cl.Ten,
net.Ten, 
cx.Ten,
kh.Ten,
ma.Ten,
cd.Ten 
order by MaHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new
                            {
                                fromDate = fromDate.Date,
                                toDate = toDate.Date,
                                maHoSo,
                                xuongId
                            })
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
        public List<T> GetTongHopThanhPhamByMaThe<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.MaLo,
p.MaNhanVien,
n.[Name] as NhanVienName,
n.MaHoSo,
tp.Ten as ThanhPhamName, 
s.Ten as SizeName,
cl.Ten as ChatLuongName,
net.Ten as NetName,
cx.Ten as ChieuXaName,
kh.Ten as KhachHangName,
ma.Ten as MauName,
cd.Ten as CongDoanName,
SUM(p.TrongLuong) as TrongLuong,
COUNT(*) as SoRo 
from PhieuCanXepKhuonBlock p,
NhanVienDaiThanh n,MaThanhPhamXepKhuonBlock tp,
MaSizeXepKhuonBlock s,
MaChatLuongXepKhuonBlock cl,
MaNetXepKhuon net,
MaChieuXaXepKhuon cx,
MaKhachHangXepKhuon kh,
MaMauXepKhuonBlock ma,
MaCongDoanXepKhuon cd ,TheTu t
where p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId 
and p.MaNhanVien = n.MaNhanVien
and p.MaThanhPham = tp.Ma 
and p.MaSize = s.Ma 
and p.MaChatLuong = cl.Ma 
and p.MaNet = net.Ma 
and p.MaChieuXa = cx.Ma 
and p.MaKhachHang = kh.Ma 
and p.MaMau = ma.Ma 
and p.MaCongDoan = cd.Ma 
and p.MaNhanVien = t.MaNhanVien
and t.MaTheTu = @maThe
group by 
p.MaLo,
p.MaNhanVien,
n.[Name],
n.MaHoSo,
tp.Ten, 
s.Ten,
cl.Ten,
net.Ten, 
cx.Ten,
kh.Ten,
ma.Ten,
cd.Ten 
order by MaHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new
                            {
                                fromDate = fromDate.Date,
                                toDate = toDate.Date,
                                maThe,
                                xuongId
                            })
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
                    "Select p.MaLo,p.MaNhanVien,n.[Name] as TenNhanVien,n.MaHoSo,tp.Ten as ThanhPhamName, s.Ten as SizeName,cl.Ten as ChatLuongName,net.Ten as NetName, cx.Ten as ChieuXaName,kh.Ten as KhachHangName,ma.Ten as MauName,cd.Ten as CongDoanName, SUM(p.TrongLuong) as TrongLuong,COUNT(*) as SoRo from PhieuCanXepKhuonBlock p, NhanVienDaiThanh n,MaThanhPhamXepKhuonBlock tp,MaSizeXepKhuonBlock s,MaChatLuongXepKhuonBlock cl,MaNetXepKhuon net,MaChieuXaXepKhuon cx,MaKhachHangXepKhuon kh,MaMauXepKhuonBlock ma,MaCongDoanXepKhuon cd where p.Ngay= @ngay and p.MaXuong = @xuongId and p.MaNhanVien = n.MaNhanVien and p.MaThanhPham = tp.Ma and p.MaSize = s.Ma and p.MaChatLuong = cl.Ma and p.MaNet = net.Ma and p.MaChieuXa = cx.Ma and p.MaKhachHang = kh.Ma and p.MaMau = ma.Ma and p.MaCongDoan = cd.Ma group by p.MaLo,p.MaNhanVien,n.[Name],n.MaHoSo,tp.Ten, s.Ten,cl.Ten,net.Ten, cx.Ten,kh.Ten,ma.Ten,cd.Ten order by MaHoSo";
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

        public List<PhieuCanXepKhuonBlock> Gets(DateTime dateTime, string xuongId, string mayCanId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanXepKhuonBlock where Ngay = @ngay and MaXuong = @xuongId and MaMayCan=@mayCanId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<PhieuCanXepKhuonBlock>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId,
                                mayCanId = mayCanId
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

        public int Insert(PhieuCanXepKhuonBlock phieuCan)
        {
            try
            {
                var query =
                    " INSERT INTO[dbo].[PhieuCanXepKhuonBlock] ([STT] ,[Ngay] ,[MaXuong] ,[MaMayCan] ,[Gio] ,[MaUserCan] ,[GhiChu] ,[MaLo] ,[MaThanhPham] ,[MaSize] ,[MaChatLuong] ,[MaNet] ,[MaChieuXa] ,[MaKhachHang] ,[MaMau] ,[MaCongDoan] ,[MaNhanVien] ,[TrongLuong]) VALUES (@STT,@Ngay,@MaXuong,@MaMayCan,@Gio,@MaUserCan,@GhiChu,@MaLo,@MaThanhPham, @MaSize,@MaChatLuong,@MaNet,@MaChieuXa,@MaKhachHang,@MaMau,@MaCongDoan,@MaNhanVien,@TrongLuong)";
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

        public int Insert<T>(List<T> phieuCan)
        {
            try
            {
                var query =
                    " INSERT INTO[dbo].[PhieuCanXepKhuonBlock] ([STT] ,[Ngay] ,[MaXuong] ,[MaMayCan] ,[Gio] ,[MaUserCan] ,[GhiChu] ,[MaLo] ,[MaThanhPham] ,[MaSize] ,[MaChatLuong] ,[MaNet] ,[MaChieuXa] ,[MaKhachHang] ,[MaMau] ,[MaCongDoan] ,[MaNhanVien] ,[TrongLuong]) VALUES (@STT,@Ngay,@MaXuong,@MaMayCan,@Gio,@MaUserCan,@GhiChu,@MaLo,@MaThanhPham, @MaSize,@MaChatLuong,@MaNet,@MaChieuXa,@MaKhachHang,@MaMau,@MaCongDoan,@MaNhanVien,@TrongLuong)";
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

        public int Update(PhieuCanXepKhuonBlock phieuCan)
        {
            try
            {
                var query =
                    "UPDATE [dbo].[PhieuCanXepKhuonBlock] SET  [Gio] = @Gio, [MaUserCan] = @MaUserCan, [GhiChu] = @GhiChu, [MaLo] = @MaLo,  [MaThanhPham] = @MaThanhPham, [MaSize] = @MaSize, [MaChatLuong] =@MaChatLuong, [MaNet] = @MaNet, [MaChieuXa] = @MaChieuXa, [MaKhachHang] = @MaKhachHang, [MaMau] = @MaMau, [MaCongDoan] = @MaCongDoan, [MaNhanVien] = @MaNhanVien, [TrongLuong] = @TrongLuong WHERE [STT] = @STT and [Ngay] =@Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";
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

        public int Update<T>(List<T> phieuCan)
        {
            try
            {
                var query =
                    "UPDATE [dbo].[PhieuCanXepKhuonBlock] SET  [Gio] = @Gio, [MaUserCan] = @MaUserCan, [GhiChu] = @GhiChu, [MaLo] = @MaLo,  [MaThanhPham] = @MaThanhPham, [MaSize] = @MaSize, [MaChatLuong] =@MaChatLuong, [MaNet] = @MaNet, [MaChieuXa] = @MaChieuXa, [MaKhachHang] = @MaKhachHang, [MaMau] = @MaMau, [MaCongDoan] = @MaCongDoan, [MaNhanVien] = @MaNhanVien, [TrongLuong] = @TrongLuong WHERE [STT] = @STT and [Ngay] =@Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";
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

        public int Delete(PhieuCanXepKhuonBlock phieuCan)
        {
            try
            {
                var query =
                    "Delete [dbo].[PhieuCanXepKhuonBlock] WHERE [STT] = @STT and [Ngay] =@Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";
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

        public int Delete<T>(List<T> phieuCan)
        {
            try
            {
                var query =
                    "Delete [dbo].[PhieuCanXepKhuonBlock] WHERE [STT] = @STT and [Ngay] =@Ngay and [MaXuong] = @MaXuong and [MaMayCan] = @MaMayCan";
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
PhieuCanXepKhuonBlock p,
MaThanhPhamXepKhuonBlock tp
where 
Ngay <=@ngay and
Ngay >= @fromDate 
and MaXuong= @xuongId 
and p.MaThanhPham = tp.Ma 
group by 
tp.Ten ,
p.MaThanhPham,p.MaXuong
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
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
P.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaNet,
n.Ten as NetName,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaMau,
M.Ten as MauName,
p.MaCongDoan,
cd.Ten as CongDoanNam,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
nv.DeptName0 as Nhom,
p.TrongLuong
from PhieuCanXepKhuonBlock p
left join MaThanhPhamXepKhuonBlock tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuonBlock s on p.MaSize = s.Ma
left join MaChatLuongXepKhuonBlock cl on p.MaKhachHang = cl.Ma
left join MaNetXepKhuon n on p.MaNet = n.Ma
left join MaChieuXaXepKhuon cx on p.MaChieuXa = cx.Ma
left join MaKhachHangXepKhuon kh on p.MaKhachHang = kh.Ma
left join MaMauXepKhuonBlock m on p.MaMau = m.Ma
left join MaCongDoanXepKhuon cd on p.MaCongDoan = cd.Ma
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
