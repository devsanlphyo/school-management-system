using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainSchoolsManagementSystem.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace MainSchoolsManagementSystem.Features.Feed.Services
{
    public class FeedService : IFeedService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public FeedService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<FeedPost>> GetAllPostsAsync()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.FeedPosts
                .AsNoTracking()
                .Include(p => p.Author)
                .Include(p => p.MediaItems)
                .Include(p => p.Reactions)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Author)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<FeedPost> CreatePostAsync(string userId, string content, PostVisibility visibility = PostVisibility.Public)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var post = new FeedPost
            {
                AuthorId = userId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                Visibility = visibility
            };
            
            context.FeedPosts.Add(post);
            await context.SaveChangesAsync();
            return post;
        }

        public async Task AddMediaToPostAsync(int postId, IBrowserFile file, string storedFileName, int sortOrder)
        {
            await AddMediaToPostAsync(postId, file.Name, storedFileName, file.ContentType, file.Size, sortOrder);
        }

        public async Task AddMediaToPostAsync(int postId, string fileName, string storedFileName, string contentType, long fileSize, int sortOrder)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var media = new FeedPostMedia
            {
                FeedPostId = postId,
                FileName = fileName,
                StoredFileName = storedFileName,
                ContentType = contentType,
                FileSize = fileSize,
                SortOrder = sortOrder
            };
            context.FeedPostMedias.Add(media);
            await context.SaveChangesAsync();
        }

        public async Task ToggleReactionAsync(int postId, string userId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var existing = await context.FeedPostReactions
                .FirstOrDefaultAsync(r => r.FeedPostId == postId && r.UserId == userId);
                
            if (existing != null)
            {
                context.FeedPostReactions.Remove(existing);
            }
            else
            {
                context.FeedPostReactions.Add(new FeedPostReaction 
                { 
                    FeedPostId = postId, 
                    UserId = userId 
                });
            }
            await context.SaveChangesAsync();
        }

        public async Task<FeedPostComment> AddCommentAsync(int postId, string userId, string content)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var comment = new FeedPostComment
            {
                FeedPostId = postId,
                AuthorId = userId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };
            
            context.FeedPostComments.Add(comment);
            await context.SaveChangesAsync();
            
            return comment;
        }
        
        public async Task DeletePostAsync(int postId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var toDelete = await context.FeedPosts
                .Include(p => p.MediaItems)
                .FirstOrDefaultAsync(p => p.Id == postId);
                
            if (toDelete != null)
            {
                var uploadDir = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "uploads", "feed");
                foreach (var media in toDelete.MediaItems)
                {
                    var filePath = System.IO.Path.Combine(uploadDir, media.StoredFileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                context.FeedPosts.Remove(toDelete);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdatePostAsync(int postId, string content)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var post = await context.FeedPosts.FindAsync(postId);
            if (post != null)
            {
                post.Content = content;
                post.UpdatedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }
        }

        public async Task<FeedPost?> GetPostByIdAsync(int postId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.FeedPosts
                .Include(p => p.MediaItems)
                .FirstOrDefaultAsync(p => p.Id == postId);
        }

        public async Task UpdatePostContentAndVisibilityAsync(int postId, string content, PostVisibility visibility)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var post = await context.FeedPosts.FindAsync(postId);
            if (post != null)
            {
                post.Content = content;
                post.Visibility = visibility;
                post.UpdatedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var toDelete = await context.FeedPostComments.FindAsync(commentId);
            if (toDelete != null)
            {
                context.FeedPostComments.Remove(toDelete);
                await context.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<string, string>> GetUserRolesForFeedAsync(IEnumerable<string> userIds)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var rolesQuery = await context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId))
                .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .ToListAsync();
                
            return rolesQuery
                .GroupBy(x => x.UserId)
                .ToDictionary(g => g.Key, g => g.First().Name!);
        }
    }
}
