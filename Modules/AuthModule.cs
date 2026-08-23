using Carter;
using TOTPDemo.WebAPI.Context;
using TOTPDemo.WebAPI.Dtos;
using TOTPDemo.WebAPI.Models;
using TS.Result;

namespace TOTPDemo.WebAPI.Modules;

public sealed class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder group)
    {
        var app = group.MapGroup("/auth");

        app.MapPost("/register", (RegisterDto request, ApplicationDbContext dbContext) =>
        {
            User user = new()
            {
                Email = request.Email,
                Password = request.Password
            };
            dbContext.Add(user);
            dbContext.SaveChanges();
            var res = Result<string>.Succeed("Kayıt işlemi başarıyla tamamlandı");
            return Results.Ok(res);
        });

        app.MapPost("/login", (LoginDto request, ApplicationDbContext dbContext, JwtProvider jwtProvider) =>
        {
            var user = dbContext.Users
                    .FirstOrDefault(p => p.Email == request.Email && p.Password == request.Password);
            if (user is null)
            {
                var error = Result<string>.Failure("Geçersiz kullanıcı adı ya da şifre");
                return Results.BadRequest(error);
            }

            var token = jwtProvider.CreateToken(user);
            var res = Result<string>.Succeed(token);
            return Results.Ok(res);
        });
    }
}
