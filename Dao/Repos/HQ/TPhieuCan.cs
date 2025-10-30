using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Dao.Repos.HQ
{
    public partial class TPhieuCan
    {
        private readonly string connectionString;
        private string tableName = @"T_PhieuCan";
        private readonly string qrDelete = @"DELETE FROM [dbo].[T_PhieuCan]
      WHERE [Ma] = @Ma
";

        private readonly string qrInsert = @"INSERT INTO [dbo].[T_PhieuCan] (
        [Ma],
        [STT],
        [MayCan],
        [Ngay],
        [NgayNguyenLieu],
        [Gio],
        [MaLoaiNguyenLieu],
        [MaSize],
        [MaThanhPham],
        [MaLo],
        [MaNhomLo],
        [MaNhanVien],
        [MaGroup],
        [MaThe],
        [MaLoaiKhuon],
        [MaQuyCach],
        [MaLoaiCan],
        [MaXuong],
        [TrongLuong],
        [TrongLuongNhan],
        [TrongLuongTare],
        [DinhMuc],
        [SuDung],
        [GhiChu],
        [MaNhaCungCap],
        [MaPhuongTien],
        [MaKhuVuc],
        [MaLenhSanXuat],
        [MaNhanVienPhucVu],
        [MaLoaiCongViec],
        [IsEnabled],
        [MaNhanVien2],
        [MaSanPham],
        [MaCongDoan],
        [MaBon],
        [Luot],
        [MaQuyTrinh],
        [MaDonHang],
        [MaKhachHang],
        [IsDatKhangSinh],
        [MaKhangSinh],
        [VoXo],
        [MaThongTinPhu],
        [MaPhuGia],
        [MaTrangThaiNguyenLieu],
        [MaQuyTrinhOrg],
        [IsNgam],
        [IsCanTay],
        [T],
        [MaTyLeNhomHoaChat],
        [SoLuong],
        [TrongLuongDonVi],
        [MaKhachHangOrg],
        [MaSizeOrg],
        [MaPhuGiaOrg],
        [MaNhomHoaChat],
        [MaPhieuPhanCo],[GhiChu2],[GhiChu3],[VoXo2],[MaSizeTP]
    )
VALUES (
        @Ma,
        @STT,
        @MayCan,
        @Ngay,
        @NgayNguyenLieu,
        @Gio,
        @MaLoaiNguyenLieu,
        @MaSize,
        @MaThanhPham,
        @MaLo,
        @MaNhomLo,
        @MaNhanVien,
        @MaGroup,
        @MaThe,
        @MaLoaiKhuon,
        @MaQuyCach,
        @MaLoaiCan,
        @MaXuong,
        @TrongLuong,
        @TrongLuongNhan,
        @TrongLuongTare,
        @DinhMuc,
        @SuDung,
        @GhiChu,
        @MaNhaCungCap,
        @MaPhuongTien,
        @MaKhuVuc,
        @MaLenhSanXuat,
        @MaNhanVienPhucVu,
        @MaLoaiCongViec,
        @IsEnabled,
        @MaNhanVien2,
        @MaSanPham,
        @MaCongDoan,
        @MaBon,
        @Luot,
        @MaQuyTrinh,
        @MaDonHang,
        @MaKhachHang,
        @IsDatKhangSinh,
        @MaKhangSinh,
        @VoXo,
        @MaThongTinPhu,
        @MaPhuGia,
        @MaTrangThaiNguyenLieu,
        @MaQuyTrinhOrg,
        @IsNgam,
        @IsCanTay,
        @T,
        @MaTyLeNhomHoaChat,
        @SoLuong,
        @TrongLuongDonVi,
        @MaKhachHangOrg,
        @MaSizeOrg,
        @MaPhuGiaOrg,
        @MaNhomHoaChat,
        @MaPhieuPhanCo,@GhiChu2,@GhiChu3,@VoXo2,@MaSizeTP
    )";


        private readonly string qrUpdate = @"UPDATE [dbo].[T_PhieuCan]
SET [STT] = @STT,
    [MayCan] = @MayCan,
    [Ngay] = @Ngay,
    [NgayNguyenLieu] = @NgayNguyenLieu,
    [Gio] = @Gio,
    [MaLoaiNguyenLieu] = @MaLoaiNguyenLieu,
    [MaSize] = @MaSize,
    [MaThanhPham] = @MaThanhPham,
    [MaLo] = @MaLo,
    [MaNhomLo] = @MaNhomLo,
    [MaNhanVien] = @MaNhanVien,
    [MaGroup] = @MaGroup,
    [MaThe] = @MaThe,
    [MaLoaiKhuon] = @MaLoaiKhuon,
    [MaQuyCach] = @MaQuyCach,
    [MaLoaiCan] = @MaLoaiCan,
    [MaXuong] = @MaXuong,
    [TrongLuong] = @TrongLuong,
    [TrongLuongNhan] = @TrongLuongNhan,
    [TrongLuongTare] = @TrongLuongTare,
    [DinhMuc] = @DinhMuc,
    [SuDung] = @SuDung,
    [GhiChu] = @GhiChu,
    [MaNhaCungCap] = @MaNhaCungCap,
    [MaPhuongTien] = @MaPhuongTien,
    [MaKhuVuc] = @MaKhuVuc,
    [MaLenhSanXuat] = @MaLenhSanXuat,
    [MaNhanVienPhucVu] = @MaNhanVienPhucVu,
    [MaLoaiCongViec] = @MaLoaiCongViec,
    [IsEnabled] = @IsEnabled,
    [MaNhanVien2] = @MaNhanVien2,
    [MaSanPham] = @MaSanPham,
    [MaCongDoan] = @MaCongDoan,
    [MaBon] = @MaBon,
    Luot = @Luot,
    MaQuyTrinh = @MaQuyTrinh,
    MaDonHang = @MaDonHang,
    MaKhachHang = @MaKhachHang,
    IsDatKhangSinh = @IsDatKhangSinh,
    MaKhangSinh = @MaKhangSinh,
    VoXo = @VoXo,
    [MaThongTinPhu] = @MaThongTinPhu,
    [MaPhuGia] = MaPhuGia,
    MaTrangThaiNguyenLieu = @MaTrangThaiNguyenLieu,
    [MaQuyTrinhOrg] = @MaQuyTrinhOrg,
    IsNgam = @IsNgam,
    IsCanTay = @IsCanTay,
    [T] = @T,
    MaTyLeNhomHoaChat = @MaTyLeNhomHoaChat,
    [SoLuong] = @SoLuong,
    [TrongLuongDonVi] = @TrongLuongDonVi,
    [MaKhachHangOrg] = @MaKhachHangOrg,
    [MaSizeOrg] = @MaSizeOrg,
    [MaPhuGiaOrg] = @MaPhuGiaOrg,
    [MaNhomHoaChat] = @MaNhomHoaChat,
    [MaPhieuPhanCo] = @MaPhieuPhanCo, [GhiChu2]=@GhiChu2, [GhiChu3]=@GhiChu3, [VoXo2] = @VoXo2, [MaSizeTP] = @MaSizeTP
WHERE [Ma] = @Ma";
        private readonly string qrUpdateDb = @"IF (
    NOT EXISTS (
        SELECT *
        FROM INFORMATION_SCHEMA.TABLES
        WHERE TABLE_SCHEMA = 'dbo'
            AND TABLE_NAME = 'T_PhieuCan'
    )
) BEGIN CREATE TABLE [dbo].[T_PhieuCan](
    [Ma] [varchar](200) NOT NULL,
    [STT] [int] NOT NULL,
    [MayCan] [varchar](50) NOT NULL DEFAULT (getdate()),
    [Ngay] [date] NOT NULL DEFAULT (getdate()),
    [NgayNguyenLieu] [date] NOT NULL DEFAULT (getdate()),
    [Gio] [time](7) NOT NULL DEFAULT (getdate()),
    [MaLoaiNguyenLieu] [varchar](50) NULL,
    [MaSize] [varchar](50) NOT NULL,
    [MaThanhPham] [varchar](50) NOT NULL,
    [MaLo] [varchar](50) NULL,
    [MaNhomLo] [varchar](50) NULL,
    [MaNhanVien] [varchar](50) NULL,
    [MaGroup] [varchar](50) NULL,
    [MaThe] [varchar](50) NOT NULL DEFAULT ((0)),
    [MaLoaiKhuon] [varchar](50) NULL,
    [MaQuyCach] [varchar](50) NULL,
    [MaLoaiCan] [varchar](50) NOT NULL DEFAULT ('TP'),
    [MaXuong] [varchar](50) NOT NULL,
    [TrongLuong] [decimal](18, 3) NOT NULL DEFAULT ((0)),
    [TrongLuongNhan] [decimal](18, 3) NOT NULL DEFAULT ((0)),
    [TrongLuongTare] [decimal](18, 3) NOT NULL DEFAULT ((0)),
    [DinhMuc] [decimal](18, 3) NOT NULL,
    [SuDung] [bit] NOT NULL DEFAULT ((1)),
    [GhiChu] [nvarchar](max) NULL,
    [MaNhaCungCap] [varchar](50) NULL,
    [MaPhuongTien] [varchar](50) NULL,
    [MaKhuVuc] [varchar](50) NOT NULL DEFAULT ('T'),
    [MaLenhSanXuat] [varchar](50) NOT NULL,
    [MaNhanVienPhucVu] [varchar](50) NULL,
    [MaLoaiCongViec] [varchar](50) NOT NULL,
    [MaNhanVien2] [varchar](50) NULL,
    [IsEnabled] [bit] NOT NULL DEFAULT ((0)),
    [MaSanPham] [varchar](50) NULL,
    [MaCongDoan] [varchar](50) NULL,
    [MaBon] [varchar](50) NULL,
    [Luot] [int] NOT NULL DEFAULT ((0)),
    [MaQuyTrinh] [varchar](50) NULL,
    [MaDonHang] [varchar](50) NULL,
    [MaKhachHang] [varchar](50) NULL,
    [IsDatKhangSinh] [bit] NOT NULL DEFAULT ((1)),
    [MaKhangSinh] [varchar](50) NULL,
    [VoXo] [decimal](18, 3) NOT NULL DEFAULT ((0)),
    [MaThongTinPhu] [varchar](50) NULL,
    [MaPhuGia] [varchar](50) NULL,
    [MaTrangThaiNguyenLieu] [varchar](50) NULL,
    CONSTRAINT [PK_T_PhieuCan] PRIMARY KEY CLUSTERED ([Ma] ASC) WITH (
        PAD_INDEX = OFF,
        STATISTICS_NORECOMPUTE = OFF,
        IGNORE_DUP_KEY = OFF,
        ALLOW_ROW_LOCKS = ON,
        ALLOW_PAGE_LOCKS = ON
    ) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
DECLARE @tb varchar(30) = 'T_PhieuCan' IF COL_LENGTH(@tb, 'IsEnabled') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD IsEnabled bit NOT NULL DEFAULT ((0))
END IF COL_LENGTH(@tb, 'MaNhanVien2') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaNhanVien2] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'MaSanPham') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaSanPham] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'MaCongDoan') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaCongDoan] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'MaBon') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaBon] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'Luot') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [Luot] [int] NOT NULL DEFAULT ((0))
END IF COL_LENGTH(@tb, 'MaQuyTrinh') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaQuyTrinh] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'MaKhachHang') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaKhachHang] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'MaDonHang') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaDonHang] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'IsDatKhangSinh') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [IsDatKhangSinh] [bit] NOT NULL DEFAULT ((1))
END IF COL_LENGTH(@tb, 'MaKhangSinh') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaKhangSinh] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'VoXo') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [VoXo] [decimal](18, 3) NOT NULL DEFAULT ((0))
END IF COL_LENGTH(@tb, 'MaThongTinPhu') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaThongTinPhu] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'MaPhuGia') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaPhuGia] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'MaTrangThaiNguyenLieu') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaTrangThaiNguyenLieu] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'MaQuyTrinhOrg') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaQuyTrinhOrg] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'IsNgam') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [IsNgam] bit NOT NULL DEFAULT ((0))
END IF COL_LENGTH(@tb, 'IsCanTay') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [IsCanTay] bit NOT NULL DEFAULT ((0))
END IF COL_LENGTH(@tb, 'T') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [T] [decimal](18, 3) NOT NULL DEFAULT ((0))
END IF COL_LENGTH(@tb, 'MaTyLeNhomHoaChat') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaTyLeNhomHoaChat] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'SoLuong') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [SoLuong] [decimal](18, 3) NOT NULL DEFAULT ((1))
END IF COL_LENGTH(@tb, 'TrongLuongDonVi') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [TrongLuongDonVi] [decimal](18, 3) NOT NULL DEFAULT ((1))
END IF COL_LENGTH(@tb, 'MaKhachHangOrg') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaKhachHangOrg] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'MaSizeOrg') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaSizeOrg] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'MaPhuGiaOrg') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaPhuGiaOrg] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'MaNhomHoaChat') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaNhomHoaChat] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'MaSizeTP') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaSizeTP] [varchar](50) NULL
END IF COL_LENGTH(@tb, 'MaPhieuPhanCo') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [MaPhieuPhanCo] [varchar](50) NULL
END

IF COL_LENGTH(@tb, 'GhiChu2') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [GhiChu2] [nvarchar](max) NULL
END
IF COL_LENGTH(@tb, 'GhiChu3') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [GhiChu3] [nvarchar](max) NULL
END
IF COL_LENGTH(@tb, 'VoXo2') IS NULL BEGIN
ALTER TABLE T_PhieuCan
ADD [VoXo2] [nvarchar](50) NULL
END
";
        private readonly string qrGetAll = "Select * from T_PhieuCan";

        public TPhieuCan()
        {
            connectionString = AppViewModels.Base.Ins.ConnectionString;

        }

        public int Delete<T>(T item)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(qrDelete, item);
            return rows;
        }

        public T Get<T>(string id)
        {
            try
            {
                var query = "Select * from T_PhieuCan Where Ma = @id";
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

        public T Get<T>(string nhomloId, string thanhPhamId, string xuongId)
        {
            var query =
                "Select * from ( Select * from T_PhieuCan where Ngay  = (Select Max(Ngay) from T_PhieuCan where MaXuong = @xuongId and MaThanhPham = @thanhPhamId and MaNhomLo = @nhomloId)) p where p.STT = (Select Max(STT) from T_PhieuCan where MaXuong = @xuongId and MaThanhPham = @thanhPhamId and MaNhomLo = @nhomloId and Ngay  = (Select Max(Ngay) from T_PhieuCan  where MaXuong = @xuongId and MaThanhPham = @thanhPhamId and MaNhomLo = @nhomloId) )";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.Query<T>(query, new { xuongId, nhomloId, thanhPhamId }).FirstOrDefault();
            return item;
        }

        public List<T> Gets<T>()
        {
            try
            {
                var query = "Select * from T_PhieuCan order by STT DESC";
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

        public List<T> Gets<T>(DateTime dateTime)
        {
            try
            {
                var query = "Select * from T_PhieuCan Where  Ngay = @ngay  order by STT DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date }).Result.ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> Gets<T>(DateTime dateTime, bool suDung)
        {
            try
            {
                var query = "Select * from T_PhieuCan Where  Ngay = @ngay and SuDung = @suDung  order by STT DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, suDung }).Result.ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> Gets<T>(string lenhSanXuatId)
        {
            try
            {
                var query = "Select * from T_PhieuCan where MaLenhSanXuat = @lenhSanXuatId ";
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

        public List<T> Gets<T>(bool suDung)
        {
            try
            {
                var query = "Select * from T_PhieuCan where SuDung = @suDung";
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

        public List<T> Gets<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = "Select * from T_PhieuCan Where  Ngay = @ngay and MaXuong=@xuongId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { ngay = dateTime.Date, xuongId }).Result.ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> Gets<T>(
            DateTime dateTime,
            string nhomloId,
            string thanhPhamId,
            string khachHangId,
            string quyTrinhId,
            string xuongId)
        {
            try
            {
                var query =
                    "Select * from T_PhieuCan Where  Ngay = @ngay and MaXuong=@xuongId and MaNhomLo = @nhomloId and MaKhachHang =@khachHangId and MaQuyTrinh = @quyTrinhId and MaThanhPham =@thanhPhamId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId, nhomloId, khachHangId, quyTrinhId, thanhPhamId })
                    .ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<T> Gets<T>(DateTime dateTime, string maPhieuPhanCo, string maSize, string maQuyTrinh, string maPhuGia, string maKhachHang, string maKhangSinh, decimal voXo, string maThongTinPhu, string maTrangThaiNguyenLieu)
        {
            var query =
                @"select p.Ngay,
    p.MaPhieuPhanCo,
    p.MaSize,
    p.MaQuyTrinh,
    p.MaPhuGia,
    p.MaKhachHang,
    p.MaKhangSinh,
    p.VoXo,
p.VoXo2,
    p.MaThongTinPhu,
    p.MaTrangThaiNguyenLieu,
    Sum(p.TrongLuong) as TrongLuong
from T_PhieuCan p
where p.Ngay = @ngay
    and p.MaPhieuPhanCo = @maPhieuPhanCo
    and p.MaSize = @maSize
    and p.MaQuyTrinh = @maQuyTrinh
    and p.MaPhuGia = @maPhuGia
    and p.MaKhachHang = @maKhachHang
    and p.MaKhangSinh = @maKhangSinh
    and p.VoXo = @voXo
    and p.MaThongTinPhu = @maThongTinPhu
    and p.MaTrangThaiNguyenLieu = @maTrangThaiNguyenLieu
GROUP BY p.Ngay,
    p.MaPhieuPhanCo,
    p.MaSize,
    p.MaQuyTrinh,
    p.MaPhuGia,
    p.MaKhachHang,
    p.MaKhangSinh,
p.VoXo2,
    p.VoXo,
    p.MaThongTinPhu,
    p.MaTrangThaiNguyenLieu";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { ngay = dateTime.Date, maPhieuPhanCo, maSize, maQuyTrinh, maPhuGia, maKhachHang, maKhangSinh, voXo, maThongTinPhu, maTrangThaiNguyenLieu }).ToList();
            return items;
        }
        public List<T> GetsByPhieuPhanCo<T>( string maPhieuPhanCo)
        {
            var query =
                @"select 
    p.MaPhieuPhanCo,
    p.MaSizeTP as MaSize,
    p.MaQuyTrinh,
    p.MaKhachHang,
    p.VoXo2,
    Sum(p.TrongLuong) as TrongLuong
from T_PhieuCan p
where  p.MaPhieuPhanCo = @maPhieuPhanCo
GROUP BY 
    p.MaPhieuPhanCo,
    p.MaSizeTP,
    p.MaQuyTrinh,
    p.MaKhachHang,
p.VoXo2";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new {  maPhieuPhanCo}).ToList();
            return items;
        }
        public List<T> GetsByPhieuPhanCo<T>(DateTime dateTime, string maPhieuPhanCo)
        {
            var query =
                @"select p.Ngay,
    p.MaPhieuPhanCo,
    p.MaSizeTP as MaSize,
    p.MaQuyTrinh,
    p.MaPhuGia,
    p.MaKhachHang,
    p.MaKhangSinh,
p.VoXo2,
    p.VoXo,
    p.MaThongTinPhu,
    Sum(p.TrongLuong) as TrongLuong
from T_PhieuCan p
where p.Ngay = @ngay
    and p.MaPhieuPhanCo = @maPhieuPhanCo
GROUP BY p.Ngay,
    p.MaPhieuPhanCo,
    p.MaSizeTP,
    p.MaQuyTrinh,
    p.MaPhuGia,
    p.MaKhachHang,
    p.MaKhangSinh,
p.VoXo2,
    p.VoXo,
    p.MaThongTinPhu";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { ngay = dateTime.Date, maPhieuPhanCo}).ToList();
            return items;
        }
        public List<T> Gets<T>(DateTime dateTime, string xuongId, string mayCanId)
        {
            try
            {
                var query =
                    "Select * from T_PhieuCan Where  Ngay = @ngay and MaXuong=@xuongId and MayCan = @mayCanId order by STT DESC";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId, mayCanId }).ToList();
                return items;
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
                @"Select * from T_PhieuCan Where Ngay = @ngay and MaXuong=@xuongId and MaMayCan= @mayCanId and STT >@stt order by STT DESC";
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

        public Tuple<int, decimal> GetSoRoTongTrongLuong(DateTime dateTime, string xuongId, string mayCanId,
            string thanhPhamId)
        {
            try
            {
                var query =
                    "Select IsNull( Count(*),0) as Item1,isNull( Sum(TrongLuong),0) As Item2 from T_PhieuCan Where  Ngay = @ngay and MaXuong=@xuongId and MayCan = @mayCanId and MaThanhPham = @thanhPhamId ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var item = connection
                    .Query<Tuple<int, decimal>>(query, new { ngay = dateTime.Date, xuongId, mayCanId, thanhPhamId })
                    .SingleOrDefault();
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public Tuple<int, decimal> GetSoRoTongTrongLuongByLoaiCanId(DateTime dateTime, string loaiCanId)
        {
            var query =
                @"Select IsNull( Count(*),0) as Item1,isNull( Sum(TrongLuong),0) As Item2 from T_PhieuCan where MaLoaiCan = @loaiCanId and Ngay =@ngay";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var row = connection.Query<Tuple<int, decimal>>(query, new { ngay = dateTime.Date, loaiCanId })
                .SingleOrDefault();
            return row;
        }

        public Tuple<int, decimal> GetSoRoTongTrongLuongByNhanVienId(DateTime dateTime, string nhanVienId)
        {
            var query =
                @"Select IsNull( Count(*),0) as Item1,isNull( Sum(TrongLuong),0) As Item2 from T_PhieuCan where MaNhanVien = @nhanVienId and Ngay =@ngay";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var row = connection.Query<Tuple<int, decimal>>(query, new { ngay = dateTime.Date, nhanVienId })
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
    T_PhieuCan
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

        /// <summary>
        ///     Pivot
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="xuongId"></param>
        /// <param name="mayCanId"></param>
        /// <param name="khuVucId"></param>
        /// <returns></returns>
        public DataTable GetTongHops(DateTime dateTime, string xuongId, string mayCanId, string khuVucId)
        {
            var query = @"
DECLARE @ParmDefinition NVARCHAR(500);

DECLARE @columnHeaders NVARCHAR (MAX);

DECLARE @GrandTotalCol NVARCHAR (MAX);

DECLARE @GrandTotalRow NVARCHAR(MAX);

DECLARE @FinalQuery NVARCHAR (MAX);

SET
    @ParmDefinition = N'@ngay Date, @xuongId varchar(50), @mayCanId varchar(50), @khuVucId varchar(50)';

SELECT
    @columnHeaders = COALESCE (
        @columnHeaders + ', [' + tp.Ma + ']',
        '[' + tp.Ma + ']'
    )
FROM
    T_PhieuCan p,
    T_ThanhPham tp
Where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.TrongLuong > 0
    and p.MayCan = @mayCanId
    and p.MaKhuVuc = @khuVucId
GROUP BY
    tp.Ma
ORDER BY
    tp.Ma DESC PRINT @columnHeaders
    /* GRAND TOTAL COLUMN */
SELECT
    @GrandTotalCol = COALESCE (
        @GrandTotalCol + 'ISNULL([' + tp.Ma + '],0) + ',
        'ISNULL([' + tp.Ma + '],0) + '
    )
FROM
    T_PhieuCan p,
    T_ThanhPham tp
Where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.TrongLuong > 0
    and p.MayCan = @mayCanId
    and p.MaKhuVuc = @khuVucId
GROUP BY
    tp.Ma
ORDER BY
    tp.Ma DESC
SET
    @GrandTotalCol = LEFT (@GrandTotalCol, LEN (@GrandTotalCol) -1)
    /* GRAND TOTAL ROW */
SELECT
    @GrandTotalRow = COALESCE(
        @GrandTotalRow + ',ISNULL(SUM([' + tp.Ma + ']),0)',
        'ISNULL(SUM([' + tp.Ma + ']),0)'
    )
FROM
    T_PhieuCan p,
    T_ThanhPham tp
Where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and p.TrongLuong > 0
    and p.MayCan = @mayCanId
    and p.MaKhuVuc = @khuVucId
GROUP BY
    tp.Ma
ORDER BY
    tp.Ma DESC
    /* MAIN QUERY */
SET
    @FinalQuery = N'SELECT
    *,
    (' + @GrandTotalCol + ') AS [Tong]
FROM
    (
        SELECT
            ncc.Ten as NhaCC,
            p.MaPhuongTien as Ghe,
            tp.Ma as [TP],
            round(Sum(p.TrongLuong), 3) as TrongLuong
        FROM
            T_PhieuCan p,
            T_NhaCungCap ncc,
            T_ThanhPham tp
        Where
            p.Ngay = @ngay
            and p.MaXuong = @xuongId
            and p.TrongLuong > 0
            and p.MayCan = @mayCanId
            and p.MaNhaCungCap = ncc.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaKhuVuc = @khuVucId
        Group By
            ncc.Ten,
            p.MaPhuongTien,
            tp.Ma
    ) A PIVOT (
        Sum(TrongLuong) FOR TP IN (' + @columnHeaders + ')
    ) B';

EXEC sp_executesql @FinalQuery,
@ParmDefinition,
@ngay,
@xuongId,
@mayCanId,
@khuVucId";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
            cmd.Parameters.AddWithValue("@xuongId", xuongId);
            cmd.Parameters.AddWithValue("@mayCanId", mayCanId);
            cmd.Parameters.AddWithValue("@khuVucId", khuVucId);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }
        public List<T> GetAllWithFullFields<T>(DateTime dateTime)
        {
            var query =
                @"select 
p.STT,
p.Gio,
p.MaNhanVien,
nv.MaHoSo,
nv.Name as TenNhanVien,
p.NgayNguyenLieu,
p.MaSize,
s.Ten as SizeName,
p.MaNhomLo,
nl.Ten as NhomLoName,
p.MaThe,
p.MaLoaiNguyenLieu,
lnl.Ten as LoaiNguyenLieuName,
p.MaLoaiKhuon,
lk.Ten as LoaiKhuonName,
p.MaQuyCach,
qc.Ten as QuyCachName,
p.MaLoaiCan,
lc.Ten as LoaiCanName,
p.MaXuong,
xn.Ten as XuongName,
p.TrongLuong,
p.TrongLuongNhan,
p.TrongLuongTare,
p.MaNhaCungCap,
ncc.Ten as NhaCungCapName,
p.MaPhuongTien,
pt.Ten as PhuongTienName,
p.MaKhuVuc,
kv.Ten as KhuVucName,
p.MaLenhSanXuat,
lsx.Ten as LenhSanXuatName,
p.MaLoaiCongViec,
tp.Ma as LoaiCongVienName,
p.MaSanPham,
sp.Ten as SanPhamName,
p.MaCongDoan,
cd.Ten as CongDoanName,
p.MaBon,
b.Ten as BonName,
p.MaQuyTrinh,
qt.Ten as QuyTrinhName,
p.MaDonHang,
p.MaKhachHang,
kh.Ten as KhachHangName,
p.MaKhangSinh,
ks.Ten as KhangSinhName,
p.VoXo,
p.VoXo2,
p.MaThongTinPhu,
ttp.Ten as ThongTinPhuName,
p.MaPhuGia,
pg.Ten as PhuGiaName,
p.MaTrangThaiNguyenLieu,
ttnl.Ten as TrangThaiNguyenLieuName,
p.SuDung,
p.MayCan,
p.GhiChu,
p.GhiChu2,
p.GhiChu3


from T_PhieuCan p
left join NhanVienDaiThanh nv on nv.MaNhanVien = p.MaNhanVien
left join T_Size s on s.Ma = p.MaSize
left join T_NhomLo nl on nl.Ma = p.MaNhomLo
left join T_LoaiNguyenLieu lnl on lnl.Ma = p.MaLoaiNguyenLieu
left join T_LoaiKhuon lk on lk.Ma = p.MaLoaiKhuon
left join T_QuyCach qc on qc.Ma = p.MaQuyCach
left join T_LoaiCan lc on lc.Ma = p.MaLoaiCan
left join T_NhaCungCap ncc on ncc.Ma = p.MaNhaCungCap
left join T_PhuongTien pt on pt.Ma = p.MaPhuongTien
left join T_KhuVuc kv on kv.Ma = p.MaKhuVuc
left join T_LenhSanXuat lsx on lsx.Ma = p.MaLenhSanXuat
left join T_ThanhPham tp on tp.Ma = p.MaThanhPham
left join T_SanPham sp on sp.Ma = p.MaSanPham
left join T_CongDoan cd on cd.Ma = p.MaCongDoan
left join T_Bon b on b.Ma =p.MaBon
left join T_QuyTrinh qt on qt.Ma = p.MaQuyTrinh
left join T_DonHang dh on dh.Ma = p.MaDonHang
left join T_KhachHang kh on kh.Ma = p.MaKhachHang
left join T_KhangSinh ks on ks.Ma = p.MaKhangSinh
left join T_ThongTinPhu ttp on ttp.Ma = p.MaThongTinPhu
left join T_PhuGia pg on pg.Ma = p.MaPhuGia
left join T_TrangThaiNguyenLieu ttnl on ttnl.Ma = p.MaTrangThaiNguyenLieu
left join XiNghiep xn on xn.Ma = p.MaXuong
where Ngay = @ngay
order by STT DESC
";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { ngay = dateTime.Date}).ToList();
            return items;
        }

        //   public IList<string> GetSqlsInBatches(IList<Modelv1.EF.T_PhieuCan> phieuCans)
        //   {
        //       var insertSql = @"INSERT INTO [dbo].[PhieuCanTPDinhHinh]
        //      ([Ma] 
        //      ,[STT]
        //      ,[MayCan]
        //      ,[Ngay]
        //      ,[NgayNguyenLieu]
        //      ,[Gio]
        //      ,[MaLoaiNguyenLieu]
        //      ,[MaSize]
        //      ,[MaThanhPham]
        //      ,[MaLo]
        //      ,[MaNhomLo]
        //      ,[MaNhanVien]
        //      ,[MaGroup]
        //      ,[MaThe]
        //      ,[MaLoaiKhuon]
        //      ,[MaQuyCach]
        //      ,[MaLoaiCan]
        //      ,[MaXuong]
        //      ,[TrongLuong]
        //      ,[TrongLuongNhan]
        //      ,[TrongLuongTare]
        //      ,[DinhMuc]
        //      ,[SuDung]
        //      ,[GhiChu]
        //      ,[MaNhaCungCap]
        //      ,[MaPhuongTien]
        //      ,[MaKhuVuc]
        //      ,[MaLenhSanXuat])
        //VALUES";
        //       var valuesSql = @"('{0}',{1},'{2}', '{3}', '{4}', '{5}', '{6}', '{7}','{8}','{9}','{10}','{11}', '{12}', '{13}', '{14}', '{15}','{16}','{17}', {18}, {19}, {20},{21},{22},'{23}','{24}','{25}','{26}','{27}')";
        //       var batchSize = 1000;

        //       var sqlsToExecute = new List<string>();
        //       var numberOfBatches = (int)Math.Ceiling((double)phieuCans.Count / batchSize);

        //       for (int i = 0; i < numberOfBatches; i++)
        //       {
        //           var phieuCanToInsert = phieuCans.Skip(i * batchSize).Take(batchSize);
        //           var valuesToInsert = phieuCanToInsert.Select(
        //               x => string.Format(
        //                   valuesSql,
        //                   x.Ma,
        //                   x.STT,
        //                   x.MayCan,
        //                   x.Ngay.ToString("yyyy-MM-dd"),
        //                   x.NgayNguyenLieu.ToString("yyyy-MM-dd"),
        //                   x.Gio.ToString(@"hh\:mm\:ss"),
        //                   x.MaLoaiNguyenLieu,
        //                   x.MaSize,
        //                   x.MaThanhPham,
        //                   x.MaLo,
        //                   x.MaNhomLo,
        //                   x.MaNhanVien,
        //                   x.MaGroup,
        //                   x.MaThe,
        //                   x.MaLoaiKhuon,
        //                   x.MaQuyCach,
        //                   x.MaXuong,
        //                   x.TrongLuong,
        //                   x.TrongLuongNhan,
        //                   x.TrongLuongTare,
        //                   x.DinhMuc,
        //                   x.SuDung ? 1 : 0,
        //                   x.GhiChu,
        //                   x.MaNhaCungCap,
        //                   x.MaPhuongTien,
        //                   x.MaKhuVuc,
        //                   x.MaLenhSanXuat));
        //           sqlsToExecute.Add(insertSql + string.Join(",", valuesToInsert));
        //       }

        //       return sqlsToExecute;
        //   }

        public int Insert<T>(T item)
        {
            var query = qrInsert;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, item);
            return rows;
        }

        public int Insert<T>(List<T> items)
        {
            var query = qrInsert;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, items);
            return rows;
        }

        //public int InsertBatch<T>(List<T> items)
        //{
        //    var batches = DbExtensions.GetSqlsInBatches(items);
        //    var row = 0;
        //    var database = new Database(connectionString);
        //    foreach (var batche in batches) row += database.ExecuteNonQuery(batche);

        //    return row;
        //}

        public int Update<T>(T item)
        {
            var query = qrUpdate;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, item);
            return rows;
        }

        public int Update<T>(List<T> items)
        {
            var query = qrUpdate;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Execute(query, items);
            return rows;
        }

        //public int UpdateDatabase()
        //{
        //    try
        //    {
        //        //Thêm Cột Id Vào bảng MaLoaiCa trên máy cân đầu Ao
        //        var query = qrUpdateDb;
        //        var dao = new Database(connectionString);
        //        return dao.ExecuteNonQuery(query);
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception(
        //            $@"Không thể cập nhật Cơ Sở Dữ Liệu vui lòng liên hệ PMS để được hổ trợ [T_PhieuCan]. {Environment.NewLine}{exception.Message}");
        //        //throw;
        //    }
        //}

        #region báo cáo

        public int GetMaxSTT(DateTime dateTime, string xuongId, string mayCanId)
        {
            var query =
                "Select Max(STT) as STT from T_PhieuCan where  Ngay =@ngay and MaXuong = @xuongId and MaMayCan =@mayCanId";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.Query<int>(query, new { ngay = dateTime.Date, xuongId, mayCanId })
                .SingleOrDefault();
            return item;
        }

        public Tuple<int, decimal> GetTongSoRoTongTrongLuong(DateTime dateTime, string nhanVienId)
        {
            var query =
                "Select Count(*) as Item1,ISNULL(Sum(TrongLuong) ,0) As Item2 from T_PhieuCan where MaNhanVien = @nhanVienId and Ngay =@ngay and ISNULL(GhiChu,'') <> 'HUY'";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.Query<Tuple<int, decimal>>(query, new { ngay = dateTime.Date, nhanVienId })
                .SingleOrDefault();
            return item;
        }

        public List<T> GetTongHopThanhPham<T>(DateTime dateTime, string maLo, string xuongId)
        {
            var query = @"Select
    p.MaThanhPham,
    SUM(p.TrongLuong) as TrongLuongHienTai
from
    T_PhieuCan p
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaLo =@maLo
GROUP BY
    p.MaThanhPham";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(query, new { ngay = dateTime.Date, maLo, xuongId }).ToList();
            return rows;
        }

        public List<T> GetTongHopThanhPham<T>(DateTime dateTime, string maLo, string xuongId, string khuvuc)
        {
            var query = @"Select
    p.MaThanhPham,
    SUM(p.TrongLuong) as TrongLuongHienTai,
	p.MaKhuVuc
from
    T_PhieuCan p,
	T_KhuVuc kv
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaLo =@maLo
	and p.MaKhuVuc = kv.Ma and kv.Ma= @khuVuc
GROUP BY
    p.MaThanhPham,
	p.MaKhuVuc";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var rows = connection.Query<T>(query, new { ngay = dateTime.Date, maLo, xuongId, khuvuc }).ToList();
            return rows;
        }

        public DataTable GetTongHopThanhPhams(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var query = @"Select
    p.Ngay as [Ngày],
    cast(
        DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
    ) as [Thời Gian Làm Việc (h)],
   
    p.MaLo as [Lô],
    tp.Ten as [Thành Phẩm],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng],
    --Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
	kv.Ten as [Khu Vực]
from
    T_PhieuCan p,
    T_ThanhPham tp,
	T_KhuVuc kv
where
    p.Ngay <= @ngay
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
	and p.MaKhuVuc = kv.Ma
GROUP BY
    p.MaLo,
    tp.Ten,
    p.Ngay,
	kv.Ten
order by
    p.Ngay,
    p.MaLo,
    tp.Ten";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", toDate.Date);
            cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
            cmd.Parameters.AddWithValue("@xuongId", xuongId);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetTongHopThanhPhams(DateTime fromDate, DateTime toDate, string xuongId, string khuVuc)
        {
            var query = @"Select
    p.Ngay as [Ngày],
    cast(
        DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
    ) as [Thời Gian Làm Việc (h)],
   
    p.MaLo as [Lô],
    tp.Ten as [Thành Phẩm],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng],
    --Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
	kv.Ten as [Khu Vực]
from
    T_PhieuCan p,
    T_ThanhPham tp,
	T_KhuVuc kv
where
    p.Ngay <= @ngay
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
	and p.MaKhuVuc = kv.Ma and kv.Ma = @khuVuc
GROUP BY
    p.MaLo,
    tp.Ten,
    p.Ngay,
	kv.Ten
order by
    p.Ngay,
    p.MaLo,
    tp.Ten";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", toDate.Date);
            cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
            cmd.Parameters.AddWithValue("@xuongId", xuongId);
            cmd.Parameters.AddWithValue("@khuVuc", khuVuc);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetTongHopThanhPhams(DateTime dateTime, string xuongId)
        {
            var query = @"Select
    p.MaLo as [Lô],
    tp.Ten as [Thành Phẩm],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng],
	kv.Ten [Khu Vực]
    --Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
from
    T_PhieuCan p,
    T_LoaiNguyenLieu la,
    T_ThanhPham tp,
    T_Size s,
	T_KhuVuc kv,
    NhanVienDaiThanh n
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiNguyenLieu= la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
	and p.MaKhuVuc = kv.Ma
GROUP BY
    p.MaLo,
    tp.Ten,
    kv.Ten
order by
    p.MaLo,
    tp.Ten";
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

        public DataTable GetTongHopThanhPhams(DateTime dateTime, string xuongId, string khuVuc)
        {
            var query = @"Select
    p.MaLo as [Lô],
    tp.Ten as [Thành Phẩm],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng],
	kv.Ten [Khu Vực]
    --Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
from
    T_PhieuCan p,
    T_LoaiNguyenLieu la,
    T_ThanhPham tp,
    T_Size s,
	T_KhuVuc kv,
    NhanVienDaiThanh n
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiNguyenLieu= la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
	and p.MaKhuVuc = kv.Ma and kv.Ma = @khuVuc
GROUP BY
    p.MaLo,
    tp.Ten,
    kv.Ten
order by
    p.MaLo,
    tp.Ten";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
            cmd.Parameters.AddWithValue("@xuongId", xuongId);
            cmd.Parameters.AddWithValue("@khuVuc", khuVuc);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetTongHops(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var query = @"Select
    p.Ngay as [Ngày],
    cast(
        DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
    ) as [Thời Gian Làm Việc (h)],
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    p.MaLo as [Lô],
    la.Ten as [Loại Cá],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng],
    --Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt],
	kv.Ten as [Khu Vực]
from
    T_PhieuCan p,
    T_LoaiNguyenLieu la,
    T_ThanhPham tp,
    T_Size s,
    NhanVienDaiThanh n,
	T_KhuVuc kv
where
    p.Ngay <= @ngay
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiNguyenLieu = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
	and p.MaKhuVuc = kv.Ma
GROUP BY
    n.MaHoSo,
    n.Name,
    p.MaLo,
    la.Ten,
    tp.Ten,
    s.Ten,
	kv.Ten,
    p.Ngay
order by
    p.Ngay,
    n.MaHoSo,
    p.MaLo,
    tp.Ten,
    s.Ten";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", toDate.Date);
            cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
            cmd.Parameters.AddWithValue("@xuongId", xuongId);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetTongHops(DateTime fromDate, DateTime toDate, string xuongId, string khuVuc)
        {
            var query = @"Select
    p.Ngay as [Ngày],
    cast(
        DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
    ) as [Thời Gian Làm Việc (h)],
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    p.MaLo as [Lô],
    la.Ten as [Loại Cá],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng],
    --Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt],
	kv.Ten as [Khu Vực]
from
    T_PhieuCan p,
    T_LoaiNguyenLieu la,
    T_ThanhPham tp,
    T_Size s,
    NhanVienDaiThanh n,
	T_KhuVuc kv
where
    p.Ngay <= @ngay
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiNguyenLieu = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
	and p.MaKhuVuc = kv.Ma and kv.Ma = @khuVuc
GROUP BY
    n.MaHoSo,
    n.Name,
    p.MaLo,
    la.Ten,
    tp.Ten,
    s.Ten,
	kv.Ten,
    p.Ngay
order by
    p.Ngay,
    n.MaHoSo,
    p.MaLo,
    tp.Ten,
    s.Ten";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", toDate.Date);
            cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
            cmd.Parameters.AddWithValue("@xuongId", xuongId);
            cmd.Parameters.AddWithValue("@khuVuc", khuVuc);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetTongHops(DateTime dateTime, string xuongId)
        {
            var query = @"Select
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    p.MaLo as [Lô],
    la.Ten as [Loại Cá],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng],
	kv.Ten [Khu Vực]
    --Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
from
    T_PhieuCan p,
    T_LoaiNguyenLieu la,
    T_ThanhPham tp,
    T_Size s,
	T_KhuVuc kv,
    NhanVienDaiThanh n
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiNguyenLieu = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaKhuVuc = kv.Ma
GROUP BY
    n.MaHoSo,
    n.Name,
    p.MaLo,
    la.Ten,
    tp.Ten,
    s.Ten,
	kv.Ten
    
order by
    n.MaHoSo,
    p.MaLo,
    tp.Ten,
    s.Ten";
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

        public DataTable GetTongHops(DateTime dateTime, string xuongId, string khuVuc)
        {
            var query = @"Select
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    p.MaLo as [Lô],
    la.Ten as [Loại Cá],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng],
	kv.Ten [Khu Vực]
    --Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
from
    T_PhieuCan p,
    T_LoaiNguyenLieu la,
    T_ThanhPham tp,
    T_Size s,
	T_KhuVuc kv,
    NhanVienDaiThanh n
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and p.MaLoaiNguyenLieu = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaKhuVuc = kv.Ma and kv.Ma = @khuVuc
GROUP BY
    n.MaHoSo,
    n.Name,
    p.MaLo,
    la.Ten,
    tp.Ten,
    s.Ten,
	kv.Ten
    
order by
    n.MaHoSo,
    p.MaLo,
    tp.Ten,
    s.Ten";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
            cmd.Parameters.AddWithValue("@xuongId", xuongId);
            cmd.Parameters.AddWithValue("@khuVuc", khuVuc);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetTongHopPhucVus(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var query = @"Select
    p.Ngay as [Ngày],
    cast(
        DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
    ) as [Thời Gian Làm Việc (h)],
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    p.MaLo as [Lô],
    la.Ten as [Loại Cá],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng]
    --Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
from
    T_PhieuCan p,
    T_LoaiNguyenLieu la,
    T_ThanhPham tp,
    T_Size s,
    NhanVienDaiThanh n,
	T_KhuVuc kv
where
    p.Ngay <= @ngay
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaNhanVienPhucVu = n.MaNhanVien
    and p.MaLoaiNguyenLieu = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
GROUP BY
    n.MaHoSo,
    n.Name,
    p.MaLo,
    la.Ten,
    tp.Ten,
    s.Ten,
    p.Ngay
order by
    p.Ngay,
    n.MaHoSo,
    p.MaLo,
    tp.Ten,
    s.Ten";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", toDate.Date);
            cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
            cmd.Parameters.AddWithValue("@xuongId", xuongId);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetTongHopPhucVus(DateTime fromDate, DateTime toDate, string xuongId, string khuVuc)
        {
            var query = @"Select
    p.Ngay as [Ngày],
    cast(
        DATEDIFF(second, Min(p.Gio), Max(p.Gio)) / 3600.0 as decimal(18, 3)
    ) as [Thời Gian Làm Việc (h)],
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    p.MaLo as [Lô],
    la.Ten as [Loại Cá],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng],
    --Sum(p.TrongLuong) * tp.DinhMucHaoHut as [TL Trước Hao Hụt]
	kv.Ten [Khu Vực]
from
    T_PhieuCan p,
    T_LoaiNguyenLieu la,
    T_ThanhPham tp,
    T_Size s,
    NhanVienDaiThanh n,
	T_KhuVuc kv
where
    p.Ngay <= @ngay
    and p.Ngay >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaNhanVienPhucVu = n.MaNhanVien
    and p.MaLoaiNguyenLieu = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
	and p.MaKhuVuc = kv.Ma and kv.Ma = @khuVuc
GROUP BY
    n.MaHoSo,
    n.Name,
    p.MaLo,
    la.Ten,
    tp.Ten,
    s.Ten,
    p.Ngay,
	kv.Ten

order by
    p.Ngay,
    n.MaHoSo,
    p.MaLo,
    tp.Ten,
    s.Ten";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", toDate.Date);
            cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
            cmd.Parameters.AddWithValue("@xuongId", xuongId);
            cmd.Parameters.AddWithValue("@khuVuc", khuVuc);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetTongHopPhucVus(DateTime dateTime, string xuongId)
        {
            var query = @"Select
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    p.MaLo as [Lô],
    la.Ten as [Loại Nguyên Liệu],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng]
from
    T_PhieuCan p,
    T_LoaiNguyenLieu la,
    T_ThanhPham tp,
    T_Size s,
    NhanVienDaiThanh n
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVienPhucVu = n.MaNhanVien
    and p.MaLoaiNguyenLieu = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
GROUP BY
    n.MaHoSo,
    n.Name,
    p.MaLo,
    la.Ten,
    tp.Ten,
    s.Ten
order by
    n.MaHoSo,
    p.MaLo,
    tp.Ten,
    s.Ten";
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

        public DataTable GetTongHopPhucVus(DateTime dateTime, string xuongId, string khuVuc)
        {
            var query = @"Select
    n.MaHoSo as [Mã Hồ Sơ],
    n.Name as [Tên Nhân Viên],
    p.MaLo as [Lô],
    la.Ten as [Loại Nguyên Liệu],
    tp.Ten as [Thành Phẩm],
    s.Ten as [Size],
    Count(*) as [Số Rổ],
    Sum(p.TrongLuong) as [Trọng Lượng],
	kv.Ten [Khu Vực]
from
    T_PhieuCan p,
    T_LoaiNguyenLieu la,
    T_ThanhPham tp,
    T_Size s,
    NhanVienDaiThanh n,
	T_KhuVuc kv
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVienPhucVu = n.MaNhanVien
    and p.MaLoaiNguyenLieu = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
	and p.MaKhuVuc = kv.Ma and kv.Ma = @khuVuc
GROUP BY
    n.MaHoSo,
    n.Name,
    p.MaLo,
    la.Ten,
    tp.Ten,
    s.Ten,
	kv.Ten
order by
    n.MaHoSo,
    p.MaLo,
    tp.Ten,
    s.Ten,
	kv.Ten";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
            cmd.Parameters.AddWithValue("@xuongId", xuongId);
            cmd.Parameters.AddWithValue("@khuVuc", khuVuc);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public int GetNumNhanVienDaChia(DateTime dateTime, string xuongId, string khuVucId)
        {
            var query = @"Select
    COUNT(Distinct MaNhanVien)
from
    PhieuCanBTPDinhHinh
where
    Ngay = @ngay
    and MaXuong = @xuongId and MaKhuVuc = @khuVucId";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var item = connection.ExecuteScalar<int>(query, new { ngay = dateTime.Date, xuongId, khuVucId });
            return item;
        }

        public List<T> GetTongHopNhanh2<T>(DateTime dateTime, string xuongId, string maHoSo)
        {
            var query = @"Select
    p.MaNhanVien,
    n.Name as NhanVienName,
    k.Ten as KhuVucName,
    nl.Ten as Lo,
    sp.Ten as SanPhamName,
    tp.Ten as NhomCongViecName,
    cd.Ten as CongViecName,
    s.Ten as SizeName,
    qt.Ten as QuyTrinhName,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong
from
    T_PhieuCan p,
    NhanVienDaiThanh n,
    T_ThanhPham tp,
    T_Size s,
    T_KhuVuc k,
    T_NhomLo nl,
    T_CongDoan cd,
    T_QuyTrinh qt,
    T_SanPham sp
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and n.MaHoSo = @maHoSo
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaKhuVuc = k.Ma
    and p.MaNhomLo = nl.Ma
    and p.MaCongDoan = cd.Ma
    and p.MaQuyTrinh = qt.Ma
    and p.MaSanPham = sp.Ma
group by
     p.MaNhanVien,
    n.Name ,
    k.Ten ,
    nl.Ten ,
    tp.Ten ,
    cd.Ten ,
    s.Ten ,
    qt.Ten,
    sp.Ten
order by
    k.Ten,
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

        public List<T> GetTongHopNhanh<T>(DateTime dateTime, string xuongId, string maHoSo)
        {
            var query = @"Select
    p.MaNhanVien,
    n.Name as NhanVienName,
    k.Ten as KhuVucName,
    nl.Ten as Lo,
    la.Ten as LoaiNguyenlieuName,
    tp.Ten as ThanhPhamName,
    s.Ten as SizeName,
    Count(*) as SoRo,
    Sum(p.TrongLuong) as TrongLuong
from
    T_PhieuCan p,
    NhanVienDaiThanh n,
    T_LoaiNguyenLieu la,
    T_ThanhPham tp,
    T_Size s,
    T_KhuVuc k,
    T_NhomLo nl
where
    p.Ngay = @ngay
    and p.MaXuong = @xuongId
    and p.MaNhanVien = n.MaNhanVien
    and n.MaHoSo = @maHoSo
    and p.MaLoaiNguyenLieu = la.Ma
    and p.MaThanhPham = tp.Ma
    and p.MaSize = s.Ma
    and p.MaKhuVuc = k.Ma
    and p.MaNhomLo = nl.Ma
group by
    p.MaNhanVien,
    n.Name,
    k.Ten,
    nl.Ten,
    la.Ten,
    tp.Ten,
    s.Ten
order by
    k.Ten,
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

        public List<T> GetTongHopNhanh<T>(DateTime dateTime, string xuongId, string maHoSo, string khuVuc)
        {
            var query = @"Select 
	p.MaNhanVien,
	n.Name as NhanVienName,
	p.MaLo,
	la.Ten as LoaiNguyenlieuName,
	tp.Ten as ThanhPhamName,
	Sum(p.TrongLuong) as TrongLuong
from 
	T_PhieuCan p,
	NhanVienDaiThanh n,
	T_LoaiNguyenLieu la,
	T_ThanhPham tp ,
	T_KhuVuc kv
where 
	p.Ngay= @ngay 
	and p.MaXuong = @xuongId 
	and p.MaNhanVien = n.MaNhanVien 
	and n.MaHoSo = @maHoSo 
	and p.MaLoaiNguyenLieu = la.Ma 
	and p.MaThanhPham = tp.Ma
	and p.MaKhuVuc = kv.Ma  and kv.Ma =@khuVuc
group by 
	n.Name,
	p.MaNhanVien,
	p.MaLo,
	la.Ten ,
	tp.Ten,
	kv.Ten
	
order by 
	p.MaLo,
	tp.Ten";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId, maHoSo, khuVuc })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetTongHopsNgam<T>(string maNhomLo, string khuVuc = "NGAM")
        {
            var query = @"Select
    b.Ten as BonName,
    ptp.MaLoaiCan as LoaiCan,
    tp.Ma as MaThanhPham,
    tp.Ten as ThanhPhamName,
    s.Ma as MaSize,
    s.Ten as SizeName,
    ptp.MaLo,
    nl.Ten as NhomLo,
    ptp.MaLoaiNguyenLieu,
    la.Ten as LoaiNguyenLieuName,
    Sum(ptp.TrongLuong ) as TrongLuong,
    ptp.MaXuong,
sp.Ten as SanPhamName
--kv.Ten as KhuVuc
from
    T_PhieuCan ptp,
    NhanVienDaiThanh n,
    T_ThanhPham tp,
    T_Size s,
    T_LoaiNguyenLieu la,
    T_NhomLo nl,
T_SanPham sp,
    T_Bon b
where  ptp.MaNhanVien = n.MaNhanVien
    and ptp.MaThanhPham = tp.Ma
    and ptp.MaSize = s.Ma --and ptp.MaMau = mau.Ma
    and ptp.MaLoaiNguyenLieu = la.Ma
    and ptp.TrongLuong > 0
    and ptp.MaKhuVuc = @khuVuc
    and ptp.MaNhomLo = nl.Ma
    and ptp.MaBon = b.Ma
    and ptp.MaNhomLo = @maNhomLo

 and ptp.MaSanPham = sp.Ma
GROUP BY
 b.Ten  ,
    ptp.MaLoaiCan  ,
    tp.Ma  ,
    tp.Ten  ,
    s.Ma ,
    s.Ten  ,
    ptp.MaLo,
    nl.Ten  ,
    ptp.MaLoaiNguyenLieu,
    la.Ten,
    ptp.MaXuong ,sp.Ten";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { maNhomLo, khuVuc }).ToList();
            return items;
        }

        public List<T> GetTongHopLatDauPhanCo<T>(DateTime fromDate, DateTime toDate)
        {
            var query = @"select
    pLD.*,
    pPC.TrongLuongPC,
    nl.Ten as NhomLo,
    sp.Ten as SanPhamName
from
    (
        select
            p.NgayNguyenLieu,
            p.MaNhomLo,
            p.MaSanPham,
            p.MaKhachHang,
            sum(p.TrongLuong) as TrongLuongLD,
            COUNT(*) as SoRo
        from
            T_PhieuCan p,
            T_NhomLo nl
        where
            p.NgayNguyenLieu >= @fromDate
            and p.NgayNguyenLieu <= @toDate
            and p.MaThanhPham = 'LD'
            and p.TrongLuong > 0
            and p.MaNhomLo = nl.Ma
        GROUP BY
            p.NgayNguyenLieu,
            p.MaNhomLo,
            p.MaSanPham,
            p.MaKhachHang
    ) pLD
    left join (
        select
            p.NgayNguyenLieu,
            p.MaNhomLo,
            p.MaSanPham,
            p.MaKhachHang,
            sum(p.TrongLuong) as TrongLuongPC,
            COUNT(*) as SoRo
        from
            T_PhieuCan p,
            T_ThanhPham tp,
            T_NhomLo nl
        where
            p.NgayNguyenLieu >= @fromDate
            and p.NgayNguyenLieu <= @toDate
            and p.MaThanhPham = tp.Ma
            and tp.IsPhanCo = 1
            and p.TrongLuong > 0
            and p.MaNhomLo = nl.Ma
        GROUP BY
            p.NgayNguyenLieu,
            p.MaNhomLo,
            p.MaSanPham,
            p.MaKhachHang
    ) pPC on pLD.NgayNguyenLieu = pLD.NgayNguyenLieu
    and pLD.MaKhachHang = pPC.MaKhachHang
    and pLD.MaSanPham = pPC.MaSanPham
    and pLD.MaNhomLo = pPC.MaNhomLo
LEFT JOIN T_NhomLo nl on pLD.MaNhomLo = nl.Ma
LEFT JOIN T_SanPham sp on pLD.MaSanPham = sp.Ma";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { fromDate, toDate }).ToList();
            return items;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thanhPhamId"></param>
        /// <param name="fromDate">Ngay Nguyen Lieu</param>
        /// <param name="toDate">Ngay Nguyen Lieu</param>
        /// <returns></returns>
        public List<T> GetTongHopNhomLoByThanPham<T>(string thanhPhamId, DateTime fromDate, DateTime toDate)
        {
            var query =
                @"Select
    nl.NgayNguyenLieu,
    sp.MaLoaiNguyenLieu,
    sp.Ma as MaSanPham,
    nl.Ten,
    sp.MaNL,
    Sum(p.TrongLuong) as TrongLuong
from
    T_PhieuCan p,
    T_NhomLo nl,
    T_SanPham sp
where
    p.MaNhomLo = nl.Ma
    and nl.NgayNguyenLieu >= @fromDate
    and nl.NgayNguyenLieu <= @toDate
    and p.MaThanhPham = @thanhPhamId
    and p.MaSanPham = sp.Ma

group by
    nl.NgayNguyenLieu,
    sp.MaLoaiNguyenLieu,
    sp.Ma,
    nl.Ten,sp.MaNL";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(
                    query,
                    new { thanhPhamId, fromDate = fromDate.Date, toDate = toDate.Date })
                .ToList();
            return items;
        }

        public List<T> GetTongHopNhanViens<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            //                var query = @"Select
            //    IsNull(tp.Ngay, btp.Ngay) as Ngay,
            //    IsNull(tp.MaNhanVien, btp.MaNhanVien) as MaNhanVien,
            //    ISNULL(tp.MaHoSo, btp.MaHoSo) as MaHoSo,
            //    ISNULL(tp.NhanVienName, btp.NhanVienName) as NhanVienName,
            //    IsNull(tp.MaLo, btp.MaLo) as MaLo,
            //    ISNULL(tp.MaLoaiNguyenLieu, btp.MaLoaiNguyenLieu) as MaLoaiNguyenLieu,
            //    ISNULL(tp.LoaiNguyenLieuName, btp.LoaiNguyenLieuName) as LoaiNguyenLieuName,
            //    ISNULL(tp.MaSize, btp.MaSize) as MaSize,
            //    ISNULL(tp.SizeName, btp.SizeName) as SizeName,
            //    ISNULL(tp.MaThanhPham, btp.MaThanhPham) as MaThanhPham,
            //    ISNULL(tp.ThanhPhamName, btp.ThanhPhamName) as ThanhPhamName,
            //    --ISNULL(tp.MaSanPham, btp.MaSanPham) as MaSanPham,
            //    --ISNULL(tp.MaMau, btp.MaMau) as MaMau,
            //    --ISNULL(tp.MauName, btp.MauName) as MauName,
            //    --ISNULL(tp.CaTra, btp.CaTra) as CaTra,
            //    --IsNull(tp.ChiSanLuong, btp.ChiSanLuong) as ChiSanLuong,
            //    ISNULL(tp.TrongLuongNhan, 0) as TrongLuongNhan,
            //    ISNULL(tp.TrongLuongTra, 0) as TrongLuongTra,
            //    ISNULL(tp.DinhMucThucTe, 0) as DinhMuc,
            //    ISNULL(tp.DinhMucYeuCau, 0) as DinhMucChuan,
            //    cast(
            //        (
            //            Case
            //                when @isDinhMucBinhThuong = 1 then (
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
            //    ISNULL(tp.MaXuong, btp.MaXuong) as MaXuong,
            //	ISNULL(tp.KhuVucName,btp.KhuVucName) as KhuVucName
            //from
            //    (
            //        Select
            //            p.Ngay,
            //            p.MaNhanVien,
            //            n.MaHoSo,
            //            n.Name as NhanVienName,
            //            la.Ten as LoaiNguyenLieuName,
            //            s.Ten as SizeName,
            //            tp.Ten as ThanhPhamName,
            //            --tp.BravoId as MaSanPham,
            //            --ma.Ten as MauName,
            //            p.MaLoaiNguyenLieu,
            //            p.MaLo,
            //            --p.MaMau,
            //            p.MaThanhPham,
            //            p.MaSize,
            //            --p.CaTra,
            //            --p.ChiSanLuong,
            //            p.MaXuong,
            //            COUNT(*) as SoRo,
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
            //            ) as SoRoChuaCanTP,
            //			kv.Ten as KhuVucName
            //        from
            //            T_PhieuCan p
            //            LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
            //            LEFT Join T_Size s on p.MaSize = s.Ma
            //            LEFT Join T_LoaiNguyenLieu la on p.MaLoaiNguyenLieu = la.Ma
            //            --LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
            //            LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            //			LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
            //        where
            //            p.STT > 0
            //            and p.Ngay <= @toDate
            //            and p.Ngay >= @fromDate
            //			and p.MaKhuVuc = kv.Ma and kv.Ma = @khuVuc
            //        GROUP BY
            //            p.Ngay,
            //            p.MaNhanVien,
            //            p.MaLoaiNguyenLieu,
            //			p.MaKhuVuc,
            //			kv.Ten,
            //            p.MaLo,
            //            --p.MaMau,
            //            p.MaThanhPham,
            //            p.MaSize,
            //            --p.CaTra,
            //            --p.ChiSanLuong,
            //            p.MaXuong,
            //            n.MaHoSo,
            //            n.Name,
            //            la.Ten,
            //            --ma.Ten,
            //            tp.Ten,
            //            s.Ten

            //            --tp.BravoId
            //    ) btp Full
            //    outer join (
            //        Select
            //            p.Ngay,
            //            p.MaNhanVien,
            //            n.MaHoSo,
            //            n.Name as NhanVienName,
            //            p.MaLo,
            //            p.MaLoaiNguyenLieu,
            //            la.Ten as LoaiNguyenLieuName,
            //            p.MaSize,
            //            s.Ten as SizeName,
            //            p.MaThanhPham,
            //            tp.Ten as ThanhPhamName,
            //            --tp.BravoId as MaSanPham,
            //            --p.MaMau,
            //            --ma.Ten as MauName,
            //            --p.CaTra,
            //            --p.ChiSanLuong,
            //            Sum(p.TrongLuongNhan) as TrongLuongNhan,
            //            Sum(p.TrongLuong) as TrongLuongTra,
            //            case
            //                when @isDinhMucBinhThuong = 1 then cast (
            //                    case
            //                        when sum(p.TrongLuong) = 0 then 0
            //                        else (
            //                            case
            //                                when @isFloor = 1 then (
            //                                    FLOOR(
            //                                        (Sum(p.TrongLuongNhan) / sum(p.TrongLuong)) * 100
            //                                    ) / 100
            //                                )
            //                                else Sum(p.TrongLuongNhan) / sum(p.TrongLuong)
            //                            end
            //                        )
            //                    end as decimal(18, 2)
            //                )
            //                ELSE cast (
            //                    case
            //                        when sum(p.TrongLuongNhan) = 0 then 0
            //                        else sum(p.TrongLuong) / sum(p.TrongLuongNhan)
            //                    end as decimal(18, 4)
            //                )
            //            end as DinhMucThucTe,
            //            DinhMuc.DinhMuc as DinhMucYeuCau,
            //            Count(*) as SoRo,
            //            p.MaXuong,
            //			kv.Ten as KhuVucName
            //        from
            //            T_PhieuCan p
            //            LEFT JOIN (
            //                Select
            //                    tp1.MaLo,
            //                    tp1.MaLoaiNguyenLieu,
            //                    tp1.MaSize,
            //                    --tp1.MaMau,
            //                    tp1.MaThanhPham,
            //                    --tp1.CaTra,
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
            //                            p.MaLoaiNguyenLieu,
            //                            p.MaSize,
            //                            --p.MaMau,
            //                            p.MaThanhPham,
            //                            --p.CaTra,
            //                            --CASE
            //                            --    WHEN p.CaTra = 1 THEN 1.37
            //                            --    ELSE tp.DinhMuc
            //                            --END AS DinhMuc,
            //							1 as DinhMuc,
            //                            p.Ngay,
            //                            p.MaXuong
            //                        from
            //                            T_PhieuCan p,
            //                            T_ThanhPham tp
            //                        where
            //                            p.Ngay <= @toDate
            //                            and p.Ngay >= @fromDate
            //                            and MaXuong = @xuongId
            //                            and p.MaThanhPham = tp.Ma
            //                            --and p.MaLoaiNguyenLieu = tp.MaCa
            //                    ) tp1
            //                    LEFT JOIN (
            //                        Select
            //                            MaLo,
            //                            MaLoaiNguyenLieu,
            //                            MaSize,
            //                            --MaMau,
            //                            MaThanhPham,
            //                            --CaTra,
            //                            DinhMuc,
            //                            Ngay,
            //                            MaXuong
            //                        from
            //                            (
            //                                Select
            //                                    d.*,
            //                                    ROW_NUMBER() OVER (
            //                                        PARTITION BY MaLo,
            //                                        MaLoaiNguyenLieu,
            //                                        --MaMau,
            //                                        MaSize,
            //                                        MaThanhPham,
            //                                        --CaTra,
            //                                        Ngay
            //                                        ORDER BY
            //                                            Gio DESC
            //                                    ) AS [ROW NUMBER]
            //                                from
            //                                    T_DinhMuc d
            //                                where
            //                                    d.Ngay <= @toDate
            //                                    and d.Ngay >= @fromDate
            //                                    And MaXuong = @xuongId
            //                            ) dm
            //                        Where
            //                            dm.[ROW NUMBER] = 1
            //                    ) tp2 on tp1.MaLo = tp2.MaLo
            //                    and tp1.MaLoaiNguyenLieu = tp2.MaLoaiNguyenLieu
            //                    and tp1.MaSize = tp2.MaSize
            //                    --and tp1.MaMau = tp2.MaMau
            //                    and tp1.MaThanhPham = tp2.MaThanhPham
            //                    --and tp1.CaTra = tp2.CaTra
            //                    and tp1.Ngay = tp2.Ngay
            //                    and tp1.MaXuong = tp2.MaXuong
            //            ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
            //            and p.MaLo = DinhMuc.MaLo
            //            and p.MaLoaiNguyenLieu = DinhMuc.MaLoaiNguyenLieu
            //            and p.MaSize = DinhMuc.MaSize
            //            --and p.MaMau = DinhMuc.MaMau
            //            and p.MaThanhPham = DinhMuc.MaThanhPham
            //            --And p.CaTra = DinhMuc.CaTra
            //            and p.Ngay = DinhMuc.Ngay
            //            LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
            //            LEFT Join T_Size s on p.MaSize = s.Ma
            //            LEFT Join T_LoaiNguyenLieu la on p.MaLoaiNguyenLieu = la.Ma
            //            --LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
            //            LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            //			LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
            //        where
            //            p.STT > 0
            //            and p.Ngay <= @toDate
            //            and p.Ngay >= @fromDate
            //			and p.MaKhuVuc = kv.Ma and kv.Ma = @khuVuc
            //        group by
            //            p.MaNhanVien,
            //            p.MaLo,
            //            p.MaLoaiNguyenLieu,
            //            p.MaSize,
            //            p.MaThanhPham,
            //            --p.MaMau,
            //            --p.CaTra,
            //            DinhMuc.DinhMuc,
            //            p.Ngay,
            //            --p.ChiSanLuong,
            //            n.MaHoSo,
            //            n.Name,
            //            la.Ten,
            //            tp.Ten,
            //            --tp.BravoId,
            //            --ma.Ten,
            //            s.Ten,
            //            p.MaXuong,
            //			kv.Ten
            //    ) tp on btp.Ngay = tp.Ngay
            //    --and btp.CaTra = tp.CaTra
            //    and btp.MaLo = tp.MaLo
            //    and btp.MaLoaiNguyenLieu = tp.MaLoaiNguyenLieu
            //    --and btp.MaMau = tp.MaMau
            //    and btp.MaNhanVien = tp.MaNhanVien
            //    and btp.MaSize = tp.MaSize
            //    and btp.MaThanhPham = tp.MaThanhPham
            //    and btp.MaXuong = tp.MaXuong
            //order by
            //    tp.MaHoSo";
            var query = @"Select
    ptp.Ngay,
    ptp.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,
    tp.Ma as MaThanhPham,
    tp.Ten as ThanhPhamName,
    s.Ma as MaSize,
    s.Ten as SizeName,
    sp.Ten as SanPhamName,
    cd.Ten as CongDoanName,
    ptp.MaLo,
    nl.Ten as NhomLo,
    ptp.MaLoaiNguyenLieu,
    la.Ten as LoaiNguyenLieuName,
    SUM(ptp.TrongLuong) as TrongLuong,
    COUNT(*) as SoRo
from
    T_PhieuCan ptp,
    NhanVienDaiThanh n,
    T_ThanhPham tp,
    T_Size s,
    T_LoaiNguyenLieu la,
    T_KhuVuc kv,
    T_NhomLo nl,
    T_SanPham sp,
    T_CongDoan cd
where
   ptp.Ngay <= @toDate
     and ptp.Ngay >= @fromDate
    and ptp.MaXuong = @xuongId
    and ptp.MaNhanVien = n.MaNhanVien
    and ptp.MaThanhPham = tp.Ma
    and ptp.MaSize = s.Ma --and ptp.MaMau = mau.Ma
    and ptp.MaLoaiNguyenLieu = la.Ma
    and ptp.TrongLuong > 0
    and ptp.MaKhuVuc = kv.Ma
    and kv.ma = @khuVuc
    and ptp.MaNhomLo = nl.Ma
    and ptp.MaSanPham = sp.Ma
    and ptp.MaCongDoan = cd.Ma
GROUP by
    ptp.Ngay,
    ptp.MaNhanVien,
    n.MaHoSo,
    n.Name,
    tp.Ma,
    tp.Ten,
    s.Ma,
    s.Ten,
    sp.Ten,
    cd.Ten,
    ptp.MaLo,
    nl.Ten,
    ptp.MaLoaiNguyenLieu,
    la.Ten";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate, toDate, xuongId, khuVuc }).Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetTongHopNhanViens2<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var query = @"Select
    p.*,
    kh.Ten as KhachHangName
from
    (
        Select
            p.Ngay,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
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
            pHC.GioBatDauLoKH
        from
            (
                select
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
p.VoXo2,
                    p.VoXo,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia
                from
                    T_PhieuCan p
                where
                    p.Ngay >= @fromDate
                    and p.Ngay <= @toDate
                    and p.MaXuong = @xuongId
                    and p.MaKhuVuc = @khuVuc
                    and p.TrongLuong > 0
                GROUP BY
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
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
                select
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang,
                    Min(p.Gio) as GioBatDauLoKH
                from
                    T_PhieuCan p
                GROUP BY
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang
            ) pHC on p.Ngay = pHC.Ngay
            and p.MaNhomLo = phc.MaNhomLo
            and p.MaKhachHang = pHC.MaKhachHang
    ) p
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
order by
    MaHoSo DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, khuVuc })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetTongHopNhanViens2<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            string thanhPhamId,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var query = @"Select
    p.*,
    kh.Ten as KhachHangName
from
    (
        Select
            p.Ngay,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.Ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qt.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            tp.Ten as NhomCongViecName,
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
            pHC.GioBatDauLoKH
        from
            (
                select
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia
                from
                    T_PhieuCan p
                where
                    p.Ngay >= @fromDate
                    and p.Ngay <= @toDate
                    and p.MaThanhPham = @thanhPhamId
                    and p.MaXuong = @xuongId
                    and p.MaKhuVuc = @khuVuc
                    and p.TrongLuong > 0
                GROUP BY
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
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
                select
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang,
                    Min(p.Gio) as GioBatDauLoKH
                from
                    T_PhieuCan p
                GROUP BY
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang
            ) pHC on p.Ngay = pHC.Ngay
            and p.MaNhomLo = phc.MaNhomLo
            and p.MaKhachHang = pHC.MaKhachHang
    ) p
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
order by
    MaHoSo DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate, toDate, xuongId, khuVuc, thanhPhamId })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopNhanViensDateTimeToDateTime<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            string thanhPhamId,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var query = @"Select
    p.*,
    kh.Ten as KhachHangName
from
    (
        Select
            p.Ngay,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.Ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qt.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            tp.Ten as NhomCongViecName,
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
            pHC.GioBatDauLoKH
        from
            (
                select
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia
                from
                    T_PhieuCan p
                where
                    CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
                    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime)  <= @toDate
                    and p.MaThanhPham = @thanhPhamId
                    and p.MaXuong = @xuongId
                    and p.MaKhuVuc = @khuVuc
                    and p.TrongLuong > 0
                GROUP BY
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
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
                select
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang,
                    Min(p.Gio) as GioBatDauLoKH
                from
                    T_PhieuCan p
                GROUP BY
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang
            ) pHC on p.Ngay = pHC.Ngay
            and p.MaNhomLo = phc.MaNhomLo
            and p.MaKhachHang = pHC.MaKhachHang
    ) p
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
order by
    MaHoSo DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate, toDate, xuongId, khuVuc, thanhPhamId })
                    .Result
                    .ToList();
                return items;
            }
        }
         public List<T> GetTongHopNhanViensDateTimeToDateTimeEN<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            string thanhPhamId,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var query = @"Select
   p.Ngay as Date,
    p.MaNhanVien as Emp_Id,
    p.TenNhanVien as Emp_Name,
    p.SanPhamName as ProductName,
    p.SizeName,
    p.NhomCongViecName as Process,
    p.MaCongDoan as Task,
    p.MaLo as Lot_No,
    p.TrongLuong as Weight,
    p.SoRo as Pieces
from
    (
        Select
            p.Ngay,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.Ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qt.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            tp.Ten as NhomCongViecName,
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
            pHC.GioBatDauLoKH
        from
            (
                select
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia
                from
                    T_PhieuCan p
                where
                    CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
                    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime)  <= @toDate
                    and p.MaThanhPham = @thanhPhamId
                    and p.MaXuong = @xuongId
                    and p.MaKhuVuc = @khuVuc
                    and p.TrongLuong > 0
                GROUP BY
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
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
                select
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang,
                    Min(p.Gio) as GioBatDauLoKH
                from
                    T_PhieuCan p
                GROUP BY
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang
            ) pHC on p.Ngay = pHC.Ngay
            and p.MaNhomLo = phc.MaNhomLo
            and p.MaKhachHang = pHC.MaKhachHang
    ) p
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
order by
    MaNhanVien DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate, toDate, xuongId, khuVuc, thanhPhamId })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopNhanViensDateTimeToDateTimeEN<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var query = @"Select
    p.Ngay as Date,
    p.MaNhanVien as Emp_Id,
    p.TenNhanVien as Emp_Name,
    p.SanPhamName as ProductName,
    p.SizeName,
    p.NhomCongViecName as Process,
    p.MaCongDoan as Task,
    p.NhomLo as Lot_No,
    P.TrongLuong as Weight,
    p.SoRo as Pieces
from
    (
        Select
            p.Ngay,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.Ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qt.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            tp.Ten as NhomCongViecName,
            p.MaCongDoan,
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
            pHC.GioBatDauLoKH
        from
            (
                select
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia
                from
                    T_PhieuCan p
                where
                    CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
                    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime)  <= @toDate
                   
                    and p.MaXuong = @xuongId
                    and p.MaKhuVuc = @khuVuc
                    and p.TrongLuong > 0
                GROUP BY
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
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
                select
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang,
                    Min(p.Gio) as GioBatDauLoKH
                from
                    T_PhieuCan p
                GROUP BY
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang
            ) pHC on p.Ngay = pHC.Ngay
            and p.MaNhomLo = phc.MaNhomLo
            and p.MaKhachHang = pHC.MaKhachHang
    ) p
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
order by
    p.Ngay DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate = fromDate, toDate = toDate, xuongId, khuVuc })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopNhanViensDateTimeToDateTime<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var query = @"Select
    p.*,
    kh.Ten as KhachHangName
from
    (
        Select
            p.Ngay,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.Ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qt.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            tp.Ten as NhomCongViecName,
            p.MaCongDoan,
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
            pHC.GioBatDauLoKH
        from
            (
                select
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia
                from
                    T_PhieuCan p
                where
                    CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
                    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime)  <= @toDate
                   
                    and p.MaXuong = @xuongId
                    and p.MaKhuVuc = @khuVuc
                    and p.TrongLuong > 0
                GROUP BY
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
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
                select
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang,
                    Min(p.Gio) as GioBatDauLoKH
                from
                    T_PhieuCan p
                GROUP BY
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang
            ) pHC on p.Ngay = pHC.Ngay
            and p.MaNhomLo = phc.MaNhomLo
            and p.MaKhachHang = pHC.MaKhachHang
    ) p
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
order by
    MaHoSo DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate = fromDate, toDate = toDate, xuongId, khuVuc })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopNhanViensToNgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            string thanhPhamId,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var query = @"Select
    p.*,
    kh.Ten as KhachHangName
from
    (
        Select
            p.Ngay,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.Ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qt.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            tp.Ten as NhomCongViecName,
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
            pHC.GioBatDauLoKH
        from
            (
                select
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia
                from
                    T_PhieuCan p,
                    T_NhomLo nl
                where
                    --CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
                    --and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime)  <= @toDate
                    p.MaNhomLo = nl.Ma
                    and nl.NgayNguyenLieu <= @toDate
                    and nl.NgayNguyenLieu >= @fromDate --p.Ngay = @ngay
                    and p.MaThanhPham = @thanhPhamId
                    and p.MaXuong = @xuongId
                    and p.MaKhuVuc = @khuVuc
                    and p.TrongLuong > 0
                GROUP BY
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
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
                select
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang,
                    Min(p.Gio) as GioBatDauLoKH
                from
                    T_PhieuCan p
                GROUP BY
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang
            ) pHC on p.Ngay = pHC.Ngay
            and p.MaNhomLo = phc.MaNhomLo
            and p.MaKhachHang = pHC.MaKhachHang
    ) p
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
order by
    MaHoSo DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate, toDate, xuongId, khuVuc, thanhPhamId })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopNhanViensToNgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId)
        {
            var query = @"Select
    p.*,
    kh.Ten as KhachHangName
from
    (
        Select
            p.Ngay,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.Ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qt.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            tp.Ten as NhomCongViecName,
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
            pHC.GioBatDauLoKH
        from
            (
                select
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia
                from
                    T_PhieuCan p,
                    T_NhomLo nl
                where
                    --CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
                    --and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime)  <= @toDate
                    p.MaNhomLo = nl.Ma
                    and nl.NgayNguyenLieu <= @toDate
                    and nl.NgayNguyenLieu >= @fromDate --p.Ngay = @ngay
                    and p.MaXuong = @xuongId
                    and p.TrongLuong > 0
                GROUP BY
                    p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
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
                select
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang,
                    Min(p.Gio) as GioBatDauLoKH
                from
                    T_PhieuCan p
                GROUP BY
                    p.Ngay,
                    p.MaNhomLo,
                    p.MaKhachHang
            ) pHC on p.Ngay = pHC.Ngay
            and p.MaNhomLo = phc.MaNhomLo
            and p.MaKhachHang = pHC.MaKhachHang
    ) p
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
order by
    MaHoSo DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate = fromDate, toDate = toDate, xuongId })
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
            var query = @"if object_id('tempdb..#phieuCan', 'U') is not null drop table #phieuCan
Select distinct 
	ISNULL(p.MaSizeTP, '') as MaSizeTP,
    ISNULL(p.VoXo2, '') as VoXo2,
    ISNULL(p.MaPhieuPhanCo, '') as MaPhieuPhanCo,
    ISNULL(p.MaQuyTrinh, '') as MaQuyTrinh,
    ISNULL(p.MaKhachHang, '') as MaKhachHang,
    ISNULL(p.MaKhangSinh, '') as MaKhangSinh,
    SUM(p.TrongLuong) as TrongLuong,
    ISNULL(nl.Ten, '') as MaDaiLy into #phieuCan
from T_PhieuCan p,
    T_ThanhPham tp,
    (Select p.Ma,case when LEN(p.Ten)>3 then SUBSTRING(p.Ten,1,3) else p.Ten end as Ten from T_NhomLo p) nl
where CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @ngay
    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and tp.IsPhanCo = 1
    and p.MaNhomLo = nl.Ma
GROUP BY 
    MaSizeTP,
    VoXo2,
    MaPhieuPhanCo,
    MaQuyTrinh,
    MaKhachHang,
    nl.Ten,
    MaKhangSinh
SELECT 
    ROW_NUMBER() OVER (ORDER BY t.MaSizeTP, t.VoXo2, t.MaPhieuPhanCo, t.MaQuyTrinh, t.MaKhachHang, t.MaKhangSinh) AS STT,
	MaSizeTP,
    CONCAT('''',s.Ten) as SizeTPName,
    VoXo2,
    MaPhieuPhanCo,
    CONCAT('''',ppc.Ten) as PhieuPhanCoName,
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
LEFT JOIN T_Size s on MaSizeTP = s.Ma
left JOIN T_PhieuPhanCo ppc on MaPhieuPhanCo = ppc.Ma
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
    ppc.Ten,
    qt.Ten,
    kh.Ten,
    ks.Ten";
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
            var query = @"if object_id('tempdb..#phieuCan', 'U') is not null drop table #phieuCan
Select distinct 
	ISNULL(p.MaSizeTP, '') as MaSizeTP,
    ISNULL(p.VoXo2, '') as VoXo2,
    ISNULL(p.MaPhieuPhanCo, '') as MaPhieuPhanCo,
    ISNULL(p.MaQuyTrinh, '') as MaQuyTrinh,
    ISNULL(p.MaKhachHang, '') as MaKhachHang,
    ISNULL(p.MaKhangSinh, '') as MaKhangSinh,
    SUM(p.TrongLuong) as TrongLuong,
    ISNULL(nl.Ten, '') as MaDaiLy into #phieuCan
from T_PhieuCan p,
    T_ThanhPham tp,
    (Select p.Ma,case when LEN(p.Ten)>3 then SUBSTRING(p.Ten,1,3) else p.Ten end as Ten from T_NhomLo p) nl
where p.NgayNguyenLieu <= @ngay
    and p.NgayNguyenLieu >= @fromDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = tp.Ma
    and tp.IsPhanCo = 1
    and p.MaNhomLo = nl.Ma
GROUP BY 
    MaSizeTP,
    VoXo2,
    MaPhieuPhanCo,
    MaQuyTrinh,
    MaKhachHang,
    nl.Ten,
    MaKhangSinh
SELECT 
    ROW_NUMBER() OVER (ORDER BY t.MaSizeTP, t.VoXo2, t.MaPhieuPhanCo, t.MaQuyTrinh, t.MaKhachHang, t.MaKhangSinh) AS STT,
	MaSizeTP,
    CONCAT('''',s.Ten) as SizeTPName,
    VoXo2,
    MaPhieuPhanCo,
    CONCAT('''',ppc.Ten) as PhieuPhanCoName,
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
LEFT JOIN T_Size s on MaSizeTP = s.Ma
left JOIN T_PhieuPhanCo ppc on MaPhieuPhanCo = ppc.Ma
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
    ppc.Ten,
    qt.Ten,
    kh.Ten,
    ks.Ten";
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
        public List<T> GetTongHopBaoCaoHoaChat<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
           )
        {
            var query = @"SELECT p.*,
    ISNULL(kh.Ten, '') as KhachHangName,
    ISNULL(s.Ten, '') as SizeTPName,
    ISNULL(sp.Ten, '') as SanPhamName,
    ISNULL(qt.Ten, '') as QuyTrinhName,
    ISNULL(ks.Ten, '') as KhangSinhName,
    ISNULL(nhc.Ten, '') as NhomHoaChatName
from (
        SELECT
            ROW_NUMBER()OVER(ORDER BY p.MaKhachHang,p.MaSanPham,p.MaQuyTrinh,p.MaKhangSinh,p.MaNhomHoaChat,p.MaSizeTP,p.VoXo) as STT,
            p.MaKhachHang,
            p.MaSanPham,
            p.MaQuyTrinh,
            p.MaKhangSinh,
            p.MaNhomHoaChat,
            p.MaSizeTP,
            p.VoXo,
            sum(p.TrongLuong) as TrongLuong
        FROM(
                select p.MaKhachHang,
                    P.MaSanPham,
                    p.MaQuyTrinh,
                    p.MaKhangSinh,
                    p.MaNhomHoaChat,
                    p.TrongLuong,
					Concat('''',(
                        case
                            when LEN(p.GhiChu) - LEN(REPLACE(p.GhiChu, '-', '')) >= 2 then LEFT(
                                Right(
                                    p.GhiChu,
                                    LEN(p.GhiChu) - CHARINDEX('-', p.GhiChu)
                                ),
                                CHARINDEX(
                                    '-',
                                    Right(
                                        p.GhiChu,
                                        LEN(p.GhiChu) - CHARINDEX('-', p.GhiChu)
                                    )
                                ) -1
                            )
                            else ''
                        end
                    ))as MaSizeTP,
                    (
                        case
                            when LEN(p.GhiChu) - LEN(REPLACE(p.GhiChu, ',', '')) >= 2 then LEFT(
                                Right(
                                    p.GhiChu,
                                    LEN(p.GhiChu) - CHARINDEX(',', p.GhiChu)
                                ),
                                CHARINDEX(
                                    ',',
                                    Right(
                                        p.GhiChu,
                                        LEN(p.GhiChu) - CHARINDEX(',', p.GhiChu)
                                    )
                                ) -1
                            )
                            else ''
                        end
                    ) as VoXo
                from T_PhieuCan p,
                    T_ThanhPham tp
                where CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @ngay
                    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
                    and p.MaXuong = @xuongId
					and p.MaThanhPham = tp.Ma
                    and tp.IsHoaChat = 1
                    and tp.IsHoaChatMain = 1
            ) p
        GROUP BY p.MaKhachHang,
            P.MaSanPham,
            p.MaQuyTrinh,
            p.MaKhangSinh,
            p.MaNhomHoaChat,
            p.MaSizeTP,
            p.VoXo
    ) p
    LEFT JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
    LEFT JOIN T_SanPham sp on p.MaSanPham = sp.Ma
    LEFT JOIN T_QuyTrinh qt on p.MaQuyTrinh = qt.Ma
    LEFT JOIN T_KhangSinh ks on p.MaKhangSinh = ks.Ma
    LEFT JOIN T_NhomHoaChat nhc on p.MaNhomHoaChat = nhc.Ma
    LEFT JOIN T_Size s on p.MaSizeTP = s.Ma";
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
        public List<T> GetTongHopBaoCaoHoaChatNgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
           )
        {
            var query = @"SELECT p.*,
    ISNULL(kh.Ten, '') as KhachHangName,
    ISNULL(s.Ten, '') as SizeTPName,
    ISNULL(sp.Ten, '') as SanPhamName,
    ISNULL(qt.Ten, '') as QuyTrinhName,
    ISNULL(ks.Ten, '') as KhangSinhName,
    ISNULL(nhc.Ten, '') as NhomHoaChatName
from (
        SELECT 
            ROW_NUMBER()OVER(ORDER BY p.MaKhachHang,p.MaSanPham,p.MaQuyTrinh,p.MaKhangSinh,p.MaNhomHoaChat,p.MaSizeTP,p.VoXo) as STT,
            p.MaKhachHang,
            p.MaSanPham,
            p.MaQuyTrinh,
            p.MaKhangSinh,
            p.MaNhomHoaChat,
            p.MaSizeTP,
            p.VoXo,
            sum(p.TrongLuong) as TrongLuong
        FROM(
                select p.MaKhachHang,
                    P.MaSanPham,
                    p.MaQuyTrinh,
                    p.MaKhangSinh,
                    p.MaNhomHoaChat,
                    p.TrongLuong,
                    (
                        case
                            when LEN(p.GhiChu) - LEN(REPLACE(p.GhiChu, '-', '')) >= 2 then LEFT(
                                Right(
                                    p.GhiChu,
                                    LEN(p.GhiChu) - CHARINDEX('-', p.GhiChu)
                                ),
                                CHARINDEX(
                                    '-',
                                    Right(
                                        p.GhiChu,
                                        LEN(p.GhiChu) - CHARINDEX('-', p.GhiChu)
                                    )
                                ) -1
                            )
                            else ''
                        end
                    ) as MaSizeTP,
                    (
                        case
                            when LEN(p.GhiChu) - LEN(REPLACE(p.GhiChu, ',', '')) >= 2 then LEFT(
                                Right(
                                    p.GhiChu,
                                    LEN(p.GhiChu) - CHARINDEX(',', p.GhiChu)
                                ),
                                CHARINDEX(
                                    ',',
                                    Right(
                                        p.GhiChu,
                                        LEN(p.GhiChu) - CHARINDEX(',', p.GhiChu)
                                    )
                                ) -1
                            )
                            else ''
                        end
                    ) as VoXo
                from T_PhieuCan p,
                    T_ThanhPham tp
                where p.NgayNguyenLieu <= @ngay
                    and p.NgayNguyenLieu >= @fromDate
                    and p.MaXuong = @xuongId
                    and p.MaThanhPham = tp.Ma
                    and tp.IsHoaChat = 1
                    and tp.IsHoaChatMain = 1
            ) p
        GROUP BY p.MaKhachHang,
            P.MaSanPham,
            p.MaQuyTrinh,
            p.MaKhangSinh,
            p.MaNhomHoaChat,
            p.MaSizeTP,
            p.VoXo
    ) p
    LEFT JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
    LEFT JOIN T_SanPham sp on p.MaSanPham = sp.Ma
    LEFT JOIN T_QuyTrinh qt on p.MaQuyTrinh = qt.Ma
    LEFT JOIN T_KhangSinh ks on p.MaKhangSinh = ks.Ma
    LEFT JOIN T_NhomHoaChat nhc on p.MaNhomHoaChat = nhc.Ma
    LEFT JOIN T_Size s on p.MaSizeTP = s.Ma";
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
        public List<T> GetBaoCaoTongHopHangNgay<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId
           )
        {
            var query = @"select 
ISNULL(nv.DeptName0,'') AS NhomName,
ISNULL(sp.Ten,'') as SanPhamName,
ISNULL(cd.Ten,'') as CongDoanName,
sum(p.TrongLuong) as TrongLuong
from 
(select 
p.MaNhanVien,
p.MaSanPham,
p.MaCongDoan,
p.TrongLuong
from 
	T_PhieuCan p
where 
	CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @ngay
    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
    and p.MaXuong = @xuongId
	)p
	left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
	left join T_SanPham sp on p.MaSanPham = sp.Ma
	left join T_CongDoan cd on p.MaCongDoan = cd.Ma
group by
nv.DeptName0,
sp.Ten,
cd.Ten";
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
from T_PhieuCan p
    LEFT JOIN T_ThanhPham tp ON p.MaThanhPham = tp.Ma
where CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @ngay
    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
    and p.MaXuong = @xuongId
    and tp.IsPhanCo = 1";
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
    LEFT JOIN T_ThanhPham tp ON p.MaThanhPham = tp.Ma
where p.NgayNguyenLieu <= @ngay
    and p.NgayNguyenLieu >= @fromDate
    and p.MaXuong = @xuongId
    and tp.IsPhanCo = 1";
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
        public List<T> GetNgayAndNguyenLieuHoaChat<T>(
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
    LEFT JOIN T_ThanhPham tp ON p.MaThanhPham = tp.Ma
where CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @ngay
    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
    and p.MaXuong = @xuongId
    and tp.IsHoaChat = 1";
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
        public List<T> GetNgayAndNguyenLieuHoaChat_NgayNguyenLieu<T>(
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
    LEFT JOIN T_ThanhPham tp ON p.MaThanhPham = tp.Ma
where p.NgayNguyenLieu <= @ngay
    and p.NgayNguyenLieu >= @fromDate
    and p.MaXuong = @xuongId
    and tp.IsHoaChat = 1";
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
        public List<T> GetTongHopNhanViens_SanPham<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            //                var query = @"Select
            //    IsNull(tp.Ngay, btp.Ngay) as Ngay,
            //    IsNull(tp.MaNhanVien, btp.MaNhanVien) as MaNhanVien,
            //    ISNULL(tp.MaHoSo, btp.MaHoSo) as MaHoSo,
            //    ISNULL(tp.NhanVienName, btp.NhanVienName) as NhanVienName,
            //    IsNull(tp.MaLo, btp.MaLo) as MaLo,
            //    ISNULL(tp.MaLoaiNguyenLieu, btp.MaLoaiNguyenLieu) as MaLoaiNguyenLieu,
            //    ISNULL(tp.LoaiNguyenLieuName, btp.LoaiNguyenLieuName) as LoaiNguyenLieuName,
            //    ISNULL(tp.MaSize, btp.MaSize) as MaSize,
            //    ISNULL(tp.SizeName, btp.SizeName) as SizeName,
            //    ISNULL(tp.MaThanhPham, btp.MaThanhPham) as MaThanhPham,
            //    ISNULL(tp.ThanhPhamName, btp.ThanhPhamName) as ThanhPhamName,
            //    --ISNULL(tp.MaSanPham, btp.MaSanPham) as MaSanPham,
            //    --ISNULL(tp.MaMau, btp.MaMau) as MaMau,
            //    --ISNULL(tp.MauName, btp.MauName) as MauName,
            //    --ISNULL(tp.CaTra, btp.CaTra) as CaTra,
            //    --IsNull(tp.ChiSanLuong, btp.ChiSanLuong) as ChiSanLuong,
            //    ISNULL(tp.TrongLuongNhan, 0) as TrongLuongNhan,
            //    ISNULL(tp.TrongLuongTra, 0) as TrongLuongTra,
            //    ISNULL(tp.DinhMucThucTe, 0) as DinhMuc,
            //    ISNULL(tp.DinhMucYeuCau, 0) as DinhMucChuan,
            //    cast(
            //        (
            //            Case
            //                when @isDinhMucBinhThuong = 1 then (
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
            //    ISNULL(tp.MaXuong, btp.MaXuong) as MaXuong,
            //	ISNULL(tp.KhuVucName,btp.KhuVucName) as KhuVucName
            //from
            //    (
            //        Select
            //            p.Ngay,
            //            p.MaNhanVien,
            //            n.MaHoSo,
            //            n.Name as NhanVienName,
            //            la.Ten as LoaiNguyenLieuName,
            //            s.Ten as SizeName,
            //            tp.Ten as ThanhPhamName,
            //            --tp.BravoId as MaSanPham,
            //            --ma.Ten as MauName,
            //            p.MaLoaiNguyenLieu,
            //            p.MaLo,
            //            --p.MaMau,
            //            p.MaThanhPham,
            //            p.MaSize,
            //            --p.CaTra,
            //            --p.ChiSanLuong,
            //            p.MaXuong,
            //            COUNT(*) as SoRo,
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
            //            ) as SoRoChuaCanTP,
            //			kv.Ten as KhuVucName
            //        from
            //            T_PhieuCan p
            //            LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
            //            LEFT Join T_Size s on p.MaSize = s.Ma
            //            LEFT Join T_LoaiNguyenLieu la on p.MaLoaiNguyenLieu = la.Ma
            //            --LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
            //            LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            //			LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
            //        where
            //            p.STT > 0
            //            and p.Ngay <= @toDate
            //            and p.Ngay >= @fromDate
            //			and p.MaKhuVuc = kv.Ma and kv.Ma = @khuVuc
            //        GROUP BY
            //            p.Ngay,
            //            p.MaNhanVien,
            //            p.MaLoaiNguyenLieu,
            //			p.MaKhuVuc,
            //			kv.Ten,
            //            p.MaLo,
            //            --p.MaMau,
            //            p.MaThanhPham,
            //            p.MaSize,
            //            --p.CaTra,
            //            --p.ChiSanLuong,
            //            p.MaXuong,
            //            n.MaHoSo,
            //            n.Name,
            //            la.Ten,
            //            --ma.Ten,
            //            tp.Ten,
            //            s.Ten

            //            --tp.BravoId
            //    ) btp Full
            //    outer join (
            //        Select
            //            p.Ngay,
            //            p.MaNhanVien,
            //            n.MaHoSo,
            //            n.Name as NhanVienName,
            //            p.MaLo,
            //            p.MaLoaiNguyenLieu,
            //            la.Ten as LoaiNguyenLieuName,
            //            p.MaSize,
            //            s.Ten as SizeName,
            //            p.MaThanhPham,
            //            tp.Ten as ThanhPhamName,
            //            --tp.BravoId as MaSanPham,
            //            --p.MaMau,
            //            --ma.Ten as MauName,
            //            --p.CaTra,
            //            --p.ChiSanLuong,
            //            Sum(p.TrongLuongNhan) as TrongLuongNhan,
            //            Sum(p.TrongLuong) as TrongLuongTra,
            //            case
            //                when @isDinhMucBinhThuong = 1 then cast (
            //                    case
            //                        when sum(p.TrongLuong) = 0 then 0
            //                        else (
            //                            case
            //                                when @isFloor = 1 then (
            //                                    FLOOR(
            //                                        (Sum(p.TrongLuongNhan) / sum(p.TrongLuong)) * 100
            //                                    ) / 100
            //                                )
            //                                else Sum(p.TrongLuongNhan) / sum(p.TrongLuong)
            //                            end
            //                        )
            //                    end as decimal(18, 2)
            //                )
            //                ELSE cast (
            //                    case
            //                        when sum(p.TrongLuongNhan) = 0 then 0
            //                        else sum(p.TrongLuong) / sum(p.TrongLuongNhan)
            //                    end as decimal(18, 4)
            //                )
            //            end as DinhMucThucTe,
            //            DinhMuc.DinhMuc as DinhMucYeuCau,
            //            Count(*) as SoRo,
            //            p.MaXuong,
            //			kv.Ten as KhuVucName
            //        from
            //            T_PhieuCan p
            //            LEFT JOIN (
            //                Select
            //                    tp1.MaLo,
            //                    tp1.MaLoaiNguyenLieu,
            //                    tp1.MaSize,
            //                    --tp1.MaMau,
            //                    tp1.MaThanhPham,
            //                    --tp1.CaTra,
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
            //                            p.MaLoaiNguyenLieu,
            //                            p.MaSize,
            //                            --p.MaMau,
            //                            p.MaThanhPham,
            //                            --p.CaTra,
            //                            --CASE
            //                            --    WHEN p.CaTra = 1 THEN 1.37
            //                            --    ELSE tp.DinhMuc
            //                            --END AS DinhMuc,
            //							1 as DinhMuc,
            //                            p.Ngay,
            //                            p.MaXuong
            //                        from
            //                            T_PhieuCan p,
            //                            T_ThanhPham tp
            //                        where
            //                            p.Ngay <= @toDate
            //                            and p.Ngay >= @fromDate
            //                            and MaXuong = @xuongId
            //                            and p.MaThanhPham = tp.Ma
            //                            --and p.MaLoaiNguyenLieu = tp.MaCa
            //                    ) tp1
            //                    LEFT JOIN (
            //                        Select
            //                            MaLo,
            //                            MaLoaiNguyenLieu,
            //                            MaSize,
            //                            --MaMau,
            //                            MaThanhPham,
            //                            --CaTra,
            //                            DinhMuc,
            //                            Ngay,
            //                            MaXuong
            //                        from
            //                            (
            //                                Select
            //                                    d.*,
            //                                    ROW_NUMBER() OVER (
            //                                        PARTITION BY MaLo,
            //                                        MaLoaiNguyenLieu,
            //                                        --MaMau,
            //                                        MaSize,
            //                                        MaThanhPham,
            //                                        --CaTra,
            //                                        Ngay
            //                                        ORDER BY
            //                                            Gio DESC
            //                                    ) AS [ROW NUMBER]
            //                                from
            //                                    T_DinhMuc d
            //                                where
            //                                    d.Ngay <= @toDate
            //                                    and d.Ngay >= @fromDate
            //                                    And MaXuong = @xuongId
            //                            ) dm
            //                        Where
            //                            dm.[ROW NUMBER] = 1
            //                    ) tp2 on tp1.MaLo = tp2.MaLo
            //                    and tp1.MaLoaiNguyenLieu = tp2.MaLoaiNguyenLieu
            //                    and tp1.MaSize = tp2.MaSize
            //                    --and tp1.MaMau = tp2.MaMau
            //                    and tp1.MaThanhPham = tp2.MaThanhPham
            //                    --and tp1.CaTra = tp2.CaTra
            //                    and tp1.Ngay = tp2.Ngay
            //                    and tp1.MaXuong = tp2.MaXuong
            //            ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
            //            and p.MaLo = DinhMuc.MaLo
            //            and p.MaLoaiNguyenLieu = DinhMuc.MaLoaiNguyenLieu
            //            and p.MaSize = DinhMuc.MaSize
            //            --and p.MaMau = DinhMuc.MaMau
            //            and p.MaThanhPham = DinhMuc.MaThanhPham
            //            --And p.CaTra = DinhMuc.CaTra
            //            and p.Ngay = DinhMuc.Ngay
            //            LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
            //            LEFT Join T_Size s on p.MaSize = s.Ma
            //            LEFT Join T_LoaiNguyenLieu la on p.MaLoaiNguyenLieu = la.Ma
            //            --LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
            //            LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            //			LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
            //        where
            //            p.STT > 0
            //            and p.Ngay <= @toDate
            //            and p.Ngay >= @fromDate
            //			and p.MaKhuVuc = kv.Ma and kv.Ma = @khuVuc
            //        group by
            //            p.MaNhanVien,
            //            p.MaLo,
            //            p.MaLoaiNguyenLieu,
            //            p.MaSize,
            //            p.MaThanhPham,
            //            --p.MaMau,
            //            --p.CaTra,
            //            DinhMuc.DinhMuc,
            //            p.Ngay,
            //            --p.ChiSanLuong,
            //            n.MaHoSo,
            //            n.Name,
            //            la.Ten,
            //            tp.Ten,
            //            --tp.BravoId,
            //            --ma.Ten,
            //            s.Ten,
            //            p.MaXuong,
            //			kv.Ten
            //    ) tp on btp.Ngay = tp.Ngay
            //    --and btp.CaTra = tp.CaTra
            //    and btp.MaLo = tp.MaLo
            //    and btp.MaLoaiNguyenLieu = tp.MaLoaiNguyenLieu
            //    --and btp.MaMau = tp.MaMau
            //    and btp.MaNhanVien = tp.MaNhanVien
            //    and btp.MaSize = tp.MaSize
            //    and btp.MaThanhPham = tp.MaThanhPham
            //    and btp.MaXuong = tp.MaXuong
            //order by
            //    tp.MaHoSo";
            var query = @"Select
    ptp.Ngay,
    ptp.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,
   
   
    sp.Ten as SanPhamName,
    cd.Ten as CongDoanName,
    
    SUM(ptp.TrongLuong) as TrongLuong,
    COUNT(*) as SoRo
from
    T_PhieuCan ptp,
    NhanVienDaiThanh n,
   
    T_KhuVuc kv,
    
    T_SanPham sp,
    T_CongDoan cd
where
   ptp.Ngay <= @toDate
     and ptp.Ngay >= @fromDate
    and ptp.MaXuong = @xuongId
    and ptp.MaNhanVien = n.MaNhanVien
    
    and ptp.TrongLuong > 0
    and ptp.MaKhuVuc = kv.Ma
    and kv.ma = @khuVuc
   
    and ptp.MaSanPham = sp.Ma
    and ptp.MaCongDoan = cd.Ma
GROUP by
    ptp.Ngay,
    ptp.MaNhanVien,
    n.MaHoSo,
    n.Name ,
   
   
    sp.Ten,
    cd.Ten";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { fromDate, toDate, xuongId, khuVuc }).Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetTongHopDanhGiaDinhMucs<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true,
            //bool isCaTraChuyenDoi = true,
            bool isFloor = true)
        {
            var query = @"SELECT
    p.Ngay,
    p.MaNhanVien,
    p.MaHoSo,
    p.NhanVienName as TenNhanVien,
    p.IsGiaCong,
    p.MaLo,
    p.MaLoaiNguyenLieu,
    p.LoaiNguyenLieuName,
    p.MaSize,
    p.SizeName,
    p.MaThanhPham,
    p.ThanhPhamName,
    --p.MaSanPham,
    --p.MaMau,
    --p.MauName,
    --p.CaTra,
    --p.ChiSanLuong,
    p.TrongLuongNhan,
    p.TrongLuong,
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
    end as DinhMuc,
    p.DinhMucChuan,
    p.DanhGia,
    p.DonGia,
    p.ThanhTien,
    p.SoRo,
	p.MaKhuVuc
from
    (
        Select
            p.Ngay,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as NhanVienName,
            n.IsGiaCong,
            p.MaLo,
            p.MaLoaiNguyenLieu,
            la.Ten as LoaiNguyenLieuName,
            p.MaSize,
            s.Ten as SizeName,
            p.MaThanhPham,
            tp.Ten as ThanhPhamName,
            --tp.BravoId as MaSanPham,
            --p.MaMau,
            --ma.Ten as MauName,
            --p.CaTra,
            --p.ChiSanLuong,
            Sum(p.TrongLuongNhan) as TrongLuongNhan,
            Sum(p.TrongLuong) as TrongLuong,
            case
                when @isDinhMucBinhThuong = 1 then cast (
                    case
                        when sum(p.TrongLuong) = 0 then 0
                        else (
                            case
                                when @isFloor = 1 then (
                                    FLOOR(
                                        (Sum(p.TrongLuongNhan) / sum(p.TrongLuong)) * 100
                                    ) / 100
                                )
                                else Sum(p.TrongLuongNhan) / sum(p.TrongLuong)
                            end
                        )
                    end as decimal(18, 2)
                )
                ELSE cast (
                    case
                        when sum(p.TrongLuongNhan) = 0 then 0
                        else ROUND(
                            sum(p.TrongLuong) / sum(p.TrongLuongNhan),
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
                            when sum(p.TrongLuong) = 0 then 0
                            when (
                                case
                                    when @isFloor = 1 then (
                                        FLOOR(
                                            (Sum(p.TrongLuongNhan) / sum(p.TrongLuong)) * 100
                                        ) / 100
                                    )
                                    else Sum(p.TrongLuongNhan) / sum(p.TrongLuong)
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
                                    sum(p.TrongLuong) / sum(p.TrongLuongNhan),
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
            Count(*) as SoRo,
			p.MaKhuVuc as MaKhuVuc
			
        from
            (
				Select *
				from
					T_PhieuCan p
				where 
					p.Ngay <= @toDate
					and p.Ngay >= @fromDate
					and MaXuong = @xuongId
					and p.MaKhuVuc = @khuVuc
			) p
    
            LEFT JOIN (
                Select
                    tp1.MaLo,
                    tp1.MaLoaiNguyenLieu,
                    tp1.MaSize,
                    --tp1.MaMau,
                    --CASE
                    --    WHEN tp1.CaTra = 1
                    --    and @isCaTraChuyenDoi = 1 THEN 'B'
                    --    ELSE tp1.MaThanhPham
                    --END as MaThanhPham,
					tp1.MaThanhPham as MaThanhPham,
                    --tp1.CaTra,
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
                            p.MaLoaiNguyenLieu,
                            p.MaSize,
                            --p.MaMau,
                            p.MaThanhPham,
                            --p.CaTra,
							--CASE
							--  WHEN p.CaTra = 1 THEN 1.37
							--  ELSE tp.DinhMuc
							--END AS DinhMuc,
							tp.DinhMuc as DinhMuc,
                            p.Ngay,
                            p.MaXuong
							
                        from
                            T_PhieuCan p,
                            T_ThanhPham tp
							
                        where
                            p.Ngay <= @toDate
                            and p.Ngay >= @fromDate
                            and MaXuong = @xuongId
                            and p.MaThanhPham = tp.Ma
							
                            --and p.MaLoaiNguyenLieu = tp.MaCa
                    ) tp1
                    LEFT JOIN (
                        Select
                            MaLo,
                            MaLoaiNguyenLieu,
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
                                        MaLoaiNguyenLieu,
                                        --MaMau,
                                        MaSize,
                                        MaThanhPham,
                                        --CaTra,
                                        Ngay
                                        ORDER BY
                                            Gio DESC
                                    ) AS [ROW NUMBER]
                                from
                                    T_DinhMuc d
                                where
                                    d.Ngay <= @toDate
                                    and d.Ngay >= @fromDate
                                    And MaXuong = @xuongId
                            ) dm
                        Where
                            dm.[ROW NUMBER] = 1
                    ) tp2 on tp1.MaLo = tp2.MaLo
                    and tp1.MaLoaiNguyenLieu = tp2.MaLoaiNguyenLieu
                    and tp1.MaSize = tp2.MaSize
                    --and tp1.MaMau = tp2.MaMau
                    and tp1.MaThanhPham = tp2.MaThanhPham
                    --and tp1.CaTra = tp2.CaTra
                    and tp1.Ngay = tp2.Ngay
                    and tp1.MaXuong = tp2.MaXuong
            ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
            and p.MaLo = DinhMuc.MaLo
            and p.MaLoaiNguyenLieu = DinhMuc.MaLoaiNguyenLieu
            and p.MaSize = DinhMuc.MaSize
            --and p.MaMau = DinhMuc.MaMau
            and p.MaThanhPham = DinhMuc.MaThanhPham
            --And p.CaTra = DinhMuc.CaTra
            and p.Ngay = DinhMuc.Ngay
            LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
            LEFT Join T_Size s on p.MaSize = s.Ma
            LEFT Join T_LoaiNguyenLieu la on p.MaLoaiNguyenLieu = la.Ma
            --LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
            LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
        where
            p.STT > 0
            and p.Ngay <= @toDate
            and p.Ngay >= @fromDate
        group by
            p.MaNhanVien,
            p.MaLo,
            p.MaLoaiNguyenLieu,
            p.MaSize,
            p.MaThanhPham,
            --p.MaMau,
            --p.CaTra,
            DinhMuc.DinhMuc,
            p.Ngay,
            --p.ChiSanLuong,
            n.MaHoSo,
            n.Name,
            n.IsGiaCong,
            la.Ten,
            tp.Ten,
            --tp.BravoId,
            --ma.Ten,
            s.Ten,
			p.MaKhuVuc
    ) p";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new
                        {
                            fromDate, toDate, xuongId, khuVuc, isDinhMucBinhThuong, /*isCaTraChuyenDoi,*/ isFloor
                        })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetChiTietTheoLos<T>(
            DateTime dateTime,
            string xuongId,
            string MaLo,
            bool isDinhMucBinhThuong = true)
        {
            var query = @"Select
    tp.STT,
	tp.Ngay,
    tp.Gio,
    tp.MaNhanVien,
    tp.MaHoSo,
    tp.TenNhanVien,
    tp.ThanhPhamName,
    tp.SizeName,
    tp.LoaiNguyenLieu,
	tp.MaThe,
    --tp.MauName,
    --Case
    --    when tp.SizeName = N'Cá Lớn' then 'L'
    --    when tp.SizeName = N'Cá Nhỏ' then 'N'
    --    else tp.SizeName
    --end as SizeName,
    --case
    --    when tp.CaTra = 1 then 'True'
    --    when tp.CaTra = 0 then 'F'
    --    else ''
    --end as CaTra,
    --tp.ChiSanLuong,
    tp.MaLo,
    tp.DinhMuc,
    --dm.DinhMuc as DinhMucChuan,
    tp.TrongLuong,
	tp.TrongLuongNhan,
	tp.MayCan,
    tp.TrongLuongTare,
    0 as DonGia,
    0 as ThanhTien,
   -- Case
   --     when @isDinhMucBinhThuong = 1 then case
   --         when tp.DinhMuc <= tp.DinhMuc then N'Đạt'
   --         else N'Không Đạt'
   --     end
   --     else case
   --         when tp.DinhMuc >= tp.DinhMuc then N'Đạt'
   --         else N'Không Đạt'
   --     end
   --end as DanhGia,
    tp.KhuVuc,
	tp.TenXuong
	
from
    (
        Select
            ptp.STT,
            ptp.Gio,
			ptp.Ngay,
            ptp.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            tp.Ma as MaThanhPham,
            tp.Ten as ThanhPhamName,
            s.Ma as MaSize,
            s.Ten as SizeName,
            --ptp.CaTra,
            --ptp.ChiSanLuong,
            ptp.MaLo,
            --ptp.MaMau,
            ptp.MaLoaiNguyenLieu,
            la.Ten as LoaiNguyenLieu,
			ptp.MaThe,
			ptp.TrongLuongNhan,
			ptp.TrongLuongTare,
			
            --mau.Ten as MauName,
            case
               when @isDinhMucBinhThuong = 1 then floor(
                    100 * ptp.TrongLuongNhan / ptp.TrongLuong
                ) / 100
                else ptp.TrongLuong / ptp.TrongLuongNhan
            end as DinhMuc,
            ptp.TrongLuong,
            ptp.MayCan,
			kv.Ten as KhuVuc,
			xn.Ten as TenXuong


        from
            T_PhieuCan ptp,
            NhanVienDaiThanh n,
            T_ThanhPham tp,
            T_Size s,
            T_LoaiNguyenLieu la,
			T_KhuVuc kv,
			XiNghiep xn
			--T_LoNguyenLieu lo
        where
            ptp.Ngay = @ngay
            and ptp.MaXuong = xn.Ma and xn.Ma =@xuongId
			and ptp.MaKhuVuc = kv.Ma 
			--and kv.Ma = @khuVuc
			and ptp.MaLo = @MaLo
            and ptp.MaNhanVien = n.MaNhanVien
            and ptp.MaThanhPham = tp.Ma
            and ptp.MaSize = s.Ma
            --and ptp.MaMau = mau.Ma
            and ptp.MaLoaiNguyenLieu = la.Ma
            and ptp.TrongLuong > 0
    ) tp
    --left join(
    --    Select
    --        *
    --    from
    --        (
    --            Select
    --                d.*,
    --                ROW_NUMBER() OVER (
    --                    PARTITION BY MaLo,
    --                    MaLoaiCa,
    --                    MaSize,
    --                    MaThanhPham
                       
    --                    ORDER BY
    --                        Gio DESC
    --                ) AS [ROW NUMBER]
    --            from
    --                DinhMucDinhHinh d
    --            where
    --                Ngay = @ngay
    --                And MaXuong = @xuongId
    --        ) dm
    --    Where
    --        dm.[ROW NUMBER] = 1
    --) dm on tp.MaThanhPham = dm.MaThanhPham
    --and tp.MaLo = dm.MaLo
    --and tp.MaSize = dm.MaSize
    ----and tp.CaTra = dm.CaTra
    ----and tp.MaMau = dm.MaMau
    --and tp.MaLoaiNguyenLieu = dm.MaLoaiCa
order by
    Gio";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId, isDinhMucBinhThuong, MaLo })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetTongHopLoaiThanhPhams<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var query = @"Select
    p.Ngay,
    p.MaLoaiNguyenLieu,
    la.Ten as LoaiNguyenLieuName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    --p.MaMau,
    --ma.Ten as MauName,
    --p.CaTra,
    --p.ChiSanLuong,
    Sum(p.TrongLuongNhan) as TrongLuongNhan,
    Sum(p.TrongLuong) as TrongLuong,
    case
        when @isDinhMucBinhThuong = 1 then cast (
            case
                when sum(p.TrongLuong) = 0 then 0
                else (
                    case
                        when @isFloor = 1 then (
                            FLOOR(
                                (Sum(p.TrongLuongNhan) / sum(p.TrongLuong)) * 100
                            ) / 100
                        )
                        else Sum(p.TrongLuongNhan) / sum(p.TrongLuong)
                    end
                )
            end as decimal(18, 2)
        )
        ELSE cast (
            case
                when sum(p.TrongLuongNhan) = 0 then 0
                else sum(p.TrongLuong) / sum(p.TrongLuongNhan)
            end as decimal(18, 4)
        )
    end as DinhMuc,
    DinhMuc.DinhMuc as DinhMucChuan,
    Count(*) as SoRo,
	kv.Ten	as KhuVucName
from
    (Select * from
            T_PhieuCan p where p.Ngay <= @toDate
                            and p.Ngay >= @fromDate
                            and MaXuong = @xuongId) p
    LEFT JOIN (
        Select
            tp1.MaLo,
            tp1.MaLoaiNguyenLieu,
            tp1.MaSize,
            --tp1.MaMau,
            --CASE
            --    WHEN tp1.CaTra = 1
            --    and @isCaTraChuyenDoi = 1 THEN 'B'
            --    ELSE tp1.MaThanhPham
            --END as MaThanhPham,
			tp1.MaThanhPham as MaThanhPham,
            --tp1.CaTra,
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
                    p.MaLoaiNguyenLieu,
                    p.MaSize,
                    --p.MaMau,
                    p.MaThanhPham,
                    --p.CaTra,
                    --CASE
                    --    WHEN p.CaTra = 1 THEN 1.37
                    --    ELSE tp.DinhMuc
                    --END AS DinhMuc,
					tp.DinhMuc as DinhMuc,
                    p.Ngay,
                    p.MaXuong
                from
                    T_PhieuCan p,
                    T_ThanhPham tp
                where
                    p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and MaXuong = @xuongId
                    and p.MaThanhPham = tp.Ma
                    --and p.MaLoaiNguyenLieu = tp.MaCa
            ) tp1
            LEFT JOIN (
                Select
                    MaLo,
                    MaLoaiNguyenLieu,
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
                                MaLoaiNguyenLieu,
                                --MaMau,
                                MaSize,
                                MaThanhPham,
                                --CaTra,
                                Ngay
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            T_DinhMuc d
                        where
                            d.Ngay <= @toDate
                            and d.Ngay >= @fromDate
                            And MaXuong = @xuongId
                    ) dm
                Where
                    dm.[ROW NUMBER] = 1
            ) tp2 on tp1.MaLo = tp2.MaLo
            and tp1.MaLo = tp2.MaLoaiNguyenLieu
            and tp1.MaSize = tp2.MaSize
            --and tp1.MaMau = tp2.MaMau
            and tp1.MaThanhPham = tp2.MaThanhPham
            --and tp1.CaTra = tp2.CaTra
            and tp1.Ngay = tp2.Ngay
            and tp1.MaXuong = tp2.MaXuong
    ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
    and p.MaLo = DinhMuc.MaLo
    and p.MaLoaiNguyenLieu = DinhMuc.MaLoaiNguyenLieu
    and p.MaSize = DinhMuc.MaSize
    --and p.MaMau = DinhMuc.MaMau
    and p.MaThanhPham = DinhMuc.MaThanhPham
    --And p.CaTra = DinhMuc.CaTra
    and p.Ngay = DinhMuc.Ngay
    LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
    LEFT Join T_Size s on p.MaSize = s.Ma
    LEFT Join T_LoaiNguyenLieu la on p.MaLoaiNguyenLieu = la.Ma
    --LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
	LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
where
    p.STT > 0
    and p.Ngay <= @toDate
    and p.Ngay >= @fromDate
	and p.MaKhuVuc = kv.Ma and kv.Ma = @khuVuc
group by
    p.MaLoaiNguyenLieu,
    p.MaSize,
    p.MaThanhPham,
    --p.MaMau,
    --p.CaTra,
    DinhMuc.DinhMuc,
    p.Ngay,
    --p.ChiSanLuong,
    la.Ten,
    tp.Ten,
    --ma.Ten,
    s.Ten,
	kv.Ten";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { fromDate, toDate, xuongId, khuVuc, isDinhMucBinhThuong, isFloor })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetTongHopTheoLos<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var query = @"Select
    p.Ngay,
	p.MaLo,
	lnl.Ten as LoNguyenLieuName,
	p.MaNhaCungCap as MaNCC,
	lnl.MaNhaCungCap as MaNccLNL,
	ncc.Ten as NhaCungCapName,
	p.MaNhomLo as MaNL,
	lnl.MaNhomLo as MaNlLnl,
	nl.Ten as NhomLoName,
    p.MaLoaiNguyenLieu,
    la.Ten as LoaiNguyenLieuName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    --p.MaMau,
    --ma.Ten as MauName,
    --p.CaTra,
    --p.ChiSanLuong,
    Sum(p.TrongLuongNhan) as TrongLuongNhan,
    Sum(p.TrongLuong) as TrongLuong,
    case
        when @isDinhMucBinhThuong = 1 then cast (
            case
                when sum(p.TrongLuong) = 0 then 0
                else (
                    case
                        when @isFloor = 1 then (
                            FLOOR(
                                (Sum(p.TrongLuongNhan) / sum(p.TrongLuong)) * 100
                            ) / 100
                        )
                        else Sum(p.TrongLuongNhan) / sum(p.TrongLuong)
                    end
                )
            end as decimal(18, 2)
        )
        ELSE cast (
            case
                when sum(p.TrongLuongNhan) = 0 then 0
                else sum(p.TrongLuong) / sum(p.TrongLuongNhan)
            end as decimal(18, 4)
        )
    end as DinhMuc,
    DinhMuc.DinhMuc as DinhMucChuan,
    Count(*) as SoRo,
	kv.Ten	as KhuVucName
	
from
    (Select * from
            T_PhieuCan p where p.Ngay <= @toDate
                            and p.Ngay >= @fromDate
                            and MaXuong = @xuongId) p
    LEFT JOIN (
        Select
            tp1.MaLo,
            tp1.MaLoaiNguyenLieu,
            tp1.MaSize,
            --tp1.MaMau,
            --CASE
            --    WHEN tp1.CaTra = 1
            --    and @isCaTraChuyenDoi = 1 THEN 'B'
            --    ELSE tp1.MaThanhPham
            --END as MaThanhPham,
			tp1.MaThanhPham as MaThanhPham,
            --tp1.CaTra,
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
                    p.MaLoaiNguyenLieu,
                    p.MaSize,
                    --p.MaMau,
                    p.MaThanhPham,
                    --p.CaTra,
                    --CASE
                    --    WHEN p.CaTra = 1 THEN 1.37
                    --    ELSE tp.DinhMuc
                    --END AS DinhMuc,
					tp.DinhMuc as DinhMuc,
                    p.Ngay,
                    p.MaXuong
                from
                    T_PhieuCan p,
                    T_ThanhPham tp
                where
                    p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and MaXuong = @xuongId
                    and p.MaThanhPham = tp.Ma
                    --and p.MaLoaiNguyenLieu = tp.MaCa
            ) tp1
            LEFT JOIN (
                Select
                    MaLo,
                    MaLoaiNguyenLieu,
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
                                MaLoaiNguyenLieu,
                                --MaMau,
                                MaSize,
                                MaThanhPham,
                                --CaTra,
                                Ngay
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            T_DinhMuc d
                        where
                            d.Ngay <= @toDate
                            and d.Ngay >= @fromDate
                            And MaXuong = @xuongId
                    ) dm
                Where
                    dm.[ROW NUMBER] = 1
            ) tp2 on tp1.MaLo = tp2.MaLo
            and tp1.MaLo = tp2.MaLoaiNguyenLieu
            and tp1.MaSize = tp2.MaSize
            --and tp1.MaMau = tp2.MaMau
            and tp1.MaThanhPham = tp2.MaThanhPham
            --and tp1.CaTra = tp2.CaTra
            and tp1.Ngay = tp2.Ngay
            and tp1.MaXuong = tp2.MaXuong
    ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
    and p.MaLo = DinhMuc.MaLo
    and p.MaLoaiNguyenLieu = DinhMuc.MaLoaiNguyenLieu
    and p.MaSize = DinhMuc.MaSize
    --and p.MaMau = DinhMuc.MaMau
    and p.MaThanhPham = DinhMuc.MaThanhPham
    --And p.CaTra = DinhMuc.CaTra
    and p.Ngay = DinhMuc.Ngay
    LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
    LEFT Join T_Size s on p.MaSize = s.Ma
    LEFT Join T_LoaiNguyenLieu la on p.MaLoaiNguyenLieu = la.Ma
    --LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
	LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
	LEFT JOIN T_LoNguyenLieu lnl on p.MaLo = lnl.Ma
	LEFT join T_NhaCungCap ncc on p.MaNhaCungCap = ncc.Ma
	LEFT join T_NhomLo nl on p.MaNhomLo = nl.Ma 
where
    p.STT > 0
    and p.Ngay <= @toDate
    and p.Ngay >= @fromDate
	and p.MaKhuVuc = kv.Ma and kv.Ma = @khuVuc
	--and p.MaNhaCungCap = ncc.Ma
	-- and ncc.Ma =lnl.MaNhaCungCap
	-- and p.MaNhomLo = nl.Ma
	-- and nl.Ma = lnl.MaNhomLo
group by
    p.MaLoaiNguyenLieu,
    p.MaSize,
    p.MaThanhPham,
    --p.MaMau,
    --p.CaTra,
    DinhMuc.DinhMuc,
    p.Ngay,
    --p.ChiSanLuong,
    la.Ten,
    tp.Ten,
    --ma.Ten,
    s.Ten,
	kv.Ten,
	p.MaLo,
	lnl.Ten,
	p.MaNhaCungCap,
	lnl.MaNhaCungCap,
	ncc.Ten,
	p.MaNhomLo,
	lnl.MaNhomLo,
	nl.Ten";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { fromDate, toDate, xuongId, khuVuc, isDinhMucBinhThuong, isFloor })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetTongHopHoaChats<T>(DateTime fromDate, DateTime toDate, string xuongId,
            string thanhPhamId = "HC")
        {
            try
            {
                var query = @"
IF OBJECT_ID('tempdb..#pWithRowNum') IS NOT NULL DROP TABLE #pWithRowNum;  
Select
    -- p.Ngay,
    -- p.MaSanPham,
    -- p.MaKhachHang,
    -- p.MaQuyTrinh,
    -- p.MaSize,
    -- p.MaThanhPham,
    -- p.MaCongDoan,
    -- p.TrongLuong,
    *,
    ROW_NUMBER() over (
        PARTITION BY p.Ngay,
        p.MaSanPham,
        p.MaKhachHang,
        p.MaQuyTrinh,
        p.MaSize,
        p.MaThanhPham,
        p.MaCongDoan
        ORDER BY
            p.Gio
    ) as rowNum into #pWithRowNum
from
    T_PhieuCan p
where
    p.Ngay >= @fromDate
    and p.Ngay <= @toDate
    and p.MaXuong = @xuongId
    and p.MaThanhPham = @thanhPhamId
Select
    p.*,
    sp.Ten as SanPhamName,
    kh.Ten as KhachHangName,
    qt.Ten as QuyTrinhName,
    s.Ten as SizeName,
    tp.Ten as ThanhPhamName,
    cd.Ten as CongDoanName,
    ttnl.Ten as TrangThaiNguyenLieuName,
    pg.Ten as PhuGiaName,
    ks.Ten as KhangSinhName
from
    (
        select
            p.*,
            pWRN.Gio,
            pWRN.T,
            pWRN.MaNhomHoaChat
        from
            (
                Select
                    p.Ngay,
                    p.MaSanPham,
                    p.MaKhachHang,
                    p.MaQuyTrinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia,
                    p.MaKhangSinh,
                    p.GhiChu,
                    Sum(p.TrongLuong) as TrongLuong
                from
                    #pWithRowNum p
                GROUP BY
                    p.Ngay,
                    p.MaSanPham,
                    p.MaKhachHang,
                    p.MaQuyTrinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia,
                    p.MaKhangSinh,
                    p.GhiChu
            ) p
            LEFT JOIN #pWithRowNum pWRN ON
            p.Ngay = pWRN.Ngay
            and p.MaCongDoan = pWRN.MaSanPham
            and p.MaKhachHang = pWRN.MaKhachHang
            and p.MaQuyTrinh = pWRN.MaQuyTrinh
            and p.MaSanPham = pWRN.MaSanPham
            and p.MaSize = pWRN.MaSize
            and p.MaThanhPham = pWRN.MaThanhPham
            and p.MaTrangThaiNguyenLieu = pWRN.MaTrangThaiNguyenLieu
            and p.MaPhuGia = pWRN.MaPhuGia
            and p.MaKhangSinh = pWRN.MaKhangSinh
            and p.GhiChu = pWRN.GhiChu
    ) p
    LEFT JOIN T_CongDoan cd on p.MaCongDoan = cd.Ma
    LEFT JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
    LEFT JOIN T_QuyTrinh qt on p.MaQuyTrinh = qt.Ma
    LEFT JOIN T_Size s on p.MaSize = s.Ma
    LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
    LEFT JOIN T_TrangThaiNguyenLieu ttnl on p.MaTrangThaiNguyenLieu = ttnl.Ma
    LEFT JOIN T_PhuGia pg on p.MaPhuGia = pg.Ma
    LEFT JOIN T_KhangSinh ks on p.MaKhangSinh = ks.Ma
    LEFT JOIN T_SanPham sp on p.MaSanPham = sp.Ma";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(
                        query,
                        new { fromDate, toDate, xuongId, thanhPhamId })
                    .ToList();
                return items;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<T> GetTongHopTheoLenhSanXuats<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var query = @"Select
    p.Ngay,
	p.MaLo,
	lnl.Ten as LoNguyenLieuName,
	p.MaNhaCungCap as MaNCC,
	lnl.MaNhaCungCap as MaNccLNL,
	ncc.Ten as NhaCungCapName,
	p.MaNhomLo as MaNL,
	lnl.MaNhomLo as MaNlLnl,
	nl.Ten as NhomLoName,
    p.MaLoaiNguyenLieu,
    la.Ten as LoaiNguyenLieuName,
    p.MaSize,
    s.Ten as SizeName,
    p.MaThanhPham,
    tp.Ten as ThanhPhamName,
    --p.MaMau,
    --ma.Ten as MauName,
    --p.CaTra,
    --p.ChiSanLuong,
    Sum(p.TrongLuongNhan) as TrongLuongNhan,
    Sum(p.TrongLuong) as TrongLuong,
    case
        when @isDinhMucBinhThuong = 1 then cast (
            case
                when sum(p.TrongLuong) = 0 then 0
                else (
                    case
                        when @isFloor = 1 then (
                            FLOOR(
                                (Sum(p.TrongLuongNhan) / sum(p.TrongLuong)) * 100
                            ) / 100
                        )
                        else Sum(p.TrongLuongNhan) / sum(p.TrongLuong)
                    end
                )
            end as decimal(18, 2)
        )
        ELSE cast (
            case
                when sum(p.TrongLuongNhan) = 0 then 0
                else sum(p.TrongLuong) / sum(p.TrongLuongNhan)
            end as decimal(18, 4)
        )
    end as DinhMuc,
    DinhMuc.DinhMuc as DinhMucChuan,
    Count(*) as SoRo,
	kv.Ten	as KhuVucName
	
from
    (Select * from
            T_PhieuCan p where p.Ngay <= @toDate
                            and p.Ngay >= @fromDate
                            and MaXuong = @xuongId) p
    LEFT JOIN (
        Select
            tp1.MaLo,
            tp1.MaLoaiNguyenLieu,
            tp1.MaSize,
            --tp1.MaMau,
            --CASE
            --    WHEN tp1.CaTra = 1
            --    and @isCaTraChuyenDoi = 1 THEN 'B'
            --    ELSE tp1.MaThanhPham
            --END as MaThanhPham,
			tp1.MaThanhPham as MaThanhPham,
            --tp1.CaTra,
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
                    p.MaLoaiNguyenLieu,
                    p.MaSize,
                    --p.MaMau,
                    p.MaThanhPham,
                    --p.CaTra,
                    --CASE
                    --    WHEN p.CaTra = 1 THEN 1.37
                    --    ELSE tp.DinhMuc
                    --END AS DinhMuc,
					tp.DinhMuc as DinhMuc,
                    p.Ngay,
                    p.MaXuong
                from
                    T_PhieuCan p,
                    T_ThanhPham tp
                where
                    p.Ngay <= @toDate
                    and p.Ngay >= @fromDate
                    and MaXuong = @xuongId
                    and p.MaThanhPham = tp.Ma
                    --and p.MaLoaiNguyenLieu = tp.MaCa
            ) tp1
            LEFT JOIN (
                Select
                    MaLo,
                    MaLoaiNguyenLieu,
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
                                MaLoaiNguyenLieu,
                                --MaMau,
                                MaSize,
                                MaThanhPham,
                                --CaTra,
                                Ngay
                                ORDER BY
                                    Gio DESC
                            ) AS [ROW NUMBER]
                        from
                            T_DinhMuc d
                        where
                            d.Ngay <= @toDate
                            and d.Ngay >= @fromDate
                            And MaXuong = @xuongId
                    ) dm
                Where
                    dm.[ROW NUMBER] = 1
            ) tp2 on tp1.MaLo = tp2.MaLo
            and tp1.MaLo = tp2.MaLoaiNguyenLieu
            and tp1.MaSize = tp2.MaSize
            --and tp1.MaMau = tp2.MaMau
            and tp1.MaThanhPham = tp2.MaThanhPham
            --and tp1.CaTra = tp2.CaTra
            and tp1.Ngay = tp2.Ngay
            and tp1.MaXuong = tp2.MaXuong
    ) DinhMuc on p.MaXuong = DinhMuc.MaXuong
    and p.MaLo = DinhMuc.MaLo
    and p.MaLoaiNguyenLieu = DinhMuc.MaLoaiNguyenLieu
    and p.MaSize = DinhMuc.MaSize
    --and p.MaMau = DinhMuc.MaMau
    and p.MaThanhPham = DinhMuc.MaThanhPham
    --And p.CaTra = DinhMuc.CaTra
    and p.Ngay = DinhMuc.Ngay
    LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
    LEFT Join T_Size s on p.MaSize = s.Ma
    LEFT Join T_LoaiNguyenLieu la on p.MaLoaiNguyenLieu = la.Ma
    --LEFT Join MaMauDinhHinh ma on p.MaMau = ma.Ma
    LEFT JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
	LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
	LEFT JOIN T_LoNguyenLieu lnl on p.MaLo = lnl.Ma
	LEFT join T_NhaCungCap ncc on p.MaNhaCungCap = ncc.Ma
	LEFT join T_NhomLo nl on p.MaNhomLo = nl.Ma 
where
    p.STT > 0
    and p.Ngay <= @toDate
    and p.Ngay >= @fromDate
	and p.MaKhuVuc = kv.Ma and kv.Ma = @khuVuc
	--and p.MaNhaCungCap = ncc.Ma
	-- and ncc.Ma =lnl.MaNhaCungCap
	-- and p.MaNhomLo = nl.Ma
	-- and nl.Ma = lnl.MaNhomLo
group by
    p.MaLoaiNguyenLieu,
    p.MaSize,
    p.MaThanhPham,
    --p.MaMau,
    --p.CaTra,
    DinhMuc.DinhMuc,
    p.Ngay,
    --p.ChiSanLuong,
    la.Ten,
    tp.Ten,
    --ma.Ten,
    s.Ten,
	kv.Ten,
	p.MaLo,
	lnl.Ten,
	p.MaNhaCungCap,
	lnl.MaNhaCungCap,
	ncc.Ten,
	p.MaNhomLo,
	lnl.MaNhomLo,
	nl.Ten";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.QueryAsync<T>(
                    query,
                    new { fromDate, toDate, xuongId, khuVuc, isDinhMucBinhThuong, isFloor })
                .Result
                .ToList();
            return items;
        }

        public List<T> GetChiTiets2<T>(
            DateTime dateTime,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true)
        {
            //                 var query = @"Select
//     p.*,
//     kh.Ten as KhachHangName
// from
//     (
//         Select
//             p.STT,
//             p.Ngay,
//             p.Gio,
//             p.MaNhanVien,
//             n.MaHoSo,
//             n.Name as TenNhanVien,
//             n.DeptName0 as Nhom,
//             nl.Ten as NhomLo,
//             nl.NgayNguyenLieu,
//             sp.Ten as SanPhamName,
//             qt.Ten as QuyTrinhName,
//             p.IsDatKhangSinh,
//             s.Ten as SizeName,
//             tp.Ma as MaNhomCongViec,
//             tp.Ten as NhomCongViecName,
//             cd.Ten as CongViecName,
//             p.MaLo,
//             p.TrongLuong,
//             p.VoXo,
//             p.TrongLuongTare,
//             p.MayCan,
//             p.MaXuong,
//             p.MaKhachHang,
//             ks.Ten as KhangSinhName,
//             ttp.Ten as ThongTinPhuName,
//             ttnl.Ten as ThongTinNguyenLieuName,
//             pg.Ten as PhuGiaName,
//             
// tp.IsPhanCo,
//             p.SoLuong,
//             p.TrongLuongDonVi,
//             p.IsCanTay
//         from
//             T_PhieuCan p,
//             NhanVienDaiThanh n,
//             T_ThanhPham tp,
//             T_Size s,
//             T_KhuVuc kv,
//             T_NhomLo nl,
//             T_SanPham sp,
//             T_CongDoan cd,
//             T_QuyTrinh qt,
//             T_KhangSinh ks,
//             T_ThongTinPhu ttp,
//             T_TrangThaiNguyenLieu ttnl,
//             T_PhuGia pg
//         where
//             p.Ngay = @ngay
//             and p.MaXuong = @xuongId
//             and p.MaNhanVien = n.MaNhanVien
//             and p.MaThanhPham = tp.Ma
//             and p.MaSize = s.Ma
//             and p.TrongLuong > 0
//             and p.MaKhuVuc = kv.Ma
//             and kv.ma = @khuVuc
//             and p.MaNhomLo = nl.Ma
//             and p.MaSanPham = sp.Ma
//             and p.MaCongDoan = cd.Ma
//             and p.MaQuyTrinh = qt.Ma
//             and p.MaKhangSinh = ks.Ma
//             and p.MaThongTinPhu = ttp.Ma
//             and p.MaTrangThaiNguyenLieu = ttnl.Ma
//             and p.MaPhuGia = pg.Ma
//     ) p
//     left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
// order by
//     STT DESC";
            var query = @"Select
    p.*,
    kh.Ten as KhachHangName,
    qt.Ten as QuyTrinhOrgName,
    khorg.Ten as KhachHangOrgName,
    size.Ten as SizeOrgName,
    pg.Ten as PhuGiaOrgName,
    pgr.Ten as PhuGiaName,
    nhcr.Ten as NhomHoaChatName,
    qcr.Ten as QuyCachName,
     ttp.Ten as ThongTinPhuName
from
    (
        Select
            p.STT,
            p.Ngay,
            p.Gio,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.Ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qt.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            tp.Ma as MaNhomCongViec,
            tp.Ten as NhomCongViecName,
            cd.Ten as CongViecName,
            p.MaLo,
            p.TrongLuong,
            p.VoXo,
p.VoXo2,
            p.TrongLuongTare,
            p.MayCan,
            p.MaXuong,
            p.MaKhachHang,
            ks.Ten as KhangSinhName,
            ttnl.Ten as ThongTinNguyenLieuName,
            -- pg.Ten as PhuGiaName,
            tp.IsPhanCo,
            -- nhc.Ten as NhomHoaChatName,
            p.SoLuong,
            p.TrongLuongDonVi,
            p.IsCanTay,
            p.T,
            --    qc.Ten as QuyCachName,
            p.MaQuyTrinhOrg,
            p.MaKhachHangOrg,
            p.MaSizeOrg,
            p.MaPhuGiaOrg,
            p.MaPhuGia,
            p.MaNhomHoaChat,
            p.MaQuyCach,
            p.MaThongTinPhu
        from
            T_PhieuCan p,
            NhanVienDaiThanh n,
            T_ThanhPham tp,
            T_Size s,
            T_KhuVuc kv,
            T_NhomLo nl,
            T_SanPham sp,
            T_CongDoan cd,
            T_QuyTrinh qt,
            T_KhangSinh ks,
            T_TrangThaiNguyenLieu ttnl
        where
            p.Ngay = @ngay
            and p.MaXuong = @xuongId
            and p.MaNhanVien = n.MaNhanVien
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.TrongLuong > 0
            and p.MaKhuVuc = kv.Ma
            and kv.ma = @khuVuc
            and p.MaNhomLo = nl.Ma
            and p.MaSanPham = sp.Ma
            and p.MaCongDoan = cd.Ma
            and p.MaQuyTrinh = qt.Ma
            and p.MaKhangSinh = ks.Ma
            --and p.MaThongTinPhu = ttp.Ma
            and p.MaTrangThaiNguyenLieu = ttnl.Ma -- and p.MaPhuGia = pg.Ma
            -- and p.MaNhomHoaChat = nhc.Ma
            -- and p.MaQuyCach = qc.Ma
    ) p
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
    LEFT JOIN T_QuyTrinh qt on p.MaQuyTrinhOrg = qt.Ma
    LEFT JOIN T_KhachHang khorg on p.MaKhachHang = khorg.Ma
    LEFT JOIN T_Size size on p.MaSizeOrg = size.Ma
    LEFT JOIN T_PhuGia pg on p.MaPhuGiaOrg = pg.Ma
    LEFT JOIN T_PhuGia pgr on p.MaPhuGia = pgr.Ma
    LEFT JOIN T_NhomHoaChat nhcr on p.MaNhomHoaChat = nhcr.Ma
    LEFT JOIN T_QuyCach qcr on p.MaQuyCach = qcr.Ma
    LEFT JOIN T_ThongTinPhu ttp on p.MaThongTinPhu = ttp.Ma
order by
    STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId, isDinhMucBinhThuong, khuVuc })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetChiTiets2<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true)
        {
            //                 var query = @"Select
            //     p.*,
            //     kh.Ten as KhachHangName
            // from
            //     (
            //         Select
            //             p.STT,
            //             p.Ngay,
            //             p.Gio,
            //             p.MaNhanVien,
            //             n.MaHoSo,
            //             n.Name as TenNhanVien,
            //             n.DeptName0 as Nhom,
            //             nl.Ten as NhomLo,
            //             nl.NgayNguyenLieu,
            //             sp.Ten as SanPhamName,
            //             qt.Ten as QuyTrinhName,
            //             p.IsDatKhangSinh,
            //             s.Ten as SizeName,
            //             tp.Ma as MaNhomCongViec,
            //             tp.Ten as NhomCongViecName,
            //             cd.Ten as CongViecName,
            //             p.MaLo,
            //             p.TrongLuong,
            //             p.VoXo,
            //             p.TrongLuongTare,
            //             p.MayCan,
            //             p.MaXuong,
            //             p.MaKhachHang,
            //             ks.Ten as KhangSinhName,
            //             ttp.Ten as ThongTinPhuName,
            //             ttnl.Ten as ThongTinNguyenLieuName,
            //             pg.Ten as PhuGiaName,
            //             
            // tp.IsPhanCo,
            //             p.SoLuong,
            //             p.TrongLuongDonVi,
            //             p.IsCanTay
            //         from
            //             T_PhieuCan p,
            //             NhanVienDaiThanh n,
            //             T_ThanhPham tp,
            //             T_Size s,
            //             T_KhuVuc kv,
            //             T_NhomLo nl,
            //             T_SanPham sp,
            //             T_CongDoan cd,
            //             T_QuyTrinh qt,
            //             T_KhangSinh ks,
            //             T_ThongTinPhu ttp,
            //             T_TrangThaiNguyenLieu ttnl,
            //             T_PhuGia pg
            //         where
            //             p.Ngay = @ngay
            //             and p.MaXuong = @xuongId
            //             and p.MaNhanVien = n.MaNhanVien
            //             and p.MaThanhPham = tp.Ma
            //             and p.MaSize = s.Ma
            //             and p.TrongLuong > 0
            //             and p.MaKhuVuc = kv.Ma
            //             and kv.ma = @khuVuc
            //             and p.MaNhomLo = nl.Ma
            //             and p.MaSanPham = sp.Ma
            //             and p.MaCongDoan = cd.Ma
            //             and p.MaQuyTrinh = qt.Ma
            //             and p.MaKhangSinh = ks.Ma
            //             and p.MaThongTinPhu = ttp.Ma
            //             and p.MaTrangThaiNguyenLieu = ttnl.Ma
            //             and p.MaPhuGia = pg.Ma
            //     ) p
            //     left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
            // order by
            //     STT DESC";
//                var query = @"Select
//    p.STT,
//    p.Ngay,
//    p.Gio,
//    p.MaNhanVien,
//    n.MaHoSo,
//    n.Name as TenNhanVien,
//    n.DeptName0 as Nhom,
//    nl.ten as NhomLo,
//    nl.NgayNguyenLieu,
//    sp.Ten as SanPhamName,
//    qtc.Ten as QuyTrinhName,
//    p.IsDatKhangSinh,
//    s.Ten as SizeName,
//    sTP.Ten as SizeTPName,
//    tp.ma as MaNhomCongViec,
//    tp.Ten as NhomCongViecName,
//    cd.Ten as CongViecName,
//    p.MaLo,
//    p.TrongLuong,
//    p.VoXo,
//    p.TrongLuongTare,
//    p.MayCan,
//    p.MaXuong,
//    p.MaKhachHang,
//    ks.Ten as KhangSinhName,
//    ttnl.Ten as ThongTinNguyenLieuName,
//    tp.IsPhanCo,
//    p.SoLuong,
//    p.TrongLuongDonVi,
//    p.IsCanTay,
//    p.T,
//    p.MaQuyTrinhOrg,
//    p.MaKhachHangOrg,
//    p.MaSizeOrg,
//    p.MaPhuGiaOrg,
//    p.MaPhuGia,
//    p.MaNhomHoaChat,
//    p.MaQuyCach,
//    p.MaThongTinPhu,
//    p.GhiChu,
//    kh.Ten as KhachHangName,
//    qt.Ten as QuyTrinhOrgName,
//    khorg.Ten as KhachHangOrgName,
//    size.Ten as SizeOrgName,
//    pg.Ten as PhuGiaOrgName,
//    pgr.Ten as PhuGiaName,
//    nhcr.Ten as NhomHoaChatName,
//    qcr.Ten as QuyCachName,
//    ttp.Ten as ThongTinPhuName
//from
//    (
//        Select
//            *
//        from
//            T_PhieuCan p
//        where
//            p.Ngay <= @ngay
//            and p.Ngay >= @fromDate --p.Ngay = @ngay
//            and p.MaXuong = @xuongId
//    ) p
//    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
//    LEFT JOIN T_QuyTrinh qt on p.MaQuyTrinhOrg = qt.Ma
//    LEFT JOIN T_KhachHang khorg on p.MaKhachHang = khorg.Ma
//    LEFT JOIN T_Size size on p.MaSizeOrg = size.Ma
//    LEFT JOIN T_PhuGia pg on p.MaPhuGiaOrg = pg.Ma
//    LEFT JOIN T_PhuGia pgr on p.MaPhuGia = pgr.Ma
//    LEFT JOIN T_NhomHoaChat nhcr on p.MaNhomHoaChat = nhcr.Ma
//    LEFT JOIN T_QuyCach qcr on p.MaQuyCach = qcr.Ma
//    LEFT JOIN T_ThongTinPhu ttp on p.MaThongTinPhu = ttp.Ma
//    left JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
//    LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
//    LEFT JOIN T_Size s on p.MaSize = s.Ma
//    LEFT JOIN T_Size sTP on p.MaSizeTP = sTP.Ma
//    LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
//    LEFT JOIN T_NhomLo nl on p.MaNhomLo = nl.Ma
//    left JOIN T_SanPham sp on p.MaSanPham = sp.Ma
//    LEFT JOIN T_CongDoan cd on p.MaCongDoan = cd.Ma
//    LEFT JOIN T_QuyTrinh qtc on p.MaQuyTrinh = qtc.Ma
//    LEFT JOIN T_KhangSinh ks on p.MaKhangSinh = ks.Ma
//    left JOIN T_TrangThaiNguyenLieu ttnl on p.MaTrangThaiNguyenLieu = ttnl.Ma

//order by
//    STT DESC";
            var query = @"Select p.STT,
    p.Ngay,
    p.Gio,
    p.MaNhanVien,
    p.MaHoSo,
    p.TenNhanVien,
    p.Nhom,
    p.NhomLo,
    p.NgayNguyenLieu,
    p.SanPhamName,
    p.QuyTrinhName,
    p.IsDatKhangSinh,
    p.SizeName,
    p.SizeTPName,
    p.MaNhomCongViec,
    p.NhomCongViecName,
    (
        case
            WHEN isnull(ctvx.MaSizeVoXo, '') = '' then p.CongViecName
            else concat(p.CongViecName, ' ', ctvx.MaSizeVoXo)
        end
    ) CongViecName,
    p.MaLo,
    p.TrongLuong,
    p.VoXo,
p.VoXo2,
    p.TrongLuongTare,
    p.MayCan,
    p.MaXuong,
    p.MaKhachHang,
    p.KhangSinhName,
    p.ThongTinNguyenLieuName,
    p.IsPhanCo,
    p.SoLuong,
    p.TrongLuongDonVi,
    p.IsCanTay,
    p.T,
    p.MaQuyTrinhOrg,
    p.MaKhachHangOrg,
    p.MaSizeOrg,
    p.MaPhuGiaOrg,
    p.MaPhuGia,
    p.MaNhomHoaChat,
    p.MaQuyCach,
    p.MaThongTinPhu,
    p.GhiChu,
    p.KhachHangName,
    p.QuyTrinhOrgName,
    p.KhachHangOrgName,
    p.SizeOrgName,
    p.PhuGiaOrgName,
    p.PhuGiaName,
    p.NhomHoaChatName,
    p.QuyCachName,
    p.ThongTinPhuName,
    p.LoaiPhieuPhanCo,
    p.LoaiQuyTrinh,
    p.MaSize,
    p.MaCongDoan
from (
        Select p.STT,
            p.Ngay,
            p.Gio,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qtc.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            sTP.Ten as SizeTPName,
            tp.ma as MaNhomCongViec,
            tp.Ten as NhomCongViecName,
            cd.Ten as CongViecName,
            p.MaLo,
            p.TrongLuong,
            p.VoXo,
p.VoXo2,
            p.TrongLuongTare,
            p.MayCan,
            p.MaXuong,
            p.MaKhachHang,
            ks.Ten as KhangSinhName,
            ttnl.Ten as ThongTinNguyenLieuName,
            tp.IsPhanCo,
            p.SoLuong,
            p.TrongLuongDonVi,
            p.IsCanTay,
            p.T,
            p.MaQuyTrinhOrg,
            p.MaKhachHangOrg,
            p.MaSizeOrg,
            p.MaPhuGiaOrg,
            p.MaPhuGia,
            p.MaNhomHoaChat,
            p.MaQuyCach,
            p.MaThongTinPhu,
            p.GhiChu,
            kh.Ten as KhachHangName,
            qt.Ten as QuyTrinhOrgName,
            khorg.Ten as KhachHangOrgName,
            size.Ten as SizeOrgName,
            pg.Ten as PhuGiaOrgName,
            pgr.Ten as PhuGiaName,
            nhcr.Ten as NhomHoaChatName,
            qcr.Ten as QuyCachName,
            ttp.Ten as ThongTinPhuName,
            ppc.LoaiPhieuPhanCo,
            qtc.LoaiQuyTrinh,
            p.MaSize,
            p.MaCongDoan
        from (
                Select *
                from T_PhieuCan p
                where p.Ngay <= @ngay
                    and p.Ngay >= @fromDate --p.Ngay = @ngay
                    and p.MaXuong = @xuongId
            ) p
            left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
            LEFT JOIN T_QuyTrinh qt on p.MaQuyTrinhOrg = qt.Ma
            LEFT JOIN T_KhachHang khorg on p.MaKhachHang = khorg.Ma
            LEFT JOIN T_Size size on p.MaSizeOrg = size.Ma
            LEFT JOIN T_PhuGia pg on p.MaPhuGiaOrg = pg.Ma
            LEFT JOIN T_PhuGia pgr on p.MaPhuGia = pgr.Ma
            LEFT JOIN T_NhomHoaChat nhcr on p.MaNhomHoaChat = nhcr.Ma
            LEFT JOIN T_QuyCach qcr on p.MaQuyCach = qcr.Ma
            LEFT JOIN T_ThongTinPhu ttp on p.MaThongTinPhu = ttp.Ma
            left JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
            LEFT JOIN T_Size s on p.MaSize = s.Ma
            LEFT JOIN T_Size sTP on p.MaSizeTP = sTP.Ma
            LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
            LEFT JOIN T_NhomLo nl on p.MaNhomLo = nl.Ma
            left JOIN T_SanPham sp on p.MaSanPham = sp.Ma
            LEFT JOIN T_CongDoan cd on p.MaCongDoan = cd.Ma
            LEFT JOIN T_QuyTrinh qtc on p.MaQuyTrinh = qtc.Ma
            LEFT JOIN T_KhangSinh ks on p.MaKhangSinh = ks.Ma
            left JOIN T_TrangThaiNguyenLieu ttnl on p.MaTrangThaiNguyenLieu = ttnl.Ma
            LEFT JOIN T_PhieuPhanCo ppc on p.MaPhieuPhanCo = ppc.Ma
    ) p
    LEFT JOIN T_ChiTietVoXo ctvx ON p.LoaiPhieuPhanCo = ctvx.MaPhieuPhanCo
    and p.VoXo = ctvx.VoXo
    and p.VoXo2 = ctvx.VoXo2
    and p.LoaiQuyTrinh = ctvx.MaQuyTrinh
    and p.MaSize = ctvx.MaSize
    and p.MaCongDoan = ctvx.MaCongDoan
order by STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { fromDate = fromDate.Date, ngay = dateTime.Date, xuongId, isDinhMucBinhThuong, khuVuc })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetChiTiets2<T>(
            DateTime dateTime,
            string xuongId,
            string khuVuc,
            string thanhPhamId,
            bool isDinhMucBinhThuong = true)
        {
            /*var query = @"Select
    p.*,
    kh.Ten as KhachHangName
from
    (
        Select
            p.STT,
            p.Gio,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.Ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qt.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            tp.Ten as NhomCongViecName,
            cd.Ten as CongViecName,
            p.MaLo,
            p.TrongLuong,
            p.VoXo,
            p.TrongLuongTare,
            p.MayCan,
            p.MaXuong,
            p.MaKhachHang,
            ks.Ten as KhangSinhName,
            ttp.Ten as ThongTinPhuName,
            ttnl.Ten as TrangThaiNguyenLieuName,
            pg.Ten as PhuGiaName
        from
            T_PhieuCan p,
            NhanVienDaiThanh n,
            T_ThanhPham tp,
            T_Size s,
            T_KhuVuc kv,
            T_NhomLo nl,
            T_SanPham sp,
            T_CongDoan cd,
            T_QuyTrinh qt,
            T_KhangSinh ks,
           T_ThongTinPhu ttp,
           T_TrangThaiNguyenLieu ttnl,
            T_PhuGia pg
        where
            p.Ngay = @ngay
            and p.MaXuong = @xuongId
            and p.MaThanhPham = @thanhPhamId
            and p.MaNhanVien = n.MaNhanVien
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.TrongLuong > 0
            and p.MaKhuVuc = kv.Ma
            and kv.ma = @khuVuc
            and p.MaNhomLo = nl.Ma
            and p.MaSanPham = sp.Ma
            and p.MaCongDoan = cd.Ma
            and p.MaQuyTrinh = qt.Ma
            and p.MaKhangSinh = ks.Ma
            and p.MaThongTinPhu = ttp.Ma
            and p.MaTrangThaiNguyenLieu = ttnl.Ma
            and p.MaPhuGia = pg.Ma
    ) p
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
order by
    STT DESC";*/
            var query = @"Select
    p.*,
    kh.Ten as KhachHangName,
    qt.Ten as QuyTrinhOrgName,
    khorg.Ten as KhachHangOrgName,
    size.Ten as SizeOrgName,
    pg.Ten as PhuGiaOrgName,
    pgr.Ten as PhuGiaName,
    nhcr.Ten as NhomHoaChatName,
    qcr.Ten as QuyCachName,
     ttp.Ten as ThongTinPhuName
from
    (
        Select
            p.STT,
            p.Ngay,
            p.Gio,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.Ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qt.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            tp.Ma as MaNhomCongViec,
            tp.Ten as NhomCongViecName,
            cd.Ten as CongViecName,
            p.MaLo,
            p.TrongLuong,
            p.VoXo,
p.VoXo2,
            p.TrongLuongTare,
            p.MayCan,
            p.MaXuong,
            p.MaKhachHang,
            ks.Ten as KhangSinhName,
            ttnl.Ten as ThongTinNguyenLieuName,
            -- pg.Ten as PhuGiaName,
            tp.IsPhanCo,
            -- nhc.Ten as NhomHoaChatName,
            p.SoLuong,
            p.TrongLuongDonVi,
            p.IsCanTay,
            p.T,
            --    qc.Ten as QuyCachName,
            p.MaQuyTrinhOrg,
            p.MaKhachHangOrg,
            p.MaSizeOrg,
            p.MaPhuGiaOrg,
            p.MaPhuGia,
            p.MaNhomHoaChat,
            p.MaQuyCach,
            p.MaThongTinPhu
        from
            T_PhieuCan p,
            NhanVienDaiThanh n,
            T_ThanhPham tp,
            T_Size s,
            T_KhuVuc kv,
            T_NhomLo nl,
            T_SanPham sp,
            T_CongDoan cd,
            T_QuyTrinh qt,
            T_KhangSinh ks,
            T_TrangThaiNguyenLieu ttnl
        where
            p.Ngay = @ngay
            and p.MaXuong = @xuongId
            and p.MaThanhPham = @thanhPhamId
            and p.MaNhanVien = n.MaNhanVien
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.TrongLuong > 0
            and p.MaKhuVuc = kv.Ma
            and kv.ma = @khuVuc
            and p.MaNhomLo = nl.Ma
            and p.MaSanPham = sp.Ma
            and p.MaCongDoan = cd.Ma
            and p.MaQuyTrinh = qt.Ma
            and p.MaKhangSinh = ks.Ma
            --and p.MaThongTinPhu = ttp.Ma
            and p.MaTrangThaiNguyenLieu = ttnl.Ma -- and p.MaPhuGia = pg.Ma
            -- and p.MaNhomHoaChat = nhc.Ma
            -- and p.MaQuyCach = qc.Ma
    ) p
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
    LEFT JOIN T_QuyTrinh qt on p.MaQuyTrinhOrg = qt.Ma
    LEFT JOIN T_KhachHang khorg on p.MaKhachHang = khorg.Ma
    LEFT JOIN T_Size size on p.MaSizeOrg = size.Ma
    LEFT JOIN T_PhuGia pg on p.MaPhuGiaOrg = pg.Ma
    LEFT JOIN T_PhuGia pgr on p.MaPhuGia = pgr.Ma
    LEFT JOIN T_NhomHoaChat nhcr on p.MaNhomHoaChat = nhcr.Ma
    LEFT JOIN T_QuyCach qcr on p.MaQuyCach = qcr.Ma
    LEFT JOIN T_ThongTinPhu ttp on p.MaThongTinPhu = ttp.Ma
order by
    STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId, isDinhMucBinhThuong, khuVuc, thanhPhamId })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetChiTiets2<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId,
            string khuVuc,
            string thanhPhamId,
            bool isDinhMucBinhThuong = true)
        {
            /*var query = @"Select
    p.*,
    kh.Ten as KhachHangName
from
    (
        Select
            p.STT,
            p.Gio,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.Ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qt.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            tp.Ten as NhomCongViecName,
            cd.Ten as CongViecName,
            p.MaLo,
            p.TrongLuong,
            p.VoXo,
            p.TrongLuongTare,
            p.MayCan,
            p.MaXuong,
            p.MaKhachHang,
            ks.Ten as KhangSinhName,
            ttp.Ten as ThongTinPhuName,
            ttnl.Ten as TrangThaiNguyenLieuName,
            pg.Ten as PhuGiaName
        from
            T_PhieuCan p,
            NhanVienDaiThanh n,
            T_ThanhPham tp,
            T_Size s,
            T_KhuVuc kv,
            T_NhomLo nl,
            T_SanPham sp,
            T_CongDoan cd,
            T_QuyTrinh qt,
            T_KhangSinh ks,
           T_ThongTinPhu ttp,
           T_TrangThaiNguyenLieu ttnl,
            T_PhuGia pg
        where
            p.Ngay = @ngay
            and p.MaXuong = @xuongId
            and p.MaThanhPham = @thanhPhamId
            and p.MaNhanVien = n.MaNhanVien
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
            and p.TrongLuong > 0
            and p.MaKhuVuc = kv.Ma
            and kv.ma = @khuVuc
            and p.MaNhomLo = nl.Ma
            and p.MaSanPham = sp.Ma
            and p.MaCongDoan = cd.Ma
            and p.MaQuyTrinh = qt.Ma
            and p.MaKhangSinh = ks.Ma
            and p.MaThongTinPhu = ttp.Ma
            and p.MaTrangThaiNguyenLieu = ttnl.Ma
            and p.MaPhuGia = pg.Ma
    ) p
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
order by
    STT DESC";*/
            var query = @"Select p.STT,
    p.Ngay,
    p.Gio,
    p.MaNhanVien,
    p.MaHoSo,
    p.TenNhanVien,
    p.Nhom,
    p.NhomLo,
    p.NgayNguyenLieu,
    p.SanPhamName,
    p.QuyTrinhName,
    p.IsDatKhangSinh,
    p.SizeName,
    p.SizeTPName,
    p.MaNhomCongViec,
    p.NhomCongViecName,
    (
        case
            WHEN isnull(ctvx.MaSizeVoXo, '') = '' then p.CongViecName
            else concat(p.CongViecName, ' ', ctvx.MaSizeVoXo)
        end
    ) CongViecName,
    p.MaLo,
    p.TrongLuong,
    p.VoXo,
p.VoXo2,
    p.TrongLuongTare,
    p.MayCan,
    p.MaXuong,
    p.MaKhachHang,
    p.KhangSinhName,
    p.ThongTinNguyenLieuName,
    p.IsPhanCo,
    p.SoLuong,
    p.TrongLuongDonVi,
    p.IsCanTay,
    p.T,
    p.MaQuyTrinhOrg,
    p.MaKhachHangOrg,
    p.MaSizeOrg,
    p.MaPhuGiaOrg,
    p.MaPhuGia,
    p.MaNhomHoaChat,
    p.MaQuyCach,
    p.MaThongTinPhu,
    p.GhiChu,
    p.KhachHangName,
    p.QuyTrinhOrgName,
    p.KhachHangOrgName,
    p.SizeOrgName,
    p.PhuGiaOrgName,
    p.PhuGiaName,
    p.NhomHoaChatName,
    p.QuyCachName,
    p.ThongTinPhuName,
    p.LoaiPhieuPhanCo,
    p.LoaiQuyTrinh,
    p.MaSize,
    p.MaCongDoan
from (
        Select p.STT,
            p.Ngay,
            p.Gio,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qtc.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            sTP.Ten as SizeTPName,
            tp.ma as MaNhomCongViec,
            tp.Ten as NhomCongViecName,
            cd.Ten as CongViecName,
            p.MaLo,
            p.TrongLuong,
            p.VoXo,
p.VoXo2,
            p.TrongLuongTare,
            p.MayCan,
            p.MaXuong,
            p.MaKhachHang,
            ks.Ten as KhangSinhName,
            ttnl.Ten as ThongTinNguyenLieuName,
            tp.IsPhanCo,
            p.SoLuong,
            p.TrongLuongDonVi,
            p.IsCanTay,
            p.T,
            p.MaQuyTrinhOrg,
            p.MaKhachHangOrg,
            p.MaSizeOrg,
            p.MaPhuGiaOrg,
            p.MaPhuGia,
            p.MaNhomHoaChat,
            p.MaQuyCach,
            p.MaThongTinPhu,
            p.GhiChu,
            kh.Ten as KhachHangName,
            qt.Ten as QuyTrinhOrgName,
            khorg.Ten as KhachHangOrgName,
            size.Ten as SizeOrgName,
            pg.Ten as PhuGiaOrgName,
            pgr.Ten as PhuGiaName,
            nhcr.Ten as NhomHoaChatName,
            qcr.Ten as QuyCachName,
            ttp.Ten as ThongTinPhuName,
            ppc.LoaiPhieuPhanCo,
            qtc.LoaiQuyTrinh,
            p.MaSize,
            p.MaCongDoan
        from (
                Select *
                from T_PhieuCan p
                where p.Ngay <= @ngay
                    and p.Ngay >= @fromDate --p.Ngay = @ngay
                    and p.MaXuong = @xuongId
                    and p.MaThanhPham = @thanhPhamId
            ) p
            left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
            LEFT JOIN T_QuyTrinh qt on p.MaQuyTrinhOrg = qt.Ma
            LEFT JOIN T_KhachHang khorg on p.MaKhachHang = khorg.Ma
            LEFT JOIN T_Size size on p.MaSizeOrg = size.Ma
            LEFT JOIN T_PhuGia pg on p.MaPhuGiaOrg = pg.Ma
            LEFT JOIN T_PhuGia pgr on p.MaPhuGia = pgr.Ma
            LEFT JOIN T_NhomHoaChat nhcr on p.MaNhomHoaChat = nhcr.Ma
            LEFT JOIN T_QuyCach qcr on p.MaQuyCach = qcr.Ma
            LEFT JOIN T_ThongTinPhu ttp on p.MaThongTinPhu = ttp.Ma
            left JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
            LEFT JOIN T_Size s on p.MaSize = s.Ma
            LEFT JOIN T_Size sTP on p.MaSizeTP = sTP.Ma
            LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
            LEFT JOIN T_NhomLo nl on p.MaNhomLo = nl.Ma
            left JOIN T_SanPham sp on p.MaSanPham = sp.Ma
            LEFT JOIN T_CongDoan cd on p.MaCongDoan = cd.Ma
            LEFT JOIN T_QuyTrinh qtc on p.MaQuyTrinh = qtc.Ma
            LEFT JOIN T_KhangSinh ks on p.MaKhangSinh = ks.Ma
            left JOIN T_TrangThaiNguyenLieu ttnl on p.MaTrangThaiNguyenLieu = ttnl.Ma
            LEFT JOIN T_PhieuPhanCo ppc on p.MaPhieuPhanCo = ppc.Ma
    ) p
    LEFT JOIN T_ChiTietVoXo ctvx ON p.LoaiPhieuPhanCo = ctvx.MaPhieuPhanCo
    and p.VoXo = ctvx.VoXo
and p.VoXo2 = ctvx.VoXo2
    and p.LoaiQuyTrinh = ctvx.MaQuyTrinh
    and p.MaSize = ctvx.MaSize
    and p.MaCongDoan = ctvx.MaCongDoan
order by STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new
                        {
                            fromDate = fromDate.Date, ngay = dateTime.Date, xuongId, isDinhMucBinhThuong, khuVuc,
                            thanhPhamId
                        })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetChiTietsDateTimeToDateTime<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId,
            string khuVuc,
            string thanhPhamId,
            bool isDinhMucBinhThuong = true)
        {
            
            var query = @"Select p.STT,
    --them NgayGio
    p.Ngay,
    p.Gio,
    p.MaNhanVien,
    p.MaHoSo,
    p.TenNhanVien,
    p.Nhom,
    p.NhomLo,
    p.NgayNguyenLieu,
    p.SanPhamName,
    p.QuyTrinhName,
    p.IsDatKhangSinh,
    p.SizeName,
    case
        when p.IsPhanCo != 1 then (
            case
                when LEN(p.GhiChu) - LEN(REPLACE(p.GhiChu, '-', '')) >= 2 then LEFT(
                    Right(
                        p.GhiChu,
                        LEN(p.GhiChu) - CHARINDEX('-', p.GhiChu)
                    ),
                    CHARINDEX(
                        '-',
                        Right(
                            p.GhiChu,
                            LEN(p.GhiChu) - CHARINDEX('-', p.GhiChu)
                        )
                    ) -1
                )
                else ''
            end
        )
        else p.SizeTPName
    END as SizeTPName,
    p.MaNhomCongViec,
    p.NhomCongViecName,
    (
        case
            WHEN isnull(ctvx.MaSizeVoXo, '') = '' then p.CongViecName
            else concat(p.CongViecName, ' ', ctvx.MaSizeVoXo)
        end
    ) CongViecName,
    p.MaLo,
    p.TrongLuong,
    p.VoXo,
    case
        when p.IsPhanCo = 1 then CAST(cast(p.VoXo as decimal(18, 1)) as varchar(20))
        when p.IsHoaChat = 1 then (
            case
                when LEN(p.GhiChu) - LEN(REPLACE(p.GhiChu, ',', '')) >= 2 then LEFT(
                    Right(
                        p.GhiChu,
                        LEN(p.GhiChu) - CHARINDEX(',', p.GhiChu)
                    ),
                    CHARINDEX(
                        ',',
                        Right(
                            p.GhiChu,
                            LEN(p.GhiChu) - CHARINDEX(',', p.GhiChu)
                        )
                    ) -1
                )
                else ''
            end
        )
        else p.VoXo2
    END as VoXo2,
    p.TrongLuongTare,
    p.MayCan,
    p.MaXuong,
    p.MaKhachHang,
    p.KhangSinhName,
    p.ThongTinNguyenLieuName,
    p.IsPhanCo,
    p.SoLuong,
    p.TrongLuongDonVi,
    p.IsCanTay,
    p.T,
    p.MaQuyTrinhOrg,
    p.MaKhachHangOrg,
    p.MaSizeOrg,
    p.MaPhuGiaOrg,
    p.MaPhuGia,
    p.MaNhomHoaChat,
    p.MaQuyCach,
    p.MaThongTinPhu,
    p.GhiChu,
    p.KhachHangName,
    p.QuyTrinhOrgName,
    p.KhachHangOrgName,
    p.SizeOrgName,
    p.PhuGiaOrgName,
    p.PhuGiaName,
    p.NhomHoaChatName,
    p.QuyCachName,
    p.ThongTinPhuName,
    p.LoaiPhieuPhanCo,
    p.LoaiQuyTrinh,
    p.MaSize,
    p.MaCongDoan,
    p.GhiChu2,
    p.GhiChu3
from (
        Select p.STT,
            --Them NgayGio
            p.Ngay,
            p.Gio,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qtc.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            sTP.Ten as SizeTPName,
            tp.ma as MaNhomCongViec,
            tp.Ten as NhomCongViecName,
            cd.Ten as CongViecName,
            p.MaLo,
            p.TrongLuong,
            p.VoXo,
            p.VoXo2,
            p.TrongLuongTare,
            p.MayCan,
            p.MaXuong,
            p.MaKhachHang,
            ks.Ten as KhangSinhName,
            ttnl.Ten as ThongTinNguyenLieuName,
            tp.IsPhanCo,
            tp.IsHoaChat,
            p.SoLuong,
            p.TrongLuongDonVi,
            p.IsCanTay,
            p.T,
            p.MaQuyTrinhOrg,
            p.MaKhachHangOrg,
            p.MaSizeOrg,
            p.MaPhuGiaOrg,
            p.MaPhuGia,
            p.MaNhomHoaChat,
            p.MaQuyCach,
            p.MaThongTinPhu,
            p.GhiChu,
            kh.Ten as KhachHangName,
            qt.Ten as QuyTrinhOrgName,
            khorg.Ten as KhachHangOrgName,
            size.Ten as SizeOrgName,
            pg.Ten as PhuGiaOrgName,
            pgr.Ten as PhuGiaName,
            nhcr.Ten as NhomHoaChatName,
            qcr.Ten as QuyCachName,
            ttp.Ten as ThongTinPhuName,
            ppc.LoaiPhieuPhanCo,
            qtc.LoaiQuyTrinh,
            p.MaSize,
            p.MaCongDoan,
            p.GhiChu2,
            p.GhiChu3
        from (
                -- xu ly ngay gio
                SELECT *
                from T_PhieuCan p1
                where CAST(p1.Ngay as datetime) + CAST(p1.Gio as datetime) <= @ngay
                    and CAST(p1.Ngay as datetime) + CAST(p1.Gio as datetime) >= @fromDate --p.Ngay = @ngay
                    and p1.MaXuong = @xuongId and p1.MaThanhPham = @thanhPhamId
            ) p
            left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
            LEFT JOIN T_QuyTrinh qt on p.MaQuyTrinhOrg = qt.Ma
            LEFT JOIN T_KhachHang khorg on p.MaKhachHang = khorg.Ma
            LEFT JOIN T_Size size on p.MaSizeOrg = size.Ma
            LEFT JOIN T_PhuGia pg on p.MaPhuGiaOrg = pg.Ma
            LEFT JOIN T_PhuGia pgr on p.MaPhuGia = pgr.Ma
            LEFT JOIN T_NhomHoaChat nhcr on p.MaNhomHoaChat = nhcr.Ma
            LEFT JOIN T_QuyCach qcr on p.MaQuyCach = qcr.Ma
            LEFT JOIN T_ThongTinPhu ttp on p.MaThongTinPhu = ttp.Ma
            left JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
            LEFT JOIN T_Size s on p.MaSize = s.Ma
            LEFT JOIN T_Size sTP on p.MaSizeTP = sTP.Ma
            LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
            LEFT JOIN T_NhomLo nl on p.MaNhomLo = nl.Ma
            left JOIN T_SanPham sp on p.MaSanPham = sp.Ma
            LEFT JOIN T_CongDoan cd on p.MaCongDoan = cd.Ma
            LEFT JOIN T_QuyTrinh qtc on p.MaQuyTrinh = qtc.Ma
            LEFT JOIN T_KhangSinh ks on p.MaKhangSinh = ks.Ma
            left JOIN T_TrangThaiNguyenLieu ttnl on p.MaTrangThaiNguyenLieu = ttnl.Ma
            LEFT JOIN T_PhieuPhanCo ppc on p.MaPhieuPhanCo = ppc.Ma
    ) p
    LEFT JOIN T_ChiTietVoXo ctvx ON p.LoaiPhieuPhanCo = ctvx.MaPhieuPhanCo
    and p.VoXo = ctvx.VoXo
    and p.VoXo2 = ctvx.VoXo2
    and p.LoaiQuyTrinh = ctvx.MaQuyTrinh
    and p.MaSize = ctvx.MaSize
    and p.MaCongDoan = ctvx.MaCongDoan
order by STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new
                        {
                            fromDate = fromDate,
                            ngay = dateTime,
                            xuongId,
                            isDinhMucBinhThuong,
                            khuVuc,
                            thanhPhamId
                        })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetChiTietsDateTimeToDateTime<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true)
        {
            
            var query = @"Select p.STT,
    --them NgayGio
    p.Ngay,
    p.Gio,
    p.MaNhanVien,
    p.MaHoSo,
    p.TenNhanVien,
    p.Nhom,
    p.NhomLo,
    p.NgayNguyenLieu,
    p.SanPhamName,
    p.QuyTrinhName,
    p.IsDatKhangSinh,
    p.SizeName,
    case
        when p.IsPhanCo != 1 then (
            case
                when LEN(p.GhiChu) - LEN(REPLACE(p.GhiChu, '-', '')) >= 2 then LEFT(
                    Right(
                        p.GhiChu,
                        LEN(p.GhiChu) - CHARINDEX('-', p.GhiChu)
                    ),
                    CHARINDEX(
                        '-',
                        Right(
                            p.GhiChu,
                            LEN(p.GhiChu) - CHARINDEX('-', p.GhiChu)
                        )
                    ) -1
                )
                else ''
            end
        )
        else p.SizeTPName
    END as SizeTPName,
    p.MaNhomCongViec,
    p.NhomCongViecName,
    (
        case
            WHEN isnull(ctvx.MaSizeVoXo, '') = '' then p.CongViecName
            else concat(p.CongViecName, ' ', ctvx.MaSizeVoXo)
        end
    ) CongViecName,
    p.MaLo,
    p.TrongLuong,
    p.VoXo,
    case
        when p.IsPhanCo = 1 then CAST(cast(p.VoXo as decimal(18, 1)) as varchar(20))
        when p.IsHoaChat = 1 then (
            case
                when LEN(p.GhiChu) - LEN(REPLACE(p.GhiChu, ',', '')) >= 2 then LEFT(
                    Right(
                        p.GhiChu,
                        LEN(p.GhiChu) - CHARINDEX(',', p.GhiChu)
                    ),
                    CHARINDEX(
                        ',',
                        Right(
                            p.GhiChu,
                            LEN(p.GhiChu) - CHARINDEX(',', p.GhiChu)
                        )
                    ) -1
                )
                else ''
            end
        )
        else p.VoXo2
    END as VoXo2,
    p.TrongLuongTare,
    p.MayCan,
    p.MaXuong,
    p.MaKhachHang,
    p.KhangSinhName,
    p.ThongTinNguyenLieuName,
    p.IsPhanCo,
    p.SoLuong,
    p.TrongLuongDonVi,
    p.IsCanTay,
    p.T,
    p.MaQuyTrinhOrg,
    p.MaKhachHangOrg,
    p.MaSizeOrg,
    p.MaPhuGiaOrg,
    p.MaPhuGia,
    p.MaNhomHoaChat,
    p.MaQuyCach,
    p.MaThongTinPhu,
    p.GhiChu,
    p.KhachHangName,
    p.QuyTrinhOrgName,
    p.KhachHangOrgName,
    p.SizeOrgName,
    p.PhuGiaOrgName,
    p.PhuGiaName,
    p.NhomHoaChatName,
    p.QuyCachName,
    p.ThongTinPhuName,
    p.LoaiPhieuPhanCo,
    p.LoaiQuyTrinh,
    p.MaSize,
    p.MaCongDoan,
    p.GhiChu2,
    p.GhiChu3
from (
        Select p.STT,
            --Them NgayGio
            p.Ngay,
            p.Gio,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qtc.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            sTP.Ten as SizeTPName,
            tp.ma as MaNhomCongViec,
            tp.Ten as NhomCongViecName,
            cd.Ten as CongViecName,
            p.MaLo,
            p.TrongLuong,
            p.VoXo,
            p.VoXo2,
            p.TrongLuongTare,
            p.MayCan,
            p.MaXuong,
            p.MaKhachHang,
            ks.Ten as KhangSinhName,
            ttnl.Ten as ThongTinNguyenLieuName,
            tp.IsPhanCo,
            tp.IsHoaChat,
            p.SoLuong,
            p.TrongLuongDonVi,
            p.IsCanTay,
            p.T,
            p.MaQuyTrinhOrg,
            p.MaKhachHangOrg,
            p.MaSizeOrg,
            p.MaPhuGiaOrg,
            p.MaPhuGia,
            p.MaNhomHoaChat,
            p.MaQuyCach,
            p.MaThongTinPhu,
            p.GhiChu,
            kh.Ten as KhachHangName,
            qt.Ten as QuyTrinhOrgName,
            khorg.Ten as KhachHangOrgName,
            size.Ten as SizeOrgName,
            pg.Ten as PhuGiaOrgName,
            pgr.Ten as PhuGiaName,
            nhcr.Ten as NhomHoaChatName,
            qcr.Ten as QuyCachName,
            ttp.Ten as ThongTinPhuName,
            ppc.LoaiPhieuPhanCo,
            qtc.LoaiQuyTrinh,
            p.MaSize,
            p.MaCongDoan,
            p.GhiChu2,
            p.GhiChu3
        from (
                -- xu ly ngay gio
                SELECT *
                from T_PhieuCan p1
                where CAST(p1.Ngay as datetime) + CAST(p1.Gio as datetime) <= @ngay
                    and CAST(p1.Ngay as datetime) + CAST(p1.Gio as datetime) >= @fromDate --p.Ngay = @ngay
                    and p1.MaXuong = @xuongId --and p3.MaThanhPham = @thanhPhamId
            ) p
            left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
            LEFT JOIN T_QuyTrinh qt on p.MaQuyTrinhOrg = qt.Ma
            LEFT JOIN T_KhachHang khorg on p.MaKhachHang = khorg.Ma
            LEFT JOIN T_Size size on p.MaSizeOrg = size.Ma
            LEFT JOIN T_PhuGia pg on p.MaPhuGiaOrg = pg.Ma
            LEFT JOIN T_PhuGia pgr on p.MaPhuGia = pgr.Ma
            LEFT JOIN T_NhomHoaChat nhcr on p.MaNhomHoaChat = nhcr.Ma
            LEFT JOIN T_QuyCach qcr on p.MaQuyCach = qcr.Ma
            LEFT JOIN T_ThongTinPhu ttp on p.MaThongTinPhu = ttp.Ma
            left JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
            LEFT JOIN T_Size s on p.MaSize = s.Ma
            LEFT JOIN T_Size sTP on p.MaSizeTP = sTP.Ma
            LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
            LEFT JOIN T_NhomLo nl on p.MaNhomLo = nl.Ma
            left JOIN T_SanPham sp on p.MaSanPham = sp.Ma
            LEFT JOIN T_CongDoan cd on p.MaCongDoan = cd.Ma
            LEFT JOIN T_QuyTrinh qtc on p.MaQuyTrinh = qtc.Ma
            LEFT JOIN T_KhangSinh ks on p.MaKhangSinh = ks.Ma
            left JOIN T_TrangThaiNguyenLieu ttnl on p.MaTrangThaiNguyenLieu = ttnl.Ma
            LEFT JOIN T_PhieuPhanCo ppc on p.MaPhieuPhanCo = ppc.Ma
    ) p
    LEFT JOIN T_ChiTietVoXo ctvx ON p.LoaiPhieuPhanCo = ctvx.MaPhieuPhanCo
    and p.VoXo = ctvx.VoXo
    and p.VoXo2 = ctvx.VoXo2
    and p.LoaiQuyTrinh = ctvx.MaQuyTrinh
    and p.MaSize = ctvx.MaSize
    and p.MaCongDoan = ctvx.MaCongDoan
order by STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { fromDate = fromDate, ngay = dateTime, xuongId, isDinhMucBinhThuong, khuVuc })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetChiTietsToNgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId,
            string khuVuc,
            string thanhPhamId,
            bool isDinhMucBinhThuong = true)
        {
            var query = @"Select p.STT,
    --them NgayGio
    p.Ngay,
    p.Gio,
    p.MaNhanVien,
    p.MaHoSo,
    p.TenNhanVien,
    p.Nhom,
    p.NhomLo,
    p.NgayNguyenLieu,
    p.SanPhamName,
    p.QuyTrinhName,
    p.IsDatKhangSinh,
    p.SizeName,
    case
        when p.IsPhanCo != 1 then (
            case
                when LEN(p.GhiChu) - LEN(REPLACE(p.GhiChu, '-', '')) >= 2 then LEFT(
                    Right(
                        p.GhiChu,
                        LEN(p.GhiChu) - CHARINDEX('-', p.GhiChu)
                    ),
                    CHARINDEX(
                        '-',
                        Right(
                            p.GhiChu,
                            LEN(p.GhiChu) - CHARINDEX('-', p.GhiChu)
                        )
                    ) -1
                )
                else ''
            end
        )
        else p.SizeTPName
    END as SizeTPName,
    p.MaNhomCongViec,
    p.NhomCongViecName,
    (
        case
            WHEN isnull(ctvx.MaSizeVoXo, '') = '' then p.CongViecName
            else concat(p.CongViecName, ' ', ctvx.MaSizeVoXo)
        end
    ) CongViecName,
    p.MaLo,
    p.TrongLuong,
    p.VoXo,
    case
        when p.IsPhanCo = 1 then CAST(cast(p.VoXo as decimal(18, 1)) as varchar(20))
        when p.IsHoaChat = 1 then (
            case
                when LEN(p.GhiChu) - LEN(REPLACE(p.GhiChu, ',', '')) >= 2 then LEFT(
                    Right(
                        p.GhiChu,
                        LEN(p.GhiChu) - CHARINDEX(',', p.GhiChu)
                    ),
                    CHARINDEX(
                        ',',
                        Right(
                            p.GhiChu,
                            LEN(p.GhiChu) - CHARINDEX(',', p.GhiChu)
                        )
                    ) -1
                )
                else ''
            end
        )
        else p.VoXo2
    END as VoXo2,
    p.TrongLuongTare,
    p.MayCan,
    p.MaXuong,
    p.MaKhachHang,
    p.KhangSinhName,
    p.ThongTinNguyenLieuName,
    p.IsPhanCo,
    p.SoLuong,
    p.TrongLuongDonVi,
    p.IsCanTay,
    p.T,
    p.MaQuyTrinhOrg,
    p.MaKhachHangOrg,
    p.MaSizeOrg,
    p.MaPhuGiaOrg,
    p.MaPhuGia,
    p.MaNhomHoaChat,
    p.MaQuyCach,
    p.MaThongTinPhu,
    p.GhiChu,
    p.KhachHangName,
    p.QuyTrinhOrgName,
    p.KhachHangOrgName,
    p.SizeOrgName,
    p.PhuGiaOrgName,
    p.PhuGiaName,
    p.NhomHoaChatName,
    p.QuyCachName,
    p.ThongTinPhuName,
    p.LoaiPhieuPhanCo,
    p.LoaiQuyTrinh,
    p.MaSize,
    p.MaCongDoan,
    p.GhiChu2,
    p.GhiChu3
from (
        Select p.STT,
            --Them NgayGio
            p.Ngay,
            p.Gio,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qtc.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            sTP.Ten as SizeTPName,
            tp.ma as MaNhomCongViec,
            tp.Ten as NhomCongViecName,
            cd.Ten as CongViecName,
            p.MaLo,
            p.TrongLuong,
            p.VoXo,
            p.VoXo2,
            p.TrongLuongTare,
            p.MayCan,
            p.MaXuong,
            p.MaKhachHang,
            ks.Ten as KhangSinhName,
            ttnl.Ten as ThongTinNguyenLieuName,
            tp.IsPhanCo,
            tp.IsHoaChat,
            p.SoLuong,
            p.TrongLuongDonVi,
            p.IsCanTay,
            p.T,
            p.MaQuyTrinhOrg,
            p.MaKhachHangOrg,
            p.MaSizeOrg,
            p.MaPhuGiaOrg,
            p.MaPhuGia,
            p.MaNhomHoaChat,
            p.MaQuyCach,
            p.MaThongTinPhu,
            p.GhiChu,
            kh.Ten as KhachHangName,
            qt.Ten as QuyTrinhOrgName,
            khorg.Ten as KhachHangOrgName,
            size.Ten as SizeOrgName,
            pg.Ten as PhuGiaOrgName,
            pgr.Ten as PhuGiaName,
            nhcr.Ten as NhomHoaChatName,
            qcr.Ten as QuyCachName,
            ttp.Ten as ThongTinPhuName,
            ppc.LoaiPhieuPhanCo,
            qtc.LoaiQuyTrinh,
            p.MaSize,
            p.MaCongDoan,
            p.GhiChu2,
            p.GhiChu3
        from (
                Select p.*
                from T_PhieuCan p,
                    T_NhomLo nl
                where p.MaNhomLo = nl.Ma
                    and nl.NgayNguyenLieu <= @ngay
                    and nl.NgayNguyenLieu >= @fromDate --p.Ngay = @ngay
                    and p.MaXuong = @xuongId
                    and p.MaThanhPham = @thanhPhamId
            ) p
            left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
            LEFT JOIN T_QuyTrinh qt on p.MaQuyTrinhOrg = qt.Ma
            LEFT JOIN T_KhachHang khorg on p.MaKhachHang = khorg.Ma
            LEFT JOIN T_Size size on p.MaSizeOrg = size.Ma
            LEFT JOIN T_PhuGia pg on p.MaPhuGiaOrg = pg.Ma
            LEFT JOIN T_PhuGia pgr on p.MaPhuGia = pgr.Ma
            LEFT JOIN T_NhomHoaChat nhcr on p.MaNhomHoaChat = nhcr.Ma
            LEFT JOIN T_QuyCach qcr on p.MaQuyCach = qcr.Ma
            LEFT JOIN T_ThongTinPhu ttp on p.MaThongTinPhu = ttp.Ma
            left JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
            LEFT JOIN T_Size s on p.MaSize = s.Ma
            LEFT JOIN T_Size sTP on p.MaSizeTP = sTP.Ma
            LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
            LEFT JOIN T_NhomLo nl on p.MaNhomLo = nl.Ma
            left JOIN T_SanPham sp on p.MaSanPham = sp.Ma
            LEFT JOIN T_CongDoan cd on p.MaCongDoan = cd.Ma
            LEFT JOIN T_QuyTrinh qtc on p.MaQuyTrinh = qtc.Ma
            LEFT JOIN T_KhangSinh ks on p.MaKhangSinh = ks.Ma
            left JOIN T_TrangThaiNguyenLieu ttnl on p.MaTrangThaiNguyenLieu = ttnl.Ma
            LEFT JOIN T_PhieuPhanCo ppc on p.MaPhieuPhanCo = ppc.Ma
    ) p
    LEFT JOIN T_ChiTietVoXo ctvx ON p.LoaiPhieuPhanCo = ctvx.MaPhieuPhanCo
    and p.VoXo = ctvx.VoXo
    and p.VoXo2 = ctvx.VoXo2
    and p.LoaiQuyTrinh = ctvx.MaQuyTrinh
    and p.MaSize = ctvx.MaSize
    and p.MaCongDoan = ctvx.MaCongDoan
order by STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new
                        {
                            fromDate = fromDate.Date,
                            ngay = dateTime.Date,
                            xuongId,
                            isDinhMucBinhThuong,
                            khuVuc,
                            thanhPhamId
                        })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetChiTietsToNgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true)
        {
            var query = @"Select p.STT,
    --them NgayGio
    p.Ngay,
    p.Gio,
    p.MaNhanVien,
    p.MaHoSo,
    p.TenNhanVien,
    p.Nhom,
    p.NhomLo,
    p.NgayNguyenLieu,
    p.SanPhamName,
    p.QuyTrinhName,
    p.IsDatKhangSinh,
    p.SizeName,
    case
        when p.IsPhanCo != 1 then (
            case
                when LEN(p.GhiChu) - LEN(REPLACE(p.GhiChu, '-', '')) >= 2 then LEFT(
                    Right(
                        p.GhiChu,
                        LEN(p.GhiChu) - CHARINDEX('-', p.GhiChu)
                    ),
                    CHARINDEX(
                        '-',
                        Right(
                            p.GhiChu,
                            LEN(p.GhiChu) - CHARINDEX('-', p.GhiChu)
                        )
                    ) -1
                )
                else ''
            end
        )
        else p.SizeTPName
    END as SizeTPName,
    p.MaNhomCongViec,
    p.NhomCongViecName,
    (
        case
            WHEN isnull(ctvx.MaSizeVoXo, '') = '' then p.CongViecName
            else concat(p.CongViecName, ' ', ctvx.MaSizeVoXo)
        end
    ) CongViecName,
    p.MaLo,
    p.TrongLuong,
    p.VoXo,
    case
        when p.IsPhanCo = 1 then CAST(cast(p.VoXo as decimal(18, 1)) as varchar(20))
        when p.IsHoaChat = 1 then (
            case
                when LEN(p.GhiChu) - LEN(REPLACE(p.GhiChu, ',', '')) >= 2 then LEFT(
                    Right(
                        p.GhiChu,
                        LEN(p.GhiChu) - CHARINDEX(',', p.GhiChu)
                    ),
                    CHARINDEX(
                        ',',
                        Right(
                            p.GhiChu,
                            LEN(p.GhiChu) - CHARINDEX(',', p.GhiChu)
                        )
                    ) -1
                )
                else ''
            end
        )
        else p.VoXo2
    END as VoXo2,
    p.TrongLuongTare,
    p.MayCan,
    p.MaXuong,
    p.MaKhachHang,
    p.KhangSinhName,
    p.ThongTinNguyenLieuName,
    p.IsPhanCo,
    p.SoLuong,
    p.TrongLuongDonVi,
    p.IsCanTay,
    p.T,
    p.MaQuyTrinhOrg,
    p.MaKhachHangOrg,
    p.MaSizeOrg,
    p.MaPhuGiaOrg,
    p.MaPhuGia,
    p.MaNhomHoaChat,
    p.MaQuyCach,
    p.MaThongTinPhu,
    p.GhiChu,
    p.KhachHangName,
    p.QuyTrinhOrgName,
    p.KhachHangOrgName,
    p.SizeOrgName,
    p.PhuGiaOrgName,
    p.PhuGiaName,
    p.NhomHoaChatName,
    p.QuyCachName,
    p.ThongTinPhuName,
    p.LoaiPhieuPhanCo,
    p.LoaiQuyTrinh,
    p.MaSize,
    p.MaCongDoan,
    p.GhiChu2,
    p.GhiChu3
from (
        Select p.STT,
            --Them NgayGio
            p.Ngay,
            p.Gio,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qtc.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            sTP.Ten as SizeTPName,
            tp.ma as MaNhomCongViec,
            tp.Ten as NhomCongViecName,
            cd.Ten as CongViecName,
            p.MaLo,
            p.TrongLuong,
            p.VoXo,
            p.VoXo2,
            p.TrongLuongTare,
            p.MayCan,
            p.MaXuong,
            p.MaKhachHang,
            ks.Ten as KhangSinhName,
            ttnl.Ten as ThongTinNguyenLieuName,
            tp.IsPhanCo,
            tp.IsHoaChat,
            p.SoLuong,
            p.TrongLuongDonVi,
            p.IsCanTay,
            p.T,
            p.MaQuyTrinhOrg,
            p.MaKhachHangOrg,
            p.MaSizeOrg,
            p.MaPhuGiaOrg,
            p.MaPhuGia,
            p.MaNhomHoaChat,
            p.MaQuyCach,
            p.MaThongTinPhu,
            p.GhiChu,
            kh.Ten as KhachHangName,
            qt.Ten as QuyTrinhOrgName,
            khorg.Ten as KhachHangOrgName,
            size.Ten as SizeOrgName,
            pg.Ten as PhuGiaOrgName,
            pgr.Ten as PhuGiaName,
            nhcr.Ten as NhomHoaChatName,
            qcr.Ten as QuyCachName,
            ttp.Ten as ThongTinPhuName,
            ppc.LoaiPhieuPhanCo,
            qtc.LoaiQuyTrinh,
            p.MaSize,
            p.MaCongDoan,
            p.GhiChu2,
            p.GhiChu3
        from (
                Select p.*
                from T_PhieuCan p,
                    T_NhomLo nl
                where p.MaNhomLo = nl.Ma
                    and nl.NgayNguyenLieu <= @ngay
                    and nl.NgayNguyenLieu >= @fromDate --p.Ngay = @ngay
                    and p.MaXuong = @xuongId
            ) p
            left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
            LEFT JOIN T_QuyTrinh qt on p.MaQuyTrinhOrg = qt.Ma
            LEFT JOIN T_KhachHang khorg on p.MaKhachHang = khorg.Ma
            LEFT JOIN T_Size size on p.MaSizeOrg = size.Ma
            LEFT JOIN T_PhuGia pg on p.MaPhuGiaOrg = pg.Ma
            LEFT JOIN T_PhuGia pgr on p.MaPhuGia = pgr.Ma
            LEFT JOIN T_NhomHoaChat nhcr on p.MaNhomHoaChat = nhcr.Ma
            LEFT JOIN T_QuyCach qcr on p.MaQuyCach = qcr.Ma
            LEFT JOIN T_ThongTinPhu ttp on p.MaThongTinPhu = ttp.Ma
            left JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
            LEFT JOIN T_Size s on p.MaSize = s.Ma
            LEFT JOIN T_Size sTP on p.MaSizeTP = sTP.Ma
            LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
            LEFT JOIN T_NhomLo nl on p.MaNhomLo = nl.Ma
            left JOIN T_SanPham sp on p.MaSanPham = sp.Ma
            LEFT JOIN T_CongDoan cd on p.MaCongDoan = cd.Ma
            LEFT JOIN T_QuyTrinh qtc on p.MaQuyTrinh = qtc.Ma
            LEFT JOIN T_KhangSinh ks on p.MaKhangSinh = ks.Ma
            left JOIN T_TrangThaiNguyenLieu ttnl on p.MaTrangThaiNguyenLieu = ttnl.Ma
            LEFT JOIN T_PhieuPhanCo ppc on p.MaPhieuPhanCo = ppc.Ma
    ) p
    LEFT JOIN T_ChiTietVoXo ctvx ON p.LoaiPhieuPhanCo = ctvx.MaPhieuPhanCo
    and p.VoXo = ctvx.VoXo
    and p.VoXo2 = ctvx.VoXo2
    and p.LoaiQuyTrinh = ctvx.MaQuyTrinh
    and p.MaSize = ctvx.MaSize
    and p.MaCongDoan = ctvx.MaCongDoan
order by STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { fromDate = fromDate.Date, ngay = dateTime.Date, xuongId, isDinhMucBinhThuong, khuVuc })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetChiTietsToNgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime dateTime,
            string xuongId)
        {
            var query = @"Select p.STT,
    --them NgayGio
    p.Ngay,
    p.Gio,
    p.MaNhanVien,
    p.MaHoSo,
    p.TenNhanVien,
    p.Nhom,
    p.NhomLo,
    p.NgayNguyenLieu,
    p.SanPhamName,
    p.QuyTrinhName,
    p.IsDatKhangSinh,
    p.SizeName,
    case
        when p.IsPhanCo != 1 then (
            case
                when LEN(p.GhiChu) - LEN(REPLACE(p.GhiChu, '-', '')) >= 2 then LEFT(
                    Right(
                        p.GhiChu,
                        LEN(p.GhiChu) - CHARINDEX('-', p.GhiChu)
                    ),
                    CHARINDEX(
                        '-',
                        Right(
                            p.GhiChu,
                            LEN(p.GhiChu) - CHARINDEX('-', p.GhiChu)
                        )
                    ) -1
                )
                else ''
            end
        )
        else p.SizeTPName
    END as SizeTPName,
    p.MaNhomCongViec,
    p.NhomCongViecName,
    (
        case
            WHEN isnull(ctvx.MaSizeVoXo, '') = '' then p.CongViecName
            else concat(p.CongViecName, ' ', ctvx.MaSizeVoXo)
        end
    ) CongViecName,
    p.MaLo,
    p.TrongLuong,
    p.VoXo,
    case
        when p.IsPhanCo = 1 then CAST(cast(p.VoXo as decimal(18, 1)) as varchar(20))
        when p.IsHoaChat = 1 then (
            case
                when LEN(p.GhiChu) - LEN(REPLACE(p.GhiChu, ',', '')) >= 2 then LEFT(
                    Right(
                        p.GhiChu,
                        LEN(p.GhiChu) - CHARINDEX(',', p.GhiChu)
                    ),
                    CHARINDEX(
                        ',',
                        Right(
                            p.GhiChu,
                            LEN(p.GhiChu) - CHARINDEX(',', p.GhiChu)
                        )
                    ) -1
                )
                else ''
            end
        )
        else p.VoXo2
    END as VoXo2,
    p.TrongLuongTare,
    p.MayCan,
    p.MaXuong,
    p.MaKhachHang,
    p.KhangSinhName,
    p.ThongTinNguyenLieuName,
    p.IsPhanCo,
    p.SoLuong,
    p.TrongLuongDonVi,
    p.IsCanTay,
    p.T,
    p.MaQuyTrinhOrg,
    p.MaKhachHangOrg,
    p.MaSizeOrg,
    p.MaPhuGiaOrg,
    p.MaPhuGia,
    p.MaNhomHoaChat,
    p.MaQuyCach,
    p.MaThongTinPhu,
    p.GhiChu,
    p.KhachHangName,
    p.QuyTrinhOrgName,
    p.KhachHangOrgName,
    p.SizeOrgName,
    p.PhuGiaOrgName,
    p.PhuGiaName,
    p.NhomHoaChatName,
    p.QuyCachName,
    p.ThongTinPhuName,
    p.LoaiPhieuPhanCo,
    p.LoaiQuyTrinh,
    p.MaSize,
    p.MaCongDoan,
    p.GhiChu2,
    p.GhiChu3
from (
        Select p.STT,
            --Them NgayGio
            p.Ngay,
            p.Gio,
            p.MaNhanVien,
            n.MaHoSo,
            n.Name as TenNhanVien,
            n.DeptName0 as Nhom,
            nl.ten as NhomLo,
            nl.NgayNguyenLieu,
            sp.Ten as SanPhamName,
            qtc.Ten as QuyTrinhName,
            p.IsDatKhangSinh,
            s.Ten as SizeName,
            sTP.Ten as SizeTPName,
            tp.ma as MaNhomCongViec,
            tp.Ten as NhomCongViecName,
            cd.Ten as CongViecName,
            p.MaLo,
            p.TrongLuong,
            p.VoXo,
            p.VoXo2,
            p.TrongLuongTare,
            p.MayCan,
            p.MaXuong,
            p.MaKhachHang,
            ks.Ten as KhangSinhName,
            ttnl.Ten as ThongTinNguyenLieuName,
            tp.IsPhanCo,
            tp.IsHoaChat,
            p.SoLuong,
            p.TrongLuongDonVi,
            p.IsCanTay,
            p.T,
            p.MaQuyTrinhOrg,
            p.MaKhachHangOrg,
            p.MaSizeOrg,
            p.MaPhuGiaOrg,
            p.MaPhuGia,
            p.MaNhomHoaChat,
            p.MaQuyCach,
            p.MaThongTinPhu,
            p.GhiChu,
            kh.Ten as KhachHangName,
            qt.Ten as QuyTrinhOrgName,
            khorg.Ten as KhachHangOrgName,
            size.Ten as SizeOrgName,
            pg.Ten as PhuGiaOrgName,
            pgr.Ten as PhuGiaName,
            nhcr.Ten as NhomHoaChatName,
            qcr.Ten as QuyCachName,
            ttp.Ten as ThongTinPhuName,
            ppc.LoaiPhieuPhanCo,
            qtc.LoaiQuyTrinh,
            p.MaSize,
            p.MaCongDoan,
            p.GhiChu2,
            p.GhiChu3
        from (
                Select p.*
                from T_PhieuCan p,
                    T_NhomLo nl
                where p.MaNhomLo = nl.Ma
                    and nl.NgayNguyenLieu <= @ngay
                    and nl.NgayNguyenLieu >= @fromDate --p.Ngay = @ngay
                    and p.MaXuong = @xuongId
            ) p
            left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
            LEFT JOIN T_QuyTrinh qt on p.MaQuyTrinhOrg = qt.Ma
            LEFT JOIN T_KhachHang khorg on p.MaKhachHang = khorg.Ma
            LEFT JOIN T_Size size on p.MaSizeOrg = size.Ma
            LEFT JOIN T_PhuGia pg on p.MaPhuGiaOrg = pg.Ma
            LEFT JOIN T_PhuGia pgr on p.MaPhuGia = pgr.Ma
            LEFT JOIN T_NhomHoaChat nhcr on p.MaNhomHoaChat = nhcr.Ma
            LEFT JOIN T_QuyCach qcr on p.MaQuyCach = qcr.Ma
            LEFT JOIN T_ThongTinPhu ttp on p.MaThongTinPhu = ttp.Ma
            left JOIN NhanVienDaiThanh n on p.MaNhanVien = n.MaNhanVien
            LEFT JOIN T_ThanhPham tp on p.MaThanhPham = tp.Ma
            LEFT JOIN T_Size s on p.MaSize = s.Ma
            LEFT JOIN T_Size sTP on p.MaSizeTP = sTP.Ma
            LEFT JOIN T_KhuVuc kv on p.MaKhuVuc = kv.Ma
            LEFT JOIN T_NhomLo nl on p.MaNhomLo = nl.Ma
            left JOIN T_SanPham sp on p.MaSanPham = sp.Ma
            LEFT JOIN T_CongDoan cd on p.MaCongDoan = cd.Ma
            LEFT JOIN T_QuyTrinh qtc on p.MaQuyTrinh = qtc.Ma
            LEFT JOIN T_KhangSinh ks on p.MaKhangSinh = ks.Ma
            left JOIN T_TrangThaiNguyenLieu ttnl on p.MaTrangThaiNguyenLieu = ttnl.Ma
            LEFT JOIN T_PhieuPhanCo ppc on p.MaPhieuPhanCo = ppc.Ma
    ) p
    LEFT JOIN T_ChiTietVoXo ctvx ON p.LoaiPhieuPhanCo = ctvx.MaPhieuPhanCo
    and p.VoXo = ctvx.VoXo
    and p.VoXo2 = ctvx.VoXo2
    and p.LoaiQuyTrinh = ctvx.MaQuyTrinh
    and p.MaSize = ctvx.MaSize
    and p.MaCongDoan = ctvx.MaCongDoan
order by STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { fromDate = fromDate.Date, ngay = dateTime.Date, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }
        #region Chắt thêm để xuất báo cáo KeNangXuatCMX

        public List<T> GetChiTiets3<T>(DateTime dateTime, string xuongId, string khuVuc,
            bool isDinhMucBinhThuong = true)
        {
            //                var query = @"Select p.*, kh.Ten as KhachHangName
            //from (
            //Select
            //        p.STT,
            //        p.Gio,
            //        p.MaNhanVien,
            //        n.MaHoSo,
            //        n.Name as TenNhanVien,
            //		n.DeptName0 as Nhom,
            //        nl.Ten as NhomLo,
            //        nl.NgayNguyenLieu,
            //        sp.Ten as SanPhamName,
            //		p.MaQuyTrinh,
            //        qt.Ten as QuyTrinhName,
            //        p.IsDatKhangSinh,
            //		p.MaSize,
            //        s.Ten as SizeName,
            //        tp.Ten as NhomCongViecName,
            //		p.MaCongDoan as MaCongViec,
            //        cd.Ten as CongViecName,
            //        p.MaLo,
            //        p.TrongLuong,
            //p.TrongLuongTare,
            //        p.MayCan,
            //        p.MaXuong,
            //        p.MaKhachHang
            //    from
            //        T_PhieuCan p,
            //        NhanVienDaiThanh n,
            //        T_ThanhPham tp,
            //        T_Size s,
            //        T_KhuVuc kv,
            //        T_NhomLo nl,
            //        T_SanPham sp,
            //        T_CongDoan cd,
            //        T_QuyTrinh qt
            //    where
            //		p.Ngay = @ngay
            //		--and p.Ngay >= @fromDate
            //		--and p.Ngay <= @toDate
            //    --and p.Ngay >= @fromDate
            //        and p.MaXuong = @xuongId
            //        and p.MaNhanVien = n.MaNhanVien
            //        and p.MaThanhPham = tp.Ma
            //        and p.MaSize = s.Ma
            //        and p.TrongLuong > 0
            //        and p.MaKhuVuc = kv.Ma
            //        and kv.ma = @khuVuc
            //        and p.MaNhomLo = nl.Ma
            //        and p.MaSanPham = sp.Ma
            //        and p.MaCongDoan = cd.Ma
            //        and p.MaQuyTrinh = qt.Ma) p
            //    left JOIN T_KhachHang kh
            //    on p.MaKhachHang = kh.Ma
            //order by STT DESC";
            var query = @"select 
	P.STT,
        p.Gio,
        nv.MaNhanVien,
        nv.MaHoSo,
        nv.Name as TenNhanVien,
		nv.DeptName0 as Nhom,
        p.NhomLo,
        p.NgayNguyenLieu,
        p.SanPhamName,
		p.MaQuyTrinh,
        p.QuyTrinhName,
        p.IsDatKhangSinh,
		p.MaSize,
        p.SizeName,
        p.NhomCongViecName,
		p.MaCongViec,
        p.CongViecName,
        p.MaLo,
       isnull( p.TrongLuong,0) as TrongLuong,
		--p.TrongLuongTare,
		isnull( p.TrongLuongTare,0) as TrongLuongTare,
        p.MayCan,
        --p.MaXuong,
        p.MaKhachHang,
		nv.Xuong AS MaXuong

	from
(Select nv.MaNhanVien,nv.Name,nv.MaHoSo,nv.DeptName0, nv.Xuong from NhanVienDaiThanh nv) NV
left join(Select
        p.STT,
        p.Gio,
        p.MaNhanVien,
        n.MaHoSo,
        n.Name as TenNhanVien,
		n.DeptName0 as Nhom,
        nl.Ten as NhomLo,
        nl.NgayNguyenLieu,
        sp.Ten as SanPhamName,
		p.MaQuyTrinh,
        qt.Ten as QuyTrinhName,
        p.IsDatKhangSinh,
		p.MaSize,
        s.Ten as SizeName,
        tp.Ten as NhomCongViecName,
		p.MaCongDoan as MaCongViec,
        cd.Ten as CongViecName,
        p.MaLo,
        p.TrongLuong,
p.TrongLuongTare,
        p.MayCan,
        p.MaXuong,
        p.MaKhachHang
    from
        T_PhieuCan p,
        NhanVienDaiThanh n,
        T_ThanhPham tp,
        T_Size s,
        T_KhuVuc kv,
        T_NhomLo nl,
        T_SanPham sp,
        T_CongDoan cd,
        T_QuyTrinh qt
    where
		p.Ngay = @ngay
		--and p.Ngay >= @fromDate
		--and p.Ngay <= @toDate
    --and p.Ngay >= @fromDate
        and p.MaXuong = @xuongId
        and p.MaNhanVien = n.MaNhanVien
        and p.MaThanhPham = tp.Ma
        and p.MaSize = s.Ma
        and p.TrongLuong > 0
        and p.MaKhuVuc = kv.Ma
        and kv.ma = @khuVuc
        and p.MaNhomLo = nl.Ma
        and p.MaSanPham = sp.Ma
        and p.MaCongDoan = cd.Ma
        and p.MaQuyTrinh = qt.Ma) P on NV.MaNhanVien = P.MaNhanVien

order by STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId, isDinhMucBinhThuong, khuVuc })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetChiTiets3<T>(DateTime fromDate, DateTime dateTime, string xuongId, string khuVuc,
            bool isDinhMucBinhThuong = true)
        {
            //                var query = @"Select p.*, kh.Ten as KhachHangName
            //from (
            //Select
            //        p.STT,
            //        p.Gio,
            //        p.MaNhanVien,
            //        n.MaHoSo,
            //        n.Name as TenNhanVien,
            //		n.DeptName0 as Nhom,
            //        nl.Ten as NhomLo,
            //        nl.NgayNguyenLieu,
            //        sp.Ten as SanPhamName,
            //		p.MaQuyTrinh,
            //        qt.Ten as QuyTrinhName,
            //        p.IsDatKhangSinh,
            //		p.MaSize,
            //        s.Ten as SizeName,
            //        tp.Ten as NhomCongViecName,
            //		p.MaCongDoan as MaCongViec,
            //        cd.Ten as CongViecName,
            //        p.MaLo,
            //        p.TrongLuong,
            //p.TrongLuongTare,
            //        p.MayCan,
            //        p.MaXuong,
            //        p.MaKhachHang
            //    from
            //        T_PhieuCan p,
            //        NhanVienDaiThanh n,
            //        T_ThanhPham tp,
            //        T_Size s,
            //        T_KhuVuc kv,
            //        T_NhomLo nl,
            //        T_SanPham sp,
            //        T_CongDoan cd,
            //        T_QuyTrinh qt
            //    where
            //		p.Ngay = @ngay
            //		--and p.Ngay >= @fromDate
            //		--and p.Ngay <= @toDate
            //    --and p.Ngay >= @fromDate
            //        and p.MaXuong = @xuongId
            //        and p.MaNhanVien = n.MaNhanVien
            //        and p.MaThanhPham = tp.Ma
            //        and p.MaSize = s.Ma
            //        and p.TrongLuong > 0
            //        and p.MaKhuVuc = kv.Ma
            //        and kv.ma = @khuVuc
            //        and p.MaNhomLo = nl.Ma
            //        and p.MaSanPham = sp.Ma
            //        and p.MaCongDoan = cd.Ma
            //        and p.MaQuyTrinh = qt.Ma) p
            //    left JOIN T_KhachHang kh
            //    on p.MaKhachHang = kh.Ma
            //order by STT DESC";
            var query = @"select 
	P.STT,
        p.Gio,
        nv.MaNhanVien,
        nv.MaHoSo,
        nv.Name as TenNhanVien,
		nv.DeptName0 as Nhom,
        p.NhomLo,
        p.NgayNguyenLieu,
        p.SanPhamName,
		p.MaQuyTrinh,
        p.QuyTrinhName,
        p.IsDatKhangSinh,
		p.MaSize,
        p.SizeName,
        p.NhomCongViecName,
		p.MaCongViec,
        p.CongViecName,
        p.MaLo,
       isnull( p.TrongLuong,0) as TrongLuong,
		--p.TrongLuongTare,
		isnull( p.TrongLuongTare,0) as TrongLuongTare,
        p.MayCan,
        --p.MaXuong,
        p.MaKhachHang,
		nv.Xuong AS MaXuong

	from
(Select nv.MaNhanVien,nv.Name,nv.MaHoSo,nv.DeptName0, nv.Xuong from NhanVienDaiThanh nv) NV
left join(Select
        p.STT,
        p.Gio,
        p.MaNhanVien,
        n.MaHoSo,
        n.Name as TenNhanVien,
		n.DeptName0 as Nhom,
        nl.Ten as NhomLo,
        nl.NgayNguyenLieu,
        sp.Ten as SanPhamName,
		p.MaQuyTrinh,
        qt.Ten as QuyTrinhName,
        p.IsDatKhangSinh,
		p.MaSize,
        s.Ten as SizeName,
        tp.Ten as NhomCongViecName,
		p.MaCongDoan as MaCongViec,
        cd.Ten as CongViecName,
        p.MaLo,
        p.TrongLuong,
p.TrongLuongTare,
        p.MayCan,
        p.MaXuong,
        p.MaKhachHang
    from
        T_PhieuCan p,
        NhanVienDaiThanh n,
        T_ThanhPham tp,
        T_Size s,
        T_KhuVuc kv,
        T_NhomLo nl,
        T_SanPham sp,
        T_CongDoan cd,
        T_QuyTrinh qt
    where
		--p.Ngay = @ngay
        p.Ngay <= @ngay
        and p.Ngay >= @fromDate
		--and p.Ngay >= @fromDate
		--and p.Ngay <= @toDate
    --and p.Ngay >= @fromDate
        and p.MaXuong = @xuongId
        and p.MaNhanVien = n.MaNhanVien
        and p.MaThanhPham = tp.Ma
        and p.MaSize = s.Ma
        and p.TrongLuong > 0
        and p.MaKhuVuc = kv.Ma
        and kv.ma = @khuVuc
        and p.MaNhomLo = nl.Ma
        and p.MaSanPham = sp.Ma
        and p.MaCongDoan = cd.Ma
        and p.MaQuyTrinh = qt.Ma) P on NV.MaNhanVien = P.MaNhanVien

order by STT DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { fromDate = fromDate.Date, ngay = dateTime.Date, xuongId, isDinhMucBinhThuong, khuVuc })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetChiTietPhanCo<T>(DateTime dateTime, string xuongId, string khuVuc,
            bool isDinhMucBinhThuong = true)
        {
            var query = string.Empty;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId, isDinhMucBinhThuong, khuVuc })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetTongHopNhanViens3<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            //                var query = @"Select
//	nv.MaNhanVien,nv.Name as TenNhanVien,nv.MaHoSo,nv.DeptName0 as Nhom,nv.Xuong,
//	p.*,
//    kh.Ten as KhachHangName
//from
//	(Select nv.MaNhanVien,nv.Name,nv.MaHoSo,nv.DeptName0, nv.Xuong from NhanVienDaiThanh nv) NV
//  LEFT JOIN  ( 
//        Select
//            p.Ngay,
//            p.MaNhanVien as MNV,
//            nl.Ten as NhomLo,
//            nl.NgayNguyenLieu,
//            sp.Ten as SanPhamName,
//            qt.Ten as QuyTrinhName,
//            p.IsDatKhangSinh,
//			p.MaSize,
//            s.Ten as SizeName,
//			p.MaThanhPham,
//            tp.Ten as NhomCongViecName,
//			p.MaCongDoan as MaCongViec,
//            cd.Ten as CongViecName,
//            p.MaLo,
//            p.VoXo,
//            p.TrongLuong,
//            p.SoRo,
//            p.MayCan,
//            p.MaXuong,
//            p.MaKhachHang,
//            ks.Ten as KhangSinhName,
//            ttp.Ten as ThongTinPhuName,
//            ttnl.Ten as ThongTinNguyenLieuName,
//            pg.Ten as PhuGiaName,
//            pHC.GioBatDauLoKH
//        from
//            (
//                select
//                    p.Ngay,
//                    p.MaNhanVien,
//                    p.MaNhomLo,
//                    p.MaSanPham,
//                    p.MaQuyTrinh,
//                    p.IsDatKhangSinh,
//                    p.MaSize,
//                    p.MaThanhPham,
//                    p.MaCongDoan,
//                    p.MaLo,
//                    p.VoXo,
//                    sum(p.TrongLuong) as TrongLuong,
//                    COUNT(*) as SoRo,
//                    p.MayCan,
//                    p.MaXuong,
//                    p.MaKhachHang,
//                    p.MaKhangSinh,
//                    p.MaThongTinPhu,
//                    p.MaTrangThaiNguyenLieu,
//                    p.MaPhuGia
//                from
//                    T_PhieuCan p
//                where
//                    p.Ngay >= @fromDate
//                    and p.Ngay <= @toDate
//                    and p.MaXuong = @xuongId
//                    and p.MaKhuVuc = @khuVuc
//                    and p.TrongLuong > 0
//                GROUP BY
//                    p.Ngay,
//                    p.MaNhanVien,
//                    p.MaNhomLo,
//                    p.MaSanPham,
//                    p.MaQuyTrinh,
//                    p.IsDatKhangSinh,
//                    p.MaSize,
//                    p.MaThanhPham,
//                    p.MaCongDoan,
//                    p.MaLo,
//                    p.VoXo,
//                    p.MayCan,
//                    p.MaXuong,
//                    p.MaKhachHang,
//                    p.MaKhangSinh,
//                    p.MaThongTinPhu,
//                    p.MaTrangThaiNguyenLieu,
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
//                select
//                    p.Ngay,
//                    p.MaNhomLo,
//                    p.MaKhachHang,
//                    Min(p.Gio) as GioBatDauLoKH
//                from
//                    T_PhieuCan p
//                GROUP BY
//                    p.Ngay,
//                    p.MaNhomLo,
//                    p.MaKhachHang
//            ) pHC on p.Ngay = pHC.Ngay
//            and p.MaNhomLo = phc.MaNhomLo
//            and p.MaKhachHang = pHC.MaKhachHang
//    ) p on NV.MaNhanVien = p.MNV
//    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
//order by
//    NV.MaHoSo DESC";
            var query = @"Select nv.MaNhanVien,
    nv.Name as TenNhanVien,
    nv.MaHoSo,
    nv.DeptName0 as Nhom,
    nv.Xuong,
    kh.Ten as KhachHangName,
    (
        case
            WHEN isnull(ctvx.MaSizeVoXo, '') = '' then p.CongViecName
            else concat(p.CongViecName, ' ', ctvx.MaSizeVoXo)
        end
    ) CongViecName,
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
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia,
                    p.MaPhieuPhanCo
                from T_PhieuCan p
                where p.Ngay >= @fromDate
                    and p.Ngay <= @toDate
                    and p.MaXuong = @xuongId
                    and p.MaKhuVuc = @khuVuc
                    and p.TrongLuong > 0
                GROUP BY p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia,
                    p.MaPhieuPhanCo
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
                from T_PhieuCan p
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
and p.VoXo2 = ctvx.VoXo
    and p.LoaiQuyTrinh = ctvx.MaQuyTrinh
    and p.MaSize = ctvx.MaSize
    and p.MaCongViec = ctvx.MaCongDoan
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
order by NV.MaHoSo DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, khuVuc })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopNhanViens3ToNgayNguyenLieu<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var query = @"Select nv.MaNhanVien,
    nv.Name as TenNhanVien,
    nv.MaHoSo,
    nv.DeptName0 as Nhom,
    nv.Xuong,
    kh.Ten as KhachHangName,
    (
        case
            WHEN isnull(ctvx.MaSizeVoXo, '') = '' then p.CongViecName
            else concat(p.CongViecName, ' ', ctvx.MaSizeVoXo)
        end
    ) CongViecName,
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
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia,
                    p.MaPhieuPhanCo
                from T_PhieuCan p,
                    T_NhomLo nl
                where p.MaNhomLo = nl.Ma
                    and nl.NgayNguyenLieu >= @fromDate
                    and nl.NgayNguyenLieu <= @toDate
                    and p.MaXuong = @xuongId
                    and p.MaKhuVuc = @khuVuc
                    and p.TrongLuong > 0
                GROUP BY p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia,
                    p.MaPhieuPhanCo
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
                from T_PhieuCan p
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
and p.VoXo2 =ctvx.VoXo2
    and p.LoaiQuyTrinh = ctvx.MaQuyTrinh
    and p.MaSize = ctvx.MaSize
    and p.MaCongViec = ctvx.MaCongDoan
    left JOIN T_KhachHang kh on p.MaKhachHang = kh.Ma
order by NV.MaHoSo DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection
                    .QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, khuVuc })
                    .Result
                    .ToList();
                return items;
            }
        }
        public List<T> GetTongHopNhanViens3DateTimeToDateTime<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            string khuVuc,
            bool isDinhMucBinhThuong = true,
            bool isFloor = true)
        {
            var query = @"Select nv.MaNhanVien,
    nv.Name as TenNhanVien,
    nv.MaHoSo,
    nv.DeptName0 as Nhom,
    nv.Xuong,
    kh.Ten as KhachHangName,
    (
        case
            WHEN isnull(ctvx.MaSizeVoXo, '') = '' then p.CongViecName
            else concat(p.CongViecName, ' ', ctvx.MaSizeVoXo)
        end
    ) CongViecName,
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
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia,
                    p.MaPhieuPhanCo
                from T_PhieuCan p
                 where CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
                    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime)  <= @toDate
                    and p.MaXuong = @xuongId
                    and p.MaKhuVuc = @khuVuc
                    and p.TrongLuong > 0
                GROUP BY p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia,
                    p.MaPhieuPhanCo
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
                from T_PhieuCan p
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
                    .QueryAsync<T>(query, new { fromDate = fromDate.Date, toDate = toDate.Date, xuongId, khuVuc })
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
            var query = @"Select nv.MaNhanVien,
    nv.Name as TenNhanVien,
    nv.MaHoSo,
    nv.DeptName0 as Nhom,
    nv.Xuong,
    kh.Ten as KhachHangName,
    (
        case
            WHEN isnull(ctvx.MaSizeVoXo, '') = '' then p.CongViecName
            else concat(p.CongViecName, ' ', ctvx.MaSizeVoXo)
        end
    ) CongViecName,
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
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia,
                    p.MaPhieuPhanCo
                from T_PhieuCan p,
                    T_NhomLo nl,
                    NhanVienDaiThanh nv
                where p.MaNhomLo = nl.Ma
                    and nl.NgayNguyenLieu >= @fromDate
                    and nl.NgayNguyenLieu <= @toDate
                    and p.MaXuong = @xuongId
                    and p.MaKhuVuc = @khuVuc
                    and nv.MaNhanVien = p.MaNhanVien
                    and nv.IsNhom = @isNhom
                    and p.TrongLuong > 0
                GROUP BY p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia,
                    p.MaPhieuPhanCo
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
                from T_PhieuCan p
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
        public List<T> GetTongHopNhanViens3DateTimeToDateTimeNSRC<T>(
            DateTime fromDate,
            DateTime toDate,
            string xuongId,
            bool isNhom
            )
        {
            var query = @"Select nv.MaNhanVien,
    nv.Name as TenNhanVien,
    nv.MaHoSo,
    nv.DeptName0 as Nhom,
    nv.Xuong,
    kh.Ten as KhachHangName,
    (
        case
            WHEN isnull(ctvx.MaSizeVoXo, '') = '' then p.CongViecName
            else concat(p.CongViecName, ' ', ctvx.MaSizeVoXo)
        end
    ) CongViecName,
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
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    sum(p.TrongLuong) as TrongLuong,
                    COUNT(*) as SoRo,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia,
                    p.MaPhieuPhanCo
                from T_PhieuCan p,
                    NhanVienDaiThanh nv
                where CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) >= @fromDate
                    and CAST(p.Ngay as datetime) + CAST(p.Gio as datetime) <= @toDate
                    and p.MaXuong = @xuongId
                    and p.TrongLuong > 0
                    and nv.MaNhanVien = p.MaNhanVien
                    and nv.IsNhom = @isNhom
                GROUP BY p.Ngay,
                    p.MaNhanVien,
                    p.MaNhomLo,
                    p.MaSanPham,
                    p.MaQuyTrinh,
                    p.IsDatKhangSinh,
                    p.MaSize,
                    p.MaThanhPham,
                    p.MaCongDoan,
                    p.MaLo,
                    p.VoXo,
p.VoXo2,
                    p.MayCan,
                    p.MaXuong,
                    p.MaKhachHang,
                    p.MaKhangSinh,
                    p.MaThongTinPhu,
                    p.MaTrangThaiNguyenLieu,
                    p.MaPhuGia,
                    p.MaPhieuPhanCo
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
                from T_PhieuCan p
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
                    .QueryAsync<T>(query, new { fromDate = fromDate, toDate = toDate, xuongId, isNhom = isNhom })
                    .Result
                    .ToList();
                return items;
            }
        }
        #endregion

        public List<T> GetChiTiets<T>(DateTime dateTime, string xuongId, string khuVuc, bool isDinhMucBinhThuong = true)
        {
            //                var query = @"Select
            //    tp.STT,
            //    tp.Gio,
            //    tp.MaNhanVien,
            //    tp.MaHoSo,
            //    tp.TenNhanVien,
            //    tp.ThanhPhamName,
            //    tp.SizeName,
            //    tp.LoaiNguyenLieuName,
            //    --tp.MauName,
            //    Case
            //        when tp.SizeName = N'Cá Lớn' then 'L'
            //        when tp.SizeName = N'Cá Nhỏ' then 'N'
            //        else tp.SizeName
            //    end as SizeName,
            //    --case
            //    --    when tp.CaTra = 1 then 'True'
            //    --    when tp.CaTra = 0 then 'F'
            //    --    else ''
            //    --end as CaTra,
            //    --tp.ChiSanLuong,
            //    tp.MaLo,
            //    tp.DinhMuc,
            //    --dm.DinhMuc as DinhMucChuan,
            //	case 
            //		when dm.DinhMuc is NULL then 0
            //		else dm.DinhMuc
            //	end as DinhMucChuan,
            //    tp.TrongLuong,
            //    Case
            //        when @isDinhMucBinhThuong = 1 then case
            //            when tp.DinhMuc <= dm.DinhMuc then N'Đạt'
            //            else N'Không Đạt'
            //        end
            //        else case
            //            when tp.DinhMuc >= dm.DinhMuc then N'Đạt'
            //            else N'Không Đạt'
            //        end
            //    end as DanhGia,
            //    tp.MayCan
            //	--tp.KhuVuc
            //from
            //    (
            //        Select
            //            ptp.STT,
            //            ptp.Gio,
            //            ptp.MaNhanVien,
            //            n.MaHoSo,
            //            n.Name as TenNhanVien,
            //            tp.Ma as MaThanhPham,
            //            tp.Ten as ThanhPhamName,
            //            s.Ma as MaSize,
            //            s.Ten as SizeName,
            //            --ptp.CaTra,
            //            --ptp.ChiSanLuong,
            //            ptp.MaLo,
            //            --ptp.MaMau,
            //            ptp.MaLoaiNguyenLieu,
            //            la.Ten as LoaiNguyenLieuName,
            //            --mau.Ten as MauName,
            //            case
            //                when @isDinhMucBinhThuong = 1 then floor(
            //                    100 * ptp.TrongLuongNhan / ptp.TrongLuong
            //                ) / 100
            //                else ptp.TrongLuong / ptp.TrongLuongNhan
            //            end as DinhMuc,
            //            ptp.TrongLuong,
            //            ptp.MayCan
            //			--kv.Ten as KhuVuc
            //        from
            //            T_PhieuCan ptp,
            //            NhanVienDaiThanh n,
            //            T_ThanhPham tp,
            //            T_Size s,
            //            --MaMauDinhHinh mau,
            //            T_LoaiNguyenLieu la,
            //			T_KhuVuc kv
            //        where
            //            ptp.Ngay = @ngay
            //            and ptp.MaXuong = @xuongId
            //            and ptp.MaNhanVien = n.MaNhanVien
            //            and ptp.MaThanhPham = tp.Ma
            //            and ptp.MaSize = s.Ma
            //            --and ptp.MaMau = mau.Ma
            //            and ptp.MaLoaiNguyenLieu = la.Ma
            //            and ptp.TrongLuong > 0
            //			and ptp.MaKhuVuc = kv.Ma and kv.ma = @khuVuc
            //    ) tp
            //    left join(
            //        Select
            //            *
            //        from
            //            (
            //                Select
            //                    d.*,
            //                    ROW_NUMBER() OVER (
            //                        PARTITION BY MaLo,
            //                        MaLoaiNguyenLieu,
            //                        --MaMau,
            //                        MaSize,
            //                        MaThanhPham
            //                        --CaTra
            //                        ORDER BY
            //                            Gio DESC
            //                    ) AS [ROW NUMBER]
            //                from
            //                    T_DinhMuc d
            //                where
            //                    Ngay = @ngay
            //                    And MaXuong = @xuongId
            //            ) dm
            //        Where
            //            dm.[ROW NUMBER] = 1
            //    ) dm on tp.MaThanhPham = dm.MaThanhPham
            //    and tp.MaLo = dm.MaLo
            //    and tp.MaSize = dm.MaSize
            //    --and tp.CaTra = dm.CaTra
            //    --and tp.MaMau = dm.MaMau
            //    and tp.MaLoaiNguyenLieu = dm.MaLoaiNguyenLieu
            //order by
            //    Gio";
            var query = @"Select
    ptp.STT,
    ptp.Gio,
    ptp.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,
    tp.Ma as MaThanhPham,
    tp.Ten as ThanhPhamName,
    s.Ma as MaSize,
    s.Ten as SizeName,
    sp.Ten as SanPhamName,
    cd.Ten as CongDoanName,
    ptp.MaLo,
    nl.Ten as NhomLo,
    nl.NgayNguyenLieu,
    ptp.MaLoaiNguyenLieu,
    la.Ten as LoaiNguyenLieuName,
    ptp.TrongLuong,
    ptp.MayCan,
    ptp.MaXuong --kv.Ten as KhuVuc
from
    T_PhieuCan ptp,
    NhanVienDaiThanh n,
    T_ThanhPham tp,
    T_Size s,
    T_LoaiNguyenLieu la,
    T_KhuVuc kv,
    T_NhomLo nl,
    T_SanPham sp,
    T_CongDoan cd
where
    ptp.Ngay = @ngay
    and ptp.MaXuong = @xuongId
    and ptp.MaNhanVien = n.MaNhanVien
    and ptp.MaThanhPham = tp.Ma
    and ptp.MaSize = s.Ma --and ptp.MaMau = mau.Ma
    and ptp.MaLoaiNguyenLieu = la.Ma
    and ptp.TrongLuong > 0
    and ptp.MaKhuVuc = kv.Ma
    and kv.ma = @khuVuc
    and ptp.MaNhomLo = nl.Ma
    and ptp.MaSanPham = sp.Ma
    and ptp.MaCongDoan = cd.Ma";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId, isDinhMucBinhThuong, khuVuc })
                    .Result
                    .ToList();
                return items;
            }
        }

        public List<T> GetChiTietsNgam<T>(string maNhomLo, string khuVuc = "NGAM")
        {
            var query = @"Select
    ptp.STT,
    ptp.Gio,
 ptp.Ngay,
    ptp.MaNhanVien,
    n.MaHoSo,
    n.Name as TenNhanVien,
    b.Ten as BonName,
    ptp.MaLoaiCan as LoaiCan,
sp.Ten as SanPhamName,
    tp.Ma as MaThanhPham,
    tp.Ten as ThanhPhamName,
    s.Ma as MaSize,
    s.Ten as SizeName,
    ptp.MaLo,
    nl.Ten as NhomLo,
    ptp.MaLoaiNguyenLieu,
    la.Ten as LoaiNguyenLieuName,
    ptp.TrongLuong,
    ptp.MayCan,
    ptp.MaXuong
--kv.Ten as KhuVuc
from
    T_PhieuCan ptp,
    NhanVienDaiThanh n,
    T_ThanhPham tp,
    T_Size s,
    T_LoaiNguyenLieu la,
    T_NhomLo nl,
 T_SanPham sp,
    T_Bon b
where ptp.MaNhanVien = n.MaNhanVien
    and ptp.MaThanhPham = tp.Ma
    and ptp.MaSize = s.Ma --and ptp.MaMau = mau.Ma
    and ptp.MaLoaiNguyenLieu = la.Ma
    and ptp.TrongLuong > 0
    and ptp.MaKhuVuc = @khuVuc
    and ptp.MaNhomLo = nl.Ma
 and ptp.MaNhomLo = @maNhomLo
    and ptp.MaBon = b.Ma
 and ptp.MaSanPham = sp.Ma
order by MayCan,STT";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(query, new { maNhomLo, khuVuc }).Result.ToList();
                return items;
            }
        }

        public DataTable GetChiTiets(DateTime dateTime, string xuongId)
        {
            var query = @"SELECT
    p.STT,
    p.[Ngày],
    p.[Giờ],
    p.[Lô],
    p.[Mã Nhân Viên],
    p.[Mã Hồ Sơ],
    p.[Tên Nhân Viên],
    p.[Loại Nguyên Liệu],
    p.[Thành Phẩm],
    p.[Size],
    p.[Trọng Lượng],
    p.[Mã Thẻ],
    p.[Máy Cân],
    p.[Xưởng],
	p.[Khu Vực]
from
    (
        Select
            p.STT,
            p.Ngay as [Ngày],
            Cast(
                convert(
                    varchar(19),
                    cast(@ngay as datetime) + Cast(p.Gio as datetime),
                    120
                ) as datetime
            ) as [Giờ],
            p.Malo as [Lô],
            p.MaNhanVien as [Mã Nhân Viên],
            n.MaHoSo as [Mã Hồ Sơ],
            n.Name as [Tên Nhân Viên],
            la.Ten as [Loại Nguyên Liệu],
            tp.Ten as [Thành Phẩm],
            s.Ten as [Size],
            p.TrongLuong as [Trọng Lượng],
            p.MaThe as [Mã Thẻ],
            p.MayCan as [Máy Cân],
            p.MaXuong as [Xưởng],
			p.MaKhuVuc as [Khu Vực]
        from
            T_PhieuCan p,
            T_LoaiNguyenLieu la,
            T_ThanhPham tp,
            T_Size s,
			T_KhuVuc kv,
            NhanVienDaiThanh n
        where
            p.Ngay = @ngay
            and p.MaXuong = @xuongId
            and p.MaNhanVien = n.MaNhanVien
            and p.MaLoaiNguyenLieu = la.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma 
			and p.MaKhuVuc = kv.Ma
    ) p
order by
    p.STT desc,
    p.[Lô],
    p.[Máy Cân]";
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

        public DataTable GetChiTiets(DateTime dateTime, string xuongId, string khuVuc)
        {
            var query = @"SELECT
    p.STT,
    p.[Ngày],
    p.[Giờ],
    p.[Lô],
    p.[Mã Nhân Viên],
    p.[Mã Hồ Sơ],
    p.[Tên Nhân Viên],
    p.[Loại Nguyên Liệu],
    p.[Thành Phẩm],
    p.[Size],
    p.[Trọng Lượng],
    p.[Mã Thẻ],
    p.[Máy Cân],
    p.[Xưởng],
	p.[Khu Vực]
from
    (
        Select
            p.STT,
            p.Ngay as [Ngày],
            Cast(
                convert(
                    varchar(19),
                    cast(@ngay as datetime) + Cast(p.Gio as datetime),
                    120
                ) as datetime
            ) as [Giờ],
            p.Malo as [Lô],
            p.MaNhanVien as [Mã Nhân Viên],
            n.MaHoSo as [Mã Hồ Sơ],
            n.Name as [Tên Nhân Viên],
            la.Ten as [Loại Nguyên Liệu],
            tp.Ten as [Thành Phẩm],
            s.Ten as [Size],
            p.TrongLuong as [Trọng Lượng],
            p.MaThe as [Mã Thẻ],
            p.MayCan as [Máy Cân],
            p.MaXuong as [Xưởng],
			p.MaKhuVuc as [Khu Vực]
        from
            T_PhieuCan p,
            T_LoaiNguyenLieu la,
            T_ThanhPham tp,
            T_Size s,
			T_KhuVuc kv,
            NhanVienDaiThanh n
        where
            p.Ngay = @ngay
            and p.MaXuong = @xuongId
			and p.MaKhuVuc = kv.Ma  and kv.Ma =@khuVuc
            and p.MaNhanVien = n.MaNhanVien
            and p.MaLoaiNguyenLieu = la.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma 
		
    ) p
	
order by
    p.STT desc,
    p.[Lô],
    p.[Máy Cân]";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", dateTime.Date);
            cmd.Parameters.AddWithValue("@xuongId", xuongId);
            cmd.Parameters.AddWithValue("@khuVuc", khuVuc);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetChiTiets(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var query = @"SELECT
    p.STT,
    p.[Ngày],
    p.[Giờ],
    p.[Lô],
    p.[Mã Nhân Viên],
    p.[Mã Hồ Sơ],
    p.[Tên Nhân Viên],
    p.[Loại Nguyên Liệu],
    p.[Thành Phẩm],
    p.[Size],
    p.[Trọng Lượng],
    p.[Mã Thẻ],
    p.[Máy Cân],
    p.[Xưởng],
	P.[Khu Vực]
from
    (
        Select
            p.STT,
            p.Ngay as [Ngày],
            Cast(
                convert(
                    varchar(19),
                    cast(@ngay as datetime) + Cast(p.Gio as datetime),
                    120
                ) as datetime
            ) as [Giờ],
            p.Malo as [Lô],
            p.MaNhanVien as [Mã Nhân Viên],
            n.MaHoSo as [Mã Hồ Sơ],
            n.Name as [Tên Nhân Viên],
            la.Ten as [Loại Nguyên Liệu],
            tp.Ten as [Thành Phẩm],
            s.Ten as [Size],
            p.TrongLuong as [Trọng Lượng],
            p.MaThe as [Mã Thẻ],
            p.MayCan as [Máy Cân],
            p.MaXuong as [Xưởng],
			p.MaKhuVuc as [Khu Vực]
        from
            T_PhieuCan p,
            T_LoaiNguyenLieu la,
            T_ThanhPham tp,
            T_Size s,
			T_KhuVuc kv,
            NhanVienDaiThanh n
        where
            p.Ngay <= @ngay
            and p.Ngay >= @fromDate
            and p.MaXuong = @xuongId
            and p.MaNhanVien = n.MaNhanVien
            and p.MaLoaiNguyenLieu = la.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
			and p.MaKhuVuc = kv.Ma
    ) p
order by
    p.STT desc,
    p.[Lô],
    p.[Máy Cân]";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", toDate.Date);
            cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
            cmd.Parameters.AddWithValue("@xuongId", xuongId);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public DataTable GetChiTiets(DateTime fromDate, DateTime toDate, string xuongId, string khuVuc)
        {
            var query = @"SELECT
    p.STT,
    p.[Ngày],
    p.[Giờ],
    p.[Lô],
    p.[Mã Nhân Viên],
    p.[Mã Hồ Sơ],
    p.[Tên Nhân Viên],
    p.[Loại Nguyên Liệu],
    p.[Thành Phẩm],
    p.[Size],
    p.[Trọng Lượng],
    p.[Mã Thẻ],
    p.[Máy Cân],
    p.[Xưởng],
	P.[Khu Vực]
from
    (
        Select
            p.STT,
            p.Ngay as [Ngày],
            Cast(
                convert(
                    varchar(19),
                    cast(@ngay as datetime) + Cast(p.Gio as datetime),
                    120
                ) as datetime
            ) as [Giờ],
            p.Malo as [Lô],
            p.MaNhanVien as [Mã Nhân Viên],
            n.MaHoSo as [Mã Hồ Sơ],
            n.Name as [Tên Nhân Viên],
            la.Ten as [Loại Nguyên Liệu],
            tp.Ten as [Thành Phẩm],
            s.Ten as [Size],
            p.TrongLuong as [Trọng Lượng],
            p.MaThe as [Mã Thẻ],
            p.MayCan as [Máy Cân],
            p.MaXuong as [Xưởng],
			p.MaKhuVuc as [Khu Vực]
        from
            T_PhieuCan p,
            T_LoaiNguyenLieu la,
            T_ThanhPham tp,
            T_Size s,
			T_KhuVuc kv,
            NhanVienDaiThanh n
        where
            p.Ngay <= @ngay
            and p.Ngay >= @fromDate
            and p.MaXuong = @xuongId
            and p.MaNhanVien = n.MaNhanVien
            and p.MaLoaiNguyenLieu = la.Ma
            and p.MaThanhPham = tp.Ma
            and p.MaSize = s.Ma
			and p.MaKhuVuc = kv.Ma  and kv.Ma =@khuVuc
    ) p
order by
    p.STT desc,
    p.[Lô],
    p.[Máy Cân]";
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ngay", toDate.Date);
            cmd.Parameters.AddWithValue("@fromDate", fromDate.Date);
            cmd.Parameters.AddWithValue("@xuongId", xuongId);
            cmd.Parameters.AddWithValue("@khuVuc", khuVuc);
            connection.Open();
            using var da = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();
            da.Fill(dataTable);
            return dataTable;
        }

        public T GetLastByThe<T>(DateTime dateTime, string theId, bool isEnabled = false)
        {
            //var query = "Select top(1) * from PhieuCanBTPFilletv2 where Ngay = @ngay And MaThe = @theId And IsEnabled =@isEnabled order by Gio Desc";
            var query = @"Select top(1) [STT]
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
      ,[GhiChu],[TrongLuongTare] from T_PhieuCan where Ngay = @ngay And MaThe = @theId And IsEnabled =@isEnabled order by Gio Desc";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var item = connection.Query<T>(
                        query,
                        new { ngay = dateTime.Date, theId, isEnabled })
                    .SingleOrDefault();
                return item;
            }
        }

        public List<T> GetChiTiets_TG<T>(DateTime dateTime)
        {
            var query = @"SELECT
    p.Ngay,
    p.STT,
    p.Gio,
    p.TrongLuong  + p.TrongLuongTare as TrongLuongCan,
    p.TrongLuongTare,
    p.TrongLuong,
    tp.Ten as MaSanPham,
    p.MaNhanVien,
    p.MaLo,
    p.MaXuong,
    p.MayCan,
    0 as DonGia,
    0 * p.TrongLuong as ThanhTien,
    p.MaHoSo
from
    (
        Select
            p.*,
            nv.MaHoSo
        from
            T_PhieuCan p,
            NhanVienDaiThanh nv
        where
            p.Ngay = @ngay
            and p.MaNhanVien = nv.MaNhanVien
    ) p
    left join T_ThanhPham tp on p.MaThanhPham = tp.Ma
order by
    p.STT";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { ngay = dateTime.Date }).ToList();
            return items;
        }

        public List<T> GetChiTiets_TG<T>(DateTime dateTime, string khuVuc)
        {
            var query = @"SELECT
    p.Ngay,
    p.STT,
    p.Gio,
    p.TrongLuong  + p.TrongLuongTare as TrongLuongCan,
    p.TrongLuongTare,
    p.TrongLuong,
    tp.Ten as MaSanPham,
    p.MaNhanVien,
    p.MaLo,
    p.MaXuong,
    p.MayCan,
    0 as DonGia,
    0 * p.TrongLuong as ThanhTien, 
    p.MaHoSo,
	p.MaKhuVuc
from
    (
        Select
            p.*,
            nv.MaHoSo
			
			
        from
            T_PhieuCan p,
            NhanVienDaiThanh nv,
			T_KhuVuc kv
        where
            p.Ngay = @ngay
			and p.MaKhuVuc = kv.Ma  and kv.Ma =@khuVuc
            and p.MaNhanVien = nv.MaNhanVien
			
    ) p
    left join T_ThanhPham tp on p.MaThanhPham = tp.Ma
order by
    p.STT";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { ngay = dateTime.Date, khuVuc }).ToList();
            return items;
        }


        public List<T> GetChiTiets_TG<T>(DateTime fromDate, DateTime toDate)
        {
            var query = @"SELECT
    p.Ngay,
    p.STT,
    p.Gio,
    p.TrongLuong  + p.TrongLuongTare as TrongLuongCan,
    p.TrongLuongTare,
    p.TrongLuong,
    tp.Ten as MaSanPham,
    p.MaNhanVien,
    p.MaLo,
    p.MaXuong,
    p.MayCan,
    0 as DonGia,
    0 * p.TrongLuong as ThanhTien, --0?
    p.MaHoSo,
	p.MaKhuVuc
from
    (
        Select
            p.*,
            nv.MaHoSo
        from
            T_PhieuCan p,
            NhanVienDaiThanh nv
        where
            p.Ngay <= @ngay and p.Ngay>=@fromDate
            and p.MaNhanVien = nv.MaNhanVien
    ) p
    left join T_ThanhPham tp on p.MaThanhPham = tp.Ma
order by
    p.STT";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query, new { ngay = toDate.Date, fromDate = fromDate.Date }).ToList();
            return items;
        }

        public List<T> GetChiTiets_TG<T>(DateTime fromDate, DateTime toDate, string khuVuc)
        {
            var query = @"SELECT
    p.Ngay,
    p.STT,
    p.Gio,
    p.TrongLuong  + p.TrongLuongTare as TrongLuongCan,
    p.TrongLuongTare,
    p.TrongLuong,
    tp.Ten as MaSanPham,
    p.MaNhanVien,
    p.MaLo,
    p.MaXuong,
    p.MayCan,
    0 as DonGia,
    0 * p.TrongLuong as ThanhTien, --0?
    p.MaHoSo,
	p.MaKhuVuc
from
    (
        Select
            p.*,
            nv.MaHoSo
        from
            T_PhieuCan p,
            NhanVienDaiThanh nv,
			T_KhuVuc kv
        where
            p.Ngay <= @ngay and p.Ngay>=@fromDate
            and p.MaNhanVien = nv.MaNhanVien
			and p.MaKhuVuc = kv.Ma  and kv.Ma =@khuVuc
    ) p
    left join T_ThanhPham tp on p.MaThanhPham = tp.Ma
order by
    p.STT";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(
                    query,
                    new { ngay = toDate.Date, fromDate = fromDate.Date, khuVuc })
                .ToList();
            return items;
        }

        public List<T> GetTheTuDaCap<T>()
        {
            var query = @"select 
nv.MaNhanVien,
nv.MaHoSo,
nv.Name as [TenNhanVien],
tt.MaTheTu,
tt.NgayGio

from
TheTu tt
 left join NhanVienDaiThanh nv on tt.MaNhanVien = nv.MaNhanVien";
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var items = connection.Query<T>(query).ToList();
            return items;
        }

        #endregion
    }
}
 