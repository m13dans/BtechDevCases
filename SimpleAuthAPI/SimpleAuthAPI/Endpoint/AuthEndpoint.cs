using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SimpleAuthAPI.Filter;
using SimpleAuthAPI.Model;
using SimpleAuthAPI.Service;
using System.Security.Claims;

namespace SimpleAuthAPI.Endpoint;

public static class AuthEndpoint
{
    public static IEndpointRouteBuilder MapAuthEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", Register);
        group.MapPost("/login", Login);
        group.MapGet("/users", GetAll);
        group.MapGet("/restricted", GetResticted)
             .RequireAuthorization()
             .AddEndpointFilter<RefreshTokenFilter>();

        return app;
    }

    public static async Task<ResponseModel<int>> Register([FromBody] RegisterUserRequest request, AuthService service) =>
        await service.RegisterUser(request);

    public static async Task<ResponseModel<string>> Login([FromBody] LoginUserRequest request, AuthService service) =>
        await service.LoginUser(request);

    public static async Task<ResponseModel<List<string>>> GetAll(AuthService service) =>
        await service.GetAll();

    public static ResponseModel<string> GetResticted(AuthService service, HttpContext context)
    {
        var user = context.User;
        var email = user.FindFirstValue(ClaimTypes.Email);

        return new ResponseModel<string>() 
        { 
            Data = $"Hello {email}, welcome back",
            Message = "Success",
            ResponseType = ResponseType.Success
        };
    }
        
}
