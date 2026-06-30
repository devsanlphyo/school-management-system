## 2026-06-30T06:12:36Z

You are the Worker for Milestone 1 of the LessonPlans feature.
Your working directory is: c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\worker_m1

Your task is to implement the secure file storage and download endpoints for lesson plans and justifications, set up the xUnit test project, and write integration tests.

### Implementation Requirements:
1. **Directory Setup**:
   - In `MainSchoolsManagementSystem/Program.cs`, before `app.Run()`, ensure that the `uploads` folder is created in the content root path:
     ```csharp
     var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
     if (!Directory.Exists(uploadsPath))
     {
         Directory.CreateDirectory(uploadsPath);
     }
     ```
2. **Minimal API Endpoints**:
   - In `MainSchoolsManagementSystem/Program.cs`, after `app.MapAdditionalIdentityEndpoints();`, add the two download endpoints:
     - `GET /api/lesson-plans/{id:int}/download`
     - `GET /api/lesson-plans/{id:int}/justification/download`
   - Use `IDbContextFactory<ApplicationDbContext>` inside the endpoint handlers to resolve the DbContext. Eager-load the `Teacher` navigation property when fetching the `LessonPlan`.
   - Implement the following authorization logic:
     - If the user is in the `"Teacher"` role, they must own the lesson plan (i.e. `LessonPlan.TeacherId == currentUserId`).
     - If the user is in the `"Admin"`, `"Headmaster"`, or `"Officer"` role, their `SchoolId` must match the `LessonPlan.Teacher.SchoolId` (retrieve the current user's `SchoolId` from the DB using `currentUserId`).
     - If not authorized, return `Results.Forbid()`.
   - Resolve the physical file:
     - Scan the `uploads` folder using `Directory.EnumerateFiles(uploadsPath, $"lessonplan_{id}.*")` (or `$"justification_{id}.*"`) to find the file dynamically.
     - If the file is not found, return `Results.NotFound()`.
     - Use `FileExtensionContentTypeProvider` to determine the MIME type, defaulting to `application/octet-stream`.
     - Return the file using `Results.File(filePath, contentType, fileDownloadName: Path.GetFileName(filePath))`.
   - Secure the endpoints using `.RequireAuthorization()`.

3. **Test Project Setup**:
   - Create a new xUnit project named `MainSchoolsManagementSystem.Tests` at the solution level.
   - Reference `MainSchoolsManagementSystem.csproj`.
   - Add packages: `Microsoft.AspNetCore.Mvc.Testing`, `Microsoft.EntityFrameworkCore.Sqlite`, and `Moq`.
   - Add the test project to the solution `SchoolsManagementSystem.sln`.
   - Set up `CustomWebApplicationFactory<Program>` (make sure `Program` class is accessible, you may need to add `public partial class Program { }` at the end of `Program.cs` if it's not already there).
   - In the factory, configure SQLite In-Memory database, run `EnsureCreated()`, and register a custom `TestAuthHandler` that reads `X-Test-UserId`, `X-Test-Role`, and `X-Test-SchoolId` headers.
   - In the factory, override the uploads directory to a temporary path, and clean it up on dispose.

4. **Write Integration Tests**:
   - Create `LessonPlansApiTests.cs` in `MainSchoolsManagementSystem.Tests`.
   - Write tests for the 5 scenarios:
     1. Unauthenticated user -> `401 Unauthorized`
     2. Teacher who owns the plan -> `200 OK`
     3. Teacher who does not own the plan -> `403 Forbidden`
     4. Headmaster from the same school -> `200 OK`
     5. Headmaster from a different school -> `403 Forbidden`
     6. Justification download for a plan with/without justification.
   - Make sure to write dummy files to the temp uploads directory in the test setup.

5. **Build & Test**:
   - Run the build and run `dotnet test` to verify that all tests compile and pass.
   - Document the commands used and the test execution results in your handoff report.

MANDATORY INTEGRITY WARNING:
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A Forensic Auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

Write your report to handoff.md in your working directory and notify the orchestrator (conversation ID: 1e7e10d5-bf84-4a21-b975-37a79f543488).

## 2026-06-29T23:43:10Z

Your working directory is c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\worker_m1\. Read your task in task.md there. Implement the DB Schema Extension. Run the build and migrations, verify that the database updates successfully, and write your changes report to changes.md and a handoff to handoff.md in your working directory. Send a message back to me (conv ID: 3e6f64c1-4c0a-4057-a762-bcf3869ac3e4) when done.

MANDATORY INTEGRITY WARNING:
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A Forensic Auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

