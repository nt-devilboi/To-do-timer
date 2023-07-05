using System.Net;
using System.Security.Claims;
using BBServer;
using BBServer.Extensions;
using Microsoft.AspNetCore.Authentication;
using To_do_timer.Models;

namespace To_do_timer.MiddleWares;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var userId = httpContext.User.FindFirstValue("id");
        if (httpContext.User.Identity?.IsAuthenticated ?? false)
        {
            var entityByUser = httpContext.Request.Form["userId"];
            if (userId != entityByUser)
            {
                httpContext.Response.StatusCode = 404;
                await httpContext.Response.WriteAsync("Эта книжка другого юзера");
            }
        }
        
        await _next(httpContext);
    }
}

public static class AuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthenticationMiddleware>();
    }
}