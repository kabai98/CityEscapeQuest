using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityEscapeQuest.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLobbyGuestJoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "JoinCodeExpiresAtUtc",
                table: "GameSessions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_JoinCodeHash",
                table: "GameSessions",
                column: "JoinCodeHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameSessions_JoinCodeHash",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "JoinCodeExpiresAtUtc",
                table: "GameSessions");
        }
    }
}
