using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TOTPDemo.WebAPI.Models;

namespace TOTPDemo.WebAPI;

public sealed class JwtProvider(IConfiguration configuration)
{
    public string CreateToken(User user)
    {
        var issuer = configuration.GetSection("JWT:Issuer").Value;
        var audience = configuration.GetSection("JWT:Audience").Value;
        var secretKey = configuration.GetSection("JWT:SecretKey").Value!;

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var expires = DateTime.Now.AddDays(1);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwtSecurityToken = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: expires,
            signingCredentials: signingCredentials);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(jwtSecurityToken);

        return token;
    }
}
