using System;
using System.ComponentModel.DataAnnotations;

namespace SecuredChatApp.Core.DTOs
{
    public class GetAddFriendRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}
