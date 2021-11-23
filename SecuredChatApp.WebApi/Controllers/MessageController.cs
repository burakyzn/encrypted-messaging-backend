using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuredChatApp.Core.DTOs;
using SecuredChatApp.Core.Interfaces.Services;

namespace SecuredChatApp.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("GetMessageBox")]
        public ResultModel<object> GetMessageBox([FromBody] GetMessageBoxRequest request)
        {
            var result = _messageService.GetMessageBox(request);
            return result;
        }

        [HttpPost("GetMessages")]
        public ResultModel<object> GetMessages([FromBody] GetMessagesRequest request)
        {
            var result = _messageService.GetMessages(request);
            return result;
        }
    }
}
