using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using MainSchoolsManagementSystem.Data;

namespace MainSchoolsManagementSystem.Tests
{
    public static class TestDbHelper
    {
        public const string ConnectionString = "Server=localhost;Database=SchoolsManagementSystem_Test;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        public static ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            return new ApplicationDbContext(options);
        }

        public static void InitializeDatabase()
        {
            using var context = CreateDbContext();
            Console.WriteLine("Recreating test database (SchoolsManagementSystem_Test)...");
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            Console.WriteLine("Test database recreated successfully.");
        }

        public static void ClearTables(ApplicationDbContext context)
        {
            // Delete in order to satisfy foreign key constraints
            context.Database.ExecuteSqlRaw("DELETE FROM Attendances");
            context.Database.ExecuteSqlRaw("DELETE FROM AspNetUserRoles");
            context.Database.ExecuteSqlRaw("DELETE FROM AspNetUsers");
            context.Database.ExecuteSqlRaw("DELETE FROM SchoolClasses");
            context.Database.ExecuteSqlRaw("DELETE FROM Departments");
            context.Database.ExecuteSqlRaw("DELETE FROM Schools");
            context.Database.ExecuteSqlRaw("DELETE FROM SystemSettings");
        }
    }
}
