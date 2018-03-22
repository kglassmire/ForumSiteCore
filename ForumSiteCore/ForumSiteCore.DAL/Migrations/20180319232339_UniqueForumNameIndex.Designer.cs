﻿// <auto-generated />
using ForumSiteCore.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace ForumSiteCore.DAL.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20180319232339_UniqueForumNameIndex")]
    partial class UniqueForumNameIndex
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:PostgresExtension:citext", "'citext', '', ''")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("ForumSiteCore.DAL.Models.ApplicationRole", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnName("normalized_name")
                        .HasMaxLength(256);

                    b.HasKey("Id")
                        .HasName("pk_asp_net_roles");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("role_name_index");

                    b.ToTable("asp_net_roles");
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.ApplicationUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnName("access_failed_count");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("Email")
                        .HasColumnName("email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnName("email_confirmed");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnName("lockout_enabled");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnName("lockout_end");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnName("normalized_email")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnName("normalized_user_name")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnName("password_hash");

                    b.Property<string>("PhoneNumber")
                        .HasColumnName("phone_number");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnName("phone_number_confirmed");

                    b.Property<string>("SecurityStamp")
                        .HasColumnName("security_stamp");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnName("two_factor_enabled");

                    b.Property<string>("UserName")
                        .HasColumnName("user_name")
                        .HasMaxLength(256);

                    b.HasKey("Id")
                        .HasName("pk_asp_net_users");

                    b.HasIndex("NormalizedEmail")
                        .HasName("email_index");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("user_name_index");

                    b.ToTable("asp_net_users");
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.Comment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnName("created");

                    b.Property<string>("Description")
                        .HasColumnName("description");

                    b.Property<long?>("ParentId")
                        .HasColumnName("parent_id");

                    b.Property<long>("PostId")
                        .HasColumnName("post_id");

                    b.Property<DateTimeOffset>("Updated")
                        .HasColumnName("updated");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_comments");

                    b.HasIndex("ParentId")
                        .HasName("ix_comments_parent_id");

                    b.HasIndex("PostId")
                        .HasName("ix_comments_post_id");

                    b.ToTable("comments");
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.CommentSave", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<long>("CommentId")
                        .HasColumnName("comment_id");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnName("created");

                    b.Property<bool>("Inactive")
                        .HasColumnName("inactive");

                    b.Property<DateTimeOffset>("Updated")
                        .HasColumnName("updated");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_comment_saves");

                    b.HasIndex("CommentId")
                        .HasName("ix_comment_saves_comment_id");

                    b.HasIndex("UserId")
                        .HasName("ix_comment_saves_user_id");

                    b.ToTable("comment_saves");
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.CommentVote", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<long>("CommentId")
                        .HasColumnName("comment_id");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnName("created");

                    b.Property<bool>("Direction")
                        .HasColumnName("direction");

                    b.Property<DateTimeOffset>("Updated")
                        .HasColumnName("updated");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_comment_votes");

                    b.HasIndex("CommentId")
                        .HasName("ix_comment_votes_comment_id");

                    b.HasIndex("UserId")
                        .HasName("ix_comment_votes_user_id");

                    b.ToTable("comment_votes");
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.Forum", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnName("created");

                    b.Property<string>("Description")
                        .HasColumnName("description");

                    b.Property<bool>("Inactive")
                        .HasColumnName("inactive");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("citext");

                    b.Property<long>("Saves")
                        .HasColumnName("saves");

                    b.Property<DateTimeOffset>("Updated")
                        .HasColumnName("updated");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_forums");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .HasName("ix_forums_user_id");

                    b.ToTable("forums");
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.ForumSave", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnName("created");

                    b.Property<long>("ForumId")
                        .HasColumnName("forum_id");

                    b.Property<bool>("Inactive")
                        .HasColumnName("inactive");

                    b.Property<DateTimeOffset>("Updated")
                        .HasColumnName("updated");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_forum_saves");

                    b.HasIndex("ForumId")
                        .HasName("ix_forum_saves_forum_id");

                    b.HasIndex("UserId")
                        .HasName("ix_forum_saves_user_id");

                    b.ToTable("forum_saves");
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.Post", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<long>("CommentsCount")
                        .HasColumnName("comments_count");

                    b.Property<decimal>("ControversyScore")
                        .HasColumnName("controversy_score");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnName("created");

                    b.Property<string>("Description")
                        .HasColumnName("description");

                    b.Property<long>("Downvotes")
                        .HasColumnName("downvotes");

                    b.Property<long>("ForumId")
                        .HasColumnName("forum_id");

                    b.Property<decimal>("HotScore")
                        .HasColumnName("hot_score");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<long>("SavesCount")
                        .HasColumnName("saves_count");

                    b.Property<DateTimeOffset>("Updated")
                        .HasColumnName("updated");

                    b.Property<long>("Upvotes")
                        .HasColumnName("upvotes");

                    b.Property<string>("Url")
                        .HasColumnName("url");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_posts");

                    b.HasIndex("ForumId")
                        .HasName("ix_posts_forum_id");

                    b.HasIndex("UserId")
                        .HasName("ix_posts_user_id");

                    b.ToTable("posts");
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.PostSave", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnName("created");

                    b.Property<bool>("Inactive")
                        .HasColumnName("inactive");

                    b.Property<long>("PostId")
                        .HasColumnName("post_id");

                    b.Property<DateTimeOffset>("Updated")
                        .HasColumnName("updated");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_post_saves");

                    b.HasIndex("PostId")
                        .HasName("ix_post_saves_post_id");

                    b.HasIndex("UserId")
                        .HasName("ix_post_saves_user_id");

                    b.ToTable("post_saves");
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.PostVote", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnName("created");

                    b.Property<bool>("Direction")
                        .HasColumnName("direction");

                    b.Property<long>("PostId")
                        .HasColumnName("post_id");

                    b.Property<DateTimeOffset>("Updated")
                        .HasColumnName("updated");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_post_votes");

                    b.HasIndex("PostId")
                        .HasName("ix_post_votes_post_id");

                    b.HasIndex("UserId")
                        .HasName("ix_post_votes_user_id");

                    b.ToTable("post_votes");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("ClaimType")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnName("claim_value");

                    b.Property<long>("RoleId")
                        .HasColumnName("role_id");

                    b.HasKey("Id")
                        .HasName("pk_asp_net_role_claims");

                    b.HasIndex("RoleId")
                        .HasName("ix_asp_net_role_claims_role_id");

                    b.ToTable("asp_net_role_claims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("ClaimType")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnName("claim_value");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_asp_net_user_claims");

                    b.HasIndex("UserId")
                        .HasName("ix_asp_net_user_claims_user_id");

                    b.ToTable("asp_net_user_claims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<long>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnName("login_provider");

                    b.Property<string>("ProviderKey")
                        .HasColumnName("provider_key");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnName("provider_display_name");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("LoginProvider", "ProviderKey")
                        .HasName("pk_asp_net_user_logins");

                    b.HasIndex("UserId")
                        .HasName("ix_asp_net_user_logins_user_id");

                    b.ToTable("asp_net_user_logins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<long>", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.Property<long>("RoleId")
                        .HasColumnName("role_id");

                    b.HasKey("UserId", "RoleId")
                        .HasName("pk_asp_net_user_roles");

                    b.HasIndex("RoleId")
                        .HasName("ix_asp_net_user_roles_role_id");

                    b.ToTable("asp_net_user_roles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<long>", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnName("user_id");

                    b.Property<string>("LoginProvider")
                        .HasColumnName("login_provider");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<string>("Value")
                        .HasColumnName("value");

                    b.HasKey("UserId", "LoginProvider", "Name")
                        .HasName("pk_asp_net_user_tokens");

                    b.ToTable("asp_net_user_tokens");
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.Comment", b =>
                {
                    b.HasOne("ForumSiteCore.DAL.Models.Comment", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId")
                        .HasConstraintName("fk_comments_comments_parent_id");

                    b.HasOne("ForumSiteCore.DAL.Models.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .HasConstraintName("fk_comments_posts_post_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.CommentSave", b =>
                {
                    b.HasOne("ForumSiteCore.DAL.Models.Comment", "Comment")
                        .WithMany("CommentSaves")
                        .HasForeignKey("CommentId")
                        .HasConstraintName("fk_comment_saves_comments_comment_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ForumSiteCore.DAL.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_comment_saves_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.CommentVote", b =>
                {
                    b.HasOne("ForumSiteCore.DAL.Models.Comment", "Comment")
                        .WithMany("CommentVotes")
                        .HasForeignKey("CommentId")
                        .HasConstraintName("fk_comment_votes_comments_comment_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ForumSiteCore.DAL.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_comment_votes_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.Forum", b =>
                {
                    b.HasOne("ForumSiteCore.DAL.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_forums_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.ForumSave", b =>
                {
                    b.HasOne("ForumSiteCore.DAL.Models.Forum", "Forum")
                        .WithMany("ForumSaves")
                        .HasForeignKey("ForumId")
                        .HasConstraintName("fk_forum_saves_forums_forum_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ForumSiteCore.DAL.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_forum_saves_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.Post", b =>
                {
                    b.HasOne("ForumSiteCore.DAL.Models.Forum", "Forum")
                        .WithMany("Posts")
                        .HasForeignKey("ForumId")
                        .HasConstraintName("fk_posts_forums_forum_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ForumSiteCore.DAL.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_posts_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.PostSave", b =>
                {
                    b.HasOne("ForumSiteCore.DAL.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .HasConstraintName("fk_post_saves_posts_post_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ForumSiteCore.DAL.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_post_saves_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ForumSiteCore.DAL.Models.PostVote", b =>
                {
                    b.HasOne("ForumSiteCore.DAL.Models.Post", "Post")
                        .WithMany("PostVotes")
                        .HasForeignKey("PostId")
                        .HasConstraintName("fk_post_votes_posts_post_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ForumSiteCore.DAL.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_post_votes_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>", b =>
                {
                    b.HasOne("ForumSiteCore.DAL.Models.ApplicationRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_asp_net_role_claims_asp_net_roles_role_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<long>", b =>
                {
                    b.HasOne("ForumSiteCore.DAL.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_asp_net_user_claims_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<long>", b =>
                {
                    b.HasOne("ForumSiteCore.DAL.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_asp_net_user_logins_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<long>", b =>
                {
                    b.HasOne("ForumSiteCore.DAL.Models.ApplicationRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_asp_net_user_roles_asp_net_roles_role_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ForumSiteCore.DAL.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_asp_net_user_roles_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<long>", b =>
                {
                    b.HasOne("ForumSiteCore.DAL.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_asp_net_user_tokens_asp_net_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
