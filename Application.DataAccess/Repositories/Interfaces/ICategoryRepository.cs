namespace Application.DataAccess.Repositories.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category obj);
}
