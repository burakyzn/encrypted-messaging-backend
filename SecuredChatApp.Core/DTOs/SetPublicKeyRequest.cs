using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SecuredChatApp.Core.DTOs
{
    public class SetPublicKeyRequest
    {
        [Required]
        public Guid SenderUserId { get; set; }

        [Required]
        public Guid ToUserId { get; set; }
        
        [Required]
        public string PublicKey { get; set; }
    }
}