using Stripe.Checkout;

namespace Application.Utility;

public class StripeSessionProvider
{
    public virtual Session GetStripeSession(string sessionId)
    {
        var service = new SessionService();
        Session session = service.Get(sessionId);
        return session;
    }
}
