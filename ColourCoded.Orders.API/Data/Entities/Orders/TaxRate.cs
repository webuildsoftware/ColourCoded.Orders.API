using System;

namespace ColourCoded.Orders.API.Data.Entities.Orders
{
  public class TaxRate
  {
    public int TaxRateId { get; set; }
    public string TaxCode { get; set; }
    public decimal Rate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string UpdateUser { get; set; }
  }
}
