using ApplicationWeb.PaymentServices.Interfaces;

namespace ApplicationWeb.Payments.Models;

public class StripeModel : IPaymentModel
{
    public int Id { get; set; }
}   
