using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SensorMonitoring.Api.Migrations
{
    public partial class SensorMonitoringNewIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SensorReadings_SensorId",
                table: "SensorReadings");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_SensorId_DateTime",
                table: "SensorReadings",
                columns: new[] { "SensorId", "DateTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SensorReadings_SensorId_DateTime",
                table: "SensorReadings");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_SensorId",
                table: "SensorReadings",
                column: "SensorId");
        }
    }
}
