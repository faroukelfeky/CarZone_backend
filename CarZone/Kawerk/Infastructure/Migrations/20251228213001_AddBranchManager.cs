using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawerk.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchManagerCustomerID",
                table: "Branches",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_BranchManagerCustomerID",
                table: "Branches",
                column: "BranchManagerCustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Customers_BranchManagerCustomerID",
                table: "Branches",
                column: "BranchManagerCustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Customers_BranchManagerCustomerID",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_BranchManagerCustomerID",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "BranchManagerCustomerID",
                table: "Branches");
        }
    }
}
