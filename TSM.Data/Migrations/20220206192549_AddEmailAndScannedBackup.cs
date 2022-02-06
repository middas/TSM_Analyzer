using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TSM.Data.Migrations
{
    public partial class AddEmailAndScannedBackup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterMailItem",
                columns: table => new
                {
                    CharacterMailItemID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterID = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemID = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterMailItem", x => x.CharacterMailItemID);
                    table.ForeignKey(
                        name: "FK_CharacterMailItem_Character_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "Character",
                        principalColumn: "CharacterID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScannedBackup",
                columns: table => new
                {
                    ScannedBackupID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BackupPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Duration = table.Column<double>(type: "REAL", nullable: false),
                    ScannedTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannedBackup", x => x.ScannedBackupID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterMailItem_CharacterID",
                table: "CharacterMailItem",
                column: "CharacterID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterMailItem");

            migrationBuilder.DropTable(
                name: "ScannedBackup");
        }
    }
}
