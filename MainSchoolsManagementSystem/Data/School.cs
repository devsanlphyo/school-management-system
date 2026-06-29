using System.Collections.Generic;

namespace MainSchoolsManagementSystem.Data
{
    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
