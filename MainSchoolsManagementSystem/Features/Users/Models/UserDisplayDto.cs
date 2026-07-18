namespace MainSchoolsManagementSystem.Features.Users.Models
{
    public class UserDisplayDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? SchoolId { get; set; }
        public string? SchoolName { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? ProfilePicturePath { get; set; }
    }
}
