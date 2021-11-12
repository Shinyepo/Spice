using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Migrations
{
    public partial class ESize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "MenuItem",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "MenuItem");
        }
    }
}
