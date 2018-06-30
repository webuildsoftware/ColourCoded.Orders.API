using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class CustomerMapping : IEntityTypeConfiguration<Customer>
  {
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
      builder.ToTable("Customers");

      builder.HasKey("CustomerId");

      builder.HasMany(c => c.ContactPeople);
      builder.HasMany(c => c.Addresses);
    }
  }
}
