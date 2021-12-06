using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecuredChatApp.Business.Helpers;
using SecuredChatApp.Core.DTOs;
using SecuredChatApp.Core.Entities;
using SecuredChatApp.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ClientModel client = ClientSource.Clients.FirstOrDefault(client => client.ConnectionId == Context.ConnectionId);
            ClientSource.Clients.Remove(client);

            string json = JsonSerializer.Serialize(ClientSource.Clients.Select(clients => clients.UserID).ToList());
            await Clients.Others.SendAsync("onlineList", json);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task Connection(Guid id, string jwtToken)
        {
            if (!AuthCheck(jwtToken))
                return;

            var user = _dbContext.Users.SingleOrDefault(user => user.Id == id && user.IsActive);

            if (user == null)
                return;

            ClientModel client = new ClientModel
            {
                ConnectionId = Context.ConnectionId,
                UserID = user.Id,
                Nickname = user.Nickname
            };

            if(ClientSource.Clients.Any(client => client.UserID == id))
                ClientSource.Clients.Remove(ClientSource.Clients.FirstOrDefault(client => client.UserID == id));

            ClientSource.Clients.Add(client);

            string json = JsonSerializer.Serialize(ClientSource.Clients.Select(clients => clients.UserID).ToList());
            await Clients.All.SendAsync("onlineList", json);
        }

        public async Task SendMessage(Guid id, Guid friendId, string message, string sendDate, string jwtToken)
        {
            if (string.IsNullOrEmpty(message) || !AuthCheck(jwtToken))
                return;

            var user = _dbContext.Users.SingleOrDefault(user => user.Id == id && user.IsActive);

            if (user == null)
                return;

            ClientModel client = ClientSource.Clients.FirstOrDefault(client => client.UserID == friendId);

            if (client == null)
                return;

            MessageEntity messageEntity = new MessageEntity()
            {
                Sender = id,
                To = friendId,
                Message = message,
                Created = Convert.ToDateTime(sendDate),
                Creator = id.ToString(),
                Read = false
            };
            _dbContext.Messages.Add(messageEntity);
            _dbContext.SaveChanges();

            await Clients.Client(client.ConnectionId).SendAsync("receiveMessage", user.Id.ToString(), message, sendDate);
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
