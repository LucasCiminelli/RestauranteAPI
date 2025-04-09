
using Restaurants.Domain.Exceptions;

namespace Restaurants.API.Middlewares
{
    public class ErrorHandlingMiddleware : IMiddleware
    {

        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch(NotFoundException notFound)
            {
                _logger.LogWarning(notFound, notFound.Message);

                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(notFound.Message);
            }
            catch(ForbidException forbidden)
            {
                _logger.LogWarning(forbidden, forbidden.Message);
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync($"Access Forbidden");
            }
            catch(UnauthorizedAccessException unauthorizedException)
            {
                _logger.LogWarning(unauthorizedException, unauthorizedException.Message);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went wrong");

            }
        }
    }
}
