using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Data.Entities.Identity;

namespace Smart_Freelance_Infrastructure.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<Role> roleManager)
        {
            var rolesCount = await roleManager.Roles.CountAsync();
            if (rolesCount == 0)
            {
                await roleManager.CreateAsync(new Role() { Name = "Client" });
                await roleManager.CreateAsync(new Role() { Name = "Freelancer" });
                await roleManager.CreateAsync(new Role() { Name = "Admin" });
            }
        }
    }
}
