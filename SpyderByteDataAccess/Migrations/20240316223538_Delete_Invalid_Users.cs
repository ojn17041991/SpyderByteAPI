using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

#nullable disable

namespace SpyderByteAPI.Migrations
{
    /// <inheritdoc />
    public partial class Delete_Invalid_Users : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaderboardGame_User",
                table: "LeaderboardGames");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaderboardGame_Game",
                table: "LeaderboardGames",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");

            migrationBuilder.Operations.Add(
                new SqlOperation
                {
                    Sql = "DELETE FROM Users WHERE UserName = ''"
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaderboardGame_Game",
                table: "LeaderboardGames");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaderboardGame_User",
                table: "LeaderboardGames",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");
        }
    }
}
