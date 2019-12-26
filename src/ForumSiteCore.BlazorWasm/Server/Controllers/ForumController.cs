using ForumSiteCore.Business.Consts;
using ForumSiteCore.Business.Services;
using ForumSiteCore.Business.ViewModels;
using ForumSiteCore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ForumSiteCore.BlazorWasm.Server.Controllers
{
    [Route("api/forums")]    
    [ApiController]
    public class ForumController : Controller
    {
        private readonly String[] _acceptedLookups = new[] { "hot", "top", "new", "controversial" };
        private readonly ForumService _forumService;
        private readonly CurrentUserActivitiesService _userActivitiesService;

        public ForumController(ForumService forumService, CurrentUserActivitiesService userActivitiesService)
        {
            _forumService = forumService;
            _userActivitiesService = userActivitiesService;
        }                      
        
        [ResponseCache(VaryByQueryKeys = new[] { "name", "lookup", "ceiling", "floor", "limit", "dtstart", "dtend" }, Duration = 10, Location = ResponseCacheLocation.Any)]
        [HttpGet("{name}/{lookup}")]
        [ProducesResponseType(typeof(ForumPostListingVM), 200)]
        public IActionResult Get(String name, String lookup, String ceiling = null, String floor = null, Int32? limit = null, DateTimeOffset? dtstart = null, DateTimeOffset? dtend = null)
        {            
            if (!_acceptedLookups.Contains(lookup.ToLower()))
                return BadRequest();

            ForumPostListingVM forumPostListingVM = null; ;
            switch (lookup.ToLower())
            {
                case LookupConsts.LookupHot:
                    forumPostListingVM = PrepareHotForumGet(name, ceiling, floor, limit, dtstart, dtend);
                    break;
                case LookupConsts.LookupNew:
                    forumPostListingVM = PrepareNewForumGet(name, ceiling, floor, limit, dtstart, dtend);
                    break;
                case LookupConsts.LookupTop:
                    forumPostListingVM = PrepareTopForumGet(name, ceiling, floor, limit, dtstart, dtend);
                    break;
                case LookupConsts.LookupControversial:
                    forumPostListingVM = PrepareControversialForumGet(name, ceiling, floor, limit, dtstart, dtend);
                    break;
            }

            return Ok(forumPostListingVM);
        }

        private ForumPostListingVM PrepareHotForumGet(String name, String ceiling = null, String floor = null, Int32? limit = null, DateTimeOffset? dtstart = null, DateTimeOffset? dtend = null)
        {
            Decimal? ceilingDecimal = ceiling.ToDecimalOrNull();
            Decimal? floorDecimal = floor.ToDecimalOrNull();

            var forumPostListingVM = _forumService.Hot(name, ceilingDecimal, floorDecimal, 25, dtstart, dtend);
            return forumPostListingVM;
        }

        private ForumPostListingVM PrepareTopForumGet(String name, String ceiling = null, String floor = null, Int32? limit = null, DateTimeOffset? dtstart = null, DateTimeOffset? dtend = null)
        {
            Int64? ceilingLong = ceiling.ToInt64OrNull();
            Int64? floorLong = floor.ToInt64OrNull();

            var forumPostListingVM = _forumService.Top(name, ceilingLong, floorLong, 25, dtstart, dtend);
            return forumPostListingVM;
        }

        private ForumPostListingVM PrepareNewForumGet(String name, String ceiling = null, String floor = null, Int32? limit = null, DateTimeOffset? dtstart = null, DateTimeOffset? dtend = null)
        {
            DateTimeOffset? ceilingDateTimeOffset = ceiling.ToDateTimeOffsetOrNull();
            DateTimeOffset? floorDateTimeOffset = floor.ToDateTimeOffsetOrNull();

            var forumPostListingVM = _forumService.New(name, ceilingDateTimeOffset, floorDateTimeOffset, 25, dtstart, dtend);
            return forumPostListingVM;
        }

        private ForumPostListingVM PrepareControversialForumGet(String name, String ceiling = null, String floor = null, Int32? limit = null, DateTimeOffset? dtstart = null, DateTimeOffset? dtend = null)
        {
            Decimal? ceilingDecimal = ceiling.ToDecimalOrNull();
            Decimal? floorDecimal = floor.ToDecimalOrNull();
            
            var forumPostListingVM = _forumService.Controversial(name, ceilingDecimal, floorDecimal, 25, dtstart, dtend);
            return forumPostListingVM;
        }

        [Authorize]
        [HttpPost("save")]
        [ProducesResponseType(typeof(ForumSaveVM), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        public IActionResult Save([FromBody]ForumSaveVM model)
        {
            var forumSaveVM = _forumService.Save(model.ForumId, model.Saved);

            return Ok(forumSaveVM);
        }

        [HttpGet("search/{search}")]
        [ResponseCache(VaryByQueryKeys = new[] { "*" }, Duration = 60, Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(ForumSearchVM), 200)]        
        public IActionResult ForumSearch(String search)
        {
            var searchResults = _forumService.ForumSearch(search);

            return Ok(searchResults);
        }
    }
}