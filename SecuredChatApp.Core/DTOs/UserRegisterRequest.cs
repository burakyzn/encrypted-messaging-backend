using System.ComponentModel.DataAnnotations;

namespace SecuredChatApp.Core.DTOs
{
    public class UserRegisterRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}