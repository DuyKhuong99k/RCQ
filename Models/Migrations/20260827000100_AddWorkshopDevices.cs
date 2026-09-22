using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations;

public partial class AddChamCongTaiXuong : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ThietBiChamCongTaiXuong",
            columns: table => new
            {
                DeviceCode = table.Column<string>(
                    type: "nvarchar(450)",
                    nullable: false),

                DeviceName = table.Column<string>(
                    type: "nvarchar(200)",
                    maxLength: 200,
                    nullable: false),

                WorkshopCode = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: false),

                ConnectionId = table.Column<string>(
                    type: "nvarchar(200)",
                    maxLength: 200,
                    nullable: true),

                IsOnline = table.Column<bool>(
                    type: "bit",
                    nullable: false),

                LastSeen = table.Column<DateTime>(
                    type: "datetime2",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey(
                    "PK_ThietBiChamCongTaiXuong",
                    x => x.DeviceCode);
            });

        migrationBuilder.CreateTable(
            name: "ChamCongTaiXuong",
            columns: table => new
            {

                ThoiGian = table.Column<DateTime>(
                    type: "datetime2(0)",
                    nullable: false),

                Xuong = table.Column<string>(
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: true),

                ThietBi = table.Column<string>(
                    type: "nvarchar(50)",
                    maxLength: 50,
                    nullable: false),

                MaTheTu = table.Column<string>(
                    type: "nvarchar(50)",
                    maxLength: 50,
                    nullable: false),

                MaNhanVien = table.Column<string>(
                    type: "nvarchar(50)",
                    maxLength: 50,
                    nullable: true),

                TenNhanVien = table.Column<string>(
                    type: "nvarchar(200)",
                    maxLength: 200,
                    nullable: true),

                TrangThai = table.Column<string>(
                    type: "nvarchar(20)",
                    maxLength: 20,
                    nullable: true),

                Id = table.Column<long>(
                    type: "bigint",
                    nullable: false)
                    .Annotation(
                        "SqlServer:Identity",
                        "1, 1"),
            },
            constraints: table =>
            {
                table.PrimaryKey(
                    "PK_ChamCongTaiXuong",
                    x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ChamCongTaiXuong_ThoiGian",
            table: "ChamCongTaiXuong",
            column: "ThoiGian");

        migrationBuilder.CreateIndex(
            name: "IX_ChamCongTaiXuong_MaTheTu",
            table: "ChamCongTaiXuong",
            column: "MaTheTu");

        migrationBuilder.CreateIndex(
            name: "IX_ChamCongTaiXuong_ThietBi_ThoiGian",
            table: "ChamCongTaiXuong",
            columns: new[] { "ThietBi", "ThoiGian" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ChamCongTaiXuong");

        migrationBuilder.DropTable(
            name: "ThietBiChamCongTaiXuong");
    }
}