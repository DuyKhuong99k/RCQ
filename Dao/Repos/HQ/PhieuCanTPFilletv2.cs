using AppViewModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ;

public class PhieuCanTPFilletv2
{
    private readonly string connectionString;

    private readonly string qrDelete = @"DELETE FROM [dbo].[PhieuCanTPFilletv2]
      WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";

    private readonly string qrGetAll = "Select * from PhieuCanTPFilletv2";
    private readonly string qrDinhMucTheoSanPham = @"declare @fromDate as date = '{0}',
@toDate as date = '{1}',
@xuongId as varchar(1) = '{2}'
Select
    p.Ngay,
    p.MaThanhPham,
    cast(
        case
            when p.TrongLuongTra = 0 then 0
            else p.TrongLuongNhan / p.TrongLuongTra
        end as decimal(18, 2)
    ) as DinhMuc,
    isnull(dm.DinhMuc, tp.DinhMuc) as DinhMucChuan
from
    (
        Select
            p.Ngay,
            p.MaThanhPham,
            Sum(p.TrongLuongNhan) as TrongLuongNhan,
            SUM(p.TrongLuongTra) as TrongLuongTra
        from
            PhieuCanTPFilletv2 p
        where
            Ngay <= @toDate
            and Ngay >= @fromDate
            and p.MaXuong = @xuongId
        group by
            p.Ngay,
            p.MaThanhPham
    ) p
    left join (
        Select
            *
        from
            (
                Select
                    dm.STT,
                    dm.Ngay,
                    dm.Gio,
                    dm.MaLo,
                    dm.MaLoaiCa,
                    dm.MaMau,
                    dm.MaSize,
                    dm.MaThanhPham,
                    dm.MaXuong,
                    dm.CaTra,
                    dm.DinhMuc,
                    dm.SuDung,
                    ROW_NUMBER() OVER (
                        PARTITION BY MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        MaThanhPham,
                        CaTra,
                        Ngay
                        ORDER BY
                            Gio DESC
                    ) AS [ROW NUMBER]
                from
                    DinhMucFillet dm
                where
                    Ngay <= @toDate
                    And SuDung = 1
                    and MaXuong = @xuongId
            ) dm
        where
            dm.[ROW NUMBER] = 1
    ) dm on p.MaThanhPham = dm.MaThanhPham
    left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma";
    private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanTPFilletv2]
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
           ,[TrongLuongNhan]
           ,[TrongLuongTra]
           ,[DinhMucThucTe]
           ,[DinhMucYeuCau]
           ,[MaXuong]
           ,[MaNhanVien]
           ,[CaTra]
           ,[STTBTP]
           ,[MaMayCanBTP]
           ,[GhiChu],[TrongLuongTare],[MaNhanVienPhucVu],[ThePhieuSanLuongId],[Id],[IdIn])
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
           ,@TrongLuongNhan 
           ,@TrongLuongTra 
           ,@DinhMucThucTe 
           ,@DinhMucYeuCau 
           ,@MaXuong 
           ,@MaNhanVien 
           ,@CaTra 
           ,@STTBTP 
           ,@MaMayCanBTP 
           ,@GhiChu,@TrongLuongTare,@MaNhanVienPhucVu,@ThePhieuSanLuongId,@Id,@IdIn)";

    private readonly string qrUpdate = @"
UPDATE [dbo].[PhieuCanTPFilletv2]
   SET [Gio] = @Gio 
      ,[MaUserCan] = @MaUserCan
      ,[MaLoaiCa] = @MaLoaiCa 
      ,[MaMau] = @MaMau 
      ,[MaSize] = @MaSize 
      ,[MaThanhPham] = @MaThanhPham 
      ,[MaLo] = @MaLo 
      ,[MaThe] = @MaThe 
      ,[TrongLuongNhan] = @TrongLuongNhan 
      ,[TrongLuongTra] = @TrongLuongTra 
      ,[DinhMucThucTe] = @DinhMucThucTe 
      ,[DinhMucYeuCau] = @DinhMucYeuCau 
      ,[MaNhanVien] = @MaNhanVien 
      ,[CaTra] = @CaTra 
      ,[STTBTP] = @STTBTP 
      ,[MaMayCanBTP] = @MaMayCanBTP 
      ,[GhiChu] = @GhiChu,[TrongLuongTare]= @TrongLuongTare,
[MaNhanVienPhucVu] = @MaNhanVienPhucVu,[ThePhieuSanLuongId] =@ThePhieuSanLuongId
 WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";
    private readonly string qrGetsNangSuatTheoNhom = @"
Select
    p.Nhom,
    Sum(p.TrongLuong) as TongTrongLuong,
    count(Distinct p.MaNhanVien) as TongSoNhanVien,
   cast( Sum(p.TrongLuong)/ count(Distinct p.MaNhanVien) as decimal(18,2)) as NangSuat
from
    (
        Select
            tp.MaNhanVien,
            nv.DeptName0 as Nhom,
            Sum(tp.TrongLuongTra) as TrongLuong
        from
            PhieuCanTPFilletv2 tp,
            NhanVienDaiThanh nv
        where
            tp.Ngay >= @fromDate
            and tp.Ngay <= @toDate
            and tp.MaXuong = @xuongId
            and tp.MaNhanVien = nv.MaNhanVien
        GROUP BY
            tp.MaNhanVien,
            nv.DeptName0
    ) p
GROUP BY
    p.Nhom";
    private string tableName = @"PhieuCanTPFilletv2";
    
    public PhieuCanTPFilletv2(string? _connectionString = null)
    {
        connectionString = _connectionString ?? Base.Ins.ConnectionString;
    }
//public List<T> GetTongHopDinhMucTheoSanPham<T>(DateTime fromDate, DateTime toDate, string xuongId)
//    {
//        var query = string.Format(qrDinhMucTheoSanPham, fromDate.ToString("yyyy-MM-dd"),
//            toDate.ToString("yyyy-MM-dd"), xuongId);
//        using var connection = new SqlConnection(connectionString);
//        connection.Open();
//        var items = connection.Query<T>(query).ToList();
//        return items;
//    }
    public List<T> GetTongHopDinhMucTheoSanPham<T>( DateTime fromDate,DateTime toDate,string xuongId)
    {
        var query = @"Select
    p.Ngay,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    cast(
        case
            when p.TrongLuongTra = 0 then 0
            else p.TrongLuongNhan / p.TrongLuongTra
        end as decimal(18, 2)
    ) as DinhMuc,
    isnull(dm.DinhMuc, tp.DinhMuc) as DinhMucChuan
from
    (
        Select
            p.Ngay,
            p.MaThanhPham,
            Sum(p.TrongLuongNhan) as TrongLuongNhan,
            SUM(p.TrongLuongTra) as TrongLuongTra
        from
            PhieuCanTPFilletv2 p
        where
            Ngay <= @toDate
            and Ngay >= @fromDate
            and p.MaXuong = @xuongId
        group by
            p.Ngay,
            p.MaThanhPham
    ) p
    left join (
        Select
            *
        from
            (
                Select
                    dm.STT,
                    dm.Ngay,
                    dm.Gio,
                    dm.MaLo,
                    dm.MaLoaiCa,
                    dm.MaMau,
                    dm.MaSize,
                    dm.MaThanhPham,
                    dm.MaXuong,
                    dm.CaTra,
                    dm.DinhMuc,
                    dm.SuDung,
                    ROW_NUMBER() OVER (
                        PARTITION BY MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        MaThanhPham,
                        CaTra,
                        Ngay
                        ORDER BY
                            Gio DESC
                    ) AS [ROW NUMBER]
                from
                    DinhMucFillet dm
                where
                    Ngay <= @toDate
                    And SuDung = 1
                    and MaXuong = @xuongId
            ) dm
        where
            dm.[ROW NUMBER] = 1
    ) dm on p.MaThanhPham = dm.MaThanhPham
    left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
where tp.IsNguyenCon = 0";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate = fromDate.Date,toDate = toDate.Date, xuongId })
                .Result
                .ToList();
            return items;
        }
    }
    //public List<T> GetTongHopNangSuatNhom<T>(DateTime fromDate, DateTime toDate, string xuongId)
    //{
    //    var query = string.Format(qrGetsNangSuatTheoNhom, fromDate.ToString("yyyy-MM-dd"),
    //        toDate.ToString("yyyy-MM-dd"), xuongId);
    //    using var connection = new SqlConnection(connectionString);
    //    connection.Open();
    //    var items = connection.Query<T>(query).ToList();
    //    return items;
    //}


    public List<T> GetTongHopNangSuatNhom<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var query = @"Select
	p.Ngay,
    p.Nhom,
    Sum(p.TrongLuong) as TongTrongLuong,
    count(Distinct p.MaNhanVien) as TongSoNhanVien,
   cast( Sum(p.TrongLuong)/ count(Distinct p.MaNhanVien) as decimal(18,2)) as NangSuat
from
    (
        Select
			tp.Ngay,
            tp.MaNhanVien,
            nv.DeptName0 as Nhom,
            Sum(tp.TrongLuongTra) as TrongLuong
        from
            PhieuCanTPFilletv2 tp,
            NhanVienDaiThanh nv
        where
            tp.Ngay >= @fromDate
            and tp.Ngay <= @toDate
            and tp.MaXuong = @xuongId
            and tp.MaNhanVien = nv.MaNhanVien
            --and nv.IsGiaCong=0
        GROUP BY
            tp.MaNhanVien,
            nv.DeptName0,
			tp.Ngay
    ) p
GROUP BY
    p.Nhom,p.Ngay";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate = fromDate.Date,toDate = toDate.Date, xuongId })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongSanLuongTP<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var query = @"select 
p.MaThanhPham,
sum(p.TrongLuongNhan) as TrongLuongNhan,
sum(p.TrongLuongTra) as TrongLuongTra,
(sum(p.TrongLuongNhan)/SUM(p.TrongLuongTra) )as DinhMuc
from PhieuCanTPFilletv2 p
left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
where
p.Ngay <= @toDate
and p.Ngay >= @fromDate
and p.MaXuong = @xuongId and tp.IsNguyenCon = 0
group by p.MaThanhPham";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate = fromDate.Date,toDate = toDate.Date, xuongId })
                .Result
                .ToList();
            return items;
        }
    }
    public List<T> GetTongHopSanLuongAndDinhMucThanhPhamTheoChuyen<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var query = @"SELECT
    p.Nhom,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    CAST(
        CASE 
            WHEN p.TLTra = 0 THEN 0
            ELSE ROUND(p.TLNhan / p.TLTra, 2)
        END AS DECIMAL(18,2)
    ) AS DinhMuc,
    p.TLNhan,
    p.TLTra
FROM
(
    SELECT
        nv.DeptName0 AS Nhom,
        p.MaThanhPham,
        SUM(p.TrongLuongNhan) AS TLNhan,
        SUM(p.TrongLuongTra) AS TLTra
    FROM
        PhieuCanTPFilletv2 p
        JOIN NhanVienDaiThanh nv ON p.MaNhanVien = nv.MaNhanVien
    WHERE
        p.Ngay >= @fromDate AND p.Ngay <= @toDate
        AND p.MaXuong = @xuongId
    GROUP BY
        nv.DeptName0, p.MaThanhPham
) p
LEFT JOIN MaThanhPhamFillet tp ON p.MaThanhPham = tp.Ma
where tp.IsNguyenCon = 0
order by p.Nhom

";
        var items = connection.QueryAsync<T>(
                query,
                new { fromDate = fromDate.Date,toDate = toDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }
    public List<T> GetSanLuongTBTPFillet<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var query = @"
SELECT SUM(p.TrongLuongTra) as TrongLuongTra,
COUNT(DISTINCT p.MaNhanVien) as SoLuongNhanVien,
(SUM(p.TrongLuongTra)/COUNT(DISTINCT p.MaNhanVien)) as SanLuongTB
FROM PhieuCanTPFilletv2 p
left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
WHERE p.Ngay >= @fromDate AND p.Ngay <= @toDate
AND p.MaXuong = @xuongId and tp.IsNguyenCon = 0";
        var items = connection.QueryAsync<T>(
                query,
                new { fromDate = fromDate.Date,toDate = toDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }
    public List<T> GetDinhMucLangDa<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var query = @"
;WITH TraFL AS (
    SELECT SUM(TrongLuongTra) AS TrongLuongTraFL
    FROM PhieuCanTPFilletv2
    WHERE Ngay >= @fromDate AND Ngay <= @toDate 
      AND MaXuong = @xuongId
),
CaDa AS (
    SELECT SUM(p.TrongLuong) AS TrongLuongCaDa
    FROM PhieuCanBTPDinhHinh p
    INNER JOIN MaThanhPhamDinhHinh tp ON tp.Ma = p.MaThanhPham
    WHERE p.Ngay >= @fromDate AND p.Ngay <= @toDate 
      AND p.MaXuong = @xuongId 
      AND tp.IsCaDa = 1
),
NhanDinhHinh AS (
    SELECT SUM(p.TrongLuong) AS TrongLuongNhanDinhHinh
    FROM PhieuCanBTPDinhHinh p
    INNER JOIN MaThanhPhamDinhHinh tp ON tp.Ma = p.MaThanhPham
    WHERE p.Ngay >= @fromDate AND p.Ngay <= @toDate 
      AND p.MaXuong = @xuongId
)
SELECT 
    t.TrongLuongTraFL,
   IsNull( c.TrongLuongCaDa,0) as TrongLuongCaDa,
    n.TrongLuongNhanDinhHinh,
    CASE 
        WHEN (n.TrongLuongNhanDinhHinh - IsNull( c.TrongLuongCaDa,0)) = 0 THEN 0
        ELSE (t.TrongLuongTraFL - IsNull( c.TrongLuongCaDa,0)) * 1.0
             / (n.TrongLuongNhanDinhHinh - IsNull( c.TrongLuongCaDa,0))
    END AS DinhMucLangDa
FROM TraFL t
CROSS JOIN CaDa c
CROSS JOIN NhanDinhHinh n;";
        var items = connection.QueryAsync<T>(
                query,
                new { fromDate = fromDate.Date,toDate = toDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }

    public int ChuyenSize<T>(List<T> items, string sizeId)
    {
        var query = $@"
UPDATE [dbo].[PhieuCanTPFilletv2]
   SET [MaSize] = '{sizeId}',[GhiChu] = @GhiChu
 WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var rows = connection.Execute(query, items);
            return rows;
        }
    }

    public int ChuyenXuong<T>(List<T> items, string xuongId)
    {
        var query = $@"
UPDATE [dbo].[PhieuCanTPFilletv2]
   SET [MaXuong] = '{xuongId}',[GhiChu] = @GhiChu
 WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong ";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var rows = connection.Execute(query, items);
            return rows;
        }
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
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var rows = connection.Execute(qrDelete, items);
            return rows;
        }
    }

    public decimal Get(string phieuSanLuongId)
    {
        try
        {
            var query = @"Select IsNull(Sum(TrongLuongTra), 0) as TrongLuong
from PhieuCanTPFilletv2
Where ThePhieuSanLuongId = @phieuSanLuongId";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.ExecuteScalar<decimal>(query, new { phieuSanLuongId });
            return items;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public List<T> GetChiTietMix<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        try
        {
            //                var query = @"
            //select p.*,
            //    cast(
            //        (
            //            case
            //                when p.DinhMucBan <= p.DinhMucChuan then 1
            //                else 0
            //            end
            //        ) as bit
            //    ) as DanhGia
            //from (
            //        Select p.*,
            //            CAST(
            //                case
            //                    when p.TrongLuongTraBan = 0 then 0
            //                    else p.TrongLuongNhan / p.TrongLuongTraBan
            //                end as decimal(18, 3)
            //            ) as DinhMucBan,
            //            ISNULL(dm.DinhMuc, p.DinhMucYeuCau) as DinhMucChuan
            //        from (
            //                select p.*,
            //                    SUM(p.TrongLuongTra) over (
            //                        PARTITION BY p.MaBan,
            //                        p.MaThe,
            //                        p.MaXuong,
            //                        p.Ngay,
            //                        p.TrongLuongNhan,
            //                        p.ThePhieuSanLuongId
            //                    ) as TrongLuongTraBan
            //                from PhieuCanTPFilletv2 p
            //                WHERE p.Ngay <= @toDate
            //                    and p.Ngay >= @fromDate
            //                    and p.MaXuong = @xuongId
            //                    and p.STT > 0
            //            ) p
            //            left join(
            //                Select *
            //                from (
            //                        Select d.*,
            //                            ROW_NUMBER() OVER(
            //                                PARTITION BY MaLo,
            //                                MaLoaiCa,
            //                                MaMau,
            //                                MaSize,
            //                                MaThanhPham
            //                                ORDER BY Gio DESC
            //                            ) AS [ROW NUMBER]
            //                        from DinhMucFillet d
            //                        where Ngay >= @fromDate
            //                            and Ngay <= @toDate
            //                            And MaXuong = @xuongId
            //                    ) dm
            //                Where dm.[ROW NUMBER] = 1
            //            ) dm on p.MaThanhPham = dm.MaThanhPham
            //            and p.MaLo = dm.MaLo
            //            and p.MaSize = dm.MaSize
            //            and p.CaTra = dm.CaTra
            //            and p.MaMau = dm.MaMau
            //            and p.MaLoaiCa = dm.MaLoaiCa
            //            and p.Ngay = dm.Ngay
            //    ) p";
            var query = @"
select p.*,
    cast(
        (
            case
                when p.DinhMucBan <= p.DinhMucChuan then 1
                else 0
            end
        ) as bit
    ) as DanhGia,
    tp.Ten as ThanhPhamName,
    s.Ten as SizeName,
    n.Name as NhanVienName
from (
        Select p.*,
            CAST(
                case
                    when p.TrongLuongTraBan = 0 then 0
                    else p.TrongLuongNhan / p.TrongLuongTraBan
                end as decimal(18, 3)
            ) as DinhMucBan,
            ISNULL(dm.DinhMuc, p.DinhMucYeuCau) as DinhMucChuan
        from (
                select p.*,
                    SUM(p.TrongLuongTra) over (
                        PARTITION BY p.MaBan,
                        p.MaThe,
                        p.MaXuong,
                        p.Ngay,
                        p.TrongLuongNhan,
                        p.ThePhieuSanLuongId
                    ) as TrongLuongTraBan
                from PhieuCanTPFilletv2 p
                WHERE p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and p.MaXuong = @xuongId
                    and p.STT > 0
            ) p
            left join(
                Select *
                from (
                        Select d.*,
                            ROW_NUMBER() OVER(
                                PARTITION BY MaLo,
                                MaLoaiCa,
                                MaMau,
                                MaSize,
                                MaThanhPham
                                ORDER BY Gio DESC
                            ) AS [ROW NUMBER]
                        from DinhMucFillet d
                        where Ngay >= @fromDate
                            and Ngay <= @toDate
                            And MaXuong = @xuongId
                    ) dm
                Where dm.[ROW NUMBER] = 1
            ) dm on p.MaThanhPham = dm.MaThanhPham
            and p.MaLo = dm.MaLo
            and p.MaSize = dm.MaSize
            and p.CaTra = dm.CaTra
            and p.MaMau = dm.MaMau
            and p.MaLoaiCa = dm.MaLoaiCa
            and p.Ngay = dm.Ngay
    ) p
     LEFT Join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
     LEFT JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
     LEFT JOIN MaSizeFillet s on p.MaSize = s.Ma";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId })
                .ToList();
            return items;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public List<T> GetChiTiets<T>(DateTime dateTime, string xuongId)
    {
        var query = @"Select
    tp.STT,
    tp.Gio,
    tp.MaNhanVien,
    tp.MaHoSo,
    tp.TenNhanVien,
    n.MaNhanVien as MaNhanVienPhucVu,
    n.MaHoSo as MaHoSoPhucVu,
    n.Name as TenNhanVienPhucVu,
    tp.ThanhPhamName,
    tp.SizeName,
    tp.LoaiCaName,
    tp.MauName,
    Case
        when tp.SizeName = N'Cá Lớn' then 'L'
        when tp.SizeName = N'Cá Nhỏ' then 'N'
        else tp.SizeName
    end as SizeName,
    case
        when tp.CaTra = 1 then 'True'
        when tp.CaTra = 0 then 'F'
        else ''
    end as CaTra,
    tp.MaLo,
    tp.DinhMuc,
    dm.DinhMuc as DinhMucChuan,
    tp.TrongLuongTra,
    case
        when tp.DinhMuc <= dm.DinhMuc then N'Đạt'
        else N'Không Đạt'
    end as DanhGia,
    tp.MaMayCan
from
    (
        Select
            ptp.STT,
            ptp.Gio,
            ptp.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            tp.Ma as MaThanhPham,
            tp.Ten as ThanhPhamName,
            s.Ma as MaSize,
            s.Ten as SizeName,
            ptp.CaTra,
            ptp.MaLo,
            ptp.MaMau,
            ptp.MaLoaiCa,
            la.Ten as LoaiCaName,
            mau.Ten as MauName,
            floor(
                100 * ptp.TrongLuongNhan / ptp.TrongLuongTra
            ) / 100 as DinhMuc,
            ptp.TrongLuongTra,
            ptp.MaMayCan,
            ptp.MaNhanVienPhucVu
        from
            PhieuCanTPFilletv2 ptp,
            NhanVienDaiThanh n,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau,
            MaLoaiCaFillet la
        where
            ptp.Ngay = @ngay
            and ptp.MaXuong = @xuongId
            and ptp.MaNhanVien = n.MaNhanVien
            and ptp.MaThanhPham = tp.Ma
            and ptp.MaSize = s.Ma
            and ptp.MaMau = mau.Ma
            and ptp.MaLoaiCa = la.Ma
            and ptp.TrongLuongTra > 0
    ) tp
    left join(
        Select
            *
        from
            (
                Select
                    d.*,
                    ROW_NUMBER() OVER (
                        PARTITION BY MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        MaThanhPham,
                        CaTra
                        ORDER BY
                            Gio DESC
                    ) AS [ROW NUMBER]
                from
                    DinhMucFillet d
                where
                    Ngay = @ngay
                    And MaXuong = @xuongId
            ) dm
        Where
            dm.[ROW NUMBER] = 1
    ) dm on tp.MaThanhPham = dm.MaThanhPham
    and tp.MaLo = dm.MaLo
    and tp.MaSize = dm.MaSize
    and tp.CaTra = dm.CaTra
    and tp.MaMau = dm.MaMau
    and tp.MaLoaiCa = dm.MaLoaiCa
    left JOIN NhanVienDaiThanh n ON tp.MaNhanVienPhucVu = n.MaNhanVien
order by
    Gio";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result
                .ToList();
            return items;
        }
    }

    public List<T> GetChiTiets_TG<T>(DateTime dateTime, string maLoaiDonGia)
    {
        var query = @"SELECT
    p.Ngay,
    p.STT,
    p.Gio,
    p.TrongLuongTra + p.TrongLuongTare as TrongLuongCan,
    p.TrongLuongTare,
    p.TrongLuongTra as TrongLuong,
    tp.Ten as MaSanPham,
    p.MaNhanVien,
    p.MaNhanVienPhucVu,
    n.MaHoSo as MaHoSoPhucVu,
    p.MaLo,
    p.MaXuong,
    p.MaMayCan,
    p.DinhMucThucTe,
    ISNULL(map.DonGia, 0) as DonGia,
    ISNULL(map.DonGia, 0) * p.TrongLuongTra as ThanhTien,
    p.MaHoSo,
p.MaSize
from
    (
        Select
            p.*,
            nv.MaHoSo
        from
            PhieuCanTPFilletv2 p,
            NhanVienDaiThanh nv
        where
            p.Ngay = @ngay
            and p.MaNhanVien = nv.MaNhanVien 
    ) p
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVienPhucVu = n.MaNhanVien
    left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
    left JOIN (
        Select
            map.*,
            dg.DinhMucDown,
            dg.DinhMucUp,
            dg.DonGia,
            dg.HeSo,
            dg.DanhGia
        from
            MapThanhPhamFillet map,
            (
                Select
                    *
                from
                    (
                        Select
                            dg.Ngay,
                            dg.MaSanPham,
                            dg.HeSo,
                            dg.DonGia,
                            dg.DanhGia,
                            dg.DinhMucDown,
                            dg.DinhMucUp,
                            dg.MaLoaiDonGia,
                            ROW_NUMBER() OVER (
                                PARTITION by dg.MaSanPham,
                                dg.HeSo,
                                dg.DanhGia,
                                dg.DinhMucDown,
                                dg.DinhMucUp,
                                dg.MaLoaiDonGia
                                ORDER by
                                    dg.Ngay DESC,
                                    dg.Gio DESC
                            ) as row_number
                        from
                            DG_DonGia dg
                        where
                            dg.MaLoaiDonGia = @maLoaiDonGia and dg.LoaiCan=''
                    ) dg
                where
                    row_number = 1
            ) dg
        where
            map.MaBravoFillet = dg.MaSanPham
    ) map ON p.MaThanhPham = map.MaTPFillet
    and p.MaSize = map.MaSize
    and p.MaLoaiCa = map.MaLoaiCaFillet
    and map.IsTangCa = 0
    and p.DinhMucThucTe >= map.DinhMucDown
    and p.DinhMucThucTe <= map.DinhMucUp
order by
    p.STT";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.Query<T>(query, new { ngay = dateTime.Date, maLoaiDonGia }).ToList();
        return items;
    }

    public List<T> GetChiTiets2<T>(DateTime dateTime, string xuongId)
    {
        var query = @"Select
    tp.STT,
    Cast(
        convert(
            varchar(19),
            cast(@ngay as datetime) + Cast(tp.Gio as datetime),
            120
        ) as datetime
    ) as [Giờ],
    tp.MaNhanVien as [Mã Nhân Viên],
    tp.MaHoSo as [Mã Số],
    tp.TenNhanVien as [Tên Nhân Viên],
    n.MaNhanVien as [Mã Nhân Viên Phục Vụ],
    n.MaHoSo as [Mã Số Phục Vụ],
    n.Name as [Tên Nhân Viên Phục Vụ],
    tp.MaLo as [Lô],
    tp.ThanhPhamName as [Thành Phẩm],
    tp.LoaiCaName as [Loại Cá],
    tp.SizeName as [Size],
    tp.MauName as [Màu],
    tp.DinhMuc as [Định Mức],
    tp.TrongLuongTra as [Trọng Lượng],
    tp.MaMayCan as [Máy Cân]
from
    (
        Select
            ptp.STT,
            ptp.Gio,
            ptp.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            tp.Ma as MaThanhPham,
            tp.Ten as ThanhPhamName,
            s.Ma as MaSize,
            s.Ten as SizeName,
            ptp.CaTra,
            ptp.MaLo,
            ptp.MaMau,
            ptp.MaLoaiCa,
            la.Ten as LoaiCaName,
            mau.Ten as MauName,
            floor(
                100 * ptp.TrongLuongNhan / ptp.TrongLuongTra
            ) / 100 as DinhMuc,
            ptp.TrongLuongTra,
            ptp.MaMayCan,
            ptp.MaNhanVienPhucVu
        from
            PhieuCanTPFilletv2 ptp,
            NhanVienDaiThanh n,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau,
            MaLoaiCaFillet la
        where
            ptp.Ngay = @ngay
            and ptp.MaXuong = @xuongId
            and ptp.MaNhanVien = n.MaNhanVien
            and ptp.MaThanhPham = tp.Ma
            and ptp.MaSize = s.Ma
            and ptp.MaMau = mau.Ma
            and ptp.MaLoaiCa = la.Ma
            and ptp.TrongLuongTra > 0
    ) tp
    left join(
        Select
            *
        from
            (
                Select
                    d.*,
                    ROW_NUMBER() OVER (
                        PARTITION BY MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        MaThanhPham,
                        CaTra
                        ORDER BY
                            Gio DESC
                    ) AS [ROW NUMBER]
                from
                    DinhMucFillet d
                where
                    Ngay = @ngay
                    And MaXuong = @xuongId
            ) dm
        Where
            dm.[ROW NUMBER] = 1
    ) dm on tp.MaThanhPham = dm.MaThanhPham
    and tp.MaLo = dm.MaLo
    and tp.MaSize = dm.MaSize
    and tp.CaTra = dm.CaTra
    and tp.MaMau = dm.MaMau
    and tp.MaLoaiCa = dm.MaLoaiCa
    left JOIN NhanVienDaiThanh n ON tp.MaNhanVienPhucVu = n.MaNhanVien
order by
    tp.STT";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result
            .ToList();
        return items;
    }

    public List<T> GetChiTiets2<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var query = @"Select
    tp.STT,
    tp.Ngay as [Ngày],
    Cast(
        convert(
            varchar(19),
            cast(tp.Ngay as datetime) + Cast(tp.Gio as datetime),
            120
        ) as datetime
    ) as [Giờ],
    tp.MaNhanVien as [Mã Nhân Viên],
    tp.MaHoSo as [Mã Số],
    tp.TenNhanVien as [Tên Nhân Viên],
    n.MaNhanVien as [Mã Nhân Viên Phục Vụ],
    n.MaHoSo as [Mã Số Phục Vụ],
    n.Name as [Tên Nhân Viên Phục Vụ],
    tp.MaLo as [Lô],
    tp.ThanhPhamName as [Thành Phẩm],
    tp.LoaiCaName as [Loại Cá],
    tp.SizeName as [Size],
    tp.MauName as [Màu],
    tp.DinhMuc as [Định Mức],
    tp.TrongLuongTra as [Trọng Lượng],
    tp.MaMayCan as [Máy Cân]
from
    (
        Select
            ptp.STT,
            ptp.Ngay,
            ptp.Gio,
            ptp.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            tp.Ma as MaThanhPham,
            tp.Ten as ThanhPhamName,
            s.Ma as MaSize,
            s.Ten as SizeName,
            ptp.CaTra,
            ptp.MaLo,
            ptp.MaMau,
            ptp.MaLoaiCa,
            la.Ten as LoaiCaName,
            mau.Ten as MauName,
            floor(
                100 * ptp.TrongLuongNhan / ptp.TrongLuongTra
            ) / 100 as DinhMuc,
            ptp.TrongLuongTra,
            ptp.MaMayCan,
            ptp.MaNhanVienPhucVu
        from
            PhieuCanTPFilletv2 ptp,
            NhanVienDaiThanh n,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau,
            MaLoaiCaFillet la
        where
            ptp.Ngay <= @ngay
            and ptp.Ngay >= @fromDate
            and ptp.MaXuong = @xuongId
            and ptp.MaNhanVien = n.MaNhanVien
            and ptp.MaThanhPham = tp.Ma
            and ptp.MaSize = s.Ma
            and ptp.MaMau = mau.Ma
            and ptp.MaLoaiCa = la.Ma
            and ptp.TrongLuongTra > 0
    ) tp
    left join(
        Select
            *
        from
            (
                Select
                    d.*,
                    ROW_NUMBER() OVER (
                        PARTITION BY MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        MaThanhPham,
                        CaTra
                        ORDER BY
                            Gio DESC
                    ) AS [ROW NUMBER]
                from
                    DinhMucFillet d
                where
                    Ngay = @ngay
                    And MaXuong = @xuongId
            ) dm
        Where
            dm.[ROW NUMBER] = 1
    ) dm on tp.MaThanhPham = dm.MaThanhPham
    and tp.MaLo = dm.MaLo
    and tp.MaSize = dm.MaSize
    and tp.CaTra = dm.CaTra
    and tp.MaMau = dm.MaMau
    and tp.MaLoaiCa = dm.MaLoaiCa
    left JOIN NhanVienDaiThanh n ON tp.MaNhanVienPhucVu = n.MaNhanVien
order by
    tp.Ngay,
    tp.STT";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(
                query,
                new { fromDate = fromDate.Date, ngay = toDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }

    public List<T> GetChiTietType1_2<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var query = @"
Select p.STT,
    p.Ngay as [Ngày],
      Cast(
        convert(
            varchar(19),
            cast(p.Ngay as datetime) + Cast(p.Gio as datetime),
            120
        ) as datetime
    ) as [Giờ],
    p.MaLo as [Lô],
    p.NhanVienName as [Tên Nhân Viên],
    p.MaNhanVien as [Mã Nhân Viên],
    p.MaHoSo as [Mã Hồ Sơ],
    p.MaHoSoPhucVu [Mã Hồ Sơ Phục Vụ],
    p.ThanhPhamName [Thành Phẩm],
    p.MaSanPham AS [Mã Sản Phẩm],
    p.SizeName as Size,
    p.LoaiCaName as [Loại Cá],
    p.MauName as [Màu],
    p.BanName as [Bàn],
    p.TrongLuongTra as [TL Trả],
    p.TrongLuongTare as [TL Tare],
    p.MaMayCan as [Máy TP],
    p.STTBTP as [STT_BTP],
     p.TrongLuongNhan as [TL BTP],
    p.MaMayCanBTP as [Máy BTP],
    p.MaNhanVienPhucVu as [Mã Nhân Viên Phục Vụ],
    p.NhanVienNamePhucVu as [Tên Nhân Viên Phục Vụ],    
    p.MaThe,
    p.ThePhieuSanLuongId
from (
        SELECT p.STT,
            p.Ngay,
            p.Gio,
            n.Name as NhanVienName,
            n.MaNhanVien,
            n.MaHoSo,
            nv.MaHoSo as MaHoSoPhucVu,
            nv.Name as NhanVienNamePhucVu,
            nv.MaNhanVien as MaNhanVienPhucVu,
            p.MaThanhPham,
            tp.Ten as ThanhPhamName,
            map.MaBravoFillet as MaSanPham,
            s.Ten as SizeName,
            la.Ten as LoaiCaName,
            mau.Ten as MauName,
            ban.Ten as BanName,
            p.TrongLuongNhan,
            p.TrongLuongTra,
            p.TrongLuongTare,
            p.ThePhieuSanLuongId,
            p.MaLo,
            p.MaMayCan,
            p.STTBTP,
            p.MaMayCanBTP,
            p.MaBan,
            p.MaSize,
            p.MaLoaiCa,
            p.MaMau,
            pSl.MaThe
        from (
                Select p.*,
                    case
                        when p.Gio <= '05:00:00' then 1
                        else 0
                    end as IsTangCa
                from PhieuCanTPFilletv2 p
                where p.Ngay <= @ngay
                    and p.Ngay >= @fromDate
                    and p.MaXuong = @xuongId
                    and p.STT > 0
            ) p
            LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            LEFT JOIN NhanVienDaiThanh nv on p.MaNhanVienPhucVu = nv.MaNhanVien
            LEFT JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
            LEFT JOIN MaSizeFillet s on p.MaSize = s.Ma
            LEFT JOIN MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
            LEFT JOIN BanFillet ban on p.MaBan = ban.Ma
            LEFT JOIN MaMauFillet mau on p.MaMau = mau.Ma
            LEFT JOIN (
                select map.*,
                    isnull(dg.DonGia, 0) as DonGia,
                    isnull(dg.HeSo, 1) as HeSo
                from MapThanhPhamFillet map
                    LEFT JOIN (
                        Select *
                        from (
                                Select *,
                                    ROW_NUMBER() over (
                                        partition by MaSanPham
                                        order by Ngay DESC,
                                            Gio Desc
                                    ) as rowId
                                from DG_DonGia
                                where Ngay <= @ngay
                                    and MaLoaiDonGia = 'DM'
                                    and LoaiCan = ''
                            ) p
                        where p.rowId = 1
                    ) dg ON dg.MaSanPham = map.MaBravoFillet
            ) map on p.MaThanhPham = map.MaTPFillet
            and p.IsTangCa = map.IsTangCa
            and p.MaSize = map.MaSize
            and p.MaLoaiCa = map.MaLoaiCaFillet
            LEFT JOIN ThePhieuSanLuongFillet pSl on p.ThePhieuSanLuongId = pSl.Id
    ) p -- where STTBTP = 51 and MaMayCanBTP ='DESKTOP-Q8IL9F5' 
order by Gio,
    MaBan,
    TrongLuongNhan
";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(
                query,
                new { fromDate = fromDate.Date, ngay = toDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }

    public int GetMaxSTT(DateTime dateTime, string xuongId, string mayCanId)
    {
        var query =
            @"select ISNULL( MAX(STT),0) from PhieuCanTPFilletv2 where Ngay=@ngay and MaXuong=@xuongId  and MaMayCan=@mayCanId";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var item = connection.ExecuteScalar<int>(query, new { ngay = dateTime.Date, xuongId, mayCanId });
        return item;
    }

    public int GetNumNhanVienDaChiaCa(DateTime dateTime, string xuongId)
    {
        var query = @"Select
    COUNT(Distinct MaNhanVien)
from
    PhieuCanTPFilletv2
where
    Ngay = @ngay
    and MaXuong = @xuongId";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var item = connection.ExecuteScalar<int>(query, new { ngay = dateTime.Date, xuongId });
        return item;
    }

    /// <summary>
    ///     Cach Tinh cu ko theo ty le tron ca
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="dateTime"></param>
    /// <param name="xuongId"></param>
    /// <returns></returns>
    public List<TEntity> GetPhieuCanTongHopTinhLuong<TEntity>(DateTime dateTime, string xuongId)
    {
        try
        {
            var query =
                @"
Select 
	p.MaNhanVien,n.MaHoSo,n.[Name] as TenNhanVien,la.Ten As LoaiCaName,s.Ten As SizeName,tp.Ten as ThanhPhamName,tp.Ma As MaThanhPham,ma.Ten as MauName,p.CaTra,Sum(p.TrongLuongNhan) as TrongLuongNhan,Sum(p.TrongLuongTra) as TrongLuongTra,(FLOOR( (Sum(p.TrongLuongNhan)/Sum(p.TrongLuongTra)) *100 )/100) as DinhMucThucTe,p.DinhMucYeuCau,Sum( p.SoRo) As SoRo 
from 
	(
		Select
			p.MaNhanVien, p.MaLo,p.MaLoaiCa,p.MaSize,
			CASE
				WHEN p.CaTra =1 THEN 'B' 
				ELSE p.MaThanhPham 
			END  as MaThanhPham,
			p.MaMau,p.CaTra,Sum(p.TrongLuongNhan) as TrongLuongNhan,Sum(p.TrongLuongTra) as TrongLuongTra,AVG(p.DinhMucThucTe) as DinhMucThucTe,
			 DinhMuc.DinhMuc as DinhMucYeuCau,
			Count(*) as SoRo  
		from  
			PhieuCanTPFilletv2 p,
			(
				Select 
					tp1.MaLo,tp1.MaLoaiCa,tp1.MaSize,tp1.MaMau,tp1.MaThanhPham,tp1.CaTra, 
					CASE 
						WHEN tp2.DinhMuc is Null THEN tp1.DinhMuc 
						ELSE tp2.DinhMuc 
					END AS DinhMuc 
				from
					(
						Select 
							distinct p.MaLo,p.MaLoaiCa,p.MaSize,p.MaMau,p.MaThanhPham,p.CaTra, 
							CASE 
								WHEN p.CaTra =1 THEN 1.37 
								ELSE tp.DinhMuc 
							END AS DinhMuc 
						from 
							PhieuCanTPFilletv2 p,MaThanhPhamFillet tp 
						where 
							Ngay= @ngay and MaXuong = @xuongId  and p.MaThanhPham = tp.Ma and p.MaLoaiCa =tp.MaCa 
					) tp1
				LEFT JOIN 
					(
						Select
							MaLo,MaLoaiCa,MaSize,MaMau,MaThanhPham,CaTra, DinhMuc
						from 
							(
								Select 
									d.*,ROW_NUMBER() 
								OVER 
									(PARTITION BY  MaLo,MaLoaiCa,MaMau,MaSize,MaThanhPham,CaTra ORDER BY Gio DESC) AS [ROW NUMBER] 
								from 
									DinhMucFillet d 
								where 
									Ngay = @ngay And MaXuong = @xuongId
							) dm
						 Where 
							dm.[ROW NUMBER] =1
					) tp2
				on 
					tp1.MaLo = tp2.MaLo and tp1.MaLoaiCa = tp2.MaLoaiCa and tp1.MaSize=tp2.MaSize and tp1.MaMau = tp2.MaMau and tp1.MaThanhPham = tp2.MaThanhPham and tp1.CaTra = tp2.CaTra
			) DinhMuc
		where 
			Ngay= @ngay and MaXuong = @xuongId  and p.MaLo = DinhMuc.MaLo and p.MaLoaiCa = DinhMuc.MaLoaiCa and p.MaSize = DinhMuc.MaSize and p.MaMau = DinhMuc.MaMau and p.MaThanhPham = DinhMuc.MaThanhPham And p.CaTra = DinhMuc.CaTra and p.STT >0
		group by  
			p.MaNhanVien, p.MaLo,p.MaLoaiCa,p.MaSize,p.MaThanhPham,p.MaMau,p.CaTra,DinhMuc.DinhMuc 
	) p,
	MaThanhPhamFillet tp,
	MaSizeFillet s, 
	MaLoaiCaFillet la,
	MaMauFillet ma, 
	NhanVienDaiThanh n 
where 
	p.MaThanhPham = tp.Ma and p.MaLoaiCa = tp.MaCa and p.MaLoaiCa = la.Ma and p.MaSize = s.Ma and p.MaMau = ma.Ma and p.MaNhanVien = n.MaNhanVien 
group by 
	p.MaNhanVien,n.MaHoSo,n.[Name],la.Ten,s.Ten,tp.Ten,tp.Ma,ma.Ten ,p.CaTra,p.DinhMucYeuCau 
order by 
	n.MaHoSo, tp.Ma,p.DinhMucYeuCau";
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

    /// <summary>
    ///     Cach tinh theo ty le tron ca
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="dateTime"></param>
    /// <param name="xuongId"></param>
    /// <param name="khuVucId"></param>
    /// <returns></returns>
    public List<TEntity> GetPhieuCanTongHopTinhLuong<TEntity>(
        DateTime dateTime,
        string xuongId,
        string khuVucId,
        string loaiDonGiaId = "DM")
    {
        try
        {
            var query =
                @"Select
    p.*,
    ISNULL(dg.DonGia, 0) as DonGia,
    ISNULL(dg.DonGia * p.TrongLuongTra * dg.HeSo, 0) as ThanhTien
from
    (
        Select
            p.MaNhanVien,
            n.MaHoSo,
            n.[Name] as TenNhanVien,
            la.Ten As LoaiCaName,
            s.Ten As SizeName,
            p.MaThanhPhamOrg,
            p.ThanhPhamNameOrg,
            tp.Ten as ThanhPhamName,
            tp.Ma As MaThanhPham,
            p.TyLe,
            ma.Ten as MauName,
            p.CaTra,
            SUM(p.TrongLuongNhanOrg) as TrongLuongNhanOrg,
            Cast(Sum(p.TrongLuongNhan) as decimal(18, 2)) as TrongLuongNhan,
            SUM(p.TrongLuongTraOrg) as TrongLuongTraOrg,
            CAST(Sum(p.TrongLuongTra) as decimal(18, 2)) as TrongLuongTra,
            (
                FLOOR(
                    (
                        Sum(p.TrongLuongNhanOrg) / Sum(p.TrongLuongTraOrg)
                    ) * 100
                ) / 100
            ) as DinhMucThucTeOrg,
            (
                FLOOR(
                    (Sum(p.TrongLuongNhan) / Sum(p.TrongLuongTra)) * 100
                ) / 100
            ) as DinhMucThucTe,
            p.DinhMucYeuCau,
            SUM(p.SoRoOrg) as SoRoOrg,
            Sum(p.SoRo) As SoRo,
            tp.BravoId as MaSanPham,
            p.MaXuong
        from
            (
                Select
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaThanhPham as MaThanhPhamOrg,
                    _tp.Ten as ThanhPhamNameOrg,
                    tp.MaThanhPhamDes as MaThanhPham,
                    p.MaMau,
                    p.CaTra,
                    tp.TyLe,
                    p.TrongLuongNhan as TrongLuongNhanOrg,
                    tp.TyLe * p.TrongLuongNhan as TrongLuongNhan,
                    p.TrongLuongTra as TrongLuongTraOrg,
                    tp.TyLe * p.TrongLuongTra as TrongLuongTra,
                    p.DinhMucYeuCau,
                    p.SoRo as SoRoOrg,
                    Cast(p.SoRo * tp.TyLe as int) as SoRo,
                    p.MaXuong
                from
                    (
                        Select
                            p.MaNhanVien,
                            p.MaLo,
                            p.MaLoaiCa,
                            p.MaSize,
                            CASE
                                WHEN p.CaTra = 1 THEN 'B'
                                ELSE p.MaThanhPham
                            END as MaThanhPham,
                            p.MaMau,
                            p.CaTra,
                            Sum(p.TrongLuongNhan) as TrongLuongNhan,
                            Sum(p.TrongLuongTra) as TrongLuongTra,
                            DinhMuc.DinhMuc as DinhMucYeuCau,
                            Count(*) as SoRo,
                            p.MaXuong
                        from
                            PhieuCanTPFilletv2 p,
                            (
                                Select
                                    tp1.MaLo,
                                    tp1.MaLoaiCa,
                                    tp1.MaSize,
                                    tp1.MaMau,
                                    tp1.MaThanhPham,
                                    tp1.CaTra,
                                    CASE
                                        WHEN tp2.DinhMuc is Null THEN tp1.DinhMuc
                                        ELSE tp2.DinhMuc
                                    END AS DinhMuc
                                from
                                    (
                                        Select
                                            distinct p.MaLo,
                                            p.MaLoaiCa,
                                            p.MaSize,
                                            p.MaMau,
                                            p.MaThanhPham,
                                            p.CaTra,
                                            CASE
                                                WHEN p.CaTra = 1 THEN 1.37
                                                ELSE tp.DinhMuc
                                            END AS DinhMuc
                                        from
                                            PhieuCanTPFilletv2 p,
                                            MaThanhPhamFillet tp
                                        where
                                            Ngay = @ngay
                                            and MaXuong = @xuongId
                                            and p.MaThanhPham = tp.Ma
                                            and p.MaLoaiCa = tp.MaCa
                                    ) tp1
                                    LEFT JOIN (
                                        Select
                                            MaLo,
                                            MaLoaiCa,
                                            MaSize,
                                            MaMau,
                                            MaThanhPham,
                                            CaTra,
                                            DinhMuc
                                        from
                                            (
                                                Select
                                                    d.*,
                                                    ROW_NUMBER() OVER (
                                                        PARTITION BY MaLo,
                                                        MaLoaiCa,
                                                        MaMau,
                                                        MaSize,
                                                        MaThanhPham,
                                                        CaTra
                                                        ORDER BY
                                                            Gio DESC
                                                    ) AS [ROW NUMBER]
                                                from
                                                    DinhMucFillet d
                                                where
                                                    Ngay = @ngay
                                                    And MaXuong = @xuongId
                                            ) dm
                                        Where
                                            dm.[ROW NUMBER] = 1
                                    ) tp2 on tp1.MaLo = tp2.MaLo
                                    and tp1.MaLoaiCa = tp2.MaLoaiCa
                                    and tp1.MaSize = tp2.MaSize
                                    and tp1.MaMau = tp2.MaMau
                                    and tp1.MaThanhPham = tp2.MaThanhPham
                                    and tp1.CaTra = tp2.CaTra
                            ) DinhMuc
                        where
                            p.Ngay = @ngay
                            and p.MaXuong = @xuongId
                            and p.MaLo = DinhMuc.MaLo
                            and p.MaLoaiCa = DinhMuc.MaLoaiCa
                            and p.MaSize = DinhMuc.MaSize
                            and p.MaMau = DinhMuc.MaMau
                            and p.MaThanhPham = DinhMuc.MaThanhPham
                            And p.CaTra = DinhMuc.CaTra
                            and p.STT > 0
                        group by
                            p.MaNhanVien,
                            p.MaLo,
                            p.MaLoaiCa,
                            p.MaSize,
                            p.MaThanhPham,
                            p.MaMau,
                            p.CaTra,
                            DinhMuc.DinhMuc,
                            p.MaXuong
                    ) p,
                    (
                        select
                            (@ngay) as Ngay,
                            (@khuVucId) as MaKhuVuc,
                            (@xuongId) as MaXuong,
                            ISNULL(tppt.MaLo, tp.MaLo) as MaLo,
                            tp.MaThanhPham as MaThanhPhamOrg,
                            ISNULL(tppt.MaThanhPhamDes, tp.MaThanhPham) as MaThanhPhamDes,
                            ISNULL(tppt.TyLe, 1) as TyLe
                        from
                            (
                                Select
                                    (@ngay) as Ngay,
                                    (@xuongId) as MaXuong,
                                    p.MaLo,
                                    tp.Ma as MaThanhPham
                                from
                                    MaThanhPhamFillet tp,
                                    (
                                        Select
                                            distinct MSL as MaLo
                                        from
                                            PhieuCanNguyenLieu
                                        where
                                            Ngay = @ngay
                                            and MaXuongSanXuat = @xuongId
                                    ) p
                            ) tp
                            LEFT join (
                                Select
                                    *
                                from
                                    MaThanhPham_PhoiTron
                                where
                                    Ngay = @ngay
                                    and MaXuong = @xuongId
                                    and MaKhuVuc = @khuVucId
                            ) tppt ON tp.MaThanhPham = tppt.MaThanhPhamOrg
                            and tp.Ngay = tppt.Ngay
                            and tp.MaXuong = tppt.MaXuong
                            and tp.MaLo = tppt.MaLo
                    ) tp,
                    MaThanhPhamFillet _tp
                where
                    p.MaThanhPham = tp.MaThanhPhamOrg
                    and p.MaLo = tp.MaLo
                    and p.MaThanhPham = _tp.ma
            ) p,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaLoaiCaFillet la,
            MaMauFillet ma,
            NhanVienDaiThanh n
        where
            p.MaThanhPham = tp.Ma
            and p.MaLoaiCa = tp.MaCa
            and p.MaLoaiCa = la.Ma
            and p.MaSize = s.Ma
            and p.MaMau = ma.Ma
            and p.MaNhanVien = n.MaNhanVien
        group by
            p.MaNhanVien,
            n.MaHoSo,
            n.[Name],
            la.Ten,
            s.Ten,
            tp.Ten,
            tp.Ma,
            ma.Ten,
            p.CaTra,
            p.DinhMucYeuCau,
            p.MaThanhPhamOrg,
            p.ThanhPhamNameOrg,
            p.TyLe,
            tp.BravoId,
            p.MaXuong
    ) p
    LEFT JOIN (
        Select
            *
        from
            (
                Select
                    *,
                    ROW_NUMBER() over (
                        partition by MaSanPham,
                        DanhGia,
                        DinhMucUp,
                        DinhMucDown
                        order by
                            Ngay DESC,
                            Gio Desc
                    ) as rowId
                from
                    DG_DonGia
                where
                    Ngay <= @ngay
                    and MaLoaiDonGia = @loaiDonGiaId and LoaiCan=''
            ) p
        where
            p.rowId = 1
    ) dg ON p.MaSanPham = dg.MaSanPham
    and p.DinhMucThucTe <= dg.DinhMucUp
    and p.DinhMucThucTe >= dg.DinhMucDown
order by
    p.MaHoSo,
    p.MaThanhPhamOrg,
    p.TyLe,
    p.DinhMucYeuCau";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.QueryAsync<TEntity>(
                    query,
                    new { ngay = dateTime.Date, xuongId, khuVucId, loaiDonGiaId })
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
public List<T> GetTongHopTyLeThoiGianVaDinhMuc<T>( DateTime fromDate,DateTime toDate,string xuongId,int MocThoiGian)
{
        var query = @"	select
    *
from
(
        Select
            category = 'DinhMuc',
            cast(
                p.SoLuongDatDinhMuc /(p.SoLuongDatDinhMuc + p.SoLuongKhongDatDinhMuc) as DECIMAL(18, 4)
            ) as TyLeDat,
            cast(
                p.SoLuongKhongDatDinhMuc /(p.SoLuongDatDinhMuc + p.SoLuongKhongDatDinhMuc) as DECIMAL(18, 4)
            ) as TyLeKhongDat,
            SoRoDat,
            SoRoKhongDat
        from
            (
                Select
                    cast(
                        sum(
                            case
                                when DanhGia = 1 then 1
                                else 0
                            end
                        ) as decimal(18, 2)
                    ) as SoLuongDatDinhMuc,
                    cast(
                        sum(
                            case
                                when DanhGia = 0 then 1
                                else 0
                            end
                        ) as DECIMAL(18, 2)
                    ) as SoLuongKhongDatDinhMuc,
                    cast(
                        sum(
                            case
                                when DanhGia = 1 then SoLuong
                                else 0
                            end
                        ) as decimal(18, 2)
                    ) as SoRoDat,
                    cast(
                        sum(
                            case
                                when DanhGia = 0 then SoLuong
                                else 0
                            end
                        ) as DECIMAL(18, 2)
                    ) as SoRoKhongDat
                from
                    (
                        SELECT
                            p.*,
                            case
                                WHEN p.DinhMucThuc < p.DinhMucChuan THEN 1
                                ELSE 0
                            END AS DanhGia
                        from
                            (
                                Select
                                    p.*,
                                    cast(
                                        p.TrongLuongNhan / p.TrongLuongTra as DECIMAL(18, 2)
                                    ) as DinhMucThuc,
                                    Cast(
                                        ISNULL(dm.DinhMuc, tp.DinhMuc) as DECIMAL(18, 2)
                                    ) as DinhMucChuan
                                from
                                    (
                                        Select
                                            Ngay,
                                            MaNhanVien,
                                            MaThanhPham,
                                            MaLo,
                                            MaLoaiCa,
                                            MaMau,
                                            MaSize,
                                            Sum(TrongLuongNhan) as TrongLuongNhan,
                                            sum(TrongLuongTra) as TrongLuongTra,
                                            Count(*) as SoLuong
                                        from
                                            PhieuCanTPFilletv2 p
                                        where
                                            p.Ngay >= @fromDate and p.Ngay <= @toDate and p.MaXuong = @xuongId and p.TrongLuongTra >0
                                        GROUP BY
                                            Ngay,
                                            MaNhanVien,
                                            MaThanhPham,
                                            MaLo,
                                            MaLoaiCa,
                                            MaMau,
                                            MaSize
                                    ) p
                                    LEFT JOIN (
                                        Select
                                            *
                                        from
                                            (
                                                Select
                                                    d.*,
                                                    ROW_NUMBER() OVER(
                                                        PARTITION BY MaLo,
                                                        MaLoaiCa,
                                                        MaMau,
                                                        MaSize,
                                                        MaThanhPham
                                                        ORDER BY
                                                            Gio DESC
                                                    ) AS [ROW NUMBER]
                                                from
                                                    DinhMucFillet d
                                                where
                                                    Ngay >= @fromDate and Ngay <= @toDate and MaXuong = @xuongId
                                            ) dm
                                        Where
                                            dm.[ROW NUMBER] = 1
                                    ) dm on p.MaThanhPham = dm.MaThanhPham
                                    and p.MaLo = dm.MaLo
                                    and p.MaSize = dm.MaSize
                                    and p.MaMau = dm.MaMau
                                    and p.MaLoaiCa = dm.MaLoaiCa
                                    and p.Ngay = dm.Ngay
                                    LEFT JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
                            ) p
                    ) p
            ) p
    ) p1
UNION ALL
    
        Select
            category = 'ThoiGian',
            cast(
                p.SoLuongDatDinhMuc /(p.SoLuongDatDinhMuc + p.SoLuongKhongDatDinhMuc) as DECIMAL(18, 4)
            ) as TyLeDat,
            cast(
                p.SoLuongKhongDatDinhMuc /(p.SoLuongDatDinhMuc + p.SoLuongKhongDatDinhMuc) as DECIMAL(18, 4)
            ) as TyLeKhongDat,
           p.SoLuongDatDinhMuc as SoRoDat,
           p.SoLuongKhongDatDinhMuc as SoRoKhongDat
        from
            (
                Select
                    cast(
                        sum(
                            case
                                when DanhGia = 1 then 1
                                else 0
                            end
                        ) as decimal(18, 2)
                    ) as SoLuongDatDinhMuc,
                    cast(
                        sum(
                            case
                                when DanhGia = 0 then 1
                                else 0
                            end
                        ) as DECIMAL(18, 2)
                    ) as SoLuongKhongDatDinhMuc
                  
                from
                    (
                        Select
                            case
                                when DATEDIFF(
                                    MINUTE,
                                    cast(btp.Gio as datetime),
                                    cast(tp.Gio as datetime)
                                ) <= @MocThoiGian then 1
                                else 0
                            end as DanhGia
                        from
                            PhieuCanTPFilletv2 tp
                            left join PhieuCanBTPFilletv2 btp on btp.Id = tp.IdIn
                        where
                            tp.Ngay >= @fromDate and tp.Ngay <= @toDate
							and btp.Ngay >= @fromDate and btp.Ngay <= @toDate
                            and tp.MaXuong = @xuongId  and btp.TrongLuong >0
                            --and tp.IdIn = btp.Id
                    ) p
            ) p";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(
            query,
            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, MocThoiGian }).Result.ToList();
        return items;
    }
    public List<T> GetTongHopTyLeThoiGianVaDinhMucForGrid<T>(DateTime fromDate, DateTime toDate, string xuongId, int MocThoiGian)
    {
        var query = @"	SELECT *
FROM (
    -- PHẦN 1: Định Mức
    SELECT
        category = 'DinhMuc',
        Ngay,
        CAST(p.SoLuongDatDinhMuc / NULLIF((p.SoLuongDatDinhMuc + p.SoLuongKhongDatDinhMuc), 0) AS DECIMAL(18, 4)) AS TyLeDat,
        CAST(p.SoLuongKhongDatDinhMuc / NULLIF((p.SoLuongDatDinhMuc + p.SoLuongKhongDatDinhMuc), 0) AS DECIMAL(18, 4)) AS TyLeKhongDat,
        SoRoDat,
        SoRoKhongDat
    FROM (
        SELECT
            Ngay,
            CAST(SUM(CASE WHEN DanhGia = 1 THEN 1 ELSE 0 END) AS DECIMAL(18, 2)) AS SoLuongDatDinhMuc,
            CAST(SUM(CASE WHEN DanhGia = 0 THEN 1 ELSE 0 END) AS DECIMAL(18, 2)) AS SoLuongKhongDatDinhMuc,
            CAST(SUM(CASE WHEN DanhGia = 1 THEN SoLuong ELSE 0 END) AS DECIMAL(18, 2)) AS SoRoDat,
            CAST(SUM(CASE WHEN DanhGia = 0 THEN SoLuong ELSE 0 END) AS DECIMAL(18, 2)) AS SoRoKhongDat
        FROM (
            SELECT
                p.Ngay,
                p.SoLuong,
                CASE WHEN p.DinhMucThuc < p.DinhMucChuan THEN 1 ELSE 0 END AS DanhGia
            FROM (
                SELECT
                    p.*,
                    CAST(p.TrongLuongNhan / NULLIF(p.TrongLuongTra, 0) AS DECIMAL(18, 2)) AS DinhMucThuc,
                    CAST(ISNULL(dm.DinhMuc, tp.DinhMuc) AS DECIMAL(18, 2)) AS DinhMucChuan
                FROM (
                    SELECT
                        Ngay,
                        MaNhanVien,
                        MaThanhPham,
                        MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        SUM(TrongLuongNhan) AS TrongLuongNhan,
                        SUM(TrongLuongTra) AS TrongLuongTra,
                        COUNT(*) AS SoLuong
                    FROM PhieuCanTPFilletv2
                    WHERE Ngay >= @fromDate AND Ngay <= @toDate
                    GROUP BY Ngay, MaNhanVien, MaThanhPham, MaLo, MaLoaiCa, MaMau, MaSize
                ) p
                LEFT JOIN (
                    SELECT *
                    FROM (
                        SELECT
                            d.*,
                            ROW_NUMBER() OVER (
                                PARTITION BY MaLo, MaLoaiCa, MaMau, MaSize, MaThanhPham
                                ORDER BY Gio DESC
                            ) AS rn
                        FROM DinhMucFillet d
                        WHERE Ngay >= @fromDate AND Ngay <= @toDate AND MaXuong = @xuongId
                    ) dm
                    WHERE rn = 1
                ) dm ON p.MaThanhPham = dm.MaThanhPham
                        AND p.MaLo = dm.MaLo
                        AND p.MaSize = dm.MaSize
                        AND p.MaMau = dm.MaMau
                        AND p.MaLoaiCa = dm.MaLoaiCa
                        AND p.Ngay = dm.Ngay
                LEFT JOIN MaThanhPhamFillet tp ON p.MaThanhPham = tp.Ma
            ) p
        ) p
        GROUP BY Ngay
    ) p

    UNION ALL

    -- PHẦN 2: Thời Gian
    SELECT
        category = 'ThoiGian',
        Ngay,
        CAST(p.SoLuongDatDinhMuc / NULLIF((p.SoLuongDatDinhMuc + p.SoLuongKhongDatDinhMuc), 0) AS DECIMAL(18, 4)) AS TyLeDat,
        CAST(p.SoLuongKhongDatDinhMuc / NULLIF((p.SoLuongDatDinhMuc + p.SoLuongKhongDatDinhMuc), 0) AS DECIMAL(18, 4)) AS TyLeKhongDat,
        SoLuongDatDinhMuc AS SoRoDat,
        SoLuongKhongDatDinhMuc AS SoRoKhongDat
    FROM (
        SELECT
            Ngay,
            CAST(SUM(CASE WHEN DanhGia = 1 THEN 1 ELSE 0 END) AS DECIMAL(18, 2)) AS SoLuongDatDinhMuc,
            CAST(SUM(CASE WHEN DanhGia = 0 THEN 1 ELSE 0 END) AS DECIMAL(18, 2)) AS SoLuongKhongDatDinhMuc
        FROM (
            SELECT
                tp.Ngay,
                CASE
                    WHEN DATEDIFF(MINUTE, CAST(btp.Gio AS datetime), CAST(tp.Gio AS datetime)) <= @MocThoiGian THEN 1
                    ELSE 0
                END AS DanhGia
            FROM PhieuCanTPFilletv2 tp
            LEFT JOIN PhieuCanBTPFilletv2 btp ON btp.Id = tp.IdIn
            WHERE tp.Ngay >= @fromDate AND tp.Ngay <= @toDate
                AND btp.Ngay >= @fromDate AND btp.Ngay <= @toDate
                AND tp.MaXuong = @xuongId
        ) p
        GROUP BY Ngay
    ) p
) final
ORDER BY Ngay, category";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(
            query,
            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, MocThoiGian }).Result.ToList();
        return items;
    }
    public List<TEntity> GetPhieuCanTongHopTinhLuongType1_2<TEntity>(
        DateTime dateTime,
        string xuongId,
        string khuVucId,
        string loaiDonGiaId = "DM")
    {
        try
        {
            var query =
                @"
SELECT p.*,
    map.MaBravoFillet as [BravoId],
    map.MaBravoFillet as [MaSanPham],
    cast(0 as decimal(18,2)) as DonGia,
    cast(0 as decimal(18,2)) as ThanhTien
from (
        Select p.Ngay,
            p.MaNhanVien as [MaNhanVien],
            n.MaHoSo as [MaHoSo],
            n.Name as [TenNhanVien],
            b.Ten as [BanName],
            p.SoBanDuocSap as [SoBanDuocSap],
            tp.Ten as [ThanhPhamName],
            p.TrongLuong as [TrongLuongTra],
            p.SoRo as [SoRo],
            cast(pBan.TrongLuongNhan as DECIMAL(18, 2)) as [TLNhanBan],
            pBan.TrongLuongTra as [TLTraBan],
            pBan.DinhMuc as [DinhMucThucTe],
            ISNULL(dm.DinhMuc, 0) as [DinhMucYeuCau],
            cast(
                case
                    when pBan.DinhMuc > dm.DinhMuc then 0
                    else 1
                end as bit
            ) as DanhGia,
            pBan.SoRoNhan as [SoRoNhanBan],
            pBan.SoRoTra as [SoRoTraBan],
            p.MaXuong,
            p.IsTangCa,
            p.MaThanhPham,
            p.MaSize,
            p.MaLoaiCa,
            p.MaBan
        from (
                select p.Ngay,
                    p.MaNhanVien,
                    p.MaBan,
                    '0' as MaSize,
                    'A' as MaLoaiCa,
                    (
                        dense_rank() over (
                            PARTITION BY p.Ngay,
                            p.MaNhanVien
                            order by p.MaBan
                        ) + dense_rank() over (
                            PARTITION BY p.Ngay,
                            p.MaNhanVien
                            order by p.MaBan desc
                        ) -1
                    ) as SoBanDuocSap,
                    p.MaThanhPham,
                    p.MaXuong,
                    cast(0 as bit) as IsTangCa,
                    SUM(p.TrongLuongTra) as TrongLuong,
                    COUNT(*) as SoRo
                from PhieuCanTPFilletv2 p
                where p.Ngay <= @ngay
                    and p.Ngay >= @fromDate
                    and p.MaXuong = @xuongId
                    and p.STT > 0
                GROUP BY p.Ngay,
                    p.MaNhanVien,
                    p.MaThanhPham,
                    p.MaXuong,
                    p.MaBan
            ) p
            LEFT JOIN (
                Select p.Ngay,
                    p.MaBan,
                    p.MaThanhPham,
                    p.MaXuong,
                    SUM(p.TrongLuongTra) as TrongLuongTra,
                    SUM(p.TrongLuongNhan) as TrongLuongNhan,
                    Cast(
                        case
                            when ISNULL(SUM(p.TrongLuongTra), 0) = 0 then 0
                            else SUM(p.TrongLuongNhan) / SUM(p.TrongLuongTra)
                        end as DECIMAL(18, 2)
                    ) as DinhMuc,
                    SUM(p.SoRoTra) as SoRoTra,
                    sum(p.SoRoNhan) as SoRoNhan
                from (
                        Select p.*,
                            t.TrongLuongNhan,
                            t.SoRoNhan
                        from(
                                select p.Ngay,
                                    p.MaBan,
                                    p.MaThanhPham,
                                    p.MaXuong,
                                    SUM(p.TrongLuongTra) as TrongLuongTra,
                                    COUNT(*) as SoRoTra
                                from PhieuCanTPFilletv2 p
                                where p.Ngay <= @ngay
                                    and p.Ngay >= @fromDate
                                    and p.MaXuong = @xuongId
                                    and p.STT > 0
                                GROUP BY p.Ngay,
                                    p.MaThanhPham,
                                    p.MaXuong,
                                    p.MaBan
                            ) p
                            LEFT JOIN (
                                select p.Ngay,
                                    p.MaBan,
                                    p.MaThanhPham,
                                    p.MaXuong,
                                    SUM(p.TrongLuongNhan) as TrongLuongNhan,
                                    count(*) as SoRoNhan
                                from ThePhieuSanLuongFillet p
                                where p.Ngay <= @ngay
                                    and p.Ngay >= @fromDate
                                    and p.MaXuong = @xuongId
                                GROUP BY p.Ngay,
                                    p.MaBan,
                                    p.MaXuong,
                                    p.MaThanhPham
                            ) t on p.Ngay = t.Ngay
                            and p.MaXuong = t.MaXuong
                            and p.MaBan = t.MaBan
                            and p.MaThanhPham = t.MaThanhPham
                    ) p
                GROUP BY p.Ngay,
                    p.MaBan,
                    p.MaThanhPham,
                    p.MaXuong
            ) pBan on p.Ngay = pBan.Ngay
            and p.MaXuong = pBan.MaXuong
            and p.MaThanhPham = pBan.MaThanhPham
            and p.MaBan = pBan.MaBan
            LEFT JOIN BanFillet b on p.MaBan = b.Ma
            LEFT JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
            LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            LEFT JOIN (
                Select *
                from (
                        Select d.*,
                            ROW_NUMBER() OVER (
                                PARTITION BY MaThanhPham
                                ORDER BY Gio DESC
                            ) AS [ROW NUMBER]
                        from DinhMucFillet d
                        where Ngay = @ngay
                            And MaXuong = @xuongId
                    ) dm
                Where dm.[ROW NUMBER] = 1
            ) dm on p.MaThanhPham = dm.MaThanhPham
    ) p
    LEFT JOIN (
        select map.*,
            isnull(dg.DonGia, 0) as DonGia,
            isnull(dg.HeSo, 1) as HeSo
        from MapThanhPhamFillet map
            LEFT JOIN (
                Select *
                from (
                        Select *,
                            ROW_NUMBER() over (
                                partition by MaSanPham,
                                DanhGia
                                order by Ngay DESC,
                                    Gio Desc
                            ) as rowId
                        from DG_DonGia
                        where Ngay <= @ngay
                            and MaLoaiDonGia = 'DG'
                            and LoaiCan = ''
                    ) p
                where p.rowId = 1
            ) dg ON dg.MaSanPham = map.MaBravoFillet
    ) map on p.MaThanhPham = map.MaTPFillet
    and p.IsTangCa = map.IsTangCa
    and p.MaSize = map.MaSize
    and p.MaLoaiCa = map.MaLoaiCaFillet
    and p.DanhGia = map.DonGia
order by p.ngay,
    p.MaBan,
    p.MaHoSo,
    p.MaThanhPham
";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.QueryAsync<TEntity>(
                    query,
                    new { ngay = dateTime.Date, fromDate = dateTime.Date, xuongId, khuVucId, loaiDonGiaId })
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

    #region XLPC Filletv2

    public List<T> GetPhieuCanTPFilletv2_XLPC<T>(DateTime dateTime, string xuongId)
    {
        var query = @"select
p.STT,
p.Ngay,
p.Gio,
p.MaUserCan,
p.MaMayCan,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaMau,
m.Ten as MauName,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLo,
P.MaThe,
p.TrongLuongNhan,
p.TrongLuongTra,
p.DinhMucThucTe,
p.DinhMucYeuCau,
p.MaXuong,
x.Ten as XuongName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
nv.DeptName0 as Nhom,
p.CaTra,
p.STTBTP,
p.MaMayCanBTP,
p.GhiChu,
p.TrongLuongTare
from PhieuCanTPFilletv2 p
left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
left join MaMauFillet m on p.MaMau = m.Ma
left join MaSizeFillet s on p.MaSize = s.Ma
left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
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

    public List<T> Gets<T>(DateTime dateTime, string xuongId)
    {
        var query =
            @"Select * from PhieuCanTPFilletv2 Where Ngay = @ngay and MaXuong=@xuongId order by STT DESC";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result
                .ToList();
            return items;
        }
    }

    public List<T> Gets<T>(DateTime dateTime, string xuongId, string mayCanId)
    {
        var query =
            @"Select * from PhieuCanTPFilletv2 Where Ngay = @ngay and MaXuong=@xuongId and MaMayCan= @mayCanId order by STT DESC";
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

    public List<T> Gets<T>(DateTime dateTime, string xuongId, string mayCanId, int stt)
    {
        var query =
            @"Select * from PhieuCanTPFilletv2 Where Ngay = @ngay and MaXuong=@xuongId and MaMayCan= @mayCanId and STT >@stt order by STT DESC";
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

    public List<T> Gets<T>(DateTime dateTime, string xuongId, string nhanVienId, bool isPhucVu = true)
    {
        var query = @"SELECT p.MaNhanVien,
    n.Name as NhanVienName,
    p.MaLoaiCa,
    p.MaLo,
    p.MaThanhPham,
    p.MaSize,
    Count(p.TrongLuongTra) as SoRo,
    CAST(Sum(p.TrongLuongTra) as decimal(18, 2)) as TrongLuong
FROM PhieuCanTPFilletv2 p,
    NhanVienDaiThanh n
Where p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = @nhanVienId
    and p.MaNhanVien = n.MaNhanVien
Group By p.MaNhanVien,
    p.MaLo,
    p.MaLoaiCa,
    p.MaSize,
    p.MaThanhPham,
    n.Name";
        if (isPhucVu)
            query = @"SELECT p.MaNhanVienPhucVu as MaNhanVien,
    n.Name as NhanVienName,
    p.MaLoaiCa,
    p.MaLo,
    p.MaThanhPham,
    p.MaSize,
    Count(p.TrongLuongTra) as SoRo,
    CAST(Sum(p.TrongLuongTra) as decimal(18, 2)) as TrongLuong
FROM PhieuCanTPFilletv2 p,
    NhanVienDaiThanh n
Where p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVienPhucVu = @nhanVienId
    and p.MaNhanVien = n.MaNhanVien
Group By p.MaNhanVienPhucVu,
    p.MaLo,
    p.MaLoaiCa,
    p.MaSize,
    p.MaThanhPham,
    n.name";

        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId, nhanVienId }).ToList();
        return items;
    }

    public List<T> Gets<T>()
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var rows = connection.Query<T>(qrGetAll).ToList();
        return rows;
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
            $@"Select Isnull(sum(p.TrongLuongTra),0) from PhieuCanTPFilletv2 p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaThanhPham in {listOfIdsJoined} and p.Gio >= @fromTime and p.Gio < @toTime ";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var item = (decimal)connection.ExecuteScalar(
                query,
                new { ngay = dateTime.Date, fromTime, toTime, xuongId });
            return item;
        }
    }

    public double GetSanLuong(
        TimeSpan fromTime,
        TimeSpan toTime,
        DateTime dateTime,
        string xuongId,
        IEnumerable<string> ids)
    {
        try
        {
            var listOfIdsJoined = "('" + string.Join("','", ids.ToArray()) + "')";
            var query =
                $@"Select ISNULL(SUM(TrongLuong),0) from PhieuCanTPFillet where Ngay = @ngay and CONVERT(time,ThoiGianCan) >= @fromTime and CONVERT(time,ThoiGianCan)< @toTime and SuDung = 1 and MaXuongSanXuat = @xuongId and MaNhanVien in {listOfIdsJoined}";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.ExecuteScalar<double>(
                query,
                new { ngay = dateTime.Date, fromTime, toTime, xuongId });
            return items;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public decimal GetSanLuongTras(
        DateTime dateTime,
        string xuongId,
        List<Models.Repos.Models.MaThanhPhamExDinhHinh> thanhPhamExFillets)
    {
        try
        {
            var idsEx = thanhPhamExFillets.Select(x => x.Id).Distinct();
            var query =
                @"Select isnull( Sum(TrongLuongTra),0) from PhieuCanTPFilletv2 where Ngay=@ngay and MaXuong=@xuongId  and MaThanhPham Not In @idsEx";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var item = (decimal)connection.ExecuteScalarAsync(
                        query,
                        new { ngay = dateTime.Date, xuongId, idsEx })
                    .Result;
                return item;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public decimal GetSanLuongTras(
        DateTime dateTime,
        string xuongId,
        List<Models.Repos.Models.NhanVienDaiThanh> nhanVienDaiThanhs,
        List<Models.Repos.Models.MaThanhPhamExDinhHinh> thanhPhamExFillets)
    {
        try
        {
            var ids = nhanVienDaiThanhs.Select(x => x.MaNhanVien).Distinct();
            var listOfIdsJoined = "('" + string.Join("','", ids.ToArray()) + "')";
            var idsEx = thanhPhamExFillets.Select(x => x.Id).Distinct();
            var query =
                $@"Select isnull( Sum(TrongLuongTra),0) from PhieuCanTPFilletv2 where Ngay=@ngay and MaXuong=@xuongId and MaNhanVien In {listOfIdsJoined} and MaThanhPham Not In @idsEx";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var item = (decimal)connection.ExecuteScalarAsync(
                        query,
                        new { ngay = dateTime.Date, xuongId, idsEx })
                    .Result;
                return item;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public double GetSanLuongTruCaMuoi(TimeSpan fromTime, TimeSpan toTime, DateTime dateTime, string xuongId)
    {
        try
        {
            var query =
                "Select ISNULL(SUM(p.TrongLuong),0) from PhieuCanTPFillet p,MaThanhPhamFillet tp where p.Ngay = @ngay and CONVERT(time,p.ThoiGianCan) >= @fromTime and CONVERT(time,p.ThoiGianCan) < @toTime and p.SuDung = 1 and p.MaXuongSanXuat = @xuongId and p.MaLoaiThanhPham = tp.ma and tp.IsCaMuoi = 0";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.ExecuteScalar<double>(
                    query,
                    new { ngay = dateTime.Date, fromTime, toTime, xuongId });
                return items;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public double GetSanLuongv2(TimeSpan fromTime, TimeSpan toTime, DateTime dateTime, string xuongId)
    {
        try
        {
            var query =
                "Select ISNULL(SUM(TrongLuongTra),0) from PhieuCanTPFilletv2 where Ngay = @ngay and CONVERT(time,Gio) >= @fromTime and CONVERT(time,Gio) < @toTime  and MaXuong = @xuongId";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.ExecuteScalar<double>(
                    query,
                    new { ngay = dateTime.Date, fromTime, toTime, xuongId });
                return items;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public List<T> GetsLast<T>(DateTime dateTime, int num)
    {
        var query = @"WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MaMayCan ORDER BY Ngay DESC, Gio DESC) AS RowNum
    FROM PhieuCanTPFilletv2 where Ngay =@ngay
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
    public Tuple<int, decimal> GetSoRoTongTrongLuongByNhanVienId(DateTime dateTime, string nhanVienId)
    {
        var query =
            @"Select IsNull( Count(*),0) as Item1,isNull( Sum(TrongLuongTra),0) As Item2 from PhieuCanTPFilletv2 WITH(READPAST) where MaNhanVien = @nhanVienId and Ngay =@ngay";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var row = connection.Query<Tuple<int, decimal>>(
                    query,
                    new { ngay = dateTime.Date, nhanVienId })
                .SingleOrDefault();
            return row;
        }
    }
    public List<T> GetsLastMinutes<T>(int minu)
    {
        var now = DateTime.Now;
        var fromTime = now.AddMinutes(-1 * minu).TimeOfDay;
        var toTime = now.TimeOfDay;

        var ngay = now.Date;

        var query = @"
Select
    p.*,
    btp.Gio as GioBTP,
    Cast(
        DATEDIFF(MINUTE, btp.Gio, p.Gio) as DECIMAL(18, 2)
    ) AS ThoiGianHT,
    (
        case
            When p.DinhMucThucTe > p.DinhMucChuan then 0
            else 1
        end
    ) as DanhGia
    

from
    (
        Select
            p.*,ISNULL(DinhMuc.DinhMuc,tp.DinhMuc) as DinhMucChuan
        from
            (
                SELECT
                    TOP 100 *
                FROM
                    PhieuCanTPFilletv2 With(NOLOCK)
                WHERE
                    Ngay = @ngay
                AND Gio BETWEEN @fromTime AND @toTime
            ) p
            LEFT JOIN (
                Select
                    tp1.MaLo,
                    tp1.MaSize,
                     tp1.MaThanhPham,
                    CASE
                        WHEN tp2.DinhMuc is Null THEN tp1.DinhMuc
                        ELSE tp2.DinhMuc
                    END AS DinhMuc,
                    tp1.Ngay,
                    tp1.MaXuong
                from
                    (
                        Select
                            distinct p.MaLo,
                            p.MaSize,
                            p.MaThanhPham,
                            CASE
                                WHEN p.CaTra = 1 THEN 1.37
                                ELSE tp.DinhMuc
                            END AS DinhMuc,
                            p.Ngay,
                            p.MaXuong
                        from
                            PhieuCanTPFilletv2 p,
                            MaThanhPhamFillet tp
                        where
                            p.Ngay = @ngay
                            and p.MaThanhPham = tp.Ma
                    ) tp1
                    LEFT JOIN (
                        Select
                            MaLo,
                            MaSize,
                            MaThanhPham,
                            CaTra,
                            DinhMuc,
                            Ngay,
                            MaXuong
                        from
                            (
                                Select
                                    d.*,
                                    ROW_NUMBER() OVER (
                                        PARTITION BY MaLo,
                                        MaSize,
                                        MaThanhPham,
                                        Ngay
                                        ORDER BY
                                            Gio DESC
                                    ) AS [ROW NUMBER]
                                from
                                    DinhMucFillet d
                                where
                                    d.Ngay =@ngay
                            ) dm
                        Where
                            dm.[ROW NUMBER] = 1
                    ) tp2 on tp1.MaLo = tp2.MaLo
                    and tp1.MaSize = tp2.MaSize
                    and tp1.MaThanhPham = tp2.MaThanhPham
                    and tp1.Ngay = tp2.Ngay
                    and tp1.MaXuong = tp2.MaXuong
            ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
            and p.MaLo = DinhMuc.MaLo
            and p.MaSize = DinhMuc.MaSize
            and p.MaThanhPham = DinhMuc.MaThanhPham
            and p.Ngay = DinhMuc.Ngay
            LEFT JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
    ) p,
    PhieuCanBTPFilletv2 btp
where
    p.IdIn = btp.Id
order by
    p.Gio desc
         
";

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.Query<T>(query, new { ngay, fromTime, toTime }).ToList();
            return items;
        }
    }

    // public Tuple<int, decimal> GetSoRoTongTrongLuongByNhanVienId(DateTime dateTime, string nhanVienId)
    // {
    //     var query =
    //         @"Select IsNull( Count(*),0) as Item1,isNull( Sum(TrongLuongTra),0) As Item2 from PhieuCanTPFilletv2 where MaNhanVien = @nhanVienId and Ngay =@ngay";
    //     using (var connection = new SqlConnection(connectionString))
    //     {
    //         connection.Open();
    //         var row = connection.Query<Tuple<int, decimal>>(
    //                 query,
    //                 new { ngay = dateTime.Date, nhanVienId })
    //             .SingleOrDefault();
    //         return row;
    //     }
    // }

    public List<T> GetsTPSoft<T>(DateTime dateTime, string xuongId)
    {
        var query =
            @"Select p.MaMayCan+'-'+p.MaXuong+'-K01-'+FORMAT(p.Ngay,'yyyy-MM-dd')+'-'+FORMAT(CAST(p.Gio as datetime),'hh:mm:ss') as [ID],p.TrongLuongTra as [TrongLuong],sp.Id as [CongDoanID],ISNULL(n.Tel,'') as [CMND],GETDATE() as [DateCreate],GetDate() as [DateSync],1 as [Status], cast( p.Ngay as datetime) + cast(p.gio as datetime) as [ThoiGian]  from PhieuCanTPFilletv2 p, MaSanPhamFilletBravo sp, NhanVienDaiThanh n where Ngay=@ngay and MaXuong =@xuongId and p.MaThanhPham = sp.DaiThanhId and p.MaNhanVien = n.MaNhanVien";
        using (var connnection = new SqlConnection(connectionString))
        {
            connnection.Open();
            var items = connnection.QueryAsync<T>(query, new { Ngay = dateTime.Date, xuongId }).Result
                .ToList();
            return items;
        }
    }

    public List<T> GetsView<T>(DateTime dateTime, string xuongId, string mayCanId)
    {
        var query =
            @"Select Top(20) p.STT,p.Gio,p.MaThe as TheId,p.MaLo,p.MaNhanVien,n.MaHoSo,n.Name as TenNhanVien,p.MaThanhPham,tp.Ten as TenThanhPham,p.CaTra,p.MaSize,s.Ten as TenSize,p.TrongLuongTra from PhieuCanTPFilletv2 p,NhanVienDaiThanh n,MaThanhPhamFillet tp,MaSizeFillet s Where p.Ngay = @ngay and p.MaXuong=@xuongId and p.MaMayCan= @mayCanId and p.MaNhanVien = n.MaNhanVien and p.MaThanhPham = tp.Ma and p.MaSize = s.Ma order by p.STT DESC";
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

    public List<T> GetTongHopDinhMuc<T>(DateTime dateTime, string xuongId)
    {
        var query =
            @"SELECT p.MaLo, la.Ten As LoaiCaName,tp.Ma As MaThanhPham,tp.Ten As ThanhPhamName,p.CaTra,COUNT(p.STT) As SoRo,SUM(p.TrongLuongTra) as TrongLuongTra,SUM(p.TrongLuongNhan) as TrongLuongNhan,FLOOR((SUM(p.TrongLuongNhan)/SUM(p.TrongLuongTra)) *100)/100 as DinhMuc from PhieuCanTPFilletv2 p,MaThanhPhamFillet tp,MaLoaiCaFillet la where p.MaLoaiCa = la.Ma and p.MaLoaiCa = tp.MaCa and p.MaThanhPham = tp.Ma and p.Ngay=@ngay and p.MaXuong =@xuongId  group by p.MaLo, la.Ten,tp.Ten,tp.Ma,p.CaTra order by p.MaLo,tp.Ma";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopDinhMuc<T>(DateTime dateTime, string xuongId, DateTime fromTime, DateTime toTime)
    {
        var query = @"select
    p.*,
    nl.MaAo,
    nl.NhaCungCapName
from
    (
        SELECT
            p.MaLo,
            la.Ten As LoaiCaName,
            tp.Ma As MaThanhPham,
            tp.Ten As ThanhPhamName,
            p.CaTra,
            COUNT(p.STT) As SoRo,
            SUM(p.TrongLuongTra) as TrongLuongTra,
            SUM(p.TrongLuongNhan) as TrongLuongNhan,
            FLOOR(
                (SUM(p.TrongLuongNhan) / SUM(p.TrongLuongTra)) * 100
            ) / 100 as DinhMuc,
            dm.DinhMuc as DinhMucChuan
        from
            PhieuCanTPFilletv2 p,
            MaThanhPhamFillet tp,
            MaLoaiCaFillet la,
            (
                Select
                    *
                from
                    (
                        Select
                            d.*,
                            ROW_NUMBER() OVER (
                                PARTITION BY MaLo,
                                MaLoaiCa,
                                MaMau,
                                MaSize,
                                MaThanhPham,
                                CaTra
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            DinhMucFillet d
                        where
                            Ngay = @ngay
                            And MaXuong = @xuongId
                    ) dm
                Where
                    dm.[ROW NUMBER] = 1
            ) dm
        where
            p.MaLoaiCa = la.Ma
            and p.MaLoaiCa = tp.MaCa
            and p.MaThanhPham = tp.Ma
            and p.Ngay = @ngay
            and p.MaXuong = @xuongId
            and p.MaLo = dm.MaLo
            and p.MaMau = dm.MaMau
            and p.MaSize = dm.MaSize
            and p.MaThanhPham = dm.MaThanhPham
            and p.MaXuong = dm.MaXuong
            and p.CaTra = dm.CaTra
            and p.MaLoaiCa = dm.MaLoaiCa
            and p.MaXuong = @xuongId
            and p.Gio >= @fromTime
            and p.Gio <= @toTime
        group by
            p.MaLo,
            la.Ten,
            tp.Ten,
            tp.Ma,
            p.CaTra,
            dm.DinhMuc
    ) p
    LEFT JOIN (
        SELECT
            distinct ncc.Ten as NhaCungCapName,
            MaAo,
            MSL as MaLo
        FROM
            PhieuCanNguyenLieu p,
            NhaCungCapNguyenLieu ncc
        where
            p.MaXuongSanXuat = @xuongId
            and p.Ngay = @ngay
            and p.NhaCC = ncc.Ma
    ) nl ON p.MaLo = nl.MaLo
order by
    p.MaLo,
    p.MaThanhPham";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.Query<T>(
                    query,
                    new
                    {
                        ngay = dateTime.Date,
                        xuongId,
                        fromTime = fromTime.TimeOfDay,
                        toTime = toTime.TimeOfDay
                    })
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopDinhMucSanLuongTheoNhom<T>(DateTime fromDate, DateTime toDate,string xuongId)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var query = @"Select
    p.Nhom,
    p.TongSoNhanVienTP,
    p.DinhMuc,
    p.TLNhan,
    p.TLTra
from
    (
        Select
            p.Nhom,
            p.TongSoNhanVienTP,
           
            cast(
                case
                    when p.TLNhan = 0 then 0
                    else round(p.TLNhan / p.TLTra, 2)
                end as DECIMAL(18, 2)
            ) as DinhMuc,
            p.TLNhan,
            p.TLTra
        from
            (
                Select
                    nv.DeptName0 as Nhom,
                    Count(Distinct p.MaNhanVien) as TongSoNhanVienTP,
                    Sum(TrongLuongNhan) as TLNhan,
                    Sum(TrongLuongTra) as TLTra
                from
                    PhieuCanTPFilletv2 p , NhanVienDaiThanh nv
                Where
                    p.Ngay >= @fromDate
					and p.Ngay <= @toDate and p.MaXuong = @xuongId
                    And p.MaNhanVien = nv.MaNhanVien
                GROUP BY
                    nv.DeptName0
            ) p
    ) p";
        var items = connection.QueryAsync<T>(
                query,
                new { fromDate = fromDate.Date,toDate = toDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }
    

    public List<T> GetTongHopDinhMucSanLuongTheoThanhPham<T>(
        DateTime dateTime,
        string xuongId)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var query = @"Select
    p.MaThanhPham,
    p.ThanhPhamName,
    p.TongSoNhanVienTP,
    p.DinhMuc,
    p.TLNhan,
    p.TLTra
from
    (
        Select
            p.MaThanhPham,
            p.TongSoNhanVienTP,
            tp.Ten as ThanhPhamName,
            cast(
                case
                    when p.TLNhan = 0 then 0
                    else round(p.TLNhan / p.TLTra, 2)
                end as DECIMAL(18, 2)
            ) as DinhMuc,
            p.TLNhan,
            p.TLTra
        from
            (
                Select
                    MaThanhPham,
                    Count(Distinct MaNhanVien) as TongSoNhanVienTP,
                    Sum(TrongLuongNhan) as TLNhan,
                    Sum(TrongLuongTra) as TLTra
                from
                    PhieuCanTPFilletv2
                Where
                    Ngay = @ngay and MaXuong = @xuongId
                GROUP BY
                    MaThanhPham
            ) p
            LEFT JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
    ) p";
        var items = connection.QueryAsync<T>(
                query,
                new { ngay = dateTime.Date, xuongId })
            .Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopGioLamViec<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var query = @"Select
    p.Ngay,
    n.MaHoSo,
    n.Name as NhanVienName,
    Cast(
        convert(
            varchar(19),
            cast(p.Ngay as datetime) + Cast(Min(p.Gio) as datetime),
            120
        ) as datetime
    ) as GioBatDau,
    Cast(
        convert(
            varchar(19),
            cast(p.Ngay as datetime) + Cast(Max(ptp.Gio) as datetime),
            120
        ) as datetime
    ) as GioKetThuc,
    cast(
        DATEDIFF(second, Min(p.Gio), Max(ptp.Gio)) / 3600.0 as decimal(18, 3)
    ) as ThoiGianLamViec
from
    PhieuCanBTPFilletv2 p,
    PhieuCanTPFilletv2 ptp,
    NhanVienDaiThanh n
where
    p.Ngay <= @toDate
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and ptp.MaNhanVien = n.MaNhanVien
    and ptp.Ngay = p.Ngay
GROUP BY
    n.MaHoSo,
    n.Name,
    p.Ngay
order by
    p.Ngay,
    n.MaHoSo";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(
                query,
                new { ngay = toDate.Date, fromDate = fromDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopLo<T>(DateTime fromDate, DateTime toDate, string xuongId, bool isCaTraChuyenDoi = false,
        bool isDinhMucBinhThuong = true, bool isFloor = true)
    {
        var query = @"
Select
    p.Ngay,
	p.MaLo,
    p.MaSize,
    s.Ten as SizeName,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    Sum(p.TrongLuongNhan*p.LoaiSanLuong) as TrongLuongNhan,
    Sum(p.TrongLuongTra*p.LoaiSanLuong) as TrongLuongTra,
    case
        when @isDinhMucBinhThuong = 1 then cast (
            case
                when sum(p.TrongLuongTra) = 0 then 0
                else (
                    case
                        when @isFloor = 1 then (
                            FLOOR(
                                (Sum(p.TrongLuongNhan) / sum(p.TrongLuongTra)) * 1000
                            ) / 1000
                        )
                        else Sum(p.TrongLuongNhan) / sum(p.TrongLuongTra)
                    end
                )
            end as decimal(18, 3)
        )
        ELSE cast (
            case
                when sum(p.TrongLuongNhan) = 0 then 0
                else sum(p.TrongLuongTra) / sum(p.TrongLuongNhan)
            end as decimal(18, 4)
        )
    end as DinhMuc,
    cast( DinhMuc.DinhMuc as decimal(18,2)) as DinhMucChuan,
    Count(*) as SoRo
from
    (
        Select
            p.*,
            n.LoaiSanLuong
        from
            PhieuCanTPFilletv2 p,
            NhanVienDaiThanh n
        where
            p.Ngay <= @toDate
            and p.Ngay >= @fromDate
            and MaXuong = @xuongId
            and p.manhanvien = n.manhanvien
    ) p
    LEFT JOIN (
        Select
            tp1.MaLo,
            tp1.MaSize,
            CASE
                WHEN @isCaTraChuyenDoi = 1 THEN 'B'
                ELSE tp1.MaThanhPham
            END as MaThanhPham,
            CASE
                WHEN tp2.DinhMuc is Null THEN tp1.DinhMuc
                ELSE tp2.DinhMuc
            END AS DinhMuc,
            tp1.Ngay,
            tp1.MaXuong
        from
            (
                Select
                    distinct 
					p.MaLo,
                    p.MaSize,
                    p.MaThanhPham,
                    CASE
                        WHEN p.CaTra = 1 THEN 1.37
                        ELSE tp.DinhMuc
                    END AS DinhMuc,
                    p.Ngay,
                    p.MaXuong
                from
                    PhieuCanTPFilletv2 p,
                    MaThanhPhamFillet tp
                where
                    p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and MaXuong = @xuongId
                    and p.MaThanhPham = tp.Ma and tp.IsNguyenCon = 0
            ) tp1
            LEFT JOIN (
                Select
                    MaLo,
                    MaSize,
                    MaThanhPham,
                    CaTra,
                    DinhMuc,
                    Ngay,
                    MaXuong
                from
                    (
                        Select
                            d.*,
                            ROW_NUMBER() OVER (
                                PARTITION BY MaLo,
                                MaSize,
                                MaThanhPham,
                                Ngay
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            DinhMucFillet d
                        where
                            d.Ngay <= @toDate
                            and d.Ngay >= @fromDate
                            And MaXuong = @xuongId
                    ) dm
                Where
                    dm.[ROW NUMBER] = 1
            ) tp2 on tp1.MaLo = tp2.MaLo
            
            and tp1.MaSize = tp2.MaSize
            
            and tp1.MaThanhPham = tp2.MaThanhPham
            
            and tp1.Ngay = tp2.Ngay
            and tp1.MaXuong = tp2.MaXuong
    ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
    and p.MaLo = DinhMuc.MaLo
    and p.MaSize = DinhMuc.MaSize
    and p.MaThanhPham = DinhMuc.MaThanhPham
    and p.Ngay = DinhMuc.Ngay
    LEFT JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
    LEFT Join MaSizeFillet s on p.MaSize = s.Ma
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
where
    p.STT > 0
    and p.Ngay <= @toDate
    and p.Ngay >= @fromDate and tp.IsNguyenCon = 0
group by
    p.MaLoaiCa,
    p.MaSize,
    p.MaThanhPham,
    p.MaMau,
    p.CaTra,
    DinhMuc.DinhMuc,
    p.Ngay, 
    tp.Ten,
    s.Ten,
    p.LoaiSanLuong,
	p.MaLo
";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(query,
                new
                {
                    fromDate = fromDate.Date, toDate = toDate.Date, xuongId, isCaTraChuyenDoi, isDinhMucBinhThuong,
                    isFloor
                }).Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopNhanh<T>(DateTime dateTime, string xuongId, string maHoSo)
    {
        var query = @"Select
    p.MaNhanVien,
    p.MaLo,
    la.Ten as LoaiCaName,
    tp.Ten as ThanhPhamName,
    p.CaTra,
    COUNT(*) as SoRo,
    Sum(p.TrongLuongTra) as TrongLuongTra,
    SUM(p.TrongLuongNhan) as TrongLuongNhan,
    Sum(p.TrongLuongNhan) / SUM(p.TrongLuongTra) as DinhMuc
from
    PhieuCanTPFilletv2 p,
    NhanVienDaiThanh n,
    MaLoaiCaFillet la,
    MaThanhPhamFillet tp
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and n.MaHoSo = @maHoSo
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
group by
    p.MaNhanVien,
    p.MaLo,
    la.Ten,
    tp.Ten,
    p.CaTra
    
order by
    p.MaLo,
    tp.Ten";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { ngay = dateTime.Date, xuongId, maHoSo })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopNhanVien<T>(DateTime dateTime, string xuongId)
    {
        //                var query = @"Select
        //    tp.MaNhanVien,
        //    n.MaHoSo,
        //    n.Name as NhanVienName,
        //    tp.MaLo,
        //    la.Ten as LoaiCaName,
        //    _tp.Ten as ThanhPhamName,
        //    s.Ten as SizeName,
        //    ma.Ten as MauName,
        //    tp.TrongLuong as TrongLuongTP,
        //    tp.SoRo as SoRoTP,
        //   isnull(  btp.TrongLuong,0) as TrongLuongBTP,
        //   isnull( btp.SoRo ,0) as SoRoBTP,
        //    isnull( cast (
        //        case
        //            when tp.TrongLuong = 0 then 0
        //            else btp.TrongLuong / tp.TrongLuong
        //        end as decimal(18, 3)
        //    ),0) as DinhMuc,
        //    cast(0 as decimal(18, 2)) as DonGia,
        //    cast(0 as decimal(18, 2)) as ThanhTien
        //from
        //    (
        //        Select
        //            p.MaNhanVien,
        //            p.MaLo,
        //            p.MaLoaiCa,
        //            p.MaSize,
        //            p.MaMau,
        //            p.MaThanhPham,
        //            sum(p.TrongLuongTra) as TrongLuong,
        //            COUNT(*) as SoRo
        //        from
        //            PhieuCanTPFilletv2 p
        //        Where
        //            p.Ngay = @ngay
        //            and p.MaXuong = @xuongId
        //        GROUP BY
        //            p.MaNhanVien,
        //            p.MaLo,
        //            p.MaLoaiCa,
        //            p.MaSize,
        //            p.MaMau,
        //            p.MaThanhPham
        //    ) tp
        //    LEFT JOIN (
        //        Select
        //            p.MaNhanVien,
        //            p.MaLo,
        //            p.MaLoaiCa,
        //            p.MaSize,
        //            p.MaMau,
        //            p.MaThanhPham,
        //            sum(p.TrongLuong) as TrongLuong,
        //            COUNT(*) as SoRo
        //        from
        //            PhieuCanBTPFilletv2 p
        //        Where
        //            p.Ngay = @ngay
        //            and p.MaXuong = @xuongId
        //        GROUP BY
        //            p.MaNhanVien,
        //            p.MaLo,
        //            p.MaLoaiCa,
        //            p.MaSize,
        //            p.MaMau,
        //            p.MaThanhPham
        //    ) btp ON tp.MaLo = btp.MaLo
        //    and tp.MaLoaiCa = btp.MaLoaiCa
        //    and tp.MaMau = btp.MaMau
        //    and tp.MaNhanVien = btp.MaNhanVien
        //    and tp.MaSize = btp.MaSize
        //    and tp.MaThanhPham = btp.MaThanhPham
        //    LEFT JOIN MaLoaiCaFillet la on tp.MaLoaiCa = la.Ma
        //    LEFT JOIN MaMauFillet ma on tp.MaMau = ma.Ma
        //    LEFT JOIN MaSizeFillet s on tp.MaSize = s.Ma
        //    LEFT JOIN MaThanhPhamFillet _tp on tp.MaThanhPham = _tp.Ma
        //    LEFT JOIN NhanVienDaiThanh n on tp.MaNhanVien = n.MaNhanVien
        //order by
        //    tp.MaLo,
        //    _tp.Ten";

        var query = @"Select
    p.*,
    dgxh.TenXepHang,
    dgxh.DinhMuc as DinhMucChuan,
    ISNULL(dgxh.DonGia, 0) as DonGia,
    cast(
        (
            (
                ISNULL(dgxh.DonGia, 0) * dgxh.HeSo - ISNULL(dgxh.DonGia, 0) * dgxh.HeSo *(
                    case
                        when dgxh.IsUsedHeSoRot = 1 then dgxh.HeSoRot
                        else 0
                    end
                )
            ) * p.TrongLuongTP
        ) as decimal(18, 2)
    ) as ThanhTien
from
    (
        Select
            tp.MaNhanVien,
            n.MaHoSo,
            n.Name as NhanVienName,
            tp.MaLo,
            la.Ten as LoaiCaName,
            _tp.Ten as ThanhPhamName,
            _tp.BravoId as MaSanPham,
            s.Ten as SizeName,
            ma.Ten as MauName,
            tp.TrongLuong as TrongLuongTP,
            tp.SoRo as SoRoTP,
            isnull(btp.TrongLuong, 0) as TrongLuongBTP,
            isnull(btp.TrongLuong*_tp.DinhMucHaoHut, 0) as TLTruocHaoHutgBTP,
            isnull(btp.SoRo, 0) as SoRoBTP,
            isnull(
                cast (
                    case
                        when tp.TrongLuong = 0 then 0
                        else btp.TrongLuong*_tp.DinhMucHaoHut / tp.TrongLuong
                    end as decimal(18, 3)
                ),
                0
            ) as DinhMuc
        from
            (
                Select
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham,
                    sum(p.TrongLuongTra) as TrongLuong,
                    COUNT(*) as SoRo
                from
                    PhieuCanTPFilletv2 p
                Where
                    p.Ngay = @ngay
                    and p.MaXuong = @xuongId  and p.STT > 0
                GROUP BY
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham
            ) tp
            LEFT JOIN (
                Select
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo
                from
                    PhieuCanBTPFilletv2 p
                Where
                    p.Ngay = @ngay
                    and p.MaXuong = @xuongId  and p.STT > 0
                GROUP BY
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham
            ) btp ON tp.MaLo = btp.MaLo
            and tp.MaLoaiCa = btp.MaLoaiCa
            and tp.MaMau = btp.MaMau
            and tp.MaNhanVien = btp.MaNhanVien
            and tp.MaSize = btp.MaSize
            and tp.MaThanhPham = btp.MaThanhPham
            LEFT JOIN MaLoaiCaFillet la on tp.MaLoaiCa = la.Ma
            LEFT JOIN MaMauFillet ma on tp.MaMau = ma.Ma
            LEFT JOIN MaSizeFillet s on tp.MaSize = s.Ma
            LEFT JOIN MaThanhPhamFillet _tp on tp.MaThanhPham = _tp.Ma
            LEFT JOIN NhanVienDaiThanh n on tp.MaNhanVien = n.MaNhanVien
    ) p
    LEFT JOIN (
        select
            dmxh.*,
            dg.DonGia,
            dg.DonGiaGiaCong,
            dg.HeSo,
            dg.HeSoRot,
            dg.IsUsedHeSoRot
        from
            (
                Select
                    dmxh.*,
                    xh.Ten as TenXepHang
                from
                    DinhMucXepHang dmxh,
                    MaXepHang xh
                where
                    [Year] = YEAR(@ngay)
                    and dmxh.MaXepHang = xh.Ma
            ) dmxh
            LEFT JOIN(
                Select
                    *
                from
                    (
                        Select
                            *,
                            ROW_NUMBER() over (
                                partition by MaSanPham,
                                DanhGia,
                                DinhMucDown,
                                MaLoaiDonGia,
                                MaSizeDinhHinh,
                                MaXepHang
                                order by
                                    Ngay DESC,
                                    Gio Desc
                            ) as rowId
                        from
                            DG_DonGia
                        where
                            Ngay <= @ngay
                            and MaLoaiDonGia = 'XH' and LoaiCan =''
                    ) p
                where
                    p.rowId = 1
            ) dg on dmxh.MaSanPham = dg.MaSanPham
            and dmxh.MaXepHang = dg.MaXepHang
    ) dgxh on p.MaSanPham = dgxh.MaSanPham
    and p.MaLo = dgxh.MaLo
    and p.DinhMuc >= dgxh.DinhMucDown
    and p.DinhMuc <= dgxh.DinhMucUp
order by
    p.MaLo,
    p.ThanhPhamName";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopNhanVien<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var query = @"Select
    p.*,
    dgxh.TenXepHang,
    ISNULL(dgxh.DinhMuc,0) as DinhMucChuan,
    ISNULL(dgxh.DonGia, 0) as DonGia,
    cast(
        (
            (
                ISNULL(dgxh.DonGia, 0) * ISNULL(dgxh.HeSo,0) - ISNULL(dgxh.DonGia, 0) * ISNULL(dgxh.HeSo,0) *(
                    case
                        when dgxh.IsUsedHeSoRot = 1 then dgxh.HeSoRot
                        else 0
                    end
                )
            ) * p.TrongLuongTP
        ) as decimal(18, 2)
    ) as ThanhTien
from
    (
        Select
            tp.MaNhanVien,
            n.MaHoSo,
            n.Name as NhanVienName,
            tp.MaLo,
            la.Ten as LoaiCaName,
            _tp.Ten as ThanhPhamName,
            _tp.BravoId as MaSanPham,
            s.Ten as SizeName,
            ma.Ten as MauName,
            tp.TrongLuong as TrongLuongTP,
            tp.SoRo as SoRoTP,
            isnull(btp.TrongLuong, 0) as TrongLuongBTP,
            isnull(btp.TrongLuong * _tp.DinhMucHaoHut, 0) as TLTruocHaoHutgBTP,
            isnull(btp.SoRo, 0) as SoRoBTP,
            isnull(
                cast (
                    case
                        when tp.TrongLuong = 0 then 0
                        else btp.TrongLuong * _tp.DinhMucHaoHut / tp.TrongLuong
                    end as decimal(18, 3)
                ),
                0
            ) as DinhMuc
        from
            (
                Select
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham,
                    sum(p.TrongLuongTra) as TrongLuong,
                    COUNT(*) as SoRo
                from
                    PhieuCanTPFilletv2 p
                Where
                    p.Ngay <= @ngay
                    and p.Ngay >= @fromDate
                    and p.MaXuong = @xuongId
                    and p.STT > 0
                GROUP BY
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham
            ) tp
            LEFT JOIN (
                Select
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo
                from
                    PhieuCanBTPFilletv2 p
                Where
                    p.Ngay <= @ngay
                    and p.Ngay >= @fromDate
                    and p.MaXuong = @xuongId
                    and p.STT > 0
                GROUP BY
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham
            ) btp ON tp.MaLo = btp.MaLo
            and tp.MaLoaiCa = btp.MaLoaiCa
            and tp.MaMau = btp.MaMau
            and tp.MaNhanVien = btp.MaNhanVien
            and tp.MaSize = btp.MaSize
            and tp.MaThanhPham = btp.MaThanhPham
            LEFT JOIN MaLoaiCaFillet la on tp.MaLoaiCa = la.Ma
            LEFT JOIN MaMauFillet ma on tp.MaMau = ma.Ma
            LEFT JOIN MaSizeFillet s on tp.MaSize = s.Ma
            LEFT JOIN MaThanhPhamFillet _tp on tp.MaThanhPham = _tp.Ma
            LEFT JOIN NhanVienDaiThanh n on tp.MaNhanVien = n.MaNhanVien
    ) p
    LEFT JOIN (
        select
            dmxh.*,
            dg.DonGia,
            dg.DonGiaGiaCong,
            dg.HeSo,
            dg.HeSoRot,
            dg.IsUsedHeSoRot
        from
            (
                Select
                    dmxh.*,
                    xh.Ten as TenXepHang
                from
                    DinhMucXepHang dmxh,
                    MaXepHang xh
                where
                    [Year] = YEAR(@ngay)
                    and dmxh.MaXepHang = xh.Ma
            ) dmxh
            LEFT JOIN(
                Select
                    *
                from
                    (
                        Select
                            *,
                            ROW_NUMBER() over (
                                partition by MaSanPham,
                                DanhGia,
                                DinhMucDown,
                                MaLoaiDonGia,
                                MaSizeDinhHinh,
                                MaXepHang
                                order by
                                    Ngay DESC,
                                    Gio Desc
                            ) as rowId
                        from
                            DG_DonGia
                        where
                            Ngay <= @ngay
                            and MaLoaiDonGia = 'XH'  and LoaiCan =''
                    ) p
                where
                    p.rowId = 1
            ) dg on dmxh.MaSanPham = dg.MaSanPham
            and dmxh.MaXepHang = dg.MaXepHang
    ) dgxh on p.MaSanPham = dgxh.MaSanPham
    and p.MaLo = dgxh.MaLo
    and p.DinhMuc >= dgxh.DinhMucDown
    and p.DinhMuc <= dgxh.DinhMucUp
order by
    p.MaLo,
    p.ThanhPhamName";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(query, new { fromDate, ngay = toDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopNhanVien<T>(DateTime dateTime, string xuongId, TimeSpan time)
    {
        var query = @"Select
    p.*,
    dgxh.TenXepHang,
    dgxh.DinhMuc as DinhMucChuan,
    ISNULL(dgxh.DonGia, 0) as DonGia,
    cast(
        (
            (
                ISNULL(dgxh.DonGia, 0) * dgxh.HeSo - ISNULL(dgxh.DonGia, 0) * dgxh.HeSo *(
                    case
                        when dgxh.IsUsedHeSoRot = 1 then dgxh.HeSoRot
                        else 0
                    end
                )
            ) * p.TrongLuongTP
        ) as decimal(18, 2)
    ) as ThanhTien
from
    (
        Select
            tp.MaNhanVien,
            n.MaHoSo,
            n.Name as NhanVienName,
            tp.MaLo,
            la.Ten as LoaiCaName,
            _tp.Ten as ThanhPhamName,
            _tp.BravoId as MaSanPham,
            s.Ten as SizeName,
            ma.Ten as MauName,
            tp.TrongLuong as TrongLuongTP,
            tp.SoRo as SoRoTP,
            isnull(btp.TrongLuong, 0) as TrongLuongBTP,
            isnull(btp.TrongLuong * _tp.DinhMucHaoHut, 0) as TLTruocHaoHutBTP,
            isnull(btp.SoRo, 0) as SoRoBTP,
            isnull(
                cast (
                    case
                        when tp.TrongLuong = 0 then 0
                        else btp.TrongLuong * _tp.DinhMucHaoHut / tp.TrongLuong
                    end as decimal(18, 3)
                ),
                0
            ) as DinhMuc
        from
            (
                Select
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham,
                    sum(p.TrongLuongTra) as TrongLuong,
                    COUNT(*) as SoRo
                from
                    PhieuCanTPFilletv2 p
                Where
                    p.Ngay = @ngay
                    and p.MaXuong = @xuongId
                    and p.Gio <= @time  and p.STT > 0
                GROUP BY
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham
            ) tp
            LEFT JOIN (
                Select
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo
                from
                    PhieuCanBTPFilletv2 p
                Where
                    p.Ngay = @ngay
                    and p.MaXuong = @xuongId
                    and p.Gio <= @time and p.STT > 0
                GROUP BY
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham
            ) btp ON tp.MaLo = btp.MaLo
            and tp.MaLoaiCa = btp.MaLoaiCa
            and tp.MaMau = btp.MaMau
            and tp.MaNhanVien = btp.MaNhanVien
            and tp.MaSize = btp.MaSize
            and tp.MaThanhPham = btp.MaThanhPham
            LEFT JOIN MaLoaiCaFillet la on tp.MaLoaiCa = la.Ma
            LEFT JOIN MaMauFillet ma on tp.MaMau = ma.Ma
            LEFT JOIN MaSizeFillet s on tp.MaSize = s.Ma
            LEFT JOIN MaThanhPhamFillet _tp on tp.MaThanhPham = _tp.Ma
            LEFT JOIN NhanVienDaiThanh n on tp.MaNhanVien = n.MaNhanVien
    ) p
    LEFT JOIN (
        select
            dmxh.*,
            dg.DonGia,
            dg.DonGiaGiaCong,
            dg.HeSo,
            dg.HeSoRot,
            dg.IsUsedHeSoRot
        from
            (
                Select
                    dmxh.*,
                    xh.Ten as TenXepHang
                from
                    DinhMucXepHang dmxh,
                    MaXepHang xh
                where
                    [Year] = YEAR(@ngay)
                    and dmxh.MaXepHang = xh.Ma
            ) dmxh
            LEFT JOIN(
                Select
                    *
                from
                    (
                        Select
                            *,
                            ROW_NUMBER() over (
                                partition by MaSanPham,
                                DanhGia,
                                DinhMucDown,
                                MaLoaiDonGia,
                                MaSizeDinhHinh,
                                MaXepHang
                                order by
                                    Ngay DESC,
                                    Gio Desc
                            ) as rowId
                        from
                            DG_DonGia
                        where
                            Ngay <= @ngay
                            and MaLoaiDonGia = 'XH'  and LoaiCan =''
                    ) p
                where
                    p.rowId = 1
            ) dg on dmxh.MaSanPham = dg.MaSanPham
            and dmxh.MaXepHang = dg.MaXepHang
    ) dgxh on p.MaSanPham = dgxh.MaSanPham
    and p.MaLo = dgxh.MaLo
    and p.DinhMuc >= dgxh.DinhMucDown
    and p.DinhMuc <= dgxh.DinhMucUp
order by
    p.MaLo,
    p.ThanhPhamName";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId, time }).Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopNhanVien<T>(DateTime fromDate, DateTime toDate, string xuongId, TimeSpan time)
    {
        var query = @"Select
    p.*,
    dgxh.TenXepHang,
    ISNULL(dgxh.DinhMuc,0) as DinhMucChuan,
    ISNULL(dgxh.DonGia, 0) as DonGia,
    cast(
        (
            (
                ISNULL(dgxh.DonGia, 0) * ISNULL(dgxh.HeSo,0) - ISNULL(dgxh.DonGia, 0) * ISNULL(dgxh.HeSo,0) *(
                    case
                        when dgxh.IsUsedHeSoRot = 1 then dgxh.HeSoRot
                        else 0
                    end
                )
            ) * p.TrongLuongTP
        ) as decimal(18, 2)
    ) as ThanhTien
from
    (
        Select
            tp.MaNhanVien,
            n.MaHoSo,
            n.Name as NhanVienName,
            tp.MaLo,
            la.Ten as LoaiCaName,
            _tp.Ten as ThanhPhamName,
            _tp.BravoId as MaSanPham,
            s.Ten as SizeName,
            ma.Ten as MauName,
            tp.TrongLuong as TrongLuongTP,
            tp.SoRo as SoRoTP,
            isnull(btp.TrongLuong, 0) as TrongLuongBTP,
            isnull(btp.TrongLuong * _tp.DinhMucHaoHut, 0) as TLTruocHaoHutBTP,
            isnull(btp.SoRo, 0) as SoRoBTP,
            isnull(
                cast (
                    case
                        when tp.TrongLuong = 0 then 0
                        else btp.TrongLuong * _tp.DinhMucHaoHut / tp.TrongLuong
                    end as decimal(18, 3)
                ),
                0
            ) as DinhMuc
        from
            (
                Select
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham,
                    sum(p.TrongLuongTra) as TrongLuong,
                    COUNT(*) as SoRo
                from
                    PhieuCanTPFilletv2 p
                Where
                    p.Ngay <= @ngay
                    and p.Ngay >= @fromDate
                    and p.MaXuong = @xuongId
                    and p.Gio <= @time
                    and p.STT > 0
                GROUP BY
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham
            ) tp
            LEFT JOIN (
                Select
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo
                from
                    PhieuCanBTPFilletv2 p
                Where
                    p.Ngay <= @ngay
                    and p.Ngay >= @fromDate
                    and p.MaXuong = @xuongId
                    and p.Gio <= @time
                    and p.STT > 0
                GROUP BY
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham
            ) btp ON tp.MaLo = btp.MaLo
            and tp.MaLoaiCa = btp.MaLoaiCa
            and tp.MaMau = btp.MaMau
            and tp.MaNhanVien = btp.MaNhanVien
            and tp.MaSize = btp.MaSize
            and tp.MaThanhPham = btp.MaThanhPham
            LEFT JOIN MaLoaiCaFillet la on tp.MaLoaiCa = la.Ma
            LEFT JOIN MaMauFillet ma on tp.MaMau = ma.Ma
            LEFT JOIN MaSizeFillet s on tp.MaSize = s.Ma
            LEFT JOIN MaThanhPhamFillet _tp on tp.MaThanhPham = _tp.Ma
            LEFT JOIN NhanVienDaiThanh n on tp.MaNhanVien = n.MaNhanVien
    ) p
    LEFT JOIN (
        select
            dmxh.*,
            dg.DonGia,
            dg.DonGiaGiaCong,
            dg.HeSo,
            dg.HeSoRot,
            dg.IsUsedHeSoRot
        from
            (
                Select
                    dmxh.*,
                    xh.Ten as TenXepHang
                from
                    DinhMucXepHang dmxh,
                    MaXepHang xh
                where
                    [Year] = YEAR(@ngay)
                    and dmxh.MaXepHang = xh.Ma
            ) dmxh
            LEFT JOIN(
                Select
                    *
                from
                    (
                        Select
                            *,
                            ROW_NUMBER() over (
                                partition by MaSanPham,
                                DanhGia,
                                DinhMucDown,
                                MaLoaiDonGia,
                                MaSizeDinhHinh,
                                MaXepHang
                                order by
                                    Ngay DESC,
                                    Gio Desc
                            ) as rowId
                        from
                            DG_DonGia
                        where
                            Ngay <= @ngay
                            and MaLoaiDonGia = 'XH'  and LoaiCan =''
                    ) p
                where
                    p.rowId = 1
            ) dg on dmxh.MaSanPham = dg.MaSanPham
            and dmxh.MaXepHang = dg.MaXepHang
    ) dgxh on p.MaSanPham = dgxh.MaSanPham
    and p.MaLo = dgxh.MaLo
    and p.DinhMuc >= dgxh.DinhMucDown
    and p.DinhMuc <= dgxh.DinhMucUp
order by
    p.MaLo,
    p.ThanhPhamName";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(query, new { fromDate, ngay = toDate.Date, xuongId, time }).Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopNhanVienPhucVu<T>(DateTime dateTime, string xuongId)
    {
        var query = @"Select
    p.MaNhanVienPhucVu as [Mã Nhân Viên],
    n.MaHoSo as [Mã Số],
    n.Name as [Tên Nhân Viên],
    p.MaLo as [Lô],
    la.Ten as [Loại cá],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    SUM(p.TrongLuongTra) as [Trọng Lượng],
    COUNT(*) as [Số Rổ]
from
    PhieuCanTPFilletv2 p,
    NhanVienDaiThanh n,
    MaLoaiCaFillet la,
    MaThanhPhamFillet tp,
    MaSizeFillet s
where
    p.Ngay = @ngay
    and p.MaNhanVienPhucVu = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
GROUP BY
    p.MaNhanVienPhucVu,
    n.MaHoSo,
    n.Name,
    p.MaLo,
    la.Ten,
    tp.Ten,
    s.Ten
order by
    p.MaLo,
    tp.Ten,
    s.Ten,
    n.MaHoSo";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopNhanVienPhucVu<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
//            var query = @"Select
//    p.Ngay,
//    p.MaNhanVienPhucVu,
//    n.MaHoSo,
//    n.Name as NhanVienName,
//    p.MaLo,
//    la.Ten as LoaiCaName,
//    tp.Ten as ThanhPhamName,
//    s.Ten as SizeName,
//    SUM(p.TrongLuongTra) as TrongLuong,
//    COUNT(*) as SoRo
//from
//    PhieuCanTPFilletv2 p,
//    NhanVienDaiThanh n,
//    MaLoaiCaFillet la,
//    MaThanhPhamFillet tp,
//    MaSizeFillet s
//where
//    p.Ngay <= @ngay
//    and p.Ngay >= @fromDate
//    and p.MaNhanVienPhucVu = n.MaNhanVien
//    and p.MaLoaiCa = la.Ma
//    and p.MaThanhPham = tp.Ma
//    and p.MaSize = s.Ma
//GROUP BY
//    p.Ngay,
//    p.MaNhanVienPhucVu,
//    n.MaHoSo,
//    n.Name,
//    p.MaLo,
//    la.Ten,
//    tp.Ten,
//    s.Ten
//order by
//    p.Ngay,
//    p.MaLo,
//    tp.Ten,
//    s.Ten,
//    n.MaHoSo";
        var query = @"Select
    p.Ngay,
    p.MaNhanVienPhucVu,
    n.MaHoSo,
    n.Name as NhanVienName,
    p.MaLo,
    la.Ten as LoaiCaName,
    tp.Ten as ThanhPhamName,
    s.Ten as SizeName,
    SUM(p.TrongLuongTra) as TrongLuong,
    COUNT(*) as SoRo,
	MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianVao,
	MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianRa,
	DATEDIFF(hour, MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay), MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay)) as TongThoiGian
from
    PhieuCanTPFilletv2 p,
    NhanVienDaiThanh n,
    MaLoaiCaFillet la,
    MaThanhPhamFillet tp,
    MaSizeFillet s,
	CheckInOut c
where
    p.Ngay <= @ngay
    and p.Ngay >= @fromDate
    and p.MaNhanVienPhucVu = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
	AND n.MaChamCong = c.MaChamCong AND c.ThoiGian = p.Ngay AND c.ThoiGian >= @fromDate AND c.ThoiGian <= @ngay 
GROUP BY
    p.Ngay,
    p.MaNhanVienPhucVu,
    n.MaHoSo,
    n.Name,
    p.MaLo,
    la.Ten,
    tp.Ten,
    s.Ten,
	n.MaChamCong,
	c.ThoiGian
order by
    p.Ngay,
    p.MaLo,
    tp.Ten,
    s.Ten,
    n.MaHoSo";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(
                query,
                new { ngay = toDate.Date, fromDate = fromDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopNhanVienPhucVusv2<T>(DateTime dateTime, string sanPhamId, string xuongId)
    {
        var query = @"
        SELECT
            n.MaNhanVien ,
            n.MaHoSo ,
            isnull( round(Sum(p.TrongLuongTra), 2) ,0)as SanLuongHuong,
            1 as TyLeHuong,
            0 as SanLuongTru,
            0 as SoGio,
            1 as TyLeHuong,
            0 as TyLeTru,
            @sanPhamId as MaThanhPham
        FROM
            PhieuCanTPFilletv2 p,
            NhanVienDaiThanh n
        Where
            p.Ngay = @ngay
            and p.TrongLuongTra > 0
            and p.MaNhanVienPhucVu = n.MaNhanVien and p.MaXuong = @xuongId
        Group By
            n.MaNhanVien,
            n.MaHoSo";
        var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.Query<T>(query, new { ngay = dateTime.Date, sanPhamId, xuongId }).ToList();
        return items;
    }

    public List<T> GetTongHopNhanViens<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId
    )
    {
        var query = @"Select
            p.Ngay,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as NhanVienName,
            n.DeptName0 as Nhom,
            p.MSL,
            p.MaLoaiCa,
            la.Ten as LoaiCaName,
            p.MaSize,
            s.Ten as SizeName,
            p.MaLoaiThanhPham,
            tp.Ten as ThanhPhamName,
            tp.BravoId as MaSanPham,
		
            p.MaMau,
            ma.Ten as MauName,
            n.LoaiSanLuong,
            Sum(p.TrongLuong) as TrongLuong,
            
            
            
            Count(*) as SoRo,
            p.MaXuongSanXuat
        from
            PhieuCanTPFillet p
            LEFT JOIN (
                Select
                    tp1.MSL,
                    tp1.MaLoaiCa,
                    tp1.MaSize,
                    tp1.MaMau,
                    tp1.MaLoaiThanhPham,
                    tp1.Ngay,
                    tp1.MaXuongSanXuat
                from
                    (
                        Select
                            distinct p.MSL,
                            p.MaLoaiCa,
                            p.MaSize,
                            p.MaMau,
                            p.MaLoaiThanhPham,
                          
                          
                            p.Ngay,
                            p.MaXuongSanXuat
                        from
                            PhieuCanTPFillet p,
                            MaThanhPhamFillet tp
                        where
                            p.Ngay <= @toDate
                            and p.Ngay >= @fromDate
                            and p.MaXuongSanXuat = @xuongId
                            and p.MaLoaiThanhPham = tp.Ma
                            and p.MaLoaiCa = tp.MaCa
                    ) tp1
                    LEFT JOIN (
                        Select
                            MaLo,
                            MaLoaiCa,
                            MaSize,
                            MaMau,
                            MaThanhPham,
                            CaTra,
                            DinhMuc,
                            Ngay,
                            MaXuong
                        from
                            (
                                Select
                                    d.*,
                                    ROW_NUMBER() OVER (
                                        PARTITION BY MaLo,
                                        MaLoaiCa,
                                        MaMau,
                                        MaSize,
                                        MaThanhPham,
                                        CaTra,
                                        Ngay
                                        ORDER BY
                                            Gio DESC
                                    ) AS [ROW NUMBER]
                                from
                                    DinhMucFillet d
                                where
                                    d.Ngay <= @toDate
                                    and d.Ngay >= @fromDate
                                    And MaXuong = @xuongId
                            ) dm
                        Where
                            dm.[ROW NUMBER] = 1
                    ) tp2 on tp1.MSL = tp2.MaLo
                    and tp1.MaLoaiCa = tp2.MaLoaiCa
                    and tp1.MaSize = tp2.MaSize
                    and tp1.MaMau = tp2.MaMau
                    and tp1.MaLoaiThanhPham = tp2.MaThanhPham
                   
                    and tp1.Ngay = tp2.Ngay
                    and tp1.MaXuongSanXuat = tp2.MaXuong
            ) DinhMuc on p.MaXuongSanXuat = DinhMuc.MaXuongSanXuat
            and p.MSL = DinhMuc.MSL
            and p.MaLoaiCa = DinhMuc.MaLoaiCa
            and p.MaSize = DinhMuc.MaSize
            and p.MaMau = DinhMuc.MaMau
            and p.MaLoaiThanhPham = DinhMuc.MaLoaiThanhPham
            
            and p.Ngay = DinhMuc.Ngay
            LEFT JOIN MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma  and p.MaLoaiCa = tp.MaCa
            LEFT Join MaSizeFillet s on p.MaSize = s.Ma
            LEFT Join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
            LEFT Join MaMauFillet ma on p.MaMau = ma.Ma
            LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
        where
            p.Ngay <= @toDate
            and p.Ngay >= @fromDate
            and p.MaXuongSanXuat = @xuongId
        group by
            p.MaNhanVien,
            p.MSL,
            p.MaLoaiCa,
            p.MaSize,
            p.MaLoaiThanhPham,
            p.MaMau,
           
            
            p.Ngay,
            
            n.MaHoSo,
            n.Name,
            la.Ten,
            tp.Ten,
            tp.BravoId,
		
            ma.Ten,
            s.Ten,
            n.DeptName0,
            n.LoaiSanLuong,
            p.MaXuongSanXuat
     ";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate, toDate, xuongId })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopNhanViens2<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId
    )
    {
        var query = @"
Select
    p.*,
    map.MaBravoFillet as MaLuong
from
    (
        Select
            p.Ngay,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as [NhanVienName],
            n.DeptName0 as Nhom,
            isnull(p.LoTheoLine, p.MSL) as MSL,
            la.ma as MaLoaiCa,
            la.Ten as LoaiCaName,
            isnull(p.MaThanhPhamBoTri, p.MaLoaiThanhPham) as [MaLoaiThanhPham],
            isnull(p.ThanhPhamName, tp.Ten) as [ThanhPhamName],
            ISNULL(p.MaSizeBoTri, p.MaSize) as [MaSize],
            ISNULL(p.SizeName, s.Ten) as [SizeName],
            p.TrongLuong as [TrongLuong],
            p.MaXuongSanXuat as [Xuong]
        from
            (
                Select
                    p.*,
                    case
                        when tp2.IsNotSetByTime = 1 then s2.Ma
                        else bt.MaSize
                    end as MaSizeBoTri,
                    case
                        when tp2.IsNotSetByTime = 1 then s2.Ten
                        else s.Ten
                    end as SizeName,
                    case
                        when tp2.IsNotSetByTime = 1 then NULL
                        else bt.MaThanhPham
                    end as MaThanhPhamBoTri,
                    case
                        when tp2.IsNotSetByTime = 1 then NULL
                        else tp.Ten
                    end as ThanhPhamName
                from
                    (
                        select
                            p.*,
                            ll.MaLo as LoTheoLine
                        from
                            (
                                Select
                                    p.*,
                                    nl.MaLine,
                                    ln.Ten as LineName,
                                    nl.MaViTri,
                                    vt.Ten as ViTriName,
                                    xn.CodeId
                                from
                                    (select * from PhieuCanTPFillet where TrongLuong>0) p
                                    left join NhanVienTheoLine nl on p.Ngay = nl.Ngay
                                    and p.MaNhanVien = nl.MaNhanVien
                                    and nl.Gio = (
                                        Select
                                            MAX(nlc.Gio)
                                        from
                                            NhanVienTheoLine nlc
                                        where
                                            nlc.Ngay = p.Ngay
                                            and nlc.MaNhanVien = p.MaNhanVien
                                            and nlc.Gio <= p.ThoiGianCan
                                    )
                                    left join XiNghiep xn on xn.Ma = p.MaXuongSanXuat
                                    left join LineFilletv2 ln on nl.MaLine = ln.Ma
                                    left join ViTriFillet vt on nl.MaViTri = vt.Ma
                                where
                                    p.Ngay <= @toDate
                                    and p.Ngay >= @fromDate
                                    and p.MaXuongSanXuat = @xuongId
                            ) p
                            left JOIN LoTheoLine ll ON p.MaLine = ll.MaLine
                            and p.Ngay = ll.Ngay
                            and p.CodeId = ll.CodeId
                            and ll.Gio = (
                                Select
                                    MAX(llc.Gio)
                                from
                                    LoTheoLine llc
                                where
                                    llc.MaLine = p.MaLine
                                    and llc.CodeId = p.CodeId
                                    and llc.Ngay = p.Ngay
                                    and llc.Gio <= p.ThoiGianCan
                            )
                    ) p
                    left JOIN BoTriLoSizeThanhPham bt ON p.LoTheoLine = bt.MaLo
                    and p.CodeId = bt.CodeId
                    and p.MaViTri = bt.MaViTri
                    and bt.Ngay = p.Ngay
                    and bt.Gio = (
                        Select
                            MAX(btc.Gio)
                        from
                            BoTriLoSizeThanhPham btc
                        where
                            btc.MaLo = p.LoTheoLine
                            and btc.MaViTri = p.MaViTri
                            and btc.CodeId = p.CodeId
                            and btc.Ngay = p.Ngay
                            and btc.Gio <= p.ThoiGianCan
                    )
                    left join MaThanhPhamFillet tp on bt.MaThanhPham = tp.Ma and tp.MaCa = p.MaLoaiCa
                    left join MaThanhPhamFillet tp2 on p.MaLoaiThanhPham = tp2.Ma and tp2.MaCa = p.MaLoaiCa
                    LEFT join MaSizeFillet s on bt.MaSize = s.Ma
                    LEFT join MaSizeFillet s2 on bt.MaSizePhu = s2.Ma
            ) p
            left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            left join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
            left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma  and p.MaLoaiCa = tp.MaCa
            LEFT join MaSizeFillet s on p.MaSize = s.Ma
    ) p
    LEFT JOIN MapThanhPhamFillet map on p.MaLoaiThanhPham = map.MaTPFillet
    and p.MaSize = map.MaSize
     ";
//                var query = @"Select
//    p.Ngay,
//    p.MaNhanVien ,
//    n.MaHoSo ,
//    n.Name as [NhanVienName],
//    n.DeptName0 as Nhom,
//    isnull(p.LoTheoLine, p.MSL) as MSL,
//    la.ma as MaLoaiCa,
//    la.Ten as LoaiCaName,
//    isnull(p.MaThanhPhamBoTri, p.MaLoaiThanhPham) as [MaLoaiThanhPham],
//    isnull(p.ThanhPhamName, tp.Ten) as [ThanhPhamName],
//    ISNULL(p.MaSizeBoTri, p.MaSize) as [MaSize],
//    ISNULL(p.SizeName, s.Ten) as [SizeName],
//    p.TrongLuong as [TrongLuong],
//    p.MaXuongSanXuat as [Xuong] 
//from
//    (
//        Select
//            p.*,
//            case
//                when tp2.IsNotSetByTime = 1 then s2.Ma
//                else bt.MaSize
//            end as MaSizeBoTri,
//            case
//                when tp2.IsNotSetByTime = 1 then s2.Ten
//                else s.Ten
//            end as SizeName,
//            case
//                when tp2.IsNotSetByTime = 1 then NULL
//                else bt.MaThanhPham
//            end as MaThanhPhamBoTri,
//            case
//                when tp2.IsNotSetByTime = 1 then NULL
//                else tp.Ten
//            end as ThanhPhamName
//        from
//            (
//                select
//                    p.*,
//                    ll.MaLo as LoTheoLine
//                from
//                    (
//                        Select
//                            p.*,
//                            nl.MaLine,
//                            ln.Ten as LineName,
//                            nl.MaViTri,
//                            vt.Ten as ViTriName,
//                            xn.CodeId
//                        from
//                            PhieuCanTPFillet p
//                            left join NhanVienTheoLine nl on p.Ngay = nl.Ngay
//                            and p.MaNhanVien = nl.MaNhanVien
//                            and nl.Gio = (
//                                Select
//                                    MAX(nlc.Gio)
//                                from
//                                    NhanVienTheoLine nlc
//                                where
//                                    nlc.Ngay = p.Ngay
//                                    and nlc.MaNhanVien = p.MaNhanVien
//                                    and nlc.Gio <= p.ThoiGianCan
//                            )
//                            left join XiNghiep xn on xn.Ma = p.MaXuongSanXuat
//                            left join LineFilletv2 ln on nl.MaLine = ln.Ma
//                            left join ViTriFillet vt on nl.MaViTri = vt.Ma
//                        where
//                            p.Ngay <= @toDate
//                            and p.Ngay >= @fromDate
//                            and p.MaXuongSanXuat = @xuongId
//                    ) p
//                    left JOIN LoTheoLine ll ON p.MaLine = ll.MaLine
//                    and p.Ngay = ll.Ngay
//                    and p.CodeId = ll.CodeId
//                    and ll.Gio = (
//                        Select
//                            MAX(llc.Gio)
//                        from
//                            LoTheoLine llc
//                        where
//                            llc.MaLine = p.MaLine
//                            and llc.CodeId = p.CodeId
//                            and llc.Ngay = p.Ngay
//                            and llc.Gio <= p.ThoiGianCan
//                    )
//            ) p
//            left JOIN BoTriLoSizeThanhPham bt ON p.LoTheoLine = bt.MaLo
//            and p.CodeId = bt.CodeId
//            and p.MaViTri = bt.MaViTri
//            and bt.Ngay = p.Ngay
//            and bt.Gio = (
//                Select
//                    MAX(btc.Gio)
//                from
//                    BoTriLoSizeThanhPham btc
//                where
//                    btc.MaLo = p.LoTheoLine
//                    and btc.MaViTri = p.MaViTri
//                    and btc.CodeId = p.CodeId
//                    and btc.Ngay = p.Ngay
//                    and btc.Gio <= p.ThoiGianCan
//            )
//            left join MaThanhPhamFillet tp on bt.MaThanhPham = tp.Ma
//            left join MaThanhPhamFillet tp2 on p.MaLoaiThanhPham = tp2.Ma
//            LEFT join MaSizeFillet s on bt.MaSize = s.Ma
//            LEFT join MaSizeFillet s2 on bt.MaSizePhu = s2.Ma
//    ) p
//    left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
//    left join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
//    left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma
//    LEFT join MaSizeFillet s on p.MaSize = s.Ma
//     ";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate, toDate, xuongId })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopNhanViensMix<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId)
    {
        var query = @"
Select p.*,
    p.TrongLuongTra * p.DinhMucBanTrungBinh as TrongLuongNhanUocTinh,
    ISNULL(dm.DinhMuc, tp.DinhMuc) as DinhMucChuan,
    cast(
        (
            case
                when p.DinhMucBanTrungBinh <= ISNULL(dm.DinhMuc, tp.DinhMuc) then 1
                else 0
            end
        ) as bit
    ) as DanhGia
from (
        Select p.Ngay,
            P.MaLo,
            p.MaNhanVien,
            p.MaLoaiCa,
            p.MaThanhPham,
            p.MaSize,
            p.MaMau,
            p.CaTra,
            SUM(p.TrongLuongTra) as TrongLuongTra,
            cast(AVG(p.DinhMucBan) as DECIMAL(18, 3)) as DinhMucBanTrungBinh,
            COUNT(p.TrongLuongTra) as SoRoTra
        from (
                Select p.*,
                    CAST(
                        case
                            when p.TrongLuongTraBan = 0 then 0
                            else p.TrongLuongNhan / p.TrongLuongTraBan
                        end as decimal(18, 3)
                    ) as DinhMucBan
                from (
                        select p.*,
                            SUM(p.TrongLuongTra) over (
                                PARTITION BY p.MaBan,
                                p.MaThe,
                                p.MaXuong,
                                p.Ngay,
                                p.TrongLuongNhan,
                                p.ThePhieuSanLuongId
                            ) as TrongLuongTraBan
                        from PhieuCanTPFilletv2 p
                        WHERE p.Ngay <= @toDate
                            and p.Ngay >= @fromDate
                            and p.MaXuong = @xuongId
and p.STT > 0
                  
                    ) p
            ) p
        GROUP BY p.Ngay,
            P.MaLo,
            p.MaNhanVien,
            p.MaLoaiCa,
            p.MaThanhPham,
            p.MaSize,
            p.MaMau,
            p.CaTra
    ) p
    left join(
        Select *
        from (
                Select d.*,
                    ROW_NUMBER() OVER(
                        PARTITION BY MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        MaThanhPham
                        ORDER BY Gio DESC
                    ) AS [ROW NUMBER]
                from DinhMucFillet d
                where Ngay >= @fromDate
                    and Ngay <= @toDate
                    And MaXuong = @xuongId
            ) dm
        Where dm.[ROW NUMBER] = 1
    ) dm on p.MaThanhPham = dm.MaThanhPham
    and p.MaLo = dm.MaLo
    and p.MaSize = dm.MaSize
    and p.CaTra = dm.CaTra
    and p.MaMau = dm.MaMau
    and p.MaLoaiCa = dm.MaLoaiCa
    and p.Ngay = dm.Ngay
    left JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate, toDate, xuongId })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopNhanViensMix<T>(
        DateTime fromDate,
        DateTime toDate, string nhanVienId,
        string xuongId)
    {
        var query = @"
Select p.*,
    p.TrongLuongTra * p.DinhMucBanTrungBinh as TrongLuongNhanUocTinh,
    ISNULL(dm.DinhMuc, tp.DinhMuc) as DinhMucChuan,
    cast(
        (
            case
                when p.DinhMucBanTrungBinh <= ISNULL(dm.DinhMuc, tp.DinhMuc) then 1
                else 0
            end
        ) as bit
    ) as DanhGia
from (
        Select p.Ngay,
            P.MaLo,
            p.MaNhanVien,
            p.MaLoaiCa,
            p.MaThanhPham,
            p.MaSize,
            p.MaMau,
            p.CaTra,
            SUM(p.TrongLuongTra) as TrongLuongTra,
            cast(AVG(p.DinhMucBan) as DECIMAL(18, 3)) as DinhMucBanTrungBinh,
            COUNT(p.TrongLuongTra) as SoRoTra
        from (
                Select p.*,
                    CAST(
                        case
                            when p.TrongLuongTraBan = 0 then 0
                            else p.TrongLuongNhan / p.TrongLuongTraBan
                        end as decimal(18, 3)
                    ) as DinhMucBan
                from (
                        select p.*,
                            SUM(p.TrongLuongTra) over (
                                PARTITION BY p.MaBan,
                                p.MaThe,
                                p.MaXuong,
                                p.Ngay,
                                p.TrongLuongNhan,
                                p.ThePhieuSanLuongId
                            ) as TrongLuongTraBan
                        from PhieuCanTPFilletv2 p
                        WHERE p.Ngay <= @toDate
                            and p.Ngay >= @fromDate
                            and p.MaXuong = @xuongId
and p.STT > 0
and p.MaNhanVien= @nhanVienId
                  
                    ) p
            ) p
        GROUP BY p.Ngay,
            P.MaLo,
            p.MaNhanVien,
            p.MaLoaiCa,
            p.MaThanhPham,
            p.MaSize,
            p.MaMau,
            p.CaTra
    ) p
    left join(
        Select *
        from (
                Select d.*,
                    ROW_NUMBER() OVER(
                        PARTITION BY MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        MaThanhPham
                        ORDER BY Gio DESC
                    ) AS [ROW NUMBER]
                from DinhMucFillet d
                where Ngay >= @fromDate
                    and Ngay <= @toDate
                    And MaXuong = @xuongId
            ) dm
        Where dm.[ROW NUMBER] = 1
    ) dm on p.MaThanhPham = dm.MaThanhPham
    and p.MaLo = dm.MaLo
    and p.MaSize = dm.MaSize
    and p.CaTra = dm.CaTra
    and p.MaMau = dm.MaMau
    and p.MaLoaiCa = dm.MaLoaiCa
    and p.Ngay = dm.Ngay
    left JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate, toDate, xuongId, nhanVienId })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopNhanViensType1_2<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var query = @"
Select p.Ngay as [Ngày],
    p.MaNhanVien as [Mã Nhân Viên],
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    b.Ten as [Bàn],
    p.SoBanDuocSap as [Số Bàn Được Sắp],
    tp.Ten as [Thành Phẩm],
    map.MaBravoFillet as [Mã Sản Phẩm],
    p.TrongLuong as [TL Trả],
    p.SoRo as [Số Rổ Trả],
    cast(pBan.TrongLuongNhan as DECIMAL(18, 2)) as [TL Nhận Bàn],
    pBan.TrongLuongTra as [TL Trả Bàn],
    pBan.DinhMuc as [DM Bàn],
    dm.DinhMuc as [DM Chuẩn],
    pBan.SoRoNhan as [Số Rổ Nhận Bàn],
    pBan.SoRoTra as [Số Rổ Trả Bàn],
    p.MaXuong as [Xưởng]
from (
        select p.Ngay,
            p.MaNhanVien,
            p.MaBan,
            '0' as MaSize,
            'A' as MaLoaiCa,
            (
                dense_rank() over (
                    PARTITION BY p.Ngay,
                    p.MaNhanVien
                    order by p.MaBan
                ) + dense_rank() over (
                    PARTITION BY p.Ngay,
                    p.MaNhanVien
                    order by p.MaBan desc
                ) -1
            ) as SoBanDuocSap,
            p.MaThanhPham,
            p.MaXuong,
            cast(0 as bit) as IsTangCa,
            SUM(p.TrongLuongTra) as TrongLuong,
            COUNT(*) as SoRo
        from PhieuCanTPFilletv2 p
        where p.Ngay <= @ngay
            and p.Ngay >= @fromDate
            and p.MaXuong = @xuongId
            and p.STT > 0
        GROUP BY p.Ngay,
            p.MaNhanVien,
            p.MaThanhPham,
            p.MaXuong,
            p.MaBan
    ) p
    LEFT JOIN (
        Select p.Ngay,
            p.MaBan,
            p.MaThanhPham,
            p.MaXuong,
            SUM(p.TrongLuongTra) as TrongLuongTra,
            SUM(p.TrongLuongNhan) as TrongLuongNhan,
            Cast(
                case
                    when ISNULL(SUM(p.TrongLuongTra), 0) = 0 then 0
                    else SUM(p.TrongLuongNhan) / SUM(p.TrongLuongTra)
                end as DECIMAL(18, 2)
            ) as DinhMuc,
            SUM(p.SoRoTra) as SoRoTra,
            sum(p.SoRoNhan) as SoRoNhan
        from (
                Select p.*,
                    t.TrongLuongNhan,
                    t.SoRoNhan
                from(
                        select p.Ngay,
                            p.MaBan,
                            p.MaThanhPham,
                            p.MaXuong,
                            SUM(p.TrongLuongTra) as TrongLuongTra,
                            COUNT(*) as SoRoTra
                        from PhieuCanTPFilletv2 p
                        where p.Ngay <= @ngay
                            and p.Ngay >= @fromDate
                            and p.MaXuong = @xuongId
                            and p.STT > 0
                        GROUP BY p.Ngay,
                            p.MaThanhPham,
                            p.MaXuong,
                            p.MaBan
                    ) p
                    LEFT JOIN (
                        select p.Ngay,
                            p.MaBan,
                            p.MaThanhPham,
                            p.MaXuong,
                            SUM(p.TrongLuongNhan) as TrongLuongNhan,
                            count(*) as SoRoNhan
                        from ThePhieuSanLuongFillet p
                        where p.Ngay <= @ngay
                            and p.Ngay >= @fromDate
                            and p.MaXuong = @xuongId
                        GROUP BY p.Ngay,
                            p.MaBan,
                            p.MaXuong,
                            p.MaThanhPham
                    ) t on p.Ngay = t.Ngay
                    and p.MaXuong = t.MaXuong
                    and p.MaBan = t.MaBan
                    and p.MaThanhPham = t.MaThanhPham
            ) p
        GROUP BY p.Ngay,
            p.MaBan,
            p.MaThanhPham,
            p.MaXuong
    ) pBan on p.Ngay = pBan.Ngay
    and p.MaXuong = pBan.MaXuong
    and p.MaThanhPham = pBan.MaThanhPham
    and p.MaBan = pBan.MaBan
    LEFT JOIN BanFillet b on p.MaBan = b.Ma
    LEFT JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
    LEFT JOIN (
        Select *
        from (
                Select d.*,
                    ROW_NUMBER() OVER (
                        PARTITION BY MaThanhPham
                        ORDER BY Gio DESC
                    ) AS [ROW NUMBER]
                from DinhMucFillet d
                where Ngay = @ngay
                    And MaXuong = @xuongId
            ) dm
        Where dm.[ROW NUMBER] = 1
    ) dm on p.MaThanhPham = dm.MaThanhPham
    LEFT JOIN (
        select map.*,
            isnull(dg.DonGia, 0) as DonGia,
            isnull(dg.HeSo, 1) as HeSo
        from MapThanhPhamFillet map
            LEFT JOIN (
                Select *
                from (
                        Select *,
                            ROW_NUMBER() over (
                                partition by MaSanPham
                                order by Ngay DESC,
                                    Gio Desc
                            ) as rowId
                        from DG_DonGia
                        where Ngay <= @ngay
                            and MaLoaiDonGia = 'DM'
                            and LoaiCan = ''
                    ) p
                where p.rowId = 1
            ) dg ON dg.MaSanPham = map.MaBravoFillet
    ) map on p.MaThanhPham = map.MaTPFillet
    and p.IsTangCa = map.IsTangCa
    and p.MaSize = map.MaSize
    and p.MaLoaiCa = map.MaLoaiCaFillet
order by p.ngay,
    p.MaBan,
    n.MaHoSo,
    p.MaThanhPham
";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(
                query,
                new { fromDate = fromDate.Date, ngay = toDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }

    // Bao Cao Tong hop 2 + TongHop
    public List<T> GetTongHopNhanVienTheoDoiLois<T>(DateTime dateTime, string xuongId)
    {
        var query = @"Select
    tp.MaNhanVien,
    tp.MaHoSo,
    tp.TenNhanVien,
    tp.ThanhPhamName,
    Case
        when tp.SizeName = N'Cá Lớn' then 'L'
        when tp.SizeName = N'Cá Nhỏ' then 'N'
        else tp.SizeName
    end as SizeName,
    case
        when tp.CaTra = 1 then 'True'
        when tp.CaTra = 0 then 'F'
        else ''
    end as CaTra,
    tp.MaLo,
    tp.SoRoTra,
    tp.TrongLuongNhan,
    tp.TrongLuongTra,
    tp.DinhMuc,
    case
        when tp.CaTra = 1
        and dm.DinhMuc is null then 1.37
        else isnull(dm.DinhMuc, tp._DinhMucChuan)
    end as DinhMucChuan,
    case
        when tp.DinhMuc <= (
            case
                when tp.CaTra = 1
                and dm.DinhMuc is null then 1.37
                else isnull(dm.DinhMuc, tp._DinhMucChuan)
            end
        ) then N'Đ'
        ELSE N'Không Đạt'
    end as DanhGia,
    btp.SoRoChuaCanTP,
    btp.SoRoHuy,
    btp.SoRoChuaCanTP + btp.SoRoHuy as SoLoi
from
    (
        Select
            ptp.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            tp.Ma as MaThanhPham,
            tp.Ten as ThanhPhamName,
            s.Ma as MaSize,
            s.Ten as SizeName,
            ptp.CaTra,
            ptp.MaLo,
            ptp.MaMau,
            ptp.MaLoaiCa,
            COUNT(*) as SoRoTra,
            Sum(ptp.TrongLuongNhan) as TrongLuongNhan,
            sum(ptp.TrongLuongTra) as TrongLuongTra,
            floor(
                100 * Sum(pbtp.TrongLuong) / sum(ptp.TrongLuongTra)
            ) / 100 as DinhMuc,
            tp.DinhMuc as _DinhMucChuan
        from
            PhieuCanTPFilletv2 ptp,
            PhieuCanBTPFilletv2 pbtp,
            NhanVienDaiThanh n,
            MaThanhPhamFillet tp,
            MaSizeFillet s
        where
            ptp.Ngay = @ngay
            and ptp.MaXuong = @xuongId
            and ptp.MaNhanVien = n.MaNhanVien
            and ptp.MaThanhPham = tp.Ma
            and ptp.MaSize = s.Ma
            and ptp.TrongLuongTra > 0
            and ptp.MaNhanVien = pbtp.MaNhanVien
            and ptp.MaLo =pbtp.MaLo
            and ptp.MaSize= pbtp.MaSize
            and ptp.MaThanhPham= pbtp.MaThanhPham
            and ptp.MaMau = pbtp.MaMau
            and ptp.MaLoaiCa= pbtp.MaLoaiCa
        group by
            ptp.MaNhanVien,
            n.MaHoSo,
            n.Name,
            tp.Ma,
            s.Ma,
            ptp.CaTra,
            ptp.MaLo,
            ptp.MaMau,
            ptp.MaLoaiCa,
            tp.ten,
            s.Ten,
            tp.DinhMuc
    ) tp
    left join(
        Select
            *
        from
            (
                Select
                    d.*,
                    ROW_NUMBER() OVER (
                        PARTITION BY MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        MaThanhPham,
                        CaTra
                        ORDER BY
                            Gio DESC
                    ) AS [ROW NUMBER]
                from
                    DinhMucFillet d
                where
                    Ngay = @ngay
                    And MaXuong = @xuongId
            ) dm
        Where
            dm.[ROW NUMBER] = 1
    ) dm on tp.MaThanhPham = dm.MaThanhPham
    and tp.MaLo = dm.MaLo
    and tp.MaSize = dm.MaSize
    and tp.CaTra = dm.CaTra
    and tp.MaMau = dm.MaMau
    and tp.MaLoaiCa = dm.MaLoaiCa
    left join (
        Select
            MaNhanVien,
            MaLo,
            MaSize,
            MaLoaiCa,
            MaThanhPham,
            CaTra,
            COUNT(NULLIF(1, IsEnabled)) as SoRoChuaCanTP,
            COUNT(
                case
                    when ISNULL(GhiChu, '') = 'HUY' then 1
                    else null
                end
            ) as SoRoHuy
        from
            PhieuCanBTPFilletv2
        where
            Ngay = @ngay
            and MaXuong = @xuongId
        Group by
            MaNhanVien,
            MaLo,
            MaSize,
            MaLoaiCa,
            MaThanhPham,
            CaTra
    ) btp ON tp.MaNhanVien = btp.MaNhanVien
    and tp.MaLo = btp.MaLo
    and tp.MaSize = btp.MaSize
    and tp.MaLoaiCa = btp.MaLoaiCa
    and tp.MaThanhPham = btp.MaThanhPham
    and tp.CaTra = btp.CaTra
    

order by
    MaHoSo";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopThanhPham<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var query = @"
SELECT btp.Ngay as [Ngày],
    btp.MaLo as [Lô],
    la.Ten as [Loại Cá],
    _tp.Ten as [Thành Phẩm],
    CAST(
        (
            case
                when SUM(isnull(btp.TrongLuongNhan, 0)) over(
                    PARTITION BY btp.Ngay,
                    btp.Malo,
                    btp.MaXuong
                    ORDER BY btp.Ngay
                ) = 0 then 0
                else isnull(btp.TrongLuongNhan, 0) / (
                    SUM(isnull(btp.TrongLuongNhan, 0)) over(
                        PARTITION BY btp.Ngay,
                        btp.Malo,
                        btp.MaXuong
                        ORDER BY btp.Ngay
                    )
                )
            end
        ) * 100 AS decimal(18, 2)
    ) as [Tỷ Lệ Nhận],
    CAST(
        (
            case
                when SUM(isnull(tp.TrongLuongTra, 0)) over(
                    PARTITION BY tp.Ngay,
                    tp.Malo,
                    tp.MaXuong
                    ORDER BY tp.ngay
                ) = 0 then 0
                else isnull(tp.TrongLuongTra, 0) / (
                    SUM(isnull(tp.TrongLuongTra, 0)) over(
                        PARTITION BY tp.Ngay,
                        tp.Malo,
                        tp.MaXuong
                        ORDER BY tp.ngay
                    )
                )
            end
        ) * 100 as decimal(18, 2)
    ) as [Tỷ Lệ Trả],
    s.Ten as [Size],
    m.Ten as [Màu],
    btp.TrongLuongNhan as [TL Nhận],
    tp.TrongLuongTra as [TL Trả],
    cast(
        case
            when isnull(tp.TrongLuongTra, 0) = 0 then 0
            else btp.TrongLuongNhan / tp.TrongLuongTra
        END as decimal(18, 2)
    ) as [DM],
    dm.DinhMuc as [DM Chuẩn],
    btp.SoRo as [Số Rổ Nhận],
    tp.SoRo as [Số Rổ Trả],
    btp.MaXuong as [Xưởng]
from (
        Select p.Ngay,
            p.MaLo,
            p.MaLoaiCa,
            p.MaThanhPham,
            p.MaSize,
            p.MaMau,
            p.MaXuong,
            SUM(p.TrongLuong) as TrongLuongNhan,
            COUNT(*) as SoRo
        from PhieuCanBTPFilletv2 p
        left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
        where p.Ngay <= @ngay
            and p.Ngay >= @fromDate
            and p.MaXuong = @xuongId
            and p.STT > 0
            and ISNULL (p.GhiChu, '') <> 'HUY' and tp.IsNguyenCon = 0
        GROUP BY p.Ngay,
            p.MaLo,
            p.MaLoaiCa,
            p.MaThanhPham,
            p.MaSize,
            p.MaMau,
            p.MaXuong
    ) btp
    LEFT JOIN (
        Select p.Ngay,
            p.MaLo,
            p.MaLoaiCa,
            p.MaThanhPham,
            p.MaSize,
            p.MaMau,
            p.MaXuong,
            SUM(p.TrongLuongTra) as TrongLuongTra,
            COUNT(*) as SoRo
        from PhieuCanTPFilletv2 p
        left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
        where p.Ngay <= @ngay
            and p.Ngay >= @fromDate
            and p.MaXuong = @xuongId
            and p.STT > 0 and tp.IsNguyenCon = 0
        GROUP BY p.Ngay,
            p.MaLo,
            p.MaLoaiCa,
            p.MaThanhPham,
            p.MaSize,
            p.MaMau,
            p.MaXuong
    ) tp on btp.MaLo = tp.MaLo
    and btp.MaLoaiCa = tp.MaLoaiCa
    and btp.MaMau = tp.MaMau
    and btp.MaSize = tp.MaSize
    and btp.MaThanhPham = tp.MaThanhPham
    and btp.MaXuong = tp.MaXuong
    and btp.Ngay = tp.Ngay
    LEFT JOIN (
        Select *
        from (
                Select d.*,
                    ROW_NUMBER() OVER (
                        PARTITION BY MaThanhPham
                        ORDER BY Gio DESC
                    ) AS [ROW NUMBER]
                from DinhMucFillet d
                where Ngay = @ngay
                    And MaXuong = @xuongId
            ) dm
        Where dm.[ROW NUMBER] = 1
    ) dm on btp.MaThanhPham = dm.MaThanhPham
    LEFT JOIN MaLoaiCaFillet la on btp.MaLoaiCa = la.Ma
    LEFT JOIN MaThanhPhamFillet _tp on btp.MaThanhPham = _tp.Ma
    LEFT JOIN MaSizeFillet s on btp.MaSize = s.Ma
    LEFT JOIN MaMauFillet m on btp.MaMau = m.Ma
order by btp.Ngay,
    btp.MaLo,
    la.Ten,
    _tp.Ten,
    s.Ten,
    m.Ten
";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, ngay = toDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopThanhPham2<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var query = @"
SELECT btp.Ngay,
    btp.MaLo,
    la.Ten as LoaiCaName,
	_tp.Ma as MaThanhPham,
    _tp.Ten as ThanhPhamName,
    CAST(
        (
            case
                when SUM(isnull(btp.TrongLuongNhan, 0)) over(
                    PARTITION BY btp.Ngay,
                    btp.Malo,
                    btp.MaXuong
                    ORDER BY btp.Ngay
                ) = 0 then 0
                else isnull(btp.TrongLuongNhan, 0) / (
                    SUM(isnull(btp.TrongLuongNhan, 0)) over(
                        PARTITION BY btp.Ngay,
                        btp.Malo,
                        btp.MaXuong
                        ORDER BY btp.Ngay
                    )
                )
            end
        ) * 100 AS decimal(18, 2)
    ) as TyLeNhan,
    CAST(
        (
            case
                when SUM(isnull(tp.TrongLuongTra, 0)) over(
                    PARTITION BY tp.Ngay,
                    tp.Malo,
                    tp.MaXuong
                    ORDER BY tp.ngay
                ) = 0 then 0
                else isnull(tp.TrongLuongTra, 0) / (
                    SUM(isnull(tp.TrongLuongTra, 0)) over(
                        PARTITION BY tp.Ngay,
                        tp.Malo,
                        tp.MaXuong
                        ORDER BY tp.ngay
                    )
                )
            end
        ) * 100 as decimal(18, 2)
    ) as TyLeTra,
    s.Ten as SizeName,
    m.Ten as MauName,
    ISNULL( tp.TrongLuongNhan ,btp.TrongLuongNhan) as TrongLuongNhan ,
   isnull( tp.TrongLuongTra,0) as TrongLuongTra,
    cast(
        case
            when isnull(tp.TrongLuongTra, 0) = 0 then 0
            else tp.TrongLuongNhan / tp.TrongLuongTra
        END as decimal(18, 2)
    ) as DinhMuc,
    ISNULL(dm.DinhMuc,0) as DinhMucChuan,
    btp.SoRo as SoRoNhan,
    isnull(tp.SoRo,0) as SoRoTra,
    btp.MaXuong as XuongName,
    btp.MaXuong
from (
        Select p.Ngay,
            p.MaLo,
            p.MaLoaiCa,
            p.MaThanhPham,
            p.MaSize,
            p.MaMau,
            p.MaXuong,
            SUM(p.TrongLuong) as TrongLuongNhan,
            COUNT(*) as SoRo
        from PhieuCanBTPFilletv2 p
		left join MaThanhPhamFillet tp on tp.Ma = p.MaThanhPham
        where p.Ngay <= @ngay
            and p.Ngay >= @fromDate
            and p.MaXuong = @xuongId
            and p.STT > 0
            and ISNULL (p.GhiChu, '') <> 'HUY' and tp.IsNguyenCon = 0
        GROUP BY p.Ngay,
            p.MaLo,
            p.MaLoaiCa,
            p.MaThanhPham,
            p.MaSize,
            p.MaMau,
            p.MaXuong
    ) btp
    LEFT JOIN (
        Select p.Ngay,
            p.MaLo,
            p.MaLoaiCa,
            p.MaThanhPham,
            p.MaSize,
            p.MaMau,
            p.MaXuong,
			SUM(p.TrongLuongNhan) as TrongLuongNhan,
            SUM(p.TrongLuongTra) as TrongLuongTra,
            COUNT(*) as SoRo
        from PhieuCanTPFilletv2 p
		left join MaThanhPhamFillet tp on tp.Ma = p.MaThanhPham
        where p.Ngay <= @ngay
            and p.Ngay >= @fromDate
            and p.MaXuong = @xuongId
            and p.STT > 0
			and tp.IsNguyenCon = 0
        GROUP BY p.Ngay,
            p.MaLo,
            p.MaLoaiCa,
            p.MaThanhPham,
            p.MaSize,
            p.MaMau,
            p.MaXuong
    ) tp on btp.MaLo = tp.MaLo
    and btp.MaLoaiCa = tp.MaLoaiCa
    and btp.MaMau = tp.MaMau
    and btp.MaSize = tp.MaSize
    and btp.MaThanhPham = tp.MaThanhPham
    and btp.MaXuong = tp.MaXuong
    and btp.Ngay = tp.Ngay
    LEFT JOIN (
        Select *
        from (
                Select d.*,
                    ROW_NUMBER() OVER (
                        PARTITION BY MaThanhPham
                        ORDER BY Gio DESC
                    ) AS [ROW NUMBER]
                from DinhMucFillet d
                where Ngay = @ngay
                    And MaXuong = @xuongId
            ) dm
        Where dm.[ROW NUMBER] = 1
    ) dm on btp.MaThanhPham = dm.MaThanhPham
    LEFT JOIN MaLoaiCaFillet la on btp.MaLoaiCa = la.Ma
    LEFT JOIN MaThanhPhamFillet _tp on btp.MaThanhPham = _tp.Ma
    LEFT JOIN MaSizeFillet s on btp.MaSize = s.Ma
    LEFT JOIN MaMauFillet m on btp.MaMau = m.Ma
where _tp.IsNguyenCon = 0
order by btp.Ngay,
    btp.MaLo,
    la.Ten,
    _tp.Ten,
    s.Ten,
    m.Ten
";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, ngay = toDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopThanhPhamByMaHoSos<T>(
        DateTime fromDate,
        DateTime toDate,
        string maHoSo,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = false,
        bool isFloor = true
    )
    {
        var query = @"Select
    p.MaNhanVien,
	n.MaHoSo,
	n.Name as NhanVienName,
    p.MaLoaiCa,
    la.Ten as LoaiCaName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.MaMau,
    ma.Ten as MauName,
    p.CaTra,
    Sum(p.TrongLuongNhan*p.LoaiSanLuong) as TrongLuongNhan,
    Sum(p.TrongLuongTra*p.LoaiSanLuong) as TrongLuongTra,
    case
        when @isDinhMucBinhThuong = 1 then cast (
            case
                when sum(p.TrongLuongTra) = 0 then 0
                else (
                    case
                        when @isFloor = 1 then (
                            FLOOR(
                                (Sum(p.TrongLuongNhan) / sum(p.TrongLuongTra)) * 1000
                            ) / 1000
                        )
                        else Sum(p.TrongLuongNhan) / sum(p.TrongLuongTra)
                    end
                )
            end as decimal(18, 3)
        )
        ELSE cast (
            case
                when sum(p.TrongLuongNhan) = 0 then 0
                else sum(p.TrongLuongTra) / sum(p.TrongLuongNhan)
            end as decimal(18, 4)
        )
    end as DinhMuc,
    DinhMuc.DinhMuc as DinhMucChuan,
    Count(*) as SoRo
from
    (
        Select
            p.*,
            n.LoaiSanLuong
        from
            PhieuCanTPFilletv2 p,
            NhanVienDaiThanh n
        where
            p.Ngay <= @toDate
            and p.Ngay >= @fromDate
            and MaXuong = @xuongId
            and p.manhanvien = n.manhanvien
    ) p
    LEFT JOIN (
        Select
            tp1.MaLo,
            tp1.MaLoaiCa,
            tp1.MaSize,
            tp1.MaMau,
            CASE
                WHEN tp1.CaTra = 1
                and @isCaTraChuyenDoi = 1 THEN 'B'
                ELSE tp1.MaThanhPham
            END as MaThanhPham,
            tp1.CaTra,
            CASE
                WHEN tp2.DinhMuc is Null THEN tp1.DinhMuc
                ELSE tp2.DinhMuc
            END AS DinhMuc,
            tp1.Ngay,
            tp1.MaXuong
        from
            (
                Select
                    distinct p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham,
                    p.CaTra,
                    CASE
                        WHEN p.CaTra = 1 THEN 1.37
                        ELSE tp.DinhMuc
                    END AS DinhMuc,
                    p.Ngay,
                    p.MaXuong
                from
                    PhieuCanTPFilletv2 p,
                    MaThanhPhamFillet tp
                where
                    p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and MaXuong = @xuongId
                    and p.MaThanhPham = tp.Ma
                    and p.MaLoaiCa = tp.MaCa
            ) tp1
            LEFT JOIN (
                Select
                    MaLo,
                    MaLoaiCa,
                    MaSize,
                    MaMau,
                    MaThanhPham,
                    CaTra,
                    DinhMuc,
                    Ngay,
                    MaXuong
                from
                    (
                        Select
                            d.*,
                            ROW_NUMBER() OVER (
                                PARTITION BY MaLo,
                                MaLoaiCa,
                                MaMau,
                                MaSize,
                                MaThanhPham,
                                CaTra,
                                Ngay
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            DinhMucFillet d
                        where
                            d.Ngay <= @toDate
                            and d.Ngay >= @fromDate
                            And MaXuong = @xuongId
                    ) dm
                Where
                    dm.[ROW NUMBER] = 1
            ) tp2 on tp1.MaLo = tp2.MaLo
            and tp1.MaLoaiCa = tp2.MaLoaiCa
            and tp1.MaSize = tp2.MaSize
            and tp1.MaMau = tp2.MaMau
            and tp1.MaThanhPham = tp2.MaThanhPham
            and tp1.CaTra = tp2.CaTra
            and tp1.Ngay = tp2.Ngay
            and tp1.MaXuong = tp2.MaXuong
    ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
    and p.MaLo = DinhMuc.MaLo
    and p.MaLoaiCa = DinhMuc.MaLoaiCa
    and p.MaSize = DinhMuc.MaSize
    and p.MaMau = DinhMuc.MaMau
    and p.MaThanhPham = DinhMuc.MaThanhPham
    And p.CaTra = DinhMuc.CaTra
    and p.Ngay = DinhMuc.Ngay
    LEFT JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
    LEFT Join MaSizeFillet s on p.MaSize = s.Ma
    LEFT Join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
    LEFT Join MaMauFillet ma on p.MaMau = ma.Ma
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
where
    p.STT > 0
    and p.Ngay <= @toDate
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
	and n.MaHoSo = @maHoSo
group by
    p.MaLoaiCa,
    p.MaSize,
    p.MaThanhPham,
    p.MaMau,
    p.CaTra,
    DinhMuc.DinhMuc,
    p.Ngay,
    la.Ten,
    tp.Ten,
    ma.Ten,
    s.Ten,
    p.LoaiSanLuong,
	n.MaHoSo,
	n.Name,
	p.MaNhanVien";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate, toDate, maHoSo, isDinhMucBinhThuong, isCaTraChuyenDoi, isFloor, xuongId })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopThanhPhamByMaNhanViens<T>(
        DateTime fromDate,
        DateTime toDate,
        string maNhanVien,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = false,
        bool isFloor = true
    )
    {
        var query = @"Select
    p.MaNhanVien,
	n.MaHoSo,
	n.Name as NhanVienName,
    p.MaLoaiCa,
    la.Ten as LoaiCaName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.MaMau,
    ma.Ten as MauName,
    p.CaTra,
    Sum(p.TrongLuongNhan*p.LoaiSanLuong) as TrongLuongNhan,
    Sum(p.TrongLuongTra*p.LoaiSanLuong) as TrongLuongTra,
    case
        when @isDinhMucBinhThuong = 1 then cast (
            case
                when sum(p.TrongLuongTra) = 0 then 0
                else (
                    case
                        when @isFloor = 1 then (
                            FLOOR(
                                (Sum(p.TrongLuongNhan) / sum(p.TrongLuongTra)) * 1000
                            ) / 1000
                        )
                        else Sum(p.TrongLuongNhan) / sum(p.TrongLuongTra)
                    end
                )
            end as decimal(18, 3)
        )
        ELSE cast (
            case
                when sum(p.TrongLuongNhan) = 0 then 0
                else sum(p.TrongLuongTra) / sum(p.TrongLuongNhan)
            end as decimal(18, 4)
        )
    end as DinhMuc,
    DinhMuc.DinhMuc as DinhMucChuan,
    Count(*) as SoRo
from
    (
        Select
            p.*,
            n.LoaiSanLuong
        from
            PhieuCanTPFilletv2 p,
            NhanVienDaiThanh n
        where
            p.Ngay <= @toDate
            and p.Ngay >= @fromDate
            and MaXuong = @xuongId
            and p.manhanvien = n.manhanvien
    ) p
    LEFT JOIN (
        Select
            tp1.MaLo,
            tp1.MaLoaiCa,
            tp1.MaSize,
            tp1.MaMau,
            CASE
                WHEN tp1.CaTra = 1
                and @isCaTraChuyenDoi = 1 THEN 'B'
                ELSE tp1.MaThanhPham
            END as MaThanhPham,
            tp1.CaTra,
            CASE
                WHEN tp2.DinhMuc is Null THEN tp1.DinhMuc
                ELSE tp2.DinhMuc
            END AS DinhMuc,
            tp1.Ngay,
            tp1.MaXuong
        from
            (
                Select
                    distinct p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham,
                    p.CaTra,
                    CASE
                        WHEN p.CaTra = 1 THEN 1.37
                        ELSE tp.DinhMuc
                    END AS DinhMuc,
                    p.Ngay,
                    p.MaXuong
                from
                    PhieuCanTPFilletv2 p,
                    MaThanhPhamFillet tp
                where
                    p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and MaXuong = @xuongId
                    and p.MaThanhPham = tp.Ma
                    and p.MaLoaiCa = tp.MaCa
            ) tp1
            LEFT JOIN (
                Select
                    MaLo,
                    MaLoaiCa,
                    MaSize,
                    MaMau,
                    MaThanhPham,
                    CaTra,
                    DinhMuc,
                    Ngay,
                    MaXuong
                from
                    (
                        Select
                            d.*,
                            ROW_NUMBER() OVER (
                                PARTITION BY MaLo,
                                MaLoaiCa,
                                MaMau,
                                MaSize,
                                MaThanhPham,
                                CaTra,
                                Ngay
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            DinhMucFillet d
                        where
                            d.Ngay <= @toDate
                            and d.Ngay >= @fromDate
                            And MaXuong = @xuongId
                    ) dm
                Where
                    dm.[ROW NUMBER] = 1
            ) tp2 on tp1.MaLo = tp2.MaLo
            and tp1.MaLoaiCa = tp2.MaLoaiCa
            and tp1.MaSize = tp2.MaSize
            and tp1.MaMau = tp2.MaMau
            and tp1.MaThanhPham = tp2.MaThanhPham
            and tp1.CaTra = tp2.CaTra
            and tp1.Ngay = tp2.Ngay
            and tp1.MaXuong = tp2.MaXuong
    ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
    and p.MaLo = DinhMuc.MaLo
    and p.MaLoaiCa = DinhMuc.MaLoaiCa
    and p.MaSize = DinhMuc.MaSize
    and p.MaMau = DinhMuc.MaMau
    and p.MaThanhPham = DinhMuc.MaThanhPham
    And p.CaTra = DinhMuc.CaTra
    and p.Ngay = DinhMuc.Ngay
    LEFT JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
    LEFT Join MaSizeFillet s on p.MaSize = s.Ma
    LEFT Join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
    LEFT Join MaMauFillet ma on p.MaMau = ma.Ma
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
where
    p.STT > 0
    and p.Ngay <= @toDate
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
	and p.MaNhanVien = @maNhanVien
group by
    p.MaLoaiCa,
    p.MaSize,
    p.MaThanhPham,
    p.MaMau,
    p.CaTra,
    DinhMuc.DinhMuc,
    p.Ngay,
    la.Ten,
    tp.Ten,
    ma.Ten,
    s.Ten,
    p.LoaiSanLuong,
	n.MaHoSo,
	n.Name,
	p.MaNhanVien";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate, toDate, maNhanVien, isDinhMucBinhThuong, isCaTraChuyenDoi, isFloor, xuongId })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopThanhPhamByMaThes<T>(
        DateTime fromDate,
        DateTime toDate,
        string maThe,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = false,
        bool isFloor = true
    )
    {
        var query = @"Select
    p.MaNhanVien,
	n.MaHoSo,
	n.Name as NhanVienName,
    p.MaLoaiCa,
    la.Ten as LoaiCaName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.MaMau,
    ma.Ten as MauName,
    p.CaTra,
    Sum(p.TrongLuongNhan*p.LoaiSanLuong) as TrongLuongNhan,
    Sum(p.TrongLuongTra*p.LoaiSanLuong) as TrongLuongTra,
    case
        when @isDinhMucBinhThuong = 1 then cast (
            case
                when sum(p.TrongLuongTra) = 0 then 0
                else (
                    case
                        when @isFloor = 1 then (
                            FLOOR(
                                (Sum(p.TrongLuongNhan) / sum(p.TrongLuongTra)) * 1000
                            ) / 1000
                        )
                        else Sum(p.TrongLuongNhan) / sum(p.TrongLuongTra)
                    end
                )
            end as decimal(18, 3)
        )
        ELSE cast (
            case
                when sum(p.TrongLuongNhan) = 0 then 0
                else sum(p.TrongLuongTra) / sum(p.TrongLuongNhan)
            end as decimal(18, 4)
        )
    end as DinhMuc,
    DinhMuc.DinhMuc as DinhMucChuan,
    Count(*) as SoRo
from
    (
        Select
            p.*,
            n.LoaiSanLuong
        from
            PhieuCanTPFilletv2 p,
            NhanVienDaiThanh n
        where
            p.Ngay <= @toDate
            and p.Ngay >= @fromDate
            and MaXuong = @xuongId
            and p.manhanvien = n.manhanvien
    ) p
    LEFT JOIN (
        Select
            tp1.MaLo,
            tp1.MaLoaiCa,
            tp1.MaSize,
            tp1.MaMau,
            CASE
                WHEN tp1.CaTra = 1
                and @isCaTraChuyenDoi = 1 THEN 'B'
                ELSE tp1.MaThanhPham
            END as MaThanhPham,
            tp1.CaTra,
            CASE
                WHEN tp2.DinhMuc is Null THEN tp1.DinhMuc
                ELSE tp2.DinhMuc
            END AS DinhMuc,
            tp1.Ngay,
            tp1.MaXuong
        from
            (
                Select
                    distinct p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham,
                    p.CaTra,
                    CASE
                        WHEN p.CaTra = 1 THEN 1.37
                        ELSE tp.DinhMuc
                    END AS DinhMuc,
                    p.Ngay,
                    p.MaXuong
                from
                    PhieuCanTPFilletv2 p,
                    MaThanhPhamFillet tp
                where
                    p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and MaXuong = @xuongId
                    and p.MaThanhPham = tp.Ma
                    and p.MaLoaiCa = tp.MaCa
            ) tp1
            LEFT JOIN (
                Select
                    MaLo,
                    MaLoaiCa,
                    MaSize,
                    MaMau,
                    MaThanhPham,
                    CaTra,
                    DinhMuc,
                    Ngay,
                    MaXuong
                from
                    (
                        Select
                            d.*,
                            ROW_NUMBER() OVER (
                                PARTITION BY MaLo,
                                MaLoaiCa,
                                MaMau,
                                MaSize,
                                MaThanhPham,
                                CaTra,
                                Ngay
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            DinhMucFillet d
                        where
                            d.Ngay <= @toDate
                            and d.Ngay >= @fromDate
                            And MaXuong = @xuongId
                    ) dm
                Where
                    dm.[ROW NUMBER] = 1
            ) tp2 on tp1.MaLo = tp2.MaLo
            and tp1.MaLoaiCa = tp2.MaLoaiCa
            and tp1.MaSize = tp2.MaSize
            and tp1.MaMau = tp2.MaMau
            and tp1.MaThanhPham = tp2.MaThanhPham
            and tp1.CaTra = tp2.CaTra
            and tp1.Ngay = tp2.Ngay
            and tp1.MaXuong = tp2.MaXuong
    ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
    and p.MaLo = DinhMuc.MaLo
    and p.MaLoaiCa = DinhMuc.MaLoaiCa
    and p.MaSize = DinhMuc.MaSize
    and p.MaMau = DinhMuc.MaMau
    and p.MaThanhPham = DinhMuc.MaThanhPham
    And p.CaTra = DinhMuc.CaTra
    and p.Ngay = DinhMuc.Ngay
    LEFT JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
    LEFT Join MaSizeFillet s on p.MaSize = s.Ma
    LEFT Join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
    LEFT Join MaMauFillet ma on p.MaMau = ma.Ma
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
    LEFT JOIN TheTu t on p.MaNhanVien = n.MaNhanVien
where
    p.STT > 0
    and p.Ngay <= @toDate
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
	and t.MaTheTu = @maThe
group by
    p.MaLoaiCa,
    p.MaSize,
    p.MaThanhPham,
    p.MaMau,
    p.CaTra,
    DinhMuc.DinhMuc,
    p.Ngay,
    la.Ten,
    tp.Ten,
    ma.Ten,
    s.Ten,
    p.LoaiSanLuong,
	n.MaHoSo,
	n.Name,
	p.MaNhanVien";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate, toDate, maThe, isDinhMucBinhThuong, isCaTraChuyenDoi, isFloor, xuongId })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopThanhPhamMix<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId)
    {
        var query = @"
Select ISNULL(dm.DinhMuc, tp.DinhMuc) as DinhMucChuan,
    cast(
        (
            case
                when p.DinhMuc <= ISNULL(dm.DinhMuc, tp.DinhMuc) then 1
                else 0
            end
        ) as bit
    ) as DanhGia,
    p.*
from (
        SELECT btp.*,
            ISNULL(tp.TrongLuongTra, 0) as TrongLuongTra,
            cast(
                case
                    when ISNULL(tp.TrongLuongTra, 0) = 0 then 0
                    else btp.TrongLuongNhan / tp.TrongLuongTra
                end as DECIMAL(18, 3)
            ) as DinhMuc
        from (
                Select btp.Ngay,
                    btp.MaLo,
                    btp.MaLoaiCa,
                    btp.MaThanhPham,
                    btp.MaSize,
                    btp.MaMau,
                    SUM(btp.TrongLuong) as TrongLuongNhan
                from PhieuCanBTPFilletv2 btp
                where btp.Ngay <= @fromDate
                    and btp.Ngay >= @toDate
                    and btp.MaXuong = @xuongId
                    and btp.STT > 0
                    and ISNULL(btp.GhiChu, '') != ''
                GROUP BY btp.Ngay,
                    btp.MaLo,
                    btp.MaLoaiCa,
                    btp.MaThanhPham,
                    btp.MaSize,
                    btp.MaMau
            ) btp
            LEFT JOIN (
                Select tp.Ngay,
                    tp.MaLo,
                    tp.MaLoaiCa,
                    tp.MaThanhPham,
                    tp.MaSize,
                    tp.MaMau,
                    SUM(tp.TrongLuongTra) as TrongLuongTra
                from PhieuCanTPFilletv2 tp
                where tp.Ngay <= @fromDate
                    and tp.Ngay >= @toDate
                    and tp.MaXuong = @xuongId
                    and btp.STT > 0
                GROUP BY tp.Ngay,
                    tp.MaLo,
                    tp.MaLoaiCa,
                    tp.MaThanhPham,
                    tp.MaSize,
                    tp.MaMau
            ) tp on btp.MaLo = tp.MaLo
            and btp.MaLoaiCa = tp.MaLoaiCa
            and btp.MaThanhPham = tp.MaThanhPham
            and btp.MaSize = tp.MaSize
            and btp.MaMau = tp.MaMau
            and btp.Ngay = tp.Ngay
    ) p
    left join(
        Select *
        from (
                Select d.*,
                    ROW_NUMBER() OVER(
                        PARTITION BY MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        MaThanhPham
                        ORDER BY Gio DESC
                    ) AS [ROW NUMBER]
                from DinhMucFillet d
                where Ngay >= @fromDate
                    and Ngay <= @toDate
                    And MaXuong = @xuongId
            ) dm
        Where dm.[ROW NUMBER] = 1
    ) dm on p.MaThanhPham = dm.MaThanhPham
    and p.MaLo = dm.MaLo
    and p.MaSize = dm.MaSize
    and p.CaTra = dm.CaTra
    and p.MaMau = dm.MaMau
    and p.MaLoaiCa = dm.MaLoaiCa
    and p.Ngay = dm.Ngay
    left JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate, toDate, xuongId })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopToType1<T>(DateTime dateTime, string xuongId, int batdau, int ketthuc, string toName)
    {
        var query =
            @"SELECT @toName As ToName,p.CaTra, p.MaLo , la.Ten As LoaiCaName,tp.Ma As MaThanhPham,tp.Ten As ThanhPhamName,COUNT(p.STT) As SoRo,SUM(p.TrongLuongTra) as TrongLuongTra,SUM(p.TrongLuongNhan) as TrongLuongNhan,FLOOR((SUM(p.TrongLuongNhan)/SUM(p.TrongLuongTra)) *100)/100 as DinhMuc from PhieuCanTPFilletv2 p,MaThanhPhamFillet tp,MaLoaiCaFillet la, NhanVienDaiThanh n where p.MaLoaiCa = la.Ma and p.MaLoaiCa = tp.MaCa and p.MaThanhPham = tp.Ma and p.Ngay=@ngay and p.MaXuong =@xuongId and p.MaNhanVien = n.MaNhanVien and (case when ISNUMERIC(n.MaHoSo) = 1 then cast (n.MaHoSo as int) else null
  end) between @batdau and @ketthuc   group by p.MaLo, la.Ten,tp.Ten,tp.Ma,p.CaTra order by p.MaLo,tp.Ma";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new
                    {
                        ngay = dateTime.Date,
                        xuongId,
                        batdau,
                        ketthuc,
                        toName
                    })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopToType2<T>(DateTime dateTime, string xuongId, string toName)
    {
        var query =
            @"SELECT @toName As ToName,p.CaTra, p.MaLo , la.Ten As LoaiCaName,tp.Ma As MaThanhPham,tp.Ten As ThanhPhamName,COUNT(p.STT) As SoRo,SUM(p.TrongLuongTra) as TrongLuongTra,SUM(p.TrongLuongNhan) as TrongLuongNhan,FLOOR((SUM(p.TrongLuongNhan)/SUM(p.TrongLuongTra)) *100)/100 as DinhMuc from PhieuCanTPFilletv2 p,MaThanhPhamFillet tp,MaLoaiCaFillet la, NhanVienDaiThanh n where p.MaLoaiCa = la.Ma and p.MaLoaiCa = tp.MaCa and p.MaThanhPham = tp.Ma and p.Ngay=@ngay and p.MaXuong =@xuongId and p.MaNhanVien = n.MaNhanVien and ((case when ISNUMERIC(n.MaHoSo) = 1 then cast (n.MaHoSo as int) else 0
  end) NOT between 1 and 300) and ((case when ISNUMERIC(n.MaHoSo) = 1 then cast (n.MaHoSo as int) else 0
  end) NOT between 431 and 699 )    group by p.MaLo, la.Ten,tp.Ten,tp.Ma,p.CaTra order by p.MaLo,tp.Ma";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { ngay = dateTime.Date, xuongId, toName })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopToType2X2<T>(DateTime dateTime, string xuongId, string toName)
    {
        var query =
            @"SELECT @toName As ToName,p.CaTra, p.MaLo , la.Ten As LoaiCaName,tp.Ma As MaThanhPham,tp.Ten As ThanhPhamName,COUNT(p.STT) As SoRo,SUM(p.TrongLuongTra) as TrongLuongTra,SUM(p.TrongLuongNhan) as TrongLuongNhan,FLOOR((SUM(p.TrongLuongNhan)/SUM(p.TrongLuongTra)) *100)/100 as DinhMuc from PhieuCanTPFilletv2 p,MaThanhPhamFillet tp,MaLoaiCaFillet la, NhanVienDaiThanh n where p.MaLoaiCa = la.Ma and p.MaLoaiCa = tp.MaCa and p.MaThanhPham = tp.Ma and p.Ngay=@ngay and p.MaXuong =@xuongId and p.MaNhanVien = n.MaNhanVien and ((case when ISNUMERIC(n.MaHoSo) = 1 then cast (n.MaHoSo as int) else 0
  end) NOT between 1 and 160) and ((case when ISNUMERIC(n.MaHoSo) = 1 then cast (n.MaHoSo as int) else 0
  end) NOT between 161 and 380 )    group by p.MaLo, la.Ten,tp.Ten,tp.Ma,p.CaTra order by p.MaLo,tp.Ma";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { ngay = dateTime.Date, xuongId, toName })
                .Result
                .ToList();
            return items;
        }
    }

    //bao cao tong hop 3
    public List<T> GetTongHopTypeTinhLuong<T>(DateTime dateTime, string xuongId)
    {
        var query = @"SELECT
    p.*,
    Round(p.TrongLuongTra / p.SoRoTra, 2) as BinhQuan,
    Case
        when p.DinhMuc <= p.DinhMucChuan then N'Đ'
        else N'Không Đạt'
    end as DanhGia
from
    (
        Select
            p.MaNhanVien,
            p.MaHoSo,
            p.TenNhanVien,
            Cast(p.DinhMucChuan as varchar(10)) + '-' + p.ThanhPhamName + '-' + p.SizeName + case
                when p.CaTra = 1 then '-CaTra'
                else ''
            end as Header,
            p.CaTra,
            p.DinhMucChuan,
            SUM(p.SoRoTra) as SoRoTra,
            SUM(p.TrongLuongNhan) as TrongLuongNhan,
            Sum (p.TrongLuongTra) as TrongLuongTra,
            floor(
                100 * SUM(p.TrongLuongNhan) / Sum (p.TrongLuongTra)
            ) / 100 as DinhMuc
        from
            (
                Select
                    tp.MaNhanVien,
                    tp.MaHoSo,
                    tp.TenNhanVien,
                    tp.ThanhPhamName,
                    tp.SizeName,
                    tp.CaTra,
                    tp.SoRoTra,
                    tp.TrongLuongNhan,
                    tp.TrongLuongTra,
                    Case
                        when tp.CaTra = 1
                        and dm.DinhMuc is null then 1.37
                        else ISNULL(dm.DinhMuc, tp._DinhMucChuan)
                    end as DinhMucChuan
                from
                    (
                        Select
                            ptp.MaNhanVien,
                            n.MaHoSo,
                            n.Name as TenNhanVien,
                            tp.Ma as MaThanhPham,
                            tp.Ten as ThanhPhamName,
                            s.Ma as MaSize,
                            s.Ten as SizeName,
                            ptp.CaTra,
                            ptp.MaLo,
                            ptp.MaMau,
                            ptp.MaLoaiCa,
                            COUNT(*) as SoRoTra,
                            Sum(ptp.TrongLuongNhan) as TrongLuongNhan,
                            sum(ptp.TrongLuongTra) as TrongLuongTra,
                            floor(
                                100 * Sum(ptp.TrongLuongNhan) / sum(ptp.TrongLuongTra)
                            ) / 100 as DinhMuc,
                            tp.DinhMuc as _DinhMucChuan
                        from
                            PhieuCanTPFilletv2 ptp,
                            NhanVienDaiThanh n,
                            MaThanhPhamFillet tp,
                            MaSizeFillet s
                        where
                            ptp.Ngay = @ngay
                            and ptp.MaXuong = @xuongId
                            and ptp.MaNhanVien = n.MaNhanVien
                            and ptp.MaThanhPham = tp.Ma
                            and ptp.MaSize = s.Ma
                            and ptp.TrongLuongTra > 0
                        group by
                            ptp.MaNhanVien,
                            n.MaHoSo,
                            n.Name,
                            tp.Ma,
                            s.Ma,
                            ptp.CaTra,
                            ptp.MaLo,
                            ptp.MaMau,
                            ptp.MaLoaiCa,
                            tp.ten,
                            s.Ten,
                            tp.DinhMuc
                    ) tp
                    left join(
                        Select
                            *
                        from
                            (
                                Select
                                    d.*,
                                    ROW_NUMBER() OVER (
                                        PARTITION BY MaLo,
                                        MaLoaiCa,
                                        MaMau,
                                        MaSize,
                                        MaThanhPham,
                                        CaTra
                                        ORDER BY
                                            Gio DESC
                                    ) AS [ROW NUMBER]
                                from
                                    DinhMucFillet d
                                where
                                    Ngay = @ngay
                                    And MaXuong = @xuongId
                            ) dm
                        Where
                            dm.[ROW NUMBER] = 1
                    ) dm on tp.MaThanhPham = dm.MaThanhPham
                    and tp.MaLo = dm.MaLo
                    and tp.MaSize = dm.MaSize
                    and tp.CaTra = dm.CaTra
                    and tp.MaMau = dm.MaMau
                    and tp.MaLoaiCa = dm.MaLoaiCa
                    left join (
                        Select
                            MaNhanVien,
                            MaLo,
                            MaSize,
                            MaLoaiCa,
                            MaThanhPham,
                            CaTra,
                            COUNT(NULLIF(1, IsEnabled)) as SoRoChuaCanTP,
                            COUNT(
                                case
                                    when ISNULL(GhiChu, '') = 'HUY' then 1
                                    else null
                                end
                            ) as SoRoHuy
                        from
                            PhieuCanBTPFilletv2
                        where
                            Ngay = @ngay
                            and MaXuong = @xuongId
                        Group by
                            MaNhanVien,
                            MaLo,
                            MaSize,
                            MaLoaiCa,
                            MaThanhPham,
                            CaTra
                    ) btp ON tp.MaNhanVien = btp.MaNhanVien
                    and tp.MaLo = btp.MaLo
                    and tp.MaSize = btp.MaSize
                    and tp.MaLoaiCa = btp.MaLoaiCa
                    and tp.MaThanhPham = btp.MaThanhPham
                    and tp.CaTra = btp.CaTra
            ) p
        GROUP BY
            p.MaNhanVien,
            p.MaHoSo,
            p.TenNhanVien,
            p.DinhMucChuan,
            p.SizeName,
            p.CaTra,
            p.ThanhPhamName
    ) p
order by
    Header";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result
                .ToList();
            return items;
        }
    }

    public int Insert<T>(T item)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var rows = connection.Execute(qrInsert, item);
        return rows;
    }

    public int Insert<T>(List<T> items)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var rows = connection.Execute(qrInsert, items);
            return rows;
        }
    }

    public int Update<T>(T item)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var rows = connection.Execute(qrUpdate, item);
        return rows;
    }

    public int Update<T>(List<T> items)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var rows = connection.Execute(qrUpdate, items);
            return rows;
        }
    }

    #region TP FILLET HAI NẮM

    public List<T> GetChiTiets2HN<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true)
    {
        var query = @$"DECLARE @fromDate as Date,
@toDate as DATE,
@xuongId as VARCHAR(50),
@isDinhMucBinhThuong as bit;

set
    @fromDate = '{fromDate.ToString("yyyy - MM - dd")}';

set
    @toDate = '{toDate.ToString("yyyy-MM-dd")}';

                set
                    @xuongId = '{xuongId}';

                set
                    @isDinhMucBinhThuong = {(isDinhMucBinhThuong ? "1" : "0")};
select (case
				when p.DanhGia = 1 then N'Đậu'
				else N'Rớt'
			end) as DanhGia2 ,p.* from 
(Select
    tp.STT,
    tp.Ngay,
    tp.Gio,
    cast(
        DATEDIFF(MINUTE, pbtp.Gio, tp.Gio) as decimal(18, 3)
    ) as ThoiGianHoanThanh,
    tp.MaNhanVien,
    tp.IsGiaCong,
    tp.MaHoSo,
    tp.TenNhanVien,
    --tp.MaNhanVienBanKiem,
    --nbk.Name as TenNhanVienBanKiem,
    --nbk.MaHoSo as MaNhanVienBanKiemHoSo,
    --tp.MaNhanVienPhucVu,
    --npv.Name as TenNhanVienPhucVu,
    --npv.MaHoSo as MaNhanVienPhucVuHoSo,
    tp.Nhom,
    tp.MaThanhPham,
    tp.Ten as TenXuong,
    tp.ThanhPhamName,
    tp.LoaiCaName,
    tp.MauName,
    tp.SizeName,
    tp.MaSize,
    --case
    --    when tp.CaTra = 1 then 'True'
    --    when tp.CaTra = 0 then 'F'
    --    else ''
    --end as CaTra,
    tp.MaLo,
    tp.MaThe,
    case
        when @isDinhMucBinhThuong = 1 then(tp.DinhMuc)
        else (
            case
                when tp.DinhMuc > 1 THEN 1
                else (
                    tp.DinhMuc
                )
            end
        )
    end as DinhMuc,
    ISNULL(dm.DinhMuc, tp.DinhMucChuan) as DinhMucChuan,
    tp.TrongLuongNhan,
    tp.TrongLuongTra,
    tp.TrongLuongTare,

    cast(
        (
            Case
                when @isDinhMucBinhThuong = 1 then case
                    when tp.DinhMuc <= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                    else 0
                end
                else case
                    when tp.DinhMuc >= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                    else 0
                end
            end
        ) as bit
    ) as DanhGia,

    --0 as DonGia,
    --0 as ThanhTien,
    tp.MaMayCan,
    pbtp.Gio as GioBTP,
    pbtp.MaMayCan as MayCanBTP,
    tp.MaXuong,
    --tp.MaSanPham,
    tp.GhiChu
from
    (
        Select
            ptp.STT,
            ptp.Gio,
            ptp.STTBTP,
            ptp.MaMayCanBTP,
            ptp.Ngay,
            ptp.MaNhanVien,
            ptp.MaNhanVienPhucVu,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            n.IsGiaCong,
            tp.Ma as MaThanhPham,
            tp.Ten as ThanhPhamName,
            n.LoaiSanLuong,
            s.Ma as MaSize,
            s.Ten as SizeName,
            ptp.CaTra,
            ptp.MaLo,
            ptp.MaMau,
            ptp.MaLoaiCa,
            ptp.MaThe,
            la.Ten as LoaiCaName,
            mau.Ten as MauName,
            case
                when @isDinhMucBinhThuong = 1 then(
                    case
                        when ptp.TrongLuongTra = 0 then 0
                        else floor(
                            1000 * ptp.TrongLuongNhan / ptp.TrongLuongTra
                        ) / 1000
                    end
                )
                else (
                    case
                        when(ptp.TrongLuongNhan) = 0 THEN 0
                        else (
                            ROUND(
                                ptp.TrongLuongTra / (ptp.TrongLuongNhan),
                                4
                            )
                        )
                    end
                )
            end as DinhMuc,
            ptp.TrongLuongTare,
            tp.DinhMuc as DinhMucChuan,
            ptp.TrongLuongNhan,
            ptp.TrongLuongTra,
            ptp.MaMayCan,
            ptp.MaXuong,
            xn.Ten,
            ptp.GhiChu
        from
            PhieuCanTPFilletv2 ptp
            left join NhanVienDaiThanh n on ptp.MaNhanVien = n.MaNhanVien
            left join MaThanhPhamFillet tp on ptp.MaThanhPham = tp.Ma
            left join MaSizeFillet s on ptp.MaSize = s.Ma
            left join MaMauFillet mau on ptp.MaMau =  mau.Ma
            left join MaLoaiCaFillet la on ptp.MaLoaiCa = la.Ma
            left join XiNghiep xn on ptp.MaXuong = xn.Ma
        where
            ptp.Ngay >= @fromDate
            and ptp.Ngay <= @toDate
            and ptp.MaXuong = @xuongId
            and ptp.TrongLuongTra > 0
            and tp.IsNguyenCon = 0
    ) tp
    left join(
        Select
            *
        from
            (
                Select
                    d.*,
                    ROW_NUMBER() OVER(
                        PARTITION BY MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        MaThanhPham
                        ORDER BY
                            Gio DESC
                    ) AS [ROW NUMBER]
                from
                    DinhMucFillet d
                where
                    Ngay >= @fromDate
                    and Ngay <= @toDate
                    And MaXuong = @xuongId
            ) dm
        Where
            dm.[ROW NUMBER] = 1
    ) dm on tp.MaThanhPham = dm.MaThanhPham
    and tp.MaLo = dm.MaLo
    and tp.MaSize = dm.MaSize
    and tp.CaTra = dm.CaTra
    and tp.MaMau = dm.MaMau
    and tp.MaLoaiCa = dm.MaLoaiCa
    and tp.Ngay = dm.Ngay
    LEFT JOIN PhieuCanBTPFilletv2 pbtp on tp.Ngay = pbtp.Ngay
    and tp.STTBTP = pbtp.STT
    and tp.MaMayCanBTP = pbtp.MaMayCan)p
order by
    Gio";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query).Result.ToList();
            return items;
        }
    }


    public List<T> GetChiTiets2HN<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        string maNhanVien,
        bool isDinhMucBinhThuong = true)
    {
        var query = @$"DECLARE @fromDate as Date,
@toDate as DATE,
@xuongId as VARCHAR(50),
@maNhanVien as VARCHAR(50),
@isDinhMucBinhThuong as bit;

set
    @fromDate = '{fromDate.ToString("yyyy - MM - dd")}';

set
    @toDate = '{toDate.ToString("yyyy-MM-dd")}';

                set
                    @xuongId = '{xuongId}';

                set
                    @isDinhMucBinhThuong = {(isDinhMucBinhThuong ? "1" : "0")};
set @maNhanVien = '{maNhanVien}';
select (case
				when p.DanhGia = 1 then N'Đậu'
				else N'Rớt'
			end) as DanhGia2 ,p.* from 
(Select
    tp.STT,
    tp.Ngay,
    tp.Gio,
    cast(
        DATEDIFF(MINUTE, pbtp.Gio, tp.Gio) as decimal(18, 3)
    ) as ThoiGianHoanThanh,
    tp.MaNhanVien,
    tp.IsGiaCong,
    tp.MaHoSo,
    tp.TenNhanVien,
    --tp.MaNhanVienBanKiem,
    --nbk.Name as TenNhanVienBanKiem,
    --nbk.MaHoSo as MaNhanVienBanKiemHoSo,
    --tp.MaNhanVienPhucVu,
    --npv.Name as TenNhanVienPhucVu,
    --npv.MaHoSo as MaNhanVienPhucVuHoSo,
    tp.Nhom,
    tp.MaThanhPham,
    tp.Ten as TenXuong,
    tp.ThanhPhamName,
    tp.LoaiCaName,
    tp.MauName,
    Case
        when tp.SizeName = N'Cá Lớn' then 'L'
        when tp.SizeName = N'Cá Nhỏ' then 'N'
        else tp.SizeName
    end as SizeName,
    tp.MaSize,
    --case
    --    when tp.CaTra = 1 then 'True'
    --    when tp.CaTra = 0 then 'F'
    --    else ''
    --end as CaTra,
    tp.MaLo,
    tp.MaThe,
    case
        when @isDinhMucBinhThuong = 1 then(tp.DinhMuc)
        else (
            case
                when tp.DinhMuc > 1 THEN 1
                else (
                    tp.DinhMuc
                )
            end
        )
    end as DinhMuc,
    ISNULL(dm.DinhMuc, tp.DinhMucChuan) as DinhMucChuan,
    tp.TrongLuongNhan,
    tp.TrongLuongTra,
    tp.TrongLuongTare,

    cast(
        (
            Case
                when @isDinhMucBinhThuong = 1 then case
                    when tp.DinhMuc <= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                    else 0
                end
                else case
                    when tp.DinhMuc >= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                    else 0
                end
            end
        ) as bit
    ) as DanhGia,

    --0 as DonGia,
    --0 as ThanhTien,
    tp.MaMayCan,
    pbtp.Gio as GioBTP,
    pbtp.MaMayCan as MayCanBTP,
    tp.MaXuong,
    --tp.MaSanPham,
    tp.GhiChu
from
    (
        Select
            ptp.STT,
            ptp.Gio,
            ptp.STTBTP,
            ptp.MaMayCanBTP,
            ptp.Ngay,
            ptp.MaNhanVien,
            ptp.MaNhanVienPhucVu,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            n.IsGiaCong,
            tp.Ma as MaThanhPham,
            tp.Ten as ThanhPhamName,
            n.LoaiSanLuong,
            s.Ma as MaSize,
            s.Ten as SizeName,
            ptp.CaTra,
            ptp.MaLo,
            ptp.MaMau,
            ptp.MaLoaiCa,
            ptp.MaThe,
            la.Ten as LoaiCaName,
            mau.Ten as MauName,
            case
                when @isDinhMucBinhThuong = 1 then(
                    case
                        when ptp.TrongLuongTra = 0 then 0
                        else floor(
                            1000 * ptp.TrongLuongNhan / ptp.TrongLuongTra
                        ) / 1000
                    end
                )
                else (
                    case
                        when(ptp.TrongLuongNhan) = 0 THEN 0
                        else (
                            ROUND(
                                ptp.TrongLuongTra / (ptp.TrongLuongNhan),
                                4
                            )
                        )
                    end
                )
            end as DinhMuc,
            ptp.TrongLuongTare,
            tp.DinhMuc as DinhMucChuan,
            ptp.TrongLuongNhan,
            ptp.TrongLuongTra,
            ptp.MaMayCan,
            ptp.MaXuong,
            xn.Ten,
            ptp.GhiChu
        from
            PhieuCanTPFilletv2 ptp,
            NhanVienDaiThanh n,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau,
            MaLoaiCaFillet la,
            XiNghiep xn
        where
            ptp.Ngay >= @fromDate
            and ptp.Ngay <= @toDate
            and ptp.MaXuong = @xuongId
            and ptp.MaNhanVien = n.MaNhanVien
            and ptp.MaNhanVien = @maNhanVien
            and ptp.MaThanhPham = tp.Ma
            and ptp.MaSize = s.Ma
            and ptp.MaMau = mau.Ma
            and ptp.MaLoaiCa = la.Ma
            and ptp.TrongLuongTra > 0
            and ptp.MaXuong = xn.Ma
    ) tp
    left join(
        Select
            *
        from
            (
                Select
                    d.*,
                    ROW_NUMBER() OVER(
                        PARTITION BY MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        MaThanhPham
                        ORDER BY
                            Gio DESC
                    ) AS [ROW NUMBER]
                from
                    DinhMucFillet d
                where
                    Ngay >= @fromDate
                    and Ngay <= @toDate
                    And MaXuong = @xuongId
            ) dm
        Where
            dm.[ROW NUMBER] = 1
    ) dm on tp.MaThanhPham = dm.MaThanhPham
    and tp.MaLo = dm.MaLo
    and tp.MaSize = dm.MaSize
    and tp.CaTra = dm.CaTra
    and tp.MaMau = dm.MaMau
    and tp.MaLoaiCa = dm.MaLoaiCa
    and tp.Ngay = dm.Ngay
    LEFT JOIN PhieuCanBTPFilletv2 pbtp on tp.Ngay = pbtp.Ngay
    and tp.STTBTP = pbtp.STT
    and tp.MaMayCanBTP = pbtp.MaMayCan)p
order by
    Gio";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query).Result.ToList();
            return items;
        }
    }

    public List<T> GetChiTiets2HNByMaNhanVien<T>(
        DateTime fromDate,
        DateTime toDate,
        string maNhanVien,
        string xuongId,
        bool isDinhMucBinhThuong = true)
    {
        var query = @$"DECLARE @fromDate as Date,
@toDate as DATE,
@xuongId as VARCHAR(50),
@maNhanVien as VARCHAR(50),
@isDinhMucBinhThuong as bit;

set
    @fromDate = '{fromDate.ToString("yyyy - MM - dd")}';

set
    @toDate = '{toDate.ToString("yyyy-MM-dd")}';

                set
                    @xuongId = '{xuongId}';

                set
                    @isDinhMucBinhThuong = {(isDinhMucBinhThuong ? "1" : "0")};
set @maNhanVien = '{maNhanVien}';
select (case
				when p.DanhGia = 1 then N'Đậu'
				else N'Rớt'
			end) as DanhGia2 ,p.* from 
(Select
    tp.STT,
    tp.Ngay,
    tp.Gio,
    cast(
        DATEDIFF(MINUTE, pbtp.Gio, tp.Gio) as decimal(18, 3)
    ) as ThoiGianHoanThanh,
    tp.MaNhanVien,
    tp.IsGiaCong,
    tp.MaHoSo,
    tp.TenNhanVien,
    --tp.MaNhanVienBanKiem,
    --nbk.Name as TenNhanVienBanKiem,
    --nbk.MaHoSo as MaNhanVienBanKiemHoSo,
    --tp.MaNhanVienPhucVu,
    --npv.Name as TenNhanVienPhucVu,
    --npv.MaHoSo as MaNhanVienPhucVuHoSo,
    tp.Nhom,
    tp.MaThanhPham,
    tp.Ten as TenXuong,
    tp.ThanhPhamName,
    tp.LoaiCaName,
    tp.MauName,
    Case
        when tp.SizeName = N'Cá Lớn' then 'L'
        when tp.SizeName = N'Cá Nhỏ' then 'N'
        else tp.SizeName
    end as SizeName,
    tp.MaSize,
    --case
    --    when tp.CaTra = 1 then 'True'
    --    when tp.CaTra = 0 then 'F'
    --    else ''
    --end as CaTra,
    tp.MaLo,
    tp.MaThe,
    case
        when @isDinhMucBinhThuong = 1 then(tp.DinhMuc)
        else (
            case
                when tp.DinhMuc > 1 THEN 1
                else (
                    tp.DinhMuc
                )
            end
        )
    end as DinhMuc,
    ISNULL(dm.DinhMuc, tp.DinhMucChuan) as DinhMucChuan,
    tp.TrongLuongNhan,
    tp.TrongLuongTra,
    tp.TrongLuongTare,

    cast(
        (
            Case
                when @isDinhMucBinhThuong = 1 then case
                    when tp.DinhMuc <= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                    else 0
                end
                else case
                    when tp.DinhMuc >= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                    else 0
                end
            end
        ) as bit
    ) as DanhGia,

    --0 as DonGia,
    --0 as ThanhTien,
    tp.MaMayCan,
    pbtp.Gio as GioBTP,
    pbtp.MaMayCan as MayCanBTP,
    tp.MaXuong,
    --tp.MaSanPham,
    tp.GhiChu
from
    (
        Select
            ptp.STT,
            ptp.Gio,
            ptp.STTBTP,
            ptp.MaMayCanBTP,
            ptp.Ngay,
            ptp.MaNhanVien,
            ptp.MaNhanVienPhucVu,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            n.IsGiaCong,
            tp.Ma as MaThanhPham,
            tp.Ten as ThanhPhamName,
            n.LoaiSanLuong,
            s.Ma as MaSize,
            s.Ten as SizeName,
            ptp.CaTra,
            ptp.MaLo,
            ptp.MaMau,
            ptp.MaLoaiCa,
            ptp.MaThe,
            la.Ten as LoaiCaName,
            mau.Ten as MauName,
            case
                when @isDinhMucBinhThuong = 1 then(
                    case
                        when ptp.TrongLuongTra = 0 then 0
                        else floor(
                            1000 * ptp.TrongLuongNhan / ptp.TrongLuongTra
                        ) / 1000
                    end
                )
                else (
                    case
                        when(ptp.TrongLuongNhan) = 0 THEN 0
                        else (
                            ROUND(
                                ptp.TrongLuongTra / (ptp.TrongLuongNhan),
                                4
                            )
                        )
                    end
                )
            end as DinhMuc,
            ptp.TrongLuongTare,
            tp.DinhMuc as DinhMucChuan,
            ptp.TrongLuongNhan,
            ptp.TrongLuongTra,
            ptp.MaMayCan,
            ptp.MaXuong,
            xn.Ten,
            ptp.GhiChu
        from
            PhieuCanTPFilletv2 ptp,
            NhanVienDaiThanh n,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau,
            MaLoaiCaFillet la,
            XiNghiep xn
        where
            ptp.Ngay >= @fromDate
            and ptp.Ngay <= @toDate
            and ptp.MaXuong = @xuongId
            and ptp.MaNhanVien = n.MaNhanVien
            and ptp.MaNhanVien = @maNhanVien
            and ptp.MaThanhPham = tp.Ma
            and ptp.MaSize = s.Ma
            and ptp.MaMau = mau.Ma
            and ptp.MaLoaiCa = la.Ma
            and ptp.TrongLuongTra > 0
            and ptp.MaXuong = xn.Ma
    ) tp
    left join(
        Select
            *
        from
            (
                Select
                    d.*,
                    ROW_NUMBER() OVER(
                        PARTITION BY MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        MaThanhPham
                        ORDER BY
                            Gio DESC
                    ) AS [ROW NUMBER]
                from
                    DinhMucFillet d
                where
                    Ngay >= @fromDate
                    and Ngay <= @toDate
                    And MaXuong = @xuongId
            ) dm
        Where
            dm.[ROW NUMBER] = 1
    ) dm on tp.MaThanhPham = dm.MaThanhPham
    and tp.MaLo = dm.MaLo
    and tp.MaSize = dm.MaSize
    and tp.CaTra = dm.CaTra
    and tp.MaMau = dm.MaMau
    and tp.MaLoaiCa = dm.MaLoaiCa
    and tp.Ngay = dm.Ngay
    LEFT JOIN PhieuCanBTPFilletv2 pbtp on tp.Ngay = pbtp.Ngay
    and tp.STTBTP = pbtp.STT
    and tp.MaMayCanBTP = pbtp.MaMayCan)p
order by
    Gio";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query).Result.ToList();
            return items;
        }
    }

    public List<T> GetChiTiets2HNByMaHoSo<T>(
        DateTime fromDate,
        DateTime toDate,
        string maHoSo,
        string xuongId,
        bool isDinhMucBinhThuong = true)
    {
        var query = @$"DECLARE @fromDate as Date,
@toDate as DATE,
@xuongId as VARCHAR(50),
@maNhanVien as VARCHAR(50),
@isDinhMucBinhThuong as bit;

set
    @fromDate = '{fromDate.ToString("yyyy - MM - dd")}';

set
    @toDate = '{toDate.ToString("yyyy-MM-dd")}';

                set
                    @xuongId = '{xuongId}';

                set
                    @isDinhMucBinhThuong = {(isDinhMucBinhThuong ? "1" : "0")};
set @maHoSo = '{maHoSo}';
select (case
				when p.DanhGia = 1 then N'Đậu'
				else N'Rớt'
			end) as DanhGia2 ,p.* from 
(Select
    tp.STT,
    tp.Ngay,
    tp.Gio,
    cast(
        DATEDIFF(MINUTE, pbtp.Gio, tp.Gio) as decimal(18, 3)
    ) as ThoiGianHoanThanh,
    tp.MaNhanVien,
    tp.IsGiaCong,
    tp.MaHoSo,
    tp.TenNhanVien,
    --tp.MaNhanVienBanKiem,
    --nbk.Name as TenNhanVienBanKiem,
    --nbk.MaHoSo as MaNhanVienBanKiemHoSo,
    --tp.MaNhanVienPhucVu,
    --npv.Name as TenNhanVienPhucVu,
    --npv.MaHoSo as MaNhanVienPhucVuHoSo,
    tp.Nhom,
    tp.MaThanhPham,
    tp.Ten as TenXuong,
    tp.ThanhPhamName,
    tp.LoaiCaName,
    tp.MauName,
    Case
        when tp.SizeName = N'Cá Lớn' then 'L'
        when tp.SizeName = N'Cá Nhỏ' then 'N'
        else tp.SizeName
    end as SizeName,
    tp.MaSize,
    --case
    --    when tp.CaTra = 1 then 'True'
    --    when tp.CaTra = 0 then 'F'
    --    else ''
    --end as CaTra,
    tp.MaLo,
    tp.MaThe,
    case
        when @isDinhMucBinhThuong = 1 then(tp.DinhMuc)
        else (
            case
                when tp.DinhMuc > 1 THEN 1
                else (
                    tp.DinhMuc
                )
            end
        )
    end as DinhMuc,
    ISNULL(dm.DinhMuc, tp.DinhMucChuan) as DinhMucChuan,
    tp.TrongLuongNhan,
    tp.TrongLuongTra,
    tp.TrongLuongTare,

    cast(
        (
            Case
                when @isDinhMucBinhThuong = 1 then case
                    when tp.DinhMuc <= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                    else 0
                end
                else case
                    when tp.DinhMuc >= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                    else 0
                end
            end
        ) as bit
    ) as DanhGia,

    --0 as DonGia,
    --0 as ThanhTien,
    tp.MaMayCan,
    pbtp.Gio as GioBTP,
    pbtp.MaMayCan as MayCanBTP,
    tp.MaXuong,
    --tp.MaSanPham,
    tp.GhiChu
from
    (
        Select
            ptp.STT,
            ptp.Gio,
            ptp.STTBTP,
            ptp.MaMayCanBTP,
            ptp.Ngay,
            ptp.MaNhanVien,
            ptp.MaNhanVienPhucVu,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            n.IsGiaCong,
            tp.Ma as MaThanhPham,
            tp.Ten as ThanhPhamName,
            n.LoaiSanLuong,
            s.Ma as MaSize,
            s.Ten as SizeName,
            ptp.CaTra,
            ptp.MaLo,
            ptp.MaMau,
            ptp.MaLoaiCa,
            ptp.MaThe,
            la.Ten as LoaiCaName,
            mau.Ten as MauName,
            case
                when @isDinhMucBinhThuong = 1 then(
                    case
                        when ptp.TrongLuongTra = 0 then 0
                        else floor(
                            1000 * ptp.TrongLuongNhan / ptp.TrongLuongTra
                        ) / 1000
                    end
                )
                else (
                    case
                        when(ptp.TrongLuongNhan) = 0 THEN 0
                        else (
                            ROUND(
                                ptp.TrongLuongTra / (ptp.TrongLuongNhan),
                                4
                            )
                        )
                    end
                )
            end as DinhMuc,
            ptp.TrongLuongTare,
            tp.DinhMuc as DinhMucChuan,
            ptp.TrongLuongNhan,
            ptp.TrongLuongTra,
            ptp.MaMayCan,
            ptp.MaXuong,
            xn.Ten,
            ptp.GhiChu
        from
            PhieuCanTPFilletv2 ptp,
            NhanVienDaiThanh n,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau,
            MaLoaiCaFillet la,
            XiNghiep xn
        where
            ptp.Ngay >= @fromDate
            and ptp.Ngay <= @toDate
            and ptp.MaXuong = @xuongId
            and ptp.MaNhanVien = n.MaNhanVien
            and n.MaHoSo = @maHoSo
            and ptp.MaThanhPham = tp.Ma
            and ptp.MaSize = s.Ma
            and ptp.MaMau = mau.Ma
            and ptp.MaLoaiCa = la.Ma
            and ptp.TrongLuongTra > 0
            and ptp.MaXuong = xn.Ma
    ) tp
    left join(
        Select
            *
        from
            (
                Select
                    d.*,
                    ROW_NUMBER() OVER(
                        PARTITION BY MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        MaThanhPham
                        ORDER BY
                            Gio DESC
                    ) AS [ROW NUMBER]
                from
                    DinhMucFillet d
                where
                    Ngay >= @fromDate
                    and Ngay <= @toDate
                    And MaXuong = @xuongId
            ) dm
        Where
            dm.[ROW NUMBER] = 1
    ) dm on tp.MaThanhPham = dm.MaThanhPham
    and tp.MaLo = dm.MaLo
    and tp.MaSize = dm.MaSize
    and tp.CaTra = dm.CaTra
    and tp.MaMau = dm.MaMau
    and tp.MaLoaiCa = dm.MaLoaiCa
    and tp.Ngay = dm.Ngay
    LEFT JOIN PhieuCanBTPFilletv2 pbtp on tp.Ngay = pbtp.Ngay
    and tp.STTBTP = pbtp.STT
    and tp.MaMayCanBTP = pbtp.MaMayCan)p
order by
    Gio";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query).Result.ToList();
            return items;
        }
    }

    public List<T> GetChiTiets2HNByMaThe<T>(
        DateTime fromDate,
        DateTime toDate,
        string maThe,
        string xuongId,
        bool isDinhMucBinhThuong = true)
    {
        var query = @$"DECLARE @fromDate as Date,
@toDate as DATE,
@xuongId as VARCHAR(50),
@maNhanVien as VARCHAR(50),
@isDinhMucBinhThuong as bit;

set
    @fromDate = '{fromDate.ToString("yyyy - MM - dd")}';

set
    @toDate = '{toDate.ToString("yyyy-MM-dd")}';

                set
                    @xuongId = '{xuongId}';

                set
                    @isDinhMucBinhThuong = {(isDinhMucBinhThuong ? "1" : "0")};
set @maThe = '{maThe}';
select (case
				when p.DanhGia = 1 then N'Đậu'
				else N'Rớt'
			end) as DanhGia2 ,p.* from 
(Select
    tp.STT,
    tp.Ngay,
    tp.Gio,
    cast(
        DATEDIFF(MINUTE, pbtp.Gio, tp.Gio) as decimal(18, 3)
    ) as ThoiGianHoanThanh,
    tp.MaNhanVien,
    tp.IsGiaCong,
    tp.MaHoSo,
    tp.TenNhanVien,
    --tp.MaNhanVienBanKiem,
    --nbk.Name as TenNhanVienBanKiem,
    --nbk.MaHoSo as MaNhanVienBanKiemHoSo,
    --tp.MaNhanVienPhucVu,
    --npv.Name as TenNhanVienPhucVu,
    --npv.MaHoSo as MaNhanVienPhucVuHoSo,
    tp.Nhom,
    tp.MaThanhPham,
    tp.Ten as TenXuong,
    tp.ThanhPhamName,
    tp.LoaiCaName,
    tp.MauName,
    Case
        when tp.SizeName = N'Cá Lớn' then 'L'
        when tp.SizeName = N'Cá Nhỏ' then 'N'
        else tp.SizeName
    end as SizeName,
    tp.MaSize,
    --case
    --    when tp.CaTra = 1 then 'True'
    --    when tp.CaTra = 0 then 'F'
    --    else ''
    --end as CaTra,
    tp.MaLo,
    tp.MaThe,
    case
        when @isDinhMucBinhThuong = 1 then(tp.DinhMuc)
        else (
            case
                when tp.DinhMuc > 1 THEN 1
                else (
                    tp.DinhMuc
                )
            end
        )
    end as DinhMuc,
    ISNULL(dm.DinhMuc, tp.DinhMucChuan) as DinhMucChuan,
    tp.TrongLuongNhan,
    tp.TrongLuongTra,
    tp.TrongLuongTare,

    cast(
        (
            Case
                when @isDinhMucBinhThuong = 1 then case
                    when tp.DinhMuc <= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                    else 0
                end
                else case
                    when tp.DinhMuc >= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                    else 0
                end
            end
        ) as bit
    ) as DanhGia,

    --0 as DonGia,
    --0 as ThanhTien,
    tp.MaMayCan,
    pbtp.Gio as GioBTP,
    pbtp.MaMayCan as MayCanBTP,
    tp.MaXuong,
    --tp.MaSanPham,
    tp.GhiChu
from
    (
        Select
            ptp.STT,
            ptp.Gio,
            ptp.STTBTP,
            ptp.MaMayCanBTP,
            ptp.Ngay,
            ptp.MaNhanVien,
            ptp.MaNhanVienPhucVu,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            n.IsGiaCong,
            tp.Ma as MaThanhPham,
            tp.Ten as ThanhPhamName,
            n.LoaiSanLuong,
            s.Ma as MaSize,
            s.Ten as SizeName,
            ptp.CaTra,
            ptp.MaLo,
            ptp.MaMau,
            ptp.MaLoaiCa,
            ptp.MaThe,
            la.Ten as LoaiCaName,
            mau.Ten as MauName,
            case
                when @isDinhMucBinhThuong = 1 then(
                    case
                        when ptp.TrongLuongTra = 0 then 0
                        else floor(
                            1000 * ptp.TrongLuongNhan / ptp.TrongLuongTra
                        ) / 1000
                    end
                )
                else (
                    case
                        when(ptp.TrongLuongNhan) = 0 THEN 0
                        else (
                            ROUND(
                                ptp.TrongLuongTra / (ptp.TrongLuongNhan),
                                4
                            )
                        )
                    end
                )
            end as DinhMuc,
            ptp.TrongLuongTare,
            tp.DinhMuc as DinhMucChuan,
            ptp.TrongLuongNhan,
            ptp.TrongLuongTra,
            ptp.MaMayCan,
            ptp.MaXuong,
            xn.Ten,
            ptp.GhiChu
        from
            PhieuCanTPFilletv2 ptp,
            NhanVienDaiThanh n,
            MaThanhPhamFillet tp,
            MaSizeFillet s,
            MaMauFillet mau,
            MaLoaiCaFillet la,
            XiNghiep xn,
            TheTu t
        where
            ptp.Ngay >= @fromDate
            and ptp.Ngay <= @toDate
            and ptp.MaXuong = @xuongId
            and ptp.MaNhanVien = n.MaNhanVien
            and ptp.MaNhanVien = t.MaNhanVien
			and t.MaTheTu = @maThe
            and ptp.MaThanhPham = tp.Ma
            and ptp.MaSize = s.Ma
            and ptp.MaMau = mau.Ma
            and ptp.MaLoaiCa = la.Ma
            and ptp.TrongLuongTra > 0
            and ptp.MaXuong = xn.Ma
    ) tp
    left join(
        Select
            *
        from
            (
                Select
                    d.*,
                    ROW_NUMBER() OVER(
                        PARTITION BY MaLo,
                        MaLoaiCa,
                        MaMau,
                        MaSize,
                        MaThanhPham
                        ORDER BY
                            Gio DESC
                    ) AS [ROW NUMBER]
                from
                    DinhMucFillet d
                where
                    Ngay >= @fromDate
                    and Ngay <= @toDate
                    And MaXuong = @xuongId
            ) dm
        Where
            dm.[ROW NUMBER] = 1
    ) dm on tp.MaThanhPham = dm.MaThanhPham
    and tp.MaLo = dm.MaLo
    and tp.MaSize = dm.MaSize
    and tp.CaTra = dm.CaTra
    and tp.MaMau = dm.MaMau
    and tp.MaLoaiCa = dm.MaLoaiCa
    and tp.Ngay = dm.Ngay
    LEFT JOIN PhieuCanBTPFilletv2 pbtp on tp.Ngay = pbtp.Ngay
    and tp.STTBTP = pbtp.STT
    and tp.MaMayCanBTP = pbtp.MaMayCan)p
order by
    Gio";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query).Result.ToList();
            return items;
        }
    }

    public List<T> GetTongHopLoaiThanhPhamsHN<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = false,
        bool isFloor = true)
    {
        var query = @"Select
    p.Ngay,
    p.MaLoaiCa,
    la.Ten as LoaiCaName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    p.MaMau,
    ma.Ten as MauName,
    p.CaTra,
    Sum(p.TrongLuongNhan*p.LoaiSanLuong) as TrongLuongNhan,
    Sum(p.TrongLuongTra*p.LoaiSanLuong) as TrongLuongTra,
    case
        when @isDinhMucBinhThuong = 1 then cast (
            case
                when sum(p.TrongLuongTra) = 0 then 0
                else (
                    case
                        when @isFloor = 1 then (
                            FLOOR(
                                (Sum(p.TrongLuongNhan) / sum(p.TrongLuongTra)) * 1000
                            ) / 1000
                        )
                        else Sum(p.TrongLuongNhan) / sum(p.TrongLuongTra)
                    end
                )
            end as decimal(18, 3)
        )
        ELSE cast (
            case
                when sum(p.TrongLuongNhan) = 0 then 0
                else sum(p.TrongLuongTra) / sum(p.TrongLuongNhan)
            end as decimal(18, 4)
        )
    end as DinhMuc,
    DinhMuc.DinhMuc as DinhMucChuan,
    Count(*) as SoRo
from
    (
        Select
            p.*,
            n.LoaiSanLuong
        from
            PhieuCanTPFilletv2 p,
            NhanVienDaiThanh n
        where
            p.Ngay <= @toDate
            and p.Ngay >= @fromDate
            and MaXuong = @xuongId
            and p.manhanvien = n.manhanvien
    ) p
    LEFT JOIN (
        Select
            tp1.MaLo,
            tp1.MaLoaiCa,
            tp1.MaSize,
            tp1.MaMau,
            CASE
                WHEN tp1.CaTra = 1
                and @isCaTraChuyenDoi = 1 THEN 'B'
                ELSE tp1.MaThanhPham
            END as MaThanhPham,
            tp1.CaTra,
            CASE
                WHEN tp2.DinhMuc is Null THEN tp1.DinhMuc
                ELSE tp2.DinhMuc
            END AS DinhMuc,
            tp1.Ngay,
            tp1.MaXuong
        from
            (
                Select
                    distinct p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaMau,
                    p.MaThanhPham,
                    p.CaTra,
                    CASE
                        WHEN p.CaTra = 1 THEN 1.37
                        ELSE tp.DinhMuc
                    END AS DinhMuc,
                    p.Ngay,
                    p.MaXuong
                from
                    PhieuCanTPFilletv2 p,
                    MaThanhPhamFillet tp
                where
                    p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and MaXuong = @xuongId
                    and p.MaThanhPham = tp.Ma
                    and p.MaLoaiCa = tp.MaCa
            ) tp1
            LEFT JOIN (
                Select
                    MaLo,
                    MaLoaiCa,
                    MaSize,
                    MaMau,
                    MaThanhPham,
                    CaTra,
                    DinhMuc,
                    Ngay,
                    MaXuong
                from
                    (
                        Select
                            d.*,
                            ROW_NUMBER() OVER (
                                PARTITION BY MaLo,
                                MaLoaiCa,
                                MaMau,
                                MaSize,
                                MaThanhPham,
                                CaTra,
                                Ngay
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            DinhMucFillet d
                        where
                            d.Ngay <= @toDate
                            and d.Ngay >= @fromDate
                            And MaXuong = @xuongId
                    ) dm
                Where
                    dm.[ROW NUMBER] = 1
            ) tp2 on tp1.MaLo = tp2.MaLo
            and tp1.MaLoaiCa = tp2.MaLoaiCa
            and tp1.MaSize = tp2.MaSize
            and tp1.MaMau = tp2.MaMau
            and tp1.MaThanhPham = tp2.MaThanhPham
            and tp1.CaTra = tp2.CaTra
            and tp1.Ngay = tp2.Ngay
            and tp1.MaXuong = tp2.MaXuong
    ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
    and p.MaLo = DinhMuc.MaLo
    and p.MaLoaiCa = DinhMuc.MaLoaiCa
    and p.MaSize = DinhMuc.MaSize
    and p.MaMau = DinhMuc.MaMau
    and p.MaThanhPham = DinhMuc.MaThanhPham
    And p.CaTra = DinhMuc.CaTra
    and p.Ngay = DinhMuc.Ngay
    LEFT JOIN MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
    LEFT Join MaSizeFillet s on p.MaSize = s.Ma
    LEFT Join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
    LEFT Join MaMauFillet ma on p.MaMau = ma.Ma
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
where
    p.STT > 0
    and p.Ngay <= @toDate
    and p.Ngay >= @fromDate and tp.IsNguyenCon =0
group by
    p.MaLoaiCa,
    p.MaSize,
    p.MaThanhPham,
    p.MaMau,
    p.CaTra,
    DinhMuc.DinhMuc,
    p.Ngay,
    la.Ten,
    tp.Ten,
    ma.Ten,
    s.Ten,
    p.LoaiSanLuong";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate, toDate, xuongId, isDinhMucBinhThuong, isCaTraChuyenDoi, isFloor })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopNhanViensHN<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isFloor = true)
    {
//            var query = @"
//Select pbtp.*,
//    ptp.KhongPhanBietSize,
//    ptp.DinhMucChuan,
//    ptp.TongDauNhan,
//    ptp.TongDauTra,
//    ptp.TongRotNhan,
//    ptp.TongRotTra,
//    ptp.TongSoDauTra,
//    ptp.TongSoRotTra,
//    ptp.TongNhan,
//    ptp.TongTra,
//    ptp.TongSoTra,
//    ptp.DinhMucDau,
//    ptp.DinhMucRot,
//    ptp.DinhMucThucTe
//from (
//        select p.*,
//            n.Name as NhanVienName,
//            n.MaHoSo,
//            n.DeptName0 as Nhom,
//            n.IsGiaCong,
//            tp.Ten as ThanhPhamName,
//            s.Ten as SizeName,
//            la.Ten as LoaiCaName,
//            mau.Ten as MauName,
//            xn.Ten as TenXuong
//        from (
//                Select p.Ngay,
//                    p.MaNhanVien,
//                    p.MaThanhPham,
//                    p.MaSize,
//                    p.MaLoaiCa,
//                    p.MaMau,
//                    p.MaLo,
//                    p.MaXuong,
//                    sum(
//                        case
//                            when IsNull(p.GhiChu, '') <> 'HUY' then 1
//                            else 0
//                        end
//                    ) as SoRoBTP,
//                    sum(
//                        case
//                            when IsNull(p.GhiChu, '') = 'HUY' then 1
//                            else 0
//                        end
//                    ) as SoRoHuy,
//                    sum(
//                        case
//                            when p.IsEnabled = 0 then 1
//                            else 0
//                        end
//                    ) as SoRoChuaCanTP
//                from PhieuCanBTPFilletv2 p
//                where p.Ngay >= @fromDate
//                    and p.Ngay <= @toDate
//                    and p.MaXuong = @xuongId
//                    and p.STT>0
//                GROUP BY p.Ngay,
//                    p.MaNhanVien,
//                    p.MaThanhPham,
//                    p.MaSize,
//                    p.MaLoaiCa,
//                    p.MaMau,
//                    p.MaLo,
//                    p.MaXuong
//            ) p
//            LEFT Join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
//            LEFT join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
//            LEFT join MaSizeFillet s on p.MaSize = s.Ma
//            LEFT join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
//            LEFT join MaMauFillet mau on p.MaMau = mau.Ma
//            left join XiNghiep xn on p.MaXuong = xn.Ma
//    ) pbtp
//    LEFT JOIN (
//        Select p.*,
//            p.TongDauNhan + p.TongRotNhan as TongNhan,
//            p.TongDauTra + p.TongRotTra as TongTra,
//            p.TongSoDauTra + p.TongSoRotTra as TongSoTra,
//            cast(
//                case
//                    when p.TongDauTra = 0 then 0
//                    else p.TongDauNhan / p.TongDauTra
//                end as decimal(18, 2)
//            ) as DinhMucDau,
//            cast(
//                case
//                    when p.TongRotTra = 0 then 0
//                    else p.TongRotNhan / p.TongRotTra
//                end as decimal(18, 2)
//            ) as DinhMucRot,
//            cast(
//                case
//                    when p.TongRotTra + p.TongDauTra = 0 then 0
//                    else (p.TongRotNhan + p.TongDauNhan) / (p.TongRotTra + p.TongDauTra)
//                end as decimal(18, 2)
//            ) as DinhMucThucTe
//        from (
//                Select p.Ngay,
//                    p.MaNhanVien,
//                    p.IsGiaCong,
//                    p.NhanVienName,
//                    p.MaHoSo,
//                    p.Nhom,
//                    p.MaThanhPham,
//                    p.TenXuong,
//                    p.ThanhPhamName,
//                    p.KhongPhanBietSize,
//                    p.LoaiCaName,
//                    p.MaLoaiCa,
//                    p.MaMau,
//                    p.MauName,
//                    p.SizeName,
//                    p.MaSize,
//                    p.MaLo,
//                    p.DinhMucChuan,
//                    p.MaXuong,
//                    sum(
//                        case
//                            when p.DanhGia = 1 then p.TrongLuongNhan
//                            else 0
//                        end
//                    ) as TongDauNhan,
//                    sum(
//                        case
//                            when p.DanhGia = 1 then p.TrongLuongTra
//                            else 0
//                        end
//                    ) as TongDauTra,
//                    sum(
//                        case
//                            when p.DanhGia = 0 then p.TrongLuongNhan
//                            else 0
//                        end
//                    ) as TongRotNhan,
//                    sum(
//                        case
//                            when p.DanhGia = 0 then p.TrongLuongTra
//                            else 0
//                        end
//                    ) as TongRotTra,
//                    sum(
//                        case
//                            when p.DanhGia = 1 then 1
//                            else 0
//                        end
//                    ) as TongSoDauTra,
//                    sum(
//                        case
//                            when p.DanhGia = 0 then 1
//                            else 0
//                        end
//                    ) as TongSoRotTra
//                from (
//                        select (
//                                case
//                                    when p.DanhGia = 1 then N'Đậu'
//                                    else N'Rớt'
//                                end
//                            ) as DanhGia2,
//                            p.*
//                        from (
//                                Select tp.STT,
//                                    tp.Ngay,
//                                    tp.Gio,
//                                    cast(
//                                        DATEDIFF(MINUTE, pbtp.Gio, tp.Gio) as decimal(18, 3)
//                                    ) as ThoiGianHoanThanh,
//                                    tp.MaNhanVien,
//                                    tp.IsGiaCong,
//                                    tp.MaHoSo,
//                                    tp.NhanVienName,
//                                    tp.Nhom,
//                                    tp.MaThanhPham,
//                                    tp.Ten as TenXuong,
//                                    tp.ThanhPhamName,
//                                    tp.KhongPhanBietSize,
//                                    tp.LoaiCaName,
//                                    tp.MaLoaiCa,
//                                    tp.MaMau,
//                                    tp.MauName,
//                                    tp.SizeName,
//                                    tp.MaSize,
//                                    tp.MaLo,
//                                    case
//                                        when @isDinhMucBinhThuong = 1 then(tp.DinhMuc)
//                                        else (
//                                            case
//                                                when tp.DinhMuc > 1 THEN 1
//                                                else (
//                                                    tp.DinhMuc
//                                                )
//                                            end
//                                        )
//                                    end as DinhMuc,
//                                    ISNULL(dm.DinhMuc, tp.DinhMucChuan) as DinhMucChuan,
//                                    tp.TrongLuongNhan,
//                                    tp.TrongLuongTra,
//                                    tp.TrongLuongTare,
//                                    cast(
//                                        (
//                                            Case
//                                                when @isDinhMucBinhThuong = 1 then case
//                                                    when tp.DinhMuc <= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
//                                                    else 0
//                                                end
//                                                else case
//                                                    when tp.DinhMuc >= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
//                                                    else 0
//                                                end
//                                            end
//                                        ) as bit
//                                    ) as DanhGia,
//                                    --0 as DonGia,
//                                    --0 as ThanhTien,
//                                    tp.MaMayCan,
//                                    pbtp.Gio as GioBTP,
//                                    pbtp.MaMayCan as MayCanBTP,
//                                    tp.MaXuong,
//                                    --tp.MaSanPham,
//                                    tp.GhiChu
//                                from (
//                                        Select ptp.STT,
//                                            ptp.Gio,
//                                            ptp.STTBTP,
//                                            ptp.MaMayCanBTP,
//                                            ptp.Ngay,
//                                            ptp.MaNhanVien,
//                                            ptp.MaNhanVienPhucVu,
//                                            n.MaHoSo,
//                                            n.Name as NhanVienName,
//                                            n.DeptName0 as Nhom,
//                                            n.IsGiaCong,
//                                            tp.Ma as MaThanhPham,
//                                            tp.Ten as ThanhPhamName,
//                                            n.LoaiSanLuong,
//                                            s.Ma as MaSize,
//                                            s.Ten as SizeName,
//                                            ptp.CaTra,
//                                            ptp.MaLo,
//                                            ptp.MaMau,
//                                            ptp.MaLoaiCa,
//                                            la.Ten as LoaiCaName,
//                                            mau.Ten as MauName,
//                                            case
//                                                when @isDinhMucBinhThuong = 1 then(
//                                                    case
//                                                        when ptp.TrongLuongTra = 0 then 0
//                                                        else floor(
//                                                            1000 * ptp.TrongLuongNhan / ptp.TrongLuongTra
//                                                        ) / 1000
//                                                    end
//                                                )
//                                                else (
//                                                    case
//                                                        when(ptp.TrongLuongNhan) = 0 THEN 0
//                                                        else (
//                                                            ROUND(
//                                                                ptp.TrongLuongTra / (ptp.TrongLuongNhan),
//                                                                4
//                                                            )
//                                                        )
//                                                    end
//                                                )
//                                            end as DinhMuc,
//                                            ptp.TrongLuongTare,
//                                            tp.DinhMuc as DinhMucChuan,
//                                            ptp.TrongLuongNhan,
//                                            ptp.TrongLuongTra,
//                                            ptp.MaMayCan,
//                                            ptp.MaXuong,
//                                            xn.Ten,
//                                            ptp.GhiChu,
//                                            tp.KhongPhanBietSize
//                                        from PhieuCanTPFilletv2 ptp,
//                                            NhanVienDaiThanh n,
//                                            MaThanhPhamFillet tp,
//                                            MaSizeFillet s,
//                                            MaMauFillet mau,
//                                            MaLoaiCaFillet la,
//                                            XiNghiep xn
//                                        where ptp.Ngay >= @fromDate
//                                            and ptp.Ngay <= @toDate
//                                            and ptp.MaXuong = @xuongId
//                                            and ptp.MaNhanVien = n.MaNhanVien
//                                            and ptp.MaThanhPham = tp.Ma
//                                            and ptp.MaSize = s.Ma
//                                            and ptp.MaMau = mau.Ma
//                                            and ptp.MaLoaiCa = la.Ma
//                                            and ptp.TrongLuongTra > 0
//                                            and ptp.MaXuong = xn.Ma
//                                            and ptp.STT>0
//                                    ) tp
//                                    left join(
//                                        Select *
//                                        from (
//                                                Select d.*,
//                                                    ROW_NUMBER() OVER(
//                                                        PARTITION BY MaLo,
//                                                        MaLoaiCa,
//                                                        MaMau,
//                                                        MaSize,
//                                                        MaThanhPham
//                                                        ORDER BY Gio DESC
//                                                    ) AS [ROW NUMBER]
//                                                from DinhMucFillet d
//                                                where Ngay >= @fromDate
//                                                    and Ngay <= @toDate
//                                                    And MaXuong = @xuongId
//                                            ) dm
//                                        Where dm.[ROW NUMBER] = 1
//                                    ) dm on tp.MaThanhPham = dm.MaThanhPham
//                                    and tp.MaLo = dm.MaLo
//                                    and tp.MaSize = dm.MaSize
//                                    and tp.CaTra = dm.CaTra
//                                    and tp.MaMau = dm.MaMau
//                                    and tp.MaLoaiCa = dm.MaLoaiCa
//                                    and tp.Ngay = dm.Ngay
//                                    LEFT JOIN PhieuCanBTPFilletv2 pbtp on tp.Ngay = pbtp.Ngay
//                                    and tp.STTBTP = pbtp.STT
//                                    and tp.MaMayCanBTP = pbtp.MaMayCan
//                            ) p
//                    ) p
//                GROUP BY p.Ngay,
//                    p.MaNhanVien,
//                    p.IsGiaCong,
//                    p.NhanVienName,
//                    p.MaHoSo,
//                    p.Nhom,
//                    p.MaThanhPham,
//                    p.TenXuong,
//                    p.ThanhPhamName,
//                    p.LoaiCaName,
//                    p.MauName,
//                    p.SizeName,
//                    p.MaSize,
//                    p.MaLo,
//                    p.DinhMucChuan,
//                    p.MaXuong,
//                    p.KhongPhanBietSize,
//                    p.MaLoaiCa,
//                    p.MaMau
//            ) p
//    ) ptp on pbtp.Ngay = ptp.Ngay
//    and pbtp.MaNhanVien = ptp.MaNhanVien
//    and pbtp.MaThanhPham = ptp.MaThanhPham
//    and pbtp.MaSize = ptp.MaSize
//    and pbtp.MaLoaiCa = pbtp.MaLoaiCa
//    and pbtp.MaMau = pbtp.MaMau
//    and pbtp.MaLo = pbtp.MaLo
//    and pbtp.MaXuong = ptp.MaXuong
//order by pbtp.Ngay,
//    pbtp.MaXuong,
//    pbtp.Nhom,
//    pbtp.MaNhanVien,
//    pbtp.MaThanhPham";
//thêm checkinout để lấy thời gian vào ra
        var query = @"
;WITH CheckInOutData AS (
    SELECT 
        c.MaChamCong,
        MIN(c.ThoiGian) AS ThoiGianVao,
        MAX(c.ThoiGian) AS ThoiGianRa
    FROM CheckInOut c 
    WHERE c.ThoiGian >= @fromDate AND c.ThoiGian <= @toDate
    GROUP BY c.MaChamCong
)

Select pbtp.*,
    ptp.KhongPhanBietSize,
    ptp.DinhMucChuan,
    ptp.TongDauNhan,
    ptp.TongDauTra,
    ptp.TongRotNhan,
    ptp.TongRotTra,
    ptp.TongSoDauTra,
    ptp.TongSoRotTra,
    ptp.TongNhan,
    ptp.TongTra,
    ptp.TongSoTra,
    ptp.DinhMucDau,
    ptp.DinhMucRot,
    ptp.DinhMucThucTe,
    ptp.MaNhanVien,
	n.Name as NhanVienName,
    n.MaHoSo,
    n.DeptName0 as Nhom,
    n.IsGiaCong,
	ISNULL(c.ThoiGianVao, '1900-01-01') AS ThoiGianVao,
	ISNULL(c.ThoiGianRa, '1900-01-01') AS ThoiGianRa,
	DATEDIFF(hour, ISNULL(c.ThoiGianVao, '1900-01-01'), ISNULL(c.ThoiGianRa, '1900-01-01')) AS TongThoiGian
from (
        select p.*,
            --n.Name as NhanVienName,
            --n.MaHoSo,
            --n.DeptName0 as Nhom,
            --n.IsGiaCong,
            tp.Ten as ThanhPhamName,
            s.Ten as SizeName,
            la.Ten as LoaiCaName,
            mau.Ten as MauName,
            xn.Ten as TenXuong
			
        from (
                Select p.Ngay,
                    --p.MaNhanVien,
                    p.MaThanhPham,
                    p.MaSize,
                    p.MaLoaiCa,
                    p.MaMau,
                    p.MaLo,
                    p.MaXuong,
                    sum(
                        case
                            when IsNull(p.GhiChu, '') <> 'HUY' then 1
                            else 0
                        end
                    ) as SoRoBTP,
                    sum(
                        case
                            when IsNull(p.GhiChu, '') = 'HUY' then 1
                            else 0
                        end
                    ) as SoRoHuy,
                    sum(
                        case
                            when p.IsEnabled = 0 then 1
                            else 0
                        end
                    ) as SoRoChuaCanTP
                from PhieuCanBTPFilletv2 p
                where p.Ngay >= @fromDate
                    and p.Ngay <= @toDate
                    and p.MaXuong = @xuongId
                    and p.STT>0
                GROUP BY p.Ngay,
                    --p.MaNhanVien,
                    p.MaThanhPham,
                    p.MaSize,
                    p.MaLoaiCa,
                    p.MaMau,
                    p.MaLo,
                    p.MaXuong
            ) p
            --LEFT Join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            LEFT join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
            LEFT join MaSizeFillet s on p.MaSize = s.Ma
            LEFT join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
            LEFT join MaMauFillet mau on p.MaMau = mau.Ma
            left join XiNghiep xn on p.MaXuong = xn.Ma
			--left join CheckInOutData c on n.MaChamCong = c.MaChamCong 
    ) pbtp
    LEFT JOIN (
        Select p.*,
            p.TongDauNhan + p.TongRotNhan as TongNhan,
            p.TongDauTra + p.TongRotTra as TongTra,
            p.TongSoDauTra + p.TongSoRotTra as TongSoTra,
            cast(
                case
                    when p.TongDauTra = 0 then 0
                    else p.TongDauNhan / p.TongDauTra
                end as decimal(18, 2)
            ) as DinhMucDau,
            cast(
                case
                    when p.TongRotTra = 0 then 0
                    else p.TongRotNhan / p.TongRotTra
                end as decimal(18, 2)
            ) as DinhMucRot,
            cast(
                case
                    when p.TongRotTra + p.TongDauTra = 0 then 0
                    else (p.TongRotNhan + p.TongDauNhan) / (p.TongRotTra + p.TongDauTra)
                end as decimal(18, 2)
            ) as DinhMucThucTe
        from (
                Select p.Ngay,
                    p.MaNhanVien,
                    p.IsGiaCong,
                    p.NhanVienName,
                    p.MaHoSo,
                    p.Nhom,
                    p.MaThanhPham,
                    p.TenXuong,
                    p.ThanhPhamName,
                    p.KhongPhanBietSize,
                    p.LoaiCaName,
                    p.MaLoaiCa,
                    p.MaMau,
                    p.MauName,
                    p.SizeName,
                    p.MaSize,
                    p.MaLo,
                    p.DinhMucChuan,
                    p.MaXuong,
                    sum(
                        case
                            when p.DanhGia = 1 then p.TrongLuongNhan
                            else 0
                        end
                    ) as TongDauNhan,
                    sum(
                        case
                            when p.DanhGia = 1 then p.TrongLuongTra
                            else 0
                        end
                    ) as TongDauTra,
                    sum(
                        case
                            when p.DanhGia = 0 then p.TrongLuongNhan
                            else 0
                        end
                    ) as TongRotNhan,
                    sum(
                        case
                            when p.DanhGia = 0 then p.TrongLuongTra
                            else 0
                        end
                    ) as TongRotTra,
                    sum(
                        case
                            when p.DanhGia = 1 then 1
                            else 0
                        end
                    ) as TongSoDauTra,
                    sum(
                        case
                            when p.DanhGia = 0 then 1
                            else 0
                        end
                    ) as TongSoRotTra
                from (
                        select (
                                case
                                    when p.DanhGia = 1 then N'Đậu'
                                    else N'Rớt'
                                end
                            ) as DanhGia2,
                            p.*
                        from (
                                Select tp.STT,
                                    tp.Ngay,
                                    tp.Gio,
                                    cast(
                                        DATEDIFF(MINUTE, pbtp.Gio, tp.Gio) as decimal(18, 3)
                                    ) as ThoiGianHoanThanh,
                                    tp.MaNhanVien,
                                    tp.IsGiaCong,
                                    tp.MaHoSo,
                                    tp.NhanVienName,
                                    tp.Nhom,
                                    tp.MaThanhPham,
                                    tp.Ten as TenXuong,
                                    tp.ThanhPhamName,
                                    tp.KhongPhanBietSize,
                                    tp.LoaiCaName,
                                    tp.MaLoaiCa,
                                    tp.MaMau,
                                    tp.MauName,
                                    tp.SizeName,
                                    tp.MaSize,
                                    tp.MaLo,
                                    case
                                        when @isDinhMucBinhThuong = 1 then(tp.DinhMuc)
                                        else (
                                            case
                                                when tp.DinhMuc > 1 THEN 1
                                                else (
                                                    tp.DinhMuc
                                                )
                                            end
                                        )
                                    end as DinhMuc,
                                    ISNULL(dm.DinhMuc, tp.DinhMucChuan) as DinhMucChuan,
                                    tp.TrongLuongNhan,
                                    tp.TrongLuongTra,
                                    tp.TrongLuongTare,
                                    cast(
                                        (
                                            Case
                                                when @isDinhMucBinhThuong = 1 then case
                                                    when tp.DinhMuc <= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                                                    else 0
                                                end
                                                else case
                                                    when tp.DinhMuc >= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                                                    else 0
                                                end
                                            end
                                        ) as bit
                                    ) as DanhGia,
                                    --0 as DonGia,
                                    --0 as ThanhTien,
                                    tp.MaMayCan,
                                    pbtp.Gio as GioBTP,
                                    pbtp.MaMayCan as MayCanBTP,
                                    tp.MaXuong,
                                    --tp.MaSanPham,
                                    tp.GhiChu
                                from (
                                        Select ptp.STT,
                                            ptp.Gio,
                                            ptp.STTBTP,
                                            ptp.MaMayCanBTP,
                                            ptp.Ngay,
                                            ptp.MaNhanVien,
                                            ptp.MaNhanVienPhucVu,
                                            n.MaHoSo,
                                            n.Name as NhanVienName,
                                            n.DeptName0 as Nhom,
                                            n.IsGiaCong,
                                            tp.Ma as MaThanhPham,
                                            tp.Ten as ThanhPhamName,
                                            n.LoaiSanLuong,
                                            s.Ma as MaSize,
                                            s.Ten as SizeName,
                                            ptp.MaLo,
                                            ptp.MaMau,
                                            ptp.MaLoaiCa,
                                            la.Ten as LoaiCaName,
                                            mau.Ten as MauName,
                                            case
                                                when @isDinhMucBinhThuong = 1 then(
                                                    case
                                                        when ptp.TrongLuongTra = 0 then 0
                                                        else floor(
                                                            1000 * ptp.TrongLuongNhan / ptp.TrongLuongTra
                                                        ) / 1000
                                                    end
                                                )
                                                else (
                                                    case
                                                        when(ptp.TrongLuongNhan) = 0 THEN 0
                                                        else (
                                                            ROUND(
                                                                ptp.TrongLuongTra / (ptp.TrongLuongNhan),
                                                                4
                                                            )
                                                        )
                                                    end
                                                )
                                            end as DinhMuc,
                                            ptp.TrongLuongTare,
                                            tp.DinhMuc as DinhMucChuan,
                                            ptp.TrongLuongNhan,
                                            ptp.TrongLuongTra,
                                            ptp.MaMayCan,
                                            ptp.MaXuong,
                                            xn.Ten,
                                            ptp.GhiChu,
                                            tp.KhongPhanBietSize
                                        from PhieuCanTPFilletv2 ptp
											left join NhanVienDaiThanh n on ptp.MaNhanVien = n.MaNhanVien
                                            left join MaThanhPhamFillet tp on ptp.MaThanhPham = tp.Ma
                                            left join MaSizeFillet s on ptp.MaSize = s.Ma
                                            left join MaMauFillet mau on ptp.MaMau = mau.Ma
                                            left join MaLoaiCaFillet la on ptp.MaLoaiCa = la.Ma
                                            left join XiNghiep xn on ptp.MaXuong = xn.Ma
                                        where ptp.Ngay >= @fromDate
                                            and ptp.Ngay <= @toDate
                                            and ptp.MaXuong = @xuongId
                                            and ptp.TrongLuongTra > 0
                                            and ptp.STT>0
                                    ) tp
                                    left join(
                                        Select *
                                        from (
                                                Select d.*,
                                                    ROW_NUMBER() OVER(
                                                        PARTITION BY MaLo,
                                                        MaLoaiCa,
                                                        MaMau,
                                                        MaSize,
                                                        MaThanhPham
                                                        ORDER BY Gio DESC
                                                    ) AS [ROW NUMBER]
                                                from DinhMucFillet d
                                                where Ngay >= @fromDate
                                                    and Ngay <= @toDate
                                                    And MaXuong = @xuongId
                                            ) dm
                                        Where dm.[ROW NUMBER] = 1
                                    ) dm on tp.MaThanhPham = dm.MaThanhPham
                                    and tp.MaLo = dm.MaLo
                                    and tp.MaSize = dm.MaSize
                                    and tp.MaMau = dm.MaMau
                                    and tp.MaLoaiCa = dm.MaLoaiCa
                                    and tp.Ngay = dm.Ngay
                                    LEFT JOIN PhieuCanBTPFilletv2 pbtp on tp.Ngay = pbtp.Ngay
                                    and tp.STTBTP = pbtp.STT
                                    and tp.MaMayCanBTP = pbtp.MaMayCan
                            ) p
                    ) p
                GROUP BY p.Ngay,
                    p.MaNhanVien,
                    p.IsGiaCong,
                    p.NhanVienName,
                    p.MaHoSo,
                    p.Nhom,
                    p.MaThanhPham,
                    p.TenXuong,
                    p.ThanhPhamName,
                    p.LoaiCaName,
                    p.MauName,
                    p.SizeName,
                    p.MaSize,
                    p.MaLo,
                    p.DinhMucChuan,
                    p.MaXuong,
                    p.KhongPhanBietSize,
                    p.MaLoaiCa,
                    p.MaMau
            ) p
    ) ptp on pbtp.Ngay = ptp.Ngay
    --and pbtp.MaNhanVien = ptp.MaNhanVien
    and pbtp.MaThanhPham = ptp.MaThanhPham
    and pbtp.MaSize = ptp.MaSize
    and pbtp.MaLoaiCa = pbtp.MaLoaiCa
    and pbtp.MaMau = pbtp.MaMau
    and pbtp.MaLo = pbtp.MaLo
    and pbtp.MaXuong = ptp.MaXuong
	LEFT JOIN NhanVienDaiThanh n on ptp.MaNhanVien = n.MaNhanVien
	left join CheckInOutData c on n.MaChamCong = c.MaChamCong
order by pbtp.Ngay,
    pbtp.MaXuong,
    --pbtp.Nhom,
    --pbtp.MaNhanVien,
    pbtp.MaThanhPham";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate, toDate, xuongId, isDinhMucBinhThuong, isFloor })
                .Result
                .ToList();
            return items;
        }
    }
 public List<T> GetTongHopNhanViensHN2<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isFloor = true)
    {

        var query = @"
         
Select
    p.*,
    case
        when p.DinhMucThucTe > p.DinhMucChuan then 0
        else 1
    end as DanhGia
from
    (
        Select
            p.*,
            p.TongDauNhan + p.TongRotNhan as TongNhan,
            p.TongDauTra + p.TongRotTra as TongTra,
            p.TongSoDauTra + p.TongSoRotTra as TongSoTra,
            cast(
                case
                    when p.TongDauTra = 0 then 0
                    else p.TongDauNhan / p.TongDauTra
                end as decimal(18, 2)
            ) as DinhMucDau,
            cast(
                case
                    when p.TongRotTra = 0 then 0
                    else p.TongRotNhan / p.TongRotTra
                end as decimal(18, 2)
            ) as DinhMucRot,
            cast(
                case
                    when p.TongRotTra + p.TongDauTra = 0 then 0
                    else (p.TongRotNhan + p.TongDauNhan) / (p.TongRotTra + p.TongDauTra)
                end as decimal(18, 2)
            ) as DinhMucThucTe,
            thoiGian.TongThoiGian,
            thoiGian.ThoiGianVao,
            thoiGian.ThoiGianRa
        from
            (
                Select
                    p.Ngay,
                    p.MaNhanVien,
                    p.IsGiaCong,
                    p.NhanVienName,
                    p.MaHoSo,
                    p.Nhom,
                    p.MaThanhPham,
                    p.TenXuong,
                    p.ThanhPhamName,
                    p.KhongPhanBietSize,
                    p.LoaiCaName,
                    p.MaLoaiCa,
                    p.MaMau,
                    p.MauName,
                    p.SizeName,
                    p.MaSize,
                    p.MaLo,
                    p.DinhMucChuan,
                    p.MaXuong,
                    sum(
                        case
                            when p.DanhGia = 1 then p.TrongLuongNhan
                            else 0
                        end
                    ) as TongDauNhan,
                    sum(
                        case
                            when p.DanhGia = 1 then p.TrongLuongTra
                            else 0
                        end
                    ) as TongDauTra,
                    sum(
                        case
                            when p.DanhGia = 0 then p.TrongLuongNhan
                            else 0
                        end
                    ) as TongRotNhan,
                    sum(
                        case
                            when p.DanhGia = 0 then p.TrongLuongTra
                            else 0
                        end
                    ) as TongRotTra,
                    sum(
                        case
                            when p.DanhGia = 1 then 1
                            else 0
                        end
                    ) as TongSoDauTra,
                    sum(
                        case
                            when p.DanhGia = 0 then 1
                            else 0
                        end
                    ) as TongSoRotTra
                from
                    (
                        select
                            (
                                case
                                    when p.DanhGia = 1 then N'Đậu'
                                    else N'Rớt'
                                end
                            ) as DanhGia2,
                            p.*
                        from
                            (
                                Select
                                    tp.STT,
                                    tp.Ngay,
                                    tp.Gio,
                                    cast(
                                        DATEDIFF(MINUTE, pbtp.Gio, tp.Gio) as decimal(18, 3)
                                    ) as ThoiGianHoanThanh,
                                    tp.MaNhanVien,
                                    tp.IsGiaCong,
                                    tp.MaHoSo,
                                    tp.NhanVienName,
                                    tp.Nhom,
                                    tp.MaThanhPham,
                                    tp.Ten as TenXuong,
                                    tp.ThanhPhamName,
                                    tp.KhongPhanBietSize,
                                    tp.LoaiCaName,
                                    tp.MaLoaiCa,
                                    tp.MaMau,
                                    tp.MauName,
                                    tp.SizeName,
                                    tp.MaSize,
                                    tp.MaLo,
                                    case
                                        when @isDinhMucBinhThuong = 1 then(tp.DinhMuc)
                                        else (
                                            case
                                                when tp.DinhMuc > 1 THEN 1
                                                else (
                                                    tp.DinhMuc
                                                )
                                            end
                                        )
                                    end as DinhMuc,
                                    ISNULL(dm.DinhMuc, tp.DinhMucChuan) as DinhMucChuan,
                                    tp.TrongLuongNhan,
                                    tp.TrongLuongTra,
                                    tp.TrongLuongTare,
                                    cast(
                                        (
                                            Case
                                                when @isDinhMucBinhThuong = 1 then case
                                                    when tp.DinhMuc <= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                                                    else 0
                                                end
                                                else case
                                                    when tp.DinhMuc >= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                                                    else 0
                                                end
                                            end
                                        ) as bit
                                    ) as DanhGia,
                                    --0 as DonGia,
                                    --0 as ThanhTien,
                                    tp.MaMayCan,
                                    pbtp.Gio as GioBTP,
                                    pbtp.MaMayCan as MayCanBTP,
                                    tp.MaXuong,
                                    --tp.MaSanPham,
                                    tp.GhiChu
                                from
                                    (
                                        Select
                                            ptp.STT,
                                            ptp.Gio,
                                            ptp.STTBTP,
                                            ptp.MaMayCanBTP,
                                            ptp.Ngay,
                                            ptp.MaNhanVien,
                                            ptp.MaNhanVienPhucVu,
                                            n.MaHoSo,
                                            n.Name as NhanVienName,
                                            n.DeptName0 as Nhom,
                                            n.IsGiaCong,
                                            tp.Ma as MaThanhPham,
                                            tp.Ten as ThanhPhamName,
                                            n.LoaiSanLuong,
                                            s.Ma as MaSize,
                                            s.Ten as SizeName,
                                            ptp.MaLo,
                                            ptp.MaMau,
                                            ptp.MaLoaiCa,
                                            la.Ten as LoaiCaName,
                                            mau.Ten as MauName,
                                            case
                                                when @isDinhMucBinhThuong = 1 then(
                                                    case
                                                        when ptp.TrongLuongTra = 0 then 0
                                                        else floor(
                                                            1000 * ptp.TrongLuongNhan / ptp.TrongLuongTra
                                                        ) / 1000
                                                    end
                                                )
                                                else (
                                                    case
                                                        when(ptp.TrongLuongNhan) = 0 THEN 0
                                                        else (
                                                            ROUND(
                                                                ptp.TrongLuongTra / (ptp.TrongLuongNhan),
                                                                4
                                                            )
                                                        )
                                                    end
                                                )
                                            end as DinhMuc,
                                            ptp.TrongLuongTare,
                                            tp.DinhMuc as DinhMucChuan,
                                            ptp.TrongLuongNhan,
                                            ptp.TrongLuongTra,
                                            ptp.MaMayCan,
                                            ptp.MaXuong,
                                            xn.Ten,
                                            ptp.GhiChu,
                                            tp.KhongPhanBietSize
                                        from
                                            PhieuCanTPFilletv2 ptp
                                            left join NhanVienDaiThanh n on ptp.MaNhanVien = n.MaNhanVien
                                            left join MaThanhPhamFillet tp on ptp.MaThanhPham = tp.Ma
                                            left join MaSizeFillet s on ptp.MaSize = s.Ma
                                            left join MaMauFillet mau on ptp.MaMau = mau.Ma
                                            left join MaLoaiCaFillet la on ptp.MaLoaiCa = la.Ma
                                            left join XiNghiep xn on ptp.MaXuong = xn.Ma
                                        where
                                            ptp.Ngay >= @fromDate
                                            and ptp.Ngay <= @toDate
                                            and ptp.MaXuong = @xuongId
                                            and ptp.TrongLuongTra > 0
                                            and ptp.STT > 0 and tp.IsNguyenCon = 0
                                    ) tp
                                    left join(
                                        Select
                                            *
                                        from
                                            (
                                                Select
                                                    d.*,
                                                    ROW_NUMBER() OVER(
                                                        PARTITION BY MaLo,
                                                        MaLoaiCa,
                                                        MaMau,
                                                        MaSize,
                                                        MaThanhPham
                                                        ORDER BY
                                                            Gio DESC
                                                    ) AS [ROW NUMBER]
                                                from
                                                    DinhMucFillet d
                                                where
                                                    Ngay >= @fromDate
                                                    and Ngay <= @toDate
                                                    And MaXuong = @xuongId
                                            ) dm
                                        Where
                                            dm.[ROW NUMBER] = 1
                                    ) dm on tp.MaThanhPham = dm.MaThanhPham
                                    and tp.MaLo = dm.MaLo
                                    and tp.MaSize = dm.MaSize
                                    and tp.MaMau = dm.MaMau
                                    and tp.MaLoaiCa = dm.MaLoaiCa
                                    and tp.Ngay = dm.Ngay
                                    LEFT JOIN PhieuCanBTPFilletv2 pbtp on tp.Ngay = pbtp.Ngay
                                    and tp.STTBTP = pbtp.STT
                                    and tp.MaMayCanBTP = pbtp.MaMayCan
                            ) p
                    ) p
                GROUP BY
                    p.Ngay,
                    p.MaNhanVien,
                    p.IsGiaCong,
                    p.NhanVienName,
                    p.MaHoSo,
                    p.Nhom,
                    p.MaThanhPham,
                    p.TenXuong,
                    p.ThanhPhamName,
                    p.LoaiCaName,
                    p.MauName,
                    p.SizeName,
                    p.MaSize,
                    p.MaLo,
                    p.DinhMucChuan,
                    p.MaXuong,
                    p.KhongPhanBietSize,
                    p.MaLoaiCa,
                    p.MaMau
            ) p
            LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            LEFT join (
                Select
                    p.MaXuong,
                    p.Ngay,
                    p.MaNhanVien,
                    cast(p.Ngay as datetime) + cast(Min(btp.Gio) as datetime) as ThoiGianVao,
                    cast(p.Ngay as datetime) + cast(Max(p.Gio) as datetime) as ThoiGianRa,
                    cast(
                        cast (
                            DATEDIFF(
                                MINUTE,
                                cast(p.Ngay as datetime) + cast(Min(btp.Gio) as datetime),
                                cast(p.Ngay as datetime) + cast(Max(p.Gio) as datetime)
                            ) as decimal(18, 2)
                        ) / 60 as decimal(18, 2)
                    ) as TongThoiGian
                from
                    PhieuCanTPFilletv2 p,
                    PhieuCanBTPFilletv2 btp
                where
                    p.Ngay >= @fromDate
                    and p.Ngay <= @toDate
                    and p.MaXuong = @xuongId
                    and p.IdIn = btp.Id
                GROUP BY
                    p.MaXuong,
                    p.Ngay,
                    p.MaNhanVien
            ) as thoiGian on p.MaXuong = thoiGian.MaXuong
            and p.Ngay = thoiGian.Ngay
            and p.MaNhanVien = thoiGian.MaNhanVien
    ) p
order by
    p.Ngay,
    p.MaXuong

";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate, toDate, xuongId, isDinhMucBinhThuong, isFloor })
                .Result
                .ToList();
            return items;
        }
    }

    /// <summary>
    /// Sử dụng riêng cho Hoàng Long, lý do để Thống Kê lấy dữ liệu Fillet ko bị lỗi vì Ko có loại thành phẩm Nguyên Con ở BTPFillet do Xẻ Bướm chuyển qua, chỉ hiển thị ở Xẻ Bướm loại này
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="xuongId"></param>
    /// <param name="isDinhMucBinhThuong"></param>
    /// <param name="isFloor"></param>
    /// <returns></returns>
    public List<T> GetTongHopNhanVien2HoangLong<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isFloor = true)
    {

        var query = @"
         
Select
    p.*,
    case
        when p.DinhMucThucTe > p.DinhMucChuan then 0
        else 1
    end as DanhGia
from
    (
        Select
            p.*,
            p.TongDauNhan + p.TongRotNhan as TongNhan,
            p.TongDauTra + p.TongRotTra as TongTra,
            p.TongSoDauTra + p.TongSoRotTra as TongSoTra,
            cast(
                case
                    when p.TongDauTra = 0 then 0
                    else p.TongDauNhan / p.TongDauTra
                end as decimal(18, 2)
            ) as DinhMucDau,
            cast(
                case
                    when p.TongRotTra = 0 then 0
                    else p.TongRotNhan / p.TongRotTra
                end as decimal(18, 2)
            ) as DinhMucRot,
            cast(
                case
                    when p.TongRotTra + p.TongDauTra = 0 then 0
                    else (p.TongRotNhan + p.TongDauNhan) / (p.TongRotTra + p.TongDauTra)
                end as decimal(18, 2)
            ) as DinhMucThucTe,
            thoiGian.TongThoiGian,
            thoiGian.ThoiGianVao,
            thoiGian.ThoiGianRa
        from
            (
                Select
                    p.Ngay,
                    p.MaNhanVien,
                    p.IsGiaCong,
                    p.NhanVienName,
                    p.MaHoSo,
                    p.Nhom,
                    p.MaThanhPham,
                    p.ThanhPhamName,
                    p.TenXuong,
                    p.KhongPhanBietSize,
                    p.LoaiCaName,
                    p.MaLoaiCa,
                    p.MaMau,
                    p.MauName,
                    p.SizeName,
                    p.MaSize,
                    p.MaLo,
                    p.DinhMucChuan,
                    p.MaXuong,
                    sum(
                        case
                            when p.DanhGia = 1 then p.TrongLuongNhan
                            else 0
                        end
                    ) as TongDauNhan,
                    sum(
                        case
                            when p.DanhGia = 1 then p.TrongLuongTra
                            else 0
                        end
                    ) as TongDauTra,
                    sum(
                        case
                            when p.DanhGia = 0 then p.TrongLuongNhan
                            else 0
                        end
                    ) as TongRotNhan,
                    sum(
                        case
                            when p.DanhGia = 0 then p.TrongLuongTra
                            else 0
                        end
                    ) as TongRotTra,
                    sum(
                        case
                            when p.DanhGia = 1 then 1
                            else 0
                        end
                    ) as TongSoDauTra,
                    sum(
                        case
                            when p.DanhGia = 0 then 1
                            else 0
                        end
                    ) as TongSoRotTra
                from
                    (
                        select
                            (
                                case
                                    when p.DanhGia = 1 then N'Đậu'
                                    else N'Rớt'
                                end
                            ) as DanhGia2,
                            p.*
                        from
                            (
                                Select
                                    tp.STT,
                                    tp.Ngay,
                                    tp.Gio,
                                    cast(
                                        DATEDIFF(MINUTE, pbtp.Gio, tp.Gio) as decimal(18, 3)
                                    ) as ThoiGianHoanThanh,
                                    tp.MaNhanVien,
                                    tp.IsGiaCong,
                                    tp.MaHoSo,
                                    tp.NhanVienName,
                                    tp.Nhom,
                                    tp.MaThanhPham,
                                    tp.ThanhPhamName,
                                    tp.Ten as TenXuong,
                                    tp.KhongPhanBietSize,
                                    tp.LoaiCaName,
                                    tp.MaLoaiCa,
                                    tp.MaMau,
                                    tp.MauName,
                                    tp.SizeName,
                                    tp.MaSize,
                                    tp.MaLo,
                                    case
                                        when @isDinhMucBinhThuong = 1 then(tp.DinhMuc)
                                        else (
                                            case
                                                when tp.DinhMuc > 1 THEN 1
                                                else (
                                                    tp.DinhMuc
                                                )
                                            end
                                        )
                                    end as DinhMuc,
                                    ISNULL(dm.DinhMuc, tp.DinhMucChuan) as DinhMucChuan,
                                    tp.TrongLuongNhan,
                                    tp.TrongLuongTra,
                                    tp.TrongLuongTare,
                                    cast(
                                        (
                                            Case
                                                when @isDinhMucBinhThuong = 1 then case
                                                    when tp.DinhMuc <= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                                                    else 0
                                                end
                                                else case
                                                    when tp.DinhMuc >= ISNULL(dm.DinhMuc, tp.DinhMucChuan) then 1
                                                    else 0
                                                end
                                            end
                                        ) as bit
                                    ) as DanhGia,
                                    --0 as DonGia,
                                    --0 as ThanhTien,
                                    tp.MaMayCan,
                                    pbtp.Gio as GioBTP,
                                    pbtp.MaMayCan as MayCanBTP,
                                    tp.MaXuong,
                                    --tp.MaSanPham,
                                    tp.GhiChu
                                from
                                    (
                                        Select
                                            ptp.STT,
                                            ptp.Gio,
                                            ptp.STTBTP,
                                            ptp.MaMayCanBTP,
                                            ptp.Ngay,
                                            ptp.MaNhanVien,
                                            ptp.MaNhanVienPhucVu,
                                            n.MaHoSo,
                                            n.Name as NhanVienName,
                                            n.DeptName0 as Nhom,
                                            n.IsGiaCong,
                                            --tp.Ma as MaThanhPham,
                                            --tp.Ten as ThanhPhamName,
                                            ptp.MaThanhPham2 as MaThanhPham,
                                             tp.Ten ThanhPhamName,
                                            n.LoaiSanLuong,
                                            s.Ma as MaSize,
                                            s.Ten as SizeName,
                                            ptp.MaLo,
                                            ptp.MaMau,
                                            ptp.MaLoaiCa,
                                            la.Ten as LoaiCaName,
                                            mau.Ten as MauName,
                                            case
                                                when @isDinhMucBinhThuong = 1 then(
                                                    case
                                                        when ptp.TrongLuongTra = 0 then 0
                                                        else floor(
                                                            1000 * ptp.TrongLuongNhan / ptp.TrongLuongTra
                                                        ) / 1000
                                                    end
                                                )
                                                else (
                                                    case
                                                        when(ptp.TrongLuongNhan) = 0 THEN 0
                                                        else (
                                                            ROUND(
                                                                ptp.TrongLuongTra / (ptp.TrongLuongNhan),
                                                                4
                                                            )
                                                        )
                                                    end
                                                )
                                            end as DinhMuc,
                                            ptp.TrongLuongTare,
                                            tp.DinhMuc as DinhMucChuan,
                                            ptp.TrongLuongNhan,
                                            ptp.TrongLuongTra,
                                            ptp.MaMayCan,
                                            ptp.MaXuong,
                                            xn.Ten,
                                            ptp.GhiChu,
                                            tp.KhongPhanBietSize
                                        from
                                            (
                                                Select
                                                    p.*, CASE
                                                WHEN tp.IsNguyenCon = 1 THEN 'CTR'
                                                ELSE tp.Ma
                                                END as MaThanhPham2
                                                from
                                                    PhieuCanTPFilletv2 p,
                                                    MaThanhPhamFillet tp
                                                where
                                                    p.Ngay >= @fromDate
                                                    and p.Ngay <= @toDate
                                                    and p.MaXuong = @xuongId
                                                    and p.TrongLuongTra > 0
                                                    and p.STT > 0
                                                    and p.MaThanhPham = tp.Ma
                                            ) ptp
                                            left join NhanVienDaiThanh n on ptp.MaNhanVien = n.MaNhanVien
                                            left join MaThanhPhamFillet tp on ptp.MaThanhPham2 = tp.Ma
                                            left join MaSizeFillet s on ptp.MaSize = s.Ma
                                            left join MaMauFillet mau on ptp.MaMau = mau.Ma
                                            left join MaLoaiCaFillet la on ptp.MaLoaiCa = la.Ma
                                            left join XiNghiep xn on ptp.MaXuong = xn.Ma
                                      
                                    ) tp
                                    left join(
                                        Select
                                            *
                                        from
                                            (
                                                Select
                                                    d.*,
                                                    ROW_NUMBER() OVER(
                                                        PARTITION BY MaLo,
                                                        MaLoaiCa,
                                                        MaMau,
                                                        MaSize,
                                                        MaThanhPham
                                                        ORDER BY
                                                            Gio DESC
                                                    ) AS [ROW NUMBER]
                                                from
                                                    DinhMucFillet d
                                                where
                                                    Ngay >= @fromDate
                                                    and Ngay <= @toDate
                                                    And MaXuong = @xuongId
                                            ) dm
                                        Where
                                            dm.[ROW NUMBER] = 1
                                    ) dm on tp.MaThanhPham = dm.MaThanhPham
                                    and tp.MaLo = dm.MaLo
                                    and tp.MaSize = dm.MaSize
                                    and tp.MaMau = dm.MaMau
                                    and tp.MaLoaiCa = dm.MaLoaiCa
                                    and tp.Ngay = dm.Ngay
                                    LEFT JOIN PhieuCanBTPFilletv2 pbtp on tp.Ngay = pbtp.Ngay
                                    and tp.STTBTP = pbtp.STT
                                    and tp.MaMayCanBTP = pbtp.MaMayCan
                            ) p
                    ) p
                GROUP BY
                    p.Ngay,
                    p.MaNhanVien,
                    p.IsGiaCong,
                    p.NhanVienName,
                    p.MaHoSo,
                    p.Nhom,
                    p.MaThanhPham,
                    p.TenXuong,
                    p.ThanhPhamName,
                    p.LoaiCaName,
                    p.MauName,
                    p.SizeName,
                    p.MaSize,
                    p.MaLo,
                    p.DinhMucChuan,
                    p.MaXuong,
                    p.KhongPhanBietSize,
                    p.MaLoaiCa,
                    p.MaMau
            ) p
            LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            LEFT join (
                Select
                    p.MaXuong,
                    p.Ngay,
                    p.MaNhanVien,
                    cast(p.Ngay as datetime) + cast(Min(btp.Gio) as datetime) as ThoiGianVao,
                    cast(p.Ngay as datetime) + cast(Max(p.Gio) as datetime) as ThoiGianRa,
                    cast(
                        cast (
                            DATEDIFF(
                                MINUTE,
                                cast(p.Ngay as datetime) + cast(Min(btp.Gio) as datetime),
                                cast(p.Ngay as datetime) + cast(Max(p.Gio) as datetime)
                            ) as decimal(18, 2)
                        ) / 60 as decimal(18, 2)
                    ) as TongThoiGian
                from
                    PhieuCanTPFilletv2 p,
                    PhieuCanBTPFilletv2 btp
                where
                    p.Ngay >= @fromDate
                    and p.Ngay <= @toDate
                    and p.MaXuong = @xuongId
                    and p.IdIn = btp.Id
                GROUP BY
                    p.MaXuong,
                    p.Ngay,
                    p.MaNhanVien
            ) as thoiGian on p.MaXuong = thoiGian.MaXuong
            and p.Ngay = thoiGian.Ngay
            and p.MaNhanVien = thoiGian.MaNhanVien
    ) p
order by
    p.Ngay,
    p.MaXuong

";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate, toDate, xuongId, isDinhMucBinhThuong, isFloor })
                .Result
                .ToList();
            return items;
        }
    }

    #endregion

    #region Xử Lý Phiếu Cân

    public List<T> GetPhieuCanTPFillet_XLPC<T>(DateTime dateTime, string xuongId)
    {
        var query = @"select
p.STT,
p.Ngay,
p.Gio,
p.MaUserCan,
p.MaMayCan,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaMau,
m.Ten as MauName,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLo,
P.MaThe,
p.TrongLuongNhan,
p.TrongLuongTra,
p.DinhMucThucTe,
p.DinhMucYeuCau,
p.MaXuong,
x.Ten as XuongName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.CaTra,
p.STTBTP,
p.MaMayCanBTP,
p.GhiChu,
p.TrongLuongTare,
p.MaNhanVienPhucVu,
pv.MaHoSo as MaSoPhucVu,
pv.Name as NhanVienPVName,
p.MaBan,
b.Ten as BanName,
p.ThePhieuSanLuongId
from PhieuCanTPFilletv2 p
left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
left join MaMauFillet m on p.MaMau = m.Ma
left join MaSizeFillet s on p.MaSize = s.Ma
left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join NhanVienDaiThanh pv on p.MaNhanVienPhucVu = pv.MaNhanVien
left join BanFillet b on p.MaBan = b.Ma
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

    public List<T> GetDanhSachNhanVienBanTrongLuongByThePhieuSLFillet<T>(string maBan, string idThePhieuSLFillet)
    {
        var query = @"
--select
--p.MaNhanVien,
--nv.MaHoSo,
--nv.Name,
--nv.DeptName0,
--p.MaBan,
--b.Ten as BanName,
--p.TrongLuongTra
--from PhieuCanTPFilletv2 p
--left join BanFillet b on p.MaBan = b.Ma
--left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
--where p.MaBan = @maBan and p.ThePhieuSanLuongId = @idThePhieuSLFillet
select
	nvb.MaNhanVien,
    nvb.MaBan,
    nv.MaHoSo,
    nv.Name,
    nv.DeptName0,
    nvb.MaBan,
    b.Ten as BanName,
    p.TrongLuongTra
from NhanVienTheoBan nvb
left join NhanVienDaiThanh nv on nvb.MaNhanVien = nv.MaNhanVien
left join BanFillet b on nvb.MaBan = b.Ma
left join PhieuCanTPFilletv2 p on p.MaNhanVien = nvb.MaNhanVien and p.ThePhieuSanLuongId = @idThePhieuSLFillet
where nvb.MaBan = @maBan
";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { maBan, idThePhieuSLFillet })
                .Result
                .ToList();
            return items;
        }
    }

    #endregion
}