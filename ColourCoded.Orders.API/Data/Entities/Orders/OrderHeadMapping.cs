using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class OrderHeadMapping : IEntityTypeConfiguration<OrderHead>
  {
    public void Configure(EntityTypeBuilder<OrderHead> builder)
    {
      builder.ToTable("OrderHeads");

      builder.HasKey("OrderId");

      builder.HasMany(o => o.OrderDetails);
    }
  }
}
