using Microsoft.AspNetCore.Identity;

namespace MainSchoolsManagementSystem.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int? SchoolId { get; set; }
        public School? School { get; set; }
    }

}
