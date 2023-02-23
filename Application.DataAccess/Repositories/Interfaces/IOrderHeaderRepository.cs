namespace Application.DataAccess.Repositories.Interfaces;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
    void Update(OrderHeader obj);
    void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
    void UpdatePaymentID(int id, string sessionId, string paymentIntentId);

}