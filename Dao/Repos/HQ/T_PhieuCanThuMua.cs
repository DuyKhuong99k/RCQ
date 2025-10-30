using Dapper;
using Dapper.Database;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Dao.Repos.HQ
{
    public class T_PhieuCanThuMua
    {
        private readonly string queryDelete = @"DELETE FROM [dbo].[T_PhieuCanThuMua]
      WHERE  [Id] = @Id";

        private readonly string queryDeleteByIds = @"DELETE FROM [dbo].[T_PhieuCanThuMua_Server]
      WHERE  [Id] in ({0})";
        private readonly string queryUpdateStatusByIds = "Update T_PhieuCanThuMua Set Status =@status where Id in ({0})";

        private readonly string queryGetChiTietsByIds = @"
select p.Id, p.STT,
    p.Gio,
    p.MaLo,
    p.NgayNguyenLieu,
    nv.MaHoSo,
    nv.Name as TenNhanVien,
    nv.DeptName0 as Nhom,
    xn.Ten as Xuong,
    lnl.Ten as LoaiNguyenLieu,
    tc.Ten as TieuChuan,
    s.Ten as Size,
    bb.Ten as BaoBi,
    sp.Ten as SanPham,
    qt.Ten as QuyTrinh,
    pg.Ten as PhuGia,
    p.MaKhachHang,
    kh.Ten as KhachHang,
    ks.Ten as KhangSinh,
    ttp.Ten as ThongTinPhu,
    cd.Ten as CongDoan,
    p.VoXoTB,
    p.VoXoCD,
    p.VoXoCT,
    p.TrongLuong,
    p.TrongLuongTare,
    p.GhiChu
from (
        select p.Id, p.STT,
            P.Ngay,
            p.Gio,
            p.MaNhomLo as MaLo,
            nl.NgayNguyenLieu,
            p.MaNhanVien,
            p.MaXuong,
            p.MaLoaiNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.MaBaoBi,
            p.MaSanPham,
            p.MaQuyTrinh,
            p.MaPhuGia,
            p.MaKhachHang,
            p.MaKhangSinh,
            p.MaThongTinPhu,
            p.VoXoTB,
            p.VoXoCD,
            p.VoXoCT,
            p.TrongLuong,
            p.TrongLuongTare,
            p.GhiChu,
            p.MaCongDoan
        from T_PhieuCanThuMua p,
            T_NhomLo nl
        where p.MaNhomLo = nl.Ma
        and p.Id in ({0})
    ) p
    left join T_LoaiNguyenLieu lnl on lnl.Ma = p.MaLoaiNguyenLieu
    left join T_TieuChuan tc on tc.Ma = p.MaTieuChuan
    left join T_Size s on s.Ma = p.MaSanPham
    left join T_BaoBi bb on bb.Ma = p.MaBaoBi
    left join T_SanPham sp on sp.Ma = p.MaSanPham
    left join T_QuyTrinh qt on qt.Ma = p.MaQuyTrinh
    left join T_PhuGia pg on pg.Ma = p.MaPhuGia
    left join T_KhachHang kh on kh.Ma = p.MaKhachHang
    left join T_KhangSinh ks on ks.Ma = p.MaKhangSinh
    left join T_ThongTinPhu ttp on ttp.Ma = p.MaThongTinPhu
    left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
    left join XiNghiep xn on xn.Ma = p.MaXuong
    left join T_CongDoan cd on cd.Ma = p.MaCongDoan";

        private readonly string queryGetIdsStatus = @"
Select Id from T_PhieuCanThuMua where Status =@Status and Ngay >= '2024-01-01';

";

        private readonly string queryGetsIdByStatus = "Select Id from T_PhieuCanThuMua where Status = @status";

        private readonly string queryInsert = @"INSERT INTO [dbo].[T_PhieuCanThuMua]
           ([Id]
           ,[MaLoaiNguyenLieu]
           ,[MaTieuChuan]
           ,[MaSize]
           ,[MaBaoBi]
           ,[MaSanPham]
           ,[MaQuyTrinh]
           ,[MaPhuGia]
           ,[MaKhachHang]
           ,[MaKhangSinh]
           ,[MaThongTinPhu]
           ,[VoXoTB]
           ,[VoXoCD]
           ,[VoXoCT]
           ,[TyLe]
           ,[GramCuoi]
           ,[GramDau]
           ,[NhuCau]
           ,[NgayGioTao]
           ,[MaUserCan]
           ,[MayCan]
           ,[GhiChu]
           ,[STT]
           ,[Ngay]
           ,[Gio]
           ,[MaPhieuYeuCau]
           ,[TrongLuong]
           ,[TrongLuongTare]
           ,[MaXuong]
           ,[MaNhanVien],[MaThe],[Status],[NgayNguyenLieu],[MaLo],[MaNhomLo],[MaCongDoan])
     VALUES
           (@Id
           ,@MaLoaiNguyenLieu
           ,@MaTieuChuan
           ,@MaSize
           ,@MaBaoBi
           ,@MaSanPham
           ,@MaQuyTrinh
           ,@MaPhuGia
           ,@MaKhachHang
           ,@MaKhangSinh
           ,@MaThongTinPhu
           ,@VoXoTB
           ,@VoXoCD
           ,@VoXoCT
           ,@TyLe
           ,@GramCuoi
           ,@GramDau
           ,@NhuCau
           ,@NgayGioTao
           ,@MaUserCan
           ,@MayCan
           ,@GhiChu
           ,@STT
           ,@Ngay
           ,@Gio
           ,@MaPhieuYeuCau
           ,@TrongLuong
           ,@TrongLuongTare
           ,@MaXuong
           ,@MaNhanVien,@MaThe,@Status,@NgayNguyenLieu,@MaLo,@MaNhomLo,@MaCongDoan)";

        private readonly string queryUpdate = @"UPDATE [dbo].[T_PhieuCanThuMua]
   SET      
           [MaLoaiNguyenLieu] = @MaLoaiNguyenLieu
           ,[MaTieuChuan] = @MaTieuChuan
           ,[MaSize] = @MaSize
           ,[MaBaoBi] = @MaBaoBi
           ,[MaSanPham] = @MaSanPham
           ,[MaQuyTrinh] = @MaQuyTrinh
           ,[MaPhuGia] = @MaPhuGia
           ,[MaKhachHang] = @MaKhachHang
           ,[MaKhangSinh] = @MaKhangSinh
           ,[MaThongTinPhu] = @MaThongTinPhu
           ,[VoXoTB] = @VoXoTB
           ,[VoXoCD] = @VoXoCD
           ,[VoXoCT] = @VoXoCT
           ,[TyLe] = @TyLe
           ,[GramCuoi] = @GramCuoi
           ,[GramDau] = @GramDau
           ,[NhuCau] = @NhuCau
           ,[NgayGioTao] = @NgayGioTao
           ,[MaUserCan] = @MaUserCan
           ,[MayCan] = @MayCan
           ,[GhiChu] = @GhiChu
           ,[STT] = @STT
           ,[Ngay] = @Ngay
           ,[Gio] = @Gio
           ,[MaPhieuYeuCau] = @MaPhieuYeuCau
           ,[TrongLuong] = @TrongLuong
           ,[TrongLuongTare] = @TrongLuongTare
           ,[MaXuong] = @MaXuong
           ,[MaNhanVien] = @MaNhanVien, [MaThe] =@MaThe,[Status] = @Status,[NgayNguyenLieu]=@NgayNguyenLieu,[MaLo]=@MaLo,[MaNhomLo]=@MaNhomLo, [MaCongDoan] =@MaCongDoan
 WHERE [Id] = @Id";

        private readonly string queryUpdateDb = @"IF (
    NOT EXISTS (
        SELECT
            *
        FROM
            INFORMATION_SCHEMA.TABLES
        WHERE
            TABLE_SCHEMA = 'dbo'
            AND TABLE_NAME = 'T_PhieuCanThuMua'
    )
) BEGIN 
   CREATE TABLE [dbo].[T_PhieuCanThuMua](
	[Id] [varchar](200) NOT NULL,
	[MaLoaiNguyenLieu] [varchar](50) NOT NULL,
	[MaTieuChuan] [varchar](50) NOT NULL,
	[MaSize] [varchar](50) NOT NULL,
	[MaBaoBi] [varchar](50) NOT NULL,
	[MaSanPham] [varchar](50) NOT NULL,
	[MaQuyTrinh] [varchar](50) NOT NULL,
	[MaPhuGia] [varchar](50) NOT NULL,
	[MaKhachHang] [varchar](50) NOT NULL,
	[MaKhangSinh] [varchar](50) NOT NULL,
	[MaThongTinPhu] [varchar](50) NOT NULL,
	[VoXoTB] [varchar](50) NOT NULL,
	[VoXoCD] [varchar](50) NOT NULL,
	[VoXoCT] [varchar](50) NOT NULL,
	[TyLe] [decimal](18, 4) NOT NULL,
	[GramCuoi] [decimal](18, 2) NOT NULL,
	[GramDau] [decimal](18, 2) NOT NULL,
	[NhuCau] [decimal](18, 2) NOT NULL,
	[NgayGioTao] [datetime2](7) NOT NULL,
	[MaUserCan] [nvarchar](50) NOT NULL,
	[MayCan] [nvarchar](200) NOT NULL,
	[GhiChu] [nvarchar](max) NULL,
	[STT] [int] NOT NULL,
	[Ngay] [date] NOT NULL,
	[Gio] [time](7) NOT NULL,
	[MaPhieuYeuCau] [varchar](200) NOT NULL,
	[TrongLuong] [decimal](18, 2) NOT NULL,
	[TrongLuongTare] [decimal](18, 2) NOT NULL,
	[MaXuong] [varchar](50) NOT NULL,
	[MaNhanVien] [varchar](50) NOT NULL,
 CONSTRAINT [PK_T_PhieuCanThuMua] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
DECLARE @tb varchar(30) = 'T_PhieuCanThuMua' 
IF COL_LENGTH(@tb, 'MaThe') IS NULL BEGIN
ALTER TABLE T_PhieuCanThuMua
ADD 
MaThe [varchar](50) NOT NULL DEFAULT ((0))
END
IF COL_LENGTH(@tb, 'Status') IS NULL BEGIN
ALTER TABLE T_PhieuCanThuMua
ADD [Status] int NOT NULL DEFAULT ((0))
END
IF COL_LENGTH(@tb, 'NgayNguyenLieu') IS NULL BEGIN
ALTER TABLE T_PhieuCanThuMua
ADD [NgayNguyenLieu] DATE  NULL
END
IF COL_LENGTH(@tb, 'MaLo') IS NULL BEGIN
ALTER TABLE T_PhieuCanThuMua
ADD [MaLo] [varchar](50)  NULL
END
IF COL_LENGTH(@tb, 'MaNhomLo') IS NULL BEGIN
ALTER TABLE T_PhieuCanThuMua
ADD [MaNhomLo] [varchar](50)  NULL
END
IF COL_LENGTH(@tb, 'MaCongDoan') IS NULL BEGIN
ALTER TABLE T_PhieuCanThuMua
ADD [MaCongDoan] [varchar](50)  NULL
END
";

        public T_PhieuCanThuMua(string? _connectionString = null)
        {
             connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;
        }


        private readonly string connectionString;

        public int Delete<T>(T item)
        {
            var query = queryDelete;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, item);
            return rows;
        }

        public int Delete(string Ids)
        {
            var query = string.Format(queryDeleteByIds, Ids);
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query);
            return rows;
        }

        public T Get<T>(string id)
        {
            try
            {
                var query = "Select * from T_PhieuCanThuMua Where Ma = @id";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection.QueryAsync<T>(query, new { id }).Result.SingleOrDefault();
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> GetChiTietsByIds<T>(string ids)
        {
            try
            {
                var query = string.Format(queryGetChiTietsByIds, ids);
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(
                        query)
                    .ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> GetChiTietsDateTimeToDateTime<T>(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            var query = @"
select p.*,
    nv.MaHoSo,
    nv.Name as NhanVienName,
    nv.DeptName0 as Nhom,
    xn.Ten as XuongName,
    lnl.Ten as LoaiNguyenLieuName,
    tc.Ten as TieuChuanName,
    s.Ten as SizeName,
    bb.Ten as BaoBiName,
    sp.Ten as SanPhamName,
    qt.Ten as QuyTrinhName,
    pg.Ten as PhuGiaName,
    kh.Ten as KhachHangName,
    ks.Ten as KhangSinhName,
    ttp.Ten as ThongTinPhuName,
    cd.Ten as CongDoanName
from (
        select p.STT,
            P.Ngay,
            p.Gio,
            nl.Ten as MaLo,
            nl.NgayNguyenLieu,
            p.MaNhanVien,
            p.MaXuong,
            p.MaLoaiNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.MaBaoBi,
            p.MaSanPham,
            p.MaQuyTrinh,
            p.MaPhuGia,
            p.MaKhachHang,
            p.MaKhangSinh,
            p.MaThongTinPhu,
            p.VoXoTB,
            p.VoXoCD,
            p.VoXoCT,
            p.TrongLuong,
            p.TrongLuongTare,
            p.GhiChu,
            p.MaCongDoan
        from T_PhieuCanThuMua p,
            T_NhomLo nl
        where p.MaNhomLo = nl.Ma
            and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @ngay
            and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
            and SUBSTRING(p.Id, CHARINDEX('.', p.Id) + 1, CHARINDEX('.', p.Id, CHARINDEX('.', p.Id) + 1) - CHARINDEX('.', p.Id) - 1) =@xuongId  --p.MaXuong = @xuongId
    ) p
    left join T_LoaiNguyenLieu lnl on lnl.Ma = p.MaLoaiNguyenLieu
    left join T_TieuChuan tc on tc.Ma = p.MaTieuChuan
    left join T_Size s on s.Ma = p.MaSize
    left join T_BaoBi bb on bb.Ma = p.MaBaoBi
    left join T_SanPham sp on sp.Ma = p.MaSanPham
    left join T_QuyTrinh qt on qt.Ma = p.MaQuyTrinh
    left join T_PhuGia pg on pg.Ma = p.MaPhuGia
    left join T_KhachHang kh on kh.Ma = p.MaKhachHang
    left join T_KhangSinh ks on ks.Ma = p.MaKhangSinh
    left join T_ThongTinPhu ttp on ttp.Ma = p.MaThongTinPhu
    left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
    left join XiNghiep xn on xn.Ma = p.MaXuong
    left join T_CongDoan cd on cd.Ma= p.MaCongDoan
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new
                        {
                            fromDate,
                            ngay = dateTime,
                            xuongId
                        })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetChiTietsDateTimeToDateTimeNgayNguyenLieu<T>(DateTime fromDate, DateTime dateTime,
            string xuongId)
        {
            var query = @"
select p.*,
    nv.MaHoSo,
    nv.Name as NhanVienName,
    nv.DeptName0 as Nhom,
    xn.Ten as XuongName,
    lnl.Ten as LoaiNguyenLieuName,
    tc.Ten as TieuChuanName,
    s.Ten as SizeName,
    bb.Ten as BaoBiName,
    sp.Ten as SanPhamName,
    qt.Ten as QuyTrinhName,
    pg.Ten as PhuGiaName,
    kh.Ten as KhachHangName,
    ks.Ten as KhangSinhName,
    ttp.Ten as ThongTinPhuName,
    cd.Ten as CongDoanName
from (
        select p.STT,
            P.Ngay,
            p.Gio,
            nl.Ten as MaLo,
            nl.NgayNguyenLieu,
            p.MaNhanVien,
            p.MaXuong,
            p.MaLoaiNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.MaBaoBi,
            p.MaSanPham,
            p.MaQuyTrinh,
            p.MaPhuGia,
            p.MaKhachHang,
            p.MaKhangSinh,
            p.MaThongTinPhu,
            p.VoXoTB,
            p.VoXoCD,
            p.VoXoCT,
            p.TrongLuong,
            p.TrongLuongTare,
            p.GhiChu,
            p.MaCongDoan
        from T_PhieuCanThuMua p,
            T_NhomLo nl
        where p.MaNhomLo = nl.Ma
            and nl.NgayNguyenLieu <= @ngay
            and nl.NgayNguyenLieu >= @fromDate
            and SUBSTRING(p.Id, CHARINDEX('.', p.Id) + 1, CHARINDEX('.', p.Id, CHARINDEX('.', p.Id) + 1) - CHARINDEX('.', p.Id) - 1) =@xuongId  --p.MaXuong = @xuongId
    ) p
    left join T_LoaiNguyenLieu lnl on lnl.Ma = p.MaLoaiNguyenLieu
    left join T_TieuChuan tc on tc.Ma = p.MaTieuChuan
    left join T_Size s on s.Ma = p.MaSize
    left join T_BaoBi bb on bb.Ma = p.MaBaoBi
    left join T_SanPham sp on sp.Ma = p.MaSanPham
    left join T_QuyTrinh qt on qt.Ma = p.MaQuyTrinh
    left join T_PhuGia pg on pg.Ma = p.MaPhuGia
    left join T_KhachHang kh on kh.Ma = p.MaKhachHang
    left join T_KhangSinh ks on ks.Ma = p.MaKhangSinh
    left join T_ThongTinPhu ttp on ttp.Ma = p.MaThongTinPhu
    left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
    left join XiNghiep xn on xn.Ma = p.MaXuong
    left join T_CongDoan cd on cd.Ma= p.MaCongDoan
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new
                        {
                            fromDate,
                            ngay = dateTime,
                            xuongId
                        })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetDinhMucLacDauTTTM<T>(DateTime fromDate, DateTime toDate)
        {
            var query = @"
select * from (
Select m.NgayNguyenLieu,
    m.LoaiTom,
    m.MaDaiLy,
    m.SoLo,
    pPC.NhomNguyenLieu,
    m.TrongLuong,
    m.Con_lb,
    isnull( pLD.TrongLuong,-1) as TrongLuongLD,
    Cast (
        (
            case
                when ISNULL(pLD.TrongLuong, 0) = 0 then 0
                else m.TrongLuong / pLD.TrongLuong
            end
        ) as decimal(18, 2)
    ) as DMLD,
    ISNULL( pPC.TrongLuong ,-1) as TrongLuongSauRaiMay,
    Cast (
        (
            case
                when ISNULL(pPC.TrongLuong, 0) = 0 then 0
                else m.TrongLuong / pPC.TrongLuong
            end
        ) as decimal(18, 2)
    ) as DMSPC,
    m.NhomKS
from (
        Select cast(m.NgayNguyenLieu as date) as NgayNguyenLieu,
            M.LoaiTom,
			LEFT(m.LoaiTom,2) as LoaiN,
            m.MaDaiLy,
            m.SoLo,
            SUBSTRING(m.SoLo, 8, 1) as NhomNguyenLieu,
            m.TrongLuong,
            m.Con_lb,
            m.TenSizeCo,
            --m.QuyTrinh
            m.NhomKS
        from T_MuaNguyenLieu_Import m
        where cast(m.NgayNguyenLieu as date) >= @fromDate
            and cast(m.NgayNguyenLieu as Date) <= @toDate
    ) m
    Left JOIN (
        select pLD.NgayNguyenLieu,
            pLD.LoaiTom,
            pLD.MaDaiLy,
            pLD.SoLo,
            --  nl.Ten as SoLo,
            pLD.NhomNguyenLieu,
			pLD.LoaiN,
            SUM(pLD.TrongLuong) as TrongLuong
        from(
                Select nl.NgayNguyenLieu,
                    sp.Ten as LoaiTom,
                    LEFT(nl.Ten, 3) as MaDaiLy,
                    SUBSTRING(nl.Ten, 4, 2) as SoLo,
                    --  nl.Ten as SoLo,
                    SUBSTRING(nl.Ten, 8, 1) as NhomNguyenLieu,
					LEFT(sp.Ten ,2) LoaiN,
                    SUM(p.TrongLuong) as TrongLuong
                from T_PhieuCan p,
                    T_NhomLo nl,
                    T_SanPham sp
                where p.MaNhomLo = nl.ma
                    and nl.NgayNguyenLieu >= @fromDate
                    and nl.NgayNguyenLieu <= @toDate
                    and sp.Ma = p.MaSanPham
                    and p.MaThanhPham = 'LD'
					and LEFT(sp.Ten,2) in ('NT','NS')
                GROUP BY nl.NgayNguyenLieu,
                    sp.Ten,
                    nl.Ten
            ) pLD
        GROUP BY pLD.NgayNguyenLieu,
            pLD.LoaiTom,
            pLD.MaDaiLy,
            pLD.SoLo,
            pLD.NhomNguyenLieu,
			pLD.LoaiN
    ) pLD on pLD.NgayNguyenLieu = m.NgayNguyenLieu
    --and CHARINDEX(UPPER(m.LoaiN), UPPER(pLD.LoaiN)) > 0 --and UPPER(pLD.LoaiTom) = UPPER(m.LoaiTom)
and UPPER(m.LoaiN)= UPPER(pLD.LoaiN)
    and  pLD.MaDaiLy = m.MaDaiLy
    and pLD.SoLo = m.SoLo
    LEFT JOIN (
        Select p.NgayNguyenLieu,
            p.SoLo,
            p.MaDaiLy,
            p.LoaiTom,
			p.LoaiN,
			p.NhomNguyenLieu,
            SUM(p.TrongLuong) as TrongLuong
        from(
                Select nl.NgayNguyenLieu,
                    SUBSTRING(nl.Ten, 4, 2) as SoLo,
                     LEFT(nl.Ten, 3) as MaDaiLy,
 SUBSTRING(nl.Ten, 8, 1) as NhomNguyenLieu,
                    sp.Ten as LoaiTom,
					LEFT(sp.Ten ,2) LoaiN,
                    --p.VoXoTB,
                    --qt.Ten as QuyTrimh,
                    SUM(p.TrongLuong) as TrongLuong
                from T_PhieuCanThuMua p,
                    T_NhomLo nl,
                    T_SanPham sp
                    --T_QuyTrinh qt,
                where p.MaNhomLo = nl.ma
                    and nl.NgayNguyenLieu >= @fromDate
                    and nl.NgayNguyenLieu <= @toDate
                    and sp.Ma = p.MaSanPham
                    
                GROUP BY nl.NgayNguyenLieu,
                    sp.Ten,
                    
                    --p.VoXoTB,
                    -- qt.Ten,
                    nl.Ten
            ) p
        GROUP BY p.NgayNguyenLieu,
            p.SoLo,
            p.MaDaiLy,
            p.LoaiTom,
			p.NhomNguyenLieu,p.LoaiN
    ) pPC on pPC.NgayNguyenLieu = m.NgayNguyenLieu
    --and CHARINDEX(UPPER(m.LoaiN), UPPER(pPC.LoaiN)) > 0 --and UPPER(pPC.LoaiTom) = UPPER(m.LoaiTom)
    --and UPPER(pPC.SizeName) = UPPER(m.TenSizeCo) --and UPPER(pPC.VoXoTB)
    --and UPPER(pPC.KhangSinh) = UPPER(m.NhomKS)
    and UPPER(pPC.MaDaiLy) = UPPER(m.MaDaiLy)
    and pPC.SoLo = m.SoLo
    and isnull(m.LoaiTom,'') !=''
    and UPPER(m.LoaiN)= UPPER(pPC.LoaiN)
	) p where p.TrongLuongLD >-1

";
//             var query = @"
//Select m.NgayNguyenLieu,
//    M.LoaiTom,
//    m.MaDaiLy,
//    m.SoLo,
//    m.NhomNguyenLieu,
//    pPC.TrongLuong,
//    m.Con_lb,
//    pLD.TrongLuong as TrongLuongLD,
//    Cast (
//        (
//            case
//                when ISNULL(pLD.TrongLuong, 0) = 0 then 0
//                else m.TrongLuong / pLD.TrongLuong
//            end
//        ) as decimal(18, 2)
//    ) as DMLD,
//    pPC.TrongLuong as TrongLuongSauRaiMay,
//    Cast (
//        (
//            case
//                when ISNULL(pPC.TrongLuong, 0) = 0 then 0
//                else m.TrongLuong / pPC.TrongLuong
//            end
//        ) as decimal(18, 2)
//    ) as DMSPC,
//    m.NhomKS
//from (
//        Select cast( m.NgayNguyenLieu as date) as NgayNguyenLieu,
//            M.LoaiTom,
//            m.MaDaiLy,
//            m.SoLo,
//            SUBSTRING(m.SoLo, 8, 1) as NhomNguyenLieu,
//            m.TrongLuong,
//            m.Con_lb,
//            m.TenSizeCo,
//            --m.QuyTrinh
//            m.NhomKS
//        from T_MuaNguyenLieu_Import m
//        where cast(  m.NgayNguyenLieu as date) >= @fromDate
//            and cast(m.NgayNguyenLieu as Date) <= @toDate
//    ) m
//    Left JOIN (
//        Select nl.NgayNguyenLieu,
//            sp.Ten as LoaiTom,
//            LEFT(nl.Ten, 3) as MaDaiLy,
//            nl.Ten as SoLo,
//            SUBSTRING(nl.Ten, 7, 1) as NhomNguyenLieu,
//            SUM(p.TrongLuong) as TrongLuong
//        from T_PhieuCan p,
//            T_NhomLo nl,
//            T_SanPham sp
//        where p.MaNhomLo = nl.ma
//            and nl.NgayNguyenLieu >= @fromDate
//            and nl.NgayNguyenLieu <= @toDate
//            and sp.Ma = p.MaSanPham
//            and p.MaThanhPham = 'LD'
//        GROUP BY  nl.NgayNguyenLieu,
//            sp.Ten,
//            nl.Ten
//    ) pLD on pLD.NgayNguyenLieu = m.NgayNguyenLieu
//    and UPPER(pLD.LoaiTom) = UPPER(m.LoaiTom)
//    and pLD.MaDaiLy = m.MaDaiLy
//    and pLD.SoLo = m.SoLo
//    and pLD.NhomNguyenLieu = m.NhomNguyenLieu
//    LEFT JOIN (
//        Select  nl.NgayNguyenLieu,
//            sp.Ten as LoaiTom,
//            s.Ten as SizeName,
//            --p.VoXoTB,
//            --qt.Ten as QuyTrimh,
//            p.MaKhachHang,
//            ks.Ten as KhangSinh,
//            SUM(p.TrongLuong) as TrongLuong
//        from T_PhieuCanThuMua p,
//            T_NhomLo nl,
//            T_SanPham sp,
//            T_Size s,
//            --T_QuyTrinh qt,
//            T_KhangSinh ks
//        where p.MaNhomLo = nl.ma
//            and nl.NgayNguyenLieu >= @fromDate
//            and nl.NgayNguyenLieu <= @toDate
//            and sp.Ma = p.MaSanPham
//            and p.MaSize = s.Ma --and p.MaQuyTrinh = qt.Ma
//            and p.MaKhangSinh = ks.Ma
//        GROUP BY  nl.NgayNguyenLieu,
//            sp.Ten,
//            s.Ten,
//            p.MaKhachHang,
//            --p.VoXoTB,
//            -- qt.Ten,
//            ks.Ten
//    ) pPC on pPC.NgayNguyenLieu = m.NgayNguyenLieu
//    and UPPER(pPC.LoaiTom) = UPPER(m.LoaiTom)
//    and UPPER(pPC.SizeName) = UPPER(m.TenSizeCo) --and UPPER(pPC.VoXoTB)
//    and UPPER(pPC.KhangSinh) = UPPER(m.NhomKS)
//    and UPPER(pPC.MaKhachHang) = UPPER(m.MaDaiLy)

//";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetHaoHutPC_Giao<T>(DateTime fromDate, DateTime toDate)
        {
            var query = @"
 Select b.DonViNhan,
    b.NgayLap,
    b.LoaiTom,
    b.SizeTPBaoBi,
    b.CoVoXo,
    b.LoaiQuyTrinh,
    b.KhangSinh,
    b.GhiChuPhuGia,
    b.MaKhachHang,
    pPC.TrongLuong as TrongLuongPC,
    b.TrongLuongGiao,
    cast (
        case
            when ISNULL(pPC.TrongLuong, -1) = -1 then 0
            else (b.TrongLuongGiao - pPC.TrongLuong) / pPC.TrongLuong
        end as DECIMAL(18, 2)
    ) as DinhMucGiao_Nhan,
    b.HinhThucCan,
    b.GhiChu
from (
        SELECT *
        from T_BieuMauGiao_Import b
        where CAST(b.NgayNguyenLieu as date) >= @fromDate
            and CAST(b.NgayNguyenLieu as date) <= @toDate
    ) b
    LEFT JOIN (
        Select nl.NgayNguyenLieu,
            sp.Ten as LoaiTom,
            s.Ten as SizeName,
            --p.VoXoTB,
            qt.LoaiQuyTrinh ,
            p.MaKhachHang,
            ks.Ten as KhangSinh,
            SUM(p.TrongLuong) as TrongLuong
        from T_PhieuCanThuMua p,
            T_NhomLo nl,
            T_SanPham sp,
            T_BaoBi s,
            T_QuyTrinh qt,
            T_KhangSinh ks
        where p.MaNhomLo = nl.ma
            and nl.NgayNguyenLieu >= @fromDate
            and nl.NgayNguyenLieu <= @toDate
            and sp.Ma = p.MaSanPham
            and p.MaBaoBi = s.Ma and p.MaQuyTrinh = qt.Ma
            and p.MaKhangSinh = ks.Ma
        GROUP BY nl.NgayNguyenLieu,
            sp.Ten,
            s.Ten,
            p.MaKhachHang,
            --p.VoXoTB,
            -- qt.Ten,
			qt.LoaiQuyTrinh,
            ks.Ten
    ) pPC on pPC.NgayNguyenLieu = CAST(b.NgayNguyenLieu as date) --and UPPER(pPC.LoaiTom) = UPPER(b.LoaiTom)
    and CHARINDEX(UPPER(b.LoaiTom), UPPER(pPC.LoaiTom)) > 0
    and UPPER(pPC.SizeName) = UPPER(b.SizeTPBaoBi) --and UPPER(pPC.VoXoTB)
    and UPPER(pPC.KhangSinh) = UPPER(b.KhangSinh)
    and UPPER(pPC.MaKhachHang) = UPPER(b.MaKhachHang)
	and UPPER(pPC.LoaiQuyTrinh) = UPPER(b.loaiquytrinh)

";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.Query<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date })
                    
                    .ToList();
                return items;
            }
        }

        public List<string> GetIds(int Status)
        {
            var query = queryGetIdsStatus;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<string>(query, new { Status }).ToList();
            return items;
        }

        public List<T> Gets<T>()
        {
            try
            {
                var query = "Select * from T_PhieuCanThuMua";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query).Result.ToList();
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
                var query = "Select * from T_PhieuCanThuMua where Ngay = @dateTime and MaXuong = @xuongId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new
                {
                    dateTime = dateTime.Date,
                    xuongId
                }).Result.ToList();
                return items;
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
                var query = "Select * from T_PhieuCanThuMua where Ngay = @dateTime ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new
                {
                    dateTime = dateTime.Date
                }).Result.ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<string> Gets(int status)
        {
            try
            {
                var query = queryGetIdsStatus;
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<string>(query,new {status}).Result.ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> Gets<T>(bool suDung)
        {
            try
            {
                var query = "Select * from T_PhieuCanThuMua where SuDung = @suDung";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { suDung }).Result.ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> GetsByMayCan<T>(DateTime dateTime, string mayCan)
        {
            try
            {
                var query = "Select * from T_PhieuCanThuMua where Ngay = @dateTime and MayCan = @mayCan";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new
                {
                    dateTime = dateTime.Date,
                    mayCan
                }).Result.ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> GetsByPhieuYeuCau<T>(string maPhieuPhanCo)
        {
            var query =
                @"select 
    p.MaPhieuYeuCau,
    sum(p.NhuCau) as NhuCau,
    Sum(p.TrongLuong) as TrongLuong
from T_PhieuCanThuMua p
where  p.MaPhieuYeuCau = @maPhieuPhanCo
GROUP BY 
    p.MaPhieuYeuCau";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { maPhieuPhanCo }).ToList();
            return items;
        }
        public List<T> GetsByPhieuYeuCaus<T>(string ids)
        {
            var query =
                $@"
select 
    p.MaPhieuYeuCau,
    p.NhuCau,
    Sum(p.TrongLuong) as TrongLuong
from T_PhieuCanThuMua p
where  p.MaPhieuYeuCau in ({ids})
GROUP BY 
    p.MaPhieuYeuCau,
	p.NhuCau";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query).ToList();
            return items;
        }
        public Tuple<int, decimal> GetSoRoTongTrongLuongByPhieuYeuCau(
            string phieuYeuCauId)
        {
            var query = @"Select
    Count(*) as Item1,
    ISNULL(Sum(TrongLuong), 0) As Item2
   
from
    T_PhieuCanThuMua
where MaPhieuYeuCau = @phieuYeuCauId";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var row = connection.Query<Tuple<int, decimal>>(
                    query,
                    new { phieuYeuCauId })
                .SingleOrDefault();
            return row;
        }

        public List<T> GetTongHopNhanViensDateTimeToDateTime<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var query = @"select p.*,
    nv.MaHoSo,
    nv.Name as NhanVienName,
    nv.DeptName0 as Nhom,
    lnl.Ten as LoaiNguyenLieuName,
    tc.Ten as TieuChuanName,
    s.Ten as SizeName,
    bb.Ten as BaoBiName,
    sp.Ten as SanPhamName,
    qt.Ten as QuyTrinhName,
    pg.Ten as PhuGiaName,
    kh.Ten as KhachHangName,
    ks.Ten as KhangSinhName,
    ttp.Ten as ThongTinPhuName,
    cd.Ten as CongDoanName
from (
        select p.Ngay,
            p.MaNhanVien,
            p.MaXuong,
            p.MaLoaiNguyenLieu,
            p.MaNhomLo as MaLo,
            nl.NgayNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.MaBaoBi,
            p.MaSanPham,
            p.MaQuyTrinh,
            p.MaPhuGia,
            p.MaKhachHang,
            p.MaKhangSinh,
            p.MaThongTinPhu,
            p.VoXoTB,
            p.VoXoCD,
            p.VoXoCT,
            sum(p.TrongLuong) as TrongLuong,
            COUNT(*) as SoRo,
            p.MaCongDoan
        from T_PhieuCanThuMua p,
            T_NhomLo nl
        where p.MaNhomLo = nl.Ma
            and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @ngay
            and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
            and SUBSTRING(p.Id, CHARINDEX('.', p.Id) + 1, CHARINDEX('.', p.Id, CHARINDEX('.', p.Id) + 1) - CHARINDEX('.', p.Id) - 1)=@xuongId  --p.MaXuong = @xuongId
        GROUP BY p.Ngay,
            p.MaNhomLo,
            p.MaCongDoan,
            nl.NgayNguyenLieu,
            p.MaNhanVien,
            p.MaXuong,
            p.MaLoaiNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.MaBaoBi,
            p.MaSanPham,
            p.MaQuyTrinh,
            p.MaPhuGia,
            p.MaKhachHang,
            p.MaKhangSinh,
            p.MaThongTinPhu,
            p.VoXoTB,
            p.VoXoCD,
            p.VoXoCT
    ) p
    left join T_LoaiNguyenLieu lnl on lnl.Ma = p.MaLoaiNguyenLieu
    left join T_TieuChuan tc on tc.Ma = p.MaTieuChuan
    left join T_Size s on s.Ma = p.MaSize
    left join T_BaoBi bb on bb.Ma = p.MaBaoBi
    left join T_SanPham sp on sp.Ma = p.MaSanPham
    left join T_QuyTrinh qt on qt.Ma = p.MaQuyTrinh
    left join T_PhuGia pg on pg.Ma = p.MaPhuGia
    left join T_KhachHang kh on kh.Ma = p.MaKhachHang
    left join T_KhangSinh ks on ks.Ma = p.MaKhangSinh
    left join T_ThongTinPhu ttp on ttp.Ma = p.MaThongTinPhu
    left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
    left join T_CongDoan cd on cd.Ma = p.MaCongDoan
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate, ngay = toDate, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetTongHopNhanViensDateTimeToDateTimeNgayNguyenLieu<T>(DateTime fromDate, DateTime toDate,
            string xuongId)
        {
            var query = @"
select p.*,
    nv.MaHoSo,
    p.MaLo,
    p.NgayNguyenLieu,
    nv.Name as NhanVienName,
    nv.DeptName0 as Nhom,
    lnl.Ten as LoaiNguyenLieuName,
    tc.Ten as TieuChuanName,
    s.Ten as SizeName,
    bb.Ten as BaoBiName,
    sp.Ten as SanPhamName,
    qt.Ten as QuyTrinhName,
    pg.Ten as PhuGiaName,
    kh.Ten as KhachHangName,
    ks.Ten as KhangSinhName,
    ttp.Ten as ThongTinPhuName,
    cd.Ten as CongDoanName
from (
        select p.Ngay,
            p.MaNhanVien,
            p.MaNhomLo as MaLo,
            nl.NgayNguyenLieu,
            p.MaXuong,
            p.MaLoaiNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.MaBaoBi,
            p.MaSanPham,
            p.MaQuyTrinh,
            p.MaPhuGia,
            p.MaKhachHang,
            p.MaKhangSinh,
            p.MaThongTinPhu,
            p.VoXoTB,
            p.VoXoCD,
            p.VoXoCT,
            sum(p.TrongLuong) as TrongLuong,
            COUNT(*) as SoRo,
            p.MaCongDoan
        from T_PhieuCanThuMua p,
            T_NhomLo nl
        where p.MaNhomLo = nl.Ma
            and nl.NgayNguyenLieu <= @ngay
            and nl.NgayNguyenLieu >= @fromDate
            and SUBSTRING(p.Id, CHARINDEX('.', p.Id) + 1, CHARINDEX('.', p.Id, CHARINDEX('.', p.Id) + 1) - CHARINDEX('.', p.Id) - 1) =@xuongId  --p.MaXuong = @xuongId
        GROUP BY p.Ngay,
            p.MaNhanVien,
            p.MaCongDoan,
            p.MaXuong,
            p.MaLoaiNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.MaBaoBi,
            p.MaSanPham,
            p.MaQuyTrinh,
            p.MaPhuGia,
            p.MaKhachHang,
            p.MaKhangSinh,
            p.MaThongTinPhu,
            p.VoXoTB,
            p.VoXoCD,
            p.VoXoCT,
            p.MaNhomLo,
            nl.NgayNguyenLieu
    ) p
    left join T_LoaiNguyenLieu lnl on lnl.Ma = p.MaLoaiNguyenLieu
    left join T_TieuChuan tc on tc.Ma = p.MaTieuChuan
    left join T_Size s on s.Ma = p.MaSize
    left join T_BaoBi bb on bb.Ma = p.MaBaoBi
    left join T_SanPham sp on sp.Ma = p.MaSanPham
    left join T_QuyTrinh qt on qt.Ma = p.MaQuyTrinh
    left join T_PhuGia pg on pg.Ma = p.MaPhuGia
    left join T_KhachHang kh on kh.Ma = p.MaKhachHang
    left join T_KhangSinh ks on ks.Ma = p.MaKhangSinh
    left join T_ThongTinPhu ttp on ttp.Ma = p.MaThongTinPhu
    left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
    left join T_CongDoan cd on cd.Ma = p.MaCongDoan
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate, ngay = toDate.Date, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetTongHopPhanCoTheoYeuCau<T>(DateTime fromDate, DateTime toDate)
        {
            var query = @"
Select nh.Ten as PhieuYeuCau,p.*
from (
        select pyc.MaNhomYeuCau,
            p.MaPhieuYeuCau,
            p.NgayNguyenLieu,
            concat('''', s.Ten) as SizeName,
            p.VoXoTB,
            p.VoXoCD,
            p.VoXoCT,
            sp.Ten as SanPhamName,
            qt.Ten as QuyTrinhName,
             concat('''', p.MaKhachHang) as MaKhachHang,
            kh.Ten as KhachHangName,
            ks.Ten as KhangSinhName,
            p.MaThongTinPhu,
            ttp.Ten as ThongTinPhuName,
            p.TongTrongLuongTichLuy,
            p.NhuCau,
            p.TrongLuongConThieu
        from (
                Select p.MaPhieuYeuCau,
                    p.NgayNguyenLieu,
                    pyc.MaSize,
                    pyc.VoXoTB,
                    pyc.VoXoCD,
                    pyc.VoXoCT,
                    pyc.MaSanPham,
                    pyc.MaQuyTrinh,
                    pyc.MaKhachHang,
                    pyc.MaKhangSinh,
                    pyc.MaThongTinPhu,
                    p.TongTrongLuongTichLuy,
                    pyc.NhuCau,
                    (pyc.NhuCau - p.TongTrongLuongTichLuy) as TrongLuongConThieu
                from (
                        Select p.NNL as NgayNguyenLieu,
                            p.MaPhieuYeuCau,
                            (
                                SELECT SUM(p1.TrongLuong)
                                FROM T_PhieuCanThuMua p1,
                                    T_NhomLo nl
                                WHERE p1.MaPhieuYeuCau = p.MaPhieuYeuCau
                                    and p1.MaNhomLo = nl.Ma
                                    AND nl.NgayNguyenLieu <= p.NNL
                            ) AS TongTrongLuongTichLuy
                        from (
                                Select p.*,
                                    nl.NgayNguyenLieu as NNL
                                from T_PhieuCanThuMua p,
                                    T_NhomLo nl
                                where nl.Ma = p.MaNhomLo
                                    and nl.NgayNguyenLieu <= @toDate
                                    and nl.NgayNguyenLieu >= @fromDate
                            ) p
                        where p.MaPhieuYeuCau in (
                                Select Distinct p.MaPhieuYeuCau
                                from T_PhieuCanThuMua p,
                                    T_NhomLo nl
                                where nl.Ma = p.MaNhomLo
                                    and nl.NgayNguyenLieu <= @toDate
                                    and nl.NgayNguyenLieu >= @fromDate
                            )
                        GROUP By p.NNL,
                            p.MaPhieuYeuCau
                    ) p
                    INNER JOIN (
                        select *
                        from T_PhieuYeuCau pyc
                        where pyc.Id in (
                                Select Distinct p.MaPhieuYeuCau
                                from T_PhieuCanThuMua p,
                                    T_NhomLo nl
                                where nl.Ma = p.MaNhomLo
                                    and nl.NgayNguyenLieu <= @toDate
                                    and nl.NgayNguyenLieu >= @fromDate
                            )
                    ) pyc on p.MaPhieuYeuCau = pyc.Id
            ) p
            left JOIN T_Size s on p.MaSize = s.Ma
            left JOIN T_SanPham sp on p.MaSanPham = sp.Ma
            left JOIN T_QuyTrinh qt on p.MaQuyTrinh = qt.Ma
            left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
            left JOIN T_KhangSinh ks on p.MaKhangSinh = ks.Ma
            LEFT JOIN T_PhieuYeuCau pyc on p.MaPhieuYeuCau = pyc.Id
            LEFT Join T_ThongTinPhu ttp on p.MaThongTinPhu = ttp.Ma
    ) p
    LEFT JOIN T_PhieuNhomYeuCau nh on nh.Ma = p.MaNhomYeuCau
order by p.NgayNguyenLieu
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopPhanCoTheoYeuCauDashBoard<T>()
        {
            var query = @"
SELECT 
	ttp.Ten as TTPhuName,
	lnl.Ten as LoaiTom,
    tc.Ten as TieuChuan,
    s.Ten as Size,
    pyc.VoXoTB,
	sp.Ten as SanPham,
    qt.Ten as QuyTrinh,
    pg.Ten as PhuGia,
    
	pyc.NhuCau as NhuCau,
        isnull(p.TrongLuong,0) as TrongLuong,
    cast(
        (ISNULL(p.TrongLuong, 0) / pyc.NhuCau)*100 as DECIMAL(18, 4) 
    ) as TyLeHoanThanh
from (
        Select p.MaLoaiNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.VoXoTB,
            p.MaQuyTrinh,
            p.MaPhuGia,
			p.MaSanPham,
			p.MaThongTinPhu,
            SUM(p.NhuCau) as NhuCau
        from T_PhieuYeuCau p,
		T_PhieuNhomYeuCau n
        where cast(n.NgayTao as date) = cast( DATEADD(DAY, 0, GETDATE()) as date) and p.MaNhomYeuCau = n.Ma and n.SuDung = 1
        GROUP by p.MaLoaiNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.VoXoTB,
            p.MaQuyTrinh,
            p.MaPhuGia,
			p.MaSanPham,
			p.MaThongTinPhu
    ) pyc
    LEFT join (
        Select pyc.MaLoaiNguyenLieu,
            pyc.MaTieuChuan,
            pyc.MaSize,
            pyc.VoXoTB,
            pyc.MaQuyTrinh,
            pyc.MaPhuGia,
			pyc.MaThongTinPhu,
            pyc.MaSanPham,
            SUM(p.TrongLuong) as TrongLuong
        from T_PhieuCanThuMua p,
        T_PhieuYeuCau pyc
        WHERE p.MaPhieuYeuCau in (
                Select DISTINCT Id
                from T_PhieuYeuCau pyc,
				T_PhieuNhomYeuCau n
                where cast(n.NgayTao as date) =  cast( DATEADD(DAY, 0, GETDATE()) as date)  and pyc.MaNhomYeuCau = n.Ma and n.SuDung = 1
            ) and p.MaPhieuYeuCau = pyc.Id
        GROUP by pyc.MaLoaiNguyenLieu,
            pyc.MaTieuChuan,
            pyc.MaSize,
            pyc.VoXoTB,
            pyc.MaQuyTrinh,
            pyc.MaPhuGia,
			pyc.MaThongTinPhu,
            pyc.MaSanPham
    ) p on p.MaLoaiNguyenLieu = pyc.MaLoaiNguyenLieu
    and p.MaTieuChuan = pyc.MaTieuChuan
    AND p.MaSize = pyc.MaSize
    and p.VoXoTB = pyc.VoXoTB
    and p.MaQuyTrinh = pyc.MaQuyTrinh
    and p.MaPhuGia = pyc.MaPhuGia
	and p.MaThongTinPhu = pyc.MaThongTinPhu
    and p.MaSanPham = pyc.MaSanPham
    left JOIN T_LoaiNguyenLieu lnl on pyc.MaLoaiNguyenLieu = lnl.Ma
    left JOIN T_TieuChuan tc on pyc.MaTieuChuan = tc.Ma
    left JOIN T_Size s on pyc.MaSize = s.Ma
    left JOIN T_QuyTrinh qt on pyc.MaQuyTrinh = qt.Ma
    LEFT JOIN T_PhuGia pg on pyc.MaPhuGia = pg.Ma
	left join T_SanPham sp on pyc.MaSanPham = sp.Ma
	left join T_ThongTinPhu ttp on pyc.MaThongTinPhu = ttp.Ma
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query)
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopPhanCoTheoYeuCauDashBoard<T>(string xuongId)
        {
            var query = @"
SELECT 
	ttp.Ten as TTPhuName,
	lnl.Ten as LoaiTom,
    tc.Ten as TieuChuan,
    s.Ten as Size,
    pyc.VoXoTB,
	sp.Ten as SanPham,
    qt.Ten as QuyTrinh,
    pg.Ten as PhuGia,
    
	pyc.NhuCau as NhuCau,
    p.TrongLuong as TrongLuong,
    cast(
        (ISNULL(p.TrongLuong, 0) / pyc.NhuCau)*100 as DECIMAL(18, 4) 
    ) as TyLeHoanThanh
from (
        Select p.MaLoaiNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.VoXoTB,
            p.MaQuyTrinh,
            p.MaPhuGia,
			p.MaSanPham,
			p.MaThongTinPhu,
            SUM(p.NhuCau) as NhuCau
        from T_PhieuYeuCau p,
		T_PhieuNhomYeuCau n
        where cast(n.NgayTao as date) = cast( DATEADD(DAY, 0, GETDATE()) as date) and p.MaNhomYeuCau = n.Ma and n.SuDung = 1 and p.MaXuong = @xuongId
        GROUP by p.MaLoaiNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.VoXoTB,
            p.MaQuyTrinh,
            p.MaPhuGia,
			p.MaSanPham,
			p.MaThongTinPhu
    ) pyc
    LEFT join (
        Select pyc.MaLoaiNguyenLieu,
            pyc.MaTieuChuan,
            pyc.MaSize,
            pyc.VoXoTB,
            pyc.MaQuyTrinh,
            pyc.MaPhuGia,
			pyc.MaThongTinPhu,
            pyc.MaSanPham,
            SUM(p.TrongLuong) as TrongLuong
        from T_PhieuCanThuMua p,
        T_PhieuYeuCau pyc
        WHERE p.MaPhieuYeuCau in (
                Select DISTINCT Id
                from T_PhieuYeuCau pyc,
				T_PhieuNhomYeuCau n
                where cast(n.NgayTao as date) =  cast( DATEADD(DAY, 0, GETDATE()) as date)  and pyc.MaNhomYeuCau = n.Ma and n.SuDung = 1 and pyc.MaXuong = @xuongId
            ) and p.MaPhieuYeuCau = pyc.Id
        GROUP by pyc.MaLoaiNguyenLieu,
            pyc.MaTieuChuan,
            pyc.MaSize,
            pyc.VoXoTB,
            pyc.MaQuyTrinh,
            pyc.MaPhuGia,
			pyc.MaThongTinPhu,
            pyc.MaSanPham
    ) p on p.MaLoaiNguyenLieu = pyc.MaLoaiNguyenLieu
    and p.MaTieuChuan = pyc.MaTieuChuan
    AND p.MaSize = pyc.MaSize
    and p.VoXoTB = pyc.VoXoTB
    and p.MaQuyTrinh = pyc.MaQuyTrinh
    and p.MaPhuGia = pyc.MaPhuGia
	and p.MaThongTinPhu = pyc.MaThongTinPhu
    and p.MaSanPham = pyc.MaSanPham
    left JOIN T_LoaiNguyenLieu lnl on pyc.MaLoaiNguyenLieu = lnl.Ma
    left JOIN T_TieuChuan tc on pyc.MaTieuChuan = tc.Ma
    left JOIN T_Size s on pyc.MaSize = s.Ma
    left JOIN T_QuyTrinh qt on pyc.MaQuyTrinh = qt.Ma
    LEFT JOIN T_PhuGia pg on pyc.MaPhuGia = pg.Ma
	left join T_SanPham sp on pyc.MaSanPham = sp.Ma
	left join T_ThongTinPhu ttp on pyc.MaThongTinPhu = ttp.Ma

";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query,new{ xuongId})
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopXiNghiepAndTLYeuCauDashBoard<T>()
        {
            var query = @"
Select xn.Ten as Xuong,
    SUM(pyc.NhuCau) as NhuCau
from T_PhieuYeuCau pyc,
    XiNghiep xn ,
		T_PhieuNhomYeuCau n
where  cast(pyc.NgayGioTao as date) =  cast( DATEADD(DAY, 0, GETDATE()) as date)  and pyc.MaNhomYeuCau = n.Ma and n.SuDung = 1
    and xn.Ma = pyc.MaXuong
GROUP BY xn.Ten
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query)
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopsDateTimeToDateTime<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var query = @"
select p.*,
    xn.Ten as XuongName,
    lnl.Ten as LoaiNguyenLieuName,
    tc.Ten as TieuChuanName,
    s.Ten as SizeName,
    bb.Ten as BaoBiName,
    sp.Ten as SanPhamName,
    qt.Ten as QuyTrinhName,
    pg.Ten as PhuGiaName,
    kh.Ten as KhachHangName,
    ks.Ten as KhangSinhName,
    ttp.Ten as ThongTinPhuName,
    cd.Ten as CongDoanName
from (
        select p.Ngay,
            p.MaXuong,
            p.MaNhomLo as MaLo,
            nl.NgayNguyenLieu,
            p.MaLoaiNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.MaBaoBi,
            p.MaSanPham,
            p.MaQuyTrinh,
            p.MaPhuGia,
            p.MaKhachHang,
            p.MaKhangSinh,
            p.MaThongTinPhu,
            p.VoXoTB,
            p.VoXoCD,
            p.VoXoCT,
            sum(p.TrongLuong) as TrongLuong,
            COUNT(*) as SoRo,
            p.MaCongDoan
        from T_PhieuCanThuMua p,
            T_NhomLo nl
        where p.MaNhomLo = nl.Ma
            and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @ngay
            and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
            and SUBSTRING(p.Id, CHARINDEX('.', p.Id) + 1, CHARINDEX('.', p.Id, CHARINDEX('.', p.Id) + 1) - CHARINDEX('.', p.Id) - 1) =@xuongId  --p.MaXuong = @xuongId
        GROUP BY p.Ngay,
            p.MaNhomLo,
            p.MaCongDoan,
            nl.NgayNguyenLieu,
            p.MaXuong,
            p.MaLoaiNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.MaBaoBi,
            p.MaSanPham,
            p.MaQuyTrinh,
            p.MaPhuGia,
            p.MaKhachHang,
            p.MaKhangSinh,
            p.MaThongTinPhu,
            p.VoXoTB,
            p.VoXoCD,
            p.VoXoCT
    ) p
    left join T_LoaiNguyenLieu lnl on lnl.Ma = p.MaLoaiNguyenLieu
    left join T_TieuChuan tc on tc.Ma = p.MaTieuChuan
    left join T_Size s on s.Ma = p.MaSize
    left join T_BaoBi bb on bb.Ma = p.MaBaoBi
    left join T_SanPham sp on sp.Ma = p.MaSanPham
    left join T_QuyTrinh qt on qt.Ma = p.MaQuyTrinh
    left join T_PhuGia pg on pg.Ma = p.MaPhuGia
    left join T_KhachHang kh on kh.Ma = p.MaKhachHang
    left join T_KhangSinh ks on ks.Ma = p.MaKhangSinh
    left join T_ThongTinPhu ttp on ttp.Ma = p.MaThongTinPhu
    left join XiNghiep xn on xn.Ma = p.MaXuong
    left join T_CongDoan cd on cd.Ma = p.MaCongDoan
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate, ngay = toDate, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetTongHopsDateTimeToDateTimeNgayNguyenLieu<T>(DateTime fromDate, DateTime toDate,
            string xuongId)
        {
            var query = @"
select p.*,
    xn.Ten as XuongName,
    lnl.Ten as LoaiNguyenLieuName,
    tc.Ten as TieuChuanName,
    s.Ten as SizeName,
    bb.Ten as BaoBiName,
    sp.Ten as SanPhamName,
    qt.Ten as QuyTrinhName,
    pg.Ten as PhuGiaName,
    kh.Ten as KhachHangName,
    ks.Ten as KhangSinhName,
    ttp.Ten as ThongTinPhuName,
    cd.Ten as CongDoanName
from (
        select p.Ngay,
            p.MaXuong,
            p.MaNhomLo as MaLo,
            nl.NgayNguyenLieu,
            p.MaLoaiNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.MaBaoBi,
            p.MaSanPham,
            p.MaQuyTrinh,
            p.MaPhuGia,
            p.MaKhachHang,
            p.MaKhangSinh,
            p.MaThongTinPhu,
            p.VoXoTB,
            p.VoXoCD,
            p.VoXoCT,
            sum(p.TrongLuong) as TrongLuong,
            COUNT(*) as SoRo,
            p.MaCongDoan
        from T_PhieuCanThuMua p,
            T_NhomLo nl
        where p.MaNhomLo = nl.Ma
            and p.NgayNguyenLieu <= @ngay
            and p.NgayNguyenLieu>= @fromDate
            and SUBSTRING(p.Id, CHARINDEX('.', p.Id) + 1, CHARINDEX('.', p.Id, CHARINDEX('.', p.Id) + 1) - CHARINDEX('.', p.Id) - 1)=@xuongId  --p.MaXuong = @xuongId
        GROUP BY p.Ngay,
            p.MaNhomLo,
            p.MaCongDoan,
            nl.NgayNguyenLieu,
            p.MaXuong,
            p.MaLoaiNguyenLieu,
            p.MaTieuChuan,
            p.MaSize,
            p.MaBaoBi,
            p.MaSanPham,
            p.MaQuyTrinh,
            p.MaPhuGia,
            p.MaKhachHang,
            p.MaKhangSinh,
            p.MaThongTinPhu,
            p.VoXoTB,
            p.VoXoCD,
            p.VoXoCT
    ) p
    left join T_LoaiNguyenLieu lnl on lnl.Ma = p.MaLoaiNguyenLieu
    left join T_TieuChuan tc on tc.Ma = p.MaTieuChuan
    left join T_Size s on s.Ma = p.MaSize
    left join T_BaoBi bb on bb.Ma = p.MaBaoBi
    left join T_SanPham sp on sp.Ma = p.MaSanPham
    left join T_QuyTrinh qt on qt.Ma = p.MaQuyTrinh
    left join T_PhuGia pg on pg.Ma = p.MaPhuGia
    left join T_KhachHang kh on kh.Ma = p.MaKhachHang
    left join T_KhangSinh ks on ks.Ma = p.MaKhangSinh
    left join T_ThongTinPhu ttp on ttp.Ma = p.MaThongTinPhu
    left join XiNghiep xn on xn.Ma = p.MaXuong
    left join T_CongDoan cd on cd.Ma = p.MaCongDoan
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate = fromDate.Date, ngay = toDate, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }

        public int Insert<T>(T item)
        {
            var query = queryInsert;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, item);
            return rows;
        }

        public int Insert<T>(List<T> items)
        {
            var query = queryInsert;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, items);
            return rows;
        }

        //public int InsertBatch<T>(List<T> items,string tableName = null)
        //{
        //    var batches = DbExtensions.GetSqlsInBatches(items,500,null,tableName);
        //    var row = 0;
        //    var database = new Database(ConnectionString);
        //    foreach (var batche in batches) row += database.ExecuteNonQuery(batche);

        //    return row;
        //}

        public int Update<T>(T item)
        {
            var query = queryUpdate;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, item);
            return rows;
        }
        public int Update(string ids,int status )
        {
            var query = string.Format(queryUpdateStatusByIds,ids);
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, new {status});
            return rows;
        }
        public int Update<T>(List<T> items)
        {
            var query = queryUpdate;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, items);
            return rows;
        }

        public int UpdateDatabase()
        {
            try
            {
                //Thêm Cột Id Vào bảng MaLoaiCa trên máy cân đầu Ao
                var query = queryUpdateDb;
                var dao = new Database(connectionString);
                return dao.ExecuteNonQuery(query);
            }
            catch (Exception exception)
            {
                throw new Exception(
                    $@"Không thể cập nhật Cơ Sở Dữ Liệu vui lòng liên hệ IT để được hổ trợ [T_PhieuCanThuMua]. {Environment.NewLine}{exception.Message}");
                //throw;
            }
        }
        public List<T> GetNgayAndNguyenLieuPhanCo<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
        )
        {
            var query = @"
select 
    MAX(CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) )as MaxNgayGio,
   MIN(CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) )as MinNgayGio,
    MAX(p.NgayNguyenLieu) as MaxNgayNguyenLieu,
    MIN(p.NgayNguyenLieu) as MinNgayNguyenLieu
from T_PhieuCanThuMua p
where CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @ngay
    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
    and p.MaXuong = @xuongId";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate = fromDate, ngay = toDate, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetNgayAndNguyenLieuPhanCo_NgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
        )
        {
            var query = @"
select 
    MAX(CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) )as MaxNgayGio,
    MIN(CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) )as MinNgayGio,
    MAX(p.NgayNguyenLieu) as MaxNgayNguyenLieu,
    MIN(p.NgayNguyenLieu) as MinNgayNguyenLieu
from T_PhieuCan p
where p.NgayNguyenLieu <= @ngay
    and p.NgayNguyenLieu >= @fromDate
    and p.MaXuong = @xuongId";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate = fromDate, ngay = toDate, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopBaoCaoSauRaiMayPhanCo<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
           )
        {
            var query = @"
if object_id('tempdb..#phieuCan', 'U') is not null drop table #phieuCan
Select distinct ISNULL(p.MaBaoBi, '') as MaSizeTP,
    ISNULL(p.VoXoCD, '') +'-'+ISNULL(p.VoXoCT, '') as VoXo2,
    ISNULL(pn.Ten, '') as MaPhieuPhanCo,
    ISNULL(p.MaQuyTrinh, '') as MaQuyTrinh,
    ISNULL(p.MaKhachHang, '') as MaKhachHang,
    ISNULL(p.MaKhangSinh, '') as MaKhangSinh,
    SUM(p.TrongLuong) as TrongLuong,
    ISNULL(nl.Ten, '') as MaDaiLy into #phieuCan
from T_PhieuCanThuMua p,
    T_PhieuNhomYeuCau pn,
    T_PhieuYeuCau pyc,
    (
        Select p.Ma,
case
                when LEN(p.Ten) > 3 then SUBSTRING(p.Ten, 1, 3)
                else p.Ten
            end as Ten
        from T_NhomLo p
    ) nl
where CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @ngay
    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaNhomLo = nl.Ma
    and p.MaPhieuYeuCau = pyc.Id
    and pyc.MaNhomYeuCau = pn.Ma
GROUP BY p.MaBaoBi,
    p.VoXoCD,
    p.VoXoCT,
    p.MaPhieuYeuCau,
    p.MaQuyTrinh,
    p.MaKhachHang,
    nl.Ten,
    pn.Ten,
   p. MaKhangSinh
SELECT ROW_NUMBER() OVER (
        ORDER BY t.MaSizeTP,
            t.VoXo2,
            t.MaPhieuPhanCo,
            t.MaQuyTrinh,
            t.MaKhachHang,
            t.MaKhangSinh
    ) AS STT,
    MaSizeTP,
    CONCAT('''', s.Ten) as SizeTPName,
    VoXo2,
    MaPhieuPhanCo,
    CONCAT('''', t.MaPhieuPhanCo) as PhieuPhanCoName,
    MaQuyTrinh,
    qt.Ten as QuyTrinhName,
    MaKhachHang,
    kh.Ten as KhachHangName,
    MaKhangSinh,
    ks.Ten as KhangSinhName,
    SUM(TrongLuong) as TrongLuong,
    STUFF(
        (
            SELECT '; ' + MaDaiLy
            FROM #phieuCan
            WHERE MaSizeTP = t.MaSizeTP
                AND VoXo2 = t.VoXo2
                AND MaPhieuPhanCo = t.MaPhieuPhanCo
                AND MaQuyTrinh = t.MaQuyTrinh
                AND MaKhachHang = t.MaKhachHang
                AND MaKhangSinh = t.MaKhangSinh FOR XML PATH('')
        ),
        1,
        2,
        ''''
    ) AS DanhSachDaiLy
FROM #phieuCan AS t
    LEFT JOIN T_BaoBi s on MaSizeTP = s.Ma
    LEFT JOIN T_QuyTrinh qt on MaQuyTrinh = qt.Ma
    LEFT JOIN T_KhachHang kh on MaKhachHang = kh.Ma
    LEFT JOIN T_KhangSinh ks on MaKhangSinh = ks.Ma
GROUP BY MaSizeTP,
    VoXo2,
    MaPhieuPhanCo,
    MaQuyTrinh,
    MaKhachHang,
    MaKhangSinh,
    s.Ten,
    qt.Ten,
    kh.Ten,
    ks.Ten
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate = fromDate, ngay = toDate, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
           )
        {
            var query = @"
if object_id('tempdb..#phieuCan', 'U') is not null drop table #phieuCan
Select distinct ISNULL(p.MaBaoBi, '') as MaSizeTP,
    ISNULL(p.VoXoCD, '') +'-'+ISNULL(p.VoXoCT, '') as VoXo2,
    ISNULL(pn.Ten, '') as MaPhieuPhanCo,
    ISNULL(p.MaQuyTrinh, '') as MaQuyTrinh,
    ISNULL(p.MaKhachHang, '') as MaKhachHang,
    ISNULL(p.MaKhangSinh, '') as MaKhangSinh,
    SUM(p.TrongLuong) as TrongLuong,
    ISNULL(nl.Ten, '') as MaDaiLy into #phieuCan
from T_PhieuCanThuMua p,
    T_PhieuNhomYeuCau pn,
    T_PhieuYeuCau pyc,
    (
        Select p.Ma,
case
                when LEN(p.Ten) > 3 then SUBSTRING(p.Ten, 1, 3)
                else p.Ten
            end as Ten
        from T_NhomLo p
    ) nl
where p.NgayNguyenLieu <= @ngay
    and p.NgayNguyenLieu >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaNhomLo = nl.Ma
    and p.MaPhieuYeuCau = pyc.Id
    and pyc.MaNhomYeuCau = pn.Ma
GROUP BY p.MaBaoBi,
    p.VoXoCD,
    p.VoXoCT,
    p.MaPhieuYeuCau,
    p.MaQuyTrinh,
    p.MaKhachHang,
    nl.Ten,
    pn.Ten,
   p. MaKhangSinh
SELECT ROW_NUMBER() OVER (
        ORDER BY t.MaSizeTP,
            t.VoXo2,
            t.MaPhieuPhanCo,
            t.MaQuyTrinh,
            t.MaKhachHang,
            t.MaKhangSinh
    ) AS STT,
    MaSizeTP,
    CONCAT('''', s.Ten) as SizeTPName,
    VoXo2,
    MaPhieuPhanCo,
    CONCAT('''', t.MaPhieuPhanCo) as PhieuPhanCoName,
    MaQuyTrinh,
    qt.Ten as QuyTrinhName,
    MaKhachHang,
    kh.Ten as KhachHangName,
    MaKhangSinh,
    ks.Ten as KhangSinhName,
    SUM(TrongLuong) as TrongLuong,
    STUFF(
        (
            SELECT '; ' + MaDaiLy
            FROM #phieuCan
            WHERE MaSizeTP = t.MaSizeTP
                AND VoXo2 = t.VoXo2
                AND MaPhieuPhanCo = t.MaPhieuPhanCo
                AND MaQuyTrinh = t.MaQuyTrinh
                AND MaKhachHang = t.MaKhachHang
                AND MaKhangSinh = t.MaKhangSinh FOR XML PATH('')
        ),
        1,
        2,
        ''''
    ) AS DanhSachDaiLy
FROM #phieuCan AS t
    LEFT JOIN T_BaoBi s on MaSizeTP = s.Ma
    LEFT JOIN T_QuyTrinh qt on MaQuyTrinh = qt.Ma
    LEFT JOIN T_KhachHang kh on MaKhachHang = kh.Ma
    LEFT JOIN T_KhangSinh ks on MaKhangSinh = ks.Ma
GROUP BY MaSizeTP,
    VoXo2,
    MaPhieuPhanCo,
    MaQuyTrinh,
    MaKhachHang,
    MaKhangSinh,
    s.Ten,
    qt.Ten,
    kh.Ten,
    ks.Ten
";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate = fromDate, ngay = toDate, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }
         public List<T> GetTongHopNhanViens3DateTimeToDateTimeNSRC<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isNhom,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true
            )
        {
//           var query = @"Select 
//    nv.MaNhanVien,
//    nv.Name as TenNhanVien,
//    nv.MaHoSo,
//    nv.DeptName0 as Nhom,
//    nv.Xuong,
//    kh.Ten as KhachHangName,
//    p.CongViecName,
//    p.Ngay,
//    p.NgayNguyenLieu,
//    p.SanPhamName,
//    p.QuyTrinhName,
//    p.IsDatKhangSinh,
//    p.MaSize,
//    p.SizeName,
//    p.MaThanhPham,
//    p.NhomCongViecName,
//    p.MaCongViec,
//    p.MaLo,
//    p.VoXo,
//    p.VoXo2,
//    p.TrongLuong,
//    p.SoRo,
//    p.MayCan,
//    p.MaXuong,
//    p.MaKhachHang,
//    p.KhangSinhName,
//    p.ThongTinPhuName,
//    p.ThongTinNguyenLieuName,
//    p.PhuGiaName,
//    p.GioBatDauLoKH,
//    p.LoaiPhieuPhanCo,
//    p.MaPhieuPhanCo,
//    p.LoaiQuyTrinh
//from (
//        Select nv.MaNhanVien,
//            nv.Name,
//            nv.MaHoSo,
//            nv.DeptName0,
//            nv.Xuong
//        from NhanVienDaiThanh nv
//        where nv.IsContracting = 1
//    ) NV
//    LEFT JOIN (
//        Select p.Ngay,
//            p.MaNhanVien as MNV,
//            nl.Ten as NhomLo,
//            nl.NgayNguyenLieu,
//            sp.Ten as SanPhamName,
//            qt.Ten as QuyTrinhName,
//            p.IsDatKhangSinh,
//            p.MaSize,
//            s.Ten as SizeName,
//            p.MaThanhPham,
//            tp.Ten as NhomCongViecName,
//            p.MaCongDoan as MaCongViec,
//            cd.Ten as CongViecName,
//            p.MaLo,
//            p.VoXo,
//            p.VoXo2,
//            p.TrongLuong,
//            p.SoRo,
//            p.MayCan,
//            p.MaXuong,
//            p.MaKhachHang,
//            ks.Ten as KhangSinhName,
//            ttp.Ten as ThongTinPhuName,
//            ttnl.Ten as ThongTinNguyenLieuName,
//            pg.Ten as PhuGiaName,
//            pHC.GioBatDauLoKH,
//            ppc.LoaiPhieuPhanCo,
//            ppc.Ma as MaPhieuPhanCo,
//            qt.LoaiQuyTrinh
//        from (
//                select p.Ngay,
//                    p.MaNhanVien,
//                    p.MaNhomLo,
//                    p.MaSanPham,
//                    p.MaQuyTrinh,
//                    1 as IsDatKhangSinh,
//                    p.MaSize,
//                    '' as MaThanhPham,
//                    p.MaCongDoan,
//                    p.MaLo,
//                    p.VoXoTB as VoXo,
//                    p.VoXoTB as VoXo2,
//                    sum(p.TrongLuong) as TrongLuong,
//                    COUNT(*) as SoRo,
//                    p.MayCan,
//                    p.MaXuong,
//                    p.MaKhachHang,
//                    p.MaKhangSinh,
//                    p.MaThongTinPhu,
//                    '' as MaTrangThaiNguyenLieu,
//                    p.MaPhuGia,
//                    '' as MaPhieuPhanCo
//                from T_PhieuCanThuMua p,
//                    T_NhomLo nl,
//                    NhanVienDaiThanh nv
//                where p.MaNhomLo = nl.Ma
//                    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
//                    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @toDate
//                    and SUBSTRING(
//                        p.Id,
//                        CHARINDEX('.', p.Id) + 1,
//                        CHARINDEX('.', p.Id, CHARINDEX('.', p.Id) + 1) - CHARINDEX('.', p.Id) - 1
//                    ) = @xuongId
//                    and nv.MaNhanVien = p.MaNhanVien
//                    and nv.IsNhom = @isNhom
//                    and p.TrongLuong > 0
//                    and nv.MaNhanVien in (
//                        Select distinct MaNhanVien
//                        from T_PhieuCanThuMua
//                    )
//                GROUP BY p.Ngay,
//                    p.MaNhanVien,
//                    p.MaNhomLo,
//                    p.MaSanPham,
//                    p.MaQuyTrinh,
//                    p.MaSize,
//                    p.MaCongDoan,
//                    p.MaLo,
//                    p.VoXoTB,
//                    p.MayCan,
//                    p.MaXuong,
//                    p.MaKhachHang,
//                    p.MaKhangSinh,
//                    p.MaThongTinPhu,
//                    p.MaPhuGia
//            ) p
//            left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
//            left join T_ThanhPham tp on p.MaThanhPham = tp.Ma
//            left join T_Size s on p.MaSize = s.Ma
//            left join T_NhomLo nl on p.MaNhomLo = nl.Ma
//            left join T_SanPham sp on p.MaSanPham = sp.Ma
//            left join T_CongDoan cd on p.MaCongDoan = cd.Ma
//            left join T_QuyTrinh qt on p.MaQuyTrinh = qt.Ma
//            left join T_KhangSinh ks on p.MaKhangSinh = ks.Ma
//            left join T_ThongTinPhu ttp on p.MaThongTinPhu = ttp.Ma
//            left join T_TrangThaiNguyenLieu ttnl on p.MaTrangThaiNguyenLieu = ttnl.Ma
//            left join T_PhuGia pg on p.MaPhuGia = pg.Ma
//            left join (
//                select p.Ngay,
//                    p.MaNhomLo,
//                    p.MaKhachHang,
//                    Min(p.Gio) as GioBatDauLoKH
//                from T_PhieuCanThuMua p
//                GROUP BY p.Ngay,
//                    p.MaNhomLo,
//                    p.MaKhachHang
//            ) pHC on p.Ngay = pHC.Ngay
//            and p.MaNhomLo = phc.MaNhomLo
//            and p.MaKhachHang = pHC.MaKhachHang
//            LEFT JOIN T_PhieuPhanCo ppc on p.MaPhieuPhanCo = ppc.Ma
//    ) p on NV.MaNhanVien = p.MNV
//    LEFT JOIN T_ChiTietVoXo ctvx ON p.LoaiPhieuPhanCo = ctvx.MaPhieuPhanCo
//    and p.VoXo = ctvx.VoXo
//    and p.VoXo2 = ctvx.VoXo2
//    and p.LoaiQuyTrinh = ctvx.MaQuyTrinh
//    and p.MaSize = ctvx.MaSize
//    and p.MaCongViec = ctvx.MaCongDoan
//    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
//order by NV.MaHoSo DESC";
            var query = @"IF OBJECT_ID(N'tempdb..#p') IS NOT NULL BEGIN DROP TABLE #p
END IF OBJECT_ID(N'tempdb..#HC') IS NOT NULL BEGIN DROP TABLE #HC
END
select p.Ngay,
    p.MaNhomLo,
    p.MaKhachHang,
    Min(p.Gio) as GioBatDauLoKH into #HC
from T_PhieuCanThuMua p,
    (
        select distinct p.Ngay,
            p.MaNhomLo,
            p.MaKhachHang
        from T_PhieuCanThuMua p,
            NhanVienDaiThanh nv
        where CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
            and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @toDate
            and SUBSTRING(
                p.Id,
                CHARINDEX('.', p.Id) + 1,
                CHARINDEX('.', p.Id, CHARINDEX('.', p.Id) + 1) - CHARINDEX('.', p.Id) - 1
            ) = @xuongId
            and nv.MaNhanVien = p.MaNhanVien
            and nv.IsNhom = @isNhom
            and p.TrongLuong > 0
    ) p2
where p.Ngay = p2.Ngay
    and p.MaNhomLo = p2.MaNhomLo
    and p.MaKhachHang = p2.MaKhachHang
GROUP BY p.Ngay,
    p.MaNhomLo,
    p.MaKhachHang
Select p.*,
    sp.Ten as SanPhamName,
    qt.Ten as QuyTrinhName,
    s.Ten as SizeName,
    tp.Ten as NhomCongViecName,
    p.MaCongDoan as MaCongViec,
    cd.Ten as CongViecName,
    ks.Ten as KhangSinhName,
    ttp.Ten as ThongTinPhuName,
    ttnl.Ten as ThongTinNguyenLieuName,
    pg.Ten as PhuGiaName,
    qt.LoaiQuyTrinh into #p
from (
        select p.Ngay,
            p.MaNhanVien,
            p.MaNhomLo,
            p.MaSanPham,
            p.MaQuyTrinh,
            1 as IsDatKhangSinh,
            p.MaSize,
            '' as MaThanhPham,
            p.MaCongDoan,
            p.MaLo,
            p.VoXoTB as VoXo,
            p.VoXoTB as VoXo2,
            sum(p.TrongLuong) as TrongLuong,
            COUNT(*) as SoRo,
            p.MayCan,
            p.MaXuong,
            p.MaKhachHang,
            p.MaKhangSinh,
            p.MaThongTinPhu,
            '' as MaTrangThaiNguyenLieu,
            p.MaPhuGia,
            '' as MaPhieuPhanCo,
            nl.Ten as NhomLo,
            nl.NgayNguyenLieu
        from T_PhieuCanThuMua p,
            T_NhomLo nl,
            NhanVienDaiThanh nv
        where p.MaNhomLo = nl.Ma
            and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
            and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @toDate
            and SUBSTRING(
                p.Id,
                CHARINDEX('.', p.Id) + 1,
                CHARINDEX('.', p.Id, CHARINDEX('.', p.Id) + 1) - CHARINDEX('.', p.Id) - 1
            ) = @xuongId
            and nv.MaNhanVien = p.MaNhanVien
            and nv.IsNhom = @isNhom
            and p.TrongLuong > 0
            and nv.MaNhanVien in (
                 Select distinct p.MaNhanVien
                 from T_PhieuCanThuMua p,
            T_NhomLo nl,
            NhanVienDaiThanh nv
        where p.MaNhomLo = nl.Ma
            and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
            and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @toDate
            and SUBSTRING(
                p.Id,
                CHARINDEX('.', p.Id) + 1,
                CHARINDEX('.', p.Id, CHARINDEX('.', p.Id) + 1) - CHARINDEX('.', p.Id) - 1
            ) = @xuongId
            and nv.MaNhanVien = p.MaNhanVien
            and nv.IsNhom = @isNhom
            and p.TrongLuong > 0
            )
        GROUP BY p.Ngay,
            p.MaNhanVien,
            p.MaNhomLo,
            p.MaSanPham,
            p.MaQuyTrinh,
            p.MaSize,
            p.MaCongDoan,
            p.MaLo,
            p.VoXoTB,
            p.MayCan,
            p.MaXuong,
            p.MaKhachHang,
            p.MaKhangSinh,
            p.MaThongTinPhu,
            p.MaPhuGia,
            nl.ten,
            nl.NgayNguyenLieu
    ) p
    left join T_ThanhPham tp on p.MaThanhPham = tp.Ma
    left join T_Size s on p.MaSize = s.Ma
    left join T_SanPham sp on p.MaSanPham = sp.Ma
    left join T_CongDoan cd on p.MaCongDoan = cd.Ma
    left join T_QuyTrinh qt on p.MaQuyTrinh = qt.Ma
    left join T_KhangSinh ks on p.MaKhangSinh = ks.Ma
    left join T_ThongTinPhu ttp on p.MaThongTinPhu = ttp.Ma
    left join T_TrangThaiNguyenLieu ttnl on p.MaTrangThaiNguyenLieu = ttnl.Ma
    left join T_PhuGia pg on p.MaPhuGia = pg.Ma
Select nv.MaNhanVien,
    nv.Name as TenNhanVien,
    nv.MaHoSo,
    nv.DeptName0 as Nhom,
    nv.Xuong,
    kh.Ten as KhachHangName,
    p.CongViecName,
    p.Ngay,
    p.NgayNguyenLieu,
    p.SanPhamName,
    p.QuyTrinhName,
    p.IsDatKhangSinh,
    p.MaSize,
    p.SizeName,
    p.MaThanhPham,
    p.NhomCongViecName,
    p.MaCongViec,
    p.MaLo,
    p.VoXo,
    p.VoXo2,
    p.TrongLuong,
    p.SoRo,
    p.MayCan,
    p.MaXuong,
    p.MaKhachHang,
    p.KhangSinhName,
    p.ThongTinPhuName,
    p.ThongTinNguyenLieuName,
    p.PhuGiaName,
    p.GioBatDauLoKH,
    p.LoaiPhieuPhanCo,
    p.MaPhieuPhanCo2 AS MaPhieuPhanCo,
    p.LoaiQuyTrinh
from (
        Select nv.MaNhanVien,
            nv.Name,
            nv.MaHoSo,
            nv.DeptName0,
            nv.Xuong
        from NhanVienDaiThanh nv
        where nv.IsContracting = 1
    ) NV
    LEFT JOIN (
        Select p.*,
            p.MaNhanVien as MNV,
            h.GioBatDauLoKH,
            ppc.LoaiPhieuPhanCo,
            ppc.Ma as MaPhieuPhanCo2
        from #p p LEFT JOIN #HC h ON p.Ngay = h.Ngay AND p.MaNhomLo = h.MaNhomLo AND p.MaKhachHang = h.MaKhachHang
            LEFT JOIN T_PhieuPhanCo ppc on p.MaPhieuPhanCo = ppc.Ma
    ) p on NV.MaNhanVien = p.MNV
    LEFT JOIN T_ChiTietVoXo ctvx ON p.LoaiPhieuPhanCo = ctvx.MaPhieuPhanCo
    and p.VoXo = ctvx.VoXo
    and p.VoXo2 = ctvx.VoXo2
    and p.LoaiQuyTrinh = ctvx.MaQuyTrinh
    and p.MaSize = ctvx.MaSize
    and p.MaCongViec = ctvx.MaCongDoan
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
order by NV.MaHoSo DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate = fromDate, toDate = toDate, xuongId, khuVuc , isNhom = isNhom })
                    .Result
                    .ToList();
                return items;
            }
        }
          public List<T> GetTongHopNhanViens3ToNgayNguyenLieuNSRC<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isNhom,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var query = @"Select 
    nv.MaNhanVien,
    nv.Name as TenNhanVien,
    nv.MaHoSo,
    nv.DeptName0 as Nhom,
    nv.Xuong,
    kh.Ten as KhachHangName,

p.CongViecName,
    p.Ngay,
    p.NgayNguyenLieu,
    p.SanPhamName,
    p.QuyTrinhName,
    p.IsDatKhangSinh,
    p.MaSize,
    p.SizeName,
    p.MaThanhPham,
    p.NhomCongViecName,
    p.MaCongViec,
    p.MaLo,
    p.VoXo,
    p.VoXo2,
    p.TrongLuong,
    p.SoRo,
    p.MayCan,
    p.MaXuong,
    p.MaKhachHang,
    p.KhangSinhName,
    p.ThongTinPhuName,
    p.ThongTinNguyenLieuName,
    p.PhuGiaName,
    p.GioBatDauLoKH,
    p.LoaiPhieuPhanCo,
    p.MaPhieuPhanCo,
    p.LoaiQuyTrinh
from (
        Select nv.MaNhanVien,
            nv.Name,
            nv.MaHoSo,
            nv.DeptName0,
            nv.Xuong
        from NhanVienDaiThanh nv
        where nv.IsContracting = 1
    ) NV
    LEFT JOIN (
        Select p.Ngay,
            p.MaNhanVien as MNV,
            nl.Ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qt.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            p.MaSize,
            s.Ten as SizeName,
            p.MaThanhPham,
            tp.Ten as NhomCongViecName,
            p.MaCongDoan as MaCongViec,
            cd.Ten as CongViecName,
            p.MaLo,
            p.VoXo,
            p.VoXo2,
            p.TrongLuong,
            p.SoRo,
            p.MayCan,
            p.MaXuong,
            p.MaKhachHang,
            ks.Ten as KhangSinhName,
            ttp.Ten as ThongTinPhuName,
            ttnl.Ten as ThongTinNguyenLieuName,
            pg.Ten as PhuGiaName,
            pHC.GioBatDauLoKH,
            ppc.LoaiPhieuPhanCo,
            ppc.Ma as MaPhieuPhanCo,
            qt.LoaiQuyTrinh
        from (
                select p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    1 as IsDatKhangSinh,
                    p.MaSize,
                    '' as MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXoTB as VoXo,
                    p.VoXoTB as VoXo2,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    '' as MaTrangThaiNguyenLieu,
                    p.MaPhuGia,
                    '' as MaPhieuPhanCo
                from T_PhieuCanThuMua p,
                    T_NhomLo nl,
                    NhanVienDaiThanh nv
                where p.MaNhomLo = nl.Ma
                    and nl.NgayNguyenLieu >= @fromDate
                    and nl.NgayNguyenLieu <= @toDate
                    and SUBSTRING(
                        p.Id,
                        CHARINDEX('.', p.Id) + 1,
                        CHARINDEX('.', p.Id, CHARINDEX('.', p.Id) + 1) - CHARINDEX('.', p.Id) - 1
                    ) = @xuongId
                    and nv.MaNhanVien = p.MaNhanVien
                    and nv.IsNhom = @isNhom
                    and p.TrongLuong > 0
                    and nv.MaNhanVien in (
                        Select distinct MaNhanVien
                        from T_PhieuCanThuMua
                    )
                GROUP BY p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.MaSize,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXoTB,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaPhuGia
            ) p
            left join NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            left join T_ThanhPham tp on p.MaThanhPham = tp.Ma
            left join T_Size s on p.MaSize = s.Ma
            left join T_NhomLo nl on p.MaNhomLo = nl.Ma
            left join T_SanPham sp on p.MaSanPham = sp.Ma
            left join T_CongDoan cd on p.MaCongDoan = cd.Ma
            left join T_QuyTrinh qt on p.MaQuyTrinh = qt.Ma
            left join T_KhangSinh ks on p.MaKhangSinh = ks.Ma
            left join T_ThongTinPhu ttp on p.MaThongTinPhu = ttp.Ma
            left join T_TrangThaiNguyenLieu ttnl on p.MaTrangThaiNguyenLieu = ttnl.Ma
            left join T_PhuGia pg on p.MaPhuGia = pg.Ma
            left join (
                select p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang,
                    Min(p.Gio) as GioBatDauLoKH
                from T_PhieuCanThuMua p
                GROUP BY p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang
            ) pHC on p.Ngay = pHC.Ngay
            and p.MaNhomLo = phc.MaNhomLo
            and p.MaKhachHang = pHC.MaKhachHang
            LEFT JOIN T_PhieuPhanCo ppc on p.MaPhieuPhanCo = ppc.Ma
    ) p on NV.MaNhanVien = p.MNV
    LEFT JOIN T_ChiTietVoXo ctvx ON p.LoaiPhieuPhanCo = ctvx.MaPhieuPhanCo
    and p.VoXo = ctvx.VoXo
    and p.VoXo2 = ctvx.VoXo2
    and p.LoaiQuyTrinh = ctvx.MaQuyTrinh
    and p.MaSize = ctvx.MaSize
    and p.MaCongViec = ctvx.MaCongDoan
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
order by NV.MaHoSo DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, khuVuc, isNhom = isNhom })
                    .Result
                    .ToList();
                return items;
            }
        }
    }
}
