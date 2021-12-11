using Microsoft.AspNetCore.Http;
using SecuredChatApp.Core.DTOs;
using System.Threading.Tasks;

namespace SecuredChatApp.Core.Interfaces.Services
{
    public interface IUserService
    {
        public ResultModel<object> Login(UserLoginRequest request);
        public ResultModel<object> Register(UserRegisterRequest request);
        public ResultModel<object> Profile(string userId);
        public Task<ResultModel<object>> ChangeProfileImage(IFormFile profileImage, string userId);
    }
}