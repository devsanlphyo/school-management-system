using System;
using MainSchoolsManagementSystem.Features.Users.Models;
using MainSchoolsManagementSystem.Features.Feed.Models;

namespace MainSchoolsManagementSystem.Features.Notifications.Models
{
    public class Notification
    {
        public int Id { get; set; }
        
        public string RecipientId { get; set; } = "";
        public ApplicationUser Recipient { get; set; } = null!;
        
        public string TriggerUserId { get; set; } = "";
        public ApplicationUser TriggerUser { get; set; } = null!;
        
        public int FeedPostId { get; set; }
        public FeedPost FeedPost { get; set; } = null!;
        
        public string Type { get; set; } = ""; // "Reaction" or "Comment"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
