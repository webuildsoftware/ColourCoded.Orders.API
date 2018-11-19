using Microsoft.EntityFrameworkCore;
using ColourCoded.Orders.API.Data.Entities.Orders;

namespace ColourCoded.Orders.API.Data
{
  public class OrdersContext : DbContext
  {
    public DbSet<OrderHead> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ContactPerson> Contacts { get; set; }
    public DbSet<TaxRate> TaxRates { get; set; }
    public DbSet<CompanyProfile> CompanyProfiles { get; set; }

    public OrdersContext(DbContextOptions<OrdersContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.ApplyConfiguration(new OrderHeadMapping());
      modelBuilder.ApplyConfiguration(new OrderDetailMapping());
      modelBuilder.ApplyConfiguration(new CustomerMapping());
      modelBuilder.ApplyConfiguration(new ContactPersonMapping());
      modelBuilder.ApplyConfiguration(new TaxRateMapping());
      modelBuilder.ApplyConfiguration(new CompanyProfileMapping());
      modelBuilder.ApplyConfiguration(new ContactPersonMapping());
      modelBuilder.ApplyConfiguration(new AddressDetailMapping());
      modelBuilder.ApplyConfiguration(new CompanyAddressDetailMapping());
      modelBuilder.ApplyConfiguration(new CompanyBankingDetailMapping());
    }
  }
}
