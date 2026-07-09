using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MainSchoolsManagementSystem.Features.Attendance.Services
{
    using Attendance = MainSchoolsManagementSystem.Features.Attendance.Models.Attendance;
    public interface IAttendanceService
    {
        Task<List<Attendance>> GetAttendancesBySchoolAndDateAsync(int schoolId, DateTime date);
        Task<List<Attendance>> GetAttendancesByDateAsync(DateTime date);
        
        Task<Attendance?> GetAttendanceAsync(string userId, DateTime date);
        Task<List<Attendance>> GetAttendanceHistoryAsync(string userId, DateTime startDate, DateTime endDate);
        
        Task<Attendance> CheckInAsync(string userId, DateTime timestamp, TimeSpan dailyDeadline);
    }
}
