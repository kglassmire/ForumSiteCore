using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForumSiteCore.Web.Models;
using Serilog;
using ForumSiteCore.DAL;

namespace ForumSiteCore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var date = new DateTimeOffset(new DateTime(2017, 03, 09, 08, 55, 32));
            //var posts = _context.Posts
            //    .Include(x => x.User)
            //    .Where(x => x.ForumId.Equals(592) && (x.Created < new DateTimeOffset(new DateTime(2017, 03, 09, 08, 55, 32)).ToUniversalTime() && x.Id < 17034)).ToList();
            var posts = _context.Posts
                .Include(x => x.User)
                .Where(x => x.ForumId.Equals(592) && (x.Created < date && x.Id < 17034))
                .OrderByDescending(x => x.Created).ThenByDescending(x => x.Id)
                .Take(25)
                .ToList();



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
