using System;
using System.Text.Json.Serialization;
using SecuredChatApp.Core.Entities;

namespace SecuredChatApp.Core.Models
{
    public class UserRegisterResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public UserRegisterResponse(UserEntity user)
        {
            Id = user.Id;
            Email = user.Email;
        }
    }
}
