using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumSiteCore.Web.Utility;
using ForumSiteCore.Business.Enums;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Business.Services;
using ForumSiteCore.Business.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ForumSiteCore.Web.Controllers
{    
    [Route("api/posts")]
    [ApiController]
    public class PostApiController : ControllerBase
    {
        private PostService _postService;
        private IUserAccessor<Int64> _userAccessor;
        public PostApiController(PostService postService, IUserAccessor<Int64> userAccessor)
        {
            _postService = postService;
            _userAccessor = userAccessor;
        }

        [HttpGet("{id}/comments/best")]
        public PostCommentListingVM Best(Int64 id)
        {
            Log.Debug("loading /api/posts/{id}/comments/best...", id);
            return _postService.Best(id);
        }

        [HttpGet("{id}/comments/top")]
        public PostCommentListingVM Top(Int64 id)
        {
            Log.Debug("loading /api/posts/{id}/comments/top...", id);
            return _postService.Top(id);
        }

        [HttpGet("{id}/comments/controversial")]
        public PostCommentListingVM Controversial(Int64 id)
        {
            Log.Debug("loading /api/posts/{id}/comments/controversial...", id);
            return _postService.Controversial(id);
        }

        [HttpGet("{id}/comments/new")]
        public PostCommentListingVM New(Int64 id)
        {
            Log.Debug("loading /api/posts/{id}/comments/new...", id);
            return _postService.New(id);
        }

        [Authorize]
        public IActionResult Create([FromBody]CreatePostVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Errors());
            

            return Ok();
        }

        [Authorize]
        [HttpGet("save/{id}")]
        public IActionResult Save(Int64 id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = false;
            result = _postService.Save(id, _userAccessor.UserId);

            if (!result)
            {
                Log.Information("Post Save failed for post id: {Id} for user: {User}", id, _userAccessor.UserName);
                return BadRequest();
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("vote/{id}")]
        public IActionResult Vote([FromBody]VotePostVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = false;
            if (model.Direction == true)
            {
                result = _postService.Upvote(model.Id, _userAccessor.UserId);
            }
            else if (model.Direction == false)
            {
                result = _postService.Downvote(model.Id, _userAccessor.UserId);
            }
            else
            {
                result = false;
            }
            
            if (!result)
            {
                Log.Information("Post Upvote failed for post id: {Id} for user: {User}", model.Id, _userAccessor.UserName);
                return BadRequest();
            }
                
            return Ok();
        }        
    }
}