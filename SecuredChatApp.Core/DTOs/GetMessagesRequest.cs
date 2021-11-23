using System;
using System.ComponentModel.DataAnnotations;

namespace SecuredChatApp.Core.DTOs
{
    public  class GetMessagesRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid FriendId { get; set; }
    }
}
