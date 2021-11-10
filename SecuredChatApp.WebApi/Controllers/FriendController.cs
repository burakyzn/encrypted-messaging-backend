using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuredChatApp.Core.DTOs;
using SecuredChatApp.Core.Interfaces.Services;

namespace SecuredChatApp.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FriendController : Controller
    {
        private readonly IFriendService _friendService;

        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        [HttpPost("AddFriend")]
        public ResultModel<object> AddFriend([FromBody] AddFriendRequest request)
        {
            var result = _friendService.AddFriend(request);
            return result;
        }

        [HttpPost("GetAddFriendRequests")]
        public ResultModel<object> GetAddFriendRequests([FromBody] GetAddFriendRequest request)
        {
            var result = _friendService.GetAddFriendRequests(request);
            return result;
        }

        [HttpPost("AcceptAddFriendRequests")]
        public ResultModel<object> AcceptAddFriendRequests([FromBody] AcceptAddFriendRequest request)
        {
            var result = _friendService.AcceptAddFriendRequest(request);
            return result;
        }

        [HttpPost("RejectAddFriendRequests")]
        public ResultModel<object> RejectAddFriendRequests([FromBody] RejectAddFriendRequest request)
        {
            var result = _friendService.RejectAddFriendRequest(request);
            return result;
        }

        [HttpPost("GetFriends")]
        public ResultModel<object> GetFriends([FromBody] GetFriendsRequest request)
        {
            var result = _friendService.GetFriends(request);
            return result;
        }

        [HttpPost("RemoveFriend")]
        public ResultModel<object> RemoveFriend([FromBody] RemoveFriendRequest request)
        {
            var result = _friendService.RemoveFriend(request);
            return result;
        }
    }
}
