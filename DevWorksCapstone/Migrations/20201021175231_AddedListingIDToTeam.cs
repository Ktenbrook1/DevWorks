using Microsoft.EntityFrameworkCore.Migrations;

namespace DevWorksCapstone.Migrations
{
    public partial class AddedListingIDToTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ListingId",
                table: "Teams",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_ListingId",
                table: "Teams",
                column: "ListingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Listings_ListingId",
                table: "Teams",
                column: "ListingId",
                principalTable: "Listings",
                principalColumn: "ListingId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Listings_ListingId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_ListingId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "ListingId",
                table: "Teams");
        }
    }
}
