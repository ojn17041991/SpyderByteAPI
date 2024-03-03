using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpyderByteAPI.Migrations
{
    /// <inheritdoc />
    public partial class Game_Jam_Merge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserJams");

            migrationBuilder.DropTable(
                name: "Jams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaderboardRecords",
                table: "LeaderboardRecords");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "LeaderboardRecords",
                newName: "LeaderboardId");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaderboardRecord_Id",
                table: "LeaderboardRecords",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Leaderboards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaderboards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GameId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGame_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGame_Game",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserGame_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LeaderboardGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LeaderboardId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardGame_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaderboardGame_Leaderboard",
                        column: x => x.LeaderboardId,
                        principalTable: "Leaderboards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LeaderboardGame_User",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardRecords_LeaderboardId",
                table: "LeaderboardRecords",
                column: "LeaderboardId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardGames_GameId",
                table: "LeaderboardGames",
                column: "GameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardGames_LeaderboardId",
                table: "LeaderboardGames",
                column: "LeaderboardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_GameId",
                table: "UserGames",
                column: "GameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_UserId",
                table: "UserGames",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaderboardRecord_Leaderboard",
                table: "LeaderboardRecords",
                column: "LeaderboardId",
                principalTable: "Leaderboards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaderboardRecord_Leaderboard",
                table: "LeaderboardRecords");

            migrationBuilder.DropTable(
                name: "LeaderboardGames");

            migrationBuilder.DropTable(
                name: "UserGames");

            migrationBuilder.DropTable(
                name: "Leaderboards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaderboardRecord_Id",
                table: "LeaderboardRecords");

            migrationBuilder.DropIndex(
                name: "IX_LeaderboardRecords_LeaderboardId",
                table: "LeaderboardRecords");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "LeaderboardId",
                table: "LeaderboardRecords",
                newName: "GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaderboardRecords",
                table: "LeaderboardRecords",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Jams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ImgurImageId = table.Column<string>(type: "TEXT", nullable: false),
                    ImgurUrl = table.Column<string>(type: "TEXT", nullable: false),
                    ItchUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserJams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JamId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJam_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserJam_Jam",
                        column: x => x.JamId,
                        principalTable: "Jams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserJam_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserJams_JamId",
                table: "UserJams",
                column: "JamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserJams_UserId",
                table: "UserJams",
                column: "UserId",
                unique: true);
        }
    }
}
