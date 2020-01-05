using Microsoft.EntityFrameworkCore.Migrations;

namespace ForumSiteCore.DAL.Migrations
{
    public partial class RemoveNullableFromCommentSavePostId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "post_id",
                table: "comment_saves",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_comment_saves_post_id",
                table: "comment_saves",
                column: "post_id");

            migrationBuilder.AddForeignKey(
                name: "fk_comment_saves_posts_post_id",
                table: "comment_saves",
                column: "post_id",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comment_saves_posts_post_id",
                table: "comment_saves");

            migrationBuilder.DropIndex(
                name: "ix_comment_saves_post_id",
                table: "comment_saves");

            migrationBuilder.AlterColumn<long>(
                name: "post_id",
                table: "comment_saves",
                nullable: true,
                oldClrType: typeof(long));
        }
    }
}
