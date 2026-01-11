using System.Net;
using System.Text.Json;

namespace Inventory.Api.Middleware
{
    public class AuthenticationResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    message = "Authentication required. Please provide a valid token.",
                    statusCode = 401
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    message = "You do not have permission to access this resource.",
                    statusCode = 403
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
