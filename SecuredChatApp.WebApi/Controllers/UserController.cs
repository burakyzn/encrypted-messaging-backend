using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecuredChatApp.Core.Interfaces.Services;
using SecuredChatApp.Core.Models;

namespace SecuredChatApp.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserLoginRequest request)
        {
            var response = _userService.Authenticate(request);

            if (response == null)
                throw new Exception("Username or password is incorrect");

            SetTokenCookie(response.RefreshToken);

            return Ok(new {
                success = true,
                data = response
            });
        }

        [HttpGet("LoginTest")]
        public IActionResult LoginTest()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            string userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new {
                success = true,
                data = String.Concat("Your id : ", userId)
            });
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserRegisterRequest request)
        {
            var response = _userService.Register(request);

            if (response == null)
                throw new Exception("Username or password is incorrect");

            return Ok(new
            {
                success = true,
                data = response
            });
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(60)
            };

            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}