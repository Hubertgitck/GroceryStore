using Application.Models;

namespace Application.DataAccess.Repositories.IRepository;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    void Update(OrderDetail obj);
}