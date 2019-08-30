using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumSiteCore.Web.Utility;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Business.Services;
using ForumSiteCore.Business.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ForumSiteCore.Business.Consts;
using ForumSiteCore.Business.Enums;
using Microsoft.Extensions.Logging;

namespace ForumSiteCore.Web.Controllers
{    
    [Route("api/posts")]
    [ApiController]
    public class PostApiController : ControllerBase
    {
        private readonly String[] _acceptedLookups = new []{ "best", "top", "new", "controversial" };

        private readonly PostService _postService;
        private readonly ILogger<PostApiController> _logger;

        public PostApiController(PostService postService, IUserAccessor<Int64> userAccessor, ILogger<PostApiController> logger)
        {
            _postService = postService;
            _logger = logger;
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
            {
                return BadRequest(ModelState.Errors());
            }

            return Ok();
        }


        [Authorize]
        [HttpPost("save")]
        [ProducesResponseType(typeof(PostSaveVM), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        public IActionResult Save([FromBody]PostSaveVM model)
        {
            var postSaveVM = _postService.Save(model.PostId, model.Saved);

            return Ok(postSaveVM);
        }

    
        [Authorize]
        [HttpPost("vote")]
        [ProducesResponseType(typeof(PostSaveVM), 200)]        
        [ProducesResponseType(typeof(ValidationProblemDetails), 404)]
        public IActionResult Vote([FromBody]PostVoteVM model)
        {
            PostVoteVM postVoteVm;
            postVoteVm = _postService.Vote(model.PostId, EnumTranslator.VoteTypeToDirection(model.VoteType));           
                
            return Ok(postVoteVm);
        }        
    }
}