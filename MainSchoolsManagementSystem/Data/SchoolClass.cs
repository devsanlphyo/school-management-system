namespace MainSchoolsManagementSystem.Data
{
    public class SchoolClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GradeLevel { get; set; } = string.Empty; // e.g. "KG", "Grade1", ..., "Grade12"
        public int SchoolId { get; set; }
        public School? School { get; set; }
    }
}
