using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentExample.Migrations
{
    public partial class ChangeLookupKeyfielddestination : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LookupKey",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "LookupKey",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 1,
                column: "LookupKey",
                value: "look_up_starter");

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 2,
                column: "LookupKey",
                value: "look_up_plan");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LookupKey",
                table: "Prices");

            migrationBuilder.AddColumn<string>(
                name: "LookupKey",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "LookupKey",
                value: "look_up_starter");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "LookupKey",
                value: "look_up_plan");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "LookupKey",
                value: "");
        }
    }
}
