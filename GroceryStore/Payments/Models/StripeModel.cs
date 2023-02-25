using ApplicationWeb.PaymentServices.Interfaces;

namespace ApplicationWeb.Payments.Models;

public class StripeModel : IPaymentModel
{
    public int OrderId { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? SessionId { get; set; }
    public string? PaymentStatus { get; set; }
    public IEnumerable<ShoppingCart>? CartList { get; set; }
}   
