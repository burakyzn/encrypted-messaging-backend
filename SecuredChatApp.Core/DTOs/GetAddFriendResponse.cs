using SecuredChatApp.Core.Entities;
using System.Collections.Generic;

namespace SecuredChatApp.Core.DTOs
{
    public class GetAddFriendResponse
    {
        public List<GetAddFriendResponseModel> requests { get; set; }

        public GetAddFriendResponse(List<GetAddFriendResultModel> addFriendRequests)
        {
            requests = new List<GetAddFriendResponseModel>();

            foreach (var item in addFriendRequests)
            {
                GetAddFriendResponseModel model = new GetAddFriendResponseModel
                {
                    Id = item.addFriendRequest.Id,
                    FromEmail = item.FriendEmail
                };

                requests.Add(model);
            }
        }
    }
}
