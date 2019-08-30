using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ForumSiteCore.Web.Controllers
{
    public class ErrorController : Controller
    {

        public IActionResult Index(Int32? statusCode)
        {
            if (statusCode.HasValue)
            {
                if (statusCode.Value == 404)
                {
                    var viewName = statusCode.ToString();
                    return View(viewName);
                }                
            }

            return new EmptyResult();

        }
    }
}