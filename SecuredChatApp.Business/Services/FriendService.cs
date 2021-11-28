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
    public class FriendService : IFriendService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AppSettings _appSettings;

        public FriendService(
            ApplicationDbContext dbContext,
            IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _appSettings = appSettings.Value;
        }

        public ResultModel<object> AddFriend(AddFriendRequest request)
        {
            var user = _dbContext.Users.SingleOrDefault(user => user.Id == request.Id && user.IsActive);

            if (user.Email == request.ToEmail)
                return new ResultModel<object>(data: "You can't friend request to yourself!", type: ResultModel<object>.ResultType.FAIL);

            var requestTo = _dbContext.Users.SingleOrDefault(user => user.Email == request.ToEmail && user.IsActive);

            if (requestTo == null)
                return new ResultModel<object>(data: "User does not exist!", type: ResultModel<object>.ResultType.FAIL);

            if (!CheckSingleAddFriendRequest(user.Id, requestTo.Id))
                return new ResultModel<object>(data: "Only one friend request can be sent to a person!", type: ResultModel<object>.ResultType.FAIL);

            FriendEntity friendEntity = new FriendEntity
            {
                Creator = request.Id.ToString(),
                IsRequest = true,
                SenderUserID = user.Id,
                ReceiverID = requestTo.Id,
                IsActive = true
            };

            _dbContext.Friends.Add(friendEntity);

            int result = _dbContext.SaveChanges();

            if (result < 0)
                return new ResultModel<object>(data: "An unexpected error has occurred.", type: ResultModel<object>.ResultType.FAIL);

            return new ResultModel<object>();
        }

        private bool CheckSingleAddFriendRequest(Guid SenderUserID, Guid ReceiverID)
        {
            return !_dbContext.Friends.Any(friend =>
                ((friend.SenderUserID == SenderUserID && friend.ReceiverID == ReceiverID) ||
                (friend.ReceiverID == SenderUserID && friend.SenderUserID == ReceiverID)) &&
                friend.IsActive
            );
        }

        public ResultModel<object> GetAddFriendRequests(GetAddFriendRequest request)
        {
            var user = _dbContext.Users.SingleOrDefault(user => user.Id == request.Id && user.IsActive);

            if (user == null)
                return new ResultModel<object>(data: "User does not exist!", type: ResultModel<object>.ResultType.FAIL);

            var requests = _dbContext.Friends.Where(requests =>
                requests.ReceiverID == user.Id &&
                requests.IsRequest == true &&
                requests.IsActive
            ).ToList();

            List<Guid> friendsId = new List<Guid>();
            foreach (var item in requests)
            {
                if (item.SenderUserID == user.Id)
                    friendsId.Add(item.ReceiverID);
                else
                    friendsId.Add(item.SenderUserID);
            }

            var friendUsers = _dbContext.Users.Where(users => friendsId.Contains(users.Id) && users.IsActive);

            List<GetAddFriendResultModel> result = new List<GetAddFriendResultModel>();
            foreach (var item in requests)
            {
                foreach (var item2 in friendUsers)
                {
                    if (item.SenderUserID == item2.Id || item.ReceiverID == item2.Id)
                    {
                        GetAddFriendResultModel resultModel = new GetAddFriendResultModel()
                        {
                            FriendEmail = item2.Email,
                            addFriendRequest = item
                        };
                        result.Add(resultModel);
                    }
                }
            }

            return new ResultModel<object>(data: new GetAddFriendResponse(result));
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

        public ResultModel<object> GetFriends(GetFriendsRequest request)
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

            Dictionary<Guid, string> list = new Dictionary<Guid, string>();
            foreach (var item in friendUsers)
            {
                foreach (var item2 in friends)
                {
                    if(item2.SenderUserID == item.Id || item2.ReceiverID == item.Id)
                        list.Add(item2.Id, String.Concat(item.Nickname, " (", item.Email, ")"));
                }
            }

            return new ResultModel<object>(data: new GetFriendsResponse(list));
        }

        public ResultModel<object> RemoveFriend(RemoveFriendRequest request)
        {
            var friend = _dbContext.Friends.SingleOrDefault(friend => friend.Id == request.Id && !friend.IsRequest && friend.IsActive);

            if (friend == null)
                return new ResultModel<object>(data: "Friend does not exist!", type: ResultModel<object>.ResultType.FAIL);

            friend.IsActive = false;

            _dbContext.Friends.Update(friend);

            int result = _dbContext.SaveChanges();

            if (result < 0)
                return new ResultModel<object>(data: "An unexpected error has occurred.", type: ResultModel<object>.ResultType.FAIL);

            return new ResultModel<object>();
        }
    }
}
