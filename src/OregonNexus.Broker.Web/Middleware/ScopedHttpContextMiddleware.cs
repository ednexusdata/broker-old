using OregonNexus.Broker.Web.Services;

namespace OregonNexus.Broker.Web;

public class ScopedHttpContextMiddleware 
{
    private readonly RequestDelegate _next;

    public ScopedHttpContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext context, ScopedHttpContext scopedContext)
    {
        scopedContext.HttpContext = context;
        return _next(context);
    }
}