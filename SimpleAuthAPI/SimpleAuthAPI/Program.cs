using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SimpleAuthAPI.Data;
using SimpleAuthAPI.Endpoint;
using SimpleAuthAPI.Repository;
using SimpleAuthAPI.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton( _ =>
{
    var connectionString = builder.Configuration.GetConnectionString("Database") ?? throw new ApplicationException("Database connection string is not configured");
    return new ConnectionFactory(connectionString);
});

// register global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// register service or repo
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// register auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;

    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? throw new ApplicationException("JWT Key is not configured"))),
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors();

// apply database migrations
DbUpdate.ApplyMigrations(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler(options => { });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapAuthEndpoint();


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast").RequireAuthorization();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(o =>
{
    if (app.Environment.IsDevelopment())
    {
        o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    }
    else
    {
        o.WithOrigins(builder.Configuration["Frontend:Site"]!)
            .AllowAnyHeader().AllowAnyMethod();
    }
});

app.Run();


internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

