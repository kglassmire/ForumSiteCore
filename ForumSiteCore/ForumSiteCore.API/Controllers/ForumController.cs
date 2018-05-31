using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumSiteCore.Business.Models;
using ForumSiteCore.Business.ViewModels;
using ForumSiteCore.Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace ForumSiteCore.API.Controllers
{
    [Route("api/forum")]        
    public class ForumController : Controller
    {
        private readonly ForumService _forumService;
        private readonly UserActivitiesService _userActivitiesService;

        public ForumController(ForumService forumService, UserActivitiesService userActivitiesService)
        {
            _forumService = forumService;
            _userActivitiesService = userActivitiesService;
        }

        [HttpGet("search/{search}")]
        public IList<String> ForumSearch(String search)
        {
            return _forumService.ForumSearch(search);
        }

        [HttpGet("{name}/new")]
        public ForumPostListing New(String name)
        {
            Log.Debug("loading /f/New/{Name}...", name);

            return _forumService.New(DateTime.Now.AddYears(-5), name, 25);
        }

        [HttpGet("{name}/hot")]
        public ForumPostListing Hot(String name)
        {            
            Log.Debug("loading /f/Hot/{Name}...", name);

            var forumPostListing = _forumService.Hot(name, 25);

            return forumPostListing;
        }

        [HttpGet("{name}/top")]
        public ForumPostListing Top(String name)
        {
            Log.Debug("loading /f/Top/{Name}...", name);

            return _forumService.Top(DateTime.Now.AddYears(-5), name, 25);
        }

        [HttpGet("{name}/controversial")]
        public ForumPostListing Controversial(String name)
        {
            Log.Debug("loading /f/Controversial/{Name}...", name);

            return _forumService.Controversial(DateTime.Now.AddYears(-5), name, 25);
        }
    }
}