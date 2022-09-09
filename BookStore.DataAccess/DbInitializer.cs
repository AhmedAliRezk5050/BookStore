using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStore.DataAccess;

public class DbInitializer : IDbInitializer
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly DataContext _context;

    public DbInitializer(UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        DataContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task Initialize()
    {
        try
        {
            await _context.Database.MigrateAsync();
            var adminRoleExist = await _roleManager.RoleExistsAsync(SD.AdminRole);
            if (!adminRoleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.AdminRole));
                await _roleManager.CreateAsync(new IdentityRole(SD.EmployeeRole));
                await _roleManager.CreateAsync(new IdentityRole(SD.CompanyUserRole));
                await _roleManager.CreateAsync(new IdentityRole(SD.IndividualUserRole));

                await _userManager.CreateAsync(new ApplicationUser()
                {
                    Email = "practisedev5050@gmail.com",
                    UserName = "practisedev5050@gmail.com",
                    City = "Ismail",
                    PhoneNumber = "4579631",
                    State = "California",
                    PostalCode = "78931",
                    StreetAddress = "Bank Buildings"
                }, "Ahmed123456789*");

                var user = 
                    await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == "practisedev5050@gmail.com");
                
                if (user is null) throw new Exception("User not found");
                
               await  _userManager.AddToRoleAsync(user, SD.AdminRole);
               
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}