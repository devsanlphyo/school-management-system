using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MainSchoolsManagementSystem.Data
{
    public static class DbTestRunner
    {
        public static async Task RunTestsAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            Console.WriteLine("\n==================================================");
            Console.WriteLine("🚀 STARTING DATABASE RELATIONSHIP TESTS...");
            Console.WriteLine("==================================================\n");

            try
            {
                // 1. Create a Test School
                var school = new School { Name = "St. Jude Academy" };
                context.Schools.Add(school);
                await context.SaveChangesAsync();
                Console.WriteLine($"[SUCCESS] Created School: {school.Name} (ID: {school.Id})");

                // 2. Create a Test Teacher (User)
                var teacher = new ApplicationUser
                {
                    UserName = "teacher@stjude.edu",
                    Email = "teacher@stjude.edu",
                    FullName = "Alice Johnson",
                    PhoneNumber = "1234567890",
                    SchoolId = school.Id
                };
                
                var userResult = await userManager.CreateAsync(teacher, "TestPassword123!");
                if (!userResult.Succeeded)
                {
                    throw new Exception($"Failed to create test user: {string.Join(", ", userResult.Errors.Select(e => e.Description))}");
                }
                Console.WriteLine($"[SUCCESS] Created Teacher: {teacher.UserName} (ID: {teacher.Id}) linked to School ID: {teacher.SchoolId}");

                // 3. Create a Department
                var department = new Department
                {
                    Name = "Mathematics Department",
                    SchoolId = school.Id
                };
                context.Departments.Add(department);
                await context.SaveChangesAsync();
                Console.WriteLine($"[SUCCESS] Created Department: {department.Name} (ID: {department.Id})");

                // 4. Create a Subject
                var subject = new Subject
                {
                    Name = "Algebra I",
                    DepartmentId = department.Id
                };
                context.Subjects.Add(subject);

                // 5. Create a School Class
                var schoolClass = new SchoolClass
                {
                    Name = "Grade 10-A",
                    GradeLevel = "Grade10",
                    SchoolId = school.Id
                };
                context.SchoolClasses.Add(schoolClass);
                await context.SaveChangesAsync();
                Console.WriteLine($"[SUCCESS] Created Subject: {subject.Name} (ID: {subject.Id}) & Class: {schoolClass.Name} (ID: {schoolClass.Id})");

                // 6. Create a Teacher Assignment (Sarah teaches Algebra in Grade 10-A)
                var assignment = new TeacherAssignment
                {
                    TeacherId = teacher.Id,
                    ClassId = schoolClass.Id,
                    SubjectId = subject.Id
                };
                context.TeacherAssignments.Add(assignment);

                // 7. Create a Lesson Plan
                var lessonPlan = new LessonPlan
                {
                    TeacherId = teacher.Id,
                    UploadedAt = DateTime.UtcNow,
                    IsLate = false,
                    Status = LessonPlanStatus.Pending
                };
                context.LessonPlans.Add(lessonPlan);

                // 8. Create an Attendance Log
                var attendance = new Attendance
                {
                    UserId = teacher.Id,
                    Date = DateTime.Today,
                    CheckedInAt = DateTime.UtcNow,
                    Status = AttendanceStatus.Present
                };
                context.Attendances.Add(attendance);

                // 9. Create a Leave Request
                var leaveRequest = new LeaveRequest
                {
                    UserId = teacher.Id,
                    StartDate = DateTime.Today.AddDays(2),
                    EndDate = DateTime.Today.AddDays(2),
                    SubmittedAt = DateTime.UtcNow,
                    Status = LeaveStatus.Pending,
                    Reason = "Medical checkup"
                };
                context.LeaveRequests.Add(leaveRequest);

                await context.SaveChangesAsync();
                Console.WriteLine("[SUCCESS] Successfully inserted LessonPlan, Attendance, LeaveRequest, and TeacherAssignment records.");

                // 10. Query and Verify Relationships (Normalized)
                Console.WriteLine("\n--------------------------------------------------");
                Console.WriteLine("🔍 VERIFYING RELATIONSHIPS VIA QUERIES...");
                Console.WriteLine("--------------------------------------------------");

                // Query Teacher Assignments (joining Teacher, Class, Subject, Department, and School)
                var assignments = await context.TeacherAssignments
                    .Include(ta => ta.Teacher)
                    .ThenInclude(t => t!.School)
                    .Include(ta => ta.Class)
                    .Include(ta => ta.Subject)
                    .ThenInclude(s => s!.Department)
                    .Where(ta => ta.TeacherId == teacher.Id)
                    .ToListAsync();

                Console.WriteLine($"Found {assignments.Count} Teacher Assignment(s):");
                foreach (var ta in assignments)
                {
                    Console.WriteLine($"  - Teacher: {ta.Teacher?.UserName}");
                    Console.WriteLine($"    Teaches Subject: {ta.Subject?.Name} (under {ta.Subject?.Department?.Name})");
                    Console.WriteLine($"    In Class: {ta.Class?.Name}");
                    Console.WriteLine($"    At School: {ta.Teacher?.School?.Name}");
                }

                // Query Lesson Plans
                var plansInSchool = await context.LessonPlans
                    .Include(lp => lp.Teacher)
                    .ThenInclude(t => t!.School)
                    .Where(lp => lp.Teacher != null && lp.Teacher.SchoolId == school.Id)
                    .ToListAsync();

                Console.WriteLine($"Found {plansInSchool.Count} Lesson Plan(s) for School '{school.Name}':");
                foreach (var lp in plansInSchool)
                {
                    Console.WriteLine($"  - Plan ID: {lp.Id}, Uploaded By: {lp.Teacher?.UserName}, School: {lp.Teacher?.School?.Name}, Status: {lp.Status}");
                }

                // 11. Cleanup
                Console.WriteLine("\n--------------------------------------------------");
                Console.WriteLine("🧹 CLEANING UP TEST DATA...");
                Console.WriteLine("--------------------------------------------------");

                context.LeaveRequests.Remove(leaveRequest);
                context.Attendances.Remove(attendance);
                context.LessonPlans.Remove(lessonPlan);
                context.TeacherAssignments.Remove(assignment);
                await context.SaveChangesAsync(); // Delete assignment first due to Restrict behavior

                context.Subjects.Remove(subject);
                context.SchoolClasses.Remove(schoolClass);
                await context.SaveChangesAsync();

                context.Departments.Remove(department);
                await userManager.DeleteAsync(teacher);
                context.Schools.Remove(school);
                await context.SaveChangesAsync();
                
                Console.WriteLine("[SUCCESS] Cleaned up all test records successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] Test execution failed: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }

            Console.WriteLine("\n==================================================");
            Console.WriteLine("🏁 DATABASE RELATIONSHIP TESTS COMPLETE.");
            Console.WriteLine("==================================================\n");
        }
    }
}
