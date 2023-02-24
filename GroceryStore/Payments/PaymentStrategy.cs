using ApplicationWeb.PaymentServices.Interfaces;

namespace ApplicationWeb.Payments;

public class PaymentStrategy : IPaymentStrategy
{
    private readonly IEnumerable<IPaymentService> paymentServices;

    public PaymentStrategy(IEnumerable<IPaymentService> paymentServices)
    {
        this.paymentServices = paymentServices ?? throw new ArgumentNullException(nameof(paymentServices));
    }

    public string MakePayment<T>(T model) where T : IPaymentModel
    {
        return GetPaymentService(model).MakePayment(model);   
    }

    public void MakeRefund<T>(T model) where T : IPaymentModel
    {
        GetPaymentService(model).MakeRefund(model);
    }

    private IPaymentService GetPaymentService<T>(T model) where T : IPaymentModel
    {
        var result = paymentServices.FirstOrDefault(p => p.AppliesTo(model.GetType()));
        if (result == null)
        {
            throw new InvalidOperationException(
                $"Payment service for {model.GetType().ToString()} not registered.");
        }
        return result;
    }
}
