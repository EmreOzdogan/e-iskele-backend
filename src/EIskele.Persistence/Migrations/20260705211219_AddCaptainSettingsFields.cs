using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EIskele.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCaptainSettingsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TradeRegistryNumber",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Captains",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Captains",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TradeRegistryNumber",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Captains");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Captains");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "AspNetUsers");
        }
    }
}
