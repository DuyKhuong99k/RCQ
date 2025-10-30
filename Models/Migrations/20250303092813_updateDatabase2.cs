using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class updateDatabase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ao",
                columns: table => new
                {
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:"getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ao", x => x.Ma);
                });

            migrationBuilder.CreateTable(
                name: "Ao_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaAo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:"getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ao_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ao_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaAo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ao_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaChatLuongXepKhuon_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaChatLuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaChatLuongXepKhuon_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaChatLuongXepKhuon_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaChatLuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaChatLuongXepKhuon_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaChieuXaXepKhuon_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaChieuXa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaChieuXaXepKhuon_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaChieuXaXepKhuon_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaChieuXa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaChieuXaXepKhuon_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaCoiXepKhuon_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCoi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrongLuongMax = table.Column<double>(type: "float", nullable: false),
                    Tam = table.Column<bool>(type: "bit", nullable: false),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaCoiXepKhuon_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaCoiXepKhuon_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCoi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrongLuongMax = table.Column<double>(type: "float", nullable: false),
                    Tam = table.Column<bool>(type: "bit", nullable: false),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaCoiXepKhuon_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeChinhXepKhuon_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    _type = table.Column<int>(type: "int", nullable: false),
                    Idx = table.Column<int>(type: "int", nullable: false),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeChinhXepKhuon_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeChinhXepKhuon_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    _type = table.Column<int>(type: "int", nullable: false),
                    Idx = table.Column<int>(type: "int", nullable: false),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeChinhXepKhuon_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeFillet_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeFillet_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeFillet_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeFillet_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeNguyenLieu_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeNguyenLieu_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeNguyenLieu_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeNguyenLieu_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeXepKhuon_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeXepKhuon_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaSizeXepKhuon_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaSizeXepKhuon_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamChinhXepKhuon_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    Min = table.Column<double>(type: "float", nullable: false),
                    Max = table.Column<double>(type: "float", nullable: false),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    _type = table.Column<int>(type: "int", nullable: false),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamChinhXepKhuon_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamChinhXepKhuon_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    Min = table.Column<double>(type: "float", nullable: false),
                    Max = table.Column<double>(type: "float", nullable: false),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    _type = table.Column<int>(type: "int", nullable: false),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamChinhXepKhuon_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamFillet_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    Min = table.Column<double>(type: "float", nullable: true),
                    Max = table.Column<double>(type: "float", nullable: true),
                    IsSoChe = table.Column<bool>(type: "bit", nullable: false),
                    IsCaMuoi = table.Column<bool>(type: "bit", nullable: false),
                    DinhMuc = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThaiThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuongHienTai = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThoiGianTren1kgSeconds = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DinhMucHaoHut = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CodeId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    IsNotSetByTime = table.Column<bool>(type: "bit", nullable: false),
                    ColorRGB = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    KhongPhanBietSize = table.Column<bool>(type: "bit", nullable: false),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamFillet_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamFillet_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    Min = table.Column<double>(type: "float", nullable: true),
                    Max = table.Column<double>(type: "float", nullable: true),
                    IsSoChe = table.Column<bool>(type: "bit", nullable: false),
                    IsCaMuoi = table.Column<bool>(type: "bit", nullable: false),
                    DinhMuc = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThaiThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuongHienTai = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThoiGianTren1kgSeconds = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DinhMucHaoHut = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CodeId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    IsNotSetByTime = table.Column<bool>(type: "bit", nullable: false),
                    ColorRGB = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    KhongPhanBietSize = table.Column<bool>(type: "bit", nullable: false),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamFillet_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamNguyenLieu_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    Min = table.Column<double>(type: "float", nullable: true),
                    Max = table.Column<double>(type: "float", nullable: true),
                    IsSNL = table.Column<bool>(type: "bit", nullable: false),
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
                    IsCaNgopGheAoBanNgoai = table.Column<bool>(type: "bit", nullable: false),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamNguyenLieu_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamNguyenLieu_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLoaiCa = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    Min = table.Column<double>(type: "float", nullable: true),
                    Max = table.Column<double>(type: "float", nullable: true),
                    IsSNL = table.Column<bool>(type: "bit", nullable: false),
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
                    IsCaNgopGheAoBanNgoai = table.Column<bool>(type: "bit", nullable: false),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamNguyenLieu_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamXepKhuon_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    Min = table.Column<double>(type: "float", nullable: false),
                    Max = table.Column<double>(type: "float", nullable: false),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamXepKhuon_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaThanhPhamXepKhuon_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    Min = table.Column<double>(type: "float", nullable: false),
                    Max = table.Column<double>(type: "float", nullable: false),
                    BravoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaThanhPhamXepKhuon_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhuongTienChoNguyenLieu_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhuongTien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    IsHD = table.Column<bool>(type: "bit", nullable: false),
                    IsGhe = table.Column<bool>(type: "bit", nullable: false),
                    VungNuoiId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SoGhe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongTienChoNguyenLieu_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhuongTienChoNguyenLieu_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhuongTien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SuDung = table.Column<bool>(type: "bit", nullable: true),
                    IsHD = table.Column<bool>(type: "bit", nullable: false),
                    IsGhe = table.Column<bool>(type: "bit", nullable: false),
                    VungNuoiId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    SoGhe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MNgay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongTienChoNguyenLieu_U", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ao");

            migrationBuilder.DropTable(
                name: "Ao_D");

            migrationBuilder.DropTable(
                name: "Ao_U");

            migrationBuilder.DropTable(
                name: "MaChatLuongXepKhuon_D");

            migrationBuilder.DropTable(
                name: "MaChatLuongXepKhuon_U");

            migrationBuilder.DropTable(
                name: "MaChieuXaXepKhuon_D");

            migrationBuilder.DropTable(
                name: "MaChieuXaXepKhuon_U");

            migrationBuilder.DropTable(
                name: "MaCoiXepKhuon_D");

            migrationBuilder.DropTable(
                name: "MaCoiXepKhuon_U");

            migrationBuilder.DropTable(
                name: "MaSizeChinhXepKhuon_D");

            migrationBuilder.DropTable(
                name: "MaSizeChinhXepKhuon_U");

            migrationBuilder.DropTable(
                name: "MaSizeFillet_D");

            migrationBuilder.DropTable(
                name: "MaSizeFillet_U");

            migrationBuilder.DropTable(
                name: "MaSizeNguyenLieu_D");

            migrationBuilder.DropTable(
                name: "MaSizeNguyenLieu_U");

            migrationBuilder.DropTable(
                name: "MaSizeXepKhuon_D");

            migrationBuilder.DropTable(
                name: "MaSizeXepKhuon_U");

            migrationBuilder.DropTable(
                name: "MaThanhPhamChinhXepKhuon_D");

            migrationBuilder.DropTable(
                name: "MaThanhPhamChinhXepKhuon_U");

            migrationBuilder.DropTable(
                name: "MaThanhPhamFillet_D");

            migrationBuilder.DropTable(
                name: "MaThanhPhamFillet_U");

            migrationBuilder.DropTable(
                name: "MaThanhPhamNguyenLieu_D");

            migrationBuilder.DropTable(
                name: "MaThanhPhamNguyenLieu_U");

            migrationBuilder.DropTable(
                name: "MaThanhPhamXepKhuon_D");

            migrationBuilder.DropTable(
                name: "MaThanhPhamXepKhuon_U");

            migrationBuilder.DropTable(
                name: "PhuongTienChoNguyenLieu_D");

            migrationBuilder.DropTable(
                name: "PhuongTienChoNguyenLieu_U");
        }
    }
}
