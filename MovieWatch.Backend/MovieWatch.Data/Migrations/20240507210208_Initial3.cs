using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieWatch.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "temperature_f",
                table: "weather_forecasts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "temperature_f",
                table: "weather_forecasts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
