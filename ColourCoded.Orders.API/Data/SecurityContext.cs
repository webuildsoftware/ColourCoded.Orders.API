using Microsoft.EntityFrameworkCore;
using ColourCoded.Orders.API.Data.Entities.Security;

namespace ColourCoded.Orders.API.Data
{
  public class SecurityContext : DbContext
  {
    public DbSet<Session> Sessions { get; set; }

    public SecurityContext(DbContextOptions<SecurityContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.ApplyConfiguration(new SessionMapping());
    }
  }
}
