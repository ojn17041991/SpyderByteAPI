using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpyderByteAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserJam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserJams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    JamId = table.Column<Guid>(type: "TEXT", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserJams");
        }
    }
}
