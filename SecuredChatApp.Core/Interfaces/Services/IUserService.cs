using SecuredChatApp.Core.DTOs;
using System;

namespace SecuredChatApp.Core.Interfaces.Services
{
    public interface IUserService
    {
        public ResultModel<object> Login(UserLoginRequest request);
        public ResultModel<object> Register(UserRegisterRequest request);
        public ResultModel<object> Profile(string userId);
    }
}