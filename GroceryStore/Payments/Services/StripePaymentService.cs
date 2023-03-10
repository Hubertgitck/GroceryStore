using ApplicationWeb.Payments.Models;
using ApplicationWeb.Payments.ServicesSettings;
using ApplicationWeb.PaymentServices.Services;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace ApplicationWeb.Payments.Services;

public class StripePaymentService : PaymentService<StripeModel>
{
    public IOptions<StripeSettings> _stripeSettings;
    public IUnitOfWork _unitOfWork;

    public StripePaymentService(IOptions<StripeSettings> stripeSettings, IUnitOfWork unitOfWork)
    {
        _stripeSettings = stripeSettings;
        _unitOfWork = unitOfWork;
    }

    protected override string MakePayment(StripeModel model)
    {
        var options = PrepareStripeOptions(model.CartList!, model.OrderId);

        var service = new SessionService();
        Session session = service.Create(options);

        _unitOfWork.OrderHeader.UpdatePaymentID(model.OrderId,
            session.Id, session.PaymentIntentId);
        _unitOfWork.Save();

        return session.Url;
    }

    protected override void MakeRefund(StripeModel model)
    {
        var options = new RefundCreateOptions
        {
            Reason = RefundReasons.RequestedByCustomer,
            PaymentIntent = model.PaymentIntentId
        };

        var service = new RefundService();
        service.Create(options);
    }

    protected override string GetPaymentStatus(StripeModel model)
    {
        var service = new SessionService();
        Session session = service.Get(model.SessionId);

        return session.PaymentStatus.ToLower();
    }

    private SessionCreateOptions PrepareStripeOptions(IEnumerable<ShoppingCart> cartListFromDb, int orderId)
    {
        var options = new SessionCreateOptions

        {
            PaymentMethodTypes = new List<string>
                {
                "card",
                },
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = _stripeSettings.Value.Domain + $"customer/cart/OrderConfirmation?id={orderId}",
            CancelUrl = _stripeSettings.Value.Domain + $"customer/cart/index",
        };

        foreach (var item in cartListFromDb)
        {
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        },
                    },
                    Quantity = 1,
                };
                options.LineItems.Add(sessionLineItem);
            }
        }

        return options;
    }
}
