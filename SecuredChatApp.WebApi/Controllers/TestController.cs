using Microsoft.AspNetCore.Mvc;

namespace SecuredChatApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(new
            {
                resut = true,
                data = "test answer"
            });
        }
    }
}
