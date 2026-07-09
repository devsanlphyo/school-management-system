using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MainSchoolsManagementSystem.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace MainSchoolsManagementSystem.Features.LessonPlans.Services
{
    public class LessonPlanService : ILessonPlanService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public LessonPlanService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<LessonPlan>> GetTeacherLessonPlansAsync(string teacherId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.LessonPlans
                .Include(lp => lp.Class)
                .Include(lp => lp.Subject)
                .Where(lp => lp.TeacherId == teacherId)
                .OrderByDescending(lp => lp.UploadedAt)
                .ToListAsync();
        }

        public async Task<List<LessonPlan>> GetTeacherTodayLessonPlansAsync(string teacherId)
        {
            var today = DateTime.Today;
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.LessonPlans
                .Include(lp => lp.Class)
                .Include(lp => lp.Subject)
                .Where(lp => lp.TeacherId == teacherId && lp.UploadedAt >= today && lp.UploadedAt < today.AddDays(1))
                .ToListAsync();
        }

        public async Task<List<LessonPlan>> GetLessonPlansBySchoolIdAsync(int schoolId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.LessonPlans
                .Include(lp => lp.Teacher)
                .Where(lp => lp.Teacher != null && lp.Teacher.SchoolId == schoolId)
                .OrderByDescending(lp => lp.UploadedAt)
                .ToListAsync();
        }

        public async Task<List<LessonPlan>> GetAllLessonPlansAsync()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.LessonPlans
                .Include(lp => lp.Teacher)
                    .ThenInclude(t => t!.School)
                .OrderByDescending(lp => lp.UploadedAt)
                .ToListAsync();
        }

        public async Task<LessonPlan> SubmitLessonPlanAsync(string teacherId, int classId, int subjectId, bool isLate, string? justification, List<IBrowserFile> files, string webRootPath)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            
            var lp = new LessonPlan
            {
                TeacherId = teacherId,
                ClassId = classId,
                SubjectId = subjectId,
                UploadedAt = DateTime.Now,
                IsLate = isLate,
                JustificationText = isLate ? justification : null,
                Status = LessonPlanStatus.Pending,
                HasJustificationAttachment = false
            };

            context.LessonPlans.Add(lp);
            await context.SaveChangesAsync();

            var uploadsPath = Path.Combine(webRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            int counter = 1;
            foreach (var file in files)
            {
                var lpExt = Path.GetExtension(file.Name);
                var lpPath = Path.Combine(uploadsPath, $"lessonplan_{lp.Id}_{counter}{lpExt}");
                using (var fs = new FileStream(lpPath, FileMode.Create))
                {
                    await file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(fs);
                }
                counter++;
            }

            return lp;
        }

        public async Task UpdateLessonPlanStatusAsync(int planId, LessonPlanStatus status, string feedback)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var plan = await context.LessonPlans.FindAsync(planId);
            if (plan != null)
            {
                plan.Status = status;
                plan.Feedback = feedback;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteLessonPlanAsync(int planId, string webRootPath)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var plan = await context.LessonPlans.FindAsync(planId);
            if (plan != null)
            {
                // Delete associated files
                var uploadsPath = Path.Combine(webRootPath, "uploads");
                if (Directory.Exists(uploadsPath))
                {
                    var files = Directory.GetFiles(uploadsPath, $"lessonplan_{plan.Id}_*.*");
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                    var fileExact = Path.Combine(uploadsPath, $"lessonplan_{plan.Id}.jpg"); // legacy?
                    if (File.Exists(fileExact)) File.Delete(fileExact);
                }

                context.LessonPlans.Remove(plan);
                await context.SaveChangesAsync();
            }
        }

        public Dictionary<int, List<string>> GetLessonPlanUrls(string webRootPath)
        {
            var dict = new Dictionary<int, List<string>>();
            var uploadsPath = Path.Combine(webRootPath, "uploads");
            
            if (Directory.Exists(uploadsPath))
            {
                var files = Directory.GetFiles(uploadsPath);
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    if (fileName.StartsWith("lessonplan_"))
                    {
                        var parts = Path.GetFileNameWithoutExtension(fileName).Split('_');
                        if (parts.Length >= 2 && int.TryParse(parts[1], out int id))
                        {
                            if (!dict.ContainsKey(id)) dict[id] = new List<string>();
                            dict[id].Add($"/uploads/{fileName}");
                        }
                    }
                }
            }
            return dict;
        }
    }
}
