using System.Collections.Generic;
using System.Linq;

namespace SecuredChatApp.Core.DTOs
{
    public class GetMessageBoxResponse
    {
        public List<GetMessageBoxModel> response { get; set; }

        public GetMessageBoxResponse(List<GetMessageBoxModel> getMessageBoxes)
        {
            response = new List<GetMessageBoxModel>();

            foreach (var item in getMessageBoxes)
            {
                GetMessageBoxModel model = new GetMessageBoxModel
                {
                    FriendId = item.FriendId,
                    Message = string.IsNullOrEmpty(item.Message) ? "" : item.Message,
                    Nickname = item.Nickname,
                    SendDate = item.SendDate,
                    Read = item.Read
                };

                response.Add(model);
            }
        }
    }
}
