namespace Application.DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public readonly ApplicationDbContext _dbContext;
    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        Category = new CategoryRepository(_dbContext);
        PackagingType = new PackagingTypeRepository(_dbContext);
        Product = new ProductRepository(_dbContext);
        ShoppingCart = new ShoppingCartRepository(_dbContext);
        ApplicationUser = new ApplicationUserRepository(_dbContext);
        OrderHeader = new OrderHeaderRepository(_dbContext);
        OrderDetail = new OrderDetailRepository(_dbContext);
    }
    public ICategoryRepository Category { get; private set; }
    public IPackagingTypeRepository PackagingType { get; private set; }
    public IProductRepository Product { get; private set; }
    public IShoppingCartRepository ShoppingCart { get; private set; }
    public IApplicationUserRepository ApplicationUser { get; private set; }
    public IOrderHeaderRepository OrderHeader { get; private set; }
    public IOrderDetailRepository OrderDetail { get; private set; }

    public void Save()
    {
        _dbContext.SaveChanges();
    }
  
}

