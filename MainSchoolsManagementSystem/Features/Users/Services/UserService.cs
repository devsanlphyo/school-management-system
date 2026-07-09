using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainSchoolsManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MainSchoolsManagementSystem.Features.Users.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public UserService(UserManager<ApplicationUser> userManager, IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _userManager = userManager;
            _dbFactory = dbFactory;
        }

        public async Task<List<UserDisplayDto>> GetUsersByRolesAsync(List<string> rolesToFetch)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var result = new List<UserDisplayDto>();

            var roles = await context.Roles
                .Where(r => r.Name != null && rolesToFetch.Contains(r.Name))
                .ToListAsync();
            var roleIds = roles.Select(r => r.Id).ToList();

            var userRoles = await context.UserRoles
                .Where(ur => roleIds.Contains(ur.RoleId))
                .ToListAsync();
            var userIds = userRoles.Select(ur => ur.UserId).ToList();

            var users = await context.Users
                .Include(u => u.School)
                .Include(u => u.Department)
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            foreach (var user in users)
            {
                var ur = userRoles.FirstOrDefault(x => x.UserId == user.Id);
                var role = roles.FirstOrDefault(r => r.Id == ur?.RoleId)?.Name ?? "Staff";

                result.Add(new UserDisplayDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "Unknown",
                    Role = role,
                    SchoolId = user.SchoolId,
                    SchoolName = user.School?.Name,
                    DepartmentId = user.DepartmentId,
                    DepartmentName = user.Department?.Name
                });
            }

            return result;
        }

        public async Task<UserDisplayDto?> GetUserByIdAsync(string id)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var user = await context.Users
                .Include(u => u.School)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Staff";

            return new UserDisplayDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "Unknown",
                Role = role,
                SchoolId = user.SchoolId,
                SchoolName = user.School?.Name,
                DepartmentId = user.DepartmentId,
                DepartmentName = user.Department?.Name
            };
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            var existing = await _userManager.FindByEmailAsync(email);
            return existing != null;
        }

        public async Task<(bool Success, string ErrorMessage)> RegisterUserAsync(string fullName, string email, string password, string role, int? schoolId = null, int? departmentId = null)
        {
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                return (false, "A user with this email already exists.");
            }

            var newUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName.Trim(),
                SchoolId = (role == "Admin" || role == "Director") ? null : schoolId,
                DepartmentId = (role == "Admin" || role == "Director") ? null : departmentId,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, role);
                return (true, string.Empty);
            }

            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateUserAsync(string userId, string fullName, string email, string role, int? schoolId = null, int? departmentId = null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return (false, "User not found.");

            // Check email uniqueness
            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                var existingEmail = await _userManager.FindByEmailAsync(email);
                if (existingEmail != null)
                {
                    return (false, "Email is already taken.");
                }
                user.Email = email;
                user.UserName = email;
            }

            user.FullName = fullName.Trim();
            user.SchoolId = (role == "Admin" || role == "Director") ? null : schoolId;
            user.DepartmentId = (role == "Admin" || role == "Director") ? null : departmentId;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return (false, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, role);
            }

            return (true, string.Empty);
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        public async Task<(bool Success, string ErrorMessage)> ResetUserPasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return (false, "User not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded) return (true, string.Empty);

            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<List<TeacherAssignment>> GetTeacherAssignmentsAsync(string teacherId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.TeacherAssignments
                .Include(ta => ta.Class)
                .Include(ta => ta.Subject)
                .ThenInclude(s => s!.Department)
                .Where(ta => ta.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<bool> AddTeacherAssignmentAsync(TeacherAssignment assignment)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            
            // Validation: prevent duplicate assignments
            var exists = await context.TeacherAssignments.AnyAsync(ta => 
                ta.TeacherId == assignment.TeacherId && 
                ta.ClassId == assignment.ClassId && 
                ta.SubjectId == assignment.SubjectId);
                
            if (exists) return false;

            context.TeacherAssignments.Add(assignment);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteTeacherAssignmentAsync(int assignmentId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var assignment = await context.TeacherAssignments.FindAsync(assignmentId);
            if (assignment != null)
            {
                context.TeacherAssignments.Remove(assignment);
                await context.SaveChangesAsync();
            }
        }
    }
}
