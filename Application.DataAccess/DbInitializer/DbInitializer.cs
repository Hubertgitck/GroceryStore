using Application.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.DataAccess.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;
    private const string initialAdminEmail = "admin@grocery.com";
    private const string initialAdminPassword = "Admin123*";

    public DbInitializer(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext applicationDbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = applicationDbContext;
    }
    public async Task Initialize()
    {
        try
        {
            if (_dbContext.Database.GetPendingMigrations().Count() > 0)
            {
                _dbContext.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            //TODO
        }

        if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
        {
            await CreateRoles();
            await CreateAdminUser();

            ApplicationUser user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.Email == initialAdminEmail);

            await _userManager.AddToRoleAsync(user, SD.Role_Admin);
        }
        return;
    }

    private async Task CreateRoles()
    {
        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Individual));
    }

    private async Task CreateAdminUser()
    {
        await _userManager.CreateAsync(new ApplicationUser
        {
            UserName = initialAdminEmail,
            Email = initialAdminEmail,
            Name = "Admin admin",
            PhoneNumber = "1234567890",
            StreetAddress = "Street Address",
            State = "IL",
            PostalCode = "1234567890",
            City = "City"
        }, initialAdminPassword);
    }
}
