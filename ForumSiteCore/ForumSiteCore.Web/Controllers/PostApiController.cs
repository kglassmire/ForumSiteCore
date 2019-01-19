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
        private readonly String[] _acceptedLookups = new []{ "best", "top", "new", "controversial" };

        private PostService _postService;
        private IUserAccessor<Int64> _userAccessor;
        public PostApiController(PostService postService, IUserAccessor<Int64> userAccessor)
        {
            _postService = postService;
            _userAccessor = userAccessor;
        }

        [HttpGet("{id}/comments/{lookup}")]
        [ResponseCache(VaryByQueryKeys = new[] { "lookup", "id" }, Duration = 10, Location = ResponseCacheLocation.Any)]
        public IActionResult Get(String lookup, Int64 id)
        {
            if (!_acceptedLookups.Contains(lookup))
                return BadRequest();

            PostCommentListingVM postCommentListingVM = null; ;
            switch (lookup)
            {
                case LookupConsts.LookupBest:
                    postCommentListingVM = _postService.Best(id);
                    break;
                case LookupConsts.LookupNew:
                    postCommentListingVM = _postService.New(id);
                    break;
                case LookupConsts.LookupTop:
                    postCommentListingVM = _postService.Top(id);
                    break;
                case LookupConsts.LookupControversial:
                    postCommentListingVM = _postService.Controversial(id);
                    break;
            }

            return Ok(postCommentListingVM);
        }

        [Authorize]
        public IActionResult Create([FromBody]CreatePostVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Errors());
            

            return Ok();
        }


        [Authorize]
        [HttpPost("save/{id}")]
        [ProducesResponseType(typeof(ForumSaveVM), 200)]
        public IActionResult Save(Int64 id)
        {
            var postSaveVM = _postService.Save(id, _userAccessor.UserId);

            return Ok(postSaveVM);
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