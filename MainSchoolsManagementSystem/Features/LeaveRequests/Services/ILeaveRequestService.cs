using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MainSchoolsManagementSystem.Features.LeaveRequests.Services
{
    public interface ILeaveRequestService
    {
        Task<List<LeaveRequest>> GetUserLeaveRequestsAsync(string userId);
        Task<List<LeaveRequest>> GetLeaveRequestsBySchoolAsync(int schoolId);
        Task<List<LeaveRequest>> GetAllLeaveRequestsAsync();
        
        Task<LeaveRequest> SubmitLeaveRequestAsync(string userId, DateTime startDate, DateTime endDate, string reason);
        Task UpdateLeaveRequestStatusAsync(int requestId, LeaveStatus status);
    }
}
