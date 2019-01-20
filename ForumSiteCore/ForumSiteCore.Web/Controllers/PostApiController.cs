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
        public IActionResult Save([FromBody]ForumSaveVM model)
        {
            var postSaveVM = _postService.Save(model.ForumId, _userAccessor.UserId);

            return Ok(postSaveVM);
        }

    
        [Authorize]
        [HttpPost("vote")]
        public IActionResult Vote([FromBody]PostVoteVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            PostVoteVM postVoteVm;
            postVoteVm = _postService.Vote(model.PostId, _userAccessor.UserId, EnumTranslator.VoteTypeToDirection(model.VoteType));
            
            if (postVoteVm.Status == "error")
            {
                Log.Information("Post Upvote failed for post id: {Id} for user: {User}", model.PostId, _userAccessor.UserName);
                return BadRequest();
            }
                
            return Ok(postVoteVm);
        }        
    }
}