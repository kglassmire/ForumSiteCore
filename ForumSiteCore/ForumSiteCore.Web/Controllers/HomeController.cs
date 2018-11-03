using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ForumSiteCore.Web.Models;
using ForumSiteCore.Web.Controllers;
using ForumSiteCore.Business.Responses;

namespace ForumSiteCore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ForumController _forumController;
        public HomeController(ForumController forumController)
        {
            _forumController = forumController;
        }
        public IActionResult Index()
        {
            var result = _forumController.Hot("advertising");
            var objectResult = (ObjectResult)result;
            if (objectResult.Value is ForumPostListingResponse)
            {
                return View(result);
            }

            throw new Exception("BAD RESULT");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
