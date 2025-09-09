using ChatFlow.Application.Exceptions;
using ChatFlow.Models;
using System.Net;

namespace ChatFlow.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);

            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {

            context.Response.ContentType = "application/json";

            switch (exception)
            {

                case BadRequestException e:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case NotFoundException e:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case UnauthorizedException e:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            ErrorResponse errorResponse;

            try
            {
                errorResponse = new ErrorResponse
                {
                    ErrorCode = ((CustomException)exception).ErrorCode,
                    HttpStatusCode = context.Response.StatusCode,
                    Error = exception.Message,
                    Exception = exception.GetType().Name
                };
            }
            catch
            {
                errorResponse = new ErrorResponse
                {
                    HttpStatusCode = context.Response.StatusCode,
                    Error = exception.Message,
                    Exception = exception.GetType().Name
                };

            }

            return context.Response.WriteAsync(errorResponse.ToString());

        }

    }
}
