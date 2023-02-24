using ApplicationWeb.Payments.Models;
using ApplicationWeb.PaymentServices.Services;
using Microsoft.Extensions.Options;
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
        var options = PrepareStripeOptions(model.CartList, model.OrderId);

        var service = new SessionService();
        Session session = service.Create(options);

        _unitOfWork.OrderHeader.UpdatePaymentID(model.OrderId,
            session.Id, session.PaymentIntentId);
        _unitOfWork.Save();

        return session.Url;
    }

    protected override void MakeRefund(StripeModel model)
    {
        throw new NotImplementedException();
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
