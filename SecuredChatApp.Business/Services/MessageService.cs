﻿using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
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
                    friends.SenderUserID == user.Id ||
                    friends.ReceiverID == user.Id
                ) &&
                friends.IsRequest == false &&
                friends.IsActive
            ).ToList();

            List<Guid> friendsId = new List<Guid>();
            foreach (var item in friends)
            {
                if (item.SenderUserID == user.Id)
                    friendsId.Add(item.ReceiverID);
                else
                    friendsId.Add(item.SenderUserID);
            }

            var friendUsers = _dbContext.Users.Where(users => friendsId.Contains(users.Id) && users.IsActive);

            List<GetMessageBoxModel> messageBoxList = new List<GetMessageBoxModel>();

            List<MessageEntity> lastMessages = _dbContext.Messages.Where(message =>
                    message.Sender == user.Id ||
                    message.To == user.Id
                ).OrderByDescending(message => message.Created).ToList();

            foreach (var item in friendUsers)
            {
                GetMessageBoxModel box = new GetMessageBoxModel
                {
                    FriendId = item.Id,
                    Nickname = item.Nickname,
                    AvatarUrl = item.ProfileImageUrl,
                    Read = true
                };
                foreach (var item2 in lastMessages)
                {
                    if((item.Id == item2.Sender && user.Id == item2.To) ||
                        (user.Id == item2.Sender && item.Id == item2.To))
                    {
                        box.Message = item2.Message;
                        box.SendDate = item2.Created;
                        box.LastSenderUserId = item2.Sender;
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

        public ResultModel<object> GetMessages(GetMessagesRequest request)
        {
            var user = _dbContext.Users.SingleOrDefault(user => user.Id == request.Id && user.IsActive);

            if (user == null)
                return new ResultModel<object>(data: "User does not exist!", type: ResultModel<object>.ResultType.FAIL);

            var messages = _dbContext.Messages.Where(message =>
                (
                    (message.Sender == user.Id && message.To == request.FriendId) ||
                    (message.To == user.Id && message.Sender == request.FriendId)
                ) &&
                message.IsActive
            ).OrderByDescending(message => message.Created).ToList();

            List<GetMessagesModel> result = new List<GetMessagesModel>();
            foreach (MessageEntity message in messages)
            {
                GetMessagesModel getMessageModel = new GetMessagesModel
                {
                    Sender = message.Sender,
                    To = message.To,
                    Message = message.Message,
                    Read = message.Read,
                    SendDate = message.Created
                };

                result.Add(getMessageModel);
            }

            return new ResultModel<object>(data: new GetMessagesResponse(result));
        }
    
        public ResultModel<object> GetDHParameterOfMessageBox(GetDHParameterOfMessageBoxRequest request){
            var messageBoxRecord = _dbContext.MessageBoxes
                .Where(field => (field.Sender == request.SenderUserId && field.To == request.ToUserId) 
                    || (field.To == request.SenderUserId && field.Sender == request.ToUserId))
                .SingleOrDefault();
            
            if(messageBoxRecord == null){
                var generator = new DHParametersGenerator();
                generator.Init(1024, 1, new SecureRandom());
                var generatedNumbers = generator.GenerateParameters();

                var newMessageBox = new MessageBoxEntity(){
                    Sender = request.SenderUserId,
                    To = request.ToUserId,
                    NumberP = generatedNumbers.P.ToString(),
                    NumberG = generatedNumbers.G.ToString()
                };

                _dbContext.Add(newMessageBox);
                _dbContext.SaveChanges();

                return new ResultModel<object>(data: new {
                    NumberP = newMessageBox.NumberP,
                    NumberG = newMessageBox.NumberG
                });
            }

            return new ResultModel<object>(data: new {
                NumberP = messageBoxRecord.NumberP,
                NumberG = messageBoxRecord.NumberG
            });
        }
    
        public ResultModel<object> SetPublicKey(SetPublicKeyRequest request){
            var messageBoxRecord = _dbContext.MessageBoxes
                .Where(field => (field.Sender == request.SenderUserId && field.To == request.ToUserId) 
                    || (field.To == request.SenderUserId && field.Sender == request.ToUserId))
                .SingleOrDefault();

            if(messageBoxRecord == null)
                return new ResultModel<object>(message: "Mesaj kutusu bulunmadı!", type: ResultModel<object>.ResultType.FAIL);

            if(messageBoxRecord.Sender == request.SenderUserId){
                messageBoxRecord.FromPublicKey = request.PublicKey;
            } else {
                messageBoxRecord.ToPublicKey = request.PublicKey;   
            }

            _dbContext.Update(messageBoxRecord);
            _dbContext.SaveChanges();

            return new ResultModel<object>();
        }
    
        public ResultModel<object> GetPublicKeyFromMessageBox(GetPublicKeyRequest request){
            var messageBoxRecord = _dbContext.MessageBoxes
                .Where(field => (field.Sender == request.MyId && field.To == request.FriendId) 
                    || (field.To == request.MyId && field.Sender == request.FriendId))
                .SingleOrDefault();

            if(messageBoxRecord.Sender == request.MyId){
                return new ResultModel<object>(data : new {
                    PublicKey = messageBoxRecord.ToPublicKey
                });
            }

            return new ResultModel<object>(data : new {
                PublicKey = messageBoxRecord.FromPublicKey
            });
        }
    }
}
