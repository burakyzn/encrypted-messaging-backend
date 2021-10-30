using SecuredChatApp.Core.Models;

namespace SecuredChatApp.Core.Interfaces.Services
{
    public interface IUserService
    {
        public ResultModel<object> Login(UserLoginRequest request);
        public ResultModel<object> Register(UserRegisterRequest request);
    }
}