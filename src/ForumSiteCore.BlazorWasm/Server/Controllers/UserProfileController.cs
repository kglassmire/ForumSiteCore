using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumSiteCore.Business.Responses;
using ForumSiteCore.Business.Services;
using ForumSiteCore.Business.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ForumSiteCore.BlazorWasm.Server.Controllers
{  
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : Controller
    {
        private readonly ILogger<UserProfileController> _logger;
        private readonly UserProfileService _userProfileService;

        public UserProfileController(ILogger<UserProfileController> logger,
            UserProfileService userProfileService)
        {
            _logger = logger;
            _userProfileService = userProfileService;
        }

        [HttpGet("{username}/posts-created")]
        public async Task<IActionResult> UserCreatedPosts(string username)
        {
            UserProfileResponse vm = new UserProfileResponse();
            vm.Posts = _userProfileService.UserCreatedPosts(username);

            return Ok(vm);
        }

        public async Task<IActionResult> UserSavedPosts(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> UserVotedPosts(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> UserCommentedPosts(string username)
        {
            throw new NotImplementedException();
        }



    }
}