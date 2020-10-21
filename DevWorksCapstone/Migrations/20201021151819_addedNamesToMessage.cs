using Microsoft.EntityFrameworkCore.Migrations;

namespace DevWorksCapstone.Migrations
{
    public partial class addedNamesToMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeveloperName",
                table: "Message",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployerName",
                table: "Message",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeveloperName",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "EmployerName",
                table: "Message");
        }
    }
}
