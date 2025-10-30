using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class trongluongcoitheothanhpham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrongLuongCoiTheoThanhPham",
                columns: table => new
                {
                    MaCoi = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaThanhPham = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaXuong = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TrongLuongMax = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue:500)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrongLuongCoiTheoThanhPham", x => new { x.MaCoi, x.MaThanhPham, x.MaXuong });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrongLuongCoiTheoThanhPham");
        }
    }
}
