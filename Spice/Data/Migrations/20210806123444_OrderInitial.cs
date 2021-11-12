using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Migrations
{
    public partial class OrderInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "SubCategory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "MenuItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Category",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "SubCategory");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "MenuItem");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Category");
        }
    }
}
