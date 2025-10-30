using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class ChieuXaChatLuongPhuXepKhuon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaChatLuong",
                table: "PhieuCanPhuXepKhuon",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaChieuXa",
                table: "PhieuCanPhuXepKhuon",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaChatLuong",
                table: "PhieuCanPhuXepKhuon");

            migrationBuilder.DropColumn(
                name: "MaChieuXa",
                table: "PhieuCanPhuXepKhuon");
        }
    }
}
