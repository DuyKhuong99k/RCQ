using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class trongluongcoitheosanphamAndCoile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoiLeXepKhuon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCoi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayGio = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoiLeXepKhuon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrongLuongCoiTheoSanPham",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCoi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaSanPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrongLuongCoiTheoSanPham", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoiLeXepKhuon");

            migrationBuilder.DropTable(
                name: "TrongLuongCoiTheoSanPham");
        }
    }
}
