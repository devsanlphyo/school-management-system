using System.Collections.Generic;

namespace MainSchoolsManagementSystem.Features.Schools.Models
{
    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
