using System;

namespace SecuredChatApp.Core.DTOs
{
    public class GetMessageBoxModel
    {
        public Guid FriendId { get; set; }
        public string Nickname { get; set; }
        public string Message { get; set; }
        public DateTime? SendDate { get; set; }
        public bool Read { get; set; }
    }
}
