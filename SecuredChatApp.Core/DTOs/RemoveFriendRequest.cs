using System;
using System.ComponentModel.DataAnnotations;

namespace SecuredChatApp.Core.DTOs
{
    public class RemoveFriendRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}
