using Microsoft.AspNetCore.Builder;
using SecuredChatApp.WebApi.Middlewares;

namespace SecuredChatApp.WebApi{
    static public class Extensions
    {
        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}