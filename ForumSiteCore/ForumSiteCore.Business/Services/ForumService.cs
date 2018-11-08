using AutoMapper;
using ForumSiteCore.Business.Interfaces;
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
        private readonly IUserAccessor<Int64> _userAccessor;
        private readonly UserActivitiesService _userActivitiesService;
        public ForumService(ApplicationDbContext context, IUserAccessor<Int64> userAccessor, UserActivitiesService userActivitiesService)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userActivitiesService = userActivitiesService;
        }

        public ForumPostListingVM Hot(String forumName, Int32 postLimit = 25)
        {
            var predicate = CreateForumWhereClause(forumName);

            var posts = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.HotScore)
                .Take(postLimit)
                .ToList();

            return PrepareForumPostListing(forumName, posts, Consts.POST_LISTING_TYPE_HOT);
        }

        private ForumPostListingVM PrepareForumPostListing(string forumName, List<Post> posts, String postListingType)
        {
            ForumDto forumDto;
            IList<PostDto> postDtos;

            MapDtos(forumName, posts, out forumDto, out postDtos);
            _userActivitiesService.ProcessPosts(postDtos);

            return new ForumPostListingVM(forumDto, postDtos, postListingType);
        }

        public ForumPostListingVM New(DateTimeOffset howFarBack, String forumName, Int32 postLimit = 25)
        {
            var predicate = CreateForumWhereClause(forumName);
            predicate = predicate.And(x => x.Created >= howFarBack);

            var posts = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.Created)
                .Take(postLimit)
                .ToList();

            return PrepareForumPostListing(forumName, posts, Consts.POST_LISTING_TYPE_NEW);
        }

        public ForumPostListingVM Top(DateTimeOffset howFarBack, String forumName, Int32 postLimit = 25)
        {
            var predicate = CreateForumWhereClause(forumName);
            predicate = predicate.And(x => x.Created >= howFarBack);

            var posts = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.Upvotes - x.Downvotes)
                .ToList();

            return PrepareForumPostListing(forumName, posts, Consts.POST_LISTING_TYPE_TOP);
        }

        public ForumPostListingVM Controversial(DateTimeOffset howFarBack, String forumName, Int32 postLimit = 25)
        {
            var predicate = CreateForumWhereClause(forumName);
            predicate = predicate.And(x => x.Created >= howFarBack);

            var posts = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.ControversyScore)
                .Take(postLimit)
                .ToList();
            
            return PrepareForumPostListing(forumName, posts, Consts.POST_LISTING_TYPE_CONTROVERSIAL);
        }
        
        public IList<String> ForumSearch(String search)
        {
            var results = (from f in _context.Forums                          
                          where EF.Functions.Like(f.Name, String.Format("%{0}%", search))
                          orderby f.Name
                          select f).Take(25);

            return results.Select(x => x.Name).ToList();
        }

        public ForumDto GetByName(String forumName)
        {
            var forum = _context.Forums
                .Include(x => x.User)
                .SingleOrDefault(x => x.Name.Equals(forumName));

            return Mapper.Map<ForumDto>(forum);
        }

        public ForumDto Get(Int64 forumId)
        {
            var forum = _context.Forums
                .Include(x => x.User)
                .SingleOrDefault(x => x.Id.Equals(forumId));

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

        private ExpressionStarter<Post> CreateForumWhereClause(String forumName)
        {
            var predicate = PredicateBuilder.New<Post>(true);
            
            if (ForumIsAll(forumName))
            {
                // do all stuff
                Log.Information("CreateForumWhereClause => ForumIsAll");
            }
            else if (ForumIsHome(forumName))
            {
                // do home stuff
                // get user's forums
                Int64[] ids = { 3, 10, 12 };
                Log.Information("CreateForumWhereClause => ForumIsHome");
                predicate = predicate.And(x => ids.Contains(x.ForumId));
            }
            else
            {
                predicate = predicate.And(x => x.Forum.Name.Equals(forumName));
            }

            return predicate;
        }

        private void MapDtos(String forumName, List<Post> posts, out ForumDto forumDto, out IList<PostDto> postDtos)
        {
            forumDto = new ForumDto();
            if (ForumIsAll(forumName))
            {
                forumDto = new ForumDto
                {
                    Name = "all",
                    Id = 0,
                    Created = new DateTimeOffset(new DateTime(2016, 12, 22)),
                    Updated = new DateTimeOffset(new DateTime(2016, 12, 22)),
                    Description = "All forums",
                    Inactive = false,
                    Saves = 0
                };
                // do all stuff
                Log.Information("MapDtos => ForumIsAll");
            }
            else if (ForumIsHome(forumName))
            {
                // do home stuff
                forumDto = new ForumDto
                {
                    Name = "home",
                    Id = -1,
                    Created = new DateTimeOffset(new DateTime(2016, 12, 22)),
                    Updated = new DateTimeOffset(new DateTime(2016, 12, 22)),
                    Description = "Your forums!",
                    Inactive = false,
                    Saves = 0
                };
                Log.Information("MapDtos => ForumIsHome");
            }
            else 
            {
                
                if (posts.Count > 0)
                {
                    var forum = posts.FirstOrDefault().Forum;
                    forumDto = Mapper.Map<ForumDto>(forum);
                }
                else
                {
                    var forum = GetByName(forumName);
                    forumDto = Mapper.Map<ForumDto>(forum);
                }
                
            }
            postDtos = Mapper.Map<IList<PostDto>>(posts);
        }

        private Boolean ForumIsAll(String forumName)
        {
            return forumName.Equals("all", StringComparison.OrdinalIgnoreCase);
        }

        private Boolean ForumIsHome(String forumName)
        {
            return forumName.Equals("home", StringComparison.OrdinalIgnoreCase);
        }

    }
}
