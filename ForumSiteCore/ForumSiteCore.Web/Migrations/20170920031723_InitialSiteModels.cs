using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ForumSiteCore.Web.Migrations
{
    public partial class InitialSiteModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "forums",
                columns: table => new
                {
                    id = table.Column<long>(type: "int8", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    created = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    inactive = table.Column<bool>(type: "bool", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    saves = table.Column<long>(type: "int8", nullable: false),
                    updated = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    user_id = table.Column<long>(type: "int8", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_forums", x => x.id);
                    table.ForeignKey(
                        name: "fk_forums_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "forum_saves",
                columns: table => new
                {
                    id = table.Column<long>(type: "int8", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    created = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    forum_id = table.Column<long>(type: "int8", nullable: false),
                    inactive = table.Column<bool>(type: "bool", nullable: false),
                    updated = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    user_id = table.Column<long>(type: "int8", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_forum_saves", x => x.id);
                    table.ForeignKey(
                        name: "fk_forum_saves_forums_forum_id",
                        column: x => x.forum_id,
                        principalTable: "forums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_forum_saves_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    id = table.Column<long>(type: "int8", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    comments_count = table.Column<long>(type: "int8", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    downvotes = table.Column<long>(type: "int8", nullable: false),
                    forum_id = table.Column<long>(type: "int8", nullable: false),
                    hot_score = table.Column<decimal>(type: "numeric", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    saves_count = table.Column<long>(type: "int8", nullable: false),
                    updated = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    upvotes = table.Column<long>(type: "int8", nullable: false),
                    url = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<long>(type: "int8", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_posts", x => x.id);
                    table.ForeignKey(
                        name: "fk_posts_forums_forum_id",
                        column: x => x.forum_id,
                        principalTable: "forums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_posts_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    id = table.Column<long>(type: "int8", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    created = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    parent_id = table.Column<long>(type: "int8", nullable: false),
                    post_id = table.Column<long>(type: "int8", nullable: false),
                    updated = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    user_id = table.Column<long>(type: "int8", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comments", x => x.id);
                    table.ForeignKey(
                        name: "fk_comments_comments_parent_id",
                        column: x => x.parent_id,
                        principalTable: "comments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comments_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_saves",
                columns: table => new
                {
                    id = table.Column<long>(type: "int8", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    created = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    inactive = table.Column<bool>(type: "bool", nullable: false),
                    post_id = table.Column<long>(type: "int8", nullable: false),
                    updated = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    user_id = table.Column<long>(type: "int8", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_saves", x => x.id);
                    table.ForeignKey(
                        name: "fk_post_saves_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_post_saves_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_votes",
                columns: table => new
                {
                    id = table.Column<long>(type: "int8", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    created = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    direction = table.Column<bool>(type: "bool", nullable: false),
                    post_id = table.Column<long>(type: "int8", nullable: false),
                    update = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    user_id = table.Column<long>(type: "int8", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_votes", x => x.id);
                    table.ForeignKey(
                        name: "fk_post_votes_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_post_votes_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comment_saves",
                columns: table => new
                {
                    id = table.Column<long>(type: "int8", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    comment_id = table.Column<long>(type: "int8", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    inactive = table.Column<bool>(type: "bool", nullable: false),
                    updated = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    user_id = table.Column<long>(type: "int8", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comment_saves", x => x.id);
                    table.ForeignKey(
                        name: "fk_comment_saves_comments_comment_id",
                        column: x => x.comment_id,
                        principalTable: "comments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comment_saves_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comment_votes",
                columns: table => new
                {
                    id = table.Column<long>(type: "int8", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    comment_id = table.Column<long>(type: "int8", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    direction = table.Column<bool>(type: "bool", nullable: false),
                    updated = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    user_id = table.Column<long>(type: "int8", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comment_votes", x => x.id);
                    table.ForeignKey(
                        name: "fk_comment_votes_comments_comment_id",
                        column: x => x.comment_id,
                        principalTable: "comments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comment_votes_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_comment_saves_comment_id",
                table: "comment_saves",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "ix_comment_saves_user_id",
                table: "comment_saves",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_comment_votes_comment_id",
                table: "comment_votes",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "ix_comment_votes_user_id",
                table: "comment_votes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_parent_id",
                table: "comments",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_post_id",
                table: "comments",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_forum_saves_forum_id",
                table: "forum_saves",
                column: "forum_id");

            migrationBuilder.CreateIndex(
                name: "ix_forum_saves_user_id",
                table: "forum_saves",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_forums_user_id",
                table: "forums",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_saves_post_id",
                table: "post_saves",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_saves_user_id",
                table: "post_saves",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_votes_post_id",
                table: "post_votes",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_votes_user_id",
                table: "post_votes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_posts_forum_id",
                table: "posts",
                column: "forum_id");

            migrationBuilder.CreateIndex(
                name: "ix_posts_user_id",
                table: "posts",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comment_saves");

            migrationBuilder.DropTable(
                name: "comment_votes");

            migrationBuilder.DropTable(
                name: "forum_saves");

            migrationBuilder.DropTable(
                name: "post_saves");

            migrationBuilder.DropTable(
                name: "post_votes");

            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.DropTable(
                name: "forums");
        }
    }
}
