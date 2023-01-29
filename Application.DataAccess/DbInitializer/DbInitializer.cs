using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DataAccess.Data;
using Application.Models;
using Application.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

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

            }

            if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Individual));
                await _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@grocery.com",
                    Email = "admin@grocery.com",
                    Name = "Admin admin",
                    PhoneNumber = "1234567890",
                    StreetAddress = "Street Address",
                    State = "IL",
                    PostalCode = "1234567890",
                    City = "City"
                }, "Admin123*");

                ApplicationUser user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@grocery.com");

                await _userManager.AddToRoleAsync(user, SD.Role_Admin);
            }
            return;
        }
    }
}
