using ApplicationWeb.PaymentServices.Interfaces;

namespace ApplicationWeb.Payments.Models;

public class StripeModel : IPaymentModel
{
    public int OrderId { get; set; }
    public string? PaymentIntentId { get; set; }
    public IEnumerable<ShoppingCart>? CartList { get; set; }
    

}   
