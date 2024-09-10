﻿// <auto-generated />
using System;
using Blob_service.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Blob_service.Migrations
{
    [DbContext(typeof(DeckDbContext))]
    [Migration("20240906111623_BlobTables")]
    partial class BlobTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Blob_service.Models.ActiveHand", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("GameID")
                        .HasColumnType("int");

                    b.Property<string>("LeadingSuit")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<string>("PlayerFiveCard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerFourCard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerOneCard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerSixCard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerThreeCard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerTwoCard")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("ActiveHand");
                });

            modelBuilder.Entity("Blob_service.Models.Bids", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("GameID")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerFiveBid")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerFourBid")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerOneBid")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerSixBid")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerThreeBid")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerTwoBid")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("Bids");
                });

            modelBuilder.Entity("Blob_service.Models.GameDetails", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("BotsPositions")
                        .HasColumnType("int");

                    b.Property<int>("GameID")
                        .HasColumnType("int");

                    b.Property<bool>("NoTrumpsRound")
                        .HasColumnType("bit");

                    b.Property<int>("NumberOfPlayers")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfRounds")
                        .HasColumnType("int");

                    b.Property<bool>("ScoreOnMakingBidOnly")
                        .HasColumnType("bit");

                    b.HasKey("ID");

                    b.ToTable("GameDetails");
                });

            modelBuilder.Entity("Blob_service.Models.Scores", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("GameID")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerFiveScore")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerFourScore")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerOneScore")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerSixScore")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerThreeScore")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerTwoScore")
                        .HasColumnType("int");

                    b.Property<int>("Round")
                        .HasColumnType("int");

                    b.Property<int>("Tricks")
                        .HasColumnType("int");

                    b.Property<string>("TrumpSuit")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.HasKey("ID");

                    b.ToTable("Scores");
                });

            modelBuilder.Entity("Blob_service.Models.SixPlayerHand", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("GameID")
                        .HasColumnType("int");

                    b.Property<string>("PlayerFiveCard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerFourCard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerOneCard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerSixCard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerThreeCard")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerTwoCard")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Hands");
                });
#pragma warning restore 612, 618
        }
    }
}
