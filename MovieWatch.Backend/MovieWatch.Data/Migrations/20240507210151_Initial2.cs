using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieWatch.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date",
                table: "weather_forecasts");

            migrationBuilder.AddColumn<DateTime>(
                name: "created",
                table: "weather_forecasts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated",
                table: "weather_forecasts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created",
                table: "weather_forecasts");

            migrationBuilder.DropColumn(
                name: "updated",
                table: "weather_forecasts");

            migrationBuilder.AddColumn<DateOnly>(
                name: "date",
                table: "weather_forecasts",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
