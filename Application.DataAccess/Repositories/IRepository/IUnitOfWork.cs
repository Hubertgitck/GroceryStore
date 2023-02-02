namespace Application.DataAccess.Repositories.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    IPackagingTypeRepository PackagingType { get; }
    IProductRepository Product { get; }
    IShoppingCartRepository ShoppingCart { get; }
    IApplicationUserRepository ApplicationUser { get; }
    IOrderHeaderRepository OrderHeader { get; }
    IOrderDetailRepository OrderDetail { get; }

    void Save();
}
