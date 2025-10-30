using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanPhuXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanPhuXepKhuon";
        private readonly string qrDelete = @"Delete PhieuCanPhuXepKhuon Where [STT]=@STT and [Ngay]=@Ngay and [MaXuong]= @MaXuong and [MaMayCan]=@MaMayCan";

        private readonly string qrInsert = @"
Insert Into PhieuCanPhuXepKhuon ([STT],[Ngay],[Gio],[MaLo],[MaLoaiCa],[MaThanhPham],[MaSize],[MaMau],[MaKhuVuc],[MaNhanVien],[MaNhom],[MaUserCan],[MaXuong],[MaMayCan],[TrongLuong],[GhiChu]) Values (@STT,@Ngay,@Gio,@MaLo,@MaLoaiCa,@MaThanhPham,@MaSize,@MaMau,@MaKhuVuc,@MaNhanVien,@MaNhom,@MaUserCan,@MaXuong,@MaMayCan,@TrongLuong,@GhiChu)";

        private readonly string qrUpdate = @"Update PhieuCanPhuXepKhuon Set [Gio]=@Gio,[MaLo]=@MaLo,[MaLoaiCa]=@MaLoaiCa,[MaThanhPham]=@MaThanhPham,[MaSize]=@MaSize,[MaMau]=@MaMau,[MaKhuVuc]=@MaKhuVuc,[MaNhanVien]=@MaNhanVien,[MaNhom]=@MaNhom,[MaUserCan]=@MaUserCan,[TrongLuong]=@TrongLuong,[GhiChu]=@GhiChu Where [STT]=@STT and [Ngay]=@Ngay and [MaXuong]= @MaXuong and [MaMayCan]=@MaMayCan";

        private readonly string qrGetAll = "Select * from PhieuCanPhuXepKhuon";
        private readonly string qrGetsLastByNumAndMayCan = @"WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MaMayCan ORDER BY Ngay DESC, Gio DESC) AS RowNum
    FROM PhieuCanPhuXepKhuon where Ngay =@ngay
)

SELECT *
FROM RankedPhieu
WHERE RowNum <= @num";
        public PhieuCanPhuXepKhuon(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

        }
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
        public List<PhieuCanPhuXepKhuon> GetPhieuCanPhuXepKhuonsByDate(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanPhuXepKhuon Where Ngay=@ngay and MaXuong =@xuongId order by STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<PhieuCanPhuXepKhuon>(
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

        public List<T> GetPhieuCanPhuXepKhuonsByDateToDate<T>(DateTime fromDate, DateTime toDate, string xuongId)
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
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
p.MaKhuVuc,
kv.Ten as KhuVucName,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
p.MaNhom,
n.Ten as NhomXepKhuonName,
p.MaUserCan,
p.TrongLuong,
p.GhiChu
from PhieuCanPhuXepKhuon p
left join XiNghiep x on p.MaXuong = x.Ma
left join MaLoaiCaXepKhuon lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamXepKhuon tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuon s on p.MaSize = s.Ma
left join MaMauXepKhuon m on p.MaMau = m.Ma
left join MaKhuVucXepKhuon kv on p.MaKhuVuc = kv.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join MaNhomXepKhuon n on p.MaNhom = n.Ma
Where 
p.Ngay<=@toDate 
and p.Ngay >= @fromDate 
and p.MaXuong =@xuongId 
order by STT DESC";
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
        public List<T> GetPhieuCanPhuXepKhuonsByMaNhanVien<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
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
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
p.MaKhuVuc,
kv.Ten as KhuVucName,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
p.MaNhom,
n.Ten as NhomXepKhuonName,
p.MaUserCan,
p.TrongLuong,
p.GhiChu
from PhieuCanPhuXepKhuon p
left join XiNghiep x on p.MaXuong = x.Ma
left join MaLoaiCaXepKhuon lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamXepKhuon tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuon s on p.MaSize = s.Ma
left join MaMauXepKhuon m on p.MaMau = m.Ma
left join MaKhuVucXepKhuon kv on p.MaKhuVuc = kv.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join MaNhomXepKhuon n on p.MaNhom = n.Ma
Where 
p.Ngay<=@toDate 
and p.Ngay >= @fromDate 
and p.MaXuong =@xuongId 
and p.MaNhanVien = @maNhanVien
order by STT DESC";
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
        public List<T> GetPhieuCanPhuXepKhuonsByMaHoSo<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
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
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
p.MaKhuVuc,
kv.Ten as KhuVucName,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
p.MaNhom,
n.Ten as NhomXepKhuonName,
p.MaUserCan,
p.TrongLuong,
p.GhiChu
from PhieuCanPhuXepKhuon p
left join XiNghiep x on p.MaXuong = x.Ma
left join MaLoaiCaXepKhuon lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamXepKhuon tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuon s on p.MaSize = s.Ma
left join MaMauXepKhuon m on p.MaMau = m.Ma
left join MaKhuVucXepKhuon kv on p.MaKhuVuc = kv.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join MaNhomXepKhuon n on p.MaNhom = n.Ma
Where 
p.Ngay<=@toDate 
and p.Ngay >= @fromDate 
and p.MaXuong =@xuongId 
and nv.MaHoSo = @maHoSo
order by STT DESC";
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
        public List<T> GetPhieuCanPhuXepKhuonsByMaThe<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
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
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
p.MaKhuVuc,
kv.Ten as KhuVucName,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as Nhom,
nv.Name as NhanVienName,
p.MaNhom,
n.Ten as NhomXepKhuonName,
p.MaUserCan,
p.TrongLuong,
p.GhiChu
from PhieuCanPhuXepKhuon p
left join XiNghiep x on p.MaXuong = x.Ma
left join MaLoaiCaXepKhuon lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamXepKhuon tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuon s on p.MaSize = s.Ma
left join MaMauXepKhuon m on p.MaMau = m.Ma
left join MaKhuVucXepKhuon kv on p.MaKhuVuc = kv.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join MaNhomXepKhuon n on p.MaNhom = n.Ma
left join TheTu t on p.MaNhanVien = t.MaNhanVien
Where 
p.Ngay<=@toDate 
and p.Ngay >= @fromDate 
and p.MaXuong =@xuongId 
and t.MaTheTu = @maThe
order by STT DESC";
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

        public int ChuyenXuong<T>(List<T> items, string xuongId)
        {
            try
            {
                var query = $@"
UPDATE [dbo].[PhieuCanPhuXepKhuon]
   SET [MaXuong] = '{xuongId}',[GhiChu] = @GhiChu
 WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var rows = connection.Execute(query, items);
                    return rows;
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
                    $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanPhuXepKhuon p where Ngay=@ngay and MaXuong = @xuongId and MaThanhPham in {listOfIdsJoined} and Gio >= @fromTime and Gio < @toTime";
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
            IEnumerable<string> nhanVienIds)
        {
            try
            {
                var listOfIdsJoined = $@"('{string.Join("','", thanhPhamIds.ToArray())}')";
                var listOfNhanVienIdsJoined = $@"('{string.Join("','", nhanVienIds.ToArray())}')";
                var query =
                    $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanPhuXepKhuon p where Ngay=@ngay and MaXuong = @xuongId and MaThanhPham in {listOfIdsJoined} and Gio >= @fromTime and Gio < @toTime and MaNhanVien in {listOfNhanVienIdsJoined}";
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
                    $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanPhuXepKhuon p where Ngay=@ngay and MaXuong = @xuongId and MaThanhPham in {listOfIdsJoined} and Gio >= @fromTime and Gio < @toTime and MaNhanVien =@nhanVienId";
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
(Select p.*, (case when p.Gio < @mocThoiGian then 'CT04' else 'CT05' end) as CaId   from PhieuCanPhuXepKhuon p where p.Ngay=@ngay and p.MaXuong = @xuongId ) p,MaThanhPhamXepKhuon tp
where p.MaThanhPham = tp.Ma  and tp.BravoId is not null and p.MaNhanVien not in {listOfIdsJoined}
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
(Select p.*, (case when p.Gio < @mocThoiGian then 'CT04' else 'CT05' end) as CaId   from PhieuCanPhuXepKhuon p where p.Ngay=@ngay and p.MaXuong = @xuongId ) p,MaThanhPhamXepKhuon tp
where p.MaThanhPham = tp.Ma  and tp.BravoId is not null and p.MaNhanVien in {listOfIdsJoined}
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

        public List<PhieuCanPhuXepKhuon> GetPhieuCanPhuXepKhuonsByDateMayCan(
            DateTime dateTime,
            string xuongId,
            string mayCan)
        {
            try
            {
                var query =
                    "Select * from PhieuCanPhuXepKhuon Where Ngay=@ngay and MaMayCan=@mayCan and MaXuong =@xuongId order by STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection
                        .QueryAsync<PhieuCanPhuXepKhuon>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                mayCan = mayCan,
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

        public List<PhieuCanPhuXepKhuon> GetPhieuCanPhuXepKhuons()
        {
            try
            {
                var query = "Select * from PhieuCanPhuXepKhuon";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<PhieuCanPhuXepKhuon>(query).Result.ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<TEntity> GetPhieuCanTongHops<TEntity>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select p.MaNhanVien,n.MaHoSo,n.Name as TenNhanVien,p.MaLo,la.Ten As LoaiCaName,tp.Ten as ThanhPhamName,s.Ten As SizeName,ma.Ten As MauName, SUM(p.TrongLuong) as TrongLuong,Count(*) As SoRo from  PhieuCanPhuXepKhuon p, MaLoaiCaXepKhuon la,MaSizeXepKhuon s, MaThanhPhamXepKhuon tp, MaMauXepKhuon ma,NhanVienDaiThanh n where p.Ngay =@ngay and MaXuong=@xuongId and p.MaLoaiCa = la.Ma and p.MaSize = s.Ma and p.MaThanhPham = tp.Ma and p.MaMau = ma.Ma and p.MaNhanVien= n.MaNhanVien group by p.MaNhanVien,n.MaHoSo,n.Name ,p.MaLo,tp.Ten ,s.Ten ,ma.Ten,la.Ten order by n.MaHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { ngay = dateTime.Date, xuongId = xuongId })
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
        public List<TEntity> GetPhieuCanTongHopsByDateToDate<TEntity>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.MaNhanVien,
n.MaHoSo,
n.Name as NhanVienName,
p.MaLo,la.Ten As LoaiCaName,
tp.Ten as ThanhPhamName,
s.Ten As SizeName,
ma.Ten As MauName,
SUM(p.TrongLuong) as TrongLuong,
Count(*) As SoRo 
from  
PhieuCanPhuXepKhuon p,
MaLoaiCaXepKhuon la,
MaSizeXepKhuon s,
MaThanhPhamXepKhuon tp,
MaMauXepKhuon ma,
NhanVienDaiThanh n 
where 
p.Ngay <=@toDate 
and p.Ngay >=@fromDate
and MaXuong=@xuongId 
and p.MaLoaiCa = la.Ma 
and p.MaSize = s.Ma 
and p.MaThanhPham = tp.Ma 
and p.MaMau = ma.Ma 
and p.MaNhanVien= n.MaNhanVien 
group by 
p.MaNhanVien,
n.MaHoSo,
n.Name ,
p.MaLo,
tp.Ten ,
s.Ten ,
ma.Ten,
la.Ten 
order by 
n.MaHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId })
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
        public List<TEntity> GetTongHopThanhPhamByMaNhanViens<TEntity>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.MaNhanVien,
n.MaHoSo,
n.Name as NhanVienName,
p.MaLo,la.Ten As LoaiCaName,
tp.Ten as ThanhPhamName,
s.Ten As SizeName,
ma.Ten As MauName,
SUM(p.TrongLuong) as TrongLuong,
Count(*) As SoRo 
from  
PhieuCanPhuXepKhuon p,
MaLoaiCaXepKhuon la,
MaSizeXepKhuon s,
MaThanhPhamXepKhuon tp,
MaMauXepKhuon ma,
NhanVienDaiThanh n 
where 
p.Ngay <=@toDate 
and p.Ngay >=@fromDate
and MaXuong=@xuongId 
and p.MaLoaiCa = la.Ma 
and p.MaSize = s.Ma 
and p.MaThanhPham = tp.Ma 
and p.MaMau = ma.Ma 
and p.MaNhanVien= n.MaNhanVien 
and p.MaNhanVien = @maNhanVien
group by 
p.MaNhanVien,
n.MaHoSo,
n.Name ,
p.MaLo,
tp.Ten ,
s.Ten ,
ma.Ten,
la.Ten 
order by 
n.MaHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, maNhanVien = maNhanVien, xuongId = xuongId })
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
        public List<TEntity> GetTongHopThanhPhamByMaHoSos<TEntity>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.MaNhanVien,
n.MaHoSo,
n.Name as NhanVienName,
p.MaLo,la.Ten As LoaiCaName,
tp.Ten as ThanhPhamName,
s.Ten As SizeName,
ma.Ten As MauName,
SUM(p.TrongLuong) as TrongLuong,
Count(*) As SoRo 
from  
PhieuCanPhuXepKhuon p,
MaLoaiCaXepKhuon la,
MaSizeXepKhuon s,
MaThanhPhamXepKhuon tp,
MaMauXepKhuon ma,
NhanVienDaiThanh n 
where 
p.Ngay <=@toDate 
and p.Ngay >=@fromDate
and MaXuong=@xuongId 
and p.MaLoaiCa = la.Ma 
and p.MaSize = s.Ma 
and p.MaThanhPham = tp.Ma 
and p.MaMau = ma.Ma 
and p.MaNhanVien= n.MaNhanVien 
and n.MaHoSo = @maHoSo
group by 
p.MaNhanVien,
n.MaHoSo,
n.Name ,
p.MaLo,
tp.Ten ,
s.Ten ,
ma.Ten,
la.Ten 
order by 
n.MaHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, maHoSo = maHoSo, xuongId = xuongId })
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
        public List<TEntity> GetTongHopThanhPhamByMaThes<TEntity>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.MaNhanVien,
n.MaHoSo,
n.Name as NhanVienName,
p.MaLo,la.Ten As LoaiCaName,
tp.Ten as ThanhPhamName,
s.Ten As SizeName,
ma.Ten As MauName,
SUM(p.TrongLuong) as TrongLuong,
Count(*) As SoRo 
from  
PhieuCanPhuXepKhuon p,
MaLoaiCaXepKhuon la,
MaSizeXepKhuon s,
MaThanhPhamXepKhuon tp,
MaMauXepKhuon ma,
NhanVienDaiThanh n ,TheTu t
where 
p.Ngay <=@toDate 
and p.Ngay >=@fromDate
and MaXuong=@xuongId 
and p.MaLoaiCa = la.Ma 
and p.MaSize = s.Ma 
and p.MaThanhPham = tp.Ma 
and p.MaMau = ma.Ma 
and p.MaNhanVien= n.MaNhanVien 
and p.MaNhanVien = t.MaNhanVien
and t.MaTheTu = @maThe
group by 
p.MaNhanVien,
n.MaHoSo,
n.Name ,
p.MaLo,
tp.Ten ,
s.Ten ,
ma.Ten,
la.Ten 
order by 
n.MaHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, maThe = maThe, xuongId = xuongId })
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

        public List<TEntity> GetPhieuCanTongHops2<TEntity>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select p.MaLo,la.Ten As LoaiCaName,tp.Ten as ThanhPhamName,s.Ten As SizeName,ma.Ten As MauName, SUM(p.TrongLuong) as TrongLuong from  PhieuCanPhuXepKhuon p, MaLoaiCaXepKhuon la,MaSizeXepKhuon s, MaThanhPhamXepKhuon tp, MaMauXepKhuon ma where p.Ngay =@ngay and MaXuong=@xuongId and p.MaLoaiCa = la.Ma and p.MaSize = s.Ma and p.MaThanhPham = tp.Ma and p.MaMau = ma.Ma group by p.MaLo,tp.Ten ,s.Ten ,ma.Ten,la.Ten order by p.MaLo,tp.Ten,s.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { ngay = dateTime.Date, xuongId = xuongId })
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
        public List<T> GetPhieuCanTongHops2<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"
Select *
from (
        Select p.MaLo,
            la.Ten As LoaiCaName,
            p.MaThanhPham,
            tp.Ten as ThanhPhamName,
            s.Ten As SizeName,
            ma.Ten As MauName,
            SUM(p.TrongLuong) as TrongLuong,
            p.MaXuong
        from PhieuCanPhuXepKhuon p,
            MaLoaiCaXepKhuon la,
            MaSizeXepKhuon s,
            MaThanhPhamXepKhuon tp,
            MaMauXepKhuon ma
        where Ngay <= @toDate
            and Ngay >= @fromDate
            and MaXuong = @xuongId
            and p.MaLoaiCa = la.Ma
            and p.MaSize = s.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaMau = ma.Ma
        group by p.MaLo,
            tp.Ten,
            s.Ten,
            ma.Ten,
            la.Ten,
            p.MaThanhPham,
            p.MaXuong
    ) p
where p.TrongLuong > 0
order by p.MaLo,
    p.ThanhPhamName,
    p.SizeName";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId })
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

        public bool Insert(PhieuCanPhuXepKhuon phieuCan)
        {
            try
            {
                var query =
                    "Insert Into PhieuCanPhuXepKhuon ([STT],[Ngay],[Gio],[MaLo],[MaLoaiCa],[MaThanhPham],[MaSize],[MaMau],[MaKhuVuc],[MaNhanVien],[MaNhom],[MaUserCan],[MaXuong],[MaMayCan],[TrongLuong],[GhiChu]) Values (@STT,@Ngay,@Gio,@MaLo,@MaLoaiCa,@MaThanhPham,@MaSize,@MaMau,@MaKhuVuc,@MaNhanVien,@MaNhom,@MaUserCan,@MaXuong,@MaMayCan,@TrongLuong,@GhiChu)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //using(var transaction = connection.BeginTransaction())
                    //{
                    var affectedRows = connection.Execute(query, phieuCan); //, transaction);
                    //transaction.Commit();
                    if (affectedRows > 0)
                        return true;
                    return false;
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public int Insert(List<PhieuCanPhuXepKhuon> phieuCans)
        {
            try
            {
                var query =
                    "Insert Into PhieuCanPhuXepKhuon ([STT],[Ngay],[Gio],[MaLo],[MaLoaiCa],[MaThanhPham],[MaSize],[MaMau],[MaKhuVuc],[MaNhanVien],[MaNhom],[MaUserCan],[MaXuong],[MaMayCan],[TrongLuong],[GhiChu]) Values (@STT,@Ngay,@Gio,@MaLo,@MaLoaiCa,@MaThanhPham,@MaSize,@MaMau,@MaKhuVuc,@MaNhanVien,@MaNhom,@MaUserCan,@MaXuong,@MaMayCan,@TrongLuong,@GhiChu)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //using(var transaction = connection.BeginTransaction())
                    //{
                    var affectedRows = connection.Execute(query, phieuCans); //, transaction);
                    //transaction.Commit();
                    //if(affectedRows > 0)
                    //    return true;
                    //return false;
                    //}
                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public bool Update(PhieuCanPhuXepKhuon phieuCan)
        {
            try
            {
                var query =
                    "Update PhieuCanPhuXepKhuon Set [Gio]=@Gio,[MaLo]=@MaLo,[MaLoaiCa]=@MaLoaiCa,[MaThanhPham]=@MaThanhPham,[MaSize]=@MaSize,[MaMau]=@MaMau,[MaKhuVuc]=@MaKhuVuc,[MaNhanVien]=@MaNhanVien,[MaNhom]=@MaNhom,[MaUserCan]=@MaUserCan,[TrongLuong]=@TrongLuong,[GhiChu]=@GhiChu Where [STT]=@STT and [Ngay]=@Ngay and [MaXuong]= @MaXuong and [MaMayCan]=@MaMayCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //using(var transaction = connection.BeginTransaction())
                    //{
                    var affectedRows = connection.Execute(query, phieuCan); //, transaction);
                    //transaction.Commit();
                    if (affectedRows > 0)
                        return true;
                    return false;
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public int Update(List<PhieuCanPhuXepKhuon> phieuCans)
        {
            try
            {
                var query =
                    "Update PhieuCanPhuXepKhuon Set [Gio]=@Gio,[MaLo]=@MaLo,[MaLoaiCa]=@MaLoaiCa,[MaThanhPham]=@MaThanhPham,[MaSize]=@MaSize,[MaMau]=@MaMau,[MaKhuVuc]=@MaKhuVuc,[MaNhanVien]=@MaNhanVien,[MaNhom]=@MaNhom,[MaUserCan]=@MaUserCan,[TrongLuong]=@TrongLuong,[GhiChu]=@GhiChu Where [STT]=@STT and [Ngay]=@Ngay and [MaXuong]= @MaXuong and [MaMayCan]=@MaMayCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //using(var transaction = connection.BeginTransaction())
                    //{
                    var affectedRows = connection.Execute(query, phieuCans); //, transaction);
                    //transaction.Commit();
                    //if(affectedRows > 0)
                    //    return true;
                    //return false;
                    //}
                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public bool Delete(PhieuCanPhuXepKhuon phieuCan)
        {
            try
            {
                var query =
                    "Delete PhieuCanPhuXepKhuon Where [STT]=@STT and [Ngay]=@Ngay and [MaXuong]= @MaXuong and [MaMayCan]=@MaMayCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //using(var transaction = connection.BeginTransaction())
                    //{
                    var affectedRows = connection.Execute(query, phieuCan); //, transaction);
                    //transaction.Commit();
                    if (affectedRows > 0)
                        return true;
                    return false;
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public int Delete(List<PhieuCanPhuXepKhuon> phieuCans)
        {
            try
            {
                var query =
                    "Delete PhieuCanPhuXepKhuon Where [STT]=@STT and [Ngay]=@Ngay and [MaXuong]= @MaXuong and [MaMayCan]=@MaMayCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //using(var transaction = connection.BeginTransaction())
                    //{
                    var affectedRows = connection.Execute(query, phieuCans); //, transaction);
                    // transaction.Commit();
                    return affectedRows;
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCanPhuXepKhuon_XLPC<T>(DateTime dateTime, string xuongId)
        {
            var query = @"select
p.STT,
p.Ngay,
p.Gio,
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
p.MaKhuVuc,
kv.Ten as KhuVucName,
p.MaNhanVien,
nv.Name as NhanVienName,
nv.MaHoSo,
nv.DeptName0 as Nhom,
p.MaUserCan,
p.MaMayCan,
p.TrongLuong,
p.MaXuong,
x.Ten as XuongName,
p.GhiChu

from PhieuCanPhuXepKhuon p
left join MaLoaiCaXepKhuon lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamXepKhuon tp on p.MaThanhPham = tp.Ma
left join MaSizeXepKhuon s on p.MaSize = s.Ma
left join MaMauXepKhuon m on p.MaMau = m.Ma
left join MaKhuVucXepKhuon kv on p.MaKhuVuc = kv.Ma
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
    }
}
