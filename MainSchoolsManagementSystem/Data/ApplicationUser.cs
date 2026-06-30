using Microsoft.AspNetCore.Identity;

namespace MainSchoolsManagementSystem.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
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
