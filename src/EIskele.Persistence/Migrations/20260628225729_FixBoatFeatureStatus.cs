using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EIskele.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixBoatFeatureStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE BoatFeatures SET Status = 'PendingReview' WHERE Status = 'Kontrol Bekliyor' OR Status = '0'");
            migrationBuilder.Sql("UPDATE BoatFeatures SET Status = 'Approved' WHERE Status = 'Onaylandı' OR Status = '1'");
            migrationBuilder.Sql("UPDATE BoatFeatures SET Status = 'Missing' WHERE Status = 'Eksik' OR Status = '2'");
            migrationBuilder.Sql("UPDATE BoatFeatures SET Status = 'Risky' WHERE Status = 'Riskli' OR Status = '3'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
