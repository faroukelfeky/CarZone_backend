using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawerk.Migrations
{
    /// <inheritdoc />
    public partial class AddedCreatedAtForEntitiesAndFewChangesToVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Customers_CustomerID",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Manufacturers_ManufacturerID",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_CustomerID",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "Vehicles",
                newName: "Price");

            migrationBuilder.AddColumn<Guid>(
                name: "BuyerID",
                table: "Vehicles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Vehicles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "SellerID",
                table: "Vehicles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Salesman",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Manufacturers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Customers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Branches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_BuyerID",
                table: "Vehicles",
                column: "BuyerID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_SellerID",
                table: "Vehicles",
                column: "SellerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Customers_BuyerID",
                table: "Vehicles",
                column: "BuyerID",
                principalTable: "Customers",
                principalColumn: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Customers_SellerID",
                table: "Vehicles",
                column: "SellerID",
                principalTable: "Customers",
                principalColumn: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Manufacturers_ManufacturerID",
                table: "Vehicles",
                column: "ManufacturerID",
                principalTable: "Manufacturers",
                principalColumn: "ManufacturerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Customers_BuyerID",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Customers_SellerID",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Manufacturers_ManufacturerID",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_BuyerID",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_SellerID",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "BuyerID",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "SellerID",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Salesman");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Manufacturers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Branches");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Vehicles",
                newName: "price");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerID",
                table: "Vehicles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CustomerID",
                table: "Vehicles",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Customers_CustomerID",
                table: "Vehicles",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Manufacturers_ManufacturerID",
                table: "Vehicles",
                column: "ManufacturerID",
                principalTable: "Manufacturers",
                principalColumn: "ManufacturerID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
