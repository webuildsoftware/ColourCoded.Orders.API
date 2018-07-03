using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class CompanyAddressDetailMapping : IEntityTypeConfiguration<CompanyAddressDetail>
  {
    public void Configure(EntityTypeBuilder<CompanyAddressDetail> builder)
    {
      builder.ToTable("CompanyAddressDetails");

      builder.HasKey("AddressDetailId");
    }
  }
}
