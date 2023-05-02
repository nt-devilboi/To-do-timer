namespace To_do_timer.MiddleWares;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    
    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public Task Invoke(HttpContext httpContext)
    {
        var path = httpContext.Request.Path;
        if(path.HasValue && path.Value.StartsWith("/admin"))
        {
            if (httpContext.Session.GetString("username") == null)
            {
                httpContext.Response.Redirect("/login/index");
            }
        }
        return _next(httpContext);
    }
}

public static class AuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthenticationMiddleware>();
    }
}