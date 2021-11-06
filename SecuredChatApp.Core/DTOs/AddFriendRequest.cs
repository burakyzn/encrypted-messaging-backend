using System;
using System.ComponentModel.DataAnnotations;

namespace SecuredChatApp.Core.DTOs
{
    public class AddFriendRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string ToEmail { get; set; }
    }
}
