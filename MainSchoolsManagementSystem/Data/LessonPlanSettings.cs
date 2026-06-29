using System;

namespace MainSchoolsManagementSystem.Data
{
    public class LessonPlanSettings
    {
        public int Id { get; set; }
        public TimeSpan DailyDeadline { get; set; } = new TimeSpan(8, 30, 0);
    }
}
