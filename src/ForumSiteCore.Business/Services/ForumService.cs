using AutoMapper;
using ForumSiteCore.Business.Consts;
using ForumSiteCore.Business.Exceptions;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Business.Models;
using ForumSiteCore.Business.Responses;
using ForumSiteCore.Business.ViewModels;
using ForumSiteCore.DAL;
using ForumSiteCore.DAL.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly CurrentUserActivitiesService _userActivitiesService;
        private readonly ILogger<ForumService> _logger;
        private readonly IMapper _mapper;
        
        public ForumService(ApplicationDbContext context, 
            IUserAccessor<Int64> userAccessor, 
            CurrentUserActivitiesService userActivitiesService,
            IMapper mapper,
            ILogger<ForumService> logger)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userActivitiesService = userActivitiesService;
            _mapper = mapper;
            _logger = logger;
        }

        private ExpressionStarter<Post> CreateForumWhereClause(String forumName)
        {
            var predicate = PredicateBuilder.New<Post>(true);

            if (ForumIsAll(forumName))
            {
                // do all stuff
                _logger.LogInformation("CreateForumWhereClause => ForumIsAll");
            }
            else if (ForumIsHome(forumName))
            {
                // do home stuff
                // get user's forums
                Int64[] ids = { 3, 10, 12 };
                _logger.LogInformation("CreateForumWhereClause => ForumIsHome");
                predicate = predicate.And(x => ids.Contains(x.ForumId));
            }
            else
            {
                predicate = predicate.And(x => x.Forum.Name.Equals(forumName));
            }

            return predicate;
        }

        public ForumPostListingResponse Hot(String forumName, Decimal? ceiling, Decimal? floor, Int32 limit = 25, DateTimeOffset? beginDate = null, DateTimeOffset? endDate = null)
        {
            _logger.LogDebug("Retrieving hot forum post listing for {Forum}", forumName);
            var predicate = CreateForumWhereClause(forumName);
            predicate = BuildPagingWhereClauseHot(predicate, ceiling, floor);
            predicate = BuildDateWhereClause(predicate, beginDate, endDate);
            var posts = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.HotScore)
                .Take(limit)
                .ToList();

            return PrepareForumPostListing(forumName, posts, LookupConsts.LookupHot);
        }

        private ExpressionStarter<Post> BuildDateWhereClause(ExpressionStarter<Post> predicate, DateTimeOffset? beginDate, DateTimeOffset? endDate)
        {
            if (beginDate.HasValue)
                predicate = predicate.And(x => x.Created >= beginDate);

            if (endDate.HasValue)
                predicate = predicate.And(x => x.Created < endDate);

            return predicate;
        }

        private ExpressionStarter<Post> BuildPagingWhereClauseHot(ExpressionStarter<Post> predicate, Decimal? ceiling = null, Decimal? floor = null)
        {
            if (ceiling.HasValue)
                predicate = predicate.And(x => x.HotScore < ceiling);

            if (floor.HasValue)
                predicate = predicate.And(x => x.HotScore >= floor);

            return predicate;
        }

        public ForumPostListingResponse Top(String forumName, Int64? ceiling = null, Int64? floor = null, Int32 limit = 25, DateTimeOffset? dtstart = null, DateTimeOffset? dtend = null)
        {
            _logger.LogDebug("Retrieving top forum post listing for {Forum}", forumName);
            var predicate = CreateForumWhereClause(forumName);
            predicate = BuildPagingWhereClauseTop(predicate, ceiling, floor);
            predicate = BuildDateWhereClause(predicate, dtstart, dtend);

            var posts = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.Upvotes - x.Downvotes)
                .Take(limit)
                .ToList();

            return PrepareForumPostListing(forumName, posts, LookupConsts.LookupTop);
        }

        private ExpressionStarter<Post> BuildPagingWhereClauseTop(ExpressionStarter<Post> predicate, Int64? ceiling = null, Int64? floor = null)
        {
            if (ceiling.HasValue)
                predicate = predicate.And(x => (x.Upvotes - x.Downvotes) < ceiling);

            if (floor.HasValue)
                predicate = predicate.And(x => (x.Upvotes - x.Downvotes) >= floor);

            return predicate;
        }

        public ForumPostListingResponse New(String forumName, DateTimeOffset? ceiling, DateTimeOffset? floor, Int32 limit = 25, DateTimeOffset? dtstart = null, DateTimeOffset? dtend = null)
        {
            _logger.LogDebug("Retrieving new forum post listing for {Forum}", forumName);
            var predicate = CreateForumWhereClause(forumName);
            predicate = BuildPagingWhereClauseNew(predicate, ceiling, floor);
            predicate = BuildDateWhereClause(predicate, dtstart, dtend);

            var posts = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.Created)
                .Take(limit)
                .ToList();

            return PrepareForumPostListing(forumName, posts, LookupConsts.LookupNew);
        }

        private ExpressionStarter<Post> BuildPagingWhereClauseNew(ExpressionStarter<Post> predicate, DateTimeOffset? ceiling = null, DateTimeOffset? floor = null)
        {
            if (ceiling.HasValue)
                predicate = predicate.And(x => x.Created < ceiling);

            if (floor.HasValue)
                predicate = predicate.And(x => x.Created >= floor);

            return predicate;
        }

        public ForumPostListingResponse Controversial(String forumName, Decimal? ceiling, Decimal? floor, Int32 limit = 25, DateTimeOffset? begindate = null, DateTimeOffset? enddate = null)
        {

            _logger.LogDebug("Retrieving controversial forum post listing for {Forum}", forumName);
            var predicate = CreateForumWhereClause(forumName);
            predicate = BuildPagingWhereClauseControversial(predicate, ceiling, floor);
            predicate = BuildDateWhereClause(predicate, begindate, enddate);
            var posts = _context.Posts
                .Include(x => x.User)
                .Include(x => x.Forum)
                .Where(predicate)
                .OrderByDescending(x => x.ControversyScore)
                .Take(limit)
                .ToList();

            return PrepareForumPostListing(forumName, posts, LookupConsts.LookupControversial);
        }

        private ExpressionStarter<Post> BuildPagingWhereClauseControversial(ExpressionStarter<Post> predicate, Decimal? ceiling = null, Decimal? floor = null)
        {
            if (ceiling.HasValue)
                predicate = predicate.And(x => x.ControversyScore < ceiling);

            if (floor.HasValue)
                predicate = predicate.And(x => x.ControversyScore >= floor);

            return predicate;
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

            return _mapper.Map<ForumDto>(forum);
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

            return _mapper.Map<ForumDto>(forum);
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
                    _logger.LogError(e, "Failed to add ForumSave");
                    transaction.Rollback();
                }
            }

            return false;
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
            postDtos = _mapper.Map<List<PostDto>>(posts);
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
                    forumDto = _mapper.Map<ForumDto>(forum);
                }
                else
                {
                    var forum = GetByName(forumName);
                    forumDto = _mapper.Map<ForumDto>(forum);
                }

            }
            
        }

        private ForumPostListingResponse PrepareForumPostListing(string forumName, List<Post> posts, String postListingType)
        {
            ForumDto forumDto;
            List<PostDto> postDtos;

            MapDtos(forumName, posts, out forumDto, out postDtos);
            _userActivitiesService.ProcessPosts(postDtos);
            _userActivitiesService.ProcessForums(new List<ForumDto>{ forumDto });

            var forumPostListing = new ForumPostListingResponse
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
                    _logger.LogError(e, "Failed to toggle ForumSave inactive state");
                    transaction.Rollback();
                }
            }

            return false;
        }
    }
}
