using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodBankSystem.Migrations
{
    public partial class SeedAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "AdminId", "Password", "Username" },
                values: new object[] { 1, "123456", "Admin" });
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1);
        }
    }
}
