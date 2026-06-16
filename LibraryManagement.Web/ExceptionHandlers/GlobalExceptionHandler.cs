using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Services;
using Microsoft.AspNetCore.Diagnostics;

namespace LibraryManagement.API.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) 
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            int statusCode;
            string message;

            switch (exception)
            {
                case NotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    message = exception.Message;
                    break;

                case BadRequestException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = exception.Message;
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "Unexpected error occurred";
                    break;
            }
            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(new
            {
                StatusCode = statusCode,
                Message = message
            }, cancellationToken);

            _logger.LogError(exception.Message, "Unhandled Exception occurred");
            return true;
        }
    }
}
