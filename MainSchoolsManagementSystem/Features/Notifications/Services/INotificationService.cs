using System.Collections.Generic;
using System.Threading.Tasks;
using MainSchoolsManagementSystem.Features.Notifications.Models;

namespace MainSchoolsManagementSystem.Features.Notifications.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetNotificationsForUserAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAllAsReadAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
    }
}
