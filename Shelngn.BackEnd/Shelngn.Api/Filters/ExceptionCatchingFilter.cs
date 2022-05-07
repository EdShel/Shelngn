using Microsoft.AspNetCore.Mvc.Filters;
using Shelngn.Exceptions;
using System.Text;
using System.Text.Json;

namespace Shelngn.Api.Filters
{
    public sealed class ExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this.next.Invoke(context);
            }
            catch (Exception exception)
            {
                var restException = exception as RestException;
                context.Response.StatusCode = (int?)(restException?.StatusCode) ?? 500;
                context.Response.ContentType = "application/json";
                var errorObject = new
                {
                    Code = context.Response.StatusCode,
                    Message = exception.Message
                };
                //#if DEBUG
                if (errorObject.Code == 500)
                {
                    throw;
                }
                //#endif
                string jsonError = JsonSerializer.Serialize(errorObject);

                await context.Response.Body.WriteAsync(
                    Encoding.UTF8.GetBytes(jsonError)
                );
            }
        }
    }
}
