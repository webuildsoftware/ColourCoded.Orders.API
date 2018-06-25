using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class OrderDetailMapping : IEntityTypeConfiguration<OrderDetail>
  {
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
      builder.ToTable("OrderDetails");

      builder.HasKey("OrderDetailId");
    }
  }
}
