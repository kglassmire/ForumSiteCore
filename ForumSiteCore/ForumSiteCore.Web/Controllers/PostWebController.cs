using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumSiteCore.Business.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ForumSiteCore.Web.Controllers
{
    public class PostWebController : Controller
    {
        private readonly PostApiController _postApiController;

        public PostWebController(PostApiController postApiController)
        {
            _postApiController = postApiController;
        }

        [Route("f/{name}/{postId}/comments/")]
        [Route("f/{name}/{postId}/comments/{lookup}")]
        public IActionResult Index(String name, Int64 postId, String lookup = null)
        {
            IActionResult result = null;

            if (String.IsNullOrWhiteSpace(lookup))
                lookup = "best";

            result = _postApiController.Get(lookup, postId);
            var objectResult = (ObjectResult)result;

            if (objectResult.Value is PostCommentListingVM)
            {
                return View("_PostCommentListing", objectResult.Value);
            }

            throw new Exception("BAD RESULT");
        }
    }
}