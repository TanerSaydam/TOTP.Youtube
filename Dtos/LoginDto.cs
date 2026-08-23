namespace TOTPDemo.WebAPI.Dtos;

public sealed record LoginDto(
    string Email,
    string Password);