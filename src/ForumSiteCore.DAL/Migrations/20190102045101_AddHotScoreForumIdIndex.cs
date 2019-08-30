using Microsoft.EntityFrameworkCore.Migrations;

namespace ForumSiteCore.DAL.Migrations
{
    public partial class AddHotScoreForumIdIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_posts_forum_id",
                table: "posts");

            migrationBuilder.CreateIndex(
                name: "ix_posts_forum_id_hot_score",
                table: "posts",
                columns: new[] { "forum_id", "hot_score" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_posts_forum_id_hot_score",
                table: "posts");

            migrationBuilder.CreateIndex(
                name: "ix_posts_forum_id",
                table: "posts",
                column: "forum_id");
        }
    }
}
