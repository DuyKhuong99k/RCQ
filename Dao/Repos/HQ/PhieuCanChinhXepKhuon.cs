using Dapper;
using Microsoft.Data.SqlClient;
using Models.Repos.Models;
using System;
using System.Data;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanChinhXepKhuon
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanChinhXepKhuon";
        private readonly string qrDelete = @"Delete PhieuCanChinhXepKhuon Where [STT]=@STT and [NgayNguyenLieu]=@NgayNguyenLieu and [MaXuong]= @MaXuong and [MaMayCan]=@MaMayCan";

        private readonly string qrInsert = @"
Insert Into PhieuCanChinhXepKhuon ([STT],[Ngay],[Gio],[MaCoiTam],[MaCoiChinh],[DaQuay],[ThoiGianBatDauQuay],[ThoiGianQuay],[TrongLuong],[MaLo],[MaLoaiCa],[MaSizeChinh],[MaMau],[MaThanhPhamChinh],[MaChatLuong],[MaKhuVuc],[MaNhanVien],[MaNhom],[MaUserCan],[MaXuong],[MaMayCan],[GhiChu],[ThoiGianRaCoi],[Forced],[MaChieuXa],[TaiChe],[ChuyenXuong],[MaNhanVienPvPhanCo],[TrongLuongTare],[NgayNguyenLieu],[NgayRaCoi],[NgayBatDauQuay],[MayQuay],[IdMonitor],[LuotQuay], [Id]) Values (@STT,@Ngay,@Gio,@MaCoiTam,@MaCoiChinh,@DaQuay,@ThoiGianBatDauQuay,@ThoiGianQuay,@TrongLuong,@MaLo,@MaLoaiCa,@MaSizeChinh,@MaMau,@MaThanhPhamChinh,@MaChatLuong,@MaKhuVuc,@MaNhanVien,@MaNhom,@MaUserCan,@MaXuong,@MaMayCan,@GhiChu,@ThoiGianRaCoi,@Forced,@MaChieuXa,@TaiChe,@ChuyenXuong,@MaNhanVienPvPhanCo,@TrongLuongTare,@NgayNguyenLieu,@NgayRaCoi,@NgayBatDauQuay,@MayQuay,@IdMonitor,@LuotQuay,@Id)";

        private readonly string qrUpdate = @"UPDATE[dbo].[PhieuCanChinhXepKhuon]
                SET[Gio] = @Gio 
      ,[MaCoiTam] = @MaCoiTam 
      ,[MaCoiChinh] =@MaCoiChinh 
      ,[DaQuay] = @DaQuay 
      ,[ThoiGianBatDauQuay] = @ThoiGianBatDauQuay 
      ,[ThoiGianRaCoi] = @ThoiGianRaCoi 
      ,[Forced] = @Forced 
      ,[ThoiGianQuay] = @ThoiGianQuay 
      ,[TrongLuong] = @TrongLuong 
      ,[MaLo] = @MaLo 
      ,[MaLoaiCa] = @MaLoaiCa 
      ,[MaSizeChinh] = @MaSizeChinh 
      ,[MaMau] = @MaMau 
      ,[MaChatLuong] = @MaChatLuong 
      ,[MaThanhPhamChinh] = @MaThanhPhamChinh 
      ,[MaKhuVuc] = @MaKhuVuc 
      ,[MaNhanVien] = @MaNhanVien
      ,[MaNhom] = @MaNhom 
      ,[MaChieuXa] = @MaChieuXa 
      ,[TaiChe] = @TaiChe 
      ,[MaUserCan] = @MaUserCan 
      ,[GhiChu] = @GhiChu 
      ,[LuotQuay] = @LuotQuay 
      ,[ChuyenXuong] = @ChuyenXuong 
,[MaNhanVienPvPhanCo] = @MaNhanVienPvPhanCo,[TrongLuongTare] =@TrongLuongTare, [NgayRaCoi] =@NgayRaCoi, [NgayBatDauQuay] =@NgayBatDauQuay, [MayQuay] =@MayQuay, [IdMonitor] =@IdMonitor
  WHERE [STT] = @STT 
      And [NgayNguyenLieu]=@NgayNguyenLieu
      And [MaXuong] = @MaXuong 
      And [MaMayCan] = @MaMayCan";

        private readonly string qrGetAll = "Select * from PhieuCanChinhXepKhuon";

        private readonly string qrGetLiteReport = @"Select tp.Ten as ThanhPham,
    s.Ten as Size,
    cx.Ten as ChieuXa,
    cl.Ten as ChatLuong,
    sum(p.TrongLuong) as TrongLuong
from PhieuCanChinhXepKhuon p
    LEFT JOIN MaThanhPhamChinhXepKhuon tp on p.MaThanhPhamChinh = tp.Ma
    LEFT JOIN MaSizeChinhXepKhuon s on p.MaSizeChinh = s.Ma
    LEFT JOIN MaChieuXaXepKhuon cx on p.MaChieuXa = cx.Ma
    LEFT JOIN MaChatLuongXepKhuon cl on p.MaChatLuong = cl.Ma
where IdMonitor = @idMonitor
GROUP BY tp.Ten,
    s.Ten,
    cx.Ten,
    cl.Ten";
        private readonly string qrUpdateChatLuongByIdMonirtor = @"Update PhieuCanChinhXepKhuon set MaChatLuong = @maChatLuong where IdMonitor = @idMonitor";
        private readonly string qrGetByIdMonitor = @"Select * from PhieuCanChinhXepKhuon where IdMonitor = @idMonitor";
        private readonly string qrSetQuayStateByIdMonitor = "Update PhieuCanChinhXepKhuon set DaQuay = @daQuay,[ThoiGianBatDauQuay] = @thoiGianBatDauQuay,[NgayBatDauQuay] =@ngayBatDauQuay, [MayQuay] =@MayQuay  where IdMonitor = @idMonitor";
        private readonly string qrSetRaCoiStateByIdMonitor = "Update PhieuCanChinhXepKhuon set [ThoiGianRaCoi] = @thoiGianRaCoi,[NgayRaCoi] =@ngayRaCoi, [Forced] =@forced  where IdMonitor = @idMonitor";
        private readonly string qrGetsLastByNumAndMayCan = @"WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MaMayCan ORDER BY Ngay DESC, Gio DESC) AS RowNum
    FROM PhieuCanChinhXepKhuon where Ngay =@ngay
)

SELECT *
FROM RankedPhieu
WHERE RowNum <= @num";

        private readonly string qrGetTongHopCoiTam = @"Select coi.MaCoi as MaCoiTam,
    coi.MaXuong,
    p.MaCoiChinh,
    p.MaLo,
    p.MaThanhPhamChinh,
    p.MaSizeChinh,
    p.MaChieuXa,
    p.MaChatLuong,
    p.ChuyenXuong,
    p.NgayNguyenLieu,
    p.TrongLuong
from(
        SELECT c.Ma as MaCoi,
            xn.Ma as MaXuong
        from (
                Select *
                from MaCoiXepKhuon
                where Tam = 1
            ) c
            LEFT join XiNghiep xn on 1 = 1
    ) coi
    LEFT JOIN (
        SELECT MaCoiTam,
            MaXuong,
            MaCoiChinh,
            MaLo,
            MaThanhPhamChinh,
            MaSizeChinh,
            MaChieuXa,
            MaChatLuong,
            ChuyenXuong,
            NgayNguyenLieu,
            Sum (TrongLuong) as TrongLuong
        from PhieuCanChinhXepKhuon p
        where Ngay = @ngay
            and NgayBatDauQuay is NULL and Id
        GROUP BY MaCoiTam,
            MaXuong,
            MaCoiChinh,
            MaLo,
            MaThanhPhamChinh,
            MaSizeChinh,
            MaChieuXa,
            MaChatLuong,
            ChuyenXuong,
            NgayNguyenLieu
    ) p on coi.MaCoi = p.MaCoiTam
    and p.MaXuong = coi.MaXuong
order BY coi.MaXuong,
    coi.MaCoi";

        private readonly string qrGetPhieuCanOnCoiTams = @"SELECT *
        from PhieuCanChinhXepKhuon p
        where Ngay = @ngay
            and NgayBatDauQuay is NULL and Id
        GROUP BY MaCoiTam,
            MaXuong,
            MaCoiChinh,
            MaLo,
            MaThanhPhamChinh,
            MaSizeChinh,
            MaChieuXa,
            MaChatLuong,
            ChuyenXuong,
            NgayNguyenLieu";

        public List<T> GetPhieuCanOnCoiTams<T>(DateTime dateTime)
        {
            var query = qrGetPhieuCanOnCoiTams;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date }).Result.ToList();
            return items;
        }
        public List<T> GetsTongHopCoiTam<T>(DateTime dataTime)
        {
            var query = qrGetTongHopCoiTam;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.QueryAsync<T>(query, new { ngay = dataTime.Date }).Result.ToList();
            return items;
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
        public PhieuCanChinhXepKhuon(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

        }
        public int SetRaCoiStateByIdMonitor(string idMonitor, TimeSpan thoiGianRaCoi, DateTime ngayRaCoi, bool forced)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrSetRaCoiStateByIdMonitor, new { idMonitor, thoiGianRaCoi, ngayRaCoi, forced });
            return rows;
        }
        public int SetQuayStateByIdMonitor(string idMonitor, bool daQuay, TimeSpan thoiGianBatDauQuay, DateTime ngayBatDauQuay, string mayQuay)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrSetQuayStateByIdMonitor, new { idMonitor, daQuay, thoiGianBatDauQuay, ngayBatDauQuay, mayQuay });
            return rows;
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

        public int GetMaxSTT(DateTime dateTime, string xuongId, string mayCanId)
        {
            try
            {
                var query =
                    "Select abs( ISNULL( Max(STT) ,0)) from PhieuCanChinhXepKhuon Where Ngay=@ngay and MaXuong = @xuongId and MaMayCan =@mayCanId";
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
        public List<T> GetsByIdMonitor<T>(string idMonitor)
        {
            try
            {
                var query = qrGetByIdMonitor;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.Query<T>(
                    query, new { idMonitor }).ToList();
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
        public List<T> GetLiteReport<T>(string idMonitor)
        {
            try
            {
                var query = qrGetLiteReport;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.Query<T>(
                    query, new { idMonitor }).ToList();
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
        public T? Get<T>(string id)
        {
            try
            {
                var query = "Select * from PhieuCanChinhXepKhuon Where Id=@id";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var item = connection.QueryFirstOrDefault<T>(query, new { id });
                    return item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetPhieuCanChinhXepKhuonCoTheQuay<T>(DateTime dateTime, string xuongId, string coiChinhId)
        {
            var query =
                "Select * from PhieuCanChinhXepKhuon where DaQuay = 0 and Ngay=@ngay  and MaCoiChinh = @coiChinhId and ((MaXuong <> @xuongId and ChuyenXuong =1) or (ChuyenXuong = 0 and MaXuong = @xuongId)) ";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId, coiChinhId })
                    .Result
                    .ToList();
                return items;
            }
        }

        /// <summary>
        ///     Ngay Nguyen Liệu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dateTime"></param>
        /// <param name="xuongId"></param>
        /// <param name="coiChinhId"></param>
        /// <returns></returns>
        public List<T> GetPhieuCanChinhXepKhuonCoTheQuay2<T>(DateTime dateTime, string xuongId, string coiChinhId)
        {
            var query =
                "Select * from PhieuCanChinhXepKhuon where DaQuay = 0 and NgayNguyenLieu=@ngay  and MaCoiChinh = @coiChinhId and ((MaXuong <> @xuongId and ChuyenXuong =1) or (ChuyenXuong = 0 and MaXuong = @xuongId)) ";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId, coiChinhId })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetPhieuCanChinhXepKhuons()
        {
            try
            {
                var query = "Select * from PhieuCanChinhXepKhuon";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(query).Result.ToList();
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetPhieuCanChinhXepKhuonsByCoiChinhIdChuaRaCoi(
            DateTime dateTime,
            string coiChinhId,
            string xuongId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanChinhXepKhuon where MaCoiChinh = @coiChinhId and DaQuay = 1 and Ngay=@ngay and ThoiGianRaCoi is null and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection
                        .QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(
                            query,
                            new { ngay = dateTime.Date, coiChinhId, xuongId })
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

        /// <summary>
        ///     Ngay Nguyen Lieu
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="coiChinhId"></param>
        /// <param name="xuongId"></param>
        /// <returns></returns>
        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetPhieuCanChinhXepKhuonsByCoiChinhIdChuaRaCoi2(
            DateTime dateTime,
            string coiChinhId,
            string xuongId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanChinhXepKhuon where MaCoiChinh = @coiChinhId and DaQuay = 1 and NgayNguyenLieu=@ngay and ThoiGianRaCoi is null and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection
                        .QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(
                            query,
                            new { ngay = dateTime.Date, coiChinhId, xuongId })
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
        /// <summary>
        ///     Ngay Nguyen Lieu
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="coiChinhId"></param>
        /// <param name="xuongId"></param>
        /// <returns></returns>
        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetPhieuCanChinhXepKhuonsByCoiChinhIdDaVaoCoi(
            DateTime dateTime,
            string coiChinhId,
            string xuongId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanChinhXepKhuon where MaCoiChinh = @coiChinhId  and NgayNguyenLieu=@ngay  and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection
                        .QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(
                            query,
                            new { ngay = dateTime.Date, coiChinhId, xuongId })
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
        public List<T> GetPhieuCanChinhXepKhuonsByDate<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanChinhXepKhuon Where Ngay=@ngay and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  order by Gio DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { ngay = dateTime.Date, xuongId })
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
        public List<T> GetPhieuCanChinhXepKhuonsByFromDateToDate<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
P.MaXuong,
P.MaMayCan,
P.NgayNguyenLieu,
p.Ngay,
p.Gio,
p.MaCoiTam,
ct.Ten as CoiTamName,
p.MaCoiChinh,
c.Ten as CoiChinhName,
p.DaQuay,
p.ThoiGianBatDauQuay,
p.ThoiGianRaCoi,
p.Forced as EpRaCoi,
p.ThoiGianQuay,
p.TrongLuong,
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaSizeChinh,
lc.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaThanhPhamChinh,
tp.Ten as ThanhPhamName,
p.MaKhuVuc,
kv.Ten as KhuVucName,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as NhomName,
nv.Name as NhanVienName,
p.MaNhom,
n.Ten as NhomXepKhuon,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.TaiChe,
tc.Ten as TaiCheName,
p.MaUserCan,
p.GhiChu,
p.LuotQuay,
p.ChuyenXuong,
p.MaNhanVienPvPhanCo,
nvpv.MaHoSo as MaHoSoPV,
nvpv.DeptName0 as NhomPvPhanCo,
nvpv.Name as NhanVienPhucVuPhanCoName,
p.TrongLuongTare,
p.NgayRaCoi,
p.NgayBatDauQuay,
p.MayQuay,
--p.IdMonitor,
p.MaXuong,
x.Ten as XuongName
from PhieuCanChinhXepKhuon  p
left join MaCoiXepKhuon ct on ct.Ma = p.MaCoiTam
left join MaCoiXepKhuon c on c.Ma = p.MaCoiChinh
left join MaLoaiCaXepKhuon lc on lc.Ma = p.MaLoaiCa
left join MaSizeChinhXepKhuon s on s.Ma = p.MaSizeChinh
left join MaMauXepKhuon m on m.Ma = p.MaMau
left join MaChatLuongXepKhuon cl on cl.Ma = p.MaChatLuong
left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPhamChinh
left join MaKhuVucXepKhuon kv on kv.Ma = p.MaKhuVuc
left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
left join MaNhomXepKhuon n on n.Ma = p.MaNhom
left join MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
left join MaChatLuongTaiChe tc on tc.Ma = p.TaiChe
left join NhanVienDaiThanh nvpv on nvpv.MaNhanVien = p.MaNhanVienPvPhanCo
left join XiNghiep x on x.Ma = p.MaXuong
Where 
p.NgayNguyenLieu<=@toDate 
and NgayNguyenLieu >=@fromDate 
and p.MaXuong = @xuongId
and ((p.MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and p.MaXuong <> @xuongId)) 
order by
Gio DESC";
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
        public List<T> GetPhieuCanChinhXepKhuonsByMaNhanVien<T>(DateTime fromDate, DateTime toDate,string maNhanVien, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
P.MaXuong,
P.MaMayCan,
P.NgayNguyenLieu,
p.Ngay,
p.Gio,
p.MaCoiTam,
ct.Ten as CoiTamName,
p.MaCoiChinh,
c.Ten as CoiChinhName,
p.DaQuay,
p.ThoiGianBatDauQuay,
p.ThoiGianRaCoi,
p.Forced as EpRaCoi,
p.ThoiGianQuay,
p.TrongLuong,
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaSizeChinh,
lc.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaThanhPhamChinh,
tp.Ten as ThanhPhamName,
p.MaKhuVuc,
kv.Ten as KhuVucName,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as NhomName,
nv.Name as NhanVienName,
p.MaNhom,
n.Ten as NhomXepKhuon,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.TaiChe,
tc.Ten as TaiCheName,
p.MaUserCan,
p.GhiChu,
p.LuotQuay,
p.ChuyenXuong,
p.MaNhanVienPvPhanCo,
nvpv.MaHoSo as MaHoSoPV,
nvpv.DeptName0 as NhomPvPhanCo,
nvpv.Name as NhanVienPhucVuPhanCoName,
p.TrongLuongTare,
p.NgayRaCoi,
p.NgayBatDauQuay,
p.MayQuay,
--p.IdMonitor,
p.MaXuong,
x.Ten as XuongName
from PhieuCanChinhXepKhuon  p
left join MaCoiXepKhuon ct on ct.Ma = p.MaCoiTam
left join MaCoiXepKhuon c on c.Ma = p.MaCoiChinh
left join MaLoaiCaXepKhuon lc on lc.Ma = p.MaLoaiCa
left join MaSizeChinhXepKhuon s on s.Ma = p.MaSizeChinh
left join MaMauDinhHinh m on m.Ma = p.MaMau
left join MaChatLuongXepKhuon cl on cl.Ma = p.MaChatLuong
left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPhamChinh
left join MaKhuVucXepKhuon kv on kv.Ma = p.MaKhuVuc
left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
left join MaNhomXepKhuon n on n.Ma = p.MaNhom
left join MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
left join MaChatLuongTaiChe tc on tc.Ma = p.TaiChe
left join NhanVienDaiThanh nvpv on nvpv.MaNhanVien = p.MaNhanVienPvPhanCo
left join XiNghiep x on x.Ma = p.MaXuong
Where 
p.Ngay<=@toDate 
and Ngay >=@fromDate 
and p.MaXuong = @xuongId
and p.MaNhanVien = @maNhanVien
and ((p.MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and p.MaXuong <> @xuongId)) 
order by
Gio DESC";
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
        public List<T> GetPhieuCanChinhXepKhuonsByMaHoSo<T>(DateTime fromDate, DateTime toDate,string maHoSo, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
P.MaXuong,
P.MaMayCan,
P.NgayNguyenLieu,
p.Ngay,
p.Gio,
p.MaCoiTam,
ct.Ten as CoiTamName,
p.MaCoiChinh,
c.Ten as CoiChinhName,
p.DaQuay,
p.ThoiGianBatDauQuay,
p.ThoiGianRaCoi,
p.Forced as EpRaCoi,
p.ThoiGianQuay,
p.TrongLuong,
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaSizeChinh,
lc.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaThanhPhamChinh,
tp.Ten as ThanhPhamName,
p.MaKhuVuc,
kv.Ten as KhuVucName,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as NhomName,
nv.Name as NhanVienName,
p.MaNhom,
n.Ten as NhomXepKhuon,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.TaiChe,
tc.Ten as TaiCheName,
p.MaUserCan,
p.GhiChu,
p.LuotQuay,
p.ChuyenXuong,
p.MaNhanVienPvPhanCo,
nvpv.MaHoSo as MaHoSoPV,
nvpv.DeptName0 as NhomPvPhanCo,
nvpv.Name as NhanVienPhucVuPhanCoName,
p.TrongLuongTare,
p.NgayRaCoi,
p.NgayBatDauQuay,
p.MayQuay,
--p.IdMonitor,
p.MaXuong,
x.Ten as XuongName
from PhieuCanChinhXepKhuon  p
left join MaCoiXepKhuon ct on ct.Ma = p.MaCoiTam
left join MaCoiXepKhuon c on c.Ma = p.MaCoiChinh
left join MaLoaiCaXepKhuon lc on lc.Ma = p.MaLoaiCa
left join MaSizeChinhXepKhuon s on s.Ma = p.MaSizeChinh
left join MaMauDinhHinh m on m.Ma = p.MaMau
left join MaChatLuongXepKhuon cl on cl.Ma = p.MaChatLuong
left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPhamChinh
left join MaKhuVucXepKhuon kv on kv.Ma = p.MaKhuVuc
left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
left join MaNhomXepKhuon n on n.Ma = p.MaNhom
left join MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
left join MaChatLuongTaiChe tc on tc.Ma = p.TaiChe
left join NhanVienDaiThanh nvpv on nvpv.MaNhanVien = p.MaNhanVienPvPhanCo
left join XiNghiep x on x.Ma = p.MaXuong
Where 
p.Ngay<=@toDate 
and Ngay >=@fromDate 
and p.MaXuong = @xuongId
and nv.MaHoSo = @maHoSo
and ((p.MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and p.MaXuong <> @xuongId)) 
order by
Gio DESC";
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
        public List<T> GetPhieuCanChinhXepKhuonsByMaThe<T>(DateTime fromDate, DateTime toDate,string maThe, string xuongId)
        {
            try
            {
                var query =
                    @"Select
p.STT,
P.MaXuong,
P.MaMayCan,
P.NgayNguyenLieu,
p.Ngay,
p.Gio,
p.MaCoiTam,
ct.Ten as CoiTamName,
p.MaCoiChinh,
c.Ten as CoiChinhName,
p.DaQuay,
p.ThoiGianBatDauQuay,
p.ThoiGianRaCoi,
p.Forced as EpRaCoi,
p.ThoiGianQuay,
p.TrongLuong,
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaSizeChinh,
lc.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaThanhPhamChinh,
tp.Ten as ThanhPhamName,
p.MaKhuVuc,
kv.Ten as KhuVucName,
p.MaNhanVien,
nv.MaHoSo,
nv.DeptName0 as NhomName,
nv.Name as NhanVienName,
p.MaNhom,
n.Ten as NhomXepKhuon,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.TaiChe,
tc.Ten as TaiCheName,
p.MaUserCan,
p.GhiChu,
p.LuotQuay,
p.ChuyenXuong,
p.MaNhanVienPvPhanCo,
nvpv.MaHoSo as MaHoSoPV,
nvpv.DeptName0 as NhomPvPhanCo,
nvpv.Name as NhanVienPhucVuPhanCoName,
p.TrongLuongTare,
p.NgayRaCoi,
p.NgayBatDauQuay,
p.MayQuay,
--p.IdMonitor,
p.MaXuong,
x.Ten as XuongName
from PhieuCanChinhXepKhuon  p
left join MaCoiXepKhuon ct on ct.Ma = p.MaCoiTam
left join MaCoiXepKhuon c on c.Ma = p.MaCoiChinh
left join MaLoaiCaXepKhuon lc on lc.Ma = p.MaLoaiCa
left join MaSizeChinhXepKhuon s on s.Ma = p.MaSizeChinh
left join MaMauDinhHinh m on m.Ma = p.MaMau
left join MaChatLuongXepKhuon cl on cl.Ma = p.MaChatLuong
left join MaThanhPhamChinhXepKhuon tp on tp.Ma = p.MaThanhPhamChinh
left join MaKhuVucXepKhuon kv on kv.Ma = p.MaKhuVuc
left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
left join MaNhomXepKhuon n on n.Ma = p.MaNhom
left join MaChieuXaXepKhuon cx on cx.Ma = p.MaChieuXa
left join MaChatLuongTaiChe tc on tc.Ma = p.TaiChe
left join NhanVienDaiThanh nvpv on nvpv.MaNhanVien = p.MaNhanVienPvPhanCo
left join XiNghiep x on x.Ma = p.MaXuong
left join TheTu t on t.MaNhanVien = p.MaNhanVien
Where 
p.Ngay<=@toDate 
and Ngay >=@fromDate 
and p.MaXuong = @xuongId
and t.MaTheTu = @maThe
and ((p.MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and p.MaXuong <> @xuongId)) 
order by
Gio DESC";
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
        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetPhieuCanChinhXepKhuonsByDate(
            DateTime dateTime,
            string xuongId,
            string coiChinhId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanChinhXepKhuon Where Ngay=@ngay and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) and MaCoiChinh = @coiChinhId and ThoiGianRaCoi is null and cast(DATEADD(mi, ThoiGianQuay, ThoiGianBatDauQuay) as time) <= cast(CURRENT_TIMESTAMP as time) order by Gio DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(
                            query,
                            new { ngay = dateTime.Date, xuongId, coiChinhId })
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

        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetPhieuCanChinhXepKhuonsByDate2(
            DateTime dateTime,
            string xuongId,
            string coiChinhId)
        {
            try
            {
                var query =
                    @"Select
    *
from
    PhieuCanChinhXepKhuon p
Where
    NgayNguyenLieu = @ngay
    and (
        (
            MaXuong = @xuongId
            and ChuyenXuong = 0
        )
        or (
            ChuyenXuong = 1
            and MaXuong <> @xuongId
        )
    )
    and MaCoiChinh = @coiChinhId
    and ThoiGianRaCoi is null
    and (
        DATEADD(
            MINUTE,
            p.ThoiGianQuay,
            (
                Cast(
                    isnull(p.NgayBatDauQuay, p.NgayNguyenLieu) as datetime
                ) + cast(p.ThoiGianBatDauQuay as datetime)
            )
        )
    ) <= cast(CURRENT_TIMESTAMP as datetime)
order by
    Gio DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(
                            query,
                            new { ngay = dateTime.Date, xuongId, coiChinhId })
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

        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetPhieuCanChinhXepKhuonsByDateMayCan(DateTime dateTime, string mayCan)
        {
            try
            {
                var query = "Select * from PhieuCanChinhXepKhuon Where Ngay=@ngay and MaMayCan=@mayCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection
                        .QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(query, new { ngay = dateTime.Date, mayCan })
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

        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetPhieuCanChinhXepKhuonsByDateMayCan2(DateTime dateTime, string mayCan)
        {
            try
            {
                var query = "Select * from PhieuCanChinhXepKhuon Where NgayNguyenLieu=@ngay and MaMayCan=@mayCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection
                        .QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(query, new { ngay = dateTime.Date, mayCan })
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

        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetPhieuCanChinhXepKhuonsChuaRaCoi(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanChinhXepKhuon where DaQuay = 1 and Ngay=@ngay and ThoiGianRaCoi is null and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection
                        .QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(query, new { ngay = dateTime.Date, xuongId })
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

        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetPhieuCanChinhXepKhuonsChuaRaCoiOrder(
            DateTime dateTime,
            string mayCan,
            string xuongId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanChinhXepKhuon where DaQuay = 1 and Ngay=@ngay and ThoiGianRaCoi is null and MaMayCan <> @mayCan and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection
                        .QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(
                            query,
                            new { ngay = dateTime.Date, mayCan, xuongId })
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

        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetPhieuCanChinhXepKhuonsChuaRaCoiOrder(
            DateTime dateTime,
            string mayCan,
            string xuongId,
            bool isChuyenXuong)
        {
            try
            {
                var query =
                    "Select * from PhieuCanChinhXepKhuon where DaQuay = 1 and Ngay=@ngay and ThoiGianRaCoi is null and MaMayCan <> @mayCan and ((MaXuong <> @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong = @xuongId)) ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection
                        .QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(
                            query,
                            new
                            {
                                ngay = dateTime.Date,
                                mayCan,
                                xuongId,
                                isChuyenXuong
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

        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetPhieuCanChinhXepKhuonsChuaRaCoiVaCho(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanChinhXepKhuon where  Ngay=@ngay and isnull(MaCoiChinh,'') <>''  and ThoiGianRaCoi is null and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection
                        .QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(query, new { ngay = dateTime.Date, xuongId })
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

        /// <summary>
        ///     Ngay Nguyen Lieu
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="xuongId"></param>
        /// <returns></returns>
        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetPhieuCanChinhXepKhuonsChuaRaCoiVaCho2(DateTime dateTime,
            string xuongId)
        {
            try
            {
                var query =
                    "Select * from PhieuCanChinhXepKhuon where  NgayNguyenLieu=@ngay and isnull(MaCoiChinh,'') <>''  and ThoiGianRaCoi is null and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection
                        .QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(query, new { ngay = dateTime.Date, xuongId })
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

        public List<TEntity> GetPhieuCanTongHopChiTietCois<TEntity>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    @"SELECT
    p.*,
    MAX(LanQuay) OVER (PARTITION by p.MaCoiChinh) as SoLanQuay
FROM
    (
        Select
            p.*,
            DENSE_RANK() over (
                partition by p.MaCoiChinh
                order by
                     p.ThoiGianRaCoi
            ) as [LanQuay]
        from
            (
                Select
                    p.MaCoiChinh,
                    p.ChuyenXuong,
                    p.MaXuong,
                    p.ThoiGianBatDauQuay,
                    p.Forced,
                    case
                        when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                        else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                    end as ThoiGianRaCoi,
                    p.ThoiGianQuay,
                    DATEDIFF(
                        minute,
                        MIN(p.ThoiGianBatDauQuay),
                        case
                            when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                            else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                        end
                    ) as ThoiGianQuayThucTe,
                    Sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MaLo,
                    la.Ten as LoaiCaName,
                    tp.Ten as ThanhPhamName,
                    s.Ten as SizeName,
                    ch.Ten as ChatLuongName,
                    mau.Ten as MauName,
                    cx.Ten as ChieuXaName
                from
                    PhieuCanChinhXepKhuon p,
                    MaLoaiCaXepKhuon la,
                    MaThanhPhamChinhXepKhuon tp,
                    MaSizeChinhXepKhuon s,
                    MaChatLuongXepKhuon ch,
                    MaMauXepKhuon mau,
                    MaChieuXaXepKhuon cx
                where
                    ngay = @ngay
                    and (
                        (
                            MaXuong = @xuongId
                            and ChuyenXuong = 0
                        )
                        or (
                            ChuyenXuong = 1
                            and MaXuong != @xuongId
                        )
                    )
                    and MaCoiChinh is not null
                    and DaQuay = 1
                    and p.MaLoaiCa = la.Ma
                    and p.MaThanhPhamChinh = tp.Ma
                    and p.MaSizeChinh = s.Ma
                    and p.MaChatLuong = ch.Ma
                    and p.MaMau = mau.Ma
                    and p.MaChieuXa = cx.Ma
                group by
                    MaCoiChinh,
                    ThoiGianBatDauQuay,
                    Forced,
                    ThoiGianRaCoi,
                    ThoiGianQuay,
                    p.ChuyenXuong,
                    p.MaXuong,
                    p.ThoiGianQuay,
                    p.MaLo,
                    la.Ten,
                    tp.Ten,
                    s.Ten,
                    ch.Ten,
                    mau.Ten,
                    cx.Ten
            ) p
    ) p
order by
    MaCoiChinh";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<TEntity>(query, new { ngay = dateTime.Date, xuongId })
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
        public List<TEntity> GetPhieuCanTongHopChiTietCoisFromDateToDate<TEntity>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"SELECT
    p.*,
    MAX(LanQuay) OVER (PARTITION by p.MaCoiChinh) as SoLanQuay
FROM
    (
        Select
            p.*,
            DENSE_RANK() over (
                partition by p.MaCoiChinh
                order by
                     p.ThoiGianRaCoi
            ) as [LanQuay]
        from
            (
                Select
                    p.MaCoiChinh,
                    p.ChuyenXuong,
                    p.MaXuong,
                    p.ThoiGianBatDauQuay,
                    p.Forced,
                    case
                        when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                        else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                    end as ThoiGianRaCoi,
                    p.ThoiGianQuay,
                    DATEDIFF(
                        minute,
                        MIN(p.ThoiGianBatDauQuay),
                        case
                            when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                            else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                        end
                    ) as ThoiGianQuayThucTe,
                    Sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MaLo,
                    la.Ten as LoaiCaName,
                    tp.Ten as ThanhPhamName,
                    s.Ten as SizeName,
                    ch.Ten as ChatLuongName,
                    mau.Ten as MauName,
                    cx.Ten as ChieuXaName,
                    c.Ten as CoiChinhName,
                from
                    PhieuCanChinhXepKhuon p,
                    MaLoaiCaXepKhuon la,
                    MaThanhPhamChinhXepKhuon tp,
                    MaSizeChinhXepKhuon s,
                    MaChatLuongXepKhuon ch,
                    MaMauXepKhuon mau,
                    MaChieuXaXepKhuon cx,
                    MaCoiXepKhuon c
                where
                    p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and (
                        (
                            MaXuong = @xuongId
                            and ChuyenXuong = 0
                        )
                        or (
                            ChuyenXuong = 1
                            and MaXuong != @xuongId
                        )
                    )
                    and MaCoiChinh is not null
                    and DaQuay = 1
                    and p.MaLoaiCa = la.Ma
                    and p.MaThanhPhamChinh = tp.Ma
                    and p.MaSizeChinh = s.Ma
                    and p.MaChatLuong = ch.Ma
                    and p.MaMau = mau.Ma
                    and p.MaChieuXa = cx.Ma
                    and p.MaCoiChinh = cx.Ma
                group by
                    MaCoiChinh,
                    ThoiGianBatDauQuay,
                    Forced,
                    ThoiGianRaCoi,
                    ThoiGianQuay,
                    p.ChuyenXuong,
                    p.MaXuong,
                    p.ThoiGianQuay,
                    p.MaLo,
                    la.Ten,
                    tp.Ten,
                    s.Ten,
                    ch.Ten,
                    mau.Ten,
                    cx.Ten,
                    c.Ten
            ) p
    ) p
order by
    MaCoiChinh";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<TEntity>(query, new { fromDate = fromDate.Date,toDate = toDate.Date, xuongId })
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

        public List<TEntity> GetPhieuCanTongHopCois<TEntity>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    @"Select p.MaXuong,p.ChuyenXuong, p.MaCoiChinh,p.MaLo,cx.Ten as ChieuXaName,p.TaiChe,la.Ten as LoaiCaName,tp.Ten as ThanhPhamName,s.Ten as SizeName,c.Ten As ChatLuongName,Sum(p.TrongLuong) as TrongLuong, Count(*) as SoRo from PhieuCanChinhXepKhuon p,MaLoaiCaXepKhuon la,MaThanhPhamChinhXepKhuon tp,MaSizeChinhXepKhuon s, MaChatLuongXepKhuon c,MaChieuXaXepKhuon cx where p.MaChieuXa = cx.Ma and Ngay=@ngay and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  and p.MaThanhPhamChinh = tp.Ma and p.MaLoaiCa = la.Ma and p.MaSizeChinh = s.Ma and p.MaChatLuong = c.Ma Group by p.MaCoiChinh,p.MaLo,la.Ten,tp.Ten,s.Ten,c.Ten,p.TaiChe,cx.Ten,p.MaXuong,p.ChuyenXuong  order by p.MaCoiChinh,p.MaLo,tp.Ten";
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
        public List<TEntity> GetPhieuCanTongHopCoisFromDateToDate<TEntity>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.MaXuong,
p.ChuyenXuong,
p.MaCoiChinh,
coi.Ten as CoiChinhName,
p.MaLo,cx.Ten as ChieuXaName,
p.TaiChe,la.Ten as LoaiCaName,
tp.Ten as ThanhPhamName,
s.Ten as SizeName,
c.Ten As ChatLuongName,
Sum(p.TrongLuong) as TrongLuong,
Count(*) as SoRo ,
p.MaMau,
m.Ten as MauName
from PhieuCanChinhXepKhuon p,
MaLoaiCaXepKhuon la,
MaThanhPhamChinhXepKhuon tp,
MaSizeChinhXepKhuon s,
MaChatLuongXepKhuon c,
MaChieuXaXepKhuon cx ,
MaCoiXepKhuon coi,
MaMauXepKhuon m
where 
p.MaChieuXa = cx.Ma 
and Ngay<=@toDate 
and Ngay>=@fromDate 
and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  
and p.MaThanhPhamChinh = tp.Ma 
and p.MaLoaiCa = la.Ma 
and p.MaSizeChinh = s.Ma 
and p.MaChatLuong = c.Ma 
and p.MaCoiChinh = coi.Ma
and p.MaMau = m.Ma
Group by
p.MaCoiChinh,
p.MaLo,
la.Ten,
tp.Ten,
s.Ten,
c.Ten,
p.TaiChe,
cx.Ten,
p.MaXuong,
p.ChuyenXuong  ,
coi.Ten,
p.MaMau,
m.Ten

order by
p.MaCoiChinh,
p.MaLo,
tp.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId })
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

        public List<TEntity> GetPhieuCanTongHopCois2<TEntity>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select p.ChuyenXuong,p.MaXuong, p.MaLo,la.Ten as LoaiCaName,tp.Ten as ThanhPhamName,cx.Ten as ChieuXaName,p.TaiChe,s.Ten as SizeName,c.Ten As ChatLuongName,Sum(p.TrongLuong) as TrongLuong, Count(*) as SoRo from PhieuCanChinhXepKhuon p,MaLoaiCaXepKhuon la,MaThanhPhamChinhXepKhuon tp,MaSizeChinhXepKhuon s, MaChatLuongXepKhuon c, MaChieuXaXepKhuon cx where  Ngay=@ngay and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  and p.MaThanhPhamChinh = tp.Ma and p.MaLoaiCa = la.Ma and p.MaSizeChinh = s.Ma and p.MaChatLuong = c.Ma and p.MaChieuXa = cx.Ma Group by p.MaLo,la.Ten,tp.Ten,s.Ten,c.Ten ,cx.Ten,p.ChuyenXuong,p.MaXuong,p.TaiChe order by p.MaLo,tp.Ten";
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
        public List<TEntity> GetPhieuCanTongHopCois2FromDateToDate<TEntity>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.ChuyenXuong,
p.MaXuong, 
p.MaLo,
p.MaCoiChinh,
coi.Ten as CoiChinhName,
la.Ten as LoaiCaName,
tp.Ten as ThanhPhamName,
cx.Ten as ChieuXaName,
m.Ten as MauName,
p.TaiChe,
s.Ten as SizeName,
c.Ten As ChatLuongName,
Sum(p.TrongLuong) as TrongLuong,
Count(*) as SoRo 
from 
PhieuCanChinhXepKhuon p
left join MaCoiXepKhuon coi on p.MaCoiChinh = coi.Ma
left join MaLoaiCaXepKhuon la on p.MaLoaiCa = la.Ma 
left join MaSizeChinhXepKhuon s on  p.MaSizeChinh = s.Ma 
left join MaThanhPhamChinhXepKhuon tp on p.MaThanhPhamChinh = tp.Ma 
left join MaMauXepKhuon m on p.MaMau = m.Ma
left join MaChieuXaXepKhuon cx on p.MaChieuXa = cx.Ma
left join MaChatLuongXepKhuon c on p.MaChatLuong = c.Ma
where  
NgayNguyenLieu<=@toDate and NgayNguyenLieu>=@fromDate 
and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  

Group by p.MaLo,
la.Ten,tp.Ten,s.Ten,c.Ten ,
cx.Ten,p.ChuyenXuong,p.MaXuong,
p.TaiChe ,m.Ten ,coi.Ten, p.MaCoiChinh
order by p.MaLo,tp.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId })
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

        public List<TEntity> GetPhieuCanTongHopCoisChuaQuay<TEntity>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select p.ChuyenXuong,p.MaXuong, p.MaCoiChinh,p.MaLo,la.Ten as LoaiCaName,cx.Ten as ChieuXaName,p.TaiChe,tp.Ten as ThanhPhamName,s.Ten as SizeName,Sum(p.TrongLuong) as TrongLuong, Count(*) as SoRo from PhieuCanChinhXepKhuon p,MaLoaiCaXepKhuon la,MaThanhPhamChinhXepKhuon tp,MaSizeChinhXepKhuon s , MaChieuXaXepKhuon cx where  Ngay=@ngay and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  and p.MaThanhPhamChinh = tp.Ma and p.MaLoaiCa = la.Ma and p.MaSizeChinh = s.Ma and p.MaChieuXa = cx.Ma And p.MaCoiChinh is Null  Group by p.MaCoiChinh,p.MaLo,la.Ten,tp.Ten,s.Ten,cx.Ten,p.ChuyenXuong,p.MaXuong,p.TaiChe order by p.MaCoiChinh,p.MaLo,tp.Ten";
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
        public List<TEntity> GetPhieuCanTongHopCoisChuaQuayFromDateToDate<TEntity>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"
Select p.ChuyenXuong,p.MaXuong, p.MaCoiChinh,p.MaLo,la.Ten as LoaiCaName,cx.Ten as ChieuXaName,p.TaiChe,tp.Ten as ThanhPhamName,s.Ten as SizeName,Sum(p.TrongLuong) as TrongLuong, Count(*) as SoRo from PhieuCanChinhXepKhuon p,MaLoaiCaXepKhuon la,MaThanhPhamChinhXepKhuon tp,MaSizeChinhXepKhuon s , MaChieuXaXepKhuon cx where  Ngay<=@toDate and Ngay>=@fromDate and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  and p.MaThanhPhamChinh = tp.Ma and p.MaLoaiCa = la.Ma and p.MaSizeChinh = s.Ma and p.MaChieuXa = cx.Ma And p.MaCoiChinh is Null  Group by p.MaCoiChinh,p.MaLo,la.Ten,tp.Ten,s.Ten,cx.Ten,p.ChuyenXuong,p.MaXuong,p.TaiChe order by p.MaCoiChinh,p.MaLo,tp.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId })
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
        public List<TEntity> GetPhieuCanTongHopNhanViens<TEntity>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select p.ChuyenXuong,p.MaXuong, p.MaNhanVien,n.MaHoSo,n.Name as TenNhanVien,cx.Ten as ChieuXaName,p.TaiChe,p.MaLo,la.Ten As LoaiCaName,tp.Ten as ThanhPhamName,s.Ten As SizeName,ma.Ten As MauName, SUM(p.TrongLuong) as TrongLuong ,Count(*) As SoRo from  PhieuCanChinhXepKhuon p, MaLoaiCaXepKhuon la,MaSizeChinhXepKhuon s, MaThanhPhamChinhXepKhuon tp, MaMauXepKhuon ma,NhanVienDaiThanh n, MaChieuXaXepKhuon cx where p.Ngay =@ngay and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  and p.MaLoaiCa = la.Ma and p.MaSizeChinh = s.Ma and p.MaThanhPhamChinh = tp.Ma and p.MaMau = ma.Ma and p.MaNhanVien= n.MaNhanVien and p.MaChieuXa = cx.Ma group by p.MaNhanVien,n.MaHoSo,n.Name ,p.MaLo,tp.Ten ,s.Ten ,ma.Ten,la.Ten,cx.Ten,p.TaiChe, p.ChuyenXuong,p.MaXuong order by n.MaHoSo";
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
        public List<TEntity> GetPhieuCanTongHopNhanVienFromdateToDates<TEntity>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.ChuyenXuong,
p.MaXuong,
p.MaNhanVien,
n.MaHoSo,
n.Name as TenNhanVien,
cx.Ten as ChieuXaName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.TaiChe,
p.MaLo,
la.Ten As LoaiCaName,
tp.Ten as ThanhPhamName,
s.Ten As SizeName,
ma.Ten As MauName,
SUM(p.TrongLuong) as TrongLuong ,
Count(*) As SoRo 
from  
PhieuCanChinhXepKhuon p,
MaLoaiCaXepKhuon la,
MaSizeChinhXepKhuon s,
MaThanhPhamChinhXepKhuon tp,
MaMauXepKhuon ma,
NhanVienDaiThanh n,
MaChieuXaXepKhuon cx ,
MaChatLuongXepKhuon cl
where 
p.Ngay <=@toDate
and p.Ngay>=@fromDate
and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) 
and p.MaLoaiCa = la.Ma 
and p.MaSizeChinh = s.Ma
and p.MaThanhPhamChinh = tp.Ma 
and p.MaMau = ma.Ma 
and p.MaNhanVien= n.MaNhanVien 
and p.MaChieuXa = cx.Ma 
and p.MaChatLuong = cl.Ma
group by 
p.MaNhanVien,
n.MaHoSo,
n.Name,
p.MaLo,
tp.Ten,
s.Ten ,
ma.Ten,
la.Ten,
cx.Ten,
p.TaiChe,
p.ChuyenXuong,
p.MaXuong ,
p.MaChatLuong,
cl.Ten
order by
n.MaHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId })
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
        public List<TEntity> GetPhieuCanTongHopNhanViens2<TEntity>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select p.ChuyenXuong,p.MaXuong, p.MaNhanVien,n.MaHoSo,n.Name as TenNhanVien,cx.Ten as ChieuXaName,p.TaiChe,la.Ten As LoaiCaName,tp.Ten as ThanhPhamName, SUM(p.TrongLuong) as TrongLuong,Count(*) As SoRo from  PhieuCanChinhXepKhuon p, MaLoaiCaXepKhuon la, MaThanhPhamChinhXepKhuon tp,NhanVienDaiThanh n,MaChieuXaXepKhuon cx where p.Ngay =@ngay and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  and p.MaLoaiCa = la.Ma and p.MaThanhPhamChinh = tp.Ma and p.MaNhanVien= n.MaNhanVien and p.MaChieuXa = cx.Ma group by p.MaNhanVien,n.MaHoSo,n.Name ,tp.Ten,la.Ten,cx.Ten,p.TaiChe,p.ChuyenXuong,p.MaXuong order by n.MaHoSo";
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
        public List<TEntity> GetPhieuCanTongHopNhanViens2FromDateToDate<TEntity>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                //                var query =
                //                    @"Select 
                //p.ChuyenXuong,
                //p.MaXuong,
                //p.MaNhanVien,
                //n.MaHoSo,
                //n.Name as TenNhanVien,
                //cx.Ten as ChieuXaName,
                //p.TaiChe,
                //la.Ten As LoaiCaName,
                //tp.Ten as ThanhPhamName,
                //SUM(p.TrongLuong) as TrongLuong,
                //Count(*) As SoRo ,
                //p.MaMau,
                //m.Ten as MauName
                //from  
                //PhieuCanChinhXepKhuon p,
                //MaLoaiCaXepKhuon la,
                //MaThanhPhamChinhXepKhuon tp,
                //NhanVienDaiThanh n,
                //MaChieuXaXepKhuon cx,
                //MaMauXepKhuon m
                //where 
                //p.Ngay <=@toDate 
                //and p.Ngay>=@fromDate 
                //and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) 
                //and p.MaLoaiCa = la.Ma 
                //and p.MaThanhPhamChinh = tp.Ma 
                //and p.MaNhanVien= n.MaNhanVien 
                //and p.MaChieuXa = cx.Ma
                //and p.MaMau = m.Ma
                //group by 
                //p.MaNhanVien,
                //n.MaHoSo,
                //n.Name ,
                //tp.Ten,
                //la.Ten,
                //cx.Ten,
                //p.TaiChe,
                //p.ChuyenXuong,
                //p.MaXuong ,
                //p.MaMau,
                //m.Ten
                //order by n.MaHoSo";
                var query =
                    @";WITH CheckInOutData AS (
    SELECT 
        c.MaChamCong,
        MIN(c.ThoiGian) AS ThoiGianVao,
        MAX(c.ThoiGian) AS ThoiGianRa
    FROM CheckInOut c 
    WHERE c.ThoiGian >= @fromDate AND c.ThoiGian <= @toDate
    GROUP BY c.MaChamCong
)
Select 
p.ChuyenXuong,
p.MaXuong,
p.MaNhanVien,
n.MaHoSo,
n.Name as TenNhanVien,
cx.Ten as ChieuXaName,
cl.Ten as ChatLuongName,
p.TaiChe,
la.Ten As LoaiCaName,
tp.Ten as ThanhPhamName,
SUM(p.TrongLuong) as TrongLuong,
Count(*) As SoRo ,
p.MaMau,
m.Ten as MauName,
ISNULL(c.ThoiGianVao, '1900-01-01') AS ThoiGianVao,
ISNULL(c.ThoiGianRa, '1900-01-01') AS ThoiGianRa,
DATEDIFF(hour, ISNULL(c.ThoiGianVao, '1900-01-01'), ISNULL(c.ThoiGianRa, '1900-01-01')) AS TongThoiGian
from  
PhieuCanChinhXepKhuon p
left join MaLoaiCaXepKhuon la on p.MaLoaiCa = la.Ma
left join MaThanhPhamChinhXepKhuon tp on p.MaThanhPhamChinh = tp.Ma
left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
left join MaChieuXaXepKhuon cx on p.MaChieuXa = cx.Ma
left join MaChatLuongXepKhuon cl on p.MaChatLuong = cl.Ma
left join MaMauXepKhuon m on p.MaMau = m.Ma
left join CheckInOutData c on n.MaChamCong = c.MaChamCong
where 
p.NgayNguyenLieu <=@toDate 
and p.NgayNguyenLieu>=@fromDate 
and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) 

group by 
p.MaNhanVien,
n.MaHoSo,
n.Name ,
tp.Ten,
la.Ten,
cx.Ten,
p.TaiChe,
p.ChuyenXuong,
p.MaXuong ,
p.MaMau,
m.Ten,
c.ThoiGianVao,
c.ThoiGianRa,
cl.Ten
order by n.MaHoSo";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { toDate = toDate.Date, fromDate = fromDate.Date, xuongId })
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
                    "Select p.ChuyenXuong,p.MaXuong, p.MaLo,la.Ten As LoaiCaName,cx.Ten as ChieuXaName,p.TaiChe,tp.Ten as ThanhPhamName,s.Ten As SizeName,ma.Ten As MauName, SUM(p.TrongLuong) as TrongLuong from  PhieuCanChinhXepKhuon p, MaLoaiCaXepKhuon la,MaSizeChinhXepKhuon s, MaThanhPhamChinhXepKhuon tp, MaMauXepKhuon ma,MaChieuXaXepKhuon cx where p.Ngay =@ngay  and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  and p.MaLoaiCa = la.Ma and p.MaSizeChinh = s.Ma and p.MaThanhPhamChinh = tp.Ma and p.MaMau = ma.Ma and p.MaChieuXa = cx.Ma group by p.MaLo,tp.Ten ,s.Ten ,ma.Ten,la.Ten,cx.Ten,p.TaiChe,p.ChuyenXuong,p.MaXuong order by p.MaLo,tp.ten,s.Ten";
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

        public List<TEntity> GetPhieuCanTongHops2FromDateToDate<TEntity>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var query =
                    @"Select 
p.ChuyenXuong,
p.MaXuong,
p.MaLo,
la.Ten As LoaiCaName,
cx.Ten as ChieuXaName,
cl.Ten as ChatLuongName,
p.TaiChe,
tp.Ten as ThanhPhamName,
s.Ten As SizeName,
ma.Ten As MauName,
SUM(p.TrongLuong) as TrongLuong
from  
PhieuCanChinhXepKhuon p
left join MaLoaiCaXepKhuon la on p.MaLoaiCa = la.Ma 
left join MaSizeChinhXepKhuon s on  p.MaSizeChinh = s.Ma 
left join MaThanhPhamChinhXepKhuon tp on p.MaThanhPhamChinh = tp.Ma 
left join MaMauXepKhuon ma on p.MaMau = ma.Ma
left join MaChieuXaXepKhuon cx on p.MaChieuXa = cx.Ma
left join MaChatLuongXepKhuon cl on p.MaChatLuong = cl.Ma
where 
p.NgayNguyenLieu <=@toDate 
and p.NgayNguyenLieu >= @fromDate  
and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) 
group by 
p.MaLo,tp.Ten ,
s.Ten ,
ma.Ten,
la.Ten,
cx.Ten,
p.TaiChe,
p.ChuyenXuong,
p.MaXuong ,cl.Ten
order by
p.MaLo,
tp.ten,
s.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId })
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
        public List<TEntity> GetTongHopThanhPhamByMaNhanViens<TEntity>(DateTime fromDate, DateTime toDate, string maNhanVien,string xuongId)
        {
            try
            {
                var query =
                    @"
Select
p.MaNhanVien,
n.MaHoSo,
n.Name as NhanVienName,
p.MaLo,
la.Ten As LoaiCaName,
cx.Ten as ChieuXaName,
p.TaiChe,
tp.Ten as ThanhPhamName,
s.Ten As SizeName,
ma.Ten As MauName,
SUM(p.TrongLuong) as TrongLuong

from  
PhieuCanChinhXepKhuon p, 
MaLoaiCaXepKhuon la,
MaSizeChinhXepKhuon s, 
MaThanhPhamChinhXepKhuon tp,
MaMauXepKhuon ma,
MaChieuXaXepKhuon cx ,
NhanVienDaiThanh n
where p.Ngay <=@toDate 
and p.Ngay >= @fromDate  
and p.MaXuong = @xuongId
and p.MaNhanVien = @maNhanVien
and p.MaNhanVien = n.MaNhanVien
and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) 
and p.MaLoaiCa = la.Ma 
and p.MaSizeChinh = s.Ma 
and p.MaThanhPhamChinh = tp.Ma 
and p.MaMau = ma.Ma 
and p.MaChieuXa = cx.Ma

group by p.MaLo,tp.Ten ,s.Ten ,ma.Ten,la.Ten,cx.Ten,p.TaiChe,p.MaNhanVien, n.MaHoSo, n.Name order by p.MaLo,tp.ten,s.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, maNhanVien ,xuongId=xuongId})
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
        public List<TEntity> GetTongHopThanhPhamByMaHoSos<TEntity>(DateTime fromDate, DateTime toDate, string maHoSo,string xuongId)
        {
            try
            {
                var query =
                    @"
Select
p.MaNhanVien,
n.MaHoSo,
n.Name as NhanVienName,
p.MaLo,
la.Ten As LoaiCaName,
cx.Ten as ChieuXaName,
p.TaiChe,
tp.Ten as ThanhPhamName,
s.Ten As SizeName,
ma.Ten As MauName,
SUM(p.TrongLuong) as TrongLuong

from  
PhieuCanChinhXepKhuon p, 
MaLoaiCaXepKhuon la,
MaSizeChinhXepKhuon s, 
MaThanhPhamChinhXepKhuon tp,
MaMauXepKhuon ma,
MaChieuXaXepKhuon cx ,
NhanVienDaiThanh n
where p.Ngay <=@toDate 
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId
and n.MaHoSo = @maHoSo
and p.MaNhanVien = n.MaNhanVien
and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) 
and p.MaLoaiCa = la.Ma 
and p.MaSizeChinh = s.Ma 
and p.MaThanhPhamChinh = tp.Ma 
and p.MaMau = ma.Ma 
and p.MaChieuXa = cx.Ma

group by p.MaLo,tp.Ten ,s.Ten ,ma.Ten,la.Ten,cx.Ten,p.TaiChe,p.MaNhanVien, n.MaHoSo, n.Name order by p.MaLo,tp.ten,s.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, maHoSo ,xuongId})
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
        public List<TEntity> GetTongHopThanhPhamByMaThes<TEntity>(DateTime fromDate, DateTime toDate, string maThe,string xuongId)
        {
            try
            {
                var query =
                    @"
Select
p.MaNhanVien,
n.MaHoSo,
n.Name as NhanVienName,
p.MaLo,
la.Ten As LoaiCaName,
cx.Ten as ChieuXaName,
p.TaiChe,
tp.Ten as ThanhPhamName,
s.Ten As SizeName,
ma.Ten As MauName,
SUM(p.TrongLuong) as TrongLuong

from  
PhieuCanChinhXepKhuon p, 
MaLoaiCaXepKhuon la,
MaSizeChinhXepKhuon s, 
MaThanhPhamChinhXepKhuon tp,
MaMauXepKhuon ma,
MaChieuXaXepKhuon cx ,
NhanVienDaiThanh n,TheTu t
where p.Ngay <=@toDate 
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId
and p.MaNhanVien = t.MaNhanVien
and t.MaTheTu = @maThe
and p.MaNhanVien = n.MaNhanVien
and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) 
and p.MaLoaiCa = la.Ma 
and p.MaSizeChinh = s.Ma 
and p.MaThanhPhamChinh = tp.Ma 
and p.MaMau = ma.Ma 
and p.MaChieuXa = cx.Ma

group by p.MaLo,tp.Ten ,s.Ten ,ma.Ten,la.Ten,cx.Ten,p.TaiChe,p.MaNhanVien, n.MaHoSo, n.Name order by p.MaLo,tp.ten,s.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<TEntity>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, maThe ,xuongId})
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
            PhieuCanChinhXepKhuon p
        Where
            p.Ngay = @ngay
    ) p
    left join MaThanhPhamChinhXepKhuon tp on p.MaThanhPhamChinh = tp.Ma";
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

        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> Gets(
            DateTime dateTime,
            string mayCan,
            List<string> coiChinhIds,
            bool daQuay)
        {
            var query =
                "Select * from PhieuCanChinhXepKhuon Where Ngay=@ngay and MaMayCan=@mayCan and DaQuay =@daQuay and MaCoiChinh in @coiChinhIds ";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(
                        query,
                        new { ngay = dateTime.Date, mayCan, daQuay, coiChinhIds })
                    .Result
                    .ToList();
                return items;
            }
        }


        public decimal GetSanLuong(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> thanhPhamIds)
        {
            var listOfIdsJoined = $@"('{string.Join("','", thanhPhamIds.ToArray())}')";
            var query =
                $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanChinhXepKhuon p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaThanhPhamChinh in {listOfIdsJoined} and p.Gio >= @fromTime and p.Gio < @toTime";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var item = (decimal)connection.ExecuteScalar(
                    query,
                    new { ngay = dateTime.Date, fromTime, toTime, xuongId });
                return item;
            }
        }

        public decimal GetSanLuong(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<Tuple<string, string>> thanhPhamChieuXaIds)
        {
            var query =
                @"Select p.MaThanhPhamChinh as Item1,p.MaChieuXa as Item2,Sum(TrongLuong) as Item3 from PhieuCanChinhXepKhuon p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.Gio >= @fromTime and p.Gio < @toTime group by p.MaThanhPhamChinh,p.MaChieuXa";
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

        public decimal GetSanLuong(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> thanhPhamIds,
            IEnumerable<string> nhanVienIds)
        {
            var listOfIdsJoined = $@"('{string.Join("','", thanhPhamIds.ToArray())}')";
            var listOfNhanVienIdsJoined = $@"('{string.Join("','", nhanVienIds.ToArray())}')";
            var query =
                $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanChinhXepKhuon p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaThanhPham in {listOfIdsJoined} and p.Gio >= @fromTime and p.Gio < @toTime and MaNhanVien In {nhanVienIds}";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var item = (decimal)connection.ExecuteScalar(
                    query,
                    new { ngay = dateTime.Date, fromTime, toTime, xuongId });
                return item;
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
            var listOfIdsJoined = $@"('{string.Join("','", thanhPhamIds.ToArray())}')";
            var query =
                $@"Select Isnull(sum(p.TrongLuong),0) from PhieuCanChinhXepKhuon p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaThanhPhamChinh in {listOfIdsJoined} and p.Gio >= @fromTime and p.Gio < @toTime and MaNhanVien =@nhanVienId";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var item = (decimal)connection.ExecuteScalar(
                    query,
                    new
                    {
                        ngay = dateTime.Date,
                        fromTime,
                        toTime,
                        xuongId,
                        nhanVienId
                    });
                return item;
            }
        }

        public decimal GetSanLuong(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<Tuple<string, string>> thanhPhamChieuXaIds,
            string nhanVienId)
        {
            var query =
                @"Select p.MaThanhPhamChinh as Item1,p.MaChieuXa as Item2,Sum(TrongLuong) as Item3 from PhieuCanChinhXepKhuon p where p.MaNhanVien = @nhanVienId and p.Ngay=@ngay and p.MaXuong = @xuongId and p.Gio >= @fromTime and p.Gio < @toTime group by p.MaThanhPhamChinh,p.MaChieuXa";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.Query<Tuple<string, string, decimal>>(
                        query,
                        new
                        {
                            ngay = dateTime.Date,
                            fromTime,
                            toTime,
                            xuongId,
                            nhanVienId
                        })
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

        public List<T> GetSanLuongTinhLuong<T>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> nhanVienIdsEx,
            TimeSpan mocThoiGian)
        {
            var listOfIdsJoined = $@"('{string.Join("','", nhanVienIdsEx.ToArray())}')";
            var query =
                $@"Select p.Ngay, CaId as CaLamViec,p.MaNhanVien,tp.BravoId as MaSanPham,tp.Ten as TenSanPham, Sum(p.TrongLuong) as TrongLuong,0 as _Status from 
(Select p.*, (case when p.Gio < @mocThoiGian then 'CT04' else 'CT05' end) as CaId   from PhieuCanChinhXepKhuon p where p.Ngay=@ngay and p.MaXuong = @xuongId ) p,MaThanhPhamChinhXepKhuon tp
where p.MaThanhPhamChinh = tp.Ma and tp.BravoId is not null and p.MaNhanVien not in {listOfIdsJoined}
group by p.CaId,p.Ngay,p.MaNhanVien,tp.BravoId,tp.Ten";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.Query<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId, mocThoiGian })
                    .ToList();
                return items;
            }
        }

        public List<T> GetSanLuongTinhLuongInIds<T>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> nhanVienIdsEx,
            TimeSpan mocThoiGian)
        {
            var listOfIdsJoined = $@"('{string.Join("','", nhanVienIdsEx.ToArray())}')";
            var query =
                $@"Select p.Ngay, CaId as CaLamViec,p.MaNhanVien,tp.BravoId as MaSanPham,tp.Ten as TenSanPham, Sum(p.TrongLuong) as TrongLuong,0 as _Status from 
(Select p.*, (case when p.Gio < @mocThoiGian then 'CT04' else 'CT05' end) as CaId   from PhieuCanChinhXepKhuon p where p.Ngay=@ngay and p.MaXuong = @xuongId ) p,MaThanhPhamChinhXepKhuon tp
where p.MaThanhPhamChinh = tp.Ma and tp.BravoId is not null and p.MaNhanVien in {listOfIdsJoined}
group by p.CaId,p.Ngay,p.MaNhanVien,tp.BravoId,tp.Ten";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.Query<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId, mocThoiGian })
                    .ToList();
                return items;
            }
        }

        public List<T> GetSanLuongTinhLuongInIds_PhucVuPhanCo<T>(
            DateTime dateTime,
            string xuongId,
            IEnumerable<string> nhanVienIds,
            TimeSpan mocThoiGian, string bravoId)
        {
            var listOfIdsJoined = $@"('{string.Join("','", nhanVienIds.ToArray())}')";
            var query =
                $@"Select p.Ngay, CaId as CaLamViec,p.MaNhanVienPvPhanCo,'{bravoId}' as MaSanPham,CONCAT(N'PV Phân Cở - ',tp.Ten) as TenSanPham, Sum(p.TrongLuong) as TrongLuong,0 as _Status from 
(Select p.*, (case when p.Gio < @mocThoiGian then 'CT04' else 'CT05' end) as CaId   from PhieuCanChinhXepKhuon p where p.Ngay=@ngay and p.MaXuong = @xuongId ) p,MaThanhPhamChinhXepKhuon tp
where p.MaThanhPhamChinh = tp.Ma and tp.BravoId is not null and p.MaNhanVienPvPhanCo in {listOfIdsJoined}
group by p.CaId,p.Ngay,p.MaNhanVien,tp.Ten";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.Query<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId, mocThoiGian })
                    .ToList();
                return items;
            }
        }

        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetsCoiChinhCoiTam(
            DateTime dateTime,
            string mayCan,
            List<string> coiChinhCoiTams,
            bool daQuay)
        {
            var query =
                "Select * from PhieuCanChinhXepKhuon Where Ngay=@ngay and MaMayCan=@mayCan and DaQuay =@daQuay and CONCAT(MaCoiChinh,MaCoiTam) in @coiChinhIds ";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(
                        query,
                        new
                        {
                            ngay = dateTime.Date,
                            mayCan,
                            daQuay,
                            coiChinhIds = coiChinhCoiTams
                        })
                    .Result
                    .ToList();
                return items;
            }
        }

        /// <summary>
        ///     Theo Ngày Nguyên Liệu
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="mayCan"></param>
        /// <param name="coiChinhCoiTams"></param>
        /// <param name="daQuay"></param>
        /// <returns></returns>
        public List<Models.Repos.Models.PhieuCanChinhXepKhuon> GetsCoiChinhCoiTam2(
            DateTime dateTime,
            string mayCan,
            List<string> coiChinhCoiTams,
            bool daQuay)
        {
            var query =
                "Select * from PhieuCanChinhXepKhuon Where NgayNguyenLieu=@ngay and MaMayCan=@mayCan and DaQuay =@daQuay and CONCAT(MaCoiChinh,MaCoiTam) in @coiChinhIds ";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<Models.Repos.Models.PhieuCanChinhXepKhuon>(
                        query,
                        new
                        {
                            ngay = dateTime.Date,
                            mayCan,
                            daQuay,
                            coiChinhIds = coiChinhCoiTams
                        })
                    .Result
                    .ToList();
                return items;
            }
        }

        public T GetSoLuongCoiTheoTrangThai<T>(DateTime dateTime)
        {
            var query = @"Select
    count(
        case
            when p.MaXuong = '1'
            and p.Trang_Thai = N'Đang Quay' then 1
        end
    ) as DangQuayX1,
    count(
        case
            when p.MaXuong = '2'
            and p.Trang_Thai = N'Đang Quay' then 1
        end
    ) as DangQuayX2,
    count(
        case
            when p.MaXuong = '1'
            and p.Trang_Thai = N'Đang Chờ' then 1
        end
    ) as DangChoX1,
    count(
        case
            when p.MaXuong = '2'
            and p.Trang_Thai = N'Đang Chờ' then 1
        end
    ) as DangChoX2,
    count(
        case
            when p.MaXuong = '1'
            and p.Trang_Thai = N''
            and ISNULL( p.SoGioRa,21) <= 20 then 1
        end
    ) as DangRaX1,
    count(
        case
            when p.MaXuong = '2'
            and p.Trang_Thai = N''
            and ISNULL( p.SoGioRa,21) <= 20 then 1
        end
    ) as DangRaX2,
    count(
        case
            when p.MaXuong = '1'
            and p.Trang_Thai = N''
            and ISNULL( p.SoGioRa,21) > 20 then 1
        end
    ) as DangTrongX1,
    count(
        case
            when p.MaXuong = '2'
            and p.Trang_Thai = N''
            and ISNULL( p.SoGioRa,21) > 20 then 1
        end
    ) as DangTrongX2,
    COUNT(*) as TongSoCoi
from
    (
        select
            c1.Ma as [MaCoi],
            ISNULL(c2.DangQuay, '') as [Trang_Thai],
            c1.MaXuong,
            dangRa.SoGioRa
        from
            (
                Select
                    Ma,
                    '1' as MaXuong
                from
                    MaCoiXepKhuon
                where
                    Tam = 0
                union
                Select
                    Ma,
                    '2' as MaXuong
                from
                    MaCoiXepKhuon
                where
                    Tam = 0
            ) c1
            left join (
                SELECT
                    p.MaCoiChinh,
                    p.DangQuay,
                    p.MaXuong
                from
                    (
                        Select
                            distinct MaCoiChinh,
                            Case
                                when DaQuay = 1 then N'Đang Quay'
                                else N'Đang Chờ'
                            end as DangQuay,
                            SUM(TrongLuong) as TrongLuong,
                            ThoiGianBatDauQuay,
                            ThoiGianQuay,
                            cast(
                                DATEADD(
                                    MINUTE,
                                    ThoiGianQuay,
                                    CAST(ThoiGianBatDauQuay as datetime)
                                ) as time
                            ) as ThoiGianRaCoi,
                            CASE
                                when ChuyenXuong = 0 then MaXuong
                                when ChuyenXuong = 1 then (
                                    case
                                        when MaXuong = '1' then '2'
                                        else '1'
                                    end
                                )
                            end as MaXuong,
                            ChuyenXuong
                        from
                            PhieuCanChinhXepKhuon
                        where
                            Ngay = @ngay
                            and MaCoiChinh is not null
                            and ThoiGianRaCoi is null
                        GROUP BY
                            MaCoiChinh,
                            DaQuay,
                            ThoiGianBatDauQuay,
                            ThoiGianQuay,
                            MaXuong,
                            ChuyenXuong
                    ) p
                GROUP BY
                    MaCoiChinh,
                    DangQuay,
                    ThoiGianBatDauQuay,
                    ThoiGianQuay,
                    ThoiGianRaCoi,
                    MaXuong
            ) c2 on c1.ma = c2.MaCoiChinh
            and c1.MaXuong = c2.MaXuong
            LEFT join (
                Select
                    p.MaCoiChinh,
                    Max(p.ThoiGianRaCoi) as ThoiGianRaCoi,
                    DATEDIFF(
                        minute,
                        Max(p.ThoiGianRaCoi),
                        cast(GETDATE() as time)
                    ) as SoGioRa,
                    p.MaXuong
                from
                    PhieuCanChinhXepKhuon p
                where
                    p.Ngay = @ngay
                    and p.ThoiGianRaCoi is not null
                    and p.MaCoiChinh is not null
                GROUP BY
                    p.MaCoiChinh,
                    p.MaXuong
            ) dangRa on c1.Ma = dangRa.MaCoiChinh
            and c1.MaXuong = dangRa.MaXuong
    ) p";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var item = connection.Query<T>(query, new { ngay = dateTime.Date }).SingleOrDefault();
                return item;
            }
        }

        public List<T> GetTongHopCoiGanRa<T>(DateTime dateTime)
        {
            var query = @"select
    c1.Ma as [CoiId],
    c2.ThoiGianBatDauQuay,
    c2.ThoiGianQuay,
    c2.ThoiGianRaCoi,
    c1.MaXuong,
    ISNULL(
        DATEDIFF(
            MINUTE,
            CAST(GETDATE() as time(7)),
            c2.ThoiGianRaCoi
        ),
        0
    ) as SoPhut,
    c2.DaQuay,
    c2.Forced,
    Max(p1.LuotRaCoi) as SoLuotRaCoi,
    Max(p1.GioRaCoi) as GioRaCoiGanNhat
from
    (
        Select
            Ma,
            '1' as MaXuong
        from
            MaCoiXepKhuon
        where
            Tam = 0
        union
        Select
            Ma,
            '2' as MaXuong
        from
            MaCoiXepKhuon
        where
            Tam = 0
    ) c1
    left join (
        select
            c1.Ma as [MaCoiChinh],
            c2.DaQuay,
            c2.TrongLuong,
            c2.ThoiGianBatDauQuay,
            c2.ThoiGianQuay,
            c2.ThoiGianRaCoi,
            c1.MaXuong,
            c2.Forced
        from
            (
                Select
                    Ma,
                    '1' as MaXuong
                from
                    MaCoiXepKhuon
                where
                    Tam = 0
                union
                Select
                    Ma,
                    '2' as MaXuong
                from
                    MaCoiXepKhuon
                where
                    Tam = 0
            ) c1
            left join (
                SELECT
                    p.MaCoiChinh,
                    p.DaQuay,
                    p.ThoiGianBatDauQuay,
                    p.ThoiGianQuay,
                    p.ThoiGianRaCoi,
                    p.MaXuong,
                    SUM(p.TrongLuong) as TrongLuong,
                    p.Forced
                from
                    (
                        Select
                            distinct MaCoiChinh,
                            DaQuay,
                            SUM(TrongLuong) as TrongLuong,
                            ThoiGianBatDauQuay,
                            ThoiGianQuay,
                            cast(
                                DATEADD(
                                    MINUTE,
                                    ThoiGianQuay,
                                    CAST(ThoiGianBatDauQuay as datetime)
                                ) as time
                            ) as ThoiGianRaCoi,
                            CASE
                                when ChuyenXuong = 0 then MaXuong
                                when ChuyenXuong = 1 then (
                                    case
                                        when MaXuong = '1' then '2'
                                        else '1'
                                    end
                                )
                            end as MaXuong,
                            ChuyenXuong,
                            Forced
                        from
                            PhieuCanChinhXepKhuon
                        where
                            Ngay = @ngay
                            and MaCoiChinh is not null
                            and ThoiGianRaCoi is null
                        GROUP BY
                            MaCoiChinh,
                            DaQuay,
                            ThoiGianBatDauQuay,
                            ThoiGianQuay,
                            MaXuong,
                            ChuyenXuong,
                            Forced
                    ) p
                GROUP BY
                    MaCoiChinh,
                    DaQuay,
                    ThoiGianBatDauQuay,
                    ThoiGianQuay,
                    ThoiGianRaCoi,
                    MaXuong,
                    Forced
            ) c2 on c1.ma = c2.MaCoiChinh
            and c1.MaXuong = c2.MaXuong
    ) c2 on c1.ma = c2.MaCoiChinh
    and c1.MaXuong = c2.MaXuong
    LEFT JOIN(
        Select
            p.*,
            ROW_NUMBER() over (
                partition by p.MaCoi
                order by
                    p.GioRaCoi
            ) as [LuotRaCoi]
        from
            (
                Select
                    p.MaCoiChinh as MaCoi,
                    p.ThoiGianBatDauQuay,
                    p.MaXuong,
                    p.Forced,
                    case
                        when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                        else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                    end as GioRaCoi,
                    p.Ngay,
                    Sum(p.TrongLuong) as TrongLuong
                from
                    PhieuCanChinhXepKhuon p
                where
                    ngay = @ngay
                    and (
                        (
                            MaXuong = '1'
                            and ChuyenXuong = 0
                        )
                        or (
                            ChuyenXuong = 1
                            and MaXuong <> '1'
                        )
                    )
                    and MaCoiChinh is not null
                group by
                    MaCoiChinh,
                    ThoiGianBatDauQuay,
                    Forced,
                    ThoiGianRaCoi,
                    ThoiGianQuay,
                    p.MaXuong,
                    p.Ngay
            ) p
        UNION
        all
        Select
            p.*,
            ROW_NUMBER() over (
                partition by p.MaCoi
                order by
                    p.GioRaCoi
            ) as [LuotRaCoi]
        from
            (
                Select
                    p.MaCoiChinh as MaCoi,
                    p.ThoiGianBatDauQuay,
                    p.MaXuong,
                    p.Forced,
                    case
                        when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                        else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                    end as GioRaCoi,
                    p.Ngay,
                    Sum(p.TrongLuong) as TrongLuong
                from
                    PhieuCanChinhXepKhuon p
                where
                    ngay = @ngay
                    and (
                        (
                            MaXuong = '2'
                            and ChuyenXuong = 0
                        )
                        or (
                            ChuyenXuong = 1
                            and MaXuong <> '2'
                        )
                    )
                    and MaCoiChinh is not null
                group by
                    MaCoiChinh,
                    ThoiGianBatDauQuay,
                    Forced,
                    ThoiGianRaCoi,
                    ThoiGianQuay,
                    p.MaXuong,
                    p.Ngay
            ) p
    ) p1 ON c1.Ma = p1.MaCoi
    and c1.MaXuong = p1.MaXuong
GROUP by
    c1.Ma,
    c2.ThoiGianBatDauQuay,
    c2.ThoiGianQuay,
    c2.ThoiGianRaCoi,
    c1.MaXuong,
    c2.DaQuay,
    c2.Forced
order by
    [MaXuong],
    [CoiId]";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
                return items;
            }
        }
        public List<T> GetTongHopCoiGanRaFromDateToDate<T>(DateTime fromDate,DateTime toDate)
        {
            var query = @"select
    c1.Ma as [CoiId],
    c2.ThoiGianBatDauQuay,
    c2.ThoiGianQuay,
    c2.ThoiGianRaCoi,
    c1.MaXuong,
    ISNULL(
        DATEDIFF(
            MINUTE,
            CAST(GETDATE() as time(7)),
            c2.ThoiGianRaCoi
        ),
        0
    ) as SoPhut,
    c2.DaQuay,
    c2.Forced,
    Max(p1.LuotRaCoi) as SoLuotRaCoi,
    Max(p1.GioRaCoi) as GioRaCoiGanNhat
from
    (
        Select
            Ma,
            '1' as MaXuong
        from
            MaCoiXepKhuon
        where
            Tam = 0
        union
        Select
            Ma,
            '2' as MaXuong
        from
            MaCoiXepKhuon
        where
            Tam = 0
    ) c1
    left join (
        select
            c1.Ma as [MaCoiChinh],
            c2.DaQuay,
            c2.TrongLuong,
            c2.ThoiGianBatDauQuay,
            c2.ThoiGianQuay,
            c2.ThoiGianRaCoi,
            c1.MaXuong,
            c2.Forced
        from
            (
                Select
                    Ma,
                    '1' as MaXuong
                from
                    MaCoiXepKhuon
                where
                    Tam = 0
                union
                Select
                    Ma,
                    '2' as MaXuong
                from
                    MaCoiXepKhuon
                where
                    Tam = 0
            ) c1
            left join (
                SELECT
                    p.MaCoiChinh,
                    p.DaQuay,
                    p.ThoiGianBatDauQuay,
                    p.ThoiGianQuay,
                    p.ThoiGianRaCoi,
                    p.MaXuong,
                    SUM(p.TrongLuong) as TrongLuong,
                    p.Forced
                from
                    (
                        Select
                            distinct MaCoiChinh,
                            DaQuay,
                            SUM(TrongLuong) as TrongLuong,
                            ThoiGianBatDauQuay,
                            ThoiGianQuay,
                            cast(
                                DATEADD(
                                    MINUTE,
                                    ThoiGianQuay,
                                    CAST(ThoiGianBatDauQuay as datetime)
                                ) as time
                            ) as ThoiGianRaCoi,
                            CASE
                                when ChuyenXuong = 0 then MaXuong
                                when ChuyenXuong = 1 then (
                                    case
                                        when MaXuong = '1' then '2'
                                        else '1'
                                    end
                                )
                            end as MaXuong,
                            ChuyenXuong,
                            Forced
                        from
                            PhieuCanChinhXepKhuon
                        where
                            Ngay <= @toDate
                            and Ngay >= @fromDate
                            and MaCoiChinh is not null
                            and ThoiGianRaCoi is null
                        GROUP BY
                            MaCoiChinh,
                            DaQuay,
                            ThoiGianBatDauQuay,
                            ThoiGianQuay,
                            MaXuong,
                            ChuyenXuong,
                            Forced
                    ) p
                GROUP BY
                    MaCoiChinh,
                    DaQuay,
                    ThoiGianBatDauQuay,
                    ThoiGianQuay,
                    ThoiGianRaCoi,
                    MaXuong,
                    Forced
            ) c2 on c1.ma = c2.MaCoiChinh
            and c1.MaXuong = c2.MaXuong
    ) c2 on c1.ma = c2.MaCoiChinh
    and c1.MaXuong = c2.MaXuong
    LEFT JOIN(
        Select
            p.*,
            ROW_NUMBER() over (
                partition by p.MaCoi
                order by
                    p.GioRaCoi
            ) as [LuotRaCoi]
        from
            (
                Select
                    p.MaCoiChinh as MaCoi,
                    p.ThoiGianBatDauQuay,
                    p.MaXuong,
                    p.Forced,
                    case
                        when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                        else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                    end as GioRaCoi,
                    p.Ngay,
                    Sum(p.TrongLuong) as TrongLuong
                from
                    PhieuCanChinhXepKhuon p
                where
                    ngay <= @toDate
                    and ngay >= @fromDate
                    and (
                        (
                            MaXuong = '1'
                            and ChuyenXuong = 0
                        )
                        or (
                            ChuyenXuong = 1
                            and MaXuong <> '1'
                        )
                    )
                    and MaCoiChinh is not null
                group by
                    MaCoiChinh,
                    ThoiGianBatDauQuay,
                    Forced,
                    ThoiGianRaCoi,
                    ThoiGianQuay,
                    p.MaXuong,
                    p.Ngay
            ) p
        UNION
        all
        Select
            p.*,
            ROW_NUMBER() over (
                partition by p.MaCoi
                order by
                    p.GioRaCoi
            ) as [LuotRaCoi]
        from
            (
                Select
                    p.MaCoiChinh as MaCoi,
                    p.ThoiGianBatDauQuay,
                    p.MaXuong,
                    p.Forced,
                    case
                        when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                        else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                    end as GioRaCoi,
                    p.Ngay,
                    Sum(p.TrongLuong) as TrongLuong
                from
                    PhieuCanChinhXepKhuon p
                where
                    ngay <= @toDate
                    and ngay >= @fromDate
                    and (
                        (
                            MaXuong = '2'
                            and ChuyenXuong = 0
                        )
                        or (
                            ChuyenXuong = 1
                            and MaXuong <> '2'
                        )
                    )
                    and MaCoiChinh is not null
                group by
                    MaCoiChinh,
                    ThoiGianBatDauQuay,
                    Forced,
                    ThoiGianRaCoi,
                    ThoiGianQuay,
                    p.MaXuong,
                    p.Ngay
            ) p
    ) p1 ON c1.Ma = p1.MaCoi
    and c1.MaXuong = p1.MaXuong
GROUP by
    c1.Ma,
    c2.ThoiGianBatDauQuay,
    c2.ThoiGianQuay,
    c2.ThoiGianRaCoi,
    c1.MaXuong,
    c2.DaQuay,
    c2.Forced
order by
    [MaXuong],
    [CoiId]";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date  }).ToList();
                return items;
            }
        }

        public List<T> GetTongHopCoisXuong<T>(DateTime dateTime)
        {
            var query = @" select
    c1.Ma as [MaCoi],
    ISNULL(c2.DangQuay, '') as [Trang_Thai],
    c2.TrongLuong,
    c2.ThoiGianBatDauQuay,
    c2.ThoiGianQuay,
    c2.ThoiGianRaCoi as [ThoiGianRaCoiDuKien],
    c1.MaXuong
from
    (
        Select
            Ma,
            '1' as MaXuong
        from
            MaCoiXepKhuon
        where
            Tam = 0
        union
        Select
            Ma,
            '2' as MaXuong
        from
            MaCoiXepKhuon
        where
            Tam = 0
    ) c1
    left join (
        SELECT
            p.MaCoiChinh,
            p.DangQuay,
            p.ThoiGianBatDauQuay,
            p.ThoiGianQuay,
            p.ThoiGianRaCoi,
            p.MaXuong,
            SUM(p.TrongLuong) as TrongLuong
        from
            (
                Select
                    distinct MaCoiChinh,
                    Case
                        when DaQuay = 1 then N'Đang Quay'
                        else N'Đang Chờ'
                    end as DangQuay,
                    SUM(TrongLuong) as TrongLuong,
                    ThoiGianBatDauQuay,
                    ThoiGianQuay,
                    cast(
                        DATEADD(
                            MINUTE,
                            ThoiGianQuay,
                            CAST(ThoiGianBatDauQuay as datetime)
                        ) as time
                    ) as ThoiGianRaCoi,
                    CASE
                        when ChuyenXuong = 0 then MaXuong
                        when ChuyenXuong = 1 then (
                            case
                                when MaXuong = '1' then '2'
                                else '1'
                            end
                        )
                    end as MaXuong,
                    ChuyenXuong
                from
                    PhieuCanChinhXepKhuon
                where
                    Ngay = @ngay
                    and MaCoiChinh is not null
                    and ThoiGianRaCoi is null
                GROUP BY
                    MaCoiChinh,
                    DaQuay,
                    ThoiGianBatDauQuay,
                    ThoiGianQuay,
                    MaXuong,
                    ChuyenXuong
            ) p
        GROUP BY
            MaCoiChinh,
            DangQuay,
            ThoiGianBatDauQuay,
            ThoiGianQuay,
            ThoiGianRaCoi,
            MaXuong
    ) c2 on c1.ma = c2.MaCoiChinh
    and c1.MaXuong = c2.MaXuong
order by
    [MaXuong],
    [MaCoi]";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
                return items;
            }
        }

        public List<T> GetTongHopCoisXuongFromDateToDate<T>(DateTime fromDate,DateTime toDate)
        {
            var query = @" select
    c1.Ma as [MaCoi],
	C1.Ten as CoiChinhName,
    ISNULL(c2.DangQuay, '') as [Trang_Thai],
    c2.TrongLuong,
    c2.ThoiGianBatDauQuay,
    c2.ThoiGianQuay,
    c2.ThoiGianRaCoi as [ThoiGianRaCoiDuKien],
    c1.MaXuong
	
	
from
    (
        Select
            Ma,
			Ten,
            '1' as MaXuong
        from
            MaCoiXepKhuon
        where
            Tam = 0
        union
        Select
            Ma,
			Ten,
            '2' as MaXuong
        from
            MaCoiXepKhuon
        where
            Tam = 0
    ) c1
    left join (
        SELECT
            p.MaCoiChinh,
            p.DangQuay,
            p.ThoiGianBatDauQuay,
            p.ThoiGianQuay,
            p.ThoiGianRaCoi,
            p.MaXuong,
            SUM(p.TrongLuong) as TrongLuong
        from
            (
                Select
                    distinct MaCoiChinh,
                    Case
                        when DaQuay = 1 then N'Đang Quay'
                        else N'Đang Chờ'
                    end as DangQuay,
                    SUM(TrongLuong) as TrongLuong,
                    ThoiGianBatDauQuay,
                    ThoiGianQuay,
                    cast(
                        DATEADD(
                            MINUTE,
                            ThoiGianQuay,
                            CAST(ThoiGianBatDauQuay as datetime)
                        ) as time
                    ) as ThoiGianRaCoi,
                    CASE
                        when ChuyenXuong = 0 then MaXuong
                        when ChuyenXuong = 1 then (
                            case
                                when MaXuong = '1' then '2'
                                else '1'
                            end
                        )
                    end as MaXuong,
                    ChuyenXuong
					--coi.Ten as CoiChinhName
                from
                    PhieuCanChinhXepKhuon
					--MaCoiXepKhuon coi

                where
                    Ngay <= @toDate
                    and Ngay >= @fromDate
                    and MaCoiChinh is not null
                    and ThoiGianRaCoi is null
					--and MaCoiChinh = coi.Ma
                GROUP BY
                    MaCoiChinh,
                    DaQuay,
                    ThoiGianBatDauQuay,
                    ThoiGianQuay,
                    MaXuong,
                    ChuyenXuong
					--coi.Ten
            ) p
        GROUP BY
            MaCoiChinh,
            DangQuay,
            ThoiGianBatDauQuay,
            ThoiGianQuay,
            ThoiGianRaCoi,
            MaXuong
    ) c2 on c1.ma = c2.MaCoiChinh
    and c1.MaXuong = c2.MaXuong
order by
    [MaXuong],
    [MaCoi]";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date,toDate = toDate.Date }).ToList();
                return items;
            }
        }

        public DataTable GetTongHopCoiXuong(DateTime dateTime)
        {
            var query = @" select
    c1.Ma as [MaCoi],
    ISNULL(c2.DangQuay, '') as [Trang_Thai],
    c2.TrongLuong,
    c2.ThoiGianBatDauQuay,
    c2.ThoiGianQuay,
    c2.ThoiGianRaCoi as [ThoiGianRaCoiDuKien],
    c1.MaXuong
from
    (
        Select
            Ma,
            '1' as MaXuong
        from
            MaCoiXepKhuon
        where
            Tam = 0
        union
        Select
            Ma,
            '2' as MaXuong
        from
            MaCoiXepKhuon
        where
            Tam = 0
    ) c1
    left join (
        SELECT
            p.MaCoiChinh,
            p.DangQuay,
            p.ThoiGianBatDauQuay,
            p.ThoiGianQuay,
            p.ThoiGianRaCoi,
            p.MaXuong,
            SUM(p.TrongLuong) as TrongLuong
        from
            (
                Select
                    distinct MaCoiChinh,
                    Case
                        when DaQuay = 1 then N'Đang Quay'
                        else N'Đang Chờ'
                    end as DangQuay,
                    SUM(TrongLuong) as TrongLuong,
                    ThoiGianBatDauQuay,
                    ThoiGianQuay,
                    cast(
                        DATEADD(
                            MINUTE,
                            ThoiGianQuay,
                            CAST(ThoiGianBatDauQuay as datetime)
                        ) as time
                    ) as ThoiGianRaCoi,
                    CASE
                        when ChuyenXuong = 0 then MaXuong
                        when ChuyenXuong = 1 then (
                            case
                                when MaXuong = '1' then '2'
                                else '1'
                            end
                        )
                    end as MaXuong,
                    ChuyenXuong
                from
                    PhieuCanChinhXepKhuon
                where
                    Ngay = @ngay
                    and MaCoiChinh is not null
                    and ThoiGianRaCoi is null
                GROUP BY
                    MaCoiChinh,
                    DaQuay,
                    ThoiGianBatDauQuay,
                    ThoiGianQuay,
                    MaXuong,
                    ChuyenXuong
            ) p
        GROUP BY
            MaCoiChinh,
            DangQuay,
            ThoiGianBatDauQuay,
            ThoiGianQuay,
            ThoiGianRaCoi,
            MaXuong
    ) c2 on c1.ma = c2.MaCoiChinh
    and c1.MaXuong = c2.MaXuong
order by
    [MaXuong],
    [MaCoi]";
            using (var connection = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ngay", dateTime.Date);

                    connection.Open();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dataTable = new DataTable();
                        da.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        public DataTable GetTongHopNguyenLieuDaVaoCoiXuong(DateTime dateTime)
        {
            var query = $@" declare @DynamicPivotQuery as nvarchar(Max)
 declare @ColumnName as nvarchar(Max) 
 declare @PivotSelectColumnNames AS NVARCHAR(MAX)
 select @ColumnName = ISNULL(@ColumnName + ',','') +QUOTENAME(MaXuong) from (select distinct  MaXuong from PhieuCanChinhXepKhuon where Ngay =@ngay) as MS order by MaXuong
 SELECT @PivotSelectColumnNames 
    = ISNULL(@PivotSelectColumnNames + ',','')
    + 'ISNULL(' + QUOTENAME(MaXuong) + ', 0) AS '
    +  QUOTENAME(N'Xưởng ' +MaXuong)
FROM (SELECT DISTINCT MaXuong FROM PhieuCanChinhXepKhuon where Ngay =@ngay) AS Courses order by MaXuong
 set @DynamicPivotQuery = N'Select  MaLo as [Lô],LoaiCaName as [Loại Cá],ThanhPhamName as [Thành Phẩm], ChieuXaName as [Chiếu Xạ],SizeName as [Size],MauName as [Màu],'+ @PivotSelectColumnNames  +'  from (Select   p.MaLo,la.Ten as LoaiCaName,s.Ten as SizeName,ma.Ten as MauName,tp.Ten as ThanhPhamName,cx.Ten as ChieuXaName,MaXuong ,TrongLuong  from PhieuCanChinhXepKhuon p,MaLoaiCaXepKhuon la,MaMauXepKhuon ma,MaSizeChinhXepKhuon s, MaThanhPhamChinhXepKhuon tp, MaChieuXaXepKhuon cx where p.MaCoiChinh is not null and p.DaQuay = 1 and p.Ngay = ''{dateTime.ToString("yyyy-MM-dd")}'' and p.MaLoaiCa = la.Ma and p.MaMau = ma.Ma and p.MaSizeChinh = s.Ma and p.MaThanhPhamChinh = tp.Ma and p.MaChieuXa = cx.Ma ) as tb pivot(sum ([TrongLuong]) for MaXuong in (' + @ColumnName + ')) as re order by MaLo,LoaiCaName,ThanhPhamName,ChieuXaName,SizeName,MauName, '+ @ColumnName
 EXEC sp_executesql @DynamicPivotQuery";
            using (var connection = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ngay", dateTime.Date);

                    connection.Open();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dataTable = new DataTable();
                        da.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        public DataTable GetTongHopNguyenLieuTrenXeTamXuong(DateTime dateTime)
        {
            var query = $@" declare @DynamicPivotQuery as nvarchar(Max)
 declare @ColumnName as nvarchar(Max) 
 declare @PivotSelectColumnNames AS NVARCHAR(MAX)

 select @ColumnName = ISNULL(@ColumnName + ',','') +QUOTENAME(MaXuong) from (select distinct  MaXuong from PhieuCanChinhXepKhuon where Ngay =@ngay) as MS order by MaXuong
 SELECT @PivotSelectColumnNames 
    = ISNULL(@PivotSelectColumnNames + ',','')
    + 'ISNULL(' + QUOTENAME(MaXuong) + ', 0) AS '
    +  QUOTENAME(N'Xưởng ' +MaXuong)
FROM (SELECT DISTINCT MaXuong FROM PhieuCanChinhXepKhuon where Ngay =@ngay) AS Courses order by MaXuong
 set @DynamicPivotQuery = N'Select  MaLo as [Lô],LoaiCaName as [Loại Cá],ThanhPhamName as [Thành Phẩm], ChieuXaName as [Chiếu Xạ],SizeName as [Size],MauName as [Màu],'+ @PivotSelectColumnNames  +'  from (Select   p.MaLo,la.Ten as LoaiCaName,s.Ten as SizeName,ma.Ten as MauName,tp.Ten as ThanhPhamName,cx.Ten as ChieuXaName,MaXuong ,TrongLuong  from PhieuCanChinhXepKhuon p,MaLoaiCaXepKhuon la,MaMauXepKhuon ma,MaSizeChinhXepKhuon s, MaThanhPhamChinhXepKhuon tp, MaChieuXaXepKhuon cx where p.MaCoiChinh is null and p.Ngay = ''{dateTime.ToString("yyyy-MM-dd")}'' and p.MaLoaiCa = la.Ma and p.MaMau = ma.Ma and p.MaSizeChinh = s.Ma and p.MaThanhPhamChinh = tp.Ma and p.MaChieuXa = cx.Ma ) as tb pivot(sum ([TrongLuong]) for MaXuong in (' + @ColumnName + ')) as re order by MaLo,LoaiCaName,ThanhPhamName,ChieuXaName,SizeName,MauName, '+ @ColumnName
 EXEC sp_executesql @DynamicPivotQuery";
            using (var connection = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ngay", dateTime.Date);

                    connection.Open();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dataTable = new DataTable();
                        da.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        public DataTable GetTongHopThoiGianLuotRaCoi(DateTime dateTime)
        {
            var query = @"Select
    p.MaCoi,
    format( cast( p.ThoiGianBatDauQuay as datetime),'HH:mm:ss')  as ThoiGianBatDauQuay,
    p.Forced,
    p.MaXuong,
  format( CAST(p.GioRaCoi as datetime),'HH:mm:ss') as GioRaCoi,
    p.TrongLuong,
    p.LuotRaCoi,
    p.ThoiGianQuayThucTe,
   format( CAST(p.prev_GioRaCoi as datetime),'HH:mm:ss') as prev_GioRaCoi,
    p.ThoiGianNghiSoVoiLuotTruoc
from
    (
        Select
            p.*,
            DATEDIFF(minute, p.ThoiGianBatDauQuay, p.GioRaCoi) as ThoiGianQuayThucTe,
            lag(p.GioRaCoi) over (
                PARTITION by p.MaCoi
                order by
                    p.MaCoi,
                    p.ThoiGianBatDauQuay
            ) as prev_GioRaCoi,
            DATEDIFF(
                minute,
                lag(p.GioRaCoi) over (
                    PARTITION by p.MaCoi
                    order by
                        p.MaCoi,
                        p.ThoiGianBatDauQuay
                ),
                p.ThoiGianBatDauQuay
            ) as ThoiGianNghiSoVoiLuotTruoc
        from
            (
                Select
                    p.*,
                    ROW_NUMBER() over (
                        partition by p.MaCoi
                        order by
                            p.GioRaCoi
                    ) as [LuotRaCoi]
                from
                    (
                        Select
                            p.MaCoiChinh as MaCoi,
                            p.ThoiGianBatDauQuay,
                            p.Forced,
                            '1' as MaXuong,
                            case
                                when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                                else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                            end as GioRaCoi,
                            Sum(p.TrongLuong) as TrongLuong
                        from
                            PhieuCanChinhXepKhuon p
                        where
                            ngay = @ngay
                            and (
                                (
                                    MaXuong = '1'
                                    and ChuyenXuong = 0
                                )
                                or (
                                    ChuyenXuong = 1
                                    and MaXuong <> '1'
                                )
                            )
                            and MaCoiChinh is not null
                            and DaQuay = 1
                        group by
                            MaCoiChinh,
                            ThoiGianBatDauQuay,
                            Forced,
                            ThoiGianRaCoi,
                            ThoiGianQuay
                    ) p
            ) p
        UNION
        ALL
        Select
            *,
            DATEDIFF(minute, p.ThoiGianBatDauQuay, p.GioRaCoi) as ThoiGianQuayThucTe,
            lag(p.GioRaCoi) over (
                PARTITION by p.MaCoi
                order by
                    p.MaCoi,
                    p.ThoiGianBatDauQuay
            ) as prev_GioRaCoi,
            DATEDIFF(
                minute,
                lag(p.GioRaCoi) over (
                    PARTITION by p.MaCoi
                    order by
                        p.MaCoi,
                        p.ThoiGianBatDauQuay
                ),
                p.ThoiGianBatDauQuay
            ) as ThoiGianNghiSoVoiLuotTruoc
        from
            (
                Select
                    p.*,
                    ROW_NUMBER() over (
                        partition by p.MaCoi
                        order by
                            p.GioRaCoi
                    ) as [LuotRaCoi]
                from
                    (
                        Select
                            p.MaCoiChinh as MaCoi,
                            p.ThoiGianBatDauQuay,
                            p.Forced,
                            '2' as MaXuong,
                            case
                                when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                                else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                            end as GioRaCoi,
                            Sum(p.TrongLuong) as TrongLuong
                        from
                            PhieuCanChinhXepKhuon p
                        where
                            ngay = @ngay
                            and (
                                (
                                    MaXuong = '2'
                                    and ChuyenXuong = 0
                                )
                                or (
                                    ChuyenXuong = 1
                                    and MaXuong <> '2'
                                )
                            )
                            and MaCoiChinh is not null
                            and DaQuay = 1
                        group by
                            MaCoiChinh,
                            ThoiGianBatDauQuay,
                            Forced,
                            ThoiGianRaCoi,
                            ThoiGianQuay
                    ) p
            ) p
    ) p
order by
    MaXuong,
    MaCoi,
    ThoiGianBatDauQuay";
            using (var connection = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ngay", dateTime.Date);

                    connection.Open();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dataTable = new DataTable();
                        da.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        public List<T> GetTongHopThoiGianLuotRaCoi<T>(DateTime dateTime)
        {
            var query = @"Select
    p.MaCoi,
    p.ThoiGianBatDauQuay,
    p.Forced,
    p.MaXuong,
    p.GioRaCoi,
    p.TrongLuong,
    p.LuotRaCoi,
    p.ThoiGianQuayThucTe,
     case
        when p.prev_GioRaCoi is Null
        and lag(p.MaCoi) over (
            PARTITION by p.MaCoi
            order by
                p.MaCoi,
                p.ThoiGianBatDauQuay
        ) = p.MaCoi
        and lag(p.GioRaCoi) over (
            PARTITION by p.MaCoi
            order by
                p.MaCoi,
                p.ThoiGianBatDauQuay
        ) = p.GioRaCoi then lag(p.prev_GioRaCoi) over (
            PARTITION by p.MaCoi
            order by
                p.MaCoi,
                p.ThoiGianBatDauQuay
        )
        else p.prev_GioRaCoi
    end as prev_GioRaCoi,
    case
        when p.ThoiGianNghiSoVoiLuotTruoc is Null
        and lag(p.MaCoi) over (
            PARTITION by p.MaCoi
            order by
                p.MaCoi,
                p.ThoiGianBatDauQuay
        ) = p.MaCoi
        and lag(p.GioRaCoi) over (
            PARTITION by p.MaCoi
            order by
                p.MaCoi,
                p.ThoiGianBatDauQuay
        ) = p.GioRaCoi then lag(p.ThoiGianNghiSoVoiLuotTruoc) over (
            PARTITION by p.MaCoi
            order by
                p.MaCoi,
                p.ThoiGianBatDauQuay
        )
        else p.ThoiGianNghiSoVoiLuotTruoc
    end as ThoiGianNghiSoVoiLuotTruoc,
    cl.Ten as ChatLuongName,
    s.Ten as SizeName,
    tp.Ten as ThanhPhamName,
    cx.Ten as ChieuXaName
from
    (
        Select
            p.*,
            DATEDIFF(minute, p.ThoiGianBatDauQuay, p.GioRaCoi) as ThoiGianQuayThucTe,
            case
                when lag(p.GioRaCoi) over (
                    PARTITION by p.MaCoi
                    order by
                        p.MaCoi,
                        p.ThoiGianBatDauQuay
                ) != p.GioRaCoi then lag(p.GioRaCoi) over (
                    PARTITION by p.MaCoi
                    order by
                        p.MaCoi,
                        p.ThoiGianBatDauQuay
                )
            end as prev_GioRaCoi,
            DATEDIFF(
                minute,
                case
                    when lag(p.GioRaCoi) over (
                        PARTITION by p.MaCoi
                        order by
                            p.MaCoi,
                            p.ThoiGianBatDauQuay
                    ) != p.GioRaCoi then lag(p.GioRaCoi) over (
                        PARTITION by p.MaCoi
                        order by
                            p.MaCoi,
                            p.ThoiGianBatDauQuay
                    )
                end,
                p.ThoiGianBatDauQuay
            ) as ThoiGianNghiSoVoiLuotTruoc
        from
            (
                Select
                    p.*,
                    DENSE_RANK() over (
                        partition by p.MaCoi
                        order by
                            p.GioRaCoi
                    ) as [LuotRaCoi]
                from
                    (
                        Select
                            p.MaCoiChinh as MaCoi,
                            p.ThoiGianBatDauQuay,
                            p.MaSizeChinh,
                            p.Forced,
                            '1' as MaXuong,
                            case
                                when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                                else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                            end as GioRaCoi,
                            p.MaChatLuong,
                            p.MaChieuXa,
                            Sum(p.TrongLuong) as TrongLuong,
                            p.MaThanhPhamChinh
                        from
                            PhieuCanChinhXepKhuon p
                        where
                            ngay = @ngay
                            and (
                                (
                                    MaXuong = '1'
                                    and ChuyenXuong = 0
                                )
                                or (
                                    ChuyenXuong = 1
                                    and MaXuong <> '1'
                                )
                            )
                            and MaCoiChinh is not null
                            and DaQuay = 1
                        group by
                            MaCoiChinh,
                            ThoiGianBatDauQuay,
                            Forced,
                            ThoiGianRaCoi,
                            ThoiGianQuay,
                            MaChatLuong,
                            p.MaChieuXa,
                            MaThanhPhamChinh,
                            p.MaSizeChinh
                    ) p
            ) p
        UNION
        ALL
        Select
            *,
            DATEDIFF(minute, p.ThoiGianBatDauQuay, p.GioRaCoi) as ThoiGianQuayThucTe,
            case
                when lag(p.GioRaCoi) over (
                    PARTITION by p.MaCoi
                    order by
                        p.MaCoi,
                        p.ThoiGianBatDauQuay
                ) != p.GioRaCoi then lag(p.GioRaCoi) over (
                    PARTITION by p.MaCoi
                    order by
                        p.MaCoi,
                        p.ThoiGianBatDauQuay
                )
            end as prev_GioRaCoi,
            DATEDIFF(
                minute,
                case
                    when lag(p.GioRaCoi) over (
                        PARTITION by p.MaCoi
                        order by
                            p.MaCoi,
                            p.ThoiGianBatDauQuay
                    ) != p.GioRaCoi then lag(p.GioRaCoi) over (
                        PARTITION by p.MaCoi
                        order by
                            p.MaCoi,
                            p.ThoiGianBatDauQuay
                    )
                end,
                p.ThoiGianBatDauQuay
            ) as ThoiGianNghiSoVoiLuotTruoc
        from
            (
                Select
                    p.*,
                    DENSE_RANK() over (
                        partition by p.MaCoi
                        order by
                            p.GioRaCoi
                    ) as [LuotRaCoi]
                from
                    (
                        Select
                            p.MaCoiChinh as MaCoi,
                            p.ThoiGianBatDauQuay,
                            p.MaSizeChinh,
                            p.Forced,
                            '2' as MaXuong,
                            case
                                when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                                else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                            end as GioRaCoi,
                            p.MaChatLuong,
                            p.MaChieuXa,
                            Sum(p.TrongLuong) as TrongLuong,
                            p.MaThanhPhamChinh
                        from
                            PhieuCanChinhXepKhuon p
                        where
                            ngay = @ngay
                            and (
                                (
                                    MaXuong = '2'
                                    and ChuyenXuong = 0
                                )
                                or (
                                    ChuyenXuong = 1
                                    and MaXuong <> '2'
                                )
                            )
                            and MaCoiChinh is not null
                            and DaQuay = 1
                        group by
                            MaCoiChinh,
                            ThoiGianBatDauQuay,
                            Forced,
                            ThoiGianRaCoi,
                            ThoiGianQuay,
                            MaChatLuong,
                            p.MaChieuXa,
                            MaThanhPhamChinh,
                            p.MaSizeChinh
                    ) p
            ) p
    ) p,
    MaChatLuongXepKhuon cl,
    MaThanhPhamChinhXepKhuon tp,
    MaSizeChinhXepKhuon s,
    MaChieuXaXepKhuon cx
where
    p.MaChatLuong = cl.Ma
    and p.MaThanhPhamChinh = tp.Ma
    and p.MaSizeChinh = s.Ma
    and p.MaChieuXa = cx.Ma
order by
    MaXuong,
    MaCoi,
    ThoiGianBatDauQuay";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
            return items;
        }
        public List<T> GetTongHopThoiGianLuotRaCoiFromDate<T>(DateTime fromDate,DateTime toDate)
        {
            var query = @"Select
    p.MaCoi,
	c.Ten as CoiChinhName,
    p.ThoiGianBatDauQuay,
    p.Forced,
    p.MaXuong,
	x.Ten as XuongName,
    p.GioRaCoi,
    p.TrongLuong,
    p.LuotRaCoi,
    p.ThoiGianQuayThucTe,
     case
        when p.prev_GioRaCoi is Null
        and lag(p.MaCoi) over (
            PARTITION by p.MaCoi
            order by
                p.MaCoi,
                p.ThoiGianBatDauQuay
        ) = p.MaCoi
        and lag(p.GioRaCoi) over (
            PARTITION by p.MaCoi
            order by
                p.MaCoi,
                p.ThoiGianBatDauQuay
        ) = p.GioRaCoi then lag(p.prev_GioRaCoi) over (
            PARTITION by p.MaCoi
            order by
                p.MaCoi,
                p.ThoiGianBatDauQuay
        )
        else p.prev_GioRaCoi
    end as prev_GioRaCoi,
    case
        when p.ThoiGianNghiSoVoiLuotTruoc is Null
        and lag(p.MaCoi) over (
            PARTITION by p.MaCoi
            order by
                p.MaCoi,
                p.ThoiGianBatDauQuay
        ) = p.MaCoi
        and lag(p.GioRaCoi) over (
            PARTITION by p.MaCoi
            order by
                p.MaCoi,
                p.ThoiGianBatDauQuay
        ) = p.GioRaCoi then lag(p.ThoiGianNghiSoVoiLuotTruoc) over (
            PARTITION by p.MaCoi
            order by
                p.MaCoi,
                p.ThoiGianBatDauQuay
        )
        else p.ThoiGianNghiSoVoiLuotTruoc
    end as ThoiGianNghiSoVoiLuotTruoc,
    cl.Ten as ChatLuongName,
    s.Ten as SizeName,
    tp.Ten as ThanhPhamName,
    cx.Ten as ChieuXaName
from
    (
        Select
            p.*,
            DATEDIFF(minute, p.ThoiGianBatDauQuay, p.GioRaCoi) as ThoiGianQuayThucTe,
            case
                when lag(p.GioRaCoi) over (
                    PARTITION by p.MaCoi
                    order by
                        p.MaCoi,
                        p.ThoiGianBatDauQuay
                ) != p.GioRaCoi then lag(p.GioRaCoi) over (
                    PARTITION by p.MaCoi
                    order by
                        p.MaCoi,
                        p.ThoiGianBatDauQuay
                )
            end as prev_GioRaCoi,
            DATEDIFF(
                minute,
                case
                    when lag(p.GioRaCoi) over (
                        PARTITION by p.MaCoi
                        order by
                            p.MaCoi,
                            p.ThoiGianBatDauQuay
                    ) != p.GioRaCoi then lag(p.GioRaCoi) over (
                        PARTITION by p.MaCoi
                        order by
                            p.MaCoi,
                            p.ThoiGianBatDauQuay
                    )
                end,
                p.ThoiGianBatDauQuay
            ) as ThoiGianNghiSoVoiLuotTruoc
        from
            (
                Select
                    p.*,
                    DENSE_RANK() over (
                        partition by p.MaCoi
                        order by
                            p.GioRaCoi
                    ) as [LuotRaCoi]
                from
                    (
                        Select
                            p.MaCoiChinh as MaCoi,
                            p.ThoiGianBatDauQuay,
                            p.MaSizeChinh,
                            p.Forced,
                            '1' as MaXuong,
                            case
                                when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                                else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                            end as GioRaCoi,
                            p.MaChatLuong,
                            p.MaChieuXa,
                            Sum(p.TrongLuong) as TrongLuong,
                            p.MaThanhPhamChinh
                        from
                            PhieuCanChinhXepKhuon p
                        where
                            ngay <= @toDate
                            and ngay >= @fromDate
                            and (
                                (
                                    MaXuong = '1'
                                    and ChuyenXuong = 0
                                )
                                or (
                                    ChuyenXuong = 1
                                    and MaXuong <> '1'
                                )
                            )
                            and MaCoiChinh is not null
                            and DaQuay = 1
                        group by
                            MaCoiChinh,
                            ThoiGianBatDauQuay,
                            Forced,
                            ThoiGianRaCoi,
                            ThoiGianQuay,
                            MaChatLuong,
                            p.MaChieuXa,
                            MaThanhPhamChinh,
                            p.MaSizeChinh
                    ) p
            ) p
        UNION
        ALL
        Select
            *,
            DATEDIFF(minute, p.ThoiGianBatDauQuay, p.GioRaCoi) as ThoiGianQuayThucTe,
            case
                when lag(p.GioRaCoi) over (
                    PARTITION by p.MaCoi
                    order by
                        p.MaCoi,
                        p.ThoiGianBatDauQuay
                ) != p.GioRaCoi then lag(p.GioRaCoi) over (
                    PARTITION by p.MaCoi
                    order by
                        p.MaCoi,
                        p.ThoiGianBatDauQuay
                )
            end as prev_GioRaCoi,
            DATEDIFF(
                minute,
                case
                    when lag(p.GioRaCoi) over (
                        PARTITION by p.MaCoi
                        order by
                            p.MaCoi,
                            p.ThoiGianBatDauQuay
                    ) != p.GioRaCoi then lag(p.GioRaCoi) over (
                        PARTITION by p.MaCoi
                        order by
                            p.MaCoi,
                            p.ThoiGianBatDauQuay
                    )
                end,
                p.ThoiGianBatDauQuay
            ) as ThoiGianNghiSoVoiLuotTruoc
        from
            (
                Select
                    p.*,
                    DENSE_RANK() over (
                        partition by p.MaCoi
                        order by
                            p.GioRaCoi
                    ) as [LuotRaCoi]
                from
                    (
                        Select
                            p.MaCoiChinh as MaCoi,
                            p.ThoiGianBatDauQuay,
                            p.MaSizeChinh,
                            p.Forced,
                            '2' as MaXuong,
                            case
                                when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                                else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                            end as GioRaCoi,
                            p.MaChatLuong,
                            p.MaChieuXa,
                            Sum(p.TrongLuong) as TrongLuong,
                            p.MaThanhPhamChinh
                        from
                            PhieuCanChinhXepKhuon p
                        where
                            ngay <= @toDate
                            and ngay >= @fromDate
                            and (
                                (
                                    MaXuong = '2'
                                    and ChuyenXuong = 0
                                )
                                or (
                                    ChuyenXuong = 1
                                    and MaXuong <> '2'
                                )
                            )
                            and MaCoiChinh is not null
                            and DaQuay = 1
                        group by
                            MaCoiChinh,
                            ThoiGianBatDauQuay,
                            Forced,
                            ThoiGianRaCoi,
                            ThoiGianQuay,
                            MaChatLuong,
                            p.MaChieuXa,
                            MaThanhPhamChinh,
                            p.MaSizeChinh
                    ) p
            ) p
    ) p,
    MaChatLuongXepKhuon cl,
    MaThanhPhamChinhXepKhuon tp,
    MaSizeChinhXepKhuon s,
    MaChieuXaXepKhuon cx,
	MaCoiXepKhuon c,
	XiNghiep x
where
    p.MaChatLuong = cl.Ma
    and p.MaThanhPhamChinh = tp.Ma
    and p.MaSizeChinh = s.Ma
    and p.MaChieuXa = cx.Ma
	and p.MaCoi = c.Ma
	and p.MaXuong = x.Ma
order by
    MaXuong,
    MaCoi,
    ThoiGianBatDauQuay";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { fromDate = fromDate.Date ,toDate = toDate.Date}).ToList();
            return items;
        }
        /// <summary>
        ///     DaQuay,ChuaQuay,DangQuay - Xuong, Lo, Size, ThanhPham
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public List<T> GetTongHopTrangThais<T>(DateTime dateTime)
        {
            var query = @"Select
    p.Trang_Thai,
    p.MaXuong,
    p.MaLo,
    s.Ten as SizeName,
    tp.Ten as ThanhPhamName,
    SUM(p.TrongLuong) as TrongLuong
from
    (
        Select
            p.MaLo,
            p.MaSizeChinh,
            p.MaThanhPhamChinh,
            case
                when p.MaCoiChinh is not null
                and p.ThoiGianRaCoi is null
                and p.DaQuay = 1 then N'DangQuay'
                when p.MaCoiChinh is not null
                and p.ThoiGianRaCoi is not null
                and p.DaQuay = 1 then N'DaQuay'
                else N'ChuaQuay'
            end as Trang_Thai,
            Sum(p.TrongLuong) as TrongLuong,
            CASE
                when ChuyenXuong = 0 then MaXuong
                when ChuyenXuong = 1 then (
                    case
                        when MaXuong = '1' then '2'
                        else '1'
                    end
                )
            end as MaXuong,
            p.ChuyenXuong
        from
            PhieuCanChinhXepKhuon p
        where
            p.Ngay = @ngay
        GROUP BY
            p.MaXuong,
            p.MaLo,
            p.MaSizeChinh,
            p.MaThanhPhamChinh,
            p.MaCoiChinh,
            p.ThoiGianRaCoi,
            p.DaQuay,
            p.ChuyenXuong
    ) p,
    MaThanhPhamChinhXepKhuon tp,
    MaSizeChinhXepKhuon s
where
    p.MaThanhPhamChinh = tp.Ma
    and p.MaSizeChinh = s.Ma
GROUP BY
    p.Trang_Thai,
    p.MaXuong,
    p.MaLo,
    s.Ten,
    tp.Ten
order by
    Trang_Thai,
    MaXuong,
    MaLo,
    SizeName,
    ThanhPhamName";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
                return items;
            }
        }

        public bool Insert(Models.Repos.Models.PhieuCanChinhXepKhuon phieuCan)
        {
            try
            {
                var query =
                    "Insert Into PhieuCanChinhXepKhuon ([STT],[Ngay],[Gio],[MaCoiTam],[MaCoiChinh],[DaQuay],[ThoiGianBatDauQuay],[ThoiGianQuay],[TrongLuong],[MaLo],[MaLoaiCa],[MaSizeChinh],[MaMau],[MaThanhPhamChinh],[MaChatLuong],[MaKhuVuc],[MaNhanVien],[MaNhom],[MaUserCan],[MaXuong],[MaMayCan],[GhiChu],[ThoiGianRaCoi],[Forced],[MaChieuXa],[TaiChe],[ChuyenXuong],[MaNhanVienPvPhanCo],[TrongLuongTare],[NgayNguyenLieu],[NgayRaCoi],[NgayBatDauQuay]) Values (@STT,@Ngay,@Gio,@MaCoiTam,@MaCoiChinh,@DaQuay,@ThoiGianBatDauQuay,@ThoiGianQuay,@TrongLuong,@MaLo,@MaLoaiCa,@MaSizeChinh,@MaMau,@MaThanhPhamChinh,@MaChatLuong,@MaKhuVuc,@MaNhanVien,@MaNhom,@MaUserCan,@MaXuong,@MaMayCan,@GhiChu,@ThoiGianRaCoi,@Forced,@MaChieuXa,@TaiChe,@ChuyenXuong,@MaNhanVienPvPhanCo,@TrongLuongTare,@NgayNguyenLieu,@NgayRaCoi,@NgayBatDauQuay)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //using (var transaction = connection.BeginTransaction())
                    //{
                    //    var affectedRows = connection.Execute(query, phieuCan, transaction);
                    //    transaction.Commit();
                    //    if (affectedRows > 0)
                    //        return true;
                    //    return false;
                    //}

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

        public int Insert<T>(List<T> phieuCans)
        {
            try
            {
                var query =
                    "Insert Into PhieuCanChinhXepKhuon ([STT],[Ngay],[Gio],[MaCoiTam],[MaCoiChinh],[DaQuay],[ThoiGianBatDauQuay],[ThoiGianQuay],[TrongLuong],[MaLo],[MaLoaiCa],[MaSizeChinh],[MaMau],[MaThanhPhamChinh],[MaChatLuong],[MaKhuVuc],[MaNhanVien],[MaNhom],[MaUserCan],[MaXuong],[MaMayCan],[GhiChu],[ThoiGianRaCoi],[Forced],[MaChieuXa],[TaiChe],[ChuyenXuong],[MaNhanVienPvPhanCo],[TrongLuongTare],[NgayNguyenLieu],[NgayRaCoi],[NgayBatDauQuay],[IdMonitor],[MayQuay]) Values (@STT,@Ngay,@Gio,@MaCoiTam,@MaCoiChinh,@DaQuay,@ThoiGianBatDauQuay,@ThoiGianQuay,@TrongLuong,@MaLo,@MaLoaiCa,@MaSizeChinh,@MaMau,@MaThanhPhamChinh,@MaChatLuong,@MaKhuVuc,@MaNhanVien,@MaNhom,@MaUserCan,@MaXuong,@MaMayCan,@GhiChu,@ThoiGianRaCoi,@Forced,@MaChieuXa,@TaiChe,@ChuyenXuong,@MaNhanVienPvPhanCo,@TrongLuongTare,@NgayNguyenLieu,@NgayRaCoi,@NgayBatDauQuay,@IdMonitor, @MayQuay)";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //using (var transaction = connection.BeginTransaction())
                    //{
                    //    var affectedRows = connection.Execute(query, phieuCan, transaction);
                    //    transaction.Commit();
                    //    if (affectedRows > 0)
                    //        return true;
                    //    return false;
                    //}

                    var affectedRows = connection.Execute(query, phieuCans);
                    return affectedRows;
                    //if (affectedRows > 0)
                    //    return true;
                    //return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public bool Update(Models.Repos.Models.PhieuCanChinhXepKhuon phieuCan)
        {
            try
            {
                var query =
                    "Update PhieuCanChinhXepKhuon Set [Gio] = @Gio,[MaCoiTam]=@MaCoiTam,[MaCoiChinh]=@MaCoiChinh,[DaQuay]=@DaQuay,[ThoiGianBatDauQuay]=@ThoiGianBatDauQuay,[ThoiGianQuay]=@ThoiGianQuay,[TrongLuong]=@TrongLuong,[MaLo]=@MaLo,[MaLoaiCa]=@MaLoaiCa,[MaSizeChinh]=@MaSizeChinh,[MaMau]=@MaMau,[MaThanhPhamChinh]=@MaThanhPhamChinh,[MaChatLuong]=@MaChatLuong,[MaKhuVuc]=@MaKhuVuc,[MaNhanVien]=@MaNhanVien,[MaNhom]=@MaNhom,[MaUserCan]=@MaUserCan,[GhiChu] =@GhiChu,[ThoiGianRaCoi]=@ThoiGianRaCoi,[Forced] =@Forced,[MaChieuXa] = @MaChieuXa,[TaiChe] = @TaiChe,ChuyenXuong = @ChuyenXuong, MaNhanVienPvPhanCo = @MaNhanVienPvPhanCo, [TrongLuongTare] = @TrongLuongTare, [NgayRaCoi] =@NgayRaCoi, [NgayBatDauQuay] =@NgayBatDauQuay,[MayQuay]=@MayQuay,[IdMonitor]=@IdMonitor Where [STT]=@STT and [NgayNguyenLieu]=@NgayNguyenLieu and [MaXuong]= @MaXuong and [MaMayCan]=@MaMayCan ";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //using (var transaction = connection.BeginTransaction())
                    //{
                    //    var affectedRows = connection.Execute(query, phieuCan, transaction);
                    //    transaction.Commit();
                    //    if (affectedRows > 0)
                    //        return true;
                    //    return false;
                    //}

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

        //public bool Update(List<Models.Repos.Models.PhieuCanChinhXepKhuon> phieuCans)
        //{
        //    try
        //    {
        //        var query =
        //            " UPDATE[dbo].[PhieuCanChinhXepKhuon] SET [Gio] = @Gio,[MaCoiTam] =@MaCoiTam, [MaCoiChinh] = @MaCoiChinh,[DaQuay] = @DaQuay,  [ThoiGianBatDauQuay] = @ThoiGianBatDauQuay,  [ThoiGianRaCoi] = @ThoiGianRaCoi, [Forced] = @Forced, [ThoiGianQuay] =@ThoiGianQuay, [TrongLuong] = @TrongLuong, [MaLo] = @MaLo, [MaLoaiCa] = @MaLoaiCa,[MaSizeChinh] = @MaSizeChinh, [MaMau] = @MaMau, [MaChatLuong] =@MaChatLuong, [MaThanhPhamChinh] = @MaThanhPhamChinh, [MaKhuVuc] = @MaKhuVuc, [MaNhanVien] = @MaNhanVien, [MaNhom] = @MaNhom, [MaChieuXa] = @MaChieuXa, [TaiChe] =@TaiChe, [MaUserCan] = @MaUserCan, [MaXuong] =@MaXuong,  [MaMayCan] =@MaMayCan, [GhiChu] = @GhiChu,ChuyenXuong = @ChuyenXuong WHERE [STT] = @STT and [Ngay] = @Ngay and [MaXuong] =@MaXuong and  [MaMayCan] =@MaMayCan  ";
        //        using(var connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();
        //            //using(var transaction = connection.BeginTransaction())
        //            //{
        //            //var affectedRows = connection.Execute(query, phieuCans, transaction);
        //            var affectedRows = connection.Execute(query, phieuCans);//, transaction);
        //            //transaction.Commit();
        //            if(affectedRows > 0)
        //                return true;
        //            return false;
        //            //}
        //        }
        //    } catch(Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        throw;
        //    }
        //}
        public int Update<T>(List<T> phieuCans)
        {
            try
            {
                var query =
                    @"UPDATE[dbo].[PhieuCanChinhXepKhuon]
                SET[Gio] = @Gio 
      ,[MaCoiTam] = @MaCoiTam 
      ,[MaCoiChinh] =@MaCoiChinh 
      ,[DaQuay] = @DaQuay 
      ,[ThoiGianBatDauQuay] = @ThoiGianBatDauQuay 
      ,[ThoiGianRaCoi] = @ThoiGianRaCoi 
      ,[Forced] = @Forced 
      ,[ThoiGianQuay] = @ThoiGianQuay 
      ,[TrongLuong] = @TrongLuong 
      ,[MaLo] = @MaLo 
      ,[MaLoaiCa] = @MaLoaiCa 
      ,[MaSizeChinh] = @MaSizeChinh 
      ,[MaMau] = @MaMau 
      ,[MaChatLuong] = @MaChatLuong 
      ,[MaThanhPhamChinh] = @MaThanhPhamChinh 
      ,[MaKhuVuc] = @MaKhuVuc 
      ,[MaNhanVien] = @MaNhanVien
      ,[MaNhom] = @MaNhom 
      ,[MaChieuXa] = @MaChieuXa 
      ,[TaiChe] = @TaiChe 
      ,[MaUserCan] = @MaUserCan 
      ,[GhiChu] = @GhiChu 
      ,[LuotQuay] = @LuotQuay 
      ,[ChuyenXuong] = @ChuyenXuong 
,[MaNhanVienPvPhanCo] = @MaNhanVienPvPhanCo,[TrongLuongTare] =@TrongLuongTare, [NgayRaCoi] =@NgayRaCoi, [NgayBatDauQuay] =@NgayBatDauQuay, [IdMonitor] =@IdMonitor, [MayQuay] =@MayQuay
  WHERE [STT] = @STT 
      And [NgayNguyenLieu]=@NgayNguyenLieu
      And [MaXuong] = @MaXuong 
      And [MaMayCan] = @MaMayCan";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //using(var transaction = connection.BeginTransaction())
                    //{
                    //var affectedRows = connection.Execute(query, phieuCans, transaction);
                    var affectedRows = connection.Execute(query, phieuCans); //, transaction);
                    //transaction.Commit();
                    return affectedRows;
                    //if (affectedRows > 0)
                    //    return true;
                    //return false;
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public int UpdateChatLuongByIdMonitor(string maChatLuong, string IdMonitor)
        {
            var query = qrUpdateChatLuongByIdMonirtor;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var affectedRows = connection.Execute(query, new { maChatLuong, IdMonitor });
                return affectedRows;
            }
        }
        //        public int UpdateDatabase()
        //        {
        //            try
        //            {
        //                //Thêm Cột Id Vào bảng MaLoaiCa trên máy cân đầu Ao
        //                var query =
        //                    @"DECLARE @tb varchar(30) = 'PhieuCanChinhXepKhuon' 
        //IF COL_LENGTH(@tb, 'TrongLuongTare') IS NULL BEGIN
        //ALTER TABLE
        //    PhieuCanChinhXepKhuon
        //ADD
        //    [TrongLuongTare] [decimal](18, 3) NOT NULL DEFAULT ((0))
        //END
        //IF COL_LENGTH(@tb, 'NgayNguyenLieu') IS NULL BEGIN
        //ALTER TABLE
        //    PhieuCanChinhXepKhuon
        //ADD
        //    [NgayNguyenLieu] [date] NOT NULL DEFAULT (getdate())
        //END
        //IF COL_LENGTH(@tb, 'NgayRaCoi') IS NULL BEGIN
        //ALTER TABLE
        //    PhieuCanChinhXepKhuon
        //ADD
        //    [NgayRaCoi] [date] NULL 
        //END
        //IF COL_LENGTH(@tb, 'NgayBatDauQuay') IS NULL BEGIN
        //ALTER TABLE
        //    PhieuCanChinhXepKhuon
        //ADD
        //    [NgayBatDauQuay] [date] NULL 
        //END
        //";
        //                var dao = new Database(connectionString);
        //                return dao.ExecuteNonQuery(query);
        //            }
        //            catch (Exception exception)
        //            {
        //                throw new Exception(
        //                    $@"Không thể cập nhật Cơ Sở Dữ Liệu vui lòng liên hệ PMS để được hổ trợ [Phiếu Cân Chính Xếp Khuôn]. {Environment.NewLine}{exception.Message}");
        //                //throw;
        //            }
        //        }

        #region chắt làm báo cáo xếp khuôn phát tiến

        public List<TEntity> GetPhieuCanTongHopChiTietCois<TEntity>(DateTime fromDate, DateTime dateTime,
            string xuongId)
        {
            try
            {
                var query =
                    @"select p.*,
cast(p.NgayGioRaCoi as time) as ThoiGianRaCoi from (
Select
                    p.STT,
                    p.ngay,
                    p.MaNhanVien,
                    p.MaCoiChinh,      
                    p.MaXuong,
                    p.ThoiGianBatDauQuay,   
                    p.TrongLuong,     
                    p.MaLo,   
                    tp.Ten as ThanhPhamName,
                    s.Ten as SizeName,
                    cx.Ten as ChieuXaName,
					p.MaMayCan,
					p.TrongLuongTare,
					ch.Ten,
					p.NgayNguyenLieu,
					p.NgayBatDauQuay,
					p.NgayRaCoi,
					(
                        case
                            when p.ThoiGianRaCoi is not null
                            and p.NgayRaCoi is null then cast(p.NgayNguyenLieu as datetime) + cast(p.ThoiGianRaCoi as datetime)
                            when p.ThoiGianRaCoi is not null
                            and p.NgayRaCoi is not null then cast(p.NgayRaCoi as datetime) + cast(p.ThoiGianRaCoi as datetime)
                            else (
                                DATEADD(
                                    MINUTE,
                                    p.ThoiGianQuay,
                                    (
                                        Cast(
                                            isnull(p.NgayBatDauQuay, p.NgayNguyenLieu) as datetime
                                        ) + cast(p.ThoiGianBatDauQuay as datetime)
                                    )
                                )
                            )
                        end
                    ) as NgayGioRaCoi
				
                from
                    PhieuCanChinhXepKhuon p
                  left join  MaLoaiCaXepKhuon la on p.MaLoaiCa = la.Ma
                    left join MaThanhPhamChinhXepKhuon tp on p.MaThanhPhamChinh = tp.Ma
                  left join   MaSizeChinhXepKhuon s on p.MaSizeChinh = s.Ma
                   left join MaChatLuongXepKhuon ch on p.MaChatLuong = ch.Ma
                    left join MaChieuXaXepKhuon cx on p.MaChieuXa = cx.Ma
                where
                    p.NgayNguyenLieu <= @ngay
                    and p.NgayNguyenLieu >= @fromDate
					and p.MaXuong =@xuongId
                    and p.STT >0
)p
		";
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
        public List<TEntity> GetPhieuCanTongHopChiTietCois<TEntity>(DateTime fromDate, DateTime dateTime, string maCoi,
            string xuongId)
        {
            try
            {
                var query =
                    @"select p.*,
cast(p.NgayGioRaCoi as time) as ThoiGianRaCoi from (
Select
                    p.STT,
                    p.ngay,
                    p.Gio,
                    p.MaNhanVien,
                    p.MaCoiChinh,      
                    p.MaXuong,
                    p.ThoiGianBatDauQuay,   
                    p.TrongLuong,     
                    p.MaLo,   
                    tp.Ten as ThanhPhamName,
                    s.Ten as SizeName,
                    cx.Ten as ChieuXaName,
					p.MaMayCan,
					p.TrongLuongTare,
					ch.Ten,
					p.NgayNguyenLieu,
					p.NgayBatDauQuay,
					p.NgayRaCoi,
					(
                        case
                            when p.ThoiGianRaCoi is not null
                            and p.NgayRaCoi is null then cast(p.NgayNguyenLieu as datetime) + cast(p.ThoiGianRaCoi as datetime)
                            when p.ThoiGianRaCoi is not null
                            and p.NgayRaCoi is not null then cast(p.NgayRaCoi as datetime) + cast(p.ThoiGianRaCoi as datetime)
                            else (
                                DATEADD(
                                    MINUTE,
                                    p.ThoiGianQuay,
                                    (
                                        Cast(
                                            isnull(p.NgayBatDauQuay, p.NgayNguyenLieu) as datetime
                                        ) + cast(p.ThoiGianBatDauQuay as datetime)
                                    )
                                )
                            )
                        end
                    ) as NgayGioRaCoi
				
                from
                    PhieuCanChinhXepKhuon p
                  left join  MaLoaiCaXepKhuon la on p.MaLoaiCa = la.Ma
                    left join MaThanhPhamChinhXepKhuon tp on p.MaThanhPhamChinh = tp.Ma
                  left join   MaSizeChinhXepKhuon s on p.MaSizeChinh = s.Ma
                   left join MaChatLuongXepKhuon ch on p.MaChatLuong = ch.Ma
                    left join MaChieuXaXepKhuon cx on p.MaChieuXa = cx.Ma
                where
                    p.NgayNguyenLieu <= @ngay
                    and p.NgayNguyenLieu >= @fromDate
					and p.MaXuong =@xuongId
                    and p.STT >0
                    and p.MaCoiChinh = @maCoi
)p
		";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection
                    .QueryAsync<TEntity>(query, new { fromDate = fromDate.Date, ngay = dateTime.Date, maCoi, xuongId })
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
        public List<TEntity> GetPhieuCanTongHopCois<TEntity>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    //               @"Select 
                    //                 ROW_NUMBER() OVER (ORDER BY p.Ngay) as STT, 
                    //  p.Ngay,
                    //  p.MaXuong,
                    //  p.ChuyenXuong, 
                    //  p.MaCoiChinh,
                    //  p.MaLo,
                    //  cx.Ten as ChieuXaName,
                    //  p.TaiChe,
                    //  la.Ten as LoaiCaName,
                    //  tp.Ten as ThanhPhamName,
                    //  s.Ten as SizeName,
                    //  c.Ten As ChatLuongName,
                    //  Sum(p.TrongLuong) as TrongLuong,
                    //  Count(*) as SoRo,
                    //  p.ThoiGianBatDauQuay,
                    //  --p.ThoiGianRaCoi,
                    //                 case
                    //                   when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                    //                   else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                    //                 end as ThoiGianRaCoi,
                    //                 p.ThoiGianQuay,
                    //                 p.MaNhanVien
                    //               from 
                    //PhieuCanChinhXepKhuon p,
                    //MaLoaiCaXepKhuon la,
                    //MaThanhPhamChinhXepKhuon tp,
                    //MaSizeChinhXepKhuon s,
                    //MaChatLuongXepKhuon c,
                    //MaChieuXaXepKhuon cx 
                    //               where p.MaChieuXa = cx.Ma and Ngay<=@ngay and Ngay >= @fromDate and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  and p.MaThanhPhamChinh = tp.Ma and p.MaLoaiCa = la.Ma and p.MaSizeChinh = s.Ma and p.MaChatLuong = c.Ma 
                    //               Group by p.Ngay,p.MaCoiChinh,p.MaLo,la.Ten,tp.Ten,s.Ten,c.Ten,p.TaiChe,cx.Ten,p.MaXuong,p.ChuyenXuong,p.ThoiGianBatDauQuay,p.ThoiGianRaCoi,p.MaNhanVien  ,p.ThoiGianQuay
                    //order by p.MaCoiChinh,p.MaLo,tp.Ten";
                    //                    @"
                    //                      select p.*,
                    //(
                    //     case
                    //        when p.NgayGioRaCoi > cast(CURRENT_TIMESTAMP as datetime)
                    //        then 0
                    //       else 1
                    //      end
                    //) as RaCoi,
                    //la.Ten as LoaiCaName,
                    //tp.Ten as ThanhPhamName,
                    //s.Ten as SizeName,
                    //c.Ten As ChatLuongName,
                    //cx.Ten as ChieuXaName,
                    //cast(p.NgayGioRaCoi as time) as ThoiGianRaCoi,cast(p.NgayGioRaCoi as date) as NgayRaCoi
                    //from
                    //(
                    //	Select 
                    //                      ROW_NUMBER() OVER (ORDER BY p.Ngay) as STT, 
                    //					  p.Ngay,
                    //					  p.MaXuong,
                    //					  p.ChuyenXuong, 
                    //					  p.MaCoiChinh,
                    //					  p.MaLo,

                    //					  p.MaChieuXa,
                    //					  p.TaiChe,
                    //					  --la.Ten as LoaiCaName,
                    //					  p.MaLoaiCa,
                    //					  --tp.Ten as ThanhPhamName,
                    //					  p.MaThanhPhamChinh,
                    //					  --s.Ten as SizeName,
                    //					  p.MaSizeChinh,
                    //					  --c.Ten As ChatLuongName,
                    //					  p.MaChatLuong,
                    //					  Sum(p.TrongLuong) as TrongLuong,
                    //					  Count(*) as SoRo,
                    //					  p.ThoiGianBatDauQuay,
                    //					 -- p.ThoiGianRaCoi,
                    //                      --case
                    //                      --  when p.ThoiGianRaCoi is not null then p.ThoiGianRaCoi
                    //                      --  else dateadd(MINUTE, p.ThoiGianQuay, p.ThoiGianBatDauQuay)
                    //                      --end as ThoiGianRaCoi,
                    //                      p.ThoiGianQuay,
                    //                      p.MaNhanVien,
                    //					  p.NgayNguyenLieu,
                    //					  p.NgayBatDauQuay,
                    //					  --p.NgayRaCoi,
                    //					  (
                    //                        case
                    //                            when p.ThoiGianRaCoi is not null
                    //                            and p.NgayRaCoi is null then cast(p.NgayNguyenLieu as datetime) + cast(p.ThoiGianRaCoi as datetime)
                    //                            when p.ThoiGianRaCoi is not null
                    //                            and p.NgayRaCoi is not null then cast(p.NgayRaCoi as datetime) + cast(p.ThoiGianRaCoi as datetime)
                    //                            else (
                    //                                DATEADD(
                    //                                    MINUTE,
                    //                                    p.ThoiGianQuay,
                    //                                    (
                    //                                        Cast(
                    //                                            isnull(p.NgayBatDauQuay, p.NgayNguyenLieu) as datetime
                    //                                        ) + cast(p.ThoiGianBatDauQuay as datetime)
                    //                                    )
                    //                                )
                    //                            )
                    //                        end
                    //                    ) as NgayGioRaCoi

                    //                    from 
                    //					PhieuCanChinhXepKhuon p

                    //                    where Ngay<=@ngay and Ngay >= @fromDate and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) 
                    //                    Group by p.Ngay,p.MaCoiChinh,p.MaLo,p.MaLoaiCa,p.MaThanhPhamChinh,p.MaSizeChinh,p.MaChatLuong,p.TaiChe,p.MaChieuXa,p.MaXuong,p.ChuyenXuong,p.ThoiGianBatDauQuay,p.ThoiGianRaCoi,p.MaNhanVien  ,p.ThoiGianQuay,p.NgayBatDauQuay,NgayNguyenLieu,p.NgayRaCoi

                    //) p
                    //					left join MaLoaiCaXepKhuon la on p.MaLoaiCa =la.Ma
                    //					left join MaThanhPhamChinhXepKhuon tp on p.MaThanhPhamChinh =tp.Ma
                    //					left join MaSizeChinhXepKhuon s on p.MaSizeChinh = s.Ma
                    //					left join MaChatLuongXepKhuon c on p.MaChatLuong =c.Ma
                    //					left join MaChieuXaXepKhuon cx on p.MaChieuXa = cx.Ma
                    //order by p.MaCoiChinh,p.MaLo,cx.Ten";
                    @"
select p.*,
    (
        case
            when p.NgayGioRaCoi > cast(CURRENT_TIMESTAMP as datetime)
            or isnull(p.MaCoiChinh, '') = '' then 0
            else 1
        end
    ) as RaCoi,
    tp.Ten as ThanhPhamName,
    s.Ten as SizeName,
    cx.Ten as ChieuXaName,
    cast(p.NgayGioRaCoi as time) as ThoiGianRaCoi,
    cast(p.NgayGioRaCoi as date) as NgayRaCoi,
    cast(
        (
            cast(
                DATEDIFF(ss, p.NgayGioBatDauQuay, p.NgayGioRaCoi) as decimal(18, 2)
            ) / 60
        ) as decimal(18, 2)
    ) as TongTGQuay2
from (
        Select 
            ROW_NUMBER() OVER (ORDER BY  p.MaXuong) as STT, 
            p.MaXuong,
            p.ChuyenXuong,
            p.MaCoiChinh,
            p.MaNhanVien,
            p.MaLo,
            p.MaChieuXa,
            p.MaThanhPhamChinh,
            p.MaSizeChinh,
            Sum(p.TrongLuong) as TrongLuong,
            Count(*) as SoRo,
            p.ThoiGianBatDauQuay,
            p.ThoiGianQuay,
            p.NgayNguyenLieu,
            p.NgayBatDauQuay,
            (
                Cast(
                    isnull(p.NgayBatDauQuay, p.NgayNguyenLieu) as datetime
                ) + cast(p.ThoiGianBatDauQuay as datetime)
            ) as NgayGioBatDauQuay,
            (
                case
                    when p.ThoiGianRaCoi is not null
                    and p.NgayRaCoi is null then cast(p.NgayNguyenLieu as datetime) + cast(p.ThoiGianRaCoi as datetime)
                    when p.ThoiGianRaCoi is not null
                    and p.NgayRaCoi is not null then cast(p.NgayRaCoi as datetime) + cast(p.ThoiGianRaCoi as datetime)
                    else (
                        DATEADD(
                            MINUTE,
                            p.ThoiGianQuay,
                            (
                                Cast(
                                    isnull(p.NgayBatDauQuay, p.NgayNguyenLieu) as datetime
                                ) + cast(p.ThoiGianBatDauQuay as datetime)
                            )
                        )
                    )
                end
            ) as NgayGioRaCoi
        from PhieuCanChinhXepKhuon p
        where NgayNguyenLieu <= @ngay
            and NgayNguyenLieu >= @fromDate
            and (
                (
                    MaXuong = @xuongId
                    and ChuyenXuong = 0
                )
                or (
                    ChuyenXuong = 1
                    and MaXuong <> @xuongId
                )
            )
        Group by
            p.MaCoiChinh,
            p.MaLo,
            p.MaThanhPhamChinh,
            p.MaSizeChinh,
            p.MaChieuXa,
            p.MaXuong,
            p.ChuyenXuong,
            p.ThoiGianBatDauQuay,
            p.ThoiGianRaCoi,
            p.MaNhanVien,
            p.ThoiGianQuay,
            p.NgayBatDauQuay,
            p.NgayNguyenLieu,
            p.NgayRaCoi
    ) p
    left join MaThanhPhamChinhXepKhuon tp on p.MaThanhPhamChinh = tp.Ma
    left join MaSizeChinhXepKhuon s on p.MaSizeChinh = s.Ma
    left join MaChieuXaXepKhuon cx on p.MaChieuXa = cx.Ma
order by p.MaCoiChinh,
    p.MaLo,
    cx.Ten
";
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

        /// <summary>
        ///     Ngày nguyên liệu
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="dateTime"></param>
        /// <param name="xuongId"></param>
        /// <returns></returns>
        public List<T> GetPhieuCanChinhXepKhuonsByDate<T>(DateTime fromDate, DateTime dateTime,
            string xuongId)
        {
            try
            {
                var query =
                    //"Select * from PhieuCanChinhXepKhuon Where NgayNguyenLieu<=@ngay and NgayNguyenLieu >= @fromDate  and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  order by Gio DESC";
                    //                   @"
                    //                    Select p.*,
                    //                    (
                    //                        Cast(isnull(p.NgayBatDauQuay, p.NgayNguyenLieu) as datetime) + cast(p.ThoiGianBatDauQuay as datetime)
                    //                    ) as NgayGioBatDauQuay,
                    //                    (
                    //                        case
                    //                            when p.ThoiGianRaCoi is not null
                    //                            and p.NgayRaCoi is null then cast(p.NgayNguyenLieu as datetime) + cast(p.ThoiGianRaCoi as datetime)
                    //                            when p.ThoiGianRaCoi is not null
                    //                            and p.NgayRaCoi is not null then cast(p.NgayRaCoi as datetime) + cast(p.ThoiGianRaCoi as datetime)
                    //                            else (
                    //                                DATEADD(
                    //                                    MINUTE,
                    //                                    p.ThoiGianQuay,
                    //                                    (
                    //                                        Cast(
                    //                                            isnull(p.NgayBatDauQuay, p.NgayNguyenLieu) as datetime
                    //                                        ) + cast(p.ThoiGianBatDauQuay as datetime)
                    //                                    )
                    //                                )
                    //                            )
                    //                        end
                    //                    ) as NgayGioRaCoi,  from PhieuCanChinhXepKhuon p
                    //Where NgayNguyenLieu<=@ngay 
                    //and NgayNguyenLieu >= @fromDate  
                    //and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId)) 

                    //order by Gio DESC
                    @"
select 
cast((cast(DATEDIFF(ss,p2.NgayGioBatDauQuay,p2.NgayGioRaCoi)as decimal(18,2))/60) as decimal(18,2))as TongTGQuay2, p2.*
from (Select p.*,
                    (
                        Cast(isnull(p.NgayBatDauQuay, p.NgayNguyenLieu) as datetime) + cast(p.ThoiGianBatDauQuay as datetime)
                    ) as NgayGioBatDauQuay,
                    (
                        case
                            when p.ThoiGianRaCoi is not null
                            and p.NgayRaCoi is null then cast(p.NgayNguyenLieu as datetime) + cast(p.ThoiGianRaCoi as datetime)
                            when p.ThoiGianRaCoi is not null
                            and p.NgayRaCoi is not null then cast(p.NgayRaCoi as datetime) + cast(p.ThoiGianRaCoi as datetime)
                            else (
                                DATEADD(
                                    MINUTE,
                                    p.ThoiGianQuay,
                                    (
                                        Cast(
                                            isnull(p.NgayBatDauQuay, p.NgayNguyenLieu) as datetime
                                        ) + cast(p.ThoiGianBatDauQuay as datetime)
                                    )
                                )
                            )
                        end
                    ) as NgayGioRaCoi
					--DATEDIFF(mi,DATEADD(hh,DATEDIFF(hh, NgayGioBatDauQuay, NgayGioRaCoi),NgayGioBatDauQuay),NgayGioRaCoi) as Minutes_Difference
					from PhieuCanChinhXepKhuon p
Where NgayNguyenLieu<=@ngay 
and NgayNguyenLieu >= @fromDate  
and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))
and p.STT > 0)p2




order by Gio DESC
";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(
                            query,
                            new { fromDate = fromDate.Date, ngay = dateTime.Date, xuongId })
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

        public List<TEntity> GetPhieuCanTongHopCois2<TEntity>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select p.ChuyenXuong,p.MaXuong, p.MaLo,la.Ten as LoaiCaName,tp.Ten as ThanhPhamName,cx.Ten as ChieuXaName,p.TaiChe,s.Ten as SizeName,c.Ten As ChatLuongName,Sum(p.TrongLuong) as TrongLuong, Count(*) as SoRo from PhieuCanChinhXepKhuon p,MaLoaiCaXepKhuon la,MaThanhPhamChinhXepKhuon tp,MaSizeChinhXepKhuon s, MaChatLuongXepKhuon c, MaChieuXaXepKhuon cx where  Ngay<=@ngay and Ngay >= @fromDate and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  and p.MaThanhPhamChinh = tp.Ma and p.MaLoaiCa = la.Ma and p.MaSizeChinh = s.Ma and p.MaChatLuong = c.Ma and p.MaChieuXa = cx.Ma Group by p.MaLo,la.Ten,tp.Ten,s.Ten,c.Ten ,cx.Ten,p.ChuyenXuong,p.MaXuong,p.TaiChe order by p.MaLo,tp.Ten";
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

        public List<T> GetPhieuCanTongHops2_RemoveChieuXa<T>(DateTime fromDate, DateTime dateTime,
            string xuongId)
        {
            try
            {
                var query =
                    "Select ROW_NUMBER() OVER (ORDER BY p.Ngay) as STT,p.Ngay,p.ChuyenXuong,p.MaXuong, p.MaLo,la.Ten As LoaiCaName,p.TaiChe,tp.Ten as ThanhPhamName,s.Ten As SizeName,ma.Ten As MauName, SUM(p.TrongLuong) as TrongLuong from  PhieuCanChinhXepKhuon p, MaLoaiCaXepKhuon la,MaSizeChinhXepKhuon s, MaThanhPhamChinhXepKhuon tp, MaMauXepKhuon ma where p.NgayNguyenLieu <= @ngay and p.NgayNguyenLieu >= @fromDate  and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  and p.MaLoaiCa = la.Ma and p.MaSizeChinh = s.Ma and p.MaThanhPhamChinh = tp.Ma and p.MaMau = ma.Ma  group by p.Ngay, p.MaLo,tp.Ten ,s.Ten ,ma.Ten,la.Ten,p.TaiChe,p.ChuyenXuong,p.MaXuong order by p.MaLo,tp.ten,s.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.Query<T>(query,
                            new { fromDate = fromDate.Date, ngay = dateTime.Date, xuongId })
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

        public List<TEntity> GetPhieuCanTongHops2<TEntity>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    "Select ROW_NUMBER() OVER (ORDER BY p.Ngay) as STT, p.Ngay,p.ChuyenXuong,p.MaXuong, p.MaLo,la.Ten As LoaiCaName,cx.Ten as ChieuXaName,p.TaiChe,tp.Ten as ThanhPhamName,s.Ten As SizeName,ma.Ten As MauName, SUM(p.TrongLuong) as TrongLuong from  PhieuCanChinhXepKhuon p, MaLoaiCaXepKhuon la,MaSizeChinhXepKhuon s, MaThanhPhamChinhXepKhuon tp, MaMauXepKhuon ma,MaChieuXaXepKhuon cx where p.NgayNguyenLieu <= @ngay and p.NgayNguyenLieu >= @fromDate  and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))  and p.MaLoaiCa = la.Ma and p.MaSizeChinh = s.Ma and p.MaThanhPhamChinh = tp.Ma and p.MaMau = ma.Ma and p.MaChieuXa = cx.Ma group by p.Ngay, p.MaLo,tp.Ten ,s.Ten ,ma.Ten,la.Ten,cx.Ten,p.TaiChe,p.ChuyenXuong,p.MaXuong order by p.MaLo,tp.ten,s.Ten";
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
        public List<T> GetPhieuCanTongHopsXepKhuon<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    @"Select ROW_NUMBER() OVER (ORDER BY p.Ngay) as STT,
p.Ngay,
p.ChuyenXuong,
p.MaXuong, 
p.MaLo,
la.Ten As LoaiCaName,
cx.Ten as ChieuXaName,
p.TaiChe,
p.MaThanhPhamChinh as MaThanhPham,
tp.Ten as ThanhPhamName,
s.Ten As SizeName,
ma.Ten As MauName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
SUM(p.TrongLuong) as TrongLuong
from  PhieuCanChinhXepKhuon p
left join MaLoaiCaXepKhuon la on p.MaLoaiCa = la.Ma 
left join MaSizeChinhXepKhuon s on  p.MaSizeChinh = s.Ma 
left join MaThanhPhamChinhXepKhuon tp on p.MaThanhPhamChinh = tp.Ma 
left join MaMauXepKhuon ma on p.MaMau = ma.Ma
left join MaChieuXaXepKhuon cx on p.MaChieuXa = cx.Ma
left join MaChatLuongXepKhuon cl on p.MaChatLuong = cl.Ma 
where 
p.NgayNguyenLieu <= @ngay 
and p.NgayNguyenLieu >= @fromDate  
and ((MaXuong = @xuongId and ChuyenXuong =0) or (ChuyenXuong = 1 and MaXuong <> @xuongId))   
group by 
p.Ngay,
p.MaLo,
tp.Ten ,
s.Ten ,
ma.Ten,
la.Ten,
cx.Ten,
p.TaiChe,
p.ChuyenXuong,
p.MaXuong,
p.MaThanhPhamChinh ,
p.MaChatLuong,
cl.Ten
order by 
p.MaLo,
tp.ten,
s.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query,
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
        #endregion
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan_XLPC<T>(DateTime dateTime, string xuongId)
        {
            var query = @"select
p.STT,
p.Ngay,
p.Gio,
p.MaCoiTam,
ct.Ten as CoiTamName,
p.MaCoiChinh,
cc.Ten as CoiChinhName,
p.DaQuay,
p.ThoiGianBatDauQuay,
p.ThoiGianRaCoi,
p.Forced,
p.ThoiGianQuay,
p.TrongLuong,
p.MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaSizeChinh,
s.Ten SizeChinhName,
p.MaMau,
m.Ten as MauName,
p.MaChatLuong,
cl.Ten as ChatLuongName,
p.MaThanhPhamChinh,
tp.Ten as ThanhPhamChinhName,
p.MaKhuVuc,
kv.Ten as KhuVucName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
nv.DeptName0 as Nhom,
p.MaNhom,
p.MaChieuXa,
cx.Ten as ChieuXaName,
p.TaiChe,
p.MaUserCan,
p.MaXuong,
x.Ten as XuongName,
p.MaMayCan,
p.GhiChu,
p.LuotQuay,
p.ChuyenXuong,
p.MaNhanVienPvPhanCo,
pv.Name as NhanVienPVName,
pv.MaHoSo as MaSoPhucVu,
p.TrongLuongTare,
p.NgayNguyenLieu,
p.NgayRaCoi,
p.NgayBatDauQuay,
p.IdMonitor
from PhieuCanChinhXepKhuon p
left join MaCoiXepKhuon ct on p.MaCoiTam = ct.Ma
left join MaCoiXepKhuon cc on p.MaCoiChinh = cc.Ma
left join MaLoaiCaXepKhuon lc on p.MaLoaiCa = lc.Ma
left join MaSizeChinhXepKhuon s on p.MaSizeChinh = s.Ma
left join MaMauXepKhuon m on p.MaMau = m.Ma
left join MaChatLuongXepKhuon cl on p.MaChatLuong = cl.Ma
left join MaThanhPhamChinhXepKhuon tp on p.MaThanhPhamChinh = tp.Ma
left join MaKhuVucXepKhuon kv on p.MaKhuVuc = kv.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join MaChieuXaXepKhuon cx on p.MaChieuXa = cx.Ma
left join XiNghiep x on p.MaXuong = x.Ma
left join NhanVienDaiThanh pv on p.MaNhanVienPvPhanCo = pv.MaNhanVien
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

