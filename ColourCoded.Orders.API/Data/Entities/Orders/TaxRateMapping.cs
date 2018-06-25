using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class TaxRateMapping : IEntityTypeConfiguration<TaxRate>
  {
    public void Configure(EntityTypeBuilder<TaxRate> builder)
    {
      builder.ToTable("TaxRates");

      builder.HasKey("TaxRateId");
    }
  }
}
