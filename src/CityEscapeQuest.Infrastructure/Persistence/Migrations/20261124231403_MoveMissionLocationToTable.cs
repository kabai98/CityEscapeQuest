using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CityEscapeQuest.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveMissionLocationToTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocationCheckEnabled",
                table: "Missions",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StartLocationId",
                table: "Missions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "MissionLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    RadiusMeters = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionLocations", x => x.Id);
                });

            migrationBuilder.Sql("INSERT INTO MissionLocations (Id, Latitude, Longitude, RadiusMeters) SELECT Id, StartLatitude, StartLongitude, StartRadiusMeters FROM Missions");
            migrationBuilder.Sql("UPDATE Missions SET StartLocationId = Id");

            migrationBuilder.DropColumn(name: "StartLatitude", table: "Missions");
            migrationBuilder.DropColumn(name: "StartLongitude", table: "Missions");
            migrationBuilder.DropColumn(name: "StartRadiusMeters", table: "Missions");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_StartLocationId",
                table: "Missions",
                column: "StartLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_MissionLocations_StartLocationId",
                table: "Missions",
                column: "StartLocationId",
                principalTable: "MissionLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Missions_MissionLocations_StartLocationId",
                table: "Missions");

            migrationBuilder.DropIndex(
                name: "IX_Missions_StartLocationId",
                table: "Missions");

            migrationBuilder.AddColumn<double>(
                name: "StartLatitude",
                table: "Missions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StartLongitude",
                table: "Missions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "StartRadiusMeters",
                table: "Missions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE m SET StartLatitude = l.Latitude, StartLongitude = l.Longitude, StartRadiusMeters = l.RadiusMeters FROM Missions m JOIN MissionLocations l ON m.StartLocationId = l.Id");
            migrationBuilder.DropColumn(name: "IsLocationCheckEnabled", table: "Missions");
            migrationBuilder.DropColumn(name: "StartLocationId", table: "Missions");
            migrationBuilder.DropTable(name: "MissionLocations");
        }
    }
}
