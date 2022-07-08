using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TSM.Data.Migrations
{
    public partial class AddHash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Hash",
                table: "CharacterExpiredAuction",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "CharacterExpiredAuction");
        }
    }
}
