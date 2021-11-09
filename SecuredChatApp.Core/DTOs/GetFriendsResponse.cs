using SecuredChatApp.Core.Entities;
using System;
using System.Collections.Generic;

namespace SecuredChatApp.Core.DTOs
{
    public class GetFriendsResponse
    {
        public List<GetFriendsResponseModel> requests { get; set; }

        public GetFriendsResponse(Dictionary<Guid, string> getFriends)
        {
            requests = new List<GetFriendsResponseModel>();

            foreach (var item in getFriends)
            {
                GetFriendsResponseModel model = new GetFriendsResponseModel
                {
                    Id = item.Key,
                    With = item.Value
                };

                requests.Add(model);
            }
        }
    }
}
