using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanSauXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanSauXepKhuon";
        private readonly string qrDelete = "Delete PhieuCanSauXepKhuon Where [STT]=@STT and [NgayNguyenLieu]=@NgayNguyenLieu and [MaXuong]= @MaXuong and [MaMayCan]=@MaMayCan";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanSauXepKhuon]
           ([STT]
           ,[Ngay]
           ,[NgayNguyenLieu]
           ,[Gio]
           ,[MaXuong]
           ,[MaMayCan]
           ,[MaLo]
           ,[MaLoaiCa]
           ,[MaThanhPham]
           ,[MaSize]
           ,[MaChieuXa]
           ,[MaCoi]
           ,[TrongLuong]
           ,[TrongLuongTare]
           ,[ChiTietLuotRaCoiId]
           ,[MaNhanVien]
           ,[MaUserCan]
           ,[GhiChu]
           ,[MaThe])
     VALUES 
           (@STT
           ,@Ngay
           ,@NgayNguyenLieu
           ,@Gio
           ,@MaXuong
           ,@MaMayCan
           ,@MaLo
           ,@MaLoaiCa
           ,@MaThanhPham
           ,@MaSize
           ,@MaChieuXa
           ,@MaCoi
           ,@TrongLuong
           ,@TrongLuongTare
           ,@ChiTietLuotRaCoiId
           ,@MaNhanVien
           ,@MaUserCan
           ,@GhiChu
           ,@MaThe)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanSauXepKhuon]
     SET    
           [Ngay] = @Ngay   
           ,[Gio] = @Gio
           ,[MaLo] = @MaLo
           ,[MaLoaiCa] = @MaLoaiCa
           ,[MaThanhPham] = @MaThanhPham
           ,[MaSize] = @MaSize
           ,[MaChieuXa] = @MaChieuXa
           ,[MaCoi] = @MaCoi
           ,[TrongLuong] = @TrongLuong
           ,[TrongLuongTare] = @TrongLuongTare
           ,[ChiTietLuotRaCoiId] = @ChiTietLuotRaCoiId
           ,[MaNhanVien] = @MaNhanVien
           ,[MaUserCan] = @MaUserCan
           ,[GhiChu] = @GhiChu
            ,[MaThe] = @MaThe
     WHERE [STT] = @STT and [NgayNguyenLieu]=@NgayNguyenLieu and [MaXuong]= @MaXuong and [MaMayCan]=@MaMayCan";

        private readonly string qrGetAll = "Select * from PhieuCanSauXepKhuon";

        public PhieuCanSauXepKhuon(string? _connectionString = null)
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
        public int Delete<T>(List<T> items)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var rows = connection.Execute(qrDelete, items);
                    return rows; ;
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
        public List<T> GetChiTiet<T>(DateTime ngayNguyenLieu, string malo, string thanhPhamId, string sizeId,
            string chieuXaId, int tyLeMaBang, string xuongId)
        {
            var query = @"
SELECT p.STT,
    p.Ngay,
    p.NgayNguyenLieu,
    p.MaNhanVien,
	nv.Name as TenNhanVien,
    p.Gio,
    p.MaLo,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaChieuXa,
    cx.Ten as ChieuXaName,
    p.MaCoi,
    c.Ten as CoiName,
    'Ra' as LoaiCan,
    p.TrongLuong,
    p.TyLeMaBang,
    p.IsTam,
    p.MaMayCan,
    p.GhiChu,
 cast( cast(  p.TyLeMaBang as decimal(18,2)) /10 as decimal(18,2)) as TLNET
FROM PhieuCanSauXepKhuon p
    LEFT JOIN MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPham
    LEFT JOIN MaSizeChinhXepKhuon s on s.Ma = p.MaSize
    LEFT JOIN MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
    LEFT JOIN MaCoiXepKhuon c on c.Ma = p.MaCoi
    LEFT JOIN NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
where p.NgayNguyenLieu = @ngayNguyenLieu
    and p.MaXuong = @xuongId
    and p.STT > 0
    and p.MaLo = @malo
    and p.MaThanhPham = @thanhPhamId
    and p.MaSize = @sizeId
    and p.MaChieuXa = @chieuXaId
    and p.TyLeMaBang = @tyLeMaBang
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query,
                        new { ngayNguyenLieu = ngayNguyenLieu.Date, malo, thanhPhamId, sizeId, chieuXaId, tyLeMaBang, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetChiTiet<T>(DateTime ngayNguyenLieu, string malo, string thanhPhamId, string xuongId)
        {
            var query = @"
SELECT p.STT,
    p.Ngay,
    p.NgayNguyenLieu,
    p.MaNhanVien,
	nv.Name as TenNhanVien,
    p.Gio,
    p.MaLo,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaChieuXa,
    cx.Ten as ChieuXaName,
    p.MaCoi,
    c.Ten as CoiName,
    'Ra' as LoaiCan,
    p.TrongLuong,
    p.TyLeMaBang,
    p.IsTam,
    p.MaMayCan,
    p.GhiChu,
 cast( cast(  p.TyLeMaBang as decimal(18,2)) /10 as decimal(18,2)) as TLNET
FROM PhieuCanSauXepKhuon p
    LEFT JOIN MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPham
    LEFT JOIN MaSizeChinhXepKhuon s on s.Ma = p.MaSize
    LEFT JOIN MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
    LEFT JOIN MaCoiXepKhuon c on c.Ma = p.MaCoi
    LEFT JOIN NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
where p.NgayNguyenLieu = @ngayNguyenLieu
    and p.MaXuong = @xuongId
    and p.STT > 0
    and p.MaLo = @malo
    and p.MaThanhPham = @thanhPhamId
   
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { ngayNguyenLieu = ngayNguyenLieu.Date, malo, thanhPhamId, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }


        public List<TEntity> GetChiTietCois<TEntity>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    @"  
                    Select
                        p.STT,
                        p.NgayNguyenLieu,
                        p.Ngay
                        p.Gio,
                        p.MaNhanVien,
                        nv.Name as NhanVienName,
                        p.MaCoi,
                        coi.Ten as CoiName,
                        p.MaXuong,
                        p.TrongLuong,     
                        p.MaLo,   
                        tp.Ten as ThanhPhamName,
                        s.Ten as SizeName,
                        cx.Ten as ChieuXaName,
					    p.MaMayCan,
					    p.TrongLuongTare,
					    p.NgayNguyenLieu,
                        lc.Ten as LoaiCaName,
                        ctrc.Luot as LuotRaCoi,
                        p.MaThe
                    from
                        PhieuCanSauXepKhuon p
                        left join MaLoaiCaXepKhuon la on p.MaLoaiCa = la.Ma
                        left join MaThanhPhamChinhXepKhuon tp on p.MaThanhPhamChinh = tp.Ma
                        left join MaSizeChinhXepKhuon s on p.MaSizeChinh = s.Ma
                        left join MaChieuXaXepKhuon cx on p.MaChieuXa = cx.Ma
                        left join MaLoaiCaXepKhuon lc on p.MaLoaiCa = lc.Ma
                        left join MaCoiXepKhuon coi on p.MaCoi = coi.Ma
                        left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
                        left join ChiTietRaCoi ctrc on p.ChiTietLuotRaCoiId = ctrc.Id
                    where
                        p.NgayNguyenLieu <= @ngay
                        and p.NgayNguyenLieu >= @fromDate
					    and p.MaXuong =@xuongId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection
                    .QueryAsync<TEntity>(query, new { fromDate = fromDate.Date, ngay = dateTime.Date, xuongId })
                    .Result
                    .ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> GetChiTietSauXepKhuon<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
        )
        {
            var query = @"
SELECT p.STT,
    p.Ngay,
    p.NgayNguyenLieu,
    p.MaNhanVien,
	nv.Name as TenNhanVien,
    p.Gio,
    p.MaLo,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaChieuXa,
    cx.Ten as ChieuXaName,
    p.MaCoi,
    c.Ten as CoiName,
    'Ra' as LoaiCan,
    p.TrongLuong,
    p.TyLeMaBang,
    p.MaMayCan,
    p.IsTam,
    p.GhiChu,
 cast( cast(  p.TyLeMaBang as decimal(18,2)) /10 as decimal(18,2)) as TLNET
FROM PhieuCanSauXepKhuon p
    LEFT JOIN MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPham
    LEFT JOIN MaSizeChinhXepKhuon s on s.Ma = p.MaSize
    LEFT JOIN MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
    LEFT JOIN MaCoiXepKhuon c on c.Ma = p.MaCoi
    LEFT JOIN NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
where p.NgayNguyenLieu <= @toDate
    and p.NgayNguyenLieu >= @fromDate
    and p.MaXuong = @xuongId
    and p.STT > 0
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate, toDate, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetChiTietSauXepKhuonNgaySX<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
        )
        {
            var query = @"
SELECT p.STT,
    p.Ngay,
    p.NgayNguyenLieu,
    p.MaNhanVien,
	nv.Name as TenNhanVien,
    p.Gio,
    p.MaLo,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaChieuXa,
    cx.Ten as ChieuXaName,
    p.MaCoi,
    c.Ten as CoiName,
    'Ra' as LoaiCan,
    p.TrongLuong,
    p.TyLeMaBang,
    p.MaMayCan,
    p.IsTam,
    p.GhiChu,
 cast( cast(  p.TyLeMaBang as decimal(18,2)) /10 as decimal(18,2)) as TLNET
FROM PhieuCanSauXepKhuon p
    LEFT JOIN MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPham
    LEFT JOIN MaSizeChinhXepKhuon s on s.Ma = p.MaSize
    LEFT JOIN MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
    LEFT JOIN MaCoiXepKhuon c on c.Ma = p.MaCoi
    LEFT JOIN NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
where p.Ngay <= @toDate
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.STT > 0
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate, toDate, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetChiTietTheoCoiSauXepKhuon<T>(
            DateTime fromDate,
            DateTime toDate,
            string maCoi,
            string xuongId
        )
        {
            var query = @"
SELECT p.STT,
    p.Ngay,
    p.NgayNguyenLieu,
    p.MaNhanVien,
	nv.Name as TenNhanVien,
    p.Gio,
    p.MaLo,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaChieuXa,
    cx.Ten as ChieuXaName,
    p.MaCoi,
    c.Ten as CoiName,
    'Ra' as LoaiCan,
    p.TrongLuong,
    p.TyLeMaBang,
    p.IsTam,
    p.MaMayCan,
    p.GhiChu,
cast( cast(  p.TyLeMaBang as decimal(18,2)) /10 as decimal(18,2)) as TLNET
FROM PhieuCanSauXepKhuon p
    LEFT JOIN MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPham
    LEFT JOIN MaSizeChinhXepKhuon s on s.Ma = p.MaSize
    LEFT JOIN MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
    LEFT JOIN MaCoiXepKhuon c on c.Ma = p.MaCoi
    LEFT JOIN NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
where p.NgayNguyenLieu <= @toDate
    and p.NgayNguyenLieu >= @fromDate
    and p.MaXuong = @xuongId
    and p.STT > 0
    and p.MaCoi = @maCoi
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate, toDate, maCoi, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }

        /// <summary>
        ///     Get STT lớn nhất sau xếp khuôn Ngày Nguyên Liệu
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="xuongId"></param>
        /// <param name="mayCanId"></param>
        /// <returns></returns>
        public int GetMaxSTT(DateTime dateTime, string xuongId, string mayCanId)
        {
            try
            {
                var query =
                    "Select abs( ISNULL( Max(STT) ,0)) from PhieuCanSauXepKhuon Where NgayNguyenLieu=@ngay and MaXuong = @xuongId and MaMayCan =@mayCanId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.ExecuteScalar<int>(
                        query,
                        new { ngay = dateTime.Date, xuongId, mayCanId });
                    ;
                    return item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        public List<TEntity> GetPhieuCanTongHopNhanViens<TEntity>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.MaXuong, 
p.MaNhanVien,
n.MaHoSo,
n.Name as TenNhanVien,
cx.Ten as ChieuXaName,
p.MaLo,
la.Ten As LoaiCaName,
tp.Ten as ThanhPhamName,
s.Ten As SizeName,
ma.Ten As MauName,
SUM(p.TrongLuong) as TrongLuong ,
Count(*) As SoRo 
from  
PhieuCanSauXepKhuon p,
MaLoaiCaXepKhuon la,
MaSizeChinhXepKhuon s,
MaThanhPhamChinhXepKhuon tp,
MaMauXepKhuon ma,
NhanVienDaiThanh n,
MaChieuXaXepKhuon cx
where 
p.Ngay =@ngay
and p.MaXuong = @xuongId
and p.MaLoaiCa = la.Ma 
and p.MaSizeChinh = s.Ma 
and p.MaThanhPhamChinh = tp.Ma 
and p.MaMau = ma.Ma 
and p.MaNhanVien= n.MaNhanVien 
and p.MaChieuXa = cx.Ma 
group by p.MaNhanVien,
n.MaHoSo,n.Name ,
p.MaLo,tp.Ten ,
s.Ten ,
ma.Ten,
la.Ten,
cx.Ten,
p.MaXuong order by n.MaHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { ngay = dateTime.Date, xuongId })
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

        public List<TEntity> GetPhieuCanTongHopNhanViens<TEntity>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.MaXuong, 
p.MaNhanVien,
n.MaHoSo,
n.Name as TenNhanVien,
cx.Ten as ChieuXaName,
p.MaLo,
la.Ten As LoaiCaName,
tp.Ten as ThanhPhamName,
s.Ten As SizeName,
ma.Ten As MauName,
SUM(p.TrongLuong) as TrongLuong ,
Count(*) As SoRo 
from  
PhieuCanSauXepKhuon p,
MaLoaiCaXepKhuon la,
MaSizeChinhXepKhuon s,
MaThanhPhamChinhXepKhuon tp,
MaMauXepKhuon ma,
NhanVienDaiThanh n,
MaChieuXaXepKhuon cx
where 
p.NgayNguyenLieu >= @fromDate
p.NgayNguyenLieu <= @ngay
and p.MaXuong = @xuongId
and p.MaLoaiCa = la.Ma 
and p.MaSizeChinh = s.Ma 
and p.MaThanhPhamChinh = tp.Ma 
and p.MaMau = ma.Ma 
and p.MaNhanVien= n.MaNhanVien 
and p.MaChieuXa = cx.Ma 
group by p.MaNhanVien,
n.MaHoSo,n.Name ,
p.MaLo,tp.Ten ,
s.Ten ,
ma.Ten,
la.Ten,
cx.Ten,
p.MaXuong order by n.MaHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query,
                            new { ngay = dateTime.Date, fromDate = fromDate.Date, xuongId })
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

        public List<T> Gets<T>(DateTime dateTime, string xuongId, string mayCanId)
        {
            var query =
                @"Select * from PhieuCanSauXepKhuon Where NgayNguyenLieu = @ngay and MaXuong=@xuongId and MaMayCan= @mayCanId order by STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId, mayCanId })
                    .Result
                    .ToList();
                return items;
            }
        }

        //public List<T> GetsNgayNguyenLieu<T>(DateTime dateTime,string coiId, string xuongId, string mayCanId)
        //{
        //    var query =
        //        @"Select * from PhieuCanSauXepKhuon Where NgayNguyenLieu = @ngay and MaCoi = @coiId  and MaXuong=@xuongId and MaMayCan= @mayCanId order by Ngay desc, STT DESC";
        //    using (var connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        var items = connection.QueryAsync<T>(
        //                query,
        //                new { ngay = dateTime.Date, xuongId, mayCanId ,coiId })
        //            .Result
        //            .ToList();
        //        return items;
        //    }
        //}
        /// <summary>
        ///     Get dữ liệu phiếu cân theo 1 ngày Sau xếp khuôn
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public List<T> Gets<T>(DateTime dateTime)
        {
            try
            {
                var query = @"SELECT
    p.*,
    ISNULL(tp.Ten, 'NONE') as ThanhPhamName
from
    (
        Select
            *
        from
            PhieuCanSauXepKhuon p
        Where
            p.Ngay = @ngay
    ) p
    left join MaThanhPhamChinhXepKhuon tp on p.MaThanhPham = tp.Ma";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> Gets<T>(DateTime dateTime, string xuongId, string mayCanId, int stt)
        {
            var query =
                @"Select * from PhieuCanSauXepKhuon Where Ngay = @ngay and MaXuong=@xuongId and MaMayCan= @mayCanId and STT >@stt order by STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.Query<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId, mayCanId, stt })
                    .ToList();
                return items;
            }
        }

        /// <summary>
        ///     Get  Sản lượng thành phẩm từ giờ tới giờ Sau xếp khuôn
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <param name="xuongId"></param>
        /// <param name="thanhPhamIds"></param>
        /// <returns></returns>
        public decimal GetSanLuong(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> thanhPhamIds)
        {
            var listOfIdsJoined = $@"('{string.Join("','", thanhPhamIds.ToArray())}')";
            var query =
                $@"Select Isnull(sum(p.TrongLuong),0) 
                    from PhieuCanSauXepKhuon p 
                    where 
                    p.Ngay=@ngay 
                    and p.MaXuong = @xuongId 
                    and p.MaThanhPham in {listOfIdsJoined} 
                    and p.Gio >= @fromTime 
                    and p.Gio < @toTime";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var item = (decimal)connection.ExecuteScalar(
                    query,
                    new { ngay = dateTime.Date, fromTime, toTime, xuongId });
                return item;
            }
        }

        /// <summary>
        ///     Get sản lượng Thành phẩm chiếu xạ từ giờ tới giờ Sau Xếp Khuôn
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <param name="xuongId"></param>
        /// <param name="thanhPhamChieuXaIds"></param>
        /// <returns></returns>
        public decimal GetSanLuong(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<Tuple<string, string>> thanhPhamChieuXaIds)
        {
            var query =
                @"Select p.MaThanhPham as Item1
                ,p.MaChieuXa as Item2
                ,Sum(TrongLuong) as Item3 
                from PhieuCanSauXepKhuon p 
                where p.Ngay=@ngay 
                and p.MaXuong = @xuongId 
                and p.Gio >= @fromTime 
                and p.Gio < @toTime 
                group by 
                p.MaThanhPham
                ,p.MaChieuXa";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.Query<Tuple<string, string, decimal>>(
                        query,
                        new { ngay = dateTime.Date, fromTime, toTime, xuongId })
                    .ToList();
                if (items.Any())
                {
                    var num = (from i in items
                               from td in thanhPhamChieuXaIds
                               where i.Item1 == td.Item1 && i.Item2 == td.Item2
                               select i.Item3).DefaultIfEmpty(0)
                        .Sum();
                    return num;
                }

                return 0;
            }
        }

        public List<T> GetsNgayNguyenLieu<T>(DateTime dateTime, string xuongId, string coiId)
        {
            var query =
                @"Select * from PhieuCanSauXepKhuon Where NgayNguyenLieu = @ngay and MaXuong=@xuongId and MaCoi = @coiId order by Ngay desc, STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId, coiId })
                    .Result
                    .ToList();
                return items;
            }
        }

        public Tuple<int, decimal, int> GetSoRoTongTrongLuongByCoi(
            DateTime dateTime,
            string coiId)
        {
            var query = @"Select
    Count(*) as Item1,
    ISNULL(Sum(TrongLuong), 0) As Item2,
    ISNULL(Sum(TyLeMaBang),0) as Item3
from
    PhieuCanSauXepKhuon
where MaCoi = @coiId
    and Ngay = @ngay";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var row = connection.Query<Tuple<int, decimal, int>>(
                    query,
                    new { ngay = dateTime.Date, coiId })
                .SingleOrDefault();
            return row;
        }

        public Tuple<int, decimal, int> GetSoRoTongTrongLuongByCoiNgayNguyenLieu(
            DateTime dateTime,
            string coiId)
        {
            var query = @"Select
    Count(*) as Item1,
    ISNULL(Sum(TrongLuong), 0) As Item2,
    ISNULL(Sum(TyLeMaBang),0) as Item3
from
    PhieuCanSauXepKhuon
where MaCoi = @coiId
    and NgayNguyenLieu = @ngay";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var row = connection.Query<Tuple<int, decimal, int>>(
                    query,
                    new { ngay = dateTime.Date, coiId })
                .SingleOrDefault();
            return row;
        }

        public Tuple<int, decimal, int, decimal> GetSoRoTongTrongLuongByNhanVienId(
            DateTime dateTime,
            string nhanVienId,
            string thanhPhamId)
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
    PhieuCanSauXepKhuon
where MaNhanVien = @nhanVienId
    and Ngay = @ngay";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var row = connection.Query<Tuple<int, decimal, int, decimal>>(
                    query,
                    new { ngay = dateTime.Date, nhanVienId, thanhPhamId })
                .SingleOrDefault();
            return row;
        }

        public Tuple<int, decimal> GetSoRoTongTrongLuongByNhanVienId(DateTime dateTime, string nhanVienId)
        {
            var query =
                @"Select IsNull( Count(*),0) as Item1,isNull( Sum(TrongLuong),0) As Item2 from PhieuCanSauXepKhuon where MaNhanVien = @nhanVienId and Ngay =@ngay";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var row = connection.Query<Tuple<int, decimal>>(query, new { ngay = dateTime.Date, nhanVienId })
                .SingleOrDefault();
            return row;
        }

        public List<TEntity> GetTongHopCois<TEntity>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    @"
Select p.MaCoi,
    p.MaXuong,
    p.MaLo,
    la.Ten as LoaiCaName,
    tp.Ten as ThanhPhamName,
    cx.Ten as ChieuXaName,
    s.Ten as SizeName,
    c.Ten As ChatLuongName,
    Sum(p.TrongLuong) as TrongLuong,
    Count(*) as SoRo
from PhieuCanSauXepKhuon p,
    MaLoaiCaXepKhuon la,
    MaThanhPhamChinhXepKhuon tp,
    MaSizeChinhXepKhuon s,
    MaChatLuongXepKhuon c,
    MaChieuXaXepKhuon cx
where NgayNguyenLieu <= @ngay
    and NgayNguyenLieu >= @fromDate
    and MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.MaLoaiCa = la.Ma
    and p.MaSizeChinh = s.Ma
    and p.MaChieuXa = cx.Ma
Group by p.MaCoi,
    p.MaLo,
    la.Ten,
    tp.Ten,
    s.Ten,
    c.Ten,
    cx.Ten,
    p.ChuyenXuong,
    p.MaXuong,
order by p.MaLo,
    tp.Ten                    ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query,
                            new { fromDate = fromDate.Date, ngay = dateTime.Date, xuongId })
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

        public List<T> GetTongHopThanhPham2<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
        )
        {
            var query = @"select 
p.NgayNguyenLieu,
p.MaLo,
p.MaThanhPham,
p.ThanhPhamName,
SUM(p.TrongLuong) as TrongLuong,
SUM(p.TrongLuongNET) as TrongLuongNET
from (
select
p.NgayNguyenLieu,
p.MaLo,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
SUM(p.TrongLuong) as TrongLuong,
CAST((CAST(p.TyLeMaBang AS decimal(18,2))  /10) * count(*) AS decimal(18,2))as TrongLuongNET
from PhieuCanSauXepKhuon p
left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPham
where 
    p.NgayNguyenLieu <= @toDate
    and p.NgayNguyenLieu >= @fromDate
    and p.MaXuong = @xuongId
    and p.STT > 0
GROUP by 
p.NgayNguyenLieu,
p.MaLo,
p.MaThanhPham,
tp.Ten,
p.TyLeMaBang)p
GROUP by 
p.NgayNguyenLieu,
p.MaLo,
p.MaThanhPham,
p.ThanhPhamName
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate, toDate, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<TEntity> GetTongHopThanhPhams<TEntity>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    @"
                    Select 
                        ROW_NUMBER() OVER (ORDER BY p.Ngay) as STT,
                        p.Ngay,
                        p.MaXuong,
                        p.MaLo,
                        la.Ten As LoaiCaName,
                        cx.Ten as ChieuXaName,
                        tp.Ten as ThanhPhamName,
                        s.Ten As SizeName,
                        ma.Ten As MauName,
                        SUM(p.TrongLuong) as TrongLuong 
                    from  
                        PhieuCanSauXepKhuon p,
                        MaLoaiCaXepKhuon la,
                        MaSizeChinhXepKhuon s, 
                        MaThanhPhamChinhXepKhuon tp, 
                        MaChieuXaXepKhuon cx
                    where 
                        p.NgayNguyenLieu <= @ngay 
                        and p.NgayNguyenLieu >= @fromDate  
                        and p.MaXuong = @xuongId
                        and p.MaLoaiCa = la.Ma 
                        and p.MaSizeChinh = s.Ma 
                        and p.MaThanhPham = tp.Ma 
                        and p.MaChieuXa = cx.Ma 
                    group by 
                        p.Ngay,
                        p.MaLo,
                        tp.Ten ,
                        s.Ten ,
                        ma.Ten,
                        la.Ten,
                        cx.Ten,
                        p.MaXuong 
                    order by 
                        p.MaLo,
                        tp.ten,
                        s.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query,
                            new { fromDate = fromDate.Date, ngay = dateTime.Date, xuongId })
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

        public List<T> GetTongHopThanhPhamSauXepKhuon<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
        )
        {
            var query = @"select
p.NgayNguyenLieu,
p.MaLo,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.TyLeMaBang,
CAST((CAST(p.TyLeMaBang AS decimal(18,2))  /10) * count(*) AS decimal(18,2))as TrongLuongNET,
SUM(p.TrongLuong) as TrongLuong
from PhieuCanSauXepKhuon p
left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPham
left join MaSizeChinhXepKhuon s on s.Ma = p.MaSize
left join MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa 
where 
    p.NgayNguyenLieu <= @toDate
    and p.NgayNguyenLieu >= @fromDate
    and p.MaXuong = @xuongId
    and p.STT > 0
GROUP by 
p.NgayNguyenLieu,
p.MaLo,
p.MaThanhPham,
tp.Ten,
p.MaSize,
s.Ten,
p.MaChieuXa,
cx.Ten,
p.TyLeMaBang
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate, toDate, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetTyLeTangTrong_MaBang<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
        )
        {
            var query = @"
SELECT p.*,
    concat(
        Cast (
            (p.TrongLuongChenhLech / p.TrongLuongVao) * 100 as decimal(18, 2)
        ),
        '%'
    ) as TyLeSauXepKhuon
from (
        select ISNULL(xk.NgayNguyenLieu, sxk.NgayNguyenLieu) as NgayNguyenLieu,
            ISNULL(xk.MaCoi, sxk.MaCoi) as MaCoi,
            ISNULL(xk.CoiChinhName, sxk.CoiChinhName) as CoiChinhName,
            ISNULL(xk.MaLo, sxk.MaLo) as MaLo,
            ISNULL(xk.TrongLuongVao, 0) as TrongLuongVao,
            ISNULL(sxk.TrongLuongRa, 0) as TrongLuongRa,
            ISNULL(xk.ThanhPhamName, '') as ThanhPhamName,
            ISNULL(xk.ChieuXaName, '') as ChieuXaName,
            ISNULL(sxk.TrongLuongNET, 0) - ISNULL(xk.TrongLuongVao, 0) as TrongLuongChenhLech,
            ISNULL(sxk.TrongLuongNET, 0) as TrongLuongNET,
            ISNULL(sxk.SoLuong, 0) as SoLuong
        from (
                select p.NgayNguyenLieu,
                    p.MaCoiChinh as MaCoi,
                    c.Ten as CoiChinhName,
                    p.MaLo,
                    p.MaThanhPhamChinh as MaThanhPham,
                    tp.Ten as ThanhPhamName,
                    cx.Ten as ChieuXaName,
                    Sum(p.TrongLuong) as TrongLuongVao
                from PhieuCanChinhXepKhuon p
                    left join MaCoiXepKhuon c on c.Ma = p.MaCoiChinh
                    left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPhamChinh
                    LEFT JOIN MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
                where p.NgayNguyenLieu <= @toDate
                    and p.NgayNguyenLieu >= @fromDate
                    and p.MaXuong = @xuongId
                    and p.STT > 0
                    and IsNull(p.MaCoiChinh, '') != ''
                GROUP BY p.NgayNguyenLieu,
                    p.MaCoiChinh,
                    c.Ten,
                    p.MaLo,
                    p.MaThanhPhamChinh,
                    tp.Ten,
                    cx.Ten
            ) xk
            LEFT JOIN (
                select p.NgayNguyenLieu,
                    p.MaCoi as MaCoi,
                    c.Ten as CoiChinhName,
                    p.MaLo,
                    SUM(p.TrongLuong) as TrongLuongRa,
                    Cast(
                        CAST(Sum(p.TyLeMaBang) as decimal(18, 2)) / 10 as decimal(18, 2)
                    ) as TrongLuongNET,
                    COUNT(TyLeMaBang) as SoLuong
                from PhieuCanSauXepKhuon p
                    left join MaCoiXepKhuon c on c.Ma = p.MaCoi
                where p.NgayNguyenLieu <= @toDate
                    and p.NgayNguyenLieu >= @fromDate
                    and p.MaXuong = @xuongId
                    and p.STT > 0
                GROUP BY p.NgayNguyenLieu,
                    p.MaCoi,
                    c.Ten,
                    p.MaLo --p.TyLeMaBang
            ) sxk on sxk.NgayNguyenLieu = xk.NgayNguyenLieu
            and sxk.MaCoi = xk.MaCoi
            and sxk.MaLo = xk.MaLo
    ) p
ORDER by p.MaLo,
    p.MaCoi
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate, toDate, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetTyLeTangTrongSauXepKhuonSauXepKhuon<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
        )
        {
            var query =
                    //@"
                    // SELECT 
                    //p.*,
                    //    concat(Cast ((p.TrongLuongChenhLech / p.TrongLuongVao) * 100 as decimal(18,2)) ,'%') as TyLeSauXepKhuon
                    //from (
                    //        select ISNULL(xk.Ngay, sxk.Ngay) as Ngay,
                    //            ISNULL(xk.NgayNguyenLieu, sxk.NgayNguyenLieu) as NgayNguyenLieu,
                    //            ISNULL(xk.MaCoi, sxk.MaCoi) as MaCoi,
                    //            ISNULL(xk.CoiChinhName, sxk.CoiChinhName) as CoiChinhName,
                    //            ISNULL(xk.MaLo, sxk.MaLo) as MaLo,
                    //            ISNULL(xk.TrongLuongVao, 0) as TrongLuongVao,
                    //            ISNULL(sxk.TrongLuongRa, 0) as TrongLuongRa,
                    //            sxk.TrongLuongRa - xk.TrongLuongVao as TrongLuongChenhLech
                    //        from (
                    //                select p.Ngay,
                    //                    p.NgayNguyenLieu,
                    //                    p.MaCoiChinh as MaCoi,
                    //                    c.Ten as CoiChinhName,
                    //                    p.MaLo,
                    //                    Sum(p.TrongLuong) as TrongLuongVao
                    //                from PhieuCanChinhXepKhuon p
                    //                    left join MaCoiXepKhuon c on c.Ma = p.MaCoiChinh
                    //                where p.Ngay <= @toDate
                    //                    and p.Ngay >= @fromDate
                    //                    and p.MaXuong = @xuongId
                    //                    and p.STT > 0
                    //                    and IsNull(p.MaCoiChinh ,'') != ''
                    //                GROUP BY p.Ngay,
                    //                    p.NgayNguyenLieu,
                    //                    p.MaCoiChinh,
                    //                    c.Ten,
                    //                    p.MaLo
                    //            ) xk
                    //            LEFT JOIN (
                    //                select p.Ngay,
                    //                    p.NgayNguyenLieu,
                    //                    p.MaCoi as MaCoi,
                    //                    c.Ten as CoiChinhName,
                    //                    p.MaLo,
                    //                    SUM(p.TrongLuong) as TrongLuongRa
                    //                from PhieuCanSauXepKhuon p
                    //                    left join MaCoiXepKhuon c on c.Ma = p.MaCoi
                    //                where p.NgayNguyenLieu <= @toDate
                    //                    and p.NgayNguyenLieu >= @fromDate
                    //                    and p.MaXuong = @xuongId
                    //                    and p.STT > 0
                    //                GROUP BY p.Ngay,
                    //                    p.NgayNguyenLieu,
                    //                    p.MaCoi,
                    //                    c.Ten,
                    //                    p.MaLo
                    //            ) sxk on sxk.Ngay = xk.Ngay
                    //            and sxk.NgayNguyenLieu = xk.NgayNguyenLieu
                    //            and sxk.MaCoi = xk.MaCoi
                    //            and sxk.MaLo = xk.MaLo
                    //    ) p
                    @"SELECT 
p.*,
    concat(Cast ((p.TrongLuongChenhLech / p.TrongLuongVao) * 100 as decimal(18,2)) ,'%') as TyLeSauXepKhuon
from (
        select
            ISNULL(xk.NgayNguyenLieu, sxk.NgayNguyenLieu) as NgayNguyenLieu,
            ISNULL(xk.MaCoi, sxk.MaCoi) as MaCoi,
            ISNULL(xk.CoiChinhName, sxk.CoiChinhName) as CoiChinhName,
            ISNULL(xk.MaLo, sxk.MaLo) as MaLo,
            ISNULL(xk.TrongLuongVao, 0) as TrongLuongVao,
            ISNULL(sxk.TrongLuongRa, 0) as TrongLuongRa,
			ISNULL(xk.ThanhPhamName, '') as ThanhPhamName,
            sxk.TrongLuongRa - xk.TrongLuongVao as TrongLuongChenhLech
        from (
                select
                    p.NgayNguyenLieu,
                    p.MaCoiChinh as MaCoi,
                    c.Ten as CoiChinhName,
                    p.MaLo,
					p.MaThanhPhamChinh as MaThanhPham,
					tp.Ten as ThanhPhamName,
                    Sum(p.TrongLuong) as TrongLuongVao
                from PhieuCanChinhXepKhuon p
                    left join MaCoiXepKhuon c on c.Ma = p.MaCoiChinh
					left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPhamChinh
                where p.NgayNguyenLieu <= @toDate
                    and p.NgayNguyenLieu >= @fromDate
                    and p.MaXuong = @xuongId
                    and p.STT > 0
                    and IsNull(p.MaCoiChinh ,'') != ''
                GROUP BY 
                    p.NgayNguyenLieu,
                    p.MaCoiChinh,
                    c.Ten,
                    p.MaLo,
					p.MaThanhPhamChinh,
					tp.Ten
            ) xk
            LEFT JOIN (
                select
                    p.NgayNguyenLieu,
                    p.MaCoi as MaCoi,
                    c.Ten as CoiChinhName,
                    p.MaLo,
					
                    SUM(p.TrongLuong) as TrongLuongRa
                from PhieuCanSauXepKhuon p
                    left join MaCoiXepKhuon c on c.Ma = p.MaCoi
					
                where p.NgayNguyenLieu <= @toDate
                    and p.NgayNguyenLieu >= @fromDate
                    and p.MaXuong = @xuongId
                    and p.STT > 0
                GROUP BY 
                    p.NgayNguyenLieu,
                    p.MaCoi,
                    c.Ten,
                    p.MaLo
					
            ) sxk on  sxk.NgayNguyenLieu = xk.NgayNguyenLieu
            and sxk.MaCoi = xk.MaCoi
            and sxk.MaLo = xk.MaLo
			group by 
			xk.NgayNguyenLieu,
			sxk.NgayNguyenLieu,
			xk.MaCoi,
			sxk.MaCoi,
			xk.CoiChinhName,
			sxk.CoiChinhName,
			xk.MaLo,
			sxk.MaLo,
			xk.TrongLuongVao,
			sxk.TrongLuongRa,
			xk.ThanhPhamName

    ) p
ORDER by p.MaLo,
    p.MaCoi
"
                ;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate, toDate, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }


        /// <summary>
        ///     Update cùng lúc nhiều
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public int Update<T>(List<T> items)
        {
            var query = @"UPDATE [dbo].[PhieuCanSauXepKhuon]
     SET    
           [Ngay] = @Ngay   
           ,[Gio] = @Gio
           ,[MaLo] = @MaLo
           ,[MaLoaiCa] = @MaLoaiCa
           ,[MaThanhPham] = @MaThanhPham
           ,[MaSize] = @MaSize
           ,[MaChieuXa] = @MaChieuXa
           ,[MaCoi] = @MaCoi
           ,[TrongLuong] = @TrongLuong
           ,[TrongLuongTare] = @TrongLuongTare
           ,[ChiTietLuotRaCoiId] = @ChiTietLuotRaCoiId
           ,[MaNhanVien] = @MaNhanVien
           ,[MaUserCan] = @MaUserCan
           ,[GhiChu] = @GhiChu
            ,[MaThe] = @MaThe, [TyLeMaBang] = @TyLeMaBang,[IsTam] = @IsTam
     WHERE [STT] = @STT and [NgayNguyenLieu]=@NgayNguyenLieu and [MaXuong]= @MaXuong and [MaMayCan]=@MaMayCan";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, items);
            return rows;
        }

        //        public int UpdateDatabase()
        //        {
        //            try
        //            {
        //                //Th�m C?t Id V�o b?ng MaLoaiCa tr�n m�y c�n ??u Ao
        //                var query = @"IF (
        //    NOT EXISTS (
        //        SELECT
        //            *
        //        FROM
        //            INFORMATION_SCHEMA.TABLES
        //        WHERE
        //            TABLE_SCHEMA = 'dbo'
        //            AND TABLE_NAME = 'PhieuCanSauXepKhuon'
        //    )
        //) BEGIN 
        //   CREATE TABLE [dbo].[PhieuCanSauXepKhuon] (
        //    [STT]                INT             NOT NULL,
        //    [Ngay]               DATE            CONSTRAINT [DF_PhieuCanSauXepKhuon_Ngay] DEFAULT (getdate()) NOT NULL,
        //    [NgayNguyenLieu]     DATE            CONSTRAINT [DF_PhieuCanSauXepKhuon_NgayNguyenLieu] DEFAULT (getdate()) NOT NULL,
        //    [Gio]                TIME (7)        CONSTRAINT [DF_PhieuCanSauXepKhuon_Gio] DEFAULT (getdate()) NOT NULL,
        //    [MaXuong]            VARCHAR (50)    NOT NULL,
        //    [MaMayCan]           VARCHAR (50)    NOT NULL,
        //    [MaLo]               VARCHAR (50)    NOT NULL,
        //    [MaLoaiCa]           VARCHAR (50)    NOT NULL,
        //    [MaThanhPham]        VARCHAR (50)    NOT NULL,
        //    [MaSize]             VARCHAR (50)    NOT NULL,
        //    [MaChieuXa]          VARCHAR (50)    NOT NULL,
        //    [MaCoi]              VARCHAR (50)    NOT NULL,
        //    [TrongLuong]         DECIMAL (18, 3) CONSTRAINT [DF_PhieuCanSauXepKhuon_TrongLuong] DEFAULT ((0)) NOT NULL,
        //    [TrongLuongTare]     DECIMAL (18, 3) CONSTRAINT [DF_PhieuCanSauXepKhuon_TrongLuongTare] DEFAULT ((0)) NOT NULL,
        //    [ChiTietLuotRaCoiId] BIGINT             CONSTRAINT [DF_Table_1_LuotRa] DEFAULT ((0)) NOT NULL,
        //    [MaNhanVien]         VARCHAR (50)    NOT NULL,
        //    [MaUserCan]          VARCHAR (50)    NOT NULL,
        //    [GhiChu]             NVARCHAR (MAX)  NULL,
        //    [MaThe]              VARCHAR (50)    NOT NULL,
        //    CONSTRAINT [PK_PhieuCanSauXepKhuon] PRIMARY KEY CLUSTERED ([STT] ASC, [NgayNguyenLieu] ASC, [MaXuong] ASC, [MaMayCan] ASC)
        //);
        //END
        //DECLARE @tb varchar(30) = 'PhieuCanSauXepKhuon' 
        //IF COL_LENGTH(@tb, 'TyLeMaBang') IS NULL BEGIN
        //ALTER TABLE
        //    PhieuCanSauXepKhuon
        //ADD
        //TyLeMaBang INT NOT NULL DEFAULT ((0))
        //END
        //IF COL_LENGTH(@tb, 'IsTam') IS NULL BEGIN
        //ALTER TABLE
        //    PhieuCanSauXepKhuon
        //ADD
        //IsTam Bit NOT NULL DEFAULT ((0))
        //END

        //";
        //                var dao = new Database(connectionString);
        //                return dao.ExecuteNonQuery(query);
        //            }
        //            catch (Exception exception)
        //            {
        //                throw new Exception(
        //                    $@"Kh�ng th? c?p nh?t C? S? D? Li?u vui l�ng li�n h? PMS ?? ???c h? tr? [PhieuCanSauXepKhuon]. {Environment.NewLine}{exception.Message}");
        //                //throw;
        //            }
        //        }
        public int GetMaxSTTForNgayNguyenLieu(DateTime dateTime, string xuongId, string mayCanId)
        {
            try
            {
                var query =
                    "Select ISNULL( Max(STT) ,0) from PhieuCanSauXepKhuon Where NgayNguyenLieu=@ngay and MaXuong = @xuongId and MaMayCan =@mayCanId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.ExecuteScalar<int>(
                        query,
                        new { ngay = dateTime.Date, xuongId, mayCanId });
                    ;
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
p.NgayNguyenLieu,
p.Gio,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.MaCoi,
c.Ten as CoiName,
p.TrongLuong,
p.TrongLuongTare,
p.ChiTietLuotRaCoiId,
ctrc.Luot as LuotRaCoi,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
nv.DeptName0 as Nhom,
p.MaUserCan,
p.GhiChu
from PhieuCanSauXepKhuon p
left join XiNghiep x on p.MaXuong = x.Ma
left join MaLoaiCaXepKhuon lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamChinhXepKhuon tp on p.MaThanhPham = tp.Ma
left join MaSizeChinhXepKhuon s on p.MaSize = s.Ma
left join MaChieuXaXepKhuon cx on p.MaChieuXa = cx.Ma
left join MaCoiXepKhuon c on p.MaCoi = c.Ma
left join ChiTietRaCoi ctrc on p.ChiTietLuotRaCoiId = ctrc.Id
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
where p.NgayNguyenLieu = @dateTime and p.MaXuong = @xuongId
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
