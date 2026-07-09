using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MainSchoolsManagementSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created/migrated
            // SQLite: Use EnsureCreated (migrations contain SQL Server-specific syntax)
            // SQL Server: Use MigrateAsync to apply migrations
            if (context.Database.IsSqlite())
            {
                await context.Database.EnsureCreatedAsync();
            }
            else
            {
                await context.Database.MigrateAsync();
            }

            // 1. Seed Roles
            string[] roles = { "Admin", "Director", "Headmaster", "Officer", "Teacher", "Assistant" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Schools & Academic Structures
            if (!await context.Schools.AnyAsync())
            {
                var stJude = new School { Name = "St. Jude Academy" };
                var oakridge = new School { Name = "Oakridge High" };
                context.Schools.AddRange(stJude, oakridge);
                await context.SaveChangesAsync();
            }

            var stJudeSchool = await context.Schools.FirstOrDefaultAsync(s => s.Name == "St. Jude Academy");
            var oakridgeSchool = await context.Schools.FirstOrDefaultAsync(s => s.Name == "Oakridge High");

            // 3. Seed System Settings if missing
            if (!await context.SystemSettings.AnyAsync())
            {
                // We'll seed a default settings record. 
                context.SystemSettings.Add(new SystemSetting { DailyDeadline = new TimeSpan(8, 30, 0) });
                await context.SaveChangesAsync();
            }

            // 4. Seed Users with Roles & Tenancy
            await SeedUserAsync(userManager, "admin@system.com", "Admin", null, "Global System Admin");

            

        }

        private static async Task<ApplicationUser?> SeedUserAsync(
            UserManager<ApplicationUser> userManager, 
            string email, 
            string role, 
            int? schoolId,
            string fullName)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    SchoolId = schoolId,
                    EmailConfirmed = true,
                    FullName = fullName
                };

                var result = await userManager.CreateAsync(user, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
            return user;
        }
    }
}
