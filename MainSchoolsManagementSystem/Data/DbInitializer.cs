using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MainSchoolsManagementSystem.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created/migrated
            if (context.Database.IsSqlite())
            {
                await context.Database.EnsureCreatedAsync();
            }
            else
            {
                await context.Database.MigrateAsync();
            }

            // 1. Seed Roles
            string[] roleNames = { "Admin", "Teacher" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Seed default School
            var school = await context.Schools.FirstOrDefaultAsync();
            if (school == null)
            {
                school = new School { Name = "Greenwood High School" };
                context.Schools.Add(school);
                await context.SaveChangesAsync();
            }

            // 3. Seed System Settings if missing
            var settings = await context.SystemSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new SystemSetting { DailyDeadline = new TimeSpan(8, 30, 0) };
                context.SystemSettings.Add(settings);
                await context.SaveChangesAsync();
            }

            // 4. Seed Default Admin
            var adminEmail = "admin@greenwood.edu";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    SchoolId = school.Id,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 5. Seed Default Teachers (if none exist)
            var teachers = new List<(string Email, string Name)>
            {
                ("sarah.connor@greenwood.edu", "Sarah Connor"),
                ("john.doe@greenwood.edu", "John Doe"),
                ("emily.smith@greenwood.edu", "Emily Smith"),
                ("michael.brown@greenwood.edu", "Michael Brown")
            };

            var seededTeachers = new List<ApplicationUser>();

            foreach (var t in teachers)
            {
                var teacherUser = await userManager.FindByEmailAsync(t.Email);
                if (teacherUser == null)
                {
                    teacherUser = new ApplicationUser
                    {
                        UserName = t.Email,
                        Email = t.Email,
                        SchoolId = school.Id,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(teacherUser, "Teacher123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(teacherUser, "Teacher");
                        seededTeachers.Add(teacherUser);
                    }
                }
                else
                {
                    seededTeachers.Add(teacherUser);
                }
            }

            // 6. Seed Mock Data for Dashboard/Management if the DB is fresh
            if (seededTeachers.Any() && !await context.Attendances.AnyAsync())
            {
                var today = DateTime.Today;
                var rand = new Random();

                // Seed Attendance for the last 3 days
                for (int i = 0; i < 3; i++)
                {
                    var date = today.AddDays(-i);
                    // Skip weekends
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) continue;

                    foreach (var teacher in seededTeachers)
                    {
                        // 80% Present, 15% Late, 5% Absent
                        var roll = rand.Next(100);
                        AttendanceStatus status;
                        DateTime? checkIn = null;

                        if (roll < 80)
                        {
                            status = AttendanceStatus.Present;
                            checkIn = date.AddHours(7).AddMinutes(rand.Next(1, 29)); // Before 8:30 AM
                        }
                        else if (roll < 95)
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
                            UserId = teacher.Id,
                            Date = date,
                            CheckedInAt = checkIn,
                            Status = status
                        });
                    }
                }

                // Seed some Pending and Approved Leave Requests
                context.LeaveRequests.Add(new LeaveRequest
                {
                    UserId = seededTeachers[0].Id,
                    TargetDate = today.AddDays(2),
                    SubmittedAt = today.AddDays(-1),
                    Status = LeaveStatus.Pending,
                    Reason = "Medical appointment for dental checkup."
                });

                context.LeaveRequests.Add(new LeaveRequest
                {
                    UserId = seededTeachers[1].Id,
                    TargetDate = today.AddDays(5),
                    SubmittedAt = today.AddDays(-2),
                    Status = LeaveStatus.Pending,
                    Reason = "Family function and travel out of town."
                });

                context.LeaveRequests.Add(new LeaveRequest
                {
                    UserId = seededTeachers[2].Id,
                    TargetDate = today.AddDays(-3),
                    SubmittedAt = today.AddDays(-5),
                    Status = LeaveStatus.Approved,
                    Reason = "Attending educational conference."
                });

                // Seed some Lesson Plans
                context.LessonPlans.Add(new LessonPlan
                {
                    TeacherId = seededTeachers[0].Id,
                    UploadedAt = today.AddHours(-4),
                    IsLate = false,
                    Status = LessonPlanStatus.Pending
                });

                context.LessonPlans.Add(new LessonPlan
                {
                    TeacherId = seededTeachers[1].Id,
                    UploadedAt = today.AddHours(-1),
                    IsLate = true,
                    JustificationText = "Stuck in severe traffic commute this morning.",
                    HasJustificationAttachment = false,
                    Status = LessonPlanStatus.Pending
                });

                context.LessonPlans.Add(new LessonPlan
                {
                    TeacherId = seededTeachers[2].Id,
                    UploadedAt = today.AddDays(-1).AddHours(-5),
                    IsLate = false,
                    Status = LessonPlanStatus.Reviewed,
                    Feedback = "Excellent structure and objectives."
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
