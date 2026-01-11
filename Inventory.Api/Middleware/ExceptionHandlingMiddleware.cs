using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace Inventory.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new
            {
                message = ex.Message,
                statusCode = (int)HttpStatusCode.InternalServerError
            };

            switch (ex)
            {
                case KeyNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = new { message = ex.Message, statusCode = response.StatusCode };
                    break;

                case ArgumentException:
                case InvalidOperationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new { message = ex.Message, statusCode = response.StatusCode };
                    break;

                case DbUpdateException dbEx when dbEx.InnerException is SqlException sqlEx:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    errorResponse = new 
                    { 
                        message = sqlEx.Number == 2627 || sqlEx.Number == 2601 
                            ? "A record with this code already exists" 
                            : "Database error occurred",
                        statusCode = response.StatusCode 
                    };
                    break;

                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse = new { message = "Unauthorized access", statusCode = response.StatusCode };
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new { message = "An internal error occurred", statusCode = response.StatusCode };
                    break;
            }

            await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}
