using Microsoft.EntityFrameworkCore;
using TOTPDemo.WebAPI.Models;

namespace TOTPDemo.WebAPI.Context;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}
