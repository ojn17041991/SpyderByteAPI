using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpyderByteAPI.Migrations
{
    /// <inheritdoc />
    public partial class Jams_ImgurPatchDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "Games");

            migrationBuilder.AddColumn<string>(
                name: "ImgurImageId",
                table: "Jams",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgurImageId",
                table: "Jams");

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
