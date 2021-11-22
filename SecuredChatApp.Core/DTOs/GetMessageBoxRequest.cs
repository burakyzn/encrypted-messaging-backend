using System;
using System.ComponentModel.DataAnnotations;

namespace SecuredChatApp.Core.DTOs
{
    public class GetMessageBoxRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}
