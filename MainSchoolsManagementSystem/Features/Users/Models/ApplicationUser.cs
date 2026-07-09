using Microsoft.AspNetCore.Identity;

namespace MainSchoolsManagementSystem.Features.Users.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = "";
        public int? SchoolId { get; set; }
        public School? School { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public string? ProfilePicturePath { get; set; }
    }
}
