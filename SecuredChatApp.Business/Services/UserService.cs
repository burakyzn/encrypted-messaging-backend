using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecuredChatApp.Business.Helpers;
using SecuredChatApp.Core.Entities;
using SecuredChatApp.Core.Interfaces.Services;
using SecuredChatApp.Core.DTOs;
using SecuredChatApp.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace SecuredChatApp.Business.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AppSettings _appSettings;

        public UserService(
            ApplicationDbContext dbContext,
            IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _appSettings = appSettings.Value;
        }

        private string GenerateJwtToken(UserEntity user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private String GenerateRefreshToken()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public ResultModel<object> Login(UserLoginRequest request)
        {
            var user = _dbContext.Users
                .SingleOrDefault(user => user.Email == request.Email 
                    && user.Password == request.Password
                    && user.IsActive);

            if (user == null)
                return new ResultModel<object>(data: "Email or password is not correct!", type:ResultModel<object>.ResultType.FAIL);

            var jwtToken = GenerateJwtToken(user);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpireDate = DateTime.Now.AddDays(60);

            _dbContext.Update(user);
            int result = _dbContext.SaveChanges();

            if (result < 0)
                return new ResultModel<object>(data: "An unexpected error has occurred.", type:ResultModel<object>.ResultType.FAIL);

            return new ResultModel<object>(data: new UserLoginResponse(user, jwtToken, user.RefreshToken));
        }

        public ResultModel<object> Register(UserRegisterRequest request)
        {
            if(!CheckEmailUniqueness(request.Email))
                return new ResultModel<object>(data: "This email already exists!", type:ResultModel<object>.ResultType.FAIL);

            UserEntity userEntity = new UserEntity {
                Email = request.Email,
                Nickname = request.Nickname,
                Password = request.Password,
                IsActive = true
            };

            _dbContext.Users.Add(userEntity);

            int result = _dbContext.SaveChanges();

            if (result < 0)
                return new ResultModel<object>(data: "An unexpected error has occurred.", type:ResultModel<object>.ResultType.FAIL);

            return new ResultModel<object>(data: new UserRegisterResponse(userEntity));
        }

        public ResultModel<object> Profile(string userId)
        {
            var user = _dbContext.Users
                .SingleOrDefault(user => user.Id.ToString() == userId && user.IsActive);

            if (user == null)
                return new ResultModel<object>(data: "User does not exist!", type: ResultModel<object>.ResultType.FAIL);

            return new ResultModel<object>(data: new GetUserProfileResponse(user.Nickname, user.Email, user.ProfileImageUrl));
        }

        private bool CheckEmailUniqueness(string email){
            return !_dbContext.Users.Any(user => user.Email == email);
        }

        public async Task<ResultModel<object>> ChangeProfileImage(IFormFile profileImage, string userId)
        {
            var user = _dbContext.Users
                .SingleOrDefault(user => user.Id.ToString() == userId && user.IsActive);

            if (user == null)
                return new ResultModel<object>(data: "User does not exist!", type: ResultModel<object>.ResultType.FAIL);

            string oldImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl) ? "" : user.ProfileImageUrl;

            var extension = Path.GetExtension(profileImage.FileName);

            var pathDir = Path.Combine(Directory.GetCurrentDirectory(), "Contents\\Images\\Avatar");

            if (!Directory.Exists(pathDir))
                Directory.CreateDirectory(pathDir);

            string fileName = Guid.NewGuid() + extension;
            using (var stream = new FileStream(Path.Combine(pathDir, fileName), FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            user.ProfileImageUrl = fileName;
            _dbContext.Update(user);
            int result = _dbContext.SaveChanges();

            if(result > 0)
            {
                if(!string.IsNullOrEmpty(oldImageUrl) && File.Exists(Path.Combine(pathDir, oldImageUrl)))
                    File.Delete(Path.Combine(pathDir, oldImageUrl));

                return new ResultModel<object>(data: fileName, type: ResultModel<object>.ResultType.SUCCESS);
            }
            else
                return new ResultModel<object>(data: "fail", type: ResultModel<object>.ResultType.FAIL);
        }
    }
}
