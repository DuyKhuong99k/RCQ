using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanTaiChe
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanTaiChe";
        private readonly string qrDelete = @"DELETE FROM[dbo].[PhieuCanTaiChe]
      WHERE [STT] = @STT and [Ngay] =@Ngay and [MaMayCan] = @MaMayCan and [MaXuong] =@MaXuong";
        private readonly string qrInsert = @"
INSERT INTO [dbo].[PhieuCanTaiChe] ([STT] ,[Ngay] ,[MaMayCan] ,[MaXuong] ,[Gio] ,[Id] ,[MaUserCan] ,[GhiChu] ,[MaLo] ,[MaSize] ,[MaChatLuong] ,[MaLoaiCa] ,[MaMau] ,[MaThanhPham] ,[TrongLuong] ,[MaNhanVien],[MaCongViec]) VALUES (@STT,@Ngay,@MaMayCan,@MaXuong,@Gio,@Id,@MaUserCan,@GhiChu,@MaLo,@MaSize,@MaChatLuong,@MaLoaiCa,@MaMau, @MaThanhPham,@TrongLuong,@MaNhanVien,@MaCongViec)";

        private readonly string qrUpdate = @"
UPDATE [dbo].[PhieuCanTaiChe] SET  [MaCongViec] =@MaCongViec, [Gio] = @Gio, [Id] = @Id, [MaUserCan] =@MaUserCan, [GhiChu] = @GhiChu, [MaLo] =@MaLo,  [MaSize] = @MaSize, [MaChatLuong] = @MaChatLuong, [MaLoaiCa] = @MaLoaiCa, [MaMau] = @MaMau, [MaThanhPham] =@MaThanhPham, [TrongLuong] = @TrongLuong, [MaNhanVien] = @MaNhanVien, WHERE [STT] = @STT and [Ngay] =@Ngay and [MaMayCan] = @MaMayCan and [MaXuong] =@MaXuong";

        private readonly string qrGetAll = "Select * from PhieuCanTaiChe";

        public PhieuCanTaiChe(string? _connectionString = null)
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
        public List<PhieuCanTaiChe> Gets()
        {
            try
            {
                var query = "Select * from PhieuCanTaiChe";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<PhieuCanTaiChe>(query).Result.ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanTaiCheXepKhuonsByDateToDate<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
p.Ngay,
p.MaMayCan,
p.MaXuong,
p.Gio,
p.Id,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaMau,
m.Ten as MauName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.TrongLuong,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
cv.Ten as CongViecName
from PhieuCanTaiChe p
left join MaSizeTaiChe s on p.MaSize = s.Ma
left join MaChatLuongTaiChe cl on p.MaChatLuong = cl.Ma
left join MaLoaiCaTaiChe lc on p.MaLoaiCa = lc.Ma
left join MaMauTaiChe m on p.MaMau = m.Ma
left join MaThanhPhamTaiChe tp on p.MaThanhPham = tp.Ma 
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join MaCongViecTaiChe cv on p.MaCongViec = cv.Ma
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
        public List<T> GetPhieuCanTaiCheXepKhuonsByMaNhanVien<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
p.Ngay,
p.MaMayCan,
p.MaXuong,
p.Gio,
p.Id,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaMau,
m.Ten as MauName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.TrongLuong,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
cv.Ten as CongViecName
from PhieuCanTaiChe p
left join MaSizeTaiChe s on p.MaSize = s.Ma
left join MaChatLuongTaiChe cl on p.MaChatLuong = cl.Ma
left join MaLoaiCaTaiChe lc on p.MaLoaiCa = lc.Ma
left join MaMauTaiChe m on p.MaMau = m.Ma
left join MaThanhPhamTaiChe tp on p.MaThanhPham = tp.Ma 
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join MaCongViecTaiChe cv on p.MaCongViec = cv.Ma
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
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, maNhanVien })
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

        public List<T> GetPhieuCanTaiCheXepKhuonsByMaHoSo<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
p.Ngay,
p.MaMayCan,
p.MaXuong,
p.Gio,
p.Id,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaMau,
m.Ten as MauName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.TrongLuong,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
cv.Ten as CongViecName
from PhieuCanTaiChe p
left join MaSizeTaiChe s on p.MaSize = s.Ma
left join MaChatLuongTaiChe cl on p.MaChatLuong = cl.Ma
left join MaLoaiCaTaiChe lc on p.MaLoaiCa = lc.Ma
left join MaMauTaiChe m on p.MaMau = m.Ma
left join MaThanhPhamTaiChe tp on p.MaThanhPham = tp.Ma 
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join MaCongViecTaiChe cv on p.MaCongViec = cv.Ma
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
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, maHoSo })
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
        public List<T> GetPhieuCanTaiCheXepKhuonsByMaThe<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
p.Ngay,
p.MaMayCan,
p.MaXuong,
p.Gio,
p.Id,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaMau,
m.Ten as MauName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.TrongLuong,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
cv.Ten as CongViecName
from PhieuCanTaiChe p
left join MaSizeTaiChe s on p.MaSize = s.Ma
left join MaChatLuongTaiChe cl on p.MaChatLuong = cl.Ma
left join MaLoaiCaTaiChe lc on p.MaLoaiCa = lc.Ma
left join MaMauTaiChe m on p.MaMau = m.Ma
left join MaThanhPhamTaiChe tp on p.MaThanhPham = tp.Ma 
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join MaCongViecTaiChe cv on p.MaCongViec = cv.Ma
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
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, maThe })
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
        public List<T> GetPhieuCanTongHopTaiCheXepKhuonsByDateToDate<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select p.MaNhanVien,n.MaHoSo,n.[Name] as NhanVien,p.MaLo,cv.Ten as CongViecName,tp.Ten as ThanhPhamName,s.Ten as SizeName,cl.Ten as ChatLuongName,ma.Ten as MauName,Sum(p.TrongLuong) as TrongLuong, COUNT(*) as SoRo from PhieuCanTaiChe p,NhanVienDaiThanh n,MaSizeTaiChe s, MaChatLuongTaiChe cl,MaMauTaiChe ma,MaThanhPhamTaiChe tp,MaCongViecTaiChe cv where Ngay <=@toDate and p.Ngay >= @fromDate and MaXuong = @xuongId and p.MaNhanVien = n.MaNhanVien and p.MaMau = ma.Ma and p.MaChatLuong = cl.Ma and p.MaSize = s.Ma and p.MaThanhPham = tp.Ma and p.MaCongViec = cv.Ma group by p.MaNhanVien,n.MaHoSo,n.[Name] ,p.MaLo,p.MaLo,tp.Ten ,s.Ten ,cl.Ten ,ma.Ten,cv.Ten";
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
        public List<T> GetTongHopThanhPhamByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var query =
                    @"Select p.MaNhanVien,n.MaHoSo,n.[Name] as NhanVienName,p.MaLo,cv.Ten as CongViecName,tp.Ten as ThanhPhamName,s.Ten as SizeName,cl.Ten as ChatLuongName,ma.Ten as MauName,Sum(p.TrongLuong) as TrongLuong, COUNT(*) as SoRo from PhieuCanTaiChe p,NhanVienDaiThanh n,MaSizeTaiChe s, MaChatLuongTaiChe cl,MaMauTaiChe ma,MaThanhPhamTaiChe tp,MaCongViecTaiChe cv where Ngay <=@toDate and p.Ngay >= @fromDate and p.MaXuong = @xuongId and p.MaNhanVien = @maNhanVien and p.MaNhanVien = n.MaNhanVien and p.MaMau = ma.Ma and p.MaChatLuong = cl.Ma and p.MaSize = s.Ma and p.MaThanhPham = tp.Ma and p.MaCongViec = cv.Ma group by p.MaNhanVien,n.MaHoSo,n.[Name] ,p.MaLo,p.MaLo,tp.Ten ,s.Ten ,cl.Ten ,ma.Ten,cv.Ten";
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
        public List<T> GetTongHopThanhPhamByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var query =
                    @"Select p.MaNhanVien,n.MaHoSo,n.[Name] as NhanVien,p.MaLo,cv.Ten as CongViecName,tp.Ten as ThanhPhamName,s.Ten as SizeName,cl.Ten as ChatLuongName,ma.Ten as MauName,Sum(p.TrongLuong) as TrongLuong, COUNT(*) as SoRo from PhieuCanTaiChe p,NhanVienDaiThanh n,MaSizeTaiChe s, MaChatLuongTaiChe cl,MaMauTaiChe ma,MaThanhPhamTaiChe tp,MaCongViecTaiChe cv where Ngay <=@toDate and p.Ngay >= @fromDate and n.MaHoSo = @maHoSo and p.MaXuong = @xuongId and p.MaNhanVien = n.MaNhanVien and p.MaMau = ma.Ma and p.MaChatLuong = cl.Ma and p.MaSize = s.Ma and p.MaThanhPham = tp.Ma and p.MaCongViec = cv.Ma group by p.MaNhanVien,n.MaHoSo,n.[Name] ,p.MaLo,p.MaLo,tp.Ten ,s.Ten ,cl.Ten ,ma.Ten,cv.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, maHoSo, xuongId })
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
        public List<T> GetTongHopThanhPhamByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var query =
                    @"Select p.MaNhanVien,n.MaHoSo,n.[Name] as NhanVien,p.MaLo,cv.Ten as CongViecName,tp.Ten as ThanhPhamName,s.Ten as SizeName,cl.Ten as ChatLuongName,ma.Ten as MauName,Sum(p.TrongLuong) as TrongLuong, COUNT(*) as SoRo from PhieuCanTaiChe p,NhanVienDaiThanh n,MaSizeTaiChe s, MaChatLuongTaiChe cl,MaMauTaiChe ma,MaThanhPhamTaiChe tp,MaCongViecTaiChe cv ,TheTu t where Ngay <=@toDate and p.Ngay >= @fromDate and p.MaNhanVien = t.MaNhanVien and and t.MaTheTu = @maThe and p.MaXuong = @xuongId and p.MaNhanVien = n.MaNhanVien and p.MaMau = ma.Ma and p.MaChatLuong = cl.Ma and p.MaSize = s.Ma and p.MaThanhPham = tp.Ma and p.MaCongViec = cv.Ma group by p.MaNhanVien,n.MaHoSo,n.[Name] ,p.MaLo,p.MaLo,tp.Ten ,s.Ten ,cl.Ten ,ma.Ten,cv.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, maThe, xuongId })
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
        public List<PhieuCanTaiChe> Gets(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = "Select * from PhieuCanTaiChe where Ngay = @ngay and MaXuong = @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<PhieuCanTaiChe>(
                            query,
                            new { ngay = dateTime.Date, xuongId = xuongId })
                        .Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> Gets<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select p.MaNhanVien,n.MaHoSo,n.[Name] as TenNhanVien,p.MaLo,p.MaLo,cv.Ten as CongViecName,tp.Ten as ThanhPhamName,s.Ten as SizeName,cl.Ten as ChatLuongName,ma.Ten as MauName,Sum(p.TrongLuong) as TrongLuong, COUNT(*) as SoRo from PhieuCanTaiChe p,NhanVienDaiThanh n,MaSizeTaiChe s, MaChatLuongTaiChe cl,MaMauTaiChe ma,MaThanhPhamTaiChe tp,MaCongViecTaiChe cv where Ngay =@ngay and MaXuong = @xuongId and p.MaNhanVien = n.MaNhanVien and p.MaMau = ma.Ma and p.MaChatLuong = cl.Ma and p.MaSize = s.Ma and p.MaThanhPham = tp.Ma and p.MaCongViec = cv.Ma group by p.MaNhanVien,n.MaHoSo,n.[Name] ,p.MaLo,p.MaLo,tp.Ten ,s.Ten ,cl.Ten ,ma.Ten,cv.Ten";
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

        public List<PhieuCanTaiChe> Gets(DateTime dateTime, string xuongId, string mayCanId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanTaiChe where Ngay = @ngay and MaXuong = @xuongId and MaMayCan = @mayCanId order by STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<PhieuCanTaiChe>(
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
(Select p.*, (case when p.Gio < @mocThoiGian then 'CT04' else 'CT05' end) as CaId   from PhieuCanTaiChe p where p.Ngay=@ngay and p.MaXuong = @xuongId ) p,MaCongViecTaiChe tp
where p.MaCongViec = tp.Ma  and tp.BravoId is not null and p.MaNhanVien not in {listOfIdsJoined}
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
(Select p.*, (case when p.Gio < @mocThoiGian then 'CT04' else 'CT05' end) as CaId   from PhieuCanTaiChe p where p.Ngay=@ngay and p.MaXuong = @xuongId ) p,MaCongViecTaiChe tp
where p.MaCongViec = tp.Ma  and tp.BravoId is not null and p.MaNhanVien in {listOfIdsJoined}
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
                    $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanTaiChe p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaThanhPham in {listOfIdsJoined} and p.Gio >= @fromTime and p.Gio < @toTime and MaNhanVien = @nhanVienId";
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

        public decimal GetSanLuongCongViec(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> congViecIds,
            string nhanVienId)
        {
            try
            {
                var listOfIdsJoined = $@"('{string.Join("','", congViecIds.ToArray())}')";
                var query =
                    $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanTaiChe p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaCongViec in {listOfIdsJoined} and p.Gio >= @fromTime and p.Gio < @toTime and MaNhanVien = @nhanVienId";
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
            IEnumerable<string> thanhPhamIds)
        {
            try
            {
                var listOfIdsJoined = $@"('{string.Join("','", thanhPhamIds.ToArray())}')";
                var query =
                    $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanTaiChe p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaThanhPham in {listOfIdsJoined} and p.Gio >= @fromTime and p.Gio < @toTime ";
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

        public decimal GetSanLuongCongViec(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> congViecIds)
        {
            try
            {
                var listOfIdsJoined = $@"('{string.Join("','", congViecIds.ToArray())}')";
                var query =
                    $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanTaiChe p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaCongViec in {listOfIdsJoined} and p.Gio >= @fromTime and p.Gio < @toTime ";
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

        public int Insert(PhieuCanTaiChe phieuCan)
        {
            try
            {
                var query =
                    "INSERT INTO [dbo].[PhieuCanTaiChe] ([STT] ,[Ngay] ,[MaMayCan] ,[MaXuong] ,[Gio] ,[Id] ,[MaUserCan] ,[GhiChu] ,[MaLo] ,[MaSize] ,[MaChatLuong] ,[MaLoaiCa] ,[MaMau] ,[MaThanhPham] ,[TrongLuong] ,[MaNhanVien],[MaCongViec]) VALUES (@STT,@Ngay,@MaMayCan,@MaXuong,@Gio,@Id,@MaUserCan,@GhiChu,@MaLo,@MaSize,@MaChatLuong,@MaLoaiCa,@MaMau, @MaThanhPham,@TrongLuong,@MaNhanVien,@MaCongViec)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var row = connection.Execute(query, phieuCan);
                    return row;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Update(PhieuCanTaiChe phieuCan)
        {
            try
            {
                var query =
                    "UPDATE [dbo].[PhieuCanTaiChe] SET  [MaCongViec] =@MaCongViec, [Gio] = @Gio, [Id] = @Id, [MaUserCan] =@MaUserCan, [GhiChu] = @GhiChu, [MaLo] =@MaLo,  [MaSize] = @MaSize, [MaChatLuong] = @MaChatLuong, [MaLoaiCa] = @MaLoaiCa, [MaMau] = @MaMau, [MaThanhPham] =@MaThanhPham, [TrongLuong] = @TrongLuong, [MaNhanVien] = @MaNhanVien, WHERE [STT] = @STT and [Ngay] =@Ngay and [MaMayCan] = @MaMayCan and [MaXuong] =@MaXuong";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var row = connection.Execute(query, phieuCan);
                    return row;
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
                var query = @"DELETE FROM[dbo].[PhieuCanTaiChe]
      WHERE [STT] = @STT and [Ngay] =@Ngay and [MaMayCan] = @MaMayCan and [MaXuong] =@MaXuong";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var row = connection.Execute(query, phieuCans);
                    return row;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Update<T>(List<T> phieuCans)
        {
            try
            {
                var query = @"UPDATE [dbo].[PhieuCanTaiChe]
   SET [Gio] = @Gio 
      ,[Id] = @Id 
      ,[MaUserCan] = @MaUserCan 
      ,[GhiChu] = @GhiChu 
      ,[MaLo] = @MaLo 
      ,[MaSize] = @MaSize 
      ,[MaChatLuong] = @MaChatLuong 
      ,[MaLoaiCa] = @MaLoaiCa 
      ,[MaMau] = @MaMau 
      ,[MaThanhPham] = @MaThanhPham 
      ,[TrongLuong] = @TrongLuong 
      ,[MaNhanVien] = @MaNhanVien 
      ,[MaCongViec] = @MaCongViec 
 WHERE [STT] = @STT 
      and [Ngay] = @Ngay 
      and [MaMayCan] =@MaMayCan 
      and [MaXuong] = @MaXuong ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var row = connection.Execute(query, phieuCans);
                    return row;
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
                var query = @"INSERT INTO [dbo].[PhieuCanTaiChe]
           ([STT]
           ,[Ngay]
           ,[MaMayCan]
           ,[MaXuong]
           ,[Gio]
           ,[Id]
           ,[MaUserCan]
           ,[GhiChu]
           ,[MaLo]
           ,[MaSize]
           ,[MaChatLuong]
           ,[MaLoaiCa]
           ,[MaMau]
           ,[MaThanhPham]
           ,[TrongLuong]
           ,[MaNhanVien]
           ,[MaCongViec])
     VALUES
           (@STT 
           ,@Ngay 
           ,@MaMayCan 
           ,@MaXuong 
           ,@Gio 
           ,@Id 
           ,@MaUserCan 
           ,@GhiChu 
           ,@MaLo 
           ,@MaSize 
           ,@MaChatLuong 
           ,@MaLoaiCa 
           ,@MaMau 
           ,@MaThanhPham 
           ,@TrongLuong 
           ,@MaNhanVien 
           ,@MaCongViec)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var row = connection.Execute(query, phieuCans);
                    return row;
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
PhieuCanTaiChe p,
MaThanhPhamTaiChe tp
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
p.MaMayCan,
p.MaXuong,
x.Ten as XuongName,
p.Gio,
p.MaUserCan,
p.GhiChu,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaMau,
m.Ten as MauName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.TrongLuong,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
nv.DeptName0 as Nhom,
p.MaCongViec,
cv.Ten as CongViecName
from PhieuCanTaiChe p
left join XiNghiep x on p.MaXuong = x.Ma
left join MaSizeTaiChe s on p.MaSize = s.Ma
left join MaChatLuongTaiChe cl on p.MaChatLuong = cl.Ma
left join MaLoaiCaTaiChe lc on p.MaLoaiCa = lc.Ma
left join MaMauTaiChe m on p.MaMau = m.Ma
left join MaThanhPhamTaiChe tp on p.MaThanhPham = tp.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join MaCongViecTaiChe cv on p.MaCongViec = cv.Ma
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
