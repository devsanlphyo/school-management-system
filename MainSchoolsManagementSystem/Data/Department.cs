using System.Collections.Generic;

namespace MainSchoolsManagementSystem.Data
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SchoolId { get; set; }
        public School? School { get; set; }
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
