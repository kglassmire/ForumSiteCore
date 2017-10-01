using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForumSiteCore.Web.Models;
using Serilog;
using ForumSiteCore.DAL.Repositories.Interfaces;

namespace ForumSiteCore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IForumRepository _forumRepository;

        public HomeController(IForumRepository forumRepository)
        {
            _forumRepository = forumRepository;
        }

        public IActionResult Index()
        {
            var date = new DateTimeOffset(DateTime.Now);
            var posts = _forumRepository.New(592, 25);
            foreach (var post in posts)
                Log.Debug("{@Post}", post);
            Log.Information("Hi from the Home Controller.");
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
