using Application.Models;

namespace Application.DataAccess.Repositories.IRepository;

public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category obj);
}
