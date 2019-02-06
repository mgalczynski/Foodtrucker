using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistency.Migrations
{
    public partial class ConstaintNameAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Foodtrucks_Slug",
                table: "Foodtrucks");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Foodtrucks_slug",
                table: "Foodtrucks",
                column: "Slug");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Foodtrucks_slug",
                table: "Foodtrucks");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Foodtrucks_Slug",
                table: "Foodtrucks",
                column: "Slug");
        }
    }
}
