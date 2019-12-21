using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ForumSiteCore.BlazorWasm.Server.Utility;
using ForumSiteCore.Business.Responses;
using ForumSiteCore.Business.ViewModels;
using ForumSiteCore.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ForumSiteCore.BlazorWasm.Server.Controllers
{
    [Route("api/login")]
    public class LoginController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginController> _logger;
        private readonly IConfiguration _configuration;

        public LoginController(SignInManager<ApplicationUser> signInManager, ILogger<LoginController> logger, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index([FromBody]LoginVM loginViewModel, string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");


                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, loginViewModel.UserName)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

                    var token = new JwtSecurityToken(
                        _configuration["JwtIssuer"],
                        _configuration["JwtAudience"],
                        claims,
                        expires: expiry,
                        signingCredentials: creds
                    );

                    return Ok(new LoginResponse { Success = true, Message = "Login success", ReturnUrl = returnUrl, Token = new JwtSecurityTokenHandler().WriteToken(token) });
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = loginViewModel.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return Unauthorized("Account lockout");
                }
                else
                {
                    return Unauthorized();
                }
            }

            return BadRequest(new LoginResponse { Errors = ModelState.Errors(), Success = false, Message = "Invalid login model", ReturnUrl = returnUrl });
        }
    }
}