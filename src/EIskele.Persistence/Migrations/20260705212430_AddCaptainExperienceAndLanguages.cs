using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EIskele.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCaptainExperienceAndLanguages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExperienceYears",
                table: "Captains",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Languages",
                table: "Captains",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperienceYears",
                table: "Captains");

            migrationBuilder.DropColumn(
                name: "Languages",
                table: "Captains");
        }
    }
}
