﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpyderByteDataAccess.Contexts;

#nullable disable

namespace SpyderByteAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240302162833_UserJam")]
    partial class UserJam
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Jams.Jam", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ImgurImageId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ImgurUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ItchUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PublishDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Jams");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Leaderboard.LeaderboardRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("GameId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Player")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("Score")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("LeaderboardRecords");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Users.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UserType")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Users.UserJam", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("JamId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_UserJam_Id");

                    b.HasIndex("JamId")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserJams");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Users.UserJam", b =>
                {
                    b.HasOne("SpyderByteAPI.Models.Jams.Jam", "Jam")
                        .WithOne("UserJam")
                        .HasForeignKey("SpyderByteAPI.Models.Users.UserJam", "JamId")
                        .IsRequired()
                        .HasConstraintName("FK_UserJam_Jam");

                    b.HasOne("SpyderByteAPI.Models.Users.User", "User")
                        .WithOne("UserJam")
                        .HasForeignKey("SpyderByteAPI.Models.Users.UserJam", "UserId")
                        .IsRequired()
                        .HasConstraintName("FK_UserJam_User");

                    b.Navigation("Jam");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Jams.Jam", b =>
                {
                    b.Navigation("UserJam");
                });

            modelBuilder.Entity("SpyderByteAPI.Models.Users.User", b =>
                {
                    b.Navigation("UserJam");
                });
#pragma warning restore 612, 618
        }
    }
}