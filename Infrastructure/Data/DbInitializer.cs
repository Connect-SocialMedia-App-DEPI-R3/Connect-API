using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Infrastructure.Data;
namespace Infrastructure.Data;
public static class DbInitializer
{
    public static async Task SeedAsync(
        UserManager<User> _userManager,
        RoleManager<IdentityRole<Guid>> _roleManager,
        AppDbContext context)
    {
        // seed roles
        var roles = new[] {"Admin"};
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
        }
        // seed admin user
        string adminEmail = "yasmine@gmail.com", adminPassword = "Yasmine123#", adminUserName = "Yasmine";
        if (await _userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new User
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}