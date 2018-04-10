using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumSiteCore.Business.Models;
using ForumSiteCore.Business.ViewModels;
using ForumSiteCore.Business.Services;
using ForumSiteCore.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ForumSiteCore.API.Controllers
{
    [Produces("application/json")]
    [Route("api/forum")]    
    public class ForumController : Controller
    {
        private readonly ForumService _forumService;
        public ForumController(ForumService forumService)
        {
            _forumService = forumService;
        }

        [HttpGet("search/{search}")]
        public IList<String> ForumSearch(String search)
        {
            return _forumService.ForumSearch(search);
        }

        [HttpGet("{name}/new")]
        public ForumPostListing New(String name)
        {
            return _forumService.New(DateTime.Now.AddYears(-5), name, 25);
        }

        [HttpGet("{name}/hot")]
        public ForumPostListing Hot(String name)
        {
            return _forumService.Hot(name, 25);
        }

        [HttpGet("{name}/top")]
        public ForumPostListing Top(String name)
        {
            return _forumService.Top(DateTime.Now.AddYears(-5), name, 25);
        }

        [HttpGet("{name}/controversial")]
        public ForumPostListing Controversial(String name)
        {
            return _forumService.Controversial(DateTime.Now.AddYears(-5), name, 25);
        }
    }
}