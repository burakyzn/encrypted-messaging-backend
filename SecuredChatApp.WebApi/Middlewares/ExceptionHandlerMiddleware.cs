using System;
using Microsoft.AspNetCore.Http;
using SecuredChatApp.Core.Models;
using System.Net;
using System.Threading.Tasks;

namespace SecuredChatApp.WebApi.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception error)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(
                new ResultModel<object>(
                    data: String.Concat("An unexpected error has occurred : ", error?.Message),
                    type: ResultModel<object>.ResultType.FAIL)
            ));
        }
    }
}