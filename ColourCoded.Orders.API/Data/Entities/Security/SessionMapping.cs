using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ColourCoded.Orders.API.Data.Entities.Security
{
  public class SessionMapping : IEntityTypeConfiguration<Session>
  {
    public void Configure(EntityTypeBuilder<Session> builder)
    {
      builder.ToTable("Sessions");

      builder.HasKey("SessionId");
    }
  }
}
