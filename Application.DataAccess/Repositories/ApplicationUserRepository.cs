using Application.DataAccess.Data;
using Application.DataAccess.Repositories.IRepository;
using Application.Models;

namespace Application.DataAccess.Repositories;

public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    public ApplicationUserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
