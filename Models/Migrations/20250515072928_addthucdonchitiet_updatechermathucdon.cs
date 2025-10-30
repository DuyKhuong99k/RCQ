using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class addthucdonchitiet_updatechermathucdon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonAnId",
                table: "HQ_ThucDon");

            migrationBuilder.AddColumn<string>(
                name: "Ten",
                table: "HQ_ThucDon",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "HQ_ThucDonChiTiet",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThucDonId = table.Column<long>(type: "bigint", nullable: false),
                    MonAnId = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_ThucDonChiTiet", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HQ_ThucDonChiTiet");

            migrationBuilder.DropColumn(
                name: "Ten",
                table: "HQ_ThucDon");

            migrationBuilder.AddColumn<int>(
                name: "MonAnId",
                table: "HQ_ThucDon",
                type: "int",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: 0);
        }
    }
}
