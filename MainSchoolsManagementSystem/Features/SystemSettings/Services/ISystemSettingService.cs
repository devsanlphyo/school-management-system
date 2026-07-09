using System.Threading.Tasks;

namespace MainSchoolsManagementSystem.Features.SystemSettings.Services
{
    public interface ISystemSettingService
    {
        Task<SystemSetting> GetSettingsAsync();
        Task UpdateSettingsAsync(SystemSetting settings);
    }
}
