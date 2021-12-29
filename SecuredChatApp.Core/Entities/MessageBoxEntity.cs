using System;

namespace SecuredChatApp.Core.Entities
{
    public class MessageBoxEntity : BaseEntity
    {
        public Guid Sender { get; set; }
        public Guid To { get; set; }
        public string NumberP { get; set; }
        public string NumberG { get; set; }
        public string FromPublicKey { get; set; }
        public string ToPublicKey { get; set; }
    }
}
