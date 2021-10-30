using SecuredChatApp.Core.Models;

namespace SecuredChatApp.Core.Interfaces.Services
{
    public interface IUserService
    {
        public UserLoginResponse Authenticate(UserLoginRequest request);
        public UserRegisterResponse Register(UserRegisterRequest request);
    }
}