using SimpleAuthAPI.Model;
using SimpleAuthAPI.Service;
using System.Security.Claims;

namespace SimpleAuthAPI.Filter;

public class RefreshTokenFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var result = await next(context); 

        var httpContext = context.HttpContext;
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            var email = httpContext.User.FindFirstValue(ClaimTypes.Email);
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var authService = httpContext.RequestServices.GetRequiredService<AuthService>();
            var newToken = authService.GenerateToken(int.Parse(userId), email);

            httpContext.Response.Headers["X-New-Access-Token"] = newToken;
        }

        return result;
    }
}