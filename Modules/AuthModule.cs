using Carter;
using System.Security.Claims;
using TOTPDemo.WebAPI.Context;
using TOTPDemo.WebAPI.Dtos;
using TOTPDemo.WebAPI.Models;
using TOTPDemo.WebAPI.Services;
using TS.Result;

namespace TOTPDemo.WebAPI.Modules;

public sealed class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder group)
    {
        var app = group.MapGroup("/auth");

        app.MapPost("/register", (RegisterDto request, ApplicationDbContext dbContext, TotpService totpService) =>
        {
            var secret = totpService.GenerateSecret();
            User user = new()
            {
                Email = request.Email,
                Password = request.Password,
                TOTPSecret = secret
            };
            dbContext.Add(user);
            dbContext.SaveChanges();
            //var res = Result<string>.Succeed("Kayıt işlemi başarıyla tamamlandı");

            //test amaçlı kodun ne olduğunu gördük
            //var code = totpService.GenerateCode(user.TOTPSecret);

            //return Results.Ok(res);

            var uri = totpService.GenerateOtpUri(secret, user.Email);
            var qrCode = totpService.GenerateQrCode(secret, user.Email);


            return Results.File(
                qrCode,
                contentType: "image/png");
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

            var token = jwtProvider.CreateTokenForTOTP(user);
            var res = Result<string>.Succeed(token);
            return Results.Ok(res);
        });

        app.MapGet("/verify", (
            string code,
            ApplicationDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            TotpService totpService,
            JwtProvider jwtProvider) =>
        {
            string? userId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Results.BadRequest(Result<string>.Failure("Token geçersiz"));
            }

            var user = dbContext.Users.FirstOrDefault(p => p.Id == Guid.Parse(userId));
            if (user is null)
            {
                return Results.BadRequest(Result<string>.Failure("Kullanıcı bulunamadı"));
            }

            var secret = user.TOTPSecret;
            //var code2 = totpService.GenerateCode(secret); tamamen test amaçlı
            var isVerify = totpService.Verify(secret, code);
            if (!isVerify)
            {
                return Results.BadRequest(Result<string>.Failure("Geçersiz kod"));
            }

            var token = jwtProvider.CreateToken(user);
            return Results.Ok(Result<string>.Succeed(token));
        }).RequireAuthorization("TotpPolicy");
    }
}
