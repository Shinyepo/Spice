using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spice.Migrations
{
    public partial class Adress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdressJoined",
                table: "OrderHeader",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HouseNumber",
                table: "OrderHeader",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetAdress",
                table: "OrderHeader",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetNumber",
                table: "OrderHeader",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "AdressJoined",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "HouseNumber",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "StreetAdress",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "StreetNumber",
                table: "OrderHeader");
        }
    }
}
