using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;


namespace Dao.Repos.HQ
{
    public partial class HQ_PhieuCan
    {
        private readonly string connectionString;
        private string tableName = @"HQ_PhieuCan";
        private readonly string qrDelete = @"DELETE FROM [dbo].[HQ_PhieuCan]
      WHERE [Id] = @Id
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[HQ_PhieuCan]
           ([Id]
           ,[STT]
           ,[Ngay]
           ,[NgayGio]
           ,[MayCan]
           ,[MaLo]
           ,[MaSize]
           ,[MaThanhPham]
           ,[MaLoaiNguyenLieu]
           ,[MaNhanVien]
           ,[MaNhanVienPhucVu]
           ,[MaNhanVienBanKiem]
           ,[TrongLuong]
           ,[TrongLuongTare]
           ,[ChiSanLuong]
           ,[Status]
           ,[TheId]
           ,[TheIdNhanVien]
           ,[GhiChu]
           ,[MaXuong])
     VALUES
           (@Id
           ,@STT
           ,@Ngay
           ,@NgayGio
           ,@MayCan
           ,@MaLo
           ,@MaSize
           ,@MaThanhPham
           ,@MaLoaiNguyenLieu
           ,@MaNhanVien
           ,@MaNhanVienPhucVu
           ,@MaNhanVienBanKiem
           ,@TrongLuong
           ,@TrongLuongTare
           ,@ChiSanLuong
           ,@Status
           ,@TheId
           ,@TheIdNhanVien
           ,@GhiChu
           ,@MaXuong)
";

        private readonly string qrUpdate = @"
UPDATE [dbo].[HQ_PhieuCan]
   SET
      [STT] = @STT
      ,[Ngay] = @Ngay
      ,[NgayGio] = @NgayGio
      ,[MayCan] = @MayCan
      ,[MaLo] = @MaLo
      ,[MaSize] = @MaSize
      ,[MaThanhPham] = @MaThanhPham
      ,[MaLoaiNguyenLieu] = @MaLoaiNguyenLieu
      ,[MaNhanVien] = @MaNhanVien
      ,[MaNhanVienPhucVu] = @MaNhanVienPhucVu
      ,[MaNhanVienBanKiem] = @MaNhanVienBanKiem
      ,[TrongLuong] = @TrongLuong
      ,[TrongLuongTare] = @TrongLuongTare
      ,[ChiSanLuong] = @ChiSanLuong
      ,[Status] = @Status
      ,[TheId] = @TheId
      ,[TheIdNhanVien] = @TheIdNhanVien
      ,[GhiChu] = @GhiChu
      ,[MaXuong] = @MaXuong
 WHERE [Id] =@Id
";

        public HQ_PhieuCan(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;
        }
        public T? Get<T>(string id)
        {
            try
            {
                var query = $"SELECT * FROM {tableName} WHERE Id = @id";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.QuerySingleOrDefault<T>(query, new { id });
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate,string xuongId)
        {
            try
            {
                var query = @"select
p.Id,
p.STT,
p.Ngay,
p.NgayGio,
p.MayCan,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLoaiNguyenLieu,
lnl.Ten as LoaiNguyenLieuName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.MaNhanVienPhucVu,
nvpv.MaHoSo as MaHoSoPV,
nvpv.Name as NhanVienPVName,
p.MaNhanVienBanKiem,
nvbk.MaHoSo as MaHoSoBK,
nvbk.Name as NhanVienBKName,
p.TrongLuong,
p.TrongLuongTare,
p.ChiSanLuong,
p.Status,
p.TheId,
p.TheIdNhanVien
from HQ_PhieuCan p,
	HQ_Size s,
	HQ_ThanhPham tp,
	HQ_LoaiNguyenLieu lnl,
	NhanVienDaiThanh nv,
	NhanVienDaiThanh nvpv,
	NhanVienDaiThanh nvbk

where p.Ngay >= @fromDate and p.Ngay <= @toDate  and p.MaXuong = @xuongId an
	and p.MaSize = s.Id
	and p.MaThanhPham = tp.Id
	and p.MaLoaiNguyenLieu = lnl.Id
	and p.MaNhanVien = nv.MaNhanVien
	and p.MaNhanVienPhucVu = nvpv.MaNhanVien
	and p.MaNhanVienBanKiem = nvbk.MaNhanVien
	order by
	p.STT desc
	
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date ,xuongId}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetChiTietXLPCs<T>( DateTime dateTime,string xuongId)
        {
            try
            {
                var query = @"select
p.Id,
p.STT,
p.Ngay,
p.NgayGio,
p.MayCan,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLoaiNguyenLieu,
lnl.Ten as LoaiNguyenLieuName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.MaNhanVienPhucVu,
nvpv.MaHoSo as MaHoSoPV,
nvpv.Name as NhanVienPVName,
p.MaNhanVienBanKiem,
nvbk.MaHoSo as MaHoSoBK,
nvbk.Name as NhanVienBKName,
p.TrongLuong,
p.TrongLuongTare,
p.ChiSanLuong,
p.Status,
p.TheId,
p.TheIdNhanVien,
p.GhiChu
from HQ_PhieuCan p,
	HQ_Size s,
	HQ_ThanhPham tp,
	HQ_LoaiNguyenLieu lnl,
	NhanVienDaiThanh nv,
	NhanVienDaiThanh nvpv,
	NhanVienDaiThanh nvbk

where p.Ngay = @dateTime and p.MaXuong = @xuongId
	and p.MaSize = s.Id
	and p.MaThanhPham = tp.Id
	and p.MaLoaiNguyenLieu = lnl.Id
	and p.MaNhanVien = nv.MaNhanVien
	and p.MaNhanVienPhucVu = nvpv.MaNhanVien
	and p.MaNhanVienBanKiem = nvbk.MaNhanVien
	order by
	p.STT desc
	
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { dateTime = dateTime.Date,xuongId}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        
        public List<T> GetTongHopNhanViens<T>(DateTime fromDate, DateTime toDate,string xuongId)
        {
            try
            {
                var query = @"select
p.Ngay,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLoaiNguyenLieu,
lnl.Ten as LoaiNguyenLieuName,
COUNT(*) as SoRo,
Sum(p.TrongLuong) as TrongLuong
from 
HQ_PhieuCan p,
NhanVienDaiThanh nv,
HQ_Size s,
HQ_ThanhPham tp,
HQ_LoaiNguyenLieu lnl
where
p.Ngay <= @fromDate and p.Ngay >= @toDate and p.MaXuong = @xuongId
and p.MaNhanVien = nv.MaNhanVien
and p.MaSize = s.Id
and p.MaThanhPham = tp.Id
and p.MaLoaiNguyenLieu =  lnl.Id
group by
p.Ngay,
p.MaNhanVien,
nv.MaHoSo,
nv.Name,
p.MaLo,
p.MaSize,
s.Ten,
p.MaThanhPham,
tp.Ten,
p.MaLoaiNguyenLieu,
lnl.Ten
	
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date ,xuongId}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetTongHopThanhPhams<T>(DateTime fromDate, DateTime toDate,string xuongId)
        {
            try
            {
                var query = @"select
p.Ngay,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLoaiNguyenLieu,
lnl.Ten as LoaiNguyenLieuName,
COUNT(*) as SoRo,
Sum(p.TrongLuong) as TrongLuong
from 
HQ_PhieuCan p,
NhanVienDaiThanh nv,
HQ_Size s,
HQ_ThanhPham tp,
HQ_LoaiNguyenLieu lnl
where
p.Ngay <= @fromDate and p.Ngay >= @toDate and p.MaXuong = @xuongId
and p.MaSize = s.Id
and p.MaThanhPham = tp.Id
and p.MaLoaiNguyenLieu =  lnl.Id
group by
p.Ngay,
p.MaLo,
    may.Ten,
    la.Ten,
tp.Ten,
    p.CaTra,
    p.IsEnabled
order by
    p.MaLo,
    p.IsEnabled,
    tp.Ten";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date,xuongId }).Result.ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetPhieuCanDeleteXLPCs<T>( DateTime dateTime,string xuongId)
        {
            try
            {
                var query = @"select
p.Id,
p.STT,
p.Ngay,
p.NgayGio,
p.MayCan,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLoaiNguyenLieu,
lnl.Ten as LoaiNguyenLieuName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.MaNhanVienPhucVu,
nvpv.MaHoSo as MaHoSoPV,
nvpv.Name as NhanVienPVName,
p.MaNhanVienBanKiem,
nvbk.MaHoSo as MaHoSoBK,
nvbk.Name as NhanVienBKName,
p.TrongLuong,
p.TrongLuongTare,
p.ChiSanLuong,
p.Status,
p.TheId,
p.TheIdNhanVien,
p.GhiChu
from HQ_PhieuCan_D p,
	HQ_Size s,
	HQ_ThanhPham tp,
	HQ_LoaiNguyenLieu lnl,
	NhanVienDaiThanh nv,
	NhanVienDaiThanh nvpv,
	NhanVienDaiThanh nvbk

where p.Ngay = @dateTime and p.MaXuong = @xuongId
	and p.MaSize = s.Id
	and p.MaThanhPham = tp.Id
	and p.MaLoaiNguyenLieu = lnl.Id
	and p.MaNhanVien = nv.MaNhanVien
	and p.MaNhanVienPhucVu = nvpv.MaNhanVien
	and p.MaNhanVienBanKiem = nvbk.MaNhanVien
	order by
	p.STT desc
	
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { dateTime = dateTime.Date,xuongId}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanUpdateXLPCs<T>( DateTime dateTime,string xuongId)
        {
            try
            {
                var query = @"select
p.Id,
p.STT,
p.Ngay,
p.NgayGio,
p.MayCan,
p.MaLo,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLoaiNguyenLieu,
lnl.Ten as LoaiNguyenLieuName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.MaNhanVienPhucVu,
nvpv.MaHoSo as MaHoSoPV,
nvpv.Name as NhanVienPVName,
p.MaNhanVienBanKiem,
nvbk.MaHoSo as MaHoSoBK,
nvbk.Name as NhanVienBKName,
p.TrongLuong,
p.TrongLuongTare,
p.ChiSanLuong,
p.Status,
p.TheId,
p.TheIdNhanVien,
p.GhiChu
from HQ_PhieuCan_U p,
	HQ_Size s,
	HQ_ThanhPham tp,
	HQ_LoaiNguyenLieu lnl,
	NhanVienDaiThanh nv,
	NhanVienDaiThanh nvpv,
	NhanVienDaiThanh nvbk

where p.Ngay = @dateTime and p.MaXuong = @xuongId
	and p.MaSize = s.Id
	and p.MaThanhPham = tp.Id
	and p.MaLoaiNguyenLieu = lnl.Id
	and p.MaNhanVien = nv.MaNhanVien
	and p.MaNhanVienPhucVu = nvpv.MaNhanVien
	and p.MaNhanVienBanKiem = nvbk.MaNhanVien
	order by
	p.STT desc
	
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { dateTime = dateTime.Date,xuongId}).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public Tuple<int, decimal> GetTongSoRoTongTrongLuong(DateTime dateTime, string nhanVienId)
        {
            try
            {
                var query =
                    "Select Count(*) as Item1,ISNULL(Sum(TrongLuong) ,0) As Item2 from HQ_PhieuCan where ChiSanLuong = 0 and MaNhanVien = @nhanVienId and Ngay =@ngay and ISNULL(GhiChu,'') <> 'HUY'";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.Query<Tuple<int, decimal>>(query, new { ngay = dateTime.Date, nhanVienId })
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Tuple<int, decimal, int, decimal> GetTongSoRoTongTrongLuong(
            DateTime dateTime,
            string nhanVienId,
            string thanhPhamId)
        {
            try
            {
                var query = @"Select
    Count(*) as Item1,
    ISNULL(Sum(TrongLuong), 0) As Item2,
    ISNULL(
        Sum(
            case
                when MaThanhPham = @thanhPhamId then 1
                else 0
            end
        ),
        0
    )  as Item3,
    ISNULL(
        Sum(
            case
                when MaThanhPham = @thanhPhamId then TrongLuong
                else 0
            end
        ),
        0
    ) as Item4
from
    HQ_PhieuCan WITH(READPAST)
where
    ChiSanLuong = 0
    and MaNhanVien = @nhanVienId
    and Ngay = @ngay
    and ISNULL(GhiChu, '') <> 'HUY'";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.Query<Tuple<int, decimal, int, decimal>>(
                            query,
                            new { ngay = dateTime.Date, nhanVienId, thanhPhamId })
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Tuple<int, decimal, int, decimal> GetTongSoRoTongTrongLuong_laychiSanLuong(
            DateTime dateTime,
            string nhanVienId,
            string thanhPhamId)
        {
            try
            {
                var query = @"Select
    Count(*) as Item1,
    ISNULL(Sum(TrongLuong), 0) As Item2,
    ISNULL(
        Sum(
            case
                when MaThanhPham = @thanhPhamId then 1
                else 0
            end
        ),
        0
    )  as Item3,
    ISNULL(
        Sum(
            case
                when MaThanhPham = @thanhPhamId then TrongLuong
                else 0
            end
        ),
        0
    ) as Item4
from
    HQ_PhieuCan WITH(READPAST)
where MaNhanVien = @nhanVienId
    and Ngay = @ngay
    and ISNULL(GhiChu, '') <> 'HUY'";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.Query<Tuple<int, decimal, int, decimal>>(
                            query,
                            new { ngay = dateTime.Date, nhanVienId, thanhPhamId })
                        .SingleOrDefault();
                    return item;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
	
        public List<T> GetChiTiets_TG<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"SELECT
    p.Ngay,
    p.STT,
    p.Gio,
    p.TrongLuong + p.TrongLuongTare as TrongLuongCan,
    p.TrongLuongTare,
    p.TrongLuong as TrongLuong,
    tp.Ten as MaSanPham,
    p.MaNhanVien,
    n.MaHoSo as MaHoSoPhucVu,
    p.MaLo,
    p.MaXuong,
    p.MaMayCan,
    0 as DonGia,
    0 * p.TrongLuong as ThanhTien,
    p.MaHoSo
from
    (
        Select
            p.*,
            nv.MaHoSo
        from
            HQ_PhieuCan p,
            NhanVienDaiThanh nv
        where
            p.Ngay <= @ngay and p.Ngay>=@fromDate
            and p.MaNhanVien = nv.MaNhanVien
    ) p
    left join MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
    left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
order by
    p.STT";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public int Insert<T>(T item)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(qrInsert, item);  
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Update<T>(T item)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(qrUpdate, item);
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Delete<T>(T item) {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(qrDelete, item);
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
