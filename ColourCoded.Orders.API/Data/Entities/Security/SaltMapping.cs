using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ColourCoded.Orders.API.Data.Entities.Security
{
  public class SaltMapping : IEntityTypeConfiguration<Salt>
  {
    public void Configure(EntityTypeBuilder<Salt> builder)
    {
      builder.ToTable("Salts");

      builder.HasKey("SaltId");
    }
  }
}
