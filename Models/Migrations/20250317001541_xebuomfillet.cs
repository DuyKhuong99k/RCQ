using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class xebuomfillet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsXeBuom",
                table: "MaThanhPhamFillet_U",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsXeBuom",
                table: "MaThanhPhamFillet_D",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsXeBuom",
                table: "MaThanhPhamFillet",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsXeBuom",
                table: "MaThanhPhamFillet_U");

            migrationBuilder.DropColumn(
                name: "IsXeBuom",
                table: "MaThanhPhamFillet_D");

            migrationBuilder.DropColumn(
                name: "IsXeBuom",
                table: "MaThanhPhamFillet");
        }
    }
}
