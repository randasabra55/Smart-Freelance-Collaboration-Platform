using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;

namespace Smart_Freelance_Infrastructure.Seeders
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var usersCount = await userManager.Users.CountAsync();
            if (usersCount <= 0)
            {
                var admin = new ApplicationUser
                {
                    EmailConfirmed = true,
                    PhoneNumber = "01102673638",
                    FullName = "Admin",
                    Email = "mostafasabraattia20@gmail.com",
                    UserName = "mostafasabraattia20@gmail.com",
                    Role = UserType.Admin
                };

                var result = await userManager.CreateAsync(admin, "P@$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, UserType.Admin.ToString());
                }
            }
        }
    }
}
