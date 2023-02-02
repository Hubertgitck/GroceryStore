using Application.DataAccess.Data;
using Application.DataAccess.Repositories.IRepository;
using Application.Models;

namespace Application.DataAccess.Repositories;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    private readonly ApplicationDbContext _dbContext;
    public OrderDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(OrderDetail obj)
    {
        _dbContext.OrderDetails.Update(obj);
    }
}
