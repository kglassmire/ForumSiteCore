using ForumSiteCore.Business.Models;
using ForumSiteCore.DAL;
using ForumSiteCore.DAL.Models;
using ForumSiteCore.Utility;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForumSiteCore.Business.Services
{
    public class ForumService
    {
        private readonly ApplicationDbContext _context;
        public ForumService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Forum Get(Int64 forumId)
        {
            var forum = _context.Forums.Single(x => x.Id.Equals(forumId));
            return forum;
        }

        public ForumPostListing Hot(Int64 forumId = 0, Int32 postLimit = 25)
        {
            var forum = Get(forumId);
            var posts = _context.Posts
                .OrderByDescending(x => x.HotScore)
                .Take(postLimit)
                .ToList();

            return new ForumPostListing(forum, posts, Consts.POST_LISTING_TYPE_HOT);
        }

        public ForumPostListing Top(Int64 forumId = 0, Int32 postLimit = 25)
        {
            var forum = Get(forumId);
            var posts = _context.Posts
                .Where(x => x.ForumId.Equals(forumId))
                .OrderByDescending(x => x.Upvotes - x.Downvotes)
                .Take(postLimit)
                .ToList();

            return new ForumPostListing(forum, posts, Consts.POST_LISTING_TYPE_TOP);
        }

        public ForumPostListing New(Int64 forumId = 0, Int32 postLimit = 25)
        {
            var forum = Get(forumId);
            var posts = _context.Posts
                .Where(x => x.ForumId.Equals(forumId))
                .OrderByDescending(x => x.Created)
                .Take(postLimit)
                .ToList();

            return new ForumPostListing(forum, posts, Consts.POST_LISTING_TYPE_NEW);
        }

        public Boolean Save(Int64 forumId, Int64 userId, Boolean saving)
        {            
            var forumSave = _context.ForumSaves.SingleOrDefault(x => x.ForumId.Equals(forumId) && x.UserId.Equals(userId));

            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                var result = false;
                try
                {
                    if (forumSave == null)
                    {
                        forumSave = new ForumSave();
                        forumSave.Created = forumSave.Updated = DateTimeOffset.Now;

                        forumSave.ForumId = forumId;
                        forumSave.UserId = userId;
                        _context.ForumSaves.Add(forumSave);
                    }
                    else
                    {
                        if (saving == forumSave.Inactive)
                        {
                            forumSave.Inactive = !saving;
                            forumSave.Updated = DateTimeOffset.Now;

                        }
                    }

                    result = _context.SaveChanges() == 1;
                    transaction.Commit();
                }
                catch(Exception e)
                {
                    result = false;
                    Log.Error(e, "Error while saving forum");
                    transaction.Rollback();
                }

                return result;
            }                        
        }
    }
}
