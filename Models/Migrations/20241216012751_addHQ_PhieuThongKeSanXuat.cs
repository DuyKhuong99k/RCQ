using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class addHQ_PhieuThongKeSanXuat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HQ_PhieuThongKeSanXuat",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ngay = table.Column<DateTime>(type: "date", nullable: false),
                    SoChungTu = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ca = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaTo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TenTo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TenNhanVien = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaCongViec = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TenCongViec = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SanLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    GioBatDau = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    GioKetThuc = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_PhieuThongKeSanXuat", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HQ_PhieuThongKeSanXuat");
        }
    }
}
