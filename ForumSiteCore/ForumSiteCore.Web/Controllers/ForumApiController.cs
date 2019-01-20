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
                      
        
        [ResponseCache(VaryByQueryKeys = new[] { "name", "lookup", "after" }, Duration = 10, Location = ResponseCacheLocation.Any)]
        [HttpGet("{name}/{lookup}")]
        [ProducesResponseType(typeof(ForumPostListingVM), 200)]
        public IActionResult Get(String name, String lookup, Decimal? after = null)
        {
            if (!_acceptedLookups.Contains(lookup))
                return BadRequest();

            ForumPostListingVM forumPostListingVM = null; ;
            switch (lookup)
            {
                case LookupConsts.LookupHot:
                    forumPostListingVM = _forumService.Hot(name, 25, after);
                    break;
                case LookupConsts.LookupNew:
                    forumPostListingVM = _forumService.New(new DateTimeOffset(new DateTime(2010, 1, 1)), name);
                    break;
                case LookupConsts.LookupTop:
                    forumPostListingVM = _forumService.Top(new DateTimeOffset(new DateTime(2010, 1, 1)), name);
                    break;
                case LookupConsts.LookupControversial:
                    forumPostListingVM = _forumService.Controversial(new DateTimeOffset(new DateTime(2010, 1, 1)), name);
                    break;
            }

            return Ok(forumPostListingVM);
        }

        [Authorize]
        [HttpPost("save/{id}")]
        [ProducesResponseType(typeof(ForumSaveVM), 200)]
        public IActionResult Save(Int64 id)
        {
            var forumSaveVM = _forumService.Save(id);

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