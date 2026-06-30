# BRIEFING — 2026-06-30T06:11:18Z

## Mission
Explore the codebase and design the Minimal API endpoints for secure download of lesson plans and justifications.

## 🔒 My Identity
- Archetype: Explorer
- Roles: Explorer 1
- Working directory: c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\explorer_m1_1
- Original parent: 1e7e10d5-bf84-4a21-b975-37a79f543488
- Milestone: Milestone 1

## 🔒 Key Constraints
- Read-only investigation — do NOT implement
- Design Minimal API endpoints: GET /api/lesson-plans/{id}/download and GET /api/lesson-plans/{id}/justification/download
- Detail authorization and file resolution logic

## Current Parent
- Conversation ID: 1e7e10d5-bf84-4a21-b975-37a79f543488
- Updated: 2026-06-30T06:11:18Z

## Investigation State
- **Explored paths**:
  - `MainSchoolsManagementSystem/Program.cs` (Routing, middleware, DI)
  - `MainSchoolsManagementSystem/Data/LessonPlan.cs` (Model structure, properties)
  - `MainSchoolsManagementSystem/Data/ApplicationUser.cs` (User properties, SchoolId, DepartmentId)
  - `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs` (DbSets and relationships)
  - `MainSchoolsManagementSystem/Data/DbSeeder.cs` (Seed data, roles: Admin, Director, Headmaster, Officer, Teacher, Assistant)
  - `MainSchoolsManagementSystem/Components/Account/IdentityComponentsEndpointRouteBuilderExtensions.cs` (Example endpoint mapping, user retrieval, file downloading)
- **Key findings**:
  - Minimal API endpoints can be mapped in `Program.cs` before `app.Run()`.
  - Authentication can be enforced using `.RequireAuthorization()`.
  - User ID and roles can be retrieved from `ClaimsPrincipal`.
  - `SchoolId` must be retrieved by querying the database (via `ApplicationDbContext` from `IDbContextFactory`).
  - File scanning can be done using `Directory.EnumerateFiles` with a wildcard in `uploads/` combined with `IWebHostEnvironment.ContentRootPath`.
  - MIME types can be resolved via `FileExtensionContentTypeProvider`.
- **Unexplored areas**: None.

## Key Decisions Made
- Design the endpoints using ASP.NET Core Minimal APIs with `ClaimsPrincipal`, `IDbContextFactory<ApplicationDbContext>`, and `IWebHostEnvironment` injection.
- Implement robust authorization logic that handles multiple roles and handles the `null` `SchoolId` case for global Admins explicitly.

## Artifact Index
- `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\explorer_m1_1\handoff.md` — Detailed design and strategy report.
