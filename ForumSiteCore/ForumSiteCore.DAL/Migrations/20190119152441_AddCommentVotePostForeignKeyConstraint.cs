using Microsoft.EntityFrameworkCore.Migrations;

namespace ForumSiteCore.DAL.Migrations
{
    public partial class AddCommentVotePostForeignKeyConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_comment_votes_post_id",
                table: "comment_votes",
                column: "post_id");

            migrationBuilder.AddForeignKey(
                name: "fk_comment_votes_posts_post_id",
                table: "comment_votes",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comment_votes_posts_post_id",
                table: "comment_votes");

            migrationBuilder.DropIndex(
                name: "ix_comment_votes_post_id",
                table: "comment_votes");
        }
    }
}
