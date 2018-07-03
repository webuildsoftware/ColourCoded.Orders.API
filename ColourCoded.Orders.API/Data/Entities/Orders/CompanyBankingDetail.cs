using System;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class CompanyBankingDetail
  {
    public int BankingDetailId { get; set; }
    public string BankName { get; set; }
    public string BranchCode { get; set; }
    public string AccountType { get; set; }
    public string AccountNo { get; set; }
    public string AccountHolder { get; set; }
    public int CompanyProfileId { get; set; }
    public string CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public string UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }
  }
}
