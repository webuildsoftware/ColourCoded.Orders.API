using Microsoft.EntityFrameworkCore;
using ColourCoded.Orders.API.Data.Entities.Security;

namespace ColourCoded.Orders.API.Data
{
  public class SecurityContext : DbContext
  {
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Salt> Salts { get; set; }

    public SecurityContext(DbContextOptions<SecurityContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.ApplyConfiguration(new UserMapping());
      modelBuilder.ApplyConfiguration(new SaltMapping());
      modelBuilder.ApplyConfiguration(new SessionMapping());
    }
  }
}
