using Microsoft.EntityFrameworkCore.Migrations;

namespace ForumSiteCore.DAL.Migrations
{
    public partial class RemoveNullableFromCommentVotePostId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comment_votes_posts_post_id",
                table: "comment_votes");

            migrationBuilder.AlterColumn<long>(
                name: "post_id",
                table: "comment_votes",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_comment_votes_posts_post_id",
                table: "comment_votes",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comment_votes_posts_post_id",
                table: "comment_votes");

            migrationBuilder.AlterColumn<long>(
                name: "post_id",
                table: "comment_votes",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "fk_comment_votes_posts_post_id",
                table: "comment_votes",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
