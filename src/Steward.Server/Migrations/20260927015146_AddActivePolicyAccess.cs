using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Steward.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddActivePolicyAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastAccessed",
                table: "PolicyAccess",
                newName: "UsageDate");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UnlockedUntil",
                table: "PolicyAccess",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnlockedUntil",
                table: "PolicyAccess");

            migrationBuilder.RenameColumn(
                name: "UsageDate",
                table: "PolicyAccess",
                newName: "LastAccessed");
        }
    }
}
