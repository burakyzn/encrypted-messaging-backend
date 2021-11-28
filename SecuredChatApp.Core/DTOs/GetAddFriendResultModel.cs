using SecuredChatApp.Core.Entities;

namespace SecuredChatApp.Core.DTOs
{
    public class GetAddFriendResultModel
    {
        public string FriendEmail { get; set; }
        public FriendEntity addFriendRequest { get; set; }
    }
}
