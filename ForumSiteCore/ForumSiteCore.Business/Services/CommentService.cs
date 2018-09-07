using ForumSiteCore.DAL;
using ForumSiteCore.DAL.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForumSiteCore.Business.Services
{
    public class CommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Comment Add(Comment comment)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                var result = false;
                try
                {
                    comment.Created = comment.Updated = DateTimeOffset.Now;
                    _context.Comments.Add(comment);

                    // user automatically upvotes their own post.
                    Upvote(comment.Id, comment.UserId);
                    result = _context.SaveChanges() == 1;
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    result = false;
                    Log.Error(e, "Error while adding comment.");
                    transaction.Rollback();
                }
            }

            return comment;
        }

        public Boolean Save(Int64 commentId, Int64 userId, Boolean saving)
        {
            var commentSave = _context.CommentSaves.SingleOrDefault(x => x.CommentId.Equals(commentId) && x.UserId.Equals(userId));

            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                var result = false;
                try
                {
                    if (commentSave == null)
                    {
                        commentSave = new CommentSave();
                        commentSave.Created = commentSave.Updated = DateTimeOffset.Now;

                        commentSave.CommentId = commentId;
                        commentSave.UserId = userId;
                        _context.CommentSaves.Add(commentSave);
                    }
                    else
                    {
                        if (saving == commentSave.Inactive)
                        {
                            commentSave.Inactive = !saving;
                            commentSave.Updated = DateTimeOffset.Now;
                        }
                    }

                    result = _context.SaveChanges() == 1;
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    result = false;
                    Log.Error(e, "Error while saving comment");
                    transaction.Rollback();
                }

                return result;
            }
        }

        public Boolean Downvote(Int64 commentId, Int64 userId)
        {
            return Vote(commentId, userId, false);
        }

        public Boolean Upvote(Int64 commentId, Int64 userId)
        {
            return Vote(commentId, userId, true);
        }

        internal Boolean Vote(Int64 commentId, Int64 userId, Boolean direction)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                var result = false;
                try
                {
                    var commentVote = _context.CommentVotes.SingleOrDefault(x => x.CommentId.Equals(commentId) && x.UserId.Equals(userId));
                    if (commentVote == null)
                    {
                        commentVote = new CommentVote();
                        commentVote.Created = commentVote.Updated = DateTimeOffset.Now;
                        commentVote.UserId = userId;
                        commentVote.CommentId = commentId;
                        commentVote.Direction = direction;

                        _context.CommentVotes.Add(commentVote);
                    }
                    else
                    {
                        if (commentVote.Direction == direction)
                        {
                            commentVote.Inactive = !commentVote.Inactive;
                            commentVote.Updated = DateTimeOffset.Now;
                        }
                        else
                        {
                            commentVote.Direction = direction;
                            commentVote.Updated = DateTimeOffset.Now;
                        }
                    }
                    result = _context.SaveChanges() == 1;
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    result = false;
                    Log.Error(e, "Error while voting comment");
                    transaction.Rollback();
                    
                }

                return result;
            }

        }
    }
}
