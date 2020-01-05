using Microsoft.EntityFrameworkCore.Migrations;

namespace ForumSiteCore.DAL.Migrations
{
    public partial class UpdateVoteTriggerFunctions : Migration
    {
        private const string trig_fn_comment_votes_comments_counts_update =
@"
CREATE OR REPLACE FUNCTION public.trig_fn_comment_votes_comments_counts_update()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
  upcount int;
  downcount int;
BEGIN
	IF (TG_OP = 'DELETE') THEN
		upcount := (
			SELECT count(comment_votes.id)
			FROM comment_votes			
			WHERE comment_votes.comment_id = OLD.comment_id 
				AND comment_votes.direction = TRUE);

		downcount := (
			SELECT count(comment_votes.id)
			FROM comment_votes
			WHERE comment_votes.comment_id = OLD.comment_id 
				AND comment_votes.direction = FALSE);		

		UPDATE comments
		SET upvotes = upcount, 
				downvotes = downcount, 
				best_score = calculate_best(upcount, downcount), 
				controversy_score = calculate_controversy(upcount, downcount) 
		WHERE id = OLD.comment_id;

		RETURN OLD;
	ELSE
		upcount := (
			SELECT count(comment_votes.id)
			FROM comment_votes
			WHERE comment_votes.comment_id = NEW.comment_id 
				AND comment_votes.direction = TRUE);

		downcount := (
			SELECT count(comment_votes.id)
			FROM comment_votes
			WHERE comment_votes.comment_id = NEW.comment_id 
				AND comment_votes.direction = FALSE);

		UPDATE comments
		SET upvotes = upcount, 
				downvotes = downcount, 
				best_score = calculate_best(upcount, downcount), 
				controversy_score = calculate_controversy(upcount, downcount)
		WHERE id = NEW.comment_id;

		RETURN new;
	END IF;
END;
$function$
;
";

        private const string trig_fn_post_votes_posts_counts_update =
@"
CREATE OR REPLACE FUNCTION public.trig_fn_post_votes_posts_counts_update()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
  upcount int;
  downcount int;
BEGIN
	IF (TG_OP = 'DELETE') THEN
		upcount := (
			SELECT count(post_votes.id)
			FROM post_votes
			WHERE post_votes.post_id = OLD.post_id 
				AND post_votes.direction = TRUE);

		downcount := (
			SELECT count(post_votes.id)
			FROM post_votes
			WHERE post_votes.post_id = OLD.post_id 
				AND post_votes.direction = FALSE);		

		UPDATE posts
		SET upvotes = upcount, 
				downvotes = downcount, 
				hot_score = calculate_hot(upcount, downcount, posts.created), 
				controversy_score = calculate_controversy(upcount, downcount) 
		WHERE id = OLD.post_id;

		RETURN OLD;
	ELSE
		upcount := (
			SELECT count(post_votes.id)
			FROM post_votes
			WHERE post_votes.post_id = NEW.post_id 
				AND post_votes.direction = TRUE);

		downcount := (
			SELECT count(post_votes.id)
			FROM post_votes
			WHERE post_votes.post_id = NEW.post_id 
				AND post_votes.direction = FALSE);

		UPDATE posts
		SET upvotes = upcount, 
				downvotes = downcount, 
				hot_score = calculate_hot(upcount, downcount, posts.created), 
				controversy_score = calculate_controversy(upcount, downcount)
		WHERE id = NEW.post_id;

		RETURN NEW;
	END IF;
END;
$function$
;
";

        private const string trig_fn_comment_votes_comments_counts_update_down = "" +
@"
CREATE OR REPLACE FUNCTION public.trig_fn_comment_votes_comments_counts_update()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
  upcount int;
  downcount int;
BEGIN
	IF (TG_OP = 'DELETE') THEN
		upcount := (
			SELECT count(comment_votes.id)
			FROM comment_votes			
			WHERE comment_votes.comment_id = OLD.comment_id 
				AND comment_votes.direction = TRUE
				AND comment_votes.inactive = FALSE);

		downcount := (
			SELECT count(comment_votes.id)
			FROM comment_votes
			WHERE comment_votes.comment_id = OLD.comment_id 
				AND comment_votes.direction = FALSE
				AND comment_votes.inactive = FALSE);		

		UPDATE comments
		SET upvotes = upcount, 
				downvotes = downcount, 
				best_score = calculate_best(upcount, downcount), 
				controversy_score = calculate_controversy(upcount, downcount) 
		WHERE id = OLD.comment_id;

		RETURN OLD;
	ELSE
		upcount := (
			SELECT count(comment_votes.id)
			FROM comment_votes
			WHERE comment_votes.comment_id = NEW.comment_id 
				AND comment_votes.direction = TRUE
				AND comment_votes.inactive = FALSE);

		downcount := (
			SELECT count(comment_votes.id)
			FROM comment_votes
			WHERE comment_votes.comment_id = NEW.comment_id 
				AND comment_votes.direction = FALSE
				AND comment_votes.inactive = FALSE);

		UPDATE comments
		SET upvotes = upcount, 
				downvotes = downcount, 
				best_score = calculate_best(upcount, downcount), 
				controversy_score = calculate_controversy(upcount, downcount)
		WHERE id = NEW.comment_id;

		RETURN new;
	END IF;
END;
$function$
;
";

        private const string trig_fn_post_votes_posts_counts_update_down =
@"
CREATE OR REPLACE FUNCTION public.trig_fn_post_votes_posts_counts_update()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
  upcount int;
  downcount int;
BEGIN
	IF (TG_OP = 'DELETE') THEN
		upcount := (
			SELECT count(post_votes.id)
			FROM post_votes
			WHERE post_votes.post_id = OLD.post_id 
				AND post_votes.direction = TRUE
				AND post_votes.inactive = FALSE);

		downcount := (
			SELECT count(post_votes.id)
			FROM post_votes
			WHERE post_votes.post_id = OLD.post_id 
				AND post_votes.direction = FALSE
				AND post_votes.inactive = FALSE);		

		UPDATE posts
		SET upvotes = upcount, 
				downvotes = downcount, 
				hot_score = calculate_hot(upcount, downcount, posts.created), 
				controversy_score = calculate_controversy(upcount, downcount) 
		WHERE id = OLD.post_id;

		RETURN OLD;
	ELSE
		upcount := (
			SELECT count(post_votes.id)
			FROM post_votes
			WHERE post_votes.post_id = NEW.post_id 
				AND post_votes.direction = TRUE
				AND post_votes.inactive = FALSE);

		downcount := (
			SELECT count(post_votes.id)
			FROM post_votes
			WHERE post_votes.post_id = NEW.post_id 
				AND post_votes.direction = FALSE
				AND post_votes.inactive = FALSE);

		UPDATE posts
		SET upvotes = upcount, 
				downvotes = downcount, 
				hot_score = calculate_hot(upcount, downcount, posts.created), 
				controversy_score = calculate_controversy(upcount, downcount)
		WHERE id = NEW.post_id;

		RETURN NEW;
	END IF;
END;
$function$
;
";
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(trig_fn_comment_votes_comments_counts_update);
            migrationBuilder.Sql(trig_fn_post_votes_posts_counts_update);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(trig_fn_comment_votes_comments_counts_update_down);
            migrationBuilder.Sql(trig_fn_post_votes_posts_counts_update_down);
        }
    }
}
