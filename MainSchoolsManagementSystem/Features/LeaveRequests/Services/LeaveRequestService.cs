using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainSchoolsManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace MainSchoolsManagementSystem.Features.LeaveRequests.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public LeaveRequestService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<LeaveRequest>> GetUserLeaveRequestsAsync(string userId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.LeaveRequests
                .AsNoTracking()
                .Where(lr => lr.UserId == userId)
                .OrderByDescending(lr => lr.SubmittedAt)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsBySchoolAsync(int schoolId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.LeaveRequests
                .Include(lr => lr.User)
                .Where(lr => lr.User != null && lr.User.SchoolId == schoolId)
                .OrderByDescending(lr => lr.SubmittedAt)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetAllLeaveRequestsAsync()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.LeaveRequests
                .Include(lr => lr.User)
                    .ThenInclude(u => u!.School)
                .OrderByDescending(lr => lr.SubmittedAt)
                .ToListAsync();
        }

        public async Task<LeaveRequest> SubmitLeaveRequestAsync(string userId, DateTime startDate, DateTime endDate, string reason)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            
            var request = new LeaveRequest
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate,
                SubmittedAt = DateTime.UtcNow,
                Status = LeaveStatus.Pending,
                Reason = reason
            };

            context.LeaveRequests.Add(request);
            await context.SaveChangesAsync();

            return request;
        }

        public async Task UpdateLeaveRequestStatusAsync(int requestId, LeaveStatus status)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var request = await context.LeaveRequests.FindAsync(requestId);
            if (request != null)
            {
                request.Status = status;
                context.LeaveRequests.Update(request);
                await context.SaveChangesAsync();
            }
        }
    }
}
