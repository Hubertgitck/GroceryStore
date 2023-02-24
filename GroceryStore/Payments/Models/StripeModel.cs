using ApplicationWeb.PaymentServices.Interfaces;
using Stripe.Checkout;

namespace ApplicationWeb.Payments.Models;

public class StripeModel : IPaymentModel
{
    public int OrderId { get; set; }
    public IEnumerable<ShoppingCart> CartList { get; set; }
    public SessionCreateOptions? SessionCreateOptions { get; set; }

}   
