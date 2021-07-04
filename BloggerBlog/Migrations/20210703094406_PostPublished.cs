using Microsoft.EntityFrameworkCore.Migrations;

namespace BloggerBlog.Migrations
{
    public partial class PostPublished : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PostVisibility",
                table: "Post",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StatusOfPost",
                table: "Post",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostVisibility",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "StatusOfPost",
                table: "Post");
        }
    }
}
