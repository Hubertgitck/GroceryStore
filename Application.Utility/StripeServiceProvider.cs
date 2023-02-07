using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Application.Utility;

public class StripeServiceProvider
{
    public virtual Session GetStripeSession(string sessionId)
    {
        var service = new SessionService();
        Session session = service.Get(sessionId);
        return session;
    }

    public virtual void GetRefundService(string paymentId)
    {
        var options = new RefundCreateOptions
        {
            Reason = RefundReasons.RequestedByCustomer,
            PaymentIntent = paymentId
        };

        var service = new RefundService();
        service.Create(options);
    }

}
