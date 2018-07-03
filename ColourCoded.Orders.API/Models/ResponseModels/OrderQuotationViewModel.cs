using ColourCoded.Orders.API.Models.RequestModels.Orders;

namespace ColourCoded.Orders.API.Models.ResponseModels
{
  public class OrderQuotationViewModel
  {
    public OrderCustomerDetailModel CustomerDetail { get; set; }
    public AddressDetailsModel DeliveryAddress { get; set; }
    public CompanyProfileModel CompanyProfile { get; set; }
    public OrderDetailModel OrderTotals { get; set; }
  }
}
