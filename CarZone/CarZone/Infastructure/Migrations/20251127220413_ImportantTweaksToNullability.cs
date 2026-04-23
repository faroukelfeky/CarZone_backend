using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kawerk.Migrations
{
    /// <inheritdoc />
    public partial class ImportantTweaksToNullability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salesman_Branches_BrancheBranchID",
                table: "Salesman");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Salesman");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "BrancheBranchID",
                table: "Salesman",
                newName: "BranchID");

            migrationBuilder.RenameIndex(
                name: "IX_Salesman_BrancheBranchID",
                table: "Salesman",
                newName: "IX_Salesman_BranchID");

            migrationBuilder.RenameColumn(
                name: "Region",
                table: "Customers",
                newName: "Username");

            migrationBuilder.AlterColumn<string>(
                name: "Year",
                table: "Vehicles",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Vehicles",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Transmission",
                table: "Vehicles",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Vehicles",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "Images",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FuelType",
                table: "Vehicles",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "EngineCapacity",
                table: "Vehicles",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Vehicles",
                type: "varchar(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Salesman",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Salesman",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Salesman",
                type: "varchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Salesman",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Warranty",
                table: "Manufacturers",
                type: "varchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Manufacturers",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Manufacturers",
                type: "varchar(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileUrl",
                table: "Customers",
                type: "varchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Customers",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Customers",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Customers",
                type: "varchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Customers",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Warranty",
                table: "Branches",
                type: "varchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Branches",
                type: "varchar(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Branches",
                type: "varchar(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(2000)");

            migrationBuilder.AddForeignKey(
                name: "FK_Salesman_Branches_BranchID",
                table: "Salesman",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "BranchID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salesman_Branches_BranchID",
                table: "Salesman");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Salesman");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "BranchID",
                table: "Salesman",
                newName: "BrancheBranchID");

            migrationBuilder.RenameIndex(
                name: "IX_Salesman_BranchID",
                table: "Salesman",
                newName: "IX_Salesman_BrancheBranchID");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Customers",
                newName: "Region");

            migrationBuilder.AlterColumn<string>(
                name: "Year",
                table: "Vehicles",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Vehicles",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Transmission",
                table: "Vehicles",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Vehicles",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Images",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FuelType",
                table: "Vehicles",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EngineCapacity",
                table: "Vehicles",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Vehicles",
                type: "varchar(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Salesman",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Salesman",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Salesman",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Salesman",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Warranty",
                table: "Manufacturers",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Manufacturers",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Manufacturers",
                type: "varchar(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProfileUrl",
                table: "Customers",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Customers",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Customers",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Customers",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Customers",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Warranty",
                table: "Branches",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Branches",
                type: "varchar(500)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Branches",
                type: "varchar(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(2000)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Salesman_Branches_BrancheBranchID",
                table: "Salesman",
                column: "BrancheBranchID",
                principalTable: "Branches",
                principalColumn: "BranchID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
