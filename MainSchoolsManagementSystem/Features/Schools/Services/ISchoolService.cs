using System.Collections.Generic;
using System.Threading.Tasks;

namespace MainSchoolsManagementSystem.Features.Schools.Services
{
    public interface ISchoolService
    {
        Task<List<School>> GetSchoolsWithUsersAsync();
        Task<School?> GetSchoolByIdAsync(int id);
        Task<bool> SchoolExistsAsync(string name, int? excludeId = null);
        Task CreateSchoolAsync(School school);
        Task UpdateSchoolAsync(School school);
        Task DeleteSchoolAsync(int id);

        Task<List<Department>> GetDepartmentsBySchoolIdAsync(int schoolId);
        Task CreateDepartmentAsync(Department department);
        Task DeleteDepartmentAsync(int departmentId);

        Task<List<SchoolClass>> GetClassesBySchoolIdAsync(int schoolId);
        Task CreateSchoolClassAsync(SchoolClass schoolClass);
        Task DeleteSchoolClassAsync(int classId);

        Task<List<Subject>> GetSubjectsByDepartmentIdsAsync(List<int> departmentIds);
        Task CreateSubjectAsync(Subject subject);
        Task DeleteSubjectAsync(int subjectId);
    }
}
