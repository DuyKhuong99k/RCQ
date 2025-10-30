using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class updatehqca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GioBatDau",
                table: "HQ_Ca",
                type: "datetime2(7)",
                nullable: false,
               defaultValueSql:"(getdate())");

            migrationBuilder.AddColumn<DateTime>(
                name: "GioKetThuc",
                table: "HQ_Ca",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql:"(getdate())");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GioBatDau",
                table: "HQ_Ca");

            migrationBuilder.DropColumn(
                name: "GioKetThuc",
                table: "HQ_Ca");
        }
    }
}
