using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawerk.Migrations
{
    /// <inheritdoc />
    public partial class BranchIDForeignKeyForSalesmen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salesman_Branches_BranchID",
                table: "Salesman");

            migrationBuilder.AlterColumn<Guid>(
                name: "BranchID",
                table: "Salesman",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Salesman_Branches_BranchID",
                table: "Salesman",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "BranchID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salesman_Branches_BranchID",
                table: "Salesman");

            migrationBuilder.AlterColumn<Guid>(
                name: "BranchID",
                table: "Salesman",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Salesman_Branches_BranchID",
                table: "Salesman",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "BranchID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
