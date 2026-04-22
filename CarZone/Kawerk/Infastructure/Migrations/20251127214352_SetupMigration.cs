using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawerk.Migrations
{
    /// <inheritdoc />
    public partial class SetupMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    BranchID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Location = table.Column<string>(type: "varchar(500)", nullable: false),
                    Description = table.Column<string>(type: "varchar(2000)", nullable: false),
                    Warranty = table.Column<string>(type: "varchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.BranchID);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", nullable: false),
                    Password = table.Column<string>(type: "varchar(200)", nullable: false),
                    Phone = table.Column<string>(type: "varchar(100)", nullable: false),
                    Address = table.Column<string>(type: "varchar(200)", nullable: false),
                    City = table.Column<string>(type: "varchar(100)", nullable: false),
                    Region = table.Column<string>(type: "varchar(100)", nullable: false),
                    PostalCode = table.Column<string>(type: "varchar(100)", nullable: false),
                    ProfileUrl = table.Column<string>(type: "varchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerID);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    ManufacturerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    Description = table.Column<string>(type: "varchar(2000)", nullable: false),
                    Type = table.Column<string>(type: "varchar(100)", nullable: false),
                    Warranty = table.Column<string>(type: "varchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.ManufacturerID);
                });

            migrationBuilder.CreateTable(
                name: "Salesman",
                columns: table => new
                {
                    SalesmanID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", nullable: false),
                    Password = table.Column<string>(type: "varchar(200)", nullable: false),
                    Phone = table.Column<string>(type: "varchar(100)", nullable: false),
                    Salary = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "varchar(200)", nullable: false),
                    City = table.Column<string>(type: "varchar(100)", nullable: false),
                    Region = table.Column<string>(type: "varchar(100)", nullable: false),
                    BrancheBranchID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salesman", x => x.SalesmanID);
                    table.ForeignKey(
                        name: "FK_Salesman_Branches_BrancheBranchID",
                        column: x => x.BrancheBranchID,
                        principalTable: "Branches",
                        principalColumn: "BranchID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    VehicleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false),
                    Description = table.Column<string>(type: "varchar(2000)", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "varchar(100)", nullable: false),
                    Transmission = table.Column<string>(type: "varchar(100)", nullable: false),
                    Year = table.Column<string>(type: "varchar(100)", nullable: false),
                    SeatingCapacity = table.Column<int>(type: "int", nullable: false),
                    EngineCapacity = table.Column<string>(type: "varchar(100)", nullable: false),
                    FuelType = table.Column<string>(type: "varchar(100)", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", nullable: false),
                    Images = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManufacturerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BranchesBranchID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.VehicleID);
                    table.ForeignKey(
                        name: "FK_Vehicles_Branches_BranchesBranchID",
                        column: x => x.BranchesBranchID,
                        principalTable: "Branches",
                        principalColumn: "BranchID");
                    table.ForeignKey(
                        name: "FK_Vehicles_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID");
                    table.ForeignKey(
                        name: "FK_Vehicles_Manufacturers_ManufacturerID",
                        column: x => x.ManufacturerID,
                        principalTable: "Manufacturers",
                        principalColumn: "ManufacturerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BuyerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SellerCustomerID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SellerManufacturerID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionID);
                    table.ForeignKey(
                        name: "FK_Transactions_Customers_BuyerID",
                        column: x => x.BuyerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Customers_SellerCustomerID",
                        column: x => x.SellerCustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID");
                    table.ForeignKey(
                        name: "FK_Transactions_Manufacturers_SellerManufacturerID",
                        column: x => x.SellerManufacturerID,
                        principalTable: "Manufacturers",
                        principalColumn: "ManufacturerID");
                    table.ForeignKey(
                        name: "FK_Transactions_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Salesman_BrancheBranchID",
                table: "Salesman",
                column: "BrancheBranchID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BuyerID",
                table: "Transactions",
                column: "BuyerID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SellerCustomerID",
                table: "Transactions",
                column: "SellerCustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SellerManufacturerID",
                table: "Transactions",
                column: "SellerManufacturerID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_VehicleID",
                table: "Transactions",
                column: "VehicleID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_BranchesBranchID",
                table: "Vehicles",
                column: "BranchesBranchID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CustomerID",
                table: "Vehicles",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ManufacturerID",
                table: "Vehicles",
                column: "ManufacturerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Salesman");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Manufacturers");
        }
    }
}
