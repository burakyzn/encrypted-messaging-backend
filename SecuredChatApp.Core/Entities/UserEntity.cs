using System;
using System.Text.Json.Serialization;

namespace SecuredChatApp.Core.Entities
{
    public class UserEntity : BaseEntity
    {
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfileImageUrl { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
        [JsonIgnore]
        public DateTime? RefreshTokenExpireDate { get; set; }
    }
}