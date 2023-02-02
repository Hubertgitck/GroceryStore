namespace Application.DataAccess.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _dbContext;
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(Product obj)
    {
        var objFromDb = _dbContext.Products.FirstOrDefault(p => p.Id == obj.Id);
        if (objFromDb != null)
        {
            objFromDb.Name = obj.Name;
            objFromDb.Price = obj.Price;
            objFromDb.Description = obj.Description;
            objFromDb.CategoryId = obj.CategoryId;
            objFromDb.PackagingTypeId = obj.PackagingTypeId;

            if (obj.ImageUrl != null)
            {
                objFromDb.ImageUrl = obj.ImageUrl;
            }
        }

    }
}
