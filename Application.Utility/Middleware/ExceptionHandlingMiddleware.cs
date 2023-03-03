using System.Web;
using Application.Utility.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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
                HandleException(context,forbiddenException);
            }

            catch (BadRequestException badRequestException)
            {
                _statusCode = StatusCodes.Status400BadRequest;
                HandleException(context, badRequestException);
            }

            catch(NotFoundException notFoundException)
            {
                _statusCode = StatusCodes.Status404NotFound;
                HandleException(context, notFoundException);
            }

            catch(ArgumentException argumentException)
            {
                _statusCode = StatusCodes.Status400BadRequest;
                HandleException(context, argumentException);
            }

            catch(Exception exception)
            {
                _statusCode = StatusCodes.Status500InternalServerError;
                HandleException(context, exception);
            }
        }

        private void HandleException(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            var messageEncoded = HttpUtility.UrlEncode(exception.Message);

            context.Response.Redirect($"/Customer/Home/Error?StatusCode={_statusCode}&Message={messageEncoded}");
        }
    }
}
