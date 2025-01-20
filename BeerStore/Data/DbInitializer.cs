using BeerStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeerStore.Data
{
    public static class DbInitializer
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    await context.Database.MigrateAsync(); // Ensure the database is migrated

                    // Seed roles
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await SeedRoles(roleManager);

                    // Seed users
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    await SeedUsers(userManager);

                    // Seed countries and beers
                    await SeedCountriesAndBeers(context);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during database initialization: {ex.Message}");
                }
            }
        }


        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }

        private static async Task SeedUsers(UserManager<ApplicationUser> userManager)
        {
            // Admin user
            var adminEmail = "admin@beerstore.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User"
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Normal user
            var userEmail = "user@beerstore.com";
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var normalUser = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    FirstName = "Regular",
                    LastName = "User"
                };
                var result = await userManager.CreateAsync(normalUser, "User123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(normalUser, "User");
                }
            }
        }

        private static async Task SeedCountriesAndBeers(ApplicationDbContext context)
        {
            if (!await context.Countries.AnyAsync())
            {
                var countries = new List<Country>
        {
            new Country { Name = "Poland" },
            new Country { Name = "Germany" },
            new Country { Name = "USA" }
        };

                await context.Countries.AddRangeAsync(countries);
                await context.SaveChangesAsync();

                var beers = new List<Beer>
        {
            new Beer { Name = "Żywiec", Description = "Classic Polish lager.", Price = 4.99m, Country = countries[0] },
            new Beer { Name = "Tyskie", Description = "Popular Polish beer.", Price = 5.49m, Country = countries[0] },
            new Beer { Name = "Beck's", Description = "Crisp German lager.", Price = 6.99m, Country = countries[1] },
            new Beer { Name = "Paulaner", Description = "Traditional German wheat beer.", Price = 7.99m, Country = countries[1] },
            new Beer { Name = "Budweiser", Description = "Famous American lager.", Price = 5.99m, Country = countries[2] },
            new Beer { Name = "Blue Moon", Description = "American Belgian-style wheat beer.", Price = 6.99m, Country = countries[2] }
        };

                await context.Beers.AddRangeAsync(beers);
                await context.SaveChangesAsync();
            }
        }
    }
}
