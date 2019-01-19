using Microsoft.EntityFrameworkCore.Migrations;

namespace ForumSiteCore.DAL.Migrations
{
    public partial class CreateTriggersAndFunctions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:citext", "'citext', '', ''");

            migrationBuilder.Sql(calculate_best);
            migrationBuilder.Sql(calculate_controversy);
            migrationBuilder.Sql(calculate_hot);
            migrationBuilder.Sql(comment_tree);
            migrationBuilder.Sql(maintenance_comments_counts_refresh);
            migrationBuilder.Sql(maintenance_posts_counts_refresh);
            migrationBuilder.Sql(trig_fn_comment_saves_comments_saves_count_update);
            migrationBuilder.Sql(trig_fn_comment_votes_comments_counts_update);
            migrationBuilder.Sql(trig_fn_comments_comments_comments_count_update);
            migrationBuilder.Sql(trig_fn_comments_posts_comments_count_update);
            migrationBuilder.Sql(trig_fn_forum_saves_forums_saves_count_update);
            migrationBuilder.Sql(trig_fn_post_saves_posts_saves_count_update);
            migrationBuilder.Sql(trig_fn_post_votes_posts_counts_update);
            migrationBuilder.Sql(trig_idu_comment_saves_comments_saves_count_update);
            migrationBuilder.Sql(trig_idu_comment_votes_comments_counts_update);
            migrationBuilder.Sql(trig_iu_comments_posts_comments_count_update);
            migrationBuilder.Sql(trig_iu_comments_comments_comments_count_update);
            migrationBuilder.Sql(trig_idu_forum_saves_forums_saves_count_update);
            migrationBuilder.Sql(trig_idu_post_saves_posts_saves_count_update);
            migrationBuilder.Sql(trig_idu_post_votes_posts_count_update);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", "'citext', '', ''")
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.Sql("DROP TRIGGER trig_idu_comment_saves_comments_saves_count_update ON comment_saves");
            migrationBuilder.Sql("DROP TRIGGER trig_idu_comment_votes_comments_counts_update ON comment_votes");
            migrationBuilder.Sql("DROP TRIGGER trig_iu_comments_posts_comments_count_update ON comments");
            migrationBuilder.Sql("DROP TRIGGER trig_iu_comments_comments_comments_count_update ON comments");
            migrationBuilder.Sql("DROP TRIGGER trig_idu_forum_saves_forums_saves_count_update ON forum_saves");
            migrationBuilder.Sql("DROP TRIGGER trig_idu_post_saves_posts_saves_count_update ON post_saves");
            migrationBuilder.Sql("DROP TRIGGER trig_idu_post_votes_posts_count_update ON post_votes");
            migrationBuilder.Sql("DROP FUNCTION calculate_best(bigint, bigint)");
            migrationBuilder.Sql("DROP FUNCTION calculate_controversy(bigint, bigint)");
            migrationBuilder.Sql("DROP FUNCTION calculate_hot(bigint, bigint, timestamptz");
            migrationBuilder.Sql("DROP FUNCTION comment_tree(bigint)");
            migrationBuilder.Sql("DROP FUNCTION maintenance_comments_counts_refresh()");
            migrationBuilder.Sql("DROP FUNCTION maintenance_posts_counts_refresh()");
            migrationBuilder.Sql("DROP FUNCTION trig_fn_comment_saves_comments_saves_count_update()");
            migrationBuilder.Sql("DROP FUNCTION trig_fn_comment_votes_comments_counts_update()");
            migrationBuilder.Sql("DROP FUNCTION trig_fn_comments_comments_comments_count_update()");
            migrationBuilder.Sql("DROP FUNCTION trig_fn_comments_posts_comments_count_update()");
            migrationBuilder.Sql("DROP FUNCTION trig_fn_forum_saves_forums_saves_count_update()");
            migrationBuilder.Sql("DROP FUNCTION trig_fn_post_saves_posts_saves_count_update()");
            migrationBuilder.Sql("DROP FUNCTION trig_fn_post_votes_posts_counts_update()");


        }

        private const string calculate_best =
@"CREATE OR REPLACE FUNCTION public.calculate_best(ups bigint, downs bigint)
 RETURNS double precision
 LANGUAGE plpgsql
 IMMUTABLE
AS $function$
-- wilson score interval conversion
-- https://medium.com/hacking-and-gonzo/how-reddit-ranking-algorithms-work-ef111e33d0d9
-- tested here http://epitools.ausvet.com.au/content.php?page=ciproportion
DECLARE
	z FLOAT;
	n FLOAT;	
	p FLOAT;
	leftside FLOAT;
	rightside FLOAT;
	divisor FLOAT;
BEGIN	
	n := ups + downs;
	 
	IF (n = 0) THEN
		RETURN 0;
	END IF;
	
	z := 1.96;
	p := ups / n;
	
	leftside := p + 1 / (2 * n ) * z * z;
	rightside := z * SQRT(p * ( 1 - p ) / n + z * z / ( 4 * n * n ) );
	divisor := 1 + 1 / n * z * z;
	
	RETURN ( leftside - rightside ) / divisor;
END;
$function$
;
";

        private const string calculate_controversy =
@"CREATE OR REPLACE FUNCTION public.calculate_controversy(ups bigint, downs bigint)
 RETURNS double precision
 LANGUAGE sql
 IMMUTABLE
AS $function$ 
SELECT
	CASE
		WHEN $1 <= 0 OR $2 <= 0 THEN 0
		WHEN $1 > $2 THEN POWER( $1 + $2, CAST( $2 AS FLOAT ) / $1 )
		ELSE POWER( $1 + $2, CAST( $1 as FLOAT ) / $2 )
	END;
$function$
;
";

        private const string calculate_hot =
@"CREATE OR REPLACE FUNCTION public.calculate_hot(ups bigint, downs bigint, date timestamp with time zone)
 RETURNS numeric
 LANGUAGE sql
 IMMUTABLE
AS $function$ 
	SELECT ROUND( CAST( LOG( GREATEST( ABS( $1 - $2 ), 1 )) * SIGN( $1 - $2 ) + ( DATE_PART( 'epoch', date) - 1134028003 ) / 45000.0 as NUMERIC), 7 ) 
$function$
;
";

        private const string comment_tree =
@"CREATE OR REPLACE FUNCTION public.comment_tree(bigint)
 RETURNS TABLE(id bigint, created timestamp with time zone, description text, parent_id bigint, post_id bigint, updated timestamp with time zone, user_id bigint, comments_count bigint, saves_count bigint, best_score numeric, controversy_score numeric, downvotes bigint, upvotes bigint, level integer, path bigint[])
 LANGUAGE sql
AS $function$ 	
WITH RECURSIVE comment_tree AS
(
    SELECT c.*, 0 as level, array[c.id] as path
    FROM comments c
    WHERE c.post_id = $1
        AND c.parent_id IS NULL

    UNION ALL

	SELECT cc.*, level + 1, comment_tree.path || cc.id
	FROM comment_tree
	INNER JOIN comments cc
	ON cc.parent_id = comment_tree.id
	WHERE cc.post_id = $1
)

SELECT * FROM comment_tree;
 $function$
;
";
        private const string maintenance_comments_counts_refresh =
@"CREATE OR REPLACE FUNCTION public.maintenance_comments_counts_refresh()
 RETURNS void
 LANGUAGE sql
AS $function$ 

ALTER TABLE comments DISABLE TRIGGER trig_iu_comments_posts_comments_count_update;
ALTER TABLE comments DISABLE TRIGGER trig_iu_comments_comments_comments_count_update;

UPDATE comments
SET upvotes = 0;

UPDATE comments
SET upvotes = upvotequery.upvotecount
FROM
(
	SELECT comment_votes.comment_id, count(comment_votes.id) upvotecount
	FROM comment_votes 
	WHERE comment_votes.direction = true
		AND comment_votes.inactive = false
	GROUP BY comment_votes.comment_id
) AS upvotequery
WHERE comments.id = upvotequery.comment_id;

UPDATE comments
SET downvotes = 0;

UPDATE comments
SET downvotes = downvotequery.downvotecount
FROM
(
	SELECT comment_votes.comment_id, count(comment_votes.id) downvotecount
	FROM comment_votes 
	WHERE comment_votes.direction = false
		AND comment_votes.inactive = false
	GROUP BY comment_votes.comment_id
) AS downvotequery
WHERE comments.id = downvotequery.comment_id;

UPDATE comments
SET comments_count = 0;

UPDATE comments
SET comments_count = commentscountquery.commentscount
FROM
(
	SELECT parent_id, count(id) commentscount
	FROM comments
	GROUP BY comments.parent_id
) AS commentscountquery
WHERE comments.id = commentscountquery.parent_id;

UPDATE comments
SET saves_count = 0;

UPDATE comments
SET saves_count = savescountquery.savescount
FROM
(
	SELECT comment_id, count(comment_id) savescount
	FROM comment_saves
	WHERE comment_saves.inactive = false
	GROUP BY comment_saves.comment_id
) AS savescountquery
WHERE comments.id = savescountquery.comment_id;

UPDATE comments
SET best_score = 0;

UPDATE comments
SET best_score = calculate_best(bestscorequery.upvotes, bestscorequery.downvotes)
FROM 
(
	SELECT id, upvotes, downvotes 
	FROM comments
) AS bestscorequery
WHERE comments.id = bestscorequery.id;

UPDATE comments
SET controversy_score = 0;

UPDATE comments
SET controversy_score = calculate_controversy(controversyscorequery.upvotes, controversyscorequery.downvotes)
FROM
(
	SELECT id, upvotes, downvotes 
	FROM comments
) AS controversyscorequery
WHERE comments.id = controversyscorequery.id;

ALTER TABLE comments ENABLE TRIGGER trig_iu_comments_posts_comments_count_update;
ALTER TABLE comments ENABLE TRIGGER trig_iu_comments_comments_comments_count_update;

 $function$
;
";
        private const string maintenance_posts_counts_refresh =
@"CREATE OR REPLACE FUNCTION public.maintenance_posts_counts_refresh()
 RETURNS void
 LANGUAGE sql
AS $function$ 
UPDATE posts
SET upvotes = 0;

UPDATE posts 
SET upvotes = upvotequery.upvotecount
FROM
(
	SELECT post_votes.post_id, count(post_votes.id) upvotecount
	FROM post_votes 
	WHERE post_votes.direction = true
		AND post_votes.inactive = false
	GROUP BY post_votes.post_id
) AS upvotequery
WHERE posts.id = upvotequery.post_id;

UPDATE posts
SET downvotes = 0;

UPDATE posts 
SET downvotes = downvotequery.downvotecount
FROM
(
	SELECT post_votes.post_id, count(post_votes.id) downvotecount
	FROM post_votes 
	WHERE post_votes.direction = false
		AND post_votes.inactive = false
	GROUP BY post_votes.post_id
) AS downvotequery
WHERE posts.id = downvotequery.post_id;

UPDATE posts
SET comments_count = 0;

UPDATE posts
SET comments_count = commentscountquery.commentscount
FROM
(
	SELECT post_id, count(post_id) commentscount
	FROM comments
	GROUP BY comments.post_id
) AS commentscountquery
WHERE posts.id = commentscountquery.post_id;

UPDATE posts
SET saves_count = 0;

UPDATE posts
SET saves_count = savescountquery.savescount
FROM
(
	SELECT post_id, count(post_id) savescount
	FROM post_saves
	WHERE post_saves.inactive = false
	GROUP BY post_saves.post_id
) AS savescountquery
WHERE posts.id = savescountquery.post_id;

UPDATE posts
SET hot_score = 0;

UPDATE posts
SET hot_score = calculate_hot(hotscorequery.upvotes, hotscorequery.downvotes, hotscorequery.created)
FROM 
(
	SELECT id, upvotes, downvotes, created 
	FROM posts
) AS hotscorequery
WHERE posts.id = hotscorequery.id;

UPDATE posts
SET controversy_score = 0;

UPDATE posts
SET controversy_score = calculate_controversy(controversyscorequery.upvotes, controversyscorequery.downvotes)
FROM
(
	SELECT id, upvotes, downvotes 
	FROM posts
) AS controversyscorequery
WHERE posts.id = controversyscorequery.id
 $function$
;
";

        private const string trig_fn_comment_saves_comments_saves_count_update =
@"
CREATE OR REPLACE FUNCTION public.trig_fn_comment_saves_comments_saves_count_update()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
  savecount int;  
BEGIN
  IF (TG_OP = 'DELETE') THEN
    savecount := 
			(
				SELECT count(comment_saves.id)
				FROM comment_saves			
				WHERE comment_saves.comment_id = OLD.comment_id
					AND comment_saves.inactive = FALSE
			);

    UPDATE comments
    SET saves_count = savecount
    WHERE id = OLD.comment_id;

    RETURN OLD;
  ELSE
    savecount := 
			(
				SELECT count(comment_saves.id)
				FROM comment_saves
				WHERE comment_saves.comment_id = NEW.comment_id
					AND comment_saves.inactive = FALSE
			);
             
    UPDATE comments
    SET saves_count = savecount
    WHERE id = NEW.comment_id;

    RETURN NEW;
  END IF;  
END;

$function$
;
";

        private const string trig_fn_comment_votes_comments_counts_update = "" +
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
        private const string trig_fn_comments_comments_comments_count_update =
@"
CREATE OR REPLACE FUNCTION public.trig_fn_comments_comments_comments_count_update() 
  RETURNS TRIGGER 
  LANGUAGE plpgsql AS $function$
BEGIN
	IF (TG_OP = 'DELETE') THEN 
		UPDATE comments 
		SET comments_count = 
			(
				SELECT COUNT(comments.id)
				FROM comments
				WHERE comments.parent_id = OLD.id 
			)
		WHERE parent_id = OLD.id;
		RETURN OLD;
	ELSE 
		UPDATE comments 
		SET comments_count = 
			(
				SELECT COUNT(comments.id)
				FROM comments
				WHERE comments.parent_id = NEW.id
			)
		WHERE parent_id = NEW.id;	
		RETURN NEW;
	END IF;
END;
$function$;
";
        private const string trig_fn_comments_posts_comments_count_update =
@"
CREATE OR REPLACE FUNCTION public.trig_fn_comments_posts_comments_count_update()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
BEGIN
	IF (TG_OP = 'DELETE') THEN	
	  UPDATE posts
	  SET comments_count = 
			(
				SELECT count(comments.id)
				FROM comments
				WHERE comments.post_id = OLD.post_id
			)
	  WHERE id = OLD.id;	
	  RETURN OLD;	
	ELSE
	  UPDATE posts
	  SET comments_count = 
			(
				SELECT count(comments.id)
				FROM comments
				WHERE comments.post_id = NEW.post_id
			)
	  WHERE id = NEW.post_id;	
	  RETURN NEW;	
	END IF;

END;
$function$
;
";
        private const string trig_fn_forum_saves_forums_saves_count_update =
@"
CREATE OR REPLACE FUNCTION public.trig_fn_forum_saves_forums_saves_count_update()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
  savecount int;  
BEGIN
  IF (TG_OP = 'DELETE') THEN
		savecount := 
			(
				SELECT count(forum_saves.id)
				FROM forum_saves
				WHERE forum_saves.forum_id = OLD.forum_id 
					AND forum_saves.inactive = FALSE
			);

		UPDATE forums
		SET saves = savecount
		WHERE id = OLD.forum_id;
		RETURN OLD;
	ELSE
		savecount := 
			(
				SELECT count(forum_saves.id)
				FROM forum_saves
				WHERE forum_saves.forum_id = NEW.forum_id 
					AND forum_saves.inactive = FALSE
			);		

		UPDATE forums
		SET saves = savecount
		WHERE id = NEW.forum_id;
		RETURN NEW;
	END IF;
END;

$function$
;
";
        private const string trig_fn_post_saves_posts_saves_count_update =
@"
CREATE OR REPLACE FUNCTION public.trig_fn_post_saves_posts_saves_count_update()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
  savecount int;  
BEGIN
  IF (TG_OP = 'DELETE') THEN
    savecount := 
			(
				SELECT count(post_saves.id)
				FROM post_saves
				WHERE post_saves.post_id = OLD.post_id
					AND post_saves.inactive = FALSE
			);

    UPDATE posts
    SET saves_count = savecount
    WHERE id = OLD.post_id;
    RETURN OLD;
  ELSE
    savecount := 
			(
				SELECT count(post_saves.id)
				FROM post_saves
				WHERE post_saves.post_id = NEW.post_id
					AND post_saves.inactive = FALSE
			);
             
    UPDATE posts
    SET saves_count = savecount
    WHERE id = NEW.post_id;
    RETURN NEW;
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
        private const string trig_idu_comment_saves_comments_saves_count_update =
@"
CREATE
    TRIGGER trig_idu_comment_saves_comments_saves_count_update AFTER INSERT
        OR DELETE
            OR UPDATE
                ON
                public.comment_saves FOR EACH ROW EXECUTE PROCEDURE trig_fn_comment_saves_comments_saves_count_update();
";
        private const string trig_idu_comment_votes_comments_counts_update =
@"
CREATE
    TRIGGER trig_idu_comment_votes_comments_counts_update AFTER INSERT
        OR DELETE
            OR UPDATE
                ON
                public.comment_votes FOR EACH ROW EXECUTE PROCEDURE trig_fn_comment_votes_comments_counts_update();
";
        private const string trig_iu_comments_posts_comments_count_update =
@"
CREATE
    TRIGGER trig_iu_comments_posts_comments_count_update AFTER INSERT
        OR UPDATE
            ON
            public.comments FOR EACH ROW EXECUTE PROCEDURE trig_fn_comments_posts_comments_count_update();
";
        private const string trig_iu_comments_comments_comments_count_update =
@"
CREATE
    TRIGGER trig_iu_comments_comments_comments_count_update AFTER INSERT
        OR UPDATE
            ON
            public.comments FOR EACH ROW EXECUTE PROCEDURE trig_fn_comments_comments_comments_count_update();
";
        private const string trig_idu_forum_saves_forums_saves_count_update =
@"
CREATE
    TRIGGER trig_idu_forum_saves_forums_saves_count_update AFTER INSERT
        OR DELETE
            OR UPDATE
                ON
                public.forum_saves FOR EACH ROW EXECUTE PROCEDURE trig_fn_forum_saves_forums_saves_count_update();
";
        private const string trig_idu_post_saves_posts_saves_count_update =
@"
CREATE
    TRIGGER trig_idu_post_saves_posts_saves_count_update AFTER INSERT
        OR DELETE
            OR UPDATE
                ON
                public.post_saves FOR EACH ROW EXECUTE PROCEDURE trig_fn_post_saves_posts_saves_count_update();
";
        private const string trig_idu_post_votes_posts_count_update =
@"
CREATE
    TRIGGER trig_idu_post_votes_posts_count_update AFTER INSERT
        OR DELETE
            OR UPDATE
                ON
                public.post_votes FOR EACH ROW EXECUTE PROCEDURE trig_fn_post_votes_posts_counts_update();
";
    }
}
