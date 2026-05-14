using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduledMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlayingFieldId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PartyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MatchDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledMatches_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduledMatches_PlayingFields_PlayingFieldId",
                        column: x => x.PlayingFieldId,
                        principalTable: "PlayingFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledMatchAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScheduledMatchId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsAttending = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledMatchAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledMatchAttendances_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduledMatchAttendances_ScheduledMatches_ScheduledMatchId",
                        column: x => x.ScheduledMatchId,
                        principalTable: "ScheduledMatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMatchAttendances_PlayerId",
                table: "ScheduledMatchAttendances",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMatchAttendances_ScheduledMatchId",
                table: "ScheduledMatchAttendances",
                column: "ScheduledMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMatches_PartyId",
                table: "ScheduledMatches",
                column: "PartyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMatches_PlayingFieldId",
                table: "ScheduledMatches",
                column: "PlayingFieldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduledMatchAttendances");

            migrationBuilder.DropTable(
                name: "ScheduledMatches");
        }
    }
}
