using Microsoft.AspNetCore.Mvc;

namespace ForumSiteCore.BlazorWasm.Server.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : Controller
    {
        //private readonly CommentService _commentService;

        //public CommentApiController(CommentService commentService)
        //{
        //    _commentService = commentService;
        //}

        //[Authorize]
        //[HttpPost("save")]
        //[ProducesResponseType(typeof(CommentSaveVM), 200)]
        //[ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        //public IActionResult Save([FromBody]CommentSaveVM model)
        //{
        //    var postSaveVM = _commentService.Save(model.PostId, model.Saved);

        //    return Ok(postSaveVM);
        //}


        //[Authorize]
        //[HttpPost("vote")]
        //[ProducesResponseType(typeof(CommentVoteVM), 200)]
        //[ProducesResponseType(typeof(ValidationProblemDetails), 404)]
        //public IActionResult Vote([FromBody]CommentVoteVM model)
        //{
        //    CommentVoteVM commentVoteVM;
        //    commentVoteVM = _commentService.Vote(model.PostId, EnumTranslator.VoteTypeToDirection(model.VoteType));

        //    return Ok(commentVoteVM);
        //}
    }
}