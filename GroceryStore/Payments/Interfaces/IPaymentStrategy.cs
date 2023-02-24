namespace ApplicationWeb.PaymentServices.Interfaces;

public interface IPaymentStrategy
{
    string MakePayment<T>(T model) where T : IPaymentModel;
    void MakeRefund<T>(T model) where T : IPaymentModel;
}
