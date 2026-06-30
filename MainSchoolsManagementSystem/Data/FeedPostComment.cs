using System;

namespace MainSchoolsManagementSystem.Data
{
    public class FeedPostComment
    {
        public int Id { get; set; }
        public int FeedPostId { get; set; }
        public FeedPost FeedPost { get; set; } = null!;
        public string AuthorId { get; set; } = "";
        public ApplicationUser Author { get; set; } = null!;
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
