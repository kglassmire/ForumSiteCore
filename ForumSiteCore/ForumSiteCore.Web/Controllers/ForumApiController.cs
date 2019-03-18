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
        
        [ResponseCache(VaryByQueryKeys = new[] { "name", "lookup", "after", "before", "count" }, Duration = 10, Location = ResponseCacheLocation.Any)]
        [HttpGet("{name}/{lookup}")]
        [ProducesResponseType(typeof(ForumPostListingVM), 200)]
        public IActionResult Get(String name, String lookup, String after = null, String before = null, String count = null)
        {            
            if (!_acceptedLookups.Contains(lookup.ToLower()))
                return BadRequest();

            if (!String.IsNullOrWhiteSpace(after) && !String.IsNullOrWhiteSpace(before))
                throw new Exception("Query string params should not have 'after' and 'before' arguments");



            ForumPostListingVM forumPostListingVM = null; ;
            switch (lookup.ToLower())
            {
                case LookupConsts.LookupHot:
                    forumPostListingVM = PrepareHotForumGet(name, before, after, count);
                    //forumPostListingVM = _forumService.Hot(name, 25, afterDecimal);
                    break;
                case LookupConsts.LookupNew:
                    forumPostListingVM = _forumService.New(name, 25);
                    break;
                case LookupConsts.LookupTop:
                    forumPostListingVM = PrepareTopForumGet(name, before, after, count);
                    break;
                case LookupConsts.LookupControversial:
                    forumPostListingVM = _forumService.Controversial(new DateTimeOffset(new DateTime(2010, 1, 1)), name);
                    break;
            }

            return Ok(forumPostListingVM);
        }

        private ForumPostListingVM PrepareHotForumGet(String name, String before = null, String after = null, String count = null)
        {
            Decimal? beforeDecimal = Decimal.TryParse(after, out Decimal beforeParsedValue) ? beforeParsedValue : (Decimal?)null;
            Decimal? afterDecimal = Decimal.TryParse(after, out Decimal afterParsedValue) ? afterParsedValue : (Decimal?)null;

            var forumPostListingVM = _forumService.Hot(name, 25, afterDecimal);
            return forumPostListingVM;
        }

        private ForumPostListingVM PrepareTopForumGet(String name, String before = null, String after = null, String count = null)
        {
            Int64? beforeTicks = Int64.TryParse(after, out Int64 beforeParsedValue) ? beforeParsedValue : (Int64?)null;
            Int64? afterTicks = Int64.TryParse(after, out Int64 afterParsedValue) ? afterParsedValue : (Int64?)null;
            DateTimeOffset? beforeDateTimeOffset = null;
            DateTimeOffset? afterDateTimeOffset = null;

            if (beforeTicks.HasValue)
            {
                beforeDateTimeOffset = new DateTimeOffset?(new DateTimeOffset(new DateTime(beforeTicks.Value)));
            }

            if (afterTicks.HasValue)
            {
                afterDateTimeOffset = new DateTimeOffset?(new DateTimeOffset(new DateTime(afterTicks.Value)));
            }

            var forumPostListingVM = _forumService.Top(name, beforeDateTimeOffset, afterDateTimeOffset);
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