using Microsoft.EntityFrameworkCore.Migrations;

namespace DevWorksCapstone.Migrations
{
    public partial class addinBool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TeamIsAlive",
                table: "Teams",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamIsAlive",
                table: "Teams");
        }
    }
}
