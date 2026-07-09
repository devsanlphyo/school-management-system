using System;

namespace MainSchoolsManagementSystem.Features.LessonPlans.Models
{
    public enum LessonPlanStatus
    {
        Pending = 0,
        Reviewed = 1
    }

    public class LessonPlan
    {
        public int Id { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public ApplicationUser? Teacher { get; set; }
        
        public int? ClassId { get; set; }
        public SchoolClass? Class { get; set; }
        
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public DateTime UploadedAt { get; set; }
        public bool IsLate { get; set; }
        public string? JustificationText { get; set; }
        public bool HasJustificationAttachment { get; set; }
        public LessonPlanStatus Status { get; set; } = LessonPlanStatus.Pending;
        public string? Feedback { get; set; }
    }
}
