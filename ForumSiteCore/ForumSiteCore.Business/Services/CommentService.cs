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
        private readonly UserActivitiesService _userActivitiesService;

        public CommentService(ApplicationDbContext context, UserActivitiesService userActivitiesService)
        {
            _context = context;
            _userActivitiesService = userActivitiesService;
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
                    Vote(comment.Id, comment.UserId, true);
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
                        if (saving == commentSave.Saved)
                        {
                            commentSave.Saved = saving;
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

        public Boolean Vote(Int64 postId, Int64 userId, Boolean direction)
        {
            return false;
        }
    }
}
