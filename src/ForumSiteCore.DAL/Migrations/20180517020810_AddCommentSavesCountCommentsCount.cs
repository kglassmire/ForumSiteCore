using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ForumSiteCore.DAL.Migrations
{
    public partial class AddCommentSavesCountCommentsCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "comments_count",
                table: "comments",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "saves_count",
                table: "comments",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "ix_comments_user_id",
                table: "comments",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_comments_asp_net_users_user_id",
                table: "comments",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comments_asp_net_users_user_id",
                table: "comments");

            migrationBuilder.DropIndex(
                name: "ix_comments_user_id",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "comments_count",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "saves_count",
                table: "comments");
        }
    }
}
