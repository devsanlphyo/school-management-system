namespace MainSchoolsManagementSystem.Data
{
    public class FeedPostMedia
    {
        public int Id { get; set; }
        public int FeedPostId { get; set; }
        public FeedPost FeedPost { get; set; } = null!;
        public string FileName { get; set; } = "";
        public string StoredFileName { get; set; } = "";
        public string ContentType { get; set; } = "";
        public long FileSize { get; set; }
        public int SortOrder { get; set; }
    }
}
