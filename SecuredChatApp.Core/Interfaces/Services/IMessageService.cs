using SecuredChatApp.Core.DTOs;

namespace SecuredChatApp.Core.Interfaces.Services
{
    public interface IMessageService
    {
        public ResultModel<object> GetMessageBox(GetMessageBoxRequest request);
        public ResultModel<object> GetMessages(GetMessagesRequest request);
    }
}
