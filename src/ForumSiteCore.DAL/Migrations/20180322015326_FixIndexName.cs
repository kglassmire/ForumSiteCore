using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ForumSiteCore.DAL.Migrations
{
    public partial class FixIndexName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_forums_name",
                table: "forums",
                newName: "ix_forums_name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "ix_forums_name",
                table: "forums",
                newName: "IX_forums_name");
        }
    }
}
