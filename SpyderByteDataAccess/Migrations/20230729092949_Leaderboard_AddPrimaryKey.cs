using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpyderByteAPI.Migrations
{
    /// <inheritdoc />
    public partial class Leaderboard_AddPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "LeaderboardRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaderboardRecords",
                table: "LeaderboardRecords",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaderboardRecords",
                table: "LeaderboardRecords");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LeaderboardRecords");
        }
    }
}
