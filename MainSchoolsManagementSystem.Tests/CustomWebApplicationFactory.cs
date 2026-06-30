using System;
using System.IO;
using System.Linq;
using MainSchoolsManagementSystem.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MainSchoolsManagementSystem.Tests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly SqliteConnection _connection;
        public string TempUploadsPath { get; }

        public CustomWebApplicationFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            TempUploadsPath = Path.Combine(Path.GetTempPath(), "SMS_Uploads_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(TempUploadsPath);
            // Ensure the "uploads" subfolder is created inside the temp content root
            Directory.CreateDirectory(Path.Combine(TempUploadsPath, "uploads"));
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(TempUploadsPath);

            builder.ConfigureServices(services =>
            {
                // Remove existing DbContextOptions
                var dbContextOptionsDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (dbContextOptionsDescriptor != null)
                {
                    services.Remove(dbContextOptionsDescriptor);
                }

                // Remove existing IDbContextFactory
                var dbContextFactoryDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IDbContextFactory<ApplicationDbContext>));
                if (dbContextFactoryDescriptor != null)
                {
                    services.Remove(dbContextFactoryDescriptor);
                }

                // Add DbContextFactory using SQLite
                services.AddDbContextFactory<ApplicationDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                // Remove existing ApplicationDbContext if any
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ApplicationDbContext));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                // Add ApplicationDbContext as transient resolving from factory
                services.AddTransient<ApplicationDbContext>(sp =>
                    sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

                // Replace Authentication with Test Authentication
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                    options.DefaultScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            // Ensure database is created
            using (var scope = host.Services.CreateScope())
            {
                var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
                using var db = dbContextFactory.CreateDbContext();
                db.Database.EnsureCreated();
            }

            return host;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _connection.Dispose();
                try
                {
                    if (Directory.Exists(TempUploadsPath))
                    {
                        Directory.Delete(TempUploadsPath, true);
                    }
                }
                catch
                {
                    // Ignore directory deletion errors on cleanup
                }
            }
        }
    }
}
