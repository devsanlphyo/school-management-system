using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MainSchoolsManagementSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is migrated
            await context.Database.MigrateAsync();

            // 1. Seed Roles
            string[] roles = { "Admin", "Director", "Headmaster", "Officer", "Teacher", "Assistant" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Schools & Academic Structures
            if (!await context.Schools.AnyAsync())
            {
                var stJude = new School { Name = "St. Jude Academy" };
                var oakridge = new School { Name = "Oakridge High" };
                context.Schools.AddRange(stJude, oakridge);
                await context.SaveChangesAsync();
            }

            var stJudeSchool = await context.Schools.FirstOrDefaultAsync(s => s.Name == "St. Jude Academy");
            var oakridgeSchool = await context.Schools.FirstOrDefaultAsync(s => s.Name == "Oakridge High");

            // 3. Seed System Settings if missing
            if (!await context.SystemSettings.AnyAsync())
            {
                // We'll seed a default settings record. 
                context.SystemSettings.Add(new SystemSetting { DailyDeadline = new TimeSpan(8, 30, 0) });
                await context.SaveChangesAsync();
            }

            // 4. Seed Users with Roles & Tenancy
            await SeedUserAsync(userManager, "admin@system.com", "Admin", null, "Global System Admin");
            await SeedUserAsync(userManager, "director@system.com", "Director", null, "Chief School Director");
            
            ApplicationUser? stJudeTeacher = null;
            ApplicationUser? oakridgeTeacher = null;

            if (stJudeSchool != null)
            {
                await SeedUserAsync(userManager, "headmaster@stjude.edu", "Headmaster", stJudeSchool.Id, "Arthur Pendelton");
                await SeedUserAsync(userManager, "officer@stjude.edu", "Officer", stJudeSchool.Id, "Sarah Jenkins");
                stJudeTeacher = await SeedUserAsync(userManager, "teacher@stjude.edu", "Teacher", stJudeSchool.Id, "Alice Johnson");
                
                // Add a couple more teachers for St. Jude to make lists look good
                await SeedUserAsync(userManager, "science.teacher@stjude.edu", "Teacher", stJudeSchool.Id, "David Bowman");
                await SeedUserAsync(userManager, "math.teacher@stjude.edu", "Teacher", stJudeSchool.Id, "Clara Oswald");
            }

            if (oakridgeSchool != null)
            {
                oakridgeTeacher = await SeedUserAsync(userManager, "teacher@oakridge.edu", "Teacher", oakridgeSchool.Id, "Robert Langdon");
                await SeedUserAsync(userManager, "history.teacher@oakridge.edu", "Teacher", oakridgeSchool.Id, "Elizabeth Shaw");
            }

            // 5. Seed Mock Data (Attendance, Leaves, Lesson Plans) for St. Jude Academy
            if (stJudeSchool != null && !await context.Attendances.AnyAsync())
            {
                var today = DateTime.Today;
                var rand = new Random();

                // Get all St. Jude teachers
                var stJudeTeachers = await userManager.GetUsersInRoleAsync("Teacher");
                var schoolTeachers = stJudeTeachers.Where(u => u.SchoolId == stJudeSchool.Id).ToList();

                // Seed Attendance for the last 5 days
                for (int i = 0; i < 5; i++)
                {
                    var date = today.AddDays(-i);
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) continue;

                    foreach (var teacher in schoolTeachers)
                    {
                        var roll = rand.Next(100);
                        AttendanceStatus status;
                        DateTime? checkIn = null;

                        if (roll < 75)
                        {
                            status = AttendanceStatus.Present;
                            checkIn = date.AddHours(7).AddMinutes(rand.Next(15, 55)); // Before 8:30 AM
                        }
                        else if (roll < 90)
                        {
                            status = AttendanceStatus.Late;
                            checkIn = date.AddHours(8).AddMinutes(rand.Next(31, 59)); // After 8:30 AM
                        }
                        else
                        {
                            status = AttendanceStatus.Absent;
                        }

                        context.Attendances.Add(new Attendance
                        {
                            TeacherId = teacher.Id,
                            Date = date,
                            CheckedInAt = checkIn,
                            Status = status
                        });
                    }
                }

                // Seed Leave Requests
                if (schoolTeachers.Count >= 2)
                {
                    context.LeaveRequests.Add(new LeaveRequest
                    {
                        TeacherId = schoolTeachers[0].Id,
                        TargetDate = today.AddDays(1),
                        SubmittedAt = today.AddDays(-1),
                        Status = LeaveStatus.Pending,
                        Reason = "Urgent dental extraction surgery scheduled."
                    });

                    context.LeaveRequests.Add(new LeaveRequest
                    {
                        TeacherId = schoolTeachers[1].Id,
                        TargetDate = today.AddDays(3),
                        SubmittedAt = today.AddDays(-2),
                        Status = LeaveStatus.Pending,
                        Reason = "Family wedding ceremony out of state."
                    });

                    context.LeaveRequests.Add(new LeaveRequest
                    {
                        TeacherId = schoolTeachers[0].Id,
                        TargetDate = today.AddDays(-4),
                        SubmittedAt = today.AddDays(-6),
                        Status = LeaveStatus.Approved,
                        Reason = "Sick leave - influenza flu symptoms."
                    });
                }

                // Seed Lesson Plans
                if (schoolTeachers.Count >= 3)
                {
                    context.LessonPlans.Add(new LessonPlan
                    {
                        TeacherId = schoolTeachers[0].Id,
                        UploadedAt = today.AddHours(-3),
                        IsLate = false,
                        Status = LessonPlanStatus.Pending
                    });

                    context.LessonPlans.Add(new LessonPlan
                    {
                        TeacherId = schoolTeachers[1].Id,
                        UploadedAt = today.AddHours(-1),
                        IsLate = true,
                        JustificationText = "Home internet outage delayed file upload.",
                        HasJustificationAttachment = false,
                        Status = LessonPlanStatus.Pending
                    });

                    context.LessonPlans.Add(new LessonPlan
                    {
                        TeacherId = schoolTeachers[2].Id,
                        UploadedAt = today.AddDays(-1).AddHours(-4),
                        IsLate = false,
                        Status = LessonPlanStatus.Reviewed,
                        Feedback = "Excellent curriculum layout. Approved for teaching."
                    });
                }

                await context.SaveChangesAsync();
            }
        }

        private static async Task<ApplicationUser?> SeedUserAsync(
            UserManager<ApplicationUser> userManager, 
            string email, 
            string role, 
            int? schoolId,
            string fullName)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    SchoolId = schoolId,
                    EmailConfirmed = true,
                    FullName = fullName
                };

                var result = await userManager.CreateAsync(user, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
            return user;
        }
    }
}
