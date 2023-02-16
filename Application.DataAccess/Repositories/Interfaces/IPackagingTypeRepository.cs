namespace Application.DataAccess.Repositories.Interfaces;

public interface IPackagingTypeRepository : IRepository<PackagingType>
{ 
    void Update(PackagingType obj);
}