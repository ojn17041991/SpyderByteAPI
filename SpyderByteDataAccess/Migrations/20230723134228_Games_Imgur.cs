using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpyderByteAPI.Migrations
{
    /// <inheritdoc />
    public partial class Games_Imgur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HtmlUrl",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImgurImageId",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImgurUrl",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HtmlUrl",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ImgurImageId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ImgurUrl",
                table: "Games");
        }
    }
}
