using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecuredChatApp.Business.Helpers;
using SecuredChatApp.Core.DTOs;
using SecuredChatApp.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuredChatApp.WebApi.SocketHubs
{
    public class MainHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AppSettings _appSettings;

        public MainHub(
            ApplicationDbContext dbContext,
            IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _appSettings = appSettings.Value;
        }

        public async Task ChatLogin(Guid id, string jwtToken)
        {
            if (!AuthCheck(jwtToken))
                return;

            var user = _dbContext.Users.SingleOrDefault(user => user.Id == id && user.IsActive);

            if (user == null)
                return;

            ClientModel client = new ClientModel
            {
                ConnectionId = Context.ConnectionId,
                Nickname = user.Nickname
            };

            ClientSource.Clients.Add(client);

            await Clients.Others.SendAsync("clientJoined", user.Nickname);
        }
        public async Task TestSendMessage(string message)
        {
            await Clients.All.SendAsync("newMessage", "anonymous", message);
        }

        private bool AuthCheck(string jwtToken)
        {
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
