using AppViewModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ;

public class PhieuCanTPDinhHinh
{
    private readonly string connectionString;

    private readonly string qrDelete = @"DELETE FROM [dbo].[PhieuCanTPDinhHinh]
      WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";

    private readonly string qrGetAll = "Select * from PhieuCanTPDinhHinh";
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
            PhieuCanTPDinhHinh p
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
                    DinhMucDinhHinh dm
                where
                    Ngay <= @toDate
                    And SuDung = 1
                    and MaXuong = @xuongId
            ) dm
        where
            dm.[ROW NUMBER] = 1
    ) dm on p.MaThanhPham = dm.MaThanhPham
    left join MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma";
    private readonly string qrGetsLastByNumAndMayCan = @"WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MaMayCan ORDER BY Ngay DESC, Gio DESC) AS RowNum
    FROM PhieuCanTPDinhHinh where Ngay =@ngay
)

SELECT *
FROM RankedPhieu
WHERE RowNum <= @num";

    private readonly string qrGetsNangSuatTheoNhom = @"declare @fromDate as date = '{0}', @toDate as date ='{1}', @xuongId as varchar(1) ={2}
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
            PhieuCanTPDinhHinh tp,
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
    private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanTPDinhHinh]
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
           ,[GhiChu],[ChiSanLuong],[SuDung],[TrongLuongTare]  ,[TrongLuongBu],[IsOffline],[MaNhanVienPhucVu],[MaNhanVienBanKiem],[Id],[IdIn])
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
           ,@GhiChu,@ChiSanLuong,@SuDung, @TrongLuongTare , @TrongLuongBu,@IsOffline,@MaNhanVienPhucVu,@MaNhanVienBanKiem,@Id,@IdIn)";

    private readonly string qrUpdate = @"
UPDATE [dbo].[PhieuCanTPDinhHinh]
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
      ,[GhiChu] = @GhiChu,ChiSanLuong=@ChiSanLuong, [SuDung] = @SuDung,[TrongLuongTare] = @TrongLuongTare ,[TrongLuongBu] = @TrongLuongBu,[IsOffline] =@IsOffline,[MaNhanVienPhucVu] =@MaNhanVienPhucVu, [MaNhanVienBanKiem] =@MaNhanVienBanKiem
 WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";

    private string tableName = @"PhieuCanTPDinhHinh";

    public PhieuCanTPDinhHinh(string? _connectionString = null)
    {
        connectionString = _connectionString ?? Base.Ins.ConnectionString;
    }
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
            PhieuCanTPDinhHinh tp,
            NhanVienDaiThanh nv
        where
            tp.Ngay >= @fromDate
            and tp.Ngay <= @toDate
            and tp.MaXuong = @xuongId
            and tp.MaNhanVien = nv.MaNhanVien
            and nv.IsGiaCong=0
        GROUP BY
            tp.MaNhanVien,
            nv.DeptName0,
			tp.Ngay
    ) p
GROUP BY
    p.Nhom,
	p.Ngay
order by p.Ngay";
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
            PhieuCanTPDinhHinh p
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
                    DinhMucDinhHinh dm
                where
                    Ngay <= @toDate
                    And SuDung = 1
                    and MaXuong = @xuongId
            ) dm
        where
            dm.[ROW NUMBER] = 1
    ) dm on p.MaThanhPham = dm.MaThanhPham
    left join MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma";
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
    public int ChuyenSize<T>(List<T> items, string sizeId)
    {
        var query = $@"
UPDATE [dbo].[PhieuCanTPDinhHinh]
   SET [MaSize] = '{sizeId}',[GhiChu] = @GhiChu
 WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var rows = connection.Execute(query, items);
            return rows;
        }
    }

    public int ChuyenThanhPham<T>(List<T> items, string thanhPhamId)
    {
        var query = $@"
UPDATE [dbo].[PhieuCanTPDinhHinh]
   SET [MaThanhPham] = '{thanhPhamId}',[GhiChu] = @GhiChu
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
UPDATE [dbo].[PhieuCanTPDinhHinh]
   SET [MaXuong] = '{xuongId}',[GhiChu] = @GhiChu
 WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong ";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var rows = connection.Execute(query, items);
            return rows;
        }
    }

    public int Delete<T>(List<T> items)
    {
        var query = qrDelete;
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

    public List<T> GetChiTietByMaHoSos<T>(
        DateTime fromDate,
        DateTime toDate,
        string maHoSo,
        string xuongId,
        bool isDinhMucBinhThuong = true)
    {
        var query = @"

select
    (
        case
            when p.DanhGia = 1 then N'Đậu'
            else N'Rớt'
        end
    ) as DanhGia2,
    case
        when p.STTCanNhan = 1 then 0
        else p.ThoiGianChenhLech
    end as ThoiGianChenhLech,
    p.STTCanNhan,
    p.STT,
    p.Ngay,
    p.Gio,
    p.ThoiGianHoanThanh,
    p.MaNhanVien,
    p.IsGiaCong,
    p.MaHoSo,
    p.TenNhanVien,
    p.MaNhanVienBanKiem,
    p.TenNhanVienBanKiem,
    p.MaNhanVienBanKiemHoSo,
    p.MaNhanVienPhucVu,
    p.TenNhanVienPhucVu,
    p.MaNhanVienPhucVuHoSo,
    p.Nhom,
    --chắt thêm á
    p.MaThanhPham,
    p.TenXuong,
    p.ThanhPhamName,
    p.LoaiCaName,
    p.MauName,
    p.SizeName,
    p.MaSize,
    p.CaTra,
    p.ChiSanLuong,
    p.MaLo,
    p.MaThe,
    p.DinhMuc,
    p.DinhMucChuan,
    p.TrongLuongNhan,
    p.TrongLuongTra,
    p.TrongLuongChucNang,
    p.TrongLuongTare,
    p.TrongLuongBu,
    p.DanhGia,
    p.DonGia,
    p.ThanhTien,
    p.MaMayCan,
    p.GioBTP,
    p.MayCanBTP,
    p.MaXuong,
    p.MaSanPham,
    p.GhiChu
from
    (
        Select
            cast(
                Cast(
                    DATEDIFF(
                        second,
                        LAG(tp.Gio, 1) over(
                            order by
                                tp.MaXuong,
                                tp.Ngay,
                                tp.MaNhanVien,
                                pbtp.Gio
                        ),
                        pbtp.Gio
                    ) as decimal(18, 3)
                ) / 60 as decimal(18, 3)
            ) as ThoiGianChenhLech,
            ROW_NUMBER() over(
                PARTITION by tp.MaNhanVien
                order by
                    tp.Ngay,
                    pbtp.Gio
            ) as STTCanNhan,
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
            tp.MaNhanVienBanKiem,
            nbk.Name as TenNhanVienBanKiem,
            nbk.MaHoSo as MaNhanVienBanKiemHoSo,
            tp.MaNhanVienPhucVu,
            npv.Name as TenNhanVienPhucVu,
            npv.MaHoSo as MaNhanVienPhucVuHoSo,
            tp.Nhom,
            --chắt thêm á
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
            case
                when tp.CaTra = 1 then 'True'
                when tp.CaTra = 0 then 'F'
                else ''
            end as CaTra,
            tp.ChiSanLuong,
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
            tp.TrongLuongTra * tp.LoaiSanLuong as TrongLuongChucNang,
            tp.TrongLuongTare,
            tp.TrongLuongBu,
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
            0 as DonGia,
            0 as ThanhTien,
            tp.MaMayCan,
            pbtp.Gio as GioBTP,
            pbtp.MaMayCan as MayCanBTP,
            tp.MaXuong,
            tp.MaSanPham,
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
                    ptp.MaNhanVienBanKiem,
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
                    ptp.ChiSanLuong,
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
                                when(ptp.TrongLuongNhan - ptp.TrongLuongBu) = 0 THEN 0
                                else (
                                    ROUND(
                                        ptp.TrongLuongTra / (ptp.TrongLuongNhan - ptp.TrongLuongBu),
                                        4
                                    )
                                )
                            end
                        )
                    end as DinhMuc,
                    ptp.TrongLuongTare,
                    ptp.TrongLuongBu,
                    tp.DinhMuc as DinhMucChuan,
                    ptp.TrongLuongNhan,
                    ptp.TrongLuongTra,
                    ptp.MaMayCan,
                    ptp.MaXuong,
                    tp.BravoId as MaSanPham,
                    xn.Ten,
                    ptp.GhiChu
                from
                    PhieuCanTPDinhHinh ptp,
                    NhanVienDaiThanh n,
                    MaThanhPhamDinhHinh tp,
                    MaSizeDinhHinh s,
                    MaMauDinhHinh mau,
                    MaLoaiCaDinhHinh la,
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
                                MaThanhPham,
                                CaTra
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            DinhMucDinhHinh d
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
            left join NhanVienDaiThanh nbk on tp.MaNhanVienBanKiem = nbk.MaNhanVien
            left join NhanVienDaiThanh npv on tp.MaNhanVienPhucVu = npv.MaNhanVien
            LEFT JOIN PhieuCanBTPDinhHinh pbtp on tp.Ngay = pbtp.Ngay
            and tp.STTBTP = pbtp.STT
            and tp.MaMayCanBTP = pbtp.MaMayCan
    ) p
order by
    p.Ngay,
    p.Gio";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query,
                    new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, maHoSo, isDinhMucBinhThuong }).Result
                .ToList();
            return items;
        }
    }

    public List<T> GetChiTietByMaNhanViens<T>(
        DateTime fromDate,
        DateTime toDate,
        string maNhanVien,
        string xuongId,
        bool isDinhMucBinhThuong = true)
    {
        var query = @"

select
    (
        case
            when p.DanhGia = 1 then N'Đậu'
            else N'Rớt'
        end
    ) as DanhGia2,
    case
        when p.STTCanNhan = 1 then 0
        else p.ThoiGianChenhLech
    end as ThoiGianChenhLech,
    p.STTCanNhan,
    p.STT,
    p.Ngay,
    p.Gio,
    p.ThoiGianHoanThanh,
    p.MaNhanVien,
    p.IsGiaCong,
    p.MaHoSo,
    p.TenNhanVien,
    p.MaNhanVienBanKiem,
    p.TenNhanVienBanKiem,
    p.MaNhanVienBanKiemHoSo,
    p.MaNhanVienPhucVu,
    p.TenNhanVienPhucVu,
    p.MaNhanVienPhucVuHoSo,
    p.Nhom,
    --chắt thêm á
    p.MaThanhPham,
    p.TenXuong,
    p.ThanhPhamName,
    p.LoaiCaName,
    p.MauName,
    p.SizeName,
    p.MaSize,
    p.CaTra,
    p.ChiSanLuong,
    p.MaLo,
    p.MaThe,
    p.DinhMuc,
    p.DinhMucChuan,
    p.TrongLuongNhan,
    p.TrongLuongTra,
    p.TrongLuongChucNang,
    p.TrongLuongTare,
    p.TrongLuongBu,
    p.DanhGia,
    p.DonGia,
    p.ThanhTien,
    p.MaMayCan,
    p.GioBTP,
    p.MayCanBTP,
    p.MaXuong,
    p.MaSanPham,
    p.GhiChu
from
    (
        Select
            cast(
                Cast(
                    DATEDIFF(
                        second,
                        LAG(tp.Gio, 1) over(
                            order by
                                tp.MaXuong,
                                tp.Ngay,
                                tp.MaNhanVien,
                                pbtp.Gio
                        ),
                        pbtp.Gio
                    ) as decimal(18, 3)
                ) / 60 as decimal(18, 3)
            ) as ThoiGianChenhLech,
            ROW_NUMBER() over(
                PARTITION by tp.MaNhanVien
                order by
                    tp.Ngay,
                    pbtp.Gio
            ) as STTCanNhan,
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
            tp.MaNhanVienBanKiem,
            nbk.Name as TenNhanVienBanKiem,
            nbk.MaHoSo as MaNhanVienBanKiemHoSo,
            tp.MaNhanVienPhucVu,
            npv.Name as TenNhanVienPhucVu,
            npv.MaHoSo as MaNhanVienPhucVuHoSo,
            tp.Nhom,
            --chắt thêm á
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
            case
                when tp.CaTra = 1 then 'True'
                when tp.CaTra = 0 then 'F'
                else ''
            end as CaTra,
            tp.ChiSanLuong,
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
            tp.TrongLuongTra * tp.LoaiSanLuong as TrongLuongChucNang,
            tp.TrongLuongTare,
            tp.TrongLuongBu,
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
            0 as DonGia,
            0 as ThanhTien,
            tp.MaMayCan,
            pbtp.Gio as GioBTP,
            pbtp.MaMayCan as MayCanBTP,
            tp.MaXuong,
            tp.MaSanPham,
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
                    ptp.MaNhanVienBanKiem,
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
                    ptp.ChiSanLuong,
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
                                when(ptp.TrongLuongNhan - ptp.TrongLuongBu) = 0 THEN 0
                                else (
                                    ROUND(
                                        ptp.TrongLuongTra / (ptp.TrongLuongNhan - ptp.TrongLuongBu),
                                        4
                                    )
                                )
                            end
                        )
                    end as DinhMuc,
                    ptp.TrongLuongTare,
                    ptp.TrongLuongBu,
                    tp.DinhMuc as DinhMucChuan,
                    ptp.TrongLuongNhan,
                    ptp.TrongLuongTra,
                    ptp.MaMayCan,
                    ptp.MaXuong,
                    tp.BravoId as MaSanPham,
                    xn.Ten,
                    ptp.GhiChu
                from
                    PhieuCanTPDinhHinh ptp,
                    NhanVienDaiThanh n,
                    MaThanhPhamDinhHinh tp,
                    MaSizeDinhHinh s,
                    MaMauDinhHinh mau,
                    MaLoaiCaDinhHinh la,
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
                                MaThanhPham,
                                CaTra
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            DinhMucDinhHinh d
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
            left join NhanVienDaiThanh nbk on tp.MaNhanVienBanKiem = nbk.MaNhanVien
            left join NhanVienDaiThanh npv on tp.MaNhanVienPhucVu = npv.MaNhanVien
            LEFT JOIN PhieuCanBTPDinhHinh pbtp on tp.Ngay = pbtp.Ngay
            and tp.STTBTP = pbtp.STT
            and tp.MaMayCanBTP = pbtp.MaMayCan
    ) p
order by
    p.Ngay,
    p.Gio";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query,
                    new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, maNhanVien, isDinhMucBinhThuong })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetChiTietByMaThes<T>(
        DateTime fromDate,
        DateTime toDate,
        string maThe,
        string xuongId,
        bool isDinhMucBinhThuong = true)
    {
        var query = @"

select
    (
        case
            when p.DanhGia = 1 then N'Đậu'
            else N'Rớt'
        end
    ) as DanhGia2,
    case
        when p.STTCanNhan = 1 then 0
        else p.ThoiGianChenhLech
    end as ThoiGianChenhLech,
    p.STTCanNhan,
    p.STT,
    p.Ngay,
    p.Gio,
    p.ThoiGianHoanThanh,
    p.MaNhanVien,
    p.IsGiaCong,
    p.MaHoSo,
    p.TenNhanVien,
    p.MaNhanVienBanKiem,
    p.TenNhanVienBanKiem,
    p.MaNhanVienBanKiemHoSo,
    p.MaNhanVienPhucVu,
    p.TenNhanVienPhucVu,
    p.MaNhanVienPhucVuHoSo,
    p.Nhom,
    --chắt thêm á
    p.MaThanhPham,
    p.TenXuong,
    p.ThanhPhamName,
    p.LoaiCaName,
    p.MauName,
    p.SizeName,
    p.MaSize,
    p.CaTra,
    p.ChiSanLuong,
    p.MaLo,
    p.MaThe,
    p.DinhMuc,
    p.DinhMucChuan,
    p.TrongLuongNhan,
    p.TrongLuongTra,
    p.TrongLuongChucNang,
    p.TrongLuongTare,
    p.TrongLuongBu,
    p.DanhGia,
    p.DonGia,
    p.ThanhTien,
    p.MaMayCan,
    p.GioBTP,
    p.MayCanBTP,
    p.MaXuong,
    p.MaSanPham,
    p.GhiChu
from
    (
        Select
            cast(
                Cast(
                    DATEDIFF(
                        second,
                        LAG(tp.Gio, 1) over(
                            order by
                                tp.MaXuong,
                                tp.Ngay,
                                tp.MaNhanVien,
                                pbtp.Gio
                        ),
                        pbtp.Gio
                    ) as decimal(18, 3)
                ) / 60 as decimal(18, 3)
            ) as ThoiGianChenhLech,
            ROW_NUMBER() over(
                PARTITION by tp.MaNhanVien
                order by
                    tp.Ngay,
                    pbtp.Gio
            ) as STTCanNhan,
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
            tp.MaNhanVienBanKiem,
            nbk.Name as TenNhanVienBanKiem,
            nbk.MaHoSo as MaNhanVienBanKiemHoSo,
            tp.MaNhanVienPhucVu,
            npv.Name as TenNhanVienPhucVu,
            npv.MaHoSo as MaNhanVienPhucVuHoSo,
            tp.Nhom,
            --chắt thêm á
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
            case
                when tp.CaTra = 1 then 'True'
                when tp.CaTra = 0 then 'F'
                else ''
            end as CaTra,
            tp.ChiSanLuong,
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
            tp.TrongLuongTra * tp.LoaiSanLuong as TrongLuongChucNang,
            tp.TrongLuongTare,
            tp.TrongLuongBu,
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
            0 as DonGia,
            0 as ThanhTien,
            tp.MaMayCan,
            pbtp.Gio as GioBTP,
            pbtp.MaMayCan as MayCanBTP,
            tp.MaXuong,
            tp.MaSanPham,
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
                    ptp.MaNhanVienBanKiem,
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
                    ptp.ChiSanLuong,
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
                                when(ptp.TrongLuongNhan - ptp.TrongLuongBu) = 0 THEN 0
                                else (
                                    ROUND(
                                        ptp.TrongLuongTra / (ptp.TrongLuongNhan - ptp.TrongLuongBu),
                                        4
                                    )
                                )
                            end
                        )
                    end as DinhMuc,
                    ptp.TrongLuongTare,
                    ptp.TrongLuongBu,
                    tp.DinhMuc as DinhMucChuan,
                    ptp.TrongLuongNhan,
                    ptp.TrongLuongTra,
                    ptp.MaMayCan,
                    ptp.MaXuong,
                    tp.BravoId as MaSanPham,
                    xn.Ten,
                    ptp.GhiChu
                from
                    PhieuCanTPDinhHinh ptp,
                    NhanVienDaiThanh n,
                    MaThanhPhamDinhHinh tp,
                    MaSizeDinhHinh s,
                    MaMauDinhHinh mau,
                    MaLoaiCaDinhHinh la,
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
                                MaThanhPham,
                                CaTra
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            DinhMucDinhHinh d
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
            left join NhanVienDaiThanh nbk on tp.MaNhanVienBanKiem = nbk.MaNhanVien
            left join NhanVienDaiThanh npv on tp.MaNhanVienPhucVu = npv.MaNhanVien
            LEFT JOIN PhieuCanBTPDinhHinh pbtp on tp.Ngay = pbtp.Ngay
            and tp.STTBTP = pbtp.STT
            and tp.MaMayCanBTP = pbtp.MaMayCan
    ) p
order by
    p.Ngay,
    p.Gio";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query,
                    new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, maThe, isDinhMucBinhThuong }).Result
                .ToList();
            return items;
        }
    }

    public List<T> GetChiTiets<T>(DateTime dateTime, string xuongId, bool isDinhMucBinhThuong = true)
    {
        var query = @"Select
    tp.STT,
    tp.Gio,
    tp.MaNhanVien,
    tp.MaHoSo,
    tp.TenNhanVien,
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
    tp.ChiSanLuong,
    tp.MaLo,
    tp.DinhMuc,
    dm.DinhMuc as DinhMucChuan,
    tp.TrongLuongTra,
    Case
        when @isDinhMucBinhThuong = 1 then case
            when tp.DinhMuc <= dm.DinhMuc then N'Đạt'
            else N'Không Đạt'
        end
        else case
            when tp.DinhMuc >= dm.DinhMuc then N'Đạt'
            else N'Không Đạt'
        end
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
            ptp.ChiSanLuong,
            ptp.MaLo,
            ptp.MaMau,
            ptp.MaLoaiCa,
            la.Ten as LoaiCaName,
            mau.Ten as MauName,
            case
                when @isDinhMucBinhThuong = 1 then floor(
                    1000 * ptp.TrongLuongNhan / ptp.TrongLuongTra
                ) / 1000
                else ptp.TrongLuongTra / ptp.TrongLuongNhan
            end as DinhMuc,
            ptp.TrongLuongTra,
            ptp.MaMayCan
        from
            PhieuCanTPDinhHinh ptp,
            NhanVienDaiThanh n,
            MaThanhPhamDinhHinh tp,
            MaSizeDinhHinh s,
            MaMauDinhHinh mau,
            MaLoaiCaDinhHinh la
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
                    DinhMucDinhHinh d
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
order by
    Gio";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { ngay = dateTime.Date, xuongId, isDinhMucBinhThuong })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetChiTiets<T>(
        DateTime dateTime,
        string xuongId,
        string maNhanVien,
        bool isDinhMucBinhThuong = true)
    {
        var query = @"Select
    tp.STT,
    tp.Gio,
    tp.MaNhanVien,
    tp.MaHoSo,
    tp.TenNhanVien,
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
    tp.ChiSanLuong,
    tp.MaLo,
    tp.DinhMuc,
    dm.DinhMuc as DinhMucChuan,
    tp.TrongLuongTra,
    Case
        when @isDinhMucBinhThuong = 1 then case
            when tp.DinhMuc <= dm.DinhMuc then N'Đạt'
            else N'Không Đạt'
        end
        else case
            when tp.DinhMuc >= dm.DinhMuc then N'Đạt'
            else N'Không Đạt'
        end
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
            ptp.ChiSanLuong,
            ptp.MaLo,
            ptp.MaMau,
            ptp.MaLoaiCa,
            la.Ten as LoaiCaName,
            mau.Ten as MauName,
            case
                when @isDinhMucBinhThuong = 1 then floor(
                    1000 * ptp.TrongLuongNhan / ptp.TrongLuongTra
                ) / 1000
                else ptp.TrongLuongTra / ptp.TrongLuongNhan
            end as DinhMuc,
            ptp.TrongLuongTra,
            ptp.MaMayCan
        from
            PhieuCanTPDinhHinh ptp,
            NhanVienDaiThanh n,
            MaThanhPhamDinhHinh tp,
            MaSizeDinhHinh s,
            MaMauDinhHinh mau,
            MaLoaiCaDinhHinh la
        where
            ptp.Ngay = @ngay
            and ptp.MaXuong = @xuongId
			--and n.Name = @nhanVienName
            and ptp.MaNhanVien = n.MaNhanVien 
			and ptp.MaNhanVien  = @maNhanVien
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
                    DinhMucDinhHinh d
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
order by
    Gio";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new
                    {
                        ngay = dateTime.Date, xuongId, maNhanVien, isDinhMucBinhThuong
                    })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetChiTiets<T>(
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
select
    (
        case
            when p.DanhGia = 1 then N'Đậu'
            else N'Rớt'
        end
    ) as DanhGia2,
    case
        when p.STTCanNhan = 1 then 0
        else p.ThoiGianChenhLech
    end as ThoiGianChenhLech,
    p.STTCanNhan,
    p.STT,
    p.Ngay,
    p.Gio,
    p.ThoiGianHoanThanh,
    p.MaNhanVien,
    p.IsGiaCong,
    p.MaHoSo,
    p.TenNhanVien,
    p.MaNhanVienBanKiem,
    p.TenNhanVienBanKiem,
    p.MaNhanVienBanKiemHoSo,
    p.MaNhanVienPhucVu,
    p.TenNhanVienPhucVu,
    p.MaNhanVienPhucVuHoSo,
    p.Nhom,
    --chắt thêm á
    p.MaThanhPham,
    p.TenXuong,
    p.ThanhPhamName,
    p.LoaiCaName,
    p.MauName,
    p.SizeName,
    p.MaSize,
    p.CaTra,
    p.ChiSanLuong,
    p.MaLo,
    p.MaThe,
    p.DinhMuc,
    Cast( p.DinhMucChuan as decimal(18,3)) as DinhMucChuan,
    p.TrongLuongNhan,
    p.TrongLuongTra,
    p.TrongLuongTra as TrongLuongTraTyLeOrg,
    p.TrongLuongChucNang,
    p.TrongLuongTare,
    p.TrongLuongBu,
    p.DanhGia,
    p.DonGia,
    p.ThanhTien,
    p.MaMayCan,
    p.GioBTP,
    p.MayCanBTP,
    p.MaXuong,
    p.MaSanPham,
    p.GhiChu
from
    (
        Select
            cast(
                Cast(
                    DATEDIFF(
                        second,
                        LAG(tp.Gio, 1) over(
                            order by
                                tp.MaXuong,
                                tp.Ngay,
                                tp.MaNhanVien,
                                pbtp.Gio
                        ),
                        pbtp.Gio
                    ) as decimal(18, 3)
                ) / 60 as decimal(18, 3)
            ) as ThoiGianChenhLech,
            ROW_NUMBER() over(
                PARTITION by tp.MaNhanVien
                order by
                    tp.Ngay,
                    pbtp.Gio
            ) as STTCanNhan,
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
            tp.MaNhanVienBanKiem,
            nbk.Name as TenNhanVienBanKiem,
            nbk.MaHoSo as MaNhanVienBanKiemHoSo,
            tp.MaNhanVienPhucVu,
            npv.Name as TenNhanVienPhucVu,
            npv.MaHoSo as MaNhanVienPhucVuHoSo,
            tp.Nhom,
            --chắt thêm á
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
            tp.CaTra,
            tp.ChiSanLuong,
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
            tp.TrongLuongTra * tp.LoaiSanLuong as TrongLuongChucNang,
            tp.TrongLuongTare,
            tp.TrongLuongBu,
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
            0 as DonGia,
            0 as ThanhTien,
            tp.MaMayCan,
            pbtp.Gio as GioBTP,
            pbtp.MaMayCan as MayCanBTP,
            tp.MaXuong,
            tp.MaSanPham,
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
                    ptp.MaNhanVienBanKiem,
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
                    ptp.ChiSanLuong,
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
                                when(ptp.TrongLuongNhan - ptp.TrongLuongBu) = 0 THEN 0
                                else (
                                    ROUND(
                                        ptp.TrongLuongTra / (ptp.TrongLuongNhan - ptp.TrongLuongBu),
                                        4
                                    )
                                )
                            end
                        )
                    end as DinhMuc,
                    ptp.TrongLuongTare,
                    ptp.TrongLuongBu,
                    tp.DinhMuc as DinhMucChuan,
                    ptp.TrongLuongNhan,
                    ptp.TrongLuongTra,
                    ptp.MaMayCan,
                    ptp.MaXuong,
                    tp.BravoId as MaSanPham,
                    xn.Ten,
                    ptp.GhiChu
                from
                    PhieuCanTPDinhHinh ptp
                    left join NhanVienDaiThanh n on ptp.MaNhanVien = n.MaNhanVien
                    left join MaThanhPhamDinhHinh tp on ptp.MaThanhPham = tp.Ma
                    left join MaSizeDinhHinh s on ptp.MaSize = s.Ma
                    left join MaMauDinhHinh mau on ptp.MaMau = mau.Ma
                    left join MaLoaiCaDinhHinh la on ptp.MaLoaiCa = la.Ma
                    left join XiNghiep xn on ptp.MaXuong = xn.Ma
                where
                    ptp.Ngay >= @fromDate
                    and ptp.Ngay <= @toDate
                    and ptp.MaXuong = @xuongId
                    and ptp.TrongLuongTra > 0
                    
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
                                MaThanhPham,
                                CaTra
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            DinhMucDinhHinh d
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
            left join NhanVienDaiThanh nbk on tp.MaNhanVienBanKiem = nbk.MaNhanVien
            left join NhanVienDaiThanh npv on tp.MaNhanVienPhucVu = npv.MaNhanVien
            LEFT JOIN PhieuCanBTPDinhHinh pbtp on tp.Ngay = pbtp.Ngay
            and tp.STTBTP = pbtp.STT
            and tp.MaMayCanBTP = pbtp.MaMayCan
    ) p
order by
    p.Ngay,
    p.Gio";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query).Result.ToList();
            return items;
        }
    }

    public List<T> GetChiTiets<T>(
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
select
    (
        case
            when p.DanhGia = 1 then N'Đậu'
            else N'Rớt'
        end
    ) as DanhGia2,
    case
        when p.STTCanNhan = 1 then 0
        else p.ThoiGianChenhLech
    end as ThoiGianChenhLech,
    p.STTCanNhan,
    p.STT,
    p.Ngay,
    p.Gio,
    p.ThoiGianHoanThanh,
    p.MaNhanVien,
    p.IsGiaCong,
    p.MaHoSo,
    p.TenNhanVien,
    p.MaNhanVienBanKiem,
    p.TenNhanVienBanKiem,
    p.MaNhanVienBanKiemHoSo,
    p.MaNhanVienPhucVu,
    p.TenNhanVienPhucVu,
    p.MaNhanVienPhucVuHoSo,
    p.Nhom,
    --chắt thêm á
    p.MaThanhPham,
    p.TenXuong,
    p.ThanhPhamName,
    p.LoaiCaName,
    p.MauName,
    p.SizeName,
    p.MaSize,
    p.CaTra,
    p.ChiSanLuong,
    p.MaLo,
    p.MaThe,
    p.DinhMuc,
    p.DinhMucChuan,
    p.TrongLuongNhan,
    p.TrongLuongTra,
    p.TrongLuongChucNang,
    p.TrongLuongTare,
    p.TrongLuongBu,
    p.DanhGia,
    p.DonGia,
    p.ThanhTien,
    p.MaMayCan,
    p.GioBTP,
    p.MayCanBTP,
    p.MaXuong,
    p.MaSanPham,
    p.GhiChu
from
    (
        Select
            cast(
                Cast(
                    DATEDIFF(
                        second,
                        LAG(tp.Gio, 1) over(
                            order by
                                tp.MaXuong,
                                tp.Ngay,
                                tp.MaNhanVien,
                                pbtp.Gio
                        ),
                        pbtp.Gio
                    ) as decimal(18, 3)
                ) / 60 as decimal(18, 3)
            ) as ThoiGianChenhLech,
            ROW_NUMBER() over(
                PARTITION by tp.MaNhanVien
                order by
                    tp.Ngay,
                    pbtp.Gio
            ) as STTCanNhan,
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
            tp.MaNhanVienBanKiem,
            nbk.Name as TenNhanVienBanKiem,
            nbk.MaHoSo as MaNhanVienBanKiemHoSo,
            tp.MaNhanVienPhucVu,
            npv.Name as TenNhanVienPhucVu,
            npv.MaHoSo as MaNhanVienPhucVuHoSo,
            tp.Nhom,
            --chắt thêm á
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
            case
                when tp.CaTra = 1 then 'True'
                when tp.CaTra = 0 then 'F'
                else ''
            end as CaTra,
            tp.ChiSanLuong,
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
            tp.TrongLuongTra * tp.LoaiSanLuong as TrongLuongChucNang,
            tp.TrongLuongTare,
            tp.TrongLuongBu,
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
            0 as DonGia,
            0 as ThanhTien,
            tp.MaMayCan,
            pbtp.Gio as GioBTP,
            pbtp.MaMayCan as MayCanBTP,
            tp.MaXuong,
            tp.MaSanPham,
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
                    ptp.MaNhanVienBanKiem,
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
                    ptp.ChiSanLuong,
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
                                when(ptp.TrongLuongNhan - ptp.TrongLuongBu) = 0 THEN 0
                                else (
                                    ROUND(
                                        ptp.TrongLuongTra / (ptp.TrongLuongNhan - ptp.TrongLuongBu),
                                        4
                                    )
                                )
                            end
                        )
                    end as DinhMuc,
                    ptp.TrongLuongTare,
                    ptp.TrongLuongBu,
                    tp.DinhMuc as DinhMucChuan,
                    ptp.TrongLuongNhan,
                    ptp.TrongLuongTra,
                    ptp.MaMayCan,
                    ptp.MaXuong,
                    tp.BravoId as MaSanPham,
                    xn.Ten,
                    ptp.GhiChu
                from
                    PhieuCanTPDinhHinh ptp,
                    NhanVienDaiThanh n,
                    MaThanhPhamDinhHinh tp,
                    MaSizeDinhHinh s,
                    MaMauDinhHinh mau,
                    MaLoaiCaDinhHinh la,
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
                                MaThanhPham,
                                CaTra
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            DinhMucDinhHinh d
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
            left join NhanVienDaiThanh nbk on tp.MaNhanVienBanKiem = nbk.MaNhanVien
            left join NhanVienDaiThanh npv on tp.MaNhanVienPhucVu = npv.MaNhanVien
            LEFT JOIN PhieuCanBTPDinhHinh pbtp on tp.Ngay = pbtp.Ngay
            and tp.STTBTP = pbtp.STT
            and tp.MaMayCanBTP = pbtp.MaMayCan
    ) p
order by
    p.Ngay,
    p.Gio";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query).Result.ToList();
            return items;
        }
    }

    public List<T> GetChiTiets_TG<T>(DateTime fromDate, DateTime toDate)
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
    n.MaHoSo as MaHoSoPhucVu,
    p.MaLo,
    p.MaXuong,
    p.MaMayCan,
    0 as DonGia,
    0 * p.TrongLuongTra as ThanhTien,
    p.MaHoSo
from
    (
        Select
            p.*,
            nv.MaHoSo
        from
            PhieuCanTPDinhHinh p,
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

    public int GetMaxSTT(DateTime dateTime, string xuongId, string mayCanId)
    {
        var query =
            @"select ISNULL( MAX(STT),0) from PhieuCanTPDinhHinh where Ngay=@ngay and MaXuong=@xuongId  and MaMayCan=@mayCanId";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var item = connection.ExecuteScalar<int>(query, new { ngay = dateTime.Date, xuongId, mayCanId });
        return item;
    }

    public int GetMaxSTT(DateTime dateTime, string mayCanId)
    {
        var query =
            @"select ISNULL( MAX(STT),0) from PhieuCanTPDinhHinh where Ngay=@ngay and MaMayCan=@mayCanId";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var item = connection.ExecuteScalar<int>(query, new { ngay = dateTime.Date, mayCanId });
        return item;
    }

    public List<string> GetMayCans(DateTime dateTime)
    {
        var query = @"Select DISTINCT MaMayCan from PhieuCanTPDinhHinh where Ngay = @ngay ";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.Query<string>(query, new { ngay = dateTime.Date }).ToList();
        return items;
    }

    public List<T> GetNhanVienKhongLamViecs<T>(DateTime dateTime)
    {
        try
        {
            var query = @"Select
    *
from
    NhanVienDaiThanh
where
    MaNhanVien not in (
        Select
            DISTINCT MaNhanvien
        from
            PhieuCanTPDinhHinh
        where
            Ngay = @ngay
    )
    and IsContracting = 1";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date }).Result.ToList();
                return items;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public List<T> GetOfflines<T>(DateTime dateTime, string xuongId)
    {
        var query =
            @"Select * from PhieuCanTPDinhHinh Where Ngay = @ngay and MaXuong=@xuongId and  IsOffline = 1 order by STT DESC";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result
                .ToList();
            return items;
        }
    }

    public List<T> GetPhieuCanDinhHinh_XLPC<T>(DateTime dateTime, string xuongId, bool isDinhMucBinhThuong = true)
    {
        var query = @"Select 
	pTP.STT,
    pBTP.MaMau,
    pTP.MaUserCan as MaUserTP,
    pBTP.MaUserCan as MaUserBTP,
    pBTP.MaXuong,
    pBTP.Ngay,
    pBTP.MaThe,
    pBTP.STT as STTBTP,
    pBTP.MaMayLangDa,
    pBTP.Gio as GioBTP,
    pTP.Gio,
    pTP.MaMayCan as MayTP,
    pBTP.MaMayCan as MayBTP,
    pBTP.MaLo as LoName,
    la.Ten as LoaiCaName,
    la.Ma as MaLoaiCa,
    s.Ten as SizeName,
    s.Ma as MaSize,
    tp.Ten as ThanhPhamName,
    pTP.ChiSanLuong,
    pBTP.CaTra,
    pBTP.TrongLuong as TrongLuongNhan,
    pTP.TrongLuongBu,
    pTP.TrongLuongTare,
    pTP.TrongLuongTra,
    case
        when @isDinhMucBinhThuong = 1 then(
            case
                when ptp.TrongLuongTra = 0 then 0
                else floor(
                    100 * ptp.TrongLuongNhan / ptp.TrongLuongTra
                ) / 100
            end
        )
        else (
            case
                when (ptp.TrongLuongNhan - ptp.TrongLuongBu) = 0 THEN 0
                else (
                    ptp.TrongLuongTra / (ptp.TrongLuongNhan - ptp.TrongLuongBu)
                )
            end
        )
    end as DinhMucThucTe,
    pTP.DinhMucYeuCau,
    ISNULL(pBTP.MaHoSo, pTP.MaHoSo) as MaHoSo,
    ISNULL(pBTP.MaNhanVien, pTP.MaNhanVien) as MaNhanVien,
    ISNULL(pBTP.NhanVienName, pTP.NhanVienName) as NhanVienName,
    ISNULL(pBTP.Nhom, pTP.Nhom) as Nhom,
	ISNULL(pBTP.MaHoSoPV, pTP.MaHoSoPV) as MaHoSoPhucVuBtp,
	ISNULL(pBTP.MaNhanVienPhucVu, pTP.MaNhanVienPhucVu) as MaNhanVienPhucVuBtp,
    ISNULL(pBTP.NhanVienNamePV, pTP.NhanVienNamePV) as NhanVienNamePhucVuBtp,
    ISNULL(pBTP.Nhompv, pTP.Nhompv) as Nhompv,
    tp.Ma as MaThanhPham,
    pBTP.GhiChu,
    pTP.SuDung,
    pBTP.IsEnabled
from (
        select p.*,
            n.MaHoSo,
            n.Name as NhanVienName,
            n.DeptName0 as Nhom,
			n1.MaHoSo as MaHoSoPV,
			n1.Name as NhanVienNamePV,
            n1.DeptName0 as Nhompv
        from (
                select p.*
                from PhieuCanBTPDinhHinh p
                where Ngay = @ngay
                    and MaXuong = @xuongId
                    and p.IsOffline = 0
            ) p
            LEFT JOIN nhanviendaithanh n on p.MaNhanVien = n.MaNhanVien
			LEFT jOIN NhanVienDaiThanh n1 on p.MaNhanVienPhucVu = n1.MaNhanVien
    ) pBTP
    left join (
        select p.*,
            n.MaHoSo,
            n.Name as NhanVienName,
            n.DeptName0 as Nhom,
			n1.MaHoSo as MaHoSoPV,
			n1.Name as NhanVienNamePV,
            n1.DeptName0 as Nhompv
        from (
                select *
                from PhieuCanTPDinhHinh
                where Ngay = @ngay
                    and MaXuong = @xuongId
                    and IsOffline = 0
            ) p
            LEFT JOIN nhanviendaithanh n on p.MaNhanVien = n.MaNhanVien
			LEFT jOIN NhanVienDaiThanh n1 on p.MaNhanVienPhucVu = n1.MaNhanVien
    ) pTP on pBTP.STT = pTP.STTBTP
    and pBTP.MaMayCan = pTP.MaMayCanBTP
    left join MaLoaiCaDinhHinh la on pBTP.MaLoaiCa = la.Ma
    left join MaSizeDinhHinh s on pBTP.MaSize = s.Ma
    left join MaThanhPhamDinhHinh tp on pBTP.MaThanhPham = tp.Ma
order by pBTP.STT DESC,
    pBTP.Gio DESC,
    pBTP.MaLo,
    tp.ma,
    s.Ma";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { ngay = dateTime.Date, xuongId, isDinhMucBinhThuong })
                .Result
                .ToList();
            return items;
        }
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
	p.MaNhanVien,n.MaHoSo,n.[Name] as TenNhanVien,la.Ten As LoaiCaName,s.Ten As SizeName,tp.Ten as ThanhPhamName,tp.Ma As MaThanhPham,ma.Ten as MauName,p.CaTra,Sum(p.TrongLuongNhan) as TrongLuongNhan,Sum(p.TrongLuongTra) as TrongLuongTra,(FLOOR( (Sum(p.TrongLuongNhan)/Sum(p.TrongLuongTra)) *1000 )/1000) as DinhMucThucTe,p.DinhMucYeuCau,Sum( p.SoRo) As SoRo 
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
			PhieuCanTPDinhHinh p,
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
							PhieuCanTPDinhHinh p,MaThanhPhamDinhHinh tp 
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
									DinhMucDinhHinh d 
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
	MaThanhPhamDinhHinh tp,
	MaSizeDinhHinh s, 
	MaLoaiCaDinhHinh la,
	MaMauDinhHinh ma, 
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
    public List<TEntity> GetPhieuCanTongHopTinhLuong<TEntity>(DateTime dateTime, string xuongId, string khuVucId)
    {
        try
        {
            var query =
                @"Select
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
    Cast(Sum(p.TrongLuongNhan) as decimal(18, 3)) as TrongLuongNhan,
    SUM(p.TrongLuongTraOrg) as TrongLuongTraOrg,
    CAST(Sum(p.TrongLuongTra) as decimal(18, 3)) as TrongLuongTra,
    (
        FLOOR(
            (
                Sum(p.TrongLuongNhanOrg) / Sum(p.TrongLuongTraOrg)
            ) * 1000
        ) / 1000
    ) as DinhMucThucTeOrg,
    (
        FLOOR(
            (Sum(p.TrongLuongNhan) / Sum(p.TrongLuongTra)) * 1000
        ) / 1000
    ) as DinhMucThucTe,
    p.DinhMucYeuCau,
    SUM(p.SoRoOrg) as SoRoOrg,
    Sum(p.SoRo) As SoRo
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
            Cast(p.SoRo * tp.TyLe as int) as SoRo
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
                    Count(*) as SoRo
                from
                    PhieuCanTPDinhHinh p,
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
                                    PhieuCanTPDinhHinh p,
                                    MaThanhPhamDinhHinh tp
                                where
                                    Ngay = @ngay
                                    and MaXuong = @xuongId
                                    and p.MaThanhPham = tp.Ma
                                    and p.MaLoaiCa = tp.MaCa
                                    and p.SuDung =1
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
                                            DinhMucDinhHinh d
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
                    and p.STT > 0 and p.SuDung = 1
                group by
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaMau,
                    p.CaTra,
                    DinhMuc.DinhMuc
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
                            MaThanhPhamDinhHinh tp,
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
            MaThanhPhamDinhHinh _tp
        where
            p.MaThanhPham = tp.MaThanhPhamOrg
            and p.MaLo = tp.MaLo
            and p.MaThanhPham = _tp.ma
    ) p,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaLoaiCaDinhHinh la,
    MaMauDinhHinh ma,
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
    p.TyLe
order by
    n.MaHoSo,
    p.MaThanhPhamOrg,
    p.TyLe,
    p.DinhMucYeuCau";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<TEntity>(query, new { ngay = dateTime.Date, xuongId, khuVucId })
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
    ///     Cach tinh theo ty le tron ca ap dung ty le dinh muc dau rot
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="dateTime"></param>
    /// <param name="xuongId"></param>
    /// <param name="khuVucId"></param>
    /// <returns></returns>
    public List<TEntity> GetPhieuCanTongHopTinhLuong2<TEntity>(DateTime dateTime, string xuongId, string khuVucId)
    {
        try
        {
            var query =
                @"
Select p.MaNhanVien,
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
    Cast(Sum(p.TrongLuongNhan) as decimal(18, 3)) as TrongLuongNhan,
    SUM(p.TrongLuongTraOrg) as TrongLuongTraOrg,
    CAST(Sum(p.TrongLuongTra) as decimal(18, 3)) as TrongLuongTra,
    (
        cast(
            FLOOR(
                (
                  ( case when  Sum(p.TrongLuongTraOrg) = 0 then 0 else  Sum(p.TrongLuongNhanOrg) / Sum(p.TrongLuongTraOrg) end)
                ) * 1000
            ) / 1000 as decimal(18, 2)
        )
    ) as DinhMucThucTeOrg,
    (
        cast(
            FLOOR(
                (( case when  Sum(p.TrongLuongTra) = 0 then 0 else  Sum(p.TrongLuongNhan) / Sum(p.TrongLuongTra) end )) * 1000
            ) / 1000 as decimal(18, 2)
        )
    ) as DinhMucThucTe,
    p.DinhMucYeuCau,
    SUM(p.SoRoOrg) as SoRoOrg,
    Sum(p.SoRo) As SoRo
from (
        Select p.MaNhanVien,
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
            Cast(p.SoRo * tp.TyLe as int) as SoRo
        from (
                SELECT p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaMau,
                    p.CaTra,
                    p.DinhMucYeuCau,
                    Sum(
                        case
                            when p.TrongLuongTra >= p.TrongLuongYeuCau
                            and TyLe > TyLeDau then p.TrongLuongYeuCau
                            when p.TrongLuongTra < p.TrongLuongYeuCau
                            and TyLe > TyLeRot then 0
                            else p.TrongLuongTra
                        end
                    ) as TrongLuongTra,
                    Sum(
                        case
                            when p.TrongLuongTra >= p.TrongLuongYeuCau
                            and TyLe > TyLeDau then p.TrongLuongNhan
                            when p.TrongLuongTra < p.TrongLuongYeuCau
                            and TyLe > TyLeRot then 0
                            else p.TrongLuongNhan
                        end
                    ) as TrongLuongNhan,
                    Sum(
                        case
                            when p.TrongLuongTra >= p.TrongLuongYeuCau
                            and TyLe > TyLeDau then 1
                            when p.TrongLuongTra < p.TrongLuongYeuCau
                            and TyLe > TyLeRot then 0
                            else 1
                        end
                    ) as SoRo
                from (
                        Select p.MaNhanVien,
                            p.MaLo,
                            p.MaLoaiCa,
                            p.MaSize,
                            CASE
                                WHEN p.CaTra = 1 THEN 'B'
                                ELSE p.MaThanhPham
                            END as MaThanhPham,
                            p.MaMau,
                            p.CaTra,
                            p.TrongLuongNhan,
                            p.TrongLuongTra,
                          CAST(  p.TrongLuongNhan / DinhMuc.DinhMuc  as decimal(18,2))as TrongLuongYeuCau,
                            DinhMuc.DinhMuc as DinhMucYeuCau,
                            DinhMuc.TyLeDau,
                            DinhMuc.TyLeRot,
                            cast(
                                case
                                    when p.TrongLuongTra = 0 then 0
                                    else ABS(
                                        (
                                            (p.TrongLuongNhan / DinhMuc.DinhMuc) / p.TrongLuongTra
                                        ) - 1
                                    )
                                end as decimal(18, 4)
                            ) as TyLe,
                            cast(
                                case
                                    when p.TrongLuongTra = 0 then 0
                                    else p.TrongLuongNhan / p.TrongLuongTra
                                end as decimal(18, 2)
                            ) as DinhMucThuc
                        from PhieuCanTPDinhHinh p,
                            (
                                Select tp1.MaLo,
                                    tp1.MaLoaiCa,
                                    tp1.MaSize,
                                    tp1.MaMau,
                                    tp1.MaThanhPham,
                                    tp1.CaTra,
                                    CASE
                                        WHEN tp2.DinhMuc is Null THEN tp1.DinhMuc
                                        ELSE tp2.DinhMuc
                                    END AS DinhMuc,
                                    case
                                        when tp3.TyLeDau is Null Then tp1.TyLeDinhMucDau
                                        else tp3.TyLeDau
                                    end as TyLeDau,
                                    case
                                        when tp3.TyLeRot is null then tp1.TyLeDinhMucRot
                                        else tp3.TyLeRot
                                    end as TyLeRot
                                from (
                                        Select distinct p.MaLo,
                                            p.MaLoaiCa,
                                            p.MaSize,
                                            p.MaMau,
                                            p.MaThanhPham,
                                            p.CaTra,
                                            CASE
                                                WHEN p.CaTra = 1 THEN 1.37
                                                ELSE tp.DinhMuc
                                            END AS DinhMuc,
                                            tp.TyLeDinhMucDau,
                                            tp.TyLeDinhMucRot
                                        from PhieuCanTPDinhHinh p,
                                            MaThanhPhamDinhHinh tp
                                        where Ngay = @ngay
                                            and MaXuong = @xuongId
                                            and p.MaThanhPham = tp.Ma
                                            and p.MaLoaiCa = tp.MaCa
                                    ) tp1
                                    LEFT JOIN (
                                        Select MaLo,
                                            MaLoaiCa,
                                            MaSize,
                                            MaMau,
                                            MaThanhPham,
                                            CaTra,
                                            DinhMuc
                                        from (
                                                Select d.*,
                                                    ROW_NUMBER() OVER (
                                                        PARTITION BY MaLo,
                                                        MaLoaiCa,
                                                        MaMau,
                                                        MaSize,
                                                        MaThanhPham,
                                                        CaTra
                                                        ORDER BY Gio DESC
                                                    ) AS [ROW NUMBER]
                                                from DinhMucDinhHinh d
                                                where Ngay = @ngay
                                                    And MaXuong = @xuongId
                                            ) dm
                                        Where dm.[ROW NUMBER] = 1
                                    ) tp2 on tp1.MaLo = tp2.MaLo
                                    and tp1.MaLoaiCa = tp2.MaLoaiCa
                                    and tp1.MaSize = tp2.MaSize
                                    and tp1.MaMau = tp2.MaMau
                                    and tp1.MaThanhPham = tp2.MaThanhPham
                                    and tp1.CaTra = tp2.CaTra
                                    LEFT JOIN (
                                        Select *
                                        from (
                                                Select dm.*,
                                                    ROW_NUMBER() OVER (
                                                        PARTITION BY MaThanhPham
                                                        ORDER BY NgayGio DESC
                                                    ) AS [ROW NUMBER]
                                                from MaThanhPhamDinhHinh_TyLe dm
                                                where cast(NgayGio as Date) <= @ngay
                                                        and MaXuong = @xuongId
                                            ) dm
                                        where dm.[ROW NUMBER] = 1
                                    ) tp3 ON tp1.MaThanhPham = tp3.MaThanhPham
                            ) DinhMuc
                        where p.Ngay = @ngay
                            and p.MaXuong = @xuongId
                            and p.MaLo = DinhMuc.MaLo
                            and p.MaLoaiCa = DinhMuc.MaLoaiCa
                            and p.MaSize = DinhMuc.MaSize
                            and p.MaMau = DinhMuc.MaMau
                            and p.MaThanhPham = DinhMuc.MaThanhPham
                            And p.CaTra = DinhMuc.CaTra
                            and p.STT > 0
                    ) p
                GROUP By p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaMau,
                    p.CaTra,
                    p.DinhMucYeuCau
            ) p,
            (
                select (@ngay) as Ngay,
                    (@khuVucId) as MaKhuVuc,
                    (@xuongId) as MaXuong,
                    ISNULL(tppt.MaLo, tp.MaLo) as MaLo,
                    tp.MaThanhPham as MaThanhPhamOrg,
                    ISNULL(tppt.MaThanhPhamDes, tp.MaThanhPham) as MaThanhPhamDes,
                    ISNULL(tppt.TyLe, 1) as TyLe
                from (
                        Select (@ngay) as Ngay,
                            (@xuongId) as MaXuong,
                            p.MaLo,
                            tp.Ma as MaThanhPham
                        from MaThanhPhamDinhHinh tp,
                            (
                                Select distinct MSL as MaLo
                                from PhieuCanNguyenLieu
                                where Ngay = @ngay
                                    and MaXuongSanXuat = @xuongId
                            ) p
                    ) tp
                    LEFT join (
                        Select *
                        from MaThanhPham_PhoiTron
                        where Ngay = @ngay
                            and MaXuong = @xuongId
                            and MaKhuVuc = @khuVucId
                    ) tppt ON tp.MaThanhPham = tppt.MaThanhPhamOrg
                    and tp.Ngay = tppt.Ngay
                    and tp.MaXuong = tppt.MaXuong
                    and tp.MaLo = tppt.MaLo
            ) tp,
            MaThanhPhamDinhHinh _tp
        where p.MaThanhPham = tp.MaThanhPhamOrg
            and p.MaLo = tp.MaLo
            and p.MaThanhPham = _tp.ma
    ) p,
    MaThanhPhamDinhHinh tp,
    MaSizeDinhHinh s,
    MaLoaiCaDinhHinh la,
    MaMauDinhHinh ma,
    NhanVienDaiThanh n
where p.MaThanhPham = tp.Ma
    and p.MaLoaiCa = tp.MaCa
    and p.MaLoaiCa = la.Ma
    and p.MaSize = s.Ma
    and p.MaMau = ma.Ma
    and p.MaNhanVien = n.MaNhanVien
group by p.MaNhanVien,
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
    p.TyLe
order by n.MaHoSo,
    p.MaThanhPhamOrg,
    p.TyLe,
    p.DinhMucYeuCau";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<TEntity>(query, new { ngay = dateTime.Date, xuongId, khuVucId })
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

    public List<T> Gets<T>()
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var rows = connection.Query<T>(qrGetAll).ToList();
        return rows;
    }

    public List<T> Gets<T>(DateTime dateTime)
    {
        var query = @"SELECT
    p.*,
    ISNULL(tp.BravoId, 'NONE') as BravoId
from
    (
        Select
            *
        from
            PhieuCanTPDinhHinh p
        Where
            p.Ngay = @ngay and p.STT>0
    ) p
    left join MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date }).Result.ToList();
        return items;
    }

    public List<T> Gets<T>(DateTime dateTime, string xuongId)
    {
        var query =
            @"Select * from PhieuCanTPDinhHinh Where Ngay = @ngay and MaXuong=@xuongId order by STT DESC";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result
                .ToList();
            return items;
        }
    }

    public List<T> Gets<T>(DateTime dateTime, string mayCanId, int stt)
    {
        try
        {
            var query = "Select * from PhieuCanTPDinhHinh where Ngay =@ngay and MaMayCan = @mayCanId and STT >@stt";
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
        var query =
            @"Select * from PhieuCanTPDinhHinh Where Ngay = @ngay and MaXuong=@xuongId and MaMayCan= @mayCanId order by STT DESC";
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
            @"Select * from PhieuCanTPDinhHinh Where Ngay = @ngay and MaXuong=@xuongId and MaMayCan= @mayCanId and STT >@stt order by STT DESC";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { ngay = dateTime.Date, xuongId, mayCanId, stt })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> Gets_mayCan<T>(DateTime dateTime, string mayCan)
    {
        var query =
            @"Select * from PhieuCanTPDinhHinh Where Ngay = @ngay and MaMayCan=@maycan order by STT DESC";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, mayCan }).Result
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
            $@"Select Isnull(sum(p.TrongLuongTra),0) from PhieuCanTPDinhHinh p where p.Ngay=@ngay and p.MaXuong = @xuongId and p.MaThanhPham in {listOfIdsJoined} and p.Gio >= @fromTime and p.Gio < @toTime ";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var item = (decimal)connection.ExecuteScalar(
                query,
                new { ngay = dateTime.Date, fromTime, toTime, xuongId });
            return item;
        }
    }

    public decimal GetSanLuongTras(
        DateTime dateTime,
        string xuongId,
        List<Models.Repos.Models.MaThanhPhamExDinhHinh> thanhPhamExDinhHinhs)
    {
        try
        {
            var idsEx = thanhPhamExDinhHinhs.Select(x => x.Id).Distinct();
            var query =
                @"Select isnull( Sum(TrongLuongTra),0) from PhieuCanTPDinhHinh where Ngay=@ngay and MaXuong=@xuongId  and MaThanhPham Not In @idsEx";
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
        List<Models.Repos.Models.MaThanhPhamExDinhHinh> thanhPhamExDinhHinhs)
    {
        try
        {
            var ids = nhanVienDaiThanhs.Select(x => x.MaNhanVien).Distinct();
            var listOfIdsJoined = "('" + string.Join("','", ids.ToArray()) + "')";
            var idsEx = thanhPhamExDinhHinhs.Select(x => x.Id).Distinct();
            var query =
                $@"Select isnull( Sum(TrongLuongTra),0) from PhieuCanTPDinhHinh where Ngay=@ngay and MaXuong=@xuongId and MaNhanVien In {listOfIdsJoined} and MaThanhPham Not In @idsEx";
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
                    PhieuCanTPDinhHinh With(NOLOCK)
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
                            PhieuCanTPDinhHinh p,
                            MaThanhPhamDinhHinh tp
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
                                    DinhMucDinhHinh d
                                where
                                    d.Ngay = @ngay
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
            LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
    ) p,
    PhieuCanBTPDinhHinh btp
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

    public Tuple<int, decimal> GetSoRoTongTrongLuong(DateTime dateTime, string mayCanId, bool chiSanLuong = false)
    {
        var query = @"Select
    Count(*) as Item1,
    ISNULL(Sum(TrongLuongTra), 0) As Item2
    
from
    PhieuCanTPDinhHinh WITH(READPAST)
where
    ChiSanLuong = @chiSanLuong
    and MaMayCan = @mayCanId
    and Ngay = @ngay";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var row = connection.Query<Tuple<int, decimal>>(
                query,
                new { ngay = dateTime.Date, mayCanId, chiSanLuong })
            .SingleOrDefault();
        return row;
    }

    public Tuple<int, decimal> GetSoRoTongTrongLuongByNhanVienId(DateTime dateTime, string nhanVienId)
    {
        var query =
            @"Select IsNull( Count(*),0) as Item1,isNull( Sum(TrongLuongTra),0) As Item2 from PhieuCanTPDinhHinh WITH(READPAST) where MaNhanVien = @nhanVienId and Ngay =@ngay";
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

    public decimal GetTongTrongLuongThanhPhamByNhanVienId(
    DateTime dateTime, 
    string nhanVienId,
    string maThanhPham)
{
    var query = @"
        SELECT ISNULL(SUM(TrongLuongTra), 0)
        FROM PhieuCanTPDinhHinh WITH (READPAST)
        WHERE MaNhanVien = @nhanVienId
          AND Ngay = @ngay
          AND MaThanhPham = @maThanhPham";

    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
        var result = connection.ExecuteScalar<decimal>(
            query,
            new { ngay = dateTime.Date, nhanVienId, maThanhPham });

        return result;
    }
}
    public decimal GetTongTrongLuongThanhPhamNhanByNhanVienId(
    DateTime dateTime, 
    string nhanVienId,
    string maThanhPham)
{
    var query = @"
        SELECT ISNULL(SUM(TrongLuongNhan), 0)
        FROM PhieuCanTPDinhHinh WITH (READPAST)
        WHERE MaNhanVien = @nhanVienId
          AND Ngay = @ngay
          AND MaThanhPham = @maThanhPham";

    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
        var result = connection.ExecuteScalar<decimal>(
            query,
            new { ngay = dateTime.Date, nhanVienId, maThanhPham });

        return result;
    }
}

    public Tuple<int, decimal, int, decimal> GetSoRoTongTrongLuongByNhanVienId(
        DateTime dateTime,
        string nhanVienId,
        string thanhPhamId,
        bool chiSanLuong = false)
    {
        var query = @"Select
    Count(*) as Item1,
    ISNULL(Sum(TrongLuongTra), 0) As Item2,
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
                when MaThanhPham = @thanhPhamId then TrongLuongTra
                else 0
            end
        ),
        0
    ) as Item4
from
    PhieuCanTPDinhHinh WITH(READPAST)
where
    ChiSanLuong = @chiSanLuong
    and MaNhanVien = @nhanVienId
    and Ngay = @ngay";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var row = connection.Query<Tuple<int, decimal, int, decimal>>(
                    query,
                    new { ngay = dateTime.Date, nhanVienId, thanhPhamId, chiSanLuong })
                .SingleOrDefault();
            return row;
        }
    }

    public Tuple<int, decimal, int, decimal, decimal> GetSoRoTongTrongLuongNhanTraByNhanVienId(
        DateTime dateTime,
        string nhanVienId,
        string thanhPhamId,
        bool chiSanLuong = false)
    {
        var query = @"Select
    Count(*) as Item1,
    ISNULL(Sum(TrongLuongTra), 0) As Item2,
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
                when MaThanhPham = @thanhPhamId then TrongLuongTra
                else 0
            end
        ),
        0
    ) as Item4,
ISNULL(
        Sum(
            case
                when MaThanhPham = @thanhPhamId  then TrongLuongNhan
                else 0
            end
        ),
        0
    ) as Item5
from
    PhieuCanTPDinhHinh WITH(READPAST)
where
    ChiSanLuong = @chiSanLuong
    and MaNhanVien = @nhanVienId
    and Ngay = @ngay";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var row = connection.Query<Tuple<int, decimal, int, decimal, decimal>>(
                    query,
                    new { ngay = dateTime.Date, nhanVienId, thanhPhamId, chiSanLuong })
                .SingleOrDefault();
            return row;
        }
    }

    public IList<string> GetSqlsInBatches(IList<Models.Repos.Models.PhieuCanTPDinhHinh> phieuCans)
    {
        var insertSql = @"INSERT INTO [dbo].[PhieuCanTPDinhHinh]
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
           ,[GhiChu]
           ,[ChiSanLuong],[SuDung],[TrongLuongTare],[TrongLuongBu],[IsOffline])
     VALUES";
        var valuesSql =
            @"({0},'{1}','{2}', '{3}', '{4}', '{5}', '{6}', '{7}','{8}','{9}','{10}',{11}, {12}, {13}, {14}, '{15}','{16}',{17}, {18}, '{19}', '{20}',{21},{22},{23},{24},{25})";
        var batchSize = 1000;

        var sqlsToExecute = new List<string>();
        var numberOfBatches = (int)Math.Ceiling((double)phieuCans.Count / batchSize);

        for (var i = 0; i < numberOfBatches; i++)
        {
            var phieuCanToInsert = phieuCans.Skip(i * batchSize).Take(batchSize);
            var valuesToInsert = phieuCanToInsert.Select(x => string.Format(
                valuesSql,
                x.STT,
                x.Ngay.ToString("yyyy-MM-dd"),
                x.Gio.ToString(@"hh\:mm\:ss"),
                x.MaUserCan,
                x.MaLoaiCa,
                x.MaMau,
                x.MaSize,
                x.MaThanhPham,
                x.MaLo,
                x.MaThe,
                x.TrongLuongNhan,
                x.TrongLuongTra,
                x.DinhMucThucTe,
                x.DinhMucYeuCau,
                x.MaXuong,
                x.MaNhanVien,
                x.CaTra ? 1 : 0,
                x.STTBTP,
                x.MaMayCanBTP,
                string.Empty,
                x.ChiSanLuong ? 1 : 0,
                //x.SuDung ? 1 : 0,
                x.TrongLuongTare,
                x.TrongLuongBu,
                x.IsOffline ? 1 : 0));
            sqlsToExecute.Add(insertSql + string.Join(",", valuesToInsert));
        }

        return sqlsToExecute;
    }

    public List<T> GetsTongHopMayCan<T>(DateTime dateTime, string xuongId)
    {
        var query =
            @"Select MaMayCan,SUM(TrongLuongTra) as TrongLuongTra from PhieuCanTPDinhHinh where Ngay=@Ngay and MaXuong = @xuongId Group by MaMayCan order by MaMayCan ";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.Query<T>(query, new { Ngay = dateTime.Date, xuongId }).ToList();
            return items;
        }
    }


    public List<T> GetsTPSoft<T>(DateTime dateTime, string xuongId)
    {
        var query =
            @"Select p.MaMayCan+'-'+p.MaXuong+'-K01-'+FORMAT(p.Ngay,'yyyy-MM-dd')+'-'+FORMAT(CAST(p.Gio as datetime),'hh:mm:ss') as [ID],p.TrongLuongTra as [TrongLuong],sp.Id as [CongDoanID],ISNULL(n.Tel,'') as [CMND],GETDATE() as [DateCreate],GetDate() as [DateSync],1 as [Status], cast( p.Ngay as datetime) + cast(p.gio as datetime) as [ThoiGian]  from PhieuCanTPDinhHinh p, MaSanPhamDinhHinhBravo sp, NhanVienDaiThanh n where Ngay=@ngay and MaXuong =@xuongId and p.MaThanhPham = sp.DaiThanhId and p.MaNhanVien = n.MaNhanVien";
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
            @"Select Top(20) p.STT,p.Gio,p.MaThe as TheId,p.MaLo,p.MaNhanVien,n.MaHoSo,n.Name as TenNhanVien,p.MaThanhPham,tp.Ten as TenThanhPham,p.CaTra,p.MaSize,s.Ten as TenSize,p.TrongLuongTra from PhieuCanTPDinhHinh p,NhanVienDaiThanh n,MaThanhPhamDinhHinh tp,MaSizeDinhHinh s Where p.Ngay = @ngay and p.MaXuong=@xuongId and p.MaMayCan= @mayCanId and p.MaNhanVien = n.MaNhanVien and p.MaThanhPham = tp.Ma and p.MaSize = s.Ma order by p.STT DESC";
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

    public List<T> GetTongHopHaoHut<T>(DateTime fromDate, DateTime @toDate)
    {
        var query = @"
Select
    p.*,
    CASE
        when p.MaThanhPham = 'CTR' then N'Cá Trắng'
        when p.MaThanhPham = 'CDA' then N'Cá Da'
        when p.MaThanhPham = 'CDO' then N'Cá Đỏ'
        when p.MaThanhPham = 'CDV' then N'Cá Vanh Dè'
        else ''
    end as Ten,

    CASE
        when p.MaThanhPham = 'CTR' then p.TrongLuong - (
            isnull(GiaoXKCTR.TrongLuong, 0) + isnull(CTM.TrongLuong, 0) + isnull(FLDatKT.TrongLuong, 0) + isnull(FLDatKTTTrangMang.TrongLuong, 0) + isnull(vungot.TrongLuong, 0)
        )
        when p.MaThanhPham = 'CDA' then p.TrongLuong - (
            isnull(GiaoXKCDA.TrongLuong, 0) 
        )
        when p.MaThanhPham = 'CDO' then p.TrongLuong - (isnull(GiaoXKCDO.TrongLuong, 0)+ isnull(FLDatKTTThitDo.TrongLuong, 0))
        when p.MaThanhPham = 'CDV' then p.TrongLuong - isnull(GiaoXKCVD.TrongLuong, 0)
        else 0
    end as HaoHut,
    GiaoXKCTR.TrongLuong as TrongLuongGiaoXKCTR,
    CTM.TrongLuong as TrongLuongCTM,
    FLDatKT.TrongLuong as TrongLuongFLDatKT,
    FLDatKTTTrangMang.TrongLuong as TrongLuongFLDatKTTTrangMang,
    vungot.TrongLuong as TrongLuongVungOt

from
    (
        Select
            *
        from
            (
                Select
                    p.Ngay,
                    tp.BravoId as MaThanhPham,
                    ISNULL(sum(TrongLuongTra), 0) as TrongLuong
                from
                    PhieuCanTPDinhHinh p,
                    MaThanhPhamDinhHinh tp
                where
                    p.Ngay >= @fromDate
                    and p.Ngay <= @toDate
                    and p.MaThanhPham = tp.Ma
                    and tp.BravoId != 'CTM'
                GROUP BY
                    tp.BravoId,
                    p.Ngay
            ) p
    ) p
    LEFT JOIN (
        Select
            p.NgayNguyenLieu,
            ISNULL(sum(TrongLuong), 0) as TrongLuong
        from
            PhieuCanChinhXepKhuon p,
            MaThanhPhamChinhXepKhuon tp
        where
            NgayNguyenLieu >= @fromDate
            and NgayNguyenLieu <= @toDate
            and p.MaThanhPhamChinh = tp.Ma
            and tp.BravoId = 'VG'
        group by
            p.NgayNguyenLieu
    ) vungot ON p.Ngay = vungot.NgayNguyenLieu
    LEFT JOIN (
        Select
            p.Ngay,
            ISNULL(sum(TrongLuong), 0) as TrongLuong
        from
            PhieuCanNguyenLieu p
        where
            p.Ngay >= @fromDate
            and p.Ngay <= @toDate
            and p.MaLoaiThanhPham in ('PLDKTTD', 'Pldkttdut')
        group by
            p.Ngay
    ) FLDatKTTThitDo ON p.Ngay = FLDatKTTThitDo.Ngay
    LEFT JOIN (
        Select
            p.Ngay,
            ISNULL(sum(TrongLuong), 0) as TrongLuong
        from
            PhieuCanNguyenLieu p
        where
            p.Ngay >= @fromDate
            and p.Ngay <= @toDate
            and p.MaLoaiThanhPham = 'PLDKTtTM'
        group by
            p.Ngay
    ) FLDatKTTTrangMang ON p.Ngay = FLDatKTTTrangMang.Ngay
    LEFT JOIN (
        Select
            p.Ngay,
            ISNULL(sum(TrongLuong), 0) as TrongLuong
        from
            PhieuCanNguyenLieu p
        where
            p.Ngay >= @fromDate
            and p.Ngay <= @toDate
            and p.MaLoaiThanhPham = 'pLDKT'
        group by
            p.Ngay
    ) FLDatKT on p.Ngay = FLDatKT.Ngay
    LEFT JOIN (
        Select
            p.Ngay,
            ISNULL(sum(TrongLuong), 0) as TrongLuong
        from
            PhieuCanBTPDinhHinh p,
            MaThanhPhamDinhHinh tp
        where
            Ngay >= @fromDate
            and Ngay <= @toDate
            and p.MaThanhPham = tp.Ma
            and tp.BravoId = 'CTM'
        group by
            p.Ngay
    ) CTM on p.Ngay = CTM.Ngay
    LEFT JOIN (
        Select
            p.NgayNguyenLieu,
            ISNULL(sum(TrongLuong), 0) as TrongLuong
        from
            PhieuCanChinhXepKhuon p,
            MaThanhPhamChinhXepKhuon tp
        where
            NgayNguyenLieu >= @fromDate
            and NgayNguyenLieu <= @toDate
            and p.MaThanhPhamChinh = tp.Ma
            and tp.BravoId = 'CTR'
        group by
            p.NgayNguyenLieu
    ) GiaoXKCTR on p.Ngay = GiaoXKCTR.NgayNguyenLieu
    LEFT JOIN (
        Select
            p.NgayNguyenLieu,
            ISNULL(sum(TrongLuong), 0) as TrongLuong
        from
            PhieuCanChinhXepKhuon p,
            MaThanhPhamChinhXepKhuon tp
        where
            NgayNguyenLieu >= @fromDate
            and NgayNguyenLieu <= @toDate
            and p.MaThanhPhamChinh = tp.Ma
            and tp.BravoId = 'CDA'
        group by
            p.NgayNguyenLieu
    ) GiaoXKCDA on p.Ngay = GiaoXKCDA.NgayNguyenLieu
    LEFT JOIN (
        Select
            p.NgayNguyenLieu,
            ISNULL(sum(TrongLuong), 0) as TrongLuong
        from
            PhieuCanChinhXepKhuon p,
            MaThanhPhamChinhXepKhuon tp
        where
            NgayNguyenLieu >= @fromDate
            and NgayNguyenLieu <= @toDate
            and p.MaThanhPhamChinh = tp.Ma
            and tp.BravoId = 'CDO'
        group by
            p.NgayNguyenLieu
    ) GiaoXKCDO on p.Ngay = GiaoXKCDO.NgayNguyenLieu
    LEFT JOIN (
        Select
            p.NgayNguyenLieu,
            ISNULL(sum(TrongLuong), 0) as TrongLuong
        from
            PhieuCanChinhXepKhuon p,
            MaThanhPhamChinhXepKhuon tp
        where
            NgayNguyenLieu >= @fromDate
            and NgayNguyenLieu <= @toDate
            and p.MaThanhPhamChinh = tp.Ma
            and tp.BravoId = 'CDV'
        group by
            p.NgayNguyenLieu
    ) GiaoXKCVD on p.Ngay = GiaoXKCVD.NgayNguyenLieu
    ";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.Query<T>(query, new { fromDate, toDate }).ToList();
            return items;
        }

    }
    public List<T> GetTongHopDanhGiaDinhMucs<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = true,
        bool isFloor = true)
    {
        var query = @"Select
    *
from
    (
        SELECT
            p.Ngay,
            p.MaNhanVien,
            p.MaHoSo,
            p.NhanVienName as TenNhanVien,
            p.Nhom,
            cast(
                DATEDIFF(MINUTE, p.minGio, p.maxGio) / 60.0 as decimal(18, 3)
            ) as SoGio,
            p.IsGiaCong,
            p.MaLo,
            p.MaLoaiCa,
            p.LoaiCaName,
            p.MaSize,
            p.SizeName,
            p.MaThanhPham,
            p.ThanhPhamName,
            p.MaSanPham,
            p.MaMau,
            p.MauName,
            p.CaTra,
            p.ChiSanLuong,
            p.TrongLuongNhan,
            p.TrongLuongTra,
            round(
                Cast(
                    isnull(
                        case
                            when @isDinhMucBinhThuong = 1 then(p.DinhMuc)
                            else (
                                case
                                    when p.DinhMuc > 1 THEN 1
                                    else (
                                        p.DinhMuc
                                    )
                                end
                            )
                        end,
                        0
                    ) as float
                ),
                4
            ) as DinhMuc,
            p.DinhMucChuan,
            cast(p.DanhGia as Int) as DanhGia,
			(case
				when p.DanhGia = 1 then N'Đậu'
				else N'Rớt'
			end) as DanhGia2,
            p.DonGia,
            p.ThanhTien,
            p.SoRo
        from
            (
                Select
                    p.Ngay,
                    p.MaNhanVien,
                    n.MaHoSo,
                    n.Name as NhanVienName,
                    n.DeptName0 as Nhom,
                    n.IsGiaCong,
                    p.MaLo,
                    p.MaLoaiCa,
                    la.Ten as LoaiCaName,
                    p.MaSize,
                    s.Ten as SizeName,
                    p.MaThanhPham,
                    tp.Ten as ThanhPhamName,
                    tp.BravoId as MaSanPham,
                    p.MaMau,
                    ma.Ten as MauName,
                    p.CaTra,
                    p.ChiSanLuong,
                    p.maxGio,
                    p.minGio,
                    Sum(p.TrongLuongNhan) as TrongLuongNhan,
                    Sum(p.TrongLuongTra) as TrongLuongTra,
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
                                else ROUND(
                                    sum(p.TrongLuongTra) / sum(p.TrongLuongNhan - p.TrongLuongBu),
                                    4
                                )
                            end as decimal(18, 4)
                        )
                    end as DinhMuc,
                    DinhMuc.DinhMuc as DinhMucChuan,
                    (
                        case
                            when @isDinhMucBinhThuong = 1 then cast (
                                case
                                    when sum(p.TrongLuongTra) = 0 then 0
                                    when (
                                        case
                                            when @isFloor = 1 then (
                                                FLOOR(
                                                    (Sum(p.TrongLuongNhan) / sum(p.TrongLuongTra)) * 1000
                                                ) / 1000
                                            )
                                            else Sum(p.TrongLuongNhan) / sum(p.TrongLuongTra)
                                        end
                                    ) <= DinhMuc.DinhMuc then 1
                                    else 0
                                end as bit
                            )
                            ELSE cast (
                                case
                                    when sum(p.TrongLuongNhan) = 0 then 0
                                    when (
                                        ROUND(
                                            sum(p.TrongLuongTra) / sum(p.TrongLuongNhan - p.TrongLuongBu),
                                            4
                                        )
                                    ) >= DinhMuc.DinhMuc then 1
                                    else 0
                                end as bit
                            )
                        end
                    ) as DanhGia,
                    0 as DonGia,
                    0 as ThanhTien,
                    Count(*) as SoRo
                from
                    (
                        Select
                            p.*,
                            Max(p.Gio) over (partition by p.MaNhanVien, p.Ngay) as maxGio,
                            Min(btp.Gio) over (partition by btp.MaNhanVien, btp.Ngay) as minGio
                        from
                            PhieuCanTPDinhHinh p
                             left join PhieuCanBTPDinhHinh btp on p.Ngay = btp.Ngay
                            and p.MaXuong = btp.MaXuong and p.MaNhanVien = btp.MaNhanVien and p.STTBTP = btp.STT and p.MaMayCanBTP =btp.MaMayCan
                        where
                            p.Ngay <= @toDate
                            and p.Ngay >= @fromDate
                            and p.MaXuong = @xuongId
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
                                    PhieuCanTPDinhHinh p,
                                    MaThanhPhamDinhHinh tp
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
                                            DinhMucDinhHinh d
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
                    LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
                    LEFT Join MaSizeDinhHinh s on p.MaSize = s.Ma
                    LEFT Join MaLoaiCaDinhHinh la on p.MaLoaiCa = la.Ma
                    LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
                    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
                where
                    p.STT > 0
                    and p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                group by
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaMau,
                    p.CaTra,
                    DinhMuc.DinhMuc,
                    p.Ngay,
                    p.ChiSanLuong,
                    n.MaHoSo,
                    n.Name,
                    n.IsGiaCong,
                    la.Ten,
                    tp.Ten,
                    tp.BravoId,
                    ma.Ten,
                    n.DeptName0,
                    s.Ten,
                    p.maxGio,
                    p.minGio
            ) p
    ) p";
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

    public List<T> GetTongHopDinhMuc<T>(DateTime dateTime, string xuongId)
    {
        var query =
            @"SELECT p.MaLo, la.Ten As LoaiCaName,tp.Ma As MaThanhPham,tp.Ten As ThanhPhamName,p.CaTra,COUNT(p.STT) As SoRo,SUM(p.TrongLuongTra) as TrongLuongTra,SUM(p.TrongLuongNhan) as TrongLuongNhan,FLOOR((SUM(p.TrongLuongNhan)/SUM(p.TrongLuongTra)) *1000)/1000 as DinhMuc from PhieuCanTPDinhHinh p,MaThanhPhamDinhHinh tp,MaLoaiCaDinhHinh la where p.MaLoaiCa = la.Ma and p.MaLoaiCa = tp.MaCa and p.MaThanhPham = tp.Ma and p.Ngay=@ngay and p.MaXuong =@xuongId  group by p.MaLo, la.Ten,tp.Ten,tp.Ma,p.CaTra order by p.MaLo,tp.Ma";
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
                (SUM(p.TrongLuongNhan) / SUM(p.TrongLuongTra)) * 1000
            ) / 1000 as DinhMuc,
            dm.DinhMuc as DinhMucChuan
        from
            PhieuCanTPDinhHinh p,
            MaThanhPhamDinhHinh tp,
            MaLoaiCaDinhHinh la,
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
                            DinhMucDinhHinh d
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
            [PMS].[dbo].[PhieuCanNguyenLieu] p,
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

    public List<T> GetTongHopDinhMuc<T>(
        DateTime dateTime,
        string xuongId,
        DateTime fromTime,
        DateTime toTime,
        bool isDinhMucBinhThuong = true)
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
            case
                when @isDinhMucBinhThuong = 1 then FLOOR(
                    (SUM(p.TrongLuongNhan) / SUM(p.TrongLuongTra)) * 1000
                ) / 1000
                else SUM(p.TrongLuongTra) / SUM(p.TrongLuongNhan)
            end as DinhMuc,
            dm.DinhMuc as DinhMucChuan
        from
            PhieuCanTPDinhHinh p,
            MaThanhPhamDinhHinh tp,
            MaLoaiCaDinhHinh la,
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
                            DinhMucDinhHinh d
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
            [PMS].[dbo].[PhieuCanNguyenLieu] p,
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
                        toTime = toTime.TimeOfDay,
                        isDinhMucBinhThuong
                    })
                .ToList();
            return items;
        }
    }

    /// <summary>
    /// Cái này là Năng Suất Theo Nhóm
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="xuongId"></param>
    /// <returns></returns>
    public List<T> GetTongHopDinhMucSanLuongTheoNhom<T>(DateTime fromDate, DateTime toDate, string xuongId)
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
                    PhieuCanTPDinhhinh p , NhanVienDaiThanh nv
                Where
                    p.Ngay >= @fromDate and p.Ngay <= @toDate and p.MaXuong = @xuongId
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
        PhieuCanTPDinhhinh p
        JOIN NhanVienDaiThanh nv ON p.MaNhanVien = nv.MaNhanVien
    WHERE
        p.Ngay >= @fromDate AND p.Ngay <= @toDate
        AND p.MaXuong = @xuongId
    GROUP BY
        nv.DeptName0, p.MaThanhPham
) p
LEFT JOIN MaThanhPhamDinhHinh tp ON p.MaThanhPham = tp.Ma
order by p.Nhom";
        var items = connection.QueryAsync<T>(
                query,
                new { fromDate = fromDate.Date,toDate = toDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }
    public List<T> GetSanLuongTBTPDinhHinh<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var query = @"SELECT SUM(p.TrongLuongTra) as TrongLuongTra,
COUNT(DISTINCT p.MaNhanVien) as SoLuongNhanVien,
(SUM(p.TrongLuongTra)/COUNT(DISTINCT p.MaNhanVien)) as SanLuongTB
FROM PhieuCanTPDinhHinh p
WHERE p.Ngay >= @fromDate AND p.Ngay <= @toDate
AND p.MaXuong = @xuongId";
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
                    PhieuCanTPDinhhinh
                Where
                    Ngay = @ngay and MaXuong = @xuongId
                GROUP BY
                    MaThanhPham
            ) p
            LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
    ) p";
        var items = connection.QueryAsync<T>(
                query,
                new { ngay = dateTime.Date, xuongId })
            .Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopLoaiThanhPhamByMaHoSos<T>(
        DateTime fromDate,
        DateTime toDate,
        string maHoSo,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = false,
        bool isFloor = true)
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
    CONCAT(
        case
            when p.LoaiSanLuong = -1 then N'Cá Trả-'
            else ''
        end,
        tp.Ten
    ) as ThanhPhamName,
    p.MaMau,
    ma.Ten as MauName,
    p.CaTra,
    p.ChiSanLuong,
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
            PhieuCanTPDinhHinh p,
            NhanVienDaiThanh n
        where
            p.Ngay <= @toDate
            and p.Ngay >= @fromDate
			and p.MaXuong = @xuongId
            and p.manhanvien = n.manhanvien
            and p.IsOffline = 0
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
                    PhieuCanTPDinhHinh p,
                    MaThanhPhamDinhHinh tp
                where
                    p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and MaXuong = @xuongId
                    and p.MaThanhPham = tp.Ma
                    and p.MaLoaiCa = tp.MaCa
                    and p.IsOffline = 0
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
                            DinhMucDinhHinh d
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
    LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
    LEFT Join MaSizeDinhHinh s on p.MaSize = s.Ma
    LEFT Join MaLoaiCaDinhHinh la on p.MaLoaiCa = la.Ma
    LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
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
    p.ChiSanLuong,
    la.Ten,
    tp.Ten,
    ma.Ten,
    s.Ten,
    p.LoaiSanLuong,
	p.MaNhanVien,
	n.MaHoSo,
	n.Name";
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

    public List<T> GetTongHopLoaiThanhPhamByMaNhanViens<T>(
        DateTime fromDate,
        DateTime toDate,
        string maNhanVien,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = false,
        bool isFloor = true)
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
    CONCAT(
        case
            when p.LoaiSanLuong = -1 then N'Cá Trả-'
            else ''
        end,
        tp.Ten
    ) as ThanhPhamName,
    p.MaMau,
    ma.Ten as MauName,
    p.CaTra,
    p.ChiSanLuong,
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
            PhieuCanTPDinhHinh p,
            NhanVienDaiThanh n
        where
            p.Ngay <= @toDate
            and p.Ngay >= @fromDate
			and p.MaXuong = @xuongId
            and p.manhanvien = n.manhanvien
            and p.IsOffline = 0
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
                    PhieuCanTPDinhHinh p,
                    MaThanhPhamDinhHinh tp
                where
                    p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and MaXuong = @xuongId
                    and p.MaThanhPham = tp.Ma
                    and p.MaLoaiCa = tp.MaCa
                    and p.IsOffline = 0
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
                            DinhMucDinhHinh d
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
    LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
    LEFT Join MaSizeDinhHinh s on p.MaSize = s.Ma
    LEFT Join MaLoaiCaDinhHinh la on p.MaLoaiCa = la.Ma
    LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
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
    p.ChiSanLuong,
    la.Ten,
    tp.Ten,
    ma.Ten,
    s.Ten,
    p.LoaiSanLuong,
	p.MaNhanVien,
	n.MaHoSo,
	n.Name";
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

    public List<T> GetTongHopLoaiThanhPhamByMaThes<T>(
        DateTime fromDate,
        DateTime toDate,
        string maThe,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = false,
        bool isFloor = true)
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
    CONCAT(
        case
            when p.LoaiSanLuong = -1 then N'Cá Trả-'
            else ''
        end,
        tp.Ten
    ) as ThanhPhamName,
    p.MaMau,
    ma.Ten as MauName,
    p.CaTra,
    p.ChiSanLuong,
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
            PhieuCanTPDinhHinh p,
            NhanVienDaiThanh n
        where
            p.Ngay <= @toDate
            and p.Ngay >= @fromDate
			and p.MaXuong = @xuongId
            and p.manhanvien = n.manhanvien
            and p.IsOffline = 0
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
                    PhieuCanTPDinhHinh p,
                    MaThanhPhamDinhHinh tp
                where
                    p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and MaXuong = @xuongId
                    and p.MaThanhPham = tp.Ma
                    and p.MaLoaiCa = tp.MaCa
                    and p.IsOffline = 0
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
                            DinhMucDinhHinh d
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
    LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
    LEFT Join MaSizeDinhHinh s on p.MaSize = s.Ma
    LEFT Join MaLoaiCaDinhHinh la on p.MaLoaiCa = la.Ma
    LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
    LEFT JOIN TheTu t on p.MaNhanVien = t.MaNhanVien
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
    p.ChiSanLuong,
    la.Ten,
    tp.Ten,
    ma.Ten,
    s.Ten,
    p.LoaiSanLuong,
	p.MaNhanVien,
	n.MaHoSo,
	n.Name";
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
//        public List<T> GetTongHopLoaiThanhPhams<T>(
//            DateTime fromDate,
//            DateTime toDate, string xuongId)
//        {
//            try
//            {
//                var query = @$"select 
//tp.Ma as MaThanhPham,
//tp.Ten as ThanhPhamName,
//Sum(p.TrongLuongTra) as TrongLuong,
//Count(*) As SoRo
//from PhieuCanTPDinhHinh p,
//MaThanhPhamDinhHinh tp 
//where 
//Ngay <= {toDate.ToString("yyyy-MM-dd")} and
//Ngay >= {fromDate.ToString("yyyy-MM-dd")}
//and p.MaThanhPham = tp.Ma 
//and MaXuong= {xuongId}
//and ISNULL(GhiChu,'') <> 'HUY' 
//Group by 
//tp.Ma,
//tp.Ten";
//                using (var connection = new SqlConnection(connectionString))
//                {
//                    connection.Open();
//                    var items = connection.QueryAsync<T>(query).Result.ToList();
//                    return items;
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

    public List<T> GetTongHopLoaiThanhPhamOfflines<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = false,
        bool isFloor = true)
    {
        var query = @"
select p.Ngay,
    p.LoaiSanLuong,
    CONCAT(
        case
            when p.LoaiSanLuong = -1 then N'Cá Trả-'
            else ''
        end,
        p.ThanhPhamName
    ) as ThanhPhamName,
    SUM(p.TrongLuongNhan) as TrongLuongNhan,
    SUM(p.TrongLuongTra) as TrongLuongTra,
    CASE
        WHEN SUM(p.TrongLuongTra) = 0 then 0
        else cast(
            SUM(p.TrongLuongNhan) / SUM(p.TrongLuongTra) as decimal(18, 2)
        )
    end as DinhMuc
from (
        select p.*,
            n.Name as NhanVienName,
            n.MaHoSo,
            n.DeptCode0 as MaNhom,
            n.DeptName0 as Nhom,
            tp.Ten as ThanhPhamName,
            tp.BravoId as MaSanPham,
            n.LoaiSanLuong
        from (
                select ISNULL(btp.Ngay, tp.Ngay) as Ngay,
                    isnull(btp.MaNhanVien, tp.MaNhanVien) as MaNhanVien,
                    isnull(btp.MaThanhPham, tp.MaThanhPham) as MaThanhPham,
                    isNull (btp.TrongLuongNhan, 0) as TrongLuongNhan,
                    isnull(tp.TrongLuongTra, 0) as TrongLuongTra,
                    CASE
                        WHEN isnull(tp.TrongLuongTra, 0) = 0 then 0
                        else cast(
                            isNull (btp.TrongLuongNhan, 0) / isnull(tp.TrongLuongTra, 0) as decimal(18, 2)
                        )
                    end as DinhMuc,
                    isNull (btp.SoRoNhan, 0) as SoRoNhan,
                    isnull(tp.SoRoTra, 0) as SoRoTra
                from (
                        Select p.Ngay,
                            p.MaNhanVien,
                            p.MaThanhPham,
                            SUM(p.TrongLuong) as TrongLuongNhan,
                            COUNT(p.TrongLuong) as SoRoNhan
                        from PhieuCanBTPDinhHinh p
                        where p.Ngay >= @fromDate
                            and p.Ngay <= @toDate
                            and p.MaXuong = @xuongId
                            and p.STT > 0
                            and ISNULL(p.GhiChu, '') != 'HUY'
                            and p.IsOffline = 1
                        GROUP BY p.Ngay,
                            p.MaNhanVien,
                            p.MaThanhPham
                    ) btp
                    FULL OUTER JOIN (
                        SELECT p.Ngay,
                            p.MaNhanVien,
                            p.MaThanhPham,
                            SUM(p.TrongLuongTra) as TrongLuongTra,
                            COUNT(p.TrongLuongTra) as SoRoTra
                        from PhieuCanTPDinhHinh p
                        where p.Ngay >= @fromDate
                            and p.Ngay <= @toDate
                            and p.MaXuong = @xuongId
                            and p.STT > 0
                            and p.IsOffline = 1
                        GROUP BY p.Ngay,
                            p.MaNhanVien,
                            p.MaThanhPham
                    ) tp on btp.MaNhanVien = tp.MaNhanVien
                    and btp.Ngay = tp.Ngay
                    and btp.MaThanhPham = tp.MaThanhPham
            ) p
            LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
    ) p
GROUP BY p.Ngay,
    p.LoaiSanLuong,p.ThanhPhamName";
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

    public List<T> GetTongHopLoaiThanhPhams<T>(
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
    CONCAT(
        case
            when p.LoaiSanLuong = -1 then N'Cá Trả-'
            else ''
        end,
        tp.Ten
    ) as ThanhPhamName,
    --p.MaMau,
    --ma.Ten as MauName,
    p.CaTra,
    p.ChiSanLuong,
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
    isnull( DinhMuc.DinhMuc,tp.DinhMuc) as DinhMucChuan,
    Count(*) as SoRo,
	p.MaXuong
from
    (
        Select
            p.*,
            n.LoaiSanLuong
        from
            PhieuCanTPDinhHinh p
			left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
        where
            p.Ngay <= @toDate
            and p.Ngay >= @fromDate
            and MaXuong = @xuongId
            and p.IsOffline = 0
    ) p
    LEFT JOIN (
        Select
            tp1.MaLo,
            tp1.MaLoaiCa,
            tp1.MaSize,
            --tp1.MaMau,
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
                    --p.MaMau,
                    p.MaThanhPham,
                    p.CaTra,
                    CASE
                        WHEN p.CaTra = 1 THEN 1.37
                        ELSE tp.DinhMuc
                    END AS DinhMuc,
                    p.Ngay,
                    p.MaXuong
                from
                    PhieuCanTPDinhHinh p
                    left join MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
                where
                    p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and MaXuong = @xuongId
                    and p.MaLoaiCa = tp.MaCa
                    and p.IsOffline = 0
            ) tp1
            LEFT JOIN (
                Select
                    MaLo,
                    MaLoaiCa,
                    MaSize,
                    --MaMau,
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
                                --MaMau,
                                MaSize,
                                MaThanhPham,
                                CaTra,
                                Ngay
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            DinhMucDinhHinh d
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
            --and tp1.MaMau = tp2.MaMau
            and tp1.MaThanhPham = tp2.MaThanhPham
            and tp1.CaTra = tp2.CaTra
            and tp1.Ngay = tp2.Ngay
            and tp1.MaXuong = tp2.MaXuong
    ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
    and p.MaLo = DinhMuc.MaLo
    and p.MaLoaiCa = DinhMuc.MaLoaiCa
    and p.MaSize = DinhMuc.MaSize
    --and p.MaMau = DinhMuc.MaMau
    and p.MaThanhPham = DinhMuc.MaThanhPham
    And p.CaTra = DinhMuc.CaTra
    and p.Ngay = DinhMuc.Ngay
    LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
    LEFT Join MaSizeDinhHinh s on p.MaSize = s.Ma
    LEFT Join MaLoaiCaDinhHinh la on p.MaLoaiCa = la.Ma
    --LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
where
    p.STT > 0
    and p.Ngay <= @toDate
    and p.Ngay >= @fromDate
group by
    p.MaLoaiCa,
    p.MaSize,
    p.MaThanhPham,
    --p.MaMau,
    p.CaTra,
    DinhMuc.DinhMuc,
	tp.DinhMuc,
    p.Ngay,
    p.ChiSanLuong,
    la.Ten,
    tp.Ten,
    --ma.Ten,
    s.Ten,
    p.LoaiSanLuong,
	p.MaXuong";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new
                    {
                        fromDate = fromDate.Date, toDate = toDate.Date, xuongId, isDinhMucBinhThuong, isCaTraChuyenDoi,
                        isFloor
                    })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopLos<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isCaTraChuyenDoi = false,
        bool isFloor = true)
    {
        var query = @"Select
    p.Ngay,
	p.MaLo,
    p.MaSize,
    s.Ten as SizeName,
    p.MaThanhPham,
    CONCAT(
        case
            when p.LoaiSanLuong = -1 then N'Cá Trả-'
            else ''
        end,
        tp.Ten
    ) as ThanhPhamName,
    p.CaTra,
    p.ChiSanLuong,
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
    isnull( DinhMuc.DinhMuc,tp.DinhMuc) as DinhMucChuan,
    Count(*) as SoRo,
	p.MaXuong
from
    (
        Select
            p.*,
            n.LoaiSanLuong
        from
            PhieuCanTPDinhHinh p
            left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
        where
            p.Ngay <= @toDate
            and p.Ngay >= @fromDate
            and MaXuong = @xuongId
            and p.IsOffline = 0
    ) p
    LEFT JOIN (
        Select
            tp1.MaLo,
            tp1.MaSize,
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
                    p.MaSize,
                    p.MaThanhPham,
                    p.CaTra,
                    CASE
                        WHEN p.CaTra = 1 THEN 1.37
                        ELSE tp.DinhMuc
                    END AS DinhMuc,
                    p.Ngay,
                    p.MaXuong
                from
                    PhieuCanTPDinhHinh p
                    left join MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
                where
                    p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and MaXuong = @xuongId
                    and p.MaLoaiCa = tp.MaCa
                    and p.IsOffline = 0
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
                                CaTra,
                                Ngay
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            DinhMucDinhHinh d
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
            and tp1.CaTra = tp2.CaTra
            and tp1.Ngay = tp2.Ngay
            and tp1.MaXuong = tp2.MaXuong
    ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
    and p.MaLo = DinhMuc.MaLo
    and p.MaSize = DinhMuc.MaSize
    and p.MaThanhPham = DinhMuc.MaThanhPham
    And p.CaTra = DinhMuc.CaTra
    and p.Ngay = DinhMuc.Ngay
    LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
    LEFT Join MaSizeDinhHinh s on p.MaSize = s.Ma
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
where
    p.STT > 0
    and p.Ngay <= @toDate
    and p.Ngay >= @fromDate
group by
    p.MaLoaiCa,
    p.MaSize,
    p.MaThanhPham,
    --p.MaMau,
    p.CaTra,
    DinhMuc.DinhMuc,
    tp.DinhMuc,
    p.Ngay,
    p.ChiSanLuong,
    tp.Ten,
    s.Ten,
    p.LoaiSanLuong,
	p.MaXuong,
	p.MaLo";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new
                    {
                        fromDate = fromDate.Date, toDate = toDate.Date, xuongId, isDinhMucBinhThuong, isCaTraChuyenDoi,
                        isFloor
                    })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopNhanh<T>(DateTime dateTime, string xuongId, string maHoSo)
    {
        var query =
            @"Select p.MaNhanVien,p.MaLo,la.Ten as LoaiCaName,tp.Ten as ThanhPhamName,p.CaTra,COUNT(*) as SoRo,Sum(p.TrongLuongTra) as TrongLuongTra,SUM(p.TrongLuongNhan) as TrongLuongNhan,Sum(p.TrongLuongNhan) /SUM(p.TrongLuongTra) as DinhMuc from PhieuCanTPDinhHinh p, NhanVienDaiThanh n, MaLoaiCaDinhHinh la, MaThanhPhamDinhHinh tp where p.Ngay= @ngay and p.MaXuong = @xuongId and p.MaNhanVien = n.MaNhanVien and n.MaHoSo = @maHoSo and p.MaLoaiCa = la.Ma and p.MaThanhPham = tp.Ma group by p.MaNhanVien,p.MaLo,la.Ten ,tp.Ten,p.CaTra order by p.MaLo,tp.Ten";
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

    public List<T> GetTongHopNhanh<T>(DateTime dateTime, string xuongId, string maHoSo, bool isDinhMucBinhThuong)
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
    case
        when @isDinhMucBinhThuong = 1 then Sum(p.TrongLuongNhan) / SUM(p.TrongLuongTra)
        else Sum(p.TrongLuongTra) / SUM(p.TrongLuongNhan)
    end as DinhMuc
from
    PhieuCanTPDinhHinh p,
    NhanVienDaiThanh n,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and n.MaHoSo = @maHoSo
    and p.ChiSanLuong =0
group by
    p.MaNhanVien,
    p.MaLo,
    la.Ten,
    tp.Ten,
    p.CaTra
order by
    p.MaLo,
    tp.Ten";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(
                query,
                new { ngay = dateTime.Date, xuongId, maHoSo, isDinhMucBinhThuong })
            .Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopNhanh_MaNhanVien<T>(
        DateTime dateTime,
        string xuongId,
        string maHoSo,
        bool isDinhMucBinhThuong)
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
    case
        when @isDinhMucBinhThuong = 1 then Sum(p.TrongLuongNhan) / SUM(p.TrongLuongTra)
        else Sum(p.TrongLuongTra) / SUM(p.TrongLuongNhan)
    end as DinhMuc
from
    PhieuCanTPDinhHinh p,
    NhanVienDaiThanh n,
    MaLoaiCaDinhHinh la,
    MaThanhPhamDinhHinh tp
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiCa = la.Ma
    and p.MaThanhPham = tp.Ma
    and n.MaNhanVien = @maHoSo
    and p.ChiSanLuong =0
group by
    p.MaNhanVien,
    p.MaLo,
    la.Ten,
    tp.Ten,
    p.CaTra
order by
    p.MaLo,
    tp.Ten";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(
                query,
                new { ngay = dateTime.Date, xuongId, maHoSo, isDinhMucBinhThuong })
            .Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopNhanVien<T>(DateTime dateTime, string xuongId, TimeSpan time)
    {
        //                var query = @"Select
        //    p.*,
        //    dgxh.TenXepHang,
        //    dgxh.DinhMuc as DinhMucChuan,
        //    ISNULL(dgxh.DonGia, 0) as DonGia,
        //    cast(
        //        (
        //            (
        //                ISNULL(dgxh.DonGia, 0) * dgxh.HeSo - ISNULL(dgxh.DonGia, 0) * dgxh.HeSo *(
        //                    case
        //                        when dgxh.IsUsedHeSoRot = 1 then dgxh.HeSoRot
        //                        else 0
        //                    end
        //                )
        //            ) * p.TrongLuongTP
        //        ) as decimal(18, 2)
        //    ) as ThanhTien
        //from
        //    (
        //        Select
        //            tp.MaNhanVien,
        //            n.MaHoSo,
        //            n.Name as NhanVienName,
        //            tp.MaLo,
        //            la.Ten as LoaiCaName,
        //            _tp.Ten as ThanhPhamName,
        //            _tp.BravoId as MaSanPham,
        //            s.Ten as SizeName,
        //            ma.Ten as MauName,
        //            tp.TrongLuong as TrongLuongTP,
        //            tp.SoRo as SoRoTP,
        //            isnull(btp.TrongLuong, 0) as TrongLuongBTP,
        //            --isnull(btp.TrongLuong * _tp.DinhMucHaoHut, 0) as TLTruocHaoHutBTP,
        //			isnull(btp.TrongLuong, 0) as TLTruocHaoHutBTP,
        //            isnull(btp.SoRo, 0) as SoRoBTP,
        //            isnull(
        //                cast (
        //                    case
        //                        when tp.TrongLuong = 0 then 0
        //                        --else btp.TrongLuong * _tp.DinhMucHaoHut / tp.TrongLuong
        //						else btp.TrongLuong / tp.TrongLuong
        //                    end as decimal(18, 3)
        //                ),
        //                0
        //            ) as DinhMuc
        //        from
        //            (
        //                Select
        //                    p.MaNhanVien,
        //                    p.MaLo,
        //                    p.MaLoaiCa,
        //                    p.MaSize,
        //                    p.MaMau,
        //                    p.MaThanhPham,
        //                    sum(p.TrongLuongTra) as TrongLuong,
        //                    COUNT(*) as SoRo
        //                from
        //                    PhieuCanTPDinhHinh p
        //                Where
        //                    p.Ngay = @ngay
        //                    and p.MaXuong = @xuongId
        //                    and p.Gio <= @time  and p.STT > 0
        //                GROUP BY
        //                    p.MaNhanVien,
        //                    p.MaLo,
        //                    p.MaLoaiCa,
        //                    p.MaSize,
        //                    p.MaMau,
        //                    p.MaThanhPham
        //            ) tp
        //            LEFT JOIN (
        //                Select
        //                    p.MaNhanVien,
        //                    p.MaLo,
        //                    p.MaLoaiCa,
        //                    p.MaSize,
        //                    p.MaMau,
        //                    p.MaThanhPham,
        //                    sum(p.TrongLuong) as TrongLuong,
        //                    COUNT(*) as SoRo
        //                from
        //                    PhieuCanBTPDinhHinh p
        //                Where
        //                    p.Ngay = @ngay
        //                    and p.MaXuong = @xuongId
        //                    and p.Gio <= @time and p.STT > 0
        //                GROUP BY
        //                    p.MaNhanVien,
        //                    p.MaLo,
        //                    p.MaLoaiCa,
        //                    p.MaSize,
        //                    p.MaMau,
        //                    p.MaThanhPham
        //            ) btp ON tp.MaLo = btp.MaLo
        //            and tp.MaLoaiCa = btp.MaLoaiCa
        //            and tp.MaMau = btp.MaMau
        //            and tp.MaNhanVien = btp.MaNhanVien
        //            and tp.MaSize = btp.MaSize
        //            and tp.MaThanhPham = btp.MaThanhPham
        //            LEFT JOIN MaLoaiCaDinhHinh la on tp.MaLoaiCa = la.Ma
        //            LEFT JOIN MaMauDinhHinh ma on tp.MaMau = ma.Ma
        //            LEFT JOIN MaSizeDinhHinh s on tp.MaSize = s.Ma
        //            LEFT JOIN MaThanhPhamDinhHinh _tp on tp.MaThanhPham = _tp.Ma
        //            LEFT JOIN NhanVienDaiThanh n on tp.MaNhanVien = n.MaNhanVien
        //    ) p
        //    LEFT JOIN (
        //        select
        //            dmxh.*,
        //            dg.DonGia,
        //            dg.DonGiaGiaCong,
        //            dg.HeSo,
        //            dg.HeSoRot,
        //            dg.IsUsedHeSoRot
        //        from
        //            (
        //                Select
        //                    dmxh.*,
        //                    xh.Ten as TenXepHang
        //                from
        //                    DinhMucXepHang dmxh,
        //                    MaXepHang xh
        //                where
        //                    [Year] = YEAR(@ngay)
        //                    and dmxh.MaXepHang = xh.Ma
        //            ) dmxh
        //            LEFT JOIN(
        //                Select
        //                    *
        //                from
        //                    (
        //                        Select
        //                            *,
        //                            ROW_NUMBER() over (
        //                                partition by MaSanPham,
        //                                DanhGia,
        //                                DinhMucDown,
        //                                MaLoaiDonGia,
        //                                MaSizeDinhHinh,
        //                                MaXepHang
        //                                order by
        //                                    Ngay DESC,
        //                                    Gio Desc
        //                            ) as rowId
        //                        from
        //                            DG_DonGia
        //                        where
        //                            Ngay <= @ngay
        //                            and MaLoaiDonGia = 'XH'
        //                    ) p
        //                where
        //                    p.rowId = 1
        //            ) dg on dmxh.MaSanPham = dg.MaSanPham
        //            and dmxh.MaXepHang = dg.MaXepHang
        //    ) dgxh on p.MaSanPham = dgxh.MaSanPham
        //    and p.MaLo = dgxh.MaLo
        //    and p.DinhMuc >= dgxh.DinhMucDown
        //    and p.DinhMuc <= dgxh.DinhMucUp
        //order by
        //    p.MaLo,
        //    p.ThanhPhamName";
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
            tp.TrongLuongTra as TrongLuongTP,
            tp.SoRo as SoRoTP,
            --isnull(btp.TrongLuong, 0) as TrongLuongBTP,
			 isnull(tp.TrongLuongNhan, 0) as TrongLuongBTP,
            --isnull(btp.TrongLuong * _tp.DinhMucHaoHut, 0) as TLTruocHaoHutBTP,
			--isnull(btp.TrongLuong, 0) as TLTruocHaoHutBTP,
			isnull(tp.TrongLuongNhan, 0) as TLTruocHaoHutBTP,
            --isnull(btp.SoRo, 0) as SoRoBTP,
			isnull(tp.SoRo, 0) as SoRoBTP,
            isnull(
                cast (
                    case
                        when tp.TrongLuongTra = 0 then 0
                        --else btp.TrongLuong * _tp.DinhMucHaoHut / tp.TrongLuong
						--else btp.TrongLuong / tp.TrongLuong
						else tp.TrongLuongNhan / tp.TrongLuongTra
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
                    sum(p.TrongLuongTra) as TrongLuongTra,
					sum(p.TrongLuongNhan) as TrongLuongNhan,
                    COUNT(*) as SoRo
                from
                    PhieuCanTPDinhHinh p
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
            ) tp
            --LEFT JOIN (
            --    Select
            --        p.MaNhanVien,
            --        p.MaLo,
            --        p.MaLoaiCa,
            --        p.MaSize,
            --        p.MaMau,
            --        p.MaThanhPham,
            --        sum(p.TrongLuong) as TrongLuong,
            --        COUNT(*) as SoRo
            --    from
            --        PhieuCanBTPDinhHinh p
            --    Where
            --        p.Ngay = @ngay
            --        and p.MaXuong = @xuongId
            --        and p.STT > 0
            --    GROUP BY
            --        p.MaNhanVien,
            --        p.MaLo,
            --        p.MaLoaiCa,
            --        p.MaSize,
            --        p.MaMau,
            --        p.MaThanhPham
            --) btp ON tp.MaLo = btp.MaLo
            --and tp.MaLoaiCa = btp.MaLoaiCa
            --and tp.MaMau = btp.MaMau
            --and tp.MaNhanVien = btp.MaNhanVien
            --and tp.MaSize = btp.MaSize
            --and tp.MaThanhPham = btp.MaThanhPham
            LEFT JOIN MaLoaiCaDinhHinh la on tp.MaLoaiCa = la.Ma
            LEFT JOIN MaMauDinhHinh ma on tp.MaMau = ma.Ma
            LEFT JOIN MaSizeDinhHinh s on tp.MaSize = s.Ma
            LEFT JOIN MaThanhPhamDinhHinh _tp on tp.MaThanhPham = _tp.Ma
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
                            and MaLoaiDonGia = 'XH'   and LoaiCan =''
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

        //                var query = @"Select
        //    p.*,
        //    dgxh.TenXepHang,
        //    dgxh.DinhMuc as DinhMucChuan,
        //    ISNULL(dgxh.DonGia, 0) as DonGia,
        //    cast(
        //        (
        //            (
        //                ISNULL(dgxh.DonGia, 0) * dgxh.HeSo - ISNULL(dgxh.DonGia, 0) * dgxh.HeSo *(
        //                    case
        //                        when dgxh.IsUsedHeSoRot = 1 then dgxh.HeSoRot
        //                        else 0
        //                    end
        //                )
        //            ) * p.TrongLuongTP
        //        ) as decimal(18, 2)
        //    ) as ThanhTien
        //from
        //    (
        //        Select
        //            tp.MaNhanVien,
        //            n.MaHoSo,
        //            n.Name as NhanVienName,
        //            tp.MaLo,
        //            la.Ten as LoaiCaName,
        //            _tp.Ten as ThanhPhamName,
        //            _tp.BravoId as MaSanPham,
        //            s.Ten as SizeName,
        //            ma.Ten as MauName,
        //            tp.TrongLuong as TrongLuongTP,
        //            tp.SoRo as SoRoTP,
        //            isnull(btp.TrongLuong, 0) as TrongLuongBTP,
        //            --isnull(btp.TrongLuong * _tp.DinhMucHaoHut, 0) as TLTruocHaoHutBTP,
        //			isnull(btp.TrongLuong, 0) as TLTruocHaoHutBTP,
        //            isnull(btp.SoRo, 0) as SoRoBTP,
        //            isnull(
        //                cast (
        //                    case
        //                        when tp.TrongLuong = 0 then 0
        //                        --else btp.TrongLuong * _tp.DinhMucHaoHut / tp.TrongLuong
        //						else btp.TrongLuong / tp.TrongLuong
        //                    end as decimal(18, 3)
        //                ),
        //                0
        //            ) as DinhMuc
        //        from
        //            (
        //                Select
        //                    p.MaNhanVien,
        //                    p.MaLo,
        //                    p.MaLoaiCa,
        //                    p.MaSize,
        //                    p.MaMau,
        //                    p.MaThanhPham,
        //                    sum(p.TrongLuongTra) as TrongLuong,
        //                    COUNT(*) as SoRo
        //                from
        //                    PhieuCanTPDinhHinh p
        //                Where
        //                    p.Ngay = @ngay
        //                    and p.MaXuong = @xuongId
        //                     and p.STT > 0
        //                GROUP BY
        //                    p.MaNhanVien,
        //                    p.MaLo,
        //                    p.MaLoaiCa,
        //                    p.MaSize,
        //                    p.MaMau,
        //                    p.MaThanhPham
        //            ) tp
        //            LEFT JOIN (
        //                Select
        //                    p.MaNhanVien,
        //                    p.MaLo,
        //                    p.MaLoaiCa,
        //                    p.MaSize,
        //                    p.MaMau,
        //                    p.MaThanhPham,
        //                    sum(p.TrongLuong) as TrongLuong,
        //                    COUNT(*) as SoRo
        //                from
        //                    PhieuCanBTPDinhHinh p
        //                Where
        //                    p.Ngay = @ngay
        //                    and p.MaXuong = @xuongId
        //                    and p.STT > 0
        //                GROUP BY
        //                    p.MaNhanVien,
        //                    p.MaLo,
        //                    p.MaLoaiCa,
        //                    p.MaSize,
        //                    p.MaMau,
        //                    p.MaThanhPham
        //            ) btp ON tp.MaLo = btp.MaLo
        //            and tp.MaLoaiCa = btp.MaLoaiCa
        //            and tp.MaMau = btp.MaMau
        //            and tp.MaNhanVien = btp.MaNhanVien
        //            and tp.MaSize = btp.MaSize
        //            and tp.MaThanhPham = btp.MaThanhPham
        //            LEFT JOIN MaLoaiCaDinhHinh la on tp.MaLoaiCa = la.Ma
        //            LEFT JOIN MaMauDinhHinh ma on tp.MaMau = ma.Ma
        //            LEFT JOIN MaSizeDinhHinh s on tp.MaSize = s.Ma
        //            LEFT JOIN MaThanhPhamDinhHinh _tp on tp.MaThanhPham = _tp.Ma
        //            LEFT JOIN NhanVienDaiThanh n on tp.MaNhanVien = n.MaNhanVien
        //    ) p
        //    LEFT JOIN (
        //        select
        //            dmxh.*,
        //            dg.DonGia,
        //            dg.DonGiaGiaCong,
        //            dg.HeSo,
        //            dg.HeSoRot,
        //            dg.IsUsedHeSoRot
        //        from
        //            (
        //                Select
        //                    dmxh.*,
        //                    xh.Ten as TenXepHang
        //                from
        //                    DinhMucXepHang dmxh,
        //                    MaXepHang xh
        //                where
        //                    [Year] = YEAR(@ngay)
        //                    and dmxh.MaXepHang = xh.Ma
        //            ) dmxh
        //            LEFT JOIN(
        //                Select
        //                    *
        //                from
        //                    (
        //                        Select
        //                            *,
        //                            ROW_NUMBER() over (
        //                                partition by MaSanPham,
        //                                DanhGia,
        //                                DinhMucDown,
        //                                MaLoaiDonGia,
        //                                MaSizeDinhHinh,
        //                                MaXepHang
        //                                order by
        //                                    Ngay DESC,
        //                                    Gio Desc
        //                            ) as rowId
        //                        from
        //                            DG_DonGia
        //                        where
        //                            Ngay <= @ngay
        //                            and MaLoaiDonGia = 'XH'
        //                    ) p
        //                where
        //                    p.rowId = 1
        //            ) dg on dmxh.MaSanPham = dg.MaSanPham
        //            and dmxh.MaXepHang = dg.MaXepHang
        //    ) dgxh on p.MaSanPham = dgxh.MaSanPham
        //    and p.MaLo = dgxh.MaLo
        //    and p.DinhMuc >= dgxh.DinhMucDown
        //    and p.DinhMuc <= dgxh.DinhMucUp
        //order by
        //    p.MaLo,
        //    p.ThanhPhamName";
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
            tp.TrongLuongTra as TrongLuongTP,
            tp.SoRo as SoRoTP,
            --isnull(btp.TrongLuong, 0) as TrongLuongBTP,
			 isnull(tp.TrongLuongNhan, 0) as TrongLuongBTP,
            --isnull(btp.TrongLuong * _tp.DinhMucHaoHut, 0) as TLTruocHaoHutBTP,
			--isnull(btp.TrongLuong, 0) as TLTruocHaoHutBTP,
			isnull(tp.TrongLuongNhan, 0) as TLTruocHaoHutBTP,
            --isnull(btp.SoRo, 0) as SoRoBTP,
			isnull(tp.SoRo, 0) as SoRoBTP,
            isnull(
                cast (
                    case
                        when tp.TrongLuongTra = 0 then 0
                        --else btp.TrongLuong * _tp.DinhMucHaoHut / tp.TrongLuong
						--else btp.TrongLuong / tp.TrongLuong
						else tp.TrongLuongNhan / tp.TrongLuongTra
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
                    sum(p.TrongLuongTra) as TrongLuongTra,
					sum(p.TrongLuongNhan) as TrongLuongNhan,
                    COUNT(*) as SoRo
                from
                    PhieuCanTPDinhHinh p
                Where
                    p.Ngay = @ngay
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
            --LEFT JOIN (
            --    Select
            --        p.MaNhanVien,
            --        p.MaLo,
            --        p.MaLoaiCa,
            --        p.MaSize,
            --        p.MaMau,
            --        p.MaThanhPham,
            --        sum(p.TrongLuong) as TrongLuong,
            --        COUNT(*) as SoRo
            --    from
            --        PhieuCanBTPDinhHinh p
            --    Where
            --        p.Ngay = @ngay
            --        and p.MaXuong = @xuongId
            --        and p.STT > 0
            --    GROUP BY
            --        p.MaNhanVien,
            --        p.MaLo,
            --        p.MaLoaiCa,
            --        p.MaSize,
            --        p.MaMau,
            --        p.MaThanhPham
            --) btp ON tp.MaLo = btp.MaLo
            --and tp.MaLoaiCa = btp.MaLoaiCa
            --and tp.MaMau = btp.MaMau
            --and tp.MaNhanVien = btp.MaNhanVien
            --and tp.MaSize = btp.MaSize
            --and tp.MaThanhPham = btp.MaThanhPham
            LEFT JOIN MaLoaiCaDinhHinh la on tp.MaLoaiCa = la.Ma
            LEFT JOIN MaMauDinhHinh ma on tp.MaMau = ma.Ma
            LEFT JOIN MaSizeDinhHinh s on tp.MaSize = s.Ma
            LEFT JOIN MaThanhPhamDinhHinh _tp on tp.MaThanhPham = _tp.Ma
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

    public List<T> GetTongHopNhanVienBanKiems<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
        var query = @"Select
    p.*,
    n.Name as NhanVienName,
    n.MaHoSo,
n.DeptName0 as Nhom,
    la.Ten as LoaiCaName,
    s.Ten as SizeName,
    t.Ten as ThanhPhamName,
    m.Ten as MauName
from
(
        Select
            p.Ngay,
            p.MaNhanVienBanKiem as MaNhanVien,
            p.MaLo,
            p.MaLoaiCa,
            p.MaSize,
            p.MaThanhPham,
            p.MaMau,
          Cast( isnull( SUM(p.TrongLuongTra),0) as float ) as TrongLuong
        from
            PhieuCanTPDinhHinh p
        where
            p.Ngay >= @fromDate
            and p.Ngay <= @toDate
            and p.MaXuong = @xuongId
            and p.MaNhanVienBanKiem is not null
        GROUP by
            p.Ngay,
            p.MaNhanVienBanKiem,
            p.MaLo,
            p.MaLoaiCa,
            p.MaSize,
            p.MaThanhPham,
            p.MaMau
    ) p
    left JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
    left JOIN MaLoaiCaDinhHinh la on p.MaLoaiCa = la.Ma
    left JOIN MaSizeDinhHinh s on p.MaSize = s.Ma
    left JOIN MaThanhPhamDinhHinh t on p.MaThanhPham = t.Ma
    left JOIN MaMauDinhHinh m on p.MaMau = m.Ma";
//        var query = @"Select
//    p.*,
//    n.Name as NhanVienName,
//    n.MaHoSo,
//n.DeptName0 as Nhom,
//    la.Ten as LoaiCaName,
//    s.Ten as SizeName,
//    t.Ten as ThanhPhamName,
//    m.Ten as MauName,
//	MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianVao,
//	MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianRa,
//	DATEDIFF(hour, MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay), MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay)) as TongThoiGian
//from
//(
//        Select
//            p.Ngay,
//            p.MaNhanVienBanKiem as MaNhanVien,
//            p.MaLo,
//            p.MaLoaiCa,
//            p.MaSize,
//            p.MaThanhPham,
//            p.MaMau,
//          Cast( isnull( SUM(p.TrongLuongTra),0) as float ) as TrongLuong
//        from
//            PhieuCanTPDinhHinh p
//        where
//            p.Ngay >= @fromDate
//            and p.Ngay <= @toDate
//            and p.MaXuong = @xuongId
//            and p.MaNhanVienBanKiem is not null
//        GROUP by
//            p.Ngay,
//            p.MaNhanVienBanKiem,
//            p.MaLo,
//            p.MaLoaiCa,
//            p.MaSize,
//            p.MaThanhPham,
//            p.MaMau
//    ) p
//    left JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
//    left JOIN MaLoaiCaDinhHinh la on p.MaLoaiCa = la.Ma
//    left JOIN MaSizeDinhHinh s on p.MaSize = s.Ma
//    left JOIN MaThanhPhamDinhHinh t on p.MaThanhPham = t.Ma
//    left JOIN MaMauDinhHinh m on p.MaMau = m.Ma
//	left join CheckInOut c on n.MaChamCong = c.MaChamCong";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(
                query,
                new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopNhanVienOfflines<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isFloor = true)
    {
        var query = $@"
declare @toDate as Date,@fromDate as Date,@xuongId as varchar(20),@isDinhMucBinhThuong as bit,@isFloor as bit;
set @toDate = '{toDate:yyyy-MM-dd}';
set @fromDate='{fromDate:yyyy-MM-dd}';
set @xuongId='{xuongId}';
set @isDinhMucBinhThuong ={(isDinhMucBinhThuong ? "1" : "0")};
set @isFloor ={(isFloor ? "1" : "0")};
select p.*,
    n.Name as NhanVienName,
    n.MaHoSo,
    n.DeptCode0 as MaNhom,
    n.DeptName0 as Nhom,
    tp.Ten as ThanhPhamName,
    tp.BravoId as MaSanPham
from (
        select ISNULL(btp.Ngay, tp.Ngay) as Ngay,
            isnull(btp.MaNhanVien, tp.MaNhanVien) as MaNhanVien,
            isnull(btp.MaThanhPham, tp.MaThanhPham) as MaThanhPham,
            isNull (btp.TrongLuongNhan, 0) as TrongLuongNhan,
            isnull(tp.TrongLuongTra, 0) as TrongLuongTra,
            CASE
                WHEN isnull(tp.TrongLuongTra, 0) = 0 then 0
                else cast(
                    isNull (btp.TrongLuongNhan, 0) / isnull(tp.TrongLuongTra, 0) as decimal(18, 2)
                )
            end as DinhMuc,
            isNull (btp.SoRoNhan, 0) as SoRoNhan,
            isnull(tp.SoRoTra, 0) as SoRoTra
        from (
                Select p.Ngay,
                    p.MaNhanVien,
                    p.MaThanhPham,
                    SUM(p.TrongLuong) as TrongLuongNhan,
                    COUNT(p.TrongLuong) as SoRoNhan
                from PhieuCanBTPDinhHinh p
                where p.Ngay >= @fromDate
                    and p.Ngay <= @toDate
                    and p.MaXuong = @xuongId
                    and p.STT > 0
                    and ISNULL(p.GhiChu, '') != 'HUY'
                    and p.IsOffline = 1
                GROUP BY p.Ngay,
                    p.MaNhanVien,
                    p.MaThanhPham
            ) btp
            FULL OUTER JOIN (
                SELECT p.Ngay,
                    p.MaNhanVien,
                    p.MaThanhPham,
                    SUM(p.TrongLuongTra) as TrongLuongTra,
                    COUNT(p.TrongLuongTra) as SoRoTra
                from PhieuCanTPDinhHinh p
                where p.Ngay >= @fromDate
                    and p.Ngay <= @toDate
                    and p.MaXuong = @xuongId
                    and p.STT > 0
                    and p.IsOffline = 1
                GROUP BY p.Ngay,
                    p.MaNhanVien,
                    p.MaThanhPham
            ) tp on btp.MaNhanVien = tp.MaNhanVien
            and btp.Ngay = tp.Ngay
            and btp.MaThanhPham = tp.MaThanhPham
    ) p
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
    LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
order by
n.MaHoSo";
        //p.MaHoSo"; chắt sửa p -> n
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.Query<T>(
                    query)
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopNhanVienPhucVus<T>(DateTime fromDate, DateTime toDate, string xuongId)
    {
//        var query = @"Select
//    p.*,
//    n.Name as NhanVienName,
//    n.MaHoSo,
//n.DeptName0 as Nhom,
//    la.Ten as LoaiCaName,
//    s.Ten as SizeName,
//    t.Ten as ThanhPhamName,
//    m.Ten as MauName
//from
//(
//        Select
//            p.Ngay,
//            p.MaNhanVienPhucVu as MaNhanVien,
//            p.MaLo,
//            p.MaLoaiCa,
//            p.MaSize,
//            p.MaThanhPham,
//            p.MaMau,
//            Cast( isnull( SUM(p.TrongLuongTra),0) as float ) as TrongLuong
//        from
//            PhieuCanTPDinhHinh p
//        where
//            p.Ngay >= @fromDate
//            and p.Ngay <= @toDate
//            and p.MaXuong = @xuongId
//            and p.MaNhanVienPhucVu is not null
//        GROUP by
//            p.Ngay,
//            p.MaNhanVienPhucVu,
//            p.MaLo,
//            p.MaLoaiCa,
//            p.MaSize,
//            p.MaThanhPham,
//            p.MaMau
//    ) p
//    left JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
//    left JOIN MaLoaiCaDinhHinh la on p.MaLoaiCa = la.Ma
//    left JOIN MaSizeDinhHinh s on p.MaSize = s.Ma
//    left JOIN MaThanhPhamDinhHinh t on p.MaThanhPham = t.Ma
//    left JOIN MaMauDinhHinh m on p.MaMau = m.Ma";
//thêm checkinout đẻ lấy thời gina vao ra
        var query = @"Select
    p.*,
    n.Name as NhanVienName,
    n.MaHoSo,
n.DeptName0 as Nhom,
    la.Ten as LoaiCaName,
    s.Ten as SizeName,
    t.Ten as ThanhPhamName,
    m.Ten as MauName,
	MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianVao,
	MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianRa,
	DATEDIFF(hour, MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay), MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay)) as TongThoiGian
from
(
        Select
            p.Ngay,
            p.MaNhanVienPhucVu as MaNhanVien,
            p.MaLo,
            p.MaLoaiCa,
            p.MaSize,
            p.MaThanhPham,
            p.MaMau,
            Cast( isnull( SUM(p.TrongLuongTra),0) as float ) as TrongLuong
        from
            PhieuCanTPDinhHinh p
        where
            p.Ngay >= @fromDate
            and p.Ngay <= @toDate
            and p.MaXuong = @xuongId
            and p.MaNhanVienPhucVu is not null
        GROUP by
            p.Ngay,
            p.MaNhanVienPhucVu,
            p.MaLo,
            p.MaLoaiCa,
            p.MaSize,
            p.MaThanhPham,
            p.MaMau
    ) p
    left JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
    left JOIN MaLoaiCaDinhHinh la on p.MaLoaiCa = la.Ma
    left JOIN MaSizeDinhHinh s on p.MaSize = s.Ma
    left JOIN MaThanhPhamDinhHinh t on p.MaThanhPham = t.Ma
    left JOIN MaMauDinhHinh m on p.MaMau = m.Ma
	left join CheckInOut c on n.MaChamCong = c.MaChamCong";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(
                query,
                new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId })
            .Result
            .ToList();
        return items;
    }

    public List<T> GetTongHopNhanViens<T>(
        DateTime fromDate,
        DateTime toDate,
        string xuongId,
        bool isDinhMucBinhThuong = true,
        bool isFloor = true)
    {
        //        var query = $@"
        //declare @toDate as Date,@fromDate as Date,@xuongId as varchar(20),@isDinhMucBinhThuong as bit,@isFloor as bit;
        //set @toDate = '{toDate:yyyy-MM-dd}';
        //set @fromDate='{fromDate:yyyy-MM-dd}';
        //set @xuongId='{xuongId}';
        //set @isDinhMucBinhThuong ={(isDinhMucBinhThuong ? "1" : "0")};
        //set @isFloor ={(isFloor ? "1" : "0")};
        //select
        //    IsNull(tp.Ngay, btp.Ngay) as Ngay,
        //    IsNull(tp.MaNhanVien, btp.MaNhanVien) as MaNhanVien,
        //    ISNULL(tp.MaHoSo, btp.MaHoSo) as MaHoSo,
        //    ISNULL(tp.NhanVienName, btp.NhanVienName) as NhanVienName,
        //    IsNull(tp.MaLo, btp.MaLo) as MaLo,
        //    cast(ISNULL(tp.MaNhom, btp.MaNhom) as nvarchar(500)) as MaNhom,
        //    ISNULL(tp.Nhom, btp.Nhom) as Nhom,
        //    ISNULL(tp.MaLoaiCa, btp.MaLoaiCa) as MaLoaiCa,
        //    ISNULL(tp.LoaiCaName, btp.LoaiCaName) as LoaiCaName,
        //    ISNULL(tp.MaSize, btp.MaSize) as MaSize,
        //    ISNULL(tp.SizeName, btp.SizeName) as SizeName,
        //    ISNULL(tp.MaThanhPham, btp.MaThanhPham) as MaThanhPham,
        //    ISNULL(tp.ThanhPhamName, btp.ThanhPhamName) as ThanhPhamName,
        //    ISNULL(tp.BaoCaoDauRot, btp.BaoCaoDauRot) as BaoCaoDauRot,
        //    ISNULL(tp.MaSanPham, btp.MaSanPham) as MaSanPham,
        //    ISNULL(tp.MaMau, btp.MaMau) as MaMau,
        //    ISNULL(tp.MauName, btp.MauName) as MauName,
        //    ISNULL(tp.CaTra, btp.CaTra) as CaTra,
        //    IsNull(tp.ChiSanLuong, btp.ChiSanLuong) as ChiSanLuong,
        //    ISNULL(tp.TrongLuongNhan, 0) as TrongLuongNhan,
        //    ISNULL(tp.TrongLuongTra, 0) as TrongLuongTra,
        //    IsNull(tp.LoaiSanLuong, btp.LoaiSanLuong) * ISNULL(tp.TrongLuongNhan, 0) as TrongLuongNhanChucNang,
        //    IsNull(tp.LoaiSanLuong, btp.LoaiSanLuong) * ISNULL(tp.TrongLuongTra, 0) as TrongLuongTraChucNang,
        //    ISNULL(tp.DinhMucThucTe, 0) as DinhMuc,
        //    ISNULL(tp.DinhMucYeuCau, 0) as DinhMucChuan,
        //    cast(
        //        (
        //            Case
        //                when @isDinhMucBinhThuong = 1 then(
        //                    case
        //                        when ISNULL(tp.DinhMucThucTe, 0) > ISNULL(tp.DinhMucYeuCau, 0) then 0
        //                        else 1
        //                    end
        //                )
        //                else case
        //                    when ISNULL(tp.DinhMucThucTe, 0) < ISNULL(tp.DinhMucYeuCau, 0) then 0
        //                    else 1
        //                end
        //            end
        //        ) as bit
        //    ) as DanhGia,
        //    ISNULL(tp.SoRo, 0) as SoRoTP,
        //    ISNULL(btp.SoRo, 0) as SoRoBTP,
        //    ISNULL(btp.SoRoHuy, 0) as SoRoHuy,
        //    ISNULL(btp.SoRoChuaCanTP, 0) as SoRoChuaCanTP,
        //    ISNULL(btp.SoRoHuy, 0) + ISNULL(btp.SoRoChuaCanTP, 0) as SoRoLoi,
        //    ISNULL(tp.MaXuong, btp.MaXuong) as MaXuong
        //from
        //    (
        //        Select
        //            p.Ngay,
        //            p.MaNhanVien,
        //            n.MaHoSo,
        //            n.Name as NhanVienName,
        //            n.LoaiSanLuong,
        //            n.DeptCode0 as MaNhom,
        //            n.DeptName0 as Nhom,
        //            la.Ten as LoaiCaName,
        //            s.Ten as SizeName,
        //            tp.Ten as ThanhPhamName,
        //            tp.BravoId as MaSanPham,
        //            tp.BaoCaoDauRot as BaoCaoDauRot,
        //            ma.Ten as MauName,
        //            p.MaLoaiCa,
        //            p.MaLo,
        //            p.MaMau,
        //            p.MaThanhPham,
        //            p.MaSize,
        //            p.CaTra,
        //            p.ChiSanLuong,
        //            p.MaXuong,
        //            SUM(
        //                        case
        //                            when ISNULL(p.GhiChu, '') = 'HUY' then 0
        //                            else 1
        //                        end
        //                    ) as SoRo,
        //            SUM(
        //                case
        //                    when ISNULL(p.GhiChu, '') = 'HUY' then 1
        //                    else 0
        //                end
        //            ) as SoRoHuy,
        //            Sum(
        //                case
        //                    when p.IsEnabled = 0 then 1
        //                    else 0
        //                end
        //            ) as SoRoChuaCanTP
        //        from
        //            PhieuCanBTPDinhHinh p
        //            LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
        //            LEFT Join MaSizeDinhHinh s on p.MaSize = s.Ma
        //            LEFT Join MaLoaiCaDinhHinh la on p.MaLoaiCa = la.Ma
        //            LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
        //            LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
        //        where
        //            p.STT > 0
        //            and p.Ngay <= @toDate
        //            and p.Ngay >= @fromDate
        //            and p.MaXuong = @xuongId
        //            and p.IsOffline = 0
        //        GROUP BY
        //            p.Ngay,
        //            p.MaNhanVien,
        //            p.MaLoaiCa,
        //            p.MaLo,
        //            p.MaMau,
        //            p.MaThanhPham,
        //            p.MaSize,
        //            p.CaTra,
        //            p.ChiSanLuong,
        //            n.LoaiSanLuong,
        //            p.MaXuong,
        //            n.MaHoSo,
        //            n.Name,
        //            la.Ten,
        //            ma.Ten,
        //            tp.Ten,
        //            s.Ten,
        //            n.DeptCode0,
        //            n.DeptName0,
        //            tp.BravoId,
        //            tp.BaoCaoDauRot
        //    ) btp
        //    left join(
        //        Select
        //            p.Ngay,
        //            p.MaNhanVien,
        //            n.MaHoSo,
        //            n.Name as NhanVienName,
        //            n.DeptCode0 as MaNhom,
        //            n.DeptName0 as Nhom,
        //            p.MaLo,
        //            p.MaLoaiCa,
        //            la.Ten as LoaiCaName,
        //            p.MaSize,
        //            s.Ten as SizeName,
        //            p.MaThanhPham,
        //            tp.Ten as ThanhPhamName,
        //            tp.BravoId as MaSanPham,
        //            tp.BaoCaoDauRot as BaoCaoDauRot,
        //            p.MaMau,
        //            ma.Ten as MauName,
        //            p.CaTra,
        //            p.ChiSanLuong,
        //            n.LoaiSanLuong,
        //            Sum(p.TrongLuongNhan) as TrongLuongNhan,
        //            Sum(p.TrongLuongTra) as TrongLuongTra,
        //            case
        //                when @isDinhMucBinhThuong = 1 then cast(
        //                    case
        //                        when sum(p.TrongLuongTra * n.LoaiSanLuong) = 0 then 0
        //                        else (
        //                            case
        //                                when @isFloor = 1 then(
        //                                    FLOOR(
        //                                        (
        //                                            Sum(p.TrongLuongNhan * n.LoaiSanLuong) / sum(p.TrongLuongTra * n.LoaiSanLuong)
        //                                        ) * 1000
        //                                    ) / 1000
        //                                )
        //                                else Sum(p.TrongLuongNhan * n.LoaiSanLuong) / sum(p.TrongLuongTra * n.LoaiSanLuong)
        //                            end
        //                        )
        //                    end as decimal(18, 3)
        //                )
        //                ELSE cast(
        //                    case
        //                        when sum(p.TrongLuongNhan * n.LoaiSanLuong) = 0 then 0
        //                        else sum(p.TrongLuongTra * n.LoaiSanLuong) / sum(p.TrongLuongNhan * n.LoaiSanLuong)
        //                    end as decimal(18, 4)
        //                )
        //            end as DinhMucThucTe,
        //            DinhMuc.DinhMuc as DinhMucYeuCau,
        //            Count(*) as SoRo,
        //            p.MaXuong
        //        from
        //            PhieuCanTPDinhHinh p
        //            LEFT JOIN(
        //                Select
        //                    tp1.MaLo,
        //                    tp1.MaLoaiCa,
        //                    tp1.MaSize,
        //                    tp1.MaMau,
        //                    tp1.MaThanhPham,
        //                    tp1.CaTra,
        //                    CASE
        //                        WHEN tp2.DinhMuc is Null THEN tp1.DinhMuc
        //                        ELSE tp2.DinhMuc
        //                    END AS DinhMuc,
        //                    tp1.Ngay,
        //                    tp1.MaXuong
        //                from
        //                    (
        //                        Select
        //                            distinct p.MaLo,
        //                            p.MaLoaiCa,
        //                            p.MaSize,
        //                            p.MaMau,
        //                            p.MaThanhPham,
        //                            p.CaTra,
        //                            CASE
        //                                WHEN p.CaTra = 1 THEN 1.37
        //                                ELSE tp.DinhMuc
        //                            END AS DinhMuc,
        //                            p.Ngay,
        //                            p.MaXuong
        //                        from
        //                            PhieuCanTPDinhHinh p,
        //                            MaThanhPhamDinhHinh tp
        //                        where
        //                            p.Ngay <= @toDate
        //                            and p.Ngay >= @fromDate
        //                            and MaXuong = @xuongId
        //                            and p.MaThanhPham = tp.Ma
        //                            and p.MaLoaiCa = tp.MaCa
        //                            and p.IsOffline = 0
        //                    ) tp1
        //                    LEFT JOIN(
        //                        Select
        //                            MaLo,
        //                            MaLoaiCa,
        //                            MaSize,
        //                            MaMau,
        //                            MaThanhPham,
        //                            CaTra,
        //                            DinhMuc,
        //                            Ngay,
        //                            MaXuong
        //                        from
        //                            (
        //                                Select
        //                                    d.*,
        //                                    ROW_NUMBER() OVER(
        //                                        PARTITION BY MaLo,
        //                                        MaLoaiCa,
        //                                        MaMau,
        //                                        MaSize,
        //                                        MaThanhPham,
        //                                        CaTra,
        //                                        Ngay
        //                                        ORDER BY
        //                                            Gio DESC
        //                                    ) AS[ROW NUMBER]
        //                                from
        //                                    DinhMucDinhHinh d
        //                                where
        //                                    d.Ngay <= @toDate
        //                                    and d.Ngay >= @fromDate
        //                                    And MaXuong = @xuongId
        //                                    --and IsOffline = 0
        //                            ) dm
        //                        Where
        //                            dm.[ROW NUMBER] = 1
        //                    ) tp2 on tp1.MaLo = tp2.MaLo
        //                    and tp1.MaLoaiCa = tp2.MaLoaiCa
        //                    and tp1.MaSize = tp2.MaSize
        //                    and tp1.MaMau = tp2.MaMau
        //                    and tp1.MaThanhPham = tp2.MaThanhPham
        //                    and tp1.CaTra = tp2.CaTra
        //                    and tp1.Ngay = tp2.Ngay
        //                    and tp1.MaXuong = tp2.MaXuong
        //            ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
        //            and p.MaLo = DinhMuc.MaLo
        //            and p.MaLoaiCa = DinhMuc.MaLoaiCa
        //            and p.MaSize = DinhMuc.MaSize
        //            and p.MaMau = DinhMuc.MaMau
        //            and p.MaThanhPham = DinhMuc.MaThanhPham
        //            And p.CaTra = DinhMuc.CaTra
        //            and p.Ngay = DinhMuc.Ngay
        //            LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
        //            LEFT Join MaSizeDinhHinh s on p.MaSize = s.Ma
        //            LEFT Join MaLoaiCaDinhHinh la on p.MaLoaiCa = la.Ma
        //            LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
        //            LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
        //        where
        //            p.STT > 0
        //            and p.Ngay <= @toDate
        //            and p.Ngay >= @fromDate
        //            and p.MaXuong = @xuongId
        //            and p.IsOffline = 0
        //        group by
        //            p.MaNhanVien,
        //            p.MaLo,
        //            p.MaLoaiCa,
        //            p.MaSize,
        //            p.MaThanhPham,
        //            p.MaMau,
        //            p.CaTra,
        //            DinhMuc.DinhMuc,
        //            p.Ngay,
        //            p.ChiSanLuong,
        //            n.MaHoSo,
        //            n.Name,
        //            la.Ten,
        //            tp.Ten,
        //            tp.BravoId,
        //            tp.BaoCaoDauRot,
        //            ma.Ten,
        //            s.Ten,
        //            n.DeptCode0,
        //            n.DeptName0,
        //            n.LoaiSanLuong,
        //            p.MaXuong
        //    ) tp on btp.Ngay = tp.Ngay
        //    and btp.CaTra = tp.CaTra
        //    and btp.MaLoaiCa = tp.MaLoaiCa
        //    and btp.MaMau = tp.MaMau
        //    and btp.MaNhanVien = tp.MaNhanVien
        //    and btp.MaSize = tp.MaSize
        //    and btp.MaThanhPham = tp.MaThanhPham
        //    and btp.MaXuong = tp.MaXuong
        //    and btp.ChiSanLuong = tp.ChiSanLuong
        //    and btp.MaLo = tp.MaLo
        //--    where
        //    -- - tp.TrongLuongNhan IS NOT NULL
        //order by
        //    tp.MaHoSo";
        //them checkinot để lấy thoiqf gian vao ra của công nhân
        var query = $@"
        declare @toDate as Date,@fromDate as Date,@xuongId as varchar(20),@isDinhMucBinhThuong as bit,@isFloor as bit;
        set @toDate = '{toDate:yyyy-MM-dd}';
        set @fromDate='{fromDate:yyyy-MM-dd}';
        set @xuongId='{xuongId}';
        set @isDinhMucBinhThuong ={(isDinhMucBinhThuong ? "1" : "0")};
        set @isFloor ={(isFloor ? "1" : "0")};
         ;
WITH CheckInOutData AS (
    SELECT
        c.MaChamCong,
        MIN(c.ThoiGian) AS ThoiGianVao,
        MAX(c.ThoiGian) AS ThoiGianRa
    FROM
        CheckInOut c
    WHERE
        c.ThoiGian >= @fromDate
        AND c.ThoiGian <= @toDate
    GROUP BY
        c.MaChamCong
)
Select
    p.*
    ,
    thoiGian.TongThoiGian,
    thoiGian.ThoiGianVao,
    thoiGian.ThoiGianRa
from
    (
        select
            IsNull(tp.Ngay, btp.Ngay) as Ngay,
            tp.MaNhanVien as MaNhanVien,
            tp.MaHoSo as MaHoSo,
            tp.NhanVienName as NhanVienName,
            IsNull(tp.MaLo, btp.MaLo) as MaLo,
            tp.MaNhom as MaNhom,
            tp.Nhom as Nhom,
            ISNULL(tp.MaLoaiCa, btp.MaLoaiCa) as MaLoaiCa,
            ISNULL(tp.LoaiCaName, btp.LoaiCaName) as LoaiCaName,
            ISNULL(tp.MaSize, btp.MaSize) as MaSize,
            ISNULL(tp.SizeName, btp.SizeName) as SizeName,
            ISNULL(tp.MaThanhPham, btp.MaThanhPham) as MaThanhPham,
            ISNULL(tp.ThanhPhamName, btp.ThanhPhamName) as ThanhPhamName,
            ISNULL(tp.BaoCaoDauRot, btp.BaoCaoDauRot) as BaoCaoDauRot,
            ISNULL(tp.MaSanPham, btp.MaSanPham) as MaSanPham,
            --ISNULL(tp.MaMau, btp.MaMau) as MaMau,
            --ISNULL(tp.MauName, btp.MauName) as MauName,
            ISNULL(tp.CaTra, btp.CaTra) as CaTra,
            IsNull(tp.ChiSanLuong, btp.ChiSanLuong) as ChiSanLuong,
            ISNULL(tp.TrongLuongNhan, 0) as TrongLuongNhan,
            ISNULL(tp.TrongLuongTra, 0) as TrongLuongTra,
            --IsNull(tp.LoaiSanLuong, btp.LoaiSanLuong) * ISNULL(tp.TrongLuongNhan, 0) as TrongLuongNhanChucNang,
            --IsNull(tp.LoaiSanLuong, btp.LoaiSanLuong) * ISNULL(tp.TrongLuongTra, 0) as TrongLuongTraChucNang,
            ISNULL(tp.DinhMucThucTe, 0) as DinhMuc,
            ISNULL(tp.DinhMucYeuCau, 0) as DinhMucChuan,
            cast(
                (
                    Case
                        when @isDinhMucBinhThuong = 1 then(
                            case
                                when ISNULL(tp.DinhMucThucTe, 0) > ISNULL(tp.DinhMucYeuCau, 0) then 0
                                else 1
                            end
                        )
                        else case
                            when ISNULL(tp.DinhMucThucTe, 0) < ISNULL(tp.DinhMucYeuCau, 0) then 0
                            else 1
                        end
                    end
                ) as bit
            ) as DanhGia,
            ISNULL(tp.SoRo, 0) as SoRoTP,
            ISNULL(btp.SoRo, 0) as SoRoBTP,
            ISNULL(btp.SoRoHuy, 0) as SoRoHuy,
            ISNULL(btp.SoRoChuaCanTP, 0) as SoRoChuaCanTP,
            ISNULL(btp.SoRoHuy, 0) + ISNULL(btp.SoRoChuaCanTP, 0) as SoRoLoi,
            ISNULL(tp.MaXuong, btp.MaXuong) as MaXuong --, tp.ThoiGianVao,
            --,
            -- isnull(
            --     cast(tp.Ngay as datetime) + cast(tp.ThoiGianVao as datetime),
            --     tp.Ngay
            -- ) as ThoiGianVao,
            -- isnull(
            --     cast(tp.Ngay as datetime) + cast(tp.ThoiGianRa as datetime),
            --     tp.Ngay
            -- ) as ThoiGianRa,
            -- tp.TongThoiGian as TongThoiGian
        from
            (
                Select
                    p.Ngay,
                    --p.MaNhanVien,
                    --n.MaHoSo,
                    --n.Name as NhanVienName,
                    --n.LoaiSanLuong,
                    --n.DeptCode0 as MaNhom,
                    --n.DeptName0 as Nhom,
                    la.Ten as LoaiCaName,
                    s.Ten as SizeName,
                    tp.Ten as ThanhPhamName,
                    tp.BravoId as MaSanPham,
                    tp.BaoCaoDauRot as BaoCaoDauRot,
                    --ma.Ten as MauName,
                    p.MaLoaiCa,
                    p.MaLo,
                    --p.MaMau,
                    p.MaThanhPham,
                    p.MaSize,
                    p.CaTra,
                    p.ChiSanLuong,
                    p.MaXuong,
                    SUM(
                        case
                            when ISNULL(p.GhiChu, '') = 'HUY' then 0
                            else 1
                        end
                    ) as SoRo,
                    SUM(
                        case
                            when ISNULL(p.GhiChu, '') = 'HUY' then 1
                            else 0
                        end
                    ) as SoRoHuy,
                    Sum(
                        case
                            when p.IsEnabled = 0 then 1
                            else 0
                        end
                    ) as SoRoChuaCanTP --ISNULL(c.ThoiGianVao, '1900-01-01') AS ThoiGianVao,
                    --ISNULL(c.ThoiGianRa, '1900-01-01') AS ThoiGianRa,
                    --DATEDIFF(hour, ISNULL(c.ThoiGianVao, '1900-01-01'), ISNULL(c.ThoiGianRa, '1900-01-01')) AS TongThoiGian
                from
                    PhieuCanBTPDinhHinh p
                    LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
                    LEFT Join MaSizeDinhHinh s on p.MaSize = s.Ma
                    LEFT Join MaLoaiCaDinhHinh la on p.MaLoaiCa = la.Ma --LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
                    --LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
                    --left join CheckInOutData c on n.MaChamCong = c.MaChamCong
                where
                    p.STT > 0
                    and p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and p.MaXuong = @xuongId
                    and p.IsOffline = 0
                GROUP BY
                    p.Ngay,
                    --p.MaNhanVien,
                    p.MaLoaiCa,
                    p.MaLo,
                    --p.MaMau,
                    p.MaThanhPham,
                    p.MaSize,
                    p.CaTra,
                    p.ChiSanLuong,
                    --n.LoaiSanLuong,
                    p.MaXuong,
                    --n.MaHoSo,
                    --n.Name,
                    la.Ten,
                    --ma.Ten,
                    tp.Ten,
                    s.Ten,
                    --n.DeptCode0,
                    --n.DeptName0,
                    tp.BravoId,
                    tp.BaoCaoDauRot --c.ThoiGianVao,
                    --c.ThoiGianRa
            ) btp
            left join(
                Select
                    p.Ngay,
                    p.MaNhanVien,
                    n.MaHoSo,
                    n.Name as NhanVienName,
                    n.DeptCode0 as MaNhom,
                    n.DeptName0 as Nhom,
                    p.MaLo,
                    p.MaLoaiCa,
                    la.Ten as LoaiCaName,
                    p.MaSize,
                    s.Ten as SizeName,
                    p.MaThanhPham,
                    tp.Ten as ThanhPhamName,
                    tp.BravoId as MaSanPham,
                    tp.BaoCaoDauRot as BaoCaoDauRot,
                    --p.MaMau,
                    --ma.Ten as MauName,
                    p.CaTra,
                    p.ChiSanLuong,
                    n.LoaiSanLuong,
                    Sum(p.TrongLuongNhan) as TrongLuongNhan,
                    Sum(p.TrongLuongTra) as TrongLuongTra,
                    case
                        when @isDinhMucBinhThuong = 1 then cast(
                            case
                                when sum(p.TrongLuongTra * n.LoaiSanLuong) = 0 then 0
                                else (
                                    case
                                        when @isFloor = 1 then(
                                            FLOOR(
                                                (
                                                    Sum(p.TrongLuongNhan * n.LoaiSanLuong) / sum(p.TrongLuongTra * n.LoaiSanLuong)
                                                ) * 1000
                                            ) / 1000
                                        )
                                        else Sum(p.TrongLuongNhan * n.LoaiSanLuong) / sum(p.TrongLuongTra * n.LoaiSanLuong)
                                    end
                                )
                            end as decimal(18, 3)
                        )
                        ELSE cast(
                            case
                                when sum(p.TrongLuongNhan * n.LoaiSanLuong) = 0 then 0
                                else sum(p.TrongLuongTra * n.LoaiSanLuong) / sum(p.TrongLuongNhan * n.LoaiSanLuong)
                            end as decimal(18, 4)
                        )
                    end as DinhMucThucTe,
                    isnull(DinhMuc.DinhMuc, tp.DinhMuc) as DinhMucYeuCau,
                    Count(*) as SoRo,
                    p.MaXuong -- Max(p.Gio) over (partition by p.MaNhanVien, p.Ngay, p.MaXuong) as ThoiGianRa,
                    -- Min(p.Gio) over (partition by p.MaNhanVien, p.Ngay, p.MaXuong) as ThoiGianVao,
                    -- DATEDIFF(
                    --     MINUTE,
                    --     Min(p.Gio) over (partition by p.MaNhanVien, p.Ngay, p.MaXuong),
                    --     Max(p.Gio) over (partition by p.MaNhanVien, p.Ngay, p.MaXuong)
                    -- ) as TongThoiGian -- ISNULL(c.ThoiGianVao, '1900-01-01') AS ThoiGianVao,
                    -- ISNULL(c.ThoiGianRa, '1900-01-01') AS ThoiGianRa,
                    -- DATEDIFF(
                    --     hour,
                    --     ISNULL(c.ThoiGianVao, '1900-01-01'),
                    --     ISNULL(c.ThoiGianRa, '1900-01-01')
                    -- ) AS TongThoiGian
                from
                    PhieuCanTPDinhHinh p
                    LEFT JOIN(
                        Select
                            tp1.MaLo,
                            tp1.MaLoaiCa,
                            tp1.MaSize,
                            --tp1.MaMau,
                            tp1.MaThanhPham,
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
                                    --p.MaMau,
                                    p.MaThanhPham,
                                    p.CaTra,
                                    CASE
                                        WHEN p.CaTra = 1 THEN 1.37
                                        ELSE tp.DinhMuc
                                    END AS DinhMuc,
                                    p.Ngay,
                                    p.MaXuong
                                from
                                    PhieuCanTPDinhHinh p,
                                    MaThanhPhamDinhHinh tp
                                where
                                    p.Ngay <= @toDate
                                    and p.Ngay >= @fromDate
                                    and MaXuong = @xuongId
                                    and p.MaThanhPham = tp.Ma
                                    and p.MaLoaiCa = tp.MaCa
                                    and p.IsOffline = 0
                            ) tp1
                            LEFT JOIN(
                                Select
                                    MaLo,
                                    MaLoaiCa,
                                    MaSize,
                                    --MaMau,
                                    MaThanhPham,
                                    CaTra,
                                    DinhMuc,
                                    Ngay,
                                    MaXuong
                                from
                                    (
                                        Select
                                            d.*,
                                            ROW_NUMBER() OVER(
                                                PARTITION BY MaLo,
                                                MaLoaiCa,
                                                --MaMau,
                                                MaSize,
                                                MaThanhPham,
                                                CaTra,
                                                Ngay
                                                ORDER BY
                                                    Gio DESC
                                            ) AS [ROW NUMBER]
                                        from
                                            DinhMucDinhHinh d
                                        where
                                            d.Ngay <= @toDate
                                            and d.Ngay >= @fromDate
                                            And MaXuong = @xuongId --and IsOffline = 0
                                    ) dm
                                Where
                                    dm.[ROW NUMBER] = 1
                            ) tp2 on tp1.MaLo = tp2.MaLo
                            and tp1.MaLoaiCa = tp2.MaLoaiCa
                            and tp1.MaSize = tp2.MaSize --and tp1.MaMau = tp2.MaMau
                            and tp1.MaThanhPham = tp2.MaThanhPham
                            and tp1.CaTra = tp2.CaTra
                            and tp1.Ngay = tp2.Ngay
                            and tp1.MaXuong = tp2.MaXuong
                    ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
                    and p.MaLo = DinhMuc.MaLo
                    and p.MaLoaiCa = DinhMuc.MaLoaiCa
                    and p.MaSize = DinhMuc.MaSize --and p.MaMau = DinhMuc.MaMau
                    and p.MaThanhPham = DinhMuc.MaThanhPham
                    And p.CaTra = DinhMuc.CaTra
                    and p.Ngay = DinhMuc.Ngay
                    LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
                    LEFT Join MaSizeDinhHinh s on p.MaSize = s.Ma
                    LEFT Join MaLoaiCaDinhHinh la on p.MaLoaiCa = la.Ma --LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
                    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
                    left join CheckInOutData c on c.MaChamCong = c.MaChamCong
                where
                    p.STT > 0
                    and p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and p.MaXuong = @xuongId
                    and p.IsOffline = 0
                group by
                    p.MaNhanVien,
                    p.MaLo,
                    p.MaLoaiCa,
                    p.MaSize,
                    p.MaThanhPham,
                    --p.MaMau,
                    p.CaTra,
                    DinhMuc.DinhMuc,
                    -- p.Gio,
                    p.Ngay,
                    p.ChiSanLuong,
                    n.MaHoSo,
                    n.Name,
                    la.Ten,
                    tp.Ten,
                    tp.BravoId,
                    tp.BaoCaoDauRot,
                    --ma.Ten,
                    s.Ten,
                    n.DeptCode0,
                    n.DeptName0,
                    n.LoaiSanLuong,
                    p.MaXuong,
                    c.ThoiGianVao,
                    c.ThoiGianRa,
                    tp.DinhMuc
            ) tp on btp.Ngay = tp.Ngay
            and btp.CaTra = tp.CaTra
            and btp.MaLoaiCa = tp.MaLoaiCa --and btp.MaMau = tp.MaMau
            --and btp.MaNhanVien = tp.MaNhanVien
            and btp.MaSize = tp.MaSize
            and btp.MaThanhPham = tp.MaThanhPham
            and btp.MaXuong = tp.MaXuong
            and btp.ChiSanLuong = tp.ChiSanLuong
            and btp.MaLo = tp.MaLo --    where
            -- - tp.TrongLuongNhan IS NOT NULL
    ) p
    LEFT join (
        Select
            p.MaXuong,
            p.Ngay,
            p.MaNhanVien,
            cast(p.Ngay as datetime) + cast(Min(btp.Gio) as datetime) as ThoiGianVao,
            cast(p.Ngay as datetime) + cast(Max(p.Gio) as datetime) as ThoiGianRa,
            cast( cast (DATEDIFF(
                MINUTE,
                cast(p.Ngay as datetime) + cast(Min(btp.Gio) as datetime),
                cast(p.Ngay as datetime) + cast(Max(p.Gio) as datetime)
            ) as decimal(18,2))/60 as decimal(18,2))  as TongThoiGian
        from
            PhieuCanTPDinhHinh p,
            PhieuCanBTPDinhHinh btp
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
where
    p.MaNhanVien is not null
order by
    p.MaHoSo
        	";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var items = connection.Query<T>(
                    query)
                .ToList();
            return items;
        }
    }

    // Bao Cao Tong hop 2 + TongHop
    public List<T> GetTongHopNhanVienTheoDoiLois<T>(
        DateTime dateTime,
        string xuongId,
        bool isDinhMucBinhThuong = true)
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
        when @isDinhMucBinhThuong = 1 then case
            when tp.DinhMuc <= (
                case
                    when tp.CaTra = 1
                    and dm.DinhMuc is null then 1.37
                    else isnull(dm.DinhMuc, tp._DinhMucChuan)
                end
            ) then N'Đ'
            ELSE N'Không Đạt'
        end
        else case
            when tp.DinhMuc >= (
                case
                    when tp.CaTra = 1
                    and dm.DinhMuc is null then 1.37
                    else isnull(dm.DinhMuc, tp._DinhMucChuan)
                end
            ) then N'Đ'
            ELSE N'Không Đạt'
        end
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
            case
                when @isDinhMucBinhThuong = 1 then floor(
                    1000 * Sum(ptp.TrongLuongNhan) / sum(ptp.TrongLuongTra)
                ) / 1000
                else Sum(ptp.TrongLuongTra) / sum(ptp.TrongLuongNhan)
            end as DinhMuc,
            tp.DinhMuc as _DinhMucChuan
        from
            PhieuCanTPDinhHinh ptp,
            NhanVienDaiThanh n,
            MaThanhPhamDinhHinh tp,
            MaSizeDinhHinh s
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
                    DinhMucDinhHinh d
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
            PhieuCanBTPDinhHinh
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
            var items = connection.QueryAsync<T>(
                    query,
                    new { ngay = dateTime.Date, xuongId, isDinhMucBinhThuong })
                .Result
                .ToList();
            return items;
        }
    }

    public List<T> GetTongHopToType1<T>(DateTime dateTime, string xuongId, int batdau, int ketthuc, string toName)
    {
        var query =
            @"SELECT @toName As ToName,p.CaTra, p.MaLo , la.Ten As LoaiCaName,tp.Ma As MaThanhPham,tp.Ten As ThanhPhamName,COUNT(p.STT) As SoRo,SUM(p.TrongLuongTra) as TrongLuongTra,SUM(p.TrongLuongNhan) as TrongLuongNhan,FLOOR((SUM(p.TrongLuongNhan)/SUM(p.TrongLuongTra)) *1000)/1000 as DinhMuc from PhieuCanTPDinhHinh p,MaThanhPhamDinhHinh tp,MaLoaiCaDinhHinh la, NhanVienDaiThanh n where p.MaLoaiCa = la.Ma and p.MaLoaiCa = tp.MaCa and p.MaThanhPham = tp.Ma and p.Ngay=@ngay and p.MaXuong =@xuongId and p.MaNhanVien = n.MaNhanVien and (case when ISNUMERIC(n.MaHoSo) = 1 then cast (n.MaHoSo as int) else null
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
            @"SELECT @toName As ToName,p.CaTra, p.MaLo , la.Ten As LoaiCaName,tp.Ma As MaThanhPham,tp.Ten As ThanhPhamName,COUNT(p.STT) As SoRo,SUM(p.TrongLuongTra) as TrongLuongTra,SUM(p.TrongLuongNhan) as TrongLuongNhan,FLOOR((SUM(p.TrongLuongNhan)/SUM(p.TrongLuongTra)) *1000)/1000 as DinhMuc from PhieuCanTPDinhHinh p,MaThanhPhamDinhHinh tp,MaLoaiCaDinhHinh la, NhanVienDaiThanh n where p.MaLoaiCa = la.Ma and p.MaLoaiCa = tp.MaCa and p.MaThanhPham = tp.Ma and p.Ngay=@ngay and p.MaXuong =@xuongId and p.MaNhanVien = n.MaNhanVien and ((case when ISNUMERIC(n.MaHoSo) = 1 then cast (n.MaHoSo as int) else 0
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
            @"SELECT @toName As ToName,p.CaTra, p.MaLo , la.Ten As LoaiCaName,tp.Ma As MaThanhPham,tp.Ten As ThanhPhamName,COUNT(p.STT) As SoRo,SUM(p.TrongLuongTra) as TrongLuongTra,SUM(p.TrongLuongNhan) as TrongLuongNhan,FLOOR((SUM(p.TrongLuongNhan)/SUM(p.TrongLuongTra)) *1000)/1000 as DinhMuc from PhieuCanTPDinhHinh p,MaThanhPhamDinhHinh tp,MaLoaiCaDinhHinh la, NhanVienDaiThanh n where p.MaLoaiCa = la.Ma and p.MaLoaiCa = tp.MaCa and p.MaThanhPham = tp.Ma and p.Ngay=@ngay and p.MaXuong =@xuongId and p.MaNhanVien = n.MaNhanVien and ((case when ISNUMERIC(n.MaHoSo) = 1 then cast (n.MaHoSo as int) else 0
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

    public List<T> GetTongHopTyLeThoiGianVaDinhMuc<T>(DateTime fromDate,DateTime toDate,string xuongId,int MocThoiGian)
    {
        var query = @"select
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
                                            PhieuCanTPDinhHinh p
                                        where
                                            Ngay >= @fromDate and Ngay <= @toDate and MaXuong = @xuongId and p.TrongLuongTra >0
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
                                                    DinhMucDinhHinh d
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
                                    LEFT JOIN MaThanhPhamDinhHinh tp on p.MaThanhPham = tp.Ma
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
                            PhieuCanTPDinhHinh tp,
                            PhieuCanBTPDinhHinh btp
                        where
							tp.Ngay >= @fromDate and tp.Ngay <= @toDate
							and btp.Ngay >= @fromDate and btp.Ngay <= @toDate
                            And tp.MaXuong = @xuongId
                            and tp.IdIn = btp.Id and btp.TrongLuong > 0
                    ) p
            ) p";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(
            query,
            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, MocThoiGian }).Result.ToList();
        return items;
    }
    public List<T> GetTongHopTyLeThoiGianVaDinhMucForGrid<T>(DateTime fromDate,DateTime toDate,string xuongId,int MocThoiGian)
    {
        var query = @"select
    *
from
(
    Select
        category = 'DinhMuc',
        p.Ngay,
        cast(
            p.SoLuongDatDinhMuc /(p.SoLuongDatDinhMuc + p.SoLuongKhongDatDinhMuc) as DECIMAL(18, 4)
        ) as TyLeDat,
        cast(
            p.SoLuongKhongDatDinhMuc /(p.SoLuongDatDinhMuc + p.SoLuongKhongDatDinhMuc) as DECIMAL(18, 4)
        ) as TyLeKhongDat,
        p.SoRoDat,
        p.SoRoKhongDat
    from
    (
        Select
            Ngay,
            cast(
                sum(case when DanhGia = 1 then 1 else 0 end) as decimal(18, 2)
            ) as SoLuongDatDinhMuc,
            cast(
                sum(case when DanhGia = 0 then 1 else 0 end) as decimal(18, 2)
            ) as SoLuongKhongDatDinhMuc,
            cast(
                sum(case when DanhGia = 1 then SoLuong else 0 end) as decimal(18, 2)
            ) as SoRoDat,
            cast(
                sum(case when DanhGia = 0 then SoLuong else 0 end) as decimal(18, 2)
            ) as SoRoKhongDat
        from
        (
            SELECT
                p.Ngay,
                SoLuong,
                case
                    WHEN p.DinhMucThuc < p.DinhMucChuan THEN 1 ELSE 0
                END AS DanhGia
            from
            (
                Select
                    p.*,
                    cast(p.TrongLuongNhan / p.TrongLuongTra as DECIMAL(18, 2)) as DinhMucThuc,
                    Cast(ISNULL(dm.DinhMuc, tp.DinhMuc) as DECIMAL(18, 2)) as DinhMucChuan
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
                    from PhieuCanTPDinhHinh
                    where Ngay >= @fromDate and Ngay <= @toDate and MaXuong = @xuongId
                    group by Ngay, MaNhanVien, MaThanhPham, MaLo, MaLoaiCa, MaMau, MaSize
                ) p
                LEFT JOIN (
                    Select *
                    from (
                        Select
                            d.*,
                            ROW_NUMBER() OVER (
                                PARTITION BY MaLo, MaLoaiCa, MaMau, MaSize, MaThanhPham
                                ORDER BY Gio DESC
                            ) AS [ROW NUMBER]
                        from DinhMucDinhHinh d
                        where Ngay >= @fromDate and Ngay <= @toDate and MaXuong = @xuongId
                    ) dm
                    where [ROW NUMBER] = 1
                ) dm
                ON p.MaThanhPham = dm.MaThanhPham
                and p.MaLo = dm.MaLo
                and p.MaSize = dm.MaSize
                and p.MaMau = dm.MaMau
                and p.MaLoaiCa = dm.MaLoaiCa
                and p.Ngay = dm.Ngay
                LEFT JOIN MaThanhPhamDinhHinh tp ON p.MaThanhPham = tp.Ma
            ) p
        ) p
        group by Ngay
    ) p
) p1

UNION ALL

Select
    category = 'ThoiGian',
    p.Ngay,
    cast(p.SoLuongDatDinhMuc /(p.SoLuongDatDinhMuc + p.SoLuongKhongDatDinhMuc) as DECIMAL(18, 4)) as TyLeDat,
    cast(p.SoLuongKhongDatDinhMuc /(p.SoLuongDatDinhMuc + p.SoLuongKhongDatDinhMuc) as DECIMAL(18, 4)) as TyLeKhongDat,
    p.SoLuongDatDinhMuc as SoRoDat,
    p.SoLuongKhongDatDinhMuc as SoRoKhongDat
from
(
    Select
        Ngay,
        cast(sum(case when DanhGia = 1 then 1 else 0 end) as decimal(18, 2)) as SoLuongDatDinhMuc,
        cast(sum(case when DanhGia = 0 then 1 else 0 end) as DECIMAL(18, 2)) as SoLuongKhongDatDinhMuc
    from
    (
        Select
            tp.Ngay,
            case
                when DATEDIFF(MINUTE, cast(btp.Gio as datetime), cast(tp.Gio as datetime)) <= @MocThoiGian then 1
                else 0
            end as DanhGia
        from
            PhieuCanTPDinhHinh tp
            join PhieuCanBTPDinhHinh btp on tp.IdIn = btp.Id
        where
            tp.Ngay >= @fromDate and tp.Ngay <= @toDate
            and btp.Ngay >= @fromDate and btp.Ngay <= @toDate
            and tp.MaXuong = @xuongId
    ) p
    group by Ngay
) p
order by Ngay";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(
            query,
            new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, MocThoiGian }).Result.ToList();
        return items;
    }

    public List<T> GetTongHopTyLeThoiGianVaDinhMucTheoNhom<T>(DateTime fromDate, DateTime toDate,string xuongId,int MocThoiGian1, int MocThoiGian2)
    {
        var query = @"-- ĐM THEO NHÓM
SELECT
    category = 'DinhMuc',
    p.GroupName,
    CAST(1.0 * p.SoRoDat / NULLIF(p.SoRoDat + p.SoRoKhongDat, 0) AS DECIMAL(18, 4)) AS TyLeDat,
    CAST(1.0 * p.SoRoKhongDat / NULLIF(p.SoRoDat + p.SoRoKhongDat, 0) AS DECIMAL(18, 4)) AS TyLeKhongDat,
	0.000 as TyLeKhongDat2,
    p.SoRoDat,
    p.SoRoKhongDat,
	0 as SoRoKhongDat2
FROM (
    SELECT
        nv.DeptName0 AS GroupName,
        SUM(CASE WHEN DanhGia = 1 THEN SoLuong ELSE 0 END) AS SoRoDat,
        SUM(CASE WHEN DanhGia = 0 THEN SoLuong ELSE 0 END) AS SoRoKhongDat
    FROM (
        SELECT
            p.MaNhanVien,
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
                FROM PhieuCanTPDinhHinh
                WHERE 
				Ngay >= @fromDate and Ngay <= @toDate
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
                    FROM DinhMucDinhHinh d
                    WHERE Ngay >= @fromDate and Ngay <= @toDate AND MaXuong = @xuongId
                ) dm
                WHERE rn = 1
            ) dm ON p.MaThanhPham = dm.MaThanhPham
                 AND p.MaLo = dm.MaLo
                 AND p.MaSize = dm.MaSize
                 AND p.MaMau = dm.MaMau
                 AND p.MaLoaiCa = dm.MaLoaiCa
                 AND p.Ngay = dm.Ngay
            LEFT JOIN MaThanhPhamDinhHinh tp ON p.MaThanhPham = tp.Ma
        ) p
    ) p
    JOIN NhanVienDaiThanh nv ON p.MaNhanVien = nv.MaNhanVien
    GROUP BY nv.DeptName0
) p
UNION ALL
--ThoiGian theo Nhóm
SELECT
    category = 'ThoiGian',
    p.GroupName,
    CAST(1.0 * p.SoRoDat / NULLIF(p.Total, 0) AS DECIMAL(18, 4)) AS TyLeDat,
    CAST(1.0 * p.SoRoKhongDat / NULLIF(p.Total, 0) AS DECIMAL(18, 4)) AS TyLeKhongDat,
    CAST(1.0 * p.SoRoKhongDat2 / NULLIF(p.Total, 0) AS DECIMAL(18, 4)) AS TyLeKhongDat2,
    p.SoRoDat,
    p.SoRoKhongDat,
    p.SoRoKhongDat2
FROM (
    SELECT
        nv.DeptName0 AS GroupName,
        SUM(CASE WHEN DanhGia = 'Dat' THEN 1 ELSE 0 END) AS SoRoDat,
        SUM(CASE WHEN DanhGia = 'KhongDat1' THEN 1 ELSE 0 END) AS SoRoKhongDat,
        SUM(CASE WHEN DanhGia = 'KhongDat2' THEN 1 ELSE 0 END) AS SoRoKhongDat2,
        COUNT(*) AS Total
    FROM (
        SELECT
            tp.MaNhanVien,
            DanhGia = CASE 
                WHEN DATEDIFF(MINUTE, CAST(btp.Gio AS DATETIME), CAST(tp.Gio AS DATETIME)) <= @MocThoiGian1 THEN 'Dat'
                WHEN DATEDIFF(MINUTE, CAST(btp.Gio AS DATETIME), CAST(tp.Gio AS DATETIME)) <= @MocThoiGian2 THEN 'KhongDat1'
                ELSE 'KhongDat2'
            END
        FROM PhieuCanTPDinhHinh tp
        JOIN PhieuCanBTPDinhHinh btp ON tp.IdIn = btp.Id
        WHERE 
		tp.Ngay >= @fromDate and tp.Ngay <= @toDate
		and btp.Ngay >= @fromDate and btp.Ngay <= @toDate 
		AND tp.MaXuong = @xuongId
    ) sub
    JOIN NhanVienDaiThanh nv ON sub.MaNhanVien = nv.MaNhanVien
    GROUP BY nv.DeptName0
) p

ORDER BY category, GroupName;";
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var items = connection.QueryAsync<T>(
            query,
            new { fromDate = fromDate.Date,toDate = toDate.Date, xuongId, MocThoiGian1, MocThoiGian2 }).Result.ToList();
        return items;
    }



    //bao cao tong hop 3
    public List<T> GetTongHopTypeTinhLuong<T>(DateTime dateTime, string xuongId, bool isDinhMucBinhThuong = true)
    {
        var query = @"SELECT
    p.*,
    Round(p.TrongLuongTra / p.SoRoTra, 2) as BinhQuan,
    case
        when @isDinhMucBinhThuong = 1 then Case
            when p.DinhMuc <= p.DinhMucChuan then N'Đ'
            else N'Không Đạt'
        end
        else Case
            when p.DinhMuc >= p.DinhMucChuan then N'Đ'
            else N'Không Đạt'
        end
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
            Case
                when @isDinhMucBinhThuong = 1 then floor(
                    1000 * SUM(p.TrongLuongNhan) / Sum (p.TrongLuongTra)
                ) / 1000
                else SUM(p.TrongLuongTra) / Sum (p.TrongLuongNhan)
            end as DinhMuc
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
                            case
                                when @isDinhMucBinhThuong = 1 then floor(
                                    1000 * Sum(ptp.TrongLuongNhan) / sum(ptp.TrongLuongTra)
                                ) / 1000
                                else Sum(ptp.TrongLuongTra) / sum(ptp.TrongLuongNhan)
                            end as DinhMuc,
                            tp.DinhMuc as _DinhMucChuan
                        from
                            PhieuCanTPDinhHinh ptp,
                            NhanVienDaiThanh n,
                            MaThanhPhamDinhHinh tp,
                            MaSizeDinhHinh s
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
                                    DinhMucDinhHinh d
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
                            PhieuCanBTPDinhHinh
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
            var items = connection.QueryAsync<T>(
                    query,
                    new { ngay = dateTime.Date, xuongId, isDinhMucBinhThuong })
                .Result
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
        var query = @"INSERT INTO [dbo].[PhieuCanTPDinhHinh]
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
           ,[GhiChu],[ChiSanLuong],[SuDung],[TrongLuongTare]  ,[TrongLuongBu],[IsOffline],[MaNhanVienPhucVu],[MaNhanVienBanKiem],[Id],[IdIn])
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
           ,@GhiChu,@ChiSanLuong,@SuDung, @TrongLuongTare , @TrongLuongBu,@IsOffline,@MaNhanVienPhucVu,@MaNhanVienBanKiem,@Id,@IdIn)";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var rows = connection.Execute(query, items);
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


    //public int InsertBatch(List<Models.Repos.Models.PhieuCanTPDinhHinh> phieuCans)
    //{
    //    try
    //    {
    //        var batches = ToolEx.DbExtensions.GetSqlsInBatches(phieuCans);
    //        var row = 0;
    //        var database = new Modelv1.Dao.Database(ConnectionString);
    //        foreach (var batche in batches)
    //        {
    //            row += database.ExecuteNonQuery(batche);
    //        }

    //        return row;
    //    }
    //    catch (Exception)
    //    {
    //        throw;
    //    }
    //}

    public int Update<T>(List<T> items)
    {
        var query = @"
UPDATE [dbo].[PhieuCanTPDinhHinh]
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
      ,[GhiChu] = @GhiChu,[ChiSanLuong] =@ChiSanLuong , [SuDung] = @SuDung,[TrongLuongTare] = @TrongLuongTare ,[TrongLuongBu] = @TrongLuongBu,[IsOffline] = @IsOffline,[MaNhanVienPhucVu] =@MaNhanVienPhucVu, [MaNhanVienBanKiem] =@MaNhanVienBanKiem
 WHERE [STT] = @STT and [Ngay]= @Ngay and [MaMayCan] = @MaMayCan and [MaXuong] = @MaXuong";
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var rows = connection.Execute(query, items);
            return rows;
        }
    }

//        public int UpdateDatabase()
//        {
//            try
//            {
//                //Thêm Cột Id Vào bảng MaLoaiCa trên máy cân đầu Ao
//                var query =
//                    @"DECLARE @tb varchar(30) = 'PhieuCanTPDinhHinh' IF COL_LENGTH(@tb, 'IsOffline') IS NULL BEGIN
//ALTER TABLE
//    PhieuCanTPDinhHinh
//ADD
//    IsOffline bit NOT NULL DEFAULT ((0))
//END
//IF COL_LENGTH(@tb, 'MaNhanVienPhucVu') IS NULL BEGIN
//ALTER TABLE
//    PhieuCanTPDinhHinh
//ADD
//    MaNhanVienPhucVu varchar(50) NULL
//END
//IF COL_LENGTH(@tb, 'MaNhanVienBanKiem') IS NULL BEGIN
//ALTER TABLE
//    PhieuCanTPDinhHinh
//ADD
//    MaNhanVienBanKiem varchar(50) NULL
//END
//";
//                var dao = new Dao.Repos.Database(connectionString);
//                return dao.ExecuteNonQuery(query);
//            }
//            catch (Exception exception)
//            {
//                throw new Exception(
//                    $@"Không thể cập nhật Cơ Sở Dữ Liệu vui lòng liên hệ PMS để được hổ trợ [Phiếu Cân TP Định Hình]. {Environment.NewLine}{exception.Message}");
//                //throw;
//            }
//        }
}