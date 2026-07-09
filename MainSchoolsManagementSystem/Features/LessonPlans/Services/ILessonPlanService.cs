using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace MainSchoolsManagementSystem.Features.LessonPlans.Services
{
    public interface ILessonPlanService
    {
        Task<List<LessonPlan>> GetTeacherLessonPlansAsync(string teacherId);
        Task<List<LessonPlan>> GetTeacherTodayLessonPlansAsync(string teacherId);
        Task<List<LessonPlan>> GetLessonPlansBySchoolIdAsync(int schoolId);
        Task<List<LessonPlan>> GetAllLessonPlansAsync();
        
        Task<LessonPlan> SubmitLessonPlanAsync(string teacherId, int classId, int subjectId, bool isLate, string? justification, List<IBrowserFile> files, string webRootPath);
        
        Task UpdateLessonPlanStatusAsync(int planId, LessonPlanStatus status, string feedback);
        Task DeleteLessonPlanAsync(int planId, string webRootPath);
        
        Dictionary<int, List<string>> GetLessonPlanUrls(string webRootPath);
    }
}
