namespace ColourCoded.Orders.API.Models.RequestModels.Orders
{
  public class AddOrderCustomerRequestModel
  {
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerDetails { get; set; }
    public string CustomerContactNo { get; set; }
    public string CustomerMobileNo { get; set; }
    public string CustomerAccountNo { get; set; }
    public string CustomerEmailAddress { get; set; }
    public bool ContactAdded { get; set; }
    public int ContactId { get; set; }
    public string ContactName { get; set; }
    public string ContactEmailAddress { get; set; }
    public string ContactNo { get; set; }
    public int CompanyProfileId { get; set; }
    public string Username { get; set; }
  }
}
