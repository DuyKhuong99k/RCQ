using Dapper;
using Microsoft.Data.SqlClient;
using Models.Repos.Models;
using System.Data;

namespace Dao.Repos.HQ
{
    public partial class PhieuCanTPFillet
    {
        private readonly string connectionString;
        private string tableName = @"PhieuCanTPFillet";
        private readonly string qrDelete = @"DELETE FROM [dbo].[PhieuCanTPFillet]
      WHERE [MaMayTinhCan] = @MaMayTinhCan 
      And [MaUserCan] = @MaUserCan 
      And [ThoiGianCan] = @ThoiGianCan 
      And [Ngay] = @Ngay";
        private readonly string qrInsert = @"INSERT INTO [dbo].[PhieuCanTPFillet]
           ([MaMayTinhCan]
           ,[MaUserCan]
           ,[ThoiGianCan]
           ,[Ngay]
           ,[MaXuongSanXuat]
           ,[MSL]
           ,[MaLoaiCa]
           ,[MaLoaiThanhPham]
           ,[MaSize]
           ,[MaMau]
           ,[MaNhanVien]
           ,[HoVaTen]
           ,[MaTheTu]
           ,[TrongLuong]
           ,[SuDung]
           ,[GhiChu],[MaNhanVienPhucVu],[LoaiCan],[TrongLuongTare])
     VALUES
           (@MaMayTinhCan 
           ,@MaUserCan 
           ,@ThoiGianCan 
           ,@Ngay 
           ,@MaXuongSanXuat 
           ,@MSL 
           ,@MaLoaiCa 
           ,@MaLoaiThanhPham 
           ,@MaSize 
           ,@MaMau 
           ,@MaNhanVien 
           ,@HoVaTen 
           ,@MaTheTu 
           ,@TrongLuong 
           ,@SuDung 
           ,@GhiChu,@MaNhanVienPhucVu,@LoaiCan,@TrongLuongTare)";

        private readonly string qrUpdate = @"UPDATE [dbo].[PhieuCanTPFillet]
   SET [MaXuongSanXuat] = @MaXuongSanXuat 
      ,[MSL] = @MSL 
      ,[MaLoaiCa] = @MaLoaiCa 
      ,[MaLoaiThanhPham] = @MaLoaiThanhPham 
      ,[MaSize] = @MaSize 
      ,[MaMau] = @MaMau 
      ,[MaNhanVien] = @MaNhanVien 
      ,[HoVaTen] = @HoVaTen 
      ,[MaTheTu] = @MaTheTu 
      ,[TrongLuong] = @TrongLuong 
      ,[SuDung] = @SuDung 
      ,[GhiChu] = @GhiChu ,[MaNhanVienPhucVu] = @MaNhanVienPhucVu,[LoaiCan] =@LoaiCan, [TrongLuongTare] = @TrongLuongTare
 WHERE [MaMayTinhCan] = @MaMayTinhCan 
      And [MaUserCan] = @MaUserCan 
      And [ThoiGianCan] = @ThoiGianCan 
      And [Ngay] = @Ngay ";

        private readonly string qrGetAll = "Select * from PhieuCanTPFillet";

        public PhieuCanTPFillet()
        {
            connectionString = AppViewModels.Base.Ins.ConnectionString;

        }
        public List<T> GetsTongHopMayCan<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query =
                    @$"Select MaMayTinhCan,SUM(TrongLuong) as TrongLuong from PhieuCanTPFillet where Ngay=@Ngay and MaXuongSanXuat = @xuongId Group by MaMayTinhCan order by MaMayTinhCan ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { Ngay = dateTime.Date, xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var query = @"
WITH RankedPhieu AS (
    SELECT *,
           ROW_NUMBER() OVER(PARTITION BY MaMayTinhCan ORDER BY Ngay DESC, ThoiGianCan DESC) AS RowNum
    FROM PhieuCanTPFillet where Ngay =@ngay
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
        public DataTable GetChiTiets(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"Select
    P.[Giờ],
    case
        when p.[MNV Trước] <> p.[Mã Nhân Viên] then '00:00:00'
        else p.[Giờ Trước]
    end as [Giờ Trước],
    dateadd(
        SECOND,
        case
            when ISNULL(p.GioPre, 0) <= 0
            and p.[MNV Trước] <> p.[Mã Nhân Viên] then 0
            else ISNULL(p.GioPre, 0)
        end,
        0
    ) as [TG Phiếu Liên Tiếp],
    p.[Mã Nhân Viên],
    p.[Mã Hồ Sơ],
    p.[Tên Nhân Viên],
    n.MaNhanVien as [Mã Nhân Viên Phục Vụ],
    n.MaHoSo as [Mã Hồ Sơ Phục vụ],
    n.Name as [Tên Phục Vụ],
    p.[Lô],
    p.[Loại Cá],
    p.[Thành Phẩm],
    p.Size,
    p.[Trọng Lượng],
    case
        when p.[MNV Trước] <> p.[Mã Nhân Viên] then 0
        else ISNULL(p.[TL Trước], 0)
    end as [TL Trước],
    p.[Xưởng]
from
    (
        Select
            CAST(@ngay as datetime) + cast(p.ThoiGianCan as datetime) as [Giờ],
            DATEDIFF(
                SECOND,
                LAG(p.ThoiGianCan, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                p.ThoiGianCan
            ) as [GioPre],
            CAST(@ngay as datetime) + cast(
                ISNULL(
                    LAG(p.ThoiGianCan, 1) over(
                        order by
                            p.MaXuongSanXuat,
                            p.MaNhanVien,
                            p.ThoiGianCan
                    ),
                    '00:00:00'
                ) as datetime
            ) as [Giờ Trước],
            ISNULL(
                LAG(p.TrongLuong, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                0
            ) as [TL Trước],
            p.MaNhanVien as [Mã Nhân Viên],
            ISNULL(
                LAG(p.MaNhanVien, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                ''
            ) as [MNV Trước],
            n.MaHoSo as [Mã Hồ Sơ],
            p.MaNhanVienPhucVu,
            n.Name as [Tên Nhân Viên],
            isnull(p.LoTheoLine, p.MSL) as [Lô],
            la.Ten as [Loại Cá],
            isnull(p.ThanhPhamName,tp.Ten) as [Thành Phẩm],
            ISNULL(p.SizeName, s.Ten) as [Size],
            p.TrongLuong as [Trọng Lượng],
            p.MaMayTinhCan,
            p.MaXuongSanXuat as [Xưởng]
        from
            (
                Select
                    p.*,
                    case when tp2.IsNotSetByTime =1 then s2.Ma else bt.MaSize end 
                     as MaSizeBoTri,
                     case when tp2.IsNotSetByTime =1 then s2.Ten else s.Ten end as SizeName,
                    case when tp2.IsNotSetByTime =1 then NULL else bt.MaThanhPham end as MaThanhPhamBoTri,
                     case when tp2.IsNotSetByTime =1 then NULL else  tp.Ten end  as ThanhPhamName
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
                                    and nl.Ngay = @ngay
                                    and p.MaNhanVien = nl.MaNhanVien
                                    and nl.Gio = (
                                        Select
                                            MAX(nlc.Gio)
                                        from
                                            NhanVienTheoLine nlc
                                        where
                                            nlc.Ngay = @ngay
                                            and nlc.MaNhanVien = p.MaNhanVien
                                            and nlc.Gio <= p.ThoiGianCan
                                    )
                                    left join XiNghiep xn on xn.Ma = p.MaXuongSanXuat
                                    left join LineFilletv2 ln on nl.MaLine = ln.Ma
                                    left join ViTriFillet vt on nl.MaViTri = vt.Ma
                                where
                                    p.Ngay = @ngay
                                    and p.MaXuongSanXuat = @xuongId
                            ) p
                            left JOIN LoTheoLine ll ON p.MaLine = ll.MaLine
                            and p.Ngay = ll.Ngay
                            and p.CodeId = ll.CodeId
                            and ll.Ngay = @ngay
                            and ll.Gio = (
                                Select
                                    MAX(llc.Gio)
                                from
                                    LoTheoLine llc
                                where
                                    llc.MaLine = p.MaLine
                                    and llc.CodeId = p.CodeId
                                    and llc.Ngay = @ngay
                                    and llc.Gio <= p.ThoiGianCan
                            )
                    ) p
                    left JOIN BoTriLoSizeThanhPham bt ON p.LoTheoLine = bt.MaLo
                    and p.CodeId = bt.CodeId
                    and p.MaViTri = bt.MaViTri
                    and bt.Ngay = @ngay
                    and bt.Gio = (
                         Select
                    MAX(btc.Gio)
                from
                    BoTriLoSizeThanhPham btc
                where
                    btc.MaLo = p.LoTheoLine
                    and btc.MaViTri = p.MaViTri
                    and btc.CodeId = p.CodeId
                    and btc.Ngay = @ngay
                    and btc.Gio <= p.ThoiGianCan
                    )
                    left join MaThanhPhamFillet tp on bt.MaThanhPham = tp.Ma  and tp.MaCa = p.MaLoaiCa
                    left join MaThanhPhamFillet tp2 on p.MaLoaiThanhPham = tp2.Ma  and tp2.MaCa = p.MaLoaiCa
                    LEFT join MaSizeFillet s on bt.MaSize = s.Ma
                    LEFT join MaSizeFillet s2 on bt.MaSizePhu = s2.Ma
            ) p
            left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            left join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
            left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma  and p.MaLoaiCa = tp.MaCa
            LEFT join MaSizeFillet s on p.MaSize = s.Ma
    ) p
    left join NhanVienDaiThanh n on p.MaNhanVienPhucVu = n.MaNhanVien
Order By
    p.[Xưởng],
    p.[Mã Nhân Viên],
    p.[Giờ]";
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
        public DataTable GetChiTiets(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query = $@"Select
    p.Ngay,
    P.[Giờ],
    case
        when p.[MNV Trước] <> p.[Mã Nhân Viên] then '00:00:00'
        else p.[Giờ Trước]
    end as [Giờ Trước],
    dateadd(
        SECOND,
        case
            when ISNULL(p.GioPre, 0) <= 0
            and p.[MNV Trước] <> p.[Mã Nhân Viên] then 0
            else ISNULL(p.GioPre, 0)
        end,
        0
    ) as [TG Phiếu Liên Tiếp],
    p.[Mã Nhân Viên],
    p.[Mã Hồ Sơ],
    p.[Tên Nhân Viên],
    n.MaNhanVien as [Mã Nhân Viên Phục Vụ],
    n.MaHoSo as [Mã Hồ Sơ Phục vụ],
    n.Name as [Tên Phục Vụ],
    p.[Lô],
    p.[Loại Cá],
	p.MaThanhPham,
    p.[Thành Phẩm],
    p.Size,
    p.[Trọng Lượng],
    p.TrongLuongTare,
    case
        when p.[MNV Trước] <> p.[Mã Nhân Viên] then 0
        else ISNULL(p.[TL Trước], 0)
    end as [TL Trước],
    p.[Xưởng]
from
    (
        Select
            p.Ngay,
            CAST(@ngay as datetime) + cast(p.ThoiGianCan as datetime) as [Giờ],
            DATEDIFF(
                SECOND,
                LAG(p.ThoiGianCan, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                p.ThoiGianCan
            ) as [GioPre],
            CAST(@ngay as datetime) + cast(
                ISNULL(
                    LAG(p.ThoiGianCan, 1) over(
                        order by
                            p.MaXuongSanXuat,
                            p.MaNhanVien,
                            p.ThoiGianCan
                    ),
                    '00:00:00'
                ) as datetime
            ) as [Giờ Trước],
            ISNULL(
                LAG(p.TrongLuong, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                0
            ) as [TL Trước],
            p.MaNhanVien as [Mã Nhân Viên],
            ISNULL(
                LAG(p.MaNhanVien, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                ''
            ) as [MNV Trước],
            n.MaHoSo as [Mã Hồ Sơ],
            p.MaNhanVienPhucVu,
            n.Name as [Tên Nhân Viên],
            isnull(p.LoTheoLine, p.MSL) as [Lô],
            la.Ten as [Loại Cá],
			isnull(p.MaThanhPhamBoTri,p.MaLoaiThanhPham) as MaThanhPham,
            isnull(p.ThanhPhamName, tp.Ten) as [Thành Phẩm],
            ISNULL(p.SizeName, s.Ten) as [Size],
            p.TrongLuong as [Trọng Lượng],
            p.MaMayTinhCan,
            p.MaXuongSanXuat as [Xưởng],
            p.TrongLuongTare
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
                                    p.Ngay <= @ngay
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
    left join NhanVienDaiThanh n on p.MaNhanVienPhucVu = n.MaNhanVien
Order By
    p.Ngay,
    p.[Xưởng],
    p.[Mã Nhân Viên],
    p.[Giờ]";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
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
        

        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                //CHẮT ĐÓNG LAI ĐỂ CHẠY TAHNGWF FORM TÍNH LƯƠNG FILLET, DO GỌI THÊM BANG MATHANHPHAMFILLET
                //                var query = @"Select
                //    p.Ngay ,
                //    P.[Giờ] as Gio,
                //	--FORMAT (P.[Giờ], 'hh:mm:ss') as Gio,
                //    case
                //        when p.[MNV Trước] <> p.[Mã Nhân Viên] then '00:00:00'
                //        else p.[Giờ Trước]
                //    end as GioTruoc,
                //	--FORMAT (case
                // --       when p.[MNV Trước] <> p.[Mã Nhân Viên] then '00:00:00'
                // --       else p.[Giờ Trước]
                // --   end, 'hh:mm:ss') as GioTruoc,
                //    dateadd(
                //        SECOND,
                //        case
                //            when ISNULL(p.GioPre, 0) <= 0
                //            and p.[MNV Trước] <> p.[Mã Nhân Viên] then 0
                //            else ISNULL(p.GioPre, 0)
                //        end,
                //        0
                //    ) as [TGPhieuLienTiep],

                //    p.[Mã Nhân Viên] as MaNhanVien,
                //    p.[Mã Hồ Sơ] as MaHoSo,
                //    p.[Tên Nhân Viên] as TenNhanVien,
                //    --n.MaNhanVien as [Mã Nhân Viên Phục Vụ],
                //    --n.MaHoSo as [Mã Hồ Sơ Phục vụ],
                //    --n.Name as [Tên Phục Vụ],
                //    p.[Lô] as Lo,
                //    p.[Loại Cá] as LoaiCa,
                //    p.[Thành Phẩm] as ThanhPham,
                //    p.Size,
                //    p.[Trọng Lượng] as TrongLuong,
                //    case
                //        when p.[MNV Trước] <> p.[Mã Nhân Viên] then 0
                //        else ISNULL(p.[TL Trước], 0)
                //    end as TLTruoc,
                //    p.[Xưởng] as Xuong
                //from
                //    (
                //        Select
                //            p.Ngay,
                //            CAST(@ngay as datetime) + cast(p.ThoiGianCan as datetime) as [Giờ],
                //            DATEDIFF(
                //                SECOND,
                //                LAG(p.ThoiGianCan, 1) over(
                //                    order by
                //                        p.MaXuongSanXuat,
                //                        p.MaNhanVien,
                //                        p.ThoiGianCan
                //                ),
                //                p.ThoiGianCan
                //            ) as [GioPre],
                //            CAST(@ngay as datetime) + cast(
                //                ISNULL(
                //                    LAG(p.ThoiGianCan, 1) over(
                //                        order by
                //                            p.MaXuongSanXuat,
                //                            p.MaNhanVien,
                //                            p.ThoiGianCan
                //                    ),
                //                    '00:00:00'
                //                ) as datetime
                //            ) as [Giờ Trước],
                //            ISNULL(
                //                LAG(p.TrongLuong, 1) over(
                //                    order by
                //                        p.MaXuongSanXuat,
                //                        p.MaNhanVien,
                //                        p.ThoiGianCan
                //                ),
                //                0
                //            ) as [TL Trước],
                //            p.MaNhanVien as [Mã Nhân Viên],
                //            ISNULL(
                //                LAG(p.MaNhanVien, 1) over(
                //                    order by
                //                        p.MaXuongSanXuat,
                //                        p.MaNhanVien,
                //                        p.ThoiGianCan
                //                ),
                //                ''
                //            ) as [MNV Trước],
                //            n.MaHoSo as [Mã Hồ Sơ],
                //            p.MaNhanVienPhucVu,
                //            n.Name as [Tên Nhân Viên],
                //            isnull(p.LoTheoLine, p.MSL) as [Lô],
                //            la.Ten as [Loại Cá],
                //            isnull(p.ThanhPhamName, tp.Ten) as [Thành Phẩm],
                //            ISNULL(p.SizeName, s.Ten) as [Size],
                //            p.TrongLuong as [Trọng Lượng],
                //            p.MaMayTinhCan,
                //            p.MaXuongSanXuat as [Xưởng]
                //        from
                //            (
                //                Select
                //                    p.*,
                //                    case
                //                        when tp2.IsNotSetByTime = 1 then s2.Ma
                //                        else bt.MaSize
                //                    end as MaSizeBoTri,
                //                    case
                //                        when tp2.IsNotSetByTime = 1 then s2.Ten
                //                        else s.Ten
                //                    end as SizeName,
                //                    case
                //                        when tp2.IsNotSetByTime = 1 then NULL
                //                        else bt.MaThanhPham
                //                    end as MaThanhPhamBoTri,
                //                    case
                //                        when tp2.IsNotSetByTime = 1 then NULL
                //                        else tp.Ten
                //                    end as ThanhPhamName
                //                from
                //                    (
                //                        select
                //                            p.*,
                //                            ll.MaLo as LoTheoLine
                //                        from
                //                            (
                //                                Select
                //                                    p.*,
                //                                    nl.MaLine,
                //                                    ln.Ten as LineName,
                //                                    nl.MaViTri,
                //                                    vt.Ten as ViTriName,
                //                                    xn.CodeId
                //                                from
                //                                    PhieuCanTPFillet p
                //                                    left join NhanVienTheoLine nl on p.Ngay = nl.Ngay
                //                                    and p.MaNhanVien = nl.MaNhanVien
                //                                    and nl.Gio = (
                //                                        Select
                //                                            MAX(nlc.Gio)
                //                                        from
                //                                            NhanVienTheoLine nlc
                //                                        where
                //                                            nlc.Ngay = p.Ngay
                //                                            and nlc.MaNhanVien = p.MaNhanVien
                //                                            and nlc.Gio <= p.ThoiGianCan
                //                                    )
                //                                    left join XiNghiep xn on xn.Ma = p.MaXuongSanXuat
                //                                    left join LineFilletv2 ln on nl.MaLine = ln.Ma
                //                                    left join ViTriFillet vt on nl.MaViTri = vt.Ma
                //                                where
                //                                    p.Ngay <= @ngay
                //                                    and p.Ngay >= @fromDate
                //                                    and p.MaXuongSanXuat = @xuongId
                //                            ) p
                //                            left JOIN LoTheoLine ll ON p.MaLine = ll.MaLine
                //                            and p.Ngay = ll.Ngay
                //                            and p.CodeId = ll.CodeId
                //                            and ll.Gio = (
                //                                Select
                //                                    MAX(llc.Gio)
                //                                from
                //                                    LoTheoLine llc
                //                                where
                //                                    llc.MaLine = p.MaLine
                //                                    and llc.CodeId = p.CodeId
                //                                    and llc.Ngay = p.Ngay
                //                                    and llc.Gio <= p.ThoiGianCan
                //                            )
                //                    ) p
                //                    left JOIN BoTriLoSizeThanhPham bt ON p.LoTheoLine = bt.MaLo
                //                    and p.CodeId = bt.CodeId
                //                    and p.MaViTri = bt.MaViTri
                //                    and bt.Ngay = p.Ngay
                //                    and bt.Gio = (
                //                        Select
                //                            MAX(btc.Gio)
                //                        from
                //                            BoTriLoSizeThanhPham btc
                //                        where
                //                            btc.MaLo = p.LoTheoLine
                //                            and btc.MaViTri = p.MaViTri
                //                            and btc.CodeId = p.CodeId
                //                            and btc.Ngay = p.Ngay
                //                            and btc.Gio <= p.ThoiGianCan
                //                    )
                //                    left join MaThanhPhamFillet tp on bt.MaThanhPham = tp.Ma
                //                    left join MaThanhPhamFillet tp2 on p.MaLoaiThanhPham = tp2.Ma
                //                    LEFT join MaSizeFillet s on bt.MaSize = s.Ma
                //                    LEFT join MaSizeFillet s2 on bt.MaSizePhu = s2.Ma
                //            ) p
                //            left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
                //            left join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
                //            left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma
                //            LEFT join MaSizeFillet s on p.MaSize = s.Ma
                //    ) p
                //    left join NhanVienDaiThanh n on p.MaNhanVienPhucVu = n.MaNhanVien
                //Order By
                //    p.Ngay,
                //    p.[Xưởng],
                //    p.[Mã Nhân Viên],
                //    p.[Giờ]";
                var query = @"
Select
    p.Ngay,
    P.[Giờ] as Gio,
    --FORMAT (P.[Giờ], 'hh:mm:ss') as Gio,
    case
        when p.[MNV Trước] <> p.[Mã Nhân Viên] then '00:00:00'
        else p.[Giờ Trước]
    end as GioTruoc,
    --FORMAT (case
    --       when p.[MNV Trước] <> p.[Mã Nhân Viên] then '00:00:00'
    --       else p.[Giờ Trước]
    --   end, 'hh:mm:ss') as GioTruoc,
    dateadd(
        SECOND,
        case
            when ISNULL(p.GioPre, 0) <= 0
            and p.[MNV Trước] <> p.[Mã Nhân Viên] then 0
            else ISNULL(p.GioPre, 0)
        end,
        0
    ) as [TGPhieuLienTiep],
    p.[Mã Nhân Viên] as MaNhanVien,
    p.[Mã Hồ Sơ] as MaHoSo,
    p.[Tên Nhân Viên] as TenNhanVien,
    --n.MaNhanVien as [Mã Nhân Viên Phục Vụ],
    --n.MaHoSo as [Mã Hồ Sơ Phục vụ],
    --n.Name as [Tên Phục Vụ],
    p.[Lô] as Lo,
    p.[Loại Cá] as LoaiCa,
    p.[Thành Phẩm] as ThanhPham,
    p.Size,
    p.[Trọng Lượng] as TrongLuong,
    case
        when p.[MNV Trước] <> p.[Mã Nhân Viên] then 0
        else ISNULL(p.[TL Trước], 0)
    end as TLTruoc,
    mtpf.MaBravoFillet as MaThanhPham,
    p.Ten as XuongName,
    p.[Xưởng] as Xuong,
    p.MaMayTinhCan,
    mtpf.MaBravoFillet,
    p.Nhom,
    p.MaUserCan
from
    (
        Select
            p.Ngay,
            CAST(@ngay as datetime) + cast(p.ThoiGianCan as datetime) as [Giờ],
            DATEDIFF(
                SECOND,
                LAG(p.ThoiGianCan, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                p.ThoiGianCan
            ) as [GioPre],
            CAST(@ngay as datetime) + cast(
                ISNULL(
                    LAG(p.ThoiGianCan, 1) over(
                        order by
                            p.MaXuongSanXuat,
                            p.MaNhanVien,
                            p.ThoiGianCan
                    ),
                    '00:00:00'
                ) as datetime
            ) as [Giờ Trước],
            ISNULL(
                LAG(p.TrongLuong, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                0
            ) as [TL Trước],
            p.MaNhanVien as [Mã Nhân Viên],
            ISNULL(
                LAG(p.MaNhanVien, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                ''
            ) as [MNV Trước],
            n.MaHoSo as [Mã Hồ Sơ],
            p.MaNhanVienPhucVu,
            n.Name as [Tên Nhân Viên],
            isnull(p.LoTheoLine, p.MSL) as [Lô],
            la.Ten as [Loại Cá],
            isnull(p.ThanhPhamName, tp.Ten) as [Thành Phẩm],
            ISNULL(p.SizeName, s.Ten) as [Size],
            p.TrongLuong as [Trọng Lượng],
            p.MaMayTinhCan,
            p.MaXuongSanXuat as [Xưởng],
            p.Ten,
            n.DeptName0 as Nhom,
            p.MaUserCan,
            p.MaLoaiCa,
            isnull(p.MaSizeBoTri, p.MaSize) as MaSize,
            isnull(p.MaThanhPhamBoTri, p.MaLoaiThanhPham) as MaLoaiThanhPham
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
                                    xn.CodeId,
                                    xn.Ten -- mtpf.MaBravoFillet
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
                                    left join MaThanhPhamFillet tpf on p.MaLoaiThanhPham = tpf.Ma and p.MaLoaiCa=tpf.MaCa
                                where
                                    p.Ngay <= @ngay
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
    left join NhanVienDaiThanh n on p.MaNhanVienPhucVu = n.MaNhanVien
    left join MapThanhPhamFillet mtpf on p.MaLoaiCa = mtpf.MaLoaiCaFillet
    and p.MaLoaiThanhPham = mtpf.MaTPFillet 
    and p.MaSize = mtpf.MaSize
Order By
    p.Ngay,
    p.[Xưởng],
    p.[Mã Nhân Viên],
    p.[Giờ]";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { ngay = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetChiTietByMaNhanViens<T>(DateTime fromDate, DateTime dateTime, string maNhanVien, string xuongId)
        {
            try
            {
                var query = @"
Select
    p.Ngay,
    P.[Giờ] as Gio,
    --FORMAT (P.[Giờ], 'hh:mm:ss') as Gio,
    case
        when p.[MNV Trước] <> p.[Mã Nhân Viên] then '00:00:00'
        else p.[Giờ Trước]
    end as GioTruoc,
    --FORMAT (case
    --       when p.[MNV Trước] <> p.[Mã Nhân Viên] then '00:00:00'
    --       else p.[Giờ Trước]
    --   end, 'hh:mm:ss') as GioTruoc,
    dateadd(
        SECOND,
        case
            when ISNULL(p.GioPre, 0) <= 0
            and p.[MNV Trước] <> p.[Mã Nhân Viên] then 0
            else ISNULL(p.GioPre, 0)
        end,
        0
    ) as [TGPhieuLienTiep],
    p.[Mã Nhân Viên] as MaNhanVien,
    p.[Mã Hồ Sơ] as MaHoSo,
    p.[Tên Nhân Viên] as TenNhanVien,
    --n.MaNhanVien as [Mã Nhân Viên Phục Vụ],
    --n.MaHoSo as [Mã Hồ Sơ Phục vụ],
    --n.Name as [Tên Phục Vụ],
    p.[Lô] as Lo,
    p.[Loại Cá] as LoaiCa,
    p.[Thành Phẩm] as ThanhPham,
    p.Size,
    p.[Trọng Lượng] as TrongLuong,
    case
        when p.[MNV Trước] <> p.[Mã Nhân Viên] then 0
        else ISNULL(p.[TL Trước], 0)
    end as TLTruoc,
    mtpf.MaBravoFillet as MaThanhPham,
    p.Ten as XuongName,
    p.[Xưởng] as Xuong,
    p.MaMayTinhCan,
    mtpf.MaBravoFillet,
    p.Nhom,
    p.MaUserCan
from
    (
        Select
            p.Ngay,
            CAST(@ngay as datetime) + cast(p.ThoiGianCan as datetime) as [Giờ],
            DATEDIFF(
                SECOND,
                LAG(p.ThoiGianCan, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                p.ThoiGianCan
            ) as [GioPre],
            CAST(@ngay as datetime) + cast(
                ISNULL(
                    LAG(p.ThoiGianCan, 1) over(
                        order by
                            p.MaXuongSanXuat,
                            p.MaNhanVien,
                            p.ThoiGianCan
                    ),
                    '00:00:00'
                ) as datetime
            ) as [Giờ Trước],
            ISNULL(
                LAG(p.TrongLuong, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                0
            ) as [TL Trước],
            p.MaNhanVien as [Mã Nhân Viên],
            ISNULL(
                LAG(p.MaNhanVien, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                ''
            ) as [MNV Trước],
            n.MaHoSo as [Mã Hồ Sơ],
            p.MaNhanVienPhucVu,
            n.Name as [Tên Nhân Viên],
            isnull(p.LoTheoLine, p.MSL) as [Lô],
            la.Ten as [Loại Cá],
            isnull(p.ThanhPhamName, tp.Ten) as [Thành Phẩm],
            ISNULL(p.SizeName, s.Ten) as [Size],
            p.TrongLuong as [Trọng Lượng],
            p.MaMayTinhCan,
            p.MaXuongSanXuat as [Xưởng],
            p.Ten,
            n.DeptName0 as Nhom,
            p.MaUserCan,
            p.MaLoaiCa,
            isnull(p.MaSizeBoTri, p.MaSize) as MaSize,
            isnull(p.MaThanhPhamBoTri, p.MaLoaiThanhPham) as MaLoaiThanhPham
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
                                    xn.CodeId,
                                    xn.Ten -- mtpf.MaBravoFillet
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
                                    left join MaThanhPhamFillet tpf on p.MaLoaiThanhPham = tpf.Ma and p.MaLoaiCa=tpf.MaCa
                                where
                                    p.Ngay <= @ngay
                                    and p.Ngay >= @fromDate
                                    and p.MaXuongSanXuat = @xuongId
                                    and p.MaNhanVien = @maNhanVien
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
    left join NhanVienDaiThanh n on p.MaNhanVienPhucVu = n.MaNhanVien
    left join MapThanhPhamFillet mtpf on p.MaLoaiCa = mtpf.MaLoaiCaFillet
    and p.MaLoaiThanhPham = mtpf.MaTPFillet 
    and p.MaSize = mtpf.MaSize
Order By
    p.Ngay,
    p.[Xưởng],
    p.[Mã Nhân Viên],
    p.[Giờ]";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { ngay = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId ,maNhanVien=maNhanVien}).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTietByMaHoSos<T>(DateTime fromDate, DateTime dateTime, string maHoSo, string xuongId)
        {
            try
            {
                var query = @"
Select
    p.Ngay,
    P.[Giờ] as Gio,
    --FORMAT (P.[Giờ], 'hh:mm:ss') as Gio,
    case
        when p.[MNV Trước] <> p.[Mã Nhân Viên] then '00:00:00'
        else p.[Giờ Trước]
    end as GioTruoc,
    --FORMAT (case
    --       when p.[MNV Trước] <> p.[Mã Nhân Viên] then '00:00:00'
    --       else p.[Giờ Trước]
    --   end, 'hh:mm:ss') as GioTruoc,
    dateadd(
        SECOND,
        case
            when ISNULL(p.GioPre, 0) <= 0
            and p.[MNV Trước] <> p.[Mã Nhân Viên] then 0
            else ISNULL(p.GioPre, 0)
        end,
        0
    ) as [TGPhieuLienTiep],
    p.[Mã Nhân Viên] as MaNhanVien,
    p.[Mã Hồ Sơ] as MaHoSo,
    p.[Tên Nhân Viên] as TenNhanVien,
    --n.MaNhanVien as [Mã Nhân Viên Phục Vụ],
    --n.MaHoSo as [Mã Hồ Sơ Phục vụ],
    --n.Name as [Tên Phục Vụ],
    p.[Lô] as Lo,
    p.[Loại Cá] as LoaiCa,
    p.[Thành Phẩm] as ThanhPham,
    p.Size,
    p.[Trọng Lượng] as TrongLuong,
    case
        when p.[MNV Trước] <> p.[Mã Nhân Viên] then 0
        else ISNULL(p.[TL Trước], 0)
    end as TLTruoc,
    mtpf.MaBravoFillet as MaThanhPham,
    p.Ten as XuongName,
    p.[Xưởng] as Xuong,
    p.MaMayTinhCan,
    mtpf.MaBravoFillet,
    p.Nhom,
    p.MaUserCan
from
    (
        Select
            p.Ngay,
            CAST(@toDate as datetime) + cast(p.ThoiGianCan as datetime) as [Giờ],
            DATEDIFF(
                SECOND,
                LAG(p.ThoiGianCan, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                p.ThoiGianCan
            ) as [GioPre],
            CAST(@toDate as datetime) + cast(
                ISNULL(
                    LAG(p.ThoiGianCan, 1) over(
                        order by
                            p.MaXuongSanXuat,
                            p.MaNhanVien,
                            p.ThoiGianCan
                    ),
                    '00:00:00'
                ) as datetime
            ) as [Giờ Trước],
            ISNULL(
                LAG(p.TrongLuong, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                0
            ) as [TL Trước],
            p.MaNhanVien as [Mã Nhân Viên],
            ISNULL(
                LAG(p.MaNhanVien, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                ''
            ) as [MNV Trước],
            n.MaHoSo as [Mã Hồ Sơ],
            p.MaNhanVienPhucVu,
            n.Name as [Tên Nhân Viên],
            isnull(p.LoTheoLine, p.MSL) as [Lô],
            la.Ten as [Loại Cá],
            isnull(p.ThanhPhamName, tp.Ten) as [Thành Phẩm],
            ISNULL(p.SizeName, s.Ten) as [Size],
            p.TrongLuong as [Trọng Lượng],
            p.MaMayTinhCan,
            p.MaXuongSanXuat as [Xưởng],
            p.Ten,
            n.DeptName0 as Nhom,
            p.MaUserCan,
            p.MaLoaiCa,
            isnull(p.MaSizeBoTri, p.MaSize) as MaSize,
            isnull(p.MaThanhPhamBoTri, p.MaLoaiThanhPham) as MaLoaiThanhPham
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
                                    xn.CodeId,
                                    xn.Ten -- mtpf.MaBravoFillet
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
                                    left join MaThanhPhamFillet tpf on p.MaLoaiThanhPham = tpf.Ma and p.MaLoaiCa=tpf.MaCa
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
    left join NhanVienDaiThanh n on p.MaNhanVienPhucVu = n.MaNhanVien
    left join MapThanhPhamFillet mtpf on p.MaLoaiCa = mtpf.MaLoaiCaFillet
    and p.MaLoaiThanhPham = mtpf.MaTPFillet 
    and p.MaSize = mtpf.MaSize
	where
	p.[Mã Nhân Viên] = n.MaNhanVien
	and n.MaHoSo = @maHoSo

Order By
    p.Ngay,
    p.[Xưởng],
    p.[Mã Nhân Viên],
    p.[Giờ]";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { ngay = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId ,maHoSo=maHoSo}).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetChiTietByMaThes<T>(DateTime fromDate, DateTime dateTime, string maThe, string xuongId)
        {
            try
            {
                var query = @"
Select
    p.Ngay,
    P.[Giờ] as Gio,
    --FORMAT (P.[Giờ], 'hh:mm:ss') as Gio,
    case
        when p.[MNV Trước] <> p.[Mã Nhân Viên] then '00:00:00'
        else p.[Giờ Trước]
    end as GioTruoc,
    --FORMAT (case
    --       when p.[MNV Trước] <> p.[Mã Nhân Viên] then '00:00:00'
    --       else p.[Giờ Trước]
    --   end, 'hh:mm:ss') as GioTruoc,
    dateadd(
        SECOND,
        case
            when ISNULL(p.GioPre, 0) <= 0
            and p.[MNV Trước] <> p.[Mã Nhân Viên] then 0
            else ISNULL(p.GioPre, 0)
        end,
        0
    ) as [TGPhieuLienTiep],
    p.[Mã Nhân Viên] as MaNhanVien,
    p.[Mã Hồ Sơ] as MaHoSo,
    p.[Tên Nhân Viên] as TenNhanVien,
    --n.MaNhanVien as [Mã Nhân Viên Phục Vụ],
    --n.MaHoSo as [Mã Hồ Sơ Phục vụ],
    --n.Name as [Tên Phục Vụ],
    p.[Lô] as Lo,
    p.[Loại Cá] as LoaiCa,
    p.[Thành Phẩm] as ThanhPham,
    p.Size,
    p.[Trọng Lượng] as TrongLuong,
    case
        when p.[MNV Trước] <> p.[Mã Nhân Viên] then 0
        else ISNULL(p.[TL Trước], 0)
    end as TLTruoc,
    mtpf.MaBravoFillet as MaThanhPham,
    p.Ten as XuongName,
    p.[Xưởng] as Xuong,
    p.MaMayTinhCan,
    mtpf.MaBravoFillet,
    p.Nhom,
    p.MaUserCan
from
    (
        Select
            p.Ngay,
            CAST(@toDate as datetime) + cast(p.ThoiGianCan as datetime) as [Giờ],
            DATEDIFF(
                SECOND,
                LAG(p.ThoiGianCan, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                p.ThoiGianCan
            ) as [GioPre],
            CAST(@toDate as datetime) + cast(
                ISNULL(
                    LAG(p.ThoiGianCan, 1) over(
                        order by
                            p.MaXuongSanXuat,
                            p.MaNhanVien,
                            p.ThoiGianCan
                    ),
                    '00:00:00'
                ) as datetime
            ) as [Giờ Trước],
            ISNULL(
                LAG(p.TrongLuong, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                0
            ) as [TL Trước],
            p.MaNhanVien as [Mã Nhân Viên],
            ISNULL(
                LAG(p.MaNhanVien, 1) over(
                    order by
                        p.MaXuongSanXuat,
                        p.MaNhanVien,
                        p.ThoiGianCan
                ),
                ''
            ) as [MNV Trước],
            n.MaHoSo as [Mã Hồ Sơ],
            p.MaNhanVienPhucVu,
            n.Name as [Tên Nhân Viên],
            isnull(p.LoTheoLine, p.MSL) as [Lô],
            la.Ten as [Loại Cá],
            isnull(p.ThanhPhamName, tp.Ten) as [Thành Phẩm],
            ISNULL(p.SizeName, s.Ten) as [Size],
            p.TrongLuong as [Trọng Lượng],
            p.MaMayTinhCan,
            p.MaXuongSanXuat as [Xưởng],
            p.Ten,
            n.DeptName0 as Nhom,
            p.MaUserCan,
            p.MaLoaiCa,
            isnull(p.MaSizeBoTri, p.MaSize) as MaSize,
            isnull(p.MaThanhPhamBoTri, p.MaLoaiThanhPham) as MaLoaiThanhPham
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
                                    xn.CodeId,
                                    xn.Ten -- mtpf.MaBravoFillet
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
                                    left join MaThanhPhamFillet tpf on p.MaLoaiThanhPham = tpf.Ma and p.MaLoaiCa=tpf.MaCa
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
    left join NhanVienDaiThanh n on p.MaNhanVienPhucVu = n.MaNhanVien
    left join MapThanhPhamFillet mtpf on p.MaLoaiCa = mtpf.MaLoaiCaFillet
	left join TheTu t on p.[Mã Nhân Viên] = t.MaNhanVien
    and p.MaLoaiThanhPham = mtpf.MaTPFillet 
    and p.MaSize = mtpf.MaSize
	where
	t.MaTheTu = @maThe
Order By
    p.Ngay,
    p.[Xưởng],
    p.[Mã Nhân Viên],
    p.[Giờ]";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { ngay = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId ,maThe=maThe}).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public T GetMaxThoiGianCan<T>(DateTime dateTime, string mayCanId)
        {
            try
            {
                var query =
                    @"Select Max(ThoiGianCan) from PhieuCanTPFillet where Ngay=@ngay and MaMayTinhCan = @mayCanId";
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
        public DateTime GetMaxDateTimeBefore(DateTime dateTime, string xuongId)
        {
            var query =
                $@"Select  isnull( Max(Ngay),Cast('2000-01-01' as datetime)) as Ngay from PhieuCanTPFillet  where Ngay=@ngay and MaXuongSanXuat = @xuongId";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.ExecuteScalar<DateTime>(query, new { ngay = dateTime.Date, xuongId });
            return item;
        }
        public List<string> GetMayCans(DateTime dateTime)
        {
            try
            {
                var query = @"Select DISTINCT MaMayTinhCan from PhieuCanTPFillet where Ngay = @ngay ";
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
        public List<T> Gets<T>(DateTime dateTime)
        {
            try
            {
                var query = @"Select
    p.*,
    ISNULL(tp.MaBravoFillet, 'NONE') as BravoId
from
    (
        Select
            *, (
                case
                    when p.ThoiGianCan <= '05:00:00' then 1
                    else 0
                end
            ) as IsTangCa
        from
            PhieuCanTPFillet p
        where
            p.Ngay =@ngay
    ) p
    left JOIN MapThanhPhamFillet tp on p.MaLoaiThanhPham = tp.MaTPFillet
    and p.MaSize = tp.MaSize and p.IsTangCa = tp.IsTangCa";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = "Select * from PhieuCanTPFillet where Ngay =@ngay and MaXuongSanXuat = @xuongId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime, string mayCanId, TimeSpan timeSpan)
        {
            try
            {
                var query =
                    "Select * from PhieuCanTPFillet where Ngay =@ngay and MaMayTinhCan = @mayCanId and ThoiGianCan >@timeSpan";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, mayCanId, timeSpan }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> Gets<T>(DateTime dateTime, string xuongId, string nhanVienId)
        {
            try
            {
                var query = @"SELECT
    p.MaNhanVien,
    p.HoVaTen as NhanVienName,
    p.MaLoaiCa,
    p.MSL as MaLo,
    p.MaLoaiThanhPham as MaThanhPham,
    p.MaSize,
    Count(p.TrongLuong) as SoRo,
    CAST(Sum(p.TrongLuong) as decimal(18, 2)) as TrongLuong
FROM
    PhieuCanTPFillet p
Where
    p.SuDung = 1
    and p.Ngay = @ngay
    and p.MaXuongSanXuat = @xuongId
    and p.MaNhanVien = @nhanVienId
Group By
    p.MaNhanVien,
    p.MSL,
    p.MaLoaiCa,
    p.MaSize,
    p.MaLoaiThanhPham,
    p.HoVaTen";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId, nhanVienId }).ToList();
                return items;
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
                var query = @"select
    isnull(p.LoTheoLine, p.MSL) as [Lô],
    la.Ten as [Loại Cá],
    isnull(p.ThanhPhamName, tp.Ten) as [Thành Phẩm],
    isnull(p.SizeName, s.Ten) as [Size],
    mau.Ten as [Màu],
    CAST(Sum(p.TrongLuong) as decimal(18, 2)) as [Trọng Lượng],
    Count(p.TrongLuong) as [Số Rổ],
    cast (Avg(p.Trongluong) as decimal(18, 2)) as [Trung Bình],
    p.MaXuongSanXuat as [Xuong]
From
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
            case when tp2.IsNotSetByTime =1 then NULL else bt.MaThanhPham end as MaThanhPhamBoTri,
            case when tp2.IsNotSetByTime =1 then NULL else  tp.Ten end  as ThanhPhamName
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
                            and nl.Ngay = @ngay
                            and p.MaNhanVien = nl.MaNhanVien
                            and nl.Gio = (
                                Select
                                    MAX(nlc.Gio)
                                from
                                    NhanVienTheoLine nlc
                                where
                                    nlc.Ngay = @ngay
                                    and nlc.MaNhanVien = p.MaNhanVien
                                    and nlc.Gio <= p.ThoiGianCan
                            )
                            left join XiNghiep xn on xn.Ma = p.MaXuongSanXuat
                            left join LineFilletv2 ln on nl.MaLine = ln.Ma
                            left join ViTriFillet vt on nl.MaViTri = vt.Ma
                        where
                            p.Ngay = @ngay
                            and p.MaXuongSanXuat = @xuongId
                    ) p
                    left JOIN LoTheoLine ll ON p.MaLine = ll.MaLine
                    and p.Ngay = ll.Ngay
                    and p.CodeId = ll.CodeId
                    and ll.Ngay = @ngay
                    and ll.Gio = (
                        Select
                            MAX(llc.Gio)
                        from
                            LoTheoLine llc
                        where
                            llc.MaLine = p.MaLine
                            and llc.CodeId = p.CodeId
                            and llc.Ngay = @ngay
                            and llc.Gio <= p.ThoiGianCan
                    )
            ) p
            left JOIN BoTriLoSizeThanhPham bt ON p.LoTheoLine = bt.MaLo
            and p.CodeId = bt.CodeId
            and p.MaViTri = bt.MaViTri
            and bt.Ngay = @ngay
            and bt.Gio = (
                Select
                    MAX(btc.Gio)
                from
                    BoTriLoSizeThanhPham btc
                where
                    btc.MaLo = p.LoTheoLine
                    and btc.MaViTri = p.MaViTri
                    and btc.CodeId = p.CodeId
                    and btc.Ngay = @ngay
                    and btc.Gio <= p.ThoiGianCan
            )
            left join MaThanhPhamFillet tp on bt.MaThanhPham = tp.Ma and tp.MaCa = p.MaLoaiCa
            left join MaThanhPhamFillet tp2 on p.MaLoaiThanhPham = tp2.Ma and tp2.MaCa = p.MaLoaiCa
            LEFT join MaSizeFillet s on bt.MaSize = s.Ma
LEFT join MaSizeFillet s2 on bt.MaSizePhu = s2.Ma
    ) p,
    MaMauFillet mau,
    MaSizeFillet s,
    MaThanhPhamFillet tp,
    MaLoaiCaFillet la
Where
    p.Ngay = @ngay
    and p.SuDung = 1
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
Group By
    p.LoTheoLine,
    p.ThanhPhamName,
    p.SizeName,
    p.MSL,
    la.Ten,
    tp.Ten,
    s.Ten,
    mau.Ten,
    p.MaXuongSanXuat";
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
        public List<T> GetTongHopThanhPhams<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"select
    isnull(p.LoTheoLine, p.MSL) as MaLo,
    la.Ten as LoaiCaName,
    isnull(p.ThanhPhamName, tp.Ten) as ThanhPhamName,
    isnull(p.SizeName, s.Ten) as SizeName,
    mau.Ten as MauName,
    CAST(Sum(p.TrongLuong) as decimal(18, 2)) as TrongLuong,
    Count(p.TrongLuong) as SoRo,
    cast (Avg(p.Trongluong) as decimal(18, 2)) as TrungBinh,
    p.MaXuongSanXuat as MaXuong
From
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
            case when tp2.IsNotSetByTime =1 then NULL else bt.MaThanhPham end as MaThanhPhamBoTri,
            case when tp2.IsNotSetByTime =1 then NULL else  tp.Ten end  as ThanhPhamName
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
                            and nl.Ngay <= @toDate
							and nl.Ngay >= @fromDate
                            and p.MaNhanVien = nl.MaNhanVien
                            and nl.Gio = (
                                Select
                                    MAX(nlc.Gio)
                                from
                                    NhanVienTheoLine nlc
                                where
                                    nlc.Ngay <= @toDate
									and nlc.Ngay >= @fromDate
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
                    and ll.Ngay <= @toDate
					and ll.Ngay >= @fromDate
                    and ll.Gio = (
                        Select
                            MAX(llc.Gio)
                        from
                            LoTheoLine llc
                        where
                            llc.MaLine = p.MaLine
                            and llc.CodeId = p.CodeId
                            and llc.Ngay <= @toDate
							and llc.Ngay >= @fromDate
                            and llc.Gio <= p.ThoiGianCan
                    )
            ) p
            left JOIN BoTriLoSizeThanhPham bt ON p.LoTheoLine = bt.MaLo
            and p.CodeId = bt.CodeId
            and p.MaViTri = bt.MaViTri
            and bt.Ngay <= @toDate
			and bt.Ngay >= @fromDate
            and bt.Gio = (
                Select
                    MAX(btc.Gio)
                from
                    BoTriLoSizeThanhPham btc
                where
                    btc.MaLo = p.LoTheoLine
                    and btc.MaViTri = p.MaViTri
                    and btc.CodeId = p.CodeId
                    and btc.Ngay <= @toDate
					and btc.Ngay >= @fromDate
                    and btc.Gio <= p.ThoiGianCan
            )
            left join MaThanhPhamFillet tp on bt.MaThanhPham = tp.Ma and tp.MaCa = p.MaLoaiCa
            left join MaThanhPhamFillet tp2 on p.MaLoaiThanhPham = tp2.Ma and tp2.MaCa = p.MaLoaiCa
            LEFT join MaSizeFillet s on bt.MaSize = s.Ma
LEFT join MaSizeFillet s2 on bt.MaSizePhu = s2.Ma
    ) p,
    MaMauFillet mau,
    MaSizeFillet s,
    MaThanhPhamFillet tp,
    MaLoaiCaFillet la
Where
    p.Ngay <= @toDate
	and p.Ngay >= @fromDate 
    and p.SuDung = 1
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
Group By
    p.LoTheoLine,
    p.ThanhPhamName,
    p.SizeName,
    p.MSL,
    la.Ten,
    tp.Ten,
    s.Ten,
    mau.Ten,
    p.MaXuongSanXuat
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { ngay = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaNhanViens<T>(DateTime fromDate, DateTime dateTime, string maNhanVien, string xuongId)
        {
            try
            {
                var query = @"select
	p.MaNhanVien,
	nv.MaHoSo,
	nv.Name as NhanVienName,
    isnull(p.LoTheoLine, p.MSL) as MaLo,
    la.Ten as LoaiCaName,
    isnull(p.ThanhPhamName, tp.Ten) as ThanhPhamName,
    isnull(p.SizeName, s.Ten) as SizeName,
    mau.Ten as MauName,
    CAST(Sum(p.TrongLuong) as decimal(18, 2)) as TrongLuong,
    Count(p.TrongLuong) as SoRo,
    cast (Avg(p.Trongluong) as decimal(18, 2)) as TrungBinh
From
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
            case when tp2.IsNotSetByTime =1 then NULL else bt.MaThanhPham end as MaThanhPhamBoTri,
            case when tp2.IsNotSetByTime =1 then NULL else  tp.Ten end  as ThanhPhamName
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
                            and nl.Ngay <= @toDate
							and nl.Ngay >= @fromDate
                            and p.MaNhanVien = nl.MaNhanVien
                            and nl.Gio = (
                                Select
                                    MAX(nlc.Gio)
                                from
                                    NhanVienTheoLine nlc
                                where
                                    nlc.Ngay <= @toDate
									and nlc.Ngay >= @fromDate
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
                    and ll.Ngay <= @toDate
					and ll.Ngay >= @fromDate
                    and ll.Gio = (
                        Select
                            MAX(llc.Gio)
                        from
                            LoTheoLine llc
                        where
                            llc.MaLine = p.MaLine
                            and llc.CodeId = p.CodeId
                            and llc.Ngay <= @toDate
							and llc.Ngay >= @fromDate
                            and llc.Gio <= p.ThoiGianCan
                    )
            ) p
            left JOIN BoTriLoSizeThanhPham bt ON p.LoTheoLine = bt.MaLo
            and p.CodeId = bt.CodeId
            and p.MaViTri = bt.MaViTri
            and bt.Ngay <= @toDate
			and bt.Ngay >= @fromDate
            and bt.Gio = (
                Select
                    MAX(btc.Gio)
                from
                    BoTriLoSizeThanhPham btc
                where
                    btc.MaLo = p.LoTheoLine
                    and btc.MaViTri = p.MaViTri
                    and btc.CodeId = p.CodeId
                    and btc.Ngay <= @toDate
					and btc.Ngay >= @fromDate
                    and btc.Gio <= p.ThoiGianCan
            )
            left join MaThanhPhamFillet tp on bt.MaThanhPham = tp.Ma and tp.MaCa = p.MaLoaiCa
            left join MaThanhPhamFillet tp2 on p.MaLoaiThanhPham = tp2.Ma and tp2.MaCa = p.MaLoaiCa
            LEFT join MaSizeFillet s on bt.MaSize = s.Ma
LEFT join MaSizeFillet s2 on bt.MaSizePhu = s2.Ma
    ) p,
    MaMauFillet mau,
    MaSizeFillet s,
    MaThanhPhamFillet tp,
    MaLoaiCaFillet la,
	NhanVienDaiThanh nv
Where
    p.Ngay <= @toDate
	and p.Ngay >= @fromDate 
    and p.MaXuongSanXuat = @xuongId
    and p.SuDung = 1
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
	and p.MaNhanVien = nv.MaNhanVien
	and p.MaNhanVien = @maNhanVien
Group By
    p.LoTheoLine,
    p.ThanhPhamName,
    p.SizeName,
    p.MSL,
    la.Ten,
    tp.Ten,
    s.Ten,
    mau.Ten,
	p.MaNhanVien,
	nv.MaHoSo,
	nv.Name,
    p.MaXuongSanXuat
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { ngay = dateTime.Date, fromDate = fromDate.Date, maNhanVien = maNhanVien, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaThes<T>(DateTime fromDate, DateTime dateTime, string maThe, string xuongId)
        {
            try
            {
                var query = @"select
	p.MaNhanVien,
	nv.MaHoSo,
	nv.Name as NhanVienName,
    isnull(p.LoTheoLine, p.MSL) as MaLo,
    la.Ten as LoaiCaName,
    isnull(p.ThanhPhamName, tp.Ten) as ThanhPhamName,
    isnull(p.SizeName, s.Ten) as SizeName,
    mau.Ten as MauName,
    CAST(Sum(p.TrongLuong) as decimal(18, 2)) as TrongLuong,
    Count(p.TrongLuong) as SoRo,
    cast (Avg(p.Trongluong) as decimal(18, 2)) as TrungBinh
From
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
            case when tp2.IsNotSetByTime =1 then NULL else bt.MaThanhPham end as MaThanhPhamBoTri,
            case when tp2.IsNotSetByTime =1 then NULL else  tp.Ten end  as ThanhPhamName
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
                            and nl.Ngay <= @toDate
							and nl.Ngay >= @fromDate
                            and p.MaNhanVien = nl.MaNhanVien
                            and nl.Gio = (
                                Select
                                    MAX(nlc.Gio)
                                from
                                    NhanVienTheoLine nlc
                                where
                                    nlc.Ngay <= @toDate
									and nlc.Ngay >= @fromDate
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
                    and ll.Ngay <= @toDate
					and ll.Ngay >= @fromDate
                    and ll.Gio = (
                        Select
                            MAX(llc.Gio)
                        from
                            LoTheoLine llc
                        where
                            llc.MaLine = p.MaLine
                            and llc.CodeId = p.CodeId
                            and llc.Ngay <= @toDate
							and llc.Ngay >= @fromDate
                            and llc.Gio <= p.ThoiGianCan
                    )
            ) p
            left JOIN BoTriLoSizeThanhPham bt ON p.LoTheoLine = bt.MaLo
            and p.CodeId = bt.CodeId
            and p.MaViTri = bt.MaViTri
            and bt.Ngay <= @toDate
			and bt.Ngay >= @fromDate
            and bt.Gio = (
                Select
                    MAX(btc.Gio)
                from
                    BoTriLoSizeThanhPham btc
                where
                    btc.MaLo = p.LoTheoLine
                    and btc.MaViTri = p.MaViTri
                    and btc.CodeId = p.CodeId
                    and btc.Ngay <= @toDate
					and btc.Ngay >= @fromDate
                    and btc.Gio <= p.ThoiGianCan
            )
            left join MaThanhPhamFillet tp on bt.MaThanhPham = tp.Ma and tp.MaCa = p.MaLoaiCa
            left join MaThanhPhamFillet tp2 on p.MaLoaiThanhPham = tp2.Ma and tp2.MaCa = p.MaLoaiCa
            LEFT join MaSizeFillet s on bt.MaSize = s.Ma
LEFT join MaSizeFillet s2 on bt.MaSizePhu = s2.Ma
    ) p,
    MaMauFillet mau,
    MaSizeFillet s,
    MaThanhPhamFillet tp,
    MaLoaiCaFillet la,
	NhanVienDaiThanh nv,
    TheTu t
Where
    p.Ngay <= @toDate
	and p.Ngay >= @fromDate 
    and p.MaXuongSanXuat = @xuongId
    and p.SuDung = 1
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
	and p.MaNhanVien = nv.MaNhanVien
	and p.MaNhanVien = t.MaNhanVien
	and t.MaTheTu = @maThe
Group By
    p.LoTheoLine,
    p.ThanhPhamName,
    p.SizeName,
    p.MSL,
    la.Ten,
    tp.Ten,
    s.Ten,
    mau.Ten,
	p.MaNhanVien,
	nv.MaHoSo,
	nv.Name,
    p.MaXuongSanXuat
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { ngay = dateTime.Date, fromDate = fromDate.Date, maThe = maThe, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamByMaHoSos<T>(DateTime fromDate, DateTime dateTime, string maHoSo, string xuongId)
        {
            try
            {
                var query = @"select
	p.MaNhanVien,
	nv.MaHoSo,
	nv.Name as NhanVienName,
    isnull(p.LoTheoLine, p.MSL) as MaLo,
    la.Ten as LoaiCaName,
    isnull(p.ThanhPhamName, tp.Ten) as ThanhPhamName,
    isnull(p.SizeName, s.Ten) as SizeName,
    mau.Ten as MauName,
    CAST(Sum(p.TrongLuong) as decimal(18, 2)) as TrongLuong,
    Count(p.TrongLuong) as SoRo,
    cast (Avg(p.Trongluong) as decimal(18, 2)) as TrungBinh
From
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
            case when tp2.IsNotSetByTime =1 then NULL else bt.MaThanhPham end as MaThanhPhamBoTri,
            case when tp2.IsNotSetByTime =1 then NULL else  tp.Ten end  as ThanhPhamName
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
                            and nl.Ngay <= @toDate
							and nl.Ngay >= @fromDate
                            and p.MaNhanVien = nl.MaNhanVien
                            and nl.Gio = (
                                Select
                                    MAX(nlc.Gio)
                                from
                                    NhanVienTheoLine nlc
                                where
                                    nlc.Ngay <= @toDate
									and nlc.Ngay >= @fromDate
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
                    and ll.Ngay <= @toDate
					and ll.Ngay >= @fromDate
                    and ll.Gio = (
                        Select
                            MAX(llc.Gio)
                        from
                            LoTheoLine llc
                        where
                            llc.MaLine = p.MaLine
                            and llc.CodeId = p.CodeId
                            and llc.Ngay <= @toDate
							and llc.Ngay >= @fromDate
                            and llc.Gio <= p.ThoiGianCan
                    )
            ) p
            left JOIN BoTriLoSizeThanhPham bt ON p.LoTheoLine = bt.MaLo
            and p.CodeId = bt.CodeId
            and p.MaViTri = bt.MaViTri
            and bt.Ngay <= @toDate
			and bt.Ngay >= @fromDate
            and bt.Gio = (
                Select
                    MAX(btc.Gio)
                from
                    BoTriLoSizeThanhPham btc
                where
                    btc.MaLo = p.LoTheoLine
                    and btc.MaViTri = p.MaViTri
                    and btc.CodeId = p.CodeId
                    and btc.Ngay <= @toDate
					and btc.Ngay >= @fromDate
                    and btc.Gio <= p.ThoiGianCan
            )
            left join MaThanhPhamFillet tp on bt.MaThanhPham = tp.Ma and tp.MaCa = p.MaLoaiCa
            left join MaThanhPhamFillet tp2 on p.MaLoaiThanhPham = tp2.Ma and tp2.MaCa = p.MaLoaiCa
            LEFT join MaSizeFillet s on bt.MaSize = s.Ma
LEFT join MaSizeFillet s2 on bt.MaSizePhu = s2.Ma
    ) p,
    MaMauFillet mau,
    MaSizeFillet s,
    MaThanhPhamFillet tp,
    MaLoaiCaFillet la,
	NhanVienDaiThanh nv
Where
    p.Ngay <= @toDate
	and p.Ngay >= @fromDate 
    and p.MaXuongSanXuat = @xuongId
    and p.SuDung = 1
    and p.MaLoaiCa = la.Ma
    and p.MaLoaiThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaMau = mau.Ma
	and p.MaNhanVien = nv.MaNhanVien
	and nv.MaHoSo = @maHoSo
Group By
    p.LoTheoLine,
    p.ThanhPhamName,
    p.SizeName,
    p.MSL,
    la.Ten,
    tp.Ten,
    s.Ten,
    mau.Ten,
	p.MaNhanVien,
	nv.MaHoSo,
	nv.Name,
    p.MaXuongSanXuat
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { ngay = dateTime.Date, fromDate = fromDate.Date, maHoSo = maHoSo, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetTongHopThanhPhams(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                //                var query =
                //                    @"select p.Lô,p.[Loại Cá],p.Màu,p.Size,Sum( p.[Số Rổ]) as [Số Rổ],p.[Thành Phẩm],Sum(p.[Trọng Lượng]) as [Trọng Lượng],Avg(p.[Trung Bình]) as [Trung Bình],p.Xuong from (
                //select
                //    isnull(p.LoTheoLine, p.MSL) as [Lô],
                //    la.Ten as [Loại Cá],
                //    isnull(p.ThanhPhamName, tp.Ten) as [Thành Phẩm],
                //    isnull(p.SizeName, s.Ten) as [Size],
                //    mau.Ten as [Màu],
                //    CAST(Sum(p.TrongLuong) as decimal(18, 2)) as [Trọng Lượng],
                //    Count(p.TrongLuong) as [Số Rổ],
                //    cast (Avg(p.Trongluong) as decimal(18, 2)) as [Trung Bình],
                //    p.MaXuongSanXuat as [Xuong]
                //From
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
                //                            p.Ngay <= @ngay
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
                //    ) p,
                //    MaMauFillet mau,
                //    MaSizeFillet s,
                //    MaThanhPhamFillet tp,
                //    MaLoaiCaFillet la
                //Where
                //    p.Ngay <= @ngay
                //    and p.Ngay >= @fromDate
                //    and p.SuDung = 1
                //    and p.MaLoaiCa = la.Ma
                //    and p.MaLoaiThanhPham = tp.Ma
                //    and p.MaSize = s.Ma
                //    and p.MaMau = mau.Ma
                //Group By
                //    p.LoTheoLine,
                //    p.ThanhPhamName,
                //    p.SizeName,
                //    p.MSL,
                //    la.Ten,
                //    tp.Ten,
                //    s.Ten,
                //    mau.Ten,
                //    p.MaXuongSanXuat)
                //	p
                //	group by p.Lô,p.[Loại Cá],p.Màu,p.Size,p.[Thành Phẩm],p.Xuong order by p.Lô";
                var query = @"Select
    p.Lô,
    p.[Loại Cá],
    p.Màu,
    p.Size,
    p.[Số Rổ],
    p.[Mã Thành Phẩm],
    p.[Thành Phẩm],
    p.[Trọng Lượng],
    p.[Trung Bình],
    p.Xuong,
    map.MaBravoFillet as MaLuong
from
    (
        select
            p.Lô,
            p.[Loại Cá],
            p.Màu,
            p.Size,
            Sum(p.[Số Rổ]) as [Số Rổ],
            p.MaLoaiThanhPham as [Mã Thành Phẩm],
            p.[Thành Phẩm],
            Sum(p.[Trọng Lượng]) as [Trọng Lượng],
            Avg(p.[Trung Bình]) as [Trung Bình],
            p.Xuong,
            p.MaLoaiThanhPham,
            p.MaSize,
            p.MaLoaiCa
        from
            (
                select
                    p.*,
                    isnull(p.ThanhPhamName, tp.Ten) as [Thành Phẩm]
                from
(
                        select
                            isnull(p.LoTheoLine, p.MSL) as [Lô],
                            la.Ten as [Loại Cá],
                            isnull(p.ThanhPhamName, NULL) as ThanhPhamName,
                            isnull(p.SizeName, s.Ten) as [Size],
                            mau.Ten as [Màu],
                            CAST(Sum(p.TrongLuong) as decimal(18, 2)) as [Trọng Lượng],
                            Count(p.TrongLuong) as [Số Rổ],
                            cast (Avg(p.Trongluong) as decimal(18, 2)) as [Trung Bình],
                            p.MaXuongSanXuat as [Xuong],
                            p.MaLoaiCa,
                            isnull(p.MaSizeBoTri, p.MaSize) as MaSize,
                            isnull(p.MaThanhPhamBoTri, p.MaLoaiThanhPham) as MaLoaiThanhPham
                        From
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
                                                    (
                                                        select
                                                            *
                                                        from
                                                            PhieuCanTPFillet
                                                        where
                                                            TrongLuong > 0
                                                    ) p
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
                                                    p.Ngay <= @ngay
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
                                    left join MaThanhPhamFillet tp on bt.MaThanhPham = tp.Ma
                                    and tp.MaCa = p.MaLoaiCa
                                    left join MaThanhPhamFillet tp2 on p.MaLoaiThanhPham = tp2.Ma
                                    and tp2.MaCa = p.MaLoaiCa
                                    LEFT join MaSizeFillet s on bt.MaSize = s.Ma
                                    LEFT join MaSizeFillet s2 on bt.MaSizePhu = s2.Ma
                            ) p,
                            MaMauFillet mau,
                            MaSizeFillet s,
                            MaLoaiCaFillet la
                        Where
                            p.SuDung = 1
                            and p.MaLoaiCa = la.Ma
                            and p.MaSize = s.Ma
                            and p.MaMau = mau.Ma
                        Group By
                            p.LoTheoLine,
                            p.ThanhPhamName,
                            p.SizeName,
                            p.MSL,
                            la.Ten,
                            s.Ten,
                            mau.Ten,
                            p.MaXuongSanXuat,
                            p.MaLoaiCa,
                            p.MaSize,
                            p.MaLoaiThanhPham,
                            p.MaSizeBoTri,
                            p.MaThanhPhamBoTri
                    ) p
                    left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma
                    and p.MaLoaiCa = tp.MaCa
            ) p
        group by
            p.Lô,
            p.[Loại Cá],
            p.Màu,
            p.Size,
            p.[Thành Phẩm],
            p.Xuong,
            p.MaLoaiThanhPham,
            p.MaSize,
            p.MaLoaiCa
    ) p
    Left Join MapThanhPhamFillet map on map.MaTPFillet = p.[Mã Thành Phẩm]
    and map.MaLoaiCaFillet = p.MaLoaiCa
    and map.MaSize = p.Size
order by
    p.[Lô]";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
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

        public List<T> Gets<T>(DateTime dateTime, string xuongId, string nhanVienId, bool isPhucVu = false)
        {
            try
            {
                var query = @"SELECT
    p.MaNhanVien,
    p.HoVaTen as NhanVienName,
    p.MaLoaiCa,
    p.MSL as MaLo,
    p.MaLoaiThanhPham as MaThanhPham,
    p.MaSize,
    Count(p.TrongLuong) as SoRo,
    CAST(Sum(p.TrongLuong) as decimal(18, 2)) as TrongLuong
FROM
    PhieuCanTPFillet p
Where
    p.SuDung = 1
    and p.Ngay = @ngay
    and p.MaXuongSanXuat = @xuongId
    and p.MaNhanVien = @nhanVienId
Group By
    p.MaNhanVien,
    p.MSL,
    p.MaLoaiCa,
    p.MaSize,
    p.MaLoaiThanhPham,
    p.HoVaTen";
                if (isPhucVu == true)
                {
                    query = @"SELECT
    p.MaNhanVienPhucVu as MaNhanVien,
    p.HoVaTen as NhanVienName,
    p.MaLoaiCa,
    p.MSL as MaLo,
    p.MaLoaiThanhPham as MaThanhPham,
    p.MaSize,
    Count(p.TrongLuong) as SoRo,
    CAST(Sum(p.TrongLuong) as decimal(18, 2)) as TrongLuong
FROM
    PhieuCanTPFillet p
Where
    p.SuDung = 1
    and p.Ngay = @ngay
    and p.MaXuongSanXuat = @xuongId
    and p.MaNhanVienPhucVu = @nhanVienId
Group By
    p.MaNhanVienPhucVu,
    p.MSL,
    p.MaLoaiCa,
    p.MaSize,
    p.MaLoaiThanhPham,
    p.HoVaTen";
                }

                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId, nhanVienId }).ToList();
                return items;
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
                    "Select ISNULL(SUM(TrongLuong),0) from PhieuCanTPFillet where Ngay = @ngay and CONVERT(time,ThoiGianCan) >= @fromTime and CONVERT(time,ThoiGianCan) < @toTime and SuDung = 1 and MaXuongSanXuat = @xuongId";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var items = connection.ExecuteScalar<double>(
                        query,
                        new { ngay = dateTime.Date, fromTime = fromTime, toTime = toTime, xuongId = xuongId });
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
            IEnumerable<string> ids)
        {
            try
            {
                string listOfIdsJoined = "('" + String.Join("','", ids.ToArray()) + "')";
                var query =
                    $@"Select ISNULL(SUM(TrongLuong),0) from PhieuCanTPFillet where Ngay = @ngay and CONVERT(time,ThoiGianCan) >= @fromTime and CONVERT(time,ThoiGianCan)< @toTime and SuDung = 1 and MaXuongSanXuat = @xuongId and MaNhanVien in {listOfIdsJoined}";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.ExecuteScalar<double>(
                    query,
                    new { ngay = dateTime.Date, fromTime = fromTime, toTime = toTime, xuongId = xuongId });
                return items;
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
                        new { ngay = dateTime.Date, fromTime = fromTime, toTime = toTime, xuongId = xuongId });
                    return items;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public Tuple<int, decimal> GetSoRoTongTrongLuongByNhanVienId(DateTime dateTime, string nhanVienId)
        {
            try
            {
                var query = @"Select
    
    ISNULL(COUNT(*), 0) as Item1,
    Isnull(SUM(TrongLuong), 0) as Item2
from
    PhieuCanTPFillet
where
    Ngay = @ngay
    and MaNhanVien = @nhanVienId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var row = connection.Query<Tuple<int, decimal>>(query, new { ngay = dateTime.Date, nhanVienId })
                    .SingleOrDefault();
                return row;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<string> GetSqlsInBatches(IList<Models.Repos.Models.PhieuCanTPFillet> phieuCans)
        {
            var insertSql = @"INSERT INTO [dbo].[PhieuCanTPFillet]
           ([MaMayTinhCan]
           ,[MaUserCan]
           ,[ThoiGianCan]
           ,[Ngay]
           ,[MaXuongSanXuat]
           ,[MSL]
           ,[MaLoaiCa]
           ,[MaLoaiThanhPham]
           ,[MaSize]
           ,[MaMau]
           ,[MaNhanVien]
           ,[HoVaTen]
           ,[MaTheTu]
           ,[TrongLuong]
           ,[SuDung]
           ,[GhiChu],[MaNhanVienPhucVu],[LoaiCan])
     VALUES";
            var valuesSql =
                @"('{0}','{1}','{2}', '{3}', '{4}', '{5}', '{6}', '{7}','{8}','{9}','{10}',N'{11}', '{12}', {13}, {14}, '{15}','{16}','{17}')";
            var batchSize = 1000;

            var sqlsToExecute = new List<string>();
            var numberOfBatches = (int)Math.Ceiling((double)phieuCans.Count / batchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var phieuCanToInsert = phieuCans.Skip(i * batchSize).Take(batchSize);
                var valuesToInsert = phieuCanToInsert.Select(
                    x => string.Format(
                        valuesSql,
                        x.MaMayTinhCan,
                        x.MaUserCan,
                        x.ThoiGianCan.ToString(@"hh\:mm\:ss"),
                        x.Ngay.ToString("yyyy-MM-dd"),
                        x.MaXuongSanXuat,
                        x.MSL,
                        x.MaLoaiCa,
                        x.MaLoaiThanhPham,
                        x.MaSize,
                        x.MaMau,
                        x.MaNhanVien,
                        x.HoVaTen,
                        x.MaTheTu,
                        x.TrongLuong,
                        x.SuDung == true ? 1 : 0,
                        string.Empty,
                        x.MaNhanVienPhucVu,
                        x.LoaiCan));
                sqlsToExecute.Add(insertSql + string.Join(",", valuesToInsert));
            }

            return sqlsToExecute;
        }

        public List<T> GetsTPSoft<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"DECLARE @thoiGianCan as TIME(7);

SET
    @thoiGianCan = '05:00:00';

select
    p.ID,
    p.CMND,
    p.ThoiGian,
    p.[Status],
    map.MaBravoFillet as [CongDoanID],
    p.TrongLuong,
    p.DateSync
from
    (
        SELECT
            (
                p.MaMayTinhCan + '-' + p.MaXuongSanXuat + '-K09-' + CONVERT(
                    Nvarchar(10),
                    format(p.Ngay, 'yyyy-MM-dd')
                ) + '-' + CONVERT(
                    Nvarchar(8),
                    format(
                        CAST(p.ThoiGianCan AS datetime),
                        'HH-mm-ss'
                    )
                )
            ) as ID,
            n.Tel as CMND,
            cast(
                CONVERT(Nvarchar(10), p.Ngay) + ' ' + CONVERT(
                    Nvarchar(8),
                    format(
                        CAST(p.ThoiGianCan AS datetime),
                        'HH:mm:ss'
                    )
                ) as datetime
            ) as [ThoiGian],
            p.SuDung as [Status],
            p.TrongLuong,
            (
                case
                    when p.ThoiGianCan <= @thoiGianCan then 1
                    else 0
                end
            ) as IsTangCa,
            GetDate() as [DateSync],
            p.MaLoaiThanhPham,
            p.MaLoaiCa,
            p.MaSize
        from
            PhieuCanTPFillet p
            Left Join NhanVienDaiThanh n On n.MaNhanVien = p.MaNhanVien
        where
            p.Ngay = @ngay
            and p.MaXuongSanXuat = @xuongId
    ) p
    Left Join MapThanhPhamFillet map on map.MaTPFillet = p.MaLoaiThanhPham
    and map.MaLoaiCaFillet = p.MaLoaiCa
    and map.MaSize = p.MaSize
    and map.IsTangCa = p.IsTangCa
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetsTPSoft_old<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"SELECT
    (
        p.MaMayTinhCan + '-' + p.MaXuongSanXuat + '-K09-' + CONVERT(
            Nvarchar(10),
            format(p.Ngay, 'yyyy-MM-dd')
        ) + '-' + CONVERT(
            Nvarchar(8),
            format(
                CAST(p.ThoiGianCan AS datetime),
                'HH-mm-ss'
            )
        )
    ) as ID,
    n.Tel as CMND,
    cast(
        CONVERT(Nvarchar(10), p.Ngay) + ' ' + CONVERT(
            Nvarchar(8),
            format(
                CAST(p.ThoiGianCan AS datetime),
                'HH:mm:ss'
            )
        ) as datetime
    ) as [ThoiGian],
    p.SuDung as [Status],
    map.MaBravoFillet as [CongDoanID],
    p.TrongLuong,
    GetDate() as [DateSync]
from
    PhieuCanTPFillet p
    Left Join MapThanhPhamFillet map on map.MaTPFillet = p.MaLoaiThanhPham
    and map.MaLoaiCaFillet = p.MaLoaiCa
    and map.MaSize = p.MaSize
    Left Join NhanVienDaiThanh n On n.MaNhanVien = p.MaNhanVien
where
    p.Ngay = @ngay
    and p.MaXuongSanXuat = @xuongId;
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetTinhLuongs<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"DECLARE @khuVucId as VARCHAR(50);

DECLARE @thoiGianCan as TIME(7),@loaiDonGiaId as varchar(50);

Set
    @khuVucId = 'FL';
Set 
    @loaiDonGiaId ='SP';
SET
    @thoiGianCan = '05:00:00';

Select
    p.Ngay,
    p.MaXuong,
    p.MaLo,
    p.MaNhanVien,
    p.MaHoSo,
    p.NhanVienName,
    map.MaBravoFillet as BravoId,
    map.DonGia,
    p.MaThanhPhamOrg,
    p.ThanhPhamNameOrg,
    p.MaThanhPham,
    p.ThanhPhamName,
    map.MaBravoFillet as MaSanPham,
    p.TenSanPham,
    p.MaSize,
    p.MaLoaiCa,
    p.TyLe,
    p.TrongLuong,
    p.TrongLuongOrg,
    p.SoRo,
    p.SoRoOrg,
    p.IsTangCa,
    (p.TrongLuong * map.DonGia * map.HeSo) as ThanhTien
from
    (
        Select
            p.Ngay,
            p.MaXuong,
            p.MaLo,
            p.MaNhanVien,
            p.MaHoSo,
            p.NhanVienName,
            p.MaThanhPhamOrg,
            p.ThanhPhamNameOrg,
            p.MaThanhPham,
            p.ThanhPhamName,
            p.TenSanPham,
            p.MaSize,
            p.MaLoaiCa,
            p.TyLe,
            c.TrongLuong,
            c.TrongLuongOrg,
            c.SoRo,
            c.SoRoOrg,
            c.IsTangCa
        from
            (
                SELECT
                    p.Ngay,
                    p.MaXuong,
                    p.MaLo,
                    p.MaNhanVien,
                    n.MaHoSo,
                    n.Name as NhanVienName,
                    p.MaThanhPhamOrg,
                    p.ThanhPhamNameOrg,
                    p.MaThanhPham,
                    tp.Ten as ThanhPhamName,
                    '' as TenSanPham,
                    p.MaSize,
                    p.MaLoaiCa,
                    p.TyLe,
                    cast(SUM(TrongLuongOrg) as decimal(18, 2)) as TrongLuongOrg,
                    cast(SUM(TrongLuongOrg_TangCa) as decimal(18, 2)) as TrongLuongOrg_TangCa,
                    cast((SUM(TrongLuongOrg) * p.TyLe) as decimal(18, 2)) as TrongLuong,
                    CAST(
                        (SUM(TrongLuongOrg_TangCa) * p.TyLe) as decimal(18, 2)
                    ) as TrongLuong_TangCa,
                    cast(SUM(SoRoOrg) as int) as SoRoOrg,
                    CAST(SUM(SoRoOrg_TangCa) as int) as SoRoOrg_TangCa,
                    cast((SUM(SoRoOrg) * p.TyLe) as int) as SoRo,
                    CAST((SUM(SoRoOrg_TangCa) * p.TyLe) as int) as SoRo_TangCa
                from
                    (
                        Select
                            p.Ngay,
                            p.MaXuongSanXuat as MaXuong,
                            p.MSL as MaLo,
                            p.MaNhanVien,
                            p.MaSize,
                            p.MaLoaiCa,
                            p.MaLoaiThanhPham as MaThanhPhamOrg,
                            _tp.Ten as ThanhPhamNameOrg,
                            tp.MaThanhPhamDes as MaThanhPham,
                            tp.TyLe,
                            SUM(
                                case
                                    when p.ThoiGianCan <= @thoiGianCan then p.TrongLuong
                                    else 0
                                end
                            ) as TrongLuongOrg_TangCa,
                            SUM(
                                case
                                    when p.ThoiGianCan > @thoiGianCan then p.TrongLuong
                                    else 0
                                end
                            ) as TrongLuongOrg,
                            SUM(
                                case
                                    when p.ThoiGianCan <= @thoiGianCan then 1
                                    else 0
                                end
                            ) as SoRoOrg_TangCa,
                            SUM(
                                case
                                    when p.ThoiGianCan > @thoiGianCan then 1
                                    else 0
                                end
                            ) as SoRoOrg
                        from
                            PhieuCanTPFillet p,
                            (
                                select
                                    (@ngay) as Ngay,
                                    (@khuVucId) as MaKhuVuc,
                                    tp.MaXuong,
                                    ISNULL(tppt.MaLo, tp.MaLo) as MaLo,
                                    tp.MaThanhPham as MaThanhPhamOrg,
                                    ISNULL(tppt.MaThanhPhamDes, tp.MaThanhPham) as MaThanhPhamDes,
                                    ISNULL(tppt.TyLe, 1) as TyLe
                                from
                                    (
                                        Select
                                            (@ngay) as Ngay,
                                            p.MaXuong,
                                            p.MaLo,
                                            tp.Ma as MaThanhPham
                                        from
                                            MaThanhPhamFillet tp,
                                            (
                                                Select
                                                    distinct MSL as MaLo,
                                                    MaXuongSanXuat as MaXuong
                                                from
                                                    PhieuCanNguyenLieu
                                                where
                                                    Ngay = @ngay
                                            ) p
                                    ) tp
                                    LEFT join(
                                        Select
                                            *
                                        from
                                            MaThanhPham_PhoiTron
                                        where
                                            Ngay = @ngay
                                            and MaKhuVuc = @khuVucId
                                    ) tppt ON tp.MaThanhPham = tppt.MaThanhPhamOrg
                                    and tp.Ngay = tppt.Ngay
                                    and tp.MaXuong = tppt.MaXuong
                                    and tp.MaLo = tppt.MaLo
                            ) tp,
                            MaThanhPhamFillet _tp
                        where
                            p.Ngay = @ngay
                            and p.MaLoaiThanhPham = tp.MaThanhPhamOrg
                            and p.MaXuongSanXuat = tp.MaXuong
                            and p.MSL = tp.MaLo
                            and p.MaLoaiThanhPham = _tp.Ma
                            and p.SuDung = 1
                        GROUP by
                            p.Ngay,
                            p.MaNhanVien,
                            p.MaLoaiThanhPham,
                            p.MaXuongSanXuat,
                            p.MSL,
                            tp.MaThanhPhamDes,
                            tp.TyLe,
                            _tp.Ten,
                            p.MaSize,
                            p.MaLoaiCa
                    ) p,
                    MaThanhPhamFillet tp,
                    NhanVienDaiThanh n
                Where
                    p.MaThanhPham = tp.Ma
                    and p.MaNhanVien = n.MaNhanVien
                    and p.MaXuong = @xuongId
                GROUP by
                    p.Ngay,
                    p.MaXuong,
                    p.MaLo,
                    p.MaNhanVien,
                    n.Name,
                    n.MaHoSo,
                    p.MaThanhPhamOrg,
                    p.ThanhPhamNameOrg,
                    p.MaThanhPham,
                    tp.Ten,
                    p.TyLe,
                    p.MaSize,
                    p.MaLoaiCa
            ) p
            CROSS APPLY(
                values
                    (
                        1,
                        TrongLuong_TangCa,
                        TrongLuongOrg_TangCa,
                        SoRoOrg_TangCa,
                        SoRo_TangCa
                    ),
                    (0, TrongLuong, TrongLuongOrg, SoRoOrg, SoRo)
            ) c (
                IsTangCa,
                TrongLuong,
                TrongLuongOrg,
                SoRoOrg,
                SoRo
            )
    ) p
    LEFT join (
        select
            map.*,
            isnull(dg.DonGia, 0) as DonGia,
            isnull(dg.HeSo,1) as HeSo
        from
            MapThanhPhamFillet map
            LEFT JOIN (
                Select
                    *
                from
                    (
                        Select
                            *,
                            ROW_NUMBER() over (
                                partition by MaSanPham
                                order by
                                    Ngay DESC,
                                    Gio Desc
                            ) as rowId
                        from
                            DG_DonGia
                        where
                            Ngay <= @ngay and MaLoaiDonGia = @loaiDonGiaId and LoaiCan = ''
                    ) p
                where
                    p.rowId = 1
            ) dg ON dg.MaSanPham = map.MaBravoFillet
    ) map on p.MaThanhPham = map.MaTPFillet
    and p.IsTangCa = map.IsTangCa
    and p.MaSize = map.MaSize
    and p.MaLoaiCa = map.MaLoaiCaFillet
where
    (
        p.TrongLuong + p.TrongLuongOrg + cast(p.SoRo + p.SoRoOrg as decimal(18, 2))
    ) > 0
order by
    MaXuong,
    MaLo,
    MaNhanVien,
    MaThanhPham";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetTinhLuongs_old<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"DECLARE @khuVucId as VARCHAR(50);

Set
    @khuVucId = 'FL';

                SELECT
    p.Ngay,
    p.MaXuong,
    p.MaLo,
    p.MaNhanVien as MaNhanVien,
    n.MaHoSo,
    n.Name as NhanVienName,
    map.MaBravoFillet as BravoId,
    p.MaThanhPhamOrg,
    p.ThanhPhamNameOrg,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
     map.MaBravoFillet as MaSanPham,
    '' as TenSanPham,
    p.TyLe,
    cast(SUM(TrongLuongOrg) as decimal(18, 2)) as TrongLuongOrg,
    cast(SUM(TrongLuong) as decimal(18, 2)) as TrongLuong ,
    cast(SUM(SoRoOrg) as int) as SoRoOrg,
    cast(SUM(SoRo) as int) as SoRo
from
    (
        Select
            p.Ngay,
            p.MaXuongSanXuat as MaXuong,
            p.MSL as MaLo,
            p.MaNhanVien,
            p.MaSize,
            p.MaLoaiThanhPham as MaThanhPhamOrg,
            _tp.Ten as ThanhPhamNameOrg,
            tp.MaThanhPhamDes as MaThanhPham,
            tp.TyLe,
            Round(Sum(p.TrongLuong), 1) as [TrongLuongOrg],
            (Round(Sum(p.TrongLuong), 1) * tp.TyLe) as TrongLuong,
            Count(p.TrongLuong) as [SoRoOrg],
            CAST((Count(p.TrongLuong) * tp.TyLe) as int) as SoRo
        from
            PhieuCanTPFillet p,
            (
                select
                    (@ngay) as Ngay,
                    (@khuVucId) as MaKhuVuc,
                    tp.MaXuong,
                    ISNULL(tppt.MaLo, tp.MaLo) as MaLo,
                    tp.MaThanhPham as MaThanhPhamOrg,
                    ISNULL(tppt.MaThanhPhamDes, tp.MaThanhPham) as MaThanhPhamDes,
                    ISNULL(tppt.TyLe, 1) as TyLe
                from
                    (
                        Select
                            (@ngay) as Ngay,
                            p.MaXuong,
                            p.MaLo,
                            tp.Ma as MaThanhPham
                        from
                            MaThanhPhamFillet tp,
                            (
                                Select
                                    distinct MSL as MaLo,
                                    MaXuongSanXuat as MaXuong
                                from
                                    PhieuCanNguyenLieu
                                where
                                    Ngay = @ngay
                            ) p
                    ) tp
                    LEFT join(
                        Select
                            *
                        from
                            MaThanhPham_PhoiTron
                        where
                            Ngay = @ngay
                            and MaKhuVuc = @khuVucId
                    ) tppt ON tp.MaThanhPham = tppt.MaThanhPhamOrg
                    and tp.Ngay = tppt.Ngay
                    and tp.MaXuong = tppt.MaXuong
                    and tp.MaLo = tppt.MaLo
            ) tp,
            MaThanhPhamFillet _tp
        where
            p.Ngay = @ngay
            and p.MaLoaiThanhPham = tp.MaThanhPhamOrg
            and p.MaXuongSanXuat = tp.MaXuong
            and p.MSL = tp.MaLo
            and p.MaLoaiThanhPham = _tp.Ma
            and p.SuDung = 1
        GROUP by
            p.Ngay,
            p.MaNhanVien,
            p.MaLoaiThanhPham,
            p.MaXuongSanXuat,
            p.MSL,
            tp.MaThanhPhamDes,
            tp.TyLe,
            _tp.Ten,
            p.MaSize
    ) p,
    MaThanhPhamFillet tp,
    MapThanhPhamFillet map,
    NhanVienDaiThanh n
Where
    p.MaThanhPham = tp.Ma
    and p.MaThanhPham = map.MaTPFillet
    and p.MaSize = map.MaSize
    and p.MaNhanVien = n.MaNhanVien
    and p.MaXuong = @xuongId
GROUP by
    p.Ngay,
    p.MaXuong,
    p.MaLo,
    p.MaNhanVien,
    n.Name,
    n.MaHoSo,
    map.MaBravoFillet,
    p.MaThanhPhamOrg,
    p.ThanhPhamNameOrg,
    p.MaThanhPham,
    tp.Ten,
    p.TyLe
order by
    MaXuong,
    MaLo,
    MaNhanVien,
    MaThanhPham";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetTongHopNhanVienPhucVus(DateTime dateTime)
        {
            try
            {
                var query = @"DECLARE @ParmDefinition NVARCHAR(500);

DECLARE @columnHeaders NVARCHAR (MAX);
DECLARE @columnHeadersSelected NVARCHAR(Max);

DECLARE @GrandTotalCol NVARCHAR (MAX);

DECLARE @GrandTotalRow NVARCHAR(MAX);

DECLARE @FinalQuery NVARCHAR (MAX);

SET
    @ParmDefinition = N'@ngay Date';
SELECT
    @columnHeadersSelected = ISNULL(@columnHeadersSelected + ',', '') + 'Cast( ISNULL(' + QUOTENAME(p.Ten) + ',0) as DECIMAL(18,2)) as ' + QUOTENAME(REPLACE(p.Ten, '.', ','))
from
    (
        select
            distinct tp.Ten
        from
            PhieuCanTPFillet p,
            MaThanhPhamFillet tp
        where
            p.Ngay = @ngay
            and p.TrongLuong > 0
            and p.MaLoaiThanhPham = tp.Ma
    ) p
SELECT
    @columnHeaders = COALESCE (
        @columnHeaders + ', [' + tp.Ten + ']',
        '[' + tp.Ten + ']'
    )
FROM
    PhieuCanTPFillet p,
    MaThanhPhamFillet tp
Where
    Ngay = @ngay
    and p.MaLoaiThanhPham = tp.Ma
    and p.TrongLuong > 0
GROUP BY
    tp.Ten,
    [MAX]
ORDER BY
    [MAX] DESC PRINT @columnHeaders
    /* GRAND TOTAL COLUMN */
SELECT
    @GrandTotalCol = COALESCE (
        @GrandTotalCol + 'ISNULL([' + tp.Ten + '],0) + ',
        'ISNULL([' + tp.Ten + '],0) + '
    )
FROM
    PhieuCanTPFillet p ,
    MaThanhPhamFillet tp
Where
    Ngay = @ngay
    and p.MaLoaiThanhPham = tp.Ma
    and p.TrongLuong > 0
GROUP BY
    tp.Ten,
    [MAX]
ORDER BY
    [MAX] DESC
SET
    @GrandTotalCol = LEFT (@GrandTotalCol, LEN (@GrandTotalCol) -1)
    /* GRAND TOTAL ROW */
SELECT
    @GrandTotalRow = COALESCE(
        @GrandTotalRow + ',ISNULL(SUM([' + tp.Ten + ']),0)',
        'ISNULL(SUM([' + tp.Ten + ']),0)'
    )
FROM
    PhieuCanTPFillet p,
    MaThanhPhamFillet tp
Where
    Ngay = @ngay
    and p.MaLoaiThanhPham = tp.Ma
    and p.TrongLuong > 0
GROUP BY
    tp.Ten,
    [MAX]
ORDER BY
    [MAX] DESC
    /* MAIN QUERY */
SET
    @FinalQuery = N' SELECT
    [Mã Nhân Viên],[MaHoSo],[Lô],[Size],[Xuong],'+@columnHeadersSelected+',
    (' + @GrandTotalCol + ') AS [Tong]
FROM
    (
        SELECT
            n.MaNhanVien as [Mã Nhân Viên],
            n.MaHoSo as [MaHoSo],
            n.Name as [Tên Nhân Viên],
            p.MSL as [Lô],
            s.Ten as [Size],
            tp.Ten as [TP],
           isnull( round(Sum(p.TrongLuong), 2) ,0)as TrongLuong,
            p.MaXuongSanXuat as [Xuong]
        FROM
            PhieuCanTPFillet p,
            NhanVienDaiThanh n,
            MaThanhPhamFillet tp,
            MaSizeFillet s
        Where
            p.Ngay = @ngay
            and p.TrongLuong > 0
            and p.MaNhanVienPhucVu = n.MaNhanVien
            and p.MaLoaiThanhPham = tp.Ma
            and p.MaSize = s.Ma
        Group By
            n.MaNhanVien,
            n.MaHoSo,
            n.Name,
            p.MSL,
            s.Ten,
            tp.Ten,
            p.MaXuongSanXuat
    ) A PIVOT (
        Sum(TrongLuong) FOR TP IN (' + @columnHeaders + ')
    ) B';

EXEC sp_executesql @FinalQuery,
@ParmDefinition,
@ngay";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
                // cmd.Parameters.AddWithValue("@xuongId", xuongId);
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

        public List<T> GetTongHopNhanVienPhucVus<T>(DateTime dateTime, string sanPhamId, string xuongId)
        {
            try
            {
                var query = @"
        SELECT
            n.MaNhanVien ,
            n.MaHoSo ,
            isnull( round(Sum(p.TrongLuong), 2) ,0)as SanLuongHuong,
            1 as TyLeHuong,
            0 as SanLuongTru,
            0 as SoGio,
            1 as TyLeHuong,
            0 as TyLeTru,
            @sanPhamId as MaThanhPham
        FROM
            PhieuCanTPFillet p,
            NhanVienDaiThanh n
        Where
            p.Ngay = @ngay
            and p.TrongLuong > 0
            and p.MaNhanVienPhucVu = n.MaNhanVien and p.MaXuongSanXuat = @xuongId
        Group By
            n.MaNhanVien,
            n.MaHoSo";
                var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, sanPhamId, xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetTongHopNhanViens(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"
DECLARE @ParmDefinition NVARCHAR(500);

DECLARE @columnHeaders NVARCHAR (MAX);

DECLARE @columnHeadersSelected NVARCHAR(Max);

DECLARE @GrandTotalCol NVARCHAR (MAX);

DECLARE @GrandTotalRow NVARCHAR(MAX);

DECLARE @FinalQuery NVARCHAR (MAX);

SET
    @ParmDefinition = N'@ngay Date,@fromdate as Date, @xuongId as varchar(50)';

IF OBJECT_ID(N'tempdb..#phieucan') IS NOT NULL BEGIN DROP TABLE #phieucan
END
Select
    p.Ngay,
    p.MaNhanVien as [Mã Nhân Viên],
    n.MaHoSo as [MaHoSo],
    n.Name as [Tên Nhân Viên],
    n.DeptName0 as Nhom,
    isnull(p.LoTheoLine, p.MSL) as [Lô],
    la.Ten as [Loại Cá],
    isnull(p.ThanhPhamName, tp.Ten) as [TP],
    ISNULL(p.SizeName, s.Ten) as [Size],
    p.TrongLuong as [TrongLuong],
    p.MaXuongSanXuat as [Xuong] into #phieucan
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
                            (
                                select
                                    *
                                from
                                    PhieuCanTPFillet
                                where
                                    TrongLuong > 0
                            ) p
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
                            p.Ngay <= @ngay
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
            left join MaThanhPhamFillet tp on bt.MaThanhPham = tp.Ma
            and tp.MaCa = p.MaLoaiCa
            left join MaThanhPhamFillet tp2 on p.MaLoaiThanhPham = tp2.Ma
            and tp2.MaCa = p.MaLoaiCa
            LEFT join MaSizeFillet s on bt.MaSize = s.Ma
            LEFT join MaSizeFillet s2 on bt.MaSizePhu = s2.Ma
    ) p
    left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
    left join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
    left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma
    and p.MaLoaiCa = tp.MaCa
    LEFT join MaSizeFillet s on p.MaSize = s.Ma
SELECT
    @columnHeadersSelected = ISNULL(@columnHeadersSelected + ',', '') + 'Cast( ISNULL(' + QUOTENAME(p.Ten) + ',0) as DECIMAL(18,2)) as ' + QUOTENAME(REPLACE(p.Ten, '.', ','))
from
    (
        select
            distinct p.TP as Ten
        from
            #phieucan p
        where
            p.TrongLuong > 0
    ) p
SELECT
    @columnHeaders = COALESCE (
        @columnHeaders + ', [' + p.TP + ']',
        '[' + p.TP + ']'
    )
FROM
    #phieucan p
where
    p.TrongLuong > 0
GROUP BY
    p.TP
    /* GRAND TOTAL COLUMN */
SELECT
    @GrandTotalCol = COALESCE (
        @GrandTotalCol + 'ISNULL([' + p.TP + '],0) + ',
        'ISNULL([' + p.TP + '],0) + '
    )
FROM
    #phieucan p
where
    p.TrongLuong > 0
GROUP BY
    p.TP
SET
    @GrandTotalCol = LEFT (@GrandTotalCol, LEN (@GrandTotalCol) -1)
    /* GRAND TOTAL ROW */
SELECT
    @GrandTotalRow = COALESCE(
        @GrandTotalRow + ',ISNULL(SUM([' + p.TP + ']),0)',
        'ISNULL(SUM([' + p.TP + ']),0)'
    )
FROM
    #phieucan p
where
    p.TrongLuong > 0
GROUP BY
    p.TP
    /* MAIN QUERY */
SET
    @FinalQuery = N' SELECT [Ngay],
    [Mã Nhân Viên],[MaHoSo],[Tên Nhân Viên],[Nhom],[Lô],[Size],[Xuong],' + @columnHeadersSelected + ',
    (' + @GrandTotalCol + ') AS [Tong]
FROM
    (
     select * from #phieucan
    ) A PIVOT (
       Sum(TrongLuong) FOR TP IN (' + @columnHeaders + ')
    ) B';

EXEC sp_executesql @FinalQuery,
@ParmDefinition,
@ngay,
@fromdate,
@xuongId";
                using var connection = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
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

        public DataTable GetTongHopNhanViens(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = @"DECLARE @ParmDefinition NVARCHAR(500);

DECLARE @columnHeaders NVARCHAR (MAX);

DECLARE @columnHeadersSelected NVARCHAR(Max);

DECLARE @GrandTotalCol NVARCHAR (MAX);

DECLARE @GrandTotalRow NVARCHAR(MAX);

DECLARE @FinalQuery NVARCHAR (MAX);

SET
    @ParmDefinition = N'@ngay Date, @xuongId as varchar(50)';

IF OBJECT_ID(N'tempdb..#phieucan') IS NOT NULL BEGIN DROP TABLE #phieucan
END
Select
    p.MaNhanVien as [Mã Nhân Viên],
    n.MaHoSo as [MaHoSo],
    n.Name as [Tên Nhân Viên],
    isnull(p.LoTheoLine, p.MSL) as [Lô],
    la.Ten as [Loại Cá],
    isnull(p.ThanhPhamName, tp.Ten) as [TP],
    ISNULL(p.SizeName, s.Ten) as [Size],
    p.TrongLuong as [TrongLuong],
    p.MaMayTinhCan,
    p.MaXuongSanXuat as [Xuong] into #phieucan
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
            case when tp2.IsNotSetByTime =1 then NULL else bt.MaThanhPham end as MaThanhPhamBoTri,
            case when tp2.IsNotSetByTime =1 then NULL else  tp.Ten end  as ThanhPhamName
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
                            and nl.Ngay = @ngay
                            and p.MaNhanVien = nl.MaNhanVien
                            and nl.Gio = (
                                Select
                                    MAX(nlc.Gio)
                                from
                                    NhanVienTheoLine nlc
                                where
                                    nlc.Ngay = @ngay
                                    and nlc.MaNhanVien = p.MaNhanVien
                                    and nlc.Gio <= p.ThoiGianCan
                            )
                            left join XiNghiep xn on xn.Ma = p.MaXuongSanXuat
                            left join LineFilletv2 ln on nl.MaLine = ln.Ma
                            left join ViTriFillet vt on nl.MaViTri = vt.Ma
                        where
                            p.Ngay = @ngay
                            and p.MaXuongSanXuat = @xuongId
                    ) p
                    left JOIN LoTheoLine ll ON p.MaLine = ll.MaLine
                    and p.Ngay = ll.Ngay
                    and p.CodeId = ll.CodeId
                    and ll.Ngay = @ngay
                    and ll.Gio = (
                        Select
                            MAX(llc.Gio)
                        from
                            LoTheoLine llc
                        where
                            llc.MaLine = p.MaLine
                            and llc.CodeId = p.CodeId
                            and llc.Ngay = @ngay
                            and llc.Gio <= p.ThoiGianCan
                    )
            ) p
            left JOIN BoTriLoSizeThanhPham bt ON p.LoTheoLine = bt.MaLo
            and p.CodeId = bt.CodeId
            and p.MaViTri = bt.MaViTri
            and bt.Ngay = @ngay
            and bt.Gio = (
                Select
                    MAX(btc.Gio)
                from
                    BoTriLoSizeThanhPham btc
                where
                    btc.MaLo = p.LoTheoLine
                    and btc.MaViTri = p.MaViTri
                    and btc.CodeId = p.CodeId
                    and btc.Ngay = @ngay
                    and btc.Gio <= p.ThoiGianCan
            )
            left join MaThanhPhamFillet tp on bt.MaThanhPham = tp.Ma and tp.MaCa = p.MaLoaiCa
            left join MaThanhPhamFillet tp2 on p.MaLoaiThanhPham = tp2.Ma and tp2.MaCa = p.MaLoaiCa
            LEFT join MaSizeFillet s on bt.MaSize = s.Ma
LEFT join MaSizeFillet s2 on bt.MaSize = s2.Ma
    ) p
    left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
    left join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
    left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma  and p.MaLoaiCa = tp.MaCa
    LEFT join MaSizeFillet s on p.MaSize = s.Ma
SELECT
    @columnHeadersSelected = ISNULL(@columnHeadersSelected + ',', '') + 'Cast( ISNULL(' + QUOTENAME(p.Ten) + ',0) as DECIMAL(18,2)) as ' + QUOTENAME(REPLACE(p.Ten, '.', ','))
from
    (
        select
            distinct p.TP as Ten
        from
            #phieucan p
        where
            p.TrongLuong > 0
    ) p
SELECT
    @columnHeaders = COALESCE (
        @columnHeaders + ', [' + p.TP + ']',
        '[' + p.TP + ']'
    )
FROM
    #phieucan p
where
    p.TrongLuong > 0
GROUP BY
    p.TP
    /* GRAND TOTAL COLUMN */
SELECT
    @GrandTotalCol = COALESCE (
        @GrandTotalCol + 'ISNULL([' + p.TP + '],0) + ',
        'ISNULL([' + p.TP + '],0) + '
    )
FROM
    #phieucan p
where
    p.TrongLuong > 0
GROUP BY
    p.TP
SET
    @GrandTotalCol = LEFT (@GrandTotalCol, LEN (@GrandTotalCol) -1)
    /* GRAND TOTAL ROW */
SELECT
    @GrandTotalRow = COALESCE(
        @GrandTotalRow + ',ISNULL(SUM([' + p.TP + ']),0)',
        'ISNULL(SUM([' + p.TP + ']),0)'
    )
FROM
    #phieucan p
where
    p.TrongLuong > 0
GROUP BY
    p.TP
    /* MAIN QUERY */
SET
    @FinalQuery = N' SELECT
    [Mã Nhân Viên],[MaHoSo],[Tên Nhân Viên],[Lô],[Size],[Xuong],' + @columnHeadersSelected + ',
    (' + @GrandTotalCol + ') AS [Tong]
FROM
    (
     select * from #phieucan
    ) A PIVOT (
       Sum(TrongLuong) FOR TP IN (' + @columnHeaders + ')
    ) B';

EXEC sp_executesql @FinalQuery,
@ParmDefinition,
@ngay,
@xuongId";
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

        public List<T> GetTongHopNhanViens1<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                //                var query = @"
                //DECLARE @ParmDefinition NVARCHAR(500);

                //DECLARE @columnHeaders NVARCHAR (MAX);

                //DECLARE @columnHeadersSelected NVARCHAR(Max);

                //DECLARE @GrandTotalCol NVARCHAR (MAX);

                //DECLARE @GrandTotalRow NVARCHAR(MAX);

                //DECLARE @FinalQuery NVARCHAR (MAX);

                //SET
                //    @ParmDefinition = N'@ngay Date,@fromdate as Date, @xuongId as varchar(50)';

                //IF OBJECT_ID(N'tempdb..#phieucan') IS NOT NULL BEGIN DROP TABLE #phieucan
                //END
                //Select
                //    p.Ngay,
                //    p.MaNhanVien as MaNhanVien
                //    n.MaHoSo as MaHoSo,
                //    n.Name as TenNhanVien,
                //    n.DeptName0 as Nhom,
                //    isnull(p.LoTheoLine, p.MSL) as Lo,
                //    la.Ten as LoaiCaName,
                //    isnull(p.ThanhPhamName, tp.Ten) as ThanhPhamName,
                //    ISNULL(p.SizeName, s.Ten) as [Size],
                //    p.TrongLuong as [TrongLuong],
                //    p.MaXuongSanXuat as [Xuong] into #phieucan
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
                //                             (select * from PhieuCanTPFillet where TrongLuong>0) p
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
                //                            p.Ngay <= @ngay
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
                //            left join MaThanhPhamFillet tp on bt.MaThanhPham = tp.Ma and tp.MaCa = p.MaLoaiCa
                //            left join MaThanhPhamFillet tp2 on p.MaLoaiThanhPham = tp2.Ma and tp2.MaCa = p.MaLoaiCa
                //            LEFT join MaSizeFillet s on bt.MaSize = s.Ma
                //            LEFT join MaSizeFillet s2 on bt.MaSizePhu = s2.Ma
                //    ) p
                //    left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
                //    left join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
                //    left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma  and p.MaLoaiCa = tp.MaCa
                //    LEFT join MaSizeFillet s on p.MaSize = s.Ma
                //SELECT
                //    @columnHeadersSelected = ISNULL(@columnHeadersSelected + ',', '') + 'Cast( ISNULL(' + QUOTENAME(p.Ten) + ',0) as DECIMAL(18,2)) as ' + QUOTENAME(REPLACE(p.Ten, '.', ','))
                //from
                //    (
                //        select
                //            distinct p.TP as Ten
                //        from
                //            #phieucan p
                //        where
                //            p.TrongLuong > 0
                //    ) p
                //SELECT
                //    @columnHeaders = COALESCE (
                //        @columnHeaders + ', [' + p.TP + ']',
                //        '[' + p.TP + ']'
                //    )
                //FROM
                //    #phieucan p
                //where
                //    p.TrongLuong > 0
                //GROUP BY
                //    p.TP
                //    /* GRAND TOTAL COLUMN */
                //SELECT
                //    @GrandTotalCol = COALESCE (
                //        @GrandTotalCol + 'ISNULL([' + p.TP + '],0) + ',
                //        'ISNULL([' + p.TP + '],0) + '
                //    )
                //FROM
                //    #phieucan p
                //where
                //    p.TrongLuong > 0
                //GROUP BY
                //    p.TP
                //SET
                //    @GrandTotalCol = LEFT (@GrandTotalCol, LEN (@GrandTotalCol) -1)
                //    /* GRAND TOTAL ROW */
                //SELECT
                //    @GrandTotalRow = COALESCE(
                //        @GrandTotalRow + ',ISNULL(SUM([' + p.TP + ']),0)',
                //        'ISNULL(SUM([' + p.TP + ']),0)'
                //    )
                //FROM
                //    #phieucan p
                //where
                //    p.TrongLuong > 0
                //GROUP BY
                //    p.TP
                //    /* MAIN QUERY */
                //SET
                //    @FinalQuery = N' SELECT [Ngay],
                //    [Mã Nhân Viên],[MaHoSo],[Tên Nhân Viên],[Nhom],[Lô],[Size],[Xuong],' + @columnHeadersSelected + ',
                //    (' + @GrandTotalCol + ') AS [Tong]
                //FROM
                //    (
                //     select * from #phieucan
                //    ) A PIVOT (
                //       Sum(TrongLuong) FOR TP IN (' + @columnHeaders + ')
                //    ) B';

                //EXEC sp_executesql @FinalQuery,
                //@ParmDefinition,
                //@ngay,
                //@fromdate,
                //@xuongId
                //";
                var query = $@"DECLARE @ParmDefinition NVARCHAR(500);

DECLARE @columnHeaders NVARCHAR (MAX);

DECLARE @columnHeadersSelected NVARCHAR(Max);

DECLARE @GrandTotalCol NVARCHAR (MAX);

DECLARE @GrandTotalRow NVARCHAR(MAX);

DECLARE @FinalQuery NVARCHAR (MAX);

SET
    @ParmDefinition = N'@ngay Date,@fromdate as Date, @xuongId as varchar(50)';

IF OBJECT_ID(N'tempdb..#phieucan') IS NOT NULL BEGIN DROP TABLE #phieucan
END
Select
    p.Ngay,
    p.MaNhanVien as MaNhanVien,
    n.MaHoSo as MaHoSo,
    n.Name as TenNhanVien,
    n.DeptName0 as Nhom,
    isnull(p.LoTheoLine, p.MSL) as Lo,
    la.Ten as LoaiCaName,
    isnull(p.ThanhPhamName, tp.Ten) as ThanhPhamName,
    ISNULL(p.SizeName, s.Ten) as [Size],
    p.TrongLuong as [TrongLuong],
    p.MaXuongSanXuat as [Xuong] into #phieucan
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
                            p.Ngay <= @ngay
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
    )p
    left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
    left join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
    left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma  and p.MaLoaiCa = tp.MaCa
    LEFT join MaSizeFillet s on p.MaSize = s.Ma
SELECT
    @columnHeadersSelected = ISNULL(@columnHeadersSelected + ',', '') + 'Cast( ISNULL(' + QUOTENAME(p.Ten) + ',0) as DECIMAL(18,2)) as ' + QUOTENAME(REPLACE(p.Ten, '.', ','))
from
    (
        select
            distinct p.TP as Ten
        from
            #phieucan p
        where
            p.TrongLuong > 0
    ) p
SELECT
    @columnHeaders = COALESCE (
        @columnHeaders + ', [' + p.TP + ']',
        '[' + p.TP + ']'
    )
FROM
    #phieucan p
where
    p.TrongLuong > 0
GROUP BY
    p.TP
    /* GRAND TOTAL COLUMN */
SELECT
    @GrandTotalCol = COALESCE (
        @GrandTotalCol + 'ISNULL([' + p.TP + '],0) + ',
        'ISNULL([' + p.TP + '],0) + '
    )
FROM
    #phieucan p
where
    p.TrongLuong > 0
GROUP BY
    p.TP
SET
    @GrandTotalCol = LEFT (@GrandTotalCol, LEN (@GrandTotalCol) -1)
    /* GRAND TOTAL ROW */
SELECT
    @GrandTotalRow = COALESCE(
        @GrandTotalRow + ',ISNULL(SUM([' + p.TP + ']),0)',
        'ISNULL(SUM([' + p.TP + ']),0)'
    )
FROM
    #phieucan p
where
    p.TrongLuong > 0
GROUP BY
    p.TP
    /* MAIN QUERY */
SET
    @FinalQuery = N' SELECT [Ngay],
    [Mã Nhân Viên],[MaHoSo],[Tên Nhân Viên],[Nhom],[Lô],[Size],[Xuong],' + @columnHeadersSelected + ',
    (' + @GrandTotalCol + ') AS [Tong]
FROM
    (
     select * from #phieucan
    )as A PIVOT (
       Sum(TrongLuong) FOR TP IN (' + @columnHeaders + ')
    )as B';

EXEC sp_executesql @FinalQuery,
@ParmDefinition,
@ngay,
@fromdate,
@xuongId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { ngay = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #region Chắt thêm để xuất báo cáo Nang suất fillet Hai Nắm

        //        public List<T> GetTongHopNhanViens<T>(
        //            DateTime fromDate,
        //            DateTime ngay,
        //            string xuongId
        //            )
        //        {
        //            try
        //            {
        //                var query = @"DECLARE @ParmDefinition NVARCHAR(500);

        //DECLARE @columnHeaders NVARCHAR (MAX);

        //DECLARE @columnHeadersSelected NVARCHAR(Max);

        //DECLARE @GrandTotalCol NVARCHAR (MAX);

        //DECLARE @GrandTotalRow NVARCHAR(MAX);

        //DECLARE @FinalQuery NVARCHAR (MAX);

        //SET
        //    @ParmDefinition = N'@ngay Date, @xuongId as varchar(50)';

        //IF OBJECT_ID(N'tempdb..#phieucan') IS NOT NULL BEGIN DROP TABLE #phieucan
        //END
        //Select
        //    p.MaNhanVien as [MaNhanVien],
        //    n.MaHoSo as [MaHoSo],
        //    n.Name as [NhanVienName],
        //    isnull(p.LoTheoLine, p.MSL) as [Lo],
        //    la.Ten as [LoaiCa],
        //    isnull(p.ThanhPhamName, tp.Ten) as [TP],
        //    ISNULL(p.SizeName, s.Ten) as [Size],
        //    p.TrongLuong as [TrongLuong],
        //    p.MaMayTinhCan,
        //    p.MaXuongSanXuat as [Xuong] into #phieucan
        //from
        //    (
        //        Select
        //            p.*,
        //           case
        //        when tp2.IsNotSetByTime = 1 then s2.Ma
        //        else bt.MaSize
        //    end as MaSizeBoTri,
        //    case
        //        when tp2.IsNotSetByTime = 1 then s2.Ten
        //        else s.Ten
        //    end as SizeName,
        //            case when tp2.IsNotSetByTime =1 then NULL else bt.MaThanhPham end as MaThanhPhamBoTri,
        //            case when tp2.IsNotSetByTime =1 then NULL else  tp.Ten end  as ThanhPhamName
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
        //                            and nl.Ngay = @ngay
        //                            and p.MaNhanVien = nl.MaNhanVien
        //                            and nl.Gio = (
        //                                Select
        //                                    MAX(nlc.Gio)
        //                                from
        //                                    NhanVienTheoLine nlc
        //                                where
        //                                    nlc.Ngay = @ngay
        //                                    and nlc.MaNhanVien = p.MaNhanVien
        //                                    and nlc.Gio <= p.ThoiGianCan
        //                            )
        //                            left join XiNghiep xn on xn.Ma = p.MaXuongSanXuat
        //                            left join LineFilletv2 ln on nl.MaLine = ln.Ma
        //                            left join ViTriFillet vt on nl.MaViTri = vt.Ma
        //                        where
        //                            p.Ngay <= @ngay and
        //							p.Ngay >= @fromdate
        //                            and p.MaXuongSanXuat = @xuongId
        //                    ) p
        //                    left JOIN LoTheoLine ll ON p.MaLine = ll.MaLine
        //                    and p.Ngay = ll.Ngay
        //                    and p.CodeId = ll.CodeId
        //                    and ll.Ngay = @ngay
        //                    and ll.Gio = (
        //                        Select
        //                            MAX(llc.Gio)
        //                        from
        //                            LoTheoLine llc
        //                        where
        //                            llc.MaLine = p.MaLine
        //                            and llc.CodeId = p.CodeId
        //                            and llc.Ngay = @ngay
        //                            and llc.Gio <= p.ThoiGianCan
        //                    )
        //            ) p
        //            left JOIN BoTriLoSizeThanhPham bt ON p.LoTheoLine = bt.MaLo
        //            and p.CodeId = bt.CodeId
        //            and p.MaViTri = bt.MaViTri
        //            and bt.Ngay = @ngay
        //            and bt.Gio = (
        //                Select
        //                    MAX(btc.Gio)
        //                from
        //                    BoTriLoSizeThanhPham btc
        //                where
        //                    btc.MaLo = p.LoTheoLine
        //                    and btc.MaViTri = p.MaViTri
        //                    and btc.CodeId = p.CodeId
        //                    and btc.Ngay = @ngay
        //                    and btc.Gio <= p.ThoiGianCan
        //            )
        //            left join MaThanhPhamFillet tp on bt.MaThanhPham = tp.Ma
        //            left join MaThanhPhamFillet tp2 on p.MaLoaiThanhPham = tp2.Ma
        //            LEFT join MaSizeFillet s on bt.MaSize = s.Ma
        //LEFT join MaSizeFillet s2 on bt.MaSize = s2.Ma
        //    ) p
        //    left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
        //    left join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
        //    left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma
        //    LEFT join MaSizeFillet s on p.MaSize = s.Ma
        //SELECT
        //    @columnHeadersSelected = ISNULL(@columnHeadersSelected + ',', '') + 'Cast( ISNULL(' + QUOTENAME(p.Ten) + ',0) as DECIMAL(18,2)) as ' + QUOTENAME(REPLACE(p.Ten, '.', ','))
        //from
        //    (
        //        select
        //            distinct p.TP as Ten
        //        from
        //            #phieucan p
        //        where
        //            p.TrongLuong > 0
        //    ) p
        //SELECT
        //    @columnHeaders = COALESCE (
        //        @columnHeaders + ', [' + p.TP + ']',
        //        '[' + p.TP + ']'
        //    )
        //FROM
        //    #phieucan p
        //where
        //    p.TrongLuong > 0
        //GROUP BY
        //    p.TP
        //    /* GRAND TOTAL COLUMN */
        //SELECT
        //    @GrandTotalCol = COALESCE (
        //        @GrandTotalCol + 'ISNULL([' + p.TP + '],0) + ',
        //        'ISNULL([' + p.TP + '],0) + '
        //    )
        //FROM
        //    #phieucan p
        //where
        //    p.TrongLuong > 0
        //GROUP BY
        //    p.TP
        //SET
        //    @GrandTotalCol = LEFT (@GrandTotalCol, LEN (@GrandTotalCol) -1)
        //    /* GRAND TOTAL ROW */
        //SELECT
        //    @GrandTotalRow = COALESCE(
        //        @GrandTotalRow + ',ISNULL(SUM([' + p.TP + ']),0)',
        //        'ISNULL(SUM([' + p.TP + ']),0)'
        //    )
        //FROM
        //    #phieucan p
        //where
        //    p.TrongLuong > 0
        //GROUP BY
        //    p.TP
        //    /* MAIN QUERY */
        //SET
        //    @FinalQuery = N' SELECT
        //    [MaNhanVien],[MaHoSo],[NhanVienName],[Lo],[Size],[Xuong],' + @columnHeadersSelected + ',
        //    (' + @GrandTotalCol + ') AS [Tong]
        //FROM
        //    (
        //     select * from #phieucan
        //    ) A PIVOT (
        //       Sum(TrongLuong) FOR TP IN (' + @columnHeaders + ')
        //    ) B';

        //EXEC sp_executesql @FinalQuery,
        //@ParmDefinition,
        //@ngay,
        //@xuongId";
        //                using (var connection = new SqlConnection(ConnectionString))
        //                {
        //                    connection.Open();
        //                    var items = connection.QueryAsync<T>(
        //                        query,
        //                        new { fromDate, ngay, xuongId })
        //                        .Result
        //                        .ToList();
        //                    return items;
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                throw;
        //            }
        //        }        
        public List<T> GetTongHopNhanViens<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
        )
        {
            try
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
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetTongHopNhanViens2<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
        )
        {
            try
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
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        public int Delete<T>(List<T> items)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(qrDelete, items);
                return rows;
            }
            catch (Exception)
            {
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

        public int Insert<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrInsert, item);
            return rows;
        }
        public int Insert<T>(List<T> items)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(qrInsert, items);
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int InsertFillet<T>(List<T> items)
        {
            try
            {
                var query = @"INSERT INTO [dbo].[PhieuCanFillet]
           ([Ngay]
           ,[CaLamViec]
           ,[MaNhanVien]
           ,[SuDung]
           ,[MaSanPham]
           ,[TenSanPham]
           ,[TrongLuong]
           ,[SoRo]
           ,[_Status]
           ,[CreatedAt])
     VALUES
           (@Ngay 
           ,@CaLamViec 
           ,@MaNhanVien 
           ,@SuDung 
           ,@MaSanPham 
           ,@TenSanPham 
           ,@TrongLuong 
           ,@SoRo 
           ,@Status 
           ,@CreatedAt )";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(query, items);
                return rows;
            }
            catch (Exception)
            {
                throw;
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
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(qrUpdate, items);
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
