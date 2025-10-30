using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "API_User",
                columns: table => new
                {
                    UserName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DefaultPassword = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "BanCatTiet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    X1 = table.Column<int>(type: "int", nullable: false),
                    Y1 = table.Column<int>(type: "int", nullable: false),
                    X2 = table.Column<int>(type: "int", nullable: false),
                    Y2 = table.Column<int>(type: "int", nullable: false),
                    ColSpanX1 = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ColSpanX2 = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsShowX1 = table.Column<bool>(type: "bit", nullable: false),
                    IsShowX2 = table.Column<bool>(type: "bit", nullable: false),
                    IsDetect = table.Column<bool>(type: "bit", nullable: false),
                    IsFalse = table.Column<bool>(type: "bit", nullable: false),
                    DoUuTienX1 = table.Column<int>(type: "int", nullable: false),
                    DoUuTienX2 = table.Column<int>(type: "int", nullable: false),
                    SoLanChiaCaX1 = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    SoLanChiaCaX2 = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsKhoaX1 = table.Column<bool>(type: "bit", nullable: false),
                    IsKhoaX2 = table.Column<bool>(type: "bit", nullable: false),
                    ThoiGianQuangDuongPheu1X1 = table.Column<int>(type: "int", nullable: false, defaultValue: 1000),
                    ThoiGianQuangDuongPheu1X2 = table.Column<int>(type: "int", nullable: false, defaultValue: 1000),
                    ThoiGianQuangDuongPheu2X1 = table.Column<int>(type: "int", nullable: false, defaultValue: 1000),
                    ThoiGianQuangDuongPheu2X2 = table.Column<int>(type: "int", nullable: false, defaultValue: 1000),
                    PlcAdr = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "00"),
                    TimeOpen = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    PlcValue = table.Column<bool>(type: "bit", nullable: false),
                    VongChiaCa = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    PlcYadr = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "00"),
                    PlcYValue = table.Column<bool>(type: "bit", nullable: false),
                    ViTri_HX1 = table.Column<int>(type: "int", nullable: false),
                    ViTri_HX2 = table.Column<int>(type: "int", nullable: false),
                    ThoiGianNhanCaX1 = table.Column<int>(type: "int", nullable: false, defaultValue: 1000),
                    ThoiGianNhanCaX2 = table.Column<int>(type: "int", nullable: false, defaultValue: 1000),
                    PlcOffAdr = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "00"),
                    PlcOffValue = table.Column<bool>(type: "bit", nullable: false),
                    IsCaMuoiX1 = table.Column<bool>(type: "bit", nullable: false),
                    IsCaMuoiX2 = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanCatTiet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "BanCatTiet_temp",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    X1 = table.Column<int>(type: "int", nullable: false),
                    Y1 = table.Column<int>(type: "int", nullable: false),
                    X2 = table.Column<int>(type: "int", nullable: false),
                    Y2 = table.Column<int>(type: "int", nullable: false),
                    ColSpanX1 = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ColSpanX2 = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsShowX1 = table.Column<bool>(type: "bit", nullable: false),
                    IsShowX2 = table.Column<bool>(type: "bit", nullable: false),
                    IsDetect = table.Column<bool>(type: "bit", nullable: false),
                    IsFalse = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanCatTiet_temp", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "BanCatTietTheoLine",
                columns: table => new
                {
                    MaBanCatTiet = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLine = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanCatTietTheoLine", x => new { x.MaBanCatTiet, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "BanFillet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo_BanFillet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "BanQuyenPhanMem",
                columns: table => new
                {
                    MaMayTinh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenMayTinh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanQuyenPhanMem", x => x.MaMayTinh);
                });

            migrationBuilder.CreateTable(
                name: "BieuMau_Navico",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    BoPhan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaCongDoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NguyenLieu_Ngay = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    NguyenLieu_Dem = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ThanhPham_Ngay = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ThanhPham_Dem = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MaNhanVienCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNhaMay = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ThoiDiemGhiNhanSanLuong = table.Column<TimeOnly>(type: "time", nullable: false, defaultValueSql: "(getdate())"),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKV = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BieuMau_Navico", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoPhan",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoPhan", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "BoTriLoSizeThanhPham",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CodeId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: ""),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaViTri = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false, defaultValueSql: "(getdate())"),
                    MaSizePhu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoTriLoSizeThanhPham", x => new { x.Id, x.CodeId });
                });

            migrationBuilder.CreateTable(
                name: "BoTriNhanVienTheoSize",
                columns: table => new
                {
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ThoiGianBatDau = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoTriNhanVienTheoSize", x => new { x.Ngay, x.MaNhanVien, x.MaSize, x.ThoiGianBatDau, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "BoTriNhomKiemSoChe",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaNhomKiem = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SoGio = table.Column<double>(type: "float", nullable: false),
                    TyLeHuong = table.Column<double>(type: "float", nullable: false),
                    TyLeTru = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoTriNhomKiemSoChe", x => new { x.MaNhanVien, x.Ngay, x.MaNhomKiem, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "BoTriNhomSoChe",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaNhomSoChe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SoGio = table.Column<double>(type: "float", nullable: false),
                    TyLeHuong = table.Column<double>(type: "float", nullable: false),
                    TyLeTru = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoTriNhomSoChe", x => new { x.MaNhanVien, x.Ngay, x.MaNhomSoChe, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "BoTriTinhLuonXepKhuon",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaCongViec = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    IsNhom = table.Column<bool>(type: "bit", nullable: false),
                    TyLeHuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 1m),
                    TyLeTru = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoGio = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 8m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiTriTinhLuonXepKhuon", x => new { x.MaNhanVien, x.Ngay, x.MaCongViec, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "BravoSoChe",
                columns: table => new
                {
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BravoSoChe", x => x.MaThanhPham);
                });

            migrationBuilder.CreateTable(
                name: "BT_KhachHang",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BT_KhachHang", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "BT_MaLoaiCa",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BT_MaLoaiCa", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "BT_MaThanhPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BT_MaThanhPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "BT_NhanVienTheoNhom",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhom = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "((1))"),
                    TyLeTru = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TyLeHuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 1m),
                    SoGio = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 8m),
                    MaNhomCongViec = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaCongViec = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BT_NhanVienTheoNhom", x => new { x.MaNhanVien, x.MaNhom, x.Ngay, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "BT_NhomTinhLuong",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BT_NhomTinhLuong", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "BT_PhieuCan",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhachHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreateBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "default"),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "default"),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BT_PhieuCan", x => new { x.STT, x.Ngay, x.MaMayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "ChiTietRaCoi",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    MaCoi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Luot = table.Column<int>(type: "int", nullable: false),
                    IsDone = table.Column<bool>(type: "bit", nullable: false),
                    NgayNguyenLieu = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietRaCoi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChucVu",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChucVu", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "CoiLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCoi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ActionName = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    NgayGio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    HzQuay = table.Column<int>(type: "int", nullable: false),
                    HzRa = table.Column<int>(type: "int", nullable: false),
                    TimeQuay = table.Column<int>(type: "int", nullable: false),
                    IsError = table.Column<bool>(type: "bit", nullable: false),
                    ErrorStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Decription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdMonitor = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    NhanVienId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaChatLuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoiLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoiMonitor",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCoi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayGio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    InLocked = table.Column<bool>(type: "bit", nullable: false),
                    OutLocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NgayNguyenLieu = table.Column<DateTime>(type: "date", nullable: false),
                    TimeROut = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoiMonitor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ColorCode",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "none"),
                    Code2 = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "none"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ColorRGB = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorCode", x => new { x.Code, x.Code2 });
                });

            migrationBuilder.CreateTable(
                name: "CongViecPhuFillet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhanSize = table.Column<bool>(type: "bit", nullable: false),
                    NguonSanLuong = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "1: Xuong, 2: Ban Cat Tiet, 3: Line"),
                    SanPhamId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CongViecPhuFillet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "CongViecTinhLuongXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    BravoIdDem = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LoaiDuLieu = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "1: Lay Tu Bang,2:Nhap Tay,3:Ca 2,4:Lay tu san luong ra coi theo phan bo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CongViecTinhLuongXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "CongViecTinhLuongXepKhuonSanLuong",
                columns: table => new
                {
                    MaCongViec = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SanLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CongViecXepKhuonSanLuong", x => new { x.MaCongViec, x.Ngay, x.MaCa, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "CongViecTinhLuongXepKhuonTheoLoaiThanhPham",
                columns: table => new
                {
                    MaCongViec = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPhamDinhHinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPhamPhuXepKhuon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPhamChinhXepKhuon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPhamBlockXepKhuon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPhamKHCXepKhuon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPhamTaiChe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPhamSoChe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCongViecTaiChe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: " "),
                    MaCongDoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: " "),
                    MaChieuXaChinhXepKhuon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: " ")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CongViecXepKhuonTheoLoaiThanhPham", x => new { x.MaCongViec, x.MaThanhPhamDinhHinh, x.MaThanhPhamPhuXepKhuon, x.MaThanhPhamChinhXepKhuon, x.MaThanhPhamBlockXepKhuon, x.MaThanhPhamKHCXepKhuon, x.MaThanhPhamTaiChe, x.MaThanhPhamSoChe, x.MaCongViecTaiChe, x.MaCongDoan, x.MaChieuXaChinhXepKhuon });
                });

            migrationBuilder.CreateTable(
                name: "DanhMucFormMoTrucTiep",
                columns: table => new
                {
                    TenForm = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenHienThi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucFormMoTrucTiep", x => x.TenForm);
                });

            migrationBuilder.CreateTable(
                name: "DanhSachMayTinh",
                columns: table => new
                {
                    IDMayTinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiaChiKetNoi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DataSQL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserSQL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PassSQL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    KhuVuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhSachMayTinh", x => x.IDMayTinh);
                });

            migrationBuilder.CreateTable(
                name: "DanhSachToKiem",
                columns: table => new
                {
                    MaToKiem = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    LoaiBoTri = table.Column<int>(type: "int", nullable: false),
                    SoGio = table.Column<double>(type: "float", nullable: false, defaultValue: 8.0),
                    TyLe = table.Column<double>(type: "float", nullable: false, defaultValue: 1.0),
                    TyLeTru = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhSachToKiem_1", x => new { x.MaToKiem, x.MaXuong, x.MaNhanVien, x.Ngay, x.LoaiBoTri });
                });

            migrationBuilder.CreateTable(
                name: "DataFilletv2",
                columns: table => new
                {
                    STT = table.Column<double>(type: "float", nullable: true),
                    Gio = table.Column<DateTime>(type: "datetime", nullable: true),
                    MaNhanVien = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MaHoSo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BanNameSL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ThanhPhamName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ThanhPhamNameSL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MaMayCan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MaBan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MaBanSL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MaThanhPham = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MaThanhPhamSL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MaNhanVienPhucVu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ThanhPhamFN = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TimesFN = table.Column<double>(type: "float", nullable: true),
                    SecFN = table.Column<double>(type: "float", nullable: true),
                    MinuFN = table.Column<double>(type: "float", nullable: true),
                    MaThanhPhamFN = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "DG_DonGia",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiDonGia = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    DanhGia = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DinhMucDown = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    DinhMucUp = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    HeSo = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 1m),
                    DonGia = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CreateBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "default"),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "default"),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeSoRot = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 1m),
                    IsUsedHeSoRot = table.Column<bool>(type: "bit", nullable: false),
                    Range = table.Column<int>(type: "int", nullable: false),
                    MaSizeDinhHinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    DonGiaGiaCong = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MaXepHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, defaultValue: "F"),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, defaultValue: ""),
                    LoaiCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, defaultValue: ""),
                    MaSizeFillet = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DG_DonGia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DG_DonGiaT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongDoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gia = table.Column<int>(type: "int", nullable: false),
                    NgayGio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DG_DonGiaT", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DG_LoaiDonGia",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DienGiai = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DG_LoaiDonGia", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "DG_SanPhamTinhLuong",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DG_SanPhamTinhLuong", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "DinhMucDinhHinh",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CaTra = table.Column<bool>(type: "bit", nullable: false),
                    DinhMuc = table.Column<double>(type: "float", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DinhMucDinhHinh", x => new { x.STT, x.Ngay, x.Gio, x.MaLo, x.MaLoaiCa, x.MaMau, x.MaSize, x.MaThanhPham, x.MaXuong, x.CaTra });
                });

            migrationBuilder.CreateTable(
                name: "DinhMucFillet",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CaTra = table.Column<bool>(type: "bit", nullable: false),
                    DinhMuc = table.Column<double>(type: "float", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DinhMucFillet", x => new { x.STT, x.Ngay, x.Gio, x.MaLo, x.MaLoaiCa, x.MaMau, x.MaSize, x.MaThanhPham, x.MaXuong, x.CaTra });
                });

            migrationBuilder.CreateTable(
                name: "DinhMucXepHang",
                columns: table => new
                {
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXepHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    DinhMucUp = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    DinhMucDown = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    DinhMuc = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DinhMucXepHang", x => new { x.MaLo, x.MaSanPham, x.MaXepHang, x.Year });
                });

            migrationBuilder.CreateTable(
                name: "GioVaoRaFillet",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaCongViec = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "((0))"),
                    GioVao = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    GioRa = table.Column<TimeSpan>(type: "time(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GioVaoRaFillet", x => new { x.STT, x.MaNhanVien, x.Ngay, x.MaCongViec, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "GioVaoRaTinhLuongXepKhuon",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaCongViec = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GioVao = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    GioRa = table.Column<TimeSpan>(type: "time(7)", nullable: true),
                    MaCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "((1))"),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreateBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "default"),
                    ModifyDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "default"),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "default")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GioVaoRaTinhLuongXepKhuon", x => new { x.STT, x.MaNhanVien, x.Ngay, x.MaCongViec, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "KD_DonHang",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreateDate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KD_DonHang", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "KD_PhieuCan",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    MaTui = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SoLuongPhanTu = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KD_PhieuCan", x => new { x.STT, x.Ngay, x.MayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "KD_QuyCach",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KD_QuyCach", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "KD_Size",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KD_Size", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "KD_ThanhPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KD_ThanhPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "KD_Tui",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaDonHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PhuTroi = table.Column<decimal>(type: "decimal(18,5)", nullable: false, defaultValue: 0.01m),
                    PhuTroiMax = table.Column<decimal>(type: "decimal(18,5)", nullable: false, defaultValue: 0.02m),
                    TongTrongLuong = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaQuyCach = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KD_Tui", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "KNH_PhieuCan",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    MaThongTinSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TrongLuongKiemLai = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    SoLuongPhanTu = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PhuTroi = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KNH_PhieuCan", x => new { x.STT, x.Ngay, x.MayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "KNH_QuyCach",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KNH_QuyCach", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "KNH_Size",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KNH_Size", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "KNH_ThanhPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KNH_ThanhPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "KNH_ThongTinSanPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    PhuTroi = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    SoLuongPhanTu = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaQuyCach = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsonValue = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Thuoc Tinh, Gia Tri"),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KNH_ThongTinSanPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "LineFillet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineFillet", x => new { x.Ma, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "LineFilletv2",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CodeId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineFilletv2", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "LoaiCaXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiCaXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "LogKetChuyen",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    MaKhuVuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PCName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogKetChuyen", x => new { x.STT, x.MaKhuVuc, x.Ngay, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "LogKetChuyenBravo",
                columns: table => new
                {
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    tab = table.Column<int>(type: "int", nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    NgayChuyen = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogKetChuyenBravo", x => new { x.MaSanPham, x.Ngay, x.MaXuong, x.tab });
                });

            migrationBuilder.CreateTable(
                name: "LogKiemSoatKetChuyen",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PCName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    tab = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "XOA"),
                    NgayChuyen = table.Column<DateTime>(type: "date", nullable: false),
                    GioChuyen = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    CreateDayTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogKiemSoatKetChuyen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoNguyenLieu",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NhomLoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoNguyenLieu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoTheoLine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CodeId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: ""),
                    MaLine = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoTheoLine", x => new { x.Id, x.CodeId });
                });

            migrationBuilder.CreateTable(
                name: "MaAoVungNuoi",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaAoVungNuoi", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaCa",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FromTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    ToTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaCa", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaChatLuongTaiChe",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaChatLuongTaiChe", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaChatLuongXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaChatLuongXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaChatLuongXepKhuonBlock",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaChatLuongXepKhuonBlock", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaChieuXaXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaChieuXaXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaChuAoVungNuoi",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaChuAoVungNuoi", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaCoiXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrongLuongMax = table.Column<double>(type: "float", nullable: false),
                    Tam = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaCoiXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaCongDoanVungNuoi",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaCongDoanVungNuoi", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaCongDoanXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaCongDoanXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaCongViecTaiChe",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaCongViecTaiChe", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaGheVungNuoi",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaGheVungNuoi", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaKhachHangCaChet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaKhachHangCaChet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaKhachHangXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaKhachHangXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaKhuVucXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaKhuVucXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLo",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NgayTao = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLo", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoaiCaCaChet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaCaChet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoaiCaCaoThit",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaCaoThit", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoaiCaCatTiet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaCatTiet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoaiCaDinhHinh",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaDinhHinh", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoaiCaFillet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaFillet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoaiCaGiongVungNuoi",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DanhThanhId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    DaiThanhId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaGiongVungNuoi", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoaiCaLangDa",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaLangDa", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoaiCaNguyenLieu",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaNguyenLieu", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoaiCaNguyenLieu_temp",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaNguyenLieu_temp", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoaiCaPhuPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaPhuPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoaiCaSoCheDinhHinh",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaSoCheDinhHinh", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoaiCaTaiChe",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaTaiChe", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoaiCaVungNuoi",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CanLai = table.Column<bool>(type: "bit", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DaiThanhId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IsTap = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaVungNuoi", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoaiCaXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaLoi",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoi", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaMauCaGiongVungNuoi",
                columns: table => new
                {
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaGhe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SoLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrongLuongDonVi = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 1m),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaMauCaGiongVungNuoi", x => new { x.Ngay, x.MaGhe });
                });

            migrationBuilder.CreateTable(
                name: "MaMauCatTiet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaMauCatTiet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaMauDinhHinh",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaMauDinhHinh", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaMauFillet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaMauFillet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaMauLangDa",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaMauLangDa", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaMauNguyenLieu",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaMauNguyenLieu", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaMauNguyenLieu_temp",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaMauNguyenLieu_temp", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaMauPhuPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaMauPhuPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaMauTaiChe",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaMauTaiChe", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaMauXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaMauXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaMauXepKhuonBlock",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaMauXepKhuonBlock", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaNetXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrongLuong = table.Column<double>(type: "float", nullable: false, defaultValue: 5.0),
                    BienDo = table.Column<double>(type: "float", nullable: false, defaultValue: 0.02),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaNetXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaNhanVienTheoNhomXepKhuon",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhom = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    TyLeHuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 1m),
                    TyLeTru = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoGio = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 8m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaNhanVienTheoNhomXepKhuon_1", x => new { x.MaNhanVien, x.MaNhom, x.Ngay });
                });

            migrationBuilder.CreateTable(
                name: "MaNhomXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaNhomXepKhuon_1", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MapThanhPhamFillet",
                columns: table => new
                {
                    MaLoaiCaFillet = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaTPFillet = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsTangCa = table.Column<bool>(type: "bit", nullable: false),
                    MaBravoFillet = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapThanhPhamFillet", x => new { x.MaLoaiCaFillet, x.MaTPFillet, x.MaSize, x.IsTangCa });
                });

            migrationBuilder.CreateTable(
                name: "MaQuyCachCaoThit",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaQuyCachCaoThit", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaQuyCachXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaQuyCachXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaSanPhamDinhHinhBravo",
                columns: table => new
                {
                    DaiThanhId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSanPhamDinhHinhBravo", x => x.DaiThanhId);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeCaoThit",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeCaoThit", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeCatTiet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeCatTiet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeChinhXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    _type = table.Column<int>(type: "int", nullable: false),
                    Idx = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeChinhXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeDinhHinh",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeDinhHinh", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeFillet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeFillet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeLangDa",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeLangDa", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeNguyenLieu",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeNguyenLieu", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeNguyenLieu_temp",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeNguyenLieu_temp", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaSizePhuPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizePhuPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeTaiChe",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    _type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeTaiChe", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeXepKhuonBlock",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SizeXepKhuonBlock", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeXepKhuonKHC",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeXepKhuonKHC", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPham_PhoiTron",
                columns: table => new
                {
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaThanhPhamOrg = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPhamDes = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhuVuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TyLe = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreateBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ModifyDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPham_PhoiTron", x => new { x.Ngay, x.MaThanhPhamOrg, x.MaThanhPhamDes, x.MaKhuVuc, x.MaXuong, x.MaLo });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamCaoThit",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamCaoThit", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamCatTiet",
                columns: table => new
                {
                    MaCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    Min = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Max = table.Column<double>(type: "float", nullable: true, defaultValue: 999.0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamCatTiet", x => new { x.MaCa, x.Ma });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamChinhXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    Min = table.Column<double>(type: "float", nullable: false),
                    Max = table.Column<double>(type: "float", nullable: false, defaultValue: 500.0),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    _type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamChinhXepKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamDinhHinh",
                columns: table => new
                {
                    MaCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    DinhMuc = table.Column<double>(type: "float", nullable: false, defaultValue: 1.0),
                    Min = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Max = table.Column<double>(type: "float", nullable: true, defaultValue: 999.0),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TyLeDinhMucDau = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0.5m),
                    TyLeDinhMucRot = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0.5m),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    IsDauVaoBatBuoc = table.Column<bool>(type: "bit", nullable: false),
                    DinhMucKhongDauVao = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 1m),
                    IsDisplay = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CodeId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: ""),
                    DinhMucCaTra = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 1m),
                    BaoCaoDauRot = table.Column<bool>(type: "bit", nullable: false),
                    IsSuDungThoiGianGiuaLoaiThanhPham = table.Column<bool>(type: "bit", nullable: false),
                    MinOut = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    MaxOut = table.Column<double>(type: "float", nullable: true, defaultValue: 999.0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamDinhHinh", x => new { x.MaCa, x.Ma });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamDinhHinh_Color",
                columns: table => new
                {
                    ColorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamDinhHinh_Color_1", x => new { x.ColorCode, x.Ngay, x.MaLo, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamDinhHinh_TyLe",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayGio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TyLeDau = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TyLeRot = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamDinhHinh_TyLe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamExDinhHinh",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaLoaiCaExDinhHinh", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamFillet",
                columns: table => new
                {
                    MaCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    Min = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Max = table.Column<double>(type: "float", nullable: true, defaultValue: 999.0),
                    IsSoChe = table.Column<bool>(type: "bit", nullable: false),
                    IsCaMuoi = table.Column<bool>(type: "bit", nullable: false),
                    DinhMuc = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 1m),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThaiThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "NORMAL"),
                    TrongLuongHienTai = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThoiGianTren1kgSeconds = table.Column<decimal>(type: "decimal(18,3)", nullable: false, comment: "Don Vi Giay"),
                    DinhMucHaoHut = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 1m),
                    CodeId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: ""),
                    IsNotSetByTime = table.Column<bool>(type: "bit", nullable: false),
                    ColorRGB = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    KhongPhanBietSize = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamFillet", x => new { x.MaCa, x.Ma });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamFillet_Color",
                columns: table => new
                {
                    ColorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamFillet_Color", x => new { x.ColorCode, x.Ngay, x.MaLo, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamFillet_HanMucTrongLuong",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamFillet_HanMucTrongLuong", x => new { x.STT, x.Ngay, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamFillet_ThanhPhamMacDinh",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamFillet_ThanhPhamMacDinh", x => new { x.STT, x.Ngay, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamLangDa",
                columns: table => new
                {
                    MaCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    Min = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Max = table.Column<double>(type: "float", nullable: true, defaultValue: 999.0),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamLangDa", x => new { x.MaCa, x.Ma });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamLangDa_Color",
                columns: table => new
                {
                    ColorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamLangDa_Color_1", x => new { x.ColorCode, x.Ngay, x.MaLo, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamNguyenLieu",
                columns: table => new
                {
                    MaCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    Min = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Max = table.Column<double>(type: "float", nullable: true, defaultValue: 999.0),
                    IsSNL = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsNgopGhe = table.Column<bool>(type: "bit", nullable: false),
                    IsNgopGheMuoi = table.Column<bool>(type: "bit", nullable: false),
                    IsMuoiGhePhuPham = table.Column<bool>(type: "bit", nullable: false),
                    IsNgopAoMuoi = table.Column<bool>(type: "bit", nullable: false),
                    IsNgopAoPhuPham = table.Column<bool>(type: "bit", nullable: false),
                    IsDatNho = table.Column<bool>(type: "bit", nullable: false),
                    IsPhuPhamCaTap = table.Column<bool>(type: "bit", nullable: false),
                    IsCaCanTin = table.Column<bool>(type: "bit", nullable: false),
                    IsCaNgopXeMuoi = table.Column<bool>(type: "bit", nullable: false),
                    IsNgopGhePhuPham = table.Column<bool>(type: "bit", nullable: false),
                    IsNgopXePhuPham = table.Column<bool>(type: "bit", nullable: false),
                    IsManh = table.Column<bool>(type: "bit", nullable: false),
                    TyLeNuoc = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    IsCaNgopGheTuoiBanNgoai = table.Column<bool>(type: "bit", nullable: false),
                    IsCaNgopGheAoBanNgoai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamNguyenLieu", x => new { x.MaCa, x.Ma });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamNguyenLieu_temp",
                columns: table => new
                {
                    MaCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    Min = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Max = table.Column<double>(type: "float", nullable: true, defaultValue: 999.0),
                    IsSNL = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsNgopGhe = table.Column<bool>(type: "bit", nullable: false),
                    IsNgopGheMuoi = table.Column<bool>(type: "bit", nullable: false),
                    IsMuoiGhePhuPham = table.Column<bool>(type: "bit", nullable: false),
                    IsNgopAoMuoi = table.Column<bool>(type: "bit", nullable: false),
                    IsNgopAoPhuPham = table.Column<bool>(type: "bit", nullable: false),
                    IsDatNho = table.Column<bool>(type: "bit", nullable: false),
                    IsPhuPhamCaTap = table.Column<bool>(type: "bit", nullable: false),
                    IsCaCanTin = table.Column<bool>(type: "bit", nullable: false),
                    IsCaNgopXeMuoi = table.Column<bool>(type: "bit", nullable: false),
                    IsNgopGhePhuPham = table.Column<bool>(type: "bit", nullable: false),
                    IsNgopXePhuPham = table.Column<bool>(type: "bit", nullable: false),
                    IsManh = table.Column<bool>(type: "bit", nullable: false),
                    TyLeNuoc = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    IsCaNgopGheTuoiBanNgoai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamNguyenLieu_temp", x => new { x.MaCa, x.Ma });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamPhuPham",
                columns: table => new
                {
                    MaCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    Min = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0),
                    Max = table.Column<double>(type: "float", nullable: true, defaultValue: 999.0),
                    BarvoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamPhuPham", x => new { x.MaCa, x.Ma });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamPhuPham_Color",
                columns: table => new
                {
                    ColorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamPhuPham_Color_1", x => new { x.ColorCode, x.Ngay, x.MaLo, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamSoCheDinhHinh",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    X = table.Column<int>(type: "int", nullable: false, defaultValue: -1),
                    Y = table.Column<int>(type: "int", nullable: false, defaultValue: -1),
                    Min = table.Column<double>(type: "float", nullable: false),
                    Max = table.Column<double>(type: "float", nullable: false, defaultValue: 999.0),
                    TinhKiem = table.Column<bool>(type: "bit", nullable: false),
                    TinhPhucVu = table.Column<bool>(type: "bit", nullable: false),
                    CaMuoi = table.Column<bool>(type: "bit", nullable: false),
                    NguyenLieu = table.Column<bool>(type: "bit", nullable: false),
                    Ban09 = table.Column<bool>(type: "bit", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    LoaiGui = table.Column<bool>(type: "bit", nullable: false),
                    TruocLangDa = table.Column<bool>(type: "bit", nullable: false),
                    SauLangDa = table.Column<bool>(type: "bit", nullable: false),
                    Nhan = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TinhGio = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    BatCO = table.Column<bool>(type: "bit", nullable: false),
                    IsNhapTay = table.Column<bool>(type: "bit", nullable: false),
                    Createdby = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "default"),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Modifiedby = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamSoBoDinhHinh", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamSoCheDinhHinh_Color",
                columns: table => new
                {
                    ColorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamSoCheDinhHinh_Color_1", x => new { x.ColorCode, x.Ngay, x.MaLo, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamTaiChe",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    _type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamTaiChe", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamXepKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Min = table.Column<double>(type: "float", nullable: false),
                    Max = table.Column<double>(type: "float", nullable: false, defaultValue: 999.0),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamXepKhuon_1", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamXepKhuon_Color",
                columns: table => new
                {
                    ColorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamXepKhuon_Color", x => new { x.ColorCode, x.Ngay, x.MaLo, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamXepKhuonBlock",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamXepKhuonBlock", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamXepKhuonBlock_Color",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNet = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaChatLuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ColorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamXepKhuonKHC_Color", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamXepKhuonKHC",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Min = table.Column<double>(type: "float", nullable: false),
                    Max = table.Column<double>(type: "float", nullable: false, defaultValue: 99.0),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamXepKhuonKHC", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaThongKeCaChet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThongKeCaChet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaThongKeDauAo",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThongKeDauAo", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MaXepHang",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaXepHang", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MayCan",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Par1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Par2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Par3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Par4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Idx = table.Column<int>(type: "int", nullable: false),
                    IsSuDungMauThanhPham = table.Column<bool>(type: "bit", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: ""),
                    MType = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValue: "NONE"),
                    WKv = table.Column<int>(type: "int", nullable: false),
                    AType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MayCan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MayLangDa",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MayLangDa", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MayPhanCo",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MayPhanCo", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MocThoiGianPhanCa",
                columns: table => new
                {
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreateBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ModifyDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MocThoiGianPhanCa", x => new { x.Ngay, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "MPG_CongThuc",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    CreateDate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MPG_CongThuc", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MPG_CongThucChiTiet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCongThuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TrongLuongDaCan = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TyLe = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    IsFull = table.Column<bool>(type: "bit", nullable: false),
                    PhuTroi = table.Column<decimal>(type: "decimal(18,5)", nullable: false, defaultValue: 0.01m),
                    PhuTroiMax = table.Column<decimal>(type: "decimal(18,5)", nullable: false, defaultValue: 0.02m),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsXacNhan = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MPG_CongThucChiTiet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "MPG_CongThucXacNhan",
                columns: table => new
                {
                    MaCongThuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Block = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    IsXacNhan = table.Column<bool>(type: "bit", nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TrongLuongNguyenLieu = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MPD_CongThucXacNhan", x => new { x.MaCongThuc, x.Block, x.Ngay });
                });

            migrationBuilder.CreateTable(
                name: "MPG_PhieuCan",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    MaCongThucChiTiet = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PhuTroi = table.Column<decimal>(type: "decimal(18,5)", nullable: false, defaultValue: 0.01m),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    Block = table.Column<int>(type: "int", nullable: false),
                    TrongLuongCongThuc = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MPG_PhieuCan", x => new { x.STT, x.Ngay, x.MayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "MPG_SanPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MPG_SanPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung_FormMoTrucTiep",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung_FormMoTrucTiep", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung_NhomQuyen",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung_NhomQuyen", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung_Quyen",
                columns: table => new
                {
                    TenNguoiDung = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NhomNguoiDung = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "((0))"),
                    Xem = table.Column<bool>(type: "bit", nullable: false),
                    Them = table.Column<bool>(type: "bit", nullable: false),
                    Sua = table.Column<bool>(type: "bit", nullable: false),
                    Xoa = table.Column<bool>(type: "bit", nullable: false),
                    KetChuyen = table.Column<bool>(type: "bit", nullable: false),
                    CaiDat = table.Column<bool>(type: "bit", nullable: false),
                    ChamGio = table.Column<bool>(type: "bit", nullable: false),
                    Loai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung_Quyen", x => new { x.TenNguoiDung, x.NhomNguoiDung, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung_ThongTin",
                columns: table => new
                {
                    TenNguoiDung = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MatKhau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MoFormTrucTiep = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "frmMain"),
                    FolderName = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false, defaultValue: "PMS"),
                    FileName = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false, defaultValue: "PMS.exe")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung_ThongTin", x => x.TenNguoiDung);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DefaultKey = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, defaultValue: ""),
                    MaNhanVien = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true, comment: "// 1: ĐANG LÀM, 2: TẠM NGHĨ, 3: THÔI VIỆC, 4: THỬ VIỆC"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvatarImg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NguyenLieu_PhuongTien",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaiTrong = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    HienThi = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguyenLieu_PhuongTien", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "NguyenLieu_TyLeNuoc",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaPhuongTien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TyLeNuoc = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguyenLieu_TyLeNuoc_1", x => new { x.STT, x.Ngay, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCapNguyenLieu",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaCungCapNguyenLieu", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCapNguyenLieu_DonGiaVanChuyen",
                columns: table => new
                {
                    MaNhaCC = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayApDung = table.Column<DateTime>(type: "date", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,1)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaCungCapNguyenLieu_DonGiaVanChuyen", x => new { x.MaNhaCC, x.NgayApDung });
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCapNguyenLieu_temp",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaCungCapNguyenLieu_temp", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "NhaMuaPhuPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaMuaPhuPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaChamCong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaHoSo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Xuong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BirthDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeptCode0 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeptName0 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChucVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GenderName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JobPositionName0 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FirstWorkingDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.MaNhanVien);
                });

            migrationBuilder.CreateTable(
                name: "NhanVienCongCu",
                columns: table => new
                {
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TabName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    MaSizeXepKhuonChinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaSizeXepKhuonKXL = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaThanhPhamXepKhuonChinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaThanhPhamXepKhuonKXL = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaChieuXaXepKhuonChinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaKhachHangXepKhuonKXL = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVienChucNang", x => new { x.Ngay, x.MaLo, x.TabName, x.MaNhanVien, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "NhanVienDaiThanh",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaChamCong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaHoSo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Xuong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BirthDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeptCode0 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeptName0 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChucVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GenderName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JobPositionName0 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FirstWorkingDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsShowDinhMuc = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsContracting = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsPhucVu = table.Column<bool>(type: "bit", nullable: false),
                    IsHuman = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsGiaCong = table.Column<bool>(type: "bit", nullable: false),
                    AC = table.Column<int>(type: "int", nullable: false),
                    IsChucNang = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LoaiSanLuong = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "-1 : Trừ ra khỏi tổng lượng| 0: Không cộng không trừ chỉ ghi nhận dữ liệu|1: Cộng vào tổng sản lượng"),
                    IsBanKiem = table.Column<bool>(type: "bit", nullable: false),
                    IsNhanVienCat = table.Column<bool>(type: "bit", nullable: false),
                    IsNhom = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVienDaiThanh", x => x.MaNhanVien);
                });

            migrationBuilder.CreateTable(
                name: "NhanVienDaiThanhCu",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaChamCong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaHoSo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Xuong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BirthDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeptCode0 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeptName0 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChucVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GenderName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JobPositionName0 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FirstWorkingDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVienDaiThanhCu", x => x.MaNhanVien);
                });

            migrationBuilder.CreateTable(
                name: "NhanVienPhucVuTheoBan",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MaBan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    KhuVuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, comment: "FL or SC"),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVienPhucVuTheoBan", x => new { x.MaNhanVien, x.Ngay, x.MaBan, x.MaXuong, x.KhuVuc });
                });

            migrationBuilder.CreateTable(
                name: "NhanVienPhuTheoBanFillet",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaBanCatTiet = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVienPhuTheoBanFillet", x => new { x.MaNhanVien, x.Ngay, x.MaBanCatTiet, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "NhanVienPhuTheoLine",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaLine = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCongViec = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: " ")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVienPhuTheoLine", x => new { x.MaNhanVien, x.Ngay, x.MaLine, x.MaXuong, x.MaCongViec });
                });

            migrationBuilder.CreateTable(
                name: "NhanVienSanLuongTheoLineFillet",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLine = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVienTheoLineFillet", x => new { x.MaNhanVien, x.MaLine, x.MaXuong, x.Ngay });
                });

            migrationBuilder.CreateTable(
                name: "NhanVienTheoBan",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayGio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaBan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhuVuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PCName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    UserName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IsDone = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVienTheoBan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhanVienTheoLine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CodeId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: ""),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLine = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaViTri = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVienTheoLine", x => new { x.Id, x.CodeId });
                });

            migrationBuilder.CreateTable(
                name: "NhanVienTrongBanCatTiet",
                columns: table => new
                {
                    MaBanCatTiet = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVienTrongBanCatTiet", x => new { x.MaBanCatTiet, x.MaNhanVien });
                });

            migrationBuilder.CreateTable(
                name: "NhomKiemSoChe",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaHoSo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomKiemSoChe", x => new { x.Ma, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "NhomLo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    NgayTao = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    GioTao = table.Column<TimeOnly>(type: "time", nullable: false, defaultValueSql: "(getdate())"),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomLo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhomSoCheDinhHinh",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaHoSo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IsNhomChinh = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomSoCheDinhHinh", x => new { x.Ma, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PD_CongThuc",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TrongLuongNguyenLieu = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 1m),
                    MaDonViTinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "KG"),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreateBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PD_CongThuc", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "PD_CongThucChiTiet",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaCongThuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SanLuong = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TyLe = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 1m),
                    BienDo = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0.050m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PC_CongThucChiTiet", x => new { x.STT, x.Ngay, x.MaCongThuc });
                });

            migrationBuilder.CreateTable(
                name: "PD_DonViTinh",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PD_DonViTinh", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "PD_LoaiSanPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PD_LoaiSanPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "PD_LyDo",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DienGiai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PD_LyDo", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "PD_PhieuCan",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuongNguyenLieu = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MaCoi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaCongThuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MaLyDo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreateBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Khoa = table.Column<bool>(type: "bit", nullable: false),
                    Block = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PD_PhieuCan", x => new { x.STT, x.Ngay, x.MaMayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PD_PhieuNhap",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    PCName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SoPhieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreateBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "default"),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "default"),
                    NhapTra = table.Column<bool>(type: "bit", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PD_PhieuNhap", x => new { x.STT, x.Ngay, x.PCName, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PD_PhieuNhapChiTiet",
                columns: table => new
                {
                    SoPhieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    STT = table.Column<int>(type: "int", nullable: false),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SanLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HanSuDung = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgaySanXuat = table.Column<DateOnly>(type: "date", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PD_PhieuNhapChiTiet", x => new { x.SoPhieu, x.STT });
                });

            migrationBuilder.CreateTable(
                name: "PD_PhieuXuat",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    PCName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SoPhieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreateBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PD_PhieuXuat", x => new { x.STT, x.Ngay, x.PCName, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PD_PhieuXuatChiTiet",
                columns: table => new
                {
                    SoPhieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    STT = table.Column<int>(type: "int", nullable: false),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SanLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PD_PhieuXuatChiTiet", x => new { x.SoPhieu, x.STT });
                });

            migrationBuilder.CreateTable(
                name: "PD_SanPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaLoai = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaDonVi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PD_SanPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "PhanBoNhanVienTheoCongViecPhuFillet",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCongViecPhuFillet = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TyLeHuong = table.Column<double>(type: "float", nullable: false, defaultValue: 1.0),
                    TyLeTru = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanBoNhanVienTheoCongViecPhuFillet", x => new { x.MaNhanVien, x.MaCongViecPhuFillet, x.Ngay, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanBTPDinhHinh",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "DT"),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaUserCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "admin"),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaMayLangDa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CaTra = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChiSanLuong = table.Column<bool>(type: "bit", nullable: false),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TrongLuongBu = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    IsOffline = table.Column<bool>(type: "bit", nullable: false),
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo_PhieuCanBTPDinhHinh_1", x => new { x.STT, x.Ngay, x.MaMayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanBTPDinhHinh2",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    MaUserCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    MaMau = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    MaSize = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaMayLangDa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<double>(type: "float", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CaTra = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo_PhieuCanBTPDinhHinh_3", x => new { x.STT, x.Ngay, x.MaMayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanBTPFillet",
                columns: table => new
                {
                    MaMayTinhCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaUserCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThoiGianCan = table.Column<TimeOnly>(type: "time", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaXuongSanXuat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MSL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaLoaiCa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaLoaiThanhPham = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaSize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaMau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MayPhanCo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BanFillet = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanBTPFillet", x => new { x.MaMayTinhCan, x.MaUserCan, x.ThoiGianCan, x.Ngay });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanBTPFilletv2",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "DT"),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaUserCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "admin"),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaMayLangDa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CaTra = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaNhanVienPhucVu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Id = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo_PhieuCanBTPFilletv2_1", x => new { x.STT, x.Ngay, x.MaMayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanCaChet",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhachHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThongKe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaAo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TrongLuongBinhQuan = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanCaChet", x => new { x.STT, x.Ngay, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanCaChetDaiThanhSide",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    BiosId = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenLoaiCa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TenKhachHang = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TenThongKe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TenAo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TrongLuongBinhQuan = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    GioTai = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    NgayTai = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanCaChetDaiThanhSide", x => new { x.STT, x.Ngay, x.MaMayCan, x.BiosId });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanCaGiongVungNuoi",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaGhe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaAo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaChuAo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCongDoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThongKeDauAo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanCaGiongVungNuoi", x => new { x.STT, x.Ngay, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanCaGiongVungNuoiDaiThanhSide",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    BiosId = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaLoaiCaDaiThanhId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaGhe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TenAo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TenChuAo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TenCongDoan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TenThongKeDauAo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GioTai = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    NgayTai = table.Column<DateTime>(type: "date", nullable: false),
                    TenLoaiCa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, defaultValue: ".")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanCaGiongVungNuoiDaiThanhSide", x => new { x.STT, x.Ngay, x.MaMayCan, x.BiosId });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanCaoThit",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaQuyCach = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrongLuongSec = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DinhMucThucTe = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    DinhMucYeuCau = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanCaoThit", x => new { x.STT, x.Ngay, x.MaMayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanChinhXepKhuon",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayNguyenLieu = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaCoiTam = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCoiChinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    DaQuay = table.Column<bool>(type: "bit", nullable: false),
                    ThoiGianBatDauQuay = table.Column<TimeSpan>(type: "time(7)", nullable: true),
                    ThoiGianRaCoi = table.Column<TimeSpan>(type: "time(7)", nullable: true),
                    Forced = table.Column<bool>(type: "bit", nullable: false),
                    ThoiGianQuay = table.Column<int>(type: "int", nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSizeChinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaChatLuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaThanhPhamChinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhuVuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, defaultValueSql: "((0))"),
                    MaNhom = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaChieuXa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "000"),
                    TaiChe = table.Column<bool>(type: "bit", nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LuotQuay = table.Column<int>(type: "int", nullable: false),
                    ChuyenXuong = table.Column<bool>(type: "bit", nullable: false),
                    MaNhanVienPvPhanCo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    NgayRaCoi = table.Column<DateTime>(type: "date", nullable: true),
                    NgayBatDauQuay = table.Column<DateTime>(type: "date", nullable: true),
                    MayQuay = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IdMonitor = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanChinhXepKhuon", x => new { x.STT, x.MaXuong, x.MaMayCan, x.NgayNguyenLieu });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanChinhXepKhuon2",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    MaCoiTam = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCoiChinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    DaQuay = table.Column<bool>(type: "bit", nullable: false),
                    ThoiGianBatDauQuay = table.Column<TimeOnly>(type: "time", nullable: true),
                    ThoiGianRaCoi = table.Column<TimeOnly>(type: "time", nullable: true),
                    Forced = table.Column<bool>(type: "bit", nullable: false),
                    ThoiGianQuay = table.Column<int>(type: "int", nullable: false),
                    TrongLuong = table.Column<double>(type: "float", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    MaSizeChinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    MaChatLuong = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: true),
                    MaThanhPhamChinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhuVuc = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNhom = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaChieuXa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "000"),
                    TaiChe = table.Column<bool>(type: "bit", nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaNhanVienPvPhanCo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanChinhXepKhuon2", x => new { x.STT, x.Ngay, x.MaXuong, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanDinhHinh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    CaLamViec = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaNhanVien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    MaSanPham = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: ""),
                    TenSanPham = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: ""),
                    TrongLuongNhan = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TrongLuongTra = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DinhMucThucTe = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DinhMucYeuCau = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DanhGia = table.Column<bool>(type: "bit", nullable: true),
                    SoRo = table.Column<int>(type: "int", nullable: true),
                    _Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo_PhieuCanDinhHinh", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanLangDa",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "default"),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanLangDa", x => new { x.STT, x.Ngay, x.MayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanNguyenLieu",
                columns: table => new
                {
                    MaMayTinhCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaUserCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThoiGianCan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: true),
                    NhaCC = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MSL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaAo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaPhuongTien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLoaiThanhPham = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "((0))"),
                    MaMau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "((0))"),
                    MaBanCatTiet = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "((0))"),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaXuongSanXuat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TyLeNuoc = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TrongLuongOrg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Pheu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "((0))"),
                    Chuyen = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanNguyenLieu", x => new { x.MaMayTinhCan, x.MaUserCan, x.ThoiGianCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanPhaLoc",
                columns: table => new
                {
                    MaMayTinhCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaUserCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThoiGianCan = table.Column<TimeOnly>(type: "time", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaXuongSanXuat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MSL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaLoaiCa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaLoaiThanhPham = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaSize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaMau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaNhanVien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HoVaTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaTheTu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: ""),
                    MaNhanVienPhucVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LoaiCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "TP", comment: "TP & BTP"),
                    InOut = table.Column<bool>(type: "bit", nullable: false, comment: "0 nhap, 1 xuat"),
                    ItemCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo_PhieuCanPhaLoc", x => new { x.MaMayTinhCan, x.MaUserCan, x.ThoiGianCan, x.Ngay });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanPhuPham",
                columns: table => new
                {
                    MaMayTinhCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaUserCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThoiGianCan = table.Column<DateTime>(type: "datetime", nullable: false),
                    NgayCan = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Ngay = table.Column<DateTime>(type: "date", nullable: true),
                    NhaMuaHang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MSL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaPhuongTien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaLoaiCa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaLoaiThanhPham = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaSize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValueSql: "((0))"),
                    MaMau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValueSql: "((0))"),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaXuongSanXuat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanPhuPham_1", x => new { x.MaMayTinhCan, x.MaUserCan, x.ThoiGianCan, x.NgayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanPhuPhamv2",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanPhuPhamv2", x => new { x.STT, x.Ngay, x.MaXuong, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanPhuXepKhuon",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhuVuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNhom = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanPhuXepKhuon", x => new { x.STT, x.Ngay, x.MaXuong, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanPhuXepKhuon2",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    MaKhuVuc = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNhom = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<double>(type: "float", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanPhuXepKhuon2", x => new { x.STT, x.Ngay, x.MaXuong, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanRaCoi",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    IdMonitor = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayNguyenLieu = table.Column<DateTime>(type: "date", nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaChieuXa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaChatLuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCoi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STT = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanRaCoi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanRaDong",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Id = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaChatLuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanRaDong", x => new { x.STT, x.Ngay, x.MaXuong, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanRaDong2",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Id = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaChatLuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanRaDong2", x => new { x.STT, x.Ngay, x.MaXuong, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanSauXepKhuon",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    NgayNguyenLieu = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaChieuXa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCoi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ChiTietLuotRaCoiId = table.Column<long>(type: "bigint", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TyLeMaBang = table.Column<int>(type: "int", nullable: false),
                    IsTam = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanSauXepKhuon", x => new { x.STT, x.NgayNguyenLieu, x.MaXuong, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanSoCheDinhHinh",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaMayLangDa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "test"),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "test")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanSoCheDinhHinh", x => new { x.STT, x.Ngay, x.MaXuong, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanSoCheDinhHinh2",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<double>(type: "float", nullable: false),
                    MaMayLangDa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "test"),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanSoCheDinhHinh2", x => new { x.STT, x.Ngay, x.MaXuong, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanTaiChe",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    Id = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaChatLuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCongViec = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanSoChe", x => new { x.STT, x.Ngay, x.MaMayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanTaiChe2",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    Id = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaChatLuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<double>(type: "float", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanSoChe2", x => new { x.STT, x.Ngay, x.MaMayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanTPDinhHinh",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "DT"),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaUserCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "admin"),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrongLuongNhan = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TrongLuongTra = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DinhMucThucTe = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DinhMucYeuCau = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CaTra = table.Column<bool>(type: "bit", nullable: false),
                    STTBTP = table.Column<int>(type: "int", nullable: true),
                    MaMayCanBTP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChiSanLuong = table.Column<bool>(type: "bit", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TrongLuongBu = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    IsOffline = table.Column<bool>(type: "bit", nullable: false),
                    MaNhanVienPhucVu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNhanVienBanKiem = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IdIn = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo_PhieuCanTPDinhHinh", x => new { x.STT, x.Ngay, x.MaMayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanTPDinhHinh2",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    MaUserCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    MaMau = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    MaSize = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrongLuongNhan = table.Column<double>(type: "float", nullable: false),
                    TrongLuongTra = table.Column<double>(type: "float", nullable: false),
                    DinhMucThucTe = table.Column<double>(type: "float", nullable: false),
                    DinhMucYeuCau = table.Column<double>(type: "float", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CaTra = table.Column<bool>(type: "bit", nullable: false),
                    STTBTP = table.Column<int>(type: "int", nullable: true),
                    MaMayCanBTP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo_PhieuCanTPDinhHinh2", x => new { x.STT, x.Ngay, x.MaMayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanTPFillet",
                columns: table => new
                {
                    MaMayTinhCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaUserCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThoiGianCan = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaXuongSanXuat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MSL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaLoaiCa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaLoaiThanhPham = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaSize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaMau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaNhanVien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HoVaTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaTheTu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: ""),
                    MaNhanVienPhucVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LoaiCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "TP", comment: "TP & BTP"),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo_PhieuCanTPFillet", x => new { x.MaMayTinhCan, x.MaUserCan, x.ThoiGianCan, x.Ngay });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanTPFilletv2",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "DT"),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaUserCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "admin"),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrongLuongNhan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrongLuongTra = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DinhMucThucTe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DinhMucYeuCau = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CaTra = table.Column<bool>(type: "bit", nullable: false),
                    STTBTP = table.Column<int>(type: "int", nullable: true),
                    MaMayCanBTP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaNhanVienPhucVu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaBan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ThePhieuSanLuongId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Id = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    IdIn = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo_PhieuCanTPFilletv2", x => new { x.STT, x.Ngay, x.MaMayCan, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanVungNuoi",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayNhapXuong = table.Column<DateOnly>(type: "date", nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaGhe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaAo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCongDoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThongKeDauAo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TrongLuong = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanVungNuoi", x => new { x.STT, x.Ngay, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanVungNuoiDaiThanhSide",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    BiosId = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    NgayTai = table.Column<DateTime>(type: "date", nullable: false),
                    GioTai = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaLoaiCaDaiThanhId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "N\".\""),
                    TenLoaiCa = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false, defaultValue: "N\".\""),
                    CanLai = table.Column<bool>(type: "bit", nullable: false),
                    MaGhe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TenGhe = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: "N\".\""),
                    TenAo = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    TenCongDoan = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    TenThongKeDauAo = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TrongLuong = table.Column<double>(type: "float", nullable: false),
                    IsTap = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanVungNuoiDaiThanhSide", x => new { x.STT, x.Ngay, x.MaMayCan, x.BiosId });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanXepKhuonBlock",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaChatLuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNet = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaChieuXa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhachHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCongDoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanXepKhuonBlock", x => new { x.STT, x.Ngay, x.MaXuong, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanXepKhuonBlock2",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaChatLuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNet = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaChieuXa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhachHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCongDoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanXepKhuonBlock2", x => new { x.STT, x.Ngay, x.MaXuong, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanXepKhuonKHC",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhachHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCoiTam = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DaXacNhan = table.Column<bool>(type: "bit", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Block = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanXepKhuonKHC", x => new { x.STT, x.Ngay, x.MaXuong, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanXepKhuonKHC2",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhachHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCoiTam = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<double>(type: "float", nullable: false),
                    DaXacNhan = table.Column<bool>(type: "bit", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Block = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuCanXepKhuonKHC2", x => new { x.STT, x.Ngay, x.MaXuong, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhieuSanLuongRaCoiTinhLuongXepKhuon",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaUserCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    NgayThem = table.Column<DateTime>(type: "date", nullable: false),
                    GioRaCoi = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaCoi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    LuotRaCoi = table.Column<int>(type: "int", nullable: false),
                    MaNhom = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuSanLuongRaCoiTinhLuongXepKhuon", x => new { x.STT, x.Ngay, x.MaXuong, x.MaMayCan });
                });

            migrationBuilder.CreateTable(
                name: "PhuongTien_TaiTrong",
                columns: table => new
                {
                    MaPhuongTien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayApDung = table.Column<DateTime>(type: "date", nullable: false),
                    TaiTrong = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongTien_TaiTrong", x => new { x.MaPhuongTien, x.NgayApDung });
                });

            migrationBuilder.CreateTable(
                name: "PhuongTien_TrongLuongDauAo",
                columns: table => new
                {
                    MaPhuongTien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaAo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaNhaCungCap = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "."),
                    Chuyen = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CaManh = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CaNgopAoGhe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CaNgopAoXe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongHam = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CaNgayTruoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CaConLai = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThuKy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: ""),
                    ApTai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: ""),
                    STTChuyen = table.Column<int>(type: "int", nullable: false, defaultValueSql: "(N'1')"),
                    TyLeMoi = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GioXuatPhat = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    NgayXuatPhat = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    NgayBatCa = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    CreateDateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreateBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: "default"),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: "default"),
                    IsVungNuoiBlocked = table.Column<bool>(type: "bit", nullable: false),
                    CaNgopAoBanNgoai = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongTien_TrongLuongDauAo", x => new { x.MaPhuongTien, x.Ngay, x.MaAo, x.MaNhaCungCap, x.Chuyen });
                });

            migrationBuilder.CreateTable(
                name: "PhuongTienChoNguyenLieu",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsHD = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsGhe = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    VungNuoiId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "."),
                    SoGhe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongTienChoNguyenLieu", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "PhuongTienChoNguyenLieu_temp",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    IsHD = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsGhe = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    VungNuoiId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongTienChoNguyenLieu_temp", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "PhuongTienChoPhuPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongTienChoPhuPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "PhuongTienKhongSuDung",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongTienKhongSuDung", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "PLC",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PLC", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PLCChiTiet",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCoi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PLCId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    RUN = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    RUNOUT = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    STOP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TimeQuay = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    HzQuay = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    HzRa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TimeQuayDef = table.Column<int>(type: "int", nullable: false, defaultValue: 300),
                    HzQuayDef = table.Column<int>(type: "int", nullable: false, defaultValue: 50),
                    HzRaDef = table.Column<int>(type: "int", nullable: false, defaultValue: 20),
                    INVERTER = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    RUNSTATUS = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PAUSE = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PLCChiTiet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PMS_L",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SCode = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Par1 = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Par2 = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Par3 = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Par4 = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMS_L", x => x.STT);
                });

            migrationBuilder.CreateTable(
                name: "QuickSearchMenu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Path = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuickSearchMenu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermistion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    Fu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Func = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, comment: "-1 không sử dụng, 1 sử dụng")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermistion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__3214EC074553D291", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RP_MaThanhPhamDinhHinh",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    LangDa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DinhMucLangDa = table.Column<decimal>(type: "decimal(18,3)", nullable: false, defaultValue: 1.075m),
                    MaNhom = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RP_MaThanhPhamDinhHinh", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "RP_MaThanhPhamFillet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    DinhMuc = table.Column<decimal>(type: "decimal(18,3)", nullable: false, defaultValue: 1.96m),
                    DinhMucLangDa = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MaNhom = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RP_MaThanhPhamFillet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "RP_MaThanhPhamNL",
                columns: table => new
                {
                    MaThanhPhamNL = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RP_MaThanhPhamNL", x => x.MaThanhPhamNL);
                });

            migrationBuilder.CreateTable(
                name: "RP_MaThanhPhamNL_Fillet",
                columns: table => new
                {
                    MaThanhPhamFillet = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPhamNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    DinhMuc = table.Column<decimal>(type: "decimal(18,3)", nullable: false, defaultValue: 1m),
                    DinhMucCatTiet = table.Column<decimal>(type: "decimal(18,3)", nullable: false, defaultValue: 1m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RP_MaThanhPhamNL_Fillet", x => x.MaThanhPhamFillet);
                });

            migrationBuilder.CreateTable(
                name: "RP_MaThanhPhamNL_SoChe",
                columns: table => new
                {
                    MaThanhPhamSoChe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPhamNL = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    DinhMucCatTiet = table.Column<decimal>(type: "decimal(18,3)", nullable: false, defaultValue: 1m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RP_MaThanhPhamNL_TaiChe", x => x.MaThanhPhamSoChe);
                });

            migrationBuilder.CreateTable(
                name: "RP_NangXuatDinhMuc",
                columns: table => new
                {
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ThanhPham = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Size = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CaTra = table.Column<bool>(type: "bit", nullable: false),
                    DinhMucGio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NangXuatGio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TLNhan = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TLTra = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DinhMuc = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RP_NangXuatDinhMuc", x => new { x.MaXuong, x.MaLo, x.Ngay, x.Gio, x.MaNhanVien, x.ThanhPham, x.Size, x.CaTra });
                });

            migrationBuilder.CreateTable(
                name: "RP_NhomThanhPham_DinhHinh_Fillet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RP_NhomThanhPham_DinhHinh_Fillet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "RP_SanLuongDenThoiDiem",
                columns: table => new
                {
                    KhuVuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    ThanhPham = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DinhMucLangDa = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    MaNhom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TongTrognLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxTongTrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ThanhPhamId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Tyle = table.Column<decimal>(type: "decimal(18,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RP_SanLuongDenThoiDiem", x => new { x.KhuVuc, x.MaLo, x.MaXuong, x.Ngay, x.ThanhPham, x.Size });
                });

            migrationBuilder.CreateTable(
                name: "RP_SanLuongNguyenLieuTrenBanNgay",
                columns: table => new
                {
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    KhuVuc = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    GioR = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TongTrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RP_SanLuongNguyenLieuTrenBanNgay", x => new { x.Ngay, x.KhuVuc, x.GioR, x.MaLo, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "SettingDashboard",
                columns: table => new
                {
                    NumberDate = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingDashboard_1", x => x.NumberDate);
                });

            migrationBuilder.CreateTable(
                name: "Sheet1$",
                columns: table => new
                {
                    Tênbảngchẻcở = table.Column<string>(name: "Tên bảng chẻ cở", type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Sốconvỏxô = table.Column<double>(name: "Số con vỏ xô", type: "float", nullable: true),
                    Quytrình = table.Column<string>(name: "Quy trình", type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Sizetínhlương = table.Column<string>(name: "Size tính lương", type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Côngđoạntínhlương = table.Column<string>(name: "Công đoạn tính lương", type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_BaoBi",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "T_BieuMauGiao_Import",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    KhangSinh = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ThongTinPhuGia = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CoVoXo = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    MapppingSize_HLSO = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TrongLuongGiao = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    SoLuongBo_Phi = table.Column<int>(type: "int", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: true),
                    NguoiGiao = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DonViNhan = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    NguoiNhan = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    BoPhan = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    NguoiVanChuyen = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TaiXe_BienSoXe = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    PCName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    NgayImport = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    MaBang = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    NgayLap = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoSeal = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DonVi = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    NgayNguyenLieu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    HinhThucCan = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    LoaiTom = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ChungNhan = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SizeTPBaoBi = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    QTMatHang = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    LoaiQuyTrinh = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    GhiChuPhuGia = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_BieuMauGiao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_Bon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsKhoa = table.Column<bool>(type: "bit", nullable: false),
                    IsDangRa = table.Column<bool>(type: "bit", nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false, defaultValue: 100m),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: ""),
                    TrongLuongThongBao = table.Column<decimal>(type: "decimal(18,3)", nullable: false, defaultValue: 10m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Bon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_ChiTietBon",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaBon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Luot = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    GioBatDau = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    NgayKetThuc = table.Column<DateTime>(type: "date", nullable: true),
                    GioKetThuc = table.Column<TimeSpan>(type: "time(7)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaHoanThanh = table.Column<bool>(type: "bit", nullable: false),
                    TrongLuongTP = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TrongLuongConLai = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    DaChuyen = table.Column<bool>(type: "bit", nullable: false),
                    MaBonGoc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ChiTietBon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_ChiTietVoXo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    MaPhieuPhanCo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VoXo = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MaLoaiNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, comment: "Nhóm Conong Việc"),
                    MaCongDoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true, comment: "Công việc"),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaQuyTrinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    NgayGio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    PCName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MaSizeVoXo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    VoXo2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ChiTietVoXo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_CongDoan",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MaLuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaLoaiNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Idx = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CongDoan", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_CongDoanTheoThanhPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaCongDoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Idx = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CongDoanTheoThanhPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_DinhMuc",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false, defaultValueSql: "(getdate())"),
                    MaLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLoaiNguyenLieu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CaTra = table.Column<bool>(type: "bit", nullable: false),
                    MaKhuVuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    DinhMuc = table.Column<double>(type: "float", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_DinhMuc", x => new { x.STT, x.Ngay, x.Gio, x.MaLo, x.MaLoaiNguyenLieu, x.MaSize, x.MaThanhPham, x.MaXuong, x.CaTra, x.MaKhuVuc });
                });

            migrationBuilder.CreateTable(
                name: "T_DonHang",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhachHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IsKetThuc = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_DonHang", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_Goup",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaKhuVuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Goup", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_HoaChat",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DienGiai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_HoaChat", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_KhachHang",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_KhachHang", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_KhangSinh",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_KhangSinh", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_KhuVuc",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_KhuVuc", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_LenhSanXuat",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoanThanh = table.Column<bool>(type: "bit", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_LenhSanXuat", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_LoaiCan",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_LoaiCan", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_LoaiKhuon",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaKhuVuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_LoaiKhuon", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_LoaiNguyenLieu",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaKhuVuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_LoaiNguyenLieu", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_LoNguyenLieu",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaNhaCungCap = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNhomLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNL = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_LoNguyenLieu", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_MuaNguyenLieu_Import",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CongTy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Zone_XN = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    HinhThucCan = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    LoaiTom = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    NgayNguyenLieu = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaDaiLy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SoLo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    GopLo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TenLai = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    NhomKS = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    MaLoaiNguyenLieu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    MaSizeCo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TenSizeCo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SoCon_lb = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    MaNgayNguyenLieu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Con_lb = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SoRo = table.Column<int>(type: "int", nullable: true),
                    KG_Loai1 = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    KG_Loai2 = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    KG_Dat = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    PCName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    NgayImport = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_MuaNguyenLieu_Import", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_NhaCungCap",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SoDienThoai = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_NhaCungCap", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_NhomHoaChat",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_NhomHoaChat", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_NhomLo",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NgayBatDau = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    NgayKetThuc = table.Column<DateTime>(type: "date", nullable: true),
                    NgayNguyenLieu = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    DaKetThuc = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_NhomLo", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_Phieu",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    So = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    XiNghiep = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    NgayGio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    TuNgayGio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    DenNgayGio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    MaCongDoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UserName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "default"),
                    PCName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "default")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Phieu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_PhieuCan",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false, comment: "=yyyyMMdd.xinghiepId.PCName(no space and unicode).STT"),
                    STT = table.Column<int>(type: "int", nullable: false),
                    MayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    NgayNguyenLieu = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    MaLoaiNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNhomLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaGroup = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "((0))"),
                    MaLoaiKhuon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaQuyCach = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaLoaiCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "TP"),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TrongLuongNhan = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DinhMuc = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaNhaCungCap = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaPhuongTien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaKhuVuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLenhSanXuat = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVienPhucVu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaLoaiCongViec = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "T", comment: "T: Tay, M: Máy"),
                    MaNhanVien2 = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaCongDoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaBon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Luot = table.Column<int>(type: "int", nullable: false),
                    MaQuyTrinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaDonHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaKhachHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IsDatKhangSinh = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MaKhangSinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    VoXo = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MaThongTinPhu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaPhuGia = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaTrangThaiNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaQuyTrinhOrg = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IsNgam = table.Column<bool>(type: "bit", nullable: false),
                    T = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MaTyLeNhomHoaChat = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IsCanTay = table.Column<bool>(type: "bit", nullable: false),
                    SoLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false, defaultValue: 1m),
                    TrongLuongDonVi = table.Column<decimal>(type: "decimal(18,3)", nullable: false, defaultValue: 1m),
                    MaKhachHangOrg = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaSizeOrg = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaPhuGiaOrg = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNhomHoaChat = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaSizeTP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaPhieuPhanCo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    GhiChu2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VoXo2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PhieuCan", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_PhieuCan_Server",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    STT = table.Column<int>(type: "int", nullable: true),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: true),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: true),
                    MaNhanVien = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    MaHoSo = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    TenNhanVien = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Nhom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MaLo = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    NgayNguyenLieu = table.Column<DateOnly>(type: "date", nullable: true),
                    SanPham = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    QuyTrinh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DatKhangSinh = table.Column<bool>(type: "bit", nullable: true),
                    Size = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SizeTP = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CongViec = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    VoXo = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    VoXoGiaoDong = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    MaKhachHang = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    KhangSinh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ThongTinNguyenLieu = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    PhanCo = table.Column<bool>(type: "bit", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: true),
                    TrongLuongDonVi = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    CanTay = table.Column<bool>(type: "bit", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KhachHang = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhuGia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NhomHoaChat = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    QuyCach = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ThongTinPhu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GhiChu2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GhiChu3 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PhieuCan_Server", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_PhieuCanThuMua",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    MaLoaiNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaTieuChuan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaBaoBi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaQuyTrinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaPhuGia = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhachHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhangSinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThongTinPhu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    VoXoTB = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    VoXoCD = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    VoXoCT = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TyLe = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GramCuoi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GramDau = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NhuCau = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayGioTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    MaUserCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MayCan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false, defaultValueSql: "(getdate())"),
                    MaPhieuYeuCau = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "((0))"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    NgayNguyenLieu = table.Column<DateOnly>(type: "date", nullable: true),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNhomLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaCongDoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PhieuCanThuMua", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_PhieuCanThuMua_Server",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    STT = table.Column<int>(type: "int", nullable: true),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: true),
                    NgayNguyenLieu = table.Column<DateOnly>(type: "date", nullable: true),
                    MaHoSo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TenNhanVien = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Nhom = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Xuong = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LoaiNguyenLieu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TieuChuan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Size = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BaoBi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SanPham = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    QuyTrinh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhuGia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MaKhachHang = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    KhachHang = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    KhangSinh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ThongTinPhu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CongDoan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VoXoTB = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    VoXoCD = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    VoXoCT = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PhieuCanThuMua_Server", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_PhieuNhomYeuCau",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    LoaiPhieuNhomYeuCau = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PhieuNhomYeuCau", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_PhieuPhanCo",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    LoaiPhieuPhanCo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    STT = table.Column<int>(type: "int", nullable: false),
                    MaNhomYeuCau = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PhieuPhanCo", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_PhieuPhanCoChiTiet",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    MaPhieuPhanCo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaQuyTrinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaPhuGia = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaKhachHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaKhangSinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    VoXo = table.Column<decimal>(type: "decimal(18,3)", nullable: true, defaultValue: 0m),
                    MaThongTinPhu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaTrangThaiNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TyLe = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    GramCuoi = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    GramDau = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaCongDoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    VoXo2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TrongLuong2 = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PhieuPhanCoChiTiet", x => new { x.STT, x.MaPhieuPhanCo });
                });

            migrationBuilder.CreateTable(
                name: "T_PhieuYeuCau",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaTieuChuan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaBaoBi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaQuyTrinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaPhuGia = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhachHang = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaKhangSinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThongTinPhu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    VoXoTB = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    VoXoCD = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    VoXoCT = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TyLe = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GramCuoi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GramDau = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NhuCau = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    NgayGioTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    NguoiTao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PCName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    STT = table.Column<int>(type: "int", nullable: false),
                    MaNhomYeuCau = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    MaCongDoan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PhieuYeuCau", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_PhuGia",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuGia", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_PhuongTien",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaSo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PhuongTien", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_QuyCach",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaKhuVuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_QuyCach", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_QuyTrinh",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsNguyenLieu = table.Column<bool>(type: "bit", nullable: false),
                    LoaiQuyTrinh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MaSizeVoXo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuyTrinh", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_QuyTrinhTheoNgay",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MaQuyTrinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TheId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_QuyTrinhTheoNgay", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_SanPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MaLoaiNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MaNL = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_SanPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_Size",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaKhuVuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Inx = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Size", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_ThanhPham",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaKhuVuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsPhanCo = table.Column<bool>(type: "bit", nullable: false),
                    IsDem = table.Column<bool>(type: "bit", nullable: false),
                    ChoPhepChuyenDoiQuyTrinh = table.Column<bool>(type: "bit", nullable: false),
                    IsHoaChat = table.Column<bool>(type: "bit", nullable: false),
                    IsThemLo = table.Column<bool>(type: "bit", nullable: false),
                    IsBatMau = table.Column<bool>(type: "bit", nullable: false),
                    DinhMuc = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 1m),
                    IsHoaChatMain = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ThanhPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_ThongTinPhu",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsPhanCo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsBatMau = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongTinPhu", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_TieuChuan",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TieuChuan", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_TrangThaiNguyenLieu",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TrangThaiNguyenLieu", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "T_TyLeHoaChatTheoNhom",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhomHoaChat = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaHoaChat = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TyLe = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: "Chua Ðăt Tên")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TyLeHoaChatTheoNhom", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "TableInfo",
                columns: table => new
                {
                    TableName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RecordCount = table.Column<long>(type: "bigint", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    RecentTime = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    TimeElapsed = table.Column<double>(type: "float", nullable: false),
                    SizeMB = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableInfo", x => x.TableName);
                });

            migrationBuilder.CreateTable(
                name: "Tare",
                columns: table => new
                {
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TyLeBu = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tare", x => x.TrongLuong);
                });

            migrationBuilder.CreateTable(
                name: "ThePhieuSanLuongFillet",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CaTra = table.Column<bool>(type: "bit", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    STT_PC = table.Column<int>(type: "int", nullable: false),
                    MayCan_PC = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaBan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    IsDone = table.Column<bool>(type: "bit", nullable: false),
                    STT_PC_BTP = table.Column<int>(type: "int", nullable: false),
                    MayCan_PC_BTP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuongNhan = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdIn = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThePhieuSanLuongFillet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TheRo",
                columns: table => new
                {
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ColorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "#0000FF"),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    IsRach = table.Column<bool>(type: "bit", nullable: false),
                    MaThanhPhamDinhHinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheRo", x => x.MaThe);
                });

            migrationBuilder.CreateTable(
                name: "TheThanhPham",
                columns: table => new
                {
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaSizeDinhHinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MaMayLangDa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaThanhPhamFillet = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaThanhPhamPhuPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaSizeFillet = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IsZero = table.Column<bool>(type: "bit", nullable: true),
                    MaLoaiNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IsRestart = table.Column<bool>(type: "bit", nullable: true),
                    MaQuyTrinhT = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    STTPhieuPhanCoChiTietT = table.Column<int>(type: "int", nullable: false),
                    MaCoiXepKhuon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheThanhPham", x => x.MaThe);
                });

            migrationBuilder.CreateTable(
                name: "TheTu",
                columns: table => new
                {
                    MaTheTu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayGio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    PCName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: ".")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheTu", x => x.MaTheTu);
                });

            migrationBuilder.CreateTable(
                name: "TheTuDaiThanh",
                columns: table => new
                {
                    MaTheTu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheTuDaiThanh", x => x.MaTheTu);
                });

            migrationBuilder.CreateTable(
                name: "TheTuNhomXepKhuon",
                columns: table => new
                {
                    MaTheTu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhom = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheTuNhomXepKhuon", x => x.MaTheTu);
                });

            migrationBuilder.CreateTable(
                name: "Thit_LoaiNguyenLieuPhaLoc",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsMang = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thit_LoaiNguyenLieu", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "Thit_MaThanhPhamPhaLoc",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaLoaiNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsMang = table.Column<bool>(type: "bit", nullable: false),
                    IsThit = table.Column<bool>(type: "bit", nullable: false),
                    IsXuong = table.Column<bool>(type: "bit", nullable: false),
                    IsMo = table.Column<bool>(type: "bit", nullable: false),
                    IsChanGio = table.Column<bool>(type: "bit", nullable: false),
                    CodeId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thit_MaThanhPham", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "Thit_PhieuCanPhaLoc",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    MaMayCan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false),
                    MaLoaiNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    MaNhanVienPhucVu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    InOut = table.Column<bool>(type: "bit", nullable: false, comment: "0 nhap, 1 xuat"),
                    ItemCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IsLoi = table.Column<bool>(type: "bit", nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsNL = table.Column<bool>(type: "bit", nullable: false),
                    IsSX = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thit_PhieuCanPhaLoc", x => new { x.STT, x.MaMayCan, x.Ngay, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "Thit_SizePhaLoc",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thit_SizePhaLoc", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "ToKiem",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaHoSo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToKiem_1", x => new { x.Ma, x.MaXuong });
                });

            migrationBuilder.CreateTable(
                name: "TrangThaiNhanVienTamThoi",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    KhuVuc = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "FL"),
                    Gio = table.Column<TimeSpan>(type: "time(7)", nullable: false, defaultValueSql: "(getdate())"),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    IsPhucVu = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrangThaiNhanVienTamThoi", x => new { x.STT, x.Ngay, x.MaXuong, x.KhuVuc });
                });

            migrationBuilder.CreateTable(
                name: "TrongLuongBinhQuanCaChet",
                columns: table => new
                {
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    MaAo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuongBinhQuan = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    CreateBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "default")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrongLuongBinhQuanCaChet", x => new { x.Ngay, x.MaAo });
                });

            migrationBuilder.CreateTable(
                name: "TrongLuongGioiHanDinhHinh",
                columns: table => new
                {
                    STT = table.Column<int>(type: "int", nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Gio = table.Column<TimeOnly>(type: "time", nullable: false),
                    TrongLuong = table.Column<double>(type: "float", nullable: false),
                    BienDoGiaoDong = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrongLuongGioiHanDinhHinh", x => new { x.STT, x.Ngay, x.MaLo });
                });

            migrationBuilder.CreateTable(
                name: "Update",
                columns: table => new
                {
                    TenFile = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    NgayUpdate = table.Column<DateTime>(type: "datetime", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TapTin = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CheckMD5 = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Update", x => x.TenFile);
                });

            migrationBuilder.CreateTable(
                name: "UpdateSuaCa",
                columns: table => new
                {
                    TenFile = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    TenFolder = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    ComputerName = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    MD5Code = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdateSuaCa_1", x => new { x.TenFile, x.TenFolder, x.ComputerName });
                });

            migrationBuilder.CreateTable(
                name: "UserArea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WKv = table.Column<int>(type: "int", nullable: false),
                    NgayGio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserId", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViTriFillet",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MacDinh = table.Column<bool>(type: "bit", nullable: false),
                    CodeId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViTriFillet", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "XiNghiep",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CodeId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XiNghiep", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JwtId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiredAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK__RefreshTo__UserI__3C15C135",
                        column: x => x.UserId,
                        principalTable: "NguoiDung",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserRole__3214EC0761A77614", x => x.Id);
                    table.ForeignKey(
                        name: "FK__UserRole__RoleId__459F2B6F",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__UserRole__UserId__44AB0736",
                        column: x => x.UserId,
                        principalTable: "NguoiDung",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                table: "UserRole",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "API_User");

            migrationBuilder.DropTable(
                name: "BanCatTiet");

            migrationBuilder.DropTable(
                name: "BanCatTiet_temp");

            migrationBuilder.DropTable(
                name: "BanCatTietTheoLine");

            migrationBuilder.DropTable(
                name: "BanFillet");

            migrationBuilder.DropTable(
                name: "BanQuyenPhanMem");

            migrationBuilder.DropTable(
                name: "BieuMau_Navico");

            migrationBuilder.DropTable(
                name: "BoPhan");

            migrationBuilder.DropTable(
                name: "BoTriLoSizeThanhPham");

            migrationBuilder.DropTable(
                name: "BoTriNhanVienTheoSize");

            migrationBuilder.DropTable(
                name: "BoTriNhomKiemSoChe");

            migrationBuilder.DropTable(
                name: "BoTriNhomSoChe");

            migrationBuilder.DropTable(
                name: "BoTriTinhLuonXepKhuon");

            migrationBuilder.DropTable(
                name: "BravoSoChe");

            migrationBuilder.DropTable(
                name: "BT_KhachHang");

            migrationBuilder.DropTable(
                name: "BT_MaLoaiCa");

            migrationBuilder.DropTable(
                name: "BT_MaThanhPham");

            migrationBuilder.DropTable(
                name: "BT_NhanVienTheoNhom");

            migrationBuilder.DropTable(
                name: "BT_NhomTinhLuong");

            migrationBuilder.DropTable(
                name: "BT_PhieuCan");

            migrationBuilder.DropTable(
                name: "ChiTietRaCoi");

            migrationBuilder.DropTable(
                name: "ChucVu");

            migrationBuilder.DropTable(
                name: "CoiLogs");

            migrationBuilder.DropTable(
                name: "CoiMonitor");

            migrationBuilder.DropTable(
                name: "ColorCode");

            migrationBuilder.DropTable(
                name: "CongViecPhuFillet");

            migrationBuilder.DropTable(
                name: "CongViecTinhLuongXepKhuon");

            migrationBuilder.DropTable(
                name: "CongViecTinhLuongXepKhuonSanLuong");

            migrationBuilder.DropTable(
                name: "CongViecTinhLuongXepKhuonTheoLoaiThanhPham");

            migrationBuilder.DropTable(
                name: "DanhMucFormMoTrucTiep");

            migrationBuilder.DropTable(
                name: "DanhSachMayTinh");

            migrationBuilder.DropTable(
                name: "DanhSachToKiem");

            migrationBuilder.DropTable(
                name: "DataFilletv2");

            migrationBuilder.DropTable(
                name: "DG_DonGia");

            migrationBuilder.DropTable(
                name: "DG_DonGiaT");

            migrationBuilder.DropTable(
                name: "DG_LoaiDonGia");

            migrationBuilder.DropTable(
                name: "DG_SanPhamTinhLuong");

            migrationBuilder.DropTable(
                name: "DinhMucDinhHinh");

            migrationBuilder.DropTable(
                name: "DinhMucFillet");

            migrationBuilder.DropTable(
                name: "DinhMucXepHang");

            migrationBuilder.DropTable(
                name: "GioVaoRaFillet");

            migrationBuilder.DropTable(
                name: "GioVaoRaTinhLuongXepKhuon");

            migrationBuilder.DropTable(
                name: "KD_DonHang");

            migrationBuilder.DropTable(
                name: "KD_PhieuCan");

            migrationBuilder.DropTable(
                name: "KD_QuyCach");

            migrationBuilder.DropTable(
                name: "KD_Size");

            migrationBuilder.DropTable(
                name: "KD_ThanhPham");

            migrationBuilder.DropTable(
                name: "KD_Tui");

            migrationBuilder.DropTable(
                name: "KNH_PhieuCan");

            migrationBuilder.DropTable(
                name: "KNH_QuyCach");

            migrationBuilder.DropTable(
                name: "KNH_Size");

            migrationBuilder.DropTable(
                name: "KNH_ThanhPham");

            migrationBuilder.DropTable(
                name: "KNH_ThongTinSanPham");

            migrationBuilder.DropTable(
                name: "LineFillet");

            migrationBuilder.DropTable(
                name: "LineFilletv2");

            migrationBuilder.DropTable(
                name: "LoaiCaXepKhuon");

            migrationBuilder.DropTable(
                name: "LogKetChuyen");

            migrationBuilder.DropTable(
                name: "LogKetChuyenBravo");

            migrationBuilder.DropTable(
                name: "LogKiemSoatKetChuyen");

            migrationBuilder.DropTable(
                name: "LoNguyenLieu");

            migrationBuilder.DropTable(
                name: "LoTheoLine");

            migrationBuilder.DropTable(
                name: "MaAoVungNuoi");

            migrationBuilder.DropTable(
                name: "MaCa");

            migrationBuilder.DropTable(
                name: "MaChatLuongTaiChe");

            migrationBuilder.DropTable(
                name: "MaChatLuongXepKhuon");

            migrationBuilder.DropTable(
                name: "MaChatLuongXepKhuonBlock");

            migrationBuilder.DropTable(
                name: "MaChieuXaXepKhuon");

            migrationBuilder.DropTable(
                name: "MaChuAoVungNuoi");

            migrationBuilder.DropTable(
                name: "MaCoiXepKhuon");

            migrationBuilder.DropTable(
                name: "MaCongDoanVungNuoi");

            migrationBuilder.DropTable(
                name: "MaCongDoanXepKhuon");

            migrationBuilder.DropTable(
                name: "MaCongViecTaiChe");

            migrationBuilder.DropTable(
                name: "MaGheVungNuoi");

            migrationBuilder.DropTable(
                name: "MaKhachHangCaChet");

            migrationBuilder.DropTable(
                name: "MaKhachHangXepKhuon");

            migrationBuilder.DropTable(
                name: "MaKhuVucXepKhuon");

            migrationBuilder.DropTable(
                name: "MaLo");

            migrationBuilder.DropTable(
                name: "MaLoaiCaCaChet");

            migrationBuilder.DropTable(
                name: "MaLoaiCaCaoThit");

            migrationBuilder.DropTable(
                name: "MaLoaiCaCatTiet");

            migrationBuilder.DropTable(
                name: "MaLoaiCaDinhHinh");

            migrationBuilder.DropTable(
                name: "MaLoaiCaFillet");

            migrationBuilder.DropTable(
                name: "MaLoaiCaGiongVungNuoi");

            migrationBuilder.DropTable(
                name: "MaLoaiCaLangDa");

            migrationBuilder.DropTable(
                name: "MaLoaiCaNguyenLieu");

            migrationBuilder.DropTable(
                name: "MaLoaiCaNguyenLieu_temp");

            migrationBuilder.DropTable(
                name: "MaLoaiCaPhuPham");

            migrationBuilder.DropTable(
                name: "MaLoaiCaSoCheDinhHinh");

            migrationBuilder.DropTable(
                name: "MaLoaiCaTaiChe");

            migrationBuilder.DropTable(
                name: "MaLoaiCaVungNuoi");

            migrationBuilder.DropTable(
                name: "MaLoaiCaXepKhuon");

            migrationBuilder.DropTable(
                name: "MaLoi");

            migrationBuilder.DropTable(
                name: "MaMauCaGiongVungNuoi");

            migrationBuilder.DropTable(
                name: "MaMauCatTiet");

            migrationBuilder.DropTable(
                name: "MaMauDinhHinh");

            migrationBuilder.DropTable(
                name: "MaMauFillet");

            migrationBuilder.DropTable(
                name: "MaMauLangDa");

            migrationBuilder.DropTable(
                name: "MaMauNguyenLieu");

            migrationBuilder.DropTable(
                name: "MaMauNguyenLieu_temp");

            migrationBuilder.DropTable(
                name: "MaMauPhuPham");

            migrationBuilder.DropTable(
                name: "MaMauTaiChe");

            migrationBuilder.DropTable(
                name: "MaMauXepKhuon");

            migrationBuilder.DropTable(
                name: "MaMauXepKhuonBlock");

            migrationBuilder.DropTable(
                name: "MaNetXepKhuon");

            migrationBuilder.DropTable(
                name: "MaNhanVienTheoNhomXepKhuon");

            migrationBuilder.DropTable(
                name: "MaNhomXepKhuon");

            migrationBuilder.DropTable(
                name: "MapThanhPhamFillet");

            migrationBuilder.DropTable(
                name: "MaQuyCachCaoThit");

            migrationBuilder.DropTable(
                name: "MaQuyCachXepKhuon");

            migrationBuilder.DropTable(
                name: "MaSanPhamDinhHinhBravo");

            migrationBuilder.DropTable(
                name: "MaSizeCaoThit");

            migrationBuilder.DropTable(
                name: "MaSizeCatTiet");

            migrationBuilder.DropTable(
                name: "MaSizeChinhXepKhuon");

            migrationBuilder.DropTable(
                name: "MaSizeDinhHinh");

            migrationBuilder.DropTable(
                name: "MaSizeFillet");

            migrationBuilder.DropTable(
                name: "MaSizeLangDa");

            migrationBuilder.DropTable(
                name: "MaSizeNguyenLieu");

            migrationBuilder.DropTable(
                name: "MaSizeNguyenLieu_temp");

            migrationBuilder.DropTable(
                name: "MaSizePhuPham");

            migrationBuilder.DropTable(
                name: "MaSizeTaiChe");

            migrationBuilder.DropTable(
                name: "MaSizeXepKhuon");

            migrationBuilder.DropTable(
                name: "MaSizeXepKhuonBlock");

            migrationBuilder.DropTable(
                name: "MaSizeXepKhuonKHC");

            migrationBuilder.DropTable(
                name: "MaThanhPham_PhoiTron");

            migrationBuilder.DropTable(
                name: "MaThanhPhamCaoThit");

            migrationBuilder.DropTable(
                name: "MaThanhPhamCatTiet");

            migrationBuilder.DropTable(
                name: "MaThanhPhamChinhXepKhuon");

            migrationBuilder.DropTable(
                name: "MaThanhPhamDinhHinh");

            migrationBuilder.DropTable(
                name: "MaThanhPhamDinhHinh_Color");

            migrationBuilder.DropTable(
                name: "MaThanhPhamDinhHinh_TyLe");

            migrationBuilder.DropTable(
                name: "MaThanhPhamExDinhHinh");

            migrationBuilder.DropTable(
                name: "MaThanhPhamFillet");

            migrationBuilder.DropTable(
                name: "MaThanhPhamFillet_Color");

            migrationBuilder.DropTable(
                name: "MaThanhPhamFillet_HanMucTrongLuong");

            migrationBuilder.DropTable(
                name: "MaThanhPhamFillet_ThanhPhamMacDinh");

            migrationBuilder.DropTable(
                name: "MaThanhPhamLangDa");

            migrationBuilder.DropTable(
                name: "MaThanhPhamLangDa_Color");

            migrationBuilder.DropTable(
                name: "MaThanhPhamNguyenLieu");

            migrationBuilder.DropTable(
                name: "MaThanhPhamNguyenLieu_temp");

            migrationBuilder.DropTable(
                name: "MaThanhPhamPhuPham");

            migrationBuilder.DropTable(
                name: "MaThanhPhamPhuPham_Color");

            migrationBuilder.DropTable(
                name: "MaThanhPhamSoCheDinhHinh");

            migrationBuilder.DropTable(
                name: "MaThanhPhamSoCheDinhHinh_Color");

            migrationBuilder.DropTable(
                name: "MaThanhPhamTaiChe");

            migrationBuilder.DropTable(
                name: "MaThanhPhamXepKhuon");

            migrationBuilder.DropTable(
                name: "MaThanhPhamXepKhuon_Color");

            migrationBuilder.DropTable(
                name: "MaThanhPhamXepKhuonBlock");

            migrationBuilder.DropTable(
                name: "MaThanhPhamXepKhuonBlock_Color");

            migrationBuilder.DropTable(
                name: "MaThanhPhamXepKhuonKHC");

            migrationBuilder.DropTable(
                name: "MaThongKeCaChet");

            migrationBuilder.DropTable(
                name: "MaThongKeDauAo");

            migrationBuilder.DropTable(
                name: "MaXepHang");

            migrationBuilder.DropTable(
                name: "MayCan");

            migrationBuilder.DropTable(
                name: "MayLangDa");

            migrationBuilder.DropTable(
                name: "MayPhanCo");

            migrationBuilder.DropTable(
                name: "MocThoiGianPhanCa");

            migrationBuilder.DropTable(
                name: "MPG_CongThuc");

            migrationBuilder.DropTable(
                name: "MPG_CongThucChiTiet");

            migrationBuilder.DropTable(
                name: "MPG_CongThucXacNhan");

            migrationBuilder.DropTable(
                name: "MPG_PhieuCan");

            migrationBuilder.DropTable(
                name: "MPG_SanPham");

            migrationBuilder.DropTable(
                name: "NguoiDung_FormMoTrucTiep");

            migrationBuilder.DropTable(
                name: "NguoiDung_NhomQuyen");

            migrationBuilder.DropTable(
                name: "NguoiDung_Quyen");

            migrationBuilder.DropTable(
                name: "NguoiDung_ThongTin");

            migrationBuilder.DropTable(
                name: "NguyenLieu_PhuongTien");

            migrationBuilder.DropTable(
                name: "NguyenLieu_TyLeNuoc");

            migrationBuilder.DropTable(
                name: "NhaCungCapNguyenLieu");

            migrationBuilder.DropTable(
                name: "NhaCungCapNguyenLieu_DonGiaVanChuyen");

            migrationBuilder.DropTable(
                name: "NhaCungCapNguyenLieu_temp");

            migrationBuilder.DropTable(
                name: "NhaMuaPhuPham");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "NhanVienCongCu");

            migrationBuilder.DropTable(
                name: "NhanVienDaiThanh");

            migrationBuilder.DropTable(
                name: "NhanVienDaiThanhCu");

            migrationBuilder.DropTable(
                name: "NhanVienPhucVuTheoBan");

            migrationBuilder.DropTable(
                name: "NhanVienPhuTheoBanFillet");

            migrationBuilder.DropTable(
                name: "NhanVienPhuTheoLine");

            migrationBuilder.DropTable(
                name: "NhanVienSanLuongTheoLineFillet");

            migrationBuilder.DropTable(
                name: "NhanVienTheoBan");

            migrationBuilder.DropTable(
                name: "NhanVienTheoLine");

            migrationBuilder.DropTable(
                name: "NhanVienTrongBanCatTiet");

            migrationBuilder.DropTable(
                name: "NhomKiemSoChe");

            migrationBuilder.DropTable(
                name: "NhomLo");

            migrationBuilder.DropTable(
                name: "NhomSoCheDinhHinh");

            migrationBuilder.DropTable(
                name: "PD_CongThuc");

            migrationBuilder.DropTable(
                name: "PD_CongThucChiTiet");

            migrationBuilder.DropTable(
                name: "PD_DonViTinh");

            migrationBuilder.DropTable(
                name: "PD_LoaiSanPham");

            migrationBuilder.DropTable(
                name: "PD_LyDo");

            migrationBuilder.DropTable(
                name: "PD_PhieuCan");

            migrationBuilder.DropTable(
                name: "PD_PhieuNhap");

            migrationBuilder.DropTable(
                name: "PD_PhieuNhapChiTiet");

            migrationBuilder.DropTable(
                name: "PD_PhieuXuat");

            migrationBuilder.DropTable(
                name: "PD_PhieuXuatChiTiet");

            migrationBuilder.DropTable(
                name: "PD_SanPham");

            migrationBuilder.DropTable(
                name: "PhanBoNhanVienTheoCongViecPhuFillet");

            migrationBuilder.DropTable(
                name: "PhieuCanBTPDinhHinh");

            migrationBuilder.DropTable(
                name: "PhieuCanBTPDinhHinh2");

            migrationBuilder.DropTable(
                name: "PhieuCanBTPFillet");

            migrationBuilder.DropTable(
                name: "PhieuCanBTPFilletv2");

            migrationBuilder.DropTable(
                name: "PhieuCanCaChet");

            migrationBuilder.DropTable(
                name: "PhieuCanCaChetDaiThanhSide");

            migrationBuilder.DropTable(
                name: "PhieuCanCaGiongVungNuoi");

            migrationBuilder.DropTable(
                name: "PhieuCanCaGiongVungNuoiDaiThanhSide");

            migrationBuilder.DropTable(
                name: "PhieuCanCaoThit");

            migrationBuilder.DropTable(
                name: "PhieuCanChinhXepKhuon");

            migrationBuilder.DropTable(
                name: "PhieuCanChinhXepKhuon2");

            migrationBuilder.DropTable(
                name: "PhieuCanDinhHinh");

            migrationBuilder.DropTable(
                name: "PhieuCanLangDa");

            migrationBuilder.DropTable(
                name: "PhieuCanNguyenLieu");

            migrationBuilder.DropTable(
                name: "PhieuCanPhaLoc");

            migrationBuilder.DropTable(
                name: "PhieuCanPhuPham");

            migrationBuilder.DropTable(
                name: "PhieuCanPhuPhamv2");

            migrationBuilder.DropTable(
                name: "PhieuCanPhuXepKhuon");

            migrationBuilder.DropTable(
                name: "PhieuCanPhuXepKhuon2");

            migrationBuilder.DropTable(
                name: "PhieuCanRaCoi");

            migrationBuilder.DropTable(
                name: "PhieuCanRaDong");

            migrationBuilder.DropTable(
                name: "PhieuCanRaDong2");

            migrationBuilder.DropTable(
                name: "PhieuCanSauXepKhuon");

            migrationBuilder.DropTable(
                name: "PhieuCanSoCheDinhHinh");

            migrationBuilder.DropTable(
                name: "PhieuCanSoCheDinhHinh2");

            migrationBuilder.DropTable(
                name: "PhieuCanTaiChe");

            migrationBuilder.DropTable(
                name: "PhieuCanTaiChe2");

            migrationBuilder.DropTable(
                name: "PhieuCanTPDinhHinh");

            migrationBuilder.DropTable(
                name: "PhieuCanTPDinhHinh2");

            migrationBuilder.DropTable(
                name: "PhieuCanTPFillet");

            migrationBuilder.DropTable(
                name: "PhieuCanTPFilletv2");

            migrationBuilder.DropTable(
                name: "PhieuCanVungNuoi");

            migrationBuilder.DropTable(
                name: "PhieuCanVungNuoiDaiThanhSide");

            migrationBuilder.DropTable(
                name: "PhieuCanXepKhuonBlock");

            migrationBuilder.DropTable(
                name: "PhieuCanXepKhuonBlock2");

            migrationBuilder.DropTable(
                name: "PhieuCanXepKhuonKHC");

            migrationBuilder.DropTable(
                name: "PhieuCanXepKhuonKHC2");

            migrationBuilder.DropTable(
                name: "PhieuSanLuongRaCoiTinhLuongXepKhuon");

            migrationBuilder.DropTable(
                name: "PhuongTien_TaiTrong");

            migrationBuilder.DropTable(
                name: "PhuongTien_TrongLuongDauAo");

            migrationBuilder.DropTable(
                name: "PhuongTienChoNguyenLieu");

            migrationBuilder.DropTable(
                name: "PhuongTienChoNguyenLieu_temp");

            migrationBuilder.DropTable(
                name: "PhuongTienChoPhuPham");

            migrationBuilder.DropTable(
                name: "PhuongTienKhongSuDung");

            migrationBuilder.DropTable(
                name: "PLC");

            migrationBuilder.DropTable(
                name: "PLCChiTiet");

            migrationBuilder.DropTable(
                name: "PMS_L");

            migrationBuilder.DropTable(
                name: "QuickSearchMenu");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "RolePermistion");

            migrationBuilder.DropTable(
                name: "RP_MaThanhPhamDinhHinh");

            migrationBuilder.DropTable(
                name: "RP_MaThanhPhamFillet");

            migrationBuilder.DropTable(
                name: "RP_MaThanhPhamNL");

            migrationBuilder.DropTable(
                name: "RP_MaThanhPhamNL_Fillet");

            migrationBuilder.DropTable(
                name: "RP_MaThanhPhamNL_SoChe");

            migrationBuilder.DropTable(
                name: "RP_NangXuatDinhMuc");

            migrationBuilder.DropTable(
                name: "RP_NhomThanhPham_DinhHinh_Fillet");

            migrationBuilder.DropTable(
                name: "RP_SanLuongDenThoiDiem");

            migrationBuilder.DropTable(
                name: "RP_SanLuongNguyenLieuTrenBanNgay");

            migrationBuilder.DropTable(
                name: "SettingDashboard");

            migrationBuilder.DropTable(
                name: "Sheet1$");

            migrationBuilder.DropTable(
                name: "T_BaoBi");

            migrationBuilder.DropTable(
                name: "T_BieuMauGiao_Import");

            migrationBuilder.DropTable(
                name: "T_Bon");

            migrationBuilder.DropTable(
                name: "T_ChiTietBon");

            migrationBuilder.DropTable(
                name: "T_ChiTietVoXo");

            migrationBuilder.DropTable(
                name: "T_CongDoan");

            migrationBuilder.DropTable(
                name: "T_CongDoanTheoThanhPham");

            migrationBuilder.DropTable(
                name: "T_DinhMuc");

            migrationBuilder.DropTable(
                name: "T_DonHang");

            migrationBuilder.DropTable(
                name: "T_Goup");

            migrationBuilder.DropTable(
                name: "T_HoaChat");

            migrationBuilder.DropTable(
                name: "T_KhachHang");

            migrationBuilder.DropTable(
                name: "T_KhangSinh");

            migrationBuilder.DropTable(
                name: "T_KhuVuc");

            migrationBuilder.DropTable(
                name: "T_LenhSanXuat");

            migrationBuilder.DropTable(
                name: "T_LoaiCan");

            migrationBuilder.DropTable(
                name: "T_LoaiKhuon");

            migrationBuilder.DropTable(
                name: "T_LoaiNguyenLieu");

            migrationBuilder.DropTable(
                name: "T_LoNguyenLieu");

            migrationBuilder.DropTable(
                name: "T_MuaNguyenLieu_Import");

            migrationBuilder.DropTable(
                name: "T_NhaCungCap");

            migrationBuilder.DropTable(
                name: "T_NhomHoaChat");

            migrationBuilder.DropTable(
                name: "T_NhomLo");

            migrationBuilder.DropTable(
                name: "T_Phieu");

            migrationBuilder.DropTable(
                name: "T_PhieuCan");

            migrationBuilder.DropTable(
                name: "T_PhieuCan_Server");

            migrationBuilder.DropTable(
                name: "T_PhieuCanThuMua");

            migrationBuilder.DropTable(
                name: "T_PhieuCanThuMua_Server");

            migrationBuilder.DropTable(
                name: "T_PhieuNhomYeuCau");

            migrationBuilder.DropTable(
                name: "T_PhieuPhanCo");

            migrationBuilder.DropTable(
                name: "T_PhieuPhanCoChiTiet");

            migrationBuilder.DropTable(
                name: "T_PhieuYeuCau");

            migrationBuilder.DropTable(
                name: "T_PhuGia");

            migrationBuilder.DropTable(
                name: "T_PhuongTien");

            migrationBuilder.DropTable(
                name: "T_QuyCach");

            migrationBuilder.DropTable(
                name: "T_QuyTrinh");

            migrationBuilder.DropTable(
                name: "T_QuyTrinhTheoNgay");

            migrationBuilder.DropTable(
                name: "T_SanPham");

            migrationBuilder.DropTable(
                name: "T_Size");

            migrationBuilder.DropTable(
                name: "T_ThanhPham");

            migrationBuilder.DropTable(
                name: "T_ThongTinPhu");

            migrationBuilder.DropTable(
                name: "T_TieuChuan");

            migrationBuilder.DropTable(
                name: "T_TrangThaiNguyenLieu");

            migrationBuilder.DropTable(
                name: "T_TyLeHoaChatTheoNhom");

            migrationBuilder.DropTable(
                name: "TableInfo");

            migrationBuilder.DropTable(
                name: "Tare");

            migrationBuilder.DropTable(
                name: "ThePhieuSanLuongFillet");

            migrationBuilder.DropTable(
                name: "TheRo");

            migrationBuilder.DropTable(
                name: "TheThanhPham");

            migrationBuilder.DropTable(
                name: "TheTu");

            migrationBuilder.DropTable(
                name: "TheTuDaiThanh");

            migrationBuilder.DropTable(
                name: "TheTuNhomXepKhuon");

            migrationBuilder.DropTable(
                name: "Thit_LoaiNguyenLieuPhaLoc");

            migrationBuilder.DropTable(
                name: "Thit_MaThanhPhamPhaLoc");

            migrationBuilder.DropTable(
                name: "Thit_PhieuCanPhaLoc");

            migrationBuilder.DropTable(
                name: "Thit_SizePhaLoc");

            migrationBuilder.DropTable(
                name: "ToKiem");

            migrationBuilder.DropTable(
                name: "TrangThaiNhanVienTamThoi");

            migrationBuilder.DropTable(
                name: "TrongLuongBinhQuanCaChet");

            migrationBuilder.DropTable(
                name: "TrongLuongGioiHanDinhHinh");

            migrationBuilder.DropTable(
                name: "Update");

            migrationBuilder.DropTable(
                name: "UpdateSuaCa");

            migrationBuilder.DropTable(
                name: "UserArea");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "ViTriFillet");

            migrationBuilder.DropTable(
                name: "XiNghiep");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "NguoiDung");
        }
    }
}
