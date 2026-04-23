using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawerk.Migrations
{
    /// <inheritdoc />
    public partial class ExtraVehiceInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Vehicles",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Vehicles",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Doors",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Vehicles",
                type: "varchar(200)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Doors",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Vehicles");
        }
    }
}
