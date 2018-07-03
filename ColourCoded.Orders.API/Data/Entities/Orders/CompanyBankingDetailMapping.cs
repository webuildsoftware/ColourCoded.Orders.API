using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class CompanyBankingDetailMapping : IEntityTypeConfiguration<CompanyBankingDetail>
  {
    public void Configure(EntityTypeBuilder<CompanyBankingDetail> builder)
    {
      builder.ToTable("CompanyBankingDetails");

      builder.HasKey("BankingDetailId");
    }
  }
}
