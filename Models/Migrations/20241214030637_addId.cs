using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class addId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "PhieuCanTPFillet",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "PhieuCanPhuXepKhuon",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "PhieuCanPhuPhamv2",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "PhieuCanPhuPham",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "PhieuCanNguyenLieu",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "PhieuCanChinhXepKhuon",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "PhieuCanTPFillet");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PhieuCanPhuXepKhuon");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PhieuCanPhuPhamv2");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PhieuCanPhuPham");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PhieuCanNguyenLieu");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PhieuCanChinhXepKhuon");
        }
    }
}
