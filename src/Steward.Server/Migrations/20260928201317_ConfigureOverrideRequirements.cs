using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Steward.Server.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureOverrideRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Override_DelayMinutes",
                table: "Policies",
                type: "REAL",
                nullable: false,
                defaultValue: 0.25);

            migrationBuilder.AddColumn<int>(
                name: "Override_RandomTextLength",
                table: "Policies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "OverrideRequests",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Override_DelayMinutes",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "Override_RandomTextLength",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "OverrideRequests");
        }
    }
}
