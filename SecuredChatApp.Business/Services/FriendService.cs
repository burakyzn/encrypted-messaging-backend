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

        public ResultModel<object> GetFriends(GetFriendsRequest request)
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
            ).Select(friend => new { friend.Nickname, friend.Email });

            var x = friends.Join(friendsNicknameAndEmails,
                friend => friend.User,
                friendsNicknameAndEmail => friendsNicknameAndEmail.Email,
                (friend, friendsNicknameAndEmail) => new {
                    friend.Id,
                    friendsNicknameAndEmail.Nickname,
                    friendsNicknameAndEmail.Email
                }
            );

            var y = friends.Join(friendsNicknameAndEmails,
                friend => friend.With,
                friendsNicknameAndEmail => friendsNicknameAndEmail.Email,
                (friend, friendsNicknameAndEmail) => new {
                    friend.Id,
                    friendsNicknameAndEmail.Nickname,
                    friendsNicknameAndEmail.Email
                }
            );

            var result = x.Concat(y);

            Dictionary<Guid, string> list = new Dictionary<Guid, string>();
            foreach (var item in result)
            {
                list.Add(item.Id, String.Concat(item.Nickname, " (", item.Email, ")"));
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
