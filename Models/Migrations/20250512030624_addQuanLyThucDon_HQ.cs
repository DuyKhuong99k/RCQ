using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class addQuanLyThucDon_HQ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HQ_CodeGen",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayTao = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    DangKyIds = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Printed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_CodeGen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_DuyetThucDon",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayTao = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    NguoiTao = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ThucDonId = table.Column<long>(type: "bigint", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_DuyetThucDon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_HuyThucDon",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayTao = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    NguoiTao = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ThucDonId = table.Column<long>(type: "bigint", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_HuyThucDon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_LoaiMonAn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_LoaiMonAn", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_MonAn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LoaiMonAnId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_MonAn", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_NhatKyDangKyMonAn",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayGio = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    NhanVienId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ThietBi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MonAnId = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_NhatKyDangKyMonAn", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_NhatKyNhanMonAn",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayGio = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    NhanVienId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ThietBi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    CodeId = table.Column<long>(type: "bigint", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_NhatKyNhanMonAn", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_ThucDon",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    NguoiTao = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MonAnId = table.Column<int>(type: "int", unicode: false, maxLength: 50, nullable: false),
                    ThietBi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_ThucDon", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HQ_CodeGen");

            migrationBuilder.DropTable(
                name: "HQ_DuyetThucDon");

            migrationBuilder.DropTable(
                name: "HQ_HuyThucDon");

            migrationBuilder.DropTable(
                name: "HQ_LoaiMonAn");

            migrationBuilder.DropTable(
                name: "HQ_MonAn");

            migrationBuilder.DropTable(
                name: "HQ_NhatKyDangKyMonAn");

            migrationBuilder.DropTable(
                name: "HQ_NhatKyNhanMonAn");

            migrationBuilder.DropTable(
                name: "HQ_ThucDon");
        }
    }
}
