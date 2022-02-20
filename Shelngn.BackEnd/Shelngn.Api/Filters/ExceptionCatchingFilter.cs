using Microsoft.AspNetCore.Mvc.Filters;
using Shelngn.Exceptions;

namespace Shelngn.Api.Filters
{
    public class ExceptionCatchingFilter : IExceptionFilter
    {
        private ILogger<ExceptionCatchingFilter> logger;

        public ExceptionCatchingFilter(ILogger<ExceptionCatchingFilter> logger)
        {
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is RestException restError)
            {
                context.HttpContext.Response.StatusCode = (int)restError.StatusCode;
                context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    StatusCode = (int)restError.StatusCode,
                    Message = restError.Message,
                });
            }
            //else
            //{
            //    this.logger.LogError(context.Exception, "Server could not handle the request {Url}.", context.HttpContext.Request.Path);
            //    context.HttpContext.Response.StatusCode = 500;
            //    context.HttpContext.Response.WriteAsJsonAsync(new
            //    {
            //        StatusCode = 500,
            //        Message = context.Exception.Message,
            //        StackTrace = context.Exception.StackTrace,
            //    });
            //}
        }
    }
}
