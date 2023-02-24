using ApplicationWeb.PaymentServices.Interfaces;

namespace ApplicationWeb.PaymentServices.Services;
public abstract class PaymentService<TModel> : IPaymentService
    where TModel : IPaymentModel
{
    public virtual bool AppliesTo(Type provider)
    {
        return typeof(TModel).Equals(provider);
    }

    public void MakePayment<T>(T model) where T : IPaymentModel
    {
        MakePayment((TModel)(object)model);
    }    
    public void MakeRefund<T>(T model) where T : IPaymentModel
    {
        MakeRefund((TModel)(object)model);
    }

    protected abstract void MakePayment(TModel model);
    protected abstract void MakeRefund(TModel model);
}
