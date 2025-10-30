using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class sanphamtinhluonghq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HQ_MapSanPhamTinhLuong",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NgayGio = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql:"(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_MapSanPhamTinhLuong", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HQ_MapSanPhamTinhLuong");
        }
    }
}
