using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class notethanhphamfilletHL2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDat",
                table: "MaThanhPhamFillet",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNguyenLieuXeBuom",
                table: "MaThanhPhamFillet",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDat",
                table: "MaThanhPhamFillet");

            migrationBuilder.DropColumn(
                name: "IsNguyenLieuXeBuom",
                table: "MaThanhPhamFillet");
        }
    }
}
