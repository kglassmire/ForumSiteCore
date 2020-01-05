using ForumSiteCore.BlazorWasm.Server.Utility;
using ForumSiteCore.Business.Consts;
using ForumSiteCore.Business.Enums;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Business.Responses;
using ForumSiteCore.Business.Services;
using ForumSiteCore.Business.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ForumSiteCore.BlazorWasm.Server.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : Controller
    {
        private readonly string[] _acceptedLookups = new []{ "best", "top", "new", "controversial" };

        private readonly PostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(PostService postService, IUserAccessor<long> userAccessor, ILogger<PostController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        [HttpGet("{id}/comments/{lookup}")]
        [ResponseCache(VaryByQueryKeys = new[] { "lookup", "id" }, Duration = 10, Location = ResponseCacheLocation.Any)]
        public IActionResult Get(string lookup, long id)
        {
            if (!_acceptedLookups.Contains(lookup))
                return BadRequest();
            
            PostCommentListingResponse postCommentListingVM = null; ;
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