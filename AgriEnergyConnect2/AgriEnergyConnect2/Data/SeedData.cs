using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection; // Added for GetRequiredService
using System;
using System.Threading.Tasks;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roleNames = { "Farmer", "Employee", "Admin" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Optionally, create an admin user for testing purposes
        var adminUser = await userManager.FindByEmailAsync("admin@example.com");
        if (adminUser == null)
        {
            var user = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com" };
            var result = await userManager.CreateAsync(user, "Admin@123"); // Strong password for security
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                // Log errors if user creation fails
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error creating admin user: {error.Description}");
                }
            }
        }
    }
}