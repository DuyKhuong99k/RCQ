using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class addchekforfilletdinhhinhnguyenlieu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ThamSoTangTrong",
                table: "PhieuCanRaCoi",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CTTYLE",
                table: "MaThanhPhamNguyenLieu",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsNguyenCon",
                table: "MaThanhPhamFillet",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCaDa",
                table: "MaThanhPhamDinhHinh",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ThamSoTangTrong",
                table: "MaThanhPhamChinhXepKhuon",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThamSoTangTrong",
                table: "PhieuCanRaCoi");

            migrationBuilder.DropColumn(
                name: "CTTYLE",
                table: "MaThanhPhamNguyenLieu");

            migrationBuilder.DropColumn(
                name: "IsNguyenCon",
                table: "MaThanhPhamFillet");

            migrationBuilder.DropColumn(
                name: "IsCaDa",
                table: "MaThanhPhamDinhHinh");

            migrationBuilder.DropColumn(
                name: "ThamSoTangTrong",
                table: "MaThanhPhamChinhXepKhuon");
        }
    }
}
