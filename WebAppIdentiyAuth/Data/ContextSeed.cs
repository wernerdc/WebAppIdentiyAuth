using Microsoft.AspNetCore.Identity;
using WebAppIdentiyAuth.Enums;
using WebAppIdentiyAuth.Models;

namespace WebAppIdentiyAuth.Data
{
    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Manager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Member.ToString()));
        }

        public static async Task SeedDefaultAdminAsync(UserManager<ApplicationUser> userManager)
        {
            //Seed Default Admin
            var defaultUser = new ApplicationUser
            {
                UserName = "admin@admin.de",
                Email = "admin@admin.de",
                FirstName = "admin",
                LastName = "admin",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Test1234,");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Manager.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Member.ToString());
                }
            }
        }
    }
}
