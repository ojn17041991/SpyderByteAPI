﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpyderByteDataAccess.Contexts;

#nullable disable

namespace SpyderByteAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.10");

            modelBuilder.Entity("SpyderByteAPI.Models.Games.Game", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("HtmlUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ImgurImageId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ImgurUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PublishDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Leaderboard.Leaderboard", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Leaderboards");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Leaderboard.LeaderboardGame", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("GameId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("LeaderboardId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_LeaderboardGame_Id");

                    b.HasIndex("GameId")
                        .IsUnique();

                    b.HasIndex("LeaderboardId")
                        .IsUnique();

                    b.ToTable("LeaderboardGames");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Leaderboard.LeaderboardRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("LeaderboardId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Player")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("Score")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_LeaderboardRecord_Id");

                    b.HasIndex("LeaderboardId");

                    b.ToTable("LeaderboardRecords");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UserType")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Users.UserGame", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("GameId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_UserGame_Id");

                    b.HasIndex("GameId")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserGames");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Leaderboard.LeaderboardGame", b =>
                {
                    b.HasOne("SpyderByteAPI.Models.Games.Game", "Game")
                        .WithOne("LeaderboardGame")
                        .HasForeignKey("SpyderByteAPI.Models.Leaderboard.LeaderboardGame", "GameId")
                        .IsRequired()
                        .HasConstraintName("FK_LeaderboardGame_User");

                    b.HasOne("SpyderByteAPI.Models.Leaderboard.Leaderboard", "Leaderboard")
                        .WithOne("LeaderboardGame")
                        .HasForeignKey("SpyderByteAPI.Models.Leaderboard.LeaderboardGame", "LeaderboardId")
                        .IsRequired()
                        .HasConstraintName("FK_LeaderboardGame_Leaderboard");

                    b.Navigation("Game");

                    b.Navigation("Leaderboard");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Leaderboard.LeaderboardRecord", b =>
                {
                    b.HasOne("SpyderByteAPI.Models.Leaderboard.Leaderboard", "Leaderboard")
                        .WithMany("LeaderboardRecords")
                        .HasForeignKey("LeaderboardId")
                        .IsRequired()
                        .HasConstraintName("FK_LeaderboardRecord_Leaderboard");

                    b.Navigation("Leaderboard");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Users.UserGame", b =>
                {
                    b.HasOne("SpyderByteAPI.Models.Games.Game", "Game")
                        .WithOne("UserGame")
                        .HasForeignKey("SpyderByteAPI.Models.Users.UserGame", "GameId")
                        .IsRequired()
                        .HasConstraintName("FK_UserGame_Game");

                    b.HasOne("SpyderByteAPI.Models.Users.User", "User")
                        .WithOne("UserGame")
                        .HasForeignKey("SpyderByteAPI.Models.Users.UserGame", "UserId")
                        .IsRequired()
                        .HasConstraintName("FK_UserGame_User");

                    b.Navigation("Game");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Games.Game", b =>
                {
                    b.Navigation("LeaderboardGame");

                    b.Navigation("UserGame");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Leaderboard.Leaderboard", b =>
                {
                    b.Navigation("LeaderboardGame")
                        .IsRequired();

                    b.Navigation("LeaderboardRecords");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Users.User", b =>
                {
                    b.Navigation("UserGame");
                });
#pragma warning restore 612, 618
        }
    }
}
