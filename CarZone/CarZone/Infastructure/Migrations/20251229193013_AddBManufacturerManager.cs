using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawerk.Migrations
{
    /// <inheritdoc />
    public partial class AddBManufacturerManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ManagerID",
                table: "Manufacturers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturers_ManagerID",
                table: "Manufacturers",
                column: "ManagerID",
                unique: true,
                filter: "[ManagerID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Manufacturers_Customers_ManagerID",
                table: "Manufacturers",
                column: "ManagerID",
                principalTable: "Customers",
                principalColumn: "CustomerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Manufacturers_Customers_ManagerID",
                table: "Manufacturers");

            migrationBuilder.DropIndex(
                name: "IX_Manufacturers_ManagerID",
                table: "Manufacturers");

            migrationBuilder.DropColumn(
                name: "ManagerID",
                table: "Manufacturers");
        }
    }
}
