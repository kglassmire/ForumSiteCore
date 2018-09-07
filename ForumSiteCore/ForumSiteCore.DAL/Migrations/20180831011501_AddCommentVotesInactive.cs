using Microsoft.EntityFrameworkCore.Migrations;

namespace ForumSiteCore.DAL.Migrations
{
    public partial class AddCommentVotesInactive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "inactive",
                table: "comment_votes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "inactive",
                table: "comment_votes");
        }
    }
}
