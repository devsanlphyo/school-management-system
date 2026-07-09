using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainSchoolsManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace MainSchoolsManagementSystem.Features.Attendance.Services
{
    using Attendance = MainSchoolsManagementSystem.Features.Attendance.Models.Attendance;
    public class AttendanceService : IAttendanceService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public AttendanceService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<Attendance>> GetAttendancesBySchoolAndDateAsync(int schoolId, DateTime date)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Attendances
                .Include(a => a.User)
                .Where(a => a.User != null && a.User.SchoolId == schoolId && a.Date.Date == date.Date)
                .ToListAsync();
        }

        public async Task<List<Attendance>> GetAttendancesByDateAsync(DateTime date)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Attendances
                .Include(a => a.User)
                .Where(a => a.Date.Date == date.Date)
                .ToListAsync();
        }

        public async Task<Attendance?> GetAttendanceAsync(string userId, DateTime date)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Attendances
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Date.Date == date.Date);
        }

        public async Task<List<Attendance>> GetAttendanceHistoryAsync(string userId, DateTime startDate, DateTime endDate)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Attendances
                .AsNoTracking()
                .Where(a => a.UserId == userId && a.Date.Date >= startDate.Date && a.Date.Date <= endDate.Date)
                .ToListAsync();
        }

        public async Task<Attendance> CheckInAsync(string userId, DateTime timestamp, TimeSpan dailyDeadline)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            
            var localTime = timestamp.ToLocalTime();
            var isLate = localTime.TimeOfDay > dailyDeadline;

            var attendance = new Attendance
            {
                UserId = userId,
                Date = localTime.Date,
                CheckedInAt = timestamp,
                Status = isLate ? AttendanceStatus.Late : AttendanceStatus.Present
            };

            context.Attendances.Add(attendance);
            await context.SaveChangesAsync();

            return attendance;
        }
    }
}
