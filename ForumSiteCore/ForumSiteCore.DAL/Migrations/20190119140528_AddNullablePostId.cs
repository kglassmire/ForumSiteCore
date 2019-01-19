using Microsoft.EntityFrameworkCore.Migrations;

namespace ForumSiteCore.DAL.Migrations
{
    public partial class AddNullablePostId : Migration
    {
        private const string postIdValues =
@"
ALTER TABLE comment_votes
DISABLE TRIGGER trig_idu_comment_votes_comments_counts_update;


UPDATE comment_votes
SET post_id = comments.post_id
FROM comments
WHERE comments.id = comment_votes.comment_id;

ALTER TABLE comment_votes
ENABLE TRIGGER trig_idu_comment_votes_comments_counts_update;
";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "post_id",
                table: "comment_votes",
                nullable: true);

            migrationBuilder.Sql(postIdValues);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "post_id",
                table: "comment_votes");
        }
    }
}
