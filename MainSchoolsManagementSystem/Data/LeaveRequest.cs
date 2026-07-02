using System;

namespace MainSchoolsManagementSystem.Data
{
    public enum LeaveStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class LeaveRequest
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SubmittedAt { get; set; }
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public string Reason { get; set; } = string.Empty;
    }
}
