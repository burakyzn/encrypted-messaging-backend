using System;

namespace SecuredChatApp.Core.DTOs
{
    public class GetMessagesModel
    {
        public Guid Sender { get; set; }
        public Guid To { get; set; }
        public string Message { get; set; }
        public DateTime? SendDate { get; set; }
        public bool Read { get; set; }
    }
}
