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

        // GET api/Forum/5
        [HttpGet("{id}")]
        public ForumDto Get(int id)
        {
            return _forumService.Get(id);
        }

        [HttpGet("new/{id}")]
        public ForumPostListing New(Int64 id)
        {
            return _forumService.New(DateTime.Now.AddYears(-5), id, 25);
        }

        [HttpGet("hot/{id}")]
        public ForumPostListing Hot(Int64 id)
        {
            return _forumService.Hot(id, 25);
        }

        [HttpGet("top/{id}")]
        public ForumPostListing Top(Int64 id)
        {
            return _forumService.Top(DateTime.Now.AddYears(-5), id, 25);
        }

        [HttpGet("controversial/{id}")]
        public ForumPostListing Controversial(Int64 id)
        {
            return _forumService.Controversial(DateTime.Now.AddYears(-5), id, 25);
        }
    }
}