using System;
using System.Text.Json.Serialization;
using SecuredChatApp.Core.Entities;

namespace SecuredChatApp.Core.DTOs
{
    public class UserLoginResponse
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string JwtToken { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }

        public UserLoginResponse(UserEntity user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            Nickname = user.Nickname;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}