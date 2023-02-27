using System.Globalization;
using InertiaAdapter;

namespace OregonNexus.Broker.Web;

public class InertiaSharedMiddleware
{
    private readonly RequestDelegate _next;

    public InertiaSharedMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Inertia.Share = new { User = new {
                name = context.User.Identity?.Name!
            } 
        };

        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }
}

public static class InertiaSharedMiddlewareExtensions
{
    public static IApplicationBuilder UseInertiaShared(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<InertiaSharedMiddleware>();
    }
}