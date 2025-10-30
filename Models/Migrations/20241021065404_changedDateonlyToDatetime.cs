using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class changedDateonlyToDatetime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "NhanVienDaiThanh",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_ThanhPham_U",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_ThanhPham_D",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_ThanhPham",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_Size_U",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_Size_D",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_Size",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_NhanVien_U",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_NhanVien_D",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_LoChiTiet_U",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_LoChiTiet_D",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_LoChiTiet",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_LoaiNguyenLieu_U",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_LoaiNguyenLieu_D",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_LoaiNguyenLieu",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_Lo_U",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_Lo_D",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "HQ_Lo",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MNgay",
                table: "ColorCode",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "NhanVienDaiThanh",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_ThanhPham_U",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_ThanhPham_D",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_ThanhPham",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_Size_U",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_Size_D",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_Size",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_NhanVien_U",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_NhanVien_D",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_LoChiTiet_U",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_LoChiTiet_D",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_LoChiTiet",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_LoaiNguyenLieu_U",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_LoaiNguyenLieu_D",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_LoaiNguyenLieu",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_Lo_U",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_Lo_D",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "HQ_Lo",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "MNgay",
                table: "ColorCode",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");
        }
    }
}
