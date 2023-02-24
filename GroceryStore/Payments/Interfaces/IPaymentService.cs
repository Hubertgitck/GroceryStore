namespace ApplicationWeb.PaymentServices.Interfaces;

public interface IPaymentService
{
    void MakePayment<T>(T model) where T : IPaymentModel;
    void MakeRefund<T>(T model) where T : IPaymentModel;
    bool AppliesTo(Type provider);
}
