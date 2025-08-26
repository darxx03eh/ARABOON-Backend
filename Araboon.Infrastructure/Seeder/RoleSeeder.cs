using Araboon.Data.Entities.Identity;
using Araboon.Data.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Seeder
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<AraboonRole> roleManager)
        {
            var roles = await roleManager.Roles.CountAsync();
            if (roles.Equals(0))
            {
                await roleManager.CreateAsync(new AraboonRole()
                {
                    Name = Roles.Admin
                });
                await roleManager.CreateAsync(new AraboonRole()
                {
                    Name = Roles.User
                });
            }
        }
    }
}
