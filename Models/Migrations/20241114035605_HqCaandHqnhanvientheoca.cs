using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class HqCaandHqnhanvientheoca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HQ_Ca",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ma = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NgayGio = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:"(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_Ca", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HQ_NhanVienTheoCa",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NhanVienId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NgayGio = table.Column<DateTime>(type: "datetime2(7)", nullable: false,defaultValueSql:"(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HQ_NhanVienTheoCa", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HQ_Ca");

            migrationBuilder.DropTable(
                name: "HQ_NhanVienTheoCa");
        }
    }
}
