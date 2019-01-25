using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistency.Migrations
{
    public partial class ColumnNameChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presences_Foodtrucks_FoodTruckId",
                table: "Presences");

            migrationBuilder.RenameColumn(
                name: "FoodTruckId",
                table: "Presences",
                newName: "FoodtruckId");

            migrationBuilder.RenameIndex(
                name: "IX_Presences_FoodTruckId",
                table: "Presences",
                newName: "IX_Presences_FoodtruckId");

            migrationBuilder.AddForeignKey(
                name: "FK_Presences_Foodtrucks_FoodtruckId",
                table: "Presences",
                column: "FoodtruckId",
                principalTable: "Foodtrucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presences_Foodtrucks_FoodtruckId",
                table: "Presences");

            migrationBuilder.RenameColumn(
                name: "FoodtruckId",
                table: "Presences",
                newName: "FoodTruckId");

            migrationBuilder.RenameIndex(
                name: "IX_Presences_FoodtruckId",
                table: "Presences",
                newName: "IX_Presences_FoodTruckId");

            migrationBuilder.AddForeignKey(
                name: "FK_Presences_Foodtrucks_FoodTruckId",
                table: "Presences",
                column: "FoodTruckId",
                principalTable: "Foodtrucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
