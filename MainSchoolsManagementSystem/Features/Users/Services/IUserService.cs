using System.Collections.Generic;
using System.Threading.Tasks;

namespace MainSchoolsManagementSystem.Features.Users.Services
{
    public interface IUserService
    {
        Task<List<UserDisplayDto>> GetUsersByRolesAsync(List<string> roles);
        Task<UserDisplayDto?> GetUserByIdAsync(string id);
        
        // Returns true if successful, false if email exists
        Task<bool> CheckEmailExistsAsync(string email);
        Task<(bool Success, string ErrorMessage)> RegisterUserAsync(string fullName, string email, string password, string role, int? schoolId = null, int? departmentId = null);
        
        Task<(bool Success, string ErrorMessage)> UpdateUserAsync(string userId, string fullName, string email, string role, int? schoolId = null, int? departmentId = null);
        
        Task DeleteUserAsync(string userId);
        
        Task<(bool Success, string ErrorMessage)> ResetUserPasswordAsync(string userId, string newPassword);

        // Teacher Assignments
        Task<List<TeacherAssignment>> GetTeacherAssignmentsAsync(string teacherId);
        Task<bool> AddTeacherAssignmentAsync(TeacherAssignment assignment);
        Task DeleteTeacherAssignmentAsync(int assignmentId);
    }
}
