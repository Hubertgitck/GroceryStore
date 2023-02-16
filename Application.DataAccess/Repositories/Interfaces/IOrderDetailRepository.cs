namespace Application.DataAccess.Repositories.Interfaces;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    void Update(OrderDetail obj);
}