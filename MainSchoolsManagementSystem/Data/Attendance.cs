using System;

namespace MainSchoolsManagementSystem.Data
{
    public enum AttendanceStatus
    {
        Present = 0,
        Late = 1,
        Absent = 2
    }

    public class Attendance
    {
        public int Id { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public ApplicationUser? Teacher { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckedInAt { get; set; }
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Absent;
    }
}
