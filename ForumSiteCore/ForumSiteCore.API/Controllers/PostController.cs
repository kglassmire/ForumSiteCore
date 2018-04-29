using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ForumSiteCore.API.Controllers
{
    [Authorize]
    [Route("api/post")]
    public class PostController : Controller
    {
        private PostService _postService;
        private IUserAccessor<Int64> _userAccessor;
        public PostController(PostService postService, IUserAccessor<Int64> userAccessor)
        {
            _postService = postService;
            _userAccessor = userAccessor;
        }

        [HttpGet]
        [HttpGet("vote/{id}")]
        public IActionResult Vote(Int64 id, Boolean value)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = false;
            if (value)
            {
                result = _postService.Upvote(id, _userAccessor.UserId);
            }
            else
            {
                result = _postService.Downvote(id, _userAccessor.UserId);
            }
            
            if (!result)
            {
                Log.Information("Post Upvote failed for post id: {Id} for user: {User}", id, _userAccessor.UserName);
                return BadRequest();
            }
                
            return Ok();
        }        
    }
}