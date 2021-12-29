using System;

namespace SecuredChatApp.Core.DTOs
{
    public class GetAddFriendResponseModel
    {
        public Guid Id { get; set; }
        public Guid SenderUserId { get; set; }  
        public string FromEmail { get; set; }
    }
}
