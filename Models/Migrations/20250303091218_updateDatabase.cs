using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class updateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MNgay",
                table: "PhuongTienChoNguyenLieu",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "MNgay",
                table: "MaThanhPhamXepKhuon",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql:"getdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "MNgay",
                table: "MaThanhPhamNguyenLieu",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql:"getdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "MNgay",
                table: "MaThanhPhamFillet",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "MNgay",
                table: "MaThanhPhamChinhXepKhuon",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "MNgay",
                table: "MaSizeXepKhuon",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "MNgay",
                table: "MaSizeNguyenLieu",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "MNgay",
                table: "MaSizeFillet",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "MNgay",
                table: "MaSizeChinhXepKhuon",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "MNgay",
                table: "MaCoiXepKhuon",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "MNgay",
                table: "MaChieuXaXepKhuon",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "getdate()");

            migrationBuilder.AddColumn<DateTime>(
                name: "MNgay",
                table: "MaChatLuongXepKhuon",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "getdate()");

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "CheckInOut",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MNgay",
                table: "PhuongTienChoNguyenLieu");

            migrationBuilder.DropColumn(
                name: "MNgay",
                table: "MaThanhPhamXepKhuon");

            migrationBuilder.DropColumn(
                name: "MNgay",
                table: "MaThanhPhamNguyenLieu");

            migrationBuilder.DropColumn(
                name: "MNgay",
                table: "MaThanhPhamFillet");

            migrationBuilder.DropColumn(
                name: "MNgay",
                table: "MaThanhPhamChinhXepKhuon");

            migrationBuilder.DropColumn(
                name: "MNgay",
                table: "MaSizeXepKhuon");

            migrationBuilder.DropColumn(
                name: "MNgay",
                table: "MaSizeNguyenLieu");

            migrationBuilder.DropColumn(
                name: "MNgay",
                table: "MaSizeFillet");

            migrationBuilder.DropColumn(
                name: "MNgay",
                table: "MaSizeChinhXepKhuon");

            migrationBuilder.DropColumn(
                name: "MNgay",
                table: "MaCoiXepKhuon");

            migrationBuilder.DropColumn(
                name: "MNgay",
                table: "MaChieuXaXepKhuon");

            migrationBuilder.DropColumn(
                name: "MNgay",
                table: "MaChatLuongXepKhuon");

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "CheckInOut",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
