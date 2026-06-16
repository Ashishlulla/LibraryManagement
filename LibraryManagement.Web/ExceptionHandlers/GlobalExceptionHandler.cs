using LibraryManagement.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace LibraryManagement.API.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
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

            return true;
        }
    }
}
