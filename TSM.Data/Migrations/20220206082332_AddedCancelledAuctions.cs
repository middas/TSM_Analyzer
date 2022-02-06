using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TSM.Data.Migrations
{
    public partial class AddedCancelledAuctions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterCancelledAuction",
                columns: table => new
                {
                    CharacterCancelledAuctionID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CancelledTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CharacterID = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemID = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    StackSize = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterCancelledAuction", x => x.CharacterCancelledAuctionID);
                    table.ForeignKey(
                        name: "FK_CharacterCancelledAuction_Character_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "Character",
                        principalColumn: "CharacterID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterCancelledAuction_CharacterID",
                table: "CharacterCancelledAuction",
                column: "CharacterID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterCancelledAuction");
        }
    }
}
