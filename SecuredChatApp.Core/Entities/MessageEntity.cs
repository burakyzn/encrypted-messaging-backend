using System;

namespace SecuredChatApp.Core.Entities
{
    public class MessageEntity : BaseEntity
    {
        public Guid Sender { get; set; }
        public Guid To { get; set; }
        public string Message { get; set; }
        public bool Read { get; set; } = false;
    }
}
