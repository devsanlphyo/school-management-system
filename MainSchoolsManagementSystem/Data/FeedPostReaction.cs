using System;

namespace MainSchoolsManagementSystem.Data
{
    public class FeedPostReaction
    {
        public int Id { get; set; }
        public int FeedPostId { get; set; }
        public FeedPost FeedPost { get; set; } = null!;
        public string UserId { get; set; } = "";
        public ApplicationUser User { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
