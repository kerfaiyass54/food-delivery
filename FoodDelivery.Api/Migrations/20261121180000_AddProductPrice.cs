using FoodDelivery.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Api.Migrations;

[DbContext(typeof(FoodDeliveryDbContext))]
[Migration("20261121180000_AddProductPrice")]
public partial class AddProductPrice : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "Price",
            table: "Products",
            type: "numeric(18,2)",
            precision: 18,
            scale: 2,
            nullable: true);

        migrationBuilder.AddCheckConstraint(
            name: "CK_Products_Price_Positive_WhenSet",
            table: "Products",
            sql: "\"Price\" IS NULL OR \"Price\" > 0");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropCheckConstraint(
            name: "CK_Products_Price_Positive_WhenSet",
            table: "Products");

        migrationBuilder.DropColumn(
            name: "Price",
            table: "Products");
    }
}
