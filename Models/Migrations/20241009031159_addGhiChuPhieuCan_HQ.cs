using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class addGhiChuPhieuCan_HQ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "HQ_PhieuCan_U",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "HQ_PhieuCan_D",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "HQ_PhieuCan",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "HQ_PhieuCan_U");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "HQ_PhieuCan_D");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "HQ_PhieuCan");
        }
    }
}
