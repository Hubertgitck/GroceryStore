using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Application.Utility.Exceptions;

namespace Application.Utility.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private int _statusCode;

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
                _statusCode = StatusCodes.Status403Forbidden;
                await HandleExceptionAsync(context,forbiddenException);
            }

            catch (BadRequestException badRequestException)
            {
                _statusCode = StatusCodes.Status400BadRequest;
                await HandleExceptionAsync(context, badRequestException);
            }

            catch(NotFoundException notFoundException)
            {
                _statusCode = StatusCodes.Status404NotFound;
                await HandleExceptionAsync(context, notFoundException);
            }

            catch(ArgumentException argumentException)
            {
                _statusCode = StatusCodes.Status400BadRequest;
                await HandleExceptionAsync(context, argumentException);
            }

            catch(Exception exception)
            {
                _statusCode = StatusCodes.Status500InternalServerError;
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, exception.Message);

            context.Response.Redirect($"/Customer/Home/Error?StatusCode={_statusCode}&Message={exception.Message}");
        }
    }
}
