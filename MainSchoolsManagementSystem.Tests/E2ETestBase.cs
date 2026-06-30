using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using Xunit;
using MainSchoolsManagementSystem.Data;

namespace MainSchoolsManagementSystem.Tests
{
    public class E2EWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        public string ServerAddress { get; private set; } = string.Empty;
        private IHost? _host;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Bind Kestrel to localhost on a random port
            builder.UseUrls("http://127.0.0.1:0");

            // Override connection string to use the test database
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "ConnectionStrings:DefaultConnection", TestDbHelper.ConnectionString }
                });
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            _host = base.CreateHost(builder);
            _host.Start();

            var server = _host.Services.GetRequiredService<IServer>();
            var addresses = server.Features.Get<IServerAddressesFeature>();
            ServerAddress = addresses!.Addresses.First();

            return _host;
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
        }
    }

    public abstract class E2ETestBase : IAsyncLifetime
    {
        protected static readonly E2EWebApplicationFactory<Program> Factory = new();

        protected IPlaywright Playwright { get; private set; } = null!;
        protected IBrowser Browser { get; private set; } = null!;
        protected IBrowserContext Context { get; private set; } = null!;
        protected IPage Page { get; private set; } = null!;

        protected string BaseUrl => Factory.ServerAddress;

        public virtual async Task InitializeAsync()
        {
            // 1. Ensure the database is clean by deleting it and letting DbSeeder migrate/recreate it
            using (var context = TestDbHelper.CreateDbContext())
            {
                await context.Database.EnsureDeletedAsync();
            }

            // Seed the default data using the DbSeeder via the factory's services
            using (var scope = Factory.Services.CreateScope())
            {
                await DbSeeder.SeedAsync(scope.ServiceProvider);
            }

            // 2. Initialize Playwright
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            // 3. Setup Browser Context (default: unauthenticated)
            Context = await Browser.NewContextAsync();
            Page = await Context.NewPageAsync();
        }

        public virtual async Task DisposeAsync()
        {
            if (Page != null) await Page.CloseAsync();
            if (Context != null) await Context.CloseAsync();
            if (Browser != null) await Browser.DisposeAsync();
            Playwright?.Dispose();
        }

        // Helper to get authenticated context for a specific role
        protected async Task<IBrowserContext> CreateAuthenticatedContextAsync(string role)
        {
            var storageStatePath = await GetOrCreateStorageStateAsync(role);
            return await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                StorageStatePath = storageStatePath
            });
        }

        private async Task<string> GetOrCreateStorageStateAsync(string role)
        {
            var authDir = Path.Combine(Directory.GetCurrentDirectory(), "auth");
            if (!Directory.Exists(authDir))
            {
                Directory.CreateDirectory(authDir);
            }

            var statePath = Path.Combine(authDir, $"{role.ToLower()}.json");

            // Check if we already have it. If we do, just return it.
            if (File.Exists(statePath))
            {
                return statePath;
            }

            // Otherwise, we need to log in and save the state
            string email = role.ToLower() switch
            {
                "admin" => "admin@system.com",
                "headmaster" => "headmaster@stjude.edu",
                "teacher" => "teacher@stjude.edu",
                _ => throw new ArgumentException($"Unknown role: {role}")
            };

            var tempContext = await Browser.NewContextAsync();
            var tempPage = await tempContext.NewPageAsync();

            // Navigate to login page
            await tempPage.GotoAsync($"{BaseUrl}/Account/Login");

            // Fill in credentials
            await tempPage.FillAsync("input[autocomplete='username']", email);
            await tempPage.FillAsync("input[type='password']", "Password123!");

            // Submit form
            await tempPage.ClickAsync("button[type='submit']");

            // Wait for redirect to home page
            await tempPage.WaitForURLAsync(url => url.EndsWith("/home") || url.EndsWith("/"), new PageWaitForURLOptions
            {
                Timeout = 10000
            });

            // Save storage state
            await tempContext.StorageStateAsync(new BrowserContextStorageStateOptions
            {
                Path = statePath
            });

            await tempPage.CloseAsync();
            await tempContext.CloseAsync();

            return statePath;
        }
    }
}
