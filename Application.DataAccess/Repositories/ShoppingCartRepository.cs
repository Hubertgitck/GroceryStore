namespace Application.DataAccess.Repositories;

public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
{
    private readonly ApplicationDbContext _dbContext;
    public ShoppingCartRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public int DecrementCount(ShoppingCart shoppingCart, int count)
    {
        shoppingCart.Count -= count;
        return shoppingCart.Count;
    }

    public int IncrementCount(ShoppingCart shoppingCart, int count)
    {
        shoppingCart.Count += count;
        return shoppingCart.Count;
    }

    public void Update(ShoppingCart shoppingCart)
    {
        _dbContext.Update(shoppingCart);
    }
}
