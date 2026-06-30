# BRIEFING — 2026-06-29T23:41:00Z

## Mission
Investigate the SchoolsManagementSystem codebase (DbContext, SystemSetting queries, authentication/teacher/school ID retrieval) to help design the LessonPlans feature.

## 🔒 My Identity
- Archetype: explorer
- Roles: teamwork_preview_explorer
- Working directory: c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\explorer_lessonplans_1
- Original parent: 950031d0-32cb-4ebb-babb-88ddedded6c1
- Milestone: LessonPlans Feature Investigation

## 🔒 Key Constraints
- Read-only investigation — do NOT implement
- Work only in designated agent directory for outputting reports.

## Current Parent
- Conversation ID: 950031d0-32cb-4ebb-babb-88ddedded6c1
- Updated: not yet

## Investigation State
- **Explored paths**:
  - `MainSchoolsManagementSystem/Data/ApplicationDbContext.cs`
  - `MainSchoolsManagementSystem/Data/SystemSetting.cs`
  - `MainSchoolsManagementSystem/Data/ApplicationUser.cs`
  - `MainSchoolsManagementSystem/Data/LessonPlan.cs`
  - `MainSchoolsManagementSystem/Components/Pages/Admin/Settings.razor`
  - `MainSchoolsManagementSystem/Components/Pages/Headmaster/Settings.razor`
  - `MainSchoolsManagementSystem/Components/Pages/Headmaster/Attendance.razor`
  - `MainSchoolsManagementSystem/Components/Pages/TeacherDashboard.razor`
- **Key findings**:
  1. `ApplicationDbContext` is the database context. In Blazor pages, both `IDbContextFactory<ApplicationDbContext>` (with short-lived `using` blocks) and direct `ApplicationDbContext` injection are used.
  2. `SystemSetting` is a single global record representing system configurations (such as `DailyDeadline`). Seeding ensures exactly one record exists, and it's queried via `FirstOrDefaultAsync()` without ID filtering.
  3. Logged-in teacher ID (string) and School ID (int?) are retrieved using `AuthenticationStateProvider` to get the claims principal and `UserManager<ApplicationUser>` to fetch the corresponding `ApplicationUser`.
- **Unexplored areas**: None.

## Key Decisions Made
- Recommended using `IDbContextFactory<ApplicationDbContext>` for the new `Teacher/LessonPlans.razor` page to avoid concurrency issues during file uploads.

## Artifact Index
- `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\explorer_lessonplans_1\analysis.md` — Detailed analysis of DbContext, SystemSettings, and Authentication/Identity retrieval.
- `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\explorer_lessonplans_1\handoff.md` — Handoff report.
