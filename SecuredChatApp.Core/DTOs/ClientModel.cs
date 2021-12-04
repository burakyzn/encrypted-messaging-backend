using System;

namespace SecuredChatApp.Core.DTOs
{
    public class ClientModel
    {
        public string ConnectionId { get; set; }
        public Guid UserID { get; set; } 
        public string Nickname { get; set; }
    }
}
