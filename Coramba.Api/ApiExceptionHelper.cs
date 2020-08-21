using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Coramba.Api
{
    public static class ApiExceptionHelper
    {
        public static async Task HandleException(HttpContext httpContext)
        {
            var feature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = feature.Error;

            var result = JsonSerializer.Serialize(new { error = exception.Message });
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(result);
        }
    }
}
