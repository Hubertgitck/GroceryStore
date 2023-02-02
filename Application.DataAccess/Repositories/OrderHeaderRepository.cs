using Application.DataAccess.Data;
using Application.DataAccess.Repositories.IRepository;
using Application.Models;

namespace Application.DataAccess.Repositories;

public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private readonly ApplicationDbContext _dbContext;
    public OrderHeaderRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(OrderHeader obj)
    {
        _dbContext.OrderHeaders.Update(obj);
    }

    public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
    {
        var orderFromDb = _dbContext.OrderHeaders.FirstOrDefault(u => u.Id == id);
        if (orderFromDb != null)
        {
            orderFromDb.OrderStatus = orderStatus;
            if (paymentStatus != null)
            {
                orderFromDb.PaymentStatus = paymentStatus;
            }
        }
    }
    public void UpdateStripePaymentID(int id, string sessionId, string paymentIntendId)
    {
        var orderFromDb = _dbContext.OrderHeaders.FirstOrDefault(u => u.Id == id);

        orderFromDb.SessionId = sessionId;
        orderFromDb.PaymentIntendId = paymentIntendId;
        orderFromDb.PaymentDate = DateTime.Now;
    }
}
