using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumSiteCore.Business.ViewModels;
using ForumSiteCore.Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using ForumSiteCore.Business.Responses;
using ForumSiteCore.Utility;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Business.Consts;

namespace ForumSiteCore.Web.Controllers
{
    [Route("api/forums")]    
    [ApiController]
    public class ForumApiController : ControllerBase
    {
        private readonly String[] _acceptedLookups = new[] { "hot", "top", "new", "controversial" };
        private readonly ForumService _forumService;
        private readonly UserActivitiesService _userActivitiesService;

        public ForumApiController(ForumService forumService, UserActivitiesService userActivitiesService)
        {
            _forumService = forumService;
            _userActivitiesService = userActivitiesService;
        }                      
        
        [ResponseCache(VaryByQueryKeys = new[] { "name", "lookup", "ceiling", "floor", "limit" }, Duration = 10, Location = ResponseCacheLocation.Any)]
        [HttpGet("{name}/{lookup}")]
        [ProducesResponseType(typeof(ForumPostListingVM), 200)]
        public IActionResult Get(String name, String lookup, String ceiling = null, String floor = null, String limit = null, String lookback = null)
        {            
            if (!_acceptedLookups.Contains(lookup.ToLower()))
                return BadRequest();

            ForumPostListingVM forumPostListingVM = null; ;
            switch (lookup.ToLower())
            {
                case LookupConsts.LookupHot:
                    forumPostListingVM = PrepareHotForumGet(name, ceiling, floor, limit);
                    break;
                case LookupConsts.LookupNew:
                    forumPostListingVM = PrepareNewForumGet(name, ceiling, floor, limit);
                    break;
                case LookupConsts.LookupTop:
                    forumPostListingVM = PrepareTopForumGet(name, ceiling, floor, limit);
                    break;
                case LookupConsts.LookupControversial:
                    forumPostListingVM = PrepareControversialForumGet(name, ceiling, floor, limit);
                    break;
            }

            return Ok(forumPostListingVM);
        }

        private ForumPostListingVM PrepareHotForumGet(String name, String ceiling = null, String floor = null, String limit = null)
        {
            Decimal? ceilingDecimal = ceiling.ToDecimalOrNull();
            Decimal? floorDecimal = floor.ToDecimalOrNull();

            var forumPostListingVM = _forumService.Hot(name, ceilingDecimal, floorDecimal, 25);
            return forumPostListingVM;
        }

        private ForumPostListingVM PrepareTopForumGet(String name, String ceiling = null, String floor = null, String limit = null)
        {
            Int64? ceilingLong = ceiling.ToInt64OrNull();
            Int64? floorLong = floor.ToInt64OrNull();

            var forumPostListingVM = _forumService.Top(name, ceilingLong, floorLong, 25);
            return forumPostListingVM;
        }

        private ForumPostListingVM PrepareNewForumGet(String name, String ceiling = null, String floor = null, String limit = null)
        {
            Int64? ceilingTicks = ceiling.ToInt64OrNull();
            Int64? floorTicks = floor.ToInt64OrNull();
            DateTimeOffset? ceilingDateTimeOffset = null;
            DateTimeOffset? floorDateTimeOffset = null;

            if (ceilingTicks.HasValue)
            {
                ceilingDateTimeOffset = new DateTimeOffset?(new DateTimeOffset(new DateTime(ceilingTicks.Value)));
            }

            if (floorTicks.HasValue)
            {
                floorDateTimeOffset = new DateTimeOffset?(new DateTimeOffset(new DateTime(floorTicks.Value)));
            }

            var forumPostListingVM = _forumService.New(name, ceilingDateTimeOffset, floorDateTimeOffset, 25);
            return forumPostListingVM;
        }

        private ForumPostListingVM PrepareControversialForumGet(String name, String ceiling = null, String floor = null, String limit = null)
        {
            Decimal? ceilingDecimal = ceiling.ToDecimalOrNull();
            Decimal? floorDecimal = floor.ToDecimalOrNull();
            
            var forumPostListingVM = _forumService.Controversial(name, ceilingDecimal, floorDecimal);
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