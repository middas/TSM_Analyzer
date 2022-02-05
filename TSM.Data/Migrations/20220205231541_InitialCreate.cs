using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TSM.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterAuctionSale");

            migrationBuilder.DropTable(
                name: "CharacterBank");

            migrationBuilder.DropTable(
                name: "CharacterBuy");

            migrationBuilder.DropTable(
                name: "CharacterExpiredAuction");

            migrationBuilder.DropTable(
                name: "CharacterInventory");

            migrationBuilder.DropTable(
                name: "CharacterReagent");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "Character");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Character",
                columns: table => new
                {
                    CharacterID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Class = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Copper = table.Column<long>(type: "INTEGER", nullable: false),
                    Faction = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Realm = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.CharacterID);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    ItemID = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.ItemID);
                });

            migrationBuilder.CreateTable(
                name: "CharacterAuctionSale",
                columns: table => new
                {
                    CharacterAuctionSaleID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterID = table.Column<int>(type: "INTEGER", nullable: false),
                    Copper = table.Column<long>(type: "INTEGER", nullable: false),
                    ItemID = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeOfSale = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterAuctionSale", x => x.CharacterAuctionSaleID);
                    table.ForeignKey(
                        name: "FK_CharacterAuctionSale_Character_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "Character",
                        principalColumn: "CharacterID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterBank",
                columns: table => new
                {
                    CharacterBankID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterID = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemID = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterBank", x => x.CharacterBankID);
                    table.ForeignKey(
                        name: "FK_CharacterBank_Character_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "Character",
                        principalColumn: "CharacterID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterBuy",
                columns: table => new
                {
                    CharacterAuctionBuyID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BoughtTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CharacterID = table.Column<int>(type: "INTEGER", nullable: false),
                    Copper = table.Column<long>(type: "INTEGER", nullable: false),
                    ItemID = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    StackSize = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterBuy", x => x.CharacterAuctionBuyID);
                    table.ForeignKey(
                        name: "FK_CharacterBuy_Character_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "Character",
                        principalColumn: "CharacterID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterExpiredAuction",
                columns: table => new
                {
                    CharacterExpiredAuctionID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterID = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpiredTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ItemID = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    StackSize = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterExpiredAuction", x => x.CharacterExpiredAuctionID);
                    table.ForeignKey(
                        name: "FK_CharacterExpiredAuction_Character_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "Character",
                        principalColumn: "CharacterID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterInventory",
                columns: table => new
                {
                    CharacterInventoryID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterID = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemID = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterInventory", x => x.CharacterInventoryID);
                    table.ForeignKey(
                        name: "FK_CharacterInventory_Character_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "Character",
                        principalColumn: "CharacterID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterReagent",
                columns: table => new
                {
                    CharacterReagentID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterID = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemID = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterReagent", x => x.CharacterReagentID);
                    table.ForeignKey(
                        name: "FK_CharacterReagent_Character_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "Character",
                        principalColumn: "CharacterID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAuctionSale_CharacterID",
                table: "CharacterAuctionSale",
                column: "CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterBank_CharacterID",
                table: "CharacterBank",
                column: "CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterBuy_CharacterID",
                table: "CharacterBuy",
                column: "CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterExpiredAuction_CharacterID",
                table: "CharacterExpiredAuction",
                column: "CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInventory_CharacterID",
                table: "CharacterInventory",
                column: "CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterReagent_CharacterID",
                table: "CharacterReagent",
                column: "CharacterID");
        }
    }
}