using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Application.Utility.Exceptions;

namespace Application.Utility.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            
            catch (ForbiddenException forbiddenException)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await HandleExceptionAsync(context,forbiddenException);
            }

            catch (BadRequestException badRequestException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await HandleExceptionAsync(context, badRequestException);
            }

            catch(NotFoundException notFoundException)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await HandleExceptionAsync(context, notFoundException);
            }

            catch(ArgumentException argumentException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await HandleExceptionAsync(context, argumentException);
            }

            catch(Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Something went wrong");
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            await context.Response.WriteAsync(exception.Message);
        }
    }
}
