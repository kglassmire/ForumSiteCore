using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumSiteCore.Business.Exceptions;
using ForumSiteCore.Business.Responses;
using ForumSiteCore.Business.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ForumSiteCore.Web.Controllers
{
    public class ForumWebController : Controller
    {
        private readonly ForumApiController _forumApiController;

        public ForumWebController(ForumApiController forumApiController)
        {
            _forumApiController = forumApiController;
        }
        
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return Index("home", "hot");

            return Index("all", "hot");
        }

        [Route("f/{name}/{lookup?}", Name = "forum")]
        public IActionResult Index(String name, String lookup = null, String ceiling = null, String floor = null, String limit = null, String lookback = null)
        {
            IActionResult result = null;

            if (String.IsNullOrWhiteSpace(lookup))
                lookup = "hot";

            try
            {
                result = _forumApiController.Get(name, lookup);
                var objectResult = (ObjectResult)result;

                if (objectResult.Value is ForumPostListingVM)
                {
                    return View("_ForumPostListing", objectResult.Value);
                }
            }
            catch (ForumNotFoundException)
            {}

            return new NotFoundResult();
        }

    }
}