using System;
using System.Collections.Generic;

namespace MainSchoolsManagementSystem.Data
{
    public class FeedPost
    {
        public int Id { get; set; }
        public string AuthorId { get; set; } = "";
        public ApplicationUser Author { get; set; } = null!;
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public ICollection<FeedPostMedia> MediaItems { get; set; } = new List<FeedPostMedia>();
        public ICollection<FeedPostReaction> Reactions { get; set; } = new List<FeedPostReaction>();
        public ICollection<FeedPostComment> Comments { get; set; } = new List<FeedPostComment>();
    }
}
