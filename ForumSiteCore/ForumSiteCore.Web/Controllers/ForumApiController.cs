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

        [Authorize]
        [HttpPost("save/{id}")]
        [ProducesResponseType(typeof(ForumSaveVM), 200)]
        public IActionResult Save(Int64 id)
        {
            var forumSaveVM = _forumService.Save(id);

            return Ok(forumSaveVM);
        }

        [HttpGet("{name}/controversial")]
        [ProducesResponseType(typeof(ForumPostListingVM), 200)]
        public IActionResult Controversial(String name)
        {
            Log.Debug("loading /f/Controversial/{Name}...", name);

            ForumPostListingVM forumPostListing = _forumService.Controversial(DateTime.Now.AddYears(-5), name, 25);

            return Ok(forumPostListing);
        }

        [HttpGet("search/{search}")]
        [ResponseCache(VaryByQueryKeys = new[] { "*" }, Duration = 60, Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(ForumSearchVM), 200)]
        public IActionResult ForumSearch(String search)
        {
            var searchResults = _forumService.ForumSearch(search);

            return Ok(searchResults);
        }

        [HttpGet("{name}/hot")]
        [ProducesResponseType(typeof(ForumPostListingVM), 200)]
        public IActionResult Hot(String name, Decimal? after = null)
        {
            Log.Debug("loading /f/Hot/{Name}...", name);

            ForumPostListingVM forumPostListing = _forumService.Hot(name, 25, after);

            return Ok(forumPostListing);
        }

        [HttpGet("{name}/new")]
        [ProducesResponseType(typeof(ForumPostListingVM), 200)]
        public IActionResult New(String name)
        {
            Log.Debug("loading /f/New/{Name}...", name);

            ForumPostListingVM forumPostListing = _forumService.New(DateTime.Now.AddYears(-10), name, 25);

            return Ok(forumPostListing);
        }

        [HttpGet("{name}/top")]
        [ProducesResponseType(typeof(ForumPostListingVM), 200)]
        public IActionResult Top(String name)
        {
            Log.Debug("loading /f/Top/{Name}...", name);

            ForumPostListingVM forumPostListing = _forumService.Top(DateTime.Now.AddYears(-10), name, 25);

            return Ok(forumPostListing);
        }
    }
}