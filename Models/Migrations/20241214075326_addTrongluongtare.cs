using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class addTrongluongtare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TrongLuongTare",
                table: "PhieuCanPhuXepKhuon",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,defaultValueSql:"0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrongLuongTare",
                table: "PhieuCanPhuXepKhuon");
        }
    }
}
