using System;
using System.Threading.Tasks;
using MainSchoolsManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace MainSchoolsManagementSystem.Features.SystemSettings.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public SystemSettingService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<SystemSetting> GetSettingsAsync()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var settings = await context.SystemSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new SystemSetting { DailyDeadline = new TimeSpan(8, 30, 0), MaintenanceMode = false };
                context.SystemSettings.Add(settings);
                await context.SaveChangesAsync();
            }
            return settings;
        }

        public async Task UpdateSettingsAsync(SystemSetting settings)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            context.SystemSettings.Update(settings);
            await context.SaveChangesAsync();
        }
    }
}
