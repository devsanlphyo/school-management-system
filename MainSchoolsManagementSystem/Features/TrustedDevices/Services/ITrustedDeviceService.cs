using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MainSchoolsManagementSystem.Features.TrustedDevices.Services
{
    public interface ITrustedDeviceService
    {
        Task<UserTrustedDevice?> ValidateDeviceAsync(string userId, string rawToken);
        Task<string> TrustDeviceAsync(string userId, string deviceName, string ipAddress, DeviceStatus initialStatus = DeviceStatus.Pending);
        Task<List<UserTrustedDevice>> GetUserDevicesAsync(string userId);
        Task<List<UserTrustedDevice>> GetAllDevicesAsync();
        Task RevokeDeviceAsync(string userId, Guid deviceId);
        Task UpdateDeviceStatusAsync(Guid deviceId, DeviceStatus newStatus);
        Task CleanExpiredDevicesAsync();
    }
}
