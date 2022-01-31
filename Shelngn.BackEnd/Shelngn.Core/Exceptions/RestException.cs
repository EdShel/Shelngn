using System.Net;

namespace Shelngn.Exceptions
{
    public class RestException : InvalidOperationException
    {
        public RestException(HttpStatusCode statusCode, string? message) : base(message)
        {
            this.StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }
    }

    public class BadRequestException : RestException
    {
        public BadRequestException(string? message) : base(HttpStatusCode.BadRequest, message)
        {
        }
    }

    public class NotFoundException : RestException
    {
        public NotFoundException(string? message) : base(HttpStatusCode.NotFound, message)
        {
        }
    }

    public class ForbiddenException : RestException
    {
        public ForbiddenException(string? message) : base(HttpStatusCode.Forbidden, message)
        {
        }
    }
    public class ConflictException : RestException
    {
        public ConflictException(string? message) : base(HttpStatusCode.Conflict, message)
        {
        }
    }

}
