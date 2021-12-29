using System;
using System.ComponentModel.DataAnnotations;

namespace SecuredChatApp.Core.DTOs
{
    public class GetPublicKeyRequest
    {
        [Required]
        public Guid MyId { get; set; }

        [Required]
        public Guid FriendId { get; set; }
    }
}