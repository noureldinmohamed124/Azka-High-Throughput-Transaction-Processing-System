using Azka_Transaction_Processing_System.API.Commons;
using Azka_Transaction_Processing_System.Application.Exceptions;
using System.Security;
using System.Text.Json;

namespace Azka_Transaction_Processing_System.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "Handled application error");
                await HandleAppException(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await HandleUnknownException(context, ex);
            }
        }

        private static async Task HandleAppException(HttpContext context, AppException ex)
        {
            context.Response.ContentType = "application/json";

            //context.Response.StatusCode = ex switch
            //{
            //    ForbiddenException => StatusCodes.Status403Forbidden,
            //    NotFoundException => StatusCodes.Status404NotFound,
            //    ValidationException => StatusCodes.Status400BadRequest,
            //    BadRequestException => StatusCodes.Status400BadRequest,
            //    UnauthorizedException => StatusCodes.Status401Unauthorized,
            //    BusinessRuleException => StatusCodes.Status409Conflict,
            //    ConflictException => StatusCodes.Status409Conflict,
            //    _ => StatusCodes.Status400BadRequest
            //};

            context.Response.StatusCode = (int)ex.StatusCode;

            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Fail(ex.Message)
            );
        }

        private static async Task HandleUnknownException(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Fail("An unexpected error occurred")
            );
        }
    }
}
