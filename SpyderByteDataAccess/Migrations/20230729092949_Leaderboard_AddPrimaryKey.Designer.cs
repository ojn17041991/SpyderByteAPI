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
    [Migration("20230729092949_Leaderboard_AddPrimaryKey")]
    partial class Leaderboard_AddPrimaryKey
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.7");

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
#pragma warning restore 612, 618
        }
    }
}
