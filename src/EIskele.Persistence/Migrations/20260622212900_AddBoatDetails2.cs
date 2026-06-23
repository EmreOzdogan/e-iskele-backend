using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EIskele.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBoatDetails2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BoatType",
                table: "Boats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BrandModel",
                table: "Boats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Boats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Length",
                table: "Boats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductionYear",
                table: "Boats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoatType",
                table: "Boats");

            migrationBuilder.DropColumn(
                name: "BrandModel",
                table: "Boats");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Boats");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "Boats");

            migrationBuilder.DropColumn(
                name: "ProductionYear",
                table: "Boats");
        }
    }
}
