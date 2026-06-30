using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MainSchoolsManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MainSchoolsManagementSystem.Tests
{
    public class LessonPlansApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public LessonPlansApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private async Task SeedDataAsync(
            string teacherId, int schoolId, int lessonPlanId, 
            bool hasJustification = false, string justificationText = null)
        {
            using var scope = _factory.Services.CreateScope();
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            using var db = dbContextFactory.CreateDbContext();

            // Clear tables in dependent order
            db.LessonPlans.RemoveRange(db.LessonPlans);
            db.Users.RemoveRange(db.Users);
            db.Schools.RemoveRange(db.Schools);
            await db.SaveChangesAsync();

            // Seed School
            var school = new School { Id = schoolId, Name = $"School {schoolId}" };
            db.Schools.Add(school);

            // Seed Teacher
            var teacher = new ApplicationUser
            {
                Id = teacherId,
                UserName = $"teacher_{teacherId}@test.com",
                Email = $"teacher_{teacherId}@test.com",
                FullName = $"Teacher {teacherId}",
                SchoolId = schoolId
            };
            db.Users.Add(teacher);

            // Seed LessonPlan
            var lessonPlan = new LessonPlan
            {
                Id = lessonPlanId,
                TeacherId = teacherId,
                UploadedAt = DateTime.UtcNow,
                IsLate = hasJustification,
                JustificationText = justificationText,
                HasJustificationAttachment = hasJustification
            };
            db.LessonPlans.Add(lessonPlan);

            await db.SaveChangesAsync();
        }

        private async Task SeedUserOnlyAsync(string userId, int schoolId)
        {
            using var scope = _factory.Services.CreateScope();
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            using var db = dbContextFactory.CreateDbContext();

            var user = new ApplicationUser
            {
                Id = userId,
                UserName = $"user_{userId}@test.com",
                Email = $"user_{userId}@test.com",
                FullName = $"User {userId}",
                SchoolId = schoolId
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        private void WriteTestFile(string filename, string content)
        {
            var path = Path.Combine(_factory.TempUploadsPath, "uploads", filename);
            File.WriteAllText(path, content);
        }

        // 1. Unauthenticated user -> 401 Unauthorized
        [Fact]
        public async Task Download_Unauthenticated_Returns401Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/lesson-plans/1/download");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // 2. Teacher who owns the plan -> 200 OK
        [Fact]
        public async Task Download_TeacherOwnsPlan_Returns200Ok()
        {
            // Arrange
            var teacherId = "teacher1";
            var schoolId = 10;
            var planId = 100;
            await SeedDataAsync(teacherId, schoolId, planId);
            WriteTestFile($"lessonplan_{planId}.pdf", "dummy pdf content");

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Test-UserId", teacherId);
            client.DefaultRequestHeaders.Add("X-Test-Role", "Teacher");
            client.DefaultRequestHeaders.Add("X-Test-SchoolId", schoolId.ToString());

            // Act
            var response = await client.GetAsync($"/api/lesson-plans/{planId}/download");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("dummy pdf content", content);
            Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
        }

        // 3. Teacher who does not own the plan -> 403 Forbidden
        [Fact]
        public async Task Download_TeacherDoesNotOwnPlan_Returns403Forbidden()
        {
            // Arrange
            var ownerId = "teacher1";
            var otherTeacherId = "teacher2";
            var schoolId = 10;
            var planId = 101;
            await SeedDataAsync(ownerId, schoolId, planId);
            WriteTestFile($"lessonplan_{planId}.pdf", "dummy pdf content");

            // Seed the other teacher too
            await SeedUserOnlyAsync(otherTeacherId, schoolId);

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Test-UserId", otherTeacherId);
            client.DefaultRequestHeaders.Add("X-Test-Role", "Teacher");
            client.DefaultRequestHeaders.Add("X-Test-SchoolId", schoolId.ToString());

            // Act
            var response = await client.GetAsync($"/api/lesson-plans/{planId}/download");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        // 4. Headmaster from the same school -> 200 OK
        [Fact]
        public async Task Download_HeadmasterSameSchool_Returns200Ok()
        {
            // Arrange
            var teacherId = "teacher1";
            var schoolId = 10;
            var planId = 102;
            await SeedDataAsync(teacherId, schoolId, planId);
            WriteTestFile($"lessonplan_{planId}.pdf", "dummy pdf content");

            var headmasterId = "headmaster1";
            await SeedUserOnlyAsync(headmasterId, schoolId);

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Test-UserId", headmasterId);
            client.DefaultRequestHeaders.Add("X-Test-Role", "Headmaster");
            client.DefaultRequestHeaders.Add("X-Test-SchoolId", schoolId.ToString());

            // Act
            var response = await client.GetAsync($"/api/lesson-plans/{planId}/download");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("dummy pdf content", content);
        }

        // 5. Headmaster from a different school -> 403 Forbidden
        [Fact]
        public async Task Download_HeadmasterDifferentSchool_Returns403Forbidden()
        {
            // Arrange
            var teacherId = "teacher1";
            var schoolId = 10;
            var otherSchoolId = 11;
            var planId = 103;
            await SeedDataAsync(teacherId, schoolId, planId);
            WriteTestFile($"lessonplan_{planId}.pdf", "dummy pdf content");

            var headmasterId = "headmaster2";
            await SeedUserOnlyAsync(headmasterId, otherSchoolId);

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Test-UserId", headmasterId);
            client.DefaultRequestHeaders.Add("X-Test-Role", "Headmaster");
            client.DefaultRequestHeaders.Add("X-Test-SchoolId", otherSchoolId.ToString());

            // Act
            var response = await client.GetAsync($"/api/lesson-plans/{planId}/download");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        // 6a. Justification download for a plan with justification -> 200 OK
        [Fact]
        public async Task DownloadJustification_WithJustification_Returns200Ok()
        {
            // Arrange
            var teacherId = "teacher1";
            var schoolId = 10;
            var planId = 104;
            await SeedDataAsync(teacherId, schoolId, planId, hasJustification: true, justificationText: "Late due to sickness");
            WriteTestFile($"justification_{planId}.docx", "dummy docx content");

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Test-UserId", teacherId);
            client.DefaultRequestHeaders.Add("X-Test-Role", "Teacher");
            client.DefaultRequestHeaders.Add("X-Test-SchoolId", schoolId.ToString());

            // Act
            var response = await client.GetAsync($"/api/lesson-plans/{planId}/justification/download");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("dummy docx content", content);
            Assert.Equal("application/vnd.openxmlformats-officedocument.wordprocessingml.document", response.Content.Headers.ContentType?.MediaType);
        }

        // 6b. Justification download for a plan without justification -> 404 NotFound
        [Fact]
        public async Task DownloadJustification_WithoutJustification_Returns404NotFound()
        {
            // Arrange
            var teacherId = "teacher1";
            var schoolId = 10;
            var planId = 105;
            await SeedDataAsync(teacherId, schoolId, planId, hasJustification: false);
            // DO NOT write the justification file

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Test-UserId", teacherId);
            client.DefaultRequestHeaders.Add("X-Test-Role", "Teacher");
            client.DefaultRequestHeaders.Add("X-Test-SchoolId", schoolId.ToString());

            // Act
            var response = await client.GetAsync($"/api/lesson-plans/{planId}/justification/download");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
