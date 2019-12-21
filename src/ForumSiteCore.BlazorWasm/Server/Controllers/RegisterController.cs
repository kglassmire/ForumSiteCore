using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ForumSiteCore.BlazorWasm.Server.Utility;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Business.Responses;
using ForumSiteCore.Business.ViewModels;
using ForumSiteCore.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ForumSiteCore.BlazorWasm.Server.Controllers
{
    [Route("api/register")]
    public class RegisterController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterController> _logger;
        private readonly IEmailService _emailService;

        public RegisterController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<RegisterController> logger,
            IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] RegisterVM registerViewModel, string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = registerViewModel.UserName, Email = registerViewModel.Email };
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailService.SendEmailAsync(registerViewModel.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok(new RegisterResponse { Success = true, ReturnUrl = returnUrl, Message = "Register success" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(new RegisterResponse { Message = "Registration failed", ReturnUrl = returnUrl, Success = false, Errors = ModelState.Errors() });
            }

            return BadRequest(new RegisterResponse { Message = "Invalid Register model", ReturnUrl = returnUrl, Success = false, Errors = ModelState.Errors() });
            
        }
    }
}