using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class SplitAppUserAndPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchPlayers_AspNetUsers_PlayerId",
                table: "MatchPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchVotes_AspNetUsers_PlayerId",
                table: "MatchVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Parties_AspNetUsers_PlayerCreatedId",
                table: "Parties");

            migrationBuilder.DropForeignKey(
                name: "FK_PartyMembers_AspNetUsers_MembersId",
                table: "PartyMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerRatings_AspNetUsers_PlayerGivingRatingId",
                table: "PlayerRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerRatings_AspNetUsers_PlayerReceivingRatingId",
                table: "PlayerRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledMatchAttendances_AspNetUsers_PlayerId",
                table: "ScheduledMatchAttendances");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PlayerUsername",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PreferredPosition",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Bio = table.Column<string>(type: "TEXT", nullable: false),
                    PreferredPosition = table.Column<int>(type: "INTEGER", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId",
                table: "Players",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchPlayers_Players_PlayerId",
                table: "MatchPlayers",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchVotes_Players_PlayerId",
                table: "MatchVotes",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Parties_Players_PlayerCreatedId",
                table: "Parties",
                column: "PlayerCreatedId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartyMembers_Players_MembersId",
                table: "PartyMembers",
                column: "MembersId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerRatings_Players_PlayerGivingRatingId",
                table: "PlayerRatings",
                column: "PlayerGivingRatingId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerRatings_Players_PlayerReceivingRatingId",
                table: "PlayerRatings",
                column: "PlayerReceivingRatingId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledMatchAttendances_Players_PlayerId",
                table: "ScheduledMatchAttendances",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchPlayers_Players_PlayerId",
                table: "MatchPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchVotes_Players_PlayerId",
                table: "MatchVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Parties_Players_PlayerCreatedId",
                table: "Parties");

            migrationBuilder.DropForeignKey(
                name: "FK_PartyMembers_Players_MembersId",
                table: "PartyMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerRatings_Players_PlayerGivingRatingId",
                table: "PlayerRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerRatings_Players_PlayerReceivingRatingId",
                table: "PlayerRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledMatchAttendances_Players_PlayerId",
                table: "ScheduledMatchAttendances");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlayerUsername",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PreferredPosition",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                table: "AspNetUsers",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchPlayers_AspNetUsers_PlayerId",
                table: "MatchPlayers",
                column: "PlayerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchVotes_AspNetUsers_PlayerId",
                table: "MatchVotes",
                column: "PlayerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Parties_AspNetUsers_PlayerCreatedId",
                table: "Parties",
                column: "PlayerCreatedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartyMembers_AspNetUsers_MembersId",
                table: "PartyMembers",
                column: "MembersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerRatings_AspNetUsers_PlayerGivingRatingId",
                table: "PlayerRatings",
                column: "PlayerGivingRatingId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerRatings_AspNetUsers_PlayerReceivingRatingId",
                table: "PlayerRatings",
                column: "PlayerReceivingRatingId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledMatchAttendances_AspNetUsers_PlayerId",
                table: "ScheduledMatchAttendances",
                column: "PlayerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
