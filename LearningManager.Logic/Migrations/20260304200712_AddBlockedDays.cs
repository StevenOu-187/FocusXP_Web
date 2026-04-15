using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningManager.Logic.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockedDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockedDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedDays", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockedDays_Date",
                table: "BlockedDays",
                column: "Date",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockedDays");
        }
    }
}
