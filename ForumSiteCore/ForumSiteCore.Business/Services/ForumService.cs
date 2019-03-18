using AutoMapper;
using ForumSiteCore.Business.Consts;
using ForumSiteCore.Business.Exceptions;
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

            _logger.LogDebug("Retrieving controversial forum post listing for {Forum}", forumName);
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
                           where EF.Functions.Like(f.Name, String.Format("{0}%", search))
                           orderby f.Name
                           select f).Take(25);
            List<String> returnedData;
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

            // if we've gotten this far, this means we never had posts in the first place and we
            // are now seeing if we just have a forum that has never had any posts created
            if (forum == null)
                throw new ForumNotFoundException(String.Format("Forum named {0} not found", forumName));

            return Mapper.Map<ForumDto>(forum);
        }

        public ForumPostListingVM Hot(String forumName, Int32 postLimit = 25, Decimal? prevHotScore = null)
        {
            _logger.LogDebug("Retrieving hot forum post listing for {Forum}", forumName);
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

        public ForumPostListingVM New(String forumName, Int32 postLimit = 25)
        {
            _logger.LogDebug("Retrieving new forum post listing for {Forum}", forumName);
            var predicate = CreateForumWhereClause(forumName);            

            var posts = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.Created)
                .Take(postLimit)
                .ToList();

            return PrepareForumPostListing(forumName, posts, LookupConsts.LookupNew);
        }

        public ForumSaveVM Save(Int64 forumId, Boolean saved)
        {           
            if (ForumIsHome(forumId) || ForumIsAll(forumId))
                throw new Exception("Home and All cannot be saved");

            _logger.LogDebug("Saving Forum {Forum} with value {Saved}", forumId, saved);

            if (_userActivitiesService.GetUserForumsSaved().ContainsKey(forumId))
            {
                _logger.LogDebug("ForumSave already exists, updating value...");
                if (UpdateForumSave(forumId, saved))
                {
                    var forumSaveCache = _userActivitiesService.GetUserForumsSaved();
                    forumSaveCache[forumId] = saved;
                    _userActivitiesService.SetUserForumsSaved(forumSaveCache);

                    ForumSaveVM savedForum = new ForumSaveVM { Status = "success", Saved = saved, Message = String.Format("ForumSave was updated and set to {0}", saved) };
                    _logger.LogDebug("Forum {@Forum} saved", savedForum);
                    return savedForum;
                }
            }
            else
            {
                if (AddForumSave(forumId))
                {
                    var forumSaveCache = _userActivitiesService.GetUserForumsSaved();
                    forumSaveCache.Add(forumId, true);
                    _userActivitiesService.SetUserForumsSaved(forumSaveCache);

                    ForumSaveVM savedForum = new ForumSaveVM { Status = "success", Saved = saved, Message = "ForumSave was created and set to active" };
                    _logger.LogDebug("Forum {@Forum} saved", savedForum);
                    return savedForum;
                }
            }

            return new ForumSaveVM { Status = "failure", Saved = false, Message = "ForumSave creation failed" };
        }

        public ForumPostListingVM Top(String forumName, DateTimeOffset? before = null, DateTimeOffset? after = null, Int32 postLimit = 25)
        {
            _logger.LogDebug("Retrieving top forum post listing for {Forum}", forumName);
            var predicate = CreateForumWhereClause(forumName);
            predicate = BuildPagingWhereClauseTop(predicate, before, after);

            var posts = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.Upvotes - x.Downvotes)
                .ToList();

            return PrepareForumPostListing(forumName, posts, LookupConsts.LookupTop);
        }

        private Boolean AddForumSave(Int64 forumId)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                try
                {
                    var forumSave = new ForumSave();
                    forumSave.Created = forumSave.Updated = DateTimeOffset.Now;

                    forumSave.ForumId = forumId;
                    forumSave.UserId = _userAccessor.UserId;

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

        private ExpressionStarter<Post> BuildPagingWhereClauseTop(ExpressionStarter<Post> predicate, DateTimeOffset? before = null, DateTimeOffset? after = null)
        {
            if (before.HasValue)
                predicate = predicate.And(x => x.Created <= before);

            if (after.HasValue)
                predicate = predicate.And(x => x.Created < after);

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

        private void MapDtos(String forumName, List<Post> posts, out ForumDto forumDto, out List<PostDto> postDtos)
        {
            forumDto = new ForumDto();
            postDtos = Mapper.Map<List<PostDto>>(posts);
            if (ForumIsAll(forumName) || ForumIsHome(forumName))
            {
                foreach (var postDto in postDtos)
                {
                    postDto.ShowForumName = true;
                }
                forumDto = new ForumDto
                {
                    Name = forumName,
                    Id = 0,
                    Created = new DateTimeOffset(new DateTime(2016, 12, 22)),
                    Updated = new DateTimeOffset(new DateTime(2016, 12, 22)),                    
                    Inactive = false,
                    Saves = 0
                };

                if (ForumIsAll(forumName))
                    forumDto.Description = "Posts from all forums!";
                if (ForumIsHome(forumName))
                    forumDto.Description = "Posts from all your subscribed forums!";
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
            
        }

        private ForumPostListingVM PrepareForumPostListing(string forumName, List<Post> posts, String postListingType)
        {
            ForumDto forumDto;
            List<PostDto> postDtos;

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
        private Boolean UpdateForumSave(Int64 forumId, Boolean save)
        {
            using (var transaction = _context.Database.BeginSimpleAmbientTransaction())
            {
                try
                {
                    var forumSave = _context.ForumSaves.SingleOrDefault(x => x.ForumId.Equals(forumId) && x.UserId.Equals(_userAccessor.UserId));
                    forumSave.Saved = save;
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
