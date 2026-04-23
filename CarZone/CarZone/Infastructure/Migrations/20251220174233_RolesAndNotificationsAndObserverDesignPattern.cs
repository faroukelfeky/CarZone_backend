using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawerk.Migrations
{
    /// <inheritdoc />
    public partial class RolesAndNotificationsAndObserverDesignPattern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdministratorOf",
                table: "Customers",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Customers",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CustomerManufacturer",
                columns: table => new
                {
                    SubscribedManufacturersManufacturerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscribersCustomerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerManufacturer", x => new { x.SubscribedManufacturersManufacturerID, x.SubscribersCustomerID });
                    table.ForeignKey(
                        name: "FK_CustomerManufacturer_Customers_SubscribersCustomerID",
                        column: x => x.SubscribersCustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerManufacturer_Manufacturers_SubscribedManufacturersManufacturerID",
                        column: x => x.SubscribedManufacturersManufacturerID,
                        principalTable: "Manufacturers",
                        principalColumn: "ManufacturerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerManufacturer_SubscribersCustomerID",
                table: "CustomerManufacturer",
                column: "SubscribersCustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CustomerID",
                table: "Notifications",
                column: "CustomerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerManufacturer");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropColumn(
                name: "AdministratorOf",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Customers");
        }
    }
}
