using Microsoft.AspNetCore.Identity;

using UCLoan.Models;

namespace UCLoan.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();


            // Criando cargos
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "vitodalvi@gmail.com";
            var adminPassword = "vitordalvi";

            var userExist = await userManager.FindByEmailAsync(adminEmail);

            if (userExist == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "vitodalvi",
                    FullName = "Vitor Dalvi",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new Exception("Failed to create the admin user: " + string.Join(", ", result.Errors));
                }
            }

        }
    }
}
