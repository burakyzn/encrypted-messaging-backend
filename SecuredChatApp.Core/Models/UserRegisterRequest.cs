using System.ComponentModel.DataAnnotations;

namespace SecuredChatApp.Core.Models
{
    public class UserRegisterRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}