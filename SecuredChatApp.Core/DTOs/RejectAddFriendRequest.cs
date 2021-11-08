using System;
using System.ComponentModel.DataAnnotations;

namespace SecuredChatApp.Core.DTOs
{
    public class RejectAddFriendRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}
