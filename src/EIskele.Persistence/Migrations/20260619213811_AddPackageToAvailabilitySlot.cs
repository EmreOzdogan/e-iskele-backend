using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EIskele.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageToAvailabilitySlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "AvailabilitySlots",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TourPackageId",
                table: "AvailabilitySlots",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilitySlots_TourPackageId",
                table: "AvailabilitySlots",
                column: "TourPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilitySlots_TourPackages_TourPackageId",
                table: "AvailabilitySlots",
                column: "TourPackageId",
                principalTable: "TourPackages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilitySlots_TourPackages_TourPackageId",
                table: "AvailabilitySlots");

            migrationBuilder.DropIndex(
                name: "IX_AvailabilitySlots_TourPackageId",
                table: "AvailabilitySlots");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "AvailabilitySlots");

            migrationBuilder.DropColumn(
                name: "TourPackageId",
                table: "AvailabilitySlots");
        }
    }
}
