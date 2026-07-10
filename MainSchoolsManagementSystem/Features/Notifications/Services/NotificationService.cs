using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainSchoolsManagementSystem.Data;
using MainSchoolsManagementSystem.Features.Notifications.Models;
using Microsoft.EntityFrameworkCore;

namespace MainSchoolsManagementSystem.Features.Notifications.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public NotificationService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<Notification>> GetNotificationsForUserAsync(string userId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Notifications
                .AsNoTracking()
                .Include(n => n.TriggerUser)
                .Include(n => n.FeedPost)
                .Where(n => n.RecipientId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Notifications
                .Where(n => n.RecipientId == userId && !n.IsRead)
                .CountAsync();
        }

        public static event Action<string>? OnNotificationReceived;

        public static void Notify(string recipientId)
        {
            try
            {
                OnNotificationReceived?.Invoke(recipientId);
            }
            catch { }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var unread = await context.Notifications
                .Where(n => n.RecipientId == userId && !n.IsRead)
                .ToListAsync();

            if (unread.Any())
            {
                foreach (var n in unread)
                {
                    n.IsRead = true;
                }
                await context.SaveChangesAsync();
                Notify(userId);
            }
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var n = await context.Notifications.FindAsync(notificationId);
            if (n != null)
            {
                n.IsRead = true;
                await context.SaveChangesAsync();
                Notify(n.RecipientId);
            }
        }
    }
}
