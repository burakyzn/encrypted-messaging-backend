using SecuredChatApp.Core.DTOs;

namespace SecuredChatApp.Core.Interfaces.Services
{
    public interface IFriendService
    {
        public ResultModel<object> AddFriend(AddFriendRequest request);
        public ResultModel<object> GetAddFriendRequests(GetAddFriendRequest request);
        public ResultModel<object> AcceptAddFriendRequest(AcceptAddFriendRequest request);
        public ResultModel<object> RejectAddFriendRequest(RejectAddFriendRequest request);
        public ResultModel<object> GetFriends(GetFriendsRequest request);
        public ResultModel<object> RemoveFriend(RemoveFriendRequest request);
    }
}
