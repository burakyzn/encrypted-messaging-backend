using System;
using System.ComponentModel.DataAnnotations;

namespace SecuredChatApp.Core.DTOs
{
    public  class GetDHParameterOfMessageBoxRequest
    {
        [Required]
        public Guid SenderUserId { get; set; }

        [Required]
        public Guid ToUserId { get; set; }
    }
}