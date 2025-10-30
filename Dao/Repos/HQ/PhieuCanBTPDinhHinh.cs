using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanBTPDinhHinh
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanBTPDinhHinh";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhieuCanBTPDinhHinh]
      WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanBTPDinhHinh]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MaUserCan]
           ,[MaMayCan]
           ,[MaLoaiCa]
           ,[MaMau]
           ,[MaSize]
           ,[MaThanhPham]
           ,[MaLo]
           ,[MaThe]
           ,[MaNhanVien]
           ,[MaMayLangDa]
           ,[TrongLuong]
           ,[IsEnabled]
           ,[MaXuong]
           ,[CaTra]
           ,[GhiChu],[ChiSanLuong],[TrongLuongTare],[TrongLuongBu],[IsOffline],[Id])
     VALUES
           (@STT 
           ,@Ngay 
           ,@Gio 
           ,@MaUserCan 
           ,@MaMayCan 
           ,@MaLoaiCa 
           ,@MaMau 
           ,@MaSize 
           ,@MaThanhPham 
           ,@MaLo 
           ,@MaThe 
           ,@MaNhanVien 
           ,@MaMayLangDa 
           ,@TrongLuong 
           ,@IsEnabled 
           ,@MaXuong 
           ,@CaTra 
           ,@GhiChu,@ChiSanLuong,@TrongLuongTare,@TrongLuongBu,@IsOffline,@Id)";
        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanBTPDinhHinh]
   SET [Gio] = @Gio
      ,[MaUserCan] = @MaUserCan 
       
      ,[MaLoaiCa] = @MaLoaiCa 
      ,[MaMau] = @MaMau 
      ,[MaSize] = @MaSize 
      ,[MaThanhPham] = @MaThanhPham 
      ,[MaLo] = @MaLo 
      ,[MaThe] = @MaThe 
      ,[MaNhanVien] = @MaNhanVien 
      ,[MaMayLangDa] = @MaMayLangDa 
      ,[TrongLuong] = @TrongLuong 
      ,[IsEnabled] = @IsEnabled 
      
      ,[CaTra] = @CaTra 
      ,[GhiChu] = @GhiChu,ChiSanLuong=@ChiSanLuong,[TrongLuongBu] = @TrongLuongBu, [IsOffline] =@IsOffline
 WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";
        private readonly string qrUpdateIdIsEnabled = @"UPDATE [dbo].[PhieuCanBTPDinhHinh]
   SET 
      [IsEnabled] = @isEnabled 
 WHERE [Id] = @id";

        private readonly string qrGetAll = "Select * from PhieuCanBTPDinhHinh";

        public PhieuCanBTPDinhHinh(string? _connectionString = null)
        {
            connectionString =_connectionString??AppViewModels.Base.Ins.ConnectionString;

        }

        public int Delete<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrDelete, item);
            return rows;
        }
        public int Delete(DateTime dateTime)
        {
            try
            {
                var query = @"DELETE FROM [dbo].[PhieuCanBTPDinhHinh]
                    WHERE [Ngay]= @Ngay";
                using (var connection = new SqlConnection(connectionString))
                {
                    if (connectionString.Contains("PMS_HQ") == false)
                    {
                        return 0;
                    }

                    connection.Open();
                    var rows = connection.Execute(query, new { ngay = dateTime.Date });
                    return rows;
                }
            }
            catch (Exception)
            {
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
        public int Update(string id,bool isEnabled)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrUpdateIdIsEnabled, new {id,isEnabled});
            return rows;
        }
        public List<T> GetsTongHopMayCan<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    @$"Select MaMayCan,SUM(TrongLuong) as TrongLuong from PhieuCanBTPDinhHinh where Ngay=@Ngay and MaXuong = @xuongId Group by MaMayCan order by MaMayCan ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(query, new { Ngay = dateTime.Date, xuongId }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public T Get<T>(DateTime dateTime, string theId, bool isEnabled = false)
        {
            try
            {
                var query = @"Select
    *
from
    PhieuCanBTPDinhHinh
Where
    Ngay = @ngay
    and MaThe = @theId
    and IsEnabled = @isEnabled
order by
    STT DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.Query<T>(query, new { ngay = dateTime.Date, theId, isEnabled }).FirstOrDefault();
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var query =  @"WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MaMayCan ORDER BY Ngay DESC, Gio DESC) AS RowNum
    FROM PhieuCanBTPDinhHinh where Ngay =@ngay
)

SELECT *
FROM RankedPhieu
WHERE RowNum <= @num";
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
        public List<T> GetChiTiets<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    p.STT,
    p.Ngay,
    p.Gio,
    p.Malo,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,
    la.Ten as LoaiCa,
    tp.Ten as ThanhPhamName,
    s.Ten as SizeName,
    mau.Ten as Mau,
    p.CaTra,
    p.MaThe,
    p.TrongLuong,
    p.TrongLuongBu,
    p.TrongLuongTare,
    p.IsEnabled as MoKhoa,
    may.Ten as MayLangDa,
    p.MaMayCan,
    p.MaXuong,
    p.GhiChu
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    MayLangDa may
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
order by
    p.MaLo,
    p.MaMayCan,
    p.STT desc";
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
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"Select
    p.STT,
    p.Ngay,
    p.Gio,
    p.Malo,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,

    p.MaNhanVienPhucVu,
    n1.MaHoSo as MaHoSoPV,
    n1.Name as TenNhanVienPV,

    la.Ten as LoaiCa,
    tp.Ten as ThanhPhamName,
    s.Ten as SizeName,
    mau.Ten as Mau,
    p.CaTra,
    p.MaThe,
    p.TrongLuong,
    p.TrongLuongBu,
    p.TrongLuongTare,
    p.IsEnabled as MoKhoa,
    may.Ten as MayLangDa,
    p.MaMayCan,
    p.MaXuong,
    p.GhiChu
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    NhanVienDaiThanh n1,
    MayLangDa may
where
    p.Ngay >= @fromDate
    and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and p.MaNhanVienPhucVu = n1.MaNhanVien
order by
    p.MaLo,
    p.MaMayCan,
    p.STT desc";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId }).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTietByMaNhanViens<T>(DateTime fromDate, DateTime toDate,string maNhanVien, string xuongId)
        {
            try
            {
                var query = @"Select
    p.STT,
    p.Ngay,
    p.Gio,
    p.Malo,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,

    p.MaNhanVienPhucVu,
    n1.MaHoSo as MaHoSoPV,
    n1.Name as TenNhanVienPV,

    la.Ten as LoaiCa,
    tp.Ten as ThanhPhamName,
    s.Ten as SizeName,
    mau.Ten as Mau,
    p.CaTra,
    p.MaThe,
    p.TrongLuong,
    p.TrongLuongBu,
    p.TrongLuongTare,
    p.IsEnabled as MoKhoa,
    may.Ten as MayLangDa,
    p.MaMayCan,
    p.MaXuong,
    p.GhiChu
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    NhanVienDaiThanh n1,
    MayLangDa may
where
    p.Ngay >= @fromDate
    and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
	and p.MaNhanVien = @maNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and p.MaNhanVienPhucVu = n1.MaNhanVien
order by
    p.MaLo,
    p.MaMayCan,
    p.STT desc";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId, maNhanVien = maNhanVien }).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTietByMaHoSos<T>(DateTime fromDate, DateTime toDate,string maHoSo, string xuongId)
        {
            try
            {
                var query = @"Select
    p.STT,
    p.Ngay,
    p.Gio,
    p.Malo,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,

    p.MaNhanVienPhucVu,
    n1.MaHoSo as MaHoSoPV,
    n1.Name as TenNhanVienPV,

    la.Ten as LoaiCa,
    tp.Ten as ThanhPhamName,
    s.Ten as SizeName,
    mau.Ten as Mau,
    p.CaTra,
    p.MaThe,
    p.TrongLuong,
    p.TrongLuongBu,
    p.TrongLuongTare,
    p.IsEnabled as MoKhoa,
    may.Ten as MayLangDa,
    p.MaMayCan,
    p.MaXuong,
    p.GhiChu
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    NhanVienDaiThanh n1,
    MayLangDa may
where
    p.Ngay >= @fromDate
    and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
	and n.MaHoSo = @maHoSo
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and p.MaNhanVienPhucVu = n1.MaNhanVien
order by
    p.MaLo,
    p.MaMayCan,
    p.STT desc";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId, maHoSo = maHoSo }).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTietByMaThes<T>(DateTime fromDate, DateTime toDate,string maThe, string xuongId)
        {
            try
            {
                var query = @"Select
    p.STT,
    p.Ngay,
    p.Gio,
    p.Malo,
    p.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,

    p.MaNhanVienPhucVu,
    n1.MaHoSo as MaHoSoPV,
    n1.Name as TenNhanVienPV,

    la.Ten as LoaiCa,
    tp.Ten as ThanhPhamName,
    s.Ten as SizeName,
    mau.Ten as Mau,
    p.CaTra,
    p.MaThe,
    p.TrongLuong,
    p.TrongLuongBu,
    p.TrongLuongTare,
    p.IsEnabled as MoKhoa,
    may.Ten as MayLangDa,
    p.MaMayCan,
    p.MaXuong,
    p.GhiChu
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    NhanVienDaiThanh n1,
    MayLangDa may,
	TheTu t
where
    p.Ngay >= @fromDate
    and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
	and p.MaNhanVien = t.MaNhanVien
	and t.MaTheTu = @maThe
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and p.MaNhanVienPhucVu = n1.MaNhanVien
order by
    p.MaLo,
    p.MaMayCan,
    p.STT desc";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId, maThe = maThe }).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T? GetLastByThe<T>(DateTime dateTime, string theId)
        {
            try
            {
                var query =
                    "Select top(1) * from PhieuCanBTPDinhHinh where Ngay = @ngay And MaThe = @theId order by Gio Desc";
                //var query = @"Select top(1) * from PhieuCanBTPDinhHinh WITH(READPAST) where Ngay = @ngay And MaThe = @theId And IsEnabled =@isEnabled ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.Query<T>(query, new { ngay = dateTime.Date, theId }).FirstOrDefault();
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public T? GetLastByThe<T>(DateTime dateTime, string theId, bool isEnabled = false)
        {
            try
            {
                var query =
                    "Select top(1) * from PhieuCanBTPDinhHinh where Ngay = @ngay And MaThe = @theId And IsEnabled =@isEnabled and IsOffline = 0 order by Gio Desc";
                //var query = @"Select top(1) * from PhieuCanBTPDinhHinh WITH(READPAST) where Ngay = @ngay And MaThe = @theId And IsEnabled =@isEnabled ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.Query<T>(query, new { ngay = dateTime.Date, theId, isEnabled }).SingleOrDefault();
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public T GetLastByNhanVien<T>(DateTime dateTime, string nhanVienId)
        {
            try
            {
                var query =
                    "Select top(1) * from PhieuCanBTPDinhHinh where Ngay = @ngay And MaNhanVien = @nhanVienId order by Gio Desc";
                //var query = @"Select top(1) * from PhieuCanBTPDinhHinh WITH(READPAST) where Ngay = @ngay And MaThe = @theId And IsEnabled =@isEnabled ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.Query<T>(query, new { ngay = dateTime.Date, nhanVienId }).FirstOrDefault();
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public T GetMaxSTT<T>(DateTime dateTime, string mayCanId)
        {
            try
            {
                var query =
                    @"Select ISNULL( Max(STT),0) from PhieuCanBTPDinhHinh where Ngay=@ngay and MaMayCan = @mayCanId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.ExecuteScalar<T>(query, new { ngay = dateTime.Date, mayCanId });
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetMayCans(DateTime dateTime)
        {
            try
            {
                var query = @"Select DISTINCT MaMayCan from PhieuCanBTPDinhHinh WITH(READPAST) where Ngay = @ngay ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<string>(query, new { ngay = dateTime.Date }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetNumNhanVienDaChiaCa(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    COUNT(Distinct MaNhanVien)
from
    PhieuCanBTPDinhHinh
where
    Ngay = @ngay
    and MaXuong = @xuongId ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.ExecuteScalar<int>(query, new { ngay = dateTime.Date, xuongId });
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetNumTheDaSuDung(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    COUNT(Distinct MaThe)
from
    PhieuCanBTPDinhHinh
where
    Ngay = @ngay
    and MaXuong = @xuongId and ChiSanLuong = 0 and ISNULL(GhiChu, '') <> N'Không Đầu Vào'";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.ExecuteScalar<int>(query, new { ngay = dateTime.Date, xuongId });
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime)
        {
            try
            {
                var query = "Select * from PhieuCanBTPDinhHinh Where Ngay = @ngay ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
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
                    "Select * from PhieuCanBTPDinhHinh Where Ngay = @ngay and MaXuong= @xuongId order by STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId = xuongId }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetOfflines<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanBTPDinhHinh Where Ngay = @ngay and MaXuong= @xuongId and IsOffline = 1 order by STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId = xuongId }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime, string mayCanId, int stt)
        {
            try
            {
                var query =
                    "Select * from PhieuCanBTPDinhHinh where Ngay =@ngay and MaMayCan = @mayCanId and STT >@stt";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, mayCanId, stt }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime, string xuongId, string mayCanId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanBTPDinhHinh Where Ngay = @ngay and MaXuong= @xuongId and MaMayCan = @mayCanId order by STT DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId = xuongId, mayCanId = mayCanId })
                    .ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets_maycan<T>(DateTime dateTime, string mayCan)
        {
            try
            {
                var query =
                    "Select * from PhieuCanBTPDinhHinh WITH(READPAST) Where Ngay = @ngay and MaMayCan = @mayCanId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(query, new { ngay = dateTime.Date, maycanId = mayCan }).ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets_STT_IsEnabled<T>(DateTime dateTime, string mayCanId)
        {
            try
            {
                var query =
                    @"Select STT,IsEnabled,MaMayCan from PhieuCanBTPDinhHinh WITH(READPAST) where Ngay=@ngay and MaMayCan = @mayCanId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, mayCanId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets_STT_IsEnabled<T>(DateTime dateTime, string mayCanId, List<int> sttsEx)
        {
            try
            {
                string listOfIdsJoined = "('" + String.Join("','", sttsEx.ToArray()) + "')";
                var query =
                    @$"Select * from PhieuCanBTPDinhHinh WITH(READPAST) where Ngay=@ngay and MaMayCan = @mayCanId and STT NOT IN {listOfIdsJoined}";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, mayCanId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets_STT_IsEnabled<T>(DateTime dateTime, string mayCanId, bool isEnabled)
        {
            try
            {
                var query =
                    @$"Select * from PhieuCanBTPDinhHinh WITH(READPAST) where Ngay=@ngay and MaMayCan = @mayCanId and IsEnabled = @isEnabled";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, mayCanId, isEnabled }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public int ChuyenXuong<T>(List<T> items, string xuongId)
        {
            try
            {
                var query = $@"
UPDATE [dbo].[PhieuCanBTPDinhHinh]
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
        public int ChuyenSize<T>(List<T> items, string sizeId)
        {
            try
            {
                var query = $@"
UPDATE [dbo].[PhieuCanBTPDinhHinh]
   SET [MaSize] = '{sizeId}',[GhiChu] = @GhiChu
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
         public int ChuyenThanhPham<T>(List<T> items, string thanhPhamId)
        {
            try
            {
                var query = $@"
UPDATE [dbo].[PhieuCanBTPDinhHinh]
   SET [MaThanhPham] = '{thanhPhamId}',[GhiChu] = @GhiChu
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
        public int Delete<T>(List<T> items)
        {
            try
            {
                var query = @"DELETE FROM [dbo].[PhieuCanBTPDinhHinh]
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

        //public int InsertBatch(List<Models.Repos.Models.PhieuCanBTPDinhHinh> phieuCans)
        //{
        //    try
        //    {
        //        var batches = ToolEx.DbExtensions.GetSqlsInBatches(phieuCans); //GetSqlsInBatches(phieuCans);
        //        var row = 0;
        //        var database = new Modelv1.Dao.Database(connectionString);
        //        //MessageBox.Show(batches.Count.ToString());
        //        foreach (var batche in batches)
        //        {
        //            //MessageBox.Show(batche.ToString());
        //            row += database.ExecuteNonQuery(batche);
        //        }

        //        return row;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public IList<string> GetSqlsInBatches(IList<Models.Repos.Models.PhieuCanBTPDinhHinh> phieuCans)
        {
            var insertSql = @"INSERT INTO [dbo].[PhieuCanBTPDinhHinh]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MaUserCan]
           ,[MaMayCan]
           ,[MaLoaiCa]
           ,[MaMau]
           ,[MaSize]
           ,[MaThanhPham]
           ,[MaLo]
           ,[MaThe]
           ,[MaNhanVien]
           ,[MaMayLangDa]
           ,[TrongLuong]
           ,[IsEnabled]
           ,[MaXuong]
           ,[CaTra]
           ,[GhiChu]
           ,[ChiSanLuong],[TrongLuongTare],[TrongLuongBu],[IsOffline])
     VALUES";
            var valuesSql =
                @"({0},'{1}','{2}', '{3}', '{4}', '{5}', '{6}', '{7}','{8}','{9}','{10}','{11}', '{12}', {13}, {14}, '{15}',{16},'{17}',{18},{19},{20},{21})";
            var batchSize = 1000;

            var sqlsToExecute = new List<string>();
            var numberOfBatches = (int)Math.Ceiling((double)phieuCans.Count / batchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var phieuCanToInsert = phieuCans.Skip(i * batchSize).Take(batchSize);
                var valuesToInsert = phieuCanToInsert.Select(
                    x => string.Format(
                        valuesSql,
                        x.STT,
                        x.Ngay.ToString("yyyy-MM-dd"),
                        x.Gio.ToString(@"hh\:mm\:ss"),
                        x.MaUserCan,
                        x.MaMayCan,
                        x.MaLoaiCa,
                        x.MaMau,
                        x.MaSize,
                        x.MaThanhPham,
                        x.MaLo,
                        x.MaThe,
                        x.MaNhanVien,
                        x.MaMayLangDa,
                        x.TrongLuong,
                        x.IsEnabled ? 1 : 0,
                        x.MaXuong,
                        x.CaTra ? 1 : 0,
                        string.Empty,
                        x.ChiSanLuong ? 1 : 0,
                        x.TrongLuongTare,
                        x.TrongLuongBu,
                        x.IsOffline ? 1 : 0));
                sqlsToExecute.Add(insertSql + string.Join(",", valuesToInsert));
            }

            return sqlsToExecute;
        }






        public DataTable GetChiTiets(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    p.STT,
    p.Ngay as [Ngày],
     Cast(
        convert(
            varchar(19),
            cast(@ngay as datetime) + DATEADD(ms, -DATEPART(ms, Cast(p.Gio as datetime)), Cast(p.Gio as datetime)) ,
            120
        ) as datetime
    ) as [Giờ],
    p.Malo as [Lô],
    p.MaNhanVien as [Mã Nhân Viên],
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    
    la.Ten as [Loại Cá],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    mau.Ten as [Màu],
    p.CaTra as [Cá Trả],
    p.MaThe as [Mã Thẻ],
    p.TrongLuong as [Trọng Lượng],
    p.TrongLuongBu as [Trọng Lượng Bù],
    p.TrongLuongTare as [Tare],
    p.IsEnabled as [Mở Khóa],
    p.MaThe as [Mã Thẻ],
    may.Ten as [Máy Lạng Da],
    p.MaMayCan as [Máy Cân],
    p.MaXuong as [Xưởng],
    p.GhiChu as [Ghi Chú]
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    MayLangDa may
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
order by
    p.MaLo,
    p.MaMayCan,
    p.STT desc";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetChiTiets(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"Select
    p.STT,
    p.Ngay as [Ngày],
     Cast(
        convert(
            varchar(19),
            cast(p.Ngay as datetime) + DATEADD(ms, -DATEPART(ms, Cast(p.Gio as datetime)), Cast(p.Gio as datetime)) ,
            120
        ) as datetime
    ) as [Giờ],
    p.Malo as [Lô],
    p.MaNhanVien as [Mã Nhân Viên],
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    
    la.Ten as [Loại Cá],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    mau.Ten as [Màu],
    p.CaTra as [Cá Trả],
    p.MaThe as [Mã Thẻ],
    p.TrongLuong as [Trọng Lượng],
    p.TrongLuongBu as [Trọng Lượng Bù],
    p.TrongLuongTare as [Tare],
    p.IsEnabled as [Mở Khóa],
    p.MaThe as [Mã Thẻ],
    may.Ten as [Máy Lạng Da],
    p.MaMayCan as [Máy Cân],
    p.MaXuong as [Xưởng],
    p.IsOffline as [Offline],
    p.GhiChu as [Ghi Chú]
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    MayLangDa may
where
    p.Ngay >= @fromDate
and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and p.STT>0
    and IsNull( p.GhiChu,'') <> 'HUY'
order by
    p.MaLo,
    p.MaMayCan,
    p.STT desc";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@toDate", toDate.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public DataTable GetTongHops(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    p.MaLo as [Lô],
    may.Ten as [Máy Lạng Da],
    la.Ten as [Loại Cá],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    mau.Ten as [Màu],
    p.CaTra as [Cá Trả],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng]
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    MayLangDa may
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and p.ChiSanLuong = 0
and IsNull( p.GhiChu,'') <> 'HUY'
GROUP BY
    n.MaHoSo,
    n.Name,
    p.MaLo,
    may.Ten,
    la.Ten,
    tp.Ten,
    s.Ten,
    mau.Ten,
    p.CaTRa
order by
    n.MaHoSo,
    p.MaLo,
    tp.Ten,
    s.Ten,
    p.CaTra";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetTongHops(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"Select
 p.Ngay as [Ngày],
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    p.MaLo as [Lô],
    may.Ten as [Máy Lạng Da],
    la.Ten as [Loại Cá],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    mau.Ten as [Màu],
    p.CaTra as [Cá Trả],
    p.IsOffline as [Offline],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng]
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    MayLangDa may
where
     p.Ngay >= @fromDate
and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and p.ChiSanLuong = 0
    and p.STT>0
and IsNull( p.GhiChu,'') <> 'HUY'
GROUP BY
p.Ngay,
    n.MaHoSo,
    n.Name,
    p.MaLo,
    may.Ten,
    la.Ten,
    tp.Ten,
    s.Ten,
    mau.Ten,
    p.CaTRa,
 p.IsOffline
order by
    n.MaHoSo,
    p.MaLo,
    tp.Ten,
    s.Ten,
    p.CaTra";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@toDate", toDate.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetTongHops<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    n.MaNhanVien as MaNhanVien,
    n.Name as TenNhanVien,
    p.MaLo,
    may.Ten as MayLangDaName,
    la.Ten as LoaiCaName,
    tp.Ten as ThanhPhamName,
    s.Ten as SizeName,
    mau.Ten as [Màu],
    p.CaTra as CaTra,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    MayLangDa may
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and p.ChiSanLuong = 0
and IsNull( p.GhiChu,'') <> 'HUY'
GROUP BY
    n.MaNhanVien,
    n.Name,
    p.MaLo,
    may.Ten,
    la.Ten,
    tp.Ten,
    s.Ten,
    mau.Ten,
    p.CaTRa
order by
    n.MaNhanVien,
    p.MaLo,
    tp.Ten,
    s.Ten,
    p.CaTra";
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

        public List<T> GetTongHops<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"Select
    n.MaNhanVien as MaNhanVien,
    n.Name as TenNhanVien,
    p.MaLo,
    may.Ten as MayLangDaName,
    la.Ten as LoaiCaName,
    tp.Ten as ThanhPhamName,
    s.Ten as SizeName,
    mau.Ten as [Màu],
    mau.Ten as Mau,
    p.CaTra as CaTra,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    MayLangDa may
where
    p.Ngay >= @fromDate
and p.Ngay<= @toDate
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and p.ChiSanLuong = 0
and IsNull( p.GhiChu,'') <> 'HUY'
GROUP BY
    n.MaNhanVien,
    n.Name,
    p.MaLo,
    may.Ten,
    la.Ten,
    tp.Ten,
    s.Ten,
    mau.Ten,
    p.CaTRa
order by
    n.MaNhanVien,
    p.MaLo,
    tp.Ten,
    s.Ten,
    p.CaTra";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId }).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopNhanVienPhucVus<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"Select
    n.MaNhanVien as MaNhanVien,
    n.MaHoSo as MaHoSo,
    n.Name as TenNhanVien,
    p.MaLo,
    may.Ten as MayLangDaName,
    la.Ten as LoaiCaName,
    tp.Ten as ThanhPhamName,
    s.Ten as SizeName,
    mau.Ten as [Màu],
    mau.Ten as Mau,
    p.CaTra as CaTra,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    MayLangDa may
where
    p.Ngay >= @fromDate
    and p.Ngay<= @toDate
    and p.MaXuong = @xuongId
    and p.MaNhanVienPhucVu = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and p.ChiSanLuong = 0
and IsNull( p.GhiChu,'') <> 'HUY'
GROUP BY
    n.MaNhanVien,
    n.Name,
    p.MaLo,
    may.Ten,
    la.Ten,
    tp.Ten,
    s.Ten,
    mau.Ten,
    p.CaTRa
order by
    n.MaNhanVien,
    p.MaLo,
    tp.Ten,
    s.Ten,
    p.CaTra";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId }).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetTongHopThanhPhams(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    p.MaLo as [Lô],
    tp.Ten as [Thành Phẩm],
    p.CaTra as [Cá Trả],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng]
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    MayLangDa may
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and IsNull( p.GhiChu,'') <> 'HUY'
and p.ChiSanLuong =0
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTra
order by
    p.MaLo,
    tp.Ten,
    p.CaTra";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetTongHopThanhPhams(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"Select
    p.MaLo as [Lô],
    tp.Ten as [Thành Phẩm],
    p.CaTra as [Cá Trả],
    p.IsOffline as [Offline],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng]
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    MayLangDa may
where
    p.Ngay >= @fromDate
and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and IsNull( p.GhiChu,'') <> 'HUY'
    and p.STT>0
and p.ChiSanLuong =0
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTra,
    p.IsOffline
order by
    p.MaLo,
    tp.Ten,
    p.CaTra";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                cmd.Parameters.AddWithValue("@toDate", toDate.Date);
                cmd.Parameters.AddWithValue("@xuongId", xuongId);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetTongHopThanhPhams<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query = @"Select
    p.MaLo as MaLo,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.CaTra as CaTra,
    p.IsOffline as Offline,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong,
    p.MaXuong
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    MayLangDa may
where
    p.Ngay >= @fromDate
and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and IsNull( p.GhiChu,'') <> 'HUY'
    and p.STT>0
and p.ChiSanLuong =0
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTra,
    p.IsOffline,
    p.MaThanhPham,
     p.MaXuong
order by
    p.MaLo,
    tp.Ten,
    p.CaTra";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId = xuongId }).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaNhanViens<T>(DateTime fromDate, DateTime toDate, string maNhanVien, string xuongId)
        {
            try
            {
                var query = @"Select
	p.MaNhanVien,
	n.MaHoSo,
	n.Name as NhanVienName,
    p.MaLo as MaLo,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.CaTra as CaTra,
    p.IsOffline as Offline,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    MayLangDa may
where
    p.Ngay >= @fromDate
	and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
	and p.MaNhanVien = @maNhanVien
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and IsNull( p.GhiChu,'') <> 'HUY'
    and p.STT>0
and p.ChiSanLuong =0
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTra,
    p.IsOffline,
    p.MaThanhPham,
	p.MaNhanVien,
	n.MaHoSo,
	n.Name
order by
    p.MaLo,
    tp.Ten,
    p.CaTra";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, maNhanVien = maNhanVien, xuongId = xuongId }).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaHoSos<T>(DateTime fromDate, DateTime toDate, string maHoSo, string xuongId)
        {
            try
            {
                var query = @"Select
	p.MaNhanVien,
	n.MaHoSo,
	n.Name as NhanVienName,
    p.MaLo as MaLo,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.CaTra as CaTra,
    p.IsOffline as Offline,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    MayLangDa may
where
    p.Ngay >= @fromDate
	and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and n.MaHoSo = @maHoSo
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and IsNull( p.GhiChu,'') <> 'HUY'
    and p.STT>0
and p.ChiSanLuong =0
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTra,
    p.IsOffline,
    p.MaThanhPham,
	p.MaNhanVien,
	n.MaHoSo,
	n.Name
order by
    p.MaLo,
    tp.Ten,
    p.CaTra";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, maHoSo = maHoSo ,xuongId = xuongId}).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaThes<T>(DateTime fromDate, DateTime toDate, string maThe, string xuongId)
        {
            try
            {
                var query = @"Select
	p.MaNhanVien,
	n.MaHoSo,
	n.Name as NhanVienName,
    p.MaLo as MaLo,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.CaTra as CaTra,
    p.IsOffline as Offline,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaMauDinhHinh mau,
    NhanVienDaiThanh n,
    MayLangDa may,TheTu t
where
    p.Ngay >= @fromDate
	and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaNhanVien = t.MaNhanVien
	and t.MaTheTu = @maThe
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
    and p.MaMayLangDa = may.Ma
    and IsNull( p.GhiChu,'') <> 'HUY'
    and p.STT>0
and p.ChiSanLuong =0
GROUP BY
    p.MaLo,
    tp.Ten,
    p.CaTra,
    p.IsOffline,
    p.MaThanhPham,
	p.MaNhanVien,
	n.MaHoSo,
	n.Name
order by
    p.MaLo,
    tp.Ten,
    p.CaTra";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query,
                            new { fromDate = fromDate.Date, toDate = toDate.Date, maThe = maThe ,xuongId}).Result
                        .ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public double GetSanLuong(TimeSpan fromTime, TimeSpan toTime, DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select ISNULL(SUM(TrongLuong),0) from PhieuCanBTPDinhHinh Where  Ngay= @ngay and MaXuong = @xuongId and Gio between @fromTime and @toTime and ISNULL(GhiChu,'') <> 'HUY'";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<double>(
                        query,
                        new { ngay = dateTime.Date, xuongId = xuongId, fromTime = fromTime, toTime = toTime });
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public double GetSanLuong(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string mayLangDa)
        {
            try
            {
                var query =
                    "Select ISNULL(SUM(TrongLuong),0) from PhieuCanBTPDinhHinh Where  Ngay= @ngay and MaXuong = @xuongId and MaMayLangDa =@mayLangDa and Gio between @fromTime and @toTime and ISNULL(GhiChu,'') <> 'HUY'";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<double>(
                        query,
                        new
                        {
                            ngay = dateTime.Date,
                            xuongId = xuongId,
                            mayLangDa = mayLangDa,
                            fromTime = fromTime,
                            toTime = toTime
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

        public double GetSanLuong(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string mayLangDa,
            string exThanhPhamId)
        {
            try
            {
                var query =
                    $@"Select ISNULL(SUM(TrongLuong),0) from PhieuCanBTPDinhHinh Where  Ngay= @ngay and MaXuong = @xuongId and MaMayLangDa =@mayLangDa and Gio >= @fromTime and Gio < @toTime and ISNULL(GhiChu,'') <> 'HUY' and MaThanhPham not in ({@exThanhPhamId})";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<double>(
                        query,
                        new
                        {
                            ngay = dateTime.Date,
                            xuongId = xuongId,
                            mayLangDa = mayLangDa,
                            fromTime = fromTime,
                            toTime = toTime
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

        public double GetSanLuong(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string mayLangDa,
            string exThanhPhamId,
            string sizeId)
        {
            try
            {
                var query =
                    $@"Select ISNULL(SUM(TrongLuong),0) from PhieuCanBTPDinhHinh Where  Ngay= @ngay and MaXuong = @xuongId and MaMayLangDa =@mayLangDa and Gio >= @fromTime and Gio< @toTime and ISNULL(GhiChu,'') <> 'HUY' and MaSize = @sizeId and MaThanhPham not in ({@exThanhPhamId})";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<double>(
                        query,
                        new
                        {
                            ngay = dateTime.Date,
                            xuongId = xuongId,
                            mayLangDa = mayLangDa,
                            fromTime = fromTime,
                            toTime = toTime,
                            sizeId = sizeId
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


        public List<T> GetTongHopNhanh<T>(DateTime dateTime, string xuongId, string maHoSo)
        {
            try
            {
                var query = @"Select
    p.MaNhanVien,
    n.Name as NhanVienName,
    p.MaLo,
    la.Ten as LoaiCaName,
    tp.Ten as ThanhPhamName,
    p.CaTra,
    p.IsEnabled,
    COUNT(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong
from
    PhieuCanBTPDinhHinh p,
    NhanVienDaiThanh n,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and n.MaHoSo = @maHoSo
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.ChiSanLuong = 0 and ISNULL(GhiChu, '') <> 'HUY'
group by
    n.Name,
    p.MaNhanVien,
    p.MaLo,
    la.Ten,
    tp.Ten,
    p.CaTra,
    p.IsEnabled
order by
    p.MaLo,
    p.IsEnabled,
    tp.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { ngay = dateTime.Date, xuongId = xuongId, maHoSo = maHoSo })
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

        public List<T> GetTongHopNhanh_MaNhanVien<T>(DateTime dateTime, string xuongId, string maHoSo)
        {
            try
            {
                var query = @"Select
    p.MaNhanVien,
    n.Name as NhanVienName,
    p.MaLo,
    la.Ten as LoaiCaName,
    tp.Ten as ThanhPhamName,
    p.CaTra,
    p.IsEnabled,
    COUNT(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong
from
    PhieuCanBTPDinhHinh p,
    NhanVienDaiThanh n,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and n.MaNhanVien = @maHoSo
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.ChiSanLuong = 0 and ISNULL(GhiChu, '') <> 'HUY'
group by
    n.Name,
    p.MaNhanVien,
    p.MaLo,
    la.Ten,
    tp.Ten,
    p.CaTra,
    p.IsEnabled
order by
    p.MaLo,
    p.IsEnabled,
    tp.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(
                            query,
                            new { ngay = dateTime.Date, xuongId = xuongId, maHoSo = maHoSo })
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



        public List<T> GetTongHopTheoMayLangDa<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"
Select
    p.MaLo,
    may.Ten as MayLangDaName,
    la.Ten as LoaiCaName,
    tp.Ten as ThanhPhamName,
    p.CaTra,
    p.IsEnabled,
    COUNT(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong
from
    PhieuCanBTPDinhHinh p,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp,
    MayLangDa may
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaMayLangDa = may.Ma
group by
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
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result.ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Tuple<int, decimal> GetTongSoRoTongTrongLuong(DateTime dateTime, string nhanVienId)
        {
            try
            {
                var query =
                    "Select Count(*) as Item1,ISNULL(Sum(TrongLuong) ,0) As Item2 from PhieuCanBTPDinhHinh where ChiSanLuong = 0 and MaNhanVien = @nhanVienId and Ngay =@ngay and ISNULL(GhiChu,'') <> 'HUY'";
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
    PhieuCanBTPDinhHinh WITH(READPAST)
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
    PhieuCanBTPDinhHinh WITH(READPAST)
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
            PhieuCanBTPDinhHinh p,
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
                var items = connection.Query<T>(query, new { ngay = toDate.Date, fromDate = fromDate.Date }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetChiTiets_TG(DateTime fromDate, DateTime toDate)
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
            PhieuCanBTPDinhHinh p,
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
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", toDate.Date);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
                connection.Open();
                using var da = new SqlDataAdapter(cmd);
                var dataTable = new DataTable();
                da.Fill(dataTable);
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Insert<T>(List<T> items)
        {
            try
            {
                var query = @"INSERT INTO [dbo].[PhieuCanBTPDinhHinh]
           ([STT]
           ,[Ngay]
           ,[Gio]
           ,[MaUserCan]
           ,[MaMayCan]
           ,[MaLoaiCa]
           ,[MaMau]
           ,[MaSize]
           ,[MaThanhPham]
           ,[MaLo]
           ,[MaThe]
           ,[MaNhanVien]
           ,[MaMayLangDa]
           ,[TrongLuong]
           ,[IsEnabled]
           ,[MaXuong]
           ,[CaTra]
           ,[GhiChu],[ChiSanLuong],[TrongLuongTare],[TrongLuongBu],[IsOffline],[Id])
     VALUES
           (@STT 
           ,@Ngay 
           ,@Gio 
           ,@MaUserCan 
           ,@MaMayCan 
           ,@MaLoaiCa 
           ,@MaMau 
           ,@MaSize 
           ,@MaThanhPham 
           ,@MaLo 
           ,@MaThe 
           ,@MaNhanVien 
           ,@MaMayLangDa 
           ,@TrongLuong 
           ,@IsEnabled 
           ,@MaXuong 
           ,@CaTra 
           ,@GhiChu,@ChiSanLuong,@TrongLuongTare,@TrongLuongBu,@IsOffline,@Id)";
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



        public int Update<T>(List<T> items)
        {
            try
            {
                var query = @"UPDATE [dbo].[PhieuCanBTPDinhHinh]
   SET [Gio] = @Gio
      ,[MaUserCan] = @MaUserCan 
       
      ,[MaLoaiCa] = @MaLoaiCa 
      ,[MaMau] = @MaMau 
      ,[MaSize] = @MaSize 
      ,[MaThanhPham] = @MaThanhPham 
      ,[MaLo] = @MaLo 
      ,[MaThe] = @MaThe 
      ,[MaNhanVien] = @MaNhanVien 
      ,[MaMayLangDa] = @MaMayLangDa 
      ,[TrongLuong] = @TrongLuong 
      ,[IsEnabled] = @IsEnabled 
      
      ,[CaTra] = @CaTra 
      ,[GhiChu] = @GhiChu,ChiSanLuong=@ChiSanLuong,[TrongLuongBu] = @TrongLuongBu, [IsOffline] =@IsOffline
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


    }
}