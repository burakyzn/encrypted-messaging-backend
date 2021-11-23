using System;
using System.Collections.Generic;
using System.Text;

namespace SecuredChatApp.Core.DTOs
{
    public class GetMessagesResponse
    {
        public List<GetMessagesModel> response { get; set; }

        public GetMessagesResponse(List<GetMessagesModel> getMessages)
        {
            response = new List<GetMessagesModel>();

            foreach (var item in getMessages)
            {
                GetMessagesModel model = new GetMessagesModel
                {
                    Sender = item.Sender,
                    To = item.To,
                    Message = string.IsNullOrEmpty(item.Message) ? "" : item.Message,
                    SendDate = item.SendDate,
                    Read = item.Read
                };

                response.Add(model);
            }
        }
    }
}
