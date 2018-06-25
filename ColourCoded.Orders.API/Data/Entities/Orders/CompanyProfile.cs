using System;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class CompanyProfile
  {
    public int CompanyProfileId { get; set; }
    public string DisplayName { get; set; }
    public string LegalName { get; set; }
    public string RegistrationNo { get; set; }
    public string TaxReferenceNo { get; set; }
    public string EmailAddress { get; set; }
    public string TelephoneNo { get; set; }
    public string FaxNo { get; set; }
    public int OrderNoSeed { get; set; }
    public string CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public string UpdateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
  }
}
