using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class AddressDetailMapping : IEntityTypeConfiguration<AddressDetail>
  {
    public void Configure(EntityTypeBuilder<AddressDetail> builder)
    {
      builder.ToTable("AddressDetails");

      builder.HasKey("AddressDetailId");
    }
  }
}
