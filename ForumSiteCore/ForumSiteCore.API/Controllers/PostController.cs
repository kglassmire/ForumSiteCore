using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumSiteCore.API.Utility;
using ForumSiteCore.Business.Enums;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Business.Services;
using ForumSiteCore.Business.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ForumSiteCore.API.Controllers
{
    [Authorize]
    [Route("api/posts")]
    public class PostController : Controller
    {
        private PostService _postService;
        private IUserAccessor<Int64> _userAccessor;
        public PostController(PostService postService, IUserAccessor<Int64> userAccessor)
        {
            _postService = postService;
            _userAccessor = userAccessor;
        }

        public IActionResult Create([FromBody]CreatePostVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Errors());

            return Ok();
        }

        [HttpGet("save/{id}")]
        public IActionResult Save(Int64 id, Boolean value)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = false;
            result = _postService.Save(id, _userAccessor.UserId, value);

            if (!result)
            {
                Log.Information("Post Save failed for post id: {Id} for user: {User}", id, _userAccessor.UserName);
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet("vote/{id}")]
        public IActionResult Vote(Int64 id, VotedType value)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = false;
            if (value == VotedType.Up)
            {
                result = _postService.Upvote(id, _userAccessor.UserId);
            }
            else if (value == VotedType.Down)
            {
                result = _postService.Downvote(id, _userAccessor.UserId);
            }
            else
            {
                result = false;
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