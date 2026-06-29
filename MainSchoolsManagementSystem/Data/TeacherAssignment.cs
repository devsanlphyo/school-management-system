namespace MainSchoolsManagementSystem.Data
{
    public class TeacherAssignment
    {
        public int Id { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public ApplicationUser? Teacher { get; set; }
        public int ClassId { get; set; }
        public SchoolClass? Class { get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
    }
}
