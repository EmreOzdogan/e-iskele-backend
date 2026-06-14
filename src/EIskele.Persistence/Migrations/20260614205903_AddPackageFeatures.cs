using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EIskele.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationPolicyType",
                table: "TourPackages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CaptainCancellationNote",
                table: "TourPackages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "TourPackages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "TourPackages",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DurationHours",
                table: "TourPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "TourPackages",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FreeCancellationHours",
                table: "TourPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsChildFriendly",
                table: "TourPackages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PrepaymentPercentage",
                table: "TourPackages",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RefundPolicyNote",
                table: "TourPackages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceFee",
                table: "TourPackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "TourPackages",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TourPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TimeLabel",
                table: "TourPackages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TourType",
                table: "TourPackages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WeatherPostponeNote",
                table: "TourPackages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PackageIncludes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsIncluded = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageIncludes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageIncludes_TourPackages_TourPackageId",
                        column: x => x.TourPackageId,
                        principalTable: "TourPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageIncludes_TourPackageId",
                table: "PackageIncludes",
                column: "TourPackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageIncludes");

            migrationBuilder.DropColumn(
                name: "CancellationPolicyType",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "CaptainCancellationNote",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "DurationHours",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "FreeCancellationHours",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "IsChildFriendly",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "PrepaymentPercentage",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "RefundPolicyNote",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "ServiceFee",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "TimeLabel",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "TourType",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "WeatherPostponeNote",
                table: "TourPackages");
        }
    }
}
