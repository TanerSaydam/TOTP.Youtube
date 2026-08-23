namespace TOTPDemo.WebAPI.Models;

public sealed class User
{
    public User()
    {
        Id = Guid.CreateVersion7();
    }
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
