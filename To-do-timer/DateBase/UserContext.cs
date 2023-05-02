using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using To_do_timer.Models;

namespace To_do_timer.DateBase;

public class UserContext : IdentityDbContext<User>
{
    public DbSet<User> application => Set<User>();

    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=TDT;Username=postgres;Password=Last");
    }
}