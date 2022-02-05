﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TSM.Data;

#nullable disable

namespace TSM.Data.Migrations
{
    [DbContext(typeof(SqlLiteDbContext))]
    [Migration("20220205231541_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("TSM.Data.Models.Character", b =>
                {
                    b.Property<int>("CharacterID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Class")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<long>("Copper")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Faction")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Realm")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("CharacterID");

                    b.ToTable("Character");
                });

            modelBuilder.Entity("TSM.Data.Models.CharacterAuctionSale", b =>
                {
                    b.Property<int>("CharacterAuctionSaleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CharacterID")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Copper")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemID")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("TimeOfSale")
                        .HasColumnType("TEXT");

                    b.HasKey("CharacterAuctionSaleID");

                    b.HasIndex("CharacterID");

                    b.ToTable("CharacterAuctionSale");
                });

            modelBuilder.Entity("TSM.Data.Models.CharacterBank", b =>
                {
                    b.Property<int>("CharacterBankID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CharacterID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemID")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.HasKey("CharacterBankID");

                    b.HasIndex("CharacterID");

                    b.ToTable("CharacterBank");
                });

            modelBuilder.Entity("TSM.Data.Models.CharacterBuy", b =>
                {
                    b.Property<int>("CharacterAuctionBuyID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("BoughtTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("CharacterID")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Copper")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemID")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("StackSize")
                        .HasColumnType("INTEGER");

                    b.HasKey("CharacterAuctionBuyID");

                    b.HasIndex("CharacterID");

                    b.ToTable("CharacterBuy");
                });

            modelBuilder.Entity("TSM.Data.Models.CharacterExpiredAuction", b =>
                {
                    b.Property<int>("CharacterExpiredAuctionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CharacterID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ExpiredTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemID")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StackSize")
                        .HasColumnType("INTEGER");

                    b.HasKey("CharacterExpiredAuctionID");

                    b.HasIndex("CharacterID");

                    b.ToTable("CharacterExpiredAuction");
                });

            modelBuilder.Entity("TSM.Data.Models.CharacterInventory", b =>
                {
                    b.Property<int>("CharacterInventoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CharacterID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemID")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.HasKey("CharacterInventoryID");

                    b.HasIndex("CharacterID");

                    b.ToTable("CharacterInventory");
                });

            modelBuilder.Entity("TSM.Data.Models.CharacterReagent", b =>
                {
                    b.Property<int>("CharacterReagentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CharacterID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemID")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.HasKey("CharacterReagentID");

                    b.HasIndex("CharacterID");

                    b.ToTable("CharacterReagent");
                });

            modelBuilder.Entity("TSM.Data.Models.Item", b =>
                {
                    b.Property<string>("ItemID")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("ItemID");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("TSM.Data.Models.CharacterAuctionSale", b =>
                {
                    b.HasOne("TSM.Data.Models.Character", "Character")
                        .WithMany("CharacterAuctionSales")
                        .HasForeignKey("CharacterID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");
                });

            modelBuilder.Entity("TSM.Data.Models.CharacterBank", b =>
                {
                    b.HasOne("TSM.Data.Models.Character", "Character")
                        .WithMany("CharacterBankItems")
                        .HasForeignKey("CharacterID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");
                });

            modelBuilder.Entity("TSM.Data.Models.CharacterBuy", b =>
                {
                    b.HasOne("TSM.Data.Models.Character", "Character")
                        .WithMany("CharacterBuys")
                        .HasForeignKey("CharacterID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");
                });

            modelBuilder.Entity("TSM.Data.Models.CharacterExpiredAuction", b =>
                {
                    b.HasOne("TSM.Data.Models.Character", "Character")
                        .WithMany("CharacterExpiredAuctions")
                        .HasForeignKey("CharacterID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");
                });

            modelBuilder.Entity("TSM.Data.Models.CharacterInventory", b =>
                {
                    b.HasOne("TSM.Data.Models.Character", "Character")
                        .WithMany("CharacterInventoryItems")
                        .HasForeignKey("CharacterID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");
                });

            modelBuilder.Entity("TSM.Data.Models.CharacterReagent", b =>
                {
                    b.HasOne("TSM.Data.Models.Character", "Character")
                        .WithMany("CharacterReagents")
                        .HasForeignKey("CharacterID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");
                });

            modelBuilder.Entity("TSM.Data.Models.Character", b =>
                {
                    b.Navigation("CharacterAuctionSales");

                    b.Navigation("CharacterBankItems");

                    b.Navigation("CharacterBuys");

                    b.Navigation("CharacterExpiredAuctions");

                    b.Navigation("CharacterInventoryItems");

                    b.Navigation("CharacterReagents");
                });
#pragma warning restore 612, 618
        }
    }
}
