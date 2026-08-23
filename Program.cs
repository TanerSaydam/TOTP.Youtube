using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using TOTPDemo.WebAPI;
using TOTPDemo.WebAPI.Context;
using TOTPDemo.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddScoped<JwtProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<TotpService>();
builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseInMemoryDatabase("mydb"));
builder.Services.AddCarter();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("AccessToken", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JWT:SecretKey"]!))
        };
    })
    .AddJwtBearer("TotpToken", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JWT_TOTP:Issuer"],
            ValidAudience = builder.Configuration["JWT_TOTP:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JWT_TOTP:SecretKey"]!))
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder("AccessToken")
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("TotpPolicy", policy =>
    {
        policy.AddAuthenticationSchemes("TotpToken");
        policy.RequireAuthenticatedUser();
    });
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();

app.Run();