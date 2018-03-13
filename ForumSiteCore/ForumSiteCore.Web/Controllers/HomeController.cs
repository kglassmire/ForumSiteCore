using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForumSiteCore.Web.Models;
using Serilog;
using ForumSiteCore.Business.Services;
using ForumSiteCore.Business;
using ForumSiteCore.DAL.Models;

namespace ForumSiteCore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ForumService _forumService;
        private readonly PostService _postService;

        public HomeController(ForumService forumService, PostService postService)
        {
            _forumService = forumService;
            _postService = postService;
        }

        public IActionResult Index()
        {
            var date = new DateTimeOffset(DateTime.Now);
            _forumService.Save(10, 1, true);

            Post postTest = new Post();
            postTest.Name = "Hi";
            postTest.UserId = 1;
            postTest.ForumId = 10;
            postTest.Url = "http://google.com";
            postTest.Name = String.Format("{0}testPost", DateTimeOffset.Now);
            postTest.Description = "Hi this is just a test post okay? Just chill";

            var returnPost = _postService.Add(postTest);

            var hotListing = _forumService.Hot(10, 25);
            //var forumPostListing = _forumService.New(10, 25);
            foreach (var post in hotListing.Posts)
                Log.Debug(String.Format("{0} {1} {2}", post.Id, post.Name, post.Description));
            //Log.Information("Hi from the Home Controller.");
            return View();
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

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
