
namespace ColourCoded.Orders.API.Models.ResponseModels
{
  public class CompanyProfileModel
  {
    public string DisplayName { get; set; }
    public string RegistrationNo { get; set; }
    public string TaxReferenceNo { get; set; }
    public string EmailAddress { get; set; }
    public string TelephoneNo { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public BankingDetailsModel BankingDetails { get; set; }

    public CompanyProfileModel()
    {
      BankingDetails = new BankingDetailsModel();
    }
  }
}
