using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Migrations
{
    public partial class Cleaup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State",
                table: "AspNetUsers",
                newName: "StreetNumber");

            migrationBuilder.AddColumn<string>(
                name: "HouseNumber",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentUserId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HouseNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PaymentUserId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "StreetNumber",
                table: "AspNetUsers",
                newName: "State");
        }
    }
}
