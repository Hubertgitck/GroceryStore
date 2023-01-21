using Application.DataAccess.Data;
using Application.DataAccess.Repositories.IRepository;
using Application.Models;


namespace Application.DataAccess.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Category obj)
        {
            _dbContext.Categories.Update(obj);
        }
    }
}
