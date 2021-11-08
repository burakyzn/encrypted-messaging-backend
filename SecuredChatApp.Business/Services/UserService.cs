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
    
        private bool CheckEmailUniqueness(string email){
            return !_dbContext.Users.Any(user => user.Email == email);
        }

        public ResultModel<object> AddFriend(AddFriendRequest request)
        {
            var user = _dbContext.Users.SingleOrDefault(user => user.Id == request.Id && user.IsActive);

            if(user.Email == request.ToEmail)
                return new ResultModel<object>(data: "You can't friend request to yourself!", type: ResultModel<object>.ResultType.FAIL);

            var requestTo = _dbContext.Users.SingleOrDefault(user => user.Email == request.ToEmail && user.IsActive);

            if (requestTo == null)
                return new ResultModel<object>(data: "User does not exist!", type: ResultModel<object>.ResultType.FAIL);

            if (!CheckSingleAddFriendRequest(user.Email, request.ToEmail))
                return new ResultModel<object>(data: "Only one friend request can be sent to a person!", type: ResultModel<object>.ResultType.FAIL);

            FriendEntity friendEntity = new FriendEntity
            {
                Creator = request.Id.ToString(),
                IsRequest = true,
                User = user.Email,
                With = request.ToEmail,
                IsActive = true
            };

            _dbContext.Friends.Add(friendEntity);

            int result = _dbContext.SaveChanges();

            if (result < 0)
                return new ResultModel<object>(data: "An unexpected error has occurred.", type: ResultModel<object>.ResultType.FAIL);

            return new ResultModel<object>();
        }

        private bool CheckSingleAddFriendRequest(string FromEmail, string ToEmail)
        {
            return !_dbContext.Friends.Any(friend => 
                ((friend.User == FromEmail && friend.With == ToEmail) ||
                (friend.With == FromEmail && friend.User == ToEmail)) &&
                friend.IsActive
            );
        }

        public ResultModel<object> GetAddFriendRequests(GetAddFriendRequest request)
        {
            var user = _dbContext.Users.SingleOrDefault(user => user.Id == request.Id && user.IsActive);

            if (user == null)
                return new ResultModel<object>(data: "User does not exist!", type: ResultModel<object>.ResultType.FAIL);

            var requests = _dbContext.Friends.Where(requests => 
                requests.With == user.Email &&
                requests.IsRequest == true &&
                requests.IsActive
            ).ToList();

            return new ResultModel<object>(data: new GetAddFriendResponse(requests));
        }

        public ResultModel<object> AcceptAddFriendRequest(AcceptAddFriendRequest request)
        {
            var friend = _dbContext.Friends.SingleOrDefault(friend => friend.Id == request.Id && friend.IsRequest && friend.IsActive);

            if (friend == null)
                return new ResultModel<object>(data: "Friend request does not exist!", type: ResultModel<object>.ResultType.FAIL);

            friend.IsRequest = false;

            _dbContext.Friends.Update(friend);

            int result = _dbContext.SaveChanges();

            if (result < 0)
                return new ResultModel<object>(data: "An unexpected error has occurred.", type: ResultModel<object>.ResultType.FAIL);

            return new ResultModel<object>();
        }

        public ResultModel<object> RejectAddFriendRequest(RejectAddFriendRequest request)
        {
            var friend = _dbContext.Friends.SingleOrDefault(friend => friend.Id == request.Id && friend.IsRequest && friend.IsActive);

            if (friend == null)
                return new ResultModel<object>(data: "Friend request does not exist!", type: ResultModel<object>.ResultType.FAIL);

            friend.IsActive = false;

            _dbContext.Friends.Update(friend);

            int result = _dbContext.SaveChanges();

            if (result < 0)
                return new ResultModel<object>(data: "An unexpected error has occurred.", type: ResultModel<object>.ResultType.FAIL);

            return new ResultModel<object>();
        }
    }
}
