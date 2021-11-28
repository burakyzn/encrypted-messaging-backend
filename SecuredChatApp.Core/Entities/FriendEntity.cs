using System;

namespace SecuredChatApp.Core.Entities
{
    public class FriendEntity : BaseEntity
    {
        public Guid SenderUserID { get; set; }
        public Guid ReceiverID { get; set; }
        public bool IsRequest { get; set; }
    }
}
