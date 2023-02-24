using ApplicationWeb.Payments.Models;
using ApplicationWeb.PaymentServices.Services;

namespace ApplicationWeb.Payments.Services;

public class StripeService : PaymentService<StripeModel>
{
    protected override void MakePayment(StripeModel model)
    {
        throw new NotImplementedException();
    }

    protected override void MakeRefund(StripeModel model)
    {
        throw new NotImplementedException();
    }
}
