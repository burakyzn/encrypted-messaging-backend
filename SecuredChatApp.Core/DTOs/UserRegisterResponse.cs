using System;
using SecuredChatApp.Core.Entities;

namespace SecuredChatApp.Core.DTOs
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
