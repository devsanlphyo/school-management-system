using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace MainSchoolsManagementSystem.Features.Feed.Services
{
    public interface IFeedService
    {
        Task<List<FeedPost>> GetAllPostsAsync();
        Task<FeedPost> CreatePostAsync(string userId, string content, PostVisibility visibility = PostVisibility.Public);
        Task AddMediaToPostAsync(int postId, IBrowserFile file, string storedFileName, int sortOrder);
        Task AddMediaToPostAsync(int postId, string fileName, string storedFileName, string contentType, long fileSize, int sortOrder);
        Task ToggleReactionAsync(int postId, string userId);
        Task<FeedPostComment> AddCommentAsync(int postId, string userId, string content);
        Task DeletePostAsync(int postId);
        Task UpdatePostAsync(int postId, string content);
        Task<FeedPost?> GetPostByIdAsync(int postId);
        Task UpdatePostContentAndVisibilityAsync(int postId, string content, PostVisibility visibility);
        Task DeleteCommentAsync(int commentId);
        Task<Dictionary<string, string>> GetUserRolesForFeedAsync(IEnumerable<string> userIds);
    }
}
