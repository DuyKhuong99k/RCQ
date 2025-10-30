using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanCaGiongVungNuoiDaiThanhSide
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanCaGiongVungNuoiDaiThanhSide";
        private readonly string qrDelete = @"
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanCaGiongVungNuoiDaiThanhSide]
           ([STT]
      ,[Ngay]
      ,[MaMayCan]
      ,[Gio]
      ,[MaUserCan]
      ,[GhiChu]
      ,[MaLoaiCaDaiThanhId]
      ,[MaGhe]
      ,[TenAo]
      ,[TenChuAo]
      ,[TenCongDoan]
      ,[TenThongKeDauAo]
      ,[TrongLuongTare]
      ,[TrongLuong]
      ,[BiosId]
      ,[GioTai]
      ,[NgayTai]
      ,[TenLoaiCa])
     VALUES
           (@STT
      ,@Ngay
      ,@MaMayCan
      ,@Gio
      ,@MaUserCan
      ,@GhiChu
      ,@MaLoaiCaDaiThanhId
      ,@MaGhe
      ,@TenAo
      ,@TenChuAo
      ,@TenCongDoan
      ,@TenThongKeDauAo
      ,@TrongLuongTare
      ,@TrongLuong
      ,@BiosId
      ,@GioTai
      ,@NgayTai
      ,@TenLoaiCa)";

        private readonly string qrUpdate = @"

";

        private readonly string qrGetAll = "Select * from PhieuCanCaGiongVungNuoiDaiThanhSide";

        public PhieuCanCaGiongVungNuoiDaiThanhSide(string? _connectionString = null)
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
            var query = @"Select * from PhieuCanCaGiongVungNuoiDaiThanhSide
where
    Ngay = @ngay";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
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


        #region Xử lý phiếu CÂN
        public List<T> GetPhieuCan<T>(DateTime dateTime)
        {
            try
            {
                var query = @"select
p.STT,
P.Ngay,
p.Gio,
p.NgayTai,
p.GioTai,
p.MaUserCan,
p.GhiChu,
p.MaLoaiCaDaiThanhId,
lc.Ten as LoaiCaVungNuoiName,
p.MaGhe,
pt.Ten as PhuongTienName,
p.TenAo,
p.TenCongDoan,
p.TenThongKeDauAo,
p.TrongLuong,
p.MaMayCan,
p.BiosId,
p.TenChuAo
from PhieuCanCaGiongVungNuoiDaiThanhSide p
left join MaLoaiCaVungNuoi lc on p.MaLoaiCaDaiThanhId = lc.Ma
left join PhuongTienChoNguyenLieu pt on p.MaGhe = pt.Ma
where p.Ngay =@dateTime";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { dateTime }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"Select
    STT,
    Ngay,
    MaMayCan,
     Cast(
        convert(
            varchar(19),
            cast(Ngay as datetime) + Cast(Gio as datetime),
            120
        ) as datetime
    ) as Gio,
    TenLoaiCa,
    MaGhe,
    TenAo,
    TenCongDoan,
    TenThongKeDauAo,
    TrongLuongTare,
    TrongLuong
from
    PhieuCanCaGiongVungNuoiDaiThanhSide
where
    Ngay >= @fromDate and Ngay <= @toDate
    order by 
    STT DESC";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHops<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"IF OBJECT_ID('tempdb.dbo.#temp_cagiong', 'U') IS NOT NULL DROP TABLE #temp_cagiong;
Select
    * into #temp_cagiong
from
    (
        SELECT
            (
                case
                    when isnull(p.[Minute], 0) <= 60 then 0
                    else 1
                end
            ) as lag,
            p.*
        from
            (
                Select
                    (
                        Case
                            when LAG(p.MaGhe) over(
                                order by
                                    p.Ngay,
                                    p.Gio,
                                    p.MaGhe
                            ) = p.MaGhe
                            and LAG(p.Ngay) over(
                                order by
                                    p.Ngay,
                                    p.Gio,
                                    p.MaGhe
                            ) = p.Ngay then datediff(
                                MINUTE,
                                LAG(p.Gio) over(
                                    order by
                                        p.Ngay,
                                        p.Gio,
                                        p.Maghe
                                ),
                                p.Gio
                            )
                            else 0
                        end
                    ) as Minute,
                    p.STT,
                    p.Ngay,
                    p.Gio,
                    p.MaGhe,
                    p.TenAo
                from
                    PhieuCanCaGiongVungNuoiDaiThanhSide p
                where
                    p.Ngay >= @fromDate
                    and p.Ngay <= @toDate
            ) p
    ) p
WHERE
    p.lag = 1
order by
    Ngay,
    TenAo,
    MaGhe,
    Gio;

Select
    p.TenCongDoan,
    p.TenThongKeDauAo,
    p.MaMayCan,
    p.Ngay,
    p.TenAo,
    p.TenLoaiCa,
    p.TenChuAo,
    p.MaGhe,
    ma.TrongLuong as TrongLuongTrenPhieu,
    ma.SoLuong / ma.TrongLuongDonVi as TrongLuongMau,
    ma.TrongLuong - Sum(p.TrongLuong) as LuongMat,
    CASE
        WHEN Ma.TrongLuong > 0 THEN (ma.TrongLuong - Round(Sum(p.TrongLuong), 0)) / Ma.TrongLuong
        ELSE 0
    END as TyLeMat,
    Sum(p.TrongLuong) as TrongLuongCan,
    Round(Sum(p.TrongLuong), 0) * (ma.SoLuong / ma.TrongLuongDonVi) as SoCon
from
    PhieuCanCaGiongVungNuoiDaiThanhSide p,
    MaMauCaGiongVungNuoi ma
where
    p.Ngay >= @fromDate
    and p.Ngay <= @toDate
    and ma.Ngay >= @fromDate
    and ma.Ngay <= @toDate
    and p.MaGhe = ma.MaGhe
    and p.Ngay = ma.Ngay
    and not exists (
        select
            *
        from
            #temp_cagiong where #temp_cagiong.Ngay = p.Ngay
            and #temp_cagiong.MaGhe =p.MaGhe )
        group by
            p.Ngay,
            ma.TrongLuong,
            ma.SoLuong,
            ma.TrongLuong,
            ma.TrongLuongDonVi,
            p.TenCongDoan,
            p.TenThongKeDauAo,
            p.MaMayCan,
            p.Ngay,
            p.TenAo,
            p.TenLoaiCa,
            p.TenChuAo,
            p.MaGhe
        UNION
        Select
            p.TenCongDoan,
            p.TenThongKeDauAo,
            p.MaMayCan,
            p.Ngay,
            p.TenAo,
            p.TenLoaiCa,
            p.TenChuAo,
            p.MaGhe,
            ma.TrongLuong as TrongLuongTrenPhieu,
            ma.SoLuong / ma.TrongLuongDonVi as TrongLuongMau,
            ma.TrongLuong - Sum(p.TrongLuong) as LuongMat,
            CASE
                WHEN Ma.TrongLuong > 0 THEN (ma.TrongLuong - Round(Sum(p.TrongLuong), 0)) / Ma.TrongLuong
                ELSE 0
            END as TyLeMat,
            Sum(
                case
                    when p.Gio < temp.Gio then p.TrongLuong
                    else 0
                end
            ) as TrongLuongCan,
            Round(Sum(p.TrongLuong), 0) * (ma.SoLuong / ma.TrongLuongDonVi) as SoCon
        from
            PhieuCanCaGiongVungNuoiDaiThanhSide p,
            MaMauCaGiongVungNuoi ma,
            #temp_cagiong temp
        where
            p.Ngay >= @fromDate
            and p.Ngay <= @toDate
            and ma.Ngay >= @fromDate
            and ma.Ngay <= @toDate
            and p.MaGhe = ma.MaGhe
            and p.Ngay = ma.Ngay
            and p.Ngay = temp.Ngay
            and p.MaGhe = temp.MaGhe
and p.TenAo = temp.TenAo
        group by
            p.Ngay,
            ma.TrongLuong,
            ma.SoLuong,
            ma.TrongLuong,
            ma.TrongLuongDonVi,
            p.TenCongDoan,
            p.TenThongKeDauAo,
            p.MaMayCan,
            p.Ngay,
            p.TenAo,
            p.TenLoaiCa,
            p.TenChuAo,
            p.MaGhe
        UNION
        Select
            p.TenCongDoan,
            p.TenThongKeDauAo,
            p.MaMayCan,
            p.Ngay,
            p.TenAo,
            p.TenLoaiCa,
            p.TenChuAo,
            p.MaGhe,
            ma.TrongLuong as TrongLuongTrenPhieu,
            ma.SoLuong / ma.TrongLuongDonVi as TrongLuongMau,
            ma.TrongLuong - Sum(p.TrongLuong) as LuongMat,
            CASE
                WHEN Ma.TrongLuong > 0 THEN (ma.TrongLuong - Round(Sum(p.TrongLuong), 0)) / Ma.TrongLuong
                ELSE 0
            END as TyLeMat,
            Sum(
                case
                    when p.Gio >= temp.Gio then p.TrongLuong
                    else 0
                end
            ) as TrongLuongCan,
            Round(Sum(p.TrongLuong), 0) * (ma.SoLuong / ma.TrongLuongDonVi) as SoCon
        from
            PhieuCanCaGiongVungNuoiDaiThanhSide p,
            MaMauCaGiongVungNuoi ma,
            #temp_cagiong temp
        where
            p.Ngay >= @fromDate
            and p.Ngay <= @toDate
            and ma.Ngay >= @fromDate
            and ma.Ngay <= @toDate
            and p.MaGhe = ma.MaGhe
            and p.Ngay = ma.Ngay
            and p.Ngay = temp.Ngay
            and p.MaGhe = temp.MaGhe
and p.TenAo = temp.TenAo
        group by
            p.Ngay,
            ma.TrongLuong,
            ma.SoLuong,
            ma.TrongLuong,
            ma.TrongLuongDonVi,
            p.TenCongDoan,
            p.TenThongKeDauAo,
            p.MaMayCan,
            p.Ngay,
            p.TenAo,
            p.TenLoaiCa,
            p.TenChuAo,
            p.MaGhe
        order by
            p.Ngay,
            p.MaGhe";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
