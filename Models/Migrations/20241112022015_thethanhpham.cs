using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class thethanhpham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaSize",
                table: "TheThanhPham",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaSize",
                table: "HQ_TheThanhPham_U",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaSize",
                table: "HQ_TheThanhPham_D",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaSize",
                table: "TheThanhPham");

            migrationBuilder.DropColumn(
                name: "MaSize",
                table: "HQ_TheThanhPham_U");

            migrationBuilder.DropColumn(
                name: "MaSize",
                table: "HQ_TheThanhPham_D");
        }
    }
}
