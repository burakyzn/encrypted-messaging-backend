using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecuredChatApp.Core.Interfaces.Services;
using SecuredChatApp.Core.DTOs;

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
        public ResultModel<object> Login([FromBody] UserLoginRequest request)
        {
            var result = _userService.Login(request);

            if(result.success)
                SetTokenCookie(((UserLoginResponse)result.data).RefreshToken);

            return result;
        }

        [HttpGet("LoginTest")]
        public ResultModel<object> LoginTest()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            string userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            
            return new ResultModel<object>(data: "your id : " + userId);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public ResultModel<object> Register([FromBody] UserRegisterRequest request)
        {
            var result = _userService.Register(request);
            return result;
        }

        [HttpPost("AddFriend")]
        public ResultModel<object> AddFriend([FromBody] AddFriendRequest request)
        {
            var result = _userService.AddFriend(request);
            return result;
        }

        [HttpPost("GetAddFriendRequests")]
        public ResultModel<object> GetAddFriendRequests([FromBody] GetAddFriendRequest request)
        {
            var result = _userService.GetAddFriendRequests(request);
            return result;
        }

        [HttpPost("AcceptAddFriendRequests")]
        public ResultModel<object> AcceptAddFriendRequests([FromBody] AcceptAddFriendRequest request)
        {
            var result = _userService.AcceptAddFriendRequest(request);
            return result;
        }

        [HttpPost("RejectAddFriendRequests")]
        public ResultModel<object> RejectAddFriendRequests([FromBody] RejectAddFriendRequest request)
        {
            var result = _userService.RejectAddFriendRequest(request);
            return result;
        }

        [HttpPost("GetFriends")]
        public ResultModel<object> GetFriends([FromBody] GetFriendsRequest request)
        {
            var result = _userService.GetFriends(request);
            return result;
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(60)
            };

            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}
