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

namespace ForumSiteCore.Web.Controllers
{
    [Route("api/forums")]    
    [ApiController]
    public class ForumApiController : ControllerBase
    {
        private readonly ForumService _forumService;
        private readonly UserActivitiesService _userActivitiesService;

        public ForumApiController(ForumService forumService, UserActivitiesService userActivitiesService)
        {
            _forumService = forumService;
            _userActivitiesService = userActivitiesService;
        }

        [HttpGet("{name}/controversial")]
        [ProducesResponseType(typeof(ForumPostListingResponse), 200)]
        public IActionResult Controversial(String name)
        {
            Log.Debug("loading /f/Controversial/{Name}...", name);

            ForumPostListingVM forumPostListing = _forumService.Controversial(DateTime.Now.AddYears(-5), name, 25);

            return CreateForumPostListingResponse(name, forumPostListing, Consts.POST_LISTING_TYPE_CONTROVERSIAL);
        }

        [HttpGet("search/{search}")]
        [ResponseCache(VaryByQueryKeys = new[] { "*" }, Duration = 60, Location = ResponseCacheLocation.Client)]
        [ProducesResponseType(typeof(ForumSearchResponse), 200)]
        public IActionResult ForumSearch(String search)
        {    
            var searchResults = _forumService.ForumSearch(search);
            ForumSearchResponse response = null;
            if (searchResults.Count == 0)
            {
                response = new ForumSearchResponse { Data = new List<String>(), Message = "No search results found", Status = "failure" };
            }
            else
            {
                response = new ForumSearchResponse { Data = searchResults, Message = $"{searchResults.Count} Search results found", Status = "success" };
            }
            
            return Ok(response);
        }

        [HttpGet("{name}/hot")]
        [ProducesResponseType(typeof(ForumPostListingResponse), 200)]
        public IActionResult Hot(String name)
        {
            Log.Debug("loading /f/Hot/{Name}...", name);

            ForumPostListingVM forumPostListing = _forumService.Hot(name, 25);

            return CreateForumPostListingResponse(name, forumPostListing, Consts.POST_LISTING_TYPE_HOT);
        }

        [HttpGet("{name}/new")]
        [ProducesResponseType(typeof(ForumPostListingResponse), 200)]
        public IActionResult New(String name)
        {
            Log.Debug("loading /f/New/{Name}...", name);

            ForumPostListingVM forumPostListing = _forumService.New(DateTime.Now.AddYears(-10), name, 25);

            return CreateForumPostListingResponse(name, forumPostListing, Consts.POST_LISTING_TYPE_NEW);
        }

        [HttpGet("{name}/top")]
        [ProducesResponseType(typeof(ForumPostListingResponse), 200)]
        public IActionResult Top(String name)
        {
            Log.Debug("loading /f/Top/{Name}...", name);

            ForumPostListingVM forumPostListing = _forumService.Top(DateTime.Now.AddYears(-10), name, 25);

            return CreateForumPostListingResponse(name, forumPostListing, Consts.POST_LISTING_TYPE_TOP);
        }

        private IActionResult CreateForumPostListingResponse(string name, ForumPostListingVM forumPostListing, String listingType)
        {
            ForumPostListingResponse response = null;

            if (forumPostListing.Forum != null)
            {
                response = new ForumPostListingResponse { Data = forumPostListing, Message = $"Retrieved {listingType} items for forum {name}", Status = "success" };
                return Ok(response);
            }

            response = new ForumPostListingResponse { Data = forumPostListing, Message = $"No {listingType} items for forum {name}", Status = "failure" };
            return Ok(response);
        }
    }
}