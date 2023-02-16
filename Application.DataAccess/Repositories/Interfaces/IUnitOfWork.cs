namespace Application.DataAccess.Repositories.Interfaces;

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
