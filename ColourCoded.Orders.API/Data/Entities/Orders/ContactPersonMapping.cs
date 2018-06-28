using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class ContactPersonMapping : IEntityTypeConfiguration<ContactPerson>
  {
    public void Configure(EntityTypeBuilder<ContactPerson> builder)
    {
      builder.ToTable("ContactPersons");

      builder.HasKey("ContactId");
    }
  }
}
