using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainSchoolsManagementSystem.Features.TrustedDevices.Models
{
    public enum DeviceStatus
    {
        Pending,
        Approved,
        Revoked
    }

    public class UserTrustedDevice
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [MaxLength(256)]
        public string DeviceHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string DeviceName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? IpAddress { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset LastUsedAt { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }
        
        public DeviceStatus Status { get; set; } = DeviceStatus.Pending;
    }
}
