using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawerk.Migrations
{
    /// <inheritdoc />
    public partial class NewDataToVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Vehicles",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConditionScore",
                table: "Vehicles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DaysOnMarket",
                table: "Vehicles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HorsePower",
                table: "Vehicles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerName",
                table: "Vehicles",
                type: "varchar(50)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ConditionScore",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DaysOnMarket",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "HorsePower",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ManufacturerName",
                table: "Vehicles");
        }
    }
}
