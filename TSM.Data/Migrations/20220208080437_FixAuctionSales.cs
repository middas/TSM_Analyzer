using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TSM.Data.Migrations
{
    public partial class FixAuctionSales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "CharacterAuctionSale",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StackSize",
                table: "CharacterAuctionSale",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateTime",
                table: "Character",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "CharacterAuctionSale");

            migrationBuilder.DropColumn(
                name: "StackSize",
                table: "CharacterAuctionSale");

            migrationBuilder.DropColumn(
                name: "LastUpdateTime",
                table: "Character");
        }
    }
}
