namespace Application.DataAccess.Repositories.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    void Update(Product obj);
}