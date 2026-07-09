using System;

namespace MainSchoolsManagementSystem.Features.SystemSettings.Models
{
    public class SystemSetting
    {
        public int Id { get; set; }
        public TimeSpan DailyDeadline { get; set; } = new TimeSpan(8, 30, 0);
        public bool MaintenanceMode { get; set; } = false;
    }
}
