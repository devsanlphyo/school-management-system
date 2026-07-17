using System;
using System.ComponentModel.DataAnnotations;

namespace MainSchoolsManagementSystem.Features.Admin.Models
{
    public class SystemAnnouncement
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        public string Type { get; set; } = "Info"; // Info, Warning, Error, Success

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
