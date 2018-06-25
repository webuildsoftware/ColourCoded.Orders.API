using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class CompanyProfileMapping : IEntityTypeConfiguration<CompanyProfile>
  {
    public void Configure(EntityTypeBuilder<CompanyProfile> builder)
    {
      builder.ToTable("CompanyProfiles");

      builder.HasKey("CompanyProfileId");
    }
  }
}
