using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MainSchoolsManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace MainSchoolsManagementSystem.Features.TrustedDevices.Services
{
    public class TrustedDeviceService : ITrustedDeviceService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public TrustedDeviceService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        private static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        public async Task<UserTrustedDevice?> ValidateDeviceAsync(string userId, string rawToken)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(rawToken))
                return null;

            var hash = HashToken(rawToken);

            using var context = await _dbFactory.CreateDbContextAsync();
            var device = await context.UserTrustedDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceHash == hash);

            if (device == null)
                return null;

            if (device.ExpiresAt < DateTimeOffset.UtcNow)
            {
                // Optionally remove expired device
                context.UserTrustedDevices.Remove(device);
                await context.SaveChangesAsync();
                return null;
            }

            // Update Last Used
            device.LastUsedAt = DateTimeOffset.UtcNow;
            await context.SaveChangesAsync();

            return device;
        }

        public async Task<string> TrustDeviceAsync(string userId, string deviceName, string ipAddress, DeviceStatus initialStatus = DeviceStatus.Pending)
        {
            // Generate a secure 32-byte token
            var tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            var rawToken = Convert.ToBase64String(tokenBytes);
            var hash = HashToken(rawToken);

            using var context = await _dbFactory.CreateDbContextAsync();
            
            var device = new UserTrustedDevice
            {
                UserId = userId,
                DeviceHash = hash,
                DeviceName = deviceName,
                IpAddress = ipAddress,
                CreatedAt = DateTimeOffset.UtcNow,
                LastUsedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(30), // Default 30 days as requested
                Status = initialStatus
            };

            context.UserTrustedDevices.Add(device);
            await context.SaveChangesAsync();

            return rawToken;
        }

        public async Task<List<UserTrustedDevice>> GetUserDevicesAsync(string userId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var devices = await context.UserTrustedDevices
                .Where(d => d.UserId == userId)
                .ToListAsync();
                
            return devices.OrderByDescending(d => d.LastUsedAt).ToList();
        }

        public async Task RevokeDeviceAsync(string userId, Guid deviceId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var device = await context.UserTrustedDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.Id == deviceId);

            if (device != null)
            {
                context.UserTrustedDevices.Remove(device);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<UserTrustedDevice>> GetAllDevicesAsync()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var devices = await context.UserTrustedDevices
                .Include(d => d.User)
                .ToListAsync();
                
            return devices.OrderByDescending(d => d.LastUsedAt).ToList();
        }

        public async Task UpdateDeviceStatusAsync(Guid deviceId, DeviceStatus newStatus)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var device = await context.UserTrustedDevices.FindAsync(deviceId);
            if (device != null)
            {
                device.Status = newStatus;
                await context.SaveChangesAsync();
            }
        }

        public async Task CleanExpiredDevicesAsync()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var expired = await context.UserTrustedDevices
                .Where(d => d.ExpiresAt < DateTimeOffset.UtcNow)
                .ToListAsync();

            if (expired.Any())
            {
                context.UserTrustedDevices.RemoveRange(expired);
                await context.SaveChangesAsync();
            }
        }
    }
}
