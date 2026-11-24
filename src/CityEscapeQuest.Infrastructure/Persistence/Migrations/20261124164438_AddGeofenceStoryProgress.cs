using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityEscapeQuest.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGeofenceStoryProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnswerHash",
                table: "Missions",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompletionText",
                table: "Missions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StoryIntro",
                table: "Missions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TaskPrompt",
                table: "Missions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedAtUtc",
                table: "GameSessions",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentStage",
                table: "GameSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "LastAccuracyMeters",
                table: "GameSessionParticipants",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LastLatitude",
                table: "GameSessionParticipants",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LastLongitude",
                table: "GameSessionParticipants",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LocationMeasuredAtUtc",
                table: "GameSessionParticipants",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LocationReceivedAtUtc",
                table: "GameSessionParticipants",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Missions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                columns: new[] { "AnswerHash", "CompletionText", "StoryIntro", "TaskPrompt" },
                values: new object[] { "", "", "", "" });

            migrationBuilder.UpdateData(
                table: "Missions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                columns: new[] { "StartLatitude", "StartLongitude", "AnswerHash", "CompletionText", "MinimumPlayers", "ShortDescription", "StartRadiusMeters", "StoryIntro", "TaskPrompt" },
                values: new object[] { 47.497912999999997, 19.054416, "E7F6C011776E8DB7CD330B54174FD76F7D0216B612387A5FFCFB81E6F0919683", "A rézlap fogaskerekei megmozdulnak. A 6-os kód helyes: az órák újra járnak, és az órásmester következő nyoma felvillan a szerkezet belsejében.", 1, "Az eltűnt órásmester utolsó üzenete a Deák térre vezet.", 120, "Budapest központi órái 23:47-nél megálltak. Varga Áron órásmester eltűnt, de egy rézlapot hagyott hátra: a város csak akkor indul újra, ha megtaláljátok a helyreállító számot.", "Három föld alatti vonal találkozik ezen a téren. Add össze a vonalak számait, és írd be az eredményt számjeggyel vagy betűvel." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerHash",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "CompletionText",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "StoryIntro",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "TaskPrompt",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "CompletedAtUtc",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "CurrentStage",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "LastAccuracyMeters",
                table: "GameSessionParticipants");

            migrationBuilder.DropColumn(
                name: "LastLatitude",
                table: "GameSessionParticipants");

            migrationBuilder.DropColumn(
                name: "LastLongitude",
                table: "GameSessionParticipants");

            migrationBuilder.DropColumn(
                name: "LocationMeasuredAtUtc",
                table: "GameSessionParticipants");

            migrationBuilder.DropColumn(
                name: "LocationReceivedAtUtc",
                table: "GameSessionParticipants");

            migrationBuilder.UpdateData(
                table: "Missions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                columns: new[] { "MinimumPlayers", "ShortDescription", "StartRadiusMeters", "StartLatitude", "StartLongitude" },
                values: new object[] { 2, "Ingyenes bevezető küldetés a belvárosban.", 80, 47.500900000000001, 19.0456 });
        }
    }
}
