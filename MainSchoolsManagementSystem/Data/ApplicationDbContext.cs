using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MainSchoolsManagementSystem.Features.Notifications.Models;

namespace MainSchoolsManagementSystem.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<School> Schools { get; set; }
        public DbSet<LessonPlan> LessonPlans { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SchoolClass> SchoolClasses { get; set; }
        public DbSet<TeacherAssignment> TeacherAssignments { get; set; }
        
        public DbSet<FeedPost> FeedPosts { get; set; }
        public DbSet<FeedPostMedia> FeedPostMedias { get; set; }
        public DbSet<FeedPostReaction> FeedPostReactions { get; set; }
        public DbSet<FeedPostComment> FeedPostComments { get; set; }
        public DbSet<UserTrustedDevice> UserTrustedDevices { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        
        public DbSet<MainSchoolsManagementSystem.Features.Admin.Models.SystemAnnouncement> SystemAnnouncements { get; set; }
        public DbSet<MainSchoolsManagementSystem.Features.Admin.Models.SystemErrorLog> SystemErrorLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Ignore unwanted default IdentityUser columns in the database
            // builder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumber);
            builder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumberConfirmed);
            builder.Entity<ApplicationUser>().Ignore(u => u.TwoFactorEnabled);
            builder.Entity<ApplicationUser>().Ignore(u => u.LockoutEnd);
            builder.Entity<ApplicationUser>().Ignore(u => u.LockoutEnabled);
            builder.Entity<ApplicationUser>().Ignore(u => u.AccessFailedCount);
            builder.Entity<ApplicationUser>().Ignore(u => u.EmailConfirmed);

            // Configure foreign key relations
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.School)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.SchoolId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LessonPlan>()
                .HasOne(lp => lp.Teacher)
                .WithMany()
                .HasForeignKey(lp => lp.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Attendance>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LeaveRequest>()
                .HasOne(lr => lr.User)
                .WithMany()
                .HasForeignKey(lr => lr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Department>()
                .HasOne(d => d.School)
                .WithMany()
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Subject>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Subjects)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SchoolClass>()
                .HasOne(c => c.School)
                .WithMany()
                .HasForeignKey(c => c.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TeacherAssignment>()
                .HasOne(ta => ta.Teacher)
                .WithMany()
                .HasForeignKey(ta => ta.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TeacherAssignment>()
                .HasOne(ta => ta.Class)
                .WithMany()
                .HasForeignKey(ta => ta.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TeacherAssignment>()
                .HasOne(ta => ta.Subject)
                .WithMany()
                .HasForeignKey(ta => ta.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FeedPost>()
                .HasOne(fp => fp.Author)
                .WithMany()
                .HasForeignKey(fp => fp.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FeedPostMedia>()
                .HasOne(fpm => fpm.FeedPost)
                .WithMany(fp => fp.MediaItems)
                .HasForeignKey(fpm => fpm.FeedPostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FeedPostReaction>()
                .HasOne(fpr => fpr.FeedPost)
                .WithMany(fp => fp.Reactions)
                .HasForeignKey(fpr => fpr.FeedPostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FeedPostReaction>()
                .HasOne(fpr => fpr.User)
                .WithMany()
                .HasForeignKey(fpr => fpr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FeedPostReaction>()
                .HasIndex(fpr => new { fpr.FeedPostId, fpr.UserId })
                .IsUnique();

            builder.Entity<FeedPostComment>()
                .HasOne(fpc => fpc.FeedPost)
                .WithMany(fp => fp.Comments)
                .HasForeignKey(fpc => fpc.FeedPostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FeedPostComment>()
                .HasOne(fpc => fpc.Author)
                .WithMany()
                .HasForeignKey(fpc => fpc.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Notification>()
                .HasOne(n => n.Recipient)
                .WithMany()
                .HasForeignKey(n => n.RecipientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Notification>()
                .HasOne(n => n.TriggerUser)
                .WithMany()
                .HasForeignKey(n => n.TriggerUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Notification>()
                .HasOne(n => n.FeedPost)
                .WithMany()
                .HasForeignKey(n => n.FeedPostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
