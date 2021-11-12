using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Migrations
{
    public partial class PizzaSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "MenuItem");

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "ShoppingCart",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "ShoppingCart");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "MenuItem",
                type: "text",
                nullable: true);
        }
    }
}
