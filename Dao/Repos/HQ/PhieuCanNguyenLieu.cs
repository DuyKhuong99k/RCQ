using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanNguyenLieu
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanNguyenLieu";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhieuCanNguyenLieu]
      WHERE [MaMayTinhCan] = @MaMayTinhCan 
      and [MaUserCan] = @MaUserCan 
      and [ThoiGianCan] = @ThoiGianCan";

        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanNguyenLieu]
           ([MaMayTinhCan]
           ,[MaUserCan]
           ,[ThoiGianCan]
           ,[Ngay]
           ,[NhaCC]
           ,[MSL]
           ,[MaAo]
           ,[MaPhuongTien]
           ,[MaLoaiCa]
           ,[MaLoaiThanhPham]
           ,[MaSize]
           ,[MaMau]
           ,[MaBanCatTiet]
           ,[TrongLuong]
           ,[SuDung]
           ,[GhiChu]
           ,[MaXuongSanXuat],[TyLeNuoc],[TrongLuongOrg],[TrongLuongTare],[Pheu],[Chuyen],[Id])
     VALUES
           (@MaMayTinhCan 
           ,@MaUserCan 
           ,@ThoiGianCan 
           ,@Ngay 
           ,@NhaCC 
           ,@MSL 
           ,@MaAo 
           ,@MaPhuongTien 
           ,@MaLoaiCa 
           ,@MaLoaiThanhPham 
           ,@MaSize 
           ,@MaMau 
           ,@MaBanCatTiet 
           ,@TrongLuong 
           ,@SuDung 
           ,@GhiChu 
           ,@MaXuongSanXuat,@TyLeNuoc,@TrongLuongOrg,@TrongLuongTare,@Pheu,@Chuyen,@Id)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanNguyenLieu]
   SET [Ngay] = @Ngay 
      ,[NhaCC] = @NhaCC 
      ,[MSL] = @MSL 
      ,[MaAo] = @MaAo 
      ,[MaPhuongTien] = @MaPhuongTien 
      ,[MaLoaiCa] = @MaLoaiCa 
      ,[MaLoaiThanhPham] = @MaLoaiThanhPham 
      ,[MaSize] = @MaSize 
      ,[MaMau] = @MaMau 
      ,[MaBanCatTiet] = @MaBanCatTiet 
      ,[TrongLuong] = @TrongLuong 
      ,[SuDung] = @SuDung 
      ,[GhiChu] = @GhiChu 
      ,[MaXuongSanXuat] = @MaXuongSanXuat,
[TyLeNuoc] = @TyLeNuoc, [TrongLuongOrg] = @TrongLuongOrg,[TrongLuongTare] =@TrongLuongTare,[Pheu] = @Pheu, [Chuyen] =@Chuyen
 WHERE [MaMayTinhCan] = @MaMayTinhCan 
      and [MaUserCan] = @MaUserCan 
      and [ThoiGianCan] = @ThoiGianCan";

        private readonly string qrGetAll = "Select * from PhieuCanNguyenLieu";
        private readonly string prGetTongQuan = @"
SELECT
    N'Nguyên Liệu' as KV,
    SUM(TrongLuong) as TLVao,
    (
        (
            Select
                isnull(sum(TrongLuongTra), 0) as TrongLuong
            from
                PhieuCanTPDinhHinh p
            where
                p.[Ngay] <= @toDate
                and p.Ngay >= @fromDate
        ) + (
            Select
                isnull (Sum(p.TrongLuong), 0) as TrongLuong
            from
                PhieuCanTPFillet p,
                MaThanhPhamFillet tp
            where
                p.[Ngay] <= @toDate
                and p.Ngay >= @fromDate
                and tp.Ma = p.MaLoaiThanhPham
                and tp.IsGiaoXepKhuon = 1
        )
    ) as TLRa
from
    (
        Select
            p.MaLoaiThanhPham as MaThanhPham,
            p.TenThanhPham as ThanhPhamName,
            p.TrongLuong,
            p.MaXuong,
            case
                when p.TrongLuong > 0 then 0
                else 1
            end as IsTare
        from
            (
                SELECT
                    p.[Ngay],
                    p.[MSL] as MaLo,
                    p.MaLoaiThanhPham,
                    tp.[Ten] as TenThanhPham,
                    p.MaSize,
                    s.Ten as TenSize,
                    p.MaXuongSanXuat as MaXuong,
                    sum(p.TrongLuong) as TrongLuong
                FROM
                    [PhieuCanNguyenLieu] p,
                    [MaThanhPhamNguyenLieu] tp,
                    [MaSizeNguyenLieu] s
                WHERE
                    p.[SuDung] = 'True'
                    and p.[TrongLuong] > 21
                    and p.[Ngay] <= @toDate
                    and p.Ngay >= @fromDate
                    and p.MaLoaiThanhPham = tp.Ma
                    and p.MaSize = s.Ma
                    and tp.IsPhuPhamCaTap = 0
                GROUP BY
                    p.[Ngay],
                    p.[MSL],
                    tp.[Ten],
                    s.[Ten],
                    p.[MaXuongSanXuat],
                    p.MaLoaiThanhPham,
                    p.MaSize
                union
                all
                SELECT
                    top 1 p.[Ngay],
                    p.MaLo,
                    p.MaLoaiThanhPham,
                    p.TenThanhPham,
                    p.MaSize,
                    p.TenSize,
                    p.MaXuongSanXuat as MaXuong,
                    isNull(p.TrongLuong, 0) as TrongLuong
                FROM
                    (
                        SELECT
                            top 1 p.Ngay,
                            CAST(
                                CONVERT(VARCHAR(19), p.ThoiGianCan, 120) AS DATETIME
                            ) AS ThoiGian,
                            p.MaPhuongTien,
                            p.Chuyen,
                            pt.Ten AS TenPhuongTien,
                            ncc.Ten AS TenNCC,
                            p.MSL AS MaLo,
                            p.MaAo,
                            la.Ten AS TenLoaiCa,
                            tp.Ma as MaLoaiThanhPham,
                            tp.Ten AS TenThanhPham,
                            p.MaSize,
                            s.Ten AS TenSize,
                            mau.Ten AS TenMau,
                            SoPhieuKhongTareThung * TLTBTareThung * -1 as TrongLuong,
                            p.TrongLuongTare AS TrongLuongTare,
                            p.TyLeNuoc AS TyLeNuoc,
                            p.TrongLuongOrg AS TrongLuongOrg,
                            p.Pheu AS Pheu,
                            p.MaBanCatTiet,
                            bct.Ten AS TenBanCatTiet,
                            p.MaMayTinhCan,
                            p.MaXuongSanXuat,
                            x.Ten AS TenXuong
                        FROM
                            (
                                SELECT
                                    sum(
                                        CASE
                                            WHEN p.TrongLuong < 15
                                            and p.TrongLuong > 10 THEN 2
                                            WHEN p.TrongLuong < 10 THEN 1
                                            else 0
                                        END
                                    ) AS SoPhieuTareThung,
                                    sum(
                                        CASE
                                            WHEN p.TrongLuong > 21 THEN 1
                                            else 0
                                        END
                                    ) AS SoPhieuKhongTareThung,
                                    SUM(
                                        CASE
                                            WHEN p.TrongLuong < 15 THEN p.TrongLuong
                                            else 0
                                        END
                                    ) AS TLTareThung,
                                    SUM(
                                        CASE
                                            WHEN p.TrongLuong > 21 THEN p.TrongLuong
                                            else 0
                                        END
                                    ) AS TLKhongTareThung,
                                    SUM(
                                        CASE
                                            WHEN p.TrongLuong < 15 THEN p.TrongLuong
                                            else 0
                                        END
                                    ) / NULLIF(
                                        SUM(
                                            CASE
                                                WHEN p.TrongLuong < 15
                                                AND p.TrongLuong > 10 THEN 2
                                                WHEN p.TrongLuong < 10 THEN 1
                                                ELSE 0
                                            END
                                        ),
                                        0
                                    ) AS TLTBTareThung
                                FROM
                                    (
                                        Select
                                            p.*
                                        from
                                            PhieuCanNguyenLieu p,
                                            MaThanhPhamNguyenLieu tp
                                        where
                                            p.[Ngay] <= @toDate
                                            and p.Ngay >= @fromDate
                                            and p.MaLoaiThanhPham = tp.Ma
                                            and tp.IsPhuPhamCaTap = 0
                                            and tp.IsSNL =1
                                    ) p
                                WHERE
                                    p.SuDung = 'True'
                                    AND p.TrongLuong > 5
                                    AND p.Ngay BETWEEN @fromDate
                                    AND @toDate
                            ) AS DataTareThung,
                            (
                                Select
                                    top(1) p.*
                                from
                                    PhieuCanNguyenLieu p,
                                    MaThanhPhamNguyenLieu tp
                                where
                                    p.[Ngay] <= @toDate
                                    and p.Ngay >= @fromDate
                                    and p.MaLoaiThanhPham = tp.Ma
                                    and tp.IsPhuPhamCaTap = 0
                            ) p
                            left JOIN NhaCungCapNguyenLieu ncc ON p.NhaCC = ncc.Ma
                            left JOIN MaLoaiCaNguyenLieu la ON p.MaLoaiCa = la.Ma
                            left JOIN MaSizeNguyenLieu s ON p.MaSize = s.Ma
                            left JOIN MaMauNguyenLieu mau ON p.MaMau = mau.Ma
                            left JOIN PhuongTienChoNguyenLieu pt ON p.MaPhuongTien = pt.Ma
                            left JOIN BanCatTiet bct ON p.MaBanCatTiet = bct.Ma
                            left JOIN XiNghiep x ON p.MaXuongSanXuat = x.Ma
                            left join (
                                select
                                    top(1) *
                                from
                                    MaThanhPhamNguyenLieu tp
                                where
                                    tp.IsTareThung = 1
                            ) tp on tp.Ma = tp.Ma
                        where
                            p.[SuDung] = 'True'
                            and p.[TrongLuong] > 0
                    ) p
            ) p
    ) p
UNION
ALL
SELECT
    N'Fillet' as KV,
    Sum(p.TrongLuongNhan) as TLVao,
    SUM(p.TrongLuongTra) as TLRa
from
    PhieuCanTPFilletv2 p
    Left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
where
    Ngay >= @fromDate
    and Ngay <= @toDate and tp.IsNguyenCon = 0
UNION
ALL
SELECT
    N'Xẻ Bướm' as KV,
    Sum(
        case
            when IsNguyenLieuXeBuom = 1
            and IsXeBuom = 1 then TrongLuong
            else 0
        end
    ) as TLVao,
    SUM(
        case
            when IsNguyenLieuXeBuom = 0
            and IsXeBuom = 1
            and IsDat = 0 and IsGiaoXepKhuon = 0 then TrongLuong
            when IsNguyenLieuXeBuom = 0
            and IsXeBuom = 1
            and IsDat = 1 and IsGiaoXepKhuon = 0 then -1 * TrongLuong
            
            else 0
        end
    ) as TLRa
from
    (
        Select
            tp.IsNguyenLieuXeBuom,
            tp.IsXeBuom,
            tp.IsGiaoXepKhuon,
            tp.IsDat,
            Sum(p.TrongLuong) as TrongLuong
        from
            PhieuCanTPFillet p,
            MaThanhPhamFillet tp
        where
            Ngay >= @fromDate
            and Ngay <= @toDate
            and p.MaLoaiThanhPham = tp.Ma
        GROUP BY
            tp.IsNguyenLieuXeBuom,
            tp.IsXeBuom,
            tp.IsGiaoXepKhuon,
            tp.IsDat
    ) p
UNION
ALL
Select
    N'Sửa Cá' as KV,
    Sum(TrongLuongNhan) as TLVao,
    Sum(TrongLuongTra) as TLRa
from
    PhieuCanTPDinhHinh
where
    Ngay >= @fromDate
    and Ngay <= @toDate";

        public PhieuCanNguyenLieu(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

        }

        public List<T> GetTongQuans<T>(DateTime fromDate, DateTime toDate)
        {
            var query = string.Format(prGetTongQuan, fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"));
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { fromDate, toDate })
                .ToList();
            return items;
        }
        public List<Tuple<string, DateTime>> GetsAo(string xuongId, int topVal)
        {
            var query =
                "SELECT Top (@topVal) MaAo as Item1, Max(ThoiGianCan) as Item2 FROM PhieuCanNguyenLieu Where MaXuongSanXuat = @xuongId GROUP BY MaAo ORDER BY Max(ThoiGianCan) DESC";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<Tuple<string, DateTime>>(query, new { xuongId, topVal })
                .ToList();
            return items;
        }
        public List<string> GetsMSLDinhHinh(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanTPDinhHinh where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls group p by new { p.MSL } into g select new { g.Key.MSL }).Distinct(
                    )
                    .ToList();
                var itemsNeeds = new List<string>();
                foreach (var item in items) itemsNeeds.Add(item.MSL);

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<string> GetsMSLTPFillet(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(ThoiGianCan) as datetime) as ThoiGianCan,MSL as MSL,MaSize from PhieuCanTPFillet where MaXuongSanXuat=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MSL,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls group p by new { p.MSL } into g select new { g.Key.MSL }).Distinct(
                    )
                    .ToList();
                var itemsNeeds = new List<string>();
                foreach (var item in items) itemsNeeds.Add(item.MSL);

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<string> GetsMSLDinhHinh_BTP(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanBTPDinhHinh where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls group p by new { p.MSL } into g select new { g.Key.MSL }).Distinct(
                    )
                    .ToList();
                var itemsNeeds = new List<string>();
                foreach (var item in items) itemsNeeds.Add(item.MSL);

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<string> GetsMSLBTPFilletv2(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanBTPFilletv2 where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls group p by new { p.MSL } into g select new { g.Key.MSL }).Distinct(
                    )
                    .ToList();
                var itemsNeeds = new List<string>();
                foreach (var item in items) itemsNeeds.Add(item.MSL);

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<string> GetsMSL(string xuongId)
        {
            try
            {
                var query =
                    "Select  Ngay,MAX( ThoiGianCan) as ThoiGianCan ,MSL,MaSize from PhieuCanNguyenLieu where MaXuongSanXuat=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MSL,Ngay,MaSize order by ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls group p by new { p.MSL } into g select new { g.Key.MSL }).Distinct(
                    )
                    .ToList();
                var itemsNeeds = new List<string>();
                foreach (var item in items) itemsNeeds.Add(item.MSL);

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsLastMSLWithSizeDinhHinh(string xuongId)
        {
            try
            {
                var query =
                    "Select top(1) Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanTPDinhHinh where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsLastMSLWithSizeCaoThit(string xuongId)
        {
            try
            {
                var query =
                    "Select top(1) Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanCaoThit where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsLastMSLWithSizeTPFillet(string xuongId)
        {
            try
            {
                var query =
                    "Select top(1) Ngay,Cast(max(ThoiGianCan) as datetime) as ThoiGianCan,MSL as MSL,MaSize from PhieuCanTPFillet where MaXuongSanXuat=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MSL,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsLastMSLWithSizeBTPFilletv2(string xuongId)
        {
            try
            {
                var query =
                    "Select top(1) Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanBTPFilletv2 where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsLastMSLWithSizeDinhHinh_BTP(string xuongId)
        {
            try
            {
                var query =
                    "Select top(1) Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanBTPDinhHinh where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsLastMSLWithSize(string xuongId, int top = 4)
        {
            try
            {
                var query =
                    $@"SELECT Top {top} Cast( Max(ThoiGianCan)  as Date) as Item1, MSL as Item2,MaSize as Item3  FROM PhieuCanNguyenLieu Where MaXuongSanXuat = @xuongId GROUP BY MSL,MaSize ORDER BY Max(ThoiGianCan) DESC";

                //var query =
                //    "Select  Ngay,MAX( ThoiGianCan) as ThoiGianCan ,MSL,MaSize from PhieuCanNguyenLieu where MaXuongSanXuat=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MSL,Ngay,MaSize order by ThoiGianCan DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                //var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId = xuongId }).Result.ToList();
                //var items = (from p in itemrls
                //             group p by new { p.MSL, p.MaSize, p.Ngay }into g
                //             select new { MSL = g.Key.MSL, MaSize = g.Key.MaSize, Ngay = g.Key.Ngay }).Distinct()
                //    .ToList();
                //List<Tuple<DateTime?, string, string>> itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                //foreach(var item in items)
                //{
                //    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));
                //}

                //return itemsNeeds;
                var items = connection.Query<Tuple<DateTime?, string, string>>(query, new { xuongId })
                    .ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsMSLWithSizeDinhHinh(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanTPDinhHinh where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsMSLWithSizeCaoThit(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanCaoThit where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsMSLWithSizeDinhHinh_BTP(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanBTPDinhHinh where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsMSLWithSizeTPFillet(string xuongId)
        {
            try
            {
                var query =
                    @"Select Ngay,Cast(max(ThoiGianCan) as datetime) as ThoiGianCan,MSL as MSL,MaSize from PhieuCanTPFillet where MaXuongSanXuat=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MSL,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));

                return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsMSLWithSizeBTPFilletv2(string xuongId)
        {
            try
            {
                var query =
                    "Select Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanBTPFilletv2 where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId }).Result.ToList();
                var items = (from p in itemrls
                             group p by new { p.MSL, p.MaSize, p.Ngay }
                        into g
                             select new { g.Key.MSL, g.Key.MaSize, g.Key.Ngay }).Distinct()
                    .ToList();
                var itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                foreach (var item in items)
                    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));
                return itemsNeeds;
                //var query =
                //    @"Select  Ngay,Cast(max(Gio) as datetime) as ThoiGianCan,MaLo as MSL,MaSize from PhieuCanBTPFilletv2 where MaXuong=@xuongId and DATEDIFF(day, Ngay, GETDATE()) < 365 and Ngay IS NOT NULL Group By MaLo,Ngay,MaSize order by Ngay DESC,ThoiGianCan DESC";
                //using var connection = new SqlConnection(ConnectionString);
                //connection.Open();
                //var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId = xuongId }).Result.ToList();
                //var items = (from p in itemrls
                //             group p by new { p.MSL, p.MaSize, p.Ngay } into g
                //             select new { MSL = g.Key.MSL, MaSize = g.Key.MaSize, Ngay = g.Key.Ngay }).Distinct()
                //    .ToList();
                //List<Tuple<DateTime?, string, string>> itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                //foreach (var item in items)
                //{
                //    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));
                //}

                //return itemsNeeds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsMSLWithSize(string xuongId)
        {
            try
            {
                var query =
                    @"SELECT Top 10 Cast( Max(ThoiGianCan)  as Date) as Item1, MSL as Item2,MaSize as Item3  FROM PhieuCanNguyenLieu Where MaXuongSanXuat = @xuongId GROUP BY MSL,MaSize ORDER BY Max(ThoiGianCan) DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                //var itemrls = connection.QueryAsync<ResultMSL>(query, new { xuongId = xuongId }).Result.ToList();
                //var items = (from p in itemrls
                //             group p by new { p.MSL, p.MaSize, p.Ngay }into g
                //             select new { MSL = g.Key.MSL, MaSize = g.Key.MaSize, Ngay = g.Key.Ngay }).Distinct()
                //    .ToList();
                //List<Tuple<DateTime?, string, string>> itemsNeeds = new List<Tuple<DateTime?, string, string>>();
                //foreach(var item in items)
                //{
                //    itemsNeeds.Add(new Tuple<DateTime?, string, string>(item.Ngay, item.MSL, item.MaSize));
                //}

                //return itemsNeeds;
                var items = connection.Query<Tuple<DateTime?, string, string>>(query, new { xuongId })
                    .ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
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

        public T? Get<T>(string id)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>("select * from PhieuCanNguyenLieu where Id = @id", new { id }).FirstOrDefault();
            return rows;
        }
        public List<T> Gets<T>(DateTime dateTime)
        {
            var query = @"select p.*,Isnull (tp.Ten,'') as ThanhPhamName from (Select * from PhieuCanNguyenLieu Where Ngay = @ngay) p left join MaThanhPhamNguyenLieu tp on p.MaLoaiThanhPham = tp.Ma ";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
            return items;
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
        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var query = @"
WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MaMayTinhCan ORDER BY Ngay DESC, ThoiGianCan DESC) AS RowNum
    FROM PhieuCanNguyenLieu where Ngay =@ngay
)

SELECT *
FROM RankedPhieu
WHERE RowNum <= @num ";
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
        public List<T> GetTongHopThanhPhamDashBoard<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            //            var query = @"
            //select
            //p.MaLoaiThanhPham as MaThanhPham,
            //tp.Ten as ThanhPhamName,
            //Sum(p.TrongLuong) as TrongLuong,
            //p.MaXuongSanXuat as MaXuong
            //from
            //PhieuCanNguyenLieu p,
            //MaThanhPhamNguyenLieu tp
            //where
            //p.Ngay >= @fromDate
            //and p.Ngay <= @toDate
            //and p.MaXuongSanXuat = @xuongId
            //and p.MaLoaiThanhPham = tp.Ma
            //group by
            //p.MaLoaiThanhPham,
            //tp.Ten,
            //p.MaXuongSanXuat 
            //";
            //var query = @"
            //select
            //p.MaLoaiThanhPham as MaThanhPham,
            //tp.Ten as ThanhPhamName,
            //Sum(p.TrongLuong) as TrongLuong,
            //p.MaXuongSanXuat as MaXuong
            //from
            //PhieuCanNguyenLieu p,
            //MaThanhPhamNguyenLieu tp
            //where
            //p.Ngay >= @fromDate
            //and p.Ngay <= @toDate
            //and p.MaXuongSanXuat = @xuongId
            //and p.MaLoaiThanhPham = tp.Ma
            //group by
            //p.MaLoaiThanhPham,
            //tp.Ten,
            //p.MaXuongSanXuat 

            //union all

            //select top 1
            //p.MaLoaiThanhPham as MaThanhPham,
            //tp.Ten as ThanhPhamName,
            //SoPhieuKhongTareThung * TLTBTareThung * -1 as TrongLuong,  
            //p.MaXuongSanXuat as MaXuong
            //from
            //(    
            //			SELECT   
            //			COUNT(CASE WHEN tp.IsTareThung = 'True' THEN 1 END) AS SoPhieuTareThung,  
            //			COUNT(CASE WHEN tp.IsTareThung = 'False' THEN 1 END) AS SoPhieuKhongTareThung,  
            //			SUM(CASE WHEN tp.IsTareThung = 'True' THEN p.TrongLuong ELSE 0 END) AS TLTareThung,  
            //			SUM(CASE WHEN tp.IsTareThung = 'False' THEN p.TrongLuong ELSE 0 END) AS TLKhongTareThung,  
            //			SUM(CASE WHEN tp.IsTareThung = 'True' THEN p.TrongLuong ELSE 0 END) /  COUNT(CASE WHEN tp.IsTareThung = 'True' THEN 1 END) AS TLTBTareThung
            //			FROM 
            //			PhieuCanNguyenLieu p  
            //			INNER JOIN MaThanhPhamNguyenLieu tp ON p.MaLoaiThanhPham = tp.Ma  
            //			WHERE   
            //			p.SuDung = 'True'   
            //			AND p.TrongLuong > 0   
            //			AND p.Ngay BETWEEN @fromDate AND @toDate
            //        ) AS DataTareThung,  
            //PhieuCanNguyenLieu p,
            //MaThanhPhamNguyenLieu tp
            //where

            //p.Ngay >= @fromDate
            //and p.Ngay <= @toDate
            //and p.MaXuongSanXuat = @xuongId
            //and p.MaLoaiThanhPham = tp.Ma
            //and tp.IsTareThung = 'true'
            //group by
            //p.MaLoaiThanhPham,
            //tp.Ten,
            //p.MaXuongSanXuat ,SoPhieuKhongTareThung,TLTBTareThung
            //";
            var query = @"
Select
   p.MaLoaiThanhPham as MaThanhPham,
   p.TenThanhPham as ThanhPhamName,
   --p.TrongLuong,
   CASE WHEN p.IsDatNho = 1 THEN p.TrongLuong * -1 ELSE p.TrongLuong END AS TrongLuong,
   p.MaXuong,
   case when p.TrongLuong > 0 then 0 else 1 end as IsTare
from
    (
        SELECT
            p.[Ngay],
            p.[MSL] as MaLo,
            p.MaLoaiThanhPham,
            tp.[Ten] as TenThanhPham,
            p.MaSize,
            s.Ten as TenSize,
            p.MaXuongSanXuat as MaXuong,
            sum(p.TrongLuong) as TrongLuong,
			tp.IsDatNho
        FROM
            [PhieuCanNguyenLieu] p,
            [MaThanhPhamNguyenLieu] tp,
            [MaSizeNguyenLieu] s
        WHERE
            p.[SuDung] = 'True'
            and p.[Ngay] <= @toDate
             and p.Ngay >= @fromDate
            and p.MaLoaiThanhPham = tp.Ma
            and p.MaSize = s.Ma
        and tp.IsPhuPhamCaTap = 0
		and (( p.[TrongLuong] > 21 and tp.IsSNL = 1)or(tp.IsSNL =0))
        GROUP BY
            p.[Ngay],
            p.[MSL],
            tp.[Ten],
            s.[Ten],
            p.[MaXuongSanXuat],
            p.MaLoaiThanhPham,
            p.MaSize,
			tp.IsDatNho
        union
        all
        SELECT
            top 1 p.[Ngay],
            p.MaLo,
            p.MaLoaiThanhPham,
            p.TenThanhPham,
            p.MaSize,
            p.TenSize,
            p.MaXuongSanXuat as MaXuong,
         isNull(p.TrongLuong,0) as TrongLuong,
            p.IsDatNho
        FROM
            (
                SELECT
                    top 1 p.Ngay,
                    CAST(
                        CONVERT(VARCHAR(19), p.ThoiGianCan, 120) AS DATETIME
                    ) AS ThoiGian,
                    p.MaPhuongTien,
                    p.Chuyen,
                    pt.Ten AS TenPhuongTien,
                    ncc.Ten AS TenNCC,
                    p.MSL AS MaLo,
                    p.MaAo,
                    la.Ten AS TenLoaiCa,
                    tp.Ma as MaLoaiThanhPham,
                    tp.Ten AS TenThanhPham,
                    p.MaSize,
                    s.Ten AS TenSize,
                    mau.Ten AS TenMau,
                    SoPhieuKhongTareThung * TLTBTareThung * -1 as TrongLuong,
                    p.TrongLuongTare AS TrongLuongTare,
                    p.TyLeNuoc AS TyLeNuoc,
                    p.TrongLuongOrg AS TrongLuongOrg,
                    p.Pheu AS Pheu,
                    p.MaBanCatTiet,
                    bct.Ten AS TenBanCatTiet,
                    p.MaMayTinhCan,
                    p.MaXuongSanXuat,
                    x.Ten AS TenXuong,
					tp.IsDatNho
                FROM
                    (
                        SELECT
                            sum(
                                CASE
                                    WHEN p.TrongLuong < 15
                                    and p.TrongLuong > 10 THEN 2
                                    WHEN p.TrongLuong < 10 THEN 1
                                    else 0
                                END
                            ) AS SoPhieuTareThung,
                            sum(
                                CASE
                                    WHEN p.TrongLuong > 21 THEN 1
                                    else 0
                                END
                            ) AS SoPhieuKhongTareThung,
                            SUM(
                                CASE
                                    WHEN p.TrongLuong < 15 THEN p.TrongLuong
                                    else 0
                                END
                            ) AS TLTareThung,
                            SUM(
                                CASE
                                    WHEN p.TrongLuong > 21 THEN p.TrongLuong
                                    else 0
                                END
                            ) AS TLKhongTareThung,
                            SUM(
                                CASE
                                    WHEN p.TrongLuong < 15 THEN p.TrongLuong
                                    else 0
                                END
                            ) / NULLIF(
                        SUM(
                            CASE
                                WHEN p.TrongLuong < 15 AND p.TrongLuong > 10 THEN 2
                                WHEN p.TrongLuong < 10 THEN 1
                                ELSE 0
                            END
                        ), 0
                    ) AS TLTBTareThung
                        FROM
                            (
                                Select
                                    p.*
                                from
                                    PhieuCanNguyenLieu p,
                                    MaThanhPhamNguyenLieu tp
                                where
                                    p.[Ngay] <= @toDate
                                    and p.Ngay >= @fromDate
                                    and p.MaLoaiThanhPham = tp.Ma
                                    and tp.IsPhuPhamCaTap = 0
                                     and tp.IsSNL =1
                            ) p
                        WHERE
                            p.SuDung = 'True'
                            AND p.TrongLuong > 5
                            AND p.Ngay BETWEEN @fromDate
                            AND @toDate
                    ) AS DataTareThung,
                    (
                        Select
                            top(1) p.*
                        from
                            PhieuCanNguyenLieu p,
                            MaThanhPhamNguyenLieu tp
                        where
                            p.[Ngay] <= @toDate
                            and p.Ngay >= @fromDate
                            and p.MaLoaiThanhPham = tp.Ma
                            and tp.IsPhuPhamCaTap = 0
                    ) p
                    left JOIN NhaCungCapNguyenLieu ncc ON p.NhaCC = ncc.Ma
                    left JOIN MaLoaiCaNguyenLieu la ON p.MaLoaiCa = la.Ma
                    left JOIN MaSizeNguyenLieu s ON p.MaSize = s.Ma
                    left JOIN MaMauNguyenLieu mau ON p.MaMau = mau.Ma
                    left JOIN PhuongTienChoNguyenLieu pt ON p.MaPhuongTien = pt.Ma
                    left JOIN BanCatTiet bct ON p.MaBanCatTiet = bct.Ma
                    left JOIN XiNghiep x ON p.MaXuongSanXuat = x.Ma
                    left join (
                        select
                            top(1) *
                        from
                            MaThanhPhamNguyenLieu tp
                        where
                            tp.IsTareThung = 1
                    ) tp on tp.Ma = tp.Ma
                where
                    p.[SuDung] = 'True'
                    and p.[TrongLuong] > 0
            ) p
    ) p

";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.QueryAsync<T>(query, new { fromDate, toDate, xuongId }).Result
                .ToList();
            return items;
        }
        private class ResultMSL
        {
            public string MaSize { get; set; }

            public string MSL { get; set; }

            public DateTime? Ngay { get; set; }

            public DateTime ThoiGianCan { get; set; }
        }

        #region Xử lý phiếu CÂN
        public List<T> GetPhieuCan<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                //                var query = @"select
                //p.MaMayTinhCan,
                //p.MaUserCan,
                //p.ThoiGianCan,
                //p.Ngay,
                //p.NhaCC,
                //ncc.Ten as NhaCCName,
                //p.MSL,
                //p.MaAo,
                //ao.Ten as AoName,
                //p.MaPhuongTien,
                //pt.Ten as PhuongTienName,
                //p.MaLoaiCa,
                //lc.Ten as LoaiCaName,
                //p.MaLoaiThanhPham,
                //tp.Ten as ThanhPhamName,
                //p.MaSize,
                //s.Ten as SizeName,
                //p.MaMau,
                //m.Ten as MauName,
                //p.MaBanCatTiet,
                //bct.Ten as BanCatTietName,
                //p.TrongLuong,
                //p.SuDung,
                //p.GhiChu
                //from PhieuCanNguyenLieu p
                //left join NhaCungCapNguyenLieu ncc on p.NhaCC = ncc.Ma
                //left join MaAoVungNuoi ao on p.MaAo = ao.Ma
                //left join PhuongTienChoNguyenLieu pt on p.MaPhuongTien = pt.Ma
                //left join MaLoaiCaNguyenLieu lc on  p.MaLoaiCa = lc.Ma
                //left join MaThanhPhamNguyenLieu tp on p.MaLoaiThanhPham = tp.Ma
                //left join MaSizeNguyenLieu s on p.MaSize = s.Ma
                //left join MaMauNguyenLieu m on p.MaMau = m.Ma
                //left join BanCatTiet bct on p.MaBanCatTiet = bct.Ma
                //where p.Ngay = @dateTime and p.MaXuongSanXuat = @xuongId";
                var query = @"select
p.MaMayTinhCan,
p.MaUserCan,
p.ThoiGianCan,
p.Ngay,
p.NhaCC,
ncc.Ten as NhaCCName,
p.MSL,
p.MaAo,
ao.Ten as AoName,
p.MaPhuongTien,
pt.Ten as PhuongTienName,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaLoaiThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
--p.MaBanCatTiet,
--bct.Ten as BanCatTietName,
p.TrongLuong,
p.TrongLuongTare,
p.SuDung,
p.GhiChu,
p.TyLeNuoc
from PhieuCanNguyenLieu p
left join NhaCungCapNguyenLieu ncc on p.NhaCC = ncc.Ma
left join Ao ao on p.MaAo = ao.Ma
left join PhuongTienChoNguyenLieu pt on p.MaPhuongTien = pt.Ma
left join MaLoaiCaNguyenLieu lc on  p.MaLoaiCa = lc.Ma
left join MaThanhPhamNguyenLieu tp on p.MaLoaiThanhPham = tp.Ma
left join MaSizeNguyenLieu s on p.MaSize = s.Ma
left join MaMauNguyenLieu m on p.MaMau = m.Ma
--left join BanCatTiet bct on p.MaBanCatTiet = bct.Ma
where p.Ngay = @dateTime and p.MaXuongSanXuat = @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { dateTime = dateTime.Date, xuongId }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        public double GetSanLuong(TimeSpan fromTime, TimeSpan toTime, DateTime dateTime, string xuongId)
        {
            var query =
                "Select ISNULL(SUM(TrongLuong),0) from PhieuCanNguyenLieu where Ngay = @ngay and CONVERT(time,ThoiGianCan) >= @fromTime and CONVERT(time,ThoiGianCan) < @toTime and SuDung = 1 and MaXuongSanXuat = @xuongId and TrongLuong>0";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.ExecuteScalar<double>(
                query,
                new { ngay = dateTime.Date, fromTime, toTime, xuongId });
            return item;
        }
        public double GetSanLuong(DateTime dateTime, string xuongId, string banId)
        {
            var query =
                "Select ISNULL(SUM(p.TrongLuong),0) from PhieuCanNguyenLieu p where p.Ngay = @ngay and p.SuDung = 1 and p.MaXuongSanXuat = @xuongId and p.TrongLuong>0  and MaBanCatTiet =@banId";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.ExecuteScalar<double>(
                query,
                new { ngay = dateTime.Date, xuongId, banId });
            return item;
        }
        public double GetSanLuongTruNgop(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string banId,
            string sizeId)
        {
            var query =
                "Select ISNULL(SUM(p.TrongLuong),0) from PhieuCanNguyenLieu p, MaThanhPhamNguyenLieu tp where p.Ngay = @ngay and CONVERT(time,p.ThoiGianCan) >= @fromTime and CONVERT(time,p.ThoiGianCan) < @toTime and p.SuDung = 1 and p.MaXuongSanXuat = @xuongId and  p.MaBanCatTiet = @banId and p.MaSize = @sizeId and p.MaLoaiThanhPham = tp.Ma and tp.Min <> 2";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.ExecuteScalar<double>(
                query,
                new
                {
                    ngay = dateTime.Date,
                    fromTime,
                    toTime,
                    xuongId,
                    banId,
                    sizeId
                });
            return item;
        }
        public double GetSanLuongCaNgopTruBan(DateTime dateTime, string xuongId, string sizeId)
        {
            var query =
                "Select ISNULL(SUM(p.TrongLuong),0) from PhieuCanNguyenLieu p, MaThanhPhamNguyenLieu tp where p.Ngay = @ngay and p.SuDung = 1 and p.MaXuongSanXuat = @xuongId and p.TrongLuong>0 and p.MaLoaiThanhPham = tp.Ma and tp.Min=2 and p.MaBanCatTiet in ('CT01','CT02','CT03','CT04','CT05','CT06','CT07','CT08') and MaSize =@sizeId";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.ExecuteScalar<double>(
                query,
                new { ngay = dateTime.Date, xuongId, sizeId });
            return item;
        }
        public List<string> GetMSLs(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string sizeId)
        {
            var query =
                "Select Distinct MSL from PhieuCanNguyenLieu  where CONVERT(time,ThoiGianCan) between @fromTime and @toTime and Ngay =@ngay and MaXuongSanXuat = @xuongId and MaSize = @sizeId";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.Query<string>(
                    query,
                    new
                    {
                        ngay = dateTime.Date,
                        fromTime,
                        toTime,
                        xuongId,
                        sizeId
                    })
                .ToList();
            return item;
        }
        public double GetSanLuongCaNgop(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string banId)
        {
            var query =
                "Select ISNULL(SUM(p.TrongLuong),0) from PhieuCanNguyenLieu p, MaThanhPhamNguyenLieu tp where CONVERT(time,p.ThoiGianCan) >= @fromTime and CONVERT(time,p.ThoiGianCan) < @toTime and p.Ngay = @ngay and p.SuDung = 1 and p.MaXuongSanXuat = @xuongId and p.TrongLuong>0 and p.MaLoaiThanhPham = tp.Ma and tp.Min=2 and  MaBanCatTiet =@banId";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.ExecuteScalar<double>(
                query,
                new
                {
                    ngay = dateTime.Date,
                    xuongId,
                    fromTime,
                    toTime,
                    banId
                });
            return item;
        }


        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                //                var query = @"SELECT
                //    p.[Ngay],
                //    Cast(
                //        convert(
                //            varchar(19),
                //            p.ThoiGianCan,
                //            120
                //        ) as datetime
                //    ) as ThoiGian,
                //    p.[MaPhuongTien] ,
                //    p.[Chuyen],
                //    pt.Ten as TenPhuongTien ,
                //    ncc.[Ten] as TenNCC,
                //    p.[MSL] as MaLo,
                //    p.[MaAo] ,
                //    la.[Ten] as TenLoaiCa,
                //    tp.[Ten] as TenThanhPham,
                //    s.[Ten] as TenSize,
                //    mau.Ten as TenMau,
                //    p.[TrongLuong] ,
                //    p.TrongLuongTare ,
                //    p.TyLeNuoc ,
                //    p.TrongLuongOrg,
                //    p.Pheu , 
                //    p.[MaBanCatTiet],
                //	bct.Ten as TenBanCatTiet,
                //    p.MaMayTinhCan, 
                //    p.[MaXuongSanXuat],
                //	x.Ten as TenXuong
                //FROM
                //    [PhieuCanNguyenLieu] p,
                //    [NhaCungCapNguyenLieu] ncc,
                //    [MaLoaiCaNguyenLieu] la,
                //    [MaThanhPhamNguyenLieu] tp,
                //    [MaSizeNguyenLieu] s,
                //    MaMauNguyenLieu mau,
                //    PhuongTienChoNguyenLieu pt,
                //	BanCatTiet bct,
                //	XiNghiep x
                //WHERE
                //    p.[SuDung] = 'True'
                //    and p.[TrongLuong] > 0
                //    and p.[Ngay] <= @toDate
                //    and p.Ngay >= @fromDate
                //    and p.NhaCC = ncc.Ma
                //    and p.MaLoaiCa = la.Ma
                //    and p.MaLoaiThanhPham = tp.Ma
                //    and p.MaSize = s.Ma
                //    and p.MaMau = mau.Ma
                //    and p.MaPhuongTien = pt.Ma
                //	and p.MaBanCatTiet = bct.Ma
                //	and p.MaXuongSanXuat = x.Ma
                //order by
                //    ThoiGianCan";
                var query = @"   
SELECT
    p.[Ngay],
    Cast(
        convert(
            varchar(19),
            p.ThoiGianCan,
            120
        ) as datetime
    ) as ThoiGian,
    p.[MaPhuongTien],
    p.[Chuyen],
    pt.Ten as TenPhuongTien,
    ncc.[Ten] as TenNCC,
    p.[MSL] as MaLo,
    p.[MaAo],
    la.[Ten] as TenLoaiCa,
    p.MaLoaiThanhPham,
    tp.[Ten] as TenThanhPham,
    p.MaSize,
    s.[Ten] as TenSize,
    mau.Ten as TenMau,
    p.[TrongLuong],
    p.TrongLuongTare,
    p.TyLeNuoc,
    p.TrongLuongOrg,
    p.Pheu,
    p.[MaBanCatTiet],
    bct.Ten as TenBanCatTiet,
    p.MaMayTinhCan,
    p.[MaXuongSanXuat],
    x.Ten as TenXuong
FROM
    (
        Select
            p.*
        from
            PhieuCanNguyenLieu p,
            MaThanhPhamNguyenLieu tp
        where
            p.[Ngay] <= @toDate
            and p.Ngay >= @fromDate
            and p.MaLoaiThanhPham = tp.Ma
            and tp.IsPhuPhamCaTap = 0
    ) p
    left join NhaCungCapNguyenLieu ncc on ncc.Ma = p.NhaCC
    left join MaLoaiCaNguyenLieu la on la.Ma = p.MaLoaiCa
    left join MaThanhPhamNguyenLieu tp on tp.Ma = p.MaLoaiThanhPham
    left join MaSizeNguyenLieu s on s.Ma = p.MaSize
    left join MaMauNguyenLieu mau on mau.Ma = p.MaMau
    left join PhuongTienChoNguyenLieu pt on pt.Ma = p.MaPhuongTien
    left join BanCatTiet bct on bct.Ma = p.MaBanCatTiet
    left join XiNghiep x on x.Ma = p.MaXuongSanXuat
WHERE
    p.[SuDung] = 'True'
    and p.[TrongLuong] > 0
Union
all
SELECT
    top 1 p.Ngay,
    CAST(
        CONVERT(VARCHAR(19), p.ThoiGianCan, 120) AS DATETIME
    ) AS ThoiGian,
    p.MaPhuongTien,
    p.Chuyen,
    pt.Ten AS TenPhuongTien,
    ncc.Ten AS TenNCC,
    p.MSL AS MaLo,
    p.MaAo,
    la.Ten AS TenLoaiCa,
    tp.Ma as MaLoaiThanhPham,
    tp.Ten AS TenThanhPham,
    p.MaSize,
    s.Ten AS TenSize,
    mau.Ten AS TenMau,
    SoPhieuKhongTareThung * TLTBTareThung * -1 as TrongLuong,
    p.TrongLuongTare AS TrongLuongTare,
    p.TyLeNuoc AS TyLeNuoc,
    p.TrongLuongOrg AS TrongLuongOrg,
    p.Pheu AS Pheu,
    p.MaBanCatTiet,
    bct.Ten AS TenBanCatTiet,
    p.MaMayTinhCan,
    p.MaXuongSanXuat,
    x.Ten AS TenXuong
FROM
    (
        SELECT
            sum(
                CASE
                    WHEN p.TrongLuong < 15
                    and p.TrongLuong > 10 THEN 2
                    WHEN p.TrongLuong < 10 THEN 1
                    else 0
                END
            ) AS SoPhieuTareThung,
            sum(
                CASE
                    WHEN p.TrongLuong > 21 THEN 1
                    else 0
                END
            ) AS SoPhieuKhongTareThung,
            SUM(
                CASE
                    WHEN p.TrongLuong < 15 THEN p.TrongLuong
                    else 0
                END
            ) AS TLTareThung,
            SUM(
                CASE
                    WHEN p.TrongLuong > 21 THEN p.TrongLuong
                    else 0
                END
            ) AS TLKhongTareThung,
            SUM(
                CASE
                    WHEN p.TrongLuong < 15 THEN p.TrongLuong
                    else 0
                END
            ) / NULLIF(
                        SUM(
                            CASE
                                WHEN p.TrongLuong < 15 AND p.TrongLuong > 10 THEN 2
                                WHEN p.TrongLuong < 10 THEN 1
                                ELSE 0
                            END
                        ), 0
                    ) AS TLTBTareThung
        FROM
            (
                Select
                    p.*
                from
                    PhieuCanNguyenLieu p,
                    MaThanhPhamNguyenLieu tp
                where
                    p.[Ngay] <= @toDate
                    and p.Ngay >= @fromDate
                    and p.MaLoaiThanhPham = tp.Ma
                    and tp.IsPhuPhamCaTap = 0
                 and tp.IsSNL =1
            ) p
        WHERE
            p.SuDung = 'True'
            AND p.TrongLuong > 5
            AND p.Ngay BETWEEN @fromDate
            AND @toDate
    ) AS DataTareThung,
    (
        Select
            top(1) p.*
        from
            PhieuCanNguyenLieu p,
            MaThanhPhamNguyenLieu tp
        where
            p.[Ngay] <= @toDate
            and p.Ngay >= @fromDate
            and p.MaLoaiThanhPham = tp.Ma
            and tp.IsPhuPhamCaTap = 0
    ) p
    left JOIN NhaCungCapNguyenLieu ncc ON p.NhaCC = ncc.Ma
    left JOIN MaLoaiCaNguyenLieu la ON p.MaLoaiCa = la.Ma
    left JOIN MaSizeNguyenLieu s ON p.MaSize = s.Ma
    left JOIN MaMauNguyenLieu mau ON p.MaMau = mau.Ma
    left JOIN PhuongTienChoNguyenLieu pt ON p.MaPhuongTien = pt.Ma
    left JOIN BanCatTiet bct ON p.MaBanCatTiet = bct.Ma
    left JOIN XiNghiep x ON p.MaXuongSanXuat = x.Ma
    left join (select top(1) * from MaThanhPhamNguyenLieu tp where tp.IsTareThung = 1) tp  on tp.Ma = tp.Ma
where
    p.[SuDung] = 'True'
    and p.[TrongLuong] > 0";
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
        public List<T> GetTongHopNCC<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                //                var query = @"SELECT
                //    p.[Ngay],
                //    p.[MaXuongSanXuat],
                //	x.Ten as TenXuong,
                //    ncc.[Ten] as TenNCC,
                //    p.[MSL] as MaLo,
                //    p.[MaAo] as MaAo,
                //    p.[MaPhuongTien],
                //	pt.Ten as TenPhuongTien,
                //    la.[Ten] as TenLoaiCa,
                //    tp.[Ten] as TenThanhPham,
                //    s.[Ten] as TenSize,
                //    mau.Ten as TenMau,
                //    Cast(SUM(p.TrongLuong) as decimal(18, 2)) as TrongLuong
                //FROM
                //    [PhieuCanNguyenLieu] p,
                //    [NhaCungCapNguyenLieu] ncc,
                //    [MaLoaiCaNguyenLieu] la,
                //    [MaThanhPhamNguyenLieu] tp,
                //    [MaSizeNguyenLieu] s,
                //    MaMauNguyenLieu mau,
                //	PhuongTienChoNguyenLieu pt,
                //	XiNghiep x
                //WHERE
                //    p.[SuDung] = 'True'
                //    and p.[TrongLuong] > 0
                //    and p.[Ngay] <= @toDate
                //    and p.Ngay >= @fromDate
                //    and p.NhaCC = ncc.Ma
                //    and p.MaLoaiCa = la.Ma
                //    and p.MaLoaiThanhPham = tp.Ma
                //    and p.MaSize = s.Ma
                //    and p.MaMau = mau.Ma
                //	and p.MaPhuongTien  = pt.Ma
                //	and p.MaXuongSanXuat = x.Ma

                //GROUP BY
                //    p.[Ngay],
                //    ncc.[Ten],
                //    p.[MSL],
                //    p.[MaAo],
                //    p.[MaPhuongTien],
                //    la.[Ten],
                //    tp.[Ten],
                //    s.[Ten],
                //    mau.Ten,
                //    p.[MaXuongSanXuat],
                //	pt.Ten,
                //	x.Ten
                //order by
                //    p.Ngay,
                //    ncc.Ten";
                var query = @"SELECT
    p.[Ngay],
    p.[MaXuongSanXuat],
	x.Ten as TenXuong,
    ncc.[Ten] as TenNCC,
    p.[MSL] as MaLo,
    p.[MaAo] as MaAo,
    p.[MaPhuongTien],
	pt.Ten as TenPhuongTien,
    la.[Ten] as TenLoaiCa,
    tp.[Ten] as TenThanhPham,
    s.[Ten] as TenSize,
    mau.Ten as TenMau,
    Cast(SUM(p.TrongLuong) as decimal(18, 2)) as TrongLuong
FROM
    [PhieuCanNguyenLieu] p
    left join NhaCungCapNguyenLieu ncc on ncc.Ma = p.NhaCC
                    left join MaLoaiCaNguyenLieu la on la.Ma = p.MaLoaiCa
                    left join MaThanhPhamNguyenLieu tp on tp.Ma = p.MaLoaiThanhPham
                    left join MaSizeNguyenLieu s on s.Ma = p.MaSize
                    left join MaMauNguyenLieu mau on mau.Ma = p.MaMau
                    left join PhuongTienChoNguyenLieu pt on pt.Ma = p.MaPhuongTien
                	left join BanCatTiet bct on bct.Ma = p.MaBanCatTiet
                	left join XiNghiep x on x.Ma = p.MaXuongSanXuat
WHERE
    p.[SuDung] = 'True'
    and p.[TrongLuong] > 0
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    
    
GROUP BY
    p.[Ngay],
    ncc.[Ten],
    p.[MSL],
    p.[MaAo],
    p.[MaPhuongTien],
    la.[Ten],
    tp.[Ten],
    s.[Ten],
    mau.Ten,
    p.[MaXuongSanXuat],
	pt.Ten,
	x.Ten


union all

SELECT top 1
    p.[Ngay],
    p.[MaXuongSanXuat],
	x.Ten as TenXuong,
    ncc.[Ten] as TenNCC,
    p.[MSL] as MaLo,
    p.[MaAo] as MaAo,
    p.[MaPhuongTien],
	pt.Ten as TenPhuongTien,
    la.[Ten] as TenLoaiCa,
    tp.[Ten] as TenThanhPham,
    s.[Ten] as TenSize,
    mau.Ten as TenMau,
    SoPhieuKhongTareThung * TLTBTareThung * -1 as TrongLuong
FROM
	(    
			SELECT   
			COUNT(CASE WHEN tp.IsTareThung = 'True' THEN 1 END) AS SoPhieuTareThung,  
			COUNT(CASE WHEN tp.IsTareThung = 'False' THEN 1 END) AS SoPhieuKhongTareThung,  
			SUM(CASE WHEN tp.IsTareThung = 'True' THEN p.TrongLuong ELSE 0 END) AS TLTareThung,  
			SUM(CASE WHEN tp.IsTareThung = 'False' THEN p.TrongLuong ELSE 0 END) AS TLKhongTareThung,  
			SUM(CASE WHEN tp.IsTareThung = 'True' THEN p.TrongLuong ELSE 0 END) /  COUNT(CASE WHEN tp.IsTareThung = 'True' THEN 1 END) AS TLTBTareThung
			FROM 
			PhieuCanNguyenLieu p  
			INNER JOIN MaThanhPhamNguyenLieu tp ON p.MaLoaiThanhPham = tp.Ma  
			WHERE   
			p.SuDung = 'True'   
			AND p.TrongLuong > 0   
			AND p.Ngay BETWEEN @fromDate AND @toDate
     ) AS DataTareThung,  
    [PhieuCanNguyenLieu] p,
    [NhaCungCapNguyenLieu] ncc,
    [MaLoaiCaNguyenLieu] la,
    [MaThanhPhamNguyenLieu] tp,
    [MaSizeNguyenLieu] s,
    MaMauNguyenLieu mau,
	PhuongTienChoNguyenLieu pt,
	XiNghiep x
WHERE
    p.[SuDung] = 'True'
    and p.[TrongLuong] > 0
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.NhaCC = ncc.Ma
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
	and p.MaPhuongTien  = pt.Ma
	and p.MaXuongSanXuat = x.Ma
    and tp.IsTareThung = 'true' 
GROUP BY
    p.[Ngay],
    ncc.[Ten],
    p.[MSL],
    p.[MaAo],
    p.[MaPhuongTien],
    la.[Ten],
    tp.[Ten],
    s.[Ten],
    mau.Ten,
    p.[MaXuongSanXuat],
	pt.Ten,
	x.Ten,
	SoPhieuKhongTareThung,
	TLTBTareThung
order by
    p.Ngay,
    ncc.Ten";
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
        public List<T> GetTongHopBCT<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"SELECT
    p.[Ngay],
p.[MaXuongSanXuat],
x.Ten as TenXuong,
    p.MaBanCatTiet,
	bct.Ten as TenBanCatTiet,
    p.[MSL] as MaLo,
    p.[MaAo],
    la.[Ten] as TenLoaiCa,
    tp.[Ten] as TenThanhPham,
    s.[Ten] as TenSize,
    mau.Ten as TenMau,
    Cast(SUM(p.TrongLuong) as decimal(18, 2)) as TrongLuong,
Cast(
        convert(
            varchar(19),
            Min(p.ThoiGianCan),
            120
        ) as datetime
    )  as BatDau,
Cast(
        convert(
            varchar(19),
            Max(p.ThoiGianCan),
            120
        ) as datetime
    ) as KetThuc
FROM
    [PhieuCanNguyenLieu] p,
    [MaLoaiCaNguyenLieu] la,
    [MaThanhPhamNguyenLieu] tp,
    [MaSizeNguyenLieu] s,
    MaMauNguyenLieu mau,
	XiNghiep x,
	BanCatTiet bct
WHERE
    p.[SuDung] = 'True'
    and p.[TrongLuong] > 0
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
	and p.MaXuongSanXuat = x.Ma
	and p.MaBanCatTiet = bct.Ma
GROUP BY
    p.[Ngay],
    p.[MSL],
    p.[MaAo],
    la.[Ten],
    tp.[Ten],
    s.[Ten],
    mau.Ten,
    p.MaBanCatTiet,
	x.Ten,
	bct.Ten,
p.[MaXuongSanXuat]
order by
    p.Ngay,
    p.MaBanCatTiet";
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
        public List<T> GetTongHopPhuongTien<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                //                var query = @"SELECT
                //    p.[Ngay],
                //    p.[MaXuongSanXuat],
                //    x.Ten AS TenXuong,
                //    p.[MaPhuongTien],
                //    pt.Ten AS TenPhuongTien,
                //    p.[MSL] AS MaLo,
                //    p.[MaAo] AS MaAo,
                //    la.[Ten] AS TenLoaiCa,
                //    tp.[Ten] AS TenThanhPham,
                //    s.[Ten] AS TenSize,
                //    mau.Ten AS TenMau,
                //    CAST(SUM(p.TrongLuong) AS decimal(18, 2)) AS TrongLuong,
                //    CAST(MIN(p.ThoiGianCan) AS datetime) AS BatDau,
                //    CAST(MAX(p.ThoiGianCan) AS datetime) AS KetThuc
                //FROM
                //    [PhieuCanNguyenLieu] p
                //INNER JOIN [MaLoaiCaNguyenLieu] la ON p.MaLoaiCa = la.Ma
                //INNER JOIN [MaThanhPhamNguyenLieu] tp ON p.MaLoaiThanhPham = tp.Ma
                //INNER JOIN [MaSizeNguyenLieu] s ON p.MaSize = s.Ma
                //INNER JOIN [MaMauNguyenLieu] mau ON p.MaMau = mau.Ma
                //LEFT JOIN [XiNghiep] x ON p.MaXuongSanXuat = x.Ma
                //LEFT JOIN [PhuongTienChoNguyenLieu] pt ON p.MaPhuongTien = pt.Ma
                //WHERE
                //    p.[SuDung] = 'True'
                //    AND p.[TrongLuong] > 0
                //    AND p.[Ngay] BETWEEN @fromDate AND @toDate
                //GROUP BY
                //    p.[Ngay],
                //    p.[MSL],
                //    p.[MaAo],
                //    p.[MaPhuongTien],
                //    la.[Ten],
                //    tp.[Ten],
                //    s.[Ten],
                //    mau.Ten,
                //    x.Ten,
                //    pt.Ten,
                //    p.[MaXuongSanXuat]
                //ORDER BY
                //    p.Ngay,
                //    p.MaPhuongTien;";
                var query = @"
SELECT
    p.[Ngay],
    p.[MaXuongSanXuat],
    x.Ten AS TenXuong,
    p.[MaPhuongTien],
    pt.Ten AS TenPhuongTien,
    p.[MSL] AS MaLo,
    p.[MaAo] AS MaAo,
    la.[Ten] AS TenLoaiCa,
    p.MaLoaiThanhPham,
    tp.[Ten] AS TenThanhPham,
    p.MaSize,
    s.[Ten] AS TenSize,
    mau.Ten AS TenMau,
    CAST(SUM(p.TrongLuong) AS decimal(18, 2)) AS TrongLuong,
    CAST(MIN(p.ThoiGianCan) AS datetime) AS BatDau,
    CAST(MAX(p.ThoiGianCan) AS datetime) AS KetThuc
FROM
    [PhieuCanNguyenLieu] p
    left join NhaCungCapNguyenLieu ncc on ncc.Ma = p.NhaCC
    left join MaLoaiCaNguyenLieu la on la.Ma = p.MaLoaiCa
    left join MaThanhPhamNguyenLieu tp on tp.Ma = p.MaLoaiThanhPham
    left join MaSizeNguyenLieu s on s.Ma = p.MaSize
    left join MaMauNguyenLieu mau on mau.Ma = p.MaMau
    left join PhuongTienChoNguyenLieu pt on pt.Ma = p.MaPhuongTien
    left join BanCatTiet bct on bct.Ma = p.MaBanCatTiet
    left join XiNghiep x on x.Ma = p.MaXuongSanXuat
WHERE
    p.[SuDung] = 'True'
    --AND p.[TrongLuong] > 21
    AND p.[Ngay] BETWEEN @fromDate AND @toDate and tp.IsPhuPhamCaTap = 0
    and (( p.[TrongLuong] > 21 and tp.IsSNL = 1)or(tp.IsSNL =0))
GROUP BY
    p.[Ngay],
    p.[MSL],
    p.[MaAo],
    p.[MaPhuongTien],
    la.[Ten],
    tp.[Ten],
    s.[Ten],
    mau.Ten,
    x.Ten,
    pt.Ten,
    p.[MaXuongSanXuat],
    p.MaLoaiThanhPham,
    p.MaSize
union
all
SELECT
    top 1 
	p.[Ngay],
    p.[MaXuongSanXuat],
    p.TenXuong,
    p.[MaPhuongTien],
    p.TenPhuongTien,
    p.MaLo,
    p.MaAo,
    p.TenLoaiCa,
    p.MaLoaiThanhPham,
    p.TenThanhPham,
    p.MaSize,
    p.TenSize,
    p.TenMau,
    CAST(SUM(p.TrongLuong) AS decimal(18, 2)) AS TrongLuong,
    CAST(p.Ngay AS datetime) AS BatDau,
    CAST(p.Ngay AS datetime) AS KetThuc
FROM
    (
        SELECT
            top 1 
			p.Ngay,
            CAST(
                CONVERT(VARCHAR(19), p.ThoiGianCan, 120) AS DATETIME
            ) AS ThoiGian,
            p.MaPhuongTien,
            p.Chuyen,
            pt.Ten AS TenPhuongTien,
            ncc.Ten AS TenNCC,
            p.MSL AS MaLo,
            p.MaAo,
            la.Ten AS TenLoaiCa,
            tp.Ma as MaLoaiThanhPham,
            tp.Ten AS TenThanhPham,
            p.MaSize,
            s.Ten AS TenSize,
            mau.Ten AS TenMau,
            SoPhieuKhongTareThung * TLTBTareThung * -1 as TrongLuong,
            p.TrongLuongTare AS TrongLuongTare,
            p.TyLeNuoc AS TyLeNuoc,
            p.TrongLuongOrg AS TrongLuongOrg,
            p.Pheu AS Pheu,
            p.MaBanCatTiet,
            bct.Ten AS TenBanCatTiet,
            p.MaMayTinhCan,
            p.MaXuongSanXuat,
            x.Ten AS TenXuong
        FROM
            (
                SELECT
                    sum(
                        CASE
                            WHEN p.TrongLuong < 15 and p.TrongLuong > 10 THEN 2
                            WHEN p.TrongLuong < 10 THEN 1
                            else 0
                        END
                    ) AS SoPhieuTareThung,
                    sum(
                        CASE
                            WHEN p.TrongLuong > 21 THEN 1
                            else 0
                        END
                    ) AS SoPhieuKhongTareThung,
                    SUM(
                        CASE
                            WHEN p.TrongLuong < 15 THEN p.TrongLuong
                            else 0
                        END
                    ) AS TLTareThung,
                    SUM(
                        CASE
                            WHEN p.TrongLuong > 21 THEN p.TrongLuong
                            else 0
                        END
                    ) AS TLKhongTareThung,
                    SUM(
                        CASE
                            WHEN p.TrongLuong < 15 THEN p.TrongLuong
                            else 0
                        END
                    ) / NULLIF(
                        SUM(
                            CASE
                                WHEN p.TrongLuong < 15 AND p.TrongLuong > 10 THEN 2
                                WHEN p.TrongLuong < 10 THEN 1
                                ELSE 0
                            END
                        ), 0
                    ) AS TLTBTareThung
                FROM
                    (
                        Select
                            p.*
                        from
                            PhieuCanNguyenLieu p,
                            MaThanhPhamNguyenLieu tp
                        where
                            p.[Ngay] <= @toDate
                            and p.Ngay >= @fromDate
                            and p.MaLoaiThanhPham = tp.Ma
                            and tp.IsPhuPhamCaTap = 0
                         and tp.IsSNL =1
                    ) p
                WHERE
                    p.SuDung = 'True'
                    AND p.TrongLuong > 5
                    AND p.Ngay BETWEEN @fromDate
                    AND @toDate
            ) AS DataTareThung,
            (
                Select
                    top(1) p.*
                from
                    PhieuCanNguyenLieu p,
                    MaThanhPhamNguyenLieu tp
                where
                    p.[Ngay] <= @toDate
                    and p.Ngay >= @fromDate
                    and p.MaLoaiThanhPham = tp.Ma
                    and tp.IsPhuPhamCaTap = 0
            ) p
            left JOIN NhaCungCapNguyenLieu ncc ON p.NhaCC = ncc.Ma
            left JOIN MaLoaiCaNguyenLieu la ON p.MaLoaiCa = la.Ma
            left JOIN MaSizeNguyenLieu s ON p.MaSize = s.Ma
            left JOIN MaMauNguyenLieu mau ON p.MaMau = mau.Ma
            left JOIN PhuongTienChoNguyenLieu pt ON p.MaPhuongTien = pt.Ma
            left JOIN BanCatTiet bct ON p.MaBanCatTiet = bct.Ma
            left JOIN XiNghiep x ON p.MaXuongSanXuat = x.Ma
            left join (
                select
                    top(1) *
                from
                    MaThanhPhamNguyenLieu tp
                where
                    tp.IsTareThung = 1
            ) tp on tp.Ma = tp.Ma
        where
            p.[SuDung] = 'True'
            and p.[TrongLuong] > 0
    ) p
GROUP BY
    p.[Ngay],
    p.MaLo,
    p.[MaAo],
    p.[MaPhuongTien],
    p.TenLoaiCa,
    p.TenThanhPham,
    p.TenSize,
    p.TenMau,
    p.TenXuong,
    p.TenPhuongTien,
    p.[MaXuongSanXuat],
    p.MaLoaiThanhPham,
    p.MaSize
    --p.TrongLuong
ORDER BY
    p.Ngay,
    p.MaPhuongTien;";
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
        public List<T> GetTongHopLo<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                //                var query = @"SELECT
                //    p.[Ngay],
                //    p.[MSL] as MaLo,
                //    tp.[Ten] as TenThanhPham,
                //	s.Ten as TenSize,
                //	sum(p.TrongLuong) as TrongLuong
                //FROM
                //    [PhieuCanNguyenLieu] p,
                //    [MaThanhPhamNguyenLieu] tp,
                //    [MaSizeNguyenLieu] s
                //WHERE
                //    p.[SuDung] = 'True'
                //    and p.[TrongLuong] > 0
                //    and p.[Ngay] <= @toDate
                //    and p.Ngay >= @fromDate
                //    and p.MaLoaiThanhPham = tp.Ma
                //    and p.MaSize = s.Ma
                //GROUP BY
                //    p.[Ngay],
                //    p.[MSL],
                //    tp.[Ten],
                //    s.[Ten],
                //	p.[MaXuongSanXuat]
                //order by
                //    p.Ngay";
                var query = @"
SELECT
    p.[Ngay],
    p.[MSL] as MaLo,
	p.MaLoaiThanhPham,
    tp.[Ten] as TenThanhPham,
	p.MaSize,
	s.Ten as TenSize,
	sum(p.TrongLuong) as TrongLuong
FROM
    [PhieuCanNguyenLieu] p,
    [MaThanhPhamNguyenLieu] tp,
    [MaSizeNguyenLieu] s
WHERE
    p.[SuDung] = 'True'
    --and p.[TrongLuong] > 21
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
and tp.IsPhuPhamCaTap = 0
and (( p.[TrongLuong] > 21 and tp.IsSNL = 1)or(tp.IsSNL =0))
GROUP BY
    p.[Ngay],
    p.[MSL],
    tp.[Ten],
    s.[Ten],
	p.[MaXuongSanXuat],
	p.MaLoaiThanhPham,
	p.MaSize

	union all

SELECT top 1
    p.[Ngay],
    p.MaLo,
	p.MaLoaiThanhPham,
    p.TenThanhPham,
	p.MaSize,
	p.TenSize,
	p.TrongLuong
FROM
(    
    SELECT
    top 1 p.Ngay,
    CAST(
        CONVERT(VARCHAR(19), p.ThoiGianCan, 120) AS DATETIME
    ) AS ThoiGian,
    p.MaPhuongTien,
    p.Chuyen,
    pt.Ten AS TenPhuongTien,
    ncc.Ten AS TenNCC,
    p.MSL AS MaLo,
    p.MaAo,
    la.Ten AS TenLoaiCa,
    tp.Ma as MaLoaiThanhPham,
    tp.Ten AS TenThanhPham,
    p.MaSize,
    s.Ten AS TenSize,
    mau.Ten AS TenMau,
    SoPhieuKhongTareThung * TLTBTareThung * -1 as TrongLuong,
    p.TrongLuongTare AS TrongLuongTare,
    p.TyLeNuoc AS TyLeNuoc,
    p.TrongLuongOrg AS TrongLuongOrg,
    p.Pheu AS Pheu,
    p.MaBanCatTiet,
    bct.Ten AS TenBanCatTiet,
    p.MaMayTinhCan,
    p.MaXuongSanXuat,
    x.Ten AS TenXuong
FROM
    (
        SELECT
            sum(
                CASE
                    WHEN p.TrongLuong < 15
                    and p.TrongLuong > 10 THEN 2
                    WHEN p.TrongLuong < 10 THEN 1
                    else 0
                END
            ) AS SoPhieuTareThung,
            sum(
                CASE
                    WHEN p.TrongLuong > 21 THEN 1
                    else 0
                END
            ) AS SoPhieuKhongTareThung,
            SUM(
                CASE
                    WHEN p.TrongLuong < 15 THEN p.TrongLuong
                    else 0
                END
            ) AS TLTareThung,
            SUM(
                CASE
                    WHEN p.TrongLuong > 21 THEN p.TrongLuong
                    else 0
                END
            ) AS TLKhongTareThung,
            SUM(
                CASE
                    WHEN p.TrongLuong < 15 THEN p.TrongLuong
                    else 0
                END
            ) / NULLIF(
                        SUM(
                            CASE
                                WHEN p.TrongLuong < 15 AND p.TrongLuong > 10 THEN 2
                                WHEN p.TrongLuong < 10 THEN 1
                                ELSE 0
                            END
                        ), 0
                    ) AS TLTBTareThung
        FROM
            (
                Select
                    p.*
                from
                    PhieuCanNguyenLieu p,
                    MaThanhPhamNguyenLieu tp
                where
                    p.[Ngay] <= @toDate
                    and p.Ngay >= @fromDate
                    and p.MaLoaiThanhPham = tp.Ma
                    and tp.IsPhuPhamCaTap = 0
                 and tp.IsSNL =1
            ) p
        WHERE
            p.SuDung = 'True'
            AND p.TrongLuong > 5
            AND p.Ngay BETWEEN @fromDate
            AND @toDate
    ) AS DataTareThung,
    (
        Select
            top(1) p.*
        from
            PhieuCanNguyenLieu p,
            MaThanhPhamNguyenLieu tp
        where
            p.[Ngay] <= @toDate
            and p.Ngay >= @fromDate
            and p.MaLoaiThanhPham = tp.Ma
            and tp.IsPhuPhamCaTap = 0
    ) p
    left JOIN NhaCungCapNguyenLieu ncc ON p.NhaCC = ncc.Ma
    left JOIN MaLoaiCaNguyenLieu la ON p.MaLoaiCa = la.Ma
    left JOIN MaSizeNguyenLieu s ON p.MaSize = s.Ma
    left JOIN MaMauNguyenLieu mau ON p.MaMau = mau.Ma
    left JOIN PhuongTienChoNguyenLieu pt ON p.MaPhuongTien = pt.Ma
    left JOIN BanCatTiet bct ON p.MaBanCatTiet = bct.Ma
    left JOIN XiNghiep x ON p.MaXuongSanXuat = x.Ma
    left join (select top(1) * from MaThanhPhamNguyenLieu tp where tp.IsTareThung = 1) tp  on tp.Ma = tp.Ma
where
    p.[SuDung] = 'True'
    and p.[TrongLuong] > 0
) p

order by
    p.Ngay
    ";
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
        public List<T> GetsCaTra<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    p.Ngay ,
    CAST(p.ThoiGianCan as datetime) as ThoiGian,
    ncc.Ten as NhaCungCapName,
    p.MSL as MaLo,
    p.MaAo,
    p.MaPhuongTien,
    pt.Ten as PhuongTienName,
    la.Ten as LoaiCaName,
    tp.Ten as ThanhPhamName,
    s.Ten as SizeName,
    mau.Ten as MauName,
	p.MaBanCatTiet,
	b.Ten as BanCatTietName,
    p.TrongLuong
    
from
    PhieuCanNguyenLieu p,
    NhaCungCapNguyenLieu ncc,
    PhuongTienChoNguyenLieu pt,
    MaLoaiCaNguyenLieu la,
    MaThanhPhamNguyenLieu tp,
    MaSizeNguyenLieu s,
    MaMauNguyenLieu mau,
	BanCatTiet b
where
    p.Ngay = @ngay
    and p.MaXuongSanXuat = @xuongId
    and p.TrongLuong < 0
    and p.SuDung = 1
    and p.NhaCC = ncc.Ma
    and p.MaPhuongTien = pt.Ma
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
	and p.MaBanCatTiet = b.Ma";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        #region Thành phẩm 2
        public List<T> GetChiTietThanhPham2s<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {

                var query = @"   
SELECT
    p.[Ngay],
    Cast(
        convert(
            varchar(19),
            p.ThoiGianCan,
            120
        ) as datetime
    ) as ThoiGian,
    p.[MaPhuongTien],
    p.[Chuyen],
    pt.Ten as TenPhuongTien,
    ncc.[Ten] as TenNCC,
    p.[MSL] as MaLo,
    p.[MaAo],
    la.[Ten] as TenLoaiCa,
    p.MaLoaiThanhPham,
    tp.[Ten] as TenThanhPham,
    p.MaSize,
    s.[Ten] as TenSize,
    mau.Ten as TenMau,
    p.[TrongLuong],
    p.TrongLuongTare,
    p.TyLeNuoc,
    p.TrongLuongOrg,
    p.Pheu,
    p.[MaBanCatTiet],
    bct.Ten as TenBanCatTiet,
    p.MaMayTinhCan,
    p.[MaXuongSanXuat],
    x.Ten as TenXuong
FROM
    (
        Select
            p.*
        from
            PhieuCanNguyenLieu p,
            MaThanhPhamNguyenLieu tp
        where
            p.[Ngay] <= @toDate
            and p.Ngay >= @fromDate
            and p.MaLoaiThanhPham = tp.Ma
            and tp.IsPhuPhamCaTap = 1
    ) p
    left join NhaCungCapNguyenLieu ncc on ncc.Ma = p.NhaCC
    left join MaLoaiCaNguyenLieu la on la.Ma = p.MaLoaiCa
    left join MaThanhPhamNguyenLieu tp on tp.Ma = p.MaLoaiThanhPham
    left join MaSizeNguyenLieu s on s.Ma = p.MaSize
    left join MaMauNguyenLieu mau on mau.Ma = p.MaMau
    left join PhuongTienChoNguyenLieu pt on pt.Ma = p.MaPhuongTien
    left join BanCatTiet bct on bct.Ma = p.MaBanCatTiet
    left join XiNghiep x on x.Ma = p.MaXuongSanXuat
WHERE
    p.[SuDung] = 'True'
    and p.[TrongLuong] > 0";
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
        public List<T> GetTongHopSanPhamThanhPham2<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var query = @"
SELECT
    p.[Ngay],
    p.[MSL] as MaLo,
	p.MaLoaiThanhPham,
    tp.[Ten] as TenThanhPham,
	tp.CTTYLE,
	p.MaSize,
	s.Ten as TenSize,
	sum(p.TrongLuong) as TrongLuong
FROM
    [PhieuCanNguyenLieu] p,
    [MaThanhPhamNguyenLieu] tp,
    [MaSizeNguyenLieu] s
WHERE
    p.[SuDung] = 'True'
    and p.[Ngay] <= @toDate
    and p.Ngay >= @fromDate
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
	and tp.IsPhuPhamCaTap = 1
GROUP BY
    p.[Ngay],
    p.[MSL],
    tp.[Ten],
    s.[Ten],
	p.[MaXuongSanXuat],
	p.MaLoaiThanhPham,
	p.MaSize,
	tp.CTTYLE
    ";
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


        public List<T> GetTongHopSLNLThanhPham2<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {

                var query = @"   
--SLNguyenLieu
SELECT
    p.MaLoaiThanhPham,
    tp.Ten AS TenThanhPham,
    SUM(p.TrongLuong) AS TrongLuong,
    'TONGNL' as NL
FROM PhieuCanNguyenLieu p
JOIN MaThanhPhamNguyenLieu tp ON p.MaLoaiThanhPham = tp.Ma
WHERE
    p.SuDung = 'True'
    AND p.TrongLuong > 21
    AND p.Ngay BETWEEN @fromDate AND @toDate
    AND tp.IsPhuPhamCaTap = 0
GROUP BY
    p.MaLoaiThanhPham, tp.Ten

UNION ALL
--Trừ Tare Thùng
SELECT
    tp.MaLoaiThanhPham,
    tp.Ten AS TenThanhPham,
    SoPhieuKhongTareThung * TLTBTareThung * -1 AS TrongLuong,
    'TONGNL' as NL
FROM (
    SELECT
        SUM(CASE WHEN TrongLuong < 15 AND TrongLuong > 10 THEN 2
                 WHEN TrongLuong < 10 THEN 1 ELSE 0 END) AS SoPhieuTareThung,
        SUM(CASE WHEN TrongLuong > 21 THEN 1 ELSE 0 END) AS SoPhieuKhongTareThung,
        SUM(CASE WHEN TrongLuong < 15 THEN TrongLuong ELSE 0 END) AS TLTareThung,
        SUM(CASE WHEN TrongLuong > 21 THEN TrongLuong ELSE 0 END) AS TLKhongTareThung,
        SUM(CASE WHEN TrongLuong < 15 THEN TrongLuong ELSE 0 END) /
        NULLIF(SUM(CASE WHEN TrongLuong < 15 AND TrongLuong > 10 THEN 2
                        WHEN TrongLuong < 10 THEN 1 ELSE 0 END), 0) AS TLTBTareThung
    FROM PhieuCanNguyenLieu p
    JOIN MaThanhPhamNguyenLieu tp ON p.MaLoaiThanhPham = tp.Ma
    WHERE
        p.SuDung = 'True'
        AND p.TrongLuong > 5
        AND p.Ngay BETWEEN @fromDate AND @toDate
        AND tp.IsPhuPhamCaTap = 0
         and tp.IsSNL =1
) AS TareThung
CROSS JOIN (
    SELECT TOP 1 p.MaLoaiThanhPham, tp.Ten
    FROM PhieuCanNguyenLieu p
    JOIN MaThanhPhamNguyenLieu tp ON p.MaLoaiThanhPham = tp.Ma
    WHERE
        p.SuDung = 'True'
        AND p.TrongLuong > 0
        AND p.Ngay BETWEEN @fromDate AND @toDate
        AND tp.IsPhuPhamCaTap = 0
) AS tp

UNION ALL

--SLXeBuom
SELECT 
    p.MaLoaiThanhPham,
    tp.Ten AS TenThanhPham,
    CASE 
        WHEN tp.IsDat = 1 THEN -1 * SUM(p.TrongLuong)
        ELSE SUM(p.TrongLuong)
    END AS TrongLuong,
    'NLXEBUOM' AS NL
FROM PhieuCanTPFillet p
LEFT JOIN MaThanhPhamFillet tp ON p.MaLoaiThanhPham = tp.Ma AND tp.IsNguyenlieuxebuom = 1
WHERE 
    p.Ngay BETWEEN @fromDate AND @toDate
    AND p.MaXuongSanXuat = @xuongId
    AND p.TrongLuong > 0 
    AND tp.Ma IS NOT NULL
GROUP BY
    p.MaLoaiThanhPham, tp.Ten, tp.IsDat

UNION ALL

--SLBTPFillet
SELECT
    p.MaThanhPham AS MaLoaiThanhPham,
    tp.Ten AS TenThanhPham,
    SUM(p.TrongLuong) AS TrongLuong,
    'NLFILLET' AS NL
FROM PhieuCanBTPFilletv2 p
LEFT JOIN MaThanhPhamFillet tp ON p.MaThanhPham = tp.Ma
WHERE
    p.Ngay BETWEEN @fromDate AND @toDate
    AND p.MaXuong = @xuongId
GROUP BY
    p.MaThanhPham, tp.Ten";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date ,xuongId}).Result.ToList();
                    return items;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        public List<T> GetChiTietPhieuCanKhongTheGhiNhanDuLieu<T>(DateTime fromDate, DateTime toDate)
        {
            try
            {

                var query = @"   
SELECT
    p.[Ngay],
    Cast(
        convert(
            varchar(19),
            p.ThoiGianCan,
            120
        ) as datetime
    ) as ThoiGian,
    p.[MaPhuongTien],
    p.[Chuyen],
    pt.Ten as TenPhuongTien,
    ncc.[Ten] as TenNCC,
    p.[MSL] as MaLo,
    p.[MaAo],
    la.[Ten] as TenLoaiCa,
    p.MaLoaiThanhPham,
    tp.[Ten] as TenThanhPham,
    p.MaSize,
    s.[Ten] as TenSize,
    mau.Ten as TenMau,
    p.[TrongLuong],
    p.TrongLuongTare,
    p.TyLeNuoc,
    p.TrongLuongOrg,
    p.Pheu,
    p.[MaBanCatTiet],
    bct.Ten as TenBanCatTiet,
    p.MaMayTinhCan,
    p.[MaXuongSanXuat],
    x.Ten as TenXuong
FROM
    (
        Select
            p.*
        from
            PhieuCanNguyenLieu p,
            MaThanhPhamNguyenLieu tp
        where
            p.[Ngay] <= @toDate
            and p.Ngay >= @fromDate
            and p.MaLoaiThanhPham = tp.Ma
            and tp.IsPhuPhamCaTap = 0
    ) p
    left join NhaCungCapNguyenLieu ncc on ncc.Ma = p.NhaCC
    left join MaLoaiCaNguyenLieu la on la.Ma = p.MaLoaiCa
    left join MaThanhPhamNguyenLieu tp on tp.Ma = p.MaLoaiThanhPham
    left join MaSizeNguyenLieu s on s.Ma = p.MaSize
    left join MaMauNguyenLieu mau on mau.Ma = p.MaMau
    left join PhuongTienChoNguyenLieu pt on pt.Ma = p.MaPhuongTien
    left join BanCatTiet bct on bct.Ma = p.MaBanCatTiet
    left join XiNghiep x on x.Ma = p.MaXuongSanXuat
WHERE
    p.[SuDung] = 'True'
    and p.[TrongLuong] = -100";
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
