using System;
using System.ComponentModel.DataAnnotations;

namespace SecuredChatApp.Core.DTOs
{
    public class AcceptAddFriendRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}
