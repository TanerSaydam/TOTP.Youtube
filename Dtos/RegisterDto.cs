namespace TOTPDemo.WebAPI.Dtos;

public sealed record RegisterDto(
    string Email,
    string Password);