using Microsoft.EntityFrameworkCore.Migrations;

namespace ForumSiteCore.DAL.Migrations
{
    public partial class RenameStupidInactivePropertyToSaved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "inactive",
                table: "post_saves",
                newName: "saved");

            migrationBuilder.RenameColumn(
                name: "inactive",
                table: "forum_saves",
                newName: "saved");

            migrationBuilder.RenameColumn(
                name: "inactive",
                table: "comment_saves",
                newName: "saved");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "saved",
                table: "post_saves",
                newName: "inactive");

            migrationBuilder.RenameColumn(
                name: "saved",
                table: "forum_saves",
                newName: "inactive");

            migrationBuilder.RenameColumn(
                name: "saved",
                table: "comment_saves",
                newName: "inactive");
        }
    }
}
