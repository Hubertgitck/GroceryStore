using Application.Models;

namespace Application.DataAccess.Repositories.IRepository;

public interface IPackagingTypeRepository : IRepository<PackagingType>
{ 
    void Update(PackagingType obj);
}