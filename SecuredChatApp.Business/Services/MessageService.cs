using Microsoft.Extensions.Options;
using SecuredChatApp.Business.Helpers;
using SecuredChatApp.Core.DTOs;
using SecuredChatApp.Core.Entities;
using SecuredChatApp.Core.Interfaces.Services;
using SecuredChatApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SecuredChatApp.Business.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AppSettings _appSettings;

        public MessageService(
            ApplicationDbContext dbContext,
            IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _appSettings = appSettings.Value;
        }

        public ResultModel<object> GetMessageBox(GetMessageBoxRequest request)
        {
            var user = _dbContext.Users.SingleOrDefault(user => user.Id == request.Id && user.IsActive);

            if (user == null)
                return new ResultModel<object>(data: "User does not exist!", type: ResultModel<object>.ResultType.FAIL);

            var friends = _dbContext.Friends.Where(friends =>
                (
                    friends.User == user.Email ||
                    friends.With == user.Email
                ) &&
                friends.IsRequest == false &&
                friends.IsActive
            ).ToList();

            List<string> friendsEmail = new List<string>();
            foreach (var item in friends)
            {
                if (item.User == user.Email)
                    friendsEmail.Add(item.With);
                else
                    friendsEmail.Add(item.User);
            }

            var friendsNicknameAndEmails = _dbContext.Users.Where(friends =>
                friendsEmail.Contains(friends.Email) &&
                friends.IsActive
            ).Select(friend => new { friend.Id, friend.Nickname, friend.Email });

            List<GetMessageBoxModel> messageBoxList = new List<GetMessageBoxModel>();

            List<MessageEntity> lastMessages = _dbContext.Messages.Where(message =>
                    message.Sender == user.Id ||
                    message.To == user.Id
                ).OrderByDescending(message => message.Created).ToList();

            foreach (var item in friendsNicknameAndEmails)
            {
                GetMessageBoxModel box = new GetMessageBoxModel
                {
                    FriendId = item.Id,
                    Nickname = item.Nickname,
                    Read = true
                };
                foreach (var item2 in lastMessages)
                {
                    if((item.Id == item2.Sender && user.Id == item2.To) ||
                        (user.Id == item2.Sender && item.Id == item2.To))
                    {
                        box.Message = item2.Message;
                        box.SendDate = item2.Created;
                        box.Read = item2.Read;

                        break;
                    }
                }
                messageBoxList.Add(box);
            }

            messageBoxList.OrderByDescending(message => message.SendDate).ToList();

            List<GetMessageBoxModel> SendDateNotNull = messageBoxList.Where(message => message.SendDate != null).ToList();
            SendDateNotNull = SendDateNotNull.OrderByDescending(message => message.SendDate).ToList();

            List<GetMessageBoxModel> SendDateNull = messageBoxList.Where(message => message.SendDate == null).ToList();

            List<GetMessageBoxModel> result = SendDateNotNull.Concat(SendDateNull).ToList();

            return new ResultModel<object>(data: new GetMessageBoxResponse(result));
        }
    }
}
