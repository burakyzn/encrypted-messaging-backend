using System;
using System.ComponentModel.DataAnnotations;

namespace SecuredChatApp.Core.DTOs
{
    public class GetFriendsRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}
