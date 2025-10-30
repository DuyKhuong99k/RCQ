using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class _20241111 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NhanVienTheoLine",
                table: "NhanVienTheoLine");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayGio",
                table: "TheThanhPham",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql:"(getdate())");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NhanVienTheoLine",
                table: "NhanVienTheoLine",
                columns: new[] { "Id", "Ngay", "CodeId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NhanVienTheoLine",
                table: "NhanVienTheoLine");

            migrationBuilder.DropColumn(
                name: "NgayGio",
                table: "TheThanhPham");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NhanVienTheoLine",
                table: "NhanVienTheoLine",
                columns: new[] { "Id", "CodeId" });
        }
    }
}
