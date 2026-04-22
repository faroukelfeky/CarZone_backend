using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawerk.Migrations
{
    /// <inheritdoc />
    public partial class RemovingName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Vehicles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Vehicles",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "");
        }
    }
}
