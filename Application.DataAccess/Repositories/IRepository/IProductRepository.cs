namespace Application.DataAccess.Repositories.IRepository;

public interface IProductRepository : IRepository<Product>
{
    void Update(Product obj);
}