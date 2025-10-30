using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanSoCheDinhHinh
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanSoCheDinhHinh";
        private readonly string qrDelete = "Delete PhieuCanSoCheDinhHinh Where STT = @STT and Ngay = @Ngay and MaXuong = @MaXuong and MaMayCan = @MaMayCan";

        private readonly string qrInsert = @"
INSERT INTO  PhieuCanSoCheDinhHinh (STT,Ngay,Gio,MaLo,MaLoaiCa,MaThanhPham,MaNhanVien,MaXuong,MaMayCan,TrongLuong,GhiChu,MaMayLangDa,MaUserCan) VALUES (@STT,@Ngay,@Gio,@MaLo,@MaLoaiCa,@MaThanhPham, @MaNhanVien,@MaXuong,@MaMayCan,@TrongLuong,@GhiChu,@MaMayLangDa,@MaUserCan)";

        private readonly string qrUpdate = @"UPDATE [PhieuCanSoCheDinhHinh]  SET [Gio] = @Gio, [MaLo] = @MaLo, [MaLoaiCa] = @MaLoaiCa, [MaThanhPham] = @MaThanhPham,  [MaNhanVien] =@MaNhanVien,[TrongLuong] = @TrongLuong, [MaMayLangDa] =@MaMayLangDa,[MaUserCan] = @MaUserCan,[GhiChu] = @GhiChu  WHERE STT = @STT and Ngay = @Ngay and MaXuong = @MaXuong and MaMayCan = @MaMayCan ";

        private readonly string qrGetAll = "Select * from PhieuCanSoCheDinhHinh";

        public PhieuCanSoCheDinhHinh(string? _connectionString = null)
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
        public int Delete(List<PhieuCanSoCheDinhHinh> phieuCans)
        {
            try
            {
                var query = qrDelete;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.Execute(query, phieuCans);
                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
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

        public List<PhieuCanSoCheDinhHinh> GetPhieuCanSoCheDinhHinhs(DateTime dateTime, string xuongId)
        {
            try
            {
                try
                {
                    var query =
                        "Select * from PhieuCanSoCheDinhHinh Where Ngay=@ngay and MaXuong=@xuongId order by STT DESC";
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        var items = connection.QueryAsync<PhieuCanSoCheDinhHinh>(
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanSoCheDinhHinhs<T>(DateTime fromDate,DateTime toDate, string xuongId)
        {
            var query =
                @"select
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
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.TrongLuong,
p.MaMayLangDa,
ld.Ten as MayLangDaName,
p.MaUserCan,
p.GhiChu
from PhieuCanSoCheDinhHinh p
left join XiNghiep x on x.Ma = p.MaXuong
left join MaLoaiCaSoCheDinhHinh lc on lc.Ma = p.MaLoaiCa
left join MaThanhPhamSoCheDinhHinh tp on tp.Ma = p.MaThanhPham
left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
left join MayLangDa ld on ld.Ma = p.MaMayLangDa
where 
p.Ngay <= @toDate
and p.Ngay >= @fromDate";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { fromDate = fromDate.Date, toDate = toDate.Date,xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetPhieuCanSoCheDinhHinhByMaNhanViens<T>(DateTime fromDate,DateTime toDate,string maNhanVien, string xuongId)
        {
            var query =
                @"select
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
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.TrongLuong,
p.MaMayLangDa,
ld.Ten as MayLangDaName,
p.MaUserCan,
p.GhiChu
from PhieuCanSoCheDinhHinh p
left join XiNghiep x on x.Ma = p.MaXuong
left join MaLoaiCaSoCheDinhHinh lc on lc.Ma = p.MaLoaiCa
left join MaThanhPhamSoCheDinhHinh tp on tp.Ma = p.MaThanhPham
left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
left join MayLangDa ld on ld.Ma = p.MaMayLangDa
where 
p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId
and p.MaNhanVien = @maNhanVien";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { fromDate = fromDate.Date, toDate = toDate.Date,xuongId,maNhanVien })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetPhieuCanSoCheDinhHinhByMaHoSos<T>(DateTime fromDate,DateTime toDate,string maHoSo, string xuongId)
        {
            var query =
                @"select
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
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.TrongLuong,
p.MaMayLangDa,
ld.Ten as MayLangDaName,
p.MaUserCan,
p.GhiChu
from PhieuCanSoCheDinhHinh p
left join XiNghiep x on x.Ma = p.MaXuong
left join MaLoaiCaSoCheDinhHinh lc on lc.Ma = p.MaLoaiCa
left join MaThanhPhamSoCheDinhHinh tp on tp.Ma = p.MaThanhPham
left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
left join MayLangDa ld on ld.Ma = p.MaMayLangDa
where 
p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId
and nv.MaHoSo = @maHoSo";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { fromDate = fromDate.Date, toDate = toDate.Date,xuongId,maHoSo })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetPhieuCanSoCheDinhHinhByMaThes<T>(DateTime fromDate,DateTime toDate,string maThe, string xuongId)
        {
            var query =
                @"select
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
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.TrongLuong,
p.MaMayLangDa,
ld.Ten as MayLangDaName,
p.MaUserCan,
p.GhiChu
from PhieuCanSoCheDinhHinh p
left join XiNghiep x on x.Ma = p.MaXuong
left join MaLoaiCaSoCheDinhHinh lc on lc.Ma = p.MaLoaiCa
left join MaThanhPhamSoCheDinhHinh tp on tp.Ma = p.MaThanhPham
left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
left join MayLangDa ld on ld.Ma = p.MaMayLangDa
left join TheTu t on p.MaNhanVien = t.MaNhanVien
where 
p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId
and t.MaTheTu = @maThe";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { fromDate = fromDate.Date, toDate = toDate.Date,xuongId,maThe })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetPhieuCanTongHopsNhanVien<T>(DateTime fromDate,DateTime toDate, string xuongId)
        {
//            var query =
//                @"Select p.MaLo as LoId,n.MaHoSo, p.MaNhanVien,n.Name as TenNhanVien,tp.Ma as MaThanhPham,tp.Ten as ThanhPhamName,Sum(p.TrongLuong) as TrongLuong,Count(*) As SoRo 
//from PhieuCanSoCheDinhHinh p,
//MaThanhPhamSoCheDinhHinh tp,
//NhanVienDaiThanh n 
//where 
//p.Ngay <= @toDate
//and p.Ngay >= @fromDate
//and p.MaNhanVien = n.MaNhanVien 
//and p.MaThanhPham = tp.Ma 
//and MaXuong=@xuongId 
//and ISNULL(GhiChu,'') <> 'HUY' 
//Group by 
//p.MaNhanVien,
//n.Name,
//tp.Ma,
//tp.Ten,
//p.MaLo,
//n.MaHoSo";
var query =
                @"Select
p.MaLo as LoId,
n.MaHoSo, 
p.MaNhanVien,
n.Name as TenNhanVien,
tp.Ma as MaThanhPham,
tp.Ten as ThanhPhamName,
Sum(p.TrongLuong) as TrongLuong,
Count(*) As SoRo,
MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianVao,
MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianRa,
DATEDIFF(hour, MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay), MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay)) as TongThoiGian
from
PhieuCanSoCheDinhHinh p,
MaThanhPhamSoCheDinhHinh tp,
NhanVienDaiThanh n ,
CheckInOut c
where 
p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaNhanVien = n.MaNhanVien 
and p.MaThanhPham = tp.Ma 
and MaXuong=@xuongId 
and ISNULL(p.GhiChu,'') <> 'HUY' 
and n.MaChamCong = c.MaChamCong AND c.ThoiGian = p.Ngay AND c.ThoiGian >= @fromDate AND c.ThoiGian <= @toDate 
Group by 
p.MaNhanVien,
n.Name,
tp.Ma,
tp.Ten,
p.MaLo,
n.MaHoSo,
n.MaChamCong,
c.ThoiGian,p.Ngay";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { fromDate = fromDate.Date, toDate = toDate.Date,xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<PhieuCanSoCheDinhHinh> GetPhieuCanSoCheDinhHinhs(
            DateTime dateTime,
            string xuongId,
            string mayCanId)
        {
            try
            {
                try
                {
                    var query =
                        "Select * from PhieuCanSoCheDinhHinh Where Ngay=@ngay and MaXuong=@xuongId and MaMayCan=@mayCanId order by STT DESC";
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        var items = connection.QueryAsync<PhieuCanSoCheDinhHinh>(
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<TEntity> GetPhieuCanTinhLuongs<TEntity>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select p.Ngay,'CT04' as CaLamViec,p.MaNhanVien,tp.BravoId as MaSanPham,tp.Ten as TenSanPham,SUM(p.TrongLuong) as TrongLuong,'0' as _Status from PhieuCanSoCheDinhHinh p,MaThanhPhamSoCheDinhHinh tp where p.Ngay =@ngay and p.MaXuong =@xuongId and p.MaThanhPham = tp.Ma and tp.LoaiGui = 0 group by p.Ngay,p.MaNhanVien,tp.BravoId ,tp.Ten ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { ngay = dateTime.Date, xuongId = xuongId })
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

        public List<TEntity> GetPhieuCanTongHopsLoaiTP<TEntity>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select p.MaLo as LoId, tp.Ma as MaThanhPham,tp.Ten as ThanhPhamName,Sum(p.TrongLuong) as TrongLuong,Count(*) As SoRo from PhieuCanSoCheDinhHinh p,MaThanhPhamSoCheDinhHinh tp where Ngay =@ngay and p.MaThanhPham = tp.Ma and MaXuong=@xuongId and ISNULL(GhiChu,'') <> 'HUY' Group by tp.Ma,tp.Ten,p.MaLo ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<TEntity>(query, new { ngay = dateTime.Date, xuongId = xuongId })
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
         public List<T> GetPhieuCanTongHopsLoaiTP<T>(DateTime fromDate,DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.MaLo as LoId,
tp.Ma as MaThanhPham,
tp.Ten as ThanhPhamName,
Sum(p.TrongLuong) as TrongLuong,
Count(*) As SoRo,
p.MaXuong
from PhieuCanSoCheDinhHinh p,
MaThanhPhamSoCheDinhHinh tp 
where 
Ngay <=@ngay and
Ngay >= @fromDate
and p.MaThanhPham = tp.Ma 
and MaXuong=@xuongId 
and ISNULL(GhiChu,'') <> 'HUY' 
Group by 
tp.Ma,
tp.Ten,
p.MaLo,
p.MaXuong";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<T>(query, new { fromDate = fromDate.Date,ngay = toDate.Date, xuongId = xuongId })
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
        public List<T> GetTongHopThanhPhamsByMaNhanVien<T>(DateTime fromDate,DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
Sum(p.TrongLuong) as TrongLuong,
Count(*) As SoRo
from PhieuCanSoCheDinhHinh p,
MaThanhPhamSoCheDinhHinh tp ,
NhanVienDaiThanh nv
where 
p.Ngay <=@toDate 
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId
and p.MaThanhPham = tp.Ma 
and p.MaNhanVien = nv.MaNhanVien
and p.MaNhanVien = @maNhanVien
and ISNULL(GhiChu,'') <> 'HUY' 
Group by 
p.MaThanhPham,
tp.Ten,
p.MaLo,
p.MaNhanVien,
nv.MaHoSo,
nv.Name";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<T>(query, new { fromDate = fromDate.Date,toDate = toDate.Date, maNhanVien = maNhanVien ,xuongId = xuongId})
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
        public List<T> GetTongHopThanhPhamsByMaHoSo<T>(DateTime fromDate,DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
Sum(p.TrongLuong) as TrongLuong,
Count(*) As SoRo
from PhieuCanSoCheDinhHinh p,
MaThanhPhamSoCheDinhHinh tp ,
NhanVienDaiThanh nv
where 
p.Ngay <=@toDate 
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId
and p.MaThanhPham = tp.Ma 
and p.MaNhanVien = nv.MaNhanVien
and nv.MaHoSo = @maHoSo
and ISNULL(GhiChu,'') <> 'HUY' 
Group by 
p.MaThanhPham,
tp.Ten,
p.MaLo,
p.MaNhanVien,
nv.MaHoSo,
nv.Name";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<T>(query, new { fromDate = fromDate.Date,toDate = toDate.Date, maHoSo = maHoSo ,xuongId = xuongId})
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
        public List<T> GetTongHopThanhPhamsByMaThe<T>(DateTime fromDate,DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
Sum(p.TrongLuong) as TrongLuong,
Count(*) As SoRo
from PhieuCanSoCheDinhHinh p,
MaThanhPhamSoCheDinhHinh tp ,
NhanVienDaiThanh nv,
TheTu t
where 
p.Ngay <=@toDate 
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId
and p.MaThanhPham = tp.Ma 
and p.MaNhanVien = nv.MaNhanVien
and p.MaNhanVien = t.MaNhanVien
and t.MaTheTu = @maThe
and ISNULL(GhiChu,'') <> 'HUY' 
Group by 
p.MaThanhPham,
tp.Ten,
p.MaLo,
p.MaNhanVien,
nv.MaHoSo,
nv.Name";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<T>(query, new { fromDate = fromDate.Date,toDate = toDate.Date, maThe = maThe ,xuongId = xuongId})
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
        public List<TEntity> GetPhieuCanTongHopsNhanVien<TEntity>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select p.MaLo as LoId,n.MaHoSo, p.MaNhanVien,n.Name as TenNhanVien,tp.Ma as MaThanhPham,tp.Ten as ThanhPhamName,Sum(p.TrongLuong) as TrongLuong,Count(*) As SoRo from PhieuCanSoCheDinhHinh p,MaThanhPhamSoCheDinhHinh tp,NhanVienDaiThanh n where Ngay =@ngay and p.MaNhanVien = n.MaNhanVien and p.MaThanhPham = tp.Ma and MaXuong=@xuongId and ISNULL(GhiChu,'') <> 'HUY' Group by p.MaNhanVien,n.Name,tp.Ma,tp.Ten,p.MaLo,n.MaHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection
                        .QueryAsync<TEntity>(query, new { ngay = dateTime.Date, xuongId = xuongId })
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

        public double GetSanLuong(TimeSpan fromTime, TimeSpan toTime, DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select ISNULL(SUM(TrongLuong),0) from PhieuCanSoCheDinhHinh p, MaThanhPhamSoCheDinhHinh tp where Ngay = @ngay and Gio between @fromTime and @toTime and MaXuong = @xuongId and tp.LoaiGui =0 and  p.MaThanhPham = tp.Ma";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<double>(
                        query,
                        new

                        {
                            ngay = dateTime.Date,
                            fromTime = fromTime,
                            toTime = toTime,
                            xuongId = xuongId
                        });
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
                    $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanSoCheDinhHinh p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaThanhPham in {listOfIdsJoined} and p.Gio >= @fromTime and p.Gio < @toTime ";
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

        public double GetSanLuong(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            List<string> los,
            bool loaiGui,
            bool truocLangDa,
            bool sauLangDa,
            bool nhan)
        {
            try
            {
                string listOfIdsJoined = "('" + String.Join("','", los.ToArray()) + "')";
                var query =
                    $@"Select ISNULL(SUM(TrongLuong),0) from PhieuCanSoCheDinhHinh p, MaThanhPhamSoCheDinhHinh tp where Ngay = @ngay and Gio >= @fromTime and Gio < @toTime and MaXuong = @xuongId and  p.MaThanhPham = tp.Ma and p.MaLo in {listOfIdsJoined} and tp.LoaiGui = @loaiGui and TruocLangDa = @truocLangDa and SauLangDa = @sauLangDa and Nhan = @nhan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<double>(
                        query,
                        new

                        {
                            ngay = dateTime.Date,
                            fromTime = fromTime,
                            toTime = toTime,
                            xuongId = xuongId,
                            loaiGui = loaiGui,
                            truocLangDa = truocLangDa,
                            sauLangDa = sauLangDa,
                            nhan = nhan
                        });
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public double GetSanLuongs(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select Isnull( Sum(TrongLuong),0) from PhieuCanSoCheDinhHinh where Ngay= @ngay and MaXuong = @xuongId and ISNULL(GhiChu,'') <> 'HUY'";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<double>(query, new { ngay = dateTime.Date, xuongId = xuongId })
                        .Result
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public decimal GetSanLuongs(DateTime dateTime, string xuongId, IEnumerable<string> idThanhPhams)
        {
            try
            {
                string listOfIdsJoined = "('" + String.Join("','", idThanhPhams.ToArray()) + "')";
                var query =
                    $@"Select IsNull( Sum(TrongLuong),0 )from PhieuCanSoCheDinhHinh where Ngay= @ngay and MaXuong = @xuongId and ISNULL(GhiChu,'') <> 'HUY' and MaThanhPham In {listOfIdsJoined}";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<decimal>(query, new { ngay = dateTime.Date, xuongId = xuongId })
                        .Result
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public double GetSanLuongs(
            DateTime dateTime,
            string xuongId,
            bool isNguyenLieu)
        {
            try
            {
                var query =
                    "Select ISNull( Sum(p.TrongLuong),0) from PhieuCanSoCheDinhHinh p, MaThanhPhamSoCheDinhHinh tp where p.Ngay = @ngay and p.MaXuong = @xuongId and p.MaThanhPham = tp.Ma and tp.NguyenLieu =@isNguyenLieu ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<double>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId,
                                isNguyenLieu = isNguyenLieu
                            })
                        .Result
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public double GetSanLuongsBatCO(
            DateTime dateTime,
            string xuongId,
            bool isBatCO)
        {
            try
            {
                var query =
                    "Select ISNull( Sum(p.TrongLuong),0) from PhieuCanSoCheDinhHinh p, MaThanhPhamSoCheDinhHinh tp where p.Ngay = @ngay and p.MaXuong = @xuongId and p.MaThanhPham = tp.Ma  and tp.BatCO =@isBatCO ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<double>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId,
                                isBatCO = isBatCO
                            })
                        .Result
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<TEntity> GetSanLuongs<TEntity>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> idThanhPhams,
            IEnumerable<string> nhanVienIds)
        {
            try
            {
                string listOfIdsJoined = "('" + String.Join("','", idThanhPhams.ToArray()) + "')";
                string listOfIdsNhanViensJoined = "('" + String.Join("','", nhanVienIds.ToArray()) + "')";
                var query =
                    $@"Select p.Ngay,'CT04' as CaLamViec,p.MaNhanVien,tp.BravoId as MaSanPham,tp.Ten as TenSanPham,SUM(p.TrongLuong) as TrongLuong,'0' as _Status from PhieuCanSoCheDinhHinh p,MaThanhPhamSoCheDinhHinh tp where p.Ngay = @ngay and p.MaXuong =@xuongId and p.MaThanhPham = tp.Ma and ISNULL(p.GhiChu,'') <> 'HUY' and p.MaThanhPham In {listOfIdsJoined} and p.MaNhanVien in {listOfIdsNhanViensJoined} and tp.LoaiGui = 0 group by p.Ngay,p.MaNhanVien,tp.BravoId ,tp.Ten ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<TEntity>(query, new { ngay = dateTime.Date, xuongId = xuongId })
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

        public double GetSanLuongs(
            DateTime dateTime,
            string xuongId,
            bool isNguyenLieu,
            bool isBan09)
        {
            try
            {
                var query =
                    "Select Isnull( Sum(p.TrongLuong) ,0)  from PhieuCanSoCheDinhHinh p, MaThanhPhamSoCheDinhHinh tp where p.Ngay = @ngay and p.MaXuong = @xuongId  and p.MaThanhPham = tp.Ma and tp.NguyenLieu =@isNguyenLieu and tp.Ban09 = @isBan09";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<double>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId,
                                isNguyenLieu = isNguyenLieu,
                                isBan09 = isBan09
                            })
                        .Result
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public double GetSanLuongs(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            bool isNguyenLieu)
        {
            try
            {
                var query =
                    "Select ISNull( Sum(p.TrongLuong),0) from PhieuCanSoCheDinhHinh p, MaThanhPhamSoCheDinhHinh tp where p.Ngay = @ngay and p.MaXuong = @xuongId and Gio >= @fromTime and Gio < @toTime and p.MaThanhPham = tp.Ma and tp.NguyenLieu =@isNguyenLieu ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<double>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId,
                                isNguyenLieu = isNguyenLieu,
                                fromTime = fromTime,
                                toTime = toTime
                            })
                        .Result
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public double GetSanLuongs(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            bool isNguyenLieu,
            bool isBan09)
        {
            try
            {
                var query =
                    "Select Isnull( Sum(p.TrongLuong) ,0)  from PhieuCanSoCheDinhHinh p, MaThanhPhamSoCheDinhHinh tp where p.Ngay = @ngay and p.MaXuong = @xuongId and Gio >= @fromTime and Gio < @toTime and p.MaThanhPham = tp.Ma and tp.NguyenLieu =@isNguyenLieu and tp.Ban09 = @isBan09";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<double>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId,
                                isNguyenLieu = isNguyenLieu,
                                fromTime = fromTime,
                                toTime = toTime,
                                isBan09 = isBan09
                            })
                        .Result
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public double GetSanLuongs(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            bool isNguyenLieu,
            bool isBan09,
            bool loaiGui,
            bool truocLangDa,
            bool sauLangDa)
        {
            try
            {
                var query =
                    "Select Isnull( Sum(p.TrongLuong) ,0) from PhieuCanSoCheDinhHinh p, MaThanhPhamSoCheDinhHinh tp where p.Ngay = @ngay and p.MaXuong = @xuongId and Gio >= @fromTime and Gio < @toTime and p.MaThanhPham = tp.Ma and tp.NguyenLieu =@isNguyenLieu and tp.Ban09 = @isBan09 and LoaiGui =@loaiGui and TruocLangDa = @truocLangDa and SauLangDa =@sauLangDa";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<double>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                xuongId = xuongId,
                                isNguyenLieu = isNguyenLieu,
                                fromTime = fromTime,
                                toTime = toTime,
                                isBan09 = isBan09,
                                loaiGui = loaiGui,
                                truocLangDa = truocLangDa,
                                sauLangDa = sauLangDa
                            })
                        .Result
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<TEntity> GetSanLuongsNotInNhanVienIds<TEntity>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> idThanhPhams,
            IEnumerable<string> nhanVienIds)
        {
            try
            {
                string listOfIdsJoined = "('" + String.Join("','", idThanhPhams.ToArray()) + "')";
                string listOfIdsNhanViensJoined = "('" + String.Join("','", nhanVienIds.ToArray()) + "')";
                var query =
                    $@"Select p.Ngay,'CT04' as CaLamViec,p.MaNhanVien,tp.BravoId as MaSanPham,tp.Ten as TenSanPham,SUM(p.TrongLuong) as TrongLuong,'0' as _Status from PhieuCanSoCheDinhHinh p,MaThanhPhamSoCheDinhHinh tp where p.Ngay = @ngay and p.MaXuong =@xuongId and p.MaThanhPham = tp.Ma and ISNULL(p.GhiChu,'') <> 'HUY' and p.MaThanhPham In {listOfIdsJoined} and p.MaNhanVien Not In {listOfIdsNhanViensJoined} and tp.LoaiGui=0 group by p.Ngay,p.MaNhanVien,tp.BravoId ,tp.Ten ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<TEntity>(query, new { ngay = dateTime.Date, xuongId = xuongId })
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

        public Tuple<int, decimal> GetTongSoRoTongTrongLuongByNhanVienId(
            DateTime dateTime,
            string nhanVienId,
            string xuongId)
        {
            try
            {
                var query =
                    "Select Count(*) as Item1,ISNULL(Sum(TrongLuong) ,0) As Item2 from PhieuCanSoCheDinhHinh where MaNhanVien = @nhanVienId and Ngay =@ngay and MaXuong =@xuongId and ISNULL(GhiChu,'') <> 'HUY'";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection
                        .QueryAsync<Tuple<int, decimal>>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                nhanVienId = nhanVienId,
                                xuongId = xuongId
                            })
                        .Result
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public bool Insert(PhieuCanSoCheDinhHinh phieuCan)
        {
            try
            {
                var query =
                    "INSERT INTO  PhieuCanSoCheDinhHinh (STT,Ngay,Gio,MaLo,MaLoaiCa,MaThanhPham,MaNhanVien,MaXuong,MaMayCan,TrongLuong,GhiChu,MaMayLangDa,MaUserCan) VALUES (@STT,@Ngay,@Gio,@MaLo,@MaLoaiCa,@MaThanhPham, @MaNhanVien,@MaXuong,@MaMayCan,@TrongLuong,@GhiChu,@MaMayLangDa,@MaUserCan)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.Execute(query, phieuCan);
                    if (affectedRows > 0)
                        return true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public int Insert(List<PhieuCanSoCheDinhHinh> phieuCans)
        {
            try
            {
                var query =
                    "INSERT INTO  PhieuCanSoCheDinhHinh (STT,Ngay,Gio,MaLo,MaLoaiCa,MaThanhPham,MaNhanVien,MaXuong,MaMayCan,TrongLuong,GhiChu,MaMayLangDa,MaUserCan) VALUES (@STT,@Ngay,@Gio,@MaLo,@MaLoaiCa,@MaThanhPham, @MaNhanVien,@MaXuong,@MaMayCan,@TrongLuong,@GhiChu,@MaMayLangDa,@MaUserCan)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.Execute(query, phieuCans);
                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public int Update(List<PhieuCanSoCheDinhHinh> phieuCans)
        {
            try
            {
                var query =
                    "UPDATE [PhieuCanSoCheDinhHinh]  SET [Gio] = @Gio, [MaLo] = @MaLo, [MaLoaiCa] = @MaLoaiCa, [MaThanhPham] = @MaThanhPham,  [MaNhanVien] =@MaNhanVien,[TrongLuong] = @TrongLuong, [MaMayLangDa] =@MaMayLangDa,[MaUserCan] = @MaUserCan,[GhiChu] = @GhiChu  WHERE STT = @STT and Ngay = @Ngay and MaXuong = @MaXuong and MaMayCan = @MaMayCan ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.Execute(query, phieuCans);
                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public bool Update(PhieuCanSoCheDinhHinh phieuCan)
        {
            try
            {
                var query =
                    "UPDATE [PhieuCanSoCheDinhHinh]  SET [Gio] = @Gio, [MaLo] = @MaLo, [MaLoaiCa] = @MaLoaiCa, [MaThanhPham] = @MaThanhPham,  [MaNhanVien] =@MaNhanVien,[TrongLuong] = @TrongLuong, [MaMayLangDa] =@MaMayLangDa,[MaUserCan] = @MaUserCan,[GhiChu] = @GhiChu  WHERE STT = @STT and Ngay = @Ngay and MaXuong = @MaXuong and MaMayCan = @MaMayCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.Execute(query, phieuCan);
                    if (affectedRows > 0)
                        return true;
                    return false;
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
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
nv.DeptName0 as Nhom,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.TrongLuong,
p.MaMayLangDa,
ld.Ten as LangDaName,
p.MaUserCan,
p.GhiChu
from PhieuCanSoCheDinhHinh p
left join MaLoaiCaSoCheDinhHinh lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamSoCheDinhHinh tp on p.MaThanhPham = tp.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join XiNghiep x on p.MaXuong = x.Ma
left join MayLangDa ld on p.MaMayLangDa = ld.Ma
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
