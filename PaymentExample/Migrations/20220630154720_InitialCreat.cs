using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentExample.Migrations
{
    public partial class InitialCreat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StripeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StripeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LookupKey = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nickname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StripeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitAmount = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prices_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "LookupKey", "Name", "StripeId" },
                values: new object[] { 1, "look_up_starter", "Starter plan", "prod_LxxwZgU3EpFOgn" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "LookupKey", "Name", "StripeId" },
                values: new object[] { 2, "look_up_plan", "Premium plan", "prod_LxxsTRwCIErUG8" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "LookupKey", "Name", "StripeId" },
                values: new object[] { 3, "", "Test product", "prod_Lxw2vUnp50qn0Y" });

            migrationBuilder.InsertData(
                table: "Prices",
                columns: new[] { "Id", "Currency", "Nickname", "ProductId", "StripeId", "UnitAmount" },
                values: new object[] { 1, "usd", "20 USD/per month", 1, "price_1LG20OJnaOlhKwhIPtNbGuPc", 20 });

            migrationBuilder.InsertData(
                table: "Prices",
                columns: new[] { "Id", "Currency", "Nickname", "ProductId", "StripeId", "UnitAmount" },
                values: new object[] { 2, "usd", "100 USD/per month", 2, "price_1LG1wsJnaOlhKwhIeFUE0g0u", 100 });

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ProductId",
                table: "Prices",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
