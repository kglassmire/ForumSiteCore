using Microsoft.EntityFrameworkCore.Migrations;

namespace ForumSiteCore.DAL.Migrations
{
    public partial class AddCommentsControversyScore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "controversy_score",
                table: "comments",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "controversy_score",
                table: "comments");
        }
    }
}
