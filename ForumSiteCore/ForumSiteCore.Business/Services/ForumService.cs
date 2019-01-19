using AutoMapper;
using ForumSiteCore.Business.Consts;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Business.Models;
using ForumSiteCore.Business.ViewModels;
using ForumSiteCore.DAL;
using ForumSiteCore.DAL.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ForumService> _logger;
        public ForumService(ApplicationDbContext context, IUserAccessor<Int64> userAccessor, UserActivitiesService userActivitiesService, ILogger<ForumService> logger)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userActivitiesService = userActivitiesService;
            _logger = logger;
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

            return PrepareForumPostListing(forumName, posts, LookupConsts.LookupControversial);
        }

        public ForumSearchVM ForumSearch(String search)
        {
            _logger.LogDebug("Searching for forums containing: {0}", search);

            var results = (from f in _context.Forums
                           where EF.Functions.Like(f.Name, String.Format("%{0}%", search))
                           orderby f.Name
                           select f).Take(25);
            IList<String> returnedData;
            String message;
            String status;

            returnedData = results.Select(x => x.Name).ToList();
            if (returnedData.Count == 0)
            {
                returnedData = new List<String>();
                message = "No search results found";
                status = "failure";
            }
            else
            {
                message = $"{returnedData.Count} Search results found";
                status = "success";
            }

            var vm = new ForumSearchVM
            {
                Results = returnedData,
                Status = status,
                Message = message
            };

            return vm;
        }

        public ForumDto Get(Int64 forumId)
        {
            var forum = _context.Forums
                .Include(x => x.User)
                .SingleOrDefault(x => x.Id.Equals(forumId));

            return Mapper.Map<ForumDto>(forum);
        }

        public ForumDto GetByName(String forumName)
        {
            var forum = _context.Forums
                .Include(x => x.User)
                .SingleOrDefault(x => x.Name.Equals(forumName));

            return Mapper.Map<ForumDto>(forum);
        }

        public ForumPostListingVM Hot(String forumName, Int32 postLimit = 25, Decimal? prevHotScore = null)
        {
            var predicate = CreateForumWhereClause(forumName);
            predicate = BuildPagingWhereClauseHot(predicate, prevHotScore);

            var posts = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.HotScore)
                .Take(postLimit)
                .ToList();

            return PrepareForumPostListing(forumName, posts, LookupConsts.LookupHot);
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

            return PrepareForumPostListing(forumName, posts, LookupConsts.LookupNew);
        }

        public ForumSaveVM Save(Int64 forumId)
        {
            if (ForumIsHome(forumId) || ForumIsAll(forumId))
                throw new Exception("Home and All cannot be saved");

            // see if the user already saved this at one point
            if (_userActivitiesService.UserForumsSaved.ContainsKey(forumId))
            {
                // they did save it. Is the save "inactive"?
                if (_userActivitiesService.UserForumsSaved[forumId] == true)
                {
                    // set it to active
                    if (UpdateForumSaveInactive(forumId, _userAccessor.UserId, false))
                    {
                        // update our cache item -- it's saved (active).
                        _userActivitiesService.UserForumsSaved[forumId] = false;
                        return new ForumSaveVM { Status = "success", Saved = true, Message = "ForumSave existed and was set from inactive to active" };
                    }
                }
                else // looks like they want to activate this postsave
                {
                    // take care of it in db
                    if (UpdateForumSaveInactive(forumId, _userAccessor.UserId, true))
                    {
                        // update our cache item
                        _userActivitiesService.UserForumsSaved[forumId] = true;
                        return new ForumSaveVM { Status = "success", Saved = false, Message = "ForumSave existed and was set from active to inactive" };
                    }
                }
            }
            else
            {
                if (AddForumSave(forumId, _userAccessor.UserId))
                {
                    _userActivitiesService.UserForumsSaved.Add(forumId, false);
                    return new ForumSaveVM { Status = "success", Saved = true, Message = "ForumSave was created and set to active" };
                }
            }

            return new ForumSaveVM { Status = "failure", Saved = false, Message = "ForumSave creation failed" };
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

            return PrepareForumPostListing(forumName, posts, LookupConsts.LookupTop);
        }

        private Boolean AddForumSave(Int64 forumId, Int64 userId)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                try
                {
                    var forumSave = new ForumSave();
                    forumSave.Created = forumSave.Updated = DateTimeOffset.Now;

                    forumSave.ForumId = forumId;
                    forumSave.UserId = userId;

                    _context.ForumSaves.Add(forumSave);

                    if (_context.SaveChanges() == 1)
                    {
                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to add ForumSave");
                    transaction.Rollback();
                }
            }

            return false;
        }

        private ExpressionStarter<Post> BuildPagingWhereClauseHot(ExpressionStarter<Post> predicate, decimal? prevHotScore)
        {
            if (prevHotScore.HasValue)
                predicate = predicate.And(x => x.HotScore < prevHotScore);

            return predicate;
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

        private Boolean ForumIsAll(String forumName)
        {
            return forumName.Equals("all", StringComparison.OrdinalIgnoreCase);
        }

        private Boolean ForumIsAll(Int64 id)
        {
            return id == 0;
        }

        private Boolean ForumIsHome(String forumName)
        {
            return forumName.Equals("home", StringComparison.OrdinalIgnoreCase);
        }

        private Boolean ForumIsHome(Int64 id)
        {
            return id == -1;
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

        private ForumPostListingVM PrepareForumPostListing(string forumName, List<Post> posts, String postListingType)
        {
            ForumDto forumDto;
            IList<PostDto> postDtos;

            MapDtos(forumName, posts, out forumDto, out postDtos);
            _userActivitiesService.ProcessPosts(postDtos);
            _userActivitiesService.ProcessForums(new List<ForumDto>{ forumDto });

            var forumPostListing = new ForumPostListingVM
            {
                Forum = forumDto,
                Posts = postDtos,
                ForumListingType = postListingType,
                Message = $"Retrieved {postListingType} items for forum {forumName}",
                Status = "success"
            };

            return forumPostListing;
        }
        private Boolean UpdateForumSaveInactive(Int64 forumId, Int64 userId, Boolean inactive)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                try
                {
                    var forumSave = _context.ForumSaves.SingleOrDefault(x => x.ForumId.Equals(forumId) && x.UserId.Equals(userId));
                    forumSave.Inactive = inactive;
                    forumSave.Updated = DateTimeOffset.Now;

                    if (_context.SaveChanges() == 1)
                    {
                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to toggle ForumSave inactive state");
                    transaction.Rollback();
                }
            }

            return false;
        }
    }
}
