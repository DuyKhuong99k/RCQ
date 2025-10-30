using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class addmaxuong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaXuong",
                table: "HQ_PhieuCan_U",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MaXuong",
                table: "HQ_PhieuCan_D",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MaXuong",
                table: "HQ_PhieuCan",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaXuong",
                table: "HQ_PhieuCan_U");

            migrationBuilder.DropColumn(
                name: "MaXuong",
                table: "HQ_PhieuCan_D");

            migrationBuilder.DropColumn(
                name: "MaXuong",
                table: "HQ_PhieuCan");
        }
    }
}
