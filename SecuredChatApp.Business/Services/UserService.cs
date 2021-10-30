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
using SecuredChatApp.Core.Models;
using SecuredChatApp.Infrastructure;

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
            user.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(60);

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
                Nickname = request.Email,
                Password = request.Password,
                IsActive = true
            };

            _dbContext.Users.Add(userEntity);

            int result = _dbContext.SaveChanges();

            if (result < 0)
                return new ResultModel<object>(data: "An unexpected error has occurred.", type:ResultModel<object>.ResultType.FAIL);

            return new ResultModel<object>(data: new UserRegisterResponse(userEntity));
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
                Expires = DateTime.UtcNow.AddDays(30),
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
    
        private bool CheckEmailUniqueness(string email){
            return !_dbContext.Users.Any(user => user.Email == email);
        }
    }
}