using System;

namespace MainSchoolsManagementSystem.Features.Admin.Models
{
    public class SystemErrorLog
    {
        public int Id { get; set; }
        
        public string ExceptionMessage { get; set; } = string.Empty;
        
        public string StackTrace { get; set; } = string.Empty;
        
        public string RequestPath { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsResolved { get; set; } = false;
    }
}
