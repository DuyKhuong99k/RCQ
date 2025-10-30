using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class HQ_NhomAndNhanVienTheoNhom2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HQ_NhanVienTheoNhom",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaNhom = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayGioBatDau = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:"(getdate())"),
                    HeSo = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_NhanVienTheoNhom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_Nhom",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SuDung = table.Column<bool>(type: "bit", nullable: false,defaultValue: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_Nhom", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HQ_NhanVienTheoNhom");

            migrationBuilder.DropTable(
                name: "HQ_Nhom");
        }
    }
}
