using Microsoft.EntityFrameworkCore.Migrations;

namespace ForumSiteCore.DAL.Migrations
{
    public partial class AddNullablePostIdCommentSaves : Migration
    {
        private const string postIdValues = 
@"
ALTER TABLE comment_saves
DISABLE TRIGGER trig_idu_comment_saves_comments_saves_count_update;


UPDATE comment_saves
SET post_id = comments.post_id
FROM comments
WHERE comments.id = comment_saves.comment_id;

ALTER TABLE comment_saves
ENABLE TRIGGER trig_idu_comment_saves_comments_saves_count_update;
";
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "post_id",
                table: "comment_saves",
                nullable: true);

            migrationBuilder.Sql(postIdValues);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "post_id",
                table: "comment_saves");
        }
    }
}
