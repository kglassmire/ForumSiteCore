using Microsoft.EntityFrameworkCore.Migrations;

namespace ForumSiteCore.DAL.Migrations
{
    public partial class AddCommentsBestScore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "best_score",
                table: "comments",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "best_score",
                table: "comments");
        }
    }
}
