using AutoMapper;
using ForumSiteCore.Business.Models;
using ForumSiteCore.Business.ViewModels;
using ForumSiteCore.DAL;
using ForumSiteCore.DAL.Models;
using ForumSiteCore.Utility;
using LinqKit;
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

        public ForumPostListing Hot(Int64 forumId = 0, Int32 postLimit = 25)
        {
            var predicate = CreateForumWhereClause(forumId);

            var posts = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.HotScore)
                .Take(postLimit)
                .ToList();

            ForumDto forumDto;
            IList<PostDto> postDtos;
            MapDtos(forumId, posts, out forumDto, out postDtos);

            return new ForumPostListing(forumDto, postDtos, Consts.POST_LISTING_TYPE_HOT);
        }

        public ForumPostListing New(DateTimeOffset howFarBack, Int64 forumId = 0, Int32 postLimit = 25)
        {
            var predicate = CreateForumWhereClause(forumId);
            predicate = predicate.And(x => x.Created >= howFarBack);

            var posts = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.Created)
                .Take(postLimit)
                .ToList();

            ForumDto forumDto;
            IList<PostDto> postDtos;
            MapDtos(forumId, posts, out forumDto, out postDtos);

            return new ForumPostListing(forumDto, postDtos, Consts.POST_LISTING_TYPE_NEW);
        }

        public ForumPostListing Top(DateTimeOffset howFarBack, Int64 forumId = 0, Int32 postLimit = 25)
        {
            var predicate = CreateForumWhereClause(forumId);
            predicate = predicate.And(x => x.Created >= howFarBack);

            var query = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.Upvotes - x.Downvotes)
                .ToList();

            ForumDto forumDto;
            IList<PostDto> postDtos;
            MapDtos(forumId, query, out forumDto, out postDtos);

            return new ForumPostListing(forumDto, postDtos, Consts.POST_LISTING_TYPE_TOP);
        }

        public ForumPostListing Controversial(DateTimeOffset howFarBack, Int64 forumId = 0, Int32 postLimit = 25)
        {
            var predicate = CreateForumWhereClause(forumId);
            predicate = predicate.And(x => x.Created >= howFarBack);

            var posts = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.ControversyScore)
                .Take(postLimit)
                .ToList();

            ForumDto forumDto;
            IList<PostDto> postDtos;
            MapDtos(forumId, posts, out forumDto, out postDtos);

            return new ForumPostListing(forumDto, postDtos, Consts.POST_LISTING_TYPE_CONTROVERSIAL);
        }

        public ForumDto Get(Int64 forumId)
        {
            var forum = _context.Forums
                .Include(x => x.User)
                .Single(x => x.Id.Equals(forumId));

            return Mapper.Map<ForumDto>(forum);
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
                catch (Exception e)
                {
                    result = false;
                    Log.Error(e, "Error while saving forum");
                    transaction.Rollback();
                }

                return result;
            }
        }

        private static ExpressionStarter<Post> CreateForumWhereClause(long forumId)
        {
            var predicate = PredicateBuilder.New<Post>();

            if (ForumIsAll(forumId))
            {
                // do all stuff
                Log.Information("CreateForumWhereClause => ForumIsAll");
            }
            else if (ForumIsHome(forumId))
            {
                // do home stuff
                // get user's forums
                Int64[] ids = { 3, 10, 12 };
                Log.Information("CreateForumWhereClause => ForumIsHome");
                predicate = predicate.And(x => ids.Contains(x.ForumId));
            }
            else
            {
                predicate = predicate.And(x => x.ForumId.Equals(forumId));
            }

            return predicate;
        }

        private static void MapDtos(long forumId, List<Post> posts, out ForumDto forumDto, out IList<PostDto> postDtos)
        {
            forumDto = new ForumDto();
            if (ForumIsAll(forumId))
            {
                // do all stuff
                Log.Information("MapDtos => ForumIsAll");
            }
            else if (ForumIsHome(forumId))
            {
                // do home stuff
                Log.Information("MapDtos => ForumIsHome");
            }
            else 
            {
                var forum = posts.FirstOrDefault().Forum;
                forumDto = Mapper.Map<ForumDto>(forum);
            }
            postDtos = Mapper.Map<IList<PostDto>>(posts);
        }

        private static Boolean ForumIsAll(Int64 forumId)
        {
            return forumId.Equals(0);
        }

        private static Boolean ForumIsHome(Int64 forumId)
        {
            return forumId.Equals(-1);
        }

    }
}
