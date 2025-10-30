using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class updatethethanhpham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoLevel",
                table: "TheThanhPham",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MaChatLuongChinhXepKhuon",
                table: "TheThanhPham",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaChatLuongPhuXepKhuon",
                table: "TheThanhPham",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaSizeChinhXepKhuon",
                table: "TheThanhPham",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaSizePhuXepKhuon",
                table: "TheThanhPham",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaThanhPhamChinhXepKhuon",
                table: "TheThanhPham",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaThanhPhamPhuXepKhuon",
                table: "TheThanhPham",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoLevel",
                table: "TheThanhPham");

            migrationBuilder.DropColumn(
                name: "MaChatLuongChinhXepKhuon",
                table: "TheThanhPham");

            migrationBuilder.DropColumn(
                name: "MaChatLuongPhuXepKhuon",
                table: "TheThanhPham");

            migrationBuilder.DropColumn(
                name: "MaSizeChinhXepKhuon",
                table: "TheThanhPham");

            migrationBuilder.DropColumn(
                name: "MaSizePhuXepKhuon",
                table: "TheThanhPham");

            migrationBuilder.DropColumn(
                name: "MaThanhPhamChinhXepKhuon",
                table: "TheThanhPham");

            migrationBuilder.DropColumn(
                name: "MaThanhPhamPhuXepKhuon",
                table: "TheThanhPham");
        }
    }
}
