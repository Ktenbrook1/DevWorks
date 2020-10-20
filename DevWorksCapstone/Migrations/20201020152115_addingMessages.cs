using Microsoft.EntityFrameworkCore.Migrations;

namespace DevWorksCapstone.Migrations
{
    public partial class addingMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5b824ceb-d739-4d9e-88d8-b856d71dd7dd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ce504c2e-812b-4e3e-a187-1feb66832c0b");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "03710c69-41db-4f77-a2a3-56fa8386faa2", "d5ceec89-477e-4988-bc8c-0588e8c998e1", "Developer", "DEVELOPER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "28d835e9-eeb8-4fc5-95c4-558ee06e79ba", "98a15728-4546-4648-879f-eacf3e210399", "Employer", "EMPLOYER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "03710c69-41db-4f77-a2a3-56fa8386faa2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "28d835e9-eeb8-4fc5-95c4-558ee06e79ba");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ce504c2e-812b-4e3e-a187-1feb66832c0b", "958b8606-2b83-4b45-92fe-4e7a68ee6cd0", "Developer", "DEVELOPER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5b824ceb-d739-4d9e-88d8-b856d71dd7dd", "e7242746-8e27-4b61-a115-55497698f0bf", "Employer", "EMPLOYER" });
        }
    }
}
