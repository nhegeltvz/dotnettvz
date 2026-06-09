using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduledMatchIdToMatchRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScheduledMatchId",
                table: "MatchRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchRecords_ScheduledMatchId",
                table: "MatchRecords",
                column: "ScheduledMatchId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchRecords_ScheduledMatches_ScheduledMatchId",
                table: "MatchRecords",
                column: "ScheduledMatchId",
                principalTable: "ScheduledMatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchRecords_ScheduledMatches_ScheduledMatchId",
                table: "MatchRecords");

            migrationBuilder.DropIndex(
                name: "IX_MatchRecords_ScheduledMatchId",
                table: "MatchRecords");

            migrationBuilder.DropColumn(
                name: "ScheduledMatchId",
                table: "MatchRecords");
        }
    }
}
