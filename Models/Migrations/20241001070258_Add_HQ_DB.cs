using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class Add_HQ_DB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_TableInfos",
            //    table: "TableInfos");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_PhieuCanRaCois",
            //    table: "PhieuCanRaCois");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_NguoiDungs",
            //    table: "NguoiDungs");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_MayCans",
            //    table: "MayCans");

            //migrationBuilder.RenameTable(
            //    name: "UserAreas",
            //    newName: "UserArea");

            //migrationBuilder.RenameTable(
            //    name: "TableInfos",
            //    newName: "TableInfo");

            //migrationBuilder.RenameTable(
            //    name: "PhieuCanRaCois",
            //    newName: "PhieuCanRaCoi");

            //migrationBuilder.RenameTable(
            //    name: "NguoiDungs",
            //    newName: "NguoiDung");

            //migrationBuilder.RenameTable(
            //    name: "MayCans",
            //    newName: "MayCan");

            migrationBuilder.AddColumn<DateOnly>(
                name: "MNgay",
                table: "NhanVienDaiThanh",
                type: "date",
                nullable: false,
                defaultValueSql:  "(getdate())");

            migrationBuilder.AddColumn<DateOnly>(
                name: "MNgay",
                table: "ColorCode",
                type: "date",
                nullable: false,
                defaultValueSql:  "(getdate())");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_TableInfo",
            //    table: "TableInfo",
            //    column: "TableName");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_PhieuCanRaCoi",
            //    table: "PhieuCanRaCoi",
            //    column: "Id");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_NguoiDung",
            //    table: "NguoiDung",
            //    column: "Id");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_MayCan",
            //    table: "MayCan",
            //    column: "Id");

            migrationBuilder.CreateTable(
                name: "HQ_ColorCode_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code2 = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ColorRGB = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_ColorCode_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_ColorCode_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code2 = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ColorRGB = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_ColorCode_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_Lo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayNguyenLieu = table.Column<DateOnly>(type: "date", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false,defaultValue:false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_Lo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_Lo_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayNguyenLieu = table.Column<DateOnly>(type: "date", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false,defaultValue:false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_Lo_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_Lo_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayNguyenLieu = table.Column<DateOnly>(type: "date", nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_Lo_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_LoaiNguyenLieu",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_LoaiNguyenLieu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_LoaiNguyenLieu_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiNguyenLieuId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_LoaiNguyenLieu_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_LoaiNguyenLieu_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiNguyenLieuId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false,defaultValue:false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_LoaiNguyenLieu_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_LoChiTiet",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    TyLe = table.Column<decimal>(type: "decimal(18,4)", nullable: false,defaultValue:0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_LoChiTiet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_LoChiTiet_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoChiTietId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    TyLe = table.Column<decimal>(type: "decimal(18,4)", nullable: false,defaultValue:0),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_LoChiTiet_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_LoChiTiet_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoChiTietId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LoId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    TyLe = table.Column<decimal>(type: "decimal(18,4)", nullable: false, defaultValue: 0),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_LoChiTiet_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_NhanVien_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    IsShowDinhMuc = table.Column<bool>(type: "bit", nullable: false),
                    IsContracting = table.Column<bool>(type: "bit", nullable: false),
                    IsPhucVu = table.Column<bool>(type: "bit", nullable: false),
                    IsHuman = table.Column<bool>(type: "bit", nullable: false),
                    IsGiaCong = table.Column<bool>(type: "bit", nullable: false),
                    AC = table.Column<int>(type: "int", nullable: false),
                    IsChucNang = table.Column<bool>(type: "bit", nullable: false),
                    LoaiSanLuong = table.Column<int>(type: "int", nullable: false),
                    IsBanKiem = table.Column<bool>(type: "bit", nullable: false),
                    IsNhanVienCat = table.Column<bool>(type: "bit", nullable: false),
                    IsNhom = table.Column<bool>(type: "bit", nullable: false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_NhanVien_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_NhanVien_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
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
                    IsShowDinhMuc = table.Column<bool>(type: "bit", nullable: false),
                    IsContracting = table.Column<bool>(type: "bit", nullable: false),
                    IsPhucVu = table.Column<bool>(type: "bit", nullable: false),
                    IsHuman = table.Column<bool>(type: "bit", nullable: false),
                    IsGiaCong = table.Column<bool>(type: "bit", nullable: false),
                    AC = table.Column<int>(type: "int", nullable: false),
                    IsChucNang = table.Column<bool>(type: "bit", nullable: false),
                    LoaiSanLuong = table.Column<int>(type: "int", nullable: false),
                    IsBanKiem = table.Column<bool>(type: "bit", nullable: false),
                    IsNhanVienCat = table.Column<bool>(type: "bit", nullable: false),
                    IsNhom = table.Column<bool>(type: "bit", nullable: false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_NhanVien_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_PhieuCan",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    STT = table.Column<int>(type: "int", nullable: false,defaultValue:0),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    NgayGio = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:  "(getdate())"),
                    MayCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVienPhucVu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNhanVienBanKiem = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false,defaultValue:0),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false,defaultValue:0),
                    ChiSanLuong = table.Column<bool>(type: "bit", nullable: false,defaultValue:false),
                    Status = table.Column<int>(type: "int", nullable: false,defaultValue:0),
                    TheId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TheIdNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_PhieuCan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_PhieuCan_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuCanId = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    STT = table.Column<int>(type: "int", nullable: false,defaultValue:0),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    NgayGio = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:  "(getdate())"),
                    MayCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVienPhucVu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNhanVienBanKiem = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false,defaultValue:0),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false, defaultValue: 0),
                    ChiSanLuong = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TheId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TheIdNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayC = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_PhieuCan_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_PhieuCan_U",
                columns: table => new
                {
                   Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuCanId = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    STT = table.Column<int>(type: "int", nullable: false,defaultValue:0),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    NgayGio = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:  "(getdate())"),
                    MayCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSize = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaLoaiNguyenLieu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhanVienPhucVu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MaNhanVienBanKiem = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false,defaultValue:0),
                    TrongLuongTare = table.Column<decimal>(type: "decimal(18,3)", nullable: false, defaultValue: 0),
                    ChiSanLuong = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TheId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TheIdNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayC = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_PhieuCan_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_Size",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_Size", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_Size_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SizeId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false,defaultValue:false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_Size_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_Size_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SizeId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false,defaultValue:false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_Size_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_ThanhPham",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false,defaultValue:false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Max = table.Column<decimal>(type: "decimal(18,3)", nullable: false,defaultValue:100),
                    Min = table.Column<decimal>(type: "decimal(18,3)", nullable: false,defaultValue:0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_ThanhPham", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_ThanhPham_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThanhPhamId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false,defaultValue:false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    Max = table.Column<decimal>(type: "decimal(18,3)", nullable: false,defaultValue:100),
                    Min = table.Column<decimal>(type: "decimal(18,3)", nullable: false,defaultValue:0),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_ThanhPham_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_ThanhPham_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThanhPhamId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false,defaultValue:false),
                    MNgay = table.Column<DateOnly>(type: "date", nullable: false,defaultValueSql:  "(getdate())"),
                    Max = table.Column<decimal>(type: "decimal(18,3)", nullable: false,defaultValue:100),
                    Min = table.Column<decimal>(type: "decimal(18,3)", nullable: false,defaultValue:0),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_ThanhPham_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_TheRo_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ColorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    IsRach = table.Column<bool>(type: "bit", nullable: false,defaultValue:false),
                    MaThanhPhamDinhHinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_TheRo_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_TheRo_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThe = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ColorCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    IsRach = table.Column<bool>(type: "bit", nullable: false,defaultValue:false),
                    MaThanhPhamDinhHinh = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_TheRo_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_TheThanhPham_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    MaCoiXepKhuon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_TheThanhPham_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_TheThanhPham_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    MaCoiXepKhuon = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_TheThanhPham_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_TheTu_D",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTheTu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayGio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PCName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_TheTu_D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_TheTu_U",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTheTu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayGio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PCName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ngay = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_TheTu_U", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViewBaoCaoKeToanTheoNhaCungCap",
                columns: table => new
                {
                    Ngày = table.Column<DateOnly>(type: "date", nullable: true),
                    Xưởng = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NhàCC = table.Column<string>(name: "Nhà CC", type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaAo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SốngNX = table.Column<decimal>(name: "Sống NX", type: "decimal(38,2)", nullable: true),
                    NgộpGheNX = table.Column<decimal>(name: "Ngộp Ghe NX", type: "decimal(38,2)", nullable: true),
                    NgộpGheMuốiNX = table.Column<decimal>(name: "Ngộp Ghe Muối NX", type: "decimal(38,2)", nullable: true),
                    GheNgộpPP = table.Column<decimal>(name: "Ghe Ngộp PP", type: "decimal(38,2)", nullable: true),
                    NgộpAoMuốiNX = table.Column<decimal>(name: "Ngộp Ao Muối NX", type: "decimal(38,2)", nullable: true),
                    NgộpAoPP = table.Column<decimal>(name: "Ngộp Ao PP", type: "decimal(38,2)", nullable: true),
                    DạtNhỏ = table.Column<decimal>(name: "Dạt Nhỏ", type: "decimal(38,2)", nullable: true),
                    CáTạpPP = table.Column<decimal>(name: "Cá Tạp PP", type: "decimal(38,2)", nullable: true),
                    CănTin = table.Column<decimal>(name: "Căn Tin", type: "decimal(38,2)", nullable: true),
                    NgộpXeMuốiNX = table.Column<decimal>(name: "Ngộp Xe Muối NX", type: "decimal(38,2)", nullable: true),
                    XeNgộpPP = table.Column<decimal>(name: "Xe Ngộp PP", type: "decimal(38,2)", nullable: true),
                    GheMuốiPP = table.Column<decimal>(name: "Ghe Muối PP", type: "decimal(38,2)", nullable: true),
                    TổngTừngXưởng = table.Column<decimal>(name: "Tổng Từng Xưởng", type: "decimal(38,2)", nullable: true),
                    TổngNX = table.Column<decimal>(name: "Tổng NX", type: "decimal(38,2)", nullable: true),
                    TổngDạt = table.Column<decimal>(name: "Tổng Dạt", type: "decimal(38,2)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ViewBaoCaoKeToanTheoPhuongTien",
                columns: table => new
                {
                    Ngày = table.Column<DateOnly>(type: "date", nullable: true),
                    Xưởng = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NhàCC = table.Column<string>(name: "Nhà CC", type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Ao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cù = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    Ghe = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Số = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CáMạnh = table.Column<decimal>(name: "Cá Mạnh", type: "decimal(18,2)", nullable: false),
                    NgộpAoGhe = table.Column<decimal>(name: "Ngộp Ao Ghe", type: "decimal(18,2)", nullable: false),
                    NgộpAoXe = table.Column<decimal>(name: "Ngộp Ao Xe", type: "decimal(18,2)", nullable: false),
                    TổngHầm = table.Column<decimal>(name: "Tổng Hầm", type: "decimal(18,2)", nullable: false),
                    SốngNX = table.Column<decimal>(name: "Sống NX", type: "decimal(38,2)", nullable: false),
                    NgộpGheNX = table.Column<decimal>(name: "Ngộp Ghe NX", type: "decimal(38,2)", nullable: false),
                    NgộpGheMuốiNX = table.Column<decimal>(name: "Ngộp Ghe Muối NX", type: "decimal(38,2)", nullable: false),
                    GheNgộpPP = table.Column<decimal>(name: "Ghe Ngộp PP", type: "decimal(38,2)", nullable: false),
                    NgộpAoMuốiNX = table.Column<decimal>(name: "Ngộp Ao Muối NX", type: "decimal(38,2)", nullable: false),
                    NgộpAoPP = table.Column<decimal>(name: "Ngộp Ao PP", type: "decimal(38,2)", nullable: false),
                    DạtNhỏ = table.Column<decimal>(name: "Dạt Nhỏ", type: "decimal(38,2)", nullable: false),
                    CáTạpPP = table.Column<decimal>(name: "Cá Tạp PP", type: "decimal(38,2)", nullable: false),
                    CănTin = table.Column<decimal>(name: "Căn Tin", type: "decimal(38,2)", nullable: false),
                    NgộpXeMuốiNX = table.Column<decimal>(name: "Ngộp Xe Muối NX", type: "decimal(38,2)", nullable: false),
                    XeNgộpPP = table.Column<decimal>(name: "Xe Ngộp PP", type: "decimal(38,2)", nullable: false),
                    GheMuốiPP = table.Column<decimal>(name: "Ghe Muối PP", type: "decimal(38,2)", nullable: false),
                    TổngTừngXưởng = table.Column<decimal>(name: "Tổng Từng Xưởng", type: "decimal(38,2)", nullable: true),
                    TổngNX = table.Column<decimal>(name: "Tổng NX", type: "decimal(38,2)", nullable: false),
                    TổngDạt = table.Column<decimal>(name: "Tổng Dạt", type: "decimal(38,2)", nullable: false),
                    HaoHụt = table.Column<decimal>(name: "Hao Hụt", type: "decimal(38,2)", nullable: true),
                    TỷLệ = table.Column<decimal>(name: "Tỷ Lệ", type: "decimal(38,6)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ViewChiPhiVanChuyen",
                columns: table => new
                {
                    TK = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    AT = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    Ngày = table.Column<DateOnly>(type: "date", nullable: true),
                    STT = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    Ghe = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SốGhe = table.Column<string>(name: "Số Ghe", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TảiTrọng = table.Column<string>(name: "Tải Trọng", type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    Cù = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SLHầmcámạnh = table.Column<decimal>(name: "SL Hầm cá mạnh", type: "decimal(18,2)", nullable: false),
                    SLNX = table.Column<decimal>(name: "SL NX", type: "decimal(38,2)", nullable: false),
                    CLhaohụt = table.Column<decimal>(name: "CL - (hao hụt)", type: "decimal(38,2)", nullable: true),
                    CLdư = table.Column<decimal>(name: "CL + (dư)", type: "decimal(38,2)", nullable: true),
                    TLHaohụttừngghe = table.Column<decimal>(name: "TL Hao hụt từng ghe", type: "decimal(38,6)", nullable: true),
                    NAGHầm = table.Column<decimal>(name: "NAG Hầm", type: "decimal(18,2)", nullable: false),
                    NAGNX = table.Column<decimal>(name: "NAG NX", type: "decimal(38,2)", nullable: false),
                    NAGChênhLệch = table.Column<decimal>(name: "NAG Chênh Lệch", type: "decimal(38,2)", nullable: true),
                    NAXHầm = table.Column<decimal>(name: "NAX Hầm", type: "decimal(18,2)", nullable: false),
                    NAXNX = table.Column<decimal>(name: "NAX NX", type: "decimal(38,2)", nullable: false),
                    NAXchênhLệch = table.Column<decimal>(name: "NAX chênh Lệch", type: "decimal(38,2)", nullable: true),
                    TCSLHầm = table.Column<decimal>(name: "TCSL Hầm", type: "decimal(20,2)", nullable: true),
                    TCSLNX = table.Column<decimal>(name: "TCSL NX", type: "decimal(38,2)", nullable: true),
                    TCSLChênhLệch = table.Column<decimal>(name: "TCSL Chênh Lệch", type: "decimal(38,2)", nullable: true),
                    ĐơnGiá = table.Column<string>(name: "Đơn Giá", type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    ThànhTiền = table.Column<string>(name: "Thành Tiền", type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    Ao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ĐịaChỉAo = table.Column<string>(name: "Địa Chỉ Ao", type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ViewTongTungAoTrongNgay",
                columns: table => new
                {
                    Ngày = table.Column<DateOnly>(type: "date", nullable: true),
                    Xưởng = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    NhàCC = table.Column<string>(name: "Nhà CC", type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    Ao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cù = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    Ghe = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    Số = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false),
                    CáMạnh = table.Column<decimal>(name: "Cá Mạnh", type: "decimal(38,2)", nullable: false),
                    NgộpAoGhe = table.Column<decimal>(name: "Ngộp Ao Ghe", type: "decimal(38,2)", nullable: false),
                    NgộpAoXe = table.Column<decimal>(name: "Ngộp Ao Xe", type: "decimal(38,2)", nullable: false),
                    TổngHầm = table.Column<decimal>(name: "Tổng Hầm", type: "decimal(38,2)", nullable: false),
                    SốngNX = table.Column<decimal>(name: "Sống NX", type: "decimal(38,2)", nullable: false),
                    NgộpGheNX = table.Column<decimal>(name: "Ngộp Ghe NX", type: "decimal(38,2)", nullable: false),
                    NgộpGheMuốiNX = table.Column<decimal>(name: "Ngộp Ghe Muối NX", type: "decimal(38,2)", nullable: false),
                    GheNgộpPP = table.Column<decimal>(name: "Ghe Ngộp PP", type: "decimal(38,2)", nullable: false),
                    NgộpAoMuốiNX = table.Column<decimal>(name: "Ngộp Ao Muối NX", type: "decimal(38,2)", nullable: false),
                    NgộpAoPP = table.Column<decimal>(name: "Ngộp Ao PP", type: "decimal(38,2)", nullable: false),
                    DạtNhỏ = table.Column<decimal>(name: "Dạt Nhỏ", type: "decimal(38,2)", nullable: false),
                    CáTạpPP = table.Column<decimal>(name: "Cá Tạp PP", type: "decimal(38,2)", nullable: false),
                    CănTin = table.Column<decimal>(name: "Căn Tin", type: "decimal(38,2)", nullable: false),
                    NgộpXeMuốiNX = table.Column<decimal>(name: "Ngộp Xe Muối NX", type: "decimal(38,2)", nullable: false),
                    XeNgộpPP = table.Column<decimal>(name: "Xe Ngộp PP", type: "decimal(38,2)", nullable: false),
                    GheMuốiPP = table.Column<decimal>(name: "Ghe Muối PP", type: "decimal(38,2)", nullable: false),
                    TổngTừngXưởng = table.Column<decimal>(name: "Tổng Từng Xưởng", type: "decimal(38,2)", nullable: true),
                    TổngNX = table.Column<decimal>(name: "Tổng NX", type: "decimal(38,2)", nullable: false),
                    TổngDạt = table.Column<decimal>(name: "Tổng Dạt", type: "decimal(38,2)", nullable: false),
                    HaoHụt = table.Column<decimal>(name: "Hao Hụt", type: "decimal(38,2)", nullable: true),
                    TỷLệ = table.Column<decimal>(name: "Tỷ Lệ", type: "decimal(38,6)", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HQ_ColorCode_D");

            migrationBuilder.DropTable(
                name: "HQ_ColorCode_U");

            migrationBuilder.DropTable(
                name: "HQ_Lo");

            migrationBuilder.DropTable(
                name: "HQ_Lo_D");

            migrationBuilder.DropTable(
                name: "HQ_Lo_U");

            migrationBuilder.DropTable(
                name: "HQ_LoaiNguyenLieu");

            migrationBuilder.DropTable(
                name: "HQ_LoaiNguyenLieu_D");

            migrationBuilder.DropTable(
                name: "HQ_LoaiNguyenLieu_U");

            migrationBuilder.DropTable(
                name: "HQ_LoChiTiet");

            migrationBuilder.DropTable(
                name: "HQ_LoChiTiet_D");

            migrationBuilder.DropTable(
                name: "HQ_LoChiTiet_U");

            migrationBuilder.DropTable(
                name: "HQ_NhanVien_D");

            migrationBuilder.DropTable(
                name: "HQ_NhanVien_U");

            migrationBuilder.DropTable(
                name: "HQ_PhieuCan");

            migrationBuilder.DropTable(
                name: "HQ_PhieuCan_D");

            migrationBuilder.DropTable(
                name: "HQ_PhieuCan_U");

            migrationBuilder.DropTable(
                name: "HQ_Size");

            migrationBuilder.DropTable(
                name: "HQ_Size_D");

            migrationBuilder.DropTable(
                name: "HQ_Size_U");

            migrationBuilder.DropTable(
                name: "HQ_ThanhPham");

            migrationBuilder.DropTable(
                name: "HQ_ThanhPham_D");

            migrationBuilder.DropTable(
                name: "HQ_ThanhPham_U");

            migrationBuilder.DropTable(
                name: "HQ_TheRo_D");

            migrationBuilder.DropTable(
                name: "HQ_TheRo_U");

            migrationBuilder.DropTable(
                name: "HQ_TheThanhPham_D");

            migrationBuilder.DropTable(
                name: "HQ_TheThanhPham_U");

            migrationBuilder.DropTable(
                name: "HQ_TheTu_D");

            migrationBuilder.DropTable(
                name: "HQ_TheTu_U");

            migrationBuilder.DropTable(
                name: "ViewBaoCaoKeToanTheoNhaCungCap");

            migrationBuilder.DropTable(
                name: "ViewBaoCaoKeToanTheoPhuongTien");

            migrationBuilder.DropTable(
                name: "ViewChiPhiVanChuyen");

            migrationBuilder.DropTable(
                name: "ViewTongTungAoTrongNgay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TableInfo",
                table: "TableInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhieuCanRaCoi",
                table: "PhieuCanRaCoi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NguoiDung",
                table: "NguoiDung");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MayCan",
                table: "MayCan");

            migrationBuilder.DropColumn(
                name: "MNgay",
                table: "NhanVienDaiThanh");

            migrationBuilder.DropColumn(
                name: "MNgay",
                table: "ColorCode");

            migrationBuilder.RenameTable(
                name: "UserArea",
                newName: "UserAreas");

            migrationBuilder.RenameTable(
                name: "TableInfo",
                newName: "TableInfos");

            migrationBuilder.RenameTable(
                name: "PhieuCanRaCoi",
                newName: "PhieuCanRaCois");

            migrationBuilder.RenameTable(
                name: "NguoiDung",
                newName: "NguoiDungs");

            migrationBuilder.RenameTable(
                name: "MayCan",
                newName: "MayCans");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TableInfos",
                table: "TableInfos",
                column: "TableName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhieuCanRaCois",
                table: "PhieuCanRaCois",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NguoiDungs",
                table: "NguoiDungs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MayCans",
                table: "MayCans",
                column: "Id");
        }
    }
}
