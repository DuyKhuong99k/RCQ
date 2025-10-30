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
           ,[GhiChu],[MaNhanVienPhucVu],[LoaiCan],[TrongLuongTare],[Id])
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
           ,@GhiChu,@MaNhanVienPhucVu,@LoaiCan,@TrongLuongTare,@Id)";

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

        public PhieuCanTPFillet(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;

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
                    new { toDate = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
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
        public Tuple<int, decimal> GetSoRoTongTrongLuongByNhanVienIdandThanhPhamId(DateTime dateTime, string nhanVienId,string thanhPhamId)
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
    and MaNhanVien = @nhanVienId and MaLoaiThanhPham = @thanhPhamId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var row = connection.Query<Tuple<int, decimal>>(query, new { ngay = dateTime.Date, nhanVienId , thanhPhamId })
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
     //           var query = @"Select
     //       p.Ngay,
     //       p.MaNhanVien,
     //       n.MaHoSo,
     //       n.Name as NhanVienName,
     //       n.DeptName0 as Nhom,
     //       p.MSL,
     //       p.MaLoaiCa,
     //       la.Ten as LoaiCaName,
     //       p.MaSize,
     //       s.Ten as SizeName,
     //       p.MaLoaiThanhPham,
     //       tp.Ten as ThanhPhamName,
     //       tp.BravoId as MaSanPham,
		
     //       p.MaMau,
     //       ma.Ten as MauName,
     //       n.LoaiSanLuong,
     //       Sum(p.TrongLuong) as TrongLuong,
            
            
            
     //       Count(*) as SoRo,
     //       p.MaXuongSanXuat
     //   from
     //       PhieuCanTPFillet p
     //       LEFT JOIN (
     //           Select
     //               tp1.MSL,
     //               tp1.MaLoaiCa,
     //               tp1.MaSize,
     //               tp1.MaMau,
     //               tp1.MaLoaiThanhPham,
     //               tp1.Ngay,
     //               tp1.MaXuongSanXuat
     //           from
     //               (
     //                   Select
     //                       distinct p.MSL,
     //                       p.MaLoaiCa,
     //                       p.MaSize,
     //                       p.MaMau,
     //                       p.MaLoaiThanhPham,
                          
                          
     //                       p.Ngay,
     //                       p.MaXuongSanXuat
     //                   from
     //                       PhieuCanTPFillet p,
     //                       MaThanhPhamFillet tp
     //                   where
     //                       p.Ngay <= @toDate
     //                       and p.Ngay >= @fromDate
     //                       and p.MaXuongSanXuat = @xuongId
     //                       and p.MaLoaiThanhPham = tp.Ma
     //                       and p.MaLoaiCa = tp.MaCa
     //               ) tp1
     //               LEFT JOIN (
     //                   Select
     //                       MaLo,
     //                       MaLoaiCa,
     //                       MaSize,
     //                       MaMau,
     //                       MaThanhPham,
     //                       CaTra,
     //                       DinhMuc,
     //                       Ngay,
     //                       MaXuong
     //                   from
     //                       (
     //                           Select
     //                               d.*,
     //                               ROW_NUMBER() OVER (
     //                                   PARTITION BY MaLo,
     //                                   MaLoaiCa,
     //                                   MaMau,
     //                                   MaSize,
     //                                   MaThanhPham,
     //                                   CaTra,
     //                                   Ngay
     //                                   ORDER BY
     //                                       Gio DESC
     //                               ) AS [ROW NUMBER]
     //                           from
     //                               DinhMucFillet d
     //                           where
     //                               d.Ngay <= @toDate
     //                               and d.Ngay >= @fromDate
     //                               And MaXuong = @xuongId
     //                       ) dm
     //                   Where
     //                       dm.[ROW NUMBER] = 1
     //               ) tp2 on tp1.MSL = tp2.MaLo
     //               and tp1.MaLoaiCa = tp2.MaLoaiCa
     //               and tp1.MaSize = tp2.MaSize
     //               and tp1.MaMau = tp2.MaMau
     //               and tp1.MaLoaiThanhPham = tp2.MaThanhPham
                   
     //               and tp1.Ngay = tp2.Ngay
     //               and tp1.MaXuongSanXuat = tp2.MaXuong
     //       ) DinhMuc on p.MaXuongSanXuat = DinhMuc.MaXuongSanXuat
     //       and p.MSL = DinhMuc.MSL
     //       and p.MaLoaiCa = DinhMuc.MaLoaiCa
     //       and p.MaSize = DinhMuc.MaSize
     //       and p.MaMau = DinhMuc.MaMau
     //       and p.MaLoaiThanhPham = DinhMuc.MaLoaiThanhPham
            
     //       and p.Ngay = DinhMuc.Ngay
     //       LEFT JOIN MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma  and p.MaLoaiCa = tp.MaCa
     //       LEFT Join MaSizeFillet s on p.MaSize = s.Ma
     //       LEFT Join MaLoaiCaFillet la on p.MaLoaiCa = la.Ma
     //       LEFT Join MaMauFillet ma on p.MaMau = ma.Ma
     //       LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
     //   where
     //       p.Ngay <= @toDate
     //       and p.Ngay >= @fromDate
     //       and p.MaXuongSanXuat = @xuongId
     //   group by
     //       p.MaNhanVien,
     //       p.MSL,
     //       p.MaLoaiCa,
     //       p.MaSize,
     //       p.MaLoaiThanhPham,
     //       p.MaMau,
           
            
     //       p.Ngay,
            
     //       n.MaHoSo,
     //       n.Name,
     //       la.Ten,
     //       tp.Ten,
     //       tp.BravoId,
		
     //       ma.Ten,
     //       s.Ten,
     //       n.DeptName0,
     //       n.LoaiSanLuong,
     //       p.MaXuongSanXuat
     //";
     //thêm check in out để lấy thoi gian vao ra
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
            p.MaXuongSanXuat,
			MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianVao,
			MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay) as ThoiGianRa,
			DATEDIFF(hour, MIN(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay), MAX(c.ThoiGian) OVER(PARTITION BY n.MaChamCong, p.Ngay)) as TongThoiGian
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
			LEFT JOIN CheckInOut c on n.MaChamCong = c.MaChamCong AND c.ThoiGian = p.Ngay AND c.ThoiGian >= @fromDate AND c.ThoiGian <= @toDate  
        where
            p.Ngay <= @toDate
            and p.Ngay >= @fromDate
            and p.MaXuongSanXuat = @xuongId
            and p.TrongLuong > 0
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
            p.MaXuongSanXuat,
			n.MaChamCong,
			c.ThoiGian
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
        public T? Get<T>(string id)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>("select * from PhieuCanTPFillet where Id = @id", new {id}).FirstOrDefault();
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
        #region Xẻ Bướm
        public List<T> GetChiTietBTPXeBuoms<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
//                var query = @"select 
//p.Ngay,
//cast(p.ThoiGianCan as datetime) as Gio,
//p.MaMayTinhCan,
//p.MaUserCan,
//p.MaXuongSanXuat,
//x.Ten as XuongName,
//p.MSL as MaLo,
//p.MaLoaiCa,
//lc.Ten as LoaiCaName,
//p.MaLoaiThanhPham,
//tp.Ten as ThanhPhamName,
//p.MaSize,
//s.Ten as SizeName,
//p.MaMau,
//m.Ten as MauName,
//p.MaTheTu,
//p.TrongLuong,
//p.SuDung,
//p.GhiChu,
//p.LoaiCan,
//p.TrongLuongTare
//from PhieuCanTPFillet p
//left join XiNghiep x on p.MaXuongSanXuat = x.Ma
//left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
//left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma
//left join MaSizeFillet s on p.MaSize = s.Ma
//left join MaMauFillet m on p.MaMau = m.Ma
//where 
//p.Ngay <= @dateTime
//and p.Ngay >= @fromDate
//and p.MaXuongSanXuat = @xuongId
//and p.TrongLuong > 0 
//and p.MaNhanVien is null
//";
var query = @"select 
    p.Ngay,
	p.MaNhanVien,
	nv.MaHoSo,
	nv.Name as NhanVienName,
    cast(p.ThoiGianCan as datetime) as Gio,
    p.MaMayTinhCan,
    p.MaUserCan,
    p.MaXuongSanXuat,
    x.Ten as XuongName,
    p.MSL as MaLo,
    p.MaLoaiCa,
    lc.Ten as LoaiCaName,
    p.MaLoaiThanhPham,
    tp.Ten as ThanhPhamName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaMau,
    m.Ten as MauName,
    p.MaTheTu,
    case when tp.IsDat = 1 then -1 * p.TrongLuong else p.TrongLuong end as TrongLuong,
    p.SuDung,
    p.GhiChu,
    p.LoaiCan,
    p.TrongLuongTare
from PhieuCanTPFillet p
left join XiNghiep x on p.MaXuongSanXuat = x.Ma
left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma and tp.IsNguyenlieuxebuom = 1
left join MaSizeFillet s on p.MaSize = s.Ma
left join MaMauFillet m on p.MaMau = m.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
where 
    p.Ngay <= @dateTime
    and p.Ngay >= @fromDate
    and p.MaXuongSanXuat = @xuongId
    and p.TrongLuong > 0
    and tp.Ma is not null
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { dateTime = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamBTPXeBuoms<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
//                var query = @"select 

//p.MaXuongSanXuat,
//x.Ten as XuongName,
//p.MaLoaiThanhPham,
//tp.Ten as ThanhPhamName,
//p.MaSize,
//s.Ten as SizeName,
//p.MaMau,
//m.Ten as MauName,
//Sum(p.TrongLuong)as TrongLuong,
//count(*) as SoRo
//from PhieuCanTPFillet p
//left join XiNghiep x on p.MaXuongSanXuat = x.Ma
//left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
//left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma
//left join MaSizeFillet s on p.MaSize = s.Ma
//left join MaMauFillet m on p.MaMau = m.Ma
//where 
//p.Ngay <= @dateTime
//and p.Ngay >= @fromDate
//and p.MaXuongSanXuat = @xuongId
//and p.TrongLuong > 0 
//and p.MaNhanVien is null
//group by
//p.MaXuongSanXuat,
//x.Ten,
//p.MaLoaiThanhPham,
//tp.Ten,
//p.MaSize,
//s.Ten,
//p.MaMau,
//m.Ten
//";
var query = @"select 
p.MaXuongSanXuat,
x.Ten as XuongName,
p.MaLoaiThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
case when tp.IsDat = 1 then -1 * Sum(p.TrongLuong) else Sum(p.TrongLuong) end as TrongLuong,
count(*) as SoRo
from PhieuCanTPFillet p
left join XiNghiep x on p.MaXuongSanXuat = x.Ma
left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma and tp.IsNguyenlieuxebuom = 1
left join MaSizeFillet s on p.MaSize = s.Ma
left join MaMauFillet m on p.MaMau = m.Ma
where 
p.Ngay <= @dateTime
and p.Ngay >= @fromDate
and p.MaXuongSanXuat = @xuongId
and p.TrongLuong > 0 
and tp.Ma is not null
group by
p.MaXuongSanXuat,
x.Ten,
p.MaLoaiThanhPham,
tp.Ten,
p.MaSize,
s.Ten,
p.MaMau,
m.Ten,
tp.IsDat,tp.IsNguyenLieuXeBuom
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { dateTime = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Tổng Hộp BTP Lô Loại Thành Phẩm Size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromDate"></param>
        /// <param name="dateTime"></param>
        /// <param name="xuongId"></param>
        /// <returns></returns>
        public List<T> GetTongHopLTPSBTPXeBuoms<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
//                var query = @"select
//p.MSL,
//p.MaXuongSanXuat,
//x.Ten as XuongName,
//p.MaLoaiThanhPham,
//tp.Ten as ThanhPhamName,
//p.MaSize,
//s.Ten as SizeName,
//p.MaMau,
//m.Ten as MauName,
//Sum(p.TrongLuong)as TrongLuong,
//count(*) as SoRo
//from PhieuCanTPFillet p
//left join XiNghiep x on p.MaXuongSanXuat = x.Ma
//left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
//left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma
//left join MaSizeFillet s on p.MaSize = s.Ma
//left join MaMauFillet m on p.MaMau = m.Ma
//where 
//p.Ngay <= @dateTime
//and p.Ngay >= @fromDate
//and p.MaXuongSanXuat = @xuongId
//and p.TrongLuong > 0 
//and p.MaNhanVien is null
//group by
//p.MaXuongSanXuat,
//x.Ten,
//p.MaLoaiThanhPham,
//tp.Ten,
//p.MaSize,
//s.Ten,
//p.MaMau,
//m.Ten,
//p.MSL
//";
var query = @"select
p.MSL,
p.MaXuongSanXuat,
x.Ten as XuongName,
p.MaLoaiThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
case when tp.IsDat = 1 then -1 * Sum(p.TrongLuong) else Sum(p.TrongLuong) end as TrongLuong,
count(*) as SoRo
from PhieuCanTPFillet p
left join XiNghiep x on p.MaXuongSanXuat = x.Ma
left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma and tp.IsNguyenlieuxebuom = 1
left join MaSizeFillet s on p.MaSize = s.Ma
left join MaMauFillet m on p.MaMau = m.Ma
where 
p.Ngay <= @dateTime
and p.Ngay >= @fromDate
and p.MaXuongSanXuat = @xuongId
and p.TrongLuong > 0 
and tp.Ma is not null
group by
p.MaXuongSanXuat,
x.Ten,
p.MaLoaiThanhPham,
tp.Ten,
p.MaSize,
s.Ten,
p.MaMau,
m.Ten,
p.MSL,
tp.IsDat,tp.IsNguyenLieuXeBuom
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { dateTime = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopNhanVienBTPXeBuoms<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {

var query = @"WITH CheckInOutData AS (
    SELECT 
        c.MaChamCong,
        MIN(c.ThoiGian) AS ThoiGianVao,
        MAX(c.ThoiGian) AS ThoiGianRa
    FROM CheckInOut c 
    WHERE c.ThoiGian >= @fromDate AND c.ThoiGian <= @dateTime
    GROUP BY c.MaChamCong
)

select 
p.MaXuongSanXuat,
x.Ten as XuongName,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as NhanVienName,
p.MaLoaiThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
case when tp.IsDat = 1 then -1 * Sum(p.TrongLuong) else Sum(p.TrongLuong) end as TrongLuong,
count(*) as SoRo,
ISNULL(c.ThoiGianVao, '1900-01-01') AS ThoiGianVao,
ISNULL(c.ThoiGianRa, '1900-01-01') AS ThoiGianRa,
DATEDIFF(hour, ISNULL(c.ThoiGianVao, '1900-01-01'), ISNULL(c.ThoiGianRa, '1900-01-01')) AS TongThoiGian
from PhieuCanTPFillet p
left join XiNghiep x on p.MaXuongSanXuat = x.Ma
left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma and tp.IsNguyenlieuxebuom = 1
left join MaSizeFillet s on p.MaSize = s.Ma
left join MaMauFillet m on p.MaMau = m.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
LEFT JOIN CheckInOutData c ON c.MaChamCong = nv.MaChamCong
where 
p.Ngay <= @dateTime
and p.Ngay >= @fromDate
and p.MaXuongSanXuat = @xuongId
and p.TrongLuong > 0 
and tp.Ma is not null
group by
p.MaXuongSanXuat,
x.Ten,
p.MaLoaiThanhPham,
tp.Ten,
p.MaSize,
s.Ten,
p.MaMau,
m.Ten,
tp.IsDat,tp.IsNguyenLieuXeBuom,
p.MaNhanVien, nv.MaHoSo, nv.Name,
    c.ThoiGianVao,
    c.ThoiGianRa
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { dateTime = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetChiTietTPXeBuoms<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
//                var query = @"select 
//p.Ngay,
//cast(p.ThoiGianCan as datetime) as Gio,
//p.MaMayTinhCan,
//p.MaUserCan,
//p.MaXuongSanXuat,
//x.Ten as XuongName,
//p.MSL as MaLo,
//p.MaLoaiCa,
//lc.Ten as LoaiCaName,
//p.MaLoaiThanhPham,
//tp.Ten as ThanhPhamName,
//p.MaSize,
//s.Ten as SizeName,
//p.MaMau,
//m.Ten as MauName,
//p.MaTheTu,
//p.TrongLuong,
//p.SuDung,
//p.GhiChu,
//p.MaNhanVienPhucVu,
//nvpv.Name as NhanVienPhucVuName,
//p.MaNhanVien,
//nv.Name as NhanVienName,
//nv.MaHoSo,
//p.LoaiCan,
//p.TrongLuongTare
//from PhieuCanTPFillet p
//left join XiNghiep x on p.MaXuongSanXuat = x.Ma
//left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
//left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma
//left join MaSizeFillet s on p.MaSize = s.Ma
//left join MaMauFillet m on p.MaMau = m.Ma
//left join NhanVienDaiThanh nvpv on p.MaNhanVienPhucVu = nvpv.MaNhanVien
//left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
//where 
//p.Ngay <= @dateTime
//and p.Ngay >= @fromDate
//and p.MaXuongSanXuat = @xuongId
//and p.TrongLuong > 0 
//and p.MaNhanVien is not null
//";
var query = @"select 
p.Ngay,
cast(p.ThoiGianCan as datetime) as Gio,
p.MaMayTinhCan,
p.MaUserCan,
p.MaXuongSanXuat,
x.Ten as XuongName,
p.MSL as MaLo,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaLoaiThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
p.MaTheTu,
case when tp.IsDat = 1 then -1 * p.TrongLuong else p.TrongLuong end as TrongLuong,
p.SuDung,
p.GhiChu,
p.MaNhanVienPhucVu,
nvpv.Name as NhanVienPhucVuName,
p.MaNhanVien,
nv.Name as NhanVienName,
nv.MaHoSo,
p.LoaiCan,
p.TrongLuongTare
from PhieuCanTPFillet p
left join XiNghiep x on p.MaXuongSanXuat = x.Ma
left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma and tp.IsNguyenlieuxebuom = 0
left join MaSizeFillet s on p.MaSize = s.Ma
left join MaMauFillet m on p.MaMau = m.Ma
left join NhanVienDaiThanh nvpv on p.MaNhanVienPhucVu = nvpv.MaNhanVien
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
where 
p.Ngay <= @dateTime
and p.Ngay >= @fromDate
and p.MaXuongSanXuat = @xuongId
and p.TrongLuong > 0 
and tp.Ma is not null
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { dateTime = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopThanhPhamTPXeBuoms<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
//                var query = @"select 

//p.MaXuongSanXuat,
//x.Ten as XuongName,
//p.MaLoaiThanhPham,
//tp.Ten as ThanhPhamName,
//p.MaSize,
//s.Ten as SizeName,
//p.MaMau,
//m.Ten as MauName,
//Sum(p.TrongLuong)as TrongLuong,
//COUNT(*) AS SoRo
//from PhieuCanTPFillet p
//left join XiNghiep x on p.MaXuongSanXuat = x.Ma
//left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
//left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma
//left join MaSizeFillet s on p.MaSize = s.Ma
//left join MaMauFillet m on p.MaMau = m.Ma
//left join NhanVienDaiThanh nvpv on p.MaNhanVienPhucVu = nvpv.MaNhanVien
//where 
//p.Ngay <= @dateTime
//and p.Ngay >= @fromDate
//and p.MaXuongSanXuat = @xuongId
//and p.TrongLuong > 0 
//and p.MaNhanVien is not null
//group by
//p.MaXuongSanXuat,
//x.Ten,
//p.MaLoaiThanhPham,
//tp.Ten,
//p.MaSize,
//s.Ten,
//p.MaMau,
//m.Ten
//";
var query = @"select 
p.Ngay,
p.MaXuongSanXuat,
x.Ten as XuongName,
p.MaLoaiThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
case when tp.IsDat = 1 then -1 * Sum(p.TrongLuong) else Sum(p.TrongLuong) end as TrongLuong,
COUNT(*) AS SoRo
from PhieuCanTPFillet p
left join XiNghiep x on p.MaXuongSanXuat = x.Ma
left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma and tp.IsNguyenlieuxebuom = 0
left join MaSizeFillet s on p.MaSize = s.Ma
left join MaMauFillet m on p.MaMau = m.Ma
left join NhanVienDaiThanh nvpv on p.MaNhanVienPhucVu = nvpv.MaNhanVien
where 
p.Ngay <= @dateTime
and p.Ngay >= @fromDate
and p.MaXuongSanXuat = @xuongId
and p.TrongLuong > 0 
and tp.Ma is not null
group by
p.MaXuongSanXuat,
x.Ten,
p.MaLoaiThanhPham,
tp.Ten,
p.MaSize,
s.Ten,
p.MaMau,
m.Ten,
IsDat,
p.Ngay
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { dateTime = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

         public List<T> GetTongHopThanhPhamTPXeBuoms2<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
var query = @"select 
p.Ngay,

p.MaLoaiThanhPham,
tp.Ten as ThanhPhamName,


case when tp.IsDat = 1 then -1 * Sum(p.TrongLuong) else Sum(p.TrongLuong) end as TrongLuong,
COUNT(*) AS SoRo
from PhieuCanTPFillet p

left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma and tp.IsNguyenlieuxebuom = 0

where 
p.Ngay <= @dateTime
and p.Ngay >= @fromDate
and p.MaXuongSanXuat = @xuongId
and p.TrongLuong > 0 
and tp.Ma is not null
group by
p.MaLoaiThanhPham,
tp.Ten,
p.Ngay,
tp.IsDat
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { dateTime = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }





        /// <summary>
        /// Tổng Hộp TP Lô Loại Thành Phẩm Size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromDate"></param>
        /// <param name="dateTime"></param>
        /// <param name="xuongId"></param>
        /// <returns></returns>
        public List<T> GetTongHopLTPSTPXeBuoms<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
//                var query = @"select
//p.MSL,
//p.MaXuongSanXuat,
//x.Ten as XuongName,
//p.MaLoaiThanhPham,
//tp.Ten as ThanhPhamName,
//p.MaSize,
//s.Ten as SizeName,
//p.MaMau,
//m.Ten as MauName,
//Sum(p.TrongLuong)as TrongLuong,
//count(*) as SoRo
//from PhieuCanTPFillet p
//left join XiNghiep x on p.MaXuongSanXuat = x.Ma
//left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
//left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma
//left join MaSizeFillet s on p.MaSize = s.Ma
//left join MaMauFillet m on p.MaMau = m.Ma
//left join NhanVienDaiThanh nvpv on p.MaNhanVienPhucVu = nvpv.MaNhanVien
//where 
//p.Ngay <= @dateTime
//and p.Ngay >= @fromDate
//and p.MaXuongSanXuat = @xuongId
//and p.TrongLuong > 0 
//and p.MaNhanVien is not null
//group by
//p.MaXuongSanXuat,
//x.Ten,
//p.MaLoaiThanhPham,
//tp.Ten,
//p.MaSize,
//s.Ten,
//p.MaMau,
//m.Ten,
//p.MSL
//";
var query = @"select
p.MSL,
p.MaXuongSanXuat,
x.Ten as XuongName,
p.MaLoaiThanhPham,
tp.Ten as ThanhPhamName,
p.MaSize,
s.Ten as SizeName,
p.MaMau,
m.Ten as MauName,
case when tp.IsDat = 1 then -1 * Sum(p.TrongLuong) else Sum(p.TrongLuong) end as TrongLuong,
count(*) as SoRo
from PhieuCanTPFillet p
left join XiNghiep x on p.MaXuongSanXuat = x.Ma
left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma and tp.IsNguyenlieuxebuom = 0
left join MaSizeFillet s on p.MaSize = s.Ma
left join MaMauFillet m on p.MaMau = m.Ma
left join NhanVienDaiThanh nvpv on p.MaNhanVienPhucVu = nvpv.MaNhanVien
where 
p.Ngay <= @dateTime
and p.Ngay >= @fromDate
and p.MaXuongSanXuat = @xuongId
and p.TrongLuong > 0 
and tp.Ma is not null
group by
p.MaXuongSanXuat,
x.Ten,
p.MaLoaiThanhPham,
tp.Ten,
p.MaSize,
s.Ten,
p.MaMau,
m.Ten,
p.MSL,
IsDat
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { dateTime = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetTongHopNhanVienTPXeBuoms<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
//                var query = @"select 
//p.MaNhanVien,
//nv.Name as NhanVienName,
//nv.MaHoSo,
//p.MaXuongSanXuat,
//x.Ten as XuongName,
//p.MaLoaiThanhPham,
//tp.Ten as ThanhPhamName,
//p.MaSize,
//s.Ten as SizeName,
//p.MaMau,
//m.Ten as MauName,
//Sum(p.TrongLuong)as TrongLuong
//from PhieuCanTPFillet p
//left join XiNghiep x on p.MaXuongSanXuat = x.Ma
//left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
//left join MaThanhPhamFillet tp on p.MaLoaiThanhPham = tp.Ma
//left join MaSizeFillet s on p.MaSize = s.Ma
//left join MaMauFillet m on p.MaMau = m.Ma
//left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
//where 
//p.Ngay <= @dateTime
//and p.Ngay >= @fromDate
//and p.MaXuongSanXuat = @xuongId
//and p.TrongLuong > 0 
//and p.MaNhanVien is not null
//group by
//p.MaXuongSanXuat,
//x.Ten,
//p.MaLoaiThanhPham,
//tp.Ten,
//p.MaSize,
//s.Ten,
//p.MaMau,
//m.Ten,
//p.MaNhanVien,
//nv.Name,
//nv.MaHoSo
//";
//                var query = @";WITH CheckInOutData AS (
//    SELECT 
//        c.MaChamCong,
//        MIN(c.ThoiGian) AS ThoiGianVao,
//        MAX(c.ThoiGian) AS ThoiGianRa
//    FROM CheckInOut c 
//    WHERE c.ThoiGian >= @fromDate AND c.ThoiGian <= @dateTime
//    GROUP BY c.MaChamCong
//)
//SELECT 
//    p.MaNhanVien,
//    nv.Name AS NhanVienName,
//    nv.MaHoSo,
//    p.MaXuongSanXuat,
//    x.Ten AS XuongName,
//    p.MaLoaiThanhPham,
//    tp.Ten AS ThanhPhamName,
//    p.MaSize,
//    s.Ten AS SizeName,
//    p.MaMau,
//    m.Ten AS MauName,
//    SUM(p.TrongLuong) AS TrongLuong,
//    count(*) as SoRo,
//    ISNULL(c.ThoiGianVao, '1900-01-01') AS ThoiGianVao,
//    ISNULL(c.ThoiGianRa, '1900-01-01') AS ThoiGianRa,
//	DATEDIFF(hour, ISNULL(c.ThoiGianVao, '1900-01-01'), ISNULL(c.ThoiGianRa, '1900-01-01')) AS TongThoiGian
//FROM PhieuCanTPFillet p
//LEFT JOIN XiNghiep x ON p.MaXuongSanXuat = x.Ma
//LEFT JOIN MaThanhPhamFillet tp ON p.MaLoaiThanhPham = tp.Ma
//LEFT JOIN MaSizeFillet s ON p.MaSize = s.Ma
//LEFT JOIN MaMauFillet m ON p.MaMau = m.Ma
//LEFT JOIN NhanVienDaiThanh nv ON p.MaNhanVien = nv.MaNhanVien
//LEFT JOIN CheckInOutData c ON c.MaChamCong = nv.MaChamCong
//WHERE 
//    p.Ngay <= @dateTime
//    AND p.Ngay >= @fromDate
//    AND p.MaXuongSanXuat = @xuongId
//    AND p.TrongLuong > 0 
//    AND p.MaNhanVien IS NOT NULL
//GROUP BY 
//    p.MaXuongSanXuat,
//    x.Ten,
//    p.MaLoaiThanhPham,
//    tp.Ten,
//    p.MaSize,
//    s.Ten,
//    p.MaMau,
//    m.Ten,
//    p.MaNhanVien,
//    nv.Name,
//    nv.MaHoSo,
//    c.ThoiGianVao,
//    c.ThoiGianRa
//";
var query = @"WITH CheckInOutData AS (
    SELECT 
        c.MaChamCong,
        MIN(c.ThoiGian) AS ThoiGianVao,
        MAX(c.ThoiGian) AS ThoiGianRa
    FROM CheckInOut c 
    WHERE c.ThoiGian >= @fromDate AND c.ThoiGian <= @dateTime
    GROUP BY c.MaChamCong
)
SELECT 
    p.MaNhanVien,
    nv.Name AS NhanVienName,
    nv.MaHoSo,
    p.MaXuongSanXuat,
    x.Ten AS XuongName,
    p.MaLoaiThanhPham,
    tp.Ten AS ThanhPhamName,
    p.MaSize,
    s.Ten AS SizeName,
    p.MaMau,
    m.Ten AS MauName,
    case when tp.IsDat = 1 then -1 * Sum(p.TrongLuong) else Sum(p.TrongLuong) end as TrongLuong,
    count(*) as SoRo,
    ISNULL(c.ThoiGianVao, '1900-01-01') AS ThoiGianVao,
    ISNULL(c.ThoiGianRa, '1900-01-01') AS ThoiGianRa,
	DATEDIFF(hour, ISNULL(c.ThoiGianVao, '1900-01-01'), ISNULL(c.ThoiGianRa, '1900-01-01')) AS TongThoiGian
FROM PhieuCanTPFillet p
LEFT JOIN XiNghiep x ON p.MaXuongSanXuat = x.Ma
LEFT JOIN MaThanhPhamFillet tp ON p.MaLoaiThanhPham = tp.Ma and tp.IsNguyenlieuxebuom = 0
LEFT JOIN MaSizeFillet s ON p.MaSize = s.Ma
LEFT JOIN MaMauFillet m ON p.MaMau = m.Ma
LEFT JOIN NhanVienDaiThanh nv ON p.MaNhanVien = nv.MaNhanVien
LEFT JOIN CheckInOutData c ON c.MaChamCong = nv.MaChamCong
WHERE 
    p.Ngay <= @dateTime
    AND p.Ngay >= @fromDate
    AND p.MaXuongSanXuat = @xuongId
    AND p.TrongLuong > 0 
    and tp.Ma is not null
GROUP BY 
    p.MaXuongSanXuat,
    x.Ten,
    p.MaLoaiThanhPham,
    tp.Ten,
    p.MaSize,
    s.Ten,
    p.MaMau,
    m.Ten,
    p.MaNhanVien,
    nv.Name,
    nv.MaHoSo,
    c.ThoiGianVao,
    c.ThoiGianRa,
IsDat
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { dateTime = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> GetTongHopDinhMucTPXeBuoms<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
var query = @";WITH NguyenLieuXeBuom AS (
    SELECT 
        p.MaXuongSanXuat,
        x.Ten AS XuongName,
        SUM(CASE WHEN tp.IsDat = 1 THEN -1 * p.TrongLuong ELSE p.TrongLuong END) AS NguyenLieuXeBuom
    FROM PhieuCanTPFillet p
    LEFT JOIN XiNghiep x ON p.MaXuongSanXuat = x.Ma
    LEFT JOIN MaThanhPhamFillet tp ON p.MaLoaiThanhPham = tp.Ma AND tp.IsNguyenlieuxebuom = 1
    WHERE 
         p.Ngay >= @fromDate
 and p.Ngay <= @dateTime
 and p.MaXuongSanXuat = @xuongId
        AND p.TrongLuong > 0
        AND tp.Ma IS NOT NULL and tp.IsNguyenCon = 0 and tp.IsNguyenConNXB = 0
    GROUP BY p.MaXuongSanXuat, x.Ten
),

DatFillet as (
	SELECT 
        p.MaXuong,
		p.MaThanhPham,
		tp.Ten as ThanhPhamName,
        x.Ten AS XuongName,
        SUM(CASE WHEN tp.IsDat = 1 THEN  p.TrongLuong ELSE p.TrongLuong END) AS DatXeBuom
    FROM PhieuCanBTPFilletv2 p
    LEFT JOIN XiNghiep x ON p.MaXuong = x.Ma
    LEFT JOIN MaThanhPhamFillet tp ON p.MaThanhPham = tp.Ma AND tp.IsDat = 1
    WHERE 
		 p.Ngay >= @fromDate
 and p.Ngay <= @dateTime
 and p.MaXuong = @xuongId
        AND p.TrongLuong > 0
        AND tp.Ma IS NOT NULL and tp.IsNguyenCon = 0
    GROUP BY p.MaXuong, x.Ten, p.MaThanhPham, tp.Ten
),

DatNguyenCon as (
	SELECT 
        p.MaXuong,
		p.MaThanhPham,
		tp.Ten as ThanhPhamName,
        x.Ten AS XuongName,
        SUM(CASE WHEN tp.IsDat = 1 THEN  p.TrongLuong ELSE p.TrongLuong END) AS DatNguyenCon
    FROM PhieuCanBTPFilletv2 p
    LEFT JOIN XiNghiep x ON p.MaXuong = x.Ma
    LEFT JOIN MaThanhPhamFillet tp ON p.MaThanhPham = tp.Ma AND tp.IsDat = 1
    WHERE 
		 p.Ngay >= @fromDate
 and p.Ngay <= @dateTime
 and p.MaXuong = @xuongId
        AND p.TrongLuong > 0
        AND tp.Ma IS NOT NULL and tp.IsNguyenCon = 1
    GROUP BY p.MaXuong, x.Ten, p.MaThanhPham, tp.Ten
),

TPXeBuom AS (
    SELECT 
        p.MaXuongSanXuat,
        x.Ten AS XuongName,
        --SUM(CASE WHEN tp.IsDat = 1 THEN -1 * p.TrongLuong ELSE p.TrongLuong END) AS TongTrongBangTPXeBuom,
        SUM(CASE 
            WHEN tp.IsXeBuom = 1 AND tp.IsDat = 0 AND tp.IsNguyenlieuxebuom = 0 AND tp.IsGiaoXepKhuon = 0 and tp.IsNguyenConNXB = 0 THEN p.TrongLuong 
            ELSE 0 
        END) AS TrongLuongTPXeBuom,
        SUM(CASE 
            WHEN tp.IsGiaoXepKhuon = 1 THEN p.TrongLuong 
            ELSE 0 
        END) AS TrongLuongGiaoXepKhuon
    FROM PhieuCanTPFillet p
    LEFT JOIN XiNghiep x ON p.MaXuongSanXuat = x.Ma
    LEFT JOIN MaThanhPhamFillet tp ON p.MaLoaiThanhPham = tp.Ma AND tp.IsNguyenlieuxebuom = 0
    WHERE 
         p.Ngay >= @fromDate
 and p.Ngay <= @dateTime
 and p.MaXuongSanXuat = @xuongId
        AND p.TrongLuong > 0
        AND tp.Ma IS NOT NULL
    GROUP BY p.MaXuongSanXuat, x.Ten
)

SELECT 
    COALESCE(nl.MaXuongSanXuat, df.MaXuong, dnc.MaXuong, tp.MaXuongSanXuat) AS MaXuong,
    COALESCE(nl.XuongName, df.XuongName, dnc.XuongName, tp.XuongName) AS XuongName,

    ISNULL(nl.NguyenLieuXeBuom, 0) AS NguyenLieuXeBuom,
    ISNULL(dnc.DatNguyenCon, 0) AS DatNguyenCon,
    ISNULL(tp.TrongLuongTPXeBuom, 0) AS TrongLuongTPXeBuom,
    ISNULL(tp.TrongLuongGiaoXepKhuon, 0) AS TrongLuongGiaoXepKhuon,
    ISNULL(df.DatXeBuom, 0) AS DatXeBuom,

    -- Định mức
    ROUND(
        ISNULL((1.0 * (ISNULL(nl.NguyenLieuXeBuom, 0) - ISNULL(dnc.DatNguyenCon, 0)) 
        / NULLIF(tp.TrongLuongTPXeBuom, 0)), 0), 4
    ) AS DinhMucMocRuot,

    ROUND(
        ISNULL((1.0 * (ISNULL(tp.TrongLuongTPXeBuom, 0) - ISNULL(df.DatXeBuom, 0)) 
        / NULLIF(tp.TrongLuongGiaoXepKhuon, 0)), 0), 4
    ) AS DinhMucHaoHut,

    ROUND(
        ISNULL(
            (1.0 * (ISNULL(nl.NguyenLieuXeBuom, 0) - ISNULL(dnc.DatNguyenCon, 0)) / NULLIF(tp.TrongLuongTPXeBuom, 0))
          * (1.0 * (ISNULL(tp.TrongLuongTPXeBuom, 0) - ISNULL(df.DatXeBuom, 0)) / NULLIF(tp.TrongLuongGiaoXepKhuon, 0)),
            0
        ), 4
    ) AS DinhMucCongDoan,

    ROUND(
        ISNULL(
            (1.0 * (ISNULL(nl.NguyenLieuXeBuom, 0) - ISNULL(dnc.DatNguyenCon, 0)) / NULLIF(tp.TrongLuongTPXeBuom, 0))
          * (1.0 * (ISNULL(tp.TrongLuongTPXeBuom, 0) - ISNULL(df.DatXeBuom, 0)) / NULLIF(tp.TrongLuongGiaoXepKhuon, 0))
          * 1.006,
            0
        ), 4
    ) AS DinhMucXeBuom

FROM NguyenLieuXeBuom nl
FULL OUTER JOIN DatFillet df ON nl.MaXuongSanXuat = df.MaXuong
FULL OUTER JOIN DatNguyenCon dnc ON nl.MaXuongSanXuat = dnc.MaXuong
FULL OUTER JOIN TPXeBuom tp ON nl.MaXuongSanXuat = tp.MaXuongSanXuat;
";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,
                    new { dateTime = dateTime.Date, fromDate = fromDate.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region XLPC
        #region BTP XẺ BƯỚM
        public List<T> GetPhieuCanBTPXeBuom_XLPC<T>(DateTime dateTime, string xuongId)
        {
            var query = @"SELECT 
    p.MaMayTinhCan,
    p.MaUserCan,
    p.ThoiGianCan,
    p.Ngay,
    p.MaXuongSanXuat,
    x.Ten AS XuongName,
    p.MSL,
    p.MaLoaiCa,
    lc.Ten AS LoaiCaName,
    p.MaLoaiThanhPham,
    tp.Ten AS ThanhPhamName,
    p.MaSize,
    s.Ten AS SizeName,
    p.MaMau,
    m.Ten AS MauName,
    p.TrongLuong,
    p.TrongLuongTare,
    p.MaTheTu,
    p.GhiChu
FROM PhieuCanTPFillet p
LEFT JOIN XiNghiep x ON x.Ma = p.MaXuongSanXuat
LEFT JOIN MaLoaiCaFillet lc ON lc.Ma = p.MaLoaiCa
LEFT JOIN MaThanhPhamFillet tp ON tp.Ma = p.MaLoaiThanhPham
LEFT JOIN MaSizeFillet s ON s.Ma = p.MaSize
LEFT JOIN MaMauFillet m ON m.Ma = p.MaMau
WHERE p.Ngay = @dateTime AND p.MaXuongSanXuat = @xuongId
AND (p.MaNhanVien IS NULL OR p.MaNhanVien = '')
ORDER BY 
    CASE WHEN p.TrongLuong = 0 THEN 1 ELSE 0 END,
    p.ThoiGianCan DESC,
    p.TrongLuong DESC
";
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
        #region TP XẺ BƯỚM
        public List<T> GetPhieuCanTPXeBuom_XLPC<T>(DateTime dateTime, string xuongId)
        {
            var query = @"SELECT 
    p.MaMayTinhCan,
    p.MaUserCan,
    p.ThoiGianCan,
    p.Ngay,
    p.MaXuongSanXuat,
    x.Ten AS XuongName,
    p.MaNhanVien,
    nv.MaHoSo,
    nv.Name as NhanVienName,
    p.MSL,
    p.MaLoaiCa,
    lc.Ten AS LoaiCaName,
    p.MaLoaiThanhPham,
    tp.Ten AS ThanhPhamName,
    p.MaSize,
    s.Ten AS SizeName,
    p.MaMau,
    m.Ten AS MauName,
    p.TrongLuong,
    p.TrongLuongTare,
    p.MaTheTu,
    p.GhiChu
FROM PhieuCanTPFillet p
LEFT JOIN NhanVienDaiThanh nv ON nv.MaNhanVien = p.MaNhanVien
LEFT JOIN XiNghiep x ON x.Ma = p.MaXuongSanXuat
LEFT JOIN MaLoaiCaFillet lc ON lc.Ma = p.MaLoaiCa
LEFT JOIN MaThanhPhamFillet tp ON tp.Ma = p.MaLoaiThanhPham
LEFT JOIN MaSizeFillet s ON s.Ma = p.MaSize
LEFT JOIN MaMauFillet m ON m.Ma = p.MaMau
WHERE 
    p.Ngay = @dateTime 
    AND p.MaXuongSanXuat = @xuongId
    AND p.MaNhanVien IS NOT NULL 
    AND p.MaNhanVien <> ''
ORDER BY 
    CASE WHEN p.TrongLuong = 0 THEN 1 ELSE 0 END,
    p.ThoiGianCan DESC,
    p.TrongLuong DESC
";
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
        #endregion
    }
}
