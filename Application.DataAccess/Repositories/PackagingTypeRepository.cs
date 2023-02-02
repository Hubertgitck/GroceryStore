namespace Application.DataAccess.Repositories;

public class PackagingTypeRepository : Repository<PackagingType>, IPackagingTypeRepository
{
    private readonly ApplicationDbContext _dbContext;
    public PackagingTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(PackagingType obj)
    {
        _dbContext.PackagingTypes.Update(obj);
    }
}
