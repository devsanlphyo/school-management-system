using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainSchoolsManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace MainSchoolsManagementSystem.Features.Schools.Services
{
    public class SchoolService : ISchoolService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public SchoolService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<School>> GetSchoolsWithUsersAsync()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Schools.Include(s => s.Users).ToListAsync();
        }

        public async Task<School?> GetSchoolByIdAsync(int id)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Schools.FindAsync(id);
        }

        public async Task<bool> SchoolExistsAsync(string name, int? excludeId = null)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var query = context.Schools.Where(s => s.Name.ToLower() == name.Trim().ToLower());
            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task CreateSchoolAsync(School school)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            context.Schools.Add(school);
            await context.SaveChangesAsync();
        }

        public async Task UpdateSchoolAsync(School school)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var existing = await context.Schools.FindAsync(school.Id);
            if (existing != null)
            {
                existing.Name = school.Name;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteSchoolAsync(int id)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var school = await context.Schools
                .Include(s => s.Users)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (school != null)
            {
                // Encapsulated logic: Nullify associated users before deletion
                foreach (var user in school.Users)
                {
                    user.SchoolId = null;
                    user.DepartmentId = null;
                }
                context.Schools.Remove(school);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Department>> GetDepartmentsBySchoolIdAsync(int schoolId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Departments.Where(d => d.SchoolId == schoolId).ToListAsync();
        }

        public async Task CreateDepartmentAsync(Department department)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            context.Departments.Add(department);
            await context.SaveChangesAsync();
        }

        public async Task DeleteDepartmentAsync(int departmentId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var dept = await context.Departments.FindAsync(departmentId);
            if (dept != null)
            {
                context.Departments.Remove(dept);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<SchoolClass>> GetClassesBySchoolIdAsync(int schoolId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.SchoolClasses.Where(c => c.SchoolId == schoolId).ToListAsync();
        }

        public async Task CreateSchoolClassAsync(SchoolClass schoolClass)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            context.SchoolClasses.Add(schoolClass);
            await context.SaveChangesAsync();
        }

        public async Task DeleteSchoolClassAsync(int classId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var cls = await context.SchoolClasses.FindAsync(classId);
            if (cls != null)
            {
                context.SchoolClasses.Remove(cls);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Subject>> GetSubjectsByDepartmentIdsAsync(List<int> departmentIds)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Subjects
                .Include(s => s.Department)
                .Where(s => departmentIds.Contains(s.DepartmentId))
                .ToListAsync();
        }

        public async Task CreateSubjectAsync(Subject subject)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            context.Subjects.Add(subject);
            await context.SaveChangesAsync();
        }

        public async Task DeleteSubjectAsync(int subjectId)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var sub = await context.Subjects.FindAsync(subjectId);
            if (sub != null)
            {
                context.Subjects.Remove(sub);
                await context.SaveChangesAsync();
            }
        }
    }
}
