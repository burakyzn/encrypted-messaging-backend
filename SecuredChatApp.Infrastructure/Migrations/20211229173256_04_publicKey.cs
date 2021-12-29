using Microsoft.EntityFrameworkCore.Migrations;

namespace SecuredChatApp.Infrastructure.Migrations
{
    public partial class _04_publicKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromPublicKey",
                table: "MessageBoxes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToPublicKey",
                table: "MessageBoxes",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromPublicKey",
                table: "MessageBoxes");

            migrationBuilder.DropColumn(
                name: "ToPublicKey",
                table: "MessageBoxes");
        }
    }
}
