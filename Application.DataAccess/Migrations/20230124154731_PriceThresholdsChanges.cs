using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.DataAccess.Migrations
{
    public partial class PriceThresholdsChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Threshold",
                table: "PriceThresholds",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "PriceThresholdId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxAmount",
                table: "PriceThresholds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinAmount",
                table: "PriceThresholds",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceThresholdId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MaxAmount",
                table: "PriceThresholds");

            migrationBuilder.DropColumn(
                name: "MinAmount",
                table: "PriceThresholds");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PriceThresholds",
                newName: "Threshold");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
